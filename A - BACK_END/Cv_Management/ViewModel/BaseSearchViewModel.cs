using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cv_Management.ViewModel
{
    public class BaseSearchViewModel
    {
        #region Properties

        /// <summary>
        /// Pagination information
        /// </summary>
        public PaginationViewModel Pagination { get; set; }

        #endregion
    }
}