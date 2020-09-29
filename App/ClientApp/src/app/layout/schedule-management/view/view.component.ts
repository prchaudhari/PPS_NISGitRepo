import { Component, OnInit, Injector, ChangeDetectorRef, ViewChild, OnDestroy } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import * as $ from 'jquery';
import { Router, ActivatedRoute, NavigationEnd } from '@angular/router';
import { environment } from '../../../../environments/environment';
import { Schedule } from '../schedule';
import { SortParameter, SearchMode } from '../../../shared/models/commonmodel';
import { NgxUiLoaderService } from 'ngx-ui-loader';
import { Location } from '@angular/common';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { Constants } from 'src/app/shared/constants/constants';
import { MessageDialogService } from 'src/app/shared/services/mesage-dialog.service';
import { FormGroup, FormBuilder, Validators, FormControl, SelectControlValueAccessor, FormArray, ValidatorFn } from '@angular/forms';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { ScheduleService } from '../schedule.service';
import { ConfigConstants } from '../../../shared/constants/configConstants';
@Component({
  selector: 'app-view',
  templateUrl: './view.component.html',
  styleUrls: ['./view.component.scss']
})
export class ViewComponent implements OnInit {
  public schedule: Schedule;
  public params;
  public userClaimsRolePrivilegeOperations: any[] = [];
  public isCollapsedDetails: boolean = false;
  public isCollapsedBatch: boolean = true;
  public IsBatchDetailsGet = false;
  displayedColumns: string[] = ['id', 'BatchName', 'IsExecuted', 'IsDataReady', 'DataExtractionDate', 'BatchExecutionDate','Actions'];
  dataSource = new MatTableDataSource<any>();
  public pageSize = 5;
  public currentPage = 0;
  public totalSize = 0;
  public array: any;
  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
  navigateToListPage() {
    this._location.back();
  }
  public isDaily: boolean = false;
  public isWeekly: boolean = true;
  public isMonthly: boolean = false;
  public isYearly: boolean = false;
  public isEndDate: boolean = true;
  public isEndAfter: boolean = false;
  public isNoEndDate: boolean = false;

  isEndDateClicked() {
    this.isEndDate = true;
    this.isEndAfter = false;
    this.isNoEndDate = false;
  }
  isEndAfterClicked() {
    this.isEndDate = false;
    this.isEndAfter = true;
    this.isNoEndDate = false;
  }
  isNoEndDateClicked() {
    this.isEndDate = false;
    this.isEndAfter = false;
    this.isNoEndDate = true;
  }
  isDailyClicked() {
    this.isDaily = true;
    this.isWeekly = false;
    this.isMonthly = false;
    this.isYearly = false;
  }
  isWeeklyClicked() {
    this.isDaily = false;
    this.isWeekly = true;
    this.isMonthly = false;
    this.isYearly = false;
  }
  isMonthlyClicked() {
    this.isDaily = false;
    this.isWeekly = false;
    this.isMonthly = true;
    this.isYearly = false;

  }
  isYearlyClicked() {
    this.isDaily = false;
    this.isWeekly = false;
    this.isMonthly = false;
    this.isYearly = true;
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
            this.getScheduleRecords();

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
    var response = await scheduleService.getSchedule(searchParameter);
    this.schedule = response.List[0];
  }

  navigateToScheduleEdit() {
    let queryParams = {
      Routeparams: {
        passingparams: {
          "ScheduleIdentifier": this.schedule.Identifier,
        },
        filteredparams: {
          //passing data using json stringify.
          "ScheduleName": this.schedule.Name != null ? this.schedule.Name : ""
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
  public baseURL: string = ConfigConstants.BaseURL;
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
    // ['id', 'BatchName', 'IsExecuted', 'IsDataReady', 'DataExtractionDate', 'BatchExecutionDate', 'Actions'];

    this.sortedscheduleList = data.sort((a, b) => {
      const isAsc = sort.direction === 'asc';
      switch (sort.active) {
        case 'id': return compare(a.Identifier, b.Identifier, isAsc);
        case 'BatchName': return compareStr(a.BatchName, b.BatchName, isAsc);
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
