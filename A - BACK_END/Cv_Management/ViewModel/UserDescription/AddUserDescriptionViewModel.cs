using System.ComponentModel.DataAnnotations;

namespace Cv_Management.ViewModel.UserDescription
{
    public class AddUserDescriptionViewModel
    {
        [Required]
        public string Description { get; set; }
    }
}