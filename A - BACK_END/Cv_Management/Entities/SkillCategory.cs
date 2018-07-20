using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Cv_Management.Entities
{
    public class SkillCategory
    {
        [Key]
        public  int Id { get; set; }
        public  int UserId { get; set; }
        public User User { get; set; }
        public  string Photo { get; set; }
         public string Name { get; set; }
        public  double CreatedTime { get; set; }
        public  List<PersonalSkill> PersonalSkills { get; set; }
      
    }
}