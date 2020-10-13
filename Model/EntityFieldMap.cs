// <copyright file="EntityFieldMap.cs" company="Websym Solutions Pvt. Ltd.">
////Copyright (c) 2018 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------

namespace nIS
{
    using System;
    #region References

    using System.ComponentModel;

    #endregion

    /// <summary>
    /// This class represents EntityFieldMap model.
    /// </summary>
    public class EntityFieldMap
    {
        #region Private Members

        /// <summary>
        /// The asset  id.
        /// </summary>
        private long identifier = 0;

        /// <summary>
        /// The asset  id.
        /// </summary>
        private long entityId = 0;

        /// <summary>
        /// The asset  name.
        /// </summary>
        private string name = string.Empty;

        /// <summary>
        /// The isdeleted
        /// </summary>
        private bool isDeleted;

        /// <summary>
        /// The isdeleted
        /// </summary>
        private bool isActive;

        /// <summary>
        /// The dataType
        /// </summary>
        private string dataType;

        /// <summary>
        /// The utility object
        /// </summary>
        private IUtility utility = new Utility();

        /// <summary>
        /// The validation engine objecct
        /// </summary>
        private IValidationEngine validationEngine = new ValidationEngine();

        #endregion

        #region Public Members

        /// <summary>
        /// Gets or sets Identifier.
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
        /// Gets or sets assetLibraryidentifier.
        /// </summary>
        [Description("EntityIdentifier")]
        public long EntityIdentifier
        {
            get
            {
                return this.entityId;
            }

            set
            {
                this.entityId = value;
            }
        }

        /// <summary>
        /// Gets or sets Name.
        /// </summary>
        [Description("Name")]
        public string Name
        {
            get
            {
                return this.name;
            }

            set
            {
                this.name = value;
            }
        }

        /// <summary>
        /// Gets or sets Name.
        /// </summary>
        [Description("DataType")]
        public string DataType
        {
            get
            {
                return this.dataType;
            }

            set
            {
                this.dataType = value;
            }
        }

        /// <summary>
        /// Gets or sets isDeleted
        /// </summary>
        [Description("IsDeleted")]
        public bool IsDeleted
        {
            get
            {
                return isDeleted;
            }
            set
            {
                isDeleted = value;
            }
        }

        /// <summary>
        /// Gets or sets IsActive
        /// </summary>
        [Description("IsActive")]
        public bool IsActive
        {
            get
            {
                return isActive;
            }
            set
            {
                isActive = value;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Determines whether this instance of asset is valid.
        /// </summary>
        /// <returns>
        /// Returns true if the asset object is valid, false otherwise.
        /// </returns>
        public void IsValid()
        {
            try
            {
                Exception exception = new Exception();

                if (this.EntityIdentifier == 0)
                {
                    exception.Data.Add(this.utility.GetDescription("EntityIdentifier", typeof(EntityFieldMap)), "EntityModelSection" + "~" + "InvlidEntityIdentifier");
                }
                if (exception.Data.Count > 0)
                {
                    throw exception;
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
