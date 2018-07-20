using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Cv_Management.ViewModel.PersonalSkill
{
    public class SearchPersonalSkillViewModel : BaseSearchViewModel
    {
      
        public HashSet<int> SkillCategoryIds { get; set; }
    
        public HashSet<int> SkillIds { get; set; }
   
        public int Point { get; set; }
       
    }
}