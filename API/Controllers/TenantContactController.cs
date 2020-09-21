// <copyright file="TenantContactController.cs" company="Websym Solutions Pvt Ltd">
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
    /// This class represent api controller for tenantContact
    /// </summary>
    [RoutePrefix("TenantContact")]
    public class TenantContactController : ApiController
    {
        #region Private Members

        /// <summary>
        /// The role manager object.
        /// </summary>
        private TenantContactManager tenantContactManager = null;

        /// <summary>
        /// The unity container
        /// </summary>
        private readonly IUnityContainer unityContainer = null;

        #endregion

        #region Constructor

        public TenantContactController(IUnityContainer unityContainer)
        {
            this.unityContainer = unityContainer;
            this.tenantContactManager = new TenantContactManager(this.unityContainer);
        }

        #endregion

        #region Public Methods

        #region Add

        /// <summary>
        /// This api call use to add single or list of tenantContact
        /// </summary>
        /// <param name="tenantContacts">
        /// List of tenantContacts
        /// </param>
        /// <returns>Returns true if added succesfully otherwise false</returns>
        [HttpPost]
        public bool Add(IList<TenantContact> tenantContacts)
        {
            bool result = false;
            try
            {

                string tenantCode = tenantContacts.FirstOrDefault().TenantCode;
                result = this.tenantContactManager.AddTenantContacts(tenantContacts, tenantCode);
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
        /// This api call use to update single or list of tenantContact
        /// </summary>
        /// <param name="tenantContacts">
        /// List of tenantContacts
        /// </param>
        /// <returns>Returns true if updated succesfully otherwise false</returns>
        [HttpPost]
        public bool Update(IList<TenantContact> tenantContacts)
        {
            bool result = false;
            try
            {
                string tenantCode = tenantContacts.FirstOrDefault().TenantCode;
                result = this.tenantContactManager.UpdateTenantContacts(tenantContacts, tenantCode);
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
        /// This api call use to add single or list of tenantContact
        /// </summary>
        /// <param name="tenantContacts">
        /// List of tenantContacts
        /// </param>
        /// <returns>Returns true if deleted succesfully otherwise false</returns>
        [HttpPost]
        public bool Delete(IList<TenantContact> tenantContacts)
        {
            bool result = false;
            try
            {
                string tenantCode = tenantContacts.FirstOrDefault().TenantCode;
                result = this.tenantContactManager.DeleteTenantContacts(tenantContacts, tenantCode);
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
        /// This api call use to get single or list of tenantContact
        /// </summary>
        /// <param name="tenantContacts">
        /// List of tenantContacts
        /// </param>
        /// <returns>Returns list of tenantContacts</returns>
        [HttpPost]
        public IList<TenantContact> List(TenantContactSearchParameter tenantContactSearchParameter)
        {
            IList<TenantContact> tenantContacts = new List<TenantContact>();
            try
            {
                ///string tenantCode = Helper.CheckTenantCode(Request.Headers);
                string tenantCode = tenantContactSearchParameter.TenantCode;
                tenantContacts = this.tenantContactManager.GetTenantContacts(tenantContactSearchParameter, tenantCode);
                HttpContext.Current.Response.AppendHeader("recordCount", this.tenantContactManager.GetTenantContactCount(tenantContactSearchParameter, tenantCode).ToString());
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return tenantContacts;
        }

        #endregion

        #region Detail

        /// <summary>
        /// This api used to get single tenantContact record by gievn Id.
        /// </summary>
        /// <param name="tenantContactIdentifier">The tenantContact identifier</param>
        /// <returns>Rerurns details of tenantContact</returns>
        [HttpGet]
        public TenantContact Detail(long tenantContactIdentifier)
        {
            IList<TenantContact> tenantContacts = new List<TenantContact>();
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                TenantContactSearchParameter tenantContactSearchParameter = new TenantContactSearchParameter();
                tenantContactSearchParameter.Identifier = tenantContactIdentifier.ToString();
                tenantContactSearchParameter.SortParameter.SortColumn = "Id";
                tenantContacts = this.tenantContactManager.GetTenantContacts(tenantContactSearchParameter, tenantCode);

                if (tenantContacts?.Count <= 0)
                {
                    throw new TenantContactNotFoundException(tenantCode);
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return tenantContacts.First();
        }

        #endregion

        #endregion

        #region Activate

        /// <summary>
        /// This method used to activate tenantContact
        /// </summary>
        /// <param name="tenantContactIdentifier">The tenantContact identifier</param>
        /// <returns>Returns true if activated successfully otherwise false</returns>
        [HttpGet]
        public bool Activate(long tenantContactIdentifier)
        {
            bool result = false;
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                result = this.tenantContactManager.ActivateTenantContact(tenantContactIdentifier, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return result;
        }


        /// <summary>
        /// This api call use to add single or list of tenantContact
        /// </summary>
        /// <param name="tenantContacts">
        /// List of tenantContacts
        /// </param>
        /// <returns>Returns true if added succesfully otherwise false</returns>
        [HttpPost]
        public bool SendActivationLink(IList<TenantContact> tenantContacts)
        {
            bool result = false;
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                result = this.tenantContactManager.SentActivationLink(tenantContacts, tenantCode);
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
        /// This method used to deactivate tenantContact
        /// </summary>
        /// <param name="tenantContactIdentifier">The tenantContact identifier</param>
        /// <returns>Returns true if deactivated successfully otherwise false</returns>
        [HttpGet]
        public bool DeActivate(long tenantContactIdentifier)
        {
            bool result = false;
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                result = this.tenantContactManager.DeActivateTenantContact(tenantContactIdentifier, tenantCode);
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
