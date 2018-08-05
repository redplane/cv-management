using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ApiMultiPartFormData.Models;

namespace ApiClientShared.ViewModel.SkillCategory
{
    public class UpdateSkillCategoryViewModel
    {
        [Required]
        public int UserId { get; set; }
        public HttpFile Photo { get; set; }
        [Required]
        public string Name { get; set; }
    }
}