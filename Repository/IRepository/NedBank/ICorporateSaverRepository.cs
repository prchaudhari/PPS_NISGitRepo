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
    public interface ICorporateSaverRepository
    {
        #region Corporate Saver Data

        IList<DM_CorporateSaverMaster> Get_DM_CorporateSaverMaster(CustomerCorporateSaverSearchParameter searchParameter, string tenantCode);

        IList<DM_CorporateSaverTransaction> Get_DM_CorporateSaverTransaction(CustomerCorporateSaverSearchParameter searchParameter, string tenantCode);

        #endregion
    }
}
