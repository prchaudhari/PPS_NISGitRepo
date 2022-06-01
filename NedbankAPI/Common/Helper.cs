// <copyright file="Helper.cs" company="Websym Solutions Pvt Ltd">
// Copyright (c) 2018 Websym Solutions Pvt Ltd.
// </copyright>
// ----------------------------------------------------------------------- 

namespace NedbankAPI
{
    #region References

    using System;
    using System.Linq;
    using System.Net.Http.Headers;
    using NedBankException;
    using NedbankModel;

    #endregion

    /// <summary>
    /// Represents Helper Class
    /// </summary>
    public static class Helper
    {
        /// <summary>
        /// This method will validate Tenant Code
        /// </summary>
        /// <param name="headers"> Http Request Header</param>
        /// <returns>return tenant Code value</returns>
        public static string CheckTenantCode(HttpRequestHeaders headers)
        {
            string tenantCode = string.Empty;
            try
            {
                if (!headers.Contains(ModelConstant.TENANT_CODE_KEY))
                {
                    throw new TenantNotFoundException(ModelConstant.DEFAULT_TENANT_CODE);
                }

                tenantCode = headers.GetValues(ModelConstant.TENANT_CODE_KEY).FirstOrDefault();
                if (string.IsNullOrWhiteSpace(tenantCode))
                {
                    throw new InvalidTenantException(ModelConstant.DEFAULT_TENANT_CODE);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return tenantCode;
        }
    }
}