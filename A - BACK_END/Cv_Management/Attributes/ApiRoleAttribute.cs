using System.Web.Http;
using System.Web.Http.Controllers;
using Cv_Management.Models.Entities.Context;

namespace Cv_Management.Attributes
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