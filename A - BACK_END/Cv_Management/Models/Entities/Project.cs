using System.Collections.Generic;
using Newtonsoft.Json;

namespace Cv_Management.Models.Entities
{
    public class Project
    {
        #region Properties

        public int Id { get; set; }

        public int UserId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public double StatedTime { get; set; }

        public double? FinishedTime { get; set; }

        #endregion

        #region Navigation properties

        /// <summary>
        /// User that has the project.
        /// </summary>
        [JsonIgnore]
        public virtual User User { get; set; }

        /// <summary>
        /// List of skills are used in project.
        /// </summary>
        [JsonIgnore]
        public virtual ICollection<ProjectSkill> ProjectSkills { get; set; }

        /// <summary>
        /// List of responsibilities user has in project.
        /// </summary>
        [JsonIgnore]
        public virtual ICollection<ProjectResponsibility> ProjectResponsibilities { get; set; }

        #endregion
    }
}