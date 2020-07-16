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
import { ScheduleService } from '../schedule.service';
import { Schedule } from '../schedule';
import { ScheduleRunHistory } from '../scheduleHitory';


@Component({
  selector: 'app-history',
  templateUrl: './history.component.html',
  styleUrls: ['./history.component.scss']
})
export class HistoryComponent implements OnInit {
  public isFilter: boolean = false;
  public pageSize = 5;
  public currentPage = 0;
  public totalSize = 0;
  public scheduleHistoryList: ScheduleRunHistory[] = [];
  public sortedscheduleHistoryList: ScheduleRunHistory[] = [];
  public ScheduleFilterForm: FormGroup;
  public filterFromDateError: boolean = false;
  public filterFromDateErrorMessage: string = "";
  public filterToDateError: boolean = false;
  public filterToDateErrorMessage: string = "";
  public isFilterDone = false;
  public array: any;
  public userClaimsRolePrivilegeOperations: any[] = [];
  closeFilter() {
    this.isFilter = !this.isFilter;
  }
  displayedColumns: string[] = ['name', 'template', 'startDate', 'endDate', 'DayOfMonth', 'actions'];

  dataSource = new MatTableDataSource<any>();

  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;

  ngOnInit() {
    this.getSchedule(null);
    //this.getStatementDefinition(null);
    this.ScheduleFilterForm = this.fb.group({
      filterDisplayName: [null],

      filterStatementDefiniton: [null],
      filterStartDate: [null],
      filterEndDate: [null],
    });
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
    private scheduleService: ScheduleService) {
    this.sortedscheduleHistoryList = this.scheduleHistoryList.slice();
  }

  public handlePage(e: any) {
    this.currentPage = e.pageIndex;
    this.pageSize = e.pageSize;
    this.iterator();
  }

  navigateToListPage() {
    this.route.navigate(['/schedulemanagement']);
  }

  private iterator() {
    const end = (this.currentPage + 1) * this.pageSize;
    const start = this.currentPage * this.pageSize;
    const part = this.array.slice(start, end);
    this.dataSource = part;
    this.dataSource.sort = this.sort;
  }

  get filterDisplayName() {
    return this.ScheduleFilterForm.get('filterDisplayName');
  }

  get filterStatementDefiniton() {
    return this.ScheduleFilterForm.get('filterStatementDefiniton');
  }

  get filterStartDate() {
    return this.ScheduleFilterForm.get('filterStartDate');
  }

  get filterEndDate() {
    return this.ScheduleFilterForm.get('filterEndDate');
  }

  resetFilterForm() {
    this.ScheduleFilterForm.patchValue({
      filterDisplayName: null,
      filterStatementDefiniton: null,
      filterEndDate: null,
      filterStartDate: null
    });

    this.filterFromDateError = false;
    this.filterToDateError = false;
    this.filterFromDateErrorMessage = "";
    this.filterToDateErrorMessage = "";
  }

  validateFilterDate(): boolean {
    if (this.ScheduleFilterForm.value.filterStartDate != null && this.ScheduleFilterForm.value.filterStartDate != '' &&
      this.ScheduleFilterForm.value.filterEndDate != null && this.ScheduleFilterForm.value.filterEndDate != '') {
      let startDate = this.ScheduleFilterForm.value.filterStartDate;
      let toDate = this.ScheduleFilterForm.value.filterEndDate;
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

    if (this.ScheduleFilterForm.value.filterStartDate != null && this.ScheduleFilterForm.value.filterStartDate != '' &&
      this.ScheduleFilterForm.value.filterEndDate != null && this.ScheduleFilterForm.value.filterEndDate != '') {
      let startDate = this.ScheduleFilterForm.value.filterStartDate;
      let toDate = this.ScheduleFilterForm.value.filterEndDate;
      if (startDate.getTime() > toDate.getTime()) {
        this.filterFromDateError = true;
        this.filterFromDateErrorMessage = ErrorMessageConstants.getStartDateLessThanEndDateMessage;
      }
    }
  }

  //This method has been used for fetching search records
  searchScheduleRecordFilter(searchType) {
    this.filterFromDateError = false;
    this.isFilterDone = true;
    if (searchType == 'reset') {
      this.resetFilterForm();
      this.getSchedule(null);
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
        searchParameter.SortParameter.SortOrder = Constants.Ascending;
        searchParameter.SearchMode = Constants.Contains;
        if (this.ScheduleFilterForm.value.filterDisplayName != null && this.ScheduleFilterForm.value.filterDisplayName != '') {
          searchParameter.ScheduleName = this.ScheduleFilterForm.value.filterDisplayName.trim();
        }
        if (this.ScheduleFilterForm.value.filterStatementDefiniton != null && this.ScheduleFilterForm.value.filterStatementDefiniton != '') {
          searchParameter.StatementDefinitionName = this.ScheduleFilterForm.value.filterStatementDefiniton.trim();
        }
        if (this.ScheduleFilterForm.value.filterStartDate != null && this.ScheduleFilterForm.value.filterStartDate != '') {
          //searchParameter.StartDate = this.ScheduleFilterForm.value.filterStartDate;
          searchParameter.StartDate = new Date(this.ScheduleFilterForm.value.filterStartDate.setHours(0, 0, 0));
        }
        if (this.ScheduleFilterForm.value.filterEndDate != null && this.ScheduleFilterForm.value.filterEndDate != '') {
          //searchParameter.EndDate = this.ScheduleFilterForm.value.filterEndDate;
          searchParameter.EndDate = new Date(this.ScheduleFilterForm.value.filterEndDate.setHours(23, 59, 59));
        }
        this.currentPage = 0;
        this.getSchedule(searchParameter);
        this.isFilter = !this.isFilter;
      }
    }
  }

  resetPageFilterForm() {
    this.ScheduleFilterForm.patchValue({
      filterDisplayName: null,
      filterStatementDefiniton: null,
      filterStartDate: null,
      filterEndDate: null
    });

    this.filterFromDateError = false;
    this.filterToDateError = false;
    this.filterFromDateErrorMessage = "";
    this.filterToDateErrorMessage = "";
  }

  async getSchedule(searchParameter) {
    let scheduleService = this.injector.get(ScheduleService);
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
    searchParameter.IsStatementDefinitionRequired = true;
    this.scheduleHistoryList = await scheduleService.getScheduleHistory(searchParameter);
    if (this.scheduleHistoryList.length == 0 && this.isFilterDone == true) {
      let message = ErrorMessageConstants.getNoRecordFoundMessage;
      this._messageDialogService.openDialogBox('Error', message, Constants.msgBoxError).subscribe(data => {
        if (data == true) {
          this.resetFilterForm();
          this.getSchedule(null);
        }
      });
    }
    this.dataSource = new MatTableDataSource<ScheduleRunHistory>(this.scheduleHistoryList);
    this.dataSource.sort = this.sort;
    this.array = this.scheduleHistoryList;
    this.totalSize = this.array.length;
    this.iterator();
  }

  sortData(sort: MatSort) {
    const data = this.scheduleHistoryList.slice();
    if (!sort.active || sort.direction === '') {
      this.sortedscheduleHistoryList = data;
      return;
    }
    //'name', 'schedule', 'startDate', 'endDate', 'DayOfMonth',
    this.sortedscheduleHistoryList = data.sort((a, b) => {
      const isAsc = sort.direction === 'asc';
      switch (sort.active) {
        case 'name': return compareStr(a.Schedule.Name, b.Schedule.Name, isAsc);
        case 'startDate': return compareDate(a.StartDate, b.StartDate, isAsc);
        case 'endDate': return compareDate(a.EndDate, b.EndDate, isAsc);
        case 'DayOfMonth': return compare(a.Schedule.DayOfMonth, b.Schedule.DayOfMonth, isAsc);
        
        default: return 0;
      }
    });
    this.dataSource = new MatTableDataSource<ScheduleRunHistory>(this.sortedscheduleHistoryList);
    this.dataSource.sort = this.sort;
    this.array = this.sortedscheduleHistoryList;
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
