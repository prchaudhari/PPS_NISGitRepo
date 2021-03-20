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
                                    pageHeaderContent.Append(HtmlConstants.WIDGET_HTML_HEADER);

                                    pageHeaderContent.Append(HtmlConstants.NEDBANK_STATEMENT_HEADER.Replace("{{eConfirmLogo}}", "../common/images/eConfirm.png").Replace("{{NedBankLogo}}", "../common/images/NEDBANKLogo.png").Replace("{{StatementDate}}", DateTime.Now.ToString("yyyy-MM-dd")));

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
                                                    var PaddingClass = i != 0 ? " pl-0" : string.Empty;

                                                    if (mergedlst[i].WidgetName == HtmlConstants.SERVICE_WIDGET_NAME)
                                                    {
                                                        //to add Nedbank services header... to do-- Create separate static widgets for widget's header label
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
                                                                pageHtmlContent.Append(this.CustomerDetailsWidgetFormatting(pageWidget, counter, page));
                                                                break;

                                                            case HtmlConstants.BANK_DETAILS_WIDGET_NAME:
                                                                pageHtmlContent.Append(this.BankDetailsWidgetFormatting(pageWidget, counter, page));
                                                                break;

                                                            case HtmlConstants.IMAGE_WIDGET_NAME:
                                                                pageHtmlContent.Append(this.ImageWidgetFormatting(pageWidget, counter, statement, page, divHeight));
                                                                break;

                                                            case HtmlConstants.VIDEO_WIDGET_NAME:
                                                                pageHtmlContent.Append(this.VideoWidgetFormatting(pageWidget, counter, statement, page, divHeight));
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
                                                                pageHtmlContent.Append(this.SpecialMessageWidgetFormatting(pageWidget, counter, page));
                                                                break;

                                                            case HtmlConstants.PERSONAL_LOAN_INSURANCE_MESSAGE_WIDGET_NAME:
                                                                pageHtmlContent.Append(this.PersonalLoanInsuranceMessageWidgetFormatting(pageWidget, counter, page));
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

                                    var footerContent = HtmlConstants.NEDBANK_STATEMENT_FOOTER.Replace("{{NedbankSloganImage}}", "../common/images/See_money_differently.PNG").Replace("{{NedbankNameImage}}", "../common/images/NEDBANK_Name.png").Replace("{{FooterText}}", "Nedbank Ltd Reg No 1951/000009/06. Authorised financial services and registered credit provider (NCRCP16).");

                                    statementPageContent.PageFooterContent = footerContent + HtmlConstants.WIDGET_HTML_FOOTER;
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

                var MarketingMessages = "[{'MarketingMessageHeader': 'Notice of interest rate change','MarketingMessageText1': 'The interest rates on our current notice deposit accounts have changed on 03 November 2020.','MarketingMessageText2': 'Please review your investment portfolio and rates regularly to ensure that it is still appropriate and meets your needs.','MarketingMessageText3': 'For more information about the interest rate change call 0860 555 111.','MarketingMessageText4': '','MarketingMessageText5': ''},{'MarketingMessageHeader': 'Annual investment fee review','MarketingMessageText1': 'As part of our annual pricing review, some of our fees have changed. You can view our 2020 pricing guide on the Nedbank website.','MarketingMessageText2': 'Click <a href=\"javascript:void(0);\" style=\"color:black;text-decoration: underline;\">here </a> for more information.','MarketingMessageText3': 'If you do not want to click on a link, please type the following address in your browser (without the asterisks): *https://www.nedbank.co.za/content/nedbank/desktop/gt/en/info/campaigns/annual-fee-update2020.html*.','MarketingMessageText4': '','MarketingMessageText5': ''},{'MarketingMessageHeader': 'Notice of withdrawal fee','MarketingMessageText1': 'From 1 July 2020 <strong>we will charge fees if you give notice of withdrawals at a branch or call centre</strong>.','MarketingMessageText2': 'Bank smart and avoid fees by using your Money app, Online Banking or dialling *120*001# to give notice of withdrawal on your investment.','MarketingMessageText3': '','MarketingMessageText4': '','MarketingMessageText5': ''},{'MarketingMessageHeader': 'Automatic reinvestment when your investment matures','MarketingMessageText1': 'Without your instruction to reinvest or pay out your money, we will automatically reinvest it in a notice deposit account at the rate applicable to this account when your investment matures. Any subsequent rate changes will also apply.','MarketingMessageText2': 'Please let us know what should happen to your investment before it matures.','MarketingMessageText3': '','MarketingMessageText4': '','MarketingMessageText5': ''},{'MarketingMessageHeader': 'Save time and money with our digital and mobile channels','MarketingMessageText1': 'We are continuously enhancing our digital and mobile banking features and capabilities to give you the best experience. We want you to manage your investment accounts seamlessly so that you can have greater control over your money.','MarketingMessageText2': 'Access and transact on your investment account using the Nedbank Money app, Online Banking, USSD and the Nedbank Money App Lite, which requires less data and storage and no software updates.','MarketingMessageText3': '','MarketingMessageText4': '','MarketingMessageText5': ''},{'MarketingMessageHeader': 'Notice of withdrawal limit on digital and mobile banking channels','MarketingMessageText1': 'For your safety we have placed a limit on the withdrawal amount to any party, when you use digital and mobile banking channels. This limit does not apply to withdrawals we pay to your own Nedbank accounts.','MarketingMessageText2': 'Remember to report any suspicious activity on your account by calling 0860 555 111.','MarketingMessageText3': '','MarketingMessageText4': '','MarketingMessageText5': ''}]";

                //start to render common html content data
                var htmlbody = new StringBuilder();
                htmlbody.Append(HtmlConstants.CONTAINER_DIV_HTML_HEADER);

                //this variable is used to bind all script to html statement, which helps to render data on chart and graph widgets
                var scriptHtmlRenderer = new StringBuilder();
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
                    var pageContent = new StringBuilder(statementPageContent.HtmlContent);
                    var dynamicWidgets = statementPageContent.DynamicWidgets;

                    var PageHeaderContent = new StringBuilder(statementPageContent.PageHeaderContent);
                    var tabClassName = Regex.Replace((statementPageContent.DisplayName + "-" + page.Identifier), @"\s+", "-");
                    PageHeaderContent.Replace("{{ExtraClass}}", (i > 0 ? "d-none " + tabClassName : tabClassName)).Replace("{{DivId}}", tabClassName);

                    var newPageContent = new StringBuilder();
                    newPageContent.Append(HtmlConstants.PAGE_TAB_CONTENT_HEADER);

                    var pagewidgets = new List<PageWidget>(page.PageWidgets);
                    for (int j = 0; j < pagewidgets.Count; j++)
                    {
                        var widget = pagewidgets[j];
                        if (!widget.IsDynamicWidget)
                        {
                            switch (widget.WidgetName)
                            {
                                case HtmlConstants.CUSTOMER_DETAILS_WIDGET_NAME:
                                    this.BindDummyDataToCustomerDetailsWidget(pageContent, page, widget);
                                    break;

                                case HtmlConstants.BANK_DETAILS_WIDGET_NAME:
                                    this.BindDummyDataToBankDetailsWidget(pageContent, page, widget);
                                    break;

                                case HtmlConstants.IMAGE_WIDGET_NAME:
                                    this.BindDummyDataToImageWidget(pageContent, statement, page, widget, SampleFiles, AppBaseDirectory, tenantCode);
                                    break;

                                case HtmlConstants.VIDEO_WIDGET_NAME:
                                    this.BindDummyDataToVideoWidget(pageContent, statement, page, widget, SampleFiles, AppBaseDirectory, tenantCode);
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
                    newPageContent.Append(HtmlConstants.PAGE_TAB_CONTENT_FOOTER); //to end tab-content div
                    statementPageContent.PageHeaderContent = PageHeaderContent.ToString();
                    statementPageContent.HtmlContent = newPageContent.ToString();
                }

                newStatementPageContents.ToList().ForEach(page =>
                {
                    htmlbody.Append(page.PageHeaderContent).Append(page.HtmlContent).Append(page.PageFooterContent);
                });

                htmlbody.Append(HtmlConstants.CONTAINER_DIV_HTML_FOOTER);

                StringBuilder finalHtml = new StringBuilder();
                finalHtml.Append(HtmlConstants.HTML_HEADER).Append(htmlbody.ToString()).Append(HtmlConstants.HTML_FOOTER);
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
                navbarHtml = navbarHtml.Replace("{{Today}}", DateTime.Now.ToString("dd MMM yyyy"));
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
                                                    string customerInfoJson = "{'FirstName':'Laura','MiddleName':'J','LastName':'Donald','AddressLine1':" +
                                                        "'4000 Executive Parkway','AddressLine2':'Saint Globin Rd','City':'Canary Wharf', 'State':'London', " +
                                                        "'Country':'England','Zip':'E14 9RZ'}";
                                                    if (customerInfoJson != string.Empty && validationEngine.IsValidJson(customerInfoJson))
                                                    {
                                                        CustomerInformation customerInfo = JsonConvert.DeserializeObject<CustomerInformation>(customerInfoJson);
                                                        var customerHtmlWidget = HtmlConstants.CUSTOMER_INFORMATION_WIDGET_HTML.Replace("{{VideoSource}}",
                                                            "assets/images/SampleVideo.mp4");
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
                                                    string accountInfoJson = "{'StatementDate':'1-APR-2020','StatementPeriod':'Annual Statement', " +
                                                        "'CustomerID':'ID2-8989-5656','RmName':'James Wiilims','RmContactNumber':'+4487867833'}";

                                                    string accountInfoData = string.Empty;
                                                    StringBuilder AccDivData = new StringBuilder();
                                                    if (accountInfoJson != string.Empty && validationEngine.IsValidJson(accountInfoJson))
                                                    {
                                                        AccountInformation accountInfo = JsonConvert.DeserializeObject<AccountInformation>(accountInfoJson);
                                                        AccDivData.Append("<div class='list-row-small ht70px'><div class='list-middle-row'> <div class='list-text'>" +
                                                            "Statement Date</div><label class='list-value mb-0'>" + accountInfo.StatementDate + "</label>" +
                                                            "</div></div>");

                                                        AccDivData.Append("<div class='list-row-small ht70px'><div class='list-middle-row'> <div class='list-text'>" +
                                                            "Statement Period</div><label class='list-value mb-0'>" + accountInfo.StatementPeriod + "</label></div></div>");

                                                        AccDivData.Append("<div class='list-row-small ht70px'><div class='list-middle-row'> <div class='list-text'>" +
                                                            "Cusomer ID</div><label class='list-value mb-0'>" + accountInfo.CustomerID + "</label></div></div>");

                                                        AccDivData.Append("<div class='list-row-small ht70px'><div class='list-middle-row'> <div class='list-text'>" +
                                                            "RM Name</div><label class='list-value mb-0'>" + accountInfo.RmName + "</label></div></div>");

                                                        AccDivData.Append("<div class='list-row-small ht70px'><div class='list-middle-row'> <div class='list-text'>" +
                                                            "RM Contact Number</div><label class='list-value mb-0'>" + accountInfo.RmContactNumber +
                                                            "</label></div></div>");

                                                        accountInfoData = HtmlConstants.ACCOUNT_INFORMATION_WIDGET_HTML.Replace("{{AccountInfoData}}",
                                                            AccDivData.ToString());
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
                                                    }
                                                    var imgHtmlWidget = HtmlConstants.IMAGE_WIDGET_HTML.Replace("{{ImageSource}}", imgAssetFilepath);
                                                    imgHtmlWidget = imgHtmlWidget.Replace("{{NewImageClass}}", isImageFromAsset ? " ImageAsset " + assetId : "");
                                                    imgHtmlWidget = imgHtmlWidget.Replace("{{WidgetDivHeight}}", divHeight);
                                                    htmlString.Append(imgHtmlWidget);
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
                                                else if (mergedlst[i].WidgetName == HtmlConstants.SUMMARY_AT_GLANCE_WIDGET_NAME)
                                                {
                                                    string accountBalanceDataJson = "[{\"AccountType\":\"Saving Account\",\"Currency\":\"$\",\"Amount\":\"8356\"}" +
                                                        ",{\"AccountType\":\"Current Account\",\"Currency\":\"$\",\"Amount\":\"6654\"},{\"AccountType\":" +
                                                        "\"Recurring Account\",\"Currency\":\"$\",\"Amount\":\"9367\"},{\"AccountType\":\"Wealth\",\"Currency\"" +
                                                        ":\"$\",\"Amount\":\"4589\"}]";

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
                                                                accSummary.Append("<tr><td>" + acc.AccountType + "</td><td>" + acc.Currency + "</td><td>"
                                                                    + acc.Amount + "</td></tr>");
                                                            });
                                                            accountSummary = HtmlConstants.SUMMARY_AT_GLANCE_WIDGET_HTML.Replace("{{AccountSummary}}",
                                                                accSummary.ToString());
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
                                                            transaction.Append("<tr><td>" + trans.TransactionDate + "</td><td>" + trans.TransactionType + "</td><td>" +
                                                                trans.Narration + "</td><td class='text-right'>" + trans.FCY + "</td><td class='text-right'>" + trans.CurrentRate +
                                                                "</td><td class='text-right'>" + trans.LCY + "</td><td><div class='action-btns btn-tbl-action'>" +
                                                                "<button type='button' title='View'><span class='fa fa-paper-plane-o'></span></button></div></td></tr>");
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
                                                            transaction.Append("<tr><td>" + trans.TransactionDate + "</td><td>" + trans.TransactionType + "</td><td>" +
                                                                trans.Narration + "</td><td class='text-right'>" + trans.FCY + "</td><td class='text-right'>" + trans.CurrentRate +
                                                                "</td><td class='text-right'>" + trans.LCY + "</td><td><div class='action-btns btn-tbl-action'>" +
                                                                "<button type='button' title='View'><span class='fa fa-paper-plane-o'></span></button></div></td></tr>");
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
                                                    string incomeSourceListJson = "[{ 'Source': 'Salary Transfer', 'CurrentSpend': 3453, 'AverageSpend': 123}," +
                                                        "{ 'Source': 'Cash Deposit', 'CurrentSpend': 3453, 'AverageSpend': 6123},{ 'Source': 'Profit Earned'," +
                                                        " 'CurrentSpend': 3453, 'AverageSpend': 6123}, { 'Source': 'Rebete', 'CurrentSpend': 3453, 'AverageSpend': 123}]";
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
                                                                tdstring = "<span class='fa fa-sort-asc fa-2x mt-1' aria-hidden='true' " +
                                                                "style='position:relative;top:6px;color:limegreen'></span><span class='ml-2'>" + item.AverageSpend
                                                                + "</span>";
                                                            }
                                                            incomeSrc.Append("<tr><td class='float-left'>" + item.Source + "</td>" + "<td> " + item.CurrentSpend + "" + "</td><td>" + tdstring + "</td></tr>");
                                                        });
                                                        string srcstring = HtmlConstants.TOP_4_INCOME_SOURCE_WIDGET_HTML.Replace("{{IncomeSourceList}}",
                                                            incomeSrc.ToString());
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
                                                    string reminderJson = "[{ 'Title': 'Update Missing Inofrmation', 'Action': 'Update' },{ 'Title': 'Your Rewards Video is available', 'Action': 'View' },{ 'Title': 'Payment Due for Home Loan', 'Action': 'Pay' }, { title: 'Need financial planning for savings.', action: 'Call Me' },{ title: 'Subscribe/Unsubscribe Alerts.', action: 'Apply' },{ title: 'Your credit card payment is due now.', action: 'Pay' }]";
                                                    if (reminderJson != string.Empty && validationEngine.IsValidJson(reminderJson))
                                                    {
                                                        IList<ReminderAndRecommendation> reminderAndRecommendations =
                                                            JsonConvert.DeserializeObject<List<ReminderAndRecommendation>>(reminderJson);
                                                        StringBuilder reminderstr = new StringBuilder();
                                                        reminderstr.Append("<div class='row'><div class='col-lg-9'></div><div class='col-lg-3 text-left'>" +
                                                            "<i class='fa fa-caret-left fa-3x float-left text-danger' aria-hidden='true'></i>" +
                                                            "<span class='mt-2 d-inline-block ml-2'>Click</span></div> </div>");
                                                        reminderAndRecommendations.ToList().ForEach(item =>
                                                        {
                                                            reminderstr.Append("<div class='row'><div class='col-lg-9 text-left'><p class='p-1' " +
                                                                "style='background-color: #dce3dc;'>" + item.Title + " </p></div><div class='col-lg-3 text-left'>" +
                                                                "<a><i class='fa fa-caret-left fa-3x float-left " +
                                                                "text-danger'></i><span class='mt-2 d-inline-block ml-2'>" + item.Action + "</span></a></div></div>");
                                                        });
                                                        string widgetstr = HtmlConstants.REMINDER_WIDGET_HTML.Replace("{{ReminderAndRecommdationDataList}}", reminderstr.ToString());
                                                        widgetstr = widgetstr.Replace("{{WidgetDivHeight}}", divHeight);
                                                        htmlString.Append(widgetstr);
                                                    }
                                                }
                                                else if (mergedlst[i].WidgetName == HtmlConstants.CUSTOMER_DETAILS_WIDGET_NAME)
                                                {
                                                    string jsonstr = "{'TITLE_TEXT': 'MR', 'FIRST_NAME_TEXT':'MATHYS','SURNAME_TEXT':'SMIT','ADDR_LINE_0':'VAN DER MEULENSTRAAT 39','ADDR_LINE_1':'3971 EB DRIEBERGEN','ADDR_LINE_2':'NEDERLAND','ADDR_LINE_3':'9999','ADDR_LINE_4':''}";
                                                    if (jsonstr != string.Empty && validationEngine.IsValidJson(jsonstr))
                                                    {
                                                        var customerInfo = JsonConvert.DeserializeObject<CustomerInformation>(jsonstr);
                                                        var customerHtmlWidget = HtmlConstants.CUSTOMER_DETAILS_WIDGET_HTML;
                                                        customerHtmlWidget = customerHtmlWidget.Replace("{{WidgetDivHeight}}", divHeight);
                                                        customerHtmlWidget = customerHtmlWidget.Replace("{{Title}}", customerInfo.TITLE_TEXT);
                                                        customerHtmlWidget = customerHtmlWidget.Replace("{{FirstName}}", customerInfo.FIRST_NAME_TEXT);
                                                        customerHtmlWidget = customerHtmlWidget.Replace("{{Surname}}", customerInfo.SURNAME_TEXT);
                                                        customerHtmlWidget = customerHtmlWidget.Replace("{{CustAddressLine0}}", customerInfo.ADDR_LINE_0);
                                                        customerHtmlWidget = customerHtmlWidget.Replace("{{CustAddressLine1}}", customerInfo.ADDR_LINE_1);
                                                        customerHtmlWidget = customerHtmlWidget.Replace("{{CustAddressLine2}}", customerInfo.ADDR_LINE_2);
                                                        customerHtmlWidget = customerHtmlWidget.Replace("{{CustAddressLine3}}", customerInfo.ADDR_LINE_3);
                                                        customerHtmlWidget = customerHtmlWidget.Replace("{{CustAddressLine4}}", customerInfo.ADDR_LINE_4);
                                                        htmlString.Append(customerHtmlWidget);
                                                    }
                                                }
                                                else if (mergedlst[i].WidgetName == HtmlConstants.BANK_DETAILS_WIDGET_NAME)
                                                {
                                                    string jsonstr = "{'BankName': 'Nedbank', 'AddressLine1':'135 Rivonia Road, Sandton, 2196', 'AddressLine2':'PO Box 1144, Johannesburg, 2000','CountryName':'South Africa','BankVATRegNo':'4320116074','ContactCenterNo':'0860 555 111'}";
                                                    if (jsonstr != string.Empty && validationEngine.IsValidJson(jsonstr))
                                                    {
                                                        var bankDetails = JsonConvert.DeserializeObject<BankDetails>(jsonstr);
                                                        var bankDetailHtmlWidget = HtmlConstants.BANK_DETAILS_WIDGET_HTML;
                                                        bankDetailHtmlWidget = bankDetailHtmlWidget.Replace("{{WidgetDivHeight}}", divHeight);
                                                        bankDetailHtmlWidget = bankDetailHtmlWidget.Replace("{{BankName}}", bankDetails.BankName);
                                                        bankDetailHtmlWidget = bankDetailHtmlWidget.Replace("{{AddressLine1}}", bankDetails.AddressLine1);
                                                        bankDetailHtmlWidget = bankDetailHtmlWidget.Replace("{{AddressLine2}}", bankDetails.AddressLine2);
                                                        bankDetailHtmlWidget = bankDetailHtmlWidget.Replace("{{CountryName}}", bankDetails.CountryName);
                                                        bankDetailHtmlWidget = bankDetailHtmlWidget.Replace("{{BankVATRegNo}}", "Bank VAT Reg No " + bankDetails.BankVATRegNo);
                                                        bankDetailHtmlWidget = bankDetailHtmlWidget.Replace("{{ContactCenter}}", "Contact centre: " + bankDetails.ContactCenterNo);
                                                        htmlString.Append(bankDetailHtmlWidget);
                                                    }
                                                }
                                                else if (mergedlst[i].WidgetName == HtmlConstants.INVESTMENT_PORTFOLIO_STATEMENT_WIDGET_NAME)
                                                {
                                                    string jsonstr = "{'Currency': 'R', 'TotalClosingBalance': '23 920.98', 'DayOfStatement':'21', 'InvestorId':'204626','StatementPeriod':'22/12/2020 tot 21/01/2021','StatementDate':'21/01/2021'}";
                                                    if (jsonstr != string.Empty && validationEngine.IsValidJson(jsonstr))
                                                    {
                                                        dynamic InvestmentPortfolio = JObject.Parse(jsonstr);
                                                        var InvestmentPortfolioHtmlWidget = HtmlConstants.INVESTMENT_PORTFOLIO_STATEMENT_WIDGET_HTML;
                                                        InvestmentPortfolioHtmlWidget = InvestmentPortfolioHtmlWidget.Replace("{{WidgetDivHeight}}", divHeight);
                                                        InvestmentPortfolioHtmlWidget = InvestmentPortfolioHtmlWidget.Replace("{{DSName}}", string.Empty);
                                                        InvestmentPortfolioHtmlWidget = InvestmentPortfolioHtmlWidget.Replace("{{TotalClosingBalance}}", (Convert.ToString(InvestmentPortfolio.Currency) + Convert.ToString(InvestmentPortfolio.TotalClosingBalance)));
                                                        InvestmentPortfolioHtmlWidget = InvestmentPortfolioHtmlWidget.Replace("{{DayOfStatement}}", Convert.ToString(InvestmentPortfolio.DayOfStatement));
                                                        InvestmentPortfolioHtmlWidget = InvestmentPortfolioHtmlWidget.Replace("{{InvestorID}}", Convert.ToString(InvestmentPortfolio.InvestorId));
                                                        InvestmentPortfolioHtmlWidget = InvestmentPortfolioHtmlWidget.Replace("{{StatementPeriod}}", Convert.ToString(InvestmentPortfolio.StatementPeriod));
                                                        InvestmentPortfolioHtmlWidget = InvestmentPortfolioHtmlWidget.Replace("{{StatementDate}}", Convert.ToString(InvestmentPortfolio.StatementDate));
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
                                                        InvestorPerformanceHtmlWidget = InvestorPerformanceHtmlWidget.Replace("{{WidgetDivHeight}}", divHeight);
                                                        InvestorPerformanceHtmlWidget = InvestorPerformanceHtmlWidget.Replace("{{ProductType}}", Convert.ToString(InvestmentPerformance.ProductType));
                                                        InvestorPerformanceHtmlWidget = InvestorPerformanceHtmlWidget.Replace("{{OpeningBalanceAmount}}", (Convert.ToString(InvestmentPerformance.Currency) + Convert.ToString(InvestmentPerformance.OpeningBalanceAmount)));
                                                        InvestorPerformanceHtmlWidget = InvestorPerformanceHtmlWidget.Replace("{{ClosingBalanceAmount}}", (Convert.ToString(InvestmentPerformance.Currency) + Convert.ToString(InvestmentPerformance.ClosingBalanceAmount)));
                                                        htmlString.Append(InvestorPerformanceHtmlWidget);
                                                    }
                                                }
                                                else if (mergedlst[i].WidgetName == HtmlConstants.BREAKDOWN_OF_INVESTMENT_ACCOUNTS_WIDGET_NAME)
                                                {
                                                    string jsonstr = "[{'InvestorId': '204626','InvestmentId': '9974','Currency': 'R','ProductType': 'Notice Deposits','ProductDesc': 'JustInvest','OpenDate': '05/09/1994','CurrentInterestRate': '2.95','ExpiryDate': '','InterestDisposalDesc': 'Capitalise','NoticePeriod': '1 day','AccuredInterest': '2.94','Transactions': [{'TransactionDate': '26/10/2020','TransactionDesc': 'Balance brought forward','Debit': '0','Credit': '0','Balance': '5 297.02'},{'TransactionDate': '16/11/2020','TransactionDesc': 'Interest capitalised','Debit': '0','Credit': '10.12','Balance': '0'},{'TransactionDate': '25/11/2020','TransactionDesc': 'Balance carried forward','Debit': '0','Credit': '0','Balance': '5 307.14'}]},{'InvestorId': '204626','InvestmentId': '9978','Currency': 'R','ProductType': 'Notice Deposits','ProductDesc': 'JustInvest','OpenDate': '12/11/1996', 'CurrentInterestRate': '2.25','ExpiryDate': '','InterestDisposalDesc': 'Capitalise','NoticePeriod': '1 day','AccuredInterest': '24.10','Transactions': [{'TransactionDate': '26/10/2020','TransactionDesc': 'Balance brought forward','Debit': '0','Credit': '0','Balance': '18 578.34'},{'TransactionDate': '16/11/2020','TransactionDesc': 'Interest capitalised','Debit': '0','Credit': '35.50','Balance': '0'},{'TransactionDate': '25/11/2020','TransactionDesc': 'Balance carried forward','Debit': '0','Credit': '0','Balance': '18 613.84'}]}]";

                                                    if (jsonstr != string.Empty && validationEngine.IsValidJson(jsonstr))
                                                    {
                                                        var InvestmentAccountBreakdownHtml = new StringBuilder(HtmlConstants.BREAKDOWN_OF_INVESTMENT_ACCOUNTS_WIDGET_HTML);
                                                        InvestmentAccountBreakdownHtml.Replace("{{WidgetDivHeight}}", divHeight);
                                                        IList<InvestmentAccount> InvestmentAccounts = JsonConvert.DeserializeObject<List<InvestmentAccount>>(jsonstr);

                                                        //Create Nav tab if investment accounts is more than 1
                                                        var NavTabs = new StringBuilder();
                                                        var InvestmentAccountsCount = InvestmentAccounts.Count;
                                                        if (InvestmentAccountsCount > 1)
                                                        {
                                                            NavTabs.Append("<ul class='nav nav-tabs'>");
                                                            var cnt = 0;
                                                            InvestmentAccounts.ToList().ForEach(acc =>
                                                            {
                                                                NavTabs.Append("<li class='nav-item " + (cnt == 0 ? "active" : "") + "'><a id='tab0-tab' data-toggle='tab' data-target='#" + acc.ProductDesc + "-" + acc.InvestmentId + "' role='tab' class='nav-link " + (cnt == 0 ? "active" : "") + "'> " + acc.ProductDesc + " - " + acc.InvestmentId + "</a></li>");
                                                                cnt++;
                                                            });
                                                            NavTabs.Append("</ul>");
                                                        }
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

                                                            var InvestmentNo = acc.InvestorId + acc.InvestmentId;
                                                            while (InvestmentNo.Length != 12)
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
                                                                tr.Append("<tr>");
                                                                tr.Append("<td class='w-15'>" + trans.TransactionDate + "</td>");
                                                                tr.Append("<td class='w-40'>" + trans.TransactionDesc + "</td>");
                                                                tr.Append("<td class='w-15 text-right'>" + (trans.Debit == "0" ? "-" : acc.Currency + trans.Debit) + "</td>");
                                                                tr.Append("<td class='w-15 text-right'>" + (trans.Credit == "0" ? "-" : acc.Currency + trans.Credit) + "</td>");
                                                                tr.Append("<td class='w-15 text-right'>" + (trans.Balance == "0" ? "-" : acc.Currency + trans.Balance) + "</td>");
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
                                                        NotesHtmlWidget = NotesHtmlWidget.Replace("{{WidgetDivHeight}}", divHeight);
                                                        var notes = new StringBuilder();
                                                        notes.Append("<span> " + Convert.ToString(noteObj.Note1) + " </span> <br/>");
                                                        notes.Append("<span> " + Convert.ToString(noteObj.Note2) + " </span> <br/>");
                                                        notes.Append("<span> " + Convert.ToString(noteObj.Note3) + " </span> ");
                                                        NotesHtmlWidget = NotesHtmlWidget.Replace("{{Notes}}", notes.ToString());
                                                        htmlString.Append(NotesHtmlWidget);
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
                    var MarketingMessages = "[{'MarketingMessageHeader': 'Notice of interest rate change','MarketingMessageText1': 'The interest rates on our current notice deposit accounts have changed on 03 November 2020.','MarketingMessageText2': 'Please review your investment portfolio and rates regularly to ensure that it is still appropriate and meets your needs.','MarketingMessageText3': 'For more information about the interest rate change call 0860 555 111.','MarketingMessageText4': '','MarketingMessageText5': ''},{'MarketingMessageHeader': 'Annual investment fee review','MarketingMessageText1': 'As part of our annual pricing review, some of our fees have changed. You can view our 2020 pricing guide on the Nedbank website.','MarketingMessageText2': 'Click <a href=\"javascript:void(0);\" style=\"color:black;text-decoration: underline;\">here </a> for more information.','MarketingMessageText3': 'If you do not want to click on a link, please type the following address in your browser (without the asterisks): *https://www.nedbank.co.za/content/nedbank/desktop/gt/en/info/campaigns/annual-fee-update2020.html*.','MarketingMessageText4': '','MarketingMessageText5': ''},{'MarketingMessageHeader': 'Notice of withdrawal fee','MarketingMessageText1': 'From 1 July 2020 <strong>we will charge fees if you give notice of withdrawals at a branch or call centre</strong>.','MarketingMessageText2': 'Bank smart and avoid fees by using your Money app, Online Banking or dialling *120*001# to give notice of withdrawal on your investment.','MarketingMessageText3': '','MarketingMessageText4': '','MarketingMessageText5': ''},{'MarketingMessageHeader': 'Automatic reinvestment when your investment matures','MarketingMessageText1': 'Without your instruction to reinvest or pay out your money, we will automatically reinvest it in a notice deposit account at the rate applicable to this account when your investment matures. Any subsequent rate changes will also apply.','MarketingMessageText2': 'Please let us know what should happen to your investment before it matures.','MarketingMessageText3': '','MarketingMessageText4': '','MarketingMessageText5': ''},{'MarketingMessageHeader': 'Save time and money with our digital and mobile channels','MarketingMessageText1': 'We are continuously enhancing our digital and mobile banking features and capabilities to give you the best experience. We want you to manage your investment accounts seamlessly so that you can have greater control over your money.','MarketingMessageText2': 'Access and transact on your investment account using the Nedbank Money app, Online Banking, USSD and the Nedbank Money App Lite, which requires less data and storage and no software updates.','MarketingMessageText3': '','MarketingMessageText4': '','MarketingMessageText5': ''},{'MarketingMessageHeader': 'Notice of withdrawal limit on digital and mobile banking channels','MarketingMessageText1': 'For your safety we have placed a limit on the withdrawal amount to any party, when you use digital and mobile banking channels. This limit does not apply to withdrawals we pay to your own Nedbank accounts.','MarketingMessageText2': 'Remember to report any suspicious activity on your account by calling 0860 555 111.','MarketingMessageText3': '','MarketingMessageText4': '','MarketingMessageText5': ''}]";

                    if (pages.Count != 0)
                    {
                        for (int y = 0; y < pages.Count; y++)
                        {
                            var page = pages[y];
                            var MarketingMessageCounter = 0;
                            var tabClassName = Regex.Replace((page.DisplayName + " " + page.Version), @"\s+", "-");

                            var extraclass = x > 0 ? "d-none " + tabClassName : tabClassName;
                            var pageHeaderHtml = "<div id='{{DivId}}' class='p-2 {{ExtraClass}}' {{BackgroundImage}}>";
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

                            htmlString.Append(HtmlConstants.NEDBANK_STATEMENT_HEADER.Replace("{{eConfirmLogo}}", "assets/images/eConfirm.png").Replace("{{NedBankLogo}}", "assets/images/NEDBANKLogo.png").Replace("{{StatementDate}}", DateTime.Now.ToString("yyyy-MM-dd")));

                            htmlString.Append(HtmlConstants.PAGE_TAB_CONTENT_HEADER);
                            
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
                                                    }
                                                    var imgHtmlWidget = HtmlConstants.IMAGE_WIDGET_HTML.Replace("{{ImageSource}}", imgAssetFilepath);
                                                    imgHtmlWidget = imgHtmlWidget.Replace("{{NewImageClass}}", isImageFromAsset ? " ImageAsset " + assetId : "");
                                                    //imgHtmlWidget = imgHtmlWidget.Replace("{{WidgetDivHeight}}", divHeight);
                                                    htmlString.Append(imgHtmlWidget);
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
                                                else if (mergedlst[i].WidgetName == HtmlConstants.CUSTOMER_DETAILS_WIDGET_NAME)
                                                {
                                                    string jsonstr = "{'TITLE_TEXT': 'MR', 'FIRST_NAME_TEXT':'MATHYS','SURNAME_TEXT':'SMIT','ADDR_LINE_0':'VAN DER MEULENSTRAAT 39','ADDR_LINE_1':'3971 EB DRIEBERGEN','ADDR_LINE_2':'NEDERLAND','ADDR_LINE_3':'9999','ADDR_LINE_4':'', 'MASK_CELL_NO': ''}";
                                                    if (jsonstr != string.Empty && validationEngine.IsValidJson(jsonstr))
                                                    {
                                                        var customerInfo = JsonConvert.DeserializeObject<CustomerInformation>(jsonstr);
                                                        var customerHtmlWidget = HtmlConstants.CUSTOMER_DETAILS_WIDGET_HTML;
                                                        customerHtmlWidget = customerHtmlWidget.Replace("{{Title}}", customerInfo.TITLE_TEXT);
                                                        customerHtmlWidget = customerHtmlWidget.Replace("{{FirstName}}", customerInfo.FIRST_NAME_TEXT);
                                                        customerHtmlWidget = customerHtmlWidget.Replace("{{Surname}}", customerInfo.SURNAME_TEXT);
                                                        customerHtmlWidget = customerHtmlWidget.Replace("{{CustAddressLine0}}", customerInfo.ADDR_LINE_0);
                                                        customerHtmlWidget = customerHtmlWidget.Replace("{{CustAddressLine1}}", customerInfo.ADDR_LINE_1);
                                                        customerHtmlWidget = customerHtmlWidget.Replace("{{CustAddressLine2}}", customerInfo.ADDR_LINE_2);
                                                        customerHtmlWidget = customerHtmlWidget.Replace("{{CustAddressLine3}}", customerInfo.ADDR_LINE_3);
                                                        customerHtmlWidget = customerHtmlWidget.Replace("{{CustAddressLine4}}", customerInfo.ADDR_LINE_4);
                                                        customerHtmlWidget = customerHtmlWidget.Replace("{{MaskCellNo}}", customerInfo.MASK_CELL_NO != string.Empty ? "Cell: " + customerInfo.MASK_CELL_NO : string.Empty);
                                                        htmlString.Append(customerHtmlWidget);
                                                    }
                                                }
                                                else if (mergedlst[i].WidgetName == HtmlConstants.BANK_DETAILS_WIDGET_NAME)
                                                {
                                                    string jsonstr = "{'BankName': 'Nedbank', 'AddressLine1':'135 Rivonia Road, Sandton, 2196', 'AddressLine2':'PO Box 1144, Johannesburg, 2000','CountryName':'South Africa','BankVATRegNo':'4320116074','ContactCenterNo':'0860 555 111'}";
                                                    if (jsonstr != string.Empty && validationEngine.IsValidJson(jsonstr))
                                                    {
                                                        var bankDetails = JsonConvert.DeserializeObject<BankDetails>(jsonstr);
                                                        var bankDetailHtmlWidget = HtmlConstants.BANK_DETAILS_WIDGET_HTML;
                                                        bankDetailHtmlWidget = bankDetailHtmlWidget.Replace("{{BankName}}", bankDetails.BankName);
                                                        bankDetailHtmlWidget = bankDetailHtmlWidget.Replace("{{AddressLine1}}", bankDetails.AddressLine1);
                                                        bankDetailHtmlWidget = bankDetailHtmlWidget.Replace("{{AddressLine2}}", bankDetails.AddressLine2);
                                                        bankDetailHtmlWidget = bankDetailHtmlWidget.Replace("{{CountryName}}", bankDetails.CountryName);
                                                        bankDetailHtmlWidget = bankDetailHtmlWidget.Replace("{{BankVATRegNo}}", "Bank VAT Reg No " + bankDetails.BankVATRegNo);
                                                        bankDetailHtmlWidget = bankDetailHtmlWidget.Replace("{{ContactCenter}}", "Contact centre: " + bankDetails.ContactCenterNo);
                                                        htmlString.Append(bankDetailHtmlWidget);
                                                    }
                                                }
                                                else if (mergedlst[i].WidgetName == HtmlConstants.INVESTMENT_PORTFOLIO_STATEMENT_WIDGET_NAME)
                                                {
                                                    string jsonstr = "{'Currency': 'R', 'TotalClosingBalance': '23 920.98', 'DayOfStatement':'21', 'InvestorId':'204626','StatementPeriod':'22/12/2020 to 21/01/2021','StatementDate':'21/01/2021', 'DsInvestorName' : ''}";
                                                    if (jsonstr != string.Empty && validationEngine.IsValidJson(jsonstr))
                                                    {
                                                        dynamic InvestmentPortfolio = JObject.Parse(jsonstr);
                                                        var InvestmentPortfolioHtmlWidget = HtmlConstants.INVESTMENT_PORTFOLIO_STATEMENT_WIDGET_HTML;
                                                        InvestmentPortfolioHtmlWidget = InvestmentPortfolioHtmlWidget.Replace("{{DSName}}", Convert.ToString(InvestmentPortfolio.DsInvestorName));
                                                        InvestmentPortfolioHtmlWidget = InvestmentPortfolioHtmlWidget.Replace("{{TotalClosingBalance}}", (Convert.ToString(InvestmentPortfolio.Currency) + Convert.ToString(InvestmentPortfolio.TotalClosingBalance)));
                                                        InvestmentPortfolioHtmlWidget = InvestmentPortfolioHtmlWidget.Replace("{{DayOfStatement}}", Convert.ToString(InvestmentPortfolio.DayOfStatement));
                                                        InvestmentPortfolioHtmlWidget = InvestmentPortfolioHtmlWidget.Replace("{{InvestorID}}", Convert.ToString(InvestmentPortfolio.InvestorId));
                                                        InvestmentPortfolioHtmlWidget = InvestmentPortfolioHtmlWidget.Replace("{{StatementPeriod}}", Convert.ToString(InvestmentPortfolio.StatementPeriod));
                                                        InvestmentPortfolioHtmlWidget = InvestmentPortfolioHtmlWidget.Replace("{{StatementDate}}", Convert.ToString(InvestmentPortfolio.StatementDate));
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
                                                    string jsonstr = "[{'InvestorId': '204626','InvestmentId': '9974','Currency': 'R','ProductType': 'Notice Deposits','ProductDesc': 'JustInvest','OpenDate': '05/09/1994','CurrentInterestRate': '2.95','ExpiryDate': '','InterestDisposalDesc': 'Capitalise','NoticePeriod': '1 day','AccuredInterest': '2.94','Transactions': [{'TransactionDate': '26/10/2020','TransactionDesc': 'Balance brought forward','Debit': '0','Credit': '0','Balance': '5 297.02'},{'TransactionDate': '16/11/2020','TransactionDesc': 'Interest capitalised','Debit': '0','Credit': '10.12','Balance': '0'},{'TransactionDate': '25/11/2020','TransactionDesc': 'Balance carried forward','Debit': '0','Credit': '0','Balance': '5 307.14'}]},{'InvestorId': '204626','InvestmentId': '9978','Currency': 'R','ProductType': 'Notice Deposits','ProductDesc': 'JustInvest','OpenDate': '12/11/1996', 'CurrentInterestRate': '2.25','ExpiryDate': '','InterestDisposalDesc': 'Capitalise','NoticePeriod': '1 day','AccuredInterest': '24.10','Transactions': [{'TransactionDate': '26/10/2020','TransactionDesc': 'Balance brought forward','Debit': '0','Credit': '0','Balance': '18 578.34'},{'TransactionDate': '16/11/2020','TransactionDesc': 'Interest capitalised','Debit': '0','Credit': '35.50','Balance': '0'},{'TransactionDate': '25/11/2020','TransactionDesc': 'Balance carried forward','Debit': '0','Credit': '0','Balance': '18 613.84'}]}]";

                                                    if (jsonstr != string.Empty && validationEngine.IsValidJson(jsonstr))
                                                    {
                                                        var InvestmentAccountBreakdownHtml = new StringBuilder(HtmlConstants.BREAKDOWN_OF_INVESTMENT_ACCOUNTS_WIDGET_HTML);
                                                        IList<InvestmentAccount> InvestmentAccounts = JsonConvert.DeserializeObject<List<InvestmentAccount>>(jsonstr);

                                                        //Create Nav tab if investment accounts is more than 1
                                                        var NavTabs = new StringBuilder();
                                                        var InvestmentAccountsCount = InvestmentAccounts.Count;
                                                        if (InvestmentAccountsCount > 1)
                                                        {
                                                            NavTabs.Append("<ul class='nav nav-tabs Investment-nav-tabs'>");
                                                            var cnt = 0;
                                                            InvestmentAccounts.ToList().ForEach(acc =>
                                                            {
                                                                NavTabs.Append("<li class='nav-item " + (cnt == 0 ? "active" : "") + "'><a id='tab0-tab' data-toggle='tab' data-target='#" + acc.ProductDesc + "-" + acc.InvestmentId + "' role='tab' class='nav-link " + (cnt == 0 ? "active" : "") + "'> " + acc.ProductDesc + " - " + acc.InvestmentId + "</a></li>");
                                                                cnt++;
                                                            });
                                                            NavTabs.Append("</ul>");
                                                        }
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
                                                    string jsonstr = "{'Identifier': 1,'CustomerId': 211135146504,'InvestorId': 8004334234001,'Currency': 'R','ProductType': 'PersonalLoan','BranchId': 1,'CreditAdvance': '75372','OutstandingBalance': '68169','AmountDue': '3297','ToDate': '2021-02-28 00:00:00','FromDate': '2020-12-01 00:00:00','MonthlyInstallment': '3297','DueDate': '2021-03-31 00:00:00','Arrears': '0','AnnualRate': '24','Term': '36'}";
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
                                                    string jsonstr = "[{'Identifier': 1,'CustomerId': 211135146504,'InvestorId': 8004334234001,'PostingDate': '2020-12-31 00:00:00','EffectiveDate': '2020-12-31 00:00:00','Description': 'Monthly Admin Fee','Debit': '69','Credit': '0','OutstandingCapital': '71258'},{'Identifier': 2,'CustomerId': 211135146504, 'InvestorId': 8004334234001,'PostingDate': '2021-01-01 00:00:00','EffectiveDate': '2021-01-01 00:00:00','Description': 'Interest Debit','Debit': '1512','Credit': '0','OutstandingCapital': '72770'},{'Identifier': 3,'CustomerId': 211135146504,'InvestorId': 8004334234001, 'PostingDate': '2021-01-05 00:00:00','EffectiveDate': '2021-01-05 00:00:00','Description': 'Insurance Premium','Debit': '188','Credit': '0', 'OutstandingCapital': '72958'},{'Identifier': 4,'CustomerId': 211135146504,'InvestorId': 8004334234001,'PostingDate': '2021-01-15 00:00:00', 'EffectiveDate': '2021-01-15 00:00:00','Description': '','Debit': '0','Credit': '3297','OutstandingCapital': '69660'},{'Identifier': 5,'CustomerId': 211135146504,'InvestorId': 8004334234001,'PostingDate': '2021-01-30 00:00:00','EffectiveDate': '2021-01-30 00:00:00','Description': 'Monthly Admin Fee','Debit': '69','Credit': '0','OutstandingCapital': '69729'},{'Identifier': 6,'CustomerId': 211135146504,'InvestorId': 8004334234001, 'PostingDate': '2021-02-01 00:00:00', 'EffectiveDate': '2021-02-01 00:00:00','Description': 'Interest Debit','Debit': '1480','Credit': '0', 'OutstandingCapital': '71210'},{'Identifier': 7,'CustomerId': 211135146504,'InvestorId': 8004334234001, 'PostingDate': '2021-02-03 00:00:00', 'EffectiveDate': '2021-02-03 00:00:00','Description': 'Insurance Premium','Debit': '188','Credit': '0','OutstandingCapital': '71398'}, {'Identifier': 8,'CustomerId': 211135146504,'InvestorId': 8004334234001,'PostingDate': '2021-02-15 00:00:00','EffectiveDate': '2021-02-15 00:00:00', 'Description': '','Debit': '0','Credit': '3297', 'OutstandingCapital': '68100'},{'Identifier': 9,'CustomerId': 211135146504,'InvestorId': 8004334234001,'PostingDate': '2021-02-27 00:00:00', 'EffectiveDate': '2021-02-27 00:00:00','Description': 'Monthly Admin Fee','Debit': '69', 'Credit': '0','OutstandingCapital': '68169'}]";
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
                                                                tr.Append("<td class='w-13'> " + trans.PostingDate.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy) + " </td>");
                                                                tr.Append("<td class='w-15'> " + trans.EffectiveDate.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy) + " </td>");
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
                                                    string jsonstr = "{'Identifier': '1','ParentId': '8004334234001','Header': '','Message1': 'According to our records your personal-loan account is included in the debt counselling process. Please continue making regular payments as per the agreed payment arrangement. For more information please contact us on 0860 109 279 or email us at DebtCounsellingQueries@nedbank.co.za.','Message2': '','Message3': 'Insurance Personalise text message 1','Message4': 'Insurance Personalise text message 2','Message5': 'Insurance Personalise text message 3'}";
                                                    if (jsonstr != string.Empty && validationEngine.IsValidJson(jsonstr))
                                                    {
                                                        var SpecialMessage = JsonConvert.DeserializeObject<SpecialMessage>(jsonstr);
                                                        if (SpecialMessage != null)
                                                        {
                                                            var widgetHtml = new StringBuilder(HtmlConstants.SPECIAL_MESSAGE_HTML);

                                                            var specialMsgTxtData = (!string.IsNullOrEmpty(SpecialMessage.Header) ? "<div class='SpecialMessageHeader'> " + SpecialMessage.Header + " </div>" : string.Empty) + (!string.IsNullOrEmpty(SpecialMessage.Message1) ? "<p> " + SpecialMessage.Message1 + " </p>" : string.Empty) + (!string.IsNullOrEmpty(SpecialMessage.Message2) ? "<p> " + SpecialMessage.Message2 + " </p>" : string.Empty);

                                                            widgetHtml.Replace("{{SpecialMessageTextData}}", specialMsgTxtData);
                                                            htmlString.Append(widgetHtml.ToString());
                                                        }
                                                    }
                                                }
                                                else if (mergedlst[i].WidgetName == HtmlConstants.PERSONAL_LOAN_INSURANCE_MESSAGE_WIDGET_NAME)
                                                {
                                                    string jsonstr = "{'Identifier': '1','ParentId': '8004334234001','Header': '','Message1': 'According to our records your personal-loan account is included in the debt counselling process. Please continue making regular payments as per the agreed payment arrangement. For more information please contact us on 0860 109 279 or email us at DebtCounsellingQueries@nedbank.co.za.','Message2': '','Message3': 'Insurance Personalise text message 1','Message4': 'Insurance Personalise text message 2','Message5': 'Insurance Personalise text message 3'}";
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

                var footerContent = HtmlConstants.NEDBANK_STATEMENT_FOOTER.Replace("{{NedbankSloganImage}}", "assets/images/See_money_differently.PNG").Replace("{{NedbankNameImage}}", "assets/images/NEDBANK_Name.png").Replace("{{FooterText}}", "Nedbank Ltd Reg No 1951/000009/06. Authorised financial services and registered credit provider (NCRCP16).");

                tempHtml.Append(footerContent);
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
            var customerHtmlWidget = HtmlConstants.CUSTOMER_DETAILS_WIDGET_HTML;
            customerHtmlWidget = customerHtmlWidget.Replace("{{Title}}", "{{Title_" + page.Identifier + "_" + pageWidget.Identifier + "}}");
            customerHtmlWidget = customerHtmlWidget.Replace("{{FirstName}}", "{{FirstName_" + page.Identifier + "_" + pageWidget.Identifier + "}}");
            customerHtmlWidget = customerHtmlWidget.Replace("{{Surname}}", "{{Surname_" + page.Identifier + "_" + pageWidget.Identifier + "}}");
            customerHtmlWidget = customerHtmlWidget.Replace("{{CustAddressLine0}}", "{{CustAddressLine0_" + page.Identifier + "_" + pageWidget.Identifier + "}}");
            customerHtmlWidget = customerHtmlWidget.Replace("{{CustAddressLine1}}", "{{CustAddressLine1_" + page.Identifier + "_" + pageWidget.Identifier + "}}");
            customerHtmlWidget = customerHtmlWidget.Replace("{{CustAddressLine2}}", "{{CustAddressLine2_" + page.Identifier + "_" + pageWidget.Identifier + "}}");
            customerHtmlWidget = customerHtmlWidget.Replace("{{CustAddressLine3}}", "{{CustAddressLine3_" + page.Identifier + "_" + pageWidget.Identifier + "}}");
            customerHtmlWidget = customerHtmlWidget.Replace("{{CustAddressLine4}}", "{{CustAddressLine4_" + page.Identifier + "_" + pageWidget.Identifier + "}}");
            customerHtmlWidget = customerHtmlWidget.Replace("{{MaskCellNo}}", "{{MaskCellNo_" + page.Identifier + "_" + pageWidget.Identifier + "}}");
            customerHtmlWidget = customerHtmlWidget.Replace("{{WidgetId}}", widgetId);
            return customerHtmlWidget;
        }

        private string BankDetailsWidgetFormatting(PageWidget pageWidget, int counter, Page page)
        {
            var widgetId = "PageWidgetId_" + pageWidget.Identifier + "_Counter" + counter.ToString();
            var bankDetailHtmlWidget = HtmlConstants.BANK_DETAILS_WIDGET_HTML;
            bankDetailHtmlWidget = bankDetailHtmlWidget.Replace("{{BankName}}", "{{BankName_" + page.Identifier + "_" + pageWidget.Identifier + "}}");
            bankDetailHtmlWidget = bankDetailHtmlWidget.Replace("{{AddressLine1}}", "{{AddressLine1_" + page.Identifier + "_" + pageWidget.Identifier + "}}");
            bankDetailHtmlWidget = bankDetailHtmlWidget.Replace("{{AddressLine2}}", "{{AddressLine2_" + page.Identifier + "_" + pageWidget.Identifier + "}}");
            bankDetailHtmlWidget = bankDetailHtmlWidget.Replace("{{CountryName}}", "{{CountryName_" + page.Identifier + "_" + pageWidget.Identifier + "}}");
            bankDetailHtmlWidget = bankDetailHtmlWidget.Replace("{{BankVATRegNo}}", "{{BankVATRegNo_" + page.Identifier + "_" + pageWidget.Identifier + "}}");
            bankDetailHtmlWidget = bankDetailHtmlWidget.Replace("{{ContactCenter}}", "{{ContactCenter_" + page.Identifier + "_" + pageWidget.Identifier + "}}");
            bankDetailHtmlWidget = bankDetailHtmlWidget.Replace("{{WidgetId}}", widgetId);
            return bankDetailHtmlWidget;
        }

        private string InvestmentPortfolioStatementWidgetFormatting(PageWidget pageWidget, int counter, Page page)
        {
            var widgetId = "PageWidgetId_" + pageWidget.Identifier + "_Counter" + counter.ToString();
            var InvestmentPortfolioHtmlWidget = HtmlConstants.INVESTMENT_PORTFOLIO_STATEMENT_WIDGET_HTML;
            InvestmentPortfolioHtmlWidget = InvestmentPortfolioHtmlWidget.Replace("{{DSName}}", "{{DSName_" + page.Identifier + "_" + pageWidget.Identifier + "}}");
            InvestmentPortfolioHtmlWidget = InvestmentPortfolioHtmlWidget.Replace("{{TotalClosingBalance}}", "{{TotalClosingBalance_" + page.Identifier + "_" + pageWidget.Identifier + "}}");
            InvestmentPortfolioHtmlWidget = InvestmentPortfolioHtmlWidget.Replace("{{DayOfStatement}}", "{{DayOfStatement_" + page.Identifier + "_" + pageWidget.Identifier + "}}");
            InvestmentPortfolioHtmlWidget = InvestmentPortfolioHtmlWidget.Replace("{{InvestorID}}", "{{InvestorID_" + page.Identifier + "_" + pageWidget.Identifier + "}}");
            InvestmentPortfolioHtmlWidget = InvestmentPortfolioHtmlWidget.Replace("{{StatementPeriod}}", "{{StatementPeriod_" + page.Identifier + "_" + pageWidget.Identifier + "}}");
            InvestmentPortfolioHtmlWidget = InvestmentPortfolioHtmlWidget.Replace("{{StatementDate}}", "{{StatementDate_" + page.Identifier + "_" + pageWidget.Identifier + "}}");
            InvestmentPortfolioHtmlWidget = InvestmentPortfolioHtmlWidget.Replace("{{WidgetId}}", widgetId);
            return InvestmentPortfolioHtmlWidget;
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

        private string SpecialMessageWidgetFormatting(PageWidget pageWidget, int counter, Page page)
        {
            var widgetId = "PageWidgetId_" + pageWidget.Identifier + "_Counter" + counter.ToString();
            var htmlWidget = new StringBuilder(HtmlConstants.SPECIAL_MESSAGE_HTML);
            htmlWidget.Replace("{{SpecialMessageTextData}}", "{{SpecialMessageTextData_" + page.Identifier + "_" + pageWidget.Identifier + "}}");
            htmlWidget = htmlWidget.Replace("{{WidgetId}}", widgetId);
            return htmlWidget.ToString();
        }

        private string PersonalLoanInsuranceMessageWidgetFormatting(PageWidget pageWidget, int counter, Page page)
        {
            var widgetId = "PageWidgetId_" + pageWidget.Identifier + "_Counter" + counter.ToString();
            var htmlWidget = new StringBuilder(HtmlConstants.SPECIAL_MESSAGE_HTML);
            htmlWidget.Replace("{{InsuranceMessages}}", "{{InsuranceMessages_" + page.Identifier + "_" + pageWidget.Identifier + "}}");
            htmlWidget = htmlWidget.Replace("{{WidgetId}}", widgetId);
            return htmlWidget.ToString();
        }

        #endregion

        #region Image and Video Widget formatting

        private string ImageWidgetFormatting(PageWidget pageWidget, int counter, Statement statement, Page page, string divHeight)
        {
            var widgetId = "PageWidgetId_" + pageWidget.Identifier + "_Counter" + counter.ToString();
            var widgetHTML = HtmlConstants.IMAGE_WIDGET_HTML_FOR_STMT.Replace("{{ImageSource}}", "{{ImageSource_" + statement.Identifier + "_" + page.Identifier + "_" + pageWidget.Identifier + "}}");
            widgetHTML = widgetHTML.Replace("{{WidgetDivHeight}}", divHeight);
            widgetHTML = widgetHTML.Replace("{{WidgetId}}", widgetId);
            if (pageWidget.WidgetSetting != string.Empty && validationEngine.IsValidJson(pageWidget.WidgetSetting))
            {
                var imageWidgetHtml = string.Empty;
                dynamic widgetSetting = JObject.Parse(pageWidget.WidgetSetting);
                if (widgetSetting.isPersonalize == false && widgetSetting.SourceUrl != null && widgetSetting.SourceUrl != string.Empty)
                {
                    widgetHTML = widgetHTML.Replace("{{TargetLink}}", "<a href='" + widgetSetting.SourceUrl + "' target='_blank'>");
                    widgetHTML = widgetHTML.Replace("{{EndTargetLink}}", "</a>");
                }
                else
                {
                    widgetHTML = widgetHTML.Replace("{{TargetLink}}", "");
                    widgetHTML = widgetHTML.Replace("{{EndTargetLink}}", "");
                }
            }
            return widgetHTML;
        }

        private string VideoWidgetFormatting(PageWidget pageWidget, int counter, Statement statement, Page page, string divHeight)
        {
            var widgetId = "PageWidgetId_" + pageWidget.Identifier + "_Counter" + counter.ToString();
            var widgetHTML = HtmlConstants.VIDEO_WIDGET_HTML_FOR_STMT.Replace("{{VideoSource}}", "{{VideoSource_" + statement.Identifier + "_" + page.Identifier + "_" + pageWidget.Identifier + "}}");
            widgetHTML = widgetHTML.Replace("{{WidgetDivHeight}}", divHeight);
            widgetHTML = widgetHTML.Replace("{{WidgetId}}", widgetId);
            return widgetHTML;
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
            string jsonstr = "{'TITLE_TEXT': 'MR', 'FIRST_NAME_TEXT':'MATHYS','SURNAME_TEXT':'SMIT','ADDR_LINE_0':'VAN DER MEULENSTRAAT 39','ADDR_LINE_1':'3971 EB DRIEBERGEN', 'ADDR_LINE_2':'NEDERLAND', 'ADDR_LINE_3':'9999', 'ADDR_LINE_4':'', 'MASK_CELL_NO':'*** *** 1324'}";
            if (jsonstr != string.Empty && validationEngine.IsValidJson(jsonstr))
            {
                var customerInfo = JsonConvert.DeserializeObject<CustomerInformation>(jsonstr);
                pageContent.Replace("{{Title_" + page.Identifier + "_" + widget.Identifier + "}}", customerInfo.TITLE_TEXT);
                pageContent.Replace("{{FirstName_" + page.Identifier + "_" + widget.Identifier + "}}", customerInfo.FIRST_NAME_TEXT);
                pageContent.Replace("{{Surname_" + page.Identifier + "_" + widget.Identifier + "}}", customerInfo.SURNAME_TEXT);
                pageContent.Replace("{{CustAddressLine0_" + page.Identifier + "_" + widget.Identifier + "}}", customerInfo.ADDR_LINE_0);
                pageContent.Replace("{{CustAddressLine1_" + page.Identifier + "_" + widget.Identifier + "}}", customerInfo.ADDR_LINE_1);
                pageContent.Replace("{{CustAddressLine2_" + page.Identifier + "_" + widget.Identifier + "}}", customerInfo.ADDR_LINE_2);
                pageContent.Replace("{{CustAddressLine3_" + page.Identifier + "_" + widget.Identifier + "}}", customerInfo.ADDR_LINE_3);
                pageContent.Replace("{{CustAddressLine4_" + page.Identifier + "_" + widget.Identifier + "}}", customerInfo.ADDR_LINE_4);
                pageContent.Replace("{{MaskCellNo_" + page.Identifier + "_" + widget.Identifier + "}}", "Cell: "+customerInfo.MASK_CELL_NO);
            }
        }

        private void BindDummyDataToBankDetailsWidget(StringBuilder pageContent, Page page, PageWidget widget)
        {
            string jsonstr = "{'BankName': 'Nedbank', 'AddressLine1':'135 Rivonia Road, Sandton, 2196', 'AddressLine2':'PO Box 1144, Johannesburg, 2000','CountryName':'South Africa', 'BankVATRegNo':'4320116074','ContactCenterNo':'0860 555 111'}";
            if (jsonstr != string.Empty && validationEngine.IsValidJson(jsonstr))
            {
                var bankDetails = JsonConvert.DeserializeObject<BankDetails>(jsonstr);
                pageContent.Replace("{{BankName_" + page.Identifier + "_" + widget.Identifier + "}}", bankDetails.BankName);
                pageContent.Replace("{{AddressLine1_" + page.Identifier + "_" + widget.Identifier + "}}", bankDetails.AddressLine1);
                pageContent.Replace("{{AddressLine2_" + page.Identifier + "_" + widget.Identifier + "}}", bankDetails.AddressLine2);
                pageContent.Replace("{{CountryName_" + page.Identifier + "_" + widget.Identifier + "}}", bankDetails.CountryName);
                pageContent.Replace("{{BankVATRegNo_" + page.Identifier + "_" + widget.Identifier + "}}", "Bank VAT Reg No " + bankDetails.BankVATRegNo);
                pageContent.Replace("{{ContactCenter_" + page.Identifier + "_" + widget.Identifier + "}}", "Contact centre: " + bankDetails.ContactCenterNo);
            }
        }

        private void BindDummyDataToInvestmentPortfolioStatementWidget(StringBuilder pageContent, Page page, PageWidget widget)
        {
            string jsonstr = "{'Currency': 'R', 'TotalClosingBalance': '23 920.98', 'DayOfStatement':'21', 'InvestorId':'204626','StatementPeriod':'22/12/2020 to 21/01/2021', 'StatementDate':'21/01/2021', 'DsInvestorName' : '' }";
            if (jsonstr != string.Empty && validationEngine.IsValidJson(jsonstr))
            {
                dynamic InvestmentPortfolio = JObject.Parse(jsonstr);
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
            string jsonstr = "[{'InvestorId': '204626','InvestmentId': '9974','Currency': 'R','ProductType': 'Notice Deposits','ProductDesc': 'JustInvest','OpenDate': '05/09/1994', 'CurrentInterestRate': '2.95', 'ExpiryDate': '','InterestDisposalDesc': 'Capitalise','NoticePeriod': '1 day','AccuredInterest': '2.94','Transactions': [{'TransactionDate': '26/10/2020', 'TransactionDesc': 'Balance brought forward','Debit': '0','Credit': '0','Balance': '5 297.02'},{'TransactionDate': '16/11/2020','TransactionDesc': 'Interest capitalised', 'Debit': '0','Credit': '10.12','Balance': '0'},{'TransactionDate': '25/11/2020','TransactionDesc': 'Balance carried forward','Debit': '0','Credit': '0','Balance': '5 307.14'}]},{'InvestorId': '204626','InvestmentId': '9978','Currency': 'R','ProductType': 'Notice Deposits','ProductDesc': 'JustInvest','OpenDate': '12/11/1996', 'CurrentInterestRate': '2.25', 'ExpiryDate': '','InterestDisposalDesc': 'Capitalise','NoticePeriod': '1 day','AccuredInterest': '24.10','Transactions': [{'TransactionDate': '26/10/2020','TransactionDesc': 'Balance brought forward','Debit': '0','Credit': '0','Balance': '18 578.34'},{'TransactionDate': '16/11/2020','TransactionDesc': 'Interest capitalised','Debit': '0','Credit': '35.50', 'Balance': '0'},{'TransactionDate': '25/11/2020','TransactionDesc': 'Balance carried forward','Debit': '0','Credit': '0','Balance': '18 613.84'}]}]";

            if (jsonstr != string.Empty && validationEngine.IsValidJson(jsonstr))
            {
                IList<InvestmentAccount> InvestmentAccounts = JsonConvert.DeserializeObject<List<InvestmentAccount>>(jsonstr);

                //Create Nav tab if investment accounts is more than 1
                var NavTabs = new StringBuilder();
                var InvestmentAccountsCount = InvestmentAccounts.Count;
                if (InvestmentAccountsCount > 1)
                {
                    NavTabs.Append("<ul class='nav nav-tabs Investment-nav-tabs'>");
                    var cnt = 0;
                    InvestmentAccounts.ToList().ForEach(acc =>
                    {
                        NavTabs.Append("<li class='nav-item " + (cnt == 0 ? "active" : "") + "'><a id='tab0-tab' data-toggle='tab' data-target='#" + acc.ProductDesc + "-" + acc.InvestmentId + "' role='tab' class='nav-link " + (cnt == 0 ? "active" : "") + "'> " + acc.ProductDesc + " - " + acc.InvestmentId + "</a></li>");
                        cnt++;
                    });
                    NavTabs.Append("</ul>");
                }
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
            string jsonstr = "[{'Identifier': 1,'CustomerId': 211135146504,'InvestorId': 8004334234001,'PostingDate': '2020-12-31 00:00:00','EffectiveDate': '2020-12-31 00:00:00','Description': 'Monthly Admin Fee','Debit': '69','Credit': '0','OutstandingCapital': '71258'},{'Identifier': 2,'CustomerId': 211135146504, 'InvestorId': 8004334234001,'PostingDate': '2021-01-01 00:00:00','EffectiveDate': '2021-01-01 00:00:00','Description': 'Interest Debit','Debit': '1512','Credit': '0','OutstandingCapital': '72770'},{'Identifier': 3,'CustomerId': 211135146504,'InvestorId': 8004334234001,'PostingDate': '2021-01-05 00:00:00','EffectiveDate': '2021-01-05 00:00:00','Description': 'Insurance Premium','Debit': '188','Credit': '0','OutstandingCapital': '72958'},{'Identifier': 4,'CustomerId': 211135146504,'InvestorId': 8004334234001,'PostingDate': '2021-01-15 00:00:00','EffectiveDate': '2021-01-15 00:00:00','Description': '','Debit': '0','Credit': '3297','OutstandingCapital': '69660'},{'Identifier': 5,'CustomerId': 211135146504,'InvestorId': 8004334234001,'PostingDate': '2021-01-30 00:00:00','EffectiveDate': '2021-01-30 00:00:00','Description': 'Monthly Admin Fee','Debit': '69','Credit': '0','OutstandingCapital': '69729'},{'Identifier': 6,'CustomerId': 211135146504,'InvestorId': 8004334234001,'PostingDate': '2021-02-01 00:00:00', 'EffectiveDate': '2021-02-01 00:00:00','Description': 'Interest Debit','Debit': '1480','Credit': '0','OutstandingCapital': '71210'},{'Identifier': 7,'CustomerId': 211135146504,'InvestorId': 8004334234001,'PostingDate': '2021-02-03 00:00:00','EffectiveDate': '2021-02-03 00:00:00','Description': 'Insurance Premium','Debit': '188','Credit': '0','OutstandingCapital': '71398'},{'Identifier': 8,'CustomerId': 211135146504,'InvestorId': 8004334234001,'PostingDate': '2021-02-15 00:00:00','EffectiveDate': '2021-02-15 00:00:00','Description': '','Debit': '0','Credit': '3297', 'OutstandingCapital': '68100'},{'Identifier': 9,'CustomerId': 211135146504,'InvestorId': 8004334234001,'PostingDate': '2021-02-27 00:00:00', 'EffectiveDate': '2021-02-27 00:00:00','Description': 'Monthly Admin Fee','Debit': '69','Credit': '0','OutstandingCapital': '68169'}]";
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
                        tr.Append("<td class='w-13'> " + trans.PostingDate.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy) + " </td>");
                        tr.Append("<td class='w-15'> " + trans.EffectiveDate.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy) + " </td>");
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
            string jsonstr = "{'Identifier': '1','ParentId': '8004334234001','Header': '','Message1': 'According to our records your personal-loan account is included in the debt counselling process. Please continue making regular payments as per the agreed payment arrangement. For more information please contact us on 0860 109 279 or email us at DebtCounsellingQueries@nedbank.co.za.','Message2': '','Message3': 'Insurance Personalise text message 1','Message4': 'Insurance Personalise text message 2','Message5': 'Insurance Personalise text message 3'}";
            if (jsonstr != string.Empty && validationEngine.IsValidJson(jsonstr))
            {
                var SpecialMessage = JsonConvert.DeserializeObject<SpecialMessage>(jsonstr);
                if (SpecialMessage != null)
                {
                    var specialMsgTxtData = (!string.IsNullOrEmpty(SpecialMessage.Header) ? "<div class='SpecialMessageHeader'> " + SpecialMessage.Header + " </div>" : string.Empty) + (!string.IsNullOrEmpty(SpecialMessage.Message1) ? "<p> " + SpecialMessage.Message1 + " </p>" : string.Empty) + (!string.IsNullOrEmpty(SpecialMessage.Message2) ? "<p> " + SpecialMessage.Message2 + " </p>" : string.Empty);
                    pageContent.Replace("{{SpecialMessageTextData_" + page.Identifier + "_" + widget.Identifier + "}}", specialMsgTxtData);
                }
            }
        }

        private void BindDummmyDataToPersonalLoanInsuranceMessageWidget(StringBuilder pageContent, Page page, PageWidget widget)
        {
            string jsonstr = "{'Identifier': '1','ParentId': '8004334234001','Header': '','Message1': 'According to our records your personal-loan account is included in the debt counselling process. Please continue making regular payments as per the agreed payment arrangement. For more information please contact us on 0860 109 279 or email us at DebtCounsellingQueries@nedbank.co.za.','Message2': '','Message3': 'Insurance Personalise text message 1','Message4': 'Insurance Personalise text message 2','Message5': 'Insurance Personalise text message 3'}";
            if (jsonstr != string.Empty && validationEngine.IsValidJson(jsonstr))
            {
                var InsuranceMsg = JsonConvert.DeserializeObject<SpecialMessage>(jsonstr);
                if (InsuranceMsg != null)
                {
                    var InsuranceMsgTxtData = (!string.IsNullOrEmpty(InsuranceMsg.Message3) ? "<p> " + InsuranceMsg.Message3 + " </p>" : string.Empty) +
                        (!string.IsNullOrEmpty(InsuranceMsg.Message4) ? "<p> " + InsuranceMsg.Message4 + " </p>" : string.Empty) +
                        (!string.IsNullOrEmpty(InsuranceMsg.Message5) ? "<p> " + InsuranceMsg.Message5 + " </p>" : string.Empty);

                    pageContent.Replace("{{InsuranceMessages_" + page.Identifier + "_" + widget.Identifier + "}}", InsuranceMsgTxtData);
                }
            }
        }

        #endregion

        #region Bind dummy data for Image and Video widget

        private void BindDummyDataToImageWidget(StringBuilder pageContent, Statement statement, Page page, PageWidget widget, List<FileData> SampleFiles, string AppBaseDirectory, string tenantCode)
        {
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
                        fileData.FileName = "Image" + page.Identifier + widget.Identifier + ".jpg";
                        fileData.FileUrl = asset.FilePath;
                        SampleFiles.Add(fileData);
                        imageAssetPath = "./" + fileData.FileName;
                    }
                }
            }
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
                        fileData.FileName = "Video" + page.Identifier + widget.Identifier + ".jpg";
                        fileData.FileUrl = asset.FilePath;
                        SampleFiles.Add(fileData);
                        videoAssetPath = "./" + fileData.FileName;
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

        #endregion
    }
}