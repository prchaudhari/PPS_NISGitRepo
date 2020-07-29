
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

export interface ListElement {
  schedule: string;
  time: string;
  status: string;
  date: string;
  record: string;
}

const List_Data: ListElement[] = [
  { schedule: 'SM 01', time: '40 Minutes', record: '25/25', status: 'Completed', date: '02/05/2020' },
  { schedule: 'SM 01', time: '20 Minutes', record: '0/25', status: 'Failed', date: '03/05/2020' },
  { schedule: 'SM 01', time: '30 Minutes', record: '13/25', status: 'In Progress', date: '04/05/2020' },
  { schedule: 'SM 01', time: '50 Minutes', record: '30/30', status: 'Completed', date: '02/05/2020' },
  { schedule: 'SM 01', time: '60 Minutes', record: '125/125', status: 'Completed', date: '05/05/2020' },
  { schedule: 'SM 01', time: '20 Minutes', record: '0/25', status: 'Failed', date: '02/05/2020' },
];

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

  closeFilter() {
    this.isFilter = !this.isFilter;
  }

  displayedColumns: string[] = ['schedule', 'time', 'record', 'status', 'date', 'actions'];

  dataSource = new MatTableDataSource<any>();

  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;

  constructor(private injector: Injector,
    private fb: FormBuilder,
    private uiLoader: NgxUiLoaderService,
    private _messageDialogService: MessageDialogService,
    private route: Router,
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
  get filterScheduleName() {
    return this.ScheduleLogFilterForm.get('filterScheduleName');
  }

  get filterStatus() {
    return this.ScheduleLogFilterForm.get('filterStatus');
  }

  get filterPublishedOnFromDate() {
    return this.ScheduleLogFilterForm.get('filterPublishedOnFromDate');
  }

  get filterPublishedOnToDate() {
    return this.ScheduleLogFilterForm.get('filterPublishedOnToDate');
  }

  ngOnInit() {
    this.getScheduleLogs(null);
    //this.getPageTypes();
    this.ScheduleLogFilterForm = this.fb.group({
      filterScheduleName: [null],
      filterStatus: [0],
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

  sortData(sort: MatSort) {
    const data = this.scheduleLogList.slice();
    if (!sort.active || sort.direction === '') {
      this.sortedScheduleLogList = data;
      return;
    }

    this.sortedScheduleLogList = data.sort((a, b) => {
      const isAsc = sort.direction === 'asc';
      switch (sort.active) {
        case 'schedule': return compareStr(a.ScheduleName, b.ScheduleName, isAsc);
        case 'time': return compareStr(a.ProcessingTime, b.ProcessingTime, isAsc);
        case 'record': return compareStr(a.RecordProcessed, b.RecordProcessed, isAsc);
        case 'status': return compareStr(a.ScheduleStatus, b.ScheduleStatus, isAsc);
        case 'date': return compareDate(a.CreateDate, b.CreateDate, isAsc);
        default: return 0;
      }
    });
    this.dataSource = new MatTableDataSource<ScheduleLog>(this.sortedScheduleLogList);
    this.dataSource.sort = this.sort;
    this.array = this.sortedScheduleLogList;
    this.totalSize = this.array.length;
    this.iterator();
  }

  async getScheduleLogs(searchParameter) {
    let scheduleLogService = this.injector.get(ScheduleLogService);
    if (searchParameter == null) {
      searchParameter = {};
      searchParameter.IsActive = true;
      searchParameter.PagingParameter = {};
      searchParameter.PagingParameter.PageIndex = Constants.DefaultPageIndex;
      searchParameter.PagingParameter.PageSize = Constants.DefaultPageSize;
      searchParameter.SortParameter = {};
      searchParameter.SortParameter.SortColumn = 'Id';
      searchParameter.SortParameter.SortOrder = Constants.Descending;
      searchParameter.SearchMode = Constants.Contains;
    }
    this.scheduleLogList = await scheduleLogService.getScheduleLog(searchParameter);
    if (this.scheduleLogList.length == 0 && this.isFilterDone == true) {
      let message = ErrorMessageConstants.getNoRecordFoundMessage;
      this._messageDialogService.openDialogBox('Error', message, Constants.msgBoxError).subscribe(data => {
        if (data == true) {
          //this.resetRoleFilterForm();
          this.getScheduleLogs(null);
        }
      });
    }
    this.dataSource = new MatTableDataSource<ScheduleLog>(this.scheduleLogList);
    this.dataSource.sort = this.sort;
    this.array = this.scheduleLogList;
    this.totalSize = this.array.length;
    this.iterator();
  }

  validateFilterDate(): boolean {
    if (this.ScheduleLogFilterForm.value.filterPublishedOnFromDate != null && this.ScheduleLogFilterForm.value.filterPublishedOnFromDate != '' &&
      this.ScheduleLogFilterForm.value.filterPublishedOnToDate != null && this.ScheduleLogFilterForm.value.filterPublishedOnToDate != '') {
      let startDate = this.ScheduleLogFilterForm.value.filterPublishedOnFromDate;
      let toDate = this.ScheduleLogFilterForm.value.filterPublishedOnToDate;
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
    if (this.ScheduleLogFilterForm.value.filterPublishedOnFromDate != null && this.ScheduleLogFilterForm.value.filterPublishedOnFromDate != '') {
      let startDate = this.ScheduleLogFilterForm.value.filterPublishedOnFromDate;
      if (startDate.getTime() > currentDte.getTime()) {
        this.filterFromDateError = true;
        this.filterFromDateErrorMessage = ErrorMessageConstants.getStartDateLessThanCurrentDateMessage;
      }
    }
    if (this.ScheduleLogFilterForm.value.filterPublishedOnToDate != null && this.ScheduleLogFilterForm.value.filterPublishedOnToDate != '') {
      let toDate = this.ScheduleLogFilterForm.value.filterPublishedOnToDate;
      if (toDate.getTime() > currentDte.getTime()) {
        this.filterToDateError = true;
        this.filterToDateErrorMessage = ErrorMessageConstants.getEndDateLessThanCurrentDateMessage;
      }
    }
    if (this.ScheduleLogFilterForm.value.filterPublishedOnFromDate != null && this.ScheduleLogFilterForm.value.filterPublishedOnFromDate != '' &&
      this.ScheduleLogFilterForm.value.filterPublishedOnToDate != null && this.ScheduleLogFilterForm.value.filterPublishedOnToDate != '') {
      let startDate = this.ScheduleLogFilterForm.value.filterPublishedOnFromDate;
      let toDate = this.ScheduleLogFilterForm.value.filterPublishedOnToDate;
      if (startDate.getTime() > toDate.getTime()) {
        this.filterFromDateError = true;
        this.filterFromDateErrorMessage = ErrorMessageConstants.getStartDateLessThanEndDateMessage;
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
        searchParameter.PagingParameter.PageIndex = Constants.DefaultPageIndex;
        searchParameter.PagingParameter.PageSize = Constants.DefaultPageSize;
        searchParameter.SortParameter = {};
        searchParameter.SortParameter.SortColumn = 'Id';
        searchParameter.SortParameter.SortOrder = Constants.Descending;
        searchParameter.SearchMode = Constants.Contains;
        if (this.ScheduleLogFilterForm.value.filterScheduleName != null && this.ScheduleLogFilterForm.value.filterScheduleName != '') {
          searchParameter.ScheduleName = this.ScheduleLogFilterForm.value.filterScheduleName.trim();
        }
        if (this.ScheduleLogFilterForm.value.filterOwner != null && this.ScheduleLogFilterForm.value.filterOwner != '') {
          searchParameter.PageOwner = this.ScheduleLogFilterForm.value.filterOwner.trim();
        }
        if (this.filterPageTypeId != 0) {
          searchParameter.PageTypeId = this.filterPageTypeId;
        }
        if (this.ScheduleLogFilterForm.value.filterStatus != null && this.ScheduleLogFilterForm.value.filterStatus != 0) {
          searchParameter.ScheduleStatus = this.ScheduleLogFilterForm.value.filterStatus;
        }
        if (this.ScheduleLogFilterForm.value.filterPublishedOnFromDate != null && this.ScheduleLogFilterForm.value.filterPublishedOnFromDate != '') {
          //searchParameter.StartDate = this.ScheduleLogFilterForm.value.filterPublishedOnFromDate;
          searchParameter.StartDate = new Date(this.ScheduleLogFilterForm.value.filterPublishedOnFromDate.setHours(0, 0, 0));
          searchParameter.SortParameter.SortColumn = 'PublishedOn';
        }
        if (this.ScheduleLogFilterForm.value.filterPublishedOnToDate != null && this.ScheduleLogFilterForm.value.filterPublishedOnToDate != '') {
          //searchParameter.EndDate = this.ScheduleLogFilterForm.value.filterPublishedOnToDate;
          searchParameter.EndDate = new Date(this.ScheduleLogFilterForm.value.filterPublishedOnToDate.setHours(23, 59, 59));
          searchParameter.SortParameter.SortColumn = 'PublishedOn';
        }

        console.log(searchParameter);
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
      filterPublishedOnToDate: null
    });

    this.filterFromDateError = false;
    this.filterToDateError = false;
    this.filterFromDateErrorMessage = "";
    this.filterToDateErrorMessage = "";
  }

  ngAfterViewInit() {
    this.dataSource.paginator = this.paginator;

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
