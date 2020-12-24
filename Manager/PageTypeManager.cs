// <copyright file="PageTypeManager.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2018 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------

namespace nIS
{
    #region References

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Unity;

    #endregion

    /// <summary>
    /// This class implements manager layer of pageType manager.
    /// </summary>
    public class PageTypeManager
    {
        #region Private members
        /// <summary>
        /// The unity container
        /// </summary>
        private IUnityContainer unityContainer = null;

        /// <summary>
        /// The asset library repository.
        /// </summary>
        IPageTypeRepository pageTypeRepository = null;

        #endregion

        #region Constructor

        /// <summary>
        /// This is constructor for pageType manager, which initialise
        /// pageType repository.
        /// </summary>
        /// <param name="container">IUnity container implementation object.</param>
        public PageTypeManager(IUnityContainer unityContainer)
        {
            try
            {
                this.unityContainer = unityContainer;
                this.pageTypeRepository = this.unityContainer.Resolve<IPageTypeRepository>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Public Methods

        #region Add PageType

        /// <summary>
        /// This method will call add pageType method of repository.
        /// </summary>
        /// <param name="pageTypes">PageType are to be add.</param>
        /// <param name="tenantCode">Tenant code of pageType.</param>
        /// <returns>
        /// Returns true if entities added successfully, false otherwise.
        /// </returns>
        public bool AddPageTypes(IList<PageType> pageTypes, string tenantCode)
        {
            bool result = false;
            try
            {
                this.IsValidPageType(pageTypes, tenantCode);
                this.IsDuplicatePageType(pageTypes, tenantCode);
                result = this.pageTypeRepository.AddPageTypes(pageTypes, tenantCode);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Update PageType

        /// <summary>
        /// This method reference helps to update details about pageTypes.
        /// </summary>
        /// <param name="pageTypes">
        /// The list of pageTypes.
        /// </param>
        /// <param name="tenantCode">
        /// The tenant code
        /// </param>
        /// <returns>
        /// It will return true if list of scene updates scus=ccessfully otherwise false
        /// </returns>
        public bool UpdatePageTypes(IList<PageType> pageTypes, string tenantCode)
        {
            bool result = false;
            try
            {
                this.IsValidPageType(pageTypes, tenantCode);
                this.IsDuplicatePageType(pageTypes, tenantCode);
                result = this.pageTypeRepository.UpdatePageTypes(pageTypes, tenantCode);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Delete PageType

        /// <summary>
        /// This method reference helps to delete details about pageType.
        /// </summary>
        /// <param name="pageTypes">
        /// The list of pageTypes.
        /// </param>
        /// <param name="tenantCode">
        /// The tenant code
        /// </param>
        /// <returns>
        /// It will return true if successfully updated or it will throw an exception.
        /// </returns>
        public bool DeletePageTypes(IList<PageType> pageTypes, string tenantCode)
        {
            bool result = false;
            try
            {
                result = this.pageTypeRepository.DeletePageTypes(pageTypes, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return result;
        }

        #endregion

        #region Get PageTypes

        /// <summary>
        /// This method will call get pageTypes method of repository.
        /// </summary>
        /// <param name="pageTypeSearchParameter">The pageType search parameters.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns>
        /// Returns roles if found for given parameters, else return null
        /// </returns>
        public IList<PageType> GetPageTypes(PageTypeSearchParameter pageTypeSearchParameter, string tenantCode)
        {
            IList<PageType> pageTypes = new List<PageType>();
            try
            {
                InvalidSearchParameterException invalidSearchParameterException = new InvalidSearchParameterException(tenantCode);
                try
                {
                    pageTypeSearchParameter.IsValid();
                }
                catch (Exception exception)
                {
                    invalidSearchParameterException.Data.Add("InvalidPagingParameter", exception.Data);
                }

                if (invalidSearchParameterException.Data.Count > 0)
                {
                    throw invalidSearchParameterException;
                }

                pageTypes = this.pageTypeRepository.GetPageTypes(pageTypeSearchParameter, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return pageTypes;
        }

        #endregion

        #region Get PageType Count
        /// <summary>
        /// This method helps to get count of pageType.
        /// </summary>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns>
        /// Returns count of roles
        /// </returns>
        public long GetPageTypeCount(PageTypeSearchParameter pageTypeSearchParameter, string tenantCode)
        {
            long roleCount = 0;
            try
            {
                roleCount = this.pageTypeRepository.GetPageTypeCount(pageTypeSearchParameter, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return roleCount;
        }
        #endregion

        #endregion

        #region Private Methods

        /// <summary>
        /// This method is responsible for validate pageType.
        /// </summary>
        /// <param name="pageTypes"></param>
        /// <param name="tenantCode"></param>
        private void IsValidPageType(IList<PageType> pageTypes, string tenantCode)
        {
            try
            {
                if (pageTypes?.Count <= 0)
                {
                    throw new NullArgumentException(tenantCode);
                }

                InvalidPageTypeException invalidPageTypeException = new InvalidPageTypeException(tenantCode);
                pageTypes.ToList().ForEach(item =>
                {
                    try
                    {
                        item.IsValid();
                    }
                    catch (Exception ex)
                    {
                        invalidPageTypeException.Data.Add(item.PageTypeName, ex.Data);
                    }
                });

                if (invalidPageTypeException.Data.Count > 0)
                {
                    throw invalidPageTypeException;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method helps to check duplicate pageType in the list
        /// </summary>
        /// <param name="pageTypes">pageTypes</param>
        /// <param name="tenantCode">tenant code</param>
        private void IsDuplicatePageType(IList<PageType> pageTypes, string tenantCode)
        {
            try
            {
                int isDuplicatePageType = pageTypes.GroupBy(p => p.PageTypeName).Where(g => g.Count() > 1).Count();
                if (isDuplicatePageType > 0)
                {
                    throw new DuplicatePageTypeFoundException(tenantCode);
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        #endregion
    }
}