using System.Web.Http;
using Cv_Management.Attributes;

namespace Cv_Management
{
    public class ApiFilterConfig
    {
        /// <summary>
        /// Register filters into configuration.
        /// </summary>
        /// <param name="httpConfiguration"></param>
        public static void Register(HttpConfiguration httpConfiguration)
        {
            httpConfiguration.Filters.Add(new BearerAuthenticationFilter());
            httpConfiguration.Filters.Add(new ApiAuthorizeAttribute());
        }
    }
}