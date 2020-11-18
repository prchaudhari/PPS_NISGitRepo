// <copyright file="SQLScheduleRepository.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2018 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------

namespace nIS
{
    using Microsoft.Practices.ObjectBuilder2;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    #region References
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Linq.Dynamic;
    using System.Security.Claims;
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

        /// <summary>
        /// The Tenant configuration repository.
        /// </summary>
        private ITenantConfigurationRepository tenantConfigurationRepository = null;

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
            this.tenantConfigurationRepository = this.unityContainer.Resolve<ITenantConfigurationRepository>();
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
                var claims = ClaimsPrincipal.Current.Identities.First().Claims.ToList();
                int userId = 1;
                int.TryParse(claims?.FirstOrDefault(x => x.Type.Equals("UserId", StringComparison.OrdinalIgnoreCase)).Value, out userId);

                this.SetAndValidateConnectionString(tenantCode);

                if (this.IsDuplicateSchedule(schedules, "AddOperation", tenantCode))
                {
                    throw new DuplicateScheduleFoundException(tenantCode);
                }
                IList<ScheduleRecord> scheduleRecords = new List<ScheduleRecord>();
                schedules.ToList().ForEach(schedule =>
                {
                    DateTime startDateTime = DateTime.SpecifyKind(Convert.ToDateTime(schedule.StartDate), DateTimeKind.Utc);
                    DateTime? endDateTime = DateTime.SpecifyKind(Convert.ToDateTime(schedule.EndDate), DateTimeKind.Utc);

                    scheduleRecords.Add(new ScheduleRecord()
                    {
                        Name = schedule.Name,
                        Description = schedule.Description,
                        DayOfMonth = schedule.DayOfMonth,
                        HourOfDay = schedule.HourOfDay,
                        MinuteOfDay = schedule.MinuteOfDay,
                        StartDate = startDateTime,
                        EndDate = schedule.EndDate != null ? endDateTime : null,
                        Status = schedule.Status,
                        IsDeleted = false,
                        IsActive = true,
                        TenantCode = tenantCode,
                        StatementId = schedule.Statement.Identifier,
                        IsExportToPDF = schedule.IsExportToPDF,
                        UpdateBy = userId,
                        LastUpdatedDate = DateTime.UtcNow,
                        RecurrancePattern = schedule.RecurrancePattern,
                        RepeatEveryDayMonWeekYear = schedule.RepeatEveryDayMonWeekYear,
                        WeekDays = schedule.WeekDays,
                        IsEveryWeekDay = schedule.IsEveryWeekDay,
                        MonthOfYear = schedule.MonthOfYear,
                        IsEndsAfterNoOfOccurrences = schedule.IsEndsAfterNoOfOccurrences,
                        NoOfOccurrences = schedule.NoOfOccurrences
                    });
                });
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    nISEntitiesDataContext.ScheduleRecords.AddRange(scheduleRecords);
                    nISEntitiesDataContext.SaveChanges();
                    result = true;
                }
                if (result)
                {
                    scheduleRecords.ToList().ForEach(schedulerecord =>
                    {
                        int batchIndex = 1;
                        if (schedulerecord.RecurrancePattern == ModelConstant.DOES_NOT_REPEAT)
                        {
                            this.AddDoesNotRepeatBatch(schedulerecord.StartDate ?? DateTime.Now, schedulerecord, tenantCode, userId);
                        }
                        else if (schedulerecord.RecurrancePattern == ModelConstant.DAILY || schedulerecord.RecurrancePattern == ModelConstant.CUSTOM_DAY)
                        {
                            this.AddDailyOccurenceScheduleBatches(schedulerecord.StartDate ?? DateTime.Now, schedulerecord, tenantCode, userId, batchIndex);
                        }
                        else if (schedulerecord.RecurrancePattern == ModelConstant.WEEKDAY || schedulerecord.RecurrancePattern == ModelConstant.WEEKLY || schedulerecord.RecurrancePattern == ModelConstant.CUSTOM_WEEK)
                        {
                            this.AddWeeklyOccurenceScheduleBatches(schedulerecord.StartDate ?? DateTime.Now, schedulerecord, tenantCode, userId, batchIndex);
                        }
                        else if (schedulerecord.RecurrancePattern == ModelConstant.MONTHLY || schedulerecord.RecurrancePattern == ModelConstant.CUSTOM_MONTH)
                        {
                            this.AddMonthlyOccurenceScheduleBatches(schedulerecord.StartDate ?? DateTime.Now, schedulerecord, tenantCode, userId, batchIndex);
                        }
                        else if (schedulerecord.RecurrancePattern == ModelConstant.YEARLY || schedulerecord.RecurrancePattern == ModelConstant.CUSTOM_YEAR)
                        {
                            this.AddYearlyOccurenceScheduleBatches(schedulerecord.StartDate ?? DateTime.Now, schedulerecord, tenantCode, userId, batchIndex);
                        }
                    });
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
                var claims = ClaimsPrincipal.Current.Identities.First().Claims.ToList();
                int userId = 1;
                int.TryParse(claims?.FirstOrDefault(x => x.Type.Equals("UserId", StringComparison.OrdinalIgnoreCase)).Value, out userId);

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
                        DateTime startDateTime = DateTime.SpecifyKind(Convert.ToDateTime(item.StartDate), DateTimeKind.Utc);
                        DateTime? endDateTime = DateTime.SpecifyKind(Convert.ToDateTime(item.EndDate), DateTimeKind.Utc);

                        ScheduleRecord scheduleRecord = scheduleRecords.FirstOrDefault(data => data.Id == item.Identifier && data.TenantCode == tenantCode && data.IsDeleted == false);
                        scheduleRecord.Name = item.Name;
                        scheduleRecord.Description = item.Description;
                        scheduleRecord.DayOfMonth = item.DayOfMonth;
                        scheduleRecord.HourOfDay = item.HourOfDay;
                        scheduleRecord.MinuteOfDay = item.MinuteOfDay;
                        scheduleRecord.StartDate = startDateTime;
                        scheduleRecord.EndDate = item.EndDate != null ? endDateTime : null;
                        scheduleRecord.Status = item.Status;
                        scheduleRecord.IsDeleted = false;
                        scheduleRecord.IsActive = item.IsActive;
                        scheduleRecord.TenantCode = tenantCode;
                        scheduleRecord.IsExportToPDF = item.IsExportToPDF;
                        scheduleRecord.StatementId = item.Statement.Identifier;
                        scheduleRecord.UpdateBy = userId;
                        scheduleRecord.LastUpdatedDate = DateTime.UtcNow;
                        scheduleRecord.RecurrancePattern = item.RecurrancePattern;
                        scheduleRecord.RepeatEveryDayMonWeekYear = item.RepeatEveryDayMonWeekYear;
                        scheduleRecord.WeekDays = item.WeekDays;
                        scheduleRecord.IsEveryWeekDay = item.IsEveryWeekDay;
                        scheduleRecord.MonthOfYear = item.MonthOfYear;
                        scheduleRecord.IsEndsAfterNoOfOccurrences = item.IsEndsAfterNoOfOccurrences;
                        scheduleRecord.NoOfOccurrences = item.NoOfOccurrences;
                        nISEntitiesDataContext.SaveChanges();

                        //If any batch is not executed or data ready for it, then delete all batches and re-insert it as per start date and end date, 
                        //Else insert new batches as per new end date and previous end date logic
                        var batches = nISEntitiesDataContext.BatchMasterRecords.Where(batch => batch.ScheduleId == scheduleRecord.Id && (batch.IsExecuted || batch.IsDataReady)).ToList();
                        if (batches.Count == 0)
                        {
                            var batchesToDelete = nISEntitiesDataContext.BatchMasterRecords.Where(batch => batch.ScheduleId == scheduleRecord.Id).ToList();
                            nISEntitiesDataContext.BatchMasterRecords.RemoveRange(batchesToDelete);
                            nISEntitiesDataContext.SaveChanges();
                            int batchIndex = 1;
                            if (scheduleRecord.RecurrancePattern == ModelConstant.DOES_NOT_REPEAT)
                            {
                                this.AddDoesNotRepeatBatch(scheduleRecord.StartDate ?? DateTime.Now, scheduleRecord, tenantCode, userId);
                            }
                            else if (scheduleRecord.RecurrancePattern == ModelConstant.DAILY || scheduleRecord.RecurrancePattern == ModelConstant.CUSTOM_DAY)
                            {
                                this.AddDailyOccurenceScheduleBatches(scheduleRecord.StartDate ?? DateTime.Now, scheduleRecord, tenantCode, userId, batchIndex);
                            }
                            else if (scheduleRecord.RecurrancePattern == ModelConstant.WEEKDAY || scheduleRecord.RecurrancePattern == ModelConstant.WEEKLY || scheduleRecord.RecurrancePattern == ModelConstant.CUSTOM_WEEK)
                            {
                                this.AddWeeklyOccurenceScheduleBatches(scheduleRecord.StartDate ?? DateTime.Now, scheduleRecord, tenantCode, userId, batchIndex);
                            }
                            else if (scheduleRecord.RecurrancePattern == ModelConstant.MONTHLY || scheduleRecord.RecurrancePattern == ModelConstant.CUSTOM_MONTH)
                            {
                                this.AddMonthlyOccurenceScheduleBatches(scheduleRecord.StartDate ?? DateTime.Now, scheduleRecord, tenantCode, userId, batchIndex);
                            }
                            else if (scheduleRecord.RecurrancePattern == ModelConstant.YEARLY || scheduleRecord.RecurrancePattern == ModelConstant.CUSTOM_YEAR)
                            {
                                this.AddYearlyOccurenceScheduleBatches(scheduleRecord.StartDate ?? DateTime.Now, scheduleRecord, tenantCode, userId, batchIndex);
                            }
                        }
                        else
                        {
                            var Batches = nISEntitiesDataContext.BatchMasterRecords.Where(b => b.ScheduleId == scheduleRecord.Id).OrderByDescending(x => x.BatchExecutionDate).ToList();
                            int batchIndex = Batches.Count + 1;
                            var lastExecutedBatch = Batches.FirstOrDefault();
                            var newStartDate = lastExecutedBatch.BatchExecutionDate;
                            if (scheduleRecord.RecurrancePattern == ModelConstant.DAILY || scheduleRecord.RecurrancePattern == ModelConstant.CUSTOM_DAY)
                                {
                                newStartDate.AddDays(1);
                                this.AddDailyOccurenceScheduleBatches(newStartDate, scheduleRecord, tenantCode, userId, batchIndex);
                            }
                            else if (scheduleRecord.RecurrancePattern == ModelConstant.WEEKDAY || scheduleRecord.RecurrancePattern == ModelConstant.WEEKLY || scheduleRecord.RecurrancePattern == ModelConstant.CUSTOM_WEEK)
                            {
                                newStartDate.AddDays(1);
                                this.AddWeeklyOccurenceScheduleBatches(newStartDate, scheduleRecord, tenantCode, userId, batchIndex);
                            }
                            else if (scheduleRecord.RecurrancePattern == ModelConstant.MONTHLY || scheduleRecord.RecurrancePattern == ModelConstant.CUSTOM_MONTH)
                            {
                                newStartDate.AddMonths(1);
                                this.AddMonthlyOccurenceScheduleBatches(newStartDate, scheduleRecord, tenantCode, userId, batchIndex);
                            }
                            else if (scheduleRecord.RecurrancePattern == ModelConstant.YEARLY || scheduleRecord.RecurrancePattern == ModelConstant.CUSTOM_YEAR)
                            {
                                newStartDate.AddYears(1);
                                this.AddYearlyOccurenceScheduleBatches(newStartDate, scheduleRecord, tenantCode, userId, batchIndex);
                            }
                        }

                    });

                    result = true;
                }
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
                    IList<ScheduleRecord> scheduleRecords = new List<ScheduleRecord>();
                    IList<View_ScheduleRecord> view_ScheduleRecords = new List<View_ScheduleRecord>();
                    if (scheduleSearchParameter.PagingParameter.PageIndex > 0 && scheduleSearchParameter.PagingParameter.PageSize > 0)
                    {
                        view_ScheduleRecords = nISEntitiesDataContext.View_ScheduleRecord
                        .OrderBy(scheduleSearchParameter.SortParameter.SortColumn + " " + scheduleSearchParameter.SortParameter.SortOrder.ToString())
                        .Where(whereClause)
                        .Skip((scheduleSearchParameter.PagingParameter.PageIndex - 1) * scheduleSearchParameter.PagingParameter.PageSize)
                        .Take(scheduleSearchParameter.PagingParameter.PageSize)
                        .ToList();
                    }
                    else
                    {
                        view_ScheduleRecords = nISEntitiesDataContext.View_ScheduleRecord
                        .Where(whereClause)
                        .OrderBy(scheduleSearchParameter.SortParameter.SortColumn + " " + scheduleSearchParameter.SortParameter.SortOrder.ToString().ToLower())
                        .ToList();
                    }

                    if (view_ScheduleRecords != null && view_ScheduleRecords.Count > 0)
                    {
                        schedules = view_ScheduleRecords.Select(scheduleRecord => new Schedule()
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
                            Statement = new Statement { Identifier = scheduleRecord.StatementId, Name = scheduleRecord.StatementName },
                            RecurrancePattern = scheduleRecord.RecurrancePattern,
                            RepeatEveryDayMonWeekYear = scheduleRecord.RepeatEveryDayMonWeekYear,
                            WeekDays = scheduleRecord.WeekDays,
                            IsEveryWeekDay = scheduleRecord.IsEveryWeekDay,
                            MonthOfYear = scheduleRecord.MonthOfYear,
                            IsEndsAfterNoOfOccurrences = scheduleRecord.IsEndsAfterNoOfOccurrences,
                            NoOfOccurrences = scheduleRecord.NoOfOccurrences,
                            ExecutedBatchCount = scheduleRecord.ExecutedBatchCount ?? 0
                        }).ToList();
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
                    scheduleCount = nISEntitiesDataContext.View_ScheduleRecord.Where(whereClause.ToString()).Count();
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
        /// <param name="baseURL">The base URL</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <param name="parallelThreadCount">The parallel thread count</param>
        /// <returns>True if schedules runs successfully, false otherwise</returns>
        public bool RunSchedule(string baseURL, string outputLocation, string tenantCode, int parallelThreadCount, Client client)
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

                    var tenantConfiguration = this.tenantConfigurationRepository.GetTenantConfigurations(tenantCode)?.FirstOrDefault();
                    BatchMasterRecord batchMaster = new BatchMasterRecord();
                    IList<BatchDetailRecord> batchDetails = new List<BatchDetailRecord>();
                    schedules.ToList().ForEach(schedule =>
                    {
                        ScheduleLogRecord scheduleLog = new ScheduleLogRecord();
                        scheduleLog.ScheduleId = schedule.Id;
                        scheduleLog.ScheduleName = schedule.Name;
                        scheduleLog.NumberOfRetry = 1;
                        scheduleLog.CreationDate = DateTime.Now;
                        scheduleLog.TenantCode = tenantCode;

                        using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                        {
                            batchMaster = nISEntitiesDataContext.BatchMasterRecords.Where(item => item.ScheduleId == schedule.Id && item.BatchExecutionDate.Day == schedule.DayOfMonth && item.BatchExecutionDate.Hour == schedule.HourOfDay && item.BatchExecutionDate.Minute == schedule.MinuteOfDay && !item.IsExecuted && item.Status == BatchStatus.New.ToString() && item.TenantCode == tenantCode)?.ToList()?.FirstOrDefault();
                        }

                        if (batchMaster != null)
                        {
                            if (batchMaster.IsDataReady)
                            {
                                batchMaster.Status = BatchStatus.Running.ToString();
                                scheduleLog.Status = ScheduleLogStatus.InProgress.ToString();
                                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                                {
                                    nISEntitiesDataContext.ScheduleLogRecords.Add(scheduleLog);
                                    nISEntitiesDataContext.SaveChanges();
                                }

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
                                    Statement statement = statements[0];
                                    IList<StatementPageContent> statementPageContents = this.statementRepository.GenerateHtmlFormatOfStatement(statement, tenantCode, tenantConfiguration);
                                    if (statementPageContents.Count > 0)
                                    {
                                        IList<CustomerMasterRecord> customerMasters = new List<CustomerMasterRecord>();

                                        var statementPreviewData = this.statementRepository.BindDataToCommonStatement(statement, statementPageContents, tenantConfiguration, tenantCode, client);
                                        string fileName = "Statement_" + statement.Identifier + "_" + batchMaster.Id + "_" + DateTime.Now.ToString().Replace("-", "_").Replace(":", "_").Replace(" ", "_").Replace('/', '_') + ".html";

                                        var filesDict = new Dictionary<string, string>();
                                        if (statementPreviewData.SampleFiles != null && statementPreviewData.SampleFiles.Count > 0)
                                        {
                                            statementPreviewData.SampleFiles.ToList().ForEach(file =>
                                            {
                                                if (!filesDict.ContainsKey(file.FileName))
                                                {
                                                    filesDict.Add(file.FileName, file.FileUrl);
                                                }
                                            });
                                        }
                                        string CommonStatementZipFilePath = this.utility.CreateAndWriteToZipFile(statementPreviewData.FileContent, fileName, batchMaster.Id, baseURL, outputLocation, filesDict);

                                        using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                                        {
                                            ScheduleRunHistoryRecord runHistory = new ScheduleRunHistoryRecord();
                                            runHistory.StartDate = DateTime.Now;
                                            runHistory.TenantCode = tenantCode;
                                            runHistory.ScheduleId = schedule.Id;
                                            runHistory.StatementId = statement.Identifier;
                                            runHistory.ScheduleLogId = scheduleLog.Id;
                                            runHistory.EndDate = DateTime.Now;
                                            runHistory.FilePath = CommonStatementZipFilePath;
                                            nISEntitiesDataContext.ScheduleRunHistoryRecords.Add(runHistory);
                                            nISEntitiesDataContext.SaveChanges();

                                            batchDetails = nISEntitiesDataContext.BatchDetailRecords.Where(item => item.BatchId == batchMaster.Id && item.StatementId == statement.Identifier && item.TenantCode == tenantCode)?.ToList();
                                            customerMasters = nISEntitiesDataContext.CustomerMasterRecords.Where(item => item.BatchId == batchMaster.Id && item.TenantCode == tenantCode).ToList();
                                        }

                                        if (customerMasters.Count > 0)
                                        {
                                            ParallelOptions parallelOptions = new ParallelOptions();
                                            parallelOptions.MaxDegreeOfParallelism = parallelThreadCount;
                                            Parallel.ForEach(customerMasters, parallelOptions, customer =>
                                            {
                                                this.CreateCustomerStatement(customer, statement, scheduleLog, statementPageContents, batchMaster, batchDetails, baseURL, tenantCode, customerMasters.Count, outputLocation, tenantConfiguration, client);
                                            });
                                            //customerMasters.ToList().ForEach(customer =>
                                            //{
                                            //    this.CreateCustomerStatement(customer, statement, scheduleLog, statementPageContents, batchMaster, batchDetails, baseURL, tenantCode, customerMasters.Count, outputLocation);
                                            //});
                                        }
                                        else
                                        {
                                            using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                                            {
                                                nISEntitiesDataContext.BatchMasterRecords.Where(item => item.Id == batchMaster.Id && item.TenantCode == tenantCode).ToList().ForEach(item => item.Status = BatchStatus.BatchDataNotAvailable.ToString());
                                                nISEntitiesDataContext.ScheduleLogRecords.Where(item => item.Id == scheduleLog.Id && item.TenantCode == tenantCode).ToList().ForEach(item => item.Status = ScheduleLogStatus.BatchDataNotAvailable.ToString());
                                                nISEntitiesDataContext.ScheduleRunHistoryRecords.Where(item => item.ScheduleLogId == scheduleLog.Id && item.TenantCode == tenantCode).ToList().ForEach(item => item.EndDate = DateTime.Now);
                                                nISEntitiesDataContext.ScheduleRecords.Where(item => item.Id == scheduleLog.ScheduleId && item.TenantCode == tenantCode).ToList().ForEach(item => item.Status = ScheduleStatus.BatchDataNotAvailable.ToString());
                                                nISEntitiesDataContext.SaveChanges();
                                            }
                                        }
                                    }
                                }

                            }
                            else
                            {
                                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                                {
                                    batchMaster.Status = BatchStatus.BatchDataNotAvailable.ToString();
                                    scheduleLog.Status = ScheduleLogStatus.BatchDataNotAvailable.ToString();
                                    nISEntitiesDataContext.ScheduleLogRecords.Add(scheduleLog);
                                    var scheduleRecord = nISEntitiesDataContext.ScheduleRecords.Where(item => item.Id == schedule.Id && item.TenantCode == tenantCode).FirstOrDefault();
                                    scheduleRecord.Status = ScheduleStatus.BatchDataNotAvailable.ToString();
                                    nISEntitiesDataContext.SaveChanges();
                                }
                            }
                        }
                        else
                        {
                            using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                            {
                                batchMaster = nISEntitiesDataContext.BatchMasterRecords.Where(item => item.ScheduleId == schedule.Id && item.BatchExecutionDate.Day == schedule.DayOfMonth && item.BatchExecutionDate.Hour == schedule.HourOfDay && item.BatchExecutionDate.Minute == schedule.MinuteOfDay && item.TenantCode == tenantCode)?.ToList()?.FirstOrDefault();
                                if (!batchMaster.IsExecuted)
                                {
                                    scheduleLog.Status = ScheduleLogStatus.BatchDataNotAvailable.ToString();
                                    nISEntitiesDataContext.ScheduleLogRecords.Add(scheduleLog);
                                    var scheduleRecord = nISEntitiesDataContext.ScheduleRecords.Where(item => item.Id == schedule.Id && item.TenantCode == tenantCode).FirstOrDefault();
                                    scheduleRecord.Status = ScheduleStatus.BatchDataNotAvailable.ToString();
                                    nISEntitiesDataContext.SaveChanges();
                                }
                            }
                        }
                    });
                }
                scheduleRunStatus = true;
            }
            catch (Exception ex)
            {
                WriteToFile(ex.Message);
                WriteToFile(ex.InnerException.Message);
                WriteToFile(ex.StackTrace.ToString());
                throw ex;
            }
            return scheduleRunStatus;
        }

        /// <summary>
        /// This method helps to run the schedule
        /// </summary>
        /// <param name="baseURL">The base URL</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <param name="parallelThreadCount">The parallel thread count</param>
        /// <returns>True if schedules runs successfully, false otherwise</returns>
        public bool RunScheduleNew(string baseURL, string outputLocation, string tenantCode, int parallelThreadCount, Client client)
        {
            bool scheduleRunStatus = false;
            var schedules = new List<ScheduleRecord>();
            var batchMasterRecords = new List<BatchMasterRecord>();
            
            var currentDate = DateTime.Now;
            var dueDate = currentDate.AddMinutes(30);
            this.WriteToFile("Current Date: "+currentDate + " Due Date: " + dueDate);

            try
            {
                var query = new StringBuilder();
                this.SetAndValidateConnectionString(tenantCode);
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    query.Append("BatchExecutionDate >= DateTime(" + currentDate.Year + "," + currentDate.Month + "," + currentDate.Day + "," + currentDate.Hour + "," + currentDate.Minute + "," + currentDate.Second + ") and BatchExecutionDate <= DateTime(" + +dueDate.Year + "," + dueDate.Month + "," + dueDate.Day + "," + dueDate.Hour + "," + dueDate.Minute + "," + dueDate.Second + ") and IsExecuted.Equals(false) ");
                    query.Append(string.Format(" and Status.Equals(\"{0}\") ", BatchStatus.New.ToString()));
                    query.Append(string.Format(" and TenantCode.Equals(\"{0}\") ", tenantCode));
                    this.WriteToFile("Batch Records: " + query);
                    batchMasterRecords = nISEntitiesDataContext.BatchMasterRecords.Where(query.ToString()).ToList();
                }
                if (batchMasterRecords.Count > 0)
                {
                    using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                    {
                        query = new StringBuilder();
                        query.Append("(" + string.Join("or ", string.Join(",", batchMasterRecords.Select(item => item.ScheduleId).Distinct()).ToString().Split(',').Select(item => string.Format("Id.Equals({0}) ", item))) + ") ");
                        query.Append(string.Format(" and TenantCode.Equals(\"{0}\") ", tenantCode));
                        this.WriteToFile("Schedule Records: " + query);
                        schedules = nISEntitiesDataContext.ScheduleRecords.Where(query.ToString()).Select(item => item).AsQueryable().ToList();
                    }
                    
                    if (schedules != null && schedules.Count > 0)
                    {
                        var tenantConfiguration = this.tenantConfigurationRepository.GetTenantConfigurations(tenantCode)?.FirstOrDefault();
                        var batchMaster = new BatchMasterRecord();
                        var batchDetails = new List<BatchDetailRecord>();
                        schedules.ToList().ForEach(schedule =>
                        {
                            ScheduleLogRecord scheduleLog = new ScheduleLogRecord();
                            scheduleLog.ScheduleId = schedule.Id;
                            scheduleLog.ScheduleName = schedule.Name;
                            scheduleLog.NumberOfRetry = 1;
                            scheduleLog.CreationDate = DateTime.UtcNow;
                            scheduleLog.TenantCode = tenantCode;

                            using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                            {
                                schedule.Status = ScheduleStatus.InProgress.ToString();
                                nISEntitiesDataContext.SaveChanges();

                                batchMaster = nISEntitiesDataContext.BatchMasterRecords.Where(item => item.ScheduleId == schedule.Id && !item.IsExecuted && item.Status == BatchStatus.New.ToString())?.ToList()?.FirstOrDefault();
                            }

                            if (batchMaster != null)
                            {
                                if (batchMaster.IsDataReady)
                                {
                                    batchMaster.Status = BatchStatus.Running.ToString();
                                    scheduleLog.Status = ScheduleLogStatus.InProgress.ToString();
                                    using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                                    {
                                        nISEntitiesDataContext.ScheduleLogRecords.Add(scheduleLog);
                                        nISEntitiesDataContext.SaveChanges();
                                    }

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
                                        Statement statement = statements.FirstOrDefault();
                                        IList<StatementPageContent> statementPageContents = this.statementRepository.GenerateHtmlFormatOfStatement(statement, tenantCode, tenantConfiguration);
                                        if (statementPageContents.Count > 0)
                                        {
                                            var customerMasters = new List<CustomerMasterRecord>();
                                            var statementPreviewData = this.statementRepository.BindDataToCommonStatement(statement, statementPageContents, tenantConfiguration, tenantCode, client);
                                            string fileName = "Statement_" + statement.Identifier + "_" + batchMaster.Id + "_" + DateTime.UtcNow.ToString().Replace("-", "_").Replace(":", "_").Replace(" ", "_").Replace('/', '_') + ".html";

                                            var filesDict = new Dictionary<string, string>();
                                            if (statementPreviewData.SampleFiles != null && statementPreviewData.SampleFiles.Count > 0)
                                            {
                                                statementPreviewData.SampleFiles.ToList().ForEach(file =>
                                                {
                                                    if (!filesDict.ContainsKey(file.FileName))
                                                    {
                                                        filesDict.Add(file.FileName, file.FileUrl);
                                                    }
                                                });
                                            }
                                            string CommonStatementZipFilePath = this.utility.CreateAndWriteToZipFile(statementPreviewData.FileContent, fileName, batchMaster.Id, baseURL, outputLocation, filesDict);

                                            using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                                            {
                                                ScheduleRunHistoryRecord runHistory = new ScheduleRunHistoryRecord();
                                                runHistory.StartDate = DateTime.UtcNow;
                                                runHistory.TenantCode = tenantCode;
                                                runHistory.ScheduleId = schedule.Id;
                                                runHistory.StatementId = statement.Identifier;
                                                runHistory.ScheduleLogId = scheduleLog.Id;
                                                runHistory.EndDate = DateTime.UtcNow;
                                                runHistory.FilePath = CommonStatementZipFilePath;
                                                nISEntitiesDataContext.ScheduleRunHistoryRecords.Add(runHistory);
                                                nISEntitiesDataContext.SaveChanges();

                                                batchDetails = nISEntitiesDataContext.BatchDetailRecords.Where(item => item.BatchId == batchMaster.Id && item.StatementId == statement.Identifier && item.TenantCode == tenantCode)?.ToList();
                                                customerMasters = nISEntitiesDataContext.CustomerMasterRecords.Where(item => item.BatchId == batchMaster.Id && item.TenantCode == tenantCode).ToList();
                                            }

                                            if (customerMasters.Count > 0)
                                            {
                                                ParallelOptions parallelOptions = new ParallelOptions();
                                                parallelOptions.MaxDegreeOfParallelism = parallelThreadCount;
                                                Parallel.ForEach(customerMasters, parallelOptions, customer =>
                                                {
                                                    this.CreateCustomerStatement(customer, statement, scheduleLog, statementPageContents, batchMaster, batchDetails, baseURL, tenantCode, customerMasters.Count, outputLocation, tenantConfiguration, client);
                                                });
                                                //customerMasters.ToList().ForEach(customer =>
                                                //{
                                                //    this.CreateCustomerStatement(customer, statement, scheduleLog, statementPageContents, batchMaster, batchDetails, baseURL, tenantCode, customerMasters.Count, outputLocation, tenantConfiguration, client);
                                                //});
                                            }
                                            else
                                            {
                                                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                                                {
                                                    nISEntitiesDataContext.BatchMasterRecords.Where(item => item.Id == batchMaster.Id && item.TenantCode == tenantCode).ToList().ForEach(item => item.Status = BatchStatus.BatchDataNotAvailable.ToString());
                                                    nISEntitiesDataContext.ScheduleLogRecords.Where(item => item.Id == scheduleLog.Id && item.TenantCode == tenantCode).ToList().ForEach(item => item.Status = ScheduleLogStatus.BatchDataNotAvailable.ToString());
                                                    nISEntitiesDataContext.ScheduleRunHistoryRecords.Where(item => item.ScheduleLogId == scheduleLog.Id && item.TenantCode == tenantCode).ToList().ForEach(item => item.EndDate = DateTime.UtcNow);
                                                    nISEntitiesDataContext.ScheduleRecords.Where(item => item.Id == scheduleLog.ScheduleId && item.TenantCode == tenantCode).ToList().ForEach(item => item.Status = ScheduleStatus.BatchDataNotAvailable.ToString());
                                                    nISEntitiesDataContext.SaveChanges();
                                                }
                                            }
                                        }
                                    }

                                }
                                else
                                {
                                    using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                                    {
                                        batchMaster.Status = BatchStatus.BatchDataNotAvailable.ToString();
                                        scheduleLog.Status = ScheduleLogStatus.BatchDataNotAvailable.ToString();
                                        nISEntitiesDataContext.ScheduleLogRecords.Add(scheduleLog);
                                        var scheduleRecord = nISEntitiesDataContext.ScheduleRecords.Where(item => item.Id == schedule.Id && item.TenantCode == tenantCode).FirstOrDefault();
                                        scheduleRecord.Status = ScheduleStatus.BatchDataNotAvailable.ToString();
                                        nISEntitiesDataContext.SaveChanges();
                                    }
                                }
                            }
                        });
                    }
                    
                }
                scheduleRunStatus = true;
            }
            catch (Exception ex)
            {
                WriteToFile(ex.Message);
                WriteToFile(ex.InnerException.Message);
                WriteToFile(ex.StackTrace.ToString());
                throw ex;
            }
            return scheduleRunStatus;
        }

        /// <summary>
        /// This method helps to run the schedule now
        /// </summary>
        /// <param name="batch">The batch object</param>
        /// <param name="baseURL">The base URL</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <param name="parallelThreadCount">The parallel thread count</param>
        /// <returns>True if schedules runs successfully, false otherwise</returns>
        public bool RunScheduleNow(BatchMaster batch, string baseURL, string outputLocation, string tenantCode, int parallelThreadCount, Client client)
        {
            bool isScheduleSuccess = false;
            try
            {
                ScheduleRecord scheduleRecord = new ScheduleRecord();
                BatchMasterRecord batchMaster = new BatchMasterRecord();
                ScheduleLogRecord scheduleLog = new ScheduleLogRecord();
                this.SetAndValidateConnectionString(tenantCode);
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    scheduleRecord = nISEntitiesDataContext.ScheduleRecords.Where(item => item.Id == batch.ScheduleId && item.TenantCode == tenantCode).ToList()?.FirstOrDefault();
                    if (scheduleRecord == null)
                    {
                        throw new ScheduleNotFoundException(tenantCode);
                    }

                    scheduleLog.ScheduleId = scheduleRecord.Id;
                    scheduleLog.ScheduleName = scheduleRecord.Name;
                    scheduleLog.NumberOfRetry = 1;
                    scheduleLog.CreationDate = DateTime.UtcNow;
                    scheduleLog.TenantCode = tenantCode;

                    batchMaster = nISEntitiesDataContext.BatchMasterRecords.Where(item => item.Id == batch.Identifier && item.ScheduleId == scheduleRecord.Id && !item.IsExecuted && item.Status == BatchStatus.New.ToString() && item.TenantCode == tenantCode)?.ToList().FirstOrDefault();
                    var IsDataAvail = false;
                    if (batchMaster != null)
                    {
                        if (batchMaster.IsDataReady)
                        {
                            batchMaster.Status = BatchStatus.Running.ToString();
                            scheduleRecord.Status = ScheduleStatus.InProgress.ToString();
                            scheduleLog.Status = ScheduleLogStatus.InProgress.ToString();
                            IsDataAvail = batchMaster.IsDataReady;
                        }
                        else
                        {
                            batchMaster.Status = BatchStatus.BatchDataNotAvailable.ToString();
                            scheduleRecord.Status = ScheduleStatus.BatchDataNotAvailable.ToString();
                            scheduleLog.Status = ScheduleLogStatus.BatchDataNotAvailable.ToString();
                        }
                    }
                    else
                    {
                        scheduleRecord.Status = ScheduleStatus.BatchDataNotAvailable.ToString();
                        scheduleLog.Status = ScheduleLogStatus.BatchDataNotAvailable.ToString();
                    }

                    nISEntitiesDataContext.ScheduleLogRecords.Add(scheduleLog);
                    nISEntitiesDataContext.SaveChanges();
                    if (!IsDataAvail)
                    {
                        return isScheduleSuccess;
                    }
                }

                var tenantConfiguration = this.tenantConfigurationRepository.GetTenantConfigurations(tenantCode)?.FirstOrDefault();
                StatementSearchParameter statementSearchParameter = new StatementSearchParameter
                {
                    Identifier = scheduleRecord.StatementId,
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
                if (statements.Count == 0)
                {
                    throw new StatementNotFoundException(tenantCode);
                }

                var statement = statements[0];
                var pages = statement.Pages;
                var statementPageContents = this.statementRepository.GenerateHtmlFormatOfStatement(statement, tenantCode, tenantConfiguration);
                if (statementPageContents.Count > 0)
                {
                    var statementPreviewData = this.statementRepository.BindDataToCommonStatement(statement, statementPageContents, tenantConfiguration, tenantCode, client);
                    string fileName = "Statement_" + statement.Identifier + "_" + batchMaster.Id + "_" + DateTime.UtcNow.ToString().Replace("-", "_").Replace(":", "_").Replace(" ", "_").Replace('/', '_') + ".html";

                    var filesDict = new Dictionary<string, string>();
                    for (int i = 0; i < statementPreviewData.SampleFiles.Count; i++)
                    {
                        filesDict.Add(statementPreviewData.SampleFiles[i].FileName, statementPreviewData.SampleFiles[i].FileUrl);
                    }
                    string CommonStatementZipFilePath = this.utility.CreateAndWriteToZipFile(statementPreviewData.FileContent, fileName, batchMaster.Id, baseURL, outputLocation, filesDict);

                    IList<CustomerMasterRecord> customerMasters = new List<CustomerMasterRecord>();
                    IList<BatchDetailRecord> batchDetails = new List<BatchDetailRecord>();
                    using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                    {
                        ScheduleRunHistoryRecord runHistory = new ScheduleRunHistoryRecord();
                        runHistory.StartDate = DateTime.UtcNow;
                        runHistory.TenantCode = tenantCode;
                        runHistory.ScheduleId = scheduleRecord.Id;
                        runHistory.StatementId = statement.Identifier;
                        runHistory.ScheduleLogId = scheduleLog.Id;
                        runHistory.EndDate = DateTime.UtcNow;
                        runHistory.FilePath = CommonStatementZipFilePath;
                        nISEntitiesDataContext.ScheduleRunHistoryRecords.Add(runHistory);
                        nISEntitiesDataContext.SaveChanges();

                        batchDetails = nISEntitiesDataContext.BatchDetailRecords.Where(item => item.BatchId == batchMaster.Id && item.StatementId == statement.Identifier && item.TenantCode == tenantCode)?.ToList();
                        customerMasters = nISEntitiesDataContext.CustomerMasterRecords.Where(item => item.BatchId == batchMaster.Id && item.TenantCode == tenantCode).ToList();
                    }

                    if (customerMasters.Count > 0)
                    {
                        ParallelOptions parallelOptions = new ParallelOptions();
                        parallelOptions.MaxDegreeOfParallelism = parallelThreadCount;
                        Parallel.ForEach(customerMasters, parallelOptions, customer =>
                        {
                            this.CreateCustomerStatement(customer, statement, scheduleLog, statementPageContents, batchMaster, batchDetails, baseURL, tenantCode, customerMasters.Count, outputLocation, tenantConfiguration, client);
                        });
                        //customerMasters.ForEach(customer =>
                        //{
                        //    this.CreateCustomerStatement(customer, statement, scheduleLog, statementPageContents, batchMaster, batchDetails, baseURL, tenantCode, customerMasters.Count, outputLocation, tenantConfiguration, client);
                        //});
                    }
                    else
                    {
                        using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                        {
                            nISEntitiesDataContext.BatchMasterRecords.Where(item => item.Id == batchMaster.Id && item.TenantCode == tenantCode).ToList().ForEach(item => item.Status = BatchStatus.BatchDataNotAvailable.ToString());
                            nISEntitiesDataContext.ScheduleLogRecords.Where(item => item.Id == scheduleLog.Id && item.TenantCode == tenantCode).ToList().ForEach(item => item.Status = ScheduleLogStatus.BatchDataNotAvailable.ToString());
                            nISEntitiesDataContext.ScheduleRunHistoryRecords.Where(item => item.ScheduleLogId == scheduleLog.Id && item.TenantCode == tenantCode).ToList().ForEach(item => item.EndDate = DateTime.UtcNow);
                            nISEntitiesDataContext.ScheduleRecords.Where(item => item.Id == scheduleLog.ScheduleId && item.TenantCode == tenantCode).ToList().ForEach(item => item.Status = ScheduleStatus.BatchDataNotAvailable.ToString());
                            nISEntitiesDataContext.SaveChanges();
                        }
                    }

                    isScheduleSuccess = true;
                }
            }
            catch (Exception ex)
            {
                WriteToFile(ex.Message);
                WriteToFile(ex.InnerException.Message);
                WriteToFile(ex.StackTrace.ToString());
                throw ex;
            }
            return isScheduleSuccess;
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

        #region Batch master
        public bool AddBatchMaster(ScheduleRecord schedule, int start, int end, string tenantCode, int userId)
        {
            bool result = false;
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                List<BatchMasterRecord> batchMasterRecords = new List<BatchMasterRecord>();
                if (end > 0)
                {
                    var batchExecutionDate = DateTime.Now;
                    int ValueToAddInMonth = start;
                    var scheduleStartDate = schedule.StartDate ?? DateTime.Now;
                    for (int index = start; index <= end; index++)
                    {
                        BatchMasterRecord record = new BatchMasterRecord();
                        record.BatchName = "Batch " + index + " of " + schedule.Name;
                        record.TenantCode = tenantCode;
                        record.CreatedBy = userId;
                        record.CreatedDate = DateTime.UtcNow;
                        record.ScheduleId = schedule.Id;
                        record.IsExecuted = false;
                        record.IsDataReady = false;
                        if (index == 1)
                        {
                            if (scheduleStartDate.Day < Convert.ToInt32(schedule.DayOfMonth))
                            {
                                batchExecutionDate = new DateTime(scheduleStartDate.Year, scheduleStartDate.Month, Convert.ToInt32(schedule.DayOfMonth), Convert.ToInt32(schedule.HourOfDay), Convert.ToInt32(schedule.MinuteOfDay), 0);
                                end++;
                            }

                            else
                            {
                                var tempDate = scheduleStartDate.AddMonths(ValueToAddInMonth);
                                batchExecutionDate = new DateTime(tempDate.Year, tempDate.Month, Convert.ToInt32(schedule.DayOfMonth), Convert.ToInt32(schedule.HourOfDay), Convert.ToInt32(schedule.MinuteOfDay), 0);
                                ValueToAddInMonth++;
                            }
                        }
                        else
                        {
                            var tempDate = scheduleStartDate.AddMonths(ValueToAddInMonth);
                            batchExecutionDate = new DateTime(tempDate.Year, tempDate.Month, Convert.ToInt32(schedule.DayOfMonth), Convert.ToInt32(schedule.HourOfDay), Convert.ToInt32(schedule.MinuteOfDay), 0);
                            ValueToAddInMonth++;
                        }
                        record.BatchExecutionDate = batchExecutionDate;
                        record.DataExtractionDate = batchExecutionDate.AddDays(-1);
                        record.Status = BatchStatus.New.ToString();
                        batchMasterRecords.Add(record);
                    }
                    using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                    {
                        nISEntitiesDataContext.BatchMasterRecords.AddRange(batchMasterRecords);
                        nISEntitiesDataContext.SaveChanges();
                        result = true;
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method helps to add batch with no repeat occurence.
        /// </summary>
        /// <param name="schedule"></param>
        /// <param name="tenantCode"></param>
        /// <param name="userId"></param>
        /// <returns>true if added successfully otherwise false</returns>
        private bool AddDoesNotRepeatBatch(DateTime scheduleStartDate, ScheduleRecord schedule, string tenantCode, int userId)
        {
            try
            {
                //var newstartdate = DateTime.SpecifyKind((DateTime)scheduleStartDate, DateTimeKind.Utc);
                var newstartdate = new DateTime(scheduleStartDate.Year, scheduleStartDate.Month, (scheduleStartDate.Day + 1), 0, 0, 0);
                newstartdate = DateTime.SpecifyKind((DateTime)newstartdate, DateTimeKind.Utc);
                this.SetAndValidateConnectionString(tenantCode);
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    BatchMasterRecord record = new BatchMasterRecord();
                    record.BatchName = "Batch 1 of " + schedule.Name;
                    record.TenantCode = tenantCode;
                    record.CreatedBy = userId;
                    record.CreatedDate = DateTime.UtcNow;
                    record.ScheduleId = schedule.Id;
                    record.IsExecuted = false;
                    record.IsDataReady = false;
                    var batchExecutionDate = new DateTime(newstartdate.Year, newstartdate.Month, newstartdate.Day, Convert.ToInt32(schedule.HourOfDay), Convert.ToInt32(schedule.MinuteOfDay), 0);
                    record.BatchExecutionDate = batchExecutionDate;
                    record.DataExtractionDate = batchExecutionDate.AddDays(-1);
                    record.Status = BatchStatus.New.ToString();
                    nISEntitiesDataContext.BatchMasterRecords.Add(record);
                    nISEntitiesDataContext.SaveChanges();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method helps to add batch on daily basis schedule run occurenace.
        /// </summary>
        /// <param name="schedule"></param>
        /// <param name="tenantCode"></param>
        /// <param name="userId"></param>
        /// <param name="isCustom"></param>
        /// <returns>true if added successfully otherwise false</returns>
        private bool AddDailyOccurenceScheduleBatches(DateTime scheduleStartDate, ScheduleRecord schedule, string tenantCode, int userId, int batchIndex)
        {
            bool result = false;
            try
            {
                var repeatEveryDays = schedule.RecurrancePattern == ModelConstant.CUSTOM_DAY ? Convert.ToInt32(schedule.RepeatEveryDayMonWeekYear != 0 ? schedule.RepeatEveryDayMonWeekYear : 1) : 1;
                this.SetAndValidateConnectionString(tenantCode);
                List<BatchMasterRecord> batchMasterRecords = new List<BatchMasterRecord>();
                int dayDiff = 0;
                if (schedule.EndDate != null)
                {
                    dayDiff = this.utility.DayDifference(schedule.EndDate ?? DateTime.Now, scheduleStartDate) + 1;
                }
                else
                {
                    dayDiff = Convert.ToInt32(schedule.NoOfOccurrences ?? 1) * repeatEveryDays;
                }

                if (dayDiff > 0)
                {
                    var newstartdate = new DateTime(scheduleStartDate.Year, scheduleStartDate.Month, (scheduleStartDate.Day + 1), 0, 0, 0);
                    newstartdate = DateTime.SpecifyKind((DateTime)newstartdate, DateTimeKind.Utc);
                    int idx = 1;
                    while (idx <= dayDiff)
                    {
                        BatchMasterRecord record = new BatchMasterRecord();
                        record.BatchName = "Batch " + batchIndex + " of " + schedule.Name;
                        record.TenantCode = tenantCode;
                        record.CreatedBy = userId;
                        record.CreatedDate = DateTime.UtcNow;
                        record.ScheduleId = schedule.Id;
                        record.IsExecuted = false;
                        record.IsDataReady = false;
                        var batchExecutionDate = new DateTime(newstartdate.Year, newstartdate.Month, newstartdate.Day, Convert.ToInt32(schedule.HourOfDay), Convert.ToInt32(schedule.MinuteOfDay), 0);
                        record.BatchExecutionDate = batchExecutionDate;
                        record.DataExtractionDate = batchExecutionDate.AddDays(-1);
                        record.Status = BatchStatus.New.ToString();
                        batchMasterRecords.Add(record);
                        newstartdate = newstartdate.AddDays(repeatEveryDays);
                        batchIndex++;
                        idx = idx + repeatEveryDays;
                    }
                    using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                    {
                        nISEntitiesDataContext.BatchMasterRecords.AddRange(batchMasterRecords);
                        nISEntitiesDataContext.SaveChanges();
                        result = true;
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method helps to add batch on weekday or weekly basis schedule run occurenace.
        /// </summary>
        /// <param name="schedule"></param>
        /// <param name="tenantCode"></param>
        /// <param name="userId"></param>
        /// <param name="isCustom"></param>
        /// <returns>true if added successfully otherwise false</returns>
        private bool AddWeeklyOccurenceScheduleBatches(DateTime scheduleStartDate, ScheduleRecord schedule, string tenantCode, int userId, int batchIndex)
        {
            bool result = false;
            try
            {
                var repeatEveryDays = schedule.RecurrancePattern == ModelConstant.CUSTOM_WEEK ? Convert.ToInt32(schedule.RepeatEveryDayMonWeekYear != 0 ? schedule.RepeatEveryDayMonWeekYear : 1) : 1;
                this.SetAndValidateConnectionString(tenantCode);
                List<BatchMasterRecord> batchMasterRecords = new List<BatchMasterRecord>();

                int dayDiff = 0;
                //var scheduleStartDate = schedule.StartDate ?? DateTime.Now;
                if (schedule.EndDate != null)
                {
                    dayDiff = this.utility.DayDifference(schedule.EndDate ?? DateTime.Now, scheduleStartDate) + 1;
                }
                else
                {
                    dayDiff = Convert.ToInt32(schedule.NoOfOccurrences);
                }

                var newstartdate = new DateTime(scheduleStartDate.Year, scheduleStartDate.Month, (scheduleStartDate.Day + 1), 0, 0, 0);
                newstartdate = DateTime.SpecifyKind((DateTime)newstartdate, DateTimeKind.Utc);
                //var newstartdate = DateTime.SpecifyKind((DateTime)scheduleStartDate, DateTimeKind.Utc);
                if (dayDiff > 0)
                {
                    if (schedule.RecurrancePattern == ModelConstant.WEEKDAY)
                    {
                        for (int index = 1; index <= dayDiff; index++)
                        {
                            if (newstartdate.DayOfWeek != DayOfWeek.Saturday && newstartdate.DayOfWeek != DayOfWeek.Sunday)
                            {
                                BatchMasterRecord record = new BatchMasterRecord();
                                record.BatchName = "Batch " + batchIndex + " of " + schedule.Name;
                                record.TenantCode = tenantCode;
                                record.CreatedBy = userId;
                                record.CreatedDate = DateTime.UtcNow;
                                record.ScheduleId = schedule.Id;
                                record.IsExecuted = false;
                                record.IsDataReady = false;
                                var batchExecutionDate = new DateTime(newstartdate.Year, newstartdate.Month, newstartdate.Day, Convert.ToInt32(schedule.HourOfDay), Convert.ToInt32(schedule.MinuteOfDay), 0);
                                record.BatchExecutionDate = batchExecutionDate;
                                record.DataExtractionDate = batchExecutionDate.AddDays(-1);
                                record.Status = BatchStatus.New.ToString();
                                batchMasterRecords.Add(record);
                                batchIndex++;
                            }
                            newstartdate = newstartdate.AddDays(repeatEveryDays);
                        }
                    }
                    else
                    {
                        var days = schedule.WeekDays.Split(new Char[] { ',' });
                        int idx = 1, noOfDaysInWeek = 7;
                        while (idx <= dayDiff)
                        {
                            if (days.Contains(newstartdate.DayOfWeek.ToString()))
                            {
                                BatchMasterRecord record = new BatchMasterRecord();
                                record.BatchName = "Batch " + batchIndex + " of " + schedule.Name;
                                record.TenantCode = tenantCode;
                                record.CreatedBy = userId;
                                record.CreatedDate = DateTime.UtcNow;
                                record.ScheduleId = schedule.Id;
                                record.IsExecuted = false;
                                record.IsDataReady = false;
                                var batchExecutionDate = new DateTime(newstartdate.Year, newstartdate.Month, newstartdate.Day, Convert.ToInt32(schedule.HourOfDay), Convert.ToInt32(schedule.MinuteOfDay), 0);
                                record.BatchExecutionDate = batchExecutionDate;
                                record.DataExtractionDate = batchExecutionDate.AddDays(-1);
                                record.Status = BatchStatus.New.ToString();
                                batchMasterRecords.Add(record);
                                batchIndex++;
                                if (schedule.NoOfOccurrences != null && schedule.NoOfOccurrences != 0)
                                {
                                    idx++;
                                }
                            }

                            if (newstartdate.DayOfWeek != DayOfWeek.Sunday)
                            {
                                newstartdate = newstartdate.AddDays(1);
                                if (schedule.EndDate != null)
                                {
                                    idx++;
                                }
                            }
                            else
                            {
                                newstartdate = newstartdate.AddDays(repeatEveryDays > 1 ? ((repeatEveryDays--) * noOfDaysInWeek) : 1);
                                if (schedule.EndDate != null)
                                {
                                    idx = idx + (repeatEveryDays > 1 ? ((repeatEveryDays--) * noOfDaysInWeek) : 1);
                                }
                            }
                        }
                    }

                    using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                    {
                        nISEntitiesDataContext.BatchMasterRecords.AddRange(batchMasterRecords);
                        nISEntitiesDataContext.SaveChanges();
                        result = true;
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method helps to add batch on monthly basis schedule run occurenace.
        /// </summary>
        /// <param name="schedule"></param>
        /// <param name="tenantCode"></param>
        /// <param name="userId"></param>
        /// <param name="isCustom"></param>
        /// <returns>true if added successfully otherwise false</returns>
        private bool AddMonthlyOccurenceScheduleBatches(DateTime scheduleStartDate, ScheduleRecord schedule, string tenantCode, int userId, int batchIndex)
        {
            bool result = false;
            try
            {
                var repeatEveryMonths = schedule.RecurrancePattern == ModelConstant.CUSTOM_MONTH ? Convert.ToInt32(schedule.RepeatEveryDayMonWeekYear != 0 ? schedule.RepeatEveryDayMonWeekYear : 1) : 1;
                this.SetAndValidateConnectionString(tenantCode);
                List<BatchMasterRecord> batchMasterRecords = new List<BatchMasterRecord>();

                var newstartdate = DateTime.SpecifyKind((DateTime)scheduleStartDate, DateTimeKind.Utc);
                if (schedule.DayOfMonth < newstartdate.Day)
                {
                    newstartdate = newstartdate.AddMonths(1);
                }

                if (schedule.EndDate != null)
                {
                    while (DateTime.Compare(newstartdate, schedule.EndDate ?? DateTime.Now) < 0)
                    {
                        int DayOfMonth = Convert.ToInt32(schedule.DayOfMonth);
                        if (schedule.DayOfMonth > 28)
                        {
                            int lastDayOfMonth = DateTime.DaysInMonth(newstartdate.Year, newstartdate.Month);
                            if (lastDayOfMonth < DayOfMonth)
                            {
                                DayOfMonth = lastDayOfMonth;
                            }
                        }
                        
                        BatchMasterRecord record = new BatchMasterRecord();
                        record.BatchName = "Batch " + batchIndex + " of " + schedule.Name;
                        record.TenantCode = tenantCode;
                        record.CreatedBy = userId;
                        record.CreatedDate = DateTime.UtcNow;
                        record.ScheduleId = schedule.Id;
                        record.IsExecuted = false;
                        record.IsDataReady = false;
                        var batchExecutionDate = new DateTime(newstartdate.Year, newstartdate.Month, DayOfMonth, Convert.ToInt32(schedule.HourOfDay), Convert.ToInt32(schedule.MinuteOfDay), 0);
                        record.BatchExecutionDate = batchExecutionDate;
                        record.DataExtractionDate = batchExecutionDate.AddDays(-1);
                        record.Status = BatchStatus.New.ToString();
                        batchMasterRecords.Add(record);
                        newstartdate = newstartdate.AddMonths(repeatEveryMonths);
                        batchIndex++;
                    }
                }
                else if (schedule.NoOfOccurrences != null && schedule.NoOfOccurrences > 0)
                {
                    long? occurences = schedule.NoOfOccurrences;
                    while (occurences != null && occurences > 0)
                    {
                        int DayOfMonth = Convert.ToInt32(schedule.DayOfMonth);
                        if (schedule.DayOfMonth > 28)
                        {
                            int lastDayOfMonth = DateTime.DaysInMonth(newstartdate.Year, newstartdate.Month);
                            if (lastDayOfMonth < DayOfMonth)
                            {
                                DayOfMonth = lastDayOfMonth;
                            }
                        }
                        BatchMasterRecord record = new BatchMasterRecord();
                        record.BatchName = "Batch " + batchIndex + " of " + schedule.Name;
                        record.TenantCode = tenantCode;
                        record.CreatedBy = userId;
                        record.CreatedDate = DateTime.UtcNow;
                        record.ScheduleId = schedule.Id;
                        record.IsExecuted = false;
                        record.IsDataReady = false;
                        var batchExecutionDate = new DateTime(newstartdate.Year, newstartdate.Month, DayOfMonth, Convert.ToInt32(schedule.HourOfDay), Convert.ToInt32(schedule.MinuteOfDay), 0);
                        record.BatchExecutionDate = batchExecutionDate;
                        record.DataExtractionDate = batchExecutionDate.AddDays(-1);
                        record.Status = BatchStatus.New.ToString();
                        batchMasterRecords.Add(record);
                        newstartdate = newstartdate.AddMonths(repeatEveryMonths);
                        batchIndex++;
                        occurences--;
                    }
                }

                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    nISEntitiesDataContext.BatchMasterRecords.AddRange(batchMasterRecords);
                    nISEntitiesDataContext.SaveChanges();
                    result = true;
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method helps to add batch on yearly basis schedule run occurenace.
        /// </summary>
        /// <param name="schedule"></param>
        /// <param name="tenantCode"></param>
        /// <param name="userId"></param>
        /// <param name="isCustom"></param>
        /// <returns>true if added successfully otherwise false</returns>
        private bool AddYearlyOccurenceScheduleBatches(DateTime scheduleStartDate, ScheduleRecord schedule, string tenantCode, int userId, int batchIndex)
        {
            try
            {
                var repeatEveryYears = schedule.RecurrancePattern == ModelConstant.CUSTOM_YEAR ? Convert.ToInt32(schedule.RepeatEveryDayMonWeekYear != 0 ? schedule.RepeatEveryDayMonWeekYear : 1) : 1;
                this.SetAndValidateConnectionString(tenantCode);
                List<BatchMasterRecord> batchMasterRecords = new List<BatchMasterRecord>();

                var startdate = DateTime.SpecifyKind((DateTime)scheduleStartDate, DateTimeKind.Utc);
                if (this.utility.getNumericMonth(schedule.MonthOfYear) < startdate.Month
                    || (this.utility.getNumericMonth(schedule.MonthOfYear) == startdate.Month && startdate.Day > schedule.DayOfMonth))
                {
                    startdate = startdate.AddYears(1);
                }

                int DayOfMonth = Convert.ToInt32(schedule.DayOfMonth);
                if (schedule.DayOfMonth > 28)
                {
                    int lastDayOfMonth = DateTime.DaysInMonth(startdate.Year, this.utility.getNumericMonth(schedule.MonthOfYear));
                    if (lastDayOfMonth < DayOfMonth)
                    {
                        DayOfMonth = lastDayOfMonth;
                    }
                }
                
                var newstartdate = new DateTime(startdate.Year, this.utility.getNumericMonth(schedule.MonthOfYear), DayOfMonth, 0, 0, 0);
                newstartdate = DateTime.SpecifyKind((DateTime)newstartdate, DateTimeKind.Utc);

                var yearDiff = 0;
                if (schedule.EndDate != null)
                {
                    yearDiff = this.utility.YearDifference(newstartdate, schedule.EndDate ?? DateTime.Now) + 1;
                }
                else
                {
                    yearDiff = Convert.ToInt32(schedule.NoOfOccurrences);
                }

                if (yearDiff > 0)
                {
                    int idx = 1;
                    while (idx <= yearDiff)
                    {
                        BatchMasterRecord record = new BatchMasterRecord();
                        record.BatchName = "Batch " + batchIndex + " of " + schedule.Name;
                        record.TenantCode = tenantCode;
                        record.CreatedBy = userId;
                        record.CreatedDate = DateTime.UtcNow;
                        record.ScheduleId = schedule.Id;
                        record.IsExecuted = false;
                        record.IsDataReady = false;
                        var batchExecutionDate = new DateTime(newstartdate.Year, newstartdate.Month, newstartdate.Day, Convert.ToInt32(schedule.HourOfDay), Convert.ToInt32(schedule.MinuteOfDay), 0);
                        record.BatchExecutionDate = batchExecutionDate;
                        record.DataExtractionDate = batchExecutionDate.AddDays(-1);
                        record.Status = BatchStatus.New.ToString();
                        batchMasterRecords.Add(record);
                        newstartdate = newstartdate.AddYears(repeatEveryYears);
                        batchIndex++;
                        idx = idx + repeatEveryYears;
                    }
                    using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                    {
                        nISEntitiesDataContext.BatchMasterRecords.AddRange(batchMasterRecords);
                        nISEntitiesDataContext.SaveChanges();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IList<BatchMaster> GetBatchMasters(long schdeuleIdentifier, string tenantCode)
        {
            IList<BatchMaster> batchMasters = new List<BatchMaster>();
            IList<BatchMasterRecord> batchMasterRecords = new List<BatchMasterRecord>();

            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    batchMasterRecords = nISEntitiesDataContext.BatchMasterRecords.Where(item => item.ScheduleId == schdeuleIdentifier && item.TenantCode == tenantCode).ToList();
                    if (batchMasterRecords?.Count() > 0)
                    {
                        batchMasterRecords.ToList().ForEach(item =>
                        {
                            batchMasters.Add(new BatchMaster
                            {
                                Identifier = item.Id,
                                BatchName = item.BatchName,
                                TenantCode = item.TenantCode == string.Empty ? tenantCode : item.TenantCode,
                                CreatedBy = item.CreatedBy,
                                CreatedDate = item.CreatedDate,
                                ScheduleId = item.ScheduleId,
                                IsExecuted = item.IsExecuted,
                                IsDataReady = item.IsDataReady,
                                BatchExecutionDate = item.BatchExecutionDate,
                                DataExtractionDate = item.DataExtractionDate,
                                Status = item.Status,
                            });
                        });
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return batchMasters;
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
            if (validationEngine.IsValidText(searchParameter.StatementDefinitionName))
            {
                queryString.Append(string.Format("StatementName.Contains(\"{0}\") and ", searchParameter.StatementDefinitionName));
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

        private void CreateCustomerStatement(CustomerMasterRecord customer, Statement statement, ScheduleLogRecord scheduleLog, IList<StatementPageContent> statementPageContents, BatchMasterRecord batchMaster, IList<BatchDetailRecord> batchDetails, string baseURL, string tenantCode, int customerCount, string outputLocation, TenantConfiguration tenantConfiguration, Client client)
        {
            IList<StatementMetadataRecord> statementMetadataRecords = new List<StatementMetadataRecord>();

            try
            {
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    var renderEngine = nISEntitiesDataContext.RenderEngineRecords.Where(item => item.Id == 1).FirstOrDefault();
                    var logDetailRecord = this.statementRepository.GenerateStatements(customer, statement, statementPageContents, batchMaster, batchDetails, baseURL, tenantCode, outputLocation, tenantConfiguration, client);
                    if (logDetailRecord != null)
                    {
                        logDetailRecord.ScheduleLogId = scheduleLog.Id;
                        logDetailRecord.CustomerId = customer.Id;
                        logDetailRecord.CustomerName = customer.FirstName.Trim() + (customer.MiddleName == string.Empty ? string.Empty : " " + customer.MiddleName.Trim()) + " " + customer.LastName.Trim();
                        logDetailRecord.ScheduleId = scheduleLog.ScheduleId;
                        logDetailRecord.RenderEngineId = renderEngine.Id; //To be change once render engine implmentation start
                        logDetailRecord.RenderEngineName = renderEngine.Name;
                        logDetailRecord.RenderEngineURL = renderEngine.URL;
                        logDetailRecord.NumberOfRetry = 1;
                        logDetailRecord.CreationDate = DateTime.UtcNow;
                        logDetailRecord.TenantCode = tenantCode;
                        nISEntitiesDataContext.ScheduleLogDetailRecords.Add(logDetailRecord);
                        nISEntitiesDataContext.SaveChanges();

                        if (logDetailRecord.Status.ToLower().Equals(ScheduleLogStatus.Completed.ToString().ToLower()))
                        {
                            if (logDetailRecord.StatementMetadataRecords.Count > 0)
                            {
                                logDetailRecord.StatementMetadataRecords.ToList().ForEach(metarec =>
                                {
                                    metarec.ScheduleLogId = scheduleLog.Id;
                                    metarec.ScheduleId = scheduleLog.ScheduleId;
                                    metarec.StatementDate = DateTime.UtcNow;
                                    metarec.StatementURL = logDetailRecord.StatementFilePath;
                                    metarec.TenantCode = tenantCode;
                                    statementMetadataRecords.Add(metarec);
                                });
                                nISEntitiesDataContext.StatementMetadataRecords.AddRange(statementMetadataRecords);
                                nISEntitiesDataContext.SaveChanges();
                            }
                        }
                        else if (logDetailRecord.Status.ToLower().Equals(ScheduleLogStatus.Failed.ToString().ToLower()))
                        {
                            this.utility.DeleteUnwantedDirectory(batchMaster.Id, customer.Id, outputLocation);
                        }
                    }

                    var logDetailsRecords = nISEntitiesDataContext.ScheduleLogDetailRecords.Where(item => item.ScheduleLogId == scheduleLog.Id)?.ToList();
                    if (customerCount == logDetailsRecords.Count)
                    {
                        var scheduleLogStatus = ScheduleLogStatus.Completed.ToString();
                        var failedRecords = logDetailsRecords.Where(item => item.Status == ScheduleLogStatus.Failed.ToString())?.ToList();
                        if (failedRecords != null && failedRecords.Count > 0)
                        {
                            scheduleLogStatus = ScheduleLogStatus.Failed.ToString();
                        }
                        nISEntitiesDataContext.ScheduleLogRecords.Where(item => item.Id == scheduleLog.Id).ToList().ForEach(item => item.Status = scheduleLogStatus);
                        nISEntitiesDataContext.ScheduleRunHistoryRecords.Where(item => item.ScheduleLogId == scheduleLog.Id).ToList().ForEach(item => item.EndDate = DateTime.UtcNow);

                        nISEntitiesDataContext.ScheduleRecords.Where(item => item.Id == scheduleLog.ScheduleId).ToList().ForEach(item => item.Status = ScheduleStatus.Completed.ToString());
                        nISEntitiesDataContext.BatchMasterRecords.Where(item => item.Id == batchMaster.Id).ToList().ForEach(item =>
                        {
                            item.Status = BatchStatus.Completed.ToString();
                            item.IsExecuted = true;
                        });
                        nISEntitiesDataContext.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private void WriteToFile(string Message)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\ScheduleLog_" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') + ".txt";
            if (!File.Exists(filepath))
            {
                // Create a file to write to.   
                using (StreamWriter sw = File.CreateText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
        }


        #endregion

        #endregion
    }
}
