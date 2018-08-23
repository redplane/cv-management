using System.Collections.Generic;
using ApiClientShared.Enums;
using Newtonsoft.Json;

namespace DbEntity.Models.Entities
{
    public class User
    {
        #region Properties

        public int Id { get; set; }

        public string Email { get; set; }

        [JsonIgnore]
        public string Password { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Photo { get; set; }

        public double Birthday { get; set; }

        /// <summary>
        /// User role in the system.
        /// </summary>
        public UserRoles Role { get; set; }

        /// <summary>
        /// User status in the system.
        /// </summary>
        public UserStatuses Status { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initialize user object with default settings.
        /// </summary>
        public User()
        {
            Status = UserStatuses.Pending;
        }

        #endregion

        #region Navigation properties

        [JsonIgnore]
        public virtual ICollection<SkillCategory> SkillCategories { get; set; }

        [JsonIgnore]
        public virtual ICollection<UserDescription> UserDescriptions { get; set; }

        [JsonIgnore]
        public virtual ICollection<Project> Projects { get; set; }

        [JsonIgnore]
        public virtual ICollection<Hobby> Hobbies { get; set; }
        
        #endregion
    }
}