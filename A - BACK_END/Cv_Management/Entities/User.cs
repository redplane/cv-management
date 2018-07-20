using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace Cv_Management.Entities
{
    public class User
    {
        #region Properties

        [Key]
        public int Id { get; set; }

        public string Email { get; set; }

        [JsonIgnore]
        public string Password { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Photo { get; set; }

        public double Birthday { get; set; }

        public string Role { get; set; }

        #endregion

        #region Navigation properties

        [JsonIgnore]
        public List<SkillCategory> SkillCategories { get; set; }

        [JsonIgnore]
        public List<UserDescription> UserDescriptions { get; set; }

        [JsonIgnore]
        public List<Project> Projects { get; set; }

        [JsonIgnore]
        public List<Hobby> Hobbies { get; set; }

        #endregion
    }
}