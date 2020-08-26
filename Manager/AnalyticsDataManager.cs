// <copyright file="AnalyticsDataManager.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2018 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------

namespace nIS
{
    #region References

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Unity;

    #endregion


    /// <summary>
    /// This class implements manager layer of asset library manager.
    /// </summary>
    public class AnalyticsDataManager
    {
        #region Private members
        /// <summary>
        /// The unity container
        /// </summary>
        private IUnityContainer unityContainer = null;

        /// <summary>
        /// The asset library repository.
        /// </summary>
        IAnalyticsDataRepository AnalyticsDataRepository = null;


        #endregion

        #region Constructor

        /// <summary>
        /// This is constructor for asset library manager, which initialise
        /// asset library repository.
        /// </summary>
        /// <param name="container">IUnity container implementation object.</param>
        public AnalyticsDataManager(IUnityContainer unityContainer)
        {
            try
            {
                this.unityContainer = unityContainer;
                this.AnalyticsDataRepository = this.unityContainer.Resolve<IAnalyticsDataRepository>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Public Methods

        #region Source data Functions

        /// <summary>
        /// This method will call get asset library method of repository.
        /// </summary>
        /// <param name="AnalyticsDataSearchParameter">The asset library search parameters.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns>
        /// Returns roles if found for given parameters, else return null
        /// </returns>
        public IList<AnalyticsData> GetAnalyticsData(AnalyticsSearchParameter searchParameter, string tenantCode)
        {
            IList<AnalyticsData> assetLibraries = new List<AnalyticsData>();
            try
            {

                assetLibraries = this.AnalyticsDataRepository.GetAnalyticsData(searchParameter, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return assetLibraries;
        }
        /// <summary>
        /// This method will call add asset library method of repository.
        /// </summary>
        /// <param name="assetLibraries">Asset library are to be add.</param>
        /// <param name="tenantCode">Tenant code of asset library.</param>
        /// <returns>
        /// Returns true if entities added successfully, false otherwise.
        /// </returns>
        public bool Save(IList<AnalyticsData> settings, string tenantCode)
        {
            bool result = false;
            try
            {
                result = AnalyticsDataRepository.Save(settings, tenantCode);


                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Charts

        /// <summary>
        /// This method will call get asset library method of repository.
        /// </summary>
        /// <param name="AnalyticsDataSearchParameter">The asset library search parameters.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns>
        /// Returns roles if found for given parameters, else return null
        /// </returns>
        public IList<WidgetVisitorPieChartData> GetPieChartWidgeVisitor(AnalyticsSearchParameter searchParameter, string tenantCode)
        {
            IList<WidgetVisitorPieChartData> visitorPieChartDatas = new List<WidgetVisitorPieChartData>();
            IList<AnalyticsData> AnalyticsData = new List<AnalyticsData>();
            try
            {
                AnalyticsData = this.AnalyticsDataRepository.GetAnalyticsData(searchParameter, tenantCode);
                List<string> distinctwidgets;
                decimal totalRecord = AnalyticsData.Count();
                distinctwidgets = AnalyticsData.Where(item => item.Widgetname != string.Empty).Select(item => item.Widgetname).ToList().Distinct().ToList();
                distinctwidgets.ToList().ForEach(item =>
                {
                    WidgetVisitorPieChartData widgetData = new WidgetVisitorPieChartData();
                    decimal widgetCount = AnalyticsData.Where(data => data.Widgetname == item).Count();
                    //var value = items.length / totalRecord * 100;
                    widgetData.name = item;
                    widgetData.y = widgetCount / totalRecord * 100;
                    visitorPieChartDatas.Add(widgetData);
                });
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return visitorPieChartDatas;
        }

        /// <summary>
        /// This method will call get asset library method of repository.
        /// </summary>
        /// <param name="AnalyticsDataSearchParameter">The asset library search parameters.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns>
        /// Returns roles if found for given parameters, else return null
        /// </returns>
        public PageWidgetVistorData GetPageWidgetVisitor(AnalyticsSearchParameter searchParameter, string tenantCode)
        {
            IList<WidgetVisitorPieChartData> visitorPieChartDatas = new List<WidgetVisitorPieChartData>();
            IList<AnalyticsData> AnalyticsData = new List<AnalyticsData>();
            IList<long> values = new List<long>();
            PageWidgetVistorData data = new PageWidgetVistorData();

            try
            {
                AnalyticsData = this.AnalyticsDataRepository.GetAnalyticsData(searchParameter, tenantCode);
                List<string> distinctwidgets;
                List<long> distinctCustomers;
                decimal totalRecord = AnalyticsData.Count();
                distinctCustomers = AnalyticsData.Select(item => item.CustomerId).ToList().Distinct().ToList();
                distinctwidgets = AnalyticsData.Where(item => item.Widgetname != string.Empty).Select(item => item.Widgetname).ToList().Distinct().ToList();

                distinctwidgets.ToList().ForEach(Widget =>
                {
                    long value = 0;
                    distinctCustomers.ToList().ForEach(customer =>
                    {

                        if (AnalyticsData.Any(item => item.Widgetname == Widget && item.CustomerId == customer))
                        {
                            value++;
                        }
                    });
                    values.Add(value);
                });
                data.values = values;
                data.widgetNames = distinctwidgets;
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return data;
        }

        /// <summary>
        /// This method will call get asset library method of repository.
        /// </summary>
        /// <param name="AnalyticsDataSearchParameter">The asset library search parameters.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns>
        /// Returns roles if found for given parameters, else return null
        /// </returns>
        public DatewiseVisitor GetDatewiseVisitor(AnalyticsSearchParameter searchParameter, string tenantCode)
        {
            IList<WidgetVisitorPieChartData> visitorPieChartDatas = new List<WidgetVisitorPieChartData>();
            IList<AnalyticsData> AnalyticsData = new List<AnalyticsData>();
            IList<long> values = new List<long>();
            DatewiseVisitor data = new DatewiseVisitor();

            try
            {
                AnalyticsData = this.AnalyticsDataRepository.GetAnalyticsData(searchParameter, tenantCode);
                List<string> distinctwidgets;
                List<long> distinctCustomers;
                decimal totalRecord = AnalyticsData.Count();
                distinctCustomers = AnalyticsData.Select(item => item.CustomerId).ToList().Distinct().ToList();
                distinctwidgets = AnalyticsData.Where(item => item.Widgetname != string.Empty).Select(item => item.Widgetname).ToList().Distinct().ToList();
                var pageTypeGroup = AnalyticsData.Where(item => item.PageTypeName != null && item.PageTypeName != "").GroupBy(item => item.PageTypeName).ToList();
                IList<DatewiseVisitorSeries> series = new List<DatewiseVisitorSeries>();

                pageTypeGroup.ToList().ForEach(item =>
                {
                    series.Add(new DatewiseVisitorSeries { name = item.Key });
                });
                var dateGroup = AnalyticsData.GroupBy(item => item.EventDate.Date.ToShortDateString()).ToList();
                dateGroup.ToList().ForEach(date =>
                {
                    data.dates.Add(date.Key.ToString());
                });
                data.datewiseVisitorSeries = series;

                series.ToList().ForEach(s =>
                {
                    dateGroup.ToList().ForEach(date =>
                    {
                        long value = 0;
                        distinctCustomers.ToList().ForEach(c =>
                            {
                                if (AnalyticsData.Any(item => item.EventDate.Date.ToShortDateString() == date.Key.ToString()
                                && item.PageTypeName == s.name && item.CustomerId == c))
                                {
                                    value++;
                                }
                            });
                        s.data.Add(value);
                    });
                });
                data.datewiseVisitorSeries = series;
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return data;
        }
        /// <summary>
        /// This method will call get asset library method of repository.
        /// </summary>
        /// <param name="AnalyticsDataSearchParameter">The asset library search parameters.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns>
        /// Returns roles if found for given parameters, else return null
        /// </returns>
        public VisitorForDay GeVisitorForDay(AnalyticsSearchParameter searchParameter, string tenantCode)
        {
            IList<WidgetVisitorPieChartData> visitorPieChartDatas = new List<WidgetVisitorPieChartData>();
            IList<AnalyticsData> AnalyticsData = new List<AnalyticsData>();
            IList<long> values = new List<long>();
            VisitorForDay data = new VisitorForDay();

            try
            {
                AnalyticsData = this.AnalyticsDataRepository.GetAnalyticsData(searchParameter, tenantCode);
                List<long> distinctCustomers;
                decimal totalRecord = AnalyticsData.Count();
                distinctCustomers = AnalyticsData.Select(item => item.CustomerId).ToList().Distinct().ToList();
                IList<VisitorForDaySeries> series = new List<VisitorForDaySeries>();
                var time = AnalyticsData.GroupBy(item => item.EventDate.Hour).ToList();

                DateTime dt = searchParameter.StartDate;
                time.ToList().ForEach(date =>
                {

                    data.time.Add(date.Key.ToString());
                    var hour = AnalyticsData.Where(i => i.EventDate.Hour.ToString() == date.Key.ToString()).FirstOrDefault().EventDate.Hour;

                    data.dateTime.Add(new DateTime(dt.Year, dt.Month, dt.Day, hour, 0, 0));
                });

                data.time.ToList().ForEach(s =>
                {
                    long value = 0;
                    distinctCustomers.ToList().ForEach(customer =>
                    {

                        if (AnalyticsData.Any(item => item.EventDate.Hour.ToString() == s && item.CustomerId == customer))
                        {
                            value++;
                        }
                    });
                    values.Add(value);
                });
                series.Add(new VisitorForDaySeries { name = "Count", data = values });
                data.series = series;
                if (data.time.Count > 0)
                {
                    data.time.ToList().ForEach(item =>
                    {
                        item.Concat(":00");
                    });
                    for (int i = 0; i < data.time.Count; i++)
                    {
                        data.time[i] = data.time[i] + ":00";
                    }
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return data;
        }

        #endregion
        #endregion

        #region Private Methods


        #region Asset Libraries


        /// <summary>
        /// This method is responsible for validate asset s.
        /// </summary>
        /// <param name="assets"></param>
        /// <param name="tenantCode"></param>
        private void IsValidAnalyticsData(AnalyticsData item, string tenantCode)
        {
            try
            {

                InvalidAssetException invalidAssetException = new InvalidAssetException(tenantCode);

                try
                {
                    item.IsValid();
                }
                catch (Exception ex)
                {
                    invalidAssetException.Data.Add(item.Identifier, ex.Data);
                }


                if (invalidAssetException.Data.Count > 0)
                {
                    throw invalidAssetException;
                }
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
