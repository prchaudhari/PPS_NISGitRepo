import { Component, OnInit, Injector, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';
import { Constants, ErrorMessageConstants } from 'src/app/shared/constants/constants';
import { MessageDialogService } from 'src/app/shared/services/mesage-dialog.service';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { MultiTenantUserAccessMapService } from './multi-tenant-user-access-map.service';
import { LocalStorageService } from 'src/app/shared/services/local-storage.service';
import { NgxUiLoaderService } from 'ngx-ui-loader';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { ConfigConstants } from 'src/app/shared/constants/configConstants';
import { Router } from '@angular/router';

@Component({
  selector: 'app-multi-tenant-user-access-map',
  templateUrl: './multi-tenant-user-access-map.component.html'
})
export class MultiTenantUserAccessMapComponent implements OnInit {

  public multiTenantUserAccessMapFormGroup: FormGroup;
  public TenantUserRoleMappingFilterForm: FormGroup;
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
  public isFilterDone = false;
  public array: any;
  public updateOperationMode: boolean = false;
  public multiTenantUserRoleAccessId: number = 0;
  public baseURL: string = ConfigConstants.BaseURL;

  selectedPrimaryTenantCode: string = '';
  selectedTenantUserId: number = 0;
  selectedOtherTenantCode: string = '';
  selectedRoleId: number = 0;

  displayedColumns: string[] = ['parentname', 'username', 'tenantname', 'role', 'status', 'actions'];
  dataSource = new MatTableDataSource<any>();
  public userClaimsRolePrivilegeOperations: any[] = [];

  public totalRecordCount = 0;
  public sortOrder = Constants.Descending;
  public sortColumn = 'LastUpdatedDate';
  public popupContainer: boolean;
  public isFilter: boolean = false;

  public filteredparenttenantname = '';
  public filteredusername = '';
  public filteredtargettenantname = '';
  public filteredrole = '';

  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;

  constructor(private formbuilder: FormBuilder,
    private injector: Injector,
    private _messageDialogService: MessageDialogService,
    private _http: HttpClient,
    private localstorageservice: LocalStorageService,
    private route: Router,
    private uiLoader: NgxUiLoaderService) {
    }

  closeFilter() {
    this.isFilter = !this.isFilter;
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

  get filterParentTenantName() {
    return this.TenantUserRoleMappingFilterForm.get('filterParentTenantName');
  }

  get filterUserName() {
    return this.TenantUserRoleMappingFilterForm.get('filterUserName');
  }

  get filterTargetTenantName() {
    return this.TenantUserRoleMappingFilterForm.get('filterTargetTenantName');
  }

  get filterRole() {
    return this.TenantUserRoleMappingFilterForm.get('filterRole');
  }

  //this method helps to navigate to add
  navigateToAddTenantUserRoleAccessMapping() {
    this.multiTenantUserRoleAccessId = 0;
    this.popupContainer = true;
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
        if(this.lstPrimaryTenant[i].TenantCode != '0' && this.lstPrimaryTenant[i].TenantCode != this.selectedPrimaryTenantCode
        && this.lstPrimaryTenant[i].TenantType != "Group") {
          let rec = Object.assign({}, this.lstPrimaryTenant[i]);
          this.lstOtherTenants.push(rec);
        }
      }
      this.getTenantUsers();
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
      this.getUserRoles();
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

    var userClaimsDetail = JSON.parse(localStorage.getItem('userClaims'));
    if (userClaimsDetail) {
      if(userClaimsDetail.IsTenantGroupManager == null || userClaimsDetail.IsTenantGroupManager.toLocaleLowerCase() != 'true') {
        this.localstorageservice.removeLocalStorageData();
        this.route.navigate(['login']);
      }
      this.userClaimsRolePrivilegeOperations = userClaimsDetail.Privileges;
    }
    else {
      this.localstorageservice.removeLocalStorageData();
      this.route.navigate(['login']);
    }

    //Validations for Page Form.
    this.multiTenantUserAccessMapFormGroup = this.formbuilder.group({
      primaryTenantCode: [0, Validators.compose([Validators.required])],
      tenantUserId: [0, Validators.compose([Validators.required])],
      otherTenantCode: [0, Validators.compose([Validators.required])],
      roleId: [0, Validators.compose([Validators.required])],
    });

    this.TenantUserRoleMappingFilterForm = this.formbuilder.group({
      filterParentTenantName: [null],
      filterUserName: [null],
      filterTargetTenantName: [null],
      filterRole: [null]
    });

    this.getTenants();
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
    this.popupContainer = false;
  }

  ResetTenantUserRoleMappingFilterForm() {
    this.TenantUserRoleMappingFilterForm.patchValue({
      filterParentTenantName: null,
      filterUserName: null,
      filterTargetTenantName: null,
      filterRole: null,
    });
    this.currentPage = 0;
    this.filteredparenttenantname = '';
    this.filteredusername = '';
    this.filteredtargettenantname = '';
    this.filteredrole = '';
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
      case 'parentname': this.sortColumn = "AssociatedTenantName"; break;
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
    if(this.filteredusername != '') {
      searchParameter.UserName = this.filteredusername.trim();
    }
    if(this.filteredtargettenantname != '') {
      searchParameter.OtherTenantName = this.filteredtargettenantname.trim();
    }
    if(this.filteredparenttenantname != '') {
      searchParameter.AssociatedTenantName = this.filteredparenttenantname.trim();
    }
    if(this.filteredrole != '') {
      searchParameter.RoleName = this.filteredrole.trim();
    }
    var response = await multiTenantUserRoleAccessService.getMultiTenantUserRoleMappingList(searchParameter);
    this.multiTenantUserMapingList = response.List;
    this.totalRecordCount = response.RecordCount;
    this.dataSource = new MatTableDataSource<any>(this.multiTenantUserMapingList);
    this.dataSource.sort = this.sort;
    this.array = this.multiTenantUserMapingList;
    this.totalSize = this.totalRecordCount;
  }

  //This method has been used for fetching search records
  searchFilter(searchType) {
    this.isFilterDone = true;
    if (searchType == 'reset') {
      this.ResetTenantUserRoleMappingFilterForm();
      this.getTenantUserMappingData(null);
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
      if (this.TenantUserRoleMappingFilterForm.value.filterParentTenantName != null && this.TenantUserRoleMappingFilterForm.value.filterParentTenantName != '') {
        this.filteredparenttenantname = this.TenantUserRoleMappingFilterForm.value.filterParentTenantName.trim();
        searchParameter.AssociatedTenantName = this.TenantUserRoleMappingFilterForm.value.filterParentTenantName.trim();
      }
      if (this.TenantUserRoleMappingFilterForm.value.filterUserName != null && this.TenantUserRoleMappingFilterForm.value.filterUserName != '') {
        this.filteredusername = this.TenantUserRoleMappingFilterForm.value.filterUserName.trim();
        searchParameter.UserName = this.TenantUserRoleMappingFilterForm.value.filterUserName.trim();
      }
      if (this.TenantUserRoleMappingFilterForm.value.filterTargetTenantName != null && this.TenantUserRoleMappingFilterForm.value.filterTargetTenantName != '') {
        this.filteredtargettenantname = this.TenantUserRoleMappingFilterForm.value.filterTargetTenantName.trim();
        searchParameter.OtherTenantName = this.TenantUserRoleMappingFilterForm.value.filterTargetTenantName.trim();
      }
      if (this.TenantUserRoleMappingFilterForm.value.filterRole != null && this.TenantUserRoleMappingFilterForm.value.filterRole != '') {
        this.filteredrole = this.TenantUserRoleMappingFilterForm.value.filterRole;
        searchParameter.RoleName = this.TenantUserRoleMappingFilterForm.value.filterRole;
      }
      this.currentPage = 0;
      this.getTenantUserMappingData(searchParameter);
      this.isFilter = !this.isFilter;
    }
  }

  //function written to get tenant list
  async getTenants() {
    let multiTenantUserRoleAccessService = this.injector.get(MultiTenantUserAccessMapService);
    var response = await multiTenantUserRoleAccessService.GetParentAndChildtenants();
    this.lstPrimaryTenant = [{ 'TenantCode': '0', 'TenantName': 'Select Tenant' }];
    var _lst = response.List;
    for(let i=0; i<_lst.length; i++) {
      let rec = Object.assign({}, _lst[i]);
      this.lstPrimaryTenant.push(rec);
    }
  }

  async getTenantUsers() {
    this.uiLoader.start();
    this._http.get(this.baseURL + 'MultiTenantUserRoleAccess/GetUsersByTenantCode?tenantCode=' + this.selectedPrimaryTenantCode).pipe(map(response => response))
      .subscribe(
        data => {
          let records: any = data;
          records.forEach(obj => {
            this.tenantusers = [...this.tenantusers, obj];
          });
          this.uiLoader.stop();
        },
        error => {
          //$('.overlay').show();
      });
  }

  async getUserRoles() {
    this._http.get(this.baseURL + 'MultiTenantUserRoleAccess/GetRolesByTenantCode?tenantCode=' + this.selectedOtherTenantCode).pipe(map(response => response))
      .subscribe(
        data => {
          let records: any = data;
          records.forEach(obj => {
            if(obj.Name != 'Tenant Admin') {
              this.roles = [...this.roles, obj];
            }
          });
          this.uiLoader.stop();
        },
        error => {
          //$('.overlay').show();
      });
  }

  async updateTenantUserMapping(element: any) {
    this.popupContainer = true;
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

    this.getTenantUsers();
    this.getUserRoles();

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
