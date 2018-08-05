using ApiMultiPartFormData.Models;
using System.ComponentModel.DataAnnotations;

namespace ApiClientShared.ViewModel.User
{
    public class AddUserViewModel
    {
      
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Role { get; set; }

        public HttpFile photo { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public double Birthday { get; set; }

     
    }
}