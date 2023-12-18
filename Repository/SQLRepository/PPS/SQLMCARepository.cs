namespace nIS
{
    #region References
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Dynamic;
    using System.Text;
    using Unity;
    #endregion
    public class SQLMCARepository : IMCARepository
    {
        #region Private Members

        /// <summary>
        /// The validation engine object
        /// </summary>
        IValidationEngine validationEngine = null;

        /// <summary>
        /// The connection string
        /// </summary>
        private string connectionString = string.Empty;

        /// <summary>
        /// The unity container
        /// </summary>
        private IUnityContainer unityContainer = null;

        /// <summary>
        /// The configurationutility
        /// </summary>
        private IConfigurationUtility configurationutility = null;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializing instance of class.
        /// </summary>
        /// <param name="unityContainer">The unity container.</param>
        public SQLMCARepository(IUnityContainer unityContainer)
        {
            this.unityContainer = unityContainer;
            this.validationEngine = new ValidationEngine();
            this.configurationutility = new ConfigurationUtility(this.unityContainer);
        }

        #endregion

        #region Public Functions

        //public IList<DM_MCAMaster> Get_DM_MCAMaster(CustomerMCASearchParameter searchParameter, string tenantCode)
        //{
        //    IList<DM_MCAMaster> Records = new List<DM_MCAMaster>();
        //    try
        //    {
        //        this.SetAndValidateConnectionString(tenantCode);
        //        string whereClause = this.WhereClauseGeneratorForCustomerMCA(searchParameter, tenantCode);
        //        using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
        //        {
        //            var MCAMasterRecords = nISEntitiesDataContext.NB_MCAMaster.Where(whereClause).ToList();
        //            if (MCAMasterRecords != null && MCAMasterRecords.Count > 0)
        //            {
        //                MCAMasterRecords.ForEach(item =>
        //                {
        //                    Records.Add(new DM_MCAMaster()
        //                    {
        //                        Identifier = item.Id,
        //                        BatchId = item.BatchId,
        //                        CustomerId = item.CustomerId,
        //                        InvestorId = item.InvestorId,
        //                        Currency = item.Currency,
        //                        VatNo = item.VatNo,
        //                        OverdraftLimit = item.OverdraftLimit,
        //                        FreeBalance = item.FreeBalance,
        //                        StatementNo = item.StatementNo,
        //                        StatementDate = item.StatementDate,
        //                        StatementFrequency = item.StatementFrequency,
        //                        MCATransactions = this.Get_DM_MCATransaction(new CustomerMCASearchParameter() { InvestorId = item.InvestorId, BatchId = searchParameter.BatchId, CustomerId = item.CustomerId }, tenantCode)?.ToList()
        //                    });
        //                });
        //            }
        //        }
        //        return Records;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        ///// <summary>
        ///// This method gets the specified list of customer home loan transaction records from personal loan transaction repository.
        ///// </summary>
        ///// <param name="searchParameter">The customer home loan search parameter</param>
        ///// <param name="tenantCode">The tenant code</param>
        ///// <returns>
        ///// Returns the list of customer home loan transaction record
        ///// </returns>
        //public IList<DM_MCATransaction> Get_DM_MCATransaction(CustomerMCASearchParameter searchParameter, string tenantCode)
        //{
        //    IList<DM_MCATransaction> Records = new List<DM_MCATransaction>();
        //    try
        //    {
        //        this.SetAndValidateConnectionString(tenantCode);
        //        string whereClause = this.WhereClauseGeneratorForCustomerMCA(searchParameter, tenantCode);
        //        using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
        //        {
        //            var MCATransactionRecords = nISEntitiesDataContext.NB_MCATransaction.Where(whereClause)?.OrderBy(it => it.TransactionDate)?.ToList();
        //            if (MCATransactionRecords != null && MCATransactionRecords.Count > 0)
        //            {
        //                MCATransactionRecords.ForEach(item =>
        //                {
        //                    Records.Add(new DM_MCATransaction()
        //                    {
        //                        Identifier = item.Id,
        //                        BatchId = item.BatchId,
        //                        CustomerId = item.CustomerId,
        //                        InvestorId = item.InvestorId,
        //                        Credit = item.Credit,
        //                        Debit = item.Debit,
        //                        Description = item.TransactionDescription,
        //                        Transaction_Date = item.TransactionDate ?? DateTime.Now,
        //                        Rate = item.Rate,
        //                        Days = item.Days,
        //                        AccuredInterest = item.AccuredInterest,
        //                        TenantCode = item.TenantCode
        //                    });
        //                });
        //            }
        //        }
        //        return Records;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        #endregion

        #region Private Method

        ///// <summary>
        ///// This method help to set and validate connection string
        ///// </summary>
        ///// <param name="tenantCode">The tenant code</param>
        ///// <exception cref="NedBankException.ConnectionStringNotFoundException"></exception>
        //private void SetAndValidateConnectionString(string tenantCode)
        //{
        //    try
        //    {
        //        this.connectionString = this.configurationutility.GetConnectionString(ModelConstant.COMMON_SECTION, ModelConstant.NIS_CONNECTION_STRING, ModelConstant.CONFIGURATON_BASE_URL, ModelConstant.TENANT_CODE_KEY, tenantCode);

        //        if (!this.validationEngine.IsValidText(this.connectionString))
        //        {
        //            throw new ConnectionStringNotFoundException(tenantCode);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //private string WhereClauseGeneratorForCustomerMCA(CustomerMCASearchParameter searchParameter, string tenantCode)
        //{
        //    StringBuilder queryString = new StringBuilder();

        //    //send account id value to this property when account master data fetching
        //    if (validationEngine.IsValidLong(searchParameter.Identifier))
        //    {
        //        queryString.Append("(" + string.Join("or ", searchParameter.Identifier.ToString().Split(',').Select(item => string.Format("Id.Equals({0}) ", item))) + ") and ");
        //    }

        //    if (validationEngine.IsValidLong(searchParameter.CustomerId))
        //    {
        //        queryString.Append("(" + string.Join("or ", searchParameter.CustomerId.ToString().Split(',').Select(item => string.Format("CustomerId.Equals({0}) ", item))) + ") and ");
        //    }

        //    if (validationEngine.IsValidLong(searchParameter.InvestorId))
        //    {
        //        queryString.Append("(" + string.Join("or ", searchParameter.InvestorId.ToString().Split(',').Select(item => string.Format("InvestorId.Equals({0}) ", item))) + ") and ");
        //    }

        //    if (validationEngine.IsValidLong(searchParameter.BatchId))
        //    {
        //        queryString.Append("(" + string.Join("or ", searchParameter.BatchId.ToString().Split(',').Select(item => string.Format("BatchId.Equals({0}) ", item))) + ") ");
        //    }

        //    if (searchParameter.WidgetFilterSetting != null && searchParameter.WidgetFilterSetting != string.Empty)
        //    {
        //        var filterEntities = JsonConvert.DeserializeObject<List<DynamicWidgetFilterEntity>>(searchParameter.WidgetFilterSetting);
        //        filterEntities.ForEach(filterEntity =>
        //        {
        //            queryString.Append(this.QueryGenerator(filterEntity));
        //        });
        //    }

        //    queryString.Append(string.Format(" and TenantCode.Equals(\"{0}\") ", tenantCode));
        //    return queryString.ToString();
        //}

        #endregion

        #region Private Methods
        //private string QueryGenerator(DynamicWidgetFilterEntity filterEntity)
        //{
        //    var queryString = string.Empty;
        //    var condtionalOp = filterEntity.ConditionalOperator != null && filterEntity.ConditionalOperator != string.Empty && filterEntity.ConditionalOperator != "0" ? filterEntity.ConditionalOperator : " and ";
        //    if (filterEntity.Operator == "EqualsTo")
        //    {
        //        queryString = queryString + condtionalOp + " " + (string.Format(filterEntity.FieldName + ".Equals(\"{0}\") ", filterEntity.Value));
        //    }
        //    else if (filterEntity.Operator == "NotEqualsTo")
        //    {
        //        queryString = queryString + condtionalOp + " " + (string.Format("!" + filterEntity.FieldName + ".Equals(\"{0}\") ", filterEntity.Value));
        //    }
        //    else if (filterEntity.Operator == "Contains")
        //    {
        //        queryString = queryString + condtionalOp + " " + (string.Format(filterEntity.FieldName + ".Contains(\"{0}\") ", filterEntity.Value));
        //    }
        //    else if (filterEntity.Operator == "NotContains")
        //    {
        //        queryString = queryString + condtionalOp + " " + (string.Format("!" + filterEntity.FieldName + ".Contains(\"{0}\") ", filterEntity.Value));
        //    }
        //    else if (filterEntity.Operator == "LessThan")
        //    {
        //        queryString = queryString + condtionalOp + " " + (string.Format(filterEntity.FieldName + " < " + filterEntity.Value + " "));
        //    }
        //    else if (filterEntity.Operator == "GreaterThan")
        //    {
        //        queryString = queryString + condtionalOp + " " + (string.Format(filterEntity.FieldName + " > " + filterEntity.Value + " "));
        //    }
        //    return queryString;
        //}
        #endregion
    }
}
