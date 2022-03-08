// <copyright file="IInvestmentRepository.cs" company="Websym Solutions Pvt. Ltd.">
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
    public interface IInvestmentRepository
    {
        #region Investment Data
        /// <summary>
        /// Gets the investment pottfolio by invester identifier.
        /// </summary>
        /// <param name="investorId">The investor identifier.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns></returns>
        IList<InvestmentPottfolio> GetInvestmentPottfolioByInvesterId(long investorId, string tenantCode);

        /// <summary>
        /// Gets the breakdown of investment accounts by invester identifier.
        /// </summary>
        /// <param name="investorId">The investor identifier.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns></returns>
        IList<BreakdownOfInvestmentAccounts> GetBreakdownOfInvestmentAccountsByInvesterId(long investorId, string tenantCode);
        #endregion

        #region Investor Data
        /// <summary>
        /// Gets the investor performance by invester identifier.
        /// </summary>
        /// <param name="investorId">The investor identifier.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns></returns>
        IList<InvestorPerformance> GetInvestorPerformanceByInvesterId(long investorId, string tenantCode);
        #endregion

        IList<NB_InvestmentTransaction> GetInvestmentTransactions(long investorId, string tenantCode);
    }
}
