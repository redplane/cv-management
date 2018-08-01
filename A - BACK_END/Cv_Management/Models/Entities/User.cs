using System.Collections.Generic;
using Cv_Management.Enums;
using Newtonsoft.Json;

namespace Cv_Management.Models.Entities
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

        public UserRoles Role { get; set; }

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