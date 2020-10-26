import { Component, OnInit, Injector } from '@angular/core';
import { Constants, DynamicGlobalVariable } from 'src/app/shared/constants/constants';
import { LoginService } from './../login/login.service';
import { Router } from '@angular/router';
import { LocalStorageService } from 'src/app/shared/services/local-storage.service';
import { MessageDialogService } from 'src/app/shared/services/mesage-dialog.service';
import { MultiTenantUserAccessMapService } from '../layout/multi-tenant-user-access-map/multi-tenant-user-access-map.service';
import { NgxUiLoaderService } from 'ngx-ui-loader';
import { ConfigConstants } from '../shared/constants/configConstants';
import { TenantConfigurationService } from '../layout/tenant-configuration/tenantConfiguration.service';

@Component({
  selector: 'app-select-tenant',
  templateUrl: './select-tenant.component.html',
  styleUrls: ['./select-tenant.component.scss']
})
export class SelectTenantComponent implements OnInit {

  public tenants: any[] = [];
  public roleList: any = [];
  public commonRolePrivileges = [];
  public roleDetail;
  public userData;
  public DefaultTenantCode = ConfigConstants.TenantCode;
  public baseURL: string = ConfigConstants.BaseURL;
  public statePrivilegeMap;

  constructor(private injector: Injector,
    private loginService: LoginService,
    private route: Router,
    private _messageDialogService: MessageDialogService,
    private localstorageservice: LocalStorageService,
    private uiLoader: NgxUiLoaderService) { }

  ngOnInit() {
    this.userData = JSON.parse(localStorage.getItem('userClaims'));
    this.statePrivilegeMap = JSON.parse(localStorage.getItem("StatePrivilegeMap"));
    if (this.userData) {
      if(this.userData.IsUserHaveMultiTenantAccess == null || this.userData.IsUserHaveMultiTenantAccess.toLocaleLowerCase() != 'true') {
        this.onCancelTenantSelection();
      }
      this.getTenants();
    }
    else {
      this.onCancelTenantSelection();
    }
  }

  async getTenants() {
    let service = this.injector.get(MultiTenantUserAccessMapService);
    var response = await service.GetUserTenantRoleMap(this.userData.UserIdentifier);
    this.tenants = response.List;
  }

  async onTenantSelect(tenant: any) {
    this.uiLoader.start();
    let tenantcode = tenant.TenantCode;
    if(tenant.TenantType == 'Group') {
      tenantcode = this.DefaultTenantCode == undefined ? '00000000-0000-0000-0000-000000000000' : this.DefaultTenantCode;
    }
    this.userData.Privileges = await this.getUserRoles(tenant.RoleId, tenantcode);
    if (this.roleDetail.IsActive == false) {
      this.uiLoader.stop();
      this._messageDialogService.openDialogBox('Error', "User role is deactivated.", Constants.msgBoxError);
      this.onCancelTenantSelection();
    }
    else {
      if (this.userData.Privileges.length == 0 || this.userData.Privileges == null) {
        this.uiLoader.stop();
        this._messageDialogService.openDialogBox('Error', "User role has no permission assigned", Constants.msgBoxError);
        this.onCancelTenantSelection();
      }
      else {
        let UserTheme = 'Theme0';
        var searchParameter: any = {};
        searchParameter.TenantCode = tenantcode;
        let service = this.injector.get(TenantConfigurationService);
        var response: any = await service.getTenantThemeConfigurations(searchParameter);
        if(response != null && response.length > 0 && response[0].ApplicationTheme != null) {
          UserTheme = response[0].ApplicationTheme;
        }
        var loggedInUser = this.localstorageservice.GetCurrentUser();
        loggedInUser.RoleIdentifier = this.roleDetail.Identifier;
        loggedInUser.RoleName = this.roleDetail.Name;
        loggedInUser.TenantCode = tenant.TenantCode;
        this.localstorageservice.SetCurrentUser(loggedInUser);

        this.userData.RoleIdentifier = this.roleDetail.Identifier;
        this.userData.TenantCode = tenant.TenantCode;
        this.userData.RoleName = this.roleDetail.Name;
        this.userData.UserTheme = UserTheme;
        localStorage.setItem('userClaims', JSON.stringify(this.userData));

        if(tenant.TenantType == 'Group') {
          this.route.navigate(['tenants']);
        }else {
          var userClaimsRolePrivilegeOperations = this.userData.Privileges;
          this.handleTheme(this.userData.UserTheme);
          var isFound = false;
          var state = 0;
          this.statePrivilegeMap.forEach(map => {
            var isPresent = userClaimsRolePrivilegeOperations.filter(p => p.EntityName == map.Entity);
            if (isPresent != undefined && isPresent.length > 0) {
              if (isFound == false) {
                isFound = true;
                state = map.State;
              }
            }
          });
          this.uiLoader.stop();
          if (isFound) {
            this.route.navigate([state]);
          }
        }
      }
    }       
  }

  async getUserRoles(roleIdentifier, TenantCode) {
    let searchParameter: any = {};
    searchParameter.PagingParameter = {};
    searchParameter.PagingParameter.PageIndex = Constants.DefaultPageIndex;
    searchParameter.PagingParameter.PageSize = Constants.DefaultPageSize;
    searchParameter.SortParameter = {};
    searchParameter.SortParameter.SortColumn = Constants.Name;
    searchParameter.SortParameter.SortOrder = Constants.Ascending;
    searchParameter.SearchMode = Constants.Exact;
    searchParameter.IsRequiredRolePrivileges = true;
    searchParameter.Identifier = roleIdentifier;
    searchParameter.TenantCode = TenantCode;
    this.roleList = await this.loginService.getRoles(searchParameter);
    this.roleDetail = this.roleList[0];
    if (this.roleList.length > 0) {
      this.roleList.forEach(role => {
        role.RolePrivileges.forEach(privilege => {
          privilege.RolePrivilegeOperations.forEach(operation => {
            if (!this.rolePrivilegeExists(privilege.EntityName, operation.Operation)) {
              var obj = { EntityName: privilege.EntityName, Operation: operation.Operation }
              this.commonRolePrivileges.push(obj);
            }
          });
        });
      });
    }
    return this.commonRolePrivileges;
  }

  rolePrivilegeExists(entityName, operationName) {
    return this.commonRolePrivileges.some(function (el) {
      return (el.EntityName === entityName && operationName === el.Operation);
    });
  }

  onCancelTenantSelection() {
    this.localstorageservice.removeLocalStorageData();
    this.route.navigate(['login']);
  }

  handleTheme(theme) {
    const dom: any = document.querySelector('body');
    if (theme.toLocaleLowerCase() == 'theme1') {
      dom.classList.add('theme1');
      dom.classList.remove('theme2');
      dom.classList.remove('theme3');
      dom.classList.remove('theme4');
      dom.classList.remove('theme5');
      dom.classList.remove('theme0');
    }
    else if (theme.toLocaleLowerCase() == 'theme2') {
      dom.classList.remove('theme1');
      dom.classList.add('theme2');
      dom.classList.remove('theme3');
      dom.classList.remove('theme4');
      dom.classList.remove('theme5');
      dom.classList.remove('theme0');
    }
    else if (theme.toLocaleLowerCase() == 'theme3') {
      dom.classList.remove('theme1');
      dom.classList.remove('theme2');
      dom.classList.add('theme3');
      dom.classList.remove('theme4');
      dom.classList.remove('theme5');
      dom.classList.remove('theme0');
    }
    else if (theme.toLocaleLowerCase() == 'theme4') {
      dom.classList.remove('theme1');
      dom.classList.remove('theme2');
      dom.classList.remove('theme3');
      dom.classList.add('theme4');
      dom.classList.remove('theme5');
      dom.classList.remove('theme0');
    }
    else if (theme.toLocaleLowerCase() == 'theme5') {
      dom.classList.remove('theme1');
      dom.classList.remove('theme2');
      dom.classList.remove('theme3');
      dom.classList.remove('theme4');
      dom.classList.add('theme5');
      dom.classList.remove('theme0');
    }
    else if (theme.toLocaleLowerCase() == 'theme0') {
      dom.classList.remove('theme1');
      dom.classList.remove('theme2');
      dom.classList.remove('theme3');
      dom.classList.remove('theme4');
      dom.classList.remove('theme5');
      dom.classList.add('theme0');
    }
  }

}
