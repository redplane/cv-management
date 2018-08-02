using System.Collections.Generic;
using ApiClientShared.Models;

namespace ApiClientShared.ViewModel.UserDescription
{
    public class SearchUserDescriptionViewModel
    {
        #region Properties

        /// <summary>
        /// User description indexes.
        /// </summary>
        public HashSet<int> Ids { get; set; }

        /// <summary>
        /// User index which description belongs to.
        /// </summary>
        public HashSet<int> UserIds { get; set; }
        
        /// <summary>
        /// Pagination information.
        /// </summary>
        public Pagination Pagination { get; set; }

        #endregion
    }
}