using ApiClientShared.Models;
using System.Collections.Generic;

namespace ApiClientShared.ViewModel.Responsibility
{
    public class SearchResponsibilityViewModel:BaseSearchViewModel
    {
        public HashSet<int> Ids { get; set; }

        public HashSet<string> Names { get; set; }

        public RangeModel<double,double>  CreatedTime { get; set; }

        public RangeModel<double,double> LastModifiedTime { get; set; }
    }
}