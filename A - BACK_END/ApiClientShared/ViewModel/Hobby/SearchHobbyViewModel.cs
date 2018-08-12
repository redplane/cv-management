using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiClientShared.ViewModel.Hobby
{
    public class SearchHobbyViewModel : BaseSearchViewModel
    {
        /// <summary>
        /// Id' hobbies
        /// </summary>
        public  HashSet<int> Ids { get; set; }

        /// <summary>
        /// UserIds that hobby belong to
        /// </summary>
        public  HashSet<int> UserIds { get; set; }

        /// <summary>
        /// Name' hobbies
        /// </summary>
        public  HashSet<string> Names { get; set; }
    }
}
