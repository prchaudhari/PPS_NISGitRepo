// <copyright file="MultiTenantUserRoleAccessManager.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2018 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------

namespace nIS
{
    #region References

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using Unity;

    #endregion

    public class MultiTenantUserRoleAccessManager
    {
        #region Private members
        /// <summary>
        /// The unity container
        /// </summary>
        private IUnityContainer unityContainer = null;

        /// <summary>
        /// The Statement repository.
        /// </summary>
        IMultiTenantUserRoleAccessRepository multiTenantUserRoleAccessRepository = null;

        /// <summary>
        /// The validation engine object
        /// </summary>
        IValidationEngine validationEngine = null;

        #endregion

        #region Constructor

        /// <summary>
        /// This is constructor for multi-tenant user role access manager, which initialise
        /// multi-tenant user role access repository.
        /// </summary>
        /// <param name="unityContainer">IUnity container implementation object.</param>
        public MultiTenantUserRoleAccessManager(IUnityContainer unityContainer)
        {
            try
            {
                this.unityContainer = unityContainer;
                this.multiTenantUserRoleAccessRepository = this.unityContainer.Resolve<IMultiTenantUserRoleAccessRepository>();
                this.validationEngine = new ValidationEngine();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// This method adds the specified list of multi-tenant user role access in multi tenant user role access repository.
        /// </summary>
        /// <param name="lstMultiTenantUserRoleAccess">The list of statements</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns true if list of multi-tenant user role access are added successfully, else false.
        /// </returns>
        public bool AddMultiTenantUserRoleAccess(IList<MultiTenantUserRoleAccess> lstMultiTenantUserRoleAccess, string tenantCode)
        {
            try
            {
                this.IsValidMultiTenantUserRoleAccessList(lstMultiTenantUserRoleAccess, tenantCode);
                return this.multiTenantUserRoleAccessRepository.AddMultiTenantUserRoleAccess(lstMultiTenantUserRoleAccess, tenantCode);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method updates the specified list of multi-tenant user role access in multi tenant user role access repository.
        /// </summary>
        /// <param name="lstMultiTenantUserRoleAccess">The list of statements</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns true if list of multi-tenant user role access are updated successfully, else false.
        /// </returns>
        public bool UpdateMultiTenantUserRoleAccess(IList<MultiTenantUserRoleAccess> lstMultiTenantUserRoleAccess, string tenantCode)
        {
            try
            {
                this.IsValidMultiTenantUserRoleAccessList(lstMultiTenantUserRoleAccess, tenantCode);
                return this.multiTenantUserRoleAccessRepository.UpdateMultiTenantUserRoleAccess(lstMultiTenantUserRoleAccess, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method gets the specified list of multi-tenant user role access from multi tenant user role access repository.
        /// </summary>
        /// <param name="multiTenantUserRoleAccessSearchParameter">The multi-tenant user role access search parameter</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of multi-tenant user role access
        /// </returns>
        public IList<MultiTenantUserRoleAccess> GetMultiTenantUserRoleAccessList(MultiTenantUserRoleAccessSearchParameter multiTenantUserRoleAccessSearchParameter, string tenantCode)
        {
            try
            {
                InvalidSearchParameterException invalidSearchParameterException = new InvalidSearchParameterException(tenantCode);
                try
                {
                    multiTenantUserRoleAccessSearchParameter.IsValid();
                }
                catch (Exception exception)
                {
                    invalidSearchParameterException.Data.Add("InvalidPagingParameter", exception.Data);
                }

                if (invalidSearchParameterException.Data.Count > 0)
                {
                    throw invalidSearchParameterException;
                }
                return this.multiTenantUserRoleAccessRepository.GetMultiTenantUserRoleAccessList(multiTenantUserRoleAccessSearchParameter, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method reference to get multi-tenant user role access list count
        /// </summary>
        /// <param name="multiTenantUserRoleAccessSearchParameter">The multi-tenant user role access search parameter</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>multi-tenant user role access list count</returns>
        public int GetMultiTenantUserRoleAcessListCount(MultiTenantUserRoleAccessSearchParameter multiTenantUserRoleAccessSearchParameter, string tenantCode)
        {
            try
            {
                return this.multiTenantUserRoleAccessRepository.GetMultiTenantUserRoleAcessListCount(multiTenantUserRoleAccessSearchParameter, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method helps to activate the multi-tenant user role access
        /// </summary>
        /// <param name="multiTenantUserRoleAccessIdentifier">The multi tenant user role access identifier</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>True if Multi-tenant user role access activated successfully false otherwise</returns>
        public bool ActivateMultiTenantUserRoleAccess(long multiTenantUserRoleAccessIdentifier, string tenantCode)
        {
            try
            {
                return this.multiTenantUserRoleAccessRepository.ActivateMultiTenantUserRoleAccess(multiTenantUserRoleAccessIdentifier, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method helps to deactivate the multi-tenant user role access
        /// </summary>
        /// <param name="multiTenantUserRoleAccessIdentifier">The multi tenant user role access identifier</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>True if Multi-tenant user role access deactivated successfully false otherwise</returns>
        public bool DeactivateMultiTenantUserRoleAccess(long multiTenantUserRoleAccessIdentifier, string tenantCode)
        {
            try
            {
                return this.multiTenantUserRoleAccessRepository.DeactivateMultiTenantUserRoleAccess(multiTenantUserRoleAccessIdentifier, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method helps to delete the multi-tenant user role access
        /// </summary>
        /// <param name="multiTenantUserRoleAccessIdentifier">The multi tenant user role access identifier</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>True if Multi-tenant user role access deleted successfully false otherwise</returns>
        public bool DeletedMultiTenantUserRoleAccess(long multiTenantUserRoleAccessIdentifier, string tenantCode)
        {
            try
            {
                return this.multiTenantUserRoleAccessRepository.DeletedMultiTenantUserRoleAccess(multiTenantUserRoleAccessIdentifier, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method gets the specified list of users by tenant code from user repository.
        /// </summary>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of users of specific tenant code
        /// </returns>
        public IList<User> GetUsersByTenantCode(string tenantCode)
        {
            try
            {
                return this.multiTenantUserRoleAccessRepository.GetUsersByTenantCode(tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method gets the specified list of roles by tenant code from role repository.
        /// </summary>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of roles of specific tenant code
        /// </returns>
        public IList<Role> GetRolesByTenantCode(string tenantCode)
        {
            try
            {
                return this.multiTenantUserRoleAccessRepository.GetRolesByTenantCode(tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method gets the specified list of mapped tenants to single user.
        /// </summary>
        /// <param name = "userId" > The User Identifier</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of tenants which are mapped to user
        /// </returns>
        public IList<UserTenant> GetUserTenants(long userId, string tenantCode)
        {
            try
            {
                return this.multiTenantUserRoleAccessRepository.GetUserTenants(userId, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method gets the list of parent as well as child tenants.
        /// </summary>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of parent as well as child tenants
        /// </returns>
        public IList<Client> GetParentAndChildTenants(string tenantCode)
        {
            try
            {
                return this.multiTenantUserRoleAccessRepository.GetParentAndChildTenants(tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// This method is responsible for validate multi-tenant user role access list.
        /// </summary>
        /// <param name="multiTenantUserRoleAccesses"></param>
        /// <param name="tenantCode"></param>
        private void IsValidMultiTenantUserRoleAccessList(IList<MultiTenantUserRoleAccess> lstMultiTenantUserRoleAccess, string tenantCode)
        {
            try
            {
                if (lstMultiTenantUserRoleAccess?.Count <= 0)
                {
                    throw new NullArgumentException(tenantCode);
                }

                InvalidMultiTenantUserRoleAccessException invalidMultiTenantUserRoleAccessException = new InvalidMultiTenantUserRoleAccessException(tenantCode);
                lstMultiTenantUserRoleAccess.ToList().ForEach(item =>
                {
                    try
                    {
                        item.IsValid();
                    }
                    catch (Exception ex)
                    {
                        invalidMultiTenantUserRoleAccessException.Data.Add(item.UserName, ex.Data);
                    }
                });

                if (invalidMultiTenantUserRoleAccessException.Data.Count > 0)
                {
                    throw invalidMultiTenantUserRoleAccessException;
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
