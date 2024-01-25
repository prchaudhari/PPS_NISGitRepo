// <copyright file="ProductController.cs" company="Websym Solutions Pvt Ltd">
// Copyright (c) 2018 Websym Solutions Pvt Ltd.
// </copyright>
// -----------------------------------------------------------------------  

namespace nIS
{
    #region References

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Http;
    using Unity;

    #endregion

    public class ProductController : ApiController
    {
        #region Private Members

        /// <summary>
        /// The product manager object.
        /// </summary>
        private ProductManager productManager = null;

        /// <summary>
        /// The unity container
        /// </summary>
        private readonly IUnityContainer unityContainer = null;

        #endregion

        #region Constructor

        public ProductController(IUnityContainer unityContainer)
        {
            this.unityContainer = unityContainer;
            this.productManager = new ProductManager(this.unityContainer);
        }

        #endregion

        #region Public Methods
        [HttpGet]
        public IList<ProductViewModel> GetProducts()
        {
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                var result = this.productManager.GetProducts(tenantCode);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public ProductViewModel GetProductById(int id)
        {
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                var result = this.productManager.GetProductById(id, tenantCode);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public IList<ProductPageTypeMappingViewModel> GetProductPageTypeMappingByProductId(int productId)
        {
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                var result = this.productManager.GetProductPageTypeMappingByProductId(productId, tenantCode);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return null;
        }
        #endregion

    }
}
