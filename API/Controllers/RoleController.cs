// <copyright file="RoleController.cs" company="Websym Solutions Pvt Ltd">
// Copyright (c) 2018 Websym Solutions Pvt Ltd.
// </copyright>
// -----------------------------------------------------------------------  

namespace nIS
{
    #region References

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Http;
    using Unity;

    #endregion

    /// <summary>
    /// This class represent api controller for role
    /// </summary>
    public class RoleController : ApiController
    {
        #region Private Members

        /// <summary>
        /// The role manager object.
        /// </summary>
        private RoleManager roleManager = null;

        /// <summary>
        /// The unity container
        /// </summary>
        private readonly IUnityContainer unityContainer = null;

        #endregion

        #region Constructor

        public RoleController(IUnityContainer unityContainer)
        {
            this.unityContainer = unityContainer;
            this.roleManager = new RoleManager(this.unityContainer);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// This method helps to add roles
        /// </summary>
        /// <param name="roles"></param>
        /// <returns>boolean value</returns>
        [HttpPost]
        public bool Add(IList<Role> roles)
        {
            bool result = false;
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                result = this.roleManager.AddRoles(roles, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return result;
        }

        /// <summary>
        /// This method helps to update roles.
        /// </summary>
        /// <param name="roles"></param>
        /// <returns>boolean value</returns>
        [HttpPost]
        public bool Update(IList<Role> roles)
        {
            bool result = false;
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                result = this.roleManager.UpdateRoles(roles, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return result;
        }

        /// <summary>
        /// This method helps to delete roles.
        /// </summary>
        /// <param name="roles"></param>
        /// <returns>boolean value</returns>
        [HttpPost]
        public bool Delete(IList<Role> roles)
        {
            bool result = false;
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                result = this.roleManager.DeleteRoles(roles, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return result;
        }

        /// <summary>
        /// This method helps to get roles list based on the search parameters.
        /// </summary>
        /// <param name="roleSearchParameter"></param>
        /// <returns>List of roles</returns>
        [HttpPost]
        public IList<Role> List(RoleSearchParameter roleSearchParameter)
        {
            IList<Role> roles = new List<Role>();
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                roles = this.roleManager.GetRoles(roleSearchParameter, tenantCode);
                HttpContext.Current.Response.AppendHeader("recordCount", this.roleManager.GetRoleCount(roleSearchParameter, tenantCode).ToString());
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return roles;
        }

        /// <summary>
        /// This method helps to get role based on given identifier.
        /// </summary>
        /// <param name="roleSearchParameter"></param>
        /// <returns>role record</returns>
        [HttpGet]
        public Role Detail(long roleIdentifier)
        {
            IList<Role> roles = new List<Role>();
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                RoleSearchParameter roleSearchParameter = new RoleSearchParameter();
                roleSearchParameter.Identifier = roleIdentifier.ToString();
                roleSearchParameter.SortParameter.SortColumn = "Id";
                roles = this.roleManager.GetRoles(roleSearchParameter, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return roles.First();
        }

        /// <summary>
        /// This method helps to activate the role
        /// </summary>
        /// <param name="roleIdentifier">The role identifier</param>       
        /// <returns>True if role activated successfully false otherwise</returns>
        [HttpGet]
        public bool Activate(long roleIdentifier)
        {
            bool result = false;
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                this.roleManager.ActivateRole(roleIdentifier, tenantCode);
                result = true;
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return result;
        }

        /// <summary>
        /// This method helps to activate the role
        /// </summary>
        /// <param name="roleIdentifier">The role identifier</param>       
        /// <returns>True if role activated successfully false otherwise</returns>
        [HttpGet]
        public bool IsDeactivateDependency(long roleIdentifier)
        {
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                return this.roleManager.CheckUserAssociatedWithRole(roleIdentifier, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// This method helps to deactivate the role
        /// </summary>
        /// <param name="roleIdentifier">The role identifier</param>       
        /// <returns>True if role deactivated successfully false otherwise</returns>
        [HttpGet]
        public bool Deactivate(long roleIdentifier)
        {
            bool result = false;
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                this.roleManager.DeactivateRole(roleIdentifier, tenantCode);
                result = true;
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return result;
        }
        /// <summary>
        /// THis method will call get method of entity manager.
        /// </summary>
        /// <param name="entitySearchParameter">The entity search parameter</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Return list of roleprivileges if exist other wise return null
        /// </returns>
        [HttpGet]
        public IList<RolePrivilege> GetRolePrivileges()
        {
            IList<RolePrivilege> rolePrivileges = new List<RolePrivilege>();
            string resultContent = string.Empty;
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                rolePrivileges = this.roleManager.GetRolePrivileges(tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return rolePrivileges;
        }
        #endregion

    }
}
