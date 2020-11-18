// <copyright file="StatementSearch.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2018 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------

namespace nIS
{
    #region References

    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    #endregion

    public class StatementSearch
    {
        public long Identifier { get; set; }
        public long ScheduleId { get; set; }
        public long ScheduleLogId { get; set; }
        public long StatementId { get; set; }
        public Nullable<System.DateTime> StatementDate { get; set; }
        public string StatementPeriod { get; set; }
        public long CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string AccountNumber { get; set; }
        public string AccountType { get; set; }
        public string StatementURL { get; set; }
        public string TenantCode { get; set; }

        #region

        public void IsValid()
        {


            Exception exception = new Exception();

            IUtility utility = new Utility();
            ValidationEngine validationEngine = new ValidationEngine();
            //if (!validationEngine.IsValidText(this.Name, false))
            //{
            //    exception.Data.Add(utility.GetDescription("Name", typeof(Statement)), ModelConstant.PAGE_SECTION + "~" + ModelConstant.INVALID_PAGE_DISPLAY_NAME);
            //}
            //if (this.statementPages.Count==0)
            //{
            //    exception.Data.Add(utility.GetDescription("StatementPages", typeof(Statement)), ModelConstant.PAGE_SECTION + "~" + ModelConstant.INVALID_PAGE_TYPE_ID);
            //}
            //if (exception.Data.Count > 0)
            //{
            //    throw exception;
            //}
        }
        #endregion
    }
}

