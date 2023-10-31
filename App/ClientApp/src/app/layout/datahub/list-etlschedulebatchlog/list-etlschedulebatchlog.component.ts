import { Component, OnInit, Injector, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';
import { Constants, ErrorMessageConstants } from 'src/app/shared/constants/constants';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { LocalStorageService } from 'src/app/shared/services/local-storage.service';
import { MessageDialogService } from 'src/app/shared/services/mesage-dialog.service';
import { Router, NavigationEnd } from '@angular/router';
import { DatePipe } from '@angular/common'
import * as XLSX from 'xlsx';

import { DataHubService } from '../datahub.service';
import { ETLScheduleBatchLog } from '../datahub';

@Component({
  selector: 'app-list-etlschedulebatchlog',
  templateUrl: './list-etlschedulebatchlog.component.html',
  styleUrls: ['./list-etlschedulebatchlog.component.scss']
})

export class ListEtlschedulebatchlogComponent implements OnInit {
  public isFilter: boolean = false;
  public pageNo = 0;
  public pageSize = 5;
  public currentPage = 0;
  public totalSize = 0;
  public previousPageLabel: string;
  public isFilterDone = false;
  public totalRecordCount = 0;
  public sortOrder = Constants.Descending;
  public sortColumn = 'ETLSchedule';

  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;

  public etlScheduleBatchLogList: ETLScheduleBatchLog[] = [];
  public sortedEtlScheduleBatchLogList: ETLScheduleBatchLog[] = [];
  public ETLScheduleBatchLogFilterForm: FormGroup;
  public dataSource: any;
  displayedColumns: string[] = ['eTLSchedule', 'batch', 'processingTime', 'status', 'executionDate', 'actions'];
  public userClaimsRolePrivilegeOperations: any[] = [];
  public array: any;

  public filterETLScheduleValue = '';
  public filterBatchValue = '';
  public filterProcessingTimeValue = '';
  public filterStatusValue = '';
  public filterExecutionDateValue = null;

  public DateFormat: string;
  public params;
  public ETLScheduleBatchIdentifier = 0;
  public ETLScheduleBatchName = null;
  public ETLScheduleBatchExecutionDate = null;

  public tempExecutionDate : String;

  closeFilter() {
    this.isFilter = !this.isFilter;
  }

  constructor(private injector: Injector, private _messageDialogService: MessageDialogService,
    private router: Router, private localstorageservice: LocalStorageService,
    private fb: FormBuilder, private dataHubService: DataHubService,
    public datepipe: DatePipe) {
    this.sortedEtlScheduleBatchLogList = this.etlScheduleBatchLogList.slice();

    router.events.subscribe(e => {
      if (e instanceof NavigationEnd) {
        if (e.url.includes('/list-etlschedulebatchlog')) {
          //set passing parameters to localstorage.
          this.params = JSON.parse(localStorage.getItem('eTLScheduleBatchParams'));
          if (localStorage.getItem('eTLScheduleBatchParams')) {
            this.ETLScheduleBatchIdentifier = this.params.Routeparams.passingparams.ETLScheduleBatchIdentifier;
            this.ETLScheduleBatchName = this.params.Routeparams.passingparams.ETLScheduleBatchName;
            this.ETLScheduleBatchExecutionDate = this.params.Routeparams.passingparams.ETLScheduleBatchExecutionDate;
          }
        }
        //else {
        //   localStorage.removeItem("eTLScheduleBatchParams");
        // }
      }
    });
  }

  //method called on initialization
  ngOnInit() {
    this.DateFormat = "dd/MM/yyyy h:mm a" //localStorage.getItem('DateFormat');

    var userClaimsDetail = JSON.parse(localStorage.getItem('userClaims'));
    if (userClaimsDetail) {
      this.userClaimsRolePrivilegeOperations = userClaimsDetail.Privileges;
    }
    else {
      this.localstorageservice.removeLocalStorageData();
      this.router.navigate(['login']);
    }

    this.getETLSchedulebatchLogs(null);
    this.ETLScheduleBatchLogFilterForm = this.fb.group({
      filterETLSchedule: [null],
      filterBatch: [null],
      filterProcessingTime: [null],
      filterStatus: [null],
      filterExecutionDate: [null]
    });
  }

  public handlePage(e: any) {
    this.currentPage = e.pageIndex;
    this.pageSize = e.pageSize;
    //this.iterator();
    this.getETLSchedulebatchLogs(null);
  }

  //To sort table data using column
  sortData(sort: MatSort) {
    const data = this.etlScheduleBatchLogList.slice();
    if (!sort.active || sort.direction === '') {
      this.sortedEtlScheduleBatchLogList = data;
      return;
    }

    if (sort.direction == 'asc') {
      this.sortOrder = Constants.Ascending;
    } else {
      this.sortOrder = Constants.Descending;
    }

    switch (sort.active) {
      case 'eTLSchedule': this.sortColumn = "ETLSchedule"; break;
      case 'batch': this.sortColumn = "Batch"; break;
      case 'processingTime': this.sortColumn = "ProcessingTime"; break;
      case 'status': this.sortColumn = "Status"; break;
      case 'executionDate': this.sortColumn = "ExecutionDate"; break;
      default: this.sortColumn = "LastUpdatedDate"; break;
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

    this.filterETLScheduleValue = (this.ETLScheduleBatchLogFilterForm.value.filterETLSchedule != null && this.ETLScheduleBatchLogFilterForm.value.filterETLSchedule != '') ? this.ETLScheduleBatchLogFilterForm.value.filterETLSchedule.trim() : "";
    this.filterBatchValue = (this.ETLScheduleBatchLogFilterForm.value.filterBatch != null && this.ETLScheduleBatchLogFilterForm.value.filterBatch != '') ? this.ETLScheduleBatchLogFilterForm.value.filterBatch.trim() : "";
    this.filterProcessingTimeValue = (this.ETLScheduleBatchLogFilterForm.value.filterProcessingTime != null && this.ETLScheduleBatchLogFilterForm.value.filterProcessingTime != '') ? this.ETLScheduleBatchLogFilterForm.value.filterProcessingTime.trim() : "";
    this.filterStatusValue = (this.ETLScheduleBatchLogFilterForm.value.filterStatus != null && this.ETLScheduleBatchLogFilterForm.value.filterStatus != '') ? this.ETLScheduleBatchLogFilterForm.value.filterStatus.trim() : "";
    this.filterExecutionDateValue = (this.ETLScheduleBatchLogFilterForm.value.filterExecutionDate != null && this.ETLScheduleBatchLogFilterForm.value.filterExecutionDate != '') ? new Date(this.ETLScheduleBatchLogFilterForm.value.filterExecutionDate.setHours(23, 59, 59)) : null;

    searchParameter.ETLSchedule = this.filterETLScheduleValue
    searchParameter.Batch = this.filterBatchValue;
    searchParameter.ProcessingTime = this.filterProcessingTimeValue;
    searchParameter.Status = this.filterStatusValue;
    searchParameter.ExecutionDate = this.filterExecutionDateValue;

    this.getETLSchedulebatchLogs(searchParameter);
  }

  //This method has been used for fetching search records
  searchFilter(searchType) {
    this.isFilterDone = true;
    if (searchType == 'reset') {
      this.resetETLScheduleBatchLogFilterForm();
      this.getETLSchedulebatchLogs(null);
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

      this.filterETLScheduleValue = (this.ETLScheduleBatchLogFilterForm.value.filterETLSchedule != null && this.ETLScheduleBatchLogFilterForm.value.filterETLSchedule != '') ? this.ETLScheduleBatchLogFilterForm.value.filterETLSchedule.trim() : "";
      this.filterBatchValue = (this.ETLScheduleBatchLogFilterForm.value.filterBatch != null && this.ETLScheduleBatchLogFilterForm.value.filterBatch != '') ? this.ETLScheduleBatchLogFilterForm.value.filterBatch.trim() : "";
      this.filterProcessingTimeValue = (this.ETLScheduleBatchLogFilterForm.value.filterProcessingTime != null && this.ETLScheduleBatchLogFilterForm.value.filterProcessingTime != '') ? this.ETLScheduleBatchLogFilterForm.value.filterProcessingTime.trim() : "";
      this.filterStatusValue = (this.ETLScheduleBatchLogFilterForm.value.filterStatus != null && this.ETLScheduleBatchLogFilterForm.value.filterStatus != '') ? this.ETLScheduleBatchLogFilterForm.value.filterStatus.trim() : "";
      this.filterExecutionDateValue = (this.ETLScheduleBatchLogFilterForm.value.filterExecutionDate != null && this.ETLScheduleBatchLogFilterForm.value.filterExecutionDate != '') ? new Date(this.ETLScheduleBatchLogFilterForm.value.filterExecutionDate.setHours(23, 59, 59)) : null;

      searchParameter.ETLSchedule = this.filterETLScheduleValue
      searchParameter.Batch = this.filterBatchValue;
      searchParameter.ProcessingTime = this.filterProcessingTimeValue;
      searchParameter.Status = this.filterStatusValue;
      searchParameter.ExecutionDate = this.filterExecutionDateValue;

      this.currentPage = 0;
      this.getETLSchedulebatchLogs(searchParameter);
      this.isFilter = !this.isFilter;
    }
  }

  //This method has been used for fetching Data Hub ETL-Schedules records
  async getETLSchedulebatchLogs(searchParameter) {
    let dataHubService = this.injector.get(DataHubService);
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

    if (this.filterETLScheduleValue != null && this.filterETLScheduleValue != '') {
      searchParameter.ETLSchedule = this.filterETLScheduleValue.trim();
    }
    if (this.filterBatchValue != null && this.filterBatchValue != '') {
      searchParameter.Batch = this.filterBatchValue.trim();
    }
    if (this.filterProcessingTimeValue != null && this.filterProcessingTimeValue != '') {
      searchParameter.ProcessingTime = this.filterProcessingTimeValue.trim();
    }
    if (this.filterStatusValue != null && this.filterStatusValue != '') {
      searchParameter.Status = this.filterStatusValue.trim();
    }
    if (this.filterExecutionDateValue != null && this.filterExecutionDateValue != '') {
      searchParameter.ExecutionDate = new Date(this.filterExecutionDateValue.setHours(23, 59, 59));
    }

    searchParameter.ETLBatchId = this.ETLScheduleBatchIdentifier;

    var response = await dataHubService.getETLScheduleBatchLogs(searchParameter);
    this.etlScheduleBatchLogList = response.List;
    this.totalRecordCount = response.RecordCount;
    if (this.etlScheduleBatchLogList.length == 0 && this.isFilterDone == true) {
      let message = ErrorMessageConstants.getNoRecordFoundMessage;
      this._messageDialogService.openDialogBox('Error', message, Constants.msgBoxError).subscribe(data => {
        if (data == true) {
          this.resetETLScheduleBatchLogFilterForm();
          this.getETLSchedulebatchLogs(null);
        }
      });
    }
    this.dataSource = new MatTableDataSource<ETLScheduleBatchLog>(this.etlScheduleBatchLogList);
    this.dataSource.sort = this.sort;
    this.array = this.etlScheduleBatchLogList;
    this.totalSize = this.totalRecordCount;
    //this.iterator();
  }

  //To reset field of ETL Schedule filter form
  resetETLScheduleBatchLogFilterForm() {
    this.ETLScheduleBatchLogFilterForm.patchValue({
      filterETLSchedule: null,
      filterBatch: null,
      filterProcessingTime: null,
      filterStatus: null,
      filterExecutionDate: null
    });
    this.currentPage = 0;
    this.filterETLScheduleValue = '';
    this.filterBatchValue = '';
    this.filterProcessingTimeValue = '';
    this.filterStatusValue = '';
    this.filterExecutionDateValue = null;
  }

  //Getters for ETL Schedule Batch Log Forms
  get filterETLSchedule() {
    return this.ETLScheduleBatchLogFilterForm.get('filterETLSchedule');
  }

  //Getters for ETL Schedule Batch Log Forms
  get filterBatch() {
    return this.ETLScheduleBatchLogFilterForm.get('filterBatch');
  }

  //Getters for ETL Schedule Batch Log Forms
  get filterProcessingTime() {
    return this.ETLScheduleBatchLogFilterForm.get('filterProcessingTime');
  }

  //Getters for ETL Schedule Batch Log Forms
  get filterStatus() {
    return this.ETLScheduleBatchLogFilterForm.get('filterStatus');
  }

  //Getters for ETL Schedule Batch Log Forms
  get filterExecutionDate() {
    return this.ETLScheduleBatchLogFilterForm.get('filterExecutionDate');
  }

  //this method helps to navigate to  ETL Schedule batch log Details List Page
  navigateToListETLScheduleBatchLogDetails(etlschedulebatchlog: ETLScheduleBatchLog) {
    let queryParams = {
      Routeparams: {
        passingparams: {
          "ETLScheduleBatchLogIdentifier": etlschedulebatchlog.Identifier,
          "ETLScheduleBatchExecutionDate": etlschedulebatchlog.ExecutionDate,
        },
        // filteredparams: {
        //   //passing data using json stringify.
        //   "ScheduleName": this.ScheduleFilterForm.value.filterRoleName != null ? this.ScheduleFilterForm.value.filterRoleName : ""
        // }
      }
    }
    localStorage.setItem("eTLScheduleBatchLogParams", JSON.stringify(queryParams))

    const router = this.injector.get(Router);
    router.navigate(['datahub', 'list-etlschedulebatchlogdetails']);
  }

  ExportTOExcel(etlschedulebatchlog: ETLScheduleBatchLog) {

    let latest_date;
    if(etlschedulebatchlog.ExecutionDate != null && etlschedulebatchlog.ExecutionDate.toString() != '0001-01-01T00:00:00'){
      latest_date = this.datepipe.transform(etlschedulebatchlog.ExecutionDate, this.DateFormat);
    }
    else{
      latest_date = 'NA';
    }

    let dataOfETLScheduleBatchLog: any = 
    [{
      ETLSchedule: etlschedulebatchlog.ETLSchedule,
      Batch: etlschedulebatchlog.Batch,
      ProcessingTime: etlschedulebatchlog.ProcessingTime.replace(":"," Hr").replace(":"," Min ") + " Sec",
      Status: etlschedulebatchlog.Status,
      ExecutionDate: latest_date
    }];

    const ws: XLSX.WorkSheet = XLSX.utils.json_to_sheet(dataOfETLScheduleBatchLog);
    const wb: XLSX.WorkBook = XLSX.utils.book_new();
    XLSX.utils.book_append_sheet(wb, ws, 'BatchLog');

    /* save to file */
    XLSX.writeFile(wb, 'ETLScheduleBatchLog.xlsx');

  }
}
