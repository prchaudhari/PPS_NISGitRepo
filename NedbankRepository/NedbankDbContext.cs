using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NedbankRepository
{
    public class NedbankDbContext : DbContext
    {
        public NedbankDbContext() : base("TenantManagerConnectionString")
        {
            Database.SetInitializer<NedbankDbContext>(new NedbankDbInitializer());
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ConfigurationManager>().ToTable("ConfigurationManagers", schemaName: "ConfigurationManager");
            modelBuilder.Entity<DependentOperation>().ToTable("DependentOperations", schemaName: "EntityManager");
            modelBuilder.Entity<Entity>().ToTable("Entities", schemaName: "EntityManager");
            modelBuilder.Entity<Operation>().ToTable("Operations", schemaName: "EntityManager");
            modelBuilder.Entity<AccountMaster>().ToTable("AccountMaster", schemaName: "NIS");
            modelBuilder.Entity<AccountTransaction>().ToTable("AccountTransaction", schemaName: "NIS");
            modelBuilder.Entity<AnalyticsData>().ToTable("AnalyticsData", schemaName: "NIS");
            modelBuilder.Entity<Asset>().ToTable("Asset", schemaName: "NIS");
            modelBuilder.Entity<AssetLibrary>().ToTable("AssetLibrary", schemaName: "NIS");
            modelBuilder.Entity<AssetPathSetting>().ToTable("AssetPathSetting", schemaName: "NIS");
            modelBuilder.Entity<AssetSetting>().ToTable("AssetSetting", schemaName: "NIS");
            modelBuilder.Entity<BatchDetail>().ToTable("BatchDetails", schemaName: "NIS");
            modelBuilder.Entity<BatchMaster>().ToTable("BatchMaster", schemaName: "NIS");
            modelBuilder.Entity<City>().ToTable("City", schemaName: "NIS");
            modelBuilder.Entity<ContactType>().ToTable("ContactType", schemaName: "NIS");
            modelBuilder.Entity<Country>().ToTable("Country", schemaName: "NIS");
            modelBuilder.Entity<CustomerMaster>().ToTable("CustomerMaster", schemaName: "NIS");
            modelBuilder.Entity<CustomerMedia>().ToTable("CustomerMedia", schemaName: "NIS");
            modelBuilder.Entity<DM_AccountAnalysis>().ToTable("DM_AccountAnalysis", schemaName: "NIS");
            modelBuilder.Entity<DM_AccountSummary>().ToTable("DM_AccountSummary", schemaName: "NIS");
            modelBuilder.Entity<DM_AgentDetails>().ToTable("DM_AgentDetails", schemaName: "NIS");
            modelBuilder.Entity<DM_BranchMaster>().ToTable("DM_BranchMaster", schemaName: "NIS");
            modelBuilder.Entity<DM_CustomerMaster>().ToTable("DM_CustomerMaster", schemaName: "NIS");
            modelBuilder.Entity<DM_CustomerNewsAndAlerts>().ToTable("DM_CustomerNewsAndAlerts", schemaName: "NIS");
            modelBuilder.Entity<DM_CustomerProductWiseRewardPoints>().ToTable("DM_CustomerProductWiseRewardPoints", schemaName: "NIS");
            modelBuilder.Entity<DM_CustomerReminderRecos>().ToTable("DM_CustomerReminderRecos", schemaName: "NIS");
            modelBuilder.Entity<DM_CustomerRewardPoints>().ToTable("DM_CustomerRewardPoints", schemaName: "NIS");
            modelBuilder.Entity<DM_CustomerRewardPointsRedeemed>().ToTable("DM_CustomerRewardPointsRedeemed", schemaName: "NIS");
            modelBuilder.Entity<DM_CustomerRewardSpendByCategory>().ToTable("DM_CustomerRewardSpendByCategory", schemaName: "NIS");
            modelBuilder.Entity<DM_ExplanatoryNotes>().ToTable("DM_ExplanatoryNotes", schemaName: "NIS");
            modelBuilder.Entity<DM_GreenbacksMaster>().ToTable("DM_GreenbacksMaster", schemaName: "NIS");
            modelBuilder.Entity<DM_HomeLoanArrears>().ToTable("DM_HomeLoanArrears", schemaName: "NIS");
            modelBuilder.Entity<DM_HomeLoanMaster>().ToTable("DM_HomeLoanMaster", schemaName: "NIS");
            modelBuilder.Entity<DM_HomeLoanSummary>().ToTable("DM_HomeLoanSummary", schemaName: "NIS");
            modelBuilder.Entity<DM_HomeLoanTransaction>().ToTable("DM_HomeLoanTransaction", schemaName: "NIS");
            modelBuilder.Entity<DM_InvestmentMaster>().ToTable("DM_InvestmentMaster", schemaName: "NIS");
            modelBuilder.Entity<DM_InvestmentTransaction>().ToTable("DM_InvestmentTransaction", schemaName: "NIS");
            modelBuilder.Entity<DM_MarketingMessages>().ToTable("DM_MarketingMessages", schemaName: "NIS");
            modelBuilder.Entity<DM_NewsAndAlerts>().ToTable("DM_NewsAndAlerts", schemaName: "NIS");
            modelBuilder.Entity<DM_PersonalLoanArrears>().ToTable("DM_PersonalLoanArrears", schemaName: "NIS");
            modelBuilder.Entity<DM_PersonalLoanMaster>().ToTable("DM_PersonalLoanMaster", schemaName: "NIS");
            modelBuilder.Entity<DM_PersonalLoanTransaction>().ToTable("DM_PersonalLoanTransaction", schemaName: "NIS");
            modelBuilder.Entity<DM_ReminderRecos>().ToTable("DM_ReminderRecos", schemaName: "NIS");
            modelBuilder.Entity<DM_SpecialMessages>().ToTable("DM_SpecialMessages", schemaName: "NIS");
            modelBuilder.Entity<DynamicWidget>().ToTable("DynamicWidget", schemaName: "NIS");
            modelBuilder.Entity<DynamicWidgetFilterDetail>().ToTable("DynamicWidgetFilterDetail", schemaName: "NIS");
            modelBuilder.Entity<EntityFieldMap>().ToTable("EntityFieldMap", schemaName: "NIS");
            modelBuilder.Entity<MultiTenantUserAccessMap>().ToTable("MultiTenantUserAccessMap", schemaName: "NIS");
            modelBuilder.Entity<NB_BranchMaster>().ToTable("NB_BranchMaster", schemaName: "NIS");
            modelBuilder.Entity<NB_CustomerMaster>().ToTable("NB_CustomerMaster", schemaName: "NIS");
            modelBuilder.Entity<NB_InvestmentMaster>().ToTable("NB_InvestmentMaster", schemaName: "NIS");
            modelBuilder.Entity<NB_InvestmentTransaction>().ToTable("NB_InvestmentTransaction", schemaName: "NIS");
            modelBuilder.Entity<Page>().ToTable("Page", schemaName: "NIS");
            modelBuilder.Entity<PageType>().ToTable("PageType", schemaName: "NIS");
            modelBuilder.Entity<PageWidgetMap>().ToTable("PageWidgetMap", schemaName: "NIS");
            modelBuilder.Entity<ReminderAndRecommendation>().ToTable("ReminderAndRecommendation", schemaName: "NIS");
            modelBuilder.Entity<RenderEngine>().ToTable("RenderEngine", schemaName: "NIS");
            modelBuilder.Entity<Role>().ToTable("Role", schemaName: "NIS");
            modelBuilder.Entity<RolePrivilege>().ToTable("RolePrivilege", schemaName: "NIS");
            modelBuilder.Entity<SavingTrend>().ToTable("SavingTrend", schemaName: "NIS");
            modelBuilder.Entity<Schedule>().ToTable("Schedule", schemaName: "NIS");
            modelBuilder.Entity<ScheduleLog>().ToTable("ScheduleLog", schemaName: "NIS");
            modelBuilder.Entity<ScheduleLogArchive>().ToTable("ScheduleLogArchive", schemaName: "NIS");
            modelBuilder.Entity<ScheduleLogDetail>().ToTable("ScheduleLogDetail", schemaName: "NIS");
            modelBuilder.Entity<ScheduleLogDetailArchive>().ToTable("ScheduleLogDetailArchive", schemaName: "NIS");
            modelBuilder.Entity<ScheduleRunHistory>().ToTable("ScheduleRunHistory", schemaName: "NIS");
            modelBuilder.Entity<State>().ToTable("State", schemaName: "NIS");
            modelBuilder.Entity<Statement>().ToTable("Statement", schemaName: "NIS");
            modelBuilder.Entity<StatementMetadata>().ToTable("StatementMetadata", schemaName: "NIS");
            modelBuilder.Entity<StatementMetadataArchive>().ToTable("StatementMetadataArchive", schemaName: "NIS");
            modelBuilder.Entity<StatementPageMap>().ToTable("StatementPageMap", schemaName: "NIS");
            modelBuilder.Entity<SystemActivityHistory>().ToTable("SystemActivityHistory", schemaName: "NIS");
            modelBuilder.Entity<TenantConfiguration>().ToTable("TenantConfiguration", schemaName: "NIS");
            modelBuilder.Entity<TenantContact>().ToTable("TenantContact", schemaName: "NIS");
            modelBuilder.Entity<TenantEntity>().ToTable("TenantEntity", schemaName: "NIS");
            modelBuilder.Entity<TenantSubscription>().ToTable("TenantSubscription", schemaName: "NIS");
            modelBuilder.Entity<TenantUser>().ToTable("TenantUser", schemaName: "NIS");
            modelBuilder.Entity<Top4IncomeSources>().ToTable("Top4IncomeSources", schemaName: "NIS");
            modelBuilder.Entity<TransactionDetail>().ToTable("TransactionDetail", schemaName: "NIS");
            modelBuilder.Entity<TTD_CustomerMaster>().ToTable("TTD_CustomerMaster", schemaName: "NIS");
            modelBuilder.Entity<TTD_DataUsage>().ToTable("TTD_DataUsage", schemaName: "NIS");
            modelBuilder.Entity<TTD_EmailsBySubscription>().ToTable("TTD_EmailsBySubscription", schemaName: "NIS");
            modelBuilder.Entity<TTD_MeetingUsage>().ToTable("TTD_MeetingUsage", schemaName: "NIS");
            modelBuilder.Entity<TTD_SubscriptionMaster>().ToTable("TTD_SubscriptionMaster", schemaName: "NIS");
            modelBuilder.Entity<TTD_SubscriptionSpend>().ToTable("TTD_SubscriptionSpend", schemaName: "NIS");
            modelBuilder.Entity<TTD_SubscriptionSummary>().ToTable("TTD_SubscriptionSummary", schemaName: "NIS");
            modelBuilder.Entity<TTD_SubscriptionUsage>().ToTable("TTD_SubscriptionUsage", schemaName: "NIS");
            modelBuilder.Entity<TTD_UserSubscriptions>().ToTable("TTD_UserSubscriptions", schemaName: "NIS");
            modelBuilder.Entity<TTD_VendorSubscription>().ToTable("TTD_VendorSubscription", schemaName: "NIS");
            modelBuilder.Entity<User>().ToTable("User", schemaName: "NIS");
            modelBuilder.Entity<UserCredentialHistory>().ToTable("UserCredentialHistory", schemaName: "NIS");
            modelBuilder.Entity<UserLogin>().ToTable("UserLogin", schemaName: "NIS");
            modelBuilder.Entity<UserLoginActivityHistory>().ToTable("UserLoginActivityHistory", schemaName: "NIS");
            modelBuilder.Entity<UserRoleMap>().ToTable("UserRoleMap", schemaName: "NIS");
            modelBuilder.Entity<Widget>().ToTable("Widget", schemaName: "NIS");
            modelBuilder.Entity<WidgetPageTypeMap>().ToTable("WidgetPageTypeMap", schemaName: "NIS");
            modelBuilder.Entity<Tenant>().ToTable("Tenant", schemaName: "TenantManager");
            modelBuilder.Entity<TenantSecurityCodeFormat>().ToTable("TenantSecurityCodeFormat", schemaName: "NIS");
            modelBuilder.Entity<LanguageMaster>().ToTable("LanguageMaster", schemaName: "NIS");
            modelBuilder.Entity<LanguageTenantMapping>().ToTable("LanguageTenantMapping", schemaName: "NIS");
            modelBuilder.Entity<NB_SegmentMaster>().ToTable("NB_SegmentMaster", schemaName: "NIS");
        }

        public virtual DbSet<ConfigurationManager> ConfigurationManagers { get; set; }
        public virtual DbSet<DependentOperation> DependentOperations { get; set; }
        public virtual DbSet<Entity> Entities { get; set; }
        public virtual DbSet<Operation> Operations { get; set; }
        public virtual DbSet<AccountMaster> AccountMasters { get; set; }
        public virtual DbSet<AccountTransaction> AccountTransactions { get; set; }
        public virtual DbSet<AnalyticsData> AnalyticsDatas { get; set; }
        public virtual DbSet<Asset> Assets { get; set; }
        public virtual DbSet<AssetLibrary> AssetLibraries { get; set; }
        public virtual DbSet<AssetPathSetting> AssetPathSettings { get; set; }
        public virtual DbSet<AssetSetting> AssetSettings { get; set; }
        public virtual DbSet<BatchDetail> BatchDetails { get; set; }
        public virtual DbSet<BatchMaster> BatchMasters { get; set; }
        public virtual DbSet<City> Cities { get; set; }
        public virtual DbSet<ContactType> ContactTypes { get; set; }
        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<CustomerMaster> CustomerMasters { get; set; }
        public virtual DbSet<CustomerMedia> CustomerMedias { get; set; }
        public virtual DbSet<DM_AccountAnalysis> DM_AccountAnalysis { get; set; }
        public virtual DbSet<DM_AccountSummary> DM_AccountSummary { get; set; }
        public virtual DbSet<DM_AgentDetails> DM_AgentDetails { get; set; }
        public virtual DbSet<DM_BranchMaster> DM_BranchMaster { get; set; }
        public virtual DbSet<DM_CustomerMaster> DM_CustomerMaster { get; set; }
        public virtual DbSet<DM_CustomerNewsAndAlerts> DM_CustomerNewsAndAlerts { get; set; }
        public virtual DbSet<DM_CustomerProductWiseRewardPoints> DM_CustomerProductWiseRewardPoints { get; set; }
        public virtual DbSet<DM_CustomerReminderRecos> DM_CustomerReminderRecos { get; set; }
        public virtual DbSet<DM_CustomerRewardPoints> DM_CustomerRewardPoints { get; set; }
        public virtual DbSet<DM_CustomerRewardPointsRedeemed> DM_CustomerRewardPointsRedeemed { get; set; }
        public virtual DbSet<DM_CustomerRewardSpendByCategory> DM_CustomerRewardSpendByCategory { get; set; }
        public virtual DbSet<DM_ExplanatoryNotes> DM_ExplanatoryNotes { get; set; }
        public virtual DbSet<DM_GreenbacksMaster> DM_GreenbacksMaster { get; set; }
        public virtual DbSet<DM_HomeLoanArrears> DM_HomeLoanArrears { get; set; }
        public virtual DbSet<DM_HomeLoanMaster> DM_HomeLoanMaster { get; set; }
        public virtual DbSet<DM_HomeLoanSummary> DM_HomeLoanSummary { get; set; }
        public virtual DbSet<DM_HomeLoanTransaction> DM_HomeLoanTransaction { get; set; }
        public virtual DbSet<DM_InvestmentMaster> DM_InvestmentMaster { get; set; }
        public virtual DbSet<DM_InvestmentTransaction> DM_InvestmentTransaction { get; set; }
        public virtual DbSet<DM_MarketingMessages> DM_MarketingMessages { get; set; }
        public virtual DbSet<DM_NewsAndAlerts> DM_NewsAndAlerts { get; set; }
        public virtual DbSet<DM_PersonalLoanArrears> DM_PersonalLoanArrears { get; set; }
        public virtual DbSet<DM_PersonalLoanMaster> DM_PersonalLoanMaster { get; set; }
        public virtual DbSet<DM_PersonalLoanTransaction> DM_PersonalLoanTransaction { get; set; }
        public virtual DbSet<DM_ReminderRecos> DM_ReminderRecos { get; set; }
        public virtual DbSet<DM_SpecialMessages> DM_SpecialMessages { get; set; }
        public virtual DbSet<DynamicWidget> DynamicWidgets { get; set; }
        public virtual DbSet<DynamicWidgetFilterDetail> DynamicWidgetFilterDetails { get; set; }
        public virtual DbSet<EntityFieldMap> EntityFieldMaps { get; set; }
        public virtual DbSet<MultiTenantUserAccessMap> MultiTenantUserAccessMaps { get; set; }
        public virtual DbSet<NB_BranchMaster> NB_BranchMaster { get; set; }
        public virtual DbSet<NB_CustomerMaster> NB_CustomerMaster { get; set; }
        public virtual DbSet<NB_InvestmentMaster> NB_InvestmentMaster { get; set; }
        public virtual DbSet<NB_InvestmentTransaction> NB_InvestmentTransaction { get; set; }
        public virtual DbSet<Page> Pages { get; set; }
        public virtual DbSet<PageType> PageTypes { get; set; }
        public virtual DbSet<PageWidgetMap> PageWidgetMaps { get; set; }
        public virtual DbSet<ReminderAndRecommendation> ReminderAndRecommendations { get; set; }
        public virtual DbSet<RenderEngine> RenderEngines { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<RolePrivilege> RolePrivileges { get; set; }
        public virtual DbSet<SavingTrend> SavingTrends { get; set; }
        public virtual DbSet<Schedule> Schedules { get; set; }
        public virtual DbSet<ScheduleLog> ScheduleLogs { get; set; }
        public virtual DbSet<ScheduleLogArchive> ScheduleLogArchives { get; set; }
        public virtual DbSet<ScheduleLogDetail> ScheduleLogDetails { get; set; }
        public virtual DbSet<ScheduleLogDetailArchive> ScheduleLogDetailArchives { get; set; }
        public virtual DbSet<ScheduleRunHistory> ScheduleRunHistories { get; set; }
        public virtual DbSet<State> States { get; set; }
        public virtual DbSet<Statement> Statements { get; set; }
        public virtual DbSet<StatementMetadata> StatementMetadatas { get; set; }
        public virtual DbSet<StatementMetadataArchive> StatementMetadataArchives { get; set; }
        public virtual DbSet<StatementPageMap> StatementPageMaps { get; set; }
        public virtual DbSet<SystemActivityHistory> SystemActivityHistories { get; set; }
        public virtual DbSet<TenantConfiguration> TenantConfigurations { get; set; }
        public virtual DbSet<TenantContact> TenantContacts { get; set; }
        public virtual DbSet<TenantEntity> TenantEntities { get; set; }
        public virtual DbSet<TenantSubscription> TenantSubscriptions { get; set; }
        public virtual DbSet<TenantUser> TenantUsers { get; set; }
        public virtual DbSet<Top4IncomeSources> Top4IncomeSources { get; set; }
        public virtual DbSet<TransactionDetail> TransactionDetails { get; set; }
        public virtual DbSet<TTD_CustomerMaster> TTD_CustomerMaster { get; set; }
        public virtual DbSet<TTD_DataUsage> TTD_DataUsage { get; set; }
        public virtual DbSet<TTD_EmailsBySubscription> TTD_EmailsBySubscription { get; set; }
        public virtual DbSet<TTD_MeetingUsage> TTD_MeetingUsage { get; set; }
        public virtual DbSet<TTD_SubscriptionMaster> TTD_SubscriptionMaster { get; set; }
        public virtual DbSet<TTD_SubscriptionSpend> TTD_SubscriptionSpend { get; set; }
        public virtual DbSet<TTD_SubscriptionSummary> TTD_SubscriptionSummary { get; set; }
        public virtual DbSet<TTD_SubscriptionUsage> TTD_SubscriptionUsage { get; set; }
        public virtual DbSet<TTD_UserSubscriptions> TTD_UserSubscriptions { get; set; }
        public virtual DbSet<TTD_VendorSubscription> TTD_VendorSubscription { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserCredentialHistory> UserCredentialHistories { get; set; }
        public virtual DbSet<UserLogin> UserLogins { get; set; }
        public virtual DbSet<UserLoginActivityHistory> UserLoginActivityHistories { get; set; }
        public virtual DbSet<UserRoleMap> UserRoleMaps { get; set; }
        public virtual DbSet<Widget> Widgets { get; set; }
        public virtual DbSet<WidgetPageTypeMap> WidgetPageTypeMaps { get; set; }
        public virtual DbSet<Tenant> Tenants { get; set; }
        public virtual DbSet<TenantSecurityCodeFormat> TenantSecurityCodeFormats { get; set; }
        public virtual DbSet<LanguageMaster> LanguageMasters { get; set; }
        public virtual DbSet<LanguageTenantMapping> LanguageTenantMappings { get; set; }
        public virtual DbSet<NB_SegmentMaster> NB_SegmentMaster { get; set; }

    }
}
