using System.ComponentModel.DataAnnotations;

namespace ApiClientShared.ViewModel.ProjectResponsibility
{
    public class AddProjectResponsibilityViewModel
    {
        [Required]
        public int ProjectId { get; set; }
        [Required]
        public int ResponsibilityId { get; set; }
    }
}