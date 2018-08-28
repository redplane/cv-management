using System.Collections;
using System.Collections.Generic;
using System.Security.Claims;

namespace Cv_Management.Interfaces.Services
{
    public interface ITokenService
    {
        #region Methods

        /// <summary>
        ///     Get token from user information
        /// </summary>
        /// <param name="payload"></param>
        /// <returns></returns>
        string Encode(IDictionary payload);


        /// <summary>
        ///     Decode token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        T Decode<T>(string token);

        /// <summary>
        /// Validate & decode token.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="token"></param>
        /// <param name="bValidate"></param>
        /// <returns></returns>
        T Decode<T>(string token, bool bValidate);

        /// <summary>
        /// Decode a token and initialize a claim principle which can be attached to request.
        /// </summary>
        /// <param name="token"></param>
        ClaimsPrincipal ToPrinciple(string token);


        #endregion
    }
}