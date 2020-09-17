// <copyright file="ClientManager.cs" company="Websym Solutions Pvt Ltd">
// Copyright (c) 2017 Websym Solutions Pvt Ltd.
// </copyright>
// -----------------------------------------------------------------------  

namespace nIS
{


    #region References


    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Net.Http;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using System.Transactions;
    using Unity;
    using Websym.Core.EntityManager;
    using Websym.Core.TenantManager;

    #endregion

    /// <summary>
    /// This class represents client model
    /// </summary>
    public class ClientManager
    {
        #region Private Members

        /// <summary>
        /// The unity container object
        /// </summary>
        private IUnityContainer unityContainer = null;

        /// <summary>
        /// The user manager object
        /// </summary>
        private UserManager userManager = null;

        /// <summary>
        /// The user manager object
        /// </summary>
        private CountryManager countryManager = null;

        /// <summary>
        /// The utility object
        /// </summary>
        private IUtility utility = null;

        /// <summary>
        /// The utility object
        /// </summary>
        private IConfigurationUtility configurationUtility = null;

        /// <summary>
        /// The subscription id
        /// </summary>
        private string subscriptionId = string.Empty;

        /// <summary>
        /// The tenant id
        /// </summary>
        private string tenantId = string.Empty;

        /// <summary>
        /// The client id
        /// </summary>
        private string clientId = string.Empty;

        /// <summary>
        /// The client secret
        /// </summary>
        private string clientSecret = string.Empty;

        /// <summary>
        /// The resource group name.
        /// </summary>
        private string resourceGroupName = string.Empty;

        /// <summary>
        /// The validation engine
        /// </summary>
        private IValidationEngine validationEngine = new ValidationEngine();

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientManager"/> class.
        /// </summary>
        /// <param name="unityContainer">The unity container.</param>
        public ClientManager(IUnityContainer unityContainer)
        {
            this.unityContainer = unityContainer;
            this.userManager = new UserManager(this.unityContainer);
            this.countryManager = new CountryManager(this.unityContainer);
            this.subscriptionId = ConfigurationManager.AppSettings["AzureSubscriptionID"]?.ToString();
            this.tenantId = ConfigurationManager.AppSettings["AzureTenantId"]?.ToString();
            this.clientId = ConfigurationManager.AppSettings["AzureAppClientId"]?.ToString();
            this.clientSecret = ConfigurationManager.AppSettings["AzureAppClientSecret"]?.ToString();
            this.resourceGroupName = ConfigurationManager.AppSettings["ResourceGroupName"]?.ToString();
            this.utility = new Utility();
            this.configurationUtility = new ConfigurationUtility(this.unityContainer);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// This method adds the specified list of clients.
        /// </summary>
        /// <param name="clients">The clients.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns></returns>
        /// <exception cref="DuplicateClientException">This exception is raised when the list of clients contains duplicate clients</exception>
        /// <exception cref="InvalidUserContentException">This exception is raised when the created user for the client cannot be added in repository</exception>
        /// <exception cref="RoleNotFoundException">This exception is raised when the created role is not found in the repository</exception>
        /// <exception cref="InvalidRoleException">This exception is raised when the role associated with client is not found in the repository</exception>
        /// <exception cref="InvalidEntityException">This exception is raised when the entity associated with client is not found in the repository</exception>
        /// <exception cref="InvalidClientException">This exception is raised when the specified client is not found in the repository</exception>
        /// <exception cref="System.NullReferenceException">This exception is raised when the specified client is passed as null</exception>
        public bool AddClients(IList<Client> clients, string tenantCode, bool addRegisterFlag = false)
        {
            bool result = false;
            string domainNamePattern = @"[^a-zA-Z0-9]";
            List<Client> clientsToBeDeleted = new List<Client>();
            try
            {
                string sourceConnectionString = string.Empty;

                #region Get primary Client to get source connection string 

                IList<Client> retrivedClients = this.GetClients(new ClientSearchParameter()
                {
                    SortParameter = new SortParameter()
                    {
                        SortColumn = ModelConstant.SORT_COLUMN
                    },
                    TenantCode = ModelConstant.DEFAULT_TENANT_CODE
                    //IsPrimaryTenant = true
                }, tenantCode);

                if (retrivedClients == null || retrivedClients.Count == 0)
                {
                    throw new TenantNotFoundException(String.Empty);
                }

                if (retrivedClients?.Count > 0)
                {
                    if (retrivedClients.FirstOrDefault() != null)
                    {
                        sourceConnectionString = retrivedClients.FirstOrDefault().StorageAccount;
                    }
                }

                #endregion

                IList<Tenant> tenants = new List<Tenant>();
                //// Assign primary contact
                clients.ToList().ForEach(client =>
                {
                    Tenant tenant = new Tenant();
                    client.Contacts.ToList().ForEach(item =>
                    {
                        if (item.ContactType.Equals("Primary"))
                        {
                            client.PrimaryFirstName = item.FirstName;
                            client.PrimaryLastName = item.LastName;
                            client.PrimaryEmailAddress = item.EmailAddress;
                            client.PrimaryContactNumber = item.ContactNumber; //item.CountryCode + "-" +

                            tenant.PrimaryFirstName = item.FirstName;
                            tenant.PrimaryLastName = item.LastName;
                            tenant.PrimaryEmailAddress = item.EmailAddress;
                            tenant.PrimaryContactNumber = item.ContactNumber; //item.CountryCode + "-" +
                        }
                        else if (item.ContactType.Equals("Secondary"))
                        {
                            tenant.SecondaryContactName = item.FirstName;
                            tenant.SecondaryLastName = item.LastName;
                            tenant.SecondaryEmailAddress = item.EmailAddress;
                            tenant.SecondaryContactNumber = item.ContactNumber; //item.CountryCode + "-" + 
                        }
                        else if (item.ContactType.Equals("Billing"))
                        {
                            tenant.BillingFirstName = item.FirstName;
                            tenant.BillingLastName = item.LastName;
                            tenant.BillingEmailAddress = item.EmailAddress;
                            tenant.BillingContactNumber = item.ContactNumber; //item.CountryCode + "-" +
                        }
                    });

                    //// Assign compulsary properties.
                    tenant.TenantCode = client.TenantCode;
                    tenant.TenantName = client.TenantName;
                    tenant.TenantDomainName = client.TenantDomainName;
                    tenant.TenantType = client.TenantType;
                    tenant.PrimaryPinCode = client.PrimaryPinCode;
                    tenant.PrimaryAddressLine1 = client.PrimaryAddressLine1;
                    tenant.PrimaryAddressLine2 = client.PrimaryAddressLine2;
                    tenant.StorageAccount = client.StorageAccount;
                    tenant.AccessToken = client.AccessToken;
                    tenant.StartDate = client.StartDate;
                    tenant.EndDate = client.EndDate;
                    tenant.ManageType = client.ManageType;
                    tenant.PanNumber = client.PanNumber;
                    tenant.ServiceTax = client.ServiceTax;
                    tenant.TenantCity = client.TenantCity;
                    tenant.TenantCountry = client.TenantCountry;
                    tenant.TenantState = client.TenantState;
                    tenant.IsActive = client.IsActive;
                    tenant.TenantDescription = client.TenantDescription;
                    tenant.TenantLogo = client.TenantLogo;
                    //tenant.AuthenticationMode = client.AuthenticationMode;

                    tenants.Add(tenant);
                });

                ClientSearchParameter clientSearchParameter = new ClientSearchParameter()
                {
                    SortParameter = new SortParameter()
                    {
                        SortColumn = "TenantCode"
                    },
                };

                clients.ToList().ForEach(client =>
                {
                    clientSearchParameter.TenantDomainName = client.TenantDomainName;

                    retrivedClients = this.GetClients(clientSearchParameter, tenantCode);
                    if (retrivedClients?.Count > 0)
                    {
                        throw new DuplicateClientException(tenantCode);
                    }
                });

                this.IsValidClients(clients, ModelConstant.ADD_OPERATION, tenantCode, false, addRegisterFlag);

                IList<User> users = null;
                this.configurationUtility.AddTenant(tenants);
                if (!result)
                {
                    throw new InvalidClientException(tenantCode);
                }

                try
                {
                    clients.ToList().ForEach(client =>
                    {
                        using (TransactionScope transactionScope = new TransactionScope(TransactionScopeOption.Suppress))
                        {
                            #region Assign Default entity

                            IList<Entity> tenantEntities = new List<Entity>();

                            var entitySearchParameter = new EntitySearchParameter();
                            entitySearchParameter.SortParameter.SortColumn = "Id";
                            entitySearchParameter.SortParameter.SortOrder = Websym.Core.EntityManager.SortOrder.Ascending;
                            //   IList<Entity> entities = entitymanager.GetEntities(entitySearchParameter, ModelConstant.DEFAULT_TENANT_CODE);
                            IList<Entity> entities = JsonConvert.DeserializeObject<List<Entity>>(this.utility.ExecuteWebRequest(ConfigurationManager.AppSettings[ModelConstant.ENTITY_BASE_URL], "Entity", "Get", JsonConvert.SerializeObject(entitySearchParameter), ModelConstant.TENANT_CODE_KEY, ModelConstant.DEFAULT_TENANT_CODE));

                            entities.ToList().ForEach(data =>
                            {
                                Entity entity = new Entity()
                                {
                                    EntityName = data.EntityName,
                                    ComponentCode = ModelConstant.COMPONENTCODE,
                                    // Operations = ModelConstant.CLIENTOPERATION.ToList()
                                };
                                tenantEntities.Add(entity);
                            });
                            // Assign entities to client
                            //client.Entities = tenantEntities;
                            //// Call Add Entity method in Entity manager
                            bool addEntityResult = JsonConvert.DeserializeObject<bool>(this.utility.ExecuteWebRequest(ConfigurationManager.AppSettings[ModelConstant.ENTITY_BASE_URL], "Entity", "Add", JsonConvert.SerializeObject(client.Entities), ModelConstant.TENANT_CODE_KEY, client.TenantCode));
                            if (!addEntityResult)
                            {
                                // throw new InvalidEntityException(tenantCode);
                            }

                            #endregion


                            transactionScope.Complete();
                        };

                        #region Add Default User

                        IList<User> clientusers = new List<User>();
                        IList<Role> clientRoles = new List<Role>();
                        clientRoles = new RoleManager(this.unityContainer).GetRoles(new RoleSearchParameter()
                        {
                            SortParameter = new SortParameter() { SortColumn = ModelConstant.SORT_COLUMN },
                            Name = "Administrator"
                        }, client.TenantCode);
                        clientusers.Add(new User()
                        {
                            FirstName = client.PrimaryFirstName == "" ? client.TenantName : client.PrimaryFirstName,
                            LastName = client.PrimaryLastName == "" ? client.TenantName : client.PrimaryLastName,
                            EmailAddress = client.PrimaryEmailAddress,
                            ContactNumber = client.PrimaryContactNumber,
                            Roles = clientRoles
                        });

                        bool addUserResult = this.userManager.AddUsers(clientusers, client.TenantCode);
                        if (!addUserResult)
                        {
                            throw new InvalidUserException(tenantCode);
                        }

                        #endregion

                    });
                }
                catch (Exception ex)
                {
                    //// This will rollback the role and user
                    throw ex;
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        /// <summary>
        /// This method updates the list of specified clients.
        /// </summary>
        /// <param name="clients">The clients.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns>
        /// Returns true, if clients were updated successfully. False otherwise
        /// </returns>
        /// <exception cref="DuplicateClientException">This exception is raised when the list of clients contains duplicate clients</exception>
        /// <exception cref="TenantNotFoundException">This exception is raised when the specified client is not found in the repository</exception>
        /// <exception cref="InvalidRoleException">This exception is raised when the role associated with client is not found in the repository</exception>
        /// <exception cref="InvalidEntityException">This exception is raised when the entity associated with client is not found in the repository</exception>
        /// <exception cref="InvalidClientException">This exception is raised when the specified client is not found in the repository</exception>
        /// <exception cref="System.NullReferenceException">This exception is raised when the specified client is passed as null</exception>
        public bool UpdateClients(IList<Client> clients, string tenantCode)
        {
            bool result = false;
            try
            {
                this.IsValidClients(clients, ModelConstant.UPDATE_OPERATION, tenantCode, true);

                IList<Client> nonUpdatedClients = new List<Client>();
                ClientSearchParameter clientSearchParameter = new ClientSearchParameter();
                clientSearchParameter.SortParameter.SortColumn = ModelConstant.SORT_COLUMN;
                clientSearchParameter.SortParameter.SortOrder = SortOrder.Ascending;
                clientSearchParameter.SearchMode = SearchMode.Equals;

                //// This for loop helps to create a list of client which take the current snapshot of data in client and entity from backend
                //// which will be used in the scenario when we want to rollback the changes when an exception is raised
                clients.ToList().ForEach(client =>
                {
                    clientSearchParameter.TenantCode = client.TenantCode;
                    Client nonUpdatedClient = this.GetClients(clientSearchParameter, tenantCode).FirstOrDefault();
                    if (nonUpdatedClient == null)
                    {
                        throw new TenantNotFoundException(tenantCode);
                    }

                    nonUpdatedClients.Add(nonUpdatedClient);
                });

                IList<Tenant> tenants = new List<Tenant>();
                //// Assign primary contact.
                clients.ToList().ForEach(client =>
                {
                    Tenant tenant = new Tenant();
                    client.Contacts.ToList().ForEach(item =>
                    {
                        if (item.ContactType.Equals("Primary"))
                        {
                            client.PrimaryFirstName = item.FirstName;
                            client.PrimaryLastName = item.LastName;
                            client.PrimaryEmailAddress = item.EmailAddress;
                            client.PrimaryContactNumber = item.ContactNumber; //item.CountryCode + "-" + 

                            tenant.PrimaryFirstName = item.FirstName;
                            tenant.PrimaryLastName = item.LastName;
                            tenant.PrimaryEmailAddress = item.EmailAddress;
                            tenant.PrimaryContactNumber = item.ContactNumber; //item.CountryCode + "-" +
                        }
                        else if (item.ContactType.Equals("Secondary"))
                        {
                            tenant.SecondaryContactName = item.FirstName;
                            tenant.SecondaryLastName = item.LastName;
                            tenant.SecondaryEmailAddress = item.EmailAddress;
                            tenant.SecondaryContactNumber = item.ContactNumber; //item.CountryCode + "-" +
                        }
                        else if (item.ContactType.Equals("Billing"))
                        {
                            tenant.BillingFirstName = item.FirstName;
                            tenant.BillingLastName = item.LastName;
                            tenant.BillingEmailAddress = item.EmailAddress;
                            tenant.BillingContactNumber = item.ContactNumber; //item.CountryCode + "-" +
                        }

                    });

                    //// Assign compulsary properties.
                    tenant.TenantCode = client.TenantCode;
                    tenant.TenantName = client.TenantName;
                    tenant.TenantDomainName = client.TenantDomainName;
                    //tenant.TenantType = client.TenantType;
                    tenant.PrimaryPinCode = client.PrimaryPinCode;
                    tenant.PrimaryAddressLine1 = client.PrimaryAddressLine1;
                    //tenant.PrimaryAddressLine2 = client.PrimaryAddressLine2;
                    tenant.StorageAccount = client.StorageAccount;
                    tenant.AccessToken = "Dummy";
                    tenant.StartDate = client.StartDate;
                    tenant.EndDate = client.EndDate;
                    tenant.ManageType = client.ManageType;
                    tenant.PanNumber = client.PanNumber;
                    tenant.ServiceTax = client.ServiceTax;
                    tenant.TenantCity = client.TenantCity;
                    tenant.TenantCountry = client.TenantCountry;
                    //tenant.AuthenticationMode = client.AuthenticationMode;
                    if (client.ManageType.Equals(1))
                    {
                        tenant.IsPrimaryTenant = true;
                    }

                    tenant.TenantState = client.TenantState;
                    tenant.IsActive = client.IsActive;
                    tenant.TenantDescription = client.TenantDescription;
                    tenant.TenantLogo = client.TenantLogo;
                    tenants.Add(tenant);
                });

                clientSearchParameter = new ClientSearchParameter() { SortParameter = new SortParameter() { SortColumn = "TenantCode" }, SearchMode = SearchMode.Equals };
                clients.ToList().ForEach(client =>
                {
                    clientSearchParameter.TenantDomainName = client.TenantDomainName;

                    IList<Client> retrivedClients = this.GetClients(clientSearchParameter, tenantCode);
                    if (retrivedClients?.Count > 0)
                    {
                        retrivedClients = retrivedClients.Where(item => item.TenantCode != client.TenantCode).ToList();
                        if (retrivedClients?.Count > 0)
                        {
                            throw new DuplicateClientException(tenantCode);
                        }
                    }

                    clientSearchParameter.TenantDomainName = string.Empty;

                    //clientSearchParameter.PrimaryEmailAddress = client.PrimaryEmailAddress;
                    //retrivedClients = this.GetClients(clientSearchParameter, tenantCode);
                    //if (retrivedClients?.Count > 0)
                    //{
                    //    retrivedClients = retrivedClients.Where(item => item.TenantCode != client.TenantCode).ToList();
                    //    if (retrivedClients?.Count > 0)
                    //    {
                    //        throw new DuplicateClientException(tenantCode);
                    //    }
                    //}

                    //clientSearchParameter.PrimaryEmailAddress = string.Empty;
                });

                //// Update the client using Tenant manger API
                string tenantBaseURL = ConfigurationManager.AppSettings[ModelConstant.TENANT_BASE_URL];
                result = JsonConvert.DeserializeObject<bool>(this.utility.ExecuteWebRequest(tenantBaseURL, "Tenant", "Update", JsonConvert.SerializeObject(tenants), ModelConstant.TENANT_CODE_KEY, tenantCode));
                if (!result)
                {
                    throw new InvalidClientException(tenantCode);
                }

                return result;
            }
            catch (Exception ex)
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
        /// <exception cref="DuplicateClientException">This exception is raised when the list of clients contains duplicate clients</exception>
        /// <exception cref="System.NullReferenceException">This exception is raised when the specified client is passed as null</exception>
        public bool DeleteClients(IList<Client> clients, string tenantCode)
        {
            bool result = false;
            try
            {

                this.IsValidClients(clients, ModelConstant.DELETE_OPERATION, tenantCode, false);
                //// Call Delete method of Tenant Manager
                string tenantBaseURL = ConfigurationManager.AppSettings[ModelConstant.TENANT_BASE_URL];

                IList<Entity> entities = null;

                EntitySearchParameter entitySearchParameter = new EntitySearchParameter();
                entitySearchParameter.SortParameter.SortColumn = "Id";
                entitySearchParameter.SortParameter.SortOrder = Websym.Core.EntityManager.SortOrder.Ascending;
                entitySearchParameter.SearchMode = Websym.Core.EntityManager.SearchMode.Exact;

                IList<Websym.Core.EventManager.Event> events = null;

                string eventManagerBaseURL = ConfigurationManager.AppSettings["EventManagerBaseURL"]?.ToString();
                //  IList<Event> defaultevents = null;
                Websym.Core.EventManager.EventSearchParameter eventSearchparameter = new Websym.Core.EventManager.EventSearchParameter()
                {
                    ComponentCode = ModelConstant.COMPONENTCODE,
                    //EntityName = entityName,
                    //EventName = eventName,
                    SortParameter = new Websym.Core.EventManager.SortParameter()
                    {
                        SortColumn = "EntityName"
                    }
                };

                //Delete all dependant data of client
                clients.ToList().ForEach(client =>
                {
                    #region Delete User

                    try
                    {
                        //Get roles of particulat tenant
                        UserSearchParameter userSearchParameter = new UserSearchParameter();
                        userSearchParameter.SortParameter = new SortParameter() { SortColumn = ModelConstant.SORT_COLUMN };
                        IList<User> clientUser = new UserManager(unityContainer).GetUsers(userSearchParameter, client.TenantCode);
                        if (clientUser?.Count > 0)
                        {
                            new UserManager(unityContainer).DeleteUsers(clientUser, client.TenantCode);
                        }
                    }
                    catch (Exception)
                    {
                        throw;
                    }

                    #endregion

                    #region Delete Role

                    try
                    {
                        //Get roles of particulat tenant
                        RoleSearchParameter roleSearchParameter = new RoleSearchParameter();
                        roleSearchParameter.SortParameter = new SortParameter() { SortColumn = ModelConstant.SORT_COLUMN };
                        IList<Role> clientRoles = new RoleManager(unityContainer).GetRoles(roleSearchParameter, client.TenantCode);
                        if (clientRoles?.Count > 0)
                        {
                            new RoleManager(unityContainer).DeleteRoles(clientRoles, client.TenantCode);
                        }
                    }
                    catch (Exception)
                    {
                        throw;
                    }

                    #endregion

                });

                //delete client
                result = JsonConvert.DeserializeObject<bool>(this.utility.ExecuteWebRequest(tenantBaseURL, "Tenant", "Delete", JsonConvert.SerializeObject(clients), ModelConstant.TENANT_CODE_KEY, tenantCode));

                return result;
            }
            catch
            {
                throw;
            }
        }

        public bool ActivateClients(string tenantCode)
        {
            bool result = false;
            try
            {
                // this.IsValidClients(clients, ModelConstant.UPDATE_OPERATION, tenantCode, true);

                ClientSearchParameter clientSearchParameter = new ClientSearchParameter();
                clientSearchParameter.SortParameter.SortColumn = ModelConstant.SORT_COLUMN;
                clientSearchParameter.SortParameter.SortOrder = SortOrder.Ascending;
                clientSearchParameter.SearchMode = SearchMode.Equals;

                clientSearchParameter = new ClientSearchParameter() { SortParameter = new SortParameter() { SortColumn = "TenantCode" }, SearchMode = SearchMode.Equals };
                clientSearchParameter.TenantCode = tenantCode;

                IList<Client> retrivedClients = this.GetClients(clientSearchParameter, tenantCode);
                if (retrivedClients?.Count > 0)
                {
                    IList<Tenant> tenants = new List<Tenant>();
                    //// Assign primary contact.
                    retrivedClients.ToList().ForEach(client =>
                    {
                        Tenant tenant = new Tenant(); //// Assign compulsary properties.
                        tenant.IsActive = false;
                        tenant.TenantCode = tenantCode;
                        tenants.Add(tenant);
                    });

                    //// Update the client using Tenant manger API
                    string tenantBaseURL = ConfigurationManager.AppSettings[ModelConstant.TENANT_BASE_URL];
                    result = JsonConvert.DeserializeObject<bool>(this.utility.ExecuteWebRequest(tenantBaseURL, "Tenant", "Activate", JsonConvert.SerializeObject(tenants), ModelConstant.TENANT_CODE_KEY, tenantCode));
                    if (!result)
                    {
                        throw new InvalidClientException(tenantCode);
                    }
                }


                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public bool DeactivateClients(string tenantCode)
        {
            bool result = false;
            try
            {
                // this.IsValidClients(clients, ModelConstant.UPDATE_OPERATION, tenantCode, true);

                ClientSearchParameter clientSearchParameter = new ClientSearchParameter();
                clientSearchParameter.SortParameter.SortColumn = ModelConstant.SORT_COLUMN;
                clientSearchParameter.SortParameter.SortOrder = SortOrder.Ascending;
                clientSearchParameter.SearchMode = SearchMode.Equals;

                clientSearchParameter = new ClientSearchParameter() { SortParameter = new SortParameter() { SortColumn = "TenantCode" }, SearchMode = SearchMode.Equals };
                clientSearchParameter.TenantCode = tenantCode;

                IList<Client> retrivedClients = this.GetClients(clientSearchParameter, tenantCode);
                if (retrivedClients?.Count > 0)
                {
                    IList<Tenant> tenants = new List<Tenant>();
                    //// Assign primary contact.
                    retrivedClients.ToList().ForEach(client =>
                    {
                        Tenant tenant = new Tenant(); //// Assign compulsary properties.
                        tenant.IsActive = false;
                        tenant.TenantCode = tenantCode;
                        tenants.Add(tenant);
                    });

                    //// Update the client using Tenant manger API
                    string tenantBaseURL = ConfigurationManager.AppSettings[ModelConstant.TENANT_BASE_URL];
                    result = JsonConvert.DeserializeObject<bool>(this.utility.ExecuteWebRequest(tenantBaseURL, "Tenant", "Deactivate", JsonConvert.SerializeObject(tenants), ModelConstant.TENANT_CODE_KEY, tenantCode));
                    if (!result)
                    {
                        throw new InvalidClientException(tenantCode);
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// This method gets the clients as per the specified search parameter
        /// </summary>
        /// <param name="clientSearchParameter">The client search parameter.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <param name="isEntitiesRequired">if set to <c>true</c> [is entities required].</param>
        /// <returns>
        /// Returns the list of clients as per the specified search parameter
        /// </returns>
        /// <exception cref="InvalidEntityException">This exception is raised when the retrieved client does not have any list of entities associated with it</exception>
        public IList<Client> GetClients(ClientSearchParameter clientSearchParameter, string tenantCode)
        {
            IList<Client> clients = null;
            try
            {
                try
                {
                    clientSearchParameter.IsValid();
                }
                catch (Exception ex)
                {
                    InvalidClientSearchParameter invalidClientSearchParameter = new InvalidClientSearchParameter(tenantCode);
                    invalidClientSearchParameter.Data.Add("InvalidSearchParameter", ex.Data);
                    throw invalidClientSearchParameter;
                }

                string tenantBaseURL = ConfigurationManager.AppSettings[ModelConstant.TENANT_BASE_URL];
                IList<Tenant> tenants = null;

                if (clientSearchParameter.IsSuperAdmin == true)
                {
                    clientSearchParameter.TenantCode = "";
                }
                TenantSearchParameter tenantSearchParameter = new TenantSearchParameter();
                tenantSearchParameter.SortingParameter = new Websym.Core.TenantManager.SortParameter();
                tenantSearchParameter.PagingParameter = new Websym.Core.TenantManager.PagingParameter();
                tenantSearchParameter.SortingParameter.SortColumn = clientSearchParameter.SortParameter.SortColumn;


                tenants = this.configurationUtility.GetTenant(tenantSearchParameter);
                if (tenants != null && tenants.Count > 0)
                {
                    string[] array;
                    clients = new List<Client>();
                    tenants.ToList().ForEach(tenant =>
                    {
                        Client client = new Client();
                        IList<Contact> contacts = new List<Contact>();
                        Contact contact = null;
                        if ((!(string.IsNullOrEmpty(tenant.PrimaryFirstName)) && (!string.IsNullOrEmpty(tenant.PrimaryLastName)) && (!string.IsNullOrEmpty(tenant.PrimaryEmailAddress))))
                        {
                            contact = new Contact();
                            contact.FirstName = tenant.PrimaryFirstName;
                            contact.LastName = tenant.PrimaryLastName;
                            contact.EmailAddress = tenant.PrimaryEmailAddress;
                            if (tenant.PrimaryContactNumber.Contains("-"))
                            {
                                array = tenant.PrimaryContactNumber.Split('-').ToArray();
                                contact.ContactNumber = array[1];
                                contact.CountryCode = array[0];
                            }
                            else
                            {
                                contact.ContactNumber = tenant.PrimaryContactNumber;
                            }
                            if ((!(string.IsNullOrEmpty(contact.FirstName)) && (!string.IsNullOrEmpty(contact.LastName)) && (!string.IsNullOrEmpty(contact.EmailAddress))))
                            {
                                contact.ContactType = "Primary";
                            }

                            contacts.Add(contact);
                        }
                        if (((!string.IsNullOrEmpty(tenant.SecondaryContactName)) && (!string.IsNullOrEmpty(tenant.SecondaryLastName)) && (!string.IsNullOrEmpty(tenant.SecondaryEmailAddress))))
                        {
                            contact = new Contact();
                            contact.FirstName = tenant.SecondaryContactName;
                            contact.LastName = tenant.SecondaryLastName;
                            contact.EmailAddress = tenant.SecondaryEmailAddress;
                            if (tenant.SecondaryContactNumber.Contains("-"))
                            {
                                array = tenant.SecondaryContactNumber.Split('-').ToArray();
                                contact.ContactNumber = array[1];
                                contact.CountryCode = array[0];
                            }
                            else
                            {
                                contact.ContactNumber = tenant.SecondaryContactNumber;
                            }

                            if (!(string.IsNullOrEmpty(contact.FirstName) && string.IsNullOrEmpty(contact.LastName) && string.IsNullOrEmpty(contact.EmailAddress)))
                            {
                                contact.ContactType = "Secondary";
                            }
                            contacts.Add(contact);
                        }
                        if (!(string.IsNullOrEmpty(tenant.BillingFirstName) && string.IsNullOrEmpty(tenant.BillingLastName) && string.IsNullOrEmpty(tenant.BillingEmailAddress)))
                        {
                            contact = new Contact();
                            contact.FirstName = tenant.BillingFirstName;
                            contact.LastName = tenant.BillingLastName;
                            contact.EmailAddress = tenant.BillingEmailAddress;
                            if (tenant.BillingContactNumber.Contains("-"))
                            {
                                array = tenant.BillingContactNumber.Split('-').ToArray();
                                contact.ContactNumber = array[1];
                                contact.CountryCode = array[0];
                            }
                            else
                            {
                                contact.ContactNumber = tenant.BillingContactNumber;
                            }
                            if (!(string.IsNullOrEmpty(contact.FirstName) && string.IsNullOrEmpty(contact.LastName) && string.IsNullOrEmpty(contact.EmailAddress)))
                            {
                                contact.ContactType = "Billing";
                            }

                            contacts.Add(contact);
                        }

                        #region Country State & City Get 

                        if (clientSearchParameter.IsCountryRequired == true)
                        {
                            Country tenantcountry = new CountryManager(unityContainer).GetCountries(new CountrySearchParameter()
                            {
                                Identifier = tenant.TenantCountry,
                                SortParameter = new SortParameter()
                                {
                                    SortColumn = ModelConstant.SORT_COLUMN
                                }
                            }, ModelConstant.DEFAULT_TENANT_CODE)?.FirstOrDefault();
                            client.Country = tenantcountry;

                        }

                        if (clientSearchParameter.IsStateRequired == true)
                        {
                            State tenantstate = new StateManager(unityContainer).GetStates(new StateSearchParameter()
                            {
                                Identifier = tenant.TenantState,
                                SortParameter = new SortParameter()
                                {
                                    SortColumn = ModelConstant.SORT_COLUMN
                                }
                            }, ModelConstant.DEFAULT_TENANT_CODE)?.FirstOrDefault();

                            client.State = tenantstate;
                        }

                        if (clientSearchParameter.IsCityRequired == true)
                        {
                            City tenantcity = new CityManager(unityContainer).GetCities(new CitySearchParameter()
                            {
                                Identifier = tenant.TenantCity,
                                SortParameter = new SortParameter()
                                {
                                    SortColumn = ModelConstant.SORT_COLUMN
                                }
                            }, ModelConstant.DEFAULT_TENANT_CODE)?.FirstOrDefault();

                            client.City = tenantcity;
                        }

                        #endregion


                        client.Contacts = contacts;
                        client.TenantCode = tenant.TenantCode;
                        client.TenantName = tenant.TenantName;
                        client.TenantDomainName = tenant.TenantDomainName;
                        client.PrimaryPinCode = tenant.PrimaryPinCode;
                        client.PrimaryAddressLine1 = tenant.PrimaryAddressLine1;
                        client.AccessToken = tenant.AccessToken;
                        client.StorageAccount = tenant.StorageAccount;
                        client.TenantCity = tenant.TenantCity;
                        client.TenantCountry = tenant.TenantCountry;
                        client.EndDate = tenant.EndDate;
                        client.StartDate = tenant.StartDate;
                        client.TenantDescription = tenant.TenantDescription;
                        client.TenantLogo = tenant.TenantLogo;
                        client.TenantState = tenant.TenantState;
                        client.TenantType = tenant.TenantType;
                        client.IsActive = tenant.IsActive;
                        client.PanNumber = tenant.PanNumber;
                        client.PrimaryAddressLine2 = tenant.PrimaryAddressLine2;
                        client.ServiceTax = tenant.ServiceTax;
                        client.ManageType = tenant.ManageType;
                        client.IsPrimaryTenant = tenant.IsPrimaryTenant;
                        client.PrimaryFirstName = tenant.PrimaryFirstName;
                        client.PrimaryLastName = tenant.PrimaryLastName;
                        client.PrimaryEmailAddress = tenant.PrimaryEmailAddress;
                        client.PrimaryContactNumber = tenant.PrimaryContactNumber;
                        // client.AuthenticationMode = tenant.AuthenticationMode;

                        clients.Add(client);

                    });
                }

            }
            catch
            {
                throw;
            }

            return clients;
        }

        /// <summary>
        /// This method gives the storage connection string.
        /// </summary>
        /// <param name="tenantCode">The tenantCode.</param>
        /// <returns>The storage connection string.</returns>
        public string GetStorageConnectionString(string tenantCode)
        {
            try
            {
                ////Get Client to create storage connection string
                ClientSearchParameter clientSearchParameter = new ClientSearchParameter()
                {
                    SortParameter = new SortParameter()
                    {
                        SortColumn = ModelConstant.SORT_COLUMN
                    },

                    //IsPlatformCredentialsRequired = true;
                };

                IList<Client> clients = this.GetClients(clientSearchParameter, tenantCode);
                if (clients == null || clients.Count == 0)
                {
                    throw new TenantNotFoundException(tenantCode);
                }

                string storageConnectionString = string.Empty;
                if (clients?.Count > 0)
                {
                    Client client = clients.Where(item => item.TenantCode.Equals(tenantCode)).FirstOrDefault();
                    if (client != null)
                    {
                        //storageConnectionString = ModelConstant.DEFAULT_ENDPOINTS_PROTOCOL + client.StorageAccount + ";" + ModelConstant.ACCOUNT_KEY + "= " + client.AccessToken;
                        storageConnectionString = client.StorageAccount;
                    }
                }

                return storageConnectionString;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This is responsible for get client count
        /// </summary>
        /// <param name="clientSearchParameter"></param>
        /// <param name="tenantCode"></param>
        /// <returns></returns>
        public long GetClientCount(ClientSearchParameter clientSearchParameter, string tenantCode)
        {
            long clientCount = 0;
            try
            {
                try
                {
                    clientSearchParameter.IsValid();
                }
                catch (Exception ex)
                {
                    InvalidClientSearchParameter invalidClientSearchParameter = new InvalidClientSearchParameter(tenantCode);
                    invalidClientSearchParameter.Data.Add("InvalidSearchParameter", ex.Data);
                    throw invalidClientSearchParameter;
                }

                string tenantBaseURL = ConfigurationManager.AppSettings[ModelConstant.TENANT_BASE_URL];
                IList<Tenant> tenants = null;

                if (clientSearchParameter.IsSuperAdmin == true)
                {
                    clientSearchParameter.TenantCode = "";
                }

                tenants = JsonConvert.DeserializeObject<List<Tenant>>(this.utility.ExecuteWebRequest(tenantBaseURL, "Tenant", "Get", JsonConvert.SerializeObject(clientSearchParameter), ModelConstant.TENANT_CODE_KEY, tenantCode));
                if (tenants != null && tenants.Count > 0)
                {
                    clientCount = (long)tenants?.Count();
                }
            }
            catch
            {
                throw;
            }

            return clientCount;
        }


        #endregion

        #region Private Methods

        /// <summary>
        /// This method determines whether the specified list of client objects are valid or not
        /// </summary>
        /// <param name="clients">The clients.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <param name="isUpdate">if set to <c>true</c> [is update].</param>
        private void IsValidClients(IList<Client> clients, string operation, string tenantCode, bool isUpdate = false, bool addRegisterFlag = false)
        {
            InvalidClientException invalidClientException = new InvalidClientException(tenantCode);

            #region Null and empty check

            // Check for null and empty records
            if (clients == null || clients.Count <= 0)
            {
                throw new NullReferenceException();
            }

            #endregion

            #region Operation checks

            #region Add update operation

            if (operation != ModelConstant.DELETE_OPERATION && operation != ModelConstant.CHANGE_ACTIVATION_STATUS)
            {
                // Check for duplicates in list
                int duplicateCount = clients.GroupBy(item => new { item.TenantName, item.TenantDomainName }).Where(item => item.Count() > 1).Count();
                if (duplicateCount > 0)
                {
                    throw new DuplicateClientException(tenantCode);
                }

                #region Validate entity and its child entities

                clients.ToList().ForEach(item =>
                {
                    try
                    {
                        item.IsValid(isUpdate);
                    }
                    catch (Exception ex)
                    {
                        invalidClientException.Data.Add(item.TenantName + '-' + item.TenantDomainName, ex.Data);
                    }

                    if (invalidClientException.Data.Count > 0)
                    {
                        throw invalidClientException;
                    }
                });

                #endregion

                if (!addRegisterFlag)
                {
                    #region Validate Last modified by  

                    /// Validate role last modified by user
                    string lastModifiedByUserIdentifiers = string.Join(",", clients.Where(item => !string.IsNullOrWhiteSpace(item.LastModifiedBy.ToString())).Select(item => item.LastModifiedBy.ToString()).Distinct().ToList());
                    if (!string.IsNullOrWhiteSpace(lastModifiedByUserIdentifiers))
                    {
                        UserSearchParameter userSearchParameter = new UserSearchParameter();
                        userSearchParameter.Identifier = lastModifiedByUserIdentifiers;
                        userSearchParameter.SortParameter.SortColumn = "Id";
                        if (this.userManager.GetUsers(userSearchParameter, tenantCode).Count() != lastModifiedByUserIdentifiers.Split(',').ToList().Count())
                        {
                            throw new UserNotFoundException(tenantCode);
                        }
                    }

                    #endregion
                }

            }

            #endregion

            #region Delete operation check

            if (operation == ModelConstant.DELETE_OPERATION)
            {
                // Check for duplicates in list
                int duplicateCount = clients.GroupBy(item => new { item.TenantName, item.TenantDomainName }).Where(item => item.Count() > 1).Count();
                if (duplicateCount > 0)
                {
                    throw new DuplicateClientException(tenantCode);
                }
            }

            #endregion

            #endregion
        }

        #endregion
    }
}