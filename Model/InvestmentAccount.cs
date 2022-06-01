
namespace nIS
{
    #region References
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    #endregion

    public class InvestmentAccount
    {
        public string InvestorId { get; set; }
        public string InvestmentId { get; set; }
        public string Currency { get; set; }
        public string ProductType { get; set; }
        public string ProductDesc { get; set; }
        public string OpenDate { get; set; }
        public string CurrentInterestRate { get; set; }
        public string ExpiryDate { get; set; }
        public string InterestDisposalDesc { get; set; }
        public string NoticePeriod { get; set; }
        public string AccuredInterest { get; set; }
        public List<InvestmentAccountTransaction> Transactions { get; set; }
    }

    public class InvestmentAccountTransaction
    {
        public string TransactionDate { get; set; }
        public string TransactionDesc { get; set; }
        public string Debit { get; set; }
        public string Credit { get; set; }
        public string Balance { get; set; }
    }
}
