using System.Collections.Generic;
using Newtonsoft.Json;

namespace DbEntity.Models.Entities
{
    public class Responsibility
    {
        #region Navigation properties

        [JsonIgnore]
        public virtual ICollection<ProjectResponsibility> ProjectResponsibilities { get; set; }

        #endregion

        #region Properties

        public int Id { get; set; }

        public string Name { get; set; }

        public double CreatedTime { get; set; }

        public double? LastModifiedTime { get; set; }

        #endregion
    }
}