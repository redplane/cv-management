using System.ComponentModel.DataAnnotations;

namespace ApiClientShared.ViewModel.PersonalSkill
{
    public class CreatePersonalSkillViewModel
    {
        [Required]
        public int SkillCategoryId { get; set; }
        [Required]
        public int SkillId { get; set; }
   
        public int Point { get; set; }
       
    }
}