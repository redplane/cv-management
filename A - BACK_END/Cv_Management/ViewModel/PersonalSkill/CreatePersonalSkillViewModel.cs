using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Cv_Management.ViewModel.PersonalSkill
{
    public class CreatePersonalSkillViewModel
    {
        [Required]
        public int SkillCategoryId { get; set; }
        [Required]
        public int SkillId { get; set; }
   
        public int Point { get; set; }
       
    }
}