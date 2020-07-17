// <copyright file="RenderEngineSearchParameter.cs" company="Websym Solutions Pvt Ltd">
// Copyright (c) 2018 Websym Solutions Pvt Ltd.
// </copyright>
// -----------------------------------------------------------------------  

namespace nIS
{
    #region References

    using System;
    using System.ComponentModel;

    #endregion

    /// <summary>
    /// This class represents render engine search parameter
    /// </summary>
    public class RenderEngineSearchParameter : BaseSearchEntity
    {
        #region Private members

        /// <summary>
        /// The render engine identifier
        /// </summary>
        private string identifier = string.Empty;

        /// <summary>
        /// The render engine name
        /// </summary>
        private string renderEngineName = string.Empty;

        /// <summary>
        /// The render engine name
        /// </summary>
        private string renderEngineURL = string.Empty;

        /// <summary>
        /// The utility object
        /// </summary>
        private IUtility utility = new Utility();

        /// <summary>
        /// The validation engine objecct
        /// </summary>
        private IValidationEngine validationEngine = new ValidationEngine();

        #endregion

        #region Public members

        /// <summary>
        /// Gets or sets render engine identifier
        /// </summary>
        [Description("Identifier")]
        public string Identifier
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
        /// Gets or sets render engine name
        /// </summary>
        [Description("RenderEngine Name")]
        public string RenderEngineName
        {
            get
            {
                return this.renderEngineName;
            }
            set
            {
                this.renderEngineName = value;
            }
        }

        /// <summary>
        /// Gets or sets render engine name
        /// </summary>
        [Description("RenderEngine URL")]
        public string RenderEngineURL
        {
            get
            {
                return this.renderEngineURL;
            }
            set
            {
                this.renderEngineURL = value;
            }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Determines whether this instance of render engine search parameter is valid.
        /// </summary>
        public void IsValid()
        {
            try
            {
                Exception exception = new Exception();

                if (!this.PagingParameter.IsValid())
                {
                    exception.Data.Add(this.utility.GetDescription("Paging parameter", typeof(RenderEngine)), ModelConstant.COMMON_SECTION + "~" + ModelConstant.INVALID_PAGING_PARAMETER);
                }

                if (!this.SortParameter.IsValid())
                {
                    exception.Data.Add(this.utility.GetDescription("Sort parameter", typeof(RenderEngine)), ModelConstant.COMMON_SECTION + "~" + ModelConstant.INVALID_SORT_PARAMETER);
                }

                if (exception.Data.Count > 0)
                {
                    throw exception;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        #endregion
    }
}
