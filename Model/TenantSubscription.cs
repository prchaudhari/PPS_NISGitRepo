
namespace nIS
{
    #region References

    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using Websym.Core.EntityManager;

    #endregion

    public class TenantSubscription
    {

        #region Private members
        /// <summary>
        /// The identifier
        /// </summary>
        private long identifier;

        /// <summary>
        /// The tenantCode
        /// </summary>
        private Guid tenantCode = new Guid();

        /// <summary>
        /// The subscriptionStartDate
        /// </summary>
        private DateTime subscriptionStartDate = new DateTime();

        /// <summary>
        /// The subscriptionEndtDate
        /// </summary>
        private DateTime subscriptionEndDate = new DateTime();

        /// <summary>
        /// The lastModifiedBy
        /// </summary>
        private long lastModifiedBy;

        /// <summary>
        /// The lastModifiedOn
        /// </summary>
        private DateTime lastModifiedOn = new DateTime();

        /// <summary>
        /// The validation engine object
        /// </summary>
        private IValidationEngine validationEngine = new ValidationEngine();

        /// <summary>
        /// The utility object
        /// </summary>
        private ModelUtility utility = new ModelUtility();

        #endregion

        #region Public member
        public long Identifier
        {
            get
            {
                return this.identifier;
            }
            set
            {
                this.identifier = value;
            }
        }

        [Description("TenantCode")]
        public System.Guid TenantCode
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

        [Description("SubscriptionStartDate")]
        public DateTime SubscriptionStartDate
        {
            get
            {
                return this.subscriptionStartDate;
            }
            set
            {
                this.subscriptionStartDate = value;
            }
        }

        [Description("SubscriptionEndDate")]
        public DateTime SubscriptionEndDate
        {
            get
            {
                return this.subscriptionEndDate;
            }
            set
            {
                this.subscriptionEndDate = value;
            }
        }

        [Description("LastModifiedBy")]
        public long LastModifiedBy
        {
            get
            {
                return this.lastModifiedBy;
            }
            set
            {
                this.lastModifiedBy = value;
            }
        }

        [Description("LastModifiedOn")]
        public DateTime LastModifiedOn
        {
            get
            {
                return this.lastModifiedOn;
            }
            set
            {
                this.lastModifiedOn = value;
            }
        }
        #endregion

        public void IsValid()
        {
            Exception exception = new Exception();

            if (!this.validationEngine.IsValidDate(this.subscriptionEndDate))
            {
                exception.Data.Add(this.utility.GetDescription("SubscriptionEndDate", typeof(Client)), ModelConstant.CLIENTMODELSECTION + "~" + ModelConstant.INVALIDDOMAINNAME);
            }
        }
    }
}
