// <copyright file="SQLMultiTenantUserRoleAccessRepository.cs" company="Websym Solutions Pvt. Ltd.">
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
    using System.Drawing.Imaging;
    using System.IO;
    using System.Linq;
    using System.Linq.Dynamic;
    using System.Net;
    using System.Security.Claims;
    using System.Text;
    using System.Text.RegularExpressions;
    using Unity;
    #endregion

    public class SQLMultiTenantUserRoleAccessRepository : IMultiTenantUserRoleAccessRepository
    {

        #region Private Members

        /// <summary>
        /// The validation engine object
        /// </summary>
        IValidationEngine validationEngine = null;

        /// <summary>
        /// The connection string
        /// </summary>
        private string connectionString = string.Empty;

        /// <summary>
        /// The utility object
        /// </summary>
        private IUtility utility = null;

        /// <summary>
        /// The unity container
        /// </summary>
        private IUnityContainer unityContainer = null;

        /// <summary>
        /// The utility object
        /// </summary>
        private IConfigurationUtility configurationutility = null;

        #endregion

        #region Constructor

        public SQLMultiTenantUserRoleAccessRepository(IUnityContainer unityContainer)
        {
            this.unityContainer = unityContainer;
            this.validationEngine = new ValidationEngine();
            this.utility = new Utility();
            this.configurationutility = new ConfigurationUtility(this.unityContainer);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// This method adds the specified list of multi-tenant user role access in multi tenant user role access repository.
        /// </summary>
        /// <param name="multiTenantUserRoles">The list of statements</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns true if list of multi-tenant user role access are added successfully, else false.
        /// </returns>
        public bool AddMultiTenantUserRoleAccess(IList<MultiTenantUserRoleAccess> lstMultiTenantUserRoleAccess, string tenantCode)
        {
            bool result = false;
            try
            {
                var claims = ClaimsPrincipal.Current.Identities.First().Claims.ToList();
                int loginUserId;
                int.TryParse(claims?.FirstOrDefault(x => x.Type.Equals("UserId", StringComparison.OrdinalIgnoreCase)).Value, out loginUserId);

                this.SetAndValidateConnectionString(tenantCode);
                if (this.IsDuplicateTenantUserRoleMapping(lstMultiTenantUserRoleAccess, "AddOperation", tenantCode))
                {
                    throw new DuplicateTenantUserRoleAccessMappingFoundException(tenantCode);
                }

                IList<MultiTenantUserAccessMapRecord> multiTenantUserAccessMapRecords = new List<MultiTenantUserAccessMapRecord>();
                lstMultiTenantUserRoleAccess.ToList().ForEach(record =>
                {
                    multiTenantUserAccessMapRecords.Add(new MultiTenantUserAccessMapRecord()
                    {
                        UserId = record.UserId,
                        AssociatedTenantCode = record.AssociatedTenantCode,
                        OtherTenantCode = record.OtherTenantCode,
                        OtherTenantAccessRoleId = record.RoleId,
                        IsActive = true,
                        IsDeleted = false,
                        LastUpdatedBy = loginUserId,
                        LastUpdatedDate = DateTime.Now
                    });
                });

                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    nISEntitiesDataContext.MultiTenantUserAccessMapRecords.AddRange(multiTenantUserAccessMapRecords);
                    nISEntitiesDataContext.SaveChanges();
                }
                result = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        /// <summary>
        /// This method updates the specified list of multi-tenant user role access in multi tenant user role access repository.
        /// </summary>
        /// <param name="multiTenantUserRoles">The list of statements</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns true if list of multi-tenant user role access are updated successfully, else false.
        /// </returns>
        public bool UpdateMultiTenantUserRoleAccess(IList<MultiTenantUserRoleAccess> lstMultiTenantUserRoleAccess, string tenantCode)
        {
            bool result = false;
            try
            {
                var claims = ClaimsPrincipal.Current.Identities.First().Claims.ToList();
                int loginUserId;
                int.TryParse(claims?.FirstOrDefault(x => x.Type.Equals("UserId", StringComparison.OrdinalIgnoreCase)).Value, out loginUserId);

                this.SetAndValidateConnectionString(tenantCode);
                if (this.IsDuplicateTenantUserRoleMapping(lstMultiTenantUserRoleAccess, "UpdateOperation", tenantCode))
                {
                    throw new DuplicateTenantUserRoleAccessMappingFoundException(tenantCode);
                }

                IList<MultiTenantUserAccessMapRecord> multiTenantUserAccessMapRecords = new List<MultiTenantUserAccessMapRecord>();
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    StringBuilder query = new StringBuilder();
                    query.Append("(" + string.Join("or ", string.Join(",", lstMultiTenantUserRoleAccess.Select(item => item.Identifier).Distinct()).ToString().Split(',').Select(item => string.Format("Id.Equals({0}) ", item))) + ") ");

                    multiTenantUserAccessMapRecords = nISEntitiesDataContext.MultiTenantUserAccessMapRecords.Where(query.ToString()).Select(item => item).AsQueryable().ToList();
                    if (multiTenantUserAccessMapRecords == null || multiTenantUserAccessMapRecords.Count <= 0 || multiTenantUserAccessMapRecords.Count() != string.Join(",", multiTenantUserAccessMapRecords.Select(item => item.Id).Distinct()).ToString().Split(',').Length)
                    {
                        throw new MultiTenantUserRoleAccessNotFoundException(tenantCode);
                    }

                    lstMultiTenantUserRoleAccess.ToList().ForEach(record =>
                    {
                        MultiTenantUserAccessMapRecord multiTenantUserAccessMapRecord = multiTenantUserAccessMapRecords.FirstOrDefault(data => data.Id == record.Identifier);
                        multiTenantUserAccessMapRecord.OtherTenantAccessRoleId = record.RoleId;
                        multiTenantUserAccessMapRecord.OtherTenantCode = record.OtherTenantCode;
                        multiTenantUserAccessMapRecord.LastUpdatedBy = loginUserId;
                        multiTenantUserAccessMapRecord.LastUpdatedDate = DateTime.Now;
                    });

                    nISEntitiesDataContext.SaveChanges();
                    result = true;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
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
            IList<MultiTenantUserRoleAccess> lstMultiTenantUserRoleAccess = new List<MultiTenantUserRoleAccess>();
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                string whereClause = this.WhereClauseGenerator(multiTenantUserRoleAccessSearchParameter, tenantCode);
                IList<View_MultiTenantUserAccessMapRecord> view_MultiTenantUserAccessMapRecords = new List<View_MultiTenantUserAccessMapRecord>();
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    if (multiTenantUserRoleAccessSearchParameter.PagingParameter.PageIndex > 0 && multiTenantUserRoleAccessSearchParameter.PagingParameter.PageSize > 0)
                    {
                        view_MultiTenantUserAccessMapRecords = nISEntitiesDataContext.View_MultiTenantUserAccessMapRecord
                        .OrderBy(multiTenantUserRoleAccessSearchParameter.SortParameter.SortColumn + " " + multiTenantUserRoleAccessSearchParameter.SortParameter.SortOrder.ToString())
                        .Where(whereClause)
                        .Skip((multiTenantUserRoleAccessSearchParameter.PagingParameter.PageIndex - 1) * multiTenantUserRoleAccessSearchParameter.PagingParameter.PageSize)
                        .Take(multiTenantUserRoleAccessSearchParameter.PagingParameter.PageSize)
                        .ToList();
                    }
                    else
                    {
                        view_MultiTenantUserAccessMapRecords = nISEntitiesDataContext.View_MultiTenantUserAccessMapRecord                        
                        .Where(whereClause)
                        .OrderBy(multiTenantUserRoleAccessSearchParameter.SortParameter.SortColumn + " " + multiTenantUserRoleAccessSearchParameter.SortParameter.SortOrder.ToString())
                        .ToList();
                    }

                    if (view_MultiTenantUserAccessMapRecords != null && view_MultiTenantUserAccessMapRecords.Count > 0)
                    {
                        view_MultiTenantUserAccessMapRecords?.ToList().ForEach(record =>
                        {
                            lstMultiTenantUserRoleAccess.Add(new MultiTenantUserRoleAccess
                            {
                                Identifier = record.Id,
                                UserId = record.UserId,
                                UserName = record.UserName,
                                EmailAddress = record.EmailAddress,
                                AssociatedTenantCode = record.AssociatedTenantCode,
                                AssociatedTenantName = record.AssociatedTenantName,
                                AssociatedTenantType = record.AssociatedTenantType,
                                OtherTenantCode = record.OtherTenantCode,
                                OtherTenantName = record.OtherTenantName,
                                OtherTenantType = record.OtherTenantType,
                                RoleId = record.RoleId,
                                RoleName = record.RoleName,
                                IsActive = record.IsActive,
                                IsDeleted = record.IsDeleted,
                                LastUpdatedBy = record.LastUpdatedBy,
                                LastUpdatedByUserName = record.LastUpdatedByUserName,
                                LastUpdatedDate = record.LastUpdatedDate
                            });
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lstMultiTenantUserRoleAccess;
        }

        /// <summary>
        /// This method reference to get multi-tenant user role access list count
        /// </summary>
        /// <param name="multiTenantUserRoleAccessSearchParameter">The multi-tenant user role access search parameter</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>multi-tenant user role access list count</returns>
        public int GetMultiTenantUserRoleAcessListCount(MultiTenantUserRoleAccessSearchParameter multiTenantUserRoleAccessSearchParameter, string tenantCode)
        {
            string whereClause = this.WhereClauseGenerator(multiTenantUserRoleAccessSearchParameter, tenantCode);
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    return nISEntitiesDataContext.View_MultiTenantUserAccessMapRecord.Where(whereClause.ToString()).Count();
                }
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
            bool result = false;
            try
            {
                var claims = ClaimsPrincipal.Current.Identities.First().Claims.ToList();
                int loginUserId;
                int.TryParse(claims?.FirstOrDefault(x => x.Type.Equals("UserId", StringComparison.OrdinalIgnoreCase)).Value, out loginUserId);

                this.SetAndValidateConnectionString(tenantCode);

                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    var records = nISEntitiesDataContext.MultiTenantUserAccessMapRecords.Where(itm => itm.Id == multiTenantUserRoleAccessIdentifier).ToList();
                    if (records == null || records.Count <= 0)
                    {
                        throw new MultiTenantUserRoleAccessNotFoundException(tenantCode);
                    }

                    records.ToList().ForEach(item =>
                    {
                        item.IsActive = true;
                        item.LastUpdatedBy = loginUserId;
                        item.LastUpdatedDate = DateTime.Now;
                    });

                    nISEntitiesDataContext.SaveChanges();
                }
                result = true;
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return result;
        }

        /// <summary>
        /// This method helps to deactivate the multi-tenant user role access
        /// </summary>
        /// <param name="multiTenantUserRoleAccessIdentifier">The multi tenant user role access identifier</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>True if Multi-tenant user role access deactivated successfully false otherwise</returns>
        public bool DeactivateMultiTenantUserRoleAccess(long multiTenantUserRoleAccessIdentifier, string tenantCode)
        {
            bool result = false;
            try
            {
                var claims = ClaimsPrincipal.Current.Identities.First().Claims.ToList();
                int loginUserId;
                int.TryParse(claims?.FirstOrDefault(x => x.Type.Equals("UserId", StringComparison.OrdinalIgnoreCase)).Value, out loginUserId);

                this.SetAndValidateConnectionString(tenantCode);

                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    var records = nISEntitiesDataContext.MultiTenantUserAccessMapRecords.Where(itm => itm.Id == multiTenantUserRoleAccessIdentifier).ToList();
                    if (records == null || records.Count <= 0)
                    {
                        throw new MultiTenantUserRoleAccessNotFoundException(tenantCode);
                    }

                    records.ToList().ForEach(item =>
                    {
                        item.IsActive = false;
                        item.LastUpdatedBy = loginUserId;
                        item.LastUpdatedDate = DateTime.Now;
                    });

                    nISEntitiesDataContext.SaveChanges();
                }
                result = true;
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return result;
        }

        /// <summary>
        /// This method helps to delete the multi-tenant user role access
        /// </summary>
        /// <param name="multiTenantUserRoleAccessIdentifier">The multi tenant user role access identifier</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>True if Multi-tenant user role access deleted successfully false otherwise</returns>
        public bool DeletedMultiTenantUserRoleAccess(long multiTenantUserRoleAccessIdentifier, string tenantCode)
        {
            bool result = false;
            try
            {
                var claims = ClaimsPrincipal.Current.Identities.First().Claims.ToList();
                int loginUserId;
                int.TryParse(claims?.FirstOrDefault(x => x.Type.Equals("UserId", StringComparison.OrdinalIgnoreCase)).Value, out loginUserId);

                this.SetAndValidateConnectionString(tenantCode);

                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    var records = nISEntitiesDataContext.MultiTenantUserAccessMapRecords.Where(itm => itm.Id == multiTenantUserRoleAccessIdentifier).ToList();
                    if (records == null || records.Count <= 0)
                    {
                        throw new MultiTenantUserRoleAccessNotFoundException(tenantCode);
                    }

                    records.ToList().ForEach(item =>
                    {
                        item.IsDeleted = true;
                        item.LastUpdatedBy = loginUserId;
                        item.LastUpdatedDate = DateTime.Now;
                    });

                    nISEntitiesDataContext.SaveChanges();
                }
                result = true;
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return result;
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
            IList<User> users = new List<User>();
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    var userrecords = nISEntitiesDataContext.UserRecords.Where(x => x.TenantCode == tenantCode && x.IsActive && !x.IsDeleted).ToList();
                    userrecords.ForEach(usr =>
                    {
                        users.Add(new User()
                        {
                            Identifier = usr.Id,
                            FirstName = usr.FirstName,
                            LastName = usr.LastName,
                            EmailAddress = usr.EmailAddress,
                            TenantCode = usr.TenantCode,
                        });
                    });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return users;
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
            IList<Role> roles = new List<Role>();
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    var rolerecords = nISEntitiesDataContext.RoleRecords.Where(x => x.TenantCode == tenantCode && !x.IsDeleted).ToList();
                    rolerecords.ForEach(r =>
                    {
                        roles.Add(new Role()
                        {
                            Identifier = r.Id,
                            Name = r.Name,
                        });
                    });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return roles;
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
            IList<UserTenant> userTenants = new List<UserTenant>();
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    var usrTenants = nISEntitiesDataContext.FnUserTenant(Convert.ToInt32(userId));
                    usrTenants.ToList().ForEach(record =>
                    {
                        userTenants.Add(new UserTenant()
                        {
                            UserId = record.UserId,
                            UserName = record.UserName,
                            TenantCode = record.TenantCode,
                            TenantName = record.TenantName,
                            RoleId = record.RoleId
                        });
                    });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return userTenants;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Generate string for dynamic linq.
        /// </summary>
        /// <param name="searchParameter">Role search Parameters</param>
        /// <returns>
        /// Returns a string.
        /// </returns>
        private string WhereClauseGenerator(MultiTenantUserRoleAccessSearchParameter searchParameter, string tenantCode)
        {
            StringBuilder queryString = new StringBuilder();

            if (searchParameter.SearchMode == SearchMode.Equals)
            {
                if (validationEngine.IsValidLong(searchParameter.Identifier))
                {
                    queryString.Append("(" + string.Join("or ", searchParameter.Identifier.ToString().Split(',').Select(item => string.Format("Id.Equals({0}) ", item))) + ") and ");
                }
                if (validationEngine.IsValidLong(searchParameter.UserId))
                {
                    queryString.Append("(" + string.Join("or ", searchParameter.UserId.ToString().Split(',').Select(item => string.Format("UserId.Equals({0}) ", item))) + ") and ");
                }
                if (validationEngine.IsValidLong(searchParameter.RoleId))
                {
                    queryString.Append("(" + string.Join("or ", searchParameter.RoleId.ToString().Split(',').Select(item => string.Format("RoleId.Equals({0}) ", item))) + ") and ");
                }
                if (validationEngine.IsValidText(searchParameter.AssociatedTenantCode))
                {
                    queryString.Append("(" + string.Join("or ", searchParameter.AssociatedTenantCode.ToString().Split(',').Select(item => string.Format("AssociatedTenantCode.Equals(\"{0}\") ", item))) + ") and ");
                }
                if (validationEngine.IsValidText(searchParameter.OtherTenantCode))
                {
                    queryString.Append("(" + string.Join("or ", searchParameter.OtherTenantCode.ToString().Split(',').Select(item => string.Format("OtherTenantCode.Equals(\"{0}\") ", item))) + ") and ");
                }
            }
            if (searchParameter.SearchMode == SearchMode.Contains)
            {
                if (validationEngine.IsValidText(searchParameter.UserName))
                {
                    queryString.Append(string.Format("UserName.Contains(\"{0}\") and ", searchParameter.UserName));
                }
                if (validationEngine.IsValidText(searchParameter.AssociatedTenantName))
                {
                    queryString.Append(string.Format("AssociatedTenantName.Contains(\"{0}\") and ", searchParameter.AssociatedTenantName));
                }
                if (validationEngine.IsValidText(searchParameter.OtherTenantName))
                {
                    queryString.Append(string.Format("OtherTenantName.Contains(\"{0}\") and ", searchParameter.OtherTenantName));
                }
                if (validationEngine.IsValidText(searchParameter.RoleName))
                {
                    queryString.Append(string.Format("RoleName.Contains(\"{0}\") and ", searchParameter.RoleName));
                }
            }
            
            if (searchParameter.IsActive != null)
            {
                queryString.Append(string.Format("IsActive.Equals({0}) and ", searchParameter.IsActive));
            }
            

            queryString.Append(string.Format(" IsDeleted.Equals(false)", tenantCode));
            return queryString.ToString();
        }

        /// <summary>
        /// This method help to set and validate connection string
        /// </summary>
        /// <param name="tenantCode">
        /// The tenant code
        /// </param>
        private void SetAndValidateConnectionString(string tenantCode)
        {
            try
            {
                this.connectionString = validationEngine.IsValidText(this.connectionString) ? this.connectionString : this.configurationutility.GetConnectionString(ModelConstant.COMMON_SECTION, ModelConstant.NIS_CONNECTION_STRING, ModelConstant.CONFIGURATON_BASE_URL, ModelConstant.TENANT_CODE_KEY, tenantCode);
                if (!this.validationEngine.IsValidText(this.connectionString))
                {
                    throw new ConnectionStringNotFoundException(tenantCode);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method determines uniqueness of elements in repository.
        /// </summary>
        /// <param name="tenantUserRoleAccesses">The pages to save.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns>
        /// Returns true if all elements are not present in repository, false otherwise.
        /// </returns>
        private bool IsDuplicateTenantUserRoleMapping(IList<MultiTenantUserRoleAccess> tenantUserRoleAccesses, string operation, string tenantCode)
        {
            bool result = false;
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                StringBuilder query = new StringBuilder();
                if (operation.Equals(ModelConstant.ADD_OPERATION))
                {
                    query.Append("(" + string.Join(" or ", tenantUserRoleAccesses.Select(item => string.Format("AssociatedTenantCode.Equals(\"{0}\") and UserId.Equals({1}) and OtherTenantCode.Equals(\"{2}\")", item.AssociatedTenantCode, item.UserId, item.OtherTenantCode)).ToList()) + ") and IsDeleted.Equals(false) ");
                }

                if (operation.Equals(ModelConstant.UPDATE_OPERATION))
                {
                    query.Append("(" + string.Join(" or ", tenantUserRoleAccesses.Select(item => string.Format("AssociatedTenantCode.Equals(\"{0}\") and UserId.Equals({1}) and OtherTenantCode.Equals(\"{2}\") and !Id.Equals({3}) ", item.AssociatedTenantCode, item.UserId, item.OtherTenantCode, item.Identifier)).ToList()) + ") and IsDeleted.Equals(false) ");
                }
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    var records = nISEntitiesDataContext.MultiTenantUserAccessMapRecords.Where(query.ToString()).Select(item => item).AsQueryable().ToList();
                    if (records.Count > 0)
                    {
                        result = true;
                    }
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }

            return result;
        }

        #endregion

    }
}
