// <copyright file="InvestmentController.cs" company="Websym Solutions Pvt Ltd">
// Copyright (c) 2018 Websym Solutions Pvt Ltd.
// </copyright>
// -----------------------------------------------------------------------  

namespace NedbankAPI.Controllers
{
    #region References

    using NedbankModel;
    using System;
    using System.Collections.Generic;
    using System.Web.Http;
    using Unity;
    using NedBankManager;

    #endregion

    /// <summary>
    /// The InvestmentController
    /// </summary>
    /// <seealso cref="System.Web.Http.ApiController" />
    [RoutePrefix("Investment")]
    public class InvestmentController : ApiController
    {
        #region Private Members

        /// <summary>
        /// The role manager object.
        /// </summary>
        private InvestmentManager investmentManager = null;

        /// <summary>
        /// The unity container
        /// </summary>
        private readonly IUnityContainer unityContainer = null;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="InvestmentController" /> class.
        /// </summary>
        /// <param name="unityContainer">The unity container.</param>
        public InvestmentController(IUnityContainer unityContainer)
        {
            this.unityContainer = unityContainer;
            this.investmentManager = new InvestmentManager(this.unityContainer);
        }

        #endregion

        #region Get

        #region List

        /// <summary>
        /// This api call use to get single or list of customer
        /// </summary>
        /// <param name="investorId">The investor identifier.</param>
        /// <returns>
        /// Returns list of customers
        /// </returns>
        [HttpPost]
        [Route("InvestmentPortfolioList")]
        public IList<InvestmentPottfolio> InvestmentPortfolioList(long investorId)
        {
            IList<InvestmentPottfolio> investmentPottfolios = new List<InvestmentPottfolio>();
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                investmentPottfolios = this.investmentManager.GetInvestmentPottfolioByInvesterId(investorId, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return investmentPottfolios;
        }

        /// <summary>
        /// This api call use to get single or list of customer
        /// </summary>
        /// <param name="investorId">The investor identifier.</param>
        /// <returns>
        /// Returns list of customers
        /// </returns>
        [HttpPost]
        [Route("InvestorPerformanceList")]
        public IList<InvestorPerformance> InvestorPerformanceList(long investorId)
        {
            IList<InvestorPerformance> investorPerformances = new List<InvestorPerformance>();
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                investorPerformances = this.investmentManager.GetInvestorPerformanceByInvesterId(investorId, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return investorPerformances;
        }

        /// <summary>
        /// This api call use to get single or list of customer
        /// </summary>
        /// <param name="investorId">The investor identifier.</param>
        /// <returns>
        /// Returns list of customers
        /// </returns>
        [HttpPost]
        [Route("BreakdownOfInvestmentAccountsList")]
        public IList<BreakdownOfInvestmentAccounts> BreakdownOfInvestmentAccountsList(long investorId)
        {
            IList<BreakdownOfInvestmentAccounts> breakdownOfInvestmentAccounts = new List<BreakdownOfInvestmentAccounts>();
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                breakdownOfInvestmentAccounts = this.investmentManager.GetBreakdownOfInvestmentAccountsByInvesterId(investorId, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return breakdownOfInvestmentAccounts;
        }

        #endregion

        #endregion
    }
}