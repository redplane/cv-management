using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.UI.WebControls;
using ApiClientShared.Enums.SortProperties;
using ApiClientShared.ViewModel;
using ApiClientShared.ViewModel.Hobby;
using Cv_Management.Interfaces.Services;
using DbEntity.Models.Entities;
using DbEntity.Models.Entities.Context;

namespace Cv_Management.Controllers
{
    [RoutePrefix("api/hobby")]
    public class ApiHobbyController : ApiController
    {
        #region Properties

        public readonly CvManagementDbContext _dbContext;

        public readonly IDbService _dbService;

        #endregion

        #region Contructors

        public ApiHobbyController(DbContext dbContext, IDbService dbService)
        {
            _dbContext =(CvManagementDbContext) dbContext;
            _dbService = dbService;

        }

        #endregion

        #region Methods
        /// <summary>
        /// Get hobbies using specific condition
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("search")]
        public async Task<IHttpActionResult> Search([FromBody]SearchHobbyViewModel condition)
        {
            //Check model is null
            if (condition == null)
            {
                condition = new  SearchHobbyViewModel();
                Validate(condition);
            }
            //Validate model
            if (!ModelState.IsValid)
                return BadRequest();

            var hobbies = _dbContext.Hobbies.AsQueryable();

            //Search for ids
            if (condition.Ids != null && condition.Ids.Any())
            {
                var ids = condition.Ids.Where(c => c > 0).ToList();
                if (ids.Any())
                    hobbies = hobbies.Where(c => ids.Contains(c.Id));
            }

            //Search for UserIds
            if (condition.UserIds != null && condition.UserIds.Any())
            {
                var userIds = condition.UserIds.Where(c => c > 0).ToList();
                if (userIds.Any())
                    hobbies = hobbies.Where(c => userIds.Contains(c.UserId));
            }

            //Search for names
            if (condition.Names != null && condition.Names.Any())
            {
                var names = condition.Names.Where(c => !string.IsNullOrEmpty(c)).ToList();
                if (names.Any())
                    hobbies = hobbies.Where(c => names.Contains(c.Name));
            }

            var result = new SearchResultViewModel<IList<Hobby>>();
            result.Total = await hobbies.CountAsync();

            //Do sort
            hobbies = _dbService.Sort(hobbies, SortDirection.Ascending, SkillSortProperty.Id);

            //Do pagination
            hobbies = _dbService.Paginate(hobbies, condition.Pagination);

            result.Records = await hobbies.ToListAsync();

            return Ok(result);
        }

        /// <summary>
        /// Add an hobby
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> AddHobby([FromBody]AddHobbyViewModel model)
        {
            //Check null for model
            if (model == null)
            {
                model = new AddHobbyViewModel();
                Validate(model);
            }

            //Validate model
            if (!ModelState.IsValid)
                return BadRequest();

            var hobby = new Hobby();
            hobby.Name = model.Name;
            hobby.UserId = model.UserId;
            hobby.Description = model.Description;

            //Add to db context
            _dbContext.Hobbies.Add(hobby);
          
            return Ok();
        }

        /// <summary>
        /// Edit an hobby
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{id}")]
        public async Task<IHttpActionResult> EditHobby([FromUri]int id,[FromBody]EditHobbyViewModel model)
        {
            return Ok();
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IHttpActionResult> DeleteHobby([FromUri]int id)
        {
            return Ok();
        }
        #endregion
    }
}