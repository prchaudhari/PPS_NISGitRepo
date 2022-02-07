// <copyright file="BranchController.cs" company="Websym Solutions Pvt Ltd">
// Copyright (c) 2018 Websym Solutions Pvt Ltd.
// </copyright>
// -----------------------------------------------------------------------  

namespace NedbankAPI.Controllers
{
    #region References

    using NedBankManager;
    using NedbankModel;
    using NedbankRepository;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http;
    using Unity;

    #endregion

    /// <summary>
    /// This class represent api controller for NedBank
    /// </summary>
    /// <seealso cref="System.Web.Http.ApiController" />
    [RoutePrefix("Branch")]
    public class BranchController : ApiController
    {
        #region Private Members

        /// <summary>
        /// The branch manager object.
        /// </summary>
        private BranchManager branchManager = null;

        /// <summary>
        /// The unity container
        /// </summary>
        private readonly IUnityContainer unityContainer = null;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="BranchController"/> class.
        /// </summary>
        /// <param name="unityContainer">The unity container.</param>
        public BranchController(IUnityContainer unityContainer)
        {
            this.unityContainer = unityContainer;
            this.branchManager = new BranchManager(this.unityContainer);
        }

        #endregion

        #region Get

        #region List

        /// <summary>
        /// This api call use to get single or list of branch
        /// </summary>
        /// <param name="branchId">The branch identifier.</param>
        /// <returns>
        /// Returns list of branchs
        /// </returns>
        [HttpGet]
        public BranchInformation Get(long branchId)
        {
            BranchInformation branch = new BranchInformation();
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                
                branch = this.branchManager.GetbranchsByBranchId(branchId, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return branch;
        }

        #endregion
        #endregion
    }
}
