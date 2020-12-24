import { Component, OnInit, Injector, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';
import { NgxUiLoaderService } from 'ngx-ui-loader';
import { Router, ActivatedRoute, NavigationEnd } from '@angular/router';
import { Constants } from '../../../shared/constants/constants';
import { ErrorMessageConstants } from '../../../shared/constants/constants';
import { MessageDialogService } from '../../../shared/services/mesage-dialog.service';
import { TenantService } from '../tenant.service';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { Tenant, TenantContact } from '../tenant';
export interface ListElement {
  contactType: string;
  firstName: string;
  lastName: string;
  email: string;
  no: string;
}

@Component({
  selector: 'app-view',
  templateUrl: './view.component.html',
  styleUrls: ['./view.component.scss']
})

export class ViewComponent implements OnInit {

  public isCollapsedDetails: boolean = false;
  public isCollapsedPermissions: boolean = true;
  public tenant: Tenant;
  public params;
  public userClaimsRolePrivilegeOperations: any[] = [];
  public isCollapsedAssets: boolean = true;
  public isContactContainer: boolean = false;
  public pageSize = 5;
  public currentPage = 0;
  public totalSize = 0;
  public pageNo = 0;
  public array: any;
  public sortedList: any[] = [];
  constructor(
    private _router: Router,
    private injector: Injector,
    private _messageDialogService: MessageDialogService,
    private tenantService: TenantService,
  ) {
    this.tenant = new Tenant;
    _router.events.subscribe(e => {
      if (e instanceof NavigationEnd) {
        if (e.url.includes('/tenant/Add')) {
          localStorage.removeItem("tenantparams");
        }
      }
    });

    _router.events.subscribe(e => {
      if (e instanceof NavigationEnd) {
        if (e.url.includes('/tenant')) {
          //set passing parameters to localstorage.
          this.params = JSON.parse(localStorage.getItem('tenantparams'));
          if (localStorage.getItem('tenantparams')) {
            this.tenant.TenantCode = this.params.Routeparams.passingparams.TenantTenantCode;
            //this.getTenantRecords();
          }
        } else {
          localStorage.removeItem("tenantparams");
        }
      }
    });

  }

  displayedColumns: string[] = ['contactType', 'firstName', 'lastName', 'email', 'no',];
  dataSource = new MatTableDataSource<any>();

  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;

  ngAfterViewInit() {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
  }

  public handlePage(e: any) {
    this.currentPage = e.pageIndex;
    this.pageSize = e.pageSize;
  }

  navigateToListPage() {
    this._router.navigate(['/tenants']);
  }

  async getTenantRecords() {
    let tenantService = this.injector.get(TenantService);
    let searchParameter: any = {};
    searchParameter.PagingParameter = {};
    searchParameter.PagingParameter.PageIndex = Constants.DefaultPageIndex;
    searchParameter.PagingParameter.PageSize = Constants.DefaultPageSize;
    searchParameter.SortParameter = {};
    searchParameter.SortParameter.SortColumn = 'Id';
    searchParameter.SortParameter.SortOrder = Constants.Descending;
    searchParameter.SearchMode = Constants.Exact;
    searchParameter.TenantCode = this.tenant.TenantCode;
    searchParameter.IsCountryRequired = true;
    searchParameter.IsContactRequired = true;
    searchParameter.IsSubscriptionRequired = true;
    var response = await tenantService.getTenant(searchParameter);
    this.tenant = response.List[0];
    this.dataSource = new MatTableDataSource<TenantContact>(this.tenant.TenantContacts);
    this.dataSource.sort = this.sort;
    this.array = this.tenant.TenantContacts;
    this.totalSize = this.tenant.TenantContacts.length;
  }

  public DataFormat;
  ngOnInit() {
    this.DataFormat = localStorage.getItem('DateFormat');
    var userClaimsDetail = JSON.parse(localStorage.getItem('userClaims'));
    if (userClaimsDetail) {
      this.userClaimsRolePrivilegeOperations = userClaimsDetail.Privileges;
    }
    else {
      this.userClaimsRolePrivilegeOperations = [];
    }
    if (localStorage.getItem('tenantparams')) {
      this.tenant.TenantCode = this.params.Routeparams.passingparams.TenantTenantCode;
      this.getTenantRecords();
    }
  }

  navigateToTenantEdit() {
    let queryParams = {
      Routeparams: {
        passingparams: {
          "TenantTenantCode": this.tenant.TenantCode,
        },
        filteredparams: {
          //passing data using json stringify.
          "TenantName": this.tenant.TenantName != null ? this.tenant.TenantName : ""
        }
      }
    }
    localStorage.setItem("tenantparams", JSON.stringify(queryParams))
    const router = this.injector.get(Router);
    router.navigate(['tenants', 'Edit']);
  }

  deleteTenant() {
    let message = 'Are you sure, you want to delete this record?';
    this._messageDialogService.openConfirmationDialogBox('Confirm', message, Constants.msgBoxWarning).subscribe(async (isConfirmed) => {
      if (isConfirmed) {
        let roleData = [{
          "TenantCode": this.tenant.TenantCode,
          "TenantType":this.tenant.TenantType
        }];

        let isDeleted = await this.tenantService.deleteTenant(roleData);
        if (isDeleted) {
          this.navigateToListPage();
          let messageString = Constants.recordDeletedMessage;
          this._messageDialogService.openDialogBox('Success', messageString, Constants.msgBoxSuccess);
          this._router.navigate(['tenant']);
        }
      }
    });
  }

  sortData(sort: MatSort) {
    const data = this.tenant.TenantContacts.slice();
    if (!sort.active || sort.direction === '') {
      this.sortedList = data;
      return;
    }
    //contactType', 'firstName', 'lastName', 'email'
    this.sortedList = data.sort((a, b) => {
      const isAsc = sort.direction === 'asc';
      switch (sort.active) {
        case 'contactType': return compareStr(a.ContactType, b.ContactType, isAsc);
        case 'firstName': return compareStr(a.FirstName, b.FirstName, isAsc);
        case 'lastName': return compareStr(a.LastName, b.LastName, isAsc);
        case 'email': return compareStr(a.EmailAddress, b.EmailAddress, isAsc);
        case 'no': return compareStr(a.ContactNumber, b.ContactNumber, isAsc);
        default: return 0;
      };
    });
    this.dataSource = new MatTableDataSource<TenantContact>(this.sortedList);
    this.dataSource.sort = this.sort;
    this.array = this.sortedList;
    this.totalSize = this.array.length;
    this.iterator();
  }

  private iterator() {
    const end = (this.currentPage + 1) * this.pageSize;
    const start = this.currentPage * this.pageSize;
    const part = this.array.slice(start, end);
    this.dataSource = part;
    this.dataSource.sort = this.sort;
  }
}
function compare(a: number, b: number, isAsc: boolean) {
  return (a < b ? -1 : 1) * (isAsc ? 1 : -1);
}

function compareStr(a: string, b: string, isAsc: boolean) {
  return (a.toLowerCase() < b.toLowerCase() ? -1 : 1) * (isAsc ? 1 : -1);
}

function compareDate(a: Date, b: Date, isAsc: boolean) {
  var a1 = new Date(a);
  var b1 = new Date(b);
  return (a1.getTime() < b1.getTime() ? -1 : 1) * (isAsc ? 1 : -1);
}

