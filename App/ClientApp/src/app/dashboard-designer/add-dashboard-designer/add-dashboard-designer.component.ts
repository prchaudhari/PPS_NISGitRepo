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
import { AssetLibraryService } from '../../layout/asset-libraries/asset-library.service';
import { AssetLibrary, Asset, AssetLibrarySearchParameter, AssetSearchParameter } from '../../layout/asset-libraries/asset-library';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { ConfigConstants } from 'src/app/shared/constants/configConstants';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';

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
    public isMasterSaveBtnDisabled: boolean = false;

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
    public baseURL: string = ConfigConstants.BaseURL;
    public validUrlRegexPattern = '(https?://)?([\\da-z.-]+)\\.([a-z.]{2,6})[/\\w .-]*/?';

    public templateList: Template[] = [];
    public assetLibraryList: any[] = [{ 'Identifier': '0', 'Name': 'Select Asset Library' }];
    public assets: any[] = [{ 'Identifier': '0', 'Name': 'Select Asset' }];
    public ImageConfigForm: FormGroup;
    public imgAssetLibraryId: number = 0;
    public imgAssetLibraryName: string = "";
    public imgAssetId: number = 0;
    public imgAssetName: string = "";
    public imageSourceUrl:  string;

    public VideoConfigForm: FormGroup;
    public vdoAssetLibraryId: number = 0;
    public vdoAssetLibraryName: string = "";
    public vdoAssetId: number = 0;
    public vdoAssetName: string = "";
    public vdoSourceUrl:  string;
    public imageWidgetId: number = 0;
    public videoWidgetId: number = 0;
    public widgetItemCount: number = 0;
    public selectedWidgetItemCount: number = 0;
    public pageVersion: string;

    constructor(private _location: Location,
      private injector: Injector,
      private fb: FormBuilder,
      private _dialogService: DialogService,
      private uiLoader: NgxUiLoaderService,
      private _messageDialogService: MessageDialogService,
      private localstorageservice: LocalStorageService,
      private assetLibraryService: AssetLibraryService,
      private _http: HttpClient,
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

    public imageFormErrorObject: any = {
        showAssetLibraryError: false,
        showAssetError: false
    };

    public videoFormErrorObject: any = {
        showAssetLibraryError: false,
        showAssetError: false
    }
    
    //Getters for Image config Forms
    get imgAssetLibrary() {
        return this.ImageConfigForm.get('imgAssetLibrary');
    }
    get imgAsset() {
        return this.ImageConfigForm.get('imgAsset');
    }
    get imageUrl() {
        return this.ImageConfigForm.get('imageUrl');
    }

    //Getters for Image config Forms
    get vdoAssetLibrary() {
        return this.VideoConfigForm.get('vdoAssetLibrary');
    }
    get vdoAsset() {
        return this.VideoConfigForm.get('vdoAsset');
    }
    get vdoUrl() {
        return this.VideoConfigForm.get('vdoUrl');
    }

    get imgForm(){
        return this.ImageConfigForm.controls;
    }

    get vdoForm(){
        return this.VideoConfigForm.controls;
    }

    saveImgFormValidation(): boolean {
        if (this.ImageConfigForm.controls.imageUrl.invalid && !this.isPersonalizeImage) {
            return true;
        }
        if (this.imgAssetLibraryId == 0 && !this.isPersonalizeImage) {
            return true;
        }
        if (this.imgAssetId == 0 && !this.isPersonalizeImage) {
            return true;
        }
        return false; 
    }

    saveVideoFormValidation(): boolean {
        if (this.VideoConfigForm.controls.vdoUrl.invalid && ((this.isPersonalize && this.isEmbedded) || this.isEmbedded)) {
            return true;
        }
        if (this.vdoAssetLibraryId == 0 && (!this.isPersonalize && !this.isEmbedded)) {
            return true;
        }
        if (this.vdoAssetId == 0 && (!this.isPersonalize && !this.isEmbedded)) {
            return true;
        }
        return false; 
    }

    isImageConfigForm(widgetId, widgetItemCount) {
        this.isMasterSaveBtnDisabled = true;
        this.isImageConfig = true;
        this.imageWidgetId = widgetId;
        this.selectedWidgetItemCount = widgetItemCount;
        this.isPersonalizeImage = false;

        var records = this.widgetsGridsterItemArray.filter(x => x.widgetId == this.imageWidgetId && x.widgetItemCount == this.selectedWidgetItemCount);
        if(records.length != 0) {
            var widgetSetting = records[0].WidgetSetting;
            if(widgetSetting != null && widgetSetting != '' && this.testJSON(widgetSetting)) {
                var widgetConfigObj = JSON.parse(widgetSetting);
                this.ImageConfigForm.patchValue({
                    imgAssetLibrary: widgetConfigObj.AssetLibraryId,
                    imgAsset: widgetConfigObj.AssetId,
                    imageUrl: widgetConfigObj.SourceUrl,
                });
                this.imgAssetId = widgetConfigObj.AssetId;
                this.imgAssetName = widgetConfigObj.AssetName;
                this.imgAssetLibraryId = widgetConfigObj.AssetLibraryId;
                this.imgAssetLibraryName = widgetConfigObj.AssetLibrayName;

                if(widgetConfigObj.isPersonalize != null) {
                    this.isPersonalizeImage = widgetConfigObj.isPersonalize;
                }
                if(widgetConfigObj.AssetLibraryId != null && widgetConfigObj.AssetLibraryId != 0) {
                    this.LoadAsset('image', widgetConfigObj.AssetLibraryId);
                }
            }else {
                this.ImageConfigForm.patchValue({
                    imgAssetLibrary: 0,
                    imgAsset: 0,
                    imageUrl: ''
                });           
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
        this.isMasterSaveBtnDisabled = true;
        this.isVideoConfig = true;
        this.videoWidgetId = widgetId;
        this.selectedWidgetItemCount = widgetItemCount;
        this.isPersonalize = false;
        this.isEmbedded = false;
        
        var records = this.widgetsGridsterItemArray.filter(x => x.widgetId == this.videoWidgetId && x.widgetItemCount == this.selectedWidgetItemCount);
        if(records.length != 0) {
            var widgetSetting = records[0].WidgetSetting;
            if(widgetSetting != null && widgetSetting != '' && this.testJSON(widgetSetting)) {
                var widgetConfigObj = JSON.parse(widgetSetting);
                this.VideoConfigForm.patchValue({
                    vdoAssetLibrary: widgetConfigObj.AssetLibraryId,
                    vdoAsset: widgetConfigObj.AssetId,
                    vdoUrl: widgetConfigObj.SourceUrl,
                });
                this.vdoAssetId = widgetConfigObj.AssetId;
                this.vdoAssetName = widgetConfigObj.AssetName;
                this.vdoAssetLibraryId = widgetConfigObj.AssetLibraryId;
                this.vdoAssetLibraryName = widgetConfigObj.AssetLibrayName;

                if(widgetConfigObj.isPersonalize != null) {
                    this.isPersonalize = widgetConfigObj.isPersonalize;
                }
                if(widgetConfigObj.isEmbedded != null) {
                    this.isEmbedded = widgetConfigObj.isEmbedded;
                }
                if(widgetConfigObj.AssetLibraryId != null && widgetConfigObj.AssetLibraryId != 0) {
                    this.LoadAsset('video', widgetConfigObj.AssetLibraryId);
                }
            }else {
                this.VideoConfigForm.patchValue({
                    vdoAssetLibrary: 0,
                    vdoAsset: 0,
                    vdoUrl: ''
                });
            }
        }
    }

    ngOnInit() {

        $(document).ready(function () {
            $('.widget-delete-btn').click(function () {
                $(this).parent().parent().parent().parent().addClass('hide');
            });
        });

        this.ImageConfigForm = this.fb.group({
            imgAssetLibrary: [null, [Validators.required]],
            imgAsset: [null, [Validators.required]],
            imageUrl: [null, [Validators.required, Validators.pattern(this.validUrlRegexPattern)]]
        });

        this.VideoConfigForm = this.fb.group({
            vdoAssetLibrary: [null, [Validators.required]],
            vdoAsset: [null, [Validators.required]],
            vdoUrl: [null, [Validators.required, Validators.pattern(this.validUrlRegexPattern)]]
        });

        this.getAssetLibraries();
        if(this.pageEditModeOn) {
            this.getTemplate();
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
            }
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
        this.navigateToListPage();
    }

    prevBtnClicked() {
        let queryParams = {
            Routeparams: {
              passingparams: {
                "PageName": this.PageName,
                "PageTypeId": this.PageTypeId,
              }
            }
          }
          localStorage.setItem("pageAddRouteparams", JSON.stringify(queryParams))
        const router = this.injector.get(Router);
        router.navigate(['pages', 'Add']);
    }

    OnSaveBtnClicked() {
        let pageObject: any = {};
        pageObject.DisplayName = this.PageName;
        pageObject.PageTypeId = this.PageTypeId;
        pageObject.Identifier = this.PageIdentifier;
        if(this.pageEditModeOn) {
            pageObject.Version = this.pageVersion;
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
            pageWidget.WidgetSetting = widgetsGridsterItem.WidgetSetting != null ? widgetsGridsterItem.WidgetSetting : "";
            pageWidgets.push(pageWidget);
        }
        pageObject.PageWidgets = pageWidgets;
        this.saveTemplate(pageObject);
    }

    //method written to save tempalte
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
        this.widgetItemCount = this.widgetItemCount + 1;
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
                widgetItemCount: this.widgetItemCount,
                WidgetSetting: ''
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
                widgetId : 2,
                widgetItemCount: this.widgetItemCount,
                WidgetSetting: ''
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
                widgetId : 3,
                widgetItemCount: this.widgetItemCount,
                WidgetSetting: ''
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
                widgetId : 4,
                widgetItemCount: this.widgetItemCount,
                WidgetSetting: ''
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
                widgetId : 5,
                widgetItemCount: this.widgetItemCount,
                WidgetSetting: ''
             })
        }
    }

    getTemplate() {
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
        this.getPageRecords(searchParameter);
    }

    async getPageRecords(searchParameter) {
        let templateService = this.injector.get(TemplateService);
        this.templateList = await templateService.getTemplates(searchParameter);
        if (this.templateList.length == 0) {
            let message = "No record found";
            this._messageDialogService.openDialogBox('Error', message, Constants.msgBoxError).subscribe(data => {
                if (data == true) {
                    this.getTemplate();
                }
            });
        }else {
            let template = this.templateList[0];
            this.PageName = template.DisplayName;
            this.PageTypeId = template.PageTypeId;
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

    getAssetLibraries () {
        let searchParameter: any = {};
        searchParameter.IsActive = true;
        searchParameter.PagingParameter = {};
        searchParameter.PagingParameter.PageIndex = Constants.DefaultPageIndex;
        searchParameter.PagingParameter.PageSize = Constants.DefaultPageSize;
        searchParameter.SortParameter = {};
        searchParameter.SortParameter.SortColumn = Constants.Name;
        searchParameter.SortParameter.SortOrder = Constants.Ascending;
        searchParameter.SearchMode = Constants.Contains;
        this.getAssetLibraryRecords(searchParameter);
    }

    async getAssetLibraryRecords(searchParameter) {
        let assetLibraryService = this.injector.get(AssetLibraryService);       
        let records = await assetLibraryService.getAssetLibrary(searchParameter);
        if (records.length == 0) {
            let message = "NO record found";
            this._messageDialogService.openDialogBox('Error', message, Constants.msgBoxError).subscribe(data => {
                if (data == true) {
                }
            });
        }else{
            this.assetLibraryList.push(...records);
        }
    }

    onAssetLibrarySelected(event, type) {
        const value = event.target.value;
        this.imgAssetId = 0;
        this.vdoAssetId = 0;
        if (value == "0") {
            if(type == 'image') {
                this.imgAssetLibraryId = 0;
                this.imgAssetLibraryName = '';
                this.imageFormErrorObject.showAssetLibraryError = true;
            }else {
                this.vdoAssetLibraryId = 0;
                this.vdoAssetLibraryName = '';
                this.videoFormErrorObject.showAssetLibraryError = true;
            }
        }
        else {           
            if(type == 'image'){
                this.imgAssetLibraryId = Number(value);
                this.imgAssetLibraryName = this.assetLibraryList.filter(x => x.Identifier == Number( event.target.value))[0].Name;
                this.imageFormErrorObject.showAssetLibraryError = false;   
            }else {
                this.vdoAssetLibraryId = Number(value);
                this.vdoAssetLibraryName = this.assetLibraryList.filter(x => x.Identifier == Number( event.target.value))[0].Name;
                this.videoFormErrorObject.showAssetLibraryError = false;
            }
            this.LoadAsset(type, value);
        }
    }

    onAssetSelected(event, type) {
        const value = event.target.value;
        if (value == "0") {
            if(type == 'image'){
                this.imgAssetId = 0;
                this.imgAssetName = '';
                this.imageFormErrorObject.showAssetError = true;
            }else {
                this.vdoAssetId = 0;
                this.vdoAssetName = '';
                this.videoFormErrorObject.showAssetError = true;
            }
        }
        else {
            if(type == 'image'){
                this.imgAssetId = Number(value);
                this.imgAssetName = this.assets.filter(x => x.Identifier == Number( event.target.value))[0].Name;
                this.imageFormErrorObject.showAssetError = false;
            }else {
                this.vdoAssetId = Number(value);
                this.vdoAssetName = this.assets.filter(x => x.Identifier == Number( event.target.value))[0].Name;
                this.videoFormErrorObject.showAssetError = false;
            }
        }
    }

    LoadAsset(type, value): void {
        this.assets = [];
        this.assets.push({ 'Identifier': '0', 'Name': 'Select Asset' });

        let assetSearchParameter: AssetSearchParameter = new AssetSearchParameter();
        assetSearchParameter.AssetLibraryIdentifier = String(value);
        assetSearchParameter.IsDeleted = false;           
        if(type == 'image'){
            assetSearchParameter.Extension = "jpg, png, jpeg";
            if(this.ImageConfigForm.controls.imgAsset == null || this.ImageConfigForm.controls.imgAsset.value == 0){
                this.ImageConfigForm.patchValue({
                    imgAsset: 0,
                });
            }
        }else {
            assetSearchParameter.Extension = "mp4";
            if(this.VideoConfigForm.controls.vdoAsset == null || this.VideoConfigForm.controls.vdoAsset.value == 0){
                this.VideoConfigForm.patchValue({
                    vdoAsset: 0,
                });
            }
        }
        assetSearchParameter.PagingParameter.PageIndex = Constants.DefaultPageIndex;
        assetSearchParameter.PagingParameter.PageSize = Constants.DefaultPageSize;
        assetSearchParameter.SortParameter.SortColumn = Constants.Name;
        assetSearchParameter.SortParameter.SortOrder = Constants.Ascending;
        assetSearchParameter.SearchMode = Constants.Contains;
        let assets: any[];
        this.uiLoader.start();
        this._http.post(this.baseURL + 'assetlibrary/asset/list', assetSearchParameter).subscribe(
            data => {
                this.uiLoader.stop();
                assets = <any[]>data;    
                this.assets.push(...assets);
            },
            error => {
                this.uiLoader.stop();
                this._messageDialogService.openDialogBox('Error', error.error.Message, Constants.msgBoxError);
            }
        );
    }

    OnImageConfigBtnClicked(actionFor) {
        if(actionFor == 'submit') {
            let imageConfig: any = {};
            imageConfig.AssetLibraryId = this.isPersonalizeImage == true ? 0 : this.imgAssetLibraryId;
            imageConfig.AssetLibrayName = this.isPersonalizeImage == true ? 0 : this.imgAssetLibraryName;
            imageConfig.AssetId = this.isPersonalizeImage == true ? 0 : this.imgAssetId;
            imageConfig.AssetName = this.isPersonalizeImage == true ? 0 : this.imgAssetName;
            imageConfig.SourceUrl = this.isPersonalizeImage == true ? "" : this.ImageConfigForm.value.imageUrl;
            imageConfig.isPersonalize = this.isPersonalizeImage;
            imageConfig.WidgetId = this.imageWidgetId;
            
            this.widgetsGridsterItemArray.filter(x => x.widgetId == this.imageWidgetId && x.widgetItemCount == this.selectedWidgetItemCount)[0].WidgetSetting = JSON.stringify(imageConfig);
            //console.log(this.widgetsGridsterItemArray);
        }
        this.resetImageConfigForm();
        this.isImageConfig = !this.isImageConfig;
        this.imageWidgetId = 0;
        this.selectedWidgetItemCount = 0;
        this.isMasterSaveBtnDisabled = false;
    }

    OnVideoConfigBtnClicked(actionFor) {
        if(actionFor == 'submit') { 
            let videoConfig: any = {};
            videoConfig.AssetLibraryId = this.isPersonalize == true ? 0 : this.vdoAssetLibraryId;
            videoConfig.AssetLibrayName = this.isPersonalize == true ? 0 : this.vdoAssetLibraryName;
            videoConfig.AssetId =  this.isPersonalize == true ? 0 : this.vdoAssetId;
            videoConfig.AssetName = this.isPersonalize == true ? 0 : this.vdoAssetName;
            videoConfig.SourceUrl = (this.isPersonalize == true && this.isEmbedded == true) || (this.isEmbedded == true) ? this.VideoConfigForm.value.vdoUrl : "";
            videoConfig.WidgetId = this.videoWidgetId;
            videoConfig.isPersonalize = this.isPersonalize;
            videoConfig.isEmbedded = this.isEmbedded;
            this.widgetsGridsterItemArray.filter(x => x.widgetId == this.videoWidgetId  && x.widgetItemCount == this.selectedWidgetItemCount)[0].WidgetSetting = JSON.stringify(videoConfig);
            //console.log(this.widgetsGridsterItemArray);
        }
        this.resetVideoConfigForm();
        this.isVideoConfig = !this.isVideoConfig;
        this.videoWidgetId = 0;
        this.selectedWidgetItemCount = 0;
        this.isMasterSaveBtnDisabled = false;
    }

    resetImageConfigForm() {
        this.ImageConfigForm.patchValue({
            imgAssetLibrary: null,
            imgAsset: null,
            imageUrl: null,
        });
    }

    resetVideoConfigForm() {
        this.VideoConfigForm.patchValue({
            vdoAssetLibrary: null,
            vdoAsset: null,
            vdoUrl: null,
        });
    }
}
