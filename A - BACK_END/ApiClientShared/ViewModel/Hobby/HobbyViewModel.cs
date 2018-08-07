using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiClientShared.ViewModel.Hobby
{
    public class HobbyViewModel
    {
        /// <summary>
        /// Id of hobby
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Name of hobby
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// user that hobby belong to
        /// </summary>

        public int UserId { get; set; }

        /// <summary>
        /// Description of hobby
        /// </summary>

        public string Description { get; set; }

    }
}
