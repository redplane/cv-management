using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Cv_Management.ViewModel.ProjectResponsibility
{
    public class ReadingProjectResponsibilityViewModel
    {
      
        public int ProjectId { get; set; }
     
        public int ResponsibilityId { get; set; }
        public double CreatedTime { get; set; }
    }
}