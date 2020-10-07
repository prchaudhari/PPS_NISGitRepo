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
import { TenantService } from '../../tenants/tenant.service';
import { Tenant } from '../../tenants/tenant';

const TenantGroupList: any[] = [
  { Id: '1', Name: 'Tenant Group 01', Description: 'Tenant Group Description 01', IsActive: true },
  { Id: '2', Name: 'Tenant Group 02', Description: 'Tenant Group Description 02', IsActive: true },
  { Id: '3', Name: 'Tenant Group 03', Description: 'Tenant Group Description 03', IsActive: true },
  { Id: '4', Name: 'Tenant Group 04', Description: 'Tenant Group Description 04', IsActive: true },
  { Id: '5', Name: 'Tenant Group 05', Description: 'Tenant Group Description 05', IsActive: true },
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
  public tenantgroupList: any[] = [];
  public sortedTenantGroupList: any[] = [];
  public TenantGroupFilterForm: FormGroup;
  public isFilterDone = false;
  public array: any;
  public userClaimsRolePrivilegeOperations: any[] = [];
  public totalRecordCount = 0;
  public filterTenantGroupNameValue = '';
  public sortOrder = Constants.Descending;
  public sortColumn = 'Id';

  displayedColumns: string[] = ['name', 'description', 'actions'];
  dataSource = new MatTableDataSource<any>();
  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;

  constructor(private injector: Injector,
    private fb: FormBuilder,
    private uiLoader: NgxUiLoaderService,
    private _messageDialogService: MessageDialogService,
    private route: Router,
    private localstorageservice: LocalStorageService,
    private tenantService: TenantService) {
    this.sortedTenantGroupList = this.tenantgroupList.slice();
  }

  ngOnInit() {
    var userClaimsDetail = JSON.parse(localStorage.getItem('userClaims'));
    if (userClaimsDetail) {
      if(userClaimsDetail.IsInstanceTenantManager == null || userClaimsDetail.IsInstanceTenantManager.toLocaleLowerCase() != 'true') {
        this.localstorageservice.removeLocalStorageData();
        this.route.navigate(['login']);
      }
      this.userClaimsRolePrivilegeOperations = userClaimsDetail.Privileges;
    }
    else {
      this.localstorageservice.removeLocalStorageData();
      this.route.navigate(['login']);
    }

    this.TenantGroupFilterForm = this.fb.group({
      filterName: [null]
    });
    this.getTenantGroups(null);

  }

  ngAfterViewInit() {
    this.dataSource.paginator = this.paginator;
  }

  public handlePage(e: any) {
    this.currentPage = e.pageIndex;
    this.pageSize = e.pageSize;
    this.getTenantGroups(null);
  }

  get filterName() {
    return this.TenantGroupFilterForm.get('filterName');
  }

  resetFilterForm() {
    this.TenantGroupFilterForm.patchValue({
      filterName: null,
    });

    this.currentPage = 0;
    this.filterTenantGroupNameValue = '';
  }

  closeFilter() {
    this.isFilter = !this.isFilter;
  }

  sortData(sort: MatSort) {
    const data = this.tenantgroupList.slice();
    if (!sort.active || sort.direction === '') {
      this.sortedTenantGroupList = data;
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
      case 'description': this.sortColumn = "TenantDescription"; break;
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
    this.getTenantGroups(searchParameter);
  }

  //function written to get tenant group list
  async getTenantGroups(searchParameter) {
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
    if (this.filterTenantGroupNameValue != null && this.filterTenantGroupNameValue != '') {
      searchParameter.TenantName = this.filterTenantGroupNameValue.trim();
    }

    searchParameter.IsPrimaryTenant = false;
    searchParameter.IsCountryRequired = true;
    searchParameter.TenantType = "Group";
    var response = await tenantService.getTenant(searchParameter);
    this.tenantgroupList = response.List;
    this.totalRecordCount = response.RecordCount;
    if (this.tenantgroupList.length == 0 && this.isFilterDone == true) {
      let message = ErrorMessageConstants.getNoRecordFoundMessage;
      this._messageDialogService.openDialogBox('Error', message, Constants.msgBoxError).subscribe(data => {
        if (data == true) {
          this.resetFilterForm();
          this.getTenantGroups(null);
        }
      });
    }
    this.dataSource = new MatTableDataSource<Tenant>(this.tenantgroupList);
    this.dataSource.sort = this.sort;
    this.array = this.tenantgroupList;
    this.totalSize = this.totalRecordCount;
    //this.iterator();
  }

  //This method has been used for fetching search tenant group records
  searchTenantGroupRecordFilter(searchType) {
    this.isFilterDone = true;
    if (searchType == 'reset') {
      this.resetFilterForm();
      this.getTenantGroups(null);
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

      if (this.TenantGroupFilterForm.value.filterName != null && this.TenantGroupFilterForm.value.filterName != '') {
        this.filterTenantGroupNameValue = this.TenantGroupFilterForm.value.filterName.trim();
        searchParameter.TenantName = this.TenantGroupFilterForm.value.filterName.trim();
      }

      this.currentPage = 0;
      this.getTenantGroups(searchParameter);
      this.isFilter = !this.isFilter;
    }
  }

  //this method helps to navigate to view tenant group details
  navigateToTenantGroupView(tenantgroup) {
    let queryParams = {
      Routeparams: {
        passingparams: {
          "TenantGroupCode": tenantgroup.TenantCode,
        },
        filteredparams: {
          "TenantGroupName": this.TenantGroupFilterForm.value.filterRoleName != null ? this.TenantGroupFilterForm.value.filterRoleName : ""
        }
      }
    }
    localStorage.setItem("tenantgroupparams", JSON.stringify(queryParams))
    const router = this.injector.get(Router);
    router.navigate(['tenantgroups', 'View']);
  }

  //this method helps to navigate add tenant group details
  navigateToTenantGroupAdd() {
    const router = this.injector.get(Router);
    router.navigate(['tenantgroups', 'Add']);
  }

  //this method helps to navigate edit tenant group details
  navigateToTenantGroupEdit(tenantgroup) {
    let queryParams = {
      Routeparams: {
        passingparams: {
          "TenantGroupCode": tenantgroup.TenantCode,
        },
        filteredparams: {
          "TenantGroupName": this.TenantGroupFilterForm.value.filterRoleName != null ? this.TenantGroupFilterForm.value.filterRoleName : ""
        }
      }
    }
    localStorage.setItem("tenantgroupparams", JSON.stringify(queryParams))
    const router = this.injector.get(Router);
    router.navigate(['tenantgroups', 'Edit']);
  }

  //function written to delete tenant group
  deleteTenantGroup(tenantgroup: any) {
    let message = 'Are you sure, you want to delete this record?';
    this._messageDialogService.openConfirmationDialogBox('Confirm', message, Constants.msgBoxWarning).subscribe(async (isConfirmed) => {
      if (isConfirmed) {
         let tenantGroupData = [{
           "TenantCode": tenantgroup.TenantCode,
         }];
        let isDeleted = await this.tenantService.deleteTenant(tenantGroupData);
        if (isDeleted) {
        let messageString = Constants.recordDeletedMessage;
        this._messageDialogService.openDialogBox('Success', messageString, Constants.msgBoxSuccess);
        this.getTenantGroups(null);
        }
      }
    });
  }

  //function written to activate / deactivate tenant group
  activeDeactiveTenantGroup(tenantgroup: any) {
    let message;
    if (tenantgroup.IsActive) {
      message = "Do you really want to deactivate tenant group?"
      this._messageDialogService.openConfirmationDialogBox('Confirm', message, Constants.msgBoxWarning).subscribe(async (isConfirmed) => {
        if (isConfirmed) {
          //let isDeleted = await this.tenantService.deactivateTenantGroup(tenantgroup.Id);
          //if (isDeleted) {
          let messageString = "Tenant group deactivated successfully";
          this._messageDialogService.openDialogBox('Success', messageString, Constants.msgBoxSuccess);
          //this.getTenantGroups();
          //}
        }
      });
    }
    else {
      message = "Do you really want to activate tenant group?"
      this._messageDialogService.openConfirmationDialogBox('Confirm', message, Constants.msgBoxWarning).subscribe(async (isConfirmed) => {
        if (isConfirmed) {
          //let isDeleted = await this.tenantService.activateTeantGroup(tenantgroup.Id);
          //if (isDeleted) {
          let messageString = "Tenant group activated successfully";
          this._messageDialogService.openDialogBox('Success', messageString, Constants.msgBoxSuccess);
          //this.getTenantGroups();
          //}
        }
      });
    }

  }

}
