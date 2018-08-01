namespace Cv_Management.Models
{
    public class RangeModel<TFrom, TTo>
    {
        #region Properties

        /// <summary>
        /// From
        /// </summary>
        public TFrom From { get; set; }

        /// <summary>
        /// To
        /// </summary>
        public TTo To { get; set; }

        #endregion

        #region Constructors

        public RangeModel()
        {

        }

        public RangeModel(TFrom from, TTo to)
        {
            From = from;
            To = to;
        }

        #endregion
    }
}