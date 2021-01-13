export class URLConfiguration {

  //Role Url Method
  public static get roleGetUrl(): string { return "Role/List" };
  public static get roleAddUrl(): string { return "Role/Add" };
  public static get roleUpdateUrl(): string { return "Role/Update" };
  public static get roleDeleteUrl(): string { return "Role/Delete" };
  public static get roleGetRolePrivileges(): string { return "Role/GetRolePrivileges" };
  public static get roleGetEntities(): string { return "Entity/Get" };
  public static get roleActivate(): string { return "Role/Activate" };
  public static get roleDeactivate(): string { return "Role/Deactivate" };
  public static get roleCheckIsDeactivateDependencyUrl(): string { return "Role/IsDeactivateDependency" };

  //country method
  public static get countryGetUrl(): string { return "Country/List" };
  public static get countryAddUrl(): string { return "Country/Add" };
  public static get countryUpdateUrl(): string { return "Country/Update" };
  public static get countryDeleteUrl(): string { return "Country/Delete" };

  //contact type method
  public static get contacttypeGetUrl(): string { return "ContactType/List" };
  public static get contacttypeAddUrl(): string { return "ContactType/Add" };
  public static get contacttypeUpdateUrl(): string { return "ContactType/Update" };
  public static get contacttypeDeleteUrl(): string { return "ContactType/Delete" };

  //Page type method
  public static get pagetypeGetUrl(): string { return "PageType/GetPageTypeList" };
  public static get pagetypeAddUrl(): string { return "PageType/Add" };
  public static get pagetypeUpdateUrl(): string { return "PageType/Update" };
  public static get pagetypeDeleteUrl(): string { return "PageType/Delete" };


  //tenant method
  public static get tenantGetUrl(): string { return "Client/Get" };
  public static get tenantAddUrl(): string { return "Client/Add" };
  public static get tenantUpdateUrl(): string { return "Client/Update" };
  public static get tenantDeleteUrl(): string { return "Client/Delete" };
  public static get tenantActivate(): string { return "Client/Activate" };
  public static get tenantDeactivate(): string { return "Client/DeActivate" };
  public static get tenantContactGetUrl(): string { return "TenantContact/List" };
  public static get tenantContactAdd(): string { return "TenantContact/Add" };
  public static get tenantContactUpdate(): string { return "TenantContact/Update" };
  public static get tenantContactDeleteUrl(): string { return "TenantContact/Delete" };
  public static get tenantContactSendActivationUrl(): string { return "TenantContact/SendActivationLink" };
  public static get tenantSubscriptionGetUrl(): string { return "TenantConfiguration/GetTenantSubscription" };
  public static get tenantSubscriptionsGetUrl(): string { return "TenantConfiguration/GetTenantSubscriptions" };
  public static get tenantSubscriptionAddUrl(): string { return "TenantConfiguration/AddTenantSubscriptions" };
  //User Url Method
  public static get userGetUrl(): string { return "User/List" };
  public static get userAddUrl(): string { return "User/Add" };
  public static get userUpdateUrl(): string { return "User/Update" };
  public static get userDeleteUrl(): string { return "User/Delete" };
  public static get getCountryCodeUrl(): string { return "Country/Get" };
  public static get userUnlockUrl(): string { return "User/Unlock" };
  public static get userlockUrl(): string { return "User/Lock" };
  public static get userCheckDeleteDependencyUrl(): string { return "User/IsDeleteDependency" };
  public static get getDesignationUrl(): string { return "Designation/Get" };
  public static get getLanguageUrl(): string { return "Locale/Get" };
  public static get getHierarchyOrganizationUnitUrl(): string { return "OrganisationUnit/GetOragnaisationUnitHierarchy" };
  public static get userActivate(): string { return "User/Activate" };
  public static get userDeactivate(): string { return "User/DeActivate" };
  public static get userSendPassword(): string { return "User/SendPasswordByMail" };

  //TenantTenantUser Url Method
  public static get tenantuserGetUrl(): string { return "TenantUser/List" };
  public static get tenantuserAddUrl(): string { return "TenantUser/Add" };
  public static get tenantuserUpdateUrl(): string { return "TenantUser/Update" };
  public static get tenantuserDeleteUrl(): string { return "TenantUser/Delete" };
  public static get tenantuserUnlockUrl(): string { return "TenantUser/Unlock" };
  public static get tenantuserlockUrl(): string { return "TenantUser/Lock" };
  public static get tenantuserCheckDeleteDependencyUrl(): string { return "TenantUser/IsDeleteDependency" };
  public static get tenantuserActivate(): string { return "TenantUser/Activate" };
  public static get tenantuserDeactivate(): string { return "TenantUser/DeActivate" };

  //Change password--
  public static get changePasswordUrl(): string { return "User/ChangePassword" };

  //Forgot password--
  public static get forgotPasswordUrl(): string { return "User/ResetPassword" };

  //Set password
  public static get setPasswordUrl(): string { return "User/ConfirmResetPassword" };


  //Profile page
  public static get profileUpdateUrl(): string { return "Profile/Update" };
  public static get profileGetUrl(): string { return "Profile/Get" };

  //Logout user--
  public static get logoutUrl(): string { return "Login/Logout" };

  public static get assetLibraryGetUrl(): string { return "AssetLibrary/List" };
  public static get assetLibraryAddUrl(): string { return "AssetLibrary/Add" };
  public static get assetLibraryUpdateUrl(): string { return "AssetLibrary/Update" };
  public static get assetLibraryDeleteUrl(): string { return "AssetLibrary/Delete" };
  public static get assetLibraryCheckIsDependencyUrl(): string { return "AssetLibrary/IsDeleteDependency" };

  public static get assetGetUrl(): string { return "assetlibrary/asset/list" };

  //Page Url Method
  public static get pageGetUrl(): string { return "Page/List" };
  public static get pageGetPagesForListUrl(): string { return "Page/GetPagesForList" };
  public static get pageAddUrl(): string { return "Page/Add" };
  public static get pageUpdateUrl(): string { return "Page/Update" };
  public static get pageDeleteUrl(): string { return "Page/Delete" };
  public static get pagePublishUrl(): string { return "Page/Publish" };
  public static get pagePreviewUrl(): string { return "Page/Preview" };
  public static get pageCloneUrl(): string { return "Page/Clone" };
  public static get pageTypeGetUrl(): string { return "PageType/List" };
  public static get getStaticAndDynamicWidgetsUrl(): string { return "Page/GetStaticAndDynamicWidgets" };

  //widget Url methhod
  public static get widgetGetUrl(): string { return "Widget/List" };

  //Schedule Url Method
  public static get scheduleGetUrl(): string { return "Schedule/List" };
  public static get scheduleAddUrl(): string { return "Schedule/Add" };
  public static get scheduleUpdateUrl(): string { return "Schedule/Update" };
  public static get scheduleDeleteUrl(): string { return "Schedule/Delete" };
  public static get scheduleActivate(): string { return "Schedule/Activate" };
  public static get scheduleDeactivate(): string { return "Schedule/Deactivate" };
  public static get scheduleHistoryGetUrl(): string { return "Schedule/GetScheduleRunHistories" };
  public static get RunScheduleNow(): string { return "Schedule/RunScheduleNow" };
  public static get ValidateApproveScheduleBatch(): string { return "Schedule/ValidateApproveScheduleBatch" };
  public static get ApproveScheduleBatch(): string { return "Schedule/ApproveScheduleBatch" };

  public static get CleanScheduleBatch(): string { return "Schedule/CleanScheduleBatch" };

  //Statement Url Method
  public static get statementGetUrl(): string { return "Statement/List" };
  public static get statementAddUrl(): string { return "Statement/Add" };
  public static get statementUpdateUrl(): string { return "Statement/Update" };
  public static get statementDeleteUrl(): string { return "Statement/Delete" };
  public static get statementPublishUrl(): string { return "Statement/Publish" };
  public static get statementPreviewUrl(): string { return "Statement/Preview" };
  public static get statementCloneUrl(): string { return "Statement/Clone" };

  //DynamicWidget Url Method
  public static get dynamicWidgetGetUrl(): string { return "DynamicWidget/List" };
  public static get dynamicWidgetAddUrl(): string { return "DynamicWidget/Add" };
  public static get dynamicWidgetUpdateUrl(): string { return "DynamicWidget/Update" };
  public static get dynamicWidgetDeleteUrl(): string { return "DynamicWidget/Delete" };
  public static get dynamicWidgetPublishUrl(): string { return "DynamicWidget/Publish" };
  public static get dynamicWidgetPreviewUrl(): string { return "DynamicWidget/Preview" };
  public static get dynamicWidgetCloneUrl(): string { return "DynamicWidget/Clone" };
  public static get dynamicWidgetGetEntitiesUrl(): string { return "DynamicWidget/GetTenantEntities" };
  public static get dynamicWidgetGetEntityFieldsUrl(): string { return "DynamicWidget/GetEntityFields" };

  //Render Engine Url Method
  public static get renderEngineGetUrl(): string { return "RenderEngine/List" };
  public static get renderEngineAddUrl(): string { return "RenderEngine/Add" };
  public static get renderEngineUpdateUrl(): string { return "RenderEngine/Update" };
  public static get renderEngineDeleteUrl(): string { return "RenderEngine/Delete" };
  public static get renderEngineActivate(): string { return "RenderEngine/Activate" };
  public static get renderEngineDeactivate(): string { return "RenderEngine/DeActivate" };

  //Schedule Url Method
  public static get scheduleLogGetUrl(): string { return "ScheduleLogs/List" };
  public static get scheduleLogGetDetailUrl(): string { return "ScheduleLog/ScheduleLogDetail/List" };
  public static get reRunScheduleLogGetUrl(): string { return "ScheduleLogs/ReRunSchedule" };
  public static get reRunScheduleLogDetailGetUrl(): string { return "ScheduleLog/ScheduleLogDetail/Retry" };

  //Statement Search Method
  public static get statementSearchGetUrl(): string { return "StatementSearch/List" };

  //Dashboard Data
  public static get DashboardGetUrl(): string { return "ScheduleLog/Dashboard/Get" };

  //Multi-tenant user role access mapping Url Method
  public static get multiTenantUserRoleAccessGetUrl(): string { return "MultiTenantUserRoleAccess/List" };
  public static get multiTenantUserRoleAccessAddUrl(): string { return "MultiTenantUserRoleAccess/Add" };
  public static get multiTenantUserRoleAccessUpdateUrl(): string { return "MultiTenantUserRoleAccess/Update" };
  public static get multiTenantUserRoleAccessDeleteUrl(): string { return "MultiTenantUserRoleAccess/Delete" };
  public static get multiTenantUserRoleAccessActivate(): string { return "MultiTenantUserRoleAccess/Activate" };
  public static get multiTenantUserRoleAccessDeactivate(): string { return "MultiTenantUserRoleAccess/Deactivate" };
  public static get getUserTenantRoleMap(): string { return "MultiTenantUserRoleAccess/GetUserTenants" };
  public static get getParentAndChildTenants(): string { return "MultiTenantUserRoleAccess/GetParentAndChildTenants" };

}
