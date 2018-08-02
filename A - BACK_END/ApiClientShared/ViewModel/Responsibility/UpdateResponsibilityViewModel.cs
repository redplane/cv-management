using System.ComponentModel.DataAnnotations;

namespace ApiClientShared.ViewModel.Responsibility
{
    public class UpdateResponsibilityViewModel
    {
        [Required]
        public string Name { get; set; }
    }
}