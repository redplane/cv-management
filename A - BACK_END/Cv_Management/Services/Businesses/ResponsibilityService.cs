using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Routing;
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

namespace CvManagement.Services.Businesses
{
    public class ResponsibilityService : IResponsibilityService
    {
        #region Properties

        private readonly IUnitOfWork _unitOfWork;

        private readonly HttpRequestMessage _httpRequestMessage;

        private readonly UrlHelper _urlHelper;

        private readonly IDbService _dbService;

        #endregion

        #region Constructors

        public ResponsibilityService(IUnitOfWork unitOfWork, HttpRequestMessage httpRequestMessage, UrlHelper urlHelper, IDbService dbService)
        {
            _unitOfWork = unitOfWork;
            _httpRequestMessage = httpRequestMessage;
            _urlHelper = urlHelper;
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
        public virtual async Task<Responsibility> AddResponsibilityAsync(AddResponsibilityViewModel model,
            CancellationToken cancellationToken = default(CancellationToken))
        {

            // Check exists responsibility
            var responsibilities = _unitOfWork.Responsibilities.Search();
            responsibilities = responsibilities.Where(x => x.Name.Equals(model.Name));
            var responsibility = await responsibilities.FirstOrDefaultAsync(cancellationToken);
            if (responsibility != null)
                throw new HttpException((int)HttpStatusCode.Conflict, HttpMessages.ResponsibilityAlreadyAvailable);

            //Inital responsibility object
            responsibility = new Responsibility();
            responsibility.Name = model.Name;
            responsibility.CreatedTime = DateTime.Now.ToOADate();

            //Add responsibility to database
            _unitOfWork.Responsibilities.Insert(responsibility);

            //Save changes to database
            await _unitOfWork.CommitAsync();

            return responsibility;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<Responsibility> EditResponsibilityAsync(int id, EditResponsibilityViewModel model,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            // Find Responsibility from database
            var responsibilities = _unitOfWork.Responsibilities.Search();
            var responsibility = await responsibilities.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (responsibility == null)
                throw new HttpException((int)HttpStatusCode.NotFound,
                    HttpMessages.ResponsibilityNotFound);

            if (!string.IsNullOrWhiteSpace(model.Name))
            {
                // Check name for duplication.
                var name = model.Name.Trim();
                if (await responsibilities.AnyAsync(x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase), cancellationToken))
                    throw new HttpException((int) HttpStatusCode.Conflict, HttpMessages.ResponsibilityAlreadyAvailable);
                responsibility.Name = model.Name;
            }

            responsibility.LastModifiedTime = DateTime.Now.ToOADate();

            //Save changes to database
            await _unitOfWork.CommitAsync();

            return responsibility;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task DeleteResponsibilityAsync(int id, CancellationToken cancellationToken = default(CancellationToken))
        {
            // Find responsibility in database
            var responsibilities = _unitOfWork.Responsibilities.Search();
            var responsibility = await responsibilities.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (responsibility == null)
                throw new HttpException((int) HttpStatusCode.NotFound, HttpMessages.ResponsibilityNotFound);

            //Delete responsibility from database
            _unitOfWork.Responsibilities.Remove(responsibility);

            //Save changes in database
            await _unitOfWork.CommitAsync();
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<SearchResultViewModel<IList<Responsibility>>> SearchResponsibilitiesAsync(SearchResponsibilityViewModel condition,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var responsibilities = GetResponsibilities(condition);
            var result = new SearchResultViewModel<IList<Responsibility>>();
            result.Total = await responsibilities.CountAsync(cancellationToken);

            // Sort
            responsibilities =
                _dbService.Sort(responsibilities, SortDirection.Ascending, ResponsibilitySortProperty.Id);

            // Paging
            responsibilities = _dbService.Paginate(responsibilities, condition.Pagination);

            result.Records = await responsibilities.ToListAsync(cancellationToken);
            return result;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<Responsibility> SearchResponsibilityAsync(SearchResponsibilityViewModel condition,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return await GetResponsibilities(condition).FirstOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// Search for responsibilities using specific conditions.
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        protected virtual IQueryable<Responsibility> GetResponsibilities(SearchResponsibilityViewModel condition)
        {
            var responsibilities = _unitOfWork.Responsibilities.Search();

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
            {
                var createdTime = condition.CreatedTime;
                var from = createdTime.From;
                var to = createdTime.To;

                if (from != null)
                    responsibilities = responsibilities.Where(x => x.CreatedTime >= from);

                if (to != null)
                    responsibilities = responsibilities.Where(x => x.CreatedTime <= to);
            }

            if (condition.LastModifiedTime != null)
            {
                var lastModifiedTime = condition.LastModifiedTime;
                var from = lastModifiedTime.From;
                var to = lastModifiedTime.To;

                if (from != null)
                    responsibilities = responsibilities.Where(x => x.LastModifiedTime >= from);

                if (to != null)
                    responsibilities = responsibilities.Where(x => x.LastModifiedTime <= to);
            }

            return responsibilities;
        }

        #endregion
    }
}