﻿// <copyright file="UserSearchParameter.cs" company="Websym Solutions Pvt Ltd">
// Copyright (c) 2018 Websym Solutions Pvt Ltd.
// </copyright>
// -----------------------------------------------------------------------  

using System;
using System.ComponentModel;

namespace nIS
{
    /// <summary>
    /// This class represents the user search parameter
    /// </summary>
    public class UserSearchParameter : BaseSearchEntity
    {
        #region Private members

        /// <summary>
        /// The user identifier
        /// </summary>
        private string identifier = string.Empty;

        /// <summary>
        /// The user name
        /// </summary>
        private string firstName = string.Empty;

        /// <summary>
        /// The user name
        /// </summary>
        private string lastName = string.Empty;

        /// <summary>
        /// The role identifier
        /// </summary>
        private long roleIdentifier = 0;

        /// <summary>
        /// The user email.
        /// </summary>
        private string emailAddress = string.Empty;

        /// <summary>
        /// The skill status
        /// </summary>
        private bool? isActive = null;

        /// <summary>
        /// The utility object
        /// </summary>
        private IUtility utility = new Utility();

        #endregion

        #region Public members


        /// <summary>
        /// Get or sets the  value of user identifier
        /// </summary>
        [Description("User Identifier")]
        public string Identifier
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
        /// Get or sets the  value of user name
        /// </summary>
        [Description("User First Name")]
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
        /// Get or sets the  value of user name
        /// </summary>
        [Description("User Last Name")]
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
        /// Get or sets the value of role identifier
        /// </summary>
        [Description("Role Identifier")]
        public long RoleIdentifier
        {
            get
            {
                return this.roleIdentifier;
            }
            set
            {
                this.roleIdentifier = value;
            }
        }

        /// <summary>
        /// Get or sets the value of user status
        /// </summary>
        [Description("IsActive")]
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
        /// Get or sets the value of user email
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

        #endregion

        #region Public methods

        /// <summary>
        /// This method helps to validate user search parameter entity.
        /// </summary>
        public void IsValid()
        {
            try
            {
                Exception exception = new Exception();
                if (!this.PagingParameter.IsValid())
                {
                    exception.Data.Add(this.utility.GetDescription("Paging parameter", typeof(User)), ModelConstant.COMMON_SECTION + "~" + ModelConstant.INVALID_PAGING_PARAMETER);
                }

                if (!this.SortParameter.IsValid())
                {
                    exception.Data.Add(this.utility.GetDescription("Sort parameter", typeof(User)), ModelConstant.COMMON_SECTION + "~" + ModelConstant.INVALID_SORT_PARAMETER);
                }

                if (exception.Data.Count > 0)
                {
                    throw exception;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }
}