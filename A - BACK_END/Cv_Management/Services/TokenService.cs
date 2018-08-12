using System.Collections.Generic;
using Cv_Management.Constant;
using Cv_Management.Interfaces.Services;
using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using Newtonsoft.Json;

namespace Cv_Management.Services
{
    public class TokenService : ITokenService
    {
        #region Contructors

        #endregion

        #region Methods

        /// <summary>
        ///     <inheritdoc />
        /// </summary>
        /// <param name="payload"></param>
        /// <returns></returns>
        public string Encode(Dictionary<string, string> payload)
        {
            var algorithm = new HMACSHA256Algorithm();
            var serializer = new JsonNetSerializer();
            var urlEncoder = new JwtBase64UrlEncoder();
            var encoder = new JwtEncoder(algorithm, serializer, urlEncoder);

            var token = encoder.Encode(payload, GlobalConstant.Secret);

            return token;
        }


        /// <summary>
        ///     <inheritdoc />
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public Dictionary<string, string> Decode(string token)
        {
            var serializer = new JsonNetSerializer();
            var provider = new UtcDateTimeProvider();
            var validator = new JwtValidator(serializer, provider);
            var urlEncoder = new JwtBase64UrlEncoder();
            var decoder = new JwtDecoder(serializer, validator, urlEncoder);

            var json = decoder.Decode(token, GlobalConstant.Secret, true);
            var claims = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

            return claims;
        }

        #endregion
    }
}