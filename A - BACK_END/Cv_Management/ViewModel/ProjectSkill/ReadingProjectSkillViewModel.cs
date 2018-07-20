using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Cv_Management.ViewModel.Project;
using Cv_Management.ViewModel.Skill;

namespace Cv_Management.ViewModel.ProjectSkill
{
    public class ReadingProjectSkillViewModel
    {
        public int ProjectId { get; set; }
        public ReadingProjectViewModel project { get; set; }

        public int SkillId { get; set; }
        public  ReadingSkillViewModel skill { get; set; }
    }
}