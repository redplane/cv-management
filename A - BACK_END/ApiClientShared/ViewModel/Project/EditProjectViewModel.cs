using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ApiClientShared.ViewModel.Project
{
    public class EditProjectViewModel
    {

        public int UserId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }


        public double StatedTime { get; set; }

        public double? FinishedTime { get; set; }

        public HashSet<int> SkillIds { get; set; }

        public HashSet<int> ResponsibilityIds { get; set; }
    }
}