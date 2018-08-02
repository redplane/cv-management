using System;
using System.Linq;
using System.Linq.Expressions;
using System.Web.UI.WebControls;
using ApiClientShared.Enums;
using ApiClientShared.Models;
using Cv_Management.Interfaces.Services;

namespace Cv_Management.Services
{
    public class DbService : IDbService
    {
        #region Methods

        /// <summary>
        ///     Do pagination on a specific list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="pagination"></param>
        /// <returns></returns>
        public IQueryable<T> Paginate<T>(IQueryable<T> list, Pagination pagination)
        {
            if (pagination == null)
                return list;
            return list.Skip(pagination.Index * pagination.Records).Take(pagination.Records);
        }

        /// <summary>
        ///     Sort a list by using specific property enumeration.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="sortDirection"></param>
        /// <param name="sortProperty"></param>
        /// <returns></returns>
        public IQueryable<T> Sort<T>(IQueryable<T> list, SortDirection sortDirection, Enum sortProperty)
        {
            string sortMethod;
            if (sortDirection == SortDirection.Ascending)
                sortMethod = "OrderBy";
            else
                sortMethod = "OrderByDescending";

            // Search parameter expression.
            var parameterExpression = Expression.Parameter(list.ElementType, "p");

            // Search name of property which should be used for sorting.
            var sortPropertyName = Enum.GetName(sortProperty.GetType(), sortProperty);
            if (string.IsNullOrEmpty(sortPropertyName))
                return list;

            // Search member expression.
            var memberExpression = Expression.Property(parameterExpression, sortPropertyName);

            var lamdaExpression = Expression.Lambda(memberExpression, parameterExpression);

            var methodCallExpression = Expression.Call(
                typeof(Queryable),
                sortMethod,
                new[] {list.ElementType, memberExpression.Type},
                list.Expression,
                Expression.Quote(lamdaExpression));

            return list.Provider.CreateQuery<T>(methodCallExpression);
        }

        /// <summary>
        ///     Search property base on searching mode.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="records"></param>
        /// <param name="property"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        public IQueryable<T> SearchPropertyText<T>(IQueryable<T> records, Func<T, string> property, TextSearch search)
        {
            if (search == null || string.IsNullOrWhiteSpace(search.Value))
                return records;

            switch (search.Mode)
            {
                case TextSearchMode.Contain:
                    records = records.Where(x => property(x).Contains(search.Value));
                    break;
                case TextSearchMode.Equal:
                    records = records.Where(x => property(x).Equals(search.Value));
                    break;
                case TextSearchMode.EqualIgnoreCase:
                    records =
                        records.Where(x => property(x).Equals(search.Value, StringComparison.CurrentCultureIgnoreCase));
                    break;
                case TextSearchMode.StartsWith:
                    records = records.Where(x => property(x).StartsWith(search.Value));
                    break;
                case TextSearchMode.StartsWithIgnoreCase:
                    records =
                        records.Where(
                            x => property(x).StartsWith(search.Value, StringComparison.CurrentCultureIgnoreCase));
                    break;
                case TextSearchMode.EndsWith:
                    records = records.Where(x => property(x).EndsWith(search.Value));
                    break;
                case TextSearchMode.EndsWithIgnoreCase:
                    records =
                        records.Where(
                            x => property(x).EndsWith(search.Value, StringComparison.CurrentCultureIgnoreCase));
                    break;
                default:
                    records = records.Where(x => property(x).ToLower().Contains(search.Value.ToLower()));
                    break;
            }
            return records;
        }

        #endregion
    }
}