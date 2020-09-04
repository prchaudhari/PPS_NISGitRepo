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
        /// <param name="pageIdentifier">The page identifier</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns true if pages are deleted successfully, else false.
        /// </returns>
        bool DeletePages(long pageIdentifier, string tenantCode);

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
        /// This method gets the specified list of pages from page repository.
        /// </summary>
        /// <param name="pageSearchParameter">The page search parameter</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of pages
        /// </returns>
        IList<Page> GetPagesForList(PageSearchParameter pageSearchParameter, string tenantCode);

        /// <summary>
        /// This method reference to get page count
        /// </summary>
        /// <param name="pageSearchParameter"></param>
        /// <param name="tenantCode"></param>
        /// <returns>Page count</returns>
        int GetPageCount(PageSearchParameter pageSearchParameter, string tenantCode);

        /// <summary>
        /// This method reference to publish page
        /// </summary>
        /// <param name="pageIdentifier"></param>
        /// <param name="tenantCode"></param>
        /// <returns></returns>
        bool PublishPage(long pageIdentifier, string tenantCode);

        /// <summary>
        /// This method reference to preview page
        /// </summary>
        /// <param name="pageIdentifier"></param>
        /// <param name="tenantCode"></param>
        /// <returns></returns>
        bool PreviewPage(long pageIdentifier, string tenantCode);

        /// <summary>
        /// This method reference to clone page
        /// </summary>
        /// <param name="pageIdentifier"></param>
        /// <param name="tenantCode"></param>
        /// <returns></returns>
        bool ClonePage(long pageIdentifier, string tenantCode);

        /// <summary>
        /// This method gets the specified list of page type from page repository.
        /// </summary>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of page type
        /// </returns>
        IList<PageType> GetPageTypes(string tenantCode);

    }
}
