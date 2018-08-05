using System.ComponentModel.DataAnnotations;

namespace ApiClientShared.ViewModel.Skill
{
    public class AddSkillViewModel
    {
        [Required]
        public string Name { get; set; }
      

    }
}