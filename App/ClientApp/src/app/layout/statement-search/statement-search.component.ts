import { Component, OnInit, Injector, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';
import { NgxUiLoaderService } from 'ngx-ui-loader';
import { Router } from '@angular/router';
import { Constants, ErrorMessageConstants } from '../../shared/constants/constants';
import { MessageDialogService } from '../../shared/services/mesage-dialog.service';
import { LocalStorageService } from '../../shared/services/local-storage.service';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { StatementSearchService } from './statementsearch.service';
import { StatementSearch } from './statementsearch';
import { HttpClient, HttpHeaders, HttpErrorResponse, HttpResponse, HttpRequest } from '@angular/common/http';
import { WindowRef } from '../../core/services/window-ref.service';
import { map } from 'rxjs/operators';
import * as $ from 'jquery';
import { ConfigConstants } from '../../shared/constants/configConstants';

export interface ListElement {
  id: string;
  date: string;
  period: string;
  customer: string;
  accountId: string;
  city: string;
}

@Component({
  selector: 'app-statement-search',
  templateUrl: './statement-search.component.html',
  styleUrls: ['./statement-search.component.scss']
})
export class StatementSearchComponent implements OnInit {
  //public variables
  public isFilter: boolean = true;
  public scheduleLogList: StatementSearch[] = [];
  public isLoaderActive: boolean = false;
  public isRecordFound: boolean = false;
  public pageNo = 0;
  public pageSize = 5;
  public currentPage = 0;
  public totalSize = 0;
  public array: any;
  public isFilterDone = false;
  public sortedStatementSearchList: StatementSearch[] = [];
  public pageTypeList: any[] = [];
  public StatementSearchFilterForm: FormGroup;
  public filterPageTypeId: number = 0;
  public filterPageStatus: string = '';
  public filterFromDateError: boolean = false;
  public filterFromDateErrorMessage: string = "";
  public filterToDateError: boolean = false;
  public filterToDateErrorMessage: string = "";
  public userClaimsRolePrivilegeOperations: any[] = [];
  public baseURL = ConfigConstants.BaseURL;
  closeFilter() {
    this.isFilter = !this.isFilter;
  }

  displayedColumns: string[] = ['customer', 'accountId', 'accounttype', 'date', 'period', 'actions'];
  dataSource = new MatTableDataSource<any>();

  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;

  constructor(private injector: Injector,
    private fb: FormBuilder,
    private uiLoader: NgxUiLoaderService,
    private _messageDialogService: MessageDialogService,
    private route: Router,
    private localstorageservice: LocalStorageService,
    private scheduleLogService: StatementSearchService,
    private _window: WindowRef, private http: HttpClient) {
    this.sortedStatementSearchList = this.scheduleLogList.slice();
    this._window.nativeWindow.ViewHTML = function (element: any): void {
      //me.DownloadAsset(id);
    };
  }

  public handlePage(e: any) {
    this.currentPage = e.pageIndex;
    this.pageSize = e.pageSize;
    this.iterator();
  }

  private iterator() {
    const end = (this.currentPage + 1) * this.pageSize;
    const start = this.currentPage * this.pageSize;
    const part = this.array.slice(start, end);
    this.dataSource = part;
    this.dataSource.sort = this.sort;
  }

  //Getters for Page Forms
  get filterStatementCustomer() {
    return this.StatementSearchFilterForm.get('filterStatementCustomer');
  }

  get filterStatementAccountId() {
    return this.StatementSearchFilterForm.get('filterStatementAccountId');
  }

  get filterStatementDate() {
    return this.StatementSearchFilterForm.get('filterStatementDate');
  }

  get filterStatementPeriod() {
    return this.StatementSearchFilterForm.get('filterStatementPeriod');
  }

  ngOnInit() {
    //this.getStatementSearchs(null);
    //this.getPageTypes();
    this.StatementSearchFilterForm = this.fb.group({
      filterStatementCustomer: [null],
      filterStatementAccountId: [null],
      filterStatementDate: [null],
      filterStatementPeriod: [null],
    });
    this.StatementSearchFilterForm.controls['filterStatementCustomer'].setValue(null);
    this.StatementSearchFilterForm.controls['filterStatementAccountId'].setValue(null);
    this.StatementSearchFilterForm.controls['filterStatementDate'].setValue(null);
    this.StatementSearchFilterForm.controls['filterStatementPeriod'].setValue(null);

    var userClaimsDetail = JSON.parse(localStorage.getItem('userClaims'));
    if (userClaimsDetail) {
      this.userClaimsRolePrivilegeOperations = userClaimsDetail.Privileges;
    }
    else {
      this.userClaimsRolePrivilegeOperations = [];
    }
  }

  sortData(sort: MatSort) {
    const data = this.scheduleLogList.slice();
    if (!sort.active || sort.direction === '') {
      this.sortedStatementSearchList = data;
      return;
    }
    //displayedColumns: string[] = ['id', 'date', 'period', 'customer', 'accountId', 'actions'];
    this.sortedStatementSearchList = data.sort((a, b) => {
      const isAsc = sort.direction === 'asc';
      switch (sort.active) {
        case 'accountId': return compareStr(a.AccountNumber, b.AccountNumber, isAsc);
        case 'customer': return compareStr(a.CustomerName, b.CustomerName, isAsc);
        case 'accounttype': return compareStr(a.CustomerName, b.CustomerName, isAsc);
        case 'period': return compareStr(a.StatementPeriod, b.StatementPeriod, isAsc);
        case 'date': return compareDate(a.StatementDate, b.StatementDate, isAsc);
        default: return 0;
      }
    });
    this.dataSource = new MatTableDataSource<StatementSearch>(this.sortedStatementSearchList);
    this.dataSource.sort = this.sort;
    this.array = this.sortedStatementSearchList;
    this.totalSize = this.array.length;
    this.iterator();
  }

  async getStatementSearchs(searchParameter) {
    let scheduleLogService = this.injector.get(StatementSearchService);
    if (searchParameter == null) {
      searchParameter = {};
      searchParameter.PagingParameter = {};
      searchParameter.PagingParameter.PageIndex = Constants.DefaultPageIndex;
      searchParameter.PagingParameter.PageSize = Constants.DefaultPageSize;
      searchParameter.SortParameter = {};
      searchParameter.SortParameter.SortColumn = 'Id';
      searchParameter.SortParameter.SortOrder = Constants.Descending;
      searchParameter.SearchMode = Constants.Contains;
    }
    this.scheduleLogList = await scheduleLogService.getStatementSearch(searchParameter);
    if (this.scheduleLogList.length == 0 && this.isFilterDone == true) {
      let message = ErrorMessageConstants.getNoRecordFoundMessage;
      this._messageDialogService.openDialogBox('Error', message, Constants.msgBoxError).subscribe(data => {
        if (data == true) {
          //this.resetRoleFilterForm();
          this.getStatementSearchs(null);
        }
      });
    }
    this.dataSource = new MatTableDataSource<StatementSearch>(this.scheduleLogList);
    this.dataSource.sort = this.sort;
    this.array = this.scheduleLogList;
    this.totalSize = this.array.length;
    this.iterator();
  }
  validateFilterDate(): boolean {
    let currentDte = new Date();
    if (this.StatementSearchFilterForm.value.filterStatementDate != null && this.StatementSearchFilterForm.value.filterStatementDate != '') {
      let toDate = this.StatementSearchFilterForm.value.filterStatementDate;
      if (toDate.getTime() > currentDte.getTime()) {
        this.filterToDateError = true;
        this.filterToDateErrorMessage = "Statement date should not greater than current date";
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
    
    if (this.StatementSearchFilterForm.value.filterStatementDate != null && this.StatementSearchFilterForm.value.filterStatementDate != '') {
      let toDate = this.StatementSearchFilterForm.value.filterStatementDate;
      if (toDate.getTime() > currentDte.getTime()) {
        this.filterToDateError = true;
        this.filterToDateErrorMessage = "Statement date should not greater than current date";
      }
    }
  }
  searchStatementSearchRecordFilter(searchType) {
    this.filterFromDateError = false;
    this.isFilterDone = true;
    if (searchType == 'reset') {
      this.resetSchdeuleLogFilterForm();
      //this.getStatementSearchs(null);
      this.scheduleLogList = [];
      this.dataSource = new MatTableDataSource<StatementSearch>(this.scheduleLogList);
      this.dataSource.sort = this.sort;
      this.array = this.scheduleLogList;
      this.totalSize = this.array.length;
      this.isFilter = !this.isFilter;
    }
    else {
      if (this.validateFilterDate()) {
        let searchParameter: any = {};
        searchParameter.PagingParameter = {};
        searchParameter.PagingParameter.PageIndex = Constants.DefaultPageIndex;
        searchParameter.PagingParameter.PageSize = Constants.DefaultPageSize;
        searchParameter.SortParameter = {};
        searchParameter.SortParameter.SortColumn = 'Id';
        searchParameter.SortParameter.SortOrder = Constants.Descending;
        searchParameter.SearchMode = Constants.Contains;
        if (this.StatementSearchFilterForm.value.filterStatementCustomer != null && this.StatementSearchFilterForm.value.filterStatementCustomer != '') {
          searchParameter.StatementCustomer = this.StatementSearchFilterForm.value.filterStatementCustomer.trim();
        }
        if (this.StatementSearchFilterForm.value.filterStatementAccountId != null && this.StatementSearchFilterForm.value.filterStatementAccountId != '') {
          searchParameter.StatementAccount = this.StatementSearchFilterForm.value.filterStatementAccountId.trim();
        }
        if (this.StatementSearchFilterForm.value.filterStatementDate != null && this.StatementSearchFilterForm.value.filterStatementDate != '') {
          //searchParameter.StartDate = this.StatementSearchFilterForm.value.filterStatementDate;
          searchParameter.StatementStartDate = new Date(this.StatementSearchFilterForm.value.filterStatementDate.setHours(0, 0, 0));
          searchParameter.StatementEndDate = new Date(this.StatementSearchFilterForm.value.filterStatementDate.setHours(23, 59, 59));

          searchParameter.SortParameter.SortColumn = 'StatementDate';
        }
        if (this.StatementSearchFilterForm.value.filterStatementPeriod != null && this.StatementSearchFilterForm.value.filterStatementPeriod != '') {
          //searchParameter.EndDate = this.StatementSearchFilterForm.value.filterStatementPeriod;
          searchParameter.StatementPeriod = this.StatementSearchFilterForm.value.filterStatementPeriod.trim();

        }

        this.currentPage = 0;
        this.getStatementSearchs(searchParameter);
        this.isFilter = !this.isFilter;
      }
    }
  }

  resetSchdeuleLogFilterForm() {
    this.StatementSearchFilterForm.patchValue({
      filterStatementCustomer: [null],
      filterStatementAccountId: [null],
      filterStatementDate: [null],
      filterStatementPeriod: [null],
    });

    this.filterFromDateError = false;
    this.filterToDateError = false;
    this.filterFromDateErrorMessage = "";
    this.filterToDateErrorMessage = "";
  }

  ngAfterViewInit() {
    this.dataSource.paginator = this.paginator;

  }

  ViewHTML(element) {
    this.uiLoader.start();
    this.http.get(this.baseURL + 'StatementSearch/Download?identifier=' + element.Identifier, { responseType: "arraybuffer", observe: 'response' }).pipe(map(response => response))
      .subscribe(
        data => {
          this.uiLoader.stop();
          let contentType = data.headers.get('Content-Type');
          let fileName = data.headers.get('x-filename');
          fileName = fileName.substring(fileName.lastIndexOf('\\') + 1, fileName.length);
          const blob = new Blob([data.body], { type: contentType });
          if (window.navigator.msSaveOrOpenBlob) {
            window.navigator.msSaveOrOpenBlob(blob, fileName);
          } else {
            var link = document.createElement('a');
            link.setAttribute("type", "hidden");
            link.download = fileName;
            link.href = window.URL.createObjectURL(blob);
            document.body.appendChild(link);
            link.click();
          }
        },
        error => {
          $('.overlay').show();
          this._messageDialogService.openDialogBox('Error', "File Not Found", Constants.msgBoxError);
          this.uiLoader.stop();
        });
  }
}

function compare(a: number, b: number, isAsc: boolean) {
  return (a < b ? -1 : 1) * (isAsc ? 1 : -1);
}

function compareStr(a: string, b: string, isAsc: boolean) {
  return (a.toLowerCase() < b.toLowerCase() ? -1 : 1) * (isAsc ? 1 : -1);
}

function compareDate(a: Date, b: Date, isAsc: boolean) {
  return (Date.parse("" + a) < Date.parse("" + b) ? -1 : 1) * (isAsc ? 1 : -1);
}
