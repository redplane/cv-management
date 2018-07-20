using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cv_Management.ViewModel.Skill
{
    public class ReadingSkillViewModel
    {
        public  int Id { get; set; }
        public string Name { get; set; }
        public double CreatedTime { get; set; }

        public double? LastModifiedTime { get; set; }

    }
}