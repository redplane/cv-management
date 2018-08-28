using Newtonsoft.Json;

namespace DbEntity.Models.Entities
{
    public class ProfileActivationToken
    {
        #region Properties

        /// <summary>
        /// User email address.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Token provided by system.
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Time when token is created.
        /// </summary>
        public double CreatedTime { get; set; }

        #endregion
    }
}