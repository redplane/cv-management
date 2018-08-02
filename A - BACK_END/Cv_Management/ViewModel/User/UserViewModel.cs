using System.Collections.Generic;
using ApiClientShared.Enums;
using ApiClientShared.ViewModel.User;

namespace Cv_Management.ViewModel.User
{
    public class UserViewModel 
    {
        #region Properties

        public int Id { get; set; }

        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Photo { get; set; }

        public double Birthday { get; set; }

        public UserRoles Role { get; set; }
        
        /// <summary>
        /// List of description.
        /// </summary>
        public IEnumerable<UserDescriptionViewModel> Descriptions { get; set; }

        #endregion
    }
}