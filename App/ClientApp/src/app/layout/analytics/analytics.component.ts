import { Component, OnInit } from '@angular/core';
import * as Highcharts from 'highcharts';

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

    }
    ngAfterViewInit() {
        Highcharts.chart('chartDatewisecontainer', this.options);
        Highcharts.chart('chartDaywisecontainer', this.options2);
        Highcharts.chart('chartPageWidgetwisecontainer', this.options3);
        Highcharts.chart('chartWidgetPiecontainer', this.options4);

    }

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
            data: [14,13,21,45,28, 29]

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
            data: [4,2,4,6]
            }
        ]
    }
    //Widget Visitors for day
    public options4: any = {
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
            data: [
                {
                name: 'Cutomer Information',
                y: 11.84
            }, {
                    name: 'Account Information',
                y: 10.85
            }, {
                name: 'Image',
                y: 4.67
            }, {
                name: 'Video',
                y: 4.18
            }, {
                name: 'News Alerts',
                y: 7.05
            }]
        }]
   
    }
    constructor() { }

}
