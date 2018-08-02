using System.Collections.Generic;

namespace ApiClientShared.ViewModel.Skill
{
    public class SearchSkillViewModel : BaseSearchViewModel
    {
        #region Properties
        /// <summary>
        /// Skill'id indexes 
        /// </summary>
        public HashSet<int> Ids { get; set; }

        /// <summary>
        /// Skill'name indexes
        /// </summary>
        public HashSet<string> Names { get; set; }

        #endregion




    }
}