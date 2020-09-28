// <copyright file="RoleManager.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2018 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------

namespace nIS
{
    #region References

    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics;
    using System.Linq;
    using System.Net.Mail;
    using System.Text.RegularExpressions;
    using System.Transactions;
    using Unity;
    using Websym.Core.EventManager;

    #endregion

    /// <summary>
    /// This class implements manager layer of user manager.
    /// </summary>
    public class TenantUserManager
    {
        #region Private members
        /// <summary>
        /// The unity container
        /// </summary>
        private IUnityContainer unityContainer = null;

        /// <summary>
        /// The user repository.
        /// </summary>
        ITenantUserRepository userRepository = null;

        /// <summary>
        /// The crypto manager
        /// </summary>
        private readonly ICryptoManager cryptoManager;

        /// <summary>
        /// The resource manager
        /// </summary>
        //private readonly ResourceManager resourceManager = null;

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
        public TenantUserManager(IUnityContainer unityContainer)
        {
            try
            {
                this.unityContainer = unityContainer;
                this.userRepository = this.unityContainer.Resolve<ITenantUserRepository>();
                this.cryptoManager = this.unityContainer.Resolve<ICryptoManager>();
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
        public bool AddTenantUsers(IList<TenantUser> users, string tenantCode)
        {
            bool result = false;
            try
            {
                this.IsValidusers(users, tenantCode);
                this.IsDuplicateEmailOrContactNumber(users, tenantCode);
                result = this.userRepository.AddTenantUsers(users, tenantCode);

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
        public bool UpdateTenantUsers(IList<TenantUser> users, string tenantCode)
        {
            bool result = false;
            try
            {
                this.IsValidusers(users, tenantCode);
                this.IsDuplicateEmailOrContactNumber(users, tenantCode);
                result = this.userRepository.UpdateTenantUsers(users, tenantCode);
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
        public bool DeleteTenantUsers(IList<TenantUser> users, string tenantCode)
        {
            bool result = false;
            try
            {
                this.CheckTenantUserDependency(users, tenantCode);
                result = this.userRepository.DeleteTenantUsers(users, tenantCode);
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
        public IList<TenantUser> GetTenantUsers(TenantUserSearchParameter searchParameter, string tenantCode)
        {
            try
            {
                if (searchParameter == null)
                {
                    throw new NullArgumentException(tenantCode);
                }

                InvalidSearchParameterException invalidSearchParameterException = new InvalidSearchParameterException(tenantCode);

                try
                {
                    searchParameter.IsValid();
                }
                catch (Exception exception)
                {
                    invalidSearchParameterException.Data.Add("InvalidPagingParameter", exception.Data);
                    throw invalidSearchParameterException;
                }
                IList<TenantUser> users = this.userRepository.GetTenantUsers(searchParameter, tenantCode);

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
        public int GetTenantUserCount(TenantUserSearchParameter userSearchParameter, string tenantCode)
        {
            int userCount = 0;
            try
            {
                userCount = this.userRepository.GetTenantUserCount(userSearchParameter, tenantCode);
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
        public bool ActivateTenantUser(long userIdentifier, string tenantCode)
        {
            try
            {
                return this.userRepository.ActivateTenantUser(userIdentifier, tenantCode);
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
        public bool DeActivateTenantUser(long userIdentifier, string tenantCode)
        {
            try
            {
                this.CheckTenantUserDependency(new List<TenantUser> { new TenantUser() { Identifier = userIdentifier } }, tenantCode);
                return this.userRepository.DeactivateTenantUser(userIdentifier, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Is Duplicate TenantUser

        /// <summary>
        /// This method determines uniqueness of elements in repository.
        /// </summary>
        /// <param name="roles">The roles to save.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns name="result">
        /// Returns true if all elements are not present in repository, false otherwise.
        /// </returns>
        public bool IsDuplicateTenantUserEmailAndMobileNumber(IList<TenantUser> users, string operation, string tenantCode)
        {
            try
            {
                return this.userRepository.IsDuplicateTenantUserEmailAndMobileNumber(users, operation, tenantCode);
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
        private void IsValidusers(IList<TenantUser> users, string tenantCode)
        {
            try
            {
                if (users?.Count > 0)
                {
                    InvalidTenantUserException invalidTenantUserException = new InvalidTenantUserException(tenantCode);
                    users.ToList().ForEach(item =>
                    {
                        try
                        {
                            item.IsValid();
                            if (string.IsNullOrEmpty(item.Image))
                            {
                                //string base64Image = "data:image/jpg;base64,";
                                //string path1 = System.Web.HttpContext.Current.Server.MapPath("~/images/user-avatar.jpg");
                                //byte[] imageArray = System.IO.File.ReadAllBytes(path1);
                                //base64Image += Convert.ToBase64String(imageArray);
                                //item.Image = base64Image;
                            }
                        }
                        catch (Exception ex)
                        {
                            invalidTenantUserException.Data.Add(item.FirstName, ex.Data);
                        }
                    });

                    if (invalidTenantUserException.Data.Count > 0)
                    {
                        throw invalidTenantUserException;
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
        private void IsDuplicateEmailOrContactNumber(IList<TenantUser> users, string tenantCode)
        {
            int duplicateTenantUserCount = 0;
            try
            {
                if (users?.Count > 0)
                {
                    duplicateTenantUserCount = users.GroupBy(c => new
                    {
                        c.EmailAddress,
                        c.ContactNumber,
                    }).Where(g => g.Count() > 1).Count();
                    if (duplicateTenantUserCount > 0)
                    {
                        throw new DuplicateTenantUserFoundException(tenantCode);
                    }
                    else
                    {
                        duplicateTenantUserCount = users.GroupBy(c => c.EmailAddress).Where(g => g.Count() > 1).Count();
                        if (duplicateTenantUserCount > 0)
                        {
                            throw new DuplicateTenantUserEmailAddressFoundException(tenantCode);
                        }
                        else
                        {
                            duplicateTenantUserCount = users.GroupBy(p => p.ContactNumber).Where(g => g.Count() > 1).Count();
                            if (duplicateTenantUserCount > 0)
                            {
                                throw new DuplicateTenantUserMobileNumberFoundException(tenantCode);
                            }
                        }
                    }


                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// This method will return the system generated random number 
        /// </summary>
        /// <returns>
        /// It will return rendom string.
        /// </returns>
        private string GeneratePassword()
        {
            //// Set password as random number and encrypt it
            Random random = new Random();
            return Convert.ToString(random.Next());
        }

        /// <summary>
        /// This method helps to check user dependency on other entity.
        /// </summary>
        /// <param name="users">
        /// The users
        /// </param>
        /// <param name="tenantCode">
        /// The tenant code
        /// </param>
        private void CheckTenantUserDependency(IList<TenantUser> users, string tenantCode)
        {
            try
            {
                string userIdentifiers = string.Join(",", users.Select(user => user.Identifier.ToString()).ToList());

            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// This is responsible for validate password (password shold be one capital,one small,one special & one number
        /// </summary>
        /// <param name="password"></param>
        /// <param name="tenantCode"></param>
        /// <returns></returns>
        private bool IsValidPassword(string password, string tenantCode)
        {
            try
            {
                var hasNumber = new Regex(@"[0-9]+");
                var hasUpperChar = new Regex(@"[A-Z]+");
                var hasLowerChar = new Regex(@"[a-z]+");
                var hasMinimum8Chars = new Regex(@".{8,}");
                Regex specialCharacter = new Regex(@"[~`!@#$%^&*()-+=|\{}':;.,<>/?]");
                bool isValidated = hasNumber.IsMatch(password) && hasUpperChar.IsMatch(password) && hasLowerChar.IsMatch(password) && hasMinimum8Chars.IsMatch(password) && specialCharacter.IsMatch(password);
                return isValidated;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        #endregion
    }
}
