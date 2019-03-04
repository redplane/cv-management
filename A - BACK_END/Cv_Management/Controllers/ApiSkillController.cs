using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using ApiClientShared.ViewModel.Skill;
using CvManagement.Interfaces.Services.Businesses;

namespace CvManagement.Controllers
{
    [RoutePrefix("api/skill")]
    public class ApiSkillController : ApiController
    {
        #region Properties

        private readonly ISkillService _skillService;

        #endregion

        #region Contructors

        /// <summary>
        ///     Initialize controller with injectors
        /// </summary>
        /// <param name="skillService"></param>
        public ApiSkillController(ISkillService skillService)
        {
            _skillService = skillService;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Get skills using specific conditions
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("search")]
        [AllowAnonymous]
        public async Task<IHttpActionResult> Search([FromBody] SearchSkillViewModel condition)
        {
            if (condition == null)
            {
                condition = new SearchSkillViewModel();
                Validate(condition);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var loadSkillResult = await _skillService.SearchSkillsAsync(condition, CancellationToken.None);
            return Ok(loadSkillResult);
        }

        /// <summary>
        ///     Create skill
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> AddSkill(AddSkillViewModel model)
        {
            if (model == null)
            {
                model = new AddSkillViewModel();
                Validate(model);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var skill = await _skillService.AddSkillAsync(model, CancellationToken.None);
            return Ok(skill);
        }


        /// <summary>
        ///     Update skill
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{id}")]
        public async Task<IHttpActionResult> EditSkill([FromUri] int id, [FromBody] EditSkillViewModel model)
        {
            if (model == null)
            {
                model = new EditSkillViewModel();
                Validate(model);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var skill = await _skillService.EditSkillAsync(id, model, CancellationToken.None);
            return Ok(skill);
        }

        /// <summary>
        ///     Delete skill from id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id}")]
        public async Task<IHttpActionResult> DeleteSkill([FromUri] int id)
        {
            await _skillService.DeleteSkillAsync(id, CancellationToken.None);
            return Ok();
        }

        #endregion
    }
}