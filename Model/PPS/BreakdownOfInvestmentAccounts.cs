namespace nIS
{
    #region References
    using System;
    using System.Collections.Generic;

    #endregion
    /// <summary>
    /// the BreakdownOfInvestmentAccounts model
    /// </summary>
    public class BreakdownOfInvestmentAccounts
    {
        /// <summary>
        /// Gets or sets the investment identifier.
        /// </summary>
        /// <value>
        /// The investment identifier.
        /// </value>
        public long? InvestmentId { get; set; }
        /// <summary>
        /// Gets or sets the current interest rate.
        /// </summary>
        /// <value>
        /// The current interest rate.
        /// </value>
        public string CurrentInterestRate { get; set; }
        /// <summary>
        /// Gets or sets the interest disposal desc.
        /// </summary>
        /// <value>
        /// The interest disposal desc.
        /// </value>
        public string InterestDisposalDesc { get; set; }
        /// <summary>
        /// Gets or sets the account open date.
        /// </summary>
        /// <value>
        /// The account open date.
        /// </value>
        public DateTime? AccountOpenDate { get; set; }
        /// <summary>
        /// Gets or sets the notice period.
        /// </summary>
        /// <value>
        /// The notice period.
        /// </value>
        public string NoticePeriod { get; set; }
        /// <summary>
        /// Gets or sets the transaction date.
        /// </summary>
        /// <value>
        /// The transaction date.
        /// </value>
        public DateTime? LastTransactionDate { get; set; }
        public long? InvestorId { get; set; }
        public string ProductDescription { get; set; }
        public string AccuredInterest { get; set; }
        public string Currency { get; set; }
        /// <summary>
        /// Gets or sets the investment transaction.
        /// </summary>
        /// <value>
        /// The investment transaction.
        /// </value>
        public IList<InvestmentTransaction> InvestmentTransaction { get; set; }
    }
}
