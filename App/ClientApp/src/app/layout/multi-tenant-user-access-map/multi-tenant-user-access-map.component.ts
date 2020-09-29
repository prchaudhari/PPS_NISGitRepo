import { Component, OnInit, Injector, ViewChild } from '@angular/core';
import { Location } from '@angular/common';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';
import { Constants, ErrorMessageConstants } from 'src/app/shared/constants/constants';
import { ActivatedRoute, Router, NavigationEnd } from '@angular/router';
import { NgxUiLoaderService } from 'ngx-ui-loader';
import { MessageDialogService } from 'src/app/shared/services/mesage-dialog.service';
import { DialogService } from '@tomblue/ng2-bootstrap-modal';
import { LocalStorageService } from 'src/app/shared/services/local-storage.service';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';

@Component({
  selector: 'app-multi-tenant-user-access-map',
  templateUrl: './multi-tenant-user-access-map.component.html',
 // styleUrls: ['./multi-tenant-user-access-map.component.scss']
})
export class MultiTenantUserAccessMapComponent implements OnInit {

  public multiTenantUserAccessMapFormGroup: FormGroup;
  public muliTenantUserMapingList: any[] = [];
  public sortedmuliTenantUserMapingList: any[] = [];
  public lstPrimaryTenant: any[] = [{ 'Identifier': '0', 'TenantName': 'Select Tenant' }];
  public tenantusers: any[] = [{ 'Identifier': '0', 'UserName': 'Select User' }];
  public lstOtherTenants: any[] = [{ 'Identifier': '0', 'TenantName': 'Select Tenant' }];
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

  displayedColumns: string[] = ['username', 'tenantname', 'role', 'actions'];
  dataSource = new MatTableDataSource<any>();
  public userClaimsRolePrivilegeOperations: any[] = [];

  public totalRecordCount = 0;
  public sortOrder = Constants.Descending;
  public sortColumn = 'LastUpdatedDate';

  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;

  constructor(private _location: Location,
    private formbuilder: FormBuilder,
    private injector: Injector,
    private _dialogService: DialogService,
    private uiLoader: NgxUiLoaderService,
    private _messageDialogService: MessageDialogService,
    private router: Router,
    private localstorageservice: LocalStorageService) {

    }

  //getters of muli-tenant user access mapping Form group
  get primaryTenantId() {
    return this.multiTenantUserAccessMapFormGroup.get('primaryTenantId');
  }
  get tenantUserId() {
    return this.multiTenantUserAccessMapFormGroup.get('tenantUserId');
  }
  get otherTenantId() {
    return this.multiTenantUserAccessMapFormGroup.get('otherTenantId');
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

  ngOnInit() {

    //Validations for Page Form.
    this.multiTenantUserAccessMapFormGroup = this.formbuilder.group({
      primaryTenantId: [0, Validators.compose([Validators.required])],
      tenantUserId: [0, Validators.compose([Validators.required])],
      otherTenantId: [0, Validators.compose([Validators.required])],
      roleId: [0, Validators.compose([Validators.required])],
    });

    var userClaimsDetail = JSON.parse(localStorage.getItem('userClaims'));
    if (userClaimsDetail) {
      this.userClaimsRolePrivilegeOperations = userClaimsDetail.Privileges;
    }
    else {
      this.userClaimsRolePrivilegeOperations = [];
    }

    this.getTenantUserMappingData(null);
  }

  saveBtnValidation(): boolean {
    return true;
  }

  OnSaveBtnClicked() {
    this.getTenantUserMappingData(null);
  }

  ResetForm() {
    this.multiTenantUserAccessMapFormGroup.patchValue({
      primaryTenantId: 0,
      tenantUserId: 0,
      otherTenantId: 0,
      roleId: 0,
    });
  }

  public handlePage(e: any) {
    this.currentPage = e.pageIndex;
    this.pageSize = e.pageSize;
    this.getTenantUserMappingData(null);
  }

  sortData(sort: MatSort) {
    const data = this.muliTenantUserMapingList.slice();
    if (!sort.active || sort.direction === '') {
      this.sortedmuliTenantUserMapingList = data;
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
      default: this.sortColumn = "LastUpdatedDate"; break;
    }

    let searchParameter: any = {};
    searchParameter.IsActive = true;
    searchParameter.PagingParameter = {};
    searchParameter.PagingParameter.PageIndex = this.currentPage + 1;
    searchParameter.PagingParameter.PageSize = this.pageSize;
    searchParameter.SortParameter = {};
    searchParameter.SortParameter.SortColumn = this.sortColumn;
    searchParameter.SortParameter.SortOrder = this.sortOrder;
    searchParameter.SearchMode = Constants.Contains;
    this.getTenantUserMappingData(searchParameter);
  }

  getTenantUserMappingData(searchParameter) {
    if (searchParameter == null) {
      searchParameter = {};
      searchParameter.IsActive = true;
      searchParameter.PagingParameter = {};
      searchParameter.PagingParameter.PageIndex = this.currentPage + 1;
      searchParameter.PagingParameter.PageSize = this.pageSize;
      searchParameter.SortParameter = {};
      searchParameter.SortParameter.SortColumn = this.sortColumn;
      searchParameter.SortParameter.SortOrder = this.sortOrder;
      searchParameter.SearchMode = Constants.Contains;
    }

    var response: any = {}; //await templateService.getTemplates(searchParameter);
    this.muliTenantUserMapingList = [];  //response.list;
    this.totalRecordCount = 0; //response.RecordCount;
    this.dataSource = new MatTableDataSource<any>(this.muliTenantUserMapingList);
    this.dataSource.sort = this.sort;
    this.array = this.muliTenantUserMapingList;
    this.totalSize = this.totalRecordCount;
  }

  updateTenantUserMapping(element: any) {

  }

  deactiveTenantUserMapping(element: any) {
    let message = "Are you sure, you want to deactivate this record?";
    this._messageDialogService.openConfirmationDialogBox('Confirm', message, Constants.msgBoxWarning).subscribe(async (isConfirmed) => {
      if (isConfirmed) {
        let data = [{
          "Identifier": element.Identifier,
        }];
        let resultFlag = true; //await this.templateService.clonePage(pageData);
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
        let resultFlag = true; //await this.templateService.clonePage(pageData);
        if (resultFlag) {
          let messageString = Constants.recordActivatedMessage;
          this._messageDialogService.openDialogBox('Success', messageString, Constants.msgBoxSuccess);
          this.getTenantUserMappingData(null);
        }
      }
    });
  }

}
