using System.ComponentModel.DataAnnotations;

namespace ApiClientShared.ViewModel.User
{
    public class RegisterViewModel
    {
        #region Properties

        /// <summary>
        /// User first name.
        /// </summary>
        [Required]
        public string FirstName { get; set; }

        /// <summary>
        /// User email address.
        /// Validation email will be sent to user after registration is done.
        /// </summary>
        [Required]
        public  string Email { get; set; }

        /// <summary>
        /// Password that user uses for accessing system/
        /// </summary>
        [Required]
        public  string Password { get; set; }

        /// <summary>
        /// User last name.
        /// </summary>
        [Required]
        public string LastName { get; set; }

        /// <summary>
        /// Client captcha code sent from re-captcha service.
        /// </summary>
        [Required]
        public string ClientCaptchaCode { get; set; }

        #endregion
    }
}