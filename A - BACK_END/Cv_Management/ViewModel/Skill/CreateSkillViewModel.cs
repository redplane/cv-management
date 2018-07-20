using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Cv_Management.ViewModel.Skill
{
    public class CreateSkillViewModel
    {
        [Required]
        public string Name { get; set; }
      

    }
}