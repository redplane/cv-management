using System.Collections.Generic;
using Newtonsoft.Json;

namespace Cv_Management.Models.Entities
{
    public class Skill
    {
        #region Properties

        public int Id { get; set; }

        public string Name { get; set; }

        public double CreatedTime { get; set; }

        public double? LastModifiedTime { get; set; }

        #endregion

        #region Navigation properties

        [JsonIgnore]
        public virtual ICollection<ProjectSkill> ProjectSkills { get; set; }

        [JsonIgnore]
        public virtual ICollection<SkillCategorySkillRelationship> SkillCategorySkillRelationships { get; set; }

        #endregion
    }
}