using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using Cv_Management.Entities.Context;

namespace Cv_Management.Attribuites
{
    public class ApiRoleAttribute:AuthorizeAttribute
    {
        public readonly CvManagementDbContext DbSet;
        public string[] _roles;
        public ApiRoleAttribute(string[] roles)
        {
            _roles = roles;
            DbSet = new CvManagementDbContext();
        }

        public override void OnAuthorization(HttpActionContext context)
        {

            
        }
    }
}