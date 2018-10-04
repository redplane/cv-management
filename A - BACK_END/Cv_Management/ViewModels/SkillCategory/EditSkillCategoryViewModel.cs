using ApiMultiPartFormData.Models;

namespace CvManagement.ViewModels.SkillCategory
{
    public class EditSkillCategoryViewModel
    {
        #region Properties

        /// <summary>
        /// Category photo.
        /// </summary>
        public HttpFile Photo { get; set; }
        
        /// <summary>
        /// Category name.
        /// </summary>
        public string Name { get; set; }

        #endregion
    }
}