// <copyright file="AccountTransaction.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2020 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------

namespace nIS
{
    #region References
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    #endregion

    public class AccountTransaction
    {
        public string AccountType { get; set; }
        public string TransactionDate { get; set; }
        public string TransactionType { get; set; }
        public string Narration { get; set; }
        public string FCY { get; set; }
        public string CurrentRate { get; set; }
        public string LCY { get; set; }
        public string Balance { get; set; }
        public string Credit { get; set; }
        public string Debit { get; set; }

    }
}
