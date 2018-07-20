using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cv_Management.ViewModel.UserDescription
{
    public class SearchUserDescriptionViewModel:BaseSearchViewModel
    {
        public HashSet<int> Ids { get; set; }
        public int UserId { get; set; }
        public string Description { get; set; }
    }
}