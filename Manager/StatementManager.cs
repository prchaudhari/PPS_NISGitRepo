// <copyright file="StatementManager.cs" company="Websym Solutions Pvt. Ltd.">
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
    using System.Dynamic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Security.Claims;
    using System.Text;
    using System.Text.RegularExpressions;
    using Unity;
    #endregion

    public class StatementManager
    {
        #region Private members
        /// <summary>
        /// The unity container
        /// </summary>
        private IUnityContainer unityContainer = null;

        /// <summary>
        /// The Statement repository.
        /// </summary>
        IStatementRepository StatementRepository = null;

        IPageRepository pageRepository = null;
        //IInvestmentRepository investmentRepository = null;
        //ICustomerRepository customerRepository = null;

        /// <summary>
        /// The validation engine object
        /// </summary>
        IValidationEngine validationEngine = null;

        /// <summary>
        /// The Asset repository.
        /// </summary>
        private IAssetLibraryRepository assetLibraryRepository = null;

        /// <summary>
        /// The tenant configuration manager object.
        /// </summary>
        private TenantConfigurationManager tenantConfigurationManager = null;

        /// <summary>
        /// The dynamic widget manager object.
        /// </summary>
        private DynamicWidgetManager dynamicWidgetManager = null;

        /// <summary>
        /// The tenant transaction data manager.
        /// </summary>
        TenantTransactionDataManager tenantTransactionDataManager = null;

        /// <summary>
        /// The utility object
        /// </summary>
        private IUtility utility = null;

        #endregion

        #region Constructor

        /// <summary>
        /// This is constructor for role manager, which initialise
        /// role repository.
        /// </summary>
        /// <param name="container">IUnity container implementation object.</param>
        public StatementManager(IUnityContainer unityContainer)
        {
            try
            {
                this.unityContainer = unityContainer;
                this.StatementRepository = this.unityContainer.Resolve<IStatementRepository>();
                this.pageRepository = this.unityContainer.Resolve<IPageRepository>();
                this.assetLibraryRepository = this.unityContainer.Resolve<IAssetLibraryRepository>();
                //this.customerRepository = this.unityContainer.Resolve<ICustomerRepository>();
                //this.investmentRepository = this.unityContainer.Resolve<IInvestmentRepository>();
                this.tenantConfigurationManager = new TenantConfigurationManager(unityContainer);
                this.dynamicWidgetManager = new DynamicWidgetManager(unityContainer);
                this.tenantTransactionDataManager = new TenantTransactionDataManager(unityContainer);
                this.validationEngine = new ValidationEngine();
                this.utility = new Utility();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// This method will call add Statements method of repository.
        /// </summary>
        /// <param name="Statements">Statements are to be add.</param>
        /// <param name="tenantCode">Tenant code of Statement.</param>
        /// <returns>
        /// Returns true if entities added successfully, false otherwise.
        /// </returns>
        public bool AddStatements(IList<Statement> Statements, string tenantCode)
        {
            bool result = false;
            try
            {
                this.IsValidStatements(Statements, tenantCode);
                this.IsDuplicateStatement(Statements, tenantCode);
                result = this.StatementRepository.AddStatements(Statements, tenantCode);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method will call update Statements method of repository
        /// </summary>
        /// <param name="Statements">Statements are to be update.</param>
        /// <param name="tenantCode">Tenant code of Statement.</param>
        /// <returns>
        /// Returns true if roles updated successfully, false otherwise.
        /// </returns>
        public bool UpdateStatements(IList<Statement> Statements, string tenantCode)
        {
            bool result = false;
            try
            {
                this.IsValidStatements(Statements, tenantCode);
                this.IsDuplicateStatement(Statements, tenantCode);
                result = this.StatementRepository.UpdateStatements(Statements, tenantCode);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method will call delete Statements method of repository
        /// </summary>
        /// <param name="StatementIdentifier">Statement iddentifier</param>
        /// <param name="tenantCode">Tenant code of Statement.</param>
        /// <returns>
        /// Returns true if Statements deleted successfully, false otherwise.
        /// </returns>
        public bool DeleteStatements(long StatementIdentifier, string tenantCode)
        {
            bool result = false;
            try
            {
                result = this.StatementRepository.DeleteStatements(StatementIdentifier, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return result;
        }

        /// <summary>
        /// This method will call get Statements method of repository.
        /// </summary>
        /// <param name="StatementSearchParameter">The Statement search parameters.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns>
        /// Returns Statements if found for given parameters, else return null
        /// </returns>
        public IList<Statement> GetStatements(StatementSearchParameter StatementSearchParameter, string tenantCode)
        {
            try
            {
                InvalidSearchParameterException invalidSearchParameterException = new InvalidSearchParameterException(tenantCode);
                try
                {
                    StatementSearchParameter.IsValid();
                }
                catch (Exception exception)
                {
                    invalidSearchParameterException.Data.Add("InvalidPagingParameter", exception.Data);
                }

                if (invalidSearchParameterException.Data.Count > 0)
                {
                    throw invalidSearchParameterException;
                }

                StatementSearchParameter.StartDate = this.validationEngine.IsValidDate(StatementSearchParameter.StartDate) ? StatementSearchParameter.StartDate.ToLocalTime() : StatementSearchParameter.StartDate;
                StatementSearchParameter.EndDate = this.validationEngine.IsValidDate(StatementSearchParameter.EndDate) ? StatementSearchParameter.EndDate.ToLocalTime() : StatementSearchParameter.EndDate;

                return this.StatementRepository.GetStatements(StatementSearchParameter, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// This method helps to get count of Statements.
        /// </summary>
        /// <param name="StatementSearchParameter">The Statement search parameters.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns>
        /// Returns count of Statements
        /// </returns>
        public int GetStatementCount(StatementSearchParameter StatementSearchParameter, string tenantCode)
        {
            int roleCount = 0;
            try
            {
                roleCount = this.StatementRepository.GetStatementCount(StatementSearchParameter, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return roleCount;
        }

        /// <summary>
        /// This method will call publish Statement method of repository
        /// </summary>
        /// <param name="StatementIdentifier">Statement identifier</param>
        /// <param name="tenantCode">Tenant code of Statement.</param>
        /// <returns>
        /// Returns true if Statements publish successfully, false otherwise.
        /// </returns>
        public bool PublishStatement(long StatementIdentifier, string tenantCode)
        {
            bool result = false;
            try
            {
                result = this.StatementRepository.PublishStatement(StatementIdentifier, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return result;
        }

        /// <summary>
        /// This method will call clone Statement method of repository
        /// </summary>
        /// <param name="StatementIdentifier">Statement identifier</param>
        /// <param name="tenantCode">Tenant code of Statement.</param>
        /// <returns>
        /// Returns true if Statements clone successfully, false otherwise.
        /// </returns>
        public bool CloneStatement(long StatementIdentifier, string tenantCode)
        {
            bool result = false;
            try
            {
                result = this.StatementRepository.CloneStatement(StatementIdentifier, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return result;
        }

        /// <summary>
        /// This method will geenerate preview Statement html string
        /// </summary>
        /// <param name="StatementIdentifier">Statement identifier</param>
        /// <param name="baseURL">API base URL</param>
        /// <param name="tenantCode">Tenant code of Statement.</param>
        /// <returns>
        /// Returns Statements preview html string.
        /// </returns>
        public string PreviewStatement(long statementIdentifier, string baseURL, string tenantCode)
        {

            string StatmentPreviewHtml = "";
            try
            {
                var tenantConfiguration = this.tenantConfigurationManager.GetTenantConfigurations(tenantCode)?.FirstOrDefault();
                StatementSearchParameter statementSearchParameter = new StatementSearchParameter
                {
                    Identifier = statementIdentifier,
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
                var statements = this.StatementRepository.GetStatements(statementSearchParameter, tenantCode);
                if (statements.Count != 0)
                {
                    var statementPages = statements[0].StatementPages.OrderBy(it => it.SequenceNumber).ToList();
                    if (statementPages.Count != 0)
                    {
                        var functionName = string.Empty;
                        if (tenantConfiguration != null && !string.IsNullOrEmpty(tenantConfiguration.PreviewStatementFunctionName))
                        {
                            functionName = tenantConfiguration.PreviewStatementFunctionName;
                        }

                        switch (functionName)
                        {
                            case ModelConstant.PREVIEW_FINANCIAL_STATEMENT_FUNCTION_NAME:
                                StatmentPreviewHtml = this.PreviewFinancialStatement(statementPages, tenantConfiguration, baseURL, tenantCode);
                                break;

                            //case ModelConstant.PREVIEW_NEDBANK_STATEMENT_FUNCTION_NAME:
                            //    StatmentPreviewHtml = this.PreviewNedbankStatement(statementPages, tenantConfiguration, baseURL, tenantCode);
                            //    break;

                            default:
                                StatmentPreviewHtml = this.PreviewFinancialStatement(statementPages, tenantConfiguration, baseURL, tenantCode);
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return StatmentPreviewHtml;
        }

        /// <summary>
        /// This method will geenerate HTML format of the Statement
        /// </summary>
        /// <param name="statement">the statment</param>
        /// <param name="tenantCode">Tenant code of Statement.</param>
        /// <returns>
        /// Returns list of statement page content object.
        /// </returns>
        public IList<StatementPageContent> GenerateHtmlFormatOfStatement(Statement statement, string tenantCode, TenantConfiguration tenantConfiguration)
        {
            try
            {
                List<StatementPageContent> statementPageContents = new List<StatementPageContent>();
                if (statement != null)
                {
                    //rearrange statement page with their user defined sequence
                    var statementPages = statement.StatementPages.OrderBy(it => it.SequenceNumber).ToList();
                    if (statementPages.Count > 0)
                    {
                        int counter = 0;
                        statement.Pages = new List<Page>();
                        for (int i = 0; i < statementPages.Count; i++)
                        {
                            StatementPageContent statementPageContent = new StatementPageContent();
                            StringBuilder pageHtmlContent = new StringBuilder();
                            statementPageContent.Id = i;

                            var pages = this.pageRepository.GetPages(new PageSearchParameter
                            {
                                Identifier = statementPages[i].ReferencePageId,
                                IsPageWidgetsRequired = true,
                                IsActive = true,
                                PagingParameter = new PagingParameter
                                {
                                    PageIndex = 0,
                                    PageSize = 0,
                                },
                                SortParameter = new SortParameter()
                                {
                                    SortOrder = SortOrder.Ascending,
                                    SortColumn = "DisplayName",
                                },
                                SearchMode = SearchMode.Equals
                            }, tenantCode);
                            if (pages.Count != 0)
                            {
                                for (int j = 0; j < pages.Count; j++)
                                {
                                    var page = pages[j];
                                    statementPageContent.PageId = page.Identifier;
                                    statementPageContent.PageTypeId = page.PageTypeId;
                                    statementPageContent.DisplayName = page.DisplayName;
                                    statement.Pages.Add(page);

                                    StringBuilder pageHeaderContent = new StringBuilder();
                                    pageHeaderContent.Append(HtmlConstants.WIDGET_HTML_HEADER);
                                    if (page.PageTypeName == HtmlConstants.SAVING_ACCOUNT_PAGE || page.PageTypeName == HtmlConstants.CURRENT_ACCOUNT_PAGE)
                                    {
                                        pageHeaderContent.Append("{{SubTabs}}");
                                    }
                                    statementPageContent.PageHeaderContent = pageHeaderContent.ToString();

                                    int tempRowWidth = 0; // variable to check col-lg div length (bootstrap)
                                    int max = 0;
                                    if (page.PageWidgets.Count > 0)
                                    {
                                        //get all widgets of current page
                                        var completelst = new List<PageWidget>(page.PageWidgets);

                                        //Get all dynamic widgets of current page and assign it to statement page content object for future use
                                        var dynamicwidgetids = string.Join(", ", completelst.Where(item => item.IsDynamicWidget).ToList().Select(item => item.WidgetId));
                                        var dynawidgets = new List<DynamicWidget>();
                                        if (dynamicwidgetids != string.Empty)
                                        {
                                            dynawidgets = this.dynamicWidgetManager.GetDynamicWidgets(new DynamicWidgetSearchParameter
                                            {
                                                Identifier = dynamicwidgetids,
                                                PagingParameter = new PagingParameter
                                                {
                                                    PageIndex = 0,
                                                    PageSize = 0,
                                                },
                                                SortParameter = new SortParameter()
                                                {
                                                    SortOrder = SortOrder.Ascending,
                                                    SortColumn = "Title",
                                                },
                                                SearchMode = SearchMode.Contains
                                            }, tenantCode)?.ToList();
                                        }
                                        statementPageContent.DynamicWidgets = dynawidgets;

                                        //current Y position variable to filter widgets, start with 0
                                        int currentYPosition = 0;
                                        var isRowComplete = false;

                                        while (completelst.Count != 0)
                                        {
                                            //filter 1st row widgets from current page widgets.. get widgets by current Y position
                                            var lst = completelst.Where(it => it.Yposition == currentYPosition).ToList();
                                            if (lst.Count > 0)
                                            {
                                                //find widget which has max height among them and assign it to max Y position variable
                                                max = max + lst.Max(it => it.Height);

                                                //filter widgets which are in between current Y Position and above max Y Position
                                                var _lst = completelst.Where(it => it.Yposition < max && it.Yposition != currentYPosition).ToList();

                                                //merge 2 widgets into single list which are finds by current Y position and then finds with in between current and max Y position
                                                var mergedlst = lst.Concat(_lst).OrderBy(it => it.Xposition).ToList();

                                                //assign current Y position to new max Y position
                                                currentYPosition = max;

                                                //loop over current merge widget list and create empty HTML template
                                                for (int x = 0; x < mergedlst.Count; x++)
                                                {
                                                    var pageWidget = mergedlst[x];

                                                    //if tempRowWidth equals to zero then create new div with bootstrap row class
                                                    if (tempRowWidth == 0)
                                                    {
                                                        pageHtmlContent.Append("<div class='row pt-2'>");
                                                        isRowComplete = false;
                                                    }

                                                    //get current widget width
                                                    var divLength = pageWidget.Width;

                                                    //get current widget height and multiple with 110 as per angular gridster implementation of each column height
                                                    //to find actual height for current widget in pixel
                                                    var divHeight = pageWidget.Height * 110 + "px";
                                                    tempRowWidth = tempRowWidth + divLength;

                                                    // If current col-lg class length is greater than 12, then end parent row class div and then start new row class div
                                                    if (tempRowWidth > 12)
                                                    {
                                                        tempRowWidth = divLength;
                                                        pageHtmlContent.Append(HtmlConstants.END_DIV_TAG); // to end row class div
                                                        pageHtmlContent.Append("<div class='row pt-2'>"); // to start new row class div
                                                        isRowComplete = false;
                                                    }

                                                    //if rendering html for 1st page, then add padding zero current widget div, otherwise keep default padding
                                                    var leftPaddingClass = i != 0 ? " pl-0" : string.Empty;

                                                    //create new div with col-lg with newly finded div length and above padding zero value
                                                    pageHtmlContent.Append("<div class='col-lg-" + divLength + leftPaddingClass + "'>");

                                                    //check current widget is dynamic or static and start generating empty html template for current widget
                                                    if (!pageWidget.IsDynamicWidget)
                                                    {
                                                        switch (pageWidget.WidgetName)
                                                        {
                                                            case HtmlConstants.CUSTOMER_INFORMATION_WIDGET_NAME:
                                                                pageHtmlContent.Append(this.CustomerInformationWidgetFormatting(pageWidget, counter, statement, page, divHeight));
                                                                break;
                                                            case HtmlConstants.PAYMENT_SUMMARY_WIDGET_NAME:
                                                                pageHtmlContent.Append(this.PaymentSummaryWidgetFormatting(pageWidget, counter, statement, page, divHeight));
                                                                break;

                                                            case HtmlConstants.PPS_HEADING_WIDGET_NAME:
                                                                pageHtmlContent.Append(this.PpsHeadingWidgetFormatting(pageWidget, counter, statement, page, divHeight));
                                                                break;

                                                            case HtmlConstants.PPS_DETAILS_WIDGET_NAME:
                                                                pageHtmlContent.Append(this.PpsDetailsWidgetFormatting(pageWidget, counter, statement, page, divHeight));
                                                                break;

                                                            case HtmlConstants.PPS_FOOTER1_WIDGET_NAME:
                                                                pageHtmlContent.Append(this.PpsFooter1WidgetFormatting(pageWidget, counter, statement, page, divHeight));
                                                                break;

                                                            case HtmlConstants.PRODUCT_SUMMARY_WIDGET_NAME:
                                                                pageHtmlContent.Append(this.ProductSummaryWidgetFormatting(pageWidget, counter, statement, page, divHeight));
                                                                break;
                                                            case HtmlConstants.DETAILED_TRANSACTIONS_WIDGET_NAME:
                                                                pageHtmlContent.Append(this.DetailedTransactionWidgetFormatting(pageWidget, counter, statement, page, divHeight));
                                                                break;
                                                            case HtmlConstants.PPS_DETAILED_TRANSACTIONS_WIDGET_NAME:
                                                                pageHtmlContent.Append(this.PpsDetailedTransactionWidgetFormatting(pageWidget, counter, statement, page, divHeight));
                                                                break;
                                                            case HtmlConstants.FOOTER_IMAGE_WIDGET_NAME:
                                                                pageHtmlContent.Append(this.FooterImageWidgetFormatting(pageWidget, counter, statement, page, divHeight));
                                                                break;
                                                            case HtmlConstants.PPS_DETAILS1_WIDGET_NAME:
                                                                pageHtmlContent.Append(this.PpsDetails1WidgetFormatting(pageWidget, counter, statement, page, divHeight));
                                                                break;
                                                            case HtmlConstants.PPS_DETAILS2_WIDGET_NAME:
                                                                pageHtmlContent.Append(this.PpsDetails2WidgetFormatting(pageWidget, counter, statement, page, divHeight));
                                                                break;
                                                            case HtmlConstants.EARNINGS_FOR_PERIOD_WIDGET_NAME:
                                                                pageHtmlContent.Append(this.EarningForPeriodWidgetFormatting(pageWidget, counter, statement, page, divHeight));
                                                                break;
                                                            case HtmlConstants.ACCOUNT_INFORMATION_WIDGET_NAME:
                                                                pageHtmlContent.Append(this.AccountInformationWidgetFormatting(pageWidget, counter, statement, page, divHeight));
                                                                break;

                                                            case HtmlConstants.IMAGE_WIDGET_NAME:
                                                                pageHtmlContent.Append(this.ImageWidgetFormatting(pageWidget, counter, statement, page, divHeight));
                                                                break;

                                                            case HtmlConstants.VIDEO_WIDGET_NAME:
                                                                pageHtmlContent.Append(this.VideoWidgetFormatting(pageWidget, counter, statement, page, divHeight));
                                                                break;

                                                            case HtmlConstants.SUMMARY_AT_GLANCE_WIDGET_NAME:
                                                                pageHtmlContent.Append(this.SummaryAtGlanceWidgetFormatting(pageWidget, counter, page, divHeight));
                                                                break;

                                                            case HtmlConstants.CURRENT_AVAILABLE_BALANCE_WIDGET_NAME:
                                                                pageHtmlContent.Append(this.CurrentAvailBalanceWidgetFormatting(pageWidget, counter, page, divHeight));
                                                                break;

                                                            case HtmlConstants.SAVING_AVAILABLE_BALANCE_WIDGET_NAME:
                                                                pageHtmlContent.Append(this.SavingAvailBalanceWidgetFormatting(pageWidget, counter, page, divHeight));
                                                                break;

                                                            case HtmlConstants.SAVING_TRANSACTION_WIDGET_NAME:
                                                                pageHtmlContent.Append(this.SavingTransactionWidgetFormatting(pageWidget, counter, page, divHeight));
                                                                break;

                                                            case HtmlConstants.CURRENT_TRANSACTION_WIDGET_NAME:
                                                                pageHtmlContent.Append(this.CurrentTranactionWidgetFormatting(pageWidget, counter, page, divHeight));
                                                                break;

                                                            case HtmlConstants.TOP_4_INCOME_SOURCE_WIDGET_NAME:
                                                                pageHtmlContent.Append(this.TopFourIncomeSourcesWidgetFormatting(pageWidget, counter, page, divHeight));
                                                                break;

                                                            case HtmlConstants.ANALYTICS_WIDGET_NAME:
                                                                pageHtmlContent.Append(this.AnalyticsWidgetFormatting(pageWidget, counter, page, divHeight));
                                                                break;

                                                            case HtmlConstants.SPENDING_TREND_WIDGET_NAME:
                                                                pageHtmlContent.Append(this.SpendingTrendWidgetFormatting(pageWidget, counter, page, divHeight));
                                                                break;

                                                            case HtmlConstants.SAVING_TREND_WIDGET_NAME:
                                                                pageHtmlContent.Append(this.SavingTrendWidgetFormatting(pageWidget, counter, page, divHeight));
                                                                break;

                                                            case HtmlConstants.REMINDER_AND_RECOMMENDATION_WIDGET_NAME:
                                                                pageHtmlContent.Append(this.ReminderAndRecommendationWidgetFormatting(pageWidget, counter, page, divHeight));
                                                                break;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (dynawidgets?.Count > 0)
                                                        {
                                                            var dynawidget = dynawidgets.Where(item => item.Identifier == pageWidget.WidgetId).ToList().FirstOrDefault();

                                                            //get theme for current dynamic widget, 
                                                            //if it is default take theme setting from tenant configuration, otherwise from current widget theme setting
                                                            CustomeTheme themeDetails = new CustomeTheme();
                                                            if (dynawidget.ThemeType == "Default")
                                                            {
                                                                themeDetails = JsonConvert.DeserializeObject<CustomeTheme>(tenantConfiguration.WidgetThemeSetting);
                                                            }
                                                            else
                                                            {
                                                                themeDetails = JsonConvert.DeserializeObject<CustomeTheme>(dynawidget.ThemeCSS);
                                                            }

                                                            switch (dynawidget.WidgetType)
                                                            {
                                                                case HtmlConstants.TABLE_DYNAMICWIDGET:
                                                                    pageHtmlContent.Append(this.DynamicTableWidgetFormatting(themeDetails, dynawidget, pageWidget, counter, statement, page, divHeight));
                                                                    break;

                                                                case HtmlConstants.FORM_DYNAMICWIDGET:
                                                                    pageHtmlContent.Append(this.DynamicFormWidgetFormatting(themeDetails, dynawidget, pageWidget, counter, statement, page, divHeight));
                                                                    break;

                                                                case HtmlConstants.LINEGRAPH_DYNAMICWIDGET:
                                                                    pageHtmlContent.Append(this.DynamicLineGraphWidgetFormatting(themeDetails, dynawidget, pageWidget, counter, statement, page, divHeight));
                                                                    break;

                                                                case HtmlConstants.BARGRAPH_DYNAMICWIDGET:
                                                                    pageHtmlContent.Append(this.DynamicBarGraphWidgetFormatting(themeDetails, dynawidget, pageWidget, counter, statement, page, divHeight));
                                                                    break;

                                                                case HtmlConstants.PICHART_DYNAMICWIDGET:
                                                                    pageHtmlContent.Append(this.DynamicPieChartWidgetFormatting(themeDetails, dynawidget, pageWidget, counter, statement, page, divHeight));
                                                                    break;

                                                                case HtmlConstants.HTML_DYNAMICWIDGET:
                                                                    pageHtmlContent.Append(this.DynamicHtmlWidgetFormatting(themeDetails, dynawidget, pageWidget, counter, statement, page, divHeight));
                                                                    break;
                                                            }
                                                        }
                                                    }

                                                    counter++;
                                                    // To end current col-lg class div
                                                    pageHtmlContent.Append(HtmlConstants.END_DIV_TAG);

                                                    // if current col-lg class width is equal to 12 or end before complete col-lg-12 class, 
                                                    //then end parent row class div
                                                    if (tempRowWidth == 12 || (x == mergedlst.Count - 1))
                                                    {
                                                        tempRowWidth = 0;
                                                        pageHtmlContent.Append(HtmlConstants.END_DIV_TAG); //To end row class div
                                                        isRowComplete = true;
                                                    }
                                                }
                                                mergedlst.ForEach(it =>
                                                {
                                                    completelst.Remove(it);
                                                });
                                            }
                                            else
                                            {
                                                if (completelst.Count != 0)
                                                {
                                                    currentYPosition = completelst.Min(it => it.Yposition);
                                                }
                                            }
                                        }
                                        //If row class div end before complete col-lg-12 class
                                        if (isRowComplete == false)
                                        {
                                            pageHtmlContent.Append(HtmlConstants.END_DIV_TAG);
                                        }
                                    }
                                    else
                                    {
                                        pageHtmlContent.Append(HtmlConstants.NO_WIDGET_MESSAGE_HTML);
                                    }
                                    statementPageContent.PageFooterContent = HtmlConstants.WIDGET_HTML_FOOTER;
                                }
                            }
                            else
                            {
                                pageHtmlContent.Append(HtmlConstants.NO_WIDGET_MESSAGE_HTML);
                            }
                            statementPageContent.HtmlContent = pageHtmlContent.ToString();
                            statementPageContents.Add(statementPageContent);
                        }
                    }
                }

                return statementPageContents;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// This method help to bind data to common statement
        /// </summary>
        /// <param name="statement"> the statement object </param>
        /// <param name="statementPageContents"> the statement page html content list</param>
        /// <param name="tenantCode"> the tenant code </param>
        public StatementPreviewData BindDataToCommonStatement(Statement statement, IList<StatementPageContent> statementPageContents, TenantConfiguration tenantConfiguration, string tenantCode, Client client)
        {
            try
            {
                var AppBaseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                var statementPreviewData = new StatementPreviewData();
                var SampleFiles = new List<FileData>();

                //start to render common html content data
                var htmlbody = new StringBuilder();
                var navbarHtml = HtmlConstants.NAVBAR_HTML_FOR_PREVIEW.Replace("{{logo}}", "../common/images/nisLogo.png");
                navbarHtml = navbarHtml.Replace("{{Today}}", DateTime.UtcNow.ToString("dd MMM yyyy")); //bind current date to html header

                //get client logo in string format and pass it hidden input tag, so it will be render in right side of header of html statement
                var clientlogo = client.TenantLogo != null ? client.TenantLogo : "";
                navbarHtml = navbarHtml + "<input type='hidden' id='TenantLogoImageValue' value='" + clientlogo + "'>";
                htmlbody.Append(HtmlConstants.CONTAINER_DIV_HTML_HEADER);

                //this variable is used to bind all script to html statement, which helps to render data on chart and graph widgets
                var scriptHtmlRenderer = new StringBuilder();
                var navbar = new StringBuilder();
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

                for (int i = 0; i < statement.Pages.Count; i++)
                {
                    var page = statement.Pages[i];
                    var statementPageContent = newStatementPageContents.Where(item => item.PageTypeId == page.PageTypeId && item.Id == i).FirstOrDefault();
                    var pageContent = new StringBuilder(statementPageContent.HtmlContent);
                    var dynamicWidgets = statementPageContent.DynamicWidgets;

                    var SubTabs = new StringBuilder();
                    var PageHeaderContent = new StringBuilder(statementPageContent.PageHeaderContent);

                    var tabClassName = Regex.Replace((statementPageContent.DisplayName + "-" + page.Identifier), @"\s+", "-");
                    navbar.Append(" <li class='nav-item'><a class='nav-link pt-1 mainNav " + (i == 0 ? "active" : "") + " " + tabClassName + "' href='javascript:void(0);' >" + statementPageContent.DisplayName + "</a> </li> ");
                    PageHeaderContent.Replace("{{ExtraClass}}", (i > 0 ? "d-none " + tabClassName : tabClassName)).Replace("{{DivId}}", tabClassName);

                    var newPageContent = new StringBuilder();
                    newPageContent.Append(HtmlConstants.PAGE_TAB_CONTENT_HEADER);

                    if (page.PageTypeName == HtmlConstants.SAVING_ACCOUNT_PAGE || page.PageTypeName == HtmlConstants.CURRENT_ACCOUNT_PAGE)
                    {
                        SubTabs.Append("<ul class='nav nav-tabs' style='margin-top:-20px;'>").Append("<li class='nav-item active'><a id='tab1-tab' data-toggle='tab' " + "data-target='#" + (page.PageTypeName == HtmlConstants.SAVING_ACCOUNT_PAGE ? "Saving" : "Current") + "-' role='tab' class='nav-link active'> Account - XX89</a></li>").Append("</ul>");

                        newPageContent.Append("<div id='" + (page.PageTypeName == HtmlConstants.SAVING_ACCOUNT_PAGE ? "Saving" : "Current") + "-6789' >");
                    }

                    var pagewidgets = new List<PageWidget>(page.PageWidgets);
                    for (int j = 0; j < pagewidgets.Count; j++)
                    {
                        var widget = pagewidgets[j];
                        if (!widget.IsDynamicWidget)
                        {
                            switch (widget.WidgetName)
                            {
                                case HtmlConstants.CUSTOMER_INFORMATION_WIDGET_NAME:
                                    this.BindDummyDataToCustomerInformationWidget(pageContent, statement, page, widget, AppBaseDirectory);
                                    break;
                                case HtmlConstants.PRODUCT_SUMMARY_WIDGET_NAME:
                                    this.BindDummyDataToProductSummaryWidget(pageContent, statement, page, widget, AppBaseDirectory);
                                    break;
                                case HtmlConstants.PAYMENT_SUMMARY_WIDGET_NAME:
                                    this.BindDummyDataToPaymentSummaryWidget(pageContent, statement, page, widget, AppBaseDirectory);
                                    break;

                                case HtmlConstants.PPS_HEADING_WIDGET_NAME:
                                    this.BindDummyDataToPpsHeadingWidget(pageContent, statement, page, widget, AppBaseDirectory);
                                    break;

                                case HtmlConstants.PPS_DETAILS_WIDGET_NAME:
                                    this.BindDummyDataToPpsDetailsWidget(pageContent, statement, page, widget, AppBaseDirectory);
                                    break;

                                case HtmlConstants.PPS_FOOTER1_WIDGET_NAME:
                                    this.BindDummyDataToPpsFooter1Widget(pageContent, statement, page, widget, AppBaseDirectory);
                                    break;

                                case HtmlConstants.FOOTER_IMAGE_WIDGET_NAME:
                                    this.BindDummyDataToFooterImageWidget(pageContent, statement, page, widget, AppBaseDirectory);
                                    break;

                                case HtmlConstants.EARNINGS_FOR_PERIOD_WIDGET_NAME:
                                    this.BindDummyDataToEarningForPeriodWidget(pageContent, statement, page, widget, AppBaseDirectory);
                                    break;
                                case HtmlConstants.ACCOUNT_INFORMATION_WIDGET_NAME:
                                    this.BindDummyDataToAccountInformationWidget(pageContent, page, widget);
                                    break;


                                case HtmlConstants.IMAGE_WIDGET_NAME:
                                    this.BindDummyDataToImageWidget(pageContent, statement, page, widget, SampleFiles, AppBaseDirectory, tenantCode);
                                    break;

                                case HtmlConstants.PPS_DETAILS1_WIDGET_NAME:
                                    this.BindDummyDataToPpsDetails1Widget(pageContent, statement, page, widget, AppBaseDirectory);
                                    break;
                                case HtmlConstants.PPS_DETAILS2_WIDGET_NAME:
                                    this.BindDummyDataToPpsDetails2Widget(pageContent, statement, page, widget, AppBaseDirectory);
                                    break;

                                case HtmlConstants.VIDEO_WIDGET_NAME:
                                    this.BindDummyDataToVideoWidget(pageContent, statement, page, widget, SampleFiles, AppBaseDirectory, tenantCode);
                                    break;

                                case HtmlConstants.SUMMARY_AT_GLANCE_WIDGET_NAME:
                                    this.BindDummyDataToSummaryAtGlanceWidget(pageContent, page, widget);
                                    break;

                                case HtmlConstants.CURRENT_AVAILABLE_BALANCE_WIDGET_NAME:
                                    this.BindDummyDataToCurrentAvailBalanceWidget(pageContent, page, widget);
                                    break;

                                case HtmlConstants.SAVING_AVAILABLE_BALANCE_WIDGET_NAME:
                                    this.BindDummyDataToSavingAvailBalanceWidget(pageContent, page, widget);
                                    break;

                                case HtmlConstants.SAVING_TRANSACTION_WIDGET_NAME:
                                    this.BindDummyDataToSavingTransactioneWidget(pageContent, scriptHtmlRenderer, page, widget, SampleFiles, AppBaseDirectory);
                                    break;

                                case HtmlConstants.CURRENT_TRANSACTION_WIDGET_NAME:
                                    this.BindDummyToCurrentTransactioneWidget(pageContent, scriptHtmlRenderer, page, widget, SampleFiles, AppBaseDirectory);
                                    break;

                                case HtmlConstants.TOP_4_INCOME_SOURCE_WIDGET_NAME:
                                    this.BindDummyDataToTopFourIncomeSourcesWidget(pageContent, page, widget);
                                    break;

                                case HtmlConstants.ANALYTICS_WIDGET_NAME:
                                    this.BindDummyDataToAnalyticsWidget(pageContent, scriptHtmlRenderer, page, SampleFiles, AppBaseDirectory);
                                    break;

                                case HtmlConstants.SPENDING_TREND_WIDGET_NAME:
                                    this.BindDummyDataToSpendingTrendWidget(pageContent, scriptHtmlRenderer, page, SampleFiles, AppBaseDirectory);
                                    break;

                                case HtmlConstants.SAVING_TREND_WIDGET_NAME:
                                    this.BindDummyDataToSavingTrendWidget(pageContent, scriptHtmlRenderer, page, SampleFiles, AppBaseDirectory);
                                    break;

                                case HtmlConstants.REMINDER_AND_RECOMMENDATION_WIDGET_NAME:
                                    this.BindDummyDataToReminderAndRecommendationWidget(pageContent, page, widget);
                                    break;
                            }
                        }
                        else
                        {
                            var dynaWidgets = dynamicWidgets?.Where(item => item.Identifier == widget.WidgetId).ToList();
                            if (dynaWidgets?.Count > 0)
                            {
                                var dynawidget = dynaWidgets.FirstOrDefault();
                                switch (dynawidget.WidgetType)
                                {
                                    case HtmlConstants.TABLE_DYNAMICWIDGET:
                                        this.BindDummyDataToDynamicTableWidget(pageContent, page, widget, dynawidget);
                                        break;

                                    case HtmlConstants.FORM_DYNAMICWIDGET:
                                        this.BindDummyDataToDynamicFormWidget(pageContent, page, widget, dynawidget);
                                        break;

                                    case HtmlConstants.LINEGRAPH_DYNAMICWIDGET:
                                        this.BindDummyDataToDynamicLineGraphWidget(pageContent, scriptHtmlRenderer, page, widget, dynawidget);
                                        break;

                                    case HtmlConstants.BARGRAPH_DYNAMICWIDGET:
                                        this.BindDummyDataToDynamicBarGraphWidget(pageContent, scriptHtmlRenderer, page, widget, dynawidget);
                                        break;

                                    case HtmlConstants.PICHART_DYNAMICWIDGET:
                                        this.BindDummyDataToDynamicPieChartWidget(pageContent, scriptHtmlRenderer, page, widget, dynawidget);
                                        break;

                                    case HtmlConstants.HTML_DYNAMICWIDGET:
                                        this.BindDummyDataToDynamicHtmlWidget(pageContent, page, widget, dynawidget, tenantCode);
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
                    newPageContent.Append(HtmlConstants.PAGE_TAB_CONTENT_FOOTER); //to end tab-content div

                    PageHeaderContent.Replace("{{SubTabs}}", SubTabs.ToString());
                    statementPageContent.PageHeaderContent = PageHeaderContent.ToString();
                    statementPageContent.HtmlContent = newPageContent.ToString();
                }

                newStatementPageContents.ToList().ForEach(page =>
                {
                    htmlbody.Append(page.PageHeaderContent).Append(page.HtmlContent).Append(page.PageFooterContent);
                });

                htmlbody.Append(HtmlConstants.CONTAINER_DIV_HTML_FOOTER);
                navbarHtml = navbarHtml.Replace("{{NavItemList}}", navbar.ToString());

                StringBuilder finalHtml = new StringBuilder();
                finalHtml.Append(HtmlConstants.HTML_HEADER).Append(navbarHtml).Append(htmlbody.ToString()).Append(HtmlConstants.HTML_FOOTER);
                scriptHtmlRenderer.Append(HtmlConstants.TENANT_LOGO_SCRIPT);
                finalHtml.Replace("{{ChartScripts}}", scriptHtmlRenderer.ToString());

                statementPreviewData.SampleFiles = SampleFiles;
                statementPreviewData.FileContent = finalHtml.ToString();
                return statementPreviewData;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
              
        #endregion

        #region Private Methods

        /// <summary>
        /// This method is responsible for validate Statements.
        /// </summary>
        /// <param name="Statements"></param>
        /// <param name="tenantCode"></param>
        private void IsValidStatements(IList<Statement> Statements, string tenantCode)
        {
            try
            {
                if (Statements?.Count <= 0)
                {
                    throw new NullArgumentException(tenantCode);
                }

                InvalidStatementException invalidStatementException = new InvalidStatementException(tenantCode);
                Statements.ToList().ForEach(item =>
                {
                    try
                    {
                        item.IsValid();
                    }
                    catch (Exception ex)
                    {
                        invalidStatementException.Data.Add(item.Name, ex.Data);
                    }
                });

                if (invalidStatementException.Data.Count > 0)
                {
                    throw invalidStatementException;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method helps to check duplicate Statement in the list
        /// </summary>
        /// <param name="Statements"></param>
        /// <param name="tenantCode"></param>
        private void IsDuplicateStatement(IList<Statement> Statements, string tenantCode)
        {
            try
            {
                int isDuplicateStatement = Statements.GroupBy(p => p.Name).Where(g => g.Count() > 1).Count();
                if (isDuplicateStatement > 0)
                {
                    throw new DuplicateStatementFoundException(tenantCode);
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// This method help to apply theme for dynamic table, form, and html widgets
        /// </summary>
        /// <param name="html"> the widget html string </param>
        /// <param name="themeDetails"> the theme details for widget </param>
        /// <returns>return new html after applying theme </returns>
        private string ApplyStyleCssForDynamicTableAndFormWidget(string html, CustomeTheme themeDetails)
        {
            StringBuilder style = new StringBuilder();
            style.Append(HtmlConstants.STYLE);

            if (themeDetails.TitleColor != null)
            {
                style = style.Replace("{{COLOR}}", themeDetails.TitleColor);
            }
            if (themeDetails.TitleSize != null)
            {
                style = style.Replace("{{SIZE}}", themeDetails.TitleSize);
            }
            if (themeDetails.TitleWeight != null)
            {
                style = style.Replace("{{WEIGHT}}", themeDetails.TitleWeight);
            }
            if (themeDetails.TitleType != null)
            {
                style = style.Replace("{{TYPE}}", themeDetails.TitleType);
            }
            html = html.Replace("{{TitleStyle}}", style.ToString());

            style = new StringBuilder();
            style.Append(HtmlConstants.STYLE);
            if (themeDetails.HeaderColor != null)
            {
                style = style.Replace("{{COLOR}}", themeDetails.HeaderColor);
            }
            if (themeDetails.HeaderSize != null)
            {
                style = style.Replace("{{SIZE}}", themeDetails.HeaderSize);
            }
            if (themeDetails.HeaderWeight != null)
            {
                style = style.Replace("{{WEIGHT}}", themeDetails.HeaderWeight);
            }
            if (themeDetails.HeaderType != null)
            {
                style = style.Replace("{{TYPE}}", themeDetails.HeaderType);
            }
            html = html.Replace("{{HeaderStyle}}", style.ToString());

            style = new StringBuilder();
            style.Append(HtmlConstants.STYLE);
            if (themeDetails.DataColor != null)
            {
                style = style.Replace("{{COLOR}}", themeDetails.DataColor);
            }
            if (themeDetails.DataSize != null)
            {
                style = style.Replace("{{SIZE}}", themeDetails.DataSize);
            }
            if (themeDetails.DataWeight != null)
            {
                style = style.Replace("{{WEIGHT}}", themeDetails.DataWeight);
            }
            if (themeDetails.DataType != null)
            {
                style = style.Replace("{{TYPE}}", themeDetails.DataType);
            }
            html = html.Replace("{{BodyStyle}}", style.ToString());
            return html;
        }

        /// <summary>
        /// This method help to apply theme for dynamic line graph, bar graph, and pie chart widgets
        /// </summary>
        /// <param name="html"> the widget html string </param>
        /// <param name="themeDetails"> the theme details for widget </param>
        /// <returns>return new html after applying theme </returns>
        private string ApplyStyleCssForDynamicGraphAndChartWidgets(string html, CustomeTheme themeDetails)
        {
            StringBuilder style = new StringBuilder();
            style.Append(HtmlConstants.STYLE);
            if (themeDetails.TitleColor != null)
            {
                style = style.Replace("{{COLOR}}", themeDetails.TitleColor);
            }
            if (themeDetails.TitleSize != null)
            {
                style = style.Replace("{{SIZE}}", themeDetails.TitleSize);
            }
            if (themeDetails.TitleWeight != null)
            {
                style = style.Replace("{{WEIGHT}}", themeDetails.TitleWeight);
            }
            if (themeDetails.TitleType != null)
            {
                style = style.Replace("{{TYPE}}", themeDetails.TitleType);
            }
            html = html.Replace("{{TitleStyle}}", style.ToString());
            return html;
        }

        /// <summary>
        /// This method help to get preview data for html widget
        /// </summary>
        /// <param name="entity"> the entity object </param>
        /// <param name="fieldMaps"> the list of entity field map object </param>
        /// <param name="widgetSettings"> the widget setting </param>
        /// <returns>return new updated widget setting string </returns>
        private string GetHTMLPreviewData(TenantEntity entity, IList<EntityFieldMap> fieldMaps, string widgetSettings)
        {
            string obj = string.Empty;
            JObject item = new JObject();
            fieldMaps.ToList().ForEach(field =>
            {
                item[field.Name] = field.Name + "1";
            });
            StringBuilder tableBody = new StringBuilder();

            fieldMaps.ToList().ForEach(field =>
            {
                string fieldIdFormat = "{{" + field.Name + "_" + field.Identifier + "}}";
                if (widgetSettings.Contains(fieldIdFormat))
                {
                    widgetSettings = widgetSettings.Replace(fieldIdFormat.ToString(), item[field.Name].ToString());
                }
            });
            obj = widgetSettings;
            return obj;
        }

        /// <summary>
        /// This method help to generate statement preview HTML string for financial tenants
        /// </summary>
        /// <param name="statementPages"> the list statement pages </param>
        /// <param name="tenantConfiguration"> the tenant configuration object </param>
        /// <param name="baseURL"> the base url </param>
        /// /// <param name="tenantCode"> the tenant code </param>
        /// <returns>return statement preview HTML string </returns>
        private string PreviewFinancialStatement(List<StatementPage> statementPages, TenantConfiguration tenantConfiguration, string baseURL, string tenantCode)
        {
            try
            {
                StringBuilder tempHtml = new StringBuilder();
                string finalHtml = string.Empty;
                StringBuilder navItemList = new StringBuilder();

                string navbarHtml = HtmlConstants.NAVBAR_HTML_FOR_PREVIEW.Replace("{{logo}}", "assets/images/nisLogo.png");
                navbarHtml = navbarHtml.Replace("{{Today}}", DateTime.Now.ToString(ModelConstant.DATE_FORMAT_dd_MMM_yyyy));
                tempHtml.Append("<div class='bdy-scroll stylescrollbar'>");

                IList<string> linegraphIds = new List<string>();
                IList<string> bargraphIds = new List<string>();
                IList<string> piechartIds = new List<string>();

                for (int x = 0; x < statementPages.Count; x++)
                {
                    var htmlString = new StringBuilder();
                    PageSearchParameter pageSearchParameter = new PageSearchParameter
                    {
                        Identifier = statementPages[x].ReferencePageId,
                        IsPageWidgetsRequired = true,
                        IsActive = true,
                        PagingParameter = new PagingParameter
                        {
                            PageIndex = 0,
                            PageSize = 0,
                        },
                        SortParameter = new SortParameter()
                        {
                            SortOrder = SortOrder.Ascending,
                            SortColumn = "DisplayName",
                        },
                        SearchMode = SearchMode.Equals
                    };
                    var isBackgroundImage = false;
                    IList<Page> pages = this.pageRepository.GetPages(pageSearchParameter, tenantCode);
                    if (pages.Count != 0)
                    {
                        for (int y = 0; y < pages.Count; y++)
                        {
                            var page = pages[y];
                            var tabClassName = Regex.Replace((page.DisplayName + " " + page.Version), @"\s+", "-");
                            navItemList.Append(" <li class='nav-item p-1 '><a class='nav-link pt-1 mainNav " + (x == 0 ? "active" : "") + " " + tabClassName + "' href='javascript:void(0);'>" + page.DisplayName + "</a> </li> ");

                            var extraclass = x > 0 ? "d-none " + tabClassName : tabClassName;
                            var pageHeaderHtml = "<div id='{{DivId}}' class='p-2 tabDivClass {{ExtraClass}}' {{BackgroundImage}}>";
                            if (page.PageTypeName == HtmlConstants.HOME_PAGE)
                            {
                                if (page.BackgroundImageAssetId != 0)
                                {
                                    pageHeaderHtml = pageHeaderHtml.Replace("{{BackgroundImage}}", string.Empty);
                                    extraclass = extraclass + " BackgroundImage " + page.BackgroundImageAssetId;
                                    isBackgroundImage = true;
                                }
                                else if (page.BackgroundImageURL != null && page.BackgroundImageURL != string.Empty)
                                {
                                    pageHeaderHtml = pageHeaderHtml.Replace("{{BackgroundImage}}", "style='background: url(" + page.BackgroundImageURL + ")'");
                                    isBackgroundImage = true;
                                }
                                else
                                {
                                    //Background image will set at child div of current div tag
                                    pageHeaderHtml = pageHeaderHtml.Replace("{{BackgroundImage}}", string.Empty);
                                }
                            }
                            else
                            {
                                pageHeaderHtml = pageHeaderHtml.Replace("{{BackgroundImage}}", string.Empty);
                            }
                            //htmlString.Append(pageHeaderHtml.Replace("{{DivId}}", tabClassName).Replace("{{ExtraClass}}", extraclass));

                            htmlString.Append(HtmlConstants.PAGE_TAB_CONTENT_HEADER);
                            if (page.PageTypeName == HtmlConstants.SAVING_ACCOUNT_PAGE || page.PageTypeName == HtmlConstants.CURRENT_ACCOUNT_PAGE)
                            {
                                htmlString.Append("<ul class='nav nav-tabs' style='margin-top:-5px;'>");
                                htmlString.Append("<li class='nav-item active'><a id='tab1-tab' data-toggle='tab' " + "data-target='#" + (page.PageTypeName ==
                                    HtmlConstants.SAVING_ACCOUNT_PAGE ? "Saving" : "Current") + "-' role='tab' class='nav-link active'> Account - 6789</a></li>");
                                htmlString.Append("</ul>");

                                var divClass = string.Empty;
                                var styleTag = string.Empty;
                                if (page.BackgroundImageAssetId != 0)
                                {
                                    divClass = divClass + " BackgroundImage " + page.BackgroundImageAssetId;
                                    isBackgroundImage = true;
                                }
                                else if (page.BackgroundImageURL != null && page.BackgroundImageURL != string.Empty)
                                {
                                    styleTag = styleTag + " style='background: url(" + page.BackgroundImageURL + ")'";
                                    isBackgroundImage = true;
                                }

                                htmlString.Append("<div id='" + (page.PageTypeName == HtmlConstants.SAVING_ACCOUNT_PAGE ? "Saving" : "Current") + "-6789' " +
                                    "class='" + divClass + "' " + styleTag + ">");
                            }

                            int tempRowWidth = 0; // variable to check col-lg div length (bootstrap)
                            int max = 0;
                            if (page.PageWidgets.Count > 0)
                            {
                                var completelst = new List<PageWidget>(page.PageWidgets);
                                var dynamicwidgetids = string.Join(", ", completelst.Where(item => item.IsDynamicWidget).ToList().Select(item => item.WidgetId));
                                DynamicWidgetSearchParameter dynamicWidgetSearchParameter = new DynamicWidgetSearchParameter
                                {
                                    Identifier = dynamicwidgetids,
                                    PagingParameter = new PagingParameter
                                    {
                                        PageIndex = 0,
                                        PageSize = 0,
                                    },
                                    SortParameter = new SortParameter()
                                    {
                                        SortOrder = SortOrder.Ascending,
                                        SortColumn = "Title",
                                    },
                                    SearchMode = SearchMode.Contains
                                };
                                var dynaWidgets = this.dynamicWidgetManager.GetDynamicWidgets(dynamicWidgetSearchParameter, tenantCode);

                                int currentYPosition = 0;
                                var isRowComplete = false;
                                while (completelst.Count != 0)
                                {
                                    var lst = completelst.Where(it => it.Yposition == currentYPosition).ToList();
                                    if (lst.Count > 0)
                                    {
                                        max = max + lst.Max(it => it.Height);
                                        var _lst = completelst.Where(it => it.Yposition < max && it.Yposition != currentYPosition).ToList();
                                        var mergedlst = lst.Concat(_lst).OrderBy(it => it.Xposition).ToList();
                                        currentYPosition = max;
                                        for (int i = 0; i < mergedlst.Count; i++)
                                        {
                                            if (tempRowWidth == 0)
                                            {
                                                htmlString.Append("<div class='row pt-2'>"); // to start new row class div 
                                                isRowComplete = false;
                                            }
                                            int divLength = mergedlst[i].Width;
                                            var divHeight = mergedlst[i].Height * 110 + "px";
                                            tempRowWidth = tempRowWidth + divLength;

                                            // If current col-lg class length is greater than 12, 
                                            //then end parent row class div and then start new row class div
                                            if (tempRowWidth > 12)
                                            {
                                                tempRowWidth = divLength;
                                                htmlString.Append("</div>"); // to end row class div
                                                htmlString.Append("<div class='row pt-2'>"); // to start new row class div
                                                isRowComplete = false;
                                            }

                                            var leftPaddingClass = i != 0 ? " pl-0" : string.Empty;
                                            htmlString.Append("<div class='col-lg-" + divLength + leftPaddingClass + "'>");

                                            if (!mergedlst[i].IsDynamicWidget)
                                            {
                                                if (mergedlst[i].WidgetName == HtmlConstants.CUSTOMER_INFORMATION_WIDGET_NAME)
                                                {
                                                    string customerInfoJson = "{'FirstName':'Laura','MiddleName':'J','LastName':'Donald','AddressLine1': '4000 Executive Parkway', 'AddressLine2':'Saint Globin Rd','City':'Canary Wharf', 'State':'London', 'Country':'England','Zip':'E14 9RZ'}";
                                                    if (customerInfoJson != string.Empty && validationEngine.IsValidJson(customerInfoJson))
                                                    {
                                                        CustomerInformation customerInfo = JsonConvert.DeserializeObject<CustomerInformation>(customerInfoJson);
                                                        var customerHtmlWidget = HtmlConstants.CUSTOMER_INFORMATION_WIDGET_HTML.Replace("{{VideoSource}}", "assets/images/SampleVideo.mp4");
                                                        customerHtmlWidget = customerHtmlWidget.Replace("{{WidgetDivHeight}}", divHeight);

                                                        string customerName = customerInfo.FirstName + " " + customerInfo.SurName;
                                                        customerHtmlWidget = customerHtmlWidget.Replace("{{CustomerName}}", customerName);

                                                        string address1 = customerInfo.AddressLine1 + ", " + customerInfo.AddressLine2 + ", ";
                                                        customerHtmlWidget = customerHtmlWidget.Replace("{{Address1}}", address1);

                                                        string address2 = customerInfo.AddressLine3 + ", " + customerInfo.AddressLine4 + ", ";
                                                        customerHtmlWidget = customerHtmlWidget.Replace("{{Address2}}", address2);

                                                        htmlString.Append(customerHtmlWidget);
                                                    }
                                                }
                                                else if (mergedlst[i].WidgetName == HtmlConstants.PAYMENT_SUMMARY_WIDGET_NAME)
                                                {
                                                    string paymentInfoJson = "{Reg_ID : 1,Start_Date : '2023-01-01',End_Date : '2023-01-01',Request_DateTime : 'DummyText1',ID : '124529534',Intermediary_Code : 'DummyText1',FSP_ID : 'DummyText1',Policy_Number : 'DummyText1',FSP_Party_ID : 'DummyText1',Client_Number : '124556686',FSP_REF : '2452953',Client_Name : 'Mr SCHOELER',Int_ID : 'DummyText1',Product_Type : 'DummyText1',Commission_Amount : 'DummyText1',INT_EXT_REF : '124411745',Int_Name : 'Kruger Van Heerden',Int_Type : 'DummyText1',Policy_Ref : '5596100',Member_Ref : '124556686',Member_Name : 'DummyText1',Transaction_Amount : 'DummyText1',Mem_Age : 'DummyText1',Months_In_Force : 'DummyText1',Commission_Type : 'Safe Custody Fee',Description : 'Safe Custody Service Fee',POSTED_DATE : '2023-03-03',AE_Type_ID : 'DummyText1',AE_Amount : 'DummyText1',DR_CR : 'DummyText1',NAME : 'DummyText1',Member_Surname : 'DummyText1',Jurisdiction : 'DummyText1',Sales_Office : 'DummyText1',FSP_Name : 'Miss Yvonne van Heerden',FSP_Trading_Name : 'T/A Yvonne Van Heerden Financial Planner CC',FSP_Ext_Ref : '124529534',FSP_Kind : 'DummyText1',  		FSP_VAT_Number : '2452953',Product : 'DummyText1',Prod_Group : 'Service Fee',Prod_Seq : 'DummyText1',Report_Seq : 'DummyText1',TYPE : 'DummyText1',Display_Amount : '17.55',VAT_Amount : '38001.27',Earning_Amount : '256670.66',Payment_Amount : 'DummyText1',Business_Type : 'DummyText1',Lifecycle_Description : 'DummyText1',Lifecycle_Start_Date : 'DummyText1',AE_Scheduler_ID : 'DummyText1',VAT_Amount_1 : 'DummyText1',Final_Amount : 'DummyText1'}";
                                                    if (paymentInfoJson != string.Empty && validationEngine.IsValidJson(paymentInfoJson))
                                                    {
                                                        spIAA_PaymentDetail paymentInfo = JsonConvert.DeserializeObject<spIAA_PaymentDetail>(paymentInfoJson);
                                                        var paymentHtmlWidget = HtmlConstants.PAYMENT_SUMMARY_WIDGET_HTML;
                                                        paymentHtmlWidget = paymentHtmlWidget.Replace("{{WidgetDivHeight}}", divHeight);
                                                        paymentHtmlWidget = paymentHtmlWidget.Replace("{{IntTotal}}", "R" + paymentInfo.Earning_Amount);
                                                        paymentHtmlWidget = paymentHtmlWidget.Replace("{{Vat}}", "R" + paymentInfo.VAT_Amount);
                                                        paymentHtmlWidget = paymentHtmlWidget.Replace("{{TotalDue}}", "R" + (Convert.ToDouble(paymentInfo.Earning_Amount) + Convert.ToDouble(paymentInfo.VAT_Amount)).ToString());

                                                        // Format the date to month-year format                                                   
                                                        paymentHtmlWidget = paymentHtmlWidget.Replace("{{IntTotalDate}}", paymentInfo.POSTED_DATE.ToString("MMMM yyyy"));
                                                        // Format the date with a custom format
                                                        string formattedOrdinalDate = FormatDateWithOrdinal(paymentInfo.POSTED_DATE);
                                                        paymentHtmlWidget = paymentHtmlWidget.Replace("{{IntPostedDate}}", formattedOrdinalDate);


                                                        htmlString.Append(paymentHtmlWidget);
                                                    }
                                                }

                                                else if (mergedlst[i].WidgetName == HtmlConstants.PPS_HEADING_WIDGET_NAME)
                                                {
                                                    string headingInfoJson = "{Reg_ID : 1,Start_Date : '2023-01-01',End_Date : '2023-01-01',Request_DateTime : 'DummyText1',ID : '124529534',Intermediary_Code : 'DummyText1',FSP_ID : 'DummyText1',Policy_Number : 'DummyText1',FSP_Party_ID : 'DummyText1',Client_Number : '124556686',FSP_REF : '2452953',Client_Name : 'Mr SCHOELER',Int_ID : 'DummyText1',Product_Type : 'DummyText1',Commission_Amount : 'DummyText1',INT_EXT_REF : '124411745',Int_Name : 'Kruger Van Heerden',Int_Type : 'DummyText1',Policy_Ref : '5596100',Member_Ref : '124556686',Member_Name : 'DummyText1',Transaction_Amount : 'DummyText1',Mem_Age : 'DummyText1',Months_In_Force : 'DummyText1',Commission_Type : 'Safe Custody Fee',Description : 'Safe Custody Service Fee',POSTED_DATE : '2023-03-03',AE_Type_ID : 'DummyText1',AE_Amount : 'DummyText1',DR_CR : 'DummyText1',NAME : 'DummyText1',Member_Surname : 'DummyText1',Jurisdiction : 'DummyText1',Sales_Office : 'DummyText1',FSP_Name : 'Miss Yvonne van Heerden',FSP_Trading_Name : 'T/A Yvonne Van Heerden Financial Planner CC',FSP_Ext_Ref : '124529534',FSP_Kind : 'DummyText1',  		FSP_VAT_Number : '2452953',Product : 'DummyText1',Prod_Group : 'Service Fee',Prod_Seq : 'DummyText1',Report_Seq : 'DummyText1',TYPE : 'DummyText1',Display_Amount : '17.55',VAT_Amount : '38001.27',Earning_Amount : '256670.66',Payment_Amount : 'DummyText1',Business_Type : 'DummyText1',Lifecycle_Description : 'DummyText1',Lifecycle_Start_Date : 'DummyText1',AE_Scheduler_ID : 'DummyText1',VAT_Amount_1 : 'DummyText1',Final_Amount : 'DummyText1'}";
                                                    if (headingInfoJson != string.Empty && validationEngine.IsValidJson(headingInfoJson))
                                                    {
                                                        spIAA_PaymentDetail headingInfo = JsonConvert.DeserializeObject<spIAA_PaymentDetail>(headingInfoJson);
                                                        var headingHtmlWidget = HtmlConstants.PPS_HEADING_WIDGET_HTML;
                                                        headingHtmlWidget = headingHtmlWidget.Replace("{{WidgetDivHeight}}", divHeight);

                                                        headingHtmlWidget = headingHtmlWidget.Replace("{{FSPName}}", headingInfo.FSP_Name);
                                                        headingHtmlWidget = headingHtmlWidget.Replace("{{FSPTradingName}}", headingInfo.FSP_Trading_Name);

                                                        htmlString.Append(headingHtmlWidget);
                                                    }
                                                }

                                                else if (mergedlst[i].WidgetName == HtmlConstants.PPS_DETAILS_WIDGET_NAME)
                                                {
                                                    string ppsDetailsInfoJson = "{Reg_ID : 1,Start_Date : '2023-01-01',End_Date : '2023-01-01',Request_DateTime : 'DummyText1',ID : '124529534',Intermediary_Code : 'DummyText1',FSP_ID : 'DummyText1',Policy_Number : 'DummyText1',FSP_Party_ID : 'DummyText1',Client_Number : '124556686',FSP_REF : '2452953',Client_Name : 'Mr SCHOELER',Int_ID : 'DummyText1',Product_Type : 'DummyText1',Commission_Amount : 'DummyText1',INT_EXT_REF : '124411745',Int_Name : 'Kruger Van Heerden',Int_Type : 'DummyText1',Policy_Ref : '5596100',Member_Ref : '124556686',Member_Name : 'DummyText1',Transaction_Amount : 'DummyText1',Mem_Age : 'DummyText1',Months_In_Force : 'DummyText1',Commission_Type : 'Safe Custody Fee',Description : 'Safe Custody Service Fee',POSTED_DATE : '2023-03-03',AE_Type_ID : 'DummyText1',AE_Amount : 'DummyText1',DR_CR : 'DummyText1',NAME : 'DummyText1',Member_Surname : 'DummyText1',Jurisdiction : 'DummyText1',Sales_Office : 'DummyText1',FSP_Name : 'Miss Yvonne van Heerden',FSP_Trading_Name : 'T/A Yvonne Van Heerden Financial Planner CC',FSP_Ext_Ref : '124529534',FSP_Kind : 'DummyText1',  		FSP_VAT_Number : '2452953',Product : 'DummyText1',Prod_Group : 'Service Fee',Prod_Seq : 'DummyText1',Report_Seq : 'DummyText1',TYPE : 'DummyText1',Display_Amount : '17.55',VAT_Amount : '38001.27',Earning_Amount : '256670.66',Payment_Amount : 'DummyText1',Business_Type : 'DummyText1',Lifecycle_Description : 'DummyText1',Lifecycle_Start_Date : 'DummyText1',AE_Scheduler_ID : 'DummyText1',VAT_Amount_1 : 'DummyText1',Final_Amount : 'DummyText1'}";
                                                    if (ppsDetailsInfoJson != string.Empty && validationEngine.IsValidJson(ppsDetailsInfoJson))
                                                    {
                                                        spIAA_PaymentDetail ppsDetailsInfo = JsonConvert.DeserializeObject<spIAA_PaymentDetail>(ppsDetailsInfoJson);
                                                        var ppsDetailsHtmlWidget = HtmlConstants.PPS_DETAILS_WIDGET_HTML;
                                                        ppsDetailsHtmlWidget = ppsDetailsHtmlWidget.Replace("{{WidgetDivHeight}}", divHeight);

                                                        ppsDetailsHtmlWidget = ppsDetailsHtmlWidget.Replace("{{FSPNumber}}", ppsDetailsInfo.FSP_Ext_Ref);
                                                        ppsDetailsHtmlWidget = ppsDetailsHtmlWidget.Replace("{{FSPAgreeNumber}}", ppsDetailsInfo.FSP_REF);
                                                        ppsDetailsHtmlWidget = ppsDetailsHtmlWidget.Replace("{{VATRegNumber}}", ppsDetailsInfo.FSP_VAT_Number);

                                                        htmlString.Append(ppsDetailsHtmlWidget);
                                                    }
                                                }
                                                else if (mergedlst[i].WidgetName == HtmlConstants.PRODUCT_SUMMARY_WIDGET_NAME)
                                                {                                                    
                                                    // JSON representation of product summary list
                                                    string productSummaryListJson = "[{ 'Commission_Type': 'Safe Custody Fee', 'Prod_Group': 'Safe Custody Fee', 'AE_Amount': '52.65', 'QueryLink': 'https://www.google.com/', 'DR_CR': 'DR'}, { 'Commission_Type': 'Safe Custody Fee', 'Prod_Group': 'Service Fee', 'AE_Amount': '52.66', 'QueryLink': 'https://www.google.com/', 'DR_CR': 'DR'}, { 'Commission_Type': 'Safe Custody Fee', 'Prod_Group': 'Safe Custody Fee', 'AE_Amount': '152.67', 'QueryLink': 'https://www.google.com/', 'DR_CR': 'DR'}, { 'Commission_Type': 'Safe Custody Fee', 'Prod_Group': 'Service Fee', 'AE_Amount': '52.68', 'QueryLink': 'https://www.google.com/', 'DR_CR': 'CR'}]";

                                                    // Check if productSummaryListJson is not empty and is valid JSON
                                                    if (productSummaryListJson != string.Empty && validationEngine.IsValidJson(productSummaryListJson))
                                                    {
                                                        // Deserialize JSON to a list of payment details
                                                        IList<spIAA_PaymentDetail> productSummary = JsonConvert.DeserializeObject<List<spIAA_PaymentDetail>>(productSummaryListJson);
                                                        StringBuilder productSummarySrc = new StringBuilder();

                                                        // Grouping records by Due date
                                                        var gpCommissionTypeRecords = productSummary.GroupBy(gpCommissionTypeItem => new { gpCommissionTypeItem.Commission_Type }).ToList();

                                                        // Grouping records by Fiduciary fees
                                                        var prdocutDescriptionRecords = productSummary.GroupBy(gpPrdocutDescriptionItem => new { gpPrdocutDescriptionItem.Prod_Group }).ToList();

                                                        // Initializing variables for column sums
                                                        long index = 1;
                                                        var aeAmountColSum = 0.00;
                                                        var aeAmountColSumR = "";

                                                        // Iterating through Due date groups
                                                        gpCommissionTypeRecords.ForEach(gpCommissionTypeItem =>
                                                        {
                                                            // Iterating through Fiduciary fees groups
                                                            prdocutDescriptionRecords.ForEach(gpPrdocutDescriptionItem =>
                                                            {
                                                                // Calculate sums for CR and DR amounts
                                                                var aeAmountCRSum = productSummary
                                                                    .Where(witem => ((witem.Commission_Type == gpCommissionTypeItem.Key.Commission_Type) &&
                                                                                     (witem.DR_CR == "CR"))).Sum(item => Convert.ToDouble(item.AE_Amount));

                                                                var aeAmountDRSum = productSummary
                                                                    .Where(witem => ((witem.Commission_Type == gpCommissionTypeItem.Key.Commission_Type) &&
                                                                                     (witem.DR_CR == "DR"))).Sum(item => Convert.ToDouble(item.AE_Amount));

                                                                // Calculate total AE Amount
                                                                var aeAmountSum = aeAmountCRSum - aeAmountDRSum;
                                                                var aeAmountSumR = CommonUtility.concatRWithDouble(aeAmountSum.ToString());

                                                                // Append the table row to productSummarySrc
                                                                productSummarySrc.Append("<tr><td align='center' valign='center' class='px-1 py-1 fsp-bdr-right fsp-bdr-bottom'>" + index + "</td><td class='fsp-bdr-right text-left fsp-bdr-bottom px-1'>" + gpCommissionTypeItem.Key.Commission_Type + "</td>" + "<td class='fsp-bdr-right text-left fsp-bdr-bottom px-1'> " + (gpPrdocutDescriptionItem.Key.Prod_Group == "Service Fee" ? "Premium Under Advise Fee" : gpPrdocutDescriptionItem.Key.Prod_Group) + "</td> <td class='text-right fsp-bdr-right fsp-bdr-bottom px-1'>" + aeAmountSumR + "</td></tr>");

                                                                // Update column sum and increment index
                                                                aeAmountColSum += aeAmountSum;
                                                                productSummarySrc.Append("</tr>");
                                                                index++;
                                                            });
                                                            aeAmountColSumR = (aeAmountColSum == 0) ? "R0.00" : Utility.FormatCurrency(aeAmountColSum.ToString());
                                                        });

                                                        // Generate the HTML string for the product summary widget
                                                        string productSumstring = HtmlConstants.PRODUCT_SUMMARY_WIDGET_HTML.Replace("{{ProductSummary}}", productSummarySrc.ToString());
                                                        string productInfoJson = "{VAT_Amount : '38001.27'}";
                                                        spIAA_PaymentDetail productInfo = JsonConvert.DeserializeObject<spIAA_PaymentDetail>(productInfoJson);

                                                        // Replace placeholders in the HTML string with actual values
                                                        productSumstring = productSumstring.Replace("{{WidgetDivHeight}}", divHeight);
                                                        productSumstring = productSumstring.Replace("{{QueryBtn}}", "assets/images/IfQueryBtn.jpg");
                                                        productSumstring = productSumstring.Replace("{{ProductTotalDue}}", aeAmountColSumR);
                                                        productSumstring = productSumstring.Replace("{{VATDue}}", CommonUtility.concatRWithDouble(productInfo.VAT_Amount.ToString()));

                                                        // Calculate grand total due
                                                        double grandTotalDue = (Convert.ToDouble(aeAmountColSum) + Convert.ToDouble(productInfo.VAT_Amount));
                                                        var grandTotalDueR = CommonUtility.concatRWithDouble(grandTotalDue.ToString());
                                                        productSumstring = productSumstring.Replace("{{GrandTotalDue}}", grandTotalDueR);

                                                        // Calculate PPS payment and update the HTML string
                                                        double ppsPayment = grandTotalDue;
                                                        var ppsPaymentR = (ppsPayment == 0) ? "R0.00" : Utility.FormatCurrency(ppsPayment.ToString());
                                                        productSumstring = productSumstring.Replace("{{PPSPayment}}", ppsPaymentR);

                                                        // Calculate and update the balance in the HTML string
                                                        var balance = Convert.ToDouble((grandTotalDue - ppsPayment)).ToString("F2");
                                                        // Calculate and update the balance in the HTML string
                                                        productSumstring = productSumstring.Replace("{{Balance}}", CommonUtility.concatRWithDouble(balance));

                                                        // Append the modified product summary widget HTML to the main HTML string
                                                        htmlString.Append(productSumstring);
                                                    }
                                                }

                                                else if (mergedlst[i].WidgetName == HtmlConstants.EARNINGS_FOR_PERIOD_WIDGET_NAME)
                                                {
                                                    string commisionDetailListJson = "[{'Request_ID':1338675045,'AE_TYPE_ID':542051,'INT_EXT_REF':124565256,'POLICY_REF':3830102,'MEMBER_REF':10024365,'Member_Name':'Dr JC Arthur','BUS_GROUP':'PPS INSURANCE','PRODUCT_DESCRIPTION':'Professional Health Provider Whole Life Level Rated Professional Health CI 100 Cover','OID':542053,'MeasureType':'Commission','CommissionType':'2nd Year','TRANSACTION_AMOUNT':344.73,'ALLOCATED_AMOUNT':1172.08,'MEMBER_AGE':45,'MONTHS_IN_FORCE':0,'REQUEST_DATETIME':'2022-09-23 00:00:00.000','REQUESTED_DATETIME':'2022-11-01 00:00:00.000','AE_agmt_id':4755496,'AE_Posted_Date':'2022-09-23 00:00:00.000','AE_Amount':-1172.08,'Acc_Name':'Future Dated Provisions Account','FSP_Name':'NULL','DUE_DATE':'2023-10-07 00:00:00.000','YEAR_START_DATE':'2022-01-01 00:00:00.000','YEAR_END_DATE':'2022-12-31 23:59:00.000','Type':'Policy_Data'},{'Request_ID':1302220959,'AE_TYPE_ID':542051,'INT_EXT_REF':124565256,'POLICY_REF':3830102,'MEMBER_REF':10024365,'Member_Name':'Dr JC Arthur','BUS_GROUP':'PPS INSURANCE','PRODUCT_DESCRIPTION':'Professional Health Provider Whole Life Level Rated Professional Health CI 100 Cover','OID':542053,'MeasureType':'Commission','CommissionType':'1st Year','TRANSACTION_AMOUNT':344.73,'ALLOCATED_AMOUNT':1172.08,'MEMBER_AGE':45,'MONTHS_IN_FORCE':0,'REQUEST_DATETIME':'2022-09-23 00:00:00.000','REQUESTED_DATETIME':'2022-11-01 00:00:00.000','AE_agmt_id':4755496,'REQUESTED_DATETIME':'2022-09-23 00:00:00.000','AE_Amount':1172.08,'Acc_Name':'Future Dated Provisions Account','FSP_Name':'NULL','DUE_DATE':'2023-10-07 00:00:00.000','YEAR_START_DATE':'2022-01-01 00:00:00.000','YEAR_END_DATE':'2022-12-31 23:59:00.000','Type':'Policy_Data'},{'Request_ID':1338674952,'AE_TYPE_ID':541901,'INT_EXT_REF':124565256,'POLICY_REF':3820110,'MEMBER_REF':10436136,'Member_Name':'Mnr JG Rossouw','BUS_GROUP':'PPS INSURANCE','PRODUCT_DESCRIPTION':'Professional Health Provider Whole Life Level Rated Professional Health','OID':541991,'MeasureType':'Commission','CommissionType':'2nd Year','TRANSACTION_AMOUNT':928.89,'ALLOCATED_AMOUNT':9474.68,'MEMBER_AGE':43,'MONTHS_IN_FORCE':0,'REQUEST_DATETIME':'2022-10-28 00:00:00.000','REQUESTED_DATETIME':'2022-11-01 00:00:00.000','AE_agmt_id':4755496,'REQUESTED_DATETIME':'2022-10-28 00:00:00.000','AE_Amount':-9474.68,'Acc_Name':'Future Dated Provisions Account','FSP_Name':'NULL','DUE_DATE':'2022-10-07 00:00:00.000','YEAR_START_DATE':'2022-01-01 00:00:00.000','YEAR_END_DATE':'2022-12-31 23:59:00.000','Type':'Policy_Data'},{'Request_ID':1338675128,'AE_TYPE_ID':541901,'INT_EXT_REF':124565256,'POLICY_REF':3820110,'MEMBER_REF':10436136,'Member_Name':'Mnr JG Rossouw','BUS_GROUP':'PPS INSURANCE','PRODUCT_DESCRIPTION':'Professional Health Provider Whole Life Level Rated Professional Health','OID':541991,'MeasureType':'Commission','CommissionType':'1st Year','TRANSACTION_AMOUNT':928.89,'ALLOCATED_AMOUNT':6072.47,'MEMBER_AGE':43,'MONTHS_IN_FORCE':0,'REQUEST_DATETIME':'2022-10-28 00:00:00.000','REQUESTED_DATETIME':'2022-11-01 00:00:00.000','AE_agmt_id':4755496,'REQUESTED_DATETIME':'2022-09-28 00:00:00.000','AE_Amount':6072.47,'Acc_Name':'Future Dated Provisions Account','FSP_Name':'NULL','DUE_DATE':'2022-10-07 00:00:00.000','YEAR_START_DATE':'2022-01-01 00:00:00.000','YEAR_END_DATE':'2022-12-31 23:59:00.000','Type':'Policy_Data'},{'Request_ID':1338675045,'AE_TYPE_ID':542051,'INT_EXT_REF':124565256,'POLICY_REF':3830102,'MEMBER_REF':10024365,'Member_Name':'Dr JC Arthur','BUS_GROUP':'PPS INSURANCE','PRODUCT_DESCRIPTION':'Professional Health Provider Whole Life Level Rated Professional Health CI 100 Cover','OID':542053,'MeasureType':'Commission','CommissionType':'2nd Year','TRANSACTION_AMOUNT':344.73,'ALLOCATED_AMOUNT':1500.00,'MEMBER_AGE':45,'MONTHS_IN_FORCE':0,'REQUEST_DATETIME':'2022-09-23 00:00:00.000','REQUESTED_DATETIME':'2022-11-01 00:00:00.000','AE_agmt_id':4755496,'AE_Posted_Date':'2022-09-23 00:00:00.000','AE_Amount':-1172.08,'Acc_Name':'Future Dated Provisions Account','FSP_Name':'NULL','DUE_DATE':'2023-10-07 00:00:00.000','YEAR_START_DATE':'2022-01-01 00:00:00.000','YEAR_END_DATE':'2022-12-31 23:59:00.000','Type':'Policy_Data'},{'Request_ID':1338675045,'AE_TYPE_ID':542051,'INT_EXT_REF':124565256,'POLICY_REF':3830102,'MEMBER_REF':10024365,'Member_Name':'Dr JC Arthur','BUS_GROUP':'PPS INSURANCE','PRODUCT_DESCRIPTION':'Professional Health Provider Whole Life Level Rated Professional Health CI 100 Cover','OID':542053,'MeasureType':'Commission','CommissionType':'1st Year','TRANSACTION_AMOUNT':344.73,'ALLOCATED_AMOUNT':1000.00,'MEMBER_AGE':45,'MONTHS_IN_FORCE':0,'REQUEST_DATETIME':'2022-09-23 00:00:00.000','REQUESTED_DATETIME':'2022-11-01 00:00:00.000','AE_agmt_id':4755496,'AE_Posted_Date':'2022-09-23 00:00:00.000','AE_Amount':-1172.08,'Acc_Name':'Future Dated Provisions Account','FSP_Name':'NULL','DUE_DATE':'2023-10-07 00:00:00.000','YEAR_START_DATE':'2022-01-01 00:00:00.000','YEAR_END_DATE':'2022-12-31 23:59:00.000','Type':'Policy_Data'},{'Request_ID':1338675045,'AE_TYPE_ID':542051,'INT_EXT_REF':124565256,'POLICY_REF':3830102,'MEMBER_REF':10024365,'Member_Name':'Dr JC Arthur','BUS_GROUP':'PPS INSURANCE','PRODUCT_DESCRIPTION':'Professional Health Provider Whole Life Level Rated Professional Health CI 100 Cover','OID':542053,'MeasureType':'Commission','CommissionType':'2nd Year','TRANSACTION_AMOUNT':344.73,'ALLOCATED_AMOUNT':1800.00,'MEMBER_AGE':45,'MONTHS_IN_FORCE':0,'REQUEST_DATETIME':'2022-10-23 00:00:00.000','REQUESTED_DATETIME':'2022-11-01 00:00:00.000','AE_agmt_id':4755496,'AE_Posted_Date':'2022-09-23 00:00:00.000','AE_Amount':-1172.08,'Acc_Name':'Future Dated Provisions Account','FSP_Name':'NULL','DUE_DATE':'2023-10-07 00:00:00.000','YEAR_START_DATE':'2022-01-01 00:00:00.000','YEAR_END_DATE':'2022-12-31 23:59:00.000','Type':'Policy_Data'},{'Request_ID':1338675045,'AE_TYPE_ID':542051,'INT_EXT_REF':124565256,'POLICY_REF':3830102,'MEMBER_REF':10024365,'Member_Name':'Dr JC Arthur','BUS_GROUP':'PPS INSURANCE','PRODUCT_DESCRIPTION':'Professional Health Provider Whole Life Level Rated Professional Health CI 100 Cover','OID':542053,'MeasureType':'Commission','CommissionType':'1st Year','TRANSACTION_AMOUNT':344.73,'ALLOCATED_AMOUNT':2000.00,'MEMBER_AGE':45,'MONTHS_IN_FORCE':0,'REQUEST_DATETIME':'2022-10-23 00:00:00.000','REQUESTED_DATETIME':'2022-11-01 00:00:00.000','AE_agmt_id':4755496,'AE_Posted_Date':'2022-09-23 00:00:00.000','AE_Amount':-1172.08,'Acc_Name':'Future Dated Provisions Account','FSP_Name':'NULL','DUE_DATE':'2023-10-07 00:00:00.000','YEAR_START_DATE':'2022-01-01 00:00:00.000','YEAR_END_DATE':'2022-12-31 23:59:00.000','Type':'Policy_Data'},{'Request_ID':1338675045,'AE_TYPE_ID':542051,'INT_EXT_REF':124565256,'POLICY_REF':3830102,'MEMBER_REF':10024365,'Member_Name':'Dr JC Arthur','BUS_GROUP':'PPS INSURANCE','PRODUCT_DESCRIPTION':'Professional Health Provider Whole Life Level Rated Professional Health CI 100 Cover','OID':542053,'MeasureType':'Commission','CommissionType':'2nd Year','TRANSACTION_AMOUNT':344.73,'ALLOCATED_AMOUNT':1172.08,'MEMBER_AGE':45,'MONTHS_IN_FORCE':0,'REQUEST_DATETIME':'2022-09-23 00:00:00.000','REQUESTED_DATETIME':'2022-11-01 00:00:00.000','AE_agmt_id':4755496,'AE_Posted_Date':'2022-09-23 00:00:00.000','AE_Amount':-1172.08,'Acc_Name':'Future Dated Provisions Account','FSP_Name':'NULL','DUE_DATE':'2023-10-07 00:00:00.000','YEAR_START_DATE':'2022-01-01 00:00:00.000','YEAR_END_DATE':'2022-12-31 23:59:00.000','Type':'Fiduciary_Data'},{'Request_ID':1302220959,'AE_TYPE_ID':542051,'INT_EXT_REF':124565256,'POLICY_REF':3830102,'MEMBER_REF':10024365,'Member_Name':'Dr JC Arthur','BUS_GROUP':'PPS INSURANCE','PRODUCT_DESCRIPTION':'Professional Health Provider Whole Life Level Rated Professional Health CI 100 Cover','OID':542053,'MeasureType':'Commission','CommissionType':'1st Year','TRANSACTION_AMOUNT':344.73,'ALLOCATED_AMOUNT':1172.08,'MEMBER_AGE':45,'MONTHS_IN_FORCE':0,'REQUEST_DATETIME':'2022-09-23 00:00:00.000','REQUESTED_DATETIME':'2022-11-01 00:00:00.000','AE_agmt_id':4755496,'REQUESTED_DATETIME':'2022-09-23 00:00:00.000','AE_Amount':1172.08,'Acc_Name':'Future Dated Provisions Account','FSP_Name':'NULL','DUE_DATE':'2023-10-07 00:00:00.000','YEAR_START_DATE':'2022-01-01 00:00:00.000','YEAR_END_DATE':'2022-12-31 23:59:00.000','Type':'Fiduciary_Data'},{'Request_ID':1338674952,'AE_TYPE_ID':541901,'INT_EXT_REF':124565256,'POLICY_REF':3820110,'MEMBER_REF':10436136,'Member_Name':'Mnr JG Rossouw','BUS_GROUP':'PPS INSURANCE','PRODUCT_DESCRIPTION':'Professional Health Provider Whole Life Level Rated Professional Health','OID':541991,'MeasureType':'Commission','CommissionType':'2nd Year','TRANSACTION_AMOUNT':928.89,'ALLOCATED_AMOUNT':9474.68,'MEMBER_AGE':43,'MONTHS_IN_FORCE':0,'REQUEST_DATETIME':'2022-10-28 00:00:00.000','REQUESTED_DATETIME':'2022-11-01 00:00:00.000','AE_agmt_id':4755496,'REQUESTED_DATETIME':'2022-10-28 00:00:00.000','AE_Amount':-9474.68,'Acc_Name':'Future Dated Provisions Account','FSP_Name':'NULL','DUE_DATE':'2022-10-07 00:00:00.000','YEAR_START_DATE':'2022-01-01 00:00:00.000','YEAR_END_DATE':'2022-12-31 23:59:00.000','Type':'Fiduciary_Data'},{'Request_ID':1338675128,'AE_TYPE_ID':541901,'INT_EXT_REF':124565256,'POLICY_REF':3820110,'MEMBER_REF':10436136,'Member_Name':'Mnr JG Rossouw','BUS_GROUP':'PPS INSURANCE','PRODUCT_DESCRIPTION':'Professional Health Provider Whole Life Level Rated Professional Health','OID':541991,'MeasureType':'Commission','CommissionType':'1st Year','TRANSACTION_AMOUNT':928.89,'ALLOCATED_AMOUNT':6072.47,'MEMBER_AGE':43,'MONTHS_IN_FORCE':0,'REQUEST_DATETIME':'2022-10-28 00:00:00.000','REQUESTED_DATETIME':'2022-11-01 00:00:00.000','AE_agmt_id':4755496,'REQUESTED_DATETIME':'2022-09-28 00:00:00.000','AE_Amount':6072.47,'Acc_Name':'Future Dated Provisions Account','FSP_Name':'NULL','DUE_DATE':'2022-10-07 00:00:00.000','YEAR_START_DATE':'2022-01-01 00:00:00.000','YEAR_END_DATE':'2022-12-31 23:59:00.000','Type':'Fiduciary_Data'},{'Request_ID':1338675045,'AE_TYPE_ID':542051,'INT_EXT_REF':124565256,'POLICY_REF':3830102,'MEMBER_REF':10024365,'Member_Name':'Dr JC Arthur','BUS_GROUP':'PPS INSURANCE','PRODUCT_DESCRIPTION':'Professional Health Provider Whole Life Level Rated Professional Health CI 100 Cover','OID':542053,'MeasureType':'Commission','CommissionType':'2nd Year','TRANSACTION_AMOUNT':344.73,'ALLOCATED_AMOUNT':1500.00,'MEMBER_AGE':45,'MONTHS_IN_FORCE':0,'REQUEST_DATETIME':'2022-09-23 00:00:00.000','REQUESTED_DATETIME':'2022-11-01 00:00:00.000','AE_agmt_id':4755496,'AE_Posted_Date':'2022-09-23 00:00:00.000','AE_Amount':-1172.08,'Acc_Name':'Future Dated Provisions Account','FSP_Name':'NULL','DUE_DATE':'2023-10-07 00:00:00.000','YEAR_START_DATE':'2022-01-01 00:00:00.000','YEAR_END_DATE':'2022-12-31 23:59:00.000','Type':'Fiduciary_Data'},{'Request_ID':1338675045,'AE_TYPE_ID':542051,'INT_EXT_REF':124565256,'POLICY_REF':3830102,'MEMBER_REF':10024365,'Member_Name':'Dr JC Arthur','BUS_GROUP':'PPS INSURANCE','PRODUCT_DESCRIPTION':'Professional Health Provider Whole Life Level Rated Professional Health CI 100 Cover','OID':542053,'MeasureType':'Commission','CommissionType':'1st Year','TRANSACTION_AMOUNT':344.73,'ALLOCATED_AMOUNT':1000.00,'MEMBER_AGE':45,'MONTHS_IN_FORCE':0,'REQUEST_DATETIME':'2022-09-23 00:00:00.000','REQUESTED_DATETIME':'2022-11-01 00:00:00.000','AE_agmt_id':4755496,'AE_Posted_Date':'2022-09-23 00:00:00.000','AE_Amount':-1172.08,'Acc_Name':'Future Dated Provisions Account','FSP_Name':'NULL','DUE_DATE':'2023-10-07 00:00:00.000','YEAR_START_DATE':'2022-01-01 00:00:00.000','YEAR_END_DATE':'2022-12-31 23:59:00.000','Type':'Fiduciary_Data'},{'Request_ID':1338675045,'AE_TYPE_ID':542051,'INT_EXT_REF':124565256,'POLICY_REF':3830102,'MEMBER_REF':10024365,'Member_Name':'Dr JC Arthur','BUS_GROUP':'PPS INSURANCE','PRODUCT_DESCRIPTION':'Professional Health Provider Whole Life Level Rated Professional Health CI 100 Cover','OID':542053,'MeasureType':'Commission','CommissionType':'2nd Year','TRANSACTION_AMOUNT':344.73,'ALLOCATED_AMOUNT':1800.00,'MEMBER_AGE':45,'MONTHS_IN_FORCE':0,'REQUEST_DATETIME':'2022-10-23 00:00:00.000','REQUESTED_DATETIME':'2022-11-01 00:00:00.000','AE_agmt_id':4755496,'AE_Posted_Date':'2022-09-23 00:00:00.000','AE_Amount':-1172.08,'Acc_Name':'Future Dated Provisions Account','FSP_Name':'NULL','DUE_DATE':'2023-10-07 00:00:00.000','YEAR_START_DATE':'2022-01-01 00:00:00.000','YEAR_END_DATE':'2022-12-31 23:59:00.000','Type':'Fiduciary_Data'},{'Request_ID':1338675045,'AE_TYPE_ID':542051,'INT_EXT_REF':124565256,'POLICY_REF':3830102,'MEMBER_REF':10024365,'Member_Name':'Dr JC Arthur','BUS_GROUP':'PPS INSURANCE','PRODUCT_DESCRIPTION':'Professional Health Provider Whole Life Level Rated Professional Health CI 100 Cover','OID':542053,'MeasureType':'Commission','CommissionType':'1st Year','TRANSACTION_AMOUNT':344.73,'ALLOCATED_AMOUNT':2000.00,'MEMBER_AGE':45,'MONTHS_IN_FORCE':0,'REQUEST_DATETIME':'2022-10-23 00:00:00.000','REQUESTED_DATETIME':'2022-11-01 00:00:00.000','AE_agmt_id':4755496,'AE_Posted_Date':'2022-09-23 00:00:00.000','AE_Amount':-1172.08,'Acc_Name':'Future Dated Provisions Account','FSP_Name':'NULL','DUE_DATE':'2023-10-07 00:00:00.000','YEAR_START_DATE':'2022-01-01 00:00:00.000','YEAR_END_DATE':'2022-12-31 23:59:00.000','Type':'Fiduciary_Data'},]";
                                                    double totalMonthlyEarnings = 0;


                                                    if (commisionDetailListJson != string.Empty && validationEngine.IsValidJson(commisionDetailListJson))
                                                    {
                                                        int numColumns = 0;

                                                        // Deserialize JSON string to a list of commission details
                                                        IList<spIAA_Commission_Detail> commisionDetail = JsonConvert.DeserializeObject<List<spIAA_Commission_Detail>>(commisionDetailListJson);

                                                        // Initialize a StringBuilder to store HTML content
                                                        StringBuilder commisionDetailSrc = new StringBuilder();
                                                        string commisionDetailString = HtmlConstants.EARNINGS_FOR_PERIOD_WIDGET_HTML;

                                                        // List to store monthly total amounts for each commission type
                                                        List<List<double>> monthlyTotalList = new List<List<double>>();

                                                        // Group commission details by month and type to get counts
                                                        var records = commisionDetail.GroupBy(gpcommisionitem => new { gpcommisionitem.REQUEST_DATETIME.Month, gpcommisionitem.Type })
                                                            .Select(group => new
                                                            {
                                                                GroupKey = group.Key,
                                                                CountCommissionType = group.Count()
                                                            }).ToList();

                                                        // Group commission details by CommissionType to get a list of commission types
                                                        var gpCommisionType = commisionDetail.OrderBy(item => item.CommissionType).GroupBy(gpcommisionitem => new { gpcommisionitem.CommissionType })
                                                            .Select(group => new
                                                            {
                                                                GroupKey = group.Key
                                                            }).ToList();


                                                        //Deepak*** Start

                                                        // Fees Section: Monthly Fiduciary Fees Summary - End

                                                        // FSP account postings summary start

                                                        // Grouping records by FSP Name
                                                        var fspNameRecords = commisionDetail.GroupBy(gpcommisionitem => new { gpcommisionitem.FSP_Name })
                                                            .Select(group => new
                                                            {
                                                                GroupKey = group.Key,
                                                            }).ToList();

                                                        // Iterating through FSP Name groups
                                                        fspNameRecords.ForEach(gpfspNameItem =>
                                                        {
                                                            // Initializing variables for monthly totals
                                                            List<List<double>> monthlyAEPostedTotalList = new List<List<double>>();
                                                            double totalAEPostedMonthlyEarnings = 0;

                                                            // Grouping records by AE_Posted_Date
                                                            var aePostedRecords = commisionDetail
                                                                .Where(witem => witem.FSP_Name == gpfspNameItem.GroupKey.FSP_Name)
                                                                .GroupBy(gpcommisionitem => new { gpcommisionitem.AE_Posted_Date.Date })
                                                                .Select(group => new
                                                                {
                                                                    GroupKey = group.Key,
                                                                }).ToList();

                                                            // Appending HTML for the earnings section
                                                            commisionDetailSrc.Append("<!--FSPAccountPostingsSummarySection--><div class='earnings-section-monthly d-flex mb-2'><div class='d-flex gap-1 w-100'><!--FSP account postings summary--><div class='col-6'><!--Headingfor FSP Account PostingsSummary--><h4 class='monthly-production-summary skyblue-bg-title text-white text-center'>FSP account postings summary</h4><div class='monthly-table'><!--Table forFSPAccountPostingsSummary--><table width='100%' cellpadding='0' cellspacing='0'><!--TableHeaders--><thead><tr><th style='height:50px' class='text-white font-weight-bold'>Posted Date</th>");

                                                            // Appending HTML for Commission Type headers
                                                            gpCommisionType.ForEach(gpCommisionTypeitem =>
                                                            {
                                                                commisionDetailSrc.Append("<th style='height:50px' class='text-left'> Posted (" + gpCommisionTypeitem.GroupKey.CommissionType + ")</th>");
                                                            });

                                                            // Appending HTML for Premium under advice header
                                                            commisionDetailSrc.Append("<th style='height:50px' class='text-white font-weight-bold'>Premium under advice</th></tr></thead>");

                                                            // Appending HTML for Financial Service Provider row
                                                            //commisionDetailSrc.Append("<tr><th class='text-left text-white font-weight-bold' colspan=" + (2 + gpCommisionType.Count) + ">Financial Service Provider: " + gpfspNameItem.GroupKey.FSP_Name?? "N/A" + "</th><tr>");

                                                            // Iterating through AE_Posted_Date groups
                                                            aePostedRecords.ForEach(gpMonthRangeItem =>
                                                            {
                                                                // Appending HTML for Date and Commission Type columns
                                                                commisionDetailSrc.Append("<tr><td class='text-left'>" + DateTime.Parse(gpMonthRangeItem.GroupKey.Date.ToString()).ToString("dd-MMM-yyyy") + "</td>");
                                                                List<double> innermonthlyAEPostedTotalListSum = new List<double>();

                                                                // Iterating through Commission Types
                                                                gpCommisionType.ForEach(gpCommisionTypeitem =>
                                                                {
                                                                    // Calculating and appending HTML for Premium under advice
                                                                    var premiumUnderAdviceTd1Sum = commisionDetail
                                                                        .Where(witem => witem.AE_Posted_Date.Date == gpMonthRangeItem.GroupKey.Date && witem.CommissionType == gpCommisionTypeitem.GroupKey.CommissionType)
                                                                        .Sum(item => Convert.ToDouble(item.ALLOCATED_AMOUNT));

                                                                    var premiumUnderAdviceTd1SumR = (premiumUnderAdviceTd1Sum == 0) ? "R0.00" : Utility.FormatCurrency(premiumUnderAdviceTd1Sum.ToString());
                                                                    commisionDetailSrc.Append("<td class='text-right'>" + premiumUnderAdviceTd1SumR + "</td>");
                                                                    innermonthlyAEPostedTotalListSum.Add(premiumUnderAdviceTd1Sum);
                                                                });

                                                                // Calculating and appending HTML for Total Premium under advice
                                                                var postedAmountTdSum = commisionDetail
                                                                    .Where(witem => witem.AE_Posted_Date.Date == gpMonthRangeItem.GroupKey.Date)
                                                                    .Sum(item => Convert.ToDouble(item.ALLOCATED_AMOUNT));

                                                                var postedAmountTdSumR = (postedAmountTdSum == 0) ? "R0.00" : Utility.FormatCurrency(postedAmountTdSum.ToString());
                                                                commisionDetailSrc.Append("<td class='text-right'>" + Utility.FormatCurrency(postedAmountTdSumR) + "</td>");

                                                                // Adding inner list to monthly total list and closing row
                                                                monthlyAEPostedTotalList.Add(innermonthlyAEPostedTotalListSum);
                                                                commisionDetailSrc.Append("</tr>");
                                                            });

                                                            // Calculating and appending HTML for Total row
                                                            int numaePostedColumns = monthlyAEPostedTotalList[0].Count;
                                                            numColumns = gpCommisionType.Count;
                                                            List<double> aePostedColumnSums = new List<double>(Enumerable.Repeat(0.00, numColumns));

                                                            foreach (var row in monthlyAEPostedTotalList)
                                                            {
                                                                for (int j = 0; j < numColumns; j++)
                                                                {
                                                                    aePostedColumnSums[j] += row[j];
                                                                }
                                                            }

                                                            commisionDetailSrc.Append("<tr><td class='text-left dark-blue-bg text-white font-weight-bold'>Total</td>");

                                                            for (int k = 0; k < aePostedColumnSums.Count; k++)
                                                            {
                                                                totalAEPostedMonthlyEarnings += aePostedColumnSums[k];
                                                                commisionDetailSrc.Append("<td class='text-right font-weight-bold'>" + Utility.FormatCurrency(aePostedColumnSums[k].ToString()) + "</ td>");
                                                            }

                                                            // Appending HTML for Total Premium under advice and closing the table
                                                            var totalAEPostedMonthlyEarningsR = (totalAEPostedMonthlyEarnings == 0) ? "R0.00" : Utility.FormatCurrency(totalAEPostedMonthlyEarnings.ToString());
                                                            commisionDetailSrc.Append("<td class='text-right font-weight-bold'>" + Utility.FormatCurrency(totalAEPostedMonthlyEarningsR) + "</ td>");
                                                            commisionDetailSrc.Append("</tr></table></div></div>");

                                                            // Closing FSP account postings summary section
                                                        });

                                                        // FSP account postings summary end

                                                        // Future Date Production start

                                                        // Grouping records by Due date
                                                        var dueDateRecords = commisionDetail.GroupBy(gpDueDateitem => new { gpDueDateitem.DUE_DATE.Date })
                                                            .Select(group => new
                                                            {
                                                                GroupKey = group.Key,
                                                            }).ToList();

                                                        // Grouping records by Fiduciary fees
                                                        var prdocutDescriptionRecords = commisionDetail.GroupBy(gpDueDateitem => new { gpDueDateitem.PRODUCT_DESCRIPTION })
                                                            .Select(group => new
                                                            {
                                                                GroupKey = group.Key,
                                                            }).ToList();

                                                        // Initializing variables for monthly totals
                                                        List<List<double>> monthlyDueDateTotalList = new List<List<double>>();

                                                        // Appending HTML for the Future-dated production section
                                                        commisionDetailSrc.Append("<!-- Future-dated production Section --><div class='col-6'><!-- Heading for Future-dated production --><h4 class='monthly-production-summary skyblue-bg-title text-white text-center'>Future-dated production</h4><div class='monthly-table'><!-- Table for Future-dated production --><table width='100%' cellpadding='0' cellspacing='0'><!-- Table Headers --><thead><tr><th class='text-left text-white font-weight-bold'>Due date</th><th style='height:50px;' class='text-left'>Fiduciary fees</th><th class='text-left'>Allocated amount</th></tr></thead>");

                                                        // Initializing variables for column sums
                                                        double FutureColumnSums = 0.00;
                                                        double sumOfAllocatedAmount = 0.00;
                                                        double sumOfDueDateAllocatedAmount = 0.00;

                                                        // Iterating through Due date groups
                                                        dueDateRecords.ForEach(gpDueDateItem =>
                                                        {
                                                            // Iterating through Fiduciary fees groups
                                                            prdocutDescriptionRecords.ForEach(prdocutDescription =>
                                                            {
                                                                // Appending HTML for Date, Fiduciary fees, and Allocated amount columns
                                                                commisionDetailSrc.Append("<tr><td class='text-left text-nowrap'>" + DateTime.Parse(gpDueDateItem.GroupKey.Date.ToString()).ToString("dd-MMM-yyyy") + "</td>");
                                                                sumOfAllocatedAmount = commisionDetail
                                                                    .Where(witem => witem.PRODUCT_DESCRIPTION == prdocutDescription.GroupKey.PRODUCT_DESCRIPTION && witem.DUE_DATE == gpDueDateItem.GroupKey.Date)
                                                                    .Sum(item => Convert.ToDouble(item.ALLOCATED_AMOUNT));

                                                                commisionDetailSrc.Append("<td class='text-left'>" + (prdocutDescription.GroupKey.PRODUCT_DESCRIPTION == "Commission Service Fee" ? "Premium Under Advise Fee" : prdocutDescription.GroupKey.PRODUCT_DESCRIPTION) + "</td>");
                                                                var sumOfAllocatedAmountR = (sumOfAllocatedAmount == 0) ? "R0.00" : Utility.FormatCurrency(sumOfAllocatedAmount.ToString());
                                                                commisionDetailSrc.Append("<td class='text-right'>" + sumOfAllocatedAmountR + "</td>");
                                                                FutureColumnSums += sumOfAllocatedAmount;
                                                                commisionDetailSrc.Append("</tr>");
                                                            });

                                                            // Appending HTML for SubTotal row
                                                            var sumOfDueDateAllocatedAmountR = (sumOfDueDateAllocatedAmount == 0) ? "R0.00" : Utility.FormatCurrency(sumOfDueDateAllocatedAmount.ToString());
                                                            commisionDetailSrc.Append("<tr><td class='text-right' colspan='2'>Sub Total<td class='text-right'>" + sumOfDueDateAllocatedAmountR + "</td></tr>");
                                                        });

                                                        // Appending HTML for Total earnings row and closing the table
                                                        commisionDetailSrc.Append("<td class='dark-blue-bg text-white font-weight-bold text-left' colspan='2'>Total earnings</td><td class='text-right font-weight-bold'>" + Utility.FormatCurrency(FutureColumnSums) + "</td>");
                                                        commisionDetailSrc.Append("</tr></table></div></div>");
                                                        commisionDetailSrc.Append("</div></div>");
                                                        // Future Date Production end

                                                        //Deepak*** End


                                                        // HTML generation for Monthly production summary Section
                                                        commisionDetailSrc.Append("<!-- Monthly production summary Section --><div class='earnings-section-monthly d-flex'><!-- Two Columns Layout --><div class='d-flex gap-1 w-100'><!-- Monthly production summary T1 --><div class='col-6'><!-- Heading for Monthly production summary T1 --><h4 class='monthly-production-summary skyblue-bg-title text-white text-center'>Monthly production summary</h4><div class='monthly-table'><!-- Table for Monthly production summary T1 --><table width='100%' cellpadding='0' cellspacing='0'><!-- Table Headers --><thead><tr><th class='text-white font-weight-boldtext-nowrap text-left'>Month</th>");

                                                        // HTML generation for table headers based on CommissionType
                                                        gpCommisionType.ForEach(gpCommisionTypeitem =>
                                                        {
                                                            commisionDetailSrc.Append("<th style='height:50px' class='text-left'> Premium Under Advice(" + gpCommisionTypeitem.GroupKey.CommissionType + ")</th>");
                                                        });
                                                        commisionDetailSrc.Append("</tr></thead>");

                                                        // Iterate through grouped records to populate table rows
                                                        records.ForEach(gpMonthRangeItem =>
                                                        {
                                                            commisionDetailSrc.Append("<tr><td class='text-nowrap text-left'>" + CommonUtility.GetMonthRange(gpMonthRangeItem.GroupKey.Month) + "</td>");
                                                            List<double> innermonthlyTotalListSum = new List<double>();

                                                            // Iterate through CommissionType groups to populate cells
                                                            gpCommisionType.ForEach(gpCommisionTypeitem =>
                                                            {
                                                                var premiumUnderAdviceTd1Sum = commisionDetail
                                                                    .Where(witem => ((witem.REQUEST_DATETIME.Month == gpMonthRangeItem.GroupKey.Month) &&
                                                                                     (witem.CommissionType == gpCommisionTypeitem.GroupKey.CommissionType) &&
                                                                                     (witem.Type == "Policy_Data")))
                                                                    .Sum(item => Convert.ToDouble(item.ALLOCATED_AMOUNT));

                                                                var premiumUnderAdviceTd1SumR = (premiumUnderAdviceTd1Sum == 0) ? "R0.00" : Utility.FormatCurrency(premiumUnderAdviceTd1Sum.ToString());
                                                                commisionDetailSrc.Append("<td class='text-right'>" + premiumUnderAdviceTd1SumR + "</td>");
                                                                innermonthlyTotalListSum.Add(premiumUnderAdviceTd1Sum);
                                                            });

                                                            // Add monthly total to the list
                                                            monthlyTotalList.Add(innermonthlyTotalListSum);
                                                            commisionDetailSrc.Append("</tr>");
                                                        });

                                                        // Calculate column sums for monthly totals
                                                        numColumns = monthlyTotalList[0].Count;
                                                        List<double> premiumColumnSums = new List<double>(Enumerable.Repeat(0.00, numColumns));

                                                        foreach (var row in monthlyTotalList)
                                                        {
                                                            for (int j = 0; j < numColumns; j++)
                                                            {
                                                                premiumColumnSums[j] += row[j];
                                                            }
                                                        }

                                                        // Add total row to the table
                                                        commisionDetailSrc.Append("<tr><td class='text-left dark-blue-bg text-white font-weight-bold'>Total</td>");

                                                        for (int k = 0; k < premiumColumnSums.Count; k++)
                                                        {
                                                            totalMonthlyEarnings += premiumColumnSums[k];
                                                            commisionDetailSrc.Append("<td class='text-right font-weight-bold'>" + Utility.FormatCurrency(premiumColumnSums[k]) + "</ td>");
                                                        }

                                                        commisionDetailSrc.Append("</tr></table></div></div>");


                                                        // Fees Section: Monthly Fiduciary Fees Summary
                                                        List<List<double>> monthlyFessTotalList = new List<List<double>>();

                                                        // HTML generation for Monthly Fiduciary Fees Summary Section
                                                        commisionDetailSrc.Append("<!-- Monthly production summary T2 --><div class='col-6'><!-- Heading for Monthly production summary T2 --><h4 class='monthly-production-summary skyblue-bg-title text-white text-center'>Monthly production summary</h4><div class='monthly-table'><!-- Table for Monthly production summary T1 --><table width='100%' cellpadding='0' cellspacing='0'><!-- Table Headers --><thead><tr><th class='text-white font-weight-bold text-left'>Month</th>");

                                                        // Generate table headers for each CommissionType in the group
                                                        gpCommisionType.ForEach(gpCommisionTypeitem =>
                                                        {
                                                            commisionDetailSrc.Append("<th style='height:50px' class='text-left'> Fiduciary Fees(" + gpCommisionTypeitem.GroupKey.CommissionType + ")</th>");
                                                        });

                                                        commisionDetailSrc.Append("</tr></thead>");

                                                        // Iterate through grouped records to populate table rows for Fiduciary Fees
                                                        records.ForEach(gpFeesMonthRangeItem =>
                                                        {
                                                            commisionDetailSrc.Append("<tr><td class='text-left'>" + CommonUtility.GetMonthRange(gpFeesMonthRangeItem.GroupKey.Month) + "</td>");
                                                            List<double> innerFeesMonthlyTotalListSum = new List<double>();

                                                            // Iterate through CommissionType groups to populate cells with Fiduciary Fees
                                                            gpCommisionType.ForEach(gpCommisionTypeitem =>
                                                            {
                                                                var premiumUnderAdviceTd1Sum = commisionDetail
                                                                    .Where(witem => ((witem.REQUEST_DATETIME.Month == gpFeesMonthRangeItem.GroupKey.Month) &&
                                                                                     (witem.CommissionType == gpCommisionTypeitem.GroupKey.CommissionType) &&
                                                                                     (witem.Type == "Fiduciary_Data")))
                                                                    .Sum(item => Convert.ToDouble(item.ALLOCATED_AMOUNT));

                                                                var premiumUnderAdviceTd1SumR = (premiumUnderAdviceTd1Sum == 0) ? "R0.00" : Utility.FormatCurrency(premiumUnderAdviceTd1Sum.ToString());
                                                                commisionDetailSrc.Append("<td class='text-right'>" + premiumUnderAdviceTd1SumR + "</td>");
                                                                innerFeesMonthlyTotalListSum.Add(premiumUnderAdviceTd1Sum);
                                                            });

                                                            // Add monthly total to the list for Fiduciary Fees
                                                            monthlyFessTotalList.Add(innerFeesMonthlyTotalListSum);
                                                            commisionDetailSrc.Append("</tr>");
                                                        });

                                                        // Calculate column sums for Fiduciary Fees monthly totals
                                                        int numFeesColumns = monthlyFessTotalList[0].Count;
                                                        List<double> FeesColumnSums = new List<double>(Enumerable.Repeat(0.00, numFeesColumns));

                                                        foreach (var row in monthlyFessTotalList)
                                                        {
                                                            for (int j = 0; j < numFeesColumns; j++)
                                                            {
                                                                FeesColumnSums[j] += row[j];
                                                            }
                                                        }

                                                        // Add total row for Fiduciary Fees to the table
                                                        commisionDetailSrc.Append("<tr><td class='text-left dark-blue-bg text-white font-weight-bold'>Total</td>");

                                                        for (int k = 0; k < FeesColumnSums.Count; k++)
                                                        {
                                                            totalMonthlyEarnings += FeesColumnSums[k];
                                                            commisionDetailSrc.Append("<td class='text-right font-weight-bold'>" + Utility.FormatCurrency(FeesColumnSums[k]) + "</ td>");
                                                        }

                                                        // Add total monthly earnings to the widget
                                                        var totalMonthlyEarningsR = (totalMonthlyEarnings == 0) ? "R0.00" : Utility.FormatCurrency(totalMonthlyEarnings.ToString());
                                                        commisionDetailSrc.Append("</tr></table>");

                                                        commisionDetailSrc.Append("</div></div></div></div>");

                                                        commisionDetailSrc.Append("<!-- Total Earning Section --><div class='earnings-section-monthly'><div class='row'><div class='col-3 text-right'></div><div class='col-3 text-right'><div style='padding-left: 12px;' class='dark-blue-bg text-white text-left font-weight-bold pe-3 py-1'>Total earnings</div></div><div class='col-3 text-left'><div class='total-price-title py-1 font-weight-bold text-center'>" + totalMonthlyEarningsR + "</div></div>");

                                                        // Replacing placeholder in the commisionDetailString with the specified divHeight value

                                                        // Replacing placeholder in the commisionDetailString with the generated commissionDetailSrc content
                                                        commisionDetailString = commisionDetailString.Replace("{{ppsEarningForPeriod}}", commisionDetailSrc.ToString());

                                                        // Replacing placeholder in the commisionDetailString with the specified divHeight value
                                                        commisionDetailString.Replace("{{WidgetDivHeight}}", divHeight);
                                                        // Replacing placeholder in the commisionDetailString with the generated commissionDetailSrc content
                                                        commisionDetailString = commisionDetailString.Replace("{{ppsEarningForPeriod}}", commisionDetailSrc.ToString());
                                                        // Appending the modified commisionDetailString to the htmlString
                                                        htmlString.Append(commisionDetailString);
                                                        // Appending the modified commisionDetailString to the htmlString

                                                    }
                                                }


                                                else if (mergedlst[i].WidgetName == HtmlConstants.PPS_FOOTER1_WIDGET_NAME)
                                                {
                                                    string ppsFooter1InfoJson = "{Reg_ID : 1,Start_Date : '2023-01-01',End_Date : '2023-01-01',Request_DateTime : 'DummyText1',ID : '124529534',Intermediary_Code : 'DummyText1',FSP_ID : 'DummyText1',Policy_Number : 'DummyText1',FSP_Party_ID : 'DummyText1',Client_Number : '124556686',FSP_REF : '2452953',Client_Name : 'Mr SCHOELER',Int_ID : 'DummyText1',Product_Type : 'DummyText1',Commission_Amount : 'DummyText1',INT_EXT_REF : '124411745',Int_Name : 'Kruger Van Heerden',Int_Type : 'DummyText1',Policy_Ref : '5596100',Member_Ref : '124556686',Member_Name : 'DummyText1',Transaction_Amount : 'DummyText1',Mem_Age : 'DummyText1',Months_In_Force : 'DummyText1',Commission_Type : 'Safe Custody Fee',Description : 'Safe Custody Service Fee',POSTED_DATE : '2023-03-03',AE_Type_ID : 'DummyText1',AE_Amount : 'DummyText1',DR_CR : 'DummyText1',NAME : 'DummyText1',Member_Surname : 'DummyText1',Jurisdiction : 'DummyText1',Sales_Office : 'DummyText1',FSP_Name : 'Miss Yvonne van Heerden',FSP_Trading_Name : 'T/A Yvonne Van Heerden Financial Planner CC',FSP_Ext_Ref : '124529534',FSP_Kind : 'DummyText1',  		FSP_VAT_Number : '2452953',Product : 'DummyText1',Prod_Group : 'Service Fee',Prod_Seq : 'DummyText1',Report_Seq : 'DummyText1',TYPE : 'DummyText1',Display_Amount : '17.55',VAT_Amount : '38001.27',Earning_Amount : '256670.66',Payment_Amount : 'DummyText1',Business_Type : 'DummyText1',Lifecycle_Description : 'DummyText1',Lifecycle_Start_Date : 'DummyText1',AE_Scheduler_ID : 'DummyText1',VAT_Amount_1 : 'DummyText1',Final_Amount : 'DummyText1'}";
                                                    if (ppsFooter1InfoJson != string.Empty && validationEngine.IsValidJson(ppsFooter1InfoJson))
                                                    {
                                                        string middleText = "PPS Insurance is a registered Insurer and FSP";
                                                        string pageText = "Page 1/2";
                                                        spIAA_PaymentDetail ppsFooter1Info = JsonConvert.DeserializeObject<spIAA_PaymentDetail>(ppsFooter1InfoJson);
                                                        var ppsFooter1HtmlWidget = HtmlConstants.PPS_FOOTER1_WIDGET_HTML;
                                                        ppsFooter1HtmlWidget = ppsFooter1HtmlWidget.Replace("{{WidgetDivHeight}}", divHeight);

                                                        ppsFooter1HtmlWidget = ppsFooter1HtmlWidget.Replace("{{FSPFooterDetails}}", middleText);
                                                        ppsFooter1HtmlWidget = ppsFooter1HtmlWidget.Replace("{{FSPPage}}", pageText);
                                                        htmlString.Append(ppsFooter1HtmlWidget);
                                                    }
                                                }

                                                else if (mergedlst[i].WidgetName == HtmlConstants.FOOTER_IMAGE_WIDGET_NAME)
                                                {
                                                    string footerImageInfoJson = "{Reg_ID : 1,Start_Date : '2023-01-01',End_Date : '2023-01-01',Request_DateTime : 'DummyText1',ID : '124529534',Intermediary_Code : 'DummyText1',FSP_ID : 'DummyText1',Policy_Number : 'DummyText1',FSP_Party_ID : 'DummyText1',Client_Number : '124556686',FSP_REF : '2452953',Client_Name : 'Mr SCHOELER',Int_ID : 'DummyText1',Product_Type : 'DummyText1',Commission_Amount : 'DummyText1',INT_EXT_REF : '124411745',Int_Name : 'Kruger Van Heerden',Int_Type : 'DummyText1',Policy_Ref : '5596100',Member_Ref : '124556686',Member_Name : 'DummyText1',Transaction_Amount : 'DummyText1',Mem_Age : 'DummyText1',Months_In_Force : 'DummyText1',Commission_Type : 'Safe Custody Fee',Description : 'Safe Custody Service Fee',POSTED_DATE : '2023-03-03',AE_Type_ID : 'DummyText1',AE_Amount : 'DummyText1',DR_CR : 'DummyText1',NAME : 'DummyText1',Member_Surname : 'DummyText1',Jurisdiction : 'DummyText1',Sales_Office : 'DummyText1',FSP_Name : 'Miss Yvonne van Heerden',FSP_Trading_Name : 'T/A Yvonne Van Heerden Financial Planner CC',FSP_Ext_Ref : '124529534',FSP_Kind : 'DummyText1',  		FSP_VAT_Number : '2452953',Product : 'DummyText1',Prod_Group : 'Service Fee',Prod_Seq : 'DummyText1',Report_Seq : 'DummyText1',TYPE : 'DummyText1',Display_Amount : '17.55',VAT_Amount : '38001.27',Earning_Amount : '256670.66',Payment_Amount : 'DummyText1',Business_Type : 'DummyText1',Lifecycle_Description : 'DummyText1',Lifecycle_Start_Date : 'DummyText1',AE_Scheduler_ID : 'DummyText1',VAT_Amount_1 : 'DummyText1',Final_Amount : 'DummyText1'}";
                                                    if (footerImageInfoJson != string.Empty && validationEngine.IsValidJson(footerImageInfoJson))
                                                    {
                                                        //string middleText = "PPS Insurance is a registered Insurer and FSP";
                                                        //string pageText = "Page 1/2";
                                                        spIAA_PaymentDetail footerImageInfo = JsonConvert.DeserializeObject<spIAA_PaymentDetail>(footerImageInfoJson);
                                                        var footerImageHtmlWidget = HtmlConstants.FOOTER_IMAGE_WIDGET_HTML;
                                                        footerImageHtmlWidget = footerImageHtmlWidget.Replace("{{WidgetDivHeight}}", divHeight);

                                                        //footerImageHtmlWidget = footerImageHtmlWidget.Replace("{{FSPFooterDetails}}", middleText);
                                                        //footerImageHtmlWidget = footerImageHtmlWidget.Replace("{{FSPPage}}", pageText);
                                                        htmlString.Append(footerImageHtmlWidget);
                                                    }
                                                }

                                                else if (mergedlst[i].WidgetName == HtmlConstants.DETAILED_TRANSACTIONS_WIDGET_NAME)
                                                {
                                                    string transactionListJson = "[{'INT_EXT_REF':'2164250','Int_Name':'Mr SCHOELER','Client_Name':'Kruger Van Heerden','Member_Ref':124556686,'Policy_Ref':5596100,'Description':'Safe Custody Service Fee','Commission_Type':'Safe Custody Fee','POSTED_DATE':'20-Mar-23','Display_Amount':'17.55','Query_Link':'https://www.google.com/','TYPE':'Fiduciary_Data','Prod_Group':'Safe Custody Fee'},{'INT_EXT_REF':'2164250','Int_Name':'Yvonne Van Heerden','Client_Name':'Mr SCHOELER','Member_Ref':124556686,'Policy_Ref':'5596100','Description':'Safe Custody Service Fee VAT','Commission_Type':'Safe Custody Fee','POSTED_DATE':'20-Mar-23','Display_Amount':'2.63','Query_Link':'https://www.google.com/','TYPE':'Fiduciary_Data','Prod_Group':'Safe Custody Fee'},{'INT_EXT_REF':'124411745','Int_Name':'Kruger Van Heerden','Client_Name':'DR N J Olivier','Member_Ref':'1217181','Policy_Ref':'5524069','Description':'Safe Custody Service Fee','Commission_Type':'Safe Custody Fee','POSTED_DATE':'20-Mar-23','Display_Amount':'17.55','Query_Link':'https://www.google.com/','TYPE':'Fiduciary_Data','Prod_Group':'Safe Custody Fee'},{'INT_EXT_REF':'124411745','Int_Name':'Kruger Van Heerden','Client_Name':'DR N J Olivier','Member_Ref':'124556686','Policy_Ref':'5596100','Description':'Safe Custody Service Fee VAT','Commission_Type':'Safe Custody Fee','POSTED_DATE':'20-Mar-23','Display_Amount':'2.63','Query_Link':'https://www.google.com/','TYPE':'Fiduciary_Data','Prod_Group':'VAT'}]";
                                                    double TotalPostedAmount = 0;
                                                    if (transactionListJson != string.Empty && validationEngine.IsValidJson(transactionListJson))
                                                    {
                                                        IList<spIAA_PaymentDetail> transaction = JsonConvert.DeserializeObject<List<spIAA_PaymentDetail>>(transactionListJson);
                                                        StringBuilder detailedTransactionSrc = new StringBuilder();
                                                        string detailedTransactionString = HtmlConstants.DETAILED_TRANSACTIONS_WIDGET_HTML;
                                                        var records = transaction.GroupBy(gptransactionitem => gptransactionitem.INT_EXT_REF).ToList();
                                                        records?.ForEach(transactionitem =>
                                                        {
                                                            detailedTransactionSrc.Append("<div class='px-50'><div class='prouct-table-block'><div class='text-left fsp-transaction-title font-weight-bold mb-3'>Intermediary:  " + transactionitem.FirstOrDefault().INT_EXT_REF + " " + transactionitem.FirstOrDefault().Int_Name + "</div><table width='100%' cellpadding='0' cellspacing='0'> <tr><th class='font-weight-bold text-white text-left'>Client name</th> <th class='font-weight-bold text-white text-left pe-0 bdr-r-0'>Member<br /> number</th> <th class='font-weight-bold text-white text-left'>Policy number</th> <th class='font-weight-bold text-white text-left'>Fiduciary fees</th> <th class='font-weight-bold text-white text-left'>Commission<br /> type</th> <th class='font-weight-bold text-white text-left'>Posted date</th> <th class='font-weight-bold text-white text-left'>Posted amount</th> <th class='font-weight-bold text-white'>Query</th> </tr> ");
                                                            detailedTransactionString = detailedTransactionString.Replace("{{QueryBtnImgLink}}", "https://www.google.com/");
                                                            detailedTransactionString = detailedTransactionString.Replace("{{QueryBtn}}", "assets/images/IfQueryBtn.jpg");


                                                            transaction.Where(witem => witem.INT_EXT_REF == transactionitem.FirstOrDefault().INT_EXT_REF).ToList().ForEach(item =>
                                                            {
                                                                detailedTransactionSrc.Append("<tr><td align = 'center' valign = 'center' class='px-1 py-1 fsp-bdr-right fsp-bdr-bottom text-left'>" +
                                                                        item.Client_Name + "</td><td class= 'fsp-bdr-right fsp-bdr-bottom px-1 text-left'>" + item.Member_Ref + "</td><td class= 'fsp-bdr-right fsp-bdr-bottom px-1 text-left'> " + item.Policy_Ref + "</td><td class= 'text-left fsp-bdr-right fsp-bdr-bottom px-1'>" + (item.Description == "Commission Service Fee" ? "Premium Under Advise Fee" : item.Description) + "</td><td class= 'text-left fsp-bdr-right fsp-bdr-bottom px-1'>" + item.Commission_Type + "</td><td class= 'text-left text-nowrap fsp-bdr-right fsp-bdr-bottom px-1'>" + item.POSTED_DATE.ToString("dd-MMM-yyyy") + "</td><td class= 'text-right fsp-bdr-right fsp-bdr-bottom px-1'> " + Utility.FormatCurrency(item.Display_Amount) + "</td><td class= 'text-center fsp-bdr-bottom px-1'><a href ='https://www.google.com/' target ='_blank'><img class='leftarrowlogo' src='assets/images/leftarrowlogo.png' alt='Left Arrow'></a></td></tr>");
                                                                TotalPostedAmount += ((item.TYPE == "Fiduciary_Data") && (item.Prod_Group != "VAT")) ? (Convert.ToDouble(item.Display_Amount)) : 0.0;
                                                            });
                                                            string TotalPostedAmountR = (TotalPostedAmount == 0) ? "R0.00" : Utility.FormatCurrency(TotalPostedAmount.ToString());
                                                            detailedTransactionSrc.Append("<tr> <td align='center' valign='center' class='px-1 py-1 fsp-bdr-right fsp-bdr-bottom'></td> <td class='fsp-bdr-right fsp-bdr-bottom px-1 py-1'></td> <td class='fsp-bdr-right fsp-bdr-bottom px-1 py-1'></td> <td class='text-right fsp-bdr-right fsp-bdr-bottom px-1 py-1'></td> <td class='text-center fsp-bdr-right fsp-bdr-bottom px-1 py-1'><br /></td> <td class='text-center fsp-bdr-right fsp-bdr-bottom px-1 py-1'></td> <td class='text-right fsp-bdr-right fsp-bdr-bottom px-1 py-1'>" + TotalPostedAmountR + "</td> <td class='text-center fsp-bdr-bottom px-1'><a href='https://www.google.com/' target = '_blank' ><img src='assets/images/leftarrowlogo.png'></a></td> </tr></table><div class='text-right w-100 pt-3'><a href='https://www.google.com/' target = '_blank'></a></div></div></div></div>");

                                                            TotalPostedAmount = 0;
                                                        });
                                                        detailedTransactionString = detailedTransactionString.Replace("{{detailedTransaction}}", detailedTransactionSrc.ToString());
                                                        htmlString.Append(detailedTransactionString);
                                                    }
                                                }
                                                else if (mergedlst[i].WidgetName == HtmlConstants.PPS_DETAILED_TRANSACTIONS_WIDGET_NAME)
                                                {
                                                    string transactionListJson = "[{'INT_EXT_REF':'124565256 ','POLICY_REF':'3830102','MEMBER_REF':'10024365','Member_Name':'Dr JC Arthur','BUS_GROUP':'PPS INSURANCE','PRODUCT_DESCRIPTION':'Professional Health Provider Whole Life Professional Health',  'REQUESTED_DATETIME':'01-11-2022  00:00:00', 'AE_Posted_Date':'10-12-2023  00:00:00','REQUEST_DATETIME':'2022-09-23','TRANSACTION_AMOUNT':2265.4 ,'ALLOCATED_AMOUNT':-23107.08 ,'MEMBER_AGE':'45 ','MeasureType':'Commission','CommissionType':'2nd Year','FSP_Name':'Miss HW HLONGWANE'},     {'INT_EXT_REF':'124565256 ','POLICY_REF':'3830102','MEMBER_REF':'10024365','Member_Name':'Dr JC Arthur','BUS_GROUP':'PPS INSURANCE','PRODUCT_DESCRIPTION':'Professional Health Provider Whole Life Level Rated Professional Health CatchAll Exercised ',  'REQUESTED_DATETIME':'01-11-2022  00:00:00', 'AE_Posted_Date':'10-12-2023  00:00:00', 'REQUEST_DATETIME':'2022-09-23','TRANSACTION_AMOUNT':84.97 ,'ALLOCATED_AMOUNT':-866.69 ,'MEMBER_AGE':'45 ',  'MeasureType':'Commission','CommissionType':'2nd Year','FSP_Name':'Miss HW HLONGWANE'},     {'INT_EXT_REF':'124565256 ','POLICY_REF':'3830102','MEMBER_REF':'10024365','Member_Name':'Dr JC Arthur','BUS_GROUP':'PPS INSURANCE','PRODUCT_DESCRIPTION':'Professional Health Provider Whole Life Professional Health',  'REQUESTED_DATETIME':'01-11-2022  00:00:00', 'AE_Posted_Date':'10-12-2023  00:00:00', 'REQUEST_DATETIME':'2022-09-23','TRANSACTION_AMOUNT':2265.4,'ALLOCATED_AMOUNT':10968.98,'MEMBER_AGE':'45 ',  'MeasureType':'Commission','CommissionType':'2nd Year','FSP_Name':'Miss HW HLONGWANE'},     {'INT_EXT_REF':'124565256 ','POLICY_REF':'3820110 ','MEMBER_REF':'10436136 ','Member_Name':'Mnr JG Rossouw ','BUS_GROUP':'PPS INSURANCE','PRODUCT_DESCRIPTION':'Professional Health Provider Whole Life Professional Health',  'REQUESTED_DATETIME':'01-11-2022 00:00 ', 'AE_Posted_Date':'10-12-2023  00:00:00', 'REQUEST_DATETIME':'2022-09-28','TRANSACTION_AMOUNT':928.89 ,'ALLOCATED_AMOUNT':-9474.68 ,'MEMBER_AGE':'43',  'MeasureType':'Commission','CommissionType':'1st Year','FSP_Name':'Miss HW HLONGWANE'},     {'INT_EXT_REF':'124565256 ','POLICY_REF':'3820110 ','MEMBER_REF':'10436136 ','Member_Name':'Mnr JG Rossouw ','BUS_GROUP':'PPS INSURANCE','PRODUCT_DESCRIPTION':'Professional Health Provider Whole Life Professional Health',  'REQUESTED_DATETIME':'01-11-2022 00:00 ', 'AE_Posted_Date':'10-12-2023  00:00:00', 'REQUEST_DATETIME':'2022-09-28','TRANSACTION_AMOUNT':928.89 ,'ALLOCATED_AMOUNT':6072.47 ,'MEMBER_AGE':'43',  'MeasureType':'Commission','CommissionType':'2nd Year','FSP_Name':'Miss HW HLONGWANE'}]  ";

                                                    if (transactionListJson != string.Empty && validationEngine.IsValidJson(transactionListJson))
                                                    {
                                                        IList<spIAA_Commission_Detail> ppsDetails = JsonConvert.DeserializeObject<List<spIAA_Commission_Detail>>(transactionListJson);
                                                        StringBuilder detailedTransactionSrc = new StringBuilder();
                                                        string detailedTransactionString = HtmlConstants.PPS_DETAILED_TRANSACTIONS_WIDGET_HTML;
                                                        detailedTransactionSrc.Append("<div class='pps-monthly-table w-100'><table cellpadding = '0' cellspacing='0' width='100%'><tr class='text-left'><th class='bdr-right-white sky-blue-bg text-white font-weight-bold text-left'  width='10%'>Client<br/> name</th><th class='bdr-right-white sky-blue-bg text-white font-weight-bold' width='3%'>Age</th><th class='bdr-right-white sky-blue-bg text-white font-weight-bold' width='5%'>Member number</th><th class='bdr-right-white sky-blue-bg text-white font-weight-bold' width='5%'>Policy number</th><th class='bdr-right-white sky-blue-bg text-white font-weight-bold  text-left' width='26%'>Product</th><th class='bdr-right-white sky-blue-bg text-white font-weight-bold' width='9%'>Date<br/> issued</th><th class='bdr-right-white sky-blue-bg text-white font-weight-bold' width='9%'>Inception<br/> date</th><th class='bdr-right-white sky-blue-bg text-white font-weight-bold' width='7%'>Com<br/> type</th><th class='bdr-right-white sky-blue-bg text-white font-weight-bold' width='9%'>Quantity</th><th class='bdr-right-white sky-blue-bg text-white font-weight-bold' width='9%'>Posted<br/> date</th><th class='bdr-right-white sky-blue-bg text-white font-weight-bold' width='7%'>Earnings</th></tr>");
                                                        var records = ppsDetails.GroupBy(gptransactionitem => gptransactionitem.BUS_GROUP).ToList();
                                                        records?.ForEach(transactionitem =>
                                                        {
                                                            detailedTransactionSrc.Append(" <tr> <td colspan = '11' class='text-left font-weight-bold'> PPS INSURANCE </td> </tr> ");
                                                            var busGroupdata = ppsDetails.Where(witem => witem.BUS_GROUP == transactionitem.FirstOrDefault().BUS_GROUP).ToList();

                                                            var memberGroupRecords = busGroupdata.GroupBy(gpmembertransactionitem => gpmembertransactionitem.MEMBER_REF).ToList();

                                                            memberGroupRecords.ForEach(memberitem =>
                                                            {
                                                                double TotalPostedAmount = 0;
                                                                var memberRecords = busGroupdata.Where(witem => witem.MEMBER_REF == memberitem.FirstOrDefault().MEMBER_REF).ToList();
                                                                memberRecords.ForEach(memberitemrecord =>
                                                                {
                                                                    TotalPostedAmount += (Convert.ToDouble(memberitemrecord.ALLOCATED_AMOUNT));
                                                                    detailedTransactionSrc.Append("<tr><td class='bdr-right-white text-left'>" + memberitemrecord.Member_Name + "</td><td class='bdr-right-white text-left'>" + memberitemrecord.MEMBER_AGE + "</td>" + "<td class='bdr-right-white text-left'>" + memberitemrecord.MEMBER_REF + "</td>" +
                                          "<td class='bdr-right-white text-left'>" + memberitemrecord.POLICY_REF + "</td><td class='bdr-right-white text-left'>" + memberitemrecord.PRODUCT_DESCRIPTION + "</td><td class='bdr-right-white text-left'>" + memberitemrecord.REQUEST_DATETIME.ToString("dd-MMM-yyyy") + "</td><td class='bdr-right-white text-left'>" + memberitemrecord.REQUESTED_DATETIME.ToString("dd-MMM-yyyy") + "</td><td class='bdr-right-white text-left'>" + memberitemrecord.CommissionType + "</td><td class='bdr-right-white text-right'>" + Utility.FormatCurrency(memberitemrecord.TRANSACTION_AMOUNT) + "</td><td class='bdr-right-white text-right'>" + memberitemrecord.AE_Posted_Date.ToString("dd-MMM-yyyy") + "</td>" +
                                                                        "<td class='bdr-right-white ewidth text-right'>" + Utility.FormatCurrency(memberitemrecord.ALLOCATED_AMOUNT) + "</td></tr>");
                                                                });
                                                                string TotalPostedAmountR = (TotalPostedAmount == 0) ? "R0.00" : (TotalPostedAmount.ToString());
                                                                detailedTransactionSrc.Append(" <tr><td class='dark-blue-bg text-white font-weight-bold '></td><td class='dark-blue-bg text-white font-weight-bold '></td><td class='dark-blue-bg text-white font-weight-bold '></td><td class='dark-blue-bg text-white font-weight-bold '></td><td class='dark-blue-bg text-white font-weight-bold '></td><td class='dark-blue-bg text-white font-weight-bold '></td><td class='dark-blue-bg text-white font-weight-bold '></td><td class='dark-blue-bg text-white font-weight-bold '></td><td class='dark-blue-bg text-white font-weight-bold tright fs-16'>Sub Total</td><td colspan='2' class='font-weight-bold text-right fs-16 pps-bg-gray' height='40'>" + Utility.FormatCurrency(TotalPostedAmountR) + "</td></tr>");
                                                            });

                                                        });
                                                        detailedTransactionSrc.Append("</table>");

                                                        //Adding button
                                                        detailedTransactionSrc.Append("<div class='text-center py-3'><a href='#'><img src='assets/images/IfQueryBtn.jpg'></a></div>");

                                                        detailedTransactionSrc.Append("</div>");

                                                        detailedTransactionString = detailedTransactionString.Replace("{{ppsDetailedTransactions}}", detailedTransactionSrc.ToString());
                                                        htmlString.Append(detailedTransactionString);
                                                    }
                                                }
                                                else if (mergedlst[i].WidgetName == HtmlConstants.PPS_DETAILS1_WIDGET_NAME)
                                                {
                                                    DateTime DateFrom = new DateTime(2023, 01, 01);
                                                    DateTime DateTo = new DateTime(2023, 09, 01);
                                                    string ppsDetails1InfoJson = "{'Request_ID':1,'AE_TYPE_ID':'20','INT_EXT_REF':'124529534','POLICY_REF':'October','MEMBER_REF':'Payment Details',    'Member_Name':'DummyText1','BUS_GROUP':'SERVICE FEES','PRODUCT_DESCRIPTION':'Professional Health Provider Whole Life Professional Health','OID':'DummyText1','MeasureType':'Commission','CommissionType':'2nd Year','TRANSACTION_AMOUNT':65566.20,    'ALLOCATED_AMOUNT':65566.20,'MEMBER_AGE':'DummyText1','MONTHS_IN_FORCE':'DummyText1','REQUEST_DATETIME':'2023-01-01','REQUESTED_DATETIME':'2023-09-01','AE_agmt_id':'DummyText1','AE_agmt_type_id':'5596100','AE_Posted_Date':'2023-09-01','AE_Amount':'65566.20','Acc_Name':'DummyText1','FSP_Name':'Miss HW HLONGWANE','DUE_DATE':'2023-09-01','YEAR_START_DATE':'2023-01-01','YEAR_END_DATE':'2023-09-01','Type':'DummyText1',    'Req_Year':'2023-01-01','FutureEndDate':'2023-01-01','Calc1stYear':10000,'Calc2ndYear':20000,'MonthRange':'DummyText1','calcMain2ndYear':30000 }";
                                                    if (ppsDetails1InfoJson != string.Empty && validationEngine.IsValidJson(ppsDetails1InfoJson))
                                                    {
                                                        spIAA_Commission_Detail ppsDetails1Info = JsonConvert.DeserializeObject<spIAA_Commission_Detail>(ppsDetails1InfoJson);
                                                        var ppsDetails1HtmlWidget = HtmlConstants.PPS_DETAILS1_WIDGET_HTML;
                                                        ppsDetails1HtmlWidget = ppsDetails1HtmlWidget.Replace("{{WidgetDivHeight}}", divHeight);

                                                        ppsDetails1HtmlWidget = ppsDetails1HtmlWidget.Replace("{{ref}}", ppsDetails1Info.INT_EXT_REF);
                                                        ppsDetails1HtmlWidget = ppsDetails1HtmlWidget.Replace("{{mtype}}", ppsDetails1Info.MeasureType);
                                                        ppsDetails1HtmlWidget = ppsDetails1HtmlWidget.Replace("{{month}}", DateFrom.ToString("MMMM yyyy"));
                                                        ppsDetails1HtmlWidget = ppsDetails1HtmlWidget.Replace("{{paramDate}}", DateFrom.ToString("yyyy-MM-dd") + " To " + DateTo.ToString("yyyy-MM-dd"));


                                                        htmlString.Append(ppsDetails1HtmlWidget);
                                                    }
                                                }
                                                else if (mergedlst[i].WidgetName == HtmlConstants.PPS_DETAILS2_WIDGET_NAME)
                                                {
                                                    DateTime DateFrom = new DateTime(2023, 01, 01);
                                                    DateTime DateTo = new DateTime(2023, 09, 01);
                                                    string ppsDetails2InfoJson = "{'Request_ID':1,'AE_TYPE_ID':'20','INT_EXT_REF':'124529534','POLICY_REF':'October','MEMBER_REF':'Payment Details',    'Member_Name':'DummyText1','BUS_GROUP':'SERVICE FEES','PRODUCT_DESCRIPTION':'Professional Health Provider Whole Life Professional Health','OID':'DummyText1','MeasureType':'Commission','CommissionType':'2nd Year','TRANSACTION_AMOUNT':65566.20,'ALLOCATED_AMOUNT':65566.20,'MEMBER_AGE':'DummyText1','MONTHS_IN_FORCE':'DummyText1','REQUEST_DATETIME':'2023-01-01','REQUESTED_DATETIME':'2023-09-01','AE_agmt_id':'DummyText1','AE_agmt_type_id':'5596100','AE_Posted_Date':'2023-09-01','AE_Amount':'65566.20','Acc_Name':'DummyText1','FSP_Name':'Miss HW HLONGWANE','DUE_DATE':'2023-09-01','YEAR_START_DATE':'2023-01-01','YEAR_END_DATE':'2023-09-01','Type':'DummyText1',    'Req_Year':'2023-01-01','FutureEndDate':'2023-01-01','Calc1stYear':10000,'Calc2ndYear':20000,'MonthRange':'DummyText1','calcMain2ndYear':30000 }";
                                                    if (ppsDetails2InfoJson != string.Empty && validationEngine.IsValidJson(ppsDetails2InfoJson))
                                                    {
                                                        spIAA_Commission_Detail ppsDetails2Info = JsonConvert.DeserializeObject<spIAA_Commission_Detail>(ppsDetails2InfoJson);
                                                        var ppsDetails2HtmlWidget = HtmlConstants.PPS_DETAILS2_WIDGET_HTML;
                                                        ppsDetails2HtmlWidget = ppsDetails2HtmlWidget.Replace("{{WidgetDivHeight}}", divHeight);

                                                        //ppsDetails2HtmlWidget = ppsDetails2HtmlWidget.Replace("{{ref}}", ppsDetails2Info.INT_EXT_REF);
                                                        //ppsDetails2HtmlWidget = ppsDetails2HtmlWidget.Replace("{{mtype}}", ppsDetails2Info.MeasureType);
                                                        //ppsDetails2HtmlWidget = ppsDetails2HtmlWidget.Replace("{{month}}", DateFrom.ToString("MMMM yyyy"));
                                                        //ppsDetails2HtmlWidget = ppsDetails2HtmlWidget.Replace("{{paramDate}}", DateFrom.ToString("yyyy-MM-dd") + " To " + DateTo.ToString("yyyy-MM-dd"));


                                                        htmlString.Append(ppsDetails2HtmlWidget);
                                                    }
                                                }
                                                else if (mergedlst[i].WidgetName == HtmlConstants.ACCOUNT_INFORMATION_WIDGET_NAME)
                                                {
                                                    string accountInfoJson = "{'StatementDate':'1-APR-2020','StatementPeriod':'Annual Statement', 'CustomerID':'ID2-8989-5656','RmName':'James Wiilims','RmContactNumber':'+4487867833'}";

                                                    string accountInfoData = string.Empty;
                                                    StringBuilder AccDivData = new StringBuilder();
                                                    if (accountInfoJson != string.Empty && validationEngine.IsValidJson(accountInfoJson))
                                                    {
                                                        AccountInformation accountInfo = JsonConvert.DeserializeObject<AccountInformation>(accountInfoJson);
                                                        AccDivData.Append("<div class='list-row-small ht70px'><div class='list-middle-row'> <div class='list-text'>Statement Date</div><label class='list-value mb-0'>" + accountInfo.StatementDate + "</label></div></div>");

                                                        AccDivData.Append("<div class='list-row-small ht70px'><div class='list-middle-row'> <div class='list-text'>Statement Period</div><label class='list-value mb-0'>" + accountInfo.StatementPeriod + "</label></div></div>");

                                                        AccDivData.Append("<div class='list-row-small ht70px'><div class='list-middle-row'> <div class='list-text'>Cusomer ID</div><label class='list-value mb-0'>" + accountInfo.CustomerID + "</label></div></div>");

                                                        AccDivData.Append("<div class='list-row-small ht70px'><div class='list-middle-row'> <div class='list-text'>RM Name</div><label class='list-value mb-0'>" + accountInfo.RmName + "</label></div></div>");

                                                        AccDivData.Append("<div class='list-row-small ht70px'><div class='list-middle-row'> <div class='list-text'>RM Contact Number</div><label class='list-value mb-0'>" + accountInfo.RmContactNumber + "</label></div></div>");

                                                        accountInfoData = HtmlConstants.ACCOUNT_INFORMATION_WIDGET_HTML.Replace("{{AccountInfoData}}", AccDivData.ToString());
                                                    }
                                                    else
                                                    {
                                                        AccDivData.Append("<div class='list-row-small ht70px'><div class='list-middle-row'> <div class='list-text'>" +
                                                            "No Record" + "</div><label class='list-value mb-0'>Found</label></div></div>");
                                                        accountInfoData = HtmlConstants.ACCOUNT_INFORMATION_WIDGET_HTML.Replace("{{AccountInfoData}}",
                                                            AccDivData.ToString());
                                                    }
                                                    accountInfoData = accountInfoData.Replace("{{WidgetDivHeight}}", divHeight);
                                                    htmlString.Append(accountInfoData);
                                                }
                                                else if (mergedlst[i].WidgetName == HtmlConstants.IMAGE_WIDGET_NAME)
                                                {
                                                    var isImageFromAsset = false;
                                                    var assetId = 0;
                                                    var imgAssetFilepath = "assets/images/icon-image.png";
                                                    var imgHeight = "auto";
                                                    var imgAlignment = "text-center";
                                                    if (mergedlst[i].WidgetSetting != string.Empty && validationEngine.IsValidJson(mergedlst[i].WidgetSetting))
                                                    {
                                                        dynamic widgetSetting = JObject.Parse(mergedlst[i].WidgetSetting);
                                                        if (widgetSetting.isPersonalize == false)
                                                        {
                                                            if (tenantConfiguration == null || tenantConfiguration.AssetPath == string.Empty)
                                                            {
                                                                imgAssetFilepath = baseURL + "/assets/" + widgetSetting.AssetLibraryId + "/" + widgetSetting.AssetName;
                                                                isImageFromAsset = true;
                                                                assetId = widgetSetting.AssetId;
                                                            }
                                                            else
                                                            {
                                                                var asset = assetLibraryRepository.GetAssets(new AssetSearchParameter { Identifier = widgetSetting.AssetId, SortParameter = new SortParameter { SortColumn = "Id" } }, tenantCode).ToList()?.FirstOrDefault();
                                                                imgAssetFilepath = asset.FilePath;
                                                                isImageFromAsset = true;
                                                                assetId = Convert.ToInt32(asset.Identifier);
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
                                                    }
                                                    var imgHtmlWidget = new StringBuilder(HtmlConstants.IMAGE_WIDGET_HTML.Replace("{{ImageSource}}", imgAssetFilepath));
                                                    imgHtmlWidget.Replace("{{NewImageClass}}", isImageFromAsset ? " ImageAsset " + assetId : "");
                                                    imgHtmlWidget.Replace("{{WidgetDivHeight}}", divHeight);
                                                    imgHtmlWidget.Replace("{{ImgHeight}}", imgHeight);
                                                    imgHtmlWidget.Replace("{{ImgAlignmentClass}}", imgAlignment);
                                                    htmlString.Append(imgHtmlWidget.ToString());
                                                }
                                                else if (mergedlst[i].WidgetName == HtmlConstants.VIDEO_WIDGET_NAME)
                                                {
                                                    var isVideoFromAsset = false;
                                                    var assetId = 0;
                                                    var vdoAssetFilepath = "assets/images/SampleVideo.mp4";
                                                    if (mergedlst[i].WidgetSetting != string.Empty && validationEngine.IsValidJson(mergedlst[i].WidgetSetting))
                                                    {
                                                        dynamic widgetSetting = JObject.Parse(mergedlst[i].WidgetSetting);
                                                        if (widgetSetting.isEmbedded == true)
                                                        {
                                                            vdoAssetFilepath = widgetSetting.SourceUrl;
                                                        }
                                                        else if (widgetSetting.isPersonalize == false && widgetSetting.isEmbedded == false)
                                                        {
                                                            if (tenantConfiguration == null || tenantConfiguration.AssetPath == string.Empty)
                                                            {
                                                                vdoAssetFilepath = baseURL + "/assets/" + widgetSetting.AssetLibraryId + "/" + widgetSetting.AssetName;
                                                                isVideoFromAsset = true;
                                                                assetId = widgetSetting.AssetId;
                                                            }
                                                            else
                                                            {
                                                                var asset = assetLibraryRepository.GetAssets(new AssetSearchParameter { Identifier = widgetSetting.AssetId, SortParameter = new SortParameter { SortColumn = "Id" } }, tenantCode).ToList()?.FirstOrDefault();
                                                                vdoAssetFilepath = asset.FilePath;
                                                                isVideoFromAsset = true;
                                                                assetId = Convert.ToInt32(asset.Identifier);
                                                            }
                                                        }
                                                    }
                                                    var vdoHtmlWidget = HtmlConstants.VIDEO_WIDGET_HTML.Replace("{{VideoSource}}", vdoAssetFilepath);
                                                    vdoHtmlWidget = vdoHtmlWidget.Replace("{{NewVideoClass}}", isVideoFromAsset ? " VideoAsset " + assetId : "");
                                                    vdoHtmlWidget = vdoHtmlWidget.Replace("{{WidgetDivHeight}}", divHeight);
                                                    htmlString.Append(vdoHtmlWidget);
                                                }
                                                else if (mergedlst[i].WidgetName == HtmlConstants.STATIC_HTML_WIDGET_NAME)
                                                {
                                                    var html = "<div>This is sample <b>HTML</b> for preview</div>";
                                                    if (mergedlst[i].WidgetSetting != string.Empty && validationEngine.IsValidJson(mergedlst[i].WidgetSetting))
                                                    {
                                                        dynamic widgetSetting = JObject.Parse(mergedlst[i].WidgetSetting);
                                                        if (widgetSetting.html.ToString().Length > 0)
                                                        {
                                                            html = widgetSetting.html;
                                                        }
                                                    }
                                                    var staticHtmlWidget = HtmlConstants.STATIC_HTML_WIDGET_HTML.Replace("{{StaticHtml}}", html);
                                                    staticHtmlWidget = staticHtmlWidget.Replace("{{WidgetDivHeight}}", divHeight);
                                                    htmlString.Append(staticHtmlWidget);
                                                }
                                                else if (mergedlst[i].WidgetName == HtmlConstants.PAGE_BREAK_WIDGET_NAME)
                                                {
                                                    var html = "<div style=\"page-break-before:always\">&nbsp;</div>";

                                                    var pageBreakWidget = HtmlConstants.STATIC_HTML_WIDGET_HTML.Replace("{{StaticHtml}}", html);
                                                    pageBreakWidget = pageBreakWidget.Replace("{{WidgetDivHeight}}", divHeight);
                                                    htmlString.Append(pageBreakWidget);
                                                }
                                                else if (mergedlst[i].WidgetName == HtmlConstants.STATIC_SEGMENT_BASED_CONTENT_NAME)
                                                {
                                                    var html = "<div>This is sample SegmentBasedContent</div>";
                                                    if (mergedlst[i].WidgetSetting != string.Empty && validationEngine.IsValidJson(mergedlst[i].WidgetSetting))
                                                    {
                                                        dynamic widgetSetting = JObject.Parse(mergedlst[i].WidgetSetting);
                                                        if (widgetSetting.html.ToString().Length > 0)
                                                        {
                                                            html = widgetSetting.html;
                                                        }
                                                    }
                                                    var segmentBasedContentWidget = HtmlConstants.SEGMENT_BASED_CONTENT_WIDGET_HTML.Replace("{{SegmentBasedContent}}", html);
                                                    segmentBasedContentWidget = segmentBasedContentWidget.Replace("{{WidgetDivHeight}}", divHeight);
                                                    htmlString.Append(segmentBasedContentWidget);
                                                }
                                                else if (mergedlst[i].WidgetName == HtmlConstants.SUMMARY_AT_GLANCE_WIDGET_NAME)
                                                {
                                                    string accountBalanceDataJson = "[{\"AccountType\":\"Saving Account\",\"Currency\":\"$\",\"Amount\":\"8356\"},{\"AccountType\":\"Current Account\",\"Currency\":\"$\",\"Amount\":\"6654\"},{\"AccountType\":\"Recurring Account\",\"Currency\":\"$\",\"Amount\":\"9367\"},{\"AccountType\":\"Wealth\",\"Currency\":\"$\",\"Amount\":\"4589\"}]";

                                                    string accountSummary = string.Empty;
                                                    if (accountBalanceDataJson != string.Empty && validationEngine.IsValidJson(accountBalanceDataJson))
                                                    {
                                                        IList<AccountSummary> lstAccountSummary = JsonConvert.DeserializeObject<List
                                                            <AccountSummary>>(accountBalanceDataJson);
                                                        if (lstAccountSummary.Count > 0)
                                                        {
                                                            StringBuilder accSummary = new StringBuilder();
                                                            lstAccountSummary.ToList().ForEach(acc =>
                                                            {
                                                                accSummary.Append("<tr><td>" + acc.AccountType + "</td><td>" + acc.Currency + "</td><td>" + acc.Amount + "</td></tr>");
                                                            });
                                                            accountSummary = HtmlConstants.SUMMARY_AT_GLANCE_WIDGET_HTML.Replace("{{AccountSummary}}", accSummary.ToString());
                                                        }
                                                    }
                                                    accountSummary = accountSummary.Replace("{{WidgetDivHeight}}", divHeight);
                                                    htmlString.Append(accountSummary);
                                                }
                                                else if (mergedlst[i].WidgetName == HtmlConstants.CURRENT_AVAILABLE_BALANCE_WIDGET_NAME)
                                                {
                                                    string currentAvailBalanceJson = "{'GrandTotal':'32,453,23', 'TotalDeposit':'16,250,00', 'TotalSpend':'16,254,00', 'ProfitEarned':'1,430,00 ', 'Currency':'$', 'Balance': '14,768,80', 'AccountNumber': 'J566565TR678ER', 'AccountType': 'Current', 'Indicator': 'Up'}";
                                                    if (currentAvailBalanceJson != string.Empty && validationEngine.IsValidJson(currentAvailBalanceJson))
                                                    {
                                                        AccountMaster accountMaster = JsonConvert.DeserializeObject<AccountMaster>(currentAvailBalanceJson);
                                                        var CurrentAvailBalance = HtmlConstants.SAVING_CURRENT_AVALABLE_BAL_WIDGET_HTML;
                                                        var accountIndicatorClass = accountMaster.Indicator.ToLower().Equals("up") ? "fa fa-sort-asc text-success" : "fa fa-sort-desc text-danger";
                                                        CurrentAvailBalance = CurrentAvailBalance.Replace("{{AccountIndicatorClass}}", accountIndicatorClass);
                                                        CurrentAvailBalance = CurrentAvailBalance.Replace("{{TotalValue}}", (accountMaster.Currency + accountMaster.GrandTotal));
                                                        CurrentAvailBalance = CurrentAvailBalance.Replace("{{TotalDeposit}}", (accountMaster.Currency + accountMaster.TotalDeposit));
                                                        CurrentAvailBalance = CurrentAvailBalance.Replace("{{TotalSpend}}", (accountMaster.Currency + accountMaster.TotalSpend));
                                                        CurrentAvailBalance = CurrentAvailBalance.Replace("{{Savings}}", (accountMaster.Currency + accountMaster.ProfitEarned));
                                                        CurrentAvailBalance = CurrentAvailBalance.Replace("{{WidgetDivHeight}}", divHeight);
                                                        htmlString.Append(CurrentAvailBalance);
                                                    }
                                                }
                                                else if (mergedlst[i].WidgetName == HtmlConstants.SAVING_AVAILABLE_BALANCE_WIDGET_NAME)
                                                {
                                                    string savingAvailBalanceJson = "{'GrandTotal':'26,453,23', 'TotalDeposit':'13,530,00', 'TotalSpend':'12,124,00', 'ProfitEarned':'2,340,00 ', 'Currency':'$', 'Balance': '19,456,80', 'AccountNumber': 'J566565TR678ER', 'AccountType': 'Saving', 'Indicator': 'Up'}";
                                                    if (savingAvailBalanceJson != string.Empty && validationEngine.IsValidJson(savingAvailBalanceJson))
                                                    {
                                                        AccountMaster accountMaster = JsonConvert.DeserializeObject<AccountMaster>(savingAvailBalanceJson);
                                                        var SavingAvailBalance = HtmlConstants.SAVING_CURRENT_AVALABLE_BAL_WIDGET_HTML;
                                                        var accountIndicatorClass = accountMaster.Indicator.ToLower().Equals("up") ? "fa fa-sort-asc text-success" : "fa fa-sort-desc text-danger";
                                                        SavingAvailBalance = SavingAvailBalance.Replace("{{AccountIndicatorClass}}", accountIndicatorClass);
                                                        SavingAvailBalance = SavingAvailBalance.Replace("{{TotalValue}}", (accountMaster.Currency + accountMaster.GrandTotal));
                                                        SavingAvailBalance = SavingAvailBalance.Replace("{{TotalDeposit}}", (accountMaster.Currency + accountMaster.TotalDeposit));
                                                        SavingAvailBalance = SavingAvailBalance.Replace("{{TotalSpend}}", (accountMaster.Currency + accountMaster.TotalSpend));
                                                        SavingAvailBalance = SavingAvailBalance.Replace("{{Savings}}", (accountMaster.Currency + accountMaster.ProfitEarned));
                                                        SavingAvailBalance = SavingAvailBalance.Replace("{{WidgetDivHeight}}", divHeight);
                                                        htmlString.Append(SavingAvailBalance);
                                                    }
                                                }
                                                else if (mergedlst[i].WidgetName == HtmlConstants.SAVING_TRANSACTION_WIDGET_NAME)
                                                {
                                                    string transactionJson = "[{ 'TransactionDate': '15/07/2020', 'TransactionType': 'CR', 'Narration': 'NXT TXN: IIFL IIFL6574562', 'FCY': '1666.67', 'CurrentRate': '1.062', 'LCY': '1771.42' },{ 'TransactionDate': '19/07/2020', 'TransactionType': 'CR', 'Narration': 'NXT TXN: IIFL IIFL3557346', 'FCY': '1254.71', 'CurrentRate': '1.123', 'LCY': '1876.00' }, { 'TransactionDate': '25/07/2020', 'TransactionType': 'CR', 'Narration': 'NXT TXN: IIFL IIFL8965435', 'FCY': '2345.12', 'CurrentRate': '1.461', 'LCY': '1453.21' }, { 'TransactionDate': '28/07/2020', 'TransactionType': 'CR', 'Narration': 'NXT TXN: IIFL IIFL0034212', 'FCY': '1435.89', 'CurrentRate': '0.962', 'LCY': '1654.56' }]";
                                                    if (transactionJson != string.Empty && validationEngine.IsValidJson(transactionJson))
                                                    {
                                                        IList<AccountTransaction> accountTransactions = JsonConvert.DeserializeObject<List<AccountTransaction>>(transactionJson);
                                                        StringBuilder transaction = new StringBuilder();
                                                        StringBuilder selectOption = new StringBuilder();
                                                        accountTransactions.ToList().ForEach(trans =>
                                                        {
                                                            transaction.Append("<tr><td>" + trans.TransactionDate + "</td><td>" + trans.TransactionType + "</td><td>" + trans.Narration + "</td><td class='text-right'>" + trans.FCY + "</td><td class='text-right'>" + trans.CurrentRate + "</td><td class='text-right'>" + trans.LCY + "</td><td><div class='action-btns btn-tbl-action'>" + "<button type='button' title='View'><span class='fa fa-paper-plane-o'></span></button></div></td></tr>");
                                                        });
                                                        var distinctNaration = accountTransactions.Select(item => item.Narration).Distinct().ToList();
                                                        distinctNaration.ToList().ForEach(item =>
                                                        {
                                                            selectOption.Append("<option value='" + item + "'> " + item + "</option>");
                                                        });
                                                        string accountTransactionstr = HtmlConstants.SAVING_TRANSACTION_WIDGET_HTML.Replace("{{AccountTransactionDetails}}", transaction.ToString());
                                                        accountTransactionstr = accountTransactionstr.Replace("{{SelectOption}}", selectOption.ToString());
                                                        accountTransactionstr = accountTransactionstr.Replace("{{WidgetDivHeight}}", divHeight);
                                                        htmlString.Append(accountTransactionstr);
                                                    }
                                                }
                                                else if (mergedlst[i].WidgetName == HtmlConstants.CURRENT_TRANSACTION_WIDGET_NAME)
                                                {
                                                    string transactionJson = "[{ 'TransactionDate': '15/07/2020', 'TransactionType': 'CR', 'Narration': 'NXT TXN: IIFL IIFL6574562', 'FCY': '1666.67', 'CurrentRate': '1.062', 'LCY': '1771.42' },{ 'TransactionDate': '19/07/2020', 'TransactionType': 'CR', 'Narration': 'NXT TXN: IIFL IIFL3557346', 'FCY': '1254.71', 'CurrentRate': '1.123', 'LCY': '1876.00' }, { 'TransactionDate': '25/07/2020', 'TransactionType': 'CR', 'Narration': 'NXT TXN: IIFL IIFL8965435', 'FCY': '2345.12', 'CurrentRate': '1.461', 'LCY': '1453.21' }, { 'TransactionDate': '28/07/2020', 'TransactionType': 'CR', 'Narration': 'NXT TXN: IIFL IIFL0034212', 'FCY': '1435.89', 'CurrentRate': '0.962', 'LCY': '1654.56' }]";
                                                    if (transactionJson != string.Empty && validationEngine.IsValidJson(transactionJson))
                                                    {
                                                        IList<AccountTransaction> accountTransactions = JsonConvert.DeserializeObject<List<AccountTransaction>>(transactionJson);
                                                        StringBuilder transaction = new StringBuilder();
                                                        StringBuilder selectOption = new StringBuilder();
                                                        accountTransactions.ToList().ForEach(trans =>
                                                        {
                                                            transaction.Append("<tr><td>" + trans.TransactionDate + "</td><td>" + trans.TransactionType + "</td><td>" + trans.Narration + "</td><td class='text-right'>" + trans.FCY + "</td><td class='text-right'>" + trans.CurrentRate + "</td><td class='text-right'>" + trans.LCY + "</td><td><div class='action-btns btn-tbl-action'>" + "<button type='button' title='View'><span class='fa fa-paper-plane-o'></span></button></div></td></tr>");
                                                        });
                                                        var distinctNaration = accountTransactions.Select(item => item.Narration).Distinct().ToList();
                                                        distinctNaration.ToList().ForEach(item =>
                                                        {
                                                            selectOption.Append("<option value='" + item + "'> " + item + "</option>");
                                                        });
                                                        string accountTransactionstr = HtmlConstants.CURRENT_TRANSACTION_WIDGET_HTML.Replace("{{AccountTransactionDetails}}", transaction.ToString());
                                                        accountTransactionstr = accountTransactionstr.Replace("{{SelectOption}}", selectOption.ToString());
                                                        accountTransactionstr = accountTransactionstr.Replace("{{WidgetDivHeight}}", divHeight);
                                                        htmlString.Append(accountTransactionstr);
                                                    }
                                                }
                                                else if (mergedlst[i].WidgetName == HtmlConstants.TOP_4_INCOME_SOURCE_WIDGET_NAME)
                                                {
                                                    string incomeSourceListJson = "[{ 'Source': 'Salary Transfer', 'CurrentSpend': 3453, 'AverageSpend': 123},{ 'Source': 'Cash Deposit', 'CurrentSpend': 3453, 'AverageSpend': 6123},{ 'Source': 'Profit Earned', 'CurrentSpend': 3453, 'AverageSpend': 6123}, { 'Source': 'Rebete', 'CurrentSpend': 3453, 'AverageSpend': 123}]";
                                                    if (incomeSourceListJson != string.Empty && validationEngine.IsValidJson(incomeSourceListJson))
                                                    {
                                                        IList<IncomeSources> incomeSources = JsonConvert.DeserializeObject<List<IncomeSources>>(incomeSourceListJson);
                                                        StringBuilder incomeSrc = new StringBuilder();
                                                        incomeSources.ToList().ForEach(item =>
                                                        {
                                                            var tdstring = string.Empty;
                                                            if (Int32.Parse(item.CurrentSpend) > Int32.Parse(item.AverageSpend))
                                                            {
                                                                tdstring = "<span class='fa fa-sort-desc fa-2x text-danger' aria-hidden='true'></span><span class='ml-2'>" + item.AverageSpend + "</span>";
                                                            }
                                                            else
                                                            {
                                                                tdstring = "<span class='fa fa-sort-asc fa-2x mt-1' aria-hidden='true' style='position:relative;top:6px;color:limegreen'></span><span class='ml-2'>" + item.AverageSpend + "</span>";
                                                            }
                                                            incomeSrc.Append("<tr><td class='float-left'>" + item.Source + "</td>" + "<td> " + item.CurrentSpend + "" + "</td><td>" + tdstring + "</td></tr>");
                                                        });
                                                        string srcstring = HtmlConstants.TOP_4_INCOME_SOURCE_WIDGET_HTML.Replace("{{IncomeSourceList}}", incomeSrc.ToString());
                                                        srcstring = srcstring.Replace("{{WidgetDivHeight}}", divHeight);
                                                        htmlString.Append(srcstring);
                                                    }
                                                }
                                                else if (mergedlst[i].WidgetName == HtmlConstants.ANALYTICS_WIDGET_NAME)
                                                {
                                                    htmlString.Append(HtmlConstants.ANALYTIC_WIDGET_HTML.Replace("{{WidgetDivHeight}}", divHeight));
                                                }
                                                else if (mergedlst[i].WidgetName == HtmlConstants.SPENDING_TREND_WIDGET_NAME)
                                                {
                                                    htmlString.Append(HtmlConstants.SPENDING_TRENDS_WIDGET_HTML.Replace("{{WidgetDivHeight}}", divHeight));
                                                }
                                                else if (mergedlst[i].WidgetName == HtmlConstants.SAVING_TREND_WIDGET_NAME)
                                                {
                                                    htmlString.Append(HtmlConstants.SAVING_TRENDS_WIDGET_HTML.Replace("{{WidgetDivHeight}}", divHeight));
                                                }
                                                else if (mergedlst[i].WidgetName == HtmlConstants.REMINDER_AND_RECOMMENDATION_WIDGET_NAME)
                                                {
                                                    string reminderJson = "[{ 'Title': 'Update Missing Inofrmation', 'Action': 'Update' },{ 'Title': 'Your Rewards Video is available', 'Action': 'View' },{ 'Title': 'Payment Due for Home Loan', 'Action': 'Pay' }, { title: 'Need financial planning for savings.', action: 'Call Me' },{ title: 'Subscribe/Unsubscribe Alerts.', action: 'Apply'},{ title: 'Your credit card payment is due now.', action: 'Pay' }]";
                                                    if (reminderJson != string.Empty && validationEngine.IsValidJson(reminderJson))
                                                    {
                                                        IList<ReminderAndRecommendation> reminderAndRecommendations =
                                                            JsonConvert.DeserializeObject<List<ReminderAndRecommendation>>(reminderJson);
                                                        StringBuilder reminderstr = new StringBuilder();
                                                        reminderstr.Append("<div class='row'><div class='col-lg-9'></div><div class='col-lg-3 text-left'><i class='fa fa-caret-left fa-3x float-left text-danger' aria-hidden='true'></i><span class='mt-2 d-inline-block ml-2'>Click</span></div> </div>");
                                                        reminderAndRecommendations.ToList().ForEach(item =>
                                                        {
                                                            reminderstr.Append("<div class='row'><div class='col-lg-9 text-left'><p class='p-1' style='background-color: #dce3dc;'>" + item.Title + " </p></div><div class='col-lg-3 text-left'><a><i class='fa fa-caret-left fa-3x float-left text-danger'></i><span class='mt-2 d-inline-block ml-2'>" + item.Action + "</span></a></div></div>");
                                                        });
                                                        string widgetstr = HtmlConstants.REMINDER_WIDGET_HTML.Replace("{{ReminderAndRecommdationDataList}}", reminderstr.ToString());
                                                        widgetstr = widgetstr.Replace("{{WidgetDivHeight}}", divHeight);
                                                        htmlString.Append(widgetstr);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (dynaWidgets.Count > 0)
                                                {
                                                    var dynawidget = dynaWidgets.Where(item => item.Identifier == mergedlst[i].WidgetId).ToList().FirstOrDefault();
                                                    TenantEntity entity = new TenantEntity();
                                                    entity.Identifier = dynawidget.EntityId;
                                                    entity.Name = dynawidget.EntityName;
                                                    CustomeTheme themeDetails = new CustomeTheme();
                                                    if (dynawidget.ThemeType == "Default")
                                                    {
                                                        themeDetails = JsonConvert.DeserializeObject<CustomeTheme>(tenantConfiguration.WidgetThemeSetting);
                                                    }
                                                    else
                                                    {
                                                        themeDetails = JsonConvert.DeserializeObject<CustomeTheme>(dynawidget.ThemeCSS);
                                                    }

                                                    if (dynawidget.WidgetType == HtmlConstants.TABLE_DYNAMICWIDGET)
                                                    {
                                                        var tableWidgetHtml = HtmlConstants.TABLE_WIDEGT_FOR_PAGE_PREVIEW;
                                                        tableWidgetHtml = tableWidgetHtml.Replace("{{WidgetDivHeight}}", divHeight);
                                                        tableWidgetHtml = tableWidgetHtml.Replace("{{TableMaxHeight}}", (mergedlst[i].Height * 110) - 40 + "px");
                                                        tableWidgetHtml = this.ApplyStyleCssForDynamicTableAndFormWidget(tableWidgetHtml, themeDetails);
                                                        tableWidgetHtml = tableWidgetHtml.Replace("{{WidgetTitle}}", dynawidget.Title);
                                                        List<DynamicWidgetTableEntity> tableEntities = JsonConvert.DeserializeObject<List<DynamicWidgetTableEntity>>(dynawidget.WidgetSettings);
                                                        StringBuilder tableHeader = new StringBuilder();
                                                        tableHeader.Append("<tr>" + string.Join("", tableEntities.Select(field => string.Format("<th>{0}</th> ", field.HeaderName))) + "</tr>");
                                                        tableWidgetHtml = tableWidgetHtml.Replace("{{tableHeader}}", tableHeader.ToString());
                                                        tableWidgetHtml = tableWidgetHtml.Replace("{{tableBody}}", dynawidget.PreviewData);
                                                        htmlString.Append(tableWidgetHtml);
                                                    }
                                                    else if (dynawidget.WidgetType == HtmlConstants.FORM_DYNAMICWIDGET)
                                                    {
                                                        var formWidgetHtml = HtmlConstants.FORM_WIDGET_FOR_PAGE_PREVIEW;
                                                        formWidgetHtml = formWidgetHtml.Replace("{{WidgetDivHeight}}", divHeight);
                                                        formWidgetHtml = this.ApplyStyleCssForDynamicTableAndFormWidget(formWidgetHtml, themeDetails);
                                                        formWidgetHtml = formWidgetHtml.Replace("{{WidgetTitle}}", dynawidget.Title);
                                                        formWidgetHtml = formWidgetHtml.Replace("{{FormData}}", dynawidget.PreviewData);
                                                        htmlString.Append(formWidgetHtml);
                                                    }
                                                    else if (dynawidget.WidgetType == HtmlConstants.LINEGRAPH_DYNAMICWIDGET)
                                                    {
                                                        var lineGraphWidgetHtml = HtmlConstants.LINE_GRAPH_FOR_PAGE_PREVIEW;
                                                        lineGraphWidgetHtml = lineGraphWidgetHtml.Replace("lineGraphcontainer", "lineGraphcontainer_" + dynawidget.Identifier);
                                                        lineGraphWidgetHtml = lineGraphWidgetHtml.Replace("{{WidgetDivHeight}}", divHeight);
                                                        lineGraphWidgetHtml = this.ApplyStyleCssForDynamicGraphAndChartWidgets(lineGraphWidgetHtml, themeDetails);
                                                        lineGraphWidgetHtml = lineGraphWidgetHtml.Replace("{{WidgetTitle}}", dynawidget.Title);
                                                        lineGraphWidgetHtml = lineGraphWidgetHtml + "<input type='hidden' id='hiddenLineGraphData_" + dynawidget.Identifier + "' value='" + dynawidget.PreviewData + "'>";
                                                        linegraphIds.Add("lineGraphcontainer_" + dynawidget.Identifier);
                                                        htmlString.Append(lineGraphWidgetHtml);
                                                    }
                                                    else if (dynawidget.WidgetType == HtmlConstants.BARGRAPH_DYNAMICWIDGET)
                                                    {
                                                        var barGraphWidgetHtml = HtmlConstants.BAR_GRAPH_FOR_PAGE_PREVIEW;
                                                        barGraphWidgetHtml = barGraphWidgetHtml.Replace("{{WidgetDivHeight}}", divHeight);
                                                        barGraphWidgetHtml = barGraphWidgetHtml.Replace("barGraphcontainer", "barGraphcontainer_" + dynawidget.Identifier);
                                                        barGraphWidgetHtml = this.ApplyStyleCssForDynamicGraphAndChartWidgets(barGraphWidgetHtml, themeDetails);
                                                        barGraphWidgetHtml = barGraphWidgetHtml.Replace("{{WidgetTitle}}", dynawidget.Title);
                                                        barGraphWidgetHtml = barGraphWidgetHtml + "<input type='hidden' id='hiddenBarGraphData_" + dynawidget.Identifier + "' value='" + dynawidget.PreviewData + "'>";
                                                        bargraphIds.Add("barGraphcontainer_" + dynawidget.Identifier);
                                                        htmlString.Append(barGraphWidgetHtml);
                                                    }
                                                    else if (dynawidget.WidgetType == HtmlConstants.PICHART_DYNAMICWIDGET)
                                                    {
                                                        var pieChartWidgetHtml = HtmlConstants.PIE_CHART_FOR_PAGE_PREVIEW;
                                                        pieChartWidgetHtml = pieChartWidgetHtml.Replace("{{WidgetDivHeight}}", divHeight);
                                                        pieChartWidgetHtml = pieChartWidgetHtml.Replace("pieChartcontainer", "pieChartcontainer_" + dynawidget.Identifier);
                                                        pieChartWidgetHtml = this.ApplyStyleCssForDynamicGraphAndChartWidgets(pieChartWidgetHtml, themeDetails);
                                                        pieChartWidgetHtml = pieChartWidgetHtml.Replace("{{WidgetTitle}}", dynawidget.Title);
                                                        pieChartWidgetHtml = pieChartWidgetHtml + "<input type='hidden' id='hiddenPieChartData_" + dynawidget.Identifier + "' value='" + dynawidget.PreviewData + "'>";
                                                        piechartIds.Add("pieChartcontainer_" + dynawidget.Identifier);
                                                        htmlString.Append(pieChartWidgetHtml);
                                                    }
                                                    else if (dynawidget.WidgetType == HtmlConstants.HTML_DYNAMICWIDGET)
                                                    {
                                                        var htmlWidget = HtmlConstants.HTML_WIDGET_FOR_PAGE_PREVIEW;
                                                        htmlWidget = htmlWidget.Replace("{{WidgetDivHeight}}", divHeight);
                                                        htmlWidget = this.ApplyStyleCssForDynamicTableAndFormWidget(htmlWidget, themeDetails);
                                                        htmlWidget = htmlWidget.Replace("{{WidgetTitle}}", dynawidget.Title);
                                                        string settings = dynawidget.WidgetSettings;
                                                        IList<EntityFieldMap> entityFieldMaps = new List<EntityFieldMap>();
                                                        entityFieldMaps = this.dynamicWidgetManager.GetEntityFields(dynawidget.EntityId, tenantCode);
                                                        string data = this.dynamicWidgetManager.GetHTMLPreviewData(entity, entityFieldMaps, dynawidget.PreviewData);
                                                        htmlWidget = htmlWidget.Replace("{{FormData}}", data);
                                                        htmlString.Append(htmlWidget);
                                                    }
                                                }
                                            }

                                            // To end current col-lg class div
                                            htmlString.Append("</div>");

                                            // if current col-lg class width is equal to 12 or end before complete col-lg-12 class, 
                                            //then end parent row class div
                                            if (tempRowWidth == 12 || (i == mergedlst.Count - 1))
                                            {
                                                tempRowWidth = 0;
                                                htmlString.Append("</div>"); //To end row class div
                                                isRowComplete = true;
                                            }
                                        }
                                        mergedlst.ForEach(it =>
                                        {
                                            completelst.Remove(it);
                                        });
                                    }
                                    else
                                    {
                                        if (completelst.Count != 0)
                                        {
                                            currentYPosition = completelst.Min(it => it.Yposition);
                                        }
                                    }
                                }
                                //If row class div end before complete col-lg-12 class
                                if (isRowComplete == false)
                                {
                                    htmlString.Append("</div>");
                                }
                            }
                            else
                            {
                                htmlString.Append(HtmlConstants.NO_WIDGET_MESSAGE_HTML);
                            }
                            if (page.PageTypeName == HtmlConstants.SAVING_ACCOUNT_PAGE || page.PageTypeName == HtmlConstants.CURRENT_ACCOUNT_PAGE)
                            {
                                htmlString.Append(HtmlConstants.END_DIV_TAG);
                            }

                            htmlString.Append(HtmlConstants.PAGE_TAB_CONTENT_FOOTER);
                            htmlString.Append(HtmlConstants.WIDGET_HTML_FOOTER);
                        }
                    }
                    else
                    {
                        htmlString.Append(HtmlConstants.NO_WIDGET_MESSAGE_HTML);
                    }

                    //if (isBackgroundImage)
                    //{
                    //    htmlString.Replace("card border-0", "card border-0 bg-transparent");
                    //    htmlString.Replace("bg-light", "bg-transparent");
                    //}
                    tempHtml.Append(htmlString.ToString());
                }

                if (linegraphIds.Count > 0)
                {
                    var ids = string.Join(",", linegraphIds.Select(item => item).ToList());
                    tempHtml.Append("<input type = 'hidden' id = 'hiddenLineChartIds' value = '" + ids + "'>");
                }
                if (bargraphIds.Count > 0)
                {
                    var ids = string.Join(",", bargraphIds.Select(item => item).ToList());
                    tempHtml.Append("<input type = 'hidden' id = 'hiddenBarChartIds' value = '" + ids + "'>");
                }
                if (piechartIds.Count > 0)
                {
                    var ids = string.Join(",", piechartIds.Select(item => item).ToList());
                    tempHtml.Append("<input type = 'hidden' id = 'hiddenPieChartIds' value = '" + ids + "'>");
                }

                navbarHtml = navbarHtml.Replace("{{NavItemList}}", navItemList.ToString());
                tempHtml.Append(HtmlConstants.CONTAINER_DIV_HTML_FOOTER);
                finalHtml = tempHtml.ToString();
                //finalHtml = navbarHtml + tempHtml.ToString();
                return finalHtml;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
          
        #region Financial tenant static widget formatting

        private string CustomerInformationWidgetFormatting(PageWidget pageWidget, int counter, Statement statement, Page page, string divHeight)
        {
            var widgetId = "PageWidgetId_" + pageWidget.Identifier + "_Counter" + counter.ToString();
            var widgetHTML = HtmlConstants.CUSTOMER_INFORMATION_WIDGET_HTML_FOR_STMT.Replace("{{VideoSource}}", "{{VideoSource_" + statement.Identifier + "_" + page.Identifier + "_" + pageWidget.Identifier + "}}");
            widgetHTML = widgetHTML.Replace("{{WidgetDivHeight}}", divHeight);
            widgetHTML = widgetHTML.Replace("{{WidgetId}}", widgetId);
            return widgetHTML;
        }

        private string PaymentSummaryWidgetFormatting(PageWidget pageWidget, int counter, Statement statement, Page page, string divHeight)
        {
            var widgetId = "PageWidgetId_" + pageWidget.Identifier + "_Counter" + counter.ToString();
            var widgetHTML = HtmlConstants.PAYMENT_SUMMARY_WIDGET_HTML_FOR_STMT;
            widgetHTML = widgetHTML.Replace("{{WidgetDivHeight}}", divHeight);
            widgetHTML = widgetHTML.Replace("{{WidgetId}}", widgetId);
            return widgetHTML;
        }


        private string PpsHeadingWidgetFormatting(PageWidget pageWidget, int counter, Statement statement, Page page, string divHeight)
        {
            var widgetId = "PageWidgetId_" + pageWidget.Identifier + "_Counter" + counter.ToString();
            var widgetHTML = HtmlConstants.PPS_HEADING_WIDGET_HTML_FOR_STMT;
            widgetHTML = widgetHTML.Replace("{{WidgetDivHeight}}", divHeight);
            widgetHTML = widgetHTML.Replace("{{WidgetId}}", widgetId);
            return widgetHTML;
        }

        private string PpsDetailsWidgetFormatting(PageWidget pageWidget, int counter, Statement statement, Page page, string divHeight)
        {
            var widgetId = "PageWidgetId_" + pageWidget.Identifier + "_Counter" + counter.ToString();
            var widgetHTML = HtmlConstants.PPS_DETAILS_WIDGET_HTML_FOR_STMT;
            widgetHTML = widgetHTML.Replace("{{WidgetDivHeight}}", divHeight);
            widgetHTML = widgetHTML.Replace("{{WidgetId}}", widgetId);
            return widgetHTML;
        }

        private string ProductSummaryWidgetFormatting(PageWidget pageWidget, int counter, Statement statement, Page page, string divHeight)
        {
            var widgetId = "PageWidgetId_" + pageWidget.Identifier + "_Counter" + counter.ToString();
            //var widgetHTML = HtmlConstants.PRODUCT_SUMMARY_WIDGET_HTML_FOR_STMT.Replace("{{ProductSummary}}", "{{ProductSummary_" + page.Identifier + "_" + pageWidget.Identifier + "}}").Replace("{{TotalDue}}", "{{TotalDue_" + page.Identifier + "_" + pageWidget.Identifier + "}}").Replace("{{VATDue}}", "{{VATDue_" + page.Identifier + "_" + pageWidget.Identifier + "}}").Replace("{{GrandTotalDue}}", "{{GrandTotalDue_" + page.Identifier + "_" + pageWidget.Identifier + "}}").Replace("{{PPSPayment}}", "{{PPSPayment_" + page.Identifier + "_" + pageWidget.Identifier + "}}").Replace("{{Balance}}", "{{Balance_" + page.Identifier + "_" + pageWidget.Identifier + "}}");

            var widgetHTML = HtmlConstants.PRODUCT_SUMMARY_WIDGET_HTML_FOR_STMT;
            widgetHTML = widgetHTML.Replace("{{WidgetDivHeight}}", divHeight);
            widgetHTML = widgetHTML.Replace("{{WidgetId}}", widgetId);
            return widgetHTML;
        }

        private string EarningForPeriodWidgetFormatting(PageWidget pageWidget, int counter, Statement statement, Page page, string divHeight)
        {
            var widgetId = "PageWidgetId_" + pageWidget.Identifier + "_Counter" + counter.ToString();
            //var widgetHTML = HtmlConstants.PRODUCT_SUMMARY_WIDGET_HTML_FOR_STMT.Replace("{{ProductSummary}}", "{{ProductSummary_" + page.Identifier + "_" + pageWidget.Identifier + "}}").Replace("{{TotalDue}}", "{{TotalDue_" + page.Identifier + "_" + pageWidget.Identifier + "}}").Replace("{{VATDue}}", "{{VATDue_" + page.Identifier + "_" + pageWidget.Identifier + "}}").Replace("{{GrandTotalDue}}", "{{GrandTotalDue_" + page.Identifier + "_" + pageWidget.Identifier + "}}").Replace("{{PPSPayment}}", "{{PPSPayment_" + page.Identifier + "_" + pageWidget.Identifier + "}}").Replace("{{Balance}}", "{{Balance_" + page.Identifier + "_" + pageWidget.Identifier + "}}");

            var widgetHTML = HtmlConstants.EARNINGS_FOR_PERIOD_WIDGET_HTML_FOR_STMT;
            widgetHTML = widgetHTML.Replace("{{WidgetDivHeight}}", divHeight);
            widgetHTML = widgetHTML.Replace("{{WidgetId}}", widgetId);
            return widgetHTML;
        }

        private string DetailedTransactionWidgetFormatting(PageWidget pageWidget, int counter, Statement statement, Page page, string divHeight)
        {
            var widgetId = "PageWidgetId_" + pageWidget.Identifier + "_Counter" + counter.ToString();
            //var widgetHTML = HtmlConstants.PRODUCT_SUMMARY_WIDGET_HTML_FOR_STMT.Replace("{{ProductSummary}}", "{{ProductSummary_" + page.Identifier + "_" + pageWidget.Identifier + "}}").Replace("{{TotalDue}}", "{{TotalDue_" + page.Identifier + "_" + pageWidget.Identifier + "}}").Replace("{{VATDue}}", "{{VATDue_" + page.Identifier + "_" + pageWidget.Identifier + "}}").Replace("{{GrandTotalDue}}", "{{GrandTotalDue_" + page.Identifier + "_" + pageWidget.Identifier + "}}").Replace("{{PPSPayment}}", "{{PPSPayment_" + page.Identifier + "_" + pageWidget.Identifier + "}}").Replace("{{Balance}}", "{{Balance_" + page.Identifier + "_" + pageWidget.Identifier + "}}");

            var widgetHTML = HtmlConstants.DETAILED_TRANSACTIONS_WIDGET_HTML_FOR_STMT;
            widgetHTML = widgetHTML.Replace("{{WidgetDivHeight}}", divHeight);
            widgetHTML = widgetHTML.Replace("{{WidgetId}}", widgetId);
            return widgetHTML;
        }
        private string PpsDetailedTransactionWidgetFormatting(PageWidget pageWidget, int counter, Statement statement, Page page, string divHeight)
        {
            var widgetId = "PageWidgetId_" + pageWidget.Identifier + "_Counter" + counter.ToString();
            var widgetHTML = HtmlConstants.PPS_DETAILED_TRANSACTIONS_WIDGET_HTML_FOR_STMT;
            widgetHTML = widgetHTML.Replace("{{WidgetDivHeight}}", divHeight);
            widgetHTML = widgetHTML.Replace("{{WidgetId}}", widgetId);
            return widgetHTML;
        }

        private string AccountInformationWidgetFormatting(PageWidget pageWidget, int counter, Statement statement, Page page, string divHeight)
        {
            var widgetId = "PageWidgetId_" + pageWidget.Identifier + "_Counter" + counter.ToString();
            var widgetHTML = HtmlConstants.ACCOUNT_INFORMATION_WIDGET_HTML_FOR_STMT.Replace("{{AccountInfoData}}", "{{AccountInfoData_" + page.Identifier + "_" + pageWidget.Identifier + "}}");
            widgetHTML = widgetHTML.Replace("{{WidgetDivHeight}}", divHeight);
            widgetHTML = widgetHTML.Replace("{{WidgetId}}", widgetId);
            return widgetHTML;
        }

        private string PpsFooter1WidgetFormatting(PageWidget pageWidget, int counter, Statement statement, Page page, string divHeight)
        {
            var widgetId = "PageWidgetId_" + pageWidget.Identifier + "_Counter" + counter.ToString();
            var widgetHTML = HtmlConstants.PPS_FOOTER1_WIDGET_HTML_FOR_STMT;
            widgetHTML = widgetHTML.Replace("{{WidgetDivHeight}}", divHeight);
            widgetHTML = widgetHTML.Replace("{{WidgetId}}", widgetId);
            return widgetHTML;
        }

        private string FooterImageWidgetFormatting(PageWidget pageWidget, int counter, Statement statement, Page page, string divHeight)
        {
            var widgetId = "PageWidgetId_" + pageWidget.Identifier + "_Counter" + counter.ToString();
            var widgetHTML = HtmlConstants.FOOTER_IMAGE_WIDGET_HTML_FOR_STMT;
            widgetHTML = widgetHTML.Replace("{{WidgetDivHeight}}", divHeight);
            widgetHTML = widgetHTML.Replace("{{WidgetId}}", widgetId);
            return widgetHTML;
        }
        private string PpsDetails1WidgetFormatting(PageWidget pageWidget, int counter, Statement statement, Page page, string divHeight)
        {
            var widgetId = "PageWidgetId_" + pageWidget.Identifier + "_Counter" + counter.ToString();
            var widgetHTML = HtmlConstants.PPS_DETAILS1_WIDGET_HTML_FOR_STMT;
            widgetHTML = widgetHTML.Replace("{{WidgetDivHeight}}", divHeight);
            widgetHTML = widgetHTML.Replace("{{WidgetId}}", widgetId);
            return widgetHTML;
        }
        private string PpsDetails2WidgetFormatting(PageWidget pageWidget, int counter, Statement statement, Page page, string divHeight)
        {
            var widgetId = "PageWidgetId_" + pageWidget.Identifier + "_Counter" + counter.ToString();
            var widgetHTML = HtmlConstants.PPS_DETAILS2_WIDGET_HTML_FOR_STMT;
            widgetHTML = widgetHTML.Replace("{{WidgetDivHeight}}", divHeight);
            widgetHTML = widgetHTML.Replace("{{WidgetId}}", widgetId);
            return widgetHTML;
        }

        private string SummaryAtGlanceWidgetFormatting(PageWidget pageWidget, int counter, Page page, string divHeight)
        {
            var widgetId = "PageWidgetId_" + pageWidget.Identifier + "_Counter" + counter.ToString();
            var widgetHTML = HtmlConstants.SUMMARY_AT_GLANCE_WIDGET_HTML_FOR_STMT.Replace("{{AccountSummary}}", "{{AccountSummary_" + page.Identifier + "_" + pageWidget.Identifier + "}}");
            widgetHTML = widgetHTML.Replace("{{WidgetDivHeight}}", divHeight);
            widgetHTML = widgetHTML.Replace("{{WidgetId}}", widgetId);
            return widgetHTML;
        }

        private string CurrentAvailBalanceWidgetFormatting(PageWidget pageWidget, int counter, Page page, string divHeight)
        {
            var widgetId = "PageWidgetId_" + pageWidget.Identifier + "_Counter" + counter.ToString();
            var widgetHTML = HtmlConstants.SAVING_CURRENT_AVALABLE_BAL_WIDGET_HTML_FOR_STMT.Replace("{{TotalValue}}", "{{TotalValue_" + page.Identifier + "_" + pageWidget.Identifier + "}}").Replace("{{TotalDeposit}}", "{{TotalDeposit_" + page.Identifier + "_" + pageWidget.Identifier + "}}").Replace("{{TotalSpend}}", "{{TotalSpend_" + page.Identifier + "_" + pageWidget.Identifier + "}}").Replace("{{Savings}}", "{{Savings_" + page.Identifier + "_" + pageWidget.Identifier + "}}");
            widgetHTML = widgetHTML.Replace("{{WidgetId}}", widgetId);
            widgetHTML = widgetHTML.Replace("{{WidgetDivHeight}}", divHeight);
            return widgetHTML;
        }

        private string SavingAvailBalanceWidgetFormatting(PageWidget pageWidget, int counter, Page page, string divHeight)
        {
            var widgetId = "PageWidgetId_" + pageWidget.Identifier + "_Counter" + counter.ToString();
            var widgetHTML = HtmlConstants.SAVING_CURRENT_AVALABLE_BAL_WIDGET_HTML_FOR_STMT.Replace("{{TotalValue}}", "{{TotalValue_" + page.Identifier + "_" + pageWidget.Identifier + "}}").Replace("{{TotalDeposit}}", "{{TotalDeposit_" + page.Identifier + "_" + pageWidget.Identifier + "}}").Replace("{{TotalSpend}}", "{{TotalSpend_" + page.Identifier + "_" + pageWidget.Identifier + "}}").Replace("{{Savings}}", "{{Savings_" + page.Identifier + "_" + pageWidget.Identifier + "}}");
            widgetHTML = widgetHTML.Replace("{{WidgetId}}", widgetId);
            widgetHTML = widgetHTML.Replace("{{WidgetDivHeight}}", divHeight);
            return widgetHTML;
        }

        private string SavingTransactionWidgetFormatting(PageWidget pageWidget, int counter, Page page, string divHeight)
        {
            var widgetId = "PageWidgetId_" + pageWidget.Identifier + "_Counter" + counter.ToString();
            var widgetHTML = HtmlConstants.SAVING_TRANSACTION_WIDGET_HTML_FOR_STMT.Replace("{{AccountTransactionDetails}}", "{{AccountTransactionDetails_" + page.Identifier + "_" + pageWidget.Identifier + "}}").Replace("{{SelectOption}}", "{{SelectOption_" + page.Identifier + "_" + pageWidget.Identifier + "}}");
            widgetHTML = widgetHTML.Replace("{{WidgetDivHeight}}", divHeight);
            widgetHTML = widgetHTML.Replace("{{WidgetId}}", widgetId);
            return widgetHTML;
        }

        private string CurrentTranactionWidgetFormatting(PageWidget pageWidget, int counter, Page page, string divHeight)
        {
            var widgetId = "PageWidgetId_" + pageWidget.Identifier + "_Counter" + counter.ToString();
            var widgetHTML = HtmlConstants.CURRENT_TRANSACTION_WIDGET_HTML_FOR_STMT.Replace("{{AccountTransactionDetails}}", "{{AccountTransactionDetails_" + page.Identifier + "_" + pageWidget.Identifier + "}}").Replace("{{SelectOption}}", "{{SelectOption_" + page.Identifier + "_" + pageWidget.Identifier + "}}");
            widgetHTML = widgetHTML.Replace("{{WidgetDivHeight}}", divHeight);
            widgetHTML = widgetHTML.Replace("{{WidgetId}}", widgetId);
            return widgetHTML;
        }

        private string TopFourIncomeSourcesWidgetFormatting(PageWidget pageWidget, int counter, Page page, string divHeight)
        {
            var widgetId = "PageWidgetId_" + pageWidget.Identifier + "_Counter" + counter.ToString();
            var widgetHTML = HtmlConstants.TOP_4_INCOME_SOURCE_WIDGET_HTML_FOR_STMT.Replace("{{IncomeSourceList}}", "{{IncomeSourceList_" + page.Identifier + "_" + pageWidget.Identifier + "}}");
            widgetHTML = widgetHTML.Replace("{{WidgetDivHeight}}", divHeight);
            widgetHTML = widgetHTML.Replace("{{WidgetId}}", widgetId);
            return widgetHTML;
        }

        private string AnalyticsWidgetFormatting(PageWidget pageWidget, int counter, Page page, string divHeight)
        {
            var widgetId = "PageWidgetId_" + pageWidget.Identifier + "_Counter" + counter.ToString();
            var widgetHTML = HtmlConstants.ANALYTIC_WIDGET_HTML_FOR_STMT;
            widgetHTML = widgetHTML.Replace("{{WidgetDivHeight}}", divHeight);
            widgetHTML = widgetHTML.Replace("{{WidgetId}}", widgetId);
            return widgetHTML;
        }

        private string SavingTrendWidgetFormatting(PageWidget pageWidget, int counter, Page page, string divHeight)
        {
            var widgetId = "PageWidgetId_" + pageWidget.Identifier + "_Counter" + counter.ToString();
            var widgetHTML = HtmlConstants.SAVING_TRENDS_WIDGET_HTML_FOR_STMT;
            widgetHTML = widgetHTML.Replace("{{WidgetDivHeight}}", divHeight);
            widgetHTML = widgetHTML.Replace("{{WidgetId}}", widgetId);
            return widgetHTML;
        }

        private string SpendingTrendWidgetFormatting(PageWidget pageWidget, int counter, Page page, string divHeight)
        {
            var widgetId = "PageWidgetId_" + pageWidget.Identifier + "_Counter" + counter.ToString();
            var widgetHTML = HtmlConstants.SPENDING_TRENDS_WIDGET_HTML_FOR_STMT;
            widgetHTML = widgetHTML.Replace("{{WidgetDivHeight}}", divHeight);
            widgetHTML = widgetHTML.Replace("{{WidgetId}}", widgetId);
            return widgetHTML;
        }

        private string ReminderAndRecommendationWidgetFormatting(PageWidget pageWidget, int counter, Page page, string divHeight)
        {
            var widgetId = "PageWidgetId_" + pageWidget.Identifier + "_Counter" + counter.ToString();
            var widgetHTML = HtmlConstants.REMINDER_WIDGET_HTML_FOR_STMT.Replace("{{ReminderAndRecommdationDataList}}", "{{ReminderAndRecommdationDataList_" + page.Identifier + "_" + pageWidget.Identifier + "}}");
            widgetHTML = widgetHTML.Replace("{{WidgetDivHeight}}", divHeight);
            widgetHTML = widgetHTML.Replace("{{WidgetId}}", widgetId);
            return widgetHTML;
        }

        #endregion

        #region Image and Video Widget formatting

        private string ImageWidgetFormatting(PageWidget pageWidget, int counter, Statement statement, Page page, string divHeight)
        {
            var widgetId = "PageWidgetId_" + pageWidget.Identifier + "_Counter" + counter.ToString();
            var widgetHTML = new StringBuilder(HtmlConstants.IMAGE_WIDGET_HTML_FOR_STMT.Replace("{{ImageSource}}", "{{ImageSource_" + statement.Identifier + "_" + page.Identifier + "_" + pageWidget.Identifier + "}}"));
            widgetHTML.Replace("{{ImgAlignmentClass}}", "{{ImgAlignmentClass_" + statement.Identifier + "_" + page.Identifier + "_" + pageWidget.Identifier + "}}");
            widgetHTML.Replace("{{ImgHeight}}", "{{ImgHeight_" + statement.Identifier + "_" + page.Identifier + "_" + pageWidget.Identifier + "}}");
            widgetHTML.Replace("{{WidgetDivHeight}}", "auto");
            widgetHTML.Replace("{{WidgetId}}", widgetId);
            if (pageWidget.WidgetSetting != string.Empty && validationEngine.IsValidJson(pageWidget.WidgetSetting))
            {
                dynamic widgetSetting = JObject.Parse(pageWidget.WidgetSetting);
                if (widgetSetting.isPersonalize == false && widgetSetting.SourceUrl != null && widgetSetting.SourceUrl != string.Empty)
                {
                    widgetHTML.Replace("{{TargetLink}}", "<a href='" + widgetSetting.SourceUrl + "' target='_blank'>");
                    widgetHTML.Replace("{{EndTargetLink}}", "</a>");
                }
                else
                {
                    widgetHTML.Replace("{{TargetLink}}", string.Empty);
                    widgetHTML.Replace("{{EndTargetLink}}", string.Empty);
                }
            }
            return widgetHTML.ToString();
        }

        private string VideoWidgetFormatting(PageWidget pageWidget, int counter, Statement statement, Page page, string divHeight)
        {
            var widgetId = "PageWidgetId_" + pageWidget.Identifier + "_Counter" + counter.ToString();
            var widgetHTML = HtmlConstants.VIDEO_WIDGET_HTML_FOR_STMT.Replace("{{VideoSource}}", "{{VideoSource_" + statement.Identifier + "_" + page.Identifier + "_" + pageWidget.Identifier + "}}");
            widgetHTML = widgetHTML.Replace("{{WidgetDivHeight}}", "auto");
            widgetHTML = widgetHTML.Replace("{{WidgetId}}", widgetId);
            return widgetHTML;
        }

        #endregion

        #region StaticHtml Widget Formatting

        private string StaticHtmlWidgetFormatting(PageWidget pageWidget, int counter, Statement statement, Page page, string divHeight)
        {
            var widgetId = "PageWidgetId_" + pageWidget.Identifier + "_Counter" + counter.ToString();
            var widgetHTML = new StringBuilder(HtmlConstants.STATIC_HTML_WIDGET_HTML.Replace("{{StaticHtml}}", "{{StaticHtml_" + statement.Identifier + "_" + page.Identifier + "_" + pageWidget.Identifier + "}}"));
            widgetHTML.Replace("{{WidgetDivHeight}}", "auto");
            widgetHTML.Replace("{{WidgetId}}", "PageWidgetId_" + pageWidget.Identifier + "_Counter" + counter.ToString());
            return widgetHTML.ToString();
        }

        #endregion

        #region PageBreak Widget Formatting

        private string PageBreakWidgetFormatting(PageWidget pageWidget, int counter, Statement statement, Page page, string divHeight)
        {
            var widgetId = "PageWidgetId_" + pageWidget.Identifier + "_Counter" + counter.ToString();
            var widgetHTML = new StringBuilder(HtmlConstants.PAGE_BREAK_WIDGET_HTML.Replace("{{PageBreak}}", "{{PageBreak_" + statement.Identifier + "_" + page.Identifier + "_" + pageWidget.Identifier + "}}"));
            widgetHTML.Replace("{{WidgetDivHeight}}", "auto");
            widgetHTML.Replace("{{WidgetId}}", "PageWidgetId_" + pageWidget.Identifier + "_Counter" + counter.ToString());
            return widgetHTML.ToString();
        }

        #endregion

        #region SegmentBasedContent Widget Formatting

        private string SegmentBasedContentWidgetFormatting(PageWidget pageWidget, int counter, Statement statement, Page page, string divHeight)
        {
            var widgetId = "PageWidgetId_" + pageWidget.Identifier + "_Counter" + counter.ToString();
            var widgetHTML = new StringBuilder(HtmlConstants.SEGMENT_BASED_CONTENT_WIDGET_HTML.Replace("{{SegmentBasedContent}}", "{{SegmentBasedContent_" + statement.Identifier + "_" + page.Identifier + "_" + pageWidget.Identifier + "}}"));
            widgetHTML.Replace("{{WidgetDivHeight}}", "auto");
            widgetHTML.Replace("{{WidgetId}}", "PageWidgetId_" + pageWidget.Identifier + "_Counter" + counter.ToString());
            return widgetHTML.ToString();
        }

        #endregion

        #region Dynamic Widget formatting

        private string DynamicTableWidgetFormatting(CustomeTheme themeDetails, DynamicWidget dynawidget, PageWidget pageWidget, int counter, Statement statement, Page page, string divHeight)
        {
            var htmlWidget = HtmlConstants.TABLE_WIDEGT_FOR_STMT;
            htmlWidget = htmlWidget.Replace("{{WidgetDivHeight}}", divHeight);
            htmlWidget = htmlWidget.Replace("{{TableMaxHeight}}", (pageWidget.Height * 110) - 40 + "px");
            htmlWidget = this.ApplyStyleCssForDynamicTableAndFormWidget(htmlWidget, themeDetails);
            htmlWidget = htmlWidget.Replace("{{WidgetTitle}}", dynawidget.Title);
            htmlWidget = htmlWidget.Replace("{{WidgetId}}", "PageWidgetId_" + pageWidget.Identifier + "_Counter" + counter.ToString());
            var tableEntities = JsonConvert.DeserializeObject<List<DynamicWidgetTableEntity>>(dynawidget.WidgetSettings);
            var tableHeader = new StringBuilder();
            tableHeader.Append("<tr>" + string.Join("", tableEntities.Select(field => string.Format("<th>{0}</th> ", field.HeaderName))) + "</tr>");
            htmlWidget = htmlWidget.Replace("{{tableHeader}}", tableHeader.ToString());
            htmlWidget = htmlWidget.Replace("{{tableBody}}", "{{tableBody_" + page.Identifier + "_" + pageWidget.Identifier + "}}");
            return htmlWidget;
        }

        private string DynamicFormWidgetFormatting(CustomeTheme themeDetails, DynamicWidget dynawidget, PageWidget pageWidget, int counter, Statement statement, Page page, string divHeight)
        {
            var htmlWidget = HtmlConstants.FORM_WIDGET_FOR_STMT;
            htmlWidget = htmlWidget.Replace("{{WidgetDivHeight}}", divHeight);
            htmlWidget = this.ApplyStyleCssForDynamicTableAndFormWidget(htmlWidget, themeDetails);
            htmlWidget = htmlWidget.Replace("{{WidgetTitle}}", dynawidget.Title);
            htmlWidget = htmlWidget.Replace("{{WidgetId}}", "PageWidgetId_" + pageWidget.Identifier + "_Counter" + counter.ToString());
            htmlWidget = htmlWidget.Replace("{{FormData}}", "{{FormData_" + page.Identifier + "_" + pageWidget.Identifier + "}}");
            return htmlWidget;
        }

        private string DynamicBarGraphWidgetFormatting(CustomeTheme themeDetails, DynamicWidget dynawidget, PageWidget pageWidget, int counter, Statement statement, Page page, string divHeight)
        {
            var htmlWidget = HtmlConstants.BAR_GRAPH_FOR_STMT;
            htmlWidget = htmlWidget.Replace("{{WidgetDivHeight}}", divHeight);
            htmlWidget = htmlWidget.Replace("barGraphcontainer", "barGraphcontainer_" + page.Identifier + "_" + pageWidget.Identifier + "");
            htmlWidget = this.ApplyStyleCssForDynamicGraphAndChartWidgets(htmlWidget, themeDetails);
            htmlWidget = htmlWidget.Replace("{{WidgetTitle}}", dynawidget.Title);
            htmlWidget = htmlWidget.Replace("{{WidgetId}}", "PageWidgetId_" + pageWidget.Identifier + "_Counter" + counter.ToString());
            htmlWidget = htmlWidget + "<input type='hidden' id='hiddenBarGraphData_" + page.Identifier + "_" + pageWidget.Identifier + "' value='hiddenBarGraphValue_" + page.Identifier + "_" + pageWidget.Identifier + "'>";
            return htmlWidget;
        }

        private string DynamicLineGraphWidgetFormatting(CustomeTheme themeDetails, DynamicWidget dynawidget, PageWidget pageWidget, int counter, Statement statement, Page page, string divHeight)
        {
            var htmlWidget = HtmlConstants.LINE_GRAPH_FOR_STMT;
            htmlWidget = htmlWidget.Replace("lineGraphcontainer", "lineGraphcontainer_" + page.Identifier + "_" + pageWidget.Identifier + "");
            htmlWidget = htmlWidget.Replace("{{WidgetDivHeight}}", divHeight);
            htmlWidget = this.ApplyStyleCssForDynamicGraphAndChartWidgets(htmlWidget, themeDetails);
            htmlWidget = htmlWidget.Replace("{{WidgetTitle}}", dynawidget.Title);
            htmlWidget = htmlWidget.Replace("{{WidgetId}}", "PageWidgetId_" + pageWidget.Identifier + "_Counter" + counter.ToString());
            htmlWidget = htmlWidget + "<input type='hidden' id='hiddenLineGraphData_" + page.Identifier + "_" + pageWidget.Identifier + "' value='hiddenLineGraphValue_" + page.Identifier + "_" + pageWidget.Identifier + "'>";
            return htmlWidget;
        }

        private string DynamicPieChartWidgetFormatting(CustomeTheme themeDetails, DynamicWidget dynawidget, PageWidget pageWidget, int counter, Statement statement, Page page, string divHeight)
        {
            var htmlWidget = HtmlConstants.PIE_CHART_FOR_STMT;
            htmlWidget = htmlWidget.Replace("{{WidgetDivHeight}}", divHeight);
            htmlWidget = htmlWidget.Replace("pieChartcontainer", "pieChartcontainer_" + page.Identifier + "_" + pageWidget.Identifier + "");
            htmlWidget = this.ApplyStyleCssForDynamicGraphAndChartWidgets(htmlWidget, themeDetails);
            htmlWidget = htmlWidget.Replace("{{WidgetTitle}}", dynawidget.Title);
            htmlWidget = htmlWidget.Replace("{{WidgetId}}", "PageWidgetId_" + pageWidget.Identifier + "_Counter" + counter.ToString());
            htmlWidget = htmlWidget + "<input type='hidden' id='hiddenPieChartData_" + page.Identifier + "_" + pageWidget.Identifier + "' value='hiddenPieChartValue_" + page.Identifier + "_" + pageWidget.Identifier + "'>";
            return htmlWidget;
        }

        private string DynamicHtmlWidgetFormatting(CustomeTheme themeDetails, DynamicWidget dynawidget, PageWidget pageWidget, int counter, Statement statement, Page page, string divHeight)
        {
            var htmlWidget = HtmlConstants.HTML_WIDGET_FOR_STMT;
            htmlWidget = htmlWidget.Replace("{{WidgetDivHeight}}", divHeight);
            htmlWidget = this.ApplyStyleCssForDynamicTableAndFormWidget(htmlWidget, themeDetails);
            htmlWidget = htmlWidget.Replace("{{WidgetTitle}}", dynawidget.Title);
            htmlWidget = htmlWidget.Replace("{{WidgetId}}", "PageWidgetId_" + pageWidget.Identifier + "_Counter" + counter.ToString());
            htmlWidget = htmlWidget.Replace("{{FormData}}", "{{FormData_" + page.Identifier + "_" + pageWidget.Identifier + "}}");
            return htmlWidget;
        }

        #endregion

        #region Bind dummy data to financial tenant sample statement

        private void BindDummyDataToCustomerInformationWidget(StringBuilder pageContent, Statement statement, Page page, PageWidget widget, string AppBaseDirectory)
        {
            var customerInfoJson = "{'FirstName':'Laura','MiddleName':'J','LastName':'Donald','AddressLine1':'4000 Executive Parkway', 'AddressLine2':'Saint Globin Rd','City':'Canary Wharf', 'State':'London', 'Country':'England','Zip':'E14 9RZ'}";
            if (customerInfoJson != string.Empty && validationEngine.IsValidJson(customerInfoJson))
            {
                var customerInfo = JsonConvert.DeserializeObject<CustomerInformation>(customerInfoJson);
                pageContent.Replace("{{VideoSource_" + statement.Identifier + "_" + page.Identifier + "_" + widget.Identifier + "}}", AppBaseDirectory + "\\Resources\\sampledata\\SampleVideo.mp4");
                pageContent.Replace("{{CustomerName}}", customerInfo.FirstName + " " + customerInfo.SurName);
                pageContent.Replace("{{Address1}}", customerInfo.AddressLine1 + ", " + customerInfo.AddressLine2 + ", ");
                pageContent.Replace("{{Address2}}", customerInfo.AddressLine3 + ", " + customerInfo.AddressLine4 + ", ");
            }
        }
        private void BindDummyDataToProductSummaryWidget(StringBuilder pageContent, Statement statement, Page page, PageWidget widget, string AppBaseDirectory)
        {
            // JSON representation of product summary list
            string productSummaryListJson = "[{ 'Commission_Type': 'Safe Custody Fee', 'Prod_Group': 'Safe Custody Fee', 'AE_Amount': '52.65', 'QueryLink': 'https://www.google.com/', 'DR_CR': 'DR'}, { 'Commission_Type': 'Safe Custody Fee', 'Prod_Group': 'Service Fee', 'AE_Amount': '52.66', 'QueryLink': 'https://www.google.com/', 'DR_CR': 'DR'}, { 'Commission_Type': 'Safe Custody Fee', 'Prod_Group': 'Safe Custody Fee', 'AE_Amount': '152.67', 'QueryLink': 'https://www.google.com/', 'DR_CR': 'DR'}, { 'Commission_Type': 'Safe Custody Fee', 'Prod_Group': 'Service Fee', 'AE_Amount': '52.68', 'QueryLink': 'https://www.google.com/', 'DR_CR': 'CR'}]";

            // Check if productSummaryListJson is not empty and is valid JSON
            if (productSummaryListJson != string.Empty && validationEngine.IsValidJson(productSummaryListJson))
            {
                // Deserialize JSON to a list of payment details
                IList<spIAA_PaymentDetail> productSummary = JsonConvert.DeserializeObject<List<spIAA_PaymentDetail>>(productSummaryListJson);
                StringBuilder productSummarySrc = new StringBuilder();
                // Grouping records by Due date
                var gpCommissionTypeRecords = productSummary.GroupBy(gpCommissionTypeItem => new { gpCommissionTypeItem.Commission_Type }).ToList();

                // Grouping records by Fiduciary fees
                var prdocutDescriptionRecords = productSummary.GroupBy(gpPrdocutDescriptionItem => new { gpPrdocutDescriptionItem.Prod_Group }).ToList();

                // Initializing variables for column sums
                long index = 1;
                var aeAmountColSum = 0.00;
                var aeAmountColSumR = "";

                // Iterating through Due date groups
                gpCommissionTypeRecords.ForEach(gpCommissionTypeItem =>
                {
                    // Iterating through Fiduciary fees groups
                    prdocutDescriptionRecords.ForEach(gpPrdocutDescriptionItem =>
                    {
                        // Calculate sums for CR and DR amounts
                        var aeAmountCRSum = productSummary
                            .Where(witem => ((witem.Commission_Type == gpCommissionTypeItem.Key.Commission_Type) &&
                                             (witem.DR_CR == "CR"))).Sum(item => Convert.ToDouble(item.AE_Amount));

                        var aeAmountDRSum = productSummary
                            .Where(witem => ((witem.Commission_Type == gpCommissionTypeItem.Key.Commission_Type) &&
                                             (witem.DR_CR == "DR"))).Sum(item => Convert.ToDouble(item.AE_Amount));

                        // Calculate total AE Amount
                        var aeAmountSum = aeAmountCRSum - aeAmountDRSum;
                        var aeAmountSumR = CommonUtility.concatRWithDouble(aeAmountSum.ToString());

                        // Append the table row to productSummarySrc
                        productSummarySrc.Append("<tr><td align='center' valign='center' class='px-1 py-1 fsp-bdr-right fsp-bdr-bottom'>" + index + "</td><td class='fsp-bdr-right text-left fsp-bdr-bottom px-1'>" + gpCommissionTypeItem.Key.Commission_Type + "</td>" + "<td class='fsp-bdr-right text-left fsp-bdr-bottom px-1'> " + (gpPrdocutDescriptionItem.Key.Prod_Group == "Service Fee" ? "Premium Under Advise Fee" : gpPrdocutDescriptionItem.Key.Prod_Group) + "</td> <td class='text-right fsp-bdr-right fsp-bdr-bottom px-1'>" + aeAmountSumR + "</td><td class='text-left fsp-bdr-bottom px-1'><a  href ='https://www.google.com/' target='_blank'><img class='leftarrowlogo' src ='assets/images/leftarrowlogo.png' alt = 'Left Arrow'></a></td></tr>");

                        // Update column sum and increment index
                        aeAmountColSum += aeAmountSum;
                        productSummarySrc.Append("</tr>");
                        index++;
                    });
                    aeAmountColSumR = (aeAmountColSum == 0) ? "R0.00" : Utility.FormatCurrency(aeAmountColSum.ToString());
                });

                // Generate the HTML string for the product summary widget
                string productInfoJson = "{VAT_Amount : '38001.27'}";
                spIAA_PaymentDetail productInfo = JsonConvert.DeserializeObject<spIAA_PaymentDetail>(productInfoJson);

                // Replace placeholders in the HTML string with actual values
                pageContent.Replace("{{ProductSummary}}", productSummarySrc.ToString());
                pageContent.Replace("{{QueryBtn}}", ".. / common / images / IfQueryBtn.jpg");
                pageContent.Replace("{{ProductTotalDue}}", aeAmountColSumR);
                pageContent.Replace("{{VATDue}}", CommonUtility.concatRWithDouble(productInfo.VAT_Amount.ToString()));

                // Calculate grand total due
                double grandTotalDue = (Convert.ToDouble(aeAmountColSum) + Convert.ToDouble(productInfo.VAT_Amount));
                var grandTotalDueR = CommonUtility.concatRWithDouble(grandTotalDue.ToString());
                pageContent.Replace("{{GrandTotalDue}}", grandTotalDueR);

                // Calculate PPS payment and update the HTML string
                double ppsPayment = grandTotalDue;
                var ppsPaymentR = (ppsPayment == 0) ? "R0.00" : Utility.FormatCurrency(ppsPayment.ToString());
                pageContent.Replace("{{PPSPayment}}", ppsPaymentR);

                // Calculate and update the balance in the HTML string
                var balance = Convert.ToDouble((grandTotalDue - ppsPayment)).ToString("F2");
                // Calculate and update the balance in the HTML string
                pageContent.Replace("{{Balance}}", CommonUtility.concatRWithDouble(balance));
            }
        }

        private void BindDummyDataToDetailedTransactionsWidget(StringBuilder pageContent, Statement statement, Page page, PageWidget widget, string AppBaseDirectory)
        {

            //string productSummaryListJson = "[{ 'Commission_Type': 'Safe Custody Fee', 'Prod_Group':'Safe Custody Fee', 'Display_Amount': 'R52,65','QueryLink': 'https://www.google.com/'},{ 'Commission_Type': 'Safe Custody Fee', 'Prod_Group':'Service Fee', 'Display_Amount': 'R52,66', 'QueryLink': 'https://www.google.com/' }, { 'Commission_Type': 'Safe Custody Fee', 'Prod_Group':'Safe Custody Fee', 'Display_Amount': 'R52,67', 'QueryLink': 'https://www.google.com/' }, { 'Commission_Type': 'Safe Custody Fee', 'Prod_Group':'Service Fee', 'Display_Amount': 'R52,68', 'QueryLink': 'https://www.google.com/' } ]";

            //if (productSummaryListJson != string.Empty && validationEngine.IsValidJson(productSummaryListJson))
            //{
            //    IList<spIAA_PaymentDetail> productSummary = JsonConvert.DeserializeObject<List<spIAA_PaymentDetail>>(productSummaryListJson);
            //    StringBuilder productSummarySrc = new StringBuilder();
            //    long index = 1;
            //    productSummary.ToList().ForEach(item =>
            //    {
            //        productSummarySrc.Append("<tr><td align='center' valign='center' class='px-1 py-1 fsp-bdr-right fsp-bdr-bottom'>" + index + "</td><td class='fsp-bdr-right fsp-bdr-bottom px-1'>" + item.Commission_Type + "</td>" + "<td class='fsp-bdr-right fsp-bdr-bottom px-1'> " + (item.Prod_Group == "Service Fee" ? "Premium Under Advise Fee" : item.Prod_Group) + "</td> <td class='text-right fsp-bdr-right fsp-bdr-bottom px-1'>R" + item.Display_Amount + "</td><td class='text-left fsp-bdr-bottom px-1'><a  href ='https://www.google.com/' target='_blank'><img class='leftarrowlogo' src ='assets/images/leftarrowlogo.png' alt = 'Left Arrow'></a></td></tr>");
            //        index++;
            //    });
            //    pageContent.Replace("{{ProductSummary}}", productSummarySrc.ToString());
            //    string productInfoJson = "{Earning_Amount : '256670,66',VAT_Amount : '38001,27'}";
            //    spIAA_PaymentDetail productInfo = JsonConvert.DeserializeObject<spIAA_PaymentDetail>(productInfoJson);
            //    pageContent.Replace("{{QueryBtn}}", "assets/images/IfQueryBtn.jpg");
            //    pageContent.Replace("{{TotalDue}}", "R" + (Convert.ToDouble(productInfo.Earning_Amount)).ToString());
            //    pageContent.Replace("{{VATDue}}", CommonUtility.concatRWithDouble(productInfo.VAT_Amount.ToString()); 
            //    double grandTotalDue = (Convert.ToDouble(productInfo.Earning_Amount) + Convert.ToDouble(productInfo.VAT_Amount));
            //    pageContent.Replace("{{GrandTotalDue}}", "R" + grandTotalDue.ToString());
            //    double ppsPayment = grandTotalDue;
            //    pageContent.Replace("{{PPSPayment}}", "-R" + (grandTotalDue).ToString());
            //    pageContent.Replace("{{Balance}}", "R" + Convert.ToDouble((grandTotalDue - ppsPayment)).ToString());


            //  }
            string transactionListJson = "[{'INT_EXT_REF':'2164250','Int_Name':'Mr SCHOELER','Client_Name':'Kruger Van Heerden','Member_Ref':124556686,'Policy_Ref':5596100,'Description':'Safe Custody Service Fee','Commission_Type':'Safe Custody Fee','POSTED_DATE':'20-Mar-23','Display_Amount':'17.55','Query_Link':'https://www.google.com/','TYPE':'Fiduciary_Data','Prod_Group':'Safe Custody Fee'},{'INT_EXT_REF':'2164250','Int_Name':'Yvonne Van Heerden','Client_Name':'Mr SCHOELER','Member_Ref':124556686,'Policy_Ref':'5596100','Description':'Safe Custody Service Fee VAT','Commission_Type':'Safe Custody Fee','POSTED_DATE':'20-Mar-23','Display_Amount':'2.63','Query_Link':'https://www.google.com/','TYPE':'Fiduciary_Data','Prod_Group':'Safe Custody Fee'},{'INT_EXT_REF':'124411745','Int_Name':'Kruger Van Heerden','Client_Name':'DR N J Olivier','Member_Ref':'1217181','Policy_Ref':'5524069','Description':'Safe Custody Service Fee','Commission_Type':'Safe Custody Fee','POSTED_DATE':'20-Mar-23','Display_Amount':'17.55','Query_Link':'https://www.google.com/','TYPE':'Fiduciary_Data','Prod_Group':'Safe Custody Fee'},{'INT_EXT_REF':'124411745','Int_Name':'Kruger Van Heerden','Client_Name':'DR N J Olivier','Member_Ref':'124556686','Policy_Ref':'5596100','Description':'Safe Custody Service Fee VAT','Commission_Type':'Safe Custody Fee','POSTED_DATE':'20-Mar-23','Display_Amount':'2.63','Query_Link':'https://www.google.com/','TYPE':'Fiduciary_Data','Prod_Group':'VAT'}]";
            double TotalPostedAmount = 0;
            if (transactionListJson != string.Empty && validationEngine.IsValidJson(transactionListJson))
            {
                IList<spIAA_PaymentDetail> transaction = JsonConvert.DeserializeObject<List<spIAA_PaymentDetail>>(transactionListJson);
                StringBuilder detailedTransactionSrc = new StringBuilder();

                var records = transaction.GroupBy(gptransactionitem => gptransactionitem.INT_EXT_REF).ToList();
                records?.ForEach(transactionitem =>
                {
                    detailedTransactionSrc.Append("<div class='px-50'><div class='prouct-table-block'><div class='text-left fsp-transaction-title font-weight-bold mb-3'>Intermediary:  " + transactionitem.FirstOrDefault().INT_EXT_REF + " " + transactionitem.FirstOrDefault().Int_Name + "</div><table width='100%' cellpadding='0' cellspacing='0'> <tr><th class='font-weight-bold text-white'>Client name</th> <th class='font-weight-bold text-white text-left pe-0 bdr-r-0'>Member<br /> number</th> <th class='font-weight-bold text-white text-left'>Policy number</th> <th class='font-weight-bold text-white text-left'>Fiduciary fees</th> <th class='font-weight-bold text-white text-left'>Commission<br /> type</th> <th class='font-weight-bold text-white text-left'>Posted date</th> <th class='font-weight-bold text-white text-left'>Posted amount</th> <th class='font-weight-bold text-white'>Query</th> </tr> ");

                    pageContent.Replace("{{QueryBtnImgLink}}", "https://www.google.com/");
                    pageContent.Replace("{{QueryBtn}}", "../common/images/IfQueryBtn.jpg");


                    transaction.Where(witem => witem.INT_EXT_REF == transactionitem.FirstOrDefault().INT_EXT_REF).ToList().ForEach(item =>
                    {
                        detailedTransactionSrc.Append("<tr><td class='px-1 py-1 fsp-bdr-right fsp-bdr-bottom'>" +
                                item.Client_Name + "</td><td class= 'fsp-bdr-right fsp-bdr-bottom px-1 text-nowrap text-left'>" + item.Member_Ref + "</td><td class= 'fsp-bdr-right fsp-bdr-bottom px-1 text-nowrap text-left'> " + item.Policy_Ref + "</td><td class= 'text-left fsp-bdr-right fsp-bdr-bottom px-1'>" + (item.Description == "Commission Service Fee" ? "Premium Under Advise Fee" : item.Description) + "</td><td class= 'text-left fsp-bdr-right fsp-bdr-bottom px-1'>" + item.Commission_Type + "</td><td class= 'text-left text-nowrap fsp-bdr-right fsp-bdr-bottom px-1'>" + item.POSTED_DATE.ToString("dd-MMM-yyyy") + "</td><td class= 'text-right fsp-bdr-right fsp-bdr-bottom px-1'>" + Utility.FormatCurrency(item.Display_Amount) + "</td><td class= 'text-left fsp-bdr-bottom px-1'><a href ='https://www.google.com/' target ='_blank'><img class='leftarrowlogo' src='assets/images/leftarrowlogo.png' alt='Left Arrow'></a></td></tr>");
                        TotalPostedAmount += ((item.TYPE == "Fiduciary_Data") && (item.Prod_Group != "VAT")) ? (Convert.ToDouble(item.Display_Amount)) : 0.0;
                    });

                    detailedTransactionSrc.Append("<tr> <td align='center' valign='center' class='px-1 py-1 fsp-bdr-right fsp-bdr-bottom'></td> <td class='fsp-bdr-right fsp-bdr-bottom px-1 py-1'></td> <td class='fsp-bdr-right fsp-bdr-bottom px-1 py-1'></td> <td class='text-right fsp-bdr-right fsp-bdr-bottom px-1 py-1'></td> <td class='text-left fsp-bdr-right fsp-bdr-bottom px-1 py-1'><br /></td> <td class='text-left fsp-bdr-right fsp-bdr-bottom px-1 py-1'></td> <td class='text-left fsp-bdr-right fsp-bdr-bottom px-1 py-1'>" + Utility.FormatCurrency(TotalPostedAmount) + "</td> <td class='text-left fsp-bdr-bottom px-1'><a href='https://www.google.com/' target = '_blank' ><img src='assets/images/leftarrowlogo.png'></a></td> </tr></table><div class='text-right w-100 pt-3'><a href='https://www.google.com/' target = '_blank'></a></div></div></div></div>");

                });
                pageContent.Replace("{{detailedTransaction}}", detailedTransactionSrc.ToString());

            }
        }

        private void BindDummyDataToPpsDetailedTransactionsWidget(StringBuilder pageContent, Statement statement, Page page, PageWidget widget, string AppBaseDirectory)
        {
            string transactionListJson = "[{'INT_EXT_REF':'124565256 ','POLICY_REF':'3830102','MEMBER_REF':'10024365','Member_Name':'Dr JC Arthur','BUS_GROUP':'PPS INSURANCE','PRODUCT_DESCRIPTION':'Professional Health Provider Whole Life Professional Health',  'REQUESTED_DATETIME':'01-11-2022  00:00:00', 'REQUEST_DATETIME':'23-09-2022  00:00:00','TRANSACTION_AMOUNT':2265.4 ,'ALLOCATED_AMOUNT':-23107.08 ,'MEMBER_AGE':'45 ','MeasureType':'Commission','CommissionType':'2nd Year','FSP_Name':'Miss HW HLONGWANE'},     {'INT_EXT_REF':'124565256 ','POLICY_REF':'3830102','MEMBER_REF':'10024365','Member_Name':'Dr JC Arthur','BUS_GROUP':'PPS INSURANCE','PRODUCT_DESCRIPTION':'Professional Health Provider Whole Life Level Rated Professional Health CatchAll Exercised ',  'REQUESTED_DATETIME':'01-11-2022  00:00:00', 'REQUEST_DATETIME':'23-09-2022  00:00:00','TRANSACTION_AMOUNT':84.97 ,'ALLOCATED_AMOUNT':-866.69 ,'MEMBER_AGE':'45 ',  'MeasureType':'Commission','CommissionType':'2nd Year','FSP_Name':'Miss HW HLONGWANE'},     {'INT_EXT_REF':'124565256 ','POLICY_REF':'3830102','MEMBER_REF':'10024365','Member_Name':'Dr JC Arthur','BUS_GROUP':'PPS INSURANCE','PRODUCT_DESCRIPTION':'Professional Health Provider Whole Life Professional Health',  'REQUESTED_DATETIME':'01-11-2022  00:00:00', 'REQUEST_DATETIME':'23-09-2022  00:00:00','TRANSACTION_AMOUNT':2265.4,'ALLOCATED_AMOUNT':10968.98,'MEMBER_AGE':'45 ',  'MeasureType':'Commission','CommissionType':'2nd Year','FSP_Name':'Miss HW HLONGWANE'},     {'INT_EXT_REF':'124565256 ','POLICY_REF':'3820110 ','MEMBER_REF':'10436136 ','Member_Name':'Mnr JG Rossouw ','BUS_GROUP':'PPS INSURANCE','PRODUCT_DESCRIPTION':'Professional Health Provider Whole Life Professional Health',  'REQUESTED_DATETIME':'01-11-2022 00:00 ', 'REQUEST_DATETIME':'28-09-2022 00:00 ','TRANSACTION_AMOUNT':928.89 ,'ALLOCATED_AMOUNT':-9474.68 ,'MEMBER_AGE':'43',  'MeasureType':'Commission','CommissionType':'1nd Year','FSP_Name':'Miss HW HLONGWANE'},     {'INT_EXT_REF':'124565256 ','POLICY_REF':'3820110 ','MEMBER_REF':'10436136 ','Member_Name':'Mnr JG Rossouw ','BUS_GROUP':'PPS INSURANCE','PRODUCT_DESCRIPTION':'Professional Health Provider Whole Life Professional Health',  'REQUESTED_DATETIME':'01-11-2022 00:00 ', 'REQUEST_DATETIME':'28-09-2022 00:00 ','TRANSACTION_AMOUNT':928.89 ,'ALLOCATED_AMOUNT':6072.47 ,'MEMBER_AGE':'43',  'MeasureType':'Commission','CommissionType':'2nd Year','FSP_Name':'Miss HW HLONGWANE'}]  ";
            double TotalPostedAmount = 0;
            if (transactionListJson != string.Empty && validationEngine.IsValidJson(transactionListJson))
            {
                IList<spIAA_Commission_Detail> transaction = JsonConvert.DeserializeObject<List<spIAA_Commission_Detail>>(transactionListJson);
                StringBuilder detailedTransactionSrc = new StringBuilder();
                string detailedTransactionString = HtmlConstants.PPS_DETAILED_TRANSACTIONS_WIDGET_HTML;
                detailedTransactionSrc.Append("<div class='pps-monthly-table w-100'><table cellpadding='0' cellspacing='0' width='100%'><tr><th class='bdr-right-white sky-blue-bg text-white font-weight-bold'>Client<br/>name</th><th class='bdr-right-white sky-blue-bg text-white font-weight-bold'>Age</th><th class='bdr-right-white sky-blue-bg text-white font-weight-bold'>Policy #</th><th class='bdr-right-white sky-blue-bg text-white font-weight-bold'>Policy #</th><th class='bdr-right-white sky-blue-bg text-white font-weight-bold'>Product</th><th class='bdr-right-white sky-blue-bg text-white font-weight-bold'>Date<br/>issued</th><th class='bdr-right-white sky-blue-bg text-white font-weight-bold'>Inception<br/>date</th><th class='bdr-right-white sky-blue-bg text-white font-weight-bold'>Com<br/>type</th><th class='bdr-right-white sky-blue-bg text-white font-weight-bold'>Quantity</th><th class='bdr-right-white sky-blue-bg text-white font-weight-bold'>Posted<br/>date</th><th class='bdr-right-white sky-blue-bg text-white font-weight-bold'>Earnings</th></tr>");
                var records = transaction.GroupBy(gptransactionitem => gptransactionitem.BUS_GROUP).ToList();
                records?.ForEach(transactionitem =>
                {
                    detailedTransactionSrc.Append("<tr>" + transactionitem.FirstOrDefault().BUS_GROUP + "</tr>");
                    var memberRecords = transactionitem.GroupBy(gpmembertransactionitem => gpmembertransactionitem.BUS_GROUP).ToList();
                    transaction.Where(witem => witem.INT_EXT_REF == transactionitem.FirstOrDefault().INT_EXT_REF).ToList().ForEach(item =>
                    {
                        detailedTransactionSrc.Append(" <tr><td class='bdr-right-white'>" + item.Member_Name + "</td><td class='bdr-right-white'>" + item.MEMBER_AGE + "</td><td class='bdr-right-white'>" + item.POLICY_REF + "</td><td class='bdr-right-white'>" + item.PRODUCT_DESCRIPTION + "</td><td class='bdr-right-white'>" + item.REQUEST_DATETIME + "</td><td class='bdr-right-white'>" + item.REQUESTED_DATETIME.ToString("dd-MMM-yyyy") + "</td><td class='bdr-right-white'>" + item.CommissionType + "</td><td class='bdr-right-white'>" + item.TRANSACTION_AMOUNT + "</td><td class='bdr-right-white'>" + item.AE_Posted_Date.ToString("dd-MMM-yyyy") + "</td><td class='bdr-right-white'>" + item.ALLOCATED_AMOUNT + "</td></tr>");
                        TotalPostedAmount = (Convert.ToDouble(item.ALLOCATED_AMOUNT));
                    });
                    string TotalPostedAmountR = (TotalPostedAmount == 0) ? "R0.00" : Utility.FormatCurrency(TotalPostedAmount.ToString());
                    detailedTransactionSrc.Append("<tr><td class='dark-blue-bg text-white fw-bold '></td><td class='dark-blue-bg text-white fw-bold '></td><td class='dark-blue-bg text-white fw-bold '></td><td class='dark-blue-bg text-white fw-bold '></td><td class='dark-blue-bg text-white fw-bold '></td><td class='dark-blue-bg text-white fw-bold '></td><td class='dark-blue-bg text-white fw-bold '></td><td class='dark-blue-bg text-white fw-bold '></td><td class='dark-blue-bg text-white fw-bold fs-16'>Sub Total</td><td class='' ></td><td class='fw-bold fs-16' height='40'>" + TotalPostedAmountR + "</td></tr></table>");

                    //Adding button
                    detailedTransactionSrc.Append("<div class='text-center py-3'><a href='#'><img src='assets/images/IfQueryBtn.jpg'></a></div>");

                    detailedTransactionSrc.Append("</div>");

                    TotalPostedAmount = 0;

                });
                pageContent.Replace("{{ppsDetailedTransactions}}", detailedTransactionSrc.ToString());
            }
        }


        private void BindDummyDataToPaymentSummaryWidget(StringBuilder pageContent, Statement statement, Page page, PageWidget widget, string AppBaseDirectory)
        {
            var paymentInfoJson = "{Reg_ID : 1,Start_Date : '2023-01-01',End_Date : '2023-01-01',Request_DateTime : 'DummyText1',ID : '124529534',Intermediary_Code : 'DummyText1',FSP_ID : 'DummyText1',Policy_Number : 'DummyText1',FSP_Party_ID : 'DummyText1',Client_Number : '124556686',FSP_REF : '2452953',Client_Name : 'Mr SCHOELER',Int_ID : 'DummyText1',Product_Type : 'DummyText1',Commission_Amount : 'DummyText1',INT_EXT_REF : '124411745',Int_Name : 'Kruger Van Heerden',Int_Type : 'DummyText1',Policy_Ref : '5596100',Member_Ref : '124556686',Member_Name : 'DummyText1',Transaction_Amount : 'DummyText1',Mem_Age : 'DummyText1',Months_In_Force : 'DummyText1',Commission_Type : 'Safe Custody Fee',Description : 'Safe Custody Service Fee',POSTED_DATE : '2023-03-03',AE_Type_ID : 'DummyText1',AE_Amount : 'DummyText1',DR_CR : 'DummyText1',NAME : 'DummyText1',Member_Surname : 'DummyText1',Jurisdiction : 'DummyText1',Sales_Office : 'DummyText1',FSP_Name : 'Miss Yvonne van Heerden',FSP_Trading_Name : 'T/A Yvonne Van Heerden Financial Planner CC',FSP_Ext_Ref : '124529534',FSP_Kind : 'DummyText1',  		FSP_VAT_Number : '2452953',Product : 'DummyText1',Prod_Group : 'Service Fee',Prod_Seq : 'DummyText1',Report_Seq : 'DummyText1',TYPE : 'DummyText1',Display_Amount : '17.55',VAT_Amount : '38001.27',Earning_Amount : '256670.66',Payment_Amount : 'DummyText1',Business_Type : 'DummyText1',Lifecycle_Description : 'DummyText1',Lifecycle_Start_Date : 'DummyText1',AE_Scheduler_ID : 'DummyText1',VAT_Amount_1 : 'DummyText1',Final_Amount : 'DummyText1'}";
            if (paymentInfoJson != string.Empty && validationEngine.IsValidJson(paymentInfoJson))
            {
                var paymentInfo = JsonConvert.DeserializeObject<spIAA_PaymentDetail>(paymentInfoJson);
                pageContent.Replace("{{IntTotal}}", "R" + paymentInfo.Earning_Amount);
                pageContent.Replace("{{Vat}}", "R" + paymentInfo.VAT_Amount);
                pageContent.Replace("{{TotalDue}}", "R" + (Convert.ToDouble(paymentInfo.Earning_Amount) + Convert.ToDouble(paymentInfo.VAT_Amount)).ToString());

                // Format the date to month-year format                                                   
                pageContent.Replace("{{IntTotalDate}}", paymentInfo.POSTED_DATE.ToString("MMMM yyyy"));
                // Format the date with a custom format
                string formattedOrdinalDate = FormatDateWithOrdinal(paymentInfo.POSTED_DATE);
                pageContent.Replace("{{IntPostedDate}}", formattedOrdinalDate);
            }
        }

        private void BindDummyDataToPpsHeadingWidget(StringBuilder pageContent, Statement statement, Page page, PageWidget widget, string AppBaseDirectory)
        {
            var headingInfoJson = "{Reg_ID : 1,Start_Date : '2023-01-01',End_Date : '2023-01-01',Request_DateTime : 'DummyText1',ID : '124529534',Intermediary_Code : 'DummyText1',FSP_ID : 'DummyText1',Policy_Number : 'DummyText1',FSP_Party_ID : 'DummyText1',Client_Number : '124556686',FSP_REF : '2452953',Client_Name : 'Mr SCHOELER',Int_ID : 'DummyText1',Product_Type : 'DummyText1',Commission_Amount : 'DummyText1',INT_EXT_REF : '124411745',Int_Name : 'Kruger Van Heerden',Int_Type : 'DummyText1',Policy_Ref : '5596100',Member_Ref : '124556686',Member_Name : 'DummyText1',Transaction_Amount : 'DummyText1',Mem_Age : 'DummyText1',Months_In_Force : 'DummyText1',Commission_Type : 'Safe Custody Fee',Description : 'Safe Custody Service Fee',POSTED_DATE : '2023-03-03',AE_Type_ID : 'DummyText1',AE_Amount : 'DummyText1',DR_CR : 'DummyText1',NAME : 'DummyText1',Member_Surname : 'DummyText1',Jurisdiction : 'DummyText1',Sales_Office : 'DummyText1',FSP_Name : 'Miss Yvonne van Heerden',FSP_Trading_Name : 'T/A Yvonne Van Heerden Financial Planner CC',FSP_Ext_Ref : '124529534',FSP_Kind : 'DummyText1',  		FSP_VAT_Number : '2452953',Product : 'DummyText1',Prod_Group : 'Service Fee',Prod_Seq : 'DummyText1',Report_Seq : 'DummyText1',TYPE : 'DummyText1',Display_Amount : '17.55',VAT_Amount : '38001.27',Earning_Amount : '256670.66',Payment_Amount : 'DummyText1',Business_Type : 'DummyText1',Lifecycle_Description : 'DummyText1',Lifecycle_Start_Date : 'DummyText1',AE_Scheduler_ID : 'DummyText1',VAT_Amount_1 : 'DummyText1',Final_Amount : 'DummyText1'}";
            if (headingInfoJson != string.Empty && validationEngine.IsValidJson(headingInfoJson))
            {
                var headingInfo = JsonConvert.DeserializeObject<spIAA_PaymentDetail>(headingInfoJson);
                pageContent.Replace("{{FSPName}}", headingInfo.FSP_Name);
                pageContent.Replace("{{FSPTradingName}}", headingInfo.FSP_Trading_Name);
            }
        }

        private void BindDummyDataToPpsDetailsWidget(StringBuilder pageContent, Statement statement, Page page, PageWidget widget, string AppBaseDirectory)
        {
            var ppsDetailsInfoJson = "{Reg_ID : 1,Start_Date : '2023-01-01',End_Date : '2023-01-01',Request_DateTime : 'DummyText1',ID : '124529534',Intermediary_Code : 'DummyText1',FSP_ID : 'DummyText1',Policy_Number : 'DummyText1',FSP_Party_ID : 'DummyText1',Client_Number : '124556686',FSP_REF : '2452953',Client_Name : 'Mr SCHOELER',Int_ID : 'DummyText1',Product_Type : 'DummyText1',Commission_Amount : 'DummyText1',INT_EXT_REF : '124411745',Int_Name : 'Kruger Van Heerden',Int_Type : 'DummyText1',Policy_Ref : '5596100',Member_Ref : '124556686',Member_Name : 'DummyText1',Transaction_Amount : 'DummyText1',Mem_Age : 'DummyText1',Months_In_Force : 'DummyText1',Commission_Type : 'Safe Custody Fee',Description : 'Safe Custody Service Fee',POSTED_DATE : '2023-03-03',AE_Type_ID : 'DummyText1',AE_Amount : 'DummyText1',DR_CR : 'DummyText1',NAME : 'DummyText1',Member_Surname : 'DummyText1',Jurisdiction : 'DummyText1',Sales_Office : 'DummyText1',FSP_Name : 'Miss Yvonne van Heerden',FSP_Trading_Name : 'T/A Yvonne Van Heerden Financial Planner CC',FSP_Ext_Ref : '124529534',FSP_Kind : 'DummyText1',  		FSP_VAT_Number : '2452953',Product : 'DummyText1',Prod_Group : 'Service Fee',Prod_Seq : 'DummyText1',Report_Seq : 'DummyText1',TYPE : 'DummyText1',Display_Amount : '17.55',VAT_Amount : '38001.27',Earning_Amount : '256670.66',Payment_Amount : 'DummyText1',Business_Type : 'DummyText1',Lifecycle_Description : 'DummyText1',Lifecycle_Start_Date : 'DummyText1',AE_Scheduler_ID : 'DummyText1',VAT_Amount_1 : 'DummyText1',Final_Amount : 'DummyText1'}";
            if (ppsDetailsInfoJson != string.Empty && validationEngine.IsValidJson(ppsDetailsInfoJson))
            {
                var ppsDetailsInfo = JsonConvert.DeserializeObject<spIAA_PaymentDetail>(ppsDetailsInfoJson);
                pageContent.Replace("{{FSPNumber}}", ppsDetailsInfo.FSP_Ext_Ref);
                pageContent.Replace("{{FSPAgreeNumber}}", ppsDetailsInfo.FSP_REF);
                pageContent.Replace("{{VATRegNumber}}", ppsDetailsInfo.FSP_VAT_Number);
            }
        }

        private void BindDummyDataToPpsFooter1Widget(StringBuilder pageContent, Statement statement, Page page, PageWidget widget, string AppBaseDirectory)
        {
            var ppsFooter1InfoJson = "{Reg_ID : 1,Start_Date : '2023-01-01',End_Date : '2023-01-01',Request_DateTime : 'DummyText1',ID : '124529534',Intermediary_Code : 'DummyText1',FSP_ID : 'DummyText1',Policy_Number : 'DummyText1',FSP_Party_ID : 'DummyText1',Client_Number : '124556686',FSP_REF : '2452953',Client_Name : 'Mr SCHOELER',Int_ID : 'DummyText1',Product_Type : 'DummyText1',Commission_Amount : 'DummyText1',INT_EXT_REF : '124411745',Int_Name : 'Kruger Van Heerden',Int_Type : 'DummyText1',Policy_Ref : '5596100',Member_Ref : '124556686',Member_Name : 'DummyText1',Transaction_Amount : 'DummyText1',Mem_Age : 'DummyText1',Months_In_Force : 'DummyText1',Commission_Type : 'Safe Custody Fee',Description : 'Safe Custody Service Fee',POSTED_DATE : '2023-03-03',AE_Type_ID : 'DummyText1',AE_Amount : 'DummyText1',DR_CR : 'DummyText1',NAME : 'DummyText1',Member_Surname : 'DummyText1',Jurisdiction : 'DummyText1',Sales_Office : 'DummyText1',FSP_Name : 'Miss Yvonne van Heerden',FSP_Trading_Name : 'T/A Yvonne Van Heerden Financial Planner CC',FSP_Ext_Ref : '124529534',FSP_Kind : 'DummyText1',  		FSP_VAT_Number : '2452953',Product : 'DummyText1',Prod_Group : 'Service Fee',Prod_Seq : 'DummyText1',Report_Seq : 'DummyText1',TYPE : 'DummyText1',Display_Amount : '17.55',VAT_Amount : '38001.27',Earning_Amount : '256670.66',Payment_Amount : 'DummyText1',Business_Type : 'DummyText1',Lifecycle_Description : 'DummyText1',Lifecycle_Start_Date : 'DummyText1',AE_Scheduler_ID : 'DummyText1',VAT_Amount_1 : 'DummyText1',Final_Amount : 'DummyText1'}";
            if (ppsFooter1InfoJson != string.Empty && validationEngine.IsValidJson(ppsFooter1InfoJson))
            {
                string middleText = "PPS Insurance is a registered Insurer and FSP";
                string pageText = "Page 1/2";
                var ppsFooter1Info = JsonConvert.DeserializeObject<spIAA_PaymentDetail>(ppsFooter1InfoJson);
                pageContent.Replace("{{FSPFooterDetails}}", middleText);
                pageContent.Replace("{{FSPPage}}", pageText);

            }
        }

        private void BindDummyDataToFooterImageWidget(StringBuilder pageContent, Statement statement, Page page, PageWidget widget, string AppBaseDirectory)
        {
            var footerImageInfoJson = "{Reg_ID : 1,Start_Date : '2023-01-01',End_Date : '2023-01-01',Request_DateTime : 'DummyText1',ID : '124529534',Intermediary_Code : 'DummyText1',FSP_ID : 'DummyText1',Policy_Number : 'DummyText1',FSP_Party_ID : 'DummyText1',Client_Number : '124556686',FSP_REF : '2452953',Client_Name : 'Mr SCHOELER',Int_ID : 'DummyText1',Product_Type : 'DummyText1',Commission_Amount : 'DummyText1',INT_EXT_REF : '124411745',Int_Name : 'Kruger Van Heerden',Int_Type : 'DummyText1',Policy_Ref : '5596100',Member_Ref : '124556686',Member_Name : 'DummyText1',Transaction_Amount : 'DummyText1',Mem_Age : 'DummyText1',Months_In_Force : 'DummyText1',Commission_Type : 'Safe Custody Fee',Description : 'Safe Custody Service Fee',POSTED_DATE : '2023-03-03',AE_Type_ID : 'DummyText1',AE_Amount : 'DummyText1',DR_CR : 'DummyText1',NAME : 'DummyText1',Member_Surname : 'DummyText1',Jurisdiction : 'DummyText1',Sales_Office : 'DummyText1',FSP_Name : 'Miss Yvonne van Heerden',FSP_Trading_Name : 'T/A Yvonne Van Heerden Financial Planner CC',FSP_Ext_Ref : '124529534',FSP_Kind : 'DummyText1',  		FSP_VAT_Number : '2452953',Product : 'DummyText1',Prod_Group : 'Service Fee',Prod_Seq : 'DummyText1',Report_Seq : 'DummyText1',TYPE : 'DummyText1',Display_Amount : '17.55',VAT_Amount : '38001.27',Earning_Amount : '256670.66',Payment_Amount : 'DummyText1',Business_Type : 'DummyText1',Lifecycle_Description : 'DummyText1',Lifecycle_Start_Date : 'DummyText1',AE_Scheduler_ID : 'DummyText1',VAT_Amount_1 : 'DummyText1',Final_Amount : 'DummyText1'}";
            if (footerImageInfoJson != string.Empty && validationEngine.IsValidJson(footerImageInfoJson))
            {
                //string middleText = "PPS Insurance is a registered Insurer and FSP";
                //string pageText = "Page 1/2";
                var footerImageInfo = JsonConvert.DeserializeObject<spIAA_PaymentDetail>(footerImageInfoJson);
                //pageContent.Replace("{{FSPFooterDetails}}", middleText);
                //pageContent.Replace("{{FSPPage}}", pageText);

            }
        }

        private void BindDummyDataToEarningForPeriodWidget(StringBuilder pageContent, Statement statement, Page page, PageWidget widget, string AppBaseDirectory)
        {


            string commisionDetailListJson = "[{'Request_ID':1338675045,'AE_TYPE_ID':542051,'INT_EXT_REF':124565256,'POLICY_REF':3830102,'MEMBER_REF':10024365,'Member_Name':'Dr JC Arthur','BUS_GROUP':'PPS INSURANCE','PRODUCT_DESCRIPTION':'Professional Health Provider Whole Life Level Rated Professional Health CI 100 Cover','OID':542053,'MeasureType':'Commission','CommissionType':'2nd Year','TRANSACTION_AMOUNT':344.73,'ALLOCATED_AMOUNT':1172.08,'MEMBER_AGE':45,'MONTHS_IN_FORCE':0,'REQUEST_DATETIME':'2022-09-23 00:00:00.000','REQUESTED_DATETIME':'2022-11-01 00:00:00.000','AE_agmt_id':4755496,'AE_Posted_Date':'2022-09-23 00:00:00.000','AE_Amount':-1172.08,'Acc_Name':'Future Dated Provisions Account','FSP_Name':'NULL','DUE_DATE':'2023-10-07 00:00:00.000','YEAR_START_DATE':'2022-01-01 00:00:00.000','YEAR_END_DATE':'2022-12-31 23:59:00.000','Type':'Policy_Data'},{'Request_ID':1302220959,'AE_TYPE_ID':542051,'INT_EXT_REF':124565256,'POLICY_REF':3830102,'MEMBER_REF':10024365,'Member_Name':'Dr JC Arthur','BUS_GROUP':'PPS INSURANCE','PRODUCT_DESCRIPTION':'Professional Health Provider Whole Life Level Rated Professional Health CI 100 Cover','OID':542053,'MeasureType':'Commission','CommissionType':'1st Year','TRANSACTION_AMOUNT':344.73,'ALLOCATED_AMOUNT':1172.08,'MEMBER_AGE':45,'MONTHS_IN_FORCE':0,'REQUEST_DATETIME':'2022-09-23 00:00:00.000','REQUESTED_DATETIME':'2022-11-01 00:00:00.000','AE_agmt_id':4755496,'REQUESTED_DATETIME':'2022-09-23 00:00:00.000','AE_Amount':1172.08,'Acc_Name':'Future Dated Provisions Account','FSP_Name':'NULL','DUE_DATE':'2023-10-07 00:00:00.000','YEAR_START_DATE':'2022-01-01 00:00:00.000','YEAR_END_DATE':'2022-12-31 23:59:00.000','Type':'Policy_Data'},{'Request_ID':1338674952,'AE_TYPE_ID':541901,'INT_EXT_REF':124565256,'POLICY_REF':3820110,'MEMBER_REF':10436136,'Member_Name':'Mnr JG Rossouw','BUS_GROUP':'PPS INSURANCE','PRODUCT_DESCRIPTION':'Professional Health Provider Whole Life Level Rated Professional Health','OID':541991,'MeasureType':'Commission','CommissionType':'2nd Year','TRANSACTION_AMOUNT':928.89,'ALLOCATED_AMOUNT':9474.68,'MEMBER_AGE':43,'MONTHS_IN_FORCE':0,'REQUEST_DATETIME':'2022-10-28 00:00:00.000','REQUESTED_DATETIME':'2022-11-01 00:00:00.000','AE_agmt_id':4755496,'REQUESTED_DATETIME':'2022-10-28 00:00:00.000','AE_Amount':-9474.68,'Acc_Name':'Future Dated Provisions Account','FSP_Name':'NULL','DUE_DATE':'2022-10-07 00:00:00.000','YEAR_START_DATE':'2022-01-01 00:00:00.000','YEAR_END_DATE':'2022-12-31 23:59:00.000','Type':'Policy_Data'},{'Request_ID':1338675128,'AE_TYPE_ID':541901,'INT_EXT_REF':124565256,'POLICY_REF':3820110,'MEMBER_REF':10436136,'Member_Name':'Mnr JG Rossouw','BUS_GROUP':'PPS INSURANCE','PRODUCT_DESCRIPTION':'Professional Health Provider Whole Life Level Rated Professional Health','OID':541991,'MeasureType':'Commission','CommissionType':'1st Year','TRANSACTION_AMOUNT':928.89,'ALLOCATED_AMOUNT':6072.47,'MEMBER_AGE':43,'MONTHS_IN_FORCE':0,'REQUEST_DATETIME':'2022-10-28 00:00:00.000','REQUESTED_DATETIME':'2022-11-01 00:00:00.000','AE_agmt_id':4755496,'REQUESTED_DATETIME':'2022-09-28 00:00:00.000','AE_Amount':6072.47,'Acc_Name':'Future Dated Provisions Account','FSP_Name':'NULL','DUE_DATE':'2022-10-07 00:00:00.000','YEAR_START_DATE':'2022-01-01 00:00:00.000','YEAR_END_DATE':'2022-12-31 23:59:00.000','Type':'Policy_Data'},{'Request_ID':1338675045,'AE_TYPE_ID':542051,'INT_EXT_REF':124565256,'POLICY_REF':3830102,'MEMBER_REF':10024365,'Member_Name':'Dr JC Arthur','BUS_GROUP':'PPS INSURANCE','PRODUCT_DESCRIPTION':'Professional Health Provider Whole Life Level Rated Professional Health CI 100 Cover','OID':542053,'MeasureType':'Commission','CommissionType':'2nd Year','TRANSACTION_AMOUNT':344.73,'ALLOCATED_AMOUNT':1500.00,'MEMBER_AGE':45,'MONTHS_IN_FORCE':0,'REQUEST_DATETIME':'2022-09-23 00:00:00.000','REQUESTED_DATETIME':'2022-11-01 00:00:00.000','AE_agmt_id':4755496,'AE_Posted_Date':'2022-09-23 00:00:00.000','AE_Amount':-1172.08,'Acc_Name':'Future Dated Provisions Account','FSP_Name':'NULL','DUE_DATE':'2023-10-07 00:00:00.000','YEAR_START_DATE':'2022-01-01 00:00:00.000','YEAR_END_DATE':'2022-12-31 23:59:00.000','Type':'Policy_Data'},{'Request_ID':1338675045,'AE_TYPE_ID':542051,'INT_EXT_REF':124565256,'POLICY_REF':3830102,'MEMBER_REF':10024365,'Member_Name':'Dr JC Arthur','BUS_GROUP':'PPS INSURANCE','PRODUCT_DESCRIPTION':'Professional Health Provider Whole Life Level Rated Professional Health CI 100 Cover','OID':542053,'MeasureType':'Commission','CommissionType':'1st Year','TRANSACTION_AMOUNT':344.73,'ALLOCATED_AMOUNT':1000.00,'MEMBER_AGE':45,'MONTHS_IN_FORCE':0,'REQUEST_DATETIME':'2022-09-23 00:00:00.000','REQUESTED_DATETIME':'2022-11-01 00:00:00.000','AE_agmt_id':4755496,'AE_Posted_Date':'2022-09-23 00:00:00.000','AE_Amount':-1172.08,'Acc_Name':'Future Dated Provisions Account','FSP_Name':'NULL','DUE_DATE':'2023-10-07 00:00:00.000','YEAR_START_DATE':'2022-01-01 00:00:00.000','YEAR_END_DATE':'2022-12-31 23:59:00.000','Type':'Policy_Data'},{'Request_ID':1338675045,'AE_TYPE_ID':542051,'INT_EXT_REF':124565256,'POLICY_REF':3830102,'MEMBER_REF':10024365,'Member_Name':'Dr JC Arthur','BUS_GROUP':'PPS INSURANCE','PRODUCT_DESCRIPTION':'Professional Health Provider Whole Life Level Rated Professional Health CI 100 Cover','OID':542053,'MeasureType':'Commission','CommissionType':'2nd Year','TRANSACTION_AMOUNT':344.73,'ALLOCATED_AMOUNT':1800.00,'MEMBER_AGE':45,'MONTHS_IN_FORCE':0,'REQUEST_DATETIME':'2022-10-23 00:00:00.000','REQUESTED_DATETIME':'2022-11-01 00:00:00.000','AE_agmt_id':4755496,'AE_Posted_Date':'2022-09-23 00:00:00.000','AE_Amount':-1172.08,'Acc_Name':'Future Dated Provisions Account','FSP_Name':'NULL','DUE_DATE':'2023-10-07 00:00:00.000','YEAR_START_DATE':'2022-01-01 00:00:00.000','YEAR_END_DATE':'2022-12-31 23:59:00.000','Type':'Policy_Data'},{'Request_ID':1338675045,'AE_TYPE_ID':542051,'INT_EXT_REF':124565256,'POLICY_REF':3830102,'MEMBER_REF':10024365,'Member_Name':'Dr JC Arthur','BUS_GROUP':'PPS INSURANCE','PRODUCT_DESCRIPTION':'Professional Health Provider Whole Life Level Rated Professional Health CI 100 Cover','OID':542053,'MeasureType':'Commission','CommissionType':'1st Year','TRANSACTION_AMOUNT':344.73,'ALLOCATED_AMOUNT':2000.00,'MEMBER_AGE':45,'MONTHS_IN_FORCE':0,'REQUEST_DATETIME':'2022-10-23 00:00:00.000','REQUESTED_DATETIME':'2022-11-01 00:00:00.000','AE_agmt_id':4755496,'AE_Posted_Date':'2022-09-23 00:00:00.000','AE_Amount':-1172.08,'Acc_Name':'Future Dated Provisions Account','FSP_Name':'NULL','DUE_DATE':'2023-10-07 00:00:00.000','YEAR_START_DATE':'2022-01-01 00:00:00.000','YEAR_END_DATE':'2022-12-31 23:59:00.000','Type':'Policy_Data'},{'Request_ID':1338675045,'AE_TYPE_ID':542051,'INT_EXT_REF':124565256,'POLICY_REF':3830102,'MEMBER_REF':10024365,'Member_Name':'Dr JC Arthur','BUS_GROUP':'PPS INSURANCE','PRODUCT_DESCRIPTION':'Professional Health Provider Whole Life Level Rated Professional Health CI 100 Cover','OID':542053,'MeasureType':'Commission','CommissionType':'2nd Year','TRANSACTION_AMOUNT':344.73,'ALLOCATED_AMOUNT':1172.08,'MEMBER_AGE':45,'MONTHS_IN_FORCE':0,'REQUEST_DATETIME':'2022-09-23 00:00:00.000','REQUESTED_DATETIME':'2022-11-01 00:00:00.000','AE_agmt_id':4755496,'AE_Posted_Date':'2022-09-23 00:00:00.000','AE_Amount':-1172.08,'Acc_Name':'Future Dated Provisions Account','FSP_Name':'NULL','DUE_DATE':'2023-10-07 00:00:00.000','YEAR_START_DATE':'2022-01-01 00:00:00.000','YEAR_END_DATE':'2022-12-31 23:59:00.000','Type':'Fiduciary_Data'},{'Request_ID':1302220959,'AE_TYPE_ID':542051,'INT_EXT_REF':124565256,'POLICY_REF':3830102,'MEMBER_REF':10024365,'Member_Name':'Dr JC Arthur','BUS_GROUP':'PPS INSURANCE','PRODUCT_DESCRIPTION':'Professional Health Provider Whole Life Level Rated Professional Health CI 100 Cover','OID':542053,'MeasureType':'Commission','CommissionType':'1st Year','TRANSACTION_AMOUNT':344.73,'ALLOCATED_AMOUNT':1172.08,'MEMBER_AGE':45,'MONTHS_IN_FORCE':0,'REQUEST_DATETIME':'2022-09-23 00:00:00.000','REQUESTED_DATETIME':'2022-11-01 00:00:00.000','AE_agmt_id':4755496,'REQUESTED_DATETIME':'2022-09-23 00:00:00.000','AE_Amount':1172.08,'Acc_Name':'Future Dated Provisions Account','FSP_Name':'NULL','DUE_DATE':'2023-10-07 00:00:00.000','YEAR_START_DATE':'2022-01-01 00:00:00.000','YEAR_END_DATE':'2022-12-31 23:59:00.000','Type':'Fiduciary_Data'},{'Request_ID':1338674952,'AE_TYPE_ID':541901,'INT_EXT_REF':124565256,'POLICY_REF':3820110,'MEMBER_REF':10436136,'Member_Name':'Mnr JG Rossouw','BUS_GROUP':'PPS INSURANCE','PRODUCT_DESCRIPTION':'Professional Health Provider Whole Life Level Rated Professional Health','OID':541991,'MeasureType':'Commission','CommissionType':'2nd Year','TRANSACTION_AMOUNT':928.89,'ALLOCATED_AMOUNT':9474.68,'MEMBER_AGE':43,'MONTHS_IN_FORCE':0,'REQUEST_DATETIME':'2022-10-28 00:00:00.000','REQUESTED_DATETIME':'2022-11-01 00:00:00.000','AE_agmt_id':4755496,'REQUESTED_DATETIME':'2022-10-28 00:00:00.000','AE_Amount':-9474.68,'Acc_Name':'Future Dated Provisions Account','FSP_Name':'NULL','DUE_DATE':'2022-10-07 00:00:00.000','YEAR_START_DATE':'2022-01-01 00:00:00.000','YEAR_END_DATE':'2022-12-31 23:59:00.000','Type':'Fiduciary_Data'},{'Request_ID':1338675128,'AE_TYPE_ID':541901,'INT_EXT_REF':124565256,'POLICY_REF':3820110,'MEMBER_REF':10436136,'Member_Name':'Mnr JG Rossouw','BUS_GROUP':'PPS INSURANCE','PRODUCT_DESCRIPTION':'Professional Health Provider Whole Life Level Rated Professional Health','OID':541991,'MeasureType':'Commission','CommissionType':'1st Year','TRANSACTION_AMOUNT':928.89,'ALLOCATED_AMOUNT':6072.47,'MEMBER_AGE':43,'MONTHS_IN_FORCE':0,'REQUEST_DATETIME':'2022-10-28 00:00:00.000','REQUESTED_DATETIME':'2022-11-01 00:00:00.000','AE_agmt_id':4755496,'REQUESTED_DATETIME':'2022-09-28 00:00:00.000','AE_Amount':6072.47,'Acc_Name':'Future Dated Provisions Account','FSP_Name':'NULL','DUE_DATE':'2022-10-07 00:00:00.000','YEAR_START_DATE':'2022-01-01 00:00:00.000','YEAR_END_DATE':'2022-12-31 23:59:00.000','Type':'Fiduciary_Data'},{'Request_ID':1338675045,'AE_TYPE_ID':542051,'INT_EXT_REF':124565256,'POLICY_REF':3830102,'MEMBER_REF':10024365,'Member_Name':'Dr JC Arthur','BUS_GROUP':'PPS INSURANCE','PRODUCT_DESCRIPTION':'Professional Health Provider Whole Life Level Rated Professional Health CI 100 Cover','OID':542053,'MeasureType':'Commission','CommissionType':'2nd Year','TRANSACTION_AMOUNT':344.73,'ALLOCATED_AMOUNT':1500.00,'MEMBER_AGE':45,'MONTHS_IN_FORCE':0,'REQUEST_DATETIME':'2022-09-23 00:00:00.000','REQUESTED_DATETIME':'2022-11-01 00:00:00.000','AE_agmt_id':4755496,'AE_Posted_Date':'2022-09-23 00:00:00.000','AE_Amount':-1172.08,'Acc_Name':'Future Dated Provisions Account','FSP_Name':'NULL','DUE_DATE':'2023-10-07 00:00:00.000','YEAR_START_DATE':'2022-01-01 00:00:00.000','YEAR_END_DATE':'2022-12-31 23:59:00.000','Type':'Fiduciary_Data'},{'Request_ID':1338675045,'AE_TYPE_ID':542051,'INT_EXT_REF':124565256,'POLICY_REF':3830102,'MEMBER_REF':10024365,'Member_Name':'Dr JC Arthur','BUS_GROUP':'PPS INSURANCE','PRODUCT_DESCRIPTION':'Professional Health Provider Whole Life Level Rated Professional Health CI 100 Cover','OID':542053,'MeasureType':'Commission','CommissionType':'1st Year','TRANSACTION_AMOUNT':344.73,'ALLOCATED_AMOUNT':1000.00,'MEMBER_AGE':45,'MONTHS_IN_FORCE':0,'REQUEST_DATETIME':'2022-09-23 00:00:00.000','REQUESTED_DATETIME':'2022-11-01 00:00:00.000','AE_agmt_id':4755496,'AE_Posted_Date':'2022-09-23 00:00:00.000','AE_Amount':-1172.08,'Acc_Name':'Future Dated Provisions Account','FSP_Name':'NULL','DUE_DATE':'2023-10-07 00:00:00.000','YEAR_START_DATE':'2022-01-01 00:00:00.000','YEAR_END_DATE':'2022-12-31 23:59:00.000','Type':'Fiduciary_Data'},{'Request_ID':1338675045,'AE_TYPE_ID':542051,'INT_EXT_REF':124565256,'POLICY_REF':3830102,'MEMBER_REF':10024365,'Member_Name':'Dr JC Arthur','BUS_GROUP':'PPS INSURANCE','PRODUCT_DESCRIPTION':'Professional Health Provider Whole Life Level Rated Professional Health CI 100 Cover','OID':542053,'MeasureType':'Commission','CommissionType':'2nd Year','TRANSACTION_AMOUNT':344.73,'ALLOCATED_AMOUNT':1800.00,'MEMBER_AGE':45,'MONTHS_IN_FORCE':0,'REQUEST_DATETIME':'2022-10-23 00:00:00.000','REQUESTED_DATETIME':'2022-11-01 00:00:00.000','AE_agmt_id':4755496,'AE_Posted_Date':'2022-09-23 00:00:00.000','AE_Amount':-1172.08,'Acc_Name':'Future Dated Provisions Account','FSP_Name':'NULL','DUE_DATE':'2023-10-07 00:00:00.000','YEAR_START_DATE':'2022-01-01 00:00:00.000','YEAR_END_DATE':'2022-12-31 23:59:00.000','Type':'Fiduciary_Data'},{'Request_ID':1338675045,'AE_TYPE_ID':542051,'INT_EXT_REF':124565256,'POLICY_REF':3830102,'MEMBER_REF':10024365,'Member_Name':'Dr JC Arthur','BUS_GROUP':'PPS INSURANCE','PRODUCT_DESCRIPTION':'Professional Health Provider Whole Life Level Rated Professional Health CI 100 Cover','OID':542053,'MeasureType':'Commission','CommissionType':'1st Year','TRANSACTION_AMOUNT':344.73,'ALLOCATED_AMOUNT':2000.00,'MEMBER_AGE':45,'MONTHS_IN_FORCE':0,'REQUEST_DATETIME':'2022-10-23 00:00:00.000','REQUESTED_DATETIME':'2022-11-01 00:00:00.000','AE_agmt_id':4755496,'AE_Posted_Date':'2022-09-23 00:00:00.000','AE_Amount':-1172.08,'Acc_Name':'Future Dated Provisions Account','FSP_Name':'NULL','DUE_DATE':'2023-10-07 00:00:00.000','YEAR_START_DATE':'2022-01-01 00:00:00.000','YEAR_END_DATE':'2022-12-31 23:59:00.000','Type':'Fiduciary_Data'},]";
            double totalMonthlyEarnings = 0;


            if (commisionDetailListJson != string.Empty && validationEngine.IsValidJson(commisionDetailListJson))
            {
                // Deserialize JSON string to a list of commission details
                IList<spIAA_Commission_Detail> commisionDetail = JsonConvert.DeserializeObject<List<spIAA_Commission_Detail>>(commisionDetailListJson);

                // Initialize a StringBuilder to store HTML content
                StringBuilder commisionDetailSrc = new StringBuilder();
                string commisionDetailString = HtmlConstants.EARNINGS_FOR_PERIOD_WIDGET_HTML;

                // List to store monthly total amounts for each commission type
                List<List<double>> monthlyTotalList = new List<List<double>>();

                // Group commission details by month and type to get counts
                var records = commisionDetail.GroupBy(gpcommisionitem => new { gpcommisionitem.REQUEST_DATETIME.Month, gpcommisionitem.Type })
                    .Select(group => new
                    {
                        GroupKey = group.Key,
                        CountCommissionType = group.Count()
                    }).ToList();

                // Group commission details by CommissionType to get a list of commission types
                var gpCommisionType = commisionDetail.OrderBy(item => item.CommissionType).GroupBy(gpcommisionitem => new { gpcommisionitem.CommissionType })
                    .Select(group => new
                    {
                        GroupKey = group.Key
                    }).ToList();

                // HTML generation for Monthly production summary Section
                commisionDetailSrc.Append("<!-- Monthly production summary Section --><div class='earnings-section-monthly d-flex'><!-- Two Columns Layout --><div class='d-flex gap-1 w-100'><!-- Monthly production summary T1 --><div class='col-6'><!-- Heading for Monthly production summary T1 --><h4 class='monthly-production-summary skyblue-bg-title text-white text-center'>Monthly production summary</h4><div class='monthly-table'><!-- Table for Monthly production summary T1 --><table width='100%' cellpadding='0' cellspacing='0'><!-- Table Headers --><thead><tr><th class='text-white font-weight-bold text-left'>Month</th>");

                // HTML generation for table headers based on CommissionType
                gpCommisionType.ForEach(gpCommisionTypeitem =>
                {
                    commisionDetailSrc.Append("<th style='height:50px' class='text-left'> Premium Under Advice(" + gpCommisionTypeitem.GroupKey.CommissionType + ")</th>");
                });
                commisionDetailSrc.Append("</tr></thead>");

                // Iterate through grouped records to populate table rows
                records.ForEach(gpMonthRangeItem =>
                {
                    commisionDetailSrc.Append("<tr><td class='text-left'>" + CommonUtility.GetMonthRange(gpMonthRangeItem.GroupKey.Month) + "</td>");
                    List<double> innermonthlyTotalListSum = new List<double>();

                        // Iterate through CommissionType groups to populate cells
                        gpCommisionType.ForEach(gpCommisionTypeitem =>
                    {
                        var premiumUnderAdviceTd1Sum = commisionDetail
                            .Where(witem => ((witem.REQUEST_DATETIME.Month == gpMonthRangeItem.GroupKey.Month) &&
                                             (witem.CommissionType == gpCommisionTypeitem.GroupKey.CommissionType) &&
                                             (witem.Type == "Policy_Data")))
                            .Sum(item => Convert.ToDouble(item.ALLOCATED_AMOUNT));

                        var premiumUnderAdviceTd1SumR = (premiumUnderAdviceTd1Sum == 0) ? "R0.00" : Utility.FormatCurrency(premiumUnderAdviceTd1Sum.ToString());
                        commisionDetailSrc.Append("<td class='text-right'>" + premiumUnderAdviceTd1SumR + "</td>");
                        innermonthlyTotalListSum.Add(premiumUnderAdviceTd1Sum);
                    });

                        // Add monthly total to the list
                        monthlyTotalList.Add(innermonthlyTotalListSum);
                    commisionDetailSrc.Append("</tr>");
                });

                // Calculate column sums for monthly totals
                int numColumns = monthlyTotalList[0].Count;
                List<double> premiumColumnSums = new List<double>(Enumerable.Repeat(0.00, numColumns));

                foreach (var row in monthlyTotalList)
                {
                    for (int j = 0; j < numColumns; j++)
                    {
                        premiumColumnSums[j] += row[j];
                    }
                }

                // Add total row to the table
                commisionDetailSrc.Append("<tr><td class='text-left dark-blue-bg text-white font-weight-bold'>Total</td>");

                for (int k = 0; k < premiumColumnSums.Count; k++)
                {
                    totalMonthlyEarnings += premiumColumnSums[k];
                    commisionDetailSrc.Append("<td class='text-right font-weight-bold'>" + Utility.FormatCurrency(premiumColumnSums[k]) + "</ td>");
                }

                commisionDetailSrc.Append("</tr></table></div></div>");


                // Fees Section: Monthly Fiduciary Fees Summary
                List<List<double>> monthlyFessTotalList = new List<List<double>>();

                // HTML generation for Monthly Fiduciary Fees Summary Section
                commisionDetailSrc.Append("<!-- Monthly production summary T2 --><div class='col-6'><!-- Heading for Monthly production summary T2 --><h4 class='monthly-production-summary skyblue-bg-title text-white text-center'>Monthly production summary</h4><div class='monthly-table'><!-- Table for Monthly production summary T1 --><table width='100%' cellpadding='0' cellspacing='0'><!-- Table Headers --><thead><tr><th class='text-white font-weight-bold text-left'>Month</th>");

                // Generate table headers for each CommissionType in the group
                gpCommisionType.ForEach(gpCommisionTypeitem =>
                {
                    commisionDetailSrc.Append("<th style='height:50px' class='text-left'> Fiduciary Fees(" + gpCommisionTypeitem.GroupKey.CommissionType + ")</th>");
                });

                commisionDetailSrc.Append("</tr></thead>");

                // Iterate through grouped records to populate table rows for Fiduciary Fees
                records.ForEach(gpFeesMonthRangeItem =>
                {
                    commisionDetailSrc.Append("<tr><td class='text-left'>" + CommonUtility.GetMonthRange(gpFeesMonthRangeItem.GroupKey.Month) + "</td>");
                    List<double> innerFeesMonthlyTotalListSum = new List<double>();

                        // Iterate through CommissionType groups to populate cells with Fiduciary Fees
                        gpCommisionType.ForEach(gpCommisionTypeitem =>
                    {
                        var premiumUnderAdviceTd1Sum = commisionDetail
                            .Where(witem => ((witem.REQUEST_DATETIME.Month == gpFeesMonthRangeItem.GroupKey.Month) &&
                                             (witem.CommissionType == gpCommisionTypeitem.GroupKey.CommissionType) &&
                                             (witem.Type == "Fiduciary_Data")))
                            .Sum(item => Convert.ToDouble(item.ALLOCATED_AMOUNT));

                        var premiumUnderAdviceTd1SumR = (premiumUnderAdviceTd1Sum == 0) ? "R0.00" : Utility.FormatCurrency(premiumUnderAdviceTd1Sum.ToString());
                        commisionDetailSrc.Append("<td class='text-right'>" + premiumUnderAdviceTd1SumR + "</td>");
                        innerFeesMonthlyTotalListSum.Add(premiumUnderAdviceTd1Sum);
                    });

                        // Add monthly total to the list for Fiduciary Fees
                        monthlyFessTotalList.Add(innerFeesMonthlyTotalListSum);
                    commisionDetailSrc.Append("</tr>");
                });

                // Calculate column sums for Fiduciary Fees monthly totals
                int numFeesColumns = monthlyFessTotalList[0].Count;
                List<double> FeesColumnSums = new List<double>(Enumerable.Repeat(0.00, numFeesColumns));

                foreach (var row in monthlyFessTotalList)
                {
                    for (int j = 0; j < numFeesColumns; j++)
                    {
                        FeesColumnSums[j] += row[j];
                    }
                }

                // Add total row for Fiduciary Fees to the table
                commisionDetailSrc.Append("<tr><td class='text-left dark-blue-bg text-white font-weight-bold'>Total</td>");

                for (int k = 0; k < FeesColumnSums.Count; k++)
                {
                    totalMonthlyEarnings += FeesColumnSums[k];
                    commisionDetailSrc.Append("<td class='text-right font-weight-bold'>" + Utility.FormatCurrency(FeesColumnSums[k]) + "</ td>");
                }

                // Add total monthly earnings to the widget
                var totalMonthlyEarningsR = (totalMonthlyEarnings == 0) ? "R0.00" : Utility.FormatCurrency(totalMonthlyEarnings.ToString());
                commisionDetailSrc.Append("</tr></table></div></div>");
                commisionDetailSrc.Append("</div></div><!-- Total Earning Section --><div class='total-earning mb-4'><div class='row'><div class='col-3 text-right'></div><div class='col-3 text-right'><div class='dark-blue-bg text-white text-right font-weight-bold pe-3 py-1'>Total earnings</div></div><div class='col-3 text-left'><div class='total-price-title py-1 font-weight-bold text-center'>" + totalMonthlyEarningsR + "</div></div>");

                // Fees Section: Monthly Fiduciary Fees Summary - End

                // FSP account postings summary start

                // Grouping records by FSP Name
                var fspNameRecords = commisionDetail.GroupBy(gpcommisionitem => new { gpcommisionitem.FSP_Name })
                    .Select(group => new
                    {
                        GroupKey = group.Key,
                    }).ToList();

                // Iterating through FSP Name groups
                fspNameRecords.ForEach(gpfspNameItem =>
                {
                        // Initializing variables for monthly totals
                        List<List<double>> monthlyAEPostedTotalList = new List<List<double>>();
                    double totalAEPostedMonthlyEarnings = 0;

                        // Grouping records by AE_Posted_Date
                        var aePostedRecords = commisionDetail
                        .Where(witem => witem.FSP_Name == gpfspNameItem.GroupKey.FSP_Name)
                        .GroupBy(gpcommisionitem => new { gpcommisionitem.AE_Posted_Date.Date })
                        .Select(group => new
                        {
                            GroupKey = group.Key,
                        }).ToList();

                        // Appending HTML for the earnings section
                        commisionDetailSrc.Append("<!--FSPAccountPostingsSummarySection--><div class='earnings-section-monthly d-flex mb-2'><div class='d-flex gap-1 w-100'><!--FSP account postings summary--><div class='col-6'><!--Headingfor FSP Account PostingsSummary--><h4 class='monthly-production-summary skyblue-bg-title text-white text-center'>FSP account postings summary</h4><div class='monthly-table'><!--Table forFSPAccountPostingsSummary--><table width='100%' cellpadding='0' cellspacing='0'><!--TableHeaders--><thead><tr><th style='height:50px' class='text-white font-weight-bold'>Posted Date</th>");

                        // Appending HTML for Commission Type headers
                        gpCommisionType.ForEach(gpCommisionTypeitem =>
                    {
                        commisionDetailSrc.Append("<th style='height:50px' class='text-left'> Posted (" + gpCommisionTypeitem.GroupKey.CommissionType + ")</th>");
                    });

                        // Appending HTML for Premium under advice header
                        commisionDetailSrc.Append("<th style='height:50px' class='text-white font-weight-bold'>Premium under advice</th></tr></thead>");

                        // Appending HTML for Financial Service Provider row
                        //commisionDetailSrc.Append("<tr><th class='text-left text-white font-weight-bold' colspan=" + (2 + gpCommisionType.Count) + ">Financial Service Provider: " + gpfspNameItem.GroupKey.FSP_Name ?? "N/A" + "</th><tr>");

                        // Iterating through AE_Posted_Date groups
                        aePostedRecords.ForEach(gpMonthRangeItem =>
                    {
                            // Appending HTML for Date and Commission Type columns
                            commisionDetailSrc.Append("<tr><td class='text-left'>" + DateTime.Parse(gpMonthRangeItem.GroupKey.Date.ToString()).ToString("dd-MMM-yyyy") + "</td>");
                        List<double> innermonthlyAEPostedTotalListSum = new List<double>();

                            // Iterating through Commission Types
                            gpCommisionType.ForEach(gpCommisionTypeitem =>
                        {
                                // Calculating and appending HTML for Premium under advice
                                var premiumUnderAdviceTd1Sum = commisionDetail
                                .Where(witem => witem.AE_Posted_Date.Date == gpMonthRangeItem.GroupKey.Date && witem.CommissionType == gpCommisionTypeitem.GroupKey.CommissionType)
                                .Sum(item => Convert.ToDouble(item.ALLOCATED_AMOUNT));

                            var premiumUnderAdviceTd1SumR = (premiumUnderAdviceTd1Sum == 0) ? "R0.00" : Utility.FormatCurrency(premiumUnderAdviceTd1Sum.ToString());
                            commisionDetailSrc.Append("<td class='text-right'>" + premiumUnderAdviceTd1SumR + "</td>");
                            innermonthlyAEPostedTotalListSum.Add(premiumUnderAdviceTd1Sum);
                        });

                            // Calculating and appending HTML for Total Premium under advice
                            var postedAmountTdSum = commisionDetail
                            .Where(witem => witem.AE_Posted_Date.Date == gpMonthRangeItem.GroupKey.Date)
                            .Sum(item => Convert.ToDouble(item.ALLOCATED_AMOUNT));

                        var postedAmountTdSumR = (postedAmountTdSum == 0) ? "R0.00" : Utility.FormatCurrency(postedAmountTdSum.ToString());
                        commisionDetailSrc.Append("<td class='text-right'>" + Utility.FormatCurrency(postedAmountTdSumR) + "</td>");

                            // Adding inner list to monthly total list and closing row
                            monthlyAEPostedTotalList.Add(innermonthlyAEPostedTotalListSum);
                        commisionDetailSrc.Append("</tr>");
                    });

                        // Calculating and appending HTML for Total row
                        int numaePostedColumns = monthlyAEPostedTotalList[0].Count;
                    List<double> aePostedColumnSums = new List<double>(Enumerable.Repeat(0.00, numColumns));

                    foreach (var row in monthlyAEPostedTotalList)
                    {
                        for (int j = 0; j < numColumns; j++)
                        {
                            aePostedColumnSums[j] += row[j];
                        }
                    }

                    commisionDetailSrc.Append("<tr><td class='text-left dark-blue-bg text-white font-weight-bold'>Total</td>");

                    for (int k = 0; k < aePostedColumnSums.Count; k++)
                    {
                        totalAEPostedMonthlyEarnings += aePostedColumnSums[k];
                        commisionDetailSrc.Append("<td class='text-right font-weight-bold'>" + Utility.FormatCurrency(aePostedColumnSums[k]) + "</ td>");
                    }

                        // Appending HTML for Total Premium under advice and closing the table
                        var totalAEPostedMonthlyEarningsR = (totalAEPostedMonthlyEarnings == 0) ? "R0.00" : Utility.FormatCurrency(totalAEPostedMonthlyEarnings.ToString());
                    commisionDetailSrc.Append("<td class='text-right font-weight-bold'>" + Utility.FormatCurrency(totalAEPostedMonthlyEarningsR) + "</ td>");
                    commisionDetailSrc.Append("</tr></table></div></div>");

                        // Closing FSP account postings summary section
                    });

                // FSP account postings summary end

                // Future Date Production start

                // Grouping records by Due date
                var dueDateRecords = commisionDetail.GroupBy(gpDueDateitem => new { gpDueDateitem.DUE_DATE.Date })
                    .Select(group => new
                    {
                        GroupKey = group.Key,
                    }).ToList();

                // Grouping records by Fiduciary fees
                var prdocutDescriptionRecords = commisionDetail.GroupBy(gpDueDateitem => new { gpDueDateitem.PRODUCT_DESCRIPTION })
                    .Select(group => new
                    {
                        GroupKey = group.Key,
                    }).ToList();

                // Initializing variables for monthly totals
                List<List<double>> monthlyDueDateTotalList = new List<List<double>>();

                // Appending HTML for the Future-dated production section
                commisionDetailSrc.Append("<!-- Future-dated production Section --><div class='col-6'><!-- Heading for Future-dated production --><h4 class='monthly-production-summary skyblue-bg-title text-white text-center'>Future-dated production</h4><div class='monthly-table'><!-- Table for Future-dated production --><table width='100%' cellpadding='0' cellspacing='0'><!-- Table Headers --><thead><tr><th class='text-left text-white font-weight-bold'>Due date</th><th style='height:50px;' class='text-left'>Fiduciary fees</th><th class='text-left'>Allocated amount</th></tr></thead>");

                // Initializing variables for column sums
                double FutureColumnSums = 0.00;
                double sumOfAllocatedAmount = 0.00;
                double sumOfDueDateAllocatedAmount = 0.00;

                // Iterating through Due date groups
                dueDateRecords.ForEach(gpDueDateItem =>
                {
                        // Iterating through Fiduciary fees groups
                        prdocutDescriptionRecords.ForEach(prdocutDescription =>
                    {
                            // Appending HTML for Date, Fiduciary fees, and Allocated amount columns
                            commisionDetailSrc.Append("<tr><td class='text-left text-nowrap'>" + DateTime.Parse(gpDueDateItem.GroupKey.Date.ToString()).ToString("dd-MMM-yyyy") + "</td>");
                        sumOfAllocatedAmount = commisionDetail
                            .Where(witem => witem.PRODUCT_DESCRIPTION == prdocutDescription.GroupKey.PRODUCT_DESCRIPTION && witem.DUE_DATE == gpDueDateItem.GroupKey.Date)
                            .Sum(item => Convert.ToDouble(item.ALLOCATED_AMOUNT));

                        commisionDetailSrc.Append("<td class='text-left'>" + (prdocutDescription.GroupKey.PRODUCT_DESCRIPTION == "Commission Service Fee" ? "Premium Under Advise Fee" : prdocutDescription.GroupKey.PRODUCT_DESCRIPTION) + "</td>");
                        var sumOfAllocatedAmountR = (sumOfAllocatedAmount == 0) ? "R0.00" : Utility.FormatCurrency(sumOfAllocatedAmount.ToString());
                        commisionDetailSrc.Append("<td class='text-right'>" + sumOfAllocatedAmountR + "</td>");
                        FutureColumnSums += sumOfAllocatedAmount;
                        commisionDetailSrc.Append("</tr>");
                    });

                        // Appending HTML for SubTotal row
                        var sumOfDueDateAllocatedAmountR = (sumOfDueDateAllocatedAmount == 0) ? "R0.00" : Utility.FormatCurrency(sumOfDueDateAllocatedAmount.ToString());
                    commisionDetailSrc.Append("<tr><td class='text-right' colspan='2'>Sub Total<td class='text-right'>" + sumOfDueDateAllocatedAmountR + "</td></tr>");
                });

                // Appending HTML for Total earnings row and closing the table
                commisionDetailSrc.Append("<td class='dark-blue-bg text-white font-weight-bold text-right' colspan='2'>Total earnings</td><td class='text-right font-weight-bold'>" + Utility.FormatCurrency(FutureColumnSums) + "</td>");
                commisionDetailSrc.Append("</tr></table></div></div>");
                commisionDetailSrc.Append("</div></div>");
                // Future Date Production end
                // Replacing placeholder in the commisionDetailString with the specified divHeight value

                // Replacing placeholder in the commisionDetailString with the generated commissionDetailSrc content
                pageContent.Replace("{{ppsEarningForPeriod}}", commisionDetailSrc.ToString());
                // Appending the modified commisionDetailString to the htmlString
            }

            // JSON representation of product summary list
            string productSummaryListJson = "[{ 'Commission_Type': 'Safe Custody Fee', 'Prod_Group': 'Safe Custody Fee', 'AE_Amount': '52.65', 'QueryLink': 'https://www.google.com/', 'DR_CR': 'DR'}, { 'Commission_Type': 'Safe Custody Fee', 'Prod_Group': 'Service Fee', 'AE_Amount': '52.66', 'QueryLink': 'https://www.google.com/', 'DR_CR': 'DR'}, { 'Commission_Type': 'Safe Custody Fee', 'Prod_Group': 'Safe Custody Fee', 'AE_Amount': '152.67', 'QueryLink': 'https://www.google.com/', 'DR_CR': 'DR'}, { 'Commission_Type': 'Safe Custody Fee', 'Prod_Group': 'Service Fee', 'AE_Amount': '52.68', 'QueryLink': 'https://www.google.com/', 'DR_CR': 'CR'}]";

            // Check if productSummaryListJson is not empty and is valid JSON
            if (productSummaryListJson != string.Empty && validationEngine.IsValidJson(productSummaryListJson))
            {
                // Deserialize JSON to a list of payment details
                IList<spIAA_PaymentDetail> productSummary = JsonConvert.DeserializeObject<List<spIAA_PaymentDetail>>(productSummaryListJson);
                StringBuilder productSummarySrc = new StringBuilder();
                // Grouping records by Due date
                var gpCommissionTypeRecords = productSummary.GroupBy(gpCommissionTypeItem => new { gpCommissionTypeItem.Commission_Type }).ToList();

                // Grouping records by Fiduciary fees
                var prdocutDescriptionRecords = productSummary.GroupBy(gpPrdocutDescriptionItem => new { gpPrdocutDescriptionItem.Prod_Group }).ToList();

                // Initializing variables for column sums
                long index = 1;
                var aeAmountColSum = 0.00;
                var aeAmountColSumR = "";

                // Iterating through Due date groups
                gpCommissionTypeRecords.ForEach(gpCommissionTypeItem =>
                {
                    // Iterating through Fiduciary fees groups
                    prdocutDescriptionRecords.ForEach(gpPrdocutDescriptionItem =>
                    {
                        // Calculate sums for CR and DR amounts
                        var aeAmountCRSum = productSummary
                            .Where(witem => ((witem.Commission_Type == gpCommissionTypeItem.Key.Commission_Type) &&
                                             (witem.DR_CR == "CR"))).Sum(item => Convert.ToDouble(item.AE_Amount));

                        var aeAmountDRSum = productSummary
                            .Where(witem => ((witem.Commission_Type == gpCommissionTypeItem.Key.Commission_Type) &&
                                             (witem.DR_CR == "DR"))).Sum(item => Convert.ToDouble(item.AE_Amount));

                        // Calculate total AE Amount
                        var aeAmountSum = aeAmountCRSum - aeAmountDRSum;
                        var aeAmountSumR = CommonUtility.concatRWithDouble(aeAmountSum.ToString());

                        // Append the table row to productSummarySrc
                        productSummarySrc.Append("<tr><td align='center' valign='center' class='px-1 py-1 fsp-bdr-right fsp-bdr-bottom'>" + index + "</td><td class='fsp-bdr-right text-left fsp-bdr-bottom px-1'>" + gpCommissionTypeItem.Key.Commission_Type + "</td>" + "<td class='fsp-bdr-right text-left fsp-bdr-bottom px-1'> " + (gpPrdocutDescriptionItem.Key.Prod_Group == "Service Fee" ? "Premium Under Advise Fee" : gpPrdocutDescriptionItem.Key.Prod_Group) + "</td> <td class='text-right fsp-bdr-right fsp-bdr-bottom px-1'>" + aeAmountSumR + "</td><td class='text-left fsp-bdr-bottom px-1'><a  href ='https://www.google.com/' target='_blank'><img class='leftarrowlogo' src ='assets/images/leftarrowlogo.png' alt = 'Left Arrow'></a></td></tr>");

                        // Update column sum and increment index
                        aeAmountColSum += aeAmountSum;
                        productSummarySrc.Append("</tr>");
                        index++;
                    });
                    aeAmountColSumR = (aeAmountColSum == 0) ? "R0.00" : Utility.FormatCurrency(aeAmountColSum.ToString());
                });

                // Generate the HTML string for the product summary widget
                string productInfoJson = "{VAT_Amount : '38001.27'}";
                spIAA_PaymentDetail productInfo = JsonConvert.DeserializeObject<spIAA_PaymentDetail>(productInfoJson);

                // Replace placeholders in the HTML string with actual values
                pageContent.Replace("{{ProductSummary}}", productSummarySrc.ToString());
                pageContent.Replace("{{QueryBtn}}", ".. / common / images / IfQueryBtn.jpg");
                pageContent.Replace("{{ProductTotalDue}}", aeAmountColSumR);
                pageContent.Replace("{{VATDue}}", CommonUtility.concatRWithDouble(productInfo.VAT_Amount.ToString()));

                // Calculate grand total due
                double grandTotalDue = (Convert.ToDouble(aeAmountColSum) + Convert.ToDouble(productInfo.VAT_Amount));
                var grandTotalDueR = CommonUtility.concatRWithDouble(grandTotalDue.ToString());
                pageContent.Replace("{{GrandTotalDue}}", grandTotalDueR);

                // Calculate PPS payment and update the HTML string
                double ppsPayment = grandTotalDue;
                var ppsPaymentR = (ppsPayment == 0) ? "R0.00" : Utility.FormatCurrency(ppsPayment.ToString());
                pageContent.Replace("{{PPSPayment}}", ppsPaymentR);

                // Calculate and update the balance in the HTML string
                var balance = Convert.ToDouble((grandTotalDue - ppsPayment)).ToString("F2");
                // Calculate and update the balance in the HTML string
                pageContent.Replace("{{Balance}}", CommonUtility.concatRWithDouble(balance));
            }
        }
        private void BindDummyDataToPpsDetails1Widget(StringBuilder pageContent, Statement statement, Page page, PageWidget widget, string AppBaseDirectory)
        {
            DateTime DateFrom = new DateTime(2023, 01, 01);
            DateTime DateTo = new DateTime(2023, 09, 01);
            var ppsDetails1InfoJson = "{Request_ID : 1,AE_TYPE_ID : '20',INT_EXT_REF : '124529534',POLICY_REF : 'October',MEMBER_REF : 'Payment Details',Member_Name : 'DummyText1',BUS_GROUP : 'SERVICE FEES',PRODUCT_DESCRIPTION : 'Professional Health Provider Whole Life Professional Health',OID : 'DummyText1',MeasureType : 'Commission',CommissionType : '2nd Year',TRANSACTION_AMOUNT : 65566.20,ALLOCATED_AMOUNT : 65566.20,MEMBER_AGE : 'DummyText1',MONTHS_IN_FORCE : 'DummyText1',REQUEST_DATETIME : @DateFrom,REQUESTED_DATETIME : @AEPostedDate,AE_agmt_id : 'DummyText1',AE_agmt_type_id : '5596100',AE_Posted_Date : @AEPostedDate,AE_Amount : '65 566.20',Acc_Name : 'DummyText1',FSP_Name : 'Miss HW HLONGWANE',DUE_DATE : @AEDueDate,YEAR_START_DATE : @DateFrom,YEAR_END_DATE : @DateTo,Type : 'DummyText1',Req_Year : @DateFrom,FutureEndDate : @DateFrom,Calc1stYear : 10000,Calc2ndYear : 20000,MonthRange : 'DummyText1',calcMain2ndYear : 30000}";
            if (ppsDetails1InfoJson != string.Empty && validationEngine.IsValidJson(ppsDetails1InfoJson))
            {
                var ppsDetails1Info = JsonConvert.DeserializeObject<spIAA_Commission_Detail>(ppsDetails1InfoJson);
                pageContent.Replace("{{ref}}", ppsDetails1Info.INT_EXT_REF);
                pageContent.Replace("{{mtype}}", ppsDetails1Info.MeasureType);
                pageContent.Replace("{{month}}", DateFrom.ToString("MMMM yyyy"));
                pageContent.Replace("{{paramDate}}", DateFrom.ToString() + " To " + DateTo.ToString());


            }
        }
        private void BindDummyDataToPpsDetails2Widget(StringBuilder pageContent, Statement statement, Page page, PageWidget widget, string AppBaseDirectory)
        {
            DateTime DateFrom = new DateTime(2023, 01, 01);
            DateTime DateTo = new DateTime(2023, 09, 01);
            var ppsDetails2InfoJson = "{Request_ID : 1,AE_TYPE_ID : '20',INT_EXT_REF : '124529534',POLICY_REF : 'October',MEMBER_REF : 'Payment Details',Member_Name : 'DummyText1',BUS_GROUP : 'SERVICE FEES',PRODUCT_DESCRIPTION : 'Professional Health Provider Whole Life Professional Health',OID : 'DummyText1',MeasureType : 'Commission',CommissionType : '2nd Year',TRANSACTION_AMOUNT : 65566.20,ALLOCATED_AMOUNT : 65566.20,MEMBER_AGE : 'DummyText1',MONTHS_IN_FORCE : 'DummyText1',REQUEST_DATETIME : @DateFrom,REQUESTED_DATETIME : @AEPostedDate,AE_agmt_id : 'DummyText1',AE_agmt_type_id : '5596100',AE_Posted_Date : @AEPostedDate,AE_Amount : '65 566.20',Acc_Name : 'DummyText1',FSP_Name : 'Miss HW HLONGWANE',DUE_DATE : @AEDueDate,YEAR_START_DATE : @DateFrom,YEAR_END_DATE : @DateTo,Type : 'DummyText1',Req_Year : @DateFrom,FutureEndDate : @DateFrom,Calc1stYear : 10000,Calc2ndYear : 20000,MonthRange : 'DummyText1',calcMain2ndYear : 30000}";
            if (ppsDetails2InfoJson != string.Empty && validationEngine.IsValidJson(ppsDetails2InfoJson))
            {
                var ppsDetails2Info = JsonConvert.DeserializeObject<spIAA_Commission_Detail>(ppsDetails2InfoJson);
                //pageContent.Replace("{{ref}}", ppsDetails2Info.INT_EXT_REF);
                //pageContent.Replace("{{mtype}}", ppsDetails2Info.MeasureType);
                //pageContent.Replace("{{month}}", DateFrom.ToString("MMMM yyyy"));
                //pageContent.Replace("{{paramDate}}", DateFrom.ToString() + " To " + DateTo.ToString());


            }
        }

        private void BindDummyDataToAccountInformationWidget(StringBuilder pageContent, Page page, PageWidget widget)
        {
            var accountInfoJson = "{'StatementDate':'1-APR-2020','StatementPeriod':'Annual Statement', 'CustomerID':'ID2-8989-5656','RmName':'James Wiilims', 'RmContactNumber':'+4487867833'}";

            var accountInfoData = string.Empty;
            var AccDivData = new StringBuilder();
            if (accountInfoJson != string.Empty && validationEngine.IsValidJson(accountInfoJson))
            {
                var accountInfo = JsonConvert.DeserializeObject<AccountInformation>(accountInfoJson);
                AccDivData.Append("<div class='list-row-small ht70px'><div class='list-middle-row'> <div class='list-text'>" + "Statement Date</div><label class='list-value mb-0'>" + accountInfo.StatementDate + "</label>" + "</div></div>");

                AccDivData.Append("<div class='list-row-small ht70px'><div class='list-middle-row'> <div class='list-text'>" + "Statement Period</div><label class='list-value mb-0'>" + accountInfo.StatementPeriod + "</label></div></div>");

                AccDivData.Append("<div class='list-row-small ht70px'><div class='list-middle-row'> <div class='list-text'>" + "Cusomer ID</div><label class='list-value mb-0'>" + accountInfo.CustomerID + "</label></div></div>");

                AccDivData.Append("<div class='list-row-small ht70px'><div class='list-middle-row'> <div class='list-text'>" + "RM Name</div><label class='list-value mb-0'>" + accountInfo.RmName + "</label></div></div>");

                AccDivData.Append("<div class='list-row-small ht70px'><div class='list-middle-row'> <div class='list-text'>" + "RM Contact Number</div><label class='list-value mb-0'>" + accountInfo.RmContactNumber + "</label></div></div>");
            }
            pageContent.Replace("{{AccountInfoData_" + page.Identifier + "_" + widget.Identifier + "}}", AccDivData.ToString());
        }

        private void BindDummyDataToSummaryAtGlanceWidget(StringBuilder pageContent, Page page, PageWidget widget)
        {
            var accountBalanceDataJson = "[{\"AccountType\":\"Saving Account\",\"Currency\":\"$\",\"Amount\":\"87356\"},{\"AccountType\":\"Current Account\",\"Currency\":\"$\",\"Amount\":\"18654\"},{\"AccountType\":\"Recurring Account\",\"Currency\":\"$\",\"Amount\":\"54367\"},{\"AccountType\":\"Wealth\",\"Currency\"" + ":\"$\",\"Amount\":\"4589\"}]";

            var accountSummary = string.Empty;
            if (accountBalanceDataJson != string.Empty && validationEngine.IsValidJson(accountBalanceDataJson))
            {
                var lstAccountSummary = JsonConvert.DeserializeObject<List<AccountSummary>>(accountBalanceDataJson);
                if (lstAccountSummary.Count > 0)
                {
                    var accSummary = new StringBuilder();
                    lstAccountSummary.ForEach(acc =>
                    {
                        accSummary.Append("<tr><td>" + acc.AccountType + "</td><td>" + acc.Currency + "</td><td>" + acc.Amount + "</td></tr>");
                    });
                    pageContent.Replace("{{AccountSummary_" + page.Identifier + "_" + widget.Identifier + "}}", accSummary.ToString());
                }
            }
        }

        private void BindDummyDataToCurrentAvailBalanceWidget(StringBuilder pageContent, Page page, PageWidget widget)
        {
            var currentAvailBalanceJson = "{'GrandTotal':'32,453,23', 'TotalDeposit':'16,250,00', 'TotalSpend':'16,254,00', 'ProfitEarned':'1,430,00 ', 'Currency':'$', 'Balance': '14,768,80', 'AccountNumber': 'J566565TR678ER', 'AccountType': 'Current', 'Indicator': 'Up'}";
            if (currentAvailBalanceJson != string.Empty && validationEngine.IsValidJson(currentAvailBalanceJson))
            {
                var accountMaster = JsonConvert.DeserializeObject<AccountMaster>(currentAvailBalanceJson);
                var accountIndicatorClass = accountMaster.Indicator.ToLower().Equals("up") ? "fa fa-sort-asc text-success" : "fa fa-sort-desc text-danger";
                pageContent.Replace("{{AccountIndicatorClass}}", accountIndicatorClass);
                pageContent.Replace("{{TotalValue_" + page.Identifier + "_" + widget.Identifier + "}}", (accountMaster.Currency + accountMaster.GrandTotal));
                pageContent.Replace("{{TotalDeposit_" + page.Identifier + "_" + widget.Identifier + "}}", (accountMaster.Currency + accountMaster.TotalDeposit));
                pageContent.Replace("{{TotalSpend_" + page.Identifier + "_" + widget.Identifier + "}}", (accountMaster.Currency + accountMaster.TotalSpend));
                pageContent.Replace("{{Savings_" + page.Identifier + "_" + widget.Identifier + "}}", (accountMaster.Currency + accountMaster.ProfitEarned));
            }
        }

        private void BindDummyDataToSavingAvailBalanceWidget(StringBuilder pageContent, Page page, PageWidget widget)
        {
            var savingAvailBalanceJson = "{'GrandTotal':'26,453,23', 'TotalDeposit':'13,530,00', 'TotalSpend':'12,124,00', 'ProfitEarned':'2,340,00 ', 'Currency':'$', 'Balance': '19,456,80', 'AccountNumber': 'J566565TR678ER', 'AccountType': 'Saving', 'Indicator': 'Up'}";
            if (savingAvailBalanceJson != string.Empty && validationEngine.IsValidJson(savingAvailBalanceJson))
            {
                var accountMaster = JsonConvert.DeserializeObject<AccountMaster>(savingAvailBalanceJson);
                var accountIndicatorClass = accountMaster.Indicator.ToLower().Equals("up") ? "fa fa-sort-asc text-success" : "fa fa-sort-desc text-danger";
                pageContent.Replace("{{AccountIndicatorClass}}", accountIndicatorClass);
                pageContent.Replace("{{TotalValue_" + page.Identifier + "_" + widget.Identifier + "}}", (accountMaster.Currency + accountMaster.GrandTotal));
                pageContent.Replace("{{TotalDeposit_" + page.Identifier + "_" + widget.Identifier + "}}", (accountMaster.Currency + accountMaster.TotalDeposit));
                pageContent.Replace("{{TotalSpend_" + page.Identifier + "_" + widget.Identifier + "}}", (accountMaster.Currency + accountMaster.TotalSpend));
                pageContent.Replace("{{Savings_" + page.Identifier + "_" + widget.Identifier + "}}", (accountMaster.Currency + accountMaster.ProfitEarned));
            }
        }

        private void BindDummyDataToSavingTransactioneWidget(StringBuilder pageContent, StringBuilder scriptHtmlRenderer, Page page, PageWidget widget, List<FileData> SampleFiles, string AppBaseDirectory)
        {
            var selectOption = new StringBuilder();
            var scriptval = new StringBuilder();
            var distinctNaration = new string[] { "NXT TXN: IIFL IIFL6574562", "NXT TXN: IIFL IIFL6574563", "NXT TXN: IIFL IIFL3557346", "NXT TXN: IIFL RTED87978947 REFUND", "NXT TXN: IIFL IIFL896452896ERE", "NXT TXN: IIFL IIFL8965435", "NXT TXN: IIFL FGTR454565JHGKD", "NXT TXN: OFFICE RENT 798789DFGH", "NXT TXN: IIFL IIFL0034212", "NXT TXN: IIFL IIFL045678DFGH" };
            distinctNaration.ToList().ForEach(item =>
            {
                selectOption.Append("<option value='" + item + "'> " + item + "</option>");
            });

            var fileData = new FileData();
            fileData.FileName = "savingtransactiondetail.json";
            fileData.FileUrl = AppBaseDirectory + "\\Resources\\sampledata\\savingtransactiondetail.json";
            SampleFiles.Add(fileData);
            scriptHtmlRenderer.Append("<script type='text/javascript' src='./" + fileData.FileName + "'></script>");
            scriptval = new StringBuilder(HtmlConstants.SAVING_TRANSACTION_DETAIL_GRID_WIDGET_SCRIPT);
            scriptval.Replace("SavingTransactionTable", "SavingTransactionTable" + page.Identifier);
            scriptval.Replace("savingShowAll", "savingShowAll" + page.Identifier);
            scriptval.Replace("filterStatus", "filterStatus" + page.Identifier);
            scriptval.Replace("ResetGrid", "ResetGrid" + page.Identifier);
            scriptval.Replace("PrintGrid", "PrintGrid" + page.Identifier);
            scriptHtmlRenderer.Append(scriptval);

            pageContent.Replace("savingShowAll", "savingShowAll" + page.Identifier);
            pageContent.Replace("filterStatus", "filterStatus" + page.Identifier);
            pageContent.Replace("ResetGrid", "ResetGrid" + page.Identifier);
            pageContent.Replace("PrintGrid", "PrintGrid" + page.Identifier);
            pageContent.Replace("{{SelectOption_" + page.Identifier + "_" + widget.Identifier + "}}", selectOption.ToString());
            pageContent.Replace("SavingTransactionTable", "SavingTransactionTable" + page.Identifier);
            pageContent.Replace("{{AccountTransactionDetails_" + page.Identifier + "_" + widget.Identifier + "}}", string.Empty);
        }

        private void BindDummyToCurrentTransactioneWidget(StringBuilder pageContent, StringBuilder scriptHtmlRenderer, Page page, PageWidget widget, List<FileData> SampleFiles, string AppBaseDirectory)
        {
            var selectOption = new StringBuilder();
            var scriptval = new StringBuilder();
            var distintNaration = new string[] { "NXT TXN: IIFL IIFL6574562", "NXT TXN: IIFL IIFL6574563", "NXT TXN: IIFL IIFL3557346", "NXT TXN: IIFL RTED87978947 REFUND", "NXT TXN: IIFL IIFL896452896ERE", "NXT TXN: IIFL IIFL8965435", "NXT TXN: IIFL FGTR454565JHGKD", "NXT TXN: OFFICE RENT 798789DFGH", "NXT TXN: IIFL IIFL0034212", "NXT TXN: IIFL IIFL045678DFGH" };
            distintNaration.ToList().ForEach(item =>
            {
                selectOption.Append("<option value='" + item + "'> " + item + "</option>");
            });

            var fileData = new FileData();
            fileData.FileName = "currenttransactiondetail.json";
            fileData.FileUrl = AppBaseDirectory + "\\Resources\\sampledata\\currenttransactiondetail.json";
            SampleFiles.Add(fileData);
            scriptHtmlRenderer.Append("<script type='text/javascript' src='./" + fileData.FileName + "'></script>");
            scriptval = new StringBuilder(HtmlConstants.CURRENT_TRANSACTION_DETAIL_GRID_WIDGET_SCRIPT);
            scriptval.Replace("CurrentTransactionTable", "CurrentTransactionTable" + page.Identifier);
            scriptval.Replace("currentShowAll", "currentShowAll" + page.Identifier);
            scriptval.Replace("filterStatus", "filterStatus" + page.Identifier);
            scriptval.Replace("ResetGrid", "ResetGrid" + page.Identifier);
            scriptval.Replace("PrintGrid", "PrintGrid" + page.Identifier);
            scriptHtmlRenderer.Append(scriptval);

            pageContent.Replace("currentShowAll", "currentShowAll" + page.Identifier);
            pageContent.Replace("filterStatus", "filterStatus" + page.Identifier);
            pageContent.Replace("ResetGrid", "ResetGrid" + page.Identifier);
            pageContent.Replace("PrintGrid", "PrintGrid" + page.Identifier);
            pageContent.Replace("{{SelectOption_" + page.Identifier + "_" + widget.Identifier + "}}", selectOption.ToString());
            pageContent.Replace("CurrentTransactionTable", "CurrentTransactionTable" + page.Identifier);
            pageContent.Replace("{{AccountTransactionDetails_" + page.Identifier + "_" + widget.Identifier + "}}", string.Empty);
        }

        private void BindDummyDataToTopFourIncomeSourcesWidget(StringBuilder pageContent, Page page, PageWidget widget)
        {
            string incomeSourceListJson = "[{ 'Source': 'Salary Transfer', 'CurrentSpend': 3453, 'AverageSpend': 123},{ 'Source': 'Cash Deposit', 'CurrentSpend': 3453, 'AverageSpend': 6123},{ 'Source': 'Profit Earned', 'CurrentSpend': 3453, 'AverageSpend': 6123}, { 'Source': 'Rebete', 'CurrentSpend': 3453, 'AverageSpend': 123}]";
            if (incomeSourceListJson != string.Empty && validationEngine.IsValidJson(incomeSourceListJson))
            {
                var incomeSources = JsonConvert.DeserializeObject<List<IncomeSources>>(incomeSourceListJson);
                var incomestr = new StringBuilder();
                incomeSources.ForEach(item =>
                {
                    var tdstring = string.Empty;
                    if (Int32.Parse(item.CurrentSpend) > Int32.Parse(item.AverageSpend))
                    {
                        tdstring = "<span class='fa fa-sort-desc fa-2x text-danger' aria-hidden='true'></span><span class='ml-2'>" + item.AverageSpend + "</span>";
                    }
                    else
                    {
                        tdstring = "<span class='fa fa-sort-asc fa-2x mt-1' aria-hidden='true' style='position:relative;top:6px;color:limegreen'></span><span class='ml-2'>" + item.AverageSpend + "</span>";
                    }
                    incomestr.Append("<tr><td class='float-left'>" + item.Source + "</td>" + "<td> " + item.CurrentSpend + "</td><td>" + tdstring + "</td></tr>");
                });
                pageContent.Replace("{{IncomeSourceList_" + page.Identifier + "_" + widget.Identifier + "}}", incomestr.ToString());
            }
        }

        private void BindDummyDataToAnalyticsWidget(StringBuilder pageContent, StringBuilder scriptHtmlRenderer, Page page, List<FileData> SampleFiles, string AppBaseDirectory)
        {
            var fileData = new FileData();
            fileData.FileName = "analyticschartdata.json";
            fileData.FileUrl = AppBaseDirectory + "\\Resources\\sampledata\\analyticschartdata.json";
            SampleFiles.Add(fileData);
            scriptHtmlRenderer.Append("<script type='text/javascript' src='./" + fileData.FileName + "'></script>");
            pageContent.Replace("analyticschartcontainer", "analyticschartcontainer" + page.Identifier);
            scriptHtmlRenderer.Append(HtmlConstants.ANALYTICS_CHART_WIDGET_SCRIPT.Replace("analyticschartcontainer", "analyticschartcontainer" + page.Identifier));
        }

        private void BindDummyDataToSavingTrendWidget(StringBuilder pageContent, StringBuilder scriptHtmlRenderer, Page page, List<FileData> SampleFiles, string AppBaseDirectory)
        {
            var fileData = new FileData();
            fileData.FileName = "savingtrenddata.json";
            fileData.FileUrl = AppBaseDirectory + "\\Resources\\sampledata\\savingtrenddata.json";
            SampleFiles.Add(fileData);
            scriptHtmlRenderer.Append("<script type='text/javascript' src='" + fileData.FileName + "'></script>");
            pageContent.Replace("savingTrendscontainer", "savingTrendscontainer" + page.Identifier);
            scriptHtmlRenderer.Append(HtmlConstants.SAVING_TREND_CHART_WIDGET_SCRIPT.Replace("savingTrendscontainer", "savingTrendscontainer" + page.Identifier));
        }

        private void BindDummyDataToSpendingTrendWidget(StringBuilder pageContent, StringBuilder scriptHtmlRenderer, Page page, List<FileData> SampleFiles, string AppBaseDirectory)
        {
            var fileData = new FileData();
            fileData.FileName = "spendingtrenddata.json";
            fileData.FileUrl = AppBaseDirectory + "\\Resources\\sampledata\\spendingtrenddata.json";
            SampleFiles.Add(fileData);
            scriptHtmlRenderer.Append("<script type='text/javascript' src='" + fileData.FileName + "'></script>");
            pageContent.Replace("spendingTrendscontainer", "spendingTrendscontainer" + page.Identifier);
            scriptHtmlRenderer.Append(HtmlConstants.SPENDING_TREND_CHART_WIDGET_SCRIPT.Replace("spendingTrendscontainer", "spendingTrendscontainer" + page.Identifier));
        }

        private void BindDummyDataToReminderAndRecommendationWidget(StringBuilder pageContent, Page page, PageWidget widget)
        {
            string reminderJson = "[{ 'Title': 'Update Missing Inofrmation', 'Action': 'Update' },{ 'Title': 'Your Rewards Video is available', 'Action': 'View' },{ 'Title': 'Payment Due for Home Loan', 'Action': 'Pay' }, { title: 'Need financial planning for savings.', action: 'Call Me' },{ title: 'Subscribe/Unsubscribe Alerts.', action: 'Apply' },{ title: 'Your credit card payment is due now.', action: 'Pay' }]";
            if (reminderJson != string.Empty && validationEngine.IsValidJson(reminderJson))
            {
                var reminderAndRecommendations = JsonConvert.DeserializeObject<List<ReminderAndRecommendation>>(reminderJson);
                var reminderstr = new StringBuilder();
                reminderstr.Append("<div class='row'><div class='col-lg-9'></div><div class='col-lg-3 text-left'><i class='fa fa-caret-left fa-3x float-left text-danger' aria-hidden='true'></i><span class='mt-2 d-inline-block ml-2'>Click</span></div> </div>");
                reminderAndRecommendations.ForEach(item =>
                {
                    reminderstr.Append("<div class='row'><div class='col-lg-9 text-left'><p class='p-1' style='background-color: #dce3dc;'>" + item.Title + " </p></div><div class='col-lg-3 text-left'> <a><i class='fa fa-caret-left fa-3x float-left text-danger'></i><span class='mt-2 d-inline-block ml-2'>" + item.Action + "</span></a></div></div>");
                });
                pageContent.Replace("{{ReminderAndRecommdationDataList_" + page.Identifier + "_" + widget.Identifier + "}}", reminderstr.ToString());
            }
        }

        #endregion            

        #region Bind dummy data for Image and Video widget

        private void BindDummyDataToImageWidget(StringBuilder pageContent, Statement statement, Page page, PageWidget widget, List<FileData> SampleFiles, string AppBaseDirectory, string tenantCode)
        {
            var imgHeight = "auto";
            var imgAlignment = "text-center";

            var imageAssetPath = AppBaseDirectory + "\\Resources\\sampledata\\icon-image.png";
            if (widget.WidgetSetting != string.Empty && validationEngine.IsValidJson(widget.WidgetSetting))
            {
                dynamic widgetSetting = JObject.Parse(widget.WidgetSetting);
                if (widgetSetting.isPersonalize == false)
                {
                    var asset = assetLibraryRepository.GetAssets(new AssetSearchParameter { Identifier = widgetSetting.AssetId, SortParameter = new SortParameter { SortColumn = "Id" } }, tenantCode).ToList()?.FirstOrDefault();
                    if (asset != null)
                    {
                        var fileData = new FileData();
                        fileData.FileName = asset.Name;
                        fileData.FileUrl = asset.FilePath;
                        SampleFiles.Add(fileData);
                        imageAssetPath = "../common/media/" + fileData.FileName;
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
            }

            pageContent.Replace("{{ImgHeight_" + statement.Identifier + "_" + page.Identifier + "_" + widget.Identifier + "}}", imgHeight);
            pageContent.Replace("{{ImgAlignmentClass_" + statement.Identifier + "_" + page.Identifier + "_" + widget.Identifier + "}}", imgAlignment);
            pageContent.Replace("{{ImageSource_" + statement.Identifier + "_" + page.Identifier + "_" + widget.Identifier + "}}", imageAssetPath);
        }

        private void BindDummyDataToVideoWidget(StringBuilder pageContent, Statement statement, Page page, PageWidget widget, List<FileData> SampleFiles, string AppBaseDirectory, string tenantCode)
        {
            var videoAssetPath = AppBaseDirectory + "\\Resources\\sampledata\\SampleVideo.mp4";
            if (widget.WidgetSetting != string.Empty && validationEngine.IsValidJson(widget.WidgetSetting))
            {
                dynamic widgetSetting = JObject.Parse(widget.WidgetSetting);
                if (widgetSetting.isEmbedded == true)
                {
                    videoAssetPath = widgetSetting.SourceUrl;
                }
                else if (widgetSetting.isPersonalize == false && widgetSetting.isEmbedded == false)
                {
                    var asset = assetLibraryRepository.GetAssets(new AssetSearchParameter { Identifier = widgetSetting.AssetId, SortParameter = new SortParameter { SortColumn = "Id" } }, tenantCode).ToList()?.FirstOrDefault();
                    if (asset != null)
                    {
                        var fileData = new FileData();
                        fileData.FileName = asset.Name;
                        fileData.FileUrl = asset.FilePath;
                        SampleFiles.Add(fileData);
                        videoAssetPath = "../common/media/" + fileData.FileName;
                    }
                }
            }
            pageContent.Replace("{{VideoSource_" + statement.Identifier + "_" + page.Identifier + "_" + widget.Identifier + "}}", videoAssetPath);
        }

        #endregion

        #region Bind dummy data to Dynamic widget

        private void BindDummyDataToDynamicTableWidget(StringBuilder pageContent, Page page, PageWidget widget, DynamicWidget dynawidget)
        {
            pageContent.Replace("{{tableBody_" + page.Identifier + "_" + widget.Identifier + "}}", dynawidget.PreviewData);
        }

        private void BindDummyDataToDynamicFormWidget(StringBuilder pageContent, Page page, PageWidget widget, DynamicWidget dynawidget)
        {
            pageContent.Replace("{{FormData_" + page.Identifier + "_" + widget.Identifier + "}}", dynawidget.PreviewData);
        }

        private void BindDummyDataToDynamicLineGraphWidget(StringBuilder pageContent, StringBuilder scriptHtmlRenderer, Page page, PageWidget widget, DynamicWidget dynawidget)
        {
            pageContent.Replace("hiddenLineGraphValue_" + page.Identifier + "_" + widget.Identifier + "", dynawidget.PreviewData);
            scriptHtmlRenderer.Append(HtmlConstants.LINE_GRAPH_WIDGET_SCRIPT.Replace("linechartcontainer", "lineGraphcontainer_" + page.Identifier + "_" + widget.Identifier).Replace("hiddenLineGraphData", "hiddenLineGraphData_" + page.Identifier + "_" + widget.Identifier));
        }

        private void BindDummyDataToDynamicBarGraphWidget(StringBuilder pageContent, StringBuilder scriptHtmlRenderer, Page page, PageWidget widget, DynamicWidget dynawidget)
        {
            pageContent.Replace("hiddenBarGraphValue_" + page.Identifier + "_" + widget.Identifier + "", dynawidget.PreviewData);
            scriptHtmlRenderer.Append(HtmlConstants.BAR_GRAPH_WIDGET_SCRIPT.Replace("barchartcontainer", "barGraphcontainer_" + page.Identifier + "_" + widget.Identifier).Replace("hiddenBarGraphData", "hiddenBarGraphData_" + page.Identifier + "_" + widget.Identifier));
        }

        private void BindDummyDataToDynamicPieChartWidget(StringBuilder pageContent, StringBuilder scriptHtmlRenderer, Page page, PageWidget widget, DynamicWidget dynawidget)
        {
            pageContent.Replace("hiddenPieChartValue_" + page.Identifier + "_" + widget.Identifier + "", dynawidget.PreviewData);
            scriptHtmlRenderer.Append(HtmlConstants.PIE_CHART_WIDGET_SCRIPT.Replace("pieChartcontainer", "pieChartcontainer_" + page.Identifier + "_" + widget.Identifier).Replace("hiddenPieChartData", "hiddenPieChartData_" + page.Identifier + "_" + widget.Identifier));
        }

        private void BindDummyDataToDynamicHtmlWidget(StringBuilder pageContent, Page page, PageWidget widget, DynamicWidget dynawidget, string tenantCode)
        {
            var entityFieldMaps = this.dynamicWidgetManager.GetEntityFields(dynawidget.EntityId, tenantCode);
            var data = this.GetHTMLPreviewData(new TenantEntity()
            {
                Identifier = dynawidget.EntityId,
                Name = dynawidget.EntityName
            }, entityFieldMaps, dynawidget.PreviewData);
            pageContent.Replace("{{FormData_" + page.Identifier + "_" + widget.Identifier + "}}", data);
        }

        #endregion

        #region Bind dummy data to Static Html widget

        private void BindDummyDataToStaticHtmlWidget(StringBuilder pageContent, Statement statement, Page page, PageWidget widget)
        {
            var html = "<div>This is sample <b>HTML</b> for preview</div>";
            if (widget.WidgetSetting != string.Empty && validationEngine.IsValidJson(widget.WidgetSetting))
            {
                dynamic widgetSetting = JObject.Parse(widget.WidgetSetting);
                if (widgetSetting.html.ToString().Length > 0)
                {
                    html = widgetSetting.html;
                }
            }
            pageContent.Replace("{{StaticHtml_" + statement.Identifier + "_" + page.Identifier + "_" + widget.Identifier + "}}", html);
        }

        #endregion

        #region Bind dummy data to Page Break widget

        private void BindDummyDataToPageBreakWidget(StringBuilder pageContent, Statement statement, Page page, PageWidget widget)
        {
            var html = "<div style=\"page-break-before:always\">&nbsp;</div>";

            pageContent.Replace("{{PageBreak_" + statement.Identifier + "_" + page.Identifier + "_" + widget.Identifier + "}}", html);
        }

        #endregion

        #region Bind dummy data to SegmentBasedContent widget

        private void BindDummyDataToSegmentBasedContentWidget(StringBuilder pageContent, Statement statement, Page page, PageWidget widget)
        {
            var html = "<div>This is sample SegmentBasedContent</div>";
            if (widget.WidgetSetting != string.Empty && validationEngine.IsValidJson(widget.WidgetSetting))
            {
                dynamic widgetSetting = JArray.Parse(widget.WidgetSetting);
                if (widgetSetting[0].Html.ToString().Length > 0)
                {
                    html = widgetSetting[0].Html.ToString();
                }
            }
            pageContent.Replace("{{SegmentBasedContent_" + statement.Identifier + "_" + page.Identifier + "_" + widget.Identifier + "}}", html);
        }

        #endregion

        static string FormatDateWithOrdinal(DateTime date)
        {
            // Get the day of the month
            int day = date.Day;

            // Create an ordinal suffix (e.g., "st", "nd", "rd", "th")
            string ordinalSuffix;
            if (day % 100 >= 11 && day % 100 <= 13)
            {
                ordinalSuffix = "th";
            }
            else
            {
                switch (day % 10)
                {
                    case 1:
                        ordinalSuffix = "st";
                        break;
                    case 2:
                        ordinalSuffix = "nd";
                        break;
                    case 3:
                        ordinalSuffix = "rd";
                        break;
                    default:
                        ordinalSuffix = "th";
                        break;
                }
            }

            // Format the date with the ordinal suffix
            string formattedDate = $"{day}<sup>{ordinalSuffix}</sup> {date:MMMM}";

            return formattedDate;
        }
        #endregion
    }
}