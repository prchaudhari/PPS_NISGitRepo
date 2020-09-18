// <copyright file="ITenantContactRepository.cs" company="Websym Solutions Pvt Ltd">
// Copyright (c) 2018 Websym Solutions Pvt Ltd.
// </copyright>
// -----------------------------------------------------------------------  

using System.Collections.Generic;

namespace nIS
{
    public interface ITenantContactRepository
    {
        /// <summary>
        /// This method adds the specified list of tenantContacts in tenantContact repository.
        /// </summary>
        /// <param name="tenantContacts">The list of tenantContacts</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns true if tenantContacts are added successfully, else false.
        /// </returns>
        bool AddTenantContacts(IList<TenantContact> tenantContacts, string tenantCode);

        /// <summary>
        /// This method updates the specified list of tenantContacts in tenantContact repository.
        /// </summary>
        /// <param name="tenantContacts">The list of tenantContacts</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns true if tenantContacts are updated successfully, else false.
        /// </returns>
        bool UpdateTenantContacts(IList<TenantContact> tenantContacts, string tenantCode);

        /// <summary>
        /// This method updates the specified list of tenantContacts in tenantContact repository.
        /// </summary>
        /// <param name="tenantContacts">The list of tenantContacts</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns true if tenantContacts are updated successfully, else false.
        /// </returns>
        bool UpdateActivationLinkStatus(IList<TenantContact> tenantContacts, string tenantCode);

        /// <summary>
        /// This method deletes the specified list of tenantContacts from tenantContact repository.
        /// </summary>
        /// <param name="tenantContacts">The list of tenantContacts</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns true if tenantContacts are deleted successfully, else false.
        /// </returns>
        bool DeleteTenantContacts(IList<TenantContact> tenantContacts, string tenantCode);

        /// <summary>
        /// This method gets the specified list of tenantContacts from tenantContact repository.
        /// </summary>
        /// <param name="tenantContactSearchParameter">The tenantContact search parameter</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of TenantContacts
        /// </returns>
        IList<TenantContact> GetTenantContacts(TenantContactSearchParameter tenantContactSearchParameter, string tenantCode);

        /// <summary>
        /// This method helps to activate the tenantContacts
        /// </summary>
        /// <param name="tenantContactIdentifier">The customer identifier</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>True if customer activated successfully false otherwise</returns>
        bool ActivateTenantContact(long tenantContactIdentifier, string tenantCode);

        /// <summary>
        /// This method helps to deactivate the tenantContacts
        /// </summary>
        /// <param name="tenantContactIdentifier">The customer identifier</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>True if customer deactivated successfully false otherwise</returns>
        bool DeactivateTenantContact(long tenantContactIdentifier, string tenantCode);

        /// <summary>
        /// This method reference to get tenantContact count
        /// </summary>
        /// <param name="tenantContactSearchParameter"></param>
        /// <param name="tenantCode"></param>
        /// <returns>Role count</returns>
        int GetTenantContactCount(TenantContactSearchParameter tenantContactSearchParameter, string tenantCode);

        /// <summary>
        /// This method determines uniqueness of elements in repository.
        /// </summary>
        /// <param name="roles">The roles to save.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns name="result">
        /// Returns true if all elements are not present in repository, false otherwise.
        /// </returns>
        bool IsDuplicateTenantContactEmailAndMobileNumber(IList<TenantContact> tenantContacts, string operation, string tenantCode);

    }
}
