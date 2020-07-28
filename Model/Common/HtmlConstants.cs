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

        public const int HOME_PAGE_TYPE_ID = 1;

        public const int SAVING_ACCOUNT_PAGE_TYPE_ID = 2;

        public const int CURRENT_ACCOUNT_PAGE_TYPE_ID = 3;

        public const string HTML_HEADER = "<html><head><title>NIS Output</title><meta charset='utf-8'><meta name='viewport' content='width=device-width, initial-scale=1'><link rel='stylesheet' href='https://maxcdn.bootstrapcdn.com/bootstrap/4.5.0/css/bootstrap.min.css'><link rel='stylesheet' href='https://cdnjs.cloudflare.com/ajax/libs/font-awesome/4.7.0/css/font-awesome.min.css'><script src='https://ajax.googleapis.com/ajax/libs/jquery/3.5.1/jquery.min.js'></script><script src='https://cdnjs.cloudflare.com/ajax/libs/popper.js/1.16.0/umd/popper.min.js'></script><script src='https://maxcdn.bootstrapcdn.com/bootstrap/4.5.0/js/bootstrap.min.js'></script><script src='https://code.highcharts.com/highcharts.js'></script><script src='https://code.highcharts.com/modules/series-label.js'></script><script src='https://code.highcharts.com/modules/exporting.js'></script><script src='https://code.highcharts.com/modules/export-data.js'></script><script src='https://code.highcharts.com/modules/accessibility.js'></script><script src='script.js'></script><link rel='stylesheet' href='site.css'></head><body>";

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
            "{{AccountSummary}}" +
            "</tbody></table></div></div></div>";

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
            "< div class='card-header bg-light border-0 text-left'><h5 class='m-0'>Available Balance</h5> </div>" +
            "<div class='card-body'><div><h4 style = 'text-align:right' class='mb-4'>" +
            "<i style = 'color: limegreen' class='fa fa-sort-asc'aria-hidden='true'></i>{{TotalValue}}</h4>" +
            "<span style = 'float:left;' > Total Deposits</span><span style = 'float:right;' >{{TotalDeposit}}</span><br />" +
            "<span style = 'float:left;' > Total Spend</span><span style = 'float:right;' >{{TotalSpend}}</span><br />" +
            "<span style = 'float:left;' > Savings </ span >< span style='float:right;'>{{Savings}}</span><br /></div></div></div>";

        public const string SAVING_TRANSACTION_WIDGET_HTML = "<div class='card border-0'>< div class='card-header bg-light border-0 text-left'><h5 class='m-0'>Transaction Details</h5></div>" +
            "< div style = 'float:left;' >< input type= 'radio' id= 'showAll' name= 'showAll' value= 'showAll' >" +
            "< label for='showAll'>Show All</label><input type = 'radio' id= 'grpDate' name= 'grpDate' value= 'grpDate' >< label for='grpDate'>Group By Date</label></div>" +
            " < div style = 'float:right;' >< button class='' type='button'>Seach</button><button type = 'button' > Reset </ button >< button type='button'>Print</button></div>" +
            "< div class='card-body'><div class='table-responsive'><table class='table m-0 table-hover'>" +
            "<thead><tr><th>Date</th><th>Type</th><th>Narration</th><th>Credit</th><th>Debit</th><th>Query</th><th>Balance</th></tr>" +
            "</thead><tbody>{{AccountTransactionDetails}}</tbody></table></div></div></div>";

        public const string CURRENT_TRANSACTION_WIDGET_HTML = "<div class='card border-0'><div class='card-header bg-light border-0 text-left'><h5 class='m-0'>Transaction Details</h5></div>" +
            "<div style='float:left;'><input type='radio' id='showAll' name='showAll' value='showAll'>" +
            "<label for='showAll'>Show All</label><input type='radio' id='grpDate' name='grpDate' value='grpDate'><labelfor='grpDate'>Group By Date</label></div>" +
            "<div style='float:right;'><button class='' type='button'>Seach</button><button type='button'>Reset</button><button type='button'>Print</button></div>" +
            "<div class='card-body'><div class='table-responsive'><table class='table m-0 table-hover'><thead><tr>" +
            "<th>Date</th><th>Type</th><th>Narration</th><th>FYC(GBP)</th><th>Current Rate</th><th>LCY(GBP)</th></tr>" +
            "</thead><tbody>{{CurrentAccountTransactionDetails}}</tbody></table></div></div></div>";

        public const string CONTAINER_DIV_HTML_HEADER = "<div class='container-fluid mt-3 mb-3 bdy-scroll stylescrollbar'>";

        public const string WIDGET_HTML_HEADER = "<div id='{{DivId}}' class='card border-0 p-2 tabDivClass {{ExtraClass}}'>";

        public const string WIDGET_HTML_FOOTER = "</div>";

        public const string CONTAINER_DIV_HTML_FOOTER = "</div>";

        public const string HOME_PAGE_DIV_NAME = "Home-Div";

        public const string SAVING_ACCOUNT_PAGE_DIV_NAME = "SavingAcc-Div";

        public const string CURRENT_ACCOUNT_PAGE_DIV_NAME = "CurrentAcc-Div";

        public const string TAB_NAVIGATION_SCRIPT = "<script type='text/javascript'>$(document).ready(function(){$('.nav-link').click(function(t){$('.tabDivClass').hide(),$('.nav-link').removeClass('active');let a='active '+$(t.currentTarget).attr('class');$(t.currentTarget).attr('class',a);let e=$(t.currentTarget).attr('class').split(' '),n=e[e.length-1];$('.'+n).hasClass('d-none')&&$('.'+n).removeClass('d-none'),$('.'+n).show()})});</script>";

        public const string HTML_FOOTER = "</body></html>";
    }
}
