// <copyright file="IAnalyticsDataRepository.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2018 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------

namespace nIS
{
    #region References

    using System.Collections.Generic;

    #endregion

    /// <summary>
    /// This interface represents reference to access accet library repository.
    /// </summary>
    public interface IAnalyticsDataRepository
    {
        #region Analytics Data

        /// <summary>
        /// This method reference to get roles based on serach parameter.
        /// </summary>
        /// <param name="assetLibrarySearchParameter">
        /// The search parameter for product
        /// </param>
        /// <param name="tenantCode">
        /// The tenant code
        /// </param>
        /// <returns>
        /// It returns list of roles.
        /// </returns>
        IList<AnalyticsData> GetAnalyticsData(AnalyticsSearchParameter searchParameter, string tenantCode);

        /// <summary>
        /// This method is responsible to get analytics data count
        /// </summary>
        /// <param name="searchParameter"></param>
        /// <param name="tenantCode"></param>
        /// <returns>analytics data count</returns>
        int GetAnalyticsDataCount(AnalyticsSearchParameter searchParameter, string tenantCode);

        /// <summary>
        /// This method reference to get roles based on serach parameter.
        /// </summary>
        /// <param name="assetLibrarySearchParameter">
        /// The search parameter for product
        /// </param>
        /// <param name="tenantCode">
        /// The tenant code
        /// </param>
        /// <returns>
        /// It returns list of roles.
        /// </returns>
        bool Save(IList<AnalyticsData> settings, string tenantCode);


        #endregion

    }
}

