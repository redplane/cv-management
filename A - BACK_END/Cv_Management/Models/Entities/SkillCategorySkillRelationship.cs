using Newtonsoft.Json;

namespace Cv_Management.Models.Entities
{
    public class SkillCategorySkillRelationship
    {
        #region Properties

        public int SkillCategoryId { get; set; }

        public int SkillId { get; set; }

        public int Point { get; set; }

        public double CreatedTime { get; set; }

        #endregion

        #region Navigation properties

        /// <summary>
        ///     Skill information.
        /// </summary>
        [JsonIgnore]
        public virtual Skill Skill { get; set; }

        /// <summary>
        ///     Skill category.
        /// </summary>
        [JsonIgnore]
        public virtual SkillCategory SkillCategory { get; set; }

        #endregion
    }
}