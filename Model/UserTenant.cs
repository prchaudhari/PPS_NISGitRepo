// <copyright file="MultiTenantUserRoleAccess.cs" company="Websym Solutions Pvt. Ltd.">
////Copyright (c) 2020 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------

namespace nIS
{
    #region References

    using System;
    using System.ComponentModel;

    #endregion

    /// <summary>
    /// This enum indicates the user tenant.
    /// </summary>
    /// 
    public class UserTenant
    {
        public long UserId { get; set; }
        public string UserName { get; set; }
        public string TenantCode { get; set; }
        public string TenantName { get; set; }
        public long RoleId { get; set; }
        public string TenantImage { get; set; }
        public string TenantType { get; set; }
    }
}
