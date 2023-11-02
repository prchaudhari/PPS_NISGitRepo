// <copyright file="IETLScheduleRepository.cs" company="Websym Solutions Pvt Ltd">
// Copyright (c) 2018 Websym Solutions Pvt Ltd.
// </copyright>
// ----------------------------------------------------------------------- 

using nIS;
using nIS.Models;
using nIS.Models.NedBank;
using nIS.NedBank;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace nIS
{
    public interface IETLScheduleRepository
    {
        /// <summary>
        /// This method gets the specified list of ETL Schedule from ETLSchedule repository.
        /// </summary>
        /// <param name="eTLScheduleSearchParameter">The Etl Schedule search parameter.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns>Returns the list of ETL Schedule.</returns>
        IList<ETLScheduleListModel> GetETLSchedules(ETLScheduleSearchParameter eTLScheduleSearchParameter, string tenantCode);

        int GetETLScheduleCountWithProduct(ETLScheduleSearchParameter eTLScheduleSearchParameter, string tenantCode);

        /// <summary>
        /// This method will call get ETL schedules detail method of repository.
        /// </summary>
        /// <param name="eTLScheduleSearchParameter"></param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <param name="noOfRecordCount">out parameter for total of record in database's table</param>
        /// <returns>Returns etl schedules detail if found for given parameters, else return null</returns>
        IList<ETLScheduleListModel> GetETLScheduleDetailByProduct(ETLScheduleSearchParameter eTLScheduleSearchParameter, string tenantCode, out int noOfRecordCount);

        //#region ETLScheduleBatchLog

        ///// <summary>
        ///// This method gets the specified list of ETL Schedule Batch Log from ETLSchedule repository.
        ///// </summary>
        ///// <param name="eTLScheduleBatchLogSearchParameter">The Etl Schedule Batch Log search parameter.</param>
        ///// <param name="tenantCode">The tenant code</param>
        ///// <param name="noOfRecordCount">out parameter for total of record in database's table.</param>
        ///// <returns>Returns the list of Etl Schedule Batch Log.</returns>
        //IList<ETLScheduleBatchLogModel> GetETLScheduleBatchLogs(ETLScheduleBatchLogSearchParameter eTLScheduleBatchLogSearchParameter, string tenantCode, out int noOfRecordCount);

        //#endregion

        //#region ETLScheduleBatchLogDetail

        ///// <summary>
        ///// This method gets the specified list of ETL Schedule Detail For Batch Log Detail from ETLSchedule repository.
        ///// </summary>
        ///// <param name="eTLScheduleDetailForBatchLogDetailSearchParameter">The Etl Schedule Batch Log Detail search parameter.</param>
        ///// <param name="tenantCode">The tenant code</param>
        ///// <returns>Returns the object of ETL Schedule Batch Log Detail.</returns>
        //ETLScheduleDetailForBatchLogDetail GetETLScheduleDetailForBatchLogDetail(ETLScheduleDetailForBatchLogDetailSearchParameter eTLScheduleDetailForBatchLogDetailSearchParameter, string tenantCode);

        ///// <summary>
        ///// This method gets the specified list of ETL Schedule Batch Log Detail from ETLSchedule repository.
        ///// </summary>
        ///// <param name="eTLScheduleBatchLogDetailSearchParameter">The Etl Schedule Batch Log Detail search parameter.</param>
        ///// <param name="tenantCode">The tenant code</param>
        ///// <param name="noOfRecordCount">out parameter for total of record in database's table.</param>
        ///// <returns>Returns the list of ETL Schedule Batch Log Detail.</returns>
        //IList<ETLScheduleBatchLogDetailModel> GetETLScheduleBatchLogDetail(ETLScheduleBatchLogDetailSearchParameter eTLScheduleBatchLogDetailSearchParameter, string tenantCode, out int noOfRecordCount);

        //#endregion

        IList<ETLBatchMasterViewModel> GetETLBatches(long productBatchId, string tenantCode);

        Task<bool> RunETLManually(long batchId, string tenantCode);

        Task<bool> RetryETLManually(long batchId, string tenantCode);

        bool ApproveETLBatch(long batchId, string tenantCode);

        bool DeleteETLBatch(long batchId, string tenantCode);
    }
}
