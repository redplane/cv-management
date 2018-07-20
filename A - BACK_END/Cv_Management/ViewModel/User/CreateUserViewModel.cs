using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ApiMultiPartFormData.Models;

namespace Cv_Management.ViewModel.User
{
    public class CreateUserViewModel
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public  string Email { get; set; }

        [Required]
        public  string Password { get; set; }

        [Required]
        public string LastName { get; set; }

        public HttpFile Photo { get; set; }

        [Required]
        public double Birthday { get; set; }

        [Required]
        public string Role { get; set; }
    }
}