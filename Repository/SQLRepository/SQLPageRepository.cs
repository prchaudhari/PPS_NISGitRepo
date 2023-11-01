// <copyright file="SQLPageRepository.cs" company="Websym Solutions Pvt. Ltd.">
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

    public class SQLPageRepository : IPageRepository
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

        public SQLPageRepository(IUnityContainer unityContainer)
        {
            this.unityContainer = unityContainer;
            this.validationEngine = new ValidationEngine();
            this.utility = new Utility();
            this.configurationutility = new ConfigurationUtility(this.unityContainer);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// This method reference to publish page
        /// </summary>
        /// <param name="pageIdentifier"></param>
        /// <param name="tenantCode"></param>
        /// <returns></returns>
        public bool PublishPage(long pageIdentifier, string tenantCode)
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
                    IList<PageRecord> pageRecords = nISEntitiesDataContext.PageRecords.Where(itm => itm.Id == pageIdentifier).ToList();
                    if (pageRecords == null || pageRecords.Count <= 0)
                    {
                        throw new PageNotFoundException(tenantCode);
                    }

                    pageRecords.ToList().ForEach(item =>
                    {
                        item.PublishedBy = userId;
                        item.PublishedOn = DateTime.UtcNow;
                        item.Status = PageStatus.Published.ToString();

                        SystemActivityHistoryRecord record = new SystemActivityHistoryRecord()
                        {
                            Module = ModelConstant.PAGE_SECTION,
                            EntityId = item.Id,
                            EntityName = item.DisplayName,
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
        /// This method adds the specified list of pages in page repository.
        /// </summary>
        /// <param name="pages">The list of pages</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns true if pages are added successfully, else false.
        /// </returns>
        public bool AddPages(IList<Page> pages, string tenantCode)
        {
            bool result = false;
            try
            {
                var claims = ClaimsPrincipal.Current.Identities.First().Claims.ToList();
                int userId;
                int.TryParse(claims?.FirstOrDefault(x => x.Type.Equals("UserId", StringComparison.OrdinalIgnoreCase)).Value, out userId);
                var userFullName = claims?.FirstOrDefault(x => x.Type.Equals("UserFullName", StringComparison.OrdinalIgnoreCase)).Value;

                this.SetAndValidateConnectionString(tenantCode);
                if (this.IsDuplicatePage(pages, "AddOperation", tenantCode))
                {
                    throw new DuplicatePageFoundException(tenantCode);
                }
                
                IList<PageRecord> pageRecords = new List<PageRecord>();
                pages.ToList().ForEach(page =>
                {
                    pageRecords.Add(new PageRecord()
                    {
                        DisplayName = page.DisplayName,
                        PageTypeId = page.PageTypeId,
                        IsActive = true,
                        IsDeleted = false,
                        Status = PageStatus.New.ToString(),
                        CreatedDate = DateTime.Now,
                        LastUpdatedDate = DateTime.Now,
                        Version = "1",
                        Owner = userId,
                        TenantCode = tenantCode,
                        BackgroundImageAssetId = page.BackgroundImageAssetId,
                        BackgroundImageURL = page.BackgroundImageURL
                    });
                });

                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    nISEntitiesDataContext.PageRecords.AddRange(pageRecords);
                    nISEntitiesDataContext.SaveChanges();
                }

                IList<SystemActivityHistoryRecord> Records = new List<SystemActivityHistoryRecord>();
                pages.ToList().ForEach(page =>
                {
                    page.Identifier = pageRecords.Where(p => p.DisplayName == page.DisplayName && p.PageTypeId == page.PageTypeId && p.Version == "1").Single().Id;

                    //Add page widgets in to page widget map
                    if (page.PageWidgets?.Count > 0)
                    {
                        this.AddPageWidgets(page.PageWidgets, page.Identifier, tenantCode);
                    }

                    Records.Add(new SystemActivityHistoryRecord()
                    {
                        Module = ModelConstant.PAGE_SECTION,
                        EntityId = page.Identifier,
                        EntityName = page.DisplayName,
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
            catch(Exception ex)
            {
                throw ex;
            }

            return result;
        }

        /// <summary>
        /// This method reference to preview page
        /// </summary>
        /// <param name="pageIdentifier"></param>
        /// <param name="tenantCode"></param>
        /// <returns></returns>
        public bool PreviewPage(long pageIdentifier, string tenantCode)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// This method deletes the specified list of pages from page repository.
        /// </summary>
        /// <param name="pageIdentifier">The page identifier</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns true if pages are deleted successfully, else false.
        /// </returns>
        public bool DeletePages(long pageIdentifier, string tenantCode)
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
                    var statementRecords = (from s in nISEntitiesDataContext.StatementRecords
                                   join sp in nISEntitiesDataContext.StatementPageMapRecords on s.Id equals sp.StatementId
                                   where s.IsDeleted == false && sp.ReferencePageId == pageIdentifier
                                   select new { s.Id }).ToList();
                    if (statementRecords.Count > 0)
                    {
                        throw new PageReferenceException(tenantCode);
                    }

                    IList<PageRecord> pageRecords = nISEntitiesDataContext.PageRecords.Where(itm => itm.Id == pageIdentifier).ToList();
                    if (pageRecords == null || pageRecords.Count <= 0)
                    {
                        throw new PageNotFoundException(tenantCode);
                    }

                    pageRecords.ToList().ForEach(item =>
                    {
                        item.IsDeleted = true;
                        item.UpdateBy = userId;
                        item.LastUpdatedDate = DateTime.Now;

                        SystemActivityHistoryRecord record = new SystemActivityHistoryRecord()
                        {
                            Module = ModelConstant.PAGE_SECTION,
                            EntityId = item.Id,
                            EntityName = item.DisplayName,
                            SubEntityId = null,
                            SubEntityName = null,
                            ActionTaken = "Delete",
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
        /// This method gets the specified list of pages from page repository.
        /// </summary>
        /// <param name="pageSearchParameter">The page search parameter</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of pages
        /// </returns>
        public IList<Page> GetPages(PageSearchParameter pageSearchParameter, string tenantCode)
        {
            IList<Page> pages = new List<Page>();

            try 
            {
                this.SetAndValidateConnectionString(tenantCode);
                string whereClause = this.WhereClauseGenerator(pageSearchParameter, tenantCode);

                IList<View_PageRecord> view_PageRecords = new List<View_PageRecord>();
                IList<WidgetRecord> widgetRecords = new List<WidgetRecord>();
                IList<View_DynamicWidgetRecord> dynamicWidgetRecords = new List<View_DynamicWidgetRecord>();
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString)) 
                {
                    if (pageSearchParameter.PagingParameter.PageIndex > 0 && pageSearchParameter.PagingParameter.PageSize > 0)
                    {
                        view_PageRecords = nISEntitiesDataContext.View_PageRecord
                        .OrderBy(pageSearchParameter.SortParameter.SortColumn + " " + pageSearchParameter.SortParameter.SortOrder.ToString())
                        .Where(whereClause)
                        .Skip((pageSearchParameter.PagingParameter.PageIndex - 1) * pageSearchParameter.PagingParameter.PageSize)
                        .Take(pageSearchParameter.PagingParameter.PageSize)
                        .ToList();
                    }
                    else
                    {
                        view_PageRecords = nISEntitiesDataContext.View_PageRecord
                        .Where(whereClause)
                        .OrderBy(pageSearchParameter.SortParameter.SortColumn + " " + pageSearchParameter.SortParameter.SortOrder.ToString().ToLower())
                        .ToList();
                    }

                    if (pageSearchParameter.IsPageWidgetsRequired)
                    {
                        view_PageRecords.ToList().ForEach(pageRecord =>
                        {
                            pageRecord.PageWidgetMapRecords = nISEntitiesDataContext.View_PageWidgetMapRecord.Where(itm => itm.PageId == pageRecord.Id && itm.TenantCode == tenantCode).ToList();
                        });
                    }
                }
                
                if (view_PageRecords != null && view_PageRecords.ToList().Count > 0)
                {
                    view_PageRecords?.ToList().ForEach(pageRecord =>
                    {
                        IList<PageWidget> pageWidgets = new List<PageWidget>();
                        if (pageRecord.PageWidgetMapRecords?.ToList().Count > 0)
                        {
                            pageRecord.PageWidgetMapRecords?.ToList().ForEach(pageWidgetRecord =>
                            {
                                pageWidgets.Add(new PageWidget
                                {
                                    Identifier = pageWidgetRecord.Id,
                                    PageId = pageWidgetRecord.PageId,
                                    Height = pageWidgetRecord.Height,
                                    WidgetId = pageWidgetRecord.ReferenceWidgetId,
                                    WidgetName = pageWidgetRecord.WidgetName,
                                    Width = pageWidgetRecord.Width,
                                    Xposition = pageWidgetRecord.Xposition,
                                    Yposition = pageWidgetRecord.Yposition,
                                    WidgetSetting = pageWidgetRecord.WidgetSetting,
                                    IsDynamicWidget = pageWidgetRecord.IsDynamicWidget
                                });
                            });
                        }

                        pages.Add(new Page
                        {
                            Identifier = pageRecord.Id,
                            DisplayName = pageRecord.DisplayName,
                            CreatedDate = pageRecord.CreatedDate ?? (DateTime)pageRecord.CreatedDate,
                            IsActive = pageRecord.IsActive,
                            IsDeleted = pageRecord.IsDeleted,
                            LastUpdatedDate = pageRecord.LastUpdatedDate ?? (DateTime)pageRecord.LastUpdatedDate,
                            PageOwner = pageRecord.Owner,
                            PageOwnerName = pageRecord.PageOwnerName,
                            PageWidgets = pageWidgets,
                            Status = pageRecord.Status,
                            Version = pageRecord.Version,
                            PageTypeId = pageRecord.PageTypeId,
                            PageTypeName = pageRecord.Name,
                            PublishedBy = pageRecord.PublishedBy ?? 0,
                            PagePublishedByUserName = pageRecord.PublishedByName,
                            PublishedOn = pageRecord.PublishedOn != null ? DateTime.SpecifyKind((DateTime)pageRecord.PublishedOn, DateTimeKind.Utc) : DateTime.MinValue,
                            UpdatedBy = pageRecord.UpdateBy ?? 0,
                            BackgroundImageAssetLibraryId = pageRecord.BackgroundImageAssetLibraryId ?? 0,
                            BackgroundImageAssetId = pageRecord.BackgroundImageAssetId ?? 0,
                            BackgroundImageURL = pageRecord.BackgroundImageURL,
                            HeaderHTML = pageRecord.HeaderHTML,
                            FooterHTML = pageRecord.FooterHTML,
                        });
                    });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return pages;
        }

        /// <summary>
        /// This method gets the specified list of pages from page repository.
        /// </summary>
        /// <param name="pageSearchParameter">The page search parameter</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of pages
        /// </returns>
        public IList<Page> GetPagesForList(PageSearchParameter pageSearchParameter, string tenantCode)
        {
            IList<Page> pages = new List<Page>();

            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                string whereClause = this.WhereClauseGenerator(pageSearchParameter, tenantCode);

                IList<View_PageRecord> pageRecords = new List<View_PageRecord>();
                IList<UserRecord> pageOwnerUserRecords = new List<UserRecord>();
                IList<UserRecord> pagePublishedUserRecords = new List<UserRecord>();
                IList<PageTypeRecord> pageTypeRecords = new List<PageTypeRecord>();

                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    if (pageSearchParameter.PageOwner != null && pageSearchParameter.PageOwner != string.Empty)
                    {
                        StringBuilder queryString = new StringBuilder();
                        queryString.Append(string.Format("(FirstName+\" \"+LastName).Contains(\"{0}\")", pageSearchParameter.PageOwner));

                        queryString.Append(string.Format(" and IsDeleted.Equals(false)"));
                        var userRecordIds = nISEntitiesDataContext.UserRecords.Where(queryString.ToString()).ToList().Select(itm => itm.Id).ToList();
                        if (userRecordIds.Count > 0)
                        {
                            queryString = new StringBuilder();
                            queryString.Append(" and (" + string.Join("or ", userRecordIds.Select(item => string.Format("Owner.Equals({0}) ", item))) + ") ");
                            whereClause = whereClause + queryString.ToString();
                        }
                    }

                    if (pageSearchParameter.PagingParameter.PageIndex > 0 && pageSearchParameter.PagingParameter.PageSize > 0)
                    {
                        pageRecords = nISEntitiesDataContext.View_PageRecord
                        .OrderBy(pageSearchParameter.SortParameter.SortColumn + " " + pageSearchParameter.SortParameter.SortOrder.ToString())
                        .Where(whereClause)
                        .Skip((pageSearchParameter.PagingParameter.PageIndex - 1) * pageSearchParameter.PagingParameter.PageSize)
                        .Take(pageSearchParameter.PagingParameter.PageSize)
                        .ToList();
                    }
                    else
                    {
                        pageRecords = nISEntitiesDataContext.View_PageRecord
                        .Where(whereClause)
                        .OrderBy(pageSearchParameter.SortParameter.SortColumn + " " + pageSearchParameter.SortParameter.SortOrder.ToString().ToLower())
                        .ToList();
                    }

                    if (pageRecords != null && pageRecords.ToList().Count > 0)
                    {
                        StringBuilder userIdentifier = new StringBuilder();
                        userIdentifier.Append("(" + string.Join(" or ", pageRecords.Select(item => string.Format("Id.Equals({0})", item.Owner))) + ")");
                        userIdentifier.Append(string.Format(" and IsDeleted.Equals(false)"));
                        pageOwnerUserRecords = nISEntitiesDataContext.UserRecords.Where(userIdentifier.ToString()).ToList();

                        var publisheByUserIds = pageRecords.Where(itm => itm.PublishedBy != null).ToList();
                        if (publisheByUserIds.Count > 0)
                        {
                            userIdentifier = new StringBuilder();
                            userIdentifier.Append("(" + string.Join(" or ", publisheByUserIds.Select(item => string.Format("Id.Equals({0})", item.PublishedBy))) + ")");
                            userIdentifier.Append(string.Format(" and IsDeleted.Equals(false)"));
                            pagePublishedUserRecords = nISEntitiesDataContext.UserRecords.Where(userIdentifier.ToString()).ToList();
                        }
                        pageTypeRecords = nISEntitiesDataContext.PageTypeRecords.Where(itm => itm.IsActive == true && itm.IsDeleted == false).ToList();
                    }
                }

                if (pageRecords != null && pageRecords.ToList().Count > 0)
                {
                    pageRecords?.ToList().ForEach(pageRecord =>
                    {
                        IList<PageWidget> pageWidgets = new List<PageWidget>();

                        pages.Add(new Page
                        {
                            Identifier = pageRecord.Id,
                            DisplayName = pageRecord.DisplayName,
                            CreatedDate = pageRecord.CreatedDate ?? (DateTime)pageRecord.CreatedDate,
                            IsActive = pageRecord.IsActive,
                            IsDeleted = pageRecord.IsDeleted,
                            LastUpdatedDate = pageRecord.LastUpdatedDate ?? (DateTime)pageRecord.LastUpdatedDate,
                            PageOwner = pageRecord.Owner,
                            PageOwnerName = pageOwnerUserRecords.Where(usr => usr.Id == pageRecord.Owner).ToList()?.Select(itm => new { FullName = itm.FirstName + " " + itm.LastName })?.FirstOrDefault().FullName,
                            PageWidgets = pageWidgets,
                            Status = pageRecord.Status,
                            Version = pageRecord.Version,
                            PageTypeId = pageRecord.PageTypeId,
                            PageTypeName = pageTypeRecords.FirstOrDefault(itm => itm.Id == pageRecord.PageTypeId)?.Name,
                            PublishedBy = pageRecord.PublishedBy ?? 0,
                            PagePublishedByUserName = pageRecord.PublishedBy != null ? pagePublishedUserRecords.Where(usr => usr.Id == pageRecord.PublishedBy).ToList()?.Select(itm => new { FullName = itm.FirstName + " " + itm.LastName })?.FirstOrDefault().FullName : "",
                            PublishedOn = pageRecord.PublishedOn != null ? DateTime.SpecifyKind((DateTime)pageRecord.PublishedOn, DateTimeKind.Utc) : DateTime.MinValue,
                            UpdatedBy = pageRecord.UpdateBy ?? 0,
                        });
                    });
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return pages;
        }

        /// <summary>
        /// This method reference to get page count
        /// </summary>
        /// <param name="pageSearchParameter"></param>
        /// <param name="tenantCode"></param>
        /// <returns>Page count</returns>
        public int GetPageCount(PageSearchParameter pageSearchParameter, string tenantCode)
        {
            int pageCount = 0;
            string whereClause = this.WhereClauseGenerator(pageSearchParameter, tenantCode);
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    pageCount = nISEntitiesDataContext.View_PageRecord.Where(whereClause.ToString()).Count();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return pageCount;
        }

        /// <summary>
        /// This method updates the specified list of pages in page repository.
        /// </summary>
        /// <param name="pages">The list of pages</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns true if pages are updated successfully, else false.
        /// </returns>
        public bool UpdatePages(IList<Page> pages, string tenantCode)
        {
            bool result = false;
            try
            {
                var claims = ClaimsPrincipal.Current.Identities.First().Claims.ToList();
                int userId;
                int.TryParse(claims?.FirstOrDefault(x => x.Type.Equals("UserId", StringComparison.OrdinalIgnoreCase)).Value, out userId);
                var userFullName = claims?.FirstOrDefault(x => x.Type.Equals("UserFullName", StringComparison.OrdinalIgnoreCase)).Value;

                this.SetAndValidateConnectionString(tenantCode);
                if (this.IsDuplicatePage(pages, "UpdateOperation", tenantCode))
                {
                    throw new DuplicatePageFoundException(tenantCode);
                }

                IList<PageRecord> pageRecords = new List<PageRecord>();
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    StringBuilder query = new StringBuilder();
                    query.Append("(" + string.Join("or ", string.Join(",", pages.Select(item => item.Identifier).Distinct()).ToString().Split(',').Select(item => string.Format("Id.Equals({0}) ", item))) + ") ");

                    pageRecords = nISEntitiesDataContext.PageRecords.Where(query.ToString()).Select(item => item).AsQueryable().ToList();
                    if (pageRecords == null || pageRecords.Count <= 0 || pageRecords.Count() != string.Join(",", pageRecords.Select(item => item.Id).Distinct()).ToString().Split(',').Length)
                    {
                        throw new PageNotFoundException(tenantCode);
                    }

                    pages.ToList().ForEach(item =>
                    {
                        PageRecord pageRecord = pageRecords.FirstOrDefault(data => data.Id == item.Identifier && data.TenantCode == tenantCode && data.IsDeleted == false);
                        pageRecord.DisplayName = item.DisplayName;
                        pageRecord.LastUpdatedDate = DateTime.Now;
                        pageRecord.UpdateBy = userId;
                        pageRecord.TenantCode = tenantCode;
                        pageRecord.BackgroundImageAssetId = item.BackgroundImageAssetId;
                        pageRecord.BackgroundImageURL = item.BackgroundImageURL;
                    });

                    nISEntitiesDataContext.SaveChanges();
                }

                IList<SystemActivityHistoryRecord> Records = new List<SystemActivityHistoryRecord>();
                pages.ToList().ForEach(item =>
                {
                    item.Identifier = pageRecords.ToList().Where(pageRec => pageRec.DisplayName == item.DisplayName && pageRec.PageTypeId == item.PageTypeId).FirstOrDefault().Id;

                    if (item.PageWidgets?.Count > 0)
                    {
                        using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                        {
                            var existingPageWidgetMappingRecords = nISEntitiesDataContext.PageWidgetMapRecords.Where(itm => itm.PageId == item.Identifier).ToList();
                            nISEntitiesDataContext.PageWidgetMapRecords.RemoveRange(existingPageWidgetMappingRecords);
                            nISEntitiesDataContext.SaveChanges();
                        }
                        this.AddPageWidgets(item.PageWidgets, item.Identifier, tenantCode);
                    }

                    Records.Add(new SystemActivityHistoryRecord()
                    {
                        Module = ModelConstant.PAGE_SECTION,
                        EntityId = item.Identifier,
                        EntityName = item.DisplayName,
                        SubEntityId = null,
                        SubEntityName = null,
                        ActionTaken = "Update",
                        ActionTakenBy = userId,
                        ActionTakenByUserName = userFullName,
                        ActionTakenDate = DateTime.Now,
                        TenantCode = tenantCode
                    });
                });

                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    nISEntitiesDataContext.SystemActivityHistoryRecords.AddRange(Records);
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
        /// This method reference to clone page
        /// </summary>
        /// <param name="pageIdentifier"></param>
        /// <param name="tenantCode"></param>
        /// <returns>return true if page clone successfully, else false</returns>
        public bool ClonePage(long pageIdentifier, string tenantCode)
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
                    var pageRecord = nISEntitiesDataContext.PageRecords.Where(itm => itm.Id == pageIdentifier).FirstOrDefault();
                    if (pageRecord == null)
                    {
                        throw new PageNotFoundException(tenantCode);
                    }

                    var lastPageRecord = nISEntitiesDataContext.PageRecords.Where(item => item.DisplayName == pageRecord.DisplayName && item.PageTypeId == pageRecord.PageTypeId && item.IsDeleted == false).OrderByDescending(itm => itm.Id).FirstOrDefault();
                    if (lastPageRecord == null)
                    {
                        throw new PageNotFoundException(tenantCode);
                    }

                    IList<PageRecord> pageRecordsForClone = new List<PageRecord>();
                    pageRecordsForClone.Add(new PageRecord()
                    {
                        DisplayName = lastPageRecord.DisplayName,
                        PageTypeId = lastPageRecord.PageTypeId,
                        IsActive = true,
                        IsDeleted = false,
                        Status = PageStatus.New.ToString(),
                        CreatedDate = DateTime.Now,
                        LastUpdatedDate = DateTime.Now,
                        Version = Int64.Parse(lastPageRecord.Version) + 1 + "",
                        Owner = userId,
                        TenantCode = tenantCode,
                        BackgroundImageAssetId = lastPageRecord.BackgroundImageAssetId,
                        BackgroundImageURL = lastPageRecord.BackgroundImageURL
                    });

                    nISEntitiesDataContext.PageRecords.AddRange(pageRecordsForClone);
                    nISEntitiesDataContext.SaveChanges();

                    long newPageIdentifier = pageRecordsForClone.Where(p => p.DisplayName == lastPageRecord.DisplayName && p.PageTypeId == lastPageRecord.PageTypeId && p.Version == Int64.Parse(lastPageRecord.Version) + 1 + "").Single().Id;

                    IList<PageWidgetMapRecord> pageWidgetRecords = nISEntitiesDataContext.PageWidgetMapRecords.Where(item => item.PageId == lastPageRecord.Id).ToList();
                    IList<PageWidgetMapRecord> pageWidgetRecordsForClone = new List<PageWidgetMapRecord>();
                    pageWidgetRecords.ToList().ForEach(item =>
                    {
                        pageWidgetRecordsForClone.Add(new PageWidgetMapRecord()
                        {
                            Height = item.Height,
                            PageId = newPageIdentifier,
                            ReferenceWidgetId = item.ReferenceWidgetId,
                            TenantCode = tenantCode,
                            WidgetSetting = item.WidgetSetting,
                            Width = item.Width,
                            Xposition = item.Xposition,
                            Yposition = item.Yposition,
                            IsDynamicWidget = item.IsDynamicWidget
                        });
                    });

                    nISEntitiesDataContext.PageWidgetMapRecords.AddRange(pageWidgetRecordsForClone);

                    SystemActivityHistoryRecord record = new SystemActivityHistoryRecord()
                    {
                        Module = ModelConstant.PAGE_SECTION,
                        EntityId = lastPageRecord.Id,
                        EntityName = lastPageRecord.DisplayName,
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
        /// This method gets the specified list of page types from page repository.
        /// </summary>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of page types
        /// </returns>
        public IList<PageType> GetPageTypes(string tenantCode)
        {
            IList<PageType> pageTypes = new List<PageType>();
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                IList<PageTypeRecord> pageTypeRecords = new List<PageTypeRecord>();

                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    pageTypeRecords = nISEntitiesDataContext.PageTypeRecords.Where(item => item.IsActive == true && item.IsDeleted == false && item.TenantCode == tenantCode)?.ToList();
                }
                pageTypeRecords?.ToList().ForEach(pageType => 
                pageTypes.Add(new PageType()
                { 
                    Identifier = pageType.Id,
                    PageTypeName = pageType.Name,
                    IsActive = pageType.IsActive,
                    IsDeleted = pageType.IsDeleted,
                }));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return pageTypes;
        }

        /// <summary>
        /// This method gets the specified list of static as well as dynamic widgets from widgets and dynamic widgets repository.
        /// </summary>
        /// <param name="pageTypeId">The page type identifier</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of static as well as dynamic widgets
        /// </returns>
        public IList<Widget> GetStaticAndDynamicWidgets(long pageTypeId, string tenantCode)
        {
            IList<Widget> widgets = new List<Widget>();
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    var widgetArr = nISEntitiesDataContext.FnGetStaticAndDynamicWidgets(pageTypeId, tenantCode);
                    widgetArr.ToList().ForEach(w =>
                    {
                        widgets.Add(new Widget()
                        {
                            Identifier = w.ID,
                            WidgetName = w.WidgetName,
                            DisplayName = w.DisplayName,
                            PageTypeId = w.PageTypeId ?? 0,
                            Instantiable = w.Instantiable == 0 ? false : true,
                            WidgetType = w.WidgetType
                        });
                    });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return widgets;
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
        private string WhereClauseGenerator(PageSearchParameter searchParameter, string tenantCode)
        {
            StringBuilder queryString = new StringBuilder();

            if (searchParameter.SearchMode == SearchMode.Equals)
            {
                if (validationEngine.IsValidLong(searchParameter.Identifier))
                {
                    queryString.Append("(" + string.Join("or ", searchParameter.Identifier.ToString().Split(',').Select(item => string.Format("Id.Equals({0}) ", item))) + ") and ");
                }
                if (validationEngine.IsValidText(searchParameter.DisplayName))
                {
                    queryString.Append(string.Format("DisplayName.Equals(\"{0}\") and ", searchParameter.DisplayName));
                }
            }
            if (searchParameter.SearchMode == SearchMode.Contains)
            {
                if (validationEngine.IsValidText(searchParameter.DisplayName))
                {
                    queryString.Append(string.Format("DisplayName.Contains(\"{0}\") and ", searchParameter.DisplayName));
                }
            }
            if (validationEngine.IsValidText(searchParameter.PageOwner))
            {
                queryString.Append(string.Format("PageOwnerName.Contains(\"{0}\") and ", searchParameter.PageOwner));
            }
            if (validationEngine.IsValidText(searchParameter.Status))
            {
                queryString.Append(string.Format("Status.Equals(\"{0}\") and ", searchParameter.Status));
            }
            if (validationEngine.IsValidLong(searchParameter.PageTypeId))
            {
                queryString.Append("(" + string.Join("or ", searchParameter.PageTypeId.ToString().Split(',').Select(item => string.Format("PageTypeId.Equals({0}) ", item))) + ") and ");
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
        /// <param name="pages">The pages to save.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns name="result">
        /// Returns true if all elements are not present in repository, false otherwise.
        /// </returns>
        private bool IsDuplicatePage(IList<Page> pages, string operation, string tenantCode)
        {
            bool result = false;
            try
            {
                this.SetAndValidateConnectionString(tenantCode);

                StringBuilder query = new StringBuilder();

                if (operation.Equals(ModelConstant.ADD_OPERATION))
                {
                    query.Append("(" + string.Join(" or ", pages.Select(item => string.Format("DisplayName.Equals(\"{0}\")", item.DisplayName)).ToList()) + ") and IsDeleted.Equals(false) and TenantCode.Equals(\"" + tenantCode + "\")");
                }

                if (operation.Equals(ModelConstant.UPDATE_OPERATION))
                {
                    query.Append("(" + string.Join(" or ", pages.Select(item => string.Format("(DisplayName.Equals(\"{0}\") and Version.Equals(\"{1}\") and !Id.Equals({2}))", item.DisplayName, item.Version, item.Identifier))) + ") and IsDeleted.Equals(false) and TenantCode.Equals(\"" + tenantCode + "\")");
                }

                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    IList<PageRecord> pageRecords = nISEntitiesDataContext.PageRecords.Where(query.ToString()).Select(item => item).AsQueryable().ToList();
                    if (pageRecords.Count > 0)
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
        /// <param name="pages">The pages to save.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns name="result">
        /// Returns true if all elements are not present in repository, false otherwise.
        /// </returns>
        private bool IsDuplicatePageWidget(IList<PageWidget> pageWidgets, long pageIdentifier, string operation, string tenantCode)
        {
            bool result = false;
            try
            {
                this.SetAndValidateConnectionString(tenantCode);

                StringBuilder query = new StringBuilder();

                if (operation.Equals(ModelConstant.ADD_OPERATION))
                {
                    query.Append("(" + string.Join(" or ", pageWidgets.Select(item => string.Format("PageId.equals({0}) and ReferenceWidgetId.Equals({1}) ", pageIdentifier, item.WidgetId)).ToList()) + ") and TenantCode.equals(\"" + tenantCode + "\")");
                }
                if (operation.Equals(ModelConstant.UPDATE_OPERATION))
                {
                    query.Append("(" + string.Join(" or ", pageWidgets.Select(item => string.Format("(PageId.Equals({0}) and ReferenceWidgetId.Equals({1}) and !Id.Equals({2}))", pageIdentifier, item.WidgetId, item.Identifier))) + ") and TenantCode.Equals(\"" + tenantCode + "\")");
                }
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    IList<PageWidgetMapRecord> pageWidgetMapRecords = nISEntitiesDataContext.PageWidgetMapRecords.Where(query.ToString()).Select(item => item).AsQueryable().ToList();
                    if (pageWidgetMapRecords.Count > 0)
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

        private bool AddPageWidgets(IList<PageWidget> pageWidgets, long pageIdentifier, string tenantCode)
        {
            bool result = false;
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                if (this.IsDuplicatePageWidget(pageWidgets, pageIdentifier, "AddOperation", tenantCode))
                {
                    throw new DuplicatePageWidgetFoundException(tenantCode);
                }

                IList<PageWidgetMapRecord> pageWidgetRecords = new List<PageWidgetMapRecord>();
                pageWidgets.ToList().ForEach(pageWidget =>
                {
                    pageWidgetRecords.Add(new PageWidgetMapRecord()
                    {
                        Height = pageWidget.Height,
                        PageId = pageIdentifier,
                        ReferenceWidgetId = pageWidget.WidgetId,
                        TenantCode = tenantCode,
                        WidgetSetting = pageWidget.WidgetSetting,
                        Width = pageWidget.Width,
                        Xposition = pageWidget.Xposition,
                        Yposition = pageWidget.Yposition,
                        IsDynamicWidget = pageWidget.IsDynamicWidget
                    });
                });

                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    nISEntitiesDataContext.PageWidgetMapRecords.AddRange(pageWidgetRecords);
                    nISEntitiesDataContext.SaveChanges();
                    result = true;
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
