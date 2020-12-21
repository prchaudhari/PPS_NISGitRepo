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
import { ScheduleLogService } from './schedulelog.service';
import { ScheduleLog } from './schedulelog';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { ConfigConstants } from '../../shared/constants/configConstants';
import { map } from 'rxjs/operators';
import * as $ from 'jquery';
import { PreviewDialogService } from './../../shared/services/preview-dialog.service';
import { AppSettings } from '../../appsettings';

@Component({
  selector: 'app-logs',
  templateUrl: './logs.component.html',
  styleUrls: ['./logs.component.scss']
})

export class LogsComponent implements OnInit {

  //public variables
  public isFilter: boolean = false;
  public scheduleLogList: ScheduleLog[] = [];
  public isLoaderActive: boolean = false;
  public isRecordFound: boolean = false;
  public pageNo = 0;
  public pageSize = 5;
  public currentPage = 0;
  public totalSize = 0;
  public array: any;
  public isFilterDone = false;
  public sortedScheduleLogList: ScheduleLog[] = [];
  public pageTypeList: any[] = [];
  public ScheduleLogFilterForm: FormGroup;
  public filterPageTypeId: number = 0;
  public filterPageStatus: string = '';
  public filterFromDateError: boolean = false;
  public filterFromDateErrorMessage: string = "";
  public filterToDateError: boolean = false;
  public filterToDateErrorMessage: string = "";
  public userClaimsRolePrivilegeOperations: any[] = [];
  public baseURL = AppSettings.baseURL;

  public totalRecordCount = 0;
  public filterScheduleNameValue = '';
  public filterLogStatus = '';
  public filterExecutionDate = null;
  public sortColumn = 'Name';
  public sortOrder = Constants.Ascending;

  displayedColumns: string[] = ['schedule', 'batch', 'time', 'record', 'status', 'date', 'actions'];
  dataSource = new MatTableDataSource<any>();
  public DataFormat;

  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;

  constructor(private injector: Injector,
    private fb: FormBuilder,
    private uiLoader: NgxUiLoaderService,
    private _messageDialogService: MessageDialogService,
    private route: Router,
    private _http: HttpClient,
    private localstorageservice: LocalStorageService,
    private scheduleLogService: ScheduleLogService) {
    this.sortedScheduleLogList = this.scheduleLogList.slice();
    if (localStorage.getItem('logAddRouteparams')) {
      localStorage.removeItem('logAddRouteparams');
    }
    if (localStorage.getItem('logEditRouteparams')) {
      localStorage.removeItem('logEditRouteparams');
    }
  }

  public handlePage(e: any) {
    this.currentPage = e.pageIndex;
    this.pageSize = e.pageSize;
    //this.iterator();
    this.getScheduleLogs(null);
  }

  private iterator() {
    const end = (this.currentPage + 1) * this.pageSize;
    const start = this.currentPage * this.pageSize;
    const part = this.array.slice(start, end);
    this.dataSource = part;
    this.dataSource.sort = this.sort;
  }

  //Getters for Page Forms
  get filterScheduleName() {
    return this.ScheduleLogFilterForm.get('filterScheduleName');
  }

  get filterStatus() {
    return this.ScheduleLogFilterForm.get('filterStatus');
  }

  get filterPublishedOnFromDate() {
    return this.ScheduleLogFilterForm.get('filterPublishedOnFromDate');
  }

  closeFilter() {
    this.isFilter = !this.isFilter;
  }

  ngOnInit() {
    
    var userClaimsDetail = JSON.parse(localStorage.getItem('userClaims'));
    if (userClaimsDetail) {
      this.userClaimsRolePrivilegeOperations = userClaimsDetail.Privileges;
    }
    else {
      this.localstorageservice.removeLocalStorageData();
      this.route.navigate(['login']);
    }

    this.DataFormat = localStorage.getItem('DateFormat');
    this.getScheduleLogs(null);
    this.ScheduleLogFilterForm = this.fb.group({
      filterScheduleName: [null],
      filterStatus: [0],
      filterPublishedOnFromDate: [null],
    });

  }

  sortData(sort: MatSort) {
    const data = this.scheduleLogList.slice();
    if (!sort.active || sort.direction === '') {
      this.sortedScheduleLogList = data;
      return;
    }
   // ['schedule', 'time', 'record', 'status', 'date', 'actions'];
    if (sort.direction == 'asc') {
      this.sortOrder = Constants.Ascending;
    }
    else {
      this.sortOrder = Constants.Descending;
    }
    if (sort.active == 'schedule') {
      this.sortColumn = 'ScheduleName';
    }
    else if (sort.active == 'batch') {
      this.sortColumn = 'BatchName';
    }
    else if (sort.active == 'time') {
      this.sortColumn = 'ProcessingTime';
    }
    else if (sort.active == 'record') {
      this.sortColumn = 'RecordProccessed';
    }
    else if (sort.active == 'status') {
      this.sortColumn = 'Status';
    }
    else if (sort.active == 'date') {
      this.sortColumn = 'ExecutionDate';
    }
    let searchParameter: any = {};
    searchParameter.PagingParameter = {};
    searchParameter.PagingParameter.PageIndex = this.currentPage + 1;
    searchParameter.PagingParameter.PageSize = this.pageSize;
    searchParameter.SortParameter = {};
    searchParameter.SortParameter.SortColumn = this.sortColumn;
    searchParameter.SortParameter.SortOrder = this.sortOrder;
    searchParameter.SearchMode = Constants.Contains;
    if (this.ScheduleLogFilterForm.value.filterScheduleName != null && this.ScheduleLogFilterForm.value.filterScheduleName != '') {
      this.filterScheduleNameValue = this.ScheduleLogFilterForm.value.filterScheduleName.trim();
      searchParameter.ScheduleName = this.ScheduleLogFilterForm.value.filterScheduleName.trim();
    }
    if (this.ScheduleLogFilterForm.value.filterStatus != null && this.ScheduleLogFilterForm.value.filterStatus != 0) {
      this.filterLogStatus = this.ScheduleLogFilterForm.value.filterStatus;
      searchParameter.ScheduleStatus = this.ScheduleLogFilterForm.value.filterStatus;
    }
    if (this.ScheduleLogFilterForm.value.filterPublishedOnFromDate != null && this.ScheduleLogFilterForm.value.filterPublishedOnFromDate != '') {
      this.filterExecutionDate = this.ScheduleLogFilterForm.value.filterPublishedOnFromDate;
      searchParameter.StartDate = new Date(this.ScheduleLogFilterForm.value.filterPublishedOnFromDate.setHours(0, 0, 0));
    }
    this.getScheduleLogs(searchParameter);
  }

  async getScheduleLogs(searchParameter) {
    let scheduleLogService = this.injector.get(ScheduleLogService);
    if (searchParameter == null) {
      searchParameter = {};
      searchParameter.IsActive = true;
      searchParameter.PagingParameter = {};
      searchParameter.PagingParameter.PageIndex = this.currentPage + 1;
      searchParameter.PagingParameter.PageSize = this.pageSize;
      searchParameter.SortParameter = {};
      searchParameter.SortParameter.SortColumn = 'ExecutionDate';
      searchParameter.SortParameter.SortOrder = Constants.Descending;
      searchParameter.SearchMode = Constants.Contains;
      if (this.filterScheduleNameValue != null && this.filterScheduleNameValue != '') {
        searchParameter.ScheduleName = this.filterScheduleNameValue.trim();
      }
      if (this.filterLogStatus != null && this.filterLogStatus != '') {
        searchParameter.ScheduleStatus = this.filterLogStatus.trim();
      }
      if (this.filterExecutionDate != null && this.filterExecutionDate != '') {
        searchParameter.StartDate = new Date(this.filterExecutionDate.setHours(0, 0, 0));
      }
    }
    var response = await scheduleLogService.getScheduleLog(searchParameter);
    this.scheduleLogList = response.List;
    this.totalRecordCount = response.RecordCount;
    if (this.scheduleLogList.length == 0 && this.isFilterDone == true) {
      let message = ErrorMessageConstants.getNoRecordFoundMessage;
      this._messageDialogService.openDialogBox('Error', message, Constants.msgBoxError).subscribe(data => {
        if (data == true) {
          this.resetSchdeuleLogFilterForm();
          this.getScheduleLogs(null);
        }
      });
    }
    this.dataSource = new MatTableDataSource<ScheduleLog>(this.scheduleLogList);
    this.dataSource.sort = this.sort;
    this.array = this.scheduleLogList;
    this.totalSize = this.totalRecordCount;
    //this.iterator();
  }

  validateFilterDate(): boolean {
    if (this.ScheduleLogFilterForm.value.filterPublishedOnFromDate != null && this.ScheduleLogFilterForm.value.filterPublishedOnFromDate != '' &&
      this.ScheduleLogFilterForm.value.filterPublishedOnToDate != null && this.ScheduleLogFilterForm.value.filterPublishedOnToDate != '') {
      let startDate = this.ScheduleLogFilterForm.value.filterPublishedOnFromDate;
    }
    return true;
  }

  onPublishedFilterDateChange(event) {
    this.filterFromDateError = false;
    this.filterToDateError = false;
    this.filterFromDateErrorMessage = "";
    this.filterToDateErrorMessage = "";
    let currentDte = new Date();
    if (this.ScheduleLogFilterForm.value.filterPublishedOnFromDate != null && this.ScheduleLogFilterForm.value.filterPublishedOnFromDate != '') {
      let startDate = this.ScheduleLogFilterForm.value.filterPublishedOnFromDate;
      if (startDate.getTime() > currentDte.getTime()) {
        this.filterFromDateError = true;
        this.filterFromDateErrorMessage = ErrorMessageConstants.getStartDateLessThanCurrentDateMessage;
      }
    }
  }

  //This method has been used for fetching search records
  searchScheduleLogRecordFilter(searchType) {
    this.filterFromDateError = false;
    this.isFilterDone = true;
    if (searchType == 'reset') {
      this.resetSchdeuleLogFilterForm();
      this.getScheduleLogs(null);
      this.isFilter = !this.isFilter;
    }
    else {
      if (this.validateFilterDate()) {
        let searchParameter: any = {};
        searchParameter.PagingParameter = {};
        searchParameter.PagingParameter.PageIndex = 1;
        searchParameter.PagingParameter.PageSize = this.pageSize;
        searchParameter.SortParameter = {};
        searchParameter.SortParameter.SortColumn = 'ExecutionDate';
        searchParameter.SortParameter.SortOrder = Constants.Descending;
        searchParameter.SearchMode = Constants.Contains;

        if (this.ScheduleLogFilterForm.value.filterScheduleName != null && this.ScheduleLogFilterForm.value.filterScheduleName != '') {
          this.filterScheduleNameValue = this.ScheduleLogFilterForm.value.filterScheduleName.trim();
          searchParameter.ScheduleName = this.ScheduleLogFilterForm.value.filterScheduleName.trim();
        }
        if (this.ScheduleLogFilterForm.value.filterStatus != null && this.ScheduleLogFilterForm.value.filterStatus != 0) {
          this.filterLogStatus = this.ScheduleLogFilterForm.value.filterStatus;
          searchParameter.ScheduleStatus = this.ScheduleLogFilterForm.value.filterStatus;
        }
        if (this.ScheduleLogFilterForm.value.filterPublishedOnFromDate != null && this.ScheduleLogFilterForm.value.filterPublishedOnFromDate != '') {
          this.filterExecutionDate = this.ScheduleLogFilterForm.value.filterPublishedOnFromDate;
          searchParameter.StartDate = new Date(this.ScheduleLogFilterForm.value.filterPublishedOnFromDate.setHours(0, 0, 0));
        }

        this.currentPage = 0;
        this.getScheduleLogs(searchParameter);
        this.isFilter = !this.isFilter;
      }
    }
  }

  resetSchdeuleLogFilterForm() {
    this.ScheduleLogFilterForm.patchValue({
      filterScheduleName: null,
      filterOwner: null,
      filterPageType: 0,
      filterStatus: 0,
      filterPublishedOnFromDate: null,
    });

    this.currentPage = 0;
    this.filterScheduleNameValue = '';
    this.filterLogStatus = '';
    this.filterExecutionDate = null;
    this.filterFromDateError = false;
    this.filterToDateError = false;
    this.filterFromDateErrorMessage = "";
  }

  ngAfterViewInit() {
    this.dataSource.paginator = this.paginator;

  }

  navigationLogDetails(log: ScheduleLog) {
    let queryParams = {
      Routeparams: {
        passingparams: {
          "SchdeuleName": log.ScheduleName,
          "SchdeuleLogIdetifiier": log.Identifier,
          "ExecutionDate": log.CreateDate,
          "BatchStatus": log.BatchStatus
        }
      }
    }
    localStorage.setItem("scheduleLogParams", JSON.stringify(queryParams))
    this.route.navigate(['../logsDetails']);
  }

  //function written to retry to generate HTML statements for failed customer records
  reTryLog(log: ScheduleLog) {
    let message = 'Are you sure, you want to run this schedule?';
    this._messageDialogService.openConfirmationDialogBox('Confirm', message, Constants.msgBoxWarning).subscribe(async (isConfirmed) => {
      if (isConfirmed) {
        let isDeleted = await this.scheduleLogService.reRunSchdeulLog(log.Identifier);
        if (isDeleted) {
          let messageString = Constants.ScheduleReRunSuccessfullyMessage;
          this._messageDialogService.openDialogBox('Success', messageString, Constants.msgBoxSuccess);
          this.getScheduleLogs(null);
        }
      }
    });
  }

  DownloadErrorLog(log: ScheduleLog): void {
    this.uiLoader.start();
    this._http.get(this.baseURL + 'ScheduleLog/ScheduleLog/DownloadErrorLogs?ScheduleLogIndentifier=' + log.Identifier , { responseType: "arraybuffer", observe: 'response' }).pipe(map(response => response))
      .subscribe(
        data => {
          this.uiLoader.stop();
          let contentType = data.headers.get('Content-Type');
          let fileName = data.headers.get('x-filename');
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
          this._messageDialogService.openDialogBox('Error', error.error.Message, Constants.msgBoxError);
          this.uiLoader.stop();
        });
  }

  ViewErrorLog(log: ScheduleLog) {
    let previewservice = this.injector.get(PreviewDialogService);
    previewservice.openErrorLogDialogBox(log.Identifier, log.ScheduleName);
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
