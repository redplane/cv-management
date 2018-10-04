using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using AutoMapper;
using Cv_Management.Interfaces.Services;
using Cv_Management.Models;
using DbEntity.Models.Entities.Context;
using Microsoft.EntityFrameworkCore;

namespace Cv_Management.Attributes
{
    public class ApiAuthorizeAttribute : AuthorizationFilterAttribute
    {
        #region Constructors

        #endregion

        #region Methods

        /// <inheritdoc />
        /// <summary>
        ///     Override this function for checking whether user is allowed to access function.
        /// </summary>
        /// <param name="httpActionContext"></param>
        /// <returns></returns>
        public override void OnAuthorization(HttpActionContext httpActionContext)
        {
            ProfileModel profile = null;
            IProfileService profileService = null;
            IMapper mapper;

            var dependencyScope = httpActionContext.Request.GetDependencyScope();

#if ALLOW_UNAUTHORZATION // Get identity service from lifetime scope.
            identityService = (IIdentityService)dependencyScope.GetService(typeof(IIdentityService));

            profile = new ProfileViewModel();
            profile.Id = 1;
            profile.Email = "sniper-warrior@gmail.com";
            profile.FullName = "Sniper warrior";
            profile.UserName = "sniper-warrior";
            profile.Level1Id = 1;
            profile.Level2Id = 2;
            profile.Roles =
(new [] {(int)RolesConstant.VinciAdministrator, (int)RolesConstant.OpuAdministrator, (int)RolesConstant.InspectionEngineer, (int)RolesConstant.TechnicalAssistant, (int)RolesConstant.Inspector, (int)RolesConstant.Reviewer, (int)RolesConstant.Approver, (int)RolesConstant.Viewer }).ToList();
            identityService.InitRequestIdentity(httpActionContext.Request, profile);

#else

            #region Principle validation

            // Find identity service.
            profileService = (IProfileService) dependencyScope.GetService(typeof(IProfileService));
            mapper = (IMapper) dependencyScope.GetService(typeof(IMapper));

            //var memoryCacheService = (IMemoryCacheService)dependencyScope.GetService(typeof(IMemoryCacheService));

            // Get profile cache service.
            var profileCacheService =
                (IValueCacheService<string, ProfileModel>) dependencyScope.GetService(
                    typeof(IValueCacheService<string, ProfileModel>));

            // FullSearch the principle of request.
            var principle = httpActionContext.RequestContext.Principal;

            // Principal is invalid.
            if (principle == null)
            {
                // Authentication allow. No need to check identity.
                if (IsAllowAnonymousRequest(httpActionContext))
                    return;

                httpActionContext.Response =
                    httpActionContext.Request.CreateErrorResponse(HttpStatusCode.Unauthorized,
                        "INVALID_AUTHENTICATION_TOKEN");
                return;
            }

            // FullSearch the identity set in principle.
            var identity = principle.Identity;
            if (identity == null)
            {
                // Authentication allow. No need to check identity.
                if (IsAllowAnonymousRequest(httpActionContext))
                    return;

                httpActionContext.Response =
                    httpActionContext.Request.CreateErrorResponse(HttpStatusCode.Unauthorized,
                        "INVALID_AUTHENTICATION_TOKEN");
                return;
            }

            #endregion

            #region Claim identity

            // FullSearch the claim identity.
            var claimIdentity = (ClaimsIdentity) identity;

            // Claim doesn't contain email.
            var pUserEmailClaim = claimIdentity.FindFirst(ClaimTypes.Email);
            if (string.IsNullOrEmpty(pUserEmailClaim?.Value))
            {
                // Authentication allow. No need to check identity.
                if (IsAllowAnonymousRequest(httpActionContext))
                    return;

                httpActionContext.Response =
                    httpActionContext.Request.CreateErrorResponse(HttpStatusCode.Unauthorized,
                        "INVALID_AUTHENTICATION_TOKEN");
                return;
            }

            // Find profile from cache by using user id.
            var email = pUserEmailClaim.Value;
            profile = profileCacheService.Read(email);

            // Profile has been found in Request.
            if (profile != null)
            {
                profileService.SetProfile(httpActionContext.Request, profile);
                return;
            }

            // Find unit of work from lifetime scope.
            var dbContext = (CvManagementDbContext) dependencyScope.GetService(typeof(DbContext));

            var users = dbContext.Users.AsQueryable();
            users = users.Where(x =>
                x.Email.Equals(email, StringComparison.InvariantCultureIgnoreCase));

            // Find the first matched user.
            var user = users.FirstOrDefault();
            if (user == null)
            {
                // Authentication allow. No need to check identity.
                if (IsAllowAnonymousRequest(httpActionContext))
                    return;

                httpActionContext.Response =
                    httpActionContext.Request.CreateErrorResponse(HttpStatusCode.Unauthorized,
                        "INVALID_AUTHENTICATION_TOKEN");
                return;
            }

            profile = mapper.Map<ProfileModel>(user);
            profileService.SetProfile(httpActionContext.Request, profile);
            profileCacheService.Add(profile.Email, profile, null);

            #endregion

#endif
        }

        /// <summary>
        ///     Whether method or controller allows anonymous requests or not.
        /// </summary>
        /// <param name="httpActionContext"></param>
        /// <returns></returns>
        private bool IsAllowAnonymousRequest(HttpActionContext httpActionContext)
        {
            return httpActionContext.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any()
                   ||
                   httpActionContext.ControllerContext.ControllerDescriptor.GetCustomAttributes<AllowAnonymousAttribute>
                       ().Any();
        }

        #endregion
    }
}