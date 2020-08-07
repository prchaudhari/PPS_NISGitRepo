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
                        IsActive = true,
                        TenantCode = tenantCode,
                        StatementId = schedule.Statement.Identifier,
                        IsExportToPDF = schedule.IsExportToPDF,
                        UpdateBy = schedule.UpdateBy.Identifier,
                        LastUpdatedDate = DateTime.UtcNow,
                    }); ;
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
                        scheduleRecord.IsActive = item.IsActive;
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
                            IsActive = scheduleRecord.IsActive,
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

        /// <summary>
        /// This method helps to run the schedule
        /// </summary>
        /// <param name="baseURL">The schedule identifier</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>True if schedules runs successfully, false otherwise</returns>
        public bool RunSchedule(string baseURL, string tenantCode)
        {
            bool scheduleRunStatus = false;
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
                            List<StatementPageContent> statementPageContents = new List<StatementPageContent>();

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
                                    statement.Pages = new List<Page>();
                                    for (int i = 0; i < statementPages.Count; i++)
                                    {
                                        StatementPageContent statementPageContent = new StatementPageContent();
                                        StringBuilder pageHtmlContent = new StringBuilder();
                                        statementPageContent.Id = i;
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
                                                statementPageContent.PageId = page.Identifier;
                                                statementPageContent.PageTypeId = page.PageTypeId;
                                                statementPageContent.DisplayName = page.DisplayName;
                                                statement.Pages.Add(page);
                                                
                                                StringBuilder pageHeaderContent = new StringBuilder();
                                                pageHeaderContent.Append(HtmlConstants.WIDGET_HTML_HEADER);
                                                if (page.PageTypeName.ToLower().Contains("saving") || page.PageTypeName.ToLower().Contains("current"))
                                                {
                                                    pageHeaderContent.Append("{{SubTabs}}");
                                                }
                                                statementPageContent.PageHeaderContent = pageHeaderContent.ToString();

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
                                                                    pageHtmlContent.Append("<div class='row'>");
                                                                    isRowComplete = false;
                                                                }
                                                                int divLength = ((mergedlst[x].Width * 12) % 20) != 0 ? (((mergedlst[x].Width * 12) / 20) + 1)
                                                                    : ((mergedlst[x].Width * 12) / 20);
                                                                tempRowWidth = tempRowWidth + divLength;

                                                                // If current col-lg class length is greater than 12, 
                                                                //then end parent row class div and then start new row class div
                                                                if (tempRowWidth > 12)
                                                                {
                                                                    tempRowWidth = divLength;
                                                                    pageHtmlContent.Append("</div>"); // to end row class div
                                                                    pageHtmlContent.Append("<div class='row'>"); // to start new row class div

                                                                    isRowComplete = false;
                                                                }
                                                                pageHtmlContent.Append("<div class='col-lg-" + divLength + "'>");
                                                                if (mergedlst[x].WidgetId == HtmlConstants.CUSTOMER_INFORMATION_WIDGET_ID)
                                                                {
                                                                    pageHtmlContent.Append(HtmlConstants.CUSTOMER_INFORMATION_WIDGET_HTML.Replace("{{VideoSource}}", "{{VideoSource_" + statement.Identifier + "_" + page.Identifier + "_" + mergedlst[x].Identifier + "}}"));
                                                                }
                                                                else if (mergedlst[x].WidgetId == HtmlConstants.ACCOUNT_INFORMATION_WIDGET_ID)
                                                                {
                                                                    pageHtmlContent.Append(HtmlConstants.ACCOUNT_INFORMATION_WIDGET_HTML.Replace("{{AccountInfoData}}", "{{AccountInfoData_" + page.Identifier + "_" + mergedlst[x].Identifier + "}}"));
                                                                }
                                                                else if (mergedlst[x].WidgetId == HtmlConstants.IMAGE_WIDGET_ID)
                                                                {
                                                                    pageHtmlContent.Append(HtmlConstants.IMAGE_WIDGET_HTML.Replace("{{ImageSource}}", "{{ImageSource_" + statement.Identifier + "_" + page.Identifier + "_" + mergedlst[x].Identifier + "}}"));
                                                                }
                                                                else if (mergedlst[x].WidgetId == HtmlConstants.VIDEO_WIDGET_ID)
                                                                {
                                                                    pageHtmlContent.Append(HtmlConstants.VIDEO_WIDGET_HTML.Replace("{{VideoSource}}", "{{VideoSource_" + statement.Identifier + "_" + page.Identifier + "_" + mergedlst[x].Identifier + "}}"));
                                                                }
                                                                else if (mergedlst[x].WidgetId == HtmlConstants.SUMMARY_AT_GLANCE_WIDGET_ID)
                                                                {
                                                                    pageHtmlContent.Append(HtmlConstants.SUMMARY_AT_GLANCE_WIDGET_HTML.Replace("{{AccountSummary}}", "{{AccountSummary_" + page.Identifier + "_" + mergedlst[x].Identifier + "}}"));
                                                                }
                                                                else if (mergedlst[x].WidgetId == HtmlConstants.CURRENT_AVAILABLE_BALANCE_WIDGET_ID)
                                                                {
                                                                    string CurrentAvailBalanceWidget = HtmlConstants.SAVING_CURRENT_AVALABLE_BAL_WIDGET_HTML.Replace("{{TotalValue}}", "{{TotalValue_" + page.Identifier + "_" + mergedlst[x].Identifier + "}}").Replace("{{TotalDeposit}}", "{{TotalDeposit_" + page.Identifier + "_" + mergedlst[x].Identifier + "}}").Replace("{{TotalSpend}}", "{{TotalSpend_" + page.Identifier + "_" + mergedlst[x].Identifier + "}}").Replace("{{Savings}}", "{{Savings_" + page.Identifier + "_" + mergedlst[x].Identifier + "}}");
                                                                    pageHtmlContent.Append(CurrentAvailBalanceWidget);
                                                                }
                                                                else if (mergedlst[x].WidgetId == HtmlConstants.SAVING_AVAILABLE_BALANCE_WIDGET_ID)
                                                                {
                                                                    string SavingAvailBalanceWidget = HtmlConstants.SAVING_CURRENT_AVALABLE_BAL_WIDGET_HTML.Replace("{{TotalValue}}", "{{TotalValue_" + page.Identifier + "_" + mergedlst[x].Identifier + "}}").Replace("{{TotalDeposit}}", "{{TotalDeposit_" + page.Identifier + "_" + mergedlst[x].Identifier + "}}").Replace("{{TotalSpend}}", "{{TotalSpend_" + page.Identifier + "_" + mergedlst[x].Identifier + "}}").Replace("{{Savings}}", "{{Savings_" + page.Identifier + "_" + mergedlst[x].Identifier + "}}");
                                                                    pageHtmlContent.Append(SavingAvailBalanceWidget);
                                                                }
                                                                else if (mergedlst[x].WidgetId == HtmlConstants.SAVING_TRANSACTION_WIDGET_ID)
                                                                {
                                                                    pageHtmlContent.Append(HtmlConstants.SAVING_TRANSACTION_WIDGET_HTML.Replace("{{AccountTransactionDetails}}", "{{AccountTransactionDetails_" + page.Identifier + "_" + mergedlst[x].Identifier + "}}").Replace("{{SelectOption}}", "{{SelectOption_" + page.Identifier + "_" + mergedlst[x].Identifier + "}}"));
                                                                }
                                                                else if (mergedlst[x].WidgetId == HtmlConstants.CURRENT_TRANSACTION_WIDGET_ID)
                                                                {
                                                                    pageHtmlContent.Append(HtmlConstants.CURRENT_TRANSACTION_WIDGET_HTML.Replace("{{AccountTransactionDetails}}", "{{AccountTransactionDetails_" + page.Identifier + "_" + mergedlst[x].Identifier + "}}").Replace("{{SelectOption}}", "{{SelectOption_" + page.Identifier + "_" + mergedlst[x].Identifier + "}}"));
                                                                }
                                                                else if (mergedlst[x].WidgetId == HtmlConstants.TOP_4_INCOME_SOURCES_WIDGET_ID)
                                                                {
                                                                    pageHtmlContent.Append(HtmlConstants.TOP_4_INCOME_SOURCE_WIDGET_HTML.Replace("{{IncomeSourceList}}", "{{IncomeSourceList_" + page.Identifier + "_" + mergedlst[x].Identifier + "}}"));
                                                                }
                                                                else if (mergedlst[x].WidgetId == HtmlConstants.ANALYTICS_WIDGET_ID)
                                                                {
                                                                    pageHtmlContent.Append(HtmlConstants.ANALYTIC_WIDGET_HTML);
                                                                }
                                                                else if (mergedlst[x].WidgetId == HtmlConstants.SPENDING_TREND_WIDGET_ID)
                                                                {
                                                                    pageHtmlContent.Append(HtmlConstants.SPENDING_TRENDS_WIDGET_HTML);
                                                                }
                                                                else if (mergedlst[x].WidgetId == HtmlConstants.SAVING_TREND_WIDGET_ID)
                                                                {
                                                                    pageHtmlContent.Append(HtmlConstants.SAVING_TRENDS_WIDGET_HTML);
                                                                }
                                                                else if (mergedlst[x].WidgetId == HtmlConstants.REMINDER_AND_RECOMMENDATION_WIDGET_ID)
                                                                {
                                                                    pageHtmlContent.Append(HtmlConstants.REMINDER_WIDGET_HTML.Replace("{{ReminderAndRecommdationDataList}}", "{{ReminderAndRecommdationDataList_" + page.Identifier + "_" + mergedlst[x].Identifier + "}}"));
                                                                }

                                                                // To end current col-lg class div
                                                                pageHtmlContent.Append("</div>");

                                                                // if current col-lg class width is equal to 12 or end before complete col-lg-12 class, 
                                                                //then end parent row class div
                                                                if (tempRowWidth == 12 || (x == mergedlst.Count - 1))
                                                                {
                                                                    tempRowWidth = 0;
                                                                    pageHtmlContent.Append("</div>"); //To end row class div
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
                                                        pageHtmlContent.Append("</div>");
                                                    }
                                                }
                                                else
                                                {
                                                    pageHtmlContent.Append(HtmlConstants.NO_WIDGET_MESSAGE_HTML);
                                                }
                                                statementPageContent.PageFooterContent = HtmlConstants.WIDGET_HTML_FOOTER;
                                            }
                                        }
                                        else
                                        {
                                            pageHtmlContent.Append(HtmlConstants.NO_WIDGET_MESSAGE_HTML);
                                        }

                                        statementPageContent.HtmlContent = pageHtmlContent.ToString();
                                        statementPageContents.Add(statementPageContent);
                                    }
                                }
                            }

                            IList<CustomerMasterRecord> customerMasters = new List<CustomerMasterRecord>();
                            using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                            {
                                customerMasters = nISEntitiesDataContext.CustomerMasterRecords.Where(item => item.BatchId == batchMaster.Id).ToList();
                            }

                            if (customerMasters.Count > 0)
                            {
                                IList<ScheduleLogDetailRecord> logDetailRecords = new List<ScheduleLogDetailRecord>();
                                IList<StatementMetadataRecord> statementMetadataRecords = new List<StatementMetadataRecord>();
                                customerMasters.ToList().ForEach(customer =>
                                {
                                    using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                                    {
                                        renderEngine = nISEntitiesDataContext.RenderEngineRecords.Where(item => item.Id == 1).FirstOrDefault();
                                    }

                                    ScheduleLogDetailRecord logDetailRecord = GenerateStatements(customer, statement, statementPageContents, batchMaster, batchDetails, baseURL);
                                    if (logDetailRecord != null)
                                    {
                                        logDetailRecord.ScheduleLogId = scheduleLog.Id;
                                        logDetailRecord.CustomerId = customer.Id;
                                        logDetailRecord.CustomerName = customer.FirstName + (customer.MiddleName == string.Empty ? "" : " " + customer.MiddleName) + " " + customer.LastName;
                                        logDetailRecord.ScheduleId = schedule.Id;
                                        logDetailRecord.RenderEngineId = renderEngine.Id; //To be change once render engine implmentation start
                                        logDetailRecord.RenderEngineName = renderEngine.Name;
                                        logDetailRecord.RenderEngineURL = renderEngine.URL;
                                        logDetailRecord.NumberOfRetry = 1;
                                        logDetailRecord.CreationDate = DateTime.Now;
                                        logDetailRecord.TenantCode = tenantCode;
                                        logDetailRecords.Add(logDetailRecord);

                                        if (logDetailRecord.Status.ToLower().Equals(ScheduleLogStatus.Completed.ToString().ToLower()))
                                        {
                                            if (logDetailRecord.StatementMetadataRecords.Count > 0)
                                            {
                                                logDetailRecord.StatementMetadataRecords.ToList().ForEach(metarec =>
                                                {
                                                    metarec.ScheduleLogId = scheduleLog.Id;
                                                    metarec.ScheduleId = schedule.Id;
                                                    metarec.StatementDate = DateTime.Now;
                                                    metarec.StatementURL = logDetailRecord.StatementFilePath;
                                                    statementMetadataRecords.Add(metarec);
                                                });
                                            }
                                        }
                                        else if (logDetailRecord.Status.ToLower().Equals(ScheduleLogStatus.Failed.ToString().ToLower()))
                                        {
                                            this.utility.DeleteUnwantedDirectory(batchMaster.Id, customer.Id);
                                        }
                                    }

                                });

                                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                                {
                                    nISEntitiesDataContext.StatementMetadataRecords.AddRange(statementMetadataRecords);
                                    nISEntitiesDataContext.ScheduleLogDetailRecords.AddRange(logDetailRecords);
                                    nISEntitiesDataContext.SaveChanges();
                                }

                                var successRecords = logDetailRecords.Where(item => item.Status == ScheduleLogStatus.Completed.ToString())?.ToList();
                                if (successRecords != null && successRecords.Count > 0)
                                {
                                    //string finalCommonStatementHtml = GenerateCommonStatement(statement, finalHtml, baseURL);
                                    string finalCommonStatementHtml = GenerateCommonStatements(statement, statementPageContents, baseURL);
                                    string fileName = "Statement_" + statement.Identifier + "_" + batchMaster.Id + "_" + DateTime.Now.ToString().Replace("-", "_").Replace(":", "_").Replace(" ", "_").Replace('/', '_') + ".html";
                                    string CommonStatementZipFilePath = this.utility.CreateAndWriteToZipFile(finalCommonStatementHtml, fileName, batchMaster.Id);
                                    runHistory.FilePath = CommonStatementZipFilePath;
                                }

                                var scheduleLogStatus = ScheduleLogStatus.Completed.ToString();
                                var failedRecords = logDetailRecords.Where(item => item.Status == ScheduleLogStatus.Failed.ToString())?.ToList();
                                if (failedRecords != null && failedRecords.Count > 0)
                                {
                                    scheduleLogStatus = ScheduleLogStatus.Failed.ToString();
                                }
                                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                                {
                                    ScheduleLogRecord scheduleLogRecord = nISEntitiesDataContext.ScheduleLogRecords.Where(item => item.Id == scheduleLog.Id).FirstOrDefault();
                                    scheduleLogRecord.Status = scheduleLogStatus;
                                    nISEntitiesDataContext.SaveChanges();
                                }
                            }

                            runHistory.EndDate = DateTime.Now;
                            using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                            {
                                ScheduleLogRecord scheduleLogRecord = nISEntitiesDataContext.ScheduleLogRecords.Where(item => item.Id == scheduleLog.Id).FirstOrDefault();
                                scheduleLogRecord.Status = ScheduleLogStatus.Completed.ToString();
                                runHistory.ScheduleLogId = scheduleLogRecord.Id;

                                var scheduleRecord = nISEntitiesDataContext.ScheduleRecords.Where(item => item.Id == schedule.Id).FirstOrDefault();
                                scheduleRecord.Status = ScheduleStatus.Completed.ToString();

                                var batch = nISEntitiesDataContext.BatchMasterRecords.Where(item => item.Id == batchMaster.Id).ToList().FirstOrDefault();
                                batch.IsExecuted = true;

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
        /// <param name="statementPageContents"> the statement page html content list</param>
        /// <param name="batchMaster"> the batch master object </param>
        /// <param name="batchDetails"> the list of batch details records </param>
        /// <param name="baseURL"> the base URL of API </param>
        private ScheduleLogDetailRecord GenerateStatements(CustomerMasterRecord customer, Statement statement, List<StatementPageContent> statementPageContents, BatchMasterRecord batchMaster, IList<BatchDetailRecord> batchDetails, string baseURL)
        {
            ScheduleLogDetailRecord logDetailRecord = new ScheduleLogDetailRecord();
            IList<StatementMetadataRecord> statementMetadataRecords = new List<StatementMetadataRecord>();
            try
            {
                if (statementPageContents.Count > 0)
                {
                    string currency = string.Empty;
                    IList<AccountMasterRecord> accountrecords = new List<AccountMasterRecord>();
                    IList<AccountMasterRecord> savingaccountrecords = new List<AccountMasterRecord>();
                    IList<AccountMasterRecord> curerntaccountrecords = new List<AccountMasterRecord>();

                    using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                    {
                        accountrecords = nISEntitiesDataContext.AccountMasterRecords.Where(item => item.CustomerId == customer.Id && item.BatchId == batchMaster.Id)?.ToList();
                    }
                    if (accountrecords == null && accountrecords.Count == 0)
                    {
                        logDetailRecord.LogMessage = "Account master data is not available for this customer..!!";
                        logDetailRecord.Status = ScheduleLogStatus.Failed.ToString();
                        return logDetailRecord;
                    }
                    else
                    {
                        for (int i = 0; i < accountrecords.Count; i++)
                        {
                            if (accountrecords[i].AccountType.Equals(string.Empty))
                            {
                                logDetailRecord.LogMessage = "Invalid account master data for this customer..!!";
                                logDetailRecord.Status = ScheduleLogStatus.Failed.ToString();
                                return logDetailRecord;
                            }
                        }
                    }

                    StringBuilder htmlbody = new StringBuilder();
                    currency = accountrecords[0].Currency;
                    string navbarHtml = HtmlConstants.NAVBAR_HTML.Replace("{{BrandLogo}}", "../common/images/absa-logo.png");
                    navbarHtml = navbarHtml.Replace("{{logo}}", "../common/images/nisLogo.png");
                    navbarHtml = navbarHtml.Replace("{{Today}}", DateTime.Now.ToString("dd MMM yyyy"));
                    htmlbody.Append(HtmlConstants.CONTAINER_DIV_HTML_HEADER);

                    //start to render actual html content data
                    StringBuilder scriptHtmlRenderer = new StringBuilder();
                    StringBuilder navbar = new StringBuilder();
                    int accountCount = 0;
                    string accountNumber = string.Empty;
                    string accountType = string.Empty;
                    long accountId = 0;
                    string SavingTrendChartJson = string.Empty;
                    string SpendingTrendChartJson = string.Empty;
                    string AnalyticsChartJson = string.Empty;
                    string SavingTransactionGridJson = string.Empty;
                    string CurrentTransactionGridJson = string.Empty;

                    var newStatementPageContents = new List<StatementPageContent>();
                    statementPageContents.ToList().ForEach(it => newStatementPageContents.Add(new StatementPageContent() 
                    {
                        Id = it.Id,
                        PageId = it.PageId,
                        PageTypeId = it.PageTypeId,
                        HtmlContent = it.HtmlContent,
                        PageHeaderContent = it.PageHeaderContent,
                        PageFooterContent = it.PageFooterContent,
                        DisplayName = it.DisplayName,
                        TabClassName = it.TabClassName
                    }));
                    
                    for (int i = 0; i < statement.Pages.Count; i++)
                    {
                        var page = statement.Pages[i];
                        StatementPageContent statementPageContent = newStatementPageContents.Where(item => item.PageTypeId == page.PageTypeId && item.Id == i).FirstOrDefault();

                        if (page.PageTypeId == HtmlConstants.HOME_PAGE_TYPE_ID)
                        {
                            accountCount = 1;
                        }
                        else if (page.PageTypeId == HtmlConstants.SAVING_ACCOUNT_PAGE_TYPE_ID)
                        {
                            savingaccountrecords = accountrecords.Where(item => item.CustomerId == customer.Id && item.BatchId == batchMaster.Id && item.AccountType.ToLower().Contains("saving"))?.ToList();
                            if (savingaccountrecords == null && savingaccountrecords.Count == 0)
                            {
                                logDetailRecord.LogMessage = "Saving account master data is not available for this customer..!!";
                                logDetailRecord.Status = ScheduleLogStatus.Failed.ToString();
                                return logDetailRecord;
                            }
                            accountCount = savingaccountrecords.Count;
                        }
                        else if (page.PageTypeId == HtmlConstants.CURRENT_ACCOUNT_PAGE_TYPE_ID)
                        {
                            curerntaccountrecords = accountrecords.Where(item => item.CustomerId == customer.Id && item.BatchId == batchMaster.Id && item.AccountType.ToLower().Contains("current"))?.ToList();
                            if (curerntaccountrecords == null && curerntaccountrecords.Count == 0)
                            {
                                logDetailRecord.LogMessage = "Current account master data is not available for this customer..!!";
                                logDetailRecord.Status = ScheduleLogStatus.Failed.ToString();
                                return logDetailRecord;
                            }
                            accountCount = curerntaccountrecords.Count;
                        }

                        StringBuilder SubTabs = new StringBuilder();
                        StringBuilder PageHeaderContent = new StringBuilder(statementPageContent.PageHeaderContent);

                        string tabClassName = Regex.Replace((statementPageContent.DisplayName + "-" + page.Version), @"\s+", "-");
                        navbar.Append(" <li class='nav-item'><a class='nav-link pt-1 mainNav " + (i == 0 ? "active" : "") + " " + tabClassName + "' href='javascript:void(0);' onclick='MainTabClicked(event)'>" + statementPageContent.DisplayName + "</a> </li> ");
                        string ExtraClassName = i > 0 ? "d-none " + tabClassName : tabClassName;
                        PageHeaderContent.Replace("{{ExtraClass}}", ExtraClassName);
                        PageHeaderContent.Replace("{{DivId}}", tabClassName);

                        StringBuilder newPageContent = new StringBuilder();
                        newPageContent.Append("<div class='tab-content'>");

                        for (int x = 0; x < accountCount; x++)
                        {
                            StringBuilder pageContent = new StringBuilder(statementPageContent.HtmlContent);
                            accountNumber = string.Empty;
                            accountType = string.Empty;
                            if (page.PageTypeId == HtmlConstants.SAVING_ACCOUNT_PAGE_TYPE_ID)
                            {
                                accountNumber = savingaccountrecords[x].AccountNumber;
                                accountId = savingaccountrecords[x].Id;
                                accountType = savingaccountrecords[x].AccountType;
                            }
                            if (page.PageTypeId == HtmlConstants.CURRENT_ACCOUNT_PAGE_TYPE_ID)
                            {
                                accountNumber = curerntaccountrecords[x].AccountNumber;
                                accountId = curerntaccountrecords[x].Id;
                                accountType = curerntaccountrecords[x].AccountType;
                            }

                            string lastFourDigisOfAccountNumber = accountNumber.Length > 4 ? accountNumber.Substring(Math.Max(0, accountNumber.Length - 4)) : accountNumber;
                            if (page.PageTypeId == HtmlConstants.SAVING_ACCOUNT_PAGE_TYPE_ID || page.PageTypeId == HtmlConstants.CURRENT_ACCOUNT_PAGE_TYPE_ID)
                            {
                                if (x == 0)
                                {
                                    SubTabs.Append("<ul class='nav nav-tabs' style='margin-top:-20px;'>");
                                }
                                
                                SubTabs.Append("<li class='nav-item " + (x == 0 ? "active" : "") + "'><a id='tab" + x + "-tab' data-toggle='tab' " +
                                    "data-target='#" + (page.PageTypeId == HtmlConstants.SAVING_ACCOUNT_PAGE_TYPE_ID ? "Saving" : "Current") + "-" + lastFourDigisOfAccountNumber + "' " +
                                    " role='tab' class='nav-link " + (x == 0 ? "active" : "") + "'> Account - " + lastFourDigisOfAccountNumber + "</a></li>");

                                newPageContent.Append("<div id='"+ (page.PageTypeId == HtmlConstants.SAVING_ACCOUNT_PAGE_TYPE_ID ? "Saving" : "Current") +
                                    "-"+lastFourDigisOfAccountNumber+ "' class='tab-pane fade in " + (x == 0 ? "active show" : "") + "'>");
                                
                                if (x == accountCount - 1)
                                {
                                    SubTabs.Append("</ul>");
                                }
                            }

                            var pagewidgets = page.PageWidgets;
                            for (int j = 0; j < pagewidgets.Count; j++)
                            {
                                var widget = pagewidgets[j];
                                if (widget.WidgetId == HtmlConstants.CUSTOMER_INFORMATION_WIDGET_ID) //Customer Information Widget
                                {
                                    IList<CustomerMediaRecord> customerMedias = new List<CustomerMediaRecord>();
                                    pageContent.Replace("{{CustomerName}}", (customer.FirstName + " " + customer.MiddleName + " " + customer.LastName));
                                    pageContent.Replace("{{Address1}}", customer.AddressLine1);
                                    string address2 = (customer.AddressLine2 != "" ? customer.AddressLine2 + ", " : "") + (customer.City != "" ? customer.City + ", " : "") + (customer.State != "" ? customer.State + ", " : "") + (customer.Country != "" ? customer.Country + ", " : "") + (customer.Zip != "" ? customer.Zip : "");
                                    pageContent.Replace("{{Address2}}", address2);

                                    using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                                    {
                                        customerMedias = nISEntitiesDataContext.CustomerMediaRecords.Where(item => item.CustomerId == customer.Id && item.StatementId == statement.Identifier && item.WidgetId == widget.Identifier)?.ToList();
                                    }
                                    var custMedia = customerMedias.Where(item => item.PageId == page.Identifier && item.WidgetId == widget.Identifier)?.ToList()?.FirstOrDefault();
                                    if (custMedia != null && custMedia.VideoURL != string.Empty)
                                    {
                                        pageContent.Replace("{{VideoSource_" + statement.Identifier + "_" + page.Identifier + "_" + widget.Identifier + "}}", custMedia.VideoURL);
                                    }
                                    else
                                    {
                                        var batchDetail = batchDetails.Where(item => item.StatementId == statement.Identifier && item.WidgetId == widget.Identifier && item.PageId == page.Identifier)?.ToList()?.FirstOrDefault();
                                        if (batchDetail != null && batchDetail.VideoURL != string.Empty)
                                        {
                                            pageContent.Replace("{{VideoSource_" + statement.Identifier + "_" + page.Identifier + "_" + widget.Identifier + "}}", batchDetail.VideoURL);
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
                                    pageContent.Replace("{{AccountInfoData_" + page.Identifier + "_" + widget.Identifier + "}}", AccDivData.ToString());
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
                                                imageRecord = nISEntitiesDataContext.ImageRecords.Where(item => item.BatchId == batchMaster.Id && item.StatementId == statement.Identifier && item.PageId == page.Identifier && item.WidgetId == widget.Identifier)?.ToList()?.FirstOrDefault();
                                            }
                                            if (imageRecord != null && imageRecord.Image1 != string.Empty)
                                            {
                                                imgAssetFilepath = imageRecord.Image1;
                                            }
                                            else
                                            {
                                                var batchDetail = batchDetails.Where(item => item.StatementId == statement.Identifier && item.WidgetId == widget.Identifier && item.PageId == page.Identifier)?.ToList()?.FirstOrDefault();
                                                if (batchDetail != null && batchDetail.ImageURL != string.Empty)
                                                {
                                                    imgAssetFilepath = batchDetail.ImageURL;
                                                }
                                                else
                                                {
                                                    logDetailRecord.LogMessage = "Image not found for Page: " + page.Identifier + " and Widget: " + widget.Identifier + " for image widget..!!";
                                                    logDetailRecord.Status = ScheduleLogStatus.Failed.ToString();
                                                    break;
                                                }
                                            }
                                        }
                                        pageContent.Replace("{{ImageSource_" + statement.Identifier + "_" + page.Identifier + "_" + widget.Identifier + "}}", imgAssetFilepath);
                                    }
                                    else
                                    {
                                        logDetailRecord.LogMessage = "Image widget configuration is missing for Page: " + page.Identifier + " and Widget: " + widget.Identifier + "!!";
                                        logDetailRecord.Status = ScheduleLogStatus.Failed.ToString();
                                        break;
                                    }
                                }
                                else if (widget.WidgetId == HtmlConstants.VIDEO_WIDGET_ID) //Video widget
                                {
                                    var vdoAssetFilepath = string.Empty;
                                    if (widget.WidgetSetting != string.Empty && validationEngine.IsValidJson(widget.WidgetSetting))
                                    {
                                        dynamic widgetSetting = JObject.Parse(widget.WidgetSetting);
                                        if (widgetSetting.isEmbedded == true)//If embedded then assigned it it from widget config json source url
                                        {
                                            vdoAssetFilepath = widgetSetting.SourceUrl;
                                        }
                                        else if (widgetSetting.isPersonalize == false && widgetSetting.isEmbedded == false) //If not dynamic video, then assign selected video from asset library
                                        {
                                            vdoAssetFilepath = baseURL + "/assets/" + widgetSetting.AssetLibraryId + "/" + widgetSetting.AssetName;
                                        }
                                        else //If dynamic video, then assign it from database 
                                        {
                                            VideoRecord videoRecord = new VideoRecord();
                                            using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                                            {
                                                videoRecord = nISEntitiesDataContext.VideoRecords.Where(item => item.BatchId == batchMaster.Id && item.StatementId == statement.Identifier && item.PageId == page.Identifier && item.WidgetId == widget.Identifier)?.ToList()?.FirstOrDefault();
                                            }
                                            if (videoRecord != null && videoRecord.Video1 != string.Empty)
                                            {
                                                vdoAssetFilepath = videoRecord.Video1;
                                            }
                                            else
                                            {
                                                var batchDetail = batchDetails.Where(item => item.StatementId == statement.Identifier && item.WidgetId == widget.Identifier && item.PageId == page.Identifier)?.ToList()?.FirstOrDefault();
                                                if (batchDetail != null && batchDetail.VideoURL != string.Empty)
                                                {
                                                    vdoAssetFilepath = batchDetail.VideoURL;
                                                }
                                                else
                                                {
                                                    logDetailRecord.LogMessage = "Video not found for Page: " + page.Identifier + " and Widget: " + widget.Identifier + " for video widget..!!";
                                                    logDetailRecord.Status = ScheduleLogStatus.Failed.ToString();
                                                    break;
                                                }
                                            }
                                        }
                                        pageContent.Replace("{{VideoSource_" + statement.Identifier + "_" + page.Identifier + "_" + widget.Identifier + "}}", vdoAssetFilepath);
                                    }
                                    else
                                    {
                                        logDetailRecord.LogMessage = "Video widget configuration is missing for Page: " + page.Identifier + " and Widget: " + widget.Identifier + "!!";
                                        logDetailRecord.Status = ScheduleLogStatus.Failed.ToString();
                                        break;
                                    }
                                }
                                else if (widget.WidgetId == HtmlConstants.SUMMARY_AT_GLANCE_WIDGET_ID) //Summary at Glance Widget
                                {
                                    if (accountrecords != null && accountrecords.Count > 0)
                                    {
                                        StringBuilder accSummary = new StringBuilder();
                                        var accRecords = accountrecords.GroupBy(item => item.AccountType).ToList();
                                        accRecords.ToList().ForEach(acc =>
                                        {
                                            accSummary.Append("<tr><td>" + acc.FirstOrDefault().AccountType + "</td><td>" + acc.FirstOrDefault().Currency + "</td><td>" + acc.Sum(it => it.Balance).ToString() + "</td></tr>");
                                        });
                                        pageContent.Replace("{{AccountSummary_" + page.Identifier + "_" + widget.Identifier + "}}", accSummary.ToString());
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
                                    if (accountrecords != null && accountrecords.Count > 0)
                                    {
                                        var currentAccountRecords = accountrecords.Where(item => item.CustomerId == customer.Id && item.BatchId == batchMaster.Id && item.AccountType.ToLower().Contains("current") && item.Id == accountId)?.ToList();
                                        if (currentAccountRecords != null && currentAccountRecords.Count > 0)
                                        {
                                            var records = currentAccountRecords.GroupBy(item => item.AccountType).ToList();
                                            records?.ToList().ForEach(acc =>
                                            {
                                                var accountIndicatorClass = acc.FirstOrDefault().Indicator.ToLower().Equals("up") ? "fa fa-sort-asc text-success" : "fa fa-sort-desc text-danger";
                                                pageContent.Replace("{{AccountIndicatorClass}}", accountIndicatorClass);
                                                pageContent.Replace("{{TotalValue_" + page.Identifier + "_" + widget.Identifier + "}}", (currency + " " + acc.Sum(it => it.GrandTotal).ToString()));
                                                pageContent.Replace("{{TotalDeposit_" + page.Identifier + "_" + widget.Identifier + "}}", (currency + " " + acc.Sum(it => it.TotalDeposit).ToString()));
                                                pageContent.Replace("{{TotalSpend_" + page.Identifier + "_" + widget.Identifier + "}}", (currency + " " + acc.Sum(it => it.TotalSpend).ToString()));
                                                pageContent.Replace("{{Savings_" + page.Identifier + "_" + widget.Identifier + "}}", (currency + " " + acc.Sum(it => it.ProfitEarned).ToString()));
                                            });
                                        }
                                        else
                                        {
                                            logDetailRecord.LogMessage = "Current Account master data is not available related to Current Available Balance widget..!!";
                                            logDetailRecord.Status = ScheduleLogStatus.Failed.ToString();
                                            break;
                                        }
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
                                    if (accountrecords != null && accountrecords.Count > 0)
                                    {
                                        var savingAccountRecords = accountrecords.Where(item => item.CustomerId == customer.Id && item.BatchId == batchMaster.Id && item.AccountType.ToLower().Contains("saving") && item.Id == accountId)?.ToList();
                                        if (savingAccountRecords != null && savingAccountRecords.Count > 0)
                                        {
                                            var records = savingAccountRecords.GroupBy(item => item.AccountType).ToList();
                                            records?.ToList().ForEach(acc =>
                                            {
                                                var accountIndicatorClass = acc.FirstOrDefault().Indicator.ToLower().Equals("up") ? "fa fa-sort-asc text-success" : "fa fa-sort-desc text-danger";
                                                pageContent.Replace("{{AccountIndicatorClass}}", accountIndicatorClass);
                                                pageContent.Replace("{{TotalValue_" + page.Identifier + "_" + widget.Identifier + "}}", (currency + " " + acc.Sum(it => it.GrandTotal).ToString()));
                                                pageContent.Replace("{{TotalDeposit_" + page.Identifier + "_" + widget.Identifier + "}}", (currency + " " + acc.Sum(it => it.TotalDeposit).ToString()));
                                                pageContent.Replace("{{TotalSpend_" + page.Identifier + "_" + widget.Identifier + "}}", (currency + " " + acc.Sum(it => it.TotalSpend).ToString()));
                                                pageContent.Replace("{{Savings_" + page.Identifier + "_" + widget.Identifier + "}}", (currency + " " + acc.Sum(it => it.ProfitEarned).ToString()));
                                            });
                                        }
                                        else
                                        {
                                            logDetailRecord.LogMessage = "Saving Account master data is not available related to Saving Available Balance widget..!!";
                                            logDetailRecord.Status = ScheduleLogStatus.Failed.ToString();
                                            break;
                                        }
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
                                        accountTransactions = nISEntitiesDataContext.AccountTransactionRecords.Where(item => item.CustomerId == customer.Id && item.BatchId == batchMaster.Id && item.AccountType.ToLower().Contains("saving") && item.AccountId == accountId)?.OrderByDescending(item => item.TransactionDate)?.ToList();

                                        StringBuilder transaction = new StringBuilder();
                                        StringBuilder selectOption = new StringBuilder();
                                        if (accountTransactions != null && accountTransactions.Count > 0)
                                        {
                                            IList<AccountTransaction> transactions = new List<AccountTransaction>();
                                            pageContent.Replace("{{Currency}}", currency);
                                            accountTransactions.ToList().ForEach(trans =>
                                            {
                                                AccountTransaction accountTransaction = new AccountTransaction();
                                                accountTransaction.AccountType = trans.AccountType;
                                                accountTransaction.TransactionDate = Convert.ToDateTime(trans.TransactionDate).ToShortDateString();
                                                accountTransaction.TransactionType = trans.TransactionType;
                                                accountTransaction.FCY = trans.FCY;
                                                accountTransaction.Narration = trans.Narration;
                                                accountTransaction.LCY = trans.LCY;
                                                accountTransaction.CurrentRate = trans.CurrentRate;
                                                transactions.Add(accountTransaction);
                                            });
                                            string savingtransactionjson = JsonConvert.SerializeObject(transactions);
                                            if (savingtransactionjson != null && savingtransactionjson != string.Empty)
                                            {
                                                var distinctNaration = accountTransactions.Select(item => item.Narration).Distinct().ToList();
                                                distinctNaration.ToList().ForEach(item =>
                                                {
                                                    selectOption.Append("<option value='" + item + "'> " + item + "</option>");
                                                });

                                                SavingTransactionGridJson = "savingtransactiondata" + accountId + page.Identifier + "=" + savingtransactionjson;
                                                this.utility.WriteToJsonFile(SavingTransactionGridJson, "savingtransactiondetail" + accountId + page.Identifier + ".json", batchMaster.Id, customer.Id);
                                                scriptHtmlRenderer.Append("<script type='text/javascript' src='./savingtransactiondetail" + accountId + page.Identifier + ".json'></script>");

                                                StringBuilder scriptval = new StringBuilder(HtmlConstants.SAVING_TRANSACTION_DETAIL_GRID_WIDGET_SCRIPT);
                                                scriptval.Replace("SavingTransactionTable", "SavingTransactionTable" + accountId + page.Identifier);
                                                scriptval.Replace("savingtransactiondata", "savingtransactiondata" + accountId + page.Identifier);
                                                scriptval.Replace("savingShowAll", "savingShowAll" + accountId + page.Identifier);
                                                scriptval.Replace("filterStatus", "filterStatus" + accountId + page.Identifier);
                                                scriptval.Replace("ResetGrid", "ResetGrid" + accountId + page.Identifier);
                                                scriptval.Replace("PrintGrid", "PrintGrid" + accountId + page.Identifier);
                                                scriptval.Replace("savingtransactionRadio", "savingtransactionRadio" + accountId + page.Identifier);
                                                scriptHtmlRenderer.Append(scriptval);

                                                pageContent.Replace("savingtransactiondata", "savingtransactiondata" + accountId + page.Identifier);
                                                pageContent.Replace("savingShowAll", "savingShowAll" + accountId + page.Identifier);
                                                pageContent.Replace("filterStatus", "filterStatus" + accountId + page.Identifier);
                                                pageContent.Replace("ResetGrid", "ResetGrid" + accountId + page.Identifier);
                                                pageContent.Replace("PrintGrid", "PrintGrid" + accountId + page.Identifier);
                                                pageContent.Replace("savingtransactionRadio", "savingtransactionRadio" + accountId + page.Identifier);
                                                pageContent.Replace("{{SelectOption_" + page.Identifier + "_" + widget.Identifier + "}}", selectOption.ToString());
                                                pageContent.Replace("SavingTransactionTable", "SavingTransactionTable" + accountId + page.Identifier);
                                            }
                                            else
                                            {
                                                transaction.Append("<tr><td colspan='7' class='text-danger text-center'><span>No data available</span></td></tr>");
                                            }
                                        }
                                        else
                                        {
                                            transaction.Append("<tr><td colspan='7' class='text-danger text-center'><span>No data available</span></td></tr>");
                                        }

                                        pageContent.Replace("{{AccountTransactionDetails_" + page.Identifier + "_" + widget.Identifier + "}}", transaction.ToString());
                                    }
                                }
                                else if (widget.WidgetId == HtmlConstants.CURRENT_TRANSACTION_WIDGET_ID)
                                {
                                    IList<AccountTransactionRecord> accountTransactions = new List<AccountTransactionRecord>();
                                    using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                                    {
                                        accountTransactions = nISEntitiesDataContext.AccountTransactionRecords.Where(item => item.CustomerId == customer.Id && item.BatchId == batchMaster.Id && item.AccountType.ToLower().Contains("current") && item.AccountId == accountId)?.OrderByDescending(item => item.TransactionDate)?.ToList();

                                        StringBuilder transaction = new StringBuilder();
                                        StringBuilder selectOption = new StringBuilder();
                                        if (accountTransactions != null && accountTransactions.Count > 0)
                                        {
                                            IList<AccountTransaction> transactions = new List<AccountTransaction>();
                                            pageContent.Replace("{{Currency}}", currency);
                                            accountTransactions.ToList().ForEach(trans =>
                                            {
                                                AccountTransaction accountTransaction = new AccountTransaction();
                                                accountTransaction.AccountType = trans.AccountType;
                                                accountTransaction.TransactionDate = Convert.ToDateTime(trans.TransactionDate).ToShortDateString();
                                                accountTransaction.TransactionType = trans.TransactionType;
                                                accountTransaction.FCY = trans.FCY;
                                                accountTransaction.Narration = trans.Narration;
                                                accountTransaction.LCY = trans.LCY;
                                                accountTransaction.CurrentRate = trans.CurrentRate;
                                                transactions.Add(accountTransaction);
                                            });
                                            string currenttransactionjson = JsonConvert.SerializeObject(transactions);
                                            if (currenttransactionjson != null && currenttransactionjson != string.Empty)
                                            {
                                                var distinctNaration = accountTransactions.Select(item => item.Narration).Distinct().ToList();
                                                distinctNaration.ToList().ForEach(item =>
                                                {
                                                    selectOption.Append("<option value='" + item + "'> " + item + "</option>");
                                                });

                                                CurrentTransactionGridJson = "currenttransactiondata" + accountId + page.Identifier + "=" + currenttransactionjson;
                                                this.utility.WriteToJsonFile(CurrentTransactionGridJson, "currenttransactiondetail" + accountId + page.Identifier + ".json", batchMaster.Id, customer.Id);
                                                scriptHtmlRenderer.Append("<script type='text/javascript' src='./currenttransactiondetail" + accountId + page.Identifier + ".json'></script>");

                                                StringBuilder scriptval = new StringBuilder(HtmlConstants.CURRENT_TRANSACTION_DETAIL_GRID_WIDGET_SCRIPT);
                                                scriptval.Replace("CurrentTransactionTable", "CurrentTransactionTable" + accountId + page.Identifier);
                                                scriptval.Replace("currenttransactiondata", "currenttransactiondata" + accountId + page.Identifier);
                                                scriptval.Replace("currentShowAll", "currentShowAll" + accountId + page.Identifier);
                                                scriptval.Replace("filterStatus", "filterStatus" + accountId + page.Identifier);
                                                scriptval.Replace("ResetGrid", "ResetGrid" + accountId + page.Identifier);
                                                scriptval.Replace("PrintGrid", "PrintGrid" + accountId + page.Identifier);
                                                scriptval.Replace("currenttransactionRadio", "currenttransactionRadio" + accountId + page.Identifier);
                                                scriptHtmlRenderer.Append(scriptval);

                                                pageContent.Replace("currentShowAll", "currentShowAll" + accountId + page.Identifier);
                                                pageContent.Replace("filterStatus", "filterStatus" + accountId + page.Identifier);
                                                pageContent.Replace("ResetGrid", "ResetGrid" + accountId + page.Identifier);
                                                pageContent.Replace("PrintGrid", "PrintGrid" + accountId + page.Identifier);
                                                pageContent.Replace("currenttransactionRadio", "currenttransactionRadio" + accountId + page.Identifier);
                                                pageContent.Replace("{{SelectOption_" + page.Identifier + "_" + widget.Identifier + "}}", selectOption.ToString());
                                                pageContent.Replace("CurrentTransactionTable", "CurrentTransactionTable" + accountId + page.Identifier);
                                            }
                                            else
                                            {
                                                transaction.Append("<tr><td colspan='7' class='text-danger text-center'><span>No data available</span></td></tr>");
                                            }
                                        }
                                        else
                                        {
                                            transaction.Append("<tr><td colspan='7' class='text-danger text-center'><span>No data available</span></td></tr>");
                                        }
                                        pageContent.Replace("{{AccountTransactionDetails_" + page.Identifier + "_" + widget.Identifier + "}}", transaction.ToString());
                                    }
                                }
                                else if (widget.WidgetId == HtmlConstants.TOP_4_INCOME_SOURCES_WIDGET_ID)
                                {
                                    IList<Top4IncomeSourcesRecord> top4IncomeSources = new List<Top4IncomeSourcesRecord>();
                                    using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                                    {
                                        top4IncomeSources = nISEntitiesDataContext.Top4IncomeSourcesRecord.Where(item => item.CustomerId == customer.Id && item.BatchId == batchMaster.Id)?.OrderByDescending(item => item.CurrentSpend)?.Take(4)?.ToList();
                                    }

                                    StringBuilder incomeSources = new StringBuilder();
                                    if (top4IncomeSources != null && top4IncomeSources.Count > 0)
                                    {
                                        top4IncomeSources.ToList().ForEach(src =>
                                        {
                                            var spendIndicatorClass = src.CurrentSpend > src.AverageSpend ? "fa-sort-asc text-danger align-text-top" : "fa-sort-desc text-success";
                                            incomeSources.Append("<tr><td class='float-left'>" + src.Source + "</td>" + "<td> " + src.CurrentSpend +
                                                "" + "</td><td class='align-text-top'>" + "<span class='fa fa-2x " + spendIndicatorClass + "' " +
                                                "aria-hidden='true'>" + "</span>&nbsp;" + src.AverageSpend + " " + "</td></tr>");
                                        });
                                    }
                                    else
                                    {
                                        incomeSources.Append("<tr><td colspan='3' class='text-danger text-center'><div style='margin-top: 20px;'>No data available</div></td></tr>");
                                    }
                                    pageContent.Replace("{{IncomeSourceList_" + page.Identifier + "_" + widget.Identifier + "}}", incomeSources.ToString());
                                }
                                else if (widget.WidgetId == HtmlConstants.ANALYTICS_WIDGET_ID)
                                {
                                    if (accountrecords.Count > 0)
                                    {
                                        IList<AccountMasterRecord> accounts = new List<AccountMasterRecord>();
                                        var records = accountrecords.GroupBy(item => item.AccountType).ToList();
                                        records.ToList().ForEach(acc => accounts.Add(new AccountMasterRecord()
                                        {
                                            AccountType = acc.FirstOrDefault().AccountType,
                                            Percentage = acc.Average(item => item.Percentage == null ? 0 : item.Percentage)
                                        }));

                                        string accountsJson = JsonConvert.SerializeObject(accounts);
                                        if (accountsJson != null && accountsJson != string.Empty)
                                        {
                                            AnalyticsChartJson = "analyticsdata=" + accountsJson;
                                        }
                                        else
                                        {
                                            AnalyticsChartJson = "analyticsdata=[]";
                                        }
                                    }
                                    else
                                    {
                                        AnalyticsChartJson = "analyticsdata=[]";
                                    }

                                    this.utility.WriteToJsonFile(AnalyticsChartJson, "analyticschartdata.json", batchMaster.Id, customer.Id);
                                    scriptHtmlRenderer.Append("<script type='text/javascript' src='./analyticschartdata.json'></script>");
                                    pageContent.Replace("analyticschartcontainer", "analyticschartcontainer" + page.Identifier);
                                    scriptHtmlRenderer.Append(HtmlConstants.ANALYTICS_CHART_WIDGET_SCRIPT.Replace("analyticschartcontainer", "analyticschartcontainer" + page.Identifier));
                                }
                                else if (widget.WidgetId == HtmlConstants.SAVING_TREND_WIDGET_ID)
                                {
                                    IList<SavingTrendRecord> savingtrends = new List<SavingTrendRecord>();
                                    using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                                    {
                                        savingtrends = nISEntitiesDataContext.SavingTrendRecords.Where(item => item.CustomerId == customer.Id && item.BatchId == batchMaster.Id && item.AccountId == accountId).ToList();
                                    }
                                    if (savingtrends != null && savingtrends.Count > 0)
                                    {
                                        IList<SavingTrend> savingTrendRecords = new List<SavingTrend>();
                                        int mnth = DateTime.Now.Month;
                                        for (int t = savingtrends.Count; t > 0; t--)
                                        {
                                            string month = this.utility.getMonth(mnth);
                                            var lst = savingtrends.Where(it => it.Month.ToLower().Contains(month.ToLower()))?.ToList();
                                            if (lst != null && lst.Count > 0)
                                            {
                                                SavingTrend trend = new SavingTrend();
                                                trend.Month = lst[0].Month;
                                                trend.NumericMonth = mnth;
                                                trend.Income = lst[0].Income ?? 0;
                                                trend.IncomePercentage = lst[0].IncomePercentage ?? 0;
                                                trend.SpendAmount = lst[0].SpendAmount;
                                                trend.SpendPercentage = lst[0].SpendPercentage ?? 0;
                                                savingTrendRecords.Add(trend);
                                            }
                                            else
                                            {
                                                logDetailRecord.LogMessage = "Invalid consecutive month data for Saving trend widget..!!";
                                                logDetailRecord.Status = ScheduleLogStatus.Failed.ToString();
                                                break;
                                            }
                                            mnth = mnth - 1 == 0 ? 12 : mnth - 1;
                                        }

                                        var records = savingTrendRecords.OrderByDescending(item => item.NumericMonth).Take(6).ToList();
                                        string savingtrendjson = JsonConvert.SerializeObject(records);
                                        if (savingtrendjson != null && savingtrendjson != string.Empty)
                                        {
                                            SavingTrendChartJson = "savingdata" + accountId + page.Identifier + "=" + savingtrendjson;
                                        }
                                        else
                                        {
                                            SavingTrendChartJson = "savingdata" + accountId + page.Identifier + "=[]";
                                        }
                                    }
                                    else
                                    {
                                        SavingTrendChartJson = "savingdata" + accountId + page.Identifier + "=[]";
                                    }

                                    this.utility.WriteToJsonFile(SavingTrendChartJson, "savingtrenddata" + accountId + page.Identifier + ".json", batchMaster.Id, customer.Id);
                                    scriptHtmlRenderer.Append("<script type='text/javascript' src='./savingtrenddata" + accountId + page.Identifier + ".json'></script>");

                                    pageContent.Replace("savingTrendscontainer", "savingTrendscontainer" + accountId + page.Identifier);
                                    var scriptval = HtmlConstants.SAVING_TREND_CHART_WIDGET_SCRIPT.Replace("savingTrendscontainer", "savingTrendscontainer" + accountId + page.Identifier).Replace("savingdata", "savingdata" + accountId + page.Identifier);
                                    scriptHtmlRenderer.Append(scriptval);
                                }
                                else if (widget.WidgetId == HtmlConstants.SPENDING_TREND_WIDGET_ID)
                                {
                                    IList<SavingTrendRecord> spendingtrends = new List<SavingTrendRecord>();
                                    using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                                    {
                                        spendingtrends = nISEntitiesDataContext.SavingTrendRecords.Where(item => item.CustomerId == customer.Id && item.BatchId == batchMaster.Id && item.AccountId == accountId).ToList();
                                    }
                                    if (spendingtrends != null && spendingtrends.Count > 0)
                                    {
                                        IList<SavingTrend> trends = new List<SavingTrend>();
                                        int mnth = DateTime.Now.Month;
                                        for (int t = spendingtrends.Count; t > 0; t--)
                                        {
                                            string month = this.utility.getMonth(mnth);
                                            var lst = spendingtrends.Where(it => it.Month.ToLower().Contains(month.ToLower()))?.ToList();
                                            if (lst != null && lst.Count > 0)
                                            {
                                                SavingTrend trend = new SavingTrend();
                                                trend.Month = lst[0].Month;
                                                trend.NumericMonth = mnth;
                                                trend.Income = lst[0].Income ?? 0;
                                                trend.IncomePercentage = lst[0].IncomePercentage ?? 0;
                                                trend.SpendAmount = lst[0].SpendAmount;
                                                trend.SpendPercentage = lst[0].SpendPercentage ?? 0;
                                                trends.Add(trend);
                                            }
                                            else
                                            {
                                                logDetailRecord.LogMessage = "Invalid consecutive month data for Spending trend widget..!!";
                                                logDetailRecord.Status = ScheduleLogStatus.Failed.ToString();
                                                break;
                                            }
                                            mnth = mnth - 1 == 0 ? 12 : mnth - 1;
                                        }

                                        var records = trends.OrderByDescending(item => item.NumericMonth).Take(6).ToList();
                                        string spendingtrendjson = JsonConvert.SerializeObject(records);
                                        if (spendingtrendjson != null && spendingtrendjson != string.Empty)
                                        {
                                            SpendingTrendChartJson = "spendingdata" + accountId + page.Identifier + "=" + spendingtrendjson;
                                        }
                                        else
                                        {
                                            SpendingTrendChartJson = "spendingdata" + accountId + page.Identifier + "=[]";
                                        }
                                    }
                                    else
                                    {
                                        SpendingTrendChartJson = "spendingdata" + accountId + page.Identifier + "=[]";
                                    }

                                    this.utility.WriteToJsonFile(SpendingTrendChartJson, "spendingtrenddata" + accountId + page.Identifier + ".json", batchMaster.Id, customer.Id);
                                    scriptHtmlRenderer.Append("<script type='text/javascript' src='./spendingtrenddata" + accountId + page.Identifier + ".json'></script>");

                                    pageContent.Replace("spendingTrendscontainer", "spendingTrendscontainer" + accountId + page.Identifier);
                                    var scriptval = HtmlConstants.SPENDING_TREND_CHART_WIDGET_SCRIPT.Replace("spendingTrendscontainer", "spendingTrendscontainer" + accountId + page.Identifier).Replace("spendingdata", "spendingdata" + accountId + page.Identifier);
                                    scriptHtmlRenderer.Append(scriptval);
                                }
                                else if (widget.WidgetId == HtmlConstants.REMINDER_AND_RECOMMENDATION_WIDGET_ID)
                                {
                                    IList<ReminderAndRecommendationRecord> reminderAndRecommendations = new List<ReminderAndRecommendationRecord>();
                                    using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                                    {
                                        reminderAndRecommendations = nISEntitiesDataContext.ReminderAndRecommendationRecords.Where(item => item.CustomerId == customer.Id && item.BatchId == batchMaster.Id)?.ToList();
                                    }

                                    StringBuilder reminderstr = new StringBuilder();
                                    if (reminderAndRecommendations != null && reminderAndRecommendations.Count > 0)
                                    {
                                        reminderstr.Append("<table class='width100'><thead><tr> <td class='width75 text-left'></td><td style='color:red;float: right;'><i class='fa fa-caret-left fa-2x float-left' aria-hidden='true'></i><span class='mt-2 d-inline-block ml-2'>Click</span></td></tr></thead><tbody>");
                                        reminderAndRecommendations.ToList().ForEach(item =>
                                        {
                                            string targetlink = item.TargetURL != null && item.TargetURL != string.Empty ? item.TargetURL : "javascript:void(0)";
                                            reminderstr.Append("<tr><td class='width75 text-left' style='background-color: #dce3dc;'><label>" +
                                                item.Description + "</label></td><td class='width25'><a href='" + targetlink + "'>" +
                                                "<i class='fa fa-caret-left fa-2x' style='color:red' aria-hidden='true'>" +
                                                "</i><span class='mt-2 d-inline-block ml-2'>" + item.LabelText + "</span></a></td></tr>");
                                        });
                                        reminderstr.Append("</tbody></table>");
                                    }
                                    else
                                    {
                                        reminderstr.Append("<table class='width100'><tr><td class='text-danger text-center'><div style='margin-top: 20px;'>No data available</div></td></tr></table>");
                                    }
                                    pageContent.Replace("{{ReminderAndRecommdationDataList_" + page.Identifier + "_" + widget.Identifier + "}}", reminderstr.ToString());
                                }
                            }

                            if (accountNumber != string.Empty)
                            {
                                StatementMetadataRecord statementMetadataRecord = new StatementMetadataRecord();
                                statementMetadataRecord.AccountNumber = accountNumber;
                                statementMetadataRecord.AccountType = accountType;
                                statementMetadataRecord.CustomerId = customer.Id;
                                statementMetadataRecord.CustomerName = customer.FirstName + (customer.MiddleName == string.Empty ? "" : " " + customer.MiddleName) + " " + customer.LastName;
                                statementMetadataRecord.StatementPeriod = customer.StatementPeriod;
                                statementMetadataRecord.StatementId = statement.Identifier;
                                statementMetadataRecords.Add(statementMetadataRecord);
                            }

                            newPageContent.Append(pageContent);
                            if (page.PageTypeId == HtmlConstants.SAVING_ACCOUNT_PAGE_TYPE_ID || page.PageTypeId == HtmlConstants.CURRENT_ACCOUNT_PAGE_TYPE_ID)
                            {
                                newPageContent.Append("</div>");
                            }
                                
                            if (x == accountCount - 1)
                            {
                                newPageContent.Append("</div>");
                            }
                        }

                        PageHeaderContent.Replace("{{SubTabs}}", SubTabs.ToString());
                        statementPageContent.PageHeaderContent = PageHeaderContent.ToString();
                        statementPageContent.HtmlContent = newPageContent.ToString();
                        //newStatementPageContents.Add(statementPageContent);
                    }

                    newStatementPageContents.ToList().ForEach(page => {
                        htmlbody.Append(page.PageHeaderContent);
                        htmlbody.Append(page.HtmlContent);
                        htmlbody.Append(page.PageFooterContent);
                    });

                    htmlbody.Append(HtmlConstants.CONTAINER_DIV_HTML_FOOTER);

                    navbarHtml = navbarHtml.Replace("{{NavItemList}}", navbar.ToString());

                    StringBuilder finalHtml = new StringBuilder();
                    finalHtml.Append(HtmlConstants.HTML_HEADER);
                    finalHtml.Append(navbarHtml);
                    finalHtml.Append(htmlbody.ToString());
                    finalHtml.Append(HtmlConstants.HTML_FOOTER);
                    finalHtml.Replace("{{ChartScripts}}", scriptHtmlRenderer.ToString());

                    if (logDetailRecord.Status != ScheduleLogStatus.Failed.ToString())
                    {
                        string fileName = "Statement_" + customer.Id + "_" + statement.Identifier + "_" + DateTime.Now.ToString().Replace("-", "_").Replace(":", "_").Replace(" ", "_").Replace('/', '_') + ".html";
                        string filePath = this.utility.WriteToFile(finalHtml.ToString(), fileName, batchMaster.Id, customer.Id);

                        logDetailRecord.StatementFilePath = filePath;
                        logDetailRecord.Status = ScheduleLogStatus.Completed.ToString();
                        logDetailRecord.LogMessage = "Statement generated successfully..!!";
                        logDetailRecord.StatementMetadataRecords = statementMetadataRecords;
                    }
                }

                return logDetailRecord;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method help to generate common statement for complete batch
        /// </summary>
        /// <param name="statement"> the statement object </param>
        /// <param name="statementPageContents"> the statement page html content list</param>
        /// <param name="batchMaster"> the batch master object </param>
        /// <param name="baseURL"> the base URL of API </param>
        private string GenerateCommonStatements(Statement statement, IList<StatementPageContent> statementPageContents, string baseURL)
        {
            try
            {
                //start to render common html content data
                StringBuilder htmlbody = new StringBuilder();
                string navbarHtml = HtmlConstants.NAVBAR_HTML.Replace("{{BrandLogo}}", "../common/images/absa-logo.png");
                navbarHtml = navbarHtml.Replace("{{logo}}", "../common/images/nisLogo.png");
                navbarHtml = navbarHtml.Replace("{{Today}}", DateTime.Now.ToString("dd MMM yyyy"));
                htmlbody.Append(HtmlConstants.CONTAINER_DIV_HTML_HEADER);

                //start to render actual html content data
                StringBuilder scriptHtmlRenderer = new StringBuilder();
                StringBuilder navbar = new StringBuilder();
                var newStatementPageContents = new List<StatementPageContent>();
                statementPageContents.ToList().ForEach(it => newStatementPageContents.Add(new StatementPageContent()
                {
                    Id = it.Id,
                    PageId = it.PageId,
                    PageTypeId = it.PageTypeId,
                    HtmlContent = it.HtmlContent,
                    PageHeaderContent = it.PageHeaderContent,
                    PageFooterContent = it.PageFooterContent,
                    DisplayName = it.DisplayName,
                    TabClassName = it.TabClassName
                }));
                for (int i = 0; i < statement.Pages.Count; i++)
                {
                    var page = statement.Pages[i];
                    StatementPageContent statementPageContent = newStatementPageContents.Where(item => item.PageTypeId == page.PageTypeId && item.Id == i).FirstOrDefault();
                    StringBuilder pageContent = new StringBuilder(statementPageContent.HtmlContent);

                    StringBuilder SubTabs = new StringBuilder();
                    StringBuilder PageHeaderContent = new StringBuilder(statementPageContent.PageHeaderContent);

                    string tabClassName = Regex.Replace((statementPageContent.DisplayName + "-" + page.Version), @"\s+", "-");
                    navbar.Append(" <li class='nav-item'><a class='nav-link pt-1 mainNav " + (i == 0 ? "active" : "") + " " + tabClassName + "' href='javascript:void(0);' onclick='MainTabClicked(event)'>" + statementPageContent.DisplayName + "</a> </li> ");
                    string ExtraClassName = i > 0 ? "d-none " + tabClassName : tabClassName;
                    PageHeaderContent.Replace("{{ExtraClass}}", ExtraClassName);
                    PageHeaderContent.Replace("{{DivId}}", tabClassName);

                    StringBuilder newPageContent = new StringBuilder();
                    newPageContent.Append("<div class='tab-content'>");

                    if (page.PageTypeId == HtmlConstants.SAVING_ACCOUNT_PAGE_TYPE_ID || page.PageTypeId == HtmlConstants.CURRENT_ACCOUNT_PAGE_TYPE_ID)
                    {
                        SubTabs.Append("<ul class='nav nav-tabs' style='margin-top:-20px;'>");
                        SubTabs.Append("<li class='nav-item active'><a id='tab1-tab' data-toggle='tab' " + "data-target='#" + (page.PageTypeId == 
                            HtmlConstants.SAVING_ACCOUNT_PAGE_TYPE_ID ? "Saving" : "Current") + "-' role='tab' class='nav-link active'> Account - 6789</a></li>");
                        SubTabs.Append("</ul>");

                        newPageContent.Append("<div id='" + (page.PageTypeId == HtmlConstants.SAVING_ACCOUNT_PAGE_TYPE_ID ? "Saving" : "Current") +"-6789' " +
                            "class='tab-pane fade in active show'>");
                    }

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
                                pageContent.Replace("{{VideoSource_" + statement.Identifier + "_" + page.Identifier + "_" + widget.Identifier + "}}", baseURL + "\\Resources\\sampledata\\SampleVideo.mp4");

                                string customerName = customerInfo.FirstName + " " + customerInfo.MiddleName + " " + customerInfo.LastName;
                                pageContent.Replace("{{CustomerName}}", customerName);

                                string address1 = customerInfo.AddressLine1 + ", " + customerInfo.AddressLine2 + ",";
                                pageContent.Replace("{{Address1}}", address1);

                                string address2 = (customerInfo.City != "" ? customerInfo.City + "," : "") + (customerInfo.State != "" ? customerInfo.State + "," : "") + (customerInfo.Country != "" ? customerInfo.Country + "," : "") + (customerInfo.Zip != "" ? customerInfo.Zip : "");
                                pageContent.Replace("{{Address2}}", address2);
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
                            pageContent.Replace("{{AccountInfoData_" + page.Identifier + "_" + widget.Identifier + "}}", AccDivData.ToString());
                        }
                        else if (widget.WidgetId == HtmlConstants.IMAGE_WIDGET_ID)
                        {
                            var imgAssetFilepath = baseURL + "\\Resources\\sampledata\\icon-image.png";
                            if (widget.WidgetSetting != string.Empty && validationEngine.IsValidJson(widget.WidgetSetting))
                            {
                                dynamic widgetSetting = JObject.Parse(widget.WidgetSetting);
                                if (widgetSetting.isPersonalize == false)
                                {
                                    imgAssetFilepath = baseURL + "/assets/" + widgetSetting.AssetLibraryId + "/" + widgetSetting.AssetName;
                                }
                            }
                            pageContent.Replace("{{ImageSource_" + statement.Identifier + "_" + page.Identifier + "_" + widget.Identifier + "}}", imgAssetFilepath);
                        }
                        else if (widget.WidgetId == HtmlConstants.VIDEO_WIDGET_ID)
                        {
                            var vdoAssetFilepath = baseURL + "\\Resources\\sampledata\\SampleVideo.mp4";
                            if (widget.WidgetSetting != string.Empty && validationEngine.IsValidJson(widget.WidgetSetting))
                            {
                                dynamic widgetSetting = JObject.Parse(widget.WidgetSetting);
                                if (widgetSetting.isEmbedded == true)
                                {
                                    vdoAssetFilepath = widgetSetting.SourceUrl;
                                }
                                else if (widgetSetting.isPersonalize == false && widgetSetting.isEmbedded == false)
                                {
                                    vdoAssetFilepath = baseURL + "/assets/" + widgetSetting.AssetLibraryId + "/" + widgetSetting.AssetName;
                                }
                            }
                            pageContent.Replace("{{VideoSource_" + statement.Identifier + "_" + page.Identifier + "_" + widget.Identifier + "}}", vdoAssetFilepath);
                        }
                        else if (widget.WidgetId == HtmlConstants.SUMMARY_AT_GLANCE_WIDGET_ID)
                        {
                            string accountBalanceDataJson = "[{\"AccountType\":\"Saving Account\",\"Currency\":\"$\",\"Amount\":\"87356\"}" +
                                ",{\"AccountType\":\"Current Account\",\"Currency\":\"$\",\"Amount\":\"18654\"},{\"AccountType\":" +
                                "\"Recurring Account\",\"Currency\":\"$\",\"Amount\":\"54367\"},{\"AccountType\":\"Wealth\",\"Currency\"" +
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
                                    pageContent.Replace("{{AccountSummary_" + page.Identifier + "_" + widget.Identifier + "}}", accSummary.ToString());
                                }
                            }
                        }
                        else if (widget.WidgetId == HtmlConstants.CURRENT_AVAILABLE_BALANCE_WIDGET_ID)
                        {
                            string currentAvailBalanceJson = "{'GrandTotal':'32,453,23', 'TotalDeposit':'16,250,00', 'TotalSpend':'16,254,00', 'ProfitEarned':'1,430,00 ', 'Currency':'R', 'Balance': '14,768,80', 'AccountNumber': 'J566565TR678ER', 'AccountType': 'Current', 'Indicator': 'Up'}";
                            if (currentAvailBalanceJson != string.Empty && validationEngine.IsValidJson(currentAvailBalanceJson))
                            {
                                AccountMaster accountMaster = JsonConvert.DeserializeObject<AccountMaster>(currentAvailBalanceJson);
                                var accountIndicatorClass = accountMaster.Indicator.ToLower().Equals("up") ? "fa fa-sort-asc text-success" : "fa fa-sort-desc text-danger";
                                pageContent.Replace("{{AccountIndicatorClass}}", accountIndicatorClass);
                                pageContent.Replace("{{TotalValue_" + page.Identifier + "_" + widget.Identifier + "}}", (accountMaster.Currency + accountMaster.GrandTotal));
                                pageContent.Replace("{{TotalDeposit_" + page.Identifier + "_" + widget.Identifier + "}}", (accountMaster.Currency + accountMaster.TotalDeposit));
                                pageContent.Replace("{{TotalSpend_" + page.Identifier + "_" + widget.Identifier + "}}", (accountMaster.Currency + accountMaster.TotalSpend));
                                pageContent.Replace("{{Savings_" + page.Identifier + "_" + widget.Identifier + "}}", (accountMaster.Currency + accountMaster.ProfitEarned));
                            }
                        }
                        else if (widget.WidgetId == HtmlConstants.SAVING_AVAILABLE_BALANCE_WIDGET_ID)
                        {
                            string savingAvailBalanceJson = "{'GrandTotal':'26,453,23', 'TotalDeposit':'13,530,00', 'TotalSpend':'12,124,00', 'ProfitEarned':'2,340,00 ', 'Currency':'R', 'Balance': '19,456,80', 'AccountNumber': 'J566565TR678ER', 'AccountType': 'Saving', 'Indicator': 'Up'}";
                            if (savingAvailBalanceJson != string.Empty && validationEngine.IsValidJson(savingAvailBalanceJson))
                            {
                                AccountMaster accountMaster = JsonConvert.DeserializeObject<AccountMaster>(savingAvailBalanceJson);
                                var accountIndicatorClass = accountMaster.Indicator.ToLower().Equals("up") ? "fa fa-sort-asc text-success" : "fa fa-sort-desc text-danger";
                                pageContent.Replace("{{AccountIndicatorClass}}", accountIndicatorClass);
                                pageContent.Replace("{{TotalValue_" + page.Identifier + "_" + widget.Identifier + "}}", (accountMaster.Currency + accountMaster.GrandTotal));
                                pageContent.Replace("{{TotalDeposit_" + page.Identifier + "_" + widget.Identifier + "}}", (accountMaster.Currency + accountMaster.TotalDeposit));
                                pageContent.Replace("{{TotalSpend_" + page.Identifier + "_" + widget.Identifier + "}}", (accountMaster.Currency + accountMaster.TotalSpend));
                                pageContent.Replace("{{Savings_" + page.Identifier + "_" + widget.Identifier + "}}", (accountMaster.Currency + accountMaster.ProfitEarned));
                            }
                        }
                        else if (widget.WidgetId == HtmlConstants.SAVING_TRANSACTION_WIDGET_ID)
                        {
                            StringBuilder selectOption = new StringBuilder();
                            var distinctNaration = new string[] { "NXT TXN: IIFL IIFL6574562", "NXT TXN: IIFL IIFL6574563", "NXT TXN: IIFL IIFL3557346", "NXT TXN: IIFL RTED87978947 REFUND", "NXT TXN: IIFL IIFL896452896ERE", "NXT TXN: IIFL IIFL8965435", "NXT TXN: IIFL FGTR454565JHGKD", "NXT TXN: OFFICE RENT 798789DFGH", "NXT TXN: IIFL IIFL0034212", "NXT TXN: IIFL IIFL045678DFGH" };
                            distinctNaration.ToList().ForEach(item =>
                            {
                                selectOption.Append("<option value='" + item + "'> " + item + "</option>");
                            });

                            scriptHtmlRenderer.Append("<script type='text/javascript' src='" + baseURL + "\\Resources\\sampledata\\savingtransactiondetail.json'></script>");
                            StringBuilder scriptval = new StringBuilder(HtmlConstants.SAVING_TRANSACTION_DETAIL_GRID_WIDGET_SCRIPT);
                            scriptval.Replace("SavingTransactionTable", "SavingTransactionTable" + page.Identifier);
                            scriptval.Replace("savingtransactiondata", "savingtransactiondata" + page.Identifier);
                            scriptval.Replace("savingShowAll", "savingShowAll" + page.Identifier);
                            scriptval.Replace("filterStatus", "filterStatus" + page.Identifier);
                            scriptval.Replace("ResetGrid", "ResetGrid" + page.Identifier);
                            scriptval.Replace("PrintGrid", "PrintGrid" + page.Identifier);
                            scriptval.Replace("savingtransaction", "savingtransaction" + page.Identifier);
                            scriptHtmlRenderer.Append(scriptval);

                            pageContent.Replace("savingtransactiondata", "savingtransactiondata" + page.Identifier);
                            pageContent.Replace("savingShowAll", "savingShowAll" + page.Identifier);
                            pageContent.Replace("filterStatus", "filterStatus" + page.Identifier);
                            pageContent.Replace("ResetGrid", "ResetGrid" + page.Identifier);
                            pageContent.Replace("PrintGrid", "PrintGrid" + page.Identifier);
                            pageContent.Replace("savingtransaction", "savingtransaction" + page.Identifier);
                            pageContent.Replace("{{SelectOption_" + page.Identifier + "_" + widget.Identifier + "}}", selectOption.ToString());
                            pageContent.Replace("SavingTransactionTable", "SavingTransactionTable" + page.Identifier);
                            pageContent.Replace("{{AccountTransactionDetails_" + page.Identifier + "_" + widget.Identifier + "}}", string.Empty);
                        }
                        else if (widget.WidgetId == HtmlConstants.CURRENT_TRANSACTION_WIDGET_ID)
                        {
                            StringBuilder selectOption = new StringBuilder();
                            var distinctNaration = new string[] { "NXT TXN: IIFL IIFL6574562", "NXT TXN: IIFL IIFL6574563", "NXT TXN: IIFL IIFL3557346", "NXT TXN: IIFL RTED87978947 REFUND", "NXT TXN: IIFL IIFL896452896ERE", "NXT TXN: IIFL IIFL8965435", "NXT TXN: IIFL FGTR454565JHGKD", "NXT TXN: OFFICE RENT 798789DFGH", "NXT TXN: IIFL IIFL0034212", "NXT TXN: IIFL IIFL045678DFGH" };
                            distinctNaration.ToList().ForEach(item =>
                            {
                                selectOption.Append("<option value='" + item + "'> " + item + "</option>");
                            });

                            scriptHtmlRenderer.Append("<script type='text/javascript' src='" + baseURL + "\\Resources\\sampledata\\currenttransactiondetail.json'></script>");
                            StringBuilder scriptval = new StringBuilder(HtmlConstants.CURRENT_TRANSACTION_DETAIL_GRID_WIDGET_SCRIPT);
                            scriptval.Replace("CurrentTransactionTable", "CurrentTransactionTable" + page.Identifier);
                            scriptval.Replace("currenttransactiondata", "currenttransactiondata" + page.Identifier);
                            scriptval.Replace("currentShowAll", "currentShowAll" + page.Identifier);
                            scriptval.Replace("filterStatus", "filterStatus" + page.Identifier);
                            scriptval.Replace("ResetGrid", "ResetGrid" + page.Identifier);
                            scriptval.Replace("PrintGrid", "PrintGrid" + page.Identifier);
                            scriptval.Replace("currenttransaction", "currenttransaction" + page.Identifier);
                            scriptHtmlRenderer.Append(scriptval);

                            pageContent.Replace("currentShowAll", "currentShowAll" + page.Identifier);
                            pageContent.Replace("filterStatus", "filterStatus" + page.Identifier);
                            pageContent.Replace("ResetGrid", "ResetGrid" + page.Identifier);
                            pageContent.Replace("PrintGrid", "PrintGrid" + page.Identifier);
                            pageContent.Replace("currenttransaction", "currenttransaction" + page.Identifier);
                            pageContent.Replace("{{SelectOption_" + page.Identifier + "_" + widget.Identifier + "}}", selectOption.ToString());
                            pageContent.Replace("CurrentTransactionTable", "CurrentTransactionTable" + page.Identifier);

                            pageContent.Replace("{{AccountTransactionDetails_" + page.Identifier + "_" + widget.Identifier + "}}", string.Empty);
                        }
                        else if (widget.WidgetId == HtmlConstants.TOP_4_INCOME_SOURCES_WIDGET_ID)
                        {
                            string incomeSourceListJson = "[{ 'Source': 'Salary Transfer', 'CurrentSpend': 3453, 'AverageSpend': 123},{ 'Source': 'Cash Deposit', 'CurrentSpend': 3453, 'AverageSpend': 6123},{ 'Source': 'Profit Earned', 'CurrentSpend': 3453, 'AverageSpend': 6123}, { 'Source': 'Rebete', 'CurrentSpend': 3453, 'AverageSpend': 123}]";
                            if (incomeSourceListJson != string.Empty && validationEngine.IsValidJson(incomeSourceListJson))
                            {
                                IList<IncomeSources> incomeSources = JsonConvert.DeserializeObject<List<IncomeSources>>(incomeSourceListJson);
                                StringBuilder incomestr = new StringBuilder();
                                incomeSources.ToList().ForEach(item =>
                                {
                                    incomestr.Append("<tr><td class='float-left'>" + item.Source + "</td>" + "<td> " + item.CurrentSpend +
                                      "" + "</td><td class='align-text-top'>" + "<span class='fa fa-sort-asc fa-2x text-danger align-text-top' " +
                                      "aria-hidden='true'>" + "</span>&nbsp;" + item.AverageSpend + " " + "</td></tr>");
                                });
                                pageContent.Replace("{{IncomeSourceList_" + page.Identifier + "_" + widget.Identifier + "}}", incomestr.ToString());
                            }
                        }
                        else if (widget.WidgetId == HtmlConstants.ANALYTICS_WIDGET_ID)
                        {
                            scriptHtmlRenderer.Append("<script type='text/javascript' src='" + baseURL + "\\Resources\\sampledata\\analyticschartdata.json'></script>");
                            pageContent.Replace("analyticschartcontainer", "analyticschartcontainer" + page.Identifier);
                            scriptHtmlRenderer.Append(HtmlConstants.ANALYTICS_CHART_WIDGET_SCRIPT.Replace("analyticschartcontainer", "analyticschartcontainer" + page.Identifier));
                        }
                        else if (widget.WidgetId == HtmlConstants.SPENDING_TREND_WIDGET_ID)
                        {
                            scriptHtmlRenderer.Append("<script type='text/javascript' src='" + baseURL + "\\Resources\\sampledata\\savingtrenddata.json'></script>");
                            pageContent.Replace("spendingTrendscontainer", "spendingTrendscontainer" + page.Identifier);
                            scriptHtmlRenderer.Append(HtmlConstants.SPENDING_TREND_CHART_WIDGET_SCRIPT.Replace("spendingTrendscontainer", "spendingTrendscontainer" + page.Identifier));
                        }
                        else if (widget.WidgetId == HtmlConstants.SAVING_TREND_WIDGET_ID)
                        {
                            scriptHtmlRenderer.Append("<script type='text/javascript' src='" + baseURL + "\\Resources\\sampledata\\spendingtrenddata.json'></script>");
                            pageContent.Replace("savingTrendscontainer", "savingTrendscontainer" + page.Identifier);
                            scriptHtmlRenderer.Append(HtmlConstants.SAVING_TREND_CHART_WIDGET_SCRIPT.Replace("savingTrendscontainer", "savingTrendscontainer" + page.Identifier));
                        }
                        else if (widget.WidgetId == HtmlConstants.REMINDER_AND_RECOMMENDATION_WIDGET_ID)
                        {
                            string reminderJson = "[{'Title': 'Update Missing Inofrmation', 'Action': 'Update' },{ 'Title': 'Your Rewards Video is available', 'Action': 'View' },{ 'Title': 'Payment Due for Home Loan', 'Action': 'Pay' }]";
                            if (reminderJson != string.Empty && validationEngine.IsValidJson(reminderJson))
                            {
                                IList<ReminderAndRecommendation> reminderAndRecommendations = JsonConvert.DeserializeObject<List<ReminderAndRecommendation>>(reminderJson);
                                StringBuilder reminderstr = new StringBuilder();
                                reminderstr.Append("<table class='width100'><thead><tr> <td class='width75 text-left'></td><td style='color:red;float: right;'><i class='fa fa-caret-left fa-2x float-left' aria-hidden='true'></i><span class='mt-2 d-inline-block ml-2'>Click</span></td></tr></thead><tbody>");
                                reminderAndRecommendations.ToList().ForEach(item =>
                                {
                                    reminderstr.Append("<tr><td class='width75 text-left' style='background-color: #dce3dc;'><label>" +
                                        item.Title + "</label></td><td><a>" + "<i class='fa fa-caret-left fa-2x' style='color:red' aria-hidden='true'>" +
                                        "</i>" + item.Action + "</a></td></tr>");
                                });
                                reminderstr.Append("</tbody></table>");
                                pageContent.Replace("{{ReminderAndRecommdationDataList_" + page.Identifier + "_" + widget.Identifier + "}}", reminderstr.ToString());
                            }
                        }
                    }

                    newPageContent.Append(pageContent);
                    if (page.PageTypeId == HtmlConstants.SAVING_ACCOUNT_PAGE_TYPE_ID || page.PageTypeId == HtmlConstants.CURRENT_ACCOUNT_PAGE_TYPE_ID)
                    {
                        newPageContent.Append("</div>");
                    }
                    newPageContent.Append("</div>"); //to end tab-content div

                    PageHeaderContent.Replace("{{SubTabs}}", SubTabs.ToString());
                    statementPageContent.PageHeaderContent = PageHeaderContent.ToString();
                    statementPageContent.HtmlContent = newPageContent.ToString();
                    //newStatementPageContents.Add(statementPageContent);
                }

                newStatementPageContents.ToList().ForEach(page => {
                    htmlbody.Append(page.PageHeaderContent);
                    htmlbody.Append(page.HtmlContent);
                    htmlbody.Append(page.PageFooterContent);
                });
                htmlbody.Append(HtmlConstants.CONTAINER_DIV_HTML_FOOTER);

                navbarHtml = navbarHtml.Replace("{{NavItemList}}", navbar.ToString());

                StringBuilder finalHtml = new StringBuilder();
                finalHtml.Append(HtmlConstants.HTML_HEADER);
                finalHtml.Append(navbarHtml);
                finalHtml.Append(htmlbody.ToString());
                finalHtml.Append(HtmlConstants.HTML_FOOTER);
                finalHtml.Replace("{{ChartScripts}}", scriptHtmlRenderer.ToString());
                return finalHtml.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }
}
