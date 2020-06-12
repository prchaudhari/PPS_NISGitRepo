// <copyright file="UserLogin.cs" company="Websym Solutions Pvt. Ltd.">
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
    public class UserLogin : BaseEntity
    {
        #region Private Members

        /// <summary>
        /// User identifier
        /// </summary>
        private string userIdentifier;

        /// <summary>
        /// User password
        /// </summary>
        private string userPassword;

        /// <summary>
        /// User encrypted password
        /// </summary>
        private string userEncryptedPassword;

        /// <summary>
        /// The isactive.
        /// </summary>
        private bool isSystemGenerated = false;

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
        /// Gets or sets user password.
        /// </summary>
        /// <value>
        /// Value indicates user password.
        /// </value>
        [Description("User password")]
        public string UserPassword
        {
            get
            {
                return this.userPassword;
            }
            set
            {
                this.userPassword = value;
            }
        }

        /// <summary>
        /// Gets or sets user encrypted password.
        /// </summary>
        /// <value>
        /// Value indicates user encrypted password.
        /// </value>
        [Description("User encrypted password")]
        public string UserEncryptedPassword
        {
            get
            {
                return this.userEncryptedPassword;
            }
            set
            {
                this.userEncryptedPassword = value;
            }
        }

        /// <summary>
        /// Gets or sets is system generated
        /// </summary>
        /// <value>
        /// Value indicates is system generated
        /// </value>
        [Description("isSystemGenerated")]
        public bool IsSystemGenerated
        {
            get
            {
                return this.isSystemGenerated;
            }
            set
            {
                this.isSystemGenerated = value;
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
