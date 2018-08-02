using System.ComponentModel.DataAnnotations;

namespace ApiClientShared.ViewModel.ProjectResponsibility
{
    public class CreateProjectResponsibilityViewModel
    {
        [Required]
        public int ProjectId { get; set; }
        [Required]
        public int ResponsibilityId { get; set; }
    }
}