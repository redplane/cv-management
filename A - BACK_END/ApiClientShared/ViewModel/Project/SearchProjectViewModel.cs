using ApiClientShared.Models;
using System.Collections.Generic;

namespace ApiClientShared.ViewModel.Project
{
    public class SearchProjectViewModel : BaseSearchViewModel
    {
        /// <summary>
        /// Ids of project , not duplicate
        /// </summary>
        public HashSet<int> Ids { get; set; }

        public HashSet<int> UserIds { get; set; }

        public HashSet<string> Names { get; set; }

        public bool IncludeResponsibilities { get; set; }

        public bool IncludeSkills { get; set; }

        public RangeModel<double, double> StartedTime { get; set; }

        public RangeModel<double,double> FinishedTime { get; set; }
       
    }
}