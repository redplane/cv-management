using System.ComponentModel.DataAnnotations;
using ApiMultiPartFormData.Models;

namespace Cv_Management.ViewModels.SkillCategory
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