namespace ApiClientShared.ViewModel
{
    public class SearchResultViewModel<T>
    {
        public T Records { get; set; }

        public int Total { get; set; }
    }
}