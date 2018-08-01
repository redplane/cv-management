using System;
using System.Linq;
using System.Web.UI.WebControls;
using Cv_Management.Models;

namespace Cv_Management.Interfaces.Services
{
    public interface IDbService
    {
        #region Methods

        /// <summary>
        ///     Do pagination on a specific list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="pagination"></param>
        /// <returns></returns>
        IQueryable<T> Paginate<T>(IQueryable<T> list, Pagination pagination);
        
        /// <summary>
        ///     Sort a list by using specific property enumeration.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="sortDirection"></param>
        /// <param name="sortProperty"></param>
        /// <returns></returns>
        IQueryable<T> Sort<T>(IQueryable<T> list, SortDirection sortDirection, Enum sortProperty);

        /// <summary>
        ///     Search property base on searching mode.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="records"></param>
        /// <param name="property"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        IQueryable<T> SearchPropertyText<T>(IQueryable<T> records, Func<T, string> property, TextSearch search);

        #endregion
    }
}