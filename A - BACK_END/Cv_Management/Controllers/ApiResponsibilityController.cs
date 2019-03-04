using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.UI.WebControls;
using ApiClientShared.Enums.SortProperties;
using ApiClientShared.Resources;
using ApiClientShared.ViewModel;
using ApiClientShared.ViewModel.Responsibility;
using CvManagement.Interfaces.Services;
using CvManagement.Interfaces.Services.Businesses;
using DbEntity.Interfaces;
using DbEntity.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CvManagement.Controllers
{
    [RoutePrefix("api/responsibility")]
    public class ApiResponsibilityController : ApiController
    {
        #region Contructors

        public ApiResponsibilityController(IResponsibilityService responsibilityService)
        {
            _responsibilityService = responsibilityService;
        }

        #endregion

        #region Properties 

        private readonly IResponsibilityService _responsibilityService;

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

            var loadResponsibilitiesResult =
                await _responsibilityService.SearchResponsibilitiesAsync(condition, CancellationToken.None);
            return Ok(loadResponsibilitiesResult);
        }

        /// <summary>
        ///     Add an responsibility
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> AddResponsibility([FromBody] AddResponsibilityViewModel model)
        {
            if (model == null)
            {
                model = new AddResponsibilityViewModel();
                Validate(model);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var responsibility = await _responsibilityService.AddResponsibilityAsync(model);
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

           var responsibility = await _responsibilityService.EditResponsibilityAsync(id, model);
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
            await _responsibilityService.DeleteResponsibilityAsync(id, CancellationToken.None);
            return Ok();
        }

        #endregion
    }
}