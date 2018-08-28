namespace Cv_Management.Models
{
    public class AppPathModel
    {
        #region Propeties

        /// <summary>
        /// Folder to get/upload profile image.
        /// </summary>
        public string ProfileImage { get; set; }

        /// <summary>
        /// Folder to get/upload skill category image.
        /// </summary>
        public string SkillCategoryImage { get; set; }

        /// <summary>
        /// Absolute path of root.
        /// </summary>
        public string AbsoluteRoot { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initialize app path with injectors.
        /// </summary>
        /// <param name="absoluteRoot"></param>
        public AppPathModel(string absoluteRoot)
        {
            AbsoluteRoot = absoluteRoot;
        }

        #endregion
    }
}