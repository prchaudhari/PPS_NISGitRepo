// <copyright file="SQLRoleRepository.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2018 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------

namespace nIS
{
    #region References
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Dynamic;
    using System.Text;
    using Unity;
    #endregion

    /// <summary>
    /// This class represents the methods to perform operation with database for role entity.
    /// </summary>
    /// 
    public class SQLRoleRepository : IRoleRepository
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

        public SQLRoleRepository(IUnityContainer unityContainer)
        {
            this.unityContainer = unityContainer;
            this.validationEngine = new ValidationEngine();
            this.utility = new Utility();
            this.configurationutility = new ConfigurationUtility(this.unityContainer);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// This method adds the specified list of role in the repository.
        /// </summary>
        /// <param name="roles"></param>
        /// <param name="tenantCode"></param>
        /// <returns>
        /// True, if the role values are added successfully, false otherwise
        /// </returns>
        public bool AddRoles(IList<Role> roles, string tenantCode)
        {
            bool result = false;
            try
            {
                this.SetAndValidateConnectionString(tenantCode);

                if (this.IsDuplicateRole(roles, "AddOperation", tenantCode))
                {
                    throw new DuplicateRoleFoundException(tenantCode);
                }
                IList<RoleRecord> roleRecords = new List<RoleRecord>();
                IList<RolePrivilegeRecord> rolePrivilegeRecords = new List<RolePrivilegeRecord>();

                roles.ToList().ForEach(role =>
                {
                    roleRecords.Add(new RoleRecord()
                    {
                        Name = role.Name,
                        Description = role.Description,
                        IsDeleted = false,
                        TenantCode = tenantCode
                    });
                });

                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    nISEntitiesDataContext.RoleRecords.AddRange(roleRecords);
                    nISEntitiesDataContext.SaveChanges();
                    result = true;
                }

                //Get Roles:---
                IList<RoleRecord> retrievedRoleRecords = new List<RoleRecord>();
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    retrievedRoleRecords = nISEntitiesDataContext.RoleRecords
                    .Where(i => i.TenantCode == tenantCode)
                    .ToList();
                }
                roles.ToList().ForEach(item =>
                {
                    item.Identifier = retrievedRoleRecords.ToList().Where(rolefd => rolefd.Name == item.Name && rolefd.TenantCode == tenantCode).FirstOrDefault().Id;
                    if (item.RolePrivileges?.Count > 0)
                    {
                        item.RolePrivileges.ToList().ForEach(previlegeItem =>
                        {
                            previlegeItem.RolePrivilegeOperations.ToList().ForEach(operation =>
                            {
                                rolePrivilegeRecords.Add(new RolePrivilegeRecord()
                                {
                                    RoleIdentifier = item.Identifier,
                                    EntityName = previlegeItem.EntityName,
                                    Operation = operation.Operation,
                                    IsEnable = operation.IsEnabled
                                });
                            });
                        });
                    }
                });

                //Add roleprivileges
                if (rolePrivilegeRecords?.Count > 0)
                {
                    using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                    {
                        nISEntitiesDataContext.RolePrivilegeRecords.AddRange(rolePrivilegeRecords);
                        nISEntitiesDataContext.SaveChanges();
                        result = true;
                    }
                }
            }

            catch
            {
                throw;
            }

            return result;
        }

        /// <summary>
        /// This method helps to update already added roles entry to database.
        /// </summary>
        /// <param name="roles"></param>
        /// <param name="tenantCode"></param>
        /// <returns>
        /// True, if the role values are updated successfully,otherwise false
        /// </returns>
        public bool UpdateRoles(IList<Role> roles, string tenantCode)
        {
            bool result = false;
            try
            {
                this.SetAndValidateConnectionString(tenantCode);

                if (this.IsDuplicateRole(roles, "UpdateOperation", tenantCode))
                {
                    throw new DuplicateRoleFoundException(tenantCode);
                }

                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    StringBuilder query = new StringBuilder();
                    query.Append("(" + string.Join("or ", string.Join(",", roles.Select(item => item.Identifier).Distinct()).ToString().Split(',').Select(item => string.Format("Id.Equals({0}) ", item))) + ") ");

                    IList<RoleRecord> roleRecords = nISEntitiesDataContext.RoleRecords.Where(query.ToString()).Select(item => item).AsQueryable().ToList();

                    if (roleRecords == null || roleRecords.Count <= 0 || roleRecords.Count() != string.Join(",", roleRecords.Select(item => item.Id).Distinct()).ToString().Split(',').Length)
                    {
                        throw new RoleNotFoundException(tenantCode);
                    }

                    roles.ToList().ForEach(item =>
                    {
                        RoleRecord roleRecord = roleRecords.FirstOrDefault(data => data.Id == item.Identifier && data.TenantCode == tenantCode && data.IsDeleted == false);
                        roleRecord.Name = item.Name;
                        roleRecord.Description = item.Description;
                        roleRecord.TenantCode = tenantCode;
                    });

                    nISEntitiesDataContext.SaveChanges();
                }

                IList<RoleRecord> retrievedRoleRecords = new List<RoleRecord>();
                IList<RolePrivilegeRecord> rolePrivilegeRecords = new List<RolePrivilegeRecord>();

                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    retrievedRoleRecords = nISEntitiesDataContext.RoleRecords.Where(i => i.TenantCode == tenantCode).ToList();

                    roles.ToList().ForEach(item =>
                    {
                        item.Identifier = retrievedRoleRecords.ToList().Where(rolefd => rolefd.Name == item.Name && rolefd.TenantCode == tenantCode).FirstOrDefault().Id;
                        
                        var existingRolePrivilege = nISEntitiesDataContext.RolePrivilegeRecords.Where(itm => itm.RoleIdentifier == item.Identifier).ToList();
                        nISEntitiesDataContext.RolePrivilegeRecords.RemoveRange(existingRolePrivilege);
                        nISEntitiesDataContext.SaveChanges();

                        if (item.RolePrivileges?.Count > 0)
                        {
                            item.RolePrivileges.ToList().ForEach(previlegeItem =>
                            {
                                previlegeItem.RolePrivilegeOperations.ToList().ForEach(operation =>
                                {
                                    rolePrivilegeRecords.Add(new RolePrivilegeRecord()
                                    {
                                        RoleIdentifier = item.Identifier,
                                        EntityName = previlegeItem.EntityName,
                                        Operation = operation.Operation,
                                        IsEnable = operation.IsEnabled
                                    });
                                });
                            });
                        }
                    });
                }

                //add roleprivileges
                if (rolePrivilegeRecords?.Count > 0)
                {
                    using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                    {
                        nISEntitiesDataContext.RolePrivilegeRecords.AddRange(rolePrivilegeRecords);
                        nISEntitiesDataContext.SaveChanges();
                    }
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
        /// Delete roles from database
        /// </summary>
        /// <param name="roles"></param>
        /// <param name="tenantCode"></param>
        /// <returns>True, if the role values are deleted successfully(soft delete), 
        /// otherwise false</returns>
        public bool DeleteRoles(IList<Role> roles, string tenantCode)
        {
            bool result = false;
            try
            {
                this.SetAndValidateConnectionString(tenantCode);

                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    StringBuilder query = new StringBuilder();
                    query.Append("(" + string.Join("or ", string.Join(",", roles.Select(item => item.Identifier).Distinct()).ToString().Split(',').Select(item => string.Format("Id.Equals({0}) ", item))) + ") ");
                    query.Append("and IsDeleted.Equals(false)");
                    IList<RoleRecord> roleRecords = nISEntitiesDataContext.RoleRecords.Where(query.ToString()).Select(item => item).AsQueryable().ToList();
                    if (roleRecords == null || roleRecords.Count <= 0 || roleRecords.Count() != string.Join(",", roleRecords.Select(item => item.Id).Distinct()).ToString().Split(',').Length)
                    {
                        throw new RoleNotFoundException(tenantCode);
                    }

                    roleRecords.ToList().ForEach(item =>
                    {
                        item.IsDeleted = true;
                    });

                    nISEntitiesDataContext.SaveChanges();
                }
                result = true;
                return result;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// This method used to get the rolse based on search paramter.
        /// </summary>
        /// <param name="roleSearchParameter"></param>
        /// <param name="tenantCode"></param>
        /// <returns>List of roles</returns>
        public IList<Role> GetRoles(RoleSearchParameter roleSearchParameter, string tenantCode)
        {
            IList<Role> roles = new List<Role>();
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                string whereClause = this.WhereClauseGenerator(roleSearchParameter, tenantCode);
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    IList<RoleRecord> roleRecords = new List<RoleRecord>();
                    if (roleSearchParameter.PagingParameter.PageIndex > 0 && roleSearchParameter.PagingParameter.PageSize > 0)
                    {
                        roleRecords = nISEntitiesDataContext.RoleRecords
                        .OrderBy(roleSearchParameter.SortParameter.SortColumn + " " + roleSearchParameter.SortParameter.SortOrder.ToString())
                        .Where(whereClause)
                        .Skip((roleSearchParameter.PagingParameter.PageIndex - 1) * roleSearchParameter.PagingParameter.PageSize)
                        .Take(roleSearchParameter.PagingParameter.PageSize)
                        .ToList();
                    }
                    else
                    {
                        roleRecords = nISEntitiesDataContext.RoleRecords
                        .Where(whereClause)
                        .OrderBy(roleSearchParameter.SortParameter.SortColumn + " " + roleSearchParameter.SortParameter.SortOrder.ToString().ToLower())
                        .ToList();
                    }

                    if (roleRecords != null && roleRecords.Count > 0)
                    {
                        StringBuilder roleIdentifiers = new StringBuilder();
                        roleIdentifiers.Append("(" + string.Join(" or ", roleRecords.Select(item => string.Format("RoleIdentifier.Equals({0})", item.Id))) + ")");

                        IList<RolePrivilegeRecord> rolePrivilegeRecords = null;
                        if (roleSearchParameter.IsRequiredRolePrivileges)
                        {
                            rolePrivilegeRecords = nISEntitiesDataContext.RolePrivilegeRecords.Where(roleIdentifiers.ToString()).ToList();
                        }

                        roles = roleRecords.Select(roleRecord => new Role()
                        {
                            Identifier = roleRecord.Id,
                            Name = roleRecord.Name,
                            Description = roleRecord.Description,
                            RolePrivileges = rolePrivilegeRecords?.Where(item => item.RoleIdentifier == roleRecord.Id)
                            .GroupBy(item => item.EntityName)
                            .Select(item => new RolePrivilege()
                            {
                                EntityName = item.Key,
                                RolePrivilegeOperations = item.Select(x => new RolePrivilegeOperation()
                                {
                                    Operation = x.Operation,
                                    IsEnabled = x.IsEnable
                                })
                                .ToList()
                            })
                            .ToList()
                        }).ToList();
                    }
                }
            }

            catch (Exception ex)
            {
                throw ex;
            }

            return roles;
        }

        /// <summary>
        /// This method helps to get count of roles.
        /// </summary>
        /// <param name="roleSearchParameter"></param>
        /// <param name="tenantCode"></param>
        /// <returns></returns>
        public int GetRoleCount(RoleSearchParameter roleSearchParameter, string tenantCode)
        {
            int roleCount = 0;
            string whereClause = this.WhereClauseGenerator(roleSearchParameter, tenantCode);
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    roleCount = nISEntitiesDataContext.RoleRecords.Where(whereClause.ToString()).Count();
                }
            }
            catch (Exception)
            {
                throw;
            }

            return roleCount;
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
        private string WhereClauseGenerator(RoleSearchParameter searchParameter, string tenantCode)
        {
            StringBuilder queryString = new StringBuilder();

            if (searchParameter.SearchMode == SearchMode.Equals)
            {
                if (validationEngine.IsValidText(searchParameter.Identifier))
                {
                    queryString.Append("(" + string.Join("or ", searchParameter.Identifier.ToString().Split(',').Select(item => string.Format("Id.Equals({0}) ", item))) + ") and ");
                }
                if (validationEngine.IsValidText(searchParameter.Name))
                {
                    queryString.Append(string.Format("Name.Equals(\"{0}\") and ", searchParameter.Name));
                }
            }
            if (searchParameter.SearchMode == SearchMode.Contains)
            {

                if (validationEngine.IsValidText(searchParameter.Name))
                {
                    queryString.Append(string.Format("Name.Contains(\"{0}\") and ", searchParameter.Name));
                }
            }

            queryString.Append(string.Format("TenantCode.Equals(\"{0}\") and IsDeleted.Equals(false) ", tenantCode));

            return queryString.ToString();
        }

        /// <summary>
        /// This method determines uniqueness of elements in repository.
        /// </summary>
        /// <param name="roles">The roles to save.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns name="result">
        /// Returns true if all elements are not present in repository, false otherwise.
        /// </returns>
        private bool IsDuplicateRole(IList<Role> roles, string operation, string tenantCode)
        {
            bool result = false;
            try
            {
                this.SetAndValidateConnectionString(tenantCode);

                StringBuilder query = new StringBuilder();

                if (operation.Equals(ModelConstant.ADD_OPERATION))
                {
                    query.Append("(" + string.Join(" or ", roles.Select(item => string.Format("Name.Equals(\"{0}\")", item.Name)).ToList()) + ") and IsDeleted.Equals(false) and TenantCode.Equals(\"" + tenantCode + "\")");
                }

                if (operation.Equals(ModelConstant.UPDATE_OPERATION))
                {
                    query.Append("(" + string.Join(" or ", roles.Select(item => string.Format("(Name.Equals(\"{0}\") and !Id.Equals({1}))", item.Name, item.Identifier))) + ") and IsDeleted.Equals(false) and TenantCode.Equals(\"" + tenantCode + "\")");
                }

                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    IList<RoleRecord> roleRecords = nISEntitiesDataContext.RoleRecords.Where(query.ToString()).Select(item => item).AsQueryable().ToList();
                    if (roleRecords.Count > 0)
                    {
                        result = true;
                    }
                }
            }
            catch
            {
                throw;
            }

            return result;
        }

        #region Get Connection String

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

        #endregion

        #endregion
    }
}
