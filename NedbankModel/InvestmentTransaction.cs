namespace NedbankModel
{
    #region References
    using System;

    #endregion
    /// <summary>
    /// The InvestmentTransaction model.
    /// </summary>
    public class InvestmentTransaction
    {
        /// <summary>
        /// Gets or sets the transaction date.
        /// </summary>
        /// <value>
        /// The transaction date.
        /// </value>
        public DateTime TransactionDate { get; set; }
        /// <summary>
        /// Gets or sets the transaction desc.
        /// </summary>
        /// <value>
        /// The transaction desc.
        /// </value>
        public string TransactionDesc { get; set; }
        /// <summary>
        /// Gets or sets the WJXBF s2 debit.
        /// </summary>
        /// <value>
        /// The WJXBF s2 debit.
        /// </value>
        public string WJXBFS2_Debit { get; set; }
        /// <summary>
        /// Gets or sets the WJXBF s3 credit.
        /// </summary>
        /// <value>
        /// The WJXBF s3 credit.
        /// </value>
        public string WJXBFS3_Credit { get; set; }
        /// <summary>
        /// Gets or sets the WJXBF s4 balance.
        /// </summary>
        /// <value>
        /// The WJXBF s4 balance.
        /// </value>
        public string WJXBFS4_Balance { get; set; }
    }
}
