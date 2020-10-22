import { Component, OnInit, Injector, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { HttpClient } from '@angular/common/http';
import { TenantUserService } from '../../tenantuser/tenantuser.service';
import { Router, NavigationEnd } from '@angular/router';
import { Constants } from 'src/app/shared/constants/constants';
import { LocalStorageService } from 'src/app/shared/services/local-storage.service';
import { MessageDialogService } from 'src/app/shared/services/mesage-dialog.service';
import { TenantUser } from '../../tenantuser/tenantuser';
import { UserService } from '../../users/user.service';

@Component({
  selector: 'app-list',
  templateUrl: './list.component.html',
  styleUrls: ['./list.component.scss']
})
export class ListComponent implements OnInit {

  tenantgroupuserFormGroup: FormGroup;
  public isFilter: boolean = false;

  public tenantgroupuserList;
  public tenantgroupuserLists = [];
  public dataAdapter: any = [];
  public columns = [];
  public actionCellsrenderer: any;
  public searchForm: FormGroup;
  public searchedTenantGroupUserList;
  public status;
  public displayRecords;
  public isLoaderActive: boolean = false;
  public params;
  public UserIdentifier;
  public lockSlider: boolean = false;
  public statusSlider: boolean = false;
  public tenantuserImage;
  public isRecordFound: boolean;
  public isLocked: boolean
  public isActive: boolean
  public isFilterDone = false;
  displayedColumns: string[] = ['name', 'email', 'mobileno', 'active', 'actions'];
  dataSource = new MatTableDataSource<any>();
  public sortOrder = Constants.Ascending;
  public sortColumn = 'FirstName';

  public sortedTenantGroupUserList = [];

  public lockStatusArray: any[] = [
    {
      'Identifier': 0,
      'Name': 'Both'
    },
    {
      'Identifier': 1,
      'Name': 'Locked'
    },
    {
      'Identifier': 2,
      'Name': 'Unlocked'
    }

  ];

  public activeStatusArray: any[] = [

    {
      'Identifier': 0,
      'Name': 'Both'
    },
    {
      'Identifier': 1,
      'Name': 'Active'
    },
    {
      'Identifier': 2,
      'Name': 'Inactive'
    }
  ];

  public array: any;
  public pageSize = 5;
  public currentPage = 0;
  public totalSize = 0;
  public previousPageLabel: string;
  public totalRecordCount = 0;
  public profileImage;
  public TenantGroupUserFilter: any = {
    FirstName: null,
    EmailAddress: null,
    LockStatus: null,
    ActivationStatus: null,
  };
  public userClaimsRolePrivilegeOperations: any[] = [];
  public loginUserEmailAddress: string = '';
  public TenantCode = '';

  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: true }) sort: MatSort;

  constructor(private http: HttpClient,
    private formBuilder: FormBuilder,
    private service: TenantUserService,
    private userservice: UserService,
    private router: Router,
    private localstorageservice: LocalStorageService,
    private _messageDialogService: MessageDialogService,
  ) {
    //remove localstorage item.
    router.events.subscribe(e => {
      if (e instanceof NavigationEnd) {
        if (e.url.includes('/tenantgroupusers')) {

        } else {
          localStorage.removeItem("tenantgroupuserRouteparams")
        }
      }
    });
    //getting localstorage item
    if (localStorage.getItem("tenantgroupuserRouteparams")) {
      this.params = JSON.parse(localStorage.getItem('tenantgroupuserRouteparams'));
      this.UserIdentifier = this.params.Routeparams.passingparams.UserIdentifier;
    }
    this.sortedTenantGroupUserList = this.tenantgroupuserLists.slice();
  }

  sortData(sort: MatSort) {
    const data = this.tenantgroupuserLists.slice();
    if (!sort.active || sort.direction === '') {
      this.sortedTenantGroupUserList = data;
      return;
    }
    if (sort.direction == 'asc') {
      this.sortOrder = Constants.Ascending;
    }
    else {
      this.sortOrder = Constants.Descending;
    }
    if (sort.active == 'name') {
      this.sortColumn = 'FirstName';
    }
    else if (sort.active == 'email') {
      this.sortColumn = 'EmailAddress';
    }
    else if (sort.active == 'mobileno') {
      this.sortColumn = 'ContactNumber';
    }
    else if (sort.active == 'active') {
      this.sortColumn = 'IsActive';
    }
    else if (sort.active == 'lock') {
      this.sortColumn = 'IsLocked';
    }
    let searchParameter: any = {};
    searchParameter.PagingParameter = {};
    searchParameter.PagingParameter.PageIndex = this.currentPage + 1;
    searchParameter.PagingParameter.PageSize = this.pageSize;
    searchParameter.SortParameter = {};
    searchParameter.SortParameter.SortColumn = this.sortColumn;
    searchParameter.SortParameter.SortOrder = this.sortOrder;
    searchParameter.SearchMode = Constants.Contains;
    if (this.tenantgroupuserFormGroup.value.FirstName != null) {
      searchParameter.FirstName = this.TenantGroupUserFilter.FirstName.trim();
    }
    if (this.tenantgroupuserFormGroup.value.EmailAddress != null) {
      searchParameter.EmailAddress = this.TenantGroupUserFilter.EmailAddress.trim();
    }
    searchParameter.LockStatus = this.TenantGroupUserFilter.LockStatus;
    searchParameter.ActivationStatus = this.TenantGroupUserFilter.ActivationStatus;
    this.getTenantGroupUsers(searchParameter);
  }

  public handlePage(e: any) {
    this.currentPage = e.pageIndex;
    this.pageSize = e.pageSize;
    let searchParameter: any = {};
    searchParameter.PagingParameter = {};
    searchParameter.PagingParameter.PageIndex = this.currentPage + 1;
    searchParameter.PagingParameter.PageSize = this.pageSize;
    searchParameter.SortParameter = {};
    searchParameter.SortParameter.SortColumn = Constants.UserName;
    searchParameter.SortParameter.SortColumn = this.sortColumn;
    searchParameter.SortParameter.SortOrder = this.sortOrder;
    if (this.tenantgroupuserFormGroup.value.FirstName != null) {
      searchParameter.FirstName = this.TenantGroupUserFilter.FirstName.trim();
    }
    if (this.tenantgroupuserFormGroup.value.EmailAddress != null) {
      searchParameter.EmailAddress = this.TenantGroupUserFilter.EmailAddress.trim();
    }
    searchParameter.LockStatus = this.TenantGroupUserFilter.LockStatus;
    searchParameter.ActivationStatus = this.TenantGroupUserFilter.ActivationStatus;
    this.getTenantGroupUsers(searchParameter);
  }

  ngOnInit() {
    var userClaimsDetail = JSON.parse(localStorage.getItem('userClaims'));
    if (userClaimsDetail) {
      if(userClaimsDetail.IsTenantGroupManager == null || userClaimsDetail.IsTenantGroupManager.toLocaleLowerCase() != 'true') {
        this.localstorageservice.removeLocalStorageData();
        this.router.navigate(['login']);
      }
      this.userClaimsRolePrivilegeOperations = userClaimsDetail.Privileges;
    }
    else {
      this.localstorageservice.removeLocalStorageData();
      this.router.navigate(['login']);
    }
    this.loginUserEmailAddress = userClaimsDetail.UserPrimaryEmailAddress;
    this.TenantCode = userClaimsDetail.TenantCode;
    this.tenantgroupuserFormGroup = this.formBuilder.group({
      FirstName: ['', Validators.compose([])],
      EmailAddress: ['', Validators.compose([])],
      TenantUserActiveStatus: [0, Validators.compose([])],
      TenantUserLockStatus: [0, Validators.compose([])],
    })
    this.fetchTenantGroupUserRecord();
  }

  //Get api for fetching Tenant Group Users--
  async getTenantGroupUsers(searchParameter) {
    if (searchParameter == null) {
      let newsearchParameter: any = {};
      newsearchParameter.PagingParameter = {};
      newsearchParameter.PagingParameter.PageIndex = this.currentPage + 1;
      newsearchParameter.PagingParameter.PageSize = this.pageSize;
      newsearchParameter.SortParameter = {};
      newsearchParameter.SortParameter.SortColumn = Constants.UserName;
      newsearchParameter.SortParameter.SortOrder = Constants.Ascending;
      newsearchParameter.SearchMode = Constants.Contains;
      searchParameter = newsearchParameter;
    }
    searchParameter.IsGroupManager = true;
    searchParameter.TenantCode = this.TenantCode;
    var response = await this.service.getUser(searchParameter);
    this.tenantgroupuserLists = response.usersList;
    this.totalRecordCount = response.RecordCount;
    if (this.tenantgroupuserLists.length == 0 && this.isFilterDone == true) {
      let message = "No record found";
      this._messageDialogService.openDialogBox('Error', message, Constants.msgBoxError).subscribe(data => {
        if (data == true) {
          this.getAllUSer();
        }
      });
    }
    this.tenantgroupuserLists.forEach(el => {
      if (el.Image != '' && el.Image != null) {
        el.Image = el.Image;
      }
    })

    if (this.tenantgroupuserLists.length == 0 && this.isFilterDone == true) {
      let message = "Tenant group user not found";
    }
    this.dataSource = new MatTableDataSource<TenantUser>(this.tenantgroupuserLists);
    this.dataSource.sort = this.sort;
    this.array = this.tenantgroupuserLists;
    this.totalSize = this.totalRecordCount;

    if (this.tenantgroupuserLists.length > 0) {
    }
    else {
      this.status = "No Records Found";
    }
  }

  fetchTenantGroupUserRecord() {
    this.params = JSON.parse(localStorage.getItem('tenantgroupuserRouteparams'));
    if (localStorage.getItem('tenantgroupuserRouteparams')) {
      this.UserIdentifier = this.params.Routeparams.passingparams.UserIdentifier;
    }
    let searchParameter: any = {};
    searchParameter.PagingParameter = {};
    searchParameter.PagingParameter.PageIndex = this.currentPage + 1;
    searchParameter.PagingParameter.PageSize = this.pageSize;
    searchParameter.SortParameter = {};
    searchParameter.SortParameter.SortColumn = this.sortColumn;
    searchParameter.SortParameter.SortOrder = this.sortOrder;
    searchParameter.SearchMode = Constants.Contains;
    this.getTenantGroupUsers(searchParameter);
  }

  //Function to navigate to view page of perticular tenantuser detail--
  viewTenantGroupUser(tenantgroupuser) {
    let queryParams = {
      Routeparams: {
        passingparams: {
          "UserIdentifier": tenantgroupuser.Identifier,
        },
        filteredparams: {
          //passing data using json stringify.
          "FirstName": this.TenantGroupUserFilter.FirstName != null ? this.TenantGroupUserFilter.FirstName : null,
          "EmailAddress": this.TenantGroupUserFilter.EmailAddress != null ? this.TenantGroupUserFilter.EmailAddress : null,
          "MobileNumber": this.TenantGroupUserFilter.MobileNumber != null ? this.TenantGroupUserFilter.MobileNumber : null,
          "LockStatus": this.TenantGroupUserFilter.LockStatus,
          "ActivationStatus": this.TenantGroupUserFilter.ActivationStatus,
        }
      }
    }
    localStorage.setItem("tenantgroupuserRouteparams", JSON.stringify(queryParams))
    this.router.navigate(['tenantgroupusers', 'view']);
  }

  //Function to edit perticular tenant group user--
  editTenantGroupUser(tenantgroupuser) {
    let queryParams = {
      Routeparams: {
        passingparams: {
          "UserIdentifier": tenantgroupuser.Identifier,
        },
        filteredparams: {
          //passing data using json stringify.
          "FirstName": this.TenantGroupUserFilter.FirstName != null ? this.TenantGroupUserFilter.FirstName : null,
          "EmailAddress": this.TenantGroupUserFilter.EmailAddress != null ? this.TenantGroupUserFilter.EmailAddress : null,
          "LockStatus": this.TenantGroupUserFilter.LockStatus != null ? this.TenantGroupUserFilter.LockStatus : null,
          "ActivationStatus": this.TenantGroupUserFilter.ActivationStatus != null ? this.TenantGroupUserFilter.ActivationStatus : null,
        }
      }
    }
    localStorage.setItem("tenantgroupuserRouteparams", JSON.stringify(queryParams))
    this.router.navigate(['tenantgroupusers', 'edit']);
  }

  //function written to lock and unlock tenant group user--
  unLockTenantGroupUser(tenantgroupuser: any) {
    if (tenantgroupuser.IsLocked) {
      let message = "Do you want to unlock tenant group user??"
      this._messageDialogService.openConfirmationDialogBox('Confirm', message, Constants.msgBoxWarning).subscribe(async (isConfirmed) => {
        if (isConfirmed) {
          this.isLoaderActive = true;
          let result = await this.service.unlockUser(tenantgroupuser.Identifier);
          if (result) {
            let messageString = Constants.recordUnlockedMessage;
            this._messageDialogService.openDialogBox('Success', messageString, Constants.msgBoxSuccess);
            this.fetchTenantGroupUserRecord();
          }
        }
      });
    }
    else {
      let message = "Do you want to lock tenant group user??"
      this._messageDialogService.openConfirmationDialogBox('Confirm', message, Constants.msgBoxWarning).subscribe(async (isConfirmed) => {
        if (isConfirmed) {
          this.isLoaderActive = true;
          let result = await this.service.userlockUrl(tenantgroupuser.Identifier);
          if (result) {
            let messageString = Constants.recordlockedMessage;
            this._messageDialogService.openDialogBox('Success', messageString, Constants.msgBoxSuccess);
            this.fetchTenantGroupUserRecord();
          }
        }
      });
    }
  }

  //function written to activate and de-activate tenant group user--
  activeDeactiveTenantGroupUser(tenantgroupuser: any) {
    let message;
    if (tenantgroupuser.IsActive) {
      message = "Do you really want to deactivate tenant group user?"
      this._messageDialogService.openConfirmationDialogBox('Confirm', message, Constants.msgBoxWarning).subscribe(async (isConfirmed) => {
        if (isConfirmed) {
          this.isLoaderActive = true;
          let result = await this.service.deactivate(tenantgroupuser.Identifier);
          if (result) {
            let messageString = "Record deactivated successfully";
            this._messageDialogService.openDialogBox('Success', messageString, Constants.msgBoxSuccess);
            this.fetchTenantGroupUserRecord();
          }
        }
      });
    }
    else {
      message = "Do you really want to activate tenant group user?"
      this._messageDialogService.openConfirmationDialogBox('Confirm', message, Constants.msgBoxWarning).subscribe(async (isConfirmed) => {
        if (isConfirmed) {
          this.isLoaderActive = true;
          let result = await this.service.activate(tenantgroupuser.Identifier);
          if (result) {
            let messageString = "Record activated successfully";
            this._messageDialogService.openDialogBox('Success', messageString, Constants.msgBoxSuccess);
            this.fetchTenantGroupUserRecord();
          }
        }
      });
    }
  }

  resetPassword(tenantuser) {
    let message = 'Are you sure, you want to reset password for this record?';
    this._messageDialogService.openConfirmationDialogBox('Confirm', message, Constants.msgBoxWarning).subscribe(async (isConfirmed) => {
      if (isConfirmed) {
        let data = {
          "EmailAddress": tenantuser.EmailAddress
        };
        let result = await this.userservice.sendPassword(data);
        if (result) {
          let messageString = Constants.sentPasswordMailMessage;
          this._messageDialogService.openDialogBox('Success', messageString, Constants.msgBoxSuccess);
        }
      }
    });
  }

  //this method helps to navigate to add
  navigateToAddTenantGroupUser() {
    this.router.navigate(['tenantgroupusers', 'add']);
  }

  public onLockStatusSelected(event) {
    const value = event.target.value;
    if (value == "0") {
      this.TenantGroupUserFilter.LockStatus = null;
    }
    else if (value == "1") {
      this.TenantGroupUserFilter.LockStatus = true;
    }
    else if (value == "2") {
      this.TenantGroupUserFilter.LockStatus = false;
    }
  }

  public onActiveStatusSelected(event) {
    const value = event.target.value;
    if (value == "0") {
      this.TenantGroupUserFilter.ActivationStatus = null;
    }
    else if (value == "1") {
      this.TenantGroupUserFilter.ActivationStatus = true;
    }
    else if (value == "2") {
      this.TenantGroupUserFilter.ActivationStatus = false;
    }
  }

  //TenantUser filter function--
  getAllUSer() {
    this.isFilterDone = true;
    let searchParameter: any = {};
    searchParameter.PagingParameter = {};
    searchParameter.PagingParameter.PageIndex = 1;
    searchParameter.PagingParameter.PageSize = 5;
    searchParameter.SortParameter = {};
    searchParameter.SortParameter.SortColumn = Constants.UserName;
    searchParameter.SortParameter.SortOrder = Constants.Ascending;
    searchParameter.SearchMode = Constants.Contains;
    this.TenantGroupUserFilter = {
      FirstName: null,
      EmailAddress: null,
      LockStatus: null,
      ActivationStatus: null,
    };

    this.getTenantGroupUsers(searchParameter);
    this.tenantgroupuserFormGroup.controls['TenantUserLockStatus'].setValue(0);
    this.tenantgroupuserFormGroup.controls['TenantUserActiveStatus'].setValue(0);
  }

  //TenantUser filter function--
  filterSetUp(searchType) {
    this.isFilterDone = true;
    if (searchType == 'Reset') {
      localStorage.removeItem("tenantuserRouteparams");
      this.TenantGroupUserFilter = {
        FirstName: null,
        EmailAddress: null,
        LockStatus: null,
        ActivationStatus: null,
      };
      this.tenantgroupuserFormGroup.controls['TenantUserLockStatus'].setValue(0);
      this.tenantgroupuserFormGroup.controls['TenantUserActiveStatus'].setValue(0);
      this.isFilter = !this.isFilter;
      this.fetchTenantGroupUserRecord();
      this.tenantgroupuserLists = [];
      this.searchedTenantGroupUserList = [];
    }
    else {
      let searchParameter: any = {};
      searchParameter.PagingParameter = {};
      searchParameter.PagingParameter.PageIndex = 1;
      searchParameter.PagingParameter.PageSize = this.pageSize;
      searchParameter.SortParameter = {};
      searchParameter.SortParameter.SortColumn = Constants.UserName;
      searchParameter.SortParameter.SortOrder = Constants.Ascending;
      searchParameter.SearchMode = Constants.Contains;
      if (this.tenantgroupuserFormGroup.value.FirstName != null) {
        searchParameter.FirstName = this.TenantGroupUserFilter.FirstName.trim();
      }
      if (this.tenantgroupuserFormGroup.value.EmailAddress != null) {
        searchParameter.EmailAddress = this.TenantGroupUserFilter.EmailAddress.trim();
      }
      searchParameter.LockStatus = this.TenantGroupUserFilter.LockStatus;
      searchParameter.ActivationStatus = this.TenantGroupUserFilter.ActivationStatus;
      this.currentPage = 0;
      this.getTenantGroupUsers(searchParameter);
      this.isFilter = !this.isFilter;
    }
  }

  //Function to close the filter popup--
  closeFilter() {
    this.isFilter = !this.isFilter;
  }

  activationEventCheck(event) {
    if (event.checked) {
      this.TenantGroupUserFilter.ActivationStatus = true
    }
    else {
      this.TenantGroupUserFilter.ActivationStatus = false;
    }
  }

  lockEventCheck(event) {
    if (event.checked) {
      this.TenantGroupUserFilter.LockStatus = true;
    }
    else {
      this.TenantGroupUserFilter.LockStatus = false;
    }
  }
}


