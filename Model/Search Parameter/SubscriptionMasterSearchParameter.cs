// <copyright file="SubscriptionMasterSearchParameter.cs" company="Websym Solutions Pvt Ltd">
// Copyright (c) 2018 Websym Solutions Pvt Ltd.
// </copyright>
// -----------------------------------------------------------------------  

namespace nIS
{
    #region References
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    #endregion

    /// <summary>
    /// This class represents subscription master search parameter
    /// </summary>
    /// 
    public class SubscriptionMasterSearchParameter: BaseSearchEntity
    {
        public long Identifier { get; set; }
        public string VendorName { get; set; }
        public string Subscription { get; set; }
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string EmailId { get; set; }
    }
}
