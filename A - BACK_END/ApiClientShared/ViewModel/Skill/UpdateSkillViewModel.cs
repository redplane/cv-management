using System.ComponentModel.DataAnnotations;

namespace ApiClientShared.ViewModel.Skill
{
    public class UpdateSkillViewModel
    {
        [Required]
        public string Name { get; set; }

      
    }
}