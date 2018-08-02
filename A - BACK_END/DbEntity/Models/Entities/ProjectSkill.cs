using Newtonsoft.Json;

namespace DbEntity.Models.Entities
{
    public class ProjectSkill
    {
        #region Properties

        public int ProjectId { get; set; }

        public int SkillId { get; set; }

        #endregion

        #region Navigation properties

        [JsonIgnore]
        public virtual Project Project { get; set; }

        [JsonIgnore]
        public virtual Skill Skill { get; set; }

        #endregion
    }
}