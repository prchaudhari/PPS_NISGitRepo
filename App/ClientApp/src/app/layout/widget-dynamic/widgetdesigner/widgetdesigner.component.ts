import { Component, OnInit, Injector, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';
import { NgxUiLoaderService } from 'ngx-ui-loader';
import { Router, NavigationEnd } from '@angular/router';
import { Constants, ErrorMessageConstants } from '../../../shared/constants/constants';
import { MessageDialogService } from '../../../shared/services/mesage-dialog.service';
import { LocalStorageService } from '../../../shared/services/local-storage.service';
import { DynamicWidgetService } from '../dynamicwidget.service';
import { DynamicWidget } from '../dynamicwidget';
import { CdkDragDrop, moveItemInArray } from '@angular/cdk/drag-drop';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { AngularEditorConfig } from '@kolkov/angular-editor';
import { ToolbarService, LinkService, ImageService, HtmlEditorService } from '@syncfusion/ej2-angular-richtexteditor';
import { TemplateService } from '../../template/template.service';
import { ConfigConstants } from '../../../shared/constants/configConstants';
import { RichTextEditorComponent, MarkdownFormatter, EditorMode, RichTextEditor } from '@syncfusion/ej2-angular-richtexteditor';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { AssetLibraryService } from '../../asset-libraries/asset-library.service';
import { DomSanitizer } from '@angular/platform-browser';
@Component({
  selector: 'app-widgetdesigner',
  templateUrl: './widgetdesigner.component.html',
  styleUrls: ['./widgetdesigner.component.scss'],
  providers: [ToolbarService, LinkService, ImageService, HtmlEditorService]
})
export class WidgetdesignerComponent implements OnInit {
  @ViewChild('htmleditor', { static: false }) rteObj: RichTextEditorComponent;
  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;

  //html editor code
  htmlContent = '';
  config: AngularEditorConfig = {
    editable: true,
    spellcheck: true,
    height: '15rem',
    minHeight: '5rem',
    placeholder: 'Enter text here...',
    translate: 'no',
    defaultParagraphSeparator: 'p',
    defaultFontName: 'Arial',
    toolbarHiddenButtons: [
      //['bold']
    ],
    customClasses: [
      {
        name: "quote",
        class: "quote",
      },
      {
        name: 'redText',
        class: 'redText'
      },
      {
        name: "titleText",
        class: "titleText",
        tag: "h1",
      },
    ]
  };
  public isDefault: boolean = true;
  public isCustome: boolean = false;
  public dynamicWidgetDetails: DynamicWidget;
  public params;
  public userClaimsRolePrivilegeOperations: any[] = [];
  isDefaultClicked() {
    this.isDefault = true;
    this.isCustome = false;
  }
  isCustomeClicked() {
    this.isDefault = false;
    this.isCustome = true;
  }
  public filterConditions: any[] = [];

  public isTheme1Active: boolean = false;
  public isTheme2Active: boolean = false;
  public isTheme3Active: boolean = false;
  public isTheme4Active: boolean = false;
  public isTheme5Active: boolean = false;
  public isTheme0Active: boolean = true;
  //Functions call to click the theme of the page--
  public inlineMode: object = { enable: false, onSelection: false };
  public format: Object = {
    width: 'auto',

  };
  public pageSize = 5;
  public currentPage = 0;
  public totalSize = 0;
  public DynamicWidgetForm: FormGroup;
  public pageTypeList: any[] = [{ "PageTypeName": "Select Page Type", "Identifier": 0 }];
  public entityList: any[] = [{ "Name": "Select Entity", "Identifier": 0 }];
  public entityFieldList: any[] = [{ "Name": "Select", "Identifier": 0 }];
  displayedColumns: string[] = ['series', 'displayName', 'actions'];
  public lineBarGraphList: any[] = [];
  public widgetFilterlist: any[] = [];
  public displayWidgetFilterlist: any[] = [];
  public WidgetName;
  public WidgetType;
  public PageTypeId;
  public EntityId;
  public Title;
  public PageTypeName;
  public DynamicWidgetIdentifier;
  public EntityName;
  public conditionList: any[] = [
    { "Name": "Select", "Identifier": "0" },
    { "Name": "Equals To", "Identifier": "EqualsTo" },
    { "Name": "Not Equals To", "Identifier": "NotEqualsTo" },
    { "Name": "Less Than", "Identifier": "LessThan" },
    { "Name": "Greater Than", "Identifier": "GreaterThan" },
    { "Name": "Contains", "Identifier": "Contains" },
    { "Name": "Not Contains", "Identifier": "NotContains" }

  ];
  public fontFamilyList: any[] = [
    { "Name": "Select", "Identifier": "0" },
    { "Name": "Serif", "Identifier": "Serif" },
    { "Name": "Sans-serif", "Identifier": "Sans-serif" },
    { "Name": "Monospace", "Identifier": "Monospace" },

  ];
  public updateOperationMode: boolean;
  dataSource = new MatTableDataSource<any>(this.lineBarGraphList);
  public assetLibraryList: any[] = [{ 'Identifier': '0', 'Name': 'Select Asset Library' }];
  public assets: any[] = [{ 'Identifier': '0', 'Name': 'Select Asset' }];
  public baseURL = ConfigConstants.BaseURL;
  public pieChartSeriesEntityFields: any[] = [{ "Name": "Select", "Identifier": 0 }];
  public pieChartValueEntityFields: any[] = [{ "Name": "Select", "Identifier": 0 }];
  public lineBarGraphFields: any[] = [{ "Name": "Select", "Identifier": 0 }];

  
  ngAfterViewInit() {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
  }
  public handlePage(e: any) {
    this.currentPage = e.pageIndex;
    this.pageSize = e.pageSize;
  }

  tableHeader = [];

  formList = [];

  drop(event: CdkDragDrop<string[]>) {
    moveItemInArray(this.tableHeader, event.previousIndex, event.currentIndex);
    moveItemInArray(this.formList, event.previousIndex, event.currentIndex);
  }

  //select widget type radio

  selectedLink: string = "Form";
  selectedTheme: string = "Theme1";

  setWidgetType(e: string): void {
    //  this.selectRowsOption = '';
    this.selectedLink = e;
  }

  selectTheme(theme) {
    this.selectedTheme = theme;
  }

  isSelected(name: string): boolean {
    if (!this.selectedLink) { // if no radio button is selected, always return false so every nothing is shown  
      return false;
    }
    return (this.selectedLink === name); // if current radio button is selected, return true, else return false 
  }

  navigateToListPage() {
    this._router.navigate(['dynamicwidget']);
  }

  constructor(
    private _router: Router,
    private fb: FormBuilder,
    private injector: Injector,
    private uiLoader: NgxUiLoaderService,
    private _messageDialogService: MessageDialogService,
    private _http: HttpClient,
    private dynamicWidgetService: DynamicWidgetService,
    private sanitizer: DomSanitizer,
    private localstorageservice: LocalStorageService
  ) {
    this.dynamicWidgetDetails = new DynamicWidget;
    _router.events.subscribe(e => {
      if (e instanceof NavigationEnd) {
        if (e.url.includes('/dynamicwidget')) {
          if (localStorage.getItem('widgetDesignerParams')) {
            this.params = JSON.parse(localStorage.getItem('widgetDesignerParams'));
            this.WidgetName = this.params.Routeparams.passingparams.WidgetName;
            this.WidgetType = this.params.Routeparams.passingparams.WidgetType;
            this.PageTypeId = this.params.Routeparams.passingparams.PageTypeId;
            this.PageTypeName = this.params.Routeparams.passingparams.PageTypeName;
            this.EntityId = this.params.Routeparams.passingparams.EntityId;
            this.Title = this.params.Routeparams.passingparams.Title;
            this.EntityName = this.params.Routeparams.passingparams.EntityName;
            this.updateOperationMode = this.params.Routeparams.passingparams.updateOperationMode;
            this.DynamicWidgetIdentifier = this.params.Routeparams.passingparams.DynamicWidgetIdentifier;
            this.dynamicWidgetDetails.WidgetName = this.params.Routeparams.passingparams.WidgetName;
            this.dynamicWidgetDetails.WidgetType = this.params.Routeparams.passingparams.WidgetType;
            this.dynamicWidgetDetails.PageTypeId = this.params.Routeparams.passingparams.PageTypeId;
            this.dynamicWidgetDetails.EntityId = this.params.Routeparams.passingparams.EntityId;
            this.dynamicWidgetDetails.Title = this.params.Routeparams.passingparams.Title;
            this.selectedLink = this.dynamicWidgetDetails.WidgetType;
            this.getEntityField(this.EntityId);
            this.getAssetLibraries();
          }
        } else {
          localStorage.removeItem("widgetDesignerParams");
          localStorage.removeItem("widgetDesignerParams");
        }
      }
    });
  }

  ngOnInit() {
    this.DynamicWidgetForm = this.fb.group({
      FormEntityField: [0],
      FormFieldDisplayName: [null],
      TableHeaderName: [null],
      TableEntityField: [0],
      TableIsSorting: [false],
      LineBarEntityField: [0],
      LineBarFieldDisplayName: [null],
      LineBarXAxis: [0],
      PieSeries: [0],
      PieValue: [0],
      FilterConditionOperator: ["0"],
      FilterField: [0],
      FilterOperator: [0],
      FilterValue: [null],
      TitleColor: [null],
      TitleSize: [null],
      TitleWeight: [0],
      TitleType: [0],
      HeaderColor: [null],
      HeaderSize: [null],
      HeaderWeight: [0],
      HeaderType: [0],
      DataColor: [null],
      DataSize: [null],
      DataWeight: [0],
      DataType: [0],
      HTMLEntityField: [0],
      HTMLAsset: [0],
      HTMLAssetLibrary: [0]
    });
    if (this.updateOperationMode) {
      this.dynamicWidgetDetails.Identifier = this.params.Routeparams.passingparams.DynamicWidgetIdentifier;
      this.getWidgetDetails();
    }
  }

  async getWidgetDetails() {
    let searchParameter: any = {};
    searchParameter.PagingParameter = {};
    searchParameter.PagingParameter.PageIndex = Constants.DefaultPageIndex;
    searchParameter.PagingParameter.PageSize = Constants.DefaultPageSize;
    searchParameter.SortParameter = {};
    searchParameter.SortParameter.SortColumn = "Id";
    searchParameter.SortParameter.SortOrder = Constants.Ascending;
    searchParameter.SearchMode = Constants.Exact;
    searchParameter.Identifier = this.dynamicWidgetDetails.Identifier;
    searchParameter.IsStatementPagesRequired = true;
    var response = await this.dynamicWidgetService.getDynamicWidgets(searchParameter);
    this.dynamicWidgetDetails = response.List[0];
    this.setWidgetType(this.dynamicWidgetDetails.WidgetType);
    var themeCSS: any = {};
    // this.getEntityField(this.dynamicWidgetDetails.EntityId);
    if (this.dynamicWidgetDetails.ThemeCSS != null && this.dynamicWidgetDetails.ThemeCSS != '') {
      themeCSS = JSON.parse(this.dynamicWidgetDetails.ThemeCSS);
      this.DynamicWidgetForm.patchValue({
        WidgetName: this.dynamicWidgetDetails.WidgetName,
        PageType: this.dynamicWidgetDetails.PageTypeId,
        Entity: this.dynamicWidgetDetails.EntityId,
        WidgetTitle: this.dynamicWidgetDetails.Title,
        TitleColor: themeCSS.TitleColor,
        TitleSize: themeCSS.TitleSize,
        TitleWeight: themeCSS.TitleWeight,
        TitleType: themeCSS.TitleType,
        HeaderColor: themeCSS.HeaderColor,
        HeaderSize: themeCSS.HeaderSize,
        HeaderWeight: themeCSS.HeaderWeight,
        HeaderType: themeCSS.HeaderType,
        DataColor: themeCSS.DataColor,
        DataSize: themeCSS.DataSize,
        DataWeight: themeCSS.DataWeight,
        DataType: themeCSS.DataType
      });
    }
    this.isDefault = this.dynamicWidgetDetails.ThemeType == "Default" ? true : false;

    this.isCustome = this.dynamicWidgetDetails.ThemeType == "Default" ? false : true;

    if (this.dynamicWidgetDetails.WidgetFilterSettings != null && this.dynamicWidgetDetails.WidgetFilterSettings != '') {
      this.widgetFilterlist = JSON.parse(this.dynamicWidgetDetails.WidgetFilterSettings);
      this.widgetFilterlist.forEach(item => {
        var object = item;
        object.OperatorName = this.conditionList.filter(i => i.Identifier == item.Operator)[0].Name;
        object.FieldName = this.entityFieldList.filter(i => i.Identifier == item.FieldId)[0].Name;
        this.displayWidgetFilterlist.push(object);
      });
    }
    if (this.dynamicWidgetDetails.WidgetSettings != null && this.dynamicWidgetDetails.WidgetSettings != '') {

      var settings;
      if (this.dynamicWidgetDetails.WidgetType != 'Html') {
        settings = JSON.parse(this.dynamicWidgetDetails.WidgetSettings);
      }

      if (this.dynamicWidgetDetails.WidgetType == 'Form') {
        this.formList = settings;
      }
      else if (this.dynamicWidgetDetails.WidgetType == 'Table') {
        this.tableHeader = settings;
      }
      else if (this.dynamicWidgetDetails.WidgetType == 'PieChart') {
        this.selectedTheme = this.isCustome ? themeCSS.ChartColorTheme : this.selectedTheme;
        this.DynamicWidgetForm.patchValue({
          PieSeries: settings.PieSeries,
          PieValue: settings.PieValue
        });
      }
      else if (this.dynamicWidgetDetails.WidgetType == 'LineGraph' || this.dynamicWidgetDetails.WidgetType == 'BarGraph') {
        this.selectedTheme = this.isCustome ? themeCSS.ChartColorTheme : this.selectedTheme;
        this.lineBarGraphList = settings.Details;
        this.dataSource = new MatTableDataSource<any>(this.lineBarGraphList);
        this.DynamicWidgetForm.patchValue({
          LineBarXAxis: settings.XAxis,
        });
      }
      else if (this.dynamicWidgetDetails.WidgetType == 'Html') {
        this.rteObj.executeCommand('insertHTML', this.dynamicWidgetDetails.WidgetSettings);

      }

    }

    this.DynamicWidgetForm.patchValue({
      WidgetName: this.dynamicWidgetDetails.WidgetName,
      PageType: this.dynamicWidgetDetails.PageTypeId,
      Entity: this.dynamicWidgetDetails.EntityId,
      WidgetTitle: this.dynamicWidgetDetails.Title,
    });

  }

  async getPageTypes() {
    let dynamicWidgetService = this.injector.get(TemplateService);
    var data = await dynamicWidgetService.getPageTypes();
    data.forEach(item => {
      this.pageTypeList.push(item);
    })
    if (this.pageTypeList.length == 0) {
      let message = ErrorMessageConstants.getNoRecordFoundMessage;
      this._messageDialogService.openDialogBox('Error', message, Constants.msgBoxError).subscribe(data => {
        if (data == true) {
          this.getPageTypes();
        }
      });
    }
  }

  async getEntities() {

    var data = await this.dynamicWidgetService.getEntities();
    data.forEach(item => {
      this.entityList.push(item);
    })
    if (this.entityList.length == 0) {
      let message = ErrorMessageConstants.getNoRecordFoundMessage;
      this._messageDialogService.openDialogBox('Error', message, Constants.msgBoxError).subscribe(data => {
      });
    }

  }

  async getAssetLibraries() {
    let assetLibraryService = this.injector.get(AssetLibraryService);
    var searchParameter: any = {};
    searchParameter.SortParameter = {};
    searchParameter.SortParameter.SortColumn = "Id";
    searchParameter.SearchMode = Constants.Contains;

    var response = await assetLibraryService.getAssetLibrary(searchParameter);
    this.assetLibraryList.push(...response.assestLibraryList);
  }

  onAssetLibrarySelected(event) {
    const value = event.target.value;
    if (value != '0') {
      this.LoadAsset(value);
    } else {
      this.assets = [];
      this.assets.push({ 'Identifier': '0', 'Name': 'Select Asset' });
    }
  }


  async LoadAsset(value) {
    this.assets = [];
    this.assets.push({ 'Identifier': '0', 'Name': 'Select Asset' });
    let assetLibraryService = this.injector.get(AssetLibraryService);

    let assetSearchParameter: any = {};
    assetSearchParameter.AssetLibraryIdentifier = String(value);
    assetSearchParameter.IsDeleted = false;
    assetSearchParameter.SortParameter = {};
    assetSearchParameter.SortParameter.SortColumn = Constants.Name;
    assetSearchParameter.SortParameter.SortOrder = Constants.Ascending;
    assetSearchParameter.SearchMode = Constants.Contains;

    var response = await assetLibraryService.getAsset(assetSearchParameter);
    this.assets.push(...response.assestList);
  }

  async getEntityField(value) {
    var data = await this.dynamicWidgetService.getEntityFields(value);
    this.entityFieldList = [{ "Name": "Select", "Identifier": 0 }];
    data.forEach(item => {
      this.entityFieldList.push(item);
    });
    this.entityFieldList.forEach(item => {
      if (item.DataType != null && item.DataType == "String") {
        this.pieChartSeriesEntityFields.push(item);
      }
    });

    this.entityFieldList.forEach(item => {
      if (item.DataType != null && item.DataType != "String") {
        this.pieChartValueEntityFields.push(item);
        this.lineBarGraphFields.push(item);
      }
    });

    if (this.entityFieldList.length == 0) {
      let message = ErrorMessageConstants.getNoRecordFoundMessage;
      this._messageDialogService.openDialogBox('Error', message, Constants.msgBoxError).subscribe(data => {
      });
    }
  }

  public onEntityFieldSelected(event, widgetType) {

    const value = event.target.value;
    if (value == "0") {
      //this.filterPageTypeId = 0;
    }
    else {
      // this.filterPageTypeId = Number(value);
    }
  }

  public addFieldDetails(widgetType) {
    var object: any;
    if (widgetType == 'Form') {

      var index = this.formList.findIndex(i => { return i.FieldId == this.DynamicWidgetForm.value.FormEntityField });
      if (index >= 0) {
        this._messageDialogService.openDialogBox('Error', "Field already added", Constants.msgBoxError);
      }
      else {
        var field = this.entityFieldList.filter(i => { return i.Identifier == this.DynamicWidgetForm.value.FormEntityField });
        object = {
          "DisplayName": this.DynamicWidgetForm.value.FormFieldDisplayName,
          "FieldId": this.DynamicWidgetForm.value.FormEntityField,
          "FieldName": field[0].Name
        }
        this.formList.push(object);
        this.DynamicWidgetForm.patchValue({
          FormFieldDisplayName: null,
          FormEntityField: 0,
        })
      }
    }
    else if (widgetType == 'Table') {
      var index = this.tableHeader.findIndex(i => { return i.FieldId == this.DynamicWidgetForm.value.TableEntityField });
      if (index >= 0) {
        this._messageDialogService.openDialogBox('Error', "Field already added", Constants.msgBoxError);
      }
      else {
        var field = this.entityFieldList.filter(i => { return i.Identifier == this.DynamicWidgetForm.value.TableEntityField });
        object = {
          "HeaderName": this.DynamicWidgetForm.value.TableHeaderName,
          "FieldId": this.DynamicWidgetForm.value.TableEntityField,
          "FieldName": field[0].Name,
          "IsSorting": this.DynamicWidgetForm.value.TableIsSorting
        }
        this.tableHeader.push(object);
        this.DynamicWidgetForm.patchValue({
          TableHeaderName: null,
          TableEntityField: 0,
          TableIsSorting: false
        })
      }
    }
    else if (widgetType = 'LineBraGraph') {
      var index = this.lineBarGraphList.findIndex(i => { return i.FieldId == this.DynamicWidgetForm.value.LineBarEntityField });
      if (index >= 0) {
        this._messageDialogService.openDialogBox('Error', "Field already added", Constants.msgBoxError);
      }
      else {
        var field = this.entityFieldList.filter(i => { return i.Identifier == this.DynamicWidgetForm.value.LineBarEntityField });
        object = {
          "DisplayName": this.DynamicWidgetForm.value.LineBarFieldDisplayName,
          "FieldId": this.DynamicWidgetForm.value.LineBarEntityField,
          "FieldName": field[0].Name
        }
        this.lineBarGraphList.push(object);
        this.dataSource = new MatTableDataSource<any>(this.lineBarGraphList);
        this.DynamicWidgetForm.patchValue({
          LineBarFieldDisplayName: null,
          LineBarEntityField: 0,
        })
      }
    }

  }

  public AddFilterCondition() {
    var object: any = {};
    object.FieldId = this.DynamicWidgetForm.value.FilterField
    object.Operator = this.DynamicWidgetForm.value.FilterOperator

    if (this.DynamicWidgetForm.value.FilterConditionOperator == null || this.DynamicWidgetForm.value.FilterConditionOperator == 0) {
      object.ConditionalOperator = null;
    }
    else {
      object.ConditionalOperator = this.DynamicWidgetForm.value.FilterConditionOperator;
    }
    object.Sequence = this.displayWidgetFilterlist.length + 1;
    object.Value = this.DynamicWidgetForm.value.FilterValue;
    this.widgetFilterlist.push(object);
    object.OperatorName = this.conditionList.filter(item => item.Identifier == object.Operator)[0].Name;
    object.FieldName = this.entityFieldList.filter(item => item.Identifier == object.FieldId)[0].Name;
    this.displayWidgetFilterlist.push(object);
    this.DynamicWidgetForm.patchValue({
      FilterField: 0,
      FilterOperator: 0,
      FilterConditionOperator: 0,
      FilterValue: null
    })
  }

  public AddHTMLField() {
    var entityField = this.entityFieldList.filter(item => item.Identifier == this.DynamicWidgetForm.value.HTMLEntityField)[0];
    var text = "{{" + entityField.Name + "_" + entityField.Identifier + "}}";

    this.rteObj.executeCommand('insertText', text);
    this.DynamicWidgetForm.patchValue({
      HTMLEntityField: 0,
    })
  }

  public AddAsset() {
    var asset = this.assets.filter(item => item.Identifier == this.DynamicWidgetForm.value.HTMLAsset)[0];

    this.PreviewAsset(asset);
    this.DynamicWidgetForm.patchValue({
      HTMLAssetLibrary: 0,
      HTMLAsset: 0
    });
  }

  PreviewAsset(asset: any): void {
    var fileType = asset.Name.split('.').pop();
    var isImage;
    var source;
    if (fileType == 'png' || fileType == 'jpeg' || fileType == 'jpg') {
      isImage = true;
      var url = "http://localhost/API/test/assets/10/COPY.png";
      var img = document.createElement('img');
      img.src = url;
      this.rteObj.executeCommand('insertHTML', img);
   
    }
    else {
      isImage = false;

      var url = "http://localhost/API/test/assets/31/testvideo.mp4";

      var video = document.createElement('video');
      video.src = url;
      video.controls = true;
      this.rteObj.executeCommand('insertHTML', video);
    }

  }

  public disableAddHTMLField() {

    if (this.DynamicWidgetForm.value.HTMLEntityField == null || this.DynamicWidgetForm.value.HTMLEntityField == 0) {
      return true;
    }
    return false
  }

  public disableAssetAdd() {

    if (this.DynamicWidgetForm.value.HTMLAssetLibrary == null || this.DynamicWidgetForm.value.HTMLAssetLibrary == 0) {
      return true;
    }
    if (this.DynamicWidgetForm.value.HTMLAsset == null || this.DynamicWidgetForm.value.HTMLAsset == 0) {
      return true;
    }
    return false
  }

  public disableAddFilterCondition() {
    if (this.DynamicWidgetForm.value.FilterField == null || this.DynamicWidgetForm.value.FilterField == 0) {
      return true;
    }
    if (this.DynamicWidgetForm.value.FilterOperator == null || this.DynamicWidgetForm.value.FilterOperator == 0) {
      return true;
    }
    if (this.DynamicWidgetForm.value.FilterValue == null || this.DynamicWidgetForm.value.FilterValue == '') {
      return true;
    }
    if (this.displayWidgetFilterlist.length > 0) {
      if (this.DynamicWidgetForm.value.FilterConditionOperator == null || this.DynamicWidgetForm.value.FilterConditionOperator == 0) {
        return true;
      }
      return false;
    }
  }

  public disableAddFieldDetails(widgetType) {
    if (widgetType == 'Form') {
      if (this.DynamicWidgetForm.value.FormFieldDisplayName == null || this.DynamicWidgetForm.value.FormFieldDisplayName == '') {
        return true;
      }
      if (this.DynamicWidgetForm.value.FormEntityField == null || this.DynamicWidgetForm.value.FormEntityField == 0) {
        return true;
      }
      return false;
    }
    if (widgetType == 'Table') {
      if (this.DynamicWidgetForm.value.TableHeaderName == null || this.DynamicWidgetForm.value.TableHeaderName == '') {
        return true;
      }
      if (this.DynamicWidgetForm.value.TableEntityField == null || this.DynamicWidgetForm.value.TableEntityField == 0) {
        return true;
      }
      return false;
    }
    if (widgetType == 'LineBraGraph') {
      if (this.DynamicWidgetForm.value.LineBarFieldDisplayName == null || this.DynamicWidgetForm.value.LineBarFieldDisplayName == '') {
        return true;
      }
      if (this.DynamicWidgetForm.value.LineBarEntityField == null || this.DynamicWidgetForm.value.LineBarEntityField == 0) {
        return true;
      }
      return false;
    }
  }

  public deleteFieldDetails(widgetType, index) {
    if (widgetType == 'Form') {
      this.formList.splice(index, 1);
    }
    else if (widgetType == 'Table') {
      this.tableHeader.splice(index, 1);
    }
    else if (widgetType == 'LineBarGraph') {

      var itemIndex = this.lineBarGraphList.findIndex(i => { return i.FieldId == index });
      if (itemIndex >= 0) {
        this.lineBarGraphList.splice(itemIndex, 1);
        this.dataSource = new MatTableDataSource<any>(this.lineBarGraphList);
      }

    }
  }

  public DeleteFilterCondition(seq) {
    this.widgetFilterlist = this.widgetFilterlist.filter(item => { return item.Sequence != seq });
    this.displayWidgetFilterlist = this.displayWidgetFilterlist.filter(item => { return item.Sequence != seq });
    var seqno = 1;
    this.widgetFilterlist.forEach(item => {
      item.Sequence = seqno;
      seqno++;
    });
    seqno = 1;
    this.displayWidgetFilterlist.forEach(item => {
      item.Sequence = seqno;
      seqno++;
    })

  }

  public saveButtonValidation() {
    if (this.selectedLink == "Form") {
      if (this.formList.length <= 0) {
        return true;
      }
    }
    else if (this.selectedLink == "Table") {
      if (this.tableHeader.length <= 0) {
        return true;
      }
    }
    else if (this.selectedLink == 'LineGraph' || this.selectedLink == 'BarGraph') {
      if (this.lineBarGraphList.length <= 0) {
        return true;
      }
      if (this.DynamicWidgetForm.value.LineBarXAxis == null || this.DynamicWidgetForm.value.LineBarXAxis == 0) {
        return true;
      }
    }
    else if (this.selectedLink == "PieChart") {
      if (this.DynamicWidgetForm.value.PieValue == null || this.DynamicWidgetForm.value.PieValue == 0) {
        return true;
      }
      if (this.DynamicWidgetForm.value.PieSeries == null || this.DynamicWidgetForm.value.PieSeries == 0) {
        return true;
      }
    }
    //else if (this.selectedLink == "Html") {
    //  var html = this.rteObj.getHtml();
    //  if (html == null || html == "") {
    //    return html;
    //  }
    //}
    if (this.isCustome) {

      if (this.DynamicWidgetForm.value.TitleColor == null && this.DynamicWidgetForm.value.TitleColor == "") {
        return true;
      }
      if (this.DynamicWidgetForm.value.TitleSize == null || this.DynamicWidgetForm.value.TitleSize <= 0) {
        return true;
      }
      if (this.DynamicWidgetForm.value.TitleWeight == null || this.DynamicWidgetForm.value.TitleWeight == 0) {
        return true;
      }
      if (this.DynamicWidgetForm.value.TitleType == null || this.DynamicWidgetForm.value.TitleType == 0) {
        return true;
      }
      if (this.DynamicWidgetForm.value.HeaderColor == null && this.DynamicWidgetForm.value.HeaderColor == "") {
        return true;
      }
      if (this.DynamicWidgetForm.value.HeaderSize == null || this.DynamicWidgetForm.value.HeaderSize <= 0) {
        return true;
      }
      if (this.DynamicWidgetForm.value.HeaderWeight == null || this.DynamicWidgetForm.value.HeaderWeight == 0) {

        return true;
      }
      if (this.DynamicWidgetForm.value.HeaderType == null || this.DynamicWidgetForm.value.HeaderType == 0) {
        return true;
      }
      if (this.DynamicWidgetForm.value.DataColor == null && this.DynamicWidgetForm.value.DataColor == "") {
        return true;
      }
      if (this.DynamicWidgetForm.value.DataSize == null || this.DynamicWidgetForm.value.DataSize <= 0) {
        return true;
      }
      if (this.DynamicWidgetForm.value.DataWeight == null || this.DynamicWidgetForm.value.DataWeight == 0) {
        return true;
      }
      if (this.DynamicWidgetForm.value.DataType == null || this.DynamicWidgetForm.value.DataType == 0) {
        return true;
      }
    }
    return false;

  }

  async saveWidgetDetails() {
    this.dynamicWidgetDetails.ThemeType = this.isDefault == true ? "Default" : "Custome";
    this.dynamicWidgetDetails.ThemeCSS = '';
    var chartTheme = '';
    if (this.selectedLink == 'Form') {
      this.dynamicWidgetDetails.WidgetSettings = JSON.stringify(this.formList);
    }
    else if (this.selectedLink == 'Table') {
      this.dynamicWidgetDetails.WidgetSettings = JSON.stringify(this.tableHeader);
    }
    else if (this.selectedLink == 'LineGraph' || this.selectedLink == 'BarGraph') {
      var object: any = {};
      object.Details = this.lineBarGraphList;
      object.XAxis = this.DynamicWidgetForm.value.LineBarXAxis;
      this.dynamicWidgetDetails.WidgetSettings = JSON.stringify(object);
      chartTheme = this.selectedTheme
    }
    else if (this.selectedLink == 'PieChart') {
      var object: any = {};
      object.PieSeries = this.DynamicWidgetForm.value.PieSeries;
      object.PieValue = this.DynamicWidgetForm.value.PieValue;
      this.dynamicWidgetDetails.WidgetSettings = JSON.stringify(object);
      chartTheme = this.selectedTheme;
    }
    else if (this.selectedLink == 'Html') {
      var html = this.rteObj.getHtml();
      this.dynamicWidgetDetails.WidgetSettings = html;
    }
    if (this.dynamicWidgetDetails.ThemeType == "Custome") {

      var themeObject = {
        "TitleColor": this.DynamicWidgetForm.value.TitleColor,
        "TitleSize": this.DynamicWidgetForm.value.TitleSize,
        "TitleWeight": this.DynamicWidgetForm.value.TitleWeight,
        "TitleType": this.DynamicWidgetForm.value.TitleType,
        "HeaderColor": this.DynamicWidgetForm.value.HeaderColor,
        "HeaderSize": this.DynamicWidgetForm.value.HeaderSize,
        "HeaderWeight": this.DynamicWidgetForm.value.HeaderWeight,
        "HeaderType": this.DynamicWidgetForm.value.HeaderType,
        "DataColor": this.DynamicWidgetForm.value.DataColor,
        "DataSize": this.DynamicWidgetForm.value.DataSize,
        "DataWeight": this.DynamicWidgetForm.value.DataWeight,
        "DataType": this.DynamicWidgetForm.value.DataType,
        "ChartColorTheme": chartTheme
      }
      this.dynamicWidgetDetails.ThemeCSS = JSON.stringify(themeObject);
      this.selectedTheme = themeObject.ChartColorTheme;

    }
    this.dynamicWidgetDetails.WidgetFilterSettings = '';
    if (this.widgetFilterlist.length > 0) {
      this.dynamicWidgetDetails.WidgetFilterSettings = JSON.stringify(this.widgetFilterlist);
    }


    var userid = localStorage.getItem('UserId');
    this.dynamicWidgetDetails.CreatedBy = Number(userid);
    var widgetList = [];
    widgetList.push(this.dynamicWidgetDetails);
    let isRecordSaved = await this.dynamicWidgetService.saveDynamicWidget(widgetList, this.updateOperationMode);
    if (isRecordSaved) {
      let message = Constants.recordAddedMessage;
      if (this.updateOperationMode) {
        message = Constants.recordUpdatedMessage;
      }
      this._messageDialogService.openDialogBox('Success', message, Constants.msgBoxSuccess);
      this.navigateToListPage()
    }
  }

  NavigateToBack() {

    let queryParams = {
      Routeparams: {
        passingparams: {
          "WidgetName": this.dynamicWidgetDetails.WidgetName,
          "WidgetType": this.dynamicWidgetDetails.WidgetType,
          "PageTypeId": this.dynamicWidgetDetails.PageTypeId,
          "EntityId": this.dynamicWidgetDetails.EntityId,
          "Title": this.dynamicWidgetDetails.Title,
          "updateOperationMode": this.updateOperationMode,
          "DynamicWidgetIdentifier": this.updateOperationMode ? this.dynamicWidgetDetails.Identifier : 0,
        }
      }
    }
    if (this.updateOperationMode) {
      localStorage.setItem("dynamicWidgetEditRouteparams", JSON.stringify(queryParams))
      this._router.navigate(['dynamicwidget', 'Edit']);
    }
    else {
      localStorage.setItem("dynamicWidgetAddRouteparams", JSON.stringify(queryParams))
      this._router.navigate(['dynamicwidget', 'Add']);
    }
  }

  theme1() {
    this.isTheme1Active = true;
    this.isTheme2Active = false;
    this.isTheme3Active = false;
    this.isTheme4Active = false;
    this.isTheme5Active = false;
    this.isTheme0Active = false;
  }
  theme2() {
    this.isTheme1Active = false;
    this.isTheme3Active = false;
    this.isTheme2Active = true;
    this.isTheme4Active = false;
    this.isTheme5Active = false;
    this.isTheme0Active = false;
  }
  theme3() {
    this.isTheme1Active = false;
    this.isTheme3Active = true;
    this.isTheme2Active = false;
    this.isTheme4Active = false;
    this.isTheme5Active = false;
    this.isTheme0Active = false;

  }
  theme4() {
    this.isTheme1Active = false;
    this.isTheme3Active = false;
    this.isTheme2Active = false;
    this.isTheme4Active = true;
    this.isTheme5Active = false;
    this.isTheme0Active = false;
  }
  theme5() {
    this.isTheme1Active = false;
    this.isTheme3Active = false;
    this.isTheme2Active = false;
    this.isTheme4Active = false;
    this.isTheme5Active = true;
    this.isTheme0Active = false;
  }
  theme0() {
    this.isTheme1Active = false;
    this.isTheme3Active = false;
    this.isTheme2Active = false;
    this.isTheme4Active = false;
    this.isTheme5Active = false;
    this.isTheme0Active = true;
  }

}
