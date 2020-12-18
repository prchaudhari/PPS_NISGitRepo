// <copyright file="Client.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2017 Websym Solutions Pvt Ltd.
// </copyright>
//------------------------------------------------------------------------------

namespace nIS
{
    #region References

    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using Websym.Core.EntityManager;

    #endregion

    public class Client : BaseEntity
    {
        #region Private Members

        /// <summary>
        ///The tenant code
        /// </summary>
        private string tenantCode = string.Empty;

        /// <summary>
        ///The tenant code
        /// </summary>
        private string parentTenantCode = string.Empty;

        /// <summary>
        ///The tenant name
        /// </summary>
        private string tenantName = string.Empty;

        /// <summary>
        ///The tenant type
        /// </summary>
        private string tenantType = string.Empty;

        /// <summary>
        ///The client type
        /// </summary>
        private ClientType clientType = ClientType.Asset;

        /// <summary>
        ///The tenant logo
        /// </summary>
        private string tenantLogo = string.Empty;

        /// <summary>
        /// The tenant description
        /// </summary>
        private string tenantDescription = string.Empty;

        /// <summary>
        /// The tenant domain name
        /// </summary>
        private string tenantDomainName = string.Empty;

        /// <summary>
        ///The first name
        /// </summary>
        private string primaryFirstName = string.Empty;

        /// <summary>
        ///the last name
        /// </summary>
        private string primaryLastName = string.Empty;

        /// <summary>
        ///The email id
        /// </summary>
        private string primaryEmailAddress = string.Empty;

        /// <summary>
        ///The telephone number
        /// </summary>
        private string primaryContactNumber = string.Empty;

        /// <summary>
        /// The secondary contact name
        /// </summary>
        private string secondaryContactName = string.Empty;

        /// <summary>
        /// The secondary contact number
        /// </summary>
        private string secondaryContactNumber = string.Empty;

        /// <summary>
        /// The secondary email address
        /// </summary>
        private string secondaryEmailAddress = string.Empty;

        /// <summary>
        ///The pincode
        /// </summary>
        private string primaryPinCode = string.Empty;

        /// <summary>
        ///The address line 1
        /// </summary>
        private string primaryAddressLine1 = string.Empty;

        /// <summary>
        ///The address line 2
        /// </summary>
        private string primaryAddressLine2 = string.Empty;

        /// <summary>
        /// The tenant city
        /// </summary>
        private string tenantCity = string.Empty;

        /// <summary>
        /// The tenant state
        /// </summary>
        private string tenantState = string.Empty;

        /// <summary>
        /// The tenant country
        /// </summary>
        private string tenantCountry = string.Empty;

        /// <summary>
        ///The contract start date
        /// </summary>
        private DateTime startDate = DateTime.MinValue;

        /// <summary>
        ///The contract end date
        /// </summary>
        private DateTime endDate = DateTime.MinValue;

        /// <summary>
        ///The access token
        /// </summary>
        private string accessToken = string.Empty;

        /// <summary>
        /// The account is active
        /// </summary>
        private bool isThirdPartyEnabled = false;

        /// <summary>
        /// The storage account
        /// </summary>
        private string storageAccount = string.Empty;

        /// <summary>
        /// The user object
        /// </summary>
        private User user = null;

        // <summary>
        // The entities list
        // </summary>
        private IList<Entity> entities = new List<Entity>();

        /// <summary>
        /// The account is active
        /// </summary>
        private bool isActive = true;

        /// <summary>
        /// The pan number.
        /// </summary>
        private string panNumber = string.Empty;

        /// <summary>
        /// The service tax.
        /// </summary>
        private string serviceTax = string.Empty;

        /// <summary>
        /// The TenantContacts.
        /// </summary>
        IList<TenantContact> tenantContacts = new List<TenantContact>();

        /// <summary>
        /// The is primary tenant.
        /// </summary>
        private bool isPrimaryTenant = false;

        /// <summary>
        /// The is manage type
        /// </summary>
        private string manageType = string.Empty;

        /// <summary>
        /// The ticket auto assignment
        /// </summary>
        private bool? ticketAutoAssignment = false;

        /// <summary>
        /// The validation engine object
        /// </summary>
        private IValidationEngine validationEngine = new ValidationEngine();

        /// <summary>
        /// The utility object
        /// </summary>
        private ModelUtility utility = new ModelUtility();

        /// <summary>
        ///Authentication Mode
        /// </summary>
        private string authenticationmode = string.Empty;

        /// <summary>
        /// The country.
        /// </summary>
        Country country = new Country();

        /// <summary>
        /// The State.
        /// </summary>
        State state = new State();

        /// <summary>
        /// The City.
        /// </summary>
        City city = new City();

        /// <summary>
        /// The is Tenant Configured
        /// </summary>
        private bool isTenantConfigured = false;

        /// <summary>
        /// The tenant subscription
        /// </summary>
        private IList<TenantSubscription> tenantSubscriptions = new List<TenantSubscription>();

        #endregion

        #region Public Members

        /// <summary>
        /// gets or sets the tenant code.
        /// </summary>
        [Description("Tenant code")]
        public string TenantCode
        {
            get
            {
                return this.tenantCode;
            }

            set
            {
                this.tenantCode = value;
            }
        }

        /// <summary>
        /// gets or sets the tenant code.
        /// </summary>
        [Description("Parent Tenant code")]
        public string ParentTenantCode
        {
            get
            {
                return this.parentTenantCode;
            }

            set
            {
                this.parentTenantCode = value;
            }
        }

        /// <summary>
        /// gets or sets the tenant name.
        /// </summary>
        [Description("Tenant name")]
        public string TenantName
        {
            get
            {
                return this.tenantName;
            }

            set
            {
                this.tenantName = value;
            }
        }

        /// <summary>
        /// gets or sets the tenant type.
        /// </summary>
        [Description("Tenant type")]
        public string TenantType
        {
            get
            {
                return this.tenantType;
            }

            set
            {
                this.tenantType = value;
            }
        }

        /// <summary>
        /// gets or sets the client type.
        /// </summary>
        [Description("Client type")]
        public ClientType ClientType
        {
            get
            {
                return this.clientType;
            }

            set
            {
                this.clientType = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the tenant logo.
        /// </summary>
        /// <value>
        /// The name of the tenant logo.
        /// </value>
        [Description("Tenant logo")]
        public string TenantLogo
        {
            get
            {
                return this.tenantLogo;
            }

            set
            {
                this.tenantLogo = value;
            }
        }

        /// <summary>
        /// Gets or sets the tenant description.
        /// </summary>
        /// <value>
        /// The tenant description.
        /// </value>
        [Description("Tenant description")]
        public string TenantDescription
        {
            get
            {
                return tenantDescription;
            }

            set
            {
                tenantDescription = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the tenant domain.
        /// </summary>
        /// <value>
        /// The name of the tenant domain.
        /// </value>
        [Description("Tenant domain name")]
        public string TenantDomainName
        {
            get
            {
                return tenantDomainName;
            }

            set
            {
                tenantDomainName = value?.ToLower();
            }
        }

        /// <summary>
        /// gets or sets the tenant first name.
        /// </summary>
        [Description("First name")]
        public string PrimaryFirstName
        {
            get
            {
                return this.primaryFirstName;
            }

            set
            {
                this.primaryFirstName = value;
            }
        }

        /// <summary>
        /// gets or sets the tenant last name.
        /// </summary>
        [Description("Last name")]
        public string PrimaryLastName
        {
            get
            {
                return this.primaryLastName;
            }

            set
            {
                this.primaryLastName = value;
            }
        }

        /// <summary>
        /// gets or sets the tenant email.
        /// </summary>
        [Description("Email address")]
        public string PrimaryEmailAddress
        {
            get
            {
                return this.primaryEmailAddress;
            }

            set
            {
                this.primaryEmailAddress = value;
            }
        }

        /// <summary>
        /// gets or sets the tenant telephone no.
        /// </summary>
        [Description("Contact number")]
        public string PrimaryContactNumber
        {
            get
            {
                return this.primaryContactNumber;
            }

            set
            {
                this.primaryContactNumber = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the secondary contact.
        /// </summary>
        /// <value>
        /// The name of the secondary contact.
        /// </value>
        [Description("Secondary contact name")]
        public string SecondaryContactName
        {
            get
            {
                return secondaryContactName;
            }

            set
            {
                secondaryContactName = value;
            }
        }

        /// <summary>
        /// Gets or sets the secondary contact number.
        /// </summary>
        /// <value>
        /// The secondary contact number.
        /// </value>
        [Description("Secondary contact number")]
        public string SecondaryContactNumber
        {
            get
            {
                return secondaryContactNumber;
            }

            set
            {
                secondaryContactNumber = value;
            }
        }

        /// <summary>
        /// Gets or sets the secondary email address.
        /// </summary>
        /// <value>
        /// The secondary email address.
        /// </value>
        [Description("Secondary email address")]
        public string SecondaryEmailAddress
        {
            get
            {
                return secondaryEmailAddress;
            }

            set
            {
                secondaryEmailAddress = value;
            }
        }

        /// <summary>
        /// gets or sets the tenant pincode.
        /// </summary>
        [Description("Pin code")]
        public string PrimaryPinCode
        {
            get
            {
                return this.primaryPinCode;
            }

            set
            {
                this.primaryPinCode = value;
            }
        }

        /// <summary>
        /// gets or sets the tenant address.
        /// </summary>
        [Description("Address line 1")]
        public string PrimaryAddressLine1
        {
            get
            {
                return this.primaryAddressLine1;
            }

            set
            {
                this.primaryAddressLine1 = value;
            }
        }

        /// <summary>
        /// gets or sets the primary address line 2.
        /// </summary>
        [Description("Address line 2")]
        public string PrimaryAddressLine2
        {
            get
            {
                return this.primaryAddressLine2;
            }

            set
            {
                this.primaryAddressLine2 = value;
            }
        }

        /// <summary>
        /// Gets or sets the tenant city.
        /// </summary>
        /// <value>
        /// The tenant city.
        /// </value>
        [Description("Tenant city")]
        public string TenantCity
        {
            get
            {
                return tenantCity;
            }

            set
            {
                tenantCity = value;
            }
        }

        /// <summary>
        /// Gets or sets the state of the tenant.
        /// </summary>
        /// <value>
        /// The state of the tenant.
        /// </value>
        [Description("Tenant state")]
        public string TenantState
        {
            get
            {
                return tenantState;
            }

            set
            {
                tenantState = value;
            }
        }

        /// <summary>
        /// Gets or sets the tenant country.
        /// </summary>
        /// <value>
        /// The tenant country.
        /// </value>
        [Description("Tenant country")]
        public string TenantCountry
        {
            get
            {
                return tenantCountry;
            }

            set
            {
                tenantCountry = value;
            }
        }

        /// <summary>
        /// gets or sets the tenant access token.
        /// </summary>
        [Description("Access token")]
        public string AccessToken
        {
            get
            {
                return this.accessToken;
            }

            set
            {
                this.accessToken = value;
            }
        }

        /// <summary>
        /// gets or sets the tenant start date.
        /// </summary>
        [Description("Start date")]
        public DateTime StartDate
        {
            get
            {
                return this.startDate;
            }

            set
            {
                this.startDate = value;
            }
        }

        /// <summary>
        /// gets or sets the tenant end date.
        /// </summary>
        [Description("End date")]
        public DateTime EndDate
        {
            get
            {
                return this.endDate;
            }

            set
            {
                this.endDate = value;
            }
        }

        /// <summary>
        /// gets or sets the storage account.
        /// </summary>
        [Description("Storage account")]
        public string StorageAccount
        {
            get
            {
                return this.storageAccount;
            }

            set
            {
                this.storageAccount = value;
            }
        }

        /// <summary>
        /// Gets or sets the user object.
        /// </summary>
        /// <value>
        /// The user object.
        /// </value>
        [Description("User")]
        public User User
        {
            get
            {
                return user;
            }

            set
            {
                user = value;
            }
        }

        // Gets or sets the entities.
        // </summary>
        // <value>
        // The entities.
        // </value>
        // <summary>
        [Description("Entities")]
        public IList<Entity> Entities
        {
            get
            {
                return this.entities;
            }

            set
            {
                this.entities = value;
            }
        }

        /// <summary>
        /// gets or sets the is active account.
        /// </summary>
        [Description("Is active")]
        public bool IsActive
        {
            get
            {
                return isActive;
            }

            set
            {
                isActive = value;
            }
        }

        /// <summary>
        /// gets or sets the is isThirdPartyEnabled account.
        /// </summary>
        [Description("Is isThirdPartyEnabled")]
        public bool IsThirdPartyEnabled
        {
            get
            {
                return isThirdPartyEnabled;
            }

            set
            {
                isThirdPartyEnabled = value;
            }
        }

        /// <summary>
        /// Gets or sets the pan number.
        /// </summary>
        [Description("Pan Number")]
        public string PanNumber
        {
            get
            {
                return this.panNumber;
            }

            set
            {
                this.panNumber = value;
            }
        }

        /// <summary>
        /// Gets or sets the Service Tax.
        /// </summary>
        [Description("Service Tax")]
        public string ServiceTax
        {
            get
            {
                return this.serviceTax;
            }

            set
            {
                this.serviceTax = value;
            }
        }

        /// <summary>
        /// Gets or sets the TenantContacts.
        /// </summary>
        [Description("TenantContacts")]
        public IList<TenantContact> TenantContacts
        {
            get
            {
                return this.tenantContacts;
            }

            set
            {
                this.tenantContacts = value;
            }
        }

        /// <summary>
        /// Gets or sets is primary tenant.
        /// </summary>
        [Description("Is Primary Tenant")]
        public bool IsPrimaryTenant
        {
            get
            {
                return this.isPrimaryTenant;
            }

            set
            {
                this.isPrimaryTenant = value;
            }
        }

        /// <summary>
        /// Gets or sets manage type
        /// </summary>
        [Description("Manage Type")]
        public string ManageType
        {
            get
            {
                return this.manageType;
            }

            set
            {
                this.manageType = value;
            }
        }

        /// <summary>
        /// Gets or sets ticket auto assignment
        /// </summary>
        public bool? TicketAutoAssignment
        {
            get
            {
                return this.ticketAutoAssignment;
            }
            set
            {
                this.ticketAutoAssignment = value;
            }
        }

        [Description("Authentication Mode")]
        public string AuthenticationMode
        {
            get
            {
                return this.authenticationmode;
            }

            set
            {
                this.authenticationmode = value;
            }
        }

        [Description("Country")]
        public Country Country
        {
            get
            {
                return this.country;
            }

            set
            {
                this.country = value;
            }
        }

        [Description("State")]
        public State State
        {
            get
            {
                return this.state;
            }

            set
            {
                this.state = value;
            }
        }

        [Description("City")]
        public City City
        {
            get
            {
                return this.city;
            }

            set
            {
                this.city = value;
            }
        }

        /// <summary>
        /// Gets or sets is tenant configured.
        /// </summary>
        [Description("Is Tenant Configured")]
        public bool IsTenantConfigured
        {
            get
            {
                return this.isTenantConfigured;
            }

            set
            {
                this.isTenantConfigured = value;
            }
        }

        /// <summary>
        /// Gets or sets is TenantSubscriptions.
        /// </summary>
        [Description("TenantSubscriptions")]
        public IList<TenantSubscription> TenantSubscriptions
        {
            get
            {
                return this.tenantSubscriptions;
            }
            set
            {
                this.tenantSubscriptions = value;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns true if client object is valid.
        /// </summary>
        /// <param name="isUpdate">if set to <c>true</c> [is update].</param>
        public void IsValid(bool isUpdate = false)
        {
            Exception exception = new Exception();

            if (!this.validationEngine.IsValidText(this.TenantName, true))
            {
                exception.Data.Add(this.utility.GetDescription("TenantName", typeof(Client)), ModelConstant.CLIENTMODELSECTION + "~" + ModelConstant.INVALIDTENANTNAME);
            }
            if (this.tenantType != "Group")
            {
                if (!this.validationEngine.IsValidText(this.TenantDomainName, true))
                {
                    exception.Data.Add(this.utility.GetDescription("TenantDomainName", typeof(Client)), ModelConstant.CLIENTMODELSECTION + "~" + ModelConstant.INVALIDDOMAINNAME);
                }

                if (!this.validationEngine.IsValidText(this.PrimaryAddressLine1))
                {
                    exception.Data.Add(this.utility.GetDescription("PrimaryAddressLine1", typeof(Client)), ModelConstant.CLIENTMODELSECTION + "~" + ModelConstant.INVALIDPRIMARYADDRESSLINE1);
                }

                if (!this.validationEngine.IsValidText(this.PrimaryPinCode, true))
                {
                    exception.Data.Add(this.utility.GetDescription("PrimaryPinCode", typeof(Client)), ModelConstant.CLIENTMODELSECTION + "~" + ModelConstant.INVALIDPRIMARYPINCODE);
                }
                if (this.tenantSubscriptions.Count > 0)
                { 
                    exception.Data.Add(this.utility.GetDescription("TenantSubscriptions", typeof(Client)), ModelConstant.CLIENTMODELSECTION + "~" + ModelConstant.INVALIDTENANTSUBSCRIPTION);

                }

            }

            if (this.validationEngine.IsValidText(this.TenantDescription))
            {
                if (!this.validationEngine.IsValidCharacters(this.TenantDescription))
                {
                    exception.Data.Add(this.utility.GetDescription("TenantDescription", typeof(Client)), ModelConstant.CLIENTMODELSECTION + "~" + ModelConstant.INVALIDCLIENTDESCRIPTION);
                }
            }

            if (exception.Data.Count > 0)
            {
                throw exception;
            }
        }

        #endregion
    }
}
