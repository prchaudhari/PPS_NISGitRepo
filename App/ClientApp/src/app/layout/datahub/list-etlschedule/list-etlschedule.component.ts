import { Component, OnInit, Injector, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';
import { NgxUiLoaderService } from 'ngx-ui-loader';
import { Router, NavigationEnd } from '@angular/router';

import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';

import { Constants, ErrorMessageConstants } from 'src/app/shared/constants/constants';
import { LocalStorageService } from 'src/app/shared/services/local-storage.service';
import { MessageDialogService } from 'src/app/shared/services/mesage-dialog.service';

import { ETLSchedule } from '../datahub';
import { DataHubService } from '../datahub.service';
import { ProductService } from '../../products/product.service';

@Component({
  selector: 'app-list-etlschedule',
  templateUrl: './list-etlschedule.component.html',
  styleUrls: ['./list-etlschedule.component.scss']
})
export class ListEtlscheduleComponent implements OnInit {
  public isFilter: boolean = false;
  public pageNo = 0;
  public pageSize = 5;
  public currentPage = 0;
  public totalSize = 0;
  public isFilterDone = false;
  public totalRecordCount = 0;
  public sortOrder = Constants.Descending;
  public sortColumn = 'Name';
  displayedColumns: string[] = ['product', 'name', 'scheduleDate', 'startDate', 'endDate', 'dayOfMonth', 'isActive', 'actions'];
  dataSource = new MatTableDataSource<any>();

  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;

  public etlScheduleList: ETLSchedule[] = [];
  public sortedEtlScheduleList: ETLSchedule[] = [];
  public ETLScheduleFilterForm: FormGroup;

  public userClaimsRolePrivilegeOperations: any[] = [];
  public array: any;
  public productList: any = [];
  public activeStatusArray: any[] = [
    {
      'Identifier': 0,
      'Name': 'Both'
    },
    {
      'Identifier': 1,
      'Name': 'Active'
    },
    {
      'Identifier': 2,
      'Name': 'DeActive'
    }
  ];

  public filterProductValue = 0;
  public filterNameValue = '';
  public filterScheduleDateValue = null;
  public filterScheduleStartDateValue = null;
  public filterScheduleEndDateValue = null;
  public filterIsActiveValue = null;

  public filterScheduleDateError: boolean = false;
  public filterScheduleDateErrorMessage: string = "";
  public filterStartDateError: boolean = false;
  public filterStartDateErrorMessage: string = "";
  public filterEndDateError: boolean = false;
  public filterEndDateErrorMessage: string = "";

  closeFilter() {
    this.isFilter = !this.isFilter;
  }

  constructor(private injector: Injector, private _messageDialogService: MessageDialogService,
    private router: Router, private localstorageservice: LocalStorageService,
    private fb: FormBuilder, private dataHubService: DataHubService,
    private productService: ProductService) {
    this.sortedEtlScheduleList = this.etlScheduleList.slice();
  }

  public DateFormat;

  //method called on initialization
  ngOnInit() {
    var userClaimsDetail = JSON.parse(localStorage.getItem('userClaims'));
    if (userClaimsDetail) {
      this.userClaimsRolePrivilegeOperations = userClaimsDetail.Privileges;
    }
    else {
      this.localstorageservice.removeLocalStorageData();
      this.router.navigate(['login']);
    }

    this.DateFormat = localStorage.getItem('DateFormat');
    this.getETLSchedules(null);
    this.getProducts();
    this.ETLScheduleFilterForm = this.fb.group({
      filterProduct: [null],
      filterName: [null],
      filterScheduleDate: [null],
      filterStartDate: [null],
      filterEndDate: [null],
      filterDayOfMonth: [null],
      filterIsActive: [null]
    });
    this.ETLScheduleFilterForm.controls['filterIsActive'].setValue(0);
    this.ETLScheduleFilterForm.controls['filterProduct'].setValue(0);
  }

  getProducts() {
    this.productService.getProducts().subscribe(data => {
      this.productList = data;
    });
  }

  ngAfterViewInit() {
    this.dataSource.paginator = this.paginator;
  }

  public handlePage(e: any) {
    this.currentPage = e.pageIndex;
    this.pageSize = e.pageSize;
    //this.iterator();
    this.getETLSchedules(null);
  }

  //Getters for ETL Schedule Forms
  get filterProduct() {
    return this.ETLScheduleFilterForm.get('filterProduct');
  }

  //Getters for ETL Schedule Forms
  get filterName() {
    return this.ETLScheduleFilterForm.get('filterName');
  }

  //Getters for ETL Schedule Forms
  get filterScheduleDate() {
    return this.ETLScheduleFilterForm.get('filterScheduleDate');
  }

  //Getters for ETL Schedule Forms
  get filterStartDate() {
    return this.ETLScheduleFilterForm.get('filterStartDate');
  }

  //Getters for ETL Schedule Forms
  get filterEndDate() {
    return this.ETLScheduleFilterForm.get('filterEndDate');
  }

  //Getters for ETL Schedule Forms
  get filterIsActive() {
    return this.ETLScheduleFilterForm.get('filterIsActive');
  }

  //Getters for ETL Schedule Forms
  get filterDayOfMonth() {
    return this.ETLScheduleFilterForm.get('filterDayOfMonth');
  }

  //To reset field of ETL Schedule filter form
  resetETLScheduleFilterForm() {
    this.ETLScheduleFilterForm.patchValue({
      filterProduct: null,
      filterName: null,
      filterScheduleDate: null,
      filterStartDate: null,
      filterEndDate: null,
      filterDayOfMonth: null,
      filterIsActive: null,
    });
    this.ETLScheduleFilterForm.controls['filterIsActive'].setValue(0);
    this.ETLScheduleFilterForm.controls['filterProduct'].setValue(0);

    this.currentPage = 0;
    this.filterProductValue = 0;
    this.filterNameValue = '';
    this.filterScheduleDateValue = null;
    this.filterScheduleStartDateValue = null;
    this.filterScheduleEndDateValue = null;
    this.filterStartDateError = false;
    this.filterEndDateError = false;
    this.filterIsActiveValue = null;
  }

  validateFilterDate(): boolean {
    if (this.ETLScheduleFilterForm.value.filterStartDate != null && this.ETLScheduleFilterForm.value.filterStartDate != '' &&
      this.ETLScheduleFilterForm.value.filterEndDate != null && this.ETLScheduleFilterForm.value.filterEndDate != '') {
      let startDate = this.ETLScheduleFilterForm.value.filterStartDate;
      let toDate = this.ETLScheduleFilterForm.value.filterEndDate;
      if (startDate.getTime() > toDate.getTime()) {
        this.filterStartDateError = true;
        return false;
      }
    }
    return true;
  }

  //To check End Date is bigger than Start Date
  onFilterDateChange(event) {
    this.filterStartDateError = false;
    this.filterEndDateError = false;
    this.filterStartDateErrorMessage = "";
    this.filterEndDateErrorMessage = "";

    if (this.ETLScheduleFilterForm.value.filterStartDate != null && this.ETLScheduleFilterForm.value.filterStartDate != '' &&
      this.ETLScheduleFilterForm.value.filterEndDate != null && this.ETLScheduleFilterForm.value.filterEndDate != '') {
      let startDate = this.ETLScheduleFilterForm.value.filterStartDate;
      let toDate = this.ETLScheduleFilterForm.value.filterEndDate;
      if (startDate.getTime() > toDate.getTime()) {
        this.filterStartDateError = true;
        this.filterStartDateErrorMessage = ErrorMessageConstants.getStartDateLessThanEndDateMessage;
      }
    }
  }
  
  //This method has been used for fetching search records
  searchFilter(searchType) {
    this.filterStartDateError = false;
    this.isFilterDone = true;
    if (searchType == 'reset') {
      this.resetETLScheduleFilterForm();
      this.getETLSchedules(null);
      this.isFilter = !this.isFilter;
    }
    else {
      let searchParameter: any = {};
      searchParameter.IsDeleted = false;
      searchParameter.PagingParameter = {};
      searchParameter.PagingParameter.PageIndex = 1;
      searchParameter.PagingParameter.PageSize = this.pageSize;
      searchParameter.SortParameter = {};
      searchParameter.SortParameter.SortColumn = this.sortColumn;
      searchParameter.SortParameter.SortOrder = this.sortOrder;
      searchParameter.SearchMode = Constants.Contains;

      this.filterProductValue = (this.ETLScheduleFilterForm.value.filterProduct != null && this.ETLScheduleFilterForm.value.filterProduct != '' && this.ETLScheduleFilterForm.value.filterProduct != '0') ? this.ETLScheduleFilterForm.value.filterProduct : 0;
      this.filterNameValue = (this.ETLScheduleFilterForm.value.filterName != null && this.ETLScheduleFilterForm.value.filterName != '') ? this.ETLScheduleFilterForm.value.filterName.trim() : "";
      this.filterScheduleDateValue = (this.ETLScheduleFilterForm.value.filterScheduleDate != null && this.ETLScheduleFilterForm.value.filterScheduleDate != '') ? new Date(this.ETLScheduleFilterForm.value.filterScheduleDate.setHours(23, 59, 59)) : null;
      this.filterScheduleStartDateValue = (this.ETLScheduleFilterForm.value.filterStartDate != null && this.ETLScheduleFilterForm.value.filterStartDate != '') ? new Date(this.ETLScheduleFilterForm.value.filterStartDate.setHours(23, 59, 59)) : null;
      this.filterScheduleEndDateValue = (this.ETLScheduleFilterForm.value.filterEndDate != null && this.ETLScheduleFilterForm.value.filterEndDate != '') ? new Date(this.ETLScheduleFilterForm.value.filterEndDate.setHours(23, 59, 59)) : null;

      const IsActivevalue = this.ETLScheduleFilterForm.controls['filterIsActive'].value;
      if (IsActivevalue == "0") {
        this.filterIsActiveValue = null;
      }
      else if (IsActivevalue == "1") {
        this.filterIsActiveValue = true;
      }
      else if (IsActivevalue == "2") {
        this.filterIsActiveValue = false;
      }

      //this.filterIsActiveValue = (this.ETLScheduleFilterForm.value.filterIsActive != null && this.ETLScheduleFilterForm.value.filterIsActive != '') ?  this.ETLScheduleFilterForm.value.filterIsActive : null;

      searchParameter.ProductId = this.filterProductValue
      searchParameter.Name = this.filterNameValue;
      searchParameter.ScheduleDate = this.filterScheduleDateValue;
      searchParameter.StartDate = this.filterScheduleStartDateValue;
      searchParameter.EndDate = this.filterScheduleEndDateValue;
      searchParameter.IsActive = this.filterIsActiveValue;
      this.currentPage = 0;

      this.getETLSchedules(searchParameter);
      this.isFilter = !this.isFilter;
    }
  }

  //This method has been used for fetching Data Hub ETL-Schedules records
  async getETLSchedules(searchParameter) {
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

    if (this.filterProductValue != null && this.filterProductValue != 0) {
      searchParameter.ProductId = this.filterProductValue;
    }
    if (this.filterNameValue != null && this.filterNameValue != '') {
      searchParameter.Name = this.filterNameValue.trim();
    }
    if (this.filterScheduleDateValue != null && this.filterScheduleDateValue != '') {
      searchParameter.ScheduleDate = new Date(this.filterScheduleDateValue.setHours(23, 59, 59));
    }
    if (this.filterScheduleStartDateValue != null && this.filterScheduleStartDateValue != '') {
      searchParameter.StartDate = new Date(this.filterScheduleStartDateValue.setHours(23, 59, 59));
    }
    if (this.filterScheduleEndDateValue != null && this.filterScheduleEndDateValue != '') {
      searchParameter.EndDate = new Date(this.filterScheduleEndDateValue.setHours(23, 59, 59));
    }
    if (this.filterIsActiveValue != null && this.filterIsActiveValue != '') {
      searchParameter.isActive = this.filterIsActiveValue;
    }

    var response = await dataHubService.getETLSchedules(searchParameter);
    this.etlScheduleList = response.List;
    this.totalRecordCount = response.RecordCount;
    if (this.etlScheduleList.length == 0 && this.isFilterDone == true) {
      let message = ErrorMessageConstants.getNoRecordFoundMessage;
      this._messageDialogService.openDialogBox('Error', message, Constants.msgBoxError).subscribe(data => {
        if (data == true) {
          this.resetETLScheduleFilterForm();
          this.getETLSchedules(null);
        }
      });
    }
    this.dataSource = new MatTableDataSource<ETLSchedule>(this.etlScheduleList);
    this.dataSource.sort = this.sort;
    this.array = this.etlScheduleList;
    this.totalSize = this.totalRecordCount;
  }

  //To sort table data using column
  sortData(sort: MatSort) {
    const data = this.etlScheduleList.slice();
    if (!sort.active || sort.direction === '') {
      this.sortedEtlScheduleList = data;
      return;
    }

    if (sort.direction == 'asc') {
      this.sortOrder = Constants.Ascending;
    } else {
      this.sortOrder = Constants.Descending;
    }

    switch (sort.active) {
      case 'product': this.sortColumn = "Product"; break;
      case 'name': this.sortColumn = "Name"; break;
      case 'scheduleDate': this.sortColumn = "ScheduleDate"; break;
      case 'startDate': this.sortColumn = "StartDate"; break;
      case 'endDate': this.sortColumn = "EndDate"; break;
      case 'DayOfMonth': this.sortColumn = "DayOfMonth"; break;
      case 'IsActive': this.sortColumn = "IsActive"; break;
      default: this.sortColumn = "LastUpdatedDate"; break;
    }

    let searchParameter: any = {};
    searchParameter.PagingParameter = {};
    searchParameter.PagingParameter.PageIndex = this.currentPage + 1;
    searchParameter.PagingParameter.PageSize = this.pageSize;
    searchParameter.SortParameter = {};
    searchParameter.SortParameter.SortColumn = this.sortColumn;
    searchParameter.SortParameter.SortOrder = this.sortOrder;
    searchParameter.SearchMode = Constants.Contains;

    this.filterProductValue = (this.ETLScheduleFilterForm.value.filterProduct != null && this.ETLScheduleFilterForm.value.filterProduct != '' && this.ETLScheduleFilterForm.value.filterProduct != '0') ? this.ETLScheduleFilterForm.value.filterProduct : 0;
    this.filterNameValue = (this.ETLScheduleFilterForm.value.filterName != null && this.ETLScheduleFilterForm.value.filterName != '') ? this.ETLScheduleFilterForm.value.filterName.trim() : "";
    this.filterScheduleDateValue = (this.ETLScheduleFilterForm.value.filterScheduleDate != null && this.ETLScheduleFilterForm.value.filterScheduleDate != '') ? new Date(this.ETLScheduleFilterForm.value.filterScheduleDate.setHours(23, 59, 59)) : null;
    this.filterScheduleStartDateValue = (this.ETLScheduleFilterForm.value.filterStartDate != null && this.ETLScheduleFilterForm.value.filterStartDate != '') ? new Date(this.ETLScheduleFilterForm.value.filterStartDate.setHours(23, 59, 59)) : null;
    this.filterScheduleEndDateValue = (this.ETLScheduleFilterForm.value.filterEndDate != null && this.ETLScheduleFilterForm.value.filterEndDate != '') ? new Date(this.ETLScheduleFilterForm.value.filterEndDate.setHours(23, 59, 59)) : null;
    
    const IsActivevalue = this.ETLScheduleFilterForm.controls['filterIsActive'].value;
    if (IsActivevalue == "0") {
      this.filterIsActiveValue = null;
    }
    else if (IsActivevalue == "1") {
      this.filterIsActiveValue = true;
    }
    else if (IsActivevalue == "2") {
      this.filterIsActiveValue = false;
    }

    searchParameter.ProductId = this.filterProductValue;
    searchParameter.Name = this.filterNameValue;
    searchParameter.ScheduleDate = this.filterScheduleDateValue;
    searchParameter.StartDate = this.filterScheduleStartDateValue;
    searchParameter.EndDate = this.filterScheduleEndDateValue;
    searchParameter.IsActive = this.filterIsActiveValue;

    this.getETLSchedules(searchParameter);
  }

  public onActiveStatusSelected(event) {
    const value = event.target.value;
    if (value == "0") {
      this.filterIsActiveValue = null;
    }
    else if (value == "1") {
      this.filterIsActiveValue = true;
    }
    else if (value == "2") {
      this.filterIsActiveValue = false;
    }
  }

  //this method helps to navigate to  ETL Schedule Detail Page
  navigateToETLScheduleDetail(eTLSchedule: ETLSchedule) {    
    let queryParams = {
      Routeparams: {
        passingparams: {
          "ProductBatchId": eTLSchedule.ProductBatchId,
        },
        filteredparams: {
          //passing data using json stringify.
          "Name": this.ETLScheduleFilterForm.value.filterRoleName != null ? this.ETLScheduleFilterForm.value.filterRoleName : ""
        }
      }
    }
    localStorage.setItem("eTLScheduleParams", JSON.stringify(queryParams))

    const router = this.injector.get(Router);
    router.navigate(['datahub', 'etlscheduledetail']);
  }

}
