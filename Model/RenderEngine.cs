// <copyright file="RenderEngine.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2020 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------

namespace nIS
{
    #region References
    using System;
    using System.ComponentModel;
    #endregion

    /// <summary>
    /// This class represents render engine model
    /// </summary>
    public class RenderEngine
    {
        #region Private Members

        /// <summary>
        /// The render engine identifier
        /// </summary>
        private long identifier = 0;

        /// <summary>
        /// The render engine name
        /// </summary>
        private string renderEngineName = string.Empty;
        
        /// <summary>
        /// The render engine url
        /// </summary>
        private string url = string.Empty;

        /// <summary>
        /// The priority level
        /// </summary>
        private int priorityLevel = 0;

        /// <summary>
        /// The validation engine
        /// </summary>
        private IValidationEngine validationEngine = new ValidationEngine();

        /// <summary>
        /// The utility
        /// </summary>
        private IUtility utility = new Utility();

        /// <summary>
        /// IsActive for render engine  
        /// </summary>
        bool isActive = false;

        /// <summary>
        /// IsDeleted for render engine  
        /// </summary>
        bool isDeleted = false;

        /// <summary>
        /// NumberOfThread to each render engine  
        /// </summary>
        int numberOfThread = 1;

        /// <summary>
        /// InUse for each render engine  
        /// </summary>
        bool inUse = false;

        #endregion

        #region Public Members

        /// <summary>
        /// Gets or sets render engine identifier
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
        /// Gets or sets render engine name
        /// </summary>
        [Description("Render Engine Name")]
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
        /// Gets or sets render engine code
        /// </summary>
        [Description("Render Engine URL")]
        public string URL
        {
            get
            {
                return this.url;
            }
            set
            {
                this.url = value;
            }
        }

        /// <summary>
        /// Gets or sets the priority level.
        /// </summary>
        /// <value>
        /// The priority level.
        /// </value>
        [Description("PriorityLevel")]
        public int PriorityLevel
        {
            get
            {
                return this.priorityLevel;
            }
            set
            {
                this.priorityLevel = value;
            }
        }

        /// <summary>
        /// Gets or sets  IsActive for render engine.
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
        /// Gets or sets  IsDeleted for render engine.
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

        /// <summary>
        /// Gets or sets InUse for render engine.
        /// </summary>
        [Description("InUse")]
        public bool InUse
        {
            get
            {
                return this.inUse;
            }
            set
            {
                this.inUse = value;
            }
        }

        /// <summary>
        /// Gets or sets NumberOfThread for render engine.
        /// </summary>
        [Description("NumberOfThread")]
        public int NumberOfThread
        {
            get
            {
                return this.numberOfThread;
            }
            set
            {
                this.numberOfThread = value;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// This method validates current render engine model
        /// </summary>
        public void IsValid()
        {
            try
            {
                Exception exception = new Exception();
                if (!this.validationEngine.IsValidText(this.renderEngineName))
                {
                    exception.Data.Add(this.utility.GetDescription("RenderEngineName", typeof(RenderEngine)), ModelConstant.RENDERENGINE_MODEL_SECTION + "~" + ModelConstant.INVALID_RENDERENGINE_NAME);
                }

                if (!this.validationEngine.IsValidText(this.url))
                {
                    exception.Data.Add(this.utility.GetDescription("URL", typeof(RenderEngine)), ModelConstant.RENDERENGINE_MODEL_SECTION + "~" + ModelConstant.INVALID_RENDERENGINE_URL);
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
