// <copyright file="ETLScheduleManager.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2018 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------

namespace nIS
{
    using nIS.Models.NedBank;
    using nIS.NedBank;
    #region References

    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Unity;

    #endregion

    /// <summary>
    /// This class implements manager layer of DataHub manager.
    /// </summary>
    public class ETLScheduleManager
    {
        #region Private members

        /// <summary>
        /// The unity container
        /// </summary>
        private IUnityContainer unityContainer = null;

        /// <summary>
        /// The tenant config manager object.
        /// </summary>
        private TenantConfigurationManager tenantConfigurationManager = null;

        /// <summary>
        /// The ETL schedule repository.
        /// </summary>
        IETLScheduleRepository eTLScheduleRepository = null;

        #endregion

        #region Constructor

        /// <summary>
        /// This is constructor for schedule manager, which initialise
        /// schedule repository.
        /// </summary>
        /// <param name="unityContainer">The unity container.</param>
        public ETLScheduleManager(IUnityContainer unityContainer)
        {
            try
            {
                this.unityContainer = unityContainer;
                this.eTLScheduleRepository = this.unityContainer.Resolve<IETLScheduleRepository>();
                this.tenantConfigurationManager = this.unityContainer.Resolve<TenantConfigurationManager>();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        #endregion

        #region ETLSchedule

        /// <summary>
        /// This method will call get ETL schedules method of repository.
        /// </summary>
        /// <param name="eTLScheduleSearchParameter">The schedule search parameters.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns>Returns etl schedules if found for given parameters, else return null.</returns>
        public IList<ETLScheduleListModel> GetETLSchedule(ETLScheduleSearchParameter eTLScheduleSearchParameter, string tenantCode)
        {
            IList<ETLScheduleListModel> eTLSchedules = new List<ETLScheduleListModel>();
            try
            {
                InvalidSearchParameterException invalidSearchParameterException = new InvalidSearchParameterException(tenantCode);
                try
                {
                    eTLScheduleSearchParameter.IsValid();
                }
                catch (Exception exception)
                {
                    invalidSearchParameterException.Data.Add("InvalidPagingParameter", exception.Data);
                }

                if (invalidSearchParameterException.Data.Count > 0)
                {
                    throw invalidSearchParameterException;
                }

                eTLSchedules = this.eTLScheduleRepository.GetETLSchedules(eTLScheduleSearchParameter, tenantCode);
            }
            catch (Exception exception)
            {
                throw;
            }
            return eTLSchedules;
        }

        public int GetETLScheduleCount(ETLScheduleSearchParameter scheduleSearchParameter, string tenantCode)
        {
            int scheduleCount = 0;
            try
            {
                scheduleCount = this.eTLScheduleRepository.GetETLScheduleCountWithProduct(scheduleSearchParameter, tenantCode);
            }
            catch (Exception exception)
            {
                throw;
            }
            return scheduleCount;
        }

        #endregion

        #region ETLScheduleDetail

        /// <summary>
        /// This method will call get ETL schedules detail method of repository.
        /// </summary>
        /// <param name="eTLScheduleSearchParameter"></param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <param name="recordCount">out parameter for total of record in database's table</param>
        /// <returns>Returns etl schedules detail if found for given parameters, else return null</returns>
        public IList<ETLScheduleListModel> GetETLScheduleDetailByProduct(ETLScheduleSearchParameter eTLScheduleSearchParameter, string tenantCode, out int recordCount)
        {
            IList<ETLScheduleListModel> eTLScheduleDetails = new List<ETLScheduleListModel>();
            try
            {
                InvalidSearchParameterException invalidSearchParameterException = new InvalidSearchParameterException(tenantCode);
                try
                {
                    eTLScheduleSearchParameter.IsValid();
                }
                catch (Exception exception)
                {
                    invalidSearchParameterException.Data.Add("InvalidPagingParameter", exception.Data);
                }

                if (invalidSearchParameterException.Data.Count > 0)
                {
                    throw invalidSearchParameterException;
                }

                eTLScheduleDetails = this.eTLScheduleRepository.GetETLScheduleDetailByProduct(eTLScheduleSearchParameter, tenantCode, out int noOfRecordCount);
                recordCount = noOfRecordCount;
            }
            catch (Exception exception)
            {
                throw;
            }
            return eTLScheduleDetails;
        }

        #endregion

        #region ETLScheduleBatchLogList

        /// <summary>
        /// This method gets the specified list of ETL Schedule Batch Log method of repository.
        /// </summary>
        /// <param name="eTLscheduleLogSearchParameter">The Etl Schedule Batch Log search parameter.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <param name="recordCount">out parameter for total of record in database's table.</param>
        /// <returns>Returns etl schedules batch log's list if found for given parameters, else return null.</returns>
        public IList<ETLScheduleBatchLogModel> GetETLScheduleBatchLogs(ETLScheduleBatchLogSearchParameter eTLscheduleLogSearchParameter, string tenantCode, out int recordCount)
        {
            IList<ETLScheduleBatchLogModel> eTLscheduleLogs = new List<ETLScheduleBatchLogModel>();
            try
            {
                InvalidSearchParameterException invalidSearchParameterException = new InvalidSearchParameterException(tenantCode);
                try
                {
                    eTLscheduleLogSearchParameter.IsValid();
                }
                catch (Exception exception)
                {
                    invalidSearchParameterException.Data.Add("InvalidPagingParameter", exception.Data);
                }
                if (invalidSearchParameterException.Data.Count > 0)
                {
                    throw invalidSearchParameterException;
                }
                eTLscheduleLogs = this.eTLScheduleRepository.GetETLScheduleBatchLogs(eTLscheduleLogSearchParameter, tenantCode, out int noOfRecordCount);
                recordCount = noOfRecordCount;
            }
            catch (Exception exception)
            {
                throw;
            }
            return eTLscheduleLogs;
        }

        #endregion

        //#region ETLScheduleBatchLogDetail

        ///// <summary>
        ///// This method will call get ETL Schedule Detail For Batch Log Detail method of repository.
        ///// </summary>
        ///// <param name="eTLScheduleBatchLogDetailSearchParameter">the etl schedules batch log detail parameters.</param>
        ///// <param name="tenantCode">The tenant code.</param>
        ///// <returns>Returns etl schedules batch log detail's list if found for given parameters, else return null.</returns>
        //public ETLScheduleDetailForBatchLogDetail GetETLScheduleDetailForBatchLogDetail(ETLScheduleDetailForBatchLogDetailSearchParameter eTLScheduleDetailForBatchLogDetailSearchParameter, string tenantCode)
        //{
        //    ETLScheduleDetailForBatchLogDetail eTLScheduleDetailForBatchLogDetail = new ETLScheduleDetailForBatchLogDetail();
        //    try
        //    {
        //        InvalidSearchParameterException invalidSearchParameterException = new InvalidSearchParameterException(tenantCode);
        //        try
        //        {
        //            eTLScheduleDetailForBatchLogDetailSearchParameter.IsValid();
        //        }
        //        catch (Exception exception)
        //        {
        //            invalidSearchParameterException.Data.Add("InvalidPagingParameter", exception.Data);
        //        }

        //        if (invalidSearchParameterException.Data.Count > 0)
        //        {
        //            throw invalidSearchParameterException;
        //        }

        //        eTLScheduleDetailForBatchLogDetail = this.eTLScheduleRepository.GetETLScheduleDetailForBatchLogDetail(eTLScheduleDetailForBatchLogDetailSearchParameter, tenantCode);
        //    }
        //    catch (Exception exception)
        //    {
        //        throw;
        //    }
        //    return eTLScheduleDetailForBatchLogDetail;
        //}

        ///// <summary>
        ///// This method will call get ETL schedules batch log detail method of repository.
        ///// </summary>
        ///// <param name="eTLScheduleBatchLogDetailSearchParameter">the etl schedules batch log detail parameters.</param>
        ///// <param name="tenantCode">The tenant code.</param>
        ///// <param name="recordCount">out parameter for total of record in database's table.</param>
        ///// <returns>Returns etl schedules batch log detail's list if found for given parameters, else return null.</returns>
        //public IList<ETLScheduleBatchLogDetailModel> GetETLScheduleBatchLogDetail(ETLScheduleBatchLogDetailSearchParameter eTLScheduleBatchLogDetailSearchParameter, string tenantCode, out int recordCount)
        //{
        //    IList<ETLScheduleBatchLogDetailModel> eTLSchedules = new List<ETLScheduleBatchLogDetailModel>();
        //    try
        //    {
        //        InvalidSearchParameterException invalidSearchParameterException = new InvalidSearchParameterException(tenantCode);
        //        try
        //        {
        //            eTLScheduleBatchLogDetailSearchParameter.IsValid();
        //        }
        //        catch (Exception exception)
        //        {
        //            invalidSearchParameterException.Data.Add("InvalidPagingParameter", exception.Data);
        //        }

        //        if (invalidSearchParameterException.Data.Count > 0)
        //        {
        //            throw invalidSearchParameterException;
        //        }

        //        eTLSchedules = this.eTLScheduleRepository.GetETLScheduleBatchLogDetail(eTLScheduleBatchLogDetailSearchParameter, tenantCode, out int noOfRecordCount);
        //        recordCount = noOfRecordCount;
        //    }
        //    catch (Exception exception)
        //    {
        //        throw;
        //    }
        //    return eTLSchedules;
        //}

        //#endregion

        #region Batches
        public IList<ETLBatchMasterViewModel> GetETLBatches(long productBatchId, string tenantCode)
        {
            IList<ETLBatchMasterViewModel> batchMasters = new List<ETLBatchMasterViewModel>();
            try
            {
                batchMasters = this.eTLScheduleRepository.GetETLBatches(productBatchId, tenantCode);
            }
            catch (Exception exception)
            {
                throw;
            }
            return batchMasters;
        }
        #endregion

        #region RunETLManually
        public async Task<bool> RunETLManually(long batchId, string tenantCode)
        {
            bool result = false;
            try
            {
                result = await this.eTLScheduleRepository.RunETLManually(batchId, tenantCode);
            }
            catch (Exception exception)
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
                result = await this.eTLScheduleRepository.RetryETLManually(batchId, tenantCode);
            }
            catch (Exception exception)
            {
                throw;
            }
            return result;
        }
        #endregion

        #region ETLBatchApproved
        public bool ApproveETLBatch(long batchId, string tenantCode)
        {
            bool result = false;
            try
            {
                result = this.eTLScheduleRepository.ApproveETLBatch(batchId, tenantCode);
            }
            catch (Exception exception)
            {
                throw;
            }
            return result;
        }
        #endregion

        #region DeleteETLBatch
        public bool DeleteETLBatch(long batchId, string tenantCode)
        {
            bool result = false;
            try
            {
                result = this.eTLScheduleRepository.DeleteETLBatch(batchId, tenantCode);
            }
            catch (Exception exception)
            {
                throw;
            }
            return result;
        }
        #endregion
    }
}
