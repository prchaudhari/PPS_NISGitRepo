import { Component, OnInit, Injector, ViewChild } from '@angular/core';
import { Router, ActivatedRoute, NavigationEnd } from '@angular/router';
import { Constants } from '../../../shared/constants/constants';
import { MessageDialogService } from '../../../shared/services/mesage-dialog.service';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { Tenant } from '../../tenants/tenant';
import { TenantService } from '../../tenants/tenant.service';
import { UserService } from '../../users/user.service';

@Component({
  selector: 'app-view',
  templateUrl: './view.component.html',
  styleUrls: ['./view.component.scss']
})

export class ViewComponent implements OnInit {

  public isCollapsedDetails: boolean = false;
  public isCollapsedPermissions: boolean = true;
  public tenantgroup: Tenant;
  public tenantgroupUsers: any[] = [];
  public tenantGroupUserList: any[] = [];
  public params;
  public userClaimsRolePrivilegeOperations: any[] = [];
  public isCollapsedTenantGroupUsers: boolean = true;
  public isTenantGroupUserContainer: boolean = false;
  public pageSize = 5;
  public currentPage = 0;
  public totalSize = 0;
  public pageNo = 0;
  public array: any;
  public sortedList: any[] = [];

  displayedColumns: string[] = ['firstName', 'lastName', 'email', 'no', 'actions'];
  dataSource = new MatTableDataSource<any>();
  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;

  constructor(private _router: Router,
    private injector: Injector,
    private service: UserService,
    private tenantService: TenantService,
    private _messageDialogService: MessageDialogService) {
    this.tenantgroup = new Tenant;
    _router.events.subscribe(e => {
      if (e instanceof NavigationEnd) {
        if (e.url.includes('/tenantgroups/Add')) {
          localStorage.removeItem("tenantgroupparams");
        }
      }
    });

    _router.events.subscribe(e => {
      if (e instanceof NavigationEnd) {
        if (e.url.includes('/tenantgroups')) {
          //set passing parameters to localstorage.
          this.params = JSON.parse(localStorage.getItem('tenantgroupparams'));
          if (localStorage.getItem('tenantgroupparams')) {
            this.tenantgroup.TenantCode = this.params.Routeparams.passingparams.TenantGroupCode;
          }
        } else {
          localStorage.removeItem("tenantgroupparams");
        }
      }
    });
  }

  ngAfterViewInit() {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
  }

  public handlePage(e: any) {
    this.currentPage = e.pageIndex;
    this.pageSize = e.pageSize;
    this.iterator();
  }

  navigateToListPage() {
    this._router.navigate(['/tenantgroups']);
  }

  async getTenantGroupRecords() {
    let tenantService = this.injector.get(TenantService);
    let searchParameter: any = {};
    searchParameter.PagingParameter = {};
    searchParameter.PagingParameter.PageIndex = Constants.DefaultPageIndex;
    searchParameter.PagingParameter.PageSize = Constants.DefaultPageSize;
    searchParameter.SortParameter = {};
    searchParameter.SortParameter.SortColumn = 'Id';
    searchParameter.SortParameter.SortOrder = Constants.Descending;
    searchParameter.SearchMode = Constants.Exact;
    searchParameter.TenantCode = this.tenantgroup.TenantCode;
    var response = await tenantService.getTenant(searchParameter);
    this.tenantgroup = response.List[0];
    this.BindTenantGroupUsers();
  }

  ngOnInit() {
    var userClaimsDetail = JSON.parse(localStorage.getItem('userClaims'));
    if (userClaimsDetail) {
      this.userClaimsRolePrivilegeOperations = userClaimsDetail.Privileges;
    }
    else {
      this.userClaimsRolePrivilegeOperations = [];
    }
    this.getTenantGroupRecords();
  }

  navigateToTenantGroupEdit() {
    let queryParams = {
      Routeparams: {
        passingparams: {
          "TenantGroupCode": this.tenantgroup.TenantCode,
        },
        filteredparams: {
          //passing data using json stringify.
          "TenantGroupName": this.tenantgroup.TenantName != null ? this.tenantgroup.TenantName : ""
        }
      }
    }
    localStorage.setItem("tenantgroupparams", JSON.stringify(queryParams))
    const router = this.injector.get(Router);
    router.navigate(['tenantgroups', 'Edit']);
  }

  deleteTenantGroup() {
    let message = 'Are you sure, you want to delete this record?';
    this._messageDialogService.openConfirmationDialogBox('Confirm', message, Constants.msgBoxWarning).subscribe(async (isConfirmed) => {
      if (isConfirmed) {
        let data = [{
          "TenantCode": this.tenantgroup.TenantCode,
          "TenantType": this.tenantgroup.TenantType,
        }];

        let isDeleted = await this.tenantService.deleteTenant(data);
        if (isDeleted) {
          this.navigateToListPage();
          let messageString = Constants.recordDeletedMessage;
          this._messageDialogService.openDialogBox('Success', messageString, Constants.msgBoxSuccess);
        }
      }
    });
  }

  sortData(sort: MatSort) {
    const data = this.tenantgroupUsers.slice();
    if (!sort.active || sort.direction === '') {
      this.sortedList = data;
      return;
    }
    this.sortedList = data.sort((a, b) => {
      const isAsc = sort.direction === 'asc';
      switch (sort.active) {
        case 'firstName': return compareStr(a.FirstName, b.FirstName, isAsc);
        case 'lastName': return compareStr(a.LastName, b.LastName, isAsc);
        case 'email': return compareStr(a.EmailAddress, b.EmailAddress, isAsc);
        case 'no': return compareStr(a.ContactNumber, b.ContactNumber, isAsc);
        default: return 0;
      };
    });
    this.dataSource = new MatTableDataSource<any>(this.sortedList);
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

  async BindTenantGroupUsers() {
    let searchParameter: any = {};
    searchParameter.PagingParameter = {};

    searchParameter.SortParameter = {};
    searchParameter.SortParameter.SortColumn = Constants.UserName;
    searchParameter.SortParameter.SortOrder = Constants.Ascending;
    searchParameter.SearchMode = Constants.Contains;
    searchParameter.TenantCode = this.tenantgroup.TenantCode;
    searchParameter.IsGroupManager = true;
    var response = await this.service.getUser(searchParameter);
    var userList = response.usersList;
    // this.totalRecordCount = response.RecordCount;
    this.tenantGroupUserList = userList
    this.dataSource = new MatTableDataSource<any>(this.tenantGroupUserList);
    this.dataSource.sort = this.sort;
    this.array = this.tenantGroupUserList;
    this.totalSize = this.array.length;
    this.iterator();
  }

  resetPassword(tenantgroupuser) {
    let message = 'Are you sure, you want to reset password for this record?';
    this._messageDialogService.openConfirmationDialogBox('Confirm', message, Constants.msgBoxWarning).subscribe(async (isConfirmed) => {
      if (isConfirmed) {
        let data = {
          "EmailAddress": tenantgroupuser.EmailAddress
        };
        let result = await this.service.sendPassword(data);
        if (result) {
          let messageString = Constants.sentPasswordMailMessage;
          this._messageDialogService.openDialogBox('Success', messageString, Constants.msgBoxSuccess);
        }
      }
    });
  }
}

function compareStr(a: string, b: string, isAsc: boolean) {
  return (a.toLowerCase() < b.toLowerCase() ? -1 : 1) * (isAsc ? 1 : -1);
}

function compare(a: number, b: number, isAsc: boolean) {
  return (a < b ? -1 : 1) * (isAsc ? 1 : -1);
}

