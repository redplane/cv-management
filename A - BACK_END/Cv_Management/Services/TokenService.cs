using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Security.Claims;
using CvManagement.Interfaces.Services;
using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using Newtonsoft.Json;

namespace CvManagement.Services
{
    public class TokenService : ITokenService
    {
        #region Contructors

        public TokenService()
        {
            JwtSecret = ConfigurationManager.AppSettings[nameof(JwtSecret)];
            JwtName = ConfigurationManager.AppSettings[nameof(JwtName)];
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Token secret.
        /// </summary>
        public string JwtSecret { get; set; }

        /// <summary>
        /// Token name.
        /// </summary>
        public string JwtName { get; set; }
        #endregion

        #region Methods

        /// <summary>
        ///     <inheritdoc />
        /// </summary>
        /// <param name="payload"></param>
        /// <returns></returns>
        public string Encode(IDictionary payload)
        {
            var algorithm = new HMACSHA256Algorithm();
            var serializer = new JsonNetSerializer();
            var urlEncoder = new JwtBase64UrlEncoder();
            var encoder = new JwtEncoder(algorithm, serializer, urlEncoder);

            var token = encoder.Encode(payload, JwtSecret);

            return token;
        }


        /// <summary>
        ///     <inheritdoc />
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public T Decode<T>(string token)
        {
            return Decode<T>(token, false);
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="token"></param>
        /// <param name="bValidate"></param>
        /// <returns></returns>
        public T Decode<T>(string token, bool bValidate)
        {
            var serializer = new JsonNetSerializer();
            var provider = new UtcDateTimeProvider();
            var validator = new JwtValidator(serializer, provider);
            var urlEncoder = new JwtBase64UrlEncoder();
            var decoder = new JwtDecoder(serializer, validator, urlEncoder);

            var json = decoder.Decode(token, JwtSecret, bValidate);
            var claims = JsonConvert.DeserializeObject<T>(json);

            return claims;
        }


        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public ClaimsPrincipal ToPrinciple(string token)
        {
            // Decode the token.
            var claims = Decode<Dictionary<string, string>>(token, true);
            if (claims == null)
                return null;

            var claimsIdentity = new ClaimsIdentity(null, JwtName);
            foreach (var key in claims.Keys)
            {
                if (string.IsNullOrEmpty(claims[key]))
                    continue;

                claimsIdentity.AddClaim(new Claim(key, claims[key]));
            }

            // Authenticate the request.
            return new ClaimsPrincipal(claimsIdentity);
        }

        #endregion
    }
}