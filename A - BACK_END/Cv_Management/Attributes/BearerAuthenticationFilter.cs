using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Filters;
using CvManagement.Interfaces.Services;

namespace CvManagement.Attributes
{
    public class BearerAuthenticationFilter : IAuthenticationFilter
    {
        #region Constructors

        #endregion

        #region Properties

        /// <summary>
        ///     Whether multiple authentication is supported or not.
        /// </summary>
        public bool AllowMultiple => false;

        #endregion

        #region Methods

        /// <inheritdoc />
        /// <summary>
        ///     Authenticate a request asynchronously.
        /// </summary>
        /// <param name="httpAuthenticationContext"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task AuthenticateAsync(HttpAuthenticationContext httpAuthenticationContext,
            CancellationToken cancellationToken)
        {
            // Get profile service from dependency scope.
            var tokenService = (ITokenService) httpAuthenticationContext.Request.GetDependencyScope()
                .GetService(typeof(ITokenService));

            // Account has been authenticated before token is parsed.
            // Skip the authentication.
            var principal = httpAuthenticationContext.Principal;
            if (principal != null && principal.Identity != null && principal.Identity.IsAuthenticated)
                return Task.FromResult(0);

            // FullSearch the authorization in the header.
            var authorization = httpAuthenticationContext.Request.Headers.Authorization;

            // Bearer token is detected.
            if (authorization == null)
                return Task.FromResult(0);

            // Scheme is not bearer.
            if (!"Bearer".Equals(authorization.Scheme,
                StringComparison.InvariantCultureIgnoreCase))
                return Task.FromResult(0);

            // Token parameter is not defined.
            var token = authorization.Parameter;
            if (string.IsNullOrWhiteSpace(token))
                return Task.FromResult(0);

            try
            {
                // FullSearch authentication provider from request sent from client.
                // Decode the token and set to claim. The object should be in dictionary.
                // Authenticate the request.
                httpAuthenticationContext.Principal = tokenService.ToPrinciple(token);
            }
            catch (Exception exception)
            {
                // Suppress the error.
                Debug.WriteLine(exception);
            }

            return Task.FromResult(0);
        }

        /// <summary>
        ///     Callback which is called after the authentication which to handle the result.
        /// </summary>
        /// <param name="httpAuthenticationChallengeContext"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task ChallengeAsync(HttpAuthenticationChallengeContext httpAuthenticationChallengeContext,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(0);
        }

        #endregion
    }
}