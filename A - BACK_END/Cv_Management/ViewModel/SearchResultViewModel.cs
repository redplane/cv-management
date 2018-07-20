using System.Collections.Generic;

namespace Cv_Management.ViewModel
{
    public class SearchResultViewModel<T>
    {
        public T Records { get; set; }

        public int Total { get; set; }
    }
}