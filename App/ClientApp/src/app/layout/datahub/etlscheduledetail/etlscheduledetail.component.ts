import { Component, OnInit, Injector, ViewChild } from '@angular/core';
import { NgxUiLoaderService } from 'ngx-ui-loader';
import { HttpClient } from '@angular/common/http';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MessageDialogService } from 'src/app/shared/services/mesage-dialog.service';
import { Location } from '@angular/common';
import * as $ from 'jquery';
import { Router, NavigationEnd } from '@angular/router';

import { AppSettings } from '../../../appsettings';
import { Constants, ErrorMessageConstants } from 'src/app/shared/constants/constants';
import { DataHubService } from '../datahub.service';
import { ETLSchedule, ETLScheduleBatch, ETLScheduleBatchLog } from '../datahub';

import { LocalStorageService } from 'src/app/shared/services/local-storage.service';
import { FormGroup, FormBuilder } from '@angular/forms';
import { DatePipe } from '@angular/common'
import * as XLSX from 'xlsx';

@Component({
  selector: 'app-etlscheduledetail',
  templateUrl: './etlscheduledetail.component.html',
  styleUrls: ['./etlscheduledetail.component.scss']
})

export class EtlscheduledetailComponent implements OnInit {
  public userClaimsRolePrivilegeOperations: any[] = [];

  dataSource = new MatTableDataSource<any>();
  public pageSize = 5;
  public currentPage = 0;
  public totalSize = 0;
  public array: any;
  displayedColumns: string[] = ['id', 'BatchName', 'IsExecuted', 'DataExtractionDate', 'BatchExecutionDate', 'BatchStatus', 'RecordProcess', 'Actions'];

  public eTLSchedule: ETLSchedule;
  public scheduleRecord: any;
  public params;
  public isCollapsedDetails: boolean = false;
  public isCollapsedBatch: boolean = true;
  public IsBatchDetailsGet = false;

  public RecurrencePattern = '';
  public RepeatEveryBy = '';
  public dayObjectArr = [{ Id: 1, 'Day': 'Monday' }, { Id: 2, 'Day': 'Tuesday' }, { Id: 3, 'Day': 'Wednesday' }, { Id: 4, 'Day': 'Thursday' }, { Id: 5, 'Day': 'Friday' }, { Id: 6, 'Day': 'Saturday' }, { Id: 7, 'Day': 'Sunday' }];
  public selectedWeekdays = [];
  public monthArray = ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December'];
  public ScheduleOccuranceMessage = '';
  public ScheduleforStatements = '';

  public ETLScheduleIdentifier = 0;
  public ETLScheduleName: string;
  public ETLScheduleBatchIdentifier = 0;
  public ETLScheduleBatchName: string;
  public ETLScheduleBatchExecutionDate = null;
  public batchLogDateFormat: string;

  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;

  navigateToListPage() {
    this._location.back();
  }

  constructor(private _location: Location, private injector: Injector, private _spinnerService: NgxUiLoaderService,
    private _http: HttpClient, private _messageDialogService: MessageDialogService, private uiLoader: NgxUiLoaderService,
    private _router: Router, private localstorageservice: LocalStorageService,
    private fb: FormBuilder, public datepipe: DatePipe) {
    this.eTLSchedule = new ETLSchedule;

    _router.events.subscribe(e => {
      if (e instanceof NavigationEnd) {
        if (e.url.includes('/etlscheduledetail')) {
          //set passing parameters to localstorage.
          this.params = JSON.parse(localStorage.getItem('eTLScheduleParams'));
          if (localStorage.getItem('eTLScheduleParams')) {
            this.eTLSchedule.ProductBatchId = this.params.Routeparams.passingparams.ProductBatchId;
          }
        }
        // else {
        //   localStorage.removeItem("eTLScheduleParams");
        // }
      }
    });

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
    this.getScheduleRecords();

    this.batchLogDateFormat = "dd/MM/yyyy h:mm a";
  }

  async getScheduleRecords() {
    let dataHubService = this.injector.get(DataHubService);
    let searchParameter: any = {};
    searchParameter.PagingParameter = {};
    searchParameter.PagingParameter.PageIndex = Constants.DefaultPageIndex;
    searchParameter.PagingParameter.PageSize = Constants.DefaultPageSize;
    searchParameter.SortParameter = {};
    searchParameter.SortParameter.SortColumn = Constants.Name;
    searchParameter.SortParameter.SortOrder = Constants.Ascending;
    searchParameter.SearchMode = Constants.Exact;
    searchParameter.ProductBatchId = this.eTLSchedule.ProductBatchId;
    var response = await dataHubService.getScheduleForDataHub(searchParameter);
    this.scheduleRecord = response.List[0];

    if (this.scheduleRecord != null && this.scheduleRecord.ProductBatches != null || this.scheduleRecord.ProductBatches.length > 0) {
      if (this.scheduleRecord.ProductBatches[0].RecurrancePattern == null || this.scheduleRecord.ProductBatches[0].RecurrancePattern == '') {
        this.RecurrencePattern = 'Repeat';
      }
      else {
        if (this.scheduleRecord.ProductBatches[0].RecurrancePattern.includes('Custom')) {
          let index = this.scheduleRecord.ProductBatches[0].RecurrancePattern.indexOf('-');
          this.RepeatEveryBy = this.scheduleRecord.ProductBatches[0].RecurrancePattern.substring(index + 1, this.scheduleRecord.ProductBatches[0].RecurrancePattern.length);
          this.RecurrencePattern = this.scheduleRecord.ProductBatches[0].RecurrancePattern.substring(0, index);
        }
        else {
          this.RecurrencePattern = this.scheduleRecord.ProductBatches[0].RecurrancePattern;
        }
      }
      this.RecurrencePattern = this.scheduleRecord.ProductBatches[0].RecurrancePattern;
    }

    if (this.scheduleRecord != null && this.scheduleRecord.ProductBatches != null || this.scheduleRecord.ProductBatches.length > 0) {
      if (this.scheduleRecord.ProductBatches[0].WeekDays != null && this.scheduleRecord.ProductBatches[0].WeekDays != '') {
        var scheduledays = this.scheduleRecord.ProductBatches[0].WeekDays.split(',');
        scheduledays.forEach(day => {
          var dayObj = this.dayObjectArr.filter(x => x.Day.toLocaleLowerCase() == day.toLocaleLowerCase())[0];
          this.selectedWeekdays.push({ 'Id': dayObj.Id, 'Day': dayObj.Day });
        });
      }
    }

    if (this.scheduleRecord != null && this.scheduleRecord.ProductBatches != null || this.scheduleRecord.ProductBatches.length > 0) {
      this.ScheduleforStatements = this.scheduleRecord.ProductBatches[0].ScheduleStatements;
    }

    this.setScheduleOccuranceMessage();
  }

  setScheduleOccuranceMessage() {
    if (this.scheduleRecord != null && this.scheduleRecord.ProductBatches != null || this.scheduleRecord.ProductBatches.length > 0) {
      var dt = new Date(this.scheduleRecord.ProductBatches[0].StartDate);
      var dayOfMonth = dt.getDate(); //this.schedule.DayOfMonth == undefined || this.schedule.DayOfMonth == 0 ? dt.getDate() : this.schedule.DayOfMonth;
      var ssd = new Date(dt.getFullYear(), dt.getMonth(), dayOfMonth);
      var schedulestartdte = ssd.toLocaleDateString();
      var dte = ssd.getDate();
      var month = this.monthArray[ssd.getMonth()];

      if (this.scheduleRecord.ProductBatches[0].RecurrancePattern == 'DoesNotRepeat') {
        this.ScheduleOccuranceMessage = 'Occurs once on ' + schedulestartdte;
      } else {
        let scheduleRunUtilMessage = '';
        if (this.scheduleRecord.ProductBatches[0].EndDate != null && this.scheduleRecord.ProductBatches[0].EndDate.toString() != "0001-01-01T00:00:00") {
          let sed = new Date(this.scheduleRecord.ProductBatches[0].EndDate);
          scheduleRunUtilMessage = ' until ' + sed.toLocaleDateString();
        } else if (this.scheduleRecord.ProductBatches[0].NoOfOccurrences != null) {
          scheduleRunUtilMessage = ' upto ' + this.scheduleRecord.ProductBatches[0].NoOfOccurrences + " occurence.";
        }

        let repeatEvery = this.scheduleRecord.ProductBatches[0].RepeatEveryDayMonWeekYear != null && this.scheduleRecord.ProductBatches[0].RepeatEveryDayMonWeekYear != 0 ? this.scheduleRecord.ProductBatches[0].RepeatEveryDayMonWeekYear : 1;
        let repeatEveryByVal = this.RepeatEveryBy != null && this.RepeatEveryBy != '' ? this.RepeatEveryBy : 'Month';
        let occurance = '';
        if (repeatEveryByVal == 'Day') {
          if (repeatEvery == 1) {
            occurance = 'day';
          } else {
            occurance = repeatEvery + ' days ';
          }
        }
        else if (repeatEveryByVal == 'Week') {
          var weekdaystr = '';
          if (this.selectedWeekdays.length > 0) {
            if (this.selectedWeekdays.length == 7 && repeatEvery == 1) {
              occurance = 'day';
            } else {
              this.selectedWeekdays.sort(function (a, b) {
                return a.Id - b.Id;
              });
              for (let i = 0; i < this.selectedWeekdays.length; i++) {
                let day = this.selectedWeekdays[i].Day;
                weekdaystr = weekdaystr + (weekdaystr != '' ? (i == (this.selectedWeekdays.length - 1) ? ' and ' : ', ') : '') + day;
              }
              if (repeatEvery == 1) {
                occurance = '' + (weekdaystr != '' ? 'on ' + weekdaystr : ' week');
              } else {
                occurance = repeatEvery + ' weeks ' + (weekdaystr != '' ? 'on ' + weekdaystr : '');
              }
            }
          }
        }
        else if (repeatEveryByVal == 'Month') {
          if (repeatEvery == 1) {
            occurance = 'month on day ' + dte;
          } else {
            occurance = repeatEvery + ' months on day ' + dte;
          }
        }
        else if (repeatEveryByVal == 'Year') {
          if (repeatEvery == 1) {
            occurance = 'year on day ' + dte + ' of ' + month;
          } else {
            occurance = repeatEvery + ' years on day ' + dte + ' of ' + month;
          }
        }
        this.ScheduleOccuranceMessage = 'On every ' + occurance + ' starting ' + schedulestartdte + scheduleRunUtilMessage;
      }
    }
  }


  public batchmasterList: any = [];
  public baseURL: string = AppSettings.baseURL;

  async getBatchMaster() {
    this.isCollapsedBatch = !this.isCollapsedBatch;
    if (this.IsBatchDetailsGet == false) {
      this._spinnerService.start();
      let identifier: number = 0;
      if (this.scheduleRecord.ProductBatches != null && this.scheduleRecord.ProductBatches.length > 0) {
        identifier = this.scheduleRecord.ProductBatches[0].ProductBatchId;
      }

      this._http.post(this.baseURL + 'DataHub/GetETLBatches?productBatchId=' + identifier, null).subscribe(
        data => {
          this.IsBatchDetailsGet = true;
          this.batchmasterList = data;
          for (var i = 0; i < this.batchmasterList.length; i++) {
            this.batchmasterList[i].Index = i + 1;
          }

          this.dataSource = new MatTableDataSource<ETLScheduleBatch>(this.batchmasterList);
          this.dataSource.sort = this.sort;
          this.array = this.batchmasterList;
          this.totalSize = this.array.length;
          this.iterator();
          this._spinnerService.stop();
        },
        error => {
          $('.overlay').show();
          this._messageDialogService.openDialogBox('Error', error.error.Message, Constants.msgBoxError);
          this._spinnerService.stop();
        });
    }
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

  public sortedscheduleList: any = [];
  sortData(sort: MatSort) {
    const data = this.batchmasterList.slice();
    if (!sort.active || sort.direction === '') {
      this.sortedscheduleList = data;
      return;
    }

    this.sortedscheduleList = data.sort((a, b) => {
      const isAsc = sort.direction === 'asc';
      switch (sort.active) {
        case 'id': return compare(a.Id, b.Id, isAsc);
        case 'BatchName': return compareStr(a.BatchName, b.BatchName, isAsc);
        case 'BatchStatus': return compare(a.Status, b.Status, isAsc);
        case 'IsExecuted': return compare(a.IsExecuted, b.IsExecuted, isAsc);
        case 'DataExtractionDate': return compareDate(a.DataExtractionDate, b.DataExtractionDate, isAsc);
        case 'BatchExecutionDate': return compareDate(a.BatchExecutionDate, b.BatchExecutionDate, isAsc);
        default: return compare(a.Id, b.Id, isAsc);
      }
    });
    this.dataSource = new MatTableDataSource<ETLScheduleBatch>(this.sortedscheduleList);
    this.dataSource.sort = this.sort;
    this.array = this.sortedscheduleList;
    this.totalSize = this.array.length;
    this.iterator();
  }

  removeElementsByClass(className) {
    var elements = document.getElementsByClassName(className);
    while (elements.length > 0) {
      elements[0].parentNode.removeChild(elements[0]);
    }
  }

  async runETL(etlScheduleBatch: any, elem: HTMLElement) {
    let message = 'Do you want to manual run execution of ' + etlScheduleBatch.BatchName + '?';
    this._messageDialogService.openConfirmationDialogBox('Confirm', message, Constants.msgBoxWarning).subscribe(async (isConfirmed) => {
      if (isConfirmed) {
        let isRecordSaved: boolean = false;
        let dataHubService = this.injector.get(DataHubService);
        let id: number = 0;
        id = etlScheduleBatch.Identifier;
        isRecordSaved = await dataHubService.runETL(id);
        this.IsBatchDetailsGet = false;
        this.isCollapsedBatch = !this.isCollapsedBatch;
        this.getBatchMaster();
      }
    });
  }

  public refreshBatches() {
    this.IsBatchDetailsGet = false;
    this.isCollapsedBatch = true;
    this.getBatchMaster();
  }

  async retryETLScheduleBatchExecution(eTLScheduleBatchIdentifier: Number, eTLScheduleBatchName: string) {
    let message = 'Do you want to retry execution of ' + eTLScheduleBatchName + '?';
    this._messageDialogService.openConfirmationDialogBox('Confirm', message, Constants.msgBoxWarning).subscribe(async (isConfirmed) => {
      if (isConfirmed) {
        let dataHubService = this.injector.get(DataHubService);
        let isRetry = await dataHubService.retryETLScheduleBatchExecution(eTLScheduleBatchIdentifier);
        if (isRetry) {
          let messageString = Constants.recordRetryMessage;
          this._messageDialogService.openDialogBox('Success', messageString, Constants.msgBoxSuccess);
          this.refreshBatches();
        }
      }
    });
  }

  async deleteETLScheduleBatchConfirmationPopUp(eTLScheduleBatchIdentifier: Number, eTLScheduleBatchName: string) {
    let message = "Do you want to delete " + eTLScheduleBatchName + "?";
    this._messageDialogService.openConfirmationDialogBox('Confirm', message, Constants.msgBoxWarning)
      .subscribe(async (isConfirmed) => {
        if (isConfirmed == true) {
          let dataHubService = this.injector.get(DataHubService);
          let isDeleted = await dataHubService.deleteETLScheduleBatch(eTLScheduleBatchIdentifier);
          if (isDeleted) {
            let messageString = Constants.recordDeletedMessage;
            this._messageDialogService.openDialogBox('Success', messageString, Constants.msgBoxSuccess);
            this.refreshBatches();
          }
        }
      });
  }

  async approvedETLScheduleBatchConfirmationPopUp(eTLScheduleBatchIdentifier: Number, eTLScheduleBatchName: string) {
    let message = "Do you want to approve " + eTLScheduleBatchName + "?";
    this._messageDialogService.openConfirmationDialogBox('Confirm', message, Constants.msgBoxWarning)
      .subscribe(async (isConfirmed) => {
        if (isConfirmed == true) {
          let dataHubService = this.injector.get(DataHubService);
          let isApproved = await dataHubService.approvedETLScheduleBatch(eTLScheduleBatchIdentifier);
          if (isApproved) {
            let messageString = Constants.recordApprovedMessage;
            this._messageDialogService.openDialogBox('Success', messageString, Constants.msgBoxSuccess);
            this.refreshBatches();
          }
        }
      });
  }

  //ETL Schedule Batch Log - Start
  showListETLScheduleBatchLog(eTLScheduleBatch: ETLScheduleBatch) {    
    this.ETLScheduleName = eTLScheduleBatch.ETLScheduleName;
    this.ETLScheduleBatchName = eTLScheduleBatch.BatchName;
    this.ETLScheduleBatchIdentifier = eTLScheduleBatch.Identifier;
    this.ETLScheduleBatchExecutionDate = eTLScheduleBatch.BatchExecutionDate;
    this.getETLSchedulebatchLogs(null);
  }

  public batchLogPageSize = 5;
  public batchLogCurrentPage = 0;
  public batchLogTotalSize = 0;
  public StartIndex = 0;
  public EndIndex = 0;

  public etlScheduleBatchLogList: ETLScheduleBatchLog[] = [];
  public batchLogDataSource: any;
  batchLogDisplayedColumns: string[] = ['sequenceNo', 'processingTime', 'status', 'executionDate', 'message', 'actions'];

  @ViewChild(MatSort, { static: true }) batchLogSort: MatSort;
  @ViewChild(MatPaginator, { static: true }) batchLogPaginator: MatPaginator;

  public batchLogHandlePage(e: any) {    
    this.batchLogCurrentPage = e.pageIndex;
    this.batchLogPageSize = e.pageSize;
    this.StartIndex = this.batchLogCurrentPage * this.batchLogPageSize;
    this.EndIndex = this.StartIndex + this.batchLogPageSize;

    this.batchLogDataSource = new MatTableDataSource<ETLScheduleBatchLog>(this.etlScheduleBatchLogList.slice(this.StartIndex, this.EndIndex));
  }

  //This method has been used for fetching Data Hub ETL-Schedules records
  async getETLSchedulebatchLogs(searchParameter) {    
    searchParameter = {};
    let dataHubService = this.injector.get(DataHubService);
    searchParameter.ETLBatchId = this.ETLScheduleBatchIdentifier;
    var response = await dataHubService.getETLScheduleBatchLogs(searchParameter);
    this.etlScheduleBatchLogList = response.List;

    for (var i = 0; i < this.etlScheduleBatchLogList.length; i++) {
      this.etlScheduleBatchLogList[i].SequenceNo = i + 1;
    }

    if (this.etlScheduleBatchLogList.length == 0) {
      let message = ErrorMessageConstants.getNoRecordFoundMessage;
      this._messageDialogService.openDialogBox('Error', message, Constants.msgBoxError).subscribe(data => {
        if (data == true) {
          //this.getETLSchedulebatchLogs(null);
        }
      });
    }

    this.StartIndex = this.batchLogCurrentPage * this.batchLogPageSize;
    this.EndIndex = this.StartIndex + this.batchLogPageSize;

    this.batchLogDataSource = new MatTableDataSource<ETLScheduleBatchLog>(this.etlScheduleBatchLogList.sort((a, b) => {
      return compare(a.SequenceNo, b.SequenceNo, true);
    }).slice(this.StartIndex, this.EndIndex));

    this.batchLogDataSource = new MatTableDataSource<ETLScheduleBatchLog>(this.etlScheduleBatchLogList);
    this.batchLogDataSource.sort = this.batchLogSort;
    this.batchLogDataSource.paginator = this.batchLogPaginator;
    this.batchLogTotalSize = response.RecordCount;
    //this.iterator();
  }

  //To sort table data using column
  batchLogSortData(batchLogSort: MatSort) {
    
    if (!batchLogSort.active || batchLogSort.direction === '') {
      this.batchLogDataSource = new MatTableDataSource<ETLScheduleBatchLog>(this.etlScheduleBatchLogList.slice(this.StartIndex, this.EndIndex));
      return;
    }

    this.batchLogDataSource = new MatTableDataSource<ETLScheduleBatchLog>(this.etlScheduleBatchLogList.sort((a, b) => {
      const isAsc = batchLogSort.direction === 'asc';
      switch (batchLogSort.active) {
        case 'sequenceNo':
          return compare(a.SequenceNo, b.SequenceNo, isAsc);
        case 'processingTime':
          return compare(a.ProcessingTime, b.ProcessingTime, isAsc);
        case 'status':
          return compare(a.Status, b.Status, isAsc);
        case 'executionDate':
          return compareDate(a.ExecutionDate, b.ExecutionDate, isAsc);
        case 'message':
          return compare(a.LogMessage, b.LogMessage, isAsc);
        default:
          return compare(a.SequenceNo, b.SequenceNo, isAsc);
      }
    }).slice(this.StartIndex, this.EndIndex));
  }

  async ExportTOExcel(etlschedulebatchlog: ETLScheduleBatchLog) {
    let latest_date;
    if (etlschedulebatchlog.ExecutionDate != null && etlschedulebatchlog.ExecutionDate.toString() != '0001-01-01T00:00:00') {
      latest_date = this.datepipe.transform(etlschedulebatchlog.ExecutionDate, this.batchLogDateFormat);
    }
    else {
      latest_date = 'NA';
    }

    //var logItemList = await this.getETLSchedulebatchLogDetailsForExport(null, etlschedulebatchlog.Identifier);

    let dataOfETLScheduleBatchLog: any = [];
    let tempjsonList: any;
    tempjsonList = {
      ETLSchedule: etlschedulebatchlog.ETLSchedule,
      Batch: etlschedulebatchlog.Batch,
      ProcessingTime: etlschedulebatchlog.ProcessingTime.replace(":", " Hr ").replace(":", " Min ") + " Sec",
      Status: etlschedulebatchlog.Status,
      ExecutionDate: latest_date
    }
    dataOfETLScheduleBatchLog.push(tempjsonList);
    // for (let i = 0; i < logItemList.length; i++) {
    //   if (i == 0) {
    //     tempjsonList = {
    //       ETLSchedule: etlschedulebatchlog.ETLSchedule,
    //       Batch: etlschedulebatchlog.Batch,
    //       ProcessingTime: etlschedulebatchlog.ProcessingTime.replace(":", " Hr ").replace(":", " Min ") + " Sec",
    //       Status: etlschedulebatchlog.Status,
    //       ExecutionDate: latest_date,
    //       RecordNo: logItemList[i].Identifier,
    //       Segment: logItemList[i].Segment,
    //       Language: logItemList[i].Language,
    //       ReferenceRecordId: logItemList[i].ReferenceRecordId,
    //       LogItemStatus: logItemList[i].Status,
    //       LogMessage: logItemList[i].LogMessage,
    //     }
    //   }
    //   else {
    //     tempjsonList = {
    //       ETLSchedule: '',
    //       Batch: '',
    //       ProcessingTime: '',
    //       Status: '',
    //       ExecutionDate: '',
    //       RecordNo: logItemList[i].Identifier,
    //       Segment: logItemList[i].Segment,
    //       Language: logItemList[i].Language,
    //       ReferenceRecordId: logItemList[i].ReferenceRecordId,
    //       LogItemStatus: logItemList[i].Status,
    //       LogMessage: logItemList[i].LogMessage,
    //     }
    //   }
    //   dataOfETLScheduleBatchLog.push(tempjsonList);
    // }
    //const ws: XLSX.WorkSheet = XLSX.utils.json_to_sheet(tempjsonList);

    if(dataOfETLScheduleBatchLog.length != 0){
      const ws: XLSX.WorkSheet = XLSX.utils.json_to_sheet(dataOfETLScheduleBatchLog);
      const wb: XLSX.WorkBook = XLSX.utils.book_new();
      XLSX.utils.book_append_sheet(wb, ws, 'BatchLog');
  
      /* save to file */
      XLSX.writeFile(wb, 'ETLScheduleBatchLog.xlsx');
    }
    else{
      this._messageDialogService.openDialogBox('Success', "ETLLog has no data for this log.", Constants.msgBoxSuccess);
    }
  }

  closeListETLScheduleBatchLog() {
    this.ETLScheduleBatchIdentifier = 0;
  }

  //This method has been used for fetching Data Hub ETL-Schedules records
  async getETLSchedulebatchLogDetailsForExport(searchParameter, eTLScheduleBatchLogIdentifier: Number) {
    let dataHubService = this.injector.get(DataHubService);
    if (searchParameter == null) {
      searchParameter = {};
      searchParameter.IsActive = true;
      searchParameter.PagingParameter = {};
      searchParameter.PagingParameter.PageIndex = this.currentPage + 1;
      searchParameter.PagingParameter.PageSize = this.pageSize;
      searchParameter.SortParameter = {};
      searchParameter.SortParameter.SortColumn = 'Id';
      searchParameter.SortParameter.SortOrder = Constants.Ascending;
      searchParameter.SearchMode = Constants.Contains;
    }

    searchParameter.EtlLogId = eTLScheduleBatchLogIdentifier;

    var response = await dataHubService.getETLScheduleBatchLogDetails(searchParameter);
    return response.List;
    //this.iterator();
  }

  //this method helps to navigate to  ETL Schedule batch log Details List Page
  navigateToListETLScheduleBatchLogDetails(etlschedulebatchlog: ETLScheduleBatchLog) {
    let queryParams = {
      Routeparams: {
        passingparams: {
          "ETLScheduleBatchLogIdentifier": etlschedulebatchlog.Identifier,
          "ETLScheduleName": etlschedulebatchlog.ETLSchedule,
          "ETLScheduleBatchExecutionDate": etlschedulebatchlog.ExecutionDate,
        },
        // filteredparams: {
        //   //passing data using json stringify.
        //   "ScheduleName": this.ScheduleFilterForm.value.filterRoleName != null ? this.ScheduleFilterForm.value.filterRoleName : ""
        // }
      }
    }
    localStorage.setItem("eTLScheduleBatchLogParams", JSON.stringify(queryParams))

    this.removeElementsByClass("modal-backdrop fade show");
    document.body.className = document.body.className.replace("modal-open", "");

    const router = this.injector.get(Router);
    router.navigate(['datahub', 'list-etlschedulebatchlogdetails']);
    //router.navigate([]).then(result => { window.open('/datahub/list-etlschedulebatchlogdetails', '_blank'); });    

  }
  //ETL Schedule Batch Log - End
}

function compare(a: number | string, b: number | string, isAsc: boolean) {
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
