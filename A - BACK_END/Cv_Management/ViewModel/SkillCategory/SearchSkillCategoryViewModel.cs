using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ApiMultiPartFormData.Models;

namespace Cv_Management.ViewModel.SkillCategory
{
    public class SearchSkillCategoryViewModel: BaseSearchViewModel
    {
        #region Properties

        /// <summary>
        /// List of indexes.
        /// </summary>
        public HashSet<int> Ids { get; set; }

        /// <summary>
        /// List of user indexes.
        /// </summary>
        public HashSet<int> UserIds { get; set; }
        
        /// <summary>
        /// Skill category names.
        /// </summary>
        public HashSet<string> Names { get; set; }

        #endregion
    }
}