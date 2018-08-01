using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Cv_Management.Models.Entities;
using Cv_Management.Models.Entities.Context;
using Cv_Management.ViewModel;
using Cv_Management.ViewModel.Responsibility;

namespace Cv_Management.Controllers
{
    [RoutePrefix("api/responsibility")]
    public class ApiResponsibilityController : ApiController
    {

        #region Properties 

        public readonly CvManagementDbContext DbSet;

        #endregion

        #region Contructors

        public ApiResponsibilityController()
        {
            DbSet = new CvManagementDbContext();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get Responsibilities using specific conditions
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> Search([FromBody]SearchResponsibilityViewModel model)
        {
            model = model ?? new SearchResponsibilityViewModel();
            var responsibilities = DbSet.Responsibilities.AsQueryable();
            if (model.Ids != null)
            {
                var ids = model.Ids.Where(x => x > 0).ToList();
                if (ids.Count > 0)
                    responsibilities = responsibilities.Where(x => ids.Contains(x.Id));

            }
            if (!string.IsNullOrEmpty(model.Name))
                responsibilities = responsibilities.Where(c => c.Name.Contains(model.Name));
            var result = new SearchResultViewModel<IList<Responsibility>>();
            result.Total = await responsibilities.CountAsync();
            var pagination = model.Pagination;
            if (pagination != null)
            {
                if (pagination.Page < 1)
                    pagination.Page = 1;
                responsibilities = responsibilities.Skip((pagination.Page - 1) * pagination.Records)
                    .Take(pagination.Records);
            }
            result.Records = await responsibilities.ToListAsync();
            return Ok(result);

        }

        /// <summary>
        /// Create responsibility
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> Create([FromBody]CreateResponsibilityViewModel model)
        {
            if (model == null)
            {
                model = new CreateResponsibilityViewModel();
                Validate(model);
            }
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var responsibility = new Responsibility();
            responsibility.Name = model.Name;
            responsibility.CreatedTime = DateTime.Now.ToOADate();
            responsibility = DbSet.Responsibilities.Add(responsibility);
           await  DbSet.SaveChangesAsync();
            return Ok(responsibility);

        }

        /// <summary>
        /// Update responsibility
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{id}")]
        public async Task<IHttpActionResult> Update([FromUri] int id, [FromBody]UpdateResponsibilityViewModel model)
        {
            if (model == null)
            {
                model = new UpdateResponsibilityViewModel();
                Validate(model);
            }
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            //get Responsibility
            var responsibility = DbSet.Responsibilities.Find(id);
            if (responsibility == null)
                return NotFound();
            responsibility.Name = model.Name;
            responsibility.LastModifiedTime = DateTime.Now.ToOADate();
          await  DbSet.SaveChangesAsync();
            return Ok(responsibility);

        }

        /// <summary>
        /// Delete responsibility
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id}")]
        public async Task<IHttpActionResult> Delete([FromUri]int id)
        {
            var responsibility = DbSet.Responsibilities.Find(id);
            if (responsibility == null)
                return NotFound();
            DbSet.Responsibilities.Remove(responsibility);
            await DbSet.SaveChangesAsync();
            return Ok();

        }

        #endregion

    }
}