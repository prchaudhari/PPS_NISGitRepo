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
    using System.Linq;
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
                this.validationEngine = new ValidationEngine();
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
                TenantConfiguration tenantConfiguration = new TenantConfiguration();
                tenantConfiguration = this.tenantConfigurationManager.GetTenantConfigurations(tenantCode)?.FirstOrDefault();

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
                        string navbarHtml = HtmlConstants.NAVBAR_HTML.Replace("{{BrandLogo}}", "assets/images/logo_black.png").Replace("{{logo}}", "assets/images/nisLogo.png");
                        navbarHtml = navbarHtml.Replace("{{Today}}", DateTime.Now.ToString("dd MMM yyyy"));
                        StringBuilder navItemList = new StringBuilder();
                        tempHtml.Append(HtmlConstants.CONTAINER_DIV_HTML_HEADER);
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
                                    var pageHeaderHtml = HtmlConstants.PAGE_HEADER_HTML;
                                    if (page.PageTypeId == HtmlConstants.HOME_PAGE_TYPE_ID)
                                    {
                                        if (page.BackgroundImageAssetId != 0)
                                        {
                                            pageHeaderHtml = pageHeaderHtml.Replace("{{BackgroundImage}}", "");
                                            extraclass = extraclass + " BackgroundImage " + page.BackgroundImageAssetId;
                                            isBackgroundImage = true;
                                        }
                                        else if (page.BackgroundImageURL != string.Empty)
                                        {
                                            pageHeaderHtml = pageHeaderHtml.Replace("{{BackgroundImage}}", "style='background: url(" + page.BackgroundImageURL + ")'");
                                            isBackgroundImage = true;
                                        }
                                        else
                                        {
                                            pageHeaderHtml = pageHeaderHtml.Replace("{{BackgroundImage}}", "");
                                        }
                                    }
                                    else
                                    {
                                        pageHeaderHtml = pageHeaderHtml.Replace("{{BackgroundImage}}", "");
                                    }
                                    htmlString.Append(pageHeaderHtml.Replace("{{DivId}}", tabClassName).Replace("{{ExtraClass}}", extraclass));

                                    htmlString.Append(HtmlConstants.PAGE_TAB_CONTENT_HEADER);
                                    if (page.PageTypeId == HtmlConstants.SAVING_ACCOUNT_PAGE_TYPE_ID || page.PageTypeId == HtmlConstants.CURRENT_ACCOUNT_PAGE_TYPE_ID)
                                    {
                                        htmlString.Append("<ul class='nav nav-tabs' style='margin-top:-5px;'>");
                                        htmlString.Append("<li class='nav-item active'><a id='tab1-tab' data-toggle='tab' " + "data-target='#" + (page.PageTypeId ==
                                            HtmlConstants.SAVING_ACCOUNT_PAGE_TYPE_ID ? "Saving" : "Current") + "-' role='tab' class='nav-link active'> Account - 6789</a></li>");
                                        htmlString.Append("</ul>");

                                        var divClass = string.Empty;
                                        var styleTag = string.Empty;
                                        if (page.BackgroundImageAssetId != 0)
                                        {
                                            divClass = divClass + " BackgroundImage " + page.BackgroundImageAssetId;
                                            isBackgroundImage = true;
                                        }
                                        else if (page.BackgroundImageURL != string.Empty)
                                        {
                                            styleTag = styleTag + " style='background: url(" + page.BackgroundImageURL + ")'";
                                            isBackgroundImage = true;
                                        }

                                        htmlString.Append("<div id='" + (page.PageTypeId == HtmlConstants.SAVING_ACCOUNT_PAGE_TYPE_ID ? "Saving" : "Current") + "-6789' " +
                                            "class='tab-pane fade in active show " + divClass + "' " + styleTag + ">");
                                    }

                                    int tempRowWidth = 0; // variable to check col-lg div length (bootstrap)
                                    int max = 0;
                                    if (page.PageWidgets.Count > 0)
                                    {
                                        var completelst = page.PageWidgets;
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
                                                        htmlString.Append("<div class='row'>"); // to start new row class div 
                                                        isRowComplete = false;
                                                    }
                                                    int divLength = mergedlst[i].Width;//((mergedlst[i].Width * 12) % 20) > 10 ? (((mergedlst[i].Width * 12) / 20) + 1) : ((mergedlst[i].Width * 12) / 20);
                                                    tempRowWidth = tempRowWidth + divLength;

                                                    // If current col-lg class length is greater than 12, 
                                                    //then end parent row class div and then start new row class div
                                                    if (tempRowWidth > 12)
                                                    {
                                                        tempRowWidth = divLength;
                                                        htmlString.Append("</div>"); // to end row class div
                                                        htmlString.Append("<div class='row'>"); // to start new row class div
                                                        isRowComplete = false;
                                                    }
                                                    htmlString.Append("<div class='col-lg-" + divLength + "'>");
                                                    if (mergedlst[i].WidgetId == HtmlConstants.CUSTOMER_INFORMATION_WIDGET_ID)
                                                    {
                                                        string customerInfoJson = "{'FirstName':'Laura','MiddleName':'J','LastName':'Donald','AddressLine1':" +
                                                            "'4000 Executive Parkway','AddressLine2':'Saint Globin Rd','City':'Canary Wharf', 'State':'London', " +
                                                            "'Country':'England','Zip':'E14 9RZ'}";
                                                        if (customerInfoJson != string.Empty && validationEngine.IsValidJson(customerInfoJson))
                                                        {
                                                            CustomerInformation customerInfo = JsonConvert.DeserializeObject<CustomerInformation>(customerInfoJson);
                                                            var customerHtmlWidget = HtmlConstants.CUSTOMER_INFORMATION_WIDGET_HTML.Replace("{{VideoSource}}",
                                                                "assets/images/SampleVideo.mp4");

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
                                                    else if (mergedlst[i].WidgetId == HtmlConstants.ACCOUNT_INFORMATION_WIDGET_ID)
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
                                                        htmlString.Append(accountInfoData);
                                                    }
                                                    else if (mergedlst[i].WidgetId == HtmlConstants.IMAGE_WIDGET_ID)
                                                    {
                                                        var isImageFromAsset = false;
                                                        var assetId = 0;
                                                        var imgAssetFilepath = "assets/images/icon-image.png";
                                                        if (mergedlst[i].WidgetSetting != string.Empty && validationEngine.IsValidJson(mergedlst[i].WidgetSetting))
                                                        {
                                                            dynamic widgetSetting = JObject.Parse(mergedlst[i].WidgetSetting);
                                                            if (widgetSetting.isPersonalize == false)
                                                            {
                                                                if (tenantConfiguration.AssetPath == string.Empty)
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
                                                        htmlString.Append(imgHtmlWidget);
                                                    }
                                                    else if (mergedlst[i].WidgetId == HtmlConstants.VIDEO_WIDGET_ID)
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
                                                                if (tenantConfiguration.AssetPath == string.Empty)
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
                                                        htmlString.Append(vdoHtmlWidget);
                                                    }
                                                    else if (mergedlst[i].WidgetId == HtmlConstants.SUMMARY_AT_GLANCE_WIDGET_ID)
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
                                                        htmlString.Append(accountSummary);
                                                    }
                                                    else if (mergedlst[i].WidgetId == HtmlConstants.CURRENT_AVAILABLE_BALANCE_WIDGET_ID)
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
                                                            htmlString.Append(CurrentAvailBalance);
                                                        }
                                                    }
                                                    else if (mergedlst[i].WidgetId == HtmlConstants.SAVING_AVAILABLE_BALANCE_WIDGET_ID)
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
                                                            htmlString.Append(SavingAvailBalance);
                                                        }
                                                    }
                                                    else if (mergedlst[i].WidgetId == HtmlConstants.SAVING_TRANSACTION_WIDGET_ID)
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
                                                            htmlString.Append(accountTransactionstr);
                                                        }
                                                    }
                                                    else if (mergedlst[i].WidgetId == HtmlConstants.CURRENT_TRANSACTION_WIDGET_ID)
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
                                                            htmlString.Append(accountTransactionstr);
                                                        }
                                                    }
                                                    else if (mergedlst[i].WidgetId == HtmlConstants.TOP_4_INCOME_SOURCES_WIDGET_ID)
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
                                                                    tdstring = "<span class='fa fa-sort-desc fa-2x text-danger' aria-hidden='true'></span><span class='ml-2'>"
                                                                    + item.AverageSpend + "</span>";
                                                                }
                                                                else
                                                                {
                                                                    tdstring = "<span class='fa fa-sort-asc fa-2x mt-1' aria-hidden='true' " +
                                                                    "style='position:relative;top:6px;color:limegreen'></span><span class='ml-2'>" + item.AverageSpend
                                                                    + "</span>";
                                                                }
                                                                incomeSrc.Append("<tr><td class='float-left'>" + item.Source + "</td>" + "<td> " + item.CurrentSpend +
                                                                  "" + "</td><td>" + tdstring + "</td></tr>");
                                                            });
                                                            string srcstring = HtmlConstants.TOP_4_INCOME_SOURCE_WIDGET_HTML.Replace("{{IncomeSourceList}}",
                                                                incomeSrc.ToString());
                                                            htmlString.Append(srcstring);
                                                        }
                                                    }
                                                    else if (mergedlst[i].WidgetId == HtmlConstants.ANALYTICS_WIDGET_ID)
                                                    {
                                                        htmlString.Append(HtmlConstants.ANALYTIC_WIDGET_HTML);
                                                    }
                                                    else if (mergedlst[i].WidgetId == HtmlConstants.SPENDING_TREND_WIDGET_ID)
                                                    {
                                                        htmlString.Append(HtmlConstants.SPENDING_TRENDS_WIDGET_HTML);
                                                    }
                                                    else if (mergedlst[i].WidgetId == HtmlConstants.SAVING_TREND_WIDGET_ID)
                                                    {
                                                        htmlString.Append(HtmlConstants.SAVING_TRENDS_WIDGET_HTML);
                                                    }
                                                    else if (mergedlst[i].WidgetId == HtmlConstants.REMINDER_AND_RECOMMENDATION_WIDGET_ID)
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
                                                            htmlString.Append(widgetstr);
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
                                    if (page.PageTypeId == HtmlConstants.SAVING_ACCOUNT_PAGE_TYPE_ID || page.PageTypeId == HtmlConstants.CURRENT_ACCOUNT_PAGE_TYPE_ID)
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

                            if (isBackgroundImage)
                            {
                                htmlString.Replace("card border-0", "card border-0 bg-transparent");
                                htmlString.Replace("bg-light", "bg-transparent");
                            }
                            tempHtml.Append(htmlString.ToString());
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

        #endregion
    }
}
