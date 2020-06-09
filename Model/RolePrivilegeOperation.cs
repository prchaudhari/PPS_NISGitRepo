// <copyright file="RolePrivilegeoperation.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2017 Websym Solutions Pvt Ltd.
// </copyright>
//------------------------------------------------------------------------------

namespace nIS
{
    #region References

    using System;
    using System.ComponentModel;

    #endregion

    /// <summary>
    /// This class represents role privilege opeartion model
    /// </summary>
    public class RolePrivilegeOperation
    {
        #region Private Members

        /// <summary>
        /// The role privileges operation
        /// </summary>
        private string operation = string.Empty;

        /// <summary>
        /// This flag represents enable status of operation
        /// </summary>
        private bool isEnabled = false;

        /// <summary>
        /// The utility object
        /// </summary>
        private IUtility utility = new Utility();

        /// <summary>
        /// The validation engine object
        /// </summary>
        private IValidationEngine validationEngine = new ValidationEngine();

        #endregion

        #region Public Members

        /// <summary>
        /// Gets or sets the operation.
        /// </summary>
        /// <value>
        /// The operation.
        /// </value>
        [Description("Operation")]
        public string Operation
        {
            get
            {
                return this.operation;
            }

            set
            {
                this.operation = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether operation is enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if operation is enabled; otherwise, <c>false</c>.
        /// </value> 
        [Description("Is enabled")]
        public bool IsEnabled
        {
            get
            {
                return this.isEnabled;
            }

            set
            {
                this.isEnabled = value;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// This method will validate role operation  model
        /// </summary>
        public void Isvalid()
        {
            Exception exception = new Exception();
            if (!this.validationEngine.IsValidText(this.Operation, true))
            {
                exception.Data.Add(this.utility.GetDescription("Operation", typeof(RolePrivilegeOperation)), ModelConstant.ROLEPRIVILEGEOPERATIONMODELSECTION + "~" + ModelConstant.INVALIDOPERATIONNAME);
            }

            if (exception.Data.Count > 0)
            {
                throw exception;
            }
        }

        #endregion
    }
}