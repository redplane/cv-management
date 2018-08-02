using Newtonsoft.Json;

namespace DbEntity.Models.Entities
{
    public class ProjectResponsibility
    {
        #region Properties

        public  int ProjectId { get; set; }

        public int ResponsibilityId { get; set; }

        public double CreatedTime { get; set; }

        #endregion

        #region Navigation properties

        [JsonIgnore]
        public virtual Responsibility Responsibility { get; set; }

        [JsonIgnore]
        public virtual Project Project { get; set; }

        #endregion
    }
}