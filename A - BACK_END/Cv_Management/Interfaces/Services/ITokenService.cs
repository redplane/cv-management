using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ApiClientShared.ViewModel.User;

namespace Cv_Management.Interfaces.Services
{
    public interface ITokenService
    {
        /// <summary>
        /// Get token from user information
        /// </summary>
        /// <param name="payload"></param>
        /// <returns></returns>
        string Encode(Dictionary<string, string> payload);


        /// <summary>
        /// Decode token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Dictionary<string, string> Decode(string token);
    }
}
