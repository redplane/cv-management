using System;
using Autofac.Features.AttributeFilters;
using CvManagement.Constant;
using CvManagement.Models;
using ServiceStack.Caching;

namespace CvManagement.Services.CacheServices
{
    public class ProfileCacheService : ValueCacheBaseService<string, ProfileModel>
    {
        #region Properties

        /// <summary>
        /// Redis connection manager for access token caching.
        /// </summary>
        private readonly ICacheClient _redisClient;

        #endregion

        #region Constructors

        /// <summary>
        /// Initialize service with injectors.
        /// </summary>
        /// <param name="redisClient"></param>
        public ProfileCacheService([KeyFilter(AutofacKeyConstant.ProfileRedisCaching)] ICacheClient redisClient)
        {
            _redisClient = redisClient;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Add key-value to cache.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expirationTime"></param>
        public override void Add(string key, ProfileModel value, DateTime? expirationTime)
        {
            if (expirationTime == null)
                _redisClient.Set(key, value);
            else
            {
                var expireIn = expirationTime.Value - DateTime.Now;
                _redisClient.Set(key, value, expireIn);
            }
        }

        /// <summary>
        /// Get profile information.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public override ProfileModel Read(string key)
        {
            return _redisClient.Get<ProfileModel>(key);
        }

        /// <summary>
        /// Operation which is called when class is searching for a key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected override string FindKey(string key)
        {
            return key.ToLower();
        }

        #endregion
    }
}