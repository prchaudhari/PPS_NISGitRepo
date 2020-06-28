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

  //Page Url Method
  public static get pageGetUrl(): string { return "Page/List" };
  public static get pageAddUrl(): string { return "Page/Add" };
  public static get pageUpdateUrl(): string { return "Page/Update" };
  public static get pageDeleteUrl(): string { return "Page/Delete" };
  public static get pagePublishUrl(): string { return "Page/Publish" };
  public static get pagePreviewUrl(): string { return "Page/Preview" };

}
