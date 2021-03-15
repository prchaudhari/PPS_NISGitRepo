// <copyright file="CustomerInvestmentSearchParameter.cs" company="Websym Solutions Pvt Ltd">
// Copyright (c) 2020 Websym Solutions Pvt Ltd.
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
    /// This class represents customer investment search parameter
    /// </summary>
    /// 
    public class CustomerInvestmentSearchParameter
    {
        private ModelUtility utility = new ModelUtility();
        private IValidationEngine validationEngine = new ValidationEngine();
        
        public long Identifier { get; set; } //send investment id value to this property when investment master data fetching
        public long BatchId { get; set; }
        public long CustomerId { get; set; }
        public long InvestmentId { get; set; } //send investment id value to this property when investment transaction data fetching
        public long InvestorId { get; set; }
        public string WidgetFilterSetting { get; set; }

        /// <summary>
        /// Determines whether this instance of customer investment search parameter is valid.
        /// </summary>
        public void IsValid()
        {
            try
            {
                Exception exception = new Exception();

                if (validationEngine.IsValidLong(this.BatchId))
                {
                    exception.Data.Add(this.utility.GetDescription("Batch Id", typeof(State)), ModelConstant.TRANSACTION_DATA_MODEL_SECTION + "~" + ModelConstant.INVALID_BATCH_ID);
                }
                if (validationEngine.IsValidLong(this.CustomerId))
                {
                    exception.Data.Add(this.utility.GetDescription("Customer Id", typeof(State)), ModelConstant.TRANSACTION_DATA_MODEL_SECTION + "~" + ModelConstant.INVALID_CUSTOMER_ID);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
