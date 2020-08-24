import { Component, OnInit, Injector, ViewChild } from '@angular/core';
import * as Highcharts from 'highcharts';
import { SourceDataService } from '../sourcedata/sourcedata.service';
import { SourceData } from '../sourcedata/sourcedata';
import { Constants } from '../../shared/constants/constants';
import { ErrorMessageConstants } from '../../shared/constants/constants';
import { MessageDialogService } from '../../shared/services/mesage-dialog.service';
import { LocalStorageService } from '../../shared/services/local-storage.service';

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

  closeFilter() {
    this.isFilter = !this.isFilter;
  }
  ngOnInit() {
    this.getSourceDatas(null);
  }
  ngAfterViewInit() {
    Highcharts.chart('chartDatewisecontainer', this.options);
    Highcharts.chart('chartDaywisecontainer', this.options2);
    Highcharts.chart('chartPageWidgetwisecontainer', this.options3);
    //Highcharts.chart('chartWidgetPiecontainer', this.options4);

  }
  public sourceDataList: SourceData[] = [];
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

    }
    ]
  }
  //Visitor for Page Widget
  public options3: any = {
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
      categories: [
        'Account Information',
        'Customer Information',
        'Image',
        'Video',
        'News Alerts'
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
    series: [
      {
        name: 'Visitor Count',
        data: [4, 2, 4, 6]
      }
    ]
  }
  //Widget Visitors for day
  public pieChartOptions: any;

  constructor(private injector: Injector, private _messageDialogService: MessageDialogService,) {
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
    this.sourceDataList = await scheduleLogService.getSourceData(searchParameter);
    if (this.sourceDataList.length == 0 && this.isFilterDone == true) {
      let message = ErrorMessageConstants.getNoRecordFoundMessage;
      this._messageDialogService.openDialogBox('Error', message, Constants.msgBoxError).subscribe(data => {
        if (data == true) {
          //this.resetRoleFilterForm();
          this.getSourceDatas(null);
        }
      });
    }
    else {
      this.BindPieChart(this.sourceDataList);
      this.BindVisitorPageWidgetCount(this.sourceDataList);
    }
  }
  BindPieChart(data) {
    data = data.filter(i => i.Widgetname != '');
    var FilteredData = [];
    data.forEach(item => {
      var index = FilteredData.findIndex((element) => {
        return (element == item.Widgetname);
      });
      if (index < 0) {
        FilteredData.push(item.Widgetname);
      }
    });

    var copyFilteredData = data;
    var totalRecord = data.length;

    FilteredData.forEach(item => {
      var items = copyFilteredData.filter(i => i.Widgetname == item);

      if (items != null && items.length > 0) {
        var value = items.length / totalRecord * 100;
        var object = { name: item, y: value };
        this.pieChartData.push(object);
      }
    });
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
        //data: [
        //  {
        //    name: 'Cutomer Information',
        //    y: 11.84
        //  }, {
        //    name: 'Account Information',
        //    y: 10.85
        //  }, {
        //    name: 'Image',
        //    y: 4.67
        //  }, {
        //    name: 'Video',
        //    y: 4.18
        //  }, {
        //    name: 'News Alerts',
        //    y: 7.05
        //  }]
      }]

    }
    Highcharts.chart('chartWidgetPiecontainer', this.pieChartOptions);
  }

  BindVisitorPageWidgetCount(data) {
    data = data.filter(i => i.Widgetname != '');
    var WidgetList = [];
    data.forEach(item => {
      var index = WidgetList.findIndex((element) => {
        return (element == item.Widgetname);
      });
      if (index < 0) {
        WidgetList.push(item.Widgetname);
      }
    });
    var CustomerList = [];
    data.forEach(item => {
      var index = CustomerList.findIndex((element) => {
        return (element == item.CustomerId);
      });
      if (index < 0) {
        CustomerList.push(item.CustomerId);
      }
    });
    var WidgetVistorMap = [];
    WidgetList.forEach(item => {
      var count = 0;
      CustomerList.forEach(c => {
        var index = data.findIndex((element) => {
          return (element.CustomerId == c && element.Widgetname == item);
        });
        if (index >= 0) {
          count++;
        }
      });
      WidgetVistorMap.push(count);
    });

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
        categories: WidgetList,
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
          data: WidgetVistorMap
        }
      ]
    }
    Highcharts.chart('chartPageWidgetwisecontainer', this.options3);
  }
}
