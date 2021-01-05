
namespace nIS
{
    #region References

    using System;
    using System.ComponentModel;

    #endregion
    public class TenantSecurityCodeFormat
    {
        #region Private memebers

        /// <summary>
        /// The country identifier
        /// </summary>
        private long identifier = 0;

        /// <summary>
        /// The tenant code
        /// </summary>
        private string tenantCode = string.Empty;

        /// <summary>
        /// The format
        /// </summary>
        private string format = string.Empty;

        /// <summary>
        /// The lastModifiedBy
        /// </summary>
        private long lastModifiedBy = 0;

        /// <summary>
        /// The lastModifiedOn
        /// </summary>
        private DateTime lastModifiedOn = DateTime.UtcNow;

        #endregion

        #region Public Members
        /// <summary>
        /// Gets or sets country identifier
        /// </summary>
        [Description("Identifier")]
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

        /// <summary>
        /// Gets or sets TenantCode
        /// </summary>
        [Description("TenantCode")]
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
        /// Gets or sets Format
        /// </summary>
        [Description("Format")]
        public string Format
        {
            get
            {
                return this.format;
            }
            set
            {
                this.format = value;
            }
        }

        /// <summary>
        /// Gets or sets Format
        /// </summary>
        [Description("Format")]
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

        /// <summary>
        /// Gets or sets Format
        /// </summary>
        [Description("Format")]
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

    }
}
