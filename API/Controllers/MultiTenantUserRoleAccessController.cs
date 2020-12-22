// <copyright file="MultiTenantUserRoleAccessController.cs" company="Websym Solutions Pvt Ltd">
// Copyright (c) 2020 Websym Solutions Pvt Ltd.
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
    /// This class represent api controller for multi-tenant user role access
    /// </summary>
    [RoutePrefix("MultiTenantUserRoleAccess")]
    public class MultiTenantUserRoleAccessController : ApiController
    {
        #region Private Members

        /// <summary>
        /// The role manager object.
        /// </summary>
        private MultiTenantUserRoleAccessManager multiTenantUserRoleAccessManager = null;

        /// <summary>
        /// The unity container
        /// </summary>
        private readonly IUnityContainer unityContainer = null;

        #endregion

        #region Constructor

        public MultiTenantUserRoleAccessController(IUnityContainer unityContainer)
        {
            this.unityContainer = unityContainer;
            this.multiTenantUserRoleAccessManager = new MultiTenantUserRoleAccessManager(this.unityContainer);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// This method helps to add list of multi-tenant user role access
        /// </summary>
        /// <param name="lstMultiTenantUserRoleAccess"></param>
        /// <returns>boolean value</returns>
        [HttpPost]
        public bool Add(IList<MultiTenantUserRoleAccess> lstMultiTenantUserRoleAccess)
        {
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                return this.multiTenantUserRoleAccessManager.AddMultiTenantUserRoleAccess(lstMultiTenantUserRoleAccess, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method helps to update list of multi-tenant user role access.
        /// </summary>
        /// <param name="lstMultiTenantUserRoleAccess"></param>
        /// <returns>boolean value</returns>
        [HttpPost]
        public bool Update(IList<MultiTenantUserRoleAccess> lstMultiTenantUserRoleAccess)
        {
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                return multiTenantUserRoleAccessManager.UpdateMultiTenantUserRoleAccess(lstMultiTenantUserRoleAccess, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method helps to delete multi-tenant user role access.
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns>boolean value</returns>
        [HttpPost]
        public bool Delete(long identifier)
        {
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                return this.multiTenantUserRoleAccessManager.DeletedMultiTenantUserRoleAccess(identifier, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method helps to get multi-tenant user role list based on the search parameters.
        /// </summary>
        /// <param name="multiTenantUserRoleAccessSearchParameter"></param>
        /// <returns>List of multi-tenant user role access</returns>
        [HttpPost]
        public IList<MultiTenantUserRoleAccess> List(MultiTenantUserRoleAccessSearchParameter multiTenantUserRoleAccessSearchParameter)
        {
            IList<MultiTenantUserRoleAccess> lstMultiTenantUserRoleAccess = new List<MultiTenantUserRoleAccess>();
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                lstMultiTenantUserRoleAccess = this.multiTenantUserRoleAccessManager.GetMultiTenantUserRoleAccessList(multiTenantUserRoleAccessSearchParameter, tenantCode);
                HttpContext.Current.Response.AppendHeader("recordCount", this.multiTenantUserRoleAccessManager.GetMultiTenantUserRoleAcessListCount(multiTenantUserRoleAccessSearchParameter, tenantCode).ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return lstMultiTenantUserRoleAccess;
        }

        /// <summary>
        /// This method helps to activate multi-tenant user role access.
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns>boolean value</returns>
        [HttpPost]
        public bool Activate(long identifier)
        {
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                return this.multiTenantUserRoleAccessManager.ActivateMultiTenantUserRoleAccess(identifier, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method helps to deactivate multi-tenant user role access.
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns>boolean value</returns>
        [HttpPost]
        public bool Deactivate(long identifier)
        {
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                return this.multiTenantUserRoleAccessManager.DeactivateMultiTenantUserRoleAccess(identifier, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method gets the specified list of users by tenant code from user repository.
        /// </summary>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of users of specific tenant code
        /// </returns>
        [HttpGet]
        public IList<User> GetUsersByTenantCode(string tenantCode)
        {
            IList<User> users = new List<User>();
            try
            {
                users = this.multiTenantUserRoleAccessManager.GetUsersByTenantCode(tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return users;
        }

        /// <summary>
        /// This method gets the specified list of roles by tenant code from role repository.
        /// </summary>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of users of specific tenant code
        /// </returns>
        [HttpGet]
        public IList<Role> GetRolesByTenantCode(string tenantCode)
        {
            IList<Role> roles = new List<Role>();
            try
            {
                roles = this.multiTenantUserRoleAccessManager.GetRolesByTenantCode(tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return roles;
        }

        /// <summary>
        /// This method gets the specified list of mapped tenants to single user.
        /// </summary>
        /// <param name = "userId" > The User Identifier</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of tenants which are mapped to user
        /// </returns>
        [HttpGet]
        public IList<UserTenant> GetUserTenants(long userId)
        {
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                IList<UserTenant> userTenants = new List<UserTenant>();
                userTenants = this.multiTenantUserRoleAccessManager.GetUserTenants(userId, tenantCode);

                return userTenants;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method gets the list of parent as well as child tenants.
        /// </summary>
        /// <returns>
        /// Returns the list of parent as well as child tenants
        /// </returns>
        [HttpGet]
        public IList<Client> GetParentAndChildTenants()
        {
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                return this.multiTenantUserRoleAccessManager.GetParentAndChildTenants(tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }
}