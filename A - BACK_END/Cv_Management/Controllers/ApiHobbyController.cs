using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using ApiClientShared.ViewModel.Hobby;
using CvManagement.Interfaces.Services;
using CvManagement.Interfaces.Services.Businesses;

namespace CvManagement.Controllers
{
    [RoutePrefix("api/hobby")]
    public class ApiHobbyController : ApiController
    {
        #region Properties

        private readonly IHobbyService _hobbyService;

        #endregion

        #region Contructors

        /// <summary>
        ///     Initialize controller with injectors.
        /// </summary>
        /// <param name="hobbyService"></param>
        public ApiHobbyController(IHobbyService hobbyService)
        {
            _hobbyService = hobbyService;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Get hobbies using specific condition
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("search")]
        [AllowAnonymous]
        public async Task<IHttpActionResult> Search([FromBody] SearchHobbyViewModel condition)
        {
            //Check model is null
            if (condition == null)
            {
                condition = new SearchHobbyViewModel();
                Validate(condition);
            }
            //Validate model
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var loadHobbiesResult = await _hobbyService.SearchHobbiesAsync(condition, CancellationToken.None);
            return Ok(loadHobbiesResult);
        }

        /// <summary>
        ///     Add an hobby
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> AddHobby([FromBody] AddHobbyViewModel model)
        {
            //Check null for model
            if (model == null)
            {
                model = new AddHobbyViewModel();
                Validate(model);
            }

            //Validate model
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var hobby = await _hobbyService.AddHobbyAsync(model, CancellationToken.None);

            return Ok(hobby);
        }

        /// <summary>
        ///     Edit an hobby
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{id}")]
        public async Task<IHttpActionResult> EditHobby([FromUri] int id, [FromBody] EditHobbyViewModel model)
        {
            //Check null for model
            if (model == null)
            {
                model = new EditHobbyViewModel();
                Validate(model);
            }

            //Validate model
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var hobby = await _hobbyService.EditHobbyAsync(id, model, CancellationToken.None);

            return Ok(hobby);
        }

        /// <summary>
        ///     Delete an hobby
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id}")]
        public async Task<IHttpActionResult> DeleteHobby([FromUri] int id)
        {
            await _hobbyService.DeleteHobbyAsync(id, CancellationToken.None);
            return Ok();
        }

        #endregion
    }
}