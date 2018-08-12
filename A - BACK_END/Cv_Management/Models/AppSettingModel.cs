namespace Cv_Management.Models
{
    public class AppSettingModel
    {
        #region Properties

        /// <summary>
        ///     Secret key that provided by google.
        ///     More info: https://www.google.com/recaptcha/admin#site/342382560?setup
        /// </summary>
        public string GCaptchaSecret { get; set; }

        /// <summary>
        ///     Endpoint of Google captcha validation.
        /// </summary>
        public string GCaptchaValidationEndpoint { get; set; }

        #endregion
    }
}