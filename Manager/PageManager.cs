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
                                                string paymentInfoJson = "{Reg_ID : 1,Start_Date : '2023-01-01',End_Date : '2023-01-01',Request_DateTime : 'DummyText1',ID : '124529534',Intermediary_Code : 'DummyText1',FSP_ID : 'DummyText1',Policy_Number : 'DummyText1',FSP_Party_ID : 'DummyText1',Client_Number : '124556686',FSP_REF : '2452953',Client_Name : 'Mr SCHOELER',Int_ID : 'DummyText1',Product_Type : 'DummyText1',Commission_Amount : 'DummyText1',INT_EXT_REF : '124411745',Int_Name : 'Kruger Van Heerden',Int_Type : 'DummyText1',Policy_Ref : '5596100',Member_Ref : '124556686',Member_Name : 'DummyText1',Transaction_Amount : 'DummyText1',Mem_Age : 'DummyText1',Months_In_Force : 'DummyText1',Commission_Type : 'Safe Custody Fee',Description : 'Safe Custody Service Fee',POSTED_DATE : '2023-03-03',AE_Type_ID : 'DummyText1',AE_Amount : 'DummyText1',DR_CR : 'DummyText1',NAME : 'DummyText1',Member_Surname : 'DummyText1',Jurisdiction : 'DummyText1',Sales_Office : 'DummyText1',FSP_Name : 'Miss Yvonne van Heerden',FSP_Trading_Name : 'T/A Yvonne Van Heerden Financial Planner CC',FSP_Ext_Ref : '124529534',FSP_Kind : 'DummyText1',  		FSP_VAT_Number : '2452953',Product : 'DummyText1',Prod_Group : 'Service Fee',Prod_Seq : 'DummyText1',Report_Seq : 'DummyText1',TYPE : 'DummyText1',Display_Amount : '17.55',VAT_Amount : '38001.27',Earning_Amount : '256670.66',Payment_Amount : 'DummyText1',START_DATE : 'DummyText1',END_DATE : 'DummyText1',Business_Type : 'DummyText1',Lifecycle_Description : 'DummyText1',Lifecycle_Start_Date : 'DummyText1',AE_Scheduler_ID : 'DummyText1',VAT_Amount_1 : 'DummyText1',Final_Amount : 'DummyText1'}";
                                                if (paymentInfoJson != string.Empty && validationEngine.IsValidJson(paymentInfoJson))
                                                {
                                                    spIAA_PaymentDetail paymentInfo = JsonConvert.DeserializeObject<spIAA_PaymentDetail>(paymentInfoJson);
                                                    var paymentHtmlWidget = HtmlConstants.PAYMENT_SUMMARY_WIDGET_HTML;
                                                    paymentHtmlWidget = paymentHtmlWidget.Replace("{{WidgetDivHeight}}", divHeight);
                                                    paymentHtmlWidget = paymentHtmlWidget.Replace("{{IntTotal}}", paymentInfo.Earning_Amount);
                                                    paymentHtmlWidget = paymentHtmlWidget.Replace("{{Vat}}", paymentInfo.VAT_Amount);
                                                    paymentHtmlWidget = paymentHtmlWidget.Replace("{{TotalDue}}", (Convert.ToDouble(paymentInfo.Earning_Amount) + Convert.ToDouble(paymentInfo.VAT_Amount)).ToString());
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
                                                string headingInfoJson = "{Reg_ID : 1,Start_Date : '2023-01-01',End_Date : '2023-01-01',Request_DateTime : 'DummyText1',ID : '124529534',Intermediary_Code : 'DummyText1',FSP_ID : 'DummyText1',Policy_Number : 'DummyText1',FSP_Party_ID : 'DummyText1',Client_Number : '124556686',FSP_REF : '2452953',Client_Name : 'Mr SCHOELER',Int_ID : 'DummyText1',Product_Type : 'DummyText1',Commission_Amount : 'DummyText1',INT_EXT_REF : '124411745',Int_Name : 'Kruger Van Heerden',Int_Type : 'DummyText1',Policy_Ref : '5596100',Member_Ref : '124556686',Member_Name : 'DummyText1',Transaction_Amount : 'DummyText1',Mem_Age : 'DummyText1',Months_In_Force : 'DummyText1',Commission_Type : 'Safe Custody Fee',Description : 'Safe Custody Service Fee',POSTED_DATE : '2023-03-03',AE_Type_ID : 'DummyText1',AE_Amount : 'DummyText1',DR_CR : 'DummyText1',NAME : 'DummyText1',Member_Surname : 'DummyText1',Jurisdiction : 'DummyText1',Sales_Office : 'DummyText1',FSP_Name : 'Miss Yvonne van Heerden',FSP_Trading_Name : 'T/A Yvonne Van Heerden Financial Planner CC',FSP_Ext_Ref : '124529534',FSP_Kind : 'DummyText1',  		FSP_VAT_Number : '2452953',Product : 'DummyText1',Prod_Group : 'Service Fee',Prod_Seq : 'DummyText1',Report_Seq : 'DummyText1',TYPE : 'DummyText1',Display_Amount : '17.55',VAT_Amount : '38001.27',Earning_Amount : '256670.66',Payment_Amount : 'DummyText1',START_DATE : 'DummyText1',END_DATE : 'DummyText1',Business_Type : 'DummyText1',Lifecycle_Description : 'DummyText1',Lifecycle_Start_Date : 'DummyText1',AE_Scheduler_ID : 'DummyText1',VAT_Amount_1 : 'DummyText1',Final_Amount : 'DummyText1'}";
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
                                                string ppsDetailsInfoJson = "{Reg_ID : 1,Start_Date : '2023-01-01',End_Date : '2023-01-01',Request_DateTime : 'DummyText1',ID : '124529534',Intermediary_Code : 'DummyText1',FSP_ID : 'DummyText1',Policy_Number : 'DummyText1',FSP_Party_ID : 'DummyText1',Client_Number : '124556686',FSP_REF : '2452953',Client_Name : 'Mr SCHOELER',Int_ID : 'DummyText1',Product_Type : 'DummyText1',Commission_Amount : 'DummyText1',INT_EXT_REF : '124411745',Int_Name : 'Kruger Van Heerden',Int_Type : 'DummyText1',Policy_Ref : '5596100',Member_Ref : '124556686',Member_Name : 'DummyText1',Transaction_Amount : 'DummyText1',Mem_Age : 'DummyText1',Months_In_Force : 'DummyText1',Commission_Type : 'Safe Custody Fee',Description : 'Safe Custody Service Fee',POSTED_DATE : '2023-03-03',AE_Type_ID : 'DummyText1',AE_Amount : 'DummyText1',DR_CR : 'DummyText1',NAME : 'DummyText1',Member_Surname : 'DummyText1',Jurisdiction : 'DummyText1',Sales_Office : 'DummyText1',FSP_Name : 'Miss Yvonne van Heerden',FSP_Trading_Name : 'T/A Yvonne Van Heerden Financial Planner CC',FSP_Ext_Ref : '124529534',FSP_Kind : 'DummyText1',  		FSP_VAT_Number : '2452953',Product : 'DummyText1',Prod_Group : 'Service Fee',Prod_Seq : 'DummyText1',Report_Seq : 'DummyText1',TYPE : 'DummyText1',Display_Amount : '17.55',VAT_Amount : '38001.27',Earning_Amount : '256670.66',Payment_Amount : 'DummyText1',START_DATE : 'DummyText1',END_DATE : 'DummyText1',Business_Type : 'DummyText1',Lifecycle_Description : 'DummyText1',Lifecycle_Start_Date : 'DummyText1',AE_Scheduler_ID : 'DummyText1',VAT_Amount_1 : 'DummyText1',Final_Amount : 'DummyText1'}";
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
                                                string ppsFooter1InfoJson = "{Reg_ID : 1,Start_Date : '2023-01-01',End_Date : '2023-01-01',Request_DateTime : 'DummyText1',ID : '124529534',Intermediary_Code : 'DummyText1',FSP_ID : 'DummyText1',Policy_Number : 'DummyText1',FSP_Party_ID : 'DummyText1',Client_Number : '124556686',FSP_REF : '2452953',Client_Name : 'Mr SCHOELER',Int_ID : 'DummyText1',Product_Type : 'DummyText1',Commission_Amount : 'DummyText1',INT_EXT_REF : '124411745',Int_Name : 'Kruger Van Heerden',Int_Type : 'DummyText1',Policy_Ref : '5596100',Member_Ref : '124556686',Member_Name : 'DummyText1',Transaction_Amount : 'DummyText1',Mem_Age : 'DummyText1',Months_In_Force : 'DummyText1',Commission_Type : 'Safe Custody Fee',Description : 'Safe Custody Service Fee',POSTED_DATE : '2023-03-03',AE_Type_ID : 'DummyText1',AE_Amount : 'DummyText1',DR_CR : 'DummyText1',NAME : 'DummyText1',Member_Surname : 'DummyText1',Jurisdiction : 'DummyText1',Sales_Office : 'DummyText1',FSP_Name : 'Miss Yvonne van Heerden',FSP_Trading_Name : 'T/A Yvonne Van Heerden Financial Planner CC',FSP_Ext_Ref : '124529534',FSP_Kind : 'DummyText1',FSP_VAT_Number : '2452953',Product : 'DummyText1',Prod_Group : 'Service Fee',Prod_Seq : 'DummyText1',Report_Seq : 'DummyText1',TYPE : 'DummyText1',Display_Amount : '17.55',VAT_Amount : '38001.27',Earning_Amount : '256670.66',Payment_Amount : 'DummyText1',START_DATE : 'DummyText1',END_DATE : 'DummyText1',Business_Type : 'DummyText1',Lifecycle_Description : 'DummyText1',Lifecycle_Start_Date : 'DummyText1',AE_Scheduler_ID : 'DummyText1',VAT_Amount_1 : 'DummyText1',Final_Amount : 'DummyText1'}";
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
                                                string productSummaryListJson = "[{ 'Commission_Type': 'Safe Custody Fee', 'Prod_Group':'Safe Custody Fee', 'Display_Amount': '52.65','QueryLink': 'https://facebook.com'},{ 'Commission_Type': 'Safe Custody Fee', 'Prod_Group':'Service Fee', 'Display_Amount': '52.66', 'QueryLink': 'https://facebook.com' }, { 'Commission_Type': 'Safe Custody Fee', 'Prod_Group':'Safe Custody Fee', 'Display_Amount': '52.67', 'QueryLink': 'https://facebook.com' }, { 'Commission_Type': 'Safe Custody Fee', 'Prod_Group':'Service Fee', 'Display_Amount': '52.68', 'QueryLink': 'https://facebook.com' } ]";

                                                if (productSummaryListJson != string.Empty && validationEngine.IsValidJson(productSummaryListJson))
                                                {
                                                    IList<spIAA_PaymentDetail> productSummary = JsonConvert.DeserializeObject<List<spIAA_PaymentDetail>>(productSummaryListJson);
                                                    StringBuilder productSummarySrc = new StringBuilder();
                                                    long index = 1;
                                                    productSummary.ToList().ForEach(item =>
                                                    {
                                                        productSummarySrc.Append("<tr><td align='center' valign='center' class='px-1 py-1 fsp-bdr-right fsp-bdr-bottom'>" + index + "</td><td class='fsp-bdr-right fsp-bdr-bottom px-1'>" + item.Commission_Type + "</td>" + "<td class='fsp-bdr-right fsp-bdr-bottom px-1'> " + (item.Prod_Group == "Service Fee" ? "Premium Under Advise Fee" : item.Prod_Group) + "</td> <td class='text-right fsp-bdr-right fsp-bdr-bottom px-1'>R" + item.Display_Amount + "</td><td class='text-center fsp-bdr-bottom px-1'><a  href ='https://facebook.com' target='_blank'><img class='leftarrowlogo' src ='assets/images/leftarrowlogo.png' alt = 'Left Arrow'></a></td></tr>");
                                                        index++;
                                                    });
                                                    string productSumstring = HtmlConstants.PRODUCT_SUMMARY_WIDGET_HTML.Replace("{{ProductSummary}}", productSummarySrc.ToString());
                                                    string productInfoJson = "{Earning_Amount : '256670.66',VAT_Amount : '38001.27'}";
                                                    spIAA_PaymentDetail productInfo = JsonConvert.DeserializeObject<spIAA_PaymentDetail>(productInfoJson);
                                                    productSumstring = productSumstring.Replace("{{WidgetDivHeight}}", divHeight);
                                                    productSumstring = productSumstring.Replace("{{QueryBtn}}", "assets/images/IfQueryBtn.jpg");
                                                    productSumstring = productSumstring.Replace("{{TotalDue}}", "R" + productInfo.Earning_Amount.ToString());
                                                    productSumstring = productSumstring.Replace("{{VATDue}}", "R" + productInfo.VAT_Amount.ToString());
                                                    double grandTotalDue = (Convert.ToDouble(productInfo.Earning_Amount) + Convert.ToDouble(productInfo.VAT_Amount));
                                                    productSumstring = productSumstring.Replace("{{GrandTotalDue}}", "R" + grandTotalDue.ToString());
                                                    double ppsPayment = grandTotalDue;
                                                    productSumstring = productSumstring.Replace("{{PPSPayment}}", "-R" + (grandTotalDue).ToString());
                                                    productSumstring = productSumstring.Replace("{{Balance}}", "R" + Convert.ToDouble((grandTotalDue - ppsPayment)).ToString("F2"));
                                                    htmlString.Append(productSumstring);
                                                }
                                            }

                                            else if (mergedlst[i].WidgetName == HtmlConstants.FOOTER_IMAGE_WIDGET_NAME)
                                            {
                                                string footerImageInfoJson = "{Reg_ID : 1,Start_Date : '2023-01-01',End_Date : '2023-01-01',Request_DateTime : 'DummyText1',ID : '124529534',Intermediary_Code : 'DummyText1',FSP_ID : 'DummyText1',Policy_Number : 'DummyText1',FSP_Party_ID : 'DummyText1',Client_Number : '124556686',FSP_REF : '2452953',Client_Name : 'Mr SCHOELER',Int_ID : 'DummyText1',Product_Type : 'DummyText1',Commission_Amount : 'DummyText1',INT_EXT_REF : '124411745',Int_Name : 'Kruger Van Heerden',Int_Type : 'DummyText1',Policy_Ref : '5596100',Member_Ref : '124556686',Member_Name : 'DummyText1',Transaction_Amount : 'DummyText1',Mem_Age : 'DummyText1',Months_In_Force : 'DummyText1',Commission_Type : 'Safe Custody Fee',Description : 'Safe Custody Service Fee',POSTED_DATE : '2023-03-03',AE_Type_ID : 'DummyText1',AE_Amount : 'DummyText1',DR_CR : 'DummyText1',NAME : 'DummyText1',Member_Surname : 'DummyText1',Jurisdiction : 'DummyText1',Sales_Office : 'DummyText1',FSP_Name : 'Miss Yvonne van Heerden',FSP_Trading_Name : 'T/A Yvonne Van Heerden Financial Planner CC',FSP_Ext_Ref : '124529534',FSP_Kind : 'DummyText1',  		FSP_VAT_Number : '2452953',Product : 'DummyText1',Prod_Group : 'Service Fee',Prod_Seq : 'DummyText1',Report_Seq : 'DummyText1',TYPE : 'DummyText1',Display_Amount : '17.55',VAT_Amount : '38001.27',Earning_Amount : '256670.66',Payment_Amount : 'DummyText1',START_DATE : 'DummyText1',END_DATE : 'DummyText1',Business_Type : 'DummyText1',Lifecycle_Description : 'DummyText1',Lifecycle_Start_Date : 'DummyText1',AE_Scheduler_ID : 'DummyText1',VAT_Amount_1 : 'DummyText1',Final_Amount : 'DummyText1'}";
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
                                                string transactionListJson = "[{'INT_EXT_REF':'2164250','Int_Name':'Mr SCHOELER','Client_Name':'Kruger Van Heerden','Member_Ref':124556686,'Policy_Ref':5596100,'Description':'Safe Custody Service Fee','Commission_Type':'Safe Custody Fee','POSTED_DATE':'20-Mar-23','Display_Amount':'17.55','Query_Link':'https://facebook.com','TYPE':'Fiduciary_Data','Prod_Group':'Safe Custody Fee'},{'INT_EXT_REF':'2164250','Int_Name':'Yvonne Van Heerden','Client_Name':'Mr SCHOELER','Member_Ref':124556686,'Policy_Ref':'5596100','Description':'Safe Custody Service Fee VAT','Commission_Type':'Safe Custody Fee','POSTED_DATE':'20-Mar-23','Display_Amount':'2.63','Query_Link':'https://facebook.com','TYPE':'Fiduciary_Data','Prod_Group':'Safe Custody Fee'},{'INT_EXT_REF':'124411745','Int_Name':'Kruger Van Heerden','Client_Name':'DR N J Olivier','Member_Ref':'1217181','Policy_Ref':'5524069','Description':'Safe Custody Service Fee','Commission_Type':'Safe Custody Fee','POSTED_DATE':'20-Mar-23','Display_Amount':'17.55','Query_Link':'https://facebook.com','TYPE':'Fiduciary_Data','Prod_Group':'Safe Custody Fee'},{'INT_EXT_REF':'124411745','Int_Name':'Kruger Van Heerden','Client_Name':'DR N J Olivier','Member_Ref':'124556686','Policy_Ref':'5596100','Description':'Safe Custody Service Fee VAT','Commission_Type':'Safe Custody Fee','POSTED_DATE':'20-Mar-23','Display_Amount':'2.63','Query_Link':'https://facebook.com','TYPE':'Fiduciary_Data','Prod_Group':'VAT'}]";
                                                double TotalPostedAmount = 0;
                                                if (transactionListJson != string.Empty && validationEngine.IsValidJson(transactionListJson))
                                                {
                                                    IList<spIAA_PaymentDetail> transaction = JsonConvert.DeserializeObject<List<spIAA_PaymentDetail>>(transactionListJson);
                                                    StringBuilder detailedTransactionSrc = new StringBuilder();
                                                    string detailedTransactionString = HtmlConstants.DETAILED_TRANSACTIONS_WIDGET_HTML;
                                                    var records = transaction.GroupBy(gptransactionitem => gptransactionitem.INT_EXT_REF).ToList();
                                                    records?.ForEach(transactionitem =>
                                                    {
                                                        detailedTransactionSrc.Append("<div class='px-50'><div class='prouct-table-block'><div class='text-left fsp-transaction-title font-weight-bold mb-3'>Intermediary:  " + transactionitem.FirstOrDefault().INT_EXT_REF + " " + transactionitem.FirstOrDefault().Int_Name + "</div><table width='100%' cellpadding='0' cellspacing='0'> <tr><th class='font-weight-bold text-white'>Client name</th> <th class='font-weight-bold text-white text-center pe-0 bdr-r-0'>Member<br /> number</th> <th class='font-weight-bold text-white text-center'>Will<br/> number</th> <th class='font-weight-bold text-white text-center'>Fiduciary fees</th> <th class='font-weight-bold text-white text-center'>Commission<br /> type</th> <th class='font-weight-bold text-white text-center'>Posted date</th> <th class='font-weight-bold text-white text-center'>Posted amount</th> <th class='font-weight-bold text-white'>Query</th> </tr> ");
                                                        detailedTransactionString = detailedTransactionString.Replace("{{QueryBtnImgLink}}", "https://www.google.com/");
                                                        detailedTransactionString = detailedTransactionString.Replace("{{QueryBtn}}", "assets/images/IfQueryBtn.jpg");


                                                        transaction.Where(witem => witem.INT_EXT_REF == transactionitem.FirstOrDefault().INT_EXT_REF).ToList().ForEach(item =>
                                                    {
                                                        detailedTransactionSrc.Append("<tr><td align = 'center' valign = 'center' class='px-1 py-1 fsp-bdr-right fsp-bdr-bottom'>" + item.Client_Name + "</td><td class= 'fsp-bdr-right fsp-bdr-bottom px-1'>" + item.Member_Ref + "</td><td class= 'fsp-bdr-right fsp-bdr-bottom px-1'> " + item.Policy_Ref + "</td><td class= 'text-right fsp-bdr-right fsp-bdr-bottom px-1'>" + (item.Description == "Commission Service Fee" ? "Premium Under Advise Fee" : item.Description) + "</td><td class= 'text-center fsp-bdr-right fsp-bdr-bottom px-1'>" + item.Commission_Type + "</td><td class= 'text-center fsp-bdr-right fsp-bdr-bottom px-1'>" + item.POSTED_DATE.ToString("dd-MMM-yyyy") + "</td><td class= 'text-center fsp-bdr-right fsp-bdr-bottom px-1'> R" + item.Display_Amount + "</td><td class= 'text-center fsp-bdr-bottom px-1'><a href ='https://www.google.com/' target ='_blank'><img class='leftarrowlogo' src='assets/images/leftarrowlogo.png' alt='Left Arrow'></a></td></tr>");
                                                        TotalPostedAmount += ((item.TYPE == "Fiduciary_Data") && (item.Prod_Group != "VAT")) ? (Convert.ToDouble(item.Display_Amount)) : 0.0;
                                                    });
                                                        string TotalPostedAmountR = (TotalPostedAmount == 0) ? "0.00" : ("R" + TotalPostedAmount.ToString());
                                                        detailedTransactionSrc.Append("<tr> <td align='center' valign='center' class='px-1 py-1 fsp-bdr-right fsp-bdr-bottom'></td> <td class='fsp-bdr-right fsp-bdr-bottom px-1 py-1'></td> <td class='fsp-bdr-right fsp-bdr-bottom px-1 py-1'></td> <td class='text-right fsp-bdr-right fsp-bdr-bottom px-1 py-1'></td> <td class='text-center fsp-bdr-right fsp-bdr-bottom px-1 py-1'><br /></td> <td class='text-center fsp-bdr-right fsp-bdr-bottom px-1 py-1'></td> <td class='text-center fsp-bdr-right fsp-bdr-bottom px-1 py-1'>" + TotalPostedAmountR + "</td> <td class='text-center fsp-bdr-bottom px-1'><a href='https://www.google.com/' target = '_blank' ><img src='assets/images/leftarrowlogo.png'></a></td> </tr></table><div class='text-right w-100 pt-3'><a href='https://www.google.com/' target = '_blank'></a></div></div></div></div>");
                                                        TotalPostedAmount = 0;

                                                    });
                                                    detailedTransactionString = detailedTransactionString.Replace("{{detailedTransaction}}", detailedTransactionSrc.ToString());
                                                    htmlString.Append(detailedTransactionString);
                                                }
                                            }
                                            else if (mergedlst[i].WidgetName == HtmlConstants.PPS_DETAILED_TRANSACTIONS_WIDGET_NAME)
                                            {
                                                string transactionListJson = "[{'INT_EXT_REF':'124565256 ','POLICY_REF':'3830102','MEMBER_REF':'10024365','Member_Name':'Dr JC Arthur','BUS_GROUP':'PPS INSURANCE','PRODUCT_DESCRIPTION':'Professional Health Provider Whole Life Professional Health',  'REQUESTED_DATETIME':'01-11-2022  00:00:00', 'REQUEST_DATETIME':'2022-09-23','TRANSACTION_AMOUNT':2265.4 ,'ALLOCATED_AMOUNT':-23107.08 ,'MEMBER_AGE':'45 ','MeasureType':'Commission','CommissionType':'2nd Year','FSP_Name':'Miss HW HLONGWANE'},     {'INT_EXT_REF':'124565256 ','POLICY_REF':'3830102','MEMBER_REF':'10024365','Member_Name':'Dr JC Arthur','BUS_GROUP':'PPS INSURANCE','PRODUCT_DESCRIPTION':'Professional Health Provider Whole Life Level Rated Professional Health CatchAll Exercised ',  'REQUESTED_DATETIME':'01-11-2022  00:00:00', 'REQUEST_DATETIME':'2022-09-23','TRANSACTION_AMOUNT':84.97 ,'ALLOCATED_AMOUNT':-866.69 ,'MEMBER_AGE':'45 ',  'MeasureType':'Commission','CommissionType':'2nd Year','FSP_Name':'Miss HW HLONGWANE'},     {'INT_EXT_REF':'124565256 ','POLICY_REF':'3830102','MEMBER_REF':'10024365','Member_Name':'Dr JC Arthur','BUS_GROUP':'PPS INSURANCE','PRODUCT_DESCRIPTION':'Professional Health Provider Whole Life Professional Health',  'REQUESTED_DATETIME':'01-11-2022  00:00:00', 'REQUEST_DATETIME':'2022-09-23','TRANSACTION_AMOUNT':2265.4,'ALLOCATED_AMOUNT':10968.98,'MEMBER_AGE':'45 ',  'MeasureType':'Commission','CommissionType':'2nd Year','FSP_Name':'Miss HW HLONGWANE'},     {'INT_EXT_REF':'124565256 ','POLICY_REF':'3820110 ','MEMBER_REF':'10436136 ','Member_Name':'Mnr JG Rossouw ','BUS_GROUP':'PPS INSURANCE','PRODUCT_DESCRIPTION':'Professional Health Provider Whole Life Professional Health',  'REQUESTED_DATETIME':'01-11-2022 00:00 ', 'REQUEST_DATETIME':'2022-09-28','TRANSACTION_AMOUNT':928.89 ,'ALLOCATED_AMOUNT':-9474.68 ,'MEMBER_AGE':'43',  'MeasureType':'Commission','CommissionType':'1nd Year','FSP_Name':'Miss HW HLONGWANE'},     {'INT_EXT_REF':'124565256 ','POLICY_REF':'3820110 ','MEMBER_REF':'10436136 ','Member_Name':'Mnr JG Rossouw ','BUS_GROUP':'PPS INSURANCE','PRODUCT_DESCRIPTION':'Professional Health Provider Whole Life Professional Health',  'REQUESTED_DATETIME':'01-11-2022 00:00 ', 'REQUEST_DATETIME':'2022-09-28','TRANSACTION_AMOUNT':928.89 ,'ALLOCATED_AMOUNT':6072.47 ,'MEMBER_AGE':'43',  'MeasureType':'Commission','CommissionType':'2nd Year','FSP_Name':'Miss HW HLONGWANE'}]  ";

                                                if (transactionListJson != string.Empty && validationEngine.IsValidJson(transactionListJson))
                                                {
                                                    IList<spIAA_Commission_Detail> ppsDetails = JsonConvert.DeserializeObject<List<spIAA_Commission_Detail>>(transactionListJson);
                                                    StringBuilder detailedTransactionSrc = new StringBuilder();
                                                    string detailedTransactionString = HtmlConstants.PPS_DETAILED_TRANSACTIONS_WIDGET_HTML;
                                                    detailedTransactionSrc.Append("<div class='pps-monthly-table w-100'><table cellpadding='0' cellspacing='0' width='100%'><tr><th class='bdr-right-white sky-blue-bg text-white font-weight-bold'>Client<br/>name</th><th class='bdr-right-white sky-blue-bg text-white font-weight-bold'>Age</th><th class='bdr-right-white sky-blue-bg text-white font-weight-bold'>Policy #</th><th class='bdr-right-white sky-blue-bg text-white font-weight-bold'>Policy #</th><th class='bdr-right-white sky-blue-bg text-white font-weight-bold'>Product</th><th class='bdr-right-white sky-blue-bg text-white font-weight-bold'>Date<br/>issued</th><th class='bdr-right-white sky-blue-bg text-white font-weight-bold'>Inception<br/>date</th><th class='bdr-right-white sky-blue-bg text-white font-weight-bold'>Com<br/>type</th><th class='bdr-right-white sky-blue-bg text-white font-weight-bold'>Quantity</th><th class='bdr-right-white sky-blue-bg text-white font-weight-bold'>Posted<br/>date</th><th class='bdr-right-white sky-blue-bg text-white font-weight-bold'>Earnings</th></tr>");
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
                                                                detailedTransactionSrc.Append("<tr><td class='bdr-right-white text-left'>" + memberitemrecord.Member_Name + "</td><td class='bdr-right-white text-left'>" + memberitemrecord.MEMBER_AGE + "</td>" + "<td class='bdr-right-white text-left'>" + memberitemrecord.POLICY_REF + "</td>" +  
                                      "<td class='bdr-right-white text-left'>" + memberitemrecord.POLICY_REF + "</td><td class='bdr-right-white text-left'>" + memberitemrecord.PRODUCT_DESCRIPTION + "</td><td class='bdr-right-white text-left'>" + memberitemrecord.REQUEST_DATETIME + "</td><td class='bdr-right-white text-left'>" + memberitemrecord.REQUESTED_DATETIME.ToString("dd-MMM-yyyy") + "</td><td class='bdr-right-white text-left'>" + memberitemrecord.CommissionType + "</td><td class='bdr-right-white text-left'>" + ((Convert.ToDouble(memberitemrecord.TRANSACTION_AMOUNT)) < 0 ? "-R" +
                                      (Convert.ToDouble(memberitemrecord.TRANSACTION_AMOUNT) * -1).ToString() : "R" + Convert.ToDouble(memberitemrecord.TRANSACTION_AMOUNT)).ToString() + "</td><td class='bdr-right-white text-right'>" + memberitemrecord.AE_Posted_Date.ToString("dd-MMM-yyyy") + "</td>" +
                                                                    "<td class='bdr-right-white ewidth text-right'>" + ((Convert.ToDouble(memberitemrecord.ALLOCATED_AMOUNT)) < 0? "-R" + 
                                      (Convert.ToDouble(memberitemrecord.ALLOCATED_AMOUNT)*-1).ToString() : "R" + Convert.ToDouble(memberitemrecord.ALLOCATED_AMOUNT)).ToString() + "</td></tr>");
                                                            });
                                                            string TotalPostedAmountR = (TotalPostedAmount == 0) ? "0.00" : (TotalPostedAmount.ToString());
                                                            detailedTransactionSrc.Append(" <tr><td class='dark-blue-bg text-white font-weight-bold '></td><td class='dark-blue-bg text-white font-weight-bold '></td><td class='dark-blue-bg text-white font-weight-bold '></td><td class='dark-blue-bg text-white font-weight-bold '></td><td class='dark-blue-bg text-white font-weight-bold tright fs-16'>Sub Total</td><td class='dark-blue-bg text-white font-weight-bold '></td><td class='dark-blue-bg text-white font-weight-bold '></td><td class='dark-blue-bg text-white font-weight-bold '></td><td class='dark-blue-bg text-white font-weight-bold '></td><td colspan='2' class='font-weight-bold text-right fs-16 pps-bg-gray' height='40'>" + ((Convert.ToDouble(TotalPostedAmountR)) < 0 ? "-R" +
                                      (Convert.ToDouble(TotalPostedAmountR) * -1).ToString() : "R" + Convert.ToDouble(TotalPostedAmountR)).ToString() + "</td></tr>");
                                                        });
                                                       
                                                    });
                                                    detailedTransactionSrc.Append("</table></div>");
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

                                                    // HTML generation for Monthly Production Summary Section
                                                    commisionDetailSrc.Append("<!-- Monthly Production Summary Section --><div class='earnings-section-monthly d-flex'><!-- Two Columns Layout --><div class='d-flex gap-1 w-100'><!-- Monthly Production Summary T1 --><div class='col-6'><!-- Heading for Monthly Production Summary T1 --><h4 class='monthly-production-summary skyblue-bg-title text-white text-center'>Monthly Production Summary</h4><div class='monthly-table'><!-- Table for Monthly Production Summary T1 --><table width='100%' cellpadding='0' cellspacing='0'><!-- Table Headers --><thead><tr><th class='text-white font-weight-bold text-left'>Month</th>");

                                                    // HTML generation for table headers based on CommissionType
                                                    gpCommisionType.ForEach(gpCommisionTypeitem =>
                                                    {
                                                        commisionDetailSrc.Append("<th style='height:50px' class='text-left'> Premium Under Advice(" + gpCommisionTypeitem.GroupKey.CommissionType + ")</th>");
                                                    });
                                                    commisionDetailSrc.Append("</tr></thead>");

                                                    // Iterate through grouped records to populate table rows
                                                    records.ForEach(gpMonthRangeItem =>
                                                    {
                                                        commisionDetailSrc.Append("<tr><td class='text-left'>" + GetMonthRange(gpMonthRangeItem.GroupKey.Month) + "</td>");
                                                        List<double> innermonthlyTotalListSum = new List<double>();

                                                        // Iterate through CommissionType groups to populate cells
                                                        gpCommisionType.ForEach(gpCommisionTypeitem =>
                                                        {
                                                            var premiumUnderAdviceTd1Sum = commisionDetail
                                                                .Where(witem => ((witem.REQUEST_DATETIME.Month == gpMonthRangeItem.GroupKey.Month) &&
                                                                                 (witem.CommissionType == gpCommisionTypeitem.GroupKey.CommissionType) &&
                                                                                 (witem.Type == "Policy_Data")))
                                                                .Sum(item => Convert.ToDouble(item.ALLOCATED_AMOUNT));

                                                            var premiumUnderAdviceTd1SumR = (premiumUnderAdviceTd1Sum == 0) ? "0.00" : ("R" + premiumUnderAdviceTd1Sum.ToString());
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
                                                    commisionDetailSrc.Append("<tr><td class='dark-blue-bg text-white font-weight-bold'>Total</td>");

                                                    for (int k = 0; k < premiumColumnSums.Count; k++)
                                                    {
                                                        totalMonthlyEarnings += premiumColumnSums[k];
                                                        commisionDetailSrc.Append("<td class='text-right font-weight-bold'>R" + premiumColumnSums[k] + "</ td>");
                                                    }

                                                    commisionDetailSrc.Append("</tr></table></div></div>");


                                                    // Fees Section: Monthly Fiduciary Fees Summary
                                                    List<List<double>> monthlyFessTotalList = new List<List<double>>();

                                                    // HTML generation for Monthly Fiduciary Fees Summary Section
                                                    commisionDetailSrc.Append("<!-- Monthly Production Summary T2 --><div class='col-6'><!-- Heading for Monthly Production Summary T2 --><h4 class='monthly-production-summary skyblue-bg-title text-white text-center'>Monthly Production Summary</h4><div class='monthly-table'><!-- Table for Monthly Production Summary T1 --><table width='100%' cellpadding='0' cellspacing='0'><!-- Table Headers --><thead><tr><th class='text-white font-weight-bold text-left'>Month</th>");

                                                    // Generate table headers for each CommissionType in the group
                                                    gpCommisionType.ForEach(gpCommisionTypeitem =>
                                                    {
                                                        commisionDetailSrc.Append("<th style='height:50px' class='text-left'> Fiduciary Fees(" + gpCommisionTypeitem.GroupKey.CommissionType + ")</th>");
                                                    });

                                                    commisionDetailSrc.Append("</tr></thead>");

                                                    // Iterate through grouped records to populate table rows for Fiduciary Fees
                                                    records.ForEach(gpFeesMonthRangeItem =>
                                                    {
                                                        commisionDetailSrc.Append("<tr><td class='text-left'>" + GetMonthRange(gpFeesMonthRangeItem.GroupKey.Month) + "</td>");
                                                        List<double> innerFeesMonthlyTotalListSum = new List<double>();

                                                        // Iterate through CommissionType groups to populate cells with Fiduciary Fees
                                                        gpCommisionType.ForEach(gpCommisionTypeitem =>
                                                        {
                                                            var premiumUnderAdviceTd1Sum = commisionDetail
                                                                .Where(witem => ((witem.REQUEST_DATETIME.Month == gpFeesMonthRangeItem.GroupKey.Month) &&
                                                                                 (witem.CommissionType == gpCommisionTypeitem.GroupKey.CommissionType) &&
                                                                                 (witem.Type == "Fiduciary_Data")))
                                                                .Sum(item => Convert.ToDouble(item.ALLOCATED_AMOUNT));

                                                            var premiumUnderAdviceTd1SumR = (premiumUnderAdviceTd1Sum == 0) ? "0.00" : ("R" + premiumUnderAdviceTd1Sum.ToString());
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
                                                    commisionDetailSrc.Append("<tr><td class='dark-blue-bg text-white font-weight-bold'>Total</td>");

                                                    for (int k = 0; k < FeesColumnSums.Count; k++)
                                                    {
                                                        totalMonthlyEarnings += FeesColumnSums[k];
                                                        commisionDetailSrc.Append("<td class='text-right font-weight-bold'>R" + FeesColumnSums[k] + "</ td>");
                                                    }

                                                    // Add total monthly earnings to the widget
                                                    var totalMonthlyEarningsR = (totalMonthlyEarnings == 0) ? "0.00" : ("R" + totalMonthlyEarnings.ToString());
                                                    commisionDetailSrc.Append("</tr></table></div></div>");
                                                    commisionDetailSrc.Append("</div></div><!-- Total Earning Section --><div class='total-earning mb-4'><div class='row'><div class='col-3 text-right'></div><div class='col-3 text-right'><div class='dark-blue-bg text-white text-right font-weight-bold pe-3 py-1'>Total Earnings</div></div><div class='col-3 text-left'><div class='total-price-title py-1 font-weight-bold text-center'>" + totalMonthlyEarningsR + "</div></div>");

                                                    // Fees Section: Monthly Fiduciary Fees Summary - End

                                                    // FSP Account Postings Summary start

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
                                                        commisionDetailSrc.Append("<!--FSPAccountPostingsSummarySection--><div class='earnings-section-monthly d-flex mb-2'><div class='d-flex gap-1 w-100'><!--FSP Account Postings Summary--><div class='col-6'><!--Headingfor FSP Account PostingsSummary--><h4 class='monthly-production-summary skyblue-bg-title text-white text-center'>FSP Account Postings Summary</h4><div class='monthly-table'><!--Table forFSPAccountPostingsSummary--><table width='100%' cellpadding='0' cellspacing='0'><!--TableHeaders--><thead><tr><th style='height:50px' class='text-white font-weight-bold'>PostedDate</th>");

                                                        // Appending HTML for Commission Type headers
                                                        gpCommisionType.ForEach(gpCommisionTypeitem =>
                                                        {
                                                            commisionDetailSrc.Append("<th style='height:50px' class='text-left'> Posted (" + gpCommisionTypeitem.GroupKey.CommissionType + ")</th>");
                                                        });

                                                        // Appending HTML for Posted Amount header
                                                        commisionDetailSrc.Append("<th style='height:50px' class='text-white font-weight-bold'>Posted Amount</th></tr></thead>");

                                                        // Appending HTML for Financial Service Provider row
                                                        commisionDetailSrc.Append("<tr><th class='text-left text-white font-weight-bold' colspan=" + (2 + gpCommisionType.Count) + ">Financial Service Provider: " + gpfspNameItem.GroupKey.FSP_Name + "</th><tr>");

                                                        // Iterating through AE_Posted_Date groups
                                                        aePostedRecords.ForEach(gpMonthRangeItem =>
                                                        {
                                                            // Appending HTML for Date and Commission Type columns
                                                            commisionDetailSrc.Append("<tr><td class='text-left'>" + DateTime.Parse(gpMonthRangeItem.GroupKey.Date.ToString()).ToString("dd-MMM-yyyy") + "</td>");
                                                            List<double> innermonthlyAEPostedTotalListSum = new List<double>();

                                                            // Iterating through Commission Types
                                                            gpCommisionType.ForEach(gpCommisionTypeitem =>
                                                            {
                                                                // Calculating and appending HTML for Posted Amount
                                                                var premiumUnderAdviceTd1Sum = commisionDetail
                                                                    .Where(witem => witem.AE_Posted_Date.Date == gpMonthRangeItem.GroupKey.Date && witem.CommissionType == gpCommisionTypeitem.GroupKey.CommissionType)
                                                                    .Sum(item => Convert.ToDouble(item.ALLOCATED_AMOUNT));

                                                                var premiumUnderAdviceTd1SumR = (premiumUnderAdviceTd1Sum == 0) ? "0.00" : ("R" + premiumUnderAdviceTd1Sum.ToString());
                                                                commisionDetailSrc.Append("<td class='text-right'>" + premiumUnderAdviceTd1SumR + "</td>");
                                                                innermonthlyAEPostedTotalListSum.Add(premiumUnderAdviceTd1Sum);
                                                            });

                                                            // Calculating and appending HTML for Total Posted Amount
                                                            var postedAmountTdSum = commisionDetail
                                                                .Where(witem => witem.AE_Posted_Date.Date == gpMonthRangeItem.GroupKey.Date)
                                                                .Sum(item => Convert.ToDouble(item.ALLOCATED_AMOUNT));

                                                            var postedAmountTdSumR = (postedAmountTdSum == 0) ? "0.00" : ("R" + postedAmountTdSum.ToString());
                                                            commisionDetailSrc.Append("<td class='text-right'>R" + postedAmountTdSumR + "</td>");

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

                                                        commisionDetailSrc.Append("<tr><td class='dark-blue-bg text-white font-weight-bold'>Total</td>");

                                                        for (int k = 0; k < aePostedColumnSums.Count; k++)
                                                        {
                                                            totalAEPostedMonthlyEarnings += aePostedColumnSums[k];
                                                            commisionDetailSrc.Append("<td class='text-right font-weight-bold'>R" + aePostedColumnSums[k] + "</ td>");
                                                        }

                                                        // Appending HTML for Total Posted Amount and closing the table
                                                        var totalAEPostedMonthlyEarningsR = (totalAEPostedMonthlyEarnings == 0) ? "0.00" : ("R" + totalAEPostedMonthlyEarnings.ToString());
                                                        commisionDetailSrc.Append("<td class='text-right font-weight-bold'>R" + totalAEPostedMonthlyEarningsR + "</ td>");
                                                        commisionDetailSrc.Append("</tr></table></div></div>");

                                                        // Closing FSP Account Postings Summary section
                                                    });

                                                    // FSP Account Postings Summary end

                                                    // Future Date Production start

                                                    // Grouping records by Due Date
                                                    var dueDateRecords = commisionDetail.GroupBy(gpDueDateitem => new { gpDueDateitem.DUE_DATE.Date })
                                                        .Select(group => new
                                                        {
                                                            GroupKey = group.Key,
                                                        }).ToList();

                                                    // Grouping records by Product Description
                                                    var prdocutDescriptionRecords = commisionDetail.GroupBy(gpDueDateitem => new { gpDueDateitem.PRODUCT_DESCRIPTION })
                                                        .Select(group => new
                                                        {
                                                            GroupKey = group.Key,
                                                        }).ToList();

                                                    // Initializing variables for monthly totals
                                                    List<List<double>> monthlyDueDateTotalList = new List<List<double>>();

                                                    // Appending HTML for the Future Dated Production section
                                                    commisionDetailSrc.Append("<!-- Future Dated Production Section --><div class='col-6'><!-- Heading for Future Dated Production --><h4 class='monthly-production-summary skyblue-bg-title text-white text-center'>Future Dated Production</h4><div class='monthly-table'><!-- Table for Future Dated Production --><table width='100%' cellpadding='0' cellspacing='0'><!-- Table Headers --><thead><tr><th class='text-left text-white font-weight-bold'>Due Date</th><th class='height:50px;text-left'>Fiduciary fees</th><th class='text-left'>Allocated Amount</th></tr></thead>");

                                                    // Initializing variables for column sums
                                                    double FutureColumnSums = 0.00;
                                                    double sumOfAllocatedAmount = 0.00;
                                                    double sumOfDueDateAllocatedAmount = 0.00;

                                                    // Iterating through Due Date groups
                                                    dueDateRecords.ForEach(gpDueDateItem =>
                                                    {
                                                        // Iterating through Product Description groups
                                                        prdocutDescriptionRecords.ForEach(prdocutDescription =>
                                                        {
                                                            // Appending HTML for Date, Product Description, and Allocated Amount columns
                                                            commisionDetailSrc.Append("<tr><td class='text-left'>" + DateTime.Parse(gpDueDateItem.GroupKey.Date.ToString()).ToString("dd-MMM-yyyy") + "</td>");
                                                            sumOfAllocatedAmount = commisionDetail
                                                                .Where(witem => witem.PRODUCT_DESCRIPTION == prdocutDescription.GroupKey.PRODUCT_DESCRIPTION && witem.DUE_DATE == gpDueDateItem.GroupKey.Date)
                                                                .Sum(item => Convert.ToDouble(item.ALLOCATED_AMOUNT));

                                                            commisionDetailSrc.Append("<td class='text-left'>" + (prdocutDescription.GroupKey.PRODUCT_DESCRIPTION == "Commission Service Fee" ? "Premium Under Advise Fee" : prdocutDescription.GroupKey.PRODUCT_DESCRIPTION) + "</td>");
                                                            var sumOfAllocatedAmountR = (sumOfAllocatedAmount == 0) ? "0.00" : ("R" + sumOfAllocatedAmount.ToString());
                                                            commisionDetailSrc.Append("<td class='text-right'>" + sumOfAllocatedAmountR + "</td>");
                                                            FutureColumnSums += sumOfAllocatedAmount;
                                                            commisionDetailSrc.Append("</tr>");
                                                        });

                                                        // Appending HTML for SubTotal row
                                                        var sumOfDueDateAllocatedAmountR = (sumOfDueDateAllocatedAmount == 0) ? "0.00" : ("R" + sumOfDueDateAllocatedAmount.ToString());
                                                        commisionDetailSrc.Append("<tr><td class='text-right' colspan='2'>SubTotal<td class='text-right'>" + sumOfDueDateAllocatedAmountR + "</td></tr>");
                                                    });

                                                    // Appending HTML for Total Earnings row and closing the table
                                                    commisionDetailSrc.Append("<td class='dark-blue-bg text-white font-weight-bold text-right' colspan='2'>Total Earnings</td><td class='text-right font-weight-bold'>R" + FutureColumnSums + "</td>");
                                                    commisionDetailSrc.Append("</tr></table></div></div>");
                                                    commisionDetailSrc.Append("</div></div>");
                                                    // Future Date Production end
                                                    // Replacing placeholder in the commisionDetailString with the specified divHeight value
                                                    commisionDetailString.Replace("{{WidgetDivHeight}}", divHeight);
                                                    // Replacing placeholder in the commisionDetailString with the generated commissionDetailSrc content
                                                    commisionDetailString = commisionDetailString.Replace("{{ppsEarningForPeriod}}", commisionDetailSrc.ToString());
                                                    // Appending the modified commisionDetailString to the htmlString
                                                    htmlString.Append(commisionDetailString);

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
                                            //else if (mergedlst[i].WidgetName == HtmlConstants.CUSTOMER_DETAILS_WIDGET_NAME)
                                            //{
                                            //    //IList<CustomerInformation> customers = this.customerRepository.GetCustomersByInvesterId(11083, tenantCode);
                                            //    var customerHtmlWidget = HtmlConstants.CUSTOMER_DETAILS_WIDGET_HTML;
                                            //    //CustomerInformation customerInfo = customers.FirstOrDefault();
                                            //    customerHtmlWidget = customerHtmlWidget.Replace("{{Title}}", "Title");
                                            //    customerHtmlWidget = customerHtmlWidget.Replace("{{FirstName}}", "FirstName");
                                            //    customerHtmlWidget = customerHtmlWidget.Replace("{{SurName}}", "SurName");
                                            //    customerHtmlWidget = customerHtmlWidget.Replace("{{CustAddressLine0}}", "Line0");
                                            //    customerHtmlWidget = customerHtmlWidget.Replace("{{CustAddressLine1}}", "Line1");
                                            //    customerHtmlWidget = customerHtmlWidget.Replace("{{CustAddressLine2}}", "Line2");
                                            //    customerHtmlWidget = customerHtmlWidget.Replace("{{CustAddressLine3}}", "Line3");
                                            //    customerHtmlWidget = customerHtmlWidget.Replace("{{CustAddressLine4}}", "Line4");
                                            //    customerHtmlWidget = customerHtmlWidget.Replace("{{MaskCellNo}}", "0123456789");
                                            //    //customerHtmlWidget = customerHtmlWidget.Replace("{{Barcode}}", customerInfo.Barcode != string.Empty ? customerInfo.Barcode : string.Empty);
                                            //    htmlString.Append(customerHtmlWidget);
                                            //}
                                            //else if (mergedlst[i].WidgetName == HtmlConstants.BRANCH_DETAILS_WIDGET_NAME)
                                            //{
                                            //    var htmlWidget = new StringBuilder(HtmlConstants.BRANCH_DETAILS_WIDGET_HTML);
                                            //    //if ((page.PageTypeName == HtmlConstants.HOME_LOAN_FOR_OTHER_SEGMENT_ENG_PAGE_TYPE || page.PageTypeName == HtmlConstants.HOME_LOAN_FOR_OTHER_SEGMENT_AFR_PAGE_TYPE
                                            //    //    || page.PageTypeName == HtmlConstants.HOME_LOAN_FOR_PML_SEGMENT_ENG_PAGE_TYPE || page.PageTypeName == HtmlConstants.HOME_LOAN_FOR_PML_SEGMENT_AFR_PAGE_TYPE
                                            //    //    || page.PageTypeName == HtmlConstants.HOME_LOAN_FOR_WEA_SEGMENT_AFR_PAGE_TYPE || page.PageTypeName == HtmlConstants.HOME_LOAN_FOR_WEA_SEGMENT_ENG_PAGE_TYPE))
                                            //    //{
                                            //    //    htmlWidget.Replace("{{BankName}}", DateTime.Now.ToString(ModelConstant.DATE_FORMAT_yyyy_MM_dd));
                                            //    //    htmlWidget.Replace("{{AddressLine0}}", string.Empty);
                                            //    //    htmlWidget.Replace("{{AddressLine1}}", string.Empty);
                                            //    //    htmlWidget.Replace("{{AddressLine2}}", string.Empty);
                                            //    //    htmlWidget.Replace("{{AddressLine3}}", string.Empty);
                                            //    //    htmlWidget.Replace("{{BankVATRegNo}}", string.Empty);
                                            //    //    htmlWidget.Replace("{{ContactCenter}}", "Professional Banking 24/7 Contact centre " + "0860 555 111");
                                            //    //    htmlString.Append(htmlWidget.ToString());
                                            //    //}
                                            //    //else
                                            //    //{
                                            //    //    string jsonstr = "{'BranchName': 'NEDBANK', 'AddressLine0':'Second Floor, Newtown Campus', 'AddressLine1':'141 Lilian Ngoyi Street, Newtown, Johannesburg 2001', 'AddressLine2':'PO Box 1144, Johannesburg, 2000','AddressLine3':'South Africa','VatRegNo':'4320116074','ContactNo':'0860 555 111'}";
                                            //    //    if (jsonstr != string.Empty && validationEngine.IsValidJson(jsonstr))
                                            //    //    {
                                            //    //        var branchDetails = JsonConvert.DeserializeObject<DM_BranchMaster>(jsonstr);
                                            //    //        htmlWidget.Replace("{{BankName}}", branchDetails.BranchName.ToUpper());
                                            //    //        htmlWidget.Replace("{{AddressLine0}}", branchDetails.AddressLine0.ToUpper());
                                            //    //        htmlWidget.Replace("{{AddressLine1}}", branchDetails.AddressLine1.ToUpper());
                                            //    //        htmlWidget.Replace("{{AddressLine2}}", branchDetails.AddressLine2.ToUpper());
                                            //    //        htmlWidget.Replace("{{AddressLine3}}", branchDetails.AddressLine3.ToUpper());
                                            //    //        htmlWidget.Replace("{{BankVATRegNo}}", "Bank VAT Reg No " + branchDetails.VatRegNo);
                                            //    //        htmlWidget.Replace("{{ContactCenter}}", "Nedbank Private Wealth Service Suite: " + branchDetails.ContactNo);
                                            //    //        htmlString.Append(htmlWidget.ToString());
                                            //    //    }
                                            //    //}

                                            //}
                                            //else if (mergedlst[i].WidgetName == HtmlConstants.WEALTH_BRANCH_DETAILS_WIDGET_NAME)
                                            //{
                                            //    var htmlWidget = new StringBuilder(HtmlConstants.WEALTH_BRANCH_DETAILS_WIDGET_HTML);
                                            //    //if ((page.PageTypeName == HtmlConstants.HOME_LOAN_FOR_OTHER_SEGMENT_ENG_PAGE_TYPE || page.PageTypeName == HtmlConstants.HOME_LOAN_FOR_OTHER_SEGMENT_AFR_PAGE_TYPE
                                            //    //    || page.PageTypeName == HtmlConstants.HOME_LOAN_FOR_PML_SEGMENT_ENG_PAGE_TYPE || page.PageTypeName == HtmlConstants.HOME_LOAN_FOR_PML_SEGMENT_AFR_PAGE_TYPE
                                            //    //    || page.PageTypeName == HtmlConstants.HOME_LOAN_FOR_WEA_SEGMENT_AFR_PAGE_TYPE || page.PageTypeName == HtmlConstants.HOME_LOAN_FOR_WEA_SEGMENT_ENG_PAGE_TYPE))
                                            //    //{
                                            //    //    htmlWidget.Replace("{{BranchDetails}}", DateTime.Now.ToString(ModelConstant.DATE_FORMAT_yyyy_MM_dd));
                                            //    //    htmlWidget.Replace("{{ContactCenter}}", "Professional Banking 24/7 Contact centre " + "0860 555 111");
                                            //    //    htmlString.Append(htmlWidget.ToString());
                                            //    //}
                                            //    //else
                                            //    //{
                                            //    //    string jsonstr = "{'BranchName': 'NEDBANK', 'AddressLine0':'Second Floor, Newtown Campus', 'AddressLine1':'141 Lilian Ngoyi Street, Newtown, Johannesburg 2001', 'AddressLine2':'PO Box 1144, Johannesburg, 2000','AddressLine3':'South Africa','VatRegNo':'4320116074','ContactNo':'0860 555 111'}";
                                            //    //    if (jsonstr != string.Empty && validationEngine.IsValidJson(jsonstr))
                                            //    //    {
                                            //    //        var branchDetails = JsonConvert.DeserializeObject<DM_BranchMaster>(jsonstr);
                                            //    //        htmlWidget.Replace("{{BankName}}", branchDetails.BranchName.ToUpper());
                                            //    //        htmlWidget.Replace("{{AddressLine0}}", branchDetails.AddressLine0.ToUpper());
                                            //    //        htmlWidget.Replace("{{AddressLine1}}", branchDetails.AddressLine1.ToUpper());
                                            //    //        htmlWidget.Replace("{{AddressLine2}}", branchDetails.AddressLine2.ToUpper());
                                            //    //        htmlWidget.Replace("{{AddressLine3}}", branchDetails.AddressLine3.ToUpper());
                                            //    //        htmlWidget.Replace("{{BankVATRegNo}}", "Bank VAT Reg No " + branchDetails.VatRegNo);
                                            //    //        htmlWidget.Replace("{{ContactCenter}}", "Nedbank Private Wealth Service Suite: " + branchDetails.ContactNo);
                                            //    //        htmlString.Append(htmlWidget.ToString());
                                            //    //    }
                                            //    //}

                                            //}
                                            //else if (mergedlst[i].WidgetName == HtmlConstants.INVESTMENT_PORTFOLIO_STATEMENT_WIDGET_NAME)
                                            //{
                                            //    string jsonstr = "{'Currency': 'R', 'TotalClosingBalance': '23 920.98', 'DayOfStatement':'21', 'InvestorId':'204626','StatementPeriod':'22/12/2020 to 21/01/2021','StatementDate':'21/01/2021', 'DsInvestorName' : '' }";
                                            //    string customerJsonstr = "{'TITLE_TEXT': 'MR', 'FIRST_NAME_TEXT':'MATHYS','SURNAME_TEXT':'SMIT','ADDR_LINE_0':'VAN DER MEULENSTRAAT 39','ADDR_LINE_1':'3971 EB DRIEBERGEN','ADDR_LINE_2':'NEDERLAND','ADDR_LINE_3':'9999','ADDR_LINE_4':'', 'MASK_CELL_NO': '******7786', 'FIRSTNAME': 'MATHYS', 'LASTNAME': 'SMIT', 'BARCODE': 'C:\\\\temp\\barcode.png'}";

                                            //    if (jsonstr != string.Empty && validationEngine.IsValidJson(jsonstr))
                                            //    {
                                            //        dynamic InvestmentPortfolio = JObject.Parse(jsonstr);
                                            //        var customerInfo = JsonConvert.DeserializeObject<CustomerInformation>(customerJsonstr);
                                            //        var InvestmentPortfolioHtmlWidget = HtmlConstants.INVESTMENT_PORTFOLIO_STATEMENT_WIDGET_HTML;
                                            //        InvestmentPortfolioHtmlWidget = InvestmentPortfolioHtmlWidget.Replace("{{DSName}}", Convert.ToString(InvestmentPortfolio.DsInvestorName));
                                            //        InvestmentPortfolioHtmlWidget = InvestmentPortfolioHtmlWidget.Replace("{{TotalClosingBalance}}", (Convert.ToString(InvestmentPortfolio.Currency) + Convert.ToString(InvestmentPortfolio.TotalClosingBalance)));
                                            //        InvestmentPortfolioHtmlWidget = InvestmentPortfolioHtmlWidget.Replace("{{DayOfStatement}}", Convert.ToString(InvestmentPortfolio.DayOfStatement));
                                            //        InvestmentPortfolioHtmlWidget = InvestmentPortfolioHtmlWidget.Replace("{{InvestorID}}", Convert.ToString(InvestmentPortfolio.InvestorId));
                                            //        InvestmentPortfolioHtmlWidget = InvestmentPortfolioHtmlWidget.Replace("{{StatementPeriod}}", Convert.ToString(InvestmentPortfolio.StatementPeriod));
                                            //        InvestmentPortfolioHtmlWidget = InvestmentPortfolioHtmlWidget.Replace("{{StatementDate}}", Convert.ToString(InvestmentPortfolio.StatementDate));
                                            //        InvestmentPortfolioHtmlWidget = InvestmentPortfolioHtmlWidget.Replace("{{FirstName}}", Convert.ToString(customerInfo.FirstName));
                                            //        InvestmentPortfolioHtmlWidget = InvestmentPortfolioHtmlWidget.Replace("{{SurName}}", Convert.ToString(customerInfo.SurName));
                                            //        htmlString.Append(InvestmentPortfolioHtmlWidget);
                                            //    }
                                            //}
                                            //else if (mergedlst[i].WidgetName == HtmlConstants.INVESTMENT_WEALTH_PORTFOLIO_STATEMENT_WIDGET_NAME)
                                            //{
                                            //    string jsonstr = "{'Currency': 'R', 'TotalClosingBalance': '23 920.98', 'DayOfStatement':'21', 'InvestorId':'204626','StatementPeriod':'22/12/2020 to 21/01/2021','StatementDate':'21/01/2021', 'DsInvestorName' : '' }";
                                            //    string customerJsonstr = "{'TITLE_TEXT': 'MR', 'FIRST_NAME_TEXT':'MATHYS','SURNAME_TEXT':'SMIT','ADDR_LINE_0':'VAN DER MEULENSTRAAT 39','ADDR_LINE_1':'3971 EB DRIEBERGEN','ADDR_LINE_2':'NEDERLAND','ADDR_LINE_3':'9999','ADDR_LINE_4':'', 'MASK_CELL_NO': '******7786', 'FIRSTNAME': 'MATHYS', 'LASTNAME': 'SMIT', 'BARCODE': 'C:\\\\temp\\barcode.png'}";

                                            //    if (jsonstr != string.Empty && validationEngine.IsValidJson(jsonstr))
                                            //    {
                                            //        dynamic InvestmentPortfolio = JObject.Parse(jsonstr);
                                            //        var customerInfo = JsonConvert.DeserializeObject<CustomerInformation>(customerJsonstr);
                                            //        var InvestmentPortfolioHtmlWidget = HtmlConstants.INVESTMENT_WEALTH_PORTFOLIO_STATEMENT_WIDGET_HTML;
                                            //        InvestmentPortfolioHtmlWidget = InvestmentPortfolioHtmlWidget.Replace("{{DSName}}", Convert.ToString(InvestmentPortfolio.DsInvestorName));
                                            //        InvestmentPortfolioHtmlWidget = InvestmentPortfolioHtmlWidget.Replace("{{TotalClosingBalance}}", (Convert.ToString(InvestmentPortfolio.Currency) + Convert.ToString(InvestmentPortfolio.TotalClosingBalance)));
                                            //        InvestmentPortfolioHtmlWidget = InvestmentPortfolioHtmlWidget.Replace("{{DayOfStatement}}", Convert.ToString(InvestmentPortfolio.DayOfStatement));
                                            //        InvestmentPortfolioHtmlWidget = InvestmentPortfolioHtmlWidget.Replace("{{InvestorID}}", Convert.ToString(InvestmentPortfolio.InvestorId));
                                            //        InvestmentPortfolioHtmlWidget = InvestmentPortfolioHtmlWidget.Replace("{{StatementPeriod}}", Convert.ToString(InvestmentPortfolio.StatementPeriod));
                                            //        InvestmentPortfolioHtmlWidget = InvestmentPortfolioHtmlWidget.Replace("{{StatementDate}}", Convert.ToString(InvestmentPortfolio.StatementDate));
                                            //        InvestmentPortfolioHtmlWidget = InvestmentPortfolioHtmlWidget.Replace("{{FirstName}}", Convert.ToString(customerInfo.FirstName));
                                            //        InvestmentPortfolioHtmlWidget = InvestmentPortfolioHtmlWidget.Replace("{{SurName}}", Convert.ToString(customerInfo.SurName));
                                            //        htmlString.Append(InvestmentPortfolioHtmlWidget);
                                            //    }
                                            //}
                                            //else if (mergedlst[i].WidgetName == HtmlConstants.INVESTOR_PERFORMANCE_WIDGET_NAME)
                                            //{
                                            //    string jsonstr = "{'Currency': 'R', 'ProductType': 'Notice deposits', 'OpeningBalanceAmount':'23 875.36', 'ClosingBalanceAmount':'23 920.98'}";
                                            //    if (jsonstr != string.Empty && validationEngine.IsValidJson(jsonstr))
                                            //    {
                                            //        dynamic InvestmentPerformance = JObject.Parse(jsonstr);
                                            //        var InvestorPerformanceHtmlWidget = HtmlConstants.INVESTOR_PERFORMANCE_WIDGET_HTML;
                                            //        InvestorPerformanceHtmlWidget = InvestorPerformanceHtmlWidget.Replace("{{ProductType}}", Convert.ToString(InvestmentPerformance.ProductType));
                                            //        InvestorPerformanceHtmlWidget = InvestorPerformanceHtmlWidget.Replace("{{OpeningBalanceAmount}}", (Convert.ToString(InvestmentPerformance.Currency) + Convert.ToString(InvestmentPerformance.OpeningBalanceAmount)));
                                            //        InvestorPerformanceHtmlWidget = InvestorPerformanceHtmlWidget.Replace("{{ClosingBalanceAmount}}", (Convert.ToString(InvestmentPerformance.Currency) + Convert.ToString(InvestmentPerformance.ClosingBalanceAmount)));
                                            //        htmlString.Append(InvestorPerformanceHtmlWidget);
                                            //    }
                                            //}
                                            //else if (mergedlst[i].WidgetName == HtmlConstants.WEALTH_INVESTOR_PERFORMANCE_WIDGET_NAME)
                                            //{
                                            //    string jsonstr = "{'Currency': 'R', 'ProductType': 'Notice deposits', 'OpeningBalanceAmount':'23 875.36', 'ClosingBalanceAmount':'23 920.98'}";
                                            //    if (jsonstr != string.Empty && validationEngine.IsValidJson(jsonstr))
                                            //    {
                                            //        dynamic InvestmentPerformance = JObject.Parse(jsonstr);
                                            //        var InvestorPerformanceHtmlWidget = HtmlConstants.WEALTH_INVESTOR_PERFORMANCE_WIDGET_HTML;
                                            //        InvestorPerformanceHtmlWidget = InvestorPerformanceHtmlWidget.Replace("{{ProductType}}", Convert.ToString(InvestmentPerformance.ProductType));
                                            //        InvestorPerformanceHtmlWidget = InvestorPerformanceHtmlWidget.Replace("{{OpeningBalanceAmount}}", (Convert.ToString(InvestmentPerformance.Currency) + Convert.ToString(InvestmentPerformance.OpeningBalanceAmount)));
                                            //        InvestorPerformanceHtmlWidget = InvestorPerformanceHtmlWidget.Replace("{{ClosingBalanceAmount}}", (Convert.ToString(InvestmentPerformance.Currency) + Convert.ToString(InvestmentPerformance.ClosingBalanceAmount)));
                                            //        htmlString.Append(InvestorPerformanceHtmlWidget);
                                            //    }
                                            //}
                                            //else if (mergedlst[i].WidgetName == HtmlConstants.BREAKDOWN_OF_INVESTMENT_ACCOUNTS_WIDGET_NAME)
                                            //{
                                            //    string jsonstr = HtmlConstants.BREAKDOWN_OF_INVESTMENT_ACCOUNTS_WIDGET_PREVIEW_JSON_STRING;

                                            //    if (jsonstr != string.Empty && validationEngine.IsValidJson(jsonstr))
                                            //    {
                                            //        var InvestmentAccountBreakdownHtml = new StringBuilder(HtmlConstants.BREAKDOWN_OF_INVESTMENT_ACCOUNTS_WIDGET_HTML);
                                            //        IList<InvestmentAccount> InvestmentAccounts = JsonConvert.DeserializeObject<List<InvestmentAccount>>(jsonstr);

                                            //        //Create Nav tab if investment accounts is more than 1
                                            //        var NavTabs = new StringBuilder();
                                            //        var InvestmentAccountsCount = InvestmentAccounts.Count;
                                            //        InvestmentAccountBreakdownHtml.Replace("{{NavTab}}", NavTabs.ToString());

                                            //        //create tab-content div if accounts is greater than 1, otherwise create simple div
                                            //        var TabContentHtml = new StringBuilder();
                                            //        var counter = 0;
                                            //        TabContentHtml.Append((InvestmentAccountsCount> 1) ? "<div class='tab-content'>" : string.Empty);
                                            //        InvestmentAccounts.ToList().ForEach(acc =>
                                            //        {
                                            //            var InvestmentAccountDetailHtml = new StringBuilder(HtmlConstants.INVESTMENT_ACCOUNT_DETAILS_HTML);
                                            //            InvestmentAccountDetailHtml.Replace("{{ProductDesc}}", acc.ProductDesc);
                                            //            InvestmentAccountDetailHtml.Replace("{{InvestmentId}}", acc.InvestmentId);
                                            //            InvestmentAccountDetailHtml.Replace("{{TabPaneClass}}", string.Empty);

                                            //            var InvestmentNo = acc.InvestorId + " " + acc.InvestmentId;
                                            //            //actual length is 12, due to space in between investor id and investment id we comparing for 13 characters
                                            //            while (InvestmentNo.Length != 13)
                                            //            {
                                            //                InvestmentNo = "0" + InvestmentNo;
                                            //            }
                                            //            InvestmentAccountDetailHtml.Replace("{{InvestmentNo}}", InvestmentNo);
                                            //            InvestmentAccountDetailHtml.Replace("{{AccountOpenDate}}", acc.OpenDate);

                                            //            InvestmentAccountDetailHtml.Replace("{{AccountOpenDate}}", acc.OpenDate);
                                            //            InvestmentAccountDetailHtml.Replace("{{InterestRate}}", acc.CurrentInterestRate + "% pa");
                                            //            InvestmentAccountDetailHtml.Replace("{{MaturityDate}}", acc.ExpiryDate);
                                            //            InvestmentAccountDetailHtml.Replace("{{InterestDisposal}}", acc.InterestDisposalDesc);
                                            //            InvestmentAccountDetailHtml.Replace("{{NoticePeriod}}", acc.NoticePeriod);
                                            //            InvestmentAccountDetailHtml.Replace("{{InterestDue}}", acc.Currency + acc.AccuredInterest);

                                            //            InvestmentAccountDetailHtml.Replace("{{LastTransactionDate}}", "25 November 2020");
                                            //            InvestmentAccountDetailHtml.Replace("{{BalanceOfLastTransactionDate}}", acc.Currency + (counter == 0 ? "5 307.14" : "18 613.84"));

                                            //            var InvestmentTransactionRows = new StringBuilder();
                                            //            acc.Transactions.ForEach(trans =>
                                            //            {
                                            //                var tr = new StringBuilder();
                                            //                tr.Append("<tr class='ht-20'>");
                                            //                tr.Append("<td class='w-15 pt-1'>" + trans.TransactionDate + "</td>");
                                            //                tr.Append("<td class='w-40 pt-1'>" + trans.TransactionDesc + "</td>");
                                            //                tr.Append("<td class='w-15 text-right pt-1'>" + (trans.Debit == "0" ? "-" : acc.Currency + trans.Debit) + "</td>");
                                            //                tr.Append("<td class='w-15 text-right pt-1'>" + (trans.Credit == "0" ? "-" : acc.Currency + trans.Credit) + "</td>");
                                            //                tr.Append("<td class='w-15 text-right pt-1'>" + (trans.Balance == "0" ? "-" : acc.Currency + trans.Balance) + "</td>");
                                            //                tr.Append("</tr>");
                                            //                InvestmentTransactionRows.Append(tr.ToString());
                                            //            });
                                            //            InvestmentAccountDetailHtml.Replace("{{InvestmentTransactionRows}}", InvestmentTransactionRows.ToString());
                                            //            TabContentHtml.Append(InvestmentAccountDetailHtml.ToString());
                                            //            counter++;
                                            //        });
                                            //        TabContentHtml.Append((InvestmentAccountsCount> 1) ? HtmlConstants.END_DIV_TAG : string.Empty);

                                            //        InvestmentAccountBreakdownHtml.Replace("{{TabContentsDiv}}", TabContentHtml.ToString());
                                            //        htmlString.Append(InvestmentAccountBreakdownHtml.ToString());
                                            //    }
                                            //}
                                            //else if (mergedlst[i].WidgetName == HtmlConstants.WEALTH_BREAKDOWN_OF_INVESTMENT_ACCOUNTS_WIDGET_NAME)
                                            //{
                                            //    string jsonstr = HtmlConstants.WEALTH_BREAKDOWN_OF_INVESTMENT_ACCOUNTS_WIDGET_PREVIEW_JSON_STRING;

                                            //    if (jsonstr != string.Empty && validationEngine.IsValidJson(jsonstr))
                                            //    {
                                            //        var InvestmentAccountBreakdownHtml = new StringBuilder(HtmlConstants.WEALTH_BREAKDOWN_OF_INVESTMENT_ACCOUNTS_WIDGET_HTML);
                                            //        IList<InvestmentAccount> InvestmentAccounts = JsonConvert.DeserializeObject<List<InvestmentAccount>>(jsonstr);

                                            //        //Create Nav tab if investment accounts is more than 1
                                            //        var NavTabs = new StringBuilder();
                                            //        var InvestmentAccountsCount = InvestmentAccounts.Count;
                                            //        InvestmentAccountBreakdownHtml.Replace("{{NavTab}}", NavTabs.ToString());

                                            //        //create tab-content div if accounts is greater than 1, otherwise create simple div
                                            //        var TabContentHtml = new StringBuilder();
                                            //        var counter = 0;
                                            //        TabContentHtml.Append((InvestmentAccountsCount> 1) ? "<div class='tab-content'>" : string.Empty);
                                            //        InvestmentAccounts.ToList().ForEach(acc =>
                                            //        {
                                            //            var InvestmentAccountDetailHtml = new StringBuilder(HtmlConstants.WEALTH_INVESTMENT_ACCOUNT_DETAILS_HTML);
                                            //            InvestmentAccountDetailHtml.Replace("{{ProductDesc}}", acc.ProductDesc);
                                            //            InvestmentAccountDetailHtml.Replace("{{InvestmentId}}", acc.InvestmentId);
                                            //            InvestmentAccountDetailHtml.Replace("{{TabPaneClass}}", string.Empty);

                                            //            var InvestmentNo = acc.InvestorId + " " + acc.InvestmentId;
                                            //            //actual length is 12, due to space in between investor id and investment id we comparing for 13 characters
                                            //            while (InvestmentNo.Length != 13)
                                            //            {
                                            //                InvestmentNo = "0" + InvestmentNo;
                                            //            }
                                            //            InvestmentAccountDetailHtml.Replace("{{InvestmentNo}}", InvestmentNo);
                                            //            InvestmentAccountDetailHtml.Replace("{{AccountOpenDate}}", acc.OpenDate);

                                            //            InvestmentAccountDetailHtml.Replace("{{AccountOpenDate}}", acc.OpenDate);
                                            //            InvestmentAccountDetailHtml.Replace("{{InterestRate}}", acc.CurrentInterestRate + "% pa");
                                            //            InvestmentAccountDetailHtml.Replace("{{MaturityDate}}", acc.ExpiryDate);
                                            //            InvestmentAccountDetailHtml.Replace("{{InterestDisposal}}", acc.InterestDisposalDesc);
                                            //            InvestmentAccountDetailHtml.Replace("{{NoticePeriod}}", acc.NoticePeriod);
                                            //            InvestmentAccountDetailHtml.Replace("{{InterestDue}}", acc.Currency + acc.AccuredInterest);

                                            //            InvestmentAccountDetailHtml.Replace("{{LastTransactionDate}}", "25 November 2020");
                                            //            InvestmentAccountDetailHtml.Replace("{{BalanceOfLastTransactionDate}}", acc.Currency + (counter == 0 ? "5 307.14" : "18 613.84"));

                                            //            var InvestmentTransactionRows = new StringBuilder();
                                            //            acc.Transactions.ForEach(trans =>
                                            //            {
                                            //                var tr = new StringBuilder();
                                            //                tr.Append("<tr class='ht-20'>");
                                            //                tr.Append("<td class='w-15 pt-1'>" + trans.TransactionDate + "</td>");
                                            //                tr.Append("<td class='w-40 pt-1'>" + trans.TransactionDesc + "</td>");
                                            //                tr.Append("<td class='w-15 text-right pt-1'>" + (trans.Debit == "0" ? "-" : acc.Currency + trans.Debit) + "</td>");
                                            //                tr.Append("<td class='w-15 text-right pt-1'>" + (trans.Credit == "0" ? "-" : acc.Currency + trans.Credit) + "</td>");
                                            //                tr.Append("<td class='w-15 text-right pt-1'>" + (trans.Balance == "0" ? "-" : acc.Currency + trans.Balance) + "</td>");
                                            //                tr.Append("</tr>");
                                            //                InvestmentTransactionRows.Append(tr.ToString());
                                            //            });
                                            //            InvestmentAccountDetailHtml.Replace("{{InvestmentTransactionRows}}", InvestmentTransactionRows.ToString());
                                            //            TabContentHtml.Append(InvestmentAccountDetailHtml.ToString());
                                            //            counter++;
                                            //        });
                                            //        TabContentHtml.Append((InvestmentAccountsCount> 1) ? HtmlConstants.END_DIV_TAG : string.Empty);

                                            //        InvestmentAccountBreakdownHtml.Replace("{{TabContentsDiv}}", TabContentHtml.ToString());
                                            //        htmlString.Append(InvestmentAccountBreakdownHtml.ToString());
                                            //    }
                                            //}
                                            //else if (mergedlst[i].WidgetName == HtmlConstants.EXPLANATORY_NOTES_WIDGET_NAME)
                                            //{
                                            //    string jsonstr = "{'Note1': 'Fixed deposits — Total balance of all your fixed-type accounts.', 'Note2': 'Notice deposits — Total balance of all your notice deposit accounts.', 'Note3':'Linked deposits — Total balance of all your linked-type accounts.'}";
                                            //    if (jsonstr != string.Empty && validationEngine.IsValidJson(jsonstr))
                                            //    {
                                            //        dynamic noteObj = JObject.Parse(jsonstr);
                                            //        var NotesHtmlWidget = HtmlConstants.EXPLANATORY_NOTES_WIDGET_HTML;
                                            //        var notes = new StringBuilder();
                                            //        notes.Append("<span> " + Convert.ToString(noteObj.Note1) + " </span> <br/>");
                                            //        notes.Append("<span> " + Convert.ToString(noteObj.Note2) + " </span> <br/>");
                                            //        notes.Append("<span> " + Convert.ToString(noteObj.Note3) + " </span> ");
                                            //        NotesHtmlWidget = NotesHtmlWidget.Replace("{{Notes}}", notes.ToString());
                                            //        htmlString.Append(NotesHtmlWidget);
                                            //    }
                                            //}
                                            //else if (mergedlst[i].WidgetName == HtmlConstants.WEALTH_EXPLANATORY_NOTES_WIDGET_NAME)
                                            //{
                                            //    string jsonstr = "{'Note1': 'Fixed deposits — Total balance of all your fixed-type accounts.', 'Note2': 'Notice deposits — Total balance of all your notice deposit accounts.', 'Note3':'Linked deposits — Total balance of all your linked-type accounts.'}";
                                            //    if (jsonstr != string.Empty && validationEngine.IsValidJson(jsonstr))
                                            //    {
                                            //        dynamic noteObj = JObject.Parse(jsonstr);
                                            //        var NotesHtmlWidget = HtmlConstants.WEALTH_EXPLANATORY_NOTES_WIDGET_HTML;
                                            //        var notes = new StringBuilder();
                                            //        notes.Append("<span> " + Convert.ToString(noteObj.Note1) + " </span> <br/>");
                                            //        notes.Append("<span> " + Convert.ToString(noteObj.Note2) + " </span> <br/>");
                                            //        notes.Append("<span> " + Convert.ToString(noteObj.Note3) + " </span> ");
                                            //        NotesHtmlWidget = NotesHtmlWidget.Replace("{{Notes}}", notes.ToString());
                                            //        htmlString.Append(NotesHtmlWidget);
                                            //    }
                                            //}
                                            //else if (mergedlst[i].WidgetName == HtmlConstants.SERVICE_WIDGET_NAME)
                                            //{
                                            //    if (MarketingMessages != string.Empty && validationEngine.IsValidJson(MarketingMessages))
                                            //    {
                                            //        IList<MarketingMessage> _lstMarketingMessage = JsonConvert.DeserializeObject<List<MarketingMessage>>(MarketingMessages);
                                            //        var ServiceMessage = _lstMarketingMessage[MarketingMessageCounter];
                                            //        if (ServiceMessage != null)
                                            //        {
                                            //            var messageTxt = ((!string.IsNullOrEmpty(ServiceMessage.MarketingMessageText1)) ? "<p>" + ServiceMessage.MarketingMessageText1 + "</p>" : string.Empty) + ((!string.IsNullOrEmpty(ServiceMessage.MarketingMessageText2)) ? "<p>" + ServiceMessage.MarketingMessageText2 + "</p>" : string.Empty) + ((!string.IsNullOrEmpty(ServiceMessage.MarketingMessageText3)) ? "<p>" + ServiceMessage.MarketingMessageText3 + "</p>" : string.Empty) + ((!string.IsNullOrEmpty(ServiceMessage.MarketingMessageText4)) ? "<p>" + ServiceMessage.MarketingMessageText4 + "</p>" : string.Empty) + ((!string.IsNullOrEmpty(ServiceMessage.MarketingMessageText5)) ? "<p>" + ServiceMessage.MarketingMessageText5 + "</p>" : string.Empty);

                                            //            var htmlWidget = HtmlConstants.SERVICE_WIDGET_HTML;
                                            //            htmlWidget = htmlWidget.Replace("{{ServiceMessageHeader}}", ServiceMessage.MarketingMessageHeader);
                                            //            htmlWidget = htmlWidget.Replace("{{ServiceMessageText}}", messageTxt);
                                            //            htmlString.Append(htmlWidget);
                                            //        }
                                            //    }
                                            //    MarketingMessageCounter++;
                                            //}
                                            //else if (mergedlst[i].WidgetName == HtmlConstants.WEALTH_SERVICE_WIDGET_NAME)
                                            //{
                                            //    if (MarketingMessages != string.Empty && validationEngine.IsValidJson(MarketingMessages))
                                            //    {
                                            //        IList<MarketingMessage> _lstMarketingMessage = JsonConvert.DeserializeObject<List<MarketingMessage>>(MarketingMessages);
                                            //        var ServiceMessage = _lstMarketingMessage[MarketingMessageCounter];
                                            //        if (ServiceMessage != null)
                                            //        {
                                            //            var messageTxt = ((!string.IsNullOrEmpty(ServiceMessage.MarketingMessageText1)) ? "<p>" + ServiceMessage.MarketingMessageText1 + "</p>" : string.Empty) + ((!string.IsNullOrEmpty(ServiceMessage.MarketingMessageText2)) ? "<p>" + ServiceMessage.MarketingMessageText2 + "</p>" : string.Empty) + ((!string.IsNullOrEmpty(ServiceMessage.MarketingMessageText3)) ? "<p>" + ServiceMessage.MarketingMessageText3 + "</p>" : string.Empty) + ((!string.IsNullOrEmpty(ServiceMessage.MarketingMessageText4)) ? "<p>" + ServiceMessage.MarketingMessageText4 + "</p>" : string.Empty) + ((!string.IsNullOrEmpty(ServiceMessage.MarketingMessageText5)) ? "<p>" + ServiceMessage.MarketingMessageText5 + "</p>" : string.Empty);

                                            //            var htmlWidget = HtmlConstants.WEALTH_SERVICE_WIDGET_HTML;
                                            //            htmlWidget = htmlWidget.Replace("{{ServiceMessageHeader}}", ServiceMessage.MarketingMessageHeader);
                                            //            htmlWidget = htmlWidget.Replace("{{ServiceMessageText}}", messageTxt);
                                            //            htmlString.Append(htmlWidget);
                                            //        }
                                            //    }
                                            //    MarketingMessageCounter++;
                                            //}
                                            //else if (mergedlst[i].WidgetName == HtmlConstants.PERSONAL_LOAN_DETAIL_WIDGET_NAME)
                                            //{
                                            //    string jsonstr = "{'Identifier': 1,'CustomerId': 211135146504,'InvestorId': 8004334234001,'Currency': 'R','ProductType': 'Personal Loan','BranchId': 1,'CreditAdvance': '71258','OutstandingBalance': '68169','AmountDue': '3297','ToDate': '2021-02-28 00:00:00','FromDate': '2020-12-01 00:00:00', 'MonthlyInstallment': '3297','DueDate': '2021-03-31 00:00:00','Arrears': '0','AnnualRate': '24','Term': '36'}";
                                            //    if (jsonstr != string.Empty && validationEngine.IsValidJson(jsonstr))
                                            //    {
                                            //        var PersonalLoan = JsonConvert.DeserializeObject<DM_PersonalLoanMaster>(jsonstr);
                                            //        var widgetHtml = new StringBuilder(HtmlConstants.PERSONAL_LOAN_DETAIL_HTML);

                                            //        var res = 0.0m;
                                            //        if (decimal.TryParse(PersonalLoan.CreditAdvance, out res))
                                            //        {
                                            //            widgetHtml.Replace("{{TotalLoanAmount}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                                            //        }
                                            //        else
                                            //        {
                                            //            widgetHtml.Replace("{{TotalLoanAmount}}", "R0.00");
                                            //        }

                                            //        res = 0.0m;
                                            //        if (decimal.TryParse(PersonalLoan.OutstandingBalance, out res))
                                            //        {
                                            //            widgetHtml.Replace("{{OutstandingBalance}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                                            //        }
                                            //        else
                                            //        {
                                            //            widgetHtml.Replace("{{OutstandingBalance}}", "R0.00");
                                            //        }

                                            //        res = 0.0m;
                                            //        if (decimal.TryParse(PersonalLoan.AmountDue, out res))
                                            //        {
                                            //            widgetHtml.Replace("{{DueAmount}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                                            //        }
                                            //        else
                                            //        {
                                            //            widgetHtml.Replace("{{DueAmount}}", "R0.00");
                                            //        }

                                            //        widgetHtml.Replace("{{AccountNumber}}", PersonalLoan.InvestorId.ToString());
                                            //        widgetHtml.Replace("{{StatementDate}}", PersonalLoan.ToDate.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy));
                                            //        widgetHtml.Replace("{{StatementPeriod}}", PersonalLoan.FromDate.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy) + " - " + PersonalLoan.ToDate.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy));

                                            //        res = 0.0m;
                                            //        if (decimal.TryParse(PersonalLoan.Arrears, out res))
                                            //        {
                                            //            widgetHtml.Replace("{{ArrearsAmount}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                                            //        }
                                            //        else
                                            //        {
                                            //            widgetHtml.Replace("{{ArrearsAmount}}", "R0.00");
                                            //        }

                                            //        widgetHtml.Replace("{{AnnualRate}}", PersonalLoan.AnnualRate + "% pa");

                                            //        res = 0.0m;
                                            //        if (decimal.TryParse(PersonalLoan.MonthlyInstallment, out res))
                                            //        {
                                            //            widgetHtml.Replace("{{MonthlyInstallment}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                                            //        }
                                            //        else
                                            //        {
                                            //            widgetHtml.Replace("{{MonthlyInstallment}}", "R0.00");
                                            //        }

                                            //        widgetHtml.Replace("{{Terms}}", PersonalLoan.Term);
                                            //        widgetHtml.Replace("{{DueByDate}}", PersonalLoan.DueDate.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy));
                                            //        htmlString.Append(widgetHtml.ToString());
                                            //    }
                                            //}
                                            //else if (mergedlst[i].WidgetName == HtmlConstants.PERSONAL_LOAN_TRANASCTION_WIDGET_NAME)
                                            //{
                                            //    string jsonstr = HtmlConstants.PERSONAL_LOAN_TRANSACTION_PREVIEW_JSON_STRING;
                                            //    if (jsonstr != string.Empty && validationEngine.IsValidJson(jsonstr))
                                            //    {
                                            //        var transactions = JsonConvert.DeserializeObject<List<DM_PersonalLoanTransaction>>(jsonstr);
                                            //        if (transactions != null && transactions.Count> 0)
                                            //        {
                                            //            var widgetHtml = new StringBuilder(HtmlConstants.PERSONAL_LOAN_TRANSACTION_HTML);
                                            //            var LoanTransactionRows = new StringBuilder();
                                            //            var tr = new StringBuilder();
                                            //            transactions.ForEach(trans =>
                                            //            {
                                            //                tr = new StringBuilder();
                                            //                tr.Append("<tr class='ht-20'>");
                                            //                tr.Append("<td class='w-13 text-center'> " + trans.PostingDate.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy) + " </td>");
                                            //                tr.Append("<td class='w-15 text-center'> " + trans.EffectiveDate.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy) + " </td>");
                                            //                tr.Append("<td class='w-35'> " + (!string.IsNullOrEmpty(trans.Description) ? trans.Description : ModelConstant.PAYMENT_THANK_YOU_TRANSACTION_DESC) + " </td>");

                                            //                var res = 0.0m;
                                            //                if (decimal.TryParse(trans.Debit, out res))
                                            //                {
                                            //                    tr.Append("<td class='w-12 text-right'> " + (res> 0 ? utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res) : "-") + " </td>");
                                            //                }
                                            //                else
                                            //                {
                                            //                    tr.Append("<td class='w-12 text-right'> - </td>");
                                            //                }

                                            //                res = 0.0m;
                                            //                if (decimal.TryParse(trans.Credit, out res))
                                            //                {
                                            //                    tr.Append("<td class='w-12 text-right'> " + (res> 0 ? utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res) : "-") + " </td>");
                                            //                }
                                            //                else
                                            //                {
                                            //                    tr.Append("<td class='w-12 text-right'> - </td>");
                                            //                }

                                            //                res = 0.0m;
                                            //                if (decimal.TryParse(trans.OutstandingCapital, out res))
                                            //                {
                                            //                    tr.Append("<td class='w-13 text-right'> " + (res> 0 ? utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res) : "-") + " </td>");
                                            //                }
                                            //                else
                                            //                {
                                            //                    tr.Append("<td class='w-13 text-right'> - </td>");
                                            //                }
                                            //                tr.Append("</tr>");
                                            //                LoanTransactionRows.Append(tr.ToString());
                                            //            });
                                            //            widgetHtml.Replace("{{PersonalLoanTransactionRow}}", LoanTransactionRows.ToString());
                                            //            htmlString.Append(widgetHtml.ToString());
                                            //        }
                                            //    }
                                            //}
                                            //else if (mergedlst[i].WidgetName == HtmlConstants.PERSONAL_LOAN_PAYMENT_DUE_WIDGET_NAME)
                                            //{
                                            //    string jsonstr = "{'Identifier': '1','CustomerId': '211135146504','InvestorId': '8004334234001','Arrears_120': '0','Arrears_90': '0','Arrears_60': '0','Arrears_30': '0','Arrears_0': '3297'}";
                                            //    if (jsonstr != string.Empty && validationEngine.IsValidJson(jsonstr))
                                            //    {
                                            //        var plArrears = JsonConvert.DeserializeObject<DM_PersonalLoanArrears>(jsonstr);
                                            //        if (plArrears != null)
                                            //        {
                                            //            var widgetHtml = new StringBuilder(HtmlConstants.PERSONAL_LOAN_PAYMENT_DUE_HTML);
                                            //            var res = 0.0m;
                                            //            if (decimal.TryParse(plArrears.Arrears_120, out res))
                                            //            {
                                            //                widgetHtml.Replace("{{After120Days}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                                            //            }
                                            //            else
                                            //            {
                                            //                widgetHtml.Replace("{{After120Days}}", "R0.00");
                                            //            }

                                            //            res = 0.0m;
                                            //            if (decimal.TryParse(plArrears.Arrears_90, out res))
                                            //            {
                                            //                widgetHtml.Replace("{{After90Days}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                                            //            }
                                            //            else
                                            //            {
                                            //                widgetHtml.Replace("{{After90Days}}", "R0.00");
                                            //            }

                                            //            res = 0.0m;
                                            //            if (decimal.TryParse(plArrears.Arrears_60, out res))
                                            //            {
                                            //                widgetHtml.Replace("{{After60Days}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                                            //            }
                                            //            else
                                            //            {
                                            //                widgetHtml.Replace("{{After60Days}}", "R0.00");
                                            //            }

                                            //            res = 0.0m;
                                            //            if (decimal.TryParse(plArrears.Arrears_30, out res))
                                            //            {
                                            //                widgetHtml.Replace("{{After30Days}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                                            //            }
                                            //            else
                                            //            {
                                            //                widgetHtml.Replace("{{After30Days}}", "R0.00");
                                            //            }

                                            //            res = 0.0m;
                                            //            if (decimal.TryParse(plArrears.Arrears_0, out res))
                                            //            {
                                            //                widgetHtml.Replace("{{Current}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                                            //            }
                                            //            else
                                            //            {
                                            //                widgetHtml.Replace("{{Current}}", "R0.00");
                                            //            }

                                            //            htmlString.Append(widgetHtml.ToString());
                                            //        }
                                            //    }
                                            //}
                                            else if (mergedlst[i].WidgetName == HtmlConstants.SPECIAL_MESSAGE_WIDGET_NAME)
                                            {
                                                var widgetHtml = new StringBuilder(HtmlConstants.SPECIAL_MESSAGE_HTML);
                                                //if ((page.PageTypeName == HtmlConstants.HOME_LOAN_FOR_OTHER_SEGMENT_ENG_PAGE_TYPE || page.PageTypeName == HtmlConstants.HOME_LOAN_FOR_OTHER_SEGMENT_AFR_PAGE_TYPE
                                                //    || page.PageTypeName == HtmlConstants.HOME_LOAN_FOR_PML_SEGMENT_ENG_PAGE_TYPE || page.PageTypeName == HtmlConstants.HOME_LOAN_FOR_PML_SEGMENT_AFR_PAGE_TYPE
                                                //    || page.PageTypeName == HtmlConstants.HOME_LOAN_FOR_WEA_SEGMENT_AFR_PAGE_TYPE || page.PageTypeName == HtmlConstants.HOME_LOAN_FOR_WEA_SEGMENT_ENG_PAGE_TYPE))
                                                //{
                                                //    string jsonstr = HtmlConstants.HOME_LOAN_SPECIAL_MESSAGES_WIDGET_PREVIEW_JSON_STRING;
                                                //    if (jsonstr != string.Empty && validationEngine.IsValidJson(jsonstr))
                                                //    {
                                                //        var SpecialMessage = JsonConvert.DeserializeObject<SpecialMessage>(jsonstr);
                                                //        if (SpecialMessage != null)
                                                //        {
                                                //            var specialMsgTxtData = HtmlConstants.HOME_LOAN_NBB_SPECIAL_MESSAGE;

                                                //            widgetHtml.Replace("{{SpecialMessageTextData}}", specialMsgTxtData);
                                                //            htmlString.Append(widgetHtml.ToString());
                                                //        }
                                                //    }
                                                //}
                                                //else
                                                //{
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
                                            //else if (mergedlst[i].WidgetName == HtmlConstants.PERSONAL_LOAN_INSURANCE_MESSAGE_WIDGET_NAME)
                                            //{
                                            //    string jsonstr = HtmlConstants.SPECIAL_MESSAGES_WIDGET_PREVIEW_JSON_STRING;
                                            //    if (jsonstr != string.Empty && validationEngine.IsValidJson(jsonstr))
                                            //    {
                                            //        var InsuranceMsg = JsonConvert.DeserializeObject<SpecialMessage>(jsonstr);
                                            //        if (InsuranceMsg != null)
                                            //        {
                                            //            var widgetHtml = new StringBuilder(HtmlConstants.PERSONAL_LOAN_INSURANCE_MESSAGE_HTML);
                                            //            var InsuranceMsgTxtData = (!string.IsNullOrEmpty(InsuranceMsg.Message3) ? "<p> " + InsuranceMsg.Message3 + " </p>" : string.Empty) +
                                            //                (!string.IsNullOrEmpty(InsuranceMsg.Message4) ? "<p> " + InsuranceMsg.Message4 + " </p>" : string.Empty) +
                                            //                (!string.IsNullOrEmpty(InsuranceMsg.Message5) ? "<p> " + InsuranceMsg.Message5 + " </p>" : string.Empty);

                                            //            widgetHtml.Replace("{{InsuranceMessages}}", InsuranceMsgTxtData);
                                            //            htmlString.Append(widgetHtml.ToString());
                                            //        }
                                            //    }
                                            //}
                                            //else if (mergedlst[i].WidgetName == HtmlConstants.PERSONAL_LOAN_TOTAL_AMOUNT_DETAIL_WIDGET_NAME)
                                            //{
                                            //    string jsonstr = HtmlConstants.PERSONAL_LOAN_ACCOUNTS_PREVIEW_JSON_STRING;
                                            //    var widgetHtml = new StringBuilder(HtmlConstants.PERSONAL_LOAN_TOTAL_AMOUNT_DETAIL_WIDGET_HTML);
                                            //    var TotalLoanAmt = 0.0m;
                                            //    var TotalOutstandingAmt = 0.0m;
                                            //    var TotalLoanDueAmt = 0.0m;

                                            //    if (jsonstr != string.Empty && validationEngine.IsValidJson(jsonstr))
                                            //    {
                                            //        var PersonalLoans = JsonConvert.DeserializeObject<List<DM_PersonalLoanMaster>>(jsonstr);
                                            //        if (PersonalLoans != null && PersonalLoans.Count> 0)
                                            //        {
                                            //            var res = 0.0m;
                                            //            try
                                            //            {
                                            //                TotalLoanAmt = PersonalLoans.Select(it => decimal.TryParse(it.CreditAdvance, out res) ? res : 0).ToList().Sum(it => it);
                                            //            }
                                            //            catch
                                            //            {
                                            //                TotalLoanAmt = 0.0m;
                                            //            }

                                            //            res = 0.0m;
                                            //            try
                                            //            {
                                            //                TotalOutstandingAmt = PersonalLoans.Select(it => decimal.TryParse(it.OutstandingBalance, out res) ? res : 0).ToList().Sum(it => it);
                                            //            }
                                            //            catch
                                            //            {
                                            //                TotalOutstandingAmt = 0.0m;
                                            //            }

                                            //            res = 0.0m;
                                            //            try
                                            //            {
                                            //                TotalLoanDueAmt = PersonalLoans.Select(it => decimal.TryParse(it.AmountDue, out res) ? res : 0).ToList().Sum(it => it);
                                            //            }
                                            //            catch
                                            //            {
                                            //                TotalLoanDueAmt = 0.0m;
                                            //            }
                                            //        }
                                            //    }

                                            //    widgetHtml.Replace("{{TotalLoanAmount}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, TotalLoanAmt));
                                            //    widgetHtml.Replace("{{OutstandingBalance}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, TotalOutstandingAmt));
                                            //    widgetHtml.Replace("{{DueAmount}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, TotalLoanDueAmt));
                                            //    htmlString.Append(widgetHtml.ToString());
                                            //}
                                            //else if (mergedlst[i].WidgetName == HtmlConstants.PERSONAL_LOAN_ACCOUNTS_BREAKDOWN_WIDGET_NAME)
                                            //{
                                            //    string jsonstr = HtmlConstants.PERSONAL_LOAN_ACCOUNTS_PREVIEW_JSON_STRING;
                                            //    var widgetHtml = new StringBuilder(HtmlConstants.PERSONAL_LOAN_ACCOUNTS_BREAKDOWN_WIDGET_HTML);
                                            //    if (jsonstr != string.Empty && validationEngine.IsValidJson(jsonstr))
                                            //    {
                                            //        var PersonalLoans = JsonConvert.DeserializeObject<List<DM_PersonalLoanMaster>>(jsonstr);
                                            //        if (PersonalLoans != null && PersonalLoans.Count> 0)
                                            //        {
                                            //            //create tab-content div if accounts is greater than 1, otherwise create simple div
                                            //            var TabContentHtml = new StringBuilder();
                                            //            var counter = 0;
                                            //            TabContentHtml.Append((PersonalLoans.Count> 1) ? "<div class='tab-content'>" : string.Empty);
                                            //            PersonalLoans.ForEach(PersonalLoan =>
                                            //            {
                                            //                string lastFourDigisOfAccountNumber = PersonalLoan.InvestorId.ToString().Length> 4 ? PersonalLoan.InvestorId.ToString().Substring(Math.Max(0, PersonalLoan.InvestorId.ToString().Length - 4)) : PersonalLoan.InvestorId.ToString();

                                            //                TabContentHtml.Append("<div id='PersonalLoan-" + lastFourDigisOfAccountNumber + "'>");

                                            //                var LoanDetailHtml = new StringBuilder(HtmlConstants.PERSONAL_LOAN_ACCOUNT_DETAIL);
                                            //                LoanDetailHtml.Replace("{{AccountNumber}}", PersonalLoan.InvestorId.ToString());
                                            //                LoanDetailHtml.Replace("{{StatementDate}}", PersonalLoan.ToDate.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy));
                                            //                LoanDetailHtml.Replace("{{StatementPeriod}}", PersonalLoan.FromDate.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy) + " - " + PersonalLoan.ToDate.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy));

                                            //                var res = 0.0m;
                                            //                if (decimal.TryParse(PersonalLoan.Arrears, out res))
                                            //                {
                                            //                    LoanDetailHtml.Replace("{{ArrearsAmount}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                                            //                }
                                            //                else
                                            //                {
                                            //                    LoanDetailHtml.Replace("{{ArrearsAmount}}", "R0.00");
                                            //                }

                                            //                LoanDetailHtml.Replace("{{AnnualRate}}", PersonalLoan.AnnualRate + "% pa");

                                            //                res = 0.0m;
                                            //                if (decimal.TryParse(PersonalLoan.MonthlyInstallment, out res))
                                            //                {
                                            //                    LoanDetailHtml.Replace("{{MonthlyInstallment}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                                            //                }
                                            //                else
                                            //                {
                                            //                    LoanDetailHtml.Replace("{{MonthlyInstallment}}", "R0.00");
                                            //                }

                                            //                LoanDetailHtml.Replace("{{Terms}}", PersonalLoan.Term);
                                            //                LoanDetailHtml.Replace("{{DueByDate}}", PersonalLoan.DueDate.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy));
                                            //                TabContentHtml.Append(LoanDetailHtml.ToString());

                                            //                var LoanTransactionRows = new StringBuilder();
                                            //                var tr = new StringBuilder();
                                            //                if (PersonalLoan.LoanTransactions != null && PersonalLoan.LoanTransactions.Count> 0)
                                            //                {
                                            //                    var LoanTransactionDetailHtml = new StringBuilder(HtmlConstants.PERSONAL_LOAN_ACCOUNT_TRANSACTION_DETAIL);
                                            //                    PersonalLoan.LoanTransactions.ForEach(trans =>
                                            //                    {
                                            //                        tr = new StringBuilder();
                                            //                        tr.Append("<tr class='ht-20'>");
                                            //                        tr.Append("<td class='w-13 text-center'> " + trans.PostingDate.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy) + " </td>");
                                            //                        tr.Append("<td class='w-15 text-center'> " + trans.EffectiveDate.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy) + " </td>");
                                            //                        tr.Append("<td class='w-35'> " + (!string.IsNullOrEmpty(trans.Description) ? trans.Description : ModelConstant.PAYMENT_THANK_YOU_TRANSACTION_DESC) + " </td>");

                                            //                        res = 0.0m;
                                            //                        if (decimal.TryParse(trans.Debit, out res))
                                            //                        {
                                            //                            tr.Append("<td class='w-12 text-right'> " + (res> 0 ? utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res) : "-") + " </td>");
                                            //                        }
                                            //                        else
                                            //                        {
                                            //                            tr.Append("<td class='w-12 text-right'> - </td>");
                                            //                        }

                                            //                        res = 0.0m;
                                            //                        if (decimal.TryParse(trans.Credit, out res))
                                            //                        {
                                            //                            tr.Append("<td class='w-12 text-right'> " + (res> 0 ? utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res) : "-") + " </td>");
                                            //                        }
                                            //                        else
                                            //                        {
                                            //                            tr.Append("<td class='w-12 text-right'> - </td>");
                                            //                        }

                                            //                        res = 0.0m;
                                            //                        if (decimal.TryParse(trans.OutstandingCapital, out res))
                                            //                        {
                                            //                            tr.Append("<td class='w-13 text-right'> " + (res> 0 ? utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res) : "-") + " </td>");
                                            //                        }
                                            //                        else
                                            //                        {
                                            //                            tr.Append("<td class='w-13 text-right'> - </td>");
                                            //                        }
                                            //                        tr.Append("</tr>");

                                            //                        LoanTransactionRows.Append(tr.ToString());
                                            //                    });

                                            //                    LoanTransactionDetailHtml.Replace("{{PersonalLoanTransactionRow}}", LoanTransactionRows.ToString());
                                            //                    TabContentHtml.Append(LoanTransactionDetailHtml.ToString());
                                            //                }

                                            //                if (PersonalLoan.LoanArrears != null)
                                            //                {
                                            //                    var plArrears = PersonalLoan.LoanArrears;
                                            //                    var LoanArrearHtml = new StringBuilder(HtmlConstants.PERSONAL_LOAN_PAYMENT_DUE_DETAIL);
                                            //                    res = 0.0m;
                                            //                    if (decimal.TryParse(plArrears.Arrears_120, out res))
                                            //                    {
                                            //                        LoanArrearHtml.Replace("{{After120Days}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                                            //                    }
                                            //                    else
                                            //                    {
                                            //                        LoanArrearHtml.Replace("{{After120Days}}", "R0.00");
                                            //                    }

                                            //                    res = 0.0m;
                                            //                    if (decimal.TryParse(plArrears.Arrears_90, out res))
                                            //                    {
                                            //                        LoanArrearHtml.Replace("{{After90Days}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                                            //                    }
                                            //                    else
                                            //                    {
                                            //                        LoanArrearHtml.Replace("{{After90Days}}", "R0.00");
                                            //                    }

                                            //                    res = 0.0m;
                                            //                    if (decimal.TryParse(plArrears.Arrears_60, out res))
                                            //                    {
                                            //                        LoanArrearHtml.Replace("{{After60Days}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                                            //                    }
                                            //                    else
                                            //                    {
                                            //                        LoanArrearHtml.Replace("{{After60Days}}", "R0.00");
                                            //                    }

                                            //                    res = 0.0m;
                                            //                    if (decimal.TryParse(plArrears.Arrears_30, out res))
                                            //                    {
                                            //                        LoanArrearHtml.Replace("{{After30Days}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                                            //                    }
                                            //                    else
                                            //                    {
                                            //                        LoanArrearHtml.Replace("{{After30Days}}", "R0.00");
                                            //                    }

                                            //                    res = 0.0m;
                                            //                    if (decimal.TryParse(plArrears.Arrears_0, out res))
                                            //                    {
                                            //                        LoanArrearHtml.Replace("{{Current}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                                            //                    }
                                            //                    else
                                            //                    {
                                            //                        LoanArrearHtml.Replace("{{Current}}", "R0.00");
                                            //                    }

                                            //                    TabContentHtml.Append(LoanArrearHtml.ToString());
                                            //                }
                                            //                TabContentHtml.Append(HtmlConstants.END_DIV_TAG);
                                            //                counter++;
                                            //            });

                                            //            TabContentHtml.Append((PersonalLoans.Count> 1) ? HtmlConstants.END_DIV_TAG : string.Empty);
                                            //            widgetHtml.Replace("{{TabContentsDiv}}", TabContentHtml.ToString());
                                            //            htmlString.Append(widgetHtml.ToString());
                                            //        }
                                            //    }
                                            //}
                                            //else if (mergedlst[i].WidgetName == HtmlConstants.HOME_LOAN_TOTAL_AMOUNT_DETAIL_WIDGET_NAME)
                                            //{
                                            //    string jsonstr = HtmlConstants.HOME_LOAN_ACCOUNTS_PREVIEW_JSON_STRING;
                                            //    var widgetHtml = new StringBuilder(HtmlConstants.HOME_LOAN_TOTAL_AMOUNT_DETAIL_WIDGET_HTML);
                                            //    var TotalLoanAmt = 0.0m;
                                            //    var TotalOutstandingAmt = 0.0m;
                                            //    string instalmentLabel = string.Empty;
                                            //    if (jsonstr != string.Empty && validationEngine.IsValidJson(jsonstr))
                                            //    {
                                            //        var HomeLoans = JsonConvert.DeserializeObject<List<DM_HomeLoanMaster>>(jsonstr);
                                            //        if (HomeLoans != null && HomeLoans.Count> 0)
                                            //        {
                                            //            var res = 0.0m;
                                            //            try
                                            //            {
                                            //                TotalLoanAmt = HomeLoans.Select(it => decimal.TryParse(it.LoanAmount, out res) ? res : 0).ToList().Sum(it => it);
                                            //            }
                                            //            catch
                                            //            {
                                            //                TotalLoanAmt = 0.0m;
                                            //            }

                                            //            res = 0.0m;
                                            //            try
                                            //            {
                                            //                TotalOutstandingAmt = HomeLoans.Select(it => decimal.TryParse(it.Balance, out res) ? res : 0).ToList().Sum(it => it);
                                            //            }
                                            //            catch
                                            //            {
                                            //                TotalOutstandingAmt = 0.0m;
                                            //            }

                                            //            var segmentType = HomeLoans.Select(it => it.SegmentType).FirstOrDefault();

                                            //            switch (segmentType.ToLower())
                                            //            {
                                            //                case HtmlConstants.MONTHLY_SEGMENT_FREQUENCY:
                                            //                    instalmentLabel = HtmlConstants.MONTHLY_INSTALMENT_LABEL;
                                            //                    break;
                                            //                case HtmlConstants.QUARTERLY_SEGMENT_FREQUENCY:
                                            //                    instalmentLabel = HtmlConstants.QUARTERLY_INSTALMENT_LABEL;
                                            //                    break;
                                            //                case HtmlConstants.ANNUAL_SEGMENT_FREQUENCY:
                                            //                    instalmentLabel = HtmlConstants.ANNUAL_INSTALMENT_LABEL;
                                            //                    break;
                                            //                default:
                                            //                    instalmentLabel = HtmlConstants.MONTHLY_INSTALMENT_LABEL;
                                            //                    break;
                                            //            }
                                            //        }

                                            //        widgetHtml.Replace("{{InstalmentType}}", instalmentLabel);
                                            //        widgetHtml.Replace("{{TotalHomeLoansAmount}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, TotalLoanAmt));
                                            //        widgetHtml.Replace("{{TotalHomeLoansBalanceOutstanding}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, TotalOutstandingAmt));
                                            //        htmlString.Append(widgetHtml.ToString());
                                            //    }
                                            //}
                                            //else if (mergedlst[i].WidgetName == HtmlConstants.HOME_LOAN_ACCOUNTS_BREAKDOWN_WIDGET_NAME)
                                            //{
                                            //    string jsonstr = HtmlConstants.HOME_LOAN_ACCOUNTS_PREVIEW_JSON_STRING;
                                            //    var widgetHtml = new StringBuilder(HtmlConstants.HOME_LOAN_ACCOUNTS_BREAKDOWN_HTML);
                                            //    if (jsonstr != string.Empty && validationEngine.IsValidJson(jsonstr))
                                            //    {
                                            //        var HomeLoans = JsonConvert.DeserializeObject<List<DM_HomeLoanMaster>>(jsonstr);
                                            //        if (HomeLoans != null && HomeLoans.Count> 0)
                                            //        {
                                            //            //create tab-content div if accounts is greater than 1, otherwise create simple div
                                            //            var TabContentHtml = new StringBuilder();
                                            //            var counter = 0;
                                            //            TabContentHtml.Append((HomeLoans.Count> 1) ? "<div class='tab-content'>" : string.Empty);
                                            //            HomeLoans.ForEach(HomeLoan =>
                                            //            {
                                            //                var accNo = HomeLoan.InvestorId.ToString();
                                            //                string lastFourDigisOfAccountNumber = accNo.Length> 4 ? accNo.Substring(Math.Max(0, accNo.Length - 4)) : accNo;

                                            //                TabContentHtml.Append("<div id='HomeLoan-" + lastFourDigisOfAccountNumber + "'>");

                                            //                var LoanDetailHtml = new StringBuilder(HtmlConstants.HOME_LOAN_ACCOUNT_DETAIL_DIV_HTML);
                                            //                LoanDetailHtml.Replace("{{BondNumber}}", accNo);
                                            //                LoanDetailHtml.Replace("{{RegistrationDate}}", HomeLoan.RegisteredDate.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy));

                                            //                var secDesc1 = string.Empty;
                                            //                var secDesc2 = string.Empty;
                                            //                var secDesc3 = string.Empty;
                                            //                if (HomeLoan.SecDescription1.Length> 15 || ((HomeLoan.SecDescription1 + " " + HomeLoan.SecDescription2).Length> 25))
                                            //                {
                                            //                    secDesc1 = HomeLoan.SecDescription1;
                                            //                    if ((HomeLoan.SecDescription2 + " " + HomeLoan.SecDescription3).Length> 25)
                                            //                    {
                                            //                        secDesc2 = HomeLoan.SecDescription2;
                                            //                        secDesc3 = HomeLoan.SecDescription3;
                                            //                    }
                                            //                    else
                                            //                    {
                                            //                        secDesc2 = HomeLoan.SecDescription2 + " " + HomeLoan.SecDescription3;
                                            //                    }
                                            //                }
                                            //                else
                                            //                {
                                            //                    secDesc1 = HomeLoan.SecDescription1 + " " + HomeLoan.SecDescription2;
                                            //                    secDesc2 = HomeLoan.SecDescription3;
                                            //                }

                                            //                LoanDetailHtml.Replace("{{SecDescription1}}", secDesc1);
                                            //                LoanDetailHtml.Replace("{{SecDescription2}}", secDesc2);
                                            //                LoanDetailHtml.Replace("{{SecDescription3}}", secDesc3);

                                            //                var res = 0.0m;
                                            //                if (decimal.TryParse(HomeLoan.IntialDue, out res))
                                            //                {
                                            //                    LoanDetailHtml.Replace("{{Instalment}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                                            //                }
                                            //                else
                                            //                {
                                            //                    LoanDetailHtml.Replace("{{Instalment}}", "R0.00");
                                            //                }

                                            //                LoanDetailHtml.Replace("{{InterestRate}}", HomeLoan.ChargeRate + "% pa");

                                            //                res = 0.0m;
                                            //                if (decimal.TryParse(HomeLoan.ArrearStatus, out res))
                                            //                {
                                            //                    LoanDetailHtml.Replace("{{Arrears}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                                            //                }
                                            //                else
                                            //                {
                                            //                    LoanDetailHtml.Replace("{{Arrears}}", "R0.00");
                                            //                }

                                            //                res = 0.0m;
                                            //                if (decimal.TryParse(HomeLoan.RegisteredAmount, out res))
                                            //                {
                                            //                    LoanDetailHtml.Replace("{{RegisteredAmount}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                                            //                }
                                            //                else
                                            //                {
                                            //                    LoanDetailHtml.Replace("{{RegisteredAmount}}", "R0.00");
                                            //                }

                                            //                LoanDetailHtml.Replace("{{LoanTerms}}", HomeLoan.LoanTerm);
                                            //                TabContentHtml.Append(LoanDetailHtml.ToString());

                                            //                var LoanTransactionRows = new StringBuilder();
                                            //                var LoanTransactionDetailHtml = new StringBuilder(HtmlConstants.HOME_LOAN_TRANSACTION_DETAIL_DIV_HTML);

                                            //                var tr = new StringBuilder();
                                            //                if (HomeLoan.LoanTransactions != null && HomeLoan.LoanTransactions.Count> 0)
                                            //                {
                                            //                    HomeLoan.LoanTransactions.ForEach(trans =>
                                            //                    {
                                            //                        tr = new StringBuilder();
                                            //                        tr.Append("<tr class='ht-20'>");
                                            //                        tr.Append("<td class='w-13 text-center'> " + trans.Posting_Date.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy) + " </td>");
                                            //                        tr.Append("<td class='w-15 text-center'> " + trans.Effective_Date.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy) + " </td>");
                                            //                        tr.Append("<td class='w-35'> " + (!string.IsNullOrEmpty(trans.Description) ? trans.Description : ModelConstant.PAYMENT_THANK_YOU_TRANSACTION_DESC) + " </td>");

                                            //                        res = 0.0m;
                                            //                        if (decimal.TryParse(trans.Debit, out res))
                                            //                        {
                                            //                            tr.Append("<td class='w-12 text-right'> " + (res> 0 ? utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res) : "-") + " </td>");
                                            //                        }
                                            //                        else
                                            //                        {
                                            //                            tr.Append("<td class='w-12 text-right'> - </td>");
                                            //                        }

                                            //                        res = 0.0m;
                                            //                        if (decimal.TryParse(trans.Credit, out res))
                                            //                        {
                                            //                            tr.Append("<td class='w-12 text-right'> " + (res> 0 ? utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res) : "-") + " </td>");
                                            //                        }
                                            //                        else
                                            //                        {
                                            //                            tr.Append("<td class='w-12 text-right'> - </td>");
                                            //                        }

                                            //                        res = 0.0m;
                                            //                        if (decimal.TryParse(trans.RunningBalance, out res))
                                            //                        {
                                            //                            tr.Append("<td class='w-13 text-right'> " + (res> 0 ? utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res) : "-") + " </td>");
                                            //                        }
                                            //                        else
                                            //                        {
                                            //                            tr.Append("<td class='w-13 text-right'> - </td>");
                                            //                        }
                                            //                        tr.Append("</tr>");

                                            //                        LoanTransactionRows.Append(tr.ToString());
                                            //                    });
                                            //                }
                                            //                LoanTransactionDetailHtml.Replace("{{HomeLoanTransactionRow}}", LoanTransactionRows.ToString());
                                            //                TabContentHtml.Append(LoanTransactionDetailHtml.ToString());

                                            //                var LoanArrearHtml = new StringBuilder(HtmlConstants.HOME_LOAN_STATEMENT_OVERVIEW_AND_PAYMENT_DUE_DIV_HTML);
                                            //                LoanArrearHtml.Replace("{{StatementDate}}", DateTime.Now.ToString(ModelConstant.DATE_FORMAT_yyyy_MM_dd));
                                            //                res = 0.0m;
                                            //                if (decimal.TryParse(HomeLoan.Balance, out res))
                                            //                {
                                            //                    LoanArrearHtml.Replace("{{BalanceOutstanding}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                                            //                }
                                            //                else
                                            //                {
                                            //                    LoanArrearHtml.Replace("{{BalanceOutstanding}}", "R0.00");
                                            //                }
                                            //                if (HomeLoan.LoanArrear != null)
                                            //                {
                                            //                    var plArrears = HomeLoan.LoanArrear;
                                            //                    res = 0.0m;
                                            //                    if (decimal.TryParse(plArrears.ARREARS_120, out res))
                                            //                    {
                                            //                        LoanArrearHtml.Replace("{{After120Days}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                                            //                    }
                                            //                    else
                                            //                    {
                                            //                        LoanArrearHtml.Replace("{{After120Days}}", "R0.00");
                                            //                    }

                                            //                    res = 0.0m;
                                            //                    if (decimal.TryParse(plArrears.ARREARS_90, out res))
                                            //                    {
                                            //                        LoanArrearHtml.Replace("{{After90Days}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                                            //                    }
                                            //                    else
                                            //                    {
                                            //                        LoanArrearHtml.Replace("{{After90Days}}", "R0.00");
                                            //                    }

                                            //                    res = 0.0m;
                                            //                    if (decimal.TryParse(plArrears.ARREARS_60, out res))
                                            //                    {
                                            //                        LoanArrearHtml.Replace("{{After60Days}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                                            //                    }
                                            //                    else
                                            //                    {
                                            //                        LoanArrearHtml.Replace("{{After60Days}}", "R0.00");
                                            //                    }

                                            //                    res = 0.0m;
                                            //                    if (decimal.TryParse(plArrears.ARREARS_30, out res))
                                            //                    {
                                            //                        LoanArrearHtml.Replace("{{After30Days}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                                            //                    }
                                            //                    else
                                            //                    {
                                            //                        LoanArrearHtml.Replace("{{After30Days}}", "R0.00");
                                            //                    }

                                            //                    res = 0.0m;
                                            //                    if (decimal.TryParse(plArrears.CurrentDue, out res))
                                            //                    {
                                            //                        LoanArrearHtml.Replace("{{CurrentPaymentDue}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                                            //                    }
                                            //                    else
                                            //                    {
                                            //                        LoanArrearHtml.Replace("{{CurrentPaymentDue}}", "R0.00");
                                            //                    }
                                            //                }
                                            //                else
                                            //                {
                                            //                    LoanArrearHtml.Replace("{{After120Days}}", "R0.00");
                                            //                    LoanArrearHtml.Replace("{{After90Days}}", "R0.00");
                                            //                    LoanArrearHtml.Replace("{{After60Days}}", "R0.00");
                                            //                    LoanArrearHtml.Replace("{{After30Days}}", "R0.00");
                                            //                    LoanArrearHtml.Replace("{{CurrentPaymentDue}}", "R0.00");
                                            //                }
                                            //                TabContentHtml.Append(LoanArrearHtml.ToString());

                                            //                var PaymentDueMessageDivHtml = new StringBuilder(HtmlConstants.HOME_LOAN_PAYMENT_DUE_SPECIAL_MESSAGE_DIV_HTML);
                                            //                var spjsonstr = HtmlConstants.HOME_LOAN_SPECIAL_MESSAGES_WIDGET_PREVIEW_JSON_STRING;
                                            //                if (spjsonstr != string.Empty && validationEngine.IsValidJson(spjsonstr))
                                            //                {
                                            //                    var SpecialMessage = JsonConvert.DeserializeObject<SpecialMessage>(spjsonstr);
                                            //                    if (SpecialMessage != null)
                                            //                    {
                                            //                        var PaymentDueMessage = (!string.IsNullOrEmpty(SpecialMessage.Message3) ? "<p> " + SpecialMessage.Message3 + " </p>" : string.Empty) + (!string.IsNullOrEmpty(SpecialMessage.Message4) ? "<p> " + SpecialMessage.Message4 + " </p>" : string.Empty) + (!string.IsNullOrEmpty(SpecialMessage.Message5) ? "<p> " + SpecialMessage.Message5 + " </p>" : string.Empty);

                                            //                        PaymentDueMessageDivHtml.Replace("{{PaymentDueSpecialMessage}}", PaymentDueMessage);
                                            //                        TabContentHtml.Append(PaymentDueMessageDivHtml.ToString());
                                            //                    }
                                            //                }

                                            //                TabContentHtml.Append(HtmlConstants.END_DIV_TAG);
                                            //                counter++;
                                            //            });

                                            //            TabContentHtml.Append((HomeLoans.Count> 1) ? HtmlConstants.END_DIV_TAG : string.Empty);
                                            //            widgetHtml.Replace("{{TabContentsDiv}}", TabContentHtml.ToString());
                                            //            htmlString.Append(widgetHtml.ToString());
                                            //        }
                                            //    }
                                            //}
                                            //else if (mergedlst[i].WidgetName == HtmlConstants.HOME_LOAN_SUMMARY_TAX_PURPOSE_WIDGET_NAME)
                                            //{
                                            //    string jsonstr = HtmlConstants.HOME_LOAN_SUMMARY_TAX_PURPOSE_PREVIEW_JSON_STRING;
                                            //    if (jsonstr != string.Empty && validationEngine.IsValidJson(jsonstr))
                                            //    {
                                            //        var summaryTax = JsonConvert.DeserializeObject<DM_HomeLoanSummary>(jsonstr);
                                            //        var htmlWidget = new StringBuilder(HtmlConstants.HOME_LOAN_SUMMARY_TAX_PURPOSE_HTML);
                                            //        htmlWidget.Replace("{{Interest}}", summaryTax.Annual_Interest);
                                            //        htmlWidget.Replace("{{Insurance}}", summaryTax.Annual_Insurance);
                                            //        htmlWidget.Replace("{{Servicefee}}", summaryTax.Annual_Service_Fee);
                                            //        htmlWidget.Replace("{{Legalcosts}}", summaryTax.Annual_Legal_Costs);
                                            //        htmlWidget.Replace("{{AmountReceived}}", summaryTax.Annual_Total_Recvd);
                                            //        htmlString.Append(htmlWidget);
                                            //    }
                                            //}
                                            //else if (mergedlst[i].WidgetName == HtmlConstants.HOME_LOAN_INSTALMENT_WIDGET_NAME)
                                            //{
                                            //    string jsonstr = HtmlConstants.HOME_LOAN_INSTALMENT_PREVIEW_JSON_STRING;
                                            //    if (jsonstr != string.Empty && validationEngine.IsValidJson(jsonstr))
                                            //    {
                                            //        var summaryTax = JsonConvert.DeserializeObject<DM_HomeLoanSummary>(jsonstr);
                                            //        var htmlWidget = new StringBuilder(HtmlConstants.HOME_LOAN_INSTALMENT_DETAILS_WIDGET_HTML);
                                            //        var htmlWidgetDetails = new StringBuilder(HtmlConstants.HOME_LOAN_INSTALMENT_DETAILS_HTML);
                                            //        var res = 0.0m;
                                            //        if (!string.IsNullOrEmpty(summaryTax.Basic_Instalment) && decimal.TryParse(summaryTax.Basic_Instalment, out res))
                                            //        {
                                            //            htmlWidgetDetails.Replace("{{BasicInstalment}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                                            //        }
                                            //        else
                                            //        {
                                            //            htmlWidgetDetails.Replace("{{BasicInstalment}}", "R0.00");
                                            //        }
                                            //        res = 0.0m;
                                            //        if (!string.IsNullOrEmpty(summaryTax.Houseowner_Ins) && decimal.TryParse(summaryTax.Houseowner_Ins, out res))
                                            //        {
                                            //            htmlWidgetDetails.Replace("{{HouseownerInsurance}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                                            //        }
                                            //        else
                                            //        {
                                            //            htmlWidgetDetails.Replace("{{HouseownerInsurance}}", "R0.00");
                                            //        }

                                            //        res = 0.0m;
                                            //        if (!string.IsNullOrEmpty(summaryTax.Loan_Protection) && decimal.TryParse(summaryTax.Loan_Protection, out res))
                                            //        {
                                            //            htmlWidgetDetails.Replace("{{LoanProtectionAssurance}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                                            //        }
                                            //        else
                                            //        {
                                            //            htmlWidgetDetails.Replace("{{LoanProtectionAssurance}}", "R0.00");
                                            //        }

                                            //        res = 0.0m;
                                            //        if (!string.IsNullOrEmpty(summaryTax.Recovery_Fee_Debit) && decimal.TryParse(summaryTax.Recovery_Fee_Debit, out res))
                                            //        {
                                            //            htmlWidgetDetails.Replace("{{RecoveryOfFeeDebits}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                                            //        }
                                            //        else
                                            //        {
                                            //            htmlWidgetDetails.Replace("{{RecoveryOfFeeDebits}}", "R0.00");
                                            //        }

                                            //        res = 0.0m;
                                            //        if (!string.IsNullOrEmpty(summaryTax.Capital_Redemption) && decimal.TryParse(summaryTax.Capital_Redemption, out res))
                                            //        {
                                            //            htmlWidgetDetails.Replace("{{CapitalRedemption}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                                            //        }
                                            //        else
                                            //        {
                                            //            htmlWidgetDetails.Replace("{{CapitalRedemption}}", "R0.00");
                                            //        }

                                            //        res = 0.0m;
                                            //        if (!string.IsNullOrEmpty(summaryTax.Service_Fee) && decimal.TryParse(summaryTax.Service_Fee, out res))
                                            //        {
                                            //            htmlWidgetDetails.Replace("{{ServiceFee}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                                            //        }
                                            //        else
                                            //        {
                                            //            htmlWidgetDetails.Replace("{{ServiceFee}}", "R0.00");
                                            //        }

                                            //        res = 0.0m;
                                            //        if (!string.IsNullOrEmpty(summaryTax.Total_Instalment) && decimal.TryParse(summaryTax.Total_Instalment, out res))
                                            //        {
                                            //            htmlWidgetDetails.Replace("{{TotalInstalment}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                                            //        }
                                            //        else
                                            //        {
                                            //            htmlWidgetDetails.Replace("{{TotalInstalment}}", "R0.00");
                                            //        }

                                            //        htmlWidgetDetails.Replace("{{InstalmentDate}}", DateTime.Now.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy));
                                            //        htmlWidget.Replace("{{Home_Loan_Instalment_Details}}", htmlWidgetDetails.ToString());
                                            //        htmlString.Append(htmlWidget);
                                            //    }
                                            //}
                                            //else if (mergedlst[i].WidgetName == HtmlConstants.WEALTH_HOME_LOAN_TOTAL_AMOUNT_WIDGET_NAME)
                                            //{
                                            //    string jsonstr = HtmlConstants.HOME_LOAN_ACCOUNTS_PREVIEW_JSON_STRING;
                                            //    var widgetHtml = new StringBuilder(HtmlConstants.HOME_LOAN_WEALTH_TOTAL_AMOUNT_DETAIL_WIDGET_HTML);
                                            //    var TotalLoanAmt = 0.0m;
                                            //    var TotalOutstandingAmt = 0.0m;
                                            //    string instalmentLabel = string.Empty;
                                            //    if (jsonstr != string.Empty && validationEngine.IsValidJson(jsonstr))
                                            //    {
                                            //        var HomeLoans = JsonConvert.DeserializeObject<List<DM_HomeLoanMaster>>(jsonstr);
                                            //        if (HomeLoans != null && HomeLoans.Count> 0)
                                            //        {
                                            //            var res = 0.0m;
                                            //            try
                                            //            {
                                            //                TotalLoanAmt = HomeLoans.Select(it => decimal.TryParse(it.LoanAmount, out res) ? res : 0).ToList().Sum(it => it);
                                            //            }
                                            //            catch
                                            //            {
                                            //                TotalLoanAmt = 0.0m;
                                            //            }

                                            //            res = 0.0m;
                                            //            try
                                            //            {
                                            //                TotalOutstandingAmt = HomeLoans.Select(it => decimal.TryParse(it.Balance, out res) ? res : 0).ToList().Sum(it => it);
                                            //            }
                                            //            catch
                                            //            {
                                            //                TotalOutstandingAmt = 0.0m;
                                            //            }

                                            //            var segmentType = HomeLoans.Select(it => it.SegmentType).FirstOrDefault();

                                            //            switch (segmentType.ToLower())
                                            //            {
                                            //                case HtmlConstants.MONTHLY_SEGMENT_FREQUENCY:
                                            //                    instalmentLabel = HtmlConstants.MONTHLY_INSTALMENT_LABEL;
                                            //                    break;
                                            //                case HtmlConstants.QUARTERLY_SEGMENT_FREQUENCY:
                                            //                    instalmentLabel = HtmlConstants.QUARTERLY_INSTALMENT_LABEL;
                                            //                    break;
                                            //                case HtmlConstants.ANNUAL_SEGMENT_FREQUENCY:
                                            //                    instalmentLabel = HtmlConstants.ANNUAL_INSTALMENT_LABEL;
                                            //                    break;
                                            //                default:
                                            //                    instalmentLabel = HtmlConstants.MONTHLY_INSTALMENT_LABEL;
                                            //                    break;
                                            //            }
                                            //        }

                                            //        widgetHtml.Replace("{{InstalmentType}}", instalmentLabel);
                                            //        widgetHtml.Replace("{{TotalHomeLoansAmount}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, TotalLoanAmt));
                                            //        widgetHtml.Replace("{{TotalHomeLoansBalanceOutstanding}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, TotalOutstandingAmt));
                                            //        htmlString.Append(widgetHtml.ToString());
                                            //    }
                                            //}
                                            //else if (mergedlst[i].WidgetName == HtmlConstants.WEALTH_HOME_LOAN_ACCOUNTS_BREAKDOWN_WIDGET_NAME)
                                            //{
                                            //    string jsonstr = HtmlConstants.HOME_LOAN_ACCOUNTS_PREVIEW_JSON_STRING;
                                            //    var widgetHtml = new StringBuilder(HtmlConstants.HOME_LOAN_ACCOUNTS_BREAKDOWN_HTML);
                                            //    if (jsonstr != string.Empty && validationEngine.IsValidJson(jsonstr))
                                            //    {
                                            //        var HomeLoans = JsonConvert.DeserializeObject<List<DM_HomeLoanMaster>>(jsonstr);
                                            //        if (HomeLoans != null && HomeLoans.Count> 0)
                                            //        {
                                            //            //create tab-content div if accounts is greater than 1, otherwise create simple div
                                            //            var TabContentHtml = new StringBuilder();
                                            //            var counter = 0;
                                            //            TabContentHtml.Append((HomeLoans.Count> 1) ? "<div class='tab-content'>" : string.Empty);
                                            //            HomeLoans.ForEach(HomeLoan =>
                                            //            {
                                            //                var accNo = HomeLoan.InvestorId.ToString();
                                            //                string lastFourDigisOfAccountNumber = accNo.Length> 4 ? accNo.Substring(Math.Max(0, accNo.Length - 4)) : accNo;

                                            //                TabContentHtml.Append("<div id='HomeLoan-" + lastFourDigisOfAccountNumber + "'>");

                                            //                var LoanDetailHtml = new StringBuilder(HtmlConstants.HOME_LOAN_WEALTH_ACCOUNT_DETAIL_DIV_HTML);
                                            //                LoanDetailHtml.Replace("{{BondNumber}}", accNo);
                                            //                LoanDetailHtml.Replace("{{RegistrationDate}}", HomeLoan.RegisteredDate.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy));

                                            //                var secDesc1 = string.Empty;
                                            //                var secDesc2 = string.Empty;
                                            //                var secDesc3 = string.Empty;
                                            //                if (HomeLoan.SecDescription1.Length> 15 || ((HomeLoan.SecDescription1 + " " + HomeLoan.SecDescription2).Length> 25))
                                            //                {
                                            //                    secDesc1 = HomeLoan.SecDescription1;
                                            //                    if ((HomeLoan.SecDescription2 + " " + HomeLoan.SecDescription3).Length> 25)
                                            //                    {
                                            //                        secDesc2 = HomeLoan.SecDescription2;
                                            //                        secDesc3 = HomeLoan.SecDescription3;
                                            //                    }
                                            //                    else
                                            //                    {
                                            //                        secDesc2 = HomeLoan.SecDescription2 + " " + HomeLoan.SecDescription3;
                                            //                    }
                                            //                }
                                            //                else
                                            //                {
                                            //                    secDesc1 = HomeLoan.SecDescription1 + " " + HomeLoan.SecDescription2;
                                            //                    secDesc2 = HomeLoan.SecDescription3;
                                            //                }

                                            //                LoanDetailHtml.Replace("{{SecDescription1}}", secDesc1);
                                            //                LoanDetailHtml.Replace("{{SecDescription2}}", secDesc2);
                                            //                LoanDetailHtml.Replace("{{SecDescription3}}", secDesc3);

                                            //                var res = 0.0m;
                                            //                if (decimal.TryParse(HomeLoan.IntialDue, out res))
                                            //                {
                                            //                    LoanDetailHtml.Replace("{{Instalment}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                                            //                }
                                            //                else
                                            //                {
                                            //                    LoanDetailHtml.Replace("{{Instalment}}", "R0.00");
                                            //                }

                                            //                LoanDetailHtml.Replace("{{InterestRate}}", HomeLoan.ChargeRate + "% pa");

                                            //                res = 0.0m;
                                            //                if (decimal.TryParse(HomeLoan.ArrearStatus, out res))
                                            //                {
                                            //                    LoanDetailHtml.Replace("{{Arrears}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                                            //                }
                                            //                else
                                            //                {
                                            //                    LoanDetailHtml.Replace("{{Arrears}}", "R0.00");
                                            //                }

                                            //                res = 0.0m;
                                            //                if (decimal.TryParse(HomeLoan.RegisteredAmount, out res))
                                            //                {
                                            //                    LoanDetailHtml.Replace("{{RegisteredAmount}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                                            //                }
                                            //                else
                                            //                {
                                            //                    LoanDetailHtml.Replace("{{RegisteredAmount}}", "R0.00");
                                            //                }

                                            //                LoanDetailHtml.Replace("{{LoanTerms}}", HomeLoan.LoanTerm);
                                            //                TabContentHtml.Append(LoanDetailHtml.ToString());

                                            //                var LoanTransactionRows = new StringBuilder();
                                            //                var LoanTransactionDetailHtml = new StringBuilder(HtmlConstants.HOME_LOAN_TRANSACTION_DETAIL_DIV_HTML);

                                            //                var tr = new StringBuilder();
                                            //                if (HomeLoan.LoanTransactions != null && HomeLoan.LoanTransactions.Count> 0)
                                            //                {
                                            //                    HomeLoan.LoanTransactions.ForEach(trans =>
                                            //                    {
                                            //                        tr = new StringBuilder();
                                            //                        tr.Append("<tr class='ht-20'>");
                                            //                        tr.Append("<td class='w-13 text-center'> " + trans.Posting_Date.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy) + " </td>");
                                            //                        tr.Append("<td class='w-15 text-center'> " + trans.Effective_Date.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy) + " </td>");
                                            //                        tr.Append("<td class='w-35'> " + (!string.IsNullOrEmpty(trans.Description) ? trans.Description : ModelConstant.PAYMENT_THANK_YOU_TRANSACTION_DESC) + " </td>");

                                            //                        res = 0.0m;
                                            //                        if (decimal.TryParse(trans.Debit, out res))
                                            //                        {
                                            //                            tr.Append("<td class='w-12 text-right'> " + (res> 0 ? utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res) : "-") + " </td>");
                                            //                        }
                                            //                        else
                                            //                        {
                                            //                            tr.Append("<td class='w-12 text-right'> - </td>");
                                            //                        }

                                            //                        res = 0.0m;
                                            //                        if (decimal.TryParse(trans.Credit, out res))
                                            //                        {
                                            //                            tr.Append("<td class='w-12 text-right'> " + (res> 0 ? utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res) : "-") + " </td>");
                                            //                        }
                                            //                        else
                                            //                        {
                                            //                            tr.Append("<td class='w-12 text-right'> - </td>");
                                            //                        }

                                            //                        res = 0.0m;
                                            //                        if (decimal.TryParse(trans.RunningBalance, out res))
                                            //                        {
                                            //                            tr.Append("<td class='w-13 text-right'> " + (res> 0 ? utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res) : "-") + " </td>");
                                            //                        }
                                            //                        else
                                            //                        {
                                            //                            tr.Append("<td class='w-13 text-right'> - </td>");
                                            //                        }
                                            //                        tr.Append("</tr>");

                                            //                        LoanTransactionRows.Append(tr.ToString());
                                            //                    });
                                            //                }
                                            //                LoanTransactionDetailHtml.Replace("{{HomeLoanTransactionRow}}", LoanTransactionRows.ToString());
                                            //                TabContentHtml.Append(LoanTransactionDetailHtml.ToString());

                                            //                var LoanArrearHtml = new StringBuilder(HtmlConstants.HOME_LOAN_WEALTH_STATEMENT_OVERVIEW_AND_PAYMENT_DUE_DIV_HTML);
                                            //                LoanArrearHtml.Replace("{{StatementDate}}", DateTime.Now.ToString(ModelConstant.DATE_FORMAT_dd_MMMM_yyyy));
                                            //                res = 0.0m;
                                            //                if (decimal.TryParse(HomeLoan.Balance, out res))
                                            //                {
                                            //                    LoanArrearHtml.Replace("{{BalanceOutstanding}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                                            //                }
                                            //                else
                                            //                {
                                            //                    LoanArrearHtml.Replace("{{BalanceOutstanding}}", "R0.00");
                                            //                }
                                            //                if (HomeLoan.LoanArrear != null)
                                            //                {
                                            //                    var plArrears = HomeLoan.LoanArrear;
                                            //                    res = 0.0m;
                                            //                    if (decimal.TryParse(plArrears.ARREARS_120, out res))
                                            //                    {
                                            //                        LoanArrearHtml.Replace("{{After120Days}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                                            //                    }
                                            //                    else
                                            //                    {
                                            //                        LoanArrearHtml.Replace("{{After120Days}}", "R0.00");
                                            //                    }

                                            //                    res = 0.0m;
                                            //                    if (decimal.TryParse(plArrears.ARREARS_90, out res))
                                            //                    {
                                            //                        LoanArrearHtml.Replace("{{After90Days}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                                            //                    }
                                            //                    else
                                            //                    {
                                            //                        LoanArrearHtml.Replace("{{After90Days}}", "R0.00");
                                            //                    }

                                            //                    res = 0.0m;
                                            //                    if (decimal.TryParse(plArrears.ARREARS_60, out res))
                                            //                    {
                                            //                        LoanArrearHtml.Replace("{{After60Days}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                                            //                    }
                                            //                    else
                                            //                    {
                                            //                        LoanArrearHtml.Replace("{{After60Days}}", "R0.00");
                                            //                    }

                                            //                    res = 0.0m;
                                            //                    if (decimal.TryParse(plArrears.ARREARS_30, out res))
                                            //                    {
                                            //                        LoanArrearHtml.Replace("{{After30Days}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                                            //                    }
                                            //                    else
                                            //                    {
                                            //                        LoanArrearHtml.Replace("{{After30Days}}", "R0.00");
                                            //                    }

                                            //                    res = 0.0m;
                                            //                    if (decimal.TryParse(plArrears.CurrentDue, out res))
                                            //                    {
                                            //                        LoanArrearHtml.Replace("{{CurrentPaymentDue}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                                            //                    }
                                            //                    else
                                            //                    {
                                            //                        LoanArrearHtml.Replace("{{CurrentPaymentDue}}", "R0.00");
                                            //                    }
                                            //                }
                                            //                else
                                            //                {
                                            //                    LoanArrearHtml.Replace("{{After120Days}}", "R0.00");
                                            //                    LoanArrearHtml.Replace("{{After90Days}}", "R0.00");
                                            //                    LoanArrearHtml.Replace("{{After60Days}}", "R0.00");
                                            //                    LoanArrearHtml.Replace("{{After30Days}}", "R0.00");
                                            //                    LoanArrearHtml.Replace("{{CurrentPaymentDue}}", "R0.00");
                                            //                }
                                            //                TabContentHtml.Append(LoanArrearHtml.ToString());

                                            //                var PaymentDueMessageDivHtml = new StringBuilder(HtmlConstants.HOME_LOAN_PAYMENT_DUE_SPECIAL_MESSAGE_DIV_HTML);
                                            //                var spjsonstr = HtmlConstants.HOME_LOAN_SPECIAL_MESSAGES_WIDGET_PREVIEW_JSON_STRING;
                                            //                if (spjsonstr != string.Empty && validationEngine.IsValidJson(spjsonstr))
                                            //                {
                                            //                    var SpecialMessage = JsonConvert.DeserializeObject<SpecialMessage>(spjsonstr);
                                            //                    if (SpecialMessage != null)
                                            //                    {
                                            //                        var PaymentDueMessage = (!string.IsNullOrEmpty(SpecialMessage.Message3) ? "<p> " + SpecialMessage.Message3 + " </p>" : string.Empty) + (!string.IsNullOrEmpty(SpecialMessage.Message4) ? "<p> " + SpecialMessage.Message4 + " </p>" : string.Empty) + (!string.IsNullOrEmpty(SpecialMessage.Message5) ? "<p> " + SpecialMessage.Message5 + " </p>" : string.Empty);

                                            //                        PaymentDueMessageDivHtml.Replace("{{PaymentDueSpecialMessage}}", PaymentDueMessage);
                                            //                        TabContentHtml.Append(PaymentDueMessageDivHtml.ToString());
                                            //                    }
                                            //                }

                                            //                TabContentHtml.Append(HtmlConstants.END_DIV_TAG);
                                            //                counter++;
                                            //            });

                                            //            TabContentHtml.Append((HomeLoans.Count> 1) ? HtmlConstants.END_DIV_TAG : string.Empty);
                                            //            widgetHtml.Replace("{{TabContentsDiv}}", TabContentHtml.ToString());
                                            //            htmlString.Append(widgetHtml.ToString());
                                            //        }
                                            //    }
                                            //}
                                            //else if (mergedlst[i].WidgetName == HtmlConstants.WEALTH_HOME_LOAN_SUMMARY_TAX_PURPOSE_WIDGET_NAME)
                                            //{
                                            //    string jsonstr = HtmlConstants.HOME_LOAN_SUMMARY_TAX_PURPOSE_PREVIEW_JSON_STRING;
                                            //    if (jsonstr != string.Empty && validationEngine.IsValidJson(jsonstr))
                                            //    {
                                            //        var summaryTax = JsonConvert.DeserializeObject<DM_HomeLoanSummary>(jsonstr);
                                            //        var htmlWidget = new StringBuilder(HtmlConstants.HOME_LOAN_WEALTH_SUMMARY_TAX_PURPOSE_HTML);
                                            //        htmlWidget.Replace("{{Interest}}", summaryTax.Annual_Interest);
                                            //        htmlWidget.Replace("{{Insurance}}", summaryTax.Annual_Insurance);
                                            //        htmlWidget.Replace("{{Servicefee}}", summaryTax.Annual_Service_Fee);
                                            //        htmlWidget.Replace("{{Legalcosts}}", summaryTax.Annual_Legal_Costs);
                                            //        htmlWidget.Replace("{{AmountReceived}}", summaryTax.Annual_Total_Recvd);
                                            //        htmlString.Append(htmlWidget);
                                            //    }
                                            //}
                                            //else if (mergedlst[i].WidgetName == HtmlConstants.WEALTH_HOME_LOAN_INSTALMENT_WIDGET_NAME)
                                            //{
                                            //    string jsonstr = HtmlConstants.HOME_LOAN_INSTALMENT_PREVIEW_JSON_STRING;
                                            //    if (jsonstr != string.Empty && validationEngine.IsValidJson(jsonstr))
                                            //    {
                                            //        var summaryTax = JsonConvert.DeserializeObject<DM_HomeLoanSummary>(jsonstr);
                                            //        var htmlWidget = new StringBuilder(HtmlConstants.HOME_LOAN_INSTALMENT_DETAILS_WIDGET_HTML);
                                            //        var htmlWidgetDetails = new StringBuilder(HtmlConstants.HOME_LOAN_WEALTH_INSTALMENT_DETAILS_HTML);
                                            //        var res = 0.0m;
                                            //        if (!string.IsNullOrEmpty(summaryTax.Basic_Instalment) && decimal.TryParse(summaryTax.Basic_Instalment, out res))
                                            //        {
                                            //            htmlWidgetDetails.Replace("{{BasicInstalment}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                                            //        }
                                            //        else
                                            //        {
                                            //            htmlWidgetDetails.Replace("{{BasicInstalment}}", "R0.00");
                                            //        }
                                            //        res = 0.0m;
                                            //        if (!string.IsNullOrEmpty(summaryTax.Houseowner_Ins) && decimal.TryParse(summaryTax.Houseowner_Ins, out res))
                                            //        {
                                            //            htmlWidgetDetails.Replace("{{HouseownerInsurance}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                                            //        }
                                            //        else
                                            //        {
                                            //            htmlWidgetDetails.Replace("{{HouseownerInsurance}}", "R0.00");
                                            //        }

                                            //        res = 0.0m;
                                            //        if (!string.IsNullOrEmpty(summaryTax.Loan_Protection) && decimal.TryParse(summaryTax.Loan_Protection, out res))
                                            //        {
                                            //            htmlWidgetDetails.Replace("{{LoanProtectionAssurance}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                                            //        }
                                            //        else
                                            //        {
                                            //            htmlWidgetDetails.Replace("{{LoanProtectionAssurance}}", "R0.00");
                                            //        }

                                            //        res = 0.0m;
                                            //        if (!string.IsNullOrEmpty(summaryTax.Recovery_Fee_Debit) && decimal.TryParse(summaryTax.Recovery_Fee_Debit, out res))
                                            //        {
                                            //            htmlWidgetDetails.Replace("{{RecoveryOfFeeDebits}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                                            //        }
                                            //        else
                                            //        {
                                            //            htmlWidgetDetails.Replace("{{RecoveryOfFeeDebits}}", "R0.00");
                                            //        }

                                            //        res = 0.0m;
                                            //        if (!string.IsNullOrEmpty(summaryTax.Capital_Redemption) && decimal.TryParse(summaryTax.Capital_Redemption, out res))
                                            //        {
                                            //            htmlWidgetDetails.Replace("{{CapitalRedemption}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                                            //        }
                                            //        else
                                            //        {
                                            //            htmlWidgetDetails.Replace("{{CapitalRedemption}}", "R0.00");
                                            //        }

                                            //        res = 0.0m;
                                            //        if (!string.IsNullOrEmpty(summaryTax.Service_Fee) && decimal.TryParse(summaryTax.Service_Fee, out res))
                                            //        {
                                            //            htmlWidgetDetails.Replace("{{ServiceFee}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                                            //        }
                                            //        else
                                            //        {
                                            //            htmlWidgetDetails.Replace("{{ServiceFee}}", "R0.00");
                                            //        }

                                            //        res = 0.0m;
                                            //        if (!string.IsNullOrEmpty(summaryTax.Total_Instalment) && decimal.TryParse(summaryTax.Total_Instalment, out res))
                                            //        {
                                            //            htmlWidgetDetails.Replace("{{TotalInstalment}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                                            //        }
                                            //        else
                                            //        {
                                            //            htmlWidgetDetails.Replace("{{TotalInstalment}}", "R0.00");
                                            //        }

                                            //        htmlWidgetDetails.Replace("{{InstalmentDate}}", DateTime.Now.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy));
                                            //        htmlWidget.Replace("{{Home_Loan_Instalment_Details}}", htmlWidgetDetails.ToString());
                                            //        htmlString.Append(htmlWidget);
                                            //    }
                                            //}
                                            //else if (mergedlst[i].WidgetName == HtmlConstants.WEALTH_HOME_LOAN_BRANCH_DETAILS_WIDGET_NAME)
                                            //{
                                            //    var htmlWidget = new StringBuilder(HtmlConstants.HOME_LOAN_WEALTH_BRANCH_DETAILS_WIDGET_HTML);
                                            //    string jsonstr = "{'BranchName': 'NEDBANK', 'AddressLine0':'Second Floor, Newtown Campus', 'AddressLine1':'141 Lilian Ngoyi Street, Newtown, Johannesburg 2001', 'AddressLine2':'PO Box 1144, Johannesburg, 2000','AddressLine3':'South Africa','VatRegNo':'4320116074','ContactNo':'0860 555 111'}";
                                            //    if (jsonstr != string.Empty && validationEngine.IsValidJson(jsonstr))
                                            //    {
                                            //        var branchDetails = JsonConvert.DeserializeObject<DM_BranchMaster>(jsonstr);
                                            //        htmlWidget.Replace("{{BankName}}", branchDetails.BranchName.ToUpper());
                                            //        htmlWidget.Replace("{{AddressLine0}}", branchDetails.AddressLine0.ToUpper());
                                            //        htmlWidget.Replace("{{AddressLine1}}", branchDetails.AddressLine1.ToUpper());
                                            //        htmlWidget.Replace("{{AddressLine2}}", branchDetails.AddressLine2.ToUpper());
                                            //        htmlWidget.Replace("{{AddressLine3}}", branchDetails.AddressLine3.ToUpper());
                                            //        htmlWidget.Replace("{{BankVATRegNo}}", "Bank VAT Reg No " + branchDetails.VatRegNo);
                                            //        htmlWidget.Replace("{{TodayDate}}", DateTime.Now.ToString(ModelConstant.DATE_FORMAT_yyyy_MM_dd));
                                            //        htmlWidget.Replace("{{ContactCenter}}", "Nedbank Private Wealth Service Suite: " + branchDetails.ContactNo);
                                            //        htmlString.Append(htmlWidget.ToString());
                                            //    }
                                            //}
                                            //else if (mergedlst[i].WidgetName == HtmlConstants.NEDBANK_PORTFOLIO_CUSTOMER_DETAILS_WIDGET_NAME)
                                            //{
                                            //    string jsonstr = "{'CustomerId': 171001255307, 'Title': 'MR.', 'FirstName':'MATHYS','SurName':'NKHUMISE','AddressLine0':'VERDEAU LIFESTYLE ESTATE', 'AddressLine1':'6 HERCULE CRESCENT DRIVE','AddressLine2':'WELLINGTON','AddressLine3':'7655','AddressLine4':'', 'Mask_Cell_No': '+2367 345 786', 'EmailAddress' : 'mknumise@domain.com', 'BARCODE': 'C:\\\\temp\\barcode.png'}";
                                            //    if (jsonstr != string.Empty && validationEngine.IsValidJson(jsonstr))
                                            //    {
                                            //        var customer = JsonConvert.DeserializeObject<DM_CustomerMaster>(jsonstr);
                                            //        var widgetHtml = new StringBuilder(HtmlConstants.NEDBANK_PORTFOLIO_CUSTOMER_DETAILS_WIDGET_HTML);
                                            //        widgetHtml.Replace("{{CustomerName}}", (customer.Title + " " + customer.FirstName + " " + customer.SurName));
                                            //        widgetHtml.Replace("{{CustomerId}}", customer.CustomerId.ToString());
                                            //        widgetHtml.Replace("{{MobileNumber}}", customer.Mask_Cell_No);
                                            //        widgetHtml.Replace("{{EmailAddress}}", customer.EmailAddress);
                                            //        htmlString.Append(widgetHtml.ToString());
                                            //    }
                                            //}
                                            //else if (mergedlst[i].WidgetName == HtmlConstants.NEDBANK_PORTFOLIO_CUSTOMER_ADDRESS_WIDGET_NAME)
                                            //{
                                            //    string jsonstr = "{'CustomerId': 171001255307, 'Title': 'MR.', 'FirstName':'MATHYS','SurName':'NKHUMISE','AddressLine0':'VERDEAU LIFESTYLE ESTATE', 'AddressLine1':'6 HERCULE CRESCENT DRIVE','AddressLine2':'WELLINGTON','AddressLine3':'7655','AddressLine4':'', 'Mask_Cell_No': '+2367 345 786', 'EmailAddress' : 'mknumise@domain.com', 'BARCODE': 'C:\\\\temp\\barcode.png'}";
                                            //    if (jsonstr != string.Empty && validationEngine.IsValidJson(jsonstr))
                                            //    {
                                            //        var customer = JsonConvert.DeserializeObject<DM_CustomerMaster>(jsonstr);
                                            //        var widgetHtml = new StringBuilder(HtmlConstants.NEDBANK_PORTFOLIO_CUSTOMER_ADDRESS_WIDGET_HTML);
                                            //        var custAddress = (!string.IsNullOrEmpty(customer.AddressLine0) ? (customer.AddressLine0 + "<br>") : string.Empty) +
                                            //        (!string.IsNullOrEmpty(customer.AddressLine1) ? (customer.AddressLine1 + "<br>") : string.Empty) +
                                            //        (!string.IsNullOrEmpty(customer.AddressLine2) ? (customer.AddressLine2 + "<br>") : string.Empty) +
                                            //        (!string.IsNullOrEmpty(customer.AddressLine3) ? (customer.AddressLine3 + "<br>") : string.Empty) +
                                            //        (!string.IsNullOrEmpty(customer.AddressLine4) ? customer.AddressLine4 : string.Empty);
                                            //        widgetHtml.Replace("{{CustomerAddress}}", custAddress);
                                            //        htmlString.Append(widgetHtml.ToString());
                                            //    }
                                            //}
                                            //else if (mergedlst[i].WidgetName == HtmlConstants.NEDBANK_PORTFOLIO_CLIENT_CONTACT_DETAILS_WIDGET_NAME)
                                            //{
                                            //    var widgetHtml = new StringBuilder(HtmlConstants.NEDBANK_PORTFOLIO_CLIENT_CONTACT_DETAILS_WIDGET_HTML);
                                            //    widgetHtml.Replace("{{MobileNumber}}", "0860 555 111");
                                            //    widgetHtml.Replace("{{EmailAddress}}", "supportdesk@nedbank.com");
                                            //    htmlString.Append(widgetHtml.ToString());
                                            //}
                                            //else if (mergedlst[i].WidgetName == HtmlConstants.NEDBANK_PORTFOLIO_ACCOUNT_SUMMARY_WIDGET_NAME)
                                            //{
                                            //    var widgetHtml = new StringBuilder(HtmlConstants.NEDBANK_PORTFOLIO_ACCOUNT_SUMMARY_WIDGET_HTML);
                                            //    string jsonstr = "[{'AccountType': 'Investment', 'TotalCurrentAmount': 'R9 620.98'},{'AccountType': 'Personal Loan', 'TotalCurrentAmount': 'R4 165.00'},{'AccountType': 'Home Loan', 'TotalCurrentAmount': 'R7 969.00'}]";
                                            //    if (jsonstr != string.Empty && validationEngine.IsValidJson(jsonstr))
                                            //    {
                                            //        var _lstAccounts = JsonConvert.DeserializeObject<List<DM_CustomerAccountSummary>>(jsonstr);
                                            //        if (_lstAccounts.Count> 0)
                                            //        {
                                            //            var accountSummaryRows = new StringBuilder();
                                            //            _lstAccounts.ForEach(acc =>
                                            //            {
                                            //                var tr = new StringBuilder();
                                            //                tr.Append("<tr class='ht-30'>");
                                            //                tr.Append("<td class='text-left'>" + acc.AccountType + " </td>");
                                            //                tr.Append("<td class='text-right'>" + acc.TotalCurrentAmount + " </td>");
                                            //                tr.Append("</tr>");
                                            //                accountSummaryRows.Append(tr.ToString());
                                            //            });
                                            //            widgetHtml.Replace("{{AccountSummaryRows}}", accountSummaryRows.ToString());
                                            //        }
                                            //        else
                                            //        {
                                            //            widgetHtml.Replace("{{AccountSummaryRows}}", "<tr class='ht-30'><td class='text-center' colspan='2'>No records found</td></tr>");
                                            //        }
                                            //    }

                                            //    var rewardPointsDiv = new StringBuilder("<div class='pt-2'><table class='LoanTransactionTable customTable'><thead><tr class='ht-30'><th class='text-left'>{{RewardType}} </th><th class='text-right'>{{RewardPoints}}</th></tr></thead></table></div>");
                                            //    rewardPointsDiv.Replace("{{RewardType}}", "Greenbacks rewards points");
                                            //    rewardPointsDiv.Replace("{{RewardPoints}}", "234");
                                            //    widgetHtml.Replace("{{RewardPointsDiv}}", rewardPointsDiv.ToString());

                                            //    htmlString.Append(widgetHtml.ToString());
                                            //}
                                            //else if (mergedlst[i].WidgetName == HtmlConstants.NEDBANK_PORTFOLIO_ACCOUNT_ANALYSIS_WIDGET_NAME)
                                            //{
                                            //    var widgetHtml = new StringBuilder(HtmlConstants.NEDBANK_PORTFOLIO_ACCOUNT_ANALYSIS_WIDGET_HTML);
                                            //    var data = "[{\"AccountType\": \"Investment\",\"MonthwiseAmount\" : [{\"Month\": \"Jan\", \"Amount\": 9456.12}, {\"Month\": \"Feb\", \"Amount\": 9620.98}]},{\"AccountType\": \"Personal Loan\",\"MonthwiseAmount\" : [{\"Month\": \"Jan\", \"Amount\": -4465.00}, {\"Month\": \"Feb\", \"Amount\": -4165.00}]},{\"AccountType\": \"Home Loan\",\"MonthwiseAmount\" : [{\"Month\": \"Jan\", \"Amount\": -8969.00}, {\"Month\": \"Feb\", \"Amount\": -7969.00}]}]";
                                            //    widgetHtml.Append("<input type='hidden' id='HiddenAccountAnalysisBarGraphData' value='" + data + "'/>");
                                            //    //widgetHtml.Append(HtmlConstants.PORTFOLIO_ACCOUNT_ANALYSIS_BAR_GRAPH_SCRIPT);
                                            //    htmlString.Append(widgetHtml.ToString());
                                            //}
                                            //else if (mergedlst[i].WidgetName == HtmlConstants.NEDBANK_PORTFOLIO_REMINDERS_WIDGET_NAME)
                                            //{
                                            //    var widgetHtml = new StringBuilder(HtmlConstants.NEDBANK_PORTFOLIO_REMINDER_AND_RECOMMENDATION_WIDGET_HTML);
                                            //    string jsonstr = "[{ 'Title': 'Update Missing Inofrmation', 'Action': 'Update' },{ 'Title': 'Your Rewards Video is available', 'Action': 'View' },{ 'Title': 'Payment Due for Home Loan', 'Action': 'Pay' }, { title: 'Need financial planning for savings.', action: 'Call Me' },{ title: 'Subscribe/Unsubscribe Alerts.', action: 'Apply' },{ title: 'Your credit card payment is due now.', action: 'Pay' }]";
                                            //    if (jsonstr != string.Empty && validationEngine.IsValidJson(jsonstr))
                                            //    {
                                            //        IList<ReminderAndRecommendation> reminderAndRecommendations = JsonConvert.DeserializeObject<List<ReminderAndRecommendation>>(jsonstr);
                                            //        StringBuilder reminderstr = new StringBuilder();
                                            //        reminderAndRecommendations.ToList().ForEach(item =>
                                            //        {
                                            //            reminderstr.Append("<div class='row'><div class='col-lg-9 text-left'><p class='p-1' style='background-color: #dce3dc;'>" + item.Title + " </p></div><div class='col-lg-3 text-left'><a href='javascript:void(0)' target='_blank'><i class='fa fa-caret-left fa-3x float-left text-success'></i><span class='mt-2 d-inline-block ml-2'>" + item.Action + "</span></a></div></div>");
                                            //        });
                                            //        widgetHtml.Replace("{{ReminderAndRecommendation}}", reminderstr.ToString());
                                            //        htmlString.Append(widgetHtml.ToString());
                                            //    }
                                            //}
                                            //else if (mergedlst[i].WidgetName == HtmlConstants.NEDBANK_PORTFOLIO_NEWS_ALERT_WIDGET_NAME)
                                            //{
                                            //    var widgetHtml = new StringBuilder(HtmlConstants.NEDBANK_PORTFOLIO_NEWS_ALERT_WIDGET_HTML);
                                            //    string jsonstr = "{ \"Message1\": \"Covid 19 and the subsequent lockdown has affected all areas of our daily lives. The way we work, the way we bank and how we interact with each other.\", \"Message2\": \"We want you to know we are in this together. That's why we are sharing advice, tips and news updates with you on ways to bank as well as ways to keep yorself and your loved ones safe.\", \"Message3\": \"We would like to remind you of the credit life insurance benefits available to you through your Nedbank Insurance policy. When you pass away, Nedbank Insurance will cover your outstanding loan amount. If you are permanently employed, you will also enjoy cover for comprehensive disability and loss of income. The disability benefit will cover your monthly instalments if you cannot earn your usual income due to illness or bodily injury.\", \"Message4\": \"\", \"Message5\": \"\" }";
                                            //    if (jsonstr != string.Empty && validationEngine.IsValidJson(jsonstr))
                                            //    {
                                            //        var newsAlert = JsonConvert.DeserializeObject<NewsAlert>(jsonstr);
                                            //        var newsAlertStr = (!string.IsNullOrEmpty(newsAlert.Message1) ? ("<p>" + newsAlert.Message1 + "</p>") : string.Empty) +
                                            //            (!string.IsNullOrEmpty(newsAlert.Message2) ? ("<p>" + newsAlert.Message2 + "</p>") : string.Empty) +
                                            //            (!string.IsNullOrEmpty(newsAlert.Message3) ? ("<p>" + newsAlert.Message3 + "</p>") : string.Empty) +
                                            //            (!string.IsNullOrEmpty(newsAlert.Message4) ? ("<p>" + newsAlert.Message4 + "</p>") : string.Empty) +
                                            //            (!string.IsNullOrEmpty(newsAlert.Message5) ? ("<p>" + newsAlert.Message5 + "</p>") : string.Empty);
                                            //        widgetHtml.Replace("{{NewsAlert}}", newsAlertStr);
                                            //        htmlString.Append(widgetHtml.ToString());
                                            //    }
                                            //}
                                            //else if (mergedlst[i].WidgetName == HtmlConstants.NEDBANK_GREENBACKS_TOTAL_REWARDS_POINTS_WIDGET_NAME)
                                            //{
                                            //    var widgetHtml = new StringBuilder(HtmlConstants.NEDBANK_GREENBACKS_TOTAL_REWARDS_POINTS_WIDGET_HTML);
                                            //    widgetHtml.Replace("{{TotalRewardsPoints}}", "482");
                                            //    htmlString.Append(widgetHtml.ToString());
                                            //}
                                            //else if (mergedlst[i].WidgetName == HtmlConstants.NEDBANK_GREENBACKS_CONTACT_US_WIDGET_NAME)
                                            //{
                                            //    htmlString.Append(HtmlConstants.NEDBANK_GREENBACKS_CONTACT_US_WIDGET_HTML);
                                            //}
                                            //else if (mergedlst[i].WidgetName == HtmlConstants.NEDBANK_YTD_REWARDS_POINTS_BAR_GRAPH_WIDGET_NAME)
                                            //{
                                            //    var widgetHtml = new StringBuilder(HtmlConstants.NEDBANK_YTD_REWARDS_POINTS_BAR_GRAPH_WIDGET_HTML);
                                            //    var data = "[{\"Month\": \"Jan\",\"RewardPoint\" : 98}, {\"Month\": \"Feb\",\"RewardPoint\" : 112}, {\"Month\": \"Mar\",\"RewardPoint\" : 128}, {\"Month\": \"Apr\",\"RewardPoint\" : 144}]";
                                            //    widgetHtml.Append("<input type='hidden' id='HiddenYTDRewardPointsBarGraphData' value='" + data + "'/>");
                                            //    //widgetHtml.Append(HtmlConstants.PORTFOLIO_ACCOUNT_ANALYSIS_BAR_GRAPH_SCRIPT);
                                            //    htmlString.Append(widgetHtml.ToString());
                                            //}
                                            //else if (mergedlst[i].WidgetName == HtmlConstants.NEDBANK_POINTS_REDEEMED_YTD_BAR_GRAPH_WIDGET_NAME)
                                            //{
                                            //    var widgetHtml = new StringBuilder(HtmlConstants.NEDBANK_POINTS_REDEEMED_YTD_BAR_GRAPH_WIDGET_HTML);
                                            //    var data = "[{\"Month\": \"Jan\",\"RedeemedPoints\" : 58}, {\"Month\": \"Feb\",\"RedeemedPoints\" : 71}, {\"Month\": \"Mar\",\"RedeemedPoints\" : 63}, {\"Month\": \"Apr\",\"RedeemedPoints\" : 84}]";
                                            //    widgetHtml.Append("<input type='hidden' id='HiddenPointsRedeemedBarGraphData' value='" + data + "'/>");
                                            //    //widgetHtml.Append(HtmlConstants.PORTFOLIO_ACCOUNT_ANALYSIS_BAR_GRAPH_SCRIPT);
                                            //    htmlString.Append(widgetHtml.ToString());
                                            //}
                                            //else if (mergedlst[i].WidgetName == HtmlConstants.NEDBANK_PRODUCT_RELATED_POINTS_EARNED_BAR_GRAPH_WIDGET_NAME)
                                            //{
                                            //    var widgetHtml = new StringBuilder(HtmlConstants.NEDBANK_PRODUCT_RELATED_POINTS_EARNED_BAR_GRAPH_WIDGET_HTML);
                                            //    var data = "[{\"AccountType\": \"Investment\",\"MonthwiseAmount\" : [{\"Month\": \"Jan\", \"RewardPoint\": 34}, {\"Month\": \"Feb\", \"RewardPoint\": 29},{\"Month\": \"Mar\", \"RewardPoint\": 41}, {\"Month\": \"Apr\", \"RewardPoint\": 48}]}, {\"AccountType\": \"Personal Loan\",\"MonthwiseAmount\" : [{\"Month\": \"Jan\", \"RewardPoint\": 27}, {\"Month\": \"Feb\", \"RewardPoint\": 45},{\"Month\": \"Mar\", \"RewardPoint\": 36}, {\"Month\": \"Apr\", \"RewardPoint\": 51}]}, {\"AccountType\": \"Home Loan\",\"MonthwiseAmount\" : [{\"Month\": \"Jan\", \"RewardPoint\": 37}, {\"Month\": \"Feb\", \"RewardPoint\": 38},{\"Month\": \"Mar\", \"RewardPoint\": 51}, {\"Month\": \"Apr\", \"RewardPoint\": 45}]}]"; ;
                                            //    widgetHtml.Append("<input type='hidden' id='HiddenProductRelatedPointsEarnedBarGraphData' value='" + data + "'/>");
                                            //    //widgetHtml.Append(HtmlConstants.PORTFOLIO_ACCOUNT_ANALYSIS_BAR_GRAPH_SCRIPT);
                                            //    htmlString.Append(widgetHtml.ToString());
                                            //}
                                            //else if (mergedlst[i].WidgetName == HtmlConstants.NEDBANK_CATEGORY_SPEND_REWARDS_PIE_CHART_WIDGET_NAME)
                                            //{
                                            //    var widgetHtml = new StringBuilder(HtmlConstants.NEDBANK_CATEGORY_SPEND_REWARDS_PIE_CHART_WIDGET_HTML);
                                            //    var data = "[{\"Category\": \"Fuel\",\"SpendReward\" : 34}, {\"Category\": \"Groceries\",\"SpendReward\" : 15}, {\"Category\": \"Travel\",\"SpendReward\" : 21}, {\"Category\": \"Movies\",\"SpendReward\" : 19}, {\"Category\": \"Shopping\",\"SpendReward\" : 11}]"; ;
                                            //    widgetHtml.Append("<input type='hidden' id='HiddenCategorySpendRewardsPieChartData' value='" + data + "'/>");
                                            //    //widgetHtml.Append(HtmlConstants.PORTFOLIO_ACCOUNT_ANALYSIS_BAR_GRAPH_SCRIPT);
                                            //    htmlString.Append(widgetHtml.ToString());
                                            //}
                                            //else if (mergedlst[i].WidgetName == HtmlConstants.NEDBANK_MCA_TRANSACTION_WIDGET_NAME)
                                            //{
                                            //    string jsonstr = HtmlConstants.MCA_TRANSACTION_PREVIEW_JSON_STRING;
                                            //    var widgetHtml = new StringBuilder(HtmlConstants.MCA_TRANSACTION_DETAIL_DIV_HTML);
                                            //    if (jsonstr != string.Empty && validationEngine.IsValidJson(jsonstr))
                                            //    {
                                            //        var mcaTransaction = JsonConvert.DeserializeObject<List<DM_MCATransaction>>(jsonstr);
                                            //        StringBuilder rowsHTML = new StringBuilder();
                                            //        var res = 0.0m;
                                            //        mcaTransaction.ForEach(trans =>
                                            //        {
                                            //            res = 0.0m;
                                            //            string debit = string.Empty;
                                            //            string credit = string.Empty;
                                            //            if (trans.Debit != null && decimal.TryParse(trans.Debit.ToString(), out res))
                                            //            {
                                            //                debit = utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res);
                                            //            }
                                            //            else
                                            //            {
                                            //                debit = "";
                                            //            }

                                            //            if (trans.Credit != null && decimal.TryParse(trans.Credit.ToString(), out res))
                                            //            {
                                            //                credit = utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res);
                                            //            }
                                            //            else
                                            //            {
                                            //                credit = "";
                                            //            }

                                            //            rowsHTML.Append("<tr class='ht-20'>" +
                                            //                "<td class='w-15 text-center'>" + trans.Transaction_Date.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy) + "</td>" +
                                            //                "<td class='w-35 text-left'>" + trans.Description + "</td>" +
                                            //                "<td class='w-12 text-right'>" + debit + "</td>" +
                                            //                "<td class='w-12 text-right'>" + credit + "</td>" +
                                            //                "<td class='w-7 text-center'>" + trans.Rate + "</td>" +
                                            //                "<td class='w-7 text-center'>" + trans.Days + "</td>" +
                                            //                "<td class='w-12 text-right'>" + trans.AccuredInterest + "</td>" +
                                            //                "</tr>"
                                            //                );
                                            //        });
                                            //        widgetHtml.Replace("{{MCATransactionRow}}", rowsHTML.ToString());
                                            //        htmlString.Append(widgetHtml);
                                            //    }
                                            //}
                                            //else if (mergedlst[i].WidgetName == HtmlConstants.NEDBANK_MCA_VAT_ANALYSIS_WIDGET_NAME)
                                            //{
                                            //    string jsonstr = HtmlConstants.MCA_TRANSACTION_PREVIEW_JSON_STRING;
                                            //    var widgetHtml = new StringBuilder(HtmlConstants.MCA_VAT_ANALYSIS_DETAIL_DIV_HTML);
                                            //    if (jsonstr != string.Empty && validationEngine.IsValidJson(jsonstr))
                                            //    {
                                            //        var mcaTransaction = JsonConvert.DeserializeObject<List<DM_MCATransaction>>(jsonstr);
                                            //        StringBuilder rowsHTML = new StringBuilder();
                                            //        if (mcaTransaction.Count> 0)
                                            //        {
                                            //            var trans = mcaTransaction[0];
                                            //            rowsHTML.Append("<tr class='ht-20'>" +
                                            //                "<td class='w-25 text-left'>" + DateTime.Now.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy) + "</td>" +
                                            //                "<td class='w-25 text-right'>" + DateTime.Now.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy) + "</td>" +
                                            //                "<td class='w-25 text-center'>" + trans.Rate + "</td>" +
                                            //                "<td class='w-25 text-center'>" + trans.Credit + "</td>" +
                                            //                "</tr>"
                                            //                );
                                            //        }
                                            //        widgetHtml.Replace("{{MCAVATTable}}", rowsHTML.ToString());
                                            //        htmlString.Append(widgetHtml);
                                            //    }
                                            //}
                                            //else if (mergedlst[i].WidgetName == HtmlConstants.NEDBANK_MCA_ACCOUNT_SUMMARY_WIDGET_NAME)
                                            //{
                                            //    string jsonstr = HtmlConstants.MCA_ACCOUNT_SUMMARY_PREVIEW_JSON_STRING;
                                            //    if (jsonstr != string.Empty && validationEngine.IsValidJson(jsonstr))
                                            //    {
                                            //        var mcaMaster = JsonConvert.DeserializeObject<DM_MCAMaster>(jsonstr);
                                            //        var htmlWidget = new StringBuilder(HtmlConstants.MCA_ACCOUNT_SUMMARY_DETAILS_WIDGET_HTML);
                                            //        htmlWidget.Replace("{{AccountNo}}", mcaMaster.CustomerId.ToString());
                                            //        htmlWidget.Replace("{{StatementNo}}", mcaMaster.StatementNo);
                                            //        htmlWidget.Replace("{{OverdraftLimit}}", mcaMaster.OverdraftLimit != null ? mcaMaster.OverdraftLimit.ToString() : "0.00");
                                            //        htmlWidget.Replace("{{StatementDate}}", mcaMaster.StatementDate != null ? mcaMaster.StatementDate.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy) : DateTime.Now.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy));
                                            //        htmlWidget.Replace("{{Currency}}", mcaMaster.Currency);
                                            //        htmlWidget.Replace("{{Statementfrequency}}", mcaMaster.StatementFrequency);
                                            //        htmlWidget.Replace("{{FreeBalance}}", mcaMaster.FreeBalance != null ? mcaMaster.FreeBalance.ToString() : "0.00");
                                            //        htmlString.Append(htmlWidget);
                                            //    }
                                            //}
                                            //else if (mergedlst[i].WidgetName == HtmlConstants.NEDBANK_WEALTH_MCA_TRANSACTION_WIDGET_NAME)
                                            //{
                                            //    string jsonstr = HtmlConstants.MCA_TRANSACTION_PREVIEW_JSON_STRING;
                                            //    var widgetHtml = new StringBuilder(HtmlConstants.MCA_WEALTH_TRANSACTION_DETAIL_DIV_HTML);
                                            //    if (jsonstr != string.Empty && validationEngine.IsValidJson(jsonstr))
                                            //    {
                                            //        var mcaTransaction = JsonConvert.DeserializeObject<List<DM_MCATransaction>>(jsonstr);
                                            //        StringBuilder rowsHTML = new StringBuilder();
                                            //        var res = 0.0m;
                                            //        mcaTransaction.ForEach(trans =>
                                            //        {
                                            //            res = 0.0m;
                                            //            string debit = string.Empty;
                                            //            string credit = string.Empty;
                                            //            if (trans.Debit != null && decimal.TryParse(trans.Debit.ToString(), out res))
                                            //            {
                                            //                debit = utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res);
                                            //            }
                                            //            else
                                            //            {
                                            //                debit = "";
                                            //            }

                                            //            if (trans.Credit != null && decimal.TryParse(trans.Credit.ToString(), out res))
                                            //            {
                                            //                credit = utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res);
                                            //            }
                                            //            else
                                            //            {
                                            //                credit = "";
                                            //            }

                                            //            rowsHTML.Append("<tr class='ht-20'>" +
                                            //                "<td class='w-15 text-center'>" + trans.Transaction_Date.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy) + "</td>" +
                                            //                "<td class='w-35 text-left'>" + trans.Description + "</td>" +
                                            //                "<td class='w-12 text-right'>" + debit + "</td>" +
                                            //                "<td class='w-12 text-right'>" + credit + "</td>" +
                                            //                "<td class='w-7 text-center'>" + trans.Rate + "</td>" +
                                            //                "<td class='w-7 text-center'>" + trans.Days + "</td>" +
                                            //                "<td class='w-12 text-right'>" + trans.AccuredInterest + "</td>" +
                                            //                "</tr>"
                                            //                );
                                            //        });
                                            //        widgetHtml.Replace("{{MCATransactionRow}}", rowsHTML.ToString());
                                            //        htmlString.Append(widgetHtml);
                                            //    }
                                            //}
                                            //else if (mergedlst[i].WidgetName == HtmlConstants.NEDBANK_WEALTH_MCA_VAT_ANALYSIS_WIDGET_NAME)
                                            //{
                                            //    string jsonstr = HtmlConstants.MCA_TRANSACTION_PREVIEW_JSON_STRING;
                                            //    var widgetHtml = new StringBuilder(HtmlConstants.MCA_WEALTH_VAT_ANALYSIS_DETAIL_DIV_HTML);
                                            //    if (jsonstr != string.Empty && validationEngine.IsValidJson(jsonstr))
                                            //    {
                                            //        var mcaTransaction = JsonConvert.DeserializeObject<List<DM_MCATransaction>>(jsonstr);
                                            //        StringBuilder rowsHTML = new StringBuilder();
                                            //        if (mcaTransaction.Count> 0)
                                            //        {
                                            //            var trans = mcaTransaction[0];
                                            //            rowsHTML.Append("<tr class='ht-20'>" +
                                            //                "<td class='w-25 text-left'>" + DateTime.Now.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy) + "</td>" +
                                            //                "<td class='w-25 text-right'>" + DateTime.Now.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy) + "</td>" +
                                            //                "<td class='w-25 text-center'>" + trans.Rate + "</td>" +
                                            //                "<td class='w-25 text-center'>" + trans.Credit + "</td>" +
                                            //                "</tr>"
                                            //                );
                                            //        }
                                            //        widgetHtml.Replace("{{MCAVATTable}}", rowsHTML.ToString());
                                            //        htmlString.Append(widgetHtml);
                                            //    }
                                            //}
                                            //else if (mergedlst[i].WidgetName == HtmlConstants.NEDBANK_WEALTH_MCA_ACCOUNT_SUMMARY_WIDGET_NAME)
                                            //{
                                            //    string jsonstr = HtmlConstants.MCA_ACCOUNT_SUMMARY_PREVIEW_JSON_STRING;
                                            //    if (jsonstr != string.Empty && validationEngine.IsValidJson(jsonstr))
                                            //    {
                                            //        var mcaMaster = JsonConvert.DeserializeObject<DM_MCAMaster>(jsonstr);
                                            //        var htmlWidget = new StringBuilder(HtmlConstants.MCA_WEALTH_ACCOUNT_SUMMARY_DETAILS_WIDGET_HTML);
                                            //        htmlWidget.Replace("{{AccountNo}}", mcaMaster.CustomerId.ToString());
                                            //        htmlWidget.Replace("{{StatementNo}}", mcaMaster.StatementNo);
                                            //        htmlWidget.Replace("{{OverdraftLimit}}", mcaMaster.OverdraftLimit != null ? mcaMaster.OverdraftLimit.ToString() : "0.00");
                                            //        htmlWidget.Replace("{{StatementDate}}", mcaMaster.StatementDate != null ? mcaMaster.StatementDate.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy) : DateTime.Now.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy));
                                            //        htmlWidget.Replace("{{Currency}}", mcaMaster.Currency);
                                            //        htmlWidget.Replace("{{Statementfrequency}}", mcaMaster.StatementFrequency);
                                            //        htmlWidget.Replace("{{FreeBalance}}", mcaMaster.FreeBalance != null ? mcaMaster.FreeBalance.ToString() : "0.00");
                                            //        htmlString.Append(htmlWidget);
                                            //    }
                                            //}
                                            //else if (mergedlst[i].WidgetName == HtmlConstants.NEDBANK_WEALTH_MCA_BRANCH_DETAILS_WIDGET_NAME)
                                            //{
                                            //    var htmlWidget = new StringBuilder(HtmlConstants.MCA_WEALTH_BRANCH_DETAILS_WIDGET_HTML);
                                            //    StringBuilder htmlBankDetails = new StringBuilder();
                                            //    htmlBankDetails.Append(HtmlConstants.BANK_DETAILS);
                                            //    htmlBankDetails.Replace("{{TodayDate}}", DateTime.Now.ToString(ModelConstant.DATE_FORMAT_yyyy_MM_dd));

                                            //    htmlWidget.Replace("{{BranchDetails}}", htmlBankDetails.ToString());
                                            //    htmlWidget.Replace("{{ContactCenter}}", HtmlConstants.WEA_BANKING);
                                            //    htmlString.Append(htmlWidget.ToString());
                                            //}
                                            //else if (mergedlst[i].WidgetName == HtmlConstants.CORPORATESAVER_AGENT_MESSAGE_WIDGET_NAME)
                                            //{
                                            //    //string jsonstr = HtmlConstants.CORPORATESAVER_TRANSACTION_PREVIEW_JSON_STRING;
                                            //    if (true)//jsonstr != string.Empty && validationEngine.IsValidJson(jsonstr))
                                            //    {
                                            //        //var corporateSaverMaster = JsonConvert.DeserializeObject<DM_CorporateSaverMaster>(jsonstr);

                                            //        var htmlWidget = new StringBuilder(HtmlConstants.CORPORATESAVER_AGENT_MESSAGE_HTML);
                                            //        //htmlWidget.Replace("{{AccountNo}}", "xxxxxxx");
                                            //        // htmlWidget.Replace("{{StatementNo}}", "xxxxxxx");
                                            //        // htmlWidget.Replace("{{OverdraftLimit}}", "xxxxxxx");
                                            //        // htmlWidget.Replace("{{StatementDate}}", "xxxxxxx");
                                            //        // htmlWidget.Replace("{{Currency}}", "xxxxxxx");
                                            //        // htmlWidget.Replace("{{Statementfrequency}}", "xxxxxxx");
                                            //        // htmlWidget.Replace("{{FreeBalance}}", "xxxxxxx");
                                            //        htmlString.Append(htmlWidget);
                                            //    }
                                            //}
                                            //else if (mergedlst[i].WidgetName == HtmlConstants.NEDBANK_CORPORATESAVER_CLIENTANDAGENT_DETAILS_NAME)
                                            //{
                                            //    //string jsonstr = HtmlConstants.CORPORATESAVER_TRANSACTION_PREVIEW_JSON_STRING;
                                            //    var pageContent = new StringBuilder(HtmlConstants.CORPORATESAVER_CLIENT_DETAILS_HTML);
                                            //    pageContent.Replace("{{TaxInvoiceNo}}", "3563136");
                                            //    pageContent.Replace("{{ContactPerson}}", "Louise Taylor");
                                            //    pageContent.Replace("{{EmailAddress}}", "louise@robchap.co.za");
                                            //    pageContent.Replace("{{RegNo}}", "1986/012848/23");
                                            //    pageContent.Replace("{{VATRegNo}}", "4900153653");
                                            //    pageContent.Replace("{{FSPLicNo}}", "16616");
                                            //    pageContent.Replace("{{AgentRefNo}}", "WALLI");
                                            //    pageContent.Replace("{{StatementNo}}", "184");
                                            //    pageContent.Replace("{{AccountNo}}", "9000082385");
                                            //    pageContent.Replace("{{Branchcode}}", "198765");
                                            //    pageContent.Replace("{{Agentprofile}}", "PRO315");
                                            //    pageContent.Replace("{{CIFNo}}", "5786407");
                                            //    pageContent.Replace("{{ClientCode}}", "292598");
                                            //    pageContent.Replace("{{RelationshipManager}}", "Umhlali Agencies CC");
                                            //    pageContent.Replace("{{VATCalculation}}", "VAT inclusive");
                                            //    pageContent.Replace("{{ClientVATNo}}", "Not provided");
                                            //    htmlString.Append(pageContent);
                                            //}
                                            //else if (mergedlst[i].WidgetName == HtmlConstants.CORPORATESAVER_AGENT_ADDRESS_NAME)
                                            //{
                                            //    //string jsonstr = HtmlConstants.CORPORATESAVER_TRANSACTION_PREVIEW_JSON_STRING;
                                            //    var pageContent = new StringBuilder(HtmlConstants.CORPORATESAVER_AGENT_ADDRESS_HTML);
                                            //    var AgentAddress = "Address Line 1<br>" + "Address Line 2<br>" + "Address Line 3<br>" + "Address Line 4<br>" + "Address Line 5<br>";

                                            //    pageContent.Replace("{{AgentAddress}}", AgentAddress);
                                            //    pageContent.Replace("{{AgentContact}}", "1234567890");
                                            //    htmlString.Append(pageContent);
                                            //}

                                            //else if (mergedlst[i].WidgetName == HtmlConstants.NETBANK_CORPORATESAVER_AGENTDETAILS_NAME)
                                            //{
                                            //    string jsonstr = HtmlConstants.CORPORATESAVER_TRANSACTION_PREVIEW_JSON_STRING;
                                            //    var widgetHtml = new StringBuilder(HtmlConstants.NETBANK_CORPORATESAVER_AGENTDETAILS_HTML);
                                            //    if (jsonstr != string.Empty && validationEngine.IsValidJson(jsonstr))
                                            //    {
                                            //        var mcaTransaction = JsonConvert.DeserializeObject<List<DM_MCATransaction>>(jsonstr);
                                            //        StringBuilder rowsHTML = new StringBuilder();
                                            //        if (mcaTransaction.Count> 0)
                                            //        {
                                            //            var trans = mcaTransaction[0];
                                            //            rowsHTML.Append("<tr class='ht-20'>" +
                                            //                "<td class='w-25 text-left'>" + DateTime.Now.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy) + "</td>" +
                                            //                "<td class='w-25 text-right'>" + DateTime.Now.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy) + "</td>" +
                                            //                "<td class='w-25 text-center'>" + trans.Rate + "</td>" +
                                            //                "<td class='w-25 text-center'>" + trans.Credit + "</td>" +
                                            //                "</tr>"
                                            //                );
                                            //        }
                                            //        widgetHtml.Replace("{{MCAVATTable}}", rowsHTML.ToString());
                                            //        htmlString.Append(widgetHtml);
                                            //    }
                                            //}                     
                                            //else if (mergedlst[i].WidgetName == HtmlConstants.NEDBANK_CORPORATESAVER_TRANSACTION_WIDGET_NAME)
                                            //{
                                            //    //string jsonstr = HtmlConstants.MCA_TRANSACTION_PREVIEW_JSON_STRING;
                                            //    var pageContent = new StringBuilder(HtmlConstants.NEDBANK_CORPORATESAVER_TRANSACTION_WIDGET_HTML);
                                            //    if (true)//jsonstr != string.Empty && validationEngine.IsValidJson(jsonstr))
                                            //    {
                                            //        //var mcaTransaction = JsonConvert.DeserializeObject<List<DM_MCATransaction>>(jsonstr);
                                            //        //StringBuilder rowsHTML = new StringBuilder();
                                            //        //var res = 0.0m;
                                            //        //mcaTransaction.ForEach(trans =>
                                            //        //{
                                            //        //    res = 0.0m;
                                            //        //    string debit = string.Empty;
                                            //        //    string credit = string.Empty;
                                            //        //    if (trans.Debit != null && decimal.TryParse(trans.Debit.ToString(), out res))
                                            //        //    {
                                            //        //        debit = utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res);
                                            //        //    }
                                            //        //    else
                                            //        //    {
                                            //        //        debit = "";
                                            //        //    }

                                            //        //    if (trans.Credit != null && decimal.TryParse(trans.Credit.ToString(), out res))
                                            //        //    {
                                            //        //        credit = utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res);
                                            //        //    }
                                            //        //    else
                                            //        //    {
                                            //        //        credit = "";
                                            //        //    }

                                            //        //    rowsHTML.Append("<tr class='ht-20'>" +
                                            //        //        "<td class='w-15 text-center'>" + trans.Transaction_Date.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy) + "</td>" +
                                            //        //        "<td class='w-35 text-left'>" + trans.Description + "</td>" +
                                            //        //        "<td class='w-12 text-right'>" + debit + "</td>" +
                                            //        //        "<td class='w-12 text-right'>" + credit + "</td>" +
                                            //        //        "<td class='w-7 text-center'>" + trans.Rate + "</td>" +
                                            //        //        "<td class='w-7 text-center'>" + trans.Days + "</td>" +
                                            //        //        "<td class='w-12 text-right'>" + trans.AccuredInterest + "</td>" +
                                            //        //        "</tr>"
                                            //        //        );
                                            //        //});
                                            //        //widgetHtml.Replace("{{MCATransactionRow}}", rowsHTML.ToString());
                                            //        //htmlString.Append(widgetHtml);
                                            //        StringBuilder tableHTML = new StringBuilder();

                                            //        tableHTML.Append("<tr class='ht-20'>");
                                            //        tableHTML.Append("<td class='w-12 text-center'>01/02/2022</td>");
                                            //        tableHTML.Append("<td class='text-center'  style='width: 25%'></td>");
                                            //        tableHTML.Append("<td class='text-center'  style='width: 25%'>	Balance brought forward</td>");
                                            //        tableHTML.Append("<td class='w-15 text-center'> </td>");
                                            //        tableHTML.Append("<td class='text-center'  style='width: 25%'>2.30%	</td>");
                                            //        tableHTML.Append("<td class='text-center'  style='width: 25%'>R100602.32</td>");
                                            //        tableHTML.Append("</tr>");

                                            //        tableHTML.Append("<tr class='ht-20'>");
                                            //        tableHTML.Append("<td class='w-12 text-center'>01/02/2022</td>");
                                            //        tableHTML.Append("<td class='text-center'  style='width: 25%'></td>");
                                            //        tableHTML.Append("<td class='text-center'  style='width: 25%'>January interest paid</td>");
                                            //        tableHTML.Append("<td class='w-15 text-center'> R202.20</td>");
                                            //        tableHTML.Append("<td class='text-center'  style='width: 25%'></td>");
                                            //        tableHTML.Append("<td class='text-center'  style='width: 25%'>R100804.52</td>");
                                            //        tableHTML.Append("</tr>");
                                            //        tableHTML.Append("<tr class='ht-20'>");
                                            //        tableHTML.Append("<td class='w-12 text-center'>28/02/2022</td>");
                                            //        tableHTML.Append("<td class='text-center'  style='width: 25%'></td>");
                                            //        tableHTML.Append("<td class='text-center'  style='width: 25%'>Balance carried forward</td>");
                                            //        tableHTML.Append("<td class='w-15 text-center'> </td>");
                                            //        tableHTML.Append("<td class='text-center'  style='width: 25%'>2.30%	</td>");
                                            //        tableHTML.Append("<td class='text-center'  style='width: 25%'>R89030.44</ td>");
                                            //        tableHTML.Append("</tr>");

                                            //        pageContent.Replace("{{CorporateSaverTransactions}}", tableHTML.ToString());
                                            //        pageContent.Replace("{{FromDate}}", "01/02/2022");
                                            //        pageContent.Replace("{{ToDate}}", "28/02/2022");
                                            //        htmlString.Append(pageContent.ToString());
                                            //    }
                                            //}
                                            //else if (mergedlst[i].WidgetName == HtmlConstants.NETBANK_CORPORATESAVER_AGENTDETAILS_NAME)
                                            //{
                                            //    //string jsonstr = HtmlConstants.CORPORATESAVER_TRANSACTION_PREVIEW_JSON_STRING;
                                            //    var pageContent = new StringBuilder(HtmlConstants.NETBANK_CORPORATESAVER_AGENTDETAILS_HTML);

                                            //    pageContent.Replace("{{Interest}}", "R1 397.51");

                                            //    pageContent.Replace("{{VATonfee}}", "R84.33");
                                            //    pageContent.Replace("{{Agentfeededucted}}", "R562.14");
                                            //    pageContent.Replace("min-height: 66px;", "min-height: 74px;");


                                            //    htmlString.Append(pageContent);

                                            //}
                                            //else if (mergedlst[i].WidgetName == HtmlConstants.NEDBANK_CORPORATESAVER_LASTTOTAL_NAME)
                                            //{
                                            //    var htmlWidget = new StringBuilder(HtmlConstants.CORPORATESAVER_LASTTOTAL_HTML);
                                            //    //StringBuilder htmlBankDetails = new StringBuilder();
                                            //    //htmlBankDetails.Append(HtmlConstants.BANK_DETAILS);
                                            //    //htmlBankDetails.Replace("{{TodayDate}}", DateTime.Now.ToString(ModelConstant.DATE_FORMAT_yyyy_MM_dd));

                                            //    //htmlWidget.Replace("{{BranchDetails}}", htmlBankDetails.ToString());
                                            //    //htmlWidget.Replace("{{ContactCenter}}", HtmlConstants.WEA_BANKING);
                                            //    //htmlString.Append(htmlWidget.ToString());
                                            //    StringBuilder tableHTML = new StringBuilder();
                                            //    StringBuilder tableHTML2 = new StringBuilder();

                                            //    tableHTML.Append("<div class='CSTotalAmountDetailsDiv' style='height: 40px !important; text-align: center; padding: 6px !important;'><span class='fnt-14pt'> Current investment details </span></div>");


                                            //    tableHTML.Append("<table class= 'CScustomTable HomeLoanDetailDiv' border = '0' style = 'height: auto;margin-bottom:2%;'><tbody>");
                                            //    tableHTML.Append("<tr><td colspan='2' class='w-25' style='font-weight: bold;padding-bottom: 8px !important;padding-top: 8px !important;'>Call account @ 2,30% per annum</td></tr>");
                                            //    tableHTML.Append("<tr><td class='w-25' style='padding-bottom: 8px !important'>Interest instruction</td><td class='w-25 text-right' style='padding-bottom: 8px !important;padding-right: 15px;'>Capitalised</td><td class='w-25' style='padding-bottom: 8px !important'>Date Invested</td><td class='w-25 text-right pr-1' style='padding-bottom: 8px !important'>08-10-2006</td></tr>");
                                            //    tableHTML.Append("<tr><td class='w-25' style='padding-bottom: 8px !important'>Capital</td><td class='w-25 text-right' style='padding-bottom: 8px !important;padding-right: 15px;'>R89 030.44 </td><td class='w-25' style='padding-bottom: 8px !important'>Agent fee deducted</td><td class='w-25 text-right pr-1' style='padding-bottom: 8px !important'>R59.17 </td></tr>");
                                            //    tableHTML.Append("<tr><td class='w-25' style='padding-bottom: 8px !important'>Interest</td><td class='w-25 text-right' style='padding-bottom: 8px !important;padding-right: 15px;'>R122.63</td><td class='w-25' style='padding-bottom: 8px !important'>VAT on fee</td><td class='w-25 text-right pr-1' style='padding-bottom: 8px !important'>R8.88</td></tr>");
                                            //    tableHTML.Append("<tr><td class='w-25' style='padding-bottom: 8px !important'>Agent fee structure</td><td class='w-25 text-right' style='padding-bottom: 8px !important;padding-right: 15px;'>1.18% on capital</td><td class='w-25' style='padding-bottom: 8px !important'>Interest (less agent fee and VAT)</td><td class='w-25 text-right pr-1' style='padding-bottom: 8px !important'>R11.27</td></tr>");
                                            //    tableHTML.Append("</tbody></table>");


                                            //    tableHTML.Append("<div class='d-flex flex-row' style='margin-top: -1.5%;'>");
                                            //    tableHTML.Append("<div class='paymentDueHeaderBlock1 ' style='font-weight: bold;margin-right:3px; margin-bottom:1px; '>Total capital</div>");
                                            //    tableHTML.Append("<div class='paymentDueHeaderBlock1 ' style='font-weight: bold;margin-right:3px; margin-bottom:1px; '>Total interest</div>");
                                            //    tableHTML.Append("<div class='paymentDueHeaderBlock1 ' style='font-weight: bold;margin-right:3px; margin-bottom:1px; '>Total agent fee </br>(deducted)</div>");
                                            //    tableHTML.Append("<div class='paymentDueHeaderBlock1 ' style='font-weight: bold;margin-right:3px; margin-bottom:1px; '>VAT on fee</div>");
                                            //    tableHTML.Append("<div class='paymentDueHeaderBlock1'style='font-weight: bold;margin-bottom:1px'>Interest</br>(less agent fee & VAT)</div>");
                                            //    tableHTML.Append("</div>");
                                            //    tableHTML.Append("<div class='d-flex flex-row' style='margin-top: 2px !important;margin-bottom: 1%;'>");
                                            //    tableHTML.Append("<div class='paymentDueHeaderBlock1 ' style='margin-right:3px; margin-bottom:1px; '>R89 030.44</div>");
                                            //    tableHTML.Append("<div class='paymentDueHeaderBlock1 ' style='margin-right:3px; margin-bottom:1px; '>R190.68</div>");
                                            //    tableHTML.Append("<div class='paymentDueHeaderBlock1' style='margin-right:3px; margin-bottom:1px; '>R59.17</div>");
                                            //    tableHTML.Append("<div class='paymentDueHeaderBlock1 ' style='margin-right:3px; margin-bottom:1px; '>R8.88</div>");
                                            //    tableHTML.Append("<div class='paymentDueHeaderBlock1' style='margin-bottom:1px; '>R122.63</div>");
                                            //    tableHTML.Append("</div>");




                                            //    tableHTML2.Append("<div class='CSTotalAmountDetailsDiv' style='height: 40px !important; text-align: center; padding: 6px !important;'><span class='fnt-14pt'>Matured investment details</span></div>");


                                            //    tableHTML2.Append("<table class= 'CScustomTable HomeLoanDetailDiv' border = '0' style = 'height: auto;margin-bottom:2%;'><tbody>");
                                            //    tableHTML2.Append("<tr><td colspan='2' class='w-25' style='font-weight: bold;padding-bottom: 8px !important;padding-top: 8px !important;'>10 month fixed deposit @ 3,91% per annum</td></tr>");
                                            //    tableHTML2.Append("<tr><td class='w-25' style='padding-bottom: 8px !important'>Interest instruction</td><td class='w-25 text-right' style='padding-bottom: 8px !important;padding-right: 15px;'> Capitalised</td><td class='w-25' style='padding-bottom: 8px !important'>Date Invested</td><td class='w-25 text-right pr-1' style='padding-bottom: 8px !important'>03-05-2021</td></tr>");
                                            //    tableHTML2.Append("<tr><td class='w-25' style='padding-bottom: 8px !important'>Capital</td><td class='w-25 text-right' style='padding-bottom: 8px !important;padding-right: 15px;'>R861 100.49</td><td class='w-25' style='padding-bottom: 8px !important'>Agent fee deducted</td><td class='w-25 text-right pr-1' style='padding-bottom: 8px !important'>R477.73</td></tr>");
                                            //    tableHTML2.Append("<tr><td class='w-25' style='padding-bottom: 8px !important'>Interest</td><td class='w-25 text-right' style='padding-bottom: 8px !important;padding-right: 15px;'>R2 489.95</td><td class='w-25' style='padding-bottom: 8px !important'>VAT on fee</td><td class='w-25 text-right pr-1' style='padding-bottom: 8px !important'>R71.66</td></tr>");
                                            //    tableHTML2.Append("<tr><td class='w-25' style='padding-bottom: 8px !important'>Agent fee structure</td><td class='w-25 text-right' style='padding-bottom: 8px !important;padding-right: 15px;'>0.75% on capital</td><td class='w-25' style='padding-bottom: 8px !important'>Interest (less agent fee and VAT)</td><td class='w-25 text-right pr-1' style='padding-bottom: 8px !important'>R71.66</td></tr>");
                                            //    tableHTML2.Append("</tbody></table>");


                                            //    tableHTML2.Append("<div class='d-flex flex-row' style='margin-top: -1.5%;'>");
                                            //    tableHTML2.Append("<div class='paymentDueHeaderBlock1'style='font-weight: bold;margin-right:3px; margin-bottom:1px;'>Total capital</div>");
                                            //    tableHTML2.Append("<div class='paymentDueHeaderBlock1'style='font-weight: bold;margin-right:3px; margin-bottom:1px;'>Total interest</div>");
                                            //    tableHTML2.Append("<div class='paymentDueHeaderBlock1'style='font-weight: bold;margin-right:3px; margin-bottom:1px;'>Total agent fee </br>(deducted)</div>");
                                            //    tableHTML2.Append("<div class='paymentDueHeaderBlock1'style='font-weight: bold;margin-right:3px; margin-bottom:1px;'>VAT on fee</div>");
                                            //    tableHTML2.Append("<div class='paymentDueHeaderBlock1'style='font-weight: bold; margin-bottom:1px'>Interest</br>(less agent fee & VAT)</div>");
                                            //    tableHTML2.Append("</div>");
                                            //    tableHTML2.Append("<div class='d-flex flex-row' style='margin-top: 2px !important;margin-bottom: 1%;'>");
                                            //    tableHTML2.Append("<div class='paymentDueHeaderBlock1'style='margin-right:3px; margin-bottom:1px;'>R861 100.49</div>");
                                            //    tableHTML2.Append("<div class='paymentDueHeaderBlock1'style='margin-right:3px; margin-bottom:1px;'>R2 489.95</div>");
                                            //    tableHTML2.Append("<div class='paymentDueHeaderBlock1'style='margin-right:3px; margin-bottom:1px;'>R477.73</div>");
                                            //    tableHTML2.Append("<div class='paymentDueHeaderBlock1'style='margin-right:3px; margin-bottom:1px;'>R71.66</div>");
                                            //    tableHTML2.Append("<div class='paymentDueHeaderBlock1'style='margin-bottom:1px;'>R1 940.56</div>");
                                            //    tableHTML2.Append("</div>");

                                            //    tableHTML.Append(tableHTML2);
                                            //    // if (customerMaster.InvestorId == 9018933796)
                                            //    if (pageIdentifier == 203)
                                            //    {
                                            //        htmlWidget.Replace("{{dynamicMsg}}", "<div class='card border-0'><div class='card-body text-left'style='padding: 0;'><div class='card-body-header mt-3-2' style='font-family: \"Arial\";font-weight: 700;'>Important information</div> <div class='' style='font-size: 9pt; font-family: \"Arial\";'><p>Rente (min agentadministrasiegelde en BTW) word in Maart op u rekening gekrediteer. Die agentadministrasiegelde en BTW word in Maart afgetrek en namens u aan u agent betaal, in ooreenstemming met die mandaat wat gehou word. </p><p> Artikel 86(4) - rekenings wat voor 1 November 2018 geopen is, is aan die bepalings van die Prokureurswet, 53 van 1979, onderworpe.Ingevolge artikel 86(4) van die Wet op Regspraktyk, 28 van 2014, is 5 % van die rente verdien vanaf 1 Maart 2019 aan die Getrouheidsfonds vir Regspraktisyns betaal.</p></div></div></div>");
                                            //    }
                                            //    if (pageIdentifier == 198)
                                            //    {
                                            //        htmlWidget.Replace("{{dynamicMsg}}", "<div class='card border-0'><div class='card-body text-left'style='padding: 0;'><div class='card-body-header mt-3-2' style='font-family: \"Arial\";font-weight: 700;'>Important information</div> <div class='' style='font-size: 9pt; font-family: \"Arial\";'><p>Interest(less agent administration fee and VAT) is credited to your account in March.The agent administration fee and VAT are deducted in March and paid on your behalf to your agent, in accordance with the mandate held.</p></div></div></div>");
                                            //    }


                                            //    htmlWidget.Replace("{{dynemicTables}}", tableHTML.ToString());
                                            //    htmlString.Append(htmlWidget);


                                            //}
                                            //else if (mergedlst[i].WidgetName == HtmlConstants.NETBANK_CORPORATESAVER_AGENTDETAILS_NAME)
                                            //{
                                            //    string jsonstr = HtmlConstants.CORPORATESAVER_TRANSACTION_PREVIEW_JSON_STRING;
                                            //    var widgetHtml = new StringBuilder(HtmlConstants.NETBANK_CORPORATESAVER_AGENTDETAILS_HTML);
                                            //    if (jsonstr != string.Empty && validationEngine.IsValidJson(jsonstr))
                                            //    {
                                            //        var mcaTransaction = JsonConvert.DeserializeObject<List<DM_MCATransaction>>(jsonstr);
                                            //        StringBuilder rowsHTML = new StringBuilder();
                                            //        if (mcaTransaction.Count> 0)
                                            //        {
                                            //            var trans = mcaTransaction[0];
                                            //            rowsHTML.Append("<tr class='ht-20'>" +
                                            //                "<td class='w-25 text-left'>" + DateTime.Now.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy) + "</td>" +
                                            //                "<td class='w-25 text-right'>" + DateTime.Now.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy) + "</td>" +
                                            //                "<td class='w-25 text-center'>" + trans.Rate + "</td>" +
                                            //                "<td class='w-25 text-center'>" + trans.Credit + "</td>" +
                                            //                "</tr>"
                                            //                );
                                            //        }
                                            //        widgetHtml.Replace("{{MCAVATTable}}", rowsHTML.ToString());
                                            //        htmlString.Append(widgetHtml);
                                            //    }
                                            //}
                                            //else if (mergedlst[i].WidgetName == HtmlConstants.NEDBANK_CORPORATESAVER_CLIENT_DETAILS_NAME)
                                            //{
                                            //    string jsonstr = HtmlConstants.CORPORATESAVER_CLIENT_DETAILS_HTML;
                                            //    if (jsonstr != string.Empty && validationEngine.IsValidJson(jsonstr))
                                            //    {
                                            //        var mcaMaster = JsonConvert.DeserializeObject<DM_MCAMaster>(jsonstr);
                                            //        var htmlWidget = new StringBuilder(HtmlConstants.CORPORATESAVER_CLIENT_DETAILS_HTML);
                                            //        htmlWidget.Replace("{{AccountNo}}", mcaMaster.CustomerId);
                                            //        htmlWidget.Replace("{{StatementNo}}", mcaMaster.StatementNo);
                                            //        htmlWidget.Replace("{{OverdraftLimit}}", mcaMaster.OverdraftLimit != null ? mcaMaster.OverdraftLimit.ToString() : "0.00");
                                            //        htmlWidget.Replace("{{StatementDate}}", mcaMaster.StatementDate != null ? mcaMaster.StatementDate.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy) : DateTime.Now.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy));
                                            //        htmlWidget.Replace("{{Currency}}", mcaMaster.Currency);
                                            //        htmlWidget.Replace("{{Statementfrequency}}", mcaMaster.StatementFrequency);
                                            //        htmlWidget.Replace("{{FreeBalance}}", mcaMaster.FreeBalance != null ? mcaMaster.FreeBalance.ToString() : "0.00");
                                            //        htmlString.Append(htmlWidget);
                                            //    }
                                            //}
                                            //else if (mergedlst[i].WidgetName == HtmlConstants.NEDBANK_WEALTH_CORPORATESAVER_BRANCH_DETAILS_WIDGET_NAME)
                                            // {
                                            //     var htmlWidget = new StringBuilder(HtmlConstants.CORPORATESAVER_WEALTH_BRANCH_DETAILS_WIDGET_HTML);
                                            //     StringBuilder htmlBankDetails = new StringBuilder();
                                            //     htmlBankDetails.Append(HtmlConstants.BANK_DETAILS);
                                            //     htmlBankDetails.Replace("{{TodayDate}}", DateTime.Now.ToString(ModelConstant.DATE_FORMAT_yyyy_MM_dd));

                                            //     htmlWidget.Replace("{{BranchDetails}}", htmlBankDetails.ToString());
                                            //     htmlWidget.Replace("{{ContactCenter}}", HtmlConstants.WEA_BANKING);
                                            //     htmlString.Append(htmlWidget.ToString());
                                            // }
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

        public static string GetMonthRange(int month)
        {
            switch (month)
            {
                case 1: return "1 Jan - 31 Jan";
                case 2: return "1 Feb - 28 Feb";
                case 3: return "1 Mar - 31 Mar";
                case 4: return "1 Apr - 30 Apr";
                case 5: return "1 May - 31 May";
                case 6: return "1 Jun - 30 Jun";
                case 7: return "1 Jul - 31 Jul";
                case 8: return "1 Aug - 31 Aug";
                case 9: return "1 Sep - 30 Sep";
                case 10: return "1 Oct - 31 Oct";
                case 11: return "1 Nov - 30 Nov";
                case 12: return "1 Dec - 31 Dec";
                default: return "Invalid Month";
            }
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
