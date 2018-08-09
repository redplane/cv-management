using System.Configuration;
using System.Data.Entity;
using System.Net.Http;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using AutoMapper;
using AutoMapper.Configuration;
using Cv_Management.Interfaces.Services;
using Cv_Management.Models;
using Cv_Management.Services;
using DbEntity.Models.Entities.Context;

namespace Cv_Management
{
    public class AutofacConfig
    {
        public static void Register(HttpConfiguration httpConfiguration)
        {
            var builder = new ContainerBuilder();
            builder.RegisterApiControllers();

            #region Automapper

            var options = new MapperConfigurationExpression();
            //options.CreateMap<SkillCategory, SkillCategoryModel>();

            //builder.Register(c => options)
            //    .AsImplementedInterfaces()
            //    .SingleInstance();

            builder.Register(c => c.Resolve<IConfigurationProvider>().CreateMapper())
                .As<IMapper>();

            #endregion

            #region Controllers & hubs

            // Controllers & hubs
            builder.RegisterApiControllers(typeof(Startup).Assembly);
            builder.RegisterWebApiFilterProvider(httpConfiguration);

            #endregion

            #region Database context

            builder.RegisterType<CvManagementDbContext>().As<DbContext>().InstancePerLifetimeScope();

            #endregion

            #region Model

            var appSettings = FindAppSettings();
            builder.RegisterInstance(appSettings);

            #endregion

            #region Services

            builder.RegisterType<DbService>().As<IDbService>().InstancePerLifetimeScope();
            builder.RegisterType<ProfileService>().As<IProfileService>().InstancePerLifetimeScope();
            builder.Register(c => new HttpClient()).As<HttpClient>().SingleInstance();
            builder.RegisterType<GoogleCaptchaService>().As<ICaptchaService>().SingleInstance();
            builder.RegisterType<TokenService>().As<ITokenService>().SingleInstance();
            #endregion

            var containerBuilder = builder.Build();
            httpConfiguration.DependencyResolver = new AutofacWebApiDependencyResolver(containerBuilder);
        }

        /// <summary>
        /// Find app settings.
        /// </summary>
        /// <returns></returns>
        private static AppSettingModel FindAppSettings()
        {
            var appSettingModel = new AppSettingModel();
            appSettingModel.GCaptchaSecret = ConfigurationManager.AppSettings[nameof(AppSettingModel.GCaptchaSecret)];
            appSettingModel.GCaptchaValidationEndpoint = ConfigurationManager.AppSettings[nameof(AppSettingModel.GCaptchaValidationEndpoint)];

            return appSettingModel;
        }

    }
}