using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Cv_Management.Interfaces.Services;
using Cv_Management.Models;
using JWT;
using JWT.Algorithms;
using JWT.Serializers;

namespace Cv_Management.Services
{
    public class ProfileService : IProfileService
    {
        #region Properties

        /// <summary>
        ///     Find secret key.
        /// </summary>
        public string JwtSecret => ConfigurationManager.AppSettings["jwt.secret"];

        /// <summary>
        ///     Jwt life time (in seconds)
        /// </summary>
        public int JwtLifeTime => int.Parse(ConfigurationManager.AppSettings["jwt.token.lifeTime"]);

        /// <summary>
        ///     Jwt name
        /// </summary>
        public string JwtName => ConfigurationManager.AppSettings["jwt.token.name"];

        #endregion

        #region Methods

        /// <summary>
        ///     <inheritdoc />
        /// </summary>
        /// <param name="claims"></param>
        /// <param name="secret"></param>
        /// <returns></returns>
        public string EncodeJwt(Dictionary<string, string> claims, string secret)
        {
            var bytes = Encoding.UTF8.GetBytes(secret);

            var algorithm = new HMACSHA256Algorithm();
            var serializer = new JsonNetSerializer();
            var urlEncoder = new JwtBase64UrlEncoder();
            var encoder = new JwtEncoder(algorithm, serializer, urlEncoder);
            return encoder.Encode(claims, bytes);
        }

        /// <summary>
        ///     <inheritdoc />
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jwt"></param>
        /// <param name="secret"></param>
        /// <returns></returns>
        public T DecodeJwt<T>(string jwt, string secret)
        {
            var serializer = new JsonNetSerializer();
            var provider = new UtcDateTimeProvider();
            var validator = new JwtValidator(serializer, provider);
            var urlEncoder = new JwtBase64UrlEncoder();
            var decoder = new JwtDecoder(serializer, validator, urlEncoder);

            return decoder.DecodeToObject<T>(jwt, secret, true);
        }

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