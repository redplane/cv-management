using System.Collections.Generic;

namespace ApiClientShared.ViewModel.Responsibility
{
    public class SearchResponsibilityViewModel:BaseSearchViewModel
    {
        public HashSet<int> Ids { get; set; }
        public string Name { get; set; }
    }
}