// <copyright file="User.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2018 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------

namespace nIS
{

    #region References
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    #endregion

    /// <summary>
    /// This class represent user model.
    /// </summary>
    public class User
    {
        #region Private Members

        /// <summary>
        /// The user Identifier.
        /// </summary>
        private long identifier = 0;

        /// <summary>
        /// The user first name.
        /// </summary>
        private string firstName = string.Empty;

        /// <summary>
        /// The user last name.
        /// </summary>
        private string lastName = string.Empty;

        /// <summary>
        /// The user phone number.
        /// </summary>
        private string contactNumber = string.Empty;

        /// <summary>
        /// The user NoofAttempts.
        /// </summary>
        private long noofAttempts = 0;

        /// <summary>
        /// The user image.
        /// </summary>
        private string image = string.Empty;

        /// <summary>
        /// The user email.
        /// </summary>
        private string emailAddress = string.Empty;

        /// <summary>
        /// The user status
        /// </summary>
        private bool isActive = true;

        /// <summary>
        /// The user isLocked status
        /// </summary>
        private bool isLocked = true;

        /// <summary>
        /// The user isInstanceManager status
        /// </summary>
        private bool isInstanceManager = false;

        /// <summary>
        /// The user isGroupManager status
        /// </summary>
        private bool isGroupManager = false;

        /// <summary>
        /// The role
        /// </summary>
        private IList<Role> roles = new List<Role>();

        /// <summary>
        /// dateFormat
        /// </summary>
        private string dateFormat = string.Empty;

        /// <summary>
        /// The validation engine objecct
        /// </summary>
        private IValidationEngine validationEngine = new ValidationEngine();

        /// <summary>
        /// The utility object
        /// </summary>
        private IUtility utility = new Utility();

        /// <summary>
        /// The user isPasswordResetByAdmin status
        /// </summary>
        private bool isPasswordResetByAdmin = false;

        #endregion

        #region Public Members

        /// <summary>
        /// Gets or sets user Identifier.
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
        /// Gets or sets user first name.
        /// </summary>
        [Description("First Name")]
        public string FirstName
        {
            get
            {
                return this.firstName;
            }
            set
            {
                this.firstName = value;
            }
        }

        /// <summary>
        /// Gets or sets  user last Name.
        /// </summary>
        [Description("Last Name")]
        public string LastName
        {
            get
            {
                return this.lastName;
            }
            set
            {
                this.lastName = value;
            }
        }

        /// <summary>
        /// Gets or sets user phoneNo.
        /// </summary>
        [Description("Contact Number")]
        public string ContactNumber
        {
            get
            {
                return this.contactNumber;
            }
            set
            {
                this.contactNumber = value;
            }
        }

        /// <summary>
        /// Gets or sets user image.
        /// </summary>
        [Description("Image")]
        public string Image
        {
            get
            {
                return this.image;
            }
            set
            {
                this.image = value;
            }
        }

        /// <summary>
        /// Gets or sets user email.
        /// </summary>
        [Description("Email Address")]
        public string EmailAddress
        {
            get
            {
                return this.emailAddress;
            }
            set
            {
                this.emailAddress = value;
            }
        }

        /// <summary>
        /// Gets or sets isActive field.
        /// </summary>
        [Description("Is Active")]
        public bool IsActive
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
        /// Gets or sets isLocked field.
        /// </summary>
        [Description("Is Locked")]
        public bool IsLocked
        {
            get
            {
                return this.isLocked;
            }
            set
            {
                this.isLocked = value;
            }
        }

        /// <summary>
        /// Gets or sets isGroupManager field.
        /// </summary>
        [Description("IsGroupManager")]
        public bool IsGroupManager
        {
            get
            {
                return this.isGroupManager;
            }
            set
            {
                this.isGroupManager = value;
            }
        }

        /// <summary>
        /// Gets or sets isInstanceManager field.
        /// </summary>
        [Description("isInstanceManager")]
        public bool IsInstanceManager
        {
            get
            {
                return this.isInstanceManager;
            }
            set
            {
                this.isInstanceManager = value;
            }
        }


        /// <summary>
        /// Gets or sets no of attempt field.
        /// </summary>
        [Description("No of attempt")]
        public long NoofAttempts
        {
            get
            {
                return this.noofAttempts;
            }
            set
            {
                this.noofAttempts = value;
            }
        }

        /// <summary>
        /// Gets or sets role field.
        /// </summary>
        [Description("Roles")]
        public IList<Role> Roles
        {
            get
            {
                return this.roles;
            }
            set
            {
                this.roles = value;
            }
        }

        /// <summary>
        /// The tenant code
        /// </summary>
        public string TenantCode = string.Empty;

        /// <summary>
        /// The tenant type
        /// </summary>
        public string TenantType = string.Empty;

        /// <summary>
        /// The tenant type
        /// </summary>
        public long CountryId;

        public string DateFormat = string.Empty;

        /// <summary>
        /// Gets or sets isInstanceManager field.
        /// </summary>
        [Description("isPasswordResetByAdmin")]
        public bool IsPasswordResetByAdmin
        {
            get
            {
                return this.isPasswordResetByAdmin;
            }
            set
            {
                this.isPasswordResetByAdmin = value;
            }
        }


        #endregion

        #region Public Methods

        public void IsValid()
        {
            try
            {
                Exception exception = new Exception();
                if (!this.validationEngine.IsValidText(this.FirstName))
                {
                    exception.Data.Add(this.utility.GetDescription("FirstName", typeof(User)), ModelConstant.USER_MODEL_SECTION + "~" + ModelConstant.INVALID_USER_NAME);
                }

                if (!this.validationEngine.IsValidText(this.LastName))
                {
                    exception.Data.Add(this.utility.GetDescription("LastName", typeof(User)), ModelConstant.USER_MODEL_SECTION + "~" + ModelConstant.INVALID_USER_NAME);
                }

                if (!this.validationEngine.IsValidText(this.EmailAddress))
                {
                    exception.Data.Add(this.utility.GetDescription("EmailAddress", typeof(User)), ModelConstant.USER_MODEL_SECTION + "~" + ModelConstant.INVALID_USER_EMAIL);
                }

                //if (!this.validationEngine.IsValidText(this.ContactNumber))
                //{
                //    exception.Data.Add(this.utility.GetDescription("ContactNumber", typeof(User)), ModelConstant.USER_MODEL_SECTION + "~" + ModelConstant.INVALID_USER_CONTACT_NUMBER);
                //}

                if (this.Roles?.Count == 0)
                {
                    exception.Data.Add(this.utility.GetDescription("Roles", typeof(User)), ModelConstant.USER_MODEL_SECTION + "~" + ModelConstant.INVALID_USER_NAME);
                }

                if (exception.Data.Count > 0)
                {
                    throw exception;
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
        #endregion
    }
}
