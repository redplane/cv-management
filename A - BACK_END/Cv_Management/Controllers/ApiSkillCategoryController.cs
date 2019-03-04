using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using ApiClientShared.Constants;
using ApiClientShared.Resources;
using ApiClientShared.ViewModel.SkillCategory;
using CvManagement.Interfaces.Services;
using CvManagement.Interfaces.Services.Businesses;
using CvManagement.ViewModels.SkillCategory;

namespace CvManagement.Controllers
{
    [RoutePrefix("api/skill-category")]
    public class ApiSkillCategoryController : ApiController
    {
        #region Contructors

        /// <summary>
        ///     Initialize controller with injectors.
        /// </summary>
        /// <param name="fileService"></param>
        /// <param name="skillCategoryService"></param>
        public ApiSkillCategoryController(IFileService fileService, ISkillCategoryService skillCategoryService)
        {
            _fileService = fileService;
            _skillCategoryService = skillCategoryService;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Service to handle files.
        /// </summary>
        private readonly IFileService _fileService;

        private readonly ISkillCategoryService _skillCategoryService;

        #endregion

        #region Methods

        /// <summary>
        ///     Get Skill category using specific conditions
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("search")]
        [AllowAnonymous]
        public async Task<IHttpActionResult> Search([FromBody] SearchSkillCategoryViewModel condition)
        {
            #region Parameters validation

            if (condition == null)
            {
                condition = new SearchSkillCategoryViewModel();
                Validate(condition);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            #endregion

            var loadSkillCategoriesResult = await _skillCategoryService.SearchSkillCategoriesAsync(condition);
            return Ok(loadSkillCategoriesResult);
        }

        /// <summary>
        ///     Create Skill category
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> AddSkillCategory([FromBody] AddSkillCategoryViewModel model)
        {
            //Check null for model
            if (model == null)
            {
                model = new AddSkillCategoryViewModel();
                Validate(model);
            }

            // Validate for model
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var skillCategory = await _skillCategoryService.AddSkillCategoryAsync(model);
            return Ok(skillCategory);
        }

        /// <summary>
        ///     Update skill category
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{id}")]
        public async Task<IHttpActionResult> EditSkillCategory([FromUri] int id,
            [FromBody] EditSkillCategoryViewModel model)
        {
            #region Parameter validation

            if (model == null)
            {
                model = new EditSkillCategoryViewModel();
                Validate(model);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            Image photo = null;
            if (model.Photo != null)
            {
                var bytes = model.Photo.Buffer;
                photo = _fileService.GetImage(bytes);
                if (photo.Height != ImageSizeConstant.StandardSkillCategoryImageSize ||
                    photo.Width != ImageSizeConstant.StandardSkillCategoryImageSize)
                {
                    ModelState.AddModelError($"{nameof(model)}.{nameof(model.Photo)}",
                        HttpMessages.SkillCategoryImageSizeInvalid);
                    return BadRequest(ModelState);
                }
            }

            #endregion

            var skillCategory = await _skillCategoryService.EditSkillCategoryAsync(id, model);
            return Ok(skillCategory);
        }

        /// <summary>
        ///     Delete skill category with Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id}")]
        public async Task<IHttpActionResult> DeleteSkillCategory([FromUri] int id)
        {
            await _skillCategoryService.DeleteSkillAsync(id, CancellationToken.None);
            return Ok();
        }

        #endregion
    }
}