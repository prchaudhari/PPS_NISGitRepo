// <copyright file="AccountInformation.cs" company="Websym Solutions Pvt. Ltd.">
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

    public class AccountInformation
    {
        #region Public Member
        public string StatementDate { get; set; }
        public string StatementPeriod { get; set; }
        public string CustomerID { get; set; }
        public string RmName { get; set; }
        public string RmContactNumber { get; set; }
        #endregion
    }
}
