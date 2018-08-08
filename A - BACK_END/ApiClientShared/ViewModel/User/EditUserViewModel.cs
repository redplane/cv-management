using ApiMultiPartFormData.Models;

namespace ApiClientShared.ViewModel.User
{
    public class EditUserViewModel
    {
        #region Properties

        /// <summary>
        /// First name of user.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// User last name
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Birthday (UNIX time)
        /// </summary>
        public double? Birthday { get; set; }

        /// <summary>
        /// User profile photo.
        /// </summary>
        public HttpFile Photo { get; set; }

        #endregion
    }
}