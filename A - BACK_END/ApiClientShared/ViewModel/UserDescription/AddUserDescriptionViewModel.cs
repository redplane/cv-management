using System.ComponentModel.DataAnnotations;

namespace ApiClientShared.ViewModel.UserDescription
{
    public class AddUserDescriptionViewModel
    {
        public int? UserId { get; set; }

        [Required]
        public string Description { get; set; }
    }
}