// <copyright file="Helper.cs" company="Websym Solutions Pvt Ltd">
// Copyright (c) 2018 Websym Solutions Pvt Ltd.
// </copyright>
// ----------------------------------------------------------------------- 

namespace nIS
{
    #region References

    using System;
    using System.Linq;
    using System.Net.Http.Headers;

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

        /// <summary>
        /// This method use to get time zone 
        /// </summary>
        /// <param name="headers"> Http Request Header</param>
        /// <returns>return time zone value</returns>
        public static string GetTimeZone(HttpRequestHeaders headers)
        {
            string timeZone = string.Empty;

            try
            {
                if (headers.Contains(ModelConstant.TIME_ZONE_KEY))
                {
                    timeZone = headers.GetValues(ModelConstant.TIME_ZONE_KEY).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return timeZone;
        }

        /// <summary>
        /// This method use to get off set
        /// </summary>
        /// <param name="headers"> Http Request Header</param>
        /// <returns>return off set value</returns>
        public static double GetOffSet(HttpRequestHeaders headers)
        {
            double offSet = 0.0d;

            try
            {
                if (headers.Contains(ModelConstant.OFF_SET_KEY))
                {
                    double.TryParse(headers.GetValues(ModelConstant.OFF_SET_KEY).FirstOrDefault(), out offSet);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return offSet;
        }
    }
}