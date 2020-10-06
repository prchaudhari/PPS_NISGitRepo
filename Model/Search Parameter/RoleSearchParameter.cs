// <copyright file="RoleSearchParameter.cs" company="Websym Solutions Pvt Ltd">
// Copyright (c) 2018 Websym Solutions Pvt Ltd.
// </copyright>
// -----------------------------------------------------------------------  

namespace nIS
{
    #region References
    using System;
    #endregion

    /// <summary>
    /// This class represents role search parameter
    /// </summary>
    public class RoleSearchParameter : BaseSearchEntity
    {
        #region Private Members

        /// <summary>
        /// The role identifier
        /// </summary>
        private string identifier = string.Empty;

        /// <summary>
        /// The role name
        /// </summary>
        private string name = string.Empty;

        /// <summary>
        /// The role status
        /// </summary>
        private bool? isActive = null;

        /// <summary>
        /// Is required role privileges
        /// </summary>
        private bool isRequiredRolePrivileges = false;

        /// <summary>
        ///isCallFromListPage
        /// </summary>
        private bool isCallFromListPage = false;

        /// <summary>
        /// The tenant code
        /// </summary>
        public string tenantCode = string.Empty;

        #endregion

        #region Public Members

        /// <summary>
        /// Gets or sets role identifier
        /// </summary>
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
        /// Gets or sets role name
        /// </summary>
        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }

        /// <summary>
        /// Get or sets the value of role status
        /// </summary>
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
        /// Gets or sets isRequiredRolePrivileges
        /// </summary>
        public bool IsRequiredRolePrivileges
        {
            get
            {
                return this.isRequiredRolePrivileges;
            }
            set
            {
                this.isRequiredRolePrivileges = value;
            }
        }

        /// <summary>
        /// Gets or sets IsCallFromListPage
        /// </summary>
        public bool IsCallFromListPage
        {
            get
            {
                return this.isCallFromListPage;
            }
            set
            {
                this.isCallFromListPage = value;
            }
        }

        /// <summary>
        /// Gets or sets tenant code
        /// </summary>
        public string TenantCode
        {
            get
            {
                return this.tenantCode;
            }
            set
            {
                this.name = tenantCode;
            }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Determines whether this instance of roleSearchParameter is valid.
        /// </summary>
        /// <returns>
        /// Returns true if the role object is valid, false otherwise.
        /// </returns>
        public void IsValid()
        {
            try
            {
                Exception exception = new Exception();
                if (!this.PagingParameter.IsValid())
                {
                    exception.Data.Add(ModelConstant.INVALID_PAGING_PARAMETER, ModelConstant.INVALID_PAGING_PARAMETER);
                }

                if (!this.SortParameter.IsValid())
                {
                    exception.Data.Add(ModelConstant.INVALID_SORT_PARAMETER, ModelConstant.INVALID_SORT_PARAMETER);
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
