// <copyright file="RoleManager.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2018 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------

namespace nIS
{
    #region References

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Unity;
    using Websym.Core.EntityManager;

    #endregion

    /// <summary>
    /// This class implements manager layer of role manager.
    /// </summary>
    public class RoleManager
    {
        #region Private members
        /// <summary>
        /// The unity container
        /// </summary>
        private IUnityContainer unityContainer = null;

        /// <summary>
        /// The role repository.
        /// </summary>
        IRoleRepository roleRepository = null;

        /// <summary>
        /// The utility object
        /// </summary>
        private IConfigurationUtility configurationutility = null;

        #endregion

        #region Constructor

        /// <summary>
        /// This is constructor for role manager, which initialise
        /// role repository.
        /// </summary>
        /// <param name="container">IUnity container implementation object.</param>
        public RoleManager(IUnityContainer unityContainer)
        {
            try
            {
                this.unityContainer = unityContainer;
                this.roleRepository = this.unityContainer.Resolve<IRoleRepository>();
                this.configurationutility = new ConfigurationUtility(this.unityContainer);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// This method will call add roles method of repository.
        /// </summary>
        /// <param name="roles">Roles are to be add.</param>
        /// <param name="tenantCode">Tenant code of role.</param>
        /// <returns>
        /// Returns true if entities added successfully, false otherwise.
        /// </returns>
        public bool AddRoles(IList<Role> roles, string tenantCode)
        {
            bool result = false;
            try
            {
                this.IsValidRoles(roles, tenantCode);
                this.IsDuplicateRole(roles, tenantCode);
                result = this.roleRepository.AddRoles(roles, tenantCode);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method will call update roles method of repository
        /// </summary>
        /// <param name="roles">Roles are to be update.</param>
        /// <param name="tenantCode">Tenant code of role.</param>
        /// <returns>
        /// Returns true if roles updated successfully, false otherwise.
        /// </returns>
        public bool UpdateRoles(IList<Role> roles, string tenantCode)
        {
            bool result = false;
            try
            {
                this.IsValidRoles(roles, tenantCode);
                this.IsDuplicateRole(roles, tenantCode);
                result = this.roleRepository.UpdateRoles(roles, tenantCode);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method will call delete roles method of repository
        /// </summary>
        /// <param name="roles">Roles are to be delete.</param>
        /// <param name="tenantCode">Tenant code of role.</param>
        /// <returns>
        /// Returns true if roles deleted successfully, false otherwise.
        /// </returns>
        public bool DeleteRoles(IList<Role> roles, string tenantCode)
        {
            bool result = false;
            try
            {
                result = this.roleRepository.DeleteRoles(roles, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return result;
        }

        /// <summary>
        /// This method will call get roles method of repository.
        /// </summary>
        /// <param name="roleSearchParameter">The role search parameters.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns>
        /// Returns roles if found for given parameters, else return null
        /// </returns>
        public IList<Role> GetRoles(RoleSearchParameter roleSearchParameter, string tenantCode)
        {
            IList<Role> roles = new List<Role>();
            try
            {
                InvalidSearchParameterException invalidSearchParameterException = new InvalidSearchParameterException(tenantCode);
                try
                {
                    roleSearchParameter.IsValid();
                }
                catch (Exception exception)
                {
                    invalidSearchParameterException.Data.Add("InvalidPagingParameter", exception.Data);
                }

                if (invalidSearchParameterException.Data.Count > 0)
                {
                    throw invalidSearchParameterException;
                }

                roles = this.roleRepository.GetRoles(roleSearchParameter, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return roles;
        }

        /// <summary>
        /// This method helps to get count of roles.
        /// </summary>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns>
        /// Returns count of roles
        /// </returns>
        public int GetRoleCount(RoleSearchParameter roleSearchParameter, string tenantCode)
        {
            int roleCount = 0;
            try
            {
                roleCount = this.roleRepository.GetRoleCount(roleSearchParameter, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return roleCount;
        }

        /// <summary>
        /// This method helps to activate the customers
        /// </summary>
        /// <param name="roleIdentifier">The role identifier</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>True if role activated successfully false otherwise</returns>
        public bool ActivateRole(long roleIdentifier, string tenantCode)
        {
            bool result = false;
            try
            {
                this.roleRepository.ActivateRole(roleIdentifier, tenantCode);
                result = true;
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return result;
        }

        /// <summary>
        /// This method helps to deactivate the role
        /// </summary>
        /// <param name="roleIdentifier">The role identifier</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>True if role deactivated successfully false otherwise</returns>
        public bool DeactivateRole(long roleIdentifier, string tenantCode)
        {
            bool result = false;
            try
            {
                this.roleRepository.DeActivateRole(roleIdentifier, tenantCode);
                result = true;
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return result;
        }

        /// <summary>
        /// THis method will call get method of entity manager.
        /// </summary>
        /// <param name="entitySearchParameter">The entity search parameter</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Return list of roleprivileges if exist other wise return null
        /// </returns>
        public IList<RolePrivilege> GetRolePrivileges( string tenantCode)
        {
            IList<RolePrivilege> rolePrivileges = new List<RolePrivilege>();
            IList<Entity> entities = new List<Entity>();
            string resultContent = string.Empty;
            try
            {

                //Creates a new object for entity search parameter with necessary values initialized
                EntitySearchParameter entitySearchParameter = new EntitySearchParameter();
                    entitySearchParameter.ComponentCode = ModelConstant.COMPONENTCODE;
                    entitySearchParameter.SearchMode = Websym.Core.EntityManager.SearchMode.Exact;
                    entitySearchParameter.SortParameter.SortColumn = ModelConstant.ENTITYSORTCOLUMN;
                    entitySearchParameter.SortParameter.SortOrder = Websym.Core.EntityManager.SortOrder.Ascending;
                

                entities = configurationutility.GetRolePrivileges(entitySearchParameter, tenantCode);


                if (entities != null && entities.Count > 0)
                {
                    entities.ToList().ForEach(item =>
                    {
                        IList<RolePrivilegeOperation> rolePrivilegeOperations = new List<RolePrivilegeOperation>();
                        item.Operations.ToList().ForEach(item1 =>
                        {
                            rolePrivilegeOperations.Add(new RolePrivilegeOperation() { Operation = item1.ToString(), IsEnabled = false });
                        });

                        rolePrivileges.Add(new RolePrivilege() { 
                            EntityName = item.EntityName.ToString(), 
                            RolePrivilegeOperations = rolePrivilegeOperations,
                        });
                    });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return rolePrivileges;
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// This method is responsible for validate roles.
        /// </summary>
        /// <param name="roles"></param>
        /// <param name="tenantCode"></param>
        private void IsValidRoles(IList<Role> roles, string tenantCode)
        {
            try
            {
                if (roles?.Count <= 0)
                {
                    throw new NullArgumentException(tenantCode);
                }

                InvalidRoleException invalidRoleException = new InvalidRoleException(tenantCode);
                roles.ToList().ForEach(item =>
                {
                    try
                    {
                        item.IsValid();
                    }
                    catch (Exception ex)
                    {
                        invalidRoleException.Data.Add(item.Name, ex.Data);
                    }
                });

                if (invalidRoleException.Data.Count > 0)
                {
                    throw invalidRoleException;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method helps to check duplicate role in the list
        /// </summary>
        /// <param name="roles"></param>
        /// <param name="tenantCode"></param>
        private void IsDuplicateRole(IList<Role> roles, string tenantCode)
        {
            try
            {
                int isDuplicateRole = roles.GroupBy(p => p.Name).Where(g => g.Count() > 1).Count();
                if (isDuplicateRole > 0)
                {
                    throw new DuplicateRoleFoundException(tenantCode);
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        public bool CheckUserAssociatedWithRole(long roleIdentifier, string tenantCode)
        {
            try
            {              
                return this.roleRepository.CheckUserAssociatedWithRole(roleIdentifier, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }
}
