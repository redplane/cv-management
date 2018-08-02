using System.Collections.Generic;

namespace ApiClientShared.ViewModel.Project
{
    public class SearchProjectViewModel : BaseSearchViewModel
    {
        public HashSet<int> Ids { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
       
    }
}