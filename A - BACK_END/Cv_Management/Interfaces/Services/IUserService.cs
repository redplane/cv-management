using System.Threading;
using System.Threading.Tasks;
using Cv_Management.Models;

namespace Cv_Management.Interfaces.Services
{
    public interface IUserService
    {
        #region Methods

        /// <summary>
        /// Get user information from cache / database to check whether user is valid to login or not.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<ProfileModel> LoginAsync(string email, string password, CancellationToken cancellationToken);

        #endregion

    }
}