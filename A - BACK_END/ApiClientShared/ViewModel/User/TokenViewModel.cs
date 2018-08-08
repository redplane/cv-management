namespace ApiClientShared.ViewModel.User
{
    public class TokenViewModel
    {
        /// <summary>
        /// Code for accessing system.
        /// </summary>
        public  string AccessToken { get; set; }

        /// <summary>
        /// Token life time (in second)
        /// </summary>
        public  int LifeTime { get; set; }

        /// <summary>
        /// Token type.
        /// </summary>
        public  string Type { get; set; }
    }
}