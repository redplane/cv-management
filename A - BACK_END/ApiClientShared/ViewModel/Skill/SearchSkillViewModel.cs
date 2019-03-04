using ApiClientShared.Models;
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

        /// <summary>
        /// When the skill was created.
        /// </summary>
        public RangeModel<double?,double?>  CreatedTime { get; set; }

        #endregion
    }
}