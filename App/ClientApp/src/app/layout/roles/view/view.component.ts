import { Component, OnInit, Injector } from '@angular/core';
import { Constants } from 'src/app/shared/constants/constants';
import { RoleService } from '../role.service';
import { ActivatedRoute, Router, NavigationEnd } from '@angular/router';
import { NgxUiLoaderService } from 'ngx-ui-loader';
import { MessageDialogService } from 'src/app/shared/services/mesage-dialog.service';
import { Entity } from 'src/app/shared/models/roleEntity';
import { LocalStorageService } from 'src/app/shared/services/local-storage.service';
import { Role } from '../role';
import { UserService } from '../../users/user.service';

@Component({
  selector: 'app-view',
  templateUrl: './view.component.html',
  styleUrls: ['./view.component.scss']
})
export class ViewComponent implements OnInit {

  //variables used
  public isCollapsedRoles: boolean = true;
  public isCollapsedDetails: boolean = false;
  public isCollapsedPermissions: boolean = true;
  public isCollapsedUsers: boolean = true;
  public usersList = [];
  public IsUserDetailsGet = false;

  public isLoaderActive: boolean = false;
  public params: any = {};
  public RoleIdentifier: number;
  public RoleName;
  public RoleDesc;
  public isRecordFound;
  public rolePrivileges = [];
  public roleViewResources = {}
  public ResourceLoadingFailedMsg = Constants.ResourceLoadingFailedMsg;
  public trackByFn;
  public rolePrivilegesList = [];
  public entityList: Entity[] = [];
  public Locale;
  public roleView: Role[] = [];
  public roleListResources = {};
  public Identifier;
  public Name;
  public role: Role;
  public userClaimsRolePrivilegeOperations;
  public isRolePrivilegeIsAssigned: boolean = true;
  public roleRecord: any = {};
  public roleStatus: any;
  public loggedInUserIdentifier;
  constructor(
    private injector: Injector,
    private uiLoader: NgxUiLoaderService,
    private _messageDialogService: MessageDialogService,
    private router: Router,
    private localstorageservice: LocalStorageService,
    private roleService: RoleService
  ) {
    //this.getResources();
    //getting localstorage item
    router.events.subscribe(e => {
      if (e instanceof NavigationEnd) {
        if (e.url.includes('/roles')) {
          //set passing parameters to localstorage.
          if (localStorage.getItem('roleparams')) {
            this.params = JSON.parse(localStorage.getItem('roleparams'));
            this.RoleIdentifier = this.params.Routeparams.passingparams.RoleIdentifier
            this.RoleName = this.params.Routeparams.filteredparams.RoleName
            this.roleStatus = this.params.Routeparams.filteredparams.IsActive
          }
        } else {
          localStorage.removeItem("roleparams");
        }
      }
    });
  }

  ngOnInit() {
    var userClaimsDetail = JSON.parse(localStorage.getItem('userClaims'));
    this.userClaimsRolePrivilegeOperations = userClaimsDetail.Privileges;
    this.loggedInUserIdentifier = userClaimsDetail.UserIdentifier;
    this.getRoleRecords();
  }

  //This method has been used for fetching role records
  async getRoleRecords() {
    let roleService = this.injector.get(RoleService);
    let searchParameter: any = {};
    searchParameter.PagingParameter = {};
    searchParameter.PagingParameter.PageIndex = Constants.DefaultPageIndex;
    searchParameter.PagingParameter.PageSize = Constants.DefaultPageSize;
    searchParameter.SortParameter = {};
    searchParameter.SortParameter.SortColumn = Constants.Name;
    searchParameter.SortParameter.SortOrder = Constants.Ascending;
    searchParameter.SearchMode = Constants.Exact;
    searchParameter.IsRequiredRolePrivileges = true
    if (this.RoleIdentifier != null) {
      searchParameter.Identifier = this.RoleIdentifier;
    }
    if (this.roleStatus != null) {
      searchParameter.IsActive = this.roleStatus;
    }

    var response = await roleService.getRoles(searchParameter);
    this.roleView = response.roleList;
    let rolePrivileges = [];
    for (let i = 0; i < this.roleView.length; i++) {
      this.Identifier = this.roleView[i].Identifier;
      this.Name = this.roleView[i].Name;
      this.roleRecord = this.roleView[i];
      this.RoleName = this.roleView[i].Name;
      this.RoleDesc = this.roleView[i].Description;
      rolePrivileges = this.roleView[i].RolePrivileges;
    }

    if (rolePrivileges != null && rolePrivileges.length != 0) {
      this.isRolePrivilegeIsAssigned = true;
      for (let i = 0; i < rolePrivileges.length; i++) {
        if (!this.rolePrivilegeExists(rolePrivileges[i].EntityName)) {
          let obj: any = {};
          obj.EntityName = rolePrivileges[i].EntityName;
          obj.Operation = [];
          let rlePrivilegeOperations = rolePrivileges[i].RolePrivilegeOperations;
          for (let x = 0; x < rlePrivilegeOperations.length; x++) {
            let opObj: any = {};
            opObj.IsEnabled = rlePrivilegeOperations[x].IsEnabled;
            opObj.Name = rlePrivilegeOperations[x].Operation;
            obj.Operation.push(opObj);
          }
          this.rolePrivileges.push(obj);
        }
        else {
          for (let j = 0; j < this.rolePrivileges.length; j++) {
            if (rolePrivileges[i].EntityName == this.rolePrivileges[j].EntityName) {
              let opObj: any = {};
              opObj.IsEnabled = true;
              opObj.Name = rolePrivileges[i].Operation;
              this.rolePrivileges[j].Operation.push(opObj);
            }
          }
        }
      }
    } else {
      this.isRolePrivilegeIsAssigned = false;
    }
  }

  rolePrivilegeExists(entityName) {
    return this.rolePrivileges.some(function (el) {
      return (el.EntityName === entityName);
    });
  }

  // method written to navigate to list page
  navigateToRoleList() {
    const router = this.injector.get(Router);
    router.navigate(['roles']);
  }

  //this method helps to navigate edit
  navigateToRoleEdit() {
    let queryParams = {
      Routeparams: {
        passingparams: {
          "RoleIdentifier": this.RoleIdentifier,
        },
        filteredparams: {
          //passing data using json stringify.
          "RoleName": this.RoleName,
        }
      }
    }
    localStorage.setItem("roleparams", JSON.stringify(queryParams))
    const router = this.injector.get(Router);
    router.navigate(['roles', 'Edit']);
  }

  //function written to delete role
  deleteRole() {
    let message = 'Are you sure, you want to delete this record?'
    this._messageDialogService.openConfirmationDialogBox('Confirm', message, Constants.msgBoxWarning).subscribe(async (isConfirmed) => {
      if (isConfirmed) {
        let roleData = [{
          "Identifier": this.RoleIdentifier,
        }];
        let isDependencyPresent = await this.roleService.checkDependency(roleData);
        if (isDependencyPresent) {
          let msg = 'Dependency present..!!';
          this._messageDialogService.openDialogBox('Error', msg, Constants.msgBoxError);
        }
        else {
          let isDeleted = await this.roleService.deleteRole(roleData);
          if (isDeleted) {
            let messageString = Constants.recordDeletedMessage;
            this._messageDialogService.openDialogBox('Success', messageString, Constants.msgBoxSuccess);
            this.navigateToRoleList();
          }
        }
      }
    });
  }
  async getRoleUser() {
    this.isCollapsedUsers = !this.isCollapsedUsers
    if (this.IsUserDetailsGet == false) {
      let userService = this.injector.get(UserService);
      let searchParameter: any = {};
      searchParameter.IsRolePrivilegesRequired = true;
      searchParameter.RoleIdentifier = this.RoleIdentifier;
      searchParameter.PagingParameter = {};
      searchParameter.PagingParameter.PageIndex = Constants.DefaultPageIndex;
      searchParameter.PagingParameter.PageSize = Constants.DefaultPageSize;
      searchParameter.SortParameter = {};
      searchParameter.SortParameter.SortColumn = "Id";
      searchParameter.SortParameter.SortOrder = Constants.Ascending;
      searchParameter.SearchMode = Constants.Contains;
      searchParameter.ActivationStatus = true;
      var response = await userService.getUser(searchParameter);
      this.usersList = response.usersList;
      this.IsUserDetailsGet = true;
    }


  }
}
