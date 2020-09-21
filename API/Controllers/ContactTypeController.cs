// <copyright file="ContactTypeController.cs" company="Websym Solutions Pvt Ltd">
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
    using System.Web.Http.Cors;
    using Unity;

    #endregion

    /// <summary>
    /// This class represent api controller for contactType
    /// </summary>
    [EnableCors("*", "*", "*", "*")]
    [RoutePrefix("ContactType")]
    public class ContactTypeController : ApiController
    {

        #region Private Members

        /// <summary>
        /// The contactType manager object.
        /// </summary>
        private ContactTypeManager contactTypeManager = null;

        #endregion

        #region Constructor

        public ContactTypeController(IUnityContainer unityContainer)
        {
            this.contactTypeManager = new ContactTypeManager(unityContainer);
        }

        #endregion

        #region Public Method

        #region Add ContactType

        /// <summary>
        /// This method helps to add contactType
        /// </summary>
        /// <param name="contactTypes"></param>
        /// <returns>boolean value</returns>
        [HttpPost]
        public bool Add(IList<ContactType> contactTypes)
        {
            bool result = false;
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                result = this.contactTypeManager.AddContactTypes(contactTypes, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return result;
        }

        #endregion

        #region Update ContactType

        /// <summary>
        /// This method helps to update contactType.
        /// </summary>
        /// <param name="contactTypes"></param>
        /// <returns>boolean value</returns>
        [HttpPost]
        public bool Update(IList<ContactType> contactTypes)
        {
            bool result = false;
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                result = this.contactTypeManager.UpdateContactTypes(contactTypes, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return result;
        }

        #endregion

        #region Delete ContactType

        /// <summary>
        /// This method helps to delete contactTypes.
        /// </summary>
        /// <param name="contactTypes"></param>
        /// <returns>boolean value</returns>
        [HttpPost]
        public bool Delete(IList<ContactType> contactTypes)
        {
            bool result = false;
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                result = this.contactTypeManager.DeleteContactTypes(contactTypes, tenantCode);
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
        /// This method helps to get contactType list based on the search parameters.
        /// </summary>
        /// <param name="contactTypeSearchParameter"></param>
        /// <returns>List of contactTypes</returns>
        [HttpPost]
        public IList<ContactType> List(ContactTypeSearchParameter contactTypeSearchParameter)
        {
            IList<ContactType> contactTypes = new List<ContactType>();
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                contactTypes = this.contactTypeManager.GetContactTypes(contactTypeSearchParameter, tenantCode);
                HttpContext.Current.Response.AppendHeader("recordCount", this.contactTypeManager.GetContactTypeCount(contactTypeSearchParameter, tenantCode).ToString());
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return contactTypes;
        }

        #endregion
        #endregion
    }
}