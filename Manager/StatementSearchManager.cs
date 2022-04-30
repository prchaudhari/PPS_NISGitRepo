// <copyright file="StatementSearchManager.cs" company="Websym Solutions Pvt. Ltd.">
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
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Text.RegularExpressions;
    using Unity;
    #endregion

    public class StatementSearchManager
    {
        #region Private members
        /// <summary>
        /// The unity container
        /// </summary>
        private IUnityContainer unityContainer = null;

        /// <summary>
        /// The validation engine object
        /// </summary>
        IValidationEngine validationEngine = null;

        /// <summary>
        /// The utility object
        /// </summary>
        private IUtility utility = null;

        /// <summary>
        /// The StatementSearch repository.
        /// </summary>
        IStatementSearchRepository StatementSearchRepository = null;

        /// <summary>
        /// The page repository.
        /// </summary>
        IPageRepository pageRepository = null;

        /// <summary>
        /// The Tenant configuration manager.
        /// </summary>
        private TenantConfigurationManager tenantConfigurationManager = null;

        /// <summary>
        /// The Schedule manager.
        /// </summary>
        private ScheduleManager scheduleManager = null;

        /// <summary>
        /// The Tenant transaction manager.
        /// </summary>
        private TenantTransactionDataManager tenantTransactionDataManager = null;

        /// <summary>
        /// The Statement manager.
        /// </summary>
        private StatementManager statementManager = null;

        /// <summary>
        /// The Dynamic widget manager.
        /// </summary>
        private DynamicWidgetManager dynamicWidgetManager = null;

        /// <summary>
        /// The Client manager.
        /// </summary>
        private ClientManager clientManager = null;

        /// <summary>
        /// The Asset library manager.
        /// </summary>
        private AssetLibraryManager assetLibraryManager = null;

        #endregion

        #region Constructor

        /// <summary>
        /// This is constructor for role manager, which initialise
        /// role repository.
        /// </summary>
        /// <param name="container">IUnity container implementation object.</param>
        public StatementSearchManager(IUnityContainer unityContainer)
        {
            try
            {
                this.unityContainer = unityContainer;
                this.validationEngine = new ValidationEngine();
                this.utility = new Utility();
                this.StatementSearchRepository = this.unityContainer.Resolve<IStatementSearchRepository>();
                this.pageRepository = this.unityContainer.Resolve<IPageRepository>();
                this.tenantConfigurationManager = this.unityContainer.Resolve<TenantConfigurationManager>();
                this.scheduleManager = this.unityContainer.Resolve<ScheduleManager>();
                this.tenantTransactionDataManager = this.unityContainer.Resolve<TenantTransactionDataManager>();
                this.clientManager = this.unityContainer.Resolve<ClientManager>();
                this.statementManager = this.unityContainer.Resolve<StatementManager>();
                this.dynamicWidgetManager = this.unityContainer.Resolve<DynamicWidgetManager>();
                this.assetLibraryManager = this.unityContainer.Resolve<AssetLibraryManager>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// This method will call add StatementSearchs method of repository.
        /// </summary>
        /// <param name="StatementSearchs">StatementSearchs are to be add.</param>
        /// <param name="tenantCode">Tenant code of StatementSearch.</param>
        /// <returns>
        /// Returns true if entities added successfully, false otherwise.
        /// </returns>
        public bool AddStatementSearchs(IList<StatementSearch> StatementSearchs, string tenantCode)
        {
            bool result = false;
            try
            {
                result = this.StatementSearchRepository.AddStatementSearchs(StatementSearchs, tenantCode);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method will call get StatementSearchs method of repository.
        /// </summary>
        /// <param name="StatementSearchSearchParameter">The StatementSearch search parameters.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns>
        /// Returns StatementSearchs if found for given parameters, else return null
        /// </returns>
        public IList<StatementSearch> GetStatementSearchs(StatementSearchSearchParameter StatementSearchSearchParameter, string tenantCode)
        {
            try
            {
                InvalidSearchParameterException invalidSearchParameterException = new InvalidSearchParameterException(tenantCode);
                try
                {
                    StatementSearchSearchParameter.IsValid();
                }
                catch (Exception exception)
                {
                    invalidSearchParameterException.Data.Add("InvalidPagingParameter", exception.Data);
                }

                if (invalidSearchParameterException.Data.Count > 0)
                {
                    throw invalidSearchParameterException;
                }
               
                return this.StatementSearchRepository.GetStatementSearchs(StatementSearchSearchParameter, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// This method helps to get count of StatementSearchs.
        /// </summary>
        /// <param name="StatementSearchSearchParameter">The StatementSearch search parameters.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns>
        /// Returns count of StatementSearchs
        /// </returns>
        public int GetStatementSearchCount(StatementSearchSearchParameter StatementSearchSearchParameter, string tenantCode)
        {
            int roleCount = 0;
            try
            {
                roleCount = this.StatementSearchRepository.GetStatementSearchCount(StatementSearchSearchParameter, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return roleCount;
        }

        /// <summary>
        /// This method reference to generate html statement for export to pdf
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="tenantCode"></param>
        /// <returns>output location of html statmeent file</returns>
        public string GenerateStatement(long identifier, string tenantCode)
        {
            try
            {
                ClientSearchParameter clientSearchParameter = new ClientSearchParameter
                {
                    TenantCode = tenantCode,
                    IsCountryRequired = false,
                    IsContactRequired = false,
                    PagingParameter = new PagingParameter
                    {
                        PageIndex = 0,
                        PageSize = 0,
                    },
                    SortParameter = new SortParameter()
                    {
                        SortOrder = SortOrder.Ascending,
                        SortColumn = "Id",
                    },
                    SearchMode = SearchMode.Equals
                };
                var client = this.clientManager.GetClients(clientSearchParameter, tenantCode).FirstOrDefault();
                return this.StatementSearchRepository.GenerateStatement(identifier, tenantCode, client);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method reference to generate html statement for export to pdf
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="tenantCode"></param>
        /// <returns>output location of html statmeent file</returns>
        public string GenerateStatementNew(StatementSearch statementSearch, string tenantCode)
        {
            string outputlocation = string.Empty;

            try
            {
                var client = this.clientManager.GetClients(new ClientSearchParameter
                {
                    TenantCode = tenantCode,
                    IsCountryRequired = false,
                    IsContactRequired = false,
                    PagingParameter = new PagingParameter
                    {
                        PageIndex = 0,
                        PageSize = 0,
                    },
                    SortParameter = new SortParameter()
                    {
                        SortOrder = SortOrder.Ascending,
                        SortColumn = "Id",
                    },
                    SearchMode = SearchMode.Equals
                }, tenantCode).FirstOrDefault();

                var tenantConfiguration = this.tenantConfigurationManager.GetTenantConfigurations(tenantCode)?.FirstOrDefault();

                var schedule = this.scheduleManager.GetSchedules(new ScheduleSearchParameter()
                {
                    Identifier = statementSearch.ScheduleId.ToString(),
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
                }, tenantCode)?.FirstOrDefault();

                var functionName = string.Empty;
                if (tenantConfiguration != null && !string.IsNullOrEmpty(tenantConfiguration.GenerateStatementRunNowScheduleFunctionName))
                {
                    functionName = tenantConfiguration.GenerateStatementRunNowScheduleFunctionName;
                }

                switch (functionName)
                {
                    case ModelConstant.GENERATE_FINANCIAL_CUSTOEMR_STATEMENT_BY_SCHEDULE_RUN_NOW:
                        outputlocation = this.CreateFinancialStatementHtml(statementSearch, schedule, tenantConfiguration, client, outputlocation, tenantCode);
                        break;

                    case ModelConstant.GENERATE_NEDBANK_CUSTOEMR_STATEMENT_BY_SCHEDULE_RUN_NOW:
                        outputlocation = this.CreateNedbankStatementHtml(statementSearch, schedule, tenantConfiguration, client, outputlocation, tenantCode);
                        break;

                    default:
                        outputlocation = this.CreateFinancialStatementHtml(statementSearch, schedule, tenantConfiguration, client, outputlocation, tenantCode);
                        break;
                }

                return outputlocation;
            }
            catch (Exception ex)
            {
                if (Directory.Exists(outputlocation))
                {
                    Directory.Delete(outputlocation, true);
                }
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
        /// <param name="outputLocation"> the output file path </param>
        /// <param name="client"> the client object </param>
        /// <param name="tenantConfiguration"> the tenant configuration object </param>
        /// <param name="tenantCode"> the tenant code </param>
        public string GenerateHtmlStatementForPdfGeneration(CustomerMaster customer, Statement statement, IList<StatementPageContent> statementPageContents, BatchMaster batchMaster, IList<BatchDetail> BatchDetails, string tenantCode, string outputLocation, Client client, TenantConfiguration tenantConfiguration)
        {
            string filePath = string.Empty;
            bool IsSavingOrCurrentAccountPagePresent = false;

            try
            {
                if (statementPageContents.Count > 0)
                {
                    string currency = string.Empty;
                    var accountrecords = new List<AccountMaster>();
                    var savingaccountrecords = new List<AccountMaster>();
                    var curerntaccountrecords = new List<AccountMaster>();
                    var CustomerAcccountTransactions = new List<AccountTransaction>();
                    var CustomerSavingTrends = new List<SavingTrend>();

                    var tenantEntities = this.dynamicWidgetManager.GetTenantEntities(tenantCode);

                    var pages = statement.Pages.Where(item => item.PageTypeName == HtmlConstants.SAVING_ACCOUNT_PAGE || item.PageTypeName == HtmlConstants.CURRENT_ACCOUNT_PAGE).ToList();
                    IsSavingOrCurrentAccountPagePresent = pages.Count > 0 ? true : false;

                    //collecting all required transaction required for static widgets in financial tenant html statement
                    if (IsSavingOrCurrentAccountPagePresent)
                    {
                        var customerAccountSearchParameter = new CustomerAccountSearchParameter()
                        {
                            CustomerId = customer.Identifier,
                            BatchId = batchMaster.Identifier
                        };
                        accountrecords = this.tenantTransactionDataManager.Get_AccountMaster(customerAccountSearchParameter, tenantCode)?.ToList();

                        //get customer account transaction details
                        CustomerAcccountTransactions = this.tenantTransactionDataManager.Get_AccountTransaction(customerAccountSearchParameter, tenantCode)?.OrderBy(item => item.TransactionDate)?.ToList();

                        //get customer saving and spending trend details data
                        CustomerSavingTrends = this.tenantTransactionDataManager.Get_SavingTrend(customerAccountSearchParameter, tenantCode)?.ToList();
                    }

                    //collecting all media information which is required in html statement for some widgets like image, video and static customer information widgets
                    var customerMedias = this.tenantTransactionDataManager.GetCustomerMediaList(customer.Identifier, batchMaster.Identifier, statement.Identifier, tenantCode);

                    var htmlbody = new StringBuilder();
                    currency = accountrecords.Count > 0 ? accountrecords[0].Currency : string.Empty;
                    string navbarHtml = HtmlConstants.NAVBAR_HTML_FOR_PREVIEW.Replace("{{logo}}", "./nisLogo.png");
                    navbarHtml = navbarHtml.Replace("{{Today}}", DateTime.UtcNow.ToString(ModelConstant.DATE_FORMAT_dd_MMM_yyyy));
                    var clientlogo = client.TenantLogo != null ? client.TenantLogo : "";
                    navbarHtml = navbarHtml + "<input type='hidden' id='TenantLogoImageValue' value='" + clientlogo + "'>";
                    htmlbody.Append(HtmlConstants.CONTAINER_DIV_HTML_HEADER);

                    //start to render actual html content data
                    StringBuilder scriptHtmlRenderer = new StringBuilder();
                    StringBuilder navbar = new StringBuilder();
                    int subPageCount = 0;
                    string accountNumber = string.Empty;
                    string accountType = string.Empty;
                    long accountId = 0;
                    string SavingTrendChartJson = string.Empty;
                    string SpendingTrendChartJson = string.Empty;
                    string AnalyticsChartJson = string.Empty;
                    string SavingTransactionGridJson = string.Empty;
                    string CurrentTransactionGridJson = string.Empty;
                    HttpClient httpClient = null;

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
                        TabClassName = it.TabClassName,
                        DynamicWidgets = it.DynamicWidgets
                    }));

                    long FirstPageId = statement.Pages[0].Identifier;
                    for (int i = 0; i < statement.Pages.Count; i++)
                    {
                        var page = statement.Pages[i];
                        var statementPageContent = newStatementPageContents.Where(item => item.PageTypeId == page.PageTypeId && item.Id == i).FirstOrDefault();

                        subPageCount = 1;
                        if (IsSavingOrCurrentAccountPagePresent)
                        {
                            if (page.PageTypeName == HtmlConstants.SAVING_ACCOUNT_PAGE)
                            {
                                savingaccountrecords = accountrecords.Where(item => item.CustomerId == customer.Identifier && item.BatchId == batchMaster.Identifier && item.AccountType.ToLower().Contains("saving"))?.ToList();
                                subPageCount = savingaccountrecords.Count;
                            }
                            else if (page.PageTypeName == HtmlConstants.CURRENT_ACCOUNT_PAGE)
                            {
                                curerntaccountrecords = accountrecords.Where(item => item.CustomerId == customer.Identifier && item.BatchId == batchMaster.Identifier && item.AccountType.ToLower().Contains("current"))?.ToList();
                                subPageCount = curerntaccountrecords.Count;
                            }
                        }

                        StringBuilder SubTabs = new StringBuilder();
                        StringBuilder PageHeaderContent = new StringBuilder(statementPageContent.PageHeaderContent);
                        var dynamicWidgets = new List<DynamicWidget>(statementPageContent.DynamicWidgets);

                        string tabClassName = Regex.Replace((statementPageContent.DisplayName + "-" + page.Identifier), @"\s+", "-");
                        navbar.Append(" <li class='nav-item'><a class='nav-link pt-1 " + (i == 0 ? "active" : "") + " " + tabClassName + "' href='#" + tabClassName + "' >" + statementPageContent.DisplayName + "</a> </li> ");
                        PageHeaderContent.Replace("{{ExtraClass}}", tabClassName).Replace("{{DivId}}", tabClassName);

                        StringBuilder newPageContent = new StringBuilder();
                        newPageContent.Append(HtmlConstants.PAGE_TAB_CONTENT_HEADER);

                        for (int x = 0; x < subPageCount; x++)
                        {
                            accountNumber = string.Empty;
                            accountType = string.Empty;
                            if (IsSavingOrCurrentAccountPagePresent)
                            {
                                if (page.PageTypeName == HtmlConstants.SAVING_ACCOUNT_PAGE)
                                {
                                    accountNumber = savingaccountrecords[x].AccountNumber;
                                    accountId = savingaccountrecords[x].Identifier;
                                    accountType = savingaccountrecords[x].AccountType;
                                }
                                if (page.PageTypeName == HtmlConstants.CURRENT_ACCOUNT_PAGE)
                                {
                                    accountNumber = curerntaccountrecords[x].AccountNumber;
                                    accountId = curerntaccountrecords[x].Identifier;
                                    accountType = curerntaccountrecords[x].AccountType;
                                }

                                if (page.PageTypeName == HtmlConstants.SAVING_ACCOUNT_PAGE || page.PageTypeName == HtmlConstants.CURRENT_ACCOUNT_PAGE)
                                {
                                    string lastFourDigisOfAccountNumber = accountNumber.Length > 4 ? accountNumber.Substring(Math.Max(0, accountNumber.Length - 4)) : accountNumber;
                                    if (x == 0)
                                    {
                                        SubTabs.Append("<ul class='navbar-nav nav'>");
                                    }

                                    SubTabs.Append("<li class='nav-item " + (x == 0 ? "active" : "") + "'><a id='tab" + x + "-tab' " + "href='#" + (page.PageTypeName == HtmlConstants.SAVING_ACCOUNT_PAGE ? "Saving" : "Current") + "-" + lastFourDigisOfAccountNumber + "' class='nav-link " + (x == 0 ? "active" : "") + "'> Account - " + lastFourDigisOfAccountNumber + "</a></li>");

                                    newPageContent.Append("<div id='" + (page.PageTypeName == HtmlConstants.SAVING_ACCOUNT_PAGE ? "Saving" : "Current") +
                                        "-" + lastFourDigisOfAccountNumber + "'");

                                    if (page.PageTypeName == HtmlConstants.SAVING_ACCOUNT_PAGE)
                                    {
                                        newPageContent.Append("<input type='hidden' id='SavingAccountId' name='SavingAccountId' value='" + accountId + "'>");
                                    }
                                    else
                                    {
                                        newPageContent.Append("<input type='hidden' id='CurrentAccountId' name='CurrentAccountId' value='" + accountId + "'>");
                                    }

                                    if (x == subPageCount - 1)
                                    {
                                        SubTabs.Append("</ul>");
                                    }
                                }
                            }

                            var pagewidgets = new List<PageWidget>(page.PageWidgets);
                            StringBuilder pageContent = new StringBuilder(statementPageContent.HtmlContent);
                            for (int j = 0; j < pagewidgets.Count; j++)
                            {
                                var widget = pagewidgets[j];
                                if (!widget.IsDynamicWidget)
                                {
                                    switch (widget.WidgetName)
                                    {
                                        case HtmlConstants.CUSTOMER_INFORMATION_WIDGET_NAME:
                                            this.BindCustomerInformationWidgetData(pageContent, customer, statement, page, widget, customerMedias, BatchDetails);
                                            break;
                                        case HtmlConstants.ACCOUNT_INFORMATION_WIDGET_NAME:
                                            this.BindAccountInformationWidgetData(pageContent, customer, page, widget);
                                            break;
                                        case HtmlConstants.IMAGE_WIDGET_NAME:
                                            this.BindImageWidgetData(pageContent, customer.Identifier, customerMedias, BatchDetails, statement, page, batchMaster, widget, tenantCode, outputLocation);
                                            break;
                                        case HtmlConstants.VIDEO_WIDGET_NAME:
                                            this.BindVideoWidgetData(pageContent, customer.Identifier, customerMedias, BatchDetails, statement, page, batchMaster, widget, tenantCode, outputLocation);
                                            break;
                                        case HtmlConstants.SUMMARY_AT_GLANCE_WIDGET_NAME:
                                            this.BindSummaryAtGlanceWidgetData(pageContent, accountrecords, page, widget);
                                            break;
                                        case HtmlConstants.CURRENT_AVAILABLE_BALANCE_WIDGET_NAME:
                                            this.BindCurrentAvailBalanceWidgetData(pageContent, customer, batchMaster, accountId, accountrecords, page, widget, currency);
                                            break;
                                        case HtmlConstants.SAVING_AVAILABLE_BALANCE_WIDGET_NAME:
                                            this.BindSavingAvailBalanceWidgetData(pageContent, customer, batchMaster, accountId, accountrecords, page, widget, currency);
                                            break;
                                        case HtmlConstants.SAVING_TRANSACTION_WIDGET_NAME:
                                            this.BindSavingTransactionWidgetData(pageContent, scriptHtmlRenderer, customer, batchMaster, CustomerAcccountTransactions, page, widget, accountId, tenantCode, currency, outputLocation);
                                            break;
                                        case HtmlConstants.CURRENT_TRANSACTION_WIDGET_NAME:
                                            this.BindCurrentTransactionWidgetData(pageContent, scriptHtmlRenderer, customer, batchMaster, CustomerAcccountTransactions, page, widget, accountId, tenantCode, currency, outputLocation);
                                            break;
                                        case HtmlConstants.TOP_4_INCOME_SOURCE_WIDGET_NAME:
                                            this.BindTop4IncomeSourcesWidgetData(pageContent, customer, batchMaster, page, widget, tenantCode);
                                            break;
                                        case HtmlConstants.ANALYTICS_WIDGET_NAME:
                                            this.BindAnalyticsChartWidgetData(pageContent, scriptHtmlRenderer, accountrecords, page, outputLocation);
                                            break;
                                        case HtmlConstants.SAVING_TREND_WIDGET_NAME:
                                            this.BindSavingTrendChartWidgetData(pageContent, scriptHtmlRenderer, customer, batchMaster, CustomerSavingTrends, accountId, page, outputLocation);
                                            break;
                                        case HtmlConstants.SPENDING_TREND_WIDGET_NAME:
                                            this.BindSpendingTrendChartWidgetData(pageContent, scriptHtmlRenderer, customer, batchMaster, CustomerSavingTrends, accountId, page, outputLocation);
                                            break;
                                        case HtmlConstants.REMINDER_AND_RECOMMENDATION_WIDGET_NAME:
                                            this.BindReminderAndRecommendationWidgetData(pageContent, customer, batchMaster, page, widget, tenantCode);
                                            break;
                                    }
                                }
                                else
                                {
                                    var dynaWidgets = dynamicWidgets.Where(item => item.Identifier == widget.WidgetId).ToList();
                                    if (dynaWidgets.Count > 0)
                                    {
                                        var dynawidget = dynaWidgets.FirstOrDefault();
                                        var themeDetails = new CustomeTheme();
                                        if (dynawidget.ThemeType == "Default")
                                        {
                                            themeDetails = JsonConvert.DeserializeObject<CustomeTheme>(tenantConfiguration.WidgetThemeSetting);
                                        }
                                        else
                                        {
                                            themeDetails = JsonConvert.DeserializeObject<CustomeTheme>(dynawidget.ThemeCSS);
                                        }

                                        //Get data from database for widget
                                        httpClient = new HttpClient();
                                        httpClient.BaseAddress = new Uri(tenantConfiguration.BaseUrlForTransactionData);
                                        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(ModelConstant.APPLICATION_JSON_MEDIA_TYPE));
                                        httpClient.DefaultRequestHeaders.Add(ModelConstant.TENANT_CODE_KEY, tenantCode);

                                        //API search parameter
                                        JObject searchParameter = new JObject();
                                        searchParameter[ModelConstant.BATCH_ID] = batchMaster.Identifier;
                                        searchParameter[ModelConstant.CUSTOEMR_ID] = customer.Identifier;
                                        searchParameter[ModelConstant.WIDGET_FILTER_SETTING] = dynawidget.WidgetFilterSettings;

                                        switch (dynawidget.WidgetType)
                                        {
                                            case HtmlConstants.TABLE_DYNAMICWIDGET:
                                                this.BindDynamicTableWidgetData(pageContent, page, widget, searchParameter, dynawidget, httpClient);
                                                break;
                                            case HtmlConstants.FORM_DYNAMICWIDGET:
                                                this.BindDynamicFormWidgetData(pageContent, page, widget, searchParameter, dynawidget, httpClient);
                                                break;
                                            case HtmlConstants.LINEGRAPH_DYNAMICWIDGET:
                                                this.BindDynamicLineGraphWidgetData(pageContent, scriptHtmlRenderer, page, widget, searchParameter, dynawidget, httpClient, themeDetails);
                                                break;
                                            case HtmlConstants.BARGRAPH_DYNAMICWIDGET:
                                                this.BindDynamicBarGraphWidgetData(pageContent, scriptHtmlRenderer, page, widget, searchParameter, dynawidget, httpClient, themeDetails);
                                                break;
                                            case HtmlConstants.PICHART_DYNAMICWIDGET:
                                                this.BindDynamicPieChartWidgetData(pageContent, scriptHtmlRenderer, page, widget, searchParameter, dynawidget, httpClient, themeDetails, tenantCode);
                                                break;
                                            case HtmlConstants.HTML_DYNAMICWIDGET:
                                                this.BindDynamicHtmlWidgetData(pageContent, page, widget, searchParameter, dynawidget, httpClient);
                                                break;
                                        }
                                    }
                                }
                            }

                            newPageContent.Append(pageContent);
                            if (page.PageTypeName == HtmlConstants.SAVING_ACCOUNT_PAGE || page.PageTypeName == HtmlConstants.CURRENT_ACCOUNT_PAGE)
                            {
                                newPageContent.Append(HtmlConstants.END_DIV_TAG);
                            }

                            if (x == subPageCount - 1)
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
                    scriptHtmlRenderer.Append(HtmlConstants.TENANT_LOGO_SCRIPT);

                    finalHtml.Replace("{{ChartScripts}}", scriptHtmlRenderer.ToString());
                    finalHtml.Replace("{{CustomerNumber}}", customer.Identifier.ToString());
                    finalHtml.Replace("{{StatementNumber}}", statement.Identifier.ToString());
                    finalHtml.Replace("{{FirstPageId}}", FirstPageId.ToString());
                    finalHtml.Replace("{{TenantCode}}", tenantCode);
                    finalHtml.Replace("{{TenantName}}", client != null ? client.TenantName : ModelConstant.CHILD_TENANT);
                    finalHtml.Replace("<link rel='stylesheet' href='../common/css/site.css'><link rel='stylesheet' href='../common/css/ltr.css'>", "");
                    finalHtml.Replace("../common/css/", "./").Replace("../common/js/", "./");
                    finalHtml.Replace("../common/css/", "./");

                    string fileName = "Statement_" + customer.Identifier + "_" + statement.Identifier + "_" + DateTime.Now.ToString().Replace("-", "_").Replace(":", "_").Replace(" ", "_").Replace('/', '_') + ".html";
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
        /// This method help to generate statement for customer
        /// </summary>
        /// <param name="customer"> the customer object </param>
        /// <param name="statement"> the statement object </param>
        /// <param name="statementPageContents"> the statement page html content list</param>
        /// <param name="batchMaster"> the batch master object </param>
        /// <param name="outputLocation"> the output file path </param>
        /// <param name="client"> the client object </param>
        /// <param name="tenantConfiguration"> the tenant configuration object </param>
        /// <param name="tenantCode"> the tenant code </param>
        public string GenerateNedbankHtmlStatementForPdfGeneration(DM_CustomerMaster customer, Statement statement, IList<StatementPageContent> statementPageContents, BatchMaster batchMaster, IList<BatchDetail> BatchDetails, string tenantCode, string OutputLocation, TenantConfiguration tenantConfiguration)
        {
            string filePath = string.Empty;

            try
            {
                if (statementPageContents.Count > 0)
                {
                    var tenantEntities = this.dynamicWidgetManager.GetTenantEntities(tenantCode);

                    //collecting all media information which is required in html statement for some widgets like image, video and static customer information widgets
                    var customerMedias = this.tenantTransactionDataManager.GetCustomerMediaList(customer.CustomerId, batchMaster.Identifier, statement.Identifier, tenantCode);

                    long BranchId = 0;
                    var investmentMasters = new List<DM_InvestmentMaster>();
                    var PersonalLoanAccounts = new List<DM_PersonalLoanMaster>();
                    var HomeLoanAccounts = new List<DM_HomeLoanMaster>();

                    var AccountsSummaries = new List<DM_AccountsSummary>();
                    var _lstAccountAnalysis = new List<DM_AccountAnanlysis>();
                    var Reminders = new List<DM_CustomerReminderAndRecommendation>();
                    var NewsAndAlerts = new List<DM_CustomerNewsAndAlert>();
                    var GreenbackMaster = new DM_GreenbacksMaster();
                    var GreenbacksRewardPoints = new List<DM_GreenbacksRewardPoints>();
                    var GreenbacksRedeemedPoints = new List<DM_GreenbacksRewardPointsRedeemed>();
                    var CustProductMonthwiseRewardPoints = new List<DM_CustomerProductWiseRewardPoints>();
                    var CustRewardSpendByCategory = new List<DM_CustomerRewardSpendByCategory>();

                    var customerSearchParameter = new CustomerSearchParameter() { CustomerId = customer.CustomerId, BatchId = batchMaster.Identifier };

                    var IsPortFolioPageTypePresent = statement.Pages.Where(it => it.PageTypeName == HtmlConstants.AT_A_GLANCE_PAGE_TYPE).ToList().Count > 0;
                    var IsInvestmentPageTypePresent = statement.Pages.Where(it => it.PageTypeName == HtmlConstants.INVESTMENT_PAGE_TYPE).ToList().Count > 0;
                    var IsPersonalLoanPageTypePresent = statement.Pages.Where(it => it.PageTypeName == HtmlConstants.PERSONAL_LOAN_PAGE_TYPE).ToList().Count > 0;
                    var IsHomeLoanPageTypePresent = statement.Pages.Where(it => it.PageTypeName == HtmlConstants.HOME_LOAN_PAGE_TYPE).ToList().Count > 0;
                    var IsRewardPageTypePresent = statement.Pages.Where(it => it.PageTypeName == HtmlConstants.GREENBACKS_PAGE_TYPE).ToList().Count > 0;

                    if (IsPortFolioPageTypePresent)
                    {
                        AccountsSummaries = this.tenantTransactionDataManager.GET_DM_AccountSummaries(customerSearchParameter, tenantCode)?.ToList();
                        _lstAccountAnalysis = this.tenantTransactionDataManager.GET_DM_AccountAnalysisDetails(customerSearchParameter, tenantCode)?.ToList();
                        Reminders = this.tenantTransactionDataManager.GET_DM_CustomerReminderAndRecommendations(customerSearchParameter, tenantCode)?.ToList();
                        NewsAndAlerts = this.tenantTransactionDataManager.GET_DM_CustomerNewsAndAlert(customerSearchParameter, tenantCode)?.ToList();
                    }

                    if (IsInvestmentPageTypePresent)
                    {
                        investmentMasters = this.tenantTransactionDataManager.Get_DM_InvestmasterMaster(new CustomerInvestmentSearchParameter() { CustomerId = customer.CustomerId, BatchId = batchMaster.Identifier }, tenantCode)?.ToList();
                        if (investmentMasters != null && investmentMasters.Count > 0)
                        {
                            investmentMasters.ForEach(invest =>
                            {
                                invest.investmentTransactions = this.tenantTransactionDataManager.Get_DM_InvestmentTransaction(new CustomerInvestmentSearchParameter() { CustomerId = customer.CustomerId, BatchId = batchMaster.Identifier, InvestmentId = invest.InvestmentId }, tenantCode)?.ToList();
                            });
                            BranchId = (investmentMasters != null && investmentMasters.Count > 0) ? investmentMasters[0].BranchId : 0;
                        }
                    }
                    if (IsPersonalLoanPageTypePresent)
                    {
                        PersonalLoanAccounts = this.tenantTransactionDataManager.Get_DM_PersonalLoanMaster(new CustomerPersonalLoanSearchParameter() { BatchId = batchMaster.Identifier, CustomerId = customer.CustomerId }, tenantCode)?.ToList();
                        BranchId = (PersonalLoanAccounts != null && PersonalLoanAccounts.Count > 0) ? PersonalLoanAccounts[0].BranchId : 0;
                    }
                    if (IsHomeLoanPageTypePresent)
                    {
                        HomeLoanAccounts = this.tenantTransactionDataManager.Get_DM_HomeLoanMaster(new CustomerHomeLoanSearchParameter() { BatchId = batchMaster.Identifier, CustomerId = customer.CustomerId }, tenantCode)?.ToList();
                    }
                    if (IsRewardPageTypePresent)
                    {
                        GreenbackMaster = this.tenantTransactionDataManager.GET_DM_GreenbacksMasterDetails(tenantCode)?.ToList()?.FirstOrDefault();
                        GreenbacksRewardPoints = this.tenantTransactionDataManager.GET_DM_GreenbacksRewardPoints(customerSearchParameter, tenantCode)?.ToList();
                        GreenbacksRedeemedPoints = this.tenantTransactionDataManager.GET_DM_GreenbacksRewardPointsRedeemed(customerSearchParameter, tenantCode)?.ToList();
                        CustProductMonthwiseRewardPoints = this.tenantTransactionDataManager.GET_DM_CustomerProductWiseRewardPoints(customerSearchParameter, tenantCode)?.ToList();
                        CustRewardSpendByCategory = this.tenantTransactionDataManager.GET_DM_CustomerRewardSpendByCategory(customerSearchParameter, tenantCode)?.ToList();
                    }

                    var SpecialMessage = this.tenantTransactionDataManager.Get_DM_SpecialMessages(new MessageAndNoteSearchParameter() { BatchId = batchMaster.Identifier, CustomerId = customer.CustomerId }, tenantCode)?.ToList()?.FirstOrDefault();

                    var _lstMessage = this.tenantTransactionDataManager.Get_DM_MarketingMessages(new MessageAndNoteSearchParameter() { BatchId = batchMaster.Identifier }, tenantCode)?.ToList();

                    var htmlbody = new StringBuilder();
                    htmlbody.Append(HtmlConstants.CONTAINER_DIV_HTML_HEADER);
                    htmlbody.Append(HtmlConstants.NEDBANK_STATEMENT_HEADER.Replace("{{eConfirmLogo}}", "./eConfirm.png").Replace("{{NedBankLogo}}", "./NEDBANKLogo.png").Replace("{{StatementDate}}", DateTime.Now.ToString(ModelConstant.DATE_FORMAT_yyyy_MM_dd)));

                    //start to render actual html content data
                    StringBuilder scriptHtmlRenderer = new StringBuilder();
                    HttpClient httpClient = null;
                    var NavItemList = new StringBuilder();

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
                        TabClassName = it.TabClassName,
                        DynamicWidgets = it.DynamicWidgets
                    }));

                    long FirstPageId = statement.Pages[0].Identifier;
                    for (int i = 0; i < statement.Pages.Count; i++)
                    {
                        var page = statement.Pages[i];
                        var statementPageContent = newStatementPageContents.Where(item => item.PageTypeId == page.PageTypeId && item.Id == i).FirstOrDefault();

                        var MarketingMessageCounter = 0;
                        var Messages = _lstMessage?.Where(it => it.Type == page.PageTypeName)?.ToList();

                        StringBuilder SubTabs = new StringBuilder();
                        StringBuilder PageHeaderContent = new StringBuilder(statementPageContent.PageHeaderContent);
                        var dynamicWidgets = new List<DynamicWidget>(statementPageContent.DynamicWidgets);

                        string tabClassName = Regex.Replace((statementPageContent.DisplayName + "-" + page.Identifier), @"\s+", "-");
                        NavItemList.Append("<li class='nav-item" + (i != statement.Pages.Count - 1 ? " nav-rt-border" : string.Empty) + "'><a id='tab" + i + "-tab' href='#" + tabClassName + "' class='nav-link" + (i == 0 ? " active" : string.Empty) + "'> " + statementPageContent.DisplayName + " </a></li>");

                        string ExtraClassName = string.Empty; // statement.Pages.Count > 1 ? (i == 0 ? " tab-pane fade in active show " : " tab-pane fade ") : string.Empty;
                        PageHeaderContent.Replace("{{ExtraClass}}", ExtraClassName).Replace("{{DivId}}", tabClassName);

                        StringBuilder newPageContent = new StringBuilder();
                        var pagewidgets = new List<PageWidget>(page.PageWidgets);
                        StringBuilder pageContent = new StringBuilder(statementPageContent.HtmlContent);
                        for (int j = 0; j < pagewidgets.Count; j++)
                        {
                            var widget = pagewidgets[j];
                            if (!widget.IsDynamicWidget)
                            {
                                switch (widget.WidgetName)
                                {
                                    case HtmlConstants.CUSTOMER_DETAILS_WIDGET_NAME:
                                        if (statement.Pages.Count == 1)
                                        {
                                                this.BindCustomerDetailsWidgetData(pageContent, customer, page, widget);
                                        }
                                        break;

                                    case HtmlConstants.BRANCH_DETAILS_WIDGET_NAME:
                                        if (page.PageTypeName == HtmlConstants.HOME_LOAN_PAGE_TYPE)
                                        {
                                            if (statement.Pages.Count == 1)
                                            {
                                                this.BindBondDetailsWidgetData(pageContent, page, widget, HomeLoanAccounts);
                                            }
                                        }
                                        else
                                        {
                                            this.BindBranchDetailsWidgetData(pageContent, BranchId, page, widget, tenantCode);
                                        }
                                        break;

                                    case HtmlConstants.IMAGE_WIDGET_NAME:
                                        this.BindImageWidgetData(pageContent, customer.CustomerId, customerMedias, BatchDetails, statement, page, batchMaster, widget, tenantCode, OutputLocation);
                                        break;

                                    case HtmlConstants.VIDEO_WIDGET_NAME:
                                        this.BindVideoWidgetData(pageContent, customer.CustomerId, customerMedias, BatchDetails, statement, page, batchMaster, widget, tenantCode, OutputLocation);
                                        break;

                                    case HtmlConstants.INVESTMENT_PORTFOLIO_STATEMENT_WIDGET_NAME:
                                        this.BindInvestmentPortfolioStatementWidgetData(pageContent, customer, investmentMasters, page, widget);
                                        break;

                                    case HtmlConstants.INVESTOR_PERFORMANCE_WIDGET_NAME:
                                        this.BindInvestorPerformanceWidgetData(pageContent, investmentMasters, page, widget);
                                        break;

                                    case HtmlConstants.BREAKDOWN_OF_INVESTMENT_ACCOUNTS_WIDGET_NAME:
                                        this.BindBreakdownOfInvestmentAccountsWidgetData(pageContent, investmentMasters, page, widget);
                                        break;

                                    case HtmlConstants.EXPLANATORY_NOTES_WIDGET_NAME:
                                        this.BindExplanatoryNotesWidgetData(pageContent, batchMaster, page, widget, tenantCode);
                                        break;

                                    case HtmlConstants.SERVICE_WIDGET_NAME:
                                        this.BindMarketingServiceWidgetData(pageContent, Messages, page, widget, MarketingMessageCounter);
                                        MarketingMessageCounter++;
                                        break;

                                    case HtmlConstants.WEALTH_SERVICE_WIDGET_NAME:
                                        this.BindMarketingServiceWidgetData(pageContent, Messages, page, widget, MarketingMessageCounter);
                                        MarketingMessageCounter++;
                                        break;

                                    case HtmlConstants.PERSONAL_LOAN_DETAIL_WIDGET_NAME:
                                        this.BindPersonalLoanDetailWidgetData(pageContent, batchMaster, customer, page, widget, tenantCode);
                                        break;

                                    case HtmlConstants.PERSONAL_LOAN_TRANASCTION_WIDGET_NAME:
                                        this.BindPersonalLoanTransactionWidgetData(pageContent, batchMaster, page, widget, customer, tenantCode);
                                        break;

                                    case HtmlConstants.PERSONAL_LOAN_PAYMENT_DUE_WIDGET_NAME:
                                        this.BindPersonalLoanPaymentDueWidgetData(pageContent, batchMaster, page, widget, customer, tenantCode);
                                        break;

                                    case HtmlConstants.SPECIAL_MESSAGE_WIDGET_NAME:
                                        this.BindSpecialMessageWidgetData(pageContent, SpecialMessage, page, widget);
                                        break;

                                    case HtmlConstants.PERSONAL_LOAN_INSURANCE_MESSAGE_WIDGET_NAME:
                                        this.BindPersonalLoanInsuranceMessageWidgetData(pageContent, SpecialMessage, page, widget);
                                        break;

                                    case HtmlConstants.PERSONAL_LOAN_TOTAL_AMOUNT_DETAIL_WIDGET_NAME:
                                        this.BindPersonalLoanTotalAmountDetailWidgetData(pageContent, PersonalLoanAccounts, page, widget);
                                        break;

                                    case HtmlConstants.PERSONAL_LOAN_ACCOUNTS_BREAKDOWN_WIDGET_NAME:
                                        this.BindPersonalLoanAccountsBreakdownWidgetData(pageContent, PersonalLoanAccounts, page, widget);
                                        break;

                                    case HtmlConstants.HOME_LOAN_TOTAL_AMOUNT_DETAIL_WIDGET_NAME:
                                        this.BindHomeLoanTotalAmountDetailWidgetData(pageContent, HomeLoanAccounts, page, widget);
                                        break;

                                    case HtmlConstants.HOME_LOAN_ACCOUNTS_BREAKDOWN_WIDGET_NAME:
                                        this.BindHomeLoanAccountsBreakdownWidgetData(pageContent, HomeLoanAccounts, page, widget);
                                        break;

                                    case HtmlConstants.NEDBANK_PORTFOLIO_CUSTOMER_DETAILS_WIDGET_NAME:
                                        this.BindPortfolioCustomerDetailsWidgetData(pageContent, customer, page, widget);
                                        break;

                                    case HtmlConstants.NEDBANK_PORTFOLIO_CUSTOMER_ADDRESS_WIDGET_NAME:
                                        this.BindPortfolioCustomerAddressDetailsWidgetData(pageContent, customer, page, widget);
                                        break;

                                    case HtmlConstants.NEDBANK_PORTFOLIO_CLIENT_CONTACT_DETAILS_WIDGET_NAME:
                                        this.BindPortfolioClientContactDetailsWidgetData(pageContent, page, widget);
                                        break;

                                    case HtmlConstants.NEDBANK_PORTFOLIO_ACCOUNT_SUMMARY_WIDGET_NAME:
                                        this.BindPortfolioAccountSummaryWidgetData(pageContent, AccountsSummaries, page, widget);
                                        break;

                                    case HtmlConstants.NEDBANK_PORTFOLIO_ACCOUNT_ANALYSIS_WIDGET_NAME:
                                        this.BindPortfolioAccountAnalysisWidgetData(pageContent, scriptHtmlRenderer, _lstAccountAnalysis, page, widget);
                                        break;

                                    case HtmlConstants.NEDBANK_PORTFOLIO_REMINDERS_WIDGET_NAME:
                                        this.BindPortfolioRemindersWidgetData(pageContent, Reminders, page, widget);
                                        break;

                                    case HtmlConstants.NEDBANK_PORTFOLIO_NEWS_ALERT_WIDGET_NAME:
                                        this.BindPortfolioNewsAlertsWidgetData(pageContent, NewsAndAlerts, page, widget);
                                        break;

                                    case HtmlConstants.NEDBANK_GREENBACKS_TOTAL_REWARDS_POINTS_WIDGET_NAME:
                                        this.BindGreenbacksTotalRewardPointsWidgetData(pageContent, AccountsSummaries, page, widget);
                                        break;

                                    case HtmlConstants.NEDBANK_YTD_REWARDS_POINTS_BAR_GRAPH_WIDGET_NAME:
                                        this.BindGreenbacksYtdRewardsPointsGraphWidgetData(pageContent, scriptHtmlRenderer, GreenbacksRewardPoints, page, widget);
                                        break;

                                    case HtmlConstants.NEDBANK_POINTS_REDEEMED_YTD_BAR_GRAPH_WIDGET_NAME:
                                        this.BindGreenbacksPointsRedeemedYtdGraphWidgetData(pageContent, scriptHtmlRenderer, GreenbacksRedeemedPoints, page, widget);
                                        break;

                                    case HtmlConstants.NEDBANK_PRODUCT_RELATED_POINTS_EARNED_BAR_GRAPH_WIDGET_NAME:
                                        this.BindGreenbacksProductRelatedPonitsEarnedGraphWidgetData(pageContent, scriptHtmlRenderer, CustProductMonthwiseRewardPoints, page, widget);
                                        break;

                                    case HtmlConstants.NEDBANK_CATEGORY_SPEND_REWARDS_PIE_CHART_WIDGET_NAME:
                                        this.BindGreenbacksCategorySpendRewardPointsGraphWidgetData(pageContent, scriptHtmlRenderer, CustRewardSpendByCategory, page, widget);
                                        break;
                                }
                            }
                            else
                            {
                                var dynaWidgets = dynamicWidgets.Where(item => item.Identifier == widget.WidgetId).ToList();
                                if (dynaWidgets.Count > 0)
                                {
                                    var dynawidget = dynaWidgets.FirstOrDefault();
                                    var themeDetails = new CustomeTheme();
                                    if (dynawidget.ThemeType == "Default")
                                    {
                                        themeDetails = JsonConvert.DeserializeObject<CustomeTheme>(tenantConfiguration.WidgetThemeSetting);
                                    }
                                    else
                                    {
                                        themeDetails = JsonConvert.DeserializeObject<CustomeTheme>(dynawidget.ThemeCSS);
                                    }

                                    //Get data from database for widget
                                    httpClient = new HttpClient();
                                    httpClient.BaseAddress = new Uri(tenantConfiguration.BaseUrlForTransactionData);
                                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(ModelConstant.APPLICATION_JSON_MEDIA_TYPE));
                                    httpClient.DefaultRequestHeaders.Add(ModelConstant.TENANT_CODE_KEY, tenantCode);

                                    //API search parameter
                                    JObject searchParameter = new JObject();
                                    searchParameter[ModelConstant.BATCH_ID] = batchMaster.Identifier;
                                    searchParameter[ModelConstant.CUSTOEMR_ID] = customer.CustomerId;
                                    searchParameter[ModelConstant.WIDGET_FILTER_SETTING] = dynawidget.WidgetFilterSettings;

                                    switch (dynawidget.WidgetType)
                                    {
                                        case HtmlConstants.TABLE_DYNAMICWIDGET:
                                            this.BindDynamicTableWidgetData(pageContent, page, widget, searchParameter, dynawidget, httpClient);
                                            break;
                                        case HtmlConstants.FORM_DYNAMICWIDGET:
                                            this.BindDynamicFormWidgetData(pageContent, page, widget, searchParameter, dynawidget, httpClient);
                                            break;
                                        case HtmlConstants.LINEGRAPH_DYNAMICWIDGET:
                                            this.BindDynamicLineGraphWidgetData(pageContent, scriptHtmlRenderer, page, widget, searchParameter, dynawidget, httpClient, themeDetails);
                                            break;
                                        case HtmlConstants.BARGRAPH_DYNAMICWIDGET:
                                            this.BindDynamicBarGraphWidgetData(pageContent, scriptHtmlRenderer, page, widget, searchParameter, dynawidget, httpClient, themeDetails);
                                            break;
                                        case HtmlConstants.PICHART_DYNAMICWIDGET:
                                            this.BindDynamicPieChartWidgetData(pageContent, scriptHtmlRenderer, page, widget, searchParameter, dynawidget, httpClient, themeDetails, tenantCode);
                                            break;
                                        case HtmlConstants.HTML_DYNAMICWIDGET:
                                            this.BindDynamicHtmlWidgetData(pageContent, page, widget, searchParameter, dynawidget, httpClient);
                                            break;
                                    }
                                }
                            }
                        }

                        newPageContent.Append(pageContent);
                        statementPageContent.PageHeaderContent = PageHeaderContent.ToString();
                        statementPageContent.HtmlContent = newPageContent.ToString();
                    }

                    //NAV bar will append to html statement, only if statement definition have more than 1 pages 
                    if (statement.Pages.Count > 1)
                    {
                        htmlbody.Append(HtmlConstants.NEDBANK_NAV_BAR_HTML.Replace("{{Today}}", DateTime.Now.ToString(ModelConstant.DATE_FORMAT_dd_MMM_yyyy)).Replace("{{NavItemList}}", NavItemList.ToString()));
                    }

                    htmlbody.Append(HtmlConstants.PAGE_TAB_CONTENT_HEADER);
                    newStatementPageContents.ToList().ForEach(page =>
                    {
                        htmlbody.Append(page.PageHeaderContent);
                        htmlbody.Append(page.HtmlContent);
                        htmlbody.Append(page.PageFooterContent);
                        htmlbody.Append(HtmlConstants.PAGE_FOOTER_HTML);
                    });

                    htmlbody.Append(HtmlConstants.END_DIV_TAG); // end tab-content div

                    var footerContent = new StringBuilder(HtmlConstants.NEDBANK_STATEMENT_FOOTER);
                    footerContent.Replace("{{NedbankSloganImage}}", "./See_money_differently.PNG");
                    footerContent.Replace("{{NedbankNameImage}}", "./NEDBANK_Name.png");
                    footerContent.Replace("{{FooterText}}", HtmlConstants.NEDBANK_STATEMENT_FOOTER_TEXT_STRING);
                    footerContent.Replace("{{LastFooterText}}", string.Empty);
                    htmlbody.Append(footerContent.ToString());

                    htmlbody.Append(HtmlConstants.CONTAINER_DIV_HTML_FOOTER);

                    StringBuilder finalHtml = new StringBuilder();
                    finalHtml.Append(HtmlConstants.HTML_HEADER);
                    finalHtml.Append(htmlbody.ToString());
                    finalHtml.Append(HtmlConstants.HTML_FOOTER);

                    finalHtml.Replace("{{ChartScripts}}", scriptHtmlRenderer.ToString());
                    finalHtml.Replace("{{CustomerNumber}}", customer.CustomerId.ToString());
                    finalHtml.Replace("{{StatementNumber}}", statement.Identifier.ToString());
                    finalHtml.Replace("{{FirstPageId}}", FirstPageId.ToString());
                    finalHtml.Replace("{{TenantCode}}", tenantCode);
                    finalHtml.Replace("../common/images/", "./").Replace("../common/css/", "./").Replace("../common/js/", "./");

                    string fileName = "Statement_" + customer.CustomerId + ".html";
                    filePath = this.WriteToFile(finalHtml.ToString(), fileName, OutputLocation);
                }

                return filePath;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// This method help to create HTML statement for financial tenant customer
        /// </summary>
        /// <param name="statementSearch"> the statement search object </param>
        /// <param name="schedule"> the schedule object</param>
        /// <param name="client"> the client object </param>
        /// <param name="tenantConfiguration"> the tenant configuration object </param>
        /// /// <param name="outputlocation"> the output location for HTML statement file </param>
        /// <param name="tenantCode"> the tenant code </param>
        private string CreateFinancialStatementHtml(StatementSearch statementSearch, Schedule schedule, TenantConfiguration tenantConfiguration, Client client, string outputlocation, string tenantCode)
        {
            try
            {
                var customer = this.tenantTransactionDataManager.Get_CustomerMasters(new CustomerSearchParameter()
                {
                    CustomerId = statementSearch.CustomerId,
                    BatchId = statementSearch.BatchId
                }, tenantCode)?.FirstOrDefault();

                var batch = this.scheduleManager.GetBatches(new BatchSearchParameter()
                {
                    Identifier = statementSearch.BatchId.ToString(),
                }, tenantCode)?.FirstOrDefault();

                if (schedule != null && customer != null && batch != null)
                {
                    var BatchDetails = this.tenantTransactionDataManager.GetBatchDetails(statementSearch.BatchId, schedule.Statement.Identifier, tenantCode);
                    var statements = this.statementManager.GetStatements(new StatementSearchParameter
                    {
                        Identifier = schedule.Statement.Identifier,
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
                    }, tenantCode);
                    if (statements == null || statements.Count == 0)
                    {
                        throw new StatementNotFoundException(tenantCode);
                    }

                    var statement = statements.FirstOrDefault();
                    var statementPageContents = this.statementManager.GenerateHtmlFormatOfStatement(statement, tenantCode, tenantConfiguration);
                    outputlocation = AppDomain.CurrentDomain.BaseDirectory + "\\Resources\\temp_" + DateTime.Now.ToString().Replace("-", "_").Replace(":", "_").Replace(" ", "_").Replace('/', '_');
                    if (!Directory.Exists(outputlocation))
                    {
                        Directory.CreateDirectory(outputlocation);
                    }
                    this.utility.DirectoryCopy(AppDomain.CurrentDomain.BaseDirectory + "\\Resources\\css", outputlocation, false);
                    this.utility.DirectoryCopy(AppDomain.CurrentDomain.BaseDirectory + "\\Resources\\js", outputlocation, false);
                    this.utility.DirectoryCopy(AppDomain.CurrentDomain.BaseDirectory + "\\Resources\\images", outputlocation, false);
                    this.utility.DirectoryCopy(AppDomain.CurrentDomain.BaseDirectory + "\\Resources\\fonts", outputlocation, false);
                    this.GenerateHtmlStatementForPdfGeneration(customer, statement, statementPageContents, batch, BatchDetails, tenantCode, outputlocation, client, tenantConfiguration);
                }

                return outputlocation;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method help to create HTML statement for financial tenant customer
        /// </summary>
        /// <param name="statementSearch"> the statement search object </param>
        /// <param name="schedule"> the schedule object</param>
        /// <param name="client"> the client object </param>
        /// <param name="tenantConfiguration"> the tenant configuration object </param>
        /// /// <param name="outputlocation"> the output location for HTML statement file </param>
        /// <param name="tenantCode"> the tenant code </param>
        private string CreateNedbankStatementHtml(StatementSearch statementSearch, Schedule schedule, TenantConfiguration tenantConfiguration, Client client, string outputlocation, string tenantCode)
        {
            try
            {
                var customer = this.tenantTransactionDataManager.Get_DM_CustomerMasters(new CustomerSearchParameter()
                {
                    CustomerId = statementSearch.CustomerId,
                    BatchId = statementSearch.BatchId
                }, tenantCode)?.FirstOrDefault();

                var batch = this.scheduleManager.GetBatches(new BatchSearchParameter()
                {
                    Identifier = statementSearch.BatchId.ToString(),
                }, tenantCode)?.FirstOrDefault();

                if (schedule != null && customer != null && batch != null)
                {
                    var BatchDetails = this.tenantTransactionDataManager.GetBatchDetails(statementSearch.BatchId, schedule.Statement.Identifier, tenantCode);
                    var statements = this.statementManager.GetStatements(new StatementSearchParameter
                    {
                        Identifier = schedule.Statement.Identifier,
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
                    }, tenantCode);
                    if (statements == null || statements.Count == 0)
                    {
                        throw new StatementNotFoundException(tenantCode);
                    }

                    var statement = statements.FirstOrDefault();
                    var statementPageContents = this.statementManager.GenerateHtmlFormatOfNedbankStatement(statement, tenantCode, tenantConfiguration);
                    outputlocation = AppDomain.CurrentDomain.BaseDirectory + "\\Resources\\temp_" + DateTime.Now.ToString().Replace("-", "_").Replace(":", "_").Replace(" ", "_").Replace('/', '_');
                    if (!Directory.Exists(outputlocation))
                    {
                        Directory.CreateDirectory(outputlocation);
                    }

                    this.utility.DirectoryCopy(AppDomain.CurrentDomain.BaseDirectory + "\\Resources\\css", outputlocation, false);
                    this.utility.DirectoryCopy(AppDomain.CurrentDomain.BaseDirectory + "\\Resources\\js", outputlocation, false);
                    this.utility.DirectoryCopy(AppDomain.CurrentDomain.BaseDirectory + "\\Resources\\images", outputlocation, false);
                    this.utility.DirectoryCopy(AppDomain.CurrentDomain.BaseDirectory + "\\Resources\\fonts", outputlocation, false);

                    //Update mark pro fonts url in ltr.css
                    var cssFIlePath = string.Empty;
                    var css = string.Empty;
                    if (File.Exists(outputlocation + @"\ltr.css"))
                    {
                        cssFIlePath = outputlocation + @"\ltr.css";
                        css = File.ReadAllText(cssFIlePath);
                        css = css.Replace("../fonts/", "./");
                        File.WriteAllText(cssFIlePath, css);
                    }

                    //Update mark pro fonts url in site.css
                    if (File.Exists(outputlocation + @"\site.css"))
                    {
                        cssFIlePath = outputlocation + @"\site.css";
                        css = File.ReadAllText(cssFIlePath);
                        css = css.Replace("../fonts/", "./");
                        File.WriteAllText(cssFIlePath, css);
                    }

                    //Update font awesome fonts url in fone-awesome.min.css
                    if (File.Exists(outputlocation + @"\font-awesome.min.css"))
                    {
                        cssFIlePath = outputlocation + @"\font-awesome.min.css";
                        css = File.ReadAllText(cssFIlePath);
                        css = css.Replace("../fonts/", "./");
                        File.WriteAllText(cssFIlePath, css);
                    }
                    
                    this.GenerateNedbankHtmlStatementForPdfGeneration(customer, statement, statementPageContents, batch, BatchDetails, tenantCode, outputlocation, tenantConfiguration);
                }

                return outputlocation;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        #region Bind Data to widgets in HTML Statement methods

        private void BindCustomerInformationWidgetData(StringBuilder pageContent, CustomerMaster customer, Statement statement, Page page, PageWidget widget, IList<CustomerMedia> customerMedias, IList<BatchDetail> batchDetails)
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

        private void BindAccountInformationWidgetData(StringBuilder pageContent, CustomerMaster customer, Page page, PageWidget widget)
        {
            var AccDivData = new StringBuilder();
            AccDivData.Append("<div class='list-row-small ht70px'><div class='list-middle-row'> <div class='list-text'>Statement Date" + "</div><label class='list-value mb-0'>" + Convert.ToDateTime(customer.StatementDate).ToShortDateString() + "</label></div></div>");

            AccDivData.Append("<div class='list-row-small ht70px'><div class='list-middle-row'> <div class='list-text'>Statement Period" + "</div><label class='list-value mb-0'>" + customer.StatementPeriod + "</label></div></div>");

            AccDivData.Append("<div class='list-row-small ht70px'><div class='list-middle-row'> <div class='list-text'>Cusomer ID" + "</div><label class='list-value mb-0'>" + customer.CustomerCode + "</label></div></div>");

            AccDivData.Append("<div class='list-row-small ht70px'><div class='list-middle-row'> <div class='list-text'>RM Name" + "</div><label class='list-value mb-0'>" + customer.RmName + "</label></div></div>");

            AccDivData.Append("<div class='list-row-small ht70px'><div class='list-middle-row'> <div class='list-text'>RM Contact Number" + "</div><label class='list-value mb-0'>" + customer.RmContactNumber + "</label></div></div>");
            pageContent.Replace("{{AccountInfoData_" + page.Identifier + "_" + widget.Identifier + "}}", AccDivData.ToString());
        }

        private void BindSummaryAtGlanceWidgetData(StringBuilder pageContent, IList<AccountMaster> accountrecords, Page page, PageWidget widget)
        {
            if (accountrecords != null && accountrecords.Count > 0)
            {
                var accSummary = new StringBuilder();
                var accRecords = accountrecords.GroupBy(item => item.AccountType).ToList();
                accRecords.ToList().ForEach(acc =>
                {
                    accSummary.Append("<tr><td>" + acc.FirstOrDefault().AccountType + "</td><td>" + acc.FirstOrDefault().Currency + "</td><td>" + acc.Sum(it => Convert.ToDecimal(it.Balance)).ToString() + "</td></tr>");
                });
                pageContent.Replace("{{AccountSummary_" + page.Identifier + "_" + widget.Identifier + "}}", accSummary.ToString());
            }
        }

        private void BindCurrentAvailBalanceWidgetData(StringBuilder pageContent, CustomerMaster customer, BatchMaster batchMaster, long accountId, IList<AccountMaster> accountrecords, Page page, PageWidget widget, string currency)
        {
            if (accountrecords != null && accountrecords.Count > 0)
            {
                var currentAccountRecords = accountrecords.Where(item => item.CustomerId == customer.Identifier && item.BatchId == batchMaster.Identifier && item.AccountType.ToLower().Contains("current") && item.Identifier == accountId)?.ToList();
                if (currentAccountRecords != null && currentAccountRecords.Count > 0)
                {
                    var records = currentAccountRecords.GroupBy(item => item.AccountType).ToList();
                    records?.ToList().ForEach(acc =>
                    {
                        var accountIndicatorClass = acc.FirstOrDefault().Indicator.ToLower().Equals("up") ? "fa fa-sort-asc text-success" : "fa fa-sort-desc text-danger";
                        pageContent.Replace("{{AccountIndicatorClass}}", accountIndicatorClass);
                        pageContent.Replace("{{TotalValue_" + page.Identifier + "_" + widget.Identifier + "}}", (currency + " " + acc.Sum(it => Convert.ToDecimal(it.GrandTotal)).ToString()));
                        pageContent.Replace("{{TotalDeposit_" + page.Identifier + "_" + widget.Identifier + "}}", (currency + " " + acc.Sum(it => Convert.ToDecimal(it.TotalDeposit)).ToString()));
                        pageContent.Replace("{{TotalSpend_" + page.Identifier + "_" + widget.Identifier + "}}", (currency + " " + acc.Sum(it => Convert.ToDecimal(it.TotalSpend)).ToString()));
                        pageContent.Replace("{{Savings_" + page.Identifier + "_" + widget.Identifier + "}}", (currency + " " + acc.Sum(it => Convert.ToDecimal(it.ProfitEarned)).ToString()));
                    });
                }
            }
        }

        private void BindSavingAvailBalanceWidgetData(StringBuilder pageContent, CustomerMaster customer, BatchMaster batchMaster, long accountId, IList<AccountMaster> accountrecords, Page page, PageWidget widget, string currency)
        {
            if (accountrecords != null && accountrecords.Count > 0)
            {
                var savingAccountRecords = accountrecords.Where(item => item.CustomerId == customer.Identifier && item.BatchId == batchMaster.Identifier && item.AccountType.ToLower().Contains("saving") && item.Identifier == accountId)?.ToList();
                if (savingAccountRecords != null && savingAccountRecords.Count > 0)
                {
                    var records = savingAccountRecords.GroupBy(item => item.AccountType).ToList();
                    records?.ToList().ForEach(acc =>
                    {
                        var accountIndicatorClass = acc.FirstOrDefault().Indicator.ToLower().Equals("up") ? "fa fa-sort-asc text-success" : "fa fa-sort-desc text-danger";
                        pageContent.Replace("{{AccountIndicatorClass}}", accountIndicatorClass);
                        pageContent.Replace("{{TotalValue_" + page.Identifier + "_" + widget.Identifier + "}}", (currency + " " + acc.Sum(it => Convert.ToDecimal(it.GrandTotal)).ToString()));
                        pageContent.Replace("{{TotalDeposit_" + page.Identifier + "_" + widget.Identifier + "}}", (currency + " " + acc.Sum(it => Convert.ToDecimal(it.TotalDeposit)).ToString()));
                        pageContent.Replace("{{TotalSpend_" + page.Identifier + "_" + widget.Identifier + "}}", (currency + " " + acc.Sum(it => Convert.ToDecimal(it.TotalSpend)).ToString()));
                        pageContent.Replace("{{Savings_" + page.Identifier + "_" + widget.Identifier + "}}", (currency + " " + acc.Sum(it => Convert.ToDecimal(it.ProfitEarned)).ToString()));
                    });
                }
            }
        }

        private void BindSavingTransactionWidgetData(StringBuilder pageContent, StringBuilder scriptHtmlRenderer, CustomerMaster customer, BatchMaster batchMaster, IList<AccountTransaction> CustomerAcccountTransactions, Page page, PageWidget widget, long accountId, string tenantCode, string currency, string outputLocation)
        {
            var accountTransactions = CustomerAcccountTransactions.Where(item => item.CustomerId == customer.Identifier && item.AccountId == accountId)?.ToList();

            var transaction = new StringBuilder();
            var selectOption = new StringBuilder();
            if (accountTransactions != null && accountTransactions.Count > 0)
            {
                pageContent.Replace("{{Currency}}", currency);
                // convert it to json format string and store it as file at same directory of html statement file
                string savingtransactionjson = JsonConvert.SerializeObject(accountTransactions);
                if (savingtransactionjson != null && savingtransactionjson != string.Empty)
                {
                    var distinctNaration = accountTransactions.Select(item => item.Narration).Distinct().ToList();
                    distinctNaration.ForEach(item =>
                    {
                        selectOption.Append("<option value='" + item + "'> " + item + "</option>");
                    });

                    var SavingTransactionGridJson = "savingtransactiondata" + accountId + page.Identifier + "=" + savingtransactionjson;
                    this.WriteToJsonFile(SavingTransactionGridJson, "savingtransactiondetail" + accountId + page.Identifier + ".json", outputLocation);
                    scriptHtmlRenderer.Append("<script type='text/javascript' src='./savingtransactiondetail" + accountId + page.Identifier + ".json'></script>");

                    var scriptval = new StringBuilder(HtmlConstants.SAVING_TRANSACTION_DETAIL_GRID_WIDGET_SCRIPT);
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

        private void BindCurrentTransactionWidgetData(StringBuilder pageContent, StringBuilder scriptHtmlRenderer, CustomerMaster customer, BatchMaster batchMaster, IList<AccountTransaction> CustomerAcccountTransactions, Page page, PageWidget widget, long accountId, string tenantCode, string currency, string outputLocation)
        {
            var accountTransactions = CustomerAcccountTransactions.Where(item => item.CustomerId == customer.Identifier && item.AccountId == accountId)?.ToList();
            var transaction = new StringBuilder();
            var selectOption = new StringBuilder();
            if (accountTransactions != null && accountTransactions.Count > 0)
            {
                pageContent.Replace("{{Currency}}", currency);
                //convert it to json format string and store it as json file at same directory of html statement file
                string currenttransactionjson = JsonConvert.SerializeObject(accountTransactions);
                if (currenttransactionjson != null && currenttransactionjson != string.Empty)
                {
                    var distinctNaration = accountTransactions.Select(item => item.Narration).Distinct().ToList();
                    distinctNaration.ForEach(item =>
                    {
                        selectOption.Append("<option value='" + item + "'> " + item + "</option>");
                    });

                    var CurrentTransactionGridJson = "currenttransactiondata" + accountId + page.Identifier + "=" + currenttransactionjson;
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

        private void BindTop4IncomeSourcesWidgetData(StringBuilder pageContent, CustomerMaster customer, BatchMaster batchMaster, Page page, PageWidget widget, string tenantCode)
        {
            try
            {
                var top4IncomeSources = this.tenantTransactionDataManager.GetCustomerIncomeSources(customer.Identifier, batchMaster.Identifier, tenantCode)?.OrderByDescending(it => Convert.ToDecimal(it.CurrentSpend))?.Take(4)?.ToList();
                var incomeSources = new StringBuilder();
                if (top4IncomeSources != null && top4IncomeSources.Count > 0)
                {
                    top4IncomeSources.ToList().ForEach(src =>
                    {
                        var tdstring = string.Empty;
                        if (Convert.ToDecimal(src.CurrentSpend) > Convert.ToDecimal(src.AverageSpend))
                        {
                            tdstring = "<span class='fa fa-sort-desc fa-2x text-danger' aria-hidden='true'></span><span class='ml-2'>" + src.AverageSpend + "</span>";
                        }
                        else
                        {
                            tdstring = "<span class='fa fa-sort-asc fa-2x mt-1' aria-hidden='true' style='position:relative;top:6px;color:limegreen'></span><span class='ml-2'>" + src.AverageSpend + "</span>";
                        }
                        incomeSources.Append("<tr><td class='float-left'>" + src.Source + "</td>" + "<td> " + src.CurrentSpend + "</td><td>" + tdstring + "</td></tr>");
                    });
                }
                else
                {
                    incomeSources.Append("<tr><td colspan='3' class='text-danger text-center'><div style='margin-top: 20px;'>No data available</div></td></tr>");
                }
                pageContent.Replace("{{IncomeSourceList_" + page.Identifier + "_" + widget.Identifier + "}}", incomeSources.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void BindAnalyticsChartWidgetData(StringBuilder pageContent, StringBuilder scriptHtmlRenderer, IList<AccountMaster> accountrecords, Page page, string outputLocation)
        {
            var AnalyticsChartJson = string.Empty;
            if (accountrecords.Count > 0)
            {
                var accounts = new List<AccountMasterRecord>();
                var records = accountrecords.GroupBy(item => item.AccountType).ToList();

                //get analytics chart widget data, convert it into json string format and store it as json file at same directory of html statement file
                records.ForEach(acc => accounts.Add(new AccountMasterRecord()
                {
                    AccountType = acc.FirstOrDefault().AccountType,
                    Percentage = acc.Average(item => item.Percentage == null || item.Percentage == string.Empty ? 0 : Convert.ToDecimal(item.Percentage))
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

        private void BindSavingTrendChartWidgetData(StringBuilder pageContent, StringBuilder scriptHtmlRenderer, CustomerMaster customer, BatchMaster batchMaster, IList<SavingTrend> CustomerSavingTrends, long accountId, Page page, string outputLocation)
        {
            var SavingTrendChartJson = string.Empty;
            var savingtrends = CustomerSavingTrends.Where(item => item.CustomerId == customer.Identifier && item.BatchId == batchMaster.Identifier && item.AccountId == accountId).ToList();
            if (savingtrends != null && savingtrends.Count > 0)
            {
                var currentMonth = DateTime.Now.Month;
                int mnth = currentMonth - 1 == 0 ? 12 : currentMonth - 1;  //To start month validation of consecutive month data from previous month
                for (int t = savingtrends.Count; t > 0; t--)
                {
                    string month = this.utility.getMonth(mnth);
                    var lst = savingtrends.Where(it => it.Month.ToLower().Contains(month.ToLower()))?.ToList();
                    if (lst != null && lst.Count > 0)
                    {
                        lst[0].NumericMonth = mnth;
                    }
                    mnth = mnth - 1 == 0 ? 12 : mnth - 1;
                }

                //get saving trend chart widget data, convert it into json string format and store it as json file at same directory of html statement file
                string savingtrendjson = JsonConvert.SerializeObject(savingtrends.OrderByDescending(item => item.NumericMonth).Take(6).ToList());
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

        private void BindSpendingTrendChartWidgetData(StringBuilder pageContent, StringBuilder scriptHtmlRenderer, CustomerMaster customer, BatchMaster batchMaster, IList<SavingTrend> CustomerSavingTrends, long accountId, Page page, string outputLocation)
        {
            var SpendingTrendChartJson = string.Empty;
            var spendingtrends = CustomerSavingTrends.Where(item => item.CustomerId == customer.Identifier && item.BatchId == batchMaster.Identifier && item.AccountId == accountId).ToList();
            if (spendingtrends != null && spendingtrends.Count > 0)
            {
                var currentMonth = DateTime.Now.Month;
                int mnth = currentMonth - 1 == 0 ? 12 : currentMonth - 1;  //To start month validation of consecutive month data from previous month
                for (int t = spendingtrends.Count; t > 0; t--)
                {
                    string month = this.utility.getMonth(mnth);
                    var lst = spendingtrends.Where(it => it.Month.ToLower().Contains(month.ToLower()))?.ToList();
                    if (lst != null && lst.Count > 0)
                    {
                        lst[0].NumericMonth = mnth;
                    }
                    mnth = mnth - 1 == 0 ? 12 : mnth - 1;
                }

                //get spending trend chart widget data, convert it into json string format and store it as json file at same directory of html statement file
                string spendingtrendjson = JsonConvert.SerializeObject(spendingtrends.OrderByDescending(item => item.NumericMonth).Take(6).ToList());
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

        private void BindReminderAndRecommendationWidgetData(StringBuilder pageContent, CustomerMaster customer, BatchMaster batchMaster, Page page, PageWidget widget, string tenantCode)
        {
            try
            {
                var reminderAndRecommendations = this.tenantTransactionDataManager.GetReminderAndRecommendation(customer.Identifier, batchMaster.Identifier, tenantCode);
                var reminderstr = new StringBuilder();
                if (reminderAndRecommendations != null && reminderAndRecommendations.Count > 0)
                {
                    reminderstr.Append("<div class='row'><div class='col-lg-9'></div><div class='col-lg-3 text-left'><i class='fa fa-caret-left fa-3x float-left text-danger' aria-hidden='true'></i><span class='mt-2 d-inline-block ml-2'>Click</span></div> </div>");
                    reminderAndRecommendations.ToList().ForEach(item =>
                    {
                        string targetlink = item.Action != null && item.Action != string.Empty ? item.Action : "javascript:void(0)";
                        reminderstr.Append("<div class='row'><div class='col-lg-9 text-left'><p class='p-1' style='background-color: #dce3dc;'>" + item.Description + " </p></div><div class='col-lg-3 text-left'><a href='" + targetlink + "' target='_blank'><i class='fa fa-caret-left fa-3x float-left text-danger'></i><span class='mt-2 d-inline-block ml-2'>" + item.Title + "</span></a></div></div>");
                    });
                }
                else
                {
                    reminderstr.Append("<div class='row text-danger text-center' style='margin-top: 20px;'>No data available</div>");
                }
                pageContent.Replace("{{ReminderAndRecommdationDataList_" + page.Identifier + "_" + widget.Identifier + "}}", reminderstr.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void BindDynamicTableWidgetData(StringBuilder pageContent, Page page, PageWidget widget, JObject searchParameter, DynamicWidget dynawidget, HttpClient httpClient)
        {
            try
            {
                var tableEntities = JsonConvert.DeserializeObject<List<DynamicWidgetTableEntity>>(dynawidget.WidgetSettings);
                var tr = new StringBuilder();

                //API call
                var response = httpClient.PostAsync(dynawidget.APIPath, new StringContent(JsonConvert.SerializeObject(searchParameter), Encoding.UTF8, ModelConstant.APPLICATION_JSON_MEDIA_TYPE)).Result;
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var apiOutputArr = JArray.Parse(response.Content.ReadAsStringAsync().Result);
                    if (apiOutputArr.Count > 0)
                    {
                        apiOutputArr.ToList().ForEach(op =>
                        {
                            tr.Append("<tr>");
                            tableEntities.ForEach(field =>
                            {
                                tr.Append("<td> " + op[field.FieldName] + " </td>");
                            });
                            tr.Append("</tr>");
                        });
                    }
                    else
                    {
                        tr.Append("<tr><td colspan='" + (tableEntities.Count + 1) + "'> No record found </td></tr>)");
                    }
                }
                else
                {
                    tr.Append("<tr><td colspan='" + (tableEntities.Count + 1) + "'> No record found </td></tr>");
                }
                pageContent.Replace("{{tableBody_" + page.Identifier + "_" + widget.Identifier + "}}", tr.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void BindDynamicFormWidgetData(StringBuilder pageContent, Page page, PageWidget widget, JObject searchParameter, DynamicWidget dynawidget, HttpClient httpClient)
        {
            try
            {
                var formEntities = JsonConvert.DeserializeObject<List<DynamicWidgetFormEntity>>(dynawidget.WidgetSettings);
                var formdata = new StringBuilder();

                //API call
                var response = httpClient.PostAsync(dynawidget.APIPath, new StringContent(JsonConvert.SerializeObject(searchParameter), Encoding.UTF8, ModelConstant.APPLICATION_JSON_MEDIA_TYPE)).Result;
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var apiOutputArr = JArray.Parse(response.Content.ReadAsStringAsync().Result);
                    if (apiOutputArr.Count > 0)
                    {
                        apiOutputArr.ToList().ForEach(op =>
                        {
                            formEntities.ForEach(field =>
                            {
                                formdata.Append("<div class='row'><div class='col-sm-6'><label>" + field.DisplayName + "</label></div><div class='col-sm-6'>" + op[field.FieldName] + "</div></div>");
                            });
                        });
                    }
                    else
                    {
                        formdata.Append("<div class='row'> No record found </div>");
                    }
                }
                else
                {
                    formdata.Append("<div class='row'> No record found </div>");
                }

                pageContent.Replace("{{FormData_" + page.Identifier + "_" + widget.Identifier + "}}", formdata.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void BindDynamicLineGraphWidgetData(StringBuilder pageContent, StringBuilder scriptHtmlRenderer, Page page, PageWidget widget, JObject searchParameter, DynamicWidget dynawidget, HttpClient httpClient, CustomeTheme themeDetails)
        {
            try
            {
                var chartDataVal = string.Empty;

                //API call
                var response = httpClient.PostAsync(dynawidget.APIPath, new StringContent(JsonConvert.SerializeObject(searchParameter), Encoding.UTF8, ModelConstant.APPLICATION_JSON_MEDIA_TYPE)).Result;
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var apiOutputArr = JArray.Parse(response.Content.ReadAsStringAsync().Result);
                    if (apiOutputArr.Count > 0)
                    {
                        var graphEntity = JsonConvert.DeserializeObject<DynamicWidgetLineGraph>(dynawidget.WidgetSettings);
                        var chartData = new GraphChartData();
                        chartData.title = new ChartTitle { text = dynawidget.Title };

                        //To get chart x-axis list
                        var xAxis = apiOutputArr.ToList().Select(item => item[graphEntity.XAxis].ToString()).ToList();
                        chartData.xAxis = xAxis;

                        //To get chart series data
                        var chartSeries = new List<ChartSeries>();
                        graphEntity.Details.ToList().ForEach(field =>
                        {
                            var series = new ChartSeries();
                            series.name = field.DisplayName;
                            var res = apiOutputArr.ToList().Select(item => item[field.FieldName]).ToList();
                            var seriesdata = new List<decimal>();
                            res.ForEach(r =>
                            {
                                seriesdata.Add(Convert.ToDecimal(r.ToString()));
                            });
                            series.data = seriesdata;
                            series.type = "line";
                            chartSeries.Add(series);
                        });
                        chartData.series = chartSeries;

                        //To get chart theme
                        string theme = string.Empty;
                        if (themeDetails != null)
                        {
                            if (themeDetails.ChartColorTheme != null && themeDetails.ChartColorTheme != "")
                            {
                                theme = themeDetails.ChartColorTheme;
                            }
                            else if (themeDetails.ColorTheme != null && themeDetails.ColorTheme != "")
                            {
                                theme = themeDetails.ColorTheme;
                            }
                        }
                        chartData.color = this.GetChartColorTheme(theme);
                        chartDataVal = JsonConvert.SerializeObject(chartData);
                    }
                }

                pageContent.Replace("hiddenLineGraphValue_" + page.Identifier + "_" + widget.Identifier + "", chartDataVal);
                scriptHtmlRenderer.Append(HtmlConstants.LINE_GRAPH_WIDGET_SCRIPT.Replace("linechartcontainer", "lineGraphcontainer_" + page.Identifier + "_" + widget.Identifier).Replace("hiddenLineGraphData", "hiddenLineGraphData_" + page.Identifier + "_" + widget.Identifier));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void BindDynamicBarGraphWidgetData(StringBuilder pageContent, StringBuilder scriptHtmlRenderer, Page page, PageWidget widget, JObject searchParameter, DynamicWidget dynawidget, HttpClient httpClient, CustomeTheme themeDetails)
        {
            try
            {
                var chartDataVal = string.Empty;

                //API call
                var response = httpClient.PostAsync(dynawidget.APIPath, new StringContent(JsonConvert.SerializeObject(searchParameter), Encoding.UTF8, ModelConstant.APPLICATION_JSON_MEDIA_TYPE)).Result;
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var apiOutputArr = JArray.Parse(response.Content.ReadAsStringAsync().Result);
                    if (apiOutputArr.Count > 0)
                    {
                        var graphEntity = JsonConvert.DeserializeObject<DynamicWidgetLineGraph>(dynawidget.WidgetSettings);
                        var chartData = new GraphChartData();
                        chartData.title = new ChartTitle { text = dynawidget.Title };

                        //To get chart x-axis list
                        var xAxis = apiOutputArr.ToList().Select(item => item[graphEntity.XAxis].ToString()).ToList();
                        chartData.xAxis = xAxis;

                        //To get chart series data
                        var chartSeries = new List<ChartSeries>();
                        graphEntity.Details.ToList().ForEach(field =>
                        {
                            var series = new ChartSeries();
                            series.name = field.DisplayName;
                            var res = apiOutputArr.ToList().Select(item => item[field.FieldName]).ToList();
                            var seriesdata = new List<decimal>();
                            res.ForEach(r =>
                            {
                                seriesdata.Add(Convert.ToDecimal(r.ToString()));
                            });
                            series.data = seriesdata;
                            series.type = "column";
                            chartSeries.Add(series);
                        });
                        chartData.series = chartSeries;

                        //To get chart theme
                        string theme = string.Empty;
                        if (themeDetails != null)
                        {
                            if (themeDetails.ChartColorTheme != null && themeDetails.ChartColorTheme != "")
                            {
                                theme = themeDetails.ChartColorTheme;
                            }
                            else if (themeDetails.ColorTheme != null && themeDetails.ColorTheme != "")
                            {
                                theme = themeDetails.ColorTheme;
                            }
                        }
                        chartData.color = this.GetChartColorTheme(theme);
                        chartDataVal = JsonConvert.SerializeObject(chartData);
                    }
                }

                pageContent.Replace("hiddenBarGraphValue_" + page.Identifier + "_" + widget.Identifier + "", chartDataVal);
                scriptHtmlRenderer.Append(HtmlConstants.BAR_GRAPH_WIDGET_SCRIPT.Replace("barchartcontainer", "barGraphcontainer_" + page.Identifier + "_" + widget.Identifier).Replace("hiddenBarGraphData", "hiddenBarGraphData_" + page.Identifier + "_" + widget.Identifier));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void BindDynamicPieChartWidgetData(StringBuilder pageContent, StringBuilder scriptHtmlRenderer, Page page, PageWidget widget, JObject searchParameter, DynamicWidget dynawidget, HttpClient httpClient, CustomeTheme themeDetails, string tenantCode)
        {
            try
            {
                var chartDataVal = string.Empty;

                //API call
                var response = httpClient.PostAsync(dynawidget.APIPath, new StringContent(JsonConvert.SerializeObject(searchParameter), Encoding.UTF8, ModelConstant.APPLICATION_JSON_MEDIA_TYPE)).Result;
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var apiOutputArr = JArray.Parse(response.Content.ReadAsStringAsync().Result);
                    if (apiOutputArr.Count > 0)
                    {
                        var pieChartSetting = JsonConvert.DeserializeObject<PieChartSettingDetails>(dynawidget.WidgetSettings);
                        var entityFields = this.dynamicWidgetManager.GetEntityFields(dynawidget.EntityId, tenantCode);
                        var seriesfor = entityFields.Where(it => it.Identifier == Convert.ToInt32(pieChartSetting.PieSeries))?.ToList()?.FirstOrDefault().Name;
                        var seriesdatafor = entityFields.Where(it => it.Identifier == Convert.ToInt32(pieChartSetting.PieValue))?.ToList()?.FirstOrDefault().Name;

                        var chartData = new PiChartGraphData();
                        chartData.title = new ChartTitle { text = dynawidget.Title };

                        //To get series data
                        var chartseries = new List<PieChartSeries>();
                        var datas = new List<PieChartData>();
                        apiOutputArr.ToList().ForEach(item =>
                        {
                            var pie = new PieChartData
                            {
                                name = item[seriesfor] != null ? item[seriesfor].ToString() : "",
                                y = Convert.ToDecimal(item[seriesdatafor] != null ? item[seriesdatafor] : 0)
                            };
                            datas.Add(pie);
                        });

                        var series = new PieChartSeries();
                        series.name = seriesfor;
                        series.data = datas;
                        chartseries.Add(series);
                        chartData.series = chartseries;

                        //To get chart theme
                        string theme = string.Empty;
                        if (themeDetails != null)
                        {
                            if (themeDetails.ChartColorTheme != null && themeDetails.ChartColorTheme != "")
                            {
                                theme = themeDetails.ChartColorTheme;
                            }
                            else if (themeDetails.ColorTheme != null && themeDetails.ColorTheme != "")
                            {
                                theme = themeDetails.ColorTheme;
                            }
                        }
                        chartData.color = this.GetChartColorTheme(theme);
                        chartDataVal = JsonConvert.SerializeObject(chartData);
                    }
                }

                pageContent.Replace("hiddenPieChartValue_" + page.Identifier + "_" + widget.Identifier + "", chartDataVal);
                scriptHtmlRenderer.Append(HtmlConstants.PIE_CHART_WIDGET_SCRIPT.Replace("pieChartcontainer", "pieChartcontainer_" + page.Identifier + "_" + widget.Identifier).Replace("hiddenPieChartData", "hiddenPieChartData_" + page.Identifier + "_" + widget.Identifier));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void BindDynamicHtmlWidgetData(StringBuilder pageContent, Page page, PageWidget widget, JObject searchParameter, DynamicWidget dynawidget, HttpClient httpClient)
        {
            try
            {
                var htmlWidgetContent = new StringBuilder(dynawidget.PreviewData);
                if (dynawidget.WidgetSettings != null)
                {
                    var _lstHtmlWidgetSettings = JsonConvert.DeserializeObject<List<HtmlWidgetSettings>>(dynawidget.WidgetSettings);
                    if (_lstHtmlWidgetSettings.Count > 0)
                    {
                        //API call
                        var response = httpClient.PostAsync(dynawidget.APIPath, new StringContent(JsonConvert.SerializeObject(searchParameter), Encoding.UTF8, ModelConstant.APPLICATION_JSON_MEDIA_TYPE)).Result;
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            var apiOutputArr = JArray.Parse(response.Content.ReadAsStringAsync().Result);
                            if (apiOutputArr.Count > 0)
                            {
                                var apidata = apiOutputArr.FirstOrDefault();
                                _lstHtmlWidgetSettings.ForEach(setting =>
                                {
                                    if (setting.Value != null && setting.Value != string.Empty && setting.Key != null && setting.Key != string.Empty && apidata[setting.Key] != null)
                                    {
                                        htmlWidgetContent.Replace(setting.Value, apidata[setting.Key].ToString());
                                    }
                                });
                            }
                        }
                    }
                }
                pageContent.Replace("{{FormData_" + page.Identifier + "_" + widget.Identifier + "}}", htmlWidgetContent.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region These methods helps to bind data to Image and Video widgets

        private void BindImageWidgetData(StringBuilder pageContent, long customerId, IList<CustomerMedia> customerMedias, IList<BatchDetail> batchDetails, Statement statement, Page page, BatchMaster batchMaster, PageWidget widget, string tenantCode, string outputLocation)
        {
            var imgHeight = "auto";
            var imgAlignment = "text-center";
            var imgAssetFilepath = string.Empty;

            if (widget.WidgetSetting != string.Empty && validationEngine.IsValidJson(widget.WidgetSetting))
            {
                dynamic widgetSetting = JObject.Parse(widget.WidgetSetting);
                if (widgetSetting.isPersonalize == false) //Is not dynamic image, then assign selected image from asset library
                {
                    var asset = this.assetLibraryManager.GetAssets(new AssetSearchParameter { Identifier = widgetSetting.AssetId, SortParameter = new SortParameter { SortColumn = "Id" } }, tenantCode).ToList()?.FirstOrDefault();
                    if (asset != null)
                    {
                        var path = asset.FilePath.ToString();
                        var fileName = asset.Name;
                        if (File.Exists(path) && !File.Exists(outputLocation + "\\" + fileName))
                        {
                            File.Copy(path, Path.Combine(outputLocation, fileName));
                        }
                        imgAssetFilepath = "./" + fileName;
                    }
                }
                else //Is dynamic image, then assign it from database 
                {
                    var custMedia = customerMedias.Where(item => item.PageId == page.Identifier && item.WidgetId == widget.Identifier)?.ToList()?.FirstOrDefault(); //error if multiple records
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

                if (!string.IsNullOrEmpty(Convert.ToString(widgetSetting.Height)) && Convert.ToString(widgetSetting.Height) != "0")
                {
                    imgHeight = widgetSetting.Height + "px";
                }

                if (widgetSetting.Align != null)
                {
                    imgAlignment = widgetSetting.Align == 1 ? "text-left" : widgetSetting.Align == 2 ? "text-right" : "text-center";
                }

                pageContent.Replace("{{ImgHeight_" + statement.Identifier + "_" + page.Identifier + "_" + widget.Identifier + "}}", imgHeight);
                pageContent.Replace("{{ImgAlignmentClass_" + statement.Identifier + "_" + page.Identifier + "_" + widget.Identifier + "}}", imgAlignment);
                pageContent.Replace("{{ImageSource_" + statement.Identifier + "_" + page.Identifier + "_" + widget.Identifier + "}}", imgAssetFilepath);
            }
        }

        private void BindVideoWidgetData(StringBuilder pageContent, long customerId, IList<CustomerMedia> customerMedias, IList<BatchDetail> batchDetails, Statement statement, Page page, BatchMaster batchMaster, PageWidget widget, string tenantCode, string outputLocation)
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
                    var asset = this.assetLibraryManager.GetAssets(new AssetSearchParameter { Identifier = widgetSetting.AssetId, SortParameter = new SortParameter { SortColumn = "Id" } }, tenantCode).ToList()?.FirstOrDefault();
                    if (asset != null)
                    {
                        var path = asset.FilePath.ToString();
                        var fileName = asset.Name;
                        if (File.Exists(path) && !File.Exists(outputLocation + "\\" + fileName))
                        {
                            File.Copy(path, Path.Combine(outputLocation, fileName));
                        }
                        vdoAssetFilepath = "./" + fileName;
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

        #endregion

        #region These methods helps to bind data to static widgets of Nedbank HTML statment

        private void BindCustomerDetailsWidgetData(StringBuilder pageContent, DM_CustomerMaster customer, Page page, PageWidget widget)
        {
            var CustomerDetails = customer.Title + " " + customer.FirstName + " " + customer.SurName + "<br>" +
                (!string.IsNullOrEmpty(customer.AddressLine0) ? (customer.AddressLine0 + "<br>") : string.Empty) +
                (!string.IsNullOrEmpty(customer.AddressLine1) ? (customer.AddressLine1 + "<br>") : string.Empty) +
                (!string.IsNullOrEmpty(customer.AddressLine2) ? (customer.AddressLine2 + "<br>") : string.Empty) +
                (!string.IsNullOrEmpty(customer.AddressLine3) ? (customer.AddressLine3 + "<br>") : string.Empty) +
                (!string.IsNullOrEmpty(customer.AddressLine4) ? customer.AddressLine4 : string.Empty);
            pageContent.Replace("{{CustomerDetails_" + page.Identifier + "_" + widget.Identifier + "}}", CustomerDetails);
            //pageContent.Replace("{{MaskCellNo_" + page.Identifier + "_" + widget.Identifier + "}}", customer.Mask_Cell_No != string.Empty ? "Cell: " + customer.Mask_Cell_No : string.Empty);
        }
        private void BindCorporateSaverCustomerDetailsWidgetData(StringBuilder pageContent, DM_CustomerMaster customer, Page page, PageWidget widget)
        {
            var CustomerDetails = customer.Title + " " + customer.FirstName + " " + customer.SurName + "<br>" +
                (!string.IsNullOrEmpty(customer.AddressLine0) ? (customer.AddressLine0 + "<br>") : string.Empty) +
                (!string.IsNullOrEmpty(customer.AddressLine1) ? (customer.AddressLine1 + "<br>") : string.Empty) +
                (!string.IsNullOrEmpty(customer.AddressLine2) ? (customer.AddressLine2 + "<br>") : string.Empty) +
                (!string.IsNullOrEmpty(customer.AddressLine3) ? (customer.AddressLine3 + "<br>") : string.Empty) +
                (!string.IsNullOrEmpty(customer.AddressLine4) ? customer.AddressLine4 : string.Empty);
            pageContent.Replace("{{CustomerDetails_" + page.Identifier + "_" + widget.Identifier + "}}", CustomerDetails);
            //pageContent.Replace("{{MaskCellNo_" + page.Identifier + "_" + widget.Identifier + "}}", customer.Mask_Cell_No != string.Empty ? "Cell: " + customer.Mask_Cell_No : string.Empty);
        }

        private void BindBranchDetailsWidgetData(StringBuilder pageContent, long BranchId, Page page, PageWidget widget, string tenantCode)
        {
            try
            {
                var branchDetails = this.tenantTransactionDataManager.Get_DM_BranchMaster(BranchId, tenantCode)?.FirstOrDefault();
                if (branchDetails != null)
                {
                    var BranchDetail = branchDetails.BranchName.ToUpper() + "<br>" +
                        (!string.IsNullOrEmpty(branchDetails.AddressLine0) ? (branchDetails.AddressLine0.ToUpper() + "<br>") : string.Empty) +
                        (!string.IsNullOrEmpty(branchDetails.AddressLine1) ? (branchDetails.AddressLine1.ToUpper() + "<br>") : string.Empty) +
                        (!string.IsNullOrEmpty(branchDetails.AddressLine2) ? (branchDetails.AddressLine2.ToUpper() + "<br>") : string.Empty) +
                        (!string.IsNullOrEmpty(branchDetails.AddressLine3) ? (branchDetails.AddressLine3.ToUpper() + "<br>") : string.Empty) +
                        (!string.IsNullOrEmpty(branchDetails.VatRegNo) ? "Bank VAT Reg No " + branchDetails.VatRegNo : string.Empty);

                    pageContent.Replace("{{BranchDetails_" + page.Identifier + "_" + widget.Identifier + "}}", BranchDetail);
                    pageContent.Replace("{{ContactCenter_" + page.Identifier + "_" + widget.Identifier + "}}", "Nedbank Private Wealth Service Suite: " + branchDetails.ContactNo);
                }
            }
            catch (Exception)
            {
            }
        }

        private void BindBondDetailsWidgetData(StringBuilder pageContent, Page page, PageWidget widget, List<DM_HomeLoanMaster> HomeLoans)
        {
            try
            {
                var BondDetails = new StringBuilder();
                if (HomeLoans.Count == 1)
                {
                    BondDetails.Append("Bond No: " + HomeLoans[0].InvestorId.ToString() + "<br>");
                }
                BondDetails.Append(DateTime.Now.ToString(ModelConstant.DATE_FORMAT_yyyy_MM_dd));

                pageContent.Replace("{{BranchDetails_" + page.Identifier + "_" + widget.Identifier + "}}", BondDetails.ToString());
                pageContent.Replace("{{ContactCenter_" + page.Identifier + "_" + widget.Identifier + "}}", "Professional Banking 24/7 Contact centre " + " 0860 555 222");
            }
            catch (Exception)
            {
            }
        }

        private void BindInvestmentPortfolioStatementWidgetData(StringBuilder pageContent, DM_CustomerMaster customer, List<DM_InvestmentMaster> investmentMasters, Page page, PageWidget widget)
        {
            if (investmentMasters != null && investmentMasters.Count > 0)
            {
                var TotalClosingBalance = 0.0m;
                investmentMasters.ForEach(invest =>
                {
                    var res = 0.0m;
                    try
                    {
                        TotalClosingBalance = TotalClosingBalance + invest.investmentTransactions.Where(it => it.TransactionDesc.ToLower().Contains("balance carried forward")).Select(it => decimal.TryParse(it.WJXBFS4_Balance, out res) ? it.WJXBFS4_Balance : "0").ToList().Sum(it => Convert.ToDecimal(it));
                    }
                    catch (Exception)
                    {
                        TotalClosingBalance = 0.0m;
                    }
                });

                pageContent.Replace("{{DSName_" + page.Identifier + "_" + widget.Identifier + "}}", customer.DS_Investor_Name);
                pageContent.Replace("{{TotalClosingBalance_" + page.Identifier + "_" + widget.Identifier + "}}", utility.CurrencyFormatting("en-ZA", ".", "C", TotalClosingBalance));
                pageContent.Replace("{{DayOfStatement_" + page.Identifier + "_" + widget.Identifier + "}}", investmentMasters[0].DayOfStatement);
                pageContent.Replace("{{InvestorID_" + page.Identifier + "_" + widget.Identifier + "}}", Convert.ToString(investmentMasters[0].InvestorId));

                //to separate to string dates values into required date format -- 
                //given date format for statement period is //2021-02-10 00:00:00.000 - 2021-03-09 23:00:00.000//
                //1st try with string separator, if fails then try with char separator
                var statementPeriod = string.Empty;
                string[] stringSeparators = new string[] { " - ", "- ", " -" };
                string[] dates = investmentMasters[0].StatementPeriod.Split(stringSeparators, StringSplitOptions.None);
                if (dates.Length > 0)
                {
                    if (dates.Length > 1)
                    {
                        statementPeriod = Convert.ToDateTime(dates[0]).ToString("dd'/'MM'/'yyyy") + " to " + Convert.ToDateTime(dates[1]).ToString("dd'/'MM'/'yyyy");
                    }
                    else
                    {
                        dates = investmentMasters[0].StatementPeriod.Split(new Char[] { ' ' });
                        if (dates.Length > 2)
                        {
                            statementPeriod = Convert.ToDateTime(dates[0]).ToString("dd'/'MM'/'yyyy") + " to " + Convert.ToDateTime(dates[2]).ToString("dd'/'MM'/'yyyy");
                        }
                    }
                }
                else
                {
                    statementPeriod = investmentMasters[0].StatementPeriod;
                }
                pageContent.Replace("{{StatementPeriod_" + page.Identifier + "_" + widget.Identifier + "}}", statementPeriod);

                pageContent.Replace("{{StatementDate_" + page.Identifier + "_" + widget.Identifier + "}}", investmentMasters[0].StatementDate?.ToString("dd'/'MM'/'yyyy"));
            }
        }

        private void BindInvestorPerformanceWidgetData(StringBuilder pageContent, List<DM_InvestmentMaster> investmentMasters, Page page, PageWidget widget)
        {
            if (investmentMasters != null && investmentMasters.Count > 0)
            {
                var TotalClosingBalance = 0.0m;
                var TotalOpeningBalance = 0.0m;
                investmentMasters.ForEach(invest =>
                {
                    var res = 0.0m;
                    try
                    {
                        TotalClosingBalance = TotalClosingBalance + invest.investmentTransactions.Where(it => it.TransactionDesc.ToLower().Contains("balance carried forward")).Select(it => decimal.TryParse(it.WJXBFS4_Balance, out res) ? it.WJXBFS4_Balance : "0").ToList().Sum(it => Convert.ToDecimal(it));
                    }
                    catch (Exception)
                    {
                        TotalClosingBalance = 0.0m;
                    }

                    res = 0.0m;
                    try
                    {
                        TotalOpeningBalance = TotalOpeningBalance + invest.investmentTransactions.Where(it => it.TransactionDesc.ToLower().Contains("balance brought forward")).Select(it => decimal.TryParse(it.WJXBFS4_Balance, out res) ? it.WJXBFS4_Balance : "0").ToList().Sum(it => Convert.ToDecimal(it));
                    }
                    catch (Exception)
                    {
                        TotalOpeningBalance = 0.0m;
                    }
                });


                pageContent.Replace("{{ProductType_" + page.Identifier + "_" + widget.Identifier + "}}", investmentMasters[0].ProductType);
                pageContent.Replace("{{OpeningBalanceAmount_" + page.Identifier + "_" + widget.Identifier + "}}", utility.CurrencyFormatting("en-ZA", ".", "C", TotalOpeningBalance));
                pageContent.Replace("{{ClosingBalanceAmount_" + page.Identifier + "_" + widget.Identifier + "}}", utility.CurrencyFormatting("en-ZA", ".", "C", TotalClosingBalance));
            }
        }

        private void BindBreakdownOfInvestmentAccountsWidgetData(StringBuilder pageContent, List<DM_InvestmentMaster> investmentMasters, Page page, PageWidget widget)
        {
            if (investmentMasters != null && investmentMasters.Count > 0)
            {
                var InvestmentAccountsCount = investmentMasters.Count;
                pageContent.Replace("{{NavTab_" + page.Identifier + "_" + widget.Identifier + "}}", string.Empty);

                var TabContentHtml = new StringBuilder();
                var counter = 0;
                TabContentHtml.Append((InvestmentAccountsCount > 1) ? "<div class='tab-content'>" : string.Empty);
                investmentMasters.ToList().ForEach(acc =>
                {
                    var InvestmentAccountDetailHtml = new StringBuilder(HtmlConstants.INVESTMENT_ACCOUNT_DETAILS_HTML);
                    InvestmentAccountDetailHtml.Replace("{{ProductDesc}}", acc.ProductDesc);
                    InvestmentAccountDetailHtml.Replace("{{InvestmentId}}", Convert.ToString(acc.InvestmentId));
                    InvestmentAccountDetailHtml.Replace("{{TabPaneClass}}", "" + (counter > 0 ? "pt-4" : string.Empty));

                    var InvestmentNo = Convert.ToString(acc.InvestorId) + " " + Convert.ToString(acc.InvestmentId);
                    //actual length is 12, due to space in between investor id and investment id we comparing for 13 characters
                    while (InvestmentNo.Length != 13)
                    {
                        InvestmentNo = "0" + InvestmentNo;
                    }
                    InvestmentAccountDetailHtml.Replace("{{InvestmentNo}}", InvestmentNo);
                    InvestmentAccountDetailHtml.Replace("{{AccountOpenDate}}", acc.AccountOpenDate != null ? acc.AccountOpenDate?.ToString("dd'/'MM'/'yyyy") : string.Empty);

                    InvestmentAccountDetailHtml.Replace("{{InterestRate}}", acc.CurrentInterestRate + "% pa");
                    InvestmentAccountDetailHtml.Replace("{{MaturityDate}}", acc.ExpiryDate != null ? acc.ExpiryDate?.ToString("dd'/'MM'/'yyyy") : string.Empty);
                    InvestmentAccountDetailHtml.Replace("{{InterestDisposal}}", acc.InterestDisposalDesc != null ? acc.InterestDisposalDesc : string.Empty);
                    InvestmentAccountDetailHtml.Replace("{{NoticePeriod}}", acc.NoticePeriod != null ? acc.NoticePeriod : string.Empty);
                    InvestmentAccountDetailHtml.Replace("{{InterestDue}}", acc.AccuredInterest != null ? (acc.Currenacy + acc.AccuredInterest) : string.Empty);

                    var res = 0.0m;
                    var LastInvestmentTransaction = acc.investmentTransactions.Where(it => it.TransactionDesc.ToLower().Contains("balance carried forward")).OrderByDescending(it => it.TransactionDate)?.ToList()?.FirstOrDefault();
                    if (LastInvestmentTransaction != null)
                    {
                        InvestmentAccountDetailHtml.Replace("{{LastTransactionDate}}", LastInvestmentTransaction.TransactionDate.ToString("dd MMMM yyyy"));
                        if (decimal.TryParse(LastInvestmentTransaction.WJXBFS4_Balance, out res))
                        {
                            InvestmentAccountDetailHtml.Replace("{{BalanceOfLastTransactionDate}}", (LastInvestmentTransaction.WJXBFS4_Balance == "0" ? "-" : (decimal.TryParse(LastInvestmentTransaction.WJXBFS4_Balance, out res) ? utility.CurrencyFormatting("en-ZA", ".", "C", (Convert.ToDecimal(LastInvestmentTransaction.WJXBFS4_Balance))) : "0")));
                        }
                        else
                        {
                            InvestmentAccountDetailHtml.Replace("{{BalanceOfLastTransactionDate}}", "0");
                        }
                    }
                    else
                    {
                        InvestmentAccountDetailHtml.Replace("{{LastTransactionDate}}", "");
                        InvestmentAccountDetailHtml.Replace("{{BalanceOfLastTransactionDate}}", "");
                    }

                    var InvestmentTransactionRows = new StringBuilder();
                    acc.investmentTransactions.OrderBy(it => it.TransactionDate).ToList().ForEach(trans =>
                    {
                        var tr = new StringBuilder();
                        tr.Append("<tr class='ht-20'>");
                        tr.Append("<td class='w-15 pt-1'>" + trans.TransactionDate.ToString("dd'/'MM'/'yyyy") + "</td>");
                        tr.Append("<td class='w-40 pt-1'>" + trans.TransactionDesc + "</td>");

                        res = 0.0m;
                        if (decimal.TryParse(trans.WJXBFS2_Debit, out res))
                        {
                            tr.Append("<td class='w-15 text-right pt-1'>" + (trans.WJXBFS2_Debit == "0" ? "-" : utility.CurrencyFormatting("en-ZA", ".", "C", (Convert.ToDecimal(trans.WJXBFS2_Debit)))) + "</td>");
                        }
                        else
                        {
                            tr.Append("<td class='w-15 text-right pt-1'> - </td>");
                        }

                        res = 0.0m;
                        if (decimal.TryParse(trans.WJXBFS3_Credit, out res))
                        {
                            tr.Append("<td class='w-15 text-right pt-1'>" + (trans.WJXBFS3_Credit == "0" ? "-" : utility.CurrencyFormatting("en-ZA", ".", "C", (Convert.ToDecimal(trans.WJXBFS3_Credit)))) + "</td>");
                        }
                        else
                        {
                            tr.Append("<td class='w-15 text-right pt-1'> - </td>");
                        }

                        res = 0.0m;
                        if (decimal.TryParse(trans.WJXBFS4_Balance, out res))
                        {
                            tr.Append("<td class='w-15 text-right pt-1'>" + (trans.WJXBFS4_Balance == "0" ? "-" : utility.CurrencyFormatting("en-ZA", ".", "C", (Convert.ToDecimal(trans.WJXBFS4_Balance)))) + "</td>");
                        }
                        else
                        {
                            tr.Append("<td class='w-15 text-right pt-1'> - </td>");
                        }

                        tr.Append("</tr>");
                        InvestmentTransactionRows.Append(tr.ToString());
                    });
                    InvestmentAccountDetailHtml.Replace("{{InvestmentTransactionRows}}", InvestmentTransactionRows.ToString());
                    TabContentHtml.Append(InvestmentAccountDetailHtml.ToString());
                    counter++;
                });
                TabContentHtml.Append((InvestmentAccountsCount > 1) ? "</div>" : string.Empty);
                pageContent.Replace("{{TabContentsDiv_" + page.Identifier + "_" + widget.Identifier + "}}", TabContentHtml.ToString());
            }
        }

        private void BindExplanatoryNotesWidgetData(StringBuilder pageContent, BatchMaster batchMaster, Page page, PageWidget widget, string tenantCode)
        {
            try
            {
                var ExPlanatoryNotes = this.tenantTransactionDataManager.Get_DM_ExplanatoryNotes(new MessageAndNoteSearchParameter() { BatchId = batchMaster.Identifier }, tenantCode)?.ToList();
                if (ExPlanatoryNotes != null && ExPlanatoryNotes.Count > 0)
                {
                    var notes = new StringBuilder();
                    notes.Append(string.IsNullOrEmpty(ExPlanatoryNotes[0].Note1) ? string.Empty : "<span> " + Convert.ToString(ExPlanatoryNotes[0].Note1) + " </span> <br/>");
                    notes.Append(string.IsNullOrEmpty(ExPlanatoryNotes[0].Note2) ? string.Empty : "<span> " + Convert.ToString(ExPlanatoryNotes[0].Note2) + " </span> <br/>");
                    notes.Append(string.IsNullOrEmpty(ExPlanatoryNotes[0].Note3) ? string.Empty : "<span> " + Convert.ToString(ExPlanatoryNotes[0].Note3) + " </span> <br/>");
                    notes.Append(string.IsNullOrEmpty(ExPlanatoryNotes[0].Note4) ? string.Empty : "<span> " + Convert.ToString(ExPlanatoryNotes[0].Note4) + " </span> <br/>");
                    notes.Append(string.IsNullOrEmpty(ExPlanatoryNotes[0].Note5) ? string.Empty : "<span> " + Convert.ToString(ExPlanatoryNotes[0].Note5) + " </span> <br/>");
                    pageContent.Replace("{{Notes_" + page.Identifier + "_" + widget.Identifier + "}}", notes.ToString());
                }
            }
            catch (Exception)
            {
            }
        }

        private void BindMarketingServiceWidgetData(StringBuilder pageContent, List<DM_MarketingMessage> Messages, Page page, PageWidget widget, int MarketingMessageCounter)
        {
            if (Messages != null && Messages.Count > 0)
            {
                var ServiceMessage = Messages.Count > MarketingMessageCounter ? Messages[MarketingMessageCounter] : null;
                if (ServiceMessage != null)
                {
                    var messageTxt = ((!string.IsNullOrEmpty(ServiceMessage.Message1)) ? "<p>" + ServiceMessage.Message1 + "</p>" : string.Empty) + ((!string.IsNullOrEmpty(ServiceMessage.Message2)) ? "<p>" + ServiceMessage.Message2 + "</p>" : string.Empty) + ((!string.IsNullOrEmpty(ServiceMessage.Message3)) ? "<p>" + ServiceMessage.Message3 + "</p>" : string.Empty) + ((!string.IsNullOrEmpty(ServiceMessage.Message4)) ? "<p>" + ServiceMessage.Message4 + "</p>" : string.Empty) + ((!string.IsNullOrEmpty(ServiceMessage.Message5)) ? "<p>" + ServiceMessage.Message5 + "</p>" : string.Empty);

                    pageContent.Replace("{{ServiceMessageHeader_" + page.Identifier + "_" + widget.Identifier + "_" + MarketingMessageCounter + "}}", ServiceMessage.Header).Replace("{{ServiceMessageText_" + page.Identifier + "_" + widget.Identifier + "_" + MarketingMessageCounter + "}}", messageTxt);
                }
            }
        }

        private void BindPersonalLoanDetailWidgetData(StringBuilder pageContent, BatchMaster batchMaster, DM_CustomerMaster customer, Page page, PageWidget widget, string tenantCode)
        {
            try
            {
                var PersonalLoan = this.tenantTransactionDataManager.Get_DM_PersonalLoanMaster(new CustomerPersonalLoanSearchParameter() { BatchId = batchMaster.Identifier, CustomerId = customer.CustomerId }, tenantCode)?.ToList()?.FirstOrDefault();
                if (PersonalLoan != null)
                {
                    var res = 0.0m;
                    if (decimal.TryParse(PersonalLoan.CreditAdvance, out res))
                    {
                        pageContent.Replace("{{TotalLoanAmount_" + page.Identifier + "_" + widget.Identifier + "}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                    }
                    else
                    {
                        pageContent.Replace("{{TotalLoanAmount_" + page.Identifier + "_" + widget.Identifier + "}}", string.Empty);
                    }

                    res = 0.0m;
                    if (decimal.TryParse(PersonalLoan.OutstandingBalance, out res))
                    {
                        pageContent.Replace("{{OutstandingBalance_" + page.Identifier + "_" + widget.Identifier + "}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                    }
                    else
                    {
                        pageContent.Replace("{{OutstandingBalance_" + page.Identifier + "_" + widget.Identifier + "}}", string.Empty);
                    }

                    res = 0.0m;
                    if (decimal.TryParse(PersonalLoan.AmountDue, out res))
                    {
                        pageContent.Replace("{{DueAmount_" + page.Identifier + "_" + widget.Identifier + "}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                    }
                    else
                    {
                        pageContent.Replace("{{DueAmount_" + page.Identifier + "_" + widget.Identifier + "}}", string.Empty);
                    }

                    pageContent.Replace("{{AccountNumber_" + page.Identifier + "_" + widget.Identifier + "}}", PersonalLoan.InvestorId.ToString());
                    pageContent.Replace("{{StatementDate_" + page.Identifier + "_" + widget.Identifier + "}}", PersonalLoan.ToDate.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy));
                    pageContent.Replace("{{StatementPeriod_" + page.Identifier + "_" + widget.Identifier + "}}", PersonalLoan.FromDate.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy) + " - " + PersonalLoan.ToDate.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy));

                    res = 0.0m;
                    if (decimal.TryParse(PersonalLoan.Arrears, out res))
                    {
                        pageContent.Replace("{{ArrearsAmount_" + page.Identifier + "_" + widget.Identifier + "}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                    }
                    else
                    {
                        pageContent.Replace("{{ArrearsAmount_" + page.Identifier + "_" + widget.Identifier + "}}", string.Empty);
                    }

                    pageContent.Replace("{{AnnualRate_" + page.Identifier + "_" + widget.Identifier + "}}", PersonalLoan.AnnualRate + "% pa");

                    res = 0.0m;
                    if (decimal.TryParse(PersonalLoan.MonthlyInstallment, out res))
                    {
                        pageContent.Replace("{{MonthlyInstallment_" + page.Identifier + "_" + widget.Identifier + "}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                    }
                    else
                    {
                        pageContent.Replace("{{MonthlyInstallment_" + page.Identifier + "_" + widget.Identifier + "}}", string.Empty);
                    }

                    pageContent.Replace("{{Terms_" + page.Identifier + "_" + widget.Identifier + "}}", PersonalLoan.Term);
                    pageContent.Replace("{{DueByDate_" + page.Identifier + "_" + widget.Identifier + "}}", PersonalLoan.DueDate.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy));
                }
            }
            catch (Exception)
            {
            }
        }

        private void BindPersonalLoanTransactionWidgetData(StringBuilder pageContent, BatchMaster batchMaster, Page page, PageWidget widget, DM_CustomerMaster customer, string tenantCode)
        {
            try
            {
                var transactions = this.tenantTransactionDataManager.Get_DM_PersonalLoanTransaction(new CustomerPersonalLoanSearchParameter() { BatchId = batchMaster.Identifier, CustomerId = customer.CustomerId }, tenantCode)?.ToList();
                if (transactions != null && transactions.Count > 0)
                {
                    var LoanTransactionRows = new StringBuilder();
                    var tr = new StringBuilder();
                    transactions.ForEach(trans =>
                    {
                        tr = new StringBuilder();
                        tr.Append("<tr class='ht-20'>");
                        tr.Append("<td class='w-13 text-center'> " + trans.PostingDate.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy) + " </td>");
                        tr.Append("<td class='w-15 text-center'> " + trans.EffectiveDate.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy) + " </td>");
                        tr.Append("<td class='w-35'> " + (!string.IsNullOrEmpty(trans.Description) ? trans.Description : ModelConstant.PAYMENT_THANK_YOU_TRANSACTION_DESC) + " </td>");

                        var res = 0.0m;
                        if (decimal.TryParse(trans.Debit, out res))
                        {
                            tr.Append("<td class='w-12 text-right'> " + (res > 0 ? utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res) : "-") + " </td>");
                        }
                        else
                        {
                            tr.Append("<td class='w-12 text-right'> - </td>");
                        }

                        res = 0.0m;
                        if (decimal.TryParse(trans.Credit, out res))
                        {
                            tr.Append("<td class='w-12 text-right'> " + (res > 0 ? utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res) : "-") + " </td>");
                        }
                        else
                        {
                            tr.Append("<td class='w-12 text-right'> - </td>");
                        }

                        res = 0.0m;
                        if (decimal.TryParse(trans.OutstandingCapital, out res))
                        {
                            tr.Append("<td class='w-13 text-right'> " + (res > 0 ? utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res) : "-") + " </td>");
                        }
                        else
                        {
                            tr.Append("<td class='w-13 text-right'> - </td>");
                        }
                        tr.Append("</tr>");
                        LoanTransactionRows.Append(tr.ToString());
                    });
                    pageContent.Replace("{{PersonalLoanTransactionRow_" + page.Identifier + "_" + widget.Identifier + "}}", LoanTransactionRows.ToString());
                }
                else
                {
                    pageContent.Replace("{{PersonalLoanTransactionRow_" + page.Identifier + "_" + widget.Identifier + "}}", "<tr><td class='text-center' colspan='6'>No record found</td></tr>");
                }
            }
            catch (Exception)
            {
            }
        }

        private void BindPersonalLoanPaymentDueWidgetData(StringBuilder pageContent, BatchMaster batchMaster, Page page, PageWidget widget, DM_CustomerMaster customer, string tenantCode)
        {
            try
            {
                var plArrears = this.tenantTransactionDataManager.Get_DM_PersonalLoanArrears(new CustomerPersonalLoanSearchParameter() { BatchId = batchMaster.Identifier, CustomerId = customer.CustomerId }, tenantCode)?.ToList()?.FirstOrDefault();
                if (plArrears != null)
                {
                    var res = 0.0m;
                    if (decimal.TryParse(plArrears.Arrears_120, out res))
                    {
                        pageContent.Replace("{{After120Days_" + page.Identifier + "_" + widget.Identifier + "}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                    }
                    else
                    {
                        pageContent.Replace("{{After120Days_" + page.Identifier + "_" + widget.Identifier + "}}", "R0.00");
                    }

                    res = 0.0m;
                    if (decimal.TryParse(plArrears.Arrears_90, out res))
                    {
                        pageContent.Replace("{{After90Days_" + page.Identifier + "_" + widget.Identifier + "}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                    }
                    else
                    {
                        pageContent.Replace("{{After90Days_" + page.Identifier + "_" + widget.Identifier + "}}", "R0.00");
                    }

                    res = 0.0m;
                    if (decimal.TryParse(plArrears.Arrears_60, out res))
                    {
                        pageContent.Replace("{{After60Days_" + page.Identifier + "_" + widget.Identifier + "}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                    }
                    else
                    {
                        pageContent.Replace("{{After60Days_" + page.Identifier + "_" + widget.Identifier + "}}", "R0.00");
                    }

                    res = 0.0m;
                    if (decimal.TryParse(plArrears.Arrears_30, out res))
                    {
                        pageContent.Replace("{{After30Days_" + page.Identifier + "_" + widget.Identifier + "}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                    }
                    else
                    {
                        pageContent.Replace("{{After30Days_" + page.Identifier + "_" + widget.Identifier + "}}", "R0.00");
                    }

                    res = 0.0m;
                    if (decimal.TryParse(plArrears.Arrears_0, out res))
                    {
                        pageContent.Replace("{{Current_" + page.Identifier + "_" + widget.Identifier + "}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                    }
                    else
                    {
                        pageContent.Replace("{{Current_" + page.Identifier + "_" + widget.Identifier + "}}", "R0.00");
                    }
                }
                else
                {
                    pageContent.Replace("{{After120Days_" + page.Identifier + "_" + widget.Identifier + "}}", "R0.00");
                    pageContent.Replace("{{After90Days_" + page.Identifier + "_" + widget.Identifier + "}}", "R0.00");
                    pageContent.Replace("{{After60Days_" + page.Identifier + "_" + widget.Identifier + "}}", "R0.00");
                    pageContent.Replace("{{After30Days_" + page.Identifier + "_" + widget.Identifier + "}}", "R0.00");
                    pageContent.Replace("{{Current_" + page.Identifier + "_" + widget.Identifier + "}}", "R0.00");
                }
            }
            catch (Exception)
            {
            }
        }

        private void BindSpecialMessageWidgetData(StringBuilder pageContent, SpecialMessage SpecialMessage, Page page, PageWidget widget)
        {
            if (SpecialMessage != null)
            {
                if (!string.IsNullOrEmpty(SpecialMessage.Header) || !string.IsNullOrEmpty(SpecialMessage.Message1) || !string.IsNullOrEmpty(SpecialMessage.Message2))
                {
                    var htmlWidget = new StringBuilder(HtmlConstants.SPECIAL_MESSAGE_HTML);
                    var specialMsgTxtData = (!string.IsNullOrEmpty(SpecialMessage.Header) ? "<div class='SpecialMessageHeader'> " + SpecialMessage.Header + " </div>" : string.Empty) + (!string.IsNullOrEmpty(SpecialMessage.Message1) ? "<p> " + SpecialMessage.Message1 + " </p>" : string.Empty) + (!string.IsNullOrEmpty(SpecialMessage.Message2) ? "<p> " + SpecialMessage.Message2 + " </p>" : string.Empty);
                    htmlWidget.Replace("{{SpecialMessageTextData}}", specialMsgTxtData);
                    htmlWidget = htmlWidget.Replace("{{WidgetId}}", "PageWidgetId_" + widget.Identifier + "_Counter" + (new Random().Next(100)).ToString());
                    pageContent.Replace("{{SpecialMessageTextDataDiv_" + page.Identifier + "_" + widget.Identifier + "}}", htmlWidget.ToString());
                }
                else
                {
                    pageContent.Replace("{{SpecialMessageTextDataDiv_" + page.Identifier + "_" + widget.Identifier + "}}", string.Empty);
                }
            }
            else
            {
                pageContent.Replace("{{SpecialMessageTextDataDiv_" + page.Identifier + "_" + widget.Identifier + "}}", string.Empty);
            }
        }

        private void BindPersonalLoanInsuranceMessageWidgetData(StringBuilder pageContent, SpecialMessage InsuranceMsg, Page page, PageWidget widget)
        {
            if (InsuranceMsg != null)
            {
                if (!string.IsNullOrEmpty(InsuranceMsg.Message3) || !string.IsNullOrEmpty(InsuranceMsg.Message4) || !string.IsNullOrEmpty(InsuranceMsg.Message5))
                {
                    var htmlWidget = new StringBuilder(HtmlConstants.PERSONAL_LOAN_INSURANCE_MESSAGE_HTML);
                    var InsuranceMsgTxtData = (!string.IsNullOrEmpty(InsuranceMsg.Message3) ? "<p> " + InsuranceMsg.Message3 + " </p>" : string.Empty) +
                       (!string.IsNullOrEmpty(InsuranceMsg.Message4) ? "<p> " + InsuranceMsg.Message4 + " </p>" : string.Empty) +
                       (!string.IsNullOrEmpty(InsuranceMsg.Message5) ? "<p> " + InsuranceMsg.Message5 + " </p>" : string.Empty);
                    htmlWidget.Replace("{{InsuranceMessages}}", InsuranceMsgTxtData);
                    htmlWidget = htmlWidget.Replace("{{WidgetId}}", "PageWidgetId_" + widget.Identifier + "_Counter" + (new Random().Next(100)).ToString());
                    pageContent.Replace("{{PersonalLoanInsuranceMessagesDiv_" + page.Identifier + "_" + widget.Identifier + "}}", htmlWidget.ToString());
                }
                else
                {
                    pageContent.Replace("{{PersonalLoanInsuranceMessagesDiv_" + page.Identifier + "_" + widget.Identifier + "}}", string.Empty);
                }
            }
            else
            {
                pageContent.Replace("{{PersonalLoanInsuranceMessagesDiv_" + page.Identifier + "_" + widget.Identifier + "}}", string.Empty);
            }
        }

        private void BindPersonalLoanTotalAmountDetailWidgetData(StringBuilder pageContent, List<DM_PersonalLoanMaster> PersonalLoans, Page page, PageWidget widget)
        {
            try
            {
                var TotalLoanAmt = 0.0m;
                var TotalOutstandingAmt = 0.0m;
                var TotalLoanDueAmt = 0.0m;

                if (PersonalLoans != null && PersonalLoans.Count > 0)
                {
                    var res = 0.0m;
                    try
                    {
                        TotalLoanAmt = PersonalLoans.Select(it => decimal.TryParse(it.CreditAdvance, out res) ? it.CreditAdvance : "0").ToList().Sum(it => Convert.ToDecimal(it));
                    }
                    catch
                    {
                        TotalLoanAmt = 0.0m;
                    }

                    res = 0.0m;
                    try
                    {
                        TotalOutstandingAmt = PersonalLoans.Select(it => decimal.TryParse(it.OutstandingBalance, out res) ? it.OutstandingBalance : "0").ToList().Sum(it => Convert.ToDecimal(it));
                    }
                    catch
                    {
                        TotalOutstandingAmt = 0.0m;
                    }

                    res = 0.0m;
                    try
                    {
                        TotalLoanDueAmt = PersonalLoans.Select(it => decimal.TryParse(it.AmountDue, out res) ? it.AmountDue : "0").ToList().Sum(it => Convert.ToDecimal(it));
                    }
                    catch
                    {
                        TotalLoanDueAmt = 0.0m;
                    }
                }

                pageContent.Replace("{{TotalLoanAmount_" + page.Identifier + "_" + widget.Identifier + "}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, TotalLoanAmt));
                pageContent.Replace("{{OutstandingBalance_" + page.Identifier + "_" + widget.Identifier + "}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, TotalOutstandingAmt));
                pageContent.Replace("{{DueAmount_" + page.Identifier + "_" + widget.Identifier + "}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, TotalLoanDueAmt));
            }
            catch
            {
            }
        }

        private void BindPersonalLoanAccountsBreakdownWidgetData(StringBuilder pageContent, List<DM_PersonalLoanMaster> PersonalLoans, Page page, PageWidget widget)
        {
            try
            {
                if (PersonalLoans != null && PersonalLoans.Count > 0)
                {
                    var counter = 0;
                    pageContent.Replace("{{NavTab_" + page.Identifier + "_" + widget.Identifier + "}}", string.Empty);

                    var TabContentHtml = new StringBuilder();
                    counter = 0;
                    TabContentHtml.Append((PersonalLoans.Count > 1) ? "<div class='tab-content'>" : string.Empty);
                    PersonalLoans.ForEach(PersonalLoan =>
                    {
                        var LoanDetailHtml = new StringBuilder(HtmlConstants.PERSONAL_LOAN_ACCOUNT_DETAIL);
                        LoanDetailHtml.Replace("{{AccountNumber}}", PersonalLoan.InvestorId.ToString());
                        LoanDetailHtml.Replace("{{StatementDate}}", PersonalLoan.ToDate.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy));
                        LoanDetailHtml.Replace("{{StatementPeriod}}", PersonalLoan.FromDate.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy) + " - " + PersonalLoan.ToDate.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy));

                        var res = 0.0m;
                        if (decimal.TryParse(PersonalLoan.Arrears, out res))
                        {
                            LoanDetailHtml.Replace("{{ArrearsAmount}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                        }
                        else
                        {
                            LoanDetailHtml.Replace("{{ArrearsAmount}}", "R0.00");
                        }

                        LoanDetailHtml.Replace("{{AnnualRate}}", PersonalLoan.AnnualRate + "% pa");

                        res = 0.0m;
                        if (decimal.TryParse(PersonalLoan.MonthlyInstallment, out res))
                        {
                            LoanDetailHtml.Replace("{{MonthlyInstallment}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                        }
                        else
                        {
                            LoanDetailHtml.Replace("{{MonthlyInstallment}}", "R0.00");
                        }

                        LoanDetailHtml.Replace("{{Terms}}", PersonalLoan.Term);
                        LoanDetailHtml.Replace("{{DueByDate}}", PersonalLoan.DueDate.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy));
                        TabContentHtml.Append(LoanDetailHtml.ToString());

                        var LoanTransactionRows = new StringBuilder();
                        var tr = new StringBuilder();
                        if (PersonalLoan.LoanTransactions != null && PersonalLoan.LoanTransactions.Count > 0)
                        {
                            var LoanTransactionDetailHtml = new StringBuilder(HtmlConstants.PERSONAL_LOAN_ACCOUNT_TRANSACTION_DETAIL).Replace("style='max-height:200px;'", string.Empty);
                            PersonalLoan.LoanTransactions.ForEach(trans =>
                            {
                                tr = new StringBuilder();
                                tr.Append("<tr class='ht-20'>");
                                tr.Append("<td class='w-13 text-center'> " + trans.PostingDate.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy) + " </td>");
                                tr.Append("<td class='w-15 text-center'> " + trans.EffectiveDate.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy) + " </td>");
                                tr.Append("<td class='w-35'> " + (!string.IsNullOrEmpty(trans.Description) ? trans.Description : ModelConstant.PAYMENT_THANK_YOU_TRANSACTION_DESC) + " </td>");

                                res = 0.0m;
                                if (decimal.TryParse(trans.Debit, out res))
                                {
                                    tr.Append("<td class='w-12 text-right'> " + (res > 0 ? utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res) : "-") + " </td>");
                                }
                                else
                                {
                                    tr.Append("<td class='w-12 text-right'> - </td>");
                                }

                                res = 0.0m;
                                if (decimal.TryParse(trans.Credit, out res))
                                {
                                    tr.Append("<td class='w-12 text-right'> " + (res > 0 ? utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res) : "-") + " </td>");
                                }
                                else
                                {
                                    tr.Append("<td class='w-12 text-right'> - </td>");
                                }

                                res = 0.0m;
                                if (decimal.TryParse(trans.OutstandingCapital, out res))
                                {
                                    tr.Append("<td class='w-13 text-right'> " + (res > 0 ? utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res) : "-") + " </td>");
                                }
                                else
                                {
                                    tr.Append("<td class='w-13 text-right'> - </td>");
                                }
                                tr.Append("</tr>");

                                LoanTransactionRows.Append(tr.ToString());
                            });

                            LoanTransactionDetailHtml.Replace("{{PersonalLoanTransactionRow}}", LoanTransactionRows.ToString());
                            TabContentHtml.Append(LoanTransactionDetailHtml.ToString());
                        }

                        if (PersonalLoan.LoanArrears != null)
                        {
                            var plArrears = PersonalLoan.LoanArrears;
                            var LoanArrearHtml = new StringBuilder(HtmlConstants.PERSONAL_LOAN_PAYMENT_DUE_DETAIL);
                            res = 0.0m;
                            if (decimal.TryParse(plArrears.Arrears_120, out res))
                            {
                                LoanArrearHtml.Replace("{{After120Days}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                            }
                            else
                            {
                                LoanArrearHtml.Replace("{{After120Days}}", "R0.00");
                            }

                            res = 0.0m;
                            if (decimal.TryParse(plArrears.Arrears_90, out res))
                            {
                                LoanArrearHtml.Replace("{{After90Days}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                            }
                            else
                            {
                                LoanArrearHtml.Replace("{{After90Days}}", "R0.00");
                            }

                            res = 0.0m;
                            if (decimal.TryParse(plArrears.Arrears_60, out res))
                            {
                                LoanArrearHtml.Replace("{{After60Days}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                            }
                            else
                            {
                                LoanArrearHtml.Replace("{{After60Days}}", "R0.00");
                            }

                            res = 0.0m;
                            if (decimal.TryParse(plArrears.Arrears_30, out res))
                            {
                                LoanArrearHtml.Replace("{{After30Days}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                            }
                            else
                            {
                                LoanArrearHtml.Replace("{{After30Days}}", "R0.00");
                            }

                            res = 0.0m;
                            if (decimal.TryParse(plArrears.Arrears_0, out res))
                            {
                                LoanArrearHtml.Replace("{{Current}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                            }
                            else
                            {
                                LoanArrearHtml.Replace("{{Current}}", "R0.00");
                            }

                            TabContentHtml.Append(LoanArrearHtml.ToString());
                        }
                        counter++;
                    });

                    TabContentHtml.Append((PersonalLoans.Count > 1) ? HtmlConstants.END_DIV_TAG : string.Empty);
                    pageContent.Replace("{{TabContentsDiv_" + page.Identifier + "_" + widget.Identifier + "}}", TabContentHtml.ToString());
                }
            }
            catch
            {
            }
        }

        private void BindHomeLoanTotalAmountDetailWidgetData(StringBuilder pageContent, List<DM_HomeLoanMaster> HomeLoans, Page page, PageWidget widget)
        {
            try
            {
                var TotalLoanAmt = 0.0m;
                var TotalOutstandingAmt = 0.0m;

                if (HomeLoans != null && HomeLoans.Count > 0)
                {
                    var res = 0.0m;
                    try
                    {
                        TotalLoanAmt = HomeLoans.Select(it => decimal.TryParse(it.LoanAmount, out res) ? res : 0).ToList().Sum(it => it);
                    }
                    catch
                    {
                        TotalLoanAmt = 0.0m;
                    }

                    res = 0.0m;
                    try
                    {
                        TotalOutstandingAmt = HomeLoans.Select(it => decimal.TryParse(it.Balance, out res) ? res : 0).ToList().Sum(it => it);
                    }
                    catch
                    {
                        TotalOutstandingAmt = 0.0m;
                    }
                }

                pageContent.Replace("{{TotalHomeLoansAmount_" + page.Identifier + "_" + widget.Identifier + "}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, TotalLoanAmt));
                pageContent.Replace("{{TotalHomeLoansBalanceOutstanding_" + page.Identifier + "_" + widget.Identifier + "}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, TotalOutstandingAmt));
            }
            catch
            {
            }
        }

        private void BindHomeLoanAccountsBreakdownWidgetData(StringBuilder pageContent, List<DM_HomeLoanMaster> HomeLoans, Page page, PageWidget widget)
        {
            try
            {
                if (HomeLoans != null && HomeLoans.Count > 0)
                {
                    pageContent.Replace("{{NavTab_" + page.Identifier + "_" + widget.Identifier + "}}", string.Empty);

                    //create tab-content div if accounts is greater than 1, otherwise create simple div
                    var TabContentHtml = new StringBuilder();
                    var counter = 0;
                    TabContentHtml.Append((HomeLoans.Count > 1) ? "<div class='tab-content'>" : string.Empty);
                    HomeLoans.ForEach(HomeLoan =>
                    {
                        var accNo = HomeLoan.InvestorId.ToString();
                        string lastFourDigisOfAccountNumber = accNo.Length > 4 ? accNo.Substring(Math.Max(0, accNo.Length - 4)) : accNo;

                        var LoanDetailHtml = new StringBuilder(HtmlConstants.HOME_LOAN_ACCOUNT_DETAIL_DIV_HTML);
                        LoanDetailHtml.Replace("{{BondNumber}}", accNo);
                        LoanDetailHtml.Replace("{{RegistrationDate}}", HomeLoan.RegisteredDate.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy));

                        var secDesc1 = string.Empty;
                        var secDesc2 = string.Empty;
                        var secDesc3 = string.Empty;
                        if (HomeLoan.SecDescription1.Length > 15 || ((HomeLoan.SecDescription1 + " " + HomeLoan.SecDescription2).Length > 25))
                        {
                            secDesc1 = HomeLoan.SecDescription1;
                            if ((HomeLoan.SecDescription2 + " " + HomeLoan.SecDescription3).Length > 25)
                            {
                                secDesc2 = HomeLoan.SecDescription2;
                                secDesc3 = HomeLoan.SecDescription3;
                            }
                            else
                            {
                                secDesc2 = HomeLoan.SecDescription2 + " " + HomeLoan.SecDescription3;
                            }
                        }
                        else
                        {
                            secDesc1 = HomeLoan.SecDescription1 + " " + HomeLoan.SecDescription2;
                            secDesc2 = HomeLoan.SecDescription3;
                        }

                        LoanDetailHtml.Replace("{{SecDescription1}}", secDesc1);
                        LoanDetailHtml.Replace("{{SecDescription2}}", secDesc2);
                        LoanDetailHtml.Replace("{{SecDescription3}}", secDesc3);

                        var res = 0.0m;
                        if (decimal.TryParse(HomeLoan.IntialDue, out res))
                        {
                            LoanDetailHtml.Replace("{{Instalment}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                        }
                        else
                        {
                            LoanDetailHtml.Replace("{{Instalment}}", "R0.00");
                        }

                        LoanDetailHtml.Replace("{{InterestRate}}", HomeLoan.ChargeRate + "% pa");

                        res = 0.0m;
                        if (decimal.TryParse(HomeLoan.ArrearStatus, out res))
                        {
                            LoanDetailHtml.Replace("{{Arrears}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                        }
                        else
                        {
                            LoanDetailHtml.Replace("{{Arrears}}", "R0.00");
                        }

                        res = 0.0m;
                        if (decimal.TryParse(HomeLoan.RegisteredAmount, out res))
                        {
                            LoanDetailHtml.Replace("{{RegisteredAmount}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                        }
                        else
                        {
                            LoanDetailHtml.Replace("{{RegisteredAmount}}", "R0.00");
                        }

                        LoanDetailHtml.Replace("{{LoanTerms}}", HomeLoan.LoanTerm);
                        TabContentHtml.Append(LoanDetailHtml.ToString());

                        var LoanTransactionRows = new StringBuilder();
                        var LoanTransactionDetailHtml = new StringBuilder(HtmlConstants.HOME_LOAN_TRANSACTION_DETAIL_DIV_HTML).Replace("style='max-height:200px;'", string.Empty);

                        var tr = new StringBuilder();
                        if (HomeLoan.LoanTransactions != null && HomeLoan.LoanTransactions.Count > 0)
                        {
                            HomeLoan.LoanTransactions.ForEach(trans =>
                            {
                                tr = new StringBuilder();
                                tr.Append("<tr class='ht-20'>");
                                tr.Append("<td class='w-13 text-center'> " + trans.Posting_Date.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy) + " </td>");
                                tr.Append("<td class='w-15 text-center'> " + trans.Effective_Date.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy) + " </td>");
                                tr.Append("<td class='w-35'> " + (!string.IsNullOrEmpty(trans.Description) ? trans.Description : ModelConstant.PAYMENT_THANK_YOU_TRANSACTION_DESC) + " </td>");

                                res = 0.0m;
                                if (decimal.TryParse(trans.Debit, out res))
                                {
                                    tr.Append("<td class='w-12 text-right'> " + (res > 0 ? utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res) : "-") + " </td>");
                                }
                                else
                                {
                                    tr.Append("<td class='w-12 text-right'> - </td>");
                                }

                                res = 0.0m;
                                if (decimal.TryParse(trans.Credit, out res))
                                {
                                    tr.Append("<td class='w-12 text-right'> " + (res > 0 ? utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res) : "-") + " </td>");
                                }
                                else
                                {
                                    tr.Append("<td class='w-12 text-right'> - </td>");
                                }

                                res = 0.0m;
                                if (decimal.TryParse(trans.RunningBalance, out res))
                                {
                                    tr.Append("<td class='w-13 text-right'> " + (res > 0 ? utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res) : "-") + " </td>");
                                }
                                else
                                {
                                    tr.Append("<td class='w-13 text-right'> - </td>");
                                }
                                tr.Append("</tr>");

                                LoanTransactionRows.Append(tr.ToString());
                            });
                        }

                        LoanTransactionDetailHtml.Replace("{{HomeLoanTransactionRow}}", LoanTransactionRows.ToString());
                        TabContentHtml.Append(LoanTransactionDetailHtml.ToString());

                        var LoanArrearHtml = new StringBuilder(HtmlConstants.HOME_LOAN_STATEMENT_OVERVIEW_AND_PAYMENT_DUE_DIV_HTML);
                        LoanArrearHtml.Replace("{{StatementDate}}", DateTime.Now.ToString(ModelConstant.DATE_FORMAT_yyyy_MM_dd));
                        res = 0.0m;
                        if (decimal.TryParse(HomeLoan.Balance, out res))
                        {
                            LoanArrearHtml.Replace("{{BalanceOutstanding}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                        }
                        else
                        {
                            LoanArrearHtml.Replace("{{BalanceOutstanding}}", "R0.00");
                        }

                        if (HomeLoan.LoanArrear != null)
                        {
                            var plArrears = HomeLoan.LoanArrear;
                            res = 0.0m;
                            if (decimal.TryParse(plArrears.ARREARS_120, out res))
                            {
                                LoanArrearHtml.Replace("{{After120Days}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                            }
                            else
                            {
                                LoanArrearHtml.Replace("{{After120Days}}", "R0.00");
                            }

                            res = 0.0m;
                            if (decimal.TryParse(plArrears.ARREARS_90, out res))
                            {
                                LoanArrearHtml.Replace("{{After90Days}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                            }
                            else
                            {
                                LoanArrearHtml.Replace("{{After90Days}}", "R0.00");
                            }

                            res = 0.0m;
                            if (decimal.TryParse(plArrears.ARREARS_60, out res))
                            {
                                LoanArrearHtml.Replace("{{After60Days}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                            }
                            else
                            {
                                LoanArrearHtml.Replace("{{After60Days}}", "R0.00");
                            }

                            res = 0.0m;
                            if (decimal.TryParse(plArrears.ARREARS_30, out res))
                            {
                                LoanArrearHtml.Replace("{{After30Days}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                            }
                            else
                            {
                                LoanArrearHtml.Replace("{{After30Days}}", "R0.00");
                            }

                            res = 0.0m;
                            if (decimal.TryParse(plArrears.CurrentDue, out res))
                            {
                                LoanArrearHtml.Replace("{{CurrentPaymentDue}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                            }
                            else
                            {
                                LoanArrearHtml.Replace("{{CurrentPaymentDue}}", "R0.00");
                            }
                        }
                        else
                        {
                            LoanArrearHtml.Replace("{{After120Days}}", "R0.00");
                            LoanArrearHtml.Replace("{{After90Days}}", "R0.00");
                            LoanArrearHtml.Replace("{{After60Days}}", "R0.00");
                            LoanArrearHtml.Replace("{{After30Days}}", "R0.00");
                            LoanArrearHtml.Replace("{{CurrentPaymentDue}}", "R0.00");
                        }
                        TabContentHtml.Append(LoanArrearHtml.ToString());

                        var PaymentDueMessageDivHtml = new StringBuilder(HtmlConstants.HOME_LOAN_PAYMENT_DUE_SPECIAL_MESSAGE_DIV_HTML);
                        var spjsonstr = HtmlConstants.HOME_LOAN_SPECIAL_MESSAGES_WIDGET_PREVIEW_JSON_STRING;
                        if (spjsonstr != string.Empty && validationEngine.IsValidJson(spjsonstr))
                        {
                            var SpecialMessage = JsonConvert.DeserializeObject<SpecialMessage>(spjsonstr);
                            if (SpecialMessage != null)
                            {
                                var PaymentDueMessage = (!string.IsNullOrEmpty(SpecialMessage.Message3) ? "<p> " + SpecialMessage.Message3 + " </p>" : string.Empty) + (!string.IsNullOrEmpty(SpecialMessage.Message4) ? "<p> " + SpecialMessage.Message4 + " </p>" : string.Empty) + (!string.IsNullOrEmpty(SpecialMessage.Message5) ? "<p> " + SpecialMessage.Message5 + " </p>" : string.Empty);

                                PaymentDueMessageDivHtml.Replace("{{PaymentDueSpecialMessage}}", PaymentDueMessage);
                                TabContentHtml.Append(PaymentDueMessageDivHtml.ToString());
                            }
                        }

                        //var LoanSummaryForTaxPurposesHtml = new StringBuilder(HtmlConstants.HOME_LOAN_SERVICE_FOR_TAX_PURPOSES_DIV_HTML);
                        //var LoanInstalmentHtml = new StringBuilder(HtmlConstants.HOME_LOAN_INSTALMENT_DETAILS_DIV_HTML);
                        //var HomeLoanSummary = HomeLoan.LoanSummary;
                        //if (HomeLoanSummary != null)
                        //{
                        //    #region Summary for Tax purposes div
                        //    res = 0.0m;
                        //    if (!string.IsNullOrEmpty(HomeLoanSummary.Annual_Interest) && decimal.TryParse(HomeLoanSummary.Annual_Interest, out res))
                        //    {
                        //        LoanSummaryForTaxPurposesHtml.Replace("{{AnnualInterest}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                        //    }
                        //    else
                        //    {
                        //        LoanSummaryForTaxPurposesHtml.Replace("{{AnnualInterest}}", "R0.00");
                        //    }

                        //    res = 0.0m;
                        //    if (!string.IsNullOrEmpty(HomeLoanSummary.Annual_Insurance) && decimal.TryParse(HomeLoanSummary.Annual_Insurance, out res))
                        //    {
                        //        LoanSummaryForTaxPurposesHtml.Replace("{{AnnualInsurance}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                        //    }
                        //    else
                        //    {
                        //        LoanSummaryForTaxPurposesHtml.Replace("{{AnnualInsurance}}", "R0.00");
                        //    }

                        //    res = 0.0m;
                        //    if (!string.IsNullOrEmpty(HomeLoanSummary.Annual_Service_Fee) && decimal.TryParse(HomeLoanSummary.Annual_Service_Fee, out res))
                        //    {
                        //        LoanSummaryForTaxPurposesHtml.Replace("{{AnnualServiceFee}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                        //    }
                        //    else
                        //    {
                        //        LoanSummaryForTaxPurposesHtml.Replace("{{AnnualServiceFee}}", "R0.00");
                        //    }

                        //    res = 0.0m;
                        //    if (!string.IsNullOrEmpty(HomeLoanSummary.Annual_Legal_Costs) && decimal.TryParse(HomeLoanSummary.Annual_Legal_Costs, out res))
                        //    {
                        //        LoanSummaryForTaxPurposesHtml.Replace("{{AnnualLegalCosts}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                        //    }
                        //    else
                        //    {
                        //        LoanSummaryForTaxPurposesHtml.Replace("{{AnnualLegalCosts}}", "R0.00");
                        //    }

                        //    res = 0.0m;
                        //    if (!string.IsNullOrEmpty(HomeLoanSummary.Annual_Total_Recvd) && decimal.TryParse(HomeLoanSummary.Annual_Total_Recvd, out res))
                        //    {
                        //        LoanSummaryForTaxPurposesHtml.Replace("{{AnnualTotalAmountReceived}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                        //    }
                        //    else
                        //    {
                        //        LoanSummaryForTaxPurposesHtml.Replace("{{AnnualTotalAmountReceived}}", "R0.00");
                        //    }

                        //    #endregion

                        //    #region Installment details div

                        //    res = 0.0m;
                        //    if (!string.IsNullOrEmpty(HomeLoanSummary.Basic_Instalment) && decimal.TryParse(HomeLoanSummary.Basic_Instalment, out res))
                        //    {
                        //        LoanInstalmentHtml.Replace("{{BasicInstalment}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                        //    }
                        //    else
                        //    {
                        //        LoanInstalmentHtml.Replace("{{BasicInstalment}}", "R0.00");
                        //    }

                        //    res = 0.0m;
                        //    if (!string.IsNullOrEmpty(HomeLoanSummary.Houseowner_Ins) && decimal.TryParse(HomeLoanSummary.Houseowner_Ins, out res))
                        //    {
                        //        LoanInstalmentHtml.Replace("{{HouseownerInsurance}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                        //    }
                        //    else
                        //    {
                        //        LoanInstalmentHtml.Replace("{{HouseownerInsurance}}", "R0.00");
                        //    }

                        //    res = 0.0m;
                        //    if (!string.IsNullOrEmpty(HomeLoanSummary.Loan_Protection) && decimal.TryParse(HomeLoanSummary.Loan_Protection, out res))
                        //    {
                        //        LoanInstalmentHtml.Replace("{{LoanProtectionAssurance}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                        //    }
                        //    else
                        //    {
                        //        LoanInstalmentHtml.Replace("{{LoanProtectionAssurance}}", "R0.00");
                        //    }

                        //    res = 0.0m;
                        //    if (!string.IsNullOrEmpty(HomeLoanSummary.Recovery_Fee_Debit) && decimal.TryParse(HomeLoanSummary.Recovery_Fee_Debit, out res))
                        //    {
                        //        LoanInstalmentHtml.Replace("{{RecoveryOfFeeDebits}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                        //    }
                        //    else
                        //    {
                        //        LoanInstalmentHtml.Replace("{{RecoveryOfFeeDebits}}", "R0.00");
                        //    }

                        //    res = 0.0m;
                        //    if (!string.IsNullOrEmpty(HomeLoanSummary.Capital_Redemption) && decimal.TryParse(HomeLoanSummary.Capital_Redemption, out res))
                        //    {
                        //        LoanInstalmentHtml.Replace("{{CapitalRedemption}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                        //    }
                        //    else
                        //    {
                        //        LoanInstalmentHtml.Replace("{{CapitalRedemption}}", "R0.00");
                        //    }

                        //    res = 0.0m;
                        //    if (!string.IsNullOrEmpty(HomeLoanSummary.Service_Fee) && decimal.TryParse(HomeLoanSummary.Service_Fee, out res))
                        //    {
                        //        LoanInstalmentHtml.Replace("{{ServiceFee}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                        //    }
                        //    else
                        //    {
                        //        LoanInstalmentHtml.Replace("{{ServiceFee}}", "R0.00");
                        //    }

                        //    res = 0.0m;
                        //    if (!string.IsNullOrEmpty(HomeLoanSummary.Total_Instalment) && decimal.TryParse(HomeLoanSummary.Total_Instalment, out res))
                        //    {
                        //        LoanInstalmentHtml.Replace("{{TotalInstalment}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                        //    }
                        //    else
                        //    {
                        //        LoanInstalmentHtml.Replace("{{TotalInstalment}}", "R0.00");
                        //    }

                        //    LoanInstalmentHtml.Replace("{{InstalmentDate}}", DateTime.Now.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy));

                        //    #endregion
                        //}
                        //else
                        //{
                        //    LoanSummaryForTaxPurposesHtml.Replace("{{AnnualInterest}}", "R0.00");
                        //    LoanSummaryForTaxPurposesHtml.Replace("{{AnnualInsurance}}", "R0.00");
                        //    LoanSummaryForTaxPurposesHtml.Replace("{{AnnualServiceFee}}", "R0.00");
                        //    LoanSummaryForTaxPurposesHtml.Replace("{{AnnualLegalCosts}}", "R0.00");
                        //    LoanSummaryForTaxPurposesHtml.Replace("{{AnnualTotalAmountReceived}}", "R0.00");

                        //    LoanInstalmentHtml.Replace("{{BasicInstalment}}", "R0.00");
                        //    LoanInstalmentHtml.Replace("{{HouseownerInsurance}}", "R0.00");
                        //    LoanInstalmentHtml.Replace("{{LoanProtectionAssurance}}", "R0.00");
                        //    LoanInstalmentHtml.Replace("{{RecoveryOfFeeDebits}}", "R0.00");
                        //    LoanInstalmentHtml.Replace("{{CapitalRedemption}}", "R0.00");
                        //    LoanInstalmentHtml.Replace("{{ServiceFee}}", "R0.00");
                        //    LoanInstalmentHtml.Replace("{{TotalInstalment}}", "R0.00");
                        //    LoanInstalmentHtml.Replace("{{InstalmentDate}}", DateTime.Now.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy));
                        //}

                        //TabContentHtml.Append(LoanSummaryForTaxPurposesHtml.ToString());
                        //TabContentHtml.Append(LoanInstalmentHtml.ToString());

                        TabContentHtml.Append(HtmlConstants.END_DIV_TAG);
                        counter++;
                    });

                    TabContentHtml.Append((HomeLoans.Count > 1) ? HtmlConstants.END_DIV_TAG : string.Empty);
                    pageContent.Replace("{{TabContentsDiv_" + page.Identifier + "_" + widget.Identifier + "}}", TabContentHtml.ToString());
                }
                else
                {
                    pageContent.Replace("{{NavTab_" + page.Identifier + "_" + widget.Identifier + "}}", string.Empty);
                    pageContent.Replace("{{TabContentsDiv_" + page.Identifier + "_" + widget.Identifier + "}}", string.Empty);
                }
            }
            catch
            {
            }
        }

        private void BindPortfolioCustomerDetailsWidgetData(StringBuilder pageContent, DM_CustomerMaster customer, Page page, PageWidget widget)
        {
            pageContent.Replace("{{CustomerName_" + page.Identifier + "_" + widget.Identifier + "}}", (customer.Title + " " + customer.FirstName + " " + customer.SurName));
            pageContent.Replace("{{CustomerId_" + page.Identifier + "_" + widget.Identifier + "}}", customer.CustomerId.ToString());
            pageContent.Replace("{{MobileNumber_" + page.Identifier + "_" + widget.Identifier + "}}", customer.Mask_Cell_No);
            pageContent.Replace("{{EmailAddress_" + page.Identifier + "_" + widget.Identifier + "}}", customer.EmailAddress);
        }

        private void BindPortfolioCustomerAddressDetailsWidgetData(StringBuilder pageContent, DM_CustomerMaster customer, Page page, PageWidget widget)
        {
            var custAddress = (!string.IsNullOrEmpty(customer.AddressLine0) ? (customer.AddressLine0 + "<br>") : string.Empty) +
                                (!string.IsNullOrEmpty(customer.AddressLine1) ? (customer.AddressLine1 + "<br>") : string.Empty) +
                                (!string.IsNullOrEmpty(customer.AddressLine2) ? (customer.AddressLine2 + "<br>") : string.Empty) +
                                (!string.IsNullOrEmpty(customer.AddressLine3) ? (customer.AddressLine3 + "<br>") : string.Empty) +
                                (!string.IsNullOrEmpty(customer.AddressLine4) ? customer.AddressLine4 : string.Empty);
            pageContent.Replace("{{CustomerAddress_" + page.Identifier + "_" + widget.Identifier + "}}", custAddress);
        }

        private void BindPortfolioClientContactDetailsWidgetData(StringBuilder pageContent, Page page, PageWidget widget)
        {
            pageContent.Replace("{{MobileNumber_" + page.Identifier + "_" + widget.Identifier + "}}", "0860 555 111");
            pageContent.Replace("{{EmailAddress_" + page.Identifier + "_" + widget.Identifier + "}}", "supportdesk@nedbank.com");
        }

        private void BindPortfolioAccountSummaryWidgetData(StringBuilder pageContent, List<DM_AccountsSummary> _AccountsSummaries, Page page, PageWidget widget)
        {
            if (_AccountsSummaries.Count > 0)
            {
                var res = 0.0m;
                var accountSummaryRows = new StringBuilder();
                _AccountsSummaries.ForEach(acc =>
                {
                    if (!acc.AccountType.ToLower().Contains("reward") || !acc.AccountType.ToLower().Contains("point"))
                    {
                        var tr = new StringBuilder();
                        tr.Append("<tr class='ht-30'>");
                        tr.Append("<td class='text-left'>" + acc.AccountType + " </td>");
                        tr.Append("<td class='text-right'>" + utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, (decimal.TryParse(acc.TotalAmount, out res) ? res : 0)) + " </td>");
                        tr.Append("</tr>");
                        accountSummaryRows.Append(tr.ToString());
                    }
                });
                pageContent.Replace("{{AccountSummaryRows_" + page.Identifier + "_" + widget.Identifier + "}}", accountSummaryRows.ToString());
            }
            else
            {
                pageContent.Replace("{{AccountSummaryRows_" + page.Identifier + "_" + widget.Identifier + "}}", "<tr class='ht-30'><td class='text-center' colspan='2'>No records found</td></tr>");
            }

            //To add reward points data
            var accSummary = _AccountsSummaries.Where(it => it.AccountType.ToLower().Contains("reward") || it.AccountType.ToLower().Contains("point"))?.ToList()?.FirstOrDefault();
            var rewardPointsDiv = new StringBuilder();
            if (accSummary != null)
            {
                rewardPointsDiv = new StringBuilder("<div class='pt-2'><table class='LoanTransactionTable customTable'><thead><tr class='ht-30'><th class='text-left'>" + accSummary.AccountType + " </th><th class='text-right'> " + accSummary.TotalAmount + " </th></tr></thead></table></div>");
            }
            pageContent.Replace("{{RewardPointsDiv_" + page.Identifier + "_" + widget.Identifier + "}}", rewardPointsDiv.ToString());
        }

        private void BindPortfolioAccountAnalysisWidgetData(StringBuilder pageContent, StringBuilder scriptHtmlRenderer, List<DM_AccountAnanlysis> _lstAccountAnalysis, Page page, PageWidget widget)
        {
            var data = "[]";
            if (_lstAccountAnalysis != null && _lstAccountAnalysis.Count > 0)
            {
                data = JsonConvert.SerializeObject(_lstAccountAnalysis);
            }
            pageContent.Replace("HiddenAccountAnalysisGraphValue_" + page.Identifier + "_" + widget.Identifier + "", data);
            scriptHtmlRenderer.Append(HtmlConstants.PORTFOLIO_ACCOUNT_ANALYSIS_BAR_GRAPH_SCRIPT.Replace("AccountAnalysisBarGraphcontainer", "AccountAnalysisBarGraphcontainer_" + page.Identifier + "_" + widget.Identifier).Replace("HiddenAccountAnalysisGraph", "HiddenAccountAnalysisGraph_" + page.Identifier + "_" + widget.Identifier));
        }

        private void BindPortfolioRemindersWidgetData(StringBuilder pageContent, List<DM_CustomerReminderAndRecommendation> Reminders, Page page, PageWidget widget)
        {
            StringBuilder reminderstr = new StringBuilder();
            if (Reminders != null && Reminders.Count > 0)
            {
                Reminders.ToList().ForEach(item =>
                {
                    if (!string.IsNullOrEmpty(item.reminderAndRecommendation.Description))
                    {
                        reminderstr.Append("<div class='row'><div class='col-lg-9 text-left'><p class='p-1' style='background-color: #dce3dc;'>" + item.reminderAndRecommendation.Description + " </p></div><div class='col-lg-3 text-left'><a href='" + item.reminderAndRecommendation.ActionUrl + "' target='_blank'><i class='fa fa-caret-left fa-3x float-left text-success'></i><span class='mt-2 d-inline-block ml-2'>" + item.reminderAndRecommendation.ActionTitle + "</span></a></div></div>");
                    }
                });
            }
            pageContent.Replace("{{ReminderAndRecommendation_" + page.Identifier + "_" + widget.Identifier + "}}", reminderstr.ToString());
        }

        private void BindPortfolioNewsAlertsWidgetData(StringBuilder pageContent, List<DM_CustomerNewsAndAlert> NewsAndAlerts, Page page, PageWidget widget)
        {
            var newsAlertStr = new StringBuilder();
            if (NewsAndAlerts != null && NewsAndAlerts.Count > 0)
            {
                NewsAndAlerts.ForEach(item =>
                {
                    if (!string.IsNullOrEmpty(item.NewsAndAlert.Description))
                    {
                        newsAlertStr.Append("<p>" + item.NewsAndAlert.Description + "</p>");
                    }
                });
            }
            pageContent.Replace("{{NewsAlert_" + page.Identifier + "_" + widget.Identifier + "}}", newsAlertStr.ToString());
        }

        private void BindGreenbacksTotalRewardPointsWidgetData(StringBuilder pageContent, List<DM_AccountsSummary> _AccountsSummaries, Page page, PageWidget widget)
        {
            var accSummary = _AccountsSummaries.Where(it => it.AccountType.ToLower().Contains("reward") || it.AccountType.ToLower().Contains("point"))?.ToList()?.FirstOrDefault();
            pageContent.Replace("{{TotalRewardsPoints_" + page.Identifier + "_" + widget.Identifier + "}}", (accSummary != null ? accSummary.TotalAmount : "0"));
        }

        private void BindGreenbacksYtdRewardsPointsGraphWidgetData(StringBuilder pageContent, StringBuilder scriptHtmlRenderer, List<DM_GreenbacksRewardPoints> RewardPoints, Page page, PageWidget widget)
        {
            var data = "[]";
            if (RewardPoints != null && RewardPoints.Count > 0)
            {
                data = JsonConvert.SerializeObject(RewardPoints);
            }
            pageContent.Replace("HiddenYTDRewardPointsGraphValue_" + page.Identifier + "_" + widget.Identifier + "", data);
            scriptHtmlRenderer.Append(HtmlConstants.GREENBACKS_YTD_REWARDS_POINTS_BAR_GRAPH_SCRIPT.Replace("YTDRewardPointsBarGraphcontainer", "YTDRewardPointsBarGraphcontainer_" + page.Identifier + "_" + widget.Identifier).Replace("HiddenYTDRewardPointsGraph", "HiddenYTDRewardPointsGraph_" + page.Identifier + "_" + widget.Identifier));
        }

        private void BindGreenbacksPointsRedeemedYtdGraphWidgetData(StringBuilder pageContent, StringBuilder scriptHtmlRenderer, List<DM_GreenbacksRewardPointsRedeemed> rewardPointsRedeemeds, Page page, PageWidget widget)
        {
            var data = "[]";
            if (rewardPointsRedeemeds != null && rewardPointsRedeemeds.Count > 0)
            {
                data = JsonConvert.SerializeObject(rewardPointsRedeemeds);
            }
            pageContent.Replace("HiddenPointsRedeemedGraphValue_" + page.Identifier + "_" + widget.Identifier + "", data);
            scriptHtmlRenderer.Append(HtmlConstants.GREENBACKS_POINTS_REDEEMED_YTD_BAR_GRAPH_SCRIPT.Replace("PointsRedeemedYTDBarGraphcontainer", "PointsRedeemedYTDBarGraphcontainer_" + page.Identifier + "_" + widget.Identifier).Replace("HiddenPointsRedeemedGraph", "HiddenPointsRedeemedGraph_" + page.Identifier + "_" + widget.Identifier));
        }

        private void BindGreenbacksProductRelatedPonitsEarnedGraphWidgetData(StringBuilder pageContent, StringBuilder scriptHtmlRenderer, List<DM_CustomerProductWiseRewardPoints> productWiseRewardPoints, Page page, PageWidget widget)
        {
            var data = "[]";
            if (productWiseRewardPoints != null && productWiseRewardPoints.Count > 0)
            {
                data = JsonConvert.SerializeObject(productWiseRewardPoints);
            }
            pageContent.Replace("HiddenProductRelatedPointsEarnedGraphValue_" + page.Identifier + "_" + widget.Identifier + "", data);
            scriptHtmlRenderer.Append(HtmlConstants.GREENBACKS_PRODUCT_RELATED_POINTS_EARNED_BAR_GRAPH_SCRIPT.Replace("ProductRelatedPointsEarnedBarGraphcontainer", "ProductRelatedPointsEarnedBarGraphcontainer_" + page.Identifier + "_" + widget.Identifier).Replace("HiddenProductRelatedPointsEarnedGraph", "HiddenProductRelatedPointsEarnedGraph_" + page.Identifier + "_" + widget.Identifier));
        }

        private void BindGreenbacksCategorySpendRewardPointsGraphWidgetData(StringBuilder pageContent, StringBuilder scriptHtmlRenderer, List<DM_CustomerRewardSpendByCategory> rewardSpendByCategories, Page page, PageWidget widget)
        {
            var data = "[]";
            if (rewardSpendByCategories != null && rewardSpendByCategories.Count > 0)
            {
                data = JsonConvert.SerializeObject(rewardSpendByCategories);
            }
            pageContent.Replace("HiddenCategorySpendRewardsGraphValue_" + page.Identifier + "_" + widget.Identifier + "", data);
            scriptHtmlRenderer.Append(HtmlConstants.GREENBACKS_CATEGORY_SPEND_REWARD_POINTS_BAR_GRAPH_SCRIPT.Replace("CategorySpendRewardsPieChartcontainer", "CategorySpendRewardsPieChartcontainer_" + page.Identifier + "_" + widget.Identifier).Replace("HiddenCategorySpendRewardsGraph", "HiddenCategorySpendRewardsGraph_" + page.Identifier + "_" + widget.Identifier));
        }

        #endregion

        /// <summary>
        /// This method help to actaul color theme to show series data for dynamic line graph, bar graph, and pie chart widgets
        /// </summary>
        /// <param name="theme"> the widget theme </param>
        /// <returns>return new color theme for graph and chart widgets </returns>
        private string GetChartColorTheme(string theme)
        {
            string colorTheme = string.Empty;
            if (theme == "Theme1")
            {
                colorTheme = HtmlConstants.THEME1;
            }
            else if (theme == "Theme2")
            {
                colorTheme = HtmlConstants.THEME2;
            }
            else if (theme == "Theme3")
            {
                colorTheme = HtmlConstants.THEME3;
            }
            else if (theme == "Theme4")
            {
                colorTheme = HtmlConstants.THEME4;
            }
            else if (theme.ToLower() == "ChartTheme1".ToLower())
            {
                colorTheme = HtmlConstants.THEME1;
            }
            else if (theme.ToLower() == "ChartTheme2".ToLower())
            {
                colorTheme = HtmlConstants.THEME2;
            }
            else if (theme.ToLower() == "ChartTheme3".ToLower())
            {
                colorTheme = HtmlConstants.THEME4;
            }
            else if (theme.ToLower() == "ChartTheme4".ToLower())
            {
                colorTheme = HtmlConstants.THEME3;
            }

            return colorTheme;
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
