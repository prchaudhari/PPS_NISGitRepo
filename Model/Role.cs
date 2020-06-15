// <copyright file="Role.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2018 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------

namespace nIS
{
    #region References
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    #endregion

    /// <summary>
    /// This class represent role model.
    /// </summary>
    public class Role : BaseEntity
    {
        #region Private Members
        /// <summary>
        /// The role Identifier.
        /// </summary>
        private long identifier = 0;

        /// <summary>
        /// The role name.
        /// </summary>
        private string name = string.Empty;

        /// <summary>
        /// Role privileges
        /// </summary>
        private IList<RolePrivilege> rolePrivileges = new List<RolePrivilege>();

        /// <summary>
        /// The role description.
        /// </summary>
        private string description = string.Empty;

        /// <summary>
        /// The isactive.
        /// </summary>
        private bool isActive = false;

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
        /// Gets or sets the role Identifier.
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
        /// Gets or sets the role id.
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
        /// Gets or sets the role privileges.
        /// </summary>
        [Description("Role privileges")]
        public IList<RolePrivilege> RolePrivileges
        {
            get
            {
                return this.rolePrivileges;
            }
            set
            {
                this.rolePrivileges = value;
            }
        }

        /// <summary>
        /// Gets or sets the role description.
        /// </summary>
        [Description("Description")]
        public string Description
        {
            get
            {
                return this.description;
            }
            set
            {
                this.description = value;
            }
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Determines whether this instance of skill is valid.
        /// </summary>
        /// <returns>
        /// Returns true if the skill object is valid, false otherwise.
        /// </returns>
        public void IsValid()
        {
            try
            {
                Exception exception = new Exception();
                if (!this.validationEngine.IsValidText(this.name))
                {
                    exception.Data.Add(this.utility.GetDescription("Name", typeof(Role)), ModelConstant.ROLE_MODEL_SECTION + "~" + ModelConstant.INVALID_ROLE_NAME);
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
