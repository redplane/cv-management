using ApiClientShared.Enums;
using Newtonsoft.Json;

namespace Cv_Management.Models
{
    public class ProfileModel
    {
        public int Id { get; set; }

        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        /// <summary>
        /// Account password.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Role of user in the system.
        /// </summary>
        public UserRoles Role { get; set; }

        /// <summary>
        /// Status of account.
        /// </summary>
        public UserStatuses Status { get; set; }

        /// <summary>
        /// Access token provided to the current profile.
        /// </summary>
        public string AccessToken { get; set; }
    }
}