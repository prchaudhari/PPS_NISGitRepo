// <copyright file="ICustomerRepository.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2018 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------

namespace NedbankRepository
{
    #region References

    using NedbankModel;
    using System.Collections.Generic;
    #endregion

    /// <summary>
    /// This interface represents reference to access accet library repository.
    /// </summary>
    public interface ICustomerRepository
    {
        #region Customer Data
        IList<CustomerInformation> GetCustomersByInvesterId(long investorId, string tenantCode);
        #endregion

    }
}

