using System.Linq;
using System.Net.Http.Formatting;
using System.Web.Http;
using ApiMultiPartFormData;
using Newtonsoft.Json.Serialization;

namespace Cv_Management
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration options)
        {
            // Register DI.
            AutofacConfig.Register(options);

            // Web API routes
            options.MapHttpAttributeRoutes();

            options.Routes.MapHttpRoute(
                "DefaultApi",
                "api/{controller}/{id}",
                new {id = RouteParameter.Optional}
            );

            var jsonFormatter = options.Formatters.OfType<JsonMediaTypeFormatter>().First();
            jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            options.Formatters.Add(new MultipartFormDataFormatter());
        }
    }
}