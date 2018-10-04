using System.Threading;
using System.Threading.Tasks;

namespace CvManagement.Interfaces.Services
{
    public interface ICaptchaService
    {
        #region Methods

        /// <summary>
        ///     Validate captcha code to check whether it is valid or not.
        /// </summary>
        /// <param name="code"></param>
        /// <param name="clientAddress"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> IsCaptchaValidAsync(string code, string clientAddress, CancellationToken cancellationToken);

        #endregion
    }
}