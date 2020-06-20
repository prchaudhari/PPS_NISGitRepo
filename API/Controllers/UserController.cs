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
    using System.Web.Http.Cors;
    using Unity;

    #endregion

    /// <summary>
    /// This class represent api controller for user
    /// </summary>
    [RoutePrefix("User")]
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

        #region Login password activity related api

        /// <summary>
        /// This api controller helps to change or update password.
        /// </summary>
        /// <param name="newPassword">
        /// User's new password
        /// </param>
        /// <param name="encryptedText">
        /// Encrypted text
        /// </param>
        /// <returns>
        /// Returns true if successfully done.
        /// </returns>
        [HttpGet]
        public bool Confirm(string newPassword, string encryptedText)
        {
            try
            {
                string tenantCode = ModelConstant.DEFAULT_TENANT_CODE;
                bool result = this.userManager.ChangePassword(newPassword, encryptedText, tenantCode);

                return result;
            }
            catch (Exception cautchException)
            {
                throw cautchException;
            }
        }

        /// <summary>
        /// This api controller helps to change or update password.
        /// </summary>
        /// <param name="user">User object.</param>
        /// <returns>
        /// Returns true if successfully updated password.
        /// </returns>
        [HttpGet]
        public bool ChangePassword(string userEmail, string oldPassword, string newPassword)
        {
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                return this.userManager.ChangePassword(userEmail, oldPassword, newPassword, tenantCode);
            }
            catch (Exception cautchException)
            {
                throw cautchException;
            }
        }

        /// <summary>
        /// This api controller method will send a mail for reset password.
        /// </summary>
        /// <param name="userEmail">
        /// User email address.
        /// </param>
        /// <returns>
        /// If mail will send successfully for reset passsword, it will return true.
        /// </returns>
        [HttpGet]
        public bool ResetPassword(string userEmail)
        {
            try
            {
                string tenantCode = ModelConstant.DEFAULT_TENANT_CODE;
                return this.userManager.ResetUserPassword(userEmail, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        #endregion

        #region Get User Login Activity API

        /// <summary>
        /// This is responsible for get user login activity based on their user identifier
        /// </summary>
        /// <param name="useridentifier"></param>
        /// <returns></returns>
        [HttpGet]
        public IList<UserLoginActivityHistory> LogInActivity(string useridentifier)
        {
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                return this.userManager.GetUserLogInActivityHistory(useridentifier, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        #endregion

        #region Unlock User

        /// <summary>
        /// This method used to activate user
        /// </summary>
        /// <param name="userIdentifier">The user identifier</param>
        /// <returns>Returns true if activated successfully otherwise false</returns>
        [HttpGet]
        public bool Unlock(long userIdentifier)
        {
            bool result = false;
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                result = this.userManager.UnlockUser(userIdentifier, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return result;
        }

        #endregion

        #region lock User

        /// <summary>
        /// This method used to lock user
        /// </summary>
        /// <param name="userIdentifier">The user identifier</param>
        /// <returns>Returns true if lockec successfully otherwise false</returns>
        [HttpGet]
        public bool Lock(long userIdentifier)
        {
            bool result = false;
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                result = this.userManager.LockUser(userIdentifier, tenantCode);
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
