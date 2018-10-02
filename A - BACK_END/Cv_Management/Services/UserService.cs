using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ApiClientShared.Enums;
using AutoMapper;
using Cv_Management.Interfaces.Services;
using Cv_Management.Models;
using DbEntity.Interfaces;
using DbEntity.Models.Entities.Context;
using Microsoft.EntityFrameworkCore;

namespace Cv_Management.Services
{
    public class UserService : IUserService
    {
        #region Properties

        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        /// Profile caching service (email - profile model)
        /// </summary>
        private readonly IValueCacheService<string, ProfileModel> _profileCacheService;

        /// <summary>
        /// Profile service (later will be merged to this class)
        /// </summary>
        private readonly IProfileService _profileService;

        /// <summary>
        /// Mapper instance.
        /// </summary>
        private readonly IMapper _mapper;

        #endregion

        #region Constructors

        /// <summary>
        /// Initialize service with injectors.
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="profileCacheService"></param>
        /// <param name="profileService"></param>
        /// <param name="mapper"></param>
        public UserService(IUnitOfWork unitOfWork, IValueCacheService<string, ProfileModel> profileCacheService, IProfileService profileService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _profileCacheService = profileCacheService;
            _profileService = profileService;
            _mapper = mapper;
        }

        #endregion

        #region Methods

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ProfileModel> LoginAsync(string email, string password, CancellationToken cancellationToken)
        {
            // Find user by using email from cache first.
            var profile = _profileCacheService.Read(email);

            // Hash password by using md5.
            var hashedPassword = _profileService.HashPassword(password);

            // No profile has been found from cache. Find user from database.
            if (profile == null || string.IsNullOrWhiteSpace(profile.Password))
            {
                var users = _unitOfWork.Users.Search();
                users = users.Where(x =>
                    x.Email.Equals(email, StringComparison.InvariantCultureIgnoreCase) &&
                    x.Password.Equals(hashedPassword, StringComparison.InvariantCultureIgnoreCase) && x.Status == UserStatuses.Active);

                var user = await users.FirstOrDefaultAsync(cancellationToken);
                if (user == null)
                    return null;

                profile = _mapper.Map<ProfileModel>(user);
                return profile;
            }
            
            if (!profile.Password.Equals(hashedPassword, StringComparison.InvariantCultureIgnoreCase))
                return null;

            return profile;
        }

        #endregion
    }
}