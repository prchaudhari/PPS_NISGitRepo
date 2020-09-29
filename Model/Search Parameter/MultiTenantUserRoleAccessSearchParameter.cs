// <copyright file="MultiTenantUserRoleAccessSearchParameter.cs" company="Websym Solutions Pvt Ltd">
// Copyright (c) 2018 Websym Solutions Pvt Ltd.
// </copyright>
// ----------------------------------------------------------------------- 

namespace nIS
{
    #region References
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    #endregion

    /// <summary>
    /// This class represents multi-tenant user role access search parameter
    /// </summary>
    /// 
    public class MultiTenantUserRoleAccessSearchParameter : BaseSearchEntity
    {

        #region Public Members

        /// <summary>
        /// The Multi-Tenant user role access identifier
        /// </summary>
        public long Identifier { get; set; }

        /// <summary>
        /// The tenant name
        /// </summary>
        public string TenantName { get; set; }

        /// <summary>
        /// The user name
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// The role name
        /// </summary>
        public string RoleName { get; set; }

        /// <summary>
        /// The multi-tenant user role access mapping status
        /// </summary>
        public bool? IsActive { get; set; }

        #endregion

        #region Public methods

        /// <summary>
        /// Determines whether this instance of multiTenantUserRoleAccessSearchParameter is valid.
        /// </summary>
        /// <returns>
        /// Returns true if the page object is valid, false otherwise.
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
