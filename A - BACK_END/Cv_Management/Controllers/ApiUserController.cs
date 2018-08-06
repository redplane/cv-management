using ApiClientShared.Enums.SortProperties;
using ApiClientShared.ViewModel;
using ApiClientShared.ViewModel.User;
using Cv_Management.Interfaces.Services;
using DbEntity.Models.Entities;
using DbEntity.Models.Entities.Context;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.UI.WebControls;

namespace Cv_Management.Controllers
{
    [RoutePrefix("api/user")]
    public class ApiUserController : ApiController
    {
        #region Properties
        /// <summary>
        /// Context to access to database
        /// </summary>
        private readonly CvManagementDbContext _dbContext;

        /// <summary>
        /// Service to handler database operation
        /// </summary>
        private readonly IDbService _dbService;
        #endregion

        #region Contructors
        /// <summary>
        /// Initalize controller with Injectors
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="dbService"></param>
        public ApiUserController(CvManagementDbContext dbContext,
            IDbService dbService)
        {
            _dbContext = dbContext;
            _dbService = dbService;

        }
        #endregion

        #region Methods
        /// <summary>
        /// Get users using specific conditions
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        [Route("search")]
        [HttpPost]
        public async Task<IHttpActionResult> Search([FromBody]SearchUserViewModel condition)
        {
            if (condition == null)
            {
                condition = new SearchUserViewModel();
                Validate(condition);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var users = _dbContext.Users.AsQueryable();

            if (condition.Ids != null)
            {
                var ids = condition.Ids.Where(c => c > 0).ToList();
                if (ids.Count > 0)
                    users = users.Where(c => condition.Ids.Contains(c.Id));
            }

            if (condition.LastNames != null)
            {
                var lastNames = condition.LastNames.Where(c => !string.IsNullOrEmpty(c));
                users = users.Where(c => condition.LastNames.Contains(c.LastName));
            }

            if (condition.FirstNames != null)
            {
                var firstNames = condition.FirstNames.Where(c => !string.IsNullOrEmpty(c));
                users = users.Where(c => condition.FirstNames.Contains(c.FirstName));
            }

            if (condition.Birthday > 0)
                users = users.Where(c => c.Birthday == condition.Birthday);

            var result = new SearchResultViewModel<IList<User >>();
            result.Total = await users.CountAsync();

            //do sort
            users = _dbService.Sort(users,SortDirection.Ascending,UserSortProperty.Id);

            //do pagination

            users = _dbService.Paginate(users, condition.Pagination);

            result.Records = await users.ToListAsync();

            return Ok(result);

        }


        /// <summary>
        /// Add an user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("")]
        [HttpPost]
        public async Task<IHttpActionResult> AddUser([FromBody] AddUserViewModel model)
        {

            if(model == null)
            {
                model = new AddUserViewModel();
                Validate(model);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = new User();
            user.LastName = model.LastName;
            user.FirstName = model.FirstName;
            
            return Ok();
        }


        /// <summary>
        /// Edit an user
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("{id}")]
        [HttpPut]
        public async Task<IHttpActionResult> EditUser([FromUri]int id, [FromBody] EditUserViewModel model)
        {
            return Ok();
        }


        /// <summary>
        /// Delete an user
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}")]
        [HttpDelete]
        public async Task<IHttpActionResult> DeleteUser([FromUri] int id)
        {
            return Ok();
        }
        #endregion

    }
}