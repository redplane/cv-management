using System.Collections.Generic;

namespace ApiClientShared.ViewModel.ProjectSkill
{
    public class SearchProjectSkillViewModel : BaseSearchViewModel
    {
        #region Properties

        public HashSet<int> ProjectIds { get; set; }

        public HashSet<int> SkillIds { get; set; }

        #endregion
    }
}