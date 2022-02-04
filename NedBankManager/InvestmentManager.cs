namespace NedBankManager
{
    #region References

    using NedbankModel;
    using NedbankRepository;
    using System;
    using System.Collections.Generic;
    using Unity;

    #endregion
    /// <summary>
    /// The InvestmentManager
    /// </summary>
    public class InvestmentManager
    {
        #region Private members
        /// <summary>
        /// The unity container
        /// </summary>
        private IUnityContainer unityContainer = null;

        /// <summary>
        /// The customer repository.
        /// </summary>
        IInvestmentRepository investmentRepository = null;

        #endregion

        #region Constructor

        /// <summary>
        /// This is constructor for role manager, which initialise
        /// role repository.
        /// </summary>
        /// <param name="unityContainer">The unity container.</param>
        public InvestmentManager(IUnityContainer unityContainer)
        {
            try
            {
                this.unityContainer = unityContainer;
                this.investmentRepository = this.unityContainer.Resolve<IInvestmentRepository>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Public Methods

        #region Get

        /// <summary>
        /// This method helps to get specified customer.
        /// </summary>
        /// <param name="investorId">The investor id</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns a list of customer if any found otherwise it will return enpty list.
        /// </returns>
        public IList<InvestmentPottfolio> GetInvestmentPottfolioByInvesterId(long investorId, string tenantCode)
        {
            try
            {
                IList<InvestmentPottfolio> investments = this.investmentRepository.GetInvestmentPottfolioByInvesterId(investorId, tenantCode);

                return investments;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// This method helps to get specified customer.
        /// </summary>
        /// <param name="investorId">The investor id</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns a list of customer if any found otherwise it will return enpty list.
        /// </returns>
        public IList<InvestorPerformance> GetInvestorPerformanceByInvesterId(long investorId, string tenantCode)
        {
            try
            {
                IList<InvestorPerformance> investments = this.investmentRepository.GetInvestorPerformanceByInvesterId(investorId, tenantCode);

                return investments;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
        #endregion
        #endregion
    }
}
