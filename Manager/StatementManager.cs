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

                            case ModelConstant.PREVIEW_NEDBANK_STATEMENT_FUNCTION_NAME:
                                StatmentPreviewHtml = this.PreviewNedbankStatement(statementPages, tenantConfiguration, baseURL, tenantCode);
                                break;

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
        /// This method will geenerate HTML format of the Nedbank Statement
        /// </summary>
        /// <param name="statement">the statment</param>
        /// <param name="tenantCode">Tenant code of Statement.</param>
        /// <returns>
        /// Returns list of statement page content object.
        /// </returns>
        public IList<StatementPageContent> GenerateHtmlFormatOfNedbankStatement(Statement statement, string tenantCode, TenantConfiguration tenantConfiguration)
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
                                    var MarketingMessageCounter = 0;
                                    statementPageContent.PageId = page.Identifier;
                                    statementPageContent.PageTypeId = page.PageTypeId;
                                    statementPageContent.DisplayName = page.DisplayName;
                                    statement.Pages.Add(page);

                                    StringBuilder pageHeaderContent = new StringBuilder();
                                    pageHeaderContent.Append(HtmlConstants.NEDBANK_PAGE_HEADER_HTML);

                                    //pageHeaderContent.Append(HtmlConstants.NEDBANK_STATEMENT_HEADER.Replace("{{eConfirmLogo}}", "../common/images/eConfirm.png").Replace("{{NedBankLogo}}", "../common/images/NEDBANKLogo.png").Replace("{{StatementDate}}", DateTime.Now.ToString(ModelConstant.DATE_FORMAT_yyyy_MM_dd)));

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

                                                    var PaddingClass = string.Empty;
                                                    if (pageWidget.WidgetName == HtmlConstants.SERVICE_WIDGET_NAME)
                                                    {
                                                        //to add Nedbank services as a header for nedbank services div blocks...
                                                        if (MarketingMessageCounter == 0)
                                                        {
                                                            pageHtmlContent.Append("<div class='col-lg-12 col-sm-12'><div class='card border-0'><div class='card-body text-left py-0'><div class='card-body-header pb-2'>Nedbank Services</div></div></div></div></div><div class='row'>");
                                                        }
                                                        PaddingClass = MarketingMessageCounter % 2 == 0 ? " pr-1 pl-35px" : " pl-1 pr-35px";
                                                    }

                                                    //create new div with col-lg with newly finded div length and above padding zero value
                                                    pageHtmlContent.Append("<div class='col-lg-" + divLength + " col-sm-" + divLength + PaddingClass + "'>");

                                                    //check current widget is dynamic or static and start generating empty html template for current widget
                                                    if (!pageWidget.IsDynamicWidget)
                                                    {
                                                        switch (pageWidget.WidgetName)
                                                        {
                                                            case HtmlConstants.CUSTOMER_DETAILS_WIDGET_NAME:
                                                                if (statementPages.Count == 1)
                                                                {
                                                                    pageHtmlContent.Append(this.CustomerDetailsWidgetFormatting(pageWidget, counter, page));
                                                                }
                                                                break;

                                                            case HtmlConstants.BRANCH_DETAILS_WIDGET_NAME:
                                                                if (page.PageTypeName == HtmlConstants.HOME_LOAN_PAGE_TYPE)
                                                                {
                                                                    if (statementPages.Count == 1)
                                                                    {
                                                                        pageHtmlContent.Append(this.BranchDetailsWidgetFormatting(pageWidget, counter, page));
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    pageHtmlContent.Append(this.BranchDetailsWidgetFormatting(pageWidget, counter, page));
                                                                }
                                                                break;

                                                            case HtmlConstants.IMAGE_WIDGET_NAME:
                                                                pageHtmlContent.Append(this.ImageWidgetFormatting(pageWidget, counter, statement, page, divHeight));
                                                                break;

                                                            case HtmlConstants.VIDEO_WIDGET_NAME:
                                                                pageHtmlContent.Append(this.VideoWidgetFormatting(pageWidget, counter, statement, page, divHeight));
                                                                break;

                                                            case HtmlConstants.STATIC_HTML_WIDGET_NAME:
                                                                pageHtmlContent.Append(this.StaticHtmlWidgetFormatting(pageWidget, counter, statement, page, divHeight));
                                                                break;

                                                            case HtmlConstants.STATIC_SEGMENT_BASED_CONTENT_NAME:
                                                                pageHtmlContent.Append(this.SegmentBasedContentWidgetFormatting(pageWidget, counter, statement, page, divHeight));
                                                                break;

                                                            case HtmlConstants.INVESTMENT_PORTFOLIO_STATEMENT_WIDGET_NAME:
                                                                pageHtmlContent.Append(this.InvestmentPortfolioStatementWidgetFormatting(pageWidget, counter, page));
                                                                break;

                                                            case HtmlConstants.INVESTOR_PERFORMANCE_WIDGET_NAME:
                                                                pageHtmlContent.Append(this.InvestorPerformanceWidgetFormatting(pageWidget, counter, page));
                                                                break;

                                                            case HtmlConstants.BREAKDOWN_OF_INVESTMENT_ACCOUNTS_WIDGET_NAME:
                                                                pageHtmlContent.Append(this.BreakdownOfInvestmentAccountsWidgetFormatting(pageWidget, counter, page));
                                                                break;

                                                            case HtmlConstants.EXPLANATORY_NOTES_WIDGET_NAME:
                                                                pageHtmlContent.Append(this.ExplanatoryNotesWidgetFormatting(pageWidget, counter, page));
                                                                break;

                                                            case HtmlConstants.SERVICE_WIDGET_NAME:
                                                                pageHtmlContent.Append(this.MarketingServiceMessageWidgetFormatting(pageWidget, counter, page, MarketingMessageCounter));
                                                                MarketingMessageCounter++;
                                                                break;

                                                            case HtmlConstants.PERSONAL_LOAN_DETAIL_WIDGET_NAME:
                                                                pageHtmlContent.Append(this.PersonalLoanDetailWidgetFormatting(pageWidget, counter, page));
                                                                break;

                                                            case HtmlConstants.PERSONAL_LOAN_TRANASCTION_WIDGET_NAME:
                                                                pageHtmlContent.Append(this.PersonalLoanTransactionWidgetFormatting(pageWidget, counter, page));
                                                                break;

                                                            case HtmlConstants.PERSONAL_LOAN_PAYMENT_DUE_WIDGET_NAME:
                                                                pageHtmlContent.Append(this.PersonalLoanPaymentDueWidgetFormatting(pageWidget, counter, page));
                                                                break;

                                                            case HtmlConstants.SPECIAL_MESSAGE_WIDGET_NAME:
                                                                pageHtmlContent.Append(this.SpecialMessageWidgetFormatting(pageWidget, page));
                                                                break;

                                                            case HtmlConstants.PERSONAL_LOAN_INSURANCE_MESSAGE_WIDGET_NAME:
                                                                pageHtmlContent.Append(this.PersonalLoanInsuranceMessageWidgetFormatting(pageWidget, page));
                                                                break;

                                                            case HtmlConstants.PERSONAL_LOAN_TOTAL_AMOUNT_DETAIL_WIDGET_NAME:
                                                                pageHtmlContent.Append(this.PersonalLoanTotalAmountDetailWidgetFormatting(pageWidget, counter, page));
                                                                break;

                                                            case HtmlConstants.PERSONAL_LOAN_ACCOUNTS_BREAKDOWN_WIDGET_NAME:
                                                                pageHtmlContent.Append(this.PersonalLoanAccountsBreakdownsWidgetFormatting(pageWidget, counter, page));
                                                                break;

                                                            case HtmlConstants.HOME_LOAN_TOTAL_AMOUNT_DETAIL_WIDGET_NAME:
                                                                pageHtmlContent.Append(this.HomeLoanTotalAmountDetailWidgetFormatting(pageWidget, counter, page));
                                                                break;

                                                            case HtmlConstants.HOME_LOAN_ACCOUNTS_BREAKDOWN_WIDGET_NAME:
                                                                pageHtmlContent.Append(this.HomeLoanAccountsBreakdownsWidgetFormatting(pageWidget, counter, page));
                                                                break;

                                                            case HtmlConstants.NEDBANK_PORTFOLIO_CUSTOMER_DETAILS_WIDGET_NAME:
                                                                pageHtmlContent.Append(this.PortfolioCustomerDetailsWidgetFormatting(pageWidget, counter, page));
                                                                break;

                                                            case HtmlConstants.NEDBANK_PORTFOLIO_CUSTOMER_ADDRESS_WIDGET_NAME:
                                                                pageHtmlContent.Append(this.PortfolioCustomerAddressDetailsWidgetFormatting(pageWidget, counter, page));
                                                                break;

                                                            case HtmlConstants.NEDBANK_PORTFOLIO_CLIENT_CONTACT_DETAILS_WIDGET_NAME:
                                                                pageHtmlContent.Append(this.PortfolioClientContactDetailsWidgetFormatting(pageWidget, counter, page));
                                                                break;

                                                            case HtmlConstants.NEDBANK_PORTFOLIO_ACCOUNT_SUMMARY_WIDGET_NAME:
                                                                pageHtmlContent.Append(this.PortfolioAccountSummaryWidgetFormatting(pageWidget, counter, page));
                                                                break;

                                                            case HtmlConstants.NEDBANK_PORTFOLIO_ACCOUNT_ANALYSIS_WIDGET_NAME:
                                                                pageHtmlContent.Append(this.PortfolioAccountAnalysisWidgetFormatting(pageWidget, counter, page));
                                                                break;

                                                            case HtmlConstants.NEDBANK_PORTFOLIO_REMINDERS_WIDGET_NAME:
                                                                pageHtmlContent.Append(this.PortfolioRemindersWidgetFormatting(pageWidget, counter, page));
                                                                break;

                                                            case HtmlConstants.NEDBANK_PORTFOLIO_NEWS_ALERT_WIDGET_NAME:
                                                                pageHtmlContent.Append(this.PortfolioNewsAlertWidgetFormatting(pageWidget, counter, page));
                                                                break;

                                                            case HtmlConstants.NEDBANK_GREENBACKS_TOTAL_REWARDS_POINTS_WIDGET_NAME:
                                                                pageHtmlContent.Append(this.GreenbacksTotalRewardPointsWidgetFormatting(pageWidget, counter, page));
                                                                break;

                                                            case HtmlConstants.NEDBANK_GREENBACKS_CONTACT_US_WIDGET_NAME:
                                                                pageHtmlContent.Append(this.GreenbacksContactUsWidgetFormatting(pageWidget, counter, tenantCode));
                                                                break;

                                                            case HtmlConstants.NEDBANK_YTD_REWARDS_POINTS_BAR_GRAPH_WIDGET_NAME:
                                                                pageHtmlContent.Append(this.GreenbacksYTDRewardPointsGraphWidgetFormatting(pageWidget, counter, page));
                                                                break;

                                                            case HtmlConstants.NEDBANK_POINTS_REDEEMED_YTD_BAR_GRAPH_WIDGET_NAME:
                                                                pageHtmlContent.Append(this.GreenbacksPointsRedeemedYTDGraphWidgetFormatting(pageWidget, counter, page));
                                                                break;

                                                            case HtmlConstants.NEDBANK_PRODUCT_RELATED_POINTS_EARNED_BAR_GRAPH_WIDGET_NAME:
                                                                pageHtmlContent.Append(this.GreenbacksProductRelatedPointsEarnedGraphWidgetFormatting(pageWidget, counter, page));
                                                                break;

                                                            case HtmlConstants.NEDBANK_CATEGORY_SPEND_REWARDS_PIE_CHART_WIDGET_NAME:
                                                                pageHtmlContent.Append(this.GreenbacksCategorySpendPointsGraphWidgetFormatting(pageWidget, counter, page));
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

                                    //var footerContent = new StringBuilder(HtmlConstants.NEDBANK_STATEMENT_FOOTER.Replace("{{NedbankSloganImage}}", "../common/images/See_money_differently.PNG").Replace("{{NedbankNameImage}}", "../common/images/NEDBANK_Name.png").Replace("{{FooterText}}", HtmlConstants.NEDBANK_STATEMENT_FOOTER_TEXT_STRING));
                                    //var lastFooterText = string.Empty;
                                    //if (page.PageTypeName == HtmlConstants.HOME_LOAN_PAGE_TYPE)
                                    //{
                                    //    lastFooterText = "<div class='text-center mb-n2'> Directors: V Naidoo (Chairman) MWT Brown (Chief Executive) HR Body BA Dames NP Dongwana EM Kruger RAG Leiht </div> <div class='text-center mb-n2'> L Makalima PM Makwana Prof T Marwala Dr MA Matooane RK Morathi (Chief Finance Officer) MC Nkuhlu (Chief Operating Officer) </div> <div class='text-center mb-n2'> S Subramoney IG Williamson Company Secretory: J Katzin 01.06.2020 </div>";
                                    //}
                                    //footerContent.Replace("{{LastFooterText}}", lastFooterText);
                                    //statementPageContent.PageFooterContent = footerContent.ToString() + HtmlConstants.WIDGET_HTML_FOOTER;
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

                        newPageContent.Append("<div id='" + (page.PageTypeName == HtmlConstants.SAVING_ACCOUNT_PAGE ? "Saving" : "Current") + "-6789' class='tab-pane fade in active show'>");
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

                                case HtmlConstants.ACCOUNT_INFORMATION_WIDGET_NAME:
                                    this.BindDummyDataToAccountInformationWidget(pageContent, page, widget);
                                    break;

                                case HtmlConstants.IMAGE_WIDGET_NAME:
                                    this.BindDummyDataToImageWidget(pageContent, statement, page, widget, SampleFiles, AppBaseDirectory, tenantCode);
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

        /// <summary>
        /// This method help to bind data to common nedbank statement
        /// </summary>
        /// <param name="statement"> the statement object </param>
        /// <param name="statementPageContents"> the statement page html content list</param>
        /// <param name="tenantCode"> the tenant code </param>
        public StatementPreviewData BindDataToCommonNedbankStatement(Statement statement, IList<StatementPageContent> statementPageContents, string tenantCode)
        {
            try
            {
                var AppBaseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                var statementPreviewData = new StatementPreviewData();
                var SampleFiles = new List<FileData>();

                //start to render common html content data
                var htmlbody = new StringBuilder();
                htmlbody.Append(HtmlConstants.CONTAINER_DIV_HTML_HEADER);
                htmlbody.Append(HtmlConstants.NEDBANK_STATEMENT_HEADER.Replace("{{eConfirmLogo}}", "../common/images/eConfirm.png").Replace("{{NedBankLogo}}", "../common/images/NEDBANKLogo.png").Replace("{{StatementDate}}", DateTime.Now.ToString(ModelConstant.DATE_FORMAT_yyyy_MM_dd)));

                //this variable is used to bind all script to html statement, which helps to render data on chart and graph widgets
                var scriptHtmlRenderer = new StringBuilder();
                
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

                for (int i = 0; i < statement.Pages.Count; i++)
                {
                    var page = statement.Pages[i];
                    var MarketingMessageCounter = 0;
                    var statementPageContent = newStatementPageContents.Where(item => item.PageTypeId == page.PageTypeId && item.Id == i).FirstOrDefault();

                    var MarketingMessages = string.Empty;
                    if (page.PageTypeName == HtmlConstants.INVESTMENT_PAGE_TYPE)
                    {
                        MarketingMessages = HtmlConstants.INVESTMENT_MARKETING_MESSAGE_JSON_STR;
                    }
                    else if (page.PageTypeName == HtmlConstants.PERSONAL_LOAN_PAGE_TYPE)
                    {
                        MarketingMessages = HtmlConstants.PERSONAL_LOAN_MARKETING_MESSAGE_JSON_STR;
                    }
                    else if (page.PageTypeName == HtmlConstants.HOME_LOAN_PAGE_TYPE)
                    {
                        MarketingMessages = HtmlConstants.HOME_LOAN_MARKETING_MESSAGE_JSON_STR;
                    }

                    var pageContent = new StringBuilder(statementPageContent.HtmlContent);
                    var dynamicWidgets = statementPageContent.DynamicWidgets;

                    var PageHeaderContent = new StringBuilder(statementPageContent.PageHeaderContent);

                    var tabClassName = Regex.Replace((statementPageContent.DisplayName + "-" + page.Identifier), @"\s+", "-");
                    NavItemList.Append("<li class='nav-item" + (i != statement.Pages.Count - 1 ? " nav-rt-border" : string.Empty) + "'><a id='tab" + i + "-tab' data-toggle='tab' data-target='#" + tabClassName + "' role='tab' class='nav-link" + (i == 0 ? " active" : string.Empty) + "'> " + statementPageContent.DisplayName + " </a></li>");

                    string ExtraClassName = statement.Pages.Count > 1 ? (i == 0 ? " tab-pane fade in active show " : " tab-pane fade ") : string.Empty;
                    PageHeaderContent.Replace("{{ExtraClass}}", ExtraClassName).Replace("{{DivId}}", tabClassName);

                    var newPageContent = new StringBuilder();
                    var pagewidgets = new List<PageWidget>(page.PageWidgets);
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
                                        this.BindDummyDataToCustomerDetailsWidget(pageContent, page, widget);
                                    }
                                    break;

                                case HtmlConstants.BRANCH_DETAILS_WIDGET_NAME:
                                    this.BindDummyDataToBranchDetailsWidget(pageContent, page, widget, statement.Pages.Count);
                                    break;

                                case HtmlConstants.IMAGE_WIDGET_NAME:
                                    this.BindDummyDataToImageWidget(pageContent, statement, page, widget, SampleFiles, AppBaseDirectory, tenantCode);
                                    break;

                                case HtmlConstants.VIDEO_WIDGET_NAME:
                                    this.BindDummyDataToVideoWidget(pageContent, statement, page, widget, SampleFiles, AppBaseDirectory, tenantCode);
                                    break;
                                case HtmlConstants.STATIC_HTML_WIDGET_NAME:
                                    this.BindDummyDataToStaticHtmlWidget(pageContent, statement, page, widget);
                                    break;

                                case HtmlConstants.SEGMENT_BASED_CONTENT_WIDGET_HTML:
                                    this.BindDummyDataToSegmentBasedContentWidget(pageContent, statement, page, widget);
                                    break;

                                case HtmlConstants.INVESTMENT_PORTFOLIO_STATEMENT_WIDGET_NAME:
                                    this.BindDummyDataToInvestmentPortfolioStatementWidget(pageContent, page, widget);
                                    break;

                                case HtmlConstants.INVESTOR_PERFORMANCE_WIDGET_NAME:
                                    this.BindDummyDataToInvestorPerformanceWidget(pageContent, page, widget);
                                    break;

                                case HtmlConstants.BREAKDOWN_OF_INVESTMENT_ACCOUNTS_WIDGET_NAME:
                                    this.BindDummyDataToBreakdownOfInvestmentAccountsWidget(pageContent, page, widget);
                                    break;

                                case HtmlConstants.EXPLANATORY_NOTES_WIDGET_NAME:
                                    this.BindDummyDataToExplanatoryNotesWidget(pageContent, page, widget);
                                    break;

                                case HtmlConstants.SERVICE_WIDGET_NAME:
                                    this.BindDummyDataToMarketingServiceMessageWidget(pageContent, MarketingMessages, page, widget, MarketingMessageCounter);
                                    MarketingMessageCounter++;
                                    break;

                                case HtmlConstants.PERSONAL_LOAN_DETAIL_WIDGET_NAME:
                                    this.BindDummyDataToPersonalLoanDetailWidget(pageContent, page, widget);
                                    break;

                                case HtmlConstants.PERSONAL_LOAN_TRANASCTION_WIDGET_NAME:
                                    this.BindDummyDataToPersonalLoanTransactionWidget(pageContent, page, widget);
                                    break;

                                case HtmlConstants.PERSONAL_LOAN_PAYMENT_DUE_WIDGET_NAME:
                                    this.BindDummyDataToPersonalLoanPaymentDueWidget(pageContent, page, widget);
                                    break;

                                case HtmlConstants.SPECIAL_MESSAGE_WIDGET_NAME:
                                    this.BindDummyDataToSpecialMessageWidget(pageContent, page, widget);
                                    break;

                                case HtmlConstants.PERSONAL_LOAN_INSURANCE_MESSAGE_WIDGET_NAME:
                                    this.BindDummmyDataToPersonalLoanInsuranceMessageWidget(pageContent, page, widget);
                                    break;

                                case HtmlConstants.PERSONAL_LOAN_TOTAL_AMOUNT_DETAIL_WIDGET_NAME:
                                    this.BindDummyDataToPersonalLoanTotalAmountDetailWidget(pageContent, page, widget);
                                    break;

                                case HtmlConstants.PERSONAL_LOAN_ACCOUNTS_BREAKDOWN_WIDGET_NAME:
                                    this.BindDummyDataToPersonalLoanAccountsBreakdownWidget(pageContent, page, widget);
                                    break;

                                case HtmlConstants.HOME_LOAN_TOTAL_AMOUNT_DETAIL_WIDGET_NAME:
                                    this.BindDummyDataToHomeLoanTotalAmountDetailWidget(pageContent, page, widget);
                                    break;

                                case HtmlConstants.HOME_LOAN_ACCOUNTS_BREAKDOWN_WIDGET_NAME:
                                    this.BindDummyDataToHomeLoanAccountsBreakdownWidget(pageContent, page, widget);
                                    break;

                                case HtmlConstants.NEDBANK_PORTFOLIO_CUSTOMER_DETAILS_WIDGET_NAME:
                                    this.BindDummyDataToPortfolioCustomerDetailsWidget(pageContent, page, widget);
                                    break;

                                case HtmlConstants.NEDBANK_PORTFOLIO_CUSTOMER_ADDRESS_WIDGET_NAME:
                                    this.BindDummyDataToPortfolioCustomerAddressDetailsWidget(pageContent, page, widget);
                                    break;

                                case HtmlConstants.NEDBANK_PORTFOLIO_CLIENT_CONTACT_DETAILS_WIDGET_NAME:
                                    this.BindDummyDataToPortfolioClientContactDetailsWidget(pageContent, page, widget);
                                    break;

                                case HtmlConstants.NEDBANK_PORTFOLIO_ACCOUNT_SUMMARY_WIDGET_NAME:
                                    this.BindDummyDataToPortfolioAccountSummaryDetailsWidget(pageContent, page, widget);
                                    break;

                                case HtmlConstants.NEDBANK_PORTFOLIO_ACCOUNT_ANALYSIS_WIDGET_NAME:
                                    this.BindDummyDataToPortfolioAccountAnalysisGraphWidget(pageContent, scriptHtmlRenderer, page, widget);
                                    break;

                                case HtmlConstants.NEDBANK_PORTFOLIO_REMINDERS_WIDGET_NAME:
                                    this.BindDummyDataToPortfolioReimndersWidget(pageContent, page, widget);
                                    break;

                                case HtmlConstants.NEDBANK_PORTFOLIO_NEWS_ALERT_WIDGET_NAME:
                                    this.BindDummyDataToPortfolioNewsAlertsWidget(pageContent, page, widget);
                                    break;

                                case HtmlConstants.NEDBANK_GREENBACKS_TOTAL_REWARDS_POINTS_WIDGET_NAME:
                                    this.BindDummyDataToGreenbacksTotalRewardPointsWidget(pageContent, page, widget);
                                    break;

                                case HtmlConstants.NEDBANK_YTD_REWARDS_POINTS_BAR_GRAPH_WIDGET_NAME:
                                    this.BindDummyDataToGreenbacksYtdRewardsPointsGraphWidget(pageContent, scriptHtmlRenderer, page, widget);
                                    break;

                                case HtmlConstants.NEDBANK_POINTS_REDEEMED_YTD_BAR_GRAPH_WIDGET_NAME:
                                    this.BindDummyDataToGreenbacksPointsRedeemedYtdGraphWidget(pageContent, scriptHtmlRenderer, page, widget);
                                    break;

                                case HtmlConstants.NEDBANK_PRODUCT_RELATED_POINTS_EARNED_BAR_GRAPH_WIDGET_NAME:
                                    this.BindDummyDataToGreenbacksProductRelatedPonitsEarnedGraphWidget(pageContent, scriptHtmlRenderer, page, widget);
                                    break;

                                case HtmlConstants.NEDBANK_CATEGORY_SPEND_REWARDS_PIE_CHART_WIDGET_NAME:
                                    this.BindDummyDataToGreenbacksCategorySpendRewardPointsGraphWidget(pageContent, scriptHtmlRenderer, page, widget);
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
                footerContent.Replace("{{NedbankSloganImage}}", "../common/images/See_money_differently.PNG");
                footerContent.Replace("{{NedbankNameImage}}", "../common/images/NEDBANK_Name.png");
                footerContent.Replace("{{FooterText}}", HtmlConstants.NEDBANK_STATEMENT_FOOTER_TEXT_STRING);
                footerContent.Replace("{{LastFooterText}}", string.Empty);
                htmlbody.Append(footerContent.ToString());

                htmlbody.Append(HtmlConstants.CONTAINER_DIV_HTML_FOOTER); // end of container-fluid div

                StringBuilder finalHtml = new StringBuilder();
                finalHtml.Append(HtmlConstants.HTML_HEADER);
                finalHtml.Append(htmlbody.ToString());
                finalHtml.Append(HtmlConstants.HTML_FOOTER);

                //scriptHtmlRenderer.Append(HtmlConstants.TENANT_LOGO_SCRIPT);
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
                            htmlString.Append(pageHeaderHtml.Replace("{{DivId}}", tabClassName).Replace("{{ExtraClass}}", extraclass));

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
                                    "class='tab-pane fade in active show " + divClass + "' " + styleTag + ">");
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

                                                        string customerName = customerInfo.FirstName + " " + customerInfo.MiddleName + " " + customerInfo.LastName;
                                                        customerHtmlWidget = customerHtmlWidget.Replace("{{CustomerName}}", customerName);

                                                        string address1 = customerInfo.AddressLine1 + ", " + customerInfo.AddressLine2 + ", ";
                                                        customerHtmlWidget = customerHtmlWidget.Replace("{{Address1}}", address1);

                                                        string address2 = (customerInfo.City != "" ? customerInfo.City + ", " : "") +
                                                            (customerInfo.State != "" ? customerInfo.State + ", " : "") + (customerInfo.Country != "" ?
                                                            customerInfo.Country + ", " : "") + (customerInfo.Zip != "" ? customerInfo.Zip : "");
                                                        customerHtmlWidget = customerHtmlWidget.Replace("{{Address2}}", address2);

                                                        htmlString.Append(customerHtmlWidget);
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

                                                        AccDivData.Append("<div class='list-row-small ht70px'><div class='list-middle-row'> <div class='list-text'>RM Contact Number</div><label class='list-value mb-0'>" + accountInfo.RmContactNumber +"</label></div></div>");

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
                                                                accSummary.Append("<tr><td>" + acc.AccountType + "</td><td>" + acc.Currency + "</td><td>"+ acc.Amount + "</td></tr>");
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
                finalHtml = navbarHtml + tempHtml.ToString();

                return finalHtml;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method help to generate statement preview HTML string for financial tenants
        /// </summary>
        /// <param name="statementPages"> the list statement pages </param>
        /// <param name="tenantConfiguration"> the tenant configuration object </param>
        /// <param name="baseURL"> the base url </param>
        /// /// <param name="tenantCode"> the tenant code </param>
        /// <returns>return statement preview HTML string </returns>
        private string PreviewNedbankStatement(List<StatementPage> statementPages, TenantConfiguration tenantConfiguration, string baseURL, string tenantCode)
        {
            try
            {
                StringBuilder tempHtml = new StringBuilder();
                string finalHtml = string.Empty;

                tempHtml.Append("<div class='container-fluid mt-3 mb-3 bdy-scroll stylescrollbar'>");

                IList<string> linegraphIds = new List<string>();
                IList<string> bargraphIds = new List<string>();
                IList<string> piechartIds = new List<string>();

                var IsInvestmentStatement = false;
                var IsPersonalLoanStatement = false;
                var IsHomeLoanStatement = false;

                var htmlString = new StringBuilder();
                var NavItemList = new StringBuilder();

                for (int x = 0; x < statementPages.Count; x++)
                {
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
                    IsInvestmentStatement = pages.Where(it => it.PageTypeName == HtmlConstants.INVESTMENT_PAGE_TYPE).ToList().Count > 0;
                    IsPersonalLoanStatement = pages.Where(it => it.PageTypeName == HtmlConstants.PERSONAL_LOAN_PAGE_TYPE).ToList().Count > 0;
                    IsHomeLoanStatement = pages.Where(it => it.PageTypeName == HtmlConstants.HOME_LOAN_PAGE_TYPE).ToList().Count > 0;
                    var MarketingMessages = string.Empty;
                    
                    if (IsInvestmentStatement)
                    {
                        MarketingMessages = HtmlConstants.INVESTMENT_MARKETING_MESSAGE_JSON_STR;
                    }
                    else if (IsPersonalLoanStatement)
                    {
                        MarketingMessages = HtmlConstants.PERSONAL_LOAN_MARKETING_MESSAGE_JSON_STR;
                    }
                    else if (IsHomeLoanStatement)
                    {
                        MarketingMessages = HtmlConstants.HOME_LOAN_MARKETING_MESSAGE_JSON_STR;
                    }
                    else 
                    {
                        MarketingMessages = HtmlConstants.PERSONAL_LOAN_MARKETING_MESSAGE_JSON_STR;
                    }

                    if (pages.Count != 0)
                    {
                        for (int y = 0; y < pages.Count; y++)
                        {
                            var page = pages[y];
                            var MarketingMessageCounter = 0;
                            var tabClassName = Regex.Replace((page.DisplayName + " " + page.Version), @"\s+", "-");

                            NavItemList.Append("<li class='nav-item " + (x != statementPages.Count - 1 ? "nav-rt-border" : string.Empty) + " '><a id='tab" + x + "-tab' data-toggle='tab' data-target='#" + tabClassName + "' role='tab' class='nav-link " + (x == 0 ? "active" : string.Empty) + " '> " + page.DisplayName + " </a></li>");

                            //var extraclass = x > 0 ? "d-none " + tabClassName : tabClassName;
                            string extraclass = statementPages.Count > 1 ? (x == 0 ? " tab-pane fade in active show " : " tab-pane fade ") : string.Empty;
                            var pageHeaderHtml = "<div id='{{DivId}}' class='{{ExtraClass}}' {{BackgroundImage}}>";
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
                            
                            htmlString.Append(pageHeaderHtml.Replace("{{DivId}}", tabClassName).Replace("{{ExtraClass}}", extraclass));
                            
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

                                            var PaddingClass = i != 0 ? " pl-0" : string.Empty;
                                            if (MarketingMessages.Length > 0 && mergedlst[i].WidgetName == HtmlConstants.SERVICE_WIDGET_NAME)
                                            {
                                                //to add Nedbank services header... to do-- Create separate static widgets for widget's header label
                                                if (MarketingMessageCounter == 0)
                                                {
                                                    htmlString.Append("<div class='col-lg-12'><div class='card border-0'><div class='card-body text-left py-0'><div class='card-body-header pb-2'>Nedbank Services</div></div></div></div></div><div class='row'>");
                                                }
                                                PaddingClass = MarketingMessageCounter % 2 == 0 ? " pr-1 pl-35px" : " pl-1 pr-35px";
                                            }
                                            htmlString.Append("<div class='col-lg-" + divLength + PaddingClass + "'>");

                                            if (!mergedlst[i].IsDynamicWidget)
                                            {
                                                if (mergedlst[i].WidgetName == HtmlConstants.IMAGE_WIDGET_NAME)
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
                                                    //imgHtmlWidget.Replace("{{WidgetDivHeight}}", divHeight);
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
                                                    //vdoHtmlWidget = vdoHtmlWidget.Replace("{{WidgetDivHeight}}", divHeight);
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
                                                    var segmentBasedContentWidget = HtmlConstants.STATIC_HTML_WIDGET_HTML.Replace("{{SegmentBasedContent}}", html);
                                                    segmentBasedContentWidget = segmentBasedContentWidget.Replace("{{WidgetDivHeight}}", divHeight);
                                                    htmlString.Append(segmentBasedContentWidget);
                                                }
                                                else if (mergedlst[i].WidgetName == HtmlConstants.CUSTOMER_DETAILS_WIDGET_NAME)
                                                {
                                                    if (statementPages.Count == 1)
                                                    {
                                                        string jsonstr = "{'TITLE_TEXT': 'MR', 'FIRST_NAME_TEXT':'MATHYS','SURNAME_TEXT':'SMIT','ADDR_LINE_0':'VAN DER MEULENSTRAAT 39','ADDR_LINE_1':'3971 EB DRIEBERGEN','ADDR_LINE_2':'NEDERLAND','ADDR_LINE_3':'9999','ADDR_LINE_4':'', 'MASK_CELL_NO': '******7786', 'BARCODE': 'http://3.69.64.41:8020/API/Barcode.png'}";
                                                        if (jsonstr != string.Empty && validationEngine.IsValidJson(jsonstr))
                                                        {
                                                            var customerInfo = JsonConvert.DeserializeObject<CustomerInformation>(jsonstr);
                                                            var customerHtmlWidget = HtmlConstants.CUSTOMER_DETAILS_WIDGET_HTML;
                                                            customerHtmlWidget = customerHtmlWidget.Replace("{{Title}}", customerInfo.TITLE_TEXT);
                                                            customerHtmlWidget = customerHtmlWidget.Replace("{{FirstName}}", customerInfo.FIRST_NAME_TEXT);
                                                            customerHtmlWidget = customerHtmlWidget.Replace("{{SurName}}", customerInfo.SURNAME_TEXT);
                                                            customerHtmlWidget = customerHtmlWidget.Replace("{{CustAddressLine0}}", customerInfo.ADDR_LINE_0);
                                                            customerHtmlWidget = customerHtmlWidget.Replace("{{CustAddressLine1}}", customerInfo.ADDR_LINE_1);
                                                            customerHtmlWidget = customerHtmlWidget.Replace("{{CustAddressLine2}}", customerInfo.ADDR_LINE_2);
                                                            customerHtmlWidget = customerHtmlWidget.Replace("{{CustAddressLine3}}", customerInfo.ADDR_LINE_3);
                                                            customerHtmlWidget = customerHtmlWidget.Replace("{{CustAddressLine4}}", customerInfo.ADDR_LINE_4);
                                                            customerHtmlWidget = customerHtmlWidget.Replace("{{MaskCellNo}}", customerInfo.MASK_CELL_NO != string.Empty ? "Cell: " + customerInfo.MASK_CELL_NO : string.Empty);
                                                            //customerHtmlWidget = customerHtmlWidget.Replace("{{Barcode}}", customerInfo.Barcode != string.Empty ? customerInfo.Barcode : string.Empty);
                                                            htmlString.Append(customerHtmlWidget);
                                                        }
                                                    }
                                                }
                                                else if (mergedlst[i].WidgetName == HtmlConstants.BRANCH_DETAILS_WIDGET_NAME)
                                                {
                                                    var htmlWidget = new StringBuilder(HtmlConstants.BRANCH_DETAILS_WIDGET_HTML);
                                                    if (page.PageTypeName == HtmlConstants.HOME_LOAN_PAGE_TYPE)
                                                    {
                                                        if (statementPages.Count == 1)
                                                        {
                                                            htmlWidget.Replace("{{BankName}}", DateTime.Now.ToString(ModelConstant.DATE_FORMAT_yyyy_MM_dd));
                                                            htmlWidget.Replace("{{AddressLine0}}", string.Empty);
                                                            htmlWidget.Replace("{{AddressLine1}}", string.Empty);
                                                            htmlWidget.Replace("{{AddressLine2}}", string.Empty);
                                                            htmlWidget.Replace("{{AddressLine3}}", string.Empty);
                                                            htmlWidget.Replace("{{BankVATRegNo}}", string.Empty);
                                                            htmlWidget.Replace("{{ContactCenter}}", "Professional Banking 24/7 Contact centre: " + "0860 555 111");
                                                            htmlString.Append(htmlWidget.ToString());
                                                        }
                                                    }
                                                    else
                                                    {
                                                        string jsonstr = "{'BranchName': 'NEDBANK', 'AddressLine0':'Second Floor, Newtown Campus', 'AddressLine1':'141 Lilian Ngoyi Street, Newtown, Johannesburg 2001', 'AddressLine2':'PO Box 1144, Johannesburg, 2000','AddressLine3':'South Africa','VatRegNo':'4320116074','ContactNo':'0860 555 111'}";
                                                        if (jsonstr != string.Empty && validationEngine.IsValidJson(jsonstr))
                                                        {
                                                            var branchDetails = JsonConvert.DeserializeObject<DM_BranchMaster>(jsonstr);
                                                            htmlWidget.Replace("{{BankName}}", branchDetails.BranchName.ToUpper());
                                                            htmlWidget.Replace("{{AddressLine0}}", branchDetails.AddressLine0.ToUpper());
                                                            htmlWidget.Replace("{{AddressLine1}}", branchDetails.AddressLine1.ToUpper());
                                                            htmlWidget.Replace("{{AddressLine2}}", branchDetails.AddressLine2.ToUpper());
                                                            htmlWidget.Replace("{{AddressLine3}}", branchDetails.AddressLine3.ToUpper());
                                                            htmlWidget.Replace("{{BankVATRegNo}}", "Bank VAT Reg No " + branchDetails.VatRegNo);
                                                            htmlWidget.Replace("{{ContactCenter}}", "Nedbank Private Wealth Service Suite: " + branchDetails.ContactNo);
                                                            htmlString.Append(htmlWidget.ToString());
                                                        }
                                                    }
                                                }
                                                else if (mergedlst[i].WidgetName == HtmlConstants.INVESTMENT_PORTFOLIO_STATEMENT_WIDGET_NAME)
                                                {
                                                    string customerJsonstr = "{'TITLE_TEXT': 'MR', 'FIRST_NAME_TEXT':'MATHYS','SURNAME_TEXT':'SMIT','ADDR_LINE_0':'VAN DER MEULENSTRAAT 39','ADDR_LINE_1':'3971 EB DRIEBERGEN','ADDR_LINE_2':'NEDERLAND','ADDR_LINE_3':'9999','ADDR_LINE_4':'', 'MASK_CELL_NO': '******7786', 'FIRSTNAME': 'MATHYS', 'LASTNAME': 'SMIT'}";
                                                    string jsonstr = "{'Currency': 'R', 'TotalClosingBalance': '23 920.98', 'DayOfStatement':'21', 'InvestorId':'204626','StatementPeriod':'22/12/2020 to 21/01/2021','StatementDate':'21/01/2021', 'DsInvestorName' : ''}";
                                                    if (jsonstr != string.Empty && validationEngine.IsValidJson(jsonstr))
                                                    {
                                                        var customerInfo = JsonConvert.DeserializeObject<CustomerInformation>(customerJsonstr);
                                                        dynamic InvestmentPortfolio = JObject.Parse(jsonstr);
                                                        var InvestmentPortfolioHtmlWidget = HtmlConstants.INVESTMENT_PORTFOLIO_STATEMENT_WIDGET_HTML;
                                                        InvestmentPortfolioHtmlWidget = InvestmentPortfolioHtmlWidget.Replace("{{DSName}}", Convert.ToString(InvestmentPortfolio.DsInvestorName));
                                                        InvestmentPortfolioHtmlWidget = InvestmentPortfolioHtmlWidget.Replace("{{TotalClosingBalance}}", (Convert.ToString(InvestmentPortfolio.Currency) + Convert.ToString(InvestmentPortfolio.TotalClosingBalance)));
                                                        InvestmentPortfolioHtmlWidget = InvestmentPortfolioHtmlWidget.Replace("{{DayOfStatement}}", Convert.ToString(InvestmentPortfolio.DayOfStatement));
                                                        InvestmentPortfolioHtmlWidget = InvestmentPortfolioHtmlWidget.Replace("{{InvestorID}}", Convert.ToString(InvestmentPortfolio.InvestorId));
                                                        InvestmentPortfolioHtmlWidget = InvestmentPortfolioHtmlWidget.Replace("{{StatementPeriod}}", Convert.ToString(InvestmentPortfolio.StatementPeriod));
                                                        InvestmentPortfolioHtmlWidget = InvestmentPortfolioHtmlWidget.Replace("{{StatementDate}}", Convert.ToString(InvestmentPortfolio.StatementDate));
                                                        InvestmentPortfolioHtmlWidget = InvestmentPortfolioHtmlWidget.Replace("{{FirstName}}", Convert.ToString(customerInfo.FirstName));
                                                        InvestmentPortfolioHtmlWidget = InvestmentPortfolioHtmlWidget.Replace("{{SurName}}", Convert.ToString(customerInfo.LastName));

                                                        htmlString.Append(InvestmentPortfolioHtmlWidget);
                                                    }
                                                }
                                                else if (mergedlst[i].WidgetName == HtmlConstants.INVESTOR_PERFORMANCE_WIDGET_NAME)
                                                {
                                                    string jsonstr = "{'Currency': 'R', 'ProductType': 'Notice deposits', 'OpeningBalanceAmount':'23 875.36', 'ClosingBalanceAmount':'23 920.98'}";
                                                    if (jsonstr != string.Empty && validationEngine.IsValidJson(jsonstr))
                                                    {
                                                        dynamic InvestmentPerformance = JObject.Parse(jsonstr);
                                                        var InvestorPerformanceHtmlWidget = HtmlConstants.INVESTOR_PERFORMANCE_WIDGET_HTML;
                                                        InvestorPerformanceHtmlWidget = InvestorPerformanceHtmlWidget.Replace("{{ProductType}}", Convert.ToString(InvestmentPerformance.ProductType));
                                                        InvestorPerformanceHtmlWidget = InvestorPerformanceHtmlWidget.Replace("{{OpeningBalanceAmount}}", (Convert.ToString(InvestmentPerformance.Currency) + Convert.ToString(InvestmentPerformance.OpeningBalanceAmount)));
                                                        InvestorPerformanceHtmlWidget = InvestorPerformanceHtmlWidget.Replace("{{ClosingBalanceAmount}}", (Convert.ToString(InvestmentPerformance.Currency) + Convert.ToString(InvestmentPerformance.ClosingBalanceAmount)));
                                                        htmlString.Append(InvestorPerformanceHtmlWidget);
                                                    }
                                                }
                                                else if (mergedlst[i].WidgetName == HtmlConstants.BREAKDOWN_OF_INVESTMENT_ACCOUNTS_WIDGET_NAME)
                                                {
                                                    string jsonstr = HtmlConstants.BREAKDOWN_OF_INVESTMENT_ACCOUNTS_WIDGET_PREVIEW_JSON_STRING;

                                                    if (jsonstr != string.Empty && validationEngine.IsValidJson(jsonstr))
                                                    {
                                                        var InvestmentAccountBreakdownHtml = new StringBuilder(HtmlConstants.BREAKDOWN_OF_INVESTMENT_ACCOUNTS_WIDGET_HTML);
                                                        IList<InvestmentAccount> InvestmentAccounts = JsonConvert.DeserializeObject<List<InvestmentAccount>>(jsonstr);

                                                        //Create Nav tab if investment accounts is more than 1
                                                        var NavTabs = new StringBuilder();
                                                        var InvestmentAccountsCount = InvestmentAccounts.Count;
                                                        InvestmentAccountBreakdownHtml.Replace("{{NavTab}}", NavTabs.ToString());

                                                        //create tab-content div if accounts is greater than 1, otherwise create simple div
                                                        var TabContentHtml = new StringBuilder();
                                                        var counter = 0;
                                                        TabContentHtml.Append((InvestmentAccountsCount > 1) ? "<div class='tab-content'>" : "");
                                                        InvestmentAccounts.ToList().ForEach(acc =>
                                                        {
                                                            var InvestmentAccountDetailHtml = new StringBuilder(HtmlConstants.INVESTMENT_ACCOUNT_DETAILS_HTML);

                                                            InvestmentAccountDetailHtml.Replace("{{ProductDesc}}", acc.ProductDesc);
                                                            InvestmentAccountDetailHtml.Replace("{{InvestmentId}}", acc.InvestmentId);
                                                            InvestmentAccountDetailHtml.Replace("{{TabPaneClass}}", "tab-pane fade " + (counter == 0 ? "in active show" : ""));

                                                            var InvestmentNo = acc.InvestorId + " " + acc.InvestmentId;
                                                            //actual length is 12, due to space in between investor id and investment id we comparing for 13 characters
                                                            while (InvestmentNo.Length != 13)
                                                            {
                                                                InvestmentNo = "0" + InvestmentNo;
                                                            }
                                                            InvestmentAccountDetailHtml.Replace("{{InvestmentNo}}", InvestmentNo);
                                                            InvestmentAccountDetailHtml.Replace("{{AccountOpenDate}}", acc.OpenDate);

                                                            InvestmentAccountDetailHtml.Replace("{{AccountOpenDate}}", acc.OpenDate);
                                                            InvestmentAccountDetailHtml.Replace("{{InterestRate}}", acc.CurrentInterestRate + "% pa");
                                                            InvestmentAccountDetailHtml.Replace("{{MaturityDate}}", acc.ExpiryDate);
                                                            InvestmentAccountDetailHtml.Replace("{{InterestDisposal}}", acc.InterestDisposalDesc);
                                                            InvestmentAccountDetailHtml.Replace("{{NoticePeriod}}", acc.NoticePeriod);
                                                            InvestmentAccountDetailHtml.Replace("{{InterestDue}}", acc.Currency + acc.AccuredInterest);

                                                            InvestmentAccountDetailHtml.Replace("{{LastTransactionDate}}", "25 November 2020");
                                                            InvestmentAccountDetailHtml.Replace("{{BalanceOfLastTransactionDate}}", acc.Currency + (counter == 0 ? "5 307.14" : "18 613.84"));

                                                            var InvestmentTransactionRows = new StringBuilder();
                                                            acc.Transactions.ForEach(trans =>
                                                            {
                                                                var tr = new StringBuilder();
                                                                tr.Append("<tr class='ht-20'>");
                                                                tr.Append("<td class='w-15 pt-1'>" + trans.TransactionDate + "</td>");
                                                                tr.Append("<td class='w-40 pt-1'>" + trans.TransactionDesc + "</td>");
                                                                tr.Append("<td class='w-15 text-right pt-1'>" + (trans.Debit == "0" ? "-" : acc.Currency + trans.Debit) + "</td>");
                                                                tr.Append("<td class='w-15 text-right pt-1'>" + (trans.Credit == "0" ? "-" : acc.Currency + trans.Credit) + "</td>");
                                                                tr.Append("<td class='w-15 text-right pt-1'>" + (trans.Balance == "0" ? "-" : acc.Currency + trans.Balance) + "</td>");
                                                                tr.Append("</tr>");
                                                                InvestmentTransactionRows.Append(tr.ToString());
                                                            });
                                                            InvestmentAccountDetailHtml.Replace("{{InvestmentTransactionRows}}", InvestmentTransactionRows.ToString());
                                                            TabContentHtml.Append(InvestmentAccountDetailHtml.ToString());
                                                            counter++;
                                                        });
                                                        TabContentHtml.Append((InvestmentAccountsCount > 1) ? "</div>" : "");

                                                        InvestmentAccountBreakdownHtml.Replace("{{TabContentsDiv}}", TabContentHtml.ToString());
                                                        htmlString.Append(InvestmentAccountBreakdownHtml.ToString());
                                                    }
                                                }
                                                else if (mergedlst[i].WidgetName == HtmlConstants.EXPLANATORY_NOTES_WIDGET_NAME)
                                                {
                                                    string jsonstr = "{'Note1': 'Fixed deposits — Total balance of all your fixed-type accounts.', 'Note2': 'Notice deposits — Total balance of all your notice deposit accounts.', 'Note3':'Linked deposits — Total balance of all your linked-type accounts.'}";
                                                    if (jsonstr != string.Empty && validationEngine.IsValidJson(jsonstr))
                                                    {
                                                        dynamic noteObj = JObject.Parse(jsonstr);
                                                        var NotesHtmlWidget = HtmlConstants.EXPLANATORY_NOTES_WIDGET_HTML;
                                                        var notes = new StringBuilder();
                                                        notes.Append("<span> " + Convert.ToString(noteObj.Note1) + " </span> <br/>");
                                                        notes.Append("<span> " + Convert.ToString(noteObj.Note2) + " </span> <br/>");
                                                        notes.Append("<span> " + Convert.ToString(noteObj.Note3) + " </span> ");
                                                        NotesHtmlWidget = NotesHtmlWidget.Replace("{{Notes}}", notes.ToString());
                                                        htmlString.Append(NotesHtmlWidget);
                                                    }
                                                }
                                                else if (mergedlst[i].WidgetName == HtmlConstants.SERVICE_WIDGET_NAME)
                                                {
                                                    if (MarketingMessages != string.Empty && validationEngine.IsValidJson(MarketingMessages))
                                                    {
                                                        IList<MarketingMessage> _lstMarketingMessage = JsonConvert.DeserializeObject<List<MarketingMessage>>(MarketingMessages);
                                                        var ServiceMessage = _lstMarketingMessage[MarketingMessageCounter];
                                                        if (ServiceMessage != null)
                                                        {
                                                            var messageTxt = ((!string.IsNullOrEmpty(ServiceMessage.MarketingMessageText1)) ? "<p>" + ServiceMessage.MarketingMessageText1 + "</p>" : string.Empty) + ((!string.IsNullOrEmpty(ServiceMessage.MarketingMessageText2)) ? "<p>" + ServiceMessage.MarketingMessageText2 + "</p>" : string.Empty) + ((!string.IsNullOrEmpty(ServiceMessage.MarketingMessageText3)) ? "<p>" + ServiceMessage.MarketingMessageText3 + "</p>" : string.Empty) + ((!string.IsNullOrEmpty(ServiceMessage.MarketingMessageText4)) ? "<p>" + ServiceMessage.MarketingMessageText4 + "</p>" : string.Empty) + ((!string.IsNullOrEmpty(ServiceMessage.MarketingMessageText5)) ? "<p>" + ServiceMessage.MarketingMessageText5 + "</p>" : string.Empty);

                                                            var htmlWidget = HtmlConstants.SERVICE_WIDGET_HTML;
                                                            htmlWidget = htmlWidget.Replace("{{ServiceMessageHeader}}", ServiceMessage.MarketingMessageHeader);
                                                            htmlWidget = htmlWidget.Replace("{{ServiceMessageText}}", messageTxt);
                                                            htmlString.Append(htmlWidget);
                                                        }
                                                    }
                                                    MarketingMessageCounter++;
                                                }
                                                else if (mergedlst[i].WidgetName == HtmlConstants.PERSONAL_LOAN_DETAIL_WIDGET_NAME)
                                                {
                                                    string jsonstr = "{'Identifier': 1,'CustomerId': 211135146504,'InvestorId': 8004334234001,'Currency': 'R','ProductType': 'PersonalLoan','BranchId': 1,'CreditAdvance': '71258','OutstandingBalance': '68169','AmountDue': '3297','ToDate': '2021-02-28 00:00:00','FromDate': '2020-12-01 00:00:00','MonthlyInstallment': '3297','DueDate': '2021-03-31 00:00:00','Arrears': '0','AnnualRate': '24','Term': '36'}";
                                                    if (jsonstr != string.Empty && validationEngine.IsValidJson(jsonstr))
                                                    {
                                                        var PersonalLoan = JsonConvert.DeserializeObject<DM_PersonalLoanMaster>(jsonstr);
                                                        var widgetHtml = new StringBuilder(HtmlConstants.PERSONAL_LOAN_DETAIL_HTML);

                                                        var res = 0.0m;
                                                        if (decimal.TryParse(PersonalLoan.CreditAdvance, out res))
                                                        {
                                                            widgetHtml.Replace("{{TotalLoanAmount}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                                                        }
                                                        else
                                                        {
                                                            widgetHtml.Replace("{{TotalLoanAmount}}", "");
                                                        }

                                                        res = 0.0m;
                                                        if (decimal.TryParse(PersonalLoan.OutstandingBalance, out res))
                                                        {
                                                            widgetHtml.Replace("{{OutstandingBalance}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                                                        }
                                                        else
                                                        {
                                                            widgetHtml.Replace("{{OutstandingBalance}}", "");
                                                        }

                                                        res = 0.0m;
                                                        if (decimal.TryParse(PersonalLoan.AmountDue, out res))
                                                        {
                                                            widgetHtml.Replace("{{DueAmount}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                                                        }
                                                        else
                                                        {
                                                            widgetHtml.Replace("{{DueAmount}}", "");
                                                        }

                                                        widgetHtml.Replace("{{AccountNumber}}", PersonalLoan.InvestorId.ToString());
                                                        widgetHtml.Replace("{{StatementDate}}", PersonalLoan.ToDate.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy));
                                                        widgetHtml.Replace("{{StatementPeriod}}", PersonalLoan.FromDate.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy) + " - " + PersonalLoan.ToDate.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy));

                                                        res = 0.0m;
                                                        if (decimal.TryParse(PersonalLoan.Arrears, out res))
                                                        {
                                                            widgetHtml.Replace("{{ArrearsAmount}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                                                        }
                                                        else
                                                        {
                                                            widgetHtml.Replace("{{ArrearsAmount}}", "");
                                                        }

                                                        widgetHtml.Replace("{{AnnualRate}}", PersonalLoan.AnnualRate + "% pa");

                                                        res = 0.0m;
                                                        if (decimal.TryParse(PersonalLoan.MonthlyInstallment, out res))
                                                        {
                                                            widgetHtml.Replace("{{MonthlyInstallment}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                                                        }
                                                        else
                                                        {
                                                            widgetHtml.Replace("{{MonthlyInstallment}}", "");
                                                        }

                                                        widgetHtml.Replace("{{Terms}}", PersonalLoan.Term);
                                                        widgetHtml.Replace("{{DueByDate}}", PersonalLoan.DueDate.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy));
                                                        htmlString.Append(widgetHtml.ToString());
                                                    }
                                                }
                                                else if (mergedlst[i].WidgetName == HtmlConstants.PERSONAL_LOAN_TRANASCTION_WIDGET_NAME)
                                                {
                                                    string jsonstr = HtmlConstants.PERSONAL_LOAN_TRANSACTION_PREVIEW_JSON_STRING;
                                                    if (jsonstr != string.Empty && validationEngine.IsValidJson(jsonstr))
                                                    {
                                                        var transactions = JsonConvert.DeserializeObject<List<DM_PersonalLoanTransaction>>(jsonstr);
                                                        if (transactions != null && transactions.Count > 0)
                                                        {
                                                            var widgetHtml = new StringBuilder(HtmlConstants.PERSONAL_LOAN_TRANSACTION_HTML);
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
                                                            widgetHtml.Replace("{{PersonalLoanTransactionRow}}", LoanTransactionRows.ToString());
                                                            htmlString.Append(widgetHtml.ToString());
                                                        }
                                                    }
                                                }
                                                else if (mergedlst[i].WidgetName == HtmlConstants.PERSONAL_LOAN_PAYMENT_DUE_WIDGET_NAME)
                                                {
                                                    string jsonstr = "{'Identifier': '1','CustomerId': '211135146504','InvestorId': '8004334234001','Arrears_120': '0','Arrears_90': '0','Arrears_60': '0','Arrears_30': '0','Arrears_0': '3297'}";
                                                    if (jsonstr != string.Empty && validationEngine.IsValidJson(jsonstr))
                                                    {
                                                        var plArrears = JsonConvert.DeserializeObject<DM_PersonalLoanArrears>(jsonstr);
                                                        if (plArrears != null)
                                                        {
                                                            var widgetHtml = new StringBuilder(HtmlConstants.PERSONAL_LOAN_PAYMENT_DUE_HTML);
                                                            var res = 0.0m;
                                                            if (decimal.TryParse(plArrears.Arrears_120, out res))
                                                            {
                                                                widgetHtml.Replace("{{After120Days}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                                                            }
                                                            else
                                                            {
                                                                widgetHtml.Replace("{{After120Days}}", "R0.00");
                                                            }

                                                            res = 0.0m;
                                                            if (decimal.TryParse(plArrears.Arrears_90, out res))
                                                            {
                                                                widgetHtml.Replace("{{After90Days}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                                                            }
                                                            else
                                                            {
                                                                widgetHtml.Replace("{{After90Days}}", "R0.00");
                                                            }

                                                            res = 0.0m;
                                                            if (decimal.TryParse(plArrears.Arrears_60, out res))
                                                            {
                                                                widgetHtml.Replace("{{After60Days}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                                                            }
                                                            else
                                                            {
                                                                widgetHtml.Replace("{{After60Days}}", "R0.00");
                                                            }

                                                            res = 0.0m;
                                                            if (decimal.TryParse(plArrears.Arrears_30, out res))
                                                            {
                                                                widgetHtml.Replace("{{After30Days}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                                                            }
                                                            else
                                                            {
                                                                widgetHtml.Replace("{{After30Days}}", "R0.00");
                                                            }

                                                            res = 0.0m;
                                                            if (decimal.TryParse(plArrears.Arrears_0, out res))
                                                            {
                                                                widgetHtml.Replace("{{Current}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                                                            }
                                                            else
                                                            {
                                                                widgetHtml.Replace("{{Current}}", "R0.00");
                                                            }

                                                            htmlString.Append(widgetHtml.ToString());
                                                        }
                                                    }
                                                }
                                                else if (mergedlst[i].WidgetName == HtmlConstants.SPECIAL_MESSAGE_WIDGET_NAME)
                                                {
                                                    var widgetHtml = new StringBuilder(HtmlConstants.SPECIAL_MESSAGE_HTML);
                                                    if (page.PageTypeName == HtmlConstants.HOME_LOAN_PAGE_TYPE)
                                                    {
                                                        string jsonstr = HtmlConstants.HOME_LOAN_SPECIAL_MESSAGES_WIDGET_PREVIEW_JSON_STRING;
                                                        if (jsonstr != string.Empty && validationEngine.IsValidJson(jsonstr))
                                                        {
                                                            var SpecialMessage = JsonConvert.DeserializeObject<SpecialMessage>(jsonstr);
                                                            if (SpecialMessage != null)
                                                            {
                                                                var specialMsgTxtData = (!string.IsNullOrEmpty(SpecialMessage.Header) ? "<div class='SpecialMessageHeader'> " + SpecialMessage.Header + " </div>" : string.Empty) + (!string.IsNullOrEmpty(SpecialMessage.Message1) ? "<p> " + SpecialMessage.Message1 + " </p>" : string.Empty) + (!string.IsNullOrEmpty(SpecialMessage.Message2) ? "<p> " + SpecialMessage.Message2 + " </p>" : string.Empty);

                                                                widgetHtml.Replace("{{SpecialMessageTextData}}", specialMsgTxtData);
                                                                htmlString.Append(widgetHtml.ToString());
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        string jsonstr = HtmlConstants.SPECIAL_MESSAGES_WIDGET_PREVIEW_JSON_STRING;
                                                        if (jsonstr != string.Empty && validationEngine.IsValidJson(jsonstr))
                                                        {
                                                            var SpecialMessage = JsonConvert.DeserializeObject<SpecialMessage>(jsonstr);
                                                            if (SpecialMessage != null)
                                                            {
                                                                var specialMsgTxtData = (!string.IsNullOrEmpty(SpecialMessage.Header) ? "<div class='SpecialMessageHeader'> " + SpecialMessage.Header + " </div>" : string.Empty) + (!string.IsNullOrEmpty(SpecialMessage.Message1) ? "<p> " + SpecialMessage.Message1 + " </p>" : string.Empty) + (!string.IsNullOrEmpty(SpecialMessage.Message2) ? "<p> " + SpecialMessage.Message2 + " </p>" : string.Empty);

                                                                widgetHtml.Replace("{{SpecialMessageTextData}}", specialMsgTxtData);
                                                                htmlString.Append(widgetHtml.ToString());
                                                            }
                                                        }
                                                    }
                                                }
                                                else if (mergedlst[i].WidgetName == HtmlConstants.PERSONAL_LOAN_INSURANCE_MESSAGE_WIDGET_NAME)
                                                {
                                                    string jsonstr = HtmlConstants.SPECIAL_MESSAGES_WIDGET_PREVIEW_JSON_STRING;
                                                    if (jsonstr != string.Empty && validationEngine.IsValidJson(jsonstr))
                                                    {
                                                        var InsuranceMsg = JsonConvert.DeserializeObject<SpecialMessage>(jsonstr);
                                                        if (InsuranceMsg != null)
                                                        {
                                                            var widgetHtml = new StringBuilder(HtmlConstants.PERSONAL_LOAN_INSURANCE_MESSAGE_HTML);
                                                            var InsuranceMsgTxtData = (!string.IsNullOrEmpty(InsuranceMsg.Message3) ? "<p> " + InsuranceMsg.Message3 + " </p>" : string.Empty) +
                                                                (!string.IsNullOrEmpty(InsuranceMsg.Message4) ? "<p> " + InsuranceMsg.Message4 + " </p>" : string.Empty) +
                                                                (!string.IsNullOrEmpty(InsuranceMsg.Message5) ? "<p> " + InsuranceMsg.Message5 + " </p>" : string.Empty);

                                                            widgetHtml.Replace("{{InsuranceMessages}}", InsuranceMsgTxtData);
                                                            htmlString.Append(widgetHtml.ToString());
                                                        }
                                                    }
                                                }
                                                else if (mergedlst[i].WidgetName == HtmlConstants.PERSONAL_LOAN_TOTAL_AMOUNT_DETAIL_WIDGET_NAME)
                                                {
                                                    string jsonstr = HtmlConstants.PERSONAL_LOAN_ACCOUNTS_PREVIEW_JSON_STRING;
                                                    var widgetHtml = new StringBuilder(HtmlConstants.PERSONAL_LOAN_TOTAL_AMOUNT_DETAIL_WIDGET_HTML);
                                                    var TotalLoanAmt = 0.0m;
                                                    var TotalOutstandingAmt = 0.0m;
                                                    var TotalLoanDueAmt = 0.0m;

                                                    if (jsonstr != string.Empty && validationEngine.IsValidJson(jsonstr))
                                                    {
                                                        var PersonalLoans = JsonConvert.DeserializeObject<List<DM_PersonalLoanMaster>>(jsonstr);
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
                                                    }

                                                    widgetHtml.Replace("{{TotalLoanAmount}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, TotalLoanAmt));
                                                    widgetHtml.Replace("{{OutstandingBalance}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, TotalOutstandingAmt));
                                                    widgetHtml.Replace("{{DueAmount}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, TotalLoanDueAmt));
                                                    htmlString.Append(widgetHtml.ToString());
                                                }
                                                else if (mergedlst[i].WidgetName == HtmlConstants.PERSONAL_LOAN_ACCOUNTS_BREAKDOWN_WIDGET_NAME)
                                                {
                                                    string jsonstr = HtmlConstants.PERSONAL_LOAN_ACCOUNTS_PREVIEW_JSON_STRING;
                                                    var widgetHtml = new StringBuilder(HtmlConstants.PERSONAL_LOAN_ACCOUNTS_BREAKDOWN_WIDGET_HTML);
                                                    if (jsonstr != string.Empty && validationEngine.IsValidJson(jsonstr))
                                                    {
                                                        var PersonalLoans = JsonConvert.DeserializeObject<List<DM_PersonalLoanMaster>>(jsonstr);
                                                        if (PersonalLoans != null && PersonalLoans.Count > 0)
                                                        {
                                                            //Create Nav tab if customer has more than 1 personal loan accounts
                                                            var NavTabs = new StringBuilder();
                                                            if (PersonalLoans.Count > 1)
                                                            {
                                                                NavTabs.Append("<ul class='nav nav-tabs Personalloan-nav-tabs'>");
                                                                var cnt = 0;
                                                                PersonalLoans.ToList().ForEach(acc =>
                                                                {
                                                                    var AccountNumber = acc.InvestorId.ToString();
                                                                    string lastFourDigisOfAccountNumber = AccountNumber.Length > 4 ? AccountNumber.Substring(Math.Max(0, AccountNumber.Length - 4)) : AccountNumber;
                                                                    NavTabs.Append("<li class='nav-item " + (cnt == 0 ? "active" : string.Empty) + "'><a id='tab0-tab' data-toggle='tab' data-target='#PersonalLoan-" + lastFourDigisOfAccountNumber + "' role='tab' class='nav-link " + (cnt == 0 ? "active" : string.Empty) + "'> Personal Loan - " + lastFourDigisOfAccountNumber + "</a></li>");
                                                                    cnt++;
                                                                });
                                                                NavTabs.Append("</ul>");
                                                            }
                                                            widgetHtml.Replace("{{NavTab}}", NavTabs.ToString());

                                                            //create tab-content div if accounts is greater than 1, otherwise create simple div
                                                            var TabContentHtml = new StringBuilder();
                                                            var counter = 0;
                                                            TabContentHtml.Append((PersonalLoans.Count > 1) ? "<div class='tab-content'>" : string.Empty);
                                                            PersonalLoans.ForEach(PersonalLoan =>
                                                            {
                                                                string lastFourDigisOfAccountNumber = PersonalLoan.InvestorId.ToString().Length > 4 ? PersonalLoan.InvestorId.ToString().Substring(Math.Max(0, PersonalLoan.InvestorId.ToString().Length - 4)) : PersonalLoan.InvestorId.ToString();

                                                                TabContentHtml.Append("<div id='PersonalLoan-" + lastFourDigisOfAccountNumber + "' class='tab-pane fade " + (counter == 0 ? "in active show" : string.Empty) + "'>");

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
                                                                    var LoanTransactionDetailHtml = new StringBuilder(HtmlConstants.PERSONAL_LOAN_ACCOUNT_TRANSACTION_DETAIL);
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
                                                                TabContentHtml.Append(HtmlConstants.END_DIV_TAG);
                                                                counter++;
                                                            });

                                                            TabContentHtml.Append((PersonalLoans.Count > 1) ? HtmlConstants.END_DIV_TAG : string.Empty);
                                                            widgetHtml.Replace("{{TabContentsDiv}}", TabContentHtml.ToString());
                                                            htmlString.Append(widgetHtml.ToString());
                                                        }
                                                    }
                                                }
                                                else if (mergedlst[i].WidgetName == HtmlConstants.HOME_LOAN_TOTAL_AMOUNT_DETAIL_WIDGET_NAME)
                                                {
                                                    string jsonstr = HtmlConstants.HOME_LOAN_ACCOUNTS_PREVIEW_JSON_STRING;
                                                    var widgetHtml = new StringBuilder(HtmlConstants.HOME_LOAN_TOTAL_AMOUNT_DETAIL_WIDGET_HTML);
                                                    var TotalLoanAmt = 0.0m;
                                                    var TotalOutstandingAmt = 0.0m;

                                                    if (jsonstr != string.Empty && validationEngine.IsValidJson(jsonstr))
                                                    {
                                                        var HomeLoans = JsonConvert.DeserializeObject<List<DM_HomeLoanMaster>>(jsonstr);
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

                                                        widgetHtml.Replace("{{TotalHomeLoansAmount}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, TotalLoanAmt));
                                                        widgetHtml.Replace("{{TotalHomeLoansBalanceOutstanding}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, TotalOutstandingAmt));
                                                        htmlString.Append(widgetHtml.ToString());
                                                    }
                                                }
                                                else if (mergedlst[i].WidgetName == HtmlConstants.HOME_LOAN_ACCOUNTS_BREAKDOWN_WIDGET_NAME)
                                                {
                                                    string jsonstr = HtmlConstants.HOME_LOAN_ACCOUNTS_PREVIEW_JSON_STRING;
                                                    var widgetHtml = new StringBuilder(HtmlConstants.HOME_LOAN_ACCOUNTS_BREAKDOWN_HTML);
                                                    if (jsonstr != string.Empty && validationEngine.IsValidJson(jsonstr))
                                                    {
                                                        var HomeLoans = JsonConvert.DeserializeObject<List<DM_HomeLoanMaster>>(jsonstr);
                                                        if (HomeLoans != null && HomeLoans.Count > 0)
                                                        {
                                                            //Create Nav tab if customer has more than 1 personal loan accounts
                                                            var NavTabs = new StringBuilder();
                                                            if (HomeLoans.Count > 1)
                                                            {
                                                                NavTabs.Append("<ul class='nav nav-tabs Homeloan-nav-tabs'>");
                                                                var cnt = 0;
                                                                HomeLoans.ToList().ForEach(acc =>
                                                                {
                                                                    var accNo = acc.InvestorId.ToString();
                                                                    string lastFourDigisOfAccountNumber = accNo.Length > 4 ? accNo.Substring(Math.Max(0, accNo.Length - 4)) : accNo;
                                                                    NavTabs.Append("<li class='nav-item " + (cnt == 0 ? "active" : string.Empty) + "'><a id='tab0-tab' data-toggle='tab' data-target='#HomeLoan-" + lastFourDigisOfAccountNumber + "' role='tab' class='nav-link " + (cnt == 0 ? "active" : string.Empty) + "'> Home Loan - " + lastFourDigisOfAccountNumber + "</a></li>");
                                                                    cnt++;
                                                                });
                                                                NavTabs.Append("</ul>");
                                                            }
                                                            widgetHtml.Replace("{{NavTab}}", NavTabs.ToString());

                                                            //create tab-content div if accounts is greater than 1, otherwise create simple div
                                                            var TabContentHtml = new StringBuilder();
                                                            var counter = 0;
                                                            TabContentHtml.Append((HomeLoans.Count > 1) ? "<div class='tab-content'>" : string.Empty);
                                                            HomeLoans.ForEach(HomeLoan =>
                                                            {
                                                                var accNo = HomeLoan.InvestorId.ToString();
                                                                string lastFourDigisOfAccountNumber = accNo.Length > 4 ? accNo.Substring(Math.Max(0, accNo.Length - 4)) : accNo;

                                                                TabContentHtml.Append("<div id='HomeLoan-" + lastFourDigisOfAccountNumber + "' class='tab-pane fade " + (counter == 0 ? "in active show" : string.Empty) + "'>");

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
                                                                var LoanTransactionDetailHtml = new StringBuilder(HtmlConstants.HOME_LOAN_TRANSACTION_DETAIL_DIV_HTML);

                                                                var tr = new StringBuilder();
                                                                if (HomeLoan.LoanTransactions != null && HomeLoan.LoanTransactions.Count > 0)
                                                                {
                                                                    HomeLoan.LoanTransactions.ForEach(trans =>
                                                                    {
                                                                        tr = new StringBuilder();
                                                                        tr.Append("<tr class='ht-20'>");
                                                                        tr.Append("<td class='w-13 text-center'> " + trans.Posting_date.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy) + " </td>");
                                                                        tr.Append("<td class='w-15 text-center'> " + trans.Effective_date.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy) + " </td>");
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

                                                                var PaymentDueMessageDivHtml = new StringBuilder(HtmlConstants.HOME_LAON_PAYMENT_DUE_SPECIAL_MESSAGE_DIV_HTML);
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

                                                                var LoanSummaryForTaxPurposesHtml = new StringBuilder(HtmlConstants.HOME_LOAN_SERVICE_FOR_TAX_PURPOSES_DIV_HTML);
                                                                var LoanInstalmentHtml = new StringBuilder(HtmlConstants.HOME_LOAN_INSTALMENT_DETAILS_DIV_HTML);
                                                                var HomeLoanSummary = HomeLoans[0].LoanSummary;
                                                                if (HomeLoanSummary != null)
                                                                {
                                                                    #region Summary for Tax purposes div
                                                                    res = 0.0m;
                                                                    if (!string.IsNullOrEmpty(HomeLoanSummary.Annual_Interest) && decimal.TryParse(HomeLoanSummary.Annual_Interest, out res))
                                                                    {
                                                                        LoanSummaryForTaxPurposesHtml.Replace("{{AnnualInterest}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                                                                    }
                                                                    else
                                                                    {
                                                                        LoanSummaryForTaxPurposesHtml.Replace("{{AnnualInterest}}", "R0.00");
                                                                    }

                                                                    res = 0.0m;
                                                                    if (!string.IsNullOrEmpty(HomeLoanSummary.Annual_Insurance) && decimal.TryParse(HomeLoanSummary.Annual_Insurance, out res))
                                                                    {
                                                                        LoanSummaryForTaxPurposesHtml.Replace("{{AnnualInsurance}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                                                                    }
                                                                    else
                                                                    {
                                                                        LoanSummaryForTaxPurposesHtml.Replace("{{AnnualInsurance}}", "R0.00");
                                                                    }

                                                                    res = 0.0m;
                                                                    if (!string.IsNullOrEmpty(HomeLoanSummary.Annual_Service_Fee) && decimal.TryParse(HomeLoanSummary.Annual_Service_Fee, out res))
                                                                    {
                                                                        LoanSummaryForTaxPurposesHtml.Replace("{{AnnualServiceFee}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                                                                    }
                                                                    else
                                                                    {
                                                                        LoanSummaryForTaxPurposesHtml.Replace("{{AnnualServiceFee}}", "R0.00");
                                                                    }

                                                                    res = 0.0m;
                                                                    if (!string.IsNullOrEmpty(HomeLoanSummary.Annual_Legal_Costs) && decimal.TryParse(HomeLoanSummary.Annual_Legal_Costs, out res))
                                                                    {
                                                                        LoanSummaryForTaxPurposesHtml.Replace("{{AnnualLegalCosts}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                                                                    }
                                                                    else
                                                                    {
                                                                        LoanSummaryForTaxPurposesHtml.Replace("{{AnnualLegalCosts}}", "R0.00");
                                                                    }

                                                                    res = 0.0m;
                                                                    if (!string.IsNullOrEmpty(HomeLoanSummary.Annual_Total_Recvd) && decimal.TryParse(HomeLoanSummary.Annual_Total_Recvd, out res))
                                                                    {
                                                                        LoanSummaryForTaxPurposesHtml.Replace("{{AnnualTotalAmountReceived}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                                                                    }
                                                                    else
                                                                    {
                                                                        LoanSummaryForTaxPurposesHtml.Replace("{{AnnualTotalAmountReceived}}", "R0.00");
                                                                    }

                                                                    #endregion

                                                                    #region Installment details div

                                                                    res = 0.0m;
                                                                    if (!string.IsNullOrEmpty(HomeLoanSummary.Basic_Instalment) && decimal.TryParse(HomeLoanSummary.Basic_Instalment, out res))
                                                                    {
                                                                        LoanInstalmentHtml.Replace("{{BasicInstalment}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                                                                    }
                                                                    else
                                                                    {
                                                                        LoanInstalmentHtml.Replace("{{BasicInstalment}}", "R0.00");
                                                                    }

                                                                    res = 0.0m;
                                                                    if (!string.IsNullOrEmpty(HomeLoanSummary.Houseowner_Ins) && decimal.TryParse(HomeLoanSummary.Houseowner_Ins, out res))
                                                                    {
                                                                        LoanInstalmentHtml.Replace("{{HouseownerInsurance}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                                                                    }
                                                                    else
                                                                    {
                                                                        LoanInstalmentHtml.Replace("{{HouseownerInsurance}}", "R0.00");
                                                                    }

                                                                    res = 0.0m;
                                                                    if (!string.IsNullOrEmpty(HomeLoanSummary.Loan_Protection) && decimal.TryParse(HomeLoanSummary.Loan_Protection, out res))
                                                                    {
                                                                        LoanInstalmentHtml.Replace("{{LoanProtectionAssurance}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                                                                    }
                                                                    else
                                                                    {
                                                                        LoanInstalmentHtml.Replace("{{LoanProtectionAssurance}}", "R0.00");
                                                                    }

                                                                    res = 0.0m;
                                                                    if (!string.IsNullOrEmpty(HomeLoanSummary.Recovery_Fee_Debit) && decimal.TryParse(HomeLoanSummary.Recovery_Fee_Debit, out res))
                                                                    {
                                                                        LoanInstalmentHtml.Replace("{{RecoveryOfFeeDebits}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                                                                    }
                                                                    else
                                                                    {
                                                                        LoanInstalmentHtml.Replace("{{RecoveryOfFeeDebits}}", "R0.00");
                                                                    }

                                                                    res = 0.0m;
                                                                    if (!string.IsNullOrEmpty(HomeLoanSummary.Capital_Redemption) && decimal.TryParse(HomeLoanSummary.Capital_Redemption, out res))
                                                                    {
                                                                        LoanInstalmentHtml.Replace("{{CapitalRedemption}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                                                                    }
                                                                    else
                                                                    {
                                                                        LoanInstalmentHtml.Replace("{{CapitalRedemption}}", "R0.00");
                                                                    }

                                                                    res = 0.0m;
                                                                    if (!string.IsNullOrEmpty(HomeLoanSummary.Service_Fee) && decimal.TryParse(HomeLoanSummary.Service_Fee, out res))
                                                                    {
                                                                        LoanInstalmentHtml.Replace("{{ServiceFee}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                                                                    }
                                                                    else
                                                                    {
                                                                        LoanInstalmentHtml.Replace("{{ServiceFee}}", "R0.00");
                                                                    }

                                                                    res = 0.0m;
                                                                    if (!string.IsNullOrEmpty(HomeLoanSummary.Total_Instalment) && decimal.TryParse(HomeLoanSummary.Total_Instalment, out res))
                                                                    {
                                                                        LoanInstalmentHtml.Replace("{{TotalInstalment}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                                                                    }
                                                                    else
                                                                    {
                                                                        LoanInstalmentHtml.Replace("{{TotalInstalment}}", "R0.00");
                                                                    }

                                                                    LoanInstalmentHtml.Replace("{{InstalmentDate}}", DateTime.Now.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy));

                                                                    #endregion
                                                                }
                                                                else
                                                                {
                                                                    LoanSummaryForTaxPurposesHtml.Replace("{{AnnualInterest}}", "R0.00");
                                                                    LoanSummaryForTaxPurposesHtml.Replace("{{AnnualInsurance}}", "R0.00");
                                                                    LoanSummaryForTaxPurposesHtml.Replace("{{AnnualServiceFee}}", "R0.00");
                                                                    LoanSummaryForTaxPurposesHtml.Replace("{{AnnualLegalCosts}}", "R0.00");
                                                                    LoanSummaryForTaxPurposesHtml.Replace("{{AnnualTotalAmountReceived}}", "R0.00");

                                                                    LoanInstalmentHtml.Replace("{{BasicInstalment}}", "R0.00");
                                                                    LoanInstalmentHtml.Replace("{{HouseownerInsurance}}", "R0.00");
                                                                    LoanInstalmentHtml.Replace("{{LoanProtectionAssurance}}", "R0.00");
                                                                    LoanInstalmentHtml.Replace("{{RecoveryOfFeeDebits}}", "R0.00");
                                                                    LoanInstalmentHtml.Replace("{{CapitalRedemption}}", "R0.00");
                                                                    LoanInstalmentHtml.Replace("{{ServiceFee}}", "R0.00");
                                                                    LoanInstalmentHtml.Replace("{{TotalInstalment}}", "R0.00");
                                                                    LoanInstalmentHtml.Replace("{{InstalmentDate}}", DateTime.Now.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy));
                                                                }

                                                                TabContentHtml.Append(LoanSummaryForTaxPurposesHtml.ToString());
                                                                TabContentHtml.Append(LoanInstalmentHtml.ToString());

                                                                TabContentHtml.Append(HtmlConstants.END_DIV_TAG);
                                                                counter++;
                                                            });

                                                            TabContentHtml.Append((HomeLoans.Count > 1) ? HtmlConstants.END_DIV_TAG : string.Empty);
                                                            widgetHtml.Replace("{{TabContentsDiv}}", TabContentHtml.ToString());
                                                            htmlString.Append(widgetHtml.ToString());
                                                        }
                                                    }
                                                }
                                                else if (mergedlst[i].WidgetName == HtmlConstants.NEDBANK_PORTFOLIO_CUSTOMER_DETAILS_WIDGET_NAME)
                                                {
                                                    string jsonstr = "{'CustomerId': 171001255307, 'Title': 'MR.', 'FirstName':'MATHYS','SurName':'NKHUMISE','AddressLine0':'VERDEAU LIFESTYLE ESTATE', 'AddressLine1':'6 HERCULE CRESCENT DRIVE','AddressLine2':'WELLINGTON','AddressLine3':'7655','AddressLine4':'', 'Mask_Cell_No': '+2367 345 786', 'EmailAddress' : 'mknumise@domain.com'}";
                                                    if (jsonstr != string.Empty && validationEngine.IsValidJson(jsonstr))
                                                    {
                                                        var customer = JsonConvert.DeserializeObject<DM_CustomerMaster>(jsonstr);
                                                        var widgetHtml = new StringBuilder(HtmlConstants.NEDBANK_PORTFOLIO_CUSTOMER_DETAILS_WIDGET_HTML);
                                                        widgetHtml.Replace("{{CustomerName}}", (customer.Title + " " + customer.FirstName + " " + customer.SurName));
                                                        widgetHtml.Replace("{{CustomerId}}", customer.CustomerId.ToString());
                                                        widgetHtml.Replace("{{MobileNumber}}", customer.Mask_Cell_No);
                                                        widgetHtml.Replace("{{EmailAddress}}", customer.EmailAddress);
                                                        htmlString.Append(widgetHtml.ToString());
                                                    }
                                                }
                                                else if (mergedlst[i].WidgetName == HtmlConstants.NEDBANK_PORTFOLIO_CUSTOMER_ADDRESS_WIDGET_NAME)
                                                {
                                                    string jsonstr = "{'CustomerId': 171001255307, 'Title': 'MR.', 'FirstName':'MATHYS','SurName':'NKHUMISE','AddressLine0':'VERDEAU LIFESTYLE ESTATE', 'AddressLine1':'6 HERCULE CRESCENT DRIVE','AddressLine2':'WELLINGTON','AddressLine3':'7655','AddressLine4':'', 'Mask_Cell_No': '+2367 345 786', 'EmailAddress' : 'mknumise@domain.com'}";
                                                    if (jsonstr != string.Empty && validationEngine.IsValidJson(jsonstr))
                                                    {
                                                        var customer = JsonConvert.DeserializeObject<DM_CustomerMaster>(jsonstr);
                                                        var widgetHtml = new StringBuilder(HtmlConstants.NEDBANK_PORTFOLIO_CUSTOMER_ADDRESS_WIDGET_HTML);
                                                        var custAddress = (!string.IsNullOrEmpty(customer.AddressLine0) ? (customer.AddressLine0 + "<br>") : string.Empty) +
                                                        (!string.IsNullOrEmpty(customer.AddressLine1) ? (customer.AddressLine1 + "<br>") : string.Empty) +
                                                        (!string.IsNullOrEmpty(customer.AddressLine2) ? (customer.AddressLine2 + "<br>") : string.Empty) +
                                                        (!string.IsNullOrEmpty(customer.AddressLine3) ? (customer.AddressLine3 + "<br>") : string.Empty) +
                                                        (!string.IsNullOrEmpty(customer.AddressLine4) ? customer.AddressLine4 : string.Empty);
                                                        widgetHtml.Replace("{{CustomerAddress}}", custAddress);
                                                        htmlString.Append(widgetHtml.ToString());
                                                    }
                                                }
                                                else if (mergedlst[i].WidgetName == HtmlConstants.NEDBANK_PORTFOLIO_CLIENT_CONTACT_DETAILS_WIDGET_NAME)
                                                {
                                                    var widgetHtml = new StringBuilder(HtmlConstants.NEDBANK_PORTFOLIO_CLIENT_CONTACT_DETAILS_WIDGET_HTML);
                                                    widgetHtml.Replace("{{MobileNumber}}", "0860 555 111");
                                                    widgetHtml.Replace("{{EmailAddress}}", "supportdesk@nedbank.com");
                                                    htmlString.Append(widgetHtml.ToString());
                                                }
                                                else if (mergedlst[i].WidgetName == HtmlConstants.NEDBANK_PORTFOLIO_ACCOUNT_SUMMARY_WIDGET_NAME)
                                                {
                                                    var widgetHtml = new StringBuilder(HtmlConstants.NEDBANK_PORTFOLIO_ACCOUNT_SUMMARY_WIDGET_HTML);
                                                    string jsonstr = "[{'AccountType': 'Investment', 'TotalCurrentAmount': 'R9 620.98'},{'AccountType': 'Personal Loan', 'TotalCurrentAmount': 'R4 165.00'},{'AccountType': 'Home Loan', 'TotalCurrentAmount': 'R7 969.00'}]";
                                                    if (jsonstr != string.Empty && validationEngine.IsValidJson(jsonstr))
                                                    {
                                                        var _lstAccounts = JsonConvert.DeserializeObject<List<DM_CustomerAccountSummary>>(jsonstr);
                                                        if (_lstAccounts.Count > 0)
                                                        {

                                                            var accountSummaryRows = new StringBuilder();
                                                            _lstAccounts.ForEach(acc =>
                                                            {
                                                                var tr = new StringBuilder();
                                                                tr.Append("<tr class='ht-30'>");
                                                                tr.Append("<td class='text-left'>" + acc.AccountType + " </td>");
                                                                tr.Append("<td class='text-right'>" + acc.TotalCurrentAmount + " </td>");
                                                                tr.Append("</tr>");
                                                                accountSummaryRows.Append(tr.ToString());
                                                            });
                                                            widgetHtml.Replace("{{AccountSummaryRows}}", accountSummaryRows.ToString());
                                                        }
                                                        else
                                                        {
                                                            widgetHtml.Replace("{{AccountSummaryRows}}", "<tr class='ht-30'><td class='text-center' colspan='2'>No records found</td></tr>");
                                                        }
                                                    }

                                                    var rewardPointsDiv = new StringBuilder("<div class='pt-2'><table class='LoanTransactionTable customTable'><thead><tr class='ht-30'><th class='text-left'>{{RewardType}} </th><th class='text-right'>{{RewardPoints}}</th></tr></thead></table></div>");
                                                    rewardPointsDiv.Replace("{{RewardType}}", "Greenbacks rewards points");
                                                    rewardPointsDiv.Replace("{{RewardPoints}}", "234");
                                                    widgetHtml.Replace("{{RewardPointsDiv}}", rewardPointsDiv.ToString());

                                                    htmlString.Append(widgetHtml.ToString());
                                                }
                                                else if (mergedlst[i].WidgetName == HtmlConstants.NEDBANK_PORTFOLIO_ACCOUNT_ANALYSIS_WIDGET_NAME)
                                                {
                                                    var widgetHtml = new StringBuilder(HtmlConstants.NEDBANK_PORTFOLIO_ACCOUNT_ANALYSIS_WIDGET_HTML);
                                                    var data = "[{\"AccountType\": \"Investment\",\"MonthwiseAmount\" : [{\"Month\": \"Jan\", \"Amount\": 9456.12}, {\"Month\": \"Feb\", \"Amount\": 9620.98}]},{\"AccountType\": \"Personal Loan\",\"MonthwiseAmount\" : [{\"Month\": \"Jan\", \"Amount\": -4465.00}, {\"Month\": \"Feb\", \"Amount\": -4165.00}]},{\"AccountType\": \"Home Loan\",\"MonthwiseAmount\" : [{\"Month\": \"Jan\", \"Amount\": -8969.00}, {\"Month\": \"Feb\", \"Amount\": -7969.00}]}]";
                                                    widgetHtml.Append("<input type='hidden' id='HiddenAccountAnalysisBarGraphData' value='" + data + "'/>");
                                                    //widgetHtml.Append(HtmlConstants.PORTFOLIO_ACCOUNT_ANALYSIS_BAR_GRAPH_SCRIPT);
                                                    htmlString.Append(widgetHtml.ToString());
                                                }
                                                else if (mergedlst[i].WidgetName == HtmlConstants.NEDBANK_PORTFOLIO_REMINDERS_WIDGET_NAME)
                                                {
                                                    var widgetHtml = new StringBuilder(HtmlConstants.NEDBANK_PORTFOLIO_REMINDER_AND_RECOMMENDATION_WIDGET_HTML);
                                                    string jsonstr = "[{ 'Title': 'Update Missing Inofrmation', 'Action': 'Update' },{ 'Title': 'Your Rewards Video is available', 'Action': 'View' },{ 'Title': 'Payment Due for Home Loan', 'Action': 'Pay' }, { title: 'Need financial planning for savings.', action: 'Call Me' },{ title: 'Subscribe/Unsubscribe Alerts.', action: 'Apply' },{ title: 'Your credit card payment is due now.', action: 'Pay' }]";
                                                    if (jsonstr != string.Empty && validationEngine.IsValidJson(jsonstr))
                                                    {
                                                        IList<ReminderAndRecommendation> reminderAndRecommendations = JsonConvert.DeserializeObject<List<ReminderAndRecommendation>>(jsonstr);
                                                        StringBuilder reminderstr = new StringBuilder();
                                                        reminderAndRecommendations.ToList().ForEach(item =>
                                                        {
                                                            reminderstr.Append("<div class='row'><div class='col-lg-9 text-left'><p class='p-1' style='background-color: #dce3dc;'>" + item.Title + " </p></div><div class='col-lg-3 text-left'><a href='javascript:void(0)' target='_blank'><i class='fa fa-caret-left fa-3x float-left text-success'></i><span class='mt-2 d-inline-block ml-2'>" + item.Action + "</span></a></div></div>");
                                                        });
                                                        widgetHtml.Replace("{{ReminderAndRecommendation}}", reminderstr.ToString());
                                                        htmlString.Append(widgetHtml.ToString());
                                                    }
                                                }
                                                else if (mergedlst[i].WidgetName == HtmlConstants.NEDBANK_PORTFOLIO_NEWS_ALERT_WIDGET_NAME)
                                                {
                                                    var widgetHtml = new StringBuilder(HtmlConstants.NEDBANK_PORTFOLIO_NEWS_ALERT_WIDGET_HTML);
                                                    string jsonstr = "{ \"Message1\": \"Covid 19 and the subsequent lockdown has affected all areas of our daily lives. The way we work, the way we bank and how we interact with each other.\", \"Message2\": \"We want you to know we are in this together. That's why we are sharing advice, tips and news updates with you on ways to bank as well as ways to keep yorself and your loved ones safe.\", \"Message3\": \"We would like to remind you of the credit life insurance benefits available to you through your Nedbank Insurance policy. When you pass away, Nedbank Insurance will cover your outstanding loan amount. If you are permanently employed, you will also enjoy cover for comprehensive disability and loss of income. The disability benefit will cover your monthly instalments if you cannot earn your usual income due to illness or bodily injury.\", \"Message4\": \"\", \"Message5\": \"\" }";
                                                    if (jsonstr != string.Empty && validationEngine.IsValidJson(jsonstr))
                                                    {
                                                        var newsAlert = JsonConvert.DeserializeObject<NewsAlert>(jsonstr);
                                                        var newsAlertStr = (!string.IsNullOrEmpty(newsAlert.Message1) ? ("<p>" + newsAlert.Message1 + "</p>") : string.Empty) +
                                                            (!string.IsNullOrEmpty(newsAlert.Message2) ? ("<p>" + newsAlert.Message2 + "</p>") : string.Empty) +
                                                            (!string.IsNullOrEmpty(newsAlert.Message3) ? ("<p>" + newsAlert.Message3 + "</p>") : string.Empty) +
                                                            (!string.IsNullOrEmpty(newsAlert.Message4) ? ("<p>" + newsAlert.Message4 + "</p>") : string.Empty) +
                                                            (!string.IsNullOrEmpty(newsAlert.Message5) ? ("<p>" + newsAlert.Message5 + "</p>") : string.Empty);
                                                        widgetHtml.Replace("{{NewsAlert}}", newsAlertStr);
                                                        htmlString.Append(widgetHtml.ToString());
                                                    }
                                                }
                                                else if (mergedlst[i].WidgetName == HtmlConstants.NEDBANK_GREENBACKS_TOTAL_REWARDS_POINTS_WIDGET_NAME)
                                                {
                                                    var widgetHtml = new StringBuilder(HtmlConstants.NEDBANK_GREENBACKS_TOTAL_REWARDS_POINTS_WIDGET_HTML);
                                                    widgetHtml.Replace("{{TotalRewardsPoints}}", "482");
                                                    htmlString.Append(widgetHtml.ToString());
                                                }
                                                else if (mergedlst[i].WidgetName == HtmlConstants.NEDBANK_GREENBACKS_CONTACT_US_WIDGET_NAME)
                                                {
                                                    htmlString.Append(HtmlConstants.NEDBANK_GREENBACKS_CONTACT_US_WIDGET_HTML);
                                                }
                                                else if (mergedlst[i].WidgetName == HtmlConstants.NEDBANK_YTD_REWARDS_POINTS_BAR_GRAPH_WIDGET_NAME)
                                                {
                                                    var widgetHtml = new StringBuilder(HtmlConstants.NEDBANK_YTD_REWARDS_POINTS_BAR_GRAPH_WIDGET_HTML);
                                                    var data = "[{\"Month\": \"Jan\",\"RewardPoint\" : 98}, {\"Month\": \"Feb\",\"RewardPoint\" : 112}, {\"Month\": \"Mar\",\"RewardPoint\" : 128}, {\"Month\": \"Apr\",\"RewardPoint\" : 144}]";
                                                    widgetHtml.Append("<input type='hidden' id='HiddenYTDRewardPointsBarGraphData' value='" + data + "'/>");
                                                    //widgetHtml.Append(HtmlConstants.PORTFOLIO_ACCOUNT_ANALYSIS_BAR_GRAPH_SCRIPT);
                                                    htmlString.Append(widgetHtml.ToString());
                                                }
                                                else if (mergedlst[i].WidgetName == HtmlConstants.NEDBANK_POINTS_REDEEMED_YTD_BAR_GRAPH_WIDGET_NAME)
                                                {
                                                    var widgetHtml = new StringBuilder(HtmlConstants.NEDBANK_POINTS_REDEEMED_YTD_BAR_GRAPH_WIDGET_HTML);
                                                    var data = "[{\"Month\": \"Jan\",\"RedeemedPoints\" : 58}, {\"Month\": \"Feb\",\"RedeemedPoints\" : 71}, {\"Month\": \"Mar\",\"RedeemedPoints\" : 63}, {\"Month\": \"Apr\",\"RedeemedPoints\" : 84}]";
                                                    widgetHtml.Append("<input type='hidden' id='HiddenPointsRedeemedBarGraphData' value='" + data + "'/>");
                                                    //widgetHtml.Append(HtmlConstants.PORTFOLIO_ACCOUNT_ANALYSIS_BAR_GRAPH_SCRIPT);
                                                    htmlString.Append(widgetHtml.ToString());
                                                }
                                                else if (mergedlst[i].WidgetName == HtmlConstants.NEDBANK_PRODUCT_RELATED_POINTS_EARNED_BAR_GRAPH_WIDGET_NAME)
                                                {
                                                    var widgetHtml = new StringBuilder(HtmlConstants.NEDBANK_PRODUCT_RELATED_POINTS_EARNED_BAR_GRAPH_WIDGET_HTML);
                                                    var data = "[{\"AccountType\": \"Investment\",\"MonthwiseAmount\" : [{\"Month\": \"Jan\", \"RewardPoint\": 34}, {\"Month\": \"Feb\", \"RewardPoint\": 29},{\"Month\": \"Mar\", \"RewardPoint\": 41}, {\"Month\": \"Apr\", \"RewardPoint\": 48}]}, {\"AccountType\": \"Personal Loan\",\"MonthwiseAmount\" : [{\"Month\": \"Jan\", \"RewardPoint\": 27}, {\"Month\": \"Feb\", \"RewardPoint\": 45},{\"Month\": \"Mar\", \"RewardPoint\": 36}, {\"Month\": \"Apr\", \"RewardPoint\": 51}]}, {\"AccountType\": \"Home Loan\",\"MonthwiseAmount\" : [{\"Month\": \"Jan\", \"RewardPoint\": 37}, {\"Month\": \"Feb\", \"RewardPoint\": 38},{\"Month\": \"Mar\", \"RewardPoint\": 51}, {\"Month\": \"Apr\", \"RewardPoint\": 45}]}]"; ;
                                                    widgetHtml.Append("<input type='hidden' id='HiddenProductRelatedPointsEarnedBarGraphData' value='" + data + "'/>");
                                                    //widgetHtml.Append(HtmlConstants.PORTFOLIO_ACCOUNT_ANALYSIS_BAR_GRAPH_SCRIPT);
                                                    htmlString.Append(widgetHtml.ToString());
                                                }
                                                else if (mergedlst[i].WidgetName == HtmlConstants.NEDBANK_CATEGORY_SPEND_REWARDS_PIE_CHART_WIDGET_NAME)
                                                {
                                                    var widgetHtml = new StringBuilder(HtmlConstants.NEDBANK_CATEGORY_SPEND_REWARDS_PIE_CHART_WIDGET_HTML);
                                                    var data = "[{\"Category\": \"Fuel\",\"SpendReward\" : 34}, {\"Category\": \"Groceries\",\"SpendReward\" : 15}, {\"Category\": \"Travel\",\"SpendReward\" : 21}, {\"Category\": \"Movies\",\"SpendReward\" : 19}, {\"Category\": \"Shopping\",\"SpendReward\" : 11}]"; ;
                                                    widgetHtml.Append("<input type='hidden' id='HiddenCategorySpendRewardsPieChartData' value='" + data + "'/>");
                                                    //widgetHtml.Append(HtmlConstants.PORTFOLIO_ACCOUNT_ANALYSIS_BAR_GRAPH_SCRIPT);
                                                    htmlString.Append(widgetHtml.ToString());
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
                }

                tempHtml.Append(HtmlConstants.NEDBANK_STATEMENT_HEADER.Replace("{{eConfirmLogo}}", "assets/images/eConfirm.png").Replace("{{NedBankLogo}}", "assets/images/NEDBANKLogo.png").Replace("{{StatementDate}}", DateTime.Now.ToString(ModelConstant.DATE_FORMAT_yyyy_MM_dd)));
                
                //NAV bar will append to html statement, only if statement definition have more than 1 pages 
                if (statementPages.Count > 1)
                {
                    tempHtml.Append(HtmlConstants.NEDBANK_NAV_BAR_HTML.Replace("{{Today}}", DateTime.Now.ToString(ModelConstant.DATE_FORMAT_dd_MMM_yyyy)).Replace("{{NavItemList}}", NavItemList.ToString()));
                }

                tempHtml.Append(HtmlConstants.PAGE_TAB_CONTENT_HEADER);
                tempHtml.Append(htmlString.ToString());
                tempHtml.Append(HtmlConstants.PAGE_TAB_CONTENT_FOOTER);

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

                var footerContent = new StringBuilder(HtmlConstants.NEDBANK_STATEMENT_FOOTER.Replace("{{NedbankSloganImage}}", "assets/images/See_money_differently.PNG").Replace("{{NedbankNameImage}}", "assets/images/NEDBANK_Name.png").Replace("{{FooterText}}", HtmlConstants.NEDBANK_STATEMENT_FOOTER_TEXT_STRING));

                var lastFooterText = string.Empty;
                //if (IsHomeLoanStatement)
                //{
                //    lastFooterText = "<div class='text-center mb-n2'> Directors: V Naidoo (Chairman) MWT Brown (Chief Executive) HR Body BA Dames NP Dongwana EM Kruger RAG Leiht </div> <div class='text-center mb-n2'> L Makalima PM Makwana Prof T Marwala Dr MA Matooane RK Morathi (Chief Finance Officer) MC Nkuhlu (Chief Operating Officer) </div> <div class='text-center mb-n2'> S Subramoney IG Williamson Company Secretory: J Katzin 01.06.2020 </div>";
                //}
                footerContent.Replace("{{LastFooterText}}", lastFooterText);
                
                tempHtml.Append(footerContent.ToString());
                tempHtml.Append(HtmlConstants.CONTAINER_DIV_HTML_FOOTER);
                finalHtml = tempHtml.ToString();
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

        private string AccountInformationWidgetFormatting(PageWidget pageWidget, int counter, Statement statement, Page page, string divHeight)
        {
            var widgetId = "PageWidgetId_" + pageWidget.Identifier + "_Counter" + counter.ToString();
            var widgetHTML = HtmlConstants.ACCOUNT_INFORMATION_WIDGET_HTML_FOR_STMT.Replace("{{AccountInfoData}}", "{{AccountInfoData_" + page.Identifier + "_" + pageWidget.Identifier + "}}");
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

        #region Nedbank Tenant static widget formatting

        private string CustomerDetailsWidgetFormatting(PageWidget pageWidget, int counter, Page page)
        {
            var widgetId = "PageWidgetId_" + pageWidget.Identifier + "_Counter" + counter.ToString();
            var HtmlWidget = new StringBuilder(HtmlConstants.CUSTOMER_DETAILS_WIDGET_HTML_SMT);
            HtmlWidget.Replace("{{CustomerDetails}}", "{{CustomerDetails_" + page.Identifier + "_" + pageWidget.Identifier + "}}");
            HtmlWidget.Replace("{{MaskCellNo}}", "{{MaskCellNo_" + page.Identifier + "_" + pageWidget.Identifier + "}}");
            HtmlWidget.Replace("{{WidgetId}}", widgetId);
            return HtmlWidget.ToString();
        }

        private string BranchDetailsWidgetFormatting(PageWidget pageWidget, int counter, Page page)
        {
            //if(page.PageTypeName == HtmlConstants.HOME_LOAN_PAGE_TYPE)
            var widgetId = "PageWidgetId_" + pageWidget.Identifier + "_Counter" + counter.ToString();
            var htmlWidget = new StringBuilder(HtmlConstants.BRANCH_DETAILS_WIDGET_HTML_SMT);
            htmlWidget.Replace("{{BranchDetails}}", "{{BranchDetails_" + page.Identifier + "_" + pageWidget.Identifier + "}}");
            htmlWidget.Replace("{{ContactCenter}}", "{{ContactCenter_" + page.Identifier + "_" + pageWidget.Identifier + "}}");
             htmlWidget.Replace("{{WidgetId}}", widgetId);
            return htmlWidget.ToString();
        }

        private string InvestmentPortfolioStatementWidgetFormatting(PageWidget pageWidget, int counter, Page page)
        {
            var widgetId = "PageWidgetId_" + pageWidget.Identifier + "_Counter" + counter.ToString();
            var InvestmentPortfolioHtmlWidget = new StringBuilder(HtmlConstants.INVESTMENT_PORTFOLIO_STATEMENT_WIDGET_HTML);
            InvestmentPortfolioHtmlWidget.Replace("{{DSName}}", "{{DSName_" + page.Identifier + "_" + pageWidget.Identifier + "}}");
            InvestmentPortfolioHtmlWidget.Replace("{{TotalClosingBalance}}", "{{TotalClosingBalance_" + page.Identifier + "_" + pageWidget.Identifier + "}}");
            InvestmentPortfolioHtmlWidget.Replace("{{DayOfStatement}}", "{{DayOfStatement_" + page.Identifier + "_" + pageWidget.Identifier + "}}");
            InvestmentPortfolioHtmlWidget.Replace("{{InvestorID}}", "{{InvestorID_" + page.Identifier + "_" + pageWidget.Identifier + "}}");
            InvestmentPortfolioHtmlWidget.Replace("{{StatementPeriod}}", "{{StatementPeriod_" + page.Identifier + "_" + pageWidget.Identifier + "}}");
            InvestmentPortfolioHtmlWidget.Replace("{{StatementDate}}", "{{StatementDate_" + page.Identifier + "_" + pageWidget.Identifier + "}}");
            InvestmentPortfolioHtmlWidget.Replace("{{WidgetId}}", widgetId);
            InvestmentPortfolioHtmlWidget.Replace("{{FirstName}}", "{{FirstName_" + page.Identifier + "_" + pageWidget.Identifier + "}}");
            InvestmentPortfolioHtmlWidget.Replace("{{SurName}}", "{{SurName_" + page.Identifier + "_" + pageWidget.Identifier + "}}");
            return InvestmentPortfolioHtmlWidget.ToString();
        }

        private string InvestorPerformanceWidgetFormatting(PageWidget pageWidget, int counter, Page page)
        {
            var widgetId = "PageWidgetId_" + pageWidget.Identifier + "_Counter" + counter.ToString();
            var InvestorPerformanceHtmlWidget = HtmlConstants.INVESTOR_PERFORMANCE_WIDGET_HTML;
            InvestorPerformanceHtmlWidget = InvestorPerformanceHtmlWidget.Replace("{{ProductType}}", "{{ProductType_" + page.Identifier + "_" + pageWidget.Identifier + "}}");
            InvestorPerformanceHtmlWidget = InvestorPerformanceHtmlWidget.Replace("{{OpeningBalanceAmount}}", "{{OpeningBalanceAmount_" + page.Identifier + "_" + pageWidget.Identifier + "}}");
            InvestorPerformanceHtmlWidget = InvestorPerformanceHtmlWidget.Replace("{{ClosingBalanceAmount}}", "{{ClosingBalanceAmount_" + page.Identifier + "_" + pageWidget.Identifier + "}}");
            InvestorPerformanceHtmlWidget = InvestorPerformanceHtmlWidget.Replace("{{WidgetId}}", widgetId);
            return InvestorPerformanceHtmlWidget;
        }

        private string BreakdownOfInvestmentAccountsWidgetFormatting(PageWidget pageWidget, int counter, Page page)
        {
            var widgetId = "PageWidgetId_" + pageWidget.Identifier + "_Counter" + counter.ToString();
            var htmlWidget = HtmlConstants.BREAKDOWN_OF_INVESTMENT_ACCOUNTS_WIDGET_HTML.Replace("{{NavTab}}", "{{NavTab_" + page.Identifier + "_" + pageWidget.Identifier + "}}").Replace("{{TabContentsDiv}}", "{{TabContentsDiv_" + page.Identifier + "_" + pageWidget.Identifier + "}}");
            htmlWidget = htmlWidget.Replace("{{WidgetId}}", widgetId);
            return htmlWidget;
        }

        private string ExplanatoryNotesWidgetFormatting(PageWidget pageWidget, int counter, Page page)
        {
            var widgetId = "PageWidgetId_" + pageWidget.Identifier + "_Counter" + counter.ToString();
            var htmlWidget = HtmlConstants.EXPLANATORY_NOTES_WIDGET_HTML.Replace("{{Notes}}", "{{Notes_" + page.Identifier + "_" + pageWidget.Identifier + "}}");
            htmlWidget = htmlWidget.Replace("{{WidgetId}}", widgetId);
            return htmlWidget;
        }

        private string MarketingServiceMessageWidgetFormatting(PageWidget pageWidget, int counter, Page page, int MarketingMessageCounter)
        {
            var widgetId = "PageWidgetId_" + pageWidget.Identifier + "_Counter" + counter.ToString();
            var htmlWidget = HtmlConstants.SERVICE_WIDGET_HTML.Replace("{{ServiceMessageHeader}}", "{{ServiceMessageHeader_" + page.Identifier + "_" + pageWidget.Identifier + "_" + MarketingMessageCounter + "}}").Replace("{{ServiceMessageText}}", "{{ServiceMessageText_" + page.Identifier + "_" + pageWidget.Identifier + "_" + MarketingMessageCounter + "}}");
            htmlWidget = htmlWidget.Replace("{{WidgetId}}", widgetId);
            return htmlWidget;
        }

        private string PersonalLoanDetailWidgetFormatting(PageWidget pageWidget, int counter, Page page)
        {
            var widgetId = "PageWidgetId_" + pageWidget.Identifier + "_Counter" + counter.ToString();
            var htmlWidget = new StringBuilder(HtmlConstants.PERSONAL_LOAN_DETAIL_HTML);
            htmlWidget.Replace("{{TotalLoanAmount}}", "{{TotalLoanAmount_" + page.Identifier + "_" + pageWidget.Identifier + "}}");
            htmlWidget.Replace("{{OutstandingBalance}}", "{{OutstandingBalance_" + page.Identifier + "_" + pageWidget.Identifier + "}}");
            htmlWidget.Replace("{{DueAmount}}", "{{DueAmount_" + page.Identifier + "_" + pageWidget.Identifier + "}}");
            htmlWidget.Replace("{{AccountNumber}}", "{{AccountNumber_" + page.Identifier + "_" + pageWidget.Identifier + "}}");
            htmlWidget.Replace("{{StatementDate}}", "{{StatementDate_" + page.Identifier + "_" + pageWidget.Identifier + "}}");
            htmlWidget.Replace("{{StatementPeriod}}", "{{StatementPeriod_" + page.Identifier + "_" + pageWidget.Identifier + "}}");
            htmlWidget.Replace("{{ArrearsAmount}}", "{{ArrearsAmount_" + page.Identifier + "_" + pageWidget.Identifier + "}}");
            htmlWidget.Replace("{{AnnualRate}}", "{{AnnualRate_" + page.Identifier + "_" + pageWidget.Identifier + "}}");
            htmlWidget.Replace("{{MonthlyInstallment}}", "{{MonthlyInstallment_" + page.Identifier + "_" + pageWidget.Identifier + "}}");
            htmlWidget.Replace("{{Terms}}", "{{Terms_" + page.Identifier + "_" + pageWidget.Identifier + "}}");
            htmlWidget.Replace("{{DueByDate}}", "{{DueByDate_" + page.Identifier + "_" + pageWidget.Identifier + "}}");
            htmlWidget = htmlWidget.Replace("{{WidgetId}}", widgetId);
            return htmlWidget.ToString();
        }

        private string PersonalLoanTransactionWidgetFormatting(PageWidget pageWidget, int counter, Page page)
        {
            var widgetId = "PageWidgetId_" + pageWidget.Identifier + "_Counter" + counter.ToString();
            var htmlWidget = new StringBuilder(HtmlConstants.PERSONAL_LOAN_TRANSACTION_HTML);
            htmlWidget.Replace("{{PersonalLoanTransactionRow}}", "{{PersonalLoanTransactionRow_" + page.Identifier + "_" + pageWidget.Identifier + "}}");
            htmlWidget = htmlWidget.Replace("{{WidgetId}}", widgetId);
            return htmlWidget.ToString();
        }

        private string PersonalLoanPaymentDueWidgetFormatting(PageWidget pageWidget, int counter, Page page)
        {
            var widgetId = "PageWidgetId_" + pageWidget.Identifier + "_Counter" + counter.ToString();
            var htmlWidget = new StringBuilder(HtmlConstants.PERSONAL_LOAN_PAYMENT_DUE_HTML);
            htmlWidget.Replace("{{After120Days}}", "{{After120Days_" + page.Identifier + "_" + pageWidget.Identifier + "}}");
            htmlWidget.Replace("{{After90Days}}", "{{After90Days_" + page.Identifier + "_" + pageWidget.Identifier + "}}");
            htmlWidget.Replace("{{After60Days}}", "{{After60Days_" + page.Identifier + "_" + pageWidget.Identifier + "}}");
            htmlWidget.Replace("{{After30Days}}", "{{After30Days_" + page.Identifier + "_" + pageWidget.Identifier + "}}");
            htmlWidget.Replace("{{Current}}", "{{Current_" + page.Identifier + "_" + pageWidget.Identifier + "}}");
            htmlWidget = htmlWidget.Replace("{{WidgetId}}", widgetId);
            return htmlWidget.ToString();
        }

        private string SpecialMessageWidgetFormatting(PageWidget pageWidget, Page page)
        {
            return "{{SpecialMessageTextDataDiv_" + page.Identifier + "_" + pageWidget.Identifier + "}}";
        }

        private string PersonalLoanInsuranceMessageWidgetFormatting(PageWidget pageWidget, Page page)
        {
            return "{{PersonalLoanInsuranceMessagesDiv_" + page.Identifier + "_" + pageWidget.Identifier + "}}";
        }

        private string PersonalLoanTotalAmountDetailWidgetFormatting(PageWidget pageWidget, int counter, Page page)
        {
            var widgetId = "PageWidgetId_" + pageWidget.Identifier + "_Counter" + counter.ToString();
            var htmlWidget = new StringBuilder(HtmlConstants.PERSONAL_LOAN_TOTAL_AMOUNT_DETAIL_WIDGET_HTML);
            htmlWidget.Replace("{{TotalLoanAmount}}", "{{TotalLoanAmount_" + page.Identifier + "_" + pageWidget.Identifier + "}}");
            htmlWidget.Replace("{{OutstandingBalance}}", "{{OutstandingBalance_" + page.Identifier + "_" + pageWidget.Identifier + "}}");
            htmlWidget.Replace("{{DueAmount}}", "{{DueAmount_" + page.Identifier + "_" + pageWidget.Identifier + "}}");
            htmlWidget.Replace("{{WidgetId}}", widgetId);
            return htmlWidget.ToString();
        }

        private string PersonalLoanAccountsBreakdownsWidgetFormatting(PageWidget pageWidget, int counter, Page page)
        {
            var widgetId = "PageWidgetId_" + pageWidget.Identifier + "_Counter" + counter.ToString();
            var htmlWidget = new StringBuilder(HtmlConstants.PERSONAL_LOAN_ACCOUNTS_BREAKDOWN_WIDGET_HTML).Replace("{{NavTab}}", "{{NavTab_" + page.Identifier + "_" + pageWidget.Identifier + "}}").Replace("{{TabContentsDiv}}", "{{TabContentsDiv_" + page.Identifier + "_" + pageWidget.Identifier + "}}");
            htmlWidget.Replace("{{WidgetId}}", widgetId);
            return htmlWidget.ToString();
        }

        private string HomeLoanTotalAmountDetailWidgetFormatting(PageWidget pageWidget, int counter, Page page)
        {
            var widgetId = "PageWidgetId_" + pageWidget.Identifier + "_Counter" + counter.ToString();
            var htmlWidget = new StringBuilder(HtmlConstants.HOME_LOAN_TOTAL_AMOUNT_DETAIL_WIDGET_HTML);
            htmlWidget.Replace("{{TotalHomeLoansAmount}}", "{{TotalHomeLoansAmount_" + page.Identifier + "_" + pageWidget.Identifier + "}}");
            htmlWidget.Replace("{{TotalHomeLoansBalanceOutstanding}}", "{{TotalHomeLoansBalanceOutstanding_" + page.Identifier + "_" + pageWidget.Identifier + "}}");
            htmlWidget.Replace("{{WidgetId}}", widgetId);
            return htmlWidget.ToString();
        }

        private string HomeLoanAccountsBreakdownsWidgetFormatting(PageWidget pageWidget, int counter, Page page)
        {
            var widgetId = "PageWidgetId_" + pageWidget.Identifier + "_Counter" + counter.ToString();
            var htmlWidget = new StringBuilder(HtmlConstants.HOME_LOAN_ACCOUNTS_BREAKDOWN_HTML).Replace("{{NavTab}}", "{{NavTab_" + page.Identifier + "_" + pageWidget.Identifier + "}}").Replace("{{TabContentsDiv}}", "{{TabContentsDiv_" + page.Identifier + "_" + pageWidget.Identifier + "}}");
            htmlWidget.Replace("{{WidgetId}}", widgetId);
            return htmlWidget.ToString();
        }

        private string PortfolioCustomerDetailsWidgetFormatting(PageWidget pageWidget, int counter, Page page)
        {
            var widgetId = "PageWidgetId_" + pageWidget.Identifier + "_Counter" + counter.ToString();
            var htmlwidget = new StringBuilder(HtmlConstants.NEDBANK_PORTFOLIO_CUSTOMER_DETAILS_WIDGET_HTML);
            htmlwidget.Replace("{{CustomerName}}", "{{CustomerName_" + page.Identifier + "_" + pageWidget.Identifier + "}}");
            htmlwidget.Replace("{{CustomerId}}", "{{CustomerId_" + page.Identifier + "_" + pageWidget.Identifier + "}}");
            htmlwidget.Replace("{{MobileNumber}}", "{{MobileNumber_" + page.Identifier + "_" + pageWidget.Identifier + "}}");
            htmlwidget.Replace("{{EmailAddress}}", "{{EmailAddress_" + page.Identifier + "_" + pageWidget.Identifier + "}}");
            htmlwidget.Replace("{{WidgetId}}", widgetId);
            return htmlwidget.ToString();
        }

        private string PortfolioCustomerAddressDetailsWidgetFormatting(PageWidget pageWidget, int counter, Page page)
        {
            var widgetId = "PageWidgetId_" + pageWidget.Identifier + "_Counter" + counter.ToString();
            var htmlwidget = new StringBuilder(HtmlConstants.NEDBANK_PORTFOLIO_CUSTOMER_ADDRESS_WIDGET_HTML);
            htmlwidget.Replace("{{CustomerAddress}}", "{{CustomerAddress_" + page.Identifier + "_" + pageWidget.Identifier + "}}");
            htmlwidget.Replace("{{WidgetId}}", widgetId);
            return htmlwidget.ToString();
        }

        private string PortfolioClientContactDetailsWidgetFormatting(PageWidget pageWidget, int counter, Page page)
        {
            var widgetId = "PageWidgetId_" + pageWidget.Identifier + "_Counter" + counter.ToString();
            var htmlwidget = new StringBuilder(HtmlConstants.NEDBANK_PORTFOLIO_CLIENT_CONTACT_DETAILS_WIDGET_HTML);
            htmlwidget.Replace("{{MobileNumber}}", "{{MobileNumber_" + page.Identifier + "_" + pageWidget.Identifier + "}}");
            htmlwidget.Replace("{{EmailAddress}}", "{{EmailAddress_" + page.Identifier + "_" + pageWidget.Identifier + "}}");
            htmlwidget.Replace("{{WidgetId}}", widgetId);
            return htmlwidget.ToString();
        }

        private string PortfolioAccountSummaryWidgetFormatting(PageWidget pageWidget, int counter, Page page)
        {
            var widgetId = "PageWidgetId_" + pageWidget.Identifier + "_Counter" + counter.ToString();
            var htmlwidget = new StringBuilder(HtmlConstants.NEDBANK_PORTFOLIO_ACCOUNT_SUMMARY_WIDGET_HTML);
            htmlwidget.Replace("{{AccountSummaryRows}}", "{{AccountSummaryRows_" + page.Identifier + "_" + pageWidget.Identifier + "}}");
            htmlwidget.Replace("{{RewardPointsDiv}}", "{{RewardPointsDiv_" + page.Identifier + "_" + pageWidget.Identifier + "}}");
            htmlwidget.Replace("{{WidgetId}}", widgetId);
            return htmlwidget.ToString();
        }

        private string PortfolioAccountAnalysisWidgetFormatting(PageWidget pageWidget, int counter, Page page)
        {
            var htmlWidget = new StringBuilder(HtmlConstants.NEDBANK_PORTFOLIO_ACCOUNT_ANALYSIS_WIDGET_HTML);
            htmlWidget.Replace("AccountAnalysisBarGraphcontainer", "AccountAnalysisBarGraphcontainer_" + page.Identifier + "_" + pageWidget.Identifier + "");
            htmlWidget.Replace("{{WidgetId}}", "PageWidgetId_" + pageWidget.Identifier + "_Counter" + counter.ToString());
            htmlWidget.Append("<input type='hidden' id='HiddenAccountAnalysisGraph_" + page.Identifier + "_" + pageWidget.Identifier + "' value='HiddenAccountAnalysisGraphValue_" + page.Identifier + "_" + pageWidget.Identifier + "'>");
            return htmlWidget.ToString();
        }

        private string PortfolioRemindersWidgetFormatting(PageWidget pageWidget, int counter, Page page)
        {
            var widgetId = "PageWidgetId_" + pageWidget.Identifier + "_Counter" + counter.ToString();
            var htmlwidget = new StringBuilder(HtmlConstants.NEDBANK_PORTFOLIO_REMINDER_AND_RECOMMENDATION_WIDGET_HTML);
            htmlwidget.Replace("{{ReminderAndRecommendation}}", "{{ReminderAndRecommendation_" + page.Identifier + "_" + pageWidget.Identifier + "}}");
            htmlwidget.Replace("{{WidgetId}}", widgetId);
            return htmlwidget.ToString();
        }

        private string PortfolioNewsAlertWidgetFormatting(PageWidget pageWidget, int counter, Page page)
        {
            var widgetId = "PageWidgetId_" + pageWidget.Identifier + "_Counter" + counter.ToString();
            var htmlwidget = new StringBuilder(HtmlConstants.NEDBANK_PORTFOLIO_NEWS_ALERT_WIDGET_HTML);
            htmlwidget.Replace("{{NewsAlert}}", "{{NewsAlert_" + page.Identifier + "_" + pageWidget.Identifier + "}}");
            htmlwidget.Replace("{{WidgetId}}", widgetId);
            return htmlwidget.ToString();
        }

        private string GreenbacksTotalRewardPointsWidgetFormatting(PageWidget pageWidget, int counter, Page page)
        {
            var widgetId = "PageWidgetId_" + pageWidget.Identifier + "_Counter" + counter.ToString();
            var htmlwidget = new StringBuilder(HtmlConstants.NEDBANK_GREENBACKS_TOTAL_REWARDS_POINTS_WIDGET_HTML);
            htmlwidget.Replace("{{TotalRewardsPoints}}", "{{TotalRewardsPoints_" + page.Identifier + "_" + pageWidget.Identifier + "}}");
            htmlwidget.Replace("{{WidgetId}}", widgetId);
            return htmlwidget.ToString();
        }

        private string GreenbacksContactUsWidgetFormatting(PageWidget pageWidget, int counter, string tenantCode)
        {
            var widgetId = "PageWidgetId_" + pageWidget.Identifier + "_Counter" + counter.ToString();
            var htmlwidget = new StringBuilder(HtmlConstants.NEDBANK_GREENBACKS_CONTACT_US_WIDGET_HTML_SMT);
            var greenbackmaster = this.tenantTransactionDataManager.GET_DM_GreenbacksMasterDetails(tenantCode)?.FirstOrDefault();
            if (greenbackmaster != null)
            {
                htmlwidget.Replace("{{JoinGreenbackUrl}}", (!string.IsNullOrEmpty(greenbackmaster.JoinUsUrl) ? greenbackmaster.JoinUsUrl : "javascript:void(0)"));
                htmlwidget.Replace("{{UseGreenbackUrl}}", (!string.IsNullOrEmpty(greenbackmaster.UseUsUrl) ? greenbackmaster.JoinUsUrl : "javascript:void(0)"));
                htmlwidget.Replace("{{SupportDeskContactNumber}}", (!string.IsNullOrEmpty(greenbackmaster.ContactNumber) ? greenbackmaster.ContactNumber : "0860 553 111"));
            }
            else
            {
                htmlwidget.Replace("{{JoinGreenbackUrl}}", "javascript:void(0)");
                htmlwidget.Replace("{{UseGreenbackUrl}}", "javascript:void(0)");
                htmlwidget.Replace("{{SupportDeskContactNumber}}", "0860 553 111");
            }
            htmlwidget.Replace("{{WidgetId}}", widgetId);
            return htmlwidget.ToString();
        }

        private string GreenbacksYTDRewardPointsGraphWidgetFormatting(PageWidget pageWidget, int counter, Page page)
        {
            var htmlWidget = new StringBuilder(HtmlConstants.NEDBANK_YTD_REWARDS_POINTS_BAR_GRAPH_WIDGET_HTML);
            htmlWidget.Replace("YTDRewardPointsBarGraphcontainer", "YTDRewardPointsBarGraphcontainer_" + page.Identifier + "_" + pageWidget.Identifier + "");
            htmlWidget.Replace("{{WidgetId}}", "PageWidgetId_" + pageWidget.Identifier + "_Counter" + counter.ToString());
            htmlWidget.Append("<input type='hidden' id='HiddenYTDRewardPointsGraph_" + page.Identifier + "_" + pageWidget.Identifier + "' value='HiddenYTDRewardPointsGraphValue_" + page.Identifier + "_" + pageWidget.Identifier + "'>");
            return htmlWidget.ToString();
        }

        private string GreenbacksPointsRedeemedYTDGraphWidgetFormatting(PageWidget pageWidget, int counter, Page page)
        {
            var htmlWidget = new StringBuilder(HtmlConstants.NEDBANK_POINTS_REDEEMED_YTD_BAR_GRAPH_WIDGET_HTML);
            htmlWidget.Replace("PointsRedeemedYTDBarGraphcontainer", "PointsRedeemedYTDBarGraphcontainer_" + page.Identifier + "_" + pageWidget.Identifier + "");
            htmlWidget.Replace("{{WidgetId}}", "PageWidgetId_" + pageWidget.Identifier + "_Counter" + counter.ToString());
            htmlWidget.Append("<input type='hidden' id='HiddenPointsRedeemedGraph_" + page.Identifier + "_" + pageWidget.Identifier + "' value='HiddenPointsRedeemedGraphValue_" + page.Identifier + "_" + pageWidget.Identifier + "'>");
            return htmlWidget.ToString();
        }

        private string GreenbacksProductRelatedPointsEarnedGraphWidgetFormatting(PageWidget pageWidget, int counter, Page page)
        {
            var htmlWidget = new StringBuilder(HtmlConstants.NEDBANK_PRODUCT_RELATED_POINTS_EARNED_BAR_GRAPH_WIDGET_HTML);
            htmlWidget.Replace("ProductRelatedPointsEarnedBarGraphcontainer", "ProductRelatedPointsEarnedBarGraphcontainer_" + page.Identifier + "_" + pageWidget.Identifier + "");
            htmlWidget.Replace("{{WidgetId}}", "PageWidgetId_" + pageWidget.Identifier + "_Counter" + counter.ToString());
            htmlWidget.Append("<input type='hidden' id='HiddenProductRelatedPointsEarnedGraph_" + page.Identifier + "_" + pageWidget.Identifier + "' value='HiddenProductRelatedPointsEarnedGraphValue_" + page.Identifier + "_" + pageWidget.Identifier + "'>");
            return htmlWidget.ToString();
        }

        private string GreenbacksCategorySpendPointsGraphWidgetFormatting(PageWidget pageWidget, int counter, Page page)
        {
            var htmlWidget = new StringBuilder(HtmlConstants.NEDBANK_CATEGORY_SPEND_REWARDS_PIE_CHART_WIDGET_HTML);
            htmlWidget.Replace("CategorySpendRewardsPieChartcontainer", "CategorySpendRewardsPieChartcontainer_" + page.Identifier + "_" + pageWidget.Identifier + "");
            htmlWidget.Replace("{{WidgetId}}", "PageWidgetId_" + pageWidget.Identifier + "_Counter" + counter.ToString());
            htmlWidget.Append("<input type='hidden' id='HiddenCategorySpendRewardsGraph_" + page.Identifier + "_" + pageWidget.Identifier + "' value='HiddenCategorySpendRewardsGraphValue_" + page.Identifier + "_" + pageWidget.Identifier + "'>");
            return htmlWidget.ToString();
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
                pageContent.Replace("{{CustomerName}}", customerInfo.FirstName + " " + customerInfo.MiddleName + " " + customerInfo.LastName);
                pageContent.Replace("{{Address1}}", customerInfo.AddressLine1 + ", " + customerInfo.AddressLine2 + ", ");
                pageContent.Replace("{{Address2}}", (customerInfo.City != "" ? customerInfo.City + ", " : "") + (customerInfo.State != "" ? customerInfo.State + ", " : "") + (customerInfo.Country != "" ? customerInfo.Country + ", " : "") + (customerInfo.Zip != "" ? customerInfo.Zip : ""));
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

        private void BindDummyDataToAnalyticsWidget(StringBuilder pageContent, StringBuilder scriptHtmlRenderer, Page page,List<FileData> SampleFiles, string AppBaseDirectory)
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

        #region Bind dummy data to Nedbank sample statement

        private void BindDummyDataToCustomerDetailsWidget(StringBuilder pageContent, Page page, PageWidget widget)
        {
            string jsonstr = "{'TITLE_TEXT': 'MR', 'FIRST_NAME_TEXT':'MATHYS','SURNAME_TEXT':'SMIT','ADDR_LINE_0':'VAN DER MEULENSTRAAT 39','ADDR_LINE_1':'3971 EB DRIEBERGEN', 'ADDR_LINE_2':'NEDERLAND', 'ADDR_LINE_3':'9999', 'ADDR_LINE_4':'', 'MASK_CELL_NO':'******7786'}";
            if (jsonstr != string.Empty && validationEngine.IsValidJson(jsonstr))
            {
                var customerInfo = JsonConvert.DeserializeObject<CustomerInformation>(jsonstr);
                var CustomerDetails = customerInfo.TITLE_TEXT + " " + customerInfo.FIRST_NAME_TEXT + " " + customerInfo.SURNAME_TEXT + "<br>" +
                (!string.IsNullOrEmpty(customerInfo.ADDR_LINE_0) ? (customerInfo.ADDR_LINE_0 + "<br>") : string.Empty) +
                (!string.IsNullOrEmpty(customerInfo.ADDR_LINE_1) ? (customerInfo.ADDR_LINE_1 + "<br>") : string.Empty) +
                (!string.IsNullOrEmpty(customerInfo.ADDR_LINE_2) ? (customerInfo.ADDR_LINE_2 + "<br>") : string.Empty) +
                (!string.IsNullOrEmpty(customerInfo.ADDR_LINE_3) ? (customerInfo.ADDR_LINE_3 + "<br>") : string.Empty) +
                (!string.IsNullOrEmpty(customerInfo.ADDR_LINE_4) ? customerInfo.ADDR_LINE_4 : string.Empty);
                pageContent.Replace("{{CustomerDetails_" + page.Identifier + "_" + widget.Identifier + "}}", CustomerDetails);
                pageContent.Replace("{{MaskCellNo_" + page.Identifier + "_" + widget.Identifier + "}}", "Cell: "+customerInfo.MASK_CELL_NO);
            }
        }

        private void BindDummyDataToBranchDetailsWidget(StringBuilder pageContent, Page page, PageWidget widget, int pagesCount)
        {
            var htmlWidget = new StringBuilder(HtmlConstants.BRANCH_DETAILS_WIDGET_HTML);
            if (page.PageTypeName == HtmlConstants.HOME_LOAN_PAGE_TYPE)
            {
                if (pagesCount == 1)
                {
                    pageContent.Replace("{{BranchDetails_" + page.Identifier + "_" + widget.Identifier + "}}", DateTime.Now.ToString(ModelConstant.DATE_FORMAT_yyyy_MM_dd));
                    htmlWidget.Replace("{{ContactCenter_" + page.Identifier + "_" + widget.Identifier + "}}", "Professional Banking 24/7 Contact centre " + "0860 555 111");
                }
            }
            else
            {
                string jsonstr = "{'BranchName': 'NEDBANK', 'AddressLine0':'Second Floor, Newtown Campus', 'AddressLine1':'141 Lilian Ngoyi Street, Newtown, Johannesburg 2001', 'AddressLine2':'PO Box 1144, Johannesburg, 2000','AddressLine3':'South Africa','VatRegNo':'4320116074','ContactNo':'0860 555 111'}";
                if (jsonstr != string.Empty && validationEngine.IsValidJson(jsonstr))
                {
                    var branchDetails = JsonConvert.DeserializeObject<DM_BranchMaster>(jsonstr);
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
        }

        private void BindDummyDataToInvestmentPortfolioStatementWidget(StringBuilder pageContent, Page page, PageWidget widget)
        {
            string jsonstr = "{'FirstName': 'MATHYS', 'LastName': 'SMIT','Currency': 'R', 'TotalClosingBalance': '23 920.98', 'DayOfStatement':'21', 'InvestorId':'204626','StatementPeriod':'22/12/2020 to 21/01/2021', 'StatementDate':'21/01/2021', 'DsInvestorName' : '' }";
            if (jsonstr != string.Empty && validationEngine.IsValidJson(jsonstr))
            {
                dynamic InvestmentPortfolio = JObject.Parse(jsonstr);
                pageContent.Replace("{{FirstName_" + page.Identifier + "_" + widget.Identifier + "}}", Convert.ToString(InvestmentPortfolio.FirstName));
                pageContent.Replace("{{SurName_" + page.Identifier + "_" + widget.Identifier + "}}", Convert.ToString(InvestmentPortfolio.LastName));
                pageContent.Replace("{{DSName_" + page.Identifier + "_" + widget.Identifier + "}}", Convert.ToString(InvestmentPortfolio.DsInvestorName));
                pageContent.Replace("{{TotalClosingBalance_" + page.Identifier + "_" + widget.Identifier + "}}", (Convert.ToString(InvestmentPortfolio.Currency) + Convert.ToString(InvestmentPortfolio.TotalClosingBalance)));
                pageContent.Replace("{{DayOfStatement_" + page.Identifier + "_" + widget.Identifier + "}}", Convert.ToString(InvestmentPortfolio.DayOfStatement));
                pageContent.Replace("{{InvestorID_" + page.Identifier + "_" + widget.Identifier + "}}", Convert.ToString(InvestmentPortfolio.InvestorId));
                pageContent.Replace("{{StatementPeriod_" + page.Identifier + "_" + widget.Identifier + "}}", Convert.ToString(InvestmentPortfolio.StatementPeriod));
                pageContent.Replace("{{StatementDate_" + page.Identifier + "_" + widget.Identifier + "}}", Convert.ToString(InvestmentPortfolio.StatementDate));
            }
        }

        private void BindDummyDataToInvestorPerformanceWidget(StringBuilder pageContent, Page page, PageWidget widget)
        {
            string jsonstr = "{'Currency': 'R', 'ProductType': 'Notice deposits', 'OpeningBalanceAmount':'23 875.36', 'ClosingBalanceAmount':'23 920.98'}";
            if (jsonstr != string.Empty && validationEngine.IsValidJson(jsonstr))
            {
                dynamic InvestmentPerformance = JObject.Parse(jsonstr);
                pageContent.Replace("{{ProductType_" + page.Identifier + "_" + widget.Identifier + "}}", Convert.ToString(InvestmentPerformance.ProductType));
                pageContent.Replace("{{OpeningBalanceAmount_" + page.Identifier + "_" + widget.Identifier + "}}", (Convert.ToString(InvestmentPerformance.Currency) + Convert.ToString(InvestmentPerformance.OpeningBalanceAmount)));
                pageContent.Replace("{{ClosingBalanceAmount_" + page.Identifier + "_" + widget.Identifier + "}}", (Convert.ToString(InvestmentPerformance.Currency) + Convert.ToString(InvestmentPerformance.ClosingBalanceAmount)));
            }
        }

        private void BindDummyDataToBreakdownOfInvestmentAccountsWidget(StringBuilder pageContent, Page page, PageWidget widget)
        {
            string jsonstr = HtmlConstants.BREAKDOWN_OF_INVESTMENT_ACCOUNTS_WIDGET_PREVIEW_JSON_STRING;

            if (jsonstr != string.Empty && validationEngine.IsValidJson(jsonstr))
            {
                IList<InvestmentAccount> InvestmentAccounts = JsonConvert.DeserializeObject<List<InvestmentAccount>>(jsonstr);

                //Create Nav tab if investment accounts is more than 1
                var NavTabs = new StringBuilder();
                var InvestmentAccountsCount = InvestmentAccounts.Count;
                pageContent.Replace("{{NavTab_" + page.Identifier + "_" + widget.Identifier + "}}", NavTabs.ToString());

                //create tab-content div if accounts is greater than 1, otherwise create simple div
                var TabContentHtml = new StringBuilder();
                var counter = 0;
                TabContentHtml.Append((InvestmentAccountsCount > 1) ? "<div class='tab-content'>" : "");
                InvestmentAccounts.ToList().ForEach(acc =>
                {
                    var InvestmentAccountDetailHtml = new StringBuilder(HtmlConstants.INVESTMENT_ACCOUNT_DETAILS_HTML);
                    InvestmentAccountDetailHtml.Replace("{{ProductDesc}}", acc.ProductDesc);
                    InvestmentAccountDetailHtml.Replace("{{InvestmentId}}", acc.InvestmentId);
                    InvestmentAccountDetailHtml.Replace("{{TabPaneClass}}", "tab-pane fade " + (counter == 0 ? "in active show" : ""));

                    var InvestmentNo = acc.InvestorId + " " + acc.InvestmentId;
                    //actual length is 12, due to space in between investor id and investment id we comparing for 13 characters
                    while (InvestmentNo.Length != 13)
                    {
                        InvestmentNo = "0" + InvestmentNo;
                    }
                    InvestmentAccountDetailHtml.Replace("{{InvestmentNo}}", InvestmentNo);
                    InvestmentAccountDetailHtml.Replace("{{AccountOpenDate}}", acc.OpenDate);

                    InvestmentAccountDetailHtml.Replace("{{AccountOpenDate}}", acc.OpenDate);
                    InvestmentAccountDetailHtml.Replace("{{InterestRate}}", acc.CurrentInterestRate + "% pa");
                    InvestmentAccountDetailHtml.Replace("{{MaturityDate}}", acc.ExpiryDate);
                    InvestmentAccountDetailHtml.Replace("{{InterestDisposal}}", acc.InterestDisposalDesc);
                    InvestmentAccountDetailHtml.Replace("{{NoticePeriod}}", acc.NoticePeriod);
                    InvestmentAccountDetailHtml.Replace("{{InterestDue}}", acc.Currency + acc.AccuredInterest);

                    InvestmentAccountDetailHtml.Replace("{{LastTransactionDate}}", "25 November 2020");
                    InvestmentAccountDetailHtml.Replace("{{BalanceOfLastTransactionDate}}", acc.Currency + (counter == 0 ? "5 307.14" : "18 613.84"));

                    var InvestmentTransactionRows = new StringBuilder();
                    acc.Transactions.ForEach(trans =>
                    {
                        var tr = new StringBuilder();
                        tr.Append("<tr class='ht-20'>");
                        tr.Append("<td class='w-15 pt-1'>" + trans.TransactionDate + "</td>");
                        tr.Append("<td class='w-40 pt-1'>" + trans.TransactionDesc + "</td>");
                        tr.Append("<td class='w-15 text-right pt-1'>" + (trans.Debit == "0" ? "-" : acc.Currency + trans.Debit) + "</td>");
                        tr.Append("<td class='w-15 text-right pt-1'>" + (trans.Credit == "0" ? "-" : acc.Currency + trans.Credit) + "</td>");
                        tr.Append("<td class='w-15 text-right pt-1'>" + (trans.Balance == "0" ? "-" : acc.Currency + trans.Balance) + "</td>");
                        tr.Append("</tr>");
                        InvestmentTransactionRows.Append(tr.ToString());
                    });
                    InvestmentAccountDetailHtml.Replace("{{InvestmentTransactionRows}}", InvestmentTransactionRows.ToString());
                    TabContentHtml.Append(InvestmentAccountDetailHtml.ToString());
                    counter++;
                });
                TabContentHtml.Append((InvestmentAccountsCount > 1) ? "</div>" : "");
                pageContent.Replace("{{TabContentsDiv_" + page.Identifier + "_" + widget.Identifier + "}}", TabContentHtml.ToString());
            }
        }

        private void BindDummyDataToExplanatoryNotesWidget(StringBuilder pageContent, Page page, PageWidget widget)
        {
            string jsonstr = "{'Note1': 'Fixed deposits — Total balance of all your fixed-type accounts.', 'Note2': 'Notice deposits — Total balance of all your notice deposit accounts.', 'Note3':'Linked deposits — Total balance of all your linked-type accounts.'}";
            if (jsonstr != string.Empty && validationEngine.IsValidJson(jsonstr))
            {
                dynamic noteObj = JObject.Parse(jsonstr);
                var notes = new StringBuilder();
                notes.Append("<span> " + Convert.ToString(noteObj.Note1) + " </span> <br/>");
                notes.Append("<span> " + Convert.ToString(noteObj.Note2) + " </span> <br/>");
                notes.Append("<span> " + Convert.ToString(noteObj.Note3) + " </span> ");
                pageContent.Replace("{{Notes_" + page.Identifier + "_" + widget.Identifier + "}}", notes.ToString());
            }
        }

        private void BindDummyDataToMarketingServiceMessageWidget(StringBuilder pageContent, string MarketingMessages, Page page, PageWidget widget, int MarketingMessageCounter)
        {
            if (MarketingMessages != string.Empty && validationEngine.IsValidJson(MarketingMessages))
            {
                IList<MarketingMessage> _lstMarketingMessage = JsonConvert.DeserializeObject<List<MarketingMessage>>(MarketingMessages);
                var ServiceMessage = _lstMarketingMessage[MarketingMessageCounter];
                if (ServiceMessage != null)
                {
                    var messageTxt = ((!string.IsNullOrEmpty(ServiceMessage.MarketingMessageText1)) ? "<p>" + ServiceMessage.MarketingMessageText1 + "</p>" : string.Empty) + ((!string.IsNullOrEmpty(ServiceMessage.MarketingMessageText2)) ? "<p>" + ServiceMessage.MarketingMessageText2 + "</p>" : string.Empty) + ((!string.IsNullOrEmpty(ServiceMessage.MarketingMessageText3)) ? "<p>" + ServiceMessage.MarketingMessageText3 + "</p>" : string.Empty) + ((!string.IsNullOrEmpty(ServiceMessage.MarketingMessageText4)) ? "<p>" + ServiceMessage.MarketingMessageText4 + "</p>" : string.Empty) + ((!string.IsNullOrEmpty(ServiceMessage.MarketingMessageText5)) ? "<p>" + ServiceMessage.MarketingMessageText5 + "</p>" : string.Empty);

                    pageContent.Replace("{{ServiceMessageHeader_" + page.Identifier + "_" + widget.Identifier + "_" + MarketingMessageCounter + "}}", ServiceMessage.MarketingMessageHeader).Replace("{{ServiceMessageText_" + page.Identifier + "_" + widget.Identifier + "_" + MarketingMessageCounter + "}}", messageTxt);
                }
            }
        }

        private void BindDummyDataToPersonalLoanDetailWidget(StringBuilder pageContent, Page page, PageWidget widget)
        {
            string jsonstr = "{'Identifier': 1,'CustomerId': 211135146504,'InvestorId': 8004334234001,'Currency': 'R','ProductType': 'PersonalLoan','BranchId': 1,'CreditAdvance': '75372', 'OutstandingBalance': '68169','AmountDue': '3297','ToDate': '2021-02-28 00:00:00','FromDate': '2020-12-01 00:00:00','MonthlyInstallment': '3297','DueDate': '2021-03-31 00:00:00','Arrears': '0','AnnualRate': '24','Term': '36'}";
            if (jsonstr != string.Empty && validationEngine.IsValidJson(jsonstr))
            {
                var PersonalLoan = JsonConvert.DeserializeObject<DM_PersonalLoanMaster>(jsonstr);

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

        private void BindDummyDataToPersonalLoanTransactionWidget(StringBuilder pageContent, Page page, PageWidget widget)
        {
            string jsonstr = HtmlConstants.PERSONAL_LOAN_TRANSACTION_PREVIEW_JSON_STRING;
            if (jsonstr != string.Empty && validationEngine.IsValidJson(jsonstr))
            {
                var transactions = JsonConvert.DeserializeObject<List<DM_PersonalLoanTransaction>>(jsonstr);
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
        }

        private void BindDummyDataToPersonalLoanPaymentDueWidget(StringBuilder pageContent, Page page, PageWidget widget)
        {
            string jsonstr = "{'Identifier': '1','CustomerId': '211135146504','InvestorId': '8004334234001','Arrears_120': '0','Arrears_90': '0','Arrears_60': '0','Arrears_30': '0','Arrears_0': '3297'}";
            if (jsonstr != string.Empty && validationEngine.IsValidJson(jsonstr))
            {
                var plArrears = JsonConvert.DeserializeObject<DM_PersonalLoanArrears>(jsonstr);
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
            }
        }

        private void BindDummyDataToSpecialMessageWidget(StringBuilder pageContent, Page page, PageWidget widget)
        {
            string jsonstr = string.Empty;
            if (page.PageTypeName == HtmlConstants.HOME_LOAN_PAGE_TYPE)
            {
                jsonstr = HtmlConstants.HOME_LOAN_SPECIAL_MESSAGES_WIDGET_PREVIEW_JSON_STRING;
            }
            else
            {
                jsonstr = HtmlConstants.SPECIAL_MESSAGES_WIDGET_PREVIEW_JSON_STRING;
            }

            if (jsonstr != string.Empty && validationEngine.IsValidJson(jsonstr))
            {
                var SpecialMessage = JsonConvert.DeserializeObject<SpecialMessage>(jsonstr);
                if (SpecialMessage != null)
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

        private void BindDummmyDataToPersonalLoanInsuranceMessageWidget(StringBuilder pageContent, Page page, PageWidget widget)
        {
            string jsonstr = HtmlConstants.SPECIAL_MESSAGES_WIDGET_PREVIEW_JSON_STRING;
            if (jsonstr != string.Empty && validationEngine.IsValidJson(jsonstr))
            {
                var InsuranceMsg = JsonConvert.DeserializeObject<SpecialMessage>(jsonstr);
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
        }

        private void BindDummyDataToPersonalLoanTotalAmountDetailWidget(StringBuilder pageContent, Page page, PageWidget widget)
        {
            string jsonstr = HtmlConstants.PERSONAL_LOAN_ACCOUNTS_PREVIEW_JSON_STRING;
            var TotalLoanAmt = 0.0m;
            var TotalOutstandingAmt = 0.0m;
            var TotalLoanDueAmt = 0.0m;

            if (jsonstr != string.Empty && validationEngine.IsValidJson(jsonstr))
            {
                var PersonalLoans = JsonConvert.DeserializeObject<List<DM_PersonalLoanMaster>>(jsonstr);
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
            }

            pageContent.Replace("{{TotalLoanAmount_" + page.Identifier + "_" + widget.Identifier + "}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, TotalLoanAmt));
            pageContent.Replace("{{OutstandingBalance_" + page.Identifier + "_" + widget.Identifier + "}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, TotalOutstandingAmt));
            pageContent.Replace("{{DueAmount_" + page.Identifier + "_" + widget.Identifier + "}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, TotalLoanDueAmt));
        }

        private void BindDummyDataToPersonalLoanAccountsBreakdownWidget(StringBuilder pageContent, Page page, PageWidget widget)
        {
            string jsonstr = HtmlConstants.PERSONAL_LOAN_ACCOUNTS_PREVIEW_JSON_STRING;
            if (jsonstr != string.Empty && validationEngine.IsValidJson(jsonstr))
            {
                var PersonalLoans = JsonConvert.DeserializeObject<List<DM_PersonalLoanMaster>>(jsonstr);
                if (PersonalLoans != null && PersonalLoans.Count > 0)
                {
                    //Create Nav tab if customer has more than 1 personal loan accounts
                    var NavTabs = new StringBuilder();
                    if (PersonalLoans.Count > 1)
                    {
                        NavTabs.Append("<ul class='nav nav-tabs Personalloan-nav-tabs'>");
                        var cnt = 0;
                        PersonalLoans.ToList().ForEach(acc =>
                        {
                            var AccountNumber = acc.InvestorId.ToString();
                            string lastFourDigisOfAccountNumber = AccountNumber.Length > 4 ? AccountNumber.Substring(Math.Max(0, AccountNumber.Length - 4)) : AccountNumber;
                            NavTabs.Append("<li class='nav-item " + (cnt == 0 ? "active" : string.Empty) + "'><a id='tab0-tab' data-toggle='tab' data-target='#PersonalLoan-" + lastFourDigisOfAccountNumber + "' role='tab' class='nav-link " + (cnt == 0 ? "active" : string.Empty) + "'> Personal Loan - " + lastFourDigisOfAccountNumber + "</a></li>");
                            cnt++;
                        });
                        NavTabs.Append("</ul>");
                    }
                    pageContent.Replace("{{NavTab_" + page.Identifier + "_" + widget.Identifier + "}}", NavTabs.ToString());

                    //create tab-content div if accounts is greater than 1, otherwise create simple div
                    var TabContentHtml = new StringBuilder();
                    var counter = 0;
                    TabContentHtml.Append((PersonalLoans.Count > 1) ? "<div class='tab-content'>" : string.Empty);
                    PersonalLoans.ForEach(PersonalLoan =>
                    {
                        string lastFourDigisOfAccountNumber = PersonalLoan.InvestorId.ToString().Length > 4 ? PersonalLoan.InvestorId.ToString().Substring(Math.Max(0, PersonalLoan.InvestorId.ToString().Length - 4)) : PersonalLoan.InvestorId.ToString();

                        TabContentHtml.Append("<div id='PersonalLoan-" + lastFourDigisOfAccountNumber + "' class='tab-pane fade " + (counter == 0 ? "in active show" : string.Empty) + "'>");

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
                            var LoanTransactionDetailHtml = new StringBuilder(HtmlConstants.PERSONAL_LOAN_ACCOUNT_TRANSACTION_DETAIL);
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
                        TabContentHtml.Append(HtmlConstants.END_DIV_TAG);
                        counter++;
                    });

                    TabContentHtml.Append((PersonalLoans.Count > 1) ? HtmlConstants.END_DIV_TAG : string.Empty);
                    pageContent.Replace("{{TabContentsDiv_" + page.Identifier + "_" + widget.Identifier + "}}", TabContentHtml.ToString());
                }
            }
        }

        private void BindDummyDataToHomeLoanTotalAmountDetailWidget(StringBuilder pageContent, Page page, PageWidget widget)
        {
            string jsonstr = HtmlConstants.HOME_LOAN_ACCOUNTS_PREVIEW_JSON_STRING;
            var TotalLoanAmt = 0.0m;
            var TotalOutstandingAmt = 0.0m;

            if (jsonstr != string.Empty && validationEngine.IsValidJson(jsonstr))
            {
                var HomeLoans = JsonConvert.DeserializeObject<List<DM_HomeLoanMaster>>(jsonstr);
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
        }

        private void BindDummyDataToHomeLoanAccountsBreakdownWidget(StringBuilder pageContent, Page page, PageWidget widget)
        {
            string jsonstr = HtmlConstants.HOME_LOAN_ACCOUNTS_PREVIEW_JSON_STRING;
            if (jsonstr != string.Empty && validationEngine.IsValidJson(jsonstr))
            {
                var HomeLoans = JsonConvert.DeserializeObject<List<DM_HomeLoanMaster>>(jsonstr);
                if (HomeLoans != null && HomeLoans.Count > 0)
                {
                    //Create Nav tab if customer has more than 1 personal loan accounts
                    var NavTabs = new StringBuilder();
                    if (HomeLoans.Count > 1)
                    {
                        NavTabs.Append("<ul class='nav nav-tabs Homeloan-nav-tabs'>");
                        var cnt = 0;
                        HomeLoans.ToList().ForEach(acc =>
                        {
                            var accNo = acc.InvestorId.ToString();
                            string lastFourDigisOfAccountNumber = accNo.Length > 4 ? accNo.Substring(Math.Max(0, accNo.Length - 4)) : accNo;
                            NavTabs.Append("<li class='nav-item " + (cnt == 0 ? "active" : string.Empty) + "'><a id='tab0-tab' data-toggle='tab' data-target='#HomeLoan-" + lastFourDigisOfAccountNumber + "' role='tab' class='nav-link " + (cnt == 0 ? "active" : string.Empty) + "'> Home Loan - " + lastFourDigisOfAccountNumber + "</a></li>");
                            cnt++;
                        });
                        NavTabs.Append("</ul>");
                    }
                    pageContent.Replace("{{NavTab_" + page.Identifier + "_" + widget.Identifier + "}}", NavTabs.ToString());

                    //create tab-content div if accounts is greater than 1, otherwise create simple div
                    var TabContentHtml = new StringBuilder();
                    var counter = 0;
                    TabContentHtml.Append((HomeLoans.Count > 1) ? "<div class='tab-content'>" : string.Empty);
                    HomeLoans.ForEach(HomeLoan =>
                    {
                        var accNo = HomeLoan.InvestorId.ToString();
                        string lastFourDigisOfAccountNumber = accNo.Length > 4 ? accNo.Substring(Math.Max(0, accNo.Length - 4)) : accNo;

                        TabContentHtml.Append("<div id='HomeLoan-" + lastFourDigisOfAccountNumber + "' class='tab-pane fade " + (counter == 0 ? "in active show" : string.Empty) + "'>");

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
                        var LoanTransactionDetailHtml = new StringBuilder(HtmlConstants.HOME_LOAN_TRANSACTION_DETAIL_DIV_HTML);

                        var tr = new StringBuilder();
                        if (HomeLoan.LoanTransactions != null && HomeLoan.LoanTransactions.Count > 0)
                        {
                            HomeLoan.LoanTransactions.ForEach(trans =>
                            {
                                tr = new StringBuilder();
                                tr.Append("<tr class='ht-20'>");
                                tr.Append("<td class='w-13 text-center'> " + trans.Posting_date.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy) + " </td>");
                                tr.Append("<td class='w-15 text-center'> " + trans.Effective_date.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy) + " </td>");
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

                        var PaymentDueMessageDivHtml = new StringBuilder(HtmlConstants.HOME_LAON_PAYMENT_DUE_SPECIAL_MESSAGE_DIV_HTML);
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

                        var LoanSummaryForTaxPurposesHtml = new StringBuilder(HtmlConstants.HOME_LOAN_SERVICE_FOR_TAX_PURPOSES_DIV_HTML);
                        var LoanInstalmentHtml = new StringBuilder(HtmlConstants.HOME_LOAN_INSTALMENT_DETAILS_DIV_HTML);
                        var HomeLoanSummary = HomeLoans[0].LoanSummary;
                        if (HomeLoanSummary != null)
                        {
                            #region Summary for Tax purposes div
                            res = 0.0m;
                            if (!string.IsNullOrEmpty(HomeLoanSummary.Annual_Interest) && decimal.TryParse(HomeLoanSummary.Annual_Interest, out res))
                            {
                                LoanSummaryForTaxPurposesHtml.Replace("{{AnnualInterest}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                            }
                            else
                            {
                                LoanSummaryForTaxPurposesHtml.Replace("{{AnnualInterest}}", "R0.00");
                            }

                            res = 0.0m;
                            if (!string.IsNullOrEmpty(HomeLoanSummary.Annual_Insurance) && decimal.TryParse(HomeLoanSummary.Annual_Insurance, out res))
                            {
                                LoanSummaryForTaxPurposesHtml.Replace("{{AnnualInsurance}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                            }
                            else
                            {
                                LoanSummaryForTaxPurposesHtml.Replace("{{AnnualInsurance}}", "R0.00");
                            }

                            res = 0.0m;
                            if (!string.IsNullOrEmpty(HomeLoanSummary.Annual_Service_Fee) && decimal.TryParse(HomeLoanSummary.Annual_Service_Fee, out res))
                            {
                                LoanSummaryForTaxPurposesHtml.Replace("{{AnnualServiceFee}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                            }
                            else
                            {
                                LoanSummaryForTaxPurposesHtml.Replace("{{AnnualServiceFee}}", "R0.00");
                            }

                            res = 0.0m;
                            if (!string.IsNullOrEmpty(HomeLoanSummary.Annual_Legal_Costs) && decimal.TryParse(HomeLoanSummary.Annual_Legal_Costs, out res))
                            {
                                LoanSummaryForTaxPurposesHtml.Replace("{{AnnualLegalCosts}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                            }
                            else
                            {
                                LoanSummaryForTaxPurposesHtml.Replace("{{AnnualLegalCosts}}", "R0.00");
                            }

                            res = 0.0m;
                            if (!string.IsNullOrEmpty(HomeLoanSummary.Annual_Total_Recvd) && decimal.TryParse(HomeLoanSummary.Annual_Total_Recvd, out res))
                            {
                                LoanSummaryForTaxPurposesHtml.Replace("{{AnnualTotalAmountReceived}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                            }
                            else
                            {
                                LoanSummaryForTaxPurposesHtml.Replace("{{AnnualTotalAmountReceived}}", "R0.00");
                            }

                            #endregion

                            #region Installment details div

                            res = 0.0m;
                            if (!string.IsNullOrEmpty(HomeLoanSummary.Basic_Instalment) && decimal.TryParse(HomeLoanSummary.Basic_Instalment, out res))
                            {
                                LoanInstalmentHtml.Replace("{{BasicInstalment}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                            }
                            else
                            {
                                LoanInstalmentHtml.Replace("{{BasicInstalment}}", "R0.00");
                            }

                            res = 0.0m;
                            if (!string.IsNullOrEmpty(HomeLoanSummary.Houseowner_Ins) && decimal.TryParse(HomeLoanSummary.Houseowner_Ins, out res))
                            {
                                LoanInstalmentHtml.Replace("{{HouseownerInsurance}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                            }
                            else
                            {
                                LoanInstalmentHtml.Replace("{{HouseownerInsurance}}", "R0.00");
                            }

                            res = 0.0m;
                            if (!string.IsNullOrEmpty(HomeLoanSummary.Loan_Protection) && decimal.TryParse(HomeLoanSummary.Loan_Protection, out res))
                            {
                                LoanInstalmentHtml.Replace("{{LoanProtectionAssurance}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                            }
                            else
                            {
                                LoanInstalmentHtml.Replace("{{LoanProtectionAssurance}}", "R0.00");
                            }

                            res = 0.0m;
                            if (!string.IsNullOrEmpty(HomeLoanSummary.Recovery_Fee_Debit) && decimal.TryParse(HomeLoanSummary.Recovery_Fee_Debit, out res))
                            {
                                LoanInstalmentHtml.Replace("{{RecoveryOfFeeDebits}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                            }
                            else
                            {
                                LoanInstalmentHtml.Replace("{{RecoveryOfFeeDebits}}", "R0.00");
                            }

                            res = 0.0m;
                            if (!string.IsNullOrEmpty(HomeLoanSummary.Capital_Redemption) && decimal.TryParse(HomeLoanSummary.Capital_Redemption, out res))
                            {
                                LoanInstalmentHtml.Replace("{{CapitalRedemption}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                            }
                            else
                            {
                                LoanInstalmentHtml.Replace("{{CapitalRedemption}}", "R0.00");
                            }

                            res = 0.0m;
                            if (!string.IsNullOrEmpty(HomeLoanSummary.Service_Fee) && decimal.TryParse(HomeLoanSummary.Service_Fee, out res))
                            {
                                LoanInstalmentHtml.Replace("{{ServiceFee}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                            }
                            else
                            {
                                LoanInstalmentHtml.Replace("{{ServiceFee}}", "R0.00");
                            }

                            res = 0.0m;
                            if (!string.IsNullOrEmpty(HomeLoanSummary.Total_Instalment) && decimal.TryParse(HomeLoanSummary.Total_Instalment, out res))
                            {
                                LoanInstalmentHtml.Replace("{{TotalInstalment}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                            }
                            else
                            {
                                LoanInstalmentHtml.Replace("{{TotalInstalment}}", "R0.00");
                            }

                            LoanInstalmentHtml.Replace("{{InstalmentDate}}", DateTime.Now.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy));

                            #endregion
                        }
                        else
                        {
                            LoanSummaryForTaxPurposesHtml.Replace("{{AnnualInterest}}", "R0.00");
                            LoanSummaryForTaxPurposesHtml.Replace("{{AnnualInsurance}}", "R0.00");
                            LoanSummaryForTaxPurposesHtml.Replace("{{AnnualServiceFee}}", "R0.00");
                            LoanSummaryForTaxPurposesHtml.Replace("{{AnnualLegalCosts}}", "R0.00");
                            LoanSummaryForTaxPurposesHtml.Replace("{{AnnualTotalAmountReceived}}", "R0.00");

                            LoanInstalmentHtml.Replace("{{BasicInstalment}}", "R0.00");
                            LoanInstalmentHtml.Replace("{{HouseownerInsurance}}", "R0.00");
                            LoanInstalmentHtml.Replace("{{LoanProtectionAssurance}}", "R0.00");
                            LoanInstalmentHtml.Replace("{{RecoveryOfFeeDebits}}", "R0.00");
                            LoanInstalmentHtml.Replace("{{CapitalRedemption}}", "R0.00");
                            LoanInstalmentHtml.Replace("{{ServiceFee}}", "R0.00");
                            LoanInstalmentHtml.Replace("{{TotalInstalment}}", "R0.00");
                            LoanInstalmentHtml.Replace("{{InstalmentDate}}", DateTime.Now.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy));
                        }

                        TabContentHtml.Append(LoanSummaryForTaxPurposesHtml.ToString());
                        TabContentHtml.Append(LoanInstalmentHtml.ToString());

                        TabContentHtml.Append(HtmlConstants.END_DIV_TAG);
                        counter++;
                    });

                    TabContentHtml.Append((HomeLoans.Count > 1) ? HtmlConstants.END_DIV_TAG : string.Empty);
                    pageContent.Replace("{{TabContentsDiv_" + page.Identifier + "_" + widget.Identifier + "}}", TabContentHtml.ToString());
                }
            }
        }

        private void BindDummyDataToPortfolioCustomerDetailsWidget(StringBuilder pageContent, Page page, PageWidget widget)
        {
            string jsonstr = "{'CustomerId': 171001255307, 'Title': 'MR.', 'FirstName':'MATHYS','SurName':'NKHUMISE','AddressLine0':'VERDEAU LIFESTYLE ESTATE', 'AddressLine1':'6 HERCULE CRESCENT DRIVE','AddressLine2':'WELLINGTON','AddressLine3':'7655','AddressLine4':'', 'Mask_Cell_No': '+2367 345 786', 'EmailAddress' : 'mknumise@domain.com'}";
            if (jsonstr != string.Empty && validationEngine.IsValidJson(jsonstr))
            {
                var customer = JsonConvert.DeserializeObject<DM_CustomerMaster>(jsonstr);
                pageContent.Replace("{{CustomerName_" + page.Identifier + "_" + widget.Identifier + "}}", (customer.Title + " " + customer.FirstName + " " + customer.SurName));
                pageContent.Replace("{{CustomerId_" + page.Identifier + "_" + widget.Identifier + "}}", customer.CustomerId.ToString());
                pageContent.Replace("{{MobileNumber_" + page.Identifier + "_" + widget.Identifier + "}}", customer.Mask_Cell_No);
                pageContent.Replace("{{EmailAddress_" + page.Identifier + "_" + widget.Identifier + "}}", customer.EmailAddress);
            }
        }

        private void BindDummyDataToPortfolioCustomerAddressDetailsWidget(StringBuilder pageContent, Page page, PageWidget widget)
        {
            string jsonstr = "{'CustomerId': 171001255307, 'Title': 'MR.', 'FirstName':'MATHYS','SurName':'NKHUMISE','AddressLine0':'VERDEAU LIFESTYLE ESTATE', 'AddressLine1':'6 HERCULE CRESCENT DRIVE','AddressLine2':'WELLINGTON','AddressLine3':'7655','AddressLine4':'', 'Mask_Cell_No': '+2367 345 786', 'EmailAddress' : 'mknumise@domain.com'}";
            if (jsonstr != string.Empty && validationEngine.IsValidJson(jsonstr))
            {
                var customer = JsonConvert.DeserializeObject<DM_CustomerMaster>(jsonstr);
                var custAddress = (!string.IsNullOrEmpty(customer.AddressLine0) ? (customer.AddressLine0 + "<br>") : string.Empty) +
                                (!string.IsNullOrEmpty(customer.AddressLine1) ? (customer.AddressLine1 + "<br>") : string.Empty) +
                                (!string.IsNullOrEmpty(customer.AddressLine2) ? (customer.AddressLine2 + "<br>") : string.Empty) +
                                (!string.IsNullOrEmpty(customer.AddressLine3) ? (customer.AddressLine3 + "<br>") : string.Empty) +
                                (!string.IsNullOrEmpty(customer.AddressLine4) ? customer.AddressLine4 : string.Empty);
                pageContent.Replace("{{CustomerAddress_" + page.Identifier + "_" + widget.Identifier + "}}", custAddress);
            }
        }

        private void BindDummyDataToPortfolioClientContactDetailsWidget(StringBuilder pageContent, Page page, PageWidget widget)
        {
            pageContent.Replace("{{MobileNumber_" + page.Identifier + "_" + widget.Identifier + "}}", "0860 555 111");
            pageContent.Replace("{{EmailAddress_" + page.Identifier + "_" + widget.Identifier + "}}", "supportdesk@nedbank.com");
        }

        private void BindDummyDataToPortfolioAccountSummaryDetailsWidget(StringBuilder pageContent, Page page, PageWidget widget)
        {
            string jsonstr = "[{'AccountType': 'Investment', 'TotalCurrentAmount': 'R9 620.98'}, {'AccountType': 'Personal Loan', 'TotalCurrentAmount': 'R4 165.00'}, {'AccountType': 'Home Loan', 'TotalCurrentAmount': 'R7 969.00'}]";
            if (jsonstr != string.Empty && validationEngine.IsValidJson(jsonstr))
            {
                var _lstAccounts = JsonConvert.DeserializeObject<List<DM_CustomerAccountSummary>>(jsonstr);
                if (_lstAccounts.Count > 0)
                {
                    var accountSummaryRows = new StringBuilder();
                    _lstAccounts.ForEach(acc =>
                    {
                        var tr = new StringBuilder();
                        tr.Append("<tr class='ht-30'>");
                        tr.Append("<td class='text-left'>" + acc.AccountType + " </td>");
                        tr.Append("<td class='text-right'>" + acc.TotalCurrentAmount + " </td>");
                        tr.Append("</tr>");
                        accountSummaryRows.Append(tr.ToString());
                    });
                    pageContent.Replace("{{AccountSummaryRows_" + page.Identifier + "_" + widget.Identifier + "}}", accountSummaryRows.ToString());
                }
                else
                {
                    pageContent.Replace("{{AccountSummaryRows_" + page.Identifier + "_" + widget.Identifier + "}}", "<tr class='ht-30'><td class='text-center' colspan='2'> No records found </td></tr>");
                }
            }

            var rewardPointsDiv = new StringBuilder("<div class='pt-2'><table class='LoanTransactionTable customTable'><thead><tr class='ht-30'><th class='text-left'>{{RewardType}} </th><th class='text-right'>{{RewardPoints}}</th></tr></thead></table></div>");
            rewardPointsDiv.Replace("{{RewardType}}", "Greenbacks rewards points");
            rewardPointsDiv.Replace("{{RewardPoints}}", "234");
            pageContent.Replace("{{RewardPointsDiv_" + page.Identifier + "_" + widget.Identifier + "}}", rewardPointsDiv.ToString());
        }

        private void BindDummyDataToPortfolioAccountAnalysisGraphWidget(StringBuilder pageContent, StringBuilder scriptHtmlRenderer, Page page, PageWidget widget)
        {
            var data = "[{\"AccountType\": \"Investment\",\"MonthwiseAmount\" : [{\"Month\": \"Jan\", \"Amount\": 9456.12}, {\"Month\": \"Feb\", \"Amount\": 9620.98}]},{\"AccountType\": \"Personal Loan\",\"MonthwiseAmount\" : [{\"Month\": \"Jan\", \"Amount\": -4465.00}, {\"Month\": \"Feb\", \"Amount\": -4165.00}]},{\"AccountType\": \"Home Loan\",\"MonthwiseAmount\" : [{\"Month\": \"Jan\", \"Amount\": -8969.00}, {\"Month\": \"Feb\", \"Amount\": -7969.00}]}]";
            pageContent.Replace("HiddenAccountAnalysisGraphValue_" + page.Identifier + "_" + widget.Identifier + "", data);
            scriptHtmlRenderer.Append(HtmlConstants.PORTFOLIO_ACCOUNT_ANALYSIS_BAR_GRAPH_SCRIPT.Replace("AccountAnalysisBarGraphcontainer", "AccountAnalysisBarGraphcontainer_" + page.Identifier + "_" + widget.Identifier).Replace("HiddenAccountAnalysisGraph", "HiddenAccountAnalysisGraph_" + page.Identifier + "_" + widget.Identifier));
        }

        private void BindDummyDataToPortfolioReimndersWidget(StringBuilder pageContent, Page page, PageWidget widget)
        {
            string jsonstr = "[{ 'Title': 'Update Missing Inofrmation', 'Action': 'Update' },{ 'Title': 'Your Rewards Video is available', 'Action': 'View' },{ 'Title': 'Payment Due for Home Loan', 'Action': 'Pay' }, { title: 'Need financial planning for savings.', action: 'Call Me' },{ title: 'Subscribe/Unsubscribe Alerts.', action: 'Apply' },{ title: 'Your credit card payment is due now.', action: 'Pay' }]";
            if (jsonstr != string.Empty && validationEngine.IsValidJson(jsonstr))
            {
                IList<ReminderAndRecommendation> reminderAndRecommendations = JsonConvert.DeserializeObject<List<ReminderAndRecommendation>>(jsonstr);
                StringBuilder reminderstr = new StringBuilder();
                reminderAndRecommendations.ToList().ForEach(item =>
                {
                    reminderstr.Append("<div class='row'><div class='col-lg-9 text-left'><p class='p-1' style='background-color: #dce3dc;'>" + item.Title + " </p></div><div class='col-lg-3 text-left'><a href='javascript:void(0)' target='_blank'><i class='fa fa-caret-left fa-3x float-left text-success'></i><span class='mt-2 d-inline-block ml-2'>" + item.Action + "</span></a></div></div>");
                });
                pageContent.Replace("{{ReminderAndRecommendation_" + page.Identifier + "_" + widget.Identifier + "}}", reminderstr.ToString());
            }
        }

        private void BindDummyDataToPortfolioNewsAlertsWidget(StringBuilder pageContent, Page page, PageWidget widget)
        {
            string jsonstr = "{ \"Message1\": \"Covid 19 and the subsequent lockdown has affected all areas of our daily lives. The way we work, the way we bank and how we interact with each other.\", \"Message2\": \"We want you to know we are in this together. That's why we are sharing advice, tips and news updates with you on ways to bank as well as ways to keep yorself and your loved ones safe.\", \"Message3\": \"We would like to remind you of the credit life insurance benefits available to you through your Nedbank Insurance policy. When you pass away, Nedbank Insurance will cover your outstanding loan amount. If you are permanently employed, you will also enjoy cover for comprehensive disability and loss of income. The disability benefit will cover your monthly instalments if you cannot earn your usual income due to illness or bodily injury.\", \"Message4\": \"\", \"Message5\": \"\" }";
            if (jsonstr != string.Empty && validationEngine.IsValidJson(jsonstr))
            {
                var newsAlert = JsonConvert.DeserializeObject<NewsAlert>(jsonstr);
                var newsAlertStr = (!string.IsNullOrEmpty(newsAlert.Message1) ? ("<p>" + newsAlert.Message1 + "</p>") : string.Empty) +
                    (!string.IsNullOrEmpty(newsAlert.Message2) ? ("<p>" + newsAlert.Message2 + "</p>") : string.Empty) +
                    (!string.IsNullOrEmpty(newsAlert.Message3) ? ("<p>" + newsAlert.Message3 + "</p>") : string.Empty) +
                    (!string.IsNullOrEmpty(newsAlert.Message4) ? ("<p>" + newsAlert.Message4 + "</p>") : string.Empty) +
                    (!string.IsNullOrEmpty(newsAlert.Message5) ? ("<p>" + newsAlert.Message5 + "</p>") : string.Empty);
                pageContent.Replace("{{NewsAlert_" + page.Identifier + "_" + widget.Identifier + "}}", newsAlertStr);
            }
        }

        private void BindDummyDataToGreenbacksTotalRewardPointsWidget(StringBuilder pageContent, Page page, PageWidget widget)
        {
            pageContent.Replace("{{TotalRewardsPoints_" + page.Identifier + "_" + widget.Identifier + "}}", "482");
        }

        private void BindDummyDataToGreenbacksYtdRewardsPointsGraphWidget(StringBuilder pageContent, StringBuilder scriptHtmlRenderer, Page page, PageWidget widget)
        {
            var data = "[{\"Month\": \"Jan\",\"RewardPoint\" : 98}, {\"Month\": \"Feb\",\"RewardPoint\" : 112}, {\"Month\": \"Mar\",\"RewardPoint\" : 128}, {\"Month\": \"Apr\",\"RewardPoint\" : 144}]";
            pageContent.Replace("HiddenYTDRewardPointsGraphValue_" + page.Identifier + "_" + widget.Identifier + "", data);
            scriptHtmlRenderer.Append(HtmlConstants.GREENBACKS_YTD_REWARDS_POINTS_BAR_GRAPH_SCRIPT.Replace("YTDRewardPointsBarGraphcontainer", "YTDRewardPointsBarGraphcontainer_" + page.Identifier + "_" + widget.Identifier).Replace("HiddenYTDRewardPointsGraph", "HiddenYTDRewardPointsGraph_" + page.Identifier + "_" + widget.Identifier));
        }

        private void BindDummyDataToGreenbacksPointsRedeemedYtdGraphWidget(StringBuilder pageContent, StringBuilder scriptHtmlRenderer, Page page, PageWidget widget)
        {
            var data = "[{\"Month\": \"Jan\",\"RedeemedPoints\" : 58}, {\"Month\": \"Feb\",\"RedeemedPoints\" : 71}, {\"Month\": \"Mar\",\"RedeemedPoints\" : 63}, {\"Month\": \"Apr\",\"RedeemedPoints\" : 84}]";
            pageContent.Replace("HiddenPointsRedeemedGraphValue_" + page.Identifier + "_" + widget.Identifier + "", data);
            scriptHtmlRenderer.Append(HtmlConstants.GREENBACKS_POINTS_REDEEMED_YTD_BAR_GRAPH_SCRIPT.Replace("PointsRedeemedYTDBarGraphcontainer", "PointsRedeemedYTDBarGraphcontainer_" + page.Identifier + "_" + widget.Identifier).Replace("HiddenPointsRedeemedGraph", "HiddenPointsRedeemedGraph_" + page.Identifier + "_" + widget.Identifier));
        }

        private void BindDummyDataToGreenbacksProductRelatedPonitsEarnedGraphWidget(StringBuilder pageContent, StringBuilder scriptHtmlRenderer, Page page, PageWidget widget)
        {
            var data = "[{\"AccountType\": \"Investment\",\"MonthwiseRewardPoints\" : [{\"Month\": \"Jan\", \"RewardPoint\": 34}, {\"Month\": \"Feb\", \"RewardPoint\": 29},{\"Month\": \"Mar\", \"RewardPoint\": 41}, {\"Month\": \"Apr\", \"RewardPoint\": 48}]}, {\"AccountType\": \"Personal Loan\",\"MonthwiseRewardPoints\" : [{\"Month\": \"Jan\", \"RewardPoint\": 27}, {\"Month\": \"Feb\", \"RewardPoint\": 45},{\"Month\": \"Mar\", \"RewardPoint\": 36}, {\"Month\": \"Apr\", \"RewardPoint\": 51}]}, {\"AccountType\": \"Home Loan\",\"MonthwiseRewardPoints\" : [{\"Month\": \"Jan\", \"RewardPoint\": 37}, {\"Month\": \"Feb\", \"RewardPoint\": 38},{\"Month\": \"Mar\", \"RewardPoint\": 51}, {\"Month\": \"Apr\", \"RewardPoint\": 45}]}]";
            pageContent.Replace("HiddenProductRelatedPointsEarnedGraphValue_" + page.Identifier + "_" + widget.Identifier + "", data);
            scriptHtmlRenderer.Append(HtmlConstants.GREENBACKS_PRODUCT_RELATED_POINTS_EARNED_BAR_GRAPH_SCRIPT.Replace("ProductRelatedPointsEarnedBarGraphcontainer", "ProductRelatedPointsEarnedBarGraphcontainer_" + page.Identifier + "_" + widget.Identifier).Replace("HiddenProductRelatedPointsEarnedGraph", "HiddenProductRelatedPointsEarnedGraph_" + page.Identifier + "_" + widget.Identifier));
        }

        private void BindDummyDataToGreenbacksCategorySpendRewardPointsGraphWidget(StringBuilder pageContent, StringBuilder scriptHtmlRenderer, Page page, PageWidget widget)
        {
            var data = "[{\"Category\": \"Fuel\",\"SpendReward\" : 34}, {\"Category\": \"Groceries\",\"SpendReward\" : 15}, {\"Category\": \"Travel\",\"SpendReward\" : 21}, {\"Category\": \"Movies\",\"SpendReward\" : 19}, {\"Category\": \"Shopping\",\"SpendReward\" : 11}]";
            pageContent.Replace("HiddenCategorySpendRewardsGraphValue_" + page.Identifier + "_" + widget.Identifier + "", data);
            scriptHtmlRenderer.Append(HtmlConstants.GREENBACKS_CATEGORY_SPEND_REWARD_POINTS_BAR_GRAPH_SCRIPT.Replace("CategorySpendRewardsPieChartcontainer", "CategorySpendRewardsPieChartcontainer_" + page.Identifier + "_" + widget.Identifier).Replace("HiddenCategorySpendRewardsGraph", "HiddenCategorySpendRewardsGraph_" + page.Identifier + "_" + widget.Identifier));
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

        #region Bind dummy data to SegmentBasedContent widget

        private void BindDummyDataToSegmentBasedContentWidget(StringBuilder pageContent, Statement statement, Page page, PageWidget widget)
        {
            var html = "<div>This is sample SegmentBasedContent</div>";
            if (widget.WidgetSetting != string.Empty && validationEngine.IsValidJson(widget.WidgetSetting))
            {
                dynamic widgetSetting = JObject.Parse(widget.WidgetSetting);
                if (widgetSetting.html.ToString().Length > 0)
                {
                    html = widgetSetting.html;
                }
            }
            pageContent.Replace("{{SegmentBasedContent_" + statement.Identifier + "_" + page.Identifier + "_" + widget.Identifier + "}}", html);
        }

        #endregion

        #endregion
    }
}