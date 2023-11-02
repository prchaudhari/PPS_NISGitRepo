// <copyright file="SQLETLScheduleRepository.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2018 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------

namespace nIS
{
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
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using nIS.Models;
    using nIS.NedBank;
    using NIS.Repository;
    using NIS.Repository.Entities;
    using nIS;
    using nIS.Models.NedBank;
    using System.Data.Entity;
    using log4net;
    using System.Configuration;
    #endregion

    /// <summary>
    /// This class represents the methods to perform operation with database for etl process entity.
    /// </summary>
    /// <seealso cref="nIS.IDataHubRepository" />
    public class SQLETLScheduleRepository : IETLScheduleRepository
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
        /// The unity container
        /// </summary>
        private IUnityContainer unityContainer = null;
        /// <summary>
        /// The utility object
        /// </summary>
        private IConfigurationUtility configurationutility = null;

        ILog _logger = log4net.LogManager.GetLogger(typeof(SQLETLScheduleRepository));

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SQLETLScheduleRepository" /> class.
        /// </summary>
        /// <param name="unityContainer">The unity container.</param>
        public SQLETLScheduleRepository(IUnityContainer unityContainer)
        {
            this.unityContainer = unityContainer;
            this.validationEngine = new ValidationEngine();
        }

        #endregion

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
                this.connectionString = ConfigurationManager.ConnectionStrings["TenantManagerConnectionString"]?.ConnectionString;
                if (!this.validationEngine.IsValidText(this.connectionString))
                {
                    throw new ConnectionStringNotFoundException(tenantCode);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        #endregion

        #region Public Methods

        #region ETLSchedule 

        public IList<ETLScheduleListModel> GetETLSchedules(ETLScheduleSearchParameter eTLScheduleSearchParameter, string tenantCode)
        {
            IList<ETLScheduleListModel> etlSchedules = new List<ETLScheduleListModel>();
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    IList<ETLScheduleModel> result = new List<ETLScheduleModel>();
                    if (eTLScheduleSearchParameter.PagingParameter.PageIndex > 0 && eTLScheduleSearchParameter.PagingParameter.PageSize > 0)
                    {
                        var queryResult = (from etls in nISEntitiesDataContext.EtlSchedules
                                           join sch in nISEntitiesDataContext.ScheduleRecords on etls.ProductBatchId equals sch.ProductBatchId
                                           join pr in nISEntitiesDataContext.Products on etls.ProductId equals pr.Id
                                           orderby eTLScheduleSearchParameter.SortParameter.SortColumn + " " + eTLScheduleSearchParameter.SortParameter.SortOrder.ToString()

                                           select new ETLScheduleModel
                                           {
                                               Id = etls.Id,
                                               Name = etls.Name,
                                               Product = pr.Name,
                                               ProductId = etls.ProductId,
                                               ProductBatchId = (long)sch.ProductBatchId,
                                               ScheduleDate = sch.LastUpdatedDate,
                                               DayOfMonth = sch.DayOfMonth,
                                               HourOfDay = etls.HourOfDay,
                                               MinuteOfDay = etls.MinuteOfDay,
                                               StartDate = etls.StartDate,
                                               EndDate = etls.EndDate,
                                               Status = etls.Status,
                                               IsActive = etls.IsActive,
                                               IsDeleted = sch.IsDeleted,
                                               TenantCode = sch.TenantCode,
                                               IsLastDate = etls.IsLastDate,
                                               RecurrancePattern = sch.RecurrancePattern,
                                               ScheduleNameByUser = sch.ScheduleNameByUser,
                                               RepeatEveryDayMonWeekYear = sch.RepeatEveryDayMonWeekYear,
                                               //EndDateForDisplay = sch.EndDateForDisplay,
                                               //NoOfOccuranceForDisplay = sch.NoOfOccuranceForDisplay,
                                               WeekDays = sch.WeekDays,
                                               ExecutedBatches = nISEntitiesDataContext.EtlBatches.Where(x => x.ProductBatchId == sch.ProductBatchId && x.IsExecuted == true).Count(),
                                               TotalBacthes = nISEntitiesDataContext.EtlBatches.Where(x => x.ProductBatchId == sch.ProductBatchId).Count()
                                           });

                        result = WhereClauseGeneratorInQuery(eTLScheduleSearchParameter, tenantCode, queryResult).GroupBy(x => x.ProductBatchId).OrderByDescending(x => x.Key).Skip((eTLScheduleSearchParameter.PagingParameter.PageIndex - 1) * eTLScheduleSearchParameter.PagingParameter.PageSize)
                                    .Take(eTLScheduleSearchParameter.PagingParameter.PageSize).Select(x => x.FirstOrDefault())
                                    .ToList();
                    }
                    else
                    {
                        var queryResult = (from etls in nISEntitiesDataContext.EtlSchedules
                                           join sch in nISEntitiesDataContext.ScheduleRecords on etls.ProductBatchId equals sch.ProductBatchId
                                           join s in nISEntitiesDataContext.StatementRecords on sch.StatementId equals s.Id
                                           join pr in nISEntitiesDataContext.Products on etls.ProductId equals pr.Id
                                           orderby eTLScheduleSearchParameter.SortParameter.SortColumn + " " + eTLScheduleSearchParameter.SortParameter.SortOrder.ToString()

                                           select new ETLScheduleModel
                                           {
                                               Id = etls.Id,
                                               Name = etls.Name,
                                               Product = pr.Name,
                                               ProductId = etls.ProductId,
                                               ProductBatchId = (long)sch.ProductBatchId,
                                               ScheduleDate = sch.LastUpdatedDate,
                                               DayOfMonth = sch.DayOfMonth,
                                               HourOfDay = etls.HourOfDay,
                                               MinuteOfDay = etls.MinuteOfDay,
                                               StartDate = etls.StartDate,
                                               EndDate = etls.EndDate,
                                               Status = etls.Status,
                                               IsActive = sch.IsActive,
                                               IsDeleted = sch.IsDeleted,
                                               TenantCode = sch.TenantCode,
                                               IsLastDate = etls.IsLastDate,
                                               RecurrancePattern = sch.RecurrancePattern,
                                               ScheduleNameByUser = sch.ScheduleNameByUser,
                                               RepeatEveryDayMonWeekYear = sch.RepeatEveryDayMonWeekYear,
                                               //EndDateForDisplay = sch.EndDateForDisplay,
                                               //NoOfOccuranceForDisplay = sch.NoOfOccuranceForDisplay,
                                               //WeekDays = sch.WeekDays,
                                               ScheduleStatements = s.Name,
                                           });

                        result = WhereClauseGeneratorInQuery(eTLScheduleSearchParameter, tenantCode, queryResult).OrderByDescending(x => x.Id)
                                    .ToList();
                    }

                    if (result != null && result.Count > 0)
                    {
                        if (eTLScheduleSearchParameter.PagingParameter.PageIndex > 0 && eTLScheduleSearchParameter.PagingParameter.PageSize > 0)
                        {
                            List<string> productName = new List<string>();
                            int index = 0;
                            foreach (var item in result)
                            {
                                if (!productName.Contains(item.Product))
                                {
                                    productName.Add(item.Product);
                                    index = productName.IndexOf(item.Product);
                                    etlSchedules.Add(new ETLScheduleListModel
                                    {
                                        ProductName = item.Product,
                                        ProductId = item.ProductId
                                    });
                                }
                                else
                                {
                                    index = productName.IndexOf(item.Product);
                                }

                                etlSchedules[index].ProductBatches.Add(new ETLScheduleModel
                                {
                                    Id = item.Id,
                                    Name = item.Name,
                                    Product = item.Product,
                                    ProductId = item.ProductId,
                                    ProductBatchId = item.ProductBatchId,
                                    ScheduleDate = item.ScheduleDate,
                                    DayOfMonth = item.DayOfMonth,
                                    HourOfDay = item.HourOfDay,
                                    MinuteOfDay = item.MinuteOfDay,
                                    StartDate = item.StartDate,
                                    EndDate = item.EndDate,
                                    Status = item.Status,
                                    IsActive = item.IsActive,
                                    IsDeleted = item.IsDeleted,
                                    TenantCode = item.TenantCode,
                                    IsLastDate = item.IsLastDate,
                                    RecurrancePattern = item.RecurrancePattern,
                                    ScheduleNameByUser = item.ScheduleNameByUser,
                                    RepeatEveryDayMonWeekYear = item.RepeatEveryDayMonWeekYear,
                                    EndDateForDisplay = item.EndDateForDisplay,
                                    NoOfOccuranceForDisplay = item.NoOfOccuranceForDisplay,
                                    WeekDays = item.WeekDays,
                                    ExecutedBatches = item.ExecutedBatches,
                                    TotalBacthes = item.TotalBacthes
                                });
                            }
                        }
                        else
                        {
                            var item = result.FirstOrDefault();
                            etlSchedules.Add(new ETLScheduleListModel
                            {
                                ProductName = item.Product,
                                ProductId = item.ProductId,
                                ProductBatches = new List<ETLScheduleModel>() { new ETLScheduleModel
                                {
                                    Id = item.Id,
                                    Name = item.Name,
                                    Product = item.Product,
                                    ProductId = item.ProductId,
                                    ProductBatchId = item.ProductBatchId,
                                    ScheduleDate = item.ScheduleDate,
                                    DayOfMonth = item.DayOfMonth,
                                    HourOfDay = item.HourOfDay,
                                    MinuteOfDay = item.MinuteOfDay,
                                    StartDate = item.StartDate,
                                    EndDate = item.EndDate,
                                    Status = item.Status,
                                    IsActive = item.IsActive,
                                    IsDeleted = item.IsDeleted,
                                    TenantCode = item.TenantCode,
                                    IsLastDate = item.IsLastDate,
                                    RecurrancePattern = item.RecurrancePattern,
                                    ScheduleNameByUser = item.ScheduleNameByUser,
                                    RepeatEveryDayMonWeekYear = item.RepeatEveryDayMonWeekYear,
                                    EndDateForDisplay = item.EndDateForDisplay,
                                    NoOfOccuranceForDisplay = item.NoOfOccuranceForDisplay,
                                    WeekDays = item.WeekDays,
                                    ScheduleStatements = string.Join(",", result.Select(x => x.ScheduleStatements).ToList())
                                }
                            }
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return etlSchedules;
        }

        public int GetETLScheduleCountWithProduct(ETLScheduleSearchParameter eTLScheduleSearchParameter, string tenantCode)
        {
            int scheduleCount = 0;
            IList<ETLScheduleListModel> schedules = new List<ETLScheduleListModel>();
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    IList<ETLScheduleModel> result = new List<ETLScheduleModel>();

                    var queryResult = (from etls in nISEntitiesDataContext.EtlSchedules
                                       join sch in nISEntitiesDataContext.ScheduleRecords on etls.ProductBatchId equals sch.ProductBatchId
                                       join pr in nISEntitiesDataContext.Products on etls.ProductId equals pr.Id
                                       orderby eTLScheduleSearchParameter.SortParameter.SortColumn + " " + eTLScheduleSearchParameter.SortParameter.SortOrder.ToString()

                                       select new ETLScheduleModel
                                       {
                                           Id = etls.Id,
                                           Name = etls.Name,
                                           Product = pr.Name,
                                           ProductId = etls.ProductId,
                                           ProductBatchId = (long)sch.ProductBatchId,
                                           ScheduleDate = sch.LastUpdatedDate,
                                           DayOfMonth = sch.DayOfMonth,
                                           HourOfDay = sch.HourOfDay,
                                           MinuteOfDay = sch.MinuteOfDay,
                                           StartDate = etls.StartDate,
                                           EndDate = etls.EndDate,
                                           Status = etls.Status,
                                           IsActive = sch.IsActive,
                                           IsDeleted = sch.IsDeleted,
                                           TenantCode = sch.TenantCode,
                                           IsLastDate = etls.IsLastDate,
                                           RecurrancePattern = sch.RecurrancePattern,
                                           ScheduleNameByUser = sch.ScheduleNameByUser,
                                           RepeatEveryDayMonWeekYear = sch.RepeatEveryDayMonWeekYear,
                                           //EndDateForDisplay = sch.EndDateForDisplay,
                                           //NoOfOccuranceForDisplay = sch.NoOfOccuranceForDisplay,
                                       });

                    result = WhereClauseGeneratorInQuery(eTLScheduleSearchParameter, tenantCode, queryResult).GroupBy(x => x.ProductBatchId).OrderByDescending(x => x.Key)
                                    .Select(x => x.FirstOrDefault()).ToList();

                    if (result != null && result.Count > 0)
                    {
                        foreach (var item in result)
                        {
                            scheduleCount++;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return scheduleCount;
        }

        #endregion

        #region ETLSchedule detail

        /// <summary>
        /// This method used to get the etl schedule detail based on search paramter.
        /// </summary>
        /// <param name="eTLScheduleSearchParameter"></param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <param name="noOfRecordCount">out parameter for total of record in database's table</param>
        /// <returns>Returns etl schedules detail if found for given parameters, else return null</returns>
        public IList<ETLScheduleListModel> GetETLScheduleDetailByProduct(ETLScheduleSearchParameter eTLScheduleSearchParameter, string tenantCode, out int noOfRecordCount)
        {
            IList<ETLScheduleListModel> schedules = new List<ETLScheduleListModel>();
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    IList<EtlSchedules> eTLScheduleRecords = new List<EtlSchedules>();
                    List<ETLScheduleModel> result = new List<ETLScheduleModel>();

                    if (eTLScheduleSearchParameter.PagingParameter.PageIndex > 0 && eTLScheduleSearchParameter.PagingParameter.PageSize > 0)
                    {
                        var queryResult = (from etls in nISEntitiesDataContext.EtlSchedules
                                           join sc in nISEntitiesDataContext.ScheduleRecords on etls.ScheduleId equals sc.Id
                                           join pro in nISEntitiesDataContext.Products on etls.ProductId equals pro.Id
                                           select new ETLScheduleModel
                                           {
                                               Id = etls.Id,
                                               Name = etls.Name,
                                               Product = pro.Name,
                                               ProductId = pro.Id,
                                               StartDate = etls.StartDate,
                                               EndDate = etls.EndDate,
                                               RecurrancePattern = sc.RecurrancePattern,
                                               HourOfDay = sc.HourOfDay, //Time For The day
                                               MinuteOfDay = sc.MinuteOfDay,//Time For The day
                                               RepeatEveryDayMonWeekYear = sc.RepeatEveryDayMonWeekYear,
                                               //EndDateForDisplay = sc.EndDateForDisplay,
                                               //NoOfOccuranceForDisplay = sc.NoOfOccuranceForDisplay
                                           });

                        //result = WhereClauseGeneratorInQuery(eTLScheduleSearchParameter, tenantCode, queryResult).GroupBy(x => x.ProductBatchName).OrderBy(x => x.Key).Skip((eTLScheduleSearchParameter.PagingParameter.PageIndex - 1) * eTLScheduleSearchParameter.PagingParameter.PageSize)
                        //            .Take(eTLScheduleSearchParameter.PagingParameter.PageSize).Select(x => x.FirstOrDefault())
                        //            .ToList();
                    }
                    else
                    {
                        var queryResult = (from etls in nISEntitiesDataContext.EtlSchedules
                                           join sc in nISEntitiesDataContext.ScheduleRecords on etls.ScheduleId equals sc.Id
                                           join pro in nISEntitiesDataContext.Products on etls.ProductId equals pro.Id
                                           select new ETLScheduleModel
                                           {
                                               Id = etls.Id,
                                               Name = etls.Name,
                                               Product = pro.Name,
                                               ProductId = pro.Id,
                                               StartDate = etls.StartDate,
                                               EndDate = etls.EndDate,
                                               RecurrancePattern = sc.RecurrancePattern,
                                               HourOfDay = sc.HourOfDay, //Time For The day
                                               MinuteOfDay = sc.MinuteOfDay,//Time For The day
                                               RepeatEveryDayMonWeekYear = sc.RepeatEveryDayMonWeekYear,
                                               //EndDateForDisplay = sc.EndDateForDisplay,
                                               //NoOfOccuranceForDisplay = sc.NoOfOccuranceForDisplay
                                           });

                        //result = WhereClauseGeneratorInQuery(eTLScheduleSearchParameter, tenantCode, queryResult).OrderBy(x => x.ProductBatchName).ToList();
                    }
                    if (result != null && result.Count > 0)
                    {
                        List<string> productName = new List<string>();
                        int index = 0;
                        foreach (var item in result)
                        {
                            if (!productName.Contains(item.Product))
                            {
                                productName.Add(item.Product);
                                index = productName.IndexOf(item.Product);
                                schedules.Add(new ETLScheduleListModel
                                {
                                    ProductName = item.Product,
                                    ProductId = item.ProductId
                                });
                            }
                            else
                            {
                                index = productName.IndexOf(item.Product);
                            }

                            schedules[index].ProductBatches.Add(new ETLScheduleModel
                            {
                                Id = item.Id,
                                Name = item.Name,
                                Product = item.Product,
                                ProductId = item.ProductId,
                                DayOfMonth = item.DayOfMonth,
                                HourOfDay = item.HourOfDay,
                                MinuteOfDay = item.MinuteOfDay,
                                StartDate = item.StartDate,
                                EndDate = item.EndDate,
                                IsActive = item.IsActive,
                                RecurrancePattern = item.RecurrancePattern,
                                RepeatEveryDayMonWeekYear = item.RepeatEveryDayMonWeekYear,
                                ScheduleNameByUser = item.ScheduleNameByUser,
                                EndDateForDisplay = item.EndDateForDisplay,
                                NoOfOccuranceForDisplay = item.NoOfOccuranceForDisplay
                            });
                        }
                    }
                    noOfRecordCount = result.Count();
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return schedules;
        }

        /// <summary>
        /// Generate string for dynamic linq.
        /// </summary>
        /// <param name="eTLScheduleSearchParameter">ETL ScheduleViewModel search Parameters</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns>
        /// Returns a string.
        /// </returns>
        private IQueryable<ETLScheduleModel> WhereClauseGeneratorInQuery(ETLScheduleSearchParameter eTLScheduleSearchParameter, string tenantCode, IQueryable<ETLScheduleModel> schedule)
        {
            if (eTLScheduleSearchParameter.SearchMode == SearchMode.Equals)
            {
                if (validationEngine.IsValidLong(eTLScheduleSearchParameter.ProductBatchId))
                {
                    schedule = schedule.Where(x => x.ProductBatchId == eTLScheduleSearchParameter.ProductBatchId);
                }
                if (validationEngine.IsValidText(eTLScheduleSearchParameter.Identifier))
                {
                    var ids = eTLScheduleSearchParameter.Identifier.ToString().Split(',').Select(x => x).ToList();
                    schedule = schedule.Where(x => ids.All(y => y == x.Id.ToString()));
                }
                if (validationEngine.IsValidText(eTLScheduleSearchParameter.Name))
                {
                    schedule = schedule.Where(x => x.Name == eTLScheduleSearchParameter.Name);
                }
            }
            if (eTLScheduleSearchParameter.SearchMode == SearchMode.Contains)
            {
                if (validationEngine.IsValidText(eTLScheduleSearchParameter.Name))
                {
                    schedule = schedule.Where(x => x.Name.Contains(eTLScheduleSearchParameter.Name));
                }
            }

            if (eTLScheduleSearchParameter.ProductId != 0)
            {
                schedule = schedule.Where(x => x.ProductId == eTLScheduleSearchParameter.ProductId);
            }

            if (eTLScheduleSearchParameter.IsActive != null)
            {
                if (eTLScheduleSearchParameter.IsActive == false)
                {
                    schedule = schedule.Where(x => x.IsActive == false);
                }
                else
                {
                    schedule = schedule.Where(x => x.IsActive == true);
                }
            }

            if (this.validationEngine.IsValidDate(eTLScheduleSearchParameter.ScheduleDate))
            {
                DateTime st = new DateTime(eTLScheduleSearchParameter.ScheduleDate.Year, eTLScheduleSearchParameter.ScheduleDate.Month, eTLScheduleSearchParameter.ScheduleDate.Day, 0, 0, 0);
                DateTime et = new DateTime(eTLScheduleSearchParameter.ScheduleDate.Year, eTLScheduleSearchParameter.ScheduleDate.Month, eTLScheduleSearchParameter.ScheduleDate.Day, 23, 59, 59);
                schedule = schedule.Where(x => x.ScheduleDate >= st && x.ScheduleDate <= et);
            }

            if (this.validationEngine.IsValidDate(eTLScheduleSearchParameter.StartDate) && !this.validationEngine.IsValidDate(eTLScheduleSearchParameter.EndDate))
            {
                DateTime st = new DateTime(eTLScheduleSearchParameter.StartDate.Year, eTLScheduleSearchParameter.StartDate.Month, eTLScheduleSearchParameter.StartDate.Day, 0, 0, 0);
                DateTime et = new DateTime(eTLScheduleSearchParameter.StartDate.Year, eTLScheduleSearchParameter.StartDate.Month, eTLScheduleSearchParameter.StartDate.Day, 23, 59, 59);
                schedule = schedule.Where(x => x.StartDate >= st && x.StartDate <= et);

                //schedule = schedule.Where(x => x.StartDate >= eTLScheduleSearchParameter.StartDate);
            }

            if (this.validationEngine.IsValidDate(eTLScheduleSearchParameter.EndDate) && !this.validationEngine.IsValidDate(eTLScheduleSearchParameter.StartDate))
            {
                DateTime st = new DateTime(eTLScheduleSearchParameter.EndDate.Year, eTLScheduleSearchParameter.EndDate.Month, eTLScheduleSearchParameter.EndDate.Day, 0, 0, 0);
                DateTime et = new DateTime(eTLScheduleSearchParameter.EndDate.Year, eTLScheduleSearchParameter.EndDate.Month, eTLScheduleSearchParameter.EndDate.Day, 23, 59, 59);
                schedule = schedule.Where(x => x.EndDate >= st && x.EndDate <= et);

                //DateTime toDateTime = DateTime.SpecifyKind(Convert.ToDateTime(eTLScheduleSearchParameter.EndDate), DateTimeKind.Utc);
                //schedule = schedule.Where(x => x.EndDate <= eTLScheduleSearchParameter.EndDate);
            }

            if (this.validationEngine.IsValidDate(eTLScheduleSearchParameter.StartDate) && this.validationEngine.IsValidDate(eTLScheduleSearchParameter.EndDate))
            {
                DateTime st = new DateTime(eTLScheduleSearchParameter.StartDate.Year, eTLScheduleSearchParameter.StartDate.Month, eTLScheduleSearchParameter.StartDate.Day, 0, 0, 0);
                DateTime et = new DateTime(eTLScheduleSearchParameter.EndDate.Year, eTLScheduleSearchParameter.EndDate.Month, eTLScheduleSearchParameter.EndDate.Day, 23, 59, 59);
                schedule = schedule.Where(x => x.StartDate >= st && x.EndDate <= et);

                //schedule = schedule.Where(x => x.StartDate >= eTLScheduleSearchParameter.StartDate && x.EndDate <= eTLScheduleSearchParameter.EndDate);
            }

            if (tenantCode != ModelConstant.DEFAULT_TENANT_CODE)
            {
                schedule = schedule.Where(x => x.TenantCode == tenantCode);
            }

            return schedule;
        }
        #endregion

        //#region ETLScheduleBatchLog

        ///// <summary>
        ///// Generate string for dynamic linq.
        ///// </summary>
        ///// <param name="eTLScheduleBatchLogSearchParameter">ETL Schedule Batch Log search Parameters.</param>
        ///// <param name="tenantCode">The tenant code.</param>
        ///// <returns>Returns a string.</returns>
        //private string WhereClauseGeneratorForETLScheduleBatchLogs(ETLScheduleBatchLogSearchParameter eTLScheduleBatchLogSearchParameter, string tenantCode)
        //{
        //    StringBuilder queryString = new StringBuilder();
        //    if (eTLScheduleBatchLogSearchParameter.ETLBatchId != 0)
        //    {
        //        queryString.Append(string.Format("ETLBatchId.Equals({0}) and ", eTLScheduleBatchLogSearchParameter.ETLBatchId));
        //    }

        //    var finalQuery = string.Empty;
        //    if (tenantCode != ModelConstant.DEFAULT_TENANT_CODE)
        //    {
        //        queryString.Append(string.Format("TenantCode.Equals(\"{0}\") ", tenantCode));
        //        finalQuery = queryString.ToString();
        //    }
        //    else
        //    {
        //        int last = queryString.ToString().LastIndexOf("and");
        //        finalQuery = queryString.ToString().Substring(0, last);
        //    }

        //    return finalQuery;
        //}

        ///// <summary>
        ///// This method gets the specified list of ETL Schedule Batch Log from database.
        ///// </summary>
        ///// <param name="eTLScheduleBatchLogSearchParameter">ETL Schedule Batch Log search Parameters.</param>
        ///// <param name="tenantCode">The tenant code.</param>
        ///// <returns>Returns the list of ETL Schedule Batch Log.</returns>
        //public IList<ETLScheduleBatchLogModel> GetETLScheduleBatchLogs(ETLScheduleBatchLogSearchParameter eTLScheduleBatchLogSearchParameter, string tenantCode, out int noOfRecordCount)
        //{
        //    IList<ETLScheduleBatchLogModel> eTLScheduleBatchLogs = new List<ETLScheduleBatchLogModel>();
        //    try
        //    {
        //        this.SetAndValidateConnectionString(tenantCode);
        //        string whereClause = this.WhereClauseGeneratorForETLScheduleBatchLogs(eTLScheduleBatchLogSearchParameter, tenantCode);
        //        IList<View_ETLScheduleLog> scheduleLogRecords = new List<View_ETLScheduleLog>();
        //        using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
        //        {
        //            scheduleLogRecords = nISEntitiesDataContext.View_ETLScheduleLog.Where(whereClause).ToList();
        //            noOfRecordCount = scheduleLogRecords.Count();
        //            if (noOfRecordCount > 0)
        //            {
        //                eTLScheduleBatchLogs = scheduleLogRecords.Select(eTLScheduleBatchLogRecord => new ETLScheduleBatchLogModel()
        //                {
        //                    Identifier = eTLScheduleBatchLogRecord.Id,
        //                    ETLSchedule = eTLScheduleBatchLogRecord.ETLSchedule,
        //                    Batch = eTLScheduleBatchLogRecord.BatchName,
        //                    ProcessingTime = eTLScheduleBatchLogRecord.ProcessingTime,
        //                    Status = eTLScheduleBatchLogRecord.Status,
        //                    ExecutionDate = eTLScheduleBatchLogRecord.ExecutionDate != null ? DateTime.SpecifyKind((DateTime)eTLScheduleBatchLogRecord.ExecutionDate, DateTimeKind.Utc) : DateTime.MinValue,
        //                    EtlScheduleId = eTLScheduleBatchLogRecord.EtlScheduleId,
        //                    LogMessage = string.IsNullOrEmpty(eTLScheduleBatchLogRecord.LogMessage) ? "" : eTLScheduleBatchLogRecord.LogMessage
        //                }).ToList();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //    return eTLScheduleBatchLogs;
        //}

        //#endregion

        //#region ETLScheduleBatchLogDetail

        ///// <summary>
        ///// Generate string for dynamic linq.
        ///// </summary>
        ///// <param name="eTLScheduleBatchLogDetailSearchParameter">ETL Schedule Batch Log Detail search Parameters.</param>
        ///// <param name="tenantCode">The tenant code.</param>
        ///// <returns>Returns a string.</returns>
        //private string WhereClauseGeneratorForETLScheduleDetailForBatchLogDetail(ETLScheduleDetailForBatchLogDetailSearchParameter eTLScheduleDetailForBatchLogDetailSearchParameter, string tenantCode)
        //{
        //    StringBuilder queryString = new StringBuilder();
        //    if (eTLScheduleDetailForBatchLogDetailSearchParameter.EtlLogId != 0)
        //    {
        //        queryString.Append(string.Format("EtlLogId.Equals({0}) and ", eTLScheduleDetailForBatchLogDetailSearchParameter.EtlLogId));
        //    }
        //    else
        //    {
        //        if (eTLScheduleDetailForBatchLogDetailSearchParameter.EtlScheduleId != 0 && eTLScheduleDetailForBatchLogDetailSearchParameter.EtlBatchId != 0)
        //        {
        //            queryString.Append(string.Format("EtlScheduleId.Equals({0}) and EtlBatchId.Equals({1}) and ", eTLScheduleDetailForBatchLogDetailSearchParameter.EtlScheduleId, eTLScheduleDetailForBatchLogDetailSearchParameter.EtlBatchId));
        //        }
        //    }

        //    var finalQuery = string.Empty;
        //    if (tenantCode != ModelConstant.DEFAULT_TENANT_CODE)
        //    {
        //        queryString.Append(string.Format("TenantCode.Equals(\"{0}\") ", tenantCode));
        //        finalQuery = queryString.ToString();
        //    }
        //    else
        //    {
        //        int last = queryString.ToString().LastIndexOf("and");
        //        finalQuery = queryString.ToString().Substring(0, last);
        //    }

        //    return finalQuery;
        //}

        ///// <summary>
        ///// This method gets the specified list of ETL Schedule Detail For Batch Log Detail from database.
        ///// </summary>
        ///// <param name="eTLScheduleDetailForBatchLogDetailSearchParameter">The Etl Schedule Batch Log Detail search parameter.</param>
        ///// <param name="tenantCode">The tenant code.</param>
        ///// <returns>Returns the list of ETL Schedule Batch Log Detail.</returns>
        //public ETLScheduleDetailForBatchLogDetail GetETLScheduleDetailForBatchLogDetail(ETLScheduleDetailForBatchLogDetailSearchParameter eTLScheduleDetailForBatchLogDetailSearchParameter, string tenantCode)
        //{
        //    ETLScheduleDetailForBatchLogDetail eTLScheduleDetailForBatchLogDetail = new ETLScheduleDetailForBatchLogDetail();
        //    try
        //    {
        //        this.SetAndValidateConnectionString(tenantCode);
        //        string whereClause = this.WhereClauseGeneratorForETLScheduleDetailForBatchLogDetail(eTLScheduleDetailForBatchLogDetailSearchParameter, tenantCode);

        //        using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
        //        {
        //            if (eTLScheduleDetailForBatchLogDetailSearchParameter.EtlLogId != 0)
        //            {
        //                eTLScheduleDetailForBatchLogDetail = nISEntitiesDataContext.View_ETLScheduleDetailByEtlLogId
        //                                                    .Where(whereClause)
        //                                                    .Select(x => new ETLScheduleDetailForBatchLogDetail
        //                                                    {
        //                                                        EtlLogsId = x.EtlLogId,
        //                                                        ETLScheduleId = x.ETLScheduleId,
        //                                                        ETLBatchId = x.ETLBatchId,
        //                                                        EtlScheduleName = x.EtlScheduleName,
        //                                                        ETLBatchName = x.ETLBatchName
        //                                                    }).FirstOrDefault();
        //            }
        //            else
        //            {
        //                if (eTLScheduleDetailForBatchLogDetailSearchParameter.EtlScheduleId != 0 && eTLScheduleDetailForBatchLogDetailSearchParameter.EtlBatchId != 0)
        //                {
        //                    eTLScheduleDetailForBatchLogDetail = nISEntitiesDataContext.View_ETLScheduleDetailByEtlScheduleIdAndEtlBatchId
        //                                                        .Where(whereClause)
        //                                                        .Select(x => new ETLScheduleDetailForBatchLogDetail
        //                                                        {
        //                                                            EtlLogsId = 0,
        //                                                            ETLScheduleId = x.ETLScheduleId,
        //                                                            ETLBatchId = x.ETLBatchId,
        //                                                            EtlScheduleName = x.EtlScheduleName,
        //                                                            ETLBatchName = x.ETLBatchName
        //                                                        }).FirstOrDefault();
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }

        //    return eTLScheduleDetailForBatchLogDetail;
        //}


        ///// <summary>
        ///// Generate string for dynamic linq.
        ///// </summary>
        ///// <param name="eTLScheduleBatchLogDetailSearchParameter">ETL Schedule Batch Log Detail search Parameters.</param>
        ///// <param name="tenantCode">The tenant code.</param>
        ///// <returns>Returns a string.</returns>
        //private string WhereClauseGeneratorForETLScheduleBatchLogDetail(ETLScheduleBatchLogDetailSearchParameter eTLScheduleBatchLogDetailSearchParameter, string tenantCode)
        //{
        //    StringBuilder queryString = new StringBuilder();
        //    if (eTLScheduleBatchLogDetailSearchParameter.SearchMode == SearchMode.Equals)
        //    {
        //        if (validationEngine.IsValidText(eTLScheduleBatchLogDetailSearchParameter.Identifier))
        //        {
        //            queryString.Append("(" + string.Join("or ", eTLScheduleBatchLogDetailSearchParameter.Identifier.ToString().Split(',').Select(item => string.Format("Id.Equals({0}) ", item))) + ") and ");
        //        }
        //        if (validationEngine.IsValidText(eTLScheduleBatchLogDetailSearchParameter.Segment))
        //        {
        //            queryString.Append(string.Format("Segment.Equals(\"{0}\") and ", eTLScheduleBatchLogDetailSearchParameter.Segment));
        //        }
        //        if (validationEngine.IsValidText(eTLScheduleBatchLogDetailSearchParameter.Language))
        //        {
        //            queryString.Append(string.Format("Language.Equals(\"{0}\") and ", eTLScheduleBatchLogDetailSearchParameter.Language));
        //        }
        //        if (validationEngine.IsValidText(eTLScheduleBatchLogDetailSearchParameter.Status))
        //        {
        //            queryString.Append(string.Format("Status.Equals(\"{0}\") and ", eTLScheduleBatchLogDetailSearchParameter.Status));
        //        }
        //    }
        //    if (eTLScheduleBatchLogDetailSearchParameter.SearchMode == SearchMode.Contains)
        //    {
        //        if (validationEngine.IsValidText(eTLScheduleBatchLogDetailSearchParameter.Segment))
        //        {
        //            queryString.Append(string.Format("Segment.Contains(\"{0}\") and ", eTLScheduleBatchLogDetailSearchParameter.Segment));
        //        }
        //        if (validationEngine.IsValidText(eTLScheduleBatchLogDetailSearchParameter.Language))
        //        {
        //            queryString.Append(string.Format("Language.Contains(\"{0}\") and ", eTLScheduleBatchLogDetailSearchParameter.Language));
        //        }
        //        if (validationEngine.IsValidText(eTLScheduleBatchLogDetailSearchParameter.Status))
        //        {
        //            queryString.Append(string.Format("Status.Contains(\"{0}\") and ", eTLScheduleBatchLogDetailSearchParameter.Status));
        //        }
        //    }

        //    if (eTLScheduleBatchLogDetailSearchParameter.EtlLogId != 0)
        //    {
        //        queryString.Append(string.Format("EtlLogId.Equals({0}) and ", eTLScheduleBatchLogDetailSearchParameter.EtlLogId));
        //    }

        //    var finalQuery = string.Empty;
        //    if (tenantCode != ModelConstant.DEFAULT_TENANT_CODE)
        //    {
        //        queryString.Append(string.Format("TenantCode.Equals(\"{0}\") ", tenantCode));
        //        finalQuery = queryString.ToString();
        //    }
        //    else
        //    {
        //        int last = queryString.ToString().LastIndexOf("and");
        //        finalQuery = queryString.ToString().Substring(0, last);
        //    }

        //    return finalQuery;
        //}

        ///// <summary>
        ///// This method gets the specified list of ETL Schedule Batch Log Detail from database.
        ///// </summary>
        ///// <param name="eTLScheduleBatchLogDetailSearchParameter">The Etl Schedule Batch Log Detail search parameter.</param>
        ///// <param name="tenantCode">The tenant code.</param>
        ///// <param name="noOfRecordCount">out parameter for total of record in database's table.</param>
        ///// <returns>Returns the list of ETL Schedule Batch Log Detail.</returns>
        //public IList<ETLScheduleBatchLogDetailModel> GetETLScheduleBatchLogDetail(ETLScheduleBatchLogDetailSearchParameter eTLScheduleBatchLogDetailSearchParameter, string tenantCode, out int noOfRecordCount)
        //{
        //    IList<ETLScheduleBatchLogDetailModel> eTLScheduleBatchLogDetailModels = new List<ETLScheduleBatchLogDetailModel>();
        //    try
        //    {
        //        this.SetAndValidateConnectionString(tenantCode);
        //        string whereClause = this.WhereClauseGeneratorForETLScheduleBatchLogDetail(eTLScheduleBatchLogDetailSearchParameter, tenantCode);

        //        using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
        //        {
        //            IList<EtlLogItems> etlLogItems = new List<EtlLogItems>();
        //            IList<View_ETLScheduleBatchLogDetail> view_ETLScheduleBatchLogDetails = new List<View_ETLScheduleBatchLogDetail>();

        //            if (eTLScheduleBatchLogDetailSearchParameter.PagingParameter.PageIndex > 0 && eTLScheduleBatchLogDetailSearchParameter.PagingParameter.PageSize > 0)
        //            {
        //                view_ETLScheduleBatchLogDetails = nISEntitiesDataContext.View_ETLScheduleBatchLogDetail
        //                .OrderBy(eTLScheduleBatchLogDetailSearchParameter.SortParameter.SortColumn + " " + eTLScheduleBatchLogDetailSearchParameter.SortParameter.SortOrder.ToString())
        //                .Where(whereClause)
        //                .Skip((eTLScheduleBatchLogDetailSearchParameter.PagingParameter.PageIndex - 1) * eTLScheduleBatchLogDetailSearchParameter.PagingParameter.PageSize)
        //                .Take(eTLScheduleBatchLogDetailSearchParameter.PagingParameter.PageSize)
        //                .ToList();
        //            }
        //            else
        //            {
        //                view_ETLScheduleBatchLogDetails = nISEntitiesDataContext.View_ETLScheduleBatchLogDetail
        //                .Where(whereClause)
        //                .OrderBy(eTLScheduleBatchLogDetailSearchParameter.SortParameter.SortColumn + " " + eTLScheduleBatchLogDetailSearchParameter.SortParameter.SortOrder.ToString().ToLower())
        //                .ToList();
        //            }

        //            if (view_ETLScheduleBatchLogDetails != null && view_ETLScheduleBatchLogDetails.Count > 0)
        //            {
        //                eTLScheduleBatchLogDetailModels = view_ETLScheduleBatchLogDetails.Select(eTLScheduleBatchLogDetailRecord => new ETLScheduleBatchLogDetailModel()
        //                {
        //                    Identifier = eTLScheduleBatchLogDetailRecord.Id,
        //                    Schedule = eTLScheduleBatchLogDetailRecord.Schedule,
        //                    Status = eTLScheduleBatchLogDetailRecord.Status,
        //                    ExecutionDate = eTLScheduleBatchLogDetailRecord.ExecutionDate != null ? DateTime.SpecifyKind((DateTime)eTLScheduleBatchLogDetailRecord.ExecutionDate, DateTimeKind.Utc) : DateTime.MinValue,
        //                    Segment = eTLScheduleBatchLogDetailRecord.Segment,
        //                    Language = eTLScheduleBatchLogDetailRecord.Language,
        //                    BatchName = eTLScheduleBatchLogDetailRecord.BatchName,
        //                    LogMessage = eTLScheduleBatchLogDetailRecord.LogMessage,
        //                    ReferenceRecordId = eTLScheduleBatchLogDetailRecord.ReferenceRecordId
        //                }).ToList();
        //            }
        //            noOfRecordCount = nISEntitiesDataContext.View_ETLScheduleBatchLogDetail.Where(whereClause.ToString()).Count();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }

        //    return eTLScheduleBatchLogDetailModels;
        //}

        //#endregion

        #region ETLBatches

        public IList<ETLBatchMasterViewModel> GetETLBatches(long productBatchId, string tenantCode)
        {
            IList<ETLBatchMasterViewModel> batchMasters = new List<ETLBatchMasterViewModel>();
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    var scheduleId = (from bm in nISEntitiesDataContext.BatchMasterRecords
                                      join sc in nISEntitiesDataContext.ScheduleRecords on bm.ScheduleId equals sc.Id
                                      where sc.ProductBatchId == productBatchId
                                      select sc.Id).ToList().FirstOrDefault();

                    var batchesList = (from ebm in nISEntitiesDataContext.EtlBatches
                                       join esc in nISEntitiesDataContext.EtlSchedules on ebm.EtlScheduleId equals esc.Id
                                       where (ebm.ProductBatchId == productBatchId)

                                       select new ETLBatchMasterViewModel
                                       {
                                           Identifier = ebm.Id,
                                           BatchName = ebm.BatchName,
                                           TenantCode = tenantCode,
                                           ETLScheduleId = ebm.EtlScheduleId,
                                           ETLScheduleName = esc.Name,
                                           IsExecuted = ebm.IsExecuted,
                                           BatchExecutionDate = nISEntitiesDataContext.BatchMasterRecords.Where(x => x.ScheduleId == scheduleId && x.DataExtractionDate == ebm.DataExtractionDateTime).Select(x => x.BatchExecutionDate).FirstOrDefault(),
                                           DataExtractionDate = ebm.DataExtractionDateTime,
                                           Status = ebm.Status,
                                           //RecordProcess = nISEntitiesDataContext.EtlLogs.Where(x => x.EtlBatchId == ebm.Id).Select(x => x.TotalNoOfRecordProcessed).FirstOrDefault(),
                                           //RecordRecieved = nISEntitiesDataContext.EtlLogs.Where(x => x.EtlBatchId == ebm.Id).Select(x => x.TotalNoOfRecordReceived).FirstOrDefault(),
                                           IsApproved = ebm.IsApproved,
                                       }).ToList();

                    foreach (var item in batchesList.Select((value, index) => new { index, value }))
                    {
                        batchMasters.Add(new ETLBatchMasterViewModel
                        {
                            Identifier = item.value.Identifier,
                            BatchName = item.value.BatchName,
                            TenantCode = item.value.TenantCode,
                            ETLScheduleId = item.value.ETLScheduleId,
                            ETLScheduleName = item.value.ETLScheduleName,
                            IsExecuted = batchesList.Where(x => x.BatchExecutionDate == item.value.BatchExecutionDate).Select(x => x.IsExecuted).Where(x => x == true).Count() > 0 ? true : false,
                            BatchExecutionDate = item.value.BatchExecutionDate,
                            DataExtractionDate = item.value.DataExtractionDate,
                            Status = item.value.Status,
                            RecordProcess = item.value.RecordProcess,
                            RecordRecieved = item.value.RecordRecieved,
                            IsApproved = item.value.IsApproved
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
        #endregion
        #endregion

        #region RunManualETL

        public async Task<bool> RunETLManually(long batchId, string tenantCode)
        {
            bool result = false;
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    var etlBatch = await nISEntitiesDataContext.EtlBatches.Where(x => x.Id == batchId).FirstOrDefaultAsync();
                    var batchesId = await (from bm in nISEntitiesDataContext.BatchMasterRecords
                                           join sc in nISEntitiesDataContext.ScheduleRecords on bm.ScheduleId equals sc.Id
                                           join eb in nISEntitiesDataContext.EtlBatches on sc.ProductBatchId equals eb.ProductBatchId
                                           where eb.ProductBatchId == etlBatch.ProductBatchId && bm.DataExtractionDate == etlBatch.DataExtractionDateTime
                                           select new { bm.Id }
                                 ).ToListAsync();
                    var currentDate = DateTime.Now;
                    etlBatch.ActualRunDate = currentDate;
                    etlBatch.Status = "Running";
                    List<long> batchIds = new List<long>();
                    foreach (var item in batchesId)
                    {
                        batchIds.Add(Convert.ToInt64(item.Id));
                    }


                    var productNameResult = await (from bm in nISEntitiesDataContext.BatchMasterRecords
                                                   join sc in nISEntitiesDataContext.ScheduleRecords on bm.ScheduleId equals sc.Id
                                                   join pr in nISEntitiesDataContext.Products on sc.ProductId equals pr.Id
                                                   where batchIds.Contains(bm.Id)
                                                   select new { pr.Name }
                                 ).FirstOrDefaultAsync();

                    var productName = productNameResult.Name;

                    var batches = await nISEntitiesDataContext.BatchMasterRecords.Where(x => batchIds.Contains(x.Id)).ToListAsync();
                    //batches.ForEach(item => item.ActualRunDate = currentDate);
                    await nISEntitiesDataContext.SaveChangesAsync();

                    _logger.Info("Calling Stored Procedure: dbo.ExecuteSsisPackages");

                    //Call Stored Procedure for SSIS execution
                    nISEntitiesDataContext.Database.CommandTimeout = 18000;

                    string command = string.Empty;

                    switch (productName)
                    {
                        case "Corporate Saver":
                            command = "dbo.CorporateSaverExecuteSSISPackages";
                            break;
                        case "Home Loan":
                            command = "dbo.HomeLoanExecuteSSISPackages";
                            break;
                        case "Personal Loan":
                            command = "dbo.PersonalLoanExecuteSSISPackages";
                            break;
                        case "Investment":
                            command = "dbo.InvestmentExecuteSSISPackages";
                            break;
                        case "MCA":
                            command = "dbo.MCAExecuteSSISPackages";
                            break;
                        default:
                            break;
                    }
                    if (!string.IsNullOrEmpty(command))
                    {
                        await nISEntitiesDataContext.Database.ExecuteSqlCommandAsync(command);
                    }

                    _logger.Info("Call completed Stored Procedure: " + command);

                    etlBatch = await nISEntitiesDataContext.EtlBatches.Where(x => x.Id == batchId).FirstOrDefaultAsync();
                    etlBatch.Status = "Completed";
                    await nISEntitiesDataContext.SaveChangesAsync();

                    _logger.Info("Set the etlBatch.Status = Completed");

                    result = true;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return result;
        }


        public async Task<bool> RetryETLManually(long batchId, string tenantCode)
        {
            bool result = false;
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    var etlBatch = await nISEntitiesDataContext.EtlBatches.Where(x => x.Id == batchId).FirstOrDefaultAsync();
                    var batchesId = await (from bm in nISEntitiesDataContext.BatchMasterRecords
                                           join sc in nISEntitiesDataContext.ScheduleRecords on bm.ScheduleId equals sc.Id
                                           join eb in nISEntitiesDataContext.EtlBatches on sc.ProductBatchId equals eb.ProductBatchId
                                           where eb.ProductBatchId == etlBatch.ProductBatchId && bm.DataExtractionDate == etlBatch.DataExtractionDateTime
                                           select new { bm.Id }
                                 ).ToListAsync();
                    var currentDate = DateTime.Now;
                    etlBatch.DataExtractionDateTime = currentDate;
                    etlBatch.IsExecuted = false;
                    etlBatch.Status = "Running";
                    List<long> batchIds = new List<long>();
                    foreach (var item in batchesId)
                    {
                        batchIds.Add(Convert.ToInt64(item.Id));
                    }

                    var batches = await nISEntitiesDataContext.BatchMasterRecords.Where(x => batchIds.Contains(x.Id)).ToListAsync();
                    batches.ForEach(item => item.DataExtractionDate = currentDate);
                    await nISEntitiesDataContext.SaveChangesAsync();

                    //Call Stored Procedure for SSIS execution
                    await nISEntitiesDataContext.Database.ExecuteSqlCommandAsync("dbo.ExecuteSsisPackages");

                    //Updating ETL Batch status to completed
                    etlBatch = await nISEntitiesDataContext.EtlBatches.Where(x => x.Id == batchId).FirstOrDefaultAsync();
                    etlBatch.Status = "Completed";
                    await nISEntitiesDataContext.SaveChangesAsync();

                    result = true;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return result;
        }

        public bool ApproveETLBatch(long batchId, string tenantCode)
        {
            bool result = false;
            try
            {
                string productName = string.Empty;
                DateTime batchExecutionDate = new DateTime();
                this.SetAndValidateConnectionString(tenantCode);
                var etlBatch = new EtlBatches();
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    etlBatch = nISEntitiesDataContext.EtlBatches.Where(x => x.Id == batchId).FirstOrDefault();
                    var batchesId = (from bm in nISEntitiesDataContext.BatchMasterRecords
                                     join sc in nISEntitiesDataContext.ScheduleRecords on bm.ScheduleId equals sc.Id
                                     join eb in nISEntitiesDataContext.EtlBatches on sc.ProductBatchId equals eb.ProductBatchId
                                     where eb.ProductBatchId == etlBatch.ProductBatchId && bm.DataExtractionDate == etlBatch.DataExtractionDateTime
                                     select new { bm.Id }
                                 ).ToList();
                    List<long> batchIds = new List<long>();
                    foreach (var item in batchesId)
                    {
                        batchIds.Add(Convert.ToInt64(item.Id));
                    }
                    productName = (from eb in nISEntitiesDataContext.EtlBatches
                                   join esc in nISEntitiesDataContext.EtlSchedules on eb.EtlScheduleId equals esc.Id
                                   join pr in nISEntitiesDataContext.Products on esc.ProductId equals pr.Id
                                   where (eb.Id == etlBatch.Id)
                                   select new
                                   {
                                       pr.Name
                                   }).Select(x => x.Name).FirstOrDefault();

                    var batchesList = nISEntitiesDataContext.BatchMasterRecords.Where(y => batchIds.Contains(y.Id)).ToList();
                    batchExecutionDate = batchesList.FirstOrDefault().BatchExecutionDate;
                    batchesList.ForEach(x => { x.IsDataReady = true; x.IsExecuted = false; x.Status = "New"; });
                    etlBatch.IsApproved = true;
                    etlBatch.Status = BatchStatus.Approved.ToString();
                    nISEntitiesDataContext.SaveChanges();
                    result = true;
                }

                RemoveApprovedData(productName, batchExecutionDate, etlBatch.Id, new List<long>(), false).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw;
            }
            return result;
        }

        #endregion

        public bool DeleteETLBatch(long batchId, string tenantCode)
        {
            bool result = false;
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                DateTime batchExecutionDate = new DateTime();
                string productName = string.Empty;
                List<long> batchIds = new List<long>();
                var etlBatch = new EtlBatches();
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    etlBatch = nISEntitiesDataContext.EtlBatches.Where(x => x.Id == batchId).FirstOrDefault();
                    var batchesId = (from bm in nISEntitiesDataContext.BatchMasterRecords
                                     join sc in nISEntitiesDataContext.ScheduleRecords on bm.ScheduleId equals sc.Id
                                     join eb in nISEntitiesDataContext.EtlBatches on sc.ProductBatchId equals eb.ProductBatchId
                                     where eb.ProductBatchId == etlBatch.ProductBatchId && bm.DataExtractionDate == etlBatch.DataExtractionDateTime
                                     select new { bm.Id }
                                 ).ToList();

                    foreach (var item in batchesId)
                    {
                        batchIds.Add(Convert.ToInt64(item.Id));
                    }

                    productName = (from eb in nISEntitiesDataContext.EtlBatches
                                   join esc in nISEntitiesDataContext.EtlSchedules on eb.EtlScheduleId equals esc.Id
                                   join pr in nISEntitiesDataContext.Products on esc.ProductId equals pr.Id
                                   where (eb.Id == etlBatch.Id)
                                   select new
                                   {
                                       pr.Name
                                   }).Select(x => x.Name).FirstOrDefault();

                    var batchesList = nISEntitiesDataContext.BatchMasterRecords.Where(y => batchIds.Contains(y.Id)).ToList();
                    batchExecutionDate = batchesList.FirstOrDefault().BatchExecutionDate;

                    etlBatch.IsExecuted = false;
                    etlBatch.IsApproved = false;
                    etlBatch.Status = BatchStatus.New.ToString();
                    nISEntitiesDataContext.SaveChanges();
                    result = true;
                }
                RemoveApprovedData(productName, batchExecutionDate, etlBatch.Id, batchIds, true).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw;
            }
            return result;
        }

        private Task RemoveApprovedData(string productName, DateTime statementGenerationDate, long etlBatchId, List<long> batchIds, bool flag)
        {
            var scheduleDate = new DateTime(statementGenerationDate.Year, statementGenerationDate.Month, statementGenerationDate.Day, 0, 0, 0, 0).ToString("yyyy-MM-dd HH:mm:ss.fff");
            try
            {
                using (NISEntities nISEntitiesDataContext = new NISEntities(connectionString))
                {
                    switch (productName?.ToLower())
                    {
                        case "home loan":
                            nISEntitiesDataContext.Database.ExecuteSqlCommand("exec [NB_Delete_HomeLoanIntermediateStorage] '" + scheduleDate + "'," + etlBatchId + "," + (flag ? 1 : 0));
                            if (batchIds.Count() > 0)
                            {
                                nISEntitiesDataContext.Database.ExecuteSqlCommand("exec [NB_Delete_HomeLoanStatementData] '" + string.Join(",", batchIds) + "'");
                            }
                            break;
                        case "investment":
                            nISEntitiesDataContext.Database.ExecuteSqlCommand("exec [NB_Delete_InvestmentIntermediateStorage] '" + scheduleDate + "'," + etlBatchId + "," + (flag ? 1 : 0));
                            if (batchIds.Count() > 0)
                            {
                                nISEntitiesDataContext.Database.ExecuteSqlCommand("exec [NB_Delete_InvestmentStatementData] '" + string.Join(",", batchIds) + "'");
                            }
                            break;
                        case "mca":
                            nISEntitiesDataContext.Database.ExecuteSqlCommand("exec [NB_Delete_MCAIntermediateStorage] '" + scheduleDate + "'," + etlBatchId + "," + (flag ? 1 : 0));
                            if (batchIds.Count() > 0)
                            {
                                nISEntitiesDataContext.Database.ExecuteSqlCommand("exec [NB_Delete_MCAStatementData] '" + string.Join(",", batchIds) + "'");
                            }
                            break;
                        case "personal loan":
                            nISEntitiesDataContext.Database.ExecuteSqlCommand("exec [NB_Delete_PersonalLoanIntermediateStorage] '" + scheduleDate + "'," + etlBatchId + "," + (flag ? 1 : 0));
                            if (batchIds.Count() > 0)
                            {
                                nISEntitiesDataContext.Database.ExecuteSqlCommand("exec [NB_Delete_PersonalLoanStatementData] '" + string.Join(",", batchIds) + "'");
                            }
                            break;
                        case "corporate saver":
                            nISEntitiesDataContext.Database.ExecuteSqlCommand("exec [NB_Delete_CorporateSaverIntermediateStorage] '" + scheduleDate + "'," + etlBatchId + "," + (flag ? 1 : 0));
                            if (batchIds.Count() > 0)
                            {
                                nISEntitiesDataContext.Database.ExecuteSqlCommand("exec [NB_Delete_CorporateSaverStatementData] '" + string.Join(",", batchIds) + "'");
                            }
                            break;
                    }
                }
            }
            catch (Exception exception)
            {
                throw;
            }

            return Task.CompletedTask;
        }
    }
}
