// <copyright file="ClientController.cs.cs" company="Websym Solutions Pvt Ltd">
// Copyright (c) 2017 Websym Solutions Pvt Ltd.
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
    /// This class represents the api controller methods for client.
    /// </summary>
    public class ClientController : ApiController
    {
        #region Private Members

        /// <summary>
        /// The unitycontainer
        /// </summary>
        readonly IUnityContainer container;

        /// <summary>
        /// The client manager object
        /// </summary>
        private ClientManager clientManager = null;

        /// <summary>
        /// The tenant code.
        /// </summary>
        private string tenantCode = null;

        /// <summary>
        /// The utility object
        /// </summary>
        private IUtility utility = null;

        #endregion

        #region Constructor

        public ClientController(IUnityContainer container)
        {
            this.container = container;
            this.clientManager = new ClientManager(container);
            this.utility = new Utility();
        }

        #endregion

        #region Public Functions

        /// <summary>
        /// This method adds the list of clients
        /// </summary>
        /// <param name="clients">The clients.</param>
        /// <returns>
        /// Returns true if clients are added successfully, false otherwise
        /// </returns>
        /// <exception cref="TenantNotFoundException">This exception is raised when tenant code is not specified in the request header</exception>
        [HttpPost]
        public bool Add(IList<Client> clients)
        {
            bool result = false;
            try
            {
                if (!Request.Headers.Contains(ModelConstant.TENANT_CODE_KEY))
                {
                    throw new TenantNotFoundException(ModelConstant.DEFAULT_TENANT_CODE);
                }

                this.tenantCode = ((IEnumerable<string>)Request.Headers.GetValues(ModelConstant.TENANT_CODE_KEY)).FirstOrDefault();
                result = this.clientManager.AddClients(clients, this.tenantCode);
                return result;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// This method updates the list of clients.
        /// </summary>
        /// <param name="clients">The clients.</param>
        /// <returns>
        /// Returns true if client udpated successfully, false otherwise
        /// </returns>
        /// <exception cref="TenantNotFoundException">This exception is raised when tenant code is not specified in the request header</exception>
        [HttpPost]
        public bool Update(IList<Client> clients)
        {
            bool result = false;
            try
            {
                if (!Request.Headers.Contains(ModelConstant.TENANT_CODE_KEY))
                {
                    throw new TenantNotFoundException(ModelConstant.DEFAULT_TENANT_CODE);
                }

                this.tenantCode = ((IEnumerable<string>)Request.Headers.GetValues(ModelConstant.TENANT_CODE_KEY)).FirstOrDefault();
                result = this.clientManager.UpdateClients(clients, this.tenantCode);
                return result;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// This method deletes the specified list of clients.
        /// </summary>
        /// <param name="clients">The clients.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns>
        /// Returns true, if the list of clients are deleted successfully. False otherwise
        /// </returns>
        /// <exception cref="TenantNotFoundException">This exception is raised when tenant code is not specified in the request header</exception>
        [HttpPost]
        public bool Delete(IList<Client> clients)
        {
            bool result = false;
            try
            {
                if (!Request.Headers.Contains(ModelConstant.TENANT_CODE_KEY))
                {
                    throw new TenantNotFoundException(ModelConstant.DEFAULT_TENANT_CODE);
                }

                this.tenantCode = ((IEnumerable<string>)Request.Headers.GetValues(ModelConstant.TENANT_CODE_KEY)).FirstOrDefault();
                result = this.clientManager.DeleteClients(clients, this.tenantCode);
                return result;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// This method will call get clients method from client manager.
        /// </summary>
        /// <param name="clientSearchParameter">The client search parameters.</param>
        /// <returns>
        /// Returns tags if found for given parameters, else return null
        /// </returns>
        [HttpPost]
        public IList<Client> Get(ClientSearchParameter clientSearchParameter)
        {
            IList<Client> clients = new List<Client>();
            try
            {
                if (!Request.Headers.Contains(ModelConstant.TENANT_CODE_KEY))
                {
                    throw new TenantNotFoundException(ModelConstant.DEFAULT_TENANT_CODE);
                }

                this.tenantCode = ((IEnumerable<string>)Request.Headers.GetValues(ModelConstant.TENANT_CODE_KEY)).FirstOrDefault();
                clients = this.clientManager.GetClients(clientSearchParameter, this.tenantCode);

                //// Remove pagination parameter from search in order to get the total count of clients
                clientSearchParameter.PagingParameter.PageIndex = 0;
                clientSearchParameter.PagingParameter.PageSize = 0;

                IList<Client> retrievedClients = this.clientManager.GetClients(clientSearchParameter, this.tenantCode);
                if (retrievedClients != null && retrievedClients.Count > 0)
                {
                    HttpContext.Current.Response.AppendHeader("recordCount", retrievedClients.Count.ToString());
                }
                else
                {
                    HttpContext.Current.Response.AppendHeader("recordCount", "0");
                }

                return clients;
            }
            catch
            {
                throw;
            }
        }

        [HttpPost]
        public bool Activate(IList<Client> clients)
        {
            bool result = false;
            try
            {
                //this.tenantCode = ((IEnumerable<string>)Request.Headers.GetValues(ModelConstant.TENANT_CODE_KEY)).FirstOrDefault();
                //this.tenantCode = ModelConstant.DEFAULT_TENANT_CODE;
                //string tenantBaseURL = ConfigurationManager.AppSettings[ModelConstants.TENANTBASEURL];
                //result = JsonConvert.DeserializeObject<bool>(this.utility.ExecuteWebRequest(tenantBaseURL, "Tenant", "Activate", JsonConvert.SerializeObject(clients), ModelConstant.TENANT_CODE_KEY, tenantCode));

                return result;
            }
            catch
            {
                throw;
            }
        }

        [HttpPost]
        public bool Deactivate(IList<Client> clients)
        {
            bool result = false;
            try
            {
                //this.tenantCode = ((IEnumerable<string>)Request.Headers.GetValues(ModelConstant.TENANT_CODE_KEY)).FirstOrDefault();
                //this.tenantCode = ModelConstant.DEFAULT_TENANT_CODE;
                //string tenantBaseURL = ConfigurationManager.AppSettings[ModelConstants.TENANTBASEURL];
                //result = JsonConvert.DeserializeObject<bool>(this.utility.ExecuteWebRequest(tenantBaseURL, "Tenant", "Deactivate", JsonConvert.SerializeObject(clients), ModelConstant.TENANT_CODE_KEY, tenantCode));

                return result;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// This method adds the list of clients
        /// </summary>
        /// <param name="clients">The clients.</param>
        /// <returns>
        /// Returns true if clients are added successfully, false otherwise
        /// </returns>
        /// <exception cref="TenantNotFoundException">This exception is raised when tenant code is not specified in the request header</exception>
        [HttpPost]
        public bool Register(IList<Client> clients)
        {
            bool result = false;
            try
            {
                this.tenantCode = ModelConstant.DEFAULT_TENANT_CODE;
                result = this.clientManager.AddClients(clients, this.tenantCode, true);
                return result;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// This api method will helps to upgrade tenant subscriptions.
        /// </summary>
        /// <param name="clients">
        /// The list of client.
        /// </param>
        /// <returns>
        /// It will return true if successfully updated.
        /// </returns>
        //[HttpPost]
        //public bool UpgradeSubscriptions(IList<Client> clients)
        //{
        //    try
        //    {
        //        if (!Request.Headers.Contains(ModelConstant.TENANT_CODE_KEY))
        //        {
        //            throw new TenantNotFoundException(ModelConstant.DEFAULT_TENANT_CODE);
        //        }

        //        this.tenantCode = ((IEnumerable<string>)Request.Headers.GetValues(ModelConstant.TENANT_CODE_KEY)).FirstOrDefault();
        //        return this.clientManager.UpgradeSubscriptions(clients, this.tenantCode);
        //    }
        //    catch (Exception exception)
        //    {
        //        throw exception;
        //    }
        //}


        #endregion
    }
}
