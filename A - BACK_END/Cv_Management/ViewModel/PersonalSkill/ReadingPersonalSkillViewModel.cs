using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Cv_Management.ViewModel.PersonalSkill
{
    public class ReadingPersonalSkillViewModel
    {

        public int SkillCategoryId { get; set; }
    
        public int SkillId { get; set; }
  
        public int Point { get; set; }
        public double CreatedTime { get; set; }

    }
}