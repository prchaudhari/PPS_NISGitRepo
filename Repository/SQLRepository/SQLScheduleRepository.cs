// <copyright file="SQLScheduleRepository.cs" company="Websym Solutions Pvt. Ltd.">
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
    using System.IO;
    using System.Linq;
    using System.Linq.Dynamic;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Unity;
    #endregion

    /// <summary>
    /// This class represents the methods to perform operation with database for schedule entity.
    /// </summary>
    /// 
    public class SQLScheduleRepository : IScheduleRepository
    {
        #region Private Members

        /// <summary>
        /// The validation engine object
        /// </summary>
        IValidationEngine validationEngine = null;

        /// <summary>
        /// The connection string
        /// </summary>
        private string connectionString = string.Empty;

        /// <summary>
        /// The utility object
        /// </summary>
        private IUtility utility = null;

        /// <summary>
        /// The unity container
        /// </summary>
        private IUnityContainer unityContainer = null;

        /// <summary>
        /// The utility object
        /// </summary>
        private IConfigurationUtility configurationutility = null;

        /// <summary>
        /// The statement repository.
        /// </summary>
        private IStatementRepository statementRepository = null;

        /// <summary>
        /// The Page repository.
        /// </summary>
        private IPageRepository pageRepository = null;

        #endregion

        #region Constructor

        public SQLScheduleRepository(IUnityContainer unityContainer)
        {
            this.unityContainer = unityContainer;
            this.validationEngine = new ValidationEngine();
            this.utility = new Utility();
            this.configurationutility = new ConfigurationUtility(this.unityContainer);
            this.pageRepository = this.unityContainer.Resolve<IPageRepository>();
            this.statementRepository = this.unityContainer.Resolve<IStatementRepository>();
        }

        #endregion

        #region Public Methods

        #region Schedule 
        /// <summary>
        /// This method adds the specified list of schedule in the repository.
        /// </summary>
        /// <param name="schedules"></param>
        /// <param name="tenantCode"></param>
        /// <returns>
        /// True, if the schedule values are added successfully, false otherwise
        /// </returns>
        public bool AddSchedules(IList<Schedule> schedules, string tenantCode)
        {
            bool result = false;
            try
            {
                this.SetAndValidateConnectionString(tenantCode);

                if (this.IsDuplicateSchedule(schedules, "AddOperation", tenantCode))
                {
                    throw new DuplicateScheduleFoundException(tenantCode);
                }
                IList<ScheduleRecord> scheduleRecords = new List<ScheduleRecord>();
                schedules.ToList().ForEach(schedule =>
                {
                    scheduleRecords.Add(new ScheduleRecord()
                    {
                        Name = schedule.Name,
                        Description = schedule.Description,
                        DayOfMonth = schedule.DayOfMonth,
                        HourOfDay = schedule.HourOfDay,
                        MinuteOfDay = schedule.MinuteOfDay,
                        StartDate = schedule.StartDate,
                        EndDate = schedule.EndDate,
                        Status = schedule.Status,
                        IsDeleted = false,
                        TenantCode = tenantCode,
                        StatementId = schedule.Statement.Identifier,
                        IsExportToPDF = schedule.IsExportToPDF,
                        UpdateBy = schedule.UpdateBy.Identifier,
                        LastUpdatedDate = DateTime.UtcNow,
                    });
                });
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    nISEntitiesDataContext.ScheduleRecords.AddRange(scheduleRecords);
                    nISEntitiesDataContext.SaveChanges();
                    result = true;
                }
            }

            catch
            {
                throw;
            }

            return result;
        }

        /// <summary>
        /// This method helps to update already added schedules entry to database.
        /// </summary>
        /// <param name="schedules"></param>
        /// <param name="tenantCode"></param>
        /// <returns>
        /// True, if the schedule values are updated successfully,otherwise false
        /// </returns>
        public bool UpdateSchedules(IList<Schedule> schedules, string tenantCode)
        {
            bool result = false;
            try
            {
                this.SetAndValidateConnectionString(tenantCode);

                if (this.IsDuplicateSchedule(schedules, "UpdateOperation", tenantCode))
                {
                    throw new DuplicateScheduleFoundException(tenantCode);
                }

                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    StringBuilder query = new StringBuilder();
                    query.Append("(" + string.Join("or ", string.Join(",", schedules.Select(item => item.Identifier).Distinct()).ToString().Split(',').Select(item => string.Format("Id.Equals({0}) ", item))) + ") ");

                    IList<ScheduleRecord> scheduleRecords = nISEntitiesDataContext.ScheduleRecords.Where(query.ToString()).Select(item => item).AsQueryable().ToList();

                    if (scheduleRecords == null || scheduleRecords.Count <= 0 || scheduleRecords.Count() != string.Join(",", scheduleRecords.Select(item => item.Id).Distinct()).ToString().Split(',').Length)
                    {
                        throw new ScheduleNotFoundException(tenantCode);
                    }

                    schedules.ToList().ForEach(item =>
                    {
                        ScheduleRecord scheduleRecord = scheduleRecords.FirstOrDefault(data => data.Id == item.Identifier && data.TenantCode == tenantCode && data.IsDeleted == false);
                        scheduleRecord.Name = item.Name;
                        scheduleRecord.Description = item.Description;
                        scheduleRecord.DayOfMonth = item.DayOfMonth;
                        scheduleRecord.HourOfDay = item.HourOfDay;
                        scheduleRecord.MinuteOfDay = item.MinuteOfDay;
                        scheduleRecord.StartDate = item.StartDate;
                        scheduleRecord.EndDate = item.EndDate;
                        scheduleRecord.Status = item.Status;
                        scheduleRecord.IsDeleted = false;
                        scheduleRecord.TenantCode = tenantCode;
                        scheduleRecord.IsExportToPDF = item.IsExportToPDF;
                        scheduleRecord.StatementId = item.Statement.Identifier;
                        scheduleRecord.UpdateBy = item.UpdateBy.Identifier;
                        scheduleRecord.LastUpdatedDate = DateTime.UtcNow;

                    });

                    nISEntitiesDataContext.SaveChanges();
                }


                result = true;
            }

            catch (Exception exception)
            {
                throw exception;
            }

            return result;
        }

        /// <summary>
        /// Delete schedules from database
        /// </summary>
        /// <param name="schedules"></param>
        /// <param name="tenantCode"></param>
        /// <returns>True, if the schedule values are deleted successfully(soft delete), 
        /// otherwise false</returns>
        public bool DeleteSchedules(IList<Schedule> schedules, string tenantCode)
        {
            bool result = false;
            try
            {
                this.SetAndValidateConnectionString(tenantCode);

                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    StringBuilder query = new StringBuilder();
                    query.Append("(" + string.Join("or ", string.Join(",", schedules.Select(item => item.Identifier).Distinct()).ToString().Split(',').Select(item => string.Format("Id.Equals({0}) ", item))) + ") ");
                    query.Append("and IsDeleted.Equals(false)");
                    IList<ScheduleRecord> scheduleRecords = nISEntitiesDataContext.ScheduleRecords.Where(query.ToString()).Select(item => item).AsQueryable().ToList();
                    if (scheduleRecords == null || scheduleRecords.Count <= 0 || scheduleRecords.Count() != string.Join(",", scheduleRecords.Select(item => item.Id).Distinct()).ToString().Split(',').Length)
                    {
                        throw new ScheduleNotFoundException(tenantCode);
                    }
                    query = new StringBuilder();
                    query.Append("(" + string.Join("or ", string.Join(",", schedules.Select(item => item.Identifier).Distinct()).ToString().Split(',').Select(item => string.Format("ScheduleId.Equals({0}) ", item))) + ") ");

                    IList<ScheduleRunHistoryRecord> scheduleRunHistoryRecords = nISEntitiesDataContext.ScheduleRunHistoryRecords.Where(query.ToString()).Select(item => item).AsQueryable().ToList();
                    if (scheduleRunHistoryRecords.Count() > 0)
                    {
                        throw new RunningScheduleRefrenceException(tenantCode);
                    }

                    scheduleRecords.ToList().ForEach(item =>
                    {
                        item.IsDeleted = true;
                    });

                    nISEntitiesDataContext.SaveChanges();
                }
                result = true;
                return result;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// This method used to get the rolse based on search paramter.
        /// </summary>
        /// <param name="scheduleSearchParameter"></param>
        /// <param name="tenantCode"></param>
        /// <returns>List of schedules</returns>
        public IList<Schedule> GetSchedules(ScheduleSearchParameter scheduleSearchParameter, string tenantCode)
        {
            IList<Schedule> schedules = new List<Schedule>();
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                string whereClause = this.WhereClauseGenerator(scheduleSearchParameter, tenantCode);

                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    if (scheduleSearchParameter.StatementDefinitionName != null && scheduleSearchParameter.StatementDefinitionName != string.Empty)
                    {
                        StringBuilder queryString = new StringBuilder();
                        queryString.Append(string.Format("Name.Equals(\"{0}\")", scheduleSearchParameter.StatementDefinitionName));

                        queryString.Append(string.Format(" and IsDeleted.Equals(false)"));
                        var userRecordIds = nISEntitiesDataContext.StatementRecords.Where(queryString.ToString()).ToList().Select(itm => itm.Id).ToList();
                        if (userRecordIds.Count > 0)
                        {
                            queryString = new StringBuilder();
                            queryString.Append(" and (" + string.Join("or ", userRecordIds.Select(item => string.Format("StatementId.Equals({0}) ", item))) + ") ");
                            whereClause = whereClause + queryString.ToString();
                        }
                        else
                        {
                            return schedules;
                        }
                    }
                    IList<ScheduleRecord> scheduleRecords = new List<ScheduleRecord>();
                    if (scheduleSearchParameter.PagingParameter.PageIndex > 0 && scheduleSearchParameter.PagingParameter.PageSize > 0)
                    {
                        scheduleRecords = nISEntitiesDataContext.ScheduleRecords
                        .OrderBy(scheduleSearchParameter.SortParameter.SortColumn + " " + scheduleSearchParameter.SortParameter.SortOrder.ToString())
                        .Where(whereClause)
                        .Skip((scheduleSearchParameter.PagingParameter.PageIndex - 1) * scheduleSearchParameter.PagingParameter.PageSize)
                        .Take(scheduleSearchParameter.PagingParameter.PageSize)
                        .ToList();
                    }
                    else
                    {
                        scheduleRecords = nISEntitiesDataContext.ScheduleRecords
                        .Where(whereClause)
                        .OrderBy(scheduleSearchParameter.SortParameter.SortColumn + " " + scheduleSearchParameter.SortParameter.SortOrder.ToString().ToLower())
                        .ToList();
                    }

                    if (scheduleRecords != null && scheduleRecords.Count > 0)
                    {

                        schedules = scheduleRecords.Select(scheduleRecord => new Schedule()
                        {

                            Identifier = scheduleRecord.Id,
                            Name = scheduleRecord.Name,
                            Description = scheduleRecord.Description,
                            IsActive = !scheduleRecord.IsActive,
                            DayOfMonth = scheduleRecord.DayOfMonth,
                            HourOfDay = scheduleRecord.HourOfDay,
                            MinuteOfDay = scheduleRecord.MinuteOfDay,
                            StartDate = DateTime.SpecifyKind((DateTime)scheduleRecord.StartDate, DateTimeKind.Utc),
                            EndDate = scheduleRecord.EndDate != null ? DateTime.SpecifyKind((DateTime)scheduleRecord.EndDate, DateTimeKind.Utc) : DateTime.MinValue,
                            Status = scheduleRecord.Status,
                            IsExportToPDF = scheduleRecord.IsExportToPDF,
                            LastUpdatedDate = scheduleRecord.LastUpdatedDate,
                            Statement = new Statement { Identifier = scheduleRecord.StatementId }

                        }).ToList();
                        if (scheduleSearchParameter.IsStatementDefinitionRequired == true)
                        {
                            StringBuilder statementIdentifier = new StringBuilder();
                            statementIdentifier.Append("(" + string.Join(" or ", scheduleRecords.Select(item => string.Format("Id.Equals({0})", item.StatementId))) + ")");
                            IList<StatementRecord> statementRecords = new List<StatementRecord>();
                            statementRecords = nISEntitiesDataContext.StatementRecords.Where(statementIdentifier.ToString()).ToList();
                            schedules.ToList().ForEach(schedule =>
                            {
                                if (statementRecords.Where(item => item.Id == schedule.Statement.Identifier)?.FirstOrDefault() != null)
                                {
                                    schedule.Statement.Name = statementRecords.Where(item => item.Id == schedule.Statement.Identifier)?.FirstOrDefault().Name;
                                }
                            });
                        }
                    }


                }
            }

            catch (Exception ex)
            {
                throw ex;
            }

            return schedules;
        }

        /// <summary>
        /// This method helps to get count of schedules.
        /// </summary>
        /// <param name="scheduleSearchParameter"></param>
        /// <param name="tenantCode"></param>
        /// <returns></returns>
        public int GetScheduleCount(ScheduleSearchParameter scheduleSearchParameter, string tenantCode)
        {
            int scheduleCount = 0;
            string whereClause = this.WhereClauseGenerator(scheduleSearchParameter, tenantCode);
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    scheduleCount = nISEntitiesDataContext.ScheduleRecords.Where(whereClause.ToString()).Count();
                }
            }
            catch (Exception)
            {
                throw;
            }

            return scheduleCount;
        }

        #region Activate Schedule

        /// <summary>
        /// This method helps to activate the schedule
        /// </summary>
        /// <param name="scheduleIdentifier">The schedule identifier</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>True if schedule activated successfully false otherwise</returns>
        public bool ActivateSchedule(long scheduleIdentifier, string tenantCode)
        {
            bool result = false;
            try
            {
                this.SetAndValidateConnectionString(tenantCode);

                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    IList<ScheduleRecord> scheduleRecords = nISEntitiesDataContext.ScheduleRecords.Where(itm => itm.Id == scheduleIdentifier).ToList();
                    if (scheduleRecords == null || scheduleRecords.Count <= 0)
                    {
                        throw new ScheduleNotFoundException(tenantCode);
                    }

                    scheduleRecords.ToList().ForEach(item =>
                    {
                        item.IsActive = false;
                    });

                    nISEntitiesDataContext.SaveChanges();
                }
                result = true;
                return result;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        #endregion

        #region Deactivate Schedule

        /// <summary>
        /// This method helps to deactivate the schedule
        /// </summary>
        /// <param name="scheduleIdentifier">The schedule identifier</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>True if schedule deactivated successfully false otherwise</returns>
        public bool DeActivateSchedule(long scheduleIdentifier, string tenantCode)
        {
            bool result = false;
            try
            {
                this.SetAndValidateConnectionString(tenantCode);

                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    IList<ScheduleRecord> scheduleRecords = nISEntitiesDataContext.ScheduleRecords.Where(itm => itm.Id == scheduleIdentifier).ToList();
                    if (scheduleRecords == null || scheduleRecords.Count <= 0)
                    {
                        throw new ScheduleNotFoundException(tenantCode);
                    }

                    scheduleRecords.ToList().ForEach(item =>
                    {
                        item.IsActive = true;
                    });

                    nISEntitiesDataContext.SaveChanges();
                }
                result = true;
                return result;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// This method helps to run the schedule
        /// </summary>
        /// <param name="baseURL">The schedule identifier</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>True if schedules runs successfully, false otherwise</returns>
        public bool RunSchedule(string baseURL, string tenantCode)
        {
            bool scheduleRunStatus = false;
            StringBuilder htmlbody = new StringBuilder();
            StringBuilder finalHtml = new StringBuilder();
            IList<ScheduleRecord> schedules = new List<ScheduleRecord>();
            var currentDate = DateTime.Now;

            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    schedules = nISEntitiesDataContext.ScheduleRecords.Where(item => item.StartDate <= currentDate && (item.EndDate == null || item.EndDate >= currentDate) && item.DayOfMonth == currentDate.Day && currentDate.Hour == item.HourOfDay && currentDate.Minute == item.MinuteOfDay && item.IsActive && !item.IsDeleted).ToList();
                }
                if (schedules.Count != 0)
                {
                    StringBuilder query = new StringBuilder();
                    query.Append("(" + string.Join("or ", string.Join(",", schedules.Select(item => item.Id).Distinct()).ToString().Split(',').Select(item => string.Format("Id.Equals({0}) ", item))) + ") ");

                    using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                    {
                        IList<ScheduleRecord> scheduleRecords = nISEntitiesDataContext.ScheduleRecords.Where(query.ToString()).Select(item => item).AsQueryable().ToList();
                        scheduleRecords.ToList().ForEach(item =>
                        {
                            item.Status = ScheduleStatus.InProgress.ToString();
                        });
                        nISEntitiesDataContext.SaveChanges();
                    }

                    BatchMasterRecord batchMaster = new BatchMasterRecord();
                    IList<BatchDetailRecord> batchDetails = new List<BatchDetailRecord>();
                    schedules.ToList().ForEach(schedule =>
                    {
                        RenderEngineRecord renderEngine = new RenderEngineRecord();
                        using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                        {
                            batchMaster = nISEntitiesDataContext.BatchMasterRecords.Where(item => item.ScheduleId == schedule.Id)?.ToList()?.FirstOrDefault();
                        }
                        if (batchMaster != null)
                        {
                            ScheduleRunHistoryRecord runHistory = new ScheduleRunHistoryRecord();
                            runHistory.StartDate = DateTime.Now;
                            runHistory.TenantCode = tenantCode;
                            runHistory.ScheduleId = schedule.Id;

                            ScheduleLogRecord scheduleLog = new ScheduleLogRecord();
                            scheduleLog.ScheduleId = schedule.Id;
                            scheduleLog.ScheduleName = schedule.Name;                            
                            scheduleLog.NumberOfRetry = 1;
                            scheduleLog.CreationDate = DateTime.Now;
                            scheduleLog.TenantCode = tenantCode;
                            scheduleLog.Status = ScheduleLogStatus.InProgress.ToString();
                            using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                            {
                                nISEntitiesDataContext.ScheduleLogRecords.Add(scheduleLog);
                                nISEntitiesDataContext.SaveChanges();

                                batchDetails = nISEntitiesDataContext.BatchDetailRecords.Where(item => item.BatchId == batchMaster.Id)?.ToList();
                            }

                            Statement statement = new Statement();
                            StatementSearchParameter statementSearchParameter = new StatementSearchParameter
                            {
                                Identifier = schedule.StatementId,
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
                            var statements = this.statementRepository.GetStatements(statementSearchParameter, tenantCode);
                            if (statements.Count > 0)
                            {
                                statement = statements[0];
                                runHistory.StatementId = statement.Identifier;

                                //Start to generate common html string
                                var statementPages = statements[0].StatementPages.OrderBy(it => it.SequenceNumber).ToList();
                                if (statementPages.Count > 0)
                                {
                                    string navbarHtml = HtmlConstants.NAVBAR_HTML.Replace("{{BrandLogo}}", "../common/images/absa-logo.png");
                                    navbarHtml = navbarHtml.Replace("{{Today}}", DateTime.Now.ToString("dd MMM yyyy"));
                                    StringBuilder navItemList = new StringBuilder();
                                    htmlbody.Append(HtmlConstants.CONTAINER_DIV_HTML_HEADER);

                                    statement.Pages = new List<Page>();
                                    for (int i = 0; i < statementPages.Count; i++)
                                    {
                                        PageSearchParameter pageSearchParameter = new PageSearchParameter
                                        {
                                            Identifier = statementPages[i].ReferencePageId,
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
                                        var pages = this.pageRepository.GetPages(pageSearchParameter, tenantCode);
                                        if (pages.Count != 0)
                                        {
                                            for (int j = 0; j < pages.Count; j++)
                                            {
                                                var page = pages[j];
                                                statement.Pages.Add(page);
                                                string tabClassName = Regex.Replace((page.DisplayName + " " + page.Version), @"\s+", "-");
                                                navItemList.Append(" <li class='nav-item'><a class='nav-link " + (i == 0 ? "active" : "") + " " + tabClassName + "' href='javascript:void(0);'>" + page.DisplayName + "</a> </li> ");
                                                string ExtraClassName = i > 0 ? "d-none " + tabClassName : tabClassName;
                                                string widgetHtmlHeader = HtmlConstants.WIDGET_HTML_HEADER.Replace("{{ExtraClass}}", ExtraClassName);
                                                widgetHtmlHeader = widgetHtmlHeader.Replace("{{DivId}}", tabClassName);
                                                htmlbody.Append(widgetHtmlHeader);

                                                int tempRowWidth = 0; // variable to check col-lg div length (bootstrap)
                                                int max = 0;
                                                if (page.PageWidgets.Count > 0)
                                                {
                                                    var completelst = new List<PageWidget>(page.PageWidgets);
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
                                                            for (int x = 0; x < mergedlst.Count; x++)
                                                            {
                                                                if (tempRowWidth == 0)
                                                                {
                                                                    htmlbody.Append("<div class='row'>"); // to start new row class div 
                                                                    isRowComplete = false;
                                                                }
                                                                int divLength = (mergedlst[x].Width * 12) / 20;
                                                                tempRowWidth = tempRowWidth + divLength;

                                                                // If current col-lg class length is greater than 12, 
                                                                //then end parent row class div and then start new row class div
                                                                if (tempRowWidth > 12)
                                                                {
                                                                    tempRowWidth = divLength;
                                                                    htmlbody.Append("</div>"); // to end row class div
                                                                    htmlbody.Append("<div class='row'>"); // to start new row class div
                                                                    isRowComplete = false;
                                                                }
                                                                htmlbody.Append("<div class='col-lg-" + divLength + "'>");
                                                                if (mergedlst[x].WidgetId == HtmlConstants.CUSTOMER_INFORMATION_WIDGET_ID)
                                                                {
                                                                    htmlbody.Append(HtmlConstants.CUSTOMER_INFORMATION_WIDGET_HTML.Replace("{{VideoSource}}", "{{VideoSource_" + statement.Identifier + "_" + page.Identifier + "_" + mergedlst[x].WidgetId + "}}"));
                                                                }
                                                                else if (mergedlst[x].WidgetId == HtmlConstants.ACCOUNT_INFORMATION_WIDGET_ID)
                                                                {
                                                                    htmlbody.Append(HtmlConstants.ACCOUNT_INFORMATION_WIDGET_HTML);
                                                                }
                                                                else if (mergedlst[x].WidgetId == HtmlConstants.IMAGE_WIDGET_ID)
                                                                {
                                                                    htmlbody.Append(HtmlConstants.IMAGE_WIDGET_HTML.Replace("{{ImageSource}}", "{{ImageSource_" + statement.Identifier + "_" + page.Identifier + "_" + mergedlst[x].WidgetId + "}}"));
                                                                }
                                                                else if (mergedlst[x].WidgetId == HtmlConstants.VIDEO_WIDGET_ID)
                                                                {
                                                                    htmlbody.Append(HtmlConstants.VIDEO_WIDGET_HTML.Replace("{{VideoSource}}", "{{VideoSource_" + statement.Identifier + "_" + page.Identifier + "_" + mergedlst[x].WidgetId + "}}"));
                                                                }
                                                                else if (mergedlst[x].WidgetId == HtmlConstants.SUMMARY_AT_GLANCE_WIDGET_ID)
                                                                {
                                                                    htmlbody.Append(HtmlConstants.SUMMARY_AT_GLANCE_WIDGET_HTML.Replace("{{AccountSummary}}", "{{AccountSummary_" + page.Identifier + "_" + mergedlst[x].WidgetId + "}}"));
                                                                }
                                                                else if (mergedlst[x].WidgetId == HtmlConstants.CURRENT_AVAILABLE_BALANCE_WIDGET_ID)
                                                                {
                                                                    string CurrentAvailBalanceWidget = HtmlConstants.SAVING_CURRENT_AVALABLE_BAL_WIDGET_HTML.Replace("{{TotalValue}}", "{{TotalValue_" + page.Identifier + "_" + mergedlst[x].WidgetId + "}}").Replace("{{TotalDeposit}}", "{{TotalDeposit_" + page.Identifier + "_" + mergedlst[x].WidgetId + "}}").Replace("{{TotalSpend}}", "{{TotalSpend_" + page.Identifier + "_" + mergedlst[x].WidgetId + "}}").Replace("{{Savings}}", "{{Savings_" + page.Identifier + "_" + mergedlst[x].WidgetId + "}}");
                                                                    htmlbody.Append(CurrentAvailBalanceWidget);
                                                                }
                                                                else if (mergedlst[x].WidgetId == HtmlConstants.SAVING_AVAILABLE_BALANCE_WIDGET_ID)
                                                                {
                                                                    string SavingAvailBalanceWidget = HtmlConstants.SAVING_CURRENT_AVALABLE_BAL_WIDGET_HTML.Replace("{{TotalValue}}", "{{TotalValue_" + page.Identifier + "_" + mergedlst[x].WidgetId + "}}").Replace("{{TotalDeposit}}", "{{TotalDeposit_" + page.Identifier + "_" + mergedlst[x].WidgetId + "}}").Replace("{{TotalSpend}}", "{{TotalSpend_" + page.Identifier + "_" + mergedlst[x].WidgetId + "}}").Replace("{{Savings}}", "{{Savings_" + page.Identifier + "_" + mergedlst[x].WidgetId + "}}");
                                                                    htmlbody.Append(SavingAvailBalanceWidget);
                                                                }
                                                                else if (mergedlst[x].WidgetId == HtmlConstants.SAVING_TRANSACTION_WIDGET_ID)
                                                                {
                                                                    htmlbody.Append(HtmlConstants.CURRENT_SAVING_TRANSACTION_WIDGET_HTML.Replace("{{AccountTransactionDetails}}", "{{AccountTransactionDetails_" + page.Identifier + "_" + mergedlst[x].WidgetId + "}}"));
                                                                }
                                                                else if (mergedlst[x].WidgetId == HtmlConstants.CURRENT_TRANSACTION_WIDGET_ID)
                                                                {
                                                                    htmlbody.Append(HtmlConstants.CURRENT_SAVING_TRANSACTION_WIDGET_HTML.Replace("{{AccountTransactionDetails}}", "{{AccountTransactionDetails_" + page.Identifier + "_" + mergedlst[x].WidgetId + "}}"));
                                                                }
                                                                else if (mergedlst[x].WidgetId == HtmlConstants.TOP_4_INCOME_SOURCES_WIDGET_ID)
                                                                {
                                                                    htmlbody.Append(HtmlConstants.TOP_4_INCOME_SOURCE_WIDGET_HTML.Replace("{{IncomeSourceList}}", "{{IncomeSourceList_" + page.Identifier + "_" + mergedlst[x].WidgetId + "}}"));
                                                                }
                                                                else if (mergedlst[x].WidgetId == HtmlConstants.ANALYTICS_WIDGET_ID)
                                                                {
                                                                    htmlbody.Append(HtmlConstants.ANALYTIC_WIDGET_HTML);
                                                                }
                                                                else if (mergedlst[x].WidgetId == HtmlConstants.SPENDING_TREND_WIDGET_ID)
                                                                {
                                                                    htmlbody.Append(HtmlConstants.SPENDING_TRENDS_WIDGET_HTML);
                                                                }
                                                                else if (mergedlst[x].WidgetId == HtmlConstants.SAVING_TREND_WIDGET_ID)
                                                                {
                                                                    htmlbody.Append(HtmlConstants.SAVING_TRENDS_WIDGET_HTML);
                                                                }
                                                                else if (mergedlst[x].WidgetId == HtmlConstants.REMINDER_AND_RECOMMENDATION_WIDGET_ID)
                                                                {
                                                                    htmlbody.Append(HtmlConstants.TOP_4_INCOME_SOURCE_WIDGET_HTML.Replace("{{ReminderAndRecommdationDataList}}", "{{ReminderAndRecommdationDataList_" + page.Identifier + "_" + mergedlst[x].WidgetId + "}}"));
                                                                }

                                                                // To end current col-lg class div
                                                                htmlbody.Append("</div>");

                                                                // if current col-lg class width is equal to 12 or end before complete col-lg-12 class, 
                                                                //then end parent row class div
                                                                if (tempRowWidth == 12 || (x == mergedlst.Count - 1))
                                                                {
                                                                    tempRowWidth = 0;
                                                                    htmlbody.Append("</div>"); //To end row class div
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
                                                        htmlbody.Append("</div>");
                                                    }
                                                }
                                                else
                                                {
                                                    htmlbody.Append(HtmlConstants.NO_WIDGET_MESSAGE_HTML);
                                                }
                                                htmlbody.Append(HtmlConstants.WIDGET_HTML_FOOTER);
                                            }
                                        }
                                        else
                                        {
                                            htmlbody.Append(HtmlConstants.NO_WIDGET_MESSAGE_HTML);
                                        }
                                    }

                                    htmlbody.Append(HtmlConstants.CONTAINER_DIV_HTML_FOOTER);
                                    navbarHtml = navbarHtml.Replace("{{NavItemList}}", navItemList.ToString());

                                    finalHtml.Append(HtmlConstants.HTML_HEADER);
                                    finalHtml.Append(navbarHtml);
                                    finalHtml.Append(htmlbody.ToString());
                                    //finalHtml.Append(HtmlConstants.TAB_NAVIGATION_SCRIPT);
                                    finalHtml.Append(HtmlConstants.HTML_FOOTER);
                                }
                            }

                            IList<CustomerMasterRecord> customerMasters = new List<CustomerMasterRecord>();
                            using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                            {
                                customerMasters = nISEntitiesDataContext.CustomerMasterRecords.Where(item => item.BatchId == batchMaster.Id).ToList();
                            }

                            if (customerMasters.Count > 0)
                            {
                                customerMasters.ToList().ForEach(customer =>
                                {
                                    using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                                    {
                                        renderEngine = nISEntitiesDataContext.RenderEngineRecords.Where(item => item.Id == 1).FirstOrDefault();
                                    }

                                    ScheduleLogDetailRecord logDetailRecord = GenerateStatement(customer, statement, finalHtml, batchMaster, batchDetails, baseURL);
                                    if (logDetailRecord != null)
                                    {
                                        logDetailRecord.ScheduleLogId = scheduleLog.Id;
                                        logDetailRecord.CustomerId = customer.Id;
                                        logDetailRecord.CustomerName = (customer.FirstName + " " + customer.MiddleName + " " + customer.LastName);
                                        logDetailRecord.ScheduleId = schedule.Id;
                                        logDetailRecord.RenderEngineId = renderEngine.Id; //To be change once render engine implmentation start
                                        logDetailRecord.RenderEngineName = renderEngine.Name;
                                        logDetailRecord.RenderEngineURL = renderEngine.URL;
                                        logDetailRecord.NumberOfRetry = 1;
                                        logDetailRecord.CreationDate = DateTime.Now;
                                        logDetailRecord.TenantCode = tenantCode;
                                        using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                                        {
                                            nISEntitiesDataContext.ScheduleLogDetailRecords.Add(logDetailRecord);
                                            nISEntitiesDataContext.SaveChanges();
                                        }
                                    }
                                });

                                string finalCommonStatementHtml = GenerateCommonStatement(statement, finalHtml, baseURL, batchMaster);
                                string fileName = "Statement_" + statement.Identifier + "_" + DateTime.Now.ToString().Replace("-", "_").Replace(":", "_").Replace(" ", "_").Replace('/', '_') + ".html";
                                string CommonStatementZipFilePath = this.utility.CreateAndWriteToZipFile(finalCommonStatementHtml, fileName, batchMaster.Id);
                                runHistory.FilePath = CommonStatementZipFilePath;
                            }

                            runHistory.EndDate = DateTime.Now;
                            using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                            {
                                ScheduleLogRecord scheduleLogRecord = nISEntitiesDataContext.ScheduleLogRecords.Where(item => item.Id == scheduleLog.Id).FirstOrDefault();
                                scheduleLogRecord.Status = ScheduleLogStatus.Completed.ToString();

                                var scheduleRecord = nISEntitiesDataContext.ScheduleRecords.Where(item => item.Id == schedule.Id).FirstOrDefault();
                                scheduleRecord.Status = ScheduleStatus.Completed.ToString();

                                nISEntitiesDataContext.ScheduleRunHistoryRecords.Add(runHistory);
                                nISEntitiesDataContext.SaveChanges();
                            }
                        }
                        else
                        {
                            using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                            {
                                var scheduleRecord = nISEntitiesDataContext.ScheduleRecords.Where(item => item.Id == schedule.Id).FirstOrDefault();
                                scheduleRecord.Status = ScheduleStatus.New.ToString();
                                nISEntitiesDataContext.SaveChanges();
                            }
                        }
                    });
                }
                scheduleRunStatus = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return scheduleRunStatus;
        }

        #endregion

        #endregion


        #region ScheduleRunHistory  Run History
        /// <summary>
        /// This method adds the specified list of schedule in the repository.
        /// </summary>
        /// <param name="schedules"></param>
        /// <param name="tenantCode"></param>
        /// <returns>
        /// True, if the schedule values are added successfully, false otherwise
        /// </returns>
        public bool AddScheduleRunHistorys(IList<ScheduleRunHistory> schedules, string tenantCode)
        {
            bool result = false;
            try
            {
                this.SetAndValidateConnectionString(tenantCode);

                IList<ScheduleRunHistoryRecord> scheduleRecords = new List<ScheduleRunHistoryRecord>();
                schedules.ToList().ForEach(schedule =>
                {
                    scheduleRecords.Add(new ScheduleRunHistoryRecord()
                    {
                        StartDate = schedule.StartDate,
                        EndDate = schedule.EndDate,
                        TenantCode = tenantCode,
                        ScheduleId = schedule.Schedule.Identifier
                    });
                });
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    nISEntitiesDataContext.ScheduleRunHistoryRecords.AddRange(scheduleRecords);
                    nISEntitiesDataContext.SaveChanges();
                    result = true;
                }
            }

            catch
            {
                throw;
            }

            return result;
        }

        /// <summary>
        /// This method used to get the rolse based on search paramter.
        /// </summary>
        /// <param name="scheduleSearchParameter"></param>
        /// <param name="tenantCode"></param>
        /// <returns>List of schedules</returns>
        public IList<ScheduleRunHistory> GetScheduleRunHistorys(ScheduleSearchParameter scheduleSearchParameter, string tenantCode)
        {
            IList<ScheduleRunHistory> schedules = new List<ScheduleRunHistory>();
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                string whereClause = this.WhereClauseGeneratorHistory(scheduleSearchParameter, tenantCode);

                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    if (scheduleSearchParameter.StatementDefinitionName != null && scheduleSearchParameter.StatementDefinitionName != string.Empty)
                    {
                        StringBuilder queryString = new StringBuilder();

                        queryString.Append(string.Format("Name.Contains(\"{0}\")", scheduleSearchParameter.StatementDefinitionName));

                        queryString.Append(string.Format(" and IsDeleted.Equals(false)"));
                        var userRecordIds = nISEntitiesDataContext.StatementRecords.Where(queryString.ToString()).ToList().Select(itm => itm.Id).ToList();
                        if (userRecordIds.Count > 0)
                        {
                            queryString = new StringBuilder();
                            queryString.Append(" and (" + string.Join("or ", userRecordIds.Select(item => string.Format("StatementId.Equals({0}) ", item))) + ") ");
                            whereClause = whereClause + queryString.ToString();
                        }
                        else
                        {
                            return schedules;
                        }
                    }

                    if (scheduleSearchParameter.ScheduleName != null && scheduleSearchParameter.ScheduleName != string.Empty)
                    {
                        StringBuilder queryString = new StringBuilder();
                        queryString.Append(string.Format("Name.Contains(\"{0}\")", scheduleSearchParameter.ScheduleName));

                        queryString.Append(string.Format(" and IsDeleted.Equals(false)"));
                        var userRecordIds = nISEntitiesDataContext.ScheduleRecords.Where(queryString.ToString()).ToList().Select(itm => itm.Id).ToList();
                        if (userRecordIds.Count > 0)
                        {
                            queryString = new StringBuilder();
                            queryString.Append(" and (" + string.Join("or ", userRecordIds.Select(item => string.Format("ScheduleId.Equals({0}) ", item))) + ") ");
                            whereClause = whereClause + queryString.ToString();
                        }
                        else
                        {
                            return schedules;
                        }
                    }
                    IList<ScheduleRunHistoryRecord> scheduleRecords = new List<ScheduleRunHistoryRecord>();
                    if (scheduleSearchParameter.PagingParameter.PageIndex > 0 && scheduleSearchParameter.PagingParameter.PageSize > 0)
                    {
                        scheduleRecords = nISEntitiesDataContext.ScheduleRunHistoryRecords
                        .OrderBy(scheduleSearchParameter.SortParameter.SortColumn + " " + scheduleSearchParameter.SortParameter.SortOrder.ToString())
                        .Where(whereClause)
                        .Skip((scheduleSearchParameter.PagingParameter.PageIndex - 1) * scheduleSearchParameter.PagingParameter.PageSize)
                        .Take(scheduleSearchParameter.PagingParameter.PageSize)
                        .ToList();
                    }
                    else
                    {
                        scheduleRecords = nISEntitiesDataContext.ScheduleRunHistoryRecords
                        .Where(whereClause)
                        .OrderBy(scheduleSearchParameter.SortParameter.SortColumn + " " + scheduleSearchParameter.SortParameter.SortOrder.ToString().ToLower())
                        .ToList();
                    }

                    if (scheduleRecords != null && scheduleRecords.Count > 0)
                    {

                        schedules = scheduleRecords.Select(scheduleRecord => new ScheduleRunHistory()
                        {

                            Identifier = scheduleRecord.Id,
                            StartDate = DateTime.SpecifyKind((DateTime)scheduleRecord.StartDate, DateTimeKind.Utc),
                            EndDate = scheduleRecord.EndDate != null ? DateTime.SpecifyKind((DateTime)scheduleRecord.EndDate, DateTimeKind.Utc) : DateTime.MinValue,
                            Schedule = new Schedule { Identifier = scheduleRecord.ScheduleId },
                            StatementFilePath = scheduleRecord.FilePath != null ? scheduleRecord.FilePath : string.Empty
                        }).ToList();

                        //StringBuilder scheduleIdentifier = new StringBuilder();
                        //scheduleIdentifier.Append("(" + string.Join(" or ", scheduleRecords.Select(item => string.Format("Id.Equals({0})", item.ScheduleId))) + ")");
                        //IList<ScheduleRecord> records = new List<ScheduleRecord>();
                        //records = nISEntitiesDataContext.ScheduleRecords.Where(scheduleIdentifier.ToString()).ToList();
                        //schedules.ToList().ForEach(schedule =>
                        //{
                        //    if (records.Where(item => item.Id == schedule.Schedule.Identifier)?.FirstOrDefault() != null)
                        //    {
                        //        ScheduleRecord record = new ScheduleRecord();
                        //        record = records.Where(item => item.Id == schedule.Schedule.Identifier)?.FirstOrDefault();
                        //        schedule.Schedule.Name = record.Name;
                        //        schedule.Schedule.DayOfMonth = record.DayOfMonth;

                        //    }
                        //});

                    }


                }
            }

            catch (Exception ex)
            {
                throw ex;
            }

            return schedules;
        }

        /// <summary>
        /// This method helps to get count of schedules.
        /// </summary>
        /// <param name="scheduleSearchParameter"></param>
        /// <param name="tenantCode"></param>
        /// <returns></returns>
        public int GetScheduleRunHistoryCount(ScheduleSearchParameter scheduleSearchParameter, string tenantCode)
        {
            int scheduleCount = 0;
            string whereClause = this.WhereClauseGeneratorHistory(scheduleSearchParameter, tenantCode);
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    scheduleCount = nISEntitiesDataContext.ScheduleRunHistoryRecords.Where(whereClause.ToString()).Count();
                }
            }
            catch (Exception)
            {
                throw;
            }

            return scheduleCount;
        }

        #endregion

        #endregion

        #region Private Methods

        /// <summary>
        /// Generate string for dynamic linq.
        /// </summary>
        /// <param name="searchParameter">Schedule search Parameters</param>
        /// <returns>
        /// Returns a string.
        /// </returns>
        private string WhereClauseGenerator(ScheduleSearchParameter searchParameter, string tenantCode)
        {
            StringBuilder queryString = new StringBuilder();

            if (searchParameter.SearchMode == SearchMode.Equals)
            {
                if (validationEngine.IsValidText(searchParameter.Identifier))
                {
                    queryString.Append("(" + string.Join("or ", searchParameter.Identifier.ToString().Split(',').Select(item => string.Format("Id.Equals({0}) ", item))) + ") and ");
                }
                if (validationEngine.IsValidText(searchParameter.Name))
                {
                    queryString.Append(string.Format("Name.Equals(\"{0}\") and ", searchParameter.Name));
                }
            }
            if (searchParameter.SearchMode == SearchMode.Contains)
            {

                if (validationEngine.IsValidText(searchParameter.Name))
                {
                    queryString.Append(string.Format("Name.Contains(\"{0}\") and ", searchParameter.Name));
                }
            }

            if (searchParameter.IsActive == null || searchParameter.IsActive == true)
            {
                queryString.Append(string.Format("IsDeleted.Equals(false) and "));
            }
            else if (searchParameter.IsActive != null && searchParameter.IsActive == false)
            {
                queryString.Append(string.Format("IsDeleted.Equals(true) and "));
            }
            if (this.validationEngine.IsValidDate(searchParameter.StartDate) && !this.validationEngine.IsValidDate(searchParameter.EndDate))
            {
                DateTime fromDateTime = DateTime.SpecifyKind(Convert.ToDateTime(searchParameter.StartDate), DateTimeKind.Utc);
                queryString.Append("StartDate >= DateTime(" + fromDateTime.Year + "," + fromDateTime.Month + "," + fromDateTime.Day + "," + fromDateTime.Hour + "," + fromDateTime.Minute + "," + fromDateTime.Second + ") and ");
            }

            if (this.validationEngine.IsValidDate(searchParameter.EndDate) && !this.validationEngine.IsValidDate(searchParameter.StartDate))
            {
                DateTime toDateTime = DateTime.SpecifyKind(Convert.ToDateTime(searchParameter.EndDate), DateTimeKind.Utc);
                queryString.Append("EndDate <= DateTime(" + toDateTime.Year + "," + toDateTime.Month + "," + toDateTime.Day + "," + toDateTime.Hour + "," + toDateTime.Minute + "," + toDateTime.Second + ") and ");
            }

            if (this.validationEngine.IsValidDate(searchParameter.StartDate) && this.validationEngine.IsValidDate(searchParameter.EndDate))
            {
                DateTime fromDateTime = DateTime.SpecifyKind(Convert.ToDateTime(searchParameter.StartDate), DateTimeKind.Utc);
                DateTime toDateTime = DateTime.SpecifyKind(Convert.ToDateTime(searchParameter.EndDate), DateTimeKind.Utc);

                queryString.Append("StartDate >= DateTime(" + fromDateTime.Year + "," + fromDateTime.Month + "," + fromDateTime.Day + "," + fromDateTime.Hour + "," + fromDateTime.Minute + "," + fromDateTime.Second + ") " +
                               "and EndDate <= DateTime(" + +toDateTime.Year + "," + toDateTime.Month + "," + toDateTime.Day + "," + toDateTime.Hour + "," + toDateTime.Minute + "," + toDateTime.Second + ") and ");
            }
            queryString.Append(string.Format("TenantCode.Equals(\"{0}\") ", tenantCode));
            return queryString.ToString();
        }

        /// <summary>
        /// Generate string for dynamic linq.
        /// </summary>
        /// <param name="searchParameter">Schedule search Parameters</param>
        /// <returns>
        /// Returns a string.
        /// </returns>
        private string WhereClauseGeneratorHistory(ScheduleSearchParameter searchParameter, string tenantCode)
        {
            StringBuilder queryString = new StringBuilder();
            if (validationEngine.IsValidText(searchParameter.Identifier))
            {
                queryString.Append("(" + string.Join("or ", searchParameter.Identifier.ToString().Split(',').Select(item => string.Format("ScheduleId.Equals({0}) ", item))) + ") and ");
            }
            if (validationEngine.IsValidText(searchParameter.ScheduleHistoryIdentifier))
            {
                queryString.Append("(" + string.Join("or ", searchParameter.ScheduleHistoryIdentifier.ToString().Split(',').Select(item => string.Format("Id.Equals({0}) ", item))) + ") and ");
            }
            if (this.validationEngine.IsValidDate(searchParameter.StartDate) && !this.validationEngine.IsValidDate(searchParameter.EndDate))
            {
                DateTime fromDateTime = DateTime.SpecifyKind(Convert.ToDateTime(searchParameter.StartDate), DateTimeKind.Utc);
                queryString.Append("StartDate >= DateTime(" + fromDateTime.Year + "," + fromDateTime.Month + "," + fromDateTime.Day + "," + fromDateTime.Hour + "," + fromDateTime.Minute + "," + fromDateTime.Second + ") and ");
            }

            if (this.validationEngine.IsValidDate(searchParameter.EndDate) && !this.validationEngine.IsValidDate(searchParameter.StartDate))
            {
                DateTime toDateTime = DateTime.SpecifyKind(Convert.ToDateTime(searchParameter.EndDate), DateTimeKind.Utc);
                queryString.Append("EndDate <= DateTime(" + toDateTime.Year + "," + toDateTime.Month + "," + toDateTime.Day + "," + toDateTime.Hour + "," + toDateTime.Minute + "," + toDateTime.Second + ") and ");
            }

            if (this.validationEngine.IsValidDate(searchParameter.StartDate) && this.validationEngine.IsValidDate(searchParameter.EndDate))
            {
                DateTime fromDateTime = DateTime.SpecifyKind(Convert.ToDateTime(searchParameter.StartDate), DateTimeKind.Utc);
                DateTime toDateTime = DateTime.SpecifyKind(Convert.ToDateTime(searchParameter.EndDate), DateTimeKind.Utc);

                queryString.Append("StartDate >= DateTime(" + fromDateTime.Year + "," + fromDateTime.Month + "," + fromDateTime.Day + "," + fromDateTime.Hour + "," + fromDateTime.Minute + "," + fromDateTime.Second + ") " +
                               "and EndDate <= DateTime(" + +toDateTime.Year + "," + toDateTime.Month + "," + toDateTime.Day + "," + toDateTime.Hour + "," + toDateTime.Minute + "," + toDateTime.Second + ") and ");
            }
            queryString.Append(string.Format("TenantCode.Equals(\"{0}\") ", tenantCode));
            return queryString.ToString();
        }


        /// <summary>
        /// This method determines uniqueness of elements in repository.
        /// </summary>
        /// <param name="schedules">The schedules to save.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns name="result">
        /// Returns true if all elements are not present in repository, false otherwise.
        /// </returns>
        private bool IsDuplicateSchedule(IList<Schedule> schedules, string operation, string tenantCode)
        {
            bool result = false;
            try
            {
                this.SetAndValidateConnectionString(tenantCode);

                StringBuilder query = new StringBuilder();

                if (operation.Equals(ModelConstant.ADD_OPERATION))
                {
                    query.Append("(" + string.Join(" or ", schedules.Select(item => string.Format("Name.Equals(\"{0}\")", item.Name)).ToList()) + ") and IsDeleted.Equals(false) and TenantCode.Equals(\"" + tenantCode + "\")");
                }

                if (operation.Equals(ModelConstant.UPDATE_OPERATION))
                {
                    query.Append("(" + string.Join(" or ", schedules.Select(item => string.Format("(Name.Equals(\"{0}\") and !Id.Equals({1}))", item.Name, item.Identifier))) + ") and IsDeleted.Equals(false) and TenantCode.Equals(\"" + tenantCode + "\")");
                }

                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    IList<ScheduleRecord> scheduleRecords = nISEntitiesDataContext.ScheduleRecords.Where(query.ToString()).Select(item => item).AsQueryable().ToList();
                    if (scheduleRecords.Count > 0)
                    {
                        result = true;
                    }
                }
            }
            catch
            {
                throw;
            }

            return result;
        }

        #region Get Connection String

        /// <summary>
        /// This method help to set and validate connection string
        /// </summary>
        /// <param name="tenantCode">
        /// The tenant code
        /// </param>
        private void SetAndValidateConnectionString(string tenantCode)
        {
            try
            {
                this.connectionString = validationEngine.IsValidText(this.connectionString) ? this.connectionString : this.configurationutility.GetConnectionString(ModelConstant.COMMON_SECTION, ModelConstant.NIS_CONNECTION_STRING, ModelConstant.CONFIGURATON_BASE_URL, ModelConstant.TENANT_CODE_KEY, tenantCode);
                if (!this.validationEngine.IsValidText(this.connectionString))
                {
                    throw new ConnectionStringNotFoundException(tenantCode);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        /// <summary>
        /// This method help to generate statement for customer
        /// </summary>
        /// <param name="customer"> the customer object </param>
        /// <param name="statement"> the statement object </param>
        /// <param name="finalHtml"> the final html string for statement </param>
        /// <param name="batchMaster"> the batch master object </param>
        /// <param name="batchDetails"> the list of batch details records </param>
        /// <param name="baseURL"> the base URL of API </param>
        private ScheduleLogDetailRecord GenerateStatement(CustomerMasterRecord customer, Statement statement, StringBuilder finalHtml, BatchMasterRecord batchMaster, IList<BatchDetailRecord> batchDetails, string baseURL)
        {
            ScheduleLogDetailRecord logDetailRecord = new ScheduleLogDetailRecord();
            try
            {
                //start to render actual html content data
                StringBuilder currentCustomerHtmlStatement = new StringBuilder(finalHtml.ToString());
                for (int i = 0; i < statement.Pages.Count; i++)
                {
                    var page = statement.Pages[i];
                    var pagewidgets = page.PageWidgets;
                    for (int j = 0; j < pagewidgets.Count; j++)
                    {
                        var widget = pagewidgets[j];
                        if (widget.WidgetId == HtmlConstants.CUSTOMER_INFORMATION_WIDGET_ID) //Customer Information Widget
                        {
                            IList<CustomerMediaRecord> customerMedias = new List<CustomerMediaRecord>();
                            currentCustomerHtmlStatement.Replace("{{CustomerName}}", (customer.FirstName + " " + customer.MiddleName + " " + customer.LastName));
                            currentCustomerHtmlStatement.Replace("{{Address1}}", customer.AddressLine1);
                            string address2 = (customer.AddressLine2 != "" ? customer.AddressLine2 + ", " : "") + (customer.City != "" ? customer.City + ", " : "") + (customer.State != "" ? customer.State + ", " : "") + (customer.Country != "" ? customer.Country + ", " : "") + (customer.Zip != "" ? customer.Zip : "");
                            currentCustomerHtmlStatement.Replace("{{Address2}}", address2);

                            using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                            {
                                customerMedias = nISEntitiesDataContext.CustomerMediaRecords.Where(item => item.CustomerId == customer.Id && item.StatementId == statement.Identifier && item.WidgetId == HtmlConstants.CUSTOMER_INFORMATION_WIDGET_ID)?.ToList();
                            }
                            var custMedia = customerMedias.Where(item => item.PageId == page.Identifier && item.WidgetId == widget.WidgetId)?.ToList()?.FirstOrDefault();
                            if (custMedia != null && custMedia.VideoURL != string.Empty)
                            {
                                currentCustomerHtmlStatement.Replace("{{VideoSource_" + statement.Identifier + "_" + page.Identifier + "_" + widget.WidgetId + "}}", custMedia.VideoURL);
                            }
                            else
                            {
                                var batchDetail = batchDetails.Where(item => item.StatementId == statement.Identifier && item.WidgetId == widget.WidgetId && item.PageId == page.Identifier)?.ToList()?.FirstOrDefault();
                                if (batchDetail != null && batchDetail.VideoURL != string.Empty)
                                {
                                    currentCustomerHtmlStatement.Replace("{{VideoSource_" + statement.Identifier + "_" + page.Identifier + "_" + widget.WidgetId + "}}", batchDetail.VideoURL);
                                }
                            }
                        }
                        else if (widget.WidgetId == HtmlConstants.ACCOUNT_INFORMATION_WIDGET_ID) //Account Information Widget
                        {
                            StringBuilder AccDivData = new StringBuilder();
                            AccDivData.Append("<div class='list-row-small ht70px'><div class='list-middle-row'> <div class='list-text'>Statement Date" + "</div><label class='list-value mb-0'>" + Convert.ToDateTime(customer.StatementDate).ToShortDateString() + "</label></div></div>");

                            AccDivData.Append("<div class='list-row-small ht70px'><div class='list-middle-row'> <div class='list-text'>Statement Period" + "</div><label class='list-value mb-0'>" + customer.StatementPeriod + "</label></div></div>");

                            AccDivData.Append("<div class='list-row-small ht70px'><div class='list-middle-row'> <div class='list-text'>Cusomer ID" + "</div><label class='list-value mb-0'>" + customer.CustomerCode + "</label></div></div>");

                            AccDivData.Append("<div class='list-row-small ht70px'><div class='list-middle-row'> <div class='list-text'>RM Name" + "</div><label class='list-value mb-0'>" + customer.RmName + "</label></div></div>");

                            AccDivData.Append("<div class='list-row-small ht70px'><div class='list-middle-row'> <div class='list-text'>RM Contact Number" + "</div><label class='list-value mb-0'>" + customer.RmContactNumber + "</label></div></div>");
                            currentCustomerHtmlStatement.Replace("{{AccountInfoData}}", AccDivData.ToString());
                        }
                        else if (widget.WidgetId == HtmlConstants.IMAGE_WIDGET_ID) //Image Widget
                        {
                            var imgAssetFilepath = string.Empty;
                            if (widget.WidgetSetting != string.Empty && validationEngine.IsValidJson(widget.WidgetSetting))
                            {
                                dynamic widgetSetting = JObject.Parse(widget.WidgetSetting);
                                if (widgetSetting.isPersonalize == false) //Is not dynamic image, then assign selected image from asset library
                                {
                                    imgAssetFilepath = baseURL + "/assets/" + widgetSetting.AssetLibraryId + "/" + widgetSetting.AssetName;
                                }
                                else //Is dynamic image, then assign it from database 
                                {
                                    ImageRecord imageRecord = new ImageRecord();
                                    using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                                    {
                                        imageRecord = nISEntitiesDataContext.ImageRecords.Where(item => item.BatchId == batchMaster.Id && item.StatementId == statement.Identifier && item.PageId == page.Identifier && item.WidgetId == widget.WidgetId)?.ToList()?.FirstOrDefault();
                                    }
                                    if (imageRecord != null && imageRecord.Image1 != string.Empty)
                                    {
                                        imgAssetFilepath = imageRecord.Image1;
                                    }
                                    else
                                    {
                                        var batchDetail = batchDetails.Where(item => item.StatementId == statement.Identifier && item.WidgetId == widget.WidgetId && item.PageId == page.Identifier)?.ToList()?.FirstOrDefault();
                                        if (batchDetail != null && batchDetail.ImageURL != string.Empty)
                                        {
                                            imgAssetFilepath = batchDetail.ImageURL;
                                        }
                                        else
                                        {
                                            logDetailRecord.LogMessage = "Image not found for Page: " + page.Identifier + " and Widget: " + widget.WidgetId + " for image widget..!!";
                                            logDetailRecord.Status = ScheduleLogStatus.Failed.ToString();
                                            break;
                                        }
                                    }
                                }
                                currentCustomerHtmlStatement.Replace("{{ImageSource_" + statement.Identifier + "_" + page.Identifier + "_" + widget.WidgetId + "}}", imgAssetFilepath);
                            }
                        }
                        else if (widget.WidgetId == HtmlConstants.VIDEO_WIDGET_ID) //Video widget
                        {
                            var vdoAssetFilepath = string.Empty;
                            if (widget.WidgetSetting != string.Empty && validationEngine.IsValidJson(widget.WidgetSetting))
                            {
                                dynamic widgetSetting = JObject.Parse(widget.WidgetSetting);
                                if (widgetSetting.isPersonalize == false) //Is not dynamic video, then assign selected video from asset library
                                {
                                    vdoAssetFilepath = baseURL + "/assets/" + widgetSetting.AssetLibraryId + "/" + widgetSetting.AssetName;
                                }
                                else //Is dynamic video, then assign it from database 
                                {
                                    VideoRecord videoRecord = new VideoRecord();
                                    using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                                    {
                                        videoRecord = nISEntitiesDataContext.VideoRecords.Where(item => item.BatchId == batchMaster.Id && item.StatementId == statement.Identifier && item.PageId == page.Identifier && item.WidgetId == widget.WidgetId)?.ToList()?.FirstOrDefault();
                                    }
                                    if (videoRecord != null && videoRecord.Video1 != string.Empty)
                                    {
                                        vdoAssetFilepath = videoRecord.Video1;
                                    }
                                    else
                                    {
                                        var batchDetail = batchDetails.Where(item => item.StatementId == statement.Identifier && item.WidgetId == widget.WidgetId && item.PageId == page.Identifier)?.ToList()?.FirstOrDefault();
                                        if (batchDetail != null && batchDetail.VideoURL != string.Empty)
                                        {
                                            vdoAssetFilepath = batchDetail.VideoURL;
                                        }
                                        else
                                        {
                                            logDetailRecord.LogMessage = "Video not found for Page: " + page.Identifier + " and Widget: " + widget.WidgetId + " for video widget..!!";
                                            logDetailRecord.Status = ScheduleLogStatus.Failed.ToString();
                                            break;
                                        }
                                    }
                                }
                                currentCustomerHtmlStatement.Replace("{{VideoSource_" + statement.Identifier + "_" + page.Identifier + "_" + widget.WidgetId + "}}", vdoAssetFilepath);
                            }
                        }
                        else if (widget.WidgetId == HtmlConstants.SUMMARY_AT_GLANCE_WIDGET_ID) //Summary at Glance Widget
                        {
                            IList<AccountMasterRecord> accountrecords = new List<AccountMasterRecord>();
                            using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                            {
                                accountrecords = nISEntitiesDataContext.AccountMasterRecords.Where(item => item.CustomerId == customer.Id && item.BatchId == batchMaster.Id)?.ToList();
                            }
                            if (accountrecords != null && accountrecords.Count > 0)
                            {
                                StringBuilder accSummary = new StringBuilder();
                                var accRecords = accountrecords.GroupBy(item => item.AccountType).ToList();
                                accRecords.ToList().ForEach(acc =>
                                {
                                    accSummary.Append("<tr><td>" + acc.FirstOrDefault().AccountType + "</td><td>" + acc.FirstOrDefault().Currency + "</td><td>" + acc.Sum(it => it.Balance).ToString() + "</td></tr>");
                                });
                                currentCustomerHtmlStatement.Replace("{{AccountSummary_" + page.Identifier + "_" + widget.WidgetId + "}}", accSummary.ToString());
                            }
                            else
                            {
                                logDetailRecord.LogMessage = "Account master data is not available related to Summary at Glance widget..!!";
                                logDetailRecord.Status = ScheduleLogStatus.Failed.ToString();
                                break;
                            }
                        }
                        else if (widget.WidgetId == HtmlConstants.CURRENT_AVAILABLE_BALANCE_WIDGET_ID)
                        {
                            IList<AccountMasterRecord> accountrecords = new List<AccountMasterRecord>();
                            using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                            {
                                accountrecords = nISEntitiesDataContext.AccountMasterRecords.Where(item => item.CustomerId == customer.Id && item.BatchId == batchMaster.Id && item.AccountType.ToLower().Contains("current"))?.ToList();
                            }
                            if (accountrecords != null && accountrecords.Count > 0)
                            {
                                var records = accountrecords.GroupBy(item => item.AccountType).ToList();
                                records?.ToList().ForEach(acc =>
                                {
                                    currentCustomerHtmlStatement.Replace("{{TotalValue_" + page.Identifier + "_" + widget.WidgetId + "}}", (acc.FirstOrDefault().Currency + " " + acc.Sum(it => it.GrandTotal).ToString()));
                                    currentCustomerHtmlStatement.Replace("{{TotalDeposit_" + page.Identifier + "_" + widget.WidgetId + "}}", (acc.FirstOrDefault().Currency + " " + acc.Sum(it => it.TotalDeposit).ToString()));
                                    currentCustomerHtmlStatement.Replace("{{TotalSpend_" + page.Identifier + "_" + widget.WidgetId + "}}", (acc.FirstOrDefault().Currency + " " + acc.Sum(it => it.TotalSpend).ToString()));
                                    currentCustomerHtmlStatement.Replace("{{Savings_" + page.Identifier + "_" + widget.WidgetId + "}}", (acc.FirstOrDefault().Currency + " " + acc.Sum(it => it.ProfitEarned).ToString()));
                                });
                            }
                            else
                            {
                                logDetailRecord.LogMessage = "Account master data is not available related to Current Available Balance widget..!!";
                                logDetailRecord.Status = ScheduleLogStatus.Failed.ToString();
                                break;
                            }
                        }
                        else if (widget.WidgetId == HtmlConstants.SAVING_AVAILABLE_BALANCE_WIDGET_ID)
                        {
                            IList<AccountMasterRecord> accountrecords = new List<AccountMasterRecord>();
                            using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                            {
                                accountrecords = nISEntitiesDataContext.AccountMasterRecords.Where(item => item.CustomerId == customer.Id && item.BatchId == batchMaster.Id && item.AccountType.ToLower().Contains("saving"))?.ToList();
                            }
                            if (accountrecords != null && accountrecords.Count > 0)
                            {
                                var records = accountrecords.GroupBy(item => item.AccountType).ToList();
                                records?.ToList().ForEach(acc =>
                                {
                                    currentCustomerHtmlStatement.Replace("{{TotalValue_" + page.Identifier + "_" + widget.WidgetId + "}}", (acc.FirstOrDefault().Currency + " " + acc.Sum(it => it.GrandTotal).ToString()));
                                    currentCustomerHtmlStatement.Replace("{{TotalDeposit_" + page.Identifier + "_" + widget.WidgetId + "}}", (acc.FirstOrDefault().Currency + " " + acc.Sum(it => it.TotalDeposit).ToString()));
                                    currentCustomerHtmlStatement.Replace("{{TotalSpend_" + page.Identifier + "_" + widget.WidgetId + "}}", (acc.FirstOrDefault().Currency + " " + acc.Sum(it => it.TotalSpend).ToString()));
                                    currentCustomerHtmlStatement.Replace("{{Savings_" + page.Identifier + "_" + widget.WidgetId + "}}", (acc.FirstOrDefault().Currency + " " + acc.Sum(it => it.ProfitEarned).ToString()));
                                });
                            }
                            else
                            {
                                logDetailRecord.LogMessage = "Account master data is not available related to Saving Available Balance widget..!!";
                                logDetailRecord.Status = ScheduleLogStatus.Failed.ToString();
                                break;
                            }
                        }
                        else if (widget.WidgetId == HtmlConstants.SAVING_TRANSACTION_WIDGET_ID)
                        {
                            IList<AccountTransactionRecord> accountTransactions = new List<AccountTransactionRecord>();
                            using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                            {
                                accountTransactions = nISEntitiesDataContext.AccountTransactionRecords.Where(item => item.CustomerId == customer.Id && item.BatchId == batchMaster.Id && item.AccountType.ToLower().Contains("saving"))?.OrderByDescending(item=>item.TransactionDate)?.ToList();
                                if (accountTransactions != null && accountTransactions.Count > 0)
                                {
                                    StringBuilder transaction = new StringBuilder();
                                    accountTransactions.ToList().ForEach(trans =>
                                    {
                                        transaction.Append("<tr><td>" + trans.TransactionDate + "</td><td>" + trans.TransactionType + "</td><td>" +
                                                trans.Narration + "</td><td>" + trans.FCY + "</td><td>" + trans.CurrentRate + "</td><td>"
                                                + trans.LCY + "</td><td><div class='action-btns btn-tbl-action'><button type='button' title='View'>" +
                                                "<span class='fa fa-paper-plane-o'></span></button></div></td></tr>");
                                    });
                                    currentCustomerHtmlStatement.Replace("{{AccountTransactionDetails_" + page.Identifier + "_" + widget.WidgetId + "}}", transaction.ToString());
                                }
                                else
                                {
                                    logDetailRecord.LogMessage = "Account transactions data is not available related to Saving Transaction widget..!!";
                                    logDetailRecord.Status = ScheduleLogStatus.Failed.ToString();
                                    break;
                                }
                            }
                        }
                        else if (widget.WidgetId == HtmlConstants.CURRENT_TRANSACTION_WIDGET_ID)
                        {
                            IList<AccountTransactionRecord> accountTransactions = new List<AccountTransactionRecord>();
                            using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                            {
                                accountTransactions = nISEntitiesDataContext.AccountTransactionRecords.Where(item => item.CustomerId == customer.Id && item.BatchId == batchMaster.Id && item.AccountType.ToLower().Contains("current"))?.OrderByDescending(item => item.TransactionDate)?.ToList();
                                if (accountTransactions != null && accountTransactions.Count > 0)
                                {
                                    StringBuilder transaction = new StringBuilder();
                                    accountTransactions.ToList().ForEach(trans =>
                                    {
                                        transaction.Append("<tr><td>" + trans.TransactionDate + "</td><td>" + trans.TransactionType + "</td><td>" +
                                                trans.Narration + "</td><td>" + trans.FCY + "</td><td>" + trans.CurrentRate + "</td><td>"
                                                + trans.LCY + "</td><td><div class='action-btns btn-tbl-action'><button type='button' title='View'>" +
                                                "<span class='fa fa-paper-plane-o'></span></button></div></td></tr>");
                                    });
                                    currentCustomerHtmlStatement.Replace("{{AccountTransactionDetails_" + page.Identifier + "_" + widget.WidgetId + "}}", transaction.ToString());
                                }
                                else
                                {
                                    logDetailRecord.LogMessage = "Account transactions data is not available related to Current Transaction widget..!!";
                                    logDetailRecord.Status = ScheduleLogStatus.Failed.ToString();
                                    break;
                                }
                            }
                        }
                        else if (widget.WidgetId == HtmlConstants.TOP_4_INCOME_SOURCES_WIDGET_ID)
                        {
                            IList<Top4IncomeSourcesRecord> top4IncomeSources = new List<Top4IncomeSourcesRecord>();
                            using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                            {
                                top4IncomeSources = nISEntitiesDataContext.Top4IncomeSourcesRecord.Where(item => item.CustomerId == customer.Id && item.BatchId == batchMaster.Id)?.OrderByDescending(item => item.CurrentSpend)?.Take(4)?.ToList();
                            }
                            if (top4IncomeSources != null && top4IncomeSources.Count > 0)
                            {
                                StringBuilder incomeSources = new StringBuilder();
                                top4IncomeSources.ToList().ForEach(src =>
                                {
                                    incomeSources.Append("<tr><td class='float-left'>" + src.Source + "</td>" + "<td> " + src.CurrentSpend +
                                        "" + "</td><td class='align-text-top'>" + "<span class='fa fa-sort-asc fa-2x text-danger align-text-top' " +
                                        "aria-hidden='true'>" + "</span>&nbsp;" + src.AverageSpend + " " + "</td></tr>");
                                });
                                currentCustomerHtmlStatement.Replace("{{IncomeSourceList_" + page.Identifier + "_" + widget.WidgetId + "}}", incomeSources.ToString());
                            }
                            else
                            {
                                logDetailRecord.LogMessage = "Top Income sources data is not available related to Top 4 Income Source widget..!!";
                                logDetailRecord.Status = ScheduleLogStatus.Failed.ToString();
                                break;
                            }
                        }
                        else if (widget.WidgetId == HtmlConstants.ANALYTICS_WIDGET_ID)
                        {
                        }
                        else if (widget.WidgetId == HtmlConstants.SAVING_TREND_WIDGET_ID)
                        {
                        }
                        else if (widget.WidgetId == HtmlConstants.SPENDING_TREND_WIDGET_ID)
                        {
                        }
                        else if (widget.WidgetId == HtmlConstants.REMINDER_AND_RECOMMENDATION_WIDGET_ID)
                        {
                            IList<ReminderAndRecommendationRecord> reminderAndRecommendations = new List<ReminderAndRecommendationRecord>();
                            using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                            {
                                reminderAndRecommendations = nISEntitiesDataContext.ReminderAndRecommendationRecords.Where(item => item.CustomerId == customer.Id && item.BatchId == batchMaster.Id)?.ToList();
                            }
                            if (reminderAndRecommendations != null && reminderAndRecommendations.Count > 0)
                            {
                                StringBuilder reminderstr = new StringBuilder();
                                reminderAndRecommendations.ToList().ForEach(item =>
                                {
                                    string targetlink = item.TargetURL != null && item.TargetURL != string.Empty ? item.TargetURL : "javascript:void(0)";
                                    reminderstr.Append("<tr><td class='width75 text-left' style='background-color: #dce3dc;'><label>" +
                                        item.Description + "</label></td><td><a href='" + targetlink + "'>" +
                                        "<i class='fa fa-caret-left fa-2x' style='color:red' aria-hidden='true'>" +"</i>" + item.LabelText + "</a></td></tr>");
                                });
                                currentCustomerHtmlStatement.Replace("{{ReminderAndRecommdationDataList_" + page.Identifier + "_" + widget.WidgetId + "}}", reminderstr.ToString());
                            }
                            else
                            {
                                logDetailRecord.LogMessage = "The data is not available related to Reminder and Recommendation widget..!!";
                                logDetailRecord.Status = ScheduleLogStatus.Failed.ToString();
                                break;
                            }
                        }
                    }
                }

                if (logDetailRecord.Status != ScheduleLogStatus.Failed.ToString())
                {
                    string fileName = "Statement_" + customer.Id + "_" + statement.Identifier + "_" + DateTime.Now.ToString().Replace("-", "_").Replace(":", "_").Replace(" ", "_").Replace('/', '_') + ".html";
                    string filePath = this.utility.WriteToFile(currentCustomerHtmlStatement.ToString(), fileName, batchMaster.Id);

                    logDetailRecord.StatementFilePath = filePath;
                    logDetailRecord.Status = ScheduleLogStatus.Completed.ToString();
                    logDetailRecord.LogMessage = "Statement generated successfully..!!";
                }
                return logDetailRecord;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string GenerateCommonStatement(Statement statement, StringBuilder finalHtml, string baseURL, BatchMasterRecord batchMaster)
        {
            try
            {
                //start to render common html content data
                StringBuilder htmlString = new StringBuilder(finalHtml.ToString());
                for (int i = 0; i < statement.Pages.Count; i++)
                {
                    var page = statement.Pages[i];
                    var pagewidgets = page.PageWidgets;
                    for (int j = 0; j < pagewidgets.Count; j++)
                    {
                        var widget = pagewidgets[j];
                        if (widget.WidgetId == HtmlConstants.CUSTOMER_INFORMATION_WIDGET_ID)
                        {
                            string customerInfoJson = "{'FirstName':'Laura','MiddleName':'J','LastName':'Donald','AddressLine1':" +
                                "'4000 Executive Parkway','AddressLine2':'Saint Globin Rd','City':'Canary Wharf', 'State':'London', " +
                                "'Country':'England','Zip':'E14 9RZ'}";
                            if (customerInfoJson != string.Empty && validationEngine.IsValidJson(customerInfoJson))
                            {
                                CustomerInformation customerInfo = JsonConvert.DeserializeObject<CustomerInformation>(customerInfoJson);
                                htmlString.Replace("{{VideoSource_" + statement.Identifier + "_" + page.Identifier + "_" + widget.WidgetId + "}}", "assets/images/SampleVideo.mp4");

                                string customerName = customerInfo.FirstName + " " + customerInfo.MiddleName + " " + customerInfo.LastName;
                                htmlString.Replace("{{CustomerName}}", customerName);

                                string address1 = customerInfo.AddressLine1 + ", " + customerInfo.AddressLine2 + ",";
                                htmlString.Replace("{{Address1}}", address1);

                                string address2 = (customerInfo.City != "" ? customerInfo.City + "," : "") +
                                    (customerInfo.State != "" ? customerInfo.State + "," : "") + (customerInfo.Country != "" ?
                                    customerInfo.Country + "," : "") + (customerInfo.Zip != "" ? customerInfo.Zip : "");
                                htmlString.Replace("{{Address2}}", address2);
                            }
                        }
                        else if (widget.WidgetId == HtmlConstants.ACCOUNT_INFORMATION_WIDGET_ID)
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
                            }
                            htmlString.Replace("{{AccountInfoData}}", AccDivData.ToString());
                        }
                        else if (widget.WidgetId == HtmlConstants.IMAGE_WIDGET_ID)
                        {
                            var imgAssetFilepath = "assets/images/icon-image.png";
                            if (widget.WidgetSetting != string.Empty && validationEngine.IsValidJson(widget.WidgetSetting))
                            {
                                dynamic widgetSetting = JObject.Parse(widget.WidgetSetting);
                                if (widgetSetting.isPersonalize == false)
                                {
                                    imgAssetFilepath = baseURL + "/assets/" + widgetSetting.AssetLibraryId + "/" + widgetSetting.AssetName;
                                }
                            }
                            htmlString.Replace("{{ImageSource_" + statement.Identifier + "_" + page.Identifier + "_" + widget.WidgetId + "}}", imgAssetFilepath);
                        }
                        else if (widget.WidgetId == HtmlConstants.VIDEO_WIDGET_ID)
                        {
                            var vdoAssetFilepath = "assets/images/SampleVideo.mp4";
                            if (widget.WidgetSetting != string.Empty && validationEngine.IsValidJson(widget.WidgetSetting))
                            {
                                dynamic widgetSetting = JObject.Parse(widget.WidgetSetting);
                                if (widgetSetting.isPersonalize == false && widgetSetting.isEmbedded == false)
                                {
                                    vdoAssetFilepath = baseURL + "/assets/" + widgetSetting.AssetLibraryId + "/" + widgetSetting.AssetName;
                                }
                            }
                            htmlString.Replace("{{VideoSource_" + statement.Identifier + "_" + page.Identifier + "_" + widget.WidgetId + "}}", vdoAssetFilepath);
                        }
                        else if (widget.WidgetId == HtmlConstants.SUMMARY_AT_GLANCE_WIDGET_ID)
                        {
                            string accountBalanceDataJson = "[{\"AccountType\":\"Saving Account\",\"Currency\":\"Dollor\",\"Amount\":\"87356\"}" +
                                ",{\"AccountType\":\"Current Account\",\"Currency\":\"Dollor\",\"Amount\":\"18654\"},{\"AccountType\":" +
                                "\"Recurring Account\",\"Currency\":\"Dollor\",\"Amount\":\"54367\"},{\"AccountType\":\"Wealth\",\"Currency\"" +
                                ":\"Dollor\",\"Amount\":\"4589\"}]";

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
                                    htmlString.Replace("{{AccountSummary}}", accSummary.ToString());
                                }
                            }
                        }
                        else if (widget.WidgetId == HtmlConstants.CURRENT_AVAILABLE_BALANCE_WIDGET_ID)
                        {
                            string currentAvailBalanceJson = "{'GrandTotal':'32,453,23', 'TotalDeposit':'16,250,00', 'TotalSpend':'16,254,00', 'ProfitEarned':'1,430,00 ', 'Currency':'R', 'Balance': '14,768,80', 'AccountNumber': 'J566565TR678ER', 'AccountType': 'Current'}";
                            if (currentAvailBalanceJson != string.Empty && validationEngine.IsValidJson(currentAvailBalanceJson))
                            {
                                AccountMaster accountMaster = JsonConvert.DeserializeObject<AccountMaster>(currentAvailBalanceJson);
                                htmlString.Replace("{{TotalValue}}", (accountMaster.Currency + accountMaster.GrandTotal));
                                htmlString.Replace("{{TotalDeposit}}", (accountMaster.Currency + accountMaster.TotalDeposit));
                                htmlString.Replace("{{TotalSpend}}", (accountMaster.Currency + accountMaster.TotalSpend));
                                htmlString.Replace("{{Savings}}", (accountMaster.Currency + accountMaster.ProfitEarned));
                            }
                        }
                        else if (widget.WidgetId == HtmlConstants.SAVING_AVAILABLE_BALANCE_WIDGET_ID)
                        {
                            string savingAvailBalanceJson = "{'GrandTotal':'26,453,23', 'TotalDeposit':'13,530,00', 'TotalSpend':'12,124,00', 'ProfitEarned':'2,340,00 ', 'Currency':'R', 'Balance': '19,456,80', 'AccountNumber': 'J566565TR678ER', 'AccountType': 'Saving'}";
                            if (savingAvailBalanceJson != string.Empty && validationEngine.IsValidJson(savingAvailBalanceJson))
                            {
                                AccountMaster accountMaster = JsonConvert.DeserializeObject<AccountMaster>(savingAvailBalanceJson);
                                htmlString.Replace("{{TotalValue}}", (accountMaster.Currency + accountMaster.GrandTotal));
                                htmlString.Replace("{{TotalDeposit}}", (accountMaster.Currency + accountMaster.TotalDeposit));
                                htmlString.Replace("{{TotalSpend}}", (accountMaster.Currency + accountMaster.TotalSpend));
                                htmlString.Replace("{{Savings}}", (accountMaster.Currency + accountMaster.ProfitEarned));
                            }
                        }
                        else if (widget.WidgetId == HtmlConstants.SAVING_TRANSACTION_WIDGET_ID)
                        {
                            string savingTransactionJson = "[{ 'TransactionDate': '14/07/2020', 'TransactionType': 'CR', 'Narration': 'NXT TXN: IIFL IIFL6574562', 'Credit': '1666.67', 'Debit': '1.062', 'Balance': '1771.42' },{ 'TransactionDate': '19/07/2020', 'TransactionType': 'CR', 'Narration': 'NXT TXN: IIFL IIFL3557346', 'Credit': '1254.71', 'Debit': '1.123', 'Balance': '1876.00' }, { 'TransactionDate': '21/07/2020', 'TransactionType': 'CR', 'Narration': 'NXT TXN: IIFL IIFL8965435', 'Credit': '2345.12', 'Debit': '1.461', 'Balance': '1453.21' }, { 'TransactionDate': '27/07/2020', 'TransactionType': 'CR', 'Narration': 'NXT TXN: IIFL IIFL0034212', 'Credit': '1435.89', 'Debit': '0.962', 'Balance': '1654.56' }]";
                            if (savingTransactionJson != string.Empty && validationEngine.IsValidJson(savingTransactionJson))
                            {
                                IList<AccountTransaction> accountTransactions = JsonConvert.DeserializeObject<List<AccountTransaction>>(savingTransactionJson);
                                StringBuilder transaction = new StringBuilder();
                                accountTransactions.ToList().ForEach(trans =>
                                {
                                    transaction.Append("<tr><td>" + trans.TransactionDate + "</td><td>" + trans.TransactionType + "</td><td>" +
                                        trans.Narration + "</td><td class='text-success'>" + trans.Credit + "</td><td class='text-danger'>"
                                        + trans.Debit + "</td><td class='text-danger'><i class='fa fa-caret-left fa-2x' aria-hidden='true'></i> </td>" +
                                        "<td>" + trans.Balance + "</td></tr>");
                                });
                                htmlString.Replace("{{AccountTransactionDetails}}", transaction.ToString());
                            }
                        }
                        else if (widget.WidgetId == HtmlConstants.CURRENT_TRANSACTION_WIDGET_ID)
                        {
                            string currentTransactionJson = "[{ 'TransactionDate': '15/07/2020', 'TransactionType': 'CR', 'Narration': 'NXT TXN: IIFL IIFL6574562', 'FCY': '1666.67', 'CurrentRate': '1.062', 'LCY': '1771.42' },{ 'TransactionDate': '19/07/2020', 'TransactionType': 'CR', 'Narration': 'NXT TXN: IIFL IIFL3557346', 'FCY': '1254.71', 'CurrentRate': '1.123', 'LCY': '1876.00' }, { 'TransactionDate': '25/07/2020', 'TransactionType': 'CR', 'Narration': 'NXT TXN: IIFL IIFL8965435', 'FCY': '2345.12', 'CurrentRate': '1.461', 'LCY': '1453.21' }, { 'TransactionDate': '28/07/2020', 'TransactionType': 'CR', 'Narration': 'NXT TXN: IIFL IIFL0034212', 'FCY': '1435.89', 'CurrentRate': '0.962', 'LCY': '1654.56' }]";
                            if (currentTransactionJson != string.Empty && validationEngine.IsValidJson(currentTransactionJson))
                            {
                                IList<AccountTransaction> accountTransactions = JsonConvert.DeserializeObject<List<AccountTransaction>>(currentTransactionJson);
                                StringBuilder transaction = new StringBuilder();
                                accountTransactions.ToList().ForEach(trans =>
                                {
                                    transaction.Append("<tr><td>" + trans.TransactionDate + "</td><td>" + trans.TransactionType + "</td><td>" +
                                        trans.Narration + "</td><td>" + trans.FCY + "</td><td>" + trans.CurrentRate + "</td><td>"
                                        + trans.LCY + "</td><td><div class='action-btns btn-tbl-action'><button type='button' title='View'>" +
                                        "<span class='fa fa-paper-plane-o'></span></button></div></td></tr>");
                                });
                                htmlString.Replace("{{CurrentAccountTransactionDetails}}", transaction.ToString());
                            }
                        }
                        else if (widget.WidgetId == HtmlConstants.TOP_4_INCOME_SOURCES_WIDGET_ID)
                        {
                            string incomeSourceListJson = "[{ 'Source': 'Salary Transfer', 'CurrentSpend': 3453, 'AverageSpend': 123},{ 'Source': 'Cash Deposit', 'CurrentSpend': 3453, 'AverageSpend': 6123},{ 'Source': 'Profit Earned', 'CurrentSpend': 3453, 'AverageSpend': 6123}, { 'Source': 'Rebete', 'CurrentSpend': 3453, 'AverageSpend': 123}]";
                            if (incomeSourceListJson != string.Empty && validationEngine.IsValidJson(incomeSourceListJson))
                            {
                                IList<IncomeSources> incomeSources = JsonConvert.DeserializeObject<List<IncomeSources>>(incomeSourceListJson);
                                StringBuilder incomeSrc = new StringBuilder();
                                incomeSources.ToList().ForEach(item =>
                                {
                                    incomeSrc.Append("<tr><td class='float-left'>" + item.Source + "</td>" + "<td> " + item.CurrentSpend +
                                      "" + "</td><td class='align-text-top'>" + "<span class='fa fa-sort-asc fa-2x text-danger align-text-top' " +
                                      "aria-hidden='true'>" + "</span>&nbsp;" + item.AverageSpend + " " + "</td></tr>");
                                });
                                htmlString.Replace("{{IncomeSourceList}}", incomeSrc.ToString());
                            }
                        }
                        else if (widget.WidgetId == HtmlConstants.ANALYTICS_WIDGET_ID)
                        {
                            htmlString.Append(HtmlConstants.ANALYTIC_WIDGET_HTML);
                        }
                        else if (widget.WidgetId == HtmlConstants.SPENDING_TREND_WIDGET_ID)
                        {
                            htmlString.Append(HtmlConstants.SPENDING_TRENDS_WIDGET_HTML);
                        }
                        else if (widget.WidgetId == HtmlConstants.SAVING_TREND_WIDGET_ID)
                        {
                            htmlString.Append(HtmlConstants.SAVING_TRENDS_WIDGET_HTML);
                        }
                        else if (widget.WidgetId == HtmlConstants.REMINDER_AND_RECOMMENDATION_WIDGET_ID)
                        {
                            string reminderJson = "[{ 'Title': 'Update Missing Inofrmation', 'Action': 'Update' },{ 'Title': 'Your Rewards Video ia available', 'Action': 'View' },{ 'Title': 'Payment Due for Home Loan', 'Action': 'Pay' }]";
                            if (reminderJson != string.Empty && validationEngine.IsValidJson(reminderJson))
                            {
                                IList<ReminderAndRecommendation> reminderAndRecommendations = JsonConvert.DeserializeObject<List<ReminderAndRecommendation>>(reminderJson);
                                StringBuilder reminderstr = new StringBuilder();
                                reminderAndRecommendations.ToList().ForEach(item =>
                                {
                                    reminderstr.Append("<tr><td class='width75 text-left'><label style='background-color: #dce3dc;'>" +
                                        item.Title + "</label></td><td><a>" + "<i class='fa fa-caret-left fa-2x' style='color:red' aria-hidden='true'>" +
                                        "</i>" + item.Action + "</a></td></tr>");
                                });
                                htmlString.Replace("{{ReminderAndRecommdationDataList}}", reminderstr.ToString());
                            }
                        }
                    }
                }

                return htmlString.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
