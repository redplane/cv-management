using System;
using Autofac.Features.AttributeFilters;
using Cv_Management.Constant;
using Cv_Management.Interfaces.Services;
using Cv_Management.Models;
using ServiceStack.Redis;

namespace Cv_Management.Services.CacheServices
{
    public class ProfileCacheService : ValueCacheBaseService<string, ProfileModel>
    {
        #region Properties

        /// <summary>
        /// Redis connection manager for access token caching.
        /// </summary>
        private readonly IRedisClientsManager _profileRedisClientManager;
        
        #endregion

        #region Constructors

        /// <summary>
        /// Initialize service with injectors.
        /// </summary>
        /// <param name="profileRedisClientManager"></param>
        public ProfileCacheService([KeyFilter(AutofacKeyConstant.ProfileRedisCaching)] IRedisClientsManager profileRedisClientManager)
        {
            _profileRedisClientManager = profileRedisClientManager;
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
            using (var redisClient = _profileRedisClientManager.GetClient())
            {
                var profiles = redisClient.As<ProfileModel>();
                if (expirationTime == null)
                    profiles.SetValue(key, value);
                else
                {
                    var expireIn = expirationTime.Value - DateTime.Now;
                    profiles.SetValue(key, value, expireIn);
                }
            }
        }

        /// <summary>
        /// Get profile information.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public override ProfileModel Read(string key)
        {
            using (var redis = _profileRedisClientManager.GetClient())
            {
                var profiles = redis.As<ProfileModel>();
                return profiles.GetValue(key);
            }
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