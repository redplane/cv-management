using Newtonsoft.Json;

namespace DbEntity.Models.Entities
{
    public class UserDescription
    {
        #region Properties

        /// <summary>
        /// Id of description.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Id of user.
        /// </summary>
        public int UserId { get; set; }
        
        /// <summary>
        /// Description.
        /// </summary>
        public string Description { get; set; }

        #endregion

        #region Navigation properties

        /// <summary>
        /// User who has this description.
        /// </summary>
        [JsonIgnore]
        public virtual User User { get; set; }

        #endregion
    }
}