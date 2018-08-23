using System.Collections.Generic;

namespace ApiClientShared.ViewModel.SkillCategorySkillRelationship
{
    public class AddHasSkillViewModel
    {
        #region Properties

        /// <summary>
        /// Skill category id.
        /// </summary>
        public int SkillCategoryId { get; set; }

        /// <summary>
        /// List of skills
        /// </summary>
        public HashSet<HasSkillViewModel> HasSkills { get; set; }

        #endregion
    }
}