// <copyright file="DynamicWidgetManager.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2018 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------

namespace nIS
{
    #region References
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Unity;
    using Newtonsoft.Json.Linq;
    using System.Dynamic;

    #endregion

    /// <summary>
    /// This class implements manager layer of dynamicWidget manager.
    /// </summary>
    public class DynamicWidgetManager
    {
        #region Private members
        /// <summary>
        /// The unity container
        /// </summary>
        private IUnityContainer unityContainer = null;

        /// <summary>
        /// The asset library repository.
        /// </summary>
        IDynamicWidgetRepository dynamicWidgetRepository = null;

        /// <summary>
        /// The tenant configuration manager object.
        /// </summary>
        private TenantConfigurationManager tenantConfigurationManager = null;

        #endregion

        #region Constructor

        /// <summary>
        /// This is constructor for dynamicWidget manager, which initialise
        /// dynamicWidget repository.
        /// </summary>
        /// <param name="container">IUnity container implementation object.</param>
        public DynamicWidgetManager(IUnityContainer unityContainer)
        {
            try
            {
                this.unityContainer = unityContainer;
                this.dynamicWidgetRepository = this.unityContainer.Resolve<IDynamicWidgetRepository>();
                this.tenantConfigurationManager = new TenantConfigurationManager(unityContainer);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Public Methods

        #region Add DynamicWidget

        /// <summary>
        /// This method will call add dynamicWidget method of repository.
        /// </summary>
        /// <param name="countries">DynamicWidget are to be add.</param>
        /// <param name="tenantCode">Tenant code of dynamicWidget.</param>
        /// <returns>
        /// Returns true if entities added successfully, false otherwise.
        /// </returns>
        public bool AddDynamicWidgets(IList<DynamicWidget> dynamicWidgets, string tenantCode)
        {
            bool result = false;
            try
            {
                this.IsValidDynamicWidget(dynamicWidgets, tenantCode);
                this.IsDuplicateDynamicWidget(dynamicWidgets, tenantCode);
                var tenantConfiguration = this.tenantConfigurationManager.GetTenantConfigurations(tenantCode)?.FirstOrDefault();

                dynamicWidgets.ToList().ForEach(item =>
                {
                    if (item.WidgetSettings != null && item.WidgetSettings != string.Empty)
                    {
                        TenantEntity entity = new TenantEntity();
                        entity.Identifier = item.EntityId;
                        entity.Name = item.EntityName;
                        IList<EntityFieldMap> entityFieldMaps = new List<EntityFieldMap>();
                        entityFieldMaps = this.GetEntityFields(item.EntityId, tenantCode);
                        CustomeTheme themeDetails = new CustomeTheme();
                        string theme = string.Empty;
                        if (item.ThemeType == "Default")
                        {
                            themeDetails = JsonConvert.DeserializeObject<CustomeTheme>(tenantConfiguration.WidgetThemeSetting);
                        }
                        else
                        {
                            themeDetails = JsonConvert.DeserializeObject<CustomeTheme>(item.ThemeCSS);
                        }
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
                        item.PreviewData = this.GetPreviewData(entity, item.Title, item.WidgetSettings, item.WidgetType, theme, entityFieldMaps);
                    }
                });
                result = this.dynamicWidgetRepository.AddDynamicWidgets(dynamicWidgets, tenantCode);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Update DynamicWidget

        /// <summary>
        /// This method reference helps to update details about countries.
        /// </summary>
        /// <param name="countries">
        /// The list of countries.
        /// </param>
        /// <param name="tenantCode">
        /// The tenant code
        /// </param>
        /// <returns>
        /// It will return true if list of scene updates scus=ccessfully otherwise false
        /// </returns>
        public bool UpdateDynamicWidgets(IList<DynamicWidget> dynamicWidgets, string tenantCode)
        {
            bool result = false;
            try
            {
                this.IsValidDynamicWidget(dynamicWidgets, tenantCode);
                this.IsDuplicateDynamicWidget(dynamicWidgets, tenantCode);
                var tenantConfiguration = this.tenantConfigurationManager.GetTenantConfigurations(tenantCode)?.FirstOrDefault();

                dynamicWidgets.ToList().ForEach(item =>
                {
                    if (item.WidgetSettings != null && item.WidgetSettings != string.Empty)
                    {
                        TenantEntity entity = new TenantEntity();
                        entity.Identifier = item.EntityId;
                        entity.Name = item.EntityName;
                        IList<EntityFieldMap> entityFieldMaps = new List<EntityFieldMap>();
                        entityFieldMaps = this.GetEntityFields(item.EntityId, tenantCode);
                        CustomeTheme themeDetails = new CustomeTheme();
                        string theme = string.Empty;
                        if (item.ThemeType == "Default")
                        {
                            themeDetails = JsonConvert.DeserializeObject<CustomeTheme>(tenantConfiguration.WidgetThemeSetting);
                        }
                        else
                        {
                            themeDetails = JsonConvert.DeserializeObject<CustomeTheme>(item.ThemeCSS);
                        }
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
                        item.PreviewData = this.GetPreviewData(entity, item.Title, item.WidgetSettings, item.WidgetType, theme, entityFieldMaps);
                    }
                });
                result = this.dynamicWidgetRepository.UpdateDynamicWidgets(dynamicWidgets, tenantCode);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Delete DynamicWidget

        /// <summary>
        /// This method reference helps to delete details about dynamicWidget.
        /// </summary>
        /// <param name="countries">
        /// The list of countries.
        /// </param>
        /// <param name="tenantCode">
        /// The tenant code
        /// </param>
        /// <returns>
        /// It will return true if successfully updated or it will throw an exception.
        /// </returns>
        public bool DeleteDynamicWidgets(IList<DynamicWidget> countries, string tenantCode)
        {
            bool result = false;
            try
            {
                result = this.dynamicWidgetRepository.DeleteDynamicWidgets(countries, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return result;
        }

        #endregion

        #region Get DynamicWidgets

        /// <summary>
        /// This method will call get countries method of repository.
        /// </summary>
        /// <param name="dynamicWidgetSearchParameter">The dynamicWidget search parameters.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns>
        /// Returns roles if found for given parameters, else return null
        /// </returns>
        public IList<DynamicWidget> GetDynamicWidgets(DynamicWidgetSearchParameter dynamicWidgetSearchParameter, string tenantCode)
        {
            IList<DynamicWidget> countries = new List<DynamicWidget>();
            try
            {
                InvalidSearchParameterException invalidSearchParameterException = new InvalidSearchParameterException(tenantCode);
                try
                {
                    dynamicWidgetSearchParameter.IsValid();
                }
                catch (Exception exception)
                {
                    invalidSearchParameterException.Data.Add("InvalidPagingParameter", exception.Data);
                }

                if (invalidSearchParameterException.Data.Count > 0)
                {
                    throw invalidSearchParameterException;
                }

                countries = this.dynamicWidgetRepository.GetDynamicWidgets(dynamicWidgetSearchParameter, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return countries;
        }

        #endregion

        public IList<TenantEntity> GetTenantEntities(string tenantCode)
        {
            IList<TenantEntity> tenantEntities = new List<TenantEntity>();
            try
            {

                tenantEntities = this.dynamicWidgetRepository.GetTenantEntities(tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return tenantEntities;
        }


        public IList<EntityFieldMap> GetEntityFields(long entityIdentfier, string tenantCode)
        {
            IList<EntityFieldMap> entityFieldMaps = new List<EntityFieldMap>();
            try
            {

                entityFieldMaps = this.dynamicWidgetRepository.GetEntityFields(entityIdentfier, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return entityFieldMaps;
        }

        #region Get DynamicWidget Count
        /// <summary>
        /// This method helps to get count of dynamicWidget.
        /// </summary>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns>
        /// Returns count of roles
        /// </returns>
        public long GetDynamicWidgetCount(DynamicWidgetSearchParameter dynamicWidgetSearchParameter, string tenantCode)
        {
            long roleCount = 0;
            try
            {
                roleCount = this.dynamicWidgetRepository.GetDynamicWidgetCount(dynamicWidgetSearchParameter, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return roleCount;
        }
        #endregion

        public string PreviewDynamicWidget(long widgetIdentifier, string baseURL, string tenantCode)
        {
            StringBuilder htmlString = new StringBuilder();
            string series = string.Empty;
            try
            {
                var tenantConfiguration = this.tenantConfigurationManager.GetTenantConfigurations(tenantCode)?.FirstOrDefault();
                DynamicWidgetSearchParameter pageSearchParameter = new DynamicWidgetSearchParameter
                {
                    Identifier = widgetIdentifier.ToString(),
                    SortParameter = new SortParameter()
                    {
                        SortOrder = SortOrder.Ascending,
                        SortColumn = ModelConstant.SORT_COLUMN,
                    },
                };
                IList<DynamicWidget> dynamicWidgets = this.dynamicWidgetRepository.GetDynamicWidgets(pageSearchParameter, tenantCode);
                if (dynamicWidgets.Count != 0)
                {
                    htmlString.Append(HtmlConstants.CONTAINER_DIV_HTML_HEADER);
                    for (int index = 0; index < dynamicWidgets.Count; index++)
                    {

                        var dynamicWidget = dynamicWidgets[index];
                        if (dynamicWidget.WidgetSettings == null || dynamicWidget.WidgetSettings == string.Empty)
                        {
                            throw new WidgetSettingsNotFoundException(tenantCode);
                        }
                        else
                        {
                            string html = string.Empty;
                            TenantEntity entity = new TenantEntity();
                            entity.Identifier = dynamicWidget.EntityId;
                            entity.Name = dynamicWidget.EntityName;
                            CustomeTheme themeDetails = new CustomeTheme();
                            if (dynamicWidget.ThemeType == "Default")
                            {
                                themeDetails = JsonConvert.DeserializeObject<CustomeTheme>(tenantConfiguration.WidgetThemeSetting);
                            }
                            else
                            {
                                themeDetails = JsonConvert.DeserializeObject<CustomeTheme>(dynamicWidget.ThemeCSS);
                            }
                            if (dynamicWidget.WidgetType == HtmlConstants.TABLE_DYNAMICWIDGET)
                            {
                                html = HtmlConstants.TABLEWIDEGTPREVIEW;

                                #region Apply theme settings
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
                                #endregion

                                html = html.Replace("{{WidgetTitle}}", dynamicWidget.Title);
                                List<DynamicWidgetTableEntity> entityFields = JsonConvert.DeserializeObject<List<DynamicWidgetTableEntity>>(dynamicWidget.WidgetSettings);
                                StringBuilder tableHeader = new StringBuilder();
                                tableHeader.Append("<tr>" + string.Join("", entityFields.Select(field => string.Format("<th>{0}</th> ", field.HeaderName))) + "</tr>");
                                html = html.Replace("{{tableHeader}}", tableHeader.ToString());
                                string tableBody = dynamicWidget.PreviewData;
                                html = html.Replace("{{tableBody}}", tableBody);

                                htmlString.Append(html);
                                htmlString.Append(HtmlConstants.CONTAINER_DIV_HTML_FOOTER);
                            }
                            else if (dynamicWidget.WidgetType == HtmlConstants.FORM_DYNAMICWIDGET)
                            {
                                html = HtmlConstants.FORMWIDGETPREVIEW;

                                #region Apply theme settings
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

                                #endregion

                                html = html.Replace("{{WidgetTitle}}", dynamicWidget.Title);
                                //List<DynamicWidgetFormEntity> formEntity = JsonConvert.DeserializeObject<List<DynamicWidgetFormEntity>>(dynamicWidget.WidgetSettings);
                                StringBuilder tableHeader = new StringBuilder();
                                //string jsonData = this.GetFormPreviewData(entity, formEntity);
                                html = html.Replace("{{FormData}}", dynamicWidget.PreviewData);
                                htmlString.Append(html);
                                htmlString.Append(HtmlConstants.CONTAINER_DIV_HTML_FOOTER);
                            }
                            else if (dynamicWidget.WidgetType == HtmlConstants.HTML_DYNAMICWIDGET)
                            {
                                html = HtmlConstants.HTMLWIDGETPREVIEW;

                                #region Apply theme settings
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

                                #endregion

                                html = html.Replace("{{WidgetTitle}}", dynamicWidget.Title);
                                string settings = dynamicWidget.WidgetSettings;
                                IList<EntityFieldMap> entityFieldMaps = new List<EntityFieldMap>();
                                entityFieldMaps = this.GetEntityFields(dynamicWidget.EntityId, tenantCode);
                                string data = this.GetHTMLPreviewData(entity, entityFieldMaps, dynamicWidget.WidgetSettings);
                                StringBuilder tableHeader = new StringBuilder();
                                html = html.Replace("{{FormData}}", data);
                                htmlString.Append(html);
                                htmlString.Append(HtmlConstants.CONTAINER_DIV_HTML_FOOTER);
                            }
                            else if (dynamicWidget.WidgetType == HtmlConstants.LINEGRAPH_DYNAMICWIDGET)
                            {
                                html = HtmlConstants.LINEGRAPH_WIDGETPREVIEW;
                                #region Apply theme settings
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

                                #endregion
                                html = html.Replace("{{WidgetTitle}}", dynamicWidget.Title);
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
                                series = dynamicWidget.PreviewData;
                                htmlString.Append(html);
                                htmlString.Append(HtmlConstants.CONTAINER_DIV_HTML_FOOTER);
                                JObject obj = new JObject();
                                obj["html"] = htmlString.ToString();
                                obj["chartData"] = series;
                                string chartData = JsonConvert.SerializeObject(obj);
                                htmlString = new StringBuilder();
                                htmlString = htmlString.Append(chartData);
                            }
                            else if (dynamicWidget.WidgetType == HtmlConstants.BARGRAPH_DYNAMICWIDGET)
                            {
                                html = HtmlConstants.BARGRAPH_WIDGETPREVIEW;

                                #region Apply theme settings
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

                                #endregion

                                html = html.Replace("{{WidgetTitle}}", dynamicWidget.Title);
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
                                series = dynamicWidget.PreviewData;
                                htmlString.Append(html);
                                htmlString.Append(HtmlConstants.CONTAINER_DIV_HTML_FOOTER);
                                JObject obj = new JObject();
                                obj["html"] = htmlString.ToString();
                                obj["chartData"] = series;
                                string chartData = JsonConvert.SerializeObject(obj);
                                htmlString = new StringBuilder();
                                htmlString = htmlString.Append(chartData);
                            }
                            else if (dynamicWidget.WidgetType == HtmlConstants.PICHART_DYNAMICWIDGET)
                            {
                                html = HtmlConstants.PIECHART_WIDGETPREVIEW;

                                #region Apply theme settings
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

                                #endregion

                                html = html.Replace("{{WidgetTitle}}", dynamicWidget.Title);
                                PieChartSettingDetails pieChartSetting = JsonConvert.DeserializeObject<PieChartSettingDetails>(dynamicWidget.WidgetSettings);
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

                                series = dynamicWidget.PreviewData;
                                htmlString.Append(html);
                                htmlString.Append(HtmlConstants.CONTAINER_DIV_HTML_FOOTER);
                                JObject obj = new JObject();
                                obj["html"] = htmlString.ToString();
                                obj["chartData"] = series;
                                string chartData = JsonConvert.SerializeObject(obj);
                                htmlString = new StringBuilder();
                                htmlString = htmlString.Append(chartData);
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return htmlString.ToString();
        }

        /// <summary>
        /// This method will call publish page method of repository
        /// </summary>
        /// <param name="pageIdentifier">Page identifier</param>
        /// <param name="tenantCode">Tenant code of page.</param>
        /// <returns>
        /// Returns true if pages publish successfully, false otherwise.
        /// </returns>
        public bool PublishDynamicWidget(long widgetIdentifier, string tenantCode)
        {
            bool result = false;
            try
            {
                result = this.dynamicWidgetRepository.PublishDynamicWidget(widgetIdentifier, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return result;
        }

        /// <summary>
        /// This method will call clone dynamicWidget method of repository
        /// </summary>
        /// <param name="dynamicWidgetIdentifier">DynamicWidget identifier</param>
        /// <param name="tenantCode">Tenant code of dynamicWidget.</param>
        /// <returns>
        /// Returns true if dynamicWidgets clone successfully, false otherwise.
        /// </returns>
        public bool CloneDynamicWidget(long dynamicWidgetIdentifier, string tenantCode)
        {
            bool result = false;
            try
            {
                result = this.dynamicWidgetRepository.CloneDynamicWidget(dynamicWidgetIdentifier, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return result;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// This method is responsible for validate dynamicWidget.
        /// </summary>
        /// <param name="countries"></param>
        /// <param name="tenantCode"></param>
        private void IsValidDynamicWidget(IList<DynamicWidget> countries, string tenantCode)
        {
            try
            {
                if (countries?.Count <= 0)
                {
                    throw new NullArgumentException(tenantCode);
                }

                InvalidDynamicWidgetException invalidDynamicWidgetException = new InvalidDynamicWidgetException(tenantCode);
                countries.ToList().ForEach(item =>
                {
                    try
                    {
                        item.IsValid();
                    }
                    catch (Exception ex)
                    {
                        invalidDynamicWidgetException.Data.Add(item.WidgetName, ex.Data);
                    }
                });

                if (invalidDynamicWidgetException.Data.Count > 0)
                {
                    throw invalidDynamicWidgetException;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method helps to check duplicate dynamicWidget in the list
        /// </summary>
        /// <param name="countries">countries</param>
        /// <param name="tenantCode">tenant code</param>
        private void IsDuplicateDynamicWidget(IList<DynamicWidget> countries, string tenantCode)
        {
            try
            {
                int isDuplicateDynamicWidget = countries.GroupBy(p => p.WidgetName).Where(g => g.Count() > 1).Count();
                if (isDuplicateDynamicWidget > 0)
                {
                    throw new DuplicateDynamicWidgetFoundException(tenantCode);
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        public string GetTablePreviewData(TenantEntity entity, List<DynamicWidgetTableEntity> fieldMaps)
        {
            string obj = string.Empty;
            IList<JObject> dataList = new List<JObject>();
            JObject item = new JObject();

            StringBuilder tableBody = new StringBuilder();
            for (int i = 1; i < 5; i++)
            {
                item = new JObject();
                fieldMaps.ToList().ForEach(field =>
                {
                    item[field.FieldName] = field.FieldName + i.ToString();
                });
                dataList.Add(item);
            }
            dataList.ToList().ForEach(d =>
            {
                tableBody.Append("<tr>" + string.Join("", fieldMaps.Select(field =>
                string.Format("<td>{0}</td> ", d[field.FieldName].ToString()))) + "</tr>");
            });
            obj = tableBody.ToString();
            return obj;
        }

        public string GetFormPreviewData(TenantEntity entity, List<DynamicWidgetFormEntity> fieldMaps)
        {
            string obj = string.Empty;
            IList<JObject> dataList = new List<JObject>();
            StringBuilder tableBody = new StringBuilder();
            JObject item = new JObject();

            item = new JObject();
            fieldMaps.ToList().ForEach(field =>
            {
                item[field.FieldName] = field.FieldName + "1";
            });
            dataList.Add(item);


            dataList.ToList().ForEach(d =>
            {
                obj = string.Join("", fieldMaps.Select(field =>
                  string.Format("<div class='row'><div class='col-sm-6'><label>{0}</label></div><div class='col-sm-6'>{1}</div></div>", field.DisplayName, d[field.FieldName].ToString())
                ));
            });
            return obj;
        }

        public string GetHTMLPreviewData(TenantEntity entity, IList<EntityFieldMap> fieldMaps, string widgetSettings)
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

        public string GetBarLineChartPreviewData(TenantEntity entity, string chartTitle, DynamicWidgetLineGraph lineGraphDetails, string chartType, string theme)
        {
            string obj = string.Empty;
            string colorTheme = string.Empty;

            #region Set Color Theme

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

            #endregion

            StringBuilder tableBody = new StringBuilder();
            IList<ChartSeries> series = new List<ChartSeries>();
            IList<string> xAxis = new List<string>();

            for (int i = 1; i < 5; i++)
            {
                xAxis.Add(lineGraphDetails.XAxis + i.ToString());
            }

            Random random = new Random();

            lineGraphDetails.Details.ToList().ForEach(field =>
            {
                List<decimal> data = new List<decimal>();
                data.Add(random.Next(1, 5));
                data.Add(random.Next(1, 5));
                data.Add(random.Next(1, 5));
                data.Add(random.Next(1, 5));

                series.Add(new ChartSeries()
                {
                    name = field.DisplayName,
                    data = data,
                    type = chartType
                });
            });
            GraphChartData chartData = new GraphChartData();
            chartData.series = series;
            chartData.xAxis = xAxis;
            chartData.title = new ChartTitle();
            chartData.title.text = chartTitle;
            chartData.color = colorTheme;
            obj = JsonConvert.SerializeObject(chartData);
            return obj;
        }

        public string GetPieChartPreviewData(TenantEntity entity, string chartTitle, PieChartSettingDetails pieChartSettingDetails, string chartType, string theme, IList<EntityFieldMap> fieldMaps)
        {
            string obj = string.Empty;
            string colorTheme = string.Empty;

            #region Set Color Theme
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
            #endregion

            StringBuilder tableBody = new StringBuilder();
            IList<PieChartSeries> series = new List<PieChartSeries>();
            IList<string> xAxis = new List<string>();

            Random random = new Random();
            IList<PieChartData> data = new List<PieChartData>();

            string seriesName = fieldMaps.Where(item => item.Identifier.ToString() == pieChartSettingDetails.PieSeries).ToList().FirstOrDefault().Name;
            series.Add(new PieChartSeries()
            {
                name = pieChartSettingDetails.PieSeriesName
            });
            int remainingPercentage = 100;
            for (int i = 1; i < 5; i++)
            {
                data.Add(new PieChartData
                {
                    name = pieChartSettingDetails.PieSeriesName + i.ToString(),
                    y = random.Next(1, remainingPercentage)
                });
                remainingPercentage = 100 - (int)data.Sum(item => item.y);
            }
            series[0].data = data;

            PiChartGraphData chartData = new PiChartGraphData();
            chartData.series = series;
            chartData.title = new ChartTitle();
            chartData.title.text = chartTitle;
            chartData.color = colorTheme;
            obj = JsonConvert.SerializeObject(chartData);
            return obj;
        }


        public string GetPreviewData(TenantEntity entity, string chartTitle, string widgetSettings, string widgetType, string theme, IList<EntityFieldMap> fieldMaps)
        {
            string previewData = string.Empty;
            if (widgetType == HtmlConstants.TABLE_DYNAMICWIDGET)
            {
                List<DynamicWidgetTableEntity> tableFields = JsonConvert.DeserializeObject<List<DynamicWidgetTableEntity>>(widgetSettings);
                previewData = this.GetTablePreviewData(entity, tableFields);
            }
            else if (widgetType == HtmlConstants.FORM_DYNAMICWIDGET)
            {
                List<DynamicWidgetFormEntity> formEntity = JsonConvert.DeserializeObject<List<DynamicWidgetFormEntity>>(widgetSettings);
                previewData = this.GetFormPreviewData(entity, formEntity);
            }
            else if (widgetType == HtmlConstants.HTML_DYNAMICWIDGET)
            {

            }
            else if (widgetType == HtmlConstants.LINEGRAPH_DYNAMICWIDGET)
            {
                DynamicWidgetLineGraph lineGraphDetails = JsonConvert.DeserializeObject<DynamicWidgetLineGraph>(widgetSettings);

                previewData = this.GetBarLineChartPreviewData(entity, chartTitle, lineGraphDetails, "line", theme);

            }
            else if (widgetType == HtmlConstants.BARGRAPH_DYNAMICWIDGET)
            {
                DynamicWidgetLineGraph lineGraphDetails = JsonConvert.DeserializeObject<DynamicWidgetLineGraph>(widgetSettings);

                previewData = this.GetBarLineChartPreviewData(entity, chartTitle, lineGraphDetails, "column", theme);
            }
            else if (widgetType == HtmlConstants.PICHART_DYNAMICWIDGET)
            {
                PieChartSettingDetails pieChartSetting = JsonConvert.DeserializeObject<PieChartSettingDetails>(widgetSettings);

                previewData = this.GetPieChartPreviewData(entity, chartTitle, pieChartSetting, "", theme, fieldMaps);
            }

            return previewData;
        }
        #endregion
    }
}