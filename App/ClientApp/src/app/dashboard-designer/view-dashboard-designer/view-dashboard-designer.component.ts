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
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { CustomerInformationComponent, AccountInformationComponent, ImageComponent, VideoComponent, SummaryAtGlanceComponent } from '../widgetComponent/widgetComponent';
import { FormGroup, FormBuilder } from '@angular/forms';

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
    public baseURL: string = environment.baseURL;

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

    //Back Functionality.
    backClicked() {
        this._location.back();
    }

    constructor(private _location: Location,
        private fb: FormBuilder,
        private injector: Injector,
        private _messageDialogService: MessageDialogService,
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
        var records = this.widgetsGridsterItemArray.filter(x => x.widgetId == this.imageWidgetId && x.widgetItemCount == this.selectedWidgetItemCount);
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
        var records = this.widgetsGridsterItemArray.filter(x => x.widgetId == this.videoWidgetId && x.widgetItemCount == this.selectedWidgetItemCount);
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

        this.getPageRecord();
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
            this.PageName = template.DisplayName;
            this.PageIdentifier = template.Identifier;
            this.pageVersion = template.Version;
            let pageWidgets: TemplateWidget[] = template.PageWidgets;
            if(pageWidgets.length != 0) {
                for(let i=0; i < pageWidgets.length; i++) {
                    this.widgetItemCount = this.widgetItemCount + 1;
                    let gridsterItem: any = {};
                    gridsterItem.x = pageWidgets[i].Xposition;
                    gridsterItem.y = pageWidgets[i].Yposition;
                    gridsterItem.cols = pageWidgets[i].Width;
                    gridsterItem.rows = pageWidgets[i].Height;
                    gridsterItem.widgetId = pageWidgets[i].WidgetId;
                    gridsterItem.WidgetSetting = pageWidgets[i].WidgetSetting;
                    gridsterItem.widgetItemCount = this.widgetItemCount;                    
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
