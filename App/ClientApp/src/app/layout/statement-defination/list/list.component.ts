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
import { StatementService } from '../statement.service';
import { Statement } from '../statement';

@Component({
  selector: 'app-list',
  templateUrl: './list.component.html',
  styleUrls: ['./list.component.scss']
})
export class ListComponent implements OnInit {

  public isFilter: boolean = false;
  public statementList: Statement[] = [];
  public isLoaderActive: boolean = false;
  public isRecordFound: boolean = false;
  public pageNo = 0;
  public pageSize = 5;
  public currentPage = 0;
  public totalSize = 0;
  public array: any;
  public isFilterDone = false;
  public sortedStatementList: Statement[] = [];
  public pageTypeList: any[] = [];
  public StatementFilterForm: FormGroup;
  public filterPageTypeId: number = 0;
  public filterPageStatus: string = '';
  public filterFromDateError: boolean = false;
  public filterFromDateErrorMessage: string = "";
  public filterToDateError: boolean = false;
  public filterToDateErrorMessage: string = "";
  public userClaimsRolePrivilegeOperations: any[] = [];
  
  public totalRecordCount = 0;
  public filterDsplayName = '';
  public filterStatementOwner = '';
  public filterStatementStatus = '';
  public filterStatementPublishedOnStartDate = null;
  public filterStatementPublishedOnEndDate = null;

  closeFilter() {
    this.isFilter = !this.isFilter;
  }

  displayedColumns: string[] = ['name', 'owner', 'publishedBy', 'date', 'status', 'actions'];
  dataSource = new MatTableDataSource<any>();

  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;

  ngOnInit() {
    this.getStatements(null);
    this.StatementFilterForm = this.fb.group({
      filterDisplayName: [null],
      filterOwner: [null],
      filterStatus: [0],
      filterPageType: [null],
      filterPublishedOnFromDate: [null],
      filterPublishedOnToDate: [null],
    });
    var userClaimsDetail = JSON.parse(localStorage.getItem('userClaims'));
    if (userClaimsDetail) {
      this.userClaimsRolePrivilegeOperations = userClaimsDetail.Privileges;
    }
    else {
      this.userClaimsRolePrivilegeOperations = [];
    }
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
    private statementService: StatementService) {
    this.sortedStatementList = this.statementList.slice();
  }

  public handlePage(e: any) {
    this.currentPage = e.pageIndex;
    this.pageSize = e.pageSize;
    //this.iterator();
    this.getStatements(null);
  }

  private iterator() {
    const end = (this.currentPage + 1) * this.pageSize;
    const start = this.currentPage * this.pageSize;
    const part = this.array.slice(start, end);
    this.dataSource = part;
    this.dataSource.sort = this.sort;
  }

  //Getters for Page Forms
  get filterDisplayName() {
    return this.StatementFilterForm.get('filterDisplayName');
  }
  get filterOwner() {
    return this.StatementFilterForm.get('filterOwner');
  }
  get filterPageType() {
    return this.StatementFilterForm.get('filterPageType');
  }
  get filterStatus() {
    return this.StatementFilterForm.get('filterStatus');
  }
  get filterPublishedOnFromDate() {
    return this.StatementFilterForm.get('filterPublishedOnFromDate');
  }
  get filterPublishedOnToDate() {
    return this.StatementFilterForm.get('filterPublishedOnToDate');
  }
  
  sortData(sort: MatSort) {
    const data = this.statementList.slice();
    if (!sort.active || sort.direction === '') {
      this.sortedStatementList = data;
      return;
    }

    //['name', 'version', 'owner', 'date', 'status', 'actions'];
    this.sortedStatementList = data.sort((a, b) => {
      const isAsc = sort.direction === 'asc';
      switch (sort.active) {
        case 'name': return compareStr(a.Name, b.Name, isAsc);
        case 'status': return compareStr(a.Status, b.Status, isAsc);
        //case 'pagetype': return compareStr(a.PageTypeName, b.PageTypeName, isAsc);
        case 'owner': return compareStr(a.StatementOwnerName, b.StatementOwnerName, isAsc);
        case 'publishedBy': return compareStr(a.StatementPublishedByUserName, b.StatementPublishedByUserName, isAsc);
        case 'version': return compare(Number(a.Version), Number(b.Version), isAsc);
        case 'date': return compareDate(a.PublishedOn, b.PublishedOn, isAsc);
        default: return 0;
      };
    });

    this.dataSource = new MatTableDataSource<Statement>(this.sortedStatementList);
    this.dataSource.sort = this.sort;
    this.array = this.sortedStatementList;
    this.totalSize = this.totalRecordCount;
    //this.iterator();
  }

  async getStatements(searchParameter) {
    let statementService = this.injector.get(StatementService);
    if (searchParameter == null) {
      searchParameter = {};
      searchParameter.IsActive = true;
      searchParameter.PagingParameter = {};
      searchParameter.PagingParameter.PageIndex = this.currentPage + 1;
      searchParameter.PagingParameter.PageSize = this.pageSize;
      searchParameter.SortParameter = {};
      searchParameter.SortParameter.SortColumn = 'LastUpdatedDate';
      searchParameter.SortParameter.SortOrder = Constants.Descending;
      searchParameter.SearchMode = Constants.Contains;

      if (this.filterDsplayName != null && this.filterDsplayName != '') {
        searchParameter.Name = this.filterDsplayName.trim();
      }
      if (this.filterStatementOwner != null && this.filterStatementOwner != '') {
        searchParameter.StatementOwner = this.filterStatementOwner.trim();
      }
      if (this.filterPageTypeId != 0) {
        searchParameter.PageTypeId = this.filterPageTypeId;
      }
      if (this.filterStatementStatus != null && this.filterStatementStatus != '') {
        searchParameter.Status = this.filterStatementStatus;
      }
      if (this.filterStatementPublishedOnStartDate != null && this.filterStatementPublishedOnStartDate != '') {
        searchParameter.StartDate = new Date(this.filterStatementPublishedOnStartDate.setHours(0, 0, 0));
        searchParameter.SortParameter.SortColumn = 'PublishedOn';
      }
      if (this.filterStatementPublishedOnEndDate != null && this.filterStatementPublishedOnEndDate != '') {
        searchParameter.EndDate = new Date(this.filterStatementPublishedOnEndDate.setHours(23, 59, 59));
        searchParameter.SortParameter.SortColumn = 'PublishedOn';
      }
    }
    var response = await statementService.getStatements(searchParameter);
    this.statementList = response.List;
    this.totalRecordCount = response.RecordCount;
    if (this.statementList.length == 0 && this.isFilterDone == true) {
      let message = ErrorMessageConstants.getNoRecordFoundMessage;
      this._messageDialogService.openDialogBox('Error', message, Constants.msgBoxError).subscribe(data => {
        if (data == true) {
          this.resetStatementFilterForm();
          this.getStatements(null);
        }
      });
    }
    this.dataSource = new MatTableDataSource<Statement>(this.statementList);
    this.dataSource.sort = this.sort;
    this.array = this.statementList;
    this.totalSize = this.totalRecordCount;
    //this.iterator();
  }

  validateFilterDate(): boolean {
    if (this.StatementFilterForm.value.filterPublishedOnFromDate != null && this.StatementFilterForm.value.filterPublishedOnFromDate != '' &&
      this.StatementFilterForm.value.filterPublishedOnToDate != null && this.StatementFilterForm.value.filterPublishedOnToDate != '') {
      let startDate = this.StatementFilterForm.value.filterPublishedOnFromDate;
      let toDate = this.StatementFilterForm.value.filterPublishedOnToDate;
      if (startDate.getTime() > toDate.getTime()) {
        this.filterFromDateError = true;
        return false;
      }
    }
    return true;
  }

  onPublishedFilterDateChange(event) {
    this.filterFromDateError = false;
    this.filterToDateError = false;
    this.filterFromDateErrorMessage = "";
    this.filterToDateErrorMessage = "";
    let currentDte = new Date();
    if (this.StatementFilterForm.value.filterPublishedOnFromDate != null && this.StatementFilterForm.value.filterPublishedOnFromDate != '') {
      let startDate = this.StatementFilterForm.value.filterPublishedOnFromDate;
      if (startDate.getTime() > currentDte.getTime()) {
        this.filterFromDateError = true;
        this.filterFromDateErrorMessage = ErrorMessageConstants.getStartDateLessThanCurrentDateMessage;
      }
    }
    if (this.StatementFilterForm.value.filterPublishedOnToDate != null && this.StatementFilterForm.value.filterPublishedOnToDate != '') {
      let toDate = this.StatementFilterForm.value.filterPublishedOnToDate;
      if (toDate.getTime() > currentDte.getTime()) {
        this.filterToDateError = true;
        this.filterToDateErrorMessage = ErrorMessageConstants.getEndDateLessThanCurrentDateMessage;
      }
    }
    if (this.StatementFilterForm.value.filterPublishedOnFromDate != null && this.StatementFilterForm.value.filterPublishedOnFromDate != '' &&
      this.StatementFilterForm.value.filterPublishedOnToDate != null && this.StatementFilterForm.value.filterPublishedOnToDate != '') {
      let startDate = this.StatementFilterForm.value.filterPublishedOnFromDate;
      let toDate = this.StatementFilterForm.value.filterPublishedOnToDate;
      if (startDate.getTime() > toDate.getTime()) {
        this.filterFromDateError = true;
        this.filterFromDateErrorMessage = ErrorMessageConstants.getStartDateLessThanEndDateMessage;
      }
    }
  }

  //This method has been used for fetching search records
  searchStatementRecordFilter(searchType) {
    this.filterFromDateError = false;
    this.isFilterDone = true;
    if (searchType == 'reset') {
      this.resetStatementFilterForm();
      this.getStatements(null);
      this.isFilter = !this.isFilter;
    }
    else {
      if (this.validateFilterDate()) {
        let searchParameter: any = {};
        searchParameter.PagingParameter = {};
        searchParameter.PagingParameter.PageIndex = 1;
        searchParameter.PagingParameter.PageSize = this.pageSize;
        searchParameter.SortParameter = {};
        searchParameter.SortParameter.SortColumn = 'LastUpdatedDate';
        searchParameter.SortParameter.SortOrder = Constants.Ascending;
        searchParameter.SearchMode = Constants.Contains;

        if (this.StatementFilterForm.value.filterDisplayName != null && this.StatementFilterForm.value.filterDisplayName != '') {
          this.filterDsplayName = this.StatementFilterForm.value.filterDisplayName.trim(); 
          searchParameter.Name = this.StatementFilterForm.value.filterDisplayName.trim();
        }
        if (this.StatementFilterForm.value.filterOwner != null && this.StatementFilterForm.value.filterOwner != '') {
          this.filterStatementOwner = this.StatementFilterForm.value.filterOwner.trim();
          searchParameter.StatementOwner = this.StatementFilterForm.value.filterOwner.trim();
        }
        if (this.filterPageTypeId != 0) {
          searchParameter.PageTypeId = this.filterPageTypeId;
        }
        if (this.StatementFilterForm.value.filterStatus != null && this.StatementFilterForm.value.filterStatus != 0) {
          this.filterStatementStatus = this.StatementFilterForm.value.filterStatus;
          searchParameter.Status = this.StatementFilterForm.value.filterStatus;
        }
        if (this.StatementFilterForm.value.filterPublishedOnFromDate != null && this.StatementFilterForm.value.filterPublishedOnFromDate != '') {
          this.filterStatementPublishedOnStartDate = this.StatementFilterForm.value.filterPublishedOnFromDate;
          searchParameter.StartDate = new Date(this.StatementFilterForm.value.filterPublishedOnFromDate.setHours(0, 0, 0));
          searchParameter.SortParameter.SortColumn = 'PublishedOn';
        }
        if (this.StatementFilterForm.value.filterPublishedOnToDate != null && this.StatementFilterForm.value.filterPublishedOnToDate != '') {
          this.filterStatementPublishedOnEndDate = this.StatementFilterForm.value.filterPublishedOnToDate;
          searchParameter.EndDate = new Date(this.StatementFilterForm.value.filterPublishedOnToDate.setHours(23, 59, 59));
          searchParameter.SortParameter.SortColumn = 'PublishedOn';
        }

        this.currentPage = 0;
        this.getStatements(searchParameter);
        this.isFilter = !this.isFilter;
      }
    }
  }

  resetStatementFilterForm() {
    this.StatementFilterForm.patchValue({
      filterDisplayName: null,
      filterOwner: null,
      filterPageType: 0,
      filterStatus: 0,
      filterPublishedOnFromDate: null,
      filterPublishedOnToDate: null
    });

    this.currentPage = 0;
    this.filterDsplayName = '';
    this.filterStatementStatus = '';
    this.filterStatementOwner = '';
    this.filterStatementPublishedOnStartDate = null;
    this.filterStatementPublishedOnEndDate = null;
    this.filterFromDateError = false;
    this.filterToDateError = false;
    this.filterFromDateErrorMessage = "";
    this.filterToDateErrorMessage = "";
  }

  //this method helps to navigate to view
  navigateToStatementView(statement: Statement) {
    let queryParams = {
      Routeparams: {
        passingparams: {
          "StatementIdentifier": statement.Identifier,
        },
        filteredparams: {
          //passing data using json stringify.
          "StatementName": this.StatementFilterForm.value.filterRoleName != null ? this.StatementFilterForm.value.filterRoleName : ""
        }
      }
    }
    localStorage.setItem("statementparams", JSON.stringify(queryParams))
    const router = this.injector.get(Router);
    router.navigate(['statementdefination', 'View']);
  }

  //this method helps to navigate to add
  navigateToStatementAdd() {
    const router = this.injector.get(Router);
    router.navigate(['statementdefination', 'Add']);
  }

  //this method helps to navigate edit
  navigateToStatementEdit(statement) {
    let queryParams = {
      Routeparams: {
        passingparams: {
          "StatementIdentifier": statement.Identifier,
        },
        filteredparams: {
          //passing data using json stringify.
          "StatementName": this.StatementFilterForm.value.filterRoleName != null ? this.StatementFilterForm.value.filterRoleName : ""
        }
      }
    }
    localStorage.setItem("statementparams", JSON.stringify(queryParams))
    const router = this.injector.get(Router);
    router.navigate(['statementdefination', 'Edit']);
  }

  //function written to delete role
  deleteStatement(role: Statement) {
    let message = 'Are you sure, you want to delete this record?';
    this._messageDialogService.openConfirmationDialogBox('Confirm', message, Constants.msgBoxWarning).subscribe(async (isConfirmed) => {
      if (isConfirmed) {
        let roleData = [{
          "Identifier": role.Identifier,
        }];

        let isDeleted = await this.statementService.deleteStatement(roleData);
        if (isDeleted) {
          let messageString = Constants.recordDeletedMessage;
          this._messageDialogService.openDialogBox('Success', messageString, Constants.msgBoxSuccess);
          this.getStatements(null);

        }
      }
    });
  }

  async PublishStatement(statement: Statement) {
    let message = "Are you sure, you want to publish this record?";
    this._messageDialogService.openConfirmationDialogBox('Confirm', message, Constants.msgBoxWarning).subscribe(async (isConfirmed) => {
      if (isConfirmed) {
        let pageData = [{
          "Identifier": statement.Identifier,
        }];
        let resultFlag = await this.statementService.publishStatement(pageData);
        if (resultFlag) {
          let messageString = Constants.StatementPublishedSuccessfullyMessage;
          this._messageDialogService.openDialogBox('Success', messageString, Constants.msgBoxSuccess);
          this.resetStatementFilterForm();
          this.getStatements(null);
        }
      }
    });
  }

  async PreviewStatement(statement: Statement) {
    let pageData = [{
      "Identifier": statement.Identifier,
    }];
    let resultHtmlString = await this.statementService.previewStatement(pageData);
    if (resultHtmlString != '') {
      this._messageDialogService.openPreviewDialogBox(resultHtmlString);
    }
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
