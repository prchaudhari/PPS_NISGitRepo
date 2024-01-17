// <copyright file="SQLStatementRepository.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2018 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------

namespace nIS
{
    using Microsoft.Practices.ObjectBuilder2;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    #region References
    using System;
    using System.Collections.Generic;
    using System.Drawing.Imaging;
    using System.Dynamic;
    using System.IO;
    using System.Linq;
    using System.Linq.Dynamic;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Security.Claims;
    using System.Text;
    using System.Text.RegularExpressions;
    using Unity;
    #endregion

    public class SQLStatementRepository : IStatementRepository
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

        /// <summary>
        /// The Page repository.
        /// </summary>
        private IPageRepository pageRepository = null;

        /// <summary>
        /// The Asset repository.
        /// </summary>
        private IAssetLibraryRepository assetLibraryRepository = null;

        /// <summary>
        /// The Dynamic widget repository.
        /// </summary>
        private IDynamicWidgetRepository dynamicWidgetRepository = null;

        /// <summary>
        /// The Tenant configuration repository.
        /// </summary>
        private ITenantConfigurationRepository tenantConfigurationRepository = null;

        #endregion

        #region Constructor

        public SQLStatementRepository(IUnityContainer unityContainer)
        {
            this.unityContainer = unityContainer;
            this.validationEngine = new ValidationEngine();
            this.utility = new Utility();
            this.configurationutility = new ConfigurationUtility(this.unityContainer);
            this.pageRepository = this.unityContainer.Resolve<IPageRepository>();
            this.assetLibraryRepository = this.unityContainer.Resolve<IAssetLibraryRepository>();
            this.dynamicWidgetRepository = this.unityContainer.Resolve<IDynamicWidgetRepository>();
            this.tenantConfigurationRepository = this.unityContainer.Resolve<ITenantConfigurationRepository>();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// This method reference to publish statement
        /// </summary>
        /// <param name="statementIdentifier"></param>
        /// <param name="tenantCode"></param>
        /// <returns></returns>
        public bool PublishStatement(long statementIdentifier, string tenantCode)
        {
            bool result = false;
            try
            {
                var claims = ClaimsPrincipal.Current.Identities.First().Claims.ToList();
                int userId;
                int.TryParse(claims?.FirstOrDefault(x => x.Type.Equals("UserId", StringComparison.OrdinalIgnoreCase)).Value, out userId);
                var userFullName = claims?.FirstOrDefault(x => x.Type.Equals("UserFullName", StringComparison.OrdinalIgnoreCase)).Value;

                this.SetAndValidateConnectionString(tenantCode);
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    IList<StatementRecord> statementRecords = nISEntitiesDataContext.StatementRecords.Where(itm => itm.Id == statementIdentifier).ToList();
                    if (statementRecords == null || statementRecords.Count <= 0)
                    {
                        throw new StatementNotFoundException(tenantCode);
                    }

                    statementRecords.ToList().ForEach(item =>
                    {
                        item.PublishedBy = userId;
                        item.PublishedOn = DateTime.UtcNow;
                        item.Status = StatementStatus.Published.ToString();

                        SystemActivityHistoryRecord record = new SystemActivityHistoryRecord()
                        {
                            Module = "Statement",
                            EntityId = item.Id,
                            EntityName = item.Name,
                            SubEntityId = null,
                            SubEntityName = null,
                            ActionTaken = "Publish",
                            ActionTakenBy = userId,
                            ActionTakenByUserName = userFullName,
                            ActionTakenDate = DateTime.Now,
                            TenantCode = tenantCode
                        };
                        nISEntitiesDataContext.SystemActivityHistoryRecords.Add(record);
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
        /// This method adds the specified list of statements in statement repository.
        /// </summary>
        /// <param name="statements">The list of statements</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns true if statements are added successfully, else false.
        /// </returns>
        public bool AddStatements(IList<Statement> statements, string tenantCode)
        {
            bool result = false;
            try
            {
                var claims = ClaimsPrincipal.Current.Identities.First().Claims.ToList();
                int userId;
                int.TryParse(claims?.FirstOrDefault(x => x.Type.Equals("UserId", StringComparison.OrdinalIgnoreCase)).Value, out userId);
                var userFullName = claims?.FirstOrDefault(x => x.Type.Equals("UserFullName", StringComparison.OrdinalIgnoreCase)).Value;

                this.SetAndValidateConnectionString(tenantCode);
                if (this.IsDuplicateStatement(statements, "AddOperation", tenantCode))
                {
                    throw new DuplicateStatementFoundException(tenantCode);
                }

                IList<StatementRecord> statementRecords = new List<StatementRecord>();
                statements.ToList().ForEach(statement =>
                {
                    statementRecords.Add(new StatementRecord()
                    {
                        Name = statement.Name,
                        Description = statement.Description,
                        IsActive = true,
                        IsDeleted = false,
                        Status = StatementStatus.New.ToString(),
                        CreatedDate = DateTime.Now,
                        LastUpdatedDate = DateTime.Now,
                        Version = "1",
                        Owner = userId,
                        TenantCode = tenantCode
                    });

                });

                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    nISEntitiesDataContext.StatementRecords.AddRange(statementRecords);
                    nISEntitiesDataContext.SaveChanges();
                }

                IList<SystemActivityHistoryRecord> Records = new List<SystemActivityHistoryRecord>();
                statements.ToList().ForEach(statement =>
                {
                    statement.Identifier = statementRecords.Where(p => p.Name == statement.Name && p.Version == "1").Single().Id;

                    //Add statement widgets in to statement widget map
                    if (statement.StatementPages?.Count > 0)
                    {
                        this.AddStatementPages(statement.StatementPages, statement.Identifier, tenantCode);
                    }
                    
                    Records.Add(new SystemActivityHistoryRecord()
                    {
                        Module = "Statement",
                        EntityId = statement.Identifier,
                        EntityName = statement.Name,
                        SubEntityId = null,
                        SubEntityName = null,
                        ActionTaken = "Add",
                        ActionTakenBy = userId,
                        ActionTakenByUserName = userFullName,
                        ActionTakenDate = DateTime.Now,
                        TenantCode = tenantCode
                    });
                });

                if (Records.Count > 0)
                {
                    using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                    {
                        nISEntitiesDataContext.SystemActivityHistoryRecords.AddRange(Records);
                        nISEntitiesDataContext.SaveChanges();
                    }
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
        /// This method deletes the specified list of statements from statement repository.
        /// </summary>
        /// <param name="statementIdentifier">The statement identifier</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns true if statements are deleted successfully, else false.
        /// </returns>
        public bool DeleteStatements(long statementIdentifier, string tenantCode)
        {
            bool result = false;
            try
            {
                var claims = ClaimsPrincipal.Current.Identities.First().Claims.ToList();
                int userId;
                int.TryParse(claims?.FirstOrDefault(x => x.Type.Equals("UserId", StringComparison.OrdinalIgnoreCase)).Value, out userId);
                var userFullName = claims?.FirstOrDefault(x => x.Type.Equals("UserFullName", StringComparison.OrdinalIgnoreCase)).Value;

                this.SetAndValidateConnectionString(tenantCode);

                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    var schheduleRecords = nISEntitiesDataContext.ScheduleRecords.Where(itm => itm.StatementId == statementIdentifier && itm.IsDeleted == false).ToList();
                    if (schheduleRecords?.Count > 0)
                    {
                        throw new StatementReferenceException(tenantCode);
                    }

                    IList<StatementRecord> statementRecords = nISEntitiesDataContext.StatementRecords.Where(itm => itm.Id == statementIdentifier).ToList();
                    if (statementRecords == null || statementRecords.Count <= 0)
                    {
                        throw new StatementNotFoundException(tenantCode);
                    }

                    IList<SystemActivityHistoryRecord> Records = new List<SystemActivityHistoryRecord>();
                    statementRecords.ToList().ForEach(item =>
                    {
                        item.IsDeleted = true;
                        item.UpdateBy = userId;
                        item.LastUpdatedDate = DateTime.Now;

                        Records.Add(new SystemActivityHistoryRecord()
                        {
                            Module = "Statement",
                            EntityId = item.Id,
                            EntityName = item.Name,
                            SubEntityId = null,
                            SubEntityName = null,
                            ActionTaken = "Delete",
                            ActionTakenBy = userId,
                            ActionTakenByUserName = userFullName,
                            ActionTakenDate = DateTime.Now,
                            TenantCode = tenantCode
                        });
                    });

                    if (Records.Count > 0)
                    {
                        nISEntitiesDataContext.SystemActivityHistoryRecords.AddRange(Records);
                    }
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
        /// This method gets the specified list of statements from statement repository.
        /// </summary>
        /// <param name="statementSearchParameter">The statement search parameter</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of statements
        /// </returns>
        public IList<Statement> GetStatements(StatementSearchParameter statementSearchParameter, string tenantCode)
        {
            IList<Statement> statements = new List<Statement>();

            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                string whereClause = this.WhereClauseGenerator(statementSearchParameter, tenantCode);
                IList<View_StatementDefinitionRecord> view_StatementDefinitions = new List<View_StatementDefinitionRecord>();
                IList<StatementPage> statementPages = new List<StatementPage>();

                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    if (statementSearchParameter.PagingParameter.PageIndex > 0 && statementSearchParameter.PagingParameter.PageSize > 0)
                    {
                        view_StatementDefinitions = nISEntitiesDataContext.View_StatementDefinitionRecord
                        .OrderBy(statementSearchParameter.SortParameter.SortColumn + " " + statementSearchParameter.SortParameter.SortOrder.ToString())
                        .Where(whereClause)
                        .Skip((statementSearchParameter.PagingParameter.PageIndex - 1) * statementSearchParameter.PagingParameter.PageSize)
                        .Take(statementSearchParameter.PagingParameter.PageSize)
                        .ToList();
                    }
                    else
                    {
                        view_StatementDefinitions = nISEntitiesDataContext.View_StatementDefinitionRecord
                        .Where(whereClause)
                        .OrderBy(statementSearchParameter.SortParameter.SortColumn + " " + statementSearchParameter.SortParameter.SortOrder.ToString().ToLower())
                        .ToList();
                    }

                    if (view_StatementDefinitions != null && view_StatementDefinitions.ToList().Count > 0)
                    {
                        if (statementSearchParameter.IsStatementPagesRequired == true)
                        {
                            IList<StatementPageMapRecord> statementPageRecordMaps = new List<StatementPageMapRecord>();
                            StringBuilder mapRecordIdentifier = new StringBuilder();
                            mapRecordIdentifier.Append("(" + string.Join(" or ", view_StatementDefinitions.Select(item => string.Format("StatementId.Equals({0})", item.Id))) + ")");
                            statementPageRecordMaps = nISEntitiesDataContext.StatementPageMapRecords.Where(mapRecordIdentifier.ToString()).OrderBy(item => item.SequenceNumber).ToList();
                            if (statementPageRecordMaps?.Count > 0)
                            {
                                StringBuilder pageIdentifier = new StringBuilder();
                                pageIdentifier.Append("(" + string.Join(" or ", statementPageRecordMaps.Select(item => string.Format("Id.Equals({0})", item.ReferencePageId))) + ")");
                                var pages = nISEntitiesDataContext.PageRecords.Where(pageIdentifier.ToString()).ToList();
                                statementPageRecordMaps?.ToList().ForEach(statementWidgetRecord =>
                                {
                                    statementPages.Add(new StatementPage
                                    {
                                        Identifier = statementWidgetRecord.Id,
                                        StatementId = statementWidgetRecord.StatementId,
                                        ReferencePageId = statementWidgetRecord.ReferencePageId,
                                        TenantCode = statementWidgetRecord.TenantCode,
                                        SequenceNumber = statementWidgetRecord.SequenceNumber,
                                        PageName = pages.Where(item => item.Id == statementWidgetRecord.ReferencePageId).FirstOrDefault().DisplayName,
                                        PagePublishDate = pages.Where(item => item.Id == statementWidgetRecord.ReferencePageId).FirstOrDefault().PublishedOn,
                                        PageVersion = pages.Where(item => item.Id == statementWidgetRecord.ReferencePageId).FirstOrDefault().Version,
                                    });
                                });
                            }
                        }
                    }
                }

                if (view_StatementDefinitions != null && view_StatementDefinitions.ToList().Count > 0)
                {
                    view_StatementDefinitions?.ToList().ForEach(statementRecord =>
                    {
                        statements.Add(new Statement
                        {
                            Identifier = statementRecord.Id,
                            Name = statementRecord.Name,
                            CreatedDate = statementRecord.CreatedDate == null ? DateTime.MinValue : statementRecord.CreatedDate,
                            IsActive = statementRecord.IsActive,
                            LastUpdatedDate = statementRecord.LastUpdatedDate ?? (DateTime)statementRecord.LastUpdatedDate,
                            Owner = statementRecord.Owner,
                            StatementOwnerName = statementRecord.OwnerName,
                            StatementPages = statementPages?.Where(item => item.StatementId == statementRecord.Id)?.ToList(),
                            Status = statementRecord.Status,
                            Version = statementRecord.Version,
                            PublishedBy = statementRecord.PublishedBy,
                            StatementPublishedByUserName = statementRecord.PublishedByName,
                            PublishedOn = statementRecord.PublishedOn != null ? DateTime.SpecifyKind((DateTime)statementRecord.PublishedOn, DateTimeKind.Utc) : DateTime.MinValue,
                            UpdateBy = statementRecord.UpdateBy,
                            Description = statementRecord.Description
                        });
                    });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return statements;
        }

        /// <summary>
        /// This method reference to get statement count
        /// </summary>
        /// <param name="statementSearchParameter"></param>
        /// <param name="tenantCode"></param>
        /// <returns>Statement count</returns>
        public int GetStatementCount(StatementSearchParameter statementSearchParameter, string tenantCode)
        {
            int statementCount = 0;
            string whereClause = this.WhereClauseGenerator(statementSearchParameter, tenantCode);
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    statementCount = nISEntitiesDataContext.View_StatementDefinitionRecord.Where(whereClause.ToString()).Count();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return statementCount;
        }

        /// <summary>
        /// This method updates the specified list of statements in statement repository.
        /// </summary>
        /// <param name="statements">The list of statements</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns true if statements are updated successfully, else false.
        /// </returns>
        public bool UpdateStatements(IList<Statement> statements, string tenantCode)
        {
            bool result = false;
            try
            {
                var claims = ClaimsPrincipal.Current.Identities.First().Claims.ToList();
                int userId;
                int.TryParse(claims?.FirstOrDefault(x => x.Type.Equals("UserId", StringComparison.OrdinalIgnoreCase)).Value, out userId);
                var userFullName = claims?.FirstOrDefault(x => x.Type.Equals("UserFullName", StringComparison.OrdinalIgnoreCase)).Value;

                this.SetAndValidateConnectionString(tenantCode);
                if (this.IsDuplicateStatement(statements, "UpdateOperation", tenantCode))
                {
                    throw new DuplicateStatementFoundException(tenantCode);
                }

                IList<StatementRecord> statementRecords = new List<StatementRecord>();
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    StringBuilder query = new StringBuilder();
                    query.Append("(" + string.Join("or ", string.Join(",", statements.Select(item => item.Identifier).Distinct()).ToString().Split(',').Select(item => string.Format("Id.Equals({0}) ", item))) + ") ");

                    statementRecords = nISEntitiesDataContext.StatementRecords.Where(query.ToString()).Select(item => item).AsQueryable().ToList();
                    if (statementRecords == null || statementRecords.Count <= 0 || statementRecords.Count() != string.Join(",", statementRecords.Select(item => item.Id).Distinct()).ToString().Split(',').Length)
                    {
                        throw new StatementNotFoundException(tenantCode);
                    }

                    IList<SystemActivityHistoryRecord> Records = new List<SystemActivityHistoryRecord>();
                    statements.ToList().ForEach(item =>
                    {
                        StatementRecord statementRecord = statementRecords.FirstOrDefault(data => data.Id == item.Identifier && data.TenantCode == tenantCode && data.IsDeleted == false);
                        statementRecord.LastUpdatedDate = DateTime.Now;
                        statementRecord.UpdateBy = userId;
                        statementRecord.TenantCode = tenantCode;
                        statementRecord.Name = item.Name;
                        statementRecord.Description = item.Description;

                        Records.Add(new SystemActivityHistoryRecord()
                        {
                            Module = "Statement",
                            EntityId = statementRecord.Id,
                            EntityName = statementRecord.Name,
                            SubEntityId = null,
                            SubEntityName = null,
                            ActionTaken = "Update",
                            ActionTakenBy = userId,
                            ActionTakenByUserName = userFullName,
                            ActionTakenDate = DateTime.Now,
                            TenantCode = tenantCode
                        });
                    });

                    if (Records.Count > 0)
                    {
                        nISEntitiesDataContext.SystemActivityHistoryRecords.AddRange(Records);
                    }
                    nISEntitiesDataContext.SaveChanges();
                }

                statements.ToList().ForEach(item =>
                {
                    item.Identifier = statementRecords.ToList().Where(statementRec => statementRec.Name == item.Name).FirstOrDefault().Id;

                    using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                    {
                        var existingStatementPageMappingRecords = nISEntitiesDataContext.StatementPageMapRecords.Where(itm => itm.StatementId == item.Identifier).ToList();
                        nISEntitiesDataContext.StatementPageMapRecords.RemoveRange(existingStatementPageMappingRecords);
                        nISEntitiesDataContext.SaveChanges();
                    }

                    if (item.StatementPages?.Count > 0)
                    {
                        AddStatementPages(item.StatementPages, item.Identifier, tenantCode);
                    }
                });

                result = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        /// <summary>
        /// This method reference to clone statement
        /// </summary>
        /// <param name="statementIdentifier"></param>
        /// <param name="tenantCode"></param>
        /// <returns>return true if statement clone successfully, else false</returns>
        public bool CloneStatement(long statementIdentifier, string tenantCode)
        {
            bool result = false;
            try
            {
                var claims = ClaimsPrincipal.Current.Identities.First().Claims.ToList();
                int userId;
                int.TryParse(claims?.FirstOrDefault(x => x.Type.Equals("UserId", StringComparison.OrdinalIgnoreCase)).Value, out userId);
                var userFullName = claims?.FirstOrDefault(x => x.Type.Equals("UserFullName", StringComparison.OrdinalIgnoreCase)).Value;

                this.SetAndValidateConnectionString(tenantCode);
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    var statementRecord = nISEntitiesDataContext.StatementRecords.Where(itm => itm.Id == statementIdentifier).FirstOrDefault();
                    if (statementRecord == null)
                    {
                        throw new StatementNotFoundException(tenantCode);
                    }

                    var lastStatementRecord = nISEntitiesDataContext.StatementRecords.Where(item => item.Name == statementRecord.Name && item.IsDeleted == false).OrderByDescending(itm => itm.Id).FirstOrDefault();
                    if (lastStatementRecord == null)
                    {
                        throw new StatementNotFoundException(tenantCode);
                    }

                    IList<StatementRecord> statementRecordsForClone = new List<StatementRecord>();
                    statementRecordsForClone.Add(new StatementRecord()
                    {
                        Name = lastStatementRecord.Name,
                        IsActive = true,
                        IsDeleted = false,
                        Status = StatementStatus.New.ToString(),
                        CreatedDate = DateTime.Now,
                        LastUpdatedDate = DateTime.Now,
                        Version = Int64.Parse(lastStatementRecord.Version) + 1 + "",
                        Owner = userId,
                        TenantCode = tenantCode
                    });
                    nISEntitiesDataContext.StatementRecords.AddRange(statementRecordsForClone);

                    SystemActivityHistoryRecord record = new SystemActivityHistoryRecord()
                    {
                        Module = "Statement",
                        EntityId = lastStatementRecord.Id,
                        EntityName = lastStatementRecord.Name,
                        SubEntityId = null,
                        SubEntityName = null,
                        ActionTaken = "Clone",
                        ActionTakenBy = userId,
                        ActionTakenByUserName = userFullName,
                        ActionTakenDate = DateTime.Now,
                        TenantCode = tenantCode
                    };
                    nISEntitiesDataContext.SystemActivityHistoryRecords.Add(record);

                    nISEntitiesDataContext.SaveChanges();

                    //long newStatementIdentifier = statementRecordsForClone.Where(p => p.Name == lastStatementRecord.Name && p.Version == Int64.Parse(lastStatementRecord.Version) + 1 + "").Single().Id;

                    //IList<StatementPageMapRecord> statementWidgetRecords = nISEntitiesDataContext.StatementPageMapRecords.Where(item => item.StatementId == lastStatementRecord.Id).ToList();
                    //IList<StatementPageMapRecord> statementWidgetRecordsForClone = new List<StatementPageMapRecord>();
                    //statementWidgetRecords.ToList().ForEach(item =>
                    //{
                    //    statementWidgetRecordsForClone.Add(new StatementPageMapRecord()
                    //    {
                    //        //Height = item.Height,
                    //        //StatementId = newStatementIdentifier,
                    //        //ReferenceWidgetId = item.ReferenceWidgetId,
                    //        //TenantCode = tenantCode,
                    //        //WidgetSetting = item.WidgetSetting,
                    //        //Width = item.Width,
                    //        //Xposition = item.Xposition,
                    //        //Yposition = item.Yposition
                    //    });
                    //});

                    //nISEntitiesDataContext.StatementPageMapRecords.AddRange(statementWidgetRecordsForClone);
                    //nISEntitiesDataContext.SaveChanges();
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
        /// This method will geenerate preview Statement html string
        /// </summary>
        /// <param name="StatementIdentifier">Statement identifier</param>
        /// <param name="baseURL">API base URL</param>
        /// <param name="tenantCode">Tenant code of Statement.</param>
        /// <returns>
        /// Returns Statements preview html string.
        /// </returns>
        public string PreviewStatement(long statementIdentifier, string baseURL, string tenantCode)
        {
            string finalHtml = "";
            var tenantConfiguration = this.tenantConfigurationRepository.GetTenantConfigurations(tenantCode)?.FirstOrDefault();
            StatementSearchParameter statementSearchParameter = new StatementSearchParameter
            {
                Identifier = statementIdentifier,
                IsActive = true,
                IsStatementPagesRequired = true,
                PagingParameter = new PagingParameter
                {
                    PageIndex = 0,
                    PageSize = 0,
                },
                SortParameter = new SortParameter()
                {
                    SortOrder = SortOrder.Ascending,
                    SortColumn = "Name",
                },
                SearchMode = SearchMode.Equals
            };
            var statements = this.GetStatements(statementSearchParameter, tenantCode);
            Statement statement = new Statement();
            if (statements.Count > 0)
            {
                statement = statements.ToList().FirstOrDefault();

                var statementPageContents = this.GenerateHtmlFormatOfStatement(statement, tenantCode, tenantConfiguration);
                finalHtml = this.BindPreviewDataToStatement(statement, statementPageContents, baseURL, tenantCode);
            }

            return finalHtml;
        }

        /// <summary>
        /// This method will geenerate HTML format of the Statement
        /// </summary>
        /// <param name="statement">the statment</param>
        /// <param name="tenantCode">Tenant code of Statement.</param>
        /// <returns>
        /// Returns list of statement page content object.
        /// </returns>
        public IList<StatementPageContent> GenerateHtmlFormatOfStatement(Statement statement, string tenantCode, TenantConfiguration tenantConfiguration)
        {
            try
            {
                List<StatementPageContent> statementPageContents = new List<StatementPageContent>();
                if (statement != null)
                {
                    //rearrange statement page with their user defined sequence
                    var statementPages = statement.StatementPages.OrderBy(it => it.SequenceNumber).ToList();
                    if (statementPages.Count > 0)
                    {
                        int counter = 0;
                        statement.Pages = new List<Page>();
                        for (int i = 0; i < statementPages.Count; i++)
                        {
                            StatementPageContent statementPageContent = new StatementPageContent();
                            StringBuilder pageHtmlContent = new StringBuilder();
                            statementPageContent.Id = i;
                            PageSearchParameter pageSearchParameter = new PageSearchParameter
                            {
                                Identifier = statementPages[i].ReferencePageId,
                                IsPageWidgetsRequired = true,
                                IsActive = true,
                                PagingParameter = new PagingParameter
                                {
                                    PageIndex = 0,
                                    PageSize = 0,
                                },
                                SortParameter = new SortParameter()
                                {
                                    SortOrder = SortOrder.Ascending,
                                    SortColumn = "DisplayName",
                                },
                                SearchMode = SearchMode.Equals
                            };
                            var pages = this.pageRepository.GetPages(pageSearchParameter, tenantCode);
                            if (pages.Count != 0)
                            {
                                for (int j = 0; j < pages.Count; j++)
                                {
                                    var page = pages[j];
                                    statementPageContent.PageId = page.Identifier;
                                    statementPageContent.PageTypeId = page.PageTypeId;
                                    statementPageContent.DisplayName = page.DisplayName;
                                    statement.Pages.Add(page);

                                    StringBuilder pageHeaderContent = new StringBuilder();
                                    pageHeaderContent.Append(HtmlConstants.WIDGET_HTML_HEADER);
                                    if (page.PageTypeName == HtmlConstants.SAVING_ACCOUNT_PAGE || page.PageTypeName == HtmlConstants.CURRENT_ACCOUNT_PAGE)
                                    {
                                        pageHeaderContent.Append("{{SubTabs}}");
                                    }
                                    statementPageContent.PageHeaderContent = pageHeaderContent.ToString();

                                    int tempRowWidth = 0; // variable to check col-lg div length (bootstrap)
                                    int max = 0;
                                    if (page.PageWidgets.Count > 0)
                                    {
                                        //get all widgets of current page
                                        var completelst = new List<PageWidget>(page.PageWidgets);
                                        
                                        //Get all dynamic widgets of current page and assign it to statement page content object for future use
                                        var dynamicwidgetids = string.Join(", ", completelst.Where(item => item.IsDynamicWidget).ToList().Select(item => item.WidgetId));
                                        DynamicWidgetSearchParameter dynamicWidgetSearchParameter = new DynamicWidgetSearchParameter
                                        {
                                            Identifier = dynamicwidgetids,
                                            PagingParameter = new PagingParameter
                                            {
                                                PageIndex = 0,
                                                PageSize = 0,
                                            },
                                            SortParameter = new SortParameter()
                                            {
                                                SortOrder = SortOrder.Ascending,
                                                SortColumn = "Title",
                                            },
                                            SearchMode = SearchMode.Contains
                                        };
                                        IList<DynamicWidget> dynawidgets = this.dynamicWidgetRepository.GetDynamicWidgets(dynamicWidgetSearchParameter, tenantCode);
                                        statementPageContent.DynamicWidgets = dynawidgets;

                                        //current Y position variable to filter widgets, start with 0
                                        int currentYPosition = 0;
                                        var isRowComplete = false;

                                        while (completelst.Count != 0)
                                        {
                                            //filter 1st row widgets from current page widgets..
                                            //get widgets by current Y position
                                            var lst = completelst.Where(it => it.Yposition == currentYPosition).ToList();
                                            if (lst.Count > 0)
                                            {
                                                //find widget which has max height among them and assign it to max Y position variable
                                                max = max + lst.Max(it => it.Height);

                                                //filter widgets which are in between current Y Position and above max Y Position
                                                var _lst = completelst.Where(it => it.Yposition < max && it.Yposition != currentYPosition).ToList();

                                                //merge 2 widgets into single list which are finds by current Y position and 
                                                //then finds with in between current and max Y position
                                                var mergedlst = lst.Concat(_lst).OrderBy(it => it.Xposition).ToList();

                                                //assign current Y position to new max Y position
                                                currentYPosition = max;

                                                //loop over current merge widget list and create empty HTML template
                                                for (int x = 0; x < mergedlst.Count; x++)
                                                {
                                                    //if tempRowWidth equals to zero then create new div with bootstrap row class
                                                    if (tempRowWidth == 0)
                                                    {
                                                        pageHtmlContent.Append("<div class='row pt-2'>");
                                                        isRowComplete = false;
                                                    }

                                                    //get current widget width
                                                    var divLength = mergedlst[x].Width;

                                                    //get current widget height and multiple with 110 as per angular gridster implementation of each column height
                                                    //to find actual height for current widget in pixel
                                                    var divHeight = mergedlst[x].Height * 110 + "px";
                                                    tempRowWidth = tempRowWidth + divLength;

                                                    // If current col-lg class length is greater than 12, 
                                                    //then end parent row class div and then start new row class div
                                                    if (tempRowWidth > 12)
                                                    {
                                                        tempRowWidth = divLength;
                                                        pageHtmlContent.Append(HtmlConstants.END_DIV_TAG); // to end row class div
                                                        pageHtmlContent.Append("<div class='row pt-2'>"); // to start new row class div
                                                        isRowComplete = false;
                                                    }

                                                    //if rendering html for 1st page, then add padding zero current widget div, otherwise keep default padding
                                                    var leftPaddingClass = i != 0 ? " pl-0" : string.Empty;

                                                    //create new div with col-lg with newly finded div length and above padding zero value
                                                    pageHtmlContent.Append("<div class='col-lg-" + divLength + leftPaddingClass + "'>");

                                                    //check current widget is dynamic or static and start generating empty html template for current widget
                                                    if (!mergedlst[x].IsDynamicWidget)
                                                    {
                                                        if (mergedlst[x].WidgetName == HtmlConstants.CUSTOMER_INFORMATION_WIDGET_NAME)
                                                        {
                                                            string widgetId = "PageWidgetId_" + mergedlst[x].Identifier + "_Counter" + counter.ToString();
                                                            string widgetHTML = HtmlConstants.CUSTOMER_INFORMATION_WIDGET_HTML_FOR_STMT.Replace("{{VideoSource}}", "{{VideoSource_" + statement.Identifier + "_" + page.Identifier + "_" + mergedlst[x].Identifier + "}}");
                                                            widgetHTML = widgetHTML.Replace("{{WidgetDivHeight}}", divHeight);
                                                            widgetHTML = widgetHTML.Replace("{{WidgetId}}", widgetId);
                                                            pageHtmlContent.Append(widgetHTML);

                                                        }
                                                        else if (mergedlst[x].WidgetName == HtmlConstants.PAYMENT_SUMMARY_WIDGET_NAME)
                                                        {
                                                            string widgetId = "PageWidgetId_" + mergedlst[x].Identifier + "_Counter" + counter.ToString();
                                                             string widgetHTML = HtmlConstants.PAYMENT_SUMMARY_WIDGET_HTML_FOR_STMT;
                                                            widgetHTML = widgetHTML.Replace("{{WidgetDivHeight}}", divHeight);
                                                            widgetHTML = widgetHTML.Replace("{{WidgetId}}", widgetId);
                                                            pageHtmlContent.Append(widgetHTML);

                                                        }

                                                        else if (mergedlst[x].WidgetName == HtmlConstants.PPS_HEADING_WIDGET_NAME)
                                                        {
                                                            string widgetId = "PageWidgetId_" + mergedlst[x].Identifier + "_Counter" + counter.ToString();
                                                            string widgetHTML = HtmlConstants.PPS_HEADING_WIDGET_HTML_FOR_STMT;
                                                            widgetHTML = widgetHTML.Replace("{{WidgetDivHeight}}", divHeight);
                                                            widgetHTML = widgetHTML.Replace("{{WidgetId}}", widgetId);
                                                            pageHtmlContent.Append(widgetHTML);

                                                        }

                                                        else if (mergedlst[x].WidgetName == HtmlConstants.PPS_DETAILS_WIDGET_NAME)
                                                        {
                                                            string widgetId = "PageWidgetId_" + mergedlst[x].Identifier + "_Counter" + counter.ToString();
                                                            string widgetHTML = HtmlConstants.PPS_DETAILS_WIDGET_HTML_FOR_STMT;
                                                            widgetHTML = widgetHTML.Replace("{{WidgetDivHeight}}", divHeight);
                                                            widgetHTML = widgetHTML.Replace("{{WidgetId}}", widgetId);
                                                            pageHtmlContent.Append(widgetHTML);

                                                        }
                                                        else if (mergedlst[x].WidgetName == HtmlConstants.PRODUCT_SUMMARY_WIDGET_NAME)
                                                        {
                                                            string widgetId = "PageWidgetId_" + mergedlst[x].Identifier + "_Counter" + counter.ToString();
                                                            //string widgetHTML = HtmlConstants.PAYMENT_SUMMARY_WIDGET_HTML_FOR_STMT.Replace("{{VideoSource}}", "{{VideoSource_" + statement.Identifier + "_" + page.Identifier + "_" + mergedlst[x].Identifier + "}}");
                                                            string widgetHTML = HtmlConstants.PRODUCT_SUMMARY_WIDGET_HTML_FOR_STMT;
                                                            widgetHTML = widgetHTML.Replace("{{WidgetDivHeight}}", divHeight);
                                                            widgetHTML = widgetHTML.Replace("{{WidgetId}}", widgetId);
                                                            pageHtmlContent.Append(widgetHTML);
                                                        }
                                                        else if (mergedlst[x].WidgetName == HtmlConstants.PPS_FOOTER1_WIDGET_NAME)
                                                        {
                                                            string widgetId = "PageWidgetId_" + mergedlst[x].Identifier + "_Counter" + counter.ToString();
                                                            string widgetHTML = HtmlConstants.PPS_FOOTER1_WIDGET_HTML_FOR_STMT;
                                                            widgetHTML = widgetHTML.Replace("{{WidgetDivHeight}}", divHeight);
                                                            widgetHTML = widgetHTML.Replace("{{WidgetId}}", widgetId);
                                                            pageHtmlContent.Append(widgetHTML);

                                                        }

                                                        else if (mergedlst[x].WidgetName == HtmlConstants.FOOTER_IMAGE_WIDGET_NAME)
                                                        {
                                                            string widgetId = "PageWidgetId_" + mergedlst[x].Identifier + "_Counter" + counter.ToString();
                                                            string widgetHTML = HtmlConstants.FOOTER_IMAGE_WIDGET_HTML_FOR_STMT;
                                                            widgetHTML = widgetHTML.Replace("{{WidgetDivHeight}}", divHeight);
                                                            widgetHTML = widgetHTML.Replace("{{WidgetId}}", widgetId);
                                                            pageHtmlContent.Append(widgetHTML);

                                                        }

                                                        else if (mergedlst[x].WidgetName == HtmlConstants.DETAILED_TRANSACTIONS_WIDGET_NAME)
                                                        {
                                                            string widgetId = "PageWidgetId_" + mergedlst[x].Identifier + "_Counter" + counter.ToString();
                                                            //string widgetHTML = HtmlConstants.PAYMENT_SUMMARY_WIDGET_HTML_FOR_STMT.Replace("{{VideoSource}}", "{{VideoSource_" + statement.Identifier + "_" + page.Identifier + "_" + mergedlst[x].Identifier + "}}");
                                                            string widgetHTML = HtmlConstants.DETAILED_TRANSACTIONS_WIDGET_HTML_FOR_STMT;
                                                            widgetHTML = widgetHTML.Replace("{{WidgetDivHeight}}", divHeight);
                                                            widgetHTML = widgetHTML.Replace("{{WidgetId}}", widgetId);
                                                            pageHtmlContent.Append(widgetHTML);
                                                        }

                                                        else if (mergedlst[x].WidgetName == HtmlConstants.PPS_DETAILS1_WIDGET_NAME)
                                                        {
                                                            string widgetId = "PageWidgetId_" + mergedlst[x].Identifier + "_Counter" + counter.ToString();
                                                            string widgetHTML = HtmlConstants.PPS_DETAILS_WIDGET_HTML_FOR_STMT;
                                                            widgetHTML = widgetHTML.Replace("{{WidgetDivHeight}}", divHeight);
                                                            widgetHTML = widgetHTML.Replace("{{WidgetId}}", widgetId);
                                                            pageHtmlContent.Append(widgetHTML);

                                                        }

                                                        else if (mergedlst[x].WidgetName == HtmlConstants.ACCOUNT_INFORMATION_WIDGET_NAME)
                                                        {
                                                            string widgetId = "PageWidgetId_" + mergedlst[x].Identifier + "_Counter" + counter.ToString();
                                                            string widgetHTML = HtmlConstants.ACCOUNT_INFORMATION_WIDGET_HTML_FOR_STMT.Replace("{{AccountInfoData}}", "{{AccountInfoData_" + page.Identifier + "_" + mergedlst[x].Identifier + "}}");
                                                            widgetHTML = widgetHTML.Replace("{{WidgetDivHeight}}", divHeight);
                                                            widgetHTML = widgetHTML.Replace("{{WidgetId}}", widgetId);
                                                            pageHtmlContent.Append(widgetHTML);
                                                        }
                                                        else if (mergedlst[x].WidgetName == HtmlConstants.IMAGE_WIDGET_NAME)
                                                        {

                                                            string widgetId = "PageWidgetId_" + mergedlst[x].Identifier + "_Counter" + counter.ToString();
                                                            string widgetHTML = HtmlConstants.IMAGE_WIDGET_HTML_FOR_STMT.Replace("{{ImageSource}}", "{{ImageSource_" + statement.Identifier + "_" + page.Identifier + "_" + mergedlst[x].Identifier + "}}");
                                                            widgetHTML = widgetHTML.Replace("{{WidgetDivHeight}}", divHeight);
                                                            widgetHTML = widgetHTML.Replace("{{WidgetId}}", widgetId);
                                                            if (mergedlst[x].WidgetSetting != string.Empty && validationEngine.IsValidJson(mergedlst[x].WidgetSetting))
                                                            {
                                                                var imageWidgetHtml = string.Empty;
                                                                dynamic widgetSetting = JObject.Parse(mergedlst[x].WidgetSetting);
                                                                if (widgetSetting.isPersonalize == false && widgetSetting.SourceUrl != null && widgetSetting.SourceUrl != string.Empty)
                                                                {
                                                                    widgetHTML = widgetHTML.Replace("{{TargetLink}}", "<a href='" + widgetSetting.SourceUrl + "' target='_blank'>");
                                                                    widgetHTML = widgetHTML.Replace("{{EndTargetLink}}", "</a>");
                                                                }
                                                                else
                                                                {
                                                                    widgetHTML = widgetHTML.Replace("{{TargetLink}}", "");
                                                                    widgetHTML = widgetHTML.Replace("{{EndTargetLink}}", "");
                                                                }
                                                            }
                                                            pageHtmlContent.Append(widgetHTML);
                                                        }
                                                        else if (mergedlst[x].WidgetName == HtmlConstants.VIDEO_WIDGET_NAME)
                                                        {
                                                            string widgetId = "PageWidgetId_" + mergedlst[x].Identifier + "_Counter" + counter.ToString();
                                                            string widgetHTML = HtmlConstants.VIDEO_WIDGET_HTML_FOR_STMT.Replace("{{VideoSource}}", "{{VideoSource_" + statement.Identifier + "_" + page.Identifier + "_" + mergedlst[x].Identifier + "}}");
                                                            widgetHTML = widgetHTML.Replace("{{WidgetDivHeight}}", divHeight);
                                                            widgetHTML = widgetHTML.Replace("{{WidgetId}}", widgetId);
                                                            pageHtmlContent.Append(widgetHTML);
                                                        }
                                                        else if (mergedlst[x].WidgetName == HtmlConstants.SUMMARY_AT_GLANCE_WIDGET_NAME)
                                                        {
                                                            string widgetId = "PageWidgetId_" + mergedlst[x].Identifier + "_Counter" + counter.ToString();
                                                            string widgetHTML = HtmlConstants.SUMMARY_AT_GLANCE_WIDGET_HTML_FOR_STMT.Replace("{{AccountSummary}}", "{{AccountSummary_" + page.Identifier + "_" + mergedlst[x].Identifier + "}}");
                                                            widgetHTML = widgetHTML.Replace("{{WidgetDivHeight}}", divHeight);
                                                            widgetHTML = widgetHTML.Replace("{{WidgetId}}", widgetId);
                                                            pageHtmlContent.Append(widgetHTML);
                                                        }
                                                        else if (mergedlst[x].WidgetName == HtmlConstants.CURRENT_AVAILABLE_BALANCE_WIDGET_NAME)
                                                        {
                                                            string widgetId = "PageWidgetId_" + mergedlst[x].Identifier + "_Counter" + counter.ToString();
                                                            string CurrentAvailBalanceWidget = HtmlConstants.SAVING_CURRENT_AVALABLE_BAL_WIDGET_HTML_FOR_STMT.Replace("{{TotalValue}}", "{{TotalValue_" + page.Identifier + "_" + mergedlst[x].Identifier + "}}").Replace("{{TotalDeposit}}", "{{TotalDeposit_" + page.Identifier + "_" + mergedlst[x].Identifier + "}}").Replace("{{TotalSpend}}", "{{TotalSpend_" + page.Identifier + "_" + mergedlst[x].Identifier + "}}").Replace("{{Savings}}", "{{Savings_" + page.Identifier + "_" + mergedlst[x].Identifier + "}}");
                                                            CurrentAvailBalanceWidget = CurrentAvailBalanceWidget.Replace("{{WidgetId}}", widgetId);
                                                            CurrentAvailBalanceWidget = CurrentAvailBalanceWidget.Replace("{{WidgetDivHeight}}", divHeight);
                                                            pageHtmlContent.Append(CurrentAvailBalanceWidget);
                                                        }
                                                        else if (mergedlst[x].WidgetName == HtmlConstants.SAVING_AVAILABLE_BALANCE_WIDGET_NAME)
                                                        {
                                                            string widgetId = "PageWidgetId_" + mergedlst[x].Identifier + "_Counter" + counter.ToString();
                                                            string SavingAvailBalanceWidget = HtmlConstants.SAVING_CURRENT_AVALABLE_BAL_WIDGET_HTML_FOR_STMT.Replace("{{TotalValue}}", "{{TotalValue_" + page.Identifier + "_" + mergedlst[x].Identifier + "}}").Replace("{{TotalDeposit}}", "{{TotalDeposit_" + page.Identifier + "_" + mergedlst[x].Identifier + "}}").Replace("{{TotalSpend}}", "{{TotalSpend_" + page.Identifier + "_" + mergedlst[x].Identifier + "}}").Replace("{{Savings}}", "{{Savings_" + page.Identifier + "_" + mergedlst[x].Identifier + "}}");
                                                            SavingAvailBalanceWidget = SavingAvailBalanceWidget.Replace("{{WidgetId}}", widgetId);
                                                            SavingAvailBalanceWidget = SavingAvailBalanceWidget.Replace("{{WidgetDivHeight}}", divHeight);
                                                            pageHtmlContent.Append(SavingAvailBalanceWidget);
                                                        }
                                                        else if (mergedlst[x].WidgetName == HtmlConstants.SAVING_TRANSACTION_WIDGET_NAME)
                                                        {
                                                            string widgetId = "PageWidgetId_" + mergedlst[x].Identifier + "_Counter" + counter.ToString();
                                                            string widgetHTML = HtmlConstants.SAVING_TRANSACTION_WIDGET_HTML_FOR_STMT.Replace("{{AccountTransactionDetails}}", "{{AccountTransactionDetails_" + page.Identifier + "_" + mergedlst[x].Identifier + "}}").Replace("{{SelectOption}}", "{{SelectOption_" + page.Identifier + "_" + mergedlst[x].Identifier + "}}");
                                                            widgetHTML = widgetHTML.Replace("{{WidgetDivHeight}}", divHeight);
                                                            widgetHTML = widgetHTML.Replace("{{WidgetId}}", widgetId);
                                                            pageHtmlContent.Append(widgetHTML);
                                                        }
                                                        else if (mergedlst[x].WidgetName == HtmlConstants.CURRENT_TRANSACTION_WIDGET_NAME)
                                                        {
                                                            string widgetId = "PageWidgetId_" + mergedlst[x].Identifier + "_Counter" + counter.ToString();
                                                            string widgetHTML = HtmlConstants.CURRENT_TRANSACTION_WIDGET_HTML_FOR_STMT.Replace("{{AccountTransactionDetails}}", "{{AccountTransactionDetails_" + page.Identifier + "_" + mergedlst[x].Identifier + "}}").Replace("{{SelectOption}}", "{{SelectOption_" + page.Identifier + "_" + mergedlst[x].Identifier + "}}");
                                                            widgetHTML = widgetHTML.Replace("{{WidgetDivHeight}}", divHeight);
                                                            widgetHTML = widgetHTML.Replace("{{WidgetId}}", widgetId);
                                                            pageHtmlContent.Append(widgetHTML);
                                                        }
                                                        else if (mergedlst[x].WidgetName == HtmlConstants.TOP_4_INCOME_SOURCE_WIDGET_NAME)
                                                        {
                                                            string widgetId = "PageWidgetId_" + mergedlst[x].Identifier + "_Counter" + counter.ToString();
                                                            string widgetHTML = HtmlConstants.TOP_4_INCOME_SOURCE_WIDGET_HTML_FOR_STMT.Replace("{{IncomeSourceList}}", "{{IncomeSourceList_" + page.Identifier + "_" + mergedlst[x].Identifier + "}}");
                                                            widgetHTML = widgetHTML.Replace("{{WidgetDivHeight}}", divHeight);
                                                            widgetHTML = widgetHTML.Replace("{{WidgetId}}", widgetId);
                                                            pageHtmlContent.Append(widgetHTML);
                                                        }
                                                        else if (mergedlst[x].WidgetName == HtmlConstants.ANALYTICS_WIDGET_NAME)
                                                        {
                                                            string widgetId = "PageWidgetId_" + mergedlst[x].Identifier + "_Counter" + counter.ToString();
                                                            string widgetHTML = HtmlConstants.ANALYTIC_WIDGET_HTML_FOR_STMT;
                                                            widgetHTML = widgetHTML.Replace("{{WidgetDivHeight}}", divHeight);
                                                            widgetHTML = widgetHTML.Replace("{{WidgetId}}", widgetId);
                                                            pageHtmlContent.Append(widgetHTML);
                                                        }
                                                        else if (mergedlst[x].WidgetName == HtmlConstants.SPENDING_TREND_WIDGET_NAME)
                                                        {
                                                            string widgetId = "PageWidgetId_" + mergedlst[x].Identifier + "_Counter" + counter.ToString();
                                                            string widgetHTML = HtmlConstants.SPENDING_TRENDS_WIDGET_HTML_FOR_STMT;
                                                            widgetHTML = widgetHTML.Replace("{{WidgetDivHeight}}", divHeight);
                                                            widgetHTML = widgetHTML.Replace("{{WidgetId}}", widgetId);
                                                            pageHtmlContent.Append(widgetHTML);
                                                        }
                                                        else if (mergedlst[x].WidgetName == HtmlConstants.SAVING_TREND_WIDGET_NAME)
                                                        {
                                                            string widgetId = "PageWidgetId_" + mergedlst[x].Identifier + "_Counter" + counter.ToString();
                                                            string widgetHTML = HtmlConstants.SAVING_TRENDS_WIDGET_HTML_FOR_STMT;
                                                            widgetHTML = widgetHTML.Replace("{{WidgetDivHeight}}", divHeight);
                                                            widgetHTML = widgetHTML.Replace("{{WidgetId}}", widgetId);
                                                            pageHtmlContent.Append(widgetHTML);
                                                        }
                                                        else if (mergedlst[x].WidgetName == HtmlConstants.REMINDER_AND_RECOMMENDATION_WIDGET_NAME)
                                                        {
                                                            string widgetId = "PageWidgetId_" + mergedlst[x].Identifier + "_Counter" + counter.ToString();
                                                            string widgetHTML = HtmlConstants.REMINDER_WIDGET_HTML_FOR_STMT.Replace("{{ReminderAndRecommdationDataList}}", "{{ReminderAndRecommdationDataList_" + page.Identifier + "_" + mergedlst[x].Identifier + "}}");
                                                            widgetHTML = widgetHTML.Replace("{{WidgetDivHeight}}", divHeight);
                                                            widgetHTML = widgetHTML.Replace("{{WidgetId}}", widgetId);
                                                            pageHtmlContent.Append(widgetHTML);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (dynawidgets.Count > 0)
                                                        {
                                                            var dynawidget = dynawidgets.Where(item => item.Identifier == mergedlst[x].WidgetId).ToList().FirstOrDefault();

                                                            //get theme for current dynamic widget, 
                                                            //if it is default take theme setting from tenant configuration, otherwise from current widget theme setting
                                                            CustomeTheme themeDetails = new CustomeTheme();
                                                            if (dynawidget.ThemeType == "Default")
                                                            {
                                                                themeDetails = JsonConvert.DeserializeObject<CustomeTheme>(tenantConfiguration.WidgetThemeSetting);
                                                            }
                                                            else
                                                            {
                                                                themeDetails = JsonConvert.DeserializeObject<CustomeTheme>(dynawidget.ThemeCSS);
                                                            }

                                                            if (dynawidget.WidgetType == HtmlConstants.TABLE_DYNAMICWIDGET)
                                                            {
                                                                var htmlWidget = HtmlConstants.TABLE_WIDEGT_FOR_STMT;
                                                                htmlWidget = htmlWidget.Replace("{{WidgetDivHeight}}", divHeight);
                                                                htmlWidget = htmlWidget.Replace("{{TableMaxHeight}}", (mergedlst[x].Height * 110) - 40 + "px");
                                                                htmlWidget = this.ApplyStyleCssForDynamicTableAndFormWidget(htmlWidget, themeDetails);
                                                                htmlWidget = htmlWidget.Replace("{{WidgetTitle}}", dynawidget.Title);
                                                                htmlWidget = htmlWidget.Replace("{{WidgetId}}", "PageWidgetId_" + mergedlst[x].Identifier + "_Counter" + counter.ToString());
                                                                List<DynamicWidgetTableEntity> tableEntities = JsonConvert.DeserializeObject<List<DynamicWidgetTableEntity>>(dynawidget.WidgetSettings);
                                                                StringBuilder tableHeader = new StringBuilder();
                                                                tableHeader.Append("<tr>" + string.Join("", tableEntities.Select(field => string.Format("<th>{0}</th> ", field.HeaderName))) + "</tr>");
                                                                htmlWidget = htmlWidget.Replace("{{tableHeader}}", tableHeader.ToString());
                                                                htmlWidget = htmlWidget.Replace("{{tableBody}}", "{{tableBody_" + page.Identifier + "_" + mergedlst[x].Identifier + "}}");
                                                                pageHtmlContent.Append(htmlWidget);
                                                            }
                                                            else if (dynawidget.WidgetType == HtmlConstants.FORM_DYNAMICWIDGET)
                                                            {
                                                                var htmlWidget = HtmlConstants.FORM_WIDGET_FOR_STMT;
                                                                htmlWidget = htmlWidget.Replace("{{WidgetDivHeight}}", divHeight);
                                                                htmlWidget = this.ApplyStyleCssForDynamicTableAndFormWidget(htmlWidget, themeDetails);
                                                                htmlWidget = htmlWidget.Replace("{{WidgetTitle}}", dynawidget.Title);
                                                                htmlWidget = htmlWidget.Replace("{{WidgetId}}", "PageWidgetId_" + mergedlst[x].Identifier + "_Counter" + counter.ToString());
                                                                htmlWidget = htmlWidget.Replace("{{FormData}}", "{{FormData_" + page.Identifier + "_" + mergedlst[x].Identifier + "}}");
                                                                pageHtmlContent.Append(htmlWidget);
                                                            }
                                                            else if (dynawidget.WidgetType == HtmlConstants.LINEGRAPH_DYNAMICWIDGET)
                                                            {
                                                                var htmlWidget = HtmlConstants.LINE_GRAPH_FOR_STMT;
                                                                htmlWidget = htmlWidget.Replace("lineGraphcontainer", "lineGraphcontainer_" + page.Identifier + "_" + mergedlst[x].Identifier + "");
                                                                htmlWidget = htmlWidget.Replace("{{WidgetDivHeight}}", divHeight);
                                                                htmlWidget = this.ApplyStyleCssForDynamicGraphAndChartWidgets(htmlWidget, themeDetails);
                                                                htmlWidget = htmlWidget.Replace("{{WidgetTitle}}", dynawidget.Title);
                                                                htmlWidget = htmlWidget.Replace("{{WidgetId}}", "PageWidgetId_" + mergedlst[x].Identifier + "_Counter" + counter.ToString());
                                                                htmlWidget = htmlWidget + "<input type='hidden' id='hiddenLineGraphData_" + page.Identifier + "_" + mergedlst[x].Identifier + "' value='hiddenLineGraphValue_" + page.Identifier + "_" + mergedlst[x].Identifier + "'>";
                                                                pageHtmlContent.Append(htmlWidget);
                                                            }
                                                            else if (dynawidget.WidgetType == HtmlConstants.BARGRAPH_DYNAMICWIDGET)
                                                            {
                                                                var htmlWidget = HtmlConstants.BAR_GRAPH_FOR_STMT;
                                                                htmlWidget = htmlWidget.Replace("{{WidgetDivHeight}}", divHeight);
                                                                htmlWidget = htmlWidget.Replace("barGraphcontainer", "barGraphcontainer_" + page.Identifier + "_" + mergedlst[x].Identifier + "");
                                                                htmlWidget = this.ApplyStyleCssForDynamicGraphAndChartWidgets(htmlWidget, themeDetails);
                                                                htmlWidget = htmlWidget.Replace("{{WidgetTitle}}", dynawidget.Title);
                                                                htmlWidget = htmlWidget.Replace("{{WidgetId}}", "PageWidgetId_" + mergedlst[x].Identifier + "_Counter" + counter.ToString());
                                                                htmlWidget = htmlWidget + "<input type='hidden' id='hiddenBarGraphData_" + page.Identifier + "_" + mergedlst[x].Identifier + "' value='hiddenBarGraphValue_" + page.Identifier + "_" + mergedlst[x].Identifier + "'>";
                                                                pageHtmlContent.Append(htmlWidget);
                                                            }
                                                            else if (dynawidget.WidgetType == HtmlConstants.PICHART_DYNAMICWIDGET)
                                                            {
                                                                var htmlWidget = HtmlConstants.PIE_CHART_FOR_STMT;
                                                                htmlWidget = htmlWidget.Replace("{{WidgetDivHeight}}", divHeight);
                                                                htmlWidget = htmlWidget.Replace("pieChartcontainer", "pieChartcontainer_" + page.Identifier + "_" + mergedlst[x].Identifier + "");
                                                                htmlWidget = this.ApplyStyleCssForDynamicGraphAndChartWidgets(htmlWidget, themeDetails);
                                                                htmlWidget = htmlWidget.Replace("{{WidgetTitle}}", dynawidget.Title);
                                                                htmlWidget = htmlWidget.Replace("{{WidgetId}}", "PageWidgetId_" + mergedlst[x].Identifier + "_Counter" + counter.ToString());
                                                                htmlWidget = htmlWidget + "<input type='hidden' id='hiddenPieChartData_" + page.Identifier + "_" + mergedlst[x].Identifier + "' value='hiddenPieChartValue_" + page.Identifier + "_" + mergedlst[x].Identifier + "'>";
                                                                pageHtmlContent.Append(htmlWidget);
                                                            }
                                                            else if (dynawidget.WidgetType == HtmlConstants.HTML_DYNAMICWIDGET)
                                                            {
                                                                var htmlWidget = HtmlConstants.HTML_WIDGET_FOR_STMT;
                                                                htmlWidget = htmlWidget.Replace("{{WidgetDivHeight}}", divHeight);
                                                                htmlWidget = this.ApplyStyleCssForDynamicTableAndFormWidget(htmlWidget, themeDetails);
                                                                htmlWidget = htmlWidget.Replace("{{WidgetTitle}}", dynawidget.Title);
                                                                htmlWidget = htmlWidget.Replace("{{WidgetId}}", "PageWidgetId_" + mergedlst[x].Identifier + "_Counter" + counter.ToString());
                                                                htmlWidget = htmlWidget.Replace("{{FormData}}", "{{FormData_" + page.Identifier + "_" + mergedlst[x].Identifier + "}}");
                                                                pageHtmlContent.Append(htmlWidget);
                                                            }
                                                        }
                                                    }

                                                    counter++;
                                                    // To end current col-lg class div
                                                    pageHtmlContent.Append(HtmlConstants.END_DIV_TAG);

                                                    // if current col-lg class width is equal to 12 or end before complete col-lg-12 class, 
                                                    //then end parent row class div
                                                    if (tempRowWidth == 12 || (x == mergedlst.Count - 1))
                                                    {
                                                        tempRowWidth = 0;
                                                        pageHtmlContent.Append(HtmlConstants.END_DIV_TAG); //To end row class div
                                                        isRowComplete = true;
                                                    }
                                                }
                                                mergedlst.ForEach(it =>
                                                {
                                                    completelst.Remove(it);
                                                });
                                            }
                                            else
                                            {
                                                if (completelst.Count != 0)
                                                {
                                                    currentYPosition = completelst.Min(it => it.Yposition);
                                                }
                                            }
                                        }
                                        //If row class div end before complete col-lg-12 class
                                        if (isRowComplete == false)
                                        {
                                            pageHtmlContent.Append(HtmlConstants.END_DIV_TAG);
                                        }
                                    }
                                    else
                                    {
                                        pageHtmlContent.Append(HtmlConstants.NO_WIDGET_MESSAGE_HTML);
                                    }
                                    statementPageContent.PageFooterContent = HtmlConstants.WIDGET_HTML_FOOTER;
                                }
                            }
                            else
                            {
                                pageHtmlContent.Append(HtmlConstants.NO_WIDGET_MESSAGE_HTML);
                            }
                            statementPageContent.HtmlContent = pageHtmlContent.ToString();
                            statementPageContents.Add(statementPageContent);
                        }
                    }
                }

                return statementPageContents;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method help to bind preview dara to statement
        /// </summary>
        /// <param name="statement"> the statement object </param>
        /// <param name="statementPageContents"> the statement page html content list</param>
        /// <param name="baseURL"> the base URL of API </param>
        public string BindPreviewDataToStatement(Statement statement, IList<StatementPageContent> statementPageContents, string baseURL, string tenantCode)
        {
            try
            {
                var AppBaseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                var tenantConfiguration = this.tenantConfigurationRepository.GetTenantConfigurations(tenantCode)?.FirstOrDefault();
                //start to render common html content data
                StringBuilder htmlbody = new StringBuilder();
                string navbarHtml = HtmlConstants.NAVBAR_HTML.Replace("{{BrandLogo}}", "../common/images/logo.png");
                navbarHtml = navbarHtml.Replace("{{logo}}", "../common/images/nisLogo.png");
                navbarHtml = navbarHtml.Replace("{{Today}}", DateTime.Now.ToString("dd MMM yyyy"));
                htmlbody.Append(HtmlConstants.CONTAINER_DIV_HTML_HEADER);

                //start to render actual html content data
                StringBuilder scriptHtmlRenderer = new StringBuilder();
                StringBuilder navbar = new StringBuilder();
                var newStatementPageContents = new List<StatementPageContent>();
                statementPageContents.ToList().ForEach(it => newStatementPageContents.Add(new StatementPageContent()
                {
                    Id = it.Id,
                    PageId = it.PageId,
                    PageTypeId = it.PageTypeId,
                    HtmlContent = it.HtmlContent,
                    PageHeaderContent = it.PageHeaderContent,
                    PageFooterContent = it.PageFooterContent,
                    DisplayName = it.DisplayName,
                    TabClassName = it.TabClassName
                }));
                for (int i = 0; i < statement.Pages.Count; i++)
                {
                    var page = statement.Pages[i];
                    StatementPageContent statementPageContent = newStatementPageContents.Where(item => item.PageTypeId == page.PageTypeId && item.Id == i).FirstOrDefault();
                    StringBuilder pageContent = new StringBuilder(statementPageContent.HtmlContent);

                    StringBuilder SubTabs = new StringBuilder();
                    StringBuilder PageHeaderContent = new StringBuilder(statementPageContent.PageHeaderContent);
                    IList<string> linegraphIds = new List<string>();
                    IList<string> bargraphIds = new List<string>();
                    IList<string> piechartIds = new List<string>();

                    string tabClassName = Regex.Replace((statementPageContent.DisplayName + "-" + page.Version), @"\s+", "-");
                    navbar.Append(" <li class='nav-item'><a class='nav-link pt-1 mainNav " + (i == 0 ? "active" : "") + " " + tabClassName + "' href='javascript:void(0);' >" + statementPageContent.DisplayName + "</a> </li> ");
                    string ExtraClassName = i > 0 ? "d-none " + tabClassName : tabClassName;
                    PageHeaderContent.Replace("{{ExtraClass}}", ExtraClassName);
                    PageHeaderContent.Replace("{{DivId}}", tabClassName);

                    StringBuilder newPageContent = new StringBuilder();
                    newPageContent.Append(HtmlConstants.PAGE_TAB_CONTENT_HEADER);

                    if (page.PageTypeName == HtmlConstants.SAVING_ACCOUNT_PAGE || page.PageTypeName == HtmlConstants.CURRENT_ACCOUNT_PAGE)
                    {
                        SubTabs.Append("<ul class='nav nav-tabs' style='margin-top:-20px;'>");
                        SubTabs.Append("<li class='nav-item active'><a id='tab1-tab' data-toggle='tab' " + "data-target='#" + (page.PageTypeName == HtmlConstants.SAVING_ACCOUNT_PAGE ? "Saving" : "Current") + "-' role='tab' class='nav-link active'> Account - 6789</a></li>");
                        SubTabs.Append("</ul>");

                        newPageContent.Append("<div id='" + (page.PageTypeName == HtmlConstants.SAVING_ACCOUNT_PAGE ? "Saving" : "Current") + "-6789'>");
                    }

                    var pagewidgets = page.PageWidgets;
                    for (int j = 0; j < pagewidgets.Count; j++)
                    {
                        var widget = pagewidgets[j];
                        if (!widget.IsDynamicWidget)
                        {
                            if (widget.WidgetName == HtmlConstants.CUSTOMER_INFORMATION_WIDGET_NAME)
                            {
                                string customerInfoJson = "{'FirstName':'Laura','MiddleName':'J','LastName':'Donald','AddressLine1':" +
                                    "'4000 Executive Parkway','AddressLine2':'Saint Globin Rd','City':'Canary Wharf', 'State':'London', " +
                                    "'Country':'England','Zip':'E14 9RZ'}";
                                if (customerInfoJson != string.Empty && validationEngine.IsValidJson(customerInfoJson))
                                {
                                    CustomerInformation customerInfo = JsonConvert.DeserializeObject<CustomerInformation>(customerInfoJson);
                                    pageContent.Replace("{{VideoSource_" + statement.Identifier + "_" + page.Identifier + "_" + widget.Identifier + "}}", AppBaseDirectory + "\\Resources\\sampledata\\SampleVideo.mp4");

                                    string customerName = customerInfo.FirstName + " " + customerInfo.SurName;
                                    pageContent.Replace("{{CustomerName}}", customerName);

                                    string address1 = customerInfo.AddressLine1 + ", " + customerInfo.AddressLine2 + ", ";
                                    pageContent.Replace("{{Address1}}", address1);

                                    string address2 = customerInfo.AddressLine3 + ", " + customerInfo.AddressLine4 + ", ";
                                    pageContent.Replace("{{Address2}}", address2);
                                }
                            }

                            else if (widget.WidgetName == HtmlConstants.PAYMENT_SUMMARY_WIDGET_NAME)
                            {
                                string paymentInfoJson = "{Reg_ID : 1,Start_Date : '2023-01-01',End_Date : '2023-01-01',Request_DateTime : 'DummyText1',ID : '124529534',Intermediary_Code : 'DummyText1',FSP_ID : 'DummyText1',Policy_Number : 'DummyText1',FSP_Party_ID : 'DummyText1',Client_Number : '124556686',FSP_REF : '2452953',Client_Name : 'Mr SCHOELER',Int_ID : 'DummyText1',Product_Type : 'DummyText1',Commission_Amount : 'DummyText1',INT_EXT_REF : '124411745',Int_Name : 'Kruger Van Heerden',Int_Type : 'DummyText1',Policy_Ref : '5596100',Member_Ref : '124556686',Member_Name : 'DummyText1',Transaction_Amount : 'DummyText1',Mem_Age : 'DummyText1',Months_In_Force : 'DummyText1',Commission_Type : 'Safe Custody Fee',Description : 'Safe Custody Service Fee',POSTED_DATE : '2023-03-03',AE_Type_ID : 'DummyText1',AE_Amount : 'DummyText1',DR_CR : 'DummyText1',NAME : 'DummyText1',Member_Surname : 'DummyText1',Jurisdiction : 'DummyText1',Sales_Office : 'DummyText1',FSP_Name : 'Miss Yvonne van Heerden',FSP_Trading_Name : 'T/A Yvonne Van Heerden Financial Planner CC',FSP_Ext_Ref : '124529534',FSP_Kind : 'DummyText1',  		FSP_VAT_Number : '2452953',Product : 'DummyText1',Prod_Group : 'Service Fee',Prod_Seq : 'DummyText1',Report_Seq : 'DummyText1',TYPE : 'DummyText1',Display_Amount : '17.55',VAT_Amount : '38001.27',Earning_Amount : '256670.66',Payment_Amount : 'DummyText1',START_DATE : 'DummyText1',END_DATE : 'DummyText1',Business_Type : 'DummyText1',Lifecycle_Description : 'DummyText1',Lifecycle_Start_Date : 'DummyText1',AE_Scheduler_ID : 'DummyText1',VAT_Amount_1 : 'DummyText1',Final_Amount : 'DummyText1'}";
                                if (paymentInfoJson != string.Empty && validationEngine.IsValidJson(paymentInfoJson))
                                {
                                    spIAA_PaymentDetail paymentInfo = JsonConvert.DeserializeObject<spIAA_PaymentDetail>(paymentInfoJson);
                                    pageContent.Replace("{{IntTotal}}", paymentInfo.Earning_Amount);
                                    pageContent.Replace("{{Vat}}", paymentInfo.VAT_Amount);
                                    pageContent.Replace("{{TotalDue}}", (Convert.ToDouble(paymentInfo.Earning_Amount) + Convert.ToDouble(paymentInfo.VAT_Amount)).ToString());

                                    // Format the date to month-year format                                                   
                                    pageContent.Replace("{{IntTotalDate}}", paymentInfo.POSTED_DATE.ToString("MMMM yyyy"));
                                    // Format the date with a custom format
                                    string formattedOrdinalDate = FormatDateWithOrdinal(paymentInfo.POSTED_DATE);
                                    pageContent.Replace("{{IntPostedDate}}", formattedOrdinalDate);

                                }
                            }

                            else if (widget.WidgetName == HtmlConstants.PPS_HEADING_WIDGET_NAME)
                            {
                                string headingInfoJson = "{Reg_ID : 1,Start_Date : '2023-01-01',End_Date : '2023-01-01',Request_DateTime : 'DummyText1',ID : '124529534',Intermediary_Code : 'DummyText1',FSP_ID : 'DummyText1',Policy_Number : 'DummyText1',FSP_Party_ID : 'DummyText1',Client_Number : '124556686',FSP_REF : '2452953',Client_Name : 'Mr SCHOELER',Int_ID : 'DummyText1',Product_Type : 'DummyText1',Commission_Amount : 'DummyText1',INT_EXT_REF : '124411745',Int_Name : 'Kruger Van Heerden',Int_Type : 'DummyText1',Policy_Ref : '5596100',Member_Ref : '124556686',Member_Name : 'DummyText1',Transaction_Amount : 'DummyText1',Mem_Age : 'DummyText1',Months_In_Force : 'DummyText1',Commission_Type : 'Safe Custody Fee',Description : 'Safe Custody Service Fee',POSTED_DATE : '2023-03-03',AE_Type_ID : 'DummyText1',AE_Amount : 'DummyText1',DR_CR : 'DummyText1',NAME : 'DummyText1',Member_Surname : 'DummyText1',Jurisdiction : 'DummyText1',Sales_Office : 'DummyText1',FSP_Name : 'Miss Yvonne van Heerden',FSP_Trading_Name : 'T/A Yvonne Van Heerden Financial Planner CC',FSP_Ext_Ref : '124529534',FSP_Kind : 'DummyText1',  		FSP_VAT_Number : '2452953',Product : 'DummyText1',Prod_Group : 'Service Fee',Prod_Seq : 'DummyText1',Report_Seq : 'DummyText1',TYPE : 'DummyText1',Display_Amount : '17.55',VAT_Amount : '38001.27',Earning_Amount : '256670.66',Payment_Amount : 'DummyText1',START_DATE : 'DummyText1',END_DATE : 'DummyText1',Business_Type : 'DummyText1',Lifecycle_Description : 'DummyText1',Lifecycle_Start_Date : 'DummyText1',AE_Scheduler_ID : 'DummyText1',VAT_Amount_1 : 'DummyText1',Final_Amount : 'DummyText1'}";
                                if (headingInfoJson != string.Empty && validationEngine.IsValidJson(headingInfoJson))
                                {
                                    //AccountMaster paymentInfo = JsonConvert.DeserializeObject<AccountMaster>(headingInfoJson);

                                    spIAA_PaymentDetail headingInfo = JsonConvert.DeserializeObject<spIAA_PaymentDetail>(headingInfoJson);
                                    pageContent.Replace("{{FSPName}}", headingInfo.FSP_Name);
                                    pageContent.Replace("{{FSPTradingName}}", headingInfo.FSP_Trading_Name);
                                }
                            }

                            else if (widget.WidgetName == HtmlConstants.PPS_DETAILS_WIDGET_NAME)
                            {
                                string ppsDetailsInfoJson = "{Reg_ID : 1,Start_Date : '2023-01-01',End_Date : '2023-01-01',Request_DateTime : 'DummyText1',ID : '124529534',Intermediary_Code : 'DummyText1',FSP_ID : 'DummyText1',Policy_Number : 'DummyText1',FSP_Party_ID : 'DummyText1',Client_Number : '124556686',FSP_REF : '2452953',Client_Name : 'Mr SCHOELER',Int_ID : 'DummyText1',Product_Type : 'DummyText1',Commission_Amount : 'DummyText1',INT_EXT_REF : '124411745',Int_Name : 'Kruger Van Heerden',Int_Type : 'DummyText1',Policy_Ref : '5596100',Member_Ref : '124556686',Member_Name : 'DummyText1',Transaction_Amount : 'DummyText1',Mem_Age : 'DummyText1',Months_In_Force : 'DummyText1',Commission_Type : 'Safe Custody Fee',Description : 'Safe Custody Service Fee',POSTED_DATE : '2023-03-03',AE_Type_ID : 'DummyText1',AE_Amount : 'DummyText1',DR_CR : 'DummyText1',NAME : 'DummyText1',Member_Surname : 'DummyText1',Jurisdiction : 'DummyText1',Sales_Office : 'DummyText1',FSP_Name : 'Miss Yvonne van Heerden',FSP_Trading_Name : 'T/A Yvonne Van Heerden Financial Planner CC',FSP_Ext_Ref : '124529534',FSP_Kind : 'DummyText1',  		FSP_VAT_Number : '2452953',Product : 'DummyText1',Prod_Group : 'Service Fee',Prod_Seq : 'DummyText1',Report_Seq : 'DummyText1',TYPE : 'DummyText1',Display_Amount : '17.55',VAT_Amount : '38001.27',Earning_Amount : '256670.66',Payment_Amount : 'DummyText1',START_DATE : 'DummyText1',END_DATE : 'DummyText1',Business_Type : 'DummyText1',Lifecycle_Description : 'DummyText1',Lifecycle_Start_Date : 'DummyText1',AE_Scheduler_ID : 'DummyText1',VAT_Amount_1 : 'DummyText1',Final_Amount : 'DummyText1'}";
                                if (ppsDetailsInfoJson != string.Empty && validationEngine.IsValidJson(ppsDetailsInfoJson))
                                {

                                    spIAA_PaymentDetail ppsDetailsInfo = JsonConvert.DeserializeObject<spIAA_PaymentDetail>(ppsDetailsInfoJson);
                                    pageContent.Replace("{{FSPNumber}}", ppsDetailsInfo.FSP_Ext_Ref);
                                    pageContent.Replace("{{FSPAgreeNumber}}", ppsDetailsInfo.FSP_REF);
                                    pageContent.Replace("{{VATRegNumber}}", ppsDetailsInfo.FSP_VAT_Number);
                                }
                            }

                            else if (widget.WidgetName == HtmlConstants.PRODUCT_SUMMARY_WIDGET_NAME)
                            {
                                string productSummaryListJson = "[{ 'Commission_Type': 'Safe Custody Fee', 'Prod_Group':'Safe Custody Fee', 'Display_Amount': 'R52,65','QueryLink': 'https://facebook.com'},{ 'Commission_Type': 'Safe Custody Fee', 'Prod_Group':'Service Fee', 'Display_Amount': 'R52,66', 'QueryLink': 'https://facebook.com' }, { 'Commission_Type': 'Safe Custody Fee', 'Prod_Group':'Safe Custody Fee', 'Display_Amount': 'R52,67', 'QueryLink': 'https://facebook.com' }, { 'Commission_Type': 'Safe Custody Fee', 'Prod_Group':'Service Fee', 'Display_Amount': 'R52,68', 'QueryLink': 'https://facebook.com' } ]";

                                if (productSummaryListJson != string.Empty && validationEngine.IsValidJson(productSummaryListJson))
                                {
                                    IList<spIAA_PaymentDetail> productSummary = JsonConvert.DeserializeObject<List<spIAA_PaymentDetail>>(productSummaryListJson);
                                    StringBuilder productSummarySrc = new StringBuilder();
                                    long index = 1;
                                    productSummary.ToList().ForEach(item =>
                                    {
                                        productSummarySrc.Append("<tr><td align='center' valign='center' class='px-1 py-1 fsp-bdr-right fsp-bdr-bottom'>" + index + "</td><td class='fsp-bdr-right fsp-bdr-bottom px-1'>" + item.Commission_Type + "</td>" + "<td class='fsp-bdr-right fsp-bdr-bottom px-1'> " + (item.Prod_Group == "Service Fee" ? "Premium Under Advise Fee" : item.Prod_Group) + "</td> <td class='text-right fsp-bdr-right fsp-bdr-bottom px-1'>R" + item.Display_Amount + "</td><td class='text-center fsp-bdr-bottom px-1'><a  href ='https://facebook.com' target='_blank'><img class='leftarrowlogo' src ='assets/images/leftarrowlogo.png' alt = 'Left Arrow'></a></td></tr>");
                                        index++;
                                    });
                                    pageContent.Replace("{{ProductSummary}}", productSummarySrc.ToString());
                                    string productInfoJson = "{Earning_Amount : '256670,66',VAT_Amount : '38001,27'}";
                                    spIAA_PaymentDetail productInfo = JsonConvert.DeserializeObject<spIAA_PaymentDetail>(productInfoJson);
                                    pageContent.Replace("{{QueryBtn}}", "assets/images/IfQueryBtn.jpg");
                                    pageContent.Replace("{{TotalDue}}", "R" + (Convert.ToDouble(productInfo.Earning_Amount)).ToString());
                                    pageContent.Replace("{{VATDue}}", "R" + productInfo.VAT_Amount.ToString());
                                    double grandTotalDue = (Convert.ToDouble(productInfo.Earning_Amount) + Convert.ToDouble(productInfo.VAT_Amount));
                                    pageContent.Replace("{{GrandTotalDue}}", "R" + grandTotalDue.ToString());
                                    double ppsPayment = grandTotalDue;
                                    pageContent.Replace("{{PPSPayment}}", "-R" + (grandTotalDue).ToString());
                                    pageContent.Replace("{{Balance}}", "R" + Convert.ToDouble((grandTotalDue - ppsPayment)).ToString("F2"));
                                }
                            }
                            else if (widget.WidgetName == HtmlConstants.PPS_FOOTER1_WIDGET_NAME)
                            {
                                string ppsFooter1InfoJson = "{Reg_ID : 1,Start_Date : '2023-01-01',End_Date : '2023-01-01',Request_DateTime : 'DummyText1',ID : '124529534',Intermediary_Code : 'DummyText1',FSP_ID : 'DummyText1',Policy_Number : 'DummyText1',FSP_Party_ID : 'DummyText1',Client_Number : '124556686',FSP_REF : '2452953',Client_Name : 'Mr SCHOELER',Int_ID : 'DummyText1',Product_Type : 'DummyText1',Commission_Amount : 'DummyText1',INT_EXT_REF : '124411745',Int_Name : 'Kruger Van Heerden',Int_Type : 'DummyText1',Policy_Ref : '5596100',Member_Ref : '124556686',Member_Name : 'DummyText1',Transaction_Amount : 'DummyText1',Mem_Age : 'DummyText1',Months_In_Force : 'DummyText1',Commission_Type : 'Safe Custody Fee',Description : 'Safe Custody Service Fee',POSTED_DATE : '2023-03-03',AE_Type_ID : 'DummyText1',AE_Amount : 'DummyText1',DR_CR : 'DummyText1',NAME : 'DummyText1',Member_Surname : 'DummyText1',Jurisdiction : 'DummyText1',Sales_Office : 'DummyText1',FSP_Name : 'Miss Yvonne van Heerden',FSP_Trading_Name : 'T/A Yvonne Van Heerden Financial Planner CC',FSP_Ext_Ref : '124529534',FSP_Kind : 'DummyText1',  		FSP_VAT_Number : '2452953',Product : 'DummyText1',Prod_Group : 'Service Fee',Prod_Seq : 'DummyText1',Report_Seq : 'DummyText1',TYPE : 'DummyText1',Display_Amount : '17.55',VAT_Amount : '38001.27',Earning_Amount : '256670.66',Payment_Amount : 'DummyText1',START_DATE : 'DummyText1',END_DATE : 'DummyText1',Business_Type : 'DummyText1',Lifecycle_Description : 'DummyText1',Lifecycle_Start_Date : 'DummyText1',AE_Scheduler_ID : 'DummyText1',VAT_Amount_1 : 'DummyText1',Final_Amount : 'DummyText1'}";
                                if (ppsFooter1InfoJson != string.Empty && validationEngine.IsValidJson(ppsFooter1InfoJson))
                                {
                                    string middleText = "PPS Insurance is a registered Insurer and FSP";
                                    string pageText = "Page 1/2";
                                    spIAA_PaymentDetail ppsFooter1Info = JsonConvert.DeserializeObject<spIAA_PaymentDetail>(ppsFooter1InfoJson);
                                    pageContent.Replace("{{FSPFooterDetails}}", middleText);
                                    pageContent.Replace("{{FSPPage}}", pageText);

                                }
                            }

                            else if (widget.WidgetName == HtmlConstants.FOOTER_IMAGE_WIDGET_NAME)
                            {
                                string footerImageInfoJson = "{Reg_ID : 1,Start_Date : '2023-01-01',End_Date : '2023-01-01',Request_DateTime : 'DummyText1',ID : '124529534',Intermediary_Code : 'DummyText1',FSP_ID : 'DummyText1',Policy_Number : 'DummyText1',FSP_Party_ID : 'DummyText1',Client_Number : '124556686',FSP_REF : '2452953',Client_Name : 'Mr SCHOELER',Int_ID : 'DummyText1',Product_Type : 'DummyText1',Commission_Amount : 'DummyText1',INT_EXT_REF : '124411745',Int_Name : 'Kruger Van Heerden',Int_Type : 'DummyText1',Policy_Ref : '5596100',Member_Ref : '124556686',Member_Name : 'DummyText1',Transaction_Amount : 'DummyText1',Mem_Age : 'DummyText1',Months_In_Force : 'DummyText1',Commission_Type : 'Safe Custody Fee',Description : 'Safe Custody Service Fee',POSTED_DATE : '2023-03-03',AE_Type_ID : 'DummyText1',AE_Amount : 'DummyText1',DR_CR : 'DummyText1',NAME : 'DummyText1',Member_Surname : 'DummyText1',Jurisdiction : 'DummyText1',Sales_Office : 'DummyText1',FSP_Name : 'Miss Yvonne van Heerden',FSP_Trading_Name : 'T/A Yvonne Van Heerden Financial Planner CC',FSP_Ext_Ref : '124529534',FSP_Kind : 'DummyText1',  		FSP_VAT_Number : '2452953',Product : 'DummyText1',Prod_Group : 'Service Fee',Prod_Seq : 'DummyText1',Report_Seq : 'DummyText1',TYPE : 'DummyText1',Display_Amount : '17.55',VAT_Amount : '38001.27',Earning_Amount : '256670.66',Payment_Amount : 'DummyText1',START_DATE : 'DummyText1',END_DATE : 'DummyText1',Business_Type : 'DummyText1',Lifecycle_Description : 'DummyText1',Lifecycle_Start_Date : 'DummyText1',AE_Scheduler_ID : 'DummyText1',VAT_Amount_1 : 'DummyText1',Final_Amount : 'DummyText1'}";
                                if (footerImageInfoJson != string.Empty && validationEngine.IsValidJson(footerImageInfoJson))
                                {
                                    //string middleText = "PPS Insurance is a registered Insurer and FSP";
                                    //string pageText = "Page 1/2";
                                    spIAA_PaymentDetail ppsFooter1Info = JsonConvert.DeserializeObject<spIAA_PaymentDetail>(footerImageInfoJson);
                                    //pageContent.Replace("{{FSPFooterDetails}}", middleText);
                                    //pageContent.Replace("{{FSPPage}}", pageText);

                                }
                            }


                            else if (widget.WidgetName == HtmlConstants.PPS_DETAILS1_WIDGET_NAME)
                            {
                                string ppsDetails1InfoJson = "{'Request_ID':1,'AE_TYPE_ID':'20','INT_EXT_REF':'124529534','POLICY_REF':'October','MEMBER_REF':'Payment Details',    'Member_Name':'DummyText1','BUS_GROUP':'SERVICE FEES','PRODUCT_DESCRIPTION':'Professional Health Provider Whole Life Professional Health',    'OID':'DummyText1','MeasureType':'Commission','CommissionType':'2nd Year','TRANSACTION_AMOUNT':65566.20,    'ALLOCATED_AMOUNT':65566.20,'MEMBER_AGE':'DummyText1','MONTHS_IN_FORCE':'DummyText1','REQUEST_DATETIME':'2023-01-01',    'REQUESTED_DATETIME':'2023-09-01','AE_agmt_id':'DummyText1','AE_agmt_type_id':'5596100','AE_Posted_Date':'2023-09-01','AE_Amount':'65566.20','Acc_Name':'DummyText1','FSP_Name':'Miss HW HLONGWANE','DUE_DATE':'2023-09-01','YEAR_START_DATE':'2023-01-01','YEAR_END_DATE':'2023-09-01','Type':'DummyText1',    'Req_Year':'2023-01-01','FutureEndDate':'2023-01-01','Calc1stYear':10000,'Calc2ndYear':20000,'MonthRange':'DummyText1',    'calcMain2ndYear':30000 }";
                                if (ppsDetails1InfoJson != string.Empty && validationEngine.IsValidJson(ppsDetails1InfoJson))
                                {
                                    DateTime DateFrom = new DateTime(2023, 01, 01);
                                    DateTime DateTo = new DateTime(2023, 09, 01);
                                    spIAA_Commission_Detail ppsDetails1Info = JsonConvert.DeserializeObject<spIAA_Commission_Detail>(ppsDetails1InfoJson);
                                    pageContent.Replace("{{ref}}", ppsDetails1Info.INT_EXT_REF);
                                    pageContent.Replace("{{mtype}}", ppsDetails1Info.MeasureType);
                                    pageContent.Replace("{{month}}", DateFrom.ToString("MMMM yyyy"));
                                    pageContent.Replace("{{paramDate}}", DateFrom.ToString("yyyy-MM-dd") + " To " + DateTo.ToString("yyyy-MM-dd"));

                                }
                            }

                            else if (widget.WidgetName == HtmlConstants.DETAILED_TRANSACTIONS_WIDGET_NAME)
                            {
                               string transactionListJson = "[{'INT_EXT_REF':'2164250','Int_Name':'Mr SCHOELER','Client_Name':'Kruger Van Heerden','Member_Ref':124556686,'Policy_Ref':5596100,'Description':'Safe Custody Service Fee','Commission_Type':'Safe Custody Fee','POSTED_DATE':'20-Mar-23','Display_Amount':'17.55','Query_Link':'https://facebook.com','TYPE':'Fiduciary_Data','Prod_Group':'Safe Custody Fee'},{'INT_EXT_REF':'2164250','Int_Name':'Yvonne Van Heerden','Client_Name':'Mr SCHOELER','Member_Ref':124556686,'Policy_Ref':'5596100','Description':'Safe Custody Service Fee VAT','Commission_Type':'Safe Custody Fee','POSTED_DATE':'20-Mar-23','Display_Amount':'2.63','Query_Link':'https://facebook.com','TYPE':'Fiduciary_Data','Prod_Group':'Safe Custody Fee'},{'INT_EXT_REF':'124411745','Int_Name':'Kruger Van Heerden','Client_Name':'DR N J Olivier','Member_Ref':'1217181','Policy_Ref':'5524069','Description':'Safe Custody Service Fee','Commission_Type':'Safe Custody Fee','POSTED_DATE':'20-Mar-23','Display_Amount':'17.55','Query_Link':'https://facebook.com','TYPE':'Fiduciary_Data','Prod_Group':'Safe Custody Fee'},{'INT_EXT_REF':'124411745','Int_Name':'Kruger Van Heerden','Client_Name':'DR N J Olivier','Member_Ref':'124556686','Policy_Ref':'5596100','Description':'Safe Custody Service Fee VAT','Commission_Type':'Safe Custody Fee','POSTED_DATE':'20-Mar-23','Display_Amount':'2.63','Query_Link':'https://facebook.com','TYPE':'Fiduciary_Data','Prod_Group':'VAT'}]";
                                double TotalPostedAmount = 0;
                                if (transactionListJson != string.Empty && validationEngine.IsValidJson(transactionListJson))
                                {
                                    IList<spIAA_PaymentDetail> transaction = JsonConvert.DeserializeObject<List<spIAA_PaymentDetail>>(transactionListJson);
                                    StringBuilder detailedTransactionSrc = new StringBuilder();
                                    var records = transaction.GroupBy(gptransactionitem => gptransactionitem.INT_EXT_REF).ToList();
                                    records?.ForEach(transactionitem =>
                                    {
                                        detailedTransactionSrc.Append("<div class='px-50'><div class='prouct-table-block'><div class='text-left fsp-transaction-title font-weight-bold mb-3'>Intermediary:  " + transactionitem.FirstOrDefault().INT_EXT_REF + " " + transactionitem.FirstOrDefault().Int_Name + "</div><table width='100%' cellpadding='0' cellspacing='0'> <tr><th class='font-weight-bold text-white'>Client name</th> <th class='font-weight-bold text-white text-center pe-0 bdr-r-0'>Member<br /> number</th> <th class='font-weight-bold text-white text-center'>Will<br/> number</th> <th class='font-weight-bold text-white text-center'>Fiduciary fees</th> <th class='font-weight-bold text-white text-center'>Commission<br /> type</th> <th class='font-weight-bold text-white text-center'>Posted date</th> <th class='font-weight-bold text-white text-center'>Posted amount</th> <th class='font-weight-bold text-white'>Query</th> </tr> ");
                                        pageContent.Replace("{{QueryBtnImgLink}}", "https://www.google.com/");
                                        pageContent.Replace("{{QueryBtn}}", "../common/images/IfQueryBtn.jpg");
                                        transaction.Where(witem => witem.INT_EXT_REF == transactionitem.FirstOrDefault().INT_EXT_REF).ToList().ForEach(item =>
                                        {
                                            detailedTransactionSrc.Append("<tr><td align = 'center' valign = 'center' class='px-1 py-1 fsp-bdr-right fsp-bdr-bottom'>" +
                                                    item.Client_Name + "</td><td class= 'fsp-bdr-right fsp-bdr-bottom px-1'>" + item.Member_Ref + "</td><td class= 'fsp-bdr-right fsp-bdr-bottom px-1'> " + item.Policy_Ref + "</td><td class= 'text-right fsp-bdr-right fsp-bdr-bottom px-1'>" + (item.Description == "Commission Service Fee" ? "Premium Under Advise Fee" : item.Description)  + "</td><td class= 'text-center fsp-bdr-right fsp-bdr-bottom px-1'>" + item.Commission_Type + "</td><td class= 'text-center fsp-bdr-right fsp-bdr-bottom px-1'>" + item.POSTED_DATE.ToString("dd-MMM-yyyy") + "</td><td class= 'text-center fsp-bdr-right fsp-bdr-bottom px-1'> R" + item.Display_Amount + "</td><td class= 'text-center fsp-bdr-bottom px-1'><a href ='https://www.google.com/' target ='_blank'><img class='leftarrowlogo' src='assets/images/leftarrowlogo.png' alt='Left Arrow'></a></td></tr>");
                                            TotalPostedAmount += ((item.TYPE == "Fiduciary_Data") && (item.Prod_Group != "VAT"))?  (Convert.ToDouble(item.Display_Amount)): 0.0;
                                        });
                                        string TotalPostedAmountR = (TotalPostedAmount == 0) ? "0.00" : ("R" + TotalPostedAmount.ToString());
                                        detailedTransactionSrc.Append("<tr> <td align='center' valign='center' class='px-1 py-1 fsp-bdr-right fsp-bdr-bottom'></td> <td class='fsp-bdr-right fsp-bdr-bottom px-1 py-1'></td> <td class='fsp-bdr-right fsp-bdr-bottom px-1 py-1'></td> <td class='text-right fsp-bdr-right fsp-bdr-bottom px-1 py-1'></td> <td class='text-center fsp-bdr-right fsp-bdr-bottom px-1 py-1'><br /></td> <td class='text-center fsp-bdr-right fsp-bdr-bottom px-1 py-1'></td> <td class='text-center fsp-bdr-right fsp-bdr-bottom px-1 py-1'>" + TotalPostedAmount + "</td> <td class='text-center fsp-bdr-bottom px-1'><a href='https://www.google.com/' target = '_blank' ><img src='assets/images/leftarrowlogo.png'></a></td> </tr></table><div class='text-right w-100 pt-3'><a href='https://www.google.com/' target = '_blank'><img src='assets/images/click-print-stmt-btn.jpg'></a></div></div></div></div>");
                                        TotalPostedAmount = 0;
                                    });
                                    pageContent.Replace("{{detailedTransaction}}", detailedTransactionSrc.ToString());
                                }
                            }

                            else if (widget.WidgetName == HtmlConstants.ACCOUNT_INFORMATION_WIDGET_NAME)
                            {
                                string accountInfoJson = "{'StatementDate':'1-APR-2020','StatementPeriod':'Annual Statement', " +
                                    "'CustomerID':'ID2-8989-5656','RmName':'James Wiilims','RmContactNumber':'+4487867833'}";

                                string accountInfoData = string.Empty;
                                StringBuilder AccDivData = new StringBuilder();
                                if (accountInfoJson != string.Empty && validationEngine.IsValidJson(accountInfoJson))
                                {
                                    AccountInformation accountInfo = JsonConvert.DeserializeObject<AccountInformation>(accountInfoJson);
                                    AccDivData.Append("<div class='list-row-small ht70px'><div class='list-middle-row'> <div class='list-text'>" +
                                        "Statement Date</div><label class='list-value mb-0'>" + accountInfo.StatementDate + "</label>" + "</div></div>");

                                    AccDivData.Append("<div class='list-row-small ht70px'><div class='list-middle-row'> <div class='list-text'>" +
                                        "Statement Period</div><label class='list-value mb-0'>" + accountInfo.StatementPeriod + "</label></div></div>");

                                    AccDivData.Append("<div class='list-row-small ht70px'><div class='list-middle-row'> <div class='list-text'>" +
                                        "Cusomer ID</div><label class='list-value mb-0'>" + accountInfo.CustomerID + "</label></div></div>");

                                    AccDivData.Append("<div class='list-row-small ht70px'><div class='list-middle-row'> <div class='list-text'>" +
                                        "RM Name</div><label class='list-value mb-0'>" + accountInfo.RmName + "</label></div></div>");

                                    AccDivData.Append("<div class='list-row-small ht70px'><div class='list-middle-row'> <div class='list-text'>" +
                                        "RM Contact Number</div><label class='list-value mb-0'>" + accountInfo.RmContactNumber + "</label></div></div>");
                                }
                                pageContent.Replace("{{AccountInfoData_" + page.Identifier + "_" + widget.Identifier + "}}", AccDivData.ToString());
                            }
                            else if (widget.WidgetName == HtmlConstants.IMAGE_WIDGET_NAME)
                            {
                                var imgAssetFilepath = AppBaseDirectory + "\\Resources\\sampledata\\icon-image.png";
                                if (widget.WidgetSetting != string.Empty && validationEngine.IsValidJson(widget.WidgetSetting))
                                {
                                    dynamic widgetSetting = JObject.Parse(widget.WidgetSetting);
                                    if (widgetSetting.isPersonalize == false)
                                    {
                                        var asset = assetLibraryRepository.GetAssets(new AssetSearchParameter { Identifier = widgetSetting.AssetId, SortParameter = new SortParameter { SortColumn = "Id" } }, tenantCode).ToList()?.FirstOrDefault();
                                        imgAssetFilepath = asset.FilePath; //baseURL + "/assets/" + widgetSetting.AssetLibraryId + "/" + widgetSetting.AssetName;
                                    }
                                }
                                pageContent.Replace("{{ImageSource_" + statement.Identifier + "_" + page.Identifier + "_" + widget.Identifier + "}}", imgAssetFilepath);
                            }
                            else if (widget.WidgetName == HtmlConstants.VIDEO_WIDGET_NAME)
                            {
                                var vdoAssetFilepath = AppBaseDirectory + "\\Resources\\sampledata\\SampleVideo.mp4";
                                if (widget.WidgetSetting != string.Empty && validationEngine.IsValidJson(widget.WidgetSetting))
                                {
                                    dynamic widgetSetting = JObject.Parse(widget.WidgetSetting);
                                    if (widgetSetting.isEmbedded == true)
                                    {
                                        vdoAssetFilepath = widgetSetting.SourceUrl;
                                    }
                                    else if (widgetSetting.isPersonalize == false && widgetSetting.isEmbedded == false)
                                    {
                                        var asset = assetLibraryRepository.GetAssets(new AssetSearchParameter { Identifier = widgetSetting.AssetId, SortParameter = new SortParameter { SortColumn = "Id" } }, tenantCode).ToList()?.FirstOrDefault();
                                        vdoAssetFilepath = asset.FilePath; //baseURL + "/assets/" + widgetSetting.AssetLibraryId + "/" + widgetSetting.AssetName;
                                    }
                                }
                                pageContent.Replace("{{VideoSource_" + statement.Identifier + "_" + page.Identifier + "_" + widget.Identifier + "}}", vdoAssetFilepath);
                            }
                            else if (widget.WidgetName == HtmlConstants.SUMMARY_AT_GLANCE_WIDGET_NAME)
                            {
                                string accountBalanceDataJson = "[{\"AccountType\":\"Saving Account\",\"Currency\":\"$\",\"Amount\":\"87356\"}" +
                                    ",{\"AccountType\":\"Current Account\",\"Currency\":\"$\",\"Amount\":\"18654\"},{\"AccountType\":" +
                                    "\"Recurring Account\",\"Currency\":\"$\",\"Amount\":\"54367\"},{\"AccountType\":\"Wealth\",\"Currency\"" + ":\"$\",\"Amount\":\"4589\"}]";

                                string accountSummary = string.Empty;
                                if (accountBalanceDataJson != string.Empty && validationEngine.IsValidJson(accountBalanceDataJson))
                                {
                                    IList<AccountSummary> lstAccountSummary = JsonConvert.DeserializeObject<List
                                        <AccountSummary>>(accountBalanceDataJson);
                                    if (lstAccountSummary.Count > 0)
                                    {
                                        StringBuilder accSummary = new StringBuilder();
                                        lstAccountSummary.ToList().ForEach(acc =>
                                        {
                                            accSummary.Append("<tr><td>" + acc.AccountType + "</td><td>" + acc.Currency + "</td><td>" + acc.Amount + "</td></tr>");
                                        });
                                        pageContent.Replace("{{AccountSummary_" + page.Identifier + "_" + widget.Identifier + "}}", accSummary.ToString());
                                    }
                                }
                            }
                            else if (widget.WidgetName == HtmlConstants.CURRENT_AVAILABLE_BALANCE_WIDGET_NAME)
                            {
                                string currentAvailBalanceJson = "{'GrandTotal':'32,453,23', 'TotalDeposit':'16,250,00', 'TotalSpend':'16,254,00', 'ProfitEarned':'1,430,00 ', 'Currency':'$', 'Balance': '14,768,80', 'AccountNumber': 'J566565TR678ER', 'AccountType': 'Current', 'Indicator': 'Up'}";
                                if (currentAvailBalanceJson != string.Empty && validationEngine.IsValidJson(currentAvailBalanceJson))
                                {
                                    AccountMaster accountMaster = JsonConvert.DeserializeObject<AccountMaster>(currentAvailBalanceJson);
                                    var accountIndicatorClass = accountMaster.Indicator.ToLower().Equals("up") ? "fa fa-sort-asc text-success" : "fa fa-sort-desc text-danger";
                                    pageContent.Replace("{{AccountIndicatorClass}}", accountIndicatorClass);
                                    pageContent.Replace("{{TotalValue_" + page.Identifier + "_" + widget.Identifier + "}}", (accountMaster.Currency + accountMaster.GrandTotal));
                                    pageContent.Replace("{{TotalDeposit_" + page.Identifier + "_" + widget.Identifier + "}}", (accountMaster.Currency + accountMaster.TotalDeposit));
                                    pageContent.Replace("{{TotalSpend_" + page.Identifier + "_" + widget.Identifier + "}}", (accountMaster.Currency + accountMaster.TotalSpend));
                                    pageContent.Replace("{{Savings_" + page.Identifier + "_" + widget.Identifier + "}}", (accountMaster.Currency + accountMaster.ProfitEarned));
                                }
                            }
                            else if (widget.WidgetName == HtmlConstants.SAVING_AVAILABLE_BALANCE_WIDGET_NAME)
                            {
                                string savingAvailBalanceJson = "{'GrandTotal':'26,453,23', 'TotalDeposit':'13,530,00', 'TotalSpend':'12,124,00', 'ProfitEarned':'2,340,00 ', 'Currency':'$', 'Balance': '19,456,80', 'AccountNumber': 'J566565TR678ER', 'AccountType': 'Saving', 'Indicator': 'Up'}";
                                if (savingAvailBalanceJson != string.Empty && validationEngine.IsValidJson(savingAvailBalanceJson))
                                {
                                    AccountMaster accountMaster = JsonConvert.DeserializeObject<AccountMaster>(savingAvailBalanceJson);
                                    var accountIndicatorClass = accountMaster.Indicator.ToLower().Equals("up") ? "fa fa-sort-asc text-success" : "fa fa-sort-desc text-danger";
                                    pageContent.Replace("{{AccountIndicatorClass}}", accountIndicatorClass);
                                    pageContent.Replace("{{TotalValue_" + page.Identifier + "_" + widget.Identifier + "}}", (accountMaster.Currency + accountMaster.GrandTotal));
                                    pageContent.Replace("{{TotalDeposit_" + page.Identifier + "_" + widget.Identifier + "}}", (accountMaster.Currency + accountMaster.TotalDeposit));
                                    pageContent.Replace("{{TotalSpend_" + page.Identifier + "_" + widget.Identifier + "}}", (accountMaster.Currency + accountMaster.TotalSpend));
                                    pageContent.Replace("{{Savings_" + page.Identifier + "_" + widget.Identifier + "}}", (accountMaster.Currency + accountMaster.ProfitEarned));
                                }
                            }
                            else if (widget.WidgetName == HtmlConstants.SAVING_TRANSACTION_WIDGET_NAME)
                            {
                                StringBuilder selectOption = new StringBuilder();
                                var distinctNaration = new string[] { "NXT TXN: IIFL IIFL6574562", "NXT TXN: IIFL IIFL6574563", "NXT TXN: IIFL IIFL3557346", "NXT TXN: IIFL RTED87978947 REFUND", "NXT TXN: IIFL IIFL896452896ERE", "NXT TXN: IIFL IIFL8965435", "NXT TXN: IIFL FGTR454565JHGKD", "NXT TXN: OFFICE RENT 798789DFGH", "NXT TXN: IIFL IIFL0034212", "NXT TXN: IIFL IIFL045678DFGH" };
                                distinctNaration.ToList().ForEach(item =>
                                {
                                    selectOption.Append("<option value='" + item + "'> " + item + "</option>");
                                });

                                scriptHtmlRenderer.Append("<script type='text/javascript' src='" + AppBaseDirectory + "\\Resources\\sampledata\\savingtransactiondetail.json'></script>");
                                StringBuilder scriptval = new StringBuilder(HtmlConstants.SAVING_TRANSACTION_DETAIL_GRID_WIDGET_SCRIPT);
                                scriptval.Replace("SavingTransactionTable", "SavingTransactionTable" + page.Identifier);
                                scriptval.Replace("savingShowAll", "savingShowAll" + page.Identifier);
                                scriptval.Replace("filterStatus", "filterStatus" + page.Identifier);
                                scriptval.Replace("ResetGrid", "ResetGrid" + page.Identifier);
                                scriptval.Replace("PrintGrid", "PrintGrid" + page.Identifier);
                                scriptHtmlRenderer.Append(scriptval);

                                pageContent.Replace("savingShowAll", "savingShowAll" + page.Identifier);
                                pageContent.Replace("filterStatus", "filterStatus" + page.Identifier);
                                pageContent.Replace("ResetGrid", "ResetGrid" + page.Identifier);
                                pageContent.Replace("PrintGrid", "PrintGrid" + page.Identifier);
                                pageContent.Replace("{{SelectOption_" + page.Identifier + "_" + widget.Identifier + "}}", selectOption.ToString());
                                pageContent.Replace("SavingTransactionTable", "SavingTransactionTable" + page.Identifier);
                                pageContent.Replace("{{AccountTransactionDetails_" + page.Identifier + "_" + widget.Identifier + "}}", string.Empty);
                            }
                            else if (widget.WidgetName == HtmlConstants.CURRENT_TRANSACTION_WIDGET_NAME)
                            {
                                StringBuilder selectOption = new StringBuilder();
                                var distinctNaration = new string[] { "NXT TXN: IIFL IIFL6574562", "NXT TXN: IIFL IIFL6574563", "NXT TXN: IIFL IIFL3557346", "NXT TXN: IIFL RTED87978947 REFUND", "NXT TXN: IIFL IIFL896452896ERE", "NXT TXN: IIFL IIFL8965435", "NXT TXN: IIFL FGTR454565JHGKD", "NXT TXN: OFFICE RENT 798789DFGH", "NXT TXN: IIFL IIFL0034212", "NXT TXN: IIFL IIFL045678DFGH" };
                                distinctNaration.ToList().ForEach(item =>
                                {
                                    selectOption.Append("<option value='" + item + "'> " + item + "</option>");
                                });

                                scriptHtmlRenderer.Append("<script type='text/javascript' src='" + AppBaseDirectory + "\\Resources\\sampledata\\currenttransactiondetail.json'></script>");
                                StringBuilder scriptval = new StringBuilder(HtmlConstants.CURRENT_TRANSACTION_DETAIL_GRID_WIDGET_SCRIPT);
                                scriptval.Replace("CurrentTransactionTable", "CurrentTransactionTable" + page.Identifier);
                                scriptval.Replace("currentShowAll", "currentShowAll" + page.Identifier);
                                scriptval.Replace("filterStatus", "filterStatus" + page.Identifier);
                                scriptval.Replace("ResetGrid", "ResetGrid" + page.Identifier);
                                scriptval.Replace("PrintGrid", "PrintGrid" + page.Identifier);
                                scriptHtmlRenderer.Append(scriptval);

                                pageContent.Replace("currentShowAll", "currentShowAll" + page.Identifier);
                                pageContent.Replace("filterStatus", "filterStatus" + page.Identifier);
                                pageContent.Replace("ResetGrid", "ResetGrid" + page.Identifier);
                                pageContent.Replace("PrintGrid", "PrintGrid" + page.Identifier);
                                pageContent.Replace("{{SelectOption_" + page.Identifier + "_" + widget.Identifier + "}}", selectOption.ToString());
                                pageContent.Replace("CurrentTransactionTable", "CurrentTransactionTable" + page.Identifier);

                                pageContent.Replace("{{AccountTransactionDetails_" + page.Identifier + "_" + widget.Identifier + "}}", string.Empty);
                            }
                            else if (widget.WidgetName == HtmlConstants.TOP_4_INCOME_SOURCE_WIDGET_NAME)
                            {
                                string incomeSourceListJson = "[{ 'Source': 'Salary Transfer', 'CurrentSpend': 3453, 'AverageSpend': 123},{ 'Source': 'Cash Deposit', 'CurrentSpend': 3453, 'AverageSpend': 6123},{ 'Source': 'Profit Earned', 'CurrentSpend': 3453, 'AverageSpend': 6123}, { 'Source': 'Rebete', 'CurrentSpend': 3453, 'AverageSpend': 123}]";
                                if (incomeSourceListJson != string.Empty && validationEngine.IsValidJson(incomeSourceListJson))
                                {
                                    IList<IncomeSources> incomeSources = JsonConvert.DeserializeObject<List<IncomeSources>>(incomeSourceListJson);
                                    StringBuilder incomestr = new StringBuilder();
                                    incomeSources.ToList().ForEach(item =>
                                    {
                                        var tdstring = string.Empty;
                                        if (Int32.Parse(item.CurrentSpend) > Int32.Parse(item.AverageSpend))
                                        {
                                            tdstring = "<span class='fa fa-sort-desc fa-2x text-danger' aria-hidden='true'></span><span class='ml-2'>" + item.AverageSpend + "</span>";
                                        }
                                        else
                                        {
                                            tdstring = "<span class='fa fa-sort-asc fa-2x mt-1' aria-hidden='true' " + "style='position:relative;top:6px;color:limegreen'></span><span class='ml-2'>" + item.AverageSpend + "</span>";
                                        }
                                        incomestr.Append("<tr><td class='float-left'>" + item.Source + "</td>" + "<td> " + item.CurrentSpend + "</td><td>" + tdstring + "</td></tr>");
                                    });
                                    pageContent.Replace("{{IncomeSourceList_" + page.Identifier + "_" + widget.Identifier + "}}", incomestr.ToString());
                                }
                            }
                            else if (widget.WidgetName == HtmlConstants.ANALYTICS_WIDGET_NAME)
                            {
                                scriptHtmlRenderer.Append("<script type='text/javascript' src='" + AppBaseDirectory + "\\Resources\\sampledata\\analyticschartdata.json'></script>");
                                pageContent.Replace("analyticschartcontainer", "analyticschartcontainer" + page.Identifier);
                                scriptHtmlRenderer.Append(HtmlConstants.ANALYTICS_CHART_WIDGET_SCRIPT.Replace("analyticschartcontainer", "analyticschartcontainer" + page.Identifier));
                            }
                            else if (widget.WidgetName == HtmlConstants.SPENDING_TREND_WIDGET_NAME)
                            {
                                scriptHtmlRenderer.Append("<script type='text/javascript' src='" + AppBaseDirectory + "\\Resources\\sampledata\\savingtrenddata.json'></script>");
                                pageContent.Replace("spendingTrendscontainer", "spendingTrendscontainer" + page.Identifier);
                                scriptHtmlRenderer.Append(HtmlConstants.SPENDING_TREND_CHART_WIDGET_SCRIPT.Replace("spendingTrendscontainer", "spendingTrendscontainer" + page.Identifier));
                            }
                            else if (widget.WidgetName == HtmlConstants.SAVING_TREND_WIDGET_NAME)
                            {
                                scriptHtmlRenderer.Append("<script type='text/javascript' src='" + AppBaseDirectory + "\\Resources\\sampledata\\spendingtrenddata.json'></script>");
                                pageContent.Replace("savingTrendscontainer", "savingTrendscontainer" + page.Identifier);
                                scriptHtmlRenderer.Append(HtmlConstants.SAVING_TREND_CHART_WIDGET_SCRIPT.Replace("savingTrendscontainer", "savingTrendscontainer" + page.Identifier));
                            }
                            else if (widget.WidgetName == HtmlConstants.REMINDER_AND_RECOMMENDATION_WIDGET_NAME)
                            {
                                string reminderJson = "[{ 'Title': 'Update Missing Inofrmation', 'Action': 'Update' },{ 'Title': 'Your Rewards Video is available', 'Action': 'View' },{ 'Title': 'Payment Due for Home Loan', 'Action': 'Pay' }, { title: 'Need financial planning for savings.', action: 'Call Me' },{ title: 'Subscribe/Unsubscribe Alerts.', action: 'Apply' },{ title: 'Your credit card payment is due now.', action: 'Pay' }]";
                                if (reminderJson != string.Empty && validationEngine.IsValidJson(reminderJson))
                                {
                                    IList<ReminderAndRecommendation> reminderAndRecommendations = JsonConvert.DeserializeObject<List<ReminderAndRecommendation>>(reminderJson);
                                    StringBuilder reminderstr = new StringBuilder();
                                    reminderstr.Append("<div class='row'><div class='col-lg-9'></div><div class='col-lg-3 text-left'><i class='fa fa-caret-left fa-3x float-left text-danger' aria-hidden='true'></i><span class='mt-2 d-inline-block ml-2'>Click</span></div> </div>");
                                    reminderAndRecommendations.ToList().ForEach(item =>
                                    {
                                        reminderstr.Append("<div class='row'><div class='col-lg-9 text-left'><p class='p-1' style='background-color: #dce3dc;'>" +
                                            item.Title + " </p></div><div class='col-lg-3 text-left'> <a><i class='fa fa-caret-left fa-3x float-left " +
                                            "text-danger'></i><span class='mt-2 d-inline-block ml-2'>" + item.Action + "</span></a></div></div>");
                                    });
                                    pageContent.Replace("{{ReminderAndRecommdationDataList_" + page.Identifier + "_" + widget.Identifier + "}}", reminderstr.ToString());
                                }
                            }
                        }
                        else
                        {
                            DynamicWidgetSearchParameter dynamicWidgetSearchParameter = new DynamicWidgetSearchParameter
                            {
                                Identifier = Convert.ToString(widget.WidgetId),
                                PageTypeId = Convert.ToString(page.PageTypeId),
                                PagingParameter = new PagingParameter
                                {
                                    PageIndex = 0,
                                    PageSize = 0,
                                },
                                SortParameter = new SortParameter()
                                {
                                    SortOrder = SortOrder.Ascending,
                                    SortColumn = "Title",
                                },
                                SearchMode = SearchMode.Equals
                            };
                            var dynaWidgets = this.dynamicWidgetRepository.GetDynamicWidgets(dynamicWidgetSearchParameter, tenantCode);
                            if (dynaWidgets.Count > 0)
                            {
                                var dynawidget = dynaWidgets.FirstOrDefault();
                                TenantEntity entity = new TenantEntity();
                                entity.Identifier = dynawidget.EntityId;
                                entity.Name = dynawidget.EntityName;
                                CustomeTheme themeDetails = new CustomeTheme();
                                if (dynawidget.ThemeType == "Default")
                                {
                                    themeDetails = JsonConvert.DeserializeObject<CustomeTheme>(tenantConfiguration.WidgetThemeSetting);
                                }
                                else
                                {
                                    themeDetails = JsonConvert.DeserializeObject<CustomeTheme>(dynawidget.ThemeCSS);
                                }

                                if (dynawidget.WidgetType == HtmlConstants.TABLE_DYNAMICWIDGET)
                                {
                                    var tableWidgetHtml = HtmlConstants.TABLE_WIDEGT_FOR_PAGE_PREVIEW;
                                    //tableWidgetHtml = tableWidgetHtml.Replace("{{WidgetDivHeight}}", divHeight);
                                    tableWidgetHtml = this.ApplyStyleCssForDynamicTableAndFormWidget(tableWidgetHtml, themeDetails);
                                    tableWidgetHtml = tableWidgetHtml.Replace("{{WidgetTitle}}", dynawidget.Title);
                                    List<DynamicWidgetTableEntity> tableEntities = JsonConvert.DeserializeObject<List<DynamicWidgetTableEntity>>(dynawidget.WidgetSettings);
                                    StringBuilder tableHeader = new StringBuilder();
                                    tableHeader.Append("<tr>" + string.Join("", tableEntities.Select(field => string.Format("<th>{0}</th> ", field.HeaderName))) + "</tr>");
                                    tableWidgetHtml = tableWidgetHtml.Replace("{{tableHeader}}", tableHeader.ToString());
                                    tableWidgetHtml = tableWidgetHtml.Replace("{{tableBody}}", dynawidget.PreviewData);
                                    pageContent.Append(tableWidgetHtml);
                                }
                                else if (dynawidget.WidgetType == HtmlConstants.FORM_DYNAMICWIDGET)
                                {
                                    var formWidgetHtml = HtmlConstants.FORM_WIDGET_FOR_PAGE_PREVIEW;
                                    //formWidgetHtml = formWidgetHtml.Replace("{{WidgetDivHeight}}", divHeight);
                                    formWidgetHtml = this.ApplyStyleCssForDynamicTableAndFormWidget(formWidgetHtml, themeDetails);
                                    formWidgetHtml = formWidgetHtml.Replace("{{WidgetTitle}}", dynawidget.Title);
                                    formWidgetHtml = formWidgetHtml.Replace("{{FormData}}", dynawidget.PreviewData);
                                    pageContent.Append(formWidgetHtml);
                                }
                                else if (dynawidget.WidgetType == HtmlConstants.LINEGRAPH_DYNAMICWIDGET)
                                {
                                    var lineGraphWidgetHtml = HtmlConstants.LINE_GRAPH_FOR_PAGE_PREVIEW;
                                    lineGraphWidgetHtml = lineGraphWidgetHtml.Replace("lineGraphcontainer", "lineGraphcontainer_" + dynawidget.Identifier);
                                    //lineGraphWidgetHtml = lineGraphWidgetHtml.Replace("{{WidgetDivHeight}}", divHeight);
                                    lineGraphWidgetHtml = this.ApplyStyleCssForDynamicGraphAndChartWidgets(lineGraphWidgetHtml, themeDetails);
                                    lineGraphWidgetHtml = lineGraphWidgetHtml.Replace("{{WidgetTitle}}", dynawidget.Title);
                                    lineGraphWidgetHtml = lineGraphWidgetHtml + "<input type='hidden' id='hiddenLineGraphData_" + dynawidget.Identifier + "' value='" + dynawidget.PreviewData + "'>";
                                    linegraphIds.Add("lineGraphcontainer_" + dynawidget.Identifier);
                                    pageContent.Append(lineGraphWidgetHtml);
                                }
                                else if (dynawidget.WidgetType == HtmlConstants.BARGRAPH_DYNAMICWIDGET)
                                {
                                    var barGraphWidgetHtml = HtmlConstants.BAR_GRAPH_FOR_PAGE_PREVIEW;
                                    //barGraphWidgetHtml = barGraphWidgetHtml.Replace("{{WidgetDivHeight}}", divHeight);
                                    barGraphWidgetHtml = barGraphWidgetHtml.Replace("barGraphcontainer", "barGraphcontainer_" + dynawidget.Identifier);
                                    barGraphWidgetHtml = this.ApplyStyleCssForDynamicGraphAndChartWidgets(barGraphWidgetHtml, themeDetails);
                                    barGraphWidgetHtml = barGraphWidgetHtml.Replace("{{WidgetTitle}}", dynawidget.Title);
                                    barGraphWidgetHtml = barGraphWidgetHtml + "<input type='hidden' id='hiddenBarGraphData_" + dynawidget.Identifier + "' value='" + dynawidget.PreviewData + "'>";
                                    bargraphIds.Add("barGraphcontainer_" + dynawidget.Identifier);
                                    pageContent.Append(barGraphWidgetHtml);
                                }
                                else if (dynawidget.WidgetType == HtmlConstants.PICHART_DYNAMICWIDGET)
                                {
                                    var pieChartWidgetHtml = HtmlConstants.PIE_CHART_FOR_PAGE_PREVIEW;
                                    //pieChartWidgetHtml = pieChartWidgetHtml.Replace("{{WidgetDivHeight}}", divHeight);
                                    pieChartWidgetHtml = pieChartWidgetHtml.Replace("pieChartcontainer", "pieChartcontainer_" + dynawidget.Identifier);
                                    pieChartWidgetHtml = this.ApplyStyleCssForDynamicGraphAndChartWidgets(pieChartWidgetHtml, themeDetails);
                                    pieChartWidgetHtml = pieChartWidgetHtml.Replace("{{WidgetTitle}}", dynawidget.Title);
                                    pieChartWidgetHtml = pieChartWidgetHtml + "<input type='hidden' id='hiddenPieChartData_" + dynawidget.Identifier + "' value='" + dynawidget.PreviewData + "'>";
                                    piechartIds.Add("pieChartcontainer_" + dynawidget.Identifier);
                                    pageContent.Append(pieChartWidgetHtml);
                                }
                                else if (dynawidget.WidgetType == HtmlConstants.HTMLWIDGETPREVIEW)
                                {
                                    var htmlWidget = HtmlConstants.HTML_WIDGET_FOR_PAGE_PREVIEW;
                                    //htmlWidget = htmlWidget.Replace("{{WidgetDivHeight}}", divHeight);
                                    htmlWidget = this.ApplyStyleCssForDynamicTableAndFormWidget(htmlWidget, themeDetails);
                                    htmlWidget = htmlWidget.Replace("{{WidgetTitle}}", dynawidget.Title);
                                    string settings = dynawidget.WidgetSettings;
                                    IList<EntityFieldMap> entityFieldMaps = new List<EntityFieldMap>();
                                    entityFieldMaps = this.dynamicWidgetRepository.GetEntityFields(dynawidget.EntityId, tenantCode);
                                    string data = this.GetHTMLPreviewData(entity, entityFieldMaps, dynawidget.WidgetSettings);
                                    htmlWidget = htmlWidget.Replace("{{FormData}}", data);
                                    pageContent.Append(htmlWidget);
                                }
                            }
                        }
                    }

                    if (linegraphIds.Count > 0)
                    {
                        var ids = string.Join(",", linegraphIds.Select(item => item).ToList());
                        pageContent.Append("<input type = 'hidden' id = 'hiddenLineChartIds' value = '" + ids + "'>");
                    }
                    if (bargraphIds.Count > 0)
                    {
                        var ids = string.Join(",", bargraphIds.Select(item => item).ToList());
                        pageContent.Append("<input type = 'hidden' id = 'hiddenBarChartIds' value = '" + ids + "'>");
                    }
                    if (piechartIds.Count > 0)
                    {
                        var ids = string.Join(",", piechartIds.Select(item => item).ToList());
                        pageContent.Append("<input type = 'hidden' id = 'hiddenPieChartIds' value = '" + ids + "'>");
                    }

                    newPageContent.Append(pageContent);
                    if (page.PageTypeName == HtmlConstants.SAVING_ACCOUNT_PAGE || page.PageTypeName == HtmlConstants.CURRENT_ACCOUNT_PAGE)
                    {
                        newPageContent.Append(HtmlConstants.END_DIV_TAG);
                    }
                    newPageContent.Append(HtmlConstants.PAGE_TAB_CONTENT_FOOTER); //to end tab-content div

                    PageHeaderContent.Replace("{{SubTabs}}", SubTabs.ToString());
                    statementPageContent.PageHeaderContent = PageHeaderContent.ToString();
                    statementPageContent.HtmlContent = newPageContent.ToString();
                    //newStatementPageContents.Add(statementPageContent);
                }

                newStatementPageContents.ToList().ForEach(page =>
                {
                    htmlbody.Append(page.PageHeaderContent);
                    htmlbody.Append(page.HtmlContent);
                    htmlbody.Append(page.PageFooterContent);
                });
                htmlbody.Append(HtmlConstants.CONTAINER_DIV_HTML_FOOTER);

                navbarHtml = navbarHtml.Replace("{{NavItemList}}", navbar.ToString());

                StringBuilder finalHtml = new StringBuilder();
                finalHtml.Append(HtmlConstants.HTML_HEADER);
                finalHtml.Append(navbarHtml);
                finalHtml.Append(htmlbody.ToString());
                finalHtml.Append(HtmlConstants.HTML_FOOTER);
                finalHtml.Replace("{{ChartScripts}}", scriptHtmlRenderer.ToString());
                return finalHtml.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method help to bind data to common statement
        /// </summary>
        /// <param name="statement"> the statement object </param>
        /// <param name="statementPageContents"> the statement page html content list</param>
        /// <param name="tenantCode"> the tenant code </param>
        public StatementPreviewData BindDataToCommonStatement(Statement statement, IList<StatementPageContent> statementPageContents, TenantConfiguration tenantConfiguration, string tenantCode, Client client)
        {
            try
            {
                var AppBaseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                StatementPreviewData statementPreviewData = new StatementPreviewData();

                //start to render common html content data
                StringBuilder htmlbody = new StringBuilder();
                string navbarHtml = HtmlConstants.NAVBAR_HTML_FOR_PREVIEW.Replace("{{logo}}", "../common/images/nisLogo.png");
                navbarHtml = navbarHtml.Replace("{{Today}}", DateTime.UtcNow.ToString("dd MMM yyyy")); //bind current date to html header
                
                //get client logo in string format and pass it hidden input tag, so it will be render in right side of header of html statement
                var clientlogo = client.TenantLogo != null ? client.TenantLogo : "";
                navbarHtml = navbarHtml + "<input type='hidden' id='TenantLogoImageValue' value='" + clientlogo + "'>";
                htmlbody.Append(HtmlConstants.CONTAINER_DIV_HTML_HEADER);

                //this variable is used to bind all script to html statement, which helps to render data on chart and graph widgets
                StringBuilder scriptHtmlRenderer = new StringBuilder();
                StringBuilder navbar = new StringBuilder();
                var newStatementPageContents = new List<StatementPageContent>();
                IList<FileData> SampleFiles = new List<FileData>();
                statementPageContents.ToList().ForEach(it => newStatementPageContents.Add(new StatementPageContent()
                {
                    Id = it.Id,
                    PageId = it.PageId,
                    PageTypeId = it.PageTypeId,
                    HtmlContent = it.HtmlContent,
                    PageHeaderContent = it.PageHeaderContent,
                    PageFooterContent = it.PageFooterContent,
                    DisplayName = it.DisplayName,
                    TabClassName = it.TabClassName,
                    DynamicWidgets = it.DynamicWidgets
                }));
                for (int i = 0; i < statement.Pages.Count; i++)
                {
                    var page = statement.Pages[i];
                    StatementPageContent statementPageContent = newStatementPageContents.Where(item => item.PageTypeId == page.PageTypeId && item.Id == i).FirstOrDefault();
                    StringBuilder pageContent = new StringBuilder(statementPageContent.HtmlContent);
                    var dynamicWidgets = statementPageContent.DynamicWidgets;

                    StringBuilder SubTabs = new StringBuilder();
                    StringBuilder PageHeaderContent = new StringBuilder(statementPageContent.PageHeaderContent);

                    string tabClassName = Regex.Replace((statementPageContent.DisplayName + "-" + page.Identifier), @"\s+", "-");
                    navbar.Append(" <li class='nav-item'><a class='nav-link pt-1 mainNav " + (i == 0 ? "active" : "") + " " + tabClassName + "' href='javascript:void(0);' >" + statementPageContent.DisplayName + "</a> </li> ");
                    string ExtraClassName = i > 0 ? "d-none " + tabClassName : tabClassName;
                    PageHeaderContent.Replace("{{ExtraClass}}", ExtraClassName);
                    PageHeaderContent.Replace("{{DivId}}", tabClassName);

                    StringBuilder newPageContent = new StringBuilder();
                    newPageContent.Append(HtmlConstants.PAGE_TAB_CONTENT_HEADER);

                    if (page.PageTypeName == HtmlConstants.SAVING_ACCOUNT_PAGE || page.PageTypeName == HtmlConstants.CURRENT_ACCOUNT_PAGE)
                    {
                        SubTabs.Append("<ul class='nav nav-tabs' style='margin-top:-20px;'>");
                        SubTabs.Append("<li class='nav-item active'><a id='tab1-tab' data-toggle='tab' " + "data-target='#" + (page.PageTypeName == HtmlConstants.SAVING_ACCOUNT_PAGE ? "Saving" : "Current") + "-' role='tab' class='nav-link active'> Account - 6789</a></li>");
                        SubTabs.Append("</ul>");

                        newPageContent.Append("<div id='" + (page.PageTypeName == HtmlConstants.SAVING_ACCOUNT_PAGE ? "Saving" : "Current") + "-6789' >");
                    }

                    var pagewidgets = new List<PageWidget>(page.PageWidgets);
                    for (int j = 0; j < pagewidgets.Count; j++)
                    {
                        var widget = pagewidgets[j];
                        if (!widget.IsDynamicWidget)
                        {
                            if (widget.WidgetName == HtmlConstants.CUSTOMER_INFORMATION_WIDGET_NAME)
                            {
                                string customerInfoJson = "{'FirstName':'Laura','MiddleName':'J','LastName':'Donald','AddressLine1':'4000 Executive Parkway', 'AddressLine2':'Saint Globin Rd','City':'Canary Wharf', 'State':'London', 'Country':'England','Zip':'E14 9RZ'}";
                                if (customerInfoJson != string.Empty && validationEngine.IsValidJson(customerInfoJson))
                                {
                                    CustomerInformation customerInfo = JsonConvert.DeserializeObject<CustomerInformation>(customerInfoJson);
                                    pageContent.Replace("{{VideoSource_" + statement.Identifier + "_" + page.Identifier + "_" + widget.Identifier + "}}", AppBaseDirectory + "\\Resources\\sampledata\\SampleVideo.mp4");

                                    string customerName = customerInfo.FirstName + " " + customerInfo.SurName;
                                    pageContent.Replace("{{CustomerName}}", customerName);

                                    string address1 = customerInfo.AddressLine1 + ", " + customerInfo.AddressLine2 + ", ";
                                    pageContent.Replace("{{Address1}}", address1);

                                    string address2 = customerInfo.AddressLine3 + ", " + customerInfo.AddressLine4 + ", ";
                                    pageContent.Replace("{{Address2}}", address2);
                                }
                            }
                            else if (widget.WidgetName == HtmlConstants.PAYMENT_SUMMARY_WIDGET_NAME)
                            {
                                string paymentInfoJson = "{Reg_ID : 1,Start_Date : '2023-01-01',End_Date : '2023-01-01',Request_DateTime : 'DummyText1',ID : '124529534',Intermediary_Code : 'DummyText1',FSP_ID : 'DummyText1',Policy_Number : 'DummyText1',FSP_Party_ID : 'DummyText1',Client_Number : '124556686',FSP_REF : '2452953',Client_Name : 'Mr SCHOELER',Int_ID : 'DummyText1',Product_Type : 'DummyText1',Commission_Amount : 'DummyText1',INT_EXT_REF : '124411745',Int_Name : 'Kruger Van Heerden',Int_Type : 'DummyText1',Policy_Ref : '5596100',Member_Ref : '124556686',Member_Name : 'DummyText1',Transaction_Amount : 'DummyText1',Mem_Age : 'DummyText1',Months_In_Force : 'DummyText1',Commission_Type : 'Safe Custody Fee',Description : 'Safe Custody Service Fee',POSTED_DATE : '2023-03-03',AE_Type_ID : 'DummyText1',AE_Amount : 'DummyText1',DR_CR : 'DummyText1',NAME : 'DummyText1',Member_Surname : 'DummyText1',Jurisdiction : 'DummyText1',Sales_Office : 'DummyText1',FSP_Name : 'Miss Yvonne van Heerden',FSP_Trading_Name : 'T/A Yvonne Van Heerden Financial Planner CC',FSP_Ext_Ref : '124529534',FSP_Kind : 'DummyText1',  		FSP_VAT_Number : '2452953',Product : 'DummyText1',Prod_Group : 'Service Fee',Prod_Seq : 'DummyText1',Report_Seq : 'DummyText1',TYPE : 'DummyText1',Display_Amount : '17.55',VAT_Amount : '38001.27',Earning_Amount : '256670.66',Payment_Amount : 'DummyText1',START_DATE : 'DummyText1',END_DATE : 'DummyText1',Business_Type : 'DummyText1',Lifecycle_Description : 'DummyText1',Lifecycle_Start_Date : 'DummyText1',AE_Scheduler_ID : 'DummyText1',VAT_Amount_1 : 'DummyText1',Final_Amount : 'DummyText1'}";
                                if (paymentInfoJson != string.Empty && validationEngine.IsValidJson(paymentInfoJson))
                                {
                                    spIAA_PaymentDetail paymentInfo = JsonConvert.DeserializeObject<spIAA_PaymentDetail>(paymentInfoJson);
                                    pageContent.Replace("{{IntTotal}}", paymentInfo.Earning_Amount);
                                    pageContent.Replace("{{Vat}}", paymentInfo.VAT_Amount);
                                    pageContent.Replace("{{TotalDue}}", (Convert.ToDouble(paymentInfo.Earning_Amount) + Convert.ToDouble(paymentInfo.VAT_Amount)).ToString());

                                    // Format the date to month-year format                                                   
                                    pageContent.Replace("{{IntTotalDate}}", paymentInfo.POSTED_DATE.ToString("MMMM yyyy"));
                                    // Format the date with a custom format
                                    string formattedOrdinalDate = FormatDateWithOrdinal(paymentInfo.POSTED_DATE);
                                    pageContent.Replace("{{IntPostedDate}}", formattedOrdinalDate);

                                }
                            }

                            else if (widget.WidgetName == HtmlConstants.PPS_HEADING_WIDGET_NAME)
                            {
                                string headingInfoJson = "{'CustomerId':'7','BatchId':'35','AccountNumber':'LD01254-222222','AccountType':'Current Account','Currency':'$','Balance':'6235.34','TotalDeposit':'15432.00','TotalSpend':'5760.00','ProfitEarned':'3456.00','Indicator':'Up','FeesPaid':'345.00','GrandTotal':'24356.00','Percentage':'50.00','TenantCode':'00000000-0000-0000-0000-000000000000'}";
                                if (headingInfoJson != string.Empty && validationEngine.IsValidJson(headingInfoJson))
                                {
                                    //AccountMaster paymentInfo = JsonConvert.DeserializeObject<AccountMaster>(headingInfoJson);
                                    //pageContent.Replace("{{IntTotal}}", paymentInfo.GrandTotal);
                                    //pageContent.Replace("{{Vat}}", paymentInfo.FeesPaid);
                                    //pageContent.Replace("{{TotalDue}}", (Convert.ToDouble(paymentInfo.GrandTotal) + Convert.ToDouble(paymentInfo.FeesPaid)).ToString());

                                }
                            }
                            else if (widget.WidgetName == HtmlConstants.PRODUCT_SUMMARY_WIDGET_NAME)
                            {
                                {
                                    string productSummaryListJson = "[{ 'Commission_Type': 'Safe Custody Fee', 'Prod_Group':'Safe Custody Fee', 'Display_Amount': 'R52,65','QueryLink': 'https://facebook.com'},{ 'Commission_Type': 'Safe Custody Fee', 'Prod_Group':'Service Fee', 'Display_Amount': 'R52,66', 'QueryLink': 'https://facebook.com' }, { 'Commission_Type': 'Safe Custody Fee', 'Prod_Group':'Safe Custody Fee', 'Display_Amount': 'R52,67', 'QueryLink': 'https://facebook.com' }, { 'Commission_Type': 'Safe Custody Fee', 'Prod_Group':'Service Fee', 'Display_Amount': 'R52,68', 'QueryLink': 'https://facebook.com' } ]";

                                    if (productSummaryListJson != string.Empty && validationEngine.IsValidJson(productSummaryListJson))
                                    {
                                        IList<spIAA_PaymentDetail> productSummary = JsonConvert.DeserializeObject<List<spIAA_PaymentDetail>>(productSummaryListJson);
                                        StringBuilder productSummarySrc = new StringBuilder();
                                        long index = 1;
                                        productSummary.ToList().ForEach(item =>
                                        {
                                            productSummarySrc.Append("<tr><td align='center' valign='center' class='px-1 py-1 fsp-bdr-right fsp-bdr-bottom'>" + index + "</td><td class='fsp-bdr-right fsp-bdr-bottom px-1'>" + item.Commission_Type + "</td>" + "<td class='fsp-bdr-right fsp-bdr-bottom px-1'> " + (item.Prod_Group == "Service Fee" ? "Premium Under Advise Fee" : item.Prod_Group) + "</td> <td class='text-right fsp-bdr-right fsp-bdr-bottom px-1'>R" + item.Display_Amount + "</td><td class='text-center fsp-bdr-bottom px-1'><a  href ='https://facebook.com' target='_blank'><img class='leftarrowlogo' src ='assets/images/leftarrowlogo.png' alt = 'Left Arrow'></a></td></tr>");
                                            index++;
                                        });
                                        pageContent.Replace("{{ProductSummary}}", productSummarySrc.ToString());
                                        string productInfoJson = "{Earning_Amount : '256670,66',VAT_Amount : '38001,27'}";
                                        spIAA_PaymentDetail productInfo = JsonConvert.DeserializeObject<spIAA_PaymentDetail>(productInfoJson);
                                        pageContent.Replace("{{QueryBtn}}", "assets/images/IfQueryBtn.jpg");
                                        pageContent.Replace("{{TotalDue}}", "R" + (Convert.ToDouble(productInfo.Earning_Amount)).ToString());
                                        pageContent.Replace("{{VATDue}}", "R" + productInfo.VAT_Amount.ToString());
                                        double grandTotalDue = (Convert.ToDouble(productInfo.Earning_Amount) + Convert.ToDouble(productInfo.VAT_Amount));
                                        pageContent.Replace("{{GrandTotalDue}}", "R" + grandTotalDue.ToString());
                                        double ppsPayment = grandTotalDue;
                                        pageContent.Replace("{{PPSPayment}}", "-R" + (grandTotalDue).ToString());
                                        pageContent.Replace("{{Balance}}", "R" + Convert.ToDouble((grandTotalDue - ppsPayment)).ToString("F2"));
                                    }
                                }
                            }

                            else if (widget.WidgetName == HtmlConstants.PPS_FOOTER1_WIDGET_NAME)
                            {
                                string ppsFooter1InfoJson = "{'CustomerId':'7','BatchId':'35','AccountNumber':'LD01254-222222','AccountType':'Current Account','Currency':'$','Balance':'6235.34','TotalDeposit':'15432.00','TotalSpend':'5760.00','ProfitEarned':'3456.00','Indicator':'Up','FeesPaid':'345.00','GrandTotal':'24356.00','Percentage':'50.00','TenantCode':'00000000-0000-0000-0000-000000000000'}";
                                if (ppsFooter1InfoJson != string.Empty && validationEngine.IsValidJson(ppsFooter1InfoJson))
                                {
                                    //AccountMaster paymentInfo = JsonConvert.DeserializeObject<AccountMaster>(headingInfoJson);
                                    //pageContent.Replace("{{IntTotal}}", paymentInfo.GrandTotal);
                                    //pageContent.Replace("{{Vat}}", paymentInfo.FeesPaid);
                                    //pageContent.Replace("{{TotalDue}}", (Convert.ToDouble(paymentInfo.GrandTotal) + Convert.ToDouble(paymentInfo.FeesPaid)).ToString());

                                }
                            }

                            else if (widget.WidgetName == HtmlConstants.FOOTER_IMAGE_WIDGET_NAME)
                            {
                                string footerImageInfoJson = "{'CustomerId':'7','BatchId':'35','AccountNumber':'LD01254-222222','AccountType':'Current Account','Currency':'$','Balance':'6235.34','TotalDeposit':'15432.00','TotalSpend':'5760.00','ProfitEarned':'3456.00','Indicator':'Up','FeesPaid':'345.00','GrandTotal':'24356.00','Percentage':'50.00','TenantCode':'00000000-0000-0000-0000-000000000000'}";
                                if (footerImageInfoJson != string.Empty && validationEngine.IsValidJson(footerImageInfoJson))
                                {
                                    //AccountMaster paymentInfo = JsonConvert.DeserializeObject<AccountMaster>(headingInfoJson);
                                    //pageContent.Replace("{{IntTotal}}", paymentInfo.GrandTotal);
                                    //pageContent.Replace("{{Vat}}", paymentInfo.FeesPaid);
                                    //pageContent.Replace("{{TotalDue}}", (Convert.ToDouble(paymentInfo.GrandTotal) + Convert.ToDouble(paymentInfo.FeesPaid)).ToString());

                                }
                            }

                            else if (widget.WidgetName == HtmlConstants.DETAILED_TRANSACTIONS_WIDGET_NAME)
                            {
                               string transactionListJson = "[{'INT_EXT_REF':'2164250','Int_Name':'Mr SCHOELER','Client_Name':'Kruger Van Heerden','Member_Ref':124556686,'Policy_Ref':5596100,'Description':'Safe Custody Service Fee','Commission_Type':'Safe Custody Fee','POSTED_DATE':'20-Mar-23','Display_Amount':'17.55','Query_Link':'https://facebook.com','TYPE':'Fiduciary_Data','Prod_Group':'Safe Custody Fee'},{'INT_EXT_REF':'2164250','Int_Name':'Yvonne Van Heerden','Client_Name':'Mr SCHOELER','Member_Ref':124556686,'Policy_Ref':'5596100','Description':'Safe Custody Service Fee VAT','Commission_Type':'Safe Custody Fee','POSTED_DATE':'20-Mar-23','Display_Amount':'2.63','Query_Link':'https://facebook.com','TYPE':'Fiduciary_Data','Prod_Group':'Safe Custody Fee'},{'INT_EXT_REF':'124411745','Int_Name':'Kruger Van Heerden','Client_Name':'DR N J Olivier','Member_Ref':'1217181','Policy_Ref':'5524069','Description':'Safe Custody Service Fee','Commission_Type':'Safe Custody Fee','POSTED_DATE':'20-Mar-23','Display_Amount':'17.55','Query_Link':'https://facebook.com','TYPE':'Fiduciary_Data','Prod_Group':'Safe Custody Fee'},{'INT_EXT_REF':'124411745','Int_Name':'Kruger Van Heerden','Client_Name':'DR N J Olivier','Member_Ref':'124556686','Policy_Ref':'5596100','Description':'Safe Custody Service Fee VAT','Commission_Type':'Safe Custody Fee','POSTED_DATE':'20-Mar-23','Display_Amount':'2.63','Query_Link':'https://facebook.com','TYPE':'Fiduciary_Data','Prod_Group':'VAT'}]";
                                double TotalPostedAmount = 0;
                                if (transactionListJson != string.Empty && validationEngine.IsValidJson(transactionListJson))
                                {
                                    IList<spIAA_PaymentDetail> transaction = JsonConvert.DeserializeObject<List<spIAA_PaymentDetail>>(transactionListJson);
                                    StringBuilder detailedTransactionSrc = new StringBuilder();
                                    var records = transaction.GroupBy(gptransactionitem => gptransactionitem.INT_EXT_REF).ToList();
                                    records?.ForEach(transactionitem =>
                                    {
                                        detailedTransactionSrc.Append("<div class='px-50'><div class='prouct-table-block'><div class='text-left fsp-transaction-title font-weight-bold mb-3'>Intermediary:  " + transactionitem.FirstOrDefault().INT_EXT_REF + " " + transactionitem.FirstOrDefault().Int_Name + "</div><table width='100%' cellpadding='0' cellspacing='0'> <tr><th class='font-weight-bold text-white'>Client name</th> <th class='font-weight-bold text-white text-center pe-0 bdr-r-0'>Member<br /> number</th> <th class='font-weight-bold text-white text-center'>Will<br/> number</th> <th class='font-weight-bold text-white text-center'>Fiduciary fees</th> <th class='font-weight-bold text-white text-center'>Commission<br /> type</th> <th class='font-weight-bold text-white text-center'>Posted date</th> <th class='font-weight-bold text-white text-center'>Posted amount</th> <th class='font-weight-bold text-white'>Query</th> </tr> ");
                                        pageContent.Replace("{{QueryBtnImgLink}}", "https://www.google.com/");
                                        pageContent.Replace("{{QueryBtn}}", "../common/images/IfQueryBtn.jpg");
                                        transaction.Where(witem => witem.INT_EXT_REF == transactionitem.FirstOrDefault().INT_EXT_REF).ToList().ForEach(item =>
                                        {
                                            detailedTransactionSrc.Append("<tr><td align = 'center' valign = 'center' class='px-1 py-1 fsp-bdr-right fsp-bdr-bottom'>" +
                                                    item.Client_Name + "</td><td class= 'fsp-bdr-right fsp-bdr-bottom px-1'>" + item.Member_Ref + "</td><td class= 'fsp-bdr-right fsp-bdr-bottom px-1'> " + item.Policy_Ref + "</td><td class= 'text-right fsp-bdr-right fsp-bdr-bottom px-1'>" + (item.Description == "Commission Service Fee" ? "Premium Under Advise Fee" : item.Description)  + "</td><td class= 'text-center fsp-bdr-right fsp-bdr-bottom px-1'>" + item.Commission_Type + "</td><td class= 'text-center fsp-bdr-right fsp-bdr-bottom px-1'>" + item.POSTED_DATE.ToString("dd-MMM-yyyy") + "</td><td class= 'text-center fsp-bdr-right fsp-bdr-bottom px-1'> R" + item.Display_Amount + "</td><td class= 'text-center fsp-bdr-bottom px-1'><a href ='https://www.google.com/' target ='_blank'><img class='leftarrowlogo' src='assets/images/leftarrowlogo.png' alt='Left Arrow'></a></td></tr>");
                                            TotalPostedAmount += ((item.TYPE == "Fiduciary_Data") && (item.Prod_Group != "VAT"))?  (Convert.ToDouble(item.Display_Amount)): 0.0;
                                        });
                                        string TotalPostedAmountR = (TotalPostedAmount == 0) ? "0.00" : ("R" + TotalPostedAmount.ToString());
                                        detailedTransactionSrc.Append("<tr> <td align='center' valign='center' class='px-1 py-1 fsp-bdr-right fsp-bdr-bottom'></td> <td class='fsp-bdr-right fsp-bdr-bottom px-1 py-1'></td> <td class='fsp-bdr-right fsp-bdr-bottom px-1 py-1'></td> <td class='text-right fsp-bdr-right fsp-bdr-bottom px-1 py-1'></td> <td class='text-center fsp-bdr-right fsp-bdr-bottom px-1 py-1'><br /></td> <td class='text-center fsp-bdr-right fsp-bdr-bottom px-1 py-1'></td> <td class='text-center fsp-bdr-right fsp-bdr-bottom px-1 py-1'>" + TotalPostedAmountR + "</td> <td class='text-center fsp-bdr-bottom px-1'><a href='https://www.google.com/' target = '_blank' ><img src='assets/images/leftarrowlogo.png'></a></td> </tr></table><div class='text-right w-100 pt-3'><a href='https://www.google.com/' target = '_blank'><img src='assets/images/click-print-stmt-btn.jpg'></a></div></div></div></div>");
                                        TotalPostedAmount = 0;
                                    });
                                    pageContent.Replace("{{detailedTransaction}}", detailedTransactionSrc.ToString());
                                }
                            }

                            else if (widget.WidgetName == HtmlConstants.ACCOUNT_INFORMATION_WIDGET_NAME)
                            {
                                string accountInfoJson = "{'StatementDate':'1-APR-2020','StatementPeriod':'Annual Statement', 'CustomerID':'ID2-8989-5656','RmName':'James Wiilims','RmContactNumber':'+4487867833'}";

                                string accountInfoData = string.Empty;
                                StringBuilder AccDivData = new StringBuilder();
                                if (accountInfoJson != string.Empty && validationEngine.IsValidJson(accountInfoJson))
                                {
                                    AccountInformation accountInfo = JsonConvert.DeserializeObject<AccountInformation>(accountInfoJson);
                                    AccDivData.Append("<div class='list-row-small ht70px'><div class='list-middle-row'> <div class='list-text'>" + "Statement Date</div><label class='list-value mb-0'>" + accountInfo.StatementDate + "</label>" + "</div></div>");

                                    AccDivData.Append("<div class='list-row-small ht70px'><div class='list-middle-row'> <div class='list-text'>" + "Statement Period</div><label class='list-value mb-0'>" + accountInfo.StatementPeriod + "</label></div></div>");

                                    AccDivData.Append("<div class='list-row-small ht70px'><div class='list-middle-row'> <div class='list-text'>" + "Cusomer ID</div><label class='list-value mb-0'>" + accountInfo.CustomerID + "</label></div></div>");

                                    AccDivData.Append("<div class='list-row-small ht70px'><div class='list-middle-row'> <div class='list-text'>" + "RM Name</div><label class='list-value mb-0'>" + accountInfo.RmName + "</label></div></div>");

                                    AccDivData.Append("<div class='list-row-small ht70px'><div class='list-middle-row'> <div class='list-text'>" + "RM Contact Number</div><label class='list-value mb-0'>" + accountInfo.RmContactNumber + "</label></div></div>");
                                }
                                pageContent.Replace("{{AccountInfoData_" + page.Identifier + "_" + widget.Identifier + "}}", AccDivData.ToString());
                            }
                            else if (widget.WidgetName == HtmlConstants.IMAGE_WIDGET_NAME)
                            {
                                var imageAssetPath = AppBaseDirectory + "\\Resources\\sampledata\\icon-image.png";
                                if (widget.WidgetSetting != string.Empty && validationEngine.IsValidJson(widget.WidgetSetting))
                                {
                                    dynamic widgetSetting = JObject.Parse(widget.WidgetSetting);
                                    if (widgetSetting.isPersonalize == false)
                                    {
                                        var asset = assetLibraryRepository.GetAssets(new AssetSearchParameter { Identifier = widgetSetting.AssetId, SortParameter = new SortParameter { SortColumn = "Id" } }, tenantCode).ToList()?.FirstOrDefault();
                                        if (asset != null)
                                        {
                                            FileData fileData = new FileData();
                                            fileData.FileName = "Image" + page.Identifier + widget.Identifier + ".jpg";
                                            fileData.FileUrl = asset.FilePath; //baseURL + "/assets/" + widgetSetting.AssetLibraryId + "/" + widgetSetting.AssetName;
                                            SampleFiles.Add(fileData);
                                            imageAssetPath = "./" + fileData.FileName;
                                        }
                                    }
                                }
                                pageContent.Replace("{{ImageSource_" + statement.Identifier + "_" + page.Identifier + "_" + widget.Identifier + "}}", imageAssetPath);
                            }
                            else if (widget.WidgetName == HtmlConstants.VIDEO_WIDGET_NAME)
                            {
                                var videoAssetPath = AppBaseDirectory + "\\Resources\\sampledata\\SampleVideo.mp4";
                                if (widget.WidgetSetting != string.Empty && validationEngine.IsValidJson(widget.WidgetSetting))
                                {
                                    dynamic widgetSetting = JObject.Parse(widget.WidgetSetting);
                                    if (widgetSetting.isEmbedded == true)
                                    {
                                        videoAssetPath = widgetSetting.SourceUrl;
                                    }
                                    else if (widgetSetting.isPersonalize == false && widgetSetting.isEmbedded == false)
                                    {
                                        var asset = assetLibraryRepository.GetAssets(new AssetSearchParameter { Identifier = widgetSetting.AssetId, SortParameter = new SortParameter { SortColumn = "Id" } }, tenantCode).ToList()?.FirstOrDefault();
                                        if (asset != null)
                                        {
                                            FileData fileData = new FileData();
                                            fileData.FileName = "Video" + page.Identifier + widget.Identifier + ".jpg";
                                            fileData.FileUrl = asset.FilePath;
                                            SampleFiles.Add(fileData);
                                            videoAssetPath = "./" + fileData.FileName;
                                        }
                                    }
                                }
                                pageContent.Replace("{{VideoSource_" + statement.Identifier + "_" + page.Identifier + "_" + widget.Identifier + "}}", videoAssetPath);
                            }
                            else if (widget.WidgetName == HtmlConstants.SUMMARY_AT_GLANCE_WIDGET_NAME)
                            {
                                string accountBalanceDataJson = "[{\"AccountType\":\"Saving Account\",\"Currency\":\"$\",\"Amount\":\"87356\"}" +
                                    ",{\"AccountType\":\"Current Account\",\"Currency\":\"$\",\"Amount\":\"18654\"},{\"AccountType\":" +
                                    "\"Recurring Account\",\"Currency\":\"$\",\"Amount\":\"54367\"},{\"AccountType\":\"Wealth\",\"Currency\"" + ":\"$\",\"Amount\":\"4589\"}]";

                                string accountSummary = string.Empty;
                                if (accountBalanceDataJson != string.Empty && validationEngine.IsValidJson(accountBalanceDataJson))
                                {
                                    IList<AccountSummary> lstAccountSummary = JsonConvert.DeserializeObject<List
                                        <AccountSummary>>(accountBalanceDataJson);
                                    if (lstAccountSummary.Count > 0)
                                    {
                                        StringBuilder accSummary = new StringBuilder();
                                        lstAccountSummary.ToList().ForEach(acc =>
                                        {
                                            accSummary.Append("<tr><td>" + acc.AccountType + "</td><td>" + acc.Currency + "</td><td>" + acc.Amount + "</td></tr>");
                                        });
                                        pageContent.Replace("{{AccountSummary_" + page.Identifier + "_" + widget.Identifier + "}}", accSummary.ToString());
                                    }
                                }
                            }
                            else if (widget.WidgetName == HtmlConstants.CURRENT_AVAILABLE_BALANCE_WIDGET_NAME)
                            {
                                string currentAvailBalanceJson = "{'GrandTotal':'32,453,23', 'TotalDeposit':'16,250,00', 'TotalSpend':'16,254,00', 'ProfitEarned':'1,430,00 ', 'Currency':'$', 'Balance': '14,768,80', 'AccountNumber': 'J566565TR678ER', 'AccountType': 'Current', 'Indicator': 'Up'}";
                                if (currentAvailBalanceJson != string.Empty && validationEngine.IsValidJson(currentAvailBalanceJson))
                                {
                                    AccountMaster accountMaster = JsonConvert.DeserializeObject<AccountMaster>(currentAvailBalanceJson);
                                    var accountIndicatorClass = accountMaster.Indicator.ToLower().Equals("up") ? "fa fa-sort-asc text-success" : "fa fa-sort-desc text-danger";
                                    pageContent.Replace("{{AccountIndicatorClass}}", accountIndicatorClass);
                                    pageContent.Replace("{{TotalValue_" + page.Identifier + "_" + widget.Identifier + "}}", (accountMaster.Currency + accountMaster.GrandTotal));
                                    pageContent.Replace("{{TotalDeposit_" + page.Identifier + "_" + widget.Identifier + "}}", (accountMaster.Currency + accountMaster.TotalDeposit));
                                    pageContent.Replace("{{TotalSpend_" + page.Identifier + "_" + widget.Identifier + "}}", (accountMaster.Currency + accountMaster.TotalSpend));
                                    pageContent.Replace("{{Savings_" + page.Identifier + "_" + widget.Identifier + "}}", (accountMaster.Currency + accountMaster.ProfitEarned));
                                }
                            }
                            else if (widget.WidgetName == HtmlConstants.SAVING_AVAILABLE_BALANCE_WIDGET_NAME)
                            {
                                string savingAvailBalanceJson = "{'GrandTotal':'26,453,23', 'TotalDeposit':'13,530,00', 'TotalSpend':'12,124,00', 'ProfitEarned':'2,340,00 ', 'Currency':'$', 'Balance': '19,456,80', 'AccountNumber': 'J566565TR678ER', 'AccountType': 'Saving', 'Indicator': 'Up'}";
                                if (savingAvailBalanceJson != string.Empty && validationEngine.IsValidJson(savingAvailBalanceJson))
                                {
                                    AccountMaster accountMaster = JsonConvert.DeserializeObject<AccountMaster>(savingAvailBalanceJson);
                                    var accountIndicatorClass = accountMaster.Indicator.ToLower().Equals("up") ? "fa fa-sort-asc text-success" : "fa fa-sort-desc text-danger";
                                    pageContent.Replace("{{AccountIndicatorClass}}", accountIndicatorClass);
                                    pageContent.Replace("{{TotalValue_" + page.Identifier + "_" + widget.Identifier + "}}", (accountMaster.Currency + accountMaster.GrandTotal));
                                    pageContent.Replace("{{TotalDeposit_" + page.Identifier + "_" + widget.Identifier + "}}", (accountMaster.Currency + accountMaster.TotalDeposit));
                                    pageContent.Replace("{{TotalSpend_" + page.Identifier + "_" + widget.Identifier + "}}", (accountMaster.Currency + accountMaster.TotalSpend));
                                    pageContent.Replace("{{Savings_" + page.Identifier + "_" + widget.Identifier + "}}", (accountMaster.Currency + accountMaster.ProfitEarned));
                                }
                            }
                            else if (widget.WidgetName == HtmlConstants.SAVING_TRANSACTION_WIDGET_NAME)
                            {
                                StringBuilder selectOption = new StringBuilder();
                                var distinctNaration = new string[] { "NXT TXN: IIFL IIFL6574562", "NXT TXN: IIFL IIFL6574563", "NXT TXN: IIFL IIFL3557346", "NXT TXN: IIFL RTED87978947 REFUND", "NXT TXN: IIFL IIFL896452896ERE", "NXT TXN: IIFL IIFL8965435", "NXT TXN: IIFL FGTR454565JHGKD", "NXT TXN: OFFICE RENT 798789DFGH", "NXT TXN: IIFL IIFL0034212", "NXT TXN: IIFL IIFL045678DFGH" };
                                distinctNaration.ToList().ForEach(item =>
                                {
                                    selectOption.Append("<option value='" + item + "'> " + item + "</option>");
                                });

                                FileData fileData = new FileData();
                                fileData.FileName = "savingtransactiondetail.json";
                                fileData.FileUrl = AppBaseDirectory + "\\Resources\\sampledata\\savingtransactiondetail.json";
                                SampleFiles.Add(fileData);
                                scriptHtmlRenderer.Append("<script type='text/javascript' src='./" + fileData.FileName + "'></script>");
                                //scriptHtmlRenderer.Append("<script type='text/javascript' src='" + AppBaseDirectory + "\\Resources\\sampledata\\savingtransactiondetail.json'></script>");
                                StringBuilder scriptval = new StringBuilder(HtmlConstants.SAVING_TRANSACTION_DETAIL_GRID_WIDGET_SCRIPT);
                                scriptval.Replace("SavingTransactionTable", "SavingTransactionTable" + page.Identifier);
                                scriptval.Replace("savingShowAll", "savingShowAll" + page.Identifier);
                                scriptval.Replace("filterStatus", "filterStatus" + page.Identifier);
                                scriptval.Replace("ResetGrid", "ResetGrid" + page.Identifier);
                                scriptval.Replace("PrintGrid", "PrintGrid" + page.Identifier);
                                scriptHtmlRenderer.Append(scriptval);

                                pageContent.Replace("savingShowAll", "savingShowAll" + page.Identifier);
                                pageContent.Replace("filterStatus", "filterStatus" + page.Identifier);
                                pageContent.Replace("ResetGrid", "ResetGrid" + page.Identifier);
                                pageContent.Replace("PrintGrid", "PrintGrid" + page.Identifier);
                                pageContent.Replace("{{SelectOption_" + page.Identifier + "_" + widget.Identifier + "}}", selectOption.ToString());
                                pageContent.Replace("SavingTransactionTable", "SavingTransactionTable" + page.Identifier);
                                pageContent.Replace("{{AccountTransactionDetails_" + page.Identifier + "_" + widget.Identifier + "}}", string.Empty);
                            }
                            else if (widget.WidgetName == HtmlConstants.CURRENT_TRANSACTION_WIDGET_NAME)
                            {
                                StringBuilder selectOption = new StringBuilder();
                                var distinctNaration = new string[] { "NXT TXN: IIFL IIFL6574562", "NXT TXN: IIFL IIFL6574563", "NXT TXN: IIFL IIFL3557346", "NXT TXN: IIFL RTED87978947 REFUND", "NXT TXN: IIFL IIFL896452896ERE", "NXT TXN: IIFL IIFL8965435", "NXT TXN: IIFL FGTR454565JHGKD", "NXT TXN: OFFICE RENT 798789DFGH", "NXT TXN: IIFL IIFL0034212", "NXT TXN: IIFL IIFL045678DFGH" };
                                distinctNaration.ToList().ForEach(item =>
                                {
                                    selectOption.Append("<option value='" + item + "'> " + item + "</option>");
                                });

                                FileData fileData = new FileData();
                                fileData.FileName = "currenttransactiondetail.json";
                                fileData.FileUrl = AppBaseDirectory + "\\Resources\\sampledata\\currenttransactiondetail.json";
                                SampleFiles.Add(fileData);
                                scriptHtmlRenderer.Append("<script type='text/javascript' src='./" + fileData.FileName + "'></script>");
                                //scriptHtmlRenderer.Append("<script type='text/javascript' src='" + AppBaseDirectory + "\\Resources\\sampledata\\currenttransactiondetail.json'></script>");
                                StringBuilder scriptval = new StringBuilder(HtmlConstants.CURRENT_TRANSACTION_DETAIL_GRID_WIDGET_SCRIPT);
                                scriptval.Replace("CurrentTransactionTable", "CurrentTransactionTable" + page.Identifier);
                                scriptval.Replace("currentShowAll", "currentShowAll" + page.Identifier);
                                scriptval.Replace("filterStatus", "filterStatus" + page.Identifier);
                                scriptval.Replace("ResetGrid", "ResetGrid" + page.Identifier);
                                scriptval.Replace("PrintGrid", "PrintGrid" + page.Identifier);
                                scriptHtmlRenderer.Append(scriptval);

                                pageContent.Replace("currentShowAll", "currentShowAll" + page.Identifier);
                                pageContent.Replace("filterStatus", "filterStatus" + page.Identifier);
                                pageContent.Replace("ResetGrid", "ResetGrid" + page.Identifier);
                                pageContent.Replace("PrintGrid", "PrintGrid" + page.Identifier);
                                pageContent.Replace("{{SelectOption_" + page.Identifier + "_" + widget.Identifier + "}}", selectOption.ToString());
                                pageContent.Replace("CurrentTransactionTable", "CurrentTransactionTable" + page.Identifier);

                                pageContent.Replace("{{AccountTransactionDetails_" + page.Identifier + "_" + widget.Identifier + "}}", string.Empty);
                            }
                            else if (widget.WidgetName == HtmlConstants.TOP_4_INCOME_SOURCE_WIDGET_NAME)
                            {
                                string incomeSourceListJson = "[{ 'Source': 'Salary Transfer', 'CurrentSpend': 3453, 'AverageSpend': 123},{ 'Source': 'Cash Deposit', 'CurrentSpend': 3453, 'AverageSpend': 6123},{ 'Source': 'Profit Earned', 'CurrentSpend': 3453, 'AverageSpend': 6123}, { 'Source': 'Rebete', 'CurrentSpend': 3453, 'AverageSpend': 123}]";
                                if (incomeSourceListJson != string.Empty && validationEngine.IsValidJson(incomeSourceListJson))
                                {
                                    IList<IncomeSources> incomeSources = JsonConvert.DeserializeObject<List<IncomeSources>>(incomeSourceListJson);
                                    StringBuilder incomestr = new StringBuilder();
                                    incomeSources.ToList().ForEach(item =>
                                    {
                                        var tdstring = string.Empty;
                                        if (Int32.Parse(item.CurrentSpend) > Int32.Parse(item.AverageSpend))
                                        {
                                            tdstring = "<span class='fa fa-sort-desc fa-2x text-danger' aria-hidden='true'></span><span class='ml-2'>" + item.AverageSpend + "</span>";
                                        }
                                        else
                                        {
                                            tdstring = "<span class='fa fa-sort-asc fa-2x mt-1' aria-hidden='true' " + "style='position:relative;top:6px;color:limegreen'></span><span class='ml-2'>" + item.AverageSpend + "</span>";
                                        }
                                        incomestr.Append("<tr><td class='float-left'>" + item.Source + "</td>" + "<td> " + item.CurrentSpend + "</td><td>" + tdstring + "</td></tr>");
                                    });
                                    pageContent.Replace("{{IncomeSourceList_" + page.Identifier + "_" + widget.Identifier + "}}", incomestr.ToString());
                                }
                            }
                            else if (widget.WidgetName == HtmlConstants.ANALYTICS_WIDGET_NAME)
                            {
                                FileData fileData = new FileData();
                                fileData.FileName = "analyticschartdata.json";
                                fileData.FileUrl = AppBaseDirectory + "\\Resources\\sampledata\\analyticschartdata.json";
                                SampleFiles.Add(fileData);
                                scriptHtmlRenderer.Append("<script type='text/javascript' src='./" + fileData.FileName + "'></script>");
                                pageContent.Replace("analyticschartcontainer", "analyticschartcontainer" + page.Identifier);
                                scriptHtmlRenderer.Append(HtmlConstants.ANALYTICS_CHART_WIDGET_SCRIPT.Replace("analyticschartcontainer", "analyticschartcontainer" + page.Identifier));
                            }
                            else if (widget.WidgetName == HtmlConstants.SPENDING_TREND_WIDGET_NAME)
                            {
                                FileData fileData = new FileData();
                                fileData.FileName = "spendingtrenddata.json";
                                fileData.FileUrl = AppBaseDirectory + "\\Resources\\sampledata\\spendingtrenddata.json";
                                SampleFiles.Add(fileData);
                                scriptHtmlRenderer.Append("<script type='text/javascript' src='" + fileData.FileName + "'></script>");
                                pageContent.Replace("spendingTrendscontainer", "spendingTrendscontainer" + page.Identifier);
                                scriptHtmlRenderer.Append(HtmlConstants.SPENDING_TREND_CHART_WIDGET_SCRIPT.Replace("spendingTrendscontainer", "spendingTrendscontainer" + page.Identifier));
                            }
                            else if (widget.WidgetName == HtmlConstants.SAVING_TREND_WIDGET_NAME)
                            {
                                FileData fileData = new FileData();
                                fileData.FileName = "savingtrenddata.json";
                                fileData.FileUrl = AppBaseDirectory + "\\Resources\\sampledata\\savingtrenddata.json";
                                SampleFiles.Add(fileData);
                                scriptHtmlRenderer.Append("<script type='text/javascript' src='" + fileData.FileName + "'></script>");
                                pageContent.Replace("savingTrendscontainer", "savingTrendscontainer" + page.Identifier);
                                scriptHtmlRenderer.Append(HtmlConstants.SAVING_TREND_CHART_WIDGET_SCRIPT.Replace("savingTrendscontainer", "savingTrendscontainer" + page.Identifier));
                            }
                            else if (widget.WidgetName == HtmlConstants.REMINDER_AND_RECOMMENDATION_WIDGET_NAME)
                            {
                                string reminderJson = "[{ 'Title': 'Update Missing Inofrmation', 'Action': 'Update' },{ 'Title': 'Your Rewards Video is available', 'Action': 'View' },{ 'Title': 'Payment Due for Home Loan', 'Action': 'Pay' }, { title: 'Need financial planning for savings.', action: 'Call Me' },{ title: 'Subscribe/Unsubscribe Alerts.', action: 'Apply' },{ title: 'Your credit card payment is due now.', action: 'Pay' }]";
                                if (reminderJson != string.Empty && validationEngine.IsValidJson(reminderJson))
                                {
                                    IList<ReminderAndRecommendation> reminderAndRecommendations = JsonConvert.DeserializeObject<List<ReminderAndRecommendation>>(reminderJson);
                                    StringBuilder reminderstr = new StringBuilder();
                                    reminderstr.Append("<div class='row'><div class='col-lg-9'></div><div class='col-lg-3 text-left'><i class='fa fa-caret-left fa-3x float-left text-danger' aria-hidden='true'></i><span class='mt-2 d-inline-block ml-2'>Click</span></div> </div>");
                                    reminderAndRecommendations.ToList().ForEach(item =>
                                    {
                                        reminderstr.Append("<div class='row'><div class='col-lg-9 text-left'><p class='p-1' style='background-color: #dce3dc;'>" +
                                            item.Title + " </p></div><div class='col-lg-3 text-left'> <a><i class='fa fa-caret-left fa-3x float-left " +
                                            "text-danger'></i><span class='mt-2 d-inline-block ml-2'>" + item.Action + "</span></a></div></div>");
                                    });
                                    pageContent.Replace("{{ReminderAndRecommdationDataList_" + page.Identifier + "_" + widget.Identifier + "}}", reminderstr.ToString());
                                }
                            }
                        }
                        else
                        {
                            var dynaWidgets = dynamicWidgets.Where(item => item.Identifier == widget.WidgetId).ToList();
                            if (dynaWidgets.Count > 0)
                            {
                                var dynawidget = dynaWidgets.FirstOrDefault();
                                TenantEntity entity = new TenantEntity();
                                entity.Identifier = dynawidget.EntityId;
                                entity.Name = dynawidget.EntityName;
                                CustomeTheme themeDetails = new CustomeTheme();
                                if (dynawidget.ThemeType == "Default")
                                {
                                    themeDetails = JsonConvert.DeserializeObject<CustomeTheme>(tenantConfiguration.WidgetThemeSetting);
                                }
                                else
                                {
                                    themeDetails = JsonConvert.DeserializeObject<CustomeTheme>(dynawidget.ThemeCSS);
                                }

                                if (dynawidget.WidgetType == HtmlConstants.TABLE_DYNAMICWIDGET)
                                {
                                    pageContent.Replace("{{tableBody_" + page.Identifier + "_" + widget.Identifier + "}}", dynawidget.PreviewData);
                                }
                                else if (dynawidget.WidgetType == HtmlConstants.FORM_DYNAMICWIDGET)
                                {
                                    pageContent.Replace("{{FormData_" + page.Identifier + "_" + widget.Identifier + "}}", dynawidget.PreviewData);
                                }
                                else if (dynawidget.WidgetType == HtmlConstants.LINEGRAPH_DYNAMICWIDGET)
                                {
                                    pageContent.Replace("hiddenLineGraphValue_" + page.Identifier + "_" + widget.Identifier + "", dynawidget.PreviewData);
                                    scriptHtmlRenderer.Append(HtmlConstants.LINE_GRAPH_WIDGET_SCRIPT.Replace("linechartcontainer", "lineGraphcontainer_" + page.Identifier + "_" + widget.Identifier).Replace("hiddenLineGraphData", "hiddenLineGraphData_" + page.Identifier + "_" + widget.Identifier));
                                }
                                else if (dynawidget.WidgetType == HtmlConstants.BARGRAPH_DYNAMICWIDGET)
                                {
                                    pageContent.Replace("hiddenBarGraphValue_" + page.Identifier + "_" + widget.Identifier + "", dynawidget.PreviewData);
                                    scriptHtmlRenderer.Append(HtmlConstants.BAR_GRAPH_WIDGET_SCRIPT.Replace("barchartcontainer", "barGraphcontainer_" + page.Identifier + "_" + widget.Identifier).Replace("hiddenBarGraphData", "hiddenBarGraphData_" + page.Identifier + "_" + widget.Identifier));
                                }
                                else if (dynawidget.WidgetType == HtmlConstants.PICHART_DYNAMICWIDGET)
                                {
                                    pageContent.Replace("hiddenPieChartValue_" + page.Identifier + "_" + widget.Identifier + "", dynawidget.PreviewData);
                                    scriptHtmlRenderer.Append(HtmlConstants.PIE_CHART_WIDGET_SCRIPT.Replace("pieChartcontainer", "pieChartcontainer_" + page.Identifier + "_" + widget.Identifier).Replace("hiddenPieChartData", "hiddenPieChartData_" + page.Identifier + "_" + widget.Identifier));
                                }
                                else if (dynawidget.WidgetType == HtmlConstants.HTML_DYNAMICWIDGET)
                                {
                                    var entityFieldMaps = this.dynamicWidgetRepository.GetEntityFields(dynawidget.EntityId, tenantCode);
                                    string data = this.GetHTMLPreviewData(entity, entityFieldMaps, dynawidget.PreviewData);
                                    pageContent.Replace("{{FormData_" + page.Identifier + "_" + widget.Identifier + "}}", data);
                                }
                            }
                        }
                    }

                    newPageContent.Append(pageContent);
                    if (page.PageTypeName == HtmlConstants.SAVING_ACCOUNT_PAGE || page.PageTypeName == HtmlConstants.CURRENT_ACCOUNT_PAGE)
                    {
                        newPageContent.Append(HtmlConstants.END_DIV_TAG);
                    }
                    newPageContent.Append(HtmlConstants.PAGE_TAB_CONTENT_FOOTER); //to end tab-content div

                    PageHeaderContent.Replace("{{SubTabs}}", SubTabs.ToString());
                    statementPageContent.PageHeaderContent = PageHeaderContent.ToString();
                    statementPageContent.HtmlContent = newPageContent.ToString();
                }

                newStatementPageContents.ToList().ForEach(page =>
                {
                    htmlbody.Append(page.PageHeaderContent);
                    htmlbody.Append(page.HtmlContent);
                    htmlbody.Append(page.PageFooterContent);
                });
                htmlbody.Append(HtmlConstants.CONTAINER_DIV_HTML_FOOTER);

                navbarHtml = navbarHtml.Replace("{{NavItemList}}", navbar.ToString());

                StringBuilder finalHtml = new StringBuilder();
                finalHtml.Append(HtmlConstants.HTML_HEADER);
                finalHtml.Append(navbarHtml);
                finalHtml.Append(htmlbody.ToString());
                finalHtml.Append(HtmlConstants.HTML_FOOTER);
                scriptHtmlRenderer.Append(HtmlConstants.TENANT_LOGO_SCRIPT);
                finalHtml.Replace("{{ChartScripts}}", scriptHtmlRenderer.ToString());

                statementPreviewData.SampleFiles = SampleFiles;
                statementPreviewData.FileContent = finalHtml.ToString();
                return statementPreviewData;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method help to generate statement for customer
        /// </summary>
        /// <param name="customer"> the customer object </param>
        /// <param name="statement"> the statement object </param>
        /// <param name="statementPageContents"> the statement page html content list</param>
        /// <param name="batchMaster"> the batch master object </param>
        /// <param name="batchDetails"> the list of batch details records </param>
        /// <param name="baseURL"> the base URL of API </param>
        public ScheduleLogDetailRecord GenerateStatements(CustomerMasterRecord customer, Statement statement, IList<StatementPageContent> statementPageContents, BatchMasterRecord batchMaster, IList<BatchDetailRecord> batchDetails, string baseURL, string tenantCode, string outputLocation, TenantConfiguration tenantConfiguration, Client client, IList<TenantEntity> tenantEntities)
        {
            ScheduleLogDetailRecord logDetailRecord = new ScheduleLogDetailRecord();
            StringBuilder ErrorMessages = new StringBuilder();
            bool IsFailed = false;
            bool IsSavingOrCurrentAccountPagePresent = false;
            var statementMetadataRecords = new List<StatementMetadataRecord>();

            try
            {
                if (statementPageContents.Count > 0)
                {
                    string currency = string.Empty;
                    var accountrecords = new List<AccountMasterRecord>();
                    var ppsheading = new List<spIAA_PaymentDetail>();
                    var ppsFooter1 = new List<spIAA_PaymentDetail>();
                    var footerImage = new List<spIAA_PaymentDetail>();
                    var savingaccountrecords = new List<AccountMasterRecord>();
                    var curerntaccountrecords = new List<AccountMasterRecord>();
                    var customerMedias = new List<CustomerMediaRecord>();
                    var CustomerAcccountTransactions = new List<AccountTransactionRecord>();
                    var CustomerSavingTrends = new List<SavingTrendRecord>();
                    var productSummary = new List<spIAA_PaymentDetail>();
              

                    using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                    {
                        var pages = statement.Pages.Where(item => item.PageTypeName == HtmlConstants.SAVING_ACCOUNT_PAGE || item.PageTypeName == HtmlConstants.CURRENT_ACCOUNT_PAGE).ToList();
                        IsSavingOrCurrentAccountPagePresent = pages.Count > 0 ? true : false;
                        
                        //collecting all required transaction required for static widgets in financial tenant html statement
                        if (IsSavingOrCurrentAccountPagePresent)
                        {
                            accountrecords = nISEntitiesDataContext.AccountMasterRecords.Where(item => item.CustomerId == customer.Id && item.BatchId == batchMaster.Id && item.TenantCode == tenantCode)?.ToList();
                            if ((accountrecords == null || accountrecords.Count == 0))
                            {
                                ErrorMessages.Append("<li>Account master data is not available for this customer..!!</li>");
                                IsFailed = true;
                            }
                            else
                            {
                                var records = accountrecords.Where(item => item.AccountType.Equals(string.Empty)).ToList();
                                if (records.Count > 0)
                                {
                                    ErrorMessages.Append("<li>Invalid account master data for this customer..!!</li>");
                                    IsFailed = true;
                                }
                            }

                            CustomerAcccountTransactions = nISEntitiesDataContext.AccountTransactionRecords.Where(item => item.CustomerId == customer.Id && item.BatchId == batchMaster.Id && item.TenantCode == tenantCode)?.OrderByDescending(item => item.TransactionDate)?.ToList();

                            CustomerSavingTrends = nISEntitiesDataContext.SavingTrendRecords.Where(item => item.CustomerId == customer.Id && item.BatchId == batchMaster.Id && item.TenantCode == tenantCode).ToList();
                        }

                        //collecting all media information which is required in html statement for some widgets like image, video and static customer information widgets
                        customerMedias = nISEntitiesDataContext.CustomerMediaRecords.Where(item => item.CustomerId == customer.Id && item.StatementId == statement.Identifier && item.BatchId == batchMaster.Id && item.TenantCode == tenantCode)?.ToList();
                    }

                    StringBuilder htmlbody = new StringBuilder();
                    currency = accountrecords.Count > 0 ? accountrecords[0].Currency : string.Empty;
                    string navbarHtml = HtmlConstants.NAVBAR_HTML_FOR_PREVIEW.Replace("{{logo}}", "../common/images/nisLogo.png");
                    navbarHtml = navbarHtml.Replace("{{Today}}", DateTime.UtcNow.ToString("dd MMM yyyy")); //bind current date to html header

                    //get client logo in string format and pass it hidden input tag, so it will be render in right side of header of html statement
                    var clientlogo = client.TenantLogo != null ? client.TenantLogo : "";
                    navbarHtml = navbarHtml + "<input type='hidden' id='TenantLogoImageValue' value='" + clientlogo + "'>";
                    htmlbody.Append(HtmlConstants.CONTAINER_DIV_HTML_HEADER);

                    //this variable is used to bind all script to html statement, which helps to render data on chart and graph widgets
                    StringBuilder scriptHtmlRenderer = new StringBuilder();
                    StringBuilder navbar = new StringBuilder();
                    int subPageCount = 0;
                    string accountNumber = string.Empty; //also use for Subscription
                    string accountType = string.Empty; //also use for vendor name
                    long accountId = 0;
                    string SavingTrendChartJson = string.Empty;
                    string SpendingTrendChartJson = string.Empty;
                    string AnalyticsChartJson = string.Empty;
                    string SavingTransactionGridJson = string.Empty;
                    string CurrentTransactionGridJson = string.Empty;
                    HttpClient httpClient = null;

                    var newStatementPageContents = new List<StatementPageContent>();
                    statementPageContents.ToList().ForEach(it => newStatementPageContents.Add(new StatementPageContent()
                    {
                        Id = it.Id,
                        PageId = it.PageId,
                        PageTypeId = it.PageTypeId,
                        HtmlContent = it.HtmlContent,
                        PageHeaderContent = it.PageHeaderContent,
                        PageFooterContent = it.PageFooterContent,
                        DisplayName = it.DisplayName,
                        TabClassName = it.TabClassName,
                        DynamicWidgets = it.DynamicWidgets
                    }));

                    long FirstPageId = statement.Pages[0].Identifier;
                    for (int i = 0; i < statement.Pages.Count; i++)
                    {
                        var page = statement.Pages[i];
                        StatementPageContent statementPageContent = newStatementPageContents.Where(item => item.PageTypeId == page.PageTypeId && item.Id == i).FirstOrDefault();

                        //sub page count under current page tab
                        subPageCount = 1;
                        if (IsSavingOrCurrentAccountPagePresent)
                        {
                            //This will be applicable only for financial tenant
                            //if cusomer have 2 saving or current account, then 2 tabs will be render to current page in html statement
                            if (page.PageTypeName == HtmlConstants.SAVING_ACCOUNT_PAGE)
                            {
                                savingaccountrecords = accountrecords.Where(item => item.CustomerId == customer.Id && item.BatchId == batchMaster.Id && item.AccountType.ToLower().Contains("saving"))?.ToList();
                                if (savingaccountrecords == null || savingaccountrecords.Count == 0)
                                {
                                    ErrorMessages.Append("<li>Saving account master data is not available for this customer..!!</li>");
                                    IsFailed = true;
                                }
                                subPageCount = savingaccountrecords.Count;
                            }
                            else if (page.PageTypeName == HtmlConstants.CURRENT_ACCOUNT_PAGE)
                            {
                                curerntaccountrecords = accountrecords.Where(item => item.CustomerId == customer.Id && item.BatchId == batchMaster.Id && item.AccountType.ToLower().Contains("current"))?.ToList();
                                if (curerntaccountrecords == null || curerntaccountrecords.Count == 0)
                                {
                                    ErrorMessages.Append("<li>Current account master data is not available for this customer..!!</li>");
                                    IsFailed = true;
                                }
                                subPageCount = curerntaccountrecords.Count;
                            }
                        }

                        StringBuilder SubTabs = new StringBuilder();
                        StringBuilder PageHeaderContent = new StringBuilder(statementPageContent.PageHeaderContent);
                        var dynamicWidgets = statementPageContent.DynamicWidgets;

                        string tabClassName = Regex.Replace((statementPageContent.DisplayName + "-" + page.Identifier), @"\s+", "-");
                        navbar.Append(" <li class='nav-item'><a class='nav-link pt-1 mainNav " + (i == 0 ? "active" : "") + " " + tabClassName + "' href='javascript:void(0);' >" + statementPageContent.DisplayName + "</a> </li> ");
                        string ExtraClassName = i > 0 ? "d-none " + tabClassName : tabClassName;
                        PageHeaderContent.Replace("{{ExtraClass}}", ExtraClassName);
                        PageHeaderContent.Replace("{{DivId}}", tabClassName);

                        StringBuilder newPageContent = new StringBuilder();
                        newPageContent.Append(HtmlConstants.PAGE_TAB_CONTENT_HEADER);

                        for (int x = 0; x < subPageCount; x++)
                        {
                            accountNumber = string.Empty;
                            accountType = string.Empty;

                            //Only for financial tenant
                            if (IsSavingOrCurrentAccountPagePresent)
                            {
                                if (page.PageTypeName == HtmlConstants.SAVING_ACCOUNT_PAGE)
                                {
                                    accountNumber = savingaccountrecords[x].AccountNumber;
                                    accountId = savingaccountrecords[x].Id;
                                    accountType = savingaccountrecords[x].AccountType;
                                }
                                else if (page.PageTypeName == HtmlConstants.CURRENT_ACCOUNT_PAGE)
                                {
                                    accountNumber = curerntaccountrecords[x].AccountNumber;
                                    accountId = curerntaccountrecords[x].Id;
                                    accountType = curerntaccountrecords[x].AccountType;
                                }

                                //start creating sub tabs, append tab name with last 4 digits of account number
                                if (page.PageTypeName == HtmlConstants.SAVING_ACCOUNT_PAGE || page.PageTypeName == HtmlConstants.CURRENT_ACCOUNT_PAGE)
                                {
                                    string lastFourDigisOfAccountNumber = accountNumber.Length > 4 ? accountNumber.Substring(Math.Max(0, accountNumber.Length - 4)) : accountNumber;
                                    if (x == 0)
                                    {
                                        SubTabs.Append("<ul class='nav nav-tabs' style='margin-top:-20px;'>");
                                    }

                                    SubTabs.Append("<li class='nav-item " + (x == 0 ? "active" : "") + "'><a id='tab" + x + "-tab' data-toggle='tab' " + "data-target='#" + (page.PageTypeName == HtmlConstants.SAVING_ACCOUNT_PAGE ? "Saving" : "Current") + "-" + lastFourDigisOfAccountNumber + "-" + "AccountNumber-" + accountId + "' " +
                                        " role='tab' class='nav-link " + (x == 0 ? "active" : "") + "'> Account - " + lastFourDigisOfAccountNumber + "</a></li>");

                                    newPageContent.Append("<div id='" + (page.PageTypeName == HtmlConstants.SAVING_ACCOUNT_PAGE ? "Saving" : "Current") +
                                        "-" + lastFourDigisOfAccountNumber + "-" + "AccountNumber-" + accountId + "' >");

                                    if (page.PageTypeName == HtmlConstants.SAVING_ACCOUNT_PAGE)
                                    {
                                        newPageContent.Append("<input type='hidden' id='SavingAccountId' name='SavingAccountId' value='" + accountId + "'>");
                                    }
                                    else
                                    {
                                        newPageContent.Append("<input type='hidden' id='CurrentAccountId' name='CurrentAccountId' value='" + accountId + "'>");
                                    }

                                    if (x == subPageCount - 1)
                                    {
                                        SubTabs.Append("</ul>");
                                    }
                                }
                            }

                            var pagewidgets = new List<PageWidget>(page.PageWidgets);
                            StringBuilder pageContent = new StringBuilder(statementPageContent.HtmlContent);
                            for (int j = 0; j < pagewidgets.Count; j++)
                            {
                                var widget = pagewidgets[j];
                                if (!widget.IsDynamicWidget)
                                {
                                    if (widget.WidgetName == HtmlConstants.CUSTOMER_INFORMATION_WIDGET_NAME) //Customer Information Widget
                                    {
                                        pageContent.Replace("{{CustomerName}}", (customer.FirstName.Trim() + " " + (customer.MiddleName == string.Empty ? string.Empty : " " + customer.MiddleName.Trim()) + " " + customer.LastName.Trim()));
                                        pageContent.Replace("{{Address1}}", customer.AddressLine1);
                                        string address2 = (customer.AddressLine2 != "" ? customer.AddressLine2 + ", " : "") + (customer.City != "" ? customer.City + ", " : "") + (customer.State != "" ? customer.State + ", " : "") + (customer.Country != "" ? customer.Country + ", " : "") + (customer.Zip != "" ? customer.Zip : "");
                                        pageContent.Replace("{{Address2}}", address2);

                                        var custMedia = customerMedias.Where(item => item.PageId == page.Identifier && item.WidgetId == widget.Identifier)?.ToList()?.FirstOrDefault();
                                        if (custMedia != null && custMedia.VideoURL != string.Empty)
                                        {
                                            pageContent.Replace("{{VideoSource_" + statement.Identifier + "_" + page.Identifier + "_" + widget.Identifier + "}}", custMedia.VideoURL);
                                        }
                                        else
                                        {
                                            var batchDetail = batchDetails.Where(item => item.StatementId == statement.Identifier && item.WidgetId == widget.Identifier && item.PageId == page.Identifier)?.ToList()?.FirstOrDefault();
                                            if (batchDetail != null && batchDetail.VideoURL != string.Empty)
                                            {
                                                pageContent.Replace("{{VideoSource_" + statement.Identifier + "_" + page.Identifier + "_" + widget.Identifier + "}}", batchDetail.VideoURL);
                                            }
                                        }
                                    }
                                    else if (widget.WidgetName == HtmlConstants.PRODUCT_SUMMARY_WIDGET_NAME) //Customer Information Widget
                                    {
                                        if (productSummary != null && productSummary.Count > 0)
                                        {
                                            StringBuilder productSummarySrc = new StringBuilder();
                                            long index = 1;
                                            productSummary.ToList().ForEach(item =>
                                            {
                                                productSummarySrc.Append("<tr><td align='center' valign='center' class='px-1 py-1 fsp-bdr-right fsp-bdr-bottom'>" + index + "</td><td class='fsp-bdr-right fsp-bdr-bottom px-1'>" + item.Commission_Type + "</td>" + "<td class='fsp-bdr-right fsp-bdr-bottom px-1'> " + (item.Prod_Group == "Service Fee" ? "Premium Under Advise Fee" : item.Prod_Group) + "</td> <td class='text-right fsp-bdr-right fsp-bdr-bottom px-1'>R" + item.Display_Amount + "</td><td class='text-center fsp-bdr-bottom px-1'><a  href ='https://facebook.com' target='_blank'><img class='leftarrowlogo' src ='assets/images/leftarrowlogo.png' alt = 'Left Arrow'></a></td></tr>");
                                                index++;
                                            });
                                            pageContent.Replace("{{ProductSummary}}", productSummarySrc.ToString());
                                            pageContent.Replace("{{QueryBtn}}", "assets/images/IfQueryBtn.jpg");
                                            String totalDue = productSummary.FirstOrDefault().Earning_Amount;
                                            totalDue = totalDue.Replace('.', ',');
                                            pageContent.Replace("{{TotalDue}}", "R" + totalDue);
                                            String vatAmount = productSummary.FirstOrDefault().VAT_Amount;
                                            vatAmount = vatAmount.Replace('.', ',');
                                            pageContent.Replace("{{VATDue}}", "R" + vatAmount);
                                            double grandTotalDue = (Convert.ToDouble(productSummary.FirstOrDefault().Earning_Amount) + Convert.ToDouble(productSummary.FirstOrDefault().VAT_Amount));
                                            String grandTotalDueStr = grandTotalDue.ToString().Replace(',', '.');
                                            pageContent.Replace("{{GrandTotalDue}}", "R" + grandTotalDueStr);
                                            double ppsPayment = grandTotalDue;
                                            pageContent.Replace("{{PPSPayment}}", "-R" + grandTotalDueStr);
                                            String Balance = Convert.ToDouble((grandTotalDue - ppsPayment)).ToString("F2").Replace(',', '.');
                                            pageContent.Replace("{{Balance}}", "R" + Balance);
                                        }
                                    }
                                    else if (widget.WidgetName == HtmlConstants.PAYMENT_SUMMARY_WIDGET_NAME) //Payment Summary Widget
                                    {
                                        pageContent.Replace("{{IntTotal}}", productSummary.First().Earning_Amount);
                                        pageContent.Replace("{{Vat}}", productSummary.First().VAT_Amount);
                                        pageContent.Replace("{{TotalDue}}", (Convert.ToDouble(productSummary.First().Earning_Amount) + Convert.ToDouble(productSummary.First().VAT_Amount)).ToString());

                                        // Format the date to month-year format                                                   
                                        pageContent.Replace("{{IntTotalDate}}", productSummary.First().POSTED_DATE.ToString("MMMM yyyy"));
                                        // Format the date with a custom format
                                        string formattedOrdinalDate = FormatDateWithOrdinal(productSummary.First().POSTED_DATE);
                                        pageContent.Replace("{{IntPostedDate}}", formattedOrdinalDate);

                //                        pageContent.Replace("{{IntTotal}}", productSummary.First().GrandTotal.ToString());
                //                        pageContent.Replace("{{Vat}}", productSummary.First().FeesPaid.ToString());
                //                        pageContent.Replace("{{TotalDue}}", (Convert.ToDouble(productSummary.First().GrandTotal) +
                //Convert.ToDouble(accountrecords.First().FeesPaid)).ToString());
                                    }

                                    else if (widget.WidgetName == HtmlConstants.PPS_HEADING_WIDGET_NAME)
                                    {
                                        //                        pageContent.Replace("{{IntTotal}}", ppsheading.First().GrandTotal.ToString());
                                        //                        pageContent.Replace("{{Vat}}", ppsheading.First().FeesPaid.ToString());
                                        //                        pageContent.Replace("{{TotalDue}}", (Convert.ToDouble(accountrecords.First().GrandTotal) +
                                        //Convert.ToDouble(accountrecords.First().FeesPaid)).ToString());
                                    }

                                    else if (widget.WidgetName == HtmlConstants.PPS_FOOTER1_WIDGET_NAME)
                                    {
                                        //                        pageContent.Replace("{{IntTotal}}", ppsheading.First().GrandTotal.ToString());
                                        //                        pageContent.Replace("{{Vat}}", ppsheading.First().FeesPaid.ToString());
                                        //                        pageContent.Replace("{{TotalDue}}", (Convert.ToDouble(accountrecords.First().GrandTotal) +
                                        //Convert.ToDouble(accountrecords.First().FeesPaid)).ToString());
                                    }

                                    else if (widget.WidgetName == HtmlConstants.FOOTER_IMAGE_WIDGET_NAME)
                                    {
                                        //                        pageContent.Replace("{{IntTotal}}", ppsheading.First().GrandTotal.ToString());
                                        //                        pageContent.Replace("{{Vat}}", ppsheading.First().FeesPaid.ToString());
                                        //                        pageContent.Replace("{{TotalDue}}", (Convert.ToDouble(accountrecords.First().GrandTotal) +
                                        //Convert.ToDouble(accountrecords.First().FeesPaid)).ToString());
                                    }

                                    else if (widget.WidgetName == HtmlConstants.DETAILED_TRANSACTIONS_WIDGET_NAME)
                                    {
                                       string transactionListJson = "[{'INT_EXT_REF':'2164250','Int_Name':'Mr SCHOELER','Client_Name':'Kruger Van Heerden','Member_Ref':124556686,'Policy_Ref':5596100,'Description':'Safe Custody Service Fee','Commission_Type':'Safe Custody Fee','POSTED_DATE':'20-Mar-23','Display_Amount':'17.55','Query_Link':'https://facebook.com','TYPE':'Fiduciary_Data','Prod_Group':'Safe Custody Fee'},{'INT_EXT_REF':'2164250','Int_Name':'Yvonne Van Heerden','Client_Name':'Mr SCHOELER','Member_Ref':124556686,'Policy_Ref':'5596100','Description':'Safe Custody Service Fee VAT','Commission_Type':'Safe Custody Fee','POSTED_DATE':'20-Mar-23','Display_Amount':'2.63','Query_Link':'https://facebook.com','TYPE':'Fiduciary_Data','Prod_Group':'Safe Custody Fee'},{'INT_EXT_REF':'124411745','Int_Name':'Kruger Van Heerden','Client_Name':'DR N J Olivier','Member_Ref':'1217181','Policy_Ref':'5524069','Description':'Safe Custody Service Fee','Commission_Type':'Safe Custody Fee','POSTED_DATE':'20-Mar-23','Display_Amount':'17.55','Query_Link':'https://facebook.com','TYPE':'Fiduciary_Data','Prod_Group':'Safe Custody Fee'},{'INT_EXT_REF':'124411745','Int_Name':'Kruger Van Heerden','Client_Name':'DR N J Olivier','Member_Ref':'124556686','Policy_Ref':'5596100','Description':'Safe Custody Service Fee VAT','Commission_Type':'Safe Custody Fee','POSTED_DATE':'20-Mar-23','Display_Amount':'2.63','Query_Link':'https://facebook.com','TYPE':'Fiduciary_Data','Prod_Group':'VAT'}]";
                                        double TotalPostedAmount = 0;
                                        if (transactionListJson != string.Empty && validationEngine.IsValidJson(transactionListJson))
                                        {
                                            IList<spIAA_PaymentDetail> transaction = JsonConvert.DeserializeObject<List<spIAA_PaymentDetail>>(transactionListJson);
                                            StringBuilder detailedTransactionSrc = new StringBuilder();
                                            var records = transaction.GroupBy(gptransactionitem => gptransactionitem.INT_EXT_REF).ToList();
                                            records?.ForEach(transactionitem =>
                                            {
                                                detailedTransactionSrc.Append("<div class='px-50'><div class='prouct-table-block'><div class='text-left fsp-transaction-title font-weight-bold mb-3'>Intermediary:  " + transactionitem.FirstOrDefault().INT_EXT_REF + " " + transactionitem.FirstOrDefault().Int_Name + "</div><table width='100%' cellpadding='0' cellspacing='0'> <tr><th class='font-weight-bold text-white'>Client name</th> <th class='font-weight-bold text-white text-center pe-0 bdr-r-0'>Member<br /> number</th> <th class='font-weight-bold text-white text-center'>Will<br/> number</th> <th class='font-weight-bold text-white text-center'>Fiduciary fees</th> <th class='font-weight-bold text-white text-center'>Commission<br /> type</th> <th class='font-weight-bold text-white text-center'>Posted date</th> <th class='font-weight-bold text-white text-center'>Posted amount</th> <th class='font-weight-bold text-white'>Query</th> </tr> ");
                                                pageContent.Replace("{{QueryBtnImgLink}}", "https://www.google.com/");
                                                pageContent.Replace("{{QueryBtn}}", "../common/images/IfQueryBtn.jpg");
                                                transaction.Where(witem => witem.INT_EXT_REF == transactionitem.FirstOrDefault().INT_EXT_REF).ToList().ForEach(item =>
                                                {
                                                    detailedTransactionSrc.Append("<tr><td align = 'center' valign = 'center' class='px-1 py-1 fsp-bdr-right fsp-bdr-bottom'>" +
                                                            item.Client_Name + "</td><td class= 'fsp-bdr-right fsp-bdr-bottom px-1'>" + item.Member_Ref + "</td><td class= 'fsp-bdr-right fsp-bdr-bottom px-1'> " + item.Policy_Ref + "</td><td class= 'text-right fsp-bdr-right fsp-bdr-bottom px-1'>" + (item.Description == "Commission Service Fee" ? "Premium Under Advise Fee" : item.Description)  + "</td><td class= 'text-center fsp-bdr-right fsp-bdr-bottom px-1'>" + item.Commission_Type + "</td><td class= 'text-center fsp-bdr-right fsp-bdr-bottom px-1'>" + item.POSTED_DATE.ToString("dd-MMM-yyyy") + "</td><td class= 'text-center fsp-bdr-right fsp-bdr-bottom px-1'> R" + item.Display_Amount + "</td><td class= 'text-center fsp-bdr-bottom px-1'><a href ='https://www.google.com/' target ='_blank'><img class='leftarrowlogo' src='assets/images/leftarrowlogo.png' alt='Left Arrow'></a></td></tr>");
                                                    TotalPostedAmount += ((item.TYPE == "Fiduciary_Data") && (item.Prod_Group != "VAT"))?  (Convert.ToDouble(item.Display_Amount)): 0.0;
                                                });
                                                string TotalPostedAmountR = (TotalPostedAmount == 0) ? "0.00" : ("R" + TotalPostedAmount.ToString());
                                                detailedTransactionSrc.Append("<tr> <td align='center' valign='center' class='px-1 py-1 fsp-bdr-right fsp-bdr-bottom'></td> <td class='fsp-bdr-right fsp-bdr-bottom px-1 py-1'></td> <td class='fsp-bdr-right fsp-bdr-bottom px-1 py-1'></td> <td class='text-right fsp-bdr-right fsp-bdr-bottom px-1 py-1'></td> <td class='text-center fsp-bdr-right fsp-bdr-bottom px-1 py-1'><br /></td> <td class='text-center fsp-bdr-right fsp-bdr-bottom px-1 py-1'></td> <td class='text-center fsp-bdr-right fsp-bdr-bottom px-1 py-1'>" + TotalPostedAmountR + "</td> <td class='text-center fsp-bdr-bottom px-1'><a href='https://www.google.com/' target = '_blank' ><img src='assets/images/leftarrowlogo.png'></a></td> </tr></table><div class='text-right w-100 pt-3'><a href='https://www.google.com/' target = '_blank'><img src='assets/images/click-print-stmt-btn.jpg'></a></div></div></div></div>");
                                                TotalPostedAmount = 0;

                                            });
                                            pageContent.Replace("{{detailedTransaction}}", detailedTransactionSrc.ToString());
                                        }
                                    }

                                    else if (widget.WidgetName == HtmlConstants.ACCOUNT_INFORMATION_WIDGET_NAME) //Account Information Widget
                                    {
                                        StringBuilder AccDivData = new StringBuilder();
                                        AccDivData.Append("<div class='list-row-small ht70px'><div class='list-middle-row'> <div class='list-text'>Statement Date" + "</div><label class='list-value mb-0'>" + Convert.ToDateTime(customer.StatementDate).ToShortDateString() + "</label></div></div>");

                                        AccDivData.Append("<div class='list-row-small ht70px'><div class='list-middle-row'> <div class='list-text'>Statement Period" + "</div><label class='list-value mb-0'>" + customer.StatementPeriod + "</label></div></div>");

                                        AccDivData.Append("<div class='list-row-small ht70px'><div class='list-middle-row'> <div class='list-text'>Cusomer ID" + "</div><label class='list-value mb-0'>" + customer.CustomerCode + "</label></div></div>");

                                        AccDivData.Append("<div class='list-row-small ht70px'><div class='list-middle-row'> <div class='list-text'>RM Name" + "</div><label class='list-value mb-0'>" + customer.RmName + "</label></div></div>");

                                        AccDivData.Append("<div class='list-row-small ht70px'><div class='list-middle-row'> <div class='list-text'>RM Contact Number" + "</div><label class='list-value mb-0'>" + customer.RmContactNumber + "</label></div></div>");
                                        pageContent.Replace("{{AccountInfoData_" + page.Identifier + "_" + widget.Identifier + "}}", AccDivData.ToString());
                                    }
                                    else if (widget.WidgetName == HtmlConstants.IMAGE_WIDGET_NAME) //Image Widget
                                    {
                                        var imgAssetFilepath = string.Empty;
                                        if (widget.WidgetSetting != string.Empty && validationEngine.IsValidJson(widget.WidgetSetting))
                                        {
                                            dynamic widgetSetting = JObject.Parse(widget.WidgetSetting);
                                            if (widgetSetting.isPersonalize == false) //Is not dynamic image, then assign selected image from asset library
                                            {
                                                var asset = assetLibraryRepository.GetAssets(new AssetSearchParameter { Identifier = widgetSetting.AssetId, SortParameter = new SortParameter { SortColumn = "Id" } }, tenantCode).ToList()?.FirstOrDefault();
                                                if (asset != null)
                                                {
                                                    var path = asset.FilePath.ToString();
                                                    var fileName = asset.Name;
                                                    var imagePath = outputLocation + "\\Statements\\" + batchMaster.Id + "\\" + customer.Id;
                                                    if (File.Exists(path) && !File.Exists(imagePath + "\\" + fileName))
                                                    {
                                                        File.Copy(path, Path.Combine(imagePath, fileName));
                                                    }
                                                    imgAssetFilepath = "./" + fileName;
                                                }
                                                else
                                                {
                                                    ErrorMessages.Append("<li>Image asset file not found in asset library for Page: " + page.Identifier + " and Widget: " + widget.Identifier + " for image widget..!!</li>");
                                                    IsFailed = true;
                                                }
                                            }
                                            else //Is dynamic image, then assign it from database 
                                            {
                                                var custMedia = customerMedias.Where(item => item.PageId == page.Identifier && item.WidgetId == widget.Identifier)?.ToList()?.FirstOrDefault(); //error if multiple records
                                                if (custMedia != null && custMedia.ImageURL != string.Empty)
                                                {
                                                    imgAssetFilepath = custMedia.ImageURL;
                                                }
                                                else
                                                {
                                                    var batchDetail = batchDetails.Where(item => item.StatementId == statement.Identifier && item.WidgetId == widget.Identifier && item.PageId == page.Identifier)?.ToList()?.FirstOrDefault();
                                                    if (batchDetail != null && batchDetail.ImageURL != string.Empty)
                                                    {
                                                        imgAssetFilepath = batchDetail.ImageURL;
                                                    }
                                                    else
                                                    {
                                                        ErrorMessages.Append("<li>Image not found for Page: " + page.Identifier + " and Widget: " + widget.Identifier + " for image widget..!!</li>");
                                                        IsFailed = true;
                                                    }
                                                }
                                            }
                                            pageContent.Replace("{{ImageSource_" + statement.Identifier + "_" + page.Identifier + "_" + widget.Identifier + "}}", imgAssetFilepath);
                                        }
                                        else
                                        {
                                            ErrorMessages.Append("<li>Image widget configuration is missing for Page: " + page.Identifier + " and Widget: " + widget.Identifier + "!!</li>");
                                            IsFailed = true;
                                        }
                                    }
                                    else if (widget.WidgetName == HtmlConstants.VIDEO_WIDGET_NAME) //Video widget
                                    {
                                        var vdoAssetFilepath = string.Empty;
                                        if (widget.WidgetSetting != string.Empty && validationEngine.IsValidJson(widget.WidgetSetting))
                                        {
                                            dynamic widgetSetting = JObject.Parse(widget.WidgetSetting);
                                            if (widgetSetting.isEmbedded == true)//If embedded then assigned it it from widget config json source url
                                            {
                                                vdoAssetFilepath = widgetSetting.SourceUrl;
                                            }
                                            else if (widgetSetting.isPersonalize == false && widgetSetting.isEmbedded == false) //If not dynamic video, then assign selected video from asset library
                                            {
                                                var asset = assetLibraryRepository.GetAssets(new AssetSearchParameter { Identifier = widgetSetting.AssetId, SortParameter = new SortParameter { SortColumn = "Id" } }, tenantCode).ToList()?.FirstOrDefault();
                                                if (asset != null)
                                                {
                                                    var path = asset.FilePath.ToString();
                                                    var fileName = asset.Name;
                                                    var videoPath = outputLocation + "\\Statements\\" + batchMaster.Id + "\\" + customer.Id;
                                                    if (File.Exists(path) && !File.Exists(videoPath + "\\" + fileName))
                                                    {
                                                        File.Copy(path, Path.Combine(videoPath, fileName));
                                                    }
                                                    vdoAssetFilepath = "./" + fileName;
                                                }
                                                else
                                                {
                                                    ErrorMessages.Append("<li>Video asset file not found in asset library for Page: " + page.Identifier + " and Widget: " + widget.Identifier + " for video widget..!!</li>");
                                                    IsFailed = true;
                                                }
                                            }
                                            else //If dynamic video, then assign it from database 
                                            {
                                                var custMedia = customerMedias.Where(item => item.PageId == page.Identifier && item.WidgetId == widget.Identifier)?.ToList()?.FirstOrDefault();
                                                if (custMedia != null && custMedia.VideoURL != string.Empty)
                                                {
                                                    vdoAssetFilepath = custMedia.VideoURL;
                                                }
                                                else
                                                {
                                                    var batchDetail = batchDetails.Where(item => item.StatementId == statement.Identifier && item.WidgetId == widget.Identifier && item.PageId == page.Identifier)?.ToList()?.FirstOrDefault();
                                                    if (batchDetail != null && batchDetail.VideoURL != string.Empty)
                                                    {
                                                        vdoAssetFilepath = batchDetail.VideoURL;
                                                    }
                                                    else
                                                    {
                                                        ErrorMessages.Append("<li>Video not found for Page: " + page.Identifier + " and Widget: " + widget.Identifier + " for video widget..!!</li>");
                                                        IsFailed = true;
                                                    }
                                                }
                                            }
                                            pageContent.Replace("{{VideoSource_" + statement.Identifier + "_" + page.Identifier + "_" + widget.Identifier + "}}", vdoAssetFilepath);
                                        }
                                        else
                                        {
                                            ErrorMessages.Append("<li>Video widget configuration is missing for Page: " + page.Identifier + " and Widget: " + widget.Identifier + "!!</li>");
                                            IsFailed = true;
                                        }
                                    }
                                    else if (widget.WidgetName == HtmlConstants.SUMMARY_AT_GLANCE_WIDGET_NAME) //Summary at Glance Widget
                                    {
                                        if (accountrecords != null && accountrecords.Count > 0)
                                        {
                                            StringBuilder accSummary = new StringBuilder();
                                            var accRecords = accountrecords.GroupBy(item => item.AccountType).ToList();
                                            accRecords.ToList().ForEach(acc =>
                                            {
                                                accSummary.Append("<tr><td>" + acc.FirstOrDefault().AccountType + "</td><td>" + acc.FirstOrDefault().Currency + "</td><td>" + acc.Sum(it => it.Balance).ToString() + "</td></tr>");
                                            });
                                            pageContent.Replace("{{AccountSummary_" + page.Identifier + "_" + widget.Identifier + "}}", accSummary.ToString());
                                        }
                                        else
                                        {
                                            ErrorMessages.Append("<li>Account master data is not available related to Summary at Glance widget..!!</li>");
                                            IsFailed = true;
                                        }
                                    }
                                    else if (widget.WidgetName == HtmlConstants.CURRENT_AVAILABLE_BALANCE_WIDGET_NAME)
                                    {
                                        if (accountrecords != null && accountrecords.Count > 0)
                                        {
                                            var currentAccountRecords = accountrecords.Where(item => item.CustomerId == customer.Id && item.BatchId == batchMaster.Id && item.AccountType.ToLower().Contains("current") && item.Id == accountId)?.ToList();
                                            if (currentAccountRecords != null && currentAccountRecords.Count > 0)
                                            {
                                                var records = currentAccountRecords.GroupBy(item => item.AccountType).ToList();
                                                records?.ToList().ForEach(acc =>
                                                {
                                                    var accountIndicatorClass = acc.FirstOrDefault().Indicator.ToLower().Equals("up") ? "fa fa-sort-asc text-success" : "fa fa-sort-desc text-danger";
                                                    pageContent.Replace("{{AccountIndicatorClass}}", accountIndicatorClass);
                                                    pageContent.Replace("{{TotalValue_" + page.Identifier + "_" + widget.Identifier + "}}", (currency + " " + acc.Sum(it => it.GrandTotal).ToString()));
                                                    pageContent.Replace("{{TotalDeposit_" + page.Identifier + "_" + widget.Identifier + "}}", (currency + " " + acc.Sum(it => it.TotalDeposit).ToString()));
                                                    pageContent.Replace("{{TotalSpend_" + page.Identifier + "_" + widget.Identifier + "}}", (currency + " " + acc.Sum(it => it.TotalSpend).ToString()));
                                                    pageContent.Replace("{{Savings_" + page.Identifier + "_" + widget.Identifier + "}}", (currency + " " + acc.Sum(it => it.ProfitEarned).ToString()));
                                                });
                                            }
                                            else
                                            {
                                                ErrorMessages.Append("<li>Current Account master data is not available related to Current Available Balance widget..!!</li>");
                                                IsFailed = true;
                                            }
                                        }
                                        else
                                        {
                                            ErrorMessages.Append("<li>Account master data is not available related to Current Available Balance widget..!!</li>");
                                            IsFailed = true;
                                        }
                                    }
                                    else if (widget.WidgetName == HtmlConstants.SAVING_AVAILABLE_BALANCE_WIDGET_NAME)
                                    {
                                        if (accountrecords != null && accountrecords.Count > 0)
                                        {
                                            var savingAccountRecords = accountrecords.Where(item => item.CustomerId == customer.Id && item.BatchId == batchMaster.Id && item.AccountType.ToLower().Contains("saving") && item.Id == accountId)?.ToList();
                                            if (savingAccountRecords != null && savingAccountRecords.Count > 0)
                                            {
                                                var records = savingAccountRecords.GroupBy(item => item.AccountType).ToList();
                                                records?.ToList().ForEach(acc =>
                                                {
                                                    var accountIndicatorClass = acc.FirstOrDefault().Indicator.ToLower().Equals("up") ? "fa fa-sort-asc text-success" : "fa fa-sort-desc text-danger";
                                                    pageContent.Replace("{{AccountIndicatorClass}}", accountIndicatorClass);
                                                    pageContent.Replace("{{TotalValue_" + page.Identifier + "_" + widget.Identifier + "}}", (currency + " " + acc.Sum(it => it.GrandTotal).ToString()));
                                                    pageContent.Replace("{{TotalDeposit_" + page.Identifier + "_" + widget.Identifier + "}}", (currency + " " + acc.Sum(it => it.TotalDeposit).ToString()));
                                                    pageContent.Replace("{{TotalSpend_" + page.Identifier + "_" + widget.Identifier + "}}", (currency + " " + acc.Sum(it => it.TotalSpend).ToString()));
                                                    pageContent.Replace("{{Savings_" + page.Identifier + "_" + widget.Identifier + "}}", (currency + " " + acc.Sum(it => it.ProfitEarned).ToString()));
                                                });
                                            }
                                            else
                                            {
                                                ErrorMessages.Append("<li>Saving Account master data is not available related to Saving Available Balance widget..!!</li>");
                                                IsFailed = true;
                                            }
                                        }
                                        else
                                        {
                                            ErrorMessages.Append("<li>Account master data is not available related to Saving Available Balance widget..!!</li>");
                                            IsFailed = true;
                                        }
                                    }
                                    else if (widget.WidgetName == HtmlConstants.SAVING_TRANSACTION_WIDGET_NAME)
                                    {
                                        var accountTransactions = CustomerAcccountTransactions.Where(item => item.CustomerId == customer.Id && item.BatchId == batchMaster.Id && item.AccountType.ToLower().Contains("saving") && item.AccountId == accountId && item.TenantCode == tenantCode)?.OrderByDescending(item => item.TransactionDate)?.ToList();

                                        StringBuilder transaction = new StringBuilder();
                                        StringBuilder selectOption = new StringBuilder();
                                        if (accountTransactions != null && accountTransactions.Count > 0)
                                        {
                                            IList<AccountTransaction> transactions = new List<AccountTransaction>();
                                            pageContent.Replace("{{Currency}}", currency);
                                            //get saving transaction data in the list and then convert it to json format string 
                                            //and store it as file at same directory of html statement file
                                            accountTransactions.ToList().ForEach(trans =>
                                            {
                                                AccountTransaction accountTransaction = new AccountTransaction();
                                                accountTransaction.AccountType = trans.AccountType;
                                                accountTransaction.TransactionDate = Convert.ToDateTime(trans.TransactionDate).ToShortDateString();
                                                accountTransaction.TransactionType = trans.TransactionType;
                                                accountTransaction.FCY = trans.FCY;
                                                accountTransaction.Narration = trans.Narration;
                                                accountTransaction.LCY = trans.LCY;
                                                accountTransaction.CurrentRate = trans.CurrentRate;
                                                transactions.Add(accountTransaction);
                                            });
                                            string savingtransactionjson = JsonConvert.SerializeObject(transactions);
                                            if (savingtransactionjson != null && savingtransactionjson != string.Empty)
                                            {
                                                var distinctNaration = accountTransactions.Select(item => item.Narration).Distinct().ToList();
                                                distinctNaration.ToList().ForEach(item =>
                                                {
                                                    selectOption.Append("<option value='" + item + "'> " + item + "</option>");
                                                });

                                                SavingTransactionGridJson = "savingtransactiondata" + accountId + page.Identifier + "=" + savingtransactionjson;
                                                this.utility.WriteToJsonFile(SavingTransactionGridJson, "savingtransactiondetail" + accountId + page.Identifier + ".json", batchMaster.Id, customer.Id, outputLocation);
                                                scriptHtmlRenderer.Append("<script type='text/javascript' src='./savingtransactiondetail" + accountId + page.Identifier + ".json'></script>");

                                                StringBuilder scriptval = new StringBuilder(HtmlConstants.SAVING_TRANSACTION_DETAIL_GRID_WIDGET_SCRIPT);
                                                scriptval.Replace("SavingTransactionTable", "SavingTransactionTable" + accountId + page.Identifier);
                                                scriptval.Replace("savingtransactiondata", "savingtransactiondata" + accountId + page.Identifier);
                                                scriptval.Replace("savingShowAll", "savingShowAll" + accountId + page.Identifier);
                                                scriptval.Replace("filterStatus", "filterStatus" + accountId + page.Identifier);
                                                scriptval.Replace("ResetGrid", "ResetGrid" + accountId + page.Identifier);
                                                scriptval.Replace("PrintGrid", "PrintGrid" + accountId + page.Identifier);
                                                scriptval.Replace("savingtransactionRadio", "savingtransactionRadio" + accountId + page.Identifier);
                                                scriptHtmlRenderer.Append(scriptval);

                                                pageContent.Replace("savingtransactiondata", "savingtransactiondata" + accountId + page.Identifier);
                                                pageContent.Replace("savingShowAll", "savingShowAll" + accountId + page.Identifier);
                                                pageContent.Replace("filterStatus", "filterStatus" + accountId + page.Identifier);
                                                pageContent.Replace("ResetGrid", "ResetGrid" + accountId + page.Identifier);
                                                pageContent.Replace("PrintGrid", "PrintGrid" + accountId + page.Identifier);
                                                pageContent.Replace("savingtransactionRadio", "savingtransactionRadio" + accountId + page.Identifier);
                                                pageContent.Replace("{{SelectOption_" + page.Identifier + "_" + widget.Identifier + "}}", selectOption.ToString());
                                                pageContent.Replace("SavingTransactionTable", "SavingTransactionTable" + accountId + page.Identifier);
                                            }
                                            else
                                            {
                                                transaction.Append("<tr><td colspan='7' class='text-danger text-center'><span>No data available</span></td></tr>");
                                            }
                                        }
                                        else
                                        {
                                            transaction.Append("<tr><td colspan='7' class='text-danger text-center'><span>No data available</span></td></tr>");
                                        }

                                        pageContent.Replace("{{AccountTransactionDetails_" + page.Identifier + "_" + widget.Identifier + "}}", transaction.ToString());
                                    }
                                    else if (widget.WidgetName == HtmlConstants.CURRENT_TRANSACTION_WIDGET_NAME)
                                    {
                                        var accountTransactions = CustomerAcccountTransactions.Where(item => item.CustomerId == customer.Id && item.BatchId == batchMaster.Id && item.AccountType.ToLower().Contains("current") && item.AccountId == accountId && item.TenantCode == tenantCode)?.OrderByDescending(item => item.TransactionDate)?.ToList();

                                        StringBuilder transaction = new StringBuilder();
                                        StringBuilder selectOption = new StringBuilder();
                                        if (accountTransactions != null && accountTransactions.Count > 0)
                                        {
                                            IList<AccountTransaction> transactions = new List<AccountTransaction>();
                                            pageContent.Replace("{{Currency}}", currency);
                                            //get saving transaction data in the list and then convert it to json format string
                                            //and store it as json file at same directory of html statement file
                                            accountTransactions.ToList().ForEach(trans =>
                                            {
                                                AccountTransaction accountTransaction = new AccountTransaction();
                                                accountTransaction.AccountType = trans.AccountType;
                                                accountTransaction.TransactionDate = Convert.ToDateTime(trans.TransactionDate).ToShortDateString();
                                                accountTransaction.TransactionType = trans.TransactionType;
                                                accountTransaction.FCY = trans.FCY;
                                                accountTransaction.Narration = trans.Narration;
                                                accountTransaction.LCY = trans.LCY;
                                                accountTransaction.CurrentRate = trans.CurrentRate;
                                                transactions.Add(accountTransaction);
                                            });
                                            string currenttransactionjson = JsonConvert.SerializeObject(transactions);
                                            if (currenttransactionjson != null && currenttransactionjson != string.Empty)
                                            {
                                                var distinctNaration = accountTransactions.Select(item => item.Narration).Distinct().ToList();
                                                distinctNaration.ToList().ForEach(item =>
                                                {
                                                    selectOption.Append("<option value='" + item + "'> " + item + "</option>");
                                                });

                                                CurrentTransactionGridJson = "currenttransactiondata" + accountId + page.Identifier + "=" + currenttransactionjson;
                                                this.utility.WriteToJsonFile(CurrentTransactionGridJson, "currenttransactiondetail" + accountId + page.Identifier + ".json", batchMaster.Id, customer.Id, outputLocation);
                                                scriptHtmlRenderer.Append("<script type='text/javascript' src='./currenttransactiondetail" + accountId + page.Identifier + ".json'></script>");

                                                StringBuilder scriptval = new StringBuilder(HtmlConstants.CURRENT_TRANSACTION_DETAIL_GRID_WIDGET_SCRIPT);
                                                scriptval.Replace("CurrentTransactionTable", "CurrentTransactionTable" + accountId + page.Identifier);
                                                scriptval.Replace("currenttransactiondata", "currenttransactiondata" + accountId + page.Identifier);
                                                scriptval.Replace("currentShowAll", "currentShowAll" + accountId + page.Identifier);
                                                scriptval.Replace("filterStatus", "filterStatus" + accountId + page.Identifier);
                                                scriptval.Replace("ResetGrid", "ResetGrid" + accountId + page.Identifier);
                                                scriptval.Replace("PrintGrid", "PrintGrid" + accountId + page.Identifier);
                                                scriptval.Replace("currenttransactionRadio", "currenttransactionRadio" + accountId + page.Identifier);
                                                scriptHtmlRenderer.Append(scriptval);

                                                pageContent.Replace("currentShowAll", "currentShowAll" + accountId + page.Identifier);
                                                pageContent.Replace("filterStatus", "filterStatus" + accountId + page.Identifier);
                                                pageContent.Replace("ResetGrid", "ResetGrid" + accountId + page.Identifier);
                                                pageContent.Replace("PrintGrid", "PrintGrid" + accountId + page.Identifier);
                                                pageContent.Replace("currenttransactionRadio", "currenttransactionRadio" + accountId + page.Identifier);
                                                pageContent.Replace("{{SelectOption_" + page.Identifier + "_" + widget.Identifier + "}}", selectOption.ToString());
                                                pageContent.Replace("CurrentTransactionTable", "CurrentTransactionTable" + accountId + page.Identifier);
                                            }
                                            else
                                            {
                                                transaction.Append("<tr><td colspan='7' class='text-danger text-center'><span>No data available</span></td></tr>");
                                            }
                                        }
                                        else
                                        {
                                            transaction.Append("<tr><td colspan='7' class='text-danger text-center'><span>No data available</span></td></tr>");
                                        }
                                        pageContent.Replace("{{AccountTransactionDetails_" + page.Identifier + "_" + widget.Identifier + "}}", transaction.ToString());
                                    }
                                    else if (widget.WidgetName == HtmlConstants.TOP_4_INCOME_SOURCE_WIDGET_NAME)
                                    {
                                        using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                                        {
                                            var top4IncomeSources = nISEntitiesDataContext.Top4IncomeSourcesRecord.Where(item => item.CustomerId == customer.Id && item.BatchId == batchMaster.Id && item.TenantCode == tenantCode)?.OrderByDescending(item => item.CurrentSpend)?.Take(4)?.ToList();

                                            StringBuilder incomeSources = new StringBuilder();
                                            if (top4IncomeSources != null && top4IncomeSources.Count > 0)
                                            {
                                                top4IncomeSources.ToList().ForEach(src =>
                                                {
                                                    var tdstring = string.Empty;
                                                    if (src.CurrentSpend > src.AverageSpend)
                                                    {
                                                        tdstring = "<span class='fa fa-sort-desc fa-2x text-danger' aria-hidden='true'></span><span class='ml-2'>" +
                                                        src.AverageSpend + "</span>";
                                                    }
                                                    else
                                                    {
                                                        tdstring = "<span class='fa fa-sort-asc fa-2x mt-1' aria-hidden='true' " +
                                                        "style='position:relative;top:6px;color:limegreen'></span><span class='ml-2'>" + src.AverageSpend + "</span>";
                                                    }
                                                    incomeSources.Append("<tr><td class='float-left'>" + src.Source + "</td>" + "<td> " + src.CurrentSpend + "</td><td>" +
                                                        tdstring + "</td></tr>");
                                                });
                                            }
                                            else
                                            {
                                                incomeSources.Append("<tr><td colspan='3' class='text-danger text-center'><div style='margin-top: 20px;'>No data available</div>" +
                                                    "</td></tr>");
                                            }
                                            pageContent.Replace("{{IncomeSourceList_" + page.Identifier + "_" + widget.Identifier + "}}", incomeSources.ToString());
                                        }


                                    }
                                    else if (widget.WidgetName == HtmlConstants.ANALYTICS_WIDGET_NAME)
                                    {
                                        if (accountrecords.Count > 0)
                                        {
                                            IList<AccountMasterRecord> accounts = new List<AccountMasterRecord>();
                                            var records = accountrecords.GroupBy(item => item.AccountType).ToList();

                                            //get analytics chart widget data, convert it into json string format
                                            //and store it as json file at same directory of html statement file
                                            records.ToList().ForEach(acc => accounts.Add(new AccountMasterRecord()
                                            {
                                                AccountType = acc.FirstOrDefault().AccountType,
                                                Percentage = acc.Average(item => item.Percentage == null ? 0 : item.Percentage)
                                            }));

                                            string accountsJson = JsonConvert.SerializeObject(accounts);
                                            if (accountsJson != null && accountsJson != string.Empty)
                                            {
                                                AnalyticsChartJson = "analyticsdata=" + accountsJson;
                                            }
                                            else
                                            {
                                                AnalyticsChartJson = "analyticsdata=[]";
                                            }
                                        }
                                        else
                                        {
                                            AnalyticsChartJson = "analyticsdata=[]";
                                        }

                                        this.utility.WriteToJsonFile(AnalyticsChartJson, "analyticschartdata.json", batchMaster.Id, customer.Id, outputLocation);
                                        scriptHtmlRenderer.Append("<script type='text/javascript' src='./analyticschartdata.json'></script>");
                                        pageContent.Replace("analyticschartcontainer", "analyticschartcontainer" + page.Identifier);
                                        scriptHtmlRenderer.Append(HtmlConstants.ANALYTICS_CHART_WIDGET_SCRIPT.Replace("analyticschartcontainer", "analyticschartcontainer" + page.Identifier));
                                    }
                                    else if (widget.WidgetName == HtmlConstants.SAVING_TREND_WIDGET_NAME)
                                    {
                                        var savingtrends = CustomerSavingTrends.Where(item => item.CustomerId == customer.Id && item.BatchId == batchMaster.Id && item.AccountId == accountId && item.TenantCode == tenantCode).ToList();
                                        if (savingtrends != null && savingtrends.Count > 0)
                                        {
                                            IList<SavingTrend> savingTrendRecords = new List<SavingTrend>();
                                            int mnth = DateTime.Now.Month - 1;  //To start month validation of consecutive month data from previous month
                                            for (int t = savingtrends.Count; t > 0; t--)
                                            {
                                                string month = this.utility.getMonth(mnth);
                                                var lst = savingtrends.Where(it => it.Month.ToLower().Contains(month.ToLower()))?.ToList();
                                                if (lst != null && lst.Count > 0)
                                                {
                                                    SavingTrend trend = new SavingTrend();
                                                    trend.Month = lst[0].Month;
                                                    trend.NumericMonth = mnth;
                                                    trend.Income = lst[0].Income ?? 0;
                                                    trend.IncomePercentage = lst[0].IncomePercentage ?? 0;
                                                    trend.SpendAmount = lst[0].SpendAmount;
                                                    trend.SpendPercentage = lst[0].SpendPercentage ?? 0;
                                                    savingTrendRecords.Add(trend);
                                                }
                                                else
                                                {
                                                    ErrorMessages.Append("<li>Invalid consecutive month data for Saving trend widget..!!</li>");
                                                    IsFailed = true;
                                                }
                                                mnth = mnth - 1 == 0 ? 12 : mnth - 1;
                                            }

                                            //get saving trend chart widget data, convert it into json string format
                                            //and store it as json file at same directory of html statement file
                                            var records = savingTrendRecords.OrderByDescending(item => item.NumericMonth).Take(6).ToList();
                                            string savingtrendjson = JsonConvert.SerializeObject(records);
                                            if (savingtrendjson != null && savingtrendjson != string.Empty)
                                            {
                                                SavingTrendChartJson = "savingdata" + accountId + page.Identifier + "=" + savingtrendjson;
                                            }
                                            else
                                            {
                                                SavingTrendChartJson = "savingdata" + accountId + page.Identifier + "=[]";
                                            }
                                        }
                                        else
                                        {
                                            SavingTrendChartJson = "savingdata" + accountId + page.Identifier + "=[]";
                                        }

                                        this.utility.WriteToJsonFile(SavingTrendChartJson, "savingtrenddata" + accountId + page.Identifier + ".json", batchMaster.Id, customer.Id, outputLocation);
                                        scriptHtmlRenderer.Append("<script type='text/javascript' src='./savingtrenddata" + accountId + page.Identifier + ".json'></script>");

                                        pageContent.Replace("savingTrendscontainer", "savingTrendscontainer" + accountId + page.Identifier);
                                        var scriptval = HtmlConstants.SAVING_TREND_CHART_WIDGET_SCRIPT.Replace("savingTrendscontainer", "savingTrendscontainer" + accountId + page.Identifier).Replace("savingdata", "savingdata" + accountId + page.Identifier);
                                        scriptHtmlRenderer.Append(scriptval);
                                    }
                                    else if (widget.WidgetName == HtmlConstants.SPENDING_TREND_WIDGET_NAME)
                                    {
                                        var spendingtrends = CustomerSavingTrends.Where(item => item.CustomerId == customer.Id && item.BatchId == batchMaster.Id && item.AccountId == accountId && item.TenantCode == tenantCode).ToList();
                                        if (spendingtrends != null && spendingtrends.Count > 0)
                                        {
                                            IList<SavingTrend> trends = new List<SavingTrend>();
                                            int mnth = DateTime.Now.Month - 1; //To start month validation of consecutive month data from previous month
                                            for (int t = spendingtrends.Count; t > 0; t--)
                                            {
                                                string month = this.utility.getMonth(mnth);
                                                var lst = spendingtrends.Where(it => it.Month.ToLower().Contains(month.ToLower()))?.ToList();
                                                if (lst != null && lst.Count > 0)
                                                {
                                                    SavingTrend trend = new SavingTrend();
                                                    trend.Month = lst[0].Month;
                                                    trend.NumericMonth = mnth;
                                                    trend.Income = lst[0].Income ?? 0;
                                                    trend.IncomePercentage = lst[0].IncomePercentage ?? 0;
                                                    trend.SpendAmount = lst[0].SpendAmount;
                                                    trend.SpendPercentage = lst[0].SpendPercentage ?? 0;
                                                    trends.Add(trend);
                                                }
                                                else
                                                {
                                                    ErrorMessages.Append("<li>Invalid consecutive month data for Spending trend widget..!!</li>");
                                                    IsFailed = true;
                                                }
                                                mnth = mnth - 1 == 0 ? 12 : mnth - 1;
                                            }

                                            //get spending trend chart widget data, convert it into json string format
                                            //and store it as json file at same directory of html statement file
                                            var records = trends.OrderByDescending(item => item.NumericMonth).Take(6).ToList();
                                            string spendingtrendjson = JsonConvert.SerializeObject(records);
                                            if (spendingtrendjson != null && spendingtrendjson != string.Empty)
                                            {
                                                SpendingTrendChartJson = "spendingdata" + accountId + page.Identifier + "=" + spendingtrendjson;
                                            }
                                            else
                                            {
                                                SpendingTrendChartJson = "spendingdata" + accountId + page.Identifier + "=[]";
                                            }
                                        }
                                        else
                                        {
                                            SpendingTrendChartJson = "spendingdata" + accountId + page.Identifier + "=[]";
                                        }

                                        this.utility.WriteToJsonFile(SpendingTrendChartJson, "spendingtrenddata" + accountId + page.Identifier + ".json", batchMaster.Id, customer.Id, outputLocation);
                                        scriptHtmlRenderer.Append("<script type='text/javascript' src='./spendingtrenddata" + accountId + page.Identifier + ".json'></script>");

                                        pageContent.Replace("spendingTrendscontainer", "spendingTrendscontainer" + accountId + page.Identifier);
                                        var scriptval = HtmlConstants.SPENDING_TREND_CHART_WIDGET_SCRIPT.Replace("spendingTrendscontainer", "spendingTrendscontainer" + accountId + page.Identifier).Replace("spendingdata", "spendingdata" + accountId + page.Identifier);
                                        scriptHtmlRenderer.Append(scriptval);
                                    }
                                    else if (widget.WidgetName == HtmlConstants.REMINDER_AND_RECOMMENDATION_WIDGET_NAME)
                                    {
                                        using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                                        {
                                            var reminderAndRecommendations = nISEntitiesDataContext.ReminderAndRecommendationRecords.Where(item => item.CustomerId == customer.Id && item.BatchId == batchMaster.Id && item.TenantCode == tenantCode)?.ToList();

                                            StringBuilder reminderstr = new StringBuilder();
                                            if (reminderAndRecommendations != null && reminderAndRecommendations.Count > 0)
                                            {
                                                reminderstr.Append("<div class='row'><div class='col-lg-9'></div><div class='col-lg-3 text-left'><i class='fa fa-caret-left fa-3x float-left text-danger' aria-hidden='true'></i><span class='mt-2 d-inline-block ml-2'>Click</span></div> </div>");
                                                reminderAndRecommendations.ToList().ForEach(item =>
                                                {
                                                    string targetlink = item.TargetURL != null && item.TargetURL != string.Empty ? item.TargetURL : "javascript:void(0)";
                                                    reminderstr.Append("<div class='row'><div class='col-lg-9 text-left'><p class='p-1' style='background-color: #dce3dc;'>" +
                                                        item.Description + " </p></div><div class='col-lg-3 text-left'><a href='" + targetlink +
                                                        "' target='_blank'><i class='fa fa-caret-left fa-3x float-left text-danger'></i><span class='mt-2 d-inline-block ml-2'>" +
                                                        item.LabelText + "</span></a></div></div>");
                                                });
                                            }
                                            else
                                            {
                                                reminderstr.Append("<div class='row text-danger text-center' style='margin-top: 20px;'>No data available</div>");
                                            }
                                            pageContent.Replace("{{ReminderAndRecommdationDataList_" + page.Identifier + "_" + widget.Identifier + "}}", reminderstr.ToString());
                                        }
                                    }
                                }
                                else
                                {
                                    var dynaWidgets = dynamicWidgets.Where(item => item.Identifier == widget.WidgetId).ToList();
                                    if (dynaWidgets.Count > 0)
                                    {
                                        var dynawidget = dynaWidgets.FirstOrDefault();
                                        CustomeTheme themeDetails = new CustomeTheme();
                                        if (dynawidget.ThemeType == "Default")
                                        {
                                            themeDetails = JsonConvert.DeserializeObject<CustomeTheme>(tenantConfiguration.WidgetThemeSetting);
                                        }
                                        else
                                        {
                                            themeDetails = JsonConvert.DeserializeObject<CustomeTheme>(dynawidget.ThemeCSS);
                                        }

                                        //Get data from database for widget
                                        httpClient = new HttpClient();
                                        httpClient.BaseAddress = new Uri(tenantConfiguration.BaseUrlForTransactionData);
                                        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                                        httpClient.DefaultRequestHeaders.Add("TenantCode", tenantCode);

                                        //API search parameter
                                        JObject searchParameter = new JObject();
                                        searchParameter["BatchId"] = batchMaster.Id;
                                        searchParameter["CustomerId"] = customer.Id;
                                        searchParameter["WidgetFilterSetting"] = dynawidget.WidgetFilterSettings;

                                        if (dynawidget.WidgetType == HtmlConstants.TABLE_DYNAMICWIDGET)
                                        {
                                            List<DynamicWidgetTableEntity> tableEntities = JsonConvert.DeserializeObject<List<DynamicWidgetTableEntity>>(dynawidget.WidgetSettings);
                                            var tr = new StringBuilder();

                                            //API call
                                            var response = httpClient.PostAsync(dynawidget.APIPath, new StringContent(JsonConvert.SerializeObject(searchParameter), Encoding.UTF8, "application/json")).Result;
                                            if (response.StatusCode == HttpStatusCode.OK)
                                            {
                                                var result = response.Content.ReadAsStringAsync().Result;
                                                var apiOutputArr = JArray.Parse(result);
                                                if (apiOutputArr.Count > 0)
                                                {
                                                    apiOutputArr.ToList().ForEach(op =>
                                                    {
                                                        tr.Append("<tr>");
                                                        tableEntities.ForEach(field =>
                                                        {
                                                            tr.Append("<td> " + op[field.FieldName] + " </td>");
                                                        });
                                                        tr.Append("</tr>");
                                                    });
                                                }
                                                else
                                                {
                                                    tr.Append("<tr><td colspan='" + (tableEntities.Count + 1) + "'> No record found </td></tr>)");
                                                }
                                            }
                                            else
                                            {
                                                tr.Append("<tr><td colspan='" + (tableEntities.Count + 1) + "'> No record found </td></tr>");
                                            }
                                            pageContent.Replace("{{tableBody_" + page.Identifier + "_" + widget.Identifier + "}}", tr.ToString());
                                        }
                                        else if (dynawidget.WidgetType == HtmlConstants.FORM_DYNAMICWIDGET)
                                        {
                                            var formEntities = JsonConvert.DeserializeObject<List<DynamicWidgetFormEntity>>(dynawidget.WidgetSettings);
                                            var formdata = new StringBuilder();

                                            //API call
                                            var response = httpClient.PostAsync(dynawidget.APIPath, new StringContent(JsonConvert.SerializeObject(searchParameter), Encoding.UTF8, "application/json")).Result;
                                            if (response.StatusCode == HttpStatusCode.OK)
                                            {
                                                var result = response.Content.ReadAsStringAsync().Result;
                                                var apiOutputArr = JArray.Parse(result);
                                                if (apiOutputArr.Count > 0)
                                                {
                                                    apiOutputArr.ToList().ForEach(op =>
                                                    {
                                                        formEntities.ForEach(field =>
                                                        {
                                                            formdata.Append("<div class='row'><div class='col-sm-6'><label>" + field.DisplayName + "</label></div><div class='col-sm-6'>" + op[field.FieldName] + "</div></div>");
                                                        });
                                                    });
                                                }
                                                else
                                                {
                                                    formdata.Append("<div class='row'> No record found </div>");
                                                }
                                            }
                                            else
                                            {
                                                formdata.Append("<div class='row'> No record found </div>");
                                            }

                                            pageContent.Replace("{{FormData_" + page.Identifier + "_" + widget.Identifier + "}}", formdata.ToString());
                                        }
                                        else if (dynawidget.WidgetType == HtmlConstants.LINEGRAPH_DYNAMICWIDGET)
                                        {
                                            var chartDataVal = string.Empty;

                                            //API call
                                            var response = httpClient.PostAsync(dynawidget.APIPath, new StringContent(JsonConvert.SerializeObject(searchParameter), Encoding.UTF8, "application/json")).Result;
                                            if (response.StatusCode == HttpStatusCode.OK)
                                            {
                                                var result = response.Content.ReadAsStringAsync().Result;
                                                var apiOutputArr = JArray.Parse(result);
                                                if (apiOutputArr.Count > 0)
                                                {
                                                    var graphEntity = JsonConvert.DeserializeObject<DynamicWidgetLineGraph>(dynawidget.WidgetSettings);
                                                    GraphChartData chartData = new GraphChartData();
                                                    chartData.title = new ChartTitle { text = dynawidget.Title };

                                                    //To get chart x-axis list
                                                    var xAxis = apiOutputArr.ToList().Select(item => item[graphEntity.XAxis].ToString()).ToList();
                                                    chartData.xAxis = xAxis;

                                                    //To get chart series data
                                                    IList<ChartSeries> chartSeries = new List<ChartSeries>();
                                                    graphEntity.Details.ForEach(field =>
                                                    {
                                                        ChartSeries series = new ChartSeries();
                                                        series.name = field.DisplayName;
                                                        var res = apiOutputArr.ToList().Select(item => item[field.FieldName]).ToList();
                                                        var seriesdata = new List<decimal>();
                                                        res.ForEach(r =>
                                                        {
                                                            seriesdata.Add(Convert.ToDecimal(r.ToString()));
                                                        });
                                                        series.data = seriesdata;
                                                        series.type = "line";
                                                        chartSeries.Add(series);
                                                    });
                                                    chartData.series = chartSeries;

                                                    //To get chart theme
                                                    string theme = string.Empty;
                                                    if (themeDetails != null)
                                                    {
                                                        if (themeDetails.ChartColorTheme != null && themeDetails.ChartColorTheme != "")
                                                        {
                                                            theme = themeDetails.ChartColorTheme;
                                                        }
                                                        else if (themeDetails.ColorTheme != null && themeDetails.ColorTheme != "")
                                                        {
                                                            theme = themeDetails.ColorTheme;
                                                        }
                                                    }
                                                    chartData.color = this.GetChartColorTheme(theme);
                                                    chartDataVal = JsonConvert.SerializeObject(chartData);
                                                }
                                            }

                                            pageContent.Replace("hiddenLineGraphValue_" + page.Identifier + "_" + widget.Identifier + "", chartDataVal);
                                            scriptHtmlRenderer.Append(HtmlConstants.LINE_GRAPH_WIDGET_SCRIPT.Replace("linechartcontainer", "lineGraphcontainer_" + page.Identifier + "_" + widget.Identifier).Replace("hiddenLineGraphData", "hiddenLineGraphData_" + page.Identifier + "_" + widget.Identifier));
                                        }
                                        else if (dynawidget.WidgetType == HtmlConstants.BARGRAPH_DYNAMICWIDGET)
                                        {
                                            var chartDataVal = string.Empty;

                                            //API call
                                            var response = httpClient.PostAsync(dynawidget.APIPath, new StringContent(JsonConvert.SerializeObject(searchParameter), Encoding.UTF8, "application/json")).Result;
                                            if (response.StatusCode == HttpStatusCode.OK)
                                            {
                                                var result = response.Content.ReadAsStringAsync().Result;
                                                var apiOutputArr = JArray.Parse(result);
                                                if (apiOutputArr.Count > 0)
                                                {
                                                    var graphEntity = JsonConvert.DeserializeObject<DynamicWidgetLineGraph>(dynawidget.WidgetSettings);
                                                    GraphChartData chartData = new GraphChartData();
                                                    chartData.title = new ChartTitle { text = dynawidget.Title };

                                                    //To get chart x-axis list
                                                    var xAxis = apiOutputArr.ToList().Select(item => item[graphEntity.XAxis].ToString()).ToList();
                                                    chartData.xAxis = xAxis;

                                                    //To get chart series data
                                                    IList<ChartSeries> chartSeries = new List<ChartSeries>();
                                                    graphEntity.Details.ForEach(field =>
                                                    {
                                                        ChartSeries series = new ChartSeries();
                                                        series.name = field.DisplayName;
                                                        var res = apiOutputArr.ToList().Select(item => item[field.FieldName]).ToList();
                                                        var seriesdata = new List<decimal>();
                                                        res.ForEach(r =>
                                                        {
                                                            seriesdata.Add(Convert.ToDecimal(r.ToString()));
                                                        });
                                                        series.data = seriesdata;
                                                        series.type = "column";
                                                        chartSeries.Add(series);
                                                    });
                                                    chartData.series = chartSeries;

                                                    //To get chart theme
                                                    string theme = string.Empty;
                                                    if (themeDetails != null)
                                                    {
                                                        if (themeDetails.ChartColorTheme != null && themeDetails.ChartColorTheme != "")
                                                        {
                                                            theme = themeDetails.ChartColorTheme;
                                                        }
                                                        else if (themeDetails.ColorTheme != null && themeDetails.ColorTheme != "")
                                                        {
                                                            theme = themeDetails.ColorTheme;
                                                        }
                                                    }
                                                    chartData.color = this.GetChartColorTheme(theme);
                                                    chartDataVal = JsonConvert.SerializeObject(chartData);
                                                }
                                            }


                                            pageContent.Replace("hiddenBarGraphValue_" + page.Identifier + "_" + widget.Identifier + "", chartDataVal);
                                            scriptHtmlRenderer.Append(HtmlConstants.BAR_GRAPH_WIDGET_SCRIPT.Replace("barchartcontainer", "barGraphcontainer_" + page.Identifier + "_" + widget.Identifier).Replace("hiddenBarGraphData", "hiddenBarGraphData_" + page.Identifier + "_" + widget.Identifier));
                                        }
                                        else if (dynawidget.WidgetType == HtmlConstants.PICHART_DYNAMICWIDGET)
                                        {
                                            var chartDataVal = string.Empty;

                                            //API call
                                            var response = httpClient.PostAsync(dynawidget.APIPath, new StringContent(JsonConvert.SerializeObject(searchParameter), Encoding.UTF8, "application/json")).Result;
                                            if (response.StatusCode == HttpStatusCode.OK)
                                            {
                                                var result = response.Content.ReadAsStringAsync().Result;
                                                var apiOutputArr = JArray.Parse(result);
                                                if (apiOutputArr.Count > 0)
                                                {
                                                    PieChartSettingDetails pieChartSetting = JsonConvert.DeserializeObject<PieChartSettingDetails>(dynawidget.WidgetSettings);
                                                    var entityFields = this.dynamicWidgetRepository.GetEntityFields(dynawidget.EntityId, tenantCode);
                                                    var seriesfor = entityFields.Where(it => it.Identifier == Convert.ToInt32(pieChartSetting.PieSeries))?.ToList()?.FirstOrDefault().Name;
                                                    var seriesdatafor = entityFields.Where(it => it.Identifier == Convert.ToInt32(pieChartSetting.PieValue))?.ToList()?.FirstOrDefault().Name;

                                                    PiChartGraphData chartData = new PiChartGraphData();
                                                    chartData.title = new ChartTitle { text = dynawidget.Title };

                                                    //To get series data
                                                    var chartseries = new List<PieChartSeries>();
                                                    var datas = new List<PieChartData>();
                                                    apiOutputArr.ToList().ForEach(item =>
                                                    {
                                                        PieChartData pie = new PieChartData
                                                        {
                                                            name = item[seriesfor] != null ? item[seriesfor].ToString() : "",
                                                            y = Convert.ToDecimal(item[seriesdatafor] != null ? item[seriesdatafor] : 0)
                                                        };
                                                        datas.Add(pie);
                                                    });

                                                    PieChartSeries series = new PieChartSeries();
                                                    series.name = seriesfor;
                                                    series.data = datas;
                                                    chartseries.Add(series);
                                                    chartData.series = chartseries;

                                                    //To get chart theme
                                                    string theme = string.Empty;
                                                    if (themeDetails != null)
                                                    {
                                                        if (themeDetails.ChartColorTheme != null && themeDetails.ChartColorTheme != "")
                                                        {
                                                            theme = themeDetails.ChartColorTheme;
                                                        }
                                                        else if (themeDetails.ColorTheme != null && themeDetails.ColorTheme != "")
                                                        {
                                                            theme = themeDetails.ColorTheme;
                                                        }
                                                    }
                                                    chartData.color = this.GetChartColorTheme(theme);
                                                    chartDataVal = JsonConvert.SerializeObject(chartData);
                                                }
                                            }

                                            pageContent.Replace("hiddenPieChartValue_" + page.Identifier + "_" + widget.Identifier + "", chartDataVal);
                                            scriptHtmlRenderer.Append(HtmlConstants.PIE_CHART_WIDGET_SCRIPT.Replace("pieChartcontainer", "pieChartcontainer_" + page.Identifier + "_" + widget.Identifier).Replace("hiddenPieChartData", "hiddenPieChartData_" + page.Identifier + "_" + widget.Identifier));
                                        }
                                        else if (dynawidget.WidgetType == HtmlConstants.HTML_DYNAMICWIDGET)
                                        {
                                            var htmlWidgetContent = new StringBuilder(dynawidget.PreviewData);
                                            if (dynawidget.WidgetSettings != null)
                                            {
                                                var _lstHtmlWidgetSettings = JsonConvert.DeserializeObject<List<HtmlWidgetSettings>>(dynawidget.WidgetSettings);
                                                if (_lstHtmlWidgetSettings.Count > 0)
                                                {
                                                    //API call
                                                    var response = httpClient.PostAsync(dynawidget.APIPath, new StringContent(JsonConvert.SerializeObject(searchParameter), Encoding.UTF8, "application/json")).Result;
                                                    if (response.StatusCode == HttpStatusCode.OK)
                                                    {
                                                        var result = response.Content.ReadAsStringAsync().Result;
                                                        var apiOutputArr = JArray.Parse(result);
                                                        if (apiOutputArr.Count > 0)
                                                        {
                                                            var apidata = apiOutputArr.FirstOrDefault();
                                                            _lstHtmlWidgetSettings.ForEach(setting =>
                                                            {
                                                                if (setting.Value != null && setting.Value != string.Empty && setting.Key != null && setting.Key != string.Empty
                                                                && apidata[setting.Key] != null)
                                                                {
                                                                    htmlWidgetContent.Replace(setting.Value, apidata[setting.Key].ToString());
                                                                }
                                                            });
                                                        }
                                                    }
                                                }
                                            }
                                            pageContent.Replace("{{FormData_" + page.Identifier + "_" + widget.Identifier + "}}", htmlWidgetContent.ToString());
                                        }
                                    }
                                }
                            }

                            //if account number variable is not empty means, financial tenant
                            if (accountNumber != string.Empty)
                            {
                                //generate statement metadata records in list format
                                statementMetadataRecords.Add(new StatementMetadataRecord
                                {
                                    AccountNumber = accountNumber,
                                    AccountType = accountType,
                                    CustomerId = customer.Id,
                                    CustomerName = customer.FirstName + (customer.MiddleName == string.Empty ? "" : " " + customer.MiddleName) + " " + customer.LastName,
                                    StatementPeriod = customer.StatementPeriod,
                                    StatementId = statement.Identifier,
                                });
                            }
                            else
                            {
                                //To add statement metadata records for subscription master tenant
                                //using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                                //{
                                //    var subscriptionmasters = nISEntitiesDataContext.TTD_SubscriptionMasterRecord.Where(item => item.CustomerId == customer.Id && item.BatchId == batchMaster.Id && item.TenantCode == tenantCode).ToList();
                                //    subscriptionmasters.ForEach(sub =>
                                //    {
                                //        var records = statementMetadataRecords.Where(item => item.CustomerId == customer.Id && item.StatementId == statement.Identifier && item.AccountNumber == sub.Subscription && item.AccountType == sub.VendorName).ToList();
                                //        if (records.Count <= 0)
                                //        {
                                //            statementMetadataRecords.Add(new StatementMetadataRecord
                                //            {
                                //                AccountNumber = sub.Subscription,
                                //                AccountType = sub.VendorName,
                                //                CustomerId = customer.Id,
                                //                CustomerName = customer.FirstName + (customer.MiddleName == string.Empty ? "" : " " + customer.MiddleName) + " " + customer.LastName,
                                //                StatementPeriod = customer.StatementPeriod,
                                //                StatementId = statement.Identifier,
                                //            });
                                //        }
                                //    });
                                //}
                            }

                            newPageContent.Append(pageContent);
                            if (page.PageTypeName == HtmlConstants.SAVING_ACCOUNT_PAGE || page.PageTypeName == HtmlConstants.CURRENT_ACCOUNT_PAGE)
                            {
                                newPageContent.Append(HtmlConstants.END_DIV_TAG);
                            }

                            if (x == subPageCount - 1)
                            {
                                newPageContent.Append(HtmlConstants.END_DIV_TAG);
                            }
                        }

                        PageHeaderContent.Replace("{{SubTabs}}", SubTabs.ToString());
                        statementPageContent.PageHeaderContent = PageHeaderContent.ToString();
                        statementPageContent.HtmlContent = newPageContent.ToString();
                    }

                    newStatementPageContents.ToList().ForEach(page =>
                    {
                        htmlbody.Append(page.PageHeaderContent);
                        htmlbody.Append(page.HtmlContent);
                        htmlbody.Append(page.PageFooterContent);
                    });

                    htmlbody.Append(HtmlConstants.CONTAINER_DIV_HTML_FOOTER);

                    navbarHtml = navbarHtml.Replace("{{NavItemList}}", navbar.ToString());

                    StringBuilder finalHtml = new StringBuilder();
                    finalHtml.Append(HtmlConstants.HTML_HEADER);
                    finalHtml.Append(navbarHtml);
                    finalHtml.Append(htmlbody.ToString());
                    finalHtml.Append(HtmlConstants.HTML_FOOTER);
                    scriptHtmlRenderer.Append(HtmlConstants.TENANT_LOGO_SCRIPT);

                    //replace below variable with actual data in final html string
                    finalHtml.Replace("{{ChartScripts}}", scriptHtmlRenderer.ToString());
                    finalHtml.Replace("{{CustomerNumber}}", customer.Id.ToString());
                    finalHtml.Replace("{{StatementNumber}}", statement.Identifier.ToString());
                    finalHtml.Replace("{{FirstPageId}}", FirstPageId.ToString());
                    finalHtml.Replace("{{TenantCode}}", tenantCode);

                    //If has any error while rendering html statement, then assign status as failed and all collected errors message to log message variable..
                    //Otherwise write html statement string to actual html file and store it at output location, then assign status as completed
                    if (IsFailed)
                    {
                        logDetailRecord.Status = ScheduleLogStatus.Failed.ToString();
                        logDetailRecord.LogMessage = "<ul class='pl-4 text-left'>" + ErrorMessages.ToString() + "</ul>";
                    }
                    else
                    {
                        string fileName = "Statement_" + customer.Id + "_" + statement.Identifier + "_" + DateTime.Now.ToString().Replace("-", "_").Replace(":", "_").Replace(" ", "_").Replace('/', '_') + ".html";
                        //string filePath = this.utility.WriteToFile(finalHtml.ToString(), fileName, batchMaster.Id, customer.Id, baseURL, outputLocation);
                        var scheduleName = "";
                        using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                        {
                            var schedule = nISEntitiesDataContext.ScheduleRecords.Where(a => a.Id == logDetailRecord.ScheduleId).FirstOrDefault();
                            scheduleName = schedule.Name;
                        }
                        string filePath = this.utility.WriteToFile(finalHtml.ToString(), fileName, scheduleName, batchMaster.BatchName, customer.Id, baseURL, outputLocation);

                        logDetailRecord.StatementFilePath = filePath;
                        logDetailRecord.Status = ScheduleLogStatus.Completed.ToString();
                        logDetailRecord.LogMessage = "Statement generated successfully..!!";
                        logDetailRecord.StatementMetadataRecords = statementMetadataRecords;
                    }
                }

                return logDetailRecord;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        

        /// <summary>
        /// This method help to apply theme for dynamic table, form, and html widgets
        /// </summary>
        /// <param name="html"> the widget html string </param>
        /// <param name="themeDetails"> the theme details for widget </param>
        /// <returns>return new html after applying theme </returns>
        public string ApplyStyleCssForDynamicTableAndFormWidget(string html, CustomeTheme themeDetails)
        {
            StringBuilder style = new StringBuilder();
            style.Append(HtmlConstants.STYLE);

            if (themeDetails.TitleColor != null)
            {
                style = style.Replace("{{COLOR}}", themeDetails.TitleColor);
            }
            if (themeDetails.TitleSize != null)
            {
                style = style.Replace("{{SIZE}}", themeDetails.TitleSize);
            }
            if (themeDetails.TitleWeight != null)
            {
                style = style.Replace("{{WEIGHT}}", themeDetails.TitleWeight);
            }
            if (themeDetails.TitleType != null)
            {
                style = style.Replace("{{TYPE}}", themeDetails.TitleType);
            }
            html = html.Replace("{{TitleStyle}}", style.ToString());

            style = new StringBuilder();
            style.Append(HtmlConstants.STYLE);
            if (themeDetails.HeaderColor != null)
            {
                style = style.Replace("{{COLOR}}", themeDetails.HeaderColor);
            }
            if (themeDetails.HeaderSize != null)
            {
                style = style.Replace("{{SIZE}}", themeDetails.HeaderSize);
            }
            if (themeDetails.HeaderWeight != null)
            {
                style = style.Replace("{{WEIGHT}}", themeDetails.HeaderWeight);
            }
            if (themeDetails.HeaderType != null)
            {
                style = style.Replace("{{TYPE}}", themeDetails.HeaderType);
            }
            html = html.Replace("{{HeaderStyle}}", style.ToString());

            style = new StringBuilder();
            style.Append(HtmlConstants.STYLE);
            if (themeDetails.DataColor != null)
            {
                style = style.Replace("{{COLOR}}", themeDetails.DataColor);
            }
            if (themeDetails.DataSize != null)
            {
                style = style.Replace("{{SIZE}}", themeDetails.DataSize);
            }
            if (themeDetails.DataWeight != null)
            {
                style = style.Replace("{{WEIGHT}}", themeDetails.DataWeight);
            }
            if (themeDetails.DataType != null)
            {
                style = style.Replace("{{TYPE}}", themeDetails.DataType);
            }
            html = html.Replace("{{BodyStyle}}", style.ToString());
            return html;
        }

        /// <summary>
        /// This method help to apply theme for dynamic line graph, bar graph, and pie chart widgets
        /// </summary>
        /// <param name="html"> the widget html string </param>
        /// <param name="themeDetails"> the theme details for widget </param>
        /// <returns>return new html after applying theme </returns>
        public string ApplyStyleCssForDynamicGraphAndChartWidgets(string html, CustomeTheme themeDetails)
        {
            StringBuilder style = new StringBuilder();
            style.Append(HtmlConstants.STYLE);
            if (themeDetails.TitleColor != null)
            {
                style = style.Replace("{{COLOR}}", themeDetails.TitleColor);
            }
            if (themeDetails.TitleSize != null)
            {
                style = style.Replace("{{SIZE}}", themeDetails.TitleSize);
            }
            if (themeDetails.TitleWeight != null)
            {
                style = style.Replace("{{WEIGHT}}", themeDetails.TitleWeight);
            }
            if (themeDetails.TitleType != null)
            {
                style = style.Replace("{{TYPE}}", themeDetails.TitleType);
            }
            html = html.Replace("{{TitleStyle}}", style.ToString());
            return html;
        }

        /// <summary>
        /// This method help to actaul color theme to show series data for dynamic line graph, bar graph, and pie chart widgets
        /// </summary>
        /// <param name="theme"> the widget theme </param>
        /// <returns>return new color theme for graph and chart widgets </returns>
        public string GetChartColorTheme(string theme)
        {
            string colorTheme = string.Empty;
            if (theme == "Theme1")
            {
                colorTheme = HtmlConstants.THEME1;
            }
            else if (theme == "Theme2")
            {
                colorTheme = HtmlConstants.THEME2;
            }
            else if (theme == "Theme3")
            {
                colorTheme = HtmlConstants.THEME3;
            }
            else if (theme == "Theme4")
            {
                colorTheme = HtmlConstants.THEME4;
            }
            else if (theme.ToLower() == "ChartTheme1".ToLower())
            {
                colorTheme = HtmlConstants.THEME1;
            }
            else if (theme.ToLower() == "ChartTheme2".ToLower())
            {
                colorTheme = HtmlConstants.THEME2;
            }
            else if (theme.ToLower() == "ChartTheme3".ToLower())
            {
                colorTheme = HtmlConstants.THEME4;
            }
            else if (theme.ToLower() == "ChartTheme4".ToLower())
            {
                colorTheme = HtmlConstants.THEME3;
            }

            return colorTheme;
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
        private string WhereClauseGenerator(StatementSearchParameter searchParameter, string tenantCode)
        {
            StringBuilder queryString = new StringBuilder();

            if (searchParameter.SearchMode == SearchMode.Equals)
            {
                if (validationEngine.IsValidLong(searchParameter.Identifier))
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
            if (validationEngine.IsValidText(searchParameter.StatementOwner))
            {
                queryString.Append(string.Format("OwnerName.Contains(\"{0}\") and ", searchParameter.StatementOwner));
            }
            if (validationEngine.IsValidText(searchParameter.Status))
            {
                queryString.Append(string.Format("Status.Equals(\"{0}\") and ", searchParameter.Status));
            }
            if (validationEngine.IsValidLong(searchParameter.StatementTypeId))
            {
                queryString.Append("(" + string.Join("or ", searchParameter.StatementTypeId.ToString().Split(',').Select(item => string.Format("StatementTypeId.Equals({0}) ", item))) + ") and ");
            }
            if (searchParameter.IsActive != null)
            {
                queryString.Append(string.Format("IsActive.Equals({0}) and ", searchParameter.IsActive));
            }
            if (this.validationEngine.IsValidDate(searchParameter.StartDate) && !this.validationEngine.IsValidDate(searchParameter.EndDate))
            {
                DateTime fromDateTime = DateTime.SpecifyKind(Convert.ToDateTime(searchParameter.StartDate), DateTimeKind.Utc);
                queryString.Append("PublishedOn >= DateTime(" + fromDateTime.Year + "," + fromDateTime.Month + "," + fromDateTime.Day + "," + fromDateTime.Hour + "," + fromDateTime.Minute + "," + fromDateTime.Second + ") and ");
            }
            if (this.validationEngine.IsValidDate(searchParameter.EndDate) && !this.validationEngine.IsValidDate(searchParameter.StartDate))
            {
                DateTime toDateTime = DateTime.SpecifyKind(Convert.ToDateTime(searchParameter.EndDate), DateTimeKind.Utc);
                queryString.Append("PublishedOn <= DateTime(" + toDateTime.Year + "," + toDateTime.Month + "," + toDateTime.Day + "," + toDateTime.Hour + "," + toDateTime.Minute + "," + toDateTime.Second + ") and ");
            }
            if (this.validationEngine.IsValidDate(searchParameter.StartDate) && this.validationEngine.IsValidDate(searchParameter.EndDate))
            {
                DateTime fromDateTime = DateTime.SpecifyKind(Convert.ToDateTime(searchParameter.StartDate), DateTimeKind.Utc);
                DateTime toDateTime = DateTime.SpecifyKind(Convert.ToDateTime(searchParameter.EndDate), DateTimeKind.Utc);

                queryString.Append("PublishedOn >= DateTime(" + fromDateTime.Year + "," + fromDateTime.Month + "," + fromDateTime.Day + "," + fromDateTime.Hour + "," + fromDateTime.Minute + "," + fromDateTime.Second + ") " +
                               "and PublishedOn <= DateTime(" + +toDateTime.Year + "," + toDateTime.Month + "," + toDateTime.Day + "," + toDateTime.Hour + "," + toDateTime.Minute + "," + toDateTime.Second + ") and ");
            }

            queryString.Append(string.Format("TenantCode.Equals(\"{0}\") and IsDeleted.Equals(false)", tenantCode));
            return queryString.ToString();
        }

        /// <summary>
        /// This method determines uniqueness of elements in repository.
        /// </summary>
        /// <param name="statements">The statements to save.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns name="result">
        /// Returns true if all elements are not present in repository, false otherwise.
        /// </returns>
        private bool IsDuplicateStatement(IList<Statement> statements, string operation, string tenantCode)
        {
            bool result = false;
            try
            {
                this.SetAndValidateConnectionString(tenantCode);

                StringBuilder query = new StringBuilder();

                if (operation.Equals(ModelConstant.ADD_OPERATION))
                {
                    query.Append("(" + string.Join(" or ", statements.Select(item => string.Format("Name.Equals(\"{0}\")", item.Name)).ToList()) + ") and IsDeleted.Equals(false) and TenantCode.Equals(\"" + tenantCode + "\")");
                }

                if (operation.Equals(ModelConstant.UPDATE_OPERATION))
                {
                    query.Append("(" + string.Join(" or ", statements.Select(item => string.Format("(Name.Equals(\"{0}\") and Version.Equals(\"{1}\") and !Id.Equals({2}))", item.Name, item.Version, item.Identifier))) + ") and IsDeleted.Equals(false) and TenantCode.Equals(\"" + tenantCode + "\")");
                }

                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    IList<StatementRecord> statementRecords = nISEntitiesDataContext.StatementRecords.Where(query.ToString()).Select(item => item).AsQueryable().ToList();
                    if (statementRecords.Count > 0)
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

        /// <summary>
        /// This method determines uniqueness of elements in repository.
        /// </summary>
        /// <param name="statements">The statements to save.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns name="result">
        /// Returns true if all elements are not present in repository, false otherwise.
        /// </returns>
        private bool IsDuplicateStatementPage(IList<StatementPage> statementPages, long statementIdentifier, string operation, string tenantCode)
        {
            bool result = false;
            try
            {
                this.SetAndValidateConnectionString(tenantCode);

                StringBuilder query = new StringBuilder();

                if (operation.Equals(ModelConstant.ADD_OPERATION))
                {
                    query.Append("(" + string.Join(" or ", statementPages.Select(item => string.Format("StatementId.equals({0}) and ReferenceWidgetId.Equals({1}) ", statementIdentifier, item.ReferencePageId)).ToList()) + ") and TenantCode.equals(\"" + tenantCode + "\")");
                }
                if (operation.Equals(ModelConstant.UPDATE_OPERATION))
                {
                    query.Append("(" + string.Join(" or ", statementPages.Select(item => string.Format("(StatementId.Equals({0}) and ReferenceWidgetId.Equals({1}) and !Id.Equals({2}))", statementIdentifier, item.ReferencePageId, item.Identifier))) + ") and TenantCode.Equals(\"" + tenantCode + "\")");
                }
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    IList<StatementPageMapRecord> statementWidgetMapRecords = nISEntitiesDataContext.StatementPageMapRecords.Where(query.ToString()).Select(item => item).AsQueryable().ToList();
                    if (statementWidgetMapRecords.Count > 0)
                    {
                        result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
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

        private bool AddStatementPages(IList<StatementPage> statementPages, long statementIdentifier, string tenantCode)
        {
            bool result = false;
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                //if (this.IsDuplicateStatementPage(statementPages, statementIdentifier, "AddOperation", tenantCode))
                //{
                //    throw new DuplicateStatementPageFoundException(tenantCode);
                //}

                IList<StatementPageMapRecord> statementWidgetRecords = new List<StatementPageMapRecord>();
                statementPages.ToList().ForEach(statementWidget =>
                {
                    statementWidgetRecords.Add(new StatementPageMapRecord()
                    {
                        ReferencePageId = statementWidget.ReferencePageId,
                        StatementId = statementIdentifier,
                        SequenceNumber = statementWidget.SequenceNumber,
                        TenantCode = tenantCode
                    });
                });

                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    nISEntitiesDataContext.StatementPageMapRecords.AddRange(statementWidgetRecords);
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

        private string GetHTMLPreviewData(TenantEntity entity, IList<EntityFieldMap> fieldMaps, string widgetSettings)
        {
            string obj = string.Empty;
            JObject item = new JObject();
            fieldMaps.ToList().ForEach(field =>
            {
                item[field.Name] = field.Name + "1";
            });
            StringBuilder tableBody = new StringBuilder();

            fieldMaps.ToList().ForEach(field =>
            {
                string fieldIdFormat = "{{" + field.Name + "_" + field.Identifier + "}}";
                if (widgetSettings.Contains(fieldIdFormat))
                {
                    widgetSettings = widgetSettings.Replace(fieldIdFormat.ToString(), item[field.Name].ToString());
                }
            });
            obj = widgetSettings;
            return obj;
        }

        static string FormatDateWithOrdinal(DateTime date)
        {
            // Get the day of the month
            int day = date.Day;

            // Create an ordinal suffix (e.g., "st", "nd", "rd", "th")
            string ordinalSuffix;
            if (day % 100 >= 11 && day % 100 <= 13)
            {
                ordinalSuffix = "th";
            }
            else
            {
                switch (day % 10)
                {
                    case 1:
                        ordinalSuffix = "st";
                        break;
                    case 2:
                        ordinalSuffix = "nd";
                        break;
                    case 3:
                        ordinalSuffix = "rd";
                        break;
                    default:
                        ordinalSuffix = "th";
                        break;
                }
            }

            // Format the date with the ordinal suffix
            string formattedDate = $"{day}<sup>{ordinalSuffix}</sup> {date:MMMM}";

            return formattedDate;
        }

        #endregion
    }
}
