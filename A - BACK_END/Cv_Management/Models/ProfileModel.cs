using ApiClientShared.Enums;

namespace Cv_Management.Models
{
    public class ProfileModel
    {
        public int Id { get; set; }

        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public UserRoles Role { get; set; }

        public UserStatuses Status { get; set; }
    }
}