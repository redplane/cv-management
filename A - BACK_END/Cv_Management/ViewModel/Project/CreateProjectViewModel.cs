using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Cv_Management.ViewModel.Project
{
    public class CreateProjectViewModel
    {
      
        public int UserId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public double StatedTime { get; set; }
        public double? FinishedTime { get; set; }
    }
}