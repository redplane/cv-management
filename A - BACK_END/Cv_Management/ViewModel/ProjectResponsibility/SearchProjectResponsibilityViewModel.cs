using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Cv_Management.ViewModel.ProjectResponsibility
{
    public class SearchProjectResponsibilityViewModel:BaseSearchViewModel
    {
    
        public HashSet<int> ProjectIds { get; set; }
     
        public HashSet<int> ResponsibilityIds { get; set; }

     
    }
}