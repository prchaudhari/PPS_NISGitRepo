namespace nIS
{
    /// <summary>
    /// The InvestmentInformation model
    /// </summary>
    public class InvestorPerformance
    {
        /// <summary>
        /// Gets or sets the closing balance.
        /// </summary>
        /// <value>
        /// The closing balance.
        /// </value>
        public string ClosingBalance { get; set; }

        /// <summary>
        /// Gets or sets the opening balance.
        /// </summary>
        /// <value>
        /// The opening balance.
        /// </value>
        public string OpeningBalance { get; set; }

        public string ProductType { get; set; }

        public string Currency { get; set; }
    }
}
