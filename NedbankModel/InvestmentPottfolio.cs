namespace NedbankModel
{
    #region References
    using System;

    #endregion
    /// <summary>
    /// The InvestmentInformation model
    /// </summary>
    public class InvestmentPottfolio
    {
        /// <summary>
        /// Gets or sets the investor identifier.
        /// </summary>
        /// <value>
        /// The investor identifier.
        /// </value>
        public long? InvestorId { get; set; }
        /// <summary>
        /// Gets or sets the type of the product.
        /// </summary>
        /// <value>
        /// The type of the product.
        /// </value>
        public string ProductType { get; set; }
        /// <summary>
        /// Gets or sets the closing balance.
        /// </summary>
        /// <value>
        /// The closing balance.
        /// </value>
        public string ClosingBalance { get; set; }

        /// <summary>
        /// Gets or sets the statement date.
        /// </summary>
        /// <value>
        /// The statement date.
        /// </value>
        public DateTime? StatementDate { get; set; }
        /// <summary>
        /// Gets or sets the day of statement.
        /// </summary>
        /// <value>
        /// The day of statement.
        /// </value>
        public long DayOfStatement { get; set; }
        /// <summary>
        /// Gets or sets the statement period.
        /// </summary>
        /// <value>
        /// The statement period.
        /// </value>
        public string StatementPeriod { get; set; }
    }
}
