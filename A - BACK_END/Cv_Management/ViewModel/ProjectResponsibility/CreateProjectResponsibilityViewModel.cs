using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Cv_Management.ViewModel.ProjectResponsibility
{
    public class CreateProjectResponsibilityViewModel
    {
        [Required]
        public int ProjectId { get; set; }
        [Required]
        public int ResponsibilityId { get; set; }
    }
}