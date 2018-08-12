using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using ApiClientShared.Enums;
using DbEntity.Models.Entities;
using DbEntity.Models.Entities.Context;
using JWT;

namespace Cv_Management.Attributes
{
    public class ApiRoleAttribute : AuthorizeAttribute
    {
        public readonly CvManagementDbContext _dbContext;
        public UserRoles _role;

        public ApiRoleAttribute(UserRoles role)
        {
            _role = role;
            _dbContext = new CvManagementDbContext();
        }

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            var user = actionContext.ActionArguments["User"] as User;

            try
            {
                if (user != null && user.Role == _role)
                    return;
                actionContext.Response = new HttpResponseMessage(HttpStatusCode.Forbidden);
            }
            catch (TokenExpiredException)
            {
                Console.WriteLine("Token has expired");
                throw;
            }
            catch (SignatureVerificationException)
            {
                Console.WriteLine("Token has invalid signature");
            }
        }
    }
}