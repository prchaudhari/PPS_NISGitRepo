// <copyright file="PageManager.cs" company="Websym Solutions Pvt. Ltd.">
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
            if (pages.Count != 0)
            {
                htmlString.Append(HtmlConstants.CONTAINER_DIV_HTML_HEADER);
                for (int y = 0; y < pages.Count; y++)
                {
                    string divId = pages[y].PageTypeId == HtmlConstants.HOME_PAGE_TYPE_ID ? HtmlConstants.HOME_PAGE_DIV_NAME : pages[y].PageTypeId == HtmlConstants.SAVING_ACCOUNT_PAGE_TYPE_ID ? HtmlConstants.SAVING_ACCOUNT_PAGE_DIV_NAME : pages[y].PageTypeId == HtmlConstants.CURRENT_ACCOUNT_PAGE_TYPE_ID ? HtmlConstants.CURRENT_ACCOUNT_PAGE_DIV_NAME : string.Empty;

                    htmlString.Append(HtmlConstants.WIDGET_HTML_HEADER.Replace("{{DivId}}", divId));
                    int tempRowWidth = 0;
                    int max = 0;
                    if (pages[y].PageWidgets.Count > 0)
                    {
                        var completelst = pages[y].PageWidgets;
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
                                        htmlString.Append("<div class='row'>");
                                        isRowComplete = false;
                                    }
                                    int divLength = (mergedlst[i].Width * 12) / 20;
                                    tempRowWidth = tempRowWidth + divLength;

                                    // If current col-lg class length is greater than 12, 
                                    //then end parent row class div and then start new row class div
                                    if (tempRowWidth > 12)
                                    {
                                        tempRowWidth = divLength;
                                        htmlString.Append("</div>");
                                        htmlString.Append("<div class='row'>");
                                        isRowComplete = false;
                                    }
                                    htmlString.Append("<div class='col-lg-" + divLength + "'>");
                                    if (mergedlst[i].WidgetId == HtmlConstants.CUSTOMER_INFORMATION_WIDGET_ID)
                                    {
                                        //string customerInfoJson = "{'FirstName':'Laura','MiddleName':'J','LastName':'Donald','AddressLine1':'4000 Executive Parkway','AddressLine2':'Saint Globin Rd #250','City':'Canary Wharf','State':'London','Country':'England','Zip':'E14 9RZ'}";
                                        //if (customerInfoJson != string.Empty && validationEngine.IsValidJson(customerInfoJson))
                                        //{
                                        //    dynamic customerInfo = JObject.Parse(customerInfoJson);
                                        //    var customerHtmlWidget = HtmlConstants.CUSTOMER_INFORMATION_WIDGET_HTML.Replace("{{VideoSource}}", "assets/images/SampleVideo.mp4");

                                        //    string customerName = customerInfo.FirstName + " " + customerInfo.MiddleName + " " + customerInfo.LastName;
                                        //    customerHtmlWidget = customerHtmlWidget.Replace("{{CustomerName}}", customerName);

                                        //    string address1 = customerInfo.AddressLine1 + ", " + customerInfo.AddressLine2 + ",";
                                        //    customerHtmlWidget = customerHtmlWidget.Replace("{{Address1}}", address1);

                                        //    string address2 = (customerInfo.City != "" ? customerInfo.City + "," : "") + (customerInfo.State != "" ? customerInfo.State + "," : "") + (customerInfo.Country != "" ? customerInfo.Country + "," : "") + (customerInfo.Zip != "" ? customerInfo.Zip : "");
                                        //    customerHtmlWidget = customerHtmlWidget.Replace("{{Address2}}", address2);

                                        //    htmlString.Append(customerHtmlWidget);
                                        //}

                                        var customerHtmlWidget = HtmlConstants.CUSTOMER_INFORMATION_WIDGET_HTML.Replace("{{VideoSource}}", "assets/images/SampleVideo.mp4");
                                        customerHtmlWidget = customerHtmlWidget.Replace("{{CustomerName}}", "Laura J Donald");
                                        customerHtmlWidget = customerHtmlWidget.Replace("{{Address1}}", "4000 Executive Parkway, Saint Globin Rd #250,");
                                        customerHtmlWidget = customerHtmlWidget.Replace("{{Address2}}", "Canary Wharf, E94583");
                                        htmlString.Append(customerHtmlWidget);
                                    }
                                    else if (mergedlst[i].WidgetId == HtmlConstants.ACCOUNT_INFORMATION_WIDGET_ID)
                                    {
                                        //string accountInfoJson = "{'StatementDate':'1-APR-2020','StatementPeriod':'Annual Statement','CustomerID':'ID2-8989-5656','RMName':'James Wiilims','RMContactNumber':'+4487867833'}";
                                        //if (accountInfoJson != string.Empty && validationEngine.IsValidJson(accountInfoJson))
                                        //{
                                        //    dynamic accountInfo = JObject.Parse(accountInfoJson);
                                        //    //if(accountInfo)
                                        //}
                                        htmlString.Append(HtmlConstants.ACCOUNT_INFORMATION_WIDGET_HTML);
                                    }
                                    else if (mergedlst[i].WidgetId == HtmlConstants.IMAGE_WIDGET_ID)
                                    {
                                        var imgAssetFilepath = "assets/images/icon-image.png";
                                        if (mergedlst[i].WidgetSetting != string.Empty && validationEngine.IsValidJson(mergedlst[i].WidgetSetting))
                                        {
                                            dynamic widgetSetting = JObject.Parse(mergedlst[i].WidgetSetting);
                                            if (widgetSetting.isPersonalize == false)
                                            {
                                                imgAssetFilepath = baseURL + "/assets/" + widgetSetting.AssetLibraryId + "/" + widgetSetting.AssetName;
                                            }
                                        }
                                        var imgHtmlWidget = HtmlConstants.IMAGE_WIDGET_HTML.Replace("{{ImageSource}}", imgAssetFilepath);
                                        htmlString.Append(imgHtmlWidget);
                                    }
                                    else if (mergedlst[i].WidgetId == HtmlConstants.VIDEO_WIDGET_ID)
                                    {
                                        var vdoAssetFilepath = "assets/images/SampleVideo.mp4";
                                        if (mergedlst[i].WidgetSetting != string.Empty && validationEngine.IsValidJson(mergedlst[i].WidgetSetting))
                                        {
                                            dynamic widgetSetting = JObject.Parse(mergedlst[i].WidgetSetting);
                                            if (widgetSetting.isPersonalize == false)
                                            {
                                                vdoAssetFilepath = baseURL + "/assets/" + widgetSetting.AssetLibraryId + "/" + widgetSetting.AssetName;
                                            }
                                        }
                                        var vdoHtmlWidget = HtmlConstants.VIDEO_WIDGET_HTML.Replace("{{VideoSource}}", vdoAssetFilepath);
                                        htmlString.Append(vdoHtmlWidget);
                                    }
                                    else if (mergedlst[i].WidgetId == HtmlConstants.SUMMARY_AT_GLANCE_WIDGET_ID)
                                    {
                                        htmlString.Append(HtmlConstants.SUMMARY_AT_GLANCE_WIDGET_HTML);
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
            }

            return htmlString.ToString();
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

        #endregion
    }
}
