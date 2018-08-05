using System.ComponentModel.DataAnnotations;

namespace ApiClientShared.ViewModel.Responsibility
{
    public class EditResponsibilityViewModel
    {
        [Required]
        public string Name { get; set; }
    }
}