// <copyright file="PageManager.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2018 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------

namespace nIS
{

    #region References
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using Unity;
    #endregion

    public class PageManager
    {
        #region Private members
        /// <summary>
        /// The unity container
        /// </summary>
        private IUnityContainer unityContainer = null;

        /// <summary>
        /// The page repository.
        /// </summary>
        IPageRepository pageRepository = null;

        /// <summary>
        /// The validation engine object
        /// </summary>
        IValidationEngine validationEngine = null;

        /// <summary>
        /// The tenant configuration manager object.
        /// </summary>
        private TenantConfigurationManager tenantConfigurationManager = null;

        /// <summary>
        /// The Asset repository.
        /// </summary>
        private IAssetLibraryRepository assetLibraryRepository = null;

        /// <summary>
        /// The dynamic widget manager object.
        /// </summary>
        private DynamicWidgetManager dynamicWidgetManager = null;

        #endregion

        #region Constructor

        /// <summary>
        /// This is constructor for role manager, which initialise
        /// role repository.
        /// </summary>
        /// <param name="container">IUnity container implementation object.</param>
        public PageManager(IUnityContainer unityContainer)
        {
            try
            {
                this.unityContainer = unityContainer;
                this.pageRepository = this.unityContainer.Resolve<IPageRepository>();
                this.tenantConfigurationManager = new TenantConfigurationManager(unityContainer);
                this.assetLibraryRepository = this.unityContainer.Resolve<IAssetLibraryRepository>();
                this.dynamicWidgetManager = new DynamicWidgetManager(unityContainer);
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
        /// This method will call add pages method of repository.
        /// </summary>
        /// <param name="pages">Pages are to be add.</param>
        /// <param name="tenantCode">Tenant code of page.</param>
        /// <returns>
        /// Returns true if entities added successfully, false otherwise.
        /// </returns>
        public bool AddPages(IList<Page> pages, string tenantCode)
        {
            bool result = false;
            try
            {
                this.IsValidPages(pages, tenantCode);
                this.IsDuplicatePage(pages, tenantCode);
                result = this.pageRepository.AddPages(pages, tenantCode);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method will call update pages method of repository
        /// </summary>
        /// <param name="pages">pages are to be update.</param>
        /// <param name="tenantCode">Tenant code of page.</param>
        /// <returns>
        /// Returns true if roles updated successfully, false otherwise.
        /// </returns>
        public bool UpdatePages(IList<Page> pages, string tenantCode)
        {
            bool result = false;
            try
            {
                this.IsValidPages(pages, tenantCode);
                this.IsDuplicatePage(pages, tenantCode);
                result = this.pageRepository.UpdatePages(pages, tenantCode);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method will call delete pages method of repository
        /// </summary>
        /// <param name="pageIdentifier">Page iddentifier</param>
        /// <param name="tenantCode">Tenant code of page.</param>
        /// <returns>
        /// Returns true if pages deleted successfully, false otherwise.
        /// </returns>
        public bool DeletePages(long pageIdentifier, string tenantCode)
        {
            bool result = false;
            try
            {
                result = this.pageRepository.DeletePages(pageIdentifier, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return result;
        }

        /// <summary>
        /// This method will call get pages method of repository.
        /// </summary>
        /// <param name="pageSearchParameter">The page search parameters.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns>
        /// Returns pages if found for given parameters, else return null
        /// </returns>
        public IList<Page> GetPages(PageSearchParameter pageSearchParameter, string tenantCode)
        {
            try
            {
                InvalidSearchParameterException invalidSearchParameterException = new InvalidSearchParameterException(tenantCode);
                try
                {
                    pageSearchParameter.IsValid();
                }
                catch (Exception exception)
                {
                    invalidSearchParameterException.Data.Add("InvalidPagingParameter", exception.Data);
                }

                if (invalidSearchParameterException.Data.Count > 0)
                {
                    throw invalidSearchParameterException;
                }

                pageSearchParameter.StartDate = this.validationEngine.IsValidDate(pageSearchParameter.StartDate) ? pageSearchParameter.StartDate.ToLocalTime() : pageSearchParameter.StartDate;
                pageSearchParameter.EndDate = this.validationEngine.IsValidDate(pageSearchParameter.EndDate) ? pageSearchParameter.EndDate.ToLocalTime() : pageSearchParameter.EndDate;

                return this.pageRepository.GetPages(pageSearchParameter, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// This method will call get pages method of repository.
        /// </summary>
        /// <param name="pageSearchParameter">The page search parameters.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns>
        /// Returns pages if found for given parameters, else return null
        /// </returns>
        public IList<Page> GetPagesForList(PageSearchParameter pageSearchParameter, string tenantCode)
        {
            try
            {
                InvalidSearchParameterException invalidSearchParameterException = new InvalidSearchParameterException(tenantCode);
                try
                {
                    pageSearchParameter.IsValid();
                }
                catch (Exception exception)
                {
                    invalidSearchParameterException.Data.Add("InvalidPagingParameter", exception.Data);
                }

                if (invalidSearchParameterException.Data.Count > 0)
                {
                    throw invalidSearchParameterException;
                }

                pageSearchParameter.StartDate = this.validationEngine.IsValidDate(pageSearchParameter.StartDate) ? pageSearchParameter.StartDate.ToLocalTime() : pageSearchParameter.StartDate;
                pageSearchParameter.EndDate = this.validationEngine.IsValidDate(pageSearchParameter.EndDate) ? pageSearchParameter.EndDate.ToLocalTime() : pageSearchParameter.EndDate;

                return this.pageRepository.GetPagesForList(pageSearchParameter, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// This method helps to get count of pages.
        /// </summary>
        /// <param name="pageSearchParameter">The page search parameters.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns>
        /// Returns count of pages
        /// </returns>
        public int GetPageCount(PageSearchParameter pageSearchParameter, string tenantCode)
        {
            int roleCount = 0;
            try
            {
                roleCount = this.pageRepository.GetPageCount(pageSearchParameter, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return roleCount;
        }

        /// <summary>
        /// This method will call publish page method of repository
        /// </summary>
        /// <param name="pageIdentifier">Page identifier</param>
        /// <param name="tenantCode">Tenant code of page.</param>
        /// <returns>
        /// Returns true if pages publish successfully, false otherwise.
        /// </returns>
        public bool PublishPage(long pageIdentifier, string tenantCode)
        {
            bool result = false;
            try
            {
                result = this.pageRepository.PublishPage(pageIdentifier, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return result;
        }

        /// <summary>
        /// This method will call clone page method of repository
        /// </summary>
        /// <param name="pageIdentifier">Page identifier</param>
        /// <param name="tenantCode">Tenant code of page.</param>
        /// <returns>
        /// Returns true if pages clone successfully, false otherwise.
        /// </returns>
        public bool ClonePage(long pageIdentifier, string tenantCode)
        {
            bool result = false;
            try
            {
                result = this.pageRepository.ClonePage(pageIdentifier, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return result;
        }

        public string PreviewPage(long pageIdentifier, string baseURL, string tenantCode)
        {
            StringBuilder htmlString = new StringBuilder();
            IList<string> linegraphIds = new List<string>();
            IList<string> bargraphIds = new List<string>();
            IList<string> piechartIds = new List<string>();

            try
            {
                var tenantConfiguration = this.tenantConfigurationManager.GetTenantConfigurations(tenantCode)?.FirstOrDefault();
                PageSearchParameter pageSearchParameter = new PageSearchParameter
                {
                    Identifier = pageIdentifier,
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
                IList<Page> pages = this.pageRepository.GetPages(pageSearchParameter, tenantCode);
                var MarketingMessages = "[{'MarketingMessageHeader': 'Notice of interest rate change','MarketingMessageText1': 'The interest rates on our current notice deposit accounts have changed on 03 November 2020.','MarketingMessageText2': 'Please review your investment portfolio and rates regularly to ensure that it is still appropriate and meets your needs.','MarketingMessageText3': 'For more information about the interest rate change call 0860 555 111.','MarketingMessageText4': '','MarketingMessageText5': ''},{'MarketingMessageHeader': 'Annual investment fee review','MarketingMessageText1': 'As part of our annual pricing review, some of our fees have changed. You can view our 2020 pricing guide on the Nedbank website.','MarketingMessageText2': 'Click <a href=\"javascript:void(0);\" style=\"color:black;text-decoration: underline;\">here </a> for more information.','MarketingMessageText3': 'If you do not want to click on a link, please type the following address in your browser (without the asterisks): *https://www.nedbank.co.za/content/nedbank/desktop/gt/en/info/campaigns/annual-fee-update2020.html*.','MarketingMessageText4': '','MarketingMessageText5': ''},{'MarketingMessageHeader': 'Notice of withdrawal fee','MarketingMessageText1': 'From 1 July 2020 <strong>we will charge fees if you give notice of withdrawals at a branch or call centre</strong>.','MarketingMessageText2': 'Bank smart and avoid fees by using your Money app, Online Banking or dialling *120*001# to give notice of withdrawal on your investment.','MarketingMessageText3': '','MarketingMessageText4': '','MarketingMessageText5': ''},{'MarketingMessageHeader': 'Automatic reinvestment when your investment matures','MarketingMessageText1': 'Without your instruction to reinvest or pay out your money, we will automatically reinvest it in a notice deposit account at the rate applicable to this account when your investment matures. Any subsequent rate changes will also apply.','MarketingMessageText2': 'Please let us know what should happen to your investment before it matures.','MarketingMessageText3': '','MarketingMessageText4': '','MarketingMessageText5': ''},{'MarketingMessageHeader': 'Save time and money with our digital and mobile channels','MarketingMessageText1': 'We are continuously enhancing our digital and mobile banking features and capabilities to give you the best experience. We want you to manage your investment accounts seamlessly so that you can have greater control over your money.','MarketingMessageText2': 'Access and transact on your investment account using the Nedbank Money app, Online Banking, USSD and the Nedbank Money App Lite, which requires less data and storage and no software updates.','MarketingMessageText3': '','MarketingMessageText4': '','MarketingMessageText5': ''},{'MarketingMessageHeader': 'Notice of withdrawal limit on digital and mobile banking channels','MarketingMessageText1': 'For your safety we have placed a limit on the withdrawal amount to any party, when you use digital and mobile banking channels. This limit does not apply to withdrawals we pay to your own Nedbank accounts.','MarketingMessageText2': 'Remember to report any suspicious activity on your account by calling 0860 555 111.','MarketingMessageText3': '','MarketingMessageText4': '','MarketingMessageText5': ''}]";

                if (pages.Count != 0)
                {
                    var isBackgroundImage = false;
                    var MarketingMessageCounter = 0;
                    htmlString.Append(HtmlConstants.CONTAINER_DIV_HTML_HEADER);
                    for (int y = 0; y < pages.Count; y++)
                    {
                        var page = pages[y];
                        string tabClassName = Regex.Replace(page.DisplayName, @"\s+", "-");
                        var pageHeaderHtml = HtmlConstants.PAGE_HEADER_HTML;
                        var extraclass = string.Empty;
                        if (page.BackgroundImageAssetId != 0)
                        {
                            pageHeaderHtml = pageHeaderHtml.Replace("{{BackgroundImage}}", "");
                            extraclass = extraclass + "BackgroundImage " + page.BackgroundImageAssetId;
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

                        htmlString.Append(pageHeaderHtml.Replace("{{DivId}}", tabClassName).Replace("{{ExtraClass}}", extraclass));
                        int tempRowWidth = 0;
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
                                            htmlString.Append("<div class='row pt-2'>");
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
                                            htmlString.Append("</div>");
                                            htmlString.Append("<div class='row pt-2'>");
                                            isRowComplete = false;
                                        }
                                        
                                        var PaddingClass = i != 0 ? " pl-0" : "";
                                        if (MarketingMessages.Length > 0 && mergedlst[i].WidgetName == HtmlConstants.SERVICE_WIDGET_NAME)
                                        {
                                            PaddingClass = MarketingMessageCounter % 2 == 0 ? " pr-1" : " pl-1";
                                        }
                                        htmlString.Append("<div class='col-lg-" + divLength + PaddingClass + "'>");

                                        if (!mergedlst[i].IsDynamicWidget)
                                        {
                                            if (mergedlst[i].WidgetName == HtmlConstants.CUSTOMER_INFORMATION_WIDGET_NAME)
                                            {
                                                string customerInfoJson = "{'FirstName':'Laura','MiddleName':'J','LastName':'Donald','AddressLine1':'231 Exe Parkway','AddressLine2':'Saint Globin Rd','City':'Canary Wharf','State':'London','Country':'England','Zip':'E14 9RZ'}";
                                                if (customerInfoJson != string.Empty && validationEngine.IsValidJson(customerInfoJson))
                                                {
                                                    CustomerInformation customerInfo = JsonConvert.DeserializeObject<CustomerInformation>(customerInfoJson);
                                                    var customerHtmlWidget = HtmlConstants.CUSTOMER_INFORMATION_WIDGET_HTML.Replace("{{VideoSource}}", "assets/images/SampleVideo.mp4");
                                                    customerHtmlWidget = customerHtmlWidget.Replace("{{WidgetDivHeight}}", divHeight);

                                                    string customerName = customerInfo.FirstName + " " + customerInfo.MiddleName + " " + customerInfo.LastName;
                                                    customerHtmlWidget = customerHtmlWidget.Replace("{{CustomerName}}", customerName);

                                                    string address1 = customerInfo.AddressLine1 + ", " + customerInfo.AddressLine2 + ", ";
                                                    customerHtmlWidget = customerHtmlWidget.Replace("{{Address1}}", address1);

                                                    string address2 = (customerInfo.City != "" ? customerInfo.City + ", " : "") + (customerInfo.State != "" ?
                                                        customerInfo.State + ", " : "") + (customerInfo.Country != "" ? customerInfo.Country + ", " : "") +
                                                        (customerInfo.Zip != "" ? customerInfo.Zip : "");
                                                    customerHtmlWidget = customerHtmlWidget.Replace("{{Address2}}", address2);

                                                    htmlString.Append(customerHtmlWidget);
                                                }
                                            }
                                            else if (mergedlst[i].WidgetName == HtmlConstants.ACCOUNT_INFORMATION_WIDGET_NAME)
                                            {
                                                string accountInfoJson = "{'StatementDate':'1-APR-2020','StatementPeriod':'Annual Statement','CustomerID':'ID2-8989-5656','RmName':'James Wiilims','RmContactNumber':'+4487867833'}";

                                                string accountInfoData = string.Empty;
                                                StringBuilder AccDivData = new StringBuilder();
                                                if (accountInfoJson != string.Empty && validationEngine.IsValidJson(accountInfoJson))
                                                {
                                                    AccountInformation accountInfo = JsonConvert.DeserializeObject<AccountInformation>(accountInfoJson);
                                                    AccDivData.Append("<div class='list-row-small ht70px'><div class='list-middle-row'> <div class='list-text'>Statement Date" + "</div><label class='list-value mb-0'>" + accountInfo.StatementDate + "</label></div></div>");

                                                    AccDivData.Append("<div class='list-row-small ht70px'><div class='list-middle-row'> <div class='list-text'>Statement Period" + "</div><label class='list-value mb-0'>" + accountInfo.StatementPeriod + "</label></div></div>");

                                                    AccDivData.Append("<div class='list-row-small ht70px'><div class='list-middle-row'> <div class='list-text'>Cusomer ID" +
                                                        "</div><label class='list-value mb-0'>" + accountInfo.CustomerID + "</label></div></div>");

                                                    AccDivData.Append("<div class='list-row-small ht70px'><div class='list-middle-row'> <div class='list-text'>RM Name" + "</div><label class='list-value mb-0'>" + accountInfo.RmName + "</label></div></div>");

                                                    AccDivData.Append("<div class='list-row-small ht70px'><div class='list-middle-row'> <div class='list-text'>RM Contact Number" + "</div><label class='list-value mb-0'>" + accountInfo.RmContactNumber + "</label></div></div>");

                                                    accountInfoData = HtmlConstants.ACCOUNT_INFORMATION_WIDGET_HTML.Replace("{{AccountInfoData}}", AccDivData.ToString());
                                                }
                                                else
                                                {
                                                    AccDivData.Append("<div class='list-row-small ht70px'><div class='list-middle-row'> <div class='list-text'>No Record" + "</div><label class='list-value mb-0'>Found</label></div></div>");
                                                    accountInfoData = HtmlConstants.ACCOUNT_INFORMATION_WIDGET_HTML.Replace("{{AccountInfoData}}", AccDivData.ToString());
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
                                                string accountBalanceDataJson = "[{\"AccountType\":\"Saving Account\",\"Currency\":\"$\",\"Amount\":\"8356\"},{\"AccountType\":\"Current Account\",\"Currency\":\"$\",\"Amount\":\"5654\"},{\"AccountType\":\"Recurring Account\",\"Currency\":\"$\",\"Amount\":\"4367\"},{\"AccountType\":\"Wealth\",\"Currency\":\"$\",\"Amount\":\"4589\"}]";

                                                string accountSummary = string.Empty;
                                                if (accountBalanceDataJson != string.Empty && validationEngine.IsValidJson(accountBalanceDataJson))
                                                {
                                                    IList<AccountSummary> lstAccountSummary = JsonConvert.DeserializeObject<List<AccountSummary>>(accountBalanceDataJson);
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
                                                string transactionJson = "[{ 'TransactionDate': '15/07/2020', 'TransactionType': 'CR', 'Narration': 'NXT TXN: IIFL IIFL6574562', 'FCY': '1666.67', 'CurrentRate': '1.062', 'LCY': '1771.42' },{ 'TransactionDate': '15/07/2020', 'TransactionType': 'CR', 'Narration': 'NXT TXN: IIFL IIFL6574563', 'FCY': '1435.00', 'CurrentRate': '0.962', 'LCY': '1654.56' },{ 'TransactionDate': '19/07/2020', 'TransactionType': 'CR', 'Narration': 'NXT TXN: IIFL IIFL3557346', 'FCY': '1254.71', 'CurrentRate': '1.123', 'LCY': '1876.00' }, { 'TransactionDate': '25/07/2020', 'TransactionType': 'CR', 'Narration': 'NXT TXN: IIFL IIFL8965435', 'FCY': '2345.12', 'CurrentRate': '1.461', 'LCY': '1453.21' }, { 'TransactionDate': '28/07/2020', 'TransactionType': 'CR', 'Narration': 'NXT TXN: IIFL IIFL0034212', 'FCY': '1435.00', 'CurrentRate': '0.962', 'LCY': '1654.56' }]";
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
                                                string transactionJson = "[{ 'TransactionDate': '15/07/2020', 'TransactionType': 'CR', 'Narration': 'NXT TXN: IIFL IIFL6574562', 'FCY': '1666.67', 'CurrentRate': '1.062', 'LCY': '1771.42' },{ 'TransactionDate': '15/07/2020', 'TransactionType': 'CR', 'Narration': 'NXT TXN: IIFL IIFL6574563', 'FCY': '1435.00', 'CurrentRate': '0.962', 'LCY': '1654.56' },{ 'TransactionDate': '19/07/2020', 'TransactionType': 'CR', 'Narration': 'NXT TXN: IIFL IIFL3557346', 'FCY': '1254.71', 'CurrentRate': '1.123', 'LCY': '1876.00' }, { 'TransactionDate': '25/07/2020', 'TransactionType': 'CR', 'Narration': 'NXT TXN: IIFL IIFL8965435', 'FCY': '2345.12', 'CurrentRate': '1.461', 'LCY': '1453.21' }, { 'TransactionDate': '28/07/2020', 'TransactionType': 'CR', 'Narration': 'NXT TXN: IIFL IIFL0034212', 'FCY': '1435.00', 'CurrentRate': '0.962', 'LCY': '1654.56' }]";
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
                                                            tdstring = "<span class='fa fa-sort-asc fa-2x mt-1' aria-hidden='true' " + "style='position:relative;top:6px;color:limegreen'></span><span class='ml-2'>" + item.AverageSpend + "</span>";
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
                                                string reminderJson = "[{ 'Title': 'Update Missing Inofrmation', 'Action': 'Update' },{ 'Title': 'Your Rewards Video is available', 'Action': 'View' },{ 'Title': 'Payment Due for Home Loan', 'Action': 'Pay' }, { title: 'Need financial planning for savings.', action: 'Call Me' },{ title: 'Subscribe/Unsubscribe Alerts.', action: 'Apply' },{ title: 'Your credit card payment is due now.', action: 'Pay' }]";
                                                if (reminderJson != string.Empty && validationEngine.IsValidJson(reminderJson))
                                                {
                                                    IList<ReminderAndRecommendation> reminderAndRecommendations = JsonConvert.DeserializeObject<List<ReminderAndRecommendation>>(reminderJson);
                                                    StringBuilder reminderstr = new StringBuilder();
                                                    //reminderstr.Append("<div class='row'><div class='col-lg-9'></div><div class='col-lg-3 text-left pl-0'><i class='fa fa-caret-left fa-2x float-left text-danger' aria-hidden='true'></i><span class='mt-2 d-inline-block ml-2'>Click</span></div> </div>");
                                                    reminderAndRecommendations.ToList().ForEach(item =>
                                                    {
                                                        reminderstr.Append("<div class='row'><div class='col-lg-9 text-left'><p class='p-1' style='background-color: #dce3dc;'>" + item.Title + " </p></div><div class='col-lg-3 text-left pl-0'><a><i class='fa fa-caret-left fa-2x float-left " + "text-danger'></i><span class='mt-2 d-inline-block ml-2'>" + item.Action + "</span></a></div></div>");
                                                    });
                                                    string widgetstr = HtmlConstants.REMINDER_WIDGET_HTML.Replace("{{ReminderAndRecommdationDataList}}", reminderstr.ToString());
                                                    widgetstr = widgetstr.Replace("{{WidgetDivHeight}}", divHeight);
                                                    htmlString.Append(widgetstr);
                                                }
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
                                                string jsonstr = "{'BankName': 'Nedbank', 'AddressLine1':'135 Rivonia Road, Sandton, 2196', 'AddressLine2':'PO Box 1144, Johannesburg, 2000', 'CountryName':'South Africa','BankVATRegNo':'4320116074','ContactCenterNo':'0860 555 111'}";
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
                                                string jsonstr = "{'Currency': 'R', 'TotalClosingBalance': '23 920.98', 'DayOfStatement':'21', 'InvestorId':'204626','StatementPeriod':'22/12/2020 to 21/01/2021','StatementDate':'21/01/2021', 'DsInvestorName' : '' }";
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
                                                string jsonstr = "[{'InvestorId': '204626','InvestmentId': '9974','Currency': 'R','ProductType': 'Notice Deposits','ProductDesc': 'JustInvest', 'OpenDate': '05/09/1994','CurrentInterestRate': '2.95','ExpiryDate': '','InterestDisposalDesc': 'Capitalise','NoticePeriod': '1 day', 'AccuredInterest': '2.94','Transactions': [{'TransactionDate': '26/10/2020','TransactionDesc': 'Balance brought forward','Debit': '0','Credit': '0', 'Balance': '5 297.02'},{'TransactionDate': '16/11/2020','TransactionDesc': 'Interest capitalised','Debit': '0','Credit': '10.12','Balance': '0'},{'TransactionDate': '25/11/2020','TransactionDesc': 'Balance carried forward','Debit': '0','Credit': '0','Balance': '5 307.14'}]},{'InvestorId': '204626','InvestmentId': '9978','Currency': 'R','ProductType': 'Notice Deposits','ProductDesc': 'JustInvest','OpenDate': '12/11/1996', 'CurrentInterestRate': '2.25','ExpiryDate': '','InterestDisposalDesc': 'Capitalise','NoticePeriod': '1 day','AccuredInterest': '24.10', 'Transactions': [{'TransactionDate': '26/10/2020','TransactionDesc': 'Balance brought forward','Debit': '0','Credit': '0','Balance': '18 578.34'},{'TransactionDate': '16/11/2020','TransactionDesc': 'Interest capitalised','Debit': '0','Credit': '35.50','Balance': '0'},{'TransactionDate': '25/11/2020','TransactionDesc': 'Balance carried forward','Debit': '0','Credit': '0','Balance': '18 613.84'}]}]";

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
                                                    IList<EntityFieldMap> fieldMaps = new List<EntityFieldMap>();
                                                    DynamicWidgetLineGraph lineGraphDetails = JsonConvert.DeserializeObject<DynamicWidgetLineGraph>(dynawidget.WidgetSettings);
                                                    fieldMaps = this.dynamicWidgetManager.GetEntityFields(dynawidget.EntityId, tenantCode);
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
                                                    var series = dynawidget.PreviewData;
                                                    lineGraphWidgetHtml = lineGraphWidgetHtml + "<input type='hidden' id='hiddenLineGraphData_" + dynawidget.Identifier + "' value='" + series + "'>";
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
                                                    IList<EntityFieldMap> fieldMaps = new List<EntityFieldMap>();
                                                    DynamicWidgetLineGraph lineGraphDetails = JsonConvert.DeserializeObject<DynamicWidgetLineGraph>(dynawidget.WidgetSettings);
                                                    fieldMaps = this.dynamicWidgetManager.GetEntityFields(dynawidget.EntityId, tenantCode);
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
                                                    var series = dynawidget.PreviewData;
                                                    barGraphWidgetHtml = barGraphWidgetHtml + "<input type='hidden' id='hiddenBarGraphData_" + dynawidget.Identifier + "' value='" + series + "'>";
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
                                                    PieChartSettingDetails pieChartSetting = JsonConvert.DeserializeObject<PieChartSettingDetails>(dynawidget.WidgetSettings);
                                                    IList<EntityFieldMap> fieldMaps = new List<EntityFieldMap>();
                                                    fieldMaps = this.dynamicWidgetManager.GetEntityFields(dynawidget.EntityId, tenantCode);
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
                                                    var series = dynawidget.PreviewData;
                                                    pieChartWidgetHtml = pieChartWidgetHtml + "<input type='hidden' id='hiddenPieChartData_" + dynawidget.Identifier + "' value='" + series + "'>";
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
                                            htmlString.Append("</div>");
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
                    htmlString.Append(HtmlConstants.CONTAINER_DIV_HTML_FOOTER);

                    if (linegraphIds.Count > 0)
                    {
                        var ids = string.Join(",", linegraphIds.Select(item => item).ToList());
                        htmlString.Append("<input type = 'hidden' id = 'hiddenLineChartIds' value = '" + ids + "'>");
                    }
                    if (bargraphIds.Count > 0)
                    {
                        var ids = string.Join(",", bargraphIds.Select(item => item).ToList());
                        htmlString.Append("<input type = 'hidden' id = 'hiddenBarChartIds' value = '" + ids + "'>");
                    }
                    if (piechartIds.Count > 0)
                    {
                        var ids = string.Join(",", piechartIds.Select(item => item).ToList());
                        htmlString.Append("<input type = 'hidden' id = 'hiddenPieChartIds' value = '" + ids + "'>");
                    }


                    //if (isBackgroundImage)
                    //{
                    //    htmlString.Replace("card border-0", "card border-0 bg-transparent");
                    //    htmlString.Replace("bg-light", "bg-transparent");
                    //}
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return htmlString.ToString();
        }

        /// <summary>
        /// This method will call get page types method of repository.
        /// </summary>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns>
        /// Returns page types
        /// </returns>
        public IList<PageType> GetPageTypes(string tenantCode)
        {
            try
            {
                return this.pageRepository.GetPageTypes(tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// This method gets the specified list of static as well as dynamic widgets from widgets and dynamic widgets repository.
        /// </summary>
        /// <param name="pageTypeId">The page type identifier</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of static as well as dynamic widgets
        /// </returns>
        public IList<Widget> GetStaticAndDynamicWidgets(long pageTypeId, string tenantCode)
        {
            try
            {
                return this.pageRepository.GetStaticAndDynamicWidgets(pageTypeId, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// This method is responsible for validate pages.
        /// </summary>
        /// <param name="pages"></param>
        /// <param name="tenantCode"></param>
        private void IsValidPages(IList<Page> pages, string tenantCode)
        {
            try
            {
                if (pages?.Count <= 0)
                {
                    throw new NullArgumentException(tenantCode);
                }

                InvalidPageException invalidpageException = new InvalidPageException(tenantCode);
                pages.ToList().ForEach(item =>
                {
                    try
                    {
                        item.IsValid();
                    }
                    catch (Exception ex)
                    {
                        invalidpageException.Data.Add(item.DisplayName, ex.Data);
                    }
                });

                if (invalidpageException.Data.Count > 0)
                {
                    throw invalidpageException;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method helps to check duplicate page in the list
        /// </summary>
        /// <param name="pages"></param>
        /// <param name="tenantCode"></param>
        private void IsDuplicatePage(IList<Page> pages, string tenantCode)
        {
            try
            {
                int isDuplicatePage = pages.GroupBy(p => p.DisplayName).Where(g => g.Count() > 1).Count();
                if (isDuplicatePage > 0)
                {
                    throw new DuplicatePageFoundException(tenantCode);
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

        #endregion
    }
}
