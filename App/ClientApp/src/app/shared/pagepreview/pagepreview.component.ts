import { Component, Pipe, PipeTransform, ElementRef, } from '@angular/core';
import { DialogComponent, DialogService } from '@tomblue/ng2-bootstrap-modal';
import * as $ from 'jquery';
import * as Highcharts from 'highcharts';
import { BrowserModule, DomSanitizer, SafeHtml } from '@angular/platform-browser';

@Pipe({
  name: 'safeHtml'
})
export class SafeHtmlPip implements PipeTransform {
  constructor(private sanitized: DomSanitizer) { }
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
        if ($('.' + className).hasClass('d-none')) {
          $('.' + className).removeClass('d-none');
        }
        $('.' + className).show();
      });

      $('input[type="radio"]').on('change', function (e) {
        if (e.currentTarget.id == "savingGrpDate") {
          $("#SavingTransactionTable tbody tr").remove();
          console.log(e.type);
          var d =   [
            { 'TransactionDate': '15/07/2020', 'TransactionType': 'CR', 'Narration': 'NXT TXN: IIFL IIFL6574562', 'FCY': '1666.67', 'CurrentRate': '1.062', 'LCY': '1771.42' },
            { 'TransactionDate': '15/07/2020', 'TransactionType': 'CR', 'Narration': 'NXT TXN: IIFL IIFL6574563', 'FCY': '1435.00', 'CurrentRate': '0.962', 'LCY': '1654.56' },
            { 'TransactionDate': '19/07/2020', 'TransactionType': 'CR', 'Narration': 'NXT TXN: IIFL IIFL3557346', 'FCY': '1254.71', 'CurrentRate': '1.123', 'LCY': '1976.00' },
            { 'TransactionDate': '25/07/2020', 'TransactionType': 'CR', 'Narration': 'NXT TXN: IIFL IIFL8965435', 'FCY': '2345.12', 'CurrentRate': '1.461', 'LCY': '1453.21' },
            { 'TransactionDate': '28/07/2020', 'TransactionType': 'CR', 'Narration': 'NXT TXN: IIFL IIFL0034212', 'FCY': '1435.89', 'CurrentRate': '0.962', 'LCY': '1654.56' },
          ];
          var savingAggregateDate = d.reduce(function (groups, item) {
            const val = item["TransactionDate"]
            groups[val] = groups[val] || []
            groups[val].push(item)
            return groups
          }, {});
          console.log(savingAggregateDate);

          $.each(savingAggregateDate, function (index, data) {
            var sumOfFCY = 0;
            var sumOfLCY = 0;
            var sumOfCR = 0;

            if (data.length > 1) {
              sumOfFCY = data.reduce(function (a, b) {
                return a + parseFloat(b.FCY);
              }, 0);
              sumOfLCY = data.reduce(function (a, b) {
                return a + parseFloat(b.LCY);
              }, 0);
              sumOfCR = data.reduce(function (a, b) {
                return a + parseFloat(b.CurrentRate);
              }, 0);
            }
            else {
              sumOfFCY = data[0].FCY;
              sumOfLCY = data[0].LCY;
              sumOfCR = data[0].CurrentRate;
            }
            var tbody = $("#SavingTransactionTable> tbody");

            var tr = $("<tr>");
            tr.append($("<td>", {
              'text': data[0].TransactionDate
            }));
            tr.append($("<td>", {
              'text': data[0].TransactionType
            }));
            tr.append($("<td>", {
              'text': "-"
            }));
            tr.append($("<td>", {
              'text': sumOfFCY,
              'class': 'text-right'
            }));
            tr.append($("<td>", {
              'text': sumOfCR,
              'class': 'text-right'
            }));
            tr.append($("<td>", {
              'text': sumOfLCY,
              'class': 'text-right'
            }));
            tr.append($("<td>", {
              'text': "-"
            }));
            tbody.append(tr);
          });
        }
        else if (e.currentTarget.id == "savingShowAll") {
          $("#SavingTransactionTable tbody tr").remove();
          console.log(e.type);
          var data = [
            { 'TransactionDate': '15/07/2020', 'TransactionType': 'CR', 'Narration': 'NXT TXN: IIFL IIFL6574562', 'FCY': '1666.67', 'CurrentRate': '1.062', 'LCY': '1771.42' },
            { 'TransactionDate': '15/07/2020', 'TransactionType': 'CR', 'Narration': 'NXT TXN: IIFL IIFL6574563', 'FCY': '1435.00', 'CurrentRate': '0.962', 'LCY': '1654.56' },
            { 'TransactionDate': '19/07/2020', 'TransactionType': 'CR', 'Narration': 'NXT TXN: IIFL IIFL3557346', 'FCY': '1254.71', 'CurrentRate': '1.123', 'LCY': '1976.00' },
            { 'TransactionDate': '25/07/2020', 'TransactionType': 'CR', 'Narration': 'NXT TXN: IIFL IIFL8965435', 'FCY': '2345.12', 'CurrentRate': '1.461', 'LCY': '1453.21' },
            { 'TransactionDate': '28/07/2020', 'TransactionType': 'CR', 'Narration': 'NXT TXN: IIFL IIFL0034212', 'FCY': '1435.89', 'CurrentRate': '0.962', 'LCY': '1654.56' },
            { 'TransactionDate': '28/07/2020', 'TransactionType': 'CR', 'Narration': 'NXT TXN: IIFL IIFL0034213', 'FCY': '1666.67', 'CurrentRate': '1.062', 'LCY': '1771.42' }
          ];
          console.log(data);

          $.each(data, function (index, data) {
            var tbody = $("#SavingTransactionTable> tbody");
            var tr = $("<tr>");
            tr.append($("<td>", {
              'text': data.TransactionDate
            }));
            tr.append($("<td>", {
              'text': data.TransactionType
            }));
            tr.append($("<td>", {
              'text': data.Narration
            }));
            tr.append($("<td>", {
              'text': data.FCY,
              'class': 'text-right'
            }));
            tr.append($("<td>", {
              'text': data.CurrentRate,
              'class': 'text-right'
            }));
            tr.append($("<td>", {
              'text': data.LCY,
              'class': 'text-right'
            }));
            tr.append($("<td>", {
              'html': "<div class='action-btns btn-tbl-action'><button type='button' title='View'><span class='fa fa-paper-plane-o'></span></button></div>"
            }));
            tbody.append(tr);
          });
        }
        else if (e.currentTarget.id == "currentGrpDate") {
          $("#CurrentTransactionTable tbody tr").remove();
          console.log(e.type);
          var currentData = [];
          currentData = [
            { 'TransactionDate': '15/07/2020', 'TransactionType': 'CR', 'Narration': 'NXT TXN: IIFL IIFL6574562', 'FCY': '1666.67', 'CurrentRate': '1.062', 'LCY': '1771.42' },
            { 'TransactionDate': '15/07/2020', 'TransactionType': 'CR', 'Narration': 'NXT TXN: IIFL IIFL6574563', 'FCY': '1435.00', 'CurrentRate': '0.962', 'LCY': '1654.56' },
            { 'TransactionDate': '19/07/2020', 'TransactionType': 'CR', 'Narration': 'NXT TXN: IIFL IIFL3557346', 'FCY': '1254.71', 'CurrentRate': '1.123', 'LCY': '1976.00' },
            { 'TransactionDate': '25/07/2020', 'TransactionType': 'CR', 'Narration': 'NXT TXN: IIFL IIFL8965435', 'FCY': '2345.12', 'CurrentRate': '1.461', 'LCY': '1453.21' },
            { 'TransactionDate': '28/07/2020', 'TransactionType': 'CR', 'Narration': 'NXT TXN: IIFL IIFL0034212', 'FCY': '1435.89', 'CurrentRate': '0.962', 'LCY': '1654.56' },
          ];
          var currentAggregateDate = [];
          currentAggregateDate = currentData.reduce(function (groups, item) {
            const val = item["TransactionDate"]
            groups[val] = groups[val] || []
            groups[val].push(item)
            return groups
          }, {});
          console.log(currentAggregateDate);

          $.each(currentAggregateDate, function (index, data) {
            var sumOfFCY = 0;
            var sumOfLCY = 0;
            var sumOfCR = 0;

            if (data.length > 1) {
              sumOfFCY = data.reduce(function (a, b) {
                return a + parseFloat(b.FCY);
              }, 0);
              sumOfLCY = data.reduce(function (a, b) {
                return a + parseFloat(b.LCY);
              }, 0);
              sumOfCR = data.reduce(function (a, b) {
                return a + parseFloat(b.CurrentRate);
              }, 0);
            }
            else {
              sumOfFCY = data[0].FCY;
              sumOfLCY = data[0].LCY;
              sumOfCR = data[0].CurrentRate;
            }
            var tbody = $("#CurrentTransactionTable> tbody");

            var tr = $("<tr>");
            tr.append($("<td>", {
              'text': data[0].TransactionDate
            }));
            tr.append($("<td>", {
              'text': data[0].TransactionType
            }));
            tr.append($("<td>", {
              'text': "-"
            }));
            tr.append($("<td>", {
              'text': sumOfFCY,
              'class': 'text-right'
            }));
            tr.append($("<td>", {
              'text': sumOfCR,
              'class': 'text-right'
            }));
            tr.append($("<td>", {
              'text': sumOfLCY,
              'class': 'text-right'
            }));
            tr.append($("<td>", {
              'text': "-"
            }));
            tbody.append(tr);
          });
        }
        else if (e.currentTarget.id == "currentShowAll") {
          $("#CurrentTransactionTable tbody tr").remove();
          var currentTranData = [];
          currentTranData = [
            { 'TransactionDate': '15/07/2020', 'TransactionType': 'CR', 'Narration': 'NXT TXN: IIFL IIFL6574562', 'FCY': '1666.67', 'CurrentRate': '1.062', 'LCY': '1771.42' },
            { 'TransactionDate': '15/07/2020', 'TransactionType': 'CR', 'Narration': 'NXT TXN: IIFL IIFL6574563', 'FCY': '1435.00', 'CurrentRate': '0.962', 'LCY': '1654.56' },
            { 'TransactionDate': '19/07/2020', 'TransactionType': 'CR', 'Narration': 'NXT TXN: IIFL IIFL3557346', 'FCY': '1254.71', 'CurrentRate': '1.123', 'LCY': '1976.00' },
            { 'TransactionDate': '25/07/2020', 'TransactionType': 'CR', 'Narration': 'NXT TXN: IIFL IIFL8965435', 'FCY': '2345.12', 'CurrentRate': '1.461', 'LCY': '1453.21' },
            { 'TransactionDate': '28/07/2020', 'TransactionType': 'CR', 'Narration': 'NXT TXN: IIFL IIFL0034212', 'FCY': '1435.89', 'CurrentRate': '0.962', 'LCY': '1654.56' },
            { 'TransactionDate': '28/07/2020', 'TransactionType': 'CR', 'Narration': 'NXT TXN: IIFL IIFL0034213', 'FCY': '1666.67', 'CurrentRate': '1.062', 'LCY': '1771.42' }
          ];

          console.log(currentTranData);

          $.each(currentTranData, function (index, data) {
            var tbody = $("#CurrentTransactionTable> tbody");
            var tr = $("<tr>");
            tr.append($("<td>", {
              'text': data.TransactionDate
            }));
            tr.append($("<td>", {
              'text': data.TransactionType
            }));
            tr.append($("<td>", {
              'text': data.Narration
            }));
            tr.append($("<td>", {
              'text': data.FCY,
              'class': 'text-right'
            }));
            tr.append($("<td>", {
              'text': data.CurrentRate,
              'class': 'text-right'
            }));
            tr.append($("<td>", {
              'text': data.LCY,
              'class': 'text-right'
            }));
            tr.append($("<td>", {
              'html': "<div class='action-btns btn-tbl-action'><button type='button' title='View'><span class='fa fa-paper-plane-o'></span></button></div>"
            }));
            tbody.append(tr);
          });
        }
      });

    });

  }

  ngAfterViewInit() {

    if (document.getElementById('analyticschartcontainer') != null) {
      Highcharts.chart('analyticschartcontainer', this.AnalyticsChartOptions);
    }

    if (document.getElementById('spendingTrendscontainer') != null) {
      Highcharts.chart('spendingTrendscontainer', this.SpendingTrendsChartOptions);
    }

    if (document.getElementById('savingTrendscontainer') != null) {
      Highcharts.chart('savingTrendscontainer', this.SavingTrendChartOptions);
    }
    var data = this.SavingTransactionAllData.reduce(function (groups, item) {
      const val = item["TransactionDate"]
      groups[val] = groups[val] || []
      groups[val].push(item)
      return groups
    }, {});
    console.log(data);

  }

  public SavingTransactionAllData = [
    { 'TransactionDate': '15/07/2020', 'TransactionType': 'CR', 'Narration': 'NXT TXN: IIFL IIFL6574562', 'FCY': '1666.67', 'CurrentRate': '1.062', 'LCY': '1771.42' },
    { 'TransactionDate': '15/07/2020', 'TransactionType': 'CR', 'Narration': 'NXT TXN: IIFL IIFL6574563', 'FCY': '1435.00', 'CurrentRate': '0.962', 'LCY': '1654.56' },
    { 'TransactionDate': '19/07/2020', 'TransactionType': 'CR', 'Narration': 'NXT TXN: IIFL IIFL3557346', 'FCY': '1254.71', 'CurrentRate': '1.123', 'LCY': '1976.00' },
    { 'TransactionDate': '25/07/2020', 'TransactionType': 'CR', 'Narration': 'NXT TXN: IIFL IIFL8965435', 'FCY': '2345.12', 'CurrentRate': '1.461', 'LCY': '1453.21' },
    { 'TransactionDate': '28/07/2020', 'TransactionType': 'CR', 'Narration': 'NXT TXN: IIFL IIFL0034212', 'FCY': '1435.89', 'CurrentRate': '0.962', 'LCY': '1654.56' },
    { 'TransactionDate': '28/07/2020', 'TransactionType': 'CR', 'Narration': 'NXT TXN: IIFL IIFL0034213', 'FCY': '1666.67', 'CurrentRate': '1.062', 'LCY': '1771.42' }
  ]
  public SavingTransactionAggregateData = [
    { 'TransactionDate': '15/07/2020', 'TransactionType': 'CR', 'Narration': '-', 'FCY': '3101.67', 'CurrentRate': '1.754', 'LCY': '3425.98' },
    { 'TransactionDate': '19/07/2020', 'TransactionType': 'CR', 'Narration': '-', 'FCY': '1254.71', 'CurrentRate': '1.123', 'LCY': '1976.00' },
    { 'TransactionDate': '25/07/2020', 'TransactionType': 'CR', 'Narration': '-', 'FCY': '2345.12', 'CurrentRate': '1.461', 'LCY': '1453.21' },
    { 'TransactionDate': '28/07/2020', 'TransactionType': 'CR', 'Narration': '-', 'FCY': '1435.89', 'CurrentRate': '0.962', 'LCY': '1654.56' },
    { 'TransactionDate': '28/07/2020', 'TransactionType': 'CR', 'Narration': '-', 'FCY': '1666.67', 'CurrentRate': '1.062', 'LCY': '3425.98' }
  ]
  public OnSavingTranGroupByDateClicked() {
    var aggregateDate = this.SavingTransactionAllData.reduce(function (groups, item) {
      const val = item["TransactionDate"]
      groups[val] = groups[val] || []
      groups[val].push(item)
      return groups
    }, {});
    console.log(aggregateDate);

    $.each(aggregateDate, function (index, data) {
      var tbody = $("#SavingTransactionTable> tbody");
      console.log(data);
      var tr = $("<tr>");
      tr.append($("<td>", {
        'text': ""
      }));
      tr.append($("<td>", {
        'text': ""
      }));
      tr.append($("<td>", {
        'text': "-"
      }));
      tr.append($("<td>", {
        'text': "-"
      }));
      tr.append($("<td>", {
        'text': "-"
      }));
      tr.append($("<td>", {
        'text': "-"
      }));
      tbody.append(tr);
    });
  }


}

export interface PagePreviewModel {
  htmlContent: string;
}

interface DialogOptions {
  backdropColor?: string;
}
