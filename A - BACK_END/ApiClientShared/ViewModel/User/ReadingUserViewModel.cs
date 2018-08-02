namespace ApiClientShared.ViewModel.User
{
    public class ReadingUserViewModel
    {
        public ReadingUserViewModel()
        {
            
        }
       
        public int Id { get; set; }
        public  string Email { get; set; }

        public  string Password { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Photo { get; set; }

        public double Birthday { get; set; }

        public string Role { get; set; }
    }
}