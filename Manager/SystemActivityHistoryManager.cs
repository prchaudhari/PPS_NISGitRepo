// <copyright file="SystemActivityHistoryManager.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2021 Websym Solutions Pvt. Ltd..
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
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Text.RegularExpressions;
    using Unity;
    #endregion

    public class SystemActivityHistoryManager
    {
        #region Private members
        /// <summary>
        /// The unity container
        /// </summary>
        private IUnityContainer unityContainer = null;

        /// <summary>
        /// The validation engine object
        /// </summary>
        IValidationEngine validationEngine = null;

        /// <summary>
        /// The utility object
        /// </summary>
        private IUtility utility = null;

        /// <summary>
        /// The system activity history repository.
        /// </summary>
        ISQLSystemActivityHistoryRepository systemActivityHistoryRepository = null;

        #endregion

        #region Constructor

        /// <summary>
        /// This is constructor for system activity history manager, which initialise system activity history repository.
        /// </summary>
        /// <param name="unityContainer">IUnity container implementation object.</param>
        public SystemActivityHistoryManager(IUnityContainer unityContainer)
        {
            try
            {
                this.unityContainer = unityContainer;
                this.validationEngine = new ValidationEngine();
                this.utility = new Utility();
                this.systemActivityHistoryRepository = this.unityContainer.Resolve<ISQLSystemActivityHistoryRepository>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
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
            try
            {
                return this.systemActivityHistoryRepository.GetSystemActivityHistories(tenantCode);
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
                return this.systemActivityHistoryRepository.SaveSystemActivityHistoryDetails(activityHistories, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }
}
