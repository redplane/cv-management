using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Hosting;
using CvManagement.Interfaces.Services;
using CvManagement.Models;

namespace CvManagement.Services
{
    public class FileService : IFileService
    {
        #region Properties

        /// <summary>
        /// App path instance.
        /// </summary>
        private readonly AppPathModel _appPath;

        #endregion

        #region Constructors

        /// <summary>
        /// Initialize service with injectors.
        /// </summary>
        /// <param name="appPath"></param>
        public FileService(AppPathModel appPath)
        {
            _appPath = appPath;
        }

        #endregion

        #region Methods

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public Image GetImage(byte[] bytes)
        {
            if (bytes == null)
                return null;

            using (var ms = new MemoryStream(bytes))
            {
                try
                {
                    return Image.FromStream(ms);
                }
                catch
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Save image to profile folder.
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="relativePath"></param>
        /// <param name="fileName"></param>
        /// <param name="cancellationToken"></param>
        public async Task<string> AddFileToDirectory(byte[] bytes, string relativePath, string fileName, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                fileName = $"{Guid.NewGuid().ToString("D")}.png";

            // Find the absolute path.
            var absolutePath = HostingEnvironment.MapPath(relativePath);

            if (string.IsNullOrWhiteSpace(absolutePath))
                return null;

            // Concatenate file path.
            absolutePath = Path.Combine(absolutePath, fileName);
            relativePath = Path.Combine(relativePath, fileName);

            using (var fileStream = new FileStream(absolutePath, FileMode.Create))
                await fileStream.WriteAsync(bytes, 0, bytes.Length, cancellationToken);

            return relativePath;
        }

        #endregion
    }
}