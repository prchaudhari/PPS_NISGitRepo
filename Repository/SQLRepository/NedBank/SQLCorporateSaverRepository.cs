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
    public class SQLCorporateSaverRepository : ICorporateSaverRepository
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
        public SQLCorporateSaverRepository(IUnityContainer unityContainer)
        {
            this.unityContainer = unityContainer;
            this.validationEngine = new ValidationEngine();
            this.configurationutility = new ConfigurationUtility(this.unityContainer);
        }

        #endregion

        #region Public Functions

        public IList<DM_CorporateSaverMaster> Get_DM_CorporateSaverMaster(CustomerCorporateSaverSearchParameter searchParameter, string tenantCode)
        {
            IList<DM_CorporateSaverMaster> Records = new List<DM_CorporateSaverMaster>();
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                string whereClause = this.WhereClauseGeneratorForCustomerCorporateSaver(searchParameter, tenantCode);
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    // var MCAMasterRecords = nISEntitiesDataContext.NB_CorporateSaverMaster.Where(whereClause).ToList();

                    var MCAMasterRecords = nISEntitiesDataContext.NB_CorporateSaverMaster.Where(x => x.BatchId == searchParameter.BatchId & x.TenantCode == tenantCode).ToList();

                    if (MCAMasterRecords != null && MCAMasterRecords.Count > 0)
                    {
                        MCAMasterRecords.ForEach(item =>
                        {
                            Records.Add(new DM_CorporateSaverMaster()
                            {
                                Identifier = item.Id,
                                BatchId = item.BatchId,
                                CustomerId = item.CustomerId,
                                InvestorId = item.InvestorId,
                                VatNo = item.AgentVATRegNo,
                                StatementNo = item.StatementNo,
                                AgentContactDetails = item.AgentContactDetails,
                                AgentClientAddress1 = item.AgentClientAddress1,
                                AgentClientAddress2 = item.AgentClientAddress2,
                                AgentClientAddress3 = item.AgentClientAddress3,
                                AgentClientAddress4 = item.AgentClientAddress4,
                                AgentClientAddress5 = item.AgentClientAddress5,
                                AgentClientAddress6 = item.AgentClientAddress6,
                                AgentRegNo = item.AgentRegNo,
                                TaxInvoiceNo = item.TaxInvoiceNo,
                                AgentContactPerson = item.AgentContactPerson,
                                AgentEmailAddress = item.AgentEmailAddress,
                                AgentVATRegNo = item.AgentVATRegNo,
                                AgentFSPLicNo = item.AgentFSPLicNo,
                                AgentReference = item.AgentReference,
                                CIFNo = item.CIFNo,
                                BranchCode = item.BranchCode,
                                AgentProfile = item.AgentProfile,
                                ClientCode = item.ClientCode,
                                RelationshipManager = item.RelationshipManager,
                                VATCalculation = item.VATCalculation,
                                InterestInstruction = item.InterestInstruction,
                                Interest = item.Interest,
                                AgentFeeStructure1 = item.AgentFeeStructure1,
                                DateInvested = item.DateInvested,
                                AgentFeeDeducted = item.AgentFeeDeducted,
                                VatOnFee = item.VatOnFee,



                                CorporateSaverTransactions = this.Get_DM_CorporateSaverTransaction(new CustomerCorporateSaverSearchParameter()
                                {
                                    InvestorId = item.InvestorId,
                                    BatchId = searchParameter.BatchId
                                },
                                    tenantCode)?.ToList(),

                                    CorporateSaverTax = this.Get_DM_CorporateSaverTax(new CustomerCorporateSaverSearchParameter()
                                    {
                                        InvestorId = item.InvestorId,
                                        BatchId = searchParameter.BatchId
                                    },
                                    tenantCode)?.ToList()
                            }) ;
                        });
                    }
                }
                return Records;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method gets the specified list of customer home loan transaction records from personal loan transaction repository.
        /// </summary>
        /// <param name="searchParameter">The customer home loan search parameter</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of customer home loan transaction record
        /// </returns>
        public IList<DM_CorporateSaverTransaction> Get_DM_CorporateSaverTransaction(CustomerCorporateSaverSearchParameter searchParameter, string tenantCode)
        {
            IList<DM_CorporateSaverTransaction> Records = new List<DM_CorporateSaverTransaction>();
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                string whereClause = this.WhereClauseGeneratorForCustomerCorporateSaver(searchParameter, tenantCode);
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {

                    var CorporateSaverTransactionRecords = nISEntitiesDataContext.NB_CorporateSaverTransactions.
                        Where(x => x.BatchId == searchParameter.BatchId & x.InvestorId == searchParameter.InvestorId & x.TenantCode == tenantCode)?.
                        OrderBy(it => it.FromDate)?.ToList();
                    //      var CorporateSaverTransactionRecords = nISEntitiesDataContext.NB_CorporateSaverTransactions.Where(whereClause)?.OrderBy(it => it.FromDate)?.ToList();
                    if (CorporateSaverTransactionRecords != null && CorporateSaverTransactionRecords.Count > 0)
                    {
                        CorporateSaverTransactionRecords.ForEach(item =>
                        {
                            Records.Add(new DM_CorporateSaverTransaction()
                            {
                                Identifier = item.Id,
                                BatchId = item.BatchId,
                                CustomerId = item.CustomerId,
                                InvestorId = item.InvestorId,
                                FromDate = item.FromDate,
                                ToDate = item.ToDate,
                                TransactionDescription = item.TransactionDescription,
                                Rate = item.Rate,
                                Amount = item.Amount,
                                CapitalBalance = item.CapitalBalance,
                                TenantCode = item.TenantCode
                            });
                        });
                    }
                }
                return Records;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IList<DM_CorporateSaverTax> Get_DM_CorporateSaverTax(CustomerCorporateSaverSearchParameter searchParameter, string tenantCode)
        {
            IList<DM_CorporateSaverTax> Records = new List<DM_CorporateSaverTax>();
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                string whereClause = this.WhereClauseGeneratorForCustomerCorporateSaver(searchParameter, tenantCode);
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {

                    var CorporateSaverTaxRecords = nISEntitiesDataContext.NB_CorporateSaverTax.
                        Where(x => x.BatchId == searchParameter.BatchId & x.InvestorId == searchParameter.InvestorId & x.TenantCode == tenantCode)?.
                        ToList();
                    //      var CorporateSaverTransactionRecords = nISEntitiesDataContext.NB_CorporateSaverTransactions.Where(whereClause)?.OrderBy(it => it.FromDate)?.ToList();
                    if (CorporateSaverTaxRecords != null && CorporateSaverTaxRecords.Count > 0)
                    {
                        CorporateSaverTaxRecords.ForEach(item =>
                        {
                            Records.Add(new DM_CorporateSaverTax()
                            {
                                Identifier = item.Id,
                                BatchId = item.BatchId,
                                CustomerId = item.CustomerId,
                                InvestorId = item.InvestorId,
                                CapitalBalance = item.CapitalBalance,
                                InvestType = item.InvestType,
                                InterestIntruction = item.InterestIntruction,
                                DateInvested = item.DateInvested,
                                CapitalMstr = item.CapitalMstr,
                                AgentFeeDeducted = item.AgentFeeDeducted,
                                InterestMstr0 = item.InterestMstr0,
                                VatOnFeeMstr = item.VatOnFeeMstr,
                                AGENT_FEE_STRUCTURE_1 = item.AGENT_FEE_STRUCTURE_1,
                                AGENT_FEE_STRUCTURE_2 = item.AGENT_FEE_STRUCTURE_2,
                                InterestMstr = item.InterestMstr,
                                TotalCapitalMstr = item.TotalCapitalMstr,
                                TotalInterestMstr = item.TotalInterestMstr,
                                TotalAgentFeeMstr = item.TotalAgentFeeMstr,
                                VatOnFeeMstr0 = item.VatOnFeeMstr0,
                                InterestAgentFeeMstr = item.InterestAgentFeeMstr,
                                InterestDescription=item.InterestDescription,
                                TenantCode = item.TenantCode
                            });
                        });
                    }
                }
                return Records;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Private Method

        /// <summary>
        /// This method help to set and validate connection string
        /// </summary>
        /// <param name="tenantCode">The tenant code</param>
        /// <exception cref="NedBankException.ConnectionStringNotFoundException"></exception>
        private void SetAndValidateConnectionString(string tenantCode)
        {
            try
            {
                this.connectionString = this.configurationutility.GetConnectionString(ModelConstant.COMMON_SECTION, ModelConstant.NIS_CONNECTION_STRING, ModelConstant.CONFIGURATON_BASE_URL, ModelConstant.TENANT_CODE_KEY, tenantCode);

                if (!this.validationEngine.IsValidText(this.connectionString))
                {
                    throw new ConnectionStringNotFoundException(tenantCode);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string WhereClauseGeneratorForCustomerCorporateSaver(CustomerCorporateSaverSearchParameter searchParameter, string tenantCode)
        {
            StringBuilder queryString = new StringBuilder();

            //send account id value to this property when account master data fetching
            if (validationEngine.IsValidLong(searchParameter.Identifier))
            {
                queryString.Append("(" + string.Join("or ", searchParameter.Identifier.ToString().Split(',').Select(item => string.Format("Id.Equals({0}) ", item))) + ") and ");
            }

            //if (validationEngine.IsValidLong(searchParameter.CustomerId))
            //{
            //    queryString.Append("(" + string.Join("or ", searchParameter.CustomerId.ToString().Split(',').Select(item => string.Format("CustomerId.Equals({0}) ", item))) + ") and ");
            //}

            if (validationEngine.IsValidLong(Convert.ToInt64(searchParameter.InvestorId)))
            {
                queryString.Append("(" + string.Join("or ", searchParameter.InvestorId.ToString().Split(',').Select(item => string.Format("InvestorId.Equals({0}) ", item))) + ") and ");
            }

            if (validationEngine.IsValidLong(searchParameter.BatchId))
            {
                queryString.Append("(" + string.Join("or ", searchParameter.BatchId.ToString().Split(',').Select(item => string.Format("BatchId.Equals({0}) ", item))) + ") ");
            }

            if (searchParameter.WidgetFilterSetting != null && searchParameter.WidgetFilterSetting != string.Empty)
            {
                var filterEntities = JsonConvert.DeserializeObject<List<DynamicWidgetFilterEntity>>(searchParameter.WidgetFilterSetting);
                filterEntities.ForEach(filterEntity =>
                {
                    queryString.Append(this.QueryGenerator(filterEntity));
                });
            }

            queryString.Append(string.Format(" and TenantCode.Equals(\"{0}\") ", tenantCode));
            return queryString.ToString();
        }

        #endregion

        #region Private Methods
        private string QueryGenerator(DynamicWidgetFilterEntity filterEntity)
        {
            var queryString = string.Empty;
            var condtionalOp = filterEntity.ConditionalOperator != null && filterEntity.ConditionalOperator != string.Empty && filterEntity.ConditionalOperator != "0" ? filterEntity.ConditionalOperator : " and ";
            if (filterEntity.Operator == "EqualsTo")
            {
                queryString = queryString + condtionalOp + " " + (string.Format(filterEntity.FieldName + ".Equals(\"{0}\") ", filterEntity.Value));
            }
            else if (filterEntity.Operator == "NotEqualsTo")
            {
                queryString = queryString + condtionalOp + " " + (string.Format("!" + filterEntity.FieldName + ".Equals(\"{0}\") ", filterEntity.Value));
            }
            else if (filterEntity.Operator == "Contains")
            {
                queryString = queryString + condtionalOp + " " + (string.Format(filterEntity.FieldName + ".Contains(\"{0}\") ", filterEntity.Value));
            }
            else if (filterEntity.Operator == "NotContains")
            {
                queryString = queryString + condtionalOp + " " + (string.Format("!" + filterEntity.FieldName + ".Contains(\"{0}\") ", filterEntity.Value));
            }
            else if (filterEntity.Operator == "LessThan")
            {
                queryString = queryString + condtionalOp + " " + (string.Format(filterEntity.FieldName + " < " + filterEntity.Value + " "));
            }
            else if (filterEntity.Operator == "GreaterThan")
            {
                queryString = queryString + condtionalOp + " " + (string.Format(filterEntity.FieldName + " > " + filterEntity.Value + " "));
            }
            return queryString;
        }
        #endregion
    }
}
