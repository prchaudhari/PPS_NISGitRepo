
namespace nIS
{
    #region References
    using System;
    using System.ComponentModel;
    #endregion

    /// <summary>
    /// This class represent base entity.
    /// </summary>
    public class BaseEntity
    {
        #region Private Members

        /// <summary>
        /// Last modified by User object.
        /// </summary>
        private long lastModifiedBy = 0;

        /// <summary>
        /// Last modified on.
        /// </summary>
        private DateTime lastModifiedOn = DateTime.UtcNow;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets page index.
        /// </summary>
        /// <value>
        /// Value is last modifier's detail.
        /// </value>
        [Description("Last modified by")]
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
        /// Gets or sets page index.
        /// </summary>
        /// <value>
        /// Value is last modifier on.
        /// </value>
        [Description("Last modified on")]
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
