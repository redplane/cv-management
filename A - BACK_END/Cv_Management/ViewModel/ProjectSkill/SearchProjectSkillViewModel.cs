using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Cv_Management.ViewModel.ProjectSkill
{
    public class SearchProjectSkillViewModel:BaseSearchViewModel
    {
        public HashSet<int> ProjectIds { get; set; }

        public HashSet<int> SkillIds { get; set; }
    }
}