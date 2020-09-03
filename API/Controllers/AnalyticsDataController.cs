// <copyright file="AnalyticsDataController.cs" company="Websym Solutions Pvt Ltd">
// Copyright (c) 2018 Websym Solutions Pvt Ltd.
// </copyright>
// -----------------------------------------------------------------------  

namespace nIS
{
    #region References

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Web;
    using System.Web.Http;
    using System.Web.Http.Cors;
    using Unity;
    using OfficeOpenXml;
    using OfficeOpenXml.Style;

    #endregion

    /// <summary>
    /// This class represent api controller for asset library
    /// </summary>
    [EnableCors("*", "*", "*", "*")]
    [RoutePrefix("AnalyticsData")]
    public class AnalyticsDataController : ApiController
    {
        #region Private Members

        /// <summary>
        /// The asset library manager object.
        /// </summary>
        private AnalyticsDataManager AnalyticsDataManager = null;

        #endregion

        #region Constructor

        public AnalyticsDataController(IUnityContainer unityContainer)
        {
            this.AnalyticsDataManager = new AnalyticsDataManager(unityContainer);
        }

        #endregion

        #region Public Methods

        #region Analytics Data

        /// <summary>
        /// This method helps to get analytics data based on the search parameters.
        /// </summary>
        /// <param name="searchParameter"></param>
        /// <returns>List of analytics data</returns>
        [HttpPost]
        public IList<AnalyticsData> List(AnalyticsSearchParameter searchParameter)
        {
            IList<AnalyticsData> assetlibraries = new List<AnalyticsData>();
            try
            {

                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                assetlibraries = this.AnalyticsDataManager.GetAnalyticsData(searchParameter, tenantCode);
                HttpContext.Current.Response.AppendHeader("recordCount", this.AnalyticsDataManager.GetAnalyticsDataCount(searchParameter, tenantCode).ToString());
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return assetlibraries;
        }

        /// <summary>
        /// This method helps to get asset libraries list based on the search parameters.
        /// </summary>
        /// <param name="AnalyticsDataSearchParameter"></param>
        /// <returns>List of asset libraries</returns>
        [HttpPost]
        public IList<WidgetVisitorPieChartData> GetPieChartWidgeVisitor(AnalyticsSearchParameter searchParameter)
        {
            IList<WidgetVisitorPieChartData> widgetVisitorPieChartData = new List<WidgetVisitorPieChartData>();
            try
            {

                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                widgetVisitorPieChartData = this.AnalyticsDataManager.GetPieChartWidgeVisitor(searchParameter, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return widgetVisitorPieChartData;
        }

        /// <summary>
        /// This method helps to get asset libraries list based on the search parameters.
        /// </summary>
        /// <param name="AnalyticsDataSearchParameter"></param>
        /// <returns>List of asset libraries</returns>
        [HttpPost]
        public PageWidgetVistorData GetPageWidgetVisitor(AnalyticsSearchParameter searchParameter)
        {
            PageWidgetVistorData data = new PageWidgetVistorData();
            try
            {

                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                data = this.AnalyticsDataManager.GetPageWidgetVisitor(searchParameter, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return data;
        }

        /// <summary>
        /// This method helps to get asset libraries list based on the search parameters.
        /// </summary>
        /// <param name="AnalyticsDataSearchParameter"></param>
        /// <returns>List of asset libraries</returns>
        [HttpPost]
        public DatewiseVisitor GetDatewiseVisitor(AnalyticsSearchParameter searchParameter)
        {
            DatewiseVisitor data = new DatewiseVisitor();
            try
            {

                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                data = this.AnalyticsDataManager.GetDatewiseVisitor(searchParameter, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return data;
        }

        /// <summary>
        /// This method helps to get asset libraries list based on the search parameters.
        /// </summary>
        /// <param name="AnalyticsDataSearchParameter"></param>
        /// <returns>List of asset libraries</returns>
        [HttpPost]
        public VisitorForDay GeVisitorForDay(AnalyticsSearchParameter searchParameter)
        {
            VisitorForDay data = new VisitorForDay();
            try
            {

                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                data = this.AnalyticsDataManager.GeVisitorForDay(searchParameter, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return data;
        }

        /// <summary>
        /// This method helps to get asset libraries list based on the search parameters.
        /// </summary>
        /// <param name="AnalyticsDataSearchParameter"></param>
        /// <returns>List of asset libraries</returns>
        [HttpPost]
        [AllowAnonymous]
        public bool Save(IList<AnalyticsData> setting)
        {
            bool result;
            try
            {

                // string tenantCode = Helper.CheckTenantCode(Request.Headers);
                result = this.AnalyticsDataManager.Save(setting, ModelConstant.DEFAULT_TENANT_CODE);
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return result;
        }


        /// <summary>
        /// This method helps to download error message of failed customer records for given schedule log in excel
        /// </summary>
        /// <param name="ScheduleLogIndentifier">The schedule log detail object list</param>
        /// <returns>return excel file in the http response</returns>
        [HttpPost]
        public HttpResponseMessage ExportAnalyticsData(AnalyticsSearchParameter searchParameter)
        {
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage();
            string filePath = string.Empty;
            try
            {
                IList<AnalyticExportData> analyticExports = new List<AnalyticExportData>();
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                IList<AnalyticsData> analyticsData = this.AnalyticsDataManager.GetAnalyticsData(searchParameter, tenantCode);
                if(analyticsData.Count>0)
                {
                    analyticExports = analyticsData.Select(item => new AnalyticExportData
                    {
                        CustomerName=item.CustomerName,
                        PageName=item.PageName==string.Empty?"":item.PageName,
                        Widgetname = item.Widgetname == string.Empty ? "" : item.Widgetname,
                        EventDate=item.EventDate,
                        //EventType=item.EventType

                    }).ToList();
                }
                System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex("<[^>]*>");

                string fileName = "AnalyticsData_" + DateTime.Now.ToString().Replace("-", "_").Replace(":", "_").Replace(" ", "_").Replace('/', '_') + ".xlsx";
                string fileDirectoryPath = AppDomain.CurrentDomain.BaseDirectory + "\\Resources\\sampledata\\";
                if (!Directory.Exists(fileDirectoryPath))
                {
                    Directory.CreateDirectory(fileDirectoryPath);
                }

                filePath = fileDirectoryPath + fileName;
                if (!File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                bool result = this.GenerateExcel(analyticExports, filePath);
                if (result)
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                        {
                            byte[] bytes = new byte[file.Length];
                            file.Read(bytes, 0, (int)file.Length);
                            ms.Write(bytes, 0, (int)file.Length);

                            httpResponseMessage.Content = new ByteArrayContent(bytes.ToArray());
                            httpResponseMessage.Content.Headers.Add("x-filename", fileName);
                            httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                            httpResponseMessage.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                            httpResponseMessage.Content.Headers.ContentDisposition.FileName = fileName;
                            httpResponseMessage.StatusCode = HttpStatusCode.OK;
                            ms.Position = 0;
                        }
                    }
                }
                else
                {
                    httpResponseMessage.StatusCode = HttpStatusCode.InternalServerError;
                }

                return httpResponseMessage;
            }
            catch (IOException)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (filePath != string.Empty && File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
        }

        #endregion


        #endregion

        #region Private Methods

        /// <summary>
        /// This method helps to generate excel file using list of objects
        /// </summary>
        /// <param name="scheduleLogErrors">The schedule log detail object list</param>
        /// <param name="filePath">The location where to saved excel file</param>
        /// <returns>return true if excel generated successfully</returns>
        private bool GenerateExcel(IList<AnalyticExportData> analyticsData, string filePath)
        {
            try
            {
                ExcelPackage excel = new ExcelPackage();
                var workSheet = excel.Workbook.Worksheets.Add("Sheet1");
                workSheet.TabColor = System.Drawing.Color.Black;
                workSheet.DefaultRowHeight = 12;

                workSheet.Row(1).Height = 20;
                workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                workSheet.Row(1).Style.Font.Bold = true;

                workSheet.Cells["A1"].LoadFromCollection(Collection: analyticsData, PrintHeaders: true);

                int recordIndex = 2;
                foreach (var logError in analyticsData)
                {
                    workSheet.Cells[recordIndex, 4].Style.Numberformat.Format = "dd-MM-yyyy HH:mm";
                    recordIndex++;
                }

                excel.SaveAs(new FileInfo(filePath));
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

    }
}