using ApiClientShared.Enums;

namespace ApiClientShared.Models
{
    public class TextSearch
    {
        #region Properties

        /// <summary>
        ///     Value of text.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        ///     Mode of text filter
        /// </summary>
        public TextSearchMode Mode { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initiate class with default settings.
        /// </summary>
        public TextSearch() { }

        /// <summary>
        /// Initiate class with settings.
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="value"></param>
        public TextSearch(TextSearchMode mode, string value)
        {
            Mode = mode;
            Value = value;
        }

        #endregion
    }
}