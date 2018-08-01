using Newtonsoft.Json;

namespace Cv_Management.Models.Entities
{
    public class ProjectSkill
    {
        #region Properties

        public int ProjectId { get; set; }

        public int SkillId { get; set; }

        #endregion

        #region Navigation properties

        [JsonProperty]
        public virtual Project Project { get; set; }

        [JsonProperty]
        public virtual Skill Skill { get; set; }

        #endregion
    }
}