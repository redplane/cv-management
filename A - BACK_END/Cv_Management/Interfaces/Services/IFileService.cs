using System.Drawing;
using System.Threading;
using System.Threading.Tasks;

namespace Cv_Management.Interfaces.Services
{
    public interface IFileService
    {
        #region Methods

        /// <summary>
        /// Convert bytes stream to image.
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        Image GetImage(byte[] bytes);

        /// <summary>
        /// Save image to profile folder.
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="root"></param>
        /// <param name="fileName"></param>
        /// <param name="cancellationToken"></param>
        Task<string> AddFileToDirectory(byte[] bytes, string root, string fileName, CancellationToken cancellationToken);

        #endregion
    }
}