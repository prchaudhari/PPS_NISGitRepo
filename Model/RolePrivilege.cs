// <copyright file="RolePrivilege.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2017 Websym Solutions Pvt Ltd.
// </copyright>
//------------------------------------------------------------------------------

namespace nIS
{
    #region References

    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;

    #endregion

    /// <summary>
    /// This class represents role privilege model
    /// </summary>
    public class RolePrivilege
    {
        #region Private Members

        /// <summary>
        /// The entity name
        /// </summary>
        private string entityName = string.Empty;

        /// <summary>
        ///  The role privileges operation
        /// </summary>
        private IList<RolePrivilegeOperation> rolePrivilegeOperations = new List<RolePrivilegeOperation>();

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
        /// Gets and sets entity name
        /// </summary>
        /// <value>
        /// The entity name.
        /// </value>
        [Description("Entity name")]
        public string EntityName
        {
            get
            {
                return this.entityName;
            }

            set
            {
                this.entityName = value;
            }
        }

        /// <summary>
        /// Gets and sets role privilege operation
        /// </summary>
        /// <value>
        /// The role privilege operation
        /// </value>
        [Description("RolePrivilegeOperation")]
        public IList<RolePrivilegeOperation> RolePrivilegeOperations
        {
            get
            {
                return this.rolePrivilegeOperations;
            }

            set
            {
                this.rolePrivilegeOperations = value;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// This method will validate role privilege model
        /// </summary>
        public void Isvalid()
        {
            Exception exception = new Exception();
            if (!this.validationEngine.IsValidText(this.EntityName, true))
            {
                exception.Data.Add(this.utility.GetDescription("EntityName", typeof(RolePrivilege)), ModelConstant.ROLEPRIVILEGEMODELSECTION + "~" + ModelConstant.INVALIDENTITYNAME);
            }

            if (this.RolePrivilegeOperations == null || this.RolePrivilegeOperations.Count <= 0)
            {
                exception.Data.Add(this.utility.GetDescription("RolePrivilegeOperations", typeof(RolePrivilege)), ModelConstant.ROLEPRIVILEGEMODELSECTION + "~" + ModelConstant.INVALIDROLEPRIVILEGEOPERATIONS);
            }
            else
            {
                try
                {
                    this.RolePrivilegeOperations.ToList().ForEach(item =>
                    {
                        item.Isvalid();
                    });
                }
                catch
                {
                    exception.Data.Add(this.utility.GetDescription("RolePrivilegeOperations", typeof(RolePrivilege)), ModelConstant.ROLEPRIVILEGEMODELSECTION + "~" + ModelConstant.INVALIDROLEPRIVILEGEOPERATIONS);
                }
            }

            if (exception.Data.Count > 0)
            {
                throw exception;
            }

        }

        #endregion
    }
}