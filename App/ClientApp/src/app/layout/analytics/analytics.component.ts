import { Component, OnInit, Injector, ViewChild } from '@angular/core';
import * as Highcharts from 'highcharts';
import { SourceDataService } from '../sourcedata/sourcedata.service';
import { SourceData, VisitorForDay, DatewiseVisitor } from '../sourcedata/sourcedata';
import { Constants } from '../../shared/constants/constants';
import { ErrorMessageConstants } from '../../shared/constants/constants';
import { MessageDialogService } from '../../shared/services/mesage-dialog.service';
import { LocalStorageService } from '../../shared/services/local-storage.service';
import { FormGroup, FormBuilder } from '@angular/forms';
import { TemplateService } from '../template/template.service';
declare var require: any;
let Boost = require('highcharts/modules/boost');
let noData = require('highcharts/modules/no-data-to-display');
let More = require('highcharts/highcharts-more');

Boost(Highcharts);
noData(Highcharts);
More(Highcharts);
noData(Highcharts);


@Component({
  selector: 'app-analytics',
  templateUrl: './analytics.component.html',
  styleUrls: ['./analytics.component.scss']
})

export class AnalyticsComponent implements OnInit {
  public isFilter: boolean = false;
  public AnalyticFilterForm: FormGroup;
  public PageWidgetVisitorForm: FormGroup;
  public VisitorDayForm: FormGroup;

  public filterFromDateError: boolean = false;
  public filterFromDateErrorMessage: string = "";
  public filterToDateError: boolean = false;
  public filterToDateErrorMessage: string = "";
  public filterVisitorDateError: boolean = false;
  public filterVisitorDateErrorMessage: string = "";
  public pageTypeList = [];

  closeFilter() {
    this.isFilter = !this.isFilter;
  }

  ngOnInit() {
    var fromDate = new Date(Date.now() - 7 * 24 * 60 * 60 * 1000) 
    var toDate = new Date();
  //  toDate=toDate.
    var visitorDate = new Date(Date.now() - 1 * 24 * 60 * 60 * 1000) 
    console.log(fromDate);
    console.log(toDate);

    this.AnalyticFilterForm = this.fb.group({

      filterFromDate: [fromDate],
      filterToDate: [toDate],
    });
    this.VisitorDayForm = this.fb.group({
      visitorDate: [visitorDate],
    });
    this.PageWidgetVisitorForm = this.fb.group({
      pageType: [0]
    });
    var searchParameter :any= {};
    searchParameter.PagingParameter = {};
    searchParameter.PagingParameter.PageIndex = Constants.DefaultPageIndex;
    searchParameter.PagingParameter.PageSize = Constants.DefaultPageSize;
    searchParameter.SortParameter = {};
    searchParameter.SortParameter.SortColumn = 'Id';
    searchParameter.SortParameter.SortOrder = Constants.Descending;
    searchParameter.SearchMode = Constants.Contains;
    searchParameter.StartDate = new Date(fromDate.setHours(0, 0, 0));
    searchParameter.EndDate = new Date(toDate.setHours(23, 59, 59));

    this.getSourceDatas(searchParameter);
    var visitorSearchParameter: any = {};
    visitorSearchParameter.PagingParameter = {};
    visitorSearchParameter.PagingParameter.PageIndex = Constants.DefaultPageIndex;
    visitorSearchParameter.PagingParameter.PageSize = Constants.DefaultPageSize;
    visitorSearchParameter.SortParameter = {};
    visitorSearchParameter.SortParameter.SortColumn = 'Id';
    visitorSearchParameter.SortParameter.SortOrder = Constants.Descending;
    visitorSearchParameter.SearchMode = Constants.Contains;
    visitorSearchParameter.StartDate = new Date(visitorDate.setHours(0, 0, 0));
    visitorSearchParameter.EndDate = new Date(visitorDate.setHours(23, 59, 59));
    this.BindVisitorForDay(visitorSearchParameter);
  }

  ngAfterViewInit() {
    //Highcharts.chart('chartDatewisecontainer', this.options);
    //Highcharts.chart('chartDaywisecontainer', this.options2);
    //Highcharts.chart('chartPageWidgetwisecontainer', this.options3);
    //Highcharts.chart('chartWidgetPiecontainer', this.options4);
    this.getPageTypes();
  }

  async getPageTypes() {
    let templateService = this.injector.get(TemplateService);
    this.pageTypeList = [{ "Identifier": 0, "PageTypeName": "Select Page Type" }];
    let list = await templateService.getPageTypes();
    if (this.pageTypeList.length == 0) {
      let message = ErrorMessageConstants.getNoRecordFoundMessage;
      this._messageDialogService.openDialogBox('Error', message, Constants.msgBoxError).subscribe(data => {
        if (data == true) {
          this.getPageTypes();
        }
      });
    } else {
      this.pageTypeList = [...this.pageTypeList, ...list];
    }
  }

  get filterFromDate() {
    return this.AnalyticFilterForm.get('filterFromDate');
  }

  get pageType() {
    return this.PageWidgetVisitorForm.get('pageType');
  }

  get visitorDate() {
    return this.VisitorDayForm.get('visitorDate');
  }

  get filterToDate() {
    return this.AnalyticFilterForm.get('filterToDate');
  }

  public onPageTypeSelected(event) {
    var searchParameter: any = {};
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
    if (this.PageWidgetVisitorForm.value.pageType == "0") {
      searchParameter.PageTypeName = "";
    }
    else {
      var page = this.pageTypeList.filter(i => i.Identifier.toString() == this.PageWidgetVisitorForm.value.pageType);
      searchParameter.PageTypeName = page[0].PageTypeName;

    }
    this.BindVisitorPageWidgetCount(searchParameter);
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

  searchSourceDataRecordFilter(searchType) {
    this.filterFromDateError = false;
    this.isFilterDone = true;
    if (searchType == 'reset') {
      this.resetPageFilterForm();
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

        if (this.AnalyticFilterForm.value.filterFromDate != null && this.AnalyticFilterForm.value.filterFromDate != '') {
          //searchParameter.StartDate = this.ScheduleFilterForm.value.filterStartDate;
          searchParameter.StartDate = new Date(this.AnalyticFilterForm.value.filterFromDate.setHours(0, 0, 0));
        }
        if (this.AnalyticFilterForm.value.filterToDate != null && this.AnalyticFilterForm.value.filterToDate != '') {
          //searchParameter.EndDate = this.ScheduleFilterForm.value.filterEndDate;
          searchParameter.EndDate = new Date(this.AnalyticFilterForm.value.filterToDate.setHours(23, 59, 59));
        }

        console.log(searchParameter);
        this.getSourceDatas(searchParameter);
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

  onVisistorDateSelection(event) {
    this.filterVisitorDateError = false;
    this.filterVisitorDateErrorMessage = "";
    if (this.VisitorDayForm.value.visitorDate != null && this.VisitorDayForm.value.visitorDate != '') {
      let toDate = this.VisitorDayForm.value.visitorDate;
      let currentDte = new Date();
      if (toDate.getTime() > currentDte.getTime()) {
        this.filterVisitorDateError = true;
        this.filterVisitorDateErrorMessage = ErrorMessageConstants.getEndDateLessThanCurrentDateMessage;
      }
      
    }
    if (!this.filterVisitorDateError) {
      var visitorSearchParameter: any = {};
      visitorSearchParameter.PagingParameter = {};
      visitorSearchParameter.PagingParameter.PageIndex = Constants.DefaultPageIndex;
      visitorSearchParameter.PagingParameter.PageSize = Constants.DefaultPageSize;
      visitorSearchParameter.SortParameter = {};
      visitorSearchParameter.SortParameter.SortColumn = 'Id';
      visitorSearchParameter.SortParameter.SortOrder = Constants.Descending;
      visitorSearchParameter.SearchMode = Constants.Contains;
      visitorSearchParameter.StartDate = new Date(this.VisitorDayForm.value.visitorDate.setHours(0, 0, 0));
      visitorSearchParameter.EndDate = new Date(this.VisitorDayForm.value.visitorDate.setHours(23, 59, 59));
      this.BindVisitorForDay(visitorSearchParameter);
    }
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

  resetPageFilterForm() {
    var fromDate = new Date(Date.now() - 7 * 24 * 60 * 60 * 1000)
    var toDate = new Date();
    //  toDate=toDate.
    var visitorDate = new Date(Date.now() - 1 * 24 * 60 * 60 * 1000)
    console.log(fromDate);
    console.log(toDate);

    this.AnalyticFilterForm = this.fb.group({

      filterFromDate: [fromDate],
      filterToDate: [toDate],
    });
    this.VisitorDayForm = this.fb.group({
      visitorDate: [visitorDate],
    });
    this.PageWidgetVisitorForm = this.fb.group({
      pageType: [0]
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

    this.getSourceDatas(searchParameter);
    var visitorSearchParameter: any = {};
    visitorSearchParameter.PagingParameter = {};
    visitorSearchParameter.PagingParameter.PageIndex = Constants.DefaultPageIndex;
    visitorSearchParameter.PagingParameter.PageSize = Constants.DefaultPageSize;
    visitorSearchParameter.SortParameter = {};
    visitorSearchParameter.SortParameter.SortColumn = 'Id';
    visitorSearchParameter.SortParameter.SortOrder = Constants.Descending;
    visitorSearchParameter.SearchMode = Constants.Contains;
    visitorSearchParameter.StartDate = new Date(visitorDate.setHours(0, 0, 0));
    visitorSearchParameter.EndDate = new Date(visitorDate.setHours(23, 59, 59));
    this.BindVisitorForDay(visitorSearchParameter);

    this.filterVisitorDateError = false;
    this.filterVisitorDateErrorMessage = "";
    this.filterFromDateError = false;
    this.filterToDateError = false;
    this.filterFromDateErrorMessage = "";
    this.filterToDateErrorMessage = "";
  }

  public sourceDataList: SourceData[] = [];
  public visitorData: VisitorForDay;
  public DateWiseVisitor: DatewiseVisitor;
  //Datewise Visitor for Pages
  public options: any = {}
  public isFilterDone = false;
  public pieChartData = [];

  //Visitor for days
  public options2: any = {
    chart: {
      type: 'column'
    },
    title: {
      text: ''
    },
    subtitle: {
      text: ''
    },
    xAxis: {
      title: {
        text: 'Date'
      },
      categories: [
        '09:00',
        '10:00',
        '11:00',
        '12:00',
        '13:00',
        '14:00',
      ],
      crosshair: true
    },
    yAxis: {
      min: 0,
      title: {
        text: 'Visitor Count'
      }
    },
    tooltip: {
      headerFormat: '<span style="font-size:10px">{point.key}</span><table>',
      pointFormat: '<tr><td style="color:{series.color};padding:0">{series.name}: </td>' +
        '<td style="padding:0"><b>{point.y:.1f}</b></td></tr>',
      footerFormat: '</table>',
      shared: true,
      useHTML: true
    },
    plotOptions: {
      column: {
        pointPadding: 0.2,
        borderWidth: 0
      }
    },
    series: [{
      name: 'Count',
      data: [14, 13, 21, 45, 28, 29]

    }]
  }

  //Visitor for Page Widget
  public options3: any = {};

  //Widget Visitors for day
  public pieChartOptions: any;

  constructor(private injector: Injector,
    private _messageDialogService: MessageDialogService,
    private fb: FormBuilder) {
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
    //this.sourceDataList = await scheduleLogService.getSourceData(searchParameter);
    //if (this.sourceDataList.length == 0 && this.isFilterDone == true) {
    //  let message = ErrorMessageConstants.getNoRecordFoundMessage;
    //  this._messageDialogService.openDialogBox('Error', message, Constants.msgBoxError).subscribe(data => {
    //    if (data == true) {
    //      //this.resetRoleFilterForm();
    //      this.getSourceDatas(null);
    //    }
    //  });
    //}
    //else {
    this.BindPieChart(searchParameter);
    this.BindVisitorPageWidgetCount(searchParameter);
    //this.BindVisitorForDay(searchParameter);
    this.BindVisitorForDate(searchParameter);

    // }
  }

  async BindPieChart(searchParameter) {
    let scheduleLogService = this.injector.get(SourceDataService);
    this.pieChartData = await scheduleLogService.getWidgetVisitorPieChartData(searchParameter);

    this.pieChartOptions = {
      chart: {
        plotBackgroundColor: null,
        plotBorderWidth: null,
        plotShadow: false,
        type: 'pie'
      },
      title: {
        text: ''
      },
      tooltip: {
        pointFormat: '{series.name}: <b>{point.percentage:.1f}%</b>'
      },
      accessibility: {
        point: {
          valueSuffix: '%'
        }
      },
      plotOptions: {
        pie: {
          allowPointSelect: true,
          cursor: 'pointer',
          dataLabels: {
            enabled: true,
            format: '{point.percentage:.1f} %'

          },
          showInLegend: true
        }
      },
      series: [{
        name: 'Percentage',
        colorByPoint: true,
        data: this.pieChartData
      }]

    }
    Highcharts.chart('chartWidgetPiecontainer', this.pieChartOptions);
  }

  async BindVisitorPageWidgetCount(searchParameter) {
    let scheduleLogService = this.injector.get(SourceDataService);
    var data = await scheduleLogService.getPageWidgetVistorData(searchParameter);

    this.options3 = {
      chart: {
        type: 'column'
      },
      title: {
        text: ''
      },
      subtitle: {
        text: ''
      },
      xAxis: {
        title: {
          text: 'Widgets'
        },
        categories: data.widgetNames,
        crosshair: true
      },
      yAxis: {
        min: 0,
        title: {
          text: 'Visitor Count'
        }
      },
      tooltip: {
        headerFormat: '<span style="font-size:10px">{point.key}</span><table>',
        pointFormat: '<tr><td style="color:{series.color};padding:0">{series.name}: </td>' +
          '<td style="padding:0"><b>{point.y:.1f}</b></td></tr>',
        footerFormat: '</table>',
        shared: true,
        useHTML: true
      },
      plotOptions: {
        column: {
          pointPadding: 0.2,
          borderWidth: 0
        }
      },
      series: [
        {
          name: 'Visitor Count',
          data: data.values
        }
      ]
    }
    Highcharts.chart('chartPageWidgetwisecontainer', this.options3);
  }

  async BindVisitorForDay(searchParameter) {
    let scheduleLogService = this.injector.get(SourceDataService);
    this.visitorData = await scheduleLogService.getVisitorForDay(searchParameter);
    console.log(this.visitorData);
    this.options2 = {
      chart: {
        type: 'column'
      },
      title: {
        text: ''
      },
      subtitle: {
        text: ''
      },
      xAxis: {
        title: {
          text: 'Date'
        },
        type: 'time',
        dateTimeLabelFormats: {
          day:  "%e-%b-%y",
          month:  "%b-%y",
        },
        categories: this.visitorData.time,
        crosshair: true,
      },
      yAxis: {
        min: 0,
        title: {
          text: 'Visitor Count'
        }
      },
      tooltip: {
        headerFormat: '<span style="font-size:10px">{point.key}</span><table>',
        pointFormat: '<tr><td style="color:{series.color};padding:0">{series.name}: </td>' +
          '<td style="padding:0"><b>{point.y:.1f}</b></td></tr>',
        footerFormat: '</table>',
        shared: true,
        useHTML: true
      },
      plotOptions: {
        column: {
          pointPadding: 0.2,
          borderWidth: 0
        },
       

      },
      series: this.visitorData.series
    }
    Highcharts.chart('chartDaywisecontainer', this.options2);
  }

  async BindVisitorForDate(searchParameter) {
    let scheduleLogService = this.injector.get(SourceDataService);
    this.DateWiseVisitor = await scheduleLogService.getVisitorForDate(searchParameter);
    this.options = {
      chart: {
        type: 'column'
      },
      title: {
        text: ''
      },
      subtitle: {
        text: ''
      },
      xAxis: {
        title: {
          text: 'Date'
        },
        categories: this.DateWiseVisitor.dates,
        crosshair: true
      },
      yAxis: {
        min: 0,
        title: {
          text: 'Visitor Count'
        }
      },
      tooltip: {
        headerFormat: '<span style="font-size:10px">{point.key}</span><table>',
        pointFormat: '<tr><td style="color:{series.color};padding:0">{series.name}: </td>' +
          '<td style="padding:0"><b>{point.y:.1f}</b></td></tr>',
        footerFormat: '</table>',
        shared: true,
        useHTML: true
      },
      plotOptions: {
        column: {
          pointPadding: 0.2,
          borderWidth: 0
        }
      },
      series: this.DateWiseVisitor.datewiseVisitorSeries
      //series: [{
      //  name: 'Current Account',
      //  data: [28, 29, 28, 29]

      //}, {
      //  name: 'Home',
      //  data: [37, 30, 37, 30]

      //}, {
      //  name: 'Saving Account',
      //  data: [35, 38, 35, 38]

      //}]
    }
    Highcharts.chart('chartDatewisecontainer', this.options);

  }
}
