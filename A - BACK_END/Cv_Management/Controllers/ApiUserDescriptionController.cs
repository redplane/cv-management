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
using Cv_Management.ViewModel.UserDescription;

namespace Cv_Management.Controllers
{
    [RoutePrefix("api/userDescription")]
    public class ApiUserDescriptionController : ApiController
    {
        #region Properties
        public readonly DbCvManagementContext DbSet;
        #endregion

        #region Contructors

        public ApiUserDescriptionController()
        {
            DbSet = new DbCvManagementContext();
        }

        #endregion

        #region  Methods
        /// <summary>
        /// Get user description using specific conditions 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> Search([FromBody]SearchUserDescriptionViewModel model)
        {
            model = model ?? new SearchUserDescriptionViewModel();
            var userDescriptions = DbSet.UserDescriptions.AsQueryable();
            if (model.Ids != null)
            {
                var ids = model.Ids.Where(x => x > 0).ToList();
                if (ids.Count > 0)
                    userDescriptions = userDescriptions.Where(x => ids.Contains(x.Id));

            }
            if (model.UserId > 0)
                userDescriptions = userDescriptions.Where(c => c.UserId == model.UserId);
            if (!string.IsNullOrEmpty(model.Description))
                userDescriptions = userDescriptions.Where(c => c.Description.Contains(model.Description));
            var results = new SearchResultViewModel<IList<UserDescription>>();
            results.Total = await userDescriptions.CountAsync();
            var pagination = model.Pagination;
            if (pagination != null)
            {
                if (pagination.Page < 1)
                    pagination.Page = 1;
                userDescriptions = userDescriptions.Skip((pagination.Page - 1) * pagination.Records)
                    .Take(pagination.Records);
            }
            results.Records = await userDescriptions.ToListAsync();
            return Ok(results);

        }
        /// <summary>
        /// Create User description
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        public async  Task<IHttpActionResult> Create([FromBody]CreateUserDescriptionViewModel model)
        {
            if (model == null)
            {
                model = new CreateUserDescriptionViewModel();
                Validate(model);
            }
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var userDescription = new UserDescription();
            userDescription.UserId = model.UserId;
            userDescription.Description = model.Description;
            userDescription = DbSet.UserDescriptions.Add(userDescription);
            await DbSet.SaveChangesAsync();
            return Ok(userDescription);

        }
        /// <summary>
        /// Update User description 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{id}")]
        public async Task<IHttpActionResult> Update([FromUri] int id, [FromBody]UpdateUserDescriptionViewModel model)
        {
            if (model == null)
            {
                model = new UpdateUserDescriptionViewModel();
                Validate(model);
            }
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            //get UserDescription
            var userDescription = DbSet.UserDescriptions.Find(id);
            if (userDescription == null)
                return NotFound();
            userDescription.UserId = model.UserId;
            userDescription.Description = model.Description;
            await DbSet.SaveChangesAsync();
            return Ok(userDescription);

        }
        /// <summary>
        /// Delete User Desciption from Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id}")]
        public async Task<IHttpActionResult> Delete([FromUri]int id)
        {
            var userDescription = DbSet.UserDescriptions.Find(id);
            if (userDescription == null)
                return NotFound();
            DbSet.UserDescriptions.Remove(userDescription);
           await DbSet.SaveChangesAsync();
            return Ok();

        }

        #endregion
    }
}