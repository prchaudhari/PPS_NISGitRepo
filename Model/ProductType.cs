using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nIS
{
    public class ProductType
    {

        #region Private Member

        /// <summary>
        /// The Widget Identifier
        /// </summary>
        private long identifier;

        /// <summary>
        /// The widget name
        /// </summary>
        private string productType = string.Empty;

        /// <summary>
        ///Flag for deleted or not
        /// </summary>
        private bool isDeleted = false;

        /// <summary>
        ///Flag for isActive or not
        /// </summary>
        private bool isActive = true;

        /// <summary>
        /// The validation engine object
        /// </summary>
        private IValidationEngine validationEngine = new ValidationEngine();

        /// <summary>
        /// The utility object
        /// </summary>
        private IUtility utility = new Utility();

        #endregion

        #region Public Member

        /// <summary>
        /// gets or sets the product identifier.
        /// </summary>
        [Description("Identifier")]
        public long Identifier
        {
            get
            {
                return this.identifier;
            }

            set
            {
                this.identifier = value;
            }
        }

        /// <summary>
        /// Gets or sets the product type name.
        /// </summary>
        /// <value>
        /// The product type name .
        /// </value>
        [Description("ProductTypeName")]
        public string ProductTypeName
        {
            get
            {
                return this.productType;
            }

            set
            {
                this.productType = value;
            }
        }
        /// <summary>
        /// gets or sets the isActive.
        /// </summary>
        [Description("IsActive")]
        public bool IsActive
        {
            get
            {
                return this.isActive;
            }

            set
            {
                this.isActive = value;
            }
        }

        /// <summary>
        /// gets or sets the IsDeleted.
        /// </summary>
        [Description("IsDeleted")]
        public bool IsDeleted
        {
            get
            {
                return this.isDeleted;
            }

            set
            {
                this.isDeleted = value;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// This method helps to validate the model
        /// </summary>

        public void IsValid()
        {
            Exception exception = new Exception();

            IUtility utility = new Utility();
            ValidationEngine validationEngine = new ValidationEngine();

            if (!validationEngine.IsValidLong(this.Identifier, false))
            {
                exception.Data.Add(utility.GetDescription("Identifier", typeof(ProductType)), ModelConstant.PRODUCTTYPE_SECTION + "~" + ModelConstant.INVALID_PRODUCTTYPE_ID);
            }
            if (!validationEngine.IsValidText(this.ProductTypeName, false))
            {
                exception.Data.Add(utility.GetDescription("ProductTypeName", typeof(ProductType)), ModelConstant.PRODUCTTYPE_SECTION + "~" + ModelConstant.INVALID_PRODUCTTYPE_NAME);
            }
            if (exception.Data.Count > 0)
            {
                throw exception;
            }
        }

        #endregion
    }
}
