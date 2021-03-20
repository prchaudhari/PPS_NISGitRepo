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

        public const string CUSTOMER_DETAILS_WIDGET_NAME = "CustomerDetails";

        public const string BANK_DETAILS_WIDGET_NAME = "BankDetails";

        public const string INVESTMENT_PORTFOLIO_STATEMENT_WIDGET_NAME = "InvestmentPortfolioStatement";

        public const string INVESTOR_PERFORMANCE_WIDGET_NAME = "InvestorPerformance";

        public const string BREAKDOWN_OF_INVESTMENT_ACCOUNTS_WIDGET_NAME = "BreakdownOfInvestmentAccounts";

        public const string EXPLANATORY_NOTES_WIDGET_NAME = "ExplanatoryNotes";

        public const string SERVICE_WIDGET_NAME = "NedbankService";

        public const string PERSONAL_LOAN_DETAIL_WIDGET_NAME = "PersonalLoanDetail";

        public const string PERSONAL_LOAN_TRANASCTION_WIDGET_NAME = "PersonalLoanTransaction";

        public const string PERSONAL_LOAN_PAYMENT_DUE_WIDGET_NAME = "PersonalLoanPaymentDue";

        public const string SPECIAL_MESSAGE_WIDGET_NAME = "SpecialMessage";

        public const string PERSONAL_LOAN_INSURANCE_MESSAGE_WIDGET_NAME = "PL_InsuranceMessage";

        public const string HTML_HEADER = "<html><head><title>NIS Statement</title><meta charset='utf-8'><meta name='viewport' content='width=device-width, initial-scale=1'><link rel='stylesheet' href='../common/css/bootstrap.min.css'><link rel='stylesheet' href='../common/css/font-awesome.min.css'><script src='../common/js/jquery.min.js'></script><script src='../common/js/popper.min.js'></script><script src='../common/js/bootstrap.min.js'></script><script src='../common/js/highcharts.js'></script><script src='../common/js/series-label.js'></script><script src='../common/js/exporting.js'></script><script src='../common/js/export-data.js'></script><script src='../common/js/accessibility.js'></script><script src='../common/js/script.js'></script><link rel='stylesheet' href='../common/css/site.css'><link rel='stylesheet' href='../common/css/ltr.css'></head><body onload='onPageLoad()'> <input type='hidden' id='StatementId' name='StatementId' value='{{StatementNumber}}'> <input type='hidden' id='CustomerId' name='CustomerId' value='{{CustomerNumber}}'><input type='hidden' id='FirstPageId' name='FirstPageId' value='{{FirstPageId}}'><input type='hidden' id='TenantCode' name='TenantCode' value='{{TenantCode}}'> <input type='hidden' id='TenantName' name='TenantName' value='{{TenantName}}'>";

        public const string NAVBAR_HTML_FOR_PREVIEW = "<nav class='navbar navbar-expand-sm bg-white navbar-light p-0'><a href='javascript:void(0);' class='navbar-brand ml-3'> <img src='{{logo}}' height='50'></a><a class='navbar-brand ml-auto' href='javascript:void(0);' id='TenantLogo'> </a> </nav><nav class='navbar navbar-expand-sm bg-dark navbar-dark'><div class='collapse navbar-collapse' id='collapsibleNavbar'><ul class='navbar-nav nav'>{{NavItemList}}</ul><ul class='navbar-nav ml-auto'><li class='nav-item date'><a class='text-white'>{{Today}}</a></li></ul></div></nav>";

        public const string NAVBAR_HTML = "<nav class='navbar navbar-expand-sm bg-white navbar-light p-0'><a href='javascript:void(0);' class='navbar-brand ml-3'> <img src='{{logo}}' height='50'></a><a class='navbar-brand ml-auto' href='javascript:void(0);'> <img id='TenantLogo' src='{{BrandLogo}}' height='50'></a> </nav> <nav class='navbar navbar-expand-sm bg-dark navbar-dark'><div class='collapse navbar-collapse' id='collapsibleNavbar'><ul class='navbar-nav nav'>{{NavItemList}}</ul><ul class='navbar-nav ml-auto'><li class='nav-item date'><a class='text-white'>{{Today}}</a></li></ul></div></nav>";

        public const string NEDBANK_STATEMENT_HEADER = "<div class='row'><div class='col-lg-6 col-sm-6' style='height:100px;'><div class='card border-0'><div class='card-body float-left py-2'><div class='eConfirm-logo-container'><img src='{{eConfirmLogo}}' alt='eConfirm Logo' height='80'><div class='eConfirm-logo-date'>{{StatementDate}}</div></div></div></div></div><div class='col-lg-6 col-sm-6' style='height:100px;'><div class='card border-0'><div class='card-body py-2'><a class='navbar-brand float-right mr-0' href='javascript:void(0);'> <img src='{{NedBankLogo}}' height='80'></a></div></div></div></div>";

        public const string NEDBANK_STATEMENT_FOOTER = "<div class='ftr'><div class='row'><div class='col-lg-4 col-sm-4'><img class='ftr-lt-span1' src='{{NedbankSloganImage}}' alt='Nedbank slogan' width='180'></div><div class='col-lg-8 col-sm-8'><img class='ftr-rt-span1' src='{{NedbankNameImage}}' alt='Nedbank Name' width='100'></div></div><div class='row'><div class='col-lg-12 col-sm-12'><span class='ftr-rt-span2'>{{FooterText}}</span></div></div></div>";

        public const string CUSTOMER_INFORMATION_WIDGET_HTML = "<div class='card border-0' style='height:{{WidgetDivHeight}}'><div class='p-1 bg-light border-0 text-left'><h5 class='m-0'>Customer Information</h5></div><div class='card-body'><div class='row'><div class='col-sm-4'><h4 class='mb-4'>{{CustomerName}}</h4><h6>{{Address1}}{{Address2}}</h6></div><div class='col-sm-8'> <video class='doc-video' controls><source src='{{VideoSource}}' type='video/mp4'></video></div></div></div></div>";

        public const string ACCOUNT_INFORMATION_WIDGET_HTML = "<div class='card border-0' style='height:{{WidgetDivHeight}}'><div class='p-1 bg-light border-0 text-left'><h5 class='m-0'>Account Information</h5></div> <div class='card-body overflow-auto'>{{AccountInfoData}}</div></div>";

        public const string SUMMARY_AT_GLANCE_WIDGET_HTML = "<div class='card border-0' style='height:{{WidgetDivHeight}}'><div class='p-1 bg-light border-0 text-left'> <h5 class='m-0'>Summary at Glance</h5></div> <div class='card-body overflow-auto'><div class='table-responsive'> <table class='table m-0 table-hover'><thead><tr><th>Account</th><th>Currency</th><th>Amount</th></tr></thead><tbody>{{AccountSummary}}</tbody></table></div></div></div>";

        public const string IMAGE_WIDGET_HTML = "<div class='card border-0' style='height:{{WidgetDivHeight}}'><div class='card-body text-center'><img src='{{ImageSource}}' class='img-fluid {{NewImageClass}}'/></div></div>";

        public const string VIDEO_WIDGET_HTML = "<div class='card border-0' style='height:{{WidgetDivHeight}}'> <div class='card-body text-center'><video class='video-widget {{NewVideoClass}}' controls><source src='{{VideoSource}}' type='video/mp4'></video></div></div>";

        public const string NO_WIDGET_MESSAGE_HTML = "<div class='card border-0'><div class='p-1 bg-light border-0'><h5 class='m-0'>No Configuration</h5> </div><div class='card-body text-center text-danger'><span>No configuration saved for this record.</span></div></div>";

        public const string SAVING_CURRENT_AVALABLE_BAL_WIDGET_HTML = "<div class='card border-0' style='height:{{WidgetDivHeight}}'><div class='p-1 bg-light border-0 text-left'><h5 class='m-0'>Available Balance</h5> </div><div class='card-body overflow-auto'><div class='fnt14'><h4 class='mb-4 text-right'><i class='{{AccountIndicatorClass}}' aria-hidden='true'></i>&nbsp;{{TotalValue}}</h4> <span class='float-left'> Total Deposits</span><span class='float-right'>{{TotalDeposit}}</span><br/><span class='float-left'> Total Spend</span><span class='float-right'>{{TotalSpend}}</span><br/><span class='float-left'> Savings </span><span class='float-right'>{{Savings}}</span><br/></div></div></div>";

        public const string SAVING_TRANSACTION_WIDGET_HTML = "<div class='card border-0' style='height:{{WidgetDivHeight}}'><div class='p-1 bg-light border-0 text-left'> <h5 class='m-0'>Transaction Details</h5></div> <div class='card-body overflow-auto'><div class='float-left'> <input type='radio' id='savingShowAll' checked name='savingtransactionRadio'>&nbsp;<label for='showAll'>Show All</label>&nbsp; <input type='radio' id='savingGrpDate' name='savingtransactionRadio'>&nbsp;<label for='grpDate'>Group By Date</label></div> <div class='float-right'><div class='float-left mr-2'><select class='form-control float-left' id='filterStatus'><option value = '0'> Search Item</option>{{SelectOption}} </select></div> <a href='javascript:void(0)' class='btn btn-light btn-sm' id='ResetGrid'>Reset</a>&nbsp; <a href='javascript:void(0)' class='btn btn-light btn-sm' id='PrintGrid'>Print</a> </div> <div class='table-responsive stylescrollbar' style='max-height:350px;overflow-x:hidden;overflow-y:auto;'><table id='SavingTransactionTable' class='table m-1 table-hover'><thead><tr> <th class='width12'>Date</th><th class='width8'>Type</th class='width30'><th>Narration</th><th class='width12 text-right'>FCY</th><th class='width13 text-right'>Current Rate</th> <th class='width13 text-right'>LCY</th><th class='width12'>Action</th></tr></thead><tbody>{{AccountTransactionDetails}}</tbody></table></div></div></div>";

        public const string CURRENT_TRANSACTION_WIDGET_HTML = "<div class='card border-0' style='height:{{WidgetDivHeight}}'><div class='p-1 bg-light border-0 text-left'> <h5 class='m-0'>Transaction Details</h5></div> <div class='card-body'> <div class='float-left'> <input type='radio' id='currentShowAll' checked name='currenttransactionRadio'>&nbsp;<label for='showAll'>Show All</label>&nbsp; <input type='radio' id='currentGrpDate' name='currenttransactionRadio'>&nbsp;<label for='grpDate'>Group By Date</label></div> <div class='float-right'><div class='float-left mr-2'><select class='form-control float-left' id='filterStatus'><option value = '0'> Search Item</option>{{SelectOption}} </select></div> <a href='javascript:void(0)' class='btn btn-light btn-sm' id='ResetGrid'>Reset</a>&nbsp; <a href='javascript:void(0)' class='btn btn-light btn-sm' id='PrintGrid'>Print</a> </div> <div class='table-responsive stylescrollbar'  style='max-height:350px;overflow-x:hidden;overflow-y:auto;'><table id='CurrentTransactionTable' class='table m-1 table-hover'><thead><tr> <th class='width12'>Date</th><th class='width8'>Type</th class='width30'><th>Narration</th><th class='width12 text-right'>FCY</th><th class='width13 text-right'>Current Rate</th> <th class='width13 text-right'>LCY</th><th class='width12'>Action</th></tr></thead><tbody>{{AccountTransactionDetails}}</tbody></table></div></div></div>";

        public const string REMINDER_WIDGET_HTML = "<div class='card border-0' style='height:{{WidgetDivHeight}}'><div class='p-1 bg-light border-0 text-left'><h5 class='m-0'>Reminder and Recommendation</h5></div><div class='card-body overflow-auto' style='font-size:12px;'> {{ReminderAndRecommdationDataList}} </div></div>";

        public const string TOP_4_INCOME_SOURCE_WIDGET_HTML = "<div class='card border-0' style='height:{{WidgetDivHeight}}'><div class='card-header bg-light border-0 text-left'> <h5 class='m-0'>Top 4 Income Sources</h5></div><div class='card-body'><table class='table-borderless width100'><thead class='border-bottom'><tr><td class='width50'></td><td class='width20'>This Month</td><td class='width30'>Usually you spend</td></tr></thead><tbody>{{IncomeSourceList}}</tbody></table></div></div>";

        public const string ANALYTIC_WIDGET_HTML = "<div class='card border-0' style='height:{{WidgetDivHeight}}'><div class='p-1 bg-light border-0 text-left'> <h5 class='m-0'>Analytics</h5></div><div class='card-body'> <div id=\"analyticschartcontainer\" style='height: 75%; width: 90%; position: absolute;'></div></div></div> ";

        public const string SAVING_TRENDS_WIDGET_HTML = "<div class='card border-0' style='height:{{WidgetDivHeight}}'><div class='p-1 bg-light border-0 text-left'> <h5 class='m-0'>Saving Trends</h5></div><div class='card-body'> <div id=\"savingTrendscontainer\" style='height: 75%; width: 90%; position: absolute;'></div></div></div> ";

        public const string SPENDING_TRENDS_WIDGET_HTML = "<div class='card border-0' style='height:{{WidgetDivHeight}}'><div class='p-1 bg-light border-0 text-left'> <h5 class='m-0'>Spending Trends</h5></div><div class='card-body'> <div id=\"spendingTrendscontainer\" style='height: 75%; width: 90%; position: absolute;'></div></div></div> ";

        public const string CUSTOMER_DETAILS_WIDGET_HTML = "<div id={{WidgetId}} class='card border-0'><div class='card-body CustomerDetails py-1'>{{Title}} {{FirstName}} {{Surname}}<br>{{CustAddressLine0}}<br>{{CustAddressLine1}}<br>{{CustAddressLine2}}<br>{{CustAddressLine3}}<br>{{CustAddressLine4}}<br></div><div class='CustomerCellNoDiv'>{{MaskCellNo}}</div></div>";

        public const string BANK_DETAILS_WIDGET_HTML = "<div id={{WidgetId}} class='card border-0'><div class='card-body BankDetails py-1'>{{BankName}}<br>{{AddressLine1}}<br>{{AddressLine2}}<br>{{CountryName}}<br>{{BankVATRegNo}}</div><div class='ConactCenterDiv text-success float-right pt-3'>{{ContactCenter}}</div></div>";

        public const string INVESTOR_PERFORMANCE_WIDGET_HTML = "<div id={{WidgetId}} class='card border-0'><div class='card-body text-left py-1'><div class='card-body-header pb-2'>Investor performance</div><div class='InvestmentPermanaceDiv'><table class='InvestorPermanaceTable' border='0' id='InvestorPerformance'><tbody><tr><td class='w-50' colspan='2'><span class='text-success fnt-10pt'>{{ProductType}}</span></td></tr><tr><td class='w-50 fnt-8pt pt-1'>Opening balance</td><td class='w-50 fnt-8pt'>Closing balance</td></tr><tr><td class='w-50 fnt-14pt'>{{OpeningBalanceAmount}}</td><td class='w-50 fnt-14pt'>{{ClosingBalanceAmount}}</td></tr></tbody></table></div></div></div>";

        public const string INVESTMENT_PORTFOLIO_STATEMENT_WIDGET_HTML = "<div id={{WidgetId}} class='card border-0'><div class='card-body text-left py-1'><div class='card-body-header pb-2'>Investment portfolio statement {{DSName}} </div><div class='row pb-1'><div class='col-lg-4 col-sm-4 pr-1'><div class='TotalAmountDetailsDiv'><span class='fnt-10pt'>Current investor balance</span><br><span class='fnt-14pt'>{{TotalClosingBalance}}</span>&nbsp;<br></div></div><div class='col-lg-8 col-sm-8 pl-0'><div class='TotalAmountDetailsDiv'></div></div></div><div class='pt-1 pb-2' style='background-color:#f3f3f3'><table class='customTable mt-2' border='0' id='portfolio'><tbody class='fnt-8'><tr><td class='w-25'>Account type:</td><td class='w-25 text-right pr-4 text-success'>Investment</td><td class='w-25'>Statement Day:</td><td class='w-25 text-right text-success'>{{DayOfStatement}}</td></tr><tr><td class='w-25'>Investor no:</td><td class='w-25 text-right pr-4 text-success'>{{InvestorID}}</td><td class='w-25'>Statement Period:</td><td class='w-25 text-right text-success'>{{StatementPeriod}}</td></tr><tr><td class='w-25'>Statement date:</td><td class='w-25 text-right pr-4 text-success'>{{StatementDate}}</td></tr></tbody></table></div></div></div>";

        public const string BREAKDOWN_OF_INVESTMENT_ACCOUNTS_WIDGET_HTML = "<div id={{WidgetId}} class='card border-0'><div class='card-body text-left py-1'><div class='card-body-header pb-2'>Breakdown of your investment accounts</div>{{NavTab}} {{TabContentsDiv}} </div></div>";

        public const string INVESTMENT_ACCOUNT_DETAILS_HTML = "<div id='{{ProductDesc}}-{{InvestmentId}}' class='{{TabPaneClass}}'><div style='background-color: #f3f3f3;padding:10px 0px'><h4 class='pl-25px pt-2'><span class='InvestmentProdDesc'>{{ProductDesc}}</span></h4><table border='0' class='InvestmentDetail customTable'><tbody><tr><td class='w-25'>Investment no:</td><td class='text-right w-25'><span>{{InvestmentNo}}</span></td><td class='w-25'>Opening date:</td><td class='text-right w-25'><span>{{AccountOpenDate}}</span></td></tr><tr><td class='w-25'>Current interest rate:</td><td class='text-right w-25'><span>{{InterestRate}}</span></td><td class='w-25'>Maturity date:</td><td class='text-right w-25'><span>{{MaturityDate}}</span></td></tr><tr><td class='w-25'>Interest disposal:</td><td class='text-right w-25'><span>{{InterestDisposal}}</span></td><td class='w-25'>Notice period:</td><td class='text-right w-25'><span>{{NoticePeriod}}</span></td></tr><tr><td class='w-25'>Interest due:</td><td class='text-right w-25'><span>{{InterestDue}}</span></td><td class='w-25'>&nbsp;</td><td class='text-right w-25'>&nbsp;</td></tr></tbody></table><div class='InvestmentClosingBalanceDiv'><span class='fnt-8pt'>Balance at&nbsp;</span> <span class='text-success fnt-8pt'>{{LastTransactionDate}}</span><br><span class='text-success fnt-14pt'>{{BalanceOfLastTransactionDate}}</span></div></div> <div class='pt-1'><table class='InvestmentBreakdown customTable'><thead><tr class='ht-30'><th class='w-15'>Date</th><th class='w-40'>Description</th><th class='w-15 text-right'>Debit</th><th class='w-15 text-right'>Credit</th><th class='w-15 text-right'>Balance</th></tr></thead></table></div><div class='pt-0 overflow-auto' style='max-height:200px;'><table class='InvestmentBreakdown customTable'><tbody> {{InvestmentTransactionRows}} </tbody></table></div></div>";

        public const string EXPLANATORY_NOTES_WIDGET_HTML = "<div id={{WidgetId}} class='card border-0'><div class='card-body text-left py-1'><div class='card-body-header pb-2 fnt-10pt'>Explanatory notes</div><div class='ExplanatoryNotes'>{{Notes}}</div></div></div>";

        public const string SERVICE_WIDGET_HTML = "<div id={{WidgetId}} class='card border-0'><div class='card-body text-left p-0'><div class='ServicesDiv'><div class='serviceHeader pb-2'>{{ServiceMessageHeader}}</div>{{ServiceMessageText}}</div></div></div>";

        public const string PERSONAL_LOAN_DETAIL_HTML = "<div id={{WidgetId}} class='card border-0'><div class='card-body text-left py-1'><div class='card-body-header pb-2'>Personal loan statement</div><div class='row pb-1'><div class='col-lg-4 col-sm-4 pr-1'><div class='LoanAmountDetailsDiv'><span class='fnt-10pt'>Loan Amount</span><br><span class='fnt-14pt'>{{TotalLoanAmount}}</span>&nbsp;<br></div></div><div class='col-lg-4 col-sm-4 pr-1 pl-0'><div class='LoanAmountDetailsDiv'><span class='fnt-10pt'>Balance outstanding</span><br><span class='fnt-14pt'>{{OutstandingBalance}}</span>&nbsp;<br></div></div><div class='col-lg-4 col-sm-4 pl-0'><div class='LoanAmountDetailsDiv'><span class='fnt-10pt'>Now due</span><br><span class='fnt-14pt'>{{DueAmount}}</span>&nbsp;<br></div></div></div><div class='py-2' style='background-color:#f3f3f3'><h4 class='pl-25px'><span class='NedbankPersonalLoanTxt'>Nedbank personal loan</span></h4><table class='customTable mt-2' border='0'><tbody><tr><td class='w-25'>Account Number:</td><td class='w-25 text-right pr-4 text-success'>{{AccountNumber}}</td><td class='w-25'></td><td class='w-25 text-right text-success'></td></tr><tr><td class='w-25'>Statement date:</td><td class='w-25 text-right pr-4 text-success'>{{StatementDate}}</td> <td class='w-25'> Arrears:</td><td class='w-25 text-right text-success'>{{ArrearsAmount}}</td></tr><tr><td class='w-25'>Statement period:</td><td class='w-25 text-right pr-4 text-success'>{{StatementPeriod}}</td><td class='w-25'>Annual rate of interest:</td><td class='w-25 text-right pr-4 text-success'>{{AnnualRate}}</td></tr><tr><td class='w-25'>Monthly instalment:</td><td class='w-25 text-right pr-4 text-success'>{{MonthlyInstallment}}</td><td class='w-25'>Original term (months):</td><td class='w-25 text-right pr-4 text-success'>{{Terms}}</td></tr><tr><td class='w-25'>Due by date:</td><td class='w-25 text-right pr-4 text-success'>{{DueByDate}}</td><td class='w-25'></td><td class='w-25 text-right pr-4 text-success'></td></tr></tbody></table></div></div></div>";

        public const string PERSONAL_LOAN_TRANSACTION_HTML = "<div id={{WidgetId}} class='card border-0'><div class='card-body text-left py-0'><div class='pt-0'><table class='LoanTransactionTable customTable'><thead><tr class='ht-30'><th class='w-13'>Post date</th><th class='w-15'>Effective date</th><th class='w-35'>Transaction</th><th class='w-12 text-right'>Debit</th><th class='w-12 text-right'>Credit</th><th class='w-13 text-right'>Balance outstanding</th></tr></thead></table><div class='pt-0 overflow-auto' style='max-height:200px;'><table class='LoanTransactionTable customTable'><tbody>{{PersonalLoanTransactionRow}}</tbody></table></div></div></div></div>";

        public const string PERSONAL_LOAN_PAYMENT_DUE_HTML = "<div id={{WidgetId}} class='card border-0'><div class='card-body text-left py-1'><div class='payment-due-header pb-2'>Payment Due</div><div class='d-flex flex-row'><div class='paymentDueHeaderBlock mr-1'>After 120 + days</div><div class='paymentDueHeaderBlock mr-1'>After 90 days</div><div class='paymentDueHeaderBlock mr-1'>After 60 days</div><div class='paymentDueHeaderBlock mr-1'>After 30 days</div><div class='paymentDueHeaderBlock'>Current</div></div><div class='d-flex flex-row mt-1'><div class='paymentDueFooterBlock mr-1'>{{After120Days}}</div><div class='paymentDueFooterBlock mr-1'>{{After90Days}}</div><div class='paymentDueFooterBlock mr-1'>{{After60Days}}</div><div class='paymentDueFooterBlock mr-1'>{{After30Days}}</div><div class='paymentDueFooterBlock'>{{Current}}</div></div></div></div>";

        public const string SPECIAL_MESSAGE_HTML = "<div id={{WidgetId}} class='card border-0'><div class='card-body text-left py-1'><div class='SpecialMessageDiv'>{{SpecialMessageTextData}}</div></div></div>";

        public const string PERSONAL_LOAN_INSURANCE_MESSAGE_HTML = "<div id={{WidgetId}} class='card border-0'><div class='card-body text-left py-1'><div class='InsuranceMessageDiv'><div class='card-body-header pb-2'>Insurance</div>{{InsuranceMessages}}</div></div></div>";

        public const string CUSTOMER_INFORMATION_WIDGET_HTML_FOR_STMT = "<div id={{WidgetId}} class='card border-0' style='height:{{WidgetDivHeight}}'><div class='card-header bg-light border-0 text-left'><h5 class='m-0'>Customer Information</h5></div><div class='card-body'><div class='row'><div class='col-sm-4'><h4 class='mb-4'>{{CustomerName}}</h4> <h6>{{Address1}}{{Address2}}</h6></div><div class='col-sm-8'> <video style='height: 360px;float: right;width: 95%;' controls><source src='{{VideoSource}}' type='video/mp4'></video> </div></div></div></div>";

        public const string ACCOUNT_INFORMATION_WIDGET_HTML_FOR_STMT = "<div id={{WidgetId}} class='card border-0' style='height:{{WidgetDivHeight}}'><div class='card-header bg-light border-0 text-left'><h5 class='m-0'>Account Information</h5></div> <div class='card-body'>{{AccountInfoData}}</div></div>";

        public const string SUMMARY_AT_GLANCE_WIDGET_HTML_FOR_STMT = "<div id={{WidgetId}} class='card border-0' style='height:{{WidgetDivHeight}}'><div class='card-header bg-light border-0 text-left'> <h5 class='m-0'>Summary at Glance</h5></div> <div class='card-body'><div class='table-responsive'> <table class='table m-0 table-hover'><thead><tr><th>Account</th><th>Currency</th><th>Amount</th></tr></thead><tbody>{{AccountSummary}}</tbody></table></div></div></div>";

        public const string IMAGE_WIDGET_HTML_FOR_STMT = "<div id={{WidgetId}} class='card border-0' style='height:{{WidgetDivHeight}}'><div class='card-header bg-light border-0 text-left'><h5 class='m-0'>Image Information</h5></div> <div class='card-body text-center'>{{TargetLink}}<img src='{{ImageSource}}' class='img-fluid'/>{{EndTargetLink}}</div></div>";

        public const string VIDEO_WIDGET_HTML_FOR_STMT = "<div id={{WidgetId}} class='card border-0' style='height:{{WidgetDivHeight}}'><div class='card-header bg-light border-0 text-left'><h5 class='m-0'>Video Information</h5> </div> <div class='card-body text-center'><video style='float: right;width: 95%;' controls><source src='{{VideoSource}}' type='video/mp4'></video></div></div>";

        public const string SAVING_CURRENT_AVALABLE_BAL_WIDGET_HTML_FOR_STMT = "<div id={{WidgetId}} class='card border-0' style='height:{{WidgetDivHeight}}'><div class='card-header bg-light border-0 text-left'><h5 class='m-0'>Available Balance</h5> </div> <div class='card-body'><div class='fnt14'><h4 class='mb-4 text-right'><i class='{{AccountIndicatorClass}}' aria-hidden='true'></i>&nbsp;{{TotalValue}}</h4><span class='float-left'> Total Deposits</span><span class='float-right'>{{TotalDeposit}}</span><br/><span class='float-left'> Total Spend </span><span class='float-right'>{{TotalSpend}}</span><br/><span class='float-left'> Savings </span><span class='float-right'>{{Savings}}</span><br/></div></div></div>";

        public const string SAVING_TRANSACTION_WIDGET_HTML_FOR_STMT = "<div id={{WidgetId}} class='card border-0' style='height:{{WidgetDivHeight}}'><div class='card-header bg-light border-0 text-left'> <h5 class='m-0'>Transaction Details</h5></div> <div class='card-body'> <div class='float-left'> <input type='radio' id='savingShowAll' checked name='savingtransactionRadio'> &nbsp;<label for='showAll'>Show All</label>&nbsp; <input type='radio' id='savingGrpDate' name='savingtransactionRadio'>&nbsp;<label for='grpDate'>Group By Date</label></div> <div class='float-right'><div class='float-left mr-2'><select class='form-control float-left' id='filterStatus'><option value = '0'> Search Item</option>{{SelectOption}} </select></div> <a href='javascript:void(0)' class='btn btn-light btn-sm' id='ResetGrid'>Reset</a>&nbsp; <a href='javascript:void(0)' class='btn btn-light btn-sm' id='PrintGrid'>Print</a> </div> <div class='table-responsive stylescrollbar' style='max-height:320px;overflow-x:hidden;overflow-y:auto;'> <table id='SavingTransactionTable' class='table m-1 table-hover'><thead><tr> <th class='width12'>Date</th><th class='width8'>Type</th class='width30'><th>Narration</th><th class='width12 text-right'>FCY</th><th class='width13 text-right'>Current Rate</th> <th class='width13 text-right'>LCY</th><th class='width12'>Action</th></tr></thead><tbody>{{AccountTransactionDetails}}</tbody></table></div></div></div>";

        public const string CURRENT_TRANSACTION_WIDGET_HTML_FOR_STMT = "<div id={{WidgetId}} class='card border-0' style='height:{{WidgetDivHeight}}'><div class='card-header bg-light border-0 text-left'> <h5 class='m-0'>Transaction Details</h5></div> <div class='card-body'> <div class='float-left'> <input type='radio' id='currentShowAll' checked name='currenttransactionRadio'> &nbsp;<label for='showAll'>Show All</label>&nbsp; <input type='radio' id='currentGrpDate' name='currenttransactionRadio'>&nbsp;<label for='grpDate'>Group By Date</label></div> <div class='float-right'> <div class='float-left mr-2'> <select class='form-control float-left' id='filterStatus'><option value = '0'> Search Item</option>{{SelectOption}} </select></div> <a href='javascript:void(0)' class='btn btn-light btn-sm' id='ResetGrid'>Reset</a>&nbsp; <a href='javascript:void(0)' class='btn btn-light btn-sm' id='PrintGrid'>Print</a> </div> <div class='table-responsive stylescrollbar' style='max-height:320px;overflow-x:hidden;overflow-y:auto;'><table id='CurrentTransactionTable' class='table m-1 table-hover'><thead><tr> <th class='width12'>Date</th><th class='width8'>Type</th class='width30'><th>Narration</th><th class='width12 text-right'>FCY</th><th class='width13 text-right'>Current Rate</th> <th class='width13 text-right'>LCY</th><th class='width12'>Action</th></tr></thead><tbody>{{AccountTransactionDetails}}</tbody></table></div></div></div>";

        public const string REMINDER_WIDGET_HTML_FOR_STMT = "<div id={{WidgetId}} class='card border-0' style='height:{{WidgetDivHeight}}'><div class='card-header bg-light border-0 text-left'><h5 class='m-0'>Reminder and Recommendation</h5></div><div class='card-body' style='font-size:12px;'> {{ReminderAndRecommdationDataList}} </div></div>";

        public const string TOP_4_INCOME_SOURCE_WIDGET_HTML_FOR_STMT = "<div id={{WidgetId}} class='card border-0' style='height:{{WidgetDivHeight}}'><div class='card-header bg-light border-0 text-left'> <h5 class='m-0'>Top 4 Income Sources</h5></div><div class='card-body'><table class='table-borderless width100'><thead class='border-bottom'><tr><td class='width50'></td><td class='width20'>This Month</td><td class='width30'>Usually you spend</td></tr></thead><tbody>{{IncomeSourceList}}</tbody></table></div></div>";

        public const string ANALYTIC_WIDGET_HTML_FOR_STMT = "<div id={{WidgetId}} class='card border-0' style='height:{{WidgetDivHeight}}'><div class='card-header bg-light border-0 text-left'> <h5 class='m-0'>Analytics</h5></div><div class='card-body'> <div id=\"analyticschartcontainer\" style='height: 75%; width: 90%; position: absolute;'></div></div></div> ";

        public const string SAVING_TRENDS_WIDGET_HTML_FOR_STMT = "<div id={{WidgetId}} class='card border-0' style='height:{{WidgetDivHeight}}'><div class='card-header bg-light border-0 text-left'><h5 class='m-0'>Saving Trends</h5></div><div class='card-body'> <div id=\"savingTrendscontainer\" style='height: 75%; width: 90%; position: absolute;'></div></div></div> ";

        public const string SPENDING_TRENDS_WIDGET_HTML_FOR_STMT = "<div id={{WidgetId}} class='card border-0' style='height:{{WidgetDivHeight}}'><div class='card-header bg-light border-0 text-left'><h5 class='m-0'>Spending Trends</h5></div><div class='card-body'> <div id=\"spendingTrendscontainer\" style='height: 75%; width: 90%; position: absolute;'></div></div></div> ";

        public const string TABLE_WIDEGT_FOR_STMT = "<div id={{WidgetId}} class='card border-0' style='height:{{WidgetDivHeight}}'><div class='p-1 bg-light border-0 text-left'><h5 class='m-0;' style={{TitleStyle}}>{{WidgetTitle}}</h5></div><div class=''><div class='table-responsive stylescrollbar' style='max-height:{{TableMaxHeight}};overflow-x:hidden;overflow-y:auto;'> <table id='TableWidget' class='table m-1 table-hover'><thead style={{HeaderStyle}}>{{tableHeader}}</thead><tbody style={{BodyStyle}}>{{tableBody}}</tbody></table></div></div></div>";

        public const string FORM_WIDGET_FOR_STMT = "<div id={{WidgetId}} class='card border-0' style='height:{{WidgetDivHeight}}'><div class='p-1 bg-light border-0 text-left' style='margin-bottom: 10px;'><h5 class='m-0' style={{TitleStyle}}>{{WidgetTitle}}</h5></div><div class='card border-0 card-shadow width100'><div class='card-body p-2' style={{BodyStyle}}>{{FormData}}</div></div></div>";

        public const string HTML_WIDGET_FOR_STMT = "<div id={{WidgetId}} class='card border-0' style='height:{{WidgetDivHeight}}'><div class='p-1 bg-light border-0 text-left'><h5 class='m-0' style={{TitleStyle}}>{{WidgetTitle}}</h5></div><div class='card-body text-left' style={{BodyStyle}}>{{FormData}}</div></div>";

        public const string LINE_GRAPH_FOR_STMT = "<div id={{WidgetId}} class='card border-0' style='height:{{WidgetDivHeight}}'><div class='p-1 bg-light border-0 text-left'><h5 class='m-0' style={{TitleStyle}}>{{WidgetTitle}}</h5></div><div class='card-body'> <div id=\"lineGraphcontainer\" style='height: 75%; width: 90%; position: absolute;'></div></div></div> ";

        public const string PIE_CHART_FOR_STMT = "<div id={{WidgetId}} class='card border-0' style='height:{{WidgetDivHeight}}'><div class='p-1 bg-light border-0 text-left'><h5 class='m-0' style={{TitleStyle}}>{{WidgetTitle}}</h5></div><div class='card-body'> <div id=\"pieChartcontainer\" style='height: 75%; width: 90%; position: absolute;'></div></div></div> ";

        public const string BAR_GRAPH_FOR_STMT = "<div id={{WidgetId}} class='card border-0' style='height:{{WidgetDivHeight}}'><div class='p-1 bg-light border-0 text-left'> <h5 class='m-0' style={{TitleStyle}}>{{WidgetTitle}}</h5></div><div class='card-body'> <div id=\"barGraphcontainer\" style='height: 75%; width: 90%; position: absolute;'></div></div></div> ";

        public const string CONTAINER_DIV_HTML_HEADER = "<div class='container-fluid mt-3 bdy-scroll stylescrollbar'>";

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

        public const string TENANT_LOGO_SCRIPT = "<script type='text/javascript'> $(document).ready(function () {setTimeout(function () {if(null!=$('#TenantLogoImageValue').val()&&''!= $('#TenantLogoImageValue').val()) {let e=new Image; e.src= $('#TenantLogoImageValue').val(),e.height=40,$('#TenantLogo').append(e)} else if (null != $('#TenantName').val() && '' != $('#TenantName').val()) { let tName = $('#TenantName').val(); let e = tName.charAt(0).toUpperCase(),t=document.createElement('div'); t.classList.add('ltr-img'), t.style.height='50px',t.style.width='50px',t.style.fontSize='30px';let n=document.createElement('span'); n.textContent=e,t.appendChild(n),$('#TenantLogo').append(t)}},100)}); </script>";

        public const string STYLE = "color:{{COLOR}};font-size:{{SIZE}}px;font-weight:{{WEIGHT}};font-family:'{{TYPE}}';";

        public const string HTML_FOOTER = " {{ChartScripts}} </body></html>";

        public const string TABLEWIDEGTPREVIEW = "<div class='card border-0'><div class='p-1 bg-light border-0 text-left'><h5 class='m-0;' style={{TitleStyle}}>{{WidgetTitle}}</h5></div><div class='card-body'><div class='table-responsive stylescrollbar' style='max-height:350px;overflow-x:hidden;overflow-y:auto;'><table id = 'TableWidget' class='table m-1 table-hover'><thead style={{HeaderStyle}}>{{tableHeader}}</thead><tbody style={{BodyStyle}}>{{tableBody}}</tbody></table></div></div></div>";

        public const string FORMWIDGETPREVIEW = "<div class='card border-0'><div class='p-1 bg-light border-0 text-left' style='margin-bottom: 10px;'><h5 class='m-0' style={{TitleStyle}}>{{WidgetTitle}}</h5></div><div class='card border-0 card-shadow m-auto width100'><div class='card-body p-2' style={{BodyStyle}}>{{FormData}}</div></div></div>";

        public const string HTMLWIDGETPREVIEW = "<div class='card border-0'><div class='p-1 bg-light border-0 text-left'><h5 class='m-0' style={{TitleStyle}}>{{WidgetTitle}}</h5></div><div class='card-body' style={{BodyStyle}}>{{FormData}}</div></div>";

        public const string LINEGRAPH_WIDGETPREVIEW = "<div class='card border-0' style='height:600px'><div class='p-1 bg-light border-0 text-left'> <h5 class='m-0' style={{TitleStyle}}>{{WidgetTitle}}</h5></div><div class='card-body'> <div id=\"lineGraphcontainer\" style='height: 75%; width: 90%; position: absolute;'></div></div></div> ";

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
