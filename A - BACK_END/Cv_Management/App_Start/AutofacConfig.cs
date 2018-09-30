using System;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Web.Hosting;
using System.Web.Http;
using ApiClientShared.ViewModel.SkillCategory;
using Autofac;
using Autofac.Features.AttributeFilters;
using Autofac.Integration.WebApi;
using AutoMapper;
using AutoMapper.Configuration;
using Cv_Management.Constant;
using Cv_Management.Interfaces.Services;
using Cv_Management.Models;
using Cv_Management.Services;
using Cv_Management.Services.CacheServices;
using DbEntity.Models.Entities;
using DbEntity.Models.Entities.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ServiceStack.Data;
using ServiceStack.Redis;

namespace Cv_Management
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

            builder.Register(c =>
                {
                    var dbContextOptionsBuilder = new DbContextOptionsBuilder<BaseCvManagementDbContext>();
                    var connectionString = ConfigurationManager.ConnectionStrings["CvManagement"].ConnectionString;
                    dbContextOptionsBuilder.UseSqlServer(connectionString)
                        .EnableSensitiveDataLogging()
                        .UseLoggerFactory(new LoggerFactory().AddConsole((category, level) => level == LogLevel.Information && category == DbLoggerCategory.Database.Command.Name, true));

                    var dbContext = new BaseCvManagementDbContext(dbContextOptionsBuilder.Options);
                    return dbContext;
                })
                .As<DbContext>()
                .InstancePerLifetimeScope();

            //builder.RegisterType<BaseCvManagementDbContext>()
            //    .As<DbContext>()
            //    .OnActivating(x =>
            //    {
            //        var dbContextOptionsBuilder = new DbContextOptionsBuilder<BaseCvManagementDbContext>();
            //        var connectionString = ConfigurationManager.ConnectionStrings["CvManagement"].ConnectionString;
            //        dbContextOptionsBuilder.UseSqlServer(connectionString)
            //            .EnableSensitiveDataLogging()
            //            .UseLoggerFactory(new LoggerFactory().AddConsole((category, level) => level == LogLevel.Information && category == DbLoggerCategory.Database.Command.Name, true));
                    
            //        var dbContext = new BaseCvManagementDbContext(dbContextOptionsBuilder.Options);
            //        x.ReplaceInstance(dbContext);
            //    })
            //    .InstancePerLifetimeScope();
           
            //builder.RegisterType<BaseCvManagementDbContext>().As<DbContext>()
            //    .OnActivating(x =>
            //    {
            //        var dbConnectionFactory = Effort.DbConnectionFactory.CreatePersistent(nameof(BaseCvManagementDbContext));
            //        var dbContext = new BaseCvManagementDbContext(dbConnectionFactory);
            //        x.ReplaceInstance(dbContext);
            //    })
            //    .SingleInstance();

            #endregion

            #region Model

            var appSettings = FindAppSettings();
            builder.RegisterInstance(appSettings);

            var appPaths = FindAppPathSettings();
            builder.RegisterInstance(appPaths);

            #endregion

            #region Services

            builder.RegisterType<DbService>().As<IDbService>().InstancePerLifetimeScope();
            builder.RegisterType<ProfileService>().As<IProfileService>().InstancePerLifetimeScope();
            builder.RegisterType<GoogleCaptchaService>().As<ICaptchaService>().InstancePerLifetimeScope();
            builder.RegisterType<TokenService>().As<ITokenService>().InstancePerLifetimeScope();
            builder.RegisterType<FileService>().As<IFileService>().InstancePerLifetimeScope();
            builder.Register(c => new HttpClient()).As<HttpClient>().SingleInstance();
            builder.RegisterType<ProfileCacheService>().As<IValueCacheService<string, ProfileModel>>().SingleInstance().WithAttributeFiltering();

            RegisterRedisCachingServices(ref builder);

            // Api services.
            builder.RegisterType<UserService>().As<IUserService>().InstancePerLifetimeScope();

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
            appSettingModel.GoogleCaptchaSecret = ConfigurationManager.AppSettings[nameof(AppSettingModel.GoogleCaptchaSecret)];
            appSettingModel.GoogleCaptchaValidationEndpoint =
                ConfigurationManager.AppSettings[nameof(AppSettingModel.GoogleCaptchaValidationEndpoint)];

            return appSettingModel;
        }

        /// <summary>
        /// Find app paths settings.
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
            if (!string.IsNullOrWhiteSpace(absoluteSkillCategoryImagePath) && !Directory.Exists(absoluteSkillCategoryImagePath))
                Directory.CreateDirectory(absoluteSkillCategoryImagePath);

            return appPath;
        }

        /// <summary>
        /// Register redis caching services.
        /// </summary>
        /// <param name="containerBuilder"></param>
        private static void RegisterRedisCachingServices(ref ContainerBuilder containerBuilder)
        {
            // Redis registration.
            var profileCachingRedisConnectionString = ConfigurationManager.AppSettings[$"Redis.{nameof(AutofacKeyConstant.ProfileRedisCaching)}"];
            if (string.IsNullOrWhiteSpace(profileCachingRedisConnectionString))
                throw new Exception("Access token - profile cache connection string is not found.");

            containerBuilder.Register(c => new RedisManagerPool(profileCachingRedisConnectionString))
                .Keyed<IRedisClientsManager>(AutofacKeyConstant.ProfileRedisCaching);
        }
    }
}