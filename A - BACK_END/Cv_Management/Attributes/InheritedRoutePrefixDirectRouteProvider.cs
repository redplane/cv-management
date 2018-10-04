using System;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Routing;

namespace CvManagement.Attributes
{
    public class InheritedRoutePrefixDirectRouteProvider : DefaultDirectRouteProvider
    {
        //protected override string GetRoutePrefix(HttpControllerDescriptor controllerDescriptor)
        //{
        //    var sb = new StringBuilder(base.GetRoutePrefix(controllerDescriptor));
        //    var baseType = controllerDescriptor.ControllerType.BaseType;

        //    for (var t = baseType; typeof(ApiController).IsAssignableFrom(t); t = t.BaseType)
        //    {
        //        var a = (t as MemberInfo).GetCustomAttribute<RoutePrefixAttribute>(false);
        //        if (a != null)
        //        {
        //            sb.Insert(0, $"{a.Prefix}{(sb.Length > 0 ? "/" : "")}");
        //        }
        //    }

        //    return sb.ToString();
        //}
        protected override string GetRoutePrefix(HttpControllerDescriptor controllerDescriptor)
        {
            // Get the calling controller's route prefix
            var routePrefix = base.GetRoutePrefix(controllerDescriptor);

            // Iterate through each of the calling controller's base classes that inherit from HttpController
            var baseControllerType = controllerDescriptor.ControllerType.BaseType;
            if (baseControllerType == null)
                return base.GetRoutePrefix(controllerDescriptor);

            while (typeof(IHttpController).IsAssignableFrom(baseControllerType))
            {
                // Get the base controller's route prefix, if it exists
                // GOTCHA: There are two RoutePrefixAttributes... System.Web.Http.RoutePrefixAttribute and System.Web.Mvc.RoutePrefixAttribute!
                //  Depending on your controller implementation, either one or the other might be used... checking against typeof(RoutePrefixAttribute) 
                //  without identifying which one will sometimes succeed, sometimes fail.
                //  Since this implementation is generic, I'm handling both cases.  Preference would be to extend System.Web.Mvc and System.Web.Http
                var baseRoutePrefix = Attribute.GetCustomAttribute(baseControllerType, typeof(RoutePrefixAttribute))
                                      ?? Attribute.GetCustomAttribute(baseControllerType, typeof(RoutePrefixAttribute));
                if (baseRoutePrefix != null)
                {
                    // A trailing slash is added by the system. Only add it if we're prefixing an existing string
                    var trailingSlash = string.IsNullOrEmpty(routePrefix) ? "" : "/";
                    // Prepend the base controller's prefix
                    routePrefix = ((RoutePrefixAttribute)baseRoutePrefix).Prefix + trailingSlash + routePrefix;
                }

                // Traverse up the base hierarchy to check for all inherited prefixes
                baseControllerType = baseControllerType.BaseType;
            }

            return routePrefix;
        }
    }
}