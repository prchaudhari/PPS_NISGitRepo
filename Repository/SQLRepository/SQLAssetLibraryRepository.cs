// <copyright file="SQLAssetLibraryRepository.cs" company="Websym Solutions Pvt Ltd">
// Copyright (c) 2017 Websym Solutions Pvt Ltd.
// </copyright>
// -----------------------------------------------------------------------  

namespace nIS
{
    #region Referemces 

    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.IO;
    using System.Linq;
    using System.Linq.Dynamic;
    using System.Text;
    using Unity;

    #endregion

    /// <summary>
    /// This class represents repository layer of accet library for crud operation.
    /// </summary>
    public class SQLAssetLibraryRepository : IAssetLibraryRepository
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
        public SQLAssetLibraryRepository(IUnityContainer unityContainer)
        {
            this.unityContainer = unityContainer;
            this.validationEngine = new ValidationEngine();
            this.configurationutility = new ConfigurationUtility(this.unityContainer);
            this.utility = new Utility();
        }

        #endregion

        #region Public Functions

        #region Asset Library function

        /// <summary>
        /// This is responsible for add asset library
        /// </summary>
        /// <param name="assetLibraries"></param>
        /// <param name="tenantCode"></param>
        /// <returns></returns>
        public bool AddAssetLibraries(IList<AssetLibrary> assetLibraries, string tenantCode)
        {
            bool result = false;
            try
            {
                this.SetAndValidateConnectionString(tenantCode);

                if (this.IsDuplicateAssetLibraries(assetLibraries, "AddOperation", tenantCode))
                {
                    throw new DuplicateAssetLibraryException(tenantCode);
                }
                IList<AssetLibraryRecord> assetLibraryRecords = new List<AssetLibraryRecord>();

                assetLibraries.ToList().ForEach(assetLibrary =>
                {
                    assetLibraryRecords.Add(new AssetLibraryRecord()
                    {
                        Name = assetLibrary.Name,
                        Description = assetLibrary.Description,
                        IsActive = true,
                        IsDeleted = false,
                        TenantCode = tenantCode
                    });
                });

                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    nISEntitiesDataContext.AssetLibraryRecords.AddRange(assetLibraryRecords);
                    nISEntitiesDataContext.SaveChanges();
                    result = true;
                }

            }

            catch
            {
                throw;
            }

            return result;
        }

        /// <summary>
        /// This is responsible for update asset library
        /// </summary>
        /// <param name="assetLibraries"></param>
        /// <param name="tenantCode"></param>
        /// <returns></returns>
        public bool UpdateAssetLibraries(IList<AssetLibrary> assetLibraries, string tenantCode)
        {
            bool result = false;
            try
            {
                this.SetAndValidateConnectionString(tenantCode);

                if (this.IsDuplicateAssetLibraries(assetLibraries, "UpdateOperation", tenantCode))
                {
                    throw new DuplicateAssetLibraryException(tenantCode);
                }

                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    StringBuilder query = new StringBuilder();
                    query.Append("(" + string.Join("or ", string.Join(",", assetLibraries.Select(item => item.Identifier).Distinct()).ToString().Split(',').Select(item => string.Format("Id.Equals({0}) ", item))) + ") ");

                    IList<AssetLibraryRecord> assetLibraryRecords = nISEntitiesDataContext.AssetLibraryRecords.Where(query.ToString()).Select(item => item).AsQueryable().ToList();

                    if (assetLibraryRecords == null || assetLibraryRecords.Count <= 0 || assetLibraryRecords.Count() != string.Join(",", assetLibraryRecords.Select(item => item.Id).Distinct()).ToString().Split(',').Length)
                    {
                        throw new AssetLibraryNotFoundException(tenantCode);
                    }

                    assetLibraries.ToList().ForEach(item =>
                    {
                        AssetLibraryRecord assetLibraryRecord = assetLibraryRecords.FirstOrDefault(data => data.Id == item.Identifier && data.TenantCode == tenantCode && data.IsDeleted == false);
                        assetLibraryRecord.Name = item.Name;
                        assetLibraryRecord.Description = item.Description;
                        assetLibraryRecord.TenantCode = tenantCode;
                    });

                    nISEntitiesDataContext.SaveChanges();
                    result = true;
                }
            }

            catch (Exception exception)
            {
                throw exception;
            }

            return result;
        }

        /// <summary>
        /// This is responsible for delete asset library
        /// </summary>
        /// <param name="assetLibraries"></param>
        /// <param name="tenantCode"></param>
        /// <returns></returns>
        public bool DeleteAssetLibraries(IList<AssetLibrary> assetLibraries, string tenantCode)
        {
            bool result = false;
            try
            {
                this.SetAndValidateConnectionString(tenantCode);

                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    StringBuilder query = new StringBuilder();
                    query.Append("(" + string.Join("or ", string.Join(",", assetLibraries.Select(item => item.Identifier).Distinct()).ToString().Split(',').Select(item => string.Format("Id.Equals({0}) ", item))) + ") ");
                    query.Append("and IsDeleted.Equals(false)");

                    IList<AssetLibraryRecord> assetLibraryRecords = nISEntitiesDataContext.AssetLibraryRecords.Where(query.ToString()).Select(item => item).AsQueryable().ToList();
                    if (assetLibraryRecords == null || assetLibraryRecords.Count <= 0 || assetLibraryRecords.Count() != string.Join(",", assetLibraryRecords.Select(item => item.Id).Distinct()).ToString().Split(',').Length)
                    {
                        throw new AssetLibraryNotFoundException(tenantCode);
                    }

                    #region Delete assets of Asset Library

                    //Get all assets of asset library
                    StringBuilder assetLibraryIdentifiers = new StringBuilder();
                    assetLibraryIdentifiers.Append("(" + string.Join(" or ", assetLibraryRecords.Select(item => string.Format("AssetLibraryId.Equals({0})", item.Id))) + ")");
                    assetLibraryIdentifiers.Append("and IsDeleted.Equals(false)");

                    IList<AssetRecord> assetRecords = null;
                    assetRecords = nISEntitiesDataContext.AssetRecords.Where(assetLibraryIdentifiers.ToString()).ToList();

                    if (assetRecords?.Count > 0)
                    {
                        throw new AssetReferenceException(tenantCode);
                    }

                    assetLibraryIdentifiers.Clear();
                    assetLibraryIdentifiers.Append("(" + string.Join(" or ", assetLibraryRecords.Select(item => string.Format("AssetLibraryId.Equals({0})", item.Id))) + ")");

                    //IList<SceneAssetLibraryMapRecord> sceneAssetLibraryMapRecord = null;
                    //sceneAssetLibraryMapRecord = nISEntitiesDataContext.SceneAssetLibraryMapRecords.Where(assetLibraryIdentifiers.ToString()).ToList();

                    //if (sceneAssetLibraryMapRecord?.Count > 0)
                    //{
                    //    throw new SceneLibraryReferenceException(tenantCode);
                    //}

                    #endregion

                    assetLibraryRecords.ToList().ForEach(item =>
                    {
                        item.IsDeleted = true;
                    });

                    assetRecords.ToList().ForEach(item =>
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
        /// This is responsible for get asset library
        /// </summary>
        /// <param name="assetLibrarySearchParameter"></param>
        /// <param name="tenantCode"></param>
        /// <returns></returns>
        public IList<AssetLibrary> GetAssetLibraries(AssetLibrarySearchParameter assetLibrarySearchParameter, string tenantCode)
        {
            IList<AssetLibrary> assetLibraries = new List<AssetLibrary>();
            try
            {
                this.SetAndValidateConnectionString(tenantCode);

                string whereClause = this.WhereClauseGenerator(assetLibrarySearchParameter, tenantCode);
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    IList<AssetLibraryRecord> assetLibraryRecords = new List<AssetLibraryRecord>();
                    if (assetLibrarySearchParameter.PagingParameter.PageIndex > 0 && assetLibrarySearchParameter.PagingParameter.PageSize > 0)
                    {
                        assetLibraryRecords = nISEntitiesDataContext.AssetLibraryRecords
                        .OrderBy(assetLibrarySearchParameter.SortParameter.SortColumn + " " + assetLibrarySearchParameter.SortParameter.SortOrder.ToString())
                        .Where(whereClause)
                        .Skip((assetLibrarySearchParameter.PagingParameter.PageIndex - 1) * assetLibrarySearchParameter.PagingParameter.PageSize)
                        .Take(assetLibrarySearchParameter.PagingParameter.PageSize)
                        .ToList();
                    }
                    else
                    {
                        assetLibraryRecords = nISEntitiesDataContext.AssetLibraryRecords
                        .Where(whereClause)
                        .OrderBy(assetLibrarySearchParameter.SortParameter.SortColumn + " " + assetLibrarySearchParameter.SortParameter.SortOrder.ToString().ToLower())
                        .ToList();
                    }

                    if (assetLibraryRecords != null && assetLibraryRecords.Count > 0)
                    {
                        StringBuilder assetLibraryIdentifiers = new StringBuilder();
                        assetLibraryIdentifiers.Append("(" + string.Join(" or ", assetLibraryRecords.Select(item => string.Format("AssetLibraryId.Equals({0})", item.Id))) + ")");
                        IList<UserRecord> users = new List<UserRecord>();
                        IList<AssetRecord> assetRecords = null;
                        if (assetLibrarySearchParameter.IsAssetDataRequired)
                        {
                            assetLibraryIdentifiers.Append(string.Format(" and IsDeleted.Equals(false)"));
                            assetRecords = nISEntitiesDataContext.AssetRecords.Where(assetLibraryIdentifiers.ToString()).ToList();
                            if(assetRecords?.Count>0)
                            {
                                StringBuilder userIdentifier = new StringBuilder();
                                userIdentifier.Append("(" + string.Join(" or ", assetRecords.Select(item => string.Format("Id.Equals({0})", item.LastUpdatedBy))) + ")");
                                userIdentifier.Append(string.Format(" and IsDeleted.Equals(false)"));

                                users = nISEntitiesDataContext.UserRecords.Where(userIdentifier.ToString()).ToList();
                            }
                          
                        }
                        assetLibraries = assetLibraryRecords.Select(AssetLibraryRecord => new AssetLibrary()
                        {
                            Identifier = AssetLibraryRecord.Id,
                            Name = AssetLibraryRecord.Name,
                            Description = AssetLibraryRecord.Description,
                            IsActive = AssetLibraryRecord.IsActive,
                            Assets = assetRecords?.Where(a => a.AssetLibraryId == AssetLibraryRecord.Id).Select(i => new Asset()
                            {
                                Identifier = i.Id,
                                AssetLibraryIdentifier = i.AssetLibraryId,
                                FilePath = i.FilePath,
                                Name = i.Name,
                                LastUpdatedBy = new User { Identifier = (long)i.LastUpdatedBy },
                                LastUpdatedDate = DateTime.SpecifyKind((DateTime)i.LastUpdatedDate, DateTimeKind.Utc)

                            })
                            .ToList()
                        }).ToList();
                        assetLibraries?.ToList().ForEach(library => {
                            library.Assets?.ToList().ForEach(asset => {
                                UserRecord record = users.Where(item => item.Id == asset.LastUpdatedBy.Identifier)?.ToList().FirstOrDefault();
                                asset.LastUpdatedBy.FirstName = record.FirstName;
                                asset.LastUpdatedBy.LastName = record.LastName;
                                asset.LastUpdatedBy.LastName = record.LastName;
                            });
                        });

                    }
                }
            }

            catch (Exception ex)
            {
                throw ex;
            }

            return assetLibraries;
        }

        /// <summary>
        /// This is responsible for get count of asset library
        /// </summary>
        /// <param name="assetLibrarySearchParameter"></param>
        /// <param name="tenantCode"></param>
        /// <returns></returns>
        public long GetAssetLibraryCount(AssetLibrarySearchParameter assetLibrarySearchParameter, string tenantCode)
        {
            long assetLibraryCount = 0;
            string whereClause = this.WhereClauseGenerator(assetLibrarySearchParameter, tenantCode);
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                using (NISEntities NISEntities = new NISEntities(this.connectionString))
                {
                    assetLibraryCount = NISEntities.AssetLibraryRecords
                                                      .Where(whereClause.ToString())
                                                      .Count();
                }
            }
            catch (Exception)
            {
                throw;
            }

            return assetLibraryCount;
        }



        #endregion

        #region Assets functions

        /// <summary>
        /// This is responsible for add asset
        /// </summary>
        /// <param name="assets"></param>
        /// <param name="tenantCode"></param>
        /// <returns></returns>
        public bool AddAssets(IList<Asset> assets, string tenantCode)
        {
            bool result = false;
            try
            {
                this.SetAndValidateConnectionString(tenantCode);

                if (this.IsDuplicateAssets(assets, "AddOperation", tenantCode))
                {
                    throw new DuplicateAssetException(tenantCode);
                }
                IList<AssetRecord> assetRecords = new List<AssetRecord>();

                assets.ToList().ForEach(asset =>
                {
                    assetRecords.Add(new AssetRecord()
                    {
                        Name = asset.Name,
                        FilePath = asset.FilePath,
                        AssetLibraryId = asset.AssetLibraryIdentifier,
                        IsDeleted = false,
                        LastUpdatedDate=asset.LastUpdatedDate,
                        LastUpdatedBy=asset.LastUpdatedBy.Identifier
                    });
                });

                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    nISEntitiesDataContext.AssetRecords.AddRange(assetRecords);
                    nISEntitiesDataContext.SaveChanges();
                    result = true;
                }

            }

            catch
            {
                throw;
            }

            return result;
        }

        /// <summary>
        /// This is responsible for update asset
        /// </summary>
        /// <param name="assets"></param>
        /// <param name="tenantCode"></param>
        /// <returns></returns>
        public bool UpdateAssets(IList<Asset> assets, string tenantCode)
        {
            bool result = false;
            try
            {
                this.SetAndValidateConnectionString(tenantCode);

                if (this.IsDuplicateAssets(assets, "UpdateOperation", tenantCode))
                {
                    throw new DuplicateAssetException(tenantCode);
                }

                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    StringBuilder query = new StringBuilder();
                    query.Append("(" + string.Join("or ", string.Join(",", assets.Select(item => item.Identifier).Distinct()).ToString().Split(',').Select(item => string.Format("Id.Equals({0}) ", item))) + ") ");

                    IList<AssetRecord> assetRecords = nISEntitiesDataContext.AssetRecords.Where(query.ToString()).Select(item => item).AsQueryable().ToList();

                    if (assetRecords == null || assetRecords.Count <= 0 || assetRecords.Count() != string.Join(",", assetRecords.Select(item => item.Id).Distinct()).ToString().Split(',').Length)
                    {
                        throw new AssetNotFoundException(tenantCode);
                    }

                    assets.ToList().ForEach(item =>
                    {
                        AssetRecord assetRecord = assetRecords.FirstOrDefault(data => data.Id == item.Identifier && data.IsDeleted == false);
                        assetRecord.Name = item.Name;
                        assetRecord.FilePath = item.FilePath;
                        assetRecord.AssetLibraryId = item.AssetLibraryIdentifier;
                    });

                    nISEntitiesDataContext.SaveChanges();
                    result = true;
                }
            }

            catch (Exception exception)
            {
                throw exception;
            }

            return result;
        }

        /// <summary>
        /// This is responsible for delete asset
        /// </summary>
        /// <param name="assets"></param>
        /// <param name="tenantCode"></param>
        /// <returns></returns>
        public bool DeleteAssets(IList<Asset> assets, string tenantCode)
        {
            bool result = false;
            try
            {
                this.SetAndValidateConnectionString(tenantCode);

                using (NISEntities nVidYoEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    StringBuilder query = new StringBuilder();
                    query.Append("(" + string.Join("or ", string.Join(",", assets.Select(item => item.Identifier).Distinct()).ToString().Split(',').Select(item => string.Format("Id.Equals({0}) ", item))) + ") ");
                    query.Append("and IsDeleted.Equals(false)");
                    IList<AssetRecord> assetRecords = nVidYoEntitiesDataContext.AssetRecords.Where(query.ToString()).Select(item => item).AsQueryable().ToList();
                    if (assetRecords == null || assetRecords.Count <= 0 || assetRecords.Count() != string.Join(",", assetRecords.Select(item => item.Id).Distinct()).ToString().Split(',').Length)
                    {
                        throw new AssetNotFoundException(tenantCode);
                    }

                    //Delete files from path
                    assetRecords.ToList().ForEach(item =>
                    {
                        if ((System.IO.File.Exists(item.FilePath)))
                        {
                            System.IO.File.Delete(item.FilePath);
                        }
                    });

                    assetRecords.ToList().ForEach(item =>
                    {
                        item.IsDeleted = true;
                    });

                    nVidYoEntitiesDataContext.SaveChanges();
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
        /// This is responsible for get assets
        /// </summary>
        /// <param name="assetSearchParameter"></param>
        /// <param name="tenantCode"></param>
        /// <returns></returns>
        public IList<Asset> GetAssets(AssetSearchParameter assetSearchParameter, string tenantCode)
        {
            IList<Asset> assets = new List<Asset>();
            try
            {
                this.SetAndValidateConnectionString(tenantCode);

                string whereClause = this.AssetWhereClauseGenerator(assetSearchParameter, tenantCode);
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    IList<AssetRecord> assetRecords = new List<AssetRecord>();
                    if (assetSearchParameter.PagingParameter.PageIndex > 0 && assetSearchParameter.PagingParameter.PageSize > 0)
                    {
                        assetRecords = nISEntitiesDataContext.AssetRecords
                        .OrderBy(assetSearchParameter.SortParameter.SortColumn + " " + assetSearchParameter.SortParameter.SortOrder.ToString())
                        .Where(whereClause)
                        .Skip((assetSearchParameter.PagingParameter.PageIndex - 1) * assetSearchParameter.PagingParameter.PageSize)
                        .Take(assetSearchParameter.PagingParameter.PageSize)
                        .ToList();
                    }
                    else
                    {
                        assetRecords = nISEntitiesDataContext.AssetRecords
                        .Where(whereClause)
                        .OrderBy(assetSearchParameter.SortParameter.SortColumn + " " + assetSearchParameter.SortParameter.SortOrder.ToString().ToLower())
                        .ToList();
                    }

                    if (assetRecords != null && assetRecords.Count > 0)
                    {
                        StringBuilder userIdentifier = new StringBuilder();
                        userIdentifier.Append("(" + string.Join(" or ", assetRecords.Select(item => string.Format("Id.Equals({0})", item.LastUpdatedBy))) + ")");
                        userIdentifier.Append(string.Format(" and IsDeleted.Equals(false)"));
                        IList<UserRecord> users = new List<UserRecord>();

                        users = nISEntitiesDataContext.UserRecords.Where(userIdentifier.ToString()).ToList();

                        assets = assetRecords.Select(assetRecord => new Asset()
                        {
                            Identifier = assetRecord.Id,
                            Name = assetRecord.Name,
                            FilePath = assetRecord.FilePath,
                            FileContent = Path.GetExtension(assetRecord.FilePath) == ".ssml" ? File.ReadAllText(assetRecord.FilePath) : "",
                            AssetLibraryIdentifier = assetRecord.AssetLibraryId,
                            IsDeleted = assetRecord.IsDeleted,
                            LastUpdatedBy = new User { Identifier = (long)assetRecord.LastUpdatedBy },
                            LastUpdatedDate = DateTime.SpecifyKind((DateTime)assetRecord.LastUpdatedDate, DateTimeKind.Utc)
                        }).ToList();
                        assets?.ToList().ForEach(asset => {
                            UserRecord record = users?.Where(item => item.Id == asset.LastUpdatedBy.Identifier)?.ToList().FirstOrDefault();
                            asset.LastUpdatedBy.FirstName = record.FirstName;
                            asset.LastUpdatedBy.LastName = record.LastName;
                            asset.LastUpdatedBy.LastName = record.LastName;
                        });
                    }
                }
            }

            catch (Exception ex)
            {
                throw ex;
            }

            return assets;
        }

        /// <summary>
        /// This is responsible for get count of asset
        /// </summary>
        /// <param name="assetSearchParameter"></param>
        /// <param name="tenantCode"></param>
        /// <returns></returns>
        public long GetAssetCount(AssetSearchParameter assetSearchParameter, string tenantCode)
        {
            long assetCount = 0;
            string whereClause = this.AssetWhereClauseGenerator(assetSearchParameter, tenantCode);
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                using (NISEntities NISEntities = new NISEntities(this.connectionString))
                {
                    assetCount = NISEntities.AssetRecords
                                                      .Where(whereClause.ToString())
                                                      .Count();
                }
            }
            catch (Exception)
            {
                throw;
            }

            return assetCount;
        }

        /// <summary>
        /// This method refence to add asset path
        /// </summary>
        /// <param name="assetPathSetting"></param>
        /// <param name="tenantCode"></param>
        /// <returns></returns>
        public bool AddAssetPath(AssetPathSetting assetPathSetting, string tenantCode)
        {
            bool result = false;
            try
            {
                this.SetAndValidateConnectionString(tenantCode);

                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    AssetPathSettingRecord assetPathSettingRecord = nISEntitiesDataContext.AssetPathSettingRecords.FirstOrDefault(x => x.Id == assetPathSetting.Identifier);
                    if (assetPathSettingRecord == null)
                    {
                        AssetPathSettingRecord record = new AssetPathSettingRecord() { AssetPath = assetPathSetting.AssetPath, IsActive = true, IsDeleted = false, TenantCode = tenantCode };
                        nISEntitiesDataContext.AssetPathSettingRecords.Add(record);
                    }
                    else
                    {
                        assetPathSettingRecord.AssetPath = assetPathSetting.AssetPath;
                    }

                    nISEntitiesDataContext.SaveChanges();
                    result = true;
                }

            }
            catch
            {
                throw;
            }

            return result;
        }

        /// <summary>
        /// Get the asset storage path
        /// </summary>
        /// <param name="tenantCode"></param>
        /// <returns></returns>
        public AssetPathSetting GetAssetPath(string tenantCode)
        {
            try
            {
                this.SetAndValidateConnectionString(tenantCode);

                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    var assetPathSettingRecord = nISEntitiesDataContext.AssetPathSettingRecords.FirstOrDefault();

                    if (assetPathSettingRecord != null)
                    {
                        return new AssetPathSetting()
                        {
                            Identifier = assetPathSettingRecord.Id,
                            AssetPath = assetPathSettingRecord.AssetPath,
                            IsActive = assetPathSettingRecord.IsActive
                        };
                    }
                    else
                    {
                        return new AssetPathSetting();
                    }

                }

            }
            catch
            {
                throw;
            }
        }

        #endregion

        #endregion

        #region Private Methods

        #region Asset Library

        /// <summary>
        /// Generate string for dynamic linq.
        /// </summary>
        /// <param name="searchParameter">asset library search Parameters</param>
        /// <returns>
        /// Returns a string.
        /// </returns>
        private string WhereClauseGenerator(AssetLibrarySearchParameter searchParameter, string tenantCode)
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

            if (searchParameter.IsActive != null)
            {
                queryString.Append(string.Format("IsActive.Equals({0}) and ", searchParameter.IsActive));
            }

            if (tenantCode != "00000000-0000-0000-0000-000000000000")
            {
                queryString.Append(string.Format(" TenantCode.Equals(\"{0}\") and IsDeleted.Equals(false) ", tenantCode));
            }
            else
            {
                queryString.Append(string.Format("IsDeleted.Equals(false)"));
            }

            return queryString.ToString();
        }

        /// <summary>
        /// This method determines uniqueness of elements in repository.
        /// </summary>
        /// <param name="assetLibraries">The asset libraries to save.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns name="result">
        /// Returns true if all elements are not present in repository, false otherwise.
        /// </returns>
        private bool IsDuplicateAssetLibraries(IList<AssetLibrary> assetLibraries, string operation, string tenantCode)
        {
            bool result = false;
            try
            {
                this.SetAndValidateConnectionString(tenantCode);

                StringBuilder query = new StringBuilder();

                if (operation.Equals(ModelConstant.ADD_OPERATION))
                {
                    query.Append("(" + string.Join(" or ", assetLibraries.Select(item => string.Format("Name.Equals(\"{0}\")", item.Name)).ToList()) + ") and IsDeleted.Equals(false) and TenantCode.Equals(\"" + tenantCode + "\")");
                }

                if (operation.Equals(ModelConstant.UPDATE_OPERATION))
                {
                    query.Append("(" + string.Join(" or ", assetLibraries.Select(item => string.Format("(Name.Equals(\"{0}\") and !Id.Equals({1}))", item.Name, item.Identifier))) + ") and IsDeleted.Equals(false) and TenantCode.Equals(\"" + tenantCode + "\")");
                }

                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    IList<AssetLibraryRecord> assetLibraryRecords = nISEntitiesDataContext.AssetLibraryRecords.Where(query.ToString()).Select(item => item).AsQueryable().ToList();
                    if (assetLibraryRecords.Count > 0)
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

        #endregion

        #region Assets

        /// <summary>
        /// Generate string for dynamic linq.
        /// </summary>
        /// <param name="searchParameter">asset  search Parameters</param>
        /// <returns>
        /// Returns a string.
        /// </returns>
        private string AssetWhereClauseGenerator(AssetSearchParameter searchParameter, string tenantCode)
        {
            StringBuilder queryString = new StringBuilder();

            if (validationEngine.IsValidText(searchParameter.Identifier))
            {
                queryString.Append("(" + string.Join("or ", searchParameter.Identifier.ToString().Split(',').Select(item => string.Format("Id.Equals({0}) ", item))) + ") and ");
            }

            if (validationEngine.IsValidText(searchParameter.AssetLibraryIdentifier))
            {
                queryString.Append("(" + string.Join("or ", searchParameter.AssetLibraryIdentifier.ToString().Split(',').Select(item => string.Format("AssetLibraryId.Equals({0}) ", item))) + ") and ");
            }

            if (validationEngine.IsValidText(searchParameter.Extension))
            {
                queryString.Append("(" + string.Join("or ", searchParameter.Extension.ToString().Split(',').Select(item => string.Format("FilePath.EndsWith(\"{0}\") ", item.Trim()))) + ") and ");
            }

            if (searchParameter.SearchMode == SearchMode.Equals)
            {
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
            queryString.Append(string.Format(" IsDeleted.Equals(false) "));

            return queryString.ToString();
        }

        /// <summary>
        /// This method determines uniqueness of elements in repository.
        /// </summary>
        /// <param name="assets">The asset s to save.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns name="result">
        /// Returns true if all elements are not present in repository, false otherwise.
        /// </returns>
        private bool IsDuplicateAssets(IList<Asset> assets, string operation, string tenantCode)
        {
            bool result = false;
            try
            {
                this.SetAndValidateConnectionString(tenantCode);

                StringBuilder query = new StringBuilder();

                if (operation.Equals(ModelConstant.ADD_OPERATION))
                {
                    query.Append("(" + string.Join(" or ", assets.Select(item => string.Format("(Name.Equals(\"{0}\") and AssetLibraryId.Equals({1}))", item.Name, item.AssetLibraryIdentifier))) + ") and IsDeleted.Equals(false)");
                }

                if (operation.Equals(ModelConstant.UPDATE_OPERATION))
                {
                    query.Append("(" + string.Join(" or ", assets.Select(item => string.Format("(Name.Equals(\"{0}\") and !Id.Equals({1}) and AssetLibraryId.Equals({2}))", item.Name, item.Identifier, item.AssetLibraryIdentifier))) + ") and IsDeleted.Equals(false)");
                }

                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    IList<AssetRecord> assetRecords = nISEntitiesDataContext.AssetRecords.Where(query.ToString()).Select(item => item).AsQueryable().ToList();
                    if (assetRecords.Count > 0)
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
        #endregion

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

        #endregion
    }
}
