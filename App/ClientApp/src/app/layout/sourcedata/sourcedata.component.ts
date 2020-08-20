import { Component, OnInit, Injector, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';
import { NgxUiLoaderService } from 'ngx-ui-loader';
import { Router } from '@angular/router';
import { Constants } from '../../shared/constants/constants';
import { ErrorMessageConstants } from '../../shared/constants/constants';
import { MessageDialogService } from '../../shared/services/mesage-dialog.service';
import { LocalStorageService } from '../../shared/services/local-storage.service';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { Location } from '@angular/common';
import { SourceData } from './sourcedata';
import { ConfigConstants } from '../../shared/constants/configConstants';
import { SourceDataService } from './sourcedata.service';
import { WindowRef } from '../../core/services/window-ref.service';
import { HttpClient } from '@angular/common/http';

export interface ListElement {
  time: string;
  date: string;
  page: string;
  widget: string;
  userId: string;
}

const List_Data: ListElement[] = [
  { page: 'Home', time: '09:00', date: '02/05/2020', widget: 'Image', userId: 'U001' },
  { page: 'Current Account', time: '10:00', date: '03/05/2020', widget: 'Video', userId: 'U002' },
  { page: 'Saving Account', time: '11:00', date: '04/05/2020', widget: 'Customer Information', userId: 'U003' },
  { page: 'Home', time: '11:00', date: '05/05/2020', widget: 'Customer Information', userId: 'U004' },
  { page: 'Saving Account', time: '11:00', date: '06/05/2020', widget: 'Available Balance', userId: 'U005' },
];

@Component({
  selector: 'app-sourcedata',
  templateUrl: './sourcedata.component.html',
  styleUrls: ['./sourcedata.component.scss']
})
export class SourcedataComponent implements OnInit {
  public isFilter: boolean = false;
  public scheduleLogList: SourceData[] = [];
  public isLoaderActive: boolean = false;
  public isRecordFound: boolean = false;
  public pageNo = 0;
  public pageSize = 5;
  public currentPage = 0;
  public totalSize = 0;
  public array: any;
  public isFilterDone = false;
  public sortedSourceDataList: SourceData[] = [];
  public pageTypeList: any[] = [];
  public SourceDataFilterForm: FormGroup;
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

  displayedColumns: string[] = ['date', 'time', 'page', 'widget', 'userId'];


  dataSource = new MatTableDataSource<any>();

  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;

  constructor(private injector: Injector,
    private _location: Location,
    private fb: FormBuilder,
    private uiLoader: NgxUiLoaderService,
    private _messageDialogService: MessageDialogService,
    private route: Router,
    private localstorageservice: LocalStorageService,
    private scheduleLogService: SourceDataService,
    private _window: WindowRef, private http: HttpClient) {
    this.sortedSourceDataList = this.scheduleLogList.slice();
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
  get filterCustomer() {
    return this.SourceDataFilterForm.get('filterCustomer');
  }
  get filterPage() {
    return this.SourceDataFilterForm.get('filterPage');
  } get filterWidget() {
    return this.SourceDataFilterForm.get('filterWidget');
  } get filterStartDate() {
    return this.SourceDataFilterForm.get('filterStartDate');
  } get filterEndDate() {
    return this.SourceDataFilterForm.get('filterEndDate');
  }


  ngOnInit() {
    this.getSourceDatas(null);
    //this.getPageTypes();
    this.SourceDataFilterForm = this.fb.group({
      filterCustomer: [null],
      filterPage: [null],
      filterWidget: [null],
      filterStartDate: [null],
      filterEndDate: [null],
    });
    this.SourceDataFilterForm.controls['filterCustomer'].setValue(null);
    this.SourceDataFilterForm.controls['filterPage'].setValue(null);
    this.SourceDataFilterForm.controls['filterWidget'].setValue(null);
    this.SourceDataFilterForm.controls['filterStartDate'].setValue(null);
    this.SourceDataFilterForm.controls['filterEndDate'].setValue(null);

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
      this.sortedSourceDataList = data;
      return;
    }
    //['date', 'time', 'page', 'widget', 'userId'];
    this.sortedSourceDataList = data.sort((a, b) => {
      const isAsc = sort.direction === 'asc';
      switch (sort.active) {
        case 'date': return compareDate(a.EventDate, b.EventDate, isAsc);
        case 'time': return compareDate(a.EventDate, b.EventDate, isAsc);
        case 'page': return compareStr(a.PageName, b.PageName, isAsc);
        case 'widget': return compareStr(a.Widgetname, b.Widgetname, isAsc);
        case 'userId': return compareStr(a.CustomerName, b.CustomerName, isAsc);
        default: return 0;
      }
    });
    this.dataSource = new MatTableDataSource<SourceData>(this.sortedSourceDataList);
    this.dataSource.sort = this.sort;
    this.array = this.sortedSourceDataList;
    this.totalSize = this.array.length;
    this.iterator();
  }

  async getSourceDatas(searchParameter) {
    let scheduleLogService = this.injector.get(SourceDataService);
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
    this.scheduleLogList = await scheduleLogService.getSourceData(searchParameter);
    if (this.scheduleLogList.length == 0 && this.isFilterDone == true) {
      let message = ErrorMessageConstants.getNoRecordFoundMessage;
      this._messageDialogService.openDialogBox('Error', message, Constants.msgBoxError).subscribe(data => {
        if (data == true) {
          //this.resetRoleFilterForm();
          this.getSourceDatas(null);
        }
      });
    }
    this.dataSource = new MatTableDataSource<SourceData>(this.scheduleLogList);
    this.dataSource.sort = this.sort;
    this.array = this.scheduleLogList;
    this.totalSize = this.array.length;
    this.iterator();
  }
  validateFilterDate(): boolean {
    if (this.SourceDataFilterForm.value.filterStartDate != null && this.SourceDataFilterForm.value.filterStartDate != '' &&
      this.SourceDataFilterForm.value.filterEndDate != null && this.SourceDataFilterForm.value.filterEndDate != '') {
      let startDate = this.SourceDataFilterForm.value.filterStartDate;
      let toDate = this.SourceDataFilterForm.value.filterEndDate;
      if (startDate.getTime() > toDate.getTime()) {
        this.filterFromDateError = true;
        return false;
      }
    }
    return true;
  }

  onFilterDateChange(event) {
    this.filterFromDateError = false;
    this.filterToDateError = false;
    this.filterFromDateErrorMessage = "";
    this.filterToDateErrorMessage = "";
    let currentDte = new Date();
    if (this.SourceDataFilterForm.value.filterStartDate != null && this.SourceDataFilterForm.value.filterStartDate != '') {
      let startDate = this.SourceDataFilterForm.value.filterStartDate;
      if (startDate.getTime() > currentDte.getTime()) {
        this.filterFromDateError = true;
        this.filterFromDateErrorMessage = ErrorMessageConstants.getStartDateLessThanCurrentDateMessage;
      }
    }
    if (this.SourceDataFilterForm.value.filterEndDate != null && this.SourceDataFilterForm.value.filterEndDate != '') {
      let toDate = this.SourceDataFilterForm.value.filterEndDate;
      if (toDate.getTime() > currentDte.getTime()) {
        this.filterToDateError = true;
        this.filterToDateErrorMessage = ErrorMessageConstants.getEndDateLessThanCurrentDateMessage;
      }
    }
    if (this.SourceDataFilterForm.value.filterStartDate != null && this.SourceDataFilterForm.value.filterStartDate != '' &&
      this.SourceDataFilterForm.value.filterEndDate != null && this.SourceDataFilterForm.value.filterEndDate != '') {
      let startDate = this.SourceDataFilterForm.value.filterStartDate;
      let toDate = this.SourceDataFilterForm.value.filterEndDate;
      if (startDate.getTime() > toDate.getTime()) {
        this.filterFromDateError = true;
        this.filterFromDateErrorMessage = ErrorMessageConstants.getStartDateLessThanEndDateMessage;
      }
    }
  }
  searchSourceDataRecordFilter(searchType) {
    this.filterFromDateError = false;
    this.isFilterDone = true;
    if (searchType == 'reset') {
      this.resetSchdeuleLogFilterForm();
      this.getSourceDatas(null);
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
        if (this.SourceDataFilterForm.value.filterCustomer != null && this.SourceDataFilterForm.value.filterCustomer != '') {
          searchParameter.CustomerName = this.SourceDataFilterForm.value.filterCustomer.trim();
        }
        if (this.SourceDataFilterForm.value.filterPage != null && this.SourceDataFilterForm.value.filterPage != '') {
          searchParameter.PageName = this.SourceDataFilterForm.value.filterPage.trim();
        }
        if (this.SourceDataFilterForm.value.filterWidget != null && this.SourceDataFilterForm.value.filterWidget != '') {
          searchParameter.WidgetName = this.SourceDataFilterForm.value.filterWidget.trim();
        }
        if (this.SourceDataFilterForm.value.filterStartDate != null && this.SourceDataFilterForm.value.filterStartDate != '') {
          //searchParameter.StartDate = this.ScheduleFilterForm.value.filterStartDate;
          searchParameter.StartDate = new Date(this.SourceDataFilterForm.value.filterStartDate.setHours(0, 0, 0));
        }
        if (this.SourceDataFilterForm.value.filterEndDate != null && this.SourceDataFilterForm.value.filterEndDate != '') {
          //searchParameter.EndDate = this.ScheduleFilterForm.value.filterEndDate;
          searchParameter.EndDate = new Date(this.SourceDataFilterForm.value.filterEndDate.setHours(23, 59, 59));
        }

        console.log(searchParameter);
        this.currentPage = 0;
        this.getSourceDatas(searchParameter);
        this.isFilter = !this.isFilter;
      }
    }
  }

  resetSchdeuleLogFilterForm() {
    this.SourceDataFilterForm = this.fb.group({
      filterCustomer: [null],
      filterPage: [null],
      filterWidget: [null],
      filterStartDate: [null],
      filterEndDate: [null],
    });
    this.SourceDataFilterForm.controls['filterCustomer'].setValue(null);
    this.SourceDataFilterForm.controls['filterPage'].setValue(null);
    this.SourceDataFilterForm.controls['filterWidget'].setValue(null);
    this.SourceDataFilterForm.controls['filterStartDate'].setValue(null);
    this.SourceDataFilterForm.controls['filterEndDate'].setValue(null);

    this.filterFromDateError = false;
    this.filterToDateError = false;
    this.filterFromDateErrorMessage = "";
    this.filterToDateErrorMessage = "";
  }

  ngAfterViewInit() {
    this.dataSource.paginator = this.paginator;

  }
  navigateToListPage() {
    this._location.back();
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

