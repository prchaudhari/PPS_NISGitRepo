// <copyright file="SQLRenderEngineRepository.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2020 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------

namespace nIS
{
    #region References

    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Linq.Dynamic;
    using System.Text;
    using Unity;

    #endregion

    /// <summary>
    /// Represents render engine repository that defines methods to access the repository.
    /// </summary>
    public class SQLRenderEngineRepository : IRenderEngineRepository
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

        /// <summary>
        /// Initializing instance of class.
        /// </summary>
        public SQLRenderEngineRepository(IUnityContainer unityContainer)
        {
            this.unityContainer = unityContainer;
            this.validationEngine = new ValidationEngine();
            this.configurationutility = new ConfigurationUtility(this.unityContainer);
            this.utility = new Utility();
        }

        #endregion

        #region Public Methods

        #region Add RenderEngine

        /// <summary>
        /// This method helps to adds the specified list of renderEngine in renderEngine repository.
        /// </summary>
        /// <param name="renderEngines">The list of renderEngines</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns true if render engines are added successfully, else false.
        /// </returns>
        public bool AddRenderEngine(IList<RenderEngine> renderEngines, string tenantCode)
        {
            try
            {
                SetAndValidateConnectionString(tenantCode);
                IsDuplicateRenderEngine(renderEngines, "AddOperation", tenantCode);

                IList<RenderEngineRecord> renderEngineRecords = new List<RenderEngineRecord>();
                renderEngines.ToList().ForEach(renderEngine =>
                {
                    renderEngineRecords.Add(new RenderEngineRecord()
                    {
                        Name = renderEngine.RenderEngineName,
                        URL = renderEngine.URL,
                        PriorityLevel = renderEngine.PriorityLevel,
                        IsActive = false,
                        IsDeleted = false,
                        InUse = false,
                        NumberOfThread = renderEngine.NumberOfThread
                    });
                });

                if (renderEngineRecords.Count > 0)
                {
                    using (NISEntities nisEntitiesDataContext = new NISEntities(this.connectionString))
                    {
                        nisEntitiesDataContext.RenderEngineRecords.AddRange(renderEngineRecords);
                        nisEntitiesDataContext.SaveChanges();
                    }
                }

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Update RenderEngine

        /// <summary>
        /// This method helps to update the specified list of renderEngines in render engine repository.
        /// </summary>
        /// <param name="renderEngines">The list of render engine</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns true if render engine are updated successfully, else false.
        /// </returns>
        public bool UpdateRenderEngine(IList<RenderEngine> renderEngines, string tenantCode)
        {
            bool result = false;
            try
            {
                SetAndValidateConnectionString(tenantCode);
                IsDuplicateRenderEngine(renderEngines, ModelConstant.UPDATE_OPERATION, tenantCode);

                StringBuilder query = new StringBuilder();
                query.Append("(" + string.Join("or ", string.Join(",", renderEngines.Select(item => item.Identifier).Distinct()).ToString().Split(',').Select(item => string.Format("Id.Equals({0}) ", item))) + ") ");
                using (NISEntities nisEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    IList<RenderEngineRecord> renderEngineRecords = nisEntitiesDataContext.RenderEngineRecords.Where(query.ToString()).Select(item => item).AsQueryable().ToList();

                    if (renderEngineRecords == null || renderEngineRecords.Count <= 0 || renderEngineRecords.Count() != string.Join(",", renderEngines.Select(item => item.Identifier).Distinct()).ToString().Split(',').Length)
                    {
                        throw new RenderEngineNotFoundException(tenantCode);
                    }

                    renderEngines.ToList().ForEach(renderEngine =>
                    {
                        RenderEngineRecord renderEngineRecord = renderEngineRecords.Single(item => item.Id == renderEngine.Identifier);
                        renderEngineRecord.Name = renderEngine.RenderEngineName;
                        renderEngineRecord.URL = renderEngine.URL;
                        renderEngineRecord.PriorityLevel = renderEngine.PriorityLevel;
                        //renderEngineRecord.IsActive = renderEngine.IsActive;
                        //renderEngineRecord.IsDeleted = renderEngine.IsDeleted;
                        renderEngineRecord.NumberOfThread = renderEngine.NumberOfThread;
                    });

                    nisEntitiesDataContext.SaveChanges();
                }

                result = true;
            }
            catch (SqlException)
            {
                throw new RepositoryStoreNotAccessibleException(tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return result;
        }

        /// <summary>
        /// Updates the render engine from job.
        /// </summary>
        /// <param name="renderEngines">The render engines.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns></returns>
        /// <exception cref="RenderEngineNotFoundException"></exception>
        /// <exception cref="RepositoryStoreNotAccessibleException"></exception>
        public bool UpdateRenderEngineFromJob(IList<RenderEngine> renderEngines, string tenantCode)
        {
            bool result = false;
            try
            {
                SetAndValidateConnectionString(tenantCode);
                IsDuplicateRenderEngine(renderEngines, ModelConstant.UPDATE_OPERATION, tenantCode);

                StringBuilder query = new StringBuilder();
                query.Append("(" + string.Join("or ", string.Join(",", renderEngines.Select(item => item.Identifier).Distinct()).ToString().Split(',').Select(item => string.Format("Id.Equals({0}) ", item))) + ") ");
                using (NISEntities nisEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    IList<RenderEngineRecord> renderEngineRecords = nisEntitiesDataContext.RenderEngineRecords.Where(query.ToString()).Select(item => item).AsQueryable().ToList();

                    if (renderEngineRecords == null || renderEngineRecords.Count <= 0 || renderEngineRecords.Count() != string.Join(",", renderEngines.Select(item => item.Identifier).Distinct()).ToString().Split(',').Length)
                    {
                        throw new RenderEngineNotFoundException(tenantCode);
                    }

                    renderEngines.ToList().ForEach(renderEngine =>
                    {
                        RenderEngineRecord renderEngineRecord = renderEngineRecords.Single(item => item.Id == renderEngine.Identifier);
                        renderEngineRecord.InUse = renderEngine.InUse;
                    });

                    nisEntitiesDataContext.SaveChanges();
                }

                result = true;
            }
            catch (SqlException)
            {
                throw new RepositoryStoreNotAccessibleException(tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return result;
        }

        #endregion

        #region Delete RenderEngine

        /// <summary>
        /// This method helps to delete the specified list of render engine in render engine repository.
        /// </summary>
        /// <param name="renderEngines">The list of render engine</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns true if render engine are deleted successfully, else false.
        /// </returns>        
        public bool DeleteRenderEngine(IList<RenderEngine> renderEngines, string tenantCode)
        {
            bool result = false;
            try
            {
                this.SetAndValidateConnectionString(tenantCode);

                using (NISEntities nisEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    foreach (RenderEngine renderEngine in renderEngines)
                    {
                        RenderEngineRecord renderEngineRecords = nisEntitiesDataContext.RenderEngineRecords.Where(item => item.Id == renderEngine.Identifier).FirstOrDefault();
                        if (renderEngineRecords == null)
                        {
                            throw new RenderEngineNotFoundException(tenantCode);
                        }
                        renderEngineRecords.IsDeleted = true;
                        nisEntitiesDataContext.SaveChanges();
                    }

                    result = true;
                }
            }
            catch (SqlException)
            {
                throw new RepositoryStoreNotAccessibleException(tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return result;
        }

        #endregion

        #region Get RenderEngine

        /// <summary>
        /// This method helps to retrieve render engine based on specified search condition from repository.
        /// </summary>
        /// <param name="renderEngineSearchParameter">The render engine search parameter</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of render engine based on given search criteria
        /// </returns>
        public IList<RenderEngine> GetRenderEngine(RenderEngineSearchParameter renderEngineSearchParameter, string tenantCode)
        {
            IList<RenderEngine> renderEngines = new List<RenderEngine>();
            IList<RenderEngineRecord> renderEngineRecords = new List<RenderEngineRecord>();
            try
            {
                this.SetAndValidateConnectionString(tenantCode);

                using (NISEntities nisEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    string result = this.WhereClauseGenerator(renderEngineSearchParameter, tenantCode);

                    if (renderEngineSearchParameter.PagingParameter.PageIndex != 0 && renderEngineSearchParameter.PagingParameter.PageSize != 0)
                    {
                        renderEngineRecords = nisEntitiesDataContext.RenderEngineRecords.Where(result).
                        OrderBy(renderEngineSearchParameter.SortParameter.SortColumn + " " + renderEngineSearchParameter.SortParameter.SortOrder)
                       .Skip(renderEngineSearchParameter.PagingParameter.PageSize * (renderEngineSearchParameter.PagingParameter.PageIndex - 1))
                       .Take(renderEngineSearchParameter.PagingParameter.PageIndex * renderEngineSearchParameter.PagingParameter.PageSize).ToList();
                    }
                    else
                    {
                        renderEngineRecords = nisEntitiesDataContext.RenderEngineRecords.Where(result).
                        OrderBy(renderEngineSearchParameter.SortParameter.SortColumn + " " + renderEngineSearchParameter.SortParameter.SortOrder).ToList();
                    }

                    if (renderEngineRecords?.Count > 0)
                    {
                        renderEngineRecords.ToList().ForEach(item =>
                        {
                            renderEngines.Add(new RenderEngine()
                            {
                                Identifier = item.Id,
                                RenderEngineName = item.Name,
                                URL = item.URL,
                                PriorityLevel = item.PriorityLevel.HasValue ? item.PriorityLevel.Value : 0,
                                IsActive = item.IsActive,
                                IsDeleted = item.IsDeleted,
                                InUse = item.InUse,
                                NumberOfThread = item.NumberOfThread
                            });
                        });
                    }
                }
            }
            catch (SqlException)
            {
                throw new RepositoryStoreNotAccessibleException(tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return renderEngines;
        }

        /// <summary>
        /// This method helps to retrieve render engine from repository.
        /// </summary>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>Returns the list of render engine</returns>
        public IList<RenderEngine> GetRenderEngine(string tenantCode)
        {
            IList<RenderEngine> renderEngines = new List<RenderEngine>();
            IList<RenderEngineRecord> renderEngineRecords = new List<RenderEngineRecord>();
            try
            {
                this.SetAndValidateConnectionString(tenantCode);

                using (NISEntities nisEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    renderEngineRecords = nisEntitiesDataContext.RenderEngineRecords.ToList();
                    if (renderEngineRecords?.Count > 0)
                    {
                        renderEngineRecords.ToList().ForEach(item =>
                        {
                            renderEngines.Add(new RenderEngine()
                            {
                                Identifier = item.Id,
                                RenderEngineName = item.Name,
                                URL = item.URL,
                                PriorityLevel = item.PriorityLevel.HasValue ? item.PriorityLevel.Value : 0,
                                IsActive = item.IsActive,
                                IsDeleted = item.IsDeleted,
                                InUse = item.InUse,
                                NumberOfThread = item.NumberOfThread
                            });
                        });
                    }
                }
            }
            catch (SqlException)
            {
                throw new RepositoryStoreNotAccessibleException(tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return renderEngines;
        }

        #endregion

        #region Get RenderEngine Count

        /// <summary>
        /// This method reference to get render engine count
        /// </summary>
        /// <param name="renderEngineSearchParameter"></param>
        /// <param name="tenantCode"></param>
        /// <returns>render engine count</returns>
        public int GetRenderEngineCount(RenderEngineSearchParameter renderEngineSearchParameter, string tenantCode)
        {
            int renderEngineCount = 0;
            string whereClause = string.Empty;
            try
            {
                whereClause = this.WhereClauseGenerator(renderEngineSearchParameter, tenantCode);
                this.SetAndValidateConnectionString(tenantCode);
                using (NISEntities nisEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    renderEngineCount = nisEntitiesDataContext.RenderEngineRecords.Where(whereClause.ToString()).Count();
                }
            }
            catch (SqlException)
            {
                throw new RepositoryStoreNotAccessibleException(tenantCode);
            }
            catch (Exception)
            {
                throw;
            }

            return renderEngineCount;
        }

        #endregion

        #region Activate

        /// <summary>
        /// This method will be used to activate render engine
        /// </summary>
        /// <param name="renderEngineIdentifier">The render engine identifier</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>Returns true is activated render engine successfully otherwise false</returns>
        public bool ActivateRenderEngine(long renderEngineIdentifier, string tenantCode)
        {
            bool result = false;
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                using (NISEntities nisEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    RenderEngineRecord renderEngineRecord = nisEntitiesDataContext.RenderEngineRecords.FirstOrDefault(x => x.Id == renderEngineIdentifier);
                    renderEngineRecord.IsActive = true;
                    nisEntitiesDataContext.SaveChanges();
                }
                result = true;
                return result;
            }
            catch (SqlException)
            {
                throw new RepositoryStoreNotAccessibleException(tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        #endregion

        #region DeActivate

        /// <summary>
        /// This method will be used to deactivate render engine
        /// </summary>
        /// <param name="renderEngineIdentifier">The render engine identifier</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>Returns true is activated render engine successfully otherwise false</returns>
        public bool DeactivateRenderEngine(long renderEngineIdentifier, string tenantCode)
        {
            bool result = false;
            try
            {
                this.SetAndValidateConnectionString(tenantCode);

                using (NISEntities nisEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    RenderEngineRecord renderEngineRecord = nisEntitiesDataContext.RenderEngineRecords.FirstOrDefault(x => x.Id == renderEngineIdentifier);
                    renderEngineRecord.IsActive = false;
                    nisEntitiesDataContext.SaveChanges();
                }
                result = true;
                return result;
            }
            catch (SqlException)
            {
                throw new RepositoryStoreNotAccessibleException(tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        #endregion

        #endregion

        #region Private Methods

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
                this.connectionString = this.configurationutility.GetConnectionString(ModelConstant.COMMON_SECTION, ModelConstant.NIS_CONNECTION_STRING, ModelConstant.CONFIGURATON_BASE_URL, ModelConstant.TENANT_CODE_KEY, tenantCode);
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

        #region Check Duplicate RenderEngine

        /// <summary>
        /// This method will be used to check duplicate render engine 
        /// </summary>
        /// <param name="renderEngines">list of render engine object</param>
        /// <param name="tenantCode"> the tenant code</param>
        private void IsDuplicateRenderEngine(IList<RenderEngine> renderEngines, string operation, string tenantCode)
        {
            try
            {
                this.SetAndValidateConnectionString(tenantCode);

                StringBuilder query = new StringBuilder();
                if (operation.Equals(ModelConstant.ADD_OPERATION))
                {
                    query.Append("(" + string.Join(" or ", renderEngines.Select(item => string.Format("(URL.Equals(\"{0}\") or Name.Equals(\"{1}\"))", item.URL, item.RenderEngineName)).ToList()) + ") and IsDeleted.Equals(false)");
                }

                if (operation.Equals(ModelConstant.UPDATE_OPERATION))
                {
                    query.Append("(" + string.Join(" or ", renderEngines.Select(item => string.Format("((URL.Equals(\"{0}\") or Name.Equals(\"{1}\")) and !Id.Equals({2}))", item.URL, item.RenderEngineName, item.Identifier))) + ") and IsDeleted.Equals(false)");
                }

                using (NISEntities nisEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    IList<RenderEngineRecord> renderEngineRecords = nisEntitiesDataContext.RenderEngineRecords.Where(query.ToString()).Select(item => item).AsQueryable().ToList();
                    if (renderEngineRecords.Count > 0)
                    {
                        throw new DuplicateRenderEngineFoundException(tenantCode);
                    }
                }
            }
            catch (SqlException)
            {
                throw new RepositoryStoreNotAccessibleException(tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
        #endregion

        #region Where Clause Generator

        /// <summary>
        /// This method will be used to generate Where Clause
        /// </summary>
        /// <param name="renderEngineSearchParameter">query will be done on search Parameter</param>
        /// <param name="tenantCode">the tenant code </param>
        /// <returns></returns>
        private string WhereClauseGenerator(RenderEngineSearchParameter renderEngineSearchParameter, string tenantCode)
        {
            StringBuilder queryString = new StringBuilder();
            try
            {
                if (validationEngine.IsValidText(renderEngineSearchParameter.Identifier))
                {
                    queryString.Append("(" + string.Join("or ", renderEngineSearchParameter.Identifier.ToString().Split(',').Select(item => string.Format("Id.Equals({0}) ", item))) + ") and ");
                }

                if (validationEngine.IsValidText(renderEngineSearchParameter.RenderEngineName))
                {
                    if (renderEngineSearchParameter.SearchMode == SearchMode.Equals)
                    {
                        queryString.Append(string.Format("Name.Equals(\"{0}\") and ", renderEngineSearchParameter.RenderEngineName));
                    }
                    else
                    {
                        queryString.Append(string.Format("Name.Contains(\"{0}\") and ", renderEngineSearchParameter.RenderEngineName));
                    }
                }

                if (validationEngine.IsValidText(renderEngineSearchParameter.RenderEngineURL))
                {
                    if (renderEngineSearchParameter.SearchMode == SearchMode.Equals)
                    {
                        queryString.Append(string.Format("URL.Equals(\"{0}\") and ", renderEngineSearchParameter.RenderEngineURL));
                    }
                    else
                    {
                        queryString.Append(string.Format("URL.Contains(\"{0}\") and ", renderEngineSearchParameter.RenderEngineURL));
                    }
                }

                queryString.Append("IsDeleted.Equals(false) ");
                return queryString.ToString();
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
        #endregion

        #endregion
    }
}
