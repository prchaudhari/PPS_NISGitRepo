// <copyright file="IPageRepository.cs" company="Websym Solutions Pvt Ltd">
// Copyright (c) 2018 Websym Solutions Pvt Ltd.
// </copyright>
// -----------------------------------------------------------------------  

namespace nIS
{
    #region References
    using System.Collections.Generic;
    #endregion

    public interface IPageRepository
    {
        /// <summary>
        /// This method adds the specified list of pages in page repository.
        /// </summary>
        /// <param name="pages">The list of pages</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns true if pages are added successfully, else false.
        /// </returns>
        bool AddPages(IList<Page> pages, string tenantCode);

        /// <summary>
        /// This method updates the specified list of pages in page repository.
        /// </summary>
        /// <param name="pages">The list of pages</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns true if pages are updated successfully, else false.
        /// </returns>
        bool UpdatePages(IList<Page> pages, string tenantCode);

        /// <summary>
        /// This method deletes the specified list of pages from page repository.
        /// </summary>
        /// <param name="pages">The list of pages</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns true if pages are deleted successfully, else false.
        /// </returns>
        bool DeletePages(IList<Page> pages, string tenantCode);

        /// <summary>
        /// This method gets the specified list of pages from page repository.
        /// </summary>
        /// <param name="pageSearchParameter">The page search parameter</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of pages
        /// </returns>
        IList<Page> GetPages(PageSearchParameter pageSearchParameter, string tenantCode);

        /// <summary>
        /// This method reference to get page count
        /// </summary>
        /// <param name="pageSearchParameter"></param>
        /// <param name="tenantCode"></param>
        /// <returns>Page count</returns>
        int GetPageCount(PageSearchParameter pageSearchParameter, string tenantCode);

        /// <summary>
        /// This method reference to activate page
        /// </summary>
        /// <param name="pageIdentifier"></param>
        /// <param name="tenantCode"></param>
        /// <returns></returns>
        bool ActivatePage(long pageIdentifier, string tenantCode);

        /// <summary>
        /// This method reference to deactivate page
        /// </summary>
        /// <param name="pageIdentifier"></param>
        /// <param name="tenantCode"></param>
        /// <returns></returns>
        bool DeActivatePage(long pageIdentifier, string tenantCode);
    }
}
