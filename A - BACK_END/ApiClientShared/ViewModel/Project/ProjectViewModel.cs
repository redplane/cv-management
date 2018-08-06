using ApiClientShared.ViewModel.Responsibility;
using ApiClientShared.ViewModel.Skill;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiClientShared.ViewModel.Project
{
    public class ProjectViewModel
    {
        /// <summary>
        /// Id of project.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Id of user that takes part in project.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Project name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Project description.
        /// </summary>
        public string Description { get; set; }

        public double StartedTime { get; set; }

        public double? FinishedTime { get; set; }

        public IEnumerable<SkillViewModel> Skills { get; set; }

        public IEnumerable<ResponsibilityViewModel> Responsibilities { get; set; }

    }
}
