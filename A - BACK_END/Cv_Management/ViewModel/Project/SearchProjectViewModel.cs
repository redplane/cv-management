using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cv_Management.ViewModel.Project
{
    public class SearchProjectViewModel : BaseSearchViewModel
    {
        public HashSet<int> Ids { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
       
    }
}