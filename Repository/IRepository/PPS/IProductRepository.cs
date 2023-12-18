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
    public interface IProductRepository
    {
        #region Corporate Saver Data

        IList<ProductViewModel> Get_Products(string tenantCode);

        ProductViewModel Get_ProductById(int id, string tenantCode);

        IList<ProductPageTypeMappingViewModel> Get_ProductPageTypeMappingByProductId(int productId, string tenantCode);

        #endregion
    }
}
