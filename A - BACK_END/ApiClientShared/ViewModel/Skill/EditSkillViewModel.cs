using System.ComponentModel.DataAnnotations;

namespace ApiClientShared.ViewModel.Skill
{
    public class EditSkillViewModel
    {
        [Required]
        public string Name { get; set; }

      
    }
}