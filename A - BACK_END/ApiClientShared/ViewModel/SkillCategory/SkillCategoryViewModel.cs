using System.Collections.Generic;
using ApiClientShared.ViewModel.Skill;

namespace ApiClientShared.ViewModel.SkillCategory
{
    public class SkillCategoryViewModel
    {
        #region Properties

        public int Id { get; set; }

        public int UserId { get; set; }

        public string Photo { get; set; }

        public string Name { get; set; }

        public double CreatedTime { get; set; }

        /// <summary>
        /// List of skills belongs to skill
        /// </summary>
        public IEnumerable<SkillCategorySkillRelationshipViewModel> Skills { get; set; }

        #endregion
    }
}