// <copyright file="SQLScheduleRepository.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2018 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------

namespace nIS
{
    using Microsoft.Practices.ObjectBuilder2;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using nIS.NedBank;
    using NIS.Repository.Entities;
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
    /// <seealso cref="nIS.IScheduleRepository" />
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
        /// The tenant configuration repository.
        /// </summary>
        private ITenantConfigurationRepository tenantConfigurationRepository = null;

        /// <summary>
        /// The Dynamic widget repository.
        /// </summary>
        private IDynamicWidgetRepository dynamicWidgetRepository = null;

        /// <summary>
        /// The crypto manager
        /// </summary>
        private readonly ICryptoManager cryptoManager;

        private IProductRepository productRepository = null;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SQLScheduleRepository" /> class.
        /// </summary>
        /// <param name="unityContainer">The unity container.</param>
        public SQLScheduleRepository(IUnityContainer unityContainer)
        {
            this.unityContainer = unityContainer;
            this.validationEngine = new ValidationEngine();
            this.utility = new Utility();
            this.configurationutility = new ConfigurationUtility(this.unityContainer);
            this.statementRepository = this.unityContainer.Resolve<IStatementRepository>();
            this.tenantConfigurationRepository = this.unityContainer.Resolve<ITenantConfigurationRepository>();
            this.dynamicWidgetRepository = this.unityContainer.Resolve<IDynamicWidgetRepository>();
            this.cryptoManager = this.unityContainer.Resolve<ICryptoManager>();
            this.productRepository = this.unityContainer.Resolve<IProductRepository>();
        }

        #endregion

        #region Public Methods

        #region Schedule 
        /// <summary>
        /// This method adds the specified list of schedule in the repository.
        /// </summary>
        /// <param name="schedules"></param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// True, if the schedule values are added successfully, false otherwise
        /// </returns>
        /// <exception cref="nIS.DuplicateScheduleFoundException"></exception>
        public bool AddSchedules(IList<Schedule> schedules, string tenantCode)
        {
            bool result = false;
            try
            {
                var claims = ClaimsPrincipal.Current.Identities.First().Claims.ToList();
                int userId = 1;
                int.TryParse(claims?.FirstOrDefault(x => x.Type.Equals("UserId", StringComparison.OrdinalIgnoreCase)).Value, out userId);
                var userFullName = claims?.FirstOrDefault(x => x.Type.Equals("UserFullName", StringComparison.OrdinalIgnoreCase)).Value;

                this.SetAndValidateConnectionString(tenantCode);

                if (this.IsDuplicateSchedule(schedules, "AddOperation", tenantCode))
                {
                    throw new DuplicateScheduleFoundException(tenantCode);
                }
                IList<ScheduleRecord> scheduleRecords = new List<ScheduleRecord>();

                schedules.ToList().ForEach(schedule =>
                {
                    //DateTime startDateTime = DateTime.SpecifyKind(Convert.ToDateTime(schedule.StartDate), DateTimeKind.Utc);
                    //DateTime? endDateTime = DateTime.SpecifyKind(Convert.ToDateTime(schedule.EndDate), DateTimeKind.Utc);

                    var startDateTime = schedule.StartDate + TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now);
                    var endDateTime = schedule.EndDate + TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now);
                    var productType = productRepository.Get_ProductById((int)schedule.ProductId, tenantCode);
                    var randomNumber = Guid.NewGuid().ToString().Split('-')[0];

                    long? productBatchId = 0;
                    using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                    {
                        //productBatchId = nISEntitiesDataContext.ScheduleRecords.Count() > 0 ? nISEntitiesDataContext.ScheduleRecords.Max(x => x.ProductBatchId) : 0;
                        if (nISEntitiesDataContext.ScheduleRecords.Count() > 0)
                            productBatchId = nISEntitiesDataContext.ScheduleRecords.Max(x => x.ProductBatchId);
                        else
                            productBatchId = 0;

                    }
                    if(productBatchId == null)
                        productBatchId = 0;
                    productBatchId++;

                    scheduleRecords.Add(new ScheduleRecord()
                    {
                        ScheduleNameByUser = schedule.ScheduleNameByUser,
                        ProductBatchId = productBatchId,
                        ProductId = schedule.ProductId,
                        ProductBatchName = $"{schedule.Name}_{productType}_{randomNumber}",
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
                DateTime endDate = DateTime.MinValue;
                int noOfOccurance = 0;

                if (result)
                {
                    IList<SystemActivityHistoryRecord> Records = new List<SystemActivityHistoryRecord>();
                    scheduleRecords.ToList().ForEach(schedulerecord =>
                    {
                        int batchIndex = 1;
                        if (schedulerecord.RecurrancePattern == ModelConstant.DOES_NOT_REPEAT)
                        {
                            this.AddDoesNotRepeatBatch(schedulerecord.StartDate ?? DateTime.Now, schedulerecord, tenantCode, userId, out endDate, out noOfOccurance);
                        }
                        else if (schedulerecord.RecurrancePattern == ModelConstant.DAILY || schedulerecord.RecurrancePattern == ModelConstant.CUSTOM_DAY)
                        {
                            this.AddDailyOccurenceScheduleBatches(schedulerecord.StartDate ?? DateTime.Now, schedulerecord, tenantCode, userId, batchIndex, out endDate, out noOfOccurance);
                        }
                        else if (schedulerecord.RecurrancePattern == ModelConstant.WEEKDAY || schedulerecord.RecurrancePattern == ModelConstant.WEEKLY || schedulerecord.RecurrancePattern == ModelConstant.CUSTOM_WEEK)
                        {
                            this.AddWeeklyOccurenceScheduleBatches(schedulerecord.StartDate ?? DateTime.Now, schedulerecord, tenantCode, userId, batchIndex, out endDate, out noOfOccurance);
                        }
                        else if (schedulerecord.RecurrancePattern == ModelConstant.MONTHLY || schedulerecord.RecurrancePattern == ModelConstant.CUSTOM_MONTH)
                        {
                            this.AddMonthlyOccurenceScheduleBatches(schedulerecord.StartDate ?? DateTime.Now, schedulerecord, tenantCode, userId, batchIndex, out endDate, out noOfOccurance);
                        }
                        else if (schedulerecord.RecurrancePattern == ModelConstant.YEARLY || schedulerecord.RecurrancePattern == ModelConstant.CUSTOM_YEAR)
                        {
                            this.AddYearlyOccurenceScheduleBatches(schedulerecord.StartDate ?? DateTime.Now, schedulerecord, tenantCode, userId, batchIndex, out endDate, out noOfOccurance);
                        }

                        Records.Add(new SystemActivityHistoryRecord()
                        {
                            Module = ModelConstant.SCHEDULE_MODEL_SECTION,
                            EntityId = schedulerecord.Id,
                            EntityName = schedulerecord.Name,
                            SubEntityId = null,
                            SubEntityName = null,
                            ActionTaken = "Add",
                            ActionTakenBy = userId,
                            ActionTakenByUserName = userFullName,
                            ActionTakenDate = DateTime.Now,
                            TenantCode = tenantCode
                        });
                    });

                    if (Records.Count > 0)
                    {
                        using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                        {
                            nISEntitiesDataContext.SystemActivityHistoryRecords.AddRange(Records);
                            nISEntitiesDataContext.SaveChanges();
                        }
                    }
                }
            }

            catch
            {
                throw;
            }

            return result;
        }

        /// <summary>
        /// Adds the schedules with language.
        /// </summary>
        /// <param name="schedules"></param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns></returns>
        /// <exception cref="nIS.DuplicateScheduleFoundException"></exception>
        public bool AddSchedulesWithLanguage(IList<Schedule> schedules, string tenantCode)
        {
            bool result = false;
            try
            {
                var claims = ClaimsPrincipal.Current.Identities.First().Claims.ToList();
                int userId = 1;
                int.TryParse(claims?.FirstOrDefault(x => x.Type.Equals("UserId", StringComparison.OrdinalIgnoreCase)).Value, out userId);
                var userFullName = claims?.FirstOrDefault(x => x.Type.Equals("UserFullName", StringComparison.OrdinalIgnoreCase)).Value;

                this.SetAndValidateConnectionString(tenantCode);

                if (this.IsDuplicateSchedule(schedules, "AddOperation", tenantCode))
                {
                    throw new DuplicateScheduleFoundException(tenantCode);
                }
                IList<ScheduleRecord> scheduleRecords = new List<ScheduleRecord>();
                schedules.ToList().ForEach(schedule =>
                {
                    //DateTime startDateTime = DateTime.SpecifyKind(Convert.ToDateTime(schedule.StartDate), DateTimeKind.Utc);
                    //DateTime? endDateTime = DateTime.SpecifyKind(Convert.ToDateTime(schedule.EndDate), DateTimeKind.Utc);

                    var startDateTime = schedule.StartDate + TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now);
                    var endDateTime = schedule.EndDate + TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now);
                    var productType = productRepository.Get_ProductById((int)schedule.ProductId, tenantCode);
                    var randomNumber = Guid.NewGuid().ToString().Split('-')[0];

                    scheduleRecords.Add(new ScheduleRecord()
                    {
                        ProductBatchName = $"{schedule.Name}_{productType}_{randomNumber}",
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
                        NoOfOccurrences = schedule.NoOfOccurrences,
                        Languages = string.Join(",", schedule.Languages),
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
                    IList<SystemActivityHistoryRecord> Records = new List<SystemActivityHistoryRecord>();
                    scheduleRecords.ToList().ForEach(schedulerecord =>
                    {
                        int batchIndex = 1;
                        if (schedulerecord.RecurrancePattern == ModelConstant.DOES_NOT_REPEAT)
                        {
                            this.AddDoesNotRepeatBatchWithLanguage(schedulerecord.StartDate ?? DateTime.Now, schedulerecord, tenantCode, userId);
                        }
                        else if (schedulerecord.RecurrancePattern == ModelConstant.DAILY || schedulerecord.RecurrancePattern == ModelConstant.CUSTOM_DAY)
                        {
                            this.AddDailyOccurenceScheduleBatchesWithLanguage(schedulerecord.StartDate ?? DateTime.Now, schedulerecord, tenantCode, userId, batchIndex);
                        }
                        else if (schedulerecord.RecurrancePattern == ModelConstant.WEEKDAY || schedulerecord.RecurrancePattern == ModelConstant.WEEKLY || schedulerecord.RecurrancePattern == ModelConstant.CUSTOM_WEEK)
                        {
                            this.AddWeeklyOccurenceScheduleBatchesWithLanguage(schedulerecord.StartDate ?? DateTime.Now, schedulerecord, tenantCode, userId, batchIndex);
                        }
                        else if (schedulerecord.RecurrancePattern == ModelConstant.MONTHLY || schedulerecord.RecurrancePattern == ModelConstant.CUSTOM_MONTH)
                        {
                            this.AddMonthlyOccurenceScheduleBatchesWithLanguage(schedulerecord.StartDate ?? DateTime.Now, schedulerecord, tenantCode, userId, batchIndex);
                        }
                        else if (schedulerecord.RecurrancePattern == ModelConstant.YEARLY || schedulerecord.RecurrancePattern == ModelConstant.CUSTOM_YEAR)
                        {
                            this.AddYearlyOccurenceScheduleBatchesWithLanguage(schedulerecord.StartDate ?? DateTime.Now, schedulerecord, tenantCode, userId, batchIndex);
                        }

                        Records.Add(new SystemActivityHistoryRecord()
                        {
                            Module = ModelConstant.SCHEDULE_MODEL_SECTION,
                            EntityId = schedulerecord.Id,
                            EntityName = schedulerecord.Name,
                            SubEntityId = null,
                            SubEntityName = null,
                            ActionTaken = "Add",
                            ActionTakenBy = userId,
                            ActionTakenByUserName = userFullName,
                            ActionTakenDate = DateTime.Now,
                            TenantCode = tenantCode
                        });
                    });

                    if (Records.Count > 0)
                    {
                        using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                        {
                            nISEntitiesDataContext.SystemActivityHistoryRecords.AddRange(Records);
                            nISEntitiesDataContext.SaveChanges();
                        }
                    }
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
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// True, if the schedule values are updated successfully,otherwise false
        /// </returns>
        /// <exception cref="nIS.DuplicateScheduleFoundException"></exception>
        /// <exception cref="nIS.ScheduleNotFoundException"></exception>
        public bool UpdateSchedules(IList<Schedule> schedules, string tenantCode)
        {
            bool result = false;
            try
            {
                var claims = ClaimsPrincipal.Current.Identities.First().Claims.ToList();
                int userId = 1;
                int.TryParse(claims?.FirstOrDefault(x => x.Type.Equals("UserId", StringComparison.OrdinalIgnoreCase)).Value, out userId);
                var userFullName = claims?.FirstOrDefault(x => x.Type.Equals("UserFullName", StringComparison.OrdinalIgnoreCase)).Value;

                this.SetAndValidateConnectionString(tenantCode);
                if (this.IsDuplicateSchedule(schedules, "UpdateOperation", tenantCode))
                {
                    throw new DuplicateScheduleFoundException(tenantCode);
                }
                IList<ScheduleRecord> scheduleRecords = new List<ScheduleRecord>();
                var productBatchName = schedules.Select(x => x.productBatchName).Where(z => z != null && z != string.Empty).FirstOrDefault();
                long productBatchId = 0;
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    //StringBuilder query = new StringBuilder();
                    //query.Append("(" + string.Join("or ", string.Join(",", schedules.Select(item => item.Identifier).Distinct()).ToString().Split(',').Select(item => string.Format("Id.Equals({0}) ", item))) + ") ");

                    //IList<ScheduleRecord> scheduleRecords = nISEntitiesDataContext.ScheduleRecords.Where(query.ToString()).Select(item => item).AsQueryable().ToList();
                    //if (scheduleRecords == null || scheduleRecords.Count <= 0 || scheduleRecords.Count() != string.Join(",", scheduleRecords.Select(item => item.Id).Distinct()).ToString().Split(',').Length)
                    //{
                    //    throw new ScheduleNotFoundException(tenantCode);
                    //}

                    var listOfSchedule = nISEntitiesDataContext.ScheduleRecords.Where(y => y.ProductBatchName == productBatchName).ToList();
                    var scheduleIds = listOfSchedule.Select(x => x.Id).ToList();
                    productBatchId = (long)(listOfSchedule.Count() > 0 ? listOfSchedule.FirstOrDefault().ProductBatchId : 0);
                    var listOfBatches = nISEntitiesDataContext.BatchMasterRecords.Where(a => scheduleIds.Contains(a.ScheduleId)).ToList();
                    var batchesId = listOfBatches.Select(y => y.Id).ToList();
                    var listOfETL = nISEntitiesDataContext.EtlSchedules.Where(x => x.ProductBatchId == productBatchId).ToList();
                    var listOfETLBatches = nISEntitiesDataContext.EtlBatches.Where(x => x.ProductBatchId == productBatchId).ToList();
                    nISEntitiesDataContext.EtlSchedules.RemoveRange(listOfETL);
                    nISEntitiesDataContext.BatchMasterRecords.RemoveRange(listOfBatches);
                    nISEntitiesDataContext.ScheduleRecords.RemoveRange(listOfSchedule);
                    nISEntitiesDataContext.EtlBatches.RemoveRange(listOfETLBatches);
                    nISEntitiesDataContext.SaveChanges();
                    result = true;
                }

                var productType = productRepository.Get_ProductById((int)schedules[0].ProductId, tenantCode);
                var randomNumber = Guid.NewGuid().ToString().Split('-')[0];

                schedules.ToList().ForEach(schedule =>
                {
                    var startDateTime = schedule.StartDate; //+ TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now);
                    var endDateTime = schedule.EndDate; //+ TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now);

                    scheduleRecords.Add(new ScheduleRecord()
                    {
                        ScheduleNameByUser = schedule.ScheduleNameByUser,
                        ProductBatchId = productBatchId,
                        ProductBatchName = $"{schedule.ScheduleNameByUser}_{productType?.Name}_{randomNumber}",
                        ProductId = schedule.ProductId,
                        Name = schedule.Name,
                        Description = schedule.Description,
                        DayOfMonth = schedule.DayOfMonth,
                        HourOfDay = schedule.HourOfDay,
                        MinuteOfDay = schedule.MinuteOfDay,
                        StartDate = startDateTime,
                        EndDate = schedule.EndDate != null ? endDateTime : null,
                        Status = schedule.Status,
                      //  IsDeleted = false,
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

                if (result)
                {
                    result = false;
                    using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                    {
                        nISEntitiesDataContext.ScheduleRecords.AddRange(scheduleRecords);
                        nISEntitiesDataContext.SaveChanges();
                        result = true;
                    }
                }

                DateTime endDate = DateTime.MinValue;
                int noOfOccurance = 0;
                string updatedProductBatchName = string.Empty;
                updatedProductBatchName = scheduleRecords.Select(x => x.ProductBatchName).FirstOrDefault();
                if (result)
                {
                    IList<SystemActivityHistory> Records = new List<SystemActivityHistory>();
                    scheduleRecords.ToList().ForEach(schedulerecord =>
                    {
                        int batchIndex = 1;
                        if (schedulerecord.RecurrancePattern == ModelConstant.DOES_NOT_REPEAT)
                        {
                            this.AddDoesNotRepeatBatch(schedulerecord.StartDate ?? DateTime.Now, schedulerecord, tenantCode, userId, out endDate, out noOfOccurance);
                        }
                        else if (schedulerecord.RecurrancePattern == ModelConstant.DAILY || schedulerecord.RecurrancePattern == ModelConstant.CUSTOM_DAY)
                        {
                            this.AddDailyOccurenceScheduleBatches(schedulerecord.StartDate ?? DateTime.Now, schedulerecord, tenantCode, userId, batchIndex, out endDate, out noOfOccurance);
                        }
                        else if (schedulerecord.RecurrancePattern == ModelConstant.WEEKDAY || schedulerecord.RecurrancePattern == ModelConstant.WEEKLY || schedulerecord.RecurrancePattern == ModelConstant.CUSTOM_WEEK)
                        {
                            this.AddWeeklyOccurenceScheduleBatches(schedulerecord.StartDate ?? DateTime.Now, schedulerecord, tenantCode, userId, batchIndex, out endDate, out noOfOccurance);
                        }
                        else if (schedulerecord.RecurrancePattern == ModelConstant.MONTHLY || schedulerecord.RecurrancePattern == ModelConstant.CUSTOM_MONTH)
                        {
                            this.AddMonthlyOccurenceScheduleBatches(schedulerecord.StartDate ?? DateTime.Now, schedulerecord, tenantCode, userId, batchIndex, out endDate, out noOfOccurance);
                        }
                        else if (schedulerecord.RecurrancePattern == ModelConstant.YEARLY || schedulerecord.RecurrancePattern == ModelConstant.CUSTOM_YEAR)
                        {
                            this.AddYearlyOccurenceScheduleBatches(schedulerecord.StartDate ?? DateTime.Now, schedulerecord, tenantCode, userId, batchIndex, out endDate, out noOfOccurance);
                        }

                        Records.Add(new SystemActivityHistory()
                        {
                            Module = ModelConstant.SCHEDULE_MODEL_SECTION,
                            EntityId = schedulerecord.Id,
                            EntityName = schedulerecord.Name,
                            SubEntityId = null,
                            SubEntityName = null,
                            ActionTaken = "Add",
                            ActionTakenBy = userId,
                            ActionTakenByUserName = userFullName,
                            ActionTakenDate = DateTime.Now,
                            TenantCode = tenantCode
                        });
                    });

                    using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                    {
                        List<ScheduleRecord> updatedSchedules = nISEntitiesDataContext.ScheduleRecords.Where(x => x.ProductBatchName == updatedProductBatchName).ToList();
                        //if (endDate != DateTime.MinValue && updatedSchedules.Where(x => x.EndDate == null).Count() > 0)
                        //{
                        //    updatedSchedules.ForEach(item =>
                        //    {
                        //        item.EndDateForDisplay = endDate;
                        //        item.NoOfOccuranceForDisplay = item.NoOfOccurrences;
                        //    });
                        //}
                        //else if (noOfOccurance != 0 && updatedSchedules.Where(x => x.NoOfOccurrences == null || x.NoOfOccurrences == 0).Count() > 0)
                        //{
                        //    updatedSchedules.ForEach(item =>
                        //    {
                        //        item.NoOfOccuranceForDisplay = noOfOccurance;
                        //        item.EndDateForDisplay = item.EndDate;
                        //    });
                        //}
                        nISEntitiesDataContext.SaveChanges();
                    }

                    if (Records.Count > 0)
                    {
                        using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                        {
                            nISEntitiesDataContext.SystemActivityHistory.AddRange(Records);
                            nISEntitiesDataContext.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return result;
        }

        /// <summary>
        /// Updates the schedules with language.
        /// </summary>
        /// <param name="schedules"></param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns></returns>
        /// <exception cref="nIS.DuplicateScheduleFoundException"></exception>
        /// <exception cref="nIS.ScheduleNotFoundException"></exception>
        public bool UpdateSchedulesWithLanguage(IList<Schedule> schedules, string tenantCode)
        {
            bool result = false;
            try
            {
                var claims = ClaimsPrincipal.Current.Identities.First().Claims.ToList();
                int userId = 1;
                int.TryParse(claims?.FirstOrDefault(x => x.Type.Equals("UserId", StringComparison.OrdinalIgnoreCase)).Value, out userId);
                var userFullName = claims?.FirstOrDefault(x => x.Type.Equals("UserFullName", StringComparison.OrdinalIgnoreCase)).Value;

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

                    IList<SystemActivityHistoryRecord> Records = new List<SystemActivityHistoryRecord>();
                    schedules.ToList().ForEach(item =>
                    {
                        //DateTime startDateTime = DateTime.SpecifyKind(Convert.ToDateTime(item.StartDate), DateTimeKind.Utc);
                        //DateTime? endDateTime = DateTime.SpecifyKind(Convert.ToDateTime(item.EndDate), DateTimeKind.Utc);

                        var startDateTime = item.StartDate + TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now);
                        var endDateTime = item.EndDate + TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now);
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
                        scheduleRecord.Languages = string.Join(",", item.Languages);
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
                                this.AddDoesNotRepeatBatchWithLanguage(scheduleRecord.StartDate ?? DateTime.Now, scheduleRecord, tenantCode, userId);
                            }
                            else if (scheduleRecord.RecurrancePattern == ModelConstant.DAILY || scheduleRecord.RecurrancePattern == ModelConstant.CUSTOM_DAY)
                            {
                                this.AddDailyOccurenceScheduleBatchesWithLanguage(scheduleRecord.StartDate ?? DateTime.Now, scheduleRecord, tenantCode, userId, batchIndex);
                            }
                            else if (scheduleRecord.RecurrancePattern == ModelConstant.WEEKDAY || scheduleRecord.RecurrancePattern == ModelConstant.WEEKLY || scheduleRecord.RecurrancePattern == ModelConstant.CUSTOM_WEEK)
                            {
                                this.AddWeeklyOccurenceScheduleBatchesWithLanguage(scheduleRecord.StartDate ?? DateTime.Now, scheduleRecord, tenantCode, userId, batchIndex);
                            }
                            else if (scheduleRecord.RecurrancePattern == ModelConstant.MONTHLY || scheduleRecord.RecurrancePattern == ModelConstant.CUSTOM_MONTH)
                            {
                                this.AddMonthlyOccurenceScheduleBatchesWithLanguage(scheduleRecord.StartDate ?? DateTime.Now, scheduleRecord, tenantCode, userId, batchIndex);
                            }
                            else if (scheduleRecord.RecurrancePattern == ModelConstant.YEARLY || scheduleRecord.RecurrancePattern == ModelConstant.CUSTOM_YEAR)
                            {
                                this.AddYearlyOccurenceScheduleBatchesWithLanguage(scheduleRecord.StartDate ?? DateTime.Now, scheduleRecord, tenantCode, userId, batchIndex);
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
                                this.AddDailyOccurenceScheduleBatchesWithLanguage(newStartDate, scheduleRecord, tenantCode, userId, batchIndex);
                            }
                            else if (scheduleRecord.RecurrancePattern == ModelConstant.WEEKDAY || scheduleRecord.RecurrancePattern == ModelConstant.WEEKLY || scheduleRecord.RecurrancePattern == ModelConstant.CUSTOM_WEEK)
                            {
                                newStartDate.AddDays(1);
                                this.AddWeeklyOccurenceScheduleBatchesWithLanguage(newStartDate, scheduleRecord, tenantCode, userId, batchIndex);
                            }
                            else if (scheduleRecord.RecurrancePattern == ModelConstant.MONTHLY || scheduleRecord.RecurrancePattern == ModelConstant.CUSTOM_MONTH)
                            {
                                newStartDate.AddMonths(1);
                                this.AddMonthlyOccurenceScheduleBatchesWithLanguage(newStartDate, scheduleRecord, tenantCode, userId, batchIndex);
                            }
                            else if (scheduleRecord.RecurrancePattern == ModelConstant.YEARLY || scheduleRecord.RecurrancePattern == ModelConstant.CUSTOM_YEAR)
                            {
                                newStartDate.AddYears(1);
                                this.AddYearlyOccurenceScheduleBatchesWithLanguage(newStartDate, scheduleRecord, tenantCode, userId, batchIndex);
                            }
                        }

                        Records.Add(new SystemActivityHistoryRecord()
                        {
                            Module = ModelConstant.SCHEDULE_MODEL_SECTION,
                            EntityId = scheduleRecord.Id,
                            EntityName = scheduleRecord.Name,
                            SubEntityId = null,
                            SubEntityName = null,
                            ActionTaken = "Update",
                            ActionTakenBy = userId,
                            ActionTakenByUserName = userFullName,
                            ActionTakenDate = DateTime.Now,
                            TenantCode = tenantCode
                        });

                    });

                    if (Records.Count > 0)
                    {
                        nISEntitiesDataContext.SystemActivityHistoryRecords.AddRange(Records);
                        nISEntitiesDataContext.SaveChanges();
                    }

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
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// True, if the schedule values are deleted successfully(soft delete),
        /// otherwise false
        /// </returns>
        /// <exception cref="nIS.ScheduleNotFoundException"></exception>
        /// <exception cref="nIS.RunningScheduleRefrenceException"></exception>
        public bool DeleteSchedules(IList<Schedule> schedules, string tenantCode)
        {
            bool result = false;
            try
            {
                var claims = ClaimsPrincipal.Current.Identities.First().Claims.ToList();
                int userId = 1;
                int.TryParse(claims?.FirstOrDefault(x => x.Type.Equals("UserId", StringComparison.OrdinalIgnoreCase)).Value, out userId);
                var userFullName = claims?.FirstOrDefault(x => x.Type.Equals("UserFullName", StringComparison.OrdinalIgnoreCase)).Value;

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

                    IList<SystemActivityHistoryRecord> Records = new List<SystemActivityHistoryRecord>();
                    scheduleRecords.ToList().ForEach(item =>
                    {
                        item.IsDeleted = true;
                        item.UpdateBy = userId;
                        item.LastUpdatedDate = DateTime.Now;

                        Records.Add(new SystemActivityHistoryRecord()
                        {
                            Module = ModelConstant.SCHEDULE_MODEL_SECTION,
                            EntityId = item.Id,
                            EntityName = item.Name,
                            SubEntityId = null,
                            SubEntityName = null,
                            ActionTaken = "Delete",
                            ActionTakenBy = userId,
                            ActionTakenByUserName = userFullName,
                            ActionTakenDate = DateTime.Now,
                            TenantCode = tenantCode
                        });
                    });

                    if (Records.Count > 0)
                    {
                        nISEntitiesDataContext.SystemActivityHistoryRecords.AddRange(Records);
                    }
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
        /// <param name="scheduleSearchParameter">The schedule search parameter.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns>
        /// List of schedules
        /// </returns>
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
                        view_ScheduleRecords = nISEntitiesDataContext.View_ScheduleRecord.ToList();
                        //.OrderBy(scheduleSearchParameter.SortParameter.SortColumn + " " + scheduleSearchParameter.SortParameter.SortOrder.ToString())
                        //.Where(whereClause)
                        //.Skip((scheduleSearchParameter.PagingParameter.PageIndex - 1) * scheduleSearchParameter.PagingParameter.PageSize)
                        //.Take(scheduleSearchParameter.PagingParameter.PageSize)
                        //.ToList();
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
                            ExecutedBatchCount = scheduleRecord.ExecutedBatchCount ?? 0,
                            TenantCode = scheduleRecord.TenantCode,
                            ProductBatchId = scheduleRecord.ProductBatchId,
                            ProductId = scheduleRecord.ProductId,
                            ScheduleNameByUser = scheduleRecord.ScheduleNameByUser

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

        public IList<ScheduleListModel> GetSchedulesWithProduct(ScheduleSearchParameter scheduleSearchParameter, string tenantCode)
        {
            IList<ScheduleListModel> schedules = new List<ScheduleListModel>();
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                string whereClause = this.WhereClauseGenerator(scheduleSearchParameter, tenantCode);
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    IList<Schedule> scheduleRecords = new List<Schedule>();
                    List<ScheduleModel> result = new List<ScheduleModel>();
                    if (scheduleSearchParameter.PagingParameter.PageIndex > 0 && scheduleSearchParameter.PagingParameter.PageSize > 0)
                    {
                        var queryResult = (from spm in nISEntitiesDataContext.StatementPageMapRecords
                                           join pr in nISEntitiesDataContext.PageRecords on spm.ReferencePageId equals pr.Id
                                           join ppt in nISEntitiesDataContext.ProductPageTypeMappings on pr.PageTypeId equals ppt.PageTypeId
                                           join pro in nISEntitiesDataContext.Products on ppt.ProductId equals pro.Id
                                           join s in nISEntitiesDataContext.StatementRecords on spm.StatementId equals s.Id
                                           join sc in nISEntitiesDataContext.ScheduleRecords on s.Id equals sc.StatementId
                                           orderby scheduleSearchParameter.SortParameter.SortColumn + " " + scheduleSearchParameter.SortParameter.SortOrder.ToString()
                                           where (sc.ProductBatchName != " ")

                                           select new ScheduleModel
                                           {
                                               Identifier = sc.Id,
                                               Name = sc.Name,
                                               StatementId = sc.StatementId,
                                               Description = sc.Description,
                                               DayOfMonth = sc.DayOfMonth,
                                               HourOfDay = sc.HourOfDay,
                                               MinuteOfDay = sc.MinuteOfDay,
                                               StartDate = sc.StartDate,
                                               EndDate = sc.EndDate,
                                               Status = sc.Status,
                                               IsActive = sc.IsActive,
                                               IsDeleted = sc.IsDeleted,
                                               TenantCode = sc.TenantCode,
                                               LastUpdatedDate = sc.LastUpdatedDate,
                                               UpdateBy = sc.UpdateBy,
                                               IsExportToPDF = sc.IsExportToPDF,
                                               RecurrancePattern = sc.RecurrancePattern,
                                               RepeatEveryDayMonWeekYear = sc.RepeatEveryDayMonWeekYear,
                                               WeekDays = sc.WeekDays,
                                               IsEveryWeekDay = sc.IsEveryWeekDay,
                                               MonthOfYear = sc.MonthOfYear,
                                               IsEndsAfterNoOfOccurrences = sc.IsEndsAfterNoOfOccurrences,
                                               NoOfOccurrences = sc.NoOfOccurrences,
                                               ExecutedBatchCount = (from bm in nISEntitiesDataContext.BatchMasterRecords where bm.ScheduleId == sc.Id && bm.IsDataReady == true && bm.IsExecuted == true select bm.Id).Count(),
                                               Languages = sc.Languages,
                                               ProductBatchName = sc.ProductBatchName,
                                               ProductName = pro.Name,
                                               ProductId = pro.Id,
                                               ScheduleNameByUser = sc.ScheduleNameByUser,
                                               //EndDateForDisplay = sc.EndDateForDisplay,
                                               //NoOfOccuranceForDisplay = sc.NoOfOccuranceForDisplay,
                                               IsDataReady = (from bm in nISEntitiesDataContext.BatchMasterRecords
                                                              join sr in nISEntitiesDataContext.ScheduleRecords on bm.ScheduleId equals sr.Id
                                                              where sr.ProductBatchName == sc.ProductBatchName && bm.IsDataReady == true
                                                              select bm.IsDataReady).ToList().Where(x => x == true).Count() > 0 ? true : false,
                                               TotalBatches = nISEntitiesDataContext.BatchMasterRecords.Where(x => x.ScheduleId == sc.Id).Count(),
                                               IsDeleteButtonVisible = nISEntitiesDataContext.BatchMasterRecords.Where(x => (x.IsDataReady == true || x.IsExecuted == true) && x.ScheduleId == sc.Id).Count() == 0 ? true : false
                                           });

                        result = WhereClauseGeneratorInQuery(scheduleSearchParameter, tenantCode, queryResult).GroupBy(x => x.ProductBatchName).OrderBy(x => x.Key).Skip((scheduleSearchParameter.PagingParameter.PageIndex - 1) * scheduleSearchParameter.PagingParameter.PageSize)
                                    .Take(scheduleSearchParameter.PagingParameter.PageSize).Select(x => x.FirstOrDefault())
                                    .ToList();
                    }
                    else
                    {
                        var queryResult = (from spm in nISEntitiesDataContext.StatementPageMapRecords
                                           join pr in nISEntitiesDataContext.PageRecords on spm.ReferencePageId equals pr.Id
                                           join ppt in nISEntitiesDataContext.ProductPageTypeMappings on pr.PageTypeId equals ppt.PageTypeId
                                           join pro in nISEntitiesDataContext.Products on ppt.ProductId equals pro.Id
                                           join s in nISEntitiesDataContext.StatementRecords on spm.StatementId equals s.Id
                                           join sc in nISEntitiesDataContext.ScheduleRecords on s.Id equals sc.StatementId
                                           orderby scheduleSearchParameter.SortParameter.SortColumn + " " + scheduleSearchParameter.SortParameter.SortOrder.ToString()
                                           where (sc.ProductBatchName != " ")

                                           select new ScheduleModel
                                           {
                                               Identifier = sc.Id,
                                               Name = sc.Name,
                                               StatementId = sc.StatementId,
                                               Description = sc.Description,
                                               DayOfMonth = sc.DayOfMonth,
                                               HourOfDay = sc.HourOfDay,
                                               MinuteOfDay = sc.MinuteOfDay,
                                               StartDate = sc.StartDate,
                                               EndDate = sc.EndDate,
                                               Status = sc.Status,
                                               IsActive = sc.IsActive,
                                               IsDeleted = sc.IsDeleted,
                                               TenantCode = sc.TenantCode,
                                               LastUpdatedDate = sc.LastUpdatedDate,
                                               UpdateBy = sc.UpdateBy,
                                               IsExportToPDF = sc.IsExportToPDF,
                                               RecurrancePattern = sc.RecurrancePattern,
                                               RepeatEveryDayMonWeekYear = sc.RepeatEveryDayMonWeekYear,
                                               WeekDays = sc.WeekDays,
                                               IsEveryWeekDay = sc.IsEveryWeekDay,
                                               MonthOfYear = sc.MonthOfYear,
                                               IsEndsAfterNoOfOccurrences = sc.IsEndsAfterNoOfOccurrences,
                                               NoOfOccurrences = sc.NoOfOccurrences,
                                               ExecutedBatchCount = (from bm in nISEntitiesDataContext.BatchMasterRecords where bm.ScheduleId == sc.Id && bm.IsDataReady == true && bm.IsExecuted == true select bm.Id).Count(),
                                               Languages = sc.Languages,
                                               ProductBatchName = sc.ProductBatchName,
                                               ProductName = pro.Name,
                                               ProductId = pro.Id,
                                               ScheduleNameByUser = sc.ScheduleNameByUser,
                                               //EndDateForDisplay = sc.EndDateForDisplay,
                                               //NoOfOccuranceForDisplay = sc.NoOfOccuranceForDisplay,
                                               IsDataReady = (from bm in nISEntitiesDataContext.BatchMasterRecords
                                                              join sr in nISEntitiesDataContext.ScheduleRecords on bm.ScheduleId equals sr.Id
                                                              where sr.ProductBatchName == sc.ProductBatchName && bm.IsDataReady == true
                                                              select bm.IsDataReady).ToList().Where(x => x == true).Count() > 0 ? true : false,
                                               TotalBatches = nISEntitiesDataContext.BatchMasterRecords.Where(x => x.ScheduleId == sc.Id).Count(),
                                               IsDeleteButtonVisible = nISEntitiesDataContext.BatchMasterRecords.Where(x => (x.IsDataReady == true || x.IsExecuted == true) && x.ScheduleId == sc.Id).Count() == 0 ? true : false
                                           });

                        result = WhereClauseGeneratorInQuery(scheduleSearchParameter, tenantCode, queryResult).OrderBy(x => x.ProductBatchName).ToList();
                    }
                    if (result != null && result.Count > 0)
                    {
                        List<string> productName = new List<string>();
                        int index = 0;
                        foreach (var item in result)
                        {
                            if (!productName.Contains(item.ProductName))
                            {
                                productName.Add(item.ProductName);
                                index = productName.IndexOf(item.ProductName);
                                schedules.Add(new ScheduleListModel
                                {
                                    ProductName = item.ProductName,
                                    ProductId = item.ProductId
                                });
                            }
                            else
                            {
                                index = productName.IndexOf(item.ProductName);
                            }

                            schedules[index].ProductBatches.Add(new ScheduleModel
                            {
                                Identifier = item.Identifier,
                                Name = item.Name,
                                StatementId = item.StatementId,
                                Description = item.Description,
                                DayOfMonth = item.DayOfMonth,
                                HourOfDay = item.HourOfDay,
                                MinuteOfDay = item.MinuteOfDay,
                                StartDate = item.StartDate,
                                EndDate = item.EndDate,
                                Status = item.Status,
                                IsActive = item.IsActive,
                                IsDeleted = item.IsDeleted,
                                TenantCode = item.TenantCode,
                                LastUpdatedDate = item.LastUpdatedDate,
                                UpdateBy = item.UpdateBy,
                                IsExportToPDF = item.IsExportToPDF,
                                StatementName = String.Join(", ", (from p in nISEntitiesDataContext.StatementRecords join pm in nISEntitiesDataContext.ScheduleRecords on p.Id equals pm.StatementId where pm.ProductBatchName == item.ProductBatchName select p.Name).ToList()),
                                RecurrancePattern = item.RecurrancePattern,
                                RepeatEveryDayMonWeekYear = item.RepeatEveryDayMonWeekYear,
                                WeekDays = item.WeekDays,
                                IsEveryWeekDay = item.IsEveryWeekDay,
                                MonthOfYear = item.MonthOfYear,
                                IsEndsAfterNoOfOccurrences = item.IsEndsAfterNoOfOccurrences,
                                NoOfOccurrences = item.NoOfOccurrences,
                                ExecutedBatchCount = item.ExecutedBatchCount,
                                Languages = item.Languages,
                                ProductBatchName = item.ProductBatchName,
                                ScheduleNameByUser = item.ScheduleNameByUser,
                                EndDateForDisplay = item.EndDateForDisplay,
                                NoOfOccuranceForDisplay = item.NoOfOccuranceForDisplay,
                                IsDataReady = item.IsDataReady,
                                TotalBatches = item.TotalBatches,
                                IsDeleteButtonVisible = item.IsDeleteButtonVisible,
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return schedules;
        }

        /// <summary>
        /// Gets the schedules with language.
        /// </summary>
        /// <param name="scheduleSearchParameter">The schedule search parameter.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns></returns>
        public IList<Schedule> GetSchedulesWithLanguage(ScheduleSearchParameter scheduleSearchParameter, string tenantCode)
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
                            ExecutedBatchCount = scheduleRecord.ExecutedBatchCount ?? 0,
                            TenantCode = scheduleRecord.TenantCode,
                            Languages = scheduleRecord.Languages.Split(',').ToList(),
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
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns>
        /// Schedule count
        /// </returns>
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
        /// <returns>
        /// True if schedule activated successfully false otherwise
        /// </returns>
        /// <exception cref="nIS.ScheduleNotFoundException"></exception>
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

        private IDictionary<string, long> AddETLSchedules(ScheduleRecord schedule, string tenantCode, DateTime startDate, DateTime endDate)
        {
            IDictionary<string, long> result = new Dictionary<string, long>();
            try
            {
                var claims = ClaimsPrincipal.Current.Identities.First().Claims.ToList();
                int userId = 1;
                bool isExists = false;
                int.TryParse(claims?.FirstOrDefault(x => x.Type.Equals("UserId", StringComparison.OrdinalIgnoreCase)).Value, out userId);
                var userFullName = claims?.FirstOrDefault(x => x.Type.Equals("UserFullName", StringComparison.OrdinalIgnoreCase)).Value;

                this.SetAndValidateConnectionString(tenantCode);

                List<ETLScheduleModel> etlScheduleModel = new List<ETLScheduleModel>();
                IList<EtlSchedules> scheduleRecords = new List<EtlSchedules>();

                scheduleRecords.Add(new EtlSchedules()
                {
                    Name = schedule.Name,               //***
                    ProductId = Convert.ToInt32(schedule.ProductId),
                    ProductBatchId = (long)schedule.ProductBatchId,
                    DayOfMonth = schedule.DayOfMonth,
                    StartDate = startDate,
                    EndDate = endDate,
                    IsLastDate = false,
                    IsDeleted = false,
                    IsActive = true,
                    Status = "New",
                    UpdateBy = userId,
                    LastUpdatedDate = DateTime.UtcNow,
                    TenantCode = tenantCode,
                    HourOfDay = startDate.Hour,
                    MinuteOfDay = startDate.Minute,
                });

                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    isExists = nISEntitiesDataContext.EtlSchedules.Where(x => x.ProductBatchId == schedule.ProductBatchId).Count() > 0;

                    if (!isExists)
                    {
                        nISEntitiesDataContext.EtlSchedules.AddRange(scheduleRecords);
                        nISEntitiesDataContext.SaveChanges();
                    }
                }

                if (!isExists)
                {
                    IList<SystemActivityHistoryRecord> Records = new List<SystemActivityHistoryRecord>();
                    scheduleRecords.ToList().ForEach(schedulerecord =>
                    {
                        result.Add(schedulerecord.Name, schedulerecord.Id);
                        Records.Add(new SystemActivityHistoryRecord()
                        {
                            Module = ModelConstant.ETL_SCHEDULE_MODEL_SECTION,
                            EntityId = schedulerecord.Id,
                            EntityName = schedulerecord.Name,
                            SubEntityId = null,
                            SubEntityName = null,
                            ActionTaken = "Add",
                            ActionTakenBy = userId,
                            ActionTakenByUserName = userFullName,
                            ActionTakenDate = DateTime.Now,
                            TenantCode = tenantCode
                        });
                    });

                    if (Records.Count > 0)
                    {
                        using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                        {
                            nISEntitiesDataContext.SystemActivityHistoryRecords.AddRange(Records);
                            nISEntitiesDataContext.SaveChanges();
                        }
                    }
                }
            }
            catch
            {
                throw;
            }

            return result;
        }

        private bool AddETLBatches(long scheduleId, long productBatchId, string scheduleName, string tenantCode, List<BatchMasterRecord> batchMasterRecords)
        {
            this.SetAndValidateConnectionString(tenantCode);
            IList<EtlBatches> etlBatchesRecords = new List<EtlBatches>();
            int index = 1;
            batchMasterRecords.ForEach(item =>
            {
                etlBatchesRecords.Add(new EtlBatches()
                {
                    EtlScheduleId = scheduleId,
                    ProductBatchId = productBatchId,
                    IsExecuted = false,
                    BatchName = "ETL Batch " + index + " " + scheduleName,
                    DataExtractionDateTime = item.DataExtractionDate,
                    Status = "New",
                    TenantCode = tenantCode
                });
                index++;
            });

            using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
            {
                nISEntitiesDataContext.EtlBatches.AddRange(etlBatchesRecords);
                nISEntitiesDataContext.SaveChanges();
                return true;
            }
        }
        #endregion

        #region Deactivate Schedule

        /// <summary>
        /// This method helps to deactivate the schedule
        /// </summary>
        /// <param name="scheduleIdentifier">The schedule identifier</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// True if schedule deactivated successfully false otherwise
        /// </returns>
        /// <exception cref="nIS.ScheduleNotFoundException"></exception>
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

        #endregion

        /// <summary>
        /// This method helps to run the schedule
        /// </summary>
        /// <param name="baseURL">The base URL</param>
        /// <param name="outputLocation">The output location.</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <param name="parallelThreadCount">The parallel thread count</param>
        /// <param name="tenantConfiguration">The tenant configuration.</param>
        /// <param name="client">The client.</param>
        /// <returns>
        /// True if schedules runs successfully, false otherwise
        /// </returns>
        public bool RunSchedule(string baseURL, string outputLocation, string tenantCode, int parallelThreadCount, TenantConfiguration tenantConfiguration, Client client)
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
                                        string CommonStatementZipFilePath = this.utility.CreateAndWriteToZipFile(statementPreviewData.FileContent, fileName, schedule.Name, batchMaster.BatchName, baseURL, outputLocation, filesDict);

                                        var renderEngine = new RenderEngineRecord();
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
                                            renderEngine = nISEntitiesDataContext.RenderEngineRecords.Where(item => item.Id == 1).FirstOrDefault();
                                        }

                                        if (customerMasters.Count > 0)
                                        {
                                            var tenantEntities = this.dynamicWidgetRepository.GetTenantEntities(tenantCode);

                                            ParallelOptions parallelOptions = new ParallelOptions();
                                            parallelOptions.MaxDegreeOfParallelism = parallelThreadCount;
                                            Parallel.ForEach(customerMasters, parallelOptions, customer =>
                                            {
                                                this.CreateCustomerStatement(customer, statement, scheduleLog, statementPageContents, batchMaster, batchDetails, baseURL, tenantCode, customerMasters.Count, outputLocation, tenantConfiguration, client, tenantEntities, renderEngine);
                                            });
                                            //customerMasters.ToList().ForEach(customer =>
                                            //{
                                            //    this.CreateCustomerStatement(customer, statement, scheduleLog, statementPageContents, batchMaster, batchDetails, baseURL, tenantCode, customerMasters.Count, outputLocation, tenantConfiguration, client, tenantEntities, renderEngine);
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
                //WriteToFile(ex.Message);
                //WriteToFile(ex.InnerException.Message);
                //WriteToFile(ex.StackTrace.ToString());
                throw ex;
            }
            return scheduleRunStatus;
        }

        /// <summary>
        /// This method helps to run the schedule
        /// </summary>
        /// <param name="baseURL">The base URL</param>
        /// <param name="outputLocation">The output location.</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <param name="parallelThreadCount">The parallel thread count</param>
        /// <param name="tenantConfiguration">The tenant configuration.</param>
        /// <param name="client">The client.</param>
        /// <returns>
        /// True if schedules runs successfully, false otherwise
        /// </returns>
        public bool RunScheduleNew(string baseURL, string outputLocation, string tenantCode, int parallelThreadCount, TenantConfiguration tenantConfiguration, Client client)
        {
            bool scheduleRunStatus = false;
            var schedules = new List<ScheduleRecord>();
            var batchMasterRecords = new List<BatchMasterRecord>();

            var fromdate = DateTime.Now;
            var todate = fromdate.AddMinutes(60);

            try
            {
                var query = new StringBuilder();
                this.SetAndValidateConnectionString(tenantCode);
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    query.Append("BatchExecutionDate >= DateTime(" + fromdate.Year + "," + fromdate.Month + "," + fromdate.Day + "," + fromdate.Hour + "," + fromdate.Minute + "," + fromdate.Second + ") and BatchExecutionDate <= DateTime(" + +todate.Year + "," + todate.Month + "," + todate.Day + "," + todate.Hour + "," + todate.Minute + "," + todate.Second + ") and IsExecuted.Equals(false) ");
                    query.Append(string.Format(" and Status.Equals(\"{0}\") ", BatchStatus.New.ToString()));
                    batchMasterRecords = nISEntitiesDataContext.BatchMasterRecords.Where(query.ToString()).ToList();
                }
                if (batchMasterRecords.Count > 0)
                {
                    using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                    {
                        query = new StringBuilder();
                        query.Append("(" + string.Join("or ", string.Join(",", batchMasterRecords.Select(item => item.ScheduleId).Distinct()).ToString().Split(',').Select(item => string.Format("Id.Equals({0}) ", item))) + ") ");
                        schedules = nISEntitiesDataContext.ScheduleRecords.Where(query.ToString()).Select(item => item).AsQueryable().ToList();
                    }

                    if (schedules != null && schedules.Count > 0)
                    {
                        var batchMaster = new BatchMasterRecord();
                        var batchDetails = new List<BatchDetailRecord>();
                        schedules.ToList().ForEach(schedule =>
                        {
                            tenantCode = schedule.TenantCode;

                            //If it is on-premise deployment than schedule run api will called with respective tenant code.. So in this case tenant configuration will fetch here itself
                            //If it is cloud based then, schedule run api will be called with default tenant code.. So in this case tenant configuration will be replace in repository level
                            if (tenantConfiguration.TenantCode == string.Empty || tenantConfiguration.TenantCode.Equals(ModelConstant.DEFAULT_TENANT_CODE))
                            {
                                //To set HTML statement output path as per tenant configuration
                                tenantConfiguration = this.tenantConfigurationRepository.GetTenantConfigurations(tenantCode).FirstOrDefault();
                                if (tenantConfiguration != null && !string.IsNullOrEmpty(tenantConfiguration.OutputHTMLPath))
                                {
                                    baseURL = tenantConfiguration.OutputHTMLPath;
                                    outputLocation = tenantConfiguration.OutputHTMLPath;
                                }
                            }

                            ScheduleLogRecord scheduleLog = new ScheduleLogRecord();

                            using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                            {
                                batchMaster = nISEntitiesDataContext.BatchMasterRecords.Where(item => item.ScheduleId == schedule.Id && !item.IsExecuted && item.Status == BatchStatus.New.ToString() && item.TenantCode == tenantCode)?.ToList()?.FirstOrDefault();

                                scheduleLog.ScheduleId = schedule.Id;
                                scheduleLog.ScheduleName = schedule.Name;
                                scheduleLog.BatchId = batchMaster.Id;
                                scheduleLog.BatchName = batchMaster.BatchName;
                                scheduleLog.NumberOfRetry = 1;
                                scheduleLog.CreationDate = DateTime.UtcNow;
                                scheduleLog.TenantCode = tenantCode;

                                schedule.Status = ScheduleStatus.InProgress.ToString();
                                nISEntitiesDataContext.SaveChanges();
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
                                            var renderEngine = new RenderEngineRecord();
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
                                            string CommonStatementZipFilePath = this.utility.CreateAndWriteToZipFile(statementPreviewData.FileContent, fileName, schedule.Name, batchMaster.BatchName, baseURL, outputLocation, filesDict);

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
                                                renderEngine = nISEntitiesDataContext.RenderEngineRecords.Where(item => item.Id == 1).FirstOrDefault();
                                            }

                                            if (customerMasters.Count > 0)
                                            {
                                                var tenantEntities = this.dynamicWidgetRepository.GetTenantEntities(tenantCode);

                                                ParallelOptions parallelOptions = new ParallelOptions();
                                                parallelOptions.MaxDegreeOfParallelism = parallelThreadCount;
                                                Parallel.ForEach(customerMasters, parallelOptions, customer =>
                                                {
                                                    this.CreateCustomerStatement(customer, statement, scheduleLog, statementPageContents, batchMaster, batchDetails, baseURL, tenantCode, customerMasters.Count, outputLocation, tenantConfiguration, client, tenantEntities, renderEngine);
                                                });
                                                //customerMasters.ToList().ForEach(customer =>
                                                //{
                                                //    this.CreateCustomerStatement(customer, statement, scheduleLog, statementPageContents, batchMaster, batchDetails, baseURL, tenantCode, customerMasters.Count, outputLocation, tenantConfiguration, client, tenantEntities, renderEngine);
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
                                    else
                                    {
                                        throw new StatementNotFoundException(tenantCode);
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
        /// <param name="outputLocation">The output location.</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <param name="parallelThreadCount">The parallel thread count</param>
        /// <param name="tenantConfiguration">The tenant configuration.</param>
        /// <param name="client">The client.</param>
        /// <returns>
        /// True if schedules runs successfully, false otherwise
        /// </returns>
        /// <exception cref="nIS.ScheduleNotFoundException"></exception>
        /// <exception cref="nIS.StatementNotFoundException"></exception>
        public bool RunScheduleNow(BatchMaster batch, string baseURL, string outputLocation, string tenantCode, int parallelThreadCount, TenantConfiguration tenantConfiguration, Client client)
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

                    batchMaster = nISEntitiesDataContext.BatchMasterRecords.Where(item => item.Id == batch.Identifier && item.ScheduleId == scheduleRecord.Id && !item.IsExecuted && item.Status == BatchStatus.New.ToString() && item.TenantCode == tenantCode)?.ToList().FirstOrDefault();

                    scheduleLog.ScheduleId = scheduleRecord.Id;
                    scheduleLog.ScheduleName = scheduleRecord.Name;
                    scheduleLog.BatchId = batchMaster.Id;
                    scheduleLog.BatchName = batchMaster.BatchName;
                    scheduleLog.NumberOfRetry = 1;
                    scheduleLog.CreationDate = DateTime.UtcNow;
                    scheduleLog.TenantCode = tenantCode;

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
                    string CommonStatementZipFilePath = this.utility.CreateAndWriteToZipFile(statementPreviewData.FileContent, fileName, scheduleRecord.Name, batchMaster.BatchName, baseURL, outputLocation, filesDict);

                    IList<CustomerMasterRecord> customerMasters = new List<CustomerMasterRecord>();
                    IList<BatchDetailRecord> batchDetails = new List<BatchDetailRecord>();
                    var renderEngine = new RenderEngineRecord();
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
                        renderEngine = nISEntitiesDataContext.RenderEngineRecords.Where(item => item.Id == 1).FirstOrDefault();
                    }

                    if (customerMasters.Count > 0)
                    {
                        var tenantEntities = this.dynamicWidgetRepository.GetTenantEntities(tenantCode);

                        ParallelOptions parallelOptions = new ParallelOptions();
                        parallelOptions.MaxDegreeOfParallelism = parallelThreadCount;
                        Parallel.ForEach(customerMasters, parallelOptions, customer =>
                        {
                            this.CreateCustomerStatement(customer, statement, scheduleLog, statementPageContents, batchMaster, batchDetails, baseURL, tenantCode, customerMasters.Count, outputLocation, tenantConfiguration, client, tenantEntities, renderEngine);
                        });
                        //customerMasters.ForEach(customer =>
                        //{
                        //    this.CreateCustomerStatement(customer, statement, scheduleLog, statementPageContents, batchMaster, batchDetails, baseURL, tenantCode, customerMasters.Count, outputLocation, tenantConfiguration, client, tenantEntities, renderEngine);
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

        /// <summary>
        /// This method helps to update schedule status.
        /// </summary>
        /// <param name="ScheduleIdentifier">The schedule identifier.</param>
        /// <param name="Status">The status.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns>
        /// True if success, otherwise false
        /// </returns>
        public bool UpdateScheduleStatus(long ScheduleIdentifier, string Status, string tenantCode)
        {
            bool result = false;
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    nISEntitiesDataContext.ScheduleRecords.Where(item => item.Id == ScheduleIdentifier && item.TenantCode == tenantCode).ToList().ForEach(schedule =>
                    {
                        schedule.Status = Status;
                    });
                    nISEntitiesDataContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }


        #endregion

        #region ScheduleRunHistory  Run History
        /// <summary>
        /// This method adds the specified list of schedule in the repository.
        /// </summary>
        /// <param name="schedules"></param>
        /// <param name="tenantCode">The tenant code</param>
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
                        ScheduleId = schedule.ScheduleId,
                        FilePath = schedule.StatementFilePath,
                        ScheduleLogId = schedule.ScheduleLogId,
                        StatementId = schedule.StatementId,
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
        /// <param name="scheduleSearchParameter">The schedule search parameter.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns>
        /// List of schedules
        /// </returns>
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
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns>
        /// ScheduleRunHistory count
        /// </returns>
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

        /// <summary>
        /// This method helps to update schedule run history end date.
        /// </summary>
        /// <param name="ScheduleLogIdentifier">The schedule log identifier.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns>
        /// True if success, otherwise false
        /// </returns>
        public bool UpdateScheduleRunHistoryEndDate(long ScheduleLogIdentifier, string tenantCode)
        {
            bool result = false;
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    nISEntitiesDataContext.ScheduleRunHistoryRecords.Where(item => item.ScheduleLogId == ScheduleLogIdentifier && item.TenantCode == tenantCode).ToList().ForEach(schedule =>
                    {
                        schedule.EndDate = DateTime.UtcNow;
                    });
                    nISEntitiesDataContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        #endregion

        #region Batch master
        /// <summary>
        /// Adds the batch master.
        /// </summary>
        /// <param name="schedule">The schedule.</param>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
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
        /// Gets the batch masters.
        /// </summary>
        /// <param name="schdeuleIdentifier">The schdeule identifier.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns></returns>
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

        public IList<BatchMaster> GetBatchMastersById(long identifier, string tenantCode)
        {
            IList<BatchMaster> batchMasters = new List<BatchMaster>();
            IList<BatchMasterRecord> batchMasterRecords = new List<BatchMasterRecord>();

            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    batchMasterRecords = nISEntitiesDataContext.BatchMasterRecords.Where(item => item.Id == identifier && item.TenantCode == tenantCode).ToList();
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
                throw;
            }
            return batchMasters;
        }

        /// <summary>
        /// Gets the batch masters by language.
        /// </summary>
        /// <param name="schdeuleIdentifier">The schdeule identifier.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns></returns>
        public IList<BatchMaster> GetBatchMastersByLanguage(long schdeuleIdentifier, string tenantCode)
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
                                LanguageCode = item.LanguageCode
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

        /// <summary>
        /// This method helps to get batch list by search parameter.
        /// </summary>
        /// <param name="batchSearchParameter">The batch search parameter</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns>
        /// return list of batches
        /// </returns>
        public IList<BatchMaster> GetBatches(BatchSearchParameter batchSearchParameter, string tenantCode)
        {
            IList<BatchMaster> batches = new List<BatchMaster>();
            try
            {
                var query = new StringBuilder();
                this.SetAndValidateConnectionString(tenantCode);
                var whereClause = this.WhereClauseGeneratorBatchMaster(batchSearchParameter, tenantCode);
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    var batchMasterRecords = nISEntitiesDataContext.BatchMasterRecords.Where(whereClause).ToList();
                    batchMasterRecords.ForEach(batch =>
                    {
                        batches.Add(new BatchMaster()
                        {
                            Identifier = batch.Id,
                            BatchName = batch.BatchName,
                            BatchExecutionDate = batch.BatchExecutionDate,
                            CreatedBy = batch.CreatedBy,
                            CreatedDate = batch.CreatedDate,
                            DataExtractionDate = batch.DataExtractionDate,
                            IsDataReady = batch.IsDataReady,
                            IsExecuted = batch.IsExecuted,
                            ScheduleId = batch.ScheduleId,
                            Status = batch.Status,
                            TenantCode = batch.TenantCode
                        });
                    });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return batches;
        }


        public IList<BatchMaster> GetBatchMastersByProductBatchName(string productBatchName, string tenantCode)
        {
            IList<BatchMaster> batchMasters = new List<BatchMaster>();
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    var batchesList = (from spm in nISEntitiesDataContext.StatementPageMapRecords
                                       join pr in nISEntitiesDataContext.PageRecords on spm.ReferencePageId equals pr.Id
                                       join ppt in nISEntitiesDataContext.ProductPageTypeMappings on pr.PageTypeId equals ppt.PageTypeId
                                       join pro in nISEntitiesDataContext.Products on ppt.ProductId equals pro.Id
                                       join s in nISEntitiesDataContext.StatementRecords on spm.StatementId equals s.Id
                                       join sr in nISEntitiesDataContext.ScheduleRecords on s.Id equals sr.StatementId
                                       join bm in nISEntitiesDataContext.BatchMasterRecords on sr.Id equals bm.ScheduleId
                                       where (sr.ProductBatchName == productBatchName)

                                       select new BatchMaster
                                       {
                                           Identifier = bm.Id,
                                           BatchName = bm.BatchName,
                                           TenantCode = bm.TenantCode == string.Empty ? tenantCode : bm.TenantCode,
                                           CreatedBy = bm.CreatedBy,
                                           CreatedDate = bm.CreatedDate,
                                           ScheduleId = bm.ScheduleId,
                                           IsExecuted = bm.IsExecuted,
                                           IsDataReady = bm.IsDataReady,
                                           BatchExecutionDate = bm.BatchExecutionDate,
                                           DataExtractionDate = bm.DataExtractionDate,
                                           Status = bm.Status,
                                           ProductBatchName = pro.Name
                                       }).ToList();

                    long scheduleId = 0;
                    foreach (var item in batchesList.Select((value, index) => new { index, value }))
                    {
                        if (scheduleId == 0 || scheduleId == item.value.ScheduleId)
                        {
                            var status = string.Empty;
                            if (batchesList.Where(x => x.BatchExecutionDate == item.value.BatchExecutionDate).ToList().Any(x => x.Status == BatchStatus.Running.ToString()))
                            {
                                status = BatchStatus.Running.ToString();
                            }
                            else if (batchesList.Where(x => x.BatchExecutionDate == item.value.BatchExecutionDate).ToList().Any(x => x.Status == BatchStatus.Completed.ToString()))
                            {
                                status = BatchStatus.Completed.ToString();
                            }
                            else if (batchesList.Where(x => x.BatchExecutionDate == item.value.BatchExecutionDate).ToList().All(x => x.Status == BatchStatus.Failed.ToString()))
                            {
                                status = BatchStatus.Failed.ToString();
                            }
                            else if (batchesList.Where(x => x.BatchExecutionDate == item.value.BatchExecutionDate).ToList().All(x => x.Status == BatchStatus.BatchDataNotAvailable.ToString()))
                            {
                                status = BatchStatus.BatchDataNotAvailable.ToString();
                            }
                            else
                            {
                                status = item.value.Status;
                            }

                            batchMasters.Add(new BatchMaster
                            {
                                Ids = string.Join(",", batchesList.Where(x => x.BatchExecutionDate == item.value.BatchExecutionDate).Select(x => x.Identifier)),
                                BatchName = "Batch " + (item.index + 1),
                                TenantCode = item.value.TenantCode,
                                CreatedBy = item.value.CreatedBy,
                                CreatedDate = item.value.CreatedDate,
                                ScheduleId = item.value.ScheduleId,
                                IsExecuted = batchesList.Where(x => x.BatchExecutionDate == item.value.BatchExecutionDate).Select(x => x.IsExecuted).Where(x => x == true).Count() > 0 ? true : false,
                                IsDataReady = batchesList.Where(x => x.BatchExecutionDate == item.value.BatchExecutionDate).Select(x => x.IsDataReady).Where(x => x == true).Count() > 0 ? true : false,
                                BatchExecutionDate = item.value.BatchExecutionDate,
                                DataExtractionDate = item.value.DataExtractionDate,
                                Status = status,
                                ProductBatchName = item.value.ProductBatchName
                            });
                            scheduleId = item.value.ScheduleId;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return batchMasters;
        }

        /// <summary>
        /// This method helps to approve batch of the respective schedule.
        /// </summary>
        /// <param name="BatchIdentifier">The batch identifier.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns>
        /// True if success, otherwise false
        /// </returns>
        /// <exception cref="nIS.TenantSecurityCodeFormatNotAvailableException"></exception>
        public bool ApproveScheduleBatch(long BatchIdentifier, string tenantCode)
        {
            List<CustomerMasterRecord> customerMasterRecords = new List<CustomerMasterRecord>();
            List<DM_CustomerMasterRecord> dm_customerMasterRecords = new List<DM_CustomerMasterRecord>();
            List<StatementMetadataRecord> statementMetadataRecords = new List<StatementMetadataRecord>();
            IList<ScheduleLogRecord> scheduleLogRecords = new List<ScheduleLogRecord>();
            IList<ScheduleLogDetailRecord> scheduleLogDetailRecords = new List<ScheduleLogDetailRecord>();
            IList<SystemActivityHistoryRecord> Records = new List<SystemActivityHistoryRecord>();

            try
            {
                var claims = ClaimsPrincipal.Current.Identities.First().Claims.ToList();
                int userId;
                int.TryParse(claims?.FirstOrDefault(x => x.Type.Equals("UserId", StringComparison.OrdinalIgnoreCase)).Value, out userId);
                var userFullName = claims?.FirstOrDefault(x => x.Type.Equals("UserFullName", StringComparison.OrdinalIgnoreCase)).Value;

                this.SetAndValidateConnectionString(tenantCode);
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    customerMasterRecords = nISEntitiesDataContext.CustomerMasterRecords.Where(item => item.BatchId == BatchIdentifier && item.TenantCode == tenantCode)?.ToList();
                    if (customerMasterRecords == null || customerMasterRecords.Count == 0)
                    {
                        dm_customerMasterRecords = nISEntitiesDataContext.DM_CustomerMasterRecord.Where(it => it.BatchId == BatchIdentifier && it.TenantCode == tenantCode)?.ToList();
                    }
                    scheduleLogRecords = nISEntitiesDataContext.ScheduleLogRecords.Where(item => item.BatchId == BatchIdentifier).ToList();
                }
                StringBuilder query = new StringBuilder();
                if (scheduleLogRecords?.Count > 0)
                {
                    query = query.Append("(" + string.Join("or ", scheduleLogRecords.Select(item => string.Format("ScheduleLogId.Equals({0}) ", item.Id))) + ") and ");
                    query.Append(string.Format(" TenantCode.Equals(\"{0}\") ", tenantCode));
                    using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                    {
                        statementMetadataRecords = nISEntitiesDataContext.StatementMetadataRecords.Where(query.ToString()).ToList();
                    }
                }
                if (statementMetadataRecords?.Count > 0)
                {
                    TenantSecurityCodeFormatRecord tenantSecurityCodeFormatRecord;
                    using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                    {
                        tenantSecurityCodeFormatRecord = nISEntitiesDataContext.TenantSecurityCodeFormatRecords.Where(item => item.TenantCode == tenantCode).ToList().FirstOrDefault();
                    }
                    if (tenantSecurityCodeFormatRecord == null)
                    {
                        throw new TenantSecurityCodeFormatNotAvailableException(tenantCode);
                    }
                    List<string> fields = tenantSecurityCodeFormatRecord.Format.Split('<').ToList();
                    fields.RemoveAt(0);
                    for (int i = 0; i < fields.Count; i++)
                    {
                        fields[i] = fields[i].Remove(fields[i].Length - 1);
                    }
                    IList<StatementMetadataRecord> newStatementMetadataRecords = new List<StatementMetadataRecord>();
                    if (customerMasterRecords != null && customerMasterRecords.Count > 0)
                    {
                        customerMasterRecords.ToList().ForEach(item =>
                        {
                            string password = string.Empty;
                            JObject customerDetails = JObject.FromObject(item);
                            int count = 0;

                            fields.ToList().ForEach(field =>
                            {
                                string fieldValue = string.Empty;
                                List<string> fieldDetail = field.Split(':').ToList();
                                if (customerDetails[fieldDetail[0]].ToString() == "")
                                {
                                    throw new TenantSecurityCodeFieldDataNotAvailable(tenantCode);
                                }
                                if (fieldDetail.Count == 1)
                                {
                                    fieldValue = customerDetails[fieldDetail[0]].ToString();
                                }
                                else if (fieldDetail.Count == 3)
                                {
                                    fieldValue = fieldDetail[0];
                                    if (fieldDetail[2] == "F")
                                    {
                                        count = Convert.ToInt32(fieldDetail[1]);
                                        fieldValue = customerDetails[fieldDetail[0]].ToString().Substring(0, count);
                                    }
                                    else if (fieldDetail[2] == "L")
                                    {
                                        count = Convert.ToInt32(fieldDetail[1]);
                                        int length = customerDetails[fieldDetail[0]].ToString().Length;
                                        fieldValue = customerDetails[fieldDetail[0]].ToString().Substring(length - count, count);
                                    }
                                }
                                password = password + fieldValue;
                            });
                            statementMetadataRecords.Where(stmt => stmt.CustomerId == item.Id).ToList().ForEach(st =>
                            {
                                StatementMetadataRecord statement = new StatementMetadataRecord();
                                statement = st;
                                statement.Password = this.cryptoManager.Encrypt(password);
                                newStatementMetadataRecords.Add(statement);
                            });
                        });

                        if (newStatementMetadataRecords?.Count > 0)
                        {
                            IList<StatementMetadataRecord> statementToBeUpdate = new List<StatementMetadataRecord>();
                            query = new StringBuilder();
                            query = query.Append("(" + string.Join("or ", newStatementMetadataRecords.Select(item => string.Format("Id.Equals({0}) ", item.Id))) + ") ");
                            using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                            {
                                statementToBeUpdate = nISEntitiesDataContext.StatementMetadataRecords.Where(query.ToString()).ToList();
                                statementToBeUpdate.ToList().ForEach(item =>
                                {
                                    item.Password = newStatementMetadataRecords.Where(s => s.Id == item.Id).FirstOrDefault().Password;
                                    item.IsPasswordGenerated = true;
                                });
                                nISEntitiesDataContext.SaveChanges();
                            }
                        }
                    }
                    else if (dm_customerMasterRecords != null && dm_customerMasterRecords.Count > 0)
                    {
                        dm_customerMasterRecords.ToList().ForEach(item =>
                        {
                            string password = string.Empty;
                            JObject customerDetails = JObject.FromObject(item);
                            int count = 0;

                            fields.ToList().ForEach(field =>
                            {
                                string fieldValue = string.Empty;
                                List<string> fieldDetail = field.Split(':').ToList();
                                if (customerDetails[fieldDetail[0]].ToString() == "")
                                {
                                    throw new TenantSecurityCodeFieldDataNotAvailable(tenantCode);
                                }
                                if (fieldDetail.Count == 1)
                                {
                                    fieldValue = customerDetails[fieldDetail[0]].ToString();
                                }
                                else if (fieldDetail.Count == 3)
                                {
                                    fieldValue = fieldDetail[0];
                                    if (fieldDetail[2] == "F")
                                    {
                                        count = Convert.ToInt32(fieldDetail[1]);
                                        fieldValue = customerDetails[fieldDetail[0]].ToString().Substring(0, count);
                                    }
                                    else if (fieldDetail[2] == "L")
                                    {
                                        count = Convert.ToInt32(fieldDetail[1]);
                                        int length = customerDetails[fieldDetail[0]].ToString().Length;
                                        fieldValue = customerDetails[fieldDetail[0]].ToString().Substring(length - count, count);
                                    }
                                }
                                password = password + fieldValue;
                            });
                            statementMetadataRecords.Where(stmt => stmt.CustomerId == item.Id).ToList().ForEach(st =>
                            {
                                StatementMetadataRecord statement = new StatementMetadataRecord();
                                statement = st;
                                statement.Password = this.cryptoManager.Encrypt(password);
                                newStatementMetadataRecords.Add(statement);
                            });
                        });

                        if (newStatementMetadataRecords?.Count > 0)
                        {
                            IList<StatementMetadataRecord> statementToBeUpdate = new List<StatementMetadataRecord>();
                            query = new StringBuilder();
                            query = query.Append("(" + string.Join("or ", newStatementMetadataRecords.Select(item => string.Format("Id.Equals({0}) ", item.Id))) + ") ");
                            using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                            {
                                statementToBeUpdate = nISEntitiesDataContext.StatementMetadataRecords.Where(query.ToString()).ToList();
                                statementToBeUpdate.ToList().ForEach(item =>
                                {
                                    item.Password = newStatementMetadataRecords.Where(s => s.Id == item.Id).FirstOrDefault().Password;
                                    item.IsPasswordGenerated = true;
                                });
                                nISEntitiesDataContext.SaveChanges();
                            }
                        }
                    }
                }

                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    var batchs = nISEntitiesDataContext.BatchMasterRecords.Where(item => item.Id == BatchIdentifier && item.TenantCode == tenantCode).ToList();
                    batchs.ForEach(batch =>
                    {
                        batch.Status = BatchStatus.Approved.ToString();

                        Records.Add(new SystemActivityHistoryRecord()
                        {
                            Module = ModelConstant.SCHEDULE_MODEL_SECTION,
                            EntityId = batch.ScheduleId,
                            EntityName = (scheduleLogRecords != null && scheduleLogRecords.Count > 0) ? scheduleLogRecords[0].ScheduleName : string.Empty,
                            SubEntityId = batch.Id,
                            SubEntityName = batch.BatchName,
                            ActionTaken = "ApproveBatch",
                            ActionTakenBy = userId,
                            ActionTakenByUserName = userFullName,
                            ActionTakenDate = DateTime.Now,
                            TenantCode = tenantCode
                        });
                    });

                    if (Records.Count > 0)
                    {
                        nISEntitiesDataContext.SystemActivityHistoryRecords.AddRange(Records);
                    }

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
        /// This method helps to approve batch of the respective schedule.
        /// </summary>
        /// <param name="BatchIdentifier">The batch identifier.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns>
        /// True if success, otherwise false
        /// </returns>
        /// <exception cref="nIS.TenantSecurityCodeFormatNotAvailableException"></exception>
        public bool ValidateApproveScheduleBatch(long BatchIdentifier, string tenantCode)
        {
            List<CustomerMasterRecord> customerMasterRecords = new List<CustomerMasterRecord>();
            List<DM_CustomerMasterRecord> dm_customerMasterRecords = new List<DM_CustomerMasterRecord>();
            List<StatementMetadataRecord> statementMetadataRecords = new List<StatementMetadataRecord>();
            IList<ScheduleLogRecord> scheduleLogRecords = new List<ScheduleLogRecord>();
            IList<ScheduleLogDetailRecord> scheduleLogDetailRecords = new List<ScheduleLogDetailRecord>();

            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    customerMasterRecords = nISEntitiesDataContext.CustomerMasterRecords.Where(item => item.BatchId == BatchIdentifier && item.TenantCode == tenantCode)?.ToList();
                    if (customerMasterRecords == null || customerMasterRecords.Count == 0)
                    {
                        dm_customerMasterRecords = nISEntitiesDataContext.DM_CustomerMasterRecord.Where(it => it.BatchId == BatchIdentifier && it.TenantCode == tenantCode)?.ToList();
                    }
                    scheduleLogRecords = nISEntitiesDataContext.ScheduleLogRecords.Where(item => item.BatchId == BatchIdentifier).ToList();
                }
                StringBuilder query = new StringBuilder();
                if (scheduleLogRecords?.Count > 0)
                {
                    query = query.Append("(" + string.Join("or ", scheduleLogRecords.Select(item => string.Format("ScheduleLogId.Equals({0}) ", item.Id))) + ") and ");
                    query.Append(string.Format(" TenantCode.Equals(\"{0}\") ", tenantCode));
                    using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                    {
                        statementMetadataRecords = nISEntitiesDataContext.StatementMetadataRecords.Where(query.ToString()).ToList();
                    }
                }
                if (statementMetadataRecords?.Count > 0)
                {
                    TenantSecurityCodeFormatRecord tenantSecurityCodeFormatRecord;
                    using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                    {
                        tenantSecurityCodeFormatRecord = nISEntitiesDataContext.TenantSecurityCodeFormatRecords.Where(item => item.TenantCode == tenantCode).ToList().FirstOrDefault();
                    }
                    if (tenantSecurityCodeFormatRecord == null)
                    {
                        throw new TenantSecurityCodeFormatNotAvailableException(tenantCode);
                    }
                    List<string> fields = tenantSecurityCodeFormatRecord.Format.Split('<').ToList();
                    fields.RemoveAt(0);
                    for (int i = 0; i < fields.Count; i++)
                    {
                        fields[i] = fields[i].Remove(fields[i].Length - 1);
                    }
                    IList<StatementMetadataRecord> newStatementMetadataRecords = new List<StatementMetadataRecord>();
                    if (customerMasterRecords != null && customerMasterRecords.Count > 0)
                    {
                        customerMasterRecords.ToList().ForEach(item =>
                        {
                            JObject customerDetails = JObject.FromObject(item);
                            fields.ToList().ForEach(field =>
                            {
                                string fieldValue = string.Empty;
                                List<string> fieldDetail = field.Split(':').ToList();
                                if (customerDetails[fieldDetail[0]].ToString() == "")
                                {
                                    throw new TenantSecurityCodeFieldDataNotAvailable(tenantCode);
                                }
                            });
                        });
                    }
                    else if (dm_customerMasterRecords != null && dm_customerMasterRecords.Count > 0)
                    {
                        dm_customerMasterRecords.ToList().ForEach(item =>
                        {
                            JObject customerDetails = JObject.FromObject(item);
                            fields.ToList().ForEach(field =>
                            {
                                string fieldValue = string.Empty;
                                List<string> fieldDetail = field.Split(':').ToList();
                                if (customerDetails[fieldDetail[0]].ToString() == "")
                                {
                                    throw new TenantSecurityCodeFieldDataNotAvailable(tenantCode);
                                }
                            });
                        });
                    }
                }
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    var batchs = nISEntitiesDataContext.BatchMasterRecords.Where(item => item.Id == BatchIdentifier && item.TenantCode == tenantCode).ToList();
                    batchs.ForEach(batch =>
                    {
                        batch.Status = BatchStatus.ApprovalInProgress.ToString();
                    });

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
        /// This method helps to clean batch and related data of the respective schedule.
        /// </summary>
        /// <param name="BatchIdentifier">The batch identifier.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns>
        /// True if success, otherwise false
        /// </returns>
        public bool CleanScheduleBatch(long BatchIdentifier, string tenantCode)
        {
            bool result = false;
            var HtmlFilePath = string.Empty;
            long BatchId = 0;
            IList<SystemActivityHistoryRecord> Records = new List<SystemActivityHistoryRecord>();

            try
            {
                var claims = ClaimsPrincipal.Current.Identities.First().Claims.ToList();
                int userId;
                int.TryParse(claims?.FirstOrDefault(x => x.Type.Equals("UserId", StringComparison.OrdinalIgnoreCase)).Value, out userId);
                var userFullName = claims?.FirstOrDefault(x => x.Type.Equals("UserFullName", StringComparison.OrdinalIgnoreCase)).Value;

                this.SetAndValidateConnectionString(tenantCode);
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    var batch = nISEntitiesDataContext.BatchMasterRecords.Where(item => item.Id == BatchIdentifier && item.TenantCode == tenantCode)?.ToList().FirstOrDefault();
                    if (batch != null)
                    {
                        BatchId = batch.Id;

                        //update batch status as NEW and IsExecuted flag to false to available it for re-run
                        batch.Status = BatchStatus.New.ToString();
                        batch.IsExecuted = false;

                        //get schedule log
                        var scheduleLogs = nISEntitiesDataContext.ScheduleLogRecords.Where(item => item.ScheduleId == batch.ScheduleId && item.BatchId == batch.Id && item.TenantCode == tenantCode).ToList();
                        scheduleLogs.ForEach(log =>
                        {

                            //get and delete schedule log details
                            var schedulelogdetails = nISEntitiesDataContext.ScheduleLogDetailRecords.Where(item => item.ScheduleId == batch.ScheduleId && item.ScheduleLogId == log.Id && item.TenantCode == tenantCode).ToList();
                            nISEntitiesDataContext.ScheduleLogDetailRecords.RemoveRange(schedulelogdetails);

                            //get and delete schedule run history
                            var scheduleRunHistories = nISEntitiesDataContext.ScheduleRunHistoryRecords.Where(item => item.ScheduleId == batch.ScheduleId && item.ScheduleLogId == log.Id && item.TenantCode == tenantCode).ToList();
                            if (scheduleRunHistories.Count > 0)
                            {
                                HtmlFilePath = scheduleRunHistories[0].FilePath;
                            }

                            nISEntitiesDataContext.ScheduleRunHistoryRecords.RemoveRange(scheduleRunHistories);

                            //get and delete statement metadata
                            var statementMetadatas = nISEntitiesDataContext.StatementMetadataRecords.Where(item => item.ScheduleId == batch.ScheduleId && item.ScheduleLogId == log.Id && item.TenantCode == tenantCode).ToList();
                            nISEntitiesDataContext.StatementMetadataRecords.RemoveRange(statementMetadatas);

                            Records.Add(new SystemActivityHistoryRecord()
                            {
                                Module = ModelConstant.SCHEDULE_MODEL_SECTION,
                                EntityId = batch.ScheduleId,
                                EntityName = log.ScheduleName,
                                SubEntityId = batch.Id,
                                SubEntityName = batch.BatchName,
                                ActionTaken = "CleanBatch",
                                ActionTakenBy = userId,
                                ActionTakenByUserName = userFullName,
                                ActionTakenDate = DateTime.Now,
                                TenantCode = tenantCode
                            });
                        });

                        //delete schedule log
                        nISEntitiesDataContext.ScheduleLogRecords.RemoveRange(scheduleLogs);

                        if (Records.Count > 0)
                        {
                            nISEntitiesDataContext.SystemActivityHistoryRecords.AddRange(Records);
                        }

                        //to save all above delete records in database
                        nISEntitiesDataContext.SaveChanges();
                        result = true;
                    }
                }

                if (result && BatchId != 0 && HtmlFilePath != string.Empty)
                {
                    //get HTML statements and other related files directory path
                    var filePathArr = HtmlFilePath.Split('\\').ToList();
                    var HtmlFilesDir = new StringBuilder();
                    for (int i = 0; i < filePathArr.Count; i++)
                    {
                        HtmlFilesDir = HtmlFilesDir.Append((HtmlFilesDir.ToString() != string.Empty ? "\\" : "") + filePathArr[i]);
                        if (filePathArr[i].Equals("" + BatchId))
                        {
                            break;
                        }
                    }

                    //delete all HTML statements and other related files directory from pyshical path
                    DirectoryInfo directoryInfo = new DirectoryInfo(HtmlFilesDir.ToString());
                    if (directoryInfo.Exists)
                    {
                        directoryInfo.Delete(true);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        /// <summary>
        /// This method helps to update batch status.
        /// </summary>
        /// <param name="BatchIdentifier">The batch identifier.</param>
        /// <param name="Status">The status.</param>
        /// <param name="IsExecuted">if set to <c>true</c> [is executed].</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns>
        /// True if success, otherwise false
        /// </returns>
        public bool UpdateBatchStatus(long BatchIdentifier, string Status, bool IsExecuted, string tenantCode)
        {
            bool result = false;
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    nISEntitiesDataContext.BatchMasterRecords.Where(item => item.Id == BatchIdentifier && item.TenantCode == tenantCode).ToList().ForEach(batch =>
                    {
                        batch.Status = Status;
                        batch.IsExecuted = IsExecuted;
                    });
                    nISEntitiesDataContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        /// <summary>
        /// this method get visibility of delete button.
        /// </summary>
        /// <param name="scheduleIdentifier">the schedule identifier.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns>True if visible, otherwise false</returns>
        public bool GetDeleteButtonVisibility(long scheduleIdentifier, string tenantCode)
        {
            bool result = false;
            this.SetAndValidateConnectionString(tenantCode);
            using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
            {
                result = nISEntitiesDataContext.BatchMasterRecords
                         .Where(x => (x.IsDataReady == true || x.IsExecuted == true) && x.ScheduleId == scheduleIdentifier)
                         .Count() == 0 ? true : false;
            }
            return result;
        }

        #endregion

        #endregion

        #region Private Methods

        /// <summary>
        /// Generate string for dynamic linq.
        /// </summary>
        /// <param name="searchParameter">Schedule search Parameters</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns>
        /// Returns a string.
        /// </returns>
        private IQueryable<ScheduleModel> WhereClauseGeneratorInQuery(ScheduleSearchParameter searchParameter, string tenantCode, IQueryable<ScheduleModel> schedule)
        {
            if (searchParameter.SearchMode == SearchMode.Equals)
            {
                if (validationEngine.IsValidText(searchParameter.ProductBatchName))
                {
                    schedule = schedule.Where(x => x.ProductBatchName == searchParameter.ProductBatchName);
                }
                if (validationEngine.IsValidText(searchParameter.Identifier))
                {
                    var ids = searchParameter.Identifier.ToString().Split(',').Select(x => x).ToList();
                    schedule = schedule.Where(x => ids.All(y => y == x.Identifier.ToString()));
                }
                if (validationEngine.IsValidText(searchParameter.Name))
                {
                    schedule = schedule.Where(x => x.Name == searchParameter.Name);
                }
            }
            if (searchParameter.SearchMode == SearchMode.Contains)
            {
                if (validationEngine.IsValidText(searchParameter.Name))
                {
                    schedule = schedule.Where(x => x.Name.Contains(searchParameter.Name));
                }
            }
            if (validationEngine.IsValidText(searchParameter.StatementDefinitionName))
            {
                schedule = schedule.Where(x => x.StatementName.Contains(searchParameter.StatementDefinitionName));
            }
            if (searchParameter.IsActive == null || searchParameter.IsActive == true)
            {
                schedule = schedule.Where(x => x.IsDeleted == false);
            }
            else if (searchParameter.IsActive != null && searchParameter.IsActive == false)
            {
                schedule = schedule.Where(x => x.IsDeleted == true);
            }
            else if (searchParameter.IsPublished != null && searchParameter.IsPublished == true)
            {
                schedule = schedule.Where(x => x.Status == "Published");
            }
            if (this.validationEngine.IsValidDate(searchParameter.StartDate) && !this.validationEngine.IsValidDate(searchParameter.EndDate))
            {
                schedule = schedule.Where(x => x.StartDate >= searchParameter.StartDate);
            }

            if (this.validationEngine.IsValidDate(searchParameter.EndDate) && !this.validationEngine.IsValidDate(searchParameter.StartDate))
            {
                DateTime toDateTime = DateTime.SpecifyKind(Convert.ToDateTime(searchParameter.EndDate), DateTimeKind.Utc);
                schedule = schedule.Where(x => x.EndDate <= searchParameter.EndDate);
            }

            if (this.validationEngine.IsValidDate(searchParameter.StartDate) && this.validationEngine.IsValidDate(searchParameter.EndDate))
            {
                schedule = schedule.Where(x => x.StartDate >= searchParameter.StartDate && x.EndDate <= searchParameter.EndDate);
            }

            var finalQuery = string.Empty;
            if (tenantCode != ModelConstant.DEFAULT_TENANT_CODE)
            {
                schedule = schedule.Where(x => x.TenantCode == tenantCode);
            }

            return schedule;
        }

        /// <summary>
        /// Generate string for dynamic linq.
        /// </summary>
        /// <param name="searchParameter">Schedule search Parameters</param>
        /// <param name="tenantCode">The tenant code.</param>
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

            var finalQuery = string.Empty;
            if (tenantCode != ModelConstant.DEFAULT_TENANT_CODE)
            {
                queryString.Append(string.Format("TenantCode.Equals(\"{0}\") ", tenantCode));
                finalQuery = queryString.ToString();
            }
            else
            {
                int last = queryString.ToString().LastIndexOf("and");
                finalQuery = queryString.ToString().Substring(0, last);
            }

            return finalQuery;
        }

        /// <summary>
        /// Generate string for dynamic linq.
        /// </summary>
        /// <param name="searchParameter">batch search Parameters</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns>
        /// Returns a string.
        /// </returns>
        private string WhereClauseGeneratorBatchMaster(BatchSearchParameter searchParameter, string tenantCode)
        {
            StringBuilder queryString = new StringBuilder();
            if (validationEngine.IsValidText(searchParameter.Identifier))
            {
                queryString.Append("(" + string.Join("or ", searchParameter.Identifier.ToString().Split(',').Select(item => string.Format("Id.Equals({0}) ", item))) + ") and ");
            }
            if (validationEngine.IsValidText(searchParameter.ScheduleId))
            {
                queryString.Append("(" + string.Join("or ", searchParameter.ScheduleId.ToString().Split(',').Select(item => string.Format("ScheduleId.Equals({0}) ", item))) + ") and ");
            }
            if (validationEngine.IsValidText(searchParameter.Status))
            {
                queryString.Append(string.Format("Status.Equals(\"{0}\") and ", searchParameter.Status));
            }
            if (searchParameter.IsExecuted != null)
            {
                queryString.Append(string.Format("IsExecuted.Equals({0}) and ", searchParameter.IsExecuted));
            }

            if (this.validationEngine.IsValidDate(searchParameter.FromDate) && !this.validationEngine.IsValidDate(searchParameter.ToDate))
            {
                DateTime fromDateTime = DateTime.SpecifyKind(Convert.ToDateTime(searchParameter.FromDate), DateTimeKind.Utc);
                queryString.Append("BatchExecutionDate >= DateTime(" + fromDateTime.Year + "," + fromDateTime.Month + "," + fromDateTime.Day + "," + fromDateTime.Hour + "," + fromDateTime.Minute + "," + fromDateTime.Second + ") and ");
            }

            if (this.validationEngine.IsValidDate(searchParameter.ToDate) && !this.validationEngine.IsValidDate(searchParameter.FromDate))
            {
                DateTime toDateTime = DateTime.SpecifyKind(Convert.ToDateTime(searchParameter.ToDate), DateTimeKind.Utc);
                queryString.Append("BatchExecutionDate <= DateTime(" + toDateTime.Year + "," + toDateTime.Month + "," + toDateTime.Day + "," + toDateTime.Hour + "," + toDateTime.Minute + "," + toDateTime.Second + ") and ");
            }

            if (this.validationEngine.IsValidDate(searchParameter.FromDate) && this.validationEngine.IsValidDate(searchParameter.ToDate))
            {
                DateTime fromDateTime = DateTime.SpecifyKind(Convert.ToDateTime(searchParameter.FromDate), DateTimeKind.Utc);
                DateTime toDateTime = DateTime.SpecifyKind(Convert.ToDateTime(searchParameter.ToDate), DateTimeKind.Utc);

                queryString.Append("BatchExecutionDate >= DateTime(" + fromDateTime.Year + "," + fromDateTime.Month + "," + fromDateTime.Day + "," + fromDateTime.Hour + "," + fromDateTime.Minute + "," + fromDateTime.Second + ") and BatchExecutionDate <= DateTime(" + +toDateTime.Year + "," + toDateTime.Month + "," + toDateTime.Day + "," + toDateTime.Hour + "," + toDateTime.Minute + "," + toDateTime.Second + ") and ");
            }

            var finalQuery = string.Empty;
            if (tenantCode != ModelConstant.DEFAULT_TENANT_CODE)
            {
                queryString.Append(string.Format("TenantCode.Equals(\"{0}\") ", tenantCode));
                finalQuery = queryString.ToString();
            }
            else
            {
                int last = queryString.ToString().LastIndexOf("and");
                finalQuery = queryString.ToString().Substring(0, last);
            }

            return finalQuery;
        }

        /// <summary>
        /// Generate string for dynamic linq.
        /// </summary>
        /// <param name="searchParameter">Schedule search Parameters</param>
        /// <param name="tenantCode">The tenant code.</param>
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
        /// <param name="operation">The operation.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns>
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
        /// <param name="tenantCode">The tenant code</param>
        /// <exception cref="nIS.ConnectionStringNotFoundException"></exception>
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

        /// <summary>
        /// Creates the customer statement.
        /// </summary>
        /// <param name="customer">The customer.</param>
        /// <param name="statement">The statement.</param>
        /// <param name="scheduleLog">The schedule log.</param>
        /// <param name="statementPageContents">The statement page contents.</param>
        /// <param name="batchMaster">The batch master.</param>
        /// <param name="batchDetails">The batch details.</param>
        /// <param name="baseURL">The base URL.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <param name="customerCount">The customer count.</param>
        /// <param name="outputLocation">The output location.</param>
        /// <param name="tenantConfiguration">The tenant configuration.</param>
        /// <param name="client">The client.</param>
        /// <param name="tenantEntities">The tenant entities.</param>
        /// <param name="renderEngine">The render engine.</param>
        private void CreateCustomerStatement(CustomerMasterRecord customer, Statement statement, ScheduleLogRecord scheduleLog, IList<StatementPageContent> statementPageContents, BatchMasterRecord batchMaster, IList<BatchDetailRecord> batchDetails, string baseURL, string tenantCode, int customerCount, string outputLocation, TenantConfiguration tenantConfiguration, Client client, IList<TenantEntity> tenantEntities, RenderEngineRecord renderEngine)
        {
            IList<StatementMetadataRecord> statementMetadataRecords = new List<StatementMetadataRecord>();
            try
            {
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    var logDetailRecord = this.statementRepository.GenerateStatements(customer, statement, statementPageContents, batchMaster, batchDetails, baseURL, tenantCode, outputLocation, tenantConfiguration, client, tenantEntities);
                    if (logDetailRecord != null)
                    {
                        logDetailRecord.ScheduleLogId = scheduleLog.Id;
                        logDetailRecord.CustomerId = customer.Id;
                        logDetailRecord.CustomerName = customer.FirstName.Trim() + (customer.MiddleName == string.Empty ? string.Empty : " " + customer.MiddleName.Trim()) + " " + customer.LastName.Trim();
                        logDetailRecord.ScheduleId = scheduleLog.ScheduleId;
                        logDetailRecord.RenderEngineId = renderEngine != null ? renderEngine.Id : 0; //To be change once render engine implmentation start
                        logDetailRecord.RenderEngineName = renderEngine != null ? renderEngine.Name : "";
                        logDetailRecord.RenderEngineURL = renderEngine != null ? renderEngine.URL : "";
                        logDetailRecord.NumberOfRetry = 1;
                        logDetailRecord.CreationDate = DateTime.UtcNow;
                        logDetailRecord.TenantCode = tenantCode;
                        nISEntitiesDataContext.ScheduleLogDetailRecords.Add(logDetailRecord);

                        //if statement generated successfully, then save statement metadata with actual html statement file path
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
                            }
                        }
                        nISEntitiesDataContext.SaveChanges();

                        //If any error occurs during statement generation then delete all files from output directory of current customer
                        if (logDetailRecord.Status.ToLower().Equals(ScheduleLogStatus.Failed.ToString().ToLower()))
                        {
                            this.utility.DeleteUnwantedDirectory(batchMaster.Id, customer.Id, outputLocation);
                        }
                    }

                    //update status for respective schedule log, schedule log details entities
                    //as well as update batch status if statement generation done for all customers of current batch
                    var logDetailsRecords = nISEntitiesDataContext.ScheduleLogDetailRecords.Where(item => item.ScheduleLogId == scheduleLog.Id)?.ToList();
                    if (customerCount == logDetailsRecords.Count)
                    {
                        var scheduleLogStatus = ScheduleLogStatus.Completed.ToString();
                        var batchStatus = BatchStatus.Completed.ToString();

                        var failedRecords = logDetailsRecords.Where(item => item.Status == ScheduleLogStatus.Failed.ToString())?.ToList();
                        if (failedRecords != null && failedRecords.Count > 0)
                        {
                            scheduleLogStatus = ScheduleLogStatus.Failed.ToString();
                            batchStatus = BatchStatus.Failed.ToString();
                        }
                        nISEntitiesDataContext.ScheduleLogRecords.Where(item => item.Id == scheduleLog.Id).ToList().ForEach(item => item.Status = scheduleLogStatus);
                        nISEntitiesDataContext.ScheduleRunHistoryRecords.Where(item => item.ScheduleLogId == scheduleLog.Id).ToList().ForEach(item => item.EndDate = DateTime.UtcNow);

                        nISEntitiesDataContext.ScheduleRecords.Where(item => item.Id == scheduleLog.ScheduleId).ToList().ForEach(item => item.Status = ScheduleStatus.Completed.ToString());
                        nISEntitiesDataContext.BatchMasterRecords.Where(item => item.Id == batchMaster.Id).ToList().ForEach(item =>
                        {
                            item.Status = batchStatus;
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

        /// <summary>
        /// Writes to file.
        /// </summary>
        /// <param name="Message">The message.</param>
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

        /// <summary>
        /// This method helps to add batch with no repeat occurence.
        /// </summary>
        /// <param name="scheduleStartDate">The schedule start date.</param>
        /// <param name="schedule">The schedule.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns>
        /// true if added successfully otherwise false
        /// </returns>
        private bool AddDoesNotRepeatBatch(DateTime scheduleStartDate, ScheduleRecord schedule, string tenantCode, int userId, out DateTime endDate, out int noOfOccurance)
        {
            try
            {
                //var newstartdate = DateTime.SpecifyKind((DateTime)scheduleStartDate, DateTimeKind.Utc);
                long batchId = 0;
                IDictionary<string, long> etlSchedule = new Dictionary<string, long>();
                var recordsList = new List<BatchMasterRecord>();
                var newstartdate = new DateTime(scheduleStartDate.Year, scheduleStartDate.Month, (scheduleStartDate.Day), 0, 0, 0);
                newstartdate = DateTime.SpecifyKind((DateTime)newstartdate, DateTimeKind.Utc);
                this.SetAndValidateConnectionString(tenantCode);
                double dataExtractionHours = 0;
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
                    batchId = record.Id;
                    endDate = record.BatchExecutionDate;
                    noOfOccurance = 1;
                    recordsList.Add(record);
                }
                etlSchedule = AddETLSchedules(schedule, tenantCode, endDate.AddHours(dataExtractionHours * -1), endDate.AddHours(dataExtractionHours * -1));

                if (etlSchedule.Count() > 0)
                {
                    var etlScheduleId = etlSchedule.Where(x => x.Key == schedule.Name)?.FirstOrDefault().Value;
                    AddETLBatches(etlScheduleId == null ? 0 : Convert.ToInt64(etlScheduleId), (long)schedule.ProductBatchId, schedule.ScheduleNameByUser, tenantCode, recordsList);
                }

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Adds the does not repeat batch with language.
        /// </summary>
        /// <param name="scheduleStartDate">The schedule start date.</param>
        /// <param name="schedule">The schedule.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        private bool AddDoesNotRepeatBatchWithLanguage(DateTime scheduleStartDate, ScheduleRecord schedule, string tenantCode, int userId)
        {
            try
            {
                //var newstartdate = DateTime.SpecifyKind((DateTime)scheduleStartDate, DateTimeKind.Utc);
                var newstartdate = new DateTime(scheduleStartDate.Year, scheduleStartDate.Month, (scheduleStartDate.Day), 0, 0, 0);
                newstartdate = DateTime.SpecifyKind((DateTime)newstartdate, DateTimeKind.Utc);
                this.SetAndValidateConnectionString(tenantCode);
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    schedule.Languages.Split(',').ToList().ForEach(language => 
                    {
                        BatchMasterRecord record = new BatchMasterRecord();
                        record.BatchName = "Batch 1 of " + schedule.Name + "_" + language;
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
                        record.LanguageCode = language;
                        nISEntitiesDataContext.BatchMasterRecords.Add(record);
                        nISEntitiesDataContext.SaveChanges();
                    });
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
        /// <param name="scheduleStartDate">The schedule start date.</param>
        /// <param name="schedule">The schedule.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="batchIndex">Index of the batch.</param>
        /// <returns>
        /// true if added successfully otherwise false
        /// </returns>
        private bool AddDailyOccurenceScheduleBatches(DateTime scheduleStartDate, ScheduleRecord schedule, string tenantCode, int userId, int batchIndex, out DateTime endDate, out int noOfOccurance)
        {
            bool result = false;
            IDictionary<string, long> etlSchedule = new Dictionary<string, long>();
            DateTime updatedEndDate = new DateTime();
            int updatedNoOfOccurance = 0;
            try
            {
                var repeatEveryDays = schedule.RecurrancePattern == ModelConstant.CUSTOM_DAY ? Convert.ToInt32(schedule.RepeatEveryDayMonWeekYear != 0 ? schedule.RepeatEveryDayMonWeekYear : 1) : 1;
                this.SetAndValidateConnectionString(tenantCode);
                List<BatchMasterRecord> batchMasterRecords = new List<BatchMasterRecord>();
                int dayDiff = 0;
                if (schedule.EndDate != null)
                {
                    //var newenddate = DateTime.SpecifyKind((DateTime)schedule.EndDate, DateTimeKind.Utc);
                    var newenddate = (schedule.EndDate ?? DateTime.Now) + TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now);
                    dayDiff = this.utility.DayDifference(newenddate, scheduleStartDate) + 1;
                }
                else
                {
                    dayDiff = Convert.ToInt32(schedule.NoOfOccurrences ?? 1) * repeatEveryDays;
                }

                if (dayDiff > 0)
                {
                    var newstartdate = new DateTime(scheduleStartDate.Year, scheduleStartDate.Month, (scheduleStartDate.Day), 0, 0, 0);
                    newstartdate = DateTime.SpecifyKind((DateTime)newstartdate, DateTimeKind.Utc);
                    int idx = 1;
                    double dataExtractionHours = 0;

                    //using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                    //{
                    //    dataExtractionHours = Convert.ToDouble((from prm in nISEntitiesDataContext.ProductRecurrenceMap
                    //                                            join rp in nISEntitiesDataContext.RecurrencePattern on prm.RecurrenceId equals rp.Id
                    //                                            join pr in nISEntitiesDataContext.Products on prm.ProductId equals pr.Id
                    //                                            where pr.Id == schedule.ProductId && rp.Recurrence == ModelConstant.DAILY
                    //                                            select prm.HoursForExtraction).FirstOrDefault());
                    //}

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
                    updatedEndDate = batchMasterRecords.Max(x => x.BatchExecutionDate);
                    updatedNoOfOccurance = batchMasterRecords.Count();
                    using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                    {
                        nISEntitiesDataContext.BatchMasterRecords.AddRange(batchMasterRecords);
                        nISEntitiesDataContext.SaveChanges();
                        result = true;
                    }
                    //if (updatedEndDate != DateTime.MinValue && schedule.EndDate == null)
                    //{
                    //    schedule.EndDateForDisplay = updatedEndDate;
                    //    schedule.NoOfOccuranceForDisplay = schedule.NoOfOccurrences;
                    //}
                    //else if (updatedNoOfOccurance != 0 && (schedule.NoOfOccurrences == null || schedule.NoOfOccurrences == 0))
                    //{
                    //    schedule.NoOfOccuranceForDisplay = updatedNoOfOccurance;
                    //    schedule.EndDateForDisplay = schedule.EndDate;
                    //}
                    etlSchedule = AddETLSchedules(schedule, tenantCode, batchMasterRecords.Min(x => x.DataExtractionDate), batchMasterRecords.Max(x => x.DataExtractionDate));
                    if (etlSchedule.Count() > 0)
                    {
                        var etlScheduleId = etlSchedule.Where(x => x.Key == schedule.ScheduleNameByUser)?.FirstOrDefault().Value;
                        AddETLBatches(etlScheduleId == null ? 0 : Convert.ToInt64(etlScheduleId), (long)schedule.ProductBatchId, schedule.ScheduleNameByUser, tenantCode, batchMasterRecords);
                    }
                }
                endDate = updatedEndDate;
                noOfOccurance = updatedNoOfOccurance;
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Adds the daily occurence schedule batches with language.
        /// </summary>
        /// <param name="scheduleStartDate">The schedule start date.</param>
        /// <param name="schedule">The schedule.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="batchIndex">Index of the batch.</param>
        /// <returns></returns>
        private bool AddDailyOccurenceScheduleBatchesWithLanguage(DateTime scheduleStartDate, ScheduleRecord schedule, string tenantCode, int userId, int batchIndex)
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
                    //var newenddate = DateTime.SpecifyKind((DateTime)schedule.EndDate, DateTimeKind.Utc);
                    var newenddate = (schedule.EndDate ?? DateTime.Now) + TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now);
                    dayDiff = this.utility.DayDifference(newenddate, scheduleStartDate) + 1;
                }
                else
                {
                    dayDiff = Convert.ToInt32(schedule.NoOfOccurrences ?? 1) * repeatEveryDays;
                }

                if (dayDiff > 0)
                {
                    var newstartdate = new DateTime(scheduleStartDate.Year, scheduleStartDate.Month, (scheduleStartDate.Day), 0, 0, 0);
                    newstartdate = DateTime.SpecifyKind((DateTime)newstartdate, DateTimeKind.Utc);
                    int idx = 1;
                    while (idx <= dayDiff)
                    {
                        schedule.Languages.Split(',').ToList().ForEach(language =>
                        {
                            BatchMasterRecord record = new BatchMasterRecord();
                            record.BatchName = "Batch " + batchIndex + " of " + schedule.Name + "_" + language;
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
                            record.LanguageCode = language;
                            batchMasterRecords.Add(record);
                        });
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
        /// <param name="scheduleStartDate">The schedule start date.</param>
        /// <param name="schedule">The schedule.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="batchIndex">Index of the batch.</param>
        /// <returns>
        /// true if added successfully otherwise false
        /// </returns>
        private bool AddWeeklyOccurenceScheduleBatches(DateTime scheduleStartDate, ScheduleRecord schedule, string tenantCode, int userId, int batchIndex, out DateTime endDate, out int noOfOccurance)
        {
            bool result = false;
            IDictionary<string, long> etlSchedule = new Dictionary<string, long>();
            try
            {
                var repeatEveryDays = schedule.RecurrancePattern == ModelConstant.CUSTOM_WEEK ? Convert.ToInt32(schedule.RepeatEveryDayMonWeekYear != 0 ? schedule.RepeatEveryDayMonWeekYear : 1) : 1;
                this.SetAndValidateConnectionString(tenantCode);
                List<BatchMasterRecord> batchMasterRecords = new List<BatchMasterRecord>();

                int dayDiff = 0;
                //var scheduleStartDate = schedule.StartDate ?? DateTime.Now;
                if (schedule.EndDate != null)
                {
                    //var newenddate = DateTime.SpecifyKind((DateTime)schedule.EndDate, DateTimeKind.Utc);
                    var newenddate = (schedule.EndDate ?? DateTime.Now) + TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now);
                    dayDiff = this.utility.DayDifference(newenddate, scheduleStartDate) + 1;
                }
                else
                {
                    dayDiff = Convert.ToInt32(schedule.NoOfOccurrences);
                }

                var newstartdate = new DateTime(scheduleStartDate.Year, scheduleStartDate.Month, (scheduleStartDate.Day), 0, 0, 0);
                newstartdate = DateTime.SpecifyKind((DateTime)newstartdate, DateTimeKind.Utc);
                //var newstartdate = DateTime.SpecifyKind((DateTime)scheduleStartDate, DateTimeKind.Utc);
                DateTime updatedEndDate = new DateTime();
                int updatedNoOfOccurance = 0;

                double dataExtractionHours = 0;
                //using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                //{
                //    dataExtractionHours = Convert.ToDouble((from prm in nISEntitiesDataContext.ProductRecurrenceMap
                //                                            join rp in nISEntitiesDataContext.RecurrencePattern on prm.RecurrenceId equals rp.Id
                //                                            join pr in nISEntitiesDataContext.Products on prm.ProductId equals pr.Id
                //                                            where pr.Id == schedule.ProductId && rp.Recurrence == ModelConstant.WEEKLY
                //                                            select prm.HoursForExtraction).FirstOrDefault());
                //}
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

                    updatedEndDate = batchMasterRecords.Max(x => x.BatchExecutionDate);
                    updatedNoOfOccurance = batchMasterRecords.Count();
                    using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                    {
                        nISEntitiesDataContext.BatchMasterRecords.AddRange(batchMasterRecords);
                        nISEntitiesDataContext.SaveChanges();
                        result = true;
                    }
                    //if (updatedEndDate != DateTime.MinValue && schedule.EndDate == null)
                    //{
                    //    schedule.EndDateForDisplay = updatedEndDate;
                    //    schedule.NoOfOccuranceForDisplay = schedule.NoOfOccurrences;
                    //}
                    //else if (updatedNoOfOccurance != 0 && (schedule.NoOfOccurrences == null || schedule.NoOfOccurrences == 0))
                    //{
                    //    schedule.NoOfOccuranceForDisplay = updatedNoOfOccurance;
                    //    schedule.EndDateForDisplay = schedule.EndDate;
                    //}
                    etlSchedule = AddETLSchedules(schedule, tenantCode, batchMasterRecords.Min(x => x.DataExtractionDate), batchMasterRecords.Max(x => x.DataExtractionDate));
                    if (etlSchedule.Count() > 0)
                    {
                        var etlScheduleId = etlSchedule.Where(x => x.Key == schedule.ScheduleNameByUser)?.FirstOrDefault().Value;
                        AddETLBatches(etlScheduleId == null ? 0 : Convert.ToInt64(etlScheduleId), (long)schedule.ProductBatchId, schedule.ScheduleNameByUser, tenantCode, batchMasterRecords);
                    }
                }
                endDate = updatedEndDate;
                noOfOccurance = updatedNoOfOccurance;
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Adds the weekly occurence schedule batches with language.
        /// </summary>
        /// <param name="scheduleStartDate">The schedule start date.</param>
        /// <param name="schedule">The schedule.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="batchIndex">Index of the batch.</param>
        /// <returns></returns>
        private bool AddWeeklyOccurenceScheduleBatchesWithLanguage(DateTime scheduleStartDate, ScheduleRecord schedule, string tenantCode, int userId, int batchIndex)
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
                    //var newenddate = DateTime.SpecifyKind((DateTime)schedule.EndDate, DateTimeKind.Utc);
                    var newenddate = (schedule.EndDate ?? DateTime.Now) + TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now);
                    dayDiff = this.utility.DayDifference(newenddate, scheduleStartDate) + 1;
                }
                else
                {
                    dayDiff = Convert.ToInt32(schedule.NoOfOccurrences);
                }

                var newstartdate = new DateTime(scheduleStartDate.Year, scheduleStartDate.Month, (scheduleStartDate.Day), 0, 0, 0);
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
                                schedule.Languages.Split(',').ToList().ForEach(language =>
                                {
                                    BatchMasterRecord record = new BatchMasterRecord();
                                    record.BatchName = "Batch " + batchIndex + " of " + schedule.Name + "_" + language;
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
                                    record.LanguageCode = language;
                                    batchMasterRecords.Add(record);
                                });
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
                                schedule.Languages.Split(',').ToList().ForEach(language =>
                                {
                                    BatchMasterRecord record = new BatchMasterRecord();
                                    record.BatchName = "Batch " + batchIndex + " of " + schedule.Name + "_" + language;
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
                                    record.LanguageCode = language;
                                    batchMasterRecords.Add(record);
                                });
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
        /// <param name="scheduleStartDate">The schedule start date.</param>
        /// <param name="schedule">The schedule.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="batchIndex">Index of the batch.</param>
        /// <returns>
        /// true if added successfully otherwise false
        /// </returns>
        private bool AddMonthlyOccurenceScheduleBatches(DateTime scheduleStartDate, ScheduleRecord schedule, string tenantCode, int userId, int batchIndex, out DateTime endDate, out int noOfOccurance)
        {
            bool result = false;
            IDictionary<string, long> etlSchedule = new Dictionary<string, long>();
            try
            {
                var repeatEveryMonths = schedule.RecurrancePattern == ModelConstant.CUSTOM_MONTH ? Convert.ToInt32(schedule.RepeatEveryDayMonWeekYear != 0 ? schedule.RepeatEveryDayMonWeekYear : 1) : 1;
                this.SetAndValidateConnectionString(tenantCode);
                List<BatchMasterRecord> batchMasterRecords = new List<BatchMasterRecord>();

                //var newstartdate = DateTime.SpecifyKind((DateTime)scheduleStartDate, DateTimeKind.Utc);
                //var newendate = DateTime.SpecifyKind((DateTime)schedule.EndDate, DateTimeKind.Utc);
                var newstartdate = scheduleStartDate + TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now);
                var newendate = (schedule.EndDate ?? DateTime.Now) + TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now);

                if (schedule.DayOfMonth < newstartdate.Day)
                {
                    newstartdate = newstartdate.AddMonths(1);
                }

                var scheduleExecutionDate = new DateTime(newstartdate.Year, newstartdate.Month, Convert.ToInt32(schedule.DayOfMonth), 0, 0, 0);
                var updatedEndDate = new DateTime();
                int updatedNoOfOccurance = 0;
                double dataExtractionHours = 0;
                //using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                //{
                //    dataExtractionHours = Convert.ToDouble((from prm in nISEntitiesDataContext.ProductRecurrenceMap
                //                                            join rp in nISEntitiesDataContext.RecurrencePattern on prm.RecurrenceId equals rp.Id
                //                                            join pr in nISEntitiesDataContext.Products on prm.ProductId equals pr.Id
                //                                            where pr.Id == schedule.ProductId && rp.Recurrence == ModelConstant.MONTHLY
                //                                            select prm.HoursForExtraction).FirstOrDefault());
                //}
                if (schedule.EndDate != null)
                {
                    while (DateTime.Compare(scheduleExecutionDate, newendate) <= 0)
                    {
                        int DayOfMonth = Convert.ToInt32(schedule.DayOfMonth);
                        if (schedule.DayOfMonth > 28)
                        {
                            int lastDayOfMonth = DateTime.DaysInMonth(scheduleExecutionDate.Year, scheduleExecutionDate.Month);
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
                        var batchExecutionDate = new DateTime(scheduleExecutionDate.Year, scheduleExecutionDate.Month, DayOfMonth, Convert.ToInt32(schedule.HourOfDay), Convert.ToInt32(schedule.MinuteOfDay), 0);
                        record.BatchExecutionDate = batchExecutionDate;
                        record.DataExtractionDate = batchExecutionDate.AddDays(-1);
                        record.Status = BatchStatus.New.ToString();
                        batchMasterRecords.Add(record);
                        scheduleExecutionDate = scheduleExecutionDate.AddMonths(repeatEveryMonths);
                        batchIndex++;
                    }
                }
                else if (schedule.NoOfOccurrences != null && schedule.NoOfOccurrences > 0)
                {
                    long? occurences = schedule.NoOfOccurrences;
                    while (occurences != null && occurences > 0)
                    {
                        int DayOfMonth = Convert.ToInt32(schedule.DayOfMonth);
                        if (DayOfMonth > 28)
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
                updatedEndDate = batchMasterRecords.Max(x => x.BatchExecutionDate);
                updatedNoOfOccurance = batchMasterRecords.Count();
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    nISEntitiesDataContext.BatchMasterRecords.AddRange(batchMasterRecords);
                    nISEntitiesDataContext.SaveChanges();
                    result = true;
                }
                //if (updatedEndDate != DateTime.MinValue && schedule.EndDate == null)
                //{
                //    schedule.EndDateForDisplay = updatedEndDate;
                //    schedule.NoOfOccuranceForDisplay = schedule.NoOfOccurrences;
                //}
                //else if (updatedNoOfOccurance != 0 && (schedule.NoOfOccurrences == null || schedule.NoOfOccurrences == 0))
                //{
                //    schedule.NoOfOccuranceForDisplay = updatedNoOfOccurance;
                //    schedule.EndDateForDisplay = schedule.EndDate;
                //}
                etlSchedule = AddETLSchedules(schedule, tenantCode, batchMasterRecords.Min(x => x.DataExtractionDate), batchMasterRecords.Max(x => x.DataExtractionDate));
                if (etlSchedule.Count() > 0)
                {
                    var etlScheduleId = etlSchedule.Where(x => x.Key == schedule.ScheduleNameByUser)?.FirstOrDefault().Value;
                    AddETLBatches(etlScheduleId == null ? 0 : Convert.ToInt64(etlScheduleId), (long)schedule.ProductBatchId, schedule.ScheduleNameByUser, tenantCode, batchMasterRecords);
                }

                endDate = updatedEndDate;
                noOfOccurance = updatedNoOfOccurance;
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Adds the monthly occurence schedule batches with language.
        /// </summary>
        /// <param name="scheduleStartDate">The schedule start date.</param>
        /// <param name="schedule">The schedule.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="batchIndex">Index of the batch.</param>
        /// <returns></returns>
        private bool AddMonthlyOccurenceScheduleBatchesWithLanguage(DateTime scheduleStartDate, ScheduleRecord schedule, string tenantCode, int userId, int batchIndex)
        {
            bool result = false;
            try
            {
                var repeatEveryMonths = schedule.RecurrancePattern == ModelConstant.CUSTOM_MONTH ? Convert.ToInt32(schedule.RepeatEveryDayMonWeekYear != 0 ? schedule.RepeatEveryDayMonWeekYear : 1) : 1;
                this.SetAndValidateConnectionString(tenantCode);
                List<BatchMasterRecord> batchMasterRecords = new List<BatchMasterRecord>();

                //var newstartdate = DateTime.SpecifyKind((DateTime)scheduleStartDate, DateTimeKind.Utc);
                //var newendate = DateTime.SpecifyKind((DateTime)schedule.EndDate, DateTimeKind.Utc);
                var newstartdate = scheduleStartDate + TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now);
                var newendate = (schedule.EndDate ?? DateTime.Now) + TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now);

                if (schedule.DayOfMonth < newstartdate.Day)
                {
                    newstartdate = newstartdate.AddMonths(1);
                }

                var scheduleExecutionDate = new DateTime(newstartdate.Year, newstartdate.Month, Convert.ToInt32(schedule.DayOfMonth), 0, 0, 0);
                if (schedule.EndDate != null)
                {
                    while (DateTime.Compare(scheduleExecutionDate, newendate) <= 0)
                    {
                        int DayOfMonth = Convert.ToInt32(schedule.DayOfMonth);
                        if (schedule.DayOfMonth > 28)
                        {
                            int lastDayOfMonth = DateTime.DaysInMonth(scheduleExecutionDate.Year, scheduleExecutionDate.Month);
                            if (lastDayOfMonth < DayOfMonth)
                            {
                                DayOfMonth = lastDayOfMonth;
                            }
                        }

                        schedule.Languages.Split(',').ToList().ForEach(language =>
                        {
                            BatchMasterRecord record = new BatchMasterRecord();
                            record.BatchName = "Batch " + batchIndex + " of " + schedule.Name + "_" + language;
                            record.TenantCode = tenantCode;
                            record.CreatedBy = userId;
                            record.CreatedDate = DateTime.UtcNow;
                            record.ScheduleId = schedule.Id;
                            record.IsExecuted = false;
                            record.IsDataReady = false;
                            var batchExecutionDate = new DateTime(scheduleExecutionDate.Year, scheduleExecutionDate.Month, DayOfMonth, Convert.ToInt32(schedule.HourOfDay), Convert.ToInt32(schedule.MinuteOfDay), 0);
                            record.BatchExecutionDate = batchExecutionDate;
                            record.DataExtractionDate = batchExecutionDate.AddDays(-1);
                            record.Status = BatchStatus.New.ToString();
                            record.LanguageCode = language;
                            batchMasterRecords.Add(record);
                        });
                        scheduleExecutionDate = scheduleExecutionDate.AddMonths(repeatEveryMonths);
                        batchIndex++;
                    }
                }
                else if (schedule.NoOfOccurrences != null && schedule.NoOfOccurrences > 0)
                {
                    long? occurences = schedule.NoOfOccurrences;
                    while (occurences != null && occurences > 0)
                    {
                        int DayOfMonth = Convert.ToInt32(schedule.DayOfMonth);
                        if (DayOfMonth > 28)
                        {
                            int lastDayOfMonth = DateTime.DaysInMonth(newstartdate.Year, newstartdate.Month);
                            if (lastDayOfMonth < DayOfMonth)
                            {
                                DayOfMonth = lastDayOfMonth;
                            }
                        }
                        schedule.Languages.Split(',').ToList().ForEach(language =>
                        {
                            BatchMasterRecord record = new BatchMasterRecord();
                            record.BatchName = "Batch " + batchIndex + " of " + schedule.Name + "_" + language;
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
                            record.LanguageCode = language;
                            batchMasterRecords.Add(record);
                        });
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
        /// <param name="scheduleStartDate">The schedule start date.</param>
        /// <param name="schedule">The schedule.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="batchIndex">Index of the batch.</param>
        /// <returns>
        /// true if added successfully otherwise false
        /// </returns>
        private bool AddYearlyOccurenceScheduleBatches(DateTime scheduleStartDate, ScheduleRecord schedule, string tenantCode, int userId, int batchIndex, out DateTime endDate, out int noOfOccurance)
        {
            try
            {
                IDictionary<string, long> etlSchedule = new Dictionary<string, long>();
                var repeatEveryYears = schedule.RecurrancePattern == ModelConstant.CUSTOM_YEAR ? Convert.ToInt32(schedule.RepeatEveryDayMonWeekYear != 0 ? schedule.RepeatEveryDayMonWeekYear : 1) : 1;
                this.SetAndValidateConnectionString(tenantCode);
                List<BatchMasterRecord> batchMasterRecords = new List<BatchMasterRecord>();

                //var startdate = DateTime.SpecifyKind((DateTime)scheduleStartDate, DateTimeKind.Utc);
                var startdate = scheduleStartDate + TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now);
                if (this.utility.getNumericMonth(schedule.MonthOfYear) < startdate.Month
                    || (this.utility.getNumericMonth(schedule.MonthOfYear) == startdate.Month && startdate.Day > schedule.DayOfMonth))
                {
                    startdate = startdate.AddYears(1);
                }

                int DayOfMonth = Convert.ToInt32(schedule.DayOfMonth);
                if (DayOfMonth > 28)
                {
                    int lastDayOfMonth = DateTime.DaysInMonth(startdate.Year, this.utility.getNumericMonth(schedule.MonthOfYear));
                    if (lastDayOfMonth < DayOfMonth)
                    {
                        DayOfMonth = lastDayOfMonth;
                    }
                }

                var newstartdate = new DateTime(startdate.Year, this.utility.getNumericMonth(schedule.MonthOfYear), DayOfMonth, 0, 0, 0);
                newstartdate = DateTime.SpecifyKind((DateTime)newstartdate, DateTimeKind.Utc);
                //var newenddate = DateTime.SpecifyKind((DateTime)schedule.EndDate, DateTimeKind.Utc);
                var newenddate = (schedule.EndDate ?? DateTime.Now) + TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now);

                var yearDiff = 0;
                if (newenddate != null)
                {
                    yearDiff = this.utility.YearDifference(newstartdate, Convert.ToDateTime(newenddate)) + 1;
                }
                else
                {
                    yearDiff = Convert.ToInt32(schedule.NoOfOccurrences);
                }
                var updatedEndDate = new DateTime();
                int updatedNoOfOccurance = 0;
                double dataExtractionHours = 0;
                //using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                //{
                //    dataExtractionHours = Convert.ToDouble((from prm in nISEntitiesDataContext.ProductRecurrenceMap
                //                                            join rp in nISEntitiesDataContext.RecurrencePattern on prm.RecurrenceId equals rp.Id
                //                                            join pr in nISEntitiesDataContext.Products on prm.ProductId equals pr.Id
                //                                            where pr.Id == schedule.ProductId && rp.Recurrence == ModelConstant.YEARLY
                //                                            select prm.HoursForExtraction).FirstOrDefault());
                //}
                if (yearDiff > 0)
                {
                    int idx = 1;
                    while (idx <= yearDiff)
                    {
                        DayOfMonth = Convert.ToInt32(schedule.DayOfMonth);
                        if (DayOfMonth > 28)
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
                        newstartdate = newstartdate.AddYears(repeatEveryYears);
                        batchIndex++;
                        idx = idx + repeatEveryYears;
                    }
                    updatedEndDate = batchMasterRecords.Max(x => x.BatchExecutionDate);
                    updatedNoOfOccurance = batchMasterRecords.Count();
                    using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                    {
                        nISEntitiesDataContext.BatchMasterRecords.AddRange(batchMasterRecords);
                        nISEntitiesDataContext.SaveChanges();
                    }
                    //if (updatedEndDate != DateTime.MinValue && schedule.EndDate == null)
                    //{
                    //    schedule.EndDateForDisplay = updatedEndDate;
                    //    schedule.NoOfOccuranceForDisplay = schedule.NoOfOccurrences;
                    //}
                    //else if (updatedNoOfOccurance != 0 && (schedule.NoOfOccurrences == null || schedule.NoOfOccurrences == 0))
                    //{
                    //    schedule.NoOfOccuranceForDisplay = updatedNoOfOccurance;
                    //    schedule.EndDateForDisplay = schedule.EndDate;
                    //}
                    etlSchedule = AddETLSchedules(schedule, tenantCode, batchMasterRecords.Min(x => x.DataExtractionDate), batchMasterRecords.Max(x => x.DataExtractionDate));
                    if (etlSchedule.Count() > 0)
                    {
                        var etlScheduleId = etlSchedule.Where(x => x.Key == schedule.ScheduleNameByUser)?.FirstOrDefault().Value;
                        AddETLBatches(etlScheduleId == null ? 0 : Convert.ToInt64(etlScheduleId), (long)schedule.ProductBatchId, schedule.ScheduleNameByUser, tenantCode, batchMasterRecords);
                    }                  
                }
                endDate = updatedEndDate;
                noOfOccurance = updatedNoOfOccurance;
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Adds the yearly occurence schedule batches with language.
        /// </summary>
        /// <param name="scheduleStartDate">The schedule start date.</param>
        /// <param name="schedule">The schedule.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="batchIndex">Index of the batch.</param>
        /// <returns></returns>
        private bool AddYearlyOccurenceScheduleBatchesWithLanguage(DateTime scheduleStartDate, ScheduleRecord schedule, string tenantCode, int userId, int batchIndex)
        {
            try
            {
                var repeatEveryYears = schedule.RecurrancePattern == ModelConstant.CUSTOM_YEAR ? Convert.ToInt32(schedule.RepeatEveryDayMonWeekYear != 0 ? schedule.RepeatEveryDayMonWeekYear : 1) : 1;
                this.SetAndValidateConnectionString(tenantCode);
                List<BatchMasterRecord> batchMasterRecords = new List<BatchMasterRecord>();

                //var startdate = DateTime.SpecifyKind((DateTime)scheduleStartDate, DateTimeKind.Utc);
                var startdate = scheduleStartDate + TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now);
                if (this.utility.getNumericMonth(schedule.MonthOfYear) < startdate.Month
                    || (this.utility.getNumericMonth(schedule.MonthOfYear) == startdate.Month && startdate.Day > schedule.DayOfMonth))
                {
                    startdate = startdate.AddYears(1);
                }

                int DayOfMonth = Convert.ToInt32(schedule.DayOfMonth);
                if (DayOfMonth > 28)
                {
                    int lastDayOfMonth = DateTime.DaysInMonth(startdate.Year, this.utility.getNumericMonth(schedule.MonthOfYear));
                    if (lastDayOfMonth < DayOfMonth)
                    {
                        DayOfMonth = lastDayOfMonth;
                    }
                }

                var newstartdate = new DateTime(startdate.Year, this.utility.getNumericMonth(schedule.MonthOfYear), DayOfMonth, 0, 0, 0);
                newstartdate = DateTime.SpecifyKind((DateTime)newstartdate, DateTimeKind.Utc);
                //var newenddate = DateTime.SpecifyKind((DateTime)schedule.EndDate, DateTimeKind.Utc);
                var newenddate = (schedule.EndDate ?? DateTime.Now) + TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now);

                var yearDiff = 0;
                if (newenddate != null)
                {
                    yearDiff = this.utility.YearDifference(newstartdate, newenddate) + 1;
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
                        DayOfMonth = Convert.ToInt32(schedule.DayOfMonth);
                        if (DayOfMonth > 28)
                        {
                            int lastDayOfMonth = DateTime.DaysInMonth(newstartdate.Year, newstartdate.Month);
                            if (lastDayOfMonth < DayOfMonth)
                            {
                                DayOfMonth = lastDayOfMonth;
                            }
                        }

                        schedule.Languages.Split(',').ToList().ForEach(language =>
                        {
                            BatchMasterRecord record = new BatchMasterRecord();
                            record.BatchName = "Batch " + batchIndex + " of " + schedule.Name + "_" + language;
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
                            record.LanguageCode = language;
                            batchMasterRecords.Add(record);
                        });
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

        #endregion
    }
}
