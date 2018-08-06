using System.ComponentModel.DataAnnotations;

namespace ApiClientShared.ViewModel.ProjectSkill
{
    public class AddProjectSkillViewModel
    {
        [Required]
        public int ProjectId { get; set; }
        [Required]
        public int SkillId { get; set; }
    }
}