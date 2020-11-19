// <copyright file="ScheduleManager.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2018 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------

namespace nIS
{
    #region References

    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using Unity;

    #endregion

    /// <summary>
    /// This class implements manager layer of schedule manager.
    /// </summary>
    public class ScheduleManager
    {
        #region Private members
        /// <summary>
        /// The unity container
        /// </summary>
        private IUnityContainer unityContainer = null;

        /// <summary>
        /// The schedule repository.
        /// </summary>
        IScheduleRepository scheduleRepository = null;

        /// <summary>
        /// The Client manager.
        /// </summary>
        private ClientManager clientManager = null;

        #endregion

        #region Constructor

        /// <summary>
        /// This is constructor for schedule manager, which initialise
        /// schedule repository.
        /// </summary>
        /// <param name="container">IUnity container implementation object.</param>
        public ScheduleManager(IUnityContainer unityContainer)
        {
            try
            {
                this.unityContainer = unityContainer;
                this.scheduleRepository = this.unityContainer.Resolve<IScheduleRepository>();
                this.clientManager = this.unityContainer.Resolve<ClientManager>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Public Methods

        #region Schedule 
        /// <summary>
        /// This method will call add schedules method of repository.
        /// </summary>
        /// <param name="schedules">Schedules are to be add.</param>
        /// <param name="tenantCode">Tenant code of schedule.</param>
        /// <returns>
        /// Returns true if entities added successfully, false otherwise.
        /// </returns>
        public bool AddSchedules(IList<Schedule> schedules, string tenantCode)
        {
            bool result = false;
            try
            {
                this.IsValidSchedules(schedules, tenantCode);
                this.IsDuplicateSchedule(schedules, tenantCode);
                result = this.scheduleRepository.AddSchedules(schedules, tenantCode);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method will call update schedules method of repository
        /// </summary>
        /// <param name="schedules">Schedules are to be update.</param>
        /// <param name="tenantCode">Tenant code of schedule.</param>
        /// <returns>
        /// Returns true if schedules updated successfully, false otherwise.
        /// </returns>
        public bool UpdateSchedules(IList<Schedule> schedules, string tenantCode)
        {
            bool result = false;
            try
            {
                this.IsValidSchedules(schedules, tenantCode);
                this.IsDuplicateSchedule(schedules, tenantCode);
                result = this.scheduleRepository.UpdateSchedules(schedules, tenantCode);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method will call delete schedules method of repository
        /// </summary>
        /// <param name="schedules">Schedules are to be delete.</param>
        /// <param name="tenantCode">Tenant code of schedule.</param>
        /// <returns>
        /// Returns true if schedules deleted successfully, false otherwise.
        /// </returns>
        public bool DeleteSchedules(IList<Schedule> schedules, string tenantCode)
        {
            bool result = false;
            try
            {
                result = this.scheduleRepository.DeleteSchedules(schedules, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return result;
        }

        /// <summary>
        /// This method will call get schedules method of repository.
        /// </summary>
        /// <param name="scheduleSearchParameter">The schedule search parameters.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns>
        /// Returns schedules if found for given parameters, else return null
        /// </returns>
        public IList<Schedule> GetSchedules(ScheduleSearchParameter scheduleSearchParameter, string tenantCode)
        {
            IList<Schedule> schedules = new List<Schedule>();
            try
            {
                InvalidSearchParameterException invalidSearchParameterException = new InvalidSearchParameterException(tenantCode);
                try
                {
                    scheduleSearchParameter.IsValid();
                }
                catch (Exception exception)
                {
                    invalidSearchParameterException.Data.Add("InvalidPagingParameter", exception.Data);
                }

                if (invalidSearchParameterException.Data.Count > 0)
                {
                    throw invalidSearchParameterException;
                }

                schedules = this.scheduleRepository.GetSchedules(scheduleSearchParameter, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return schedules;
        }

        /// <summary>
        /// This method helps to get count of schedules.
        /// </summary>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns>
        /// Returns count of schedules
        /// </returns>
        public int GetScheduleCount(ScheduleSearchParameter scheduleSearchParameter, string tenantCode)
        {
            int scheduleCount = 0;
            try
            {
                scheduleCount = this.scheduleRepository.GetScheduleCount(scheduleSearchParameter, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return scheduleCount;
        }

        /// <summary>
        /// This method helps to activate the customers
        /// </summary>
        /// <param name="scheduleIdentifier">The schedule identifier</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>True if schedule activated successfully false otherwise</returns>
        public bool ActivateSchedule(long scheduleIdentifier, string tenantCode)
        {
            bool result = false;
            try
            {
                this.scheduleRepository.ActivateSchedule(scheduleIdentifier, tenantCode);
                result = true;
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return result;
        }

        /// <summary>
        /// This method helps to deactivate the schedule
        /// </summary>
        /// <param name="scheduleIdentifier">The schedule identifier</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>True if schedule deactivated successfully false otherwise</returns>
        public bool DeactivateSchedule(long scheduleIdentifier, string tenantCode)
        {
            bool result = false;
            try
            {
                this.scheduleRepository.DeActivateSchedule(scheduleIdentifier, tenantCode);
                result = true;
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return result;
        }
        #endregion

        #region ScheduleRunHistory 
        /// <summary>
        /// This method will call add schedules method of repository.
        /// </summary>
        /// <param name="schedules">ScheduleRunHistorys are to be add.</param>
        /// <param name="tenantCode">Tenant code of schedule.</param>
        /// <returns>
        /// Returns true if entities added successfully, false otherwise.
        /// </returns>
        public bool AddScheduleRunHistorys(IList<ScheduleRunHistory> schedules, string tenantCode)
        {
            bool result = false;
            try
            {
                result = this.scheduleRepository.AddScheduleRunHistorys(schedules, tenantCode);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method will call get schedules method of repository.
        /// </summary>
        /// <param name="scheduleSearchParameter">The schedule search parameters.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns>
        /// Returns schedules if found for given parameters, else return null
        /// </returns>
        public IList<ScheduleRunHistory> GetScheduleRunHistorys(ScheduleSearchParameter scheduleSearchParameter, string tenantCode)
        {
            IList<ScheduleRunHistory> schedulesHistory = new List<ScheduleRunHistory>();
            try
            {
                InvalidSearchParameterException invalidSearchParameterException = new InvalidSearchParameterException(tenantCode);
                try
                {
                    scheduleSearchParameter.IsValid();
                }
                catch (Exception exception)
                {
                    invalidSearchParameterException.Data.Add("InvalidPagingParameter", exception.Data);
                }

                if (invalidSearchParameterException.Data.Count > 0)
                {
                    throw invalidSearchParameterException;
                }

                schedulesHistory = this.scheduleRepository.GetScheduleRunHistorys(scheduleSearchParameter, tenantCode);
                if (schedulesHistory?.Count > 0)
                {
                    ScheduleSearchParameter parameter = new ScheduleSearchParameter();
                    parameter.Identifier = string.Join(",", schedulesHistory.Select(item => item.Schedule.Identifier).ToList());
                    parameter.IsStatementDefinitionRequired = true;
                    parameter.SortParameter.SortColumn = "Id";
                    IList<Schedule> schedules = new List<Schedule>();
                    schedules = this.GetSchedules(parameter, tenantCode);
                    schedulesHistory.ToList().ForEach(item =>
                    {
                        item.Schedule = schedules.Where(i => i.Identifier == item.Schedule.Identifier)?.FirstOrDefault();
                    });
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return schedulesHistory;
        }

        /// <summary>
        /// This method helps to get count of schedules.
        /// </summary>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns>
        /// Returns count of schedules
        /// </returns>
        public int GetScheduleRunHistoryCount(ScheduleSearchParameter scheduleSearchParameter, string tenantCode)
        {
            int scheduleCount = 0;
            try
            {
                scheduleCount = this.scheduleRepository.GetScheduleRunHistoryCount(scheduleSearchParameter, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return scheduleCount;
        }

        /// <summary>
        /// This method helps to run the schedule
        /// </summary>
        /// <param name="baseURL">The base URL</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>True if schedules runs successfully, false otherwise</returns>
        public bool RunSchedule(string baseURL, string outputLocation, TenantConfiguration tenantConfiguration, string tenantCode)
        {
            try
            {
                ClientSearchParameter clientSearchParameter = new ClientSearchParameter
                {
                    TenantCode = tenantCode,
                    IsCountryRequired = false,
                    IsContactRequired = false,
                    PagingParameter = new PagingParameter
                    {
                        PageIndex = 0,
                        PageSize = 0,
                    },
                    SortParameter = new SortParameter()
                    {
                        SortOrder = SortOrder.Ascending,
                        SortColumn = "Id",
                    },
                    SearchMode = SearchMode.Equals
                };
                var client = this.clientManager.GetClients(clientSearchParameter, tenantCode).FirstOrDefault();
                var parallelThreadCount = int.Parse(ConfigurationManager.AppSettings["ThreadCountToGenerateStatementParallel"]);
                return this.scheduleRepository.RunScheduleNew(baseURL, outputLocation, tenantCode, parallelThreadCount,tenantConfiguration, client);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method helps to run the schedule now
        /// </summary>
        /// <param name="batchMaster">The batch object</param>
        /// <param name="baseURL">The base URL</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>True if schedules runs successfully, false otherwise</returns>
        public bool RunScheduleNow(BatchMaster batchMaster, string baseURL, string outputLocation, TenantConfiguration tenantConfiguration, string tenantCode)
        {
            try
            {
                ClientSearchParameter clientSearchParameter = new ClientSearchParameter
                {
                    TenantCode = tenantCode,
                    IsCountryRequired = false,
                    IsContactRequired = false,
                    PagingParameter = new PagingParameter
                    {
                        PageIndex = 0,
                        PageSize = 0,
                    },
                    SortParameter = new SortParameter()
                    {
                        SortOrder = SortOrder.Ascending,
                        SortColumn = "Id",
                    },
                    SearchMode = SearchMode.Equals
                };
                var client = this.clientManager.GetClients(clientSearchParameter, tenantCode).FirstOrDefault();
                var parallelThreadCount = int.Parse(ConfigurationManager.AppSettings["ThreadCountToGenerateStatementParallel"]);
                return this.scheduleRepository.RunScheduleNow(batchMaster, baseURL, outputLocation, tenantCode, parallelThreadCount, tenantConfiguration, client);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Batch master
        /// <summary>
        /// This method will call get schedules method of repository.
        /// </summary>
        /// <param name="scheduleSearchParameter">The schedule search parameters.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns>
        /// Returns schedules if found for given parameters, else return null
        /// </returns>
        public IList<BatchMaster> GetBatchMasters(long scheduleIdentifer, string tenantCode)
        {
            IList<BatchMaster> batchMasters = new List<BatchMaster>();
            try
            {
                batchMasters = this.scheduleRepository.GetBatchMasters(scheduleIdentifer, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return batchMasters;
        }
        #endregion

        #endregion

        #region Private Methods

        /// <summary>
        /// This method is responsible for validate schedules.
        /// </summary>
        /// <param name="schedules"></param>
        /// <param name="tenantCode"></param>
        private void IsValidSchedules(IList<Schedule> schedules, string tenantCode)
        {
            try
            {
                if (schedules?.Count <= 0)
                {
                    throw new NullArgumentException(tenantCode);
                }

                InvalidScheduleException invalidScheduleException = new InvalidScheduleException(tenantCode);
                schedules.ToList().ForEach(item =>
                {
                    try
                    {
                        item.IsValid();
                    }
                    catch (Exception ex)
                    {
                        invalidScheduleException.Data.Add(item.Name, ex.Data);
                    }
                });

                if (invalidScheduleException.Data.Count > 0)
                {
                    throw invalidScheduleException;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method helps to check duplicate schedule in the list
        /// </summary>
        /// <param name="schedules"></param>
        /// <param name="tenantCode"></param>
        private void IsDuplicateSchedule(IList<Schedule> schedules, string tenantCode)
        {
            try
            {
                int isDuplicateSchedule = schedules.GroupBy(p => p.Name).Where(g => g.Count() > 1).Count();
                if (isDuplicateSchedule > 0)
                {
                    throw new DuplicateScheduleFoundException(tenantCode);
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        #endregion
    }
}
