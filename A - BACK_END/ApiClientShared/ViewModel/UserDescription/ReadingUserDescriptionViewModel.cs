using ApiClientShared.ViewModel.User;

namespace ApiClientShared.ViewModel.UserDescription
{
    public class ReadingUserDescriptionViewModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public  ReadingUserViewModel User { get; set; }
        public string Description { get; set; }
    }
}