import { Component, OnInit, Injector, ChangeDetectorRef } from '@angular/core';
import { Location } from '@angular/common';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';
import { Constants } from 'src/app/shared/constants/constants';
import { ActivatedRoute, Router, NavigationEnd } from '@angular/router';
import { NgxUiLoaderService } from 'ngx-ui-loader';
import { MessageDialogService } from 'src/app/shared/services/mesage-dialog.service';
import { DialogService } from '@tomblue/ng2-bootstrap-modal';
import { LocalStorageService } from 'src/app/shared/services/local-storage.service';
import { TemplateService } from '../template.service';
import { ErrorMessageConstants } from 'src/app/shared/constants/constants';
import { HttpClient } from '@angular/common/http';
import { ConfigConstants } from 'src/app/shared/constants/configConstants';
import { URLConfiguration } from 'src/app/shared/urlConfiguration/urlconfiguration';
import { AssetSearchParameter } from '../../../layout/asset-libraries/asset-library';
//import { $ } from 'protractor';
import * as $ from 'jquery';

@Component({
  selector: 'app-add',
  templateUrl: './add.component.html',
  styleUrls: ['./add.component.scss']
})

export class AddComponent implements OnInit {

  public pageFormGroup: FormGroup;
  public pageTypelist: any[] = [];
  public onlyAlphabetsWithSpace = '[a-zA-Z ]*';
  public onlyAlphabetswithInbetweenSpaceUpto50Characters = Constants.onlyAlphabetswithInbetweenSpaceUpto50Characters;

  public pageEditModeOn: boolean = false;
  public params: any = {};
  public PageIdentifier = 0;
  public PageName;
  public PageTypeId = 0;
  public PageTypeName;
  public PageWidgetArrayString: string = "";
  public staticAndDynamicWidgetsArrayString:  string = '';
  public PageTypeIdBeforeChange: number = 0;
  public PageWidgetArrayStringBeforePageTypeChange: string = "";
  public isPageTypeChangeActionByClickingPreviousButton: boolean = false;
  public baseURL = ConfigConstants.BaseURL;
  public assetLibraryList: any[] = [{ 'Identifier': '0', 'Name': 'Select Asset Library' }];
  public assets: any[] = [{ 'Identifier': '0', 'Name': 'Select Asset' }];

  public AssetLibraryIdOfBackgroundImage = 0;
  public AssetIdOfBackgroundImage = 0;
  public UrlOfBackgroundImage = '';

  public pageFormErrorObject: any = {
    showPageNameError: false,
    showPageTypeError: false
  };

  //getters of page Form group
  get pageName() {
    return this.pageFormGroup.get('pageName');
  }
  get pageType() {
    return this.pageFormGroup.get('pageType');
  }
  get assetLibrary() {
    return this.pageFormGroup.get('assetLibrary');
  }
  get asset() {
    return this.pageFormGroup.get('asset');
  }
  get pageBackgroundImageUrl() {
    return this.pageFormGroup.get('pageBackgroundImageUrl');
  }

  get pf() {
    return this.pageFormGroup.controls;
  }

  //function to validate all fields
  validateAllFormFields(formGroup: FormGroup) {
    Object.keys(formGroup.controls).forEach(field => {
      const control = formGroup.get(field);
      if (control instanceof FormControl) {
        control.markAsTouched({ onlySelf: true });
      } else if (control instanceof FormGroup) {
        this.validateAllFormFields(control);
      }
    });
  }

  constructor(private _location: Location,
    private formbuilder: FormBuilder,
    private injector: Injector,
    private _dialogService: DialogService,
    private uiLoader: NgxUiLoaderService,
    private _messageDialogService: MessageDialogService,
    private router: Router,
    private _http: HttpClient,
    private localstorageservice: LocalStorageService) {

    if (localStorage.getItem("pageparams")) {
      this.pageEditModeOn = true;
    } else {
      this.pageEditModeOn = false;
    }

    router.events.subscribe(e => {
      if (e instanceof NavigationEnd) {
        if (e.url.includes('/pages')) {
          //set passing parameters to localstorage.
          if (localStorage.getItem('pageparams')) {
            this.params = JSON.parse(localStorage.getItem('pageparams'));
            this.PageIdentifier = this.params.Routeparams.passingparams.PageIdentifier
            this.PageName = this.params.Routeparams.filteredparams.PageName
          }
          if (localStorage.getItem('pageAddRouteparams')) {
            this.params = JSON.parse(localStorage.getItem('pageAddRouteparams'));
            this.PageName = this.params.Routeparams.passingparams.PageName
            this.PageTypeId = this.params.Routeparams.passingparams.PageTypeId
            this.PageTypeIdBeforeChange = this.params.Routeparams.passingparams.PageTypeId
            this.PageWidgetArrayString = this.params.Routeparams.passingparams.PageWidgetArray
            this.staticAndDynamicWidgetsArrayString = this.params.Routeparams.passingparams.StaticAndDynamicWidgetsArray
            this.PageWidgetArrayStringBeforePageTypeChange = this.params.Routeparams.passingparams.PageWidgetArray
            this.isPageTypeChangeActionByClickingPreviousButton = true;
            this.AssetLibraryIdOfBackgroundImage = this.params.Routeparams.passingparams.BackgroundImageAssetLibraryId
            this.AssetIdOfBackgroundImage = this.params.Routeparams.passingparams.BackgroundImageAssetId
            this.UrlOfBackgroundImage = this.params.Routeparams.passingparams.BackgroundImageURL
          }
          if (localStorage.getItem('pageEditRouteparams')) {
            this.params = JSON.parse(localStorage.getItem('pageEditRouteparams'));
            this.PageName = this.params.Routeparams.passingparams.PageName
            this.PageTypeId = this.params.Routeparams.passingparams.PageTypeId
            this.PageIdentifier = this.params.Routeparams.passingparams.PageIdentifier
            this.pageEditModeOn = this.params.Routeparams.passingparams.pageEditModeOn
            this.PageTypeIdBeforeChange = this.params.Routeparams.passingparams.PageTypeId
            this.PageWidgetArrayString = this.params.Routeparams.passingparams.PageWidgetArray
            this.staticAndDynamicWidgetsArrayString = this.params.Routeparams.passingparams.StaticAndDynamicWidgetsArray
            this.AssetLibraryIdOfBackgroundImage = this.params.Routeparams.passingparams.BackgroundImageAssetLibraryId
            this.AssetIdOfBackgroundImage = this.params.Routeparams.passingparams.BackgroundImageAssetId
            this.UrlOfBackgroundImage = this.params.Routeparams.passingparams.BackgroundImageURL
          }
        }
        else {
          localStorage.removeItem("pageparams");
          localStorage.removeItem("pageAddRouteparams");
          localStorage.removeItem("pageEditRouteparams");
        }
      }
    });
  }

  //initialization call
  ngOnInit() {
    //Validations for Page Form.
    this.pageFormGroup = this.formbuilder.group({
      pageName: [null, Validators.compose([Validators.required, Validators.minLength(Constants.inputMinLenth),
      Validators.maxLength(Constants.inputMaxLenth), Validators.pattern(this.onlyAlphabetswithInbetweenSpaceUpto50Characters)])],
      pageType: [0, Validators.compose([Validators.required])],
      assetLibrary: [0],
      asset: [0],
      pageBackgroundImageUrl: [null]
    });

    if (this.PageName != null && this.PageTypeId != null) {
      this.pageFormGroup.patchValue({
        pageName: this.PageName,
        pageType: this.PageTypeId,
        assetLibrary: this.AssetLibraryIdOfBackgroundImage,
        asset: this.AssetIdOfBackgroundImage,
        pageBackgroundImageUrl: this.UrlOfBackgroundImage
      });
    }
    this.getPageTypes();

    if (this.AssetLibraryIdOfBackgroundImage != null && this.AssetLibraryIdOfBackgroundImage != 0) {
      this.pageFormGroup.get('pageBackgroundImageUrl').disable();
      this.LoadAsset(this.AssetLibraryIdOfBackgroundImage);
    }

    if (this.UrlOfBackgroundImage != null && this.UrlOfBackgroundImage != '') {
      this.pageFormGroup.get('assetLibrary').disable();
      this.pageFormGroup.get('asset').disable();
      this.pageFormGroup.patchValue({ assetLibrary: [0], asset: [0], });
    }

  }

  saveBtnValidation(): boolean {
    if (this.pageFormGroup.controls.pageName.invalid) {
      return true;
    }
    if (this.PageTypeId == 0) {
      return true;
    }
    return false;
  }

  OnSaveBtnClicked() {
    let pageObject: any = {};
    pageObject.DisplayName = this.pageFormGroup.value.pageName;
    pageObject.PageTypeId = this.PageTypeId;
    pageObject.BackgroundImageAssetId = this.pageFormGroup.value.asset;
    pageObject.BackgroundImageURL = this.pageFormGroup.value.pageBackgroundImageUrl;
    if (this.pageEditModeOn) {
      pageObject.Identifier = this.PageIdentifier
    }

    let pageWidgets: any[] = [];
    if (this.PageWidgetArrayString != null && this.PageWidgetArrayString != "" && this.testJSON(this.PageWidgetArrayString)) {
      let widgetsGridsterItemArray = JSON.parse(this.PageWidgetArrayString);
      for (var i = 0; i < widgetsGridsterItemArray.length; i++) {
        let widgetsGridsterItem = widgetsGridsterItemArray[i];
        let pageWidget: any = {};
        pageWidget.WidgetId = widgetsGridsterItem.WidgetId;
        pageWidget.Height = widgetsGridsterItem.rows;
        pageWidget.Width = widgetsGridsterItem.cols;
        pageWidget.Xposition = widgetsGridsterItem.x;
        pageWidget.Yposition = widgetsGridsterItem.y;
        pageWidget.WidgetSetting = widgetsGridsterItem.WidgetSetting != null ? widgetsGridsterItem.WidgetSetting : "";
        pageWidget.IsDynamicWidget = widgetsGridsterItem.IsDynamicWidget;
        pageWidgets.push(pageWidget);
      }
    }
    pageObject.PageWidgets = pageWidgets;
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
      this.navigateToListPage();
      localStorage.removeItem("pageparams");
      localStorage.removeItem("pageAddRouteparams");
      localStorage.removeItem("pageEditRouteparams");
    }
  }

  testJSON(text) {
    if (typeof text !== "string") {
      return false;
    }
    try {
      JSON.parse(text);
      return true;
    }
    catch (error) {
      return false;
    }
  }

  public onPageTypeSelected(event) {
    const value = event.target.value;
    if (value == "0") {
      this.pageFormErrorObject.showPageTypeError = true;
      this.PageTypeId = 0;
      this.PageTypeName = '';
    }
    else {
      if (this.isPageTypeChangeActionByClickingPreviousButton == true && Number(value) != this.PageTypeIdBeforeChange) {
        let message = "This page type change action will discard all page widget configuration. \nAre you sure, you want to change page type?";
        this._messageDialogService.openConfirmationDialogBox('Confirm', message, Constants.msgBoxWarning).subscribe(async (isConfirmed) => {
          if (isConfirmed) {
            this.pageFormErrorObject.showPageTypeError = false;
            this.PageTypeId = Number(value);
            let pageTypeObj = this.pageTypelist.find(s => s.Identifier == value);
            this.PageTypeName = pageTypeObj.Name;
            this.PageWidgetArrayString = "";
          } else {
            this.PageTypeId = this.PageTypeIdBeforeChange;
            this.PageWidgetArrayString = this.PageWidgetArrayStringBeforePageTypeChange;
            this.pageFormGroup.patchValue({
              pageType: this.PageTypeId
            });
          }
        });
      } else {
        this.PageWidgetArrayString = this.PageWidgetArrayStringBeforePageTypeChange;
        this.pageFormErrorObject.showPageTypeError = false;
        this.PageTypeId = Number(value);
        let pageTypeObj = this.pageTypelist.find(s => s.Identifier == value);
        this.PageTypeName = pageTypeObj.Name;
      }
    }
  }

  navigateToListPage() {
    const router = this.injector.get(Router);
    router.navigate(['pages']);
  }

  navigationTodashboardDesigner() {
    let queryParams = {
      Routeparams: {
        passingparams: {
          "PageName": this.pageFormGroup.value.pageName,
          "PageTypeId": this.PageTypeId,
          "PageTypeName": this.PageTypeName,
          "BackgroundImageAssetLibraryId": this.pageFormGroup.value.assetLibrary,
          "BackgroundImageAssetId": this.pageFormGroup.value.asset,
          "BackgroundImageURL": this.pageFormGroup.value.pageBackgroundImageUrl,
          "pageEditModeOn": this.pageEditModeOn,
          "PageIdentifier": this.pageEditModeOn ? this.PageIdentifier : 0,
          "PageWidgetArrayString": this.PageWidgetArrayString,
          "StaticAndDynamicWidgetArrayString": this.staticAndDynamicWidgetsArrayString
        }
      }
    }
    localStorage.setItem("pageDesignRouteparams", JSON.stringify(queryParams));
    this.router.navigate(['../dashboardDesigner']);
  }

  async getPageTypes() {
    let templateService = this.injector.get(TemplateService);
    this.pageTypelist = [{ "Identifier": 0, "PageTypeName": "Select Page Type" }];
    let list = await templateService.getPageTypes();
    if (this.pageTypelist.length == 0) {
      let message = ErrorMessageConstants.getNoRecordFoundMessage;
      this._messageDialogService.openDialogBox('Error', message, Constants.msgBoxError).subscribe(data => {
        if (data == true) {
          this.getPageTypes();
        }
      });
    } else {
      this.getAssetLibraries();
      this.pageTypelist = [...this.pageTypelist, ...list];
    }
  }

  async getAssetLibraries() {
    let searchParameter: any = {};
    searchParameter.IsActive = true;
    searchParameter.PagingParameter = {};
    searchParameter.PagingParameter.PageIndex = Constants.DefaultPageIndex;
    searchParameter.PagingParameter.PageSize = Constants.DefaultPageSize;
    searchParameter.SortParameter = {};
    searchParameter.SortParameter.SortColumn = Constants.Name;
    searchParameter.SortParameter.SortOrder = Constants.Ascending;
    searchParameter.SearchMode = Constants.Contains;
    this.uiLoader.start();
    this._http.post(this.baseURL + URLConfiguration.assetLibraryGetUrl, searchParameter).subscribe(
      data => {
        this.uiLoader.stop();
        let records = <any[]>data;
        this.assetLibraryList.push(...records);
      },
      error => {
        this._messageDialogService.openDialogBox('Error', error.error.Message, Constants.msgBoxError);
      }
    );
  }

  onAssetLibrarySelected(event) {
    const value = event.target.value;
    if (value != '0') {
      this.LoadAsset(value);
      this.pageFormGroup.get('pageBackgroundImageUrl').disable();
    } else {
      this.assets = [];
      this.assets.push({ 'Identifier': '0', 'Name': 'Select Asset' });
      this.pageFormGroup.get('pageBackgroundImageUrl').enable();
    }
  }

  OnBackgroundImageUrlChange() {
    if (this.pageFormGroup.value.pageBackgroundImageUrl == '') {
      this.pageFormGroup.get('assetLibrary').enable();
      this.pageFormGroup.get('asset').enable();
    } else {
      this.pageFormGroup.get('assetLibrary').disable();
      this.pageFormGroup.get('asset').disable();
    }
  }

  LoadAsset(value): void {
    this.assets = [];
    this.assets.push({ 'Identifier': '0', 'Name': 'Select Asset' });
    let assetSearchParameter: AssetSearchParameter = new AssetSearchParameter();
    assetSearchParameter.AssetLibraryIdentifier = String(value);
    assetSearchParameter.IsDeleted = false;
    assetSearchParameter.PagingParameter.PageIndex = Constants.DefaultPageIndex;
    assetSearchParameter.PagingParameter.PageSize = Constants.DefaultPageSize;
    assetSearchParameter.SortParameter.SortColumn = Constants.Name;
    assetSearchParameter.SortParameter.SortOrder = Constants.Ascending;
    assetSearchParameter.SearchMode = Constants.Contains;
    let assets: any[];
    this.uiLoader.start();
    this._http.post(this.baseURL + URLConfiguration.assetGetUrl, assetSearchParameter).subscribe(
      data => {
        this.uiLoader.stop();
        assets = <any[]>data;
        this.assets.push(...assets);
        if (this.AssetIdOfBackgroundImage == null || this.AssetIdOfBackgroundImage == 0) {
          this.pageFormGroup.patchValue({ asset: [0], });
        }

      },
      error => {
        this.uiLoader.stop();
        this._messageDialogService.openDialogBox('Error', error.error.Message, Constants.msgBoxError);
      }
    );
  }

}
