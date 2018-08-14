using System.Web.Http;
using Cv_Management.Attributes;

namespace Cv_Management
{
    public class ApiFilterConfig
    {
        /// <summary>
        /// Register filters into configuration.
        /// NOTE: The position of filter will affect the application flow.
        /// Filter will be executed from top to bottom.
        /// </summary>
        /// <param name="httpConfiguration"></param>
        public static void Register(HttpConfiguration httpConfiguration)
        {
            httpConfiguration.Filters.Add(new ApiUnhandledExceptionFilter());
            httpConfiguration.Filters.Add(new BearerAuthenticationFilter());
            httpConfiguration.Filters.Add(new ApiAuthorizeAttribute());
        }
    }
}