using System.ComponentModel.DataAnnotations;

namespace ApiClientShared.ViewModel.UserDescription
{
    public class EditUserDescriptionViewModel
    {
        [Required]
        public string Description { get; set; }
    }
}