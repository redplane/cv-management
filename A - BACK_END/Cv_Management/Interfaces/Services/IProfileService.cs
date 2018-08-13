﻿using System.Collections.Generic;
using System.Net.Http;
using Cv_Management.Models;

namespace Cv_Management.Interfaces.Services
{
    public interface IProfileService
    {
        #region Methods

        /// <summary>
        ///     Get profile from Http request.
        /// </summary>
        /// <param name="httpRequestMessage"></param>
        /// <returns></returns>
        ProfileModel GetProfile(HttpRequestMessage httpRequestMessage);

        /// <summary>
        ///     Set profile to http request.
        /// </summary>
        /// <param name="httpRequestMessage"></param>
        /// <param name="profile"></param>
        void SetProfile(HttpRequestMessage httpRequestMessage, ProfileModel profile);

        /// <summary>
        ///     Hash password into md5.
        /// </summary>
        /// <param name="originalPassword"></param>
        /// <returns></returns>
        string HashPassword(string originalPassword);

        #endregion
    }
}