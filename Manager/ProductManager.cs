// <copyright file="ProductManager.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2018 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------

namespace nIS
{
    using nIS.Models;
    #region References

    using System;
    using System.Collections.Generic;
    using Unity;

    #endregion

    public class ProductManager
    {
        #region Private members
        /// <summary>
        /// The unity container
        /// </summary>
        private IUnityContainer unityContainer = null;

        /// <summary>
        /// The product repository.
        /// </summary>
        IProductRepository productRepository = null;

        #endregion

        #region Constructor

        public ProductManager(IUnityContainer unityContainer)
        {
            try
            {
                this.unityContainer = unityContainer;
                this.productRepository = this.unityContainer.Resolve<IProductRepository>();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        #endregion

        #region Public Methods
        public IList<ProductViewModel> GetProducts(string tenantCode)
        {
            try
            {
                var result = this.productRepository.Get_Products(tenantCode);
                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public ProductViewModel GetProductById(int id, string tenantCode)
        {
            try
            {
                var result = this.productRepository.Get_ProductById(id, tenantCode);
                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public IList<ProductPageTypeMappingViewModel> GetProductPageTypeMappingByProductId(int productId, string tenantCode)
        {
            try
            {
                var result = this.productRepository.Get_ProductPageTypeMappingByProductId(productId, tenantCode);
                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        #endregion
    }
}
