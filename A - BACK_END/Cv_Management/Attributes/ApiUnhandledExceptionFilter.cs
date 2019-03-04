﻿using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Filters;

namespace CvManagement.Attributes
{
    public class ApiUnhandledExceptionFilter : IExceptionFilter
    {
        #region Constructors

        /// <summary>
        ///     Default constructor.
        /// </summary>
        public ApiUnhandledExceptionFilter()
        {
        }

        #endregion

        #region Methods

        /// <inheritdoc />
        /// <summary>
        ///     Execute exception filter to log 'em all.
        /// </summary>
        /// <param name="actionExecutedContext"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task ExecuteExceptionFilterAsync(HttpActionExecutedContext actionExecutedContext,
            CancellationToken cancellationToken)
        {
            if (actionExecutedContext == null)
                return Task.FromResult(0);

            if (actionExecutedContext.Exception == null)
                return Task.FromResult(0);

            var exception = actionExecutedContext.Exception;

            //if (exception is DbEntityValidationException)
            //{
            //    var dbEntityValidationException = (DbEntityValidationException)exception;
            //    Debug.WriteLine(dbEntityValidationException);
            //    return Task.FromResult(1);
            //}

            if (exception is HttpException)
            {
                var httpException = (HttpException) exception;
                actionExecutedContext.Response =
                    actionExecutedContext.Request.CreateErrorResponse((HttpStatusCode) httpException.GetHttpCode(),
                        httpException.Message);
                return Task.FromResult(1);
            }

            Debug.WriteLine(exception.Message, exception);

            return Task.FromResult(1);
        }

        #endregion

        #region Properties

        /// <inheritdoc />
        /// <summary>
        ///     Allow multiple filter to run or not.
        /// </summary>
        public bool AllowMultiple => true;
        
        #endregion
    }
}