import { AppSettings } from '../../appsettings';
import { Component, OnInit, Injector, SecurityContext } from '@angular/core';
import { CompactType, DisplayGrid, GridsterConfig, GridsterItem, GridType } from 'angular-gridster2';
import { Location } from '@angular/common';
import { Constants } from 'src/app/shared/constants/constants';
import { Router, NavigationEnd } from '@angular/router';
import { MessageDialogService } from 'src/app/shared/services/mesage-dialog.service';
import { TemplateService } from '../../layout/template/template.service';
import { Template } from '../../layout/template/template';
import { TemplateWidget } from '../../layout/template/templateWidget';
import { ConfigConstants } from '../../shared/constants/configConstants';
import {
    CustomerInformationComponent, AccountInformationComponent, ImageComponent, VideoComponent, SummaryAtGlanceComponent, TransactionDetailsComponent,
    SavingAvailableBalanceComponent, CurrentAvailableBalanceComponent, SavingTransactionDetailsComponent,
    SpendindTrendsComponent, TopIncomeSourcesComponent, SavingTrendsComponent, AnalyticsWidgetComponent, ReminderAndRecommComponent,
  DynamicBarChartWidgetComponent, DynamicLineChartWidgetComponent, DynamicPieChartWidgetComponent, DynamicHhtmlComponent, StaticHtmlComponent, SegmentBasedContentComponent, CustomerDetailsComponent, BankDetailsComponent,
  InvestmentPortfolioStatementComponent, InvestmentWealthPortfolioStatementComponent, InvestorPerformanceComponent, WealthInvestorPerformanceComponent, BreakdownOfInvestmentAccountsComponent, WealthBreakdownOfInvestmentAccountsComponent, ExplanatoryNotesComponent, WealthExplanatoryNotesComponent,NedbankServiceComponent, WealthNedbankServiceComponent,
  PersonalLoanDetailComponent, PersonalLoanTransactionComponent, PersonalLoanPaymentDueComponent, SpecialMessageComponent, PersonalLoanInsuranceMessageComponent,
  PersonalLoanTotalAmountDetailComponent, PersonalLoanAccountsBreakdownComponent, HomeLoanTotalAmountDetailComponent, HomeLoanAccountsBreakdownComponent, HomeLoanPaymentDueSpecialMsgComponent,
  HomeLoanInstalmentDetailComponent, PortfolioCustomerDetailsComponent, PortfolioCustomerAddressDetailsComponent, PortfolioClientContactDetailsComponent, PortfolioAccountSummaryDetailsComponent,
  PortfolioAccountAnalysisComponent, PortfolioRemindersComponent, PortfolioNewsAlertsComponent, GreenbacksContactUsComponent, YTDRewardPointsComponent, PointsRedeemedYTDComponent, ProductRelatedPointsEarnedComponent, CategorySpendRewardsComponent, GreenbacksTotalRewardPointsComponent
  } from '../widgetComponent/widgetComponent';
import { FormBuilder } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';
import * as $ from 'jquery';
import { DomSanitizer } from '@angular/platform-browser';
import { DynamicWidgetService } from '../../layout/widget-dynamic/dynamicwidget.service';

@Component({
  selector: 'app-view-dashboard-designer',
  templateUrl: './view-dashboard-designer.component.html',
  styleUrls: ['./view-dashboard-designer.component.scss']
})
export class ViewDashboardDesignerComponent implements OnInit {

    public isImageConfig: boolean = false;
    public isVideoConfig: boolean = false;
    public isWidgetSidebar: boolean = false;
    public isEmbedded: boolean = false;
    public isPersonalizeImage: boolean = false;
    public isPersonalize: boolean = false;
    
    public widgetsGridsterItemArray:any[] = [];
    public params: any = {};
    public PageIdentifier;
    public PageName;    
    public options: GridsterConfig;
    public dashboard: Array<GridsterItem>;
  public baseURL: string = AppSettings.baseURL;

    public templateList: Template[] = [];
    public imgAssetLibraryId: number;
    public imgAssetId: number;
    public imageSourceUrl:  string;

    public vdoAssetLibraryId: number;
    public vdoAssetId: number;
    public vdoSourceUrl:  string;
    public imageWidgetId: number = 0;
    public videoWidgetId: number = 0;
    public widgetItemCount: number = 0;
    public selectedWidgetItemCount: number = 0;

    public imgAssetLibraryName: string = "";
    public imgAssetName: string = "";
    public vdoAssetName: string = "";
    public vdoAssetLibraryName: string = "";
    public pageVersion: string = "";
    public isNoConfigurationSaved: boolean = false;
    public widgetsArray: any = [];
    public PageTypeId = 0;

    //Back Functionality.
    backClicked() {
        this._location.back();
    }

    constructor(private _location: Location,
        private fb: FormBuilder,
        private injector: Injector,
        private _messageDialogService: MessageDialogService,
        private _http: HttpClient,
        private sanitizer: DomSanitizer,
        private router: Router) { 
          router.events.subscribe(e => {
              if (e instanceof NavigationEnd) {
                  if (e.url.includes('/dashboardDesignerView')) {
                      //set passing parameters to localstorage.
                      if (localStorage.getItem('pageDesignViewRouteparams')) {
                          this.params = JSON.parse(localStorage.getItem('pageDesignViewRouteparams'));
                          this.PageName = this.params.Routeparams.passingparams.PageName
                          this.PageIdentifier = this.params.Routeparams.passingparams.PageIdentifier
                      }
                  } else {
                      localStorage.removeItem("dashboardDesignerView");
                  }
              }
          });
      }

    isImageConfigForm(widgetId, widgetItemCount) {
        this.isImageConfig = true;
        this.imageWidgetId = widgetId;
        this.selectedWidgetItemCount = widgetItemCount;
        this.isNoConfigurationSaved = false;
        var records = this.widgetsGridsterItemArray.filter(x => x.WidgetId == this.imageWidgetId && x.widgetItemCount == this.selectedWidgetItemCount);
        if(records.length != 0) {
            var widgetSetting = records[0].WidgetSetting;
            if(widgetSetting != null && widgetSetting != '' && this.testJSON(widgetSetting)) {
                var widgetConfigObj = JSON.parse(widgetSetting);
                this.imgAssetId = widgetConfigObj.AssetId;
                this.imgAssetName = widgetConfigObj.AssetName;
                this.imgAssetLibraryId = widgetConfigObj.AssetLibraryId;
                this.imgAssetLibraryName = widgetConfigObj.AssetLibrayName;
                this.imageSourceUrl = widgetConfigObj.SourceUrl;
                if(widgetConfigObj.isPersonalize != null) {
                    this.isPersonalizeImage = widgetConfigObj.isPersonalize;
                }
            }else {
                this.isNoConfigurationSaved = true;
            }
        }
    }

    testJSON(text){
        if (typeof text!=="string"){
            return false;
        }
        try{
            JSON.parse(text);
            return true;
        }
        catch (error){
            return false;
        }
    }

    isVideoConfigForm(widgetId, widgetItemCount) {
        this.isVideoConfig = true;
        this.videoWidgetId = widgetId;
        this.selectedWidgetItemCount = widgetItemCount;
        this.isNoConfigurationSaved = false;
        var records = this.widgetsGridsterItemArray.filter(x => x.WidgetId == this.videoWidgetId && x.widgetItemCount == this.selectedWidgetItemCount);
        if(records.length != 0) {
            var widgetSetting = records[0].WidgetSetting;
            if(widgetSetting != null && widgetSetting != '' && this.testJSON(widgetSetting)) {
                var widgetConfigObj = JSON.parse(widgetSetting);
                this.vdoAssetId = widgetConfigObj.AssetId;
                this.vdoAssetName = widgetConfigObj.AssetName;
                this.vdoAssetLibraryId = widgetConfigObj.AssetLibraryId;
                this.vdoAssetLibraryName = widgetConfigObj.AssetLibrayName;
                this.vdoSourceUrl = widgetConfigObj.SourceUrl;
                if(widgetConfigObj.isPersonalize != null) {
                    this.isPersonalize = widgetConfigObj.isPersonalize;
                }
                if(widgetConfigObj.isEmbedded != null) {
                    this.isEmbedded = widgetConfigObj.isEmbedded;
                }
            }else {
                this.isNoConfigurationSaved = true;
            }
        }
    }

    ngOnInit() {

        //gridster
        this.options = {
            gridType: GridType.ScrollVertical,
            compactType: CompactType.None,
            margin: 10,
            outerMargin: true,
            outerMarginTop: null,
            outerMarginRight: null,
            outerMarginBottom: null,
            outerMarginLeft: null,
            useTransformPositioning: true,
            mobileBreakpoint: 640,
            minCols: 12,
            maxCols: 12,
            minRows: 20,
            maxRows: 100,
            maxItemCols: 100,
            minItemCols: 1,
            maxItemRows: 100,
            minItemRows: 1,
            maxItemArea: 2500,
            minItemArea: 1,
            defaultItemCols: 1,
            defaultItemRows: 1,
            fixedColWidth: 175,
            fixedRowHeight: 105,
            keepFixedHeightInMobile: false,
            keepFixedWidthInMobile: false,
            scrollSensitivity: 10,
            scrollSpeed: 20,
            enableEmptyCellClick: false,
            enableEmptyCellContextMenu: false,
            enableEmptyCellDrop: false,
            enableEmptyCellDrag: false,
            emptyCellDragMaxCols: 10,
            emptyCellDragMaxRows: 10,
            ignoreMarginInRow: false,
            draggable: {
                enabled: false,
            },
            resizable: {
                enabled: false,
            },
            swap: false,
            pushItems: false,
            disablePushOnDrag: false,
            disablePushOnResize: false,
            pushDirections: { north: false, east: false, south: false, west: false },
            pushResizeItems: false,
            displayGrid: DisplayGrid.None,
            disableWindowResize: false,
            disableWarnings: false,
            scrollToNewItems: true
        };
        this.getPageRecord();
    }

    async getStaticAndDynamicWidgets() {
        let dynamicWidgetService = this.injector.get(DynamicWidgetService);
        var response = await dynamicWidgetService.getStaticAndDynamicWidgets(this.PageTypeId);
        this.widgetsArray = <any[]>response;
    }

    async getPageRecord() {
        let templateService = this.injector.get(TemplateService);
        let searchParameter: any = {};
        searchParameter.IsActive = true;
        searchParameter.Identifier = this.PageIdentifier;
        searchParameter.IsPageWidgetsRequired = true;
        searchParameter.PagingParameter = {};
        searchParameter.PagingParameter.PageIndex = Constants.DefaultPageIndex;
        searchParameter.PagingParameter.PageSize = Constants.DefaultPageSize;
        searchParameter.SortParameter = {};
        searchParameter.SortParameter.SortColumn = 'DisplayName';
        searchParameter.SortParameter.SortOrder = Constants.Ascending;
        searchParameter.SearchMode = Constants.Exact;
        var response = await templateService.getTemplates(searchParameter);
        this.templateList = response.templateList;
        if (this.templateList.length == 0) {
            let message = "NO record found";
            this._messageDialogService.openDialogBox('Error', message, Constants.msgBoxError).subscribe(data => {
                if (data == true) {
                    this.getPageRecord();
                }
            });
        }else {
            let template = this.templateList[0];
            this.applyBackgroundImage(template.BackgroundImageAssetId, template.BackgroundImageURL);
            this.PageName = template.DisplayName;
            this.PageTypeId = template.PageTypeId;
            this.PageIdentifier = template.Identifier;
            this.pageVersion = template.Version;
            await this.getStaticAndDynamicWidgets();
            let pageWidgets: TemplateWidget[] = template.PageWidgets;
            if(pageWidgets.length != 0) {
                for(let i=0; i < pageWidgets.length; i++) {
                    this.widgetItemCount = this.widgetItemCount + 1;
                    let gridsterItem: any = {};
                    gridsterItem.x = pageWidgets[i].Xposition;
                    gridsterItem.y = pageWidgets[i].Yposition;
                    gridsterItem.cols = pageWidgets[i].Width;
                    gridsterItem.rows = pageWidgets[i].Height;
                    gridsterItem.WidgetId = pageWidgets[i].WidgetId;
                    gridsterItem.WidgetSetting = pageWidgets[i].WidgetSetting;
                    gridsterItem.widgetItemCount = this.widgetItemCount;
                    gridsterItem.IsDynamicWidget = pageWidgets[i].IsDynamicWidget;               
                    let obj = this.bindComponent(pageWidgets[i]);
                    gridsterItem.component = obj.component;
                    gridsterItem.value = obj.value;
                    gridsterItem.WidgetType = obj.WidgetType;
                    gridsterItem.TempImageIdentifier = obj.TempImageIdentifier;
                    this.widgetsGridsterItemArray.push(gridsterItem);
                }
            }
        }
    }

  bindComponent(widget): any {
        let widgetName = widget.WidgetName == undefined ? widget.value : widget.WidgetName;
        let widgetType = widget.IsDynamicWidget == false ? 'Static' : 'Dynamic';
        let gridObj: any = {};
        if(widgetType == 'Static') {
            if(widgetName == 'CustomerInformation') {
                gridObj.component = CustomerInformationComponent;
            }
            else if(widgetName == 'AccountInformation') {
                gridObj.component = AccountInformationComponent;
            }
            else if(widgetName == 'Image') {
                gridObj.component = ImageComponent;
            }
            else if(widgetName == 'Video') {
                gridObj.component = VideoComponent;
            }
            else if(widgetName == 'Summary') {
                gridObj.component = SummaryAtGlanceComponent;
            }
            else if (widgetName == 'CurrentAvailableBalance') {
                gridObj.component = CurrentAvailableBalanceComponent;
            }
            else if (widgetName == 'SavingAvailableBalance') {
                gridObj.component = SavingAvailableBalanceComponent;
            }
            else if (widgetName == 'CurrentTransaction') {
                gridObj.component = TransactionDetailsComponent;
            }
            else if (widgetName == 'SavingTransaction') {
                gridObj.component = SavingTransactionDetailsComponent;
            }
            else if (widgetName == 'SpendingTrend') {
                gridObj.component = SpendindTrendsComponent;
            }
            else if (widgetName == 'Top4IncomeSources') {
                gridObj.component = TopIncomeSourcesComponent;
            }
            else if (widgetName == 'SavingTrend') {
                gridObj.component = SavingTrendsComponent;
            }
            else if (widgetName == 'Analytics') {
                gridObj.component = AnalyticsWidgetComponent;
            }
            else if (widgetName == 'ReminderaAndRecommendation') {
                gridObj.component = ReminderAndRecommComponent;
            }
            else if (widgetName == 'CustomerDetails') {
              gridObj.component = CustomerDetailsComponent;
            }
            else if (widgetName == 'BranchDetails') {
              gridObj.component = BankDetailsComponent;
            }
            else if (widgetName == 'InvestmentPortfolioStatement') {
              gridObj.component = InvestmentPortfolioStatementComponent;
            }
            else if (widgetName == 'InvestmentWealthPortfolioStatement') {
              gridObj.component = InvestmentWealthPortfolioStatementComponent;
            }
            else if (widgetName == 'InvestorPerformance') {
              gridObj.component = InvestorPerformanceComponent;
            }
            else if (widgetName == 'WealthInvestorPerformance') {
              gridObj.component = WealthInvestorPerformanceComponent;
            }
            else if (widgetName == 'BreakdownOfInvestmentAccounts') {
              gridObj.component = BreakdownOfInvestmentAccountsComponent;
            }
            else if (widgetName == 'WealthBreakdownOfInvestmentAccounts') {
              gridObj.component = WealthBreakdownOfInvestmentAccountsComponent;
            }
            else if (widgetName == 'ExplanatoryNotes') {
              gridObj.component = ExplanatoryNotesComponent;
            }
            else if (widgetName == 'WealthExplanatoryNotes') {
              gridObj.component = WealthExplanatoryNotesComponent;
            }
            else if (widgetName == 'PersonalLoanDetail') {
              gridObj.component = PersonalLoanDetailComponent;
            }
            else if (widgetName == 'PersonalLoanTransaction') {
              gridObj.component = PersonalLoanTransactionComponent;
            }
            else if (widgetName == 'PersonalLoanPaymentDue') {
              gridObj.component = PersonalLoanPaymentDueComponent;
            }
            else if (widgetName == 'SpecialMessage') {
              gridObj.component = SpecialMessageComponent;
            }
            else if (widgetName == 'PL_InsuranceMessage') {
              gridObj.component = PersonalLoanInsuranceMessageComponent;
            }
            else if (widgetName == 'NedbankService') {
              gridObj.component = NedbankServiceComponent;
            }
            else if (widgetName == 'WealthNedbankService') {
              gridObj.component = WealthNedbankServiceComponent;
            }
            else if (widgetName == 'PersonalLoanTotalAmountDetail') {
              gridObj.component = PersonalLoanTotalAmountDetailComponent;
            }
            else if (widgetName == 'PersonalLoanAccountsBreakdown') {
              gridObj.component = PersonalLoanAccountsBreakdownComponent;
            }
            else if (widget.WidgetName == "HomeLoanTotalAmountDetail") {
              gridObj.component = HomeLoanTotalAmountDetailComponent;
            }
            else if (widget.WidgetName == "HomeLoanAccountsBreakdown") {
              gridObj.component = HomeLoanAccountsBreakdownComponent
            }
            else if (widget.WidgetName == "HomeLoanPaymentDueSpecialMsg") {
              gridObj.component = HomeLoanPaymentDueSpecialMsgComponent
            }
            else if (widget.WidgetName == "HomeLoanInstalmentDetail") {
              gridObj.component = HomeLoanInstalmentDetailComponent
            }
            else if (widget.WidgetName == "PortfolioCustomerDetails") {
              gridObj.component = PortfolioCustomerDetailsComponent
            }
            else if (widget.WidgetName == "CustomerAddressDetails") {
              gridObj.component = PortfolioCustomerAddressDetailsComponent
            }
            else if (widget.WidgetName == "ClientContactDetails") {
              gridObj.component = PortfolioClientContactDetailsComponent
            }
            else if (widget.WidgetName == "AccountSummary") {
              gridObj.component = PortfolioAccountSummaryDetailsComponent
            }
            else if (widget.WidgetName == "AccountAnalysis") {
              gridObj.component = PortfolioAccountAnalysisComponent
            }
            else if (widget.WidgetName == "PortfolioReminders") {
              gridObj.component = PortfolioRemindersComponent
            }
            else if (widget.WidgetName == "PortfolioNewsAlerts") {
              gridObj.component = PortfolioNewsAlertsComponent
            }
            else if (widget.WidgetName == "GreenbacksTotalRewardPoints") {
              gridObj.component = GreenbacksTotalRewardPointsComponent
            }
            else if (widget.WidgetName == "GreenbacksContactUs") {
              gridObj.component = GreenbacksContactUsComponent
            }
            else if (widget.WidgetName == "YTDRewardsPoints") {
              gridObj.component = YTDRewardPointsComponent
            }
            else if (widget.WidgetName == "PointsRedeemedYTD") {
              gridObj.component = PointsRedeemedYTDComponent
            }
            else if (widget.WidgetName == "ProductRelatedPointsEarned") {
              gridObj.component = ProductRelatedPointsEarnedComponent
            }
            else if (widget.WidgetName == "CategorySpendRewards") {
              gridObj.component = CategorySpendRewardsComponent
            }
            else if (widget.WidgetName == "StaticHtml") {
              gridObj.component = StaticHtmlComponent
            }
            else if (widget.WidgetName == "SegmentBasedContent") {
              gridObj.component = SegmentBasedContentComponent
            }
        }
        else {
            let dynaWidgets = this.widgetsArray.filter(item => item.Identifier == widget.WidgetId && item.WidgetName == widgetName && item.WidgetType != 'Static');
            widgetType = dynaWidgets[0].WidgetType;
            if(widgetType == 'Table') {
                gridObj.component = SummaryAtGlanceComponent;
            }
            else if(widgetType == 'Form') {
                gridObj.component = AccountInformationComponent;
            }
            else if(widgetType == 'LineGraph') {
                gridObj.component = DynamicLineChartWidgetComponent;
            }
            else if(widgetType == 'BarGraph') {
                gridObj.component = DynamicBarChartWidgetComponent;
            }
            else if(widgetType == 'PieChart') {
                gridObj.component = DynamicPieChartWidgetComponent;
            }
            else if(widgetType == 'Html') {
                gridObj.component = DynamicHhtmlComponent;
            }

        }
        gridObj.WidgetType = widgetType;
        gridObj.value = widgetName;

        if (widgetName == 'Image' && widget != null && widget != null && widget.WidgetSetting != '' && this.testJSON(widget.WidgetSetting)) {
          let widgetSetting = JSON.parse(widget.WidgetSetting);
          if (widgetSetting.TempImageIdentifier != null) {
            gridObj.TempImageIdentifier = widgetSetting.TempImageIdentifier;
          }
        }
    
        return gridObj;
    }

    applyBackgroundImage(AssetId, ImageURL) {
        if(AssetId != null && AssetId != 0) {
          this._http.get(this.baseURL + 'assetlibrary/asset/download?assetIdentifier=' + AssetId, { responseType: "arraybuffer", observe: 'response' }).pipe(map(response => response))
              .subscribe(
                data => {
                  let contentType = data.headers.get('Content-Type');
                  const blob = new Blob([data.body], { type: contentType });
                  let objectURL = URL.createObjectURL(blob);
                  let imgUrl = this.sanitizer.sanitize(SecurityContext.RESOURCE_URL, this.sanitizer.bypassSecurityTrustResourceUrl(objectURL));
                  $('gridster').css('background', 'url('+imgUrl+')');
                },
                error => {
                  //$('.overlay').show();
              });
        }else if(ImageURL != null && ImageURL != '') {
          $('gridster').css('background', 'url('+ImageURL+')');
        }    
    }
    
}
