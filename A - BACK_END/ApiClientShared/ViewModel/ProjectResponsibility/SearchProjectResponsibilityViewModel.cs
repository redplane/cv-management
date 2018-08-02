using System.Collections.Generic;

namespace ApiClientShared.ViewModel.ProjectResponsibility
{
    public class SearchProjectResponsibilityViewModel:BaseSearchViewModel
    {
    
        public HashSet<int> ProjectIds { get; set; }
     
        public HashSet<int> ResponsibilityIds { get; set; }

     
    }
}