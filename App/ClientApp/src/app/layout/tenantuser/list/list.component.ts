import { Component, OnInit, Injector, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { HttpClient, HttpResponse, HttpEvent, HttpEventType } from '@angular/common/http';
import { TenantUserService } from '../tenantuser.service';
import { Router, ActivatedRoute, NavigationEnd } from '@angular/router';
import { CellRenderService } from 'src/app/shared/services/cellsrenderer';
import { DialogService } from '@tomblue/ng2-bootstrap-modal';
import { Constants } from 'src/app/shared/constants/constants';
import { LocalStorageService } from 'src/app/shared/services/local-storage.service';
import { NgxUiLoaderService } from 'ngx-ui-loader';
import { PaginationInstance } from 'src/app/shared/modules/pagination/pagination.module';
import { MessageDialogService } from 'src/app/shared/services/mesage-dialog.service';
import { ConfigConstants } from 'src/app/shared/constants/configConstants';
import { ResourceService } from 'src/app/shared/services/resource.service';
import { TenantUser } from '../tenantuser';
import { LoginService } from '../../../login/login.service';

@Component({
  selector: 'app-list',
  templateUrl: './list.component.html',
  styleUrls: ['./list.component.scss']
})
export class ListComponent implements OnInit {

  tenantuserFormGroup: FormGroup;
  public isFilter: boolean = false;

  public tenantuserList;
  public tenantuserLists = [];
  public roleList = [{ "Name": "Select Role", "Identifier": 0 }];
  public dataAdapter: any = [];
  public columns = [];
  public actionCellsrenderer: any;
  public createWidget: boolean;
  public message;
  public searchForm: FormGroup;
  public searchedTenantUserList;
  public status;
  public displayRecords;
  public isLoaderActive: boolean = false;
  public params;
  public TenantUserIdentifier;
  public TenantUserName;
  public preferredLanguage;
  public tenantuserListResources = {}
  public ResourceLoadingFailedMsg = Constants.ResourceLoadingFailedMsg;
  public Locale;
  public sectionStr;
  public tenantuserClaimsRolePrivilegeOperations;
  public roleListsArr: any = [];
  public roleListSource: any[] = []
  public lockSlider: boolean = false;
  public statusSlider: boolean = false;
  public tenantuserImage;
  public isRecordFound: boolean;
  public organizationUnitName;
  public emailId;
  public designation;
  public roles;
  public isLocked: boolean
  public isActive: boolean
  public designationList = [];
  public designationLists: any = [];
  public designationListSource: any = [];
  public languageList = [];
  public languageLists: any = [];
  public languageSource: any[] = []
  public ouList: any[] = []
  public ouName;
  public isFilterDone = false;
  displayedColumns: string[] = ['name', 'email', 'mobileno', 'active', 'actions'];
  dataSource = new MatTableDataSource<any>();
  public sortOrder = Constants.Ascending;
  public sortColumn = 'FirstName';

  public sortedTenantUserList = [];

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
  public loggedInTenantUserIdentifier;
  public profileImage;
  public TenantUserFilter: any = {
    FirstName: null,
    LastName: null,
    Code: null,
    EmailAddress: null,
    MobileNumber: null,
    OrganisationUnitIdentifier: null,
    RoleIdentifier: null,
    DesignationIdentifier: null,
    PreferedLanguageIdentifier: null,
    LockStatus: null,
    ActivationStatus: true,
  };

  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: true }) sort: MatSort;


  constructor(private http: HttpClient,
    private formBuilder: FormBuilder,
    private service: TenantUserService,
    private router: Router,
    private route: ActivatedRoute,
    private actionCellWindow: CellRenderService,
    private _dialogService: DialogService,
    private localstorageservice: LocalStorageService,
    private fb: FormBuilder,
    private spinner: NgxUiLoaderService,
    private _messageDialogService: MessageDialogService,
    private injector: Injector,
    private loginService: LoginService,
  ) {
    //remove localstorage item.
    router.events.subscribe(e => {
      if (e instanceof NavigationEnd) {
        if (e.url.includes('/tenantuser')) {

        } else {
          localStorage.removeItem("tenantuserRouteparams")
        }
      }
    });
    //getting localstorage item
    if (localStorage.getItem("tenantuserRouteparams")) {
      this.params = JSON.parse(localStorage.getItem('tenantuserRouteparams'));
      this.TenantUserIdentifier = this.params.Routeparams.passingparams.TenantUserIdentifier;
      this.TenantUserFilter.FirstName = this.params.Routeparams.filteredparams.FirstName;
      this.TenantUserFilter.LastName = this.params.Routeparams.filteredparams.LastName;
      this.TenantUserFilter.Code = this.params.Routeparams.filteredparams.Code;
      this.TenantUserFilter.EmailAddress = this.params.Routeparams.filteredparams.EmailAddress;
      this.TenantUserFilter.OrganisationUnitIdentifier = this.params.Routeparams.filteredparams.OrganisationUnitIdentifier;
      this.TenantUserFilter.MobileNumber = this.params.Routeparams.filteredparams.MobileNumber;
      this.TenantUserFilter.DesignationIdentifier = this.params.Routeparams.filteredparams.DesignationIdentifier;
      this.TenantUserFilter.RoleIdentifier = this.params.Routeparams.filteredparams.RoleIdentifier;
      this.TenantUserFilter.LockStatus = this.params.Routeparams.filteredparams.LockStatus;
      this.TenantUserFilter.ActivationStatus = this.params.Routeparams.filteredparams.ActivationStatus;;
    }

    this.sortedTenantUserList = this.tenantuserLists.slice();
  }

  sortData(sort: MatSort) {
    const data = this.roleList.slice();
    if (!sort.active || sort.direction === '') {
      this.sortedTenantUserList = data;
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
    if (this.tenantuserFormGroup.value.FirstName != null) {
      searchParameter.FirstName = this.TenantUserFilter.FirstName.trim();
    }
    if (this.tenantuserFormGroup.value.EmailAddress != null) {
      searchParameter.EmailAddress = this.TenantUserFilter.EmailAddress.trim();
    }
    if (this.TenantUserFilter.RoleIdentifier != null) {
      searchParameter.RoleIdentifier = this.TenantUserFilter.RoleIdentifier;
    }
    searchParameter.LockStatus = this.TenantUserFilter.LockStatus;
    searchParameter.ActivationStatus = this.TenantUserFilter.ActivationStatus;
    this.getTenantUserdetail(searchParameter);
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
    if (this.tenantuserFormGroup.value.FirstName != null) {
      searchParameter.FirstName = this.TenantUserFilter.FirstName.trim();
    }
    if (this.tenantuserFormGroup.value.EmailAddress != null) {
      searchParameter.EmailAddress = this.TenantUserFilter.EmailAddress.trim();
    }
    if (this.TenantUserFilter.RoleIdentifier != null) {
      searchParameter.RoleIdentifier = this.TenantUserFilter.RoleIdentifier;
    }
    searchParameter.LockStatus = this.TenantUserFilter.LockStatus;
    searchParameter.ActivationStatus = this.TenantUserFilter.ActivationStatus;
    this.getTenantUserdetail(searchParameter);
  }

  private iterator() {
    const end = (this.currentPage + 1) * this.pageSize;
    const start = this.currentPage * this.pageSize;
    const part = this.array.slice(start, end);
    this.dataSource = part;
    this.dataSource.sort = this.sort;
  }


  ngOnInit() {
    // this.getTenantUserdetail();
    this.tenantuserFormGroup = this.formBuilder.group({
      FirstName: ['', Validators.compose([])],
      EmailAddress: ['', Validators.compose([])],
      TenantUserActiveStatus: [1, Validators.compose([])],
      TenantUserLockStatus: [0, Validators.compose([])],
    })
    //var tenantuserClaimsDetail = JSON.parse(localStorage.getItem('tenantuserClaims'));
    //this.tenantuserClaimsRolePrivilegeOperations = tenantuserClaimsDetail.Privileges;
    //this.loggedInTenantUserIdentifier = tenantuserClaimsDetail.TenantUserIdentifier;
    this.fetchTenantUserRecord();
  }

  //Get api for fetching TenantUser details--
  async getTenantUserdetail(searchParameter) {
    if (searchParameter == null) {
      let newsearchParameter: any = {};
      newsearchParameter.PagingParameter = {};
      newsearchParameter.PagingParameter.PageIndex = this.currentPage + 1;
      newsearchParameter.PagingParameter.PageSize = this.pageSize;
      newsearchParameter.SortParameter = {};
      newsearchParameter.SortParameter.SortColumn = Constants.UserName;
      newsearchParameter.SortParameter.SortOrder = Constants.Ascending;
      newsearchParameter.SearchMode = Constants.Contains;
      newsearchParameter.ActivationStatus = true;
      searchParameter = newsearchParameter;
    }
    searchParameter.IsInstanceManager = true;
    var response = await this.service.getUser(searchParameter);
    this.tenantuserLists = response.usersList;
    this.totalRecordCount = response.RecordCount;

    if (this.tenantuserLists.length == 0 && this.isFilterDone == true) {
      let message = "No record found";//this.roleListResources['lblNoRecord'] == undefined ? this.ResourceLoadingFailedMsg : this.roleListResources['lblNoRecord']
      this._messageDialogService.openDialogBox('Error', message, Constants.msgBoxError).subscribe(data => {
        if (data == true) {
          this.getAllUSer();
        }
      });
    }
    this.tenantuserLists.forEach(el => {
      if (el.Image != '' && el.Image != null) {
        el.Image = el.Image;
      }

    })

    if (this.tenantuserLists.length == 0 && this.isFilterDone == true) {
      let message = "TenantUser not found"

    }
    this.dataSource = new MatTableDataSource<TenantUser>(this.tenantuserLists);
    this.dataSource.sort = this.sort;
    this.array = this.tenantuserLists;
    this.totalSize = this.totalRecordCount;

    if (this.tenantuserLists.length > 0) {
    }
    else {
      this.status = "No Records Found";
    }
  }

  fetchTenantUserRecord() {
    this.params = JSON.parse(localStorage.getItem('tenantuserRouteparams'));
    if (localStorage.getItem('tenantuserRouteparams')) {
      this.TenantUserIdentifier = this.params.Routeparams.passingparams.TenantUserIdentifier;
      this.TenantUserFilter.FirstName = this.params.Routeparams.filteredparams.FirstName;
      this.TenantUserFilter.LastName = this.params.Routeparams.filteredparams.LastName;
      this.TenantUserFilter.Code = this.params.Routeparams.filteredparams.Code;
      this.TenantUserFilter.EmailAddress = this.params.Routeparams.filteredparams.EmailAddress;
      this.TenantUserFilter.OrganisationUnitIdentifier = this.params.Routeparams.filteredparams.OrganisationUnitIdentifier;
      this.TenantUserFilter.MobileNumber = this.params.Routeparams.filteredparams.MobileNumber;
      this.TenantUserFilter.DesignationIdentifier = this.params.Routeparams.filteredparams.DesignationIdentifier;
      this.TenantUserFilter.RoleIdentifier = this.params.Routeparams.filteredparams.RoleIdentifier;
      this.TenantUserFilter.PreferedLanguageIdentifier = this.params.Routeparams.filteredparams.PreferedLanguageIdentifier;
      this.TenantUserFilter.LockStatus = this.params.Routeparams.filteredparams.LockStatus;
      this.TenantUserFilter.ActivationStatus = this.params.Routeparams.filteredparams.ActivationStatus;
    }
    let searchParameter: any = {};
    searchParameter.PagingParameter = {};
    searchParameter.PagingParameter.PageIndex = this.currentPage + 1;
    searchParameter.PagingParameter.PageSize = this.pageSize;
    searchParameter.SortParameter = {};
    searchParameter.SortParameter.SortColumn = this.sortColumn;
    searchParameter.SortParameter.SortOrder = this.sortOrder;
    searchParameter.SearchMode = Constants.Contains;
    searchParameter.GetResources = true;
    if (this.TenantUserFilter.FirstName != null) {
      searchParameter.FirstName = this.TenantUserFilter.FirstName;
    }
    if (this.TenantUserFilter.LastName != null) {
      searchParameter.LastName = this.TenantUserFilter.LastName;
    }
    if (this.TenantUserFilter.Code != null) {
      searchParameter.Code = this.TenantUserFilter.Code;
    }
    if (this.TenantUserFilter.EmailAddress != null) {
      searchParameter.EmailAddress = this.TenantUserFilter.EmailAddress;
    }
    if (this.TenantUserFilter.MobileNumber != null) {
      searchParameter.MobileNumber = this.TenantUserFilter.MobileNumber;
    }
    if (this.TenantUserFilter.OrganisationUnitIdentifier != null) {
      searchParameter.OrganisationUnitIdentifier = this.TenantUserFilter.OrganisationUnitIdentifier;
    }
    if (this.TenantUserFilter.DesignationIdentifier != null) {
      searchParameter.DesignationIdentifier = this.TenantUserFilter.DesignationIdentifier;
    }
    if (this.TenantUserFilter.PreferedLanguageIdentifier != null) {
      searchParameter.PreferedLanguageIdentifier = this.TenantUserFilter.PreferedLanguageIdentifier;
    }
    if (this.TenantUserFilter.RoleIdentifier != null) {
      searchParameter.RoleIdentifier = this.TenantUserFilter.RoleIdentifier;
    }
    searchParameter.LockStatus = this.TenantUserFilter.LockStatus;
    searchParameter.ActivationStatus = this.TenantUserFilter.ActivationStatus;
    this.getTenantUserdetail(searchParameter);
  }

  //Function to navigate to view page of perticular tenantuser detail--
  viewTenantUser(tenantuser) {
    let queryParams = {
      Routeparams: {
        passingparams: {
          "TenantUserIdentifier": tenantuser.Identifier,
        },
        filteredparams: {
          //passing data using json stringify.
          "FirstName": this.TenantUserFilter.FirstName != null ? this.TenantUserFilter.FirstName : null,
          "LastName": this.TenantUserFilter.LastName != null ? this.TenantUserFilter.LastName : null,
          "Code": this.TenantUserFilter.Code != null ? this.TenantUserFilter.Code : null,
          "EmailAddress": this.TenantUserFilter.EmailAddress != null ? this.TenantUserFilter.EmailAddress : null,
          "MobileNumber": this.TenantUserFilter.MobileNumber != null ? this.TenantUserFilter.MobileNumber : null,
          "OrganisationUnitIdentifier": this.TenantUserFilter.OrganisationUnitIdentifier != null ? this.TenantUserFilter.OrganisationUnitIdentifier : null,
          "DesignationIdentifier": this.TenantUserFilter.DesignationIdentifier != null ? this.TenantUserFilter.DesignationIdentifier : null,
          "PreferedLanguageIdentifier": this.TenantUserFilter.PreferedLanguageIdentifier != null ? this.TenantUserFilter.PreferedLanguageIdentifier : null,
          "RoleIdentifier": this.TenantUserFilter.RoleIdentifier != null ? this.TenantUserFilter.RoleIdentifier : null,
          "LockStatus": this.TenantUserFilter.LockStatus,
          "ActivationStatus": this.TenantUserFilter.ActivationStatus,
        }
      }
    }
    localStorage.setItem("tenantuserRouteparams", JSON.stringify(queryParams))
    this.router.navigate(['tenantusers', 'view']);
  }

  //Function to edit perticular tenantuser--
  editTenantUser(tenantuser) {

    let queryParams = {
      Routeparams: {
        passingparams: {
          "TenantUserIdentifier": tenantuser.Identifier,
        },
        filteredparams: {
          //passing data using json stringify.
          "FirstName": this.TenantUserFilter.FirstName != null ? this.TenantUserFilter.FirstName : null,
          "LastName": this.TenantUserFilter.LastName != null ? this.TenantUserFilter.LastName : null,
          "EmailAddress": this.TenantUserFilter.EmailAddress != null ? this.TenantUserFilter.EmailAddress : null,
          "MobileNumber": this.TenantUserFilter.MobileNumber != null ? this.TenantUserFilter.MobileNumber : null,
          "RoleIdentifier": this.TenantUserFilter.RoleIdentifier != null ? this.TenantUserFilter.RoleIdentifier : null,
          "LockStatus": this.TenantUserFilter.LockStatus != null ? this.TenantUserFilter.LockStatus : null,
          "ActivationStatus": this.TenantUserFilter.ActivationStatus != null ? this.TenantUserFilter.ActivationStatus : null,
        }
      }
    }
    localStorage.setItem("tenantuserRouteparams", JSON.stringify(queryParams))
    this.router.navigate(['tenantusers', 'edit']);
  }

  //function written to delete tenantuser--
  deleteTenantUser(tenantuser: TenantUser) {
    let message = "Are you sure you want to delete this record?";
    this._messageDialogService.openConfirmationDialogBox('Confirm', message, Constants.msgBoxWarning).subscribe(async (isConfirmed) => {
      if (isConfirmed) {

        let isDeleted = await this.service.deleteUser(tenantuser.Identifier);
        if (isDeleted) {
          let messageString = Constants.recordDeletedMessage;
          this._messageDialogService.openDialogBox('Success', messageString, Constants.msgBoxSuccess);
          this.fetchTenantUserRecord();
        }
      }
    });
  }

  //function written to unlock tenantuser--
  unLockTenantUser(tenantuser: TenantUser) {
    if (tenantuser.IsLocked) {
      let message = "Do you want to unlock tenantuser??"
      this._messageDialogService.openConfirmationDialogBox('Confirm', message, Constants.msgBoxWarning).subscribe(async (isConfirmed) => {
        if (isConfirmed) {
          this.isLoaderActive = true;
          let isDeleted = await this.service.unlockUser(tenantuser.Identifier);
          if (isDeleted) {
            let messageString = Constants.recordUnlockedMessage;
            this._messageDialogService.openDialogBox('Success', messageString, Constants.msgBoxSuccess);
            this.fetchTenantUserRecord();
          }
        }
      });
    }
    else {
      let message = "Do you want to lock tenantuser??"
      this._messageDialogService.openConfirmationDialogBox('Confirm', message, Constants.msgBoxWarning).subscribe(async (isConfirmed) => {
        if (isConfirmed) {
          this.isLoaderActive = true;
          let isDeleted = await this.service.userlockUrl(tenantuser.Identifier);
          if (isDeleted) {
            let messageString = Constants.recordlockedMessage;
            this._messageDialogService.openDialogBox('Success', messageString, Constants.msgBoxSuccess);
            this.fetchTenantUserRecord();
          }
        }
      });
    }

  }

  activeDeactiveTenantUser(tenantuser: TenantUser) {
    let message;
    if (tenantuser.IsActive) {
      message = "Do you really want to deactivate tenantuser?"
      this._messageDialogService.openConfirmationDialogBox('Confirm', message, Constants.msgBoxWarning).subscribe(async (isConfirmed) => {
        if (isConfirmed) {
          this.isLoaderActive = true;
          let isDeleted = await this.service.deactivate(tenantuser.Identifier);
          if (isDeleted) {
            let messageString = "Record deactivated successfully";
            this._messageDialogService.openDialogBox('Success', messageString, Constants.msgBoxSuccess);
            this.fetchTenantUserRecord();
          }
        }
      });
    }
    else {
      message = "Do you really want to activate tenantuser?"

      this._messageDialogService.openConfirmationDialogBox('Confirm', message, Constants.msgBoxWarning).subscribe(async (isConfirmed) => {
        if (isConfirmed) {
          this.isLoaderActive = true;
          let isDeleted = await this.service.activate(tenantuser.Identifier);
          if (isDeleted) {
            let messageString = "Record activated successfully";
            this._messageDialogService.openDialogBox('Success', messageString, Constants.msgBoxSuccess);
            this.fetchTenantUserRecord();
          }
        }
      });
    }

  }

  //this method helps to navigate to add
  navigateToAddTenantUser() {
    this.router.navigate(['tenantusers', 'add']);
  }

  public onRoleSelected(event) {
    const value = event.target.value;
    if (value == "0") {
      this.TenantUserFilter.RoleIdentifier = null;
    }
    else {
      this.TenantUserFilter.RoleIdentifier = Number(value);

    }
  }

  public onLockStatusSelected(event) {
    const value = event.target.value;
    if (value == "0") {
      this.TenantUserFilter.LockStatus = null;
    }
    else if (value == "1") {
      this.TenantUserFilter.LockStatus = true;
    }
    else if (value == "2") {
      this.TenantUserFilter.LockStatus = false;


    }
  }

  public onActiveStatusSelected(event) {
    const value = event.target.value;
    if (value == "0") {
      this.TenantUserFilter.ActivationStatus = null;
    }
    else if (value == "1") {
      this.TenantUserFilter.ActivationStatus = true;


    }
    else if (value == "2") {
      this.TenantUserFilter.ActivationStatus = false;


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
    this.TenantUserFilter = {
      FirstName: null,
      LastName: null,
      Code: null,
      EmailAddress: null,
      MobileNumber: null,
      OrganisationUnitIdentifier: null,
      RoleIdentifier: null,
      LockStatus: null,
      ActivationStatus: null,
    };

    this.getTenantUserdetail(searchParameter);
    this.tenantuserFormGroup.controls['TenantUserLockStatus'].setValue(0);
    this.tenantuserFormGroup.controls['TenantUserActiveStatus'].setValue(0);
  }

  //TenantUser filter function--
  filterSetUp(searchType) {
    this.isFilterDone = true;
    if (searchType == 'Reset') {
      localStorage.removeItem("tenantuserRouteparams");
      this.TenantUserFilter = {
        FirstName: null,
        LastName: null,
        Code: null,
        EmailAddress: null,
        MobileNumber: null,
        OrganisationUnitIdentifier: null,
        RoleIdentifier: null,
        LockStatus: null,
        ActivationStatus: null,
      };
      this.tenantuserFormGroup.controls['TenantUserLockStatus'].setValue(0);
      this.tenantuserFormGroup.controls['TenantUserActiveStatus'].setValue(0);
      this.isFilter = !this.isFilter;
      this.fetchTenantUserRecord();
      this.tenantuserLists = [];
      this.searchedTenantUserList = [];
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
      if (this.tenantuserFormGroup.value.FirstName != null) {
        searchParameter.FirstName = this.TenantUserFilter.FirstName.trim();
      }
      if (this.tenantuserFormGroup.value.EmailAddress != null) {
        searchParameter.EmailAddress = this.TenantUserFilter.EmailAddress.trim();
      }
      if (this.TenantUserFilter.RoleIdentifier != null) {
        searchParameter.RoleIdentifier = this.TenantUserFilter.RoleIdentifier;
      }
      searchParameter.LockStatus = this.TenantUserFilter.LockStatus;
      searchParameter.ActivationStatus = this.TenantUserFilter.ActivationStatus;
      this.currentPage = 0;
      this.getTenantUserdetail(searchParameter);
      this.isFilter = !this.isFilter;
    }
  }

  //Function to close the filter popup--
  closeFilter() {
    this.isFilter = !this.isFilter;
  }

  activationEventCheck(event) {
    if (event.checked) {
      this.TenantUserFilter.ActivationStatus = true
    }
    else {
      this.TenantUserFilter.ActivationStatus = false;
    }
  }

  lockEventCheck(event) {
    if (event.checked) {
      this.TenantUserFilter.LockStatus = true;
    }
    else {
      this.TenantUserFilter.LockStatus = false;
    }
  }
}

function compareStr(a: string, b: string, isAsc: boolean) {
  return (a.toLowerCase() < b.toLowerCase() ? -1 : 1) * (isAsc ? 1 : -1);
}

function compareNumber(a: number, b: number, isAsc: boolean) {
  return (a < b ? -1 : 1) * (isAsc ? 1 : -1);
}

