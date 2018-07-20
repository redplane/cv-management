using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Cv_Management.ViewModel.ProjectSkill
{
    public class CreateProjectSkillViewModel
    {
        [Required]
        public int ProjectId { get; set; }
        [Required]
        public int SkillId { get; set; }
    }
}