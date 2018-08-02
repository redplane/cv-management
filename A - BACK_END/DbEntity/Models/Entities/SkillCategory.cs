using System.Collections.Generic;
using Newtonsoft.Json;

namespace DbEntity.Models.Entities
{
    public class SkillCategory
    {
        #region Properties

        public int Id { get; set; }

        public int UserId { get; set; }

        public string Photo { get; set; }

        public string Name { get; set; }

        public double CreatedTime { get; set; }

        #endregion

        #region Navigation properties

        /// <summary>
        ///     User who has the skill category.
        /// </summary>
        [JsonIgnore]
        public virtual User User { get; set; }

        /// <summary>
        ///     List of personal skills
        /// </summary>
        [JsonIgnore]
        public virtual ICollection<SkillCategorySkillRelationship> SkillCategorySkillRelationships { get; set; }

        #endregion
    }
}