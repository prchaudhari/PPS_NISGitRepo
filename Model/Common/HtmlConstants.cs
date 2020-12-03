// <copyright file="HtmlConstants.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2020 Websym Solutions Pvt Ltd.
// </copyright>
//------------------------------------------------------------------------------

namespace nIS
{
    public class HtmlConstants
    {
        public const int HOME_PAGE_TYPE_ID = 1;

        public const int SAVING_ACCOUNT_PAGE_TYPE_ID = 2;

        public const int CURRENT_ACCOUNT_PAGE_TYPE_ID = 3;

        public const string HOME_PAGE = "Home";

        public const string CURRENT_ACCOUNT_PAGE = "Current Account";

        public const string SAVING_ACCOUNT_PAGE = "Saving Account";

        public const string CUSTOMER_INFORMATION_WIDGET_NAME = "CustomerInformation";

        public const string ACCOUNT_INFORMATION_WIDGET_NAME = "AccountInformation";

        public const string SUMMARY_AT_GLANCE_WIDGET_NAME = "Summary";

        public const string IMAGE_WIDGET_NAME = "Image";

        public const string VIDEO_WIDGET_NAME = "Video";

        public const string ANALYTICS_WIDGET_NAME = "Analytics";

        public const string SAVING_TRANSACTION_WIDGET_NAME = "SavingTransaction";

        public const string CURRENT_TRANSACTION_WIDGET_NAME = "CurrentTransaction";

        public const string SAVING_TREND_WIDGET_NAME = "SavingTrend";

        public const string TOP_4_INCOME_SOURCE_WIDGET_NAME = "Top4IncomeSources";

        public const string CURRENT_AVAILABLE_BALANCE_WIDGET_NAME = "CurrentAvailableBalance";

        public const string SAVING_AVAILABLE_BALANCE_WIDGET_NAME = "SavingAvailableBalance";

        public const string REMINDER_AND_RECOMMENDATION_WIDGET_NAME = "ReminderaAndRecommendation";

        public const string SPENDING_TREND_WIDGET_NAME = "SpendingTrend";

        public const string TABLE_DYNAMICWIDGET = "Table";

        public const string FORM_DYNAMICWIDGET = "Form";

        public const string LINEGRAPH_DYNAMICWIDGET = "LineGraph";

        public const string BARGRAPH_DYNAMICWIDGET = "BarGraph";

        public const string PICHART_DYNAMICWIDGET = "PieChart";

        public const string HTML_DYNAMICWIDGET = "Html";

        public const string HTML_HEADER = "<html><head><title>NIS Statement</title><meta charset='utf-8'><meta name='viewport' content='width=device-width, initial-scale=1'><link rel='stylesheet' href='../common/css/bootstrap.min.css'><link rel='stylesheet' href='../common/css/font-awesome.min.css'><script src='../common/js/jquery.min.js'></script><script src='../common/js/popper.min.js'></script><script src='../common/js/bootstrap.min.js'></script><script src='../common/js/highcharts.js'></script><script src='../common/js/series-label.js'></script><script src='../common/js/exporting.js'></script><script src='../common/js/export-data.js'></script><script src='../common/js/accessibility.js'></script><script src='../common/js/script.js'></script><link rel='stylesheet' href='../common/css/site.css'><link rel='stylesheet' href='../common/css/ltr.css'></head><body onload='onPageLoad()'> <input type='hidden' id='StatementId' name='StatementId' value='{{StatementNumber}}'> <input type='hidden' id='CustomerId' name='CustomerId' value='{{CustomerNumber}}'><input type='hidden' id='FirstPageId' name='FirstPageId' value='{{FirstPageId}}'><input type='hidden' id='TenantCode' name='TenantCode' value='{{TenantCode}}'>";

        public const string NAVBAR_HTML_FOR_PREVIEW = "<nav class='navbar navbar-expand-sm bg-white navbar-light p-0'><a href='javascript:void(0);' class='navbar-brand ml-3'> <img src='{{logo}}' height='50'></a><a class='navbar-brand ml-auto' href='javascript:void(0);' id='TenantLogo'> </a> </nav><nav class='navbar navbar-expand-sm bg-dark navbar-dark'><div class='collapse navbar-collapse' id='collapsibleNavbar'><ul class='navbar-nav nav'>" + "{{NavItemList}}" + "</ul><ul class='navbar-nav ml-auto'><li class='nav-item date'><a class='text-white'>{{Today}}</a></li></ul></div></nav>";

        public const string NAVBAR_HTML = "<nav class='navbar navbar-expand-sm bg-white navbar-light p-0'><a href='javascript:void(0);' class='navbar-brand ml-3'> <img src='{{logo}}' height='50'></a><a class='navbar-brand ml-auto' href='javascript:void(0);'> <img id='TenantLogo' src='{{BrandLogo}}' height='50'></a> </nav> <nav class='navbar navbar-expand-sm bg-dark navbar-dark'><div class='collapse navbar-collapse' id='collapsibleNavbar'><ul class='navbar-nav nav'>" + "{{NavItemList}}" + "</ul><ul class='navbar-nav ml-auto'><li class='nav-item date'><a class='text-white'>{{Today}}</a></li></ul></div></nav>";

        public const string CUSTOMER_INFORMATION_WIDGET_HTML = "<div class='card border-0' style='height:{{WidgetDivHeight}}'><div class='p-1 bg-light border-0 text-left'><h5 class='m-0'>Customer Information</h5></div><div class='card-body'><div class='row'><div class='col-sm-4'><h4 class='mb-4'>{{CustomerName}}</h4><h6>{{Address1}}{{Address2}}</h6></div><div class='col-sm-8'> <video class='doc-video' controls><source src='{{VideoSource}}' type='video/mp4'></video></div></div></div></div>";

        public const string ACCOUNT_INFORMATION_WIDGET_HTML = "<div class='card border-0' style='height:{{WidgetDivHeight}}'><div class='p-1 bg-light border-0 text-left'><h5 class='m-0'>Account Information</h5></div> <div class='card-body overflow-auto'>" + "{{AccountInfoData}}" + "</div></div>";

        public const string SUMMARY_AT_GLANCE_WIDGET_HTML = "<div class='card border-0' style='height:{{WidgetDivHeight}}'><div class='p-1 bg-light border-0 text-left'> <h5 class='m-0'>Summary at Glance</h5></div> <div class='card-body overflow-auto'><div class='table-responsive'> <table class='table m-0 table-hover'><thead><tr><th>Account</th><th>Currency</th><th>Amount</th></tr></thead><tbody>{{AccountSummary}}</tbody></table></div></div></div>";

        public const string IMAGE_WIDGET_HTML = "<div class='card border-0' style='height:{{WidgetDivHeight}}'><div class='card-body text-center'><img src='{{ImageSource}}' class='img-fluid {{NewImageClass}}'/></div></div>";

        public const string VIDEO_WIDGET_HTML = "<div class='card border-0' style='height:{{WidgetDivHeight}}'> <div class='card-body text-center'><video class='video-widget {{NewVideoClass}}' controls><source src='{{VideoSource}}' type='video/mp4'></video></div></div>";

        public const string NO_WIDGET_MESSAGE_HTML = "<div class='card border-0'><div class='p-1 bg-light border-0'><h5 class='m-0'>No Configuration</h5> </div>" +
            "<div class='card-body text-center text-danger'><span>No configuration saved for this record.</span></div></div>";

        public const string SAVING_CURRENT_AVALABLE_BAL_WIDGET_HTML = "<div class='card border-0' style='height:{{WidgetDivHeight}}'>" +
            "<div class='p-1 bg-light border-0 text-left'><h5 class='m-0'>Available Balance</h5> </div>" +
            "<div class='card-body overflow-auto'><div class='fnt14'><h4 class='mb-4 text-right'><i class='{{AccountIndicatorClass}}' aria-hidden='true'></i>&nbsp;{{TotalValue}}</h4>" +
            "<span class='float-left'> Total Deposits</span><span class='float-right'>{{TotalDeposit}}</span><br/>" +
            "<span class='float-left'> Total Spend</span><span class='float-right'>{{TotalSpend}}</span><br/>" +
            "<span class='float-left'> Savings </span><span class='float-right'>{{Savings}}</span><br/></div></div></div>";

        public const string SAVING_TRANSACTION_WIDGET_HTML = "<div class='card border-0' style='height:{{WidgetDivHeight}}'><div class='p-1 bg-light border-0 text-left'>" +
            "<h5 class='m-0'>Transaction Details</h5></div>" + "<div class='card-body overflow-auto'>" + "<div class='float-left'> " +
            "<input type='radio' id='savingShowAll' checked name='savingtransactionRadio'>&nbsp;<label for='showAll'>Show All</label>&nbsp;" +
            "<input type='radio' id='savingGrpDate' name='savingtransactionRadio'>&nbsp;<label for='grpDate'>Group By Date</label></div>" +
            " <div class='float-right'><div class='float-left mr-2'><select class='form-control float-left' id='filterStatus'><option value = '0'> Search Item</option>" +
            "{{SelectOption}} </select></div>" + "<a href='javascript:void(0)' class='btn btn-light btn-sm' id='ResetGrid'>Reset</a>&nbsp;" +
             "<a href='javascript:void(0)' class='btn btn-light btn-sm' id='PrintGrid'>Print</a> </div>" + "<div class='table-responsive stylescrollbar' " +
                "style='max-height:350px;overflow-x:hidden;overflow-y:auto;'><table id='SavingTransactionTable' class='table m-1 table-hover'><thead><tr>" +
            "<th class='width12'>Date</th><th class='width8'>Type</th class='width30'><th>Narration</th><th class='width12 text-right'>FCY</th><th class='width13 text-right'>" +
                "Current Rate</th> <th class='width13 text-right'>LCY</th><th class='width12'>Action</th></tr>" +
            "</thead><tbody>{{AccountTransactionDetails}}</tbody></table></div></div></div>";

        public const string CURRENT_TRANSACTION_WIDGET_HTML = "<div class='card border-0' style='height:{{WidgetDivHeight}}'><div class='p-1 bg-light border-0 text-left'>" +
            "<h5 class='m-0'>Transaction Details</h5></div>" + "<div class='card-body'>" + "<div class='float-left'> " +
            "<input type='radio' id='currentShowAll' checked name='currenttransactionRadio'>&nbsp;<label for='showAll'>Show All</label>&nbsp;" +
            "<input type='radio' id='currentGrpDate' name='currenttransactionRadio'>&nbsp;<label for='grpDate'>Group By Date</label></div>" +
            " <div class='float-right'><div class='float-left mr-2'><select class='form-control float-left' id='filterStatus'><option value = '0'> Search Item</option>" +
            "{{SelectOption}} </select></div>" + "<a href='javascript:void(0)' class='btn btn-light btn-sm' id='ResetGrid'>Reset</a>&nbsp;" +
             "<a href='javascript:void(0)' class='btn btn-light btn-sm' id='PrintGrid'>Print</a> </div>" + "<div class='table-responsive stylescrollbar' " +
                "style='max-height:350px;overflow-x:hidden;overflow-y:auto;'><table id='CurrentTransactionTable' class='table m-1 table-hover'><thead><tr>" +
            "<th class='width12'>Date</th><th class='width8'>Type</th class='width30'><th>Narration</th><th class='width12 text-right'>FCY</th><th class='width13 text-right'>" +
                "Current Rate</th> <th class='width13 text-right'>LCY</th><th class='width12'>Action</th></tr>" +
            "</thead><tbody>{{AccountTransactionDetails}}</tbody></table></div></div></div>";

        public const string REMINDER_WIDGET_HTML = "<div class='card border-0' style='height:{{WidgetDivHeight}}'><div class='p-1 bg-light border-0 text-left'><h5 class='m-0'>" +
            "Reminder and Recommendation</h5></div><div class='card-body overflow-auto' style='font-size:12px;'> {{ReminderAndRecommdationDataList}} </div></div>";

        public const string TOP_4_INCOME_SOURCE_WIDGET_HTML = "<div class='card border-0' style='height:{{WidgetDivHeight}}'><div class='card-header bg-light border-0 text-left'>" +
            "<h5 class='m-0'>Top 4 Income Sources</h5></div><div class='card-body'><table class='table-borderless width100'><thead class='border-bottom'><tr>" +
            "<td class='width50'></td><td class='width20'>This Month</td><td class='width30'>Usually you spend</td></tr>" +
            "</thead><tbody>{{IncomeSourceList}}</tbody></table></div></div>";

        public const string ANALYTIC_WIDGET_HTML = "<div class='card border-0' style='height:{{WidgetDivHeight}}'><div class='p-1 bg-light border-0 text-left'> " +
            "<h5 class='m-0'>Analytics</h5></div>" + "<div class='card-body'> <div id=\"analyticschartcontainer\" style='height: 75%; width: 90%; position: absolute;'></div></div></div> ";

        public const string SAVING_TRENDS_WIDGET_HTML = "<div class='card border-0' style='height:{{WidgetDivHeight}}'><div class='p-1 bg-light border-0 text-left'> " +
            "<h5 class='m-0'>Saving Trends</h5></div><div class='card-body'> <div id=\"savingTrendscontainer\" style='height: 75%; width: 90%; position: absolute;'></div></div></div> ";

        public const string SPENDING_TRENDS_WIDGET_HTML = "<div class='card border-0' style='height:{{WidgetDivHeight}}'><div class='p-1 bg-light border-0 text-left'> " +
            "<h5 class='m-0'>Spending Trends</h5></div><div class='card-body'> <div id=\"spendingTrendscontainer\" style='height: 75%; width: 90%; position: absolute;'></div></div></div> ";

        public const string CUSTOMER_INFORMATION_WIDGET_HTML_FOR_STMT = "<div id={{WidgetId}} class='card border-0' style='height:{{WidgetDivHeight}}'><div class='card-header bg-light border-0 text-left'><h5 class='m-0'>Customer Information</h5></div><div class='card-body'><div class='row'><div class='col-sm-4'><h4 class='mb-4'>{{CustomerName}}</h4>" + "<h6>{{Address1}}{{Address2}}</h6></div><div class='col-sm-8'> <video class='doc-video' controls><source src='{{VideoSource}}' type='video/mp4'></video>" + "</div></div></div></div>";

        public const string ACCOUNT_INFORMATION_WIDGET_HTML_FOR_STMT = "<div id={{WidgetId}} class='card border-0' style='height:{{WidgetDivHeight}}'><div class='card-header bg-light border-0 text-left'><h5 class='m-0'>Account Information</h5></div> <div class='card-body'>{{AccountInfoData}}</div></div>";

        public const string SUMMARY_AT_GLANCE_WIDGET_HTML_FOR_STMT = "<div id={{WidgetId}} class='card border-0' style='height:{{WidgetDivHeight}}'><div class='card-header bg-light border-0 text-left'> <h5 class='m-0'>Summary at Glance</h5></div> <div class='card-body'><div class='table-responsive'> <table class='table m-0 table-hover'><thead><tr><th>Account</th><th>Currency</th><th>Amount</th></tr></thead><tbody>" + "{{AccountSummary}}</tbody></table></div></div></div>";

        public const string IMAGE_WIDGET_HTML_FOR_STMT = "<div id={{WidgetId}} class='card border-0' style='height:{{WidgetDivHeight}}'><div class='card-header bg-light border-0 text-left'><h5 class='m-0'>Image Information</h5></div> <div class='card-body text-center'>{{TargetLink}}<img src='{{ImageSource}}' class='img-fluid'/>{{EndTargetLink}}</div></div>";

        public const string VIDEO_WIDGET_HTML_FOR_STMT = "<div id={{WidgetId}} class='card border-0' style='height:{{WidgetDivHeight}}'><div class='card-header bg-light border-0 text-left'><h5 class='m-0'>Video Information</h5> </div> <div class='card-body text-center'><video class='video-widget' controls><source src='{{VideoSource}}' type='video/mp4'></video></div></div>";

        public const string SAVING_CURRENT_AVALABLE_BAL_WIDGET_HTML_FOR_STMT = "<div id={{WidgetId}} class='card border-0' style='height:{{WidgetDivHeight}}'><div class='card-header bg-light border-0 text-left'><h5 class='m-0'>Available Balance</h5> </div><div class='card-body'><div class='fnt14'><h4 class='mb-4 text-right'><i class='{{AccountIndicatorClass}}' aria-hidden='true'></i>&nbsp;{{TotalValue}}</h4><span class='float-left'> Total Deposits</span><span class='float-right'>{{TotalDeposit}}</span><br/><span class='float-left'> Total Spend </span><span class='float-right'>{{TotalSpend}}</span><br/><span class='float-left'> Savings </span><span class='float-right'>{{Savings}}</span><br/></div></div></div>";

        public const string SAVING_TRANSACTION_WIDGET_HTML_FOR_STMT = "<div id={{WidgetId}} class='card border-0' style='height:{{WidgetDivHeight}}'><div class='card-header bg-light border-0 text-left'>" + "<h5 class='m-0'>Transaction Details</h5></div>" + "<div class='card-body'>" + "<div class='float-left'> " +
        "<input type='radio' id='savingShowAll' checked name='savingtransactionRadio'>&nbsp;<label for='showAll'>Show All</label>&nbsp;" +
        "<input type='radio' id='savingGrpDate' name='savingtransactionRadio'>&nbsp;<label for='grpDate'>Group By Date</label></div>" +
        " <div class='float-right'><div class='float-left mr-2'><select class='form-control float-left' id='filterStatus'><option value = '0'> Search Item</option>" +
        "{{SelectOption}} </select></div>" + "<a href='javascript:void(0)' class='btn btn-light btn-sm' id='ResetGrid'>Reset</a>&nbsp;" +
        "<a href='javascript:void(0)' class='btn btn-light btn-sm' id='PrintGrid'>Print</a> </div>" + "<div class='table-responsive stylescrollbar' " +
        "style='max-height:320px;overflow-x:hidden;overflow-y:auto;'><table id='SavingTransactionTable' class='table m-1 table-hover'><thead><tr>" +
        "<th class='width12'>Date</th><th class='width8'>Type</th class='width30'><th>Narration</th><th class='width12 text-right'>FCY</th><th class='width13 text-right'>" +
        "Current Rate</th> <th class='width13 text-right'>LCY</th><th class='width12'>Action</th></tr></thead><tbody>{{AccountTransactionDetails}}</tbody></table></div></div></div>";

        public const string CURRENT_TRANSACTION_WIDGET_HTML_FOR_STMT = "<div id={{WidgetId}} class='card border-0' style='height:{{WidgetDivHeight}}'><div class='card-header bg-light border-0 text-left'>" + "<h5 class='m-0'>Transaction Details</h5></div>" + "<div class='card-body'>" + "<div class='float-left'> " +
        "<input type='radio' id='currentShowAll' checked name='currenttransactionRadio'>&nbsp;<label for='showAll'>Show All</label>&nbsp;" +
        "<input type='radio' id='currentGrpDate' name='currenttransactionRadio'>&nbsp;<label for='grpDate'>Group By Date</label></div>" +
        " <div class='float-right'><div class='float-left mr-2'><select class='form-control float-left' id='filterStatus'><option value = '0'> Search Item</option>" +
        "{{SelectOption}} </select></div>" + "<a href='javascript:void(0)' class='btn btn-light btn-sm' id='ResetGrid'>Reset</a>&nbsp;" +
         "<a href='javascript:void(0)' class='btn btn-light btn-sm' id='PrintGrid'>Print</a> </div>" + "<div class='table-responsive stylescrollbar' " +
         "style='max-height:320px;overflow-x:hidden;overflow-y:auto;'><table id='CurrentTransactionTable' class='table m-1 table-hover'><thead><tr>" +
        "<th class='width12'>Date</th><th class='width8'>Type</th class='width30'><th>Narration</th><th class='width12 text-right'>FCY</th><th class='width13 text-right'>" +
         "Current Rate</th> <th class='width13 text-right'>LCY</th><th class='width12'>Action</th></tr></thead><tbody>{{AccountTransactionDetails}}</tbody></table></div></div></div>";

        public const string REMINDER_WIDGET_HTML_FOR_STMT = "<div id={{WidgetId}} class='card border-0' style='height:{{WidgetDivHeight}}'><div class='card-header bg-light border-0 text-left'><h5 class='m-0'>Reminder and Recommendation</h5></div><div class='card-body' style='font-size:12px;'> {{ReminderAndRecommdationDataList}} </div></div>";

        public const string TOP_4_INCOME_SOURCE_WIDGET_HTML_FOR_STMT = "<div id={{WidgetId}} class='card border-0' style='height:{{WidgetDivHeight}}'><div class='card-header bg-light border-0 text-left'>" + "<h5 class='m-0'>Top 4 Income Sources</h5></div><div class='card-body'><table class='table-borderless width100'><thead class='border-bottom'><tr><td class='width50'></td><td class='width20'>This Month</td><td class='width30'>Usually you spend</td></tr></thead><tbody>{{IncomeSourceList}}</tbody></table></div></div>";

        public const string ANALYTIC_WIDGET_HTML_FOR_STMT = "<div id={{WidgetId}} class='card border-0' style='height:{{WidgetDivHeight}}'><div class='card-header bg-light border-0 text-left'> <h5 class='m-0'>Analytics</h5></div><div class='card-body'> <div id=\"analyticschartcontainer\" style='height: 75%; width: 90%; position: absolute;'></div></div></div> ";

        public const string SAVING_TRENDS_WIDGET_HTML_FOR_STMT = "<div id={{WidgetId}} class='card border-0' style='height:{{WidgetDivHeight}}'><div class='card-header bg-light border-0 text-left'><h5 class='m-0'>Saving Trends</h5></div><div class='card-body'> <div id=\"savingTrendscontainer\" style='height: 75%; width: 90%; position: absolute;'></div></div></div> ";

        public const string SPENDING_TRENDS_WIDGET_HTML_FOR_STMT = "<div id={{WidgetId}} class='card border-0' style='height:{{WidgetDivHeight}}'><div class='card-header bg-light border-0 text-left'><h5 class='m-0'>Spending Trends</h5></div><div class='card-body'> <div id=\"spendingTrendscontainer\" style='height: 75%; width: 90%; position: absolute;'></div></div></div> ";

        public const string TABLE_WIDEGT_FOR_STMT = "<div id={{WidgetId}} class='card border-0' style='height:{{WidgetDivHeight}}'><div class='p-1 bg-light border-0 text-left'><h5 class='m-0;' style={{TitleStyle}}>{{WidgetTitle}}</h5></div><div class=''><div class='table-responsive stylescrollbar' style='max-height:{{TableMaxHeight}};overflow-x:hidden;overflow-y:auto;'> <table id='TableWidget' class='table m-1 table-hover'><thead style={{HeaderStyle}}>{{tableHeader}}</thead><tbody style={{BodyStyle}}>{{tableBody}}</tbody></table></div></div></div>";

        public const string FORM_WIDGET_FOR_STMT = "<div id={{WidgetId}} class='card border-0' style='height:{{WidgetDivHeight}}'><div class='p-1 bg-light border-0 text-left' style='margin-bottom: 10px;'><h5 class='m-0' style={{TitleStyle}}>{{WidgetTitle}}</h5></div><div class='card border-0 card-shadow width100'><div class='card-body p-2' style={{BodyStyle}}>{{FormData}}</div></div></div>";

        public const string HTML_WIDGET_FOR_STMT = "<div id={{WidgetId}} class='card border-0' style='height:{{WidgetDivHeight}}'><div class='p-1 bg-light border-0 text-left'><h5 class='m-0' style={{TitleStyle}}>{{WidgetTitle}}</h5></div><div class='card-body text-left' style={{BodyStyle}}>{{FormData}}</div></div>";

        public const string LINE_GRAPH_FOR_STMT = "<div id={{WidgetId}} class='card border-0' style='height:{{WidgetDivHeight}}'><div class='p-1 bg-light border-0 text-left'><h5 class='m-0' style={{TitleStyle}}>{{WidgetTitle}}</h5></div><div class='card-body'> <div id=\"lineGraphcontainer\" style='height: 75%; width: 90%; position: absolute;'></div></div></div> ";

        public const string PIE_CHART_FOR_STMT = "<div id={{WidgetId}} class='card border-0' style='height:{{WidgetDivHeight}}'><div class='p-1 bg-light border-0 text-left'><h5 class='m-0' style={{TitleStyle}}>{{WidgetTitle}}</h5></div><div class='card-body'> <div id=\"pieChartcontainer\" style='height: 75%; width: 90%; position: absolute;'></div></div></div> ";

        public const string BAR_GRAPH_FOR_STMT = "<div id={{WidgetId}} class='card border-0' style='height:{{WidgetDivHeight}}'><div class='p-1 bg-light border-0 text-left'> <h5 class='m-0' style={{TitleStyle}}>{{WidgetTitle}}</h5></div><div class='card-body'> <div id=\"barGraphcontainer\" style='height: 75%; width: 90%; position: absolute;'></div></div></div> ";

        public const string CONTAINER_DIV_HTML_HEADER = "<div class='container-fluid mt-3 mb-3 bdy-scroll stylescrollbar'>";

        public const string WIDGET_HTML_HEADER = "<div id='{{DivId}}' class='card border-0 p-2 tabDivClass {{ExtraClass}}'>";

        public const string PAGE_HEADER_HTML = "<div id='{{DivId}}' class='card border-0 p-2 tabDivClass {{ExtraClass}}' {{BackgroundImage}}>";

        public const string PAGE_TAB_CONTENT_HEADER = "<div class='tab-content'>";

        public const string WIDGET_HTML_FOOTER = "</div>";

        public const string PAGE_TAB_CONTENT_FOOTER = "</div>";

        public const string CONTAINER_DIV_HTML_FOOTER = "</div>";

        public const string END_DIV_TAG = "</div>";

        public const string START_ROW_DIV_TAG = "<div class='row'>";

        public const string HOME_PAGE_DIV_NAME = "Home-Div";

        public const string SAVING_ACCOUNT_PAGE_DIV_NAME = "SavingAcc-Div";

        public const string CURRENT_ACCOUNT_PAGE_DIV_NAME = "CurrentAcc-Div";

        public const string TAB_NAVIGATION_SCRIPT = "<script type='text/javascript'>$(document).ready(function(){$('.nav-link').click(function(t){$('.tabDivClass').hide(),$('.nav-link').removeClass('active');let a='active '+$(t.currentTarget).attr('class');$(t.currentTarget).attr('class',a);let e=$(t.currentTarget).attr('class').split(' '),n=e[e.length-1];$('.'+n).hasClass('d-none')&&$('.'+n).removeClass('d-none'),$('.'+n).show()})});</script>";

        public const string ANALYTICS_CHART_WIDGET_SCRIPT = "<script type='text/javascript'> setTimeout(function(){if(0!=analyticsdata.length){var t=analyticsdata,e=[];for(i=0;i<t.length;i++){var a={name:t[i].AccountType,y:t[i].Percentage};e.push(a)}Highcharts.chart('analyticschartcontainer',{chart:{plotBackgroundColor:null,plotBorderWidth:null,plotShadow:!1,type:'pie'},title:{text:''},tooltip:{pointFormat:'{series.name}: <b>{point.percentage:.1f}%</b>'},accessibility:{point:{valueSuffix:'%'}},plotOptions:{pie:{allowPointSelect:!0,cursor:'pointer',dataLabels:{enabled:!0,format:'{point.percentage:.1f} %'},showInLegend:!0}},series:[{name:'Percentage',colorByPoint:!0,data:e}]})}else $('#analyticschartcontainer').html('<div class=\"text-danger text-center\">No data available.</div>')},100); </script>";

        public const string SAVING_TREND_CHART_WIDGET_SCRIPT = "<script type='text/javascript'>setTimeout(function(){if(0!=savingdata.length){var t=savingdata,e=[],a=[];for(i=0;i<t.length;i++)e.push(t[i].Month),a.push(t[i].Income);Highcharts.chart('savingTrendscontainer',{title:{text:''},xAxis:{categories:e},labels:{items:[{style:{left:'50px',top:'18px',color:Highcharts.defaultOptions.title.style&&Highcharts.defaultOptions.title.style.color||'black'}}]},series:[{name:'',data:a,marker:{lineWidth:1,lineColor:Highcharts.getOptions().colors[3],fillColor:'white'}}]})}else $('#savingTrendscontainer').html('<div class=\"text-danger text-center\">No data available.</div>')},100);</script>";

        public const string SPENDING_TREND_CHART_WIDGET_SCRIPT = "<script type='text/javascript'>setTimeout(function(){if(0!=spendingdata.length){var t=spendingdata,e=[],a=[],l=[],n=[];for(i=0;i<t.length;i++)e.push(t[i].Month),a.push(t[i].Income),l.push(t[i].SpendAmount),n.push(t[i].SpendPercentage);Highcharts.chart('spendingTrendscontainer',{title:{text:''},xAxis:{categories:e},labels:{items:[{style:{left:'50px',top:'18px',color:Highcharts.defaultOptions.title.style&&Highcharts.defaultOptions.title.style.color||'black'}}]},series:[{type:'column',name:'Your Income',data:a},{type:'column',name:'Your Spending',data:l},{type:'spline',name:'',data:n,marker:{lineWidth:2,lineColor:Highcharts.getOptions().colors[3],fillColor:'white'}}]})}else $('#spendingTrendscontainer').html('<div class=\"text-danger text-center\">No data available.</div>')},100);</script>";

        public const string SAVING_TRANSACTION_DETAIL_GRID_WIDGET_SCRIPT = "<script type='text/javascript'>$(document).ready(function(){setTimeout(function(){if(null!=savingtransactiondata&&savingtransactiondata.length>0){var t=savingtransactiondata;$('input[type=\"radio\"][name=\"savingtransactionRadio\"]').change(function(a){if('savingShowAll'==a.currentTarget.id)$('#SavingTransactionTable tbody tr').remove(),$.each(t,function(t,a){var e=$('#SavingTransactionTable tbody'),n=$('<tr>');n.append($('<td>',{text:a.TransactionDate})),n.append($('<td>',{text:a.TransactionType})),n.append($('<td>',{text:a.Narration})),n.append($('<td>',{text:parseFloat(a.FCY).toFixed(2),class:'text-right'})),n.append($('<td>',{text:parseFloat(a.CurrentRate).toFixed(2),class:'text-right'})),n.append($('<td>',{text:parseFloat(a.LCY).toFixed(2),class:'text-right'})),n.append($('<td>',{html:'<div class=\"action-btns btn-tbl-action\"><button type=\"button\" title=\"View\"><span class=\"fa fa-paper-plane-o\"></span></button></div>'})),e.append(n)});else{$('#SavingTransactionTable tbody tr').remove();var e=t.reduce(function(t,a){const e=a.TransactionDate;return t[e]=t[e]||[],t[e].push(a),t},{});$.each(e,function(t,a){var e=0,n=0,i=0;a.length>1?(e=a.reduce(function(t,a){return t+parseFloat(a.FCY)},0),n=a.reduce(function(t,a){return t+parseFloat(a.LCY)},0),i=a.reduce(function(t,a){return t+parseFloat(a.CurrentRate)},0)):(e=a[0].FCY,n=a[0].LCY,i=a[0].CurrentRate);var r=$('#SavingTransactionTable tbody'),o=$('<tr>');o.append($('<td>',{text:a[0].TransactionDate})),o.append($('<td>',{text:a[0].TransactionType})),o.append($('<td>',{text:'-'})),o.append($('<td>',{text:parseFloat(e).toFixed(2),class:'text-right'})),o.append($('<td>',{text:parseFloat(i).toFixed(2),class:'text-right'})),o.append($('<td>',{text:parseFloat(n).toFixed(2),class:'text-right'})),o.append($('<td>',{text:'-'})),r.append(o)})}}),$('input[type=\"radio\"][id=\"savingShowAll\"]').prop('checked',!0).trigger('change'),$('#filterStatus').on('change',function(){val=this.value;var t=[];t='0'==val?savingtransactiondata:jQuery.grep(savingtransactiondata,function(t){return t.Narration==val}),$('#SavingTransactionTable tbody tr').remove(),$.each(t,function(t,a){var e=$('#SavingTransactionTable tbody'),n=$('<tr>');n.append($('<td>',{text:a.TransactionDate})),n.append($('<td>',{text:a.TransactionType})),n.append($('<td>',{text:a.Narration})),n.append($('<td>',{text:parseFloat(a.FCY).toFixed(2),class:'text-right'})),n.append($('<td>',{text:parseFloat(a.CurrentRate).toFixed(2),class:'text-right'})),n.append($('<td>',{text:parseFloat(a.LCY).toFixed(2),class:'text-right'})),n.append($('<td>',{html:'<div class=\"action-btns btn-tbl-action\"><button type=\"button\" title=\"View\"><span class=\"fa fa-paper-plane-o\"></span></button></div>'})),e.append(n)})}),$('#ResetGrid').on('click',function(){$('#filterStatus').prop('selectedIndex','0');var t=savingtransactiondata;$.each(t,function(t,a){var e=$('#SavingTransactionTable tbody'),n=$('<tr>');n.append($('<td>',{text:a.TransactionDate})),n.append($('<td>',{text:a.TransactionType})),n.append($('<td>',{text:a.Narration})),n.append($('<td>',{text:parseFloat(a.FCY).toFixed(2),class:'text-right'})),n.append($('<td>',{text:parseFloat(a.CurrentRate).toFixed(2),class:'text-right'})),n.append($('<td>',{text:parseFloat(a.LCY).toFixed(2),class:'text-right'})),n.append($('<td>',{html:'<div class=\"action-btns btn-tbl-action\"><button type=\"button\" title=\"View\"><span class=\"fa fa-paper-plane-o\"></span></button></div>'})),e.append(n)})}),$('#PrintGrid').on('click',function(){var t=document.getElementById('SavingTransactionTable');newWin=window.open(''),newWin.document.write(t.outerHTML),newWin.print(),newWin.close()})}},100)});</script>";

        public const string CURRENT_TRANSACTION_DETAIL_GRID_WIDGET_SCRIPT = "<script type='text/javascript'>$(document).ready(function(){setTimeout(function(){if(null!=currenttransactiondata&&currenttransactiondata.length>0){var t=currenttransactiondata;$('input[type=\"radio\"][name=\"currenttransactionRadio\"]').change(function(a){if('currentShowAll'==a.currentTarget.id)$('#CurrentTransactionTable tbody tr').remove(),$.each(t,function(t,a){var e=$('#CurrentTransactionTable tbody'),n=$('<tr>');n.append($('<td>',{text:a.TransactionDate})),n.append($('<td>',{text:a.TransactionType})),n.append($('<td>',{text:a.Narration})),n.append($('<td>',{text:parseFloat(a.FCY).toFixed(2),class:'text-right'})),n.append($('<td>',{text:parseFloat(a.CurrentRate).toFixed(2),class:'text-right'})),n.append($('<td>',{text:parseFloat(a.LCY).toFixed(2),class:'text-right'})),n.append($('<td>',{html:'<div class=\"action-btns btn-tbl-action\"><button type=\"button\" title=\"View\"><span class=\"fa fa-paper-plane-o\"></span></button></div>'})),e.append(n)});else{$('#CurrentTransactionTable tbody tr').remove();var e=t.reduce(function(t,a){const e=a.TransactionDate;return t[e]=t[e]||[],t[e].push(a),t},{});$.each(e,function(t,a){var e=0,n=0,r=0;a.length>1?(e=a.reduce(function(t,a){return t+parseFloat(a.FCY)},0),n=a.reduce(function(t,a){return t+parseFloat(a.LCY)},0),r=a.reduce(function(t,a){return t+parseFloat(a.CurrentRate)},0)):(e=a[0].FCY,n=a[0].LCY,r=a[0].CurrentRate);var o=$('#CurrentTransactionTable tbody'),p=$('<tr>');p.append($('<td>',{text:a[0].TransactionDate})),p.append($('<td>',{text:a[0].TransactionType})),p.append($('<td>',{text:'-'})),p.append($('<td>',{text:parseFloat(e).toFixed(2),class:'text-right'})),p.append($('<td>',{text:parseFloat(r).toFixed(2),class:'text-right'})),p.append($('<td>',{text:parseFloat(n).toFixed(2),class:'text-right'})),p.append($('<td>',{text:'-'})),o.append(p)})}}),$('input[type=\"radio\"][id=\"currentShowAll\"]').prop('checked',!0).trigger('change'),$('#filterStatus').on('change',function(){val=this.value;var t=[];t='0'==val?currenttransactiondata:jQuery.grep(currenttransactiondata,function(t){return t.Narration==val}),$('#CurrentTransactionTable tbody tr').remove(),$.each(t,function(t,a){var e=$('#CurrentTransactionTable tbody'),n=$('<tr>');n.append($('<td>',{text:a.TransactionDate})),n.append($('<td>',{text:a.TransactionType})),n.append($('<td>',{text:a.Narration})),n.append($('<td>',{text:parseFloat(a.FCY).toFixed(2),class:'text-right'})),n.append($('<td>',{text:parseFloat(a.CurrentRate).toFixed(2),class:'text-right'})),n.append($('<td>',{text:parseFloat(a.LCY).toFixed(2),class:'text-right'})),n.append($('<td>',{html:'<div class=\"action-btns btn-tbl-action\"><button type=\"button\" title=\"View\"><span class=\"fa fa-paper-plane-o\"></span></button></div>'})),e.append(n)})}),$('#ResetGrid').on('click',function(){$('#filterStatus').prop('selectedIndex','0');var t=currenttransactiondata;$.each(t,function(t,a){var e=$('#CurrentTransactionTable tbody'),n=$('<tr>');n.append($('<td>',{text:a.TransactionDate})),n.append($('<td>',{text:a.TransactionType})),n.append($('<td>',{text:a.Narration})),n.append($('<td>',{text:parseFloat(a.FCY).toFixed(2),class:'text-right'})),n.append($('<td>',{text:parseFloat(a.CurrentRate).toFixed(2),class:'text-right'})),n.append($('<td>',{text:parseFloat(a.LCY).toFixed(2),class:'text-right'})),n.append($('<td>',{html:'<div class=\"action-btns btn-tbl-action\"><button type=\"button\" title=\"View\"><span class=\"fa fa-paper-plane-o\"></span></button></div>'})),e.append(n)})}),$('#PrintGrid').on('click',function(){var t=document.getElementById('CurrentTransactionTable');newWin=window.open(''),newWin.document.write(t.outerHTML),newWin.print(),newWin.close()})}},100)});</script>";

        public const string PIE_CHART_WIDGET_SCRIPT = "<script type='text/javascript'>$(document).ready(function(){ setTimeout(function(){if(null!=$('#hiddenPieChartData').val()&&''!=$('#hiddenPieChartData').val()){let e=JSON.parse($('#hiddenPieChartData').val()),t=e.series,a=e.color.split(',');Highcharts.chart('pieChartcontainer',{chart:{plotBackgroundColor:null,plotBorderWidth:null,plotShadow:!1,type:'pie'},title:{text:''},tooltip:{pointFormat:'{series.name}: <b>{point.percentage:.1f}%</b>'},accessibility:{point:{valueSuffix:'%'}},plotOptions:{pie:{allowPointSelect:!0,cursor:'pointer',dataLabels:{enabled:!0,format:'{point.percentage:.1f} %'},showInLegend:!1}},series:t,colors:a})}else $('#pieChartcontainer').html('<div class=\"text-danger text-center\">No data available.</div>')},100) });</script>";

        public const string BAR_GRAPH_WIDGET_SCRIPT = "<script type='text/javascript'>$(document).ready(function(){ setTimeout(function(){if(null!=$('#hiddenBarGraphData').val()&&''!=$('#hiddenBarGraphData').val()){let a=JSON.parse($('#hiddenBarGraphData').val()),t=a.series,e=a.color.split(','),r=a.xAxis;Highcharts.chart('barchartcontainer',{title:{text:''},xAxis:{categories:r},series:t,colors:e})}else $('#barchartcontainer').html('<div class=\"text-danger text-center\">No data available.</div>')},100); });</script>";

        public const string LINE_GRAPH_WIDGET_SCRIPT = "<script type='text/javascript'>$(document).ready(function(){ setTimeout(function (){if(null!=$('#hiddenLineGraphData').val()&&''!= $('#hiddenLineGraphData').val()){let a=JSON.parse($('#hiddenLineGraphData').val()),t=a.series,e=a.color.split(','),r=a.xAxis;Highcharts.chart('linechartcontainer',{title:{text:''}, xAxis:{categories:r},series:t,colors:e})}else $('#linechartcontainer').html('<div class=\"text-danger text-center\">No data available.</div>')},100); });</script>";

        public const string TENANT_LOGO_SCRIPT = "<script type='text/javascript'> $(document).ready(function () {setTimeout(function () {if(null!=$('#TenantLogoImageValue').val()&&''!= $('#TenantLogoImageValue').val()) {let e=new Image; e.src= $('#TenantLogoImageValue').val(),e.height=40,$('#TenantLogo').append(e)} else {let e = tenant.TenantName.charAt(0).toUpperCase(),t=document.createElement('div'); t.classList.add('ltr-img'), t.style.height='50px',t.style.width='50px',t.style.fontSize='30px';let n=document.createElement('span'); n.textContent=e,t.appendChild(n),$('#TenantLogo').append(t)}},100)}); </script>";

        public const string STYLE = "color:{{COLOR}};font-size:{{SIZE}}px;font-weight:{{WEIGHT}};font-family:'{{TYPE}}';";

        public const string HTML_FOOTER = " {{ChartScripts}} </body></html>";

        public const string TABLEWIDEGTPREVIEW = "<div class='card border-0'><div class='p-1 bg-light border-0 text-left'><h5 class='m-0;' style={{TitleStyle}}>{{WidgetTitle}}</h5></div><div class='card-body'><div class='table-responsive stylescrollbar' style='max-height:350px;overflow-x:hidden;overflow-y:auto;'><table id = 'TableWidget' class='table m-1 table-hover'><thead style={{HeaderStyle}}>{{tableHeader}}</thead><tbody style={{BodyStyle}}>{{tableBody}}</tbody></table></div></div></div>";

        public const string FORMWIDGETPREVIEW = "<div class='card border-0'><div class='p-1 bg-light border-0 text-left' style='margin-bottom: 10px;'><h5 class='m-0' style={{TitleStyle}}>{{WidgetTitle}}</h5></div><div class='card border-0 card-shadow m-auto width100'><div class='card-body p-2' style={{BodyStyle}}>{{FormData}}</div></div></div>";

        public const string HTMLWIDGETPREVIEW = "<div class='card border-0'><div class='p-1 bg-light border-0 text-left'><h5 class='m-0' style={{TitleStyle}}>{{WidgetTitle}}</h5></div><div class='card-body' style={{BodyStyle}}>{{FormData}}</div></div>";

        public const string LINEGRAPH_WIDGETPREVIEW = "<div class='card border-0' style='height:600px'><div class='p-1 bg-light border-0 text-left'> " +
            "<h5 class='m-0' style={{TitleStyle}}>{{WidgetTitle}}</h5></div><div class='card-body'> <div id=\"lineGraphcontainer\" style='height: 75%; width: 90%; position: absolute;'></div></div></div> ";

        public const string PIECHART_WIDGETPREVIEW = "<div class='card border-0' style='height:600px'><div class='p-1 bg-light border-0 text-left'> <h5 class='m-0' style={{TitleStyle}}>{{WidgetTitle}}</h5></div><div class='card-body'> <div id=\"pieChartcontainer\" style='height: 75%; width: 90%; position: absolute;'></div></div></div> ";

        public const string BARGRAPH_WIDGETPREVIEW = "<div class='card border-0' style='height:600px'><div class='p-1 bg-light border-0 text-left'> <h5 class='m-0' style={{TitleStyle}}>{{WidgetTitle}}</h5></div><div class='card-body'> <div id=\"barGraphcontainer\" style='height: 75%; width: 90%; position: absolute;'></div></div></div> ";

        public const string TABLE_WIDEGT_FOR_PAGE_PREVIEW = "<div class='card border-0' style='height:{{WidgetDivHeight}}'><div class='p-1 bg-light border-0 text-left'><h5 class='m-0;' style={{TitleStyle}}>{{WidgetTitle}}</h5></div><div class=''><div class='table-responsive stylescrollbar' style='max-height:{{TableMaxHeight}};overflow-x:hidden;overflow-y:auto;'> <table id = 'TableWidget' class='table m-1 table-hover'><thead style={{HeaderStyle}}>{{tableHeader}}</thead><tbody style={{BodyStyle}}>{{tableBody}}</tbody></table></div></div></div>";

        public const string FORM_WIDGET_FOR_PAGE_PREVIEW = "<div class='card border-0' style='height:{{WidgetDivHeight}}'><div class='p-1 bg-light border-0 text-left' style='margin-bottom: 10px;'><h5 class='m-0' style={{TitleStyle}}>{{WidgetTitle}}</h5></div><div class='card border-0 card-shadow width100'><div class='card-body p-2' style={{BodyStyle}}>{{FormData}}</div></div></div>";

        public const string HTML_WIDGET_FOR_PAGE_PREVIEW = "<div class='card border-0' style='height:{{WidgetDivHeight}}'><div class='p-1 bg-light border-0 text-left'><h5 class='m-0' style={{TitleStyle}}>{{WidgetTitle}}</h5></div><div class='card-body text-left' style={{BodyStyle}}>{{FormData}}</div></div>";

        public const string LINE_GRAPH_FOR_PAGE_PREVIEW = "<div class='card border-0' style='height:{{WidgetDivHeight}}'><div class='p-1 bg-light border-0 text-left'><h5 class='m-0' style={{TitleStyle}}>{{WidgetTitle}}</h5></div><div class='card-body'> <div id=\"lineGraphcontainer\" style='height: 75%; width: 90%; position: absolute;'></div></div></div> ";

        public const string PIE_CHART_FOR_PAGE_PREVIEW = "<div class='card border-0' style='height:{{WidgetDivHeight}}'><div class='p-1 bg-light border-0 text-left'><h5 class='m-0' style={{TitleStyle}}>{{WidgetTitle}}</h5></div><div class='card-body'> <div id=\"pieChartcontainer\" style='height: 75%; width: 90%; position: absolute;'></div></div></div> ";

        public const string BAR_GRAPH_FOR_PAGE_PREVIEW = "<div class='card border-0' style='height:{{WidgetDivHeight}}'><div class='p-1 bg-light border-0 text-left'> <h5 class='m-0' style={{TitleStyle}}>{{WidgetTitle}}</h5></div><div class='card-body'> <div id=\"barGraphcontainer\" style='height: 75%; width: 90%; position: absolute;'></div></div></div> ";

        public const string THEME1 = "#342ead,#EA6227,#f2a51a,#b9ebcc";

        public const string THEME2 = "#45046a,#5c2a9d,#b5076b,#f1ebbb";

        public const string THEME3 = "#5fdde5,#f4ea8e,#f37121,#d92027";

        public const string THEME4 = "#805D93,#F49FBC,#9EBD6E,#169873";
    }
}
