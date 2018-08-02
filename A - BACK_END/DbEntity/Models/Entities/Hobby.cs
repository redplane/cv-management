using Newtonsoft.Json;

namespace DbEntity.Models.Entities
{
    public class Hobby
    {
        #region Navigation properties

        /// <summary>
        ///     User that has this hobby.
        /// </summary>
        [JsonIgnore]
        public virtual User User { get; set; }

        #endregion

        #region Properties

        public int Id { get; set; }

        public int UserId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        #endregion
    }
}