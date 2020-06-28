import { Component, OnInit, ViewChild, ComponentFactoryResolver, Injector, } from '@angular/core';
import { CompactType, DisplayGrid, GridsterConfig, GridsterItem, GridType } from 'angular-gridster2';
import { Location } from '@angular/common';
import { Constants } from 'src/app/shared/constants/constants';
import * as $ from 'jquery';
import { ActivatedRoute, Router, NavigationEnd } from '@angular/router';
import { NgxUiLoaderService } from 'ngx-ui-loader';
import { HttpEvent, HttpEventType, HttpResponse } from '@angular/common/http';
import { MessageDialogService } from 'src/app/shared/services/mesage-dialog.service';
import { DialogService } from '@tomblue/ng2-bootstrap-modal';
import { TemplateService } from '../../layout/template/template.service';
import { LocalStorageService } from 'src/app/shared/services/local-storage.service';
import { Template } from '../../layout/template/template';
import { TemplateWidget } from '../../layout/template/templateWidget';
import { CustomerInformationComponent, AccountInformationComponent, ImageComponent, VideoComponent, SummaryAtGlanceComponent } from '../widgetComponent/widgetComponent';

@Component({
  selector: 'app-add-dashboard-designer',
  templateUrl: './add-dashboard-designer.component.html',
  styleUrls: ['./add-dashboard-designer.component.scss']
})
export class AddDashboardDesignerComponent implements OnInit {
    public isImageConfig: boolean = false;
    public isVideoConfig: boolean = false;
    public isWidgetSidebar: boolean = false;
    public isEmbedded: boolean = false;
    public isPersonalizeImage: boolean = false;
    public isPersonalize: boolean = false;

    options: GridsterConfig;
    dashboard: Array<GridsterItem>;
    item: any[];
    public widgetsArray:any = [];
    public widgetId;
    public widgetsGridsterItemArray:any[] = [];
    public params: any = {};
    public PageIdentifier;
    public PageName;
    public PageTypeId;
    public PageTypeName;
    public pageEditModeOn: boolean = false;

    public templateList: Template[] = [];

    constructor(private _location: Location,
      private injector: Injector,
      private _dialogService: DialogService,
      private uiLoader: NgxUiLoaderService,
      private _messageDialogService: MessageDialogService,
      private localstorageservice: LocalStorageService,
      private router: Router) { 
        router.events.subscribe(e => {
            if (e instanceof NavigationEnd) {
                if (e.url.includes('/dashboardDesigner')) {
                    //set passing parameters to localstorage.
                    if (localStorage.getItem('pageDesignRouteparams')) {
                        this.params = JSON.parse(localStorage.getItem('pageDesignRouteparams'));
                        this.PageName = this.params.Routeparams.passingparams.PageName
                        this.PageTypeId = this.params.Routeparams.passingparams.PageTypeId
                        this.PageTypeName = this.params.Routeparams.passingparams.PageTypeName
                        this.pageEditModeOn = this.params.Routeparams.passingparams.pageEditModeOn
                    }
                    else if (localStorage.getItem('pageDesignEditRouteparams')) {
                        this.params = JSON.parse(localStorage.getItem('pageDesignEditRouteparams'));
                        this.PageName = this.params.Routeparams.passingparams.PageName
                        this.PageIdentifier = this.params.Routeparams.passingparams.PageIdentifier
                        this.pageEditModeOn = this.params.Routeparams.passingparams.pageEditModeOn
                    }
                } else {
                    localStorage.removeItem("pageDesignRouteparams");
                }
            }
        });
    }
    
    isImageConfigForm() {
        this.isImageConfig = true;
    }

    isVideoConfigForm() {
        this.isVideoConfig = true;
    }

    ngOnInit() {

        $(document).ready(function () {
            $('.widget-delete-btn').click(function () {
                $(this).parent().parent().parent().parent().addClass('hide');
            });
        });

        if(this.pageEditModeOn) {
            this.getPageRecord();
        }

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
            minCols: 20,
            maxCols: 20,
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
            fixedColWidth: 105,
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
                enabled: true,
            },
            resizable: {
                enabled: false,
            },
            swap: false,
            pushItems: true,
            disablePushOnDrag: false,
            disablePushOnResize: false,
            pushDirections: { north: true, east: true, south: true, west: true },
            pushResizeItems: false,
            displayGrid: DisplayGrid.Always,
            disableWindowResize: false,
            disableWarnings: false,
            scrollToNewItems: true
        };

        //for value
        this.dashboard = [
            { cols: 2, rows: 2, y: 0, x: 0 },
            { cols: 2, rows: 2, y: 0, x: 2, hasContent: true },
            { cols: 1, rows: 2, y: 0, x: 4 },
            { cols: 1, rows: 2, y: 2, x: 5 },
            { cols: 1, rows: 2, y: 1, x: 0 },
            { cols: 1, rows: 2, y: 1, x: 0 },
            { cols: 2, rows: 2, y: 3, x: 5, minItemRows: 2, minItemCols: 2, label: 'Min rows & cols = 2' },
            { cols: 2, rows: 2, y: 2, x: 0, maxItemRows: 2, maxItemCols: 2, label: 'Max rows & cols = 2' },
            { cols: 2, rows: 2, y: 2, x: 2, dragEnabled: true, resizeEnabled: true, label: 'Drag&Resize Enabled' },
            { cols: 1, rows: 2, y: 2, x: 4, dragEnabled: false, resizeEnabled: false, label: 'Drag&Resize Disabled' },
            { cols: 1, rows: 2, y: 2, x: 6 }
        ];

        //Array for showing widgets in the sidebar--
        this.widgetsArray = [
            {
              type:  "customerInformation",
              value: "CustomerInformation",
              title: "Customer Information",
            },
            {
              type: "accountInformation",
              value: "AccountInformation" ,
              title: "Account Information",
            },
            {
              type: "summaryAtGlance",
              value: "SummaryAtGlance",
              title: "Summary at Glance",
            },
            {
              type: "image",
              value: "Image",
              title: "Image",
            },
            {
              type: "video",
              value: "Video",
              title: "Video",
            },
            // {
            //   type: "reminderAndRecommendation",
            //   value: "ReminderAndRecommendation",
            //   title: "Reminder & Recommendation",
            // },
            // {
            //   type: "investmentPortfolioRisk",
            //   value: "InvestmentPortfolioRisk",
            //   title: "Investment Portfolio Risk",
            // },
            // {
            //     type: "analytics",
            //     value: "Analytics",
            //     title: "Analytics",
            // },
            // {
            //     type: "availableBalance",
            //     value: "AvailableBalance",
            //     title: "Available Balance",
            // },
            // {
            //     type: "transactionList",
            //     value: "TransactionList",
            //     title: "Transaction List",
            // },
            // {
            //     type: "savingTrend",
            //     value: "SavingTrend",
            //     title: "Saving Trend",
            // },
            // {
            //     type: "transactionMix",
            //     value: "TransactionMix",
            //     title: "Transaction Mix",
            // },
            // {
            //     type: "spendingTrend",
            //     value: "SpendingTrend",
            //     title: "Spending Trend",
            // },
            // {
            //     type: "newsAlerts",
            //     value: "NewsAlerts",
            //     title: "News Alerts",
            // },
            // {
            //     type: "transactionDetails",
            //     value: "TransactionDetails",
            //     title: "Transaction Details",
            // },
            // {
            //     type: "top4IncomeSources",
            //     value: "Top4IncomeSources",
            //     title: "Top 4 Income Sources",
            // }
        ]
    }

    changedOptions() {
        if (this.options.api && this.options.api.optionsChanged) {
            this.options.api.optionsChanged();
        }
    }

    removeItem($event, item) {
        const index: number = this.widgetsGridsterItemArray.indexOf(item);
        this.widgetsGridsterItemArray.splice(index, 1);
    }

    //Back Functionality.
    backClicked() {
        this._location.back();
    }

    OnSaveBtnClicked() {
        //console.log(this.widgetsGridsterItemArray);
        let pageObject: any = {};
        pageObject.DisplayName = this.PageName;
        pageObject.PageTypeId = this.PageTypeId;
        var currentUser = this.localstorageservice.GetCurrentUser();
        if(currentUser != null)
        {
            pageObject.PublishedBy = Number(currentUser.UserIdentifier);
            pageObject.UpdatedBy = Number(currentUser.UserIdentifier);
            pageObject.PageOwner = Number(currentUser.UserIdentifier);
        }
        
        let pageWidgets: any[] = [];
        for(var i=0; i < this.widgetsGridsterItemArray.length; i++) {
            let widgetsGridsterItem = this.widgetsGridsterItemArray[i];
            let pageWidget: any = {};
            pageWidget.WidgetId = widgetsGridsterItem.widgetId;
            pageWidget.Height = widgetsGridsterItem.rows;
            pageWidget.Width = widgetsGridsterItem.cols;
            pageWidget.Xposition = widgetsGridsterItem.x;
            pageWidget.Yposition = widgetsGridsterItem.y;
            pageWidget.WidgetSetting = JSON.stringify({});
            pageWidgets.push(pageWidget);
        }
        pageObject.PageWidgets = pageWidgets;
        console.log(pageObject);
        this.saveTemplate(pageObject);
    }

    //method written to save role
    async saveTemplate(pageObject) {
        this.uiLoader.start();
        let pageArray = [];
        pageArray.push(pageObject);
        let templateService = this.injector.get(TemplateService);
        let isRecordSaved = await templateService.saveTemplate(pageArray, this.pageEditModeOn);
        this.uiLoader.stop();
        if (isRecordSaved) {
            let message = Constants.recordAddedMessage;
            if (this.pageEditModeOn) {
                message = Constants.recordUpdatedMessage;
            }
            this._messageDialogService.openDialogBox('Success', message, Constants.msgBoxSuccess);
            this.navigateToListPage()
        }
    }

    //navigate to page list
    navigateToListPage() {
        const router = this.injector.get(Router);
        router.navigate(['pages']);
    }

    selectWidget(widgetType){
        this.widgetId = widgetType;
        if(widgetType == "customerInformation"){
            return this.widgetsGridsterItemArray.push({
                cols: 15,
                rows: 7,
                y: 0,
                x: 0,
                component: CustomerInformationComponent,
                value : 'customerInformation',
                widgetId : 1,
             })
        }
        else if(widgetType == "accountInformation"){
            return this.widgetsGridsterItemArray.push({
                cols: 5,
                rows: 7,
                y: 0,
                x: 0,
                component: AccountInformationComponent,
                value : 'accountInformation',
                widgetId : 2
             })
        }
        else if(widgetType == "image"){
            return this.widgetsGridsterItemArray.push({
                cols: 10,
                rows: 5,
                y: 0,
                x: 0,
                component: ImageComponent,
                value : 'image',
                widgetId : 3
             })
        }
        else if(widgetType == "video"){
            return this.widgetsGridsterItemArray.push({
                cols: 10,
                rows: 5,
                y: 0,
                x: 0,
                component: VideoComponent,
                value : 'video',
                widgetId : 4
             })
        }
        else if(widgetType == "summaryAtGlance"){
            return this.widgetsGridsterItemArray.push({
                cols: 15,
                rows: 6,
                y: 0,
                x: 0,
                component: SummaryAtGlanceComponent,
                value : 'summaryAtGlance',
                widgetId : 5
             })
        }
        // else if(widgetType == "reminderAndRecommendation"){
        //     return this.widgetsGridsterItemArray.push({
        //         cols: 10,
        //         rows: 5,
        //         y: 0,
        //         x: 0,
        //         //component: SingleColumnList,
        //      })
        // }
        // else if(widgetType == "investmentPortfolioRisk"){
        //     return this.widgetsGridsterItemArray.push({
        //         cols: 10,
        //         rows: 5,
        //         y: 0,
        //         x: 0,
        //         //component: MultipleColumnList,
        //      })
        // }
        // else if(widgetType == "analytics"){
        //     return this.widgetsGridsterItemArray.push({
        //         cols: 5,
        //         rows: 6,
        //         y: 0,
        //         x: 0,
        //         //component: ProcessControlGraphComponent,
        //      })
        // }
        // else if(widgetType == "availableBalance"){
        //     return this.widgetsGridsterItemArray.push({
        //         cols: 8,
        //         rows: 5,
        //         y: 0,
        //         x: 0,
        //         //component: ProcessControlGraphComponent,
        //      })
        // }
        // else if(widgetType == "transactionList"){
        //     return this.widgetsGridsterItemArray.push({
        //         cols: 20,
        //         rows: 6,
        //         y: 0,
        //         x: 0,
        //         //component: ProcessControlGraphComponent,
        //      })
        // }
        // else if(widgetType == "savingTrend"){
        //     return this.widgetsGridsterItemArray.push({
        //         cols: 7,
        //         rows: 5,
        //         y: 0,
        //         x: 0,
        //         //component: ProcessControlGraphComponent,
        //      })
        // }
        // else if(widgetType == "transactionMix"){
        //     return this.widgetsGridsterItemArray.push({
        //         cols: 6,
        //         rows: 5,
        //         y: 0,
        //         x: 0,
        //         //component: ProcessControlGraphComponent,
        //      })
        // }
        // else if(widgetType == "spendingTrend"){
        //     return this.widgetsGridsterItemArray.push({
        //         cols: 7,
        //         rows: 5,
        //         y: 0,
        //         x: 0,
        //         //component: ProcessControlGraphComponent,
        //      })
        // }
        // else if(widgetType == "newsAlerts"){
        //     return this.widgetsGridsterItemArray.push({
        //         cols: 20,
        //         rows: 5,
        //         y: 0,
        //         x: 0,
        //         //component: ProcessControlGraphComponent,
        //      })
        // }
        // else if(widgetType == "transactionDetails"){
        //     return this.widgetsGridsterItemArray.push({
        //         cols: 20,
        //         rows: 5,
        //         y: 0,
        //         x: 0,
        //         //component: ProcessControlGraphComponent,
        //      })
        // }
        // else if(widgetType == "top4IncomeSources"){
        //     return this.widgetsGridsterItemArray.push({
        //         cols: 5,
        //         rows: 5,
        //         y: 0,
        //         x: 0,
        //         //component: ProcessControlGraphComponent,
        //      })
        // }
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
        this.templateList = await templateService.getTemplates(searchParameter);
        if (this.templateList.length == 0) {
            let message = "NO record found";
            this._messageDialogService.openDialogBox('Error', message, Constants.msgBoxError).subscribe(data => {
                if (data == true) {
                    this.getPageRecord();
                }
            });
        }else {
            let template = this.templateList[0];
            let pageWidgets: TemplateWidget[] = template.PageWidgets;
            if(pageWidgets.length != 0) {
                for(let i=0; i < pageWidgets.length; i++) {
                    let gridsterItem: any = {};
                    gridsterItem.x = pageWidgets[i].Xposition;
                    gridsterItem.y = pageWidgets[i].Yposition;
                    gridsterItem.cols = pageWidgets[i].Width;
                    gridsterItem.rows = pageWidgets[i].Height;
                    gridsterItem.widgetId = pageWidgets[i].WidgetId;
                    gridsterItem.WidgetSetting = pageWidgets[i].WidgetSetting;
                    if(pageWidgets[i].WidgetId == 1) {
                        gridsterItem.component = CustomerInformationComponent;
                        gridsterItem.value = 'customerInformation';
                    }else if(pageWidgets[i].WidgetId == 2) {
                        gridsterItem.component = AccountInformationComponent;
                        gridsterItem.value = 'accountInformation';
                    }else if(pageWidgets[i].WidgetId == 3) {
                        gridsterItem.component = ImageComponent;
                        gridsterItem.value = 'image';
                    }else if(pageWidgets[i].WidgetId == 4) {
                        gridsterItem.component = VideoComponent;
                        gridsterItem.value = 'video';
                    }else if(pageWidgets[i].WidgetId == 5) {
                        gridsterItem.component = SummaryAtGlanceComponent;
                        gridsterItem.value = 'summaryAtGlance';
                    }
                    this.widgetsGridsterItemArray.push(gridsterItem);
                }
            }
        }
    }
}
