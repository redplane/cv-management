using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Cv_Management.Entities
{
    public class Project
    {
        [Key]
        public  int Id { get; set; }
        public  int UserId { get; set; }
        public User User { get; set; }
        public  string Name { get; set; }
        public  string Description { get; set; }
        public  double StatedTime { get; set; }
        public double? FinishedTime { get; set; }
        public List<ProjectSkill> ProjectSkills { get; set; }
        public List<ProjectResponsibility> ProjectResponsibilities { get; set; }

    }
}