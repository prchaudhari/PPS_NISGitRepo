// <copyright file="SQLSystemActivityHistoryRepository.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2021 Websym Solutions Pvt. Ltd..
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
    using System.Drawing.Imaging;
    using System.Dynamic;
    using System.IO;
    using System.Linq;
    using System.Linq.Dynamic;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Security.Claims;
    using System.Text;
    using System.Text.RegularExpressions;
    using Unity;
    #endregion

    public class SQLSystemActivityHistoryRepository: ISQLSystemActivityHistoryRepository
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

        #endregion

        #region Constructor

        public SQLSystemActivityHistoryRepository(IUnityContainer unityContainer)
        {
            this.unityContainer = unityContainer;
            this.validationEngine = new ValidationEngine();
            this.utility = new Utility();
            this.configurationutility = new ConfigurationUtility(this.unityContainer);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// This method help to get system activity history records
        /// </summary>
        /// <param name="tenantCode"> the tenant code </param>
        /// <returns>list of system activity history records</returns>
        public IList<SystemActivityHistory> GetSystemActivityHistories(string tenantCode)
        {
            IList<SystemActivityHistory> systemActivityHistories = new List<SystemActivityHistory>();
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    var records = nISEntitiesDataContext.SystemActivityHistoryRecords.Where(it => it.TenantCode == tenantCode).ToList();
                    if (records != null && records.Count > 0)
                    {
                        records.ForEach(it => systemActivityHistories.Add(new SystemActivityHistory()
                        {
                            Identifier = it.Id,
                            Module = it.Module,
                            EntityId = it.EntityId,
                            EntityName = it.EntityName,
                            SubEntityId = it.SubEntityId,
                            SubEntityName = it.SubEntityName,
                            ActionTaken = it.ActionTaken,
                            ActionTakenBy = it.ActionTakenBy,
                            ActionTakenByUserName = it.ActionTakenByUserName,
                            ActionTakenDate = it.ActionTakenDate,
                            TenantCode = it.TenantCode
                        }));
                    }
                }
                return systemActivityHistories;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method help to save system activity history
        /// </summary>
        /// <param name="activityHistories"> the list of activity history records </param>
        /// <param name="tenantCode"> the tenant code </param>
        /// <returns>true if save successfully, otherwise false</returns>
        public bool SaveSystemActivityHistoryDetails(IList<SystemActivityHistory> activityHistories, string tenantCode)
        {
            try
            {
                var claims = ClaimsPrincipal.Current.Identities.First().Claims.ToList();
                int userId;
                int.TryParse(claims?.FirstOrDefault(x => x.Type.Equals("UserId", StringComparison.OrdinalIgnoreCase)).Value, out userId);
                var userFullName = claims?.FirstOrDefault(x => x.Type.Equals("UserFullName", StringComparison.OrdinalIgnoreCase)).Value;

                this.SetAndValidateConnectionString(tenantCode);
                IList<SystemActivityHistoryRecord> activityHistoryRecords = new List<SystemActivityHistoryRecord>();

                activityHistories.ForEach(it => activityHistoryRecords.Add(
                    new SystemActivityHistoryRecord()
                    {
                        Module = it.Module,
                        EntityId = it.EntityId,
                        EntityName = it.EntityName,
                        SubEntityId = it.SubEntityId,
                        SubEntityName = it.SubEntityName,
                        ActionTaken = it.ActionTaken,
                        ActionTakenBy = userId,
                        ActionTakenByUserName = userFullName,
                        ActionTakenDate = DateTime.Now,
                        TenantCode = tenantCode
                    })
                );
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    nISEntitiesDataContext.SystemActivityHistoryRecords.AddRange(activityHistoryRecords);
                    nISEntitiesDataContext.SaveChanges();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Private Methods

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
    }
}
