// <copyright file="StatementManager.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2018 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------

namespace nIS
{
    using Newtonsoft.Json.Linq;
    #region References
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
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

        public string PreviewStatement(long statementIdentifier, string baseURL, string tenantCode)
        {
            StringBuilder htmlString = new StringBuilder();
            string finalHtml = "";
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
                var statementPages = statements[0].StatementPages;
                if (statementPages.Count != 0)
                {
                    //htmlString.Append(HtmlConstants.SCRIPT_TAG);
                    string navbarHtml = HtmlConstants.NAVBAR_HTML.Replace("{{BrandLogo}}", "assets/images/absa-logo.png");
                    navbarHtml = navbarHtml.Replace("{{Today}}", DateTime.Now.ToString("dd MMM yyyy"));
                    StringBuilder navItemList = new StringBuilder();
                    htmlString.Append(HtmlConstants.CONTAINER_DIV_HTML_HEADER);
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

                        IList<Page> pages = this.pageRepository.GetPages(pageSearchParameter, tenantCode);
                        if (pages.Count != 0)
                        {
                            for (int y = 0; y < pages.Count; y++)
                            {
                                string divId = pages[y].PageTypeId == HtmlConstants.HOME_PAGE_TYPE_ID ? HtmlConstants.HOME_PAGE_DIV_NAME : pages[y].PageTypeId == HtmlConstants.SAVING_ACCOUNT_PAGE_TYPE_ID ? HtmlConstants.SAVING_ACCOUNT_PAGE_DIV_NAME : pages[y].PageTypeId == HtmlConstants.CURRENT_ACCOUNT_PAGE_TYPE_ID ? HtmlConstants.CURRENT_ACCOUNT_PAGE_DIV_NAME : string.Empty;

                                if (divId == HtmlConstants.HOME_PAGE_DIV_NAME)
                                {
                                    navItemList.Append(" <li class='nav-item'><a class='nav-link "+ (x == 0 ? "active" : "")+" "+HtmlConstants.HOME_PAGE_DIV_NAME+"' href='javascript:void(0);'>At a Glance</a> </li> ");
                                }
                                else if (divId == HtmlConstants.SAVING_ACCOUNT_PAGE_DIV_NAME)
                                {
                                    navItemList.Append(" <li class='nav-item'><a class='nav-link " + (x == 0 ? "active" : "") + " " + HtmlConstants.SAVING_ACCOUNT_PAGE_DIV_NAME + "' href='javascript:void(0);'>Saving Account</a> </li> ");
                                }
                                else if (divId == HtmlConstants.CURRENT_ACCOUNT_PAGE_DIV_NAME)
                                {
                                    navItemList.Append(" <li class='nav-item'><a class='nav-link " + (x == 0 ? "active" : "") + " " + HtmlConstants.CURRENT_ACCOUNT_PAGE_DIV_NAME + "' href='javascript:void(0);'>Current Account</a> </li> ");
                                }

                                string ExtraClassName = x > 0 ? "d-none " + divId : divId;
                                string widgetHtmlHeader = HtmlConstants.WIDGET_HTML_HEADER.Replace("{{ExtraClass}}", ExtraClassName);
                                widgetHtmlHeader = widgetHtmlHeader.Replace("{{DivId}}", divId);
                                htmlString.Append(widgetHtmlHeader);
                                int tempRowWidth = 0; // variable to check col-lg div length (bootstrap)
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
                                                    htmlString.Append("<div class='row'>"); // to start new row class div 
                                                    isRowComplete = false;
                                                }
                                                int divLength = (mergedlst[i].Width * 12) / 20;
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
                                                    //string customerInfoJson = "{'FirstName':'Laura','MiddleName':'J','LastName':'Donald','AddressLine1':'4000 Executive Parkway','AddressLine2':'Saint Globin Rd #250','City':'Canary Wharf','State':'London','Country':'England','Zip':'E14 9RZ'}";

                                                    //dynamic customerInfo = JObject.Parse(customerInfoJson);
                                                    //var customerHtmlWidget = HtmlConstants.CUSTOMER_INFORMATION_WIDGET_HTML.Replace("{{VideoSource}}", "assets/images/SampleVideo.mp4");
                                                    //customerHtmlWidget = customerHtmlWidget.Replace("{{CustomerName}}", customerInfo.FirstName + " "+ customerInfo.MiddleName+ " "+ customerInfo.LastName);
                                                    //customerHtmlWidget = customerHtmlWidget.Replace("{{Address1}}", customerInfo.AddressLine1 + ", " +customerInfo.AddressLine2 + ",");
                                                    //string address2 = (customerInfo.City != "" ? customerInfo.City + "," : "") + (customerInfo.State != "" ? customerInfo.State + "," : "") + (customerInfo.Country != "" ? customerInfo.Country + "," : "") + (customerInfo.Zip != "" ? customerInfo.Zip : "");
                                                    //customerHtmlWidget = customerHtmlWidget.Replace("{{Address2}}", address2);
                                                    //htmlString.Append(customerHtmlWidget);

                                                    var customerHtmlWidget = HtmlConstants.CUSTOMER_INFORMATION_WIDGET_HTML.Replace("{{VideoSource}}", "assets/images/SampleVideo.mp4");
                                                    customerHtmlWidget = customerHtmlWidget.Replace("{{CustomerName}}", "Laura J Donald");
                                                    customerHtmlWidget = customerHtmlWidget.Replace("{{Address1}}", "4000 Executive Parkway, Saint Globin Rd #250,");
                                                    customerHtmlWidget = customerHtmlWidget.Replace("{{Address2}}", "Canary Wharf, E94583");
                                                    htmlString.Append(customerHtmlWidget);
                                                }
                                                else if (mergedlst[i].WidgetId == HtmlConstants.ACCOUNT_INFORMATION_WIDGET_ID)
                                                {
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
                    }

                    navbarHtml = navbarHtml.Replace("{{NavItemList}}", navItemList.ToString());
                    htmlString.Append(HtmlConstants.CONTAINER_DIV_HTML_FOOTER);
                    finalHtml = navbarHtml + htmlString.ToString();
                }
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
