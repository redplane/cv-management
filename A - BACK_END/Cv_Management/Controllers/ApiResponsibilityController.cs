using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.UI.WebControls;
using ApiClientShared.Enums.SortProperties;
using ApiClientShared.ViewModel;
using ApiClientShared.ViewModel.Responsibility;
using Cv_Management.Interfaces.Services;
using DbEntity.Models.Entities;
using DbEntity.Models.Entities.Context;

namespace Cv_Management.Controllers
{
    [RoutePrefix("api/responsibility")]
    public class ApiResponsibilityController : ApiController
    {
        #region Contructors

        public ApiResponsibilityController(DbContext dbContext,
            IDbService dbService)
        {
            _dbContext = (CvManagementDbContext) dbContext;
            _dbService = dbService;
        }

        #endregion

        #region Properties 

        /// <summary>
        ///     Context to access to database
        /// </summary>
        private readonly CvManagementDbContext _dbContext;

        /// <summary>
        ///     Service to handler database operation
        /// </summary>
        private readonly IDbService _dbService;

        #endregion

        #region Methods

        /// <summary>
        ///     Get Responsibilities using specific conditions
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("search")]
        [AllowAnonymous]
        public async Task<IHttpActionResult> Search([FromBody] SearchResponsibilityViewModel condition)
        {
            if (condition == null)
            {
                condition = new SearchResponsibilityViewModel();
                Validate(condition);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var responsibilities = _dbContext.Responsibilities.AsQueryable();
            if (condition.Ids != null)
            {
                var ids = condition.Ids.Where(x => x > 0).ToList();

                if (ids.Count > 0)
                    responsibilities = responsibilities.Where(x => ids.Contains(x.Id));
            }

            if (condition.Names != null)
            {
                var names = condition.Names.Where(c => !string.IsNullOrEmpty(c)).ToList();

                if (names.Count > 0)
                    responsibilities = responsibilities.Where(c => names.Contains(c.Name));
            }

            if (condition.CreatedTime != null)
                responsibilities = responsibilities.Where(c => c.CreatedTime >= condition.CreatedTime.From
                                                               && c.CreatedTime <= condition.CreatedTime.To);

            if (condition.LastModifiedTime != null)
                responsibilities = responsibilities.Where(c => c.LastModifiedTime >= condition.LastModifiedTime.From
                                                               && c.LastModifiedTime <= condition.LastModifiedTime.To);

            var result = new SearchResultViewModel<IList<Responsibility>>();
            result.Total = await responsibilities.CountAsync();

            // Sort
            responsibilities =
                _dbService.Sort(responsibilities, SortDirection.Ascending, ResponsibilitySortProperty.Id);

            // Paging
            responsibilities = _dbService.Paginate(responsibilities, condition.Pagination);

            result.Records = await responsibilities.ToListAsync();
            return Ok(result);
        }

        /// <summary>
        ///     Add an responsibility
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> Create([FromBody] AddResponsibilityViewModel model)
        {
            if (model == null)
            {
                model = new AddResponsibilityViewModel();
                Validate(model);
            }
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //Check exists responsibility
            var isExists = await _dbContext.Responsibilities.AnyAsync(c => c.Name == model.Name);
            if (isExists)
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.Conflict, "EXISTS_CODE_ERROR"));

            //Inital responsibility object
            var responsibility = new Responsibility();
            responsibility.Name = model.Name;
            responsibility.CreatedTime = DateTime.Now.ToOADate();

            //Add responsibility to database
            responsibility = _dbContext.Responsibilities.Add(responsibility);

            //Save changes to database
            await _dbContext.SaveChangesAsync();

            return Ok(responsibility);
        }

        /// <summary>
        ///     Edit an responsibility
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{id}")]
        public async Task<IHttpActionResult> Update([FromUri] int id, [FromBody] EditResponsibilityViewModel model)
        {
            if (model == null)
            {
                model = new EditResponsibilityViewModel();
                Validate(model);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //find Responsibility from database
            var responsibility = await _dbContext.Responsibilities.FindAsync(id);
            if (responsibility == null)
                return NotFound();

            if (!string.IsNullOrEmpty(model.Name))
                responsibility.Name = model.Name;
            responsibility.LastModifiedTime = DateTime.Now.ToOADate();

            //Save changes to database
            await _dbContext.SaveChangesAsync();

            return Ok(responsibility);
        }

        /// <summary>
        ///     Delete responsibility
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id}")]
        public async Task<IHttpActionResult> Delete([FromUri] int id)
        {
            //Find responsibility in database
            var responsibility = await _dbContext.Responsibilities.FindAsync(id);
            if (responsibility == null)
                return NotFound();

            //Delete responsibility from database
            _dbContext.Responsibilities.Remove(responsibility);

            //Save changes in database
            await _dbContext.SaveChangesAsync();
            return Ok();
        }

        #endregion
    }
}