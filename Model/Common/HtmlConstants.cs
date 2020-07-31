// <copyright file="HtmlConstants.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2020 Websym Solutions Pvt Ltd.
// </copyright>
//------------------------------------------------------------------------------

namespace nIS
{
    public class HtmlConstants
    {
        public const int CUSTOMER_INFORMATION_WIDGET_ID = 1;

        public const int ACCOUNT_INFORMATION_WIDGET_ID = 2;

        public const int SUMMARY_AT_GLANCE_WIDGET_ID = 3;

        public const int IMAGE_WIDGET_ID = 4;

        public const int VIDEO_WIDGET_ID = 5;

        public const int ANALYTICS_WIDGET_ID = 6;

        public const int SAVING_TRANSACTION_WIDGET_ID = 7;

        public const int CURRENT_TRANSACTION_WIDGET_ID = 8;

        public const int SAVING_TREND_WIDGET_ID = 9;

        public const int TOP_4_INCOME_SOURCES_WIDGET_ID = 10;

        public const int CURRENT_AVAILABLE_BALANCE_WIDGET_ID = 11;

        public const int SAVING_AVAILABLE_BALANCE_WIDGET_ID = 12;

        public const int REMINDER_AND_RECOMMENDATION_WIDGET_ID = 13;

        public const int SPENDING_TREND_WIDGET_ID = 14;

        public const int HOME_PAGE_TYPE_ID = 1;

        public const int SAVING_ACCOUNT_PAGE_TYPE_ID = 2;

        public const int CURRENT_ACCOUNT_PAGE_TYPE_ID = 3;

        public const string HTML_HEADER = "<html><head><title>NIS Output</title><meta charset='utf-8'><meta name='viewport' content='width=device-width, initial-scale=1'><link rel='stylesheet' href='../common/css/bootstrap.min.css'><link rel='stylesheet' href='../common/css/font-awesome.min.css'><script src='../common/js/jquery.min.js'></script><script src='../common/js/popper.min.js'></script><script src='../common/js/bootstrap.min.js'></script><script src='../common/js/highcharts.js'></script><script src='../common/js/series-label.js'></script><script src='../common/js/exporting.js'></script><script src='../common/js/export-data.js'></script><script src='../common/js/accessibility.js'></script><script src='../common/js/script.js'></script><link rel='stylesheet' href='../common/css/site.css'></head><body>";

        public const string SCRIPT_TAG = "<script>function onTabChangeClicked(e){var t=document.getElementsByClassName('tabDivClass');for(x=0;x<t.length;x++)t[x].style.display='none';'Home'==e?document.getElementById('Home-Div').style.display='block':'Current'==e?document.getElementById('CurrentAcc-Div').style.display='block':'Saving'==e&&(document.getElementById('SavingAcc-Div').style.display='block')}  </script>";

        public const string NAVBAR_HTML = "<nav class='navbar navbar-expand-sm bg-white navbar-light'>" +
            "<a class='navbar-brand ml-auto' href='javascript:void(0);'> <img src = '{{BrandLogo}}' height='60'></a> </nav> " +
            "<nav class='navbar navbar-expand-sm bg-dark navbar-dark'><div class='collapse navbar-collapse' id='collapsibleNavbar'>" +
            "<ul class='navbar-nav nav'>" + "{{NavItemList}}" + "</ul>" +
            "<ul class='navbar-nav ml-auto width80px'>" + "<li class='nav-item date'><a class='text-white'>{{Today}}</a></li>" + "</ul></div></nav>";

        public const string CUSTOMER_INFORMATION_WIDGET_HTML = "<div class='card border-0'>" +
            "<div class='card-header bg-light border-0 text-left'><h5 class='m-0'>Customer Information</h5></div>" +
            "<div class='card-body'><div class='row'><div class='col-sm-4'><h4 class='mb-4'>{{CustomerName}}</h4>" +
            "<h6>{{Address1}}<br /> {{Address2}}</h6></div>" +
            "<div class='col-sm-8'> <video class='doc-video' controls><source src='{{VideoSource}}' type='video/mp4'></video>" +
            "</div></div></div></div>";

        public const string ACCOUNT_INFORMATION_WIDGET_HTML = "<div class='card border-0'>" +
            "<div class='card-header bg-light border-0 text-left'><h5 class='m-0'>Account Information</h5>" +
            "</div><div class='card-body'>" + "{{AccountInfoData}}" + "</div></div>";

        public const string SUMMARY_AT_GLANCE_WIDGET_HTML = "<div class='card border-0'>" +
            "<div class='card-header bg-light border-0 text-left'> <h5 class='m-0'>Summary at Glance</h5></div>" +
            "<div class='card-body'><div class='table-responsive'> <table class='table m-0 table-hover'>" +
            "<thead><tr><th>Account</th><th>Currency</th><th>Amount</th></tr></thead><tbody>" +
            "{{AccountSummary}}</tbody></table></div></div></div>";

        public const string IMAGE_WIDGET_HTML = "<div class='card border-0'>" +
            "<div class='card-header bg-light border-0 text-left'><h5 class='m-0'>Image Information</h5></div>" +
            "<div class='card-body text-center'><img src='{{ImageSource}}' class='img-fluid'/></div></div>";

        public const string VIDEO_WIDGET_HTML = "<div class='card border-0'>" +
            "<div class='card-header bg-light border-0 text-left'><h5 class='m-0'>Video Information</h5> </div><div class='card-body text-center'>" +
            "<video class='video-widget' controls><source src='{{VideoSource}}' type='video/mp4'></video></div></div>";

        public const string NO_WIDGET_MESSAGE_HTML = "<div class='card border-0'>" +
            "<div class='card-header bg-light border-0'><h5 class='m-0'>No Configuration</h5> </div><div class='card-body text-center text-danger'>" +
            "<span>No configuration saved for this record.</span></div></div>";

        public const string SAVING_CURRENT_AVALABLE_BAL_WIDGET_HTML = "<div class='card border-0'>" +
            "<div class='card-header bg-light border-0 text-left'><h5 class='m-0'>Available Balance</h5> </div>" +
            "<div class='card-body'><div><h4 class='mb-4 text-right'>" +
            "<i class='fa fa-sort-asc text-success' aria-hidden='true'></i>&nbsp;{{TotalValue}}</h4>" +
            "<span class='float-left'> Total Deposits</span><span class='float-right'>{{TotalDeposit}}</span><br/>" +
            "<span class='float-left'> Total Spend</span><span class='float-right'>{{TotalSpend}}</span><br/>" +
            "<span class='float-left'> Savings </span><span class='float-right'>{{Savings}}</span><br/></div></div></div>";

        //public const string SAVING_TRANSACTION_WIDGET_HTML = "<div class='card border-0'><div class='card-header bg-light border-0 text-left'>" +
        //    "<h5 class='m-0'>Transaction Details</h5></div>" + "<div class='card-body'>" + "<div class='float-left'> " +
        //    "<input type='radio' id='showAll' name='showAll' value='showAll'>&nbsp;" + "<label for='showAll'>Show All</label>&nbsp;" +
        //    "<input type='radio' id='grpDate' name='grpDate' value='grpDate'>&nbsp;<labelfor='grpDate'>Group By Date</label></div>" +
        //    " <div class='float-right'> <a href='javascript:void(0)' class='btn btn-light btn-sm'>Search</a>&nbsp;<a href='javascript:void(0)' class='btn btn-light btn-sm'>Reset</a>&nbsp;<a href='javascript:void(0)' class='btn btn-light btn-sm'>Print</a> </div>" +
        //    "<div class='table-responsive'><table class='table m-1 table-hover table-sm'>" +
        //    "<thead><tr><th>Date</th><th>Type</th><th>Narration</th><th>Credit</th><th>Debit</th><th>Query</th><th>Balance</th></tr>" +
        //    "</thead><tbody>{{AccountTransactionDetails}}</tbody></table></div></div></div>";

        public const string CURRENT_SAVING_TRANSACTION_WIDGET_HTML = "<div class='card border-0'><div class='card-header bg-light border-0 text-left'>" +
            "<h5 class='m-0'>Transaction Details</h5></div>" + "<div class='card-body'>" + "<div class='float-left'> " +
            "<input type='radio' id='showAll' name='showAll' value='showAll'>&nbsp;" + "<label for='showAll'>Show All</label>&nbsp;" +
            "<input type='radio' id='grpDate' name='grpDate' value='grpDate'>&nbsp;<labelfor='grpDate'>Group By Date</label></div>" +
            " <div class='float-right'> <a href='javascript:void(0)' class='btn btn-light btn-sm'>Search</a>&nbsp;" +
            "<a href='javascript:void(0)' class='btn btn-light btn-sm'>Reset</a>&nbsp;<a href='javascript:void(0)' class='btn btn-light btn-sm'>Print</a> </div>" +
            "<div class='table-responsive'><table class='table m-1 table-hover'><thead><tr>" +
            "<th class='width12'>Date</th><th class='width8'>Type</th class='width30'><th>Narration</th><th class='width12'>FCY</th><th class='width13'>Current Rate</th> <th class='width13'>LCY</th><th class='width12'>Action</th></tr>" +
            "</thead><tbody>{{AccountTransactionDetails}}</tbody></table></div></div></div>";

        public const string REMINDER_WIDGET_HTML = "<div class='card border-0'><div class='card-header bg-light border-0 text-left'>" +
            "<h5 class='m-0'>Reminder and Recommendation</h5></div><div class='card-body'> <div class='table-responsive'><table><thead><tr><td class='width75'></td>" +
            "<td class='text-danger width25'> <span><i class='fa fa-caret-left fa-2x' aria-hidden='true'></i>Click</span></td></tr></thead>" +
            "<tbody>{{ReminderAndRecommdationDataList}}</tbody></table> " +
            "</div></div></div>";

        public const string TOP_4_INCOME_SOURCE_WIDGET_HTML = "<div class='card border-0'><div class='card-header bg-light border-0 text-left'>" +
            "<h5 class='m-0'>Top 4 Income Sources</h5></div><div class='card-body'><div class='table-responsive'><table><thead class='border-bottom'><tr>" +
            "<td class='width50'></td><td class='width17'>This Month</td><td class='width33'>Usually you spend</td></tr>" +
            "</thead><tbody>{{IncomeSourceList}}</tbody></table></div></div></div>";

        public const string ANALYTIC_WIDGET_HTML = "<div class='card border-0'><div class='card-header bg-light border-0 text-left'> " +
            "<h5 class='m-0'>Analytics</h5></div>" + "<div class='card-body'> <div id=\"analyticschartcontainer\"></div></div></div> ";

        public const string SAVING_TRENDS_WIDGET_HTML = "<div class='card border-0'><div class='card-header bg-light border-0 text-left'> " +
            "<h5 class='m-0'>Saving Trends</h5></div><div class='card-body'> <div id=\"savingTrendscontainer\"></div></div></div> ";

        public const string SPENDING_TRENDS_WIDGET_HTML = "<div class='card border-0'><div class='card-header bg-light border-0 text-left'> " +
            "<h5 class='m-0'>Spending Trends</h5></div><div class='card-body'> <div id=\"spendingTrendscontainer\"></div></div></div> ";

        public const string CONTAINER_DIV_HTML_HEADER = "<div class='container-fluid mt-3 mb-3 bdy-scroll stylescrollbar'>";

        public const string WIDGET_HTML_HEADER = "<div id='{{DivId}}' class='card border-0 p-2 tabDivClass {{ExtraClass}}'>";

        public const string WIDGET_HTML_FOOTER = "</div>";

        public const string CONTAINER_DIV_HTML_FOOTER = "</div>";

        public const string HOME_PAGE_DIV_NAME = "Home-Div";

        public const string SAVING_ACCOUNT_PAGE_DIV_NAME = "SavingAcc-Div";

        public const string CURRENT_ACCOUNT_PAGE_DIV_NAME = "CurrentAcc-Div";

        public const string TAB_NAVIGATION_SCRIPT = "<script type='text/javascript'>$(document).ready(function(){$('.nav-link').click(function(t){$('.tabDivClass').hide(),$('.nav-link').removeClass('active');let a='active '+$(t.currentTarget).attr('class');$(t.currentTarget).attr('class',a);let e=$(t.currentTarget).attr('class').split(' '),n=e[e.length-1];$('.'+n).hasClass('d-none')&&$('.'+n).removeClass('d-none'),$('.'+n).show()})});</script>";

        public const string ANALYTICS_CHART_WIDGET_SCRIPT = "<script type='text/javascript'> setTimeout(function () { Highcharts.chart('analyticschartcontainer', { chart: { plotBackgroundColor: null, plotBorderWidth: null, plotShadow: !1, type: 'pie' }, title: { text: '' }, tooltip: { pointFormat: '{series.name}: <b>{point.percentage:.1f}%</b>' }, accessibility: { point: { valueSuffix: '%' } }, plotOptions: { pie: { allowPointSelect: !0, cursor: 'pointer', dataLabels: { enabled: !0, format: '{point.percentage:.1f} %' }, showInLegend: !0 } }, series: [{ name: 'Percentage', colorByPoint: !0, data: [{ name: 'Cutomer Information', y: 11.84 }, { name: 'Account Information', y: 10.85 }, { name: 'Image', y: 4.67 }, { name: 'Video', y: 4.18 }, { name: 'News Alerts', y: 7.05 }] }] }) }, 100); </script>";

        public const string HTML_FOOTER = "</body></html>";
    }
}
