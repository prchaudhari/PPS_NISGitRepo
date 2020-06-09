// <copyright file="UserController.cs" company="Websym Solutions Pvt Ltd">
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
    using Unity;

    #endregion

    /// <summary>
    /// This class represent api controller for user
    /// </summary>
    public class UserController : ApiController
    {
        #region Private Members

        /// <summary>
        /// The role manager object.
        /// </summary>
        private UserManager userManager = null;

        /// <summary>
        /// The unity container
        /// </summary>
        private readonly IUnityContainer unityContainer = null;

        #endregion

        #region Constructor

        public UserController(IUnityContainer unityContainer)
        {
            this.unityContainer = unityContainer;
            this.userManager = new UserManager(this.unityContainer);
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
        public bool Add(IList<User> users)
        {
            bool result = false;
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                result = this.userManager.AddUsers(users, tenantCode);
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
        public bool Update(IList<User> users)
        {
            bool result = false;
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                result = this.userManager.UpdateUsers(users, tenantCode);
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
        public bool Delete(IList<User> users)
        {
            bool result = false;
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                result = this.userManager.DeleteUsers(users, tenantCode);
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
        public IList<User> List(UserSearchParameter userSearchParameter)
        {
            IList<User> users = new List<User>();
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                users = this.userManager.GetUsers(userSearchParameter, tenantCode);

                HttpContext.Current.Response.AppendHeader("recordCount", this.userManager.GetUserCount(userSearchParameter, tenantCode).ToString());
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
        public User Detail(long userIdentifier)
        {
            IList<User> users = new List<User>();
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                UserSearchParameter userSearchParameter = new UserSearchParameter();
                userSearchParameter.Identifier = userIdentifier.ToString();
                userSearchParameter.SortParameter.SortColumn = "Id";
                users = this.userManager.GetUsers(userSearchParameter, tenantCode);
                if (users?.Count <= 0)
                {
                    throw new UserNotFoundException(tenantCode);
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
                result = this.userManager.ActivateUser(userIdentifier, tenantCode);
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
                result = this.userManager.DeActivateUser(userIdentifier, tenantCode);
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
