using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Cv_Management.ViewModel.UserDescription
{
    public class CreateUserDescriptionViewModel
    {
        [Required]
        public int UserId { get; set; }
        [Required]
        public string Description { get; set; }
    }
}