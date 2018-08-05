using System.ComponentModel.DataAnnotations;

namespace ApiClientShared.ViewModel.Responsibility
{
    public class AddResponsibilityViewModel
    {
        [Required]
        public string Name { get; set; }
    }
}