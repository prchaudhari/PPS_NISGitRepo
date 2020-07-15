// <copyright file="AccountSummary.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2018 Websym Solutions Pvt. Ltd..
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

    public class AccountSummary
    {
        #region Public Member
        public string AccountType { get; set; }
        public string Currency { get; set; }
        public string Amount { get; set; }
        #endregion
    }
}
