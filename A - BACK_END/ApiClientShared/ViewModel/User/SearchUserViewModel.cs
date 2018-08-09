using System.Collections.Generic;
using ApiClientShared.Models;

namespace ApiClientShared.ViewModel.User
{
    public class SearchUserViewModel : BaseSearchViewModel
    {
        public HashSet<int> Ids { get; set; }

        public HashSet<string> FirstNames { get; set; }

        public HashSet<string> LastNames { get; set; }

        /// <summary>
        /// Birthday range.
        /// </summary>
        public RangeModel<double?, double?> Birthday { get; set; }

        /// <summary>
        /// Response include descriptions of user
        /// </summary>
        public bool IncludeDescriptions { get; set; }


        /// <summary>
        /// Response include hobbies of user
        /// </summary>
        public bool IncludeHobbies { get; set; }
    }
}