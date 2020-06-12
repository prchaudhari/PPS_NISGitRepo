// <copyright file="UserLoginActivityHistory.cs" company="Websym Solutions Pvt. Ltd.">
////Copyright (c) 2016 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------

namespace nIS
{
    #region Reference

    using System;
    using System.ComponentModel;

    #endregion

    /// <summary>
    /// This class represents user login entity.
    /// </summary>
    public class UserLoginActivityHistory : BaseEntity
    {
        #region Private Members

        /// <summary>
        /// User identifier
        /// </summary>
        private string userIdentifier;

        /// <summary>
        /// The isactive.
        /// </summary>
        private DateTime createdAt = DateTime.MinValue;

        /// <summary>
        /// The isactive.
        /// </summary>
        private Activity activity = Activity.LogIn;

        /// <summary>
        /// User tenantCode
        /// </summary>
        private string tenantCode;

        #endregion

        #region Public Properties


        /// <summary>
        /// Gets or sets user identifier.
        /// </summary>
        /// <value>
        /// Value indicates user idenfier.
        /// </value>
        [Description("User identifier")]
        public string UserIdentifier
        {
            get
            {
                return this.userIdentifier;
            }
            set
            {
                userIdentifier = value;
            }
        }

        /// <summary>
        /// Gets or sets is created time
        /// </summary>
        /// <value>
        /// Value indicates is created time
        /// </value>
        [Description("CreatedTime")]
        public DateTime CreatedAt
        {
            get
            {
                return this.createdAt;
            }
            set
            {
                this.createdAt = value;
            }
        }

        /// <summary>
        /// Gets or sets Activity
        /// </summary>
        /// <value>
        /// Value indicates Activity
        /// </value>
        [Description("Activity")]
        public Activity Activity
        {
            get
            {
                return this.activity;
            }
            set
            {
                this.activity = value;
            }
        }


        /// <summary>
        /// Gets or sets TeanantCode
        /// </summary>
        /// <value>
        /// Value indicates TeanantCode
        /// </value>
        [Description("TeanantCode")]
        public string TeanantCode
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

        #endregion
    }
}
