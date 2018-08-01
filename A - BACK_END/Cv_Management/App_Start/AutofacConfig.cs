using System.Data.Entity;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using AutoMapper;
using AutoMapper.Configuration;
using Cv_Management.Models.Entities.Context;

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
            builder.RegisterApiControllers(typeof(WebApiApplication).Assembly);
            builder.RegisterWebApiFilterProvider(httpConfiguration);

            #endregion

            #region Database context

            builder.RegisterType<CvManagementDbContext>().As<DbContext>().InstancePerLifetimeScope();

            #endregion

            #region Database context

            #endregion

            var containerBuilder = builder.Build();
            httpConfiguration.DependencyResolver = new AutofacWebApiDependencyResolver(containerBuilder);
        }
    }
}