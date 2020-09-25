import { Component, OnInit, Injector, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';
import { NgxUiLoaderService } from 'ngx-ui-loader';
import { Router, ActivatedRoute, NavigationEnd } from '@angular/router';
import { Constants } from '../../../shared/constants/constants';
import { ErrorMessageConstants } from '../../../shared/constants/constants';
import { MessageDialogService } from '../../../shared/services/mesage-dialog.service';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { Tenant } from '../../tenants/tenant';
import { TenantService } from '../../tenants/tenant.service';

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

  displayedColumns: string[] = ['firstName', 'lastName', 'email', 'no'];
  dataSource = new MatTableDataSource<any>();
  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
  
  constructor(private _router: Router,
    private injector: Injector,
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

    this.tenantgroupUsers = [
      { FirstName: 'Allan', LastName: 'Finch', EmailAddress: 'allan.finch@nis.com', ContactNumber: '+231-9734667889' },
      { FirstName: 'Glenn', LastName: 'Steyn', EmailAddress: 'glenn.styen@nis.com', ContactNumber: '+231-5235674356' },
      { FirstName: 'Dean', LastName: 'Jones', EmailAddress: 'dean.jones@nis.com', ContactNumber: '+231-6756734567' },
    ];
    this.dataSource = new MatTableDataSource<any>(this.tenantgroupUsers);
    this.dataSource.sort = this.sort;
    this.array = this.tenantgroupUsers;
    this.totalSize = this.array.length;
    this.iterator();
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
        let roleData = [{
          "TenantCode": this.tenantgroup.TenantCode,
        }];

        let isDeleted = await this.tenantService.deleteTenant(roleData);
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

}

function compareStr(a: string, b: string, isAsc: boolean) {
  return (a.toLowerCase() < b.toLowerCase() ? -1 : 1) * (isAsc ? 1 : -1);
}

function compare(a: number, b: number, isAsc: boolean) {
  return (a < b ? -1 : 1) * (isAsc ? 1 : -1);
}

