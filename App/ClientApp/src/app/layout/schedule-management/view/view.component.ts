import { AppSettings } from '../../../appsettings';
import { Component, OnInit, Injector, ChangeDetectorRef, ViewChild, OnDestroy } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import * as $ from 'jquery';
import { Router, ActivatedRoute, NavigationEnd } from '@angular/router';
import { Schedule } from '../schedule';
import { NgxUiLoaderService } from 'ngx-ui-loader';
import { Location } from '@angular/common';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { Constants } from 'src/app/shared/constants/constants';
import { MessageDialogService } from 'src/app/shared/services/mesage-dialog.service';
import { FormBuilder } from '@angular/forms';
import { ScheduleService } from '../schedule.service';
import { ConfigConstants } from '../../../shared/constants/configConstants';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

@Component({
  selector: 'app-view',
  templateUrl: './view.component.html',
  styleUrls: ['./view.component.scss']
})

export class ViewComponent implements OnInit {
  public schedule: Schedule;
  public scheduleRecord: any;
  public params;
  public userClaimsRolePrivilegeOperations: any[] = [];
  public isCollapsedDetails: boolean = false;
  public isCollapsedBatch: boolean = true;
  public IsBatchDetailsGet = false;
  displayedColumns: string[] = ['id', 'BatchName', 'IsExecuted', 'IsDataReady', 'DataExtractionDate', 'BatchExecutionDate', 'BatchStatus', 'Actions'];
  dataSource = new MatTableDataSource<any>();
  public pageSize = 5;
  public currentPage = 0;
  public totalSize = 0;
  public array: any;
  
  public isDaily: boolean = false;
  public isWeekly: boolean = true;
  public isMonthly: boolean = false;
  public isYearly: boolean = false;
  public isEndDate: boolean = true;
  public isEndAfter: boolean = false;
  public isNoEndDate: boolean = false;
  public RecurrencePattern = '';
  public ScheduleOccuranceMessage = '';
  public monthArray = ['January','February','March','April','May','June','July','August','September','October','November','December'];
  public dayObjectArr = [{Id: 1, 'Day':'Monday'},{Id:2,'Day':'Tuesday'},{Id:3, 'Day':'Wednesday'},{Id:4,'Day':'Thursday'},{Id:5, 'Day':'Friday'},{Id:6,'Day':'Saturday'},{Id: 7, 'Day':'Sunday'}];
  public selectedWeekdays = [];
  public RepeatEveryBy = '';

  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;

  navigateToListPage() {
    this._location.back();
  }

  constructor(
    private _location: Location,
    private _router: Router,
    private _activatedRouter: ActivatedRoute,
    private _http: HttpClient,
    private _spinnerService: NgxUiLoaderService,
    private formbuilder: FormBuilder,
    private _messageDialogService: MessageDialogService,
    private scheduleService: ScheduleService,
    private injector: Injector,
    private router: Router,
  ) {
    this.schedule = new Schedule;
    let me = this;
    _router.events.subscribe(e => {
      if (e instanceof NavigationEnd) {
        if (e.url.includes('/schedule/Add')) {
          localStorage.removeItem("scheduleparams");
        }
      }
    });
    _router.events.subscribe(e => {
      if (e instanceof NavigationEnd) {
        if (e.url.includes('/schedule')) {
          //set passing parameters to localstorage.
          this.params = JSON.parse(localStorage.getItem('scheduleparams'));
          if (localStorage.getItem('scheduleparams')) {
            this.schedule.Identifier = this.params.Routeparams.passingparams.ScheduleIdentifier;
          }
        } else {
          localStorage.removeItem("scheduleparams");
        }
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
  }

  async getScheduleRecords() {
    let scheduleService = this.injector.get(ScheduleService);
    let searchParameter: any = {};
    searchParameter.PagingParameter = {};
    searchParameter.PagingParameter.PageIndex = Constants.DefaultPageIndex;
    searchParameter.PagingParameter.PageSize = Constants.DefaultPageSize;
    searchParameter.SortParameter = {};
    searchParameter.SortParameter.SortColumn = Constants.Name;
    searchParameter.SortParameter.SortOrder = Constants.Ascending;
    searchParameter.SearchMode = Constants.Exact;
    searchParameter.Identifier = this.schedule.Identifier;
    searchParameter.IsStatementDefinitionRequired = true;
    searchParameter.ProductBatchName = this.schedule.ProductBatchName;
    var response = await scheduleService.getSchedule(searchParameter);
    this.schedule = response.List[0];

    this.scheduleRecord = response.List[0];
    if (this.scheduleRecord != null && this.scheduleRecord.ProductBatches != null || this.scheduleRecord.ProductBatches.length > 0) {
      if (this.scheduleRecord.ProductBatches[0].RecurrancePattern == null || this.scheduleRecord.ProductBatches[0].RecurrancePattern == '') {
        this.RecurrencePattern = 'Repeat';
      } else {
        if (this.scheduleRecord.ProductBatches[0].RecurrancePattern.includes('Custom')) {
          let index = this.scheduleRecord.ProductBatches[0].RecurrancePattern.indexOf('-');
          this.RepeatEveryBy = this.scheduleRecord.ProductBatches[0].RecurrancePattern.substring(index + 1, this.scheduleRecord.ProductBatches[0].RecurrancePattern.length);
          this.RecurrencePattern = this.scheduleRecord.ProductBatches[0].RecurrancePattern.substring(0, index);
        } else {
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

    this.setScheduleOccuranceMessage();
  }

  setScheduleOccuranceMessage() {
    var dt = new Date(this.schedule.StartDate);
    var dayOfMonth = dt.getDate(); //this.schedule.DayOfMonth == undefined || this.schedule.DayOfMonth == 0 ? dt.getDate() : this.schedule.DayOfMonth;
    var ssd = new Date(dt.getFullYear(), dt.getMonth(), dayOfMonth);
    var schedulestartdte = ssd.toLocaleDateString();
    var dte = ssd.getDate();
    var month = this.monthArray[ssd.getMonth()];

    if(this.schedule.RecurrancePattern == 'DoesNotRepeat') {
      this.ScheduleOccuranceMessage = 'Occurs once on ' + schedulestartdte;
    }else {
      let scheduleRunUtilMessage = '';
      if(this.schedule.EndDate != null && this.schedule.EndDate.toString() != "0001-01-01T00:00:00") {
        let sed = new Date(this.schedule.EndDate);
        scheduleRunUtilMessage = ' until '+sed.toLocaleDateString();
      }else if(this.schedule.NoOfOccurrences != null) {
        scheduleRunUtilMessage = ' upto '+this.schedule.NoOfOccurrences + " occurence.";
      }
      
      let repeatEvery = this.schedule.RepeatEveryDayMonWeekYear != null && this.schedule.RepeatEveryDayMonWeekYear != 0 ? this.schedule.RepeatEveryDayMonWeekYear : 1;
      let repeatEveryByVal = this.RepeatEveryBy != null && this.RepeatEveryBy != '' ? this.RepeatEveryBy : 'Month';
      let occurance = '';
      if(repeatEveryByVal == 'Day') {
        if(repeatEvery == 1) {
          occurance = 'day';
        }else{
          occurance = repeatEvery+' days ';
        }
      }
      else if(repeatEveryByVal == 'Week') {
        var weekdaystr = '';
        if(this.selectedWeekdays.length > 0) {
          if(this.selectedWeekdays.length == 7 && repeatEvery == 1) {
            occurance = 'day';
          }else {
            this.selectedWeekdays.sort(function(a, b){
              return a.Id - b.Id;
            });
            for(let i=0; i<this.selectedWeekdays.length; i++) {
              let day = this.selectedWeekdays[i].Day;
              weekdaystr = weekdaystr + (weekdaystr != '' ? (i == (this.selectedWeekdays.length - 1) ? ' and ' : ', ') : '') + day;                
            }
            if(repeatEvery == 1) {
              occurance = '' + (weekdaystr != '' ? 'on '+ weekdaystr : ' week');
            }else{
              occurance = repeatEvery+' weeks ' + (weekdaystr != '' ? 'on '+ weekdaystr : '');
            }
          }
        }
      }
      else if(repeatEveryByVal == 'Month') {
        if(repeatEvery == 1) {
          occurance = 'month on day '+dte;
        }else{
          occurance = repeatEvery+' months on day '+dte;
        }
      }
      else if(repeatEveryByVal == 'Year') {
        if(repeatEvery == 1) {
          occurance = 'year on day '+dte+ ' of '+month;
        }else{
          occurance = repeatEvery+' years on day '+dte+ ' of '+month;
        }
      }
      this.ScheduleOccuranceMessage = 'On every '+occurance+' starting ' + schedulestartdte + scheduleRunUtilMessage;
    }
    
  }

  navigateToScheduleEdit() {
    let queryParams = {
      Routeparams: {
        passingparams: {
          "ScheduleIdentifier": this.schedule.Identifier,
        },
        filteredparams: {
          //passing data using json stringify.
          "ScheduleName": this.scheduleRecord != null && this.scheduleRecord.length > 0 && this.scheduleRecord.ProductBatches.length > 0 ? this.scheduleRecord.ProductBatches[0].ScheduleNameByUser : ""
        }
      }
    }
    localStorage.setItem("scheduleparams", JSON.stringify(queryParams))
    const router = this.injector.get(Router);
    router.navigate(['schedulemanagement', 'Edit']);
  }

  deleteSchedule() {
    let message = 'Are you sure, you want to delete this record?';
    this._messageDialogService.openConfirmationDialogBox('Confirm', message, Constants.msgBoxWarning).subscribe(async (isConfirmed) => {
      if (isConfirmed) {
        let roleData = [{
          "Identifier": this.schedule.Identifier,
        }];

        let isDeleted = await this.scheduleService.deleteSchedule(roleData);
        if (isDeleted) {
          let messageString = Constants.recordDeletedMessage;
          this._messageDialogService.openDialogBox('Success', messageString, Constants.msgBoxSuccess);
          this._router.navigate(['schedulemanagement']);
        }
      }
    });
  }

  public batchmasterList: any = [];
  public baseURL: string = AppSettings.baseURL;

  async getBatchMaster() {
    this.isCollapsedBatch = !this.isCollapsedBatch;
    if (this.IsBatchDetailsGet == false) {
      this._spinnerService.start();
      this._http.post(this.baseURL + 'Schedule/GetBatchMaster?scheduleIdentifier=' + this.schedule.Identifier, null).subscribe(
        data => {
          this.IsBatchDetailsGet = true;
          this.batchmasterList = data;
          for (var i = 0; i < this.batchmasterList.length; i++) {
            this.batchmasterList[i].Index = i+1;
          }
          
          this.dataSource = new MatTableDataSource<Schedule>(this.batchmasterList);
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

  RunScheduleNow(batch : any) {
    let message = 'Are you sure, you want to run schedule now for this batch record?';
    this._messageDialogService.openConfirmationDialogBox('Confirm', message, Constants.msgBoxWarning).subscribe(async (isConfirmed) => {
      if (isConfirmed) {
        batch.Status = 'Running';
        this.scheduleService.RunScheduleNow(batch);
      }
    });
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
        case 'id': return compare(a.Identifier, b.Identifier, isAsc);
        case 'BatchName': return compareStr(a.BatchName, b.BatchName, isAsc);
        case 'BatchStatus': return compareStr(a.Status, b.Status, isAsc);
        case 'IsExecuted': return compareStr(a.IsExecuted, b.IsExecuted, isAsc);
        case 'IsDataReady': return compareStr(a.IsDataReady, b.IsDataReady, isAsc);
        case 'DataExtractionDate': return compareDate(a.DataExtractionDate, b.DataExtractionDate, isAsc);
        case 'BatchExecutionDate': return compareDate(a.BatchExecutionDate, b.BatchExecutionDate, isAsc);
        default: return 0;
      }
    });
    this.dataSource = new MatTableDataSource<Schedule>(this.sortedscheduleList);
    this.dataSource.sort = this.sort;
    this.array = this.sortedscheduleList;
    this.totalSize = this.array.length;
    this.iterator();
  }

  ApproveBatch(batch: any) {
    let message = 'Are you sure, you want to approve this batch?';
    if (batch.Status == 'Failed') {
      message = "You cannot able to generate failed customer's statements once batch is approved. " + message;
    }
    this._messageDialogService.openConfirmationDialogBox('Confirm', message, Constants.msgBoxWarning).subscribe(async (isConfirmed) => {
      if (isConfirmed) {
        let result = await this.scheduleService.ValidateApproveBatchAsync(batch.Identifier);
        if (result) {
          let messageString = "Batch approval in progress successfully";
          this._messageDialogService.openDialogBox('Success', messageString, Constants.msgBoxSuccess);
          this.isCollapsedBatch = !this.isCollapsedBatch;
          this.IsBatchDetailsGet = false;
          this.getBatchMaster();
        }
      }
    });
  }

  CleanBatch(batch: any) {
    let message = 'Are you sure, you want to clean for this batch?';
    this._messageDialogService.openConfirmationDialogBox('Confirm', message, Constants.msgBoxWarning).subscribe(async (isConfirmed) => {
      if (isConfirmed) {
        let result = await this.scheduleService.CleanBatch(batch.Identifier);
        if (result) {
          let messageString = "Batch data cleaned successfully";
          this._messageDialogService.openDialogBox('Success', messageString, Constants.msgBoxSuccess);
          this.isCollapsedBatch = !this.isCollapsedBatch;
          this.IsBatchDetailsGet = false;
          this.getBatchMaster();
        }
      }
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
  var a1 = new Date(a);
  var b1 = new Date(b);
  return (a1.getTime() < b1.getTime() ? -1 : 1) * (isAsc ? 1 : -1);
}
