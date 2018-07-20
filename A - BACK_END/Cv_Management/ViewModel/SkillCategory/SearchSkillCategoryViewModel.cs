using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ApiMultiPartFormData.Models;

namespace Cv_Management.ViewModel.SkillCategory
{
    public class SearchSkillCategoryViewModel: BaseSearchViewModel
    {

        public HashSet<int> Ids { get; set; }
        public int UserId { get; set; }
        
        public string Name { get; set; }
    }
}