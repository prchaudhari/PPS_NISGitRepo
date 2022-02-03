// <copyright file="ICustomerRepository.cs" company="Websym Solutions Pvt Ltd">
// Copyright (c) 2018 Websym Solutions Pvt Ltd.
// </copyright>
// -----------------------------------------------------------------------  

using NedbankModel;
using System.Collections.Generic;

namespace NedbankRepository
{
    public interface ICustomerRepository
    {
        /// <summary>
        /// This method gets the specified list of customers.
        /// </summary>
        /// <param name="tenantCode">The tenant code</param>
        /// <param name="investorId">The investor id</param>
        /// <returns>
        /// Returns the list of Customers
        /// </returns>
        IList<CustomerInformation> GetCustomersByInvesterId(string tenantCode, int investorId);
    }
}
