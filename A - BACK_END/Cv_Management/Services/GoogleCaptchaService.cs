using System;
using System.Configuration;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Cv_Management.Interfaces.Services;
using Cv_Management.Models;
using DelegateDecompiler;
using Newtonsoft.Json.Linq;

namespace Cv_Management.Services
{
    public class GoogleCaptchaService : ICaptchaService
    {
        #region Properties

        /// <summary>
        /// Http client which is for initializing http request.
        /// </summary>
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Application setting model.
        /// </summary>
        private readonly AppSettingModel _appSetting;

        #endregion

        #region Constructors

        /// <summary>
        /// Initialize service with injectors.
        /// </summary>
        public GoogleCaptchaService(HttpClient httpClient, AppSettingModel appSetting)
        {
            _httpClient = httpClient;
            _appSetting = appSetting;
        }

        #endregion

        #region Methods

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="code"></param>
        /// <param name="clientAddress"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> IsCaptchaValidAsync(string code, string clientAddress,  CancellationToken cancellationToken)
        {
            var uri =
                $"{_appSetting.GCaptchaValidationEndpoint}?secret=${_appSetting.GCaptchaSecret}&response=${code}&remoteip=${clientAddress}";
            
            var httpResponseMessage = await _httpClient.PostAsync(uri, new StringContent("{}"), cancellationToken);
            
            // Read the http response content.
            var httpContent = httpResponseMessage.Content;
            if (httpContent == null)
                return false;

            var content = await httpContent.ReadAsStringAsync();
            if (string.IsNullOrEmpty(content))
                return false;

            var jObject = JObject.Parse(content);
            var bIsSuccess = false;
            bool.TryParse(jObject["success"].ToString(), out bIsSuccess);

            return bIsSuccess;
        }

        #endregion
    }
}