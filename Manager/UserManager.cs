// <copyright file="RoleManager.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2018 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------

namespace nIS
{
    #region References
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Transactions;
    using Unity;

    #endregion

    /// <summary>
    /// This class implements manager layer of user manager.
    /// </summary>
    public class UserManager
    {
        #region Private members
        /// <summary>
        /// The unity container
        /// </summary>
        private IUnityContainer unityContainer = null;

        /// <summary>
        /// The user repository.
        /// </summary>
        IUserRepository userRepository = null;

        /// <summary>
        /// The utility.
        /// </summary>
        IUtility utility = new Utility();

        #endregion

        #region Constructor

        /// <summary>
        /// This is constructor for role manager, which initialise
        /// role repository.
        /// </summary>
        /// <param name="container">IUnity container implementation object.</param>
        public UserManager(IUnityContainer unityContainer)
        {
            try
            {
                this.unityContainer = unityContainer;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Public Methods

        #region Add

        /// <summary>
        /// This method helps to validate users and then add to database.
        /// </summary>
        /// <param name="users">List of user object</param>
        /// <param name="tenantCode">The Tenant code</param>
        /// <returns>
        /// If successfully added, it will return true.
        /// </returns>
        public bool AddUsers(IList<User> users, string tenantCode)
        {
            bool result = false;
            try
            {

                using (TransactionScope transactionScope = new TransactionScope())
                {
                    result = this.userRepository.AddUsers(users, tenantCode);
                    transactionScope.Complete();
                };

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Update

        /// <summary>
        /// This method helps to validate users and then update to database.
        /// </summary>
        /// <param name="users">List of user object</param>
        /// <param name="tenantCode">The Tenant code</param>
        /// <returns>
        /// If successfully added, it will return true.
        /// </returns>
        public bool UpdateUsers(IList<User> users, string tenantCode)
        {
            bool result = false;
            try
            {
                this.IsValidusers(users, tenantCode);
                this.IsDuplicateEmailOrContactNumber(users, tenantCode);
                result = this.userRepository.UpdateUsers(users, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        #endregion

        #region Delete

        /// <summary>
        /// This method helps to delete users from database.
        /// </summary>
        /// <param name="users">List of user object</param>
        /// <param name="tenantCode">The Tenant code</param>
        /// <returns>
        /// If successfully added, it will return true.
        /// </returns>
        public bool DeleteUsers(IList<User> users, string tenantCode)
        {
            bool result = false;
            try
            {
                result = this.userRepository.DeleteUsers(users, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        #endregion

        #region Get

        /// <summary>
        /// This method helps to get specified users from database using given user's search parameter.
        /// </summary>
        /// <param name="searchParameter">
        /// Search parameter to search specified user.
        /// </param>
        /// <param name="tenantCode">
        /// The tenant code
        /// </param> 
        /// <returns>
        /// Returns a list of user if any found otherwise it will return enpty list.
        /// </returns>
        public IList<User> GetUsers(UserSearchParameter searchParameter, string tenantCode)
        {
            try
            {


                InvalidSearchParameterException invalidSearchParameterException = new InvalidSearchParameterException(tenantCode);

                try
                {
                    searchParameter.IsValid();
                }
                catch (Exception exception)
                {
                    invalidSearchParameterException.Data.Add("InvalidPagingParameter", exception.Data); ;
                }
                IList<User> users = this.userRepository.GetUsers(searchParameter, tenantCode);
                return users;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        #endregion

        #region Get Count

        /// <summary>
        /// This method helps to get users count.
        /// </summary>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns>
        /// Returns count of usesrs
        /// </returns>
        public int GetUserCount(UserSearchParameter userSearchParameter, string tenantCode)
        {
            int userCount = 0;
            try
            {
                userCount = this.userRepository.GetUserCount(userSearchParameter, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return userCount;
        }

        #endregion

        #region Activate

        /// <summary>
        /// This method helps to active user.
        /// </summary>
        /// <param name="userIdentifier"></param>
        /// <returns></returns>
        public bool ActivateUser(long userIdentifier, string tenantCode)
        {
            try
            {
                return this.userRepository.ActivateUser(userIdentifier, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region DeActivate

        /// <summary>
        /// This mehod helps to deactive user.
        /// </summary>
        /// <param name="userIdentifier"></param>
        /// <returns></returns>
        public bool DeActivateUser(long userIdentifier, string tenantCode)
        {
            try
            {
                return this.userRepository.DeactivateUser(userIdentifier, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #endregion

        #region Private Methods

        /// <summary>
        /// This method is responsible for validate users.
        /// </summary>
        /// <param name="roles"></param>
        /// <param name="tenantCode"></param>
        private void IsValidusers(IList<User> users, string tenantCode)
        {
            try
            {
                if (users?.Count > 0)
                {
                    InvalidUserException invalidUserException = new InvalidUserException(tenantCode);
                    users.ToList().ForEach(item =>
                    {
                        try
                        {
                            item.IsValid();
                        }
                        catch (Exception ex)
                        {
                            invalidUserException.Data.Add(item.FirstName, ex.Data);
                        }
                    });

                    if (invalidUserException.Data.Count > 0)
                    {
                        throw invalidUserException;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method helps to check duplicate user in the list
        /// </summary>
        /// <param name="users"></param>
        /// <param name="tenantCode"></param>
        private void IsDuplicateEmailOrContactNumber(IList<User> users, string tenantCode)
        {
            int duplicateUserCount = 0;
            try
            {
                if (users?.Count > 0)
                {
                    duplicateUserCount = users.GroupBy(p => p.EmailAddress).Where(g => g.Count() > 1).Count();
                    duplicateUserCount = users.GroupBy(p => p.ContactNumber).Where(g => g.Count() > 1).Count();
                    if (duplicateUserCount > 0)
                    {
                        throw new DuplicateUserFoundException(tenantCode);
                    }
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
