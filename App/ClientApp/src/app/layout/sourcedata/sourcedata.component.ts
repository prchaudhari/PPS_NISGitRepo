import { AppSettings } from '../../appsettings';
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
import { HttpClient, HttpHeaders} from '@angular/common/http';
import { map } from 'rxjs/operators';
import * as $ from 'jquery';
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
  public baseURL = AppSettings.baseURL;

  public totalRecordCount = 0;
  public filterCustomerName = '';
  public filterPageName = '';
  public filterWidgetName = '';
  public filterSourceDataStartDate = null;
  public filterSourceDataEndDate = null;
  public sortOrder = Constants.Descending;
  public sortColumn = 'EventDate';

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
    this.getSourceDatas(null);
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
    
    if (sort.direction == 'asc') {
      this.sortOrder = Constants.Ascending;
    }else {
      this.sortOrder = Constants.Descending;
    }

    switch (sort.active) {
      case 'date': this.sortColumn = "EventDate"; break;
      case 'time': this.sortColumn = "EventDate"; break;
      case 'page': this.sortColumn = "PageName"; break;
      case 'widget': this.sortColumn = "WidgetName"; break;
      case 'userId': this.sortColumn = "CustomerName"; break;
      default: this.sortColumn = "EventDate"; break;
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
    this.getSourceDatas(searchParameter);
  }

  async getSourceDatas(searchParameter) {
    let scheduleLogService = this.injector.get(SourceDataService);
    if (searchParameter == null) {
      searchParameter = {};
      searchParameter.PagingParameter = {};
      searchParameter.PagingParameter.PageIndex = this.currentPage + 1;
      searchParameter.PagingParameter.PageSize = this.pageSize;
      searchParameter.SortParameter = {};
      searchParameter.SortParameter.SortColumn = this.sortColumn;
      searchParameter.SortParameter.SortOrder = this.sortOrder;
      searchParameter.SearchMode = Constants.Contains;
    }
    if (this.filterCustomerName != null && this.filterCustomerName != '') {
      searchParameter.CustomerName = this.filterCustomerName.trim();
    }
    if (this.filterPageName != null && this.filterPageName != '') {
      searchParameter.PageName = this.filterPageName.trim();
    }
    if (this.filterWidgetName != null && this.filterWidgetName != '') {
      searchParameter.WidgetName = this.filterWidgetName.trim();
    }
    if (this.filterSourceDataStartDate != null && this.filterSourceDataStartDate != '') {
      searchParameter.StartDate = new Date(this.filterSourceDataStartDate.setHours(0, 0, 0));
    }
    if (this.filterSourceDataEndDate != null && this.filterSourceDataEndDate != '') {
      searchParameter.EndDate = new Date(this.filterSourceDataEndDate.setHours(23, 59, 59));
    }
    var response = await scheduleLogService.getSourceData(searchParameter);
    this.scheduleLogList = response.List;
    this.totalRecordCount = response.RecordCount;
    if (this.scheduleLogList.length == 0 && this.isFilterDone == true) {
      let message = ErrorMessageConstants.getNoRecordFoundMessage;
      this._messageDialogService.openDialogBox('Error', message, Constants.msgBoxError).subscribe(data => {
        if (data == true) {
          this.resetSchdeuleLogFilterForm();
          this.getSourceDatas(null);
        }
      });
    }
    this.dataSource = new MatTableDataSource<SourceData>(this.scheduleLogList);
    this.dataSource.sort = this.sort;
    this.array = this.scheduleLogList;
    this.totalSize = this.totalRecordCount;
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

  ExportData(): void {
    let searchParameter: any = {};
    searchParameter.PagingParameter = {};
    searchParameter.PagingParameter.PageIndex = Constants.DefaultPageIndex;
    searchParameter.PagingParameter.PageSize = Constants.DefaultPageSize;
    searchParameter.SortParameter = {};
    searchParameter.SortParameter.SortColumn = 'EventDate';
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
      searchParameter.StartDate = new Date(this.SourceDataFilterForm.value.filterStartDate.setHours(0, 0, 0));
    }
    if (this.SourceDataFilterForm.value.filterEndDate != null && this.SourceDataFilterForm.value.filterEndDate != '') {
      searchParameter.EndDate = new Date(this.SourceDataFilterForm.value.filterEndDate.setHours(23, 59, 59));
    }
    this.uiLoader.start();
    this.http.post(this.baseURL + 'AnalyticsData/ExportAnalyticsData', searchParameter, { responseType: "arraybuffer", observe: 'response' }).pipe(map(response => response))
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
        searchParameter.PagingParameter.PageIndex = 1;
        searchParameter.PagingParameter.PageSize = this.pageSize;
        searchParameter.SortParameter = {};
        searchParameter.SortParameter.SortColumn = this.sortColumn;
        searchParameter.SortParameter.SortOrder = this.sortOrder;
        searchParameter.SearchMode = Constants.Contains;

        if (this.SourceDataFilterForm.value.filterCustomer != null && this.SourceDataFilterForm.value.filterCustomer != '') {
          this.filterCustomerName = this.SourceDataFilterForm.value.filterCustomer.trim();
          searchParameter.CustomerName = this.SourceDataFilterForm.value.filterCustomer.trim();
        }
        if (this.SourceDataFilterForm.value.filterPage != null && this.SourceDataFilterForm.value.filterPage != '') {
          this.filterPageName = this.SourceDataFilterForm.value.filterPage.trim();
          searchParameter.PageName = this.SourceDataFilterForm.value.filterPage.trim();
        }
        if (this.SourceDataFilterForm.value.filterWidget != null && this.SourceDataFilterForm.value.filterWidget != '') {
          this.filterWidgetName = this.SourceDataFilterForm.value.filterWidget.trim();
          searchParameter.WidgetName = this.SourceDataFilterForm.value.filterWidget.trim();
        }
        if (this.SourceDataFilterForm.value.filterStartDate != null && this.SourceDataFilterForm.value.filterStartDate != '') {
          this.filterSourceDataStartDate = this.SourceDataFilterForm.value.filterStartDate;
          searchParameter.StartDate = new Date(this.SourceDataFilterForm.value.filterStartDate.setHours(0, 0, 0));
        }
        if (this.SourceDataFilterForm.value.filterEndDate != null && this.SourceDataFilterForm.value.filterEndDate != '') {
          this.filterSourceDataEndDate = this.SourceDataFilterForm.value.filterEndDate;
          searchParameter.EndDate = new Date(this.SourceDataFilterForm.value.filterEndDate.setHours(23, 59, 59));
        }

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

    this.currentPage = 0;
    this.filterCustomerName = '';
    this.filterPageName = '';
    this.filterWidgetName = '';
    this.filterSourceDataStartDate = null;
    this.filterSourceDataEndDate = null;
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


