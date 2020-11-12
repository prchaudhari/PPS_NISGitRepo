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
    using System.Threading.Tasks;
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
        /// This method will call get analytics data method of repository.
        /// </summary>
        /// <param name="searchParameter">The analytics search parameters.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns>
        /// Returns list of analytics data
        /// </returns>
        public IList<AnalyticsData> GetAnalyticsData(AnalyticsSearchParameter searchParameter, string tenantCode)
        {
            IList<AnalyticsData> analytcsdata = new List<AnalyticsData>();
            try
            {

                analytcsdata = this.AnalyticsDataRepository.GetAnalyticsData(searchParameter, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return analytcsdata;
        }

        /// <summary>
        /// This method is responsible to get analytics data count
        /// </summary>
        /// <param name="searchParameter"></param>
        /// <param name="tenantCode"></param>
        /// <returns>analytics data count</returns>
        public int GetAnalyticsDataCount(AnalyticsSearchParameter searchParameter, string tenantCode)
        {
            try
            {

                return this.AnalyticsDataRepository.GetAnalyticsDataCount(searchParameter, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }
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

                DateTime dt = DateTime.SpecifyKind(Convert.ToDateTime(searchParameter.StartDate), DateTimeKind.Utc);
                time.ToList().ForEach(date =>
                {

                    data.time.Add(date.Key.ToString());
                    var hour = AnalyticsData.Where(i => i.EventDate.Hour.ToString() == date.Key.ToString()).FirstOrDefault().EventDate.Hour;

                    var newDate = new DateTime(dt.Year, dt.Month, dt.Day, hour, 30, 0);
                    data.dateTime.Add(DateTime.SpecifyKind((DateTime)newDate, DateTimeKind.Utc));
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

        #region Group and Instance Manager Report
        public InstanceManagerReport GetInstanceManagerDashboard(DashboardReportSearchParameter dashboardReportSearchParameter, string tenantCode)
        {
            InstanceManagerReport instanceManagerReport = new InstanceManagerReport();
            ClientSearchParameter clientSearchPaarmeter = new ClientSearchParameter();
            clientSearchPaarmeter.SortParameter = new SortParameter();
            clientSearchPaarmeter.SortParameter.SortColumn = "TenantCode";
            //clientSearchPaarmeter.ParentTenantCode = tenantCode;
            clientSearchPaarmeter.TenantType = "Group";

            instanceManagerReport.UsersByGroup = new GraphChartData();
            PieChartSeries series = new PieChartSeries();
            series.name = "Groups";
            var tenantGroups = new ClientManager(this.unityContainer).GetClients(clientSearchPaarmeter, ModelConstant.DEFAULT_TENANT_CODE);
            int totalTenants = 0;
            if (tenantGroups?.Count > 0)
            {

                instanceManagerReport.TotalGroup = tenantGroups.Count;
                IList<PieChartData> pieChartDatas = new List<PieChartData>();

                GraphChartData usersByGroup = new GraphChartData();
                usersByGroup.title = new ChartTitle();
                usersByGroup.title.text = "Users by Group";
                usersByGroup.xAxis = new List<string>();
                usersByGroup.series = new List<ChartSeries>();

                instanceManagerReport.PublishedStatementByGroup = new PiChartGraphData();
                instanceManagerReport.PublishedStatementByGroup.title = new ChartTitle();
                instanceManagerReport.PublishedStatementByGroup.title.text = "Published statement by Tenant Groups";

                GraphChartData generatedStmtByGroup = new GraphChartData();
                generatedStmtByGroup.title = new ChartTitle();
                generatedStmtByGroup.title.text = "Generated statement by Groups";
                generatedStmtByGroup.series = new List<ChartSeries>();

                IList<decimal> userCount = new List<decimal>();
                IList<decimal> generatedStmtCount = new List<decimal>();
                tenantGroups.ToList().ForEach(group =>
                {
                    clientSearchPaarmeter = new ClientSearchParameter();
                    clientSearchPaarmeter.SortParameter = new SortParameter();
                    clientSearchPaarmeter.SortParameter.SortColumn = "TenantCode";
                    clientSearchPaarmeter.ParentTenantCode = group.TenantCode;
                    clientSearchPaarmeter.TenantType = "Tenant";
                    usersByGroup.xAxis.Add(group.TenantName);
                    generatedStmtByGroup.xAxis.Add(group.TenantName);
                    var tenants = new ClientManager(this.unityContainer).GetClients(clientSearchPaarmeter, ModelConstant.DEFAULT_TENANT_CODE);
                    if (tenants?.Count > 0)
                    {
                        int publishedStatementOfGroup = 0;
                        int totalUsers = 0;
                        int generatedStmt = 0;
                        totalTenants = totalTenants + tenants.Count;
                        tenants.ToList().ForEach(tenant =>
                        {
                            List<Task> tasks = new List<Task>();
                            tasks.Add(Task.Factory.StartNew(() =>
                            {
                                StatementManager statementManager = new StatementManager(this.unityContainer);
                                var publishedStatements = statementManager.GetStatementCount(new StatementSearchParameter
                                {
                                    Status = "Published",
                                    StartDate = dashboardReportSearchParameter.StartDate,
                                    EndDate = dashboardReportSearchParameter.EndDate,
                                    SortParameter = new SortParameter { SortColumn = ModelConstant.SORT_COLUMN }
                                }, tenant.TenantCode);
                                publishedStatementOfGroup = publishedStatementOfGroup + publishedStatements;
                            }));
                            tasks.Add(Task.Factory.StartNew(() =>
                            {
                                UserManager userManager = new UserManager(this.unityContainer);
                                var createdUser = userManager.GetUserCount(new UserSearchParameter
                                {
                                    SortParameter = new SortParameter { SortColumn = ModelConstant.SORT_COLUMN }
                                }, tenant.TenantCode);
                                if (createdUser > 0)
                                {
                                    totalUsers = totalUsers + createdUser;
                                }
                            }));
                            tasks.Add(Task.Factory.StartNew(() =>
                                {
                                    ScheduleLogManager scheduleLogManager = new ScheduleLogManager(this.unityContainer);
                                    var generatedStatements = scheduleLogManager.GetScheduleLogDetailsCount(new ScheduleLogDetailSearchParameter
                                    {
                                        Status = "Completed",
                                        SortParameter = new SortParameter { SortColumn = ModelConstant.SORT_COLUMN },
                                        StartDate = dashboardReportSearchParameter.StartDate,
                                        EndDate = dashboardReportSearchParameter.EndDate
                                    }, tenant.TenantCode);
                                    if (generatedStatements > 0)
                                    {
                                        generatedStmt = generatedStmt + generatedStatements;
                                    }
                                }));
                            Task.WaitAll(tasks.ToArray());
                        });
                        if (publishedStatementOfGroup > 0)
                        {
                            PieChartData data = new PieChartData();
                            data.name = group.TenantName;
                            data.y = publishedStatementOfGroup;
                            pieChartDatas.Add(data);
                        }

                        if (totalUsers > 0)
                        {
                            userCount.Add(totalUsers);

                        }
                        if (generatedStmt > 0)
                        {
                            generatedStmtCount.Add(generatedStmt);


                        }

                    }
                });
                series.data = pieChartDatas;
                usersByGroup.series.Add(new ChartSeries
                {
                    name = "Groups",
                    data = userCount,
                    type = "bar"
                });

                generatedStmtByGroup.series.Add(new ChartSeries
                {
                    name = "Groups",
                    data = generatedStmtCount,
                    type = "column"
                });

                instanceManagerReport.UsersByGroup = usersByGroup;
                instanceManagerReport.StatementByGroup = generatedStmtByGroup;

                instanceManagerReport.PublishedStatementByGroup.series = new List<PieChartSeries>();
                instanceManagerReport.PublishedStatementByGroup.series.Add(series);

            }
            instanceManagerReport.TotalTenant = totalTenants;

            return instanceManagerReport;
        }

        public GroupManagerReport GetGroupManagerDashboard(DashboardReportSearchParameter dashboardReportSearchParameter, string tenantCode)
        {
            GroupManagerReport groupManagerReport = new GroupManagerReport();
            ClientSearchParameter clientSearchPaarmeter = new ClientSearchParameter();
            clientSearchPaarmeter.SortParameter = new SortParameter();
            clientSearchPaarmeter.SortParameter.SortColumn = "TenantCode";
            clientSearchPaarmeter.ParentTenantCode = tenantCode;
            clientSearchPaarmeter.TenantType = "Tenant";

            groupManagerReport.UsersByGroup = new GraphChartData();
            PieChartSeries series = new PieChartSeries();
            series.name = "Groups";
            groupManagerReport.PublishedStatementByGroup = new PiChartGraphData();
            groupManagerReport.PublishedStatementByGroup.title = new ChartTitle();
            groupManagerReport.PublishedStatementByGroup.title.text = "Published statement by Tenant Groups";
            IList<PieChartData> pieChartDatas = new List<PieChartData>();

            GraphChartData usersByGroup = new GraphChartData();
            usersByGroup.title = new ChartTitle();
            usersByGroup.title.text = "Users by Group";
            usersByGroup.xAxis = new List<string>();
            usersByGroup.series = new List<ChartSeries>();


            GraphChartData generatedStmtByGroup = new GraphChartData();
            generatedStmtByGroup.title = new ChartTitle();
            generatedStmtByGroup.title.text = "Generated statement by Groups";
            generatedStmtByGroup.series = new List<ChartSeries>();

            IList<decimal> userCount = new List<decimal>();
            IList<decimal> generatedStmtCount = new List<decimal>();
            var tenants = new ClientManager(this.unityContainer).GetClients(clientSearchPaarmeter, ModelConstant.DEFAULT_TENANT_CODE);
            int totalTenants = 0;
            if (tenants?.Count > 0)
            {

                groupManagerReport.TotalTenant = tenants.Count;

                tenants.ToList().ForEach(tenant =>
                {
                    usersByGroup.xAxis.Add(tenant.TenantName);
                    generatedStmtByGroup.xAxis.Add(tenant.TenantName);

                    int publishedStatementOfGroup = 0;
                    int totalUsers = 0;
                    int generatedStmt = 0;
                    totalTenants = totalTenants + tenants.Count;

                    List<Task> tasks = new List<Task>();
                    StatementSearchParameter statementSearchParameter = new StatementSearchParameter
                    {
                        Status = "Published",

                        SortParameter = new SortParameter { SortColumn = ModelConstant.SORT_COLUMN }
                    };

                    //if (dashboardReportSearchParameter.StartDate != null)
                    //{
                    //    statementSearchParameter.StartDate = dashboardReportSearchParameter.StartDate;
                    //}
                    //if (dashboardReportSearchParameter.EndDate != null)
                    //{
                    //    statementSearchParameter.StartDate = dashboardReportSearchParameter.EndDate;
                    //}
                    tasks.Add(Task.Factory.StartNew(() =>
                    {
                        StatementManager statementManager = new StatementManager(this.unityContainer);
                        var publishedStatements = statementManager.GetStatementCount(new StatementSearchParameter
                        {
                            Status = "Published",
                            StartDate = dashboardReportSearchParameter.StartDate,
                            EndDate = dashboardReportSearchParameter.EndDate,
                            SortParameter = new SortParameter { SortColumn = ModelConstant.SORT_COLUMN }
                        }, tenant.TenantCode);
                        publishedStatementOfGroup = publishedStatementOfGroup + publishedStatements;
                    }));
                    tasks.Add(Task.Factory.StartNew(() =>
                    {
                        UserManager userManager = new UserManager(this.unityContainer);
                        var createdUser = userManager.GetUserCount(new UserSearchParameter
                        {
                            SortParameter = new SortParameter { SortColumn = ModelConstant.SORT_COLUMN }
                        }, tenant.TenantCode);
                        if (createdUser > 0)
                        {
                            totalUsers = totalUsers + createdUser;
                        }
                    }));
                    tasks.Add(Task.Factory.StartNew(() =>
                    {
                        ScheduleLogManager scheduleLogManager = new ScheduleLogManager(this.unityContainer);
                        var generatedStatements = scheduleLogManager.GetScheduleLogDetailsCount(new ScheduleLogDetailSearchParameter
                        {
                            Status = "Completed",
                            SortParameter = new SortParameter { SortColumn = ModelConstant.SORT_COLUMN },
                            StartDate = dashboardReportSearchParameter.StartDate,
                            EndDate = dashboardReportSearchParameter.EndDate
                        }, tenant.TenantCode);
                        if (generatedStatements > 0)
                        {
                            generatedStmt = generatedStmt + generatedStatements;
                        }
                    }));
                    Task.WaitAll(tasks.ToArray());

                    PieChartData data = new PieChartData();
                    data.name = tenant.TenantName;
                    data.y = publishedStatementOfGroup;
                    pieChartDatas.Add(data);
                    series.data = pieChartDatas;
                    userCount.Add(totalUsers);
                    generatedStmtCount.Add(generatedStmt);

                });
                usersByGroup.series.Add(new ChartSeries
                {
                    name = "Groups",
                    data = userCount,
                    type = "bar"
                });

                generatedStmtByGroup.series.Add(new ChartSeries
                {
                    name = "Groups",
                    data = generatedStmtCount,
                    type = "column"
                });

                groupManagerReport.UsersByGroup = usersByGroup;
                groupManagerReport.StatementByGroup = generatedStmtByGroup;

                groupManagerReport.PublishedStatementByGroup.series = new List<PieChartSeries>();
                groupManagerReport.PublishedStatementByGroup.series.Add(series);

            }

            return groupManagerReport;
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
