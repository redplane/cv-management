using ApiClientShared.ViewModel.Project;
using ApiClientShared.ViewModel.Skill;

namespace ApiClientShared.ViewModel.ProjectSkill
{
    public class ReadingProjectSkillViewModel
    {
        public int ProjectId { get; set; }
        public ReadingProjectViewModel project { get; set; }

        public int SkillId { get; set; }
        public  ReadingSkillViewModel skill { get; set; }
    }
}