// <copyright file="TenantUserController.cs" company="Websym Solutions Pvt Ltd">
// Copyright (c) 2018 Websym Solutions Pvt Ltd.
// </copyright>
// -----------------------------------------------------------------------  

namespace nIS
{

    #region References

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Http;
    using System.Web.Http.Cors;
    using Unity;

    #endregion

    /// <summary>
    /// This class represent api controller for user
    /// </summary>
    [RoutePrefix("TenantUser")]
    public class TenantUserController : ApiController
    {
        #region Private Members

        /// <summary>
        /// The role manager object.
        /// </summary>
        private TenantUserManager userManager = null;

        /// <summary>
        /// The unity container
        /// </summary>
        private readonly IUnityContainer unityContainer = null;

        #endregion

        #region Constructor

        public TenantUserController(IUnityContainer unityContainer)
        {
            this.unityContainer = unityContainer;
            this.userManager = new TenantUserManager(this.unityContainer);
        }

        #endregion

        #region Public Methods

        #region Add

        /// <summary>
        /// This api call use to add single or list of user
        /// </summary>
        /// <param name="users">
        /// List of users
        /// </param>
        /// <returns>Returns true if added succesfully otherwise false</returns>
        [HttpPost]
        public bool Add(IList<TenantUser> users)
        {
            bool result = false;
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                result = this.userManager.AddTenantUsers(users, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return result;
        }

        #endregion

        #region Update

        /// <summary>
        /// This api call use to update single or list of user
        /// </summary>
        /// <param name="users">
        /// List of users
        /// </param>
        /// <returns>Returns true if updated succesfully otherwise false</returns>
        [HttpPost]
        public bool Update(IList<TenantUser> users)
        {
            bool result = false;
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                result = this.userManager.UpdateTenantUsers(users, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return result;
        }

        #endregion

        #region Delete

        /// <summary>
        /// This api call use to add single or list of user
        /// </summary>
        /// <param name="users">
        /// List of users
        /// </param>
        /// <returns>Returns true if deleted succesfully otherwise false</returns>
        [HttpPost]
        public bool Delete(IList<TenantUser> users)
        {
            bool result = false;
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                result = this.userManager.DeleteTenantUsers(users, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return result;
        }

        #endregion

        #region Get

        #region List

        /// <summary>
        /// This api call use to get single or list of user
        /// </summary>
        /// <param name="users">
        /// List of users
        /// </param>
        /// <returns>Returns list of users</returns>
        [HttpPost]
        public IList<TenantUser> List(TenantUserSearchParameter userSearchParameter)
        {
            IList<TenantUser> users = new List<TenantUser>();
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                users = this.userManager.GetTenantUsers(userSearchParameter, tenantCode);
                HttpContext.Current.Response.AppendHeader("recordCount", this.userManager.GetTenantUserCount(userSearchParameter, tenantCode).ToString());
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return users;
        }

        #endregion

        #region Detail

        /// <summary>
        /// This api used to get single user record by gievn Id.
        /// </summary>
        /// <param name="userIdentifier">The user identifier</param>
        /// <returns>Rerurns details of user</returns>
        [HttpGet]
        public TenantUser Detail(long userIdentifier)
        {
            IList<TenantUser> users = new List<TenantUser>();
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                TenantUserSearchParameter userSearchParameter = new TenantUserSearchParameter();
                userSearchParameter.Identifier = userIdentifier.ToString();
                userSearchParameter.SortParameter.SortColumn = "Id";
                users = this.userManager.GetTenantUsers(userSearchParameter, tenantCode);

                if (users?.Count <= 0)
                {
                    throw new TenantUserNotFoundException(tenantCode);
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return users.First();
        }

        #endregion

        #endregion

        #region Activate

        /// <summary>
        /// This method used to activate user
        /// </summary>
        /// <param name="userIdentifier">The user identifier</param>
        /// <returns>Returns true if activated successfully otherwise false</returns>
        [HttpGet]
        public bool Activate(long userIdentifier)
        {
            bool result = false;
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                result = this.userManager.ActivateTenantUser(userIdentifier, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return result;
        }

        #endregion

        #region DeActivate

        /// <summary>
        /// This method used to deactivate user
        /// </summary>
        /// <param name="userIdentifier">The user identifier</param>
        /// <returns>Returns true if deactivated successfully otherwise false</returns>
        [HttpGet]
        public bool DeActivate(long userIdentifier)
        {
            bool result = false;
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                result = this.userManager.DeActivateTenantUser(userIdentifier, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return result;
        }

        #endregion

        #endregion

    }
}
