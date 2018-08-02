using System.Collections.Generic;

namespace ApiClientShared.ViewModel.ProjectSkill
{
    public class SearchProjectSkillViewModel:BaseSearchViewModel
    {
        public HashSet<int> ProjectIds { get; set; }

        public HashSet<int> SkillIds { get; set; }
    }
}