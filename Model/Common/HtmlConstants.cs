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

        public const string SCRIPT_TAG = "<script>function onTabChangeClicked(e){var t=document.getElementsByClassName('tabDivClass');for(x=0;x<t.length;x++)t[x].style.display='none';'Home'==e?document.getElementById('Home-Div').style.display='block':'Current'==e?document.getElementById('CurrentAcc-Div').style.display='block':'Saving'==e&&(document.getElementById('SavingAcc-Div').style.display='block')}  </script>";

        public const string NAVBAR_HTML = "<nav class='navbar navbar-expand-sm bg-white navbar-light'>" +
        "<a class='navbar-brand ml-auto' href='javascript:void(0);'> <img src = '{{BrandLogo}}' height='60'></a> </nav> " +
        "<nav class='navbar navbar-expand-sm bg-dark navbar-dark'><div class='collapse navbar-collapse' id='collapsibleNavbar'>" +
        "<ul class='navbar-nav nav'>" +
            "{{NavItemList}}"+
            //"<li class='nav-item'><a class='nav-link active Home-Div' href='javascript:void(0);'>At a Glance</a> </li>" +
            //"<li class='nav-item'> <a class='nav-link SavingAcc-Div' href='javascript:void(0);''>Saving Account</a></li> " +
            //"<li class='nav-item'><a class='nav-link CurrentAcc-Div' href='javascript:void(0);'>Current Account</a></li>" +
            //"<li class='nav-item'><a class='nav-link' href='javascript:void(0);'>Home Loan</a></li>" +
            //"<li class='nav-item'><a class='nav-link' href='javascript:void(0);'>Contact Us</a></li>" +
            "</ul>" + "<ul class='navbar-nav ml-auto'>" + 
             "<li class='nav-item date'><a class='text-white'>{{Today}}</a></li>" + "</ul></div></nav>";

        public const string CUSTOMER_INFORMATION_WIDGET_HTML = "<div class='card border-0'>" +
                        "<div class='card-header bg-light border-0 text-left'><h5 class='m-0'>Customer Information</h5></div>" +
                        "<div class='card-body'><div class='row'><div class='col-sm-4'><h4 class='mb-4'>{{CustomerName}}</h4>" +
                        "<h6>{{Address1}}<br /> {{Address2}}</h6></div>" +
                        "<div class='col-sm-8'> <video class='doc-video' controls><source src='{{VideoSource}}' type='video/mp4'></video>" +
                        "</div></div></div></div>";

        public const string ACCOUNT_INFORMATION_WIDGET_HTML = "<div class='card border-0'>" +
                       "<div class='card-header bg-light border-0 text-left'><h5 class='m-0'>Account Information</h5>" +
                       "</div><div class='card-body'>" +

                       "<div class='list-row-small ht70px'><div class='list-middle-row'>" +
                       "<div class='list-text'>Statement Date</div><label class='list-value mb-0'>1-APR-2020</label></div></div>" +

                       "<div class='list-row-small ht70px'><div class='list-middle-row'>" +
                       "<div class='list-text'>Statement Period</div><label class='list-value mb-0'>Annual Statement</label></div></div>" +

                        "<div class='list-row-small ht70px'><div class='list-middle-row'>" +
                        "<div class='list-text'>Customer ID</div><label class='list-value mb-0'>ID2-8989-5656</label></div></div>" +

                        "<div class='list-row-small ht70px'><div class='list-middle-row'>" +
                        "<div class='list-text'>RM Name</div><label class='list-value mb-0'>Laura J Donald</label></div></div>" +

                        "<div class='list-row-small ht70px'><div class='list-middle-row'>" +
                        "<div class='list-text'>RM Contact Number</div><label class='list-value mb-0'>+4487867833</label></div></div>" +

                        "</div></div>";

        public const string SUMMARY_AT_GLANCE_WIDGET_HTML = "<div class='card border-0'>" +
                        "<div class='card-header bg-light border-0 text-left'> <h5 class='m-0'>Summary at Glance</h5></div>" +
                        "<div class='card-body'><div class='table-responsive'> <table class='table m-0 table-hover'>" +
                        "<thead><tr><th>Account</th><th>Currency</th><th>Amount</th></tr></thead><tbody>" +
                        "<tr><td>Saving Account</td><td>Dollor</td><td>87356</td></tr>" +
                        "<tr><td>Current Account</td><td>Dollor</td><td>18356</td></tr>" +
                        "<tr><td>Recurring Account</td><td>Dollor</td><td>543678</td></tr>" +
                        "<tr><td>Wealth</td><td>Dollor</td><td>4567</td></tr>" +
                        "</tbody></table></div></div></div>";

        public const string IMAGE_WIDGET_HTML = "<div class='card border-0'>" +
                        "<div class='card-header bg-light border-0 text-left'><h5 class='m-0'>Image Information</h5></div>" +
                        "<div class='card-body text-center'><img src='{{ImageSource}}' class='img-fluid'/></div></div>";

        public const string VIDEO_WIDGET_HTML = "<div class='card border-0'>" +
                        "<div class='card-header bg-light border-0 text-left'><h5 class='m-0'>Video Information</h5> </div><div class='card-body text-center'>" +
                        "<video class='doc-video' controls><source src='{{VideoSource}}' type='video/mp4'></video></div></div>";

        public const string NO_WIDGET_MESSAGE_HTML = "<div class='card border-0'>" +
                        "<div class='card-header bg-light border-0'><h5 class='m-0'>No Configuration</h5> </div><div class='card-body text-center text-danger'>" +
                        "<span>No configuration saved for this record.</span></div></div>";

        public const string CONTAINER_DIV_HTML_HEADER = "<div class='container-fluid mt-3 mb-3'>";

        public const string WIDGET_HTML_HEADER = "<div id='{{DivId}}' class='card border-0 p-2 tabDivClass {{ExtraClass}}'>";

        public const string WIDGET_HTML_FOOTER = "</div>";

        public const string CONTAINER_DIV_HTML_FOOTER = "</div>";

        public const string HOME_PAGE_DIV_NAME = "Home-Div";

        public const string SAVING_ACCOUNT_PAGE_DIV_NAME = "SavingAcc-Div";

        public const string CURRENT_ACCOUNT_PAGE_DIV_NAME = "CurrentAcc-Div";
    }
}
