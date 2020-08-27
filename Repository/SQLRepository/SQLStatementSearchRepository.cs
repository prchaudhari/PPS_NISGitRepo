// <copyright file="SQLStatementSearchRepository.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2018 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------

namespace nIS
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    #region References
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Linq.Dynamic;
    using System.Security.Claims;
    using System.Text;
    using System.Text.RegularExpressions;
    using Unity;
    #endregion

    public class SQLStatementSearchRepository : IStatementSearchRepository
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
        /// The utility object
        /// </summary>
        private IUtility utility = null;

        /// <summary>
        /// The unity container
        /// </summary>
        private IUnityContainer unityContainer = null;

        /// <summary>
        /// The utility object
        /// </summary>
        private IConfigurationUtility configurationutility = null;

        /// <summary>
        /// The statement repository.
        /// </summary>
        private IStatementRepository statementRepository = null;

        /// <summary>
        /// The Page repository.
        /// </summary>
        private IPageRepository pageRepository = null;

        /// <summary>
        /// The Asset repository.
        /// </summary>
        private IAssetLibraryRepository assetLibraryRepository = null;

        #endregion

        #region Constructor

        public SQLStatementSearchRepository(IUnityContainer unityContainer)
        {
            this.unityContainer = unityContainer;
            this.validationEngine = new ValidationEngine();
            this.utility = new Utility();
            this.configurationutility = new ConfigurationUtility(this.unityContainer);
            this.pageRepository = this.unityContainer.Resolve<IPageRepository>();
            this.statementRepository = this.unityContainer.Resolve<IStatementRepository>();
            this.assetLibraryRepository = this.unityContainer.Resolve<IAssetLibraryRepository>();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// This method adds the specified list of statements in statement repository.
        /// </summary>
        /// <param name="statements">The list of statements</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns true if statements are added successfully, else false.
        /// </returns>
        public bool AddStatementSearchs(IList<StatementSearch> statements, string tenantCode)
        {
            bool result = false;
            try
            {
                var claims = ClaimsPrincipal.Current.Identities.First().Claims.ToList();
                int userId;
                int.TryParse(claims?.FirstOrDefault(x => x.Type.Equals("UserId", StringComparison.OrdinalIgnoreCase)).Value, out userId);

                this.SetAndValidateConnectionString(tenantCode);

                IList<StatementMetadataRecord> statementRecords = new List<StatementMetadataRecord>();
                statements.ToList().ForEach(statement =>
                {
                    statementRecords.Add(new StatementMetadataRecord()
                    {
                        Id = statement.Identifier,
                        ScheduleId = statement.ScheduleId,
                        ScheduleLogId = statement.ScheduleLogId,
                        StatementId = statement.StatementId,
                        StatementDate = statement.StatementDate,
                        StatementPeriod = statement.StatementPeriod,
                        CustomerId = statement.CustomerId,
                        CustomerName = statement.CustomerName,
                        AccountNumber = statement.AccountNumber,
                        AccountType = statement.AccountType,
                        StatementURL = statement.StatementURL,
                    });
                });

                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    nISEntitiesDataContext.StatementMetadataRecords.AddRange(statementRecords);
                    nISEntitiesDataContext.SaveChanges();
                }

                result = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        /// <summary>
        /// This method deletes the specified list of statements from statement repository.
        /// </summary>
        /// <param name="statementIdentifier">The statement identifier</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns true if statements are deleted successfully, else false.
        /// </returns>
        public bool DeleteStatementSearchs(long statementIdentifier, string tenantCode)
        {
            bool result = false;
            try
            {
                var claims = ClaimsPrincipal.Current.Identities.First().Claims.ToList();
                int userId;
                int.TryParse(claims?.FirstOrDefault(x => x.Type.Equals("UserId", StringComparison.OrdinalIgnoreCase)).Value, out userId);

                this.SetAndValidateConnectionString(tenantCode);

                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {

                }
                result = true;
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return result;
        }

        /// <summary>
        /// This method gets the specified list of statements from statement repository.
        /// </summary>
        /// <param name="statementSearchParameter">The statement search parameter</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of statements
        /// </returns>
        public IList<StatementSearch> GetStatementSearchs(StatementSearchSearchParameter statementSearchParameter, string tenantCode)
        {
            IList<StatementSearch> statements = new List<StatementSearch>();

            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                string whereClause = this.WhereClauseGenerator(statementSearchParameter, tenantCode);

                IList<StatementMetadataRecord> statementRecords = new List<StatementMetadataRecord>();
                IList<UserRecord> statementOwnerUserRecords = new List<UserRecord>();
                IList<UserRecord> statementPublishedUserRecords = new List<UserRecord>();
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    if(string.IsNullOrEmpty(whereClause))
                    {
                        if (statementSearchParameter.PagingParameter.PageIndex > 0 && statementSearchParameter.PagingParameter.PageSize > 0)
                        {
                            statementRecords = nISEntitiesDataContext.StatementMetadataRecords
                            .OrderBy(statementSearchParameter.SortParameter.SortColumn + " " + statementSearchParameter.SortParameter.SortOrder.ToString())
                            .Skip((statementSearchParameter.PagingParameter.PageIndex - 1) * statementSearchParameter.PagingParameter.PageSize)
                            .Take(statementSearchParameter.PagingParameter.PageSize)
                            .ToList();
                        }
                        else
                        {
                            statementRecords = nISEntitiesDataContext.StatementMetadataRecords
                            .OrderBy(statementSearchParameter.SortParameter.SortColumn + " " + statementSearchParameter.SortParameter.SortOrder.ToString().ToLower())
                            .ToList();
                        }
                    }
                    else
                    {
                        if (statementSearchParameter.PagingParameter.PageIndex > 0 && statementSearchParameter.PagingParameter.PageSize > 0)
                        {
                            statementRecords = nISEntitiesDataContext.StatementMetadataRecords
                            .OrderBy(statementSearchParameter.SortParameter.SortColumn + " " + statementSearchParameter.SortParameter.SortOrder.ToString())
                            .Where(whereClause)
                            .Skip((statementSearchParameter.PagingParameter.PageIndex - 1) * statementSearchParameter.PagingParameter.PageSize)
                            .Take(statementSearchParameter.PagingParameter.PageSize)
                            .ToList();
                        }
                        else
                        {
                            statementRecords = nISEntitiesDataContext.StatementMetadataRecords
                            .Where(whereClause)
                            .OrderBy(statementSearchParameter.SortParameter.SortColumn + " " + statementSearchParameter.SortParameter.SortOrder.ToString().ToLower())
                            .ToList();
                        }
                    }
                
                }

                if (statementRecords != null && statementRecords.ToList().Count > 0)
                {
                    statementRecords?.ToList().ForEach(statementRecord =>
                    {
                        statements.Add(new StatementSearch
                        {
                            Identifier = statementRecord.Id,
                            ScheduleId = statementRecord.ScheduleId,
                            ScheduleLogId = statementRecord.ScheduleLogId,
                            StatementId = statementRecord.StatementId,
                            StatementDate = DateTime.SpecifyKind((DateTime)statementRecord.StatementDate, DateTimeKind.Utc),
                            StatementPeriod = statementRecord.StatementPeriod,
                            CustomerId = statementRecord.CustomerId,
                            CustomerName = statementRecord.CustomerName,
                            AccountNumber = statementRecord.AccountNumber,
                            AccountType = statementRecord.AccountType,
                            StatementURL = statementRecord.StatementURL,
                        });
                    });
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return statements;
        }

        /// <summary>
        /// This method reference to get statement count
        /// </summary>
        /// <param name="statementSearchParameter"></param>
        /// <param name="tenantCode"></param>
        /// <returns>StatementSearch count</returns>
        public int GetStatementSearchCount(StatementSearchSearchParameter statementSearchParameter, string tenantCode)
        {
            int statementCount = 0;
            string whereClause = this.WhereClauseGenerator(statementSearchParameter, tenantCode);
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    statementCount = nISEntitiesDataContext.StatementMetadataRecords.Where(whereClause.ToString()).Count();
                }
            }
            catch (Exception)
            {
                throw;
            }

            return statementCount;
        }

        /// <summary>
        /// This method reference to generate html statement for export to pdf
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="tenantCode"></param>
        /// <returns>output location of html statmeent file</returns>
        public string GenerateStatement(long identifier, string tenantCode)
        {
            string outputlocation = string.Empty;
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    var statementrecords = nISEntitiesDataContext.StatementMetadataRecords.Where(item => item.Id == identifier)?.ToList();
                    if (statementrecords.Count != 0)
                    {
                        var statementrecord = statementrecords.FirstOrDefault();
                        var schedulerecord = nISEntitiesDataContext.ScheduleRecords.Where(item => item.Id == statementrecord.ScheduleId && item.TenantCode == tenantCode).ToList().FirstOrDefault();
                        var batchrecord = nISEntitiesDataContext.BatchMasterRecords.Where(item => item.ScheduleId == schedulerecord.Id && item.TenantCode == tenantCode).ToList().FirstOrDefault();
                        var customerrecord = nISEntitiesDataContext.CustomerMasterRecords.Where(item => item.Id == statementrecord.CustomerId && item.BatchId == batchrecord.Id).ToList().FirstOrDefault();
                        var batchDetails = nISEntitiesDataContext.BatchDetailRecords.Where(item => item.BatchId == batchrecord.Id)?.ToList();

                        StatementSearchParameter statementSearchParameter = new StatementSearchParameter
                        {
                            Identifier = schedulerecord.StatementId,
                            IsActive = true,
                            IsStatementPagesRequired = true,
                            PagingParameter = new PagingParameter
                            {
                                PageIndex = 0,
                                PageSize = 0,
                            },
                            SortParameter = new SortParameter()
                            {
                                SortOrder = SortOrder.Ascending,
                                SortColumn = "Name",
                            },
                            SearchMode = SearchMode.Equals
                        };
                        var statements = this.statementRepository.GetStatements(statementSearchParameter, tenantCode);
                        if (statements.Count > 0)
                        {
                            var statement = statements.FirstOrDefault();
                            var statementPageContents = this.statementRepository.GenerateHtmlFormatOfStatement(statement, tenantCode);
                            outputlocation = AppDomain.CurrentDomain.BaseDirectory + "\\Resources\\temp"+ DateTime.Now.ToString().Replace("-", "_").Replace(":", "_").Replace(" ", "_").Replace('/', '_');
                            if (!Directory.Exists(outputlocation))
                            {
                                Directory.CreateDirectory(outputlocation);
                            }
                            this.utility.DirectoryCopy(AppDomain.CurrentDomain.BaseDirectory + "\\Resources\\css", outputlocation, false);
                            this.utility.DirectoryCopy(AppDomain.CurrentDomain.BaseDirectory + "\\Resources\\js", outputlocation, false);
                            this.utility.DirectoryCopy(AppDomain.CurrentDomain.BaseDirectory + "\\Resources\\images", outputlocation, false);
                            this.utility.DirectoryCopy(AppDomain.CurrentDomain.BaseDirectory + "\\Resources\\fonts", outputlocation, false);
                            this.GenerateStatements(customerrecord, statement, statementPageContents, batchrecord, batchDetails, tenantCode, outputlocation);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return outputlocation;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Generate string for dynamic linq.
        /// </summary>
        /// <param name="searchParameter">Role search Parameters</param>
        /// <returns>
        /// Returns a string.
        /// </returns>
        private string WhereClauseGenerator(StatementSearchSearchParameter searchParameter, string tenantCode)
        {
            StringBuilder queryString = new StringBuilder();

            if (searchParameter.SearchMode == SearchMode.Equals)
            {
                if (validationEngine.IsValidText(searchParameter.Identifier))
                {
                    queryString.Append("(" + string.Join("or ", searchParameter.Identifier.ToString().Split(',').Select(item => string.Format("Id.Equals({0}) ", item))) + ") and ");
                }
                if (validationEngine.IsValidText(searchParameter.Name))
                {
                    queryString.Append(string.Format("Name.Equals(\"{0}\") and ", searchParameter.Name));
                }
                if (validationEngine.IsValidText(searchParameter.StatementCustomer))
                {
                    queryString.Append(string.Format("CustomerName.Equals(\"{0}\") and ", searchParameter.StatementCustomer));
                }
                if (validationEngine.IsValidText(searchParameter.StatementAccount))
                {
                    queryString.Append(string.Format("AccountNumber.Contains(\"{0}\") and ", searchParameter.StatementAccount));
                }
                if (validationEngine.IsValidText(searchParameter.StatementPeriod))
                {
                    queryString.Append(string.Format("StatementPeriod.Contains(\"{0}\") and ", searchParameter.StatementPeriod));
                }
            }
            if (searchParameter.SearchMode == SearchMode.Contains)
            {
                if (validationEngine.IsValidText(searchParameter.Name))
                {
                    queryString.Append(string.Format("Name.Contains(\"{0}\") and ", searchParameter.Name));
                }
                if (validationEngine.IsValidText(searchParameter.StatementCustomer))
                {
                    queryString.Append(string.Format("CustomerName.Contains(\"{0}\") and ", searchParameter.StatementCustomer));
                }
                if (validationEngine.IsValidText(searchParameter.StatementAccount))
                {
                    queryString.Append(string.Format("AccountNumber.Contains(\"{0}\") and ", searchParameter.StatementAccount));
                }
                if (validationEngine.IsValidText(searchParameter.StatementPeriod))
                {
                    queryString.Append(string.Format("StatementPeriod.Contains(\"{0}\") and ", searchParameter.StatementPeriod));
                }
            }
            if (this.validationEngine.IsValidDate(searchParameter.StatementStartDate))
            {
                DateTime fromDateTime = DateTime.SpecifyKind(Convert.ToDateTime(searchParameter.StatementStartDate), DateTimeKind.Utc);
                DateTime toDateTime = DateTime.SpecifyKind(Convert.ToDateTime(searchParameter.StatementEndDate), DateTimeKind.Utc);
                //DateTime fromDateTime = searchParameter.StatementStartDate;
                //DateTime toDateTime = searchParameter.StatementEndDate;

                queryString.Append("StatementDate >= DateTime(" + fromDateTime.Year + "," + fromDateTime.Month + "," + fromDateTime.Day + "," + fromDateTime.Hour + "," + fromDateTime.Minute + "," + fromDateTime.Second + ") " +
                               "and StatementDate <= DateTime(" + +toDateTime.Year + "," + toDateTime.Month + "," + toDateTime.Day + "," + toDateTime.Hour + "," + toDateTime.Minute + "," + toDateTime.Second + ") and ");
            }
            if (queryString.ToString() != string.Empty)
            {
                queryString.Remove(queryString.Length - 4, 4);
            }
            //queryString.Append(string.Format("TenantCode.Equals(\"{0}\") and IsDeleted.Equals(false)", tenantCode));
            return queryString.ToString();
        }

        /// <summary>
        /// This method help to set and validate connection string
        /// </summary>
        /// <param name="tenantCode">
        /// The tenant code
        /// </param>
        private void SetAndValidateConnectionString(string tenantCode)
        {
            try
            {
                this.connectionString = validationEngine.IsValidText(this.connectionString) ? this.connectionString : this.configurationutility.GetConnectionString(ModelConstant.COMMON_SECTION, ModelConstant.NIS_CONNECTION_STRING, ModelConstant.CONFIGURATON_BASE_URL, ModelConstant.TENANT_CODE_KEY, tenantCode);
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

        /// <summary>
        /// This method help to generate statement for customer
        /// </summary>
        /// <param name="customer"> the customer object </param>
        /// <param name="statement"> the statement object </param>
        /// <param name="statementPageContents"> the statement page html content list</param>
        /// <param name="batchMaster"> the batch master object </param>
        /// <param name="batchDetails"> the list of batch details records </param>
        /// <param name="outputLocation"> the output file path </param>
        private string GenerateStatements(CustomerMasterRecord customer, Statement statement, IList<StatementPageContent> statementPageContents, BatchMasterRecord batchMaster, IList<BatchDetailRecord> batchDetails, string tenantCode, string outputLocation)
        {
            string filePath = string.Empty;
            try
            {
                if (statementPageContents.Count > 0)
                {
                    string currency = string.Empty;
                    IList<AccountMasterRecord> accountrecords = new List<AccountMasterRecord>();
                    IList<AccountMasterRecord> savingaccountrecords = new List<AccountMasterRecord>();
                    IList<AccountMasterRecord> curerntaccountrecords = new List<AccountMasterRecord>();
                    IList<CustomerMediaRecord> customerMedias = new List<CustomerMediaRecord>();

                    using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                    {
                        accountrecords = nISEntitiesDataContext.AccountMasterRecords.Where(item => item.CustomerId == customer.Id && item.BatchId == batchMaster.Id)?.ToList();
                        customerMedias = nISEntitiesDataContext.CustomerMediaRecords.Where(item => item.CustomerId == customer.Id && item.StatementId == statement.Identifier && item.BatchId == batchMaster.Id)?.ToList();
                    }

                    StringBuilder htmlbody = new StringBuilder();
                    currency = accountrecords[0].Currency;
                    string navbarHtml = HtmlConstants.NAVBAR_HTML.Replace("{{BrandLogo}}", "./absa-logo.png");
                    navbarHtml = navbarHtml.Replace("{{logo}}", "./nisLogo.png");
                    navbarHtml = navbarHtml.Replace("{{Today}}", DateTime.Now.ToString("dd MMM yyyy"));
                    htmlbody.Append(HtmlConstants.CONTAINER_DIV_HTML_HEADER);

                    //start to render actual html content data
                    StringBuilder scriptHtmlRenderer = new StringBuilder();
                    StringBuilder navbar = new StringBuilder();
                    int accountCount = 0;
                    string accountNumber = string.Empty;
                    string accountType = string.Empty;
                    long accountId = 0;
                    string SavingTrendChartJson = string.Empty;
                    string SpendingTrendChartJson = string.Empty;
                    string AnalyticsChartJson = string.Empty;
                    string SavingTransactionGridJson = string.Empty;
                    string CurrentTransactionGridJson = string.Empty;
                    var newStatementPageContents = new List<StatementPageContent>();
                    statementPageContents.ToList().ForEach(it => newStatementPageContents.Add(new StatementPageContent()
                    {
                        Id = it.Id,
                        PageId = it.PageId,
                        PageTypeId = it.PageTypeId,
                        HtmlContent = it.HtmlContent,
                        PageHeaderContent = it.PageHeaderContent,
                        PageFooterContent = it.PageFooterContent,
                        DisplayName = it.DisplayName,
                        TabClassName = it.TabClassName
                    }));

                    long FirstPageId = statement.Pages[0].Identifier;
                    for (int i = 0; i < statement.Pages.Count; i++)
                    {
                        var page = statement.Pages[i];
                        StatementPageContent statementPageContent = newStatementPageContents.Where(item => item.PageTypeId == page.PageTypeId && item.Id == i).FirstOrDefault();

                        if (page.PageTypeId == HtmlConstants.HOME_PAGE_TYPE_ID)
                        {
                            accountCount = 1;
                        }
                        else if (page.PageTypeId == HtmlConstants.SAVING_ACCOUNT_PAGE_TYPE_ID)
                        {
                            savingaccountrecords = accountrecords.Where(item => item.CustomerId == customer.Id && item.BatchId == batchMaster.Id && item.AccountType.ToLower().Contains("saving"))?.ToList();
                            accountCount = savingaccountrecords.Count;
                        }
                        else if (page.PageTypeId == HtmlConstants.CURRENT_ACCOUNT_PAGE_TYPE_ID)
                        {
                            curerntaccountrecords = accountrecords.Where(item => item.CustomerId == customer.Id && item.BatchId == batchMaster.Id && item.AccountType.ToLower().Contains("current"))?.ToList();
                            accountCount = curerntaccountrecords.Count;
                        }

                        StringBuilder SubTabs = new StringBuilder();
                        StringBuilder PageHeaderContent = new StringBuilder(statementPageContent.PageHeaderContent);

                        string tabClassName = Regex.Replace((statementPageContent.DisplayName + "-" + page.Identifier), @"\s+", "-");
                        navbar.Append(" <li class='nav-item'><a class='nav-link pt-1 " + (i == 0 ? "active" : "") + " " + tabClassName + "' href='#"+ tabClassName + "' >" + statementPageContent.DisplayName + "</a> </li> ");
                        //string ExtraClassName = i > 0 ? "d-none " + tabClassName : tabClassName;
                        PageHeaderContent.Replace("{{ExtraClass}}", tabClassName);
                        PageHeaderContent.Replace("{{DivId}}", tabClassName);

                        StringBuilder newPageContent = new StringBuilder();
                        newPageContent.Append(HtmlConstants.PAGE_TAB_CONTENT_HEADER);

                        for (int x = 0; x < accountCount; x++)
                        {
                            accountNumber = string.Empty;
                            accountType = string.Empty;
                            if (page.PageTypeId == HtmlConstants.SAVING_ACCOUNT_PAGE_TYPE_ID)
                            {
                                accountNumber = savingaccountrecords[x].AccountNumber;
                                accountId = savingaccountrecords[x].Id;
                                accountType = savingaccountrecords[x].AccountType;
                            }
                            if (page.PageTypeId == HtmlConstants.CURRENT_ACCOUNT_PAGE_TYPE_ID)
                            {
                                accountNumber = curerntaccountrecords[x].AccountNumber;
                                accountId = curerntaccountrecords[x].Id;
                                accountType = curerntaccountrecords[x].AccountType;
                            }

                            string lastFourDigisOfAccountNumber = accountNumber.Length > 4 ? accountNumber.Substring(Math.Max(0, accountNumber.Length - 4)) : accountNumber;
                            if (page.PageTypeId == HtmlConstants.SAVING_ACCOUNT_PAGE_TYPE_ID || page.PageTypeId == HtmlConstants.CURRENT_ACCOUNT_PAGE_TYPE_ID)
                            {
                                if (x == 0)
                                {
                                    SubTabs.Append("<ul class='navbar-nav nav'>");
                                }

                                SubTabs.Append("<li class='nav-item " + (x == 0 ? "active" : "") + "'><a id='tab" + x + "-tab' " + "href='#" + (page.PageTypeId == HtmlConstants.SAVING_ACCOUNT_PAGE_TYPE_ID ? "Saving" : "Current") + "-" + lastFourDigisOfAccountNumber +"' class='nav-link " + (x == 0 ? "active" : "") + "'> Account - " + lastFourDigisOfAccountNumber + "</a></li>");

                                newPageContent.Append("<div id='" + (page.PageTypeId == HtmlConstants.SAVING_ACCOUNT_PAGE_TYPE_ID ? "Saving" : "Current") +
                                    "-" + lastFourDigisOfAccountNumber + "' class='tab-pane fade in active show'");

                                if (page.PageTypeId == HtmlConstants.SAVING_ACCOUNT_PAGE_TYPE_ID)
                                {
                                    newPageContent.Append("<input type='hidden' id='SavingAccountId' name='SavingAccountId' value='" + accountId + "'>");
                                }
                                else
                                {
                                    newPageContent.Append("<input type='hidden' id='CurrentAccountId' name='CurrentAccountId' value='" + accountId + "'>");
                                }

                                if (x == accountCount - 1)
                                {
                                    SubTabs.Append("</ul>");
                                }
                            }

                            var pagewidgets = page.PageWidgets;
                            StringBuilder pageContent = new StringBuilder(statementPageContent.HtmlContent);
                            for (int j = 0; j < pagewidgets.Count; j++)
                            {
                                var widget = pagewidgets[j];
                                if (widget.WidgetId == HtmlConstants.CUSTOMER_INFORMATION_WIDGET_ID) //Customer Information Widget
                                {
                                    pageContent.Replace("{{CustomerName}}", (customer.FirstName.Trim() + " " + (customer.MiddleName == string.Empty ? string.Empty : " " + customer.MiddleName.Trim()) + " " + customer.LastName.Trim()));
                                    pageContent.Replace("{{Address1}}", customer.AddressLine1);
                                    string address2 = (customer.AddressLine2 != "" ? customer.AddressLine2 + ", " : "") + (customer.City != "" ? customer.City + ", " : "") + (customer.State != "" ? customer.State + ", " : "") + (customer.Country != "" ? customer.Country + ", " : "") + (customer.Zip != "" ? customer.Zip : "");
                                    pageContent.Replace("{{Address2}}", address2);

                                    var custMedia = customerMedias.Where(item => item.PageId == page.Identifier && item.WidgetId == widget.Identifier)?.ToList()?.FirstOrDefault();
                                    if (custMedia != null && custMedia.VideoURL != string.Empty)
                                    {
                                        pageContent.Replace("{{VideoSource_" + statement.Identifier + "_" + page.Identifier + "_" + widget.Identifier + "}}", custMedia.VideoURL);
                                    }
                                    else
                                    {
                                        var batchDetail = batchDetails.Where(item => item.StatementId == statement.Identifier && item.WidgetId == widget.Identifier && item.PageId == page.Identifier)?.ToList()?.FirstOrDefault();
                                        if (batchDetail != null && batchDetail.VideoURL != string.Empty)
                                        {
                                            pageContent.Replace("{{VideoSource_" + statement.Identifier + "_" + page.Identifier + "_" + widget.Identifier + "}}", batchDetail.VideoURL);
                                        }
                                    }
                                }
                                else if (widget.WidgetId == HtmlConstants.ACCOUNT_INFORMATION_WIDGET_ID) //Account Information Widget
                                {
                                    StringBuilder AccDivData = new StringBuilder();
                                    AccDivData.Append("<div class='list-row-small ht70px'><div class='list-middle-row'> <div class='list-text'>Statement Date" + "</div><label class='list-value mb-0'>" + Convert.ToDateTime(customer.StatementDate).ToShortDateString() + "</label></div></div>");

                                    AccDivData.Append("<div class='list-row-small ht70px'><div class='list-middle-row'> <div class='list-text'>Statement Period" + "</div><label class='list-value mb-0'>" + customer.StatementPeriod + "</label></div></div>");

                                    AccDivData.Append("<div class='list-row-small ht70px'><div class='list-middle-row'> <div class='list-text'>Cusomer ID" + "</div><label class='list-value mb-0'>" + customer.CustomerCode + "</label></div></div>");

                                    AccDivData.Append("<div class='list-row-small ht70px'><div class='list-middle-row'> <div class='list-text'>RM Name" + "</div><label class='list-value mb-0'>" + customer.RmName + "</label></div></div>");

                                    AccDivData.Append("<div class='list-row-small ht70px'><div class='list-middle-row'> <div class='list-text'>RM Contact Number" + "</div><label class='list-value mb-0'>" + customer.RmContactNumber + "</label></div></div>");
                                    pageContent.Replace("{{AccountInfoData_" + page.Identifier + "_" + widget.Identifier + "}}", AccDivData.ToString());
                                }
                                else if (widget.WidgetId == HtmlConstants.IMAGE_WIDGET_ID) //Image Widget
                                {
                                    var imgAssetFilepath = string.Empty;
                                    if (widget.WidgetSetting != string.Empty && validationEngine.IsValidJson(widget.WidgetSetting))
                                    {
                                        dynamic widgetSetting = JObject.Parse(widget.WidgetSetting);
                                        if (widgetSetting.isPersonalize == false) //Is not dynamic image, then assign selected image from asset library
                                        {
                                            var asset = assetLibraryRepository.GetAssets(new AssetSearchParameter { Identifier = widgetSetting.AssetId, SortParameter = new SortParameter { SortColumn = "Id" } }, tenantCode).ToList()?.FirstOrDefault();
                                            if (asset != null)
                                            {
                                                var path = asset.FilePath.ToString();
                                                var imageFileName = asset.Name;
                                                if (File.Exists(path) && !File.Exists(outputLocation + "\\" + imageFileName))
                                                {
                                                    File.Copy(path, Path.Combine(outputLocation, imageFileName));
                                                }
                                                imgAssetFilepath = "./" + imageFileName;
                                            }
                                        }
                                        else //Is dynamic image, then assign it from database 
                                        {
                                            var custMedia = customerMedias.Where(item => item.PageId == page.Identifier && item.WidgetId == widget.Identifier)?.ToList()?.FirstOrDefault();
                                            if (custMedia != null && custMedia.ImageURL != string.Empty)
                                            {
                                                imgAssetFilepath = custMedia.ImageURL;
                                            }
                                            else
                                            {
                                                var batchDetail = batchDetails.Where(item => item.StatementId == statement.Identifier && item.WidgetId == widget.Identifier && item.PageId == page.Identifier)?.ToList()?.FirstOrDefault();
                                                if (batchDetail != null && batchDetail.ImageURL != string.Empty)
                                                {
                                                    imgAssetFilepath = batchDetail.ImageURL;
                                                }
                                            }
                                        }
                                        pageContent.Replace("{{ImageSource_" + statement.Identifier + "_" + page.Identifier + "_" + widget.Identifier + "}}", imgAssetFilepath);
                                    }
                                }
                                else if (widget.WidgetId == HtmlConstants.VIDEO_WIDGET_ID) //Video widget
                                {
                                    var vdoAssetFilepath = string.Empty;
                                    if (widget.WidgetSetting != string.Empty && validationEngine.IsValidJson(widget.WidgetSetting))
                                    {
                                        dynamic widgetSetting = JObject.Parse(widget.WidgetSetting);
                                        if (widgetSetting.isEmbedded == true)//If embedded then assigned it it from widget config json source url
                                        {
                                            vdoAssetFilepath = widgetSetting.SourceUrl;
                                        }
                                        else if (widgetSetting.isPersonalize == false && widgetSetting.isEmbedded == false) //If not dynamic video, then assign selected video from asset library
                                        {
                                            var asset = assetLibraryRepository.GetAssets(new AssetSearchParameter { Identifier = widgetSetting.AssetId, SortParameter = new SortParameter { SortColumn = "Id" } }, tenantCode).ToList()?.FirstOrDefault();
                                            if (asset != null)
                                            {
                                                var path = asset.FilePath.ToString();
                                                var videoFileName = asset.Name;
                                                if (File.Exists(path) && !File.Exists(outputLocation + "\\" + videoFileName))
                                                {
                                                    File.Copy(path, Path.Combine(outputLocation, videoFileName));
                                                }
                                                vdoAssetFilepath = "./" + videoFileName;
                                            }
                                        }
                                        else //If dynamic video, then assign it from database 
                                        {
                                            var custMedia = customerMedias.Where(item => item.PageId == page.Identifier && item.WidgetId == widget.Identifier)?.ToList()?.FirstOrDefault();
                                            if (custMedia != null && custMedia.VideoURL != string.Empty)
                                            {
                                                vdoAssetFilepath = custMedia.VideoURL;
                                            }
                                            else
                                            {
                                                var batchDetail = batchDetails.Where(item => item.StatementId == statement.Identifier && item.WidgetId == widget.Identifier && item.PageId == page.Identifier)?.ToList()?.FirstOrDefault();
                                                if (batchDetail != null && batchDetail.VideoURL != string.Empty)
                                                {
                                                    vdoAssetFilepath = batchDetail.VideoURL;
                                                }
                                            }
                                        }
                                        pageContent.Replace("{{VideoSource_" + statement.Identifier + "_" + page.Identifier + "_" + widget.Identifier + "}}", vdoAssetFilepath);
                                    }
                                }
                                else if (widget.WidgetId == HtmlConstants.SUMMARY_AT_GLANCE_WIDGET_ID) //Summary at Glance Widget
                                {
                                    if (accountrecords != null && accountrecords.Count > 0)
                                    {
                                        StringBuilder accSummary = new StringBuilder();
                                        var accRecords = accountrecords.GroupBy(item => item.AccountType).ToList();
                                        accRecords.ToList().ForEach(acc =>
                                        {
                                            accSummary.Append("<tr><td>" + acc.FirstOrDefault().AccountType + "</td><td>" + acc.FirstOrDefault().Currency + "</td><td>" + acc.Sum(it => it.Balance).ToString() + "</td></tr>");
                                        });
                                        pageContent.Replace("{{AccountSummary_" + page.Identifier + "_" + widget.Identifier + "}}", accSummary.ToString());
                                    }
                                }
                                else if (widget.WidgetId == HtmlConstants.CURRENT_AVAILABLE_BALANCE_WIDGET_ID)
                                {
                                    if (accountrecords != null && accountrecords.Count > 0)
                                    {
                                        var currentAccountRecords = accountrecords.Where(item => item.CustomerId == customer.Id && item.BatchId == batchMaster.Id && item.AccountType.ToLower().Contains("current") && item.Id == accountId)?.ToList();
                                        if (currentAccountRecords != null && currentAccountRecords.Count > 0)
                                        {
                                            var records = currentAccountRecords.GroupBy(item => item.AccountType).ToList();
                                            records?.ToList().ForEach(acc =>
                                            {
                                                var accountIndicatorClass = acc.FirstOrDefault().Indicator.ToLower().Equals("up") ? "fa fa-sort-asc text-success" : "fa fa-sort-desc text-danger";
                                                pageContent.Replace("{{AccountIndicatorClass}}", accountIndicatorClass);
                                                pageContent.Replace("{{TotalValue_" + page.Identifier + "_" + widget.Identifier + "}}", (currency + " " + acc.Sum(it => it.GrandTotal).ToString()));
                                                pageContent.Replace("{{TotalDeposit_" + page.Identifier + "_" + widget.Identifier + "}}", (currency + " " + acc.Sum(it => it.TotalDeposit).ToString()));
                                                pageContent.Replace("{{TotalSpend_" + page.Identifier + "_" + widget.Identifier + "}}", (currency + " " + acc.Sum(it => it.TotalSpend).ToString()));
                                                pageContent.Replace("{{Savings_" + page.Identifier + "_" + widget.Identifier + "}}", (currency + " " + acc.Sum(it => it.ProfitEarned).ToString()));
                                            });
                                        }
                                    }
                                }
                                else if (widget.WidgetId == HtmlConstants.SAVING_AVAILABLE_BALANCE_WIDGET_ID)
                                {
                                    if (accountrecords != null && accountrecords.Count > 0)
                                    {
                                        var savingAccountRecords = accountrecords.Where(item => item.CustomerId == customer.Id && item.BatchId == batchMaster.Id && item.AccountType.ToLower().Contains("saving") && item.Id == accountId)?.ToList();
                                        if (savingAccountRecords != null && savingAccountRecords.Count > 0)
                                        {
                                            var records = savingAccountRecords.GroupBy(item => item.AccountType).ToList();
                                            records?.ToList().ForEach(acc =>
                                            {
                                                var accountIndicatorClass = acc.FirstOrDefault().Indicator.ToLower().Equals("up") ? "fa fa-sort-asc text-success" : "fa fa-sort-desc text-danger";
                                                pageContent.Replace("{{AccountIndicatorClass}}", accountIndicatorClass);
                                                pageContent.Replace("{{TotalValue_" + page.Identifier + "_" + widget.Identifier + "}}", (currency + " " + acc.Sum(it => it.GrandTotal).ToString()));
                                                pageContent.Replace("{{TotalDeposit_" + page.Identifier + "_" + widget.Identifier + "}}", (currency + " " + acc.Sum(it => it.TotalDeposit).ToString()));
                                                pageContent.Replace("{{TotalSpend_" + page.Identifier + "_" + widget.Identifier + "}}", (currency + " " + acc.Sum(it => it.TotalSpend).ToString()));
                                                pageContent.Replace("{{Savings_" + page.Identifier + "_" + widget.Identifier + "}}", (currency + " " + acc.Sum(it => it.ProfitEarned).ToString()));
                                            });
                                        }
                                    }
                                }
                                else if (widget.WidgetId == HtmlConstants.SAVING_TRANSACTION_WIDGET_ID)
                                {
                                    IList<AccountTransactionRecord> accountTransactions = new List<AccountTransactionRecord>();
                                    using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                                    {
                                        accountTransactions = nISEntitiesDataContext.AccountTransactionRecords.Where(item => item.CustomerId == customer.Id && item.BatchId == batchMaster.Id && item.AccountType.ToLower().Contains("saving") && item.AccountId == accountId)?.OrderByDescending(item => item.TransactionDate)?.ToList();

                                        StringBuilder transaction = new StringBuilder();
                                        StringBuilder selectOption = new StringBuilder();
                                        if (accountTransactions != null && accountTransactions.Count > 0)
                                        {
                                            IList<AccountTransaction> transactions = new List<AccountTransaction>();
                                            pageContent.Replace("{{Currency}}", currency);
                                            accountTransactions.ToList().ForEach(trans =>
                                            {
                                                AccountTransaction accountTransaction = new AccountTransaction();
                                                accountTransaction.AccountType = trans.AccountType;
                                                accountTransaction.TransactionDate = Convert.ToDateTime(trans.TransactionDate).ToShortDateString();
                                                accountTransaction.TransactionType = trans.TransactionType;
                                                accountTransaction.FCY = trans.FCY;
                                                accountTransaction.Narration = trans.Narration;
                                                accountTransaction.LCY = trans.LCY;
                                                accountTransaction.CurrentRate = trans.CurrentRate;
                                                transactions.Add(accountTransaction);
                                            });
                                            string savingtransactionjson = JsonConvert.SerializeObject(transactions);
                                            if (savingtransactionjson != null && savingtransactionjson != string.Empty)
                                            {
                                                var distinctNaration = accountTransactions.Select(item => item.Narration).Distinct().ToList();
                                                distinctNaration.ToList().ForEach(item =>
                                                {
                                                    selectOption.Append("<option value='" + item + "'> " + item + "</option>");
                                                });

                                                SavingTransactionGridJson = "savingtransactiondata" + accountId + page.Identifier + "=" + savingtransactionjson;
                                                this.WriteToJsonFile(SavingTransactionGridJson, "savingtransactiondetail" + accountId + page.Identifier + ".json",  outputLocation);
                                                scriptHtmlRenderer.Append("<script type='text/javascript' src='./savingtransactiondetail" + accountId + page.Identifier + ".json'></script>");

                                                StringBuilder scriptval = new StringBuilder(HtmlConstants.SAVING_TRANSACTION_DETAIL_GRID_WIDGET_SCRIPT);
                                                scriptval.Replace("SavingTransactionTable", "SavingTransactionTable" + accountId + page.Identifier);
                                                scriptval.Replace("savingtransactiondata", "savingtransactiondata" + accountId + page.Identifier);
                                                scriptval.Replace("savingShowAll", "savingShowAll" + accountId + page.Identifier);
                                                scriptval.Replace("filterStatus", "filterStatus" + accountId + page.Identifier);
                                                scriptval.Replace("ResetGrid", "ResetGrid" + accountId + page.Identifier);
                                                scriptval.Replace("PrintGrid", "PrintGrid" + accountId + page.Identifier);
                                                scriptval.Replace("savingtransactionRadio", "savingtransactionRadio" + accountId + page.Identifier);
                                                scriptHtmlRenderer.Append(scriptval);

                                                pageContent.Replace("savingtransactiondata", "savingtransactiondata" + accountId + page.Identifier);
                                                pageContent.Replace("savingShowAll", "savingShowAll" + accountId + page.Identifier);
                                                pageContent.Replace("filterStatus", "filterStatus" + accountId + page.Identifier);
                                                pageContent.Replace("ResetGrid", "ResetGrid" + accountId + page.Identifier);
                                                pageContent.Replace("PrintGrid", "PrintGrid" + accountId + page.Identifier);
                                                pageContent.Replace("savingtransactionRadio", "savingtransactionRadio" + accountId + page.Identifier);
                                                pageContent.Replace("{{SelectOption_" + page.Identifier + "_" + widget.Identifier + "}}", selectOption.ToString());
                                                pageContent.Replace("SavingTransactionTable", "SavingTransactionTable" + accountId + page.Identifier);
                                            }
                                            else
                                            {
                                                transaction.Append("<tr><td colspan='7' class='text-danger text-center'><span>No data available</span></td></tr>");
                                            }
                                        }
                                        else
                                        {
                                            transaction.Append("<tr><td colspan='7' class='text-danger text-center'><span>No data available</span></td></tr>");
                                        }

                                        pageContent.Replace("{{AccountTransactionDetails_" + page.Identifier + "_" + widget.Identifier + "}}", transaction.ToString());
                                    }
                                }
                                else if (widget.WidgetId == HtmlConstants.CURRENT_TRANSACTION_WIDGET_ID)
                                {
                                    IList<AccountTransactionRecord> accountTransactions = new List<AccountTransactionRecord>();
                                    using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                                    {
                                        accountTransactions = nISEntitiesDataContext.AccountTransactionRecords.Where(item => item.CustomerId == customer.Id && item.BatchId == batchMaster.Id && item.AccountType.ToLower().Contains("current") && item.AccountId == accountId)?.OrderByDescending(item => item.TransactionDate)?.ToList();

                                        StringBuilder transaction = new StringBuilder();
                                        StringBuilder selectOption = new StringBuilder();
                                        if (accountTransactions != null && accountTransactions.Count > 0)
                                        {
                                            IList<AccountTransaction> transactions = new List<AccountTransaction>();
                                            pageContent.Replace("{{Currency}}", currency);
                                            accountTransactions.ToList().ForEach(trans =>
                                            {
                                                AccountTransaction accountTransaction = new AccountTransaction();
                                                accountTransaction.AccountType = trans.AccountType;
                                                accountTransaction.TransactionDate = Convert.ToDateTime(trans.TransactionDate).ToShortDateString();
                                                accountTransaction.TransactionType = trans.TransactionType;
                                                accountTransaction.FCY = trans.FCY;
                                                accountTransaction.Narration = trans.Narration;
                                                accountTransaction.LCY = trans.LCY;
                                                accountTransaction.CurrentRate = trans.CurrentRate;
                                                transactions.Add(accountTransaction);
                                            });
                                            string currenttransactionjson = JsonConvert.SerializeObject(transactions);
                                            if (currenttransactionjson != null && currenttransactionjson != string.Empty)
                                            {
                                                var distinctNaration = accountTransactions.Select(item => item.Narration).Distinct().ToList();
                                                distinctNaration.ToList().ForEach(item =>
                                                {
                                                    selectOption.Append("<option value='" + item + "'> " + item + "</option>");
                                                });

                                                CurrentTransactionGridJson = "currenttransactiondata" + accountId + page.Identifier + "=" + currenttransactionjson;
                                                this.WriteToJsonFile(CurrentTransactionGridJson, "currenttransactiondetail" + accountId + page.Identifier + ".json", outputLocation);
                                                scriptHtmlRenderer.Append("<script type='text/javascript' src='./currenttransactiondetail" + accountId + page.Identifier + ".json'></script>");

                                                StringBuilder scriptval = new StringBuilder(HtmlConstants.CURRENT_TRANSACTION_DETAIL_GRID_WIDGET_SCRIPT);
                                                scriptval.Replace("CurrentTransactionTable", "CurrentTransactionTable" + accountId + page.Identifier);
                                                scriptval.Replace("currenttransactiondata", "currenttransactiondata" + accountId + page.Identifier);
                                                scriptval.Replace("currentShowAll", "currentShowAll" + accountId + page.Identifier);
                                                scriptval.Replace("filterStatus", "filterStatus" + accountId + page.Identifier);
                                                scriptval.Replace("ResetGrid", "ResetGrid" + accountId + page.Identifier);
                                                scriptval.Replace("PrintGrid", "PrintGrid" + accountId + page.Identifier);
                                                scriptval.Replace("currenttransactionRadio", "currenttransactionRadio" + accountId + page.Identifier);
                                                scriptHtmlRenderer.Append(scriptval);

                                                pageContent.Replace("currentShowAll", "currentShowAll" + accountId + page.Identifier);
                                                pageContent.Replace("filterStatus", "filterStatus" + accountId + page.Identifier);
                                                pageContent.Replace("ResetGrid", "ResetGrid" + accountId + page.Identifier);
                                                pageContent.Replace("PrintGrid", "PrintGrid" + accountId + page.Identifier);
                                                pageContent.Replace("currenttransactionRadio", "currenttransactionRadio" + accountId + page.Identifier);
                                                pageContent.Replace("{{SelectOption_" + page.Identifier + "_" + widget.Identifier + "}}", selectOption.ToString());
                                                pageContent.Replace("CurrentTransactionTable", "CurrentTransactionTable" + accountId + page.Identifier);
                                            }
                                            else
                                            {
                                                transaction.Append("<tr><td colspan='7' class='text-danger text-center'><span>No data available</span></td></tr>");
                                            }
                                        }
                                        else
                                        {
                                            transaction.Append("<tr><td colspan='7' class='text-danger text-center'><span>No data available</span></td></tr>");
                                        }
                                        pageContent.Replace("{{AccountTransactionDetails_" + page.Identifier + "_" + widget.Identifier + "}}", transaction.ToString());
                                    }
                                }
                                else if (widget.WidgetId == HtmlConstants.TOP_4_INCOME_SOURCES_WIDGET_ID)
                                {
                                    IList<Top4IncomeSourcesRecord> top4IncomeSources = new List<Top4IncomeSourcesRecord>();
                                    using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                                    {
                                        top4IncomeSources = nISEntitiesDataContext.Top4IncomeSourcesRecord.Where(item => item.CustomerId == customer.Id && item.BatchId == batchMaster.Id)?.OrderByDescending(item => item.CurrentSpend)?.Take(4)?.ToList();
                                    }

                                    StringBuilder incomeSources = new StringBuilder();
                                    if (top4IncomeSources != null && top4IncomeSources.Count > 0)
                                    {
                                        top4IncomeSources.ToList().ForEach(src =>
                                        {
                                            var tdstring = string.Empty;
                                            if (src.CurrentSpend > src.AverageSpend)
                                            {
                                                tdstring = "<span class='fa fa-sort-desc fa-2x text-danger' aria-hidden='true'></span><span class='ml-2'>" +
                                                src.AverageSpend + "</span>";
                                            }
                                            else
                                            {
                                                tdstring = "<span class='fa fa-sort-asc fa-2x mt-1' aria-hidden='true' " +
                                                "style='position:relative;top:6px;color:limegreen'></span><span class='ml-2'>" + src.AverageSpend + "</span>";
                                            }
                                            incomeSources.Append("<tr><td class='float-left'>" + src.Source + "</td>" + "<td> " + src.CurrentSpend + "</td><td>" +
                                                tdstring + "</td></tr>");
                                        });
                                    }
                                    else
                                    {
                                        incomeSources.Append("<tr><td colspan='3' class='text-danger text-center'><div style='margin-top: 20px;'>No data available</div>" +
                                            "</td></tr>");
                                    }
                                    pageContent.Replace("{{IncomeSourceList_" + page.Identifier + "_" + widget.Identifier + "}}", incomeSources.ToString());
                                }
                                else if (widget.WidgetId == HtmlConstants.ANALYTICS_WIDGET_ID)
                                {
                                    if (accountrecords.Count > 0)
                                    {
                                        IList<AccountMasterRecord> accounts = new List<AccountMasterRecord>();
                                        var records = accountrecords.GroupBy(item => item.AccountType).ToList();
                                        records.ToList().ForEach(acc => accounts.Add(new AccountMasterRecord()
                                        {
                                            AccountType = acc.FirstOrDefault().AccountType,
                                            Percentage = acc.Average(item => item.Percentage == null ? 0 : item.Percentage)
                                        }));

                                        string accountsJson = JsonConvert.SerializeObject(accounts);
                                        if (accountsJson != null && accountsJson != string.Empty)
                                        {
                                            AnalyticsChartJson = "analyticsdata=" + accountsJson;
                                        }
                                        else
                                        {
                                            AnalyticsChartJson = "analyticsdata=[]";
                                        }
                                    }
                                    else
                                    {
                                        AnalyticsChartJson = "analyticsdata=[]";
                                    }

                                    this.WriteToJsonFile(AnalyticsChartJson, "analyticschartdata.json", outputLocation);
                                    scriptHtmlRenderer.Append("<script type='text/javascript' src='./analyticschartdata.json'></script>");
                                    pageContent.Replace("analyticschartcontainer", "analyticschartcontainer" + page.Identifier);
                                    scriptHtmlRenderer.Append(HtmlConstants.ANALYTICS_CHART_WIDGET_SCRIPT.Replace("analyticschartcontainer", "analyticschartcontainer" + page.Identifier));
                                }
                                else if (widget.WidgetId == HtmlConstants.SAVING_TREND_WIDGET_ID)
                                {
                                    IList<SavingTrendRecord> savingtrends = new List<SavingTrendRecord>();
                                    using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                                    {
                                        savingtrends = nISEntitiesDataContext.SavingTrendRecords.Where(item => item.CustomerId == customer.Id && item.BatchId == batchMaster.Id && item.AccountId == accountId).ToList();
                                    }
                                    if (savingtrends != null && savingtrends.Count > 0)
                                    {
                                        IList<SavingTrend> savingTrendRecords = new List<SavingTrend>();
                                        int mnth = DateTime.Now.Month - 1;  //To start month validation of consecutive month data from previous month
                                        for (int t = savingtrends.Count; t > 0; t--)
                                        {
                                            string month = this.utility.getMonth(mnth);
                                            var lst = savingtrends.Where(it => it.Month.ToLower().Contains(month.ToLower()))?.ToList();
                                            if (lst != null && lst.Count > 0)
                                            {
                                                SavingTrend trend = new SavingTrend();
                                                trend.Month = lst[0].Month;
                                                trend.NumericMonth = mnth;
                                                trend.Income = lst[0].Income ?? 0;
                                                trend.IncomePercentage = lst[0].IncomePercentage ?? 0;
                                                trend.SpendAmount = lst[0].SpendAmount;
                                                trend.SpendPercentage = lst[0].SpendPercentage ?? 0;
                                                savingTrendRecords.Add(trend);
                                            }
                                            mnth = mnth - 1 == 0 ? 12 : mnth - 1;
                                        }

                                        var records = savingTrendRecords.OrderByDescending(item => item.NumericMonth).Take(6).ToList();
                                        string savingtrendjson = JsonConvert.SerializeObject(records);
                                        if (savingtrendjson != null && savingtrendjson != string.Empty)
                                        {
                                            SavingTrendChartJson = "savingdata" + accountId + page.Identifier + "=" + savingtrendjson;
                                        }
                                        else
                                        {
                                            SavingTrendChartJson = "savingdata" + accountId + page.Identifier + "=[]";
                                        }
                                    }
                                    else
                                    {
                                        SavingTrendChartJson = "savingdata" + accountId + page.Identifier + "=[]";
                                    }

                                    this.WriteToJsonFile(SavingTrendChartJson, "savingtrenddata" + accountId + page.Identifier + ".json", outputLocation);
                                    scriptHtmlRenderer.Append("<script type='text/javascript' src='./savingtrenddata" + accountId + page.Identifier + ".json'></script>");

                                    pageContent.Replace("savingTrendscontainer", "savingTrendscontainer" + accountId + page.Identifier);
                                    var scriptval = HtmlConstants.SAVING_TREND_CHART_WIDGET_SCRIPT.Replace("savingTrendscontainer", "savingTrendscontainer" + accountId + page.Identifier).Replace("savingdata", "savingdata" + accountId + page.Identifier);
                                    scriptHtmlRenderer.Append(scriptval);
                                }
                                else if (widget.WidgetId == HtmlConstants.SPENDING_TREND_WIDGET_ID)
                                {
                                    IList<SavingTrendRecord> spendingtrends = new List<SavingTrendRecord>();
                                    using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                                    {
                                        spendingtrends = nISEntitiesDataContext.SavingTrendRecords.Where(item => item.CustomerId == customer.Id && item.BatchId == batchMaster.Id && item.AccountId == accountId).ToList();
                                    }
                                    if (spendingtrends != null && spendingtrends.Count > 0)
                                    {
                                        IList<SavingTrend> trends = new List<SavingTrend>();
                                        int mnth = DateTime.Now.Month - 1; //To start month validation of consecutive month data from previous month
                                        for (int t = spendingtrends.Count; t > 0; t--)
                                        {
                                            string month = this.utility.getMonth(mnth);
                                            var lst = spendingtrends.Where(it => it.Month.ToLower().Contains(month.ToLower()))?.ToList();
                                            if (lst != null && lst.Count > 0)
                                            {
                                                SavingTrend trend = new SavingTrend();
                                                trend.Month = lst[0].Month;
                                                trend.NumericMonth = mnth;
                                                trend.Income = lst[0].Income ?? 0;
                                                trend.IncomePercentage = lst[0].IncomePercentage ?? 0;
                                                trend.SpendAmount = lst[0].SpendAmount;
                                                trend.SpendPercentage = lst[0].SpendPercentage ?? 0;
                                                trends.Add(trend);
                                            }
                                            mnth = mnth - 1 == 0 ? 12 : mnth - 1;
                                        }

                                        var records = trends.OrderByDescending(item => item.NumericMonth).Take(6).ToList();
                                        string spendingtrendjson = JsonConvert.SerializeObject(records);
                                        if (spendingtrendjson != null && spendingtrendjson != string.Empty)
                                        {
                                            SpendingTrendChartJson = "spendingdata" + accountId + page.Identifier + "=" + spendingtrendjson;
                                        }
                                        else
                                        {
                                            SpendingTrendChartJson = "spendingdata" + accountId + page.Identifier + "=[]";
                                        }
                                    }
                                    else
                                    {
                                        SpendingTrendChartJson = "spendingdata" + accountId + page.Identifier + "=[]";
                                    }

                                    this.WriteToJsonFile(SpendingTrendChartJson, "spendingtrenddata" + accountId + page.Identifier + ".json", outputLocation);
                                    scriptHtmlRenderer.Append("<script type='text/javascript' src='./spendingtrenddata" + accountId + page.Identifier + ".json'></script>");

                                    pageContent.Replace("spendingTrendscontainer", "spendingTrendscontainer" + accountId + page.Identifier);
                                    var scriptval = HtmlConstants.SPENDING_TREND_CHART_WIDGET_SCRIPT.Replace("spendingTrendscontainer", "spendingTrendscontainer" + accountId + page.Identifier).Replace("spendingdata", "spendingdata" + accountId + page.Identifier);
                                    scriptHtmlRenderer.Append(scriptval);
                                }
                                else if (widget.WidgetId == HtmlConstants.REMINDER_AND_RECOMMENDATION_WIDGET_ID)
                                {
                                    IList<ReminderAndRecommendationRecord> reminderAndRecommendations = new List<ReminderAndRecommendationRecord>();
                                    using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                                    {
                                        reminderAndRecommendations = nISEntitiesDataContext.ReminderAndRecommendationRecords.Where(item => item.CustomerId == customer.Id && item.BatchId == batchMaster.Id)?.ToList();
                                    }

                                    StringBuilder reminderstr = new StringBuilder();
                                    if (reminderAndRecommendations != null && reminderAndRecommendations.Count > 0)
                                    {
                                        reminderstr.Append("<div class='row'><div class='col-lg-9'></div><div class='col-lg-3 text-left'><i class='fa fa-caret-left fa-3x float-left text-danger' aria-hidden='true'></i><span class='mt-2 d-inline-block ml-2'>Click</span></div> </div>");
                                        reminderAndRecommendations.ToList().ForEach(item =>
                                        {
                                            string targetlink = item.TargetURL != null && item.TargetURL != string.Empty ? item.TargetURL : "javascript:void(0)";
                                            reminderstr.Append("<div class='row'><div class='col-lg-9 text-left'><p class='p-1' style='background-color: #dce3dc;'>" +
                                                item.Description + " </p></div><div class='col-lg-3 text-left'><a href='" + targetlink +
                                                "'><i class='fa fa-caret-left fa-3x float-left text-danger'></i><span class='mt-2 d-inline-block ml-2'>" +
                                                item.LabelText + "</span></a></div></div>");
                                        });
                                    }
                                    else
                                    {
                                        reminderstr.Append("<div class='row text-danger text-center' style='margin-top: 20px;'>No data available</div>");
                                    }
                                    pageContent.Replace("{{ReminderAndRecommdationDataList_" + page.Identifier + "_" + widget.Identifier + "}}", reminderstr.ToString());
                                }
                            }

                            newPageContent.Append(pageContent);
                            if (page.PageTypeId == HtmlConstants.SAVING_ACCOUNT_PAGE_TYPE_ID || page.PageTypeId == HtmlConstants.CURRENT_ACCOUNT_PAGE_TYPE_ID)
                            {
                                newPageContent.Append(HtmlConstants.END_DIV_TAG);
                            }

                            if (x == accountCount - 1)
                            {
                                newPageContent.Append(HtmlConstants.END_DIV_TAG);
                            }
                        }

                        PageHeaderContent.Replace("{{SubTabs}}", SubTabs.ToString());
                        statementPageContent.PageHeaderContent = PageHeaderContent.ToString();
                        statementPageContent.HtmlContent = newPageContent.ToString();
                    }

                    newStatementPageContents.ToList().ForEach(page =>
                    {
                        htmlbody.Append(page.PageHeaderContent);
                        htmlbody.Append(page.HtmlContent);
                        htmlbody.Append(page.PageFooterContent);
                    });

                    htmlbody.Append(HtmlConstants.CONTAINER_DIV_HTML_FOOTER);

                    navbarHtml = navbarHtml.Replace("{{NavItemList}}", navbar.ToString());

                    StringBuilder finalHtml = new StringBuilder();
                    finalHtml.Append(HtmlConstants.HTML_HEADER);
                    finalHtml.Append(navbarHtml);
                    finalHtml.Append(htmlbody.ToString());
                    finalHtml.Append(HtmlConstants.HTML_FOOTER);
                    finalHtml.Replace("{{ChartScripts}}", scriptHtmlRenderer.ToString());
                    finalHtml.Replace("{{CustomerNumber}}", customer.Id.ToString());
                    finalHtml.Replace("{{StatementNumber}}", statement.Identifier.ToString());
                    finalHtml.Replace("{{FirstPageId}}", FirstPageId.ToString());
                    finalHtml.Replace("<link rel='stylesheet' href='../common/css/site.css'><link rel='stylesheet' href='../common/css/ltr.css'>", "");
                    finalHtml.Replace("../common/css/", "./");
                    finalHtml.Replace("../common/js/", "./");
                    finalHtml.Replace("../common/css/", "./");

                    string fileName = "Statement_" + customer.Id + "_" + statement.Identifier + "_" + DateTime.Now.ToString().Replace("-", "_").Replace(":", "_").Replace(" ", "_").Replace('/', '_') + ".html";
                    filePath = this.WriteToFile(finalHtml.ToString(), fileName, outputLocation);
                }

                return filePath;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method help to write json string in to actual file
        /// </summary>
        /// <param name="Message"> the message string </param>
        /// <param name="fileName"> the file name </param>
        /// <param name="path"> the file path </param>
        private void WriteToJsonFile(string Message, string fileName, string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filepath = path + "\\" + fileName;
            if (File.Exists(filepath))
            {
                File.Delete(filepath);
            }
            // Create a file to write to.
            using (StreamWriter sw = File.CreateText(filepath))
            {
                sw.WriteLine(Message);
            }
        }

        /// <summary>
        /// This method help to write html string to actual file
        /// </summary>
        /// <param name="Message"> the message string </param>
        /// <param name="fileName"> the file name </param>
        /// <param name="path"> the file path </param>
        private string WriteToFile(string Message, string fileName, string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filepath = path + "\\" + fileName;
            if (!File.Exists(filepath))
            {
                // Create a file to write to.
                using (StreamWriter sw = File.CreateText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
            return filepath;
        }

        #endregion
    }
}
