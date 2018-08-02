using System.ComponentModel.DataAnnotations;

namespace ApiClientShared.ViewModel.UserDescription
{
    public class AddUserDescriptionViewModel
    {
        [Required]
        public string Description { get; set; }
    }
}