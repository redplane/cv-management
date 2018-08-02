using System.Collections.Generic;

namespace ApiClientShared.ViewModel.Skill
{
    public class SearchSkillViewModel:BaseSearchViewModel
    {
        public HashSet<int> Ids { get; set; }
        public string Name { get; set; }
      

    }
}