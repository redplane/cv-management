using System.Data.Common;
using System.Data.Entity.Infrastructure.Interception;
using System.Diagnostics;

namespace Cv_Management.Interceptor
{
    public class GlobalDbCommandInterceptor : IDbCommandInterceptor
    {
        #region Methods

        /// <summary>
        ///     <inheritdoc />
        /// </summary>
        /// <param name="command"></param>
        /// <param name="interceptionContext"></param>
        public void NonQueryExecuting(DbCommand command, DbCommandInterceptionContext<int> interceptionContext)
        {
            Debug.WriteLine(command.CommandText);
        }

        /// <summary>
        ///     <inheritdoc />
        /// </summary>
        /// <param name="command"></param>
        /// <param name="interceptionContext"></param>
        public void NonQueryExecuted(DbCommand command, DbCommandInterceptionContext<int> interceptionContext)
        {
            Debug.WriteLine(command.CommandText);
        }

        /// <summary>
        ///     <inheritdoc />
        /// </summary>
        /// <param name="command"></param>
        /// <param name="interceptionContext"></param>
        public void ReaderExecuting(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext)
        {
            Debug.WriteLine(command.CommandText);
        }

        /// <summary>
        ///     <inheritdoc />
        /// </summary>
        /// <param name="command"></param>
        /// <param name="interceptionContext"></param>
        public void ReaderExecuted(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext)
        {
            Debug.WriteLine(command.CommandText);
        }

        /// <summary>
        ///     <inheritdoc />
        /// </summary>
        /// <param name="command"></param>
        /// <param name="interceptionContext"></param>
        public void ScalarExecuting(DbCommand command, DbCommandInterceptionContext<object> interceptionContext)
        {
            Debug.WriteLine(command.CommandText);
        }

        /// <summary>
        ///     <inheritdoc />
        /// </summary>
        /// <param name="command"></param>
        /// <param name="interceptionContext"></param>
        public void ScalarExecuted(DbCommand command, DbCommandInterceptionContext<object> interceptionContext)
        {
            Debug.WriteLine(command.CommandText);
        }

        #endregion
    }
}