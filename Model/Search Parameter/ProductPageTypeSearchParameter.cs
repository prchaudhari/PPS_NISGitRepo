// <copyright file="ProductSearchParameter.cs" company="Websym Solutions Pvt Ltd">
// Copyright (c) 2020 Websym Solutions Pvt Ltd.
// </copyright>
// -----------------------------------------------------------------------

namespace nIS
{

    #region References
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    #endregion

    /// <summary>
    /// This class represents product search parameter
    /// </summary>
    /// 
    public class ProductPageTypeSearchParameter
    {
        private ModelUtility utility = new ModelUtility();
        private IValidationEngine validationEngine = new ValidationEngine();

        public int? PageTypeId { get; set; }
        public int? ProductId { get; set; }

        /// <summary>
        /// Determines whether this instance of Product search parameter is valid.
        /// </summary>
        public void IsValid()
        {
            //No implementation because user can pass  or not pass any parameter.
        }
    }
}
