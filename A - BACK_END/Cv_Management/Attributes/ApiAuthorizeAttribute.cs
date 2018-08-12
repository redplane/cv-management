using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using Cv_Management.Interfaces.Services;
using DbEntity.Models.Entities.Context;
using JWT;

namespace Cv_Management.Attributes
{
    public class ApiAuthorizeAttribute : AuthorizeAttribute
    {
        private readonly CvManagementDbContext _dbContext;
        private readonly ITokenService _tokenService;


        public ApiAuthorizeAttribute(CvManagementDbContext dbContext,
            ITokenService tokenService)
        {
            _dbContext = dbContext;
            _tokenService = tokenService;
        }

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            var token = actionContext.Request.Headers.Authorization.Parameter;
            try
            {
                //Get user from token
                var decodeResult = _tokenService.Decode(token);

                var email = decodeResult.FirstOrDefault().Value;
                var user = _dbContext.Users.FirstOrDefault(c => c.Email == email);
                if (user == null)
                    actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);

                actionContext.ActionArguments["User"] = user;
            }
            catch (TokenExpiredException)
            {
                Console.WriteLine("Token has expired");
            }
            catch (SignatureVerificationException)
            {
                Console.WriteLine("Token has invalid signature");
            }
        }
    }
}