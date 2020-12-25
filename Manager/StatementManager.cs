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
            StringBuilder tempHtml = new StringBuilder();
            string finalHtml = "";
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
                        string navbarHtml = HtmlConstants.NAVBAR_HTML_FOR_PREVIEW.Replace("{{logo}}", "assets/images/nisLogo.png");
                        navbarHtml = navbarHtml.Replace("{{Today}}", DateTime.Now.ToString("dd MMM yyyy"));
                        StringBuilder navItemList = new StringBuilder();
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
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return finalHtml;
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
                            PageSearchParameter pageSearchParameter = new PageSearchParameter
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
                            };
                            var pages = this.pageRepository.GetPages(pageSearchParameter, tenantCode);
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
                                        IList<DynamicWidget> dynawidgets = new List<DynamicWidget>();
                                        if (dynamicwidgetids != string.Empty)
                                        {
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
                                            dynawidgets = this.dynamicWidgetManager.GetDynamicWidgets(dynamicWidgetSearchParameter, tenantCode);
                                        }
                                        statementPageContent.DynamicWidgets = dynawidgets;

                                        //current Y position variable to filter widgets, start with 0
                                        int currentYPosition = 0;
                                        var isRowComplete = false;

                                        while (completelst.Count != 0)
                                        {
                                            //filter 1st row widgets from current page widgets..
                                            //get widgets by current Y position
                                            var lst = completelst.Where(it => it.Yposition == currentYPosition).ToList();
                                            if (lst.Count > 0)
                                            {
                                                //find widget which has max height among them and assign it to max Y position variable
                                                max = max + lst.Max(it => it.Height);

                                                //filter widgets which are in between current Y Position and above max Y Position
                                                var _lst = completelst.Where(it => it.Yposition < max && it.Yposition != currentYPosition).ToList();

                                                //merge 2 widgets into single list which are finds by current Y position and 
                                                //then finds with in between current and max Y position
                                                var mergedlst = lst.Concat(_lst).OrderBy(it => it.Xposition).ToList();

                                                //assign current Y position to new max Y position
                                                currentYPosition = max;

                                                //loop over current merge widget list and create empty HTML template
                                                for (int x = 0; x < mergedlst.Count; x++)
                                                {
                                                    //if tempRowWidth equals to zero then create new div with bootstrap row class
                                                    if (tempRowWidth == 0)
                                                    {
                                                        pageHtmlContent.Append("<div class='row pt-2'>");
                                                        isRowComplete = false;
                                                    }

                                                    //get current widget width
                                                    var divLength = mergedlst[x].Width;

                                                    //get current widget height and multiple with 110 as per angular gridster implementation of each column height
                                                    //to find actual height for current widget in pixel
                                                    var divHeight = mergedlst[x].Height * 110 + "px";
                                                    tempRowWidth = tempRowWidth + divLength;

                                                    // If current col-lg class length is greater than 12, 
                                                    //then end parent row class div and then start new row class div
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
                                                    if (!mergedlst[x].IsDynamicWidget)
                                                    {
                                                        if (mergedlst[x].WidgetName == HtmlConstants.CUSTOMER_INFORMATION_WIDGET_NAME)
                                                        {
                                                            string widgetId = "PageWidgetId_" + mergedlst[x].Identifier + "_Counter" + counter.ToString();
                                                            string widgetHTML = HtmlConstants.CUSTOMER_INFORMATION_WIDGET_HTML_FOR_STMT.Replace("{{VideoSource}}", "{{VideoSource_" + statement.Identifier + "_" + page.Identifier + "_" + mergedlst[x].Identifier + "}}");
                                                            widgetHTML = widgetHTML.Replace("{{WidgetDivHeight}}", divHeight);
                                                            widgetHTML = widgetHTML.Replace("{{WidgetId}}", widgetId);
                                                            pageHtmlContent.Append(widgetHTML);

                                                        }
                                                        else if (mergedlst[x].WidgetName == HtmlConstants.ACCOUNT_INFORMATION_WIDGET_NAME)
                                                        {
                                                            string widgetId = "PageWidgetId_" + mergedlst[x].Identifier + "_Counter" + counter.ToString();
                                                            string widgetHTML = HtmlConstants.ACCOUNT_INFORMATION_WIDGET_HTML_FOR_STMT.Replace("{{AccountInfoData}}", "{{AccountInfoData_" + page.Identifier + "_" + mergedlst[x].Identifier + "}}");
                                                            widgetHTML = widgetHTML.Replace("{{WidgetDivHeight}}", divHeight);
                                                            widgetHTML = widgetHTML.Replace("{{WidgetId}}", widgetId);
                                                            pageHtmlContent.Append(widgetHTML);
                                                        }
                                                        else if (mergedlst[x].WidgetName == HtmlConstants.IMAGE_WIDGET_NAME)
                                                        {

                                                            string widgetId = "PageWidgetId_" + mergedlst[x].Identifier + "_Counter" + counter.ToString();
                                                            string widgetHTML = HtmlConstants.IMAGE_WIDGET_HTML_FOR_STMT.Replace("{{ImageSource}}", "{{ImageSource_" + statement.Identifier + "_" + page.Identifier + "_" + mergedlst[x].Identifier + "}}");
                                                            widgetHTML = widgetHTML.Replace("{{WidgetDivHeight}}", divHeight);
                                                            widgetHTML = widgetHTML.Replace("{{WidgetId}}", widgetId);
                                                            if (mergedlst[x].WidgetSetting != string.Empty && validationEngine.IsValidJson(mergedlst[x].WidgetSetting))
                                                            {
                                                                var imageWidgetHtml = string.Empty;
                                                                dynamic widgetSetting = JObject.Parse(mergedlst[x].WidgetSetting);
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
                                                            pageHtmlContent.Append(widgetHTML);
                                                        }
                                                        else if (mergedlst[x].WidgetName == HtmlConstants.VIDEO_WIDGET_NAME)
                                                        {
                                                            string widgetId = "PageWidgetId_" + mergedlst[x].Identifier + "_Counter" + counter.ToString();
                                                            string widgetHTML = HtmlConstants.VIDEO_WIDGET_HTML_FOR_STMT.Replace("{{VideoSource}}", "{{VideoSource_" + statement.Identifier + "_" + page.Identifier + "_" + mergedlst[x].Identifier + "}}");
                                                            widgetHTML = widgetHTML.Replace("{{WidgetDivHeight}}", divHeight);
                                                            widgetHTML = widgetHTML.Replace("{{WidgetId}}", widgetId);
                                                            pageHtmlContent.Append(widgetHTML);
                                                        }
                                                        else if (mergedlst[x].WidgetName == HtmlConstants.SUMMARY_AT_GLANCE_WIDGET_NAME)
                                                        {
                                                            string widgetId = "PageWidgetId_" + mergedlst[x].Identifier + "_Counter" + counter.ToString();
                                                            string widgetHTML = HtmlConstants.SUMMARY_AT_GLANCE_WIDGET_HTML_FOR_STMT.Replace("{{AccountSummary}}", "{{AccountSummary_" + page.Identifier + "_" + mergedlst[x].Identifier + "}}");
                                                            widgetHTML = widgetHTML.Replace("{{WidgetDivHeight}}", divHeight);
                                                            widgetHTML = widgetHTML.Replace("{{WidgetId}}", widgetId);
                                                            pageHtmlContent.Append(widgetHTML);
                                                        }
                                                        else if (mergedlst[x].WidgetName == HtmlConstants.CURRENT_AVAILABLE_BALANCE_WIDGET_NAME)
                                                        {
                                                            string widgetId = "PageWidgetId_" + mergedlst[x].Identifier + "_Counter" + counter.ToString();
                                                            string CurrentAvailBalanceWidget = HtmlConstants.SAVING_CURRENT_AVALABLE_BAL_WIDGET_HTML_FOR_STMT.Replace("{{TotalValue}}", "{{TotalValue_" + page.Identifier + "_" + mergedlst[x].Identifier + "}}").Replace("{{TotalDeposit}}", "{{TotalDeposit_" + page.Identifier + "_" + mergedlst[x].Identifier + "}}").Replace("{{TotalSpend}}", "{{TotalSpend_" + page.Identifier + "_" + mergedlst[x].Identifier + "}}").Replace("{{Savings}}", "{{Savings_" + page.Identifier + "_" + mergedlst[x].Identifier + "}}");
                                                            CurrentAvailBalanceWidget = CurrentAvailBalanceWidget.Replace("{{WidgetId}}", widgetId);
                                                            CurrentAvailBalanceWidget = CurrentAvailBalanceWidget.Replace("{{WidgetDivHeight}}", divHeight);
                                                            pageHtmlContent.Append(CurrentAvailBalanceWidget);
                                                        }
                                                        else if (mergedlst[x].WidgetName == HtmlConstants.SAVING_AVAILABLE_BALANCE_WIDGET_NAME)
                                                        {
                                                            string widgetId = "PageWidgetId_" + mergedlst[x].Identifier + "_Counter" + counter.ToString();
                                                            string SavingAvailBalanceWidget = HtmlConstants.SAVING_CURRENT_AVALABLE_BAL_WIDGET_HTML_FOR_STMT.Replace("{{TotalValue}}", "{{TotalValue_" + page.Identifier + "_" + mergedlst[x].Identifier + "}}").Replace("{{TotalDeposit}}", "{{TotalDeposit_" + page.Identifier + "_" + mergedlst[x].Identifier + "}}").Replace("{{TotalSpend}}", "{{TotalSpend_" + page.Identifier + "_" + mergedlst[x].Identifier + "}}").Replace("{{Savings}}", "{{Savings_" + page.Identifier + "_" + mergedlst[x].Identifier + "}}");
                                                            SavingAvailBalanceWidget = SavingAvailBalanceWidget.Replace("{{WidgetId}}", widgetId);
                                                            SavingAvailBalanceWidget = SavingAvailBalanceWidget.Replace("{{WidgetDivHeight}}", divHeight);
                                                            pageHtmlContent.Append(SavingAvailBalanceWidget);
                                                        }
                                                        else if (mergedlst[x].WidgetName == HtmlConstants.SAVING_TRANSACTION_WIDGET_NAME)
                                                        {
                                                            string widgetId = "PageWidgetId_" + mergedlst[x].Identifier + "_Counter" + counter.ToString();
                                                            string widgetHTML = HtmlConstants.SAVING_TRANSACTION_WIDGET_HTML_FOR_STMT.Replace("{{AccountTransactionDetails}}", "{{AccountTransactionDetails_" + page.Identifier + "_" + mergedlst[x].Identifier + "}}").Replace("{{SelectOption}}", "{{SelectOption_" + page.Identifier + "_" + mergedlst[x].Identifier + "}}");
                                                            widgetHTML = widgetHTML.Replace("{{WidgetDivHeight}}", divHeight);
                                                            widgetHTML = widgetHTML.Replace("{{WidgetId}}", widgetId);
                                                            pageHtmlContent.Append(widgetHTML);
                                                        }
                                                        else if (mergedlst[x].WidgetName == HtmlConstants.CURRENT_TRANSACTION_WIDGET_NAME)
                                                        {
                                                            string widgetId = "PageWidgetId_" + mergedlst[x].Identifier + "_Counter" + counter.ToString();
                                                            string widgetHTML = HtmlConstants.CURRENT_TRANSACTION_WIDGET_HTML_FOR_STMT.Replace("{{AccountTransactionDetails}}", "{{AccountTransactionDetails_" + page.Identifier + "_" + mergedlst[x].Identifier + "}}").Replace("{{SelectOption}}", "{{SelectOption_" + page.Identifier + "_" + mergedlst[x].Identifier + "}}");
                                                            widgetHTML = widgetHTML.Replace("{{WidgetDivHeight}}", divHeight);
                                                            widgetHTML = widgetHTML.Replace("{{WidgetId}}", widgetId);
                                                            pageHtmlContent.Append(widgetHTML);
                                                        }
                                                        else if (mergedlst[x].WidgetName == HtmlConstants.TOP_4_INCOME_SOURCE_WIDGET_NAME)
                                                        {
                                                            string widgetId = "PageWidgetId_" + mergedlst[x].Identifier + "_Counter" + counter.ToString();
                                                            string widgetHTML = HtmlConstants.TOP_4_INCOME_SOURCE_WIDGET_HTML_FOR_STMT.Replace("{{IncomeSourceList}}", "{{IncomeSourceList_" + page.Identifier + "_" + mergedlst[x].Identifier + "}}");
                                                            widgetHTML = widgetHTML.Replace("{{WidgetDivHeight}}", divHeight);
                                                            widgetHTML = widgetHTML.Replace("{{WidgetId}}", widgetId);
                                                            pageHtmlContent.Append(widgetHTML);
                                                        }
                                                        else if (mergedlst[x].WidgetName == HtmlConstants.ANALYTICS_WIDGET_NAME)
                                                        {
                                                            string widgetId = "PageWidgetId_" + mergedlst[x].Identifier + "_Counter" + counter.ToString();
                                                            string widgetHTML = HtmlConstants.ANALYTIC_WIDGET_HTML_FOR_STMT;
                                                            widgetHTML = widgetHTML.Replace("{{WidgetDivHeight}}", divHeight);
                                                            widgetHTML = widgetHTML.Replace("{{WidgetId}}", widgetId);
                                                            pageHtmlContent.Append(widgetHTML);
                                                        }
                                                        else if (mergedlst[x].WidgetName == HtmlConstants.SPENDING_TREND_WIDGET_NAME)
                                                        {
                                                            string widgetId = "PageWidgetId_" + mergedlst[x].Identifier + "_Counter" + counter.ToString();
                                                            string widgetHTML = HtmlConstants.SPENDING_TRENDS_WIDGET_HTML_FOR_STMT;
                                                            widgetHTML = widgetHTML.Replace("{{WidgetDivHeight}}", divHeight);
                                                            widgetHTML = widgetHTML.Replace("{{WidgetId}}", widgetId);
                                                            pageHtmlContent.Append(widgetHTML);
                                                        }
                                                        else if (mergedlst[x].WidgetName == HtmlConstants.SAVING_TREND_WIDGET_NAME)
                                                        {
                                                            string widgetId = "PageWidgetId_" + mergedlst[x].Identifier + "_Counter" + counter.ToString();
                                                            string widgetHTML = HtmlConstants.SAVING_TRENDS_WIDGET_HTML_FOR_STMT;
                                                            widgetHTML = widgetHTML.Replace("{{WidgetDivHeight}}", divHeight);
                                                            widgetHTML = widgetHTML.Replace("{{WidgetId}}", widgetId);
                                                            pageHtmlContent.Append(widgetHTML);
                                                        }
                                                        else if (mergedlst[x].WidgetName == HtmlConstants.REMINDER_AND_RECOMMENDATION_WIDGET_NAME)
                                                        {
                                                            string widgetId = "PageWidgetId_" + mergedlst[x].Identifier + "_Counter" + counter.ToString();
                                                            string widgetHTML = HtmlConstants.REMINDER_WIDGET_HTML_FOR_STMT.Replace("{{ReminderAndRecommdationDataList}}", "{{ReminderAndRecommdationDataList_" + page.Identifier + "_" + mergedlst[x].Identifier + "}}");
                                                            widgetHTML = widgetHTML.Replace("{{WidgetDivHeight}}", divHeight);
                                                            widgetHTML = widgetHTML.Replace("{{WidgetId}}", widgetId);
                                                            pageHtmlContent.Append(widgetHTML);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (dynawidgets.Count > 0)
                                                        {
                                                            var dynawidget = dynawidgets.Where(item => item.Identifier == mergedlst[x].WidgetId).ToList().FirstOrDefault();

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

                                                            if (dynawidget.WidgetType == HtmlConstants.TABLE_DYNAMICWIDGET)
                                                            {
                                                                var htmlWidget = HtmlConstants.TABLE_WIDEGT_FOR_STMT;
                                                                htmlWidget = htmlWidget.Replace("{{WidgetDivHeight}}", divHeight);
                                                                htmlWidget = htmlWidget.Replace("{{TableMaxHeight}}", (mergedlst[x].Height * 110) - 40 + "px");
                                                                htmlWidget = this.ApplyStyleCssForDynamicTableAndFormWidget(htmlWidget, themeDetails);
                                                                htmlWidget = htmlWidget.Replace("{{WidgetTitle}}", dynawidget.Title);
                                                                htmlWidget = htmlWidget.Replace("{{WidgetId}}", "PageWidgetId_" + mergedlst[x].Identifier + "_Counter" + counter.ToString());
                                                                List<DynamicWidgetTableEntity> tableEntities = JsonConvert.DeserializeObject<List<DynamicWidgetTableEntity>>(dynawidget.WidgetSettings);
                                                                StringBuilder tableHeader = new StringBuilder();
                                                                tableHeader.Append("<tr>" + string.Join("", tableEntities.Select(field => string.Format("<th>{0}</th> ", field.HeaderName))) + "</tr>");
                                                                htmlWidget = htmlWidget.Replace("{{tableHeader}}", tableHeader.ToString());
                                                                htmlWidget = htmlWidget.Replace("{{tableBody}}", "{{tableBody_" + page.Identifier + "_" + mergedlst[x].Identifier + "}}");
                                                                pageHtmlContent.Append(htmlWidget);
                                                            }
                                                            else if (dynawidget.WidgetType == HtmlConstants.FORM_DYNAMICWIDGET)
                                                            {
                                                                var htmlWidget = HtmlConstants.FORM_WIDGET_FOR_STMT;
                                                                htmlWidget = htmlWidget.Replace("{{WidgetDivHeight}}", divHeight);
                                                                htmlWidget = this.ApplyStyleCssForDynamicTableAndFormWidget(htmlWidget, themeDetails);
                                                                htmlWidget = htmlWidget.Replace("{{WidgetTitle}}", dynawidget.Title);
                                                                htmlWidget = htmlWidget.Replace("{{WidgetId}}", "PageWidgetId_" + mergedlst[x].Identifier + "_Counter" + counter.ToString());
                                                                htmlWidget = htmlWidget.Replace("{{FormData}}", "{{FormData_" + page.Identifier + "_" + mergedlst[x].Identifier + "}}");
                                                                pageHtmlContent.Append(htmlWidget);
                                                            }
                                                            else if (dynawidget.WidgetType == HtmlConstants.LINEGRAPH_DYNAMICWIDGET)
                                                            {
                                                                var htmlWidget = HtmlConstants.LINE_GRAPH_FOR_STMT;
                                                                htmlWidget = htmlWidget.Replace("lineGraphcontainer", "lineGraphcontainer_" + page.Identifier + "_" + mergedlst[x].Identifier + "");
                                                                htmlWidget = htmlWidget.Replace("{{WidgetDivHeight}}", divHeight);
                                                                htmlWidget = this.ApplyStyleCssForDynamicGraphAndChartWidgets(htmlWidget, themeDetails);
                                                                htmlWidget = htmlWidget.Replace("{{WidgetTitle}}", dynawidget.Title);
                                                                htmlWidget = htmlWidget.Replace("{{WidgetId}}", "PageWidgetId_" + mergedlst[x].Identifier + "_Counter" + counter.ToString());
                                                                htmlWidget = htmlWidget + "<input type='hidden' id='hiddenLineGraphData_" + page.Identifier + "_" + mergedlst[x].Identifier + "' value='hiddenLineGraphValue_" + page.Identifier + "_" + mergedlst[x].Identifier + "'>";
                                                                pageHtmlContent.Append(htmlWidget);
                                                            }
                                                            else if (dynawidget.WidgetType == HtmlConstants.BARGRAPH_DYNAMICWIDGET)
                                                            {
                                                                var htmlWidget = HtmlConstants.BAR_GRAPH_FOR_STMT;
                                                                htmlWidget = htmlWidget.Replace("{{WidgetDivHeight}}", divHeight);
                                                                htmlWidget = htmlWidget.Replace("barGraphcontainer", "barGraphcontainer_" + page.Identifier + "_" + mergedlst[x].Identifier + "");
                                                                htmlWidget = this.ApplyStyleCssForDynamicGraphAndChartWidgets(htmlWidget, themeDetails);
                                                                htmlWidget = htmlWidget.Replace("{{WidgetTitle}}", dynawidget.Title);
                                                                htmlWidget = htmlWidget.Replace("{{WidgetId}}", "PageWidgetId_" + mergedlst[x].Identifier + "_Counter" + counter.ToString());
                                                                htmlWidget = htmlWidget + "<input type='hidden' id='hiddenBarGraphData_" + page.Identifier + "_" + mergedlst[x].Identifier + "' value='hiddenBarGraphValue_" + page.Identifier + "_" + mergedlst[x].Identifier + "'>";
                                                                pageHtmlContent.Append(htmlWidget);
                                                            }
                                                            else if (dynawidget.WidgetType == HtmlConstants.PICHART_DYNAMICWIDGET)
                                                            {
                                                                var htmlWidget = HtmlConstants.PIE_CHART_FOR_STMT;
                                                                htmlWidget = htmlWidget.Replace("{{WidgetDivHeight}}", divHeight);
                                                                htmlWidget = htmlWidget.Replace("pieChartcontainer", "pieChartcontainer_" + page.Identifier + "_" + mergedlst[x].Identifier + "");
                                                                htmlWidget = this.ApplyStyleCssForDynamicGraphAndChartWidgets(htmlWidget, themeDetails);
                                                                htmlWidget = htmlWidget.Replace("{{WidgetTitle}}", dynawidget.Title);
                                                                htmlWidget = htmlWidget.Replace("{{WidgetId}}", "PageWidgetId_" + mergedlst[x].Identifier + "_Counter" + counter.ToString());
                                                                htmlWidget = htmlWidget + "<input type='hidden' id='hiddenPieChartData_" + page.Identifier + "_" + mergedlst[x].Identifier + "' value='hiddenPieChartValue_" + page.Identifier + "_" + mergedlst[x].Identifier + "'>";
                                                                pageHtmlContent.Append(htmlWidget);
                                                            }
                                                            else if (dynawidget.WidgetType == HtmlConstants.HTML_DYNAMICWIDGET)
                                                            {
                                                                var htmlWidget = HtmlConstants.HTML_WIDGET_FOR_STMT;
                                                                htmlWidget = htmlWidget.Replace("{{WidgetDivHeight}}", divHeight);
                                                                htmlWidget = this.ApplyStyleCssForDynamicTableAndFormWidget(htmlWidget, themeDetails);
                                                                htmlWidget = htmlWidget.Replace("{{WidgetTitle}}", dynawidget.Title);
                                                                htmlWidget = htmlWidget.Replace("{{WidgetId}}", "PageWidgetId_" + mergedlst[x].Identifier + "_Counter" + counter.ToString());
                                                                htmlWidget = htmlWidget.Replace("{{FormData}}", "{{FormData_" + page.Identifier + "_" + mergedlst[x].Identifier + "}}");
                                                                pageHtmlContent.Append(htmlWidget);
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
                StatementPreviewData statementPreviewData = new StatementPreviewData();

                //start to render common html content data
                StringBuilder htmlbody = new StringBuilder();
                string navbarHtml = HtmlConstants.NAVBAR_HTML_FOR_PREVIEW.Replace("{{logo}}", "../common/images/nisLogo.png");
                navbarHtml = navbarHtml.Replace("{{Today}}", DateTime.UtcNow.ToString("dd MMM yyyy")); //bind current date to html header

                //get client logo in string format and pass it hidden input tag, so it will be render in right side of header of html statement
                var clientlogo = client.TenantLogo != null ? client.TenantLogo : "";
                navbarHtml = navbarHtml + "<input type='hidden' id='TenantLogoImageValue' value='" + clientlogo + "'>";
                htmlbody.Append(HtmlConstants.CONTAINER_DIV_HTML_HEADER);

                //this variable is used to bind all script to html statement, which helps to render data on chart and graph widgets
                StringBuilder scriptHtmlRenderer = new StringBuilder();
                StringBuilder navbar = new StringBuilder();
                var newStatementPageContents = new List<StatementPageContent>();
                IList<FileData> SampleFiles = new List<FileData>();
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
                    StatementPageContent statementPageContent = newStatementPageContents.Where(item => item.PageTypeId == page.PageTypeId && item.Id == i).FirstOrDefault();
                    StringBuilder pageContent = new StringBuilder(statementPageContent.HtmlContent);
                    var dynamicWidgets = statementPageContent.DynamicWidgets;

                    StringBuilder SubTabs = new StringBuilder();
                    StringBuilder PageHeaderContent = new StringBuilder(statementPageContent.PageHeaderContent);

                    string tabClassName = Regex.Replace((statementPageContent.DisplayName + "-" + page.Identifier), @"\s+", "-");
                    navbar.Append(" <li class='nav-item'><a class='nav-link pt-1 mainNav " + (i == 0 ? "active" : "") + " " + tabClassName + "' href='javascript:void(0);' >" + statementPageContent.DisplayName + "</a> </li> ");
                    string ExtraClassName = i > 0 ? "d-none " + tabClassName : tabClassName;
                    PageHeaderContent.Replace("{{ExtraClass}}", ExtraClassName);
                    PageHeaderContent.Replace("{{DivId}}", tabClassName);

                    StringBuilder newPageContent = new StringBuilder();
                    newPageContent.Append(HtmlConstants.PAGE_TAB_CONTENT_HEADER);

                    if (page.PageTypeName == HtmlConstants.SAVING_ACCOUNT_PAGE || page.PageTypeName == HtmlConstants.CURRENT_ACCOUNT_PAGE)
                    {
                        SubTabs.Append("<ul class='nav nav-tabs' style='margin-top:-20px;'>");
                        SubTabs.Append("<li class='nav-item active'><a id='tab1-tab' data-toggle='tab' " + "data-target='#" + (page.PageTypeName == HtmlConstants.SAVING_ACCOUNT_PAGE ? "Saving" : "Current") + "-' role='tab' class='nav-link active'> Account - 6789</a></li>");
                        SubTabs.Append("</ul>");

                        newPageContent.Append("<div id='" + (page.PageTypeName == HtmlConstants.SAVING_ACCOUNT_PAGE ? "Saving" : "Current") + "-6789' class='tab-pane fade in active show'>");
                    }

                    var pagewidgets = new List<PageWidget>(page.PageWidgets);
                    for (int j = 0; j < pagewidgets.Count; j++)
                    {
                        var widget = pagewidgets[j];
                        if (!widget.IsDynamicWidget)
                        {
                            if (widget.WidgetName == HtmlConstants.CUSTOMER_INFORMATION_WIDGET_NAME)
                            {
                                string customerInfoJson = "{'FirstName':'Laura','MiddleName':'J','LastName':'Donald','AddressLine1':'4000 Executive Parkway', 'AddressLine2':'Saint Globin Rd','City':'Canary Wharf', 'State':'London', 'Country':'England','Zip':'E14 9RZ'}";
                                if (customerInfoJson != string.Empty && validationEngine.IsValidJson(customerInfoJson))
                                {
                                    CustomerInformation customerInfo = JsonConvert.DeserializeObject<CustomerInformation>(customerInfoJson);
                                    pageContent.Replace("{{VideoSource_" + statement.Identifier + "_" + page.Identifier + "_" + widget.Identifier + "}}", AppBaseDirectory + "\\Resources\\sampledata\\SampleVideo.mp4");

                                    string customerName = customerInfo.FirstName + " " + customerInfo.MiddleName + " " + customerInfo.LastName;
                                    pageContent.Replace("{{CustomerName}}", customerName);

                                    string address1 = customerInfo.AddressLine1 + ", " + customerInfo.AddressLine2 + ", ";
                                    pageContent.Replace("{{Address1}}", address1);

                                    string address2 = (customerInfo.City != "" ? customerInfo.City + ", " : "") + (customerInfo.State != "" ? customerInfo.State + ", " : "") + (customerInfo.Country != "" ? customerInfo.Country + ", " : "") + (customerInfo.Zip != "" ? customerInfo.Zip : "");
                                    pageContent.Replace("{{Address2}}", address2);
                                }
                            }
                            else if (widget.WidgetName == HtmlConstants.ACCOUNT_INFORMATION_WIDGET_NAME)
                            {
                                string accountInfoJson = "{'StatementDate':'1-APR-2020','StatementPeriod':'Annual Statement', 'CustomerID':'ID2-8989-5656','RmName':'James Wiilims','RmContactNumber':'+4487867833'}";

                                string accountInfoData = string.Empty;
                                StringBuilder AccDivData = new StringBuilder();
                                if (accountInfoJson != string.Empty && validationEngine.IsValidJson(accountInfoJson))
                                {
                                    AccountInformation accountInfo = JsonConvert.DeserializeObject<AccountInformation>(accountInfoJson);
                                    AccDivData.Append("<div class='list-row-small ht70px'><div class='list-middle-row'> <div class='list-text'>" + "Statement Date</div><label class='list-value mb-0'>" + accountInfo.StatementDate + "</label>" + "</div></div>");

                                    AccDivData.Append("<div class='list-row-small ht70px'><div class='list-middle-row'> <div class='list-text'>" + "Statement Period</div><label class='list-value mb-0'>" + accountInfo.StatementPeriod + "</label></div></div>");

                                    AccDivData.Append("<div class='list-row-small ht70px'><div class='list-middle-row'> <div class='list-text'>" + "Cusomer ID</div><label class='list-value mb-0'>" + accountInfo.CustomerID + "</label></div></div>");

                                    AccDivData.Append("<div class='list-row-small ht70px'><div class='list-middle-row'> <div class='list-text'>" + "RM Name</div><label class='list-value mb-0'>" + accountInfo.RmName + "</label></div></div>");

                                    AccDivData.Append("<div class='list-row-small ht70px'><div class='list-middle-row'> <div class='list-text'>" + "RM Contact Number</div><label class='list-value mb-0'>" + accountInfo.RmContactNumber + "</label></div></div>");
                                }
                                pageContent.Replace("{{AccountInfoData_" + page.Identifier + "_" + widget.Identifier + "}}", AccDivData.ToString());
                            }
                            else if (widget.WidgetName == HtmlConstants.IMAGE_WIDGET_NAME)
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
                                            FileData fileData = new FileData();
                                            fileData.FileName = "Image" + page.Identifier + widget.Identifier + ".jpg";
                                            fileData.FileUrl = asset.FilePath; //baseURL + "/assets/" + widgetSetting.AssetLibraryId + "/" + widgetSetting.AssetName;
                                            SampleFiles.Add(fileData);
                                            imageAssetPath = "./" + fileData.FileName;
                                        }
                                    }
                                }
                                pageContent.Replace("{{ImageSource_" + statement.Identifier + "_" + page.Identifier + "_" + widget.Identifier + "}}", imageAssetPath);
                            }
                            else if (widget.WidgetName == HtmlConstants.VIDEO_WIDGET_NAME)
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
                                            FileData fileData = new FileData();
                                            fileData.FileName = "Video" + page.Identifier + widget.Identifier + ".jpg";
                                            fileData.FileUrl = asset.FilePath;
                                            SampleFiles.Add(fileData);
                                            videoAssetPath = "./" + fileData.FileName;
                                        }
                                    }
                                }
                                pageContent.Replace("{{VideoSource_" + statement.Identifier + "_" + page.Identifier + "_" + widget.Identifier + "}}", videoAssetPath);
                            }
                            else if (widget.WidgetName == HtmlConstants.SUMMARY_AT_GLANCE_WIDGET_NAME)
                            {
                                string accountBalanceDataJson = "[{\"AccountType\":\"Saving Account\",\"Currency\":\"$\",\"Amount\":\"87356\"}" +
                                    ",{\"AccountType\":\"Current Account\",\"Currency\":\"$\",\"Amount\":\"18654\"},{\"AccountType\":" +
                                    "\"Recurring Account\",\"Currency\":\"$\",\"Amount\":\"54367\"},{\"AccountType\":\"Wealth\",\"Currency\"" + ":\"$\",\"Amount\":\"4589\"}]";

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
                                        pageContent.Replace("{{AccountSummary_" + page.Identifier + "_" + widget.Identifier + "}}", accSummary.ToString());
                                    }
                                }
                            }
                            else if (widget.WidgetName == HtmlConstants.CURRENT_AVAILABLE_BALANCE_WIDGET_NAME)
                            {
                                string currentAvailBalanceJson = "{'GrandTotal':'32,453,23', 'TotalDeposit':'16,250,00', 'TotalSpend':'16,254,00', 'ProfitEarned':'1,430,00 ', 'Currency':'$', 'Balance': '14,768,80', 'AccountNumber': 'J566565TR678ER', 'AccountType': 'Current', 'Indicator': 'Up'}";
                                if (currentAvailBalanceJson != string.Empty && validationEngine.IsValidJson(currentAvailBalanceJson))
                                {
                                    AccountMaster accountMaster = JsonConvert.DeserializeObject<AccountMaster>(currentAvailBalanceJson);
                                    var accountIndicatorClass = accountMaster.Indicator.ToLower().Equals("up") ? "fa fa-sort-asc text-success" : "fa fa-sort-desc text-danger";
                                    pageContent.Replace("{{AccountIndicatorClass}}", accountIndicatorClass);
                                    pageContent.Replace("{{TotalValue_" + page.Identifier + "_" + widget.Identifier + "}}", (accountMaster.Currency + accountMaster.GrandTotal));
                                    pageContent.Replace("{{TotalDeposit_" + page.Identifier + "_" + widget.Identifier + "}}", (accountMaster.Currency + accountMaster.TotalDeposit));
                                    pageContent.Replace("{{TotalSpend_" + page.Identifier + "_" + widget.Identifier + "}}", (accountMaster.Currency + accountMaster.TotalSpend));
                                    pageContent.Replace("{{Savings_" + page.Identifier + "_" + widget.Identifier + "}}", (accountMaster.Currency + accountMaster.ProfitEarned));
                                }
                            }
                            else if (widget.WidgetName == HtmlConstants.SAVING_AVAILABLE_BALANCE_WIDGET_NAME)
                            {
                                string savingAvailBalanceJson = "{'GrandTotal':'26,453,23', 'TotalDeposit':'13,530,00', 'TotalSpend':'12,124,00', 'ProfitEarned':'2,340,00 ', 'Currency':'$', 'Balance': '19,456,80', 'AccountNumber': 'J566565TR678ER', 'AccountType': 'Saving', 'Indicator': 'Up'}";
                                if (savingAvailBalanceJson != string.Empty && validationEngine.IsValidJson(savingAvailBalanceJson))
                                {
                                    AccountMaster accountMaster = JsonConvert.DeserializeObject<AccountMaster>(savingAvailBalanceJson);
                                    var accountIndicatorClass = accountMaster.Indicator.ToLower().Equals("up") ? "fa fa-sort-asc text-success" : "fa fa-sort-desc text-danger";
                                    pageContent.Replace("{{AccountIndicatorClass}}", accountIndicatorClass);
                                    pageContent.Replace("{{TotalValue_" + page.Identifier + "_" + widget.Identifier + "}}", (accountMaster.Currency + accountMaster.GrandTotal));
                                    pageContent.Replace("{{TotalDeposit_" + page.Identifier + "_" + widget.Identifier + "}}", (accountMaster.Currency + accountMaster.TotalDeposit));
                                    pageContent.Replace("{{TotalSpend_" + page.Identifier + "_" + widget.Identifier + "}}", (accountMaster.Currency + accountMaster.TotalSpend));
                                    pageContent.Replace("{{Savings_" + page.Identifier + "_" + widget.Identifier + "}}", (accountMaster.Currency + accountMaster.ProfitEarned));
                                }
                            }
                            else if (widget.WidgetName == HtmlConstants.SAVING_TRANSACTION_WIDGET_NAME)
                            {
                                StringBuilder selectOption = new StringBuilder();
                                var distinctNaration = new string[] { "NXT TXN: IIFL IIFL6574562", "NXT TXN: IIFL IIFL6574563", "NXT TXN: IIFL IIFL3557346", "NXT TXN: IIFL RTED87978947 REFUND", "NXT TXN: IIFL IIFL896452896ERE", "NXT TXN: IIFL IIFL8965435", "NXT TXN: IIFL FGTR454565JHGKD", "NXT TXN: OFFICE RENT 798789DFGH", "NXT TXN: IIFL IIFL0034212", "NXT TXN: IIFL IIFL045678DFGH" };
                                distinctNaration.ToList().ForEach(item =>
                                {
                                    selectOption.Append("<option value='" + item + "'> " + item + "</option>");
                                });

                                FileData fileData = new FileData();
                                fileData.FileName = "savingtransactiondetail.json";
                                fileData.FileUrl = AppBaseDirectory + "\\Resources\\sampledata\\savingtransactiondetail.json";
                                SampleFiles.Add(fileData);
                                scriptHtmlRenderer.Append("<script type='text/javascript' src='./" + fileData.FileName + "'></script>");
                                //scriptHtmlRenderer.Append("<script type='text/javascript' src='" + AppBaseDirectory + "\\Resources\\sampledata\\savingtransactiondetail.json'></script>");
                                StringBuilder scriptval = new StringBuilder(HtmlConstants.SAVING_TRANSACTION_DETAIL_GRID_WIDGET_SCRIPT);
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
                            else if (widget.WidgetName == HtmlConstants.CURRENT_TRANSACTION_WIDGET_NAME)
                            {
                                StringBuilder selectOption = new StringBuilder();
                                var distinctNaration = new string[] { "NXT TXN: IIFL IIFL6574562", "NXT TXN: IIFL IIFL6574563", "NXT TXN: IIFL IIFL3557346", "NXT TXN: IIFL RTED87978947 REFUND", "NXT TXN: IIFL IIFL896452896ERE", "NXT TXN: IIFL IIFL8965435", "NXT TXN: IIFL FGTR454565JHGKD", "NXT TXN: OFFICE RENT 798789DFGH", "NXT TXN: IIFL IIFL0034212", "NXT TXN: IIFL IIFL045678DFGH" };
                                distinctNaration.ToList().ForEach(item =>
                                {
                                    selectOption.Append("<option value='" + item + "'> " + item + "</option>");
                                });

                                FileData fileData = new FileData();
                                fileData.FileName = "currenttransactiondetail.json";
                                fileData.FileUrl = AppBaseDirectory + "\\Resources\\sampledata\\currenttransactiondetail.json";
                                SampleFiles.Add(fileData);
                                scriptHtmlRenderer.Append("<script type='text/javascript' src='./" + fileData.FileName + "'></script>");
                                //scriptHtmlRenderer.Append("<script type='text/javascript' src='" + AppBaseDirectory + "\\Resources\\sampledata\\currenttransactiondetail.json'></script>");
                                StringBuilder scriptval = new StringBuilder(HtmlConstants.CURRENT_TRANSACTION_DETAIL_GRID_WIDGET_SCRIPT);
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
                            else if (widget.WidgetName == HtmlConstants.TOP_4_INCOME_SOURCE_WIDGET_NAME)
                            {
                                string incomeSourceListJson = "[{ 'Source': 'Salary Transfer', 'CurrentSpend': 3453, 'AverageSpend': 123},{ 'Source': 'Cash Deposit', 'CurrentSpend': 3453, 'AverageSpend': 6123},{ 'Source': 'Profit Earned', 'CurrentSpend': 3453, 'AverageSpend': 6123}, { 'Source': 'Rebete', 'CurrentSpend': 3453, 'AverageSpend': 123}]";
                                if (incomeSourceListJson != string.Empty && validationEngine.IsValidJson(incomeSourceListJson))
                                {
                                    IList<IncomeSources> incomeSources = JsonConvert.DeserializeObject<List<IncomeSources>>(incomeSourceListJson);
                                    StringBuilder incomestr = new StringBuilder();
                                    incomeSources.ToList().ForEach(item =>
                                    {
                                        var tdstring = string.Empty;
                                        if (Int32.Parse(item.CurrentSpend) > Int32.Parse(item.AverageSpend))
                                        {
                                            tdstring = "<span class='fa fa-sort-desc fa-2x text-danger' aria-hidden='true'></span><span class='ml-2'>" + item.AverageSpend + "</span>";
                                        }
                                        else
                                        {
                                            tdstring = "<span class='fa fa-sort-asc fa-2x mt-1' aria-hidden='true' " + "style='position:relative;top:6px;color:limegreen'></span><span class='ml-2'>" + item.AverageSpend + "</span>";
                                        }
                                        incomestr.Append("<tr><td class='float-left'>" + item.Source + "</td>" + "<td> " + item.CurrentSpend + "</td><td>" + tdstring + "</td></tr>");
                                    });
                                    pageContent.Replace("{{IncomeSourceList_" + page.Identifier + "_" + widget.Identifier + "}}", incomestr.ToString());
                                }
                            }
                            else if (widget.WidgetName == HtmlConstants.ANALYTICS_WIDGET_NAME)
                            {
                                FileData fileData = new FileData();
                                fileData.FileName = "analyticschartdata.json";
                                fileData.FileUrl = AppBaseDirectory + "\\Resources\\sampledata\\analyticschartdata.json";
                                SampleFiles.Add(fileData);
                                scriptHtmlRenderer.Append("<script type='text/javascript' src='./" + fileData.FileName + "'></script>");
                                pageContent.Replace("analyticschartcontainer", "analyticschartcontainer" + page.Identifier);
                                scriptHtmlRenderer.Append(HtmlConstants.ANALYTICS_CHART_WIDGET_SCRIPT.Replace("analyticschartcontainer", "analyticschartcontainer" + page.Identifier));
                            }
                            else if (widget.WidgetName == HtmlConstants.SPENDING_TREND_WIDGET_NAME)
                            {
                                FileData fileData = new FileData();
                                fileData.FileName = "spendingtrenddata.json";
                                fileData.FileUrl = AppBaseDirectory + "\\Resources\\sampledata\\spendingtrenddata.json";
                                SampleFiles.Add(fileData);
                                scriptHtmlRenderer.Append("<script type='text/javascript' src='" + fileData.FileName + "'></script>");
                                pageContent.Replace("spendingTrendscontainer", "spendingTrendscontainer" + page.Identifier);
                                scriptHtmlRenderer.Append(HtmlConstants.SPENDING_TREND_CHART_WIDGET_SCRIPT.Replace("spendingTrendscontainer", "spendingTrendscontainer" + page.Identifier));
                            }
                            else if (widget.WidgetName == HtmlConstants.SAVING_TREND_WIDGET_NAME)
                            {
                                FileData fileData = new FileData();
                                fileData.FileName = "savingtrenddata.json";
                                fileData.FileUrl = AppBaseDirectory + "\\Resources\\sampledata\\savingtrenddata.json";
                                SampleFiles.Add(fileData);
                                scriptHtmlRenderer.Append("<script type='text/javascript' src='" + fileData.FileName + "'></script>");
                                pageContent.Replace("savingTrendscontainer", "savingTrendscontainer" + page.Identifier);
                                scriptHtmlRenderer.Append(HtmlConstants.SAVING_TREND_CHART_WIDGET_SCRIPT.Replace("savingTrendscontainer", "savingTrendscontainer" + page.Identifier));
                            }
                            else if (widget.WidgetName == HtmlConstants.REMINDER_AND_RECOMMENDATION_WIDGET_NAME)
                            {
                                string reminderJson = "[{ 'Title': 'Update Missing Inofrmation', 'Action': 'Update' },{ 'Title': 'Your Rewards Video is available', 'Action': 'View' },{ 'Title': 'Payment Due for Home Loan', 'Action': 'Pay' }, { title: 'Need financial planning for savings.', action: 'Call Me' },{ title: 'Subscribe/Unsubscribe Alerts.', action: 'Apply' },{ title: 'Your credit card payment is due now.', action: 'Pay' }]";
                                if (reminderJson != string.Empty && validationEngine.IsValidJson(reminderJson))
                                {
                                    IList<ReminderAndRecommendation> reminderAndRecommendations = JsonConvert.DeserializeObject<List<ReminderAndRecommendation>>(reminderJson);
                                    StringBuilder reminderstr = new StringBuilder();
                                    reminderstr.Append("<div class='row'><div class='col-lg-9'></div><div class='col-lg-3 text-left'><i class='fa fa-caret-left fa-3x float-left text-danger' aria-hidden='true'></i><span class='mt-2 d-inline-block ml-2'>Click</span></div> </div>");
                                    reminderAndRecommendations.ToList().ForEach(item =>
                                    {
                                        reminderstr.Append("<div class='row'><div class='col-lg-9 text-left'><p class='p-1' style='background-color: #dce3dc;'>" +
                                            item.Title + " </p></div><div class='col-lg-3 text-left'> <a><i class='fa fa-caret-left fa-3x float-left " +
                                            "text-danger'></i><span class='mt-2 d-inline-block ml-2'>" + item.Action + "</span></a></div></div>");
                                    });
                                    pageContent.Replace("{{ReminderAndRecommdationDataList_" + page.Identifier + "_" + widget.Identifier + "}}", reminderstr.ToString());
                                }
                            }
                        }
                        else
                        {
                            var dynaWidgets = dynamicWidgets.Where(item => item.Identifier == widget.WidgetId).ToList();
                            if (dynaWidgets.Count > 0)
                            {
                                var dynawidget = dynaWidgets.FirstOrDefault();
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
                                    pageContent.Replace("{{tableBody_" + page.Identifier + "_" + widget.Identifier + "}}", dynawidget.PreviewData);
                                }
                                else if (dynawidget.WidgetType == HtmlConstants.FORM_DYNAMICWIDGET)
                                {
                                    pageContent.Replace("{{FormData_" + page.Identifier + "_" + widget.Identifier + "}}", dynawidget.PreviewData);
                                }
                                else if (dynawidget.WidgetType == HtmlConstants.LINEGRAPH_DYNAMICWIDGET)
                                {
                                    pageContent.Replace("hiddenLineGraphValue_" + page.Identifier + "_" + widget.Identifier + "", dynawidget.PreviewData);
                                    scriptHtmlRenderer.Append(HtmlConstants.LINE_GRAPH_WIDGET_SCRIPT.Replace("linechartcontainer", "lineGraphcontainer_" + page.Identifier + "_" + widget.Identifier).Replace("hiddenLineGraphData", "hiddenLineGraphData_" + page.Identifier + "_" + widget.Identifier));
                                }
                                else if (dynawidget.WidgetType == HtmlConstants.BARGRAPH_DYNAMICWIDGET)
                                {
                                    pageContent.Replace("hiddenBarGraphValue_" + page.Identifier + "_" + widget.Identifier + "", dynawidget.PreviewData);
                                    scriptHtmlRenderer.Append(HtmlConstants.BAR_GRAPH_WIDGET_SCRIPT.Replace("barchartcontainer", "barGraphcontainer_" + page.Identifier + "_" + widget.Identifier).Replace("hiddenBarGraphData", "hiddenBarGraphData_" + page.Identifier + "_" + widget.Identifier));
                                }
                                else if (dynawidget.WidgetType == HtmlConstants.PICHART_DYNAMICWIDGET)
                                {
                                    pageContent.Replace("hiddenPieChartValue_" + page.Identifier + "_" + widget.Identifier + "", dynawidget.PreviewData);
                                    scriptHtmlRenderer.Append(HtmlConstants.PIE_CHART_WIDGET_SCRIPT.Replace("pieChartcontainer", "pieChartcontainer_" + page.Identifier + "_" + widget.Identifier).Replace("hiddenPieChartData", "hiddenPieChartData_" + page.Identifier + "_" + widget.Identifier));
                                }
                                else if (dynawidget.WidgetType == HtmlConstants.HTML_DYNAMICWIDGET)
                                {
                                    var entityFieldMaps = this.dynamicWidgetManager.GetEntityFields(dynawidget.EntityId, tenantCode);
                                    string data = this.GetHTMLPreviewData(entity, entityFieldMaps, dynawidget.PreviewData);
                                    pageContent.Replace("{{FormData_" + page.Identifier + "_" + widget.Identifier + "}}", data);
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

                statementPreviewData.SampleFiles = SampleFiles;
                statementPreviewData.FileContent = finalHtml.ToString();
                return statementPreviewData;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ScheduleLogDetail GenerateStatements(CustomerMaster customer, Statement statement, IList<StatementPageContent> statementPageContents, BatchMaster batchMaster, IList<BatchDetail> batchDetails, string baseURL, string tenantCode, string outputLocation, TenantConfiguration tenantConfiguration, Client client, IList<TenantEntity> tenantEntities)
        {
            ScheduleLogDetail logDetailRecord = new ScheduleLogDetail();
            StringBuilder ErrorMessages = new StringBuilder();
            bool IsFailed = false;
            bool IsSavingOrCurrentAccountPagePresent = false;
            var statementMetadataRecords = new List<StatementMetadata>();

            try
            {
                if (statementPageContents.Count > 0)
                {
                    string currency = string.Empty;
                    IList<AccountMaster> accountrecords = new List<AccountMaster>();
                    var savingaccountrecords = new List<AccountMaster>();
                    var curerntaccountrecords = new List<AccountMaster>();
                    IList<CustomerMedia> customerMedias = new List<CustomerMedia>();
                    IList<AccountTransaction> CustomerAcccountTransactions = new List<AccountTransaction>();
                    IList<SavingTrend> CustomerSavingTrends = new List<SavingTrend>();

                    var pages = statement.Pages.Where(item => item.PageTypeName == HtmlConstants.SAVING_ACCOUNT_PAGE || item.PageTypeName == HtmlConstants.CURRENT_ACCOUNT_PAGE).ToList();
                    IsSavingOrCurrentAccountPagePresent = pages.Count > 0 ? true : false;

                    //collecting all required transaction required for static widgets in financial tenant html statement
                    if (IsSavingOrCurrentAccountPagePresent)
                    {
                        CustomerAccountSearchParameter customerAccountSearchParameter = new CustomerAccountSearchParameter()
                        {
                            CustomerId = customer.Identifier,
                            BatchId = batchMaster.Identifier
                        };
                        accountrecords = this.tenantTransactionDataManager.Get_AccountMaster(customerAccountSearchParameter, tenantCode);
                        if ((accountrecords == null || accountrecords.Count == 0))
                        {
                            ErrorMessages.Append("<li>Account master data is not available for this customer..!!</li>");
                            IsFailed = true;
                        }
                        else
                        {
                            var records = accountrecords.Where(item => item.AccountType.Equals(string.Empty)).ToList();
                            if (records.Count > 0)
                            {
                                ErrorMessages.Append("<li>Invalid account master data for this customer..!!</li>");
                                IsFailed = true;
                            }
                        }

                        CustomerAcccountTransactions = this.tenantTransactionDataManager.Get_AccountTransaction(customerAccountSearchParameter, tenantCode)?.OrderBy(item => item.TransactionDate)?.ToList();

                        CustomerSavingTrends = this.tenantTransactionDataManager.Get_SavingTrend(customerAccountSearchParameter, tenantCode);

                    }

                    //collecting all media information which is required in html statement for some widgets like image, video and static customer information widgets
                    customerMedias = this.tenantTransactionDataManager.GetCustomerMediaList(customer.Identifier, batchMaster.Identifier, statement.Identifier, tenantCode);

                    StringBuilder htmlbody = new StringBuilder();
                    currency = accountrecords.Count > 0 ? accountrecords[0].Currency : string.Empty;
                    string navbarHtml = HtmlConstants.NAVBAR_HTML_FOR_PREVIEW.Replace("{{logo}}", "../common/images/nisLogo.png");
                    navbarHtml = navbarHtml.Replace("{{Today}}", DateTime.UtcNow.ToString("dd MMM yyyy")); //bind current date to html header

                    //get client logo in string format and pass it hidden input tag, so it will be render in right side of header of html statement
                    var clientlogo = client.TenantLogo != null ? client.TenantLogo : "";
                    navbarHtml = navbarHtml + "<input type='hidden' id='TenantLogoImageValue' value='" + clientlogo + "'>";
                    htmlbody.Append(HtmlConstants.CONTAINER_DIV_HTML_HEADER);

                    //this variable is used to bind all script to html statement, which helps to render data on chart and graph widgets
                    StringBuilder scriptHtmlRenderer = new StringBuilder();
                    StringBuilder navbar = new StringBuilder();
                    int subPageCount = 0;
                    string accountNumber = string.Empty; //also use for Subscription
                    string accountType = string.Empty; //also use for vendor name
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
                        StatementPageContent statementPageContent = newStatementPageContents.Where(item => item.PageTypeId == page.PageTypeId && item.Id == i).FirstOrDefault();

                        //sub page count under current page tab
                        subPageCount = 1;
                        if (IsSavingOrCurrentAccountPagePresent)
                        {
                            //This will be applicable only for financial tenant
                            //if cusomer have 2 saving or current account, then 2 tabs will be render to current page in html statement
                            if (page.PageTypeName == HtmlConstants.SAVING_ACCOUNT_PAGE)
                            {
                                savingaccountrecords = accountrecords.Where(item => item.CustomerId == customer.Identifier && item.BatchId == batchMaster.Identifier && item.AccountType.ToLower().Contains("saving"))?.ToList();
                                if (savingaccountrecords == null || savingaccountrecords.Count == 0)
                                {
                                    ErrorMessages.Append("<li>Saving account master data is not available for this customer..!!</li>");
                                    IsFailed = true;
                                }
                                subPageCount = savingaccountrecords.Count;
                            }
                            else if (page.PageTypeName == HtmlConstants.CURRENT_ACCOUNT_PAGE)
                            {
                                curerntaccountrecords = accountrecords.Where(item => item.CustomerId == customer.Identifier && item.BatchId == batchMaster.Identifier && item.AccountType.ToLower().Contains("current"))?.ToList();
                                if (curerntaccountrecords == null || curerntaccountrecords.Count == 0)
                                {
                                    ErrorMessages.Append("<li>Current account master data is not available for this customer..!!</li>");
                                    IsFailed = true;
                                }
                                subPageCount = curerntaccountrecords.Count;
                            }
                        }

                        StringBuilder SubTabs = new StringBuilder();
                        StringBuilder PageHeaderContent = new StringBuilder(statementPageContent.PageHeaderContent);
                        var dynamicWidgets = statementPageContent.DynamicWidgets;

                        string tabClassName = Regex.Replace((statementPageContent.DisplayName + "-" + page.Identifier), @"\s+", "-");
                        navbar.Append(" <li class='nav-item'><a class='nav-link pt-1 mainNav " + (i == 0 ? "active" : "") + " " + tabClassName + "' href='javascript:void(0);' >" + statementPageContent.DisplayName + "</a> </li> ");
                        string ExtraClassName = i > 0 ? "d-none " + tabClassName : tabClassName;
                        PageHeaderContent.Replace("{{ExtraClass}}", ExtraClassName);
                        PageHeaderContent.Replace("{{DivId}}", tabClassName);

                        StringBuilder newPageContent = new StringBuilder();
                        newPageContent.Append(HtmlConstants.PAGE_TAB_CONTENT_HEADER);

                        for (int x = 0; x < subPageCount; x++)
                        {
                            accountNumber = string.Empty;
                            accountType = string.Empty;

                            //Only for financial tenant
                            if (IsSavingOrCurrentAccountPagePresent)
                            {
                                if (page.PageTypeName == HtmlConstants.SAVING_ACCOUNT_PAGE)
                                {
                                    accountNumber = savingaccountrecords[x].AccountNumber;
                                    accountId = savingaccountrecords[x].Identifier;
                                    accountType = savingaccountrecords[x].AccountType;
                                }
                                else if (page.PageTypeName == HtmlConstants.CURRENT_ACCOUNT_PAGE)
                                {
                                    accountNumber = curerntaccountrecords[x].AccountNumber;
                                    accountId = curerntaccountrecords[x].Identifier;
                                    accountType = curerntaccountrecords[x].AccountType;
                                }

                                //start creating sub tabs, append tab name with last 4 digits of account number
                                if (page.PageTypeName == HtmlConstants.SAVING_ACCOUNT_PAGE || page.PageTypeName == HtmlConstants.CURRENT_ACCOUNT_PAGE)
                                {
                                    string lastFourDigisOfAccountNumber = accountNumber.Length > 4 ? accountNumber.Substring(Math.Max(0, accountNumber.Length - 4)) : accountNumber;
                                    if (x == 0)
                                    {
                                        SubTabs.Append("<ul class='nav nav-tabs' style='margin-top:-20px;'>");
                                    }

                                    SubTabs.Append("<li class='nav-item " + (x == 0 ? "active" : "") + "'><a id='tab" + x + "-tab' data-toggle='tab' " + "data-target='#" + (page.PageTypeName == HtmlConstants.SAVING_ACCOUNT_PAGE ? "Saving" : "Current") + "-" + lastFourDigisOfAccountNumber + "-" + "AccountNumber-" + accountId + "' " +
                                        " role='tab' class='nav-link " + (x == 0 ? "active" : "") + "'> Account - " + lastFourDigisOfAccountNumber + "</a></li>");

                                    newPageContent.Append("<div id='" + (page.PageTypeName == HtmlConstants.SAVING_ACCOUNT_PAGE ? "Saving" : "Current") +
                                        "-" + lastFourDigisOfAccountNumber + "-" + "AccountNumber-" + accountId + "' class='tab-pane fade in " + (x == 0 ? "active show" : "")
                                        + "'>");

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
                                            this.BindCustomerInformationWidgetData(pageContent, customer, statement, page, widget, customerMedias, batchDetails);
                                            break;
                                        case HtmlConstants.ACCOUNT_INFORMATION_WIDGET_NAME:
                                            this.BindAccountInformationWidgetData(pageContent, customer, page, widget);
                                            break;
                                        case HtmlConstants.IMAGE_WIDGET_NAME:
                                            IsFailed = this.BindImageWidgetData(pageContent, ErrorMessages, customer, customerMedias, batchDetails, statement, page, batchMaster, widget, tenantCode, outputLocation);
                                            break;
                                        case HtmlConstants.VIDEO_WIDGET_NAME:
                                            IsFailed = this.BindVideoWidgetData(pageContent, ErrorMessages, customer, customerMedias, batchDetails, statement, page, batchMaster, widget, tenantCode, outputLocation);
                                            break;
                                        case HtmlConstants.SUMMARY_AT_GLANCE_WIDGET_NAME:
                                            IsFailed = this.BindSummaryAtGlanceWidgetData(pageContent, ErrorMessages, accountrecords, page, widget);
                                            break;
                                        case HtmlConstants.CURRENT_AVAILABLE_BALANCE_WIDGET_NAME:
                                            IsFailed = this.BindCurrentAvailBalanceWidgetData(pageContent, ErrorMessages, customer, batchMaster, accountId, accountrecords, page, widget, currency);
                                            break;
                                        case HtmlConstants.SAVING_AVAILABLE_BALANCE_WIDGET_NAME:
                                            IsFailed = this.BindSavingAvailBalanceWidgetData(pageContent, ErrorMessages, customer, batchMaster, accountId, accountrecords, page, widget, currency);
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
                                            this.BindAnalyticsChartWidgetData(pageContent, scriptHtmlRenderer, customer, batchMaster, accountrecords, page, outputLocation);
                                            break;
                                        case HtmlConstants.SAVING_TREND_WIDGET_NAME:
                                            IsFailed = this.BindSavingTrendChartWidgetData(pageContent, scriptHtmlRenderer, ErrorMessages, customer, batchMaster, CustomerSavingTrends, accountId, page, tenantCode, outputLocation);
                                            break;
                                        case HtmlConstants.SPENDING_TREND_WIDGET_NAME:
                                            IsFailed = this.BindSpendingTrendChartWidgetData(pageContent, scriptHtmlRenderer, ErrorMessages, customer, batchMaster, CustomerSavingTrends, accountId, page, tenantCode, outputLocation);
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
                                        CustomeTheme themeDetails = new CustomeTheme();
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
                                        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                                        httpClient.DefaultRequestHeaders.Add("TenantCode", tenantCode);

                                        //API search parameter
                                        JObject searchParameter = new JObject();
                                        searchParameter["BatchId"] = batchMaster.Identifier;
                                        searchParameter["CustomerId"] = customer.Identifier;
                                        searchParameter["WidgetFilterSetting"] = dynawidget.WidgetFilterSettings;

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

                            //if account number variable is not empty means, financial tenant
                            if (accountNumber != string.Empty)
                            {
                                //generate statement metadata records in list format
                                statementMetadataRecords.Add(new StatementMetadata
                                {
                                    AccountNumber = accountNumber,
                                    AccountType = accountType,
                                    CustomerId = customer.Identifier,
                                    CustomerName = customer.FirstName + (customer.MiddleName == string.Empty ? "" : " " + customer.MiddleName) + " " + customer.LastName,
                                    StatementPeriod = customer.StatementPeriod,
                                    StatementId = statement.Identifier,
                                });
                            }
                            else
                            {
                                //To add statement metadata records for subscription master tenant
                                TransactionDataSearchParameter transactionDataSearchParameter = new TransactionDataSearchParameter()
                                {
                                    BatchId = batchMaster.Identifier,
                                    CustomerId = customer.Identifier
                                };
                                var subscriptionMasters = this.tenantTransactionDataManager.Get_TTD_SubscriptionMasters(transactionDataSearchParameter, tenantCode);
                                subscriptionMasters.ToList().ForEach(sub =>
                                {
                                    var records = statementMetadataRecords.Where(item => item.CustomerId == customer.Identifier && item.StatementId == statement.Identifier && item.AccountNumber == sub.Subscription && item.AccountType == sub.VendorName).ToList();
                                    if (records.Count <= 0)
                                    {
                                        statementMetadataRecords.Add(new StatementMetadata
                                        {
                                            AccountNumber = sub.Subscription,
                                            AccountType = sub.VendorName,
                                            CustomerId = customer.Identifier,
                                            CustomerName = customer.FirstName + (customer.MiddleName == string.Empty ? "" : " " + customer.MiddleName) + " " + customer.LastName,
                                            StatementPeriod = customer.StatementPeriod,
                                            StatementId = statement.Identifier,
                                        });
                                    }
                                });
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

                    //replace below variable with actual data in final html string
                    finalHtml.Replace("{{ChartScripts}}", scriptHtmlRenderer.ToString());
                    finalHtml.Replace("{{CustomerNumber}}", customer.Identifier.ToString());
                    finalHtml.Replace("{{StatementNumber}}", statement.Identifier.ToString());
                    finalHtml.Replace("{{FirstPageId}}", FirstPageId.ToString());
                    finalHtml.Replace("{{TenantCode}}", tenantCode);

                    //If has any error while rendering html statement, then assign status as failed and all collected errors message to log message variable..
                    //Otherwise write html statement string to actual html file and store it at output location, then assign status as completed
                    if (IsFailed)
                    {
                        logDetailRecord.Status = ScheduleLogStatus.Failed.ToString();
                        logDetailRecord.LogMessage = "<ul class='pl-4 text-left'>" + ErrorMessages.ToString() + "</ul>";
                    }
                    else
                    {
                        string fileName = "Statement_" + customer.Identifier + "_" + statement.Identifier + "_" + DateTime.Now.ToString().Replace("-", "_").Replace(":", "_").Replace(" ", "_").Replace('/', '_') + ".html";
                        string filePath = this.utility.WriteToFile(finalHtml.ToString(), fileName, batchMaster.Identifier, customer.Identifier, baseURL, outputLocation);

                        logDetailRecord.StatementFilePath = filePath;
                        logDetailRecord.Status = ScheduleLogStatus.Completed.ToString();
                        logDetailRecord.LogMessage = "Statement generated successfully..!!";
                        logDetailRecord.statementMetadata = statementMetadataRecords;
                    }
                }

                return logDetailRecord;
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
        /// This method help to actaul color theme to show series data for dynamic line graph, bar graph, and pie chart widgets
        /// </summary>
        /// <param name="theme"> the widget theme </param>
        /// <returns>return new color theme for graph and chart widgets </returns>
        public string GetChartColorTheme(string theme)
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
            StringBuilder AccDivData = new StringBuilder();
            AccDivData.Append("<div class='list-row-small ht70px'><div class='list-middle-row'> <div class='list-text'>Statement Date" + "</div><label class='list-value mb-0'>" + Convert.ToDateTime(customer.StatementDate).ToShortDateString() + "</label></div></div>");

            AccDivData.Append("<div class='list-row-small ht70px'><div class='list-middle-row'> <div class='list-text'>Statement Period" + "</div><label class='list-value mb-0'>" + customer.StatementPeriod + "</label></div></div>");

            AccDivData.Append("<div class='list-row-small ht70px'><div class='list-middle-row'> <div class='list-text'>Cusomer ID" + "</div><label class='list-value mb-0'>" + customer.CustomerCode + "</label></div></div>");

            AccDivData.Append("<div class='list-row-small ht70px'><div class='list-middle-row'> <div class='list-text'>RM Name" + "</div><label class='list-value mb-0'>" + customer.RmName + "</label></div></div>");

            AccDivData.Append("<div class='list-row-small ht70px'><div class='list-middle-row'> <div class='list-text'>RM Contact Number" + "</div><label class='list-value mb-0'>" + customer.RmContactNumber + "</label></div></div>");
            pageContent.Replace("{{AccountInfoData_" + page.Identifier + "_" + widget.Identifier + "}}", AccDivData.ToString());
        }

        private bool BindImageWidgetData(StringBuilder pageContent, StringBuilder ErrorMessages, CustomerMaster customer, IList<CustomerMedia> customerMedias, IList<BatchDetail> batchDetails, Statement statement, Page page, BatchMaster batchMaster, PageWidget widget, string tenantCode, string outputLocation)
        {
            var imgAssetFilepath = string.Empty;
            var IsFailed = false;

            if (widget.WidgetSetting != string.Empty && validationEngine.IsValidJson(widget.WidgetSetting))
            {
                dynamic widgetSetting = JObject.Parse(widget.WidgetSetting);
                if (widgetSetting.isPersonalize == false) //Is not dynamic image, then assign selected image from asset library
                {
                    var asset = assetLibraryRepository.GetAssets(new AssetSearchParameter { Identifier = widgetSetting.AssetId, SortParameter = new SortParameter { SortColumn = "Id" } }, tenantCode).ToList()?.FirstOrDefault();
                    if (asset != null)
                    {
                        var path = asset.FilePath.ToString();
                        var fileName = asset.Name;
                        var imagePath = outputLocation + "\\Statements\\" + batchMaster.Identifier + "\\" + customer.Identifier;
                        if (File.Exists(path) && !File.Exists(imagePath + "\\" + fileName))
                        {
                            File.Copy(path, Path.Combine(imagePath, fileName));
                        }
                        imgAssetFilepath = "./" + fileName;
                    }
                    else
                    {
                        ErrorMessages.Append("<li>Image asset file not found in asset library for Page: " + page.Identifier + " and Widget: " + widget.Identifier + " for image widget..!!</li>");
                        IsFailed = true;
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
                        else
                        {
                            ErrorMessages.Append("<li>Image not found for Page: " + page.Identifier + " and Widget: " + widget.Identifier + " for image widget..!!</li>");
                            IsFailed = true;
                        }
                    }
                }
                pageContent.Replace("{{ImageSource_" + statement.Identifier + "_" + page.Identifier + "_" + widget.Identifier + "}}", imgAssetFilepath);
            }
            else
            {
                ErrorMessages.Append("<li>Image widget configuration is missing for Page: " + page.Identifier + " and Widget: " + widget.Identifier + "!!</li>");
                IsFailed = true;
            }

            return IsFailed;
        }

        private bool BindVideoWidgetData(StringBuilder pageContent, StringBuilder ErrorMessages, CustomerMaster customer, IList<CustomerMedia> customerMedias, IList<BatchDetail> batchDetails, Statement statement, Page page, BatchMaster batchMaster, PageWidget widget, string tenantCode, string outputLocation)
        {
            var vdoAssetFilepath = string.Empty;
            var IsFailed = false;

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
                        var fileName = asset.Name;
                        var videoPath = outputLocation + "\\Statements\\" + batchMaster.Identifier + "\\" + customer.Identifier;
                        if (File.Exists(path) && !File.Exists(videoPath + "\\" + fileName))
                        {
                            File.Copy(path, Path.Combine(videoPath, fileName));
                        }
                        vdoAssetFilepath = "./" + fileName;
                    }
                    else
                    {
                        ErrorMessages.Append("<li>Video asset file not found in asset library for Page: " + page.Identifier + " and Widget: " + widget.Identifier + " for video widget..!!</li>");
                        IsFailed = true;
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
                        else
                        {
                            ErrorMessages.Append("<li>Video not found for Page: " + page.Identifier + " and Widget: " + widget.Identifier + " for video widget..!!</li>");
                            IsFailed = true;
                        }
                    }
                }
                pageContent.Replace("{{VideoSource_" + statement.Identifier + "_" + page.Identifier + "_" + widget.Identifier + "}}", vdoAssetFilepath);
            }
            else
            {
                ErrorMessages.Append("<li>Video widget configuration is missing for Page: " + page.Identifier + " and Widget: " + widget.Identifier + "!!</li>");
                IsFailed = true;
            }

            return IsFailed;
        }

        private bool BindSummaryAtGlanceWidgetData(StringBuilder pageContent, StringBuilder ErrorMessages, IList<AccountMaster> accountrecords, Page page, PageWidget widget)
        {
            var IsFailed = false;
            if (accountrecords != null && accountrecords.Count > 0)
            {
                StringBuilder accSummary = new StringBuilder();
                var accRecords = accountrecords.GroupBy(item => item.AccountType).ToList();
                accRecords.ToList().ForEach(acc =>
                {
                    accSummary.Append("<tr><td>" + acc.FirstOrDefault().AccountType + "</td><td>" + acc.FirstOrDefault().Currency + "</td><td>" + acc.Sum(it => Convert.ToDecimal(it.Balance)).ToString() + "</td></tr>");
                });
                pageContent.Replace("{{AccountSummary_" + page.Identifier + "_" + widget.Identifier + "}}", accSummary.ToString());
            }
            else
            {
                ErrorMessages.Append("<li>Account master data is not available related to Summary at Glance widget..!!</li>");
                IsFailed = true;
            }
            return IsFailed;
        }

        private bool BindCurrentAvailBalanceWidgetData(StringBuilder pageContent, StringBuilder ErrorMessages, CustomerMaster customer, BatchMaster batchMaster, long accountId, IList<AccountMaster> accountrecords, Page page, PageWidget widget, string currency)
        {
            var IsFailed = false;
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
                else
                {
                    ErrorMessages.Append("<li>Current Account master data is not available related to Current Available Balance widget..!!</li>");
                    IsFailed = true;
                }
            }
            else
            {
                ErrorMessages.Append("<li>Account master data is not available related to Current Available Balance widget..!!</li>");
                IsFailed = true;
            }
            return IsFailed;
        }

        private bool BindSavingAvailBalanceWidgetData(StringBuilder pageContent, StringBuilder ErrorMessages, CustomerMaster customer, BatchMaster batchMaster, long accountId, IList<AccountMaster> accountrecords, Page page, PageWidget widget, string currency)
        {
            var IsFailed = false;
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
                else
                {
                    ErrorMessages.Append("<li>Saving Account master data is not available related to Saving Available Balance widget..!!</li>");
                    IsFailed = true;
                }
            }
            else
            {
                ErrorMessages.Append("<li>Account master data is not available related to Saving Available Balance widget..!!</li>");
                IsFailed = true;
            }
            return IsFailed;
        }

        private void BindSavingTransactionWidgetData(StringBuilder pageContent, StringBuilder scriptHtmlRenderer, CustomerMaster customer, BatchMaster batchMaster, IList<AccountTransaction> CustomerAcccountTransactions, Page page, PageWidget widget, long accountId, string tenantCode, string currency, string outputLocation)
        {
            var accountTransactions = CustomerAcccountTransactions.Where(item => item.CustomerId == customer.Identifier && item.BatchId == batchMaster.Identifier && item.AccountType.ToLower().Contains("saving") && item.AccountId == accountId && item.TenantCode == tenantCode)?.ToList();

            StringBuilder transaction = new StringBuilder();
            StringBuilder selectOption = new StringBuilder();

            if (accountTransactions != null && accountTransactions.Count > 0)
            {
                IList<AccountTransaction> transactions = new List<AccountTransaction>();
                pageContent.Replace("{{Currency}}", currency);
                //get saving transaction data in the list and then convert it to json format string 
                //and store it as file at same directory of html statement file
                accountTransactions.ToList().ForEach(trans =>
                {
                    AccountTransaction accountTransaction = new AccountTransaction();
                    accountTransaction.AccountType = trans.AccountType;
                    accountTransaction.TransactionDate = trans.TransactionDate;
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

                    var SavingTransactionGridJson = "savingtransactiondata" + accountId + page.Identifier + "=" + savingtransactionjson;
                    this.utility.WriteToJsonFile(SavingTransactionGridJson, "savingtransactiondetail" + accountId + page.Identifier + ".json", batchMaster.Identifier, customer.Identifier, outputLocation);
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

        private void BindCurrentTransactionWidgetData(StringBuilder pageContent, StringBuilder scriptHtmlRenderer, CustomerMaster customer, BatchMaster batchMaster, IList<AccountTransaction> CustomerAcccountTransactions, Page page, PageWidget widget, long accountId, string tenantCode, string currency, string outputLocation)
        {
            var accountTransactions = CustomerAcccountTransactions.Where(item => item.CustomerId == customer.Identifier && item.BatchId == batchMaster.Identifier && item.AccountType.ToLower().Contains("current") && item.AccountId == accountId && item.TenantCode == tenantCode)?.ToList();

            StringBuilder transaction = new StringBuilder();
            StringBuilder selectOption = new StringBuilder();
            if (accountTransactions != null && accountTransactions.Count > 0)
            {
                IList<AccountTransaction> transactions = new List<AccountTransaction>();
                pageContent.Replace("{{Currency}}", currency);
                //get saving transaction data in the list and then convert it to json format string
                //and store it as json file at same directory of html statement file
                accountTransactions.ToList().ForEach(trans =>
                {
                    AccountTransaction accountTransaction = new AccountTransaction();
                    accountTransaction.AccountType = trans.AccountType;
                    accountTransaction.TransactionDate = trans.TransactionDate;
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

                    var CurrentTransactionGridJson = "currenttransactiondata" + accountId + page.Identifier + "=" + currenttransactionjson;
                    this.utility.WriteToJsonFile(CurrentTransactionGridJson, "currenttransactiondetail" + accountId + page.Identifier + ".json", batchMaster.Identifier, customer.Identifier, outputLocation);
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
                var top4IncomeSources = this.tenantTransactionDataManager.GetCustomerIncomeSources(customer.Identifier, batchMaster.Identifier, tenantCode)?.OrderByDescending(it => it.CurrentSpend)?.Take(4)?.ToList();
                StringBuilder incomeSources = new StringBuilder();
                if (top4IncomeSources != null && top4IncomeSources.Count > 0)
                {
                    top4IncomeSources.ToList().ForEach(src =>
                    {
                        var tdstring = string.Empty;
                        if (Convert.ToDecimal(src.CurrentSpend) > Convert.ToDecimal(src.AverageSpend))
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
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void BindAnalyticsChartWidgetData(StringBuilder pageContent, StringBuilder scriptHtmlRenderer, CustomerMaster customer, BatchMaster batchMaster, IList<AccountMaster> accountrecords, Page page, string outputLocation)
        {
            var AnalyticsChartJson = string.Empty;
            if (accountrecords.Count > 0)
            {
                IList<AccountMasterRecord> accounts = new List<AccountMasterRecord>();
                var records = accountrecords.GroupBy(item => item.AccountType).ToList();

                //get analytics chart widget data, convert it into json string format
                //and store it as json file at same directory of html statement file
                records.ToList().ForEach(acc => accounts.Add(new AccountMasterRecord()
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

            this.utility.WriteToJsonFile(AnalyticsChartJson, "analyticschartdata.json", batchMaster.Identifier, customer.Identifier, outputLocation);
            scriptHtmlRenderer.Append("<script type='text/javascript' src='./analyticschartdata.json'></script>");
            pageContent.Replace("analyticschartcontainer", "analyticschartcontainer" + page.Identifier);
            scriptHtmlRenderer.Append(HtmlConstants.ANALYTICS_CHART_WIDGET_SCRIPT.Replace("analyticschartcontainer", "analyticschartcontainer" + page.Identifier));
        }

        private bool BindSavingTrendChartWidgetData(StringBuilder pageContent, StringBuilder scriptHtmlRenderer, StringBuilder ErrorMessages, CustomerMaster customer, BatchMaster batchMaster, IList<SavingTrend> CustomerSavingTrends, long accountId, Page page, string tenantCode, string outputLocation)
        {
            var SavingTrendChartJson = string.Empty;
            var IsFailed = false;

            var savingtrends = CustomerSavingTrends.Where(item => item.CustomerId == customer.Identifier && item.BatchId == batchMaster.Identifier && item.AccountId == accountId && item.TenantCode == tenantCode).ToList();
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
                        trend.Income = lst[0].Income;
                        trend.IncomePercentage = lst[0].IncomePercentage;
                        trend.SpendAmount = lst[0].SpendAmount;
                        trend.SpendPercentage = lst[0].SpendPercentage;
                        savingTrendRecords.Add(trend);
                    }
                    else
                    {
                        ErrorMessages.Append("<li>Invalid consecutive month data for Saving trend widget..!!</li>");
                        IsFailed = true;
                    }
                    mnth = mnth - 1 == 0 ? 12 : mnth - 1;
                }

                //get saving trend chart widget data, convert it into json string format and store it as json file at same directory of html statement file
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

            this.utility.WriteToJsonFile(SavingTrendChartJson, "savingtrenddata" + accountId + page.Identifier + ".json", batchMaster.Identifier, customer.Identifier, outputLocation);
            scriptHtmlRenderer.Append("<script type='text/javascript' src='./savingtrenddata" + accountId + page.Identifier + ".json'></script>");

            pageContent.Replace("savingTrendscontainer", "savingTrendscontainer" + accountId + page.Identifier);
            var scriptval = HtmlConstants.SAVING_TREND_CHART_WIDGET_SCRIPT.Replace("savingTrendscontainer", "savingTrendscontainer" + accountId + page.Identifier).Replace("savingdata", "savingdata" + accountId + page.Identifier);
            scriptHtmlRenderer.Append(scriptval);

            return IsFailed;
        }

        private bool BindSpendingTrendChartWidgetData(StringBuilder pageContent, StringBuilder scriptHtmlRenderer, StringBuilder ErrorMessages, CustomerMaster customer, BatchMaster batchMaster, IList<SavingTrend> CustomerSavingTrends, long accountId, Page page, string tenantCode, string outputLocation)
        {
            var IsFailed = false;
            var SpendingTrendChartJson = string.Empty;

            var spendingtrends = CustomerSavingTrends.Where(item => item.CustomerId == customer.Identifier && item.BatchId == batchMaster.Identifier && item.AccountId == accountId && item.TenantCode == tenantCode).ToList();
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
                        trend.Income = lst[0].Income;
                        trend.IncomePercentage = lst[0].IncomePercentage;
                        trend.SpendAmount = lst[0].SpendAmount;
                        trend.SpendPercentage = lst[0].SpendPercentage;
                        trends.Add(trend);
                    }
                    else
                    {
                        ErrorMessages.Append("<li>Invalid consecutive month data for Spending trend widget..!!</li>");
                        IsFailed = true;
                    }
                    mnth = mnth - 1 == 0 ? 12 : mnth - 1;
                }

                //get spending trend chart widget data, convert it into json string format and store it as json file at same directory of html statement file
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

            this.utility.WriteToJsonFile(SpendingTrendChartJson, "spendingtrenddata" + accountId + page.Identifier + ".json", batchMaster.Identifier, customer.Identifier, outputLocation);
            scriptHtmlRenderer.Append("<script type='text/javascript' src='./spendingtrenddata" + accountId + page.Identifier + ".json'></script>");

            pageContent.Replace("spendingTrendscontainer", "spendingTrendscontainer" + accountId + page.Identifier);
            var scriptval = HtmlConstants.SPENDING_TREND_CHART_WIDGET_SCRIPT.Replace("spendingTrendscontainer", "spendingTrendscontainer" + accountId + page.Identifier).Replace("spendingdata", "spendingdata" + accountId + page.Identifier);
            scriptHtmlRenderer.Append(scriptval);

            return IsFailed;
        }

        private void BindReminderAndRecommendationWidgetData(StringBuilder pageContent, CustomerMaster customer, BatchMaster batchMaster, Page page, PageWidget widget, string tenantCode)
        {
            try
            {
                var reminderAndRecommendations = this.tenantTransactionDataManager.GetReminderAndRecommendation(customer.Identifier, batchMaster.Identifier, tenantCode);
                StringBuilder reminderstr = new StringBuilder();
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
                List<DynamicWidgetTableEntity> tableEntities = JsonConvert.DeserializeObject<List<DynamicWidgetTableEntity>>(dynawidget.WidgetSettings);
                var tr = new StringBuilder();

                //API call
                var response = httpClient.PostAsync(dynawidget.APIPath, new StringContent(JsonConvert.SerializeObject(searchParameter), Encoding.UTF8, "application/json")).Result;
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    var apiOutputArr = JArray.Parse(result);
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
                var response = httpClient.PostAsync(dynawidget.APIPath, new StringContent(JsonConvert.SerializeObject(searchParameter), Encoding.UTF8, "application/json")).Result;
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    var apiOutputArr = JArray.Parse(result);
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
                var response = httpClient.PostAsync(dynawidget.APIPath, new StringContent(JsonConvert.SerializeObject(searchParameter), Encoding.UTF8, "application/json")).Result;
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    var apiOutputArr = JArray.Parse(result);
                    if (apiOutputArr.Count > 0)
                    {
                        var graphEntity = JsonConvert.DeserializeObject<DynamicWidgetLineGraph>(dynawidget.WidgetSettings);
                        GraphChartData chartData = new GraphChartData();
                        chartData.title = new ChartTitle { text = dynawidget.Title };

                        //To get chart x-axis list
                        var xAxis = apiOutputArr.ToList().Select(item => item[graphEntity.XAxis].ToString()).ToList();
                        chartData.xAxis = xAxis;

                        //To get chart series data
                        IList<ChartSeries> chartSeries = new List<ChartSeries>();
                        graphEntity.Details.ToList().ForEach(field =>
                        {
                            ChartSeries series = new ChartSeries();
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
                var response = httpClient.PostAsync(dynawidget.APIPath, new StringContent(JsonConvert.SerializeObject(searchParameter), Encoding.UTF8, "application/json")).Result;
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    var apiOutputArr = JArray.Parse(result);
                    if (apiOutputArr.Count > 0)
                    {
                        var graphEntity = JsonConvert.DeserializeObject<DynamicWidgetLineGraph>(dynawidget.WidgetSettings);
                        GraphChartData chartData = new GraphChartData();
                        chartData.title = new ChartTitle { text = dynawidget.Title };

                        //To get chart x-axis list
                        var xAxis = apiOutputArr.ToList().Select(item => item[graphEntity.XAxis].ToString()).ToList();
                        chartData.xAxis = xAxis;

                        //To get chart series data
                        IList<ChartSeries> chartSeries = new List<ChartSeries>();
                        graphEntity.Details.ToList().ForEach(field =>
                        {
                            ChartSeries series = new ChartSeries();
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
                var response = httpClient.PostAsync(dynawidget.APIPath, new StringContent(JsonConvert.SerializeObject(searchParameter), Encoding.UTF8, "application/json")).Result;
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    var apiOutputArr = JArray.Parse(result);
                    if (apiOutputArr.Count > 0)
                    {
                        PieChartSettingDetails pieChartSetting = JsonConvert.DeserializeObject<PieChartSettingDetails>(dynawidget.WidgetSettings);
                        var entityFields = this.dynamicWidgetManager.GetEntityFields(dynawidget.EntityId, tenantCode);
                        var seriesfor = entityFields.Where(it => it.Identifier == Convert.ToInt32(pieChartSetting.PieSeries))?.ToList()?.FirstOrDefault().Name;
                        var seriesdatafor = entityFields.Where(it => it.Identifier == Convert.ToInt32(pieChartSetting.PieValue))?.ToList()?.FirstOrDefault().Name;

                        PiChartGraphData chartData = new PiChartGraphData();
                        chartData.title = new ChartTitle { text = dynawidget.Title };

                        //To get series data
                        var chartseries = new List<PieChartSeries>();
                        var datas = new List<PieChartData>();
                        apiOutputArr.ToList().ForEach(item =>
                        {
                            PieChartData pie = new PieChartData
                            {
                                name = item[seriesfor] != null ? item[seriesfor].ToString() : "",
                                y = Convert.ToDecimal(item[seriesdatafor] != null ? item[seriesdatafor] : 0)
                            };
                            datas.Add(pie);
                        });

                        PieChartSeries series = new PieChartSeries();
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
                        var response = httpClient.PostAsync(dynawidget.APIPath, new StringContent(JsonConvert.SerializeObject(searchParameter), Encoding.UTF8, "application/json")).Result;
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            var result = response.Content.ReadAsStringAsync().Result;
                            var apiOutputArr = JArray.Parse(result);
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

        #endregion
    }
}
