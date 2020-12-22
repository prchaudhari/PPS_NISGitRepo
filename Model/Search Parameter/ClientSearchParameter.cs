// <copyright file="ClientSearchParameter.cs" company="Websym Solutions Pvt Ltd">
// Copyright (c) 2018 Websym Solutions Pvt Ltd.
// </copyright>
//----------------------------------------------------------------------- 

namespace nIS
{
    #region References

    using System;
    using System.ComponentModel;

    #endregion

    /// <summary>
    /// This class represents client search parameter model
    /// </summary>
    public class ClientSearchParameter : BaseSearchEntity
    {
        #region Private Members

        /// <summary>
        /// The tenant code.
        /// </summary>
        private string tenantCode = string.Empty;

        /// <summary>
        /// The tenant name.
        /// </summary>
        private string tenantName = string.Empty;

        /// <summary>
        /// The tenant domain name
        /// </summary>
        private string tenantDomainName = string.Empty;

        /// <summary>
        /// The tenant manage type.
        /// </summary>
        private string manageType = string.Empty;

        /// <summary>
        /// The tenant manage type.
        /// </summary>
        private string tenantType = string.Empty;
        /// <summary>
        /// The tenant manage type.
        /// </summary>
        private string parentTenantCode = string.Empty;

        /// <summary>
        /// The Email Address.
        /// </summary>
        private string primaryEmailAddress = string.Empty;

        /// <summary>
        /// The contact number
        /// </summary>
        private string primaryContactNumber = string.Empty;

        /// <summary>
        /// The start date
        /// </summary>
        private DateTime? startDate = null;

        /// <summary>
        /// The end date
        /// </summary>
        private DateTime? endDate = null;

        /// <summary>
        /// The is active
        /// </summary>
        private bool? isActive = null;

        /// <summary>
        /// The is active
        /// </summary>
        private bool isLatestSubscriptionHistory = false;

        /// <summary>
        /// The IsPrimaryTenant.
        /// </summary>
        private bool? isPrimaryTenant = null;

        /// <summary>
        /// The  isSubscriptionRequired
        /// </summary>
        private bool isSubscriptionRequired = false;

        /// <summary>
        /// The  isValidateSubscription
        /// </summary>
        private bool isValidateSubscription = false;

        /// <summary>
        /// The  isCountryRequired
        /// </summary>
        private bool isCountryRequired = false;

        /// <summary>
        /// The  isStateRequired
        /// </summary>
        private bool isStateRequired = false;

        /// <summary>
        /// The  isCityRequired
        /// </summary>
        private bool isCityRequired = false;

        /// <summary>
        /// The  issueradmin
        /// </summary>
        private bool isSuperAdmin = false;

        /// <summary>
        /// The  isContactRequired
        /// </summary>
        private bool isContactRequired = false;

        /// <summary>
        /// The utility object
        /// </summary>
        private ModelUtility utility = new ModelUtility();

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
                tenantDomainName = value;
            }
        }

        /// <summary>
        /// gets or sets the manage type.
        /// </summary>
        [Description("Manage type")]
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
        /// gets or sets the manage type.
        /// </summary>
        [Description("Tenant Type")]
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
        /// gets or sets theParentTenantCode
        /// </summary>
        [Description("ParentTenantCode")]
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
        /// gets or sets email address.
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
        /// gets or sets contact number.
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
        /// gets or sets start date.
        /// </summary>
        [Description("Start date")]
        public DateTime? StartDate
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
        /// gets or sets end date.
        /// </summary>
        [Description("End Date")]
        public DateTime? EndDate
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
        /// gets or sets is active mode.
        /// </summary>
        [Description("Is active")]
        public bool? IsActive
        {
            get
            {
                return this.isActive;
            }

            set
            {
                this.isActive = value;
            }
        }

        /// <summary>
        /// gets or sets IsSubscriptionRequired mode
        /// </summary>
        [Description("Is subscription required")]
        public bool IsSubscriptionRequired
        {
            get
            {
                return this.isSubscriptionRequired;
            }

            set
            {
                this.isSubscriptionRequired = value;
            }
        }


        /// <summary>
        /// gets or sets IsSubscriptionRequired mode
        /// </summary>
        [Description("IsValidateSubscription")]
        public bool IsValidateSubscription
        {
            get
            {
                return this.isValidateSubscription;
            }

            set
            {
                this.isValidateSubscription = value;
            }
        }

        /// <summary>
        /// gets or sets isLatestSubscriptionHistory mode.
        /// </summary>
        [Description("Is latest subscription required")]
        public bool IsLatestSubscriptionHistory
        {
            get
            {
                return this.isLatestSubscriptionHistory;
            }

            set
            {
                this.isLatestSubscriptionHistory = value;
            }
        }

        /// <summary>
        /// Gets or sets IsPrimaryTenant mode.
        /// </summary>
        [Description("Is primary tenant")]
        public bool? IsPrimaryTenant
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
        /// Gets or sets is country reuired mode.
        /// </summary>
        [Description("Is Country Required")]
        public bool IsCountryRequired
        {
            get
            {
                return this.isCountryRequired;
            }

            set
            {
                this.isCountryRequired = value;
            }
        }

        /// <summary>
        /// Gets or sets isStateRequired mode.
        /// </summary>
        [Description("Is state required")]
        public bool IsStateRequired
        {
            get
            {
                return this.isStateRequired;
            }

            set
            {
                this.isStateRequired = value;
            }
        }

        /// <summary>
        /// Gets or sets is city required.
        /// </summary>
        [Description("Is city required")]
        public bool IsCityRequired
        {
            get
            {
                return this.isCityRequired;
            }

            set
            {
                this.isCityRequired = value;
            }
        }

        /// <summary>
        /// Gets or sets is super admin .
        /// </summary>
        [Description("Is super admin")]
        public bool IsSuperAdmin
        {
            get
            {
                return this.isSuperAdmin;
            }

            set
            {
                this.isSuperAdmin = value;
            }
        }

        /// <summary>
        /// Gets or sets is super admin .
        /// </summary>
        [Description("Is super admin")]
        public bool IsContactRequired
        {
            get
            {
                return this.isContactRequired;
            }

            set
            {
                this.isContactRequired = value;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns true if client search parameter object is valid.
        /// </summary>
        public void IsValid()
        {
            Exception exception = new Exception();
            if (!PagingParameter.IsValid())
            {
                exception.Data.Add(this.utility.GetDescription("PagingParameter", typeof(ClientSearchParameter)), ModelConstant.CLIENTSEARCHPARAMETERMODELSECTION + "~" + ModelConstant.INVALIDCLIENTPAGINGPARAMETER);
            }

            if (!SortParameter.IsValid())
            {
                exception.Data.Add(this.utility.GetDescription("SortParameter", typeof(ClientSearchParameter)), ModelConstant.CLIENTSEARCHPARAMETERMODELSECTION + "~" + ModelConstant.INVALIDCLIENTSORTPARAMETER);
            }

            if (exception.Data.Count > 0)
            {
                throw exception;
            }
        }

        #endregion
    }
}