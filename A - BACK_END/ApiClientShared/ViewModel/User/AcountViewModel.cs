using ApiClientShared.Enums;

namespace ApiClientShared.ViewModel.User
{
    public class AcountViewModel
    {
        public  string Username { get; set; }
        public  string Password { get; set; }
        public  UserRoles Role { get; set; }
    }
}