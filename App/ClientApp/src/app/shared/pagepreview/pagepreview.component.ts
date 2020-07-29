import { Component, Pipe, PipeTransform, ElementRef, } from '@angular/core';
import { DialogComponent, DialogService } from '@tomblue/ng2-bootstrap-modal';
import * as $ from 'jquery';
import * as Highcharts from 'highcharts';
import { BrowserModule, DomSanitizer, SafeHtml } from '@angular/platform-browser';

@Pipe({
  name: 'safeHtml'
})
export class SafeHtmlPip implements PipeTransform {
  constructor(private sanitized: DomSanitizer) {}
  transform(value) {
    return this.sanitized.bypassSecurityTrustHtml(value);
  }
}

@Component({
    selector: 'pagepreview',
    template: `<div class="modal-dialog modal-dialog-centered modal-xl" role="document">
                <div class="modal-content overflow-auto stylescrollbar">
                  <div class="modal-body p-1 text-center">
                  <button type="button" class="close p-1" (click)="cancel()">
                    <span aria-hidden="true">&times;</span>
                  </button>
                  <div [innerHtml]="htmlContent | safeHtml">
                    </div>
                  </div>
                </div>
              </div>`
})
export class PagePreviewComponent extends DialogComponent<PagePreviewModel, boolean> implements PagePreviewModel, DialogOptions {
    htmlContent: string;
    backdropColor: string = "red";
    constructor(dialogService: DialogService,
      private sanitizer: DomSanitizer) {
        super(dialogService);
    }

    cancel() {
      this.close();
    }

    public AnalyticsChartOptions: any = {
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

    public SpendingTrendsChartOptions: any = {
      title: {
        text: ''
      },
      xAxis: {
        categories: ['Now', 'Jan', 'Feb', 'Mar', 'Apr']
      },
      labels: {
        items: [{
          style: {
            left: '50px',
            top: '18px',
            color: (
              Highcharts.defaultOptions.title.style &&
              Highcharts.defaultOptions.title.style.color
            ) || 'black'
          }
        }]
      },
      series: [{
        type: 'column',
        name: 'Your Income',
        data: [1, 3, 4, 2, 5]
      }, {
        type: 'column',
        name: 'Your Spending',
        data: [2, 2, 1, 4, 1]
      }, {
        type: 'spline',
        name: '',
        data: [1.5, 2.5, 3, 1.5, 3],
        marker: {
          lineWidth: 2,
          lineColor: Highcharts.getOptions().colors[3],
          fillColor: 'white'
        }
      }]
    }

    public SavingTrendChartOptions: any = {
      title: {
        text: ''
      },
      xAxis: {
        categories: ['Now', 'Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun']
      },
      labels: {
        items: [{
          style: {
            left: '50px',
            top: '18px',
            color: ( 
              Highcharts.defaultOptions.title.style &&
              Highcharts.defaultOptions.title.style.color
            ) || 'black'
          }
        }]
      },
      series: [{
        name: '',
        data: [1.5, 2.5, 3, 1.5, 3, 2, 4],
        marker: {
          lineWidth: 1,
          lineColor: Highcharts.getOptions().colors[3],
          fillColor: 'white'
        }
      }]
    }

    ngOnInit() {

      $(document).ready(function () {
        $('.nav-link').click(function (e) { 
          $('.tabDivClass').hide();
          $('.nav-link').removeClass('active');
          let newClasses = 'active ' + $(e.currentTarget).attr('class');
          $(e.currentTarget).attr('class', newClasses);
          let classlist = $(e.currentTarget).attr('class').split(' ');
          let className = classlist[classlist.length - 1];
          if($('.'+className).hasClass('d-none')) {
            $('.'+className).removeClass('d-none');
          }
          $('.'+className).show();
        });
      });

    }

    ngAfterViewInit() {

      if(document.getElementById('analyticschartcontainer') != null) {
        Highcharts.chart('analyticschartcontainer', this.AnalyticsChartOptions);
      }

      if(document.getElementById('spendingTrendscontainer') != null) {
        Highcharts.chart('spendingTrendscontainer', this.SpendingTrendsChartOptions);
      }

      if(document.getElementById('savingTrendscontainer') != null) {
        Highcharts.chart('savingTrendscontainer', this.SavingTrendChartOptions);
      }

    }
}

export interface PagePreviewModel {
  htmlContent: string;
}

interface DialogOptions {
    backdropColor?: string;
}
