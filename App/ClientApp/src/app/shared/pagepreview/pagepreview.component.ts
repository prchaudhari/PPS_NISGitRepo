import { Component, Pipe, PipeTransform, ElementRef, SecurityContext } from '@angular/core';
import { DialogComponent, DialogService } from '@tomblue/ng2-bootstrap-modal';
import * as $ from 'jquery';
import * as Highcharts from 'highcharts';
import { BrowserModule, DomSanitizer, SafeHtml } from '@angular/platform-browser';
import { map } from 'rxjs/operators';
import { HttpClient } from '@angular/common/http';
import { AppSettings } from '../../appsettings';
import { ConfigConstants } from 'src/app/shared/constants/configConstants';

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
                  <button type="button" class="close p-1 mt-n2" (click)="cancel()">
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

  analyticschart;
  savingchart;
  spendingchart;
  public baseURL: string = AppSettings.baseURL;

  constructor(dialogService: DialogService,
    private sanitizer: DomSanitizer,
    private _http: HttpClient,) {
    super(dialogService);
  }

  cancel() {
    this.close();
  }

  public AnalyticsChartOptions: any = {
    chart: {
      backgroundColor: 'rgba(0,0,0,0)',
      plotBorderWidth: null,
      plotShadow: false,
      type: 'pie',
      height: (9 / 16 * 100) + '%'
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
          format: '<b>{point.name}</b>: {point.percentage:.1f} %'
        },
        showInLegend: false
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
    chart: {
      height: (9 / 16 * 100) + '%',
      backgroundColor: 'rgba(0,0,0,0)'
    },
    title: {
      text: ''
    },
    xAxis: {
      categories: ['Jan', 'Feb', 'Mar', 'Apr','May']
    },
    labels: {
      items: [{
        //html: 'How you have been spending',
        style: {
          left: '50px',
          top: '18px',
          color: ( // theme
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
    chart: {
      height: (9 / 16 * 100) + '%',
      backgroundColor: 'rgba(0,0,0,0)'
    },
    title: {
      text: ''
    },
    xAxis: {
      categories: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun','Jul']
    },
    labels: {
      items: [{
        //html: 'How you have been spending',
        style: {
          left: '50px',
          top: '18px',
          color: ( // theme
            Highcharts.defaultOptions.title.style &&
            Highcharts.defaultOptions.title.style.color
          ) || 'black'
        }
      }]
    },
    series: [{
      // type: 'spline',
      name: '',
      data: [1.5, 2.5, 3, 1.5, 3, 2, 4],
      marker: {
        lineWidth: 1,
        lineColor: Highcharts.getOptions().colors[3],
        fillColor: 'white'
      }
    }]
  }

  public LineGraphChartOptions: any = {
    xAxis: {
    },
  }

  public BarGraphChartOptions: any = {
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

  ngOnInit() {

    $(document).ready(() => {

      $(".mainNav").on('click', function(e) {
        $('.tabDivClass').hide();
        var tag = e.currentTarget;
        $('.mainNav').removeClass('active');
        let newClasses = 'active ' + $(tag).attr('class');
        $(tag).attr('class', newClasses);
        let classlist = $(tag).attr('class').split(' ');
        let className = classlist[classlist.length - 1];
        if ($('.' + className).hasClass('d-none')) {
          $('.' + className).removeClass('d-none');
        }
        $('#' + className).show();
      });

      $('input[type="radio"]').on('change', function (e) {
        if (e.currentTarget.id == "savingGrpDate") {
          $("#SavingTransactionTable tbody tr").remove();
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
          var data = [
            { 'TransactionDate': '15/07/2020', 'TransactionType': 'CR', 'Narration': 'NXT TXN: IIFL IIFL6574562', 'FCY': '1666.67', 'CurrentRate': '1.062', 'LCY': '1771.42' },
            { 'TransactionDate': '15/07/2020', 'TransactionType': 'CR', 'Narration': 'NXT TXN: IIFL IIFL6574563', 'FCY': '1435.00', 'CurrentRate': '0.962', 'LCY': '1654.56' },
            { 'TransactionDate': '19/07/2020', 'TransactionType': 'CR', 'Narration': 'NXT TXN: IIFL IIFL3557346', 'FCY': '1254.71', 'CurrentRate': '1.123', 'LCY': '1976.00' },
            { 'TransactionDate': '25/07/2020', 'TransactionType': 'CR', 'Narration': 'NXT TXN: IIFL IIFL8965435', 'FCY': '2345.12', 'CurrentRate': '1.461', 'LCY': '1453.21' },
            { 'TransactionDate': '28/07/2020', 'TransactionType': 'CR', 'Narration': 'NXT TXN: IIFL IIFL0034212', 'FCY': '1435.89', 'CurrentRate': '0.962', 'LCY': '1654.56' },
            { 'TransactionDate': '28/07/2020', 'TransactionType': 'CR', 'Narration': 'NXT TXN: IIFL IIFL0034213', 'FCY': '1666.67', 'CurrentRate': '1.062', 'LCY': '1771.42' }
          ];

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

      $(".ImageAsset").each((index, element) => {
        var classlst = element.classList;
        var assetId = classlst[classlst.length - 1];
        if(assetId!=undefined && assetId!=0) {
          this._http.get(this.baseURL + 'assetlibrary/asset/download?assetIdentifier=' + assetId, { responseType: "arraybuffer", observe: 'response' }).pipe(map(response => response))
          .subscribe(
            data => {
              let contentType = data.headers.get('Content-Type');
              let fileName = data.headers.get('x-filename');
              const blob = new Blob([data.body], { type: contentType });
              let objectURL = URL.createObjectURL(blob);
              element.src = this.sanitizer.sanitize(SecurityContext.RESOURCE_URL, this.sanitizer.bypassSecurityTrustResourceUrl(objectURL));
            },
            error => {
              //$('.overlay').show();
          });
        }
      });

      if($('#TenantLogo').length) {
        let tenant = JSON.parse(localStorage.getItem('tenantDetails'));
        if(tenant != null) {
          if(tenant.TenantLogo != undefined && tenant.TenantLogo != null && tenant.TenantLogo != '') {
            let image = new Image();
            //image.src = tenant.TenantLogo;
            image.src="assets/images/absa-logo.png"
            image.height = 40;
            $('#TenantLogo').append(image);
          }else {
            let tenantname = tenant.TenantName.charAt(0).toUpperCase();
            let div = document.createElement('div');
            div.classList.add('ltr-img');
            div.style.height = '50px';
            div.style.width = '50px';
            div.style.fontSize = '30px';

            let span = document.createElement('span');
            span.textContent = tenantname;
            div.appendChild(span);
            $('#TenantLogo').append(div);
          }
        }
      }

      $(".VideoAsset").each((index, element) => {
        var classlst = element.classList;
        var assetId = classlst[classlst.length - 1];
        if(assetId!=undefined && assetId!=0) {
          this._http.get(this.baseURL + 'assetlibrary/asset/download?assetIdentifier=' + assetId, { responseType: "arraybuffer", observe: 'response' }).pipe(map(response => response))
          .subscribe(
            data => {
              let contentType = data.headers.get('Content-Type');
              let fileName = data.headers.get('x-filename');
              const blob = new Blob([data.body], { type: contentType });
              let objectURL = URL.createObjectURL(blob);
              var videourl = this.sanitizer.sanitize(SecurityContext.RESOURCE_URL, this.sanitizer.bypassSecurityTrustResourceUrl(objectURL));
              
              var parentDiv = element.parentElement;
              parentDiv.removeChild(parentDiv.children[0])
              
              var video = document.createElement('video');
              video.className = 'video-widget';
              video.controls = true;

              var sourceTag = document.createElement('source');
              sourceTag.setAttribute('src', videourl);
              sourceTag.setAttribute('type', 'video/mp4');
              video.appendChild(sourceTag);
              parentDiv.appendChild(video);
            },
            error => {
              //$('.overlay').show();
          });
        }
      });

      $(".BackgroundImage").each((index, element) => {
        var classlst = element.classList;
        var assetId = classlst[classlst.length - 1];
        if(assetId!=undefined && assetId!=0) {
          this._http.get(this.baseURL + 'assetlibrary/asset/download?assetIdentifier=' + assetId, { responseType: "arraybuffer", observe: 'response' }).pipe(map(response => response))
          .subscribe(
            data => {
              let contentType = data.headers.get('Content-Type');
              const blob = new Blob([data.body], { type: contentType });
              let objectURL = URL.createObjectURL(blob);
              let imgUrl = this.sanitizer.sanitize(SecurityContext.RESOURCE_URL, this.sanitizer.bypassSecurityTrustResourceUrl(objectURL));
              element.style.background = "url(" + imgUrl + ")";
            },
            error => {
              //$('.overlay').show();
          });
        }
      });

    });

  }

  ngAfterViewInit() {

    if (document.getElementById('analyticschartcontainer') != null) {
      this.analyticschart = Highcharts.chart('analyticschartcontainer', this.AnalyticsChartOptions);
    }

    if (document.getElementById('spendingTrendscontainer') != null) {
      this.spendingchart = Highcharts.chart('spendingTrendscontainer', this.SpendingTrendsChartOptions);
    }

    if (document.getElementById('savingTrendscontainer') != null) {
      this.savingchart = Highcharts.chart('savingTrendscontainer', this.SavingTrendChartOptions);
    }

    if (document.getElementById('AccountAnalysisBarGraphcontainer') != null) {
      if (null != $('#HiddenAccountAnalysisBarGraphData').val() && '' != $('#HiddenAccountAnalysisBarGraphData').val()) {
        let data = JSON.parse($('#HiddenAccountAnalysisBarGraphData').val());
        if (data.length != 0) {
          var e = [], _lstseries = [];
          data[0].MonthwiseAmount.forEach(item => { e.push(item.Month); });
          for (let i = 0; i < data.length; i++) {
            var a = [];
            data[i].MonthwiseAmount.forEach(item => { a.push(item.Amount); });
            _lstseries.push({ 'name': data[i].AccountType, 'data': a });
          }

          Highcharts.chart('AccountAnalysisBarGraphcontainer', {
            chart: {
              type: 'column',
              style: {
                fontFamily: 'Mark Pro Regular',
                fontSize: '8pt'
              }
            },
            title: {
              text: ''
            },
            xAxis: {
              categories: e,
              crosshair: true
            },
            yAxis: {
              title: {
                text: 'Amount (R)'
              }
            },
            credits: {
              enabled: false
            },
            tooltip: {
              headerFormat: '<span>{point.key}</span><table>',
              pointFormat: '<tr><td style="color:{series.color};padding:0">{series.name}: </td> <td style="padding:0"><b>{point.y:.2f}</b></td></tr>',
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
            series: _lstseries,
            //colors: ['#005b00', '#434348', '#7cb5ec']
          });
        }
        else {
          $('#AccountAnalysisBarGraphcontainer').html('<div class=\"text-danger text-center\">No data available.</div>')
        }
      }
    }

    if (document.getElementById('YTDRewardPointsBarGraphcontainer') != null) {
      if (null != $('#HiddenYTDRewardPointsBarGraphData').val() && '' != $('#HiddenYTDRewardPointsBarGraphData').val()) {
        let data = JSON.parse($('#HiddenYTDRewardPointsBarGraphData').val());
        if (data.length != 0) {
          var e = [], a = [], _lstseries = [];
          data.forEach(item => { e.push(item.Month); a.push(item.RewardPoint); });
          _lstseries.push({ 'name': 'Rewards points', 'data': a });

          Highcharts.chart('YTDRewardPointsBarGraphcontainer', {
            chart: {
              type: 'column',
              style: {
                fontFamily: 'Mark Pro Regular',
                fontSize: '8pt'
              }
            },
            title: {
              text: ''
            },
            xAxis: {
              categories: e,
              crosshair: true
            },
            yAxis: {
              title: {
                text: 'Amount (R)'
              }
            },
            credits: {
              enabled: false
            },
            tooltip: {
              headerFormat: '<span>{point.key}</span><table>',
              pointFormat: '<tr><td style="color:{series.color};padding:0">{series.name}: </td> <td style="padding:0"><b>{point.y:.2f}</b></td></tr>',
              footerFormat: '</table>',
              shared: true,
              useHTML: true
            },
            plotOptions: {
              column: {
                pointPadding: 0.2,
                borderWidth: 0
              },
              series: {
                color: '#005b00'
              }
            },
            series: _lstseries
          });
        }
        else {
          $('#YTDRewardPointsBarGraphcontainer').html('<div class=\"text-danger text-center\">No data available.</div>')
        }
      }
    }

    if (document.getElementById('PointsRedeemedYTDBarGraphcontainer') != null) {
      if (null != $('#HiddenPointsRedeemedBarGraphData').val() && '' != $('#HiddenPointsRedeemedBarGraphData').val()) {
        let data = JSON.parse($('#HiddenPointsRedeemedBarGraphData').val());
        if (data.length != 0) {
          var e = [], a = [], _lstseries = [];
          data.forEach(item => { e.push(item.Month); a.push(item.RedeemedPoints); });
          _lstseries.push({ 'name': 'Rewards points', 'data': a });

          Highcharts.chart('PointsRedeemedYTDBarGraphcontainer', {
            chart: {
              type: 'column',
              style: {
                fontFamily: 'Mark Pro Regular',
                fontSize: '8pt'
              }
            },
            title: {
              text: ''
            },
            xAxis: {
              categories: e,
              crosshair: true
            },
            yAxis: {
              title: {
                text: 'Amount (R)'
              }
            },
            credits: {
              enabled: false
            },
            tooltip: {
              headerFormat: '<span>{point.key}</span><table>',
              pointFormat: '<tr><td style="color:{series.color};padding:0">{series.name}: </td> <td style="padding:0"><b>{point.y:.2f}</b></td></tr>',
              footerFormat: '</table>',
              shared: true,
              useHTML: true
            },
            plotOptions: {
              column: {
                pointPadding: 0.2,
                borderWidth: 0
              },
              series: {
                color: '#005b00'
              }
            },
            series: _lstseries
          });
        }
        else {
          $('#PointsRedeemedYTDBarGraphcontainer').html('<div class=\"text-danger text-center\">No data available.</div>')
        }
      }
    }

    if (document.getElementById('ProductRelatedPointsEarnedBarGraphcontainer') != null) {
      if (null != $('#HiddenProductRelatedPointsEarnedBarGraphData').val() && '' != $('#HiddenProductRelatedPointsEarnedBarGraphData').val()) {
        let data = JSON.parse($('#HiddenProductRelatedPointsEarnedBarGraphData').val());
        if (data.length != 0) {
          var e = [], _lstseries = [];
          data[0].MonthwiseAmount.forEach(item => { e.push(item.Month); });
          for (let i = 0; i < data.length; i++) {
            var a = [];
            data[i].MonthwiseAmount.forEach(item => { a.push(item.RewardPoint); });
            _lstseries.push({ 'name': data[i].AccountType, 'data': a });
          }

          Highcharts.chart('ProductRelatedPointsEarnedBarGraphcontainer', {
            chart: {
              type: 'column',
              style: {
                fontFamily: 'Mark Pro Regular',
                fontSize: '8pt'
              }
            },
            title: {
              text: ''
            },
            xAxis: {
              categories: e,
              crosshair: true
            },
            yAxis: {
              title: {
                text: 'Amount (R)'
              }
            },
            credits: {
              enabled: false
            },
            tooltip: {
              headerFormat: '<span>{point.key}</span><table>',
              pointFormat: '<tr><td style="color:{series.color};padding:0">{series.name}: </td> <td style="padding:0"><b>{point.y:.2f}</b></td></tr>',
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
            series: _lstseries
          });
        }
        else {
          $('#ProductRelatedPointsEarnedBarGraphcontainer').html('<div class=\"text-danger text-center\">No data available.</div>')
        }
      }
    }

    if (document.getElementById('CategorySpendRewardsPieChartcontainer') != null) {
      if (null != $('#HiddenCategorySpendRewardsPieChartData').val() && '' != $('#HiddenCategorySpendRewardsPieChartData').val()) {
        let data = JSON.parse($('#HiddenCategorySpendRewardsPieChartData').val());
        if (data.length != 0) {
          var e = [], _lstseries = [];
          data.forEach(item => {
            _lstseries.push({ 'name': item.Category, 'y': item.SpendReward });
          });

          let chartPropertyOptions: any = {
            chart: {
                plotBackgroundColor: null,
                plotBorderWidth: null,
                plotShadow: !1,
                type: 'pie',
                style: {
                    fontFamily: 'Mark Pro Regular',
                    fontSize: '8pt'
                }
            },
            title: { text: '' },
            credits: { enabled: false },
            tooltip: { pointFormat: '{series.name}: <b>{point.percentage:.1f}%</b>' },
            accessibility: { point: { valueSuffix: '%' } },
            plotOptions: {
                pie: {
                    allowPointSelect: !0,
                    cursor: 'pointer',
                    dataLabels: {
                        enabled: !0,
                        format: '{point.percentage:.1f} %'
                    },
                    showInLegend: !0
                }
            },
            series: [{
                name: 'Percentage',
                colorByPoint: !0,
                data: _lstseries
            }]
          }
          Highcharts.chart('CategorySpendRewardsPieChartcontainer', chartPropertyOptions );
        }
        else {
          $('#CategorySpendRewardsPieChartcontainer').html('<div class=\"text-danger text-center\">No data available.</div>')
        }
      }
    }

    if($('#hiddenLineChartIds').val() != undefined && $('#hiddenLineChartIds').val() != '') {
      let lineChartIds = $('#hiddenLineChartIds').val().split(",");
      for(let i=0; i< lineChartIds.length; i++) {
        let widgetId = lineChartIds[i].split("_")[1];
        if($('#hiddenLineGraphData_'+widgetId) != null) {
          var chartdata = JSON.parse($('#hiddenLineGraphData_'+widgetId).val());
          let linechartoptions = Object.assign({}, this.LineGraphChartOptions);
          linechartoptions.xAxis.categories = chartdata.xAxis;
          linechartoptions.series = chartdata.series;
          linechartoptions.title = {text: ''};
          if (chartdata.color != "" || chartdata.color != null) {
            Highcharts.setOptions({
              colors: chartdata.color.split(",")
            });
          }
          Highcharts.chart(''+lineChartIds[i], linechartoptions);
        }
      }
    }

    if($('#hiddenBarChartIds').val() != undefined && $('#hiddenBarChartIds').val() != '') {
      let barChartIds = $('#hiddenBarChartIds').val().split(",");
      for(let i=0; i< barChartIds.length; i++) {
        let widgetId = barChartIds[i].split("_")[1];
        if($('#hiddenBarGraphData_'+widgetId) != null) {
          var chartdata = JSON.parse($('#hiddenBarGraphData_'+widgetId).val());
          let barchartoptions = Object.assign({}, this.BarGraphChartOptions);
          barchartoptions.xAxis.categories = chartdata.xAxis;
          barchartoptions.series = chartdata.series;
          barchartoptions.title = {text: ''};
          if (chartdata.color != "" || chartdata.color != null) {
            Highcharts.setOptions({
              colors: chartdata.color.split(",")
            });
          }
          Highcharts.chart(''+barChartIds[i], barchartoptions);
        }
      }
    }

    if($('#hiddenPieChartIds').val() != null && $('#hiddenPieChartIds').val() != '') {
      let pieChartIds = $('#hiddenPieChartIds').val().split(",");
      for(let i=0; i< pieChartIds.length; i++) {
        let widgetId = pieChartIds[i].split("_")[1];
        if($('#hiddenPieChartData_'+widgetId) != null) {
          var chartdata = JSON.parse($('#hiddenPieChartData_'+widgetId).val());
          let piechartoptions = Object.assign({}, this.PieChartOptions);
          piechartoptions.series = chartdata.series;
          piechartoptions.title = {text: ''};
          if (chartdata.color != "" || chartdata.color != null) {
            Highcharts.setOptions({
              colors: chartdata.color.split(",")
            });
          }
          Highcharts.chart(''+pieChartIds[i], piechartoptions);
        }
      }
    }
    
    var data = this.SavingTransactionAllData.reduce(function (groups, item) {
      const val = item["TransactionDate"]
      groups[val] = groups[val] || []
      groups[val].push(item)
      return groups
    }, {});
  }

  public SavingTransactionAllData = [
    { 'TransactionDate': '15/07/2020', 'TransactionType': 'CR', 'Narration': 'NXT TXN: IIFL IIFL6574562', 'FCY': '1666.67', 'CurrentRate': '1.062', 'LCY': '1771.42' },
    { 'TransactionDate': '15/07/2020', 'TransactionType': 'CR', 'Narration': 'NXT TXN: IIFL IIFL6574563', 'FCY': '1435.00', 'CurrentRate': '0.962', 'LCY': '1654.56' },
    { 'TransactionDate': '19/07/2020', 'TransactionType': 'CR', 'Narration': 'NXT TXN: IIFL IIFL3557346', 'FCY': '1254.71', 'CurrentRate': '1.123', 'LCY': '1976.00' },
    { 'TransactionDate': '25/07/2020', 'TransactionType': 'CR', 'Narration': 'NXT TXN: IIFL IIFL8965435', 'FCY': '2345.12', 'CurrentRate': '1.461', 'LCY': '1453.21' },
    { 'TransactionDate': '28/07/2020', 'TransactionType': 'CR', 'Narration': 'NXT TXN: IIFL IIFL0034212', 'FCY': '1435.89', 'CurrentRate': '0.962', 'LCY': '1654.56' },
    { 'TransactionDate': '28/07/2020', 'TransactionType': 'CR', 'Narration': 'NXT TXN: IIFL IIFL0034213', 'FCY': '1666.67', 'CurrentRate': '1.062', 'LCY': '1771.42' }
  ];


  public SavingTransactionAggregateData = [
    { 'TransactionDate': '15/07/2020', 'TransactionType': 'CR', 'Narration': '-', 'FCY': '3101.67', 'CurrentRate': '1.754', 'LCY': '3425.98' },
    { 'TransactionDate': '19/07/2020', 'TransactionType': 'CR', 'Narration': '-', 'FCY': '1254.71', 'CurrentRate': '1.123', 'LCY': '1976.00' },
    { 'TransactionDate': '25/07/2020', 'TransactionType': 'CR', 'Narration': '-', 'FCY': '2345.12', 'CurrentRate': '1.461', 'LCY': '1453.21' },
    { 'TransactionDate': '28/07/2020', 'TransactionType': 'CR', 'Narration': '-', 'FCY': '1435.89', 'CurrentRate': '0.962', 'LCY': '1654.56' },
    { 'TransactionDate': '28/07/2020', 'TransactionType': 'CR', 'Narration': '-', 'FCY': '1666.67', 'CurrentRate': '1.062', 'LCY': '3425.98' }
  ];


  public OnSavingTranGroupByDateClicked() {
    var aggregateDate = this.SavingTransactionAllData.reduce(function (groups, item) {
      const val = item["TransactionDate"]
      groups[val] = groups[val] || []
      groups[val].push(item)
      return groups
    }, {});

    $.each(aggregateDate, function (index, data) {
      var tbody = $("#SavingTransactionTable> tbody");
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
