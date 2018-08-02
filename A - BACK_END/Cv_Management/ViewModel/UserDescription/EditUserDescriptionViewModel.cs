using System.ComponentModel.DataAnnotations;

namespace Cv_Management.ViewModel.UserDescription
{
    public class EditUserDescriptionViewModel
    {
        [Required]
        public string Description { get; set; }
    }
}