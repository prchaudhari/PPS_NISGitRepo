import { Component, OnInit, Injector, ViewChild } from '@angular/core';
import * as Highcharts from 'highcharts';
import { SourceDataService } from '../sourcedata/sourcedata.service';
import { SourceData, VisitorForDay } from '../sourcedata/sourcedata';
import { Constants } from '../../shared/constants/constants';
import { ErrorMessageConstants } from '../../shared/constants/constants';
import { MessageDialogService } from '../../shared/services/mesage-dialog.service';
import { LocalStorageService } from '../../shared/services/local-storage.service';
import { FormGroup, FormBuilder } from '@angular/forms';
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
  public filterFromDateError: boolean = false;
  public filterFromDateErrorMessage: string = "";
  public filterToDateError: boolean = false;
  public filterToDateErrorMessage: string = "";

  closeFilter() {
    this.isFilter = !this.isFilter;
  }

  ngOnInit() {
    this.AnalyticFilterForm = this.fb.group({

      filterFromDate: [null],
      filterToDate: [null],
    });

    this.getSourceDatas(null);

  }

  ngAfterViewInit() {
    Highcharts.chart('chartDatewisecontainer', this.options);
    //Highcharts.chart('chartDaywisecontainer', this.options2);
    //Highcharts.chart('chartPageWidgetwisecontainer', this.options3);
    //Highcharts.chart('chartWidgetPiecontainer', this.options4);
  }

  get filterFromDate() {
    return this.AnalyticFilterForm.get('filterFromDate');
  }
  get filterToDate() {
    return this.AnalyticFilterForm.get('filterToDate');
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
    this.AnalyticFilterForm.patchValue({
      filterFromDate: null,
      filterToDate: null
    });

    this.filterFromDateError = false;
    this.filterToDateError = false;
    this.filterFromDateErrorMessage = "";
    this.filterToDateErrorMessage = "";
  }
  public sourceDataList: SourceData[] = [];
  public visitorData: VisitorForDay;
  //Datewise Visitor for Pages
  public options: any = {
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
        '06/08/2020',
        '07/08/2020',
        '08/08/2020',
        '09/08/2020'
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
      name: 'Current Account',
      data: [28, 29, 28, 29]

    }, {
      name: 'Home',
      data: [37, 30, 37, 30]

    }, {
      name: 'Saving Account',
      data: [35, 38, 35, 38]

    }]
  }
  public isFilterDone = false;
  public pieChartData = [];

  //Visitor for days
  public options2: any = {};
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
    this.BindVisitorForDay(searchParameter);
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
    //let scheduleLogService = this.injector.get(SourceDataService);
    //this.visitorData = await scheduleLogService.getVisitorForDay(searchParameter);
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
    Highcharts.chart('chartDaywisecontainer', this.options2);
  }
}
