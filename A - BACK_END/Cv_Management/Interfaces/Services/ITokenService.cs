using System.Collections.Generic;

namespace Cv_Management.Interfaces.Services
{
    public interface ITokenService
    {
        /// <summary>
        ///     Get token from user information
        /// </summary>
        /// <param name="payload"></param>
        /// <returns></returns>
        string Encode(Dictionary<string, string> payload);


        /// <summary>
        ///     Decode token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Dictionary<string, string> Decode(string token);
    }
}