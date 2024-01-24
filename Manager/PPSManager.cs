// <copyright file="ProductManager.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2018 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------

namespace nIS
{
    #region References

    using System;
    using System.Collections.Generic;
    using Unity;

    #endregion

    public class PPSManager
    {
        #region Private members
        /// <summary>
        /// The unity container
        /// </summary>
        private IUnityContainer unityContainer = null;

        /// <summary>
        /// The product repository.
        /// </summary>
        IPPSRepository ppsRepository = null;

        #endregion

        #region Constructor

        public PPSManager(IUnityContainer unityContainer)
        {
            try
            {
                this.unityContainer = unityContainer;
                this.ppsRepository = this.unityContainer.Resolve<IPPSRepository>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Public Methods
        public IList<spIAA_PaymentDetail> GetFSPStoredProcData(string tenantCode)
        {
            try
            {
                var result = this.ppsRepository.spIAA_PaymentDetail_fspstatement(tenantCode);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }
}
