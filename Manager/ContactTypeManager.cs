// <copyright file="ContactTypeManager.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2018 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------

namespace nIS
{
    #region References

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Unity;

    #endregion

    /// <summary>
    /// This class implements manager layer of contactType manager.
    /// </summary>
    public class ContactTypeManager
    {
        #region Private members
        /// <summary>
        /// The unity container
        /// </summary>
        private IUnityContainer unityContainer = null;

        /// <summary>
        /// The asset library repository.
        /// </summary>
        IContactTypeRepository contactTypeRepository = null;

        #endregion

        #region Constructor

        /// <summary>
        /// This is constructor for contactType manager, which initialise
        /// contactType repository.
        /// </summary>
        /// <param name="container">IUnity container implementation object.</param>
        public ContactTypeManager(IUnityContainer unityContainer)
        {
            try
            {
                this.unityContainer = unityContainer;
                this.contactTypeRepository = this.unityContainer.Resolve<IContactTypeRepository>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Public Methods

        #region Add ContactType

        /// <summary>
        /// This method will call add contactType method of repository.
        /// </summary>
        /// <param name="contactTypes">ContactType are to be add.</param>
        /// <param name="tenantCode">Tenant code of contactType.</param>
        /// <returns>
        /// Returns true if entities added successfully, false otherwise.
        /// </returns>
        public bool AddContactTypes(IList<ContactType> contactTypes, string tenantCode)
        {
            bool result = false;
            try
            {
                this.IsValidContactType(contactTypes, tenantCode);
                this.IsDuplicateContactType(contactTypes, tenantCode);
                result = this.contactTypeRepository.AddContactTypes(contactTypes, tenantCode);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Update ContactType

        /// <summary>
        /// This method reference helps to update details about contactTypes.
        /// </summary>
        /// <param name="contactTypes">
        /// The list of contactTypes.
        /// </param>
        /// <param name="tenantCode">
        /// The tenant code
        /// </param>
        /// <returns>
        /// It will return true if list of scene updates scus=ccessfully otherwise false
        /// </returns>
        public bool UpdateContactTypes(IList<ContactType> contactTypes, string tenantCode)
        {
            bool result = false;
            try
            {
                this.IsValidContactType(contactTypes, tenantCode);
                this.IsDuplicateContactType(contactTypes, tenantCode);
                result = this.contactTypeRepository.UpdateContactTypes(contactTypes, tenantCode);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Delete ContactType

        /// <summary>
        /// This method reference helps to delete details about contactType.
        /// </summary>
        /// <param name="contactTypes">
        /// The list of contactTypes.
        /// </param>
        /// <param name="tenantCode">
        /// The tenant code
        /// </param>
        /// <returns>
        /// It will return true if successfully updated or it will throw an exception.
        /// </returns>
        public bool DeleteContactTypes(IList<ContactType> contactTypes, string tenantCode)
        {
            bool result = false;
            try
            {
                result = this.contactTypeRepository.DeleteContactTypes(contactTypes, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return result;
        }

        #endregion

        #region Get ContactTypes

        /// <summary>
        /// This method will call get contactTypes method of repository.
        /// </summary>
        /// <param name="contactTypeSearchParameter">The contactType search parameters.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns>
        /// Returns roles if found for given parameters, else return null
        /// </returns>
        public IList<ContactType> GetContactTypes(ContactTypeSearchParameter contactTypeSearchParameter, string tenantCode)
        {
            IList<ContactType> contactTypes = new List<ContactType>();
            try
            {
                InvalidSearchParameterException invalidSearchParameterException = new InvalidSearchParameterException(tenantCode);
                try
                {
                    contactTypeSearchParameter.IsValid();
                }
                catch (Exception exception)
                {
                    invalidSearchParameterException.Data.Add("InvalidPagingParameter", exception.Data);
                }

                if (invalidSearchParameterException.Data.Count > 0)
                {
                    throw invalidSearchParameterException;
                }

                contactTypes = this.contactTypeRepository.GetContactTypes(contactTypeSearchParameter, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return contactTypes;
        }

        #endregion

        #region Get ContactType Count
        /// <summary>
        /// This method helps to get count of contactType.
        /// </summary>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns>
        /// Returns count of roles
        /// </returns>
        public long GetContactTypeCount(ContactTypeSearchParameter contactTypeSearchParameter, string tenantCode)
        {
            long roleCount = 0;
            try
            {
                roleCount = this.contactTypeRepository.GetContactTypeCount(contactTypeSearchParameter, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return roleCount;
        }
        #endregion

        #endregion

        #region Private Methods

        /// <summary>
        /// This method is responsible for validate contactType.
        /// </summary>
        /// <param name="contactTypes"></param>
        /// <param name="tenantCode"></param>
        private void IsValidContactType(IList<ContactType> contactTypes, string tenantCode)
        {
            try
            {
                if (contactTypes?.Count <= 0)
                {
                    throw new NullArgumentException(tenantCode);
                }

                InvalidContactTypeException invalidContactTypeException = new InvalidContactTypeException(tenantCode);
                contactTypes.ToList().ForEach(item =>
                {
                    try
                    {
                        item.IsValid();
                    }
                    catch (Exception ex)
                    {
                        invalidContactTypeException.Data.Add(item.Name, ex.Data);
                    }
                });

                if (invalidContactTypeException.Data.Count > 0)
                {
                    throw invalidContactTypeException;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method helps to check duplicate contactType in the list
        /// </summary>
        /// <param name="contactTypes">contactTypes</param>
        /// <param name="tenantCode">tenant code</param>
        private void IsDuplicateContactType(IList<ContactType> contactTypes, string tenantCode)
        {
            try
            {
                int isDuplicateContactType = contactTypes.GroupBy(p => p.Name).Where(g => g.Count() > 1).Count();
                if (isDuplicateContactType > 0)
                {
                    throw new DuplicateContactTypeFoundException(tenantCode);
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