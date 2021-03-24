﻿// <copyright file="CustomerInvestmentSearchParameter.cs" company="Websym Solutions Pvt Ltd">
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
    /// This class represents messages or explanatory notes search parameter
    /// </summary>
    /// 
    public class MessageAndNoteSearchParameter
    {
        private ModelUtility utility = new ModelUtility();
        private IValidationEngine validationEngine = new ValidationEngine();

        public long Identifier { get; set; }
        public long BatchId { get; set; }
        public long CustomerId { get; set; }
        public string WidgetFilterSetting { get; set; }

        /// <summary>
        /// Determines whether this instance of messages or explanatory notes search parameter is valid.
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
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
