using System.ComponentModel.DataAnnotations;

namespace ApiClientShared.ViewModel.Hobby
{
    public class AddHobbyViewModel
    {
        /// <summary>
        ///     UserId that hobby belong to
        /// </summary>
        public int? UserId { get; set; }

        /// <summary>
        ///     Name'hobby
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        ///     Description 'hobby
        /// </summary>
        [Required]
        public string Description { get; set; }
    }
}