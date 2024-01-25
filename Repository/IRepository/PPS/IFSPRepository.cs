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
    public interface IPPSRepository
    {
        #region FSP

        List<spIAA_PaymentDetail> spIAA_PaymentDetail_fspstatement(string tenantCode);

        List<spIAA_Commission_Detail> spIAA_Commission_Detail_ppsStatement(string tenantCode);


        #endregion
    }
}
