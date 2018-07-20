using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ApiMultiPartFormData.Models;

namespace Cv_Management.ViewModel.SkillCategory
{
    public class ReadingSkillCategoryViewModel
    {

        public int Id { get; set; }
        public int UserId { get; set; }
 
        public string Photo { get; set; }
        public string Name { get; set; }
        public double CreatedTime { get; set; }
    }
}