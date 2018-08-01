namespace Cv_Management.Models
{
    public class Pagination
    {
        #region Properties

        /// <summary>
        ///     Id of result page.
        ///     Min: 0
        ///     Max: (infinite)
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        ///     Maximum records can be displayed per page.
        /// </summary>
        public int Records { get; set; }

        #endregion
    }
}