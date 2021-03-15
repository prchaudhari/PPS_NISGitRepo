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
        public string GenerateStatementNew(long identifier, string tenantCode)
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

                var _lstStatementSearch = this.StatementSearchRepository.GetStatementSearchs(new StatementSearchSearchParameter()
                {
                    Identifier = identifier.ToString(),
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
                }, tenantCode);
                if (_lstStatementSearch.Count > 0)
                {
                    var statementSearch = _lstStatementSearch.FirstOrDefault();

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
                    navbarHtml = navbarHtml.Replace("{{Today}}", DateTime.UtcNow.ToString("dd MMM yyyy"));
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
                                        "-" + lastFourDigisOfAccountNumber + "' class='tab-pane fade in active show'");

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
                    var customerMedias = this.tenantTransactionDataManager.GetCustomerMediaList(customer.Identifier, batchMaster.Identifier, statement.Identifier, tenantCode);

                    //get investment master data
                    var investmentMasters = this.tenantTransactionDataManager.Get_DM_InvestmasterMaster(new CustomerInvestmentSearchParameter() { CustomerId = customer.CustomerId, BatchId = batchMaster.Identifier }, tenantCode)?.ToList();
                    long ParentId = 0;
                    if (investmentMasters != null && investmentMasters.Count > 0)
                    {
                        investmentMasters.ForEach(invest =>
                        {
                            invest.investmentTransactions = this.tenantTransactionDataManager.Get_DM_InvestmentTransaction(new CustomerInvestmentSearchParameter() { CustomerId = customer.CustomerId, BatchId = batchMaster.Identifier, InvestmentId = invest.InvestmentId }, tenantCode)?.ToList();
                        });
                        ParentId = investmentMasters[0].InvestorId;
                    }

                    var ExplanatoryNotes = this.tenantTransactionDataManager.Get_DM_ExplanatoryNotes(new MessageAndNoteSearchParameter() { BatchId = batchMaster.Identifier }, tenantCode)?.ToList();
                    var Messages = this.tenantTransactionDataManager.Get_DM_MarketingMessages(new MessageAndNoteSearchParameter() { BatchId = batchMaster.Identifier }, tenantCode)?.ToList();

                    var htmlbody = new StringBuilder();
                    htmlbody.Append(HtmlConstants.CONTAINER_DIV_HTML_HEADER);

                    //start to render actual html content data
                    StringBuilder scriptHtmlRenderer = new StringBuilder();
                    int subPageCount = 0;
                    string accountNumber = string.Empty;
                    string accountType = string.Empty;
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
                        var MarketingMessageCounter = 0;
                        var statementPageContent = newStatementPageContents.Where(item => item.PageTypeId == page.PageTypeId && item.Id == i).FirstOrDefault();

                        subPageCount = 1;
                        StringBuilder SubTabs = new StringBuilder();
                        StringBuilder PageHeaderContent = new StringBuilder(statementPageContent.PageHeaderContent);
                        var dynamicWidgets = new List<DynamicWidget>(statementPageContent.DynamicWidgets);

                        string tabClassName = Regex.Replace((statementPageContent.DisplayName + "-" + page.Identifier), @"\s+", "-");
                        PageHeaderContent.Replace("{{ExtraClass}}", tabClassName).Replace("{{DivId}}", tabClassName);

                        StringBuilder newPageContent = new StringBuilder();
                        newPageContent.Append(HtmlConstants.PAGE_TAB_CONTENT_HEADER);

                        for (int x = 0; x < subPageCount; x++)
                        {
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
                                            this.BindCustomerDetailsWidgetData(pageContent, customer, page, widget);
                                            break;
                                        case HtmlConstants.BANK_DETAILS_WIDGET_NAME:
                                            this.BindBranchDetailsWidgetData(pageContent, investmentMasters[0].BranchId, page, widget, tenantCode);
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
                                            this.BindExplanatoryNotesWidgetData(pageContent, ExplanatoryNotes, page, widget);
                                            break;
                                        case HtmlConstants.SERVICE_WIDGET_NAME:
                                            this.BindMarketingServiceWidgetData(pageContent, Messages, page, widget, MarketingMessageCounter);
                                            MarketingMessageCounter++;
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

                    StringBuilder finalHtml = new StringBuilder();
                    finalHtml.Append(HtmlConstants.HTML_HEADER);
                    finalHtml.Append(htmlbody.ToString());
                    finalHtml.Append(HtmlConstants.HTML_FOOTER);
                    scriptHtmlRenderer.Append(HtmlConstants.TENANT_LOGO_SCRIPT);

                    finalHtml.Replace("{{ChartScripts}}", scriptHtmlRenderer.ToString());
                    finalHtml.Replace("{{CustomerNumber}}", customer.Identifier.ToString());
                    finalHtml.Replace("{{StatementNumber}}", statement.Identifier.ToString());
                    finalHtml.Replace("{{FirstPageId}}", FirstPageId.ToString());
                    finalHtml.Replace("<link rel='stylesheet' href='../common/css/site.css'><link rel='stylesheet' href='../common/css/ltr.css'>", "");
                    finalHtml.Replace("../common/css/", "./").Replace("../common/js/", "./");
                    finalHtml.Replace("../common/css/", "./");

                    string fileName = "Statement_" + customer.Identifier + "_" + statement.Identifier + "_" + DateTime.Now.ToString().Replace("-", "_").Replace(":", "_").Replace(" ", "_").Replace('/', '_') + ".html";
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
                        var imagePath = outputLocation + "\\Statements\\" + batchMaster.Identifier + "\\" + customerId;
                        if (File.Exists(path) && !File.Exists(imagePath + "\\" + fileName))
                        {
                            File.Copy(path, Path.Combine(imagePath, fileName));
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
                        var videoPath = outputLocation + "\\Statements\\" + batchMaster.Identifier + "\\" + customerId;
                        if (File.Exists(path) && !File.Exists(videoPath + "\\" + fileName))
                        {
                            File.Copy(path, Path.Combine(videoPath, fileName));
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
            pageContent.Replace("{{Title_" + page.Identifier + "_" + widget.Identifier + "}}", customer.Title);
            pageContent.Replace("{{FirstName_" + page.Identifier + "_" + widget.Identifier + "}}", customer.FirstName);
            pageContent.Replace("{{Surname_" + page.Identifier + "_" + widget.Identifier + "}}", customer.SurName);
            pageContent.Replace("{{CustAddressLine0_" + page.Identifier + "_" + widget.Identifier + "}}", customer.AddressLine0);
            pageContent.Replace("{{CustAddressLine1_" + page.Identifier + "_" + widget.Identifier + "}}", customer.AddressLine1);
            pageContent.Replace("{{CustAddressLine2_" + page.Identifier + "_" + widget.Identifier + "}}", customer.AddressLine2);
            pageContent.Replace("{{CustAddressLine3_" + page.Identifier + "_" + widget.Identifier + "}}", customer.AddressLine3);
            pageContent.Replace("{{CustAddressLine4_" + page.Identifier + "_" + widget.Identifier + "}}", customer.AddressLine4);
        }

        private void BindBranchDetailsWidgetData(StringBuilder pageContent, long BranchId, Page page, PageWidget widget, string tenantCode)
        {
            var branchDetails = this.tenantTransactionDataManager.Get_DM_BranchMaster(BranchId, tenantCode)?.FirstOrDefault();
            if (branchDetails != null)
            {
                pageContent.Replace("{{BankName_" + page.Identifier + "_" + widget.Identifier + "}}", branchDetails.BranchName);
                pageContent.Replace("{{AddressLine1_" + page.Identifier + "_" + widget.Identifier + "}}", branchDetails.AddressLine0);
                pageContent.Replace("{{AddressLine2_" + page.Identifier + "_" + widget.Identifier + "}}", branchDetails.AddressLine1);
                pageContent.Replace("{{CountryName_" + page.Identifier + "_" + widget.Identifier + "}}", branchDetails.AddressLine2);
                pageContent.Replace("{{BankVATRegNo_" + page.Identifier + "_" + widget.Identifier + "}}", "Bank VAT Reg No " + branchDetails.VatRegNo);
                pageContent.Replace("{{ContactCenter_" + page.Identifier + "_" + widget.Identifier + "}}", "Contact centre: " + branchDetails.ContactNo);
            }

        }

        private void BindInvestmentPortfolioStatementWidgetData(StringBuilder pageContent, DM_CustomerMaster customer, List<DM_InvestmentMaster> investmentMasters, Page page, PageWidget widget)
        {
            if (investmentMasters != null && investmentMasters.Count > 0)
            {
                var currency = investmentMasters[0].Currenacy;
                var TotalClosingBalance = 0.0;
                investmentMasters.ForEach(invest =>
                {
                    TotalClosingBalance = TotalClosingBalance + invest.investmentTransactions.Where(it => it.TransactionDesc.ToLower().Contains("balance carried forward")).Select(it => it.WJXBFS4_Balance).ToList().Sum(it => Convert.ToDouble(it));
                });

                pageContent.Replace("{{DSName_" + page.Identifier + "_" + widget.Identifier + "}}", customer.DS_Investor_Name);
                pageContent.Replace("{{TotalClosingBalance_" + page.Identifier + "_" + widget.Identifier + "}}", currency + TotalClosingBalance);
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
                var currency = investmentMasters[0].Currenacy;
                var TotalClosingBalance = 0.0;
                var TotalOpeningBalance = 0.0;
                investmentMasters.ForEach(invest =>
                {
                    TotalClosingBalance = TotalClosingBalance + invest.investmentTransactions.Where(it => it.TransactionDesc.ToLower().Contains("balance carried forward")).Select(it => it.WJXBFS4_Balance).ToList().Sum(it => Convert.ToDouble(it));
                    TotalOpeningBalance = TotalClosingBalance + invest.investmentTransactions.Where(it => it.TransactionDesc.ToLower().Contains("balance brought forward")).Select(it => it.WJXBFS4_Balance).ToList().Sum(it => Convert.ToDouble(it));
                });

                pageContent.Replace("{{ProductType_" + page.Identifier + "_" + widget.Identifier + "}}", investmentMasters[0].ProductType);
                pageContent.Replace("{{OpeningBalanceAmount_" + page.Identifier + "_" + widget.Identifier + "}}", currency + Convert.ToString(TotalOpeningBalance));
                pageContent.Replace("{{ClosingBalanceAmount_" + page.Identifier + "_" + widget.Identifier + "}}", currency + Convert.ToString(TotalClosingBalance));
            }
        }

        private void BindBreakdownOfInvestmentAccountsWidgetData(StringBuilder pageContent, List<DM_InvestmentMaster> investmentMasters, Page page, PageWidget widget)
        {
            if (investmentMasters != null && investmentMasters.Count > 0)
            {
                //Create Nav tab if investment accounts is more than 1
                var NavTabs = new StringBuilder();
                var InvestmentAccountsCount = investmentMasters.Count;
                if (InvestmentAccountsCount > 1)
                {
                    NavTabs.Append("<ul class='nav nav-tabs Investment-nav-tabs'>");
                    var cnt = 0;
                    investmentMasters.ToList().ForEach(acc =>
                    {
                        NavTabs.Append("<li class='nav-item " + (cnt == 0 ? "active" : string.Empty) + "'><a id='tab0-tab' data-toggle='tab' data-target='#" + acc.ProductDesc + "-" + acc.InvestmentId + "' role='tab' class='nav-link " + (cnt == 0 ? "active" : string.Empty) + "'> " + acc.ProductDesc + " - " + acc.InvestmentId + "</a></li>");
                        cnt++;
                    });
                    NavTabs.Append("</ul>");
                }
                pageContent.Replace("{{NavTab_" + page.Identifier + "_" + widget.Identifier + "}}", NavTabs.ToString());

                //create tab-content div if accounts is greater than 1, otherwise create simple div
                var TabContentHtml = new StringBuilder();
                var counter = 0;
                TabContentHtml.Append((InvestmentAccountsCount > 1) ? "<div class='tab-content'>" : string.Empty);
                investmentMasters.ToList().ForEach(acc =>
                {
                    var InvestmentAccountDetailHtml = new StringBuilder(HtmlConstants.INVESTMENT_ACCOUNT_DETAILS_HTML);
                    InvestmentAccountDetailHtml.Replace("{{ProductDesc}}", acc.ProductDesc);
                    InvestmentAccountDetailHtml.Replace("{{InvestmentId}}", Convert.ToString(acc.InvestmentId));
                    InvestmentAccountDetailHtml.Replace("{{TabPaneClass}}", "tab-pane fade " + (counter == 0 ? "in active show" : string.Empty));

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

                    var LastInvestmentTransaction = acc.investmentTransactions.Where(it => it.TransactionDesc.ToLower().Contains("balance carried forward")).OrderByDescending(it => it.TransactionDate).ToList().FirstOrDefault();
                    InvestmentAccountDetailHtml.Replace("{{LastTransactionDate}}", LastInvestmentTransaction.TransactionDate.ToString("dd MMMM yyyy"));
                    InvestmentAccountDetailHtml.Replace("{{BalanceOfLastTransactionDate}}", acc.Currenacy + LastInvestmentTransaction.WJXBFS4_Balance);

                    var InvestmentTransactionRows = new StringBuilder();
                    acc.investmentTransactions.OrderBy(it => it.TransactionDate).ToList().ForEach(trans =>
                    {
                        var tr = new StringBuilder();
                        tr.Append("<tr class='ht-20'>");
                        tr.Append("<td class='w-15 pt-1'>" + trans.TransactionDate.ToString("dd'/'MM'/'yyyy") + "</td>");
                        tr.Append("<td class='w-40 pt-1'>" + trans.TransactionDesc + "</td>");
                        tr.Append("<td class='w-15 text-right pt-1'>" + (trans.WJXBFS2_Debit == "0" ? "-" : acc.Currenacy + trans.WJXBFS2_Debit) + "</td>");
                        tr.Append("<td class='w-15 text-right pt-1'>" + (trans.WJXBFS3_Credit == "0" ? "-" : acc.Currenacy + trans.WJXBFS3_Credit) + "</td>");
                        tr.Append("<td class='w-15 text-right pt-1'>" + (trans.WJXBFS4_Balance == "0" ? "-" : acc.Currenacy + trans.WJXBFS4_Balance) + "</td>");
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

        private void BindExplanatoryNotesWidgetData(StringBuilder pageContent, List<DM_ExplanatoryNote> ExPlanatoryNotes, Page page, PageWidget widget)
        {
            if (ExPlanatoryNotes != null && ExPlanatoryNotes.Count > 0)
            {
                var notes = new StringBuilder();
                notes.Append(string.IsNullOrEmpty(ExPlanatoryNotes[0].Note1) ? string.Empty : "<span> " + Convert.ToString(ExPlanatoryNotes[0].Note1) + " </span> <br/>");
                notes.Append(string.IsNullOrEmpty(ExPlanatoryNotes[0].Note2) ? string.Empty : "<span> " + Convert.ToString(ExPlanatoryNotes[0].Note2) + " </span> <br/>");
                notes.Append(string.IsNullOrEmpty(ExPlanatoryNotes[0].Note3) ? string.Empty : "<span> " + Convert.ToString(ExPlanatoryNotes[0].Note3) + " </span> <br>");
                notes.Append(string.IsNullOrEmpty(ExPlanatoryNotes[0].Note4) ? string.Empty : "<span> " + Convert.ToString(ExPlanatoryNotes[0].Note4) + " </span> <br>");
                notes.Append(string.IsNullOrEmpty(ExPlanatoryNotes[0].Note5) ? string.Empty : "<span> " + Convert.ToString(ExPlanatoryNotes[0].Note5) + " </span> <br>");
                pageContent.Replace("{{Notes_" + page.Identifier + "_" + widget.Identifier + "}}", notes.ToString());
            }
        }

        private void BindMarketingServiceWidgetData(StringBuilder pageContent, List<DM_MarketingMessage> Messages, Page page, PageWidget widget, int MarketingMessageCounter)
        {
            if (Messages != null && Messages.Count > 0)
            {
                var ServiceMessage = Messages[MarketingMessageCounter];
                if (ServiceMessage != null)
                {
                    var messageTxt = ((!string.IsNullOrEmpty(ServiceMessage.Message1)) ? "<span>" + ServiceMessage.Message1 + "</span><br>" : string.Empty) + ((!string.IsNullOrEmpty(ServiceMessage.Message2)) ? "<span>" + ServiceMessage.Message2 + "</span><br>" : string.Empty) + ((!string.IsNullOrEmpty(ServiceMessage.Message3)) ? "<span>" + ServiceMessage.Message3 + "</span><br>" : string.Empty) + ((!string.IsNullOrEmpty(ServiceMessage.Message4)) ? "<span>" + ServiceMessage.Message4 + "</span><br>" : string.Empty) + ((!string.IsNullOrEmpty(ServiceMessage.Message5)) ? "<span>" + ServiceMessage.Message5 + "</span><br>" : string.Empty);

                    pageContent.Replace("{{ServiceMessageHeader_" + page.Identifier + "_" + widget.Identifier + "_" + MarketingMessageCounter + "}}", ServiceMessage.Header).Replace("{{ServiceMessageText_" + page.Identifier + "_" + widget.Identifier + "_" + MarketingMessageCounter + "}}", messageTxt);
                }
            }
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
