import { Component, OnInit, Injector, ViewChild } from '@angular/core';
import { Location } from '@angular/common';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';
import { Constants, ErrorMessageConstants } from 'src/app/shared/constants/constants';
import { MessageDialogService } from 'src/app/shared/services/mesage-dialog.service';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { MultiTenantUserAccessMapService } from './multi-tenant-user-access-map.service';
import { TenantService } from '../tenants/tenant.service';
import { UserService } from '../users/user.service';
import { RoleService } from '../roles/role.service';

@Component({
  selector: 'app-multi-tenant-user-access-map',
  templateUrl: './multi-tenant-user-access-map.component.html'
})
export class MultiTenantUserAccessMapComponent implements OnInit {

  public multiTenantUserAccessMapFormGroup: FormGroup;
  public multiTenantUserMapingList: any[] = [];
  public sortedMultiTenantUserMapingList: any[] = [];
  public lstPrimaryTenant: any[] = [{ 'TenantCode': '0', 'TenantName': 'Select Tenant' }];
  public tenantusers: any[] = [{ 'Identifier': '0', 'FirstName': 'Select ', 'LastName': 'User' }];
  public lstOtherTenants: any[] = [{ 'TenantCode': '0', 'TenantName': 'Select Tenant' }];
  public roles: any[] = [{ 'Identifier': '0', 'Name': 'Select Role' }];

  public tenantUserMappingFormErrorObject: any = {
    showPrimaryTenantError: false,
    showTenantUserError: false,
    showOtherTenantError: false,
    showUserRoleError: false,
  };

  public isLoaderActive: boolean = false;
  public isRecordFound: boolean = false;
  public pageNo = 0;
  public pageSize = 5;
  public currentPage = 0;
  public totalSize = 0;
  public array: any;
  public updateOperationMode: boolean = false;
  public multiTenantUserRoleAccessId: number = 0;

  selectedPrimaryTenantCode: string = '';
  selectedTenantUserId: number = 0;
  selectedOtherTenantCode: string = '';
  selectedRoleId: number = 0;

  displayedColumns: string[] = ['username', 'tenantname', 'role', 'status', 'actions'];
  dataSource = new MatTableDataSource<any>();
  public userClaimsRolePrivilegeOperations: any[] = [];

  public totalRecordCount = 0;
  public sortOrder = Constants.Descending;
  public sortColumn = 'LastUpdatedDate';

  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;

  constructor(private formbuilder: FormBuilder,
    private injector: Injector,
    private _messageDialogService: MessageDialogService) {
    }

  //getters of muli-tenant user access mapping Form group
  get primaryTenantCode() {
    return this.multiTenantUserAccessMapFormGroup.get('primaryTenantCode');
  }
  get tenantUserId() {
    return this.multiTenantUserAccessMapFormGroup.get('tenantUserId');
  }
  get otherTenantCode() {
    return this.multiTenantUserAccessMapFormGroup.get('otherTenantCode');
  }
  get roleId() {
    return this.multiTenantUserAccessMapFormGroup.get('roleId');
  }

  //function to validate all fields
  validateAllFormFields(formGroup: FormGroup) {
    Object.keys(formGroup.controls).forEach(field => {
        const control = formGroup.get(field);
        if (control instanceof FormControl) {
            control.markAsTouched({ onlySelf: true });
        } else if (control instanceof FormGroup) {
            this.validateAllFormFields(control);
        }
    });
  }

  public onPrimaryTenantSelected(event) {
    
    this.lstOtherTenants = [{ 'TenantCode': '0', 'TenantName': 'Select Tenant' }];
    this.tenantusers = [{ 'Identifier': '0', 'FirstName': 'Select ', 'LastName': 'User' }];
    this.roles = [{ 'Identifier': '0', 'Name': 'Select Role' }];
    this.selectedOtherTenantCode = '';
    this.selectedTenantUserId = 0;
    this.selectedRoleId = 0;
    this.multiTenantUserAccessMapFormGroup.patchValue({ tenantUserId: 0, otherTenantCode: 0, roleId: 0 });
    
    const val = event.target.value;
    if (val == "0") {
      this.tenantUserMappingFormErrorObject.showPrimaryTenantError = true;
      this.selectedPrimaryTenantCode = '';
    }
    else {
      this.tenantUserMappingFormErrorObject.showPrimaryTenantError = false;
      this.selectedPrimaryTenantCode = val;
      for(let i=0; i<this.lstPrimaryTenant.length; i++) {
        if(this.lstPrimaryTenant[i].TenantCode != '0' && this.lstPrimaryTenant[i].TenantCode != this.selectedPrimaryTenantCode) {
          let rec = Object.assign({}, this.lstPrimaryTenant[i]);
          this.lstOtherTenants.push(rec);
        }
      }
      this.getTenantUsers(null);
    }
  }

  public onOtherTenantSelected(event) {
    
    this.roles = [{ 'Identifier': '0', 'Name': 'Select Role' }];
    this.multiTenantUserAccessMapFormGroup.patchValue({ roleId: 0 });
    this.selectedRoleId = 0;

    const val = event.target.value;
    if (val == "0") {
      this.tenantUserMappingFormErrorObject.showOtherTenantError = true;
      this.selectedOtherTenantCode = '';
    }
    else {
      this.tenantUserMappingFormErrorObject.showOtherTenantError = false;
      this.selectedOtherTenantCode = val;
      this.getUserRoles(null);
    }
  }

  public onTenantUserSelected(event) {
    const val = event.target.value;
    if (val == "0") {
      this.tenantUserMappingFormErrorObject.showTenantUserError = true;
      this.selectedTenantUserId = 0;
    }
    else {
      this.tenantUserMappingFormErrorObject.showTenantUserError = false;
      this.selectedTenantUserId = val;
    }
  }

  public onUserRoleSelected(event) {
    const val = event.target.value;
    if (val == "0") {
      this.tenantUserMappingFormErrorObject.showUserRoleError = true;
      this.selectedRoleId = 0;
    }
    else {
      this.tenantUserMappingFormErrorObject.showUserRoleError = false;
      this.selectedRoleId = val;
    }
  }

  ngOnInit() {

    //Validations for Page Form.
    this.multiTenantUserAccessMapFormGroup = this.formbuilder.group({
      primaryTenantCode: [0, Validators.compose([Validators.required])],
      tenantUserId: [0, Validators.compose([Validators.required])],
      otherTenantCode: [0, Validators.compose([Validators.required])],
      roleId: [0, Validators.compose([Validators.required])],
    });

    var userClaimsDetail = JSON.parse(localStorage.getItem('userClaims'));
    if (userClaimsDetail) {
      this.userClaimsRolePrivilegeOperations = userClaimsDetail.Privileges;
    }
    else {
      this.userClaimsRolePrivilegeOperations = [];
    }

    this.getTenants(null);
    this.getTenantUserMappingData(null);
  }

  saveBtnValidation(): boolean {
    if(this.selectedPrimaryTenantCode == ''){
      return true;
    }
    if(this.selectedTenantUserId == 0) {
      return true;
    }
    if(this.selectedOtherTenantCode == '') {
      return true;
    }
    if(this.selectedRoleId == 0) {
      return true;
    }
    return false;
  }

  async OnSaveBtnClicked() {
    let tenantUserRoleAccess: any = {};
    tenantUserRoleAccess.AssociatedTenantCode = this.selectedPrimaryTenantCode;
    tenantUserRoleAccess.UserId = this.selectedTenantUserId;
    tenantUserRoleAccess.OtherTenantCode = this.selectedOtherTenantCode;
    tenantUserRoleAccess.RoleId = this.selectedRoleId;
    if(this.updateOperationMode) {
      tenantUserRoleAccess.Identifier = this.multiTenantUserRoleAccessId;
    }
    let array = [];
    array.push(tenantUserRoleAccess);
    let multiTenantUserRoleAccessService = this.injector.get(MultiTenantUserAccessMapService);
    let isRecordSaved = await multiTenantUserRoleAccessService.saveMultiTenantUserRoleAccess(array, this.updateOperationMode);
    if (isRecordSaved) {
      let message = Constants.recordAddedMessage;
      if (this.updateOperationMode) {
        message = Constants.recordUpdatedMessage;
      }
      this._messageDialogService.openDialogBox('Success', message, Constants.msgBoxSuccess);
      this.ResetForm();
      this.getTenantUserMappingData(null);
    }else {
      let message = ErrorMessageConstants.getSomethingWentWrongErrorMessage;
      this._messageDialogService.openDialogBox('Error', message, Constants.msgBoxSuccess);
    }
  }

  ResetForm() {
    this.multiTenantUserAccessMapFormGroup.patchValue({
      primaryTenantCode: 0,
      tenantUserId: 0,
      otherTenantCode: 0,
      roleId: 0,
    });
    this.updateOperationMode = false;
    this.multiTenantUserAccessMapFormGroup.get('primaryTenantCode').enable();
    this.multiTenantUserAccessMapFormGroup.get('tenantUserId').enable();
    this.tenantUserMappingFormErrorObject.showPrimaryTenantError = false;
    this.tenantUserMappingFormErrorObject.showTenantUserError = false;
    this.tenantUserMappingFormErrorObject.showOtherTenantError = false;
    this.tenantUserMappingFormErrorObject.showUserRoleError = false;
    this.validateAllFormFields(this.multiTenantUserAccessMapFormGroup);
  }

  public handlePage(e: any) {
    this.currentPage = e.pageIndex;
    this.pageSize = e.pageSize;
    this.getTenantUserMappingData(null);
  }

  sortData(sort: MatSort) {
    const data = this.multiTenantUserMapingList.slice();
    if (!sort.active || sort.direction === '') {
      this.sortedMultiTenantUserMapingList = data;
      return;
    }

    if (sort.direction == 'asc') {
      this.sortOrder = Constants.Ascending;
    }else {
      this.sortOrder = Constants.Descending;
    }

    switch (sort.active) {
      case 'username': this.sortColumn = "UserName"; break;
      case 'tenantname': this.sortColumn = "OtherTenantName"; break;
      case 'role': this.sortColumn = "RoleName"; break;
      case 'status': this.sortColumn = "IsActive"; break;
      default: this.sortColumn = "LastUpdatedDate"; break;
    }

    let searchParameter: any = {};
    //searchParameter.IsActive = true;
    searchParameter.PagingParameter = {};
    searchParameter.PagingParameter.PageIndex = this.currentPage + 1;
    searchParameter.PagingParameter.PageSize = this.pageSize;
    searchParameter.SortParameter = {};
    searchParameter.SortParameter.SortColumn = this.sortColumn;
    searchParameter.SortParameter.SortOrder = this.sortOrder;
    searchParameter.SearchMode = Constants.Contains;
    this.getTenantUserMappingData(searchParameter);
  }

  async getTenantUserMappingData(searchParameter) {
    let multiTenantUserRoleAccessService = this.injector.get(MultiTenantUserAccessMapService);
    if (searchParameter == null) {
      searchParameter = {};
      //searchParameter.IsActive = true;
      searchParameter.PagingParameter = {};
      searchParameter.PagingParameter.PageIndex = this.currentPage + 1;
      searchParameter.PagingParameter.PageSize = this.pageSize;
      searchParameter.SortParameter = {};
      searchParameter.SortParameter.SortColumn = this.sortColumn;
      searchParameter.SortParameter.SortOrder = this.sortOrder;
      searchParameter.SearchMode = Constants.Contains;
    }

    var response = await multiTenantUserRoleAccessService.getMultiTenantUserRoleMappingList(searchParameter);
    this.multiTenantUserMapingList = response.List;
    this.totalRecordCount = response.RecordCount;
    this.dataSource = new MatTableDataSource<any>(this.multiTenantUserMapingList);
    this.dataSource.sort = this.sort;
    this.array = this.multiTenantUserMapingList;
    this.totalSize = this.totalRecordCount;
  }

  //function written to get tenant list
  async getTenants(searchParameter) {
    let tenantService = this.injector.get(TenantService);
    if (searchParameter == null) {
      searchParameter = {};
      searchParameter.IsActive = true;
      searchParameter.PagingParameter = {};
      searchParameter.PagingParameter.PageIndex = Constants.DefaultPageIndex;
      searchParameter.PagingParameter.PageSize = Constants.DefaultPageSize;
      searchParameter.SortParameter = {};
      searchParameter.SortParameter.SortColumn = "Id";
      searchParameter.SortParameter.SortOrder = Constants.Descending;
      searchParameter.SearchMode = Constants.Contains;
    }
    searchParameter.IsPrimaryTenant = false;
    searchParameter.IsCountryRequired = false;
    var response = await tenantService.getTenant(searchParameter);
    this.lstPrimaryTenant.push(...response.List);
    // for(let i=0; i< response.List.length; i++) {
    //   let rec = Object.assign({}, response.List[i]);
    //   this.lstOtherTenants.push(rec);
    // }
  }

  //function written to get other tenant list
  async getOtherTenants(searchParameter) {
    let tenantService = this.injector.get(TenantService);
    if (searchParameter == null) {
      searchParameter = {};
      searchParameter.IsActive = true;
      searchParameter.PagingParameter = {};
      searchParameter.PagingParameter.PageIndex = Constants.DefaultPageIndex;
      searchParameter.PagingParameter.PageSize = Constants.DefaultPageSize;
      searchParameter.SortParameter = {};
      searchParameter.SortParameter.SortColumn = "Id";
      searchParameter.SortParameter.SortOrder = Constants.Descending;
      searchParameter.SearchMode = Constants.Contains;
    }
    searchParameter.IsPrimaryTenant = false;
    searchParameter.IsCountryRequired = false;
    var response = await tenantService.getTenant(searchParameter);
    this.lstOtherTenants.push(...response.List);
    let _lst = this.lstOtherTenants.filter(x => x.TenantCode == this.selectedPrimaryTenantCode);
    if(_lst.length > 0) {
      const index: number = this.lstOtherTenants.indexOf(_lst[0]);
      this.lstOtherTenants.splice(index, 1);
    }
  }

  async getTenantUsers(searchParameter) {
    let userService = this.injector.get(UserService);
    if (searchParameter == null) {
      searchParameter = {};
      searchParameter.PagingParameter = {};
      searchParameter.PagingParameter.PageIndex = Constants.DefaultPageIndex;
      searchParameter.PagingParameter.PageSize = Constants.DefaultPageSize;
      searchParameter.SortParameter = {};
      searchParameter.SortParameter.SortColumn = Constants.UserName;
      searchParameter.SortParameter.SortOrder = Constants.Ascending;
      searchParameter.SearchMode = Constants.Contains;
      searchParameter.ActivationStatus = true;
    }
    var response = await userService.getUser(searchParameter);
    this.tenantusers.push(...response.usersList);
  }

  async getUserRoles(searchParameter) {
    let roleService = this.injector.get(RoleService);
    if (searchParameter == null) {
      searchParameter = {};
      searchParameter.PagingParameter = {};
      searchParameter.PagingParameter.PageIndex = Constants.DefaultPageIndex;
      searchParameter.PagingParameter.PageSize = Constants.DefaultPageSize;
      searchParameter.SortParameter = {};
      searchParameter.SortParameter.SortColumn = Constants.Name;
      searchParameter.SortParameter.SortOrder = Constants.Ascending;
      searchParameter.SearchMode = Constants.Contains;
      searchParameter.IsActive = true;
    }
    var response = await roleService.getRoles(searchParameter);
    this.roles.push(...response.roleList);
  }

  async updateTenantUserMapping(element: any) {

    this.updateOperationMode = true;
    this.multiTenantUserAccessMapFormGroup.patchValue({
      primaryTenantCode: element.AssociatedTenantCode,
      tenantUserId: element.UserId,
      otherTenantCode: element.OtherTenantCode,
      roleId: element.RoleId,
    });

    this.selectedPrimaryTenantCode = element.AssociatedTenantCode;
    this.selectedTenantUserId = element.UserId;
    this.selectedOtherTenantCode = element.OtherTenantCode;
    this.selectedRoleId = element.RoleId;
    this.multiTenantUserRoleAccessId = element.Identifier;
    this.multiTenantUserAccessMapFormGroup.get('primaryTenantCode').disable();
    this.multiTenantUserAccessMapFormGroup.get('tenantUserId').disable();
    
    this.lstOtherTenants = [{ 'TenantCode': '0', 'TenantName': 'Select Tenant' }];
    this.tenantusers = [{ 'Identifier': '0', 'FirstName': 'Select ', 'LastName': 'User' }];
    this.roles = [{ 'Identifier': '0', 'Name': 'Select Role' }];

    await this.getTenantUsers(null);
    this.getUserRoles(null);

    for(let i=0; i<this.lstPrimaryTenant.length; i++) {
      if(this.lstPrimaryTenant[i].TenantCode != '0' && this.lstPrimaryTenant[i].TenantCode != this.selectedPrimaryTenantCode) {
        let rec = Object.assign({}, this.lstPrimaryTenant[i]);
        this.lstOtherTenants.push(rec);
      }
    }
  }

  deactiveTenantUserMapping(element: any) {
    let message = "Are you sure, you want to deactivate this record?";
    this._messageDialogService.openConfirmationDialogBox('Confirm', message, Constants.msgBoxWarning).subscribe(async (isConfirmed) => {
      if (isConfirmed) {
        let data = [{
          "Identifier": element.Identifier,
        }];
        let multiTenantUserRoleAccessService = this.injector.get(MultiTenantUserAccessMapService);
        let resultFlag = await multiTenantUserRoleAccessService.deactivateMultiTenantUserRoleAccess(data);
        if (resultFlag) {
          let messageString = Constants.recordDeactivatedMessage;
          this._messageDialogService.openDialogBox('Success', messageString, Constants.msgBoxSuccess);
          this.getTenantUserMappingData(null);
        }
      }
    });
  }

  activeTenantUserMapping(element: any) {
    let message = "Are you sure, you want to activate this record?";
    this._messageDialogService.openConfirmationDialogBox('Confirm', message, Constants.msgBoxWarning).subscribe(async (isConfirmed) => {
      if (isConfirmed) {
        let data = [{
          "Identifier": element.Identifier,
        }];
        let multiTenantUserRoleAccessService = this.injector.get(MultiTenantUserAccessMapService);
        let resultFlag = await multiTenantUserRoleAccessService.activateMultiTenantUserRoleAccess(data);
        if (resultFlag) {
          let messageString = Constants.recordActivatedMessage;
          this._messageDialogService.openDialogBox('Success', messageString, Constants.msgBoxSuccess);
          this.getTenantUserMappingData(null);
        }
      }
    });
  }

  deleteTenantUserMapping(element: any) {
    let message = "Are you sure, you want to delete this record?";
    this._messageDialogService.openConfirmationDialogBox('Confirm', message, Constants.msgBoxWarning).subscribe(async (isConfirmed) => {
      if (isConfirmed) {
        let data = [{
          "Identifier": element.Identifier,
        }];
        let multiTenantUserRoleAccessService = this.injector.get(MultiTenantUserAccessMapService);
        let resultFlag = await multiTenantUserRoleAccessService.deleteMultiTenantUserRoleAccess(data);
        if (resultFlag) {
          let messageString = Constants.recordDeactivatedMessage;
          this._messageDialogService.openDialogBox('Success', messageString, Constants.msgBoxSuccess);
          this.getTenantUserMappingData(null);
        }
      }
    });
  }

}
