using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Cv_Management.Models;

namespace Cv_Management.ViewModel
{
    public class BaseSearchViewModel
    {
        #region Properties

        /// <summary>
        /// Pagination information
        /// </summary>
        public Pagination Pagination { get; set; }

        #endregion
    }
}