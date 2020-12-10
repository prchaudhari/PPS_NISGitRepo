import { Component, OnInit, Injector, ViewChild } from '@angular/core';
import * as Highcharts from 'highcharts';
import { DashboardReportService } from './dashboard.service';
import { Constants } from '../../shared/constants/constants';
import { ErrorMessageConstants } from '../../shared/constants/constants';
import { MessageDialogService } from '../../shared/services/mesage-dialog.service';
import { LocalStorageService } from '../../shared/services/local-storage.service';
import { FormGroup, FormBuilder } from '@angular/forms';
import { DatePipe } from '@angular/common';
import { TenantService } from '../tenants/tenant.service';
import { Tenant } from '../tenants/tenant';

declare var require: any;
let Boost = require('highcharts/modules/boost');
let noData = require('highcharts/modules/no-data-to-display');
let More = require('highcharts/highcharts-more');

Boost(Highcharts);
noData(Highcharts);
More(Highcharts);
noData(Highcharts);

@Component({
  selector: 'app-instancemanagerdashboard',
  templateUrl: './instancemanagerdashboard.component.html',
  styleUrls: ['./instancemanagerdashboard.component.scss']
})
export class InstancemanagerdashboardComponent implements OnInit {

  public isFilter: boolean = false;
  public isFilterDone = false;
  public pieChartData = [];
  public pieChart;
  public reportData: any = {};
  public AnalyticFilterForm: FormGroup;
  public filterFromDateError: boolean = false;
  public filterFromDateErrorMessage: string = "";
  public filterToDateError: boolean = false;
  public filterToDateErrorMessage: string = "";
  public filterVisitorDateError: boolean = false;
  public filterVisitorDateErrorMessage: string = "";

  public tenantgroupList: any[] = [{ "TenantName": "Select Tenant Group", "TenantCode": 0 }];
  public sortOrder = Constants.Descending;
  constructor(private injector: Injector,
    private _messageDialogService: MessageDialogService, private datePipe: DatePipe,
    private fb: FormBuilder) {
  }

  ngOnInit() {
    var fromDate = new Date(Date.now() - 30 * 24 * 60 * 60 * 1000)
    var toDate = new Date();
    var visitorDate = new Date(Date.now() - 1 * 24 * 60 * 60 * 1000)

    this.AnalyticFilterForm = this.fb.group({
      filterFromDate: [fromDate],
      filterToDate: [toDate],
    });
    var searchParameter: any = {};
    searchParameter.PagingParameter = {};
    searchParameter.PagingParameter.PageIndex = Constants.DefaultPageIndex;
    searchParameter.PagingParameter.PageSize = Constants.DefaultPageSize;
    searchParameter.SortParameter = {};
    searchParameter.SortParameter.SortColumn = 'Id';
    searchParameter.SortParameter.SortOrder = Constants.Descending;
    searchParameter.SearchMode = Constants.Contains;
    searchParameter.StartDate = new Date(fromDate.setHours(0, 0, 0));
    searchParameter.EndDate = new Date(toDate.setHours(23, 59, 59));

    this.getDashboardReports(null);
  }
  get filterTenantGroup() {
    return this.AnalyticFilterForm.get('filterTenantGroup');
  }
  closeFilter() {
    this.isFilter = !this.isFilter;
  }

  ngAfterViewInit() {
    if (document.getElementById('pieChartcontainer') != null) {
      this.pieChart = Highcharts.chart('pieChartcontainer', this.PieChartOptions);
    }
  }

  searchDashboardReportRecordFilter(searchType) {
    this.isFilterDone = true;
    if (searchType == 'reset') {
      this.resetPageFilterForm();
      this.getDashboardReports(null);
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

        if (this.AnalyticFilterForm.value.filterFromDate != null && this.AnalyticFilterForm.value.filterFromDate != '') {
          //searchParameter.StartDate = this.ScheduleFilterForm.value.filterStartDate;
          searchParameter.StartDate = new Date(this.AnalyticFilterForm.value.filterFromDate.setHours(0, 0, 0));
        }
        if (this.AnalyticFilterForm.value.filterToDate != null && this.AnalyticFilterForm.value.filterToDate != '') {
          //searchParameter.EndDate = this.ScheduleFilterForm.value.filterEndDate;
          searchParameter.EndDate = new Date(this.AnalyticFilterForm.value.filterToDate.setHours(23, 59, 59));
        }

        this.getDashboardReports(searchParameter);
        this.isFilter = !this.isFilter;
      }
    }
  }

  validateFilterDate(): boolean {
    if (this.AnalyticFilterForm.value.filterFromDate != null && this.AnalyticFilterForm.value.filterFromDate != '' &&
      this.AnalyticFilterForm.value.filterToDate != null && this.AnalyticFilterForm.value.filterToDate != '') {
      let startDate = this.AnalyticFilterForm.value.filterFromDate;
      let toDate = this.AnalyticFilterForm.value.filterToDate;
      if (startDate.getTime() > toDate.getTime()) {
        this.filterFromDateError = true;
        return false;
      }
    }
    return true;
  }
  resetPageFilterForm() {
    var fromDate = new Date(Date.now() - 30 * 24 * 60 * 60 * 1000)
    var toDate = new Date();
    var visitorDate = new Date(Date.now() - 1 * 24 * 60 * 60 * 1000)

    this.AnalyticFilterForm = this.fb.group({
      filterFromDate: [fromDate],
      filterToDate: [toDate],
    });

    var searchParameter: any = {};
    searchParameter.PagingParameter = {};
    searchParameter.PagingParameter.PageIndex = Constants.DefaultPageIndex;
    searchParameter.PagingParameter.PageSize = Constants.DefaultPageSize;
    searchParameter.SortParameter = {};
    searchParameter.SortParameter.SortColumn = 'Id';
    searchParameter.SortParameter.SortOrder = Constants.Descending;
    searchParameter.SearchMode = Constants.Contains;
    searchParameter.StartDate = new Date(fromDate.setHours(0, 0, 0));
    searchParameter.EndDate = new Date(toDate.setHours(23, 59, 59));

    this.getDashboardReports(searchParameter);
    this.filterFromDateError = false;
    this.filterFromDateErrorMessage = "";
    this.filterToDateError = false;
    this.filterToDateErrorMessage = "";
    this.filterVisitorDateError = false;
    this.filterVisitorDateErrorMessage = "";
  }

  async getTenantGroups(searchParameter) {
    let tenantService = this.injector.get(TenantService);
    if (searchParameter == null) {
      searchParameter = {};
      searchParameter.IsActive = true;
      searchParameter.PagingParameter = {};
      searchParameter.SortParameter = {};
      searchParameter.SortParameter.SortColumn = "TenantCode";
      searchParameter.SortParameter.SortOrder = this.sortOrder;
      searchParameter.SearchMode = Constants.Contains;
    }

    searchParameter.IsPrimaryTenant = false;
    searchParameter.IsCountryRequired = true;
    searchParameter.TenantType = "Group";
    var response = await tenantService.getTenant(searchParameter);
    response.List.forEach(item => {
      this.tenantgroupList.push(item);
    })
    if (this.tenantgroupList.length == 0 && this.isFilterDone == true) {
      let message = ErrorMessageConstants.getNoRecordFoundMessage;
      this._messageDialogService.openDialogBox('Error', message, Constants.msgBoxError).subscribe(data => {
        if (data == true) {
          this.getTenantGroups(null);
        }
      });
    }

    //this.iterator();
  }

  async getDashboardReports(searchParameter) {
    let service = this.injector.get(DashboardReportService);
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
    searchParameter.IsInstanceManager = true;

    var data: any = {};
    data = await service.getDashboardReport(searchParameter);
    this.reportData = data.List;

    this.PieChartOptions.series = this.reportData.PublishedStatementByGroup.series;
    this.PieChartOptions.title = this.reportData.PublishedStatementByGroup.title;
    Highcharts.chart('pieChartcontainer', this.PieChartOptions);

    this.UserChartOptions.xAxis.categories = this.reportData.UsersByGroup.xAxis;
    this.UserChartOptions.series = this.reportData.UsersByGroup.series;
    this.UserChartOptions.title = this.reportData.UsersByGroup.title;
    Highcharts.chart('userChartcontainer', this.UserChartOptions);

    this.GeneratedStmtChartOptions.xAxis.categories = this.reportData.StatementByGroup.xAxis;
    this.GeneratedStmtChartOptions.series = this.reportData.StatementByGroup.series;
    this.GeneratedStmtChartOptions.title = this.reportData.StatementByGroup.title;
    Highcharts.chart('generatedStmtChartcontainer', this.GeneratedStmtChartOptions);

  }

  get filterFromDate() {
    return this.AnalyticFilterForm.get('filterFromDate');
  }

  get filterToDate() {
    return this.AnalyticFilterForm.get('filterToDate');
  }

  disableSeacrhButton() {
    if (this.AnalyticFilterForm.value.filterFromDate === null || this.AnalyticFilterForm.value.filterFromDate == '') {
      return true;
    }
    if (this.AnalyticFilterForm.value.filterToDate === null || this.AnalyticFilterForm.value.filterToDate == '') {
      return true;
    }
    if (this.filterToDateError || this.filterFromDateError) {
      return true;
    }
    return false;
  }

  onPublishedFilterDateChange(event) {
    this.filterFromDateError = false;
    this.filterToDateError = false;
    this.filterFromDateErrorMessage = "";
    this.filterToDateErrorMessage = "";
    let currentDte = new Date();
    if (this.AnalyticFilterForm.value.filterFromDate != null && this.AnalyticFilterForm.value.filterFromDate != '') {
      let startDate = this.AnalyticFilterForm.value.filterFromDate;
      if (startDate.getTime() > currentDte.getTime()) {
        this.filterFromDateError = true;
        this.filterFromDateErrorMessage = ErrorMessageConstants.getStartDateLessThanCurrentDateMessage;
      }
    }
    if (this.AnalyticFilterForm.value.filterToDate != null && this.AnalyticFilterForm.value.filterToDate != '') {
      let toDate = this.AnalyticFilterForm.value.filterToDate;
      if (toDate.getTime() > currentDte.getTime()) {
        this.filterToDateError = true;
        this.filterToDateErrorMessage = ErrorMessageConstants.getEndDateLessThanCurrentDateMessage;
      }
    }
    if (this.AnalyticFilterForm.value.filterFromDate != null && this.AnalyticFilterForm.value.filterFromDate != '' &&
      this.AnalyticFilterForm.value.filterToDate != null && this.AnalyticFilterForm.value.filterToDate != '') {
      let startDate = this.AnalyticFilterForm.value.filterFromDate;
      let toDate = this.AnalyticFilterForm.value.filterToDate;
      if (startDate.getTime() > toDate.getTime()) {
        this.filterFromDateError = true;
        this.filterFromDateErrorMessage = ErrorMessageConstants.getStartDateLessThanEndDateMessage;
      }
    }
  }
  public UserChartOptions: any = {
    xAxis: {
    },
  }

  public GeneratedStmtChartOptions: any = {
    xAxis: {
    },
  }

  public PieChartOptions: any = {

    chart: {

      type: 'pie'
    },

    tooltip: {
      pointFormat: '{series.name}: <b>{point.percentage:.1f}%</b>'
    },

    plotOptions: {

      pie: {
        allowPointSelect: true,
        cursor: 'pointer',
        dataLabels: {
          enabled: true,
          format: '<b>{point.name}</b>: {point.percentage:.1f} %'
        },
        showInLegend: false
      }
    },

  }
}
