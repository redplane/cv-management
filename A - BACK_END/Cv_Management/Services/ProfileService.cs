using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using CvManagement.Interfaces.Services;
using CvManagement.Models;

namespace CvManagement.Services
{
    public class ProfileService : IProfileService
    {
        #region Methods
        
        /// <summary>
        ///     <inheritdoc />
        /// </summary>
        /// <param name="httpRequestMessage"></param>
        /// <returns></returns>
        public ProfileModel GetProfile(HttpRequestMessage httpRequestMessage)
        {
            if (!httpRequestMessage.Properties.ContainsKey(ClaimTypes.Actor))
                return null;

            return (ProfileModel) httpRequestMessage.Properties[ClaimTypes.Actor];
        }

        /// <summary>
        ///     <inheritdoc />
        /// </summary>
        /// <param name="httpRequestMessage"></param>
        /// <param name="profile"></param>
        public void SetProfile(HttpRequestMessage httpRequestMessage, ProfileModel profile)
        {
            httpRequestMessage.Properties[ClaimTypes.Actor] = profile;
        }

        /// <summary>
        ///     <inheritdoc />
        /// </summary>
        /// <param name="originalPassword"></param>
        /// <returns></returns>
        public string HashPassword(string originalPassword)
        {
            var md5 = MD5.Create();
            var inputBytes = Encoding.ASCII.GetBytes(originalPassword);

            var hash = md5.ComputeHash(inputBytes);
            var sb = new StringBuilder();

            foreach (var t in hash)
                sb.Append(t.ToString("X2"));

            return sb.ToString();
        }

        #endregion
    }
}