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

        /// <summary>
        /// The utility object
        /// </summary>
        private IUtility utility = null;
        private IPPSRepository ppsRepository = null;
        //IInvestmentRepository investmentRepository = null;

        // ICustomerRepository customerRepository = null;

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
                //this.investmentRepository = this.unityContainer.Resolve<IInvestmentRepository>();
                //this.customerRepository = this.unityContainer.Resolve<ICustomerRepository>();
                this.tenantConfigurationManager = new TenantConfigurationManager(unityContainer);
                this.assetLibraryRepository = this.unityContainer.Resolve<IAssetLibraryRepository>();
                this.dynamicWidgetManager = new DynamicWidgetManager(unityContainer);
                this.validationEngine = new ValidationEngine();
                this.utility = new Utility();
                this.ppsRepository = this.unityContainer.Resolve<IPPSRepository>();
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
                   
            //var re = ppsRepository.spIAA_PaymentDetail_fspstatement(tenantCode);
            // var re = ppsRepository.spIAA_Commission_Detail_ppsStatement(tenantCode);
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

                //var IsInvestmentStatement = pages.Where(it => it.PageTypeName == HtmlConstants.INVESTMENT_PAGE_TYPE_OTHER_ENGLISH || it.PageTypeName == HtmlConstants.WEALTH_INVESTMENT_PAGE_TYPE_WEALTH_ENGLISH || it.PageTypeName == HtmlConstants.INVESTMENT_PAGE_TYPE_OTHER_AFRICAN || it.PageTypeName == HtmlConstants.WEALTH_INVESTMENT_PAGE_TYPE_WEALTH_AFRICAN).ToList().Count> 0;
                //var IsPersonalLoanStatement = pages.Where(it => it.PageTypeName == HtmlConstants.PERSONAL_LOAN_PAGE_TYPE).ToList().Count> 0;
                //var IsHomeLoanStatement = pages.Where(it => it.PageTypeName == HtmlConstants.HOME_LOAN_FOR_OTHER_SEGMENT_ENG_PAGE_TYPE || it.PageTypeName == HtmlConstants.HOME_LOAN_FOR_OTHER_SEGMENT_AFR_PAGE_TYPE || it.PageTypeName == HtmlConstants.HOME_LOAN_FOR_PML_SEGMENT_ENG_PAGE_TYPE || it.PageTypeName == HtmlConstants.HOME_LOAN_FOR_PML_SEGMENT_AFR_PAGE_TYPE || it.PageTypeName == HtmlConstants.HOME_LOAN_FOR_WEA_SEGMENT_AFR_PAGE_TYPE || it.PageTypeName == HtmlConstants.HOME_LOAN_FOR_WEA_SEGMENT_ENG_PAGE_TYPE).ToList().Count> 0;
                var MarketingMessages = string.Empty;
                //if (IsInvestmentStatement)
                //{
                //    MarketingMessages = HtmlConstants.INVESTMENT_MARKETING_MESSAGE_JSON_STR;
                //}
                //else if (IsPersonalLoanStatement)
                //{
                //    MarketingMessages = HtmlConstants.PERSONAL_LOAN_MARKETING_MESSAGE_JSON_STR;
                //}
                //else if (IsHomeLoanStatement)
                //{
                //    MarketingMessages = HtmlConstants.HOME_LOAN_MARKETING_MESSAGE_JSON_STR;
                //}
                //else
                //{
                //    MarketingMessages = HtmlConstants.PERSONAL_LOAN_MARKETING_MESSAGE_JSON_STR;
                //}

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

                                        var PaddingClass = i != 0 ? " pl-0" : string.Empty;
                                        //if (MarketingMessages.Length> 0 && mergedlst[i].WidgetName == HtmlConstants.SERVICE_WIDGET_NAME)
                                        //{
                                        //    //to add Nedbank services header... to do-- Create separate static widgets for widget's header label
                                        //    if (MarketingMessageCounter == 0)
                                        //    {
                                        //        //htmlString.Append("<div class='col-lg-12'><div class='card border-0'><div class='card-body text-left py-0'><div class='card-body-header pb-2'>Nedbank Services</div></div></div></div></div><div class='row'>");
                                        //    }
                                        //    PaddingClass = MarketingMessageCounter % 2 == 0 ? " pr-1 pl-35px" : " pl-1 pr-35px";
                                        //}
                                        //else if (MarketingMessages.Length> 0 && mergedlst[i].WidgetName == HtmlConstants.WEALTH_SERVICE_WIDGET_NAME)
                                        //{
                                        //    //to add Nedbank services header... to do-- Create separate static widgets for widget's header label
                                        //    if (MarketingMessageCounter == 0)
                                        //    {
                                        //        //htmlString.Append("<div class='col-lg-12'><div class='card border-0'><div class='card-body text-left py-0'><div class='card-body-header-w pb-2'>Nedbank Services</div></div></div></div></div><div class='row'>");
                                        //    }
                                        //    PaddingClass = MarketingMessageCounter % 2 == 0 ? " pr-1 pl-35px" : " pl-1 pr-35px";
                                        //}
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

                                                    string customerName = customerInfo.FirstName + " " + customerInfo.SurName;
                                                    customerHtmlWidget = customerHtmlWidget.Replace("{{CustomerName}}", customerName);

                                                    string address1 = customerInfo.AddressLine1 + ", " + customerInfo.AddressLine2 + ", ";
                                                    customerHtmlWidget = customerHtmlWidget.Replace("{{Address1}}", address1);

                                                    string address2 = customerInfo.AddressLine1 + ", " + customerInfo.AddressLine2 + ", ";
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
                                            else if (mergedlst[i].WidgetName == HtmlConstants.PPS_FOOTER1_WIDGET_NAME)
                                            {
                                                string ppsFooter1InfoJson = "{Reg_ID : 1,Start_Date : '2023-01-01',End_Date : '2023-01-01',Request_DateTime : 'DummyText1',ID : '124529534',Intermediary_Code : 'DummyText1',FSP_ID : 'DummyText1',Policy_Number : 'DummyText1',FSP_Party_ID : 'DummyText1',Client_Number : '124556686',FSP_REF : '2452953',Client_Name : 'Mr SCHOELER',Int_ID : 'DummyText1',Product_Type : 'DummyText1',Commission_Amount : 'DummyText1',INT_EXT_REF : '124411745',Int_Name : 'Kruger Van Heerden',Int_Type : 'DummyText1',Policy_Ref : '5596100',Member_Ref : '124556686',Member_Name : 'DummyText1',Transaction_Amount : 'DummyText1',Mem_Age : 'DummyText1',Months_In_Force : 'DummyText1',Commission_Type : 'Safe Custody Fee',Description : 'Safe Custody Service Fee',POSTED_DATE : '2023-03-03',AE_Type_ID : 'DummyText1',AE_Amount : 'DummyText1',DR_CR : 'DummyText1',NAME : 'DummyText1',Member_Surname : 'DummyText1',Jurisdiction : 'DummyText1',Sales_Office : 'DummyText1',FSP_Name : 'Miss Yvonne van Heerden',FSP_Trading_Name : 'T/A Yvonne Van Heerden Financial Planner CC',FSP_Ext_Ref : '124529534',FSP_Kind : 'DummyText1',FSP_VAT_Number : '2452953',Product : 'DummyText1',Prod_Group : 'Service Fee',Prod_Seq : 'DummyText1',Report_Seq : 'DummyText1',TYPE : 'DummyText1',Display_Amount : '17.55',VAT_Amount : '38001.27',Earning_Amount : '256670.66',Payment_Amount : 'DummyText1',Business_Type : 'DummyText1',Lifecycle_Description : 'DummyText1',Lifecycle_Start_Date : 'DummyText1',AE_Scheduler_ID : 'DummyText1',VAT_Amount_1 : 'DummyText1',Final_Amount : 'DummyText1'}";
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
                                                        aeAmountColSumR = CommonUtility.concatRWithDouble(aeAmountColSum.ToString());
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

                                                    var balance = Convert.ToDouble((grandTotalDue - ppsPayment)).ToString("F2");
                                                    // Calculate and update the balance in the HTML string
                                                    productSumstring = productSumstring.Replace("{{Balance}}", CommonUtility.concatRWithDouble(balance));

                                                    // Append the modified product summary widget HTML to the main HTML string
                                                    htmlString.Append(productSumstring);
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
                                                        detailedTransactionSrc.Append("<tr><td class='px-1 py-1 fsp-bdr-right fsp-bdr-bottom text-left'>" + item.Client_Name + "</td><td class= 'fsp-bdr-right fsp-bdr-bottom px-1 text-left'>" + item.Member_Ref + "</td><td class= 'fsp-bdr-right fsp-bdr-bottom px-1 text-left'> " + item.Policy_Ref + "</td><td class= 'text-left fsp-bdr-right fsp-bdr-bottom px-1'>" + (item.Description == "Commission Service Fee" ? "Premium Under Advise Fee" : item.Description) + "</td><td class= 'text-left fsp-bdr-right fsp-bdr-bottom px-1'>" + item.Commission_Type + "</td><td class= 'text-left text-nowrap fsp-bdr-right fsp-bdr-bottom px-1'>" + item.POSTED_DATE.ToString("dd-MMM-yyyy") + "</td><td class= 'text-right fsp-bdr-right fsp-bdr-bottom px-1'>" + Utility.FormatCurrency(item.Display_Amount) + "</td><td class= 'text-center fsp-bdr-bottom px-1'><a href ='https://www.google.com/' target ='_blank'><img class='leftarrowlogo' src='assets/images/leftarrowlogo.png' alt='Left Arrow'></a></td></tr>");
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
                                            {//10-12-2023  00:00:00
                                                string transactionListJson = "[{'INT_EXT_REF':'124565256 ','POLICY_REF':'3830102','MEMBER_REF':'10024365','Member_Name':'Dr JC Arthur','BUS_GROUP':'PPS INSURANCE','PRODUCT_DESCRIPTION':'Professional Health Provider Whole Life Professional Health',  'REQUESTED_DATETIME':'01-11-2022  00:00:00', 'AE_Posted_Date':'10-12-2023  00:00:00','REQUEST_DATETIME':'2022-09-23','TRANSACTION_AMOUNT':2265.4 ,'ALLOCATED_AMOUNT':-23107.08 ,'MEMBER_AGE':'45 ','MeasureType':'Commission','CommissionType':'2nd Year','FSP_Name':'Miss HW HLONGWANE'},     {'INT_EXT_REF':'124565256 ','POLICY_REF':'3830102','MEMBER_REF':'10024365','Member_Name':'Dr JC Arthur','BUS_GROUP':'PPS INSURANCE','PRODUCT_DESCRIPTION':'Professional Health Provider Whole Life Level Rated Professional Health CatchAll Exercised ',  'REQUESTED_DATETIME':'01-11-2022  00:00:00', 'AE_Posted_Date':'10-12-2023  00:00:00', 'REQUEST_DATETIME':'2022-09-23','TRANSACTION_AMOUNT':84.97 ,'ALLOCATED_AMOUNT':-866.69 ,'MEMBER_AGE':'45 ',  'MeasureType':'Commission','CommissionType':'2nd Year','FSP_Name':'Miss HW HLONGWANE'},     {'INT_EXT_REF':'124565256 ','POLICY_REF':'3830102','MEMBER_REF':'10024365','Member_Name':'Dr JC Arthur','BUS_GROUP':'PPS INSURANCE','PRODUCT_DESCRIPTION':'Professional Health Provider Whole Life Professional Health',  'REQUESTED_DATETIME':'01-11-2022  00:00:00', 'AE_Posted_Date':'10-12-2023  00:00:00', 'REQUEST_DATETIME':'2022-09-23','TRANSACTION_AMOUNT':2265.4,'ALLOCATED_AMOUNT':10968.98,'MEMBER_AGE':'45 ',  'MeasureType':'Commission','CommissionType':'2nd Year','FSP_Name':'Miss HW HLONGWANE'},     {'INT_EXT_REF':'124565256 ','POLICY_REF':'3820110 ','MEMBER_REF':'10436136 ','Member_Name':'Mnr JG Rossouw ','BUS_GROUP':'PPS INSURANCE','PRODUCT_DESCRIPTION':'Professional Health Provider Whole Life Professional Health',  'REQUESTED_DATETIME':'01-11-2022 00:00 ', 'AE_Posted_Date':'10-12-2023  00:00:00', 'REQUEST_DATETIME':'2022-09-28','TRANSACTION_AMOUNT':928.89 ,'ALLOCATED_AMOUNT':-9474.68 ,'MEMBER_AGE':'43',  'MeasureType':'Commission','CommissionType':'1st Year','FSP_Name':'Miss HW HLONGWANE'},     {'INT_EXT_REF':'124565256 ','POLICY_REF':'3820110 ','MEMBER_REF':'10436136 ','Member_Name':'Mnr JG Rossouw ','BUS_GROUP':'PPS INSURANCE','PRODUCT_DESCRIPTION':'Professional Health Provider Whole Life Professional Health',  'REQUESTED_DATETIME':'01-11-2022 00:00 ', 'AE_Posted_Date':'10-12-2023  00:00:00', 'REQUEST_DATETIME':'2022-09-28','TRANSACTION_AMOUNT':928.89 ,'ALLOCATED_AMOUNT':6072.47 ,'MEMBER_AGE':'43',  'MeasureType':'Commission','CommissionType':'2nd Year','FSP_Name':'Miss HW HLONGWANE'}]  ";

                                                if (transactionListJson != string.Empty && validationEngine.IsValidJson(transactionListJson))
                                                {
                                                    IList<spIAA_Commission_Detail> ppsDetails = JsonConvert.DeserializeObject<List<spIAA_Commission_Detail>>(transactionListJson);
                                                    StringBuilder detailedTransactionSrc = new StringBuilder();
                                                    string detailedTransactionString = HtmlConstants.PPS_DETAILED_TRANSACTIONS_WIDGET_HTML;
                                                    detailedTransactionSrc.Append("<div class='pps-monthly-table w-100'><table cellpadding='0' cellspacing='0' width='100%'><tr class='text-left'><th class='bdr-right-white sky-blue-bg text-white font-weight-bold'  width='10%'>Client<br/>name</th><th class='bdr-right-white sky-blue-bg text-white font-weight-bold' width='3%'>Age</th><th class='bdr-right-white sky-blue-bg text-white font-weight-bold' width='5%'>Member number</th><th class='bdr-right-white sky-blue-bg text-white font-weight-bold' width='5%'>Policy number</th><th class='bdr-right-white sky-blue-bg text-white font-weight-bold' width='26%'>Product</th><th class='bdr-right-white sky-blue-bg text-white font-weight-bold' width='9%'>Date<br/>issued</th><th class='bdr-right-white sky-blue-bg text-white font-weight-bold' width='9%'>Inception<br/>date</th><th class='bdr-right-white sky-blue-bg text-white font-weight-bold' width='7%'>Com<br/>type</th><th class='bdr-right-white sky-blue-bg text-white font-weight-bold' width='9%'>Quantity</th><th class='bdr-right-white sky-blue-bg text-white font-weight-bold' width='9%'>Posted<br/>date</th><th class='bdr-right-white sky-blue-bg text-white font-weight-bold' width='7%'>Earnings</th></tr>");
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
                                                                  "<td class='bdr-right-white text-left'>" + memberitemrecord.POLICY_REF + "</td><td class='bdr-right-white text-left'>" + memberitemrecord.PRODUCT_DESCRIPTION + "</td><td class='bdr-right-white text-left'>" + memberitemrecord.REQUEST_DATETIME.ToString("dd-MMM-yyyy") + "</td><td class='bdr-right-white text-left text-nowrap'>" + memberitemrecord.REQUESTED_DATETIME.ToString("dd-MMM-yyyy") + "</td><td class='bdr-right-white text-left'>" + memberitemrecord.CommissionType + "</td><td class='bdr-right-white text-right text-nowrap'>" + Utility.FormatCurrency(memberitemrecord.TRANSACTION_AMOUNT) + "</td><td class='bdr-right-white text-left text-nowrap'>"
                                                                  + memberitemrecord.AE_Posted_Date.ToString("dd-MMM-yyyy") + "</td>" +
                                                                    "<td class='bdr-right-white ewidth text-right text-nowrap'>" + Utility.FormatCurrency(memberitemrecord.ALLOCATED_AMOUNT) + "</td></tr>");
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
                                                string ppsDetails1InfoJson = "{'Request_ID':1,'AE_TYPE_ID':'20','INT_EXT_REF':'124529534','POLICY_REF':'October','MEMBER_REF':'Payment Details','Member_Name':'DummyText1','BUS_GROUP':'SERVICE FEES','PRODUCT_DESCRIPTION':'Professional Health Provider Whole Life Professional Health',    'OID':'DummyText1','MeasureType':'Commission','CommissionType':'2nd Year','TRANSACTION_AMOUNT':65566.20,'ALLOCATED_AMOUNT':65566.20,'MEMBER_AGE':'DummyText1','MONTHS_IN_FORCE':'DummyText1','REQUEST_DATETIME':'2023-01-01','REQUESTED_DATETIME':'2023-09-01','AE_agmt_id':'DummyText1','AE_agmt_type_id':'5596100','AE_Posted_Date':'2023-09-01','AE_Amount':'65566.20','Acc_Name':'DummyText1','FSP_Name':'Miss HW HLONGWANE','DUE_DATE':'2023-09-01','YEAR_START_DATE':'2023-01-01','YEAR_END_DATE':'2023-09-01','Type':'DummyText1', 'Req_Year':'2023-01-01','FutureEndDate':'2023-01-01','Calc1stYear':10000,'Calc2ndYear':20000,'MonthRange':'DummyText1','calcMain2ndYear':30000 }";
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
                                                //DateTime DateFrom = new DateTime(2023, 01, 01);
                                                //DateTime DateTo = new DateTime(2023, 09, 01);
                                                string ppsDetails2InfoJson = "{'Request_ID':1,'AE_TYPE_ID':'20','INT_EXT_REF':'124529534','POLICY_REF':'October','MEMBER_REF':'Payment Details','Member_Name':'DummyText1','BUS_GROUP':'SERVICE FEES','PRODUCT_DESCRIPTION':'Professional Health Provider Whole Life Professional Health',    'OID':'DummyText1','MeasureType':'Commission','CommissionType':'2nd Year','TRANSACTION_AMOUNT':65566.20,'ALLOCATED_AMOUNT':65566.20,'MEMBER_AGE':'DummyText1','MONTHS_IN_FORCE':'DummyText1','REQUEST_DATETIME':'2023-01-01','REQUESTED_DATETIME':'2023-09-01','AE_agmt_id':'DummyText1','AE_agmt_type_id':'5596100','AE_Posted_Date':'2023-09-01','AE_Amount':'65566.20','Acc_Name':'DummyText1','FSP_Name':'Miss HW HLONGWANE','DUE_DATE':'2023-09-01','YEAR_START_DATE':'2023-01-01','YEAR_END_DATE':'2023-09-01','Type':'DummyText1', 'Req_Year':'2023-01-01','FutureEndDate':'2023-01-01','Calc1stYear':10000,'Calc2ndYear':20000,'MonthRange':'DummyText1','calcMain2ndYear':30000 }";
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
                                                            commisionDetailSrc.Append("<tr><td class='text-left text-nowrap'>" + DateTime.Parse(gpMonthRangeItem.GroupKey.Date.ToString()).ToString("dd-MMM-yyyy") + "</td>");
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
                                                    commisionDetailSrc.Append("<!-- Monthly production summary Section --><div class='earnings-section-monthly d-flex'><!-- Two Columns Layout --><div class='d-flex gap-1 w-100'><!-- Monthly production summary T1 --><div class='col-6'><!-- Heading for Monthly production summary T1 --><h4 class='monthly-production-summary skyblue-bg-title text-white text-center'>Monthly production summary</h4><div class='monthly-table'><!-- Table for Monthly production summary T1 --><table width='100%' cellpadding='0' cellspacing='0'><!-- Table Headers --><thead><tr><th class='text-white font-weight-bold text-left text-nowrap'>Month</th>");

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
                                                        commisionDetailSrc.Append("<tr><td class='text-nowrap text-left'>" + CommonUtility.GetMonthRange(gpFeesMonthRangeItem.GroupKey.Month) + "</td>");
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
                                                var pageBreakWidget = HtmlConstants.PAGE_BREAK_WIDGET_HTML.Replace("{{PageBreak}}", html);
                                                pageBreakWidget = pageBreakWidget.Replace("{{WidgetDivHeight}}", divHeight);
                                                htmlString.Append(pageBreakWidget);
                                            }
                                            else if (mergedlst[i].WidgetName == HtmlConstants.STATIC_SEGMENT_BASED_CONTENT_NAME)
                                            {
                                                var html = "<div>This is sample SegmentBasedContent</div>";
                                                if (mergedlst[i].WidgetSetting != string.Empty && validationEngine.IsValidJson(mergedlst[i].WidgetSetting))
                                                {
                                                    //dynamic widgetSetting = JObject.Parse(mergedlst[i].WidgetSetting);
                                                    dynamic widgetSetting = JArray.Parse(mergedlst[i].WidgetSetting);
                                                    if (widgetSetting[0].Html.ToString().Length > 0)
                                                    {
                                                        html = widgetSetting[0].Html; //TODO: ***Deepak: Remove hard coded line
                                                    }
                                                }
                                                htmlString.Append(html);
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
                                            
                                            else if (mergedlst[i].WidgetName == HtmlConstants.SPECIAL_MESSAGE_WIDGET_NAME)
                                            {
                                                var widgetHtml = new StringBuilder(HtmlConstants.SPECIAL_MESSAGE_HTML);
                                                
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
                                                // }
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


        static void Main()
        {
            string dateString = "2022-09-23 00:00:00.000";

            // Parse the date string
            if (DateTime.TryParseExact(dateString, "yyyy-MM-dd HH:mm:ss.fff", null, System.Globalization.DateTimeStyles.None, out DateTime parsedDate))
            {
                // Get the month from the parsed date
                int month = parsedDate.Month;

                Console.WriteLine("Month: " + month);
            }
            else
            {
                Console.WriteLine("Invalid date format");
            }
        }

        #endregion
    }
}
