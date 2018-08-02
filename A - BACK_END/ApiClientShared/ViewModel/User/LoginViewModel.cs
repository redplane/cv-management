using System.ComponentModel.DataAnnotations;

namespace ApiClientShared.ViewModel.User
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

    }
}