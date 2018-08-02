using System.ComponentModel.DataAnnotations;

namespace ApiClientShared.ViewModel.Responsibility
{
    public class CreateResponsibilityViewModel
    {
        [Required]
        public string Name { get; set; }
    }
}