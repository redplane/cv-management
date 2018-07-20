using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Cv_Management.ViewModel.Responsibility
{
    public class CreateResponsibilityViewModel
    {
        [Required]
        public string Name { get; set; }
    }
}