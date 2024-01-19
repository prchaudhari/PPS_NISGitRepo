import { Component, OnInit, Injector, ChangeDetectorRef, ViewChild, OnDestroy } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { FormGroup, FormBuilder } from '@angular/forms';
import { NgxUiLoaderService } from 'ngx-ui-loader';
import { Router, NavigationEnd } from '@angular/router';
import { Constants, ErrorMessageConstants } from 'src/app/shared/constants/constants';
import { MessageDialogService } from 'src/app/shared/services/mesage-dialog.service';
import { LocalStorageService } from 'src/app/shared/services/local-storage.service';
import { Widget } from '../widget';
import { WidgetService } from '../widget.service';
import { TemplateService } from '../../template/template.service';

@Component({
  selector: 'app-add',
  templateUrl: './add.component.html',
  styleUrls: ['./add.component.scss']
})
export class AddComponent implements OnInit {

  //select widget type radio
  public widgetList: Widget[] = [];
  public selectedLink: Widget;
  setWidgetType(e): void {
    this.selectedLink = e;
  }
  public pageTypeList = [];
  constructor(private injector: Injector,
    private fb: FormBuilder,
    private _messageDialogService: MessageDialogService,
    private router: Router,
    private localstorageservice: LocalStorageService,
    private spinner: NgxUiLoaderService,
    private widgetService: WidgetService) {

  }

  ngOnInit() {
    this.getPageTypes();
    this.getWidgetRecords(null);
  }
  async getPageTypes() {
    let templateService = this.injector.get(TemplateService);
    this.pageTypeList = [{ "Identifier": 0, "PageTypeName": "Select Page Type" }];
    let list = await templateService.getPageTypes();
    if (this.pageTypeList.length == 0) {
      let message = ErrorMessageConstants.getNoRecordFoundMessage;
      this._messageDialogService.openDialogBox('Error', message, Constants.msgBoxError).subscribe(data => {
        if (data == true) {
          this.getPageTypes();
        }
      });
    } else {

      this.pageTypeList = [...this.pageTypeList, ...list];
    }
  }
  async getWidgetRecords(searchParameter) {
    //this.spinner.start();
    let widgetService = this.injector.get(WidgetService);
    if (searchParameter == null) {
      searchParameter = {};
      searchParameter.PagingParameter = {};
      searchParameter.PagingParameter.PageIndex = Constants.DefaultPageIndex;
      searchParameter.PagingParameter.PageSize = Constants.DefaultPageSize;
      searchParameter.SortParameter = {};
      searchParameter.SortParameter.SortColumn = "WidgetName";
      searchParameter.SortParameter.SortOrder = Constants.Ascending;
      searchParameter.SearchMode = Constants.Contains;
      searchParameter.IsPageTypeDetailsRequired = true;
    }
    this.widgetList = await widgetService.getWidget(searchParameter);

    if (this.widgetList.length != 0) {
      this.widgetList[0].Checked = true;
      this.selectedLink = this.widgetList[0];

      for (var i = 0; i < this.widgetList.length; i++) {
        if (this.widgetList[i].WidgetName == "CustomerInformation") {
          this.widgetList[i].ImageSource = "assets/images/CustomerInfoWidget.PNG";
          this.widgetList[i].WidgetIcon = "fa fa-address-book-o";
        }
        else if (this.widgetList[i].WidgetName == "FSPDetails") {
          this.widgetList[i].ImageSource = "assets/images/FSPDetails.png";
          this.widgetList[i].WidgetIcon = "fa fa-address-book";
        }
        else if (this.widgetList[i].WidgetName == "PaymentSummary") {
          this.widgetList[i].ImageSource = "assets/images/PaymentSummary.png";
          this.widgetList[i].WidgetIcon = "fa fa-address-book-o";
        }
        else if (this.widgetList[i].WidgetName == "ProductSummary") {
          this.widgetList[i].ImageSource = "assets/images/ProductSummary.png";
          this.widgetList[i].WidgetIcon = "fa fa-address-book-o";
        }
        else if (this.widgetList[i].WidgetName == "DetailedTransactions") {
          this.widgetList[i].ImageSource = "assets/images/DetailedTransactions.png";
          this.widgetList[i].WidgetIcon = "fa fa-address-book-o";
        }
        else if (this.widgetList[i].WidgetName == "PPSDetailedTransactions") {
          this.widgetList[i].ImageSource = "assets/images/PPSDetailedTransactions.png";
          this.widgetList[i].WidgetIcon = "fa fa-address-book-o";
        }
        else if (this.widgetList[i].WidgetName == "FSPHeading") {
          this.widgetList[i].ImageSource = "assets/images/PPSHeading.png";
          this.widgetList[i].WidgetIcon = "fa fa-address-book-o";
        }
        else if (this.widgetList[i].WidgetName == "PPSFooter1") {
          this.widgetList[i].ImageSource = "assets/images/footer1.PNG";
          this.widgetList[i].WidgetIcon = "fa fa-address-book-o";
        }
        else if (this.widgetList[i].WidgetName == "PPSDetails1") {
          this.widgetList[i].ImageSource = "assets/images/Details1.PNG";
          this.widgetList[i].WidgetIcon = "fa fa-address-book-o";
        }
        else if (this.widgetList[i].WidgetName == "PPSDetails2") {
          this.widgetList[i].ImageSource = "assets/images/Details2.PNG";
          this.widgetList[i].WidgetIcon = "fa fa-address-book-o";
        }
        else if (this.widgetList[i].WidgetName == "FooterImage") {
          this.widgetList[i].ImageSource = "assets/images/footerImage.png";
          this.widgetList[i].WidgetIcon = "fa fa-address-book-o";
        }
        else if (this.widgetList[i].WidgetName == "PPSEarningForPeriod") {
          this.widgetList[i].ImageSource = "assets/images/PPSEarningForPeriod.png";
          this.widgetList[i].WidgetIcon = "fa fa-address-book-o";
        }
        else if (this.widgetList[i].WidgetName == "AccountInformation") {
          this.widgetList[i].ImageSource = "assets/images/AccountInfoWidget.PNG"
          this.widgetList[i].WidgetIcon = "fa fa-address-card-o";

        }
        else if (this.widgetList[i].WidgetName == "Summary") {
          this.widgetList[i].ImageSource = "assets/images/SummaryWidget.PNG"
          this.widgetList[i].WidgetIcon = "icon-widget-single-column2";
        }
        else if (this.widgetList[i].WidgetName == "Image") {
          this.widgetList[i].ImageSource = "assets/images/icon-image.png"
          this.widgetList[i].WidgetIcon = "fa fa-image";
        }
        else if (this.widgetList[i].WidgetName == "Video") {
          this.widgetList[i].ImageSource = "assets/images/VideoPlaceholder.jpg"
          this.widgetList[i].WidgetIcon = "icon-videoWidget";
        }
        else if (this.widgetList[i].WidgetName == "Analytics") {
          this.widgetList[i].ImageSource = "assets/images/Analytics.png"
          this.widgetList[i].WidgetIcon = "icon-AnalyticsWidget";
        }
        else if (this.widgetList[i].WidgetName == "SavingTransaction") {
          this.widgetList[i].ImageSource = "assets/images/TranscationListWidget.PNG"
          this.widgetList[i].WidgetIcon = "icon-widget-multi-column2";
        }
        else if (this.widgetList[i].WidgetName == "CurrentTransaction") {
          this.widgetList[i].ImageSource = "assets/images/TranscationListWidget.PNG"
          this.widgetList[i].WidgetIcon = "icon-widget-multi-column2";
        }
        else if (this.widgetList[i].WidgetName == "SavingTrend") {
          this.widgetList[i].ImageSource = "assets/images/SavingTrendWidget.PNG"
          this.widgetList[i].WidgetIcon = "icon-widget-line-chart2";
        }
        else if (this.widgetList[i].WidgetName == "Top4IncomeSources") {
          this.widgetList[i].ImageSource = "assets/images/Top4IncomeWidget.PNG"
          this.widgetList[i].WidgetIcon = "icon-rp-quality1";
        }
        else if (this.widgetList[i].WidgetName == "CurrentAvailableBalance") {
          this.widgetList[i].ImageSource = "assets/images/AvailableBalanceWidget.PNG"
          this.widgetList[i].WidgetIcon = "icon-AvailableBalanceWidget";
        }
        else if (this.widgetList[i].WidgetName == "SavingAvailableBalance") {
          this.widgetList[i].ImageSource = "assets/images/AvailableBalanceWidget.PNG"
          this.widgetList[i].WidgetIcon = "icon-AvailableBalanceWidget";
        }
        else if (this.widgetList[i].WidgetName == "SpendingTrend") {
          this.widgetList[i].ImageSource = "assets/images/SpendingTrendWidget.PNG"
          this.widgetList[i].WidgetIcon = "icon-rp-production1";
        }
        else if (this.widgetList[i].WidgetName == "ReminderaAndRecommendation") {
          this.widgetList[i].ImageSource = "assets/images/ReminderWidget.PNG"
          this.widgetList[i].WidgetIcon = "fa fa-bell-o";
        }
        //else if (this.widgetList[i].WidgetName == "CustomerDetails") {
        //  this.widgetList[i].ImageSource = "assets/images/CustomerDetails.JPG";
        //  this.widgetList[i].WidgetIcon = "fa fa-address-book-o";
        //}
        //else if (this.widgetList[i].WidgetName == "BranchDetails") {
        //  this.widgetList[i].ImageSource = "assets/images/BankDetails.JPG"
        //  this.widgetList[i].WidgetIcon = "fa fa-university";
        //}
        //else if (this.widgetList[i].WidgetName == "WealthBranchDetails") {
        //  this.widgetList[i].ImageSource = "assets/images/WealthBranchDetails.PNG"
        //  this.widgetList[i].WidgetIcon = "fa fa-university";
        //}
        //else if (this.widgetList[i].WidgetName == "BreakdownOfInvestmentAccounts") {
        //  this.widgetList[i].ImageSource = "assets/images/BreakdownOfInvestmentAccounts.JPG"
        //  this.widgetList[i].WidgetIcon = "icon-widget-single-column2";
        //}
        //else if (this.widgetList[i].WidgetName == "WealthBreakdownOfInvestmentAccounts") {
        //  this.widgetList[i].ImageSource = "assets/images/WealthBreakdownOfInvestmentAccounts.PNG"
        //  this.widgetList[i].WidgetIcon = "icon-widget-single-column2";
        //}
        else if (this.widgetList[i].WidgetName == "Image") {
          this.widgetList[i].ImageSource = "assets/images/icon-image.png"
          this.widgetList[i].WidgetIcon = "fa fa-image";
        }
        else if (this.widgetList[i].WidgetName == "Video") {
          this.widgetList[i].ImageSource = "assets/images/VideoPlaceholder.jpg"
          this.widgetList[i].WidgetIcon = "icon-videoWidget";
        }
        //else if (this.widgetList[i].WidgetName == "InvestmentPortfolioStatement") {
        //  this.widgetList[i].ImageSource = "assets/images/InvestmentPortfolioStatement.JPG"
        //  this.widgetList[i].WidgetIcon = "icon-widget-multi-column2";
        //}
        //else if (this.widgetList[i].WidgetName == "InvestmentWealthPortfolioStatement") {
        //  this.widgetList[i].ImageSource = "assets/images/InvestmentWealthPortfolioStatement.PNG"
        //  this.widgetList[i].WidgetIcon = "icon-widget-multi-column2";
        //}
        //else if (this.widgetList[i].WidgetName == "InvestorPerformance") {
        //  this.widgetList[i].ImageSource = "assets/images/InvestorPerformance.JPG"
        //  this.widgetList[i].WidgetIcon = "icon-widget-line-chart2";
        //}
        //else if (this.widgetList[i].WidgetName == "WealthInvestorPerformance") {
        //  this.widgetList[i].ImageSource = "assets/images/WealthInvestorPerformance.PNG"
        //  this.widgetList[i].WidgetIcon = "icon-widget-line-chart2";
        //}
        //else if (this.widgetList[i].WidgetName == "ExplanatoryNotes") {
        //  this.widgetList[i].ImageSource = "assets/images/ExplanatoryNotes.JPG"
        //  this.widgetList[i].WidgetIcon = "fa fa-list";
        //}
        //else if (this.widgetList[i].WidgetName == "WealthExplanatoryNotes") {
        //  this.widgetList[i].ImageSource = "assets/images/WealthExplanatoryNotes.PNG"
        //  this.widgetList[i].WidgetIcon = "fa fa-list";
        //}
        //else if (this.widgetList[i].WidgetName == "NedbankService") {
        //  this.widgetList[i].ImageSource = "assets/images/NedbankService.JPG"
        //  this.widgetList[i].WidgetIcon = "fa fa-newspaper-o";
        //}
        //else if (this.widgetList[i].WidgetName == "WealthNedbankService") {
        //  this.widgetList[i].ImageSource = "assets/images/WealthNedbankService.PNG"
        //  this.widgetList[i].WidgetIcon = "fa fa-newspaper-o";
        //}
        //else if (this.widgetList[i].WidgetName == "PersonalLoanDetail") {
        //  this.widgetList[i].ImageSource = "assets/images/PersonalLoanDetails.JPG"
        //  this.widgetList[i].WidgetIcon = "icon-widget-single-column2";
        //}
        //else if (this.widgetList[i].WidgetName == "PersonalLoanTransaction") {
        //  this.widgetList[i].ImageSource = "assets/images/PersonalLoanTransactions.JPG"
        //  this.widgetList[i].WidgetIcon = "icon-widget-multi-column2";
        //}
        //else if (this.widgetList[i].WidgetName == "PersonalLoanPaymentDue") {
        //  this.widgetList[i].ImageSource = "assets/images/PaymentDue.JPG"
        //  this.widgetList[i].WidgetIcon = "icon-AvailableBalanceWidget";
        //}
        else if (this.widgetList[i].WidgetName == "PageBreak") {
          this.widgetList[i].WidgetIcon = "fa fa-scissors";
        }
        else if (this.widgetList[i].WidgetName == "SpecialMessage") {
          this.widgetList[i].ImageSource = "assets/images/SpecialMessage.JPG"
          this.widgetList[i].WidgetIcon = "fa fa-bell-o";
        }
        //else if (this.widgetList[i].WidgetName == "PL_InsuranceMessage") {
        //  this.widgetList[i].ImageSource = "assets/images/PL_InsuranceMessages.JPG"
        //  this.widgetList[i].WidgetIcon = "fa fa-exclamation-circle";
        //}
        //else if (this.widgetList[i].WidgetName == "PersonalLoanTotalAmountDetail") {
        //  this.widgetList[i].ImageSource = "assets/images/PersonalLoanTotalAmountDetails.JPG";
        //  this.widgetList[i].WidgetIcon = "icon-widget-single-column2";
        //}
        //else if (this.widgetList[i].WidgetName == "PersonalLoanAccountsBreakdown") {
        //  this.widgetList[i].ImageSource = "assets/images/PersonalLoanAccountsBreakdown.JPG";
        //  this.widgetList[i].WidgetIcon = "icon-widget-multi-column2";
        //}
        //else if (this.widgetList[i].WidgetName == "HomeLoanTotalAmountDetail") {
        //  this.widgetList[i].ImageSource = "assets/images/HomeLoanTotalAmountDetails.JPG";
        //  this.widgetList[i].WidgetIcon = "icon-widget-single-column2";
        //}
        //else if (this.widgetList[i].WidgetName == "HomeLoanAccountsBreakdown") {
        //  this.widgetList[i].ImageSource = "assets/images/HomeLoanAccountsBreakdown.JPG";
        //  this.widgetList[i].WidgetIcon = "icon-widget-multi-column2";
        //}
        //else if (this.widgetList[i].WidgetName == "HomeLoanPaymentDueSpecialMsg") {
        //  this.widgetList[i].ImageSource = "assets/images/HLPaymentDueSpecialMsg.JPG";
        //  this.widgetList[i].WidgetIcon = "fa fa-exclamation-circle";
        //}
        //else if (this.widgetList[i].WidgetName == "HomeLoanInstalmentDetail") {
        //  this.widgetList[i].ImageSource = "assets/images/HL_InstalmentDetails.JPG";
        //  this.widgetList[i].WidgetIcon = "fa fa-table";
        //}
        //else if (this.widgetList[i].WidgetName == "HomeLoanSummaryTaxPurpose") {
        //  this.widgetList[i].ImageSource = "assets/images/SummaryTaxPurpose.jpg";
        //  this.widgetList[i].WidgetIcon = "fa fa-tags";
        //}
        //else if (this.widgetList[i].WidgetName == "HomeLoanInstalment") {
        //  this.widgetList[i].ImageSource = "assets/images/NewInstalment.jpg";
        //  this.widgetList[i].WidgetIcon = "fa fa-repeat";
        //}
        //else if (this.widgetList[i].WidgetName == "WealthHomeLoanTotalAmountDetail") {
        //  this.widgetList[i].ImageSource = "assets/images/WealthHomeLoanTotalAmountDetail.jpg";
        //  this.widgetList[i].WidgetIcon = "icon-widget-single-column2";
        //}
        //else if (this.widgetList[i].WidgetName == "WealthHomeLoanAccountsBreakdown") {
        //  this.widgetList[i].ImageSource = "assets/images/WealthHomeLoanAccountsBreakdown.jpg";
        //  this.widgetList[i].WidgetIcon = "icon-widget-multi-column2";
        //}
        //else if (this.widgetList[i].WidgetName == "WealthHomeLoanSummaryTaxPurpose") {
        //  this.widgetList[i].ImageSource = "assets/images/WealthSummaryTaxPurpose.jpg";
        //  this.widgetList[i].WidgetIcon = "fa fa-tags";
        //}
        //else if (this.widgetList[i].WidgetName == "WealthHomeLoanInstalment") {
        //  this.widgetList[i].ImageSource = "assets/images/NewInstalment.jpg";
        //  this.widgetList[i].WidgetIcon = "fa fa-repeat";
        //}
        //else if (this.widgetList[i].WidgetName == "WealthHomeLoanBranchDetails") {
        //  this.widgetList[i].ImageSource = "assets/images/WealthBranchDetails.PNG"
        //  this.widgetList[i].WidgetIcon = "fa fa-university";
        //}
        //else if (this.widgetList[i].WidgetName == "PortfolioCustomerDetails") {
        //  this.widgetList[i].ImageSource = "assets/images/PortfolioCustomerDetails.JPG";
        //  this.widgetList[i].WidgetIcon = "fa fa-address-book-o";
        //}
        //else if (this.widgetList[i].WidgetName == "CustomerAddressDetails") {
        //  this.widgetList[i].ImageSource = "assets/images/PortfolioCustomerAddress.JPG";
        //  this.widgetList[i].WidgetIcon = "fa fa-address-card-o";
        //}
        //else if (this.widgetList[i].WidgetName == "ClientContactDetails") {
        //  this.widgetList[i].ImageSource = "assets/images/PortfolioClientContactDetails.JPG";
        //  this.widgetList[i].WidgetIcon = "fa fa-cc";
        //}
        //else if (this.widgetList[i].WidgetName == "AccountSummary") {
        //  this.widgetList[i].ImageSource = "assets/images/PortfolioAccountSummary.JPG";
        //  this.widgetList[i].WidgetIcon = "icon-widget-single-column2";
        //}
        //else if (this.widgetList[i].WidgetName == "AccountAnalysis") {
        //  this.widgetList[i].ImageSource = "assets/images/PortfolioAccountAnalysis.JPG";
        //  this.widgetList[i].WidgetIcon = "fa fa-bar-chart";
        //}
        //else if (this.widgetList[i].WidgetName == "PortfolioReminders") {
        //  this.widgetList[i].ImageSource = "assets/images/PortfolioReminders.JPG";
        //  this.widgetList[i].WidgetIcon = "fa fa-bell-o";
        //}
        //else if (this.widgetList[i].WidgetName == "PortfolioNewsAlerts") {
        //  this.widgetList[i].ImageSource = "assets/images/PortfolioNewsAlerts.JPG";
        //  this.widgetList[i].WidgetIcon = "fa fa-newspaper-o";
        //}
        //else if (this.widgetList[i].WidgetName == "GreenbacksContactUs") {
        //  this.widgetList[i].ImageSource = "assets/images/GreenbacksContactUs.JPG";
        //  this.widgetList[i].WidgetIcon = "fa fa-list-alt";
        //}
        //else if (this.widgetList[i].WidgetName == "YTDRewardsPoints") {
        //  this.widgetList[i].ImageSource = "assets/images/GreenbacksYTDRewardsPointsGraph.JPG";
        //  this.widgetList[i].WidgetIcon = "fa fa-bar-chart";
        //}
        //else if (this.widgetList[i].WidgetName == "PointsRedeemedYTD") {
        //  this.widgetList[i].ImageSource = "assets/images/GreenbacksPointsRedeemedYTDGraph.JPG";
        //  this.widgetList[i].WidgetIcon = "fa fa-bar-chart";
        //}
        //else if (this.widgetList[i].WidgetName == "ProductRelatedPointsEarned") {
        //  this.widgetList[i].ImageSource = "assets/images/GreenbacksProductRelatedPointsEarnedGraph.JPG";
        //  this.widgetList[i].WidgetIcon = "fa fa-bar-chart";
        //}
        //else if (this.widgetList[i].WidgetName == "CategorySpendRewards") {
        //  this.widgetList[i].ImageSource = "assets/images/GreenbacksCategorySpendPointsGraph.JPG";
        //  this.widgetList[i].WidgetIcon = "fa fa-bar-chart";
        //}
        //else if (this.widgetList[i].WidgetName == "GreenbacksTotalRewardPoints") {
        //  this.widgetList[i].ImageSource = "assets/images/TotalRewardsPoints.JPG";
        //  this.widgetList[i].WidgetIcon = "fa fa-trophy";
        //}
        else if (this.widgetList[i].WidgetName == "StaticHtml") {
          this.widgetList[i].ImageSource = "assets/images/StaticHtml.JPG";
          this.widgetList[i].WidgetIcon = "fa fa-code";
        }
        else if (this.widgetList[i].WidgetName == "CSAgentLogo") {
          this.widgetList[i].ImageSource = "assets/images/StaticHtml.JPG";
          this.widgetList[i].WidgetIcon = "fa fa-html5";
        }
        //else if (this.widgetList[i].WidgetName == "CorporateSaverAgentMessage") {
        //  this.widgetList[i].ImageSource = "assets/images/corporatesaveragentmessage.png";
        //  this.widgetList[i].WidgetIcon = "fa fa-exclamation-circle";
        //}
        //else if (this.widgetList[i].WidgetName == "CorporateSaverAgentAddress") {
        //  this.widgetList[i].ImageSource = "assets/images/CorporateSaveragentAddress.png";
        //  this.widgetList[i].WidgetIcon = "fa fa-address-card";
        //}
        //else if (this.widgetList[i].WidgetName == "CorporateSaverTransaction") {
        //  this.widgetList[i].ImageSource = "assets/images/CorporateSavertransaction.png";
        //  this.widgetList[i].WidgetIcon = "fa icon-widget-multi-column2";
        //}
        //else if (this.widgetList[i].WidgetName == "CorporateSaverClientandAgentDetails") {
        //  this.widgetList[i].ImageSource = "assets/images/CorporateSaveragentandclientdetails.png";
        //  this.widgetList[i].WidgetIcon = "fa fa-vcard-o";
        //}
        //else if (this.widgetList[i].WidgetName == "CorporateSaverTableTotal") {
        //  this.widgetList[i].ImageSource = "assets/images/CorporateSaverlasttotal.png";
        //  this.widgetList[i].WidgetIcon = "fa icon-widget-single-column2";
        //}
        //else if (this.widgetList[i].WidgetName == "CorporateAgentDetails") {
        //  this.widgetList[i].ImageSource = "assets/images/CorporateSavertaxtotal.png";
        //  this.widgetList[i].WidgetIcon = "fa fa-tags";
        //}
        else if (this.widgetList[i].WidgetName == "SegmentBasedContent") {
          this.widgetList[i].ImageSource = "assets/images/SegmentBasedContent.JPG";
          this.widgetList[i].WidgetIcon = "fa fa-id-card-o";
        }
        //else if (this.widgetList[i].WidgetName == "MCAAccountSummary") {
        //  this.widgetList[i].ImageSource = "assets/images/MCAAccountSummary.JPG"
        //  this.widgetList[i].WidgetIcon = "icon-widget-single-column2";
        //}
        //else if (this.widgetList[i].WidgetName == "MCATransaction") {
        //  this.widgetList[i].ImageSource = "assets/images/MCATransaction.JPG"
        //  this.widgetList[i].WidgetIcon = "icon-widget-multi-column2";
        //}
        //else if (this.widgetList[i].WidgetName == "MCAVATAnalysis") {
        //  this.widgetList[i].ImageSource = "assets/images/MCAVATAnalysis.JPG"
        //  this.widgetList[i].WidgetIcon = "fa fa-bar-chart";
        //}
        //else if (this.widgetList[i].WidgetName == "WealthMCAAccountSummary") {
        //  this.widgetList[i].ImageSource = "assets/images/WealthMCAAccountSummary.JPG"
        //  this.widgetList[i].WidgetIcon = "icon-widget-single-column2";
        //}
        //else if (this.widgetList[i].WidgetName == "WealthMCATransaction") {
        //  this.widgetList[i].ImageSource = "assets/images/WealthMCATransaction.JPG"
        //  this.widgetList[i].WidgetIcon = "icon-widget-multi-column2";
        //}
        //else if (this.widgetList[i].WidgetName == "WealthMCAVATAnalysis") {
        //  this.widgetList[i].ImageSource = "assets/images/WealthMCAVATAnalysis.JPG"
        //  this.widgetList[i].WidgetIcon = "fa fa-bar-chart";
        //}
        //else if (this.widgetList[i].WidgetName == "WealthMCABranchDetails") {
        //  this.widgetList[i].ImageSource = "assets/images/WealthBranchDetails.PNG"
        //  this.widgetList[i].WidgetIcon = "fa fa-university";
        //}
      }
    }

    // this.spinner.stop();
  }


  public onPageTypeSelected(event) {
    var searchParameter: any = {};
    searchParameter.PagingParameter = {};
    searchParameter.PagingParameter.PageIndex = Constants.DefaultPageIndex;
    searchParameter.PagingParameter.PageSize = Constants.DefaultPageSize;
    searchParameter.SortParameter = {};
    searchParameter.SortParameter.SortColumn = "WidgetName";
    searchParameter.SortParameter.SortOrder = Constants.Ascending;
    searchParameter.SearchMode = Constants.Contains;
    searchParameter.IsPageTypeDetailsRequired = true;

    const value = event.target.value;
    if (value == "0") {
      searchParameter.PageTypeId = "";
    }
    else {
      searchParameter.PageTypeId = value;

    }

    this.getWidgetRecords(searchParameter);
  }
}
