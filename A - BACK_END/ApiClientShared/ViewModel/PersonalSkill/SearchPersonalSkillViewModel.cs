using System.Collections.Generic;

namespace ApiClientShared.ViewModel.PersonalSkill
{
    public class SearchPersonalSkillViewModel : BaseSearchViewModel
    {
      
        public HashSet<int> SkillCategoryIds { get; set; }
    
        public HashSet<int> SkillIds { get; set; }
   
        public int Point { get; set; }
       
    }
}