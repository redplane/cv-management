using System.Collections.Generic;

namespace ApiClientShared.ViewModel.User
{
    public class SearchUserViewModel:BaseSearchViewModel
    {
        public HashSet<int> Ids { get; set; }

        public HashSet<string> FirstNames { get; set; }

        public HashSet<string> LastNames { get; set; }
      
        public double Birthday { get; set; }

        public bool IncludeDescriptions { get; set; }
    }
}