"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.URLConfiguration = void 0;
var URLConfiguration = /** @class */ (function () {
    function URLConfiguration() {
    }
    Object.defineProperty(URLConfiguration, "roleGetUrl", {
        //Role Url Method
        get: function () { return "Role/List"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "roleAddUrl", {
        get: function () { return "Role/Add"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "roleUpdateUrl", {
        get: function () { return "Role/Update"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "roleDeleteUrl", {
        get: function () { return "Role/Delete"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "roleGetRolePrivileges", {
        get: function () { return "Role/GetRolePrivileges"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "roleGetEntities", {
        get: function () { return "Entity/Get"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "roleActivate", {
        get: function () { return "Role/Activate"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "roleDeactivate", {
        get: function () { return "Role/Deactivate"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "roleCheckIsDeactivateDependencyUrl", {
        get: function () { return "Role/IsDeactivateDependency"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "countryGetUrl", {
        //country method
        get: function () { return "Country/List"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "countryAddUrl", {
        get: function () { return "Country/Add"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "countryUpdateUrl", {
        get: function () { return "Country/Update"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "countryDeleteUrl", {
        get: function () { return "Country/Delete"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "contacttypeGetUrl", {
        //contact type method
        get: function () { return "ContactType/List"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "contacttypeAddUrl", {
        get: function () { return "ContactType/Add"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "contacttypeUpdateUrl", {
        get: function () { return "ContactType/Update"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "contacttypeDeleteUrl", {
        get: function () { return "ContactType/Delete"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "tenantGetUrl", {
        //tenant method
        get: function () { return "Client/Get"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "tenantAddUrl", {
        get: function () { return "Client/Add"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "tenantUpdateUrl", {
        get: function () { return "Client/Update"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "tenantDeleteUrl", {
        get: function () { return "Client/Delete"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "tenantActivate", {
        get: function () { return "Client/Activate"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "tenantDeactivate", {
        get: function () { return "Client/DeActivate"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "tenantContactGetUrl", {
        get: function () { return "TenantContact/List"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "tenantContactAdd", {
        get: function () { return "TenantContact/Add"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "tenantContactUpdate", {
        get: function () { return "TenantContact/Update"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "tenantContactDeleteUrl", {
        get: function () { return "TenantContact/Delete"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "tenantContactSendActivationUrl", {
        get: function () { return "TenantContact/SendActivationLink"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "userGetUrl", {
        //User Url Method
        get: function () { return "User/List"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "userAddUrl", {
        get: function () { return "User/Add"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "userUpdateUrl", {
        get: function () { return "User/Update"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "userDeleteUrl", {
        get: function () { return "User/Delete"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "getCountryCodeUrl", {
        get: function () { return "Country/Get"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "userUnlockUrl", {
        get: function () { return "User/Unlock"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "userlockUrl", {
        get: function () { return "User/Lock"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "userCheckDeleteDependencyUrl", {
        get: function () { return "User/IsDeleteDependency"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "getDesignationUrl", {
        get: function () { return "Designation/Get"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "getLanguageUrl", {
        get: function () { return "Locale/Get"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "getHierarchyOrganizationUnitUrl", {
        get: function () { return "OrganisationUnit/GetOragnaisationUnitHierarchy"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "userActivate", {
        get: function () { return "User/Activate"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "userDeactivate", {
        get: function () { return "User/DeActivate"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "userSendPassword", {
        get: function () { return "User/SendPasswordByMail"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "tenantuserGetUrl", {
        //TenantTenantUser Url Method
        get: function () { return "TenantUser/List"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "tenantuserAddUrl", {
        get: function () { return "TenantUser/Add"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "tenantuserUpdateUrl", {
        get: function () { return "TenantUser/Update"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "tenantuserDeleteUrl", {
        get: function () { return "TenantUser/Delete"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "tenantuserUnlockUrl", {
        get: function () { return "TenantUser/Unlock"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "tenantuserlockUrl", {
        get: function () { return "TenantUser/Lock"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "tenantuserCheckDeleteDependencyUrl", {
        get: function () { return "TenantUser/IsDeleteDependency"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "tenantuserActivate", {
        get: function () { return "TenantUser/Activate"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "tenantuserDeactivate", {
        get: function () { return "TenantUser/DeActivate"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "changePasswordUrl", {
        //Change password--
        get: function () { return "User/ChangePassword"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "forgotPasswordUrl", {
        //Forgot password--
        get: function () { return "User/ResetPassword"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "setPasswordUrl", {
        //Set password
        get: function () { return "User/ConfirmResetPassword"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "profileUpdateUrl", {
        //Profile page
        get: function () { return "Profile/Update"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "profileGetUrl", {
        get: function () { return "Profile/Get"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "logoutUrl", {
        //Logout user--
        get: function () { return "Login/Logout"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "assetLibraryGetUrl", {
        get: function () { return "AssetLibrary/List"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "assetLibraryAddUrl", {
        get: function () { return "AssetLibrary/Add"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "assetLibraryUpdateUrl", {
        get: function () { return "AssetLibrary/Update"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "assetLibraryDeleteUrl", {
        get: function () { return "AssetLibrary/Delete"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "assetLibraryCheckIsDependencyUrl", {
        get: function () { return "AssetLibrary/IsDeleteDependency"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "assetGetUrl", {
        get: function () { return "assetlibrary/asset/list"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "pageGetUrl", {
        //Page Url Method
        get: function () { return "Page/List"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "pageGetPagesForListUrl", {
        get: function () { return "Page/GetPagesForList"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "pageAddUrl", {
        get: function () { return "Page/Add"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "pageUpdateUrl", {
        get: function () { return "Page/Update"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "pageDeleteUrl", {
        get: function () { return "Page/Delete"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "pagePublishUrl", {
        get: function () { return "Page/Publish"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "pagePreviewUrl", {
        get: function () { return "Page/Preview"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "pageCloneUrl", {
        get: function () { return "Page/Clone"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "pageTypeGetUrl", {
        get: function () { return "PageType/List"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "getStaticAndDynamicWidgetsUrl", {
        get: function () { return "Page/GetStaticAndDynamicWidgets"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "widgetGetUrl", {
        //widget Url methhod
        get: function () { return "Widget/List"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "scheduleGetUrl", {
        //Schedule Url Method
        get: function () { return "Schedule/List"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "scheduleAddUrl", {
        get: function () { return "Schedule/Add"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "scheduleUpdateUrl", {
        get: function () { return "Schedule/Update"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "scheduleDeleteUrl", {
        get: function () { return "Schedule/Delete"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "scheduleActivate", {
        get: function () { return "Schedule/Activate"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "scheduleDeactivate", {
        get: function () { return "Schedule/Deactivate"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "scheduleHistoryGetUrl", {
        get: function () { return "Schedule/GetScheduleRunHistories"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "RunScheduleNow", {
        get: function () { return "Schedule/RunScheduleNow"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "ApproveScheduleBatch", {
        get: function () { return "Schedule/ApproveScheduleBatch"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "CleanScheduleBatch", {
        get: function () { return "Schedule/CleanScheduleBatch"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "statementGetUrl", {
        //Statement Url Method
        get: function () { return "Statement/List"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "statementAddUrl", {
        get: function () { return "Statement/Add"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "statementUpdateUrl", {
        get: function () { return "Statement/Update"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "statementDeleteUrl", {
        get: function () { return "Statement/Delete"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "statementPublishUrl", {
        get: function () { return "Statement/Publish"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "statementPreviewUrl", {
        get: function () { return "Statement/Preview"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "statementCloneUrl", {
        get: function () { return "Statement/Clone"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "dynamicWidgetGetUrl", {
        //DynamicWidget Url Method
        get: function () { return "DynamicWidget/List"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "dynamicWidgetAddUrl", {
        get: function () { return "DynamicWidget/Add"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "dynamicWidgetUpdateUrl", {
        get: function () { return "DynamicWidget/Update"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "dynamicWidgetDeleteUrl", {
        get: function () { return "DynamicWidget/Delete"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "dynamicWidgetPublishUrl", {
        get: function () { return "DynamicWidget/Publish"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "dynamicWidgetPreviewUrl", {
        get: function () { return "DynamicWidget/Preview"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "dynamicWidgetCloneUrl", {
        get: function () { return "DynamicWidget/Clone"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "dynamicWidgetGetEntitiesUrl", {
        get: function () { return "DynamicWidget/GetTenantEntities"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "dynamicWidgetGetEntityFieldsUrl", {
        get: function () { return "DynamicWidget/GetEntityFields"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "renderEngineGetUrl", {
        //Render Engine Url Method
        get: function () { return "RenderEngine/List"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "renderEngineAddUrl", {
        get: function () { return "RenderEngine/Add"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "renderEngineUpdateUrl", {
        get: function () { return "RenderEngine/Update"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "renderEngineDeleteUrl", {
        get: function () { return "RenderEngine/Delete"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "renderEngineActivate", {
        get: function () { return "RenderEngine/Activate"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "renderEngineDeactivate", {
        get: function () { return "RenderEngine/DeActivate"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "scheduleLogGetUrl", {
        //Schedule Url Method
        get: function () { return "ScheduleLogs/List"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "scheduleLogGetDetailUrl", {
        get: function () { return "ScheduleLog/ScheduleLogDetail/List"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "reRunScheduleLogGetUrl", {
        get: function () { return "ScheduleLogs/ReRunSchedule"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "reRunScheduleLogDetailGetUrl", {
        get: function () { return "ScheduleLog/ScheduleLogDetail/Retry"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "statementSearchGetUrl", {
        //Statement Search Method
        get: function () { return "StatementSearch/List"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "DashboardGetUrl", {
        //Dashboard Data
        get: function () { return "ScheduleLog/Dashboard/Get"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "multiTenantUserRoleAccessGetUrl", {
        //Multi-tenant user role access mapping Url Method
        get: function () { return "MultiTenantUserRoleAccess/List"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "multiTenantUserRoleAccessAddUrl", {
        get: function () { return "MultiTenantUserRoleAccess/Add"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "multiTenantUserRoleAccessUpdateUrl", {
        get: function () { return "MultiTenantUserRoleAccess/Update"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "multiTenantUserRoleAccessDeleteUrl", {
        get: function () { return "MultiTenantUserRoleAccess/Delete"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "multiTenantUserRoleAccessActivate", {
        get: function () { return "MultiTenantUserRoleAccess/Activate"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "multiTenantUserRoleAccessDeactivate", {
        get: function () { return "MultiTenantUserRoleAccess/Deactivate"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "getUserTenantRoleMap", {
        get: function () { return "MultiTenantUserRoleAccess/GetUserTenants"; },
        enumerable: false,
        configurable: true
    });
    ;
    Object.defineProperty(URLConfiguration, "getParentAndChildTenants", {
        get: function () { return "MultiTenantUserRoleAccess/GetParentAndChildTenants"; },
        enumerable: false,
        configurable: true
    });
    ;
    return URLConfiguration;
}());
exports.URLConfiguration = URLConfiguration;
//# sourceMappingURL=urlconfiguration.js.map