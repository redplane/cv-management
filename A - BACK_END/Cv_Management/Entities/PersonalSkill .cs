using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Cv_Management.Entities
{
    public class PersonalSkill
    {
        [Key, Column(Order = 0)]
        public  int SkillCategoryId { get; set; }
        public SkillCategory SkillCategory { get; set; }
        [Key, Column(Order = 1)]
        public int SkillId { get; set; }
        public Skill Skill { get; set; }
        public int Point { get; set; }
        public  double CreatedTime { get; set; }
    }
}