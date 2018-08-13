﻿using System.Linq;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Http.Cors;
using ApiMultiPartFormData;
using Cv_Management.Attributes;
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
            
            //Cors 
            options.EnableCors(new EnableCorsAttribute("*","*", "*"));

            // Register filters.
            ApiFilterConfig.Register(options);
            
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