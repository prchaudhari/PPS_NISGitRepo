// <copyright file="SQLStatementRepository.cs" company="Websym Solutions Pvt. Ltd.">
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
    using System.Security.Claims;
    using System.Text;
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

        #endregion

        #region Constructor

        public SQLStatementRepository(IUnityContainer unityContainer)
        {
            this.unityContainer = unityContainer;
            this.validationEngine = new ValidationEngine();
            this.utility = new Utility();
            this.configurationutility = new ConfigurationUtility(this.unityContainer);
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
                        item.PublishedOn = DateTime.Now;
                        item.Status = StatementStatus.Published.ToString();
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

                statements.ToList().ForEach(statement =>
                {
                    statement.Identifier = statementRecords.Where(p => p.Name == statement.Name && p.Version == "1").Single().Id;

                    //Add statement widgets in to statement widget map
                    if (statement.StatementPages?.Count > 0)
                    {
                        this.AddStatementPages(statement.StatementPages, statement.Identifier, tenantCode);
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
        /// This method reference to preview statement
        /// </summary>
        /// <param name="statementIdentifier"></param>
        /// <param name="tenantCode"></param>
        /// <returns></returns>
        public bool PreviewStatement(long statementIdentifier, string tenantCode)
        {
            throw new NotImplementedException();
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

                this.SetAndValidateConnectionString(tenantCode);

                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    //var statementRecords = (from s in nISEntitiesDataContext.StatementRecords
                    //                        join sp in nISEntitiesDataContext.StatementPageMapRecords on s.Id equals sp.StatementId
                    //                        where s.IsDeleted == false && sp.ReferencePageId == statementIdentifier
                    //                        select new { s.Id }).ToList();
                    //if (statementRecords.Count > 0)
                    //{
                    //    throw new StatementReferenceException(tenantCode);
                    //}

                    IList<StatementRecord> statementRecords = nISEntitiesDataContext.StatementRecords.Where(itm => itm.Id == statementIdentifier).ToList();
                    if (statementRecords == null || statementRecords.Count <= 0)
                    {
                        throw new StatementNotFoundException(tenantCode);
                    }

                    statementRecords.ToList().ForEach(item =>
                    {
                        item.IsDeleted = true;
                        item.UpdateBy = userId;
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

                IList<StatementRecord> statementRecords = new List<StatementRecord>();
                IList<UserRecord> statementOwnerUserRecords = new List<UserRecord>();
                IList<UserRecord> statementPublishedUserRecords = new List<UserRecord>();
                IList<StatementPage> statementPages = new List<StatementPage>();
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    if (statementSearchParameter.StatementOwner != null && statementSearchParameter.StatementOwner != string.Empty)
                    {
                        StringBuilder queryString = new StringBuilder();
                        queryString.Append(string.Format("FirstName.Contains(\"{0}\") or LastName.Contains(\"{1}\") ", statementSearchParameter.StatementOwner, statementSearchParameter.StatementOwner));
                        queryString.Append(string.Format(" and IsDeleted.Equals(false)"));
                        var userRecordIds = nISEntitiesDataContext.UserRecords.Where(queryString.ToString()).ToList().Select(itm => itm.Id).ToList();
                        if (userRecordIds.Count > 0)
                        {
                            queryString = new StringBuilder();
                            queryString.Append(" and (" + string.Join("or ", userRecordIds.Select(item => string.Format("Owner.Equals({0}) ", item))) + ") ");
                            whereClause = whereClause + queryString.ToString();
                        }
                        else
                        {
                            return statements;
                        }
                    }

                    if (statementSearchParameter.PagingParameter.PageIndex > 0 && statementSearchParameter.PagingParameter.PageSize > 0)
                    {
                        statementRecords = nISEntitiesDataContext.StatementRecords
                        .OrderBy(statementSearchParameter.SortParameter.SortColumn + " " + statementSearchParameter.SortParameter.SortOrder.ToString())
                        .Where(whereClause)
                        .Skip((statementSearchParameter.PagingParameter.PageIndex - 1) * statementSearchParameter.PagingParameter.PageSize)
                        .Take(statementSearchParameter.PagingParameter.PageSize)
                        .ToList();
                    }
                    else
                    {
                        statementRecords = nISEntitiesDataContext.StatementRecords
                        .Where(whereClause)
                        .OrderBy(statementSearchParameter.SortParameter.SortColumn + " " + statementSearchParameter.SortParameter.SortOrder.ToString().ToLower())
                        .ToList();
                    }

                    if (statementRecords != null && statementRecords.ToList().Count > 0)
                    {
                        StringBuilder userIdentifier = new StringBuilder();
                        userIdentifier.Append("(" + string.Join(" or ", statementRecords.Select(item => string.Format("Id.Equals({0})", item.Owner))) + ")");
                        userIdentifier.Append(string.Format(" and IsDeleted.Equals(false)"));
                        statementOwnerUserRecords = nISEntitiesDataContext.UserRecords.Where(userIdentifier.ToString()).ToList();

                        var publisheByUserIds = statementRecords.Where(itm => itm.PublishedBy != null).ToList();
                        if (publisheByUserIds.Count > 0)
                        {
                            userIdentifier = new StringBuilder();
                            userIdentifier.Append("(" + string.Join(" or ", publisheByUserIds.Select(item => string.Format("Id.Equals({0})", item.PublishedBy))) + ")");
                            userIdentifier.Append(string.Format(" and IsDeleted.Equals(false)"));
                            statementPublishedUserRecords = nISEntitiesDataContext.UserRecords.Where(userIdentifier.ToString()).ToList();
                        }
                        if (statementSearchParameter.IsStatementPagesRequired == true)
                        {

                            IList<StatementPageMapRecord> statementPageRecordMaps = new List<StatementPageMapRecord>();
                            StringBuilder mapRecordIdentifier = new StringBuilder();
                            mapRecordIdentifier.Append("(" + string.Join(" or ", statementRecords.Select(item => string.Format("StatementId.Equals({0})", item.Id))) + ")");
                            statementPageRecordMaps = nISEntitiesDataContext.StatementPageMapRecords.Where(mapRecordIdentifier.ToString()).OrderBy(item => item.SequenceNumber).ToList();
                            if (statementPageRecordMaps?.Count > 0)
                            {
                                IList<PageRecord> pages = new List<PageRecord>();

                                StringBuilder pageIdentifier = new StringBuilder();
                                pageIdentifier.Append("(" + string.Join(" or ", statementPageRecordMaps.Select(item => string.Format("Id.Equals({0})", item.ReferencePageId))) + ")");
                                pages = nISEntitiesDataContext.PageRecords.Where(pageIdentifier.ToString()).ToList();

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

                if (statementRecords != null && statementRecords.ToList().Count > 0)
                {
                    statementRecords?.ToList().ForEach(statementRecord =>
                    {
                        statements.Add(new Statement
                        {
                            Identifier = statementRecord.Id,
                            Name = statementRecord.Name,
                            CreatedDate = statementRecord.CreatedDate ?? (DateTime)statementRecord.CreatedDate,
                            IsActive = statementRecord.IsActive,
                            LastUpdatedDate = statementRecord.LastUpdatedDate ?? (DateTime)statementRecord.LastUpdatedDate,
                            Owner = statementRecord.Owner,
                            StatementOwnerName = statementOwnerUserRecords.Where(usr => usr.Id == statementRecord.Owner).ToList()?.Select(itm => new { FullName = itm.FirstName + " " + itm.LastName })?.FirstOrDefault().FullName,
                            StatementPages = statementPages?.Where(item => item.StatementId == statementRecord.Id)?.ToList(),
                            Status = statementRecord.Status,
                            Version = statementRecord.Version,
                            PublishedBy = statementRecord.PublishedBy,
                            StatementPublishedByUserName = statementRecord.PublishedBy > 0 ? statementPublishedUserRecords.Where(usr => usr.Id == statementRecord.PublishedBy).ToList()?.Select(itm => new { FullName = itm.FirstName + " " + itm.LastName })?.FirstOrDefault().FullName : "",
                            PublishedOn = statementRecord.PublishedOn ?? DateTime.MinValue,
                            UpdateBy = statementRecord.UpdateBy,
                            Description= statementRecord.Description
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
                    if (statementSearchParameter.StatementOwner != null && statementSearchParameter.StatementOwner != string.Empty)
                    {
                        StringBuilder queryString = new StringBuilder();
                        queryString.Append(string.Format("FirstName.Contains(\"{0}\") or LastName.Contains(\"{1}\") ", statementSearchParameter.StatementOwner, statementSearchParameter.StatementOwner));
                        queryString.Append(string.Format(" and IsDeleted.Equals(false)"));
                        var userRecordIds = nISEntitiesDataContext.UserRecords.Where(queryString.ToString()).ToList().Select(itm => itm.Id).ToList();
                        if (userRecordIds.Count > 0)
                        {
                            queryString = new StringBuilder();
                            queryString.Append(" and (" + string.Join("or ", userRecordIds.Select(item => string.Format("Owner.Equals({0}) ", item))) + ") ");
                            whereClause = whereClause + queryString.ToString();
                        }
                    }
                    statementCount = nISEntitiesDataContext.StatementRecords.Where(whereClause.ToString()).Count();
                }
            }
            catch (Exception)
            {
                throw;
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

                    statements.ToList().ForEach(item =>
                    {
                        StatementRecord statementRecord = statementRecords.FirstOrDefault(data => data.Id == item.Identifier && data.TenantCode == tenantCode && data.IsDeleted == false);
                        statementRecord.LastUpdatedDate = DateTime.Now;
                        statementRecord.UpdateBy = userId;
                        statementRecord.TenantCode = tenantCode;
                        statementRecord.Name = item.Name;
                        statementRecord.Description = item.Description;

                    });

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
                    nISEntitiesDataContext.SaveChanges();

                    long newStatementIdentifier = statementRecordsForClone.Where(p => p.Name == lastStatementRecord.Name && p.Version == Int64.Parse(lastStatementRecord.Version) + 1 + "").Single().Id;

                    IList<StatementPageMapRecord> statementWidgetRecords = nISEntitiesDataContext.StatementPageMapRecords.Where(item => item.StatementId == lastStatementRecord.Id).ToList();
                    IList<StatementPageMapRecord> statementWidgetRecordsForClone = new List<StatementPageMapRecord>();
                    statementWidgetRecords.ToList().ForEach(item =>
                    {
                        statementWidgetRecordsForClone.Add(new StatementPageMapRecord()
                        {
                            //Height = item.Height,
                            //StatementId = newStatementIdentifier,
                            //ReferenceWidgetId = item.ReferenceWidgetId,
                            //TenantCode = tenantCode,
                            //WidgetSetting = item.WidgetSetting,
                            //Width = item.Width,
                            //Xposition = item.Xposition,
                            //Yposition = item.Yposition
                        });
                    });

                    nISEntitiesDataContext.StatementPageMapRecords.AddRange(statementWidgetRecordsForClone);
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
                queryString.Append("PublishedOn >= DateTime(" + searchParameter.StartDate.Year + "," + searchParameter.StartDate.Month + "," + searchParameter.StartDate.Day + ") and ");
            }

            if (this.validationEngine.IsValidDate(searchParameter.EndDate) && !this.validationEngine.IsValidDate(searchParameter.StartDate))
            {
                queryString.Append("PublishedOn <= DateTime(" + searchParameter.EndDate.Year + "," + searchParameter.EndDate.Month + "," + searchParameter.EndDate.Day + ") and ");
            }

            if (this.validationEngine.IsValidDate(searchParameter.StartDate) && this.validationEngine.IsValidDate(searchParameter.EndDate))
            {
                queryString.Append("PublishedOn >= DateTime(" + searchParameter.StartDate.Year + "," + searchParameter.StartDate.Month + "," + searchParameter.StartDate.Day + ") and PublishedOn <= DateTime(" + searchParameter.EndDate.Year + ", " + searchParameter.EndDate.Month + ", " + searchParameter.EndDate.Day + ") and ");
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

        #endregion
    }
}
