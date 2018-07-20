using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Cv_Management.Entities;
using Cv_Management.Entities.Context;
using Cv_Management.ViewModel;
using Cv_Management.ViewModel.User;

namespace Cv_Management.Controllers
{
    [RoutePrefix("api/user")]
    public class ApiUserController : ApiController
    {
        #region Properties

        public readonly DbCvManagementContext DbSet;

        #endregion

        #region Constructors

        public ApiUserController()
        {
            DbSet = new DbCvManagementContext();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get users using specific conditions.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("search")]
        public async Task<IHttpActionResult> GellAll([FromBody] SearchUserViewModel model)
        {
            var result = new List<ReadingUserViewModel>();
            var users = DbSet.Users.AsQueryable();
            model = model ?? new SearchUserViewModel();
            if (model.Ids != null)
            {
                var ids = model.Ids.Where(x => x > 0).ToList();
                if (ids.Count > 0)
                    users = users.Where(x => ids.Contains(x.Id));

            }

            if (model.LastNames!=null)
                users = users.Where(c => model.LastNames.Contains(c.LastName));

            if (model.FirstNames != null)
                users = users.Where(c => model.FirstNames.Contains(c.FirstName));

            if (model.Birthday >0 )
                users = users.Where(c => c.Birthday.Equals(model.Birthday));

            var results = new SearchResultViewModel<IList<User>>();
            results.Total = await users.CountAsync();
            var pagination = model.Pagination;
            if (pagination != null)
            {
                if (pagination.Page < 1)
                    pagination.Page = 1;
                users = users.Skip((pagination.Page - 1) * pagination.Records)
                    .Take(pagination.Records);
            }
            results.Records = await users.ToListAsync();
            return Ok(result);
        }
        /// <summary>
        /// Create User
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> Create([FromBody] CreateUserViewModel model)
        {
            if (model == null)
            {
                model = new CreateUserViewModel();
                Validate(model);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = new User();
            MappingData(user, model);

            user = DbSet.Users.Add(user);
            await DbSet.SaveChangesAsync();
            return Ok(user);
        }
        /// <summary>
        /// Update User
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{id}")]
        public async Task<IHttpActionResult> Update([FromUri] int id, [FromBody] UpdateUserViewModel model)
        {
            var user = DbSet.Users.Find(id);
            if (user == null)
                return NotFound();
            if (model == null)
            {
                model = new UpdateUserViewModel();
                Validate(model);
            }
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            MappingDataForUpdate(user, model);

            await DbSet.SaveChangesAsync();
            return Ok(new ReadingUserViewModel(user));
        }
        /// <summary>
        /// Delete User using Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id}")]
        public IHttpActionResult Delete([FromUri]int id)
        {
            return Ok();
        }
        /// <summary>
        /// Mapping data from Entity to model for create
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="model"></param>
        public void MappingData(User entity, CreateUserViewModel model)
        {
            entity.FirstName = model.FirstName;
            entity.LastName = model.LastName;
            entity.Birthday = model.Birthday;
            if (model.Photo != null)
                entity.Photo = Convert.ToBase64String(model.Photo.Buffer);
            entity.Role = model.Role;
            entity.Email = model.Email;
            entity.Password = model.Password;
        }
        /// <summary>
        /// Mapping data from entity to model for update
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="model"></param>
        public void MappingDataForUpdate(User entity, UpdateUserViewModel model)
        {
            entity.FirstName = model.FirstName;
            entity.LastName = model.LastName;
            entity.Birthday = model.Birthday;
            if (model.Photo != null)
                entity.Photo = Convert.ToBase64String(model.Photo.Buffer);
            entity.Role = model.Role;
        }

        #endregion
    }
}