using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Routing;
using System.Web.UI.WebControls;
using ApiClientShared.Enums;
using ApiClientShared.Enums.SortProperties;
using ApiClientShared.Resources;
using ApiClientShared.ViewModel;
using ApiClientShared.ViewModel.Hobby;
using ApiClientShared.ViewModel.Project;
using CvManagement.Interfaces.Services;
using CvManagement.Interfaces.Services.Businesses;
using DbEntity.Interfaces;
using DbEntity.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CvManagement.Services.Businesses
{
    public class HobbyService: IHobbyService
    {
        #region Properties

        private readonly IUnitOfWork _unitOfWork;

        private readonly HttpRequestMessage _httpRequestMessage;

        private readonly UrlHelper _urlHelper;

        private readonly IProfileService _profileService;

        private IDbService _dbService;

        #endregion

        #region Constructor

        /// <summary>
        /// Initialize service with injectors.
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="httpRequestMessage"></param>
        /// <param name="urlHelper"></param>
        /// <param name="profileService"></param>
        /// <param name="dbService"></param>
        public HobbyService(IUnitOfWork unitOfWork, HttpRequestMessage httpRequestMessage, UrlHelper urlHelper, IProfileService profileService, IDbService dbService)
        {
            _unitOfWork = unitOfWork;
            _httpRequestMessage = httpRequestMessage;
            _urlHelper = urlHelper;
            _profileService = profileService;
            _dbService = dbService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<Hobby> AddHobbyAsync(AddHobbyViewModel model, CancellationToken cancellationToken = default(CancellationToken))
        {
            // Get profile information.
            var profile = _profileService.GetProfile(_httpRequestMessage);

            var hobby = new Hobby();
            hobby.Name = model.Name;
            hobby.UserId = profile.Role == UserRoles.Admin && model.UserId != null ? model.UserId.Value : profile.Id;
            hobby.Description = model.Description;

            //Add to db context
            _unitOfWork.Hobbies.Insert(hobby);
            await _unitOfWork.CommitAsync();
            return hobby;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<Hobby> EditHobbyAsync(int id, EditHobbyViewModel model, CancellationToken cancellationToken = default(CancellationToken))
        {
            //Find hobby by id
            var hobbies = _unitOfWork.Hobbies.Search();
            var hobby = await hobbies.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (hobby == null)
                throw new HttpException((int) HttpStatusCode.NotFound, HttpMessages.HobbyNotFound);

            //Update hobby
            if (!string.IsNullOrEmpty(model.Name))
                hobby.Name = model.Name;

            if (!string.IsNullOrEmpty(model.Description))
                hobby.Description = model.Description;

            //Save change to database
            await _unitOfWork.CommitAsync();
            return hobby;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task DeleteHobbyAsync(int id, CancellationToken cancellationToken = default(CancellationToken))
        {
            //Find hobby
            var hobbies = _unitOfWork.Hobbies.Search();
            var hobby = await hobbies.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (hobby == null)
                throw new HttpException((int) HttpStatusCode.NotFound, HttpMessages.HobbyNotFound);

            // Remove hobby
            _unitOfWork.Hobbies.Remove(hobby);

            //Save change to db
            await _unitOfWork.CommitAsync();
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<SearchResultViewModel<IList<Hobby>>> SearchHobbiesAsync(SearchHobbyViewModel condition,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            // Get all hobbies.
            var hobbies = GetHobbies(condition);

            var result = new SearchResultViewModel<IList<Hobby>>();
            result.Total = await hobbies.CountAsync(cancellationToken);

            //Do sort
            hobbies = _dbService.Sort(hobbies, SortDirection.Ascending, SkillSortProperty.Id);

            //Do pagination
            hobbies = _dbService.Paginate(hobbies, condition.Pagination);

            result.Records = await hobbies.ToListAsync(cancellationToken);

            return result;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<Hobby> SearchHobbyAsync(SearchHobbyViewModel condition,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return await GetHobbies(condition).FirstOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// Get hobby using specific conditions.
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        protected virtual IQueryable<Hobby> GetHobbies(SearchHobbyViewModel condition)
        {
            // Get hobbies.
            var hobbies = _unitOfWork.Hobbies.Search();

            // Search for ids
            if (condition.Ids != null && condition.Ids.Any())
            {
                var ids = condition.Ids.Where(id => id > 0).ToList();
                if (ids.Any())
                    hobbies = hobbies.Where(hobby => ids.Contains(hobby.Id));
            }

            // Search for UserIds
            if (condition.UserIds != null && condition.UserIds.Any())
            {
                var userIds = condition.UserIds.Where(userId => userId > 0).ToList();
                if (userIds.Any())
                    hobbies = hobbies.Where(hobby => userIds.Contains(hobby.UserId));
            }

            // Search for names
            if (condition.Names != null && condition.Names.Any())
            {
                var names = condition.Names.Where(name => !string.IsNullOrEmpty(name)).ToList();
                if (names.Any())
                    hobbies = hobbies.Where(hobby => names.Contains(hobby.Name));
            }

            return hobbies;
        }

        #endregion
    }
}