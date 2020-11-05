import { Component, OnInit, Injector, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';
import { NgxUiLoaderService } from 'ngx-ui-loader';
import { Router } from '@angular/router';
import { Constants } from 'src/app/shared/constants/constants';
import { ErrorMessageConstants } from 'src/app/shared/constants/constants';
import { MessageDialogService } from 'src/app/shared/services/mesage-dialog.service';
import { LocalStorageService } from 'src/app/shared/services/local-storage.service';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { TenantService } from '../tenant.service';
import { Tenant } from '../tenant';

export interface ListElement {
  name: string;
  domainName: string;
  country: string;
}

const List_Data: ListElement[] = [
  { name: 'Name 01', domainName: 'Domain 01', country: 'India' },
  { name: 'Name 01', domainName: 'Domain 01', country: 'India' },
  { name: 'Name 01', domainName: 'Domain 01', country: 'India' },
  { name: 'Name 01', domainName: 'Domain 01', country: 'India' },
  { name: 'Name 01', domainName: 'Domain 01', country: 'India' },
  { name: 'Name 01', domainName: 'Domain 01', country: 'India' },
];

@Component({
  selector: 'app-list',
  templateUrl: './list.component.html',
  styleUrls: ['./list.component.scss']
})
export class ListComponent implements OnInit {
  public isFilter: boolean = false;
  public pageSize = 5;
  public currentPage = 0;
  public totalSize = 0;
  public tenantList: Tenant[] = [];
  public sortedtenantList: Tenant[] = [];
  public TenantFilterForm: FormGroup;
  public isFilterDone = false;
  public array: any;
  public userClaimsRolePrivilegeOperations: any[] = [];
  public totalRecordCount = 0;
  public filterTenantNameValue = '';
  public filterTenantDomainName = '';
  public filterTenantStartDate = null;
  public filterTenantEndDate = null;
  public sortOrder = Constants.Descending;
  public sortColumn = 'Id';

  closeFilter() {
    this.isFilter = !this.isFilter;
  }
  displayedColumns: string[] = ['name', 'primarycontact', 'country', 'actions'];

  dataSource = new MatTableDataSource<any>();

  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;

  ngOnInit() {

    var userClaimsDetail = JSON.parse(localStorage.getItem('userClaims'));
    if (userClaimsDetail) {
      if (userClaimsDetail.IsTenantGroupManager == null || userClaimsDetail.IsTenantGroupManager.toLocaleLowerCase() != 'true') {
        this.localstorageservice.removeLocalStorageData();
        this.route.navigate(['login']);
      }
      this.userClaimsRolePrivilegeOperations = userClaimsDetail.Privileges;
    }
    else {
      this.localstorageservice.removeLocalStorageData();
      this.route.navigate(['login']);
    }

    this.getTenant(null);
    this.TenantFilterForm = this.fb.group({
      filterName: [null],
      filterDomainName: [null],
      filterCountry: [null],
    });
  }

  ngAfterViewInit() {
    this.dataSource.paginator = this.paginator;
  }

  constructor(private injector: Injector,
    private fb: FormBuilder,
    private uiLoader: NgxUiLoaderService,
    private _messageDialogService: MessageDialogService,
    private route: Router,
    private localstorageservice: LocalStorageService,
    private tenantService: TenantService) {
    this.sortedtenantList = this.tenantList.slice();
  }

  public handlePage(e: any) {
    this.currentPage = e.pageIndex;
    this.pageSize = e.pageSize;
    this.getTenant(null);
  }

  get filterName() {
    return this.TenantFilterForm.get('filterName');
  }

  get filterDomainName() {
    return this.TenantFilterForm.get('filterDomainName');
  }

  get filterCountry() {
    return this.TenantFilterForm.get('filterCountry');
  }

  resetFilterForm() {
    this.TenantFilterForm.patchValue({
      filterName: null,
      filterDomainName: null,
      filterCountry: null
    });

    this.currentPage = 0;
    this.filterTenantNameValue = '';
    this.filterTenantDomainName = '';
    this.filterTenantStartDate = null;
    this.filterTenantEndDate = null;
  }

  //This method has been used for fetching search records
  searchTenantRecordFilter(searchType) {
    this.isFilterDone = true;
    if (searchType == 'reset') {
      this.resetFilterForm();
      this.getTenant(null);
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

      if (this.TenantFilterForm.value.filterName != null && this.TenantFilterForm.value.filterName != '') {
        this.filterTenantNameValue = this.TenantFilterForm.value.filterName.trim();
        searchParameter.TenantName = this.TenantFilterForm.value.filterName.trim();
      }
      if (this.TenantFilterForm.value.filterDomainName != null && this.TenantFilterForm.value.filterDomainName != '') {
        this.filterTenantDomainName = this.TenantFilterForm.value.filterDomainName.trim();
        searchParameter.TenantDomainName = this.TenantFilterForm.value.filterDomainName.trim();
      }

      this.currentPage = 0;
      this.getTenant(searchParameter);
      this.isFilter = !this.isFilter;
    }
  }

  async getTenant(searchParameter) {
    let tenantService = this.injector.get(TenantService);
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
    if (this.filterTenantNameValue != null && this.filterTenantNameValue != '') {
      searchParameter.TenantName = this.filterTenantNameValue.trim();
    }
    if (this.filterTenantDomainName != null && this.filterTenantDomainName != '') {
      searchParameter.TenantDomainName = this.filterTenantDomainName.trim();
    }
    var currentUser = this.localstorageservice.GetCurrentUser();
    searchParameter.IsPrimaryTenant = false;
    searchParameter.ParentTenantCode = currentUser.TenantCode;
    searchParameter.IsCountryRequired = true;
    searchParameter.TenantType = "Tenant";
    var response = await tenantService.getTenant(searchParameter);
    this.tenantList = response.List;
    this.totalRecordCount = response.RecordCount;
    if (this.tenantList.length == 0 && this.isFilterDone == true) {
      let message = ErrorMessageConstants.getNoRecordFoundMessage;
      this._messageDialogService.openDialogBox('Error', message, Constants.msgBoxError).subscribe(data => {
        if (data == true) {
          this.resetFilterForm();
          this.getTenant(null);
        }
      });
    }
    this.dataSource = new MatTableDataSource<Tenant>(this.tenantList);
    this.dataSource.sort = this.sort;
    this.array = this.tenantList;
    this.totalSize = this.totalRecordCount;
    //this.iterator();
  }

  sortData(sort: MatSort) {
    const data = this.tenantList.slice();
    if (!sort.active || sort.direction === '') {
      this.sortedtenantList = data;
      return;
    }

    if (sort.direction == 'asc') {
      this.sortOrder = Constants.Ascending;
    } else {
      this.sortOrder = Constants.Descending;
    }
    //'name', 'domainName', 'country', 
    switch (sort.active) {
      case 'name': this.sortColumn = "TenantName"; break;
      case 'primarycontact': this.sortColumn = "PrimaryEmailAddress"; break;
      default: this.sortColumn = "Id"; break;
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
    this.getTenant(searchParameter);
  }

  //this method helps to navigate to view
  navigateToTenantView(tenant: Tenant) {
    let queryParams = {
      Routeparams: {
        passingparams: {
          "TenantTenantCode": tenant.TenantCode,
        },
        filteredparams: {
          //passing data using json stringify.
          "TenantName": this.TenantFilterForm.value.filterRoleName != null ? this.TenantFilterForm.value.filterRoleName : ""
        }
      }
    }
    localStorage.setItem("tenantparams", JSON.stringify(queryParams))
    const router = this.injector.get(Router);
    router.navigate(['tenants', 'View']);
  }

  //this method helps to navigate to add
  navigateToTenantAdd() {
    const router = this.injector.get(Router);
    router.navigate(['tenants', 'Add']);
  }

  //this method helps to navigate edit
  navigateToTenantEdit(tenant) {
    let queryParams = {
      Routeparams: {
        passingparams: {
          "TenantTenantCode": tenant.TenantCode,
        },
        filteredparams: {
          //passing data using json stringify.
          "TenantName": this.TenantFilterForm.value.filterRoleName != null ? this.TenantFilterForm.value.filterRoleName : ""
        }
      }
    }
    localStorage.setItem("tenantparams", JSON.stringify(queryParams))
    const router = this.injector.get(Router);
    router.navigate(['tenants', 'Edit']);
  }

  navigateToTenantHistory(tenant) {
    let queryParams = {
      Routeparams: {
        passingparams: {
          "TenantTenantCode": tenant.TenantCode,
        },
        filteredparams: {
          //passing data using json stringify.
          "TenantName": this.TenantFilterForm.value.filterRoleName != null ? this.TenantFilterForm.value.filterRoleName : ""
        }
      }
    }
    localStorage.setItem("tenantparams", JSON.stringify(queryParams))
    const router = this.injector.get(Router);
    router.navigate(['tenants', 'History']);
  }

  //function written to delete role
  deleteTenant(tenant: Tenant) {
    let message = 'Are you sure, you want to delete this record?';
    this._messageDialogService.openConfirmationDialogBox('Confirm', message, Constants.msgBoxWarning).subscribe(async (isConfirmed) => {
      if (isConfirmed) {
        let data = [];
        data.push(tenant);
        let isDeleted = await this.tenantService.deleteTenant(data);
        if (isDeleted) {
          let messageString = Constants.recordDeletedMessage;
          this._messageDialogService.openDialogBox('Success', messageString, Constants.msgBoxSuccess);
          this.getTenant(null);
        }
      }
    });
  }

  activeDeactiveTenant(tenant: Tenant) {
    let message;
    if (tenant.IsActive) {
      message = "Do you really want to deactivate tenant?"
      this._messageDialogService.openConfirmationDialogBox('Confirm', message, Constants.msgBoxWarning).subscribe(async (isConfirmed) => {
        if (isConfirmed) {
          let data = [];
          data.push(tenant);
          let isDeleted = await this.tenantService.deactivate(data);
          if (isDeleted) {
            let messageString = "Tenant deactivated successfully";
            this._messageDialogService.openDialogBox('Success', messageString, Constants.msgBoxSuccess);
            this.getTenant(null);
          }
        }
      });
    }
    else {
      message = "Do you really want to activate tenant?"
      this._messageDialogService.openConfirmationDialogBox('Confirm', message, Constants.msgBoxWarning).subscribe(async (isConfirmed) => {
        if (isConfirmed) {
          let data = [];
          data.push(tenant);
          let isDeleted = await this.tenantService.activate(data);
          if (isDeleted) {
            let messageString = "Tenant activated successfully";
            this._messageDialogService.openDialogBox('Success', messageString, Constants.msgBoxSuccess);
            this.getTenant(null);
          }
        }
      });
    }
  }

}
