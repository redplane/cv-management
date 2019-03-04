using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Http.Routing;
using ApiClientShared.ViewModel.SkillCategory;
using Autofac;
using Autofac.Features.AttributeFilters;
using Autofac.Integration.WebApi;
using AutoMapper;
using AutoMapper.Configuration;
using CvManagement.Constant;
using CvManagement.Interfaces.Services;
using CvManagement.Interfaces.Services.Businesses;
using CvManagement.Models;
using CvManagement.Services;
using CvManagement.Services.Businesses;
using CvManagement.Services.CacheServices;
using DbEntity.Extensions;
using DbEntity.Interfaces;
using DbEntity.Models.Entities;
using DbEntity.Models.Entities.Context;
using DbEntity.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using ServiceStack.Caching;

namespace CvManagement
{
    public class AutofacConfig
    {
        public static void Register(HttpConfiguration httpConfiguration)
        {
            var builder = new ContainerBuilder();
            builder.RegisterApiControllers()
                .WithAttributeFiltering();

            #region Automapper

            var oMapperConfigurationExpression = new MapperConfigurationExpression();
            oMapperConfigurationExpression.CreateMap<User, ProfileModel>();
            oMapperConfigurationExpression.CreateMap<ProfileModel, User>();
            oMapperConfigurationExpression.CreateMap<SkillCategory, SkillCategoryViewModel>();
            oMapperConfigurationExpression.CreateMap<IQueryable<SkillCategory>, IQueryable<SkillCategoryViewModel>>();
            //var autoMapperOptions = new MapperConfiguration(options =>
            //{
            //    options.CreateMap<User, ProfileModel>();
            //    options.CreateMap<ProfileModel, User>();
            //    options.CreateMap<SkillCategory, SkillCategoryViewModel>();
            //    options.CreateMap<IQueryable<SkillCategory>, IQueryable<SkillCategoryViewModel>>();
            //});

            var autoMapperOptions = new MapperConfiguration(oMapperConfigurationExpression);
            var mapper = new Mapper(autoMapperOptions);
            Mapper.Initialize(oMapperConfigurationExpression);
            builder.RegisterInstance(mapper)
                .As<IMapper>()
                .SingleInstance();

            #endregion

            #region Controllers & hubs

            // Controllers & hubs
            builder.RegisterApiControllers(typeof(Startup).Assembly);
            builder.RegisterWebApiFilterProvider(httpConfiguration);

            #endregion

            #region Database context

#if !USE_IN_MEMORY
            builder.Register(c =>
                {
                    var dbContextOptionsBuilder = new DbContextOptionsBuilder<CvManagementDbContext>();
                    var connectionString = ConfigurationManager.ConnectionStrings["CvManagement"].ConnectionString;
                    dbContextOptionsBuilder.UseSqlServer(connectionString)
                        .EnableSensitiveDataLogging()
                        .UseLoggerFactory(new LoggerFactory().AddConsole(
                            (category, level) => level == LogLevel.Information &&
                                                 category == DbLoggerCategory.Database.Command.Name, true));

                    var dbContext = new CvManagementDbContext(dbContextOptionsBuilder.Options);
                    return dbContext;
                })
                .As<DbContext>()
                .InstancePerLifetimeScope();
#else
            builder.Register(c =>
                {
                    var dbContextOptionsBuilder = new DbContextOptionsBuilder<CvManagementInMemoryDbContext>();
                    dbContextOptionsBuilder.UseInMemoryDatabase(nameof(CvManagementInMemoryDbContext))
                        .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));

                    var dbContext = new CvManagementInMemoryDbContext(dbContextOptionsBuilder.Options);
                    dbContext.Seed();
                    return dbContext;
                })
                .As<DbContext>()
                .SingleInstance();
#endif
            #endregion

            #region Model

            var appSettings = FindAppSettings();
            builder.RegisterInstance(appSettings);

            var appPaths = FindAppPathSettings();
            builder.RegisterInstance(appPaths);

            #endregion

            #region Services

            builder.RegisterGeneric(typeof(Repository<>)).As(typeof(IRepository<>)).InstancePerLifetimeScope();
            builder.RegisterType<UnitOfWork>().As<IUnitOfWork>().InstancePerLifetimeScope();
            builder.RegisterType<DbService>().As<IDbService>().InstancePerLifetimeScope();
            builder.RegisterType<ProfileService>().As<IProfileService>().InstancePerLifetimeScope();
            builder.RegisterType<GoogleCaptchaService>().As<ICaptchaService>().InstancePerLifetimeScope();
            builder.RegisterType<TokenService>().As<ITokenService>().InstancePerLifetimeScope();
            builder.RegisterType<FileService>().As<IFileService>().InstancePerLifetimeScope();
            builder.Register(c => new HttpClient()).As<HttpClient>().SingleInstance();
            builder.RegisterHttpRequestMessage(httpConfiguration);
            builder.Register(x => new UrlHelper(x.Resolve<HttpRequestMessage>()));
            builder.RegisterType<ProfileCacheService>().As<IValueCacheService<string, ProfileModel>>().SingleInstance()
                .WithAttributeFiltering();

            RegisterRedisCachingServices(ref builder);

            // Api services.
            builder.RegisterType<UserService>().As<IUserService>().InstancePerLifetimeScope();
            builder.RegisterType<UserDescriptionService>().As<IUserDescriptionService>().InstancePerLifetimeScope();
            builder.RegisterType<ProjectService>().As<IProjectService>().InstancePerLifetimeScope();
            builder.RegisterType<HobbyService>().As<IHobbyService>().InstancePerLifetimeScope();
            builder.RegisterType<SkillService>().As<ISkillService>().InstancePerLifetimeScope();
            builder.RegisterType<ProjectSkill>().As<IProjectSkillService>().InstancePerLifetimeScope();
            builder.RegisterType<ProjectResponsibilityService>().As<IProjectResponsibilityService>().InstancePerLifetimeScope();
            builder.RegisterType<ResponsibilityService>().As<IResponsibilityService>().InstancePerLifetimeScope();
            builder.RegisterType<SkillCategoryService>().As<ISkillCategoryService>().InstancePerLifetimeScope();

            #endregion

            var containerBuilder = builder.Build();
            httpConfiguration.DependencyResolver = new AutofacWebApiDependencyResolver(containerBuilder);
        }

        /// <summary>
        ///     Find app settings.
        /// </summary>
        /// <returns></returns>
        private static AppSettingModel FindAppSettings()
        {
            var appSettingModel = new AppSettingModel();
            appSettingModel.GoogleCaptchaSecret =
                ConfigurationManager.AppSettings[nameof(AppSettingModel.GoogleCaptchaSecret)];
            appSettingModel.GoogleCaptchaValidationEndpoint =
                ConfigurationManager.AppSettings[nameof(AppSettingModel.GoogleCaptchaValidationEndpoint)];

            return appSettingModel;
        }

        /// <summary>
        ///     Find app paths settings.
        /// </summary>
        /// <returns></returns>
        private static AppPathModel FindAppPathSettings()
        {
            var appPath = new AppPathModel(HostingEnvironment.MapPath("~/"));
            appPath.ProfileImage = ConfigurationManager.AppSettings[$"Path.{nameof(appPath.ProfileImage)}"];
            appPath.SkillCategoryImage = ConfigurationManager.AppSettings[$"Path.{nameof(appPath.SkillCategoryImage)}"];

            if (string.IsNullOrWhiteSpace(appPath.ProfileImage))
                throw new Exception($"Invalid {nameof(appPath.ProfileImage)} setting");

            var absoluteProfileImagePath = HostingEnvironment.MapPath(appPath.ProfileImage);
            if (!string.IsNullOrWhiteSpace(absoluteProfileImagePath) && !Directory.Exists(absoluteProfileImagePath))
                Directory.CreateDirectory(absoluteProfileImagePath);

            var absoluteSkillCategoryImagePath = HostingEnvironment.MapPath(appPath.SkillCategoryImage);
            if (!string.IsNullOrWhiteSpace(absoluteSkillCategoryImagePath) &&
                !Directory.Exists(absoluteSkillCategoryImagePath))
                Directory.CreateDirectory(absoluteSkillCategoryImagePath);

            return appPath;
        }

        /// <summary>
        ///     Register redis caching services.
        /// </summary>
        /// <param name="containerBuilder"></param>
        private static void RegisterRedisCachingServices(ref ContainerBuilder containerBuilder)
        {
            // Redis registration.
            var profileCachingRedisConnectionString =
                ConfigurationManager.AppSettings[$"Redis.{nameof(AutofacKeyConstant.ProfileRedisCaching)}"];
            if (string.IsNullOrWhiteSpace(profileCachingRedisConnectionString))
                throw new Exception("Access token - profile cache connection string is not found.");

            //containerBuilder.Register(c => new RedisManagerPool(profileCachingRedisConnectionString).GetClient())
            //    .Keyed<IRedisClient>(AutofacKeyConstant.ProfileRedisCaching)
            //    .InstancePerLifetimeScope();

            containerBuilder.Register(c => new MemoryCacheClient())
                .Keyed<ICacheClient>(AutofacKeyConstant.ProfileRedisCaching)
                .InstancePerLifetimeScope();
        }
    }
}