using System.Collections.Generic;
using ApiClientShared.Models;

namespace ApiClientShared.ViewModel.PersonalSkill
{
    public class SearchPersonalSkillViewModel : BaseSearchViewModel
    {
        /// <summary>
        /// List of user ids .
        /// </summary>
        public HashSet<int> UserIds { get; set; }

        /// <summary>
        /// List of skill categories.
        /// </summary>
        public HashSet<int> SkillCategoryIds { get; set; }
    
        /// <summary>
        /// List of skill ids.
        /// </summary>
        public HashSet<int> SkillIds { get; set; }
   
        /// <summary>
        /// Point range.
        /// </summary>
        public RangeModel<int?, int?> Point { get; set; }
       
    }
}