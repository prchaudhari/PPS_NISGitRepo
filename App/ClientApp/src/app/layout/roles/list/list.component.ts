import { Component, OnInit, Injector, ChangeDetectorRef, ViewChild, OnDestroy } from '@angular/core';
import { Role } from '../role';
import { RoleService } from '../role.service';
import { FormGroup, FormBuilder } from '@angular/forms';
import { NgxUiLoaderService } from 'ngx-ui-loader';
import { Router, NavigationEnd } from '@angular/router';
import { Constants } from 'src/app/shared/constants/constants';
import { MessageDialogService } from 'src/app/shared/services/mesage-dialog.service';
import { ConfigConstants } from 'src/app/shared/constants/configConstants';
import { LocalStorageService } from 'src/app/shared/services/local-storage.service';
import { ResourceService } from 'src/app/shared/services/resource.service';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';

export interface ListElement {
  name: string;
  description: string;
}

@Component({
  selector: 'app-list',
  templateUrl: './list.component.html',
  styleUrls: ['./list.component.scss']
})
export class ListComponent implements OnInit {

  //public variables
  public isFilter: boolean = false;
  public roleList: Role[] = [];
  public isLoaderActive: boolean = false;
  public isRecordFound: boolean = false;
  public pageNo = 0;
  public roleFilterForm: FormGroup;
  public params: any = {};
  public RoleIdentifier;
  public RoleName;
  public status;
  public roleListResources = {}
  public ResourceLoadingFailedMsg = Constants.ResourceLoadingFailedMsg;
  public Locale;
  public userClaimsRolePrivilegeOperations: any[] = [];
  public array: any;
  public displayedColumns: string[] = ['name', 'description', 'active', 'actions'];
  public dataSource: any;

  public pageSize = 5;
  public currentPage = 0;
  public totalSize = 0;
  public previousPageLabel: string;
  public isFilterDone = false;
  public filterRoleValue = '';
  public filterActiveRole = true;
  public totalRecordCount = 0;

  public RoleFilter: any = {
    Name: null,
    DeactivateRole: null,
  };

  public sortedRoleList: Role[] = [];
  public sortOrder = Constants.Ascending;
  public sortColumn = 'Name';
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: true }) sort: MatSort;

  public handlePage(e: any) {
    this.currentPage = e.pageIndex;
    this.pageSize = e.pageSize;
    //this.iterator();
    this.getRoleRecords(null);
  }

  private iterator() {
    const end = (this.currentPage + 1) * this.pageSize;
    const start = this.currentPage * this.pageSize;
    const part = this.array.slice(start, end);
    this.dataSource = part;
    this.dataSource.sort = this.sort;
  }

  //Getters for Role Forms
  get filterRoleName() {
    return this.roleFilterForm.get('filterRoleName');
  }

  //Getters for Role Forms
  get filterDeactivateRole() {
    return this.roleFilterForm.get('DeactivateRole');
  }

  constructor(
    private injector: Injector,
    private fb: FormBuilder,
    private uiLoader: NgxUiLoaderService,
    private _messageDialogService: MessageDialogService,
    private router: Router,
    private localstorageservice: LocalStorageService,
    private roleService: RoleService
  ) {
    this.sortedRoleList = this.roleList.slice();
  }

  sortData(sort: MatSort) {
    const data = this.roleList.slice();
    if (!sort.active || sort.direction === '') {
      this.sortedRoleList = data;
      return;
    }
   
    if (sort.direction == 'asc') {
      this.sortOrder = Constants.Ascending;
    }
    else {
      this.sortOrder = Constants.Descending;
    }
    if (sort.active == 'name') {
      this.sortColumn = 'Name';
    }
    else if (sort.active == 'description') {
      this.sortColumn = 'Description';
    }
    else if (sort.active == 'active') {
      this.sortColumn = 'IsActive';
    }
    let searchParameter: any = {};
    searchParameter.PagingParameter = {};
    searchParameter.PagingParameter.PageIndex = this.currentPage+1;
    searchParameter.PagingParameter.PageSize = this.pageSize;
    searchParameter.SortParameter = {};
    searchParameter.SortParameter.SortColumn = this.sortColumn;
    searchParameter.SortParameter.SortOrder = this.sortOrder;
    searchParameter.SearchMode = Constants.Contains;
    this.filterRoleValue = this.roleFilterForm.value.filterRoleName != null ? this.roleFilterForm.value.filterRoleName.trim() : "";
    this.filterActiveRole = this.roleFilterForm.value.DeactivateRole != null ? !this.roleFilterForm.value.DeactivateRole : true;
    searchParameter.Name = this.filterRoleValue;
    searchParameter.IsActive = this.filterActiveRole;
    this.getRoleRecords(searchParameter);
  }

  //method called on initialization
  ngOnInit() {
    this.getRoleRecords(null)
    this.roleFilterForm = this.fb.group({
      filterRoleName: [null],
      DeactivateRole: [null]
    });
    //this.backParamCheck();
    var userClaimsDetail = JSON.parse(localStorage.getItem('userClaims'));
    if (userClaimsDetail) {
      this.userClaimsRolePrivilegeOperations = userClaimsDetail.Privileges;
    }
    else {
      this.userClaimsRolePrivilegeOperations = [];
    }
  }

  //This method has been used for fetching role records
  async getRoleRecords(searchParameter) {
    let roleService = this.injector.get(RoleService);
    if (searchParameter == null) {
      searchParameter = {};
      searchParameter.PagingParameter = {};
      searchParameter.PagingParameter.PageIndex = this.currentPage + 1;
      searchParameter.PagingParameter.PageSize = this.pageSize;
      searchParameter.SortParameter = {};
      searchParameter.SortParameter.SortColumn = this.sortColumn;
      searchParameter.SortParameter.SortOrder = this.sortOrder;
      searchParameter.SearchMode = Constants.Contains;
      searchParameter.Name = this.filterRoleValue;
      searchParameter.IsActive = this.filterActiveRole;
    }
    searchParameter.IsCallFromListPage = true;
    searchParameter.IsActive = this.filterActiveRole;
    if (this.filterRoleValue != "") {
      searchParameter.Name = this.filterRoleValue;
    }
    var response = await roleService.getRoles(searchParameter);
    this.roleList = response.roleList;
    this.totalRecordCount = response.RecordCount;
    if (this.roleList.length == 0 && this.isFilterDone == true) {
      let message = "NO record found";
      this._messageDialogService.openDialogBox('Error', message, Constants.msgBoxError).subscribe(data => {
        if (data == true) {
          this.resetRoleFilterForm();
          this.getRoleRecords(null);
        }
      });
    }
    this.dataSource = new MatTableDataSource<Role>(this.roleList);
    this.dataSource.sort = this.sort;
    this.array = this.roleList;
    this.totalSize = this.totalRecordCount;
  }

  //This method has been used for fetching search records
  searchFilter(searchType) {
    this.isFilterDone = true;
    if (searchType == 'reset') {
      this.resetRoleFilterForm();
      this.getRoleRecords(null);
      this.isFilter = !this.isFilter;
    }
    else {
      let searchParameter: any = {};
      searchParameter.PagingParameter = {};
      searchParameter.PagingParameter.PageIndex = 1;
      searchParameter.PagingParameter.PageSize = this.pageSize;
      searchParameter.SortParameter = {};
      searchParameter.SortParameter.SortColumn = this.sortColumn;
      searchParameter.SortParameter.SortOrder = this.sortOrder;
      searchParameter.SearchMode = Constants.Contains;
      this.filterRoleValue = this.roleFilterForm.value.filterRoleName != null ? this.roleFilterForm.value.filterRoleName.trim() : "";
      this.filterActiveRole = this.roleFilterForm.value.DeactivateRole != null ? !this.roleFilterForm.value.DeactivateRole : true;
      searchParameter.Name = this.filterRoleValue;
      searchParameter.IsActive = this.filterActiveRole;
      this.currentPage = 0;
      this.getRoleRecords(searchParameter);
      this.isFilter = !this.isFilter;
    }
  }

  //this method helps to navigate to view
  navigateToRoleView(role: Role) {
    let queryParams = {
      Routeparams: {
        passingparams: {
          "RoleIdentifier": role.Identifier,
        },
        filteredparams: {
          //passing data using json stringify.
          "RoleName": this.roleFilterForm.value.filterRoleName != null ? this.roleFilterForm.value.filterRoleName : "",
          "IsActive": this.roleFilterForm.value.DeactivateRole != null ? !this.roleFilterForm.value.DeactivateRole : null
        }
      }
    }
    localStorage.setItem("roleparams", JSON.stringify(queryParams))
    const router = this.injector.get(Router);
    router.navigate(['roles', 'View']);
  }

  //this method helps to navigate to add
  navigateToRoleAdd() {
    const router = this.injector.get(Router);
    router.navigate(['roles', 'Add']);
  }

  //this method helps to navigate edit
  navigateToRoleEdit(role) {
    let queryParams = {
      Routeparams: {
        passingparams: {
          "RoleIdentifier": role.Identifier,
        },
        filteredparams: {
          //passing data using json stringify.
          "RoleName": this.roleFilterForm.value.filterRoleName != null ? this.roleFilterForm.value.filterRoleName : "",
        }
      }
    }
    localStorage.setItem("roleparams", JSON.stringify(queryParams))
    const router = this.injector.get(Router);
    router.navigate(['roles', 'Edit']);
  }

  //function written to delete role
  deleteRole(role: Role) {
    let message = 'Are you sure, you want to delete this record?';
    this._messageDialogService.openConfirmationDialogBox('Confirm', message, Constants.msgBoxWarning).subscribe(async (isConfirmed) => {
      if (isConfirmed) {
        let roleData = [{
          "Identifier": role.Identifier,
        }];
        let isDependencyPresent = await this.roleService.checkDependency(roleData);
        if (isDependencyPresent) {
          let msg = 'Dependency present ..!!';
          this._messageDialogService.openDialogBox('Error', msg, Constants.msgBoxError);
        }
        else {
          let isDeleted = await this.roleService.deleteRole(roleData);
          if (isDeleted) {
            let messageString = Constants.recordDeletedMessage;
            this._messageDialogService.openDialogBox('Success', messageString, Constants.msgBoxSuccess);
            this.getRoleRecords(null);
          }
        }
      }
    });
  }

  async DeactivateRole(role: Role) {
    let message = "Are you sure, you want to deactivate this record?";
    this._messageDialogService.openConfirmationDialogBox('Confirm', message, Constants.msgBoxWarning).subscribe(async (isConfirmed) => {
      if (isConfirmed) {
        let roleData = [{
          "Identifier": role.Identifier,
        }];

        let resultFlag = await this.roleService.deactivateRole(roleData);
        if (resultFlag) {
          let messageString = Constants.recordDeactivatedMessage;
          this._messageDialogService.openDialogBox('Success', messageString, Constants.msgBoxSuccess);
          this.resetRoleFilterForm();
          this.getRoleRecords(null);
        }
      }
    });
  }

  async ActivateRole(role: Role) {
    let roleData = [{
      "Identifier": role.Identifier,
    }];
    let resultFlag = await this.roleService.activateRole(roleData);
    if (resultFlag) {
      let messageString = Constants.recordActivatedMessage;
      this._messageDialogService.openDialogBox('Success', messageString, Constants.msgBoxSuccess);
      this.resetRoleFilterForm();
      this.getRoleRecords(null);
    }
  }

  //to get filter data as it is 
  backParamCheck() {
    if (localStorage.getItem('roleparams')) {
      this.params = JSON.parse(localStorage.getItem('roleparams'));
      this.RoleIdentifier = this.params.Routeparams.passingparams.RoleIdentifier
      this.RoleName = this.params.Routeparams.filteredparams.RoleName
      // this.status = this.params.Routeparams.filteredparams.Status
    }
    if (this.RoleName != null) {
      this.roleFilterForm.patchValue({
        filterRoleName: this.RoleName,
      })
      let searchParameter: any = {};
      searchParameter.PagingParameter = {};
      searchParameter.PagingParameter.PageIndex = Constants.DefaultPageIndex;
      searchParameter.PagingParameter.PageSize = Constants.DefaultPageSize;
      searchParameter.SortParameter = {};
      searchParameter.SortParameter.SortColumn = Constants.Name;
      searchParameter.SortParameter.SortOrder = Constants.Ascending;
      searchParameter.SearchMode = Constants.Contains;
      if (this.RoleName != "")
        searchParameter.Name = this.RoleName
      this.getRoleRecords(searchParameter);

    }
    else {
      let searchParameter: any = {};
      searchParameter.PagingParameter = {};
      searchParameter.PagingParameter.PageIndex = Constants.DefaultPageIndex;
      searchParameter.PagingParameter.PageSize = Constants.DefaultPageSize;
      searchParameter.SortParameter = {};
      searchParameter.SortParameter.SortColumn = Constants.Name;
      searchParameter.SortParameter.SortOrder = Constants.Ascending;
      searchParameter.SearchMode = Constants.Contains;
      this.getRoleRecords(searchParameter);
    }
  }

  activationEventCheck(event) {
    if (event.checked) {
      this.roleFilterForm.value.DeactivateRole = true;
    }
    else {
      this.roleFilterForm.value.DeactivateRole = false;
    }
  }

  resetRoleFilterForm() {
    this.roleFilterForm.patchValue({
      filterRoleName: null,
      DeactivateRole: null
    });
    this.currentPage = 0;
    this.filterRoleValue = '';
    this.filterActiveRole = true;
  }

}

function compareStr(a: string, b: string, isAsc: boolean) {
  return (a.toLowerCase() < b.toLowerCase() ? -1 : 1) * (isAsc ? 1 : -1);
}

function compareNumber(a: number, b: number, isAsc: boolean) {
  return (a < b ? -1 : 1) * (isAsc ? 1 : -1);
}
