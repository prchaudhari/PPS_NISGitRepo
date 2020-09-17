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
    /// This class implements manager layer of tenantContact manager.
    /// </summary>
    public class TenantContactManager
    {
        #region Private members
        /// <summary>
        /// The unity container
        /// </summary>
        private IUnityContainer unityContainer = null;

        /// <summary>
        /// The tenantContact repository.
        /// </summary>
        ITenantContactRepository tenantContactRepository = null;

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

        private UserManager userManager = null;

        #endregion

        #region Constructor

        /// <summary>
        /// This is constructor for role manager, which initialise
        /// role repository.
        /// </summary>
        /// <param name="container">IUnity container implementation object.</param>
        public TenantContactManager(IUnityContainer unityContainer)
        {
            try
            {
                this.unityContainer = unityContainer;
                this.tenantContactRepository = this.unityContainer.Resolve<ITenantContactRepository>();
                this.cryptoManager = this.unityContainer.Resolve<ICryptoManager>();
                this.userManager = new UserManager(this.unityContainer);
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
        /// This method helps to validate tenantContacts and then add to database.
        /// </summary>
        /// <param name="tenantContacts">List of tenantContact object</param>
        /// <param name="tenantCode">The Tenant code</param>
        /// <returns>
        /// If successfully added, it will return true.
        /// </returns>
        public bool AddTenantContacts(IList<TenantContact> tenantContacts, string tenantCode)
        {
            bool result = false;
            try
            {
                this.IsValidtenantContacts(tenantContacts, tenantCode);
                this.IsDuplicateEmailOrContactNumber(tenantContacts, tenantCode);
                //{
                result = this.tenantContactRepository.AddTenantContacts(tenantContacts, tenantCode);

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
        /// This method helps to validate tenantContacts and then update to database.
        /// </summary>
        /// <param name="tenantContacts">List of tenantContact object</param>
        /// <param name="tenantCode">The Tenant code</param>
        /// <returns>
        /// If successfully added, it will return true.
        /// </returns>
        public bool UpdateTenantContacts(IList<TenantContact> tenantContacts, string tenantCode)
        {
            bool result = false;
            try
            {
                this.IsValidtenantContacts(tenantContacts, tenantCode);
                this.IsDuplicateEmailOrContactNumber(tenantContacts, tenantCode);
                result = this.tenantContactRepository.UpdateTenantContacts(tenantContacts, tenantCode);
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
        /// This method helps to delete tenantContacts from database.
        /// </summary>
        /// <param name="tenantContacts">List of tenantContact object</param>
        /// <param name="tenantCode">The Tenant code</param>
        /// <returns>
        /// If successfully added, it will return true.
        /// </returns>
        public bool DeleteTenantContacts(IList<TenantContact> tenantContacts, string tenantCode)
        {
            bool result = false;
            try
            {
                this.CheckTenantContactDependency(tenantContacts, tenantCode);
                result = this.tenantContactRepository.DeleteTenantContacts(tenantContacts, tenantCode);
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
        /// This method helps to get specified tenantContacts from database using given tenantContact's search parameter.
        /// </summary>
        /// <param name="searchParameter">
        /// Search parameter to search specified tenantContact.
        /// </param>
        /// <param name="tenantCode">
        /// The tenant code
        /// </param> 
        /// <returns>
        /// Returns a list of tenantContact if any found otherwise it will return enpty list.
        /// </returns>
        public IList<TenantContact> GetTenantContacts(TenantContactSearchParameter searchParameter, string tenantCode)
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
                IList<TenantContact> tenantContacts = this.tenantContactRepository.GetTenantContacts(searchParameter, tenantCode);

                return tenantContacts;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        #endregion

        #region Get Count

        /// <summary>
        /// This method helps to get tenantContacts count.
        /// </summary>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns>
        /// Returns count of usesrs
        /// </returns>
        public int GetTenantContactCount(TenantContactSearchParameter tenantContactSearchParameter, string tenantCode)
        {
            int tenantContactCount = 0;
            try
            {
                tenantContactCount = this.tenantContactRepository.GetTenantContactCount(tenantContactSearchParameter, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return tenantContactCount;
        }

        #endregion

        #region Activate

        /// <summary>
        /// This method helps to active tenantContact.
        /// </summary>
        /// <param name="tenantContactIdentifier"></param>
        /// <returns></returns>
        public bool ActivateTenantContact(long tenantContactIdentifier, string tenantCode)
        {
            try
            {
                return this.tenantContactRepository.ActivateTenantContact(tenantContactIdentifier, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method helps to validate tenantContacts and then add to database.
        /// </summary>
        /// <param name="tenantContacts">List of tenantContact object</param>
        /// <param name="tenantCode">The Tenant code</param>
        /// <returns>
        /// If successfully added, it will return true.
        /// </returns>
        public bool SentActivationLink(IList<TenantContact> tenantContacts, string tenantCode)
        {
            bool result = false;
            try
            {
                IList<User> users = new List<User>();
                if (tenantContacts?.Count() > 0)
                {
                    users = tenantContacts.Select(item => new User
                    {
                        FirstName = item.FirstName,
                        LastName = item.LastName,
                        ContactNumber = item.ContactNumber,
                        EmailAddress = item.EmailAddress,
                        Image = item.Image,
                        IsActive = item.IsActive,
                        IsLocked = false,
                        NoofAttempts = 0,
                        CountryId = item.CountryId,
                        TenantCode = tenantCode
                    }).ToList();
                    result= this.userManager.AddUsers(users, tenantCode);
                }

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region DeActivate

        /// <summary>
        /// This mehod helps to deactive tenantContact.
        /// </summary>
        /// <param name="tenantContactIdentifier"></param>
        /// <returns></returns>
        public bool DeActivateTenantContact(long tenantContactIdentifier, string tenantCode)
        {
            try
            {
                this.CheckTenantContactDependency(new List<TenantContact> { new TenantContact() { Identifier = tenantContactIdentifier } }, tenantCode);
                return this.tenantContactRepository.DeactivateTenantContact(tenantContactIdentifier, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #endregion

        #region Private Methods
        private void IsValidtenantContacts(IList<TenantContact> tenantContacts, string tenantCode)
        {
            try
            {
                if (tenantContacts?.Count > 0)
                {
                    InvalidTenantContactException invalidTenantContactException = new InvalidTenantContactException(tenantCode);
                    tenantContacts.ToList().ForEach(item =>
                    {
                        try
                        {
                            item.IsValid();
                            if (string.IsNullOrEmpty(item.Image))
                            {

                            }
                        }
                        catch (Exception ex)
                        {
                            invalidTenantContactException.Data.Add(item.FirstName, ex.Data);
                        }
                    });

                    if (invalidTenantContactException.Data.Count > 0)
                    {
                        throw invalidTenantContactException;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method helps to check duplicate tenantContact in the list
        /// </summary>
        /// <param name="tenantContacts"></param>
        /// <param name="tenantCode"></param>
        private void IsDuplicateEmailOrContactNumber(IList<TenantContact> tenantContacts, string tenantCode)
        {
            int duplicateTenantContactCount = 0;
            try
            {
                if (tenantContacts?.Count > 0)
                {
                    duplicateTenantContactCount = tenantContacts.GroupBy(c => new
                    {
                        c.EmailAddress,
                        c.ContactNumber,
                    }).Where(g => g.Count() > 1).Count();
                    if (duplicateTenantContactCount > 0)
                    {
                        throw new DuplicateTenantContactFoundException(tenantCode);
                    }
                    else
                    {
                        duplicateTenantContactCount = tenantContacts.GroupBy(c => c.EmailAddress).Where(g => g.Count() > 1).Count();
                        if (duplicateTenantContactCount > 0)
                        {
                            throw new DuplicateTenantContactEmailAddressFoundException(tenantCode);
                        }
                        else
                        {
                            duplicateTenantContactCount = tenantContacts.GroupBy(p => p.ContactNumber).Where(g => g.Count() > 1).Count();
                            if (duplicateTenantContactCount > 0)
                            {
                                throw new DuplicateTenantContactMobileNumberFoundException(tenantCode);
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
        /// This method helps to check tenantContact dependency on other entity.
        /// </summary>
        /// <param name="tenantContacts">
        /// The tenantContacts
        /// </param>
        /// <param name="tenantCode">
        /// The tenant code
        /// </param>
        private void CheckTenantContactDependency(IList<TenantContact> tenantContacts, string tenantCode)
        {
            try
            {
                string tenantContactIdentifiers = string.Join(",", tenantContacts.Select(tenantContact => tenantContact.Identifier.ToString()).ToList());

            }
            catch (Exception)
            {
                throw;
            }
        }



        #endregion
    }
}
