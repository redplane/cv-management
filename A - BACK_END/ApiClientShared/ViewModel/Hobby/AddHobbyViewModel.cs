using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiClientShared.ViewModel.Hobby
{
    public class AddHobbyViewModel
    {
        /// <summary>
        /// UserId that hobby belong to
        /// </summary>
        public int? UserId { get; set; }

        /// <summary>
        /// Name'hobby
        /// </summary>
        [Required]
        public  string Name { get; set; }

        /// <summary>
        /// Description 'hobby
        /// </summary>
        [Required]
        public  string Description { get; set; }
    }
}
