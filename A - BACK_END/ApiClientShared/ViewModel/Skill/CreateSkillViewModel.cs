using System.ComponentModel.DataAnnotations;

namespace ApiClientShared.ViewModel.Skill
{
    public class CreateSkillViewModel
    {
        [Required]
        public string Name { get; set; }
      

    }
}