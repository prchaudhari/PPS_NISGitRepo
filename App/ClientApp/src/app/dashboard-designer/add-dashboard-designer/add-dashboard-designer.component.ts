import { AppSettings } from '../../appsettings';
import { Component, OnInit, Injector, SecurityContext, ViewChild } from '@angular/core';
import { CompactType, DisplayGrid, GridsterConfig, GridsterItem, GridType, GridsterItemComponentInterface, GridsterComponentInterface } from 'angular-gridster2';
import { Location } from '@angular/common';
import { Constants } from 'src/app/shared/constants/constants';
import { ActivatedRoute, Router, NavigationEnd } from '@angular/router';
import { NgxUiLoaderService } from 'ngx-ui-loader';
import { MessageDialogService } from 'src/app/shared/services/mesage-dialog.service';
import { DialogService } from '@tomblue/ng2-bootstrap-modal';
import { TemplateService } from '../../layout/template/template.service';
import { LocalStorageService } from 'src/app/shared/services/local-storage.service';
import { Template } from '../../layout/template/template';
//import { TemplateWidget } from '../../layout/template/templateWidget';CorporateSaverAgentMessageComponent
import {
  CustomerInformationComponent, AccountInformationComponent, ImageComponent, VideoComponent, SummaryAtGlanceComponent, TransactionDetailsComponent,
  SavingAvailableBalanceComponent, CurrentAvailableBalanceComponent, SavingTransactionDetailsComponent,
  SpendindTrendsComponent, TopIncomeSourcesComponent, SavingTrendsComponent, AnalyticsWidgetComponent, ReminderAndRecommComponent,
  DynamicBarChartWidgetComponent, DynamicLineChartWidgetComponent, DynamicPieChartWidgetComponent, DynamicHhtmlComponent, StaticHtmlComponent, SegmentBasedContentComponent,
  SpecialMessageComponent, PersonalLoanInsuranceMessageComponent,
  CSAgentLogoComponent, FSPDetailsComponent
} from '../widgetComponent/widgetComponent';
import { AssetLibraryService } from '../../layout/asset-libraries/asset-library.service';
import { AssetSearchParameter } from '../../layout/asset-libraries/asset-library';
import { HttpClient } from '@angular/common/http';
import { ConfigConstants } from 'src/app/shared/constants/configConstants';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';
import { WidgetService } from '../../layout/widgets/widget.service';
import { URLConfiguration } from 'src/app/shared/urlConfiguration/urlconfiguration';
import * as $ from 'jquery';
import { map } from 'rxjs/operators';
import { DomSanitizer, SafeResourceUrl, SafeUrl } from '@angular/platform-browser';
import { DynamicWidgetService } from '../../layout/widget-dynamic/dynamicwidget.service';
import { PreviewDialogService } from '../../shared/services/preview-dialog.service';
import { RichTextEditorComponent } from '@syncfusion/ej2-angular-richtexteditor';
import { ToolbarService, LinkService, ImageService, HtmlEditorService, QuickToolbarService } from '@syncfusion/ej2-angular-richtexteditor';
import { TemplateWidget } from '../../layout/template/templateWidget';

@Component({
  selector: 'app-add-dashboard-designer',
  templateUrl: './add-dashboard-designer.component.html',
  styleUrls: ['./add-dashboard-designer.component.scss'],
  providers: [ToolbarService, LinkService, ImageService, HtmlEditorService, QuickToolbarService]
})

export class AddDashboardDesignerComponent implements OnInit {
  @ViewChild('staticHtmlRTE', null) rteObj: RichTextEditorComponent;
  public isImageConfig: boolean = false;
  public isVideoConfig: boolean = false;
  public isStaticHtmlConfig: boolean = false;
  public isCSAgentLogoConfig: boolean = false;
  public isCorporateSaverAgentMessageConfig: boolean = false;
  public isCorporateSaverClientandAgentDetailsConfig: boolean = false;
  public isCorporateSaverTransactionConfig: boolean = false;
  public isCorporateSaverAgentAddressConfig: boolean = false;
  public isCorporateSaverTableTotalConfig: boolean = false;
  public isCorporateAgentDetailsConfig: boolean = false;
  public issegmentBasedContentConfig: boolean = false;
  public isWidgetSidebar: boolean = false;
  public isEmbedded: boolean = false;
  public isPersonalizeImage: boolean = false;
  public isPersonalize: boolean = false;
  public isMasterSaveBtnDisabled: boolean = false;


  options: GridsterConfig;
  dashboard: Array<GridsterItem>;
  item: any[];
  public widgetsArray: any = [];
  public widgetId;
  public widgetsGridsterItemArray: any[] = [];
  public params: any = {};
  public PageIdentifier;
  public PageName;
  public PageTypeId;
  public PageTypeName;
  public pageEditModeOn: boolean = false;
  public baseURL: string = AppSettings.baseURL;
  public validUrlRegexPattern = '(https?://)?([\\da-z.-]+)\\.([a-z.]{2,6})[/\\w .-]*/?';
  public OnlyNumbersAllowed = '[0-9]*';

  public templateList: Template[] = [];
  public assetLibraryList: any[] = [{ 'Identifier': '0', 'Name': 'Select Asset Library' }];
  public assets: any[] = [{ 'Identifier': '0', 'Name': 'Select Asset' }];
  public ImageConfigForm: FormGroup;
  public imgAssetLibraryId: number = 0;
  public imgAssetLibraryName: string = "";
  public imgAssetId: number = 0;
  public imgAssetName: string = "";
  public imageSourceUrl: string;

  public VideoConfigForm: FormGroup;
  public vdoAssetLibraryId: number = 0;
  public vdoAssetLibraryName: string = "";
  public vdoAssetId: number = 0;
  public vdoAssetName: string = "";
  public vdoSourceUrl: string;
  public imageWidgetId: number = 0;
  public videoWidgetId: number = 0;
  public staticHtmlWidgetId: number = 0;
  public segmentBasedContentWidgetId: number = 0;
  public widgetItemCount: number = 0;
  public selectedWidgetItemCount: number = 0;
  public pageVersion: string;
  public ImagePreviewSrc;
  public videoPreviewSrc;
  public videoBlobObjectUrl;
  public imageBlobObjectUrl;

  public StaticConfigForm: FormGroup;
  public staticHtmlContent: string = "";

  public SegmentBasedContentForm: FormGroup;
  public segmentBasedContentContent: string = "";
  public segmentList: any[] = [{ 'Identifier': '0', 'Name': 'Select Segment' }];

  public BackgroundImageAssetId = 0;
  public BackgroundImageURL = '';
  public BackgroundImageAssetLibraryId = 0;

  public dynamicBarChartWidgetId: number = 0;

  //html editor code
  public editorValue: any = "";
  htmlContent = '';
  public onlyNumbers = "^(?=.*[1-9])[+]?([0-9]+(?:[\\.]\\d{1,2})?|\\.[0-9])$";

  public tools: object = {
    type: 'Expand',
    items: ['Bold', 'Italic', 'Underline', 'StrikeThrough',
      'FontName', 'FontSize', 'FontColor', 'BackgroundColor',
      'LowerCase', 'UpperCase', '|',
      'Formats', 'Alignments', 'OrderedList', 'UnorderedList',
      'Outdent', 'Indent', '|',
      'CreateLink', 'Image', '|', 'ClearFormat', 'Print',
      'SourceCode', 'FullScreen', '|', 'Undo', 'Redo']
  };

  public quickTools: object = {
    image: [
      'Replace', 'Align', 'Caption', 'Remove', 'InsertLink', '-', 'Display', 'AltText', 'Dimension']
  };

  public insertImageSettings: object = {
    saveFormat: "Base64"
  };

  constructor(private _location: Location,
    private injector: Injector,
    private fb: FormBuilder,
    private _dialogService: DialogService,
    private uiLoader: NgxUiLoaderService,
    private _messageDialogService: MessageDialogService,
    private localstorageservice: LocalStorageService,
    private assetLibraryService: AssetLibraryService,
    private _http: HttpClient,
    private sanitizer: DomSanitizer,
   // private staticHtmlComponent: StaticHtmlComponent,
    private router: Router) {
    router.events.subscribe(e => {
      if (e instanceof NavigationEnd) {
        if (e.url.includes('/dashboardDesigner')) {
          //set passing parameters to localstorage.
          if (localStorage.getItem('pageDesignRouteparams')) {
            this.params = JSON.parse(localStorage.getItem('pageDesignRouteparams'));
            this.PageName = this.params.Routeparams.passingparams.PageName
            this.PageIdentifier = this.params.Routeparams.passingparams.PageIdentifier
            this.PageTypeId = this.params.Routeparams.passingparams.PageTypeId
            this.PageTypeName = this.params.Routeparams.passingparams.PageTypeName
            this.pageEditModeOn = this.params.Routeparams.passingparams.pageEditModeOn
            this.BackgroundImageAssetLibraryId = this.params.Routeparams.passingparams.BackgroundImageAssetLibraryId
            this.BackgroundImageAssetId = this.params.Routeparams.passingparams.BackgroundImageAssetId
            this.BackgroundImageURL = this.params.Routeparams.passingparams.BackgroundImageURL
            this.applyBackgroundImage(this.BackgroundImageAssetId, this.BackgroundImageURL);

            if (this.params.Routeparams.passingparams.StaticAndDynamicWidgetArrayString != null && this.params.Routeparams.passingparams.StaticAndDynamicWidgetArrayString != ""
              && this.testJSON(this.params.Routeparams.passingparams.StaticAndDynamicWidgetArrayString)) {
              this.widgetsArray = JSON.parse(this.params.Routeparams.passingparams.StaticAndDynamicWidgetArrayString);
            }

            if (this.params.Routeparams.passingparams.PageWidgetArrayString != null && this.params.Routeparams.passingparams.PageWidgetArrayString != ""
              && this.testJSON(this.params.Routeparams.passingparams.PageWidgetArrayString)) {

              this.widgetsGridsterItemArray = JSON.parse(this.params.Routeparams.passingparams.PageWidgetArrayString)
              for (let x = 0; x < this.widgetsGridsterItemArray.length; x++) {
                let obj = this.bindComponent(this.widgetsGridsterItemArray[x]);
                if (obj != null) {
                  this.widgetsGridsterItemArray[x].component = obj.component;
                  this.widgetsGridsterItemArray[x].value = obj.value;
                  this.widgetsGridsterItemArray[x].TempImageIdentifier = obj.TempImageIdentifier;
                }
              }
            }
          }
        } else {
          localStorage.removeItem("pageDesignRouteparams");
          localStorage.removeItem("pageDesignEditRouteparams");
        }
      }
    });
  }

  public imageFormErrorObject: any = {
    showAssetLibraryError: false,
    showAssetError: false,
    showTaargetLinkError: false
  };

  public videoFormErrorObject: any = {
    showAssetLibraryError: false,
    showAssetError: false,
    showTaargetLinkError: false
  }

  public segmentBasedContentFormErrorObject: any = {
    showSegmentError: false,
    showContentError: false
  };

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
  get imageHeight() {
    return this.ImageConfigForm.get('imageHeight');
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

  get imgForm() {
    return this.ImageConfigForm.controls;
  }

  get vdoForm() {
    return this.VideoConfigForm.controls;
  }

  applyBackgroundImage(AssetId, ImageURL) {
    if (AssetId != null && AssetId != 0) {
      this._http.get(this.baseURL + 'assetlibrary/asset/download?assetIdentifier=' + AssetId, { responseType: "arraybuffer", observe: 'response' }).pipe(map(response => response))
        .subscribe(
          data => {
            let contentType = data.headers.get('Content-Type');
            const blob = new Blob([data.body], { type: contentType });
            let objectURL = URL.createObjectURL(blob);
            let imgUrl = this.sanitizer.sanitize(SecurityContext.RESOURCE_URL, this.sanitizer.bypassSecurityTrustResourceUrl(objectURL));
            $('gridster').css('background', 'url(' + imgUrl + ')');
          },
          error => {
            //$('.overlay').show();
          });
    } else if (ImageURL != null && ImageURL != '') {
      $('gridster').css('background', 'url(' + ImageURL + ')');
    }

  }

  saveImgFormValidation(): boolean {

    if (this.ImageConfigForm.controls.imageUrl.invalid && !this.isPersonalizeImage) {
      return true;
    }
    if (this.ImageConfigForm.controls.imageHeight.invalid && !this.isPersonalizeImage) {
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

  /**
* Marks all controls in a form group as touched
* @param formGroup - The form group to touch
*/
  private markFormGroupUnTouched(formGroup: FormGroup) {
    (<any>Object).values(formGroup.controls).forEach(control => {
      control.markAsUntouched();

      if (control.controls) {
        this.markFormGroupUnTouched(control);
      }
    });
  }

  saveVideoFormValidation(): boolean {
    if (this.VideoConfigForm.controls.vdoUrl.invalid && (!this.isPersonalize && this.isEmbedded)) {
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

  saveStaticHtmlFormValidation(): boolean {
    if (this.rteObj.value != undefined && this.rteObj.value.length > 0) {
      return false;
    }
    return true;
  }

  saveSegmentBasedContentFormValidation(): boolean {
    if (!this.segmentBasedContentFormErrorObject.showContentError && !this.segmentBasedContentFormErrorObject.showSegmentError) {
      return true;
    }
    return false;
  }

  isImageConfigForm(widgetId, widgetItemCount) {
    this.imageFormErrorObject.showAssetLibraryError = false;
    this.imageFormErrorObject.showAssetError = false;
    this.isMasterSaveBtnDisabled = true;
    this.isImageConfig = true;
    this.imageWidgetId = widgetId;
    this.selectedWidgetItemCount = widgetItemCount;
    this.isPersonalizeImage = false;

    var records = this.widgetsGridsterItemArray.filter(x => x.WidgetId == this.imageWidgetId && x.widgetItemCount == this.selectedWidgetItemCount);
    if (records.length != 0) {
      var widgetSetting = records[0].WidgetSetting;
      if (widgetSetting != null && widgetSetting != '' && this.testJSON(widgetSetting)) {
        var widgetConfigObj = JSON.parse(widgetSetting);
        this.ImageConfigForm.patchValue({
          imgAssetLibrary: widgetConfigObj.AssetLibraryId,
          imgAsset: widgetConfigObj.AssetId,
          imageUrl: widgetConfigObj.SourceUrl,
          imageHeight: widgetConfigObj.Height,
          imgAlign: widgetConfigObj.Align != undefined ? widgetConfigObj.Align : 0,
        });
        this.imgAssetId = widgetConfigObj.AssetId;
        this.imgAssetName = widgetConfigObj.AssetName;
        this.imgAssetLibraryId = widgetConfigObj.AssetLibraryId;
        this.imgAssetLibraryName = widgetConfigObj.AssetLibrayName;
        if (widgetConfigObj.AssetLibraryId != null && widgetConfigObj.AssetLibraryId != 0) {
          this.LoadAsset('image', widgetConfigObj.AssetLibraryId);
        }
        if (widgetConfigObj.isPersonalize != null) {
          this.isPersonalizeImage = widgetConfigObj.isPersonalize;
          if (widgetConfigObj.isPersonalize == false) {
            this._http.get(this.baseURL + 'assetlibrary/asset/download?assetIdentifier=' + widgetConfigObj.AssetId, { responseType: "arraybuffer", observe: 'response' }).pipe(map(response => response))
              .subscribe(
                data => {
                  let contentType = data.headers.get('Content-Type');
                  let fileName = data.headers.get('x-filename');
                  const blob = new Blob([data.body], { type: contentType });
                  let objectURL = URL.createObjectURL(blob);
                  this.ImagePreviewSrc = this.sanitizer.bypassSecurityTrustResourceUrl(objectURL);
                },
                error => {
                  //$('.overlay').show();
                });
          }
        }
      } else {
        this.ImageConfigForm.patchValue({
          imgAssetLibrary: 0,
          imgAsset: 0,
          imageUrl: null,
          imageHeight: null,
          imgAlign: 0
        });
        this.markFormGroupUnTouched(this.ImageConfigForm);
        this.ImagePreviewSrc = '';
      }
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

  isVideoConfigForm(widgetId, widgetItemCount) {
    this.videoFormErrorObject.showAssetLibraryError = false;
    this.videoFormErrorObject.showAssetError = false;
    this.isMasterSaveBtnDisabled = true;
    this.isVideoConfig = true;
    this.videoWidgetId = widgetId;
    this.selectedWidgetItemCount = widgetItemCount;
    this.isPersonalize = false;
    this.isEmbedded = false;

    var records = this.widgetsGridsterItemArray.filter(x => x.WidgetId == this.videoWidgetId && x.widgetItemCount == this.selectedWidgetItemCount);
    if (records.length != 0) {
      var widgetSetting = records[0].WidgetSetting;
      if (widgetSetting != null && widgetSetting != '' && this.testJSON(widgetSetting)) {
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

        if (widgetConfigObj.isPersonalize != null) {
          this.isPersonalize = widgetConfigObj.isPersonalize;
        }
        if (widgetConfigObj.isEmbedded != null) {
          this.isEmbedded = widgetConfigObj.isEmbedded;
        }
        if (widgetConfigObj.AssetLibraryId != null && widgetConfigObj.AssetLibraryId != 0) {
          this.LoadAsset('video', widgetConfigObj.AssetLibraryId);
        }

        if (widgetConfigObj.AssetLibraryId != 0 && widgetConfigObj.AssetId != 0) {

          this._http.get(this.baseURL + 'assetlibrary/asset/download?assetIdentifier=' + widgetConfigObj.AssetId, { responseType: "arraybuffer", observe: 'response' }).pipe(map(response => response))
            .subscribe(
              data => {
                let contentType = data.headers.get('Content-Type');
                let fileName = data.headers.get('x-filename');
                const blob = new Blob([data.body], { type: contentType });
                let objectURL = URL.createObjectURL(blob);
                this.videoPreviewSrc = this.sanitizer.sanitize(SecurityContext.RESOURCE_URL, this.sanitizer.bypassSecurityTrustResourceUrl(objectURL));
                var videoDiv = document.getElementById('videoPreviewDiv');
                if (videoDiv != undefined && videoDiv != null) {
                  if (videoDiv.hasChildNodes()) {
                    videoDiv.removeChild(document.getElementById('videoConfigPreviewSrc'));
                  }
                  var video = document.createElement('video');
                  video.id = 'videoConfigPreviewSrc';
                  video.style.height = "200px";
                  video.style.width = "75%";
                  video.controls = true;

                  var sourceTag = document.createElement('source');
                  sourceTag.setAttribute('src', this.videoPreviewSrc);
                  sourceTag.setAttribute('type', 'video/mp4');
                  video.appendChild(sourceTag);
                  videoDiv.appendChild(video);

                  video.load();
                  video.currentTime = 0;
                  video.play();
                }
              },
              error => {
                //$('.overlay').show();
              });
        }
      } else {
        this.VideoConfigForm.patchValue({
          vdoAssetLibrary: 0,
          vdoAsset: 0,
          vdoUrl: ''
        });
        this.markFormGroupUnTouched(this.VideoConfigForm);

        this.videoPreviewSrc = '';
        var videoDiv = document.getElementById('videoPreviewDiv');
        if (videoDiv.hasChildNodes()) {
          videoDiv.removeChild(document.getElementById('videoConfigPreviewSrc'));
        }
      }
    }
  }

  isCSAgentLogoConfigForm(widgetId, widgetItemCount) {
    this.isMasterSaveBtnDisabled = true;
    this.isCSAgentLogoConfig = true;
    this.staticHtmlWidgetId = widgetId;
    this.selectedWidgetItemCount = widgetItemCount;
    const records = this.widgetsGridsterItemArray.filter(x => x.WidgetId == this.staticHtmlWidgetId &&
      x.widgetItemCount == this.selectedWidgetItemCount);
    if (records.length != 0) {
      const widgetSetting = records[0].WidgetSetting;
      if (widgetSetting != null && widgetSetting != '' && this.testJSON(widgetSetting)) {
        const widgetConfigObj = JSON.parse(widgetSetting);
        this.StaticConfigForm.patchValue({
          staticHtml: widgetConfigObj.html
        });
      } else {
        this.StaticConfigForm.patchValue({
          staticHtml: ''
        });
        this.markFormGroupUnTouched(this.StaticConfigForm);
      }
    }
  }
  isCorporateSaverAgentMessageConfigForm(widgetId, widgetItemCount) {
    this.isMasterSaveBtnDisabled = true;
    this.isCorporateSaverAgentMessageConfig = true;
    this.staticHtmlWidgetId = widgetId;
    this.selectedWidgetItemCount = widgetItemCount;
    const records = this.widgetsGridsterItemArray.filter(x => x.WidgetId == this.staticHtmlWidgetId &&
      x.widgetItemCount == this.selectedWidgetItemCount);
    if (records.length != 0) {
      const widgetSetting = records[0].WidgetSetting;
      if (widgetSetting != null && widgetSetting != '' && this.testJSON(widgetSetting)) {
        const widgetConfigObj = JSON.parse(widgetSetting);
        this.StaticConfigForm.patchValue({
          staticHtml: widgetConfigObj.html
        });
      } else {
        this.StaticConfigForm.patchValue({
          staticHtml: ''
        });
        this.markFormGroupUnTouched(this.StaticConfigForm);
      }
    }
  }
  isCorporateSaverAgentAddressConfigForm(widgetId, widgetItemCount) {
    this.isMasterSaveBtnDisabled = true;
    this.isCorporateSaverAgentAddressConfig = true;
    this.staticHtmlWidgetId = widgetId;
    this.selectedWidgetItemCount = widgetItemCount;
    const records = this.widgetsGridsterItemArray.filter(x => x.WidgetId == this.staticHtmlWidgetId &&
      x.widgetItemCount == this.selectedWidgetItemCount);
    if (records.length != 0) {
      const widgetSetting = records[0].WidgetSetting;
      if (widgetSetting != null && widgetSetting != '' && this.testJSON(widgetSetting)) {
        const widgetConfigObj = JSON.parse(widgetSetting);
        this.StaticConfigForm.patchValue({
          staticHtml: widgetConfigObj.html
        });
      } else {
        this.StaticConfigForm.patchValue({
          staticHtml: ''
        });
        this.markFormGroupUnTouched(this.StaticConfigForm);
      }
    }
  }

  isCorporateSaverTransactionConfigForm(widgetId, widgetItemCount) {
    this.isMasterSaveBtnDisabled = true;
    this.isCorporateSaverTransactionConfig = true;
    this.staticHtmlWidgetId = widgetId;
    this.selectedWidgetItemCount = widgetItemCount;
    const records = this.widgetsGridsterItemArray.filter(x => x.WidgetId == this.staticHtmlWidgetId &&
      x.widgetItemCount == this.selectedWidgetItemCount);
    if (records.length != 0) {
      const widgetSetting = records[0].WidgetSetting;
      if (widgetSetting != null && widgetSetting != '' && this.testJSON(widgetSetting)) {
        const widgetConfigObj = JSON.parse(widgetSetting);
        this.StaticConfigForm.patchValue({
          staticHtml: widgetConfigObj.html
        });
      } else {
        this.StaticConfigForm.patchValue({
          staticHtml: ''
        });
        this.markFormGroupUnTouched(this.StaticConfigForm);
      }
    }
  }
  isCorporateSaverClientandAgentDetailsConfigForm(widgetId, widgetItemCount) {
    this.isMasterSaveBtnDisabled = true;
    this.isCorporateSaverClientandAgentDetailsConfig = true;
    this.staticHtmlWidgetId = widgetId;
    this.selectedWidgetItemCount = widgetItemCount;
    const records = this.widgetsGridsterItemArray.filter(x => x.WidgetId == this.staticHtmlWidgetId &&
      x.widgetItemCount == this.selectedWidgetItemCount);
    if (records.length != 0) {
      const widgetSetting = records[0].WidgetSetting;
      if (widgetSetting != null && widgetSetting != '' && this.testJSON(widgetSetting)) {
        const widgetConfigObj = JSON.parse(widgetSetting);
        this.StaticConfigForm.patchValue({
          staticHtml: widgetConfigObj.html
        });
      } else {
        this.StaticConfigForm.patchValue({
          staticHtml: ''
        });
        this.markFormGroupUnTouched(this.StaticConfigForm);
      }
    }
  }
  isCorporateSaverTableTotalConfigForm(widgetId, widgetItemCount) {
    this.isMasterSaveBtnDisabled = true;
    this.isCorporateSaverTableTotalConfig = true;
    this.staticHtmlWidgetId = widgetId;
    this.selectedWidgetItemCount = widgetItemCount;
    const records = this.widgetsGridsterItemArray.filter(x => x.WidgetId == this.staticHtmlWidgetId &&
      x.widgetItemCount == this.selectedWidgetItemCount);
    if (records.length != 0) {
      const widgetSetting = records[0].WidgetSetting;
      if (widgetSetting != null && widgetSetting != '' && this.testJSON(widgetSetting)) {
        const widgetConfigObj = JSON.parse(widgetSetting);
        this.StaticConfigForm.patchValue({
          staticHtml: widgetConfigObj.html
        });
      } else {
        this.StaticConfigForm.patchValue({
          staticHtml: ''
        });
        this.markFormGroupUnTouched(this.StaticConfigForm);
      }
    }
  }
  isCorporateAgentDetailsConfigForm(widgetId, widgetItemCount) {
    this.isMasterSaveBtnDisabled = true;
    this.isCorporateAgentDetailsConfig = true;
    this.staticHtmlWidgetId = widgetId;
    this.selectedWidgetItemCount = widgetItemCount;
    const records = this.widgetsGridsterItemArray.filter(x => x.WidgetId == this.staticHtmlWidgetId &&
      x.widgetItemCount == this.selectedWidgetItemCount);
    if (records.length != 0) {
      const widgetSetting = records[0].WidgetSetting;
      if (widgetSetting != null && widgetSetting != '' && this.testJSON(widgetSetting)) {
        const widgetConfigObj = JSON.parse(widgetSetting);
        this.StaticConfigForm.patchValue({
          staticHtml: widgetConfigObj.html
        });
      } else {
        this.StaticConfigForm.patchValue({
          staticHtml: ''
        });
        this.markFormGroupUnTouched(this.StaticConfigForm);
      }
    }
  }

  isStaticHtmlConfigForm(widgetId, widgetItemCount) {
    debugger;
    this.isMasterSaveBtnDisabled = true;
    this.isStaticHtmlConfig = true;
    this.staticHtmlWidgetId = widgetId;
    this.selectedWidgetItemCount = widgetItemCount;

    var records = this.widgetsGridsterItemArray.filter(x => x.WidgetId == this.staticHtmlWidgetId && x.widgetItemCount == this.selectedWidgetItemCount);
    if (records.length != 0) {
      var widgetSetting = records[0].WidgetSetting;
      if (widgetSetting != null && widgetSetting != '' && this.testJSON(widgetSetting)) {
        var widgetConfigObj = JSON.parse(widgetSetting);
        this.StaticConfigForm.patchValue({
          staticHtml: widgetConfigObj.html
        });
      } else {
        this.StaticConfigForm.patchValue({
          staticHtml: ''
        });
        this.markFormGroupUnTouched(this.StaticConfigForm);
      }
    }

  }

  isSegmentBasedContentConfigForm(widgetId, widgetItemCount) {
    this.isMasterSaveBtnDisabled = true;
    this.issegmentBasedContentConfig = true;
    this.segmentBasedContentWidgetId = widgetId;
    this.selectedWidgetItemCount = widgetItemCount;

    var records = this.widgetsGridsterItemArray.filter(x => x.WidgetId == this.segmentBasedContentWidgetId && x.widgetItemCount == this.selectedWidgetItemCount);
    if (records.length != 0) {
      var widgetSetting = records[0].WidgetSetting;
      if (widgetSetting != null && widgetSetting != '' && this.testJSON(widgetSetting)) {
        var widgetConfigObj = JSON.parse(widgetSetting);
        this.SegmentBasedContentForm.patchValue({
          segmentBasedContent: widgetConfigObj.html
        });
      } else {
        this.SegmentBasedContentForm.patchValue({
          segmentBasedContent: ''
        });
        this.markFormGroupUnTouched(this.SegmentBasedContentForm);
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
      imageUrl: [null, [Validators.pattern(this.validUrlRegexPattern)]],
      imageHeight: [null, [Validators.pattern(this.OnlyNumbersAllowed)]],
      imgAlign: [0]
    });

    this.VideoConfigForm = this.fb.group({
      vdoAssetLibrary: [null, [Validators.required]],
      vdoAsset: [null, [Validators.required]],
      vdoUrl: [null, [Validators.required, Validators.pattern(this.validUrlRegexPattern)]]
    });

    this.StaticConfigForm = this.fb.group({
      staticHtml: [null, [Validators.required]]
    });

    this.SegmentBasedContentForm = this.fb.group({
      SegmentId: 0,
      SegmentBasedContent: [null, [Validators.required]]
    });

    this.getAssetLibraries();

   // this.getSegments();

    if (this.pageEditModeOn && this.widgetsGridsterItemArray.length == 0) {
      this.getTemplate();
    } else {
      //this.getWidgetsByPageType();
      this.getStaticAndDynamicWidgets();
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
      itemResizeCallback: AddDashboardDesignerComponent.itemResize,
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
        enabled: true,
      },
      resizable: {
        enabled: true,
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

  }

  changedOptions() {
    if (this.options.api && this.options.api.optionsChanged) {
      this.options.api.optionsChanged();
    }
  }

  static itemResize(item: GridsterItem, itemComponent: GridsterItemComponentInterface): void {
    setTimeout(function () {
      window.dispatchEvent(new Event('resize'));
    }, 10);
  }

  removeItem($event, item) {
    const index: number = this.widgetsGridsterItemArray.indexOf(item);
    this.widgetsGridsterItemArray.splice(index, 1);
  }

  async getWidgetsByPageType() {
    let widgetService = this.injector.get(WidgetService);
    let searchParameter: any = {};
    searchParameter.PagingParameter = {};
    searchParameter.PagingParameter.PageIndex = Constants.DefaultPageIndex;
    searchParameter.PagingParameter.PageSize = Constants.DefaultPageSize;
    searchParameter.SortParameter = {};
    searchParameter.SortParameter.SortColumn = "WidgetName";
    searchParameter.SortParameter.SortOrder = Constants.Ascending;
    searchParameter.SearchMode = Constants.Contains;
    searchParameter.PageTypeId = this.PageTypeId;
    searchParameter.IsPageTypeDetailsRequired = false;
    debugger;
    this._http.post(this.baseURL + URLConfiguration.widgetGetUrl, searchParameter).subscribe(
      data => {
        this.widgetsArray = <any[]>data;
      },
      error => {
        this._messageDialogService.openDialogBox('Error', error.error.Message, Constants.msgBoxError);
      }
    );
  }

  async getStaticAndDynamicWidgets() {
    let dynamicWidgetService = this.injector.get(DynamicWidgetService);
    var response = await dynamicWidgetService.getStaticAndDynamicWidgets(this.PageTypeId);
    this.widgetsArray = <any[]>response;
  }

  //Back Functionality.
  backClicked() {
    this.navigateToListPage();
  }

  prevBtnClicked() {
    const router = this.injector.get(Router);
    let queryParams = {
      Routeparams: {
        passingparams: {
          "PageName": this.PageName,
          "PageTypeId": this.PageTypeId,
          "PageWidgetArray": JSON.stringify(this.widgetsGridsterItemArray),
          "StaticAndDynamicWidgetsArray": JSON.stringify(this.widgetsArray),
          "pageEditModeOn": this.pageEditModeOn,
          "BackgroundImageAssetId": this.BackgroundImageAssetId,
          "BackgroundImageURL": this.BackgroundImageURL,
          "BackgroundImageAssetLibraryId": this.BackgroundImageAssetLibraryId,
          "PageIdentifier": this.PageIdentifier
        }
      }
    }
    if (this.pageEditModeOn) {
      localStorage.setItem("pageEditRouteparams", JSON.stringify(queryParams))
      router.navigate(['pages', 'Edit']);
    } else {
      localStorage.setItem("pageAddRouteparams", JSON.stringify(queryParams))
      router.navigate(['pages', 'Add']);
    }
  }

  OnSaveBtnClicked() {
    let pageObject: any = {};
    pageObject.DisplayName = this.PageName;
    pageObject.PageTypeId = this.PageTypeId;
    pageObject.Identifier = this.PageIdentifier;
    pageObject.BackgroundImageAssetId = this.BackgroundImageAssetId;
    pageObject.BackgroundImageURL = this.BackgroundImageURL;

    if (this.pageEditModeOn) {
      pageObject.Version = this.pageVersion;
    }
    let pageWidgets: any[] = [];
    for (var i = 0; i < this.widgetsGridsterItemArray.length; i++) {
      let widgetsGridsterItem = this.widgetsGridsterItemArray[i];
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

  selectWidget(widgetId, widgetName) {
    let widgets = this.widgetsArray.filter(x => x.Identifier == widgetId && x.WidgetName == widgetName);
    if (widgets.length != 0) {
      let widget = widgets[0];
      let widgetItems = this.widgetsGridsterItemArray.filter(w => w.WidgetId == widget.Identifier && w.value == widget.WidgetName);
      if (widget.Instantiable == false && widgetItems.length > 0) {
        let message = "You can not add multiple times " + widget.DisplayName + " widget";
        this._messageDialogService.openDialogBox('Error', message, Constants.msgBoxError);
      } else {
        this.widgetItemCount++;
        if (widget.WidgetType == 'Static') {

          if (widget.WidgetName == "CustomerInformation") {
            return this.widgetsGridsterItemArray.push({
              cols: 7,
              rows: 3,
              y: 0,
              x: 0,
              component: CustomerInformationComponent,
              value: widget.WidgetName,
              WidgetId: widget.Identifier,
              widgetItemCount: this.widgetItemCount,
              WidgetSetting: '',
              WidgetType: widget.WidgetType,
              IsDynamicWidget: false
            })
          }
          else if (widget.WidgetName == "FSPDetails") {
            return this.widgetsGridsterItemArray.push({
              cols: 3,
              rows: 4,
              y: 0,
              x: 0,
              component: FSPDetailsComponent,
              value: widget.WidgetName,
              WidgetId: widget.Identifier,
              widgetItemCount: this.widgetItemCount,
              WidgetSetting: '',
              WidgetType: widget.WidgetType,
              IsDynamicWidget: false
            })
          }
          else if (widget.WidgetName == "AccountInformation") {
            return this.widgetsGridsterItemArray.push({
              cols: 3,
              rows: 4,
              y: 0,
              x: 0,
              component: AccountInformationComponent,
              value: widget.WidgetName,
              WidgetId: widget.Identifier,
              widgetItemCount: this.widgetItemCount,
              WidgetSetting: '',
              WidgetType: widget.WidgetType,
              IsDynamicWidget: false
            })
          }
          else if (widget.WidgetName == "Image") {
            return this.widgetsGridsterItemArray.push({
              cols: 4,
              rows: 3,
              y: 0,
              x: 0,
              component: ImageComponent,
              value: widget.WidgetName,
              WidgetId: widget.Identifier,
              widgetItemCount: this.widgetItemCount,
              WidgetSetting: '',
              WidgetType: widget.WidgetType,
              TempImageIdentifier: '' + widget.Identifier + this.widgetItemCount,
              IsDynamicWidget: false
            })
          }
          else if (widget.WidgetName == "Video") {
            return this.widgetsGridsterItemArray.push({
              cols: 4,
              rows: 3,
              y: 0,
              x: 0,
              component: VideoComponent,
              value: widget.WidgetName,
              WidgetId: widget.Identifier,
              widgetItemCount: this.widgetItemCount,
              WidgetSetting: '',
              WidgetType: widget.WidgetType,
              IsDynamicWidget: false
            })
          }
          else if (widget.WidgetName == "Summary") {
            return this.widgetsGridsterItemArray.push({
              cols: 6,
              rows: 3,
              y: 0,
              x: 0,
              component: SummaryAtGlanceComponent,
              value: widget.WidgetName,
              WidgetId: widget.Identifier,
              widgetItemCount: this.widgetItemCount,
              WidgetSetting: '',
              WidgetType: widget.WidgetType,
              IsDynamicWidget: false
            })
          }
          else if (widget.WidgetName == "SavingAvailableBalance") {
            return this.widgetsGridsterItemArray.push({
              cols: 3,
              rows: 2,
              y: 0,
              x: 0,
              component: SavingAvailableBalanceComponent,
              value: widget.WidgetName,
              WidgetId: widget.Identifier,
              widgetItemCount: this.widgetItemCount,
              WidgetSetting: '',
              WidgetType: widget.WidgetType,
              IsDynamicWidget: false
            })
          }
          else if (widget.WidgetName == "CurrentAvailableBalance") {
            return this.widgetsGridsterItemArray.push({
              cols: 3,
              rows: 2,
              y: 0,
              x: 0,
              component: CurrentAvailableBalanceComponent,
              value: widget.WidgetName,
              WidgetId: widget.Identifier,
              widgetItemCount: this.widgetItemCount,
              WidgetSetting: '',
              WidgetType: widget.WidgetType,
              IsDynamicWidget: false
            })
          }
          else if (widget.WidgetName == "CurrentTransaction") {
            return this.widgetsGridsterItemArray.push({
              cols: 12,
              rows: 4,
              y: 0,
              x: 0,
              component: TransactionDetailsComponent,
              value: widget.WidgetName,
              WidgetId: widget.Identifier,
              widgetItemCount: this.widgetItemCount,
              WidgetSetting: '',
              WidgetType: widget.WidgetType,
              IsDynamicWidget: false
            })
          }
          else if (widget.WidgetName == "SavingTransaction") {
            return this.widgetsGridsterItemArray.push({
              cols: 12,
              rows: 4,
              y: 0,
              x: 0,
              component: SavingTransactionDetailsComponent,
              value: widget.WidgetName,
              WidgetId: widget.Identifier,
              widgetItemCount: this.widgetItemCount,
              WidgetSetting: '',
              WidgetType: widget.WidgetType,
              IsDynamicWidget: false
            })
          }
          else if (widget.WidgetName == "SpendingTrend") {
            return this.widgetsGridsterItemArray.push({
              cols: 4,
              rows: 3,
              y: 0,
              x: 0,
              component: SpendindTrendsComponent,
              value: widget.WidgetName,
              WidgetId: widget.Identifier,
              widgetItemCount: this.widgetItemCount,
              WidgetSetting: '',
              IsDynamicWidget: false
            })
          }
          else if (widget.WidgetName == "ReminderaAndRecommendation") {
            return this.widgetsGridsterItemArray.push({
              cols: 4,
              rows: 3,
              y: 0,
              x: 0,
              component: ReminderAndRecommComponent,
              value: widget.WidgetName,
              WidgetId: widget.Identifier,
              widgetItemCount: this.widgetItemCount,
              WidgetSetting: '',
              WidgetType: widget.WidgetType,
              IsDynamicWidget: false
            })
          }
          else if (widget.WidgetName == "Top4IncomeSources") {
            return this.widgetsGridsterItemArray.push({
              cols: 4,
              rows: 2,
              y: 0,
              x: 0,
              component: TopIncomeSourcesComponent,
              value: widget.WidgetName,
              WidgetId: widget.Identifier,
              widgetItemCount: this.widgetItemCount,
              WidgetSetting: '',
              WidgetType: widget.WidgetType,
              IsDynamicWidget: false
            })
          }
          else if (widget.WidgetName == "SavingTrend") {
            return this.widgetsGridsterItemArray.push({
              cols: 4,
              rows: 3,
              y: 0,
              x: 0,
              component: SavingTrendsComponent,
              value: widget.WidgetName,
              WidgetId: widget.Identifier,
              widgetItemCount: this.widgetItemCount,
              WidgetSetting: '',
              WidgetType: widget.WidgetType,
              IsDynamicWidget: false
            })
          }
          else if (widget.WidgetName == "Analytics") {
            return this.widgetsGridsterItemArray.push({
              cols: 4,
              rows: 3,
              y: 0,
              x: 0,
              component: AnalyticsWidgetComponent,
              value: widget.WidgetName,
              WidgetId: widget.Identifier,
              widgetItemCount: this.widgetItemCount,
              WidgetSetting: '',
              WidgetType: widget.WidgetType,
              IsDynamicWidget: false
            })
          }
          //else if (widget.WidgetName == "CustomerDetails") {
          //  return this.widgetsGridsterItemArray.push({
          //    cols: 6,
          //    rows: 2,
          //    y: 0,
          //    x: 0,
          //    component: CustomerDetailsComponent,
          //    value: widget.WidgetName,
          //    WidgetId: widget.Identifier,
          //    widgetItemCount: this.widgetItemCount,
          //    WidgetSetting: '',
          //    WidgetType: widget.WidgetType,
          //    IsDynamicWidget: false
          //  })
          //}
          //else if (widget.WidgetName == "CorporateSaverAgentAddress") {
          //  return this.widgetsGridsterItemArray.push({
          //    cols: 6,
          //    rows: 2,
          //    y: 0,
          //    x: 0,
          //    component: CorporateSaverAgentAddressComponent,
          //    value: widget.WidgetName,
          //    WidgetId: widget.Identifier,
          //    widgetItemCount: this.widgetItemCount,
          //    WidgetSetting: '',
          //    WidgetType: widget.WidgetType,
          //    IsDynamicWidget: false
          //  })
          //}
          //else if (widget.WidgetName == "BranchDetails") {
          //  return this.widgetsGridsterItemArray.push({
          //    cols: 6,
          //    rows: 2,
          //    y: 0,
          //    x: 0,
          //    component: BankDetailsComponent,
          //    value: widget.WidgetName,
          //    WidgetId: widget.Identifier,
          //    widgetItemCount: this.widgetItemCount,
          //    WidgetSetting: '',
          //    WidgetType: widget.WidgetType,
          //    IsDynamicWidget: false
          //  })
          //}
          //else if (widget.WidgetName == "WealthBranchDetails") {
          //  return this.widgetsGridsterItemArray.push({
          //    cols: 6,
          //    rows: 2,
          //    y: 0,
          //    x: 0,
          //    component: WealthBankDetailsComponent,
          //    value: widget.WidgetName,
          //    WidgetId: widget.Identifier,
          //    widgetItemCount: this.widgetItemCount,
          //    WidgetSetting: '',
          //    WidgetType: widget.WidgetType,
          //    IsDynamicWidget: false
          //  })
          //}
          //else if (widget.WidgetName == "InvestmentPortfolioStatement") {
          //  return this.widgetsGridsterItemArray.push({
          //    cols: 12,
          //    rows: 2,
          //    y: 0,
          //    x: 0,
          //    component: InvestmentPortfolioStatementComponent,
          //    value: widget.WidgetName,
          //    WidgetId: widget.Identifier,
          //    widgetItemCount: this.widgetItemCount,
          //    WidgetSetting: '',
          //    WidgetType: widget.WidgetType,
          //    IsDynamicWidget: false
          //  })
          //}
          //else if (widget.WidgetName == "InvestmentWealthPortfolioStatement") {
          //  return this.widgetsGridsterItemArray.push({
          //    cols: 12,
          //    rows: 2,
          //    y: 0,
          //    x: 0,
          //    component: InvestmentWealthPortfolioStatementComponent,
          //    value: widget.WidgetName,
          //    WidgetId: widget.Identifier,
          //    widgetItemCount: this.widgetItemCount,
          //    WidgetSetting: '',
          //    WidgetType: widget.WidgetType,
          //    IsDynamicWidget: false
          //  })
          //}
          //else if (widget.WidgetName == "InvestorPerformance") {
          //  return this.widgetsGridsterItemArray.push({
          //    cols: 12,
          //    rows: 2,
          //    y: 0,
          //    x: 0,
          //    component: InvestorPerformanceComponent,
          //    value: widget.WidgetName,
          //    WidgetId: widget.Identifier,
          //    widgetItemCount: this.widgetItemCount,
          //    WidgetSetting: '',
          //    WidgetType: widget.WidgetType,
          //    IsDynamicWidget: false
          //  })
          //}
          //else if (widget.WidgetName == "WealthInvestorPerformance") {
          //  return this.widgetsGridsterItemArray.push({
          //    cols: 12,
          //    rows: 2,
          //    y: 0,
          //    x: 0,
          //    component: WealthInvestorPerformanceComponent,
          //    value: widget.WidgetName,
          //    WidgetId: widget.Identifier,
          //    widgetItemCount: this.widgetItemCount,
          //    WidgetSetting: '',
          //    WidgetType: widget.WidgetType,
          //    IsDynamicWidget: false
          //  })
          //}
          //else if (widget.WidgetName == "BreakdownOfInvestmentAccounts") {
          //  return this.widgetsGridsterItemArray.push({
          //    cols: 12,
          //    rows: 5,
          //    y: 0,
          //    x: 0,
          //    component: BreakdownOfInvestmentAccountsComponent,
          //    value: widget.WidgetName,
          //    WidgetId: widget.Identifier,
          //    widgetItemCount: this.widgetItemCount,
          //    WidgetSetting: '',
          //    WidgetType: widget.WidgetType,
          //    IsDynamicWidget: false
          //  })
          //}
          //else if (widget.WidgetName == "WealthBreakdownOfInvestmentAccounts") {
          //  return this.widgetsGridsterItemArray.push({
          //    cols: 12,
          //    rows: 5,
          //    y: 0,
          //    x: 0,
          //    component: WealthBreakdownOfInvestmentAccountsComponent,
          //    value: widget.WidgetName,
          //    WidgetId: widget.Identifier,
          //    widgetItemCount: this.widgetItemCount,
          //    WidgetSetting: '',
          //    WidgetType: widget.WidgetType,
          //    IsDynamicWidget: false
          //  })
          //}
          //else if (widget.WidgetName == "ExplanatoryNotes") {
          //  return this.widgetsGridsterItemArray.push({
          //    cols: 12,
          //    rows: 2,
          //    y: 0,
          //    x: 0,
          //    component: ExplanatoryNotesComponent,
          //    value: widget.WidgetName,
          //    WidgetId: widget.Identifier,
          //    widgetItemCount: this.widgetItemCount,
          //    WidgetSetting: '',
          //    WidgetType: widget.WidgetType,
          //    IsDynamicWidget: false
          //  })
          //}
          //else if (widget.WidgetName == "WealthExplanatoryNotes") {
          //  return this.widgetsGridsterItemArray.push({
          //    cols: 12,
          //    rows: 2,
          //    y: 0,
          //    x: 0,
          //    component: WealthExplanatoryNotesComponent,
          //    value: widget.WidgetName,
          //    WidgetId: widget.Identifier,
          //    widgetItemCount: this.widgetItemCount,
          //    WidgetSetting: '',
          //    WidgetType: widget.WidgetType,
          //    IsDynamicWidget: false
          //  })
          //}
          //else if (widget.WidgetName == "PersonalLoanDetail") {
          //  return this.widgetsGridsterItemArray.push({
          //    cols: 12,
          //    rows: 3,
          //    y: 0,
          //    x: 0,
          //    component: PersonalLoanDetailComponent,
          //    value: widget.WidgetName,
          //    WidgetId: widget.Identifier,
          //    widgetItemCount: this.widgetItemCount,
          //    WidgetSetting: '',
          //    WidgetType: widget.WidgetType,
          //    IsDynamicWidget: false
          //  })
          //}
          //else if (widget.WidgetName == "PersonalLoanTransaction") {
          //  return this.widgetsGridsterItemArray.push({
          //    cols: 12,
          //    rows: 2,
          //    y: 0,
          //    x: 0,
          //    component: PersonalLoanTransactionComponent,
          //    value: widget.WidgetName,
          //    WidgetId: widget.Identifier,
          //    widgetItemCount: this.widgetItemCount,
          //    WidgetSetting: '',
          //    WidgetType: widget.WidgetType,
          //    IsDynamicWidget: false
          //  })
          //}
          //else if (widget.WidgetName == "PersonalLoanPaymentDue") {
          //  return this.widgetsGridsterItemArray.push({
          //    cols: 12,
          //    rows: 2,
          //    y: 0,
          //    x: 0,
          //    component: PersonalLoanPaymentDueComponent,
          //    value: widget.WidgetName,
          //    WidgetId: widget.Identifier,
          //    widgetItemCount: this.widgetItemCount,
          //    WidgetSetting: '',
          //    WidgetType: widget.WidgetType,
          //    IsDynamicWidget: false
          //  })
          //}
          else if (widget.WidgetName == "SpecialMessage") {
            return this.widgetsGridsterItemArray.push({
              cols: 12,
              rows: 1,
              y: 0,
              x: 0,
              component: SpecialMessageComponent,
              value: widget.WidgetName,
              WidgetId: widget.Identifier,
              widgetItemCount: this.widgetItemCount,
              WidgetSetting: '',
              WidgetType: widget.WidgetType,
              IsDynamicWidget: false
            })
          }
          //else if (widget.WidgetName == "PL_InsuranceMessage") {
          //  return this.widgetsGridsterItemArray.push({
          //    cols: 12,
          //    rows: 2,
          //    y: 0,
          //    x: 0,
          //    component: PersonalLoanInsuranceMessageComponent,
          //    value: widget.WidgetName,
          //    WidgetId: widget.Identifier,
          //    widgetItemCount: this.widgetItemCount,
          //    WidgetSetting: '',
          //    WidgetType: widget.WidgetType,
          //    IsDynamicWidget: false
          //  })
          //}
          //else if (widget.WidgetName == "NedbankService") {
          //  return this.widgetsGridsterItemArray.push({
          //    cols: 12,
          //    rows: 3,
          //    y: 0,
          //    x: 0,
          //    component: NedbankServiceComponent,
          //    value: widget.WidgetName,
          //    WidgetId: widget.Identifier,
          //    widgetItemCount: this.widgetItemCount,
          //    WidgetSetting: '',
          //    WidgetType: widget.WidgetType,
          //    IsDynamicWidget: false
          //  })
          //}
          //else if (widget.WidgetName == "WealthNedbankService") {
          //  return this.widgetsGridsterItemArray.push({
          //    cols: 6,
          //    rows: 3,
          //    y: 0,
          //    x: 0,
          //    component: WealthNedbankServiceComponent,
          //    value: widget.WidgetName,
          //    WidgetId: widget.Identifier,
          //    widgetItemCount: this.widgetItemCount,
          //    WidgetSetting: '',
          //    WidgetType: widget.WidgetType,
          //    IsDynamicWidget: false
          //  })
          //}
          //else if (widget.WidgetName == "PersonalLoanTotalAmountDetail") {
          //  return this.widgetsGridsterItemArray.push({
          //    cols: 12,
          //    rows: 2,
          //    y: 0,
          //    x: 0,
          //    component: PersonalLoanTotalAmountDetailComponent,
          //    value: widget.WidgetName,
          //    WidgetId: widget.Identifier,
          //    widgetItemCount: this.widgetItemCount,
          //    WidgetSetting: '',
          //    WidgetType: widget.WidgetType,
          //    IsDynamicWidget: false
          //  })
          //}
          //else if (widget.WidgetName == "PersonalLoanAccountsBreakdown") {
          //  return this.widgetsGridsterItemArray.push({
          //    cols: 12,
          //    rows: 4,
          //    y: 0,
          //    x: 0,
          //    component: PersonalLoanAccountsBreakdownComponent,
          //    value: widget.WidgetName,
          //    WidgetId: widget.Identifier,
          //    widgetItemCount: this.widgetItemCount,
          //    WidgetSetting: '',
          //    WidgetType: widget.WidgetType,
          //    IsDynamicWidget: false
          //  })
          //}
          //else if (widget.WidgetName == "HomeLoanTotalAmountDetail") {
          //  return this.widgetsGridsterItemArray.push({
          //    cols: 12,
          //    rows: 2,
          //    y: 0,
          //    x: 0,
          //    component: HomeLoanTotalAmountDetailComponent,
          //    value: widget.WidgetName,
          //    WidgetId: widget.Identifier,
          //    widgetItemCount: this.widgetItemCount,
          //    WidgetSetting: '',
          //    WidgetType: widget.WidgetType,
          //    IsDynamicWidget: false
          //  })
          //}
          //else if (widget.WidgetName == "HomeLoanAccountsBreakdown") {
          //  return this.widgetsGridsterItemArray.push({
          //    cols: 12,
          //    rows: 10,
          //    y: 0,
          //    x: 0,
          //    component: HomeLoanAccountsBreakdownComponent,
          //    value: widget.WidgetName,
          //    WidgetId: widget.Identifier,
          //    widgetItemCount: this.widgetItemCount,
          //    WidgetSetting: '',
          //    WidgetType: widget.WidgetType,
          //    IsDynamicWidget: false
          //  })
          //}
          //else if (widget.WidgetName == "HomeLoanPaymentDueSpecialMsg") {
          //  return this.widgetsGridsterItemArray.push({
          //    cols: 12,
          //    rows: 1,
          //    y: 0,
          //    x: 0,
          //    component: HomeLoanPaymentDueSpecialMsgComponent,
          //    value: widget.WidgetName,
          //    WidgetId: widget.Identifier,
          //    widgetItemCount: this.widgetItemCount,
          //    WidgetSetting: '',
          //    WidgetType: widget.WidgetType,
          //    IsDynamicWidget: false
          //  })
          //}
          //else if (widget.WidgetName == "HomeLoanInstalmentDetail") {
          //  return this.widgetsGridsterItemArray.push({
          //    cols: 12,
          //    rows: 4,
          //    y: 0,
          //    x: 0,
          //    component: HomeLoanInstalmentDetailComponent,
          //    value: widget.WidgetName,
          //    WidgetId: widget.Identifier,
          //    widgetItemCount: this.widgetItemCount,
          //    WidgetSetting: '',
          //    WidgetType: widget.WidgetType,
          //    IsDynamicWidget: false
          //  })
          //}
          //else if (widget.WidgetName == "PortfolioCustomerDetails") {
          //  return this.widgetsGridsterItemArray.push({
          //    cols: 4,
          //    rows: 2,
          //    y: 0,
          //    x: 0,
          //    component: PortfolioCustomerDetailsComponent,
          //    value: widget.WidgetName,
          //    WidgetId: widget.Identifier,
          //    widgetItemCount: this.widgetItemCount,
          //    WidgetSetting: '',
          //    WidgetType: widget.WidgetType,
          //    IsDynamicWidget: false
          //  })
          //}
          //else if (widget.WidgetName == "CustomerAddressDetails") {
          //  return this.widgetsGridsterItemArray.push({
          //    cols: 4,
          //    rows: 2,
          //    y: 0,
          //    x: 0,
          //    component: PortfolioCustomerAddressDetailsComponent,
          //    value: widget.WidgetName,
          //    WidgetId: widget.Identifier,
          //    widgetItemCount: this.widgetItemCount,
          //    WidgetSetting: '',
          //    WidgetType: widget.WidgetType,
          //    IsDynamicWidget: false
          //  })
          //}
          //else if (widget.WidgetName == "ClientContactDetails") {
          //  return this.widgetsGridsterItemArray.push({
          //    cols: 4,
          //    rows: 2,
          //    y: 0,
          //    x: 0,
          //    component: PortfolioClientContactDetailsComponent,
          //    value: widget.WidgetName,
          //    WidgetId: widget.Identifier,
          //    widgetItemCount: this.widgetItemCount,
          //    WidgetSetting: '',
          //    WidgetType: widget.WidgetType,
          //    IsDynamicWidget: false
          //  })
          //}
          //else if (widget.WidgetName == "AccountSummary") {
          //  return this.widgetsGridsterItemArray.push({
          //    cols: 5,
          //    rows: 3,
          //    y: 0,
          //    x: 0,
          //    component: PortfolioAccountSummaryDetailsComponent,
          //    value: widget.WidgetName,
          //    WidgetId: widget.Identifier,
          //    widgetItemCount: this.widgetItemCount,
          //    WidgetSetting: '',
          //    WidgetType: widget.WidgetType,
          //    IsDynamicWidget: false
          //  })
          //}
          //else if (widget.WidgetName == "AccountAnalysis") {
          //  return this.widgetsGridsterItemArray.push({
          //    cols: 7,
          //    rows: 3,
          //    y: 0,
          //    x: 0,
          //    component: PortfolioAccountAnalysisComponent,
          //    value: widget.WidgetName,
          //    WidgetId: widget.Identifier,
          //    widgetItemCount: this.widgetItemCount,
          //    WidgetSetting: '',
          //    WidgetType: widget.WidgetType,
          //    IsDynamicWidget: false
          //  })
          //}
          //else if (widget.WidgetName == "PortfolioReminders") {
          //  return this.widgetsGridsterItemArray.push({
          //    cols: 6,
          //    rows: 3,
          //    y: 0,
          //    x: 0,
          //    component: PortfolioRemindersComponent,
          //    value: widget.WidgetName,
          //    WidgetId: widget.Identifier,
          //    widgetItemCount: this.widgetItemCount,
          //    WidgetSetting: '',
          //    WidgetType: widget.WidgetType,
          //    IsDynamicWidget: false
          //  })
          //}
          //else if (widget.WidgetName == "PortfolioNewsAlerts") {
          //  return this.widgetsGridsterItemArray.push({
          //    cols: 6,
          //    rows: 3,
          //    y: 0,
          //    x: 0,
          //    component: PortfolioNewsAlertsComponent,
          //    value: widget.WidgetName,
          //    WidgetId: widget.Identifier,
          //    widgetItemCount: this.widgetItemCount,
          //    WidgetSetting: '',
          //    WidgetType: widget.WidgetType,
          //    IsDynamicWidget: false
          //  })
          //}
          //else if (widget.WidgetName == "GreenbacksTotalRewardPoints") {
          //  return this.widgetsGridsterItemArray.push({
          //    cols: 4,
          //    rows: 2,
          //    y: 0,
          //    x: 0,
          //    component: GreenbacksTotalRewardPointsComponent,
          //    value: widget.WidgetName,
          //    WidgetId: widget.Identifier,
          //    widgetItemCount: this.widgetItemCount,
          //    WidgetSetting: '',
          //    WidgetType: widget.WidgetType,
          //    IsDynamicWidget: false
          //  })
          //}
          //else if (widget.WidgetName == "GreenbacksContactUs") {
          //  return this.widgetsGridsterItemArray.push({
          //    cols: 4,
          //    rows: 2,
          //    y: 0,
          //    x: 0,
          //    component: GreenbacksContactUsComponent,
          //    value: widget.WidgetName,
          //    WidgetId: widget.Identifier,
          //    widgetItemCount: this.widgetItemCount,
          //    WidgetSetting: '',
          //    WidgetType: widget.WidgetType,
          //    IsDynamicWidget: false
          //  })
          //}
          //else if (widget.WidgetName == "YTDRewardsPoints") {
          //  return this.widgetsGridsterItemArray.push({
          //    cols: 6,
          //    rows: 3,
          //    y: 0,
          //    x: 0,
          //    component: YTDRewardPointsComponent,
          //    value: widget.WidgetName,
          //    WidgetId: widget.Identifier,
          //    widgetItemCount: this.widgetItemCount,
          //    WidgetSetting: '',
          //    WidgetType: widget.WidgetType,
          //    IsDynamicWidget: false
          //  })
          //}
          //else if (widget.WidgetName == "PointsRedeemedYTD") {
          //  return this.widgetsGridsterItemArray.push({
          //    cols: 6,
          //    rows: 3,
          //    y: 0,
          //    x: 0,
          //    component: PointsRedeemedYTDComponent,
          //    value: widget.WidgetName,
          //    WidgetId: widget.Identifier,
          //    widgetItemCount: this.widgetItemCount,
          //    WidgetSetting: '',
          //    WidgetType: widget.WidgetType,
          //    IsDynamicWidget: false
          //  })
          //}
          //else if (widget.WidgetName == "ProductRelatedPointsEarned") {
          //  return this.widgetsGridsterItemArray.push({
          //    cols: 6,
          //    rows: 3,
          //    y: 0,
          //    x: 0,
          //    component: ProductRelatedPointsEarnedComponent,
          //    value: widget.WidgetName,
          //    WidgetId: widget.Identifier,
          //    widgetItemCount: this.widgetItemCount,
          //    WidgetSetting: '',
          //    WidgetType: widget.WidgetType,
          //    IsDynamicWidget: false
          //  })
          //}
          //else if (widget.WidgetName == "CategorySpendRewards") {
          //  return this.widgetsGridsterItemArray.push({
          //    cols: 6,
          //    rows: 3,
          //    y: 0,
          //    x: 0,
          //    component: CategorySpendRewardsComponent,
          //    value: widget.WidgetName,
          //    WidgetId: widget.Identifier,
          //    widgetItemCount: this.widgetItemCount,
          //    WidgetSetting: '',
          //    WidgetType: widget.WidgetType,
          //    IsDynamicWidget: false
          //  })
          //}
          else if (widget.WidgetName == "StaticHtml") {
            debugger;
            return this.widgetsGridsterItemArray.push({
              cols: 4,
              rows: 3,
              y: 0,
              x: 0,
              component: StaticHtmlComponent,
              value: widget.WidgetName,
              WidgetId: widget.Identifier,
              widgetItemCount: this.widgetItemCount,
              WidgetSetting: '',
              WidgetType: widget.WidgetType,
              TempImageIdentifier: '' + widget.Identifier + this.widgetItemCount,
              IsDynamicWidget: false
            })
          }
          else if (widget.WidgetName == "CSAgentLogo") {
            return this.widgetsGridsterItemArray.push({
              cols: 4,
              rows: 3,
              y: 0,
              x: 0,
              component: CSAgentLogoComponent,
              value: widget.WidgetName,
              WidgetId: widget.Identifier,
              widgetItemCount: this.widgetItemCount,
              WidgetSetting: '',
              WidgetType: widget.WidgetType,
              TempImageIdentifier: '' + widget.Identifier + this.widgetItemCount,
              IsDynamicWidget: false
            })
          }
          //else if (widget.WidgetName == "CorporateSaverAgentMessage") {
          //  return this.widgetsGridsterItemArray.push({
          //    cols: 4,
          //    rows: 3,
          //    y: 0,
          //    x: 0,
          //    component: CorporateSaverAgentMessageComponent,
          //    value: widget.WidgetName,
          //    WidgetId: widget.Identifier,
          //    widgetItemCount: this.widgetItemCount,
          //    WidgetSetting: '',
          //    WidgetType: widget.WidgetType,
          //    TempImageIdentifier: '' + widget.Identifier + this.widgetItemCount,
          //    IsDynamicWidget: false
          //  })
          //}
          //else if (widget.WidgetName == "CorporateSaverTransaction") {
          //  return this.widgetsGridsterItemArray.push({
          //    cols: 4,
          //    rows: 3,
          //    y: 0,
          //    x: 0,
          //    component: CorporateSaverTransactionComponent,
          //    value: widget.WidgetName,
          //    WidgetId: widget.Identifier,
          //    widgetItemCount: this.widgetItemCount,
          //    WidgetSetting: '',
          //    WidgetType: widget.WidgetType,
          //    TempImageIdentifier: '' + widget.Identifier + this.widgetItemCount,
          //    IsDynamicWidget: false
          //  })
          //}
          //else if (widget.WidgetName == "CorporateSaverTableTotal") {
          //  return this.widgetsGridsterItemArray.push({
          //    cols: 4,
          //    rows: 3,
          //    y: 0,
          //    x: 0,
          //    component: CorporateSaverTableTotalComponent,
          //    value: widget.WidgetName,
          //    WidgetId: widget.Identifier,
          //    widgetItemCount: this.widgetItemCount,
          //    WidgetSetting: '',
          //    WidgetType: widget.WidgetType,
          //    TempImageIdentifier: '' + widget.Identifier + this.widgetItemCount,
          //    IsDynamicWidget: false
          //  })
          //}
          //else if (widget.WidgetName == "CorporateSaverClientandAgentDetails") {
          //  return this.widgetsGridsterItemArray.push({
          //    cols: 4,
          //    rows: 3,
          //    y: 0,
          //    x: 0,
          //    component: CorporateSaverClientDetailsComponent,
          //    value: widget.WidgetName,
          //    WidgetId: widget.Identifier,
          //    widgetItemCount: this.widgetItemCount,
          //    WidgetSetting: '',
          //    WidgetType: widget.WidgetType,
          //    TempImageIdentifier: '' + widget.Identifier + this.widgetItemCount,
          //    IsDynamicWidget: false
          //  })
          //}
          //else if (widget.WidgetName == "CorporateAgentDetails") {
          //  return this.widgetsGridsterItemArray.push({
          //    cols: 4,
          //    rows: 3,
          //    y: 0,
          //    x: 0,
          //    component: CorporateAgentDetailsComponent,
          //    value: widget.WidgetName,
          //    WidgetId: widget.Identifier,
          //    widgetItemCount: this.widgetItemCount,
          //    WidgetSetting: '',
          //    WidgetType: widget.WidgetType,
          //    TempImageIdentifier: '' + widget.Identifier + this.widgetItemCount,
          //    IsDynamicWidget: false
          //  })
          //}
          else if (widget.WidgetName == "SegmentBasedContent") {
            return this.widgetsGridsterItemArray.push({
              cols: 4,
              rows: 3,
              y: 0,
              x: 0,
              component: SegmentBasedContentComponent,
              value: widget.WidgetName,
              WidgetId: widget.Identifier,
              widgetItemCount: this.widgetItemCount,
              WidgetSetting: '',
              WidgetType: widget.WidgetType,
              TempImageIdentifier: '' + widget.Identifier + this.widgetItemCount,
              IsDynamicWidget: false
            })
          }
        ////  else if (widget.WidgetName == "HomeLoanSummaryTaxPurpose") {
        ////    return this.widgetsGridsterItemArray.push({
        ////      cols: 12,
        ////      rows: 2,
        ////      y: 0,
        ////      x: 0,
        ////      component: HomeLoanSummaryTaxPurposeComponent,
        ////      value: widget.WidgetName,
        ////      WidgetId: widget.Identifier,
        ////      widgetItemCount: this.widgetItemCount,
        ////      WidgetSetting: '',
        ////      WidgetType: widget.WidgetType,
        ////      TempImageIdentifier: '' + widget.Identifier + this.widgetItemCount,
        ////      IsDynamicWidget: false
        ////    })
        ////  }
        ////  else if (widget.WidgetName == "HomeLoanInstalment") {
        ////    return this.widgetsGridsterItemArray.push({
        ////      cols: 12,
        ////      rows: 2,
        ////      y: 0,
        ////      x: 0,
        ////      component: HomeLoanInstalmentComponent,
        ////      value: widget.WidgetName,
        ////      WidgetId: widget.Identifier,
        ////      widgetItemCount: this.widgetItemCount,
        ////      WidgetSetting: '',
        ////      WidgetType: widget.WidgetType,
        ////      TempImageIdentifier: '' + widget.Identifier + this.widgetItemCount,
        ////      IsDynamicWidget: false
        ////    })
        ////  }
        ////  else if (widget.WidgetName == "WealthHomeLoanTotalAmountDetail") {
        ////    return this.widgetsGridsterItemArray.push({
        ////      cols: 12,
        ////      rows: 2,
        ////      y: 0,
        ////      x: 0,
        ////      component: WealthHomeLoanTotalAmountDetailComponent,
        ////      value: widget.WidgetName,
        ////      WidgetId: widget.Identifier,
        ////      widgetItemCount: this.widgetItemCount,
        ////      WidgetSetting: '',
        ////      WidgetType: widget.WidgetType,
        ////      IsDynamicWidget: false
        ////    })
        ////  }
        ////  else if (widget.WidgetName == "WealthHomeLoanAccountsBreakdown") {
        ////    return this.widgetsGridsterItemArray.push({
        ////      cols: 12,
        ////      rows: 10,
        ////      y: 0,
        ////      x: 0,
        ////      component: WealthHomeLoanAccountsBreakdownComponent,
        ////      value: widget.WidgetName,
        ////      WidgetId: widget.Identifier,
        ////      widgetItemCount: this.widgetItemCount,
        ////      WidgetSetting: '',
        ////      WidgetType: widget.WidgetType,
        ////      IsDynamicWidget: false
        ////    })
        ////  }
        ////  else if (widget.WidgetName == "WealthHomeLoanSummaryTaxPurpose") {
        ////    return this.widgetsGridsterItemArray.push({
        ////      cols: 12,
        ////      rows: 2,
        ////      y: 0,
        ////      x: 0,
        ////      component: WealthHomeLoanSummaryTaxPurposeComponent,
        ////      value: widget.WidgetName,
        ////      WidgetId: widget.Identifier,
        ////      widgetItemCount: this.widgetItemCount,
        ////      WidgetSetting: '',
        ////      WidgetType: widget.WidgetType,
        ////      TempImageIdentifier: '' + widget.Identifier + this.widgetItemCount,
        ////      IsDynamicWidget: false
        ////    })
        ////  }
        ////  else if (widget.WidgetName == "WealthHomeLoanInstalment") {
        ////    return this.widgetsGridsterItemArray.push({
        ////      cols: 12,
        ////      rows: 2,
        ////      y: 0,
        ////      x: 0,
        ////      component: WealthHomeLoanInstalmentComponent,
        ////      value: widget.WidgetName,
        ////      WidgetId: widget.Identifier,
        ////      widgetItemCount: this.widgetItemCount,
        ////      WidgetSetting: '',
        ////      WidgetType: widget.WidgetType,
        ////      TempImageIdentifier: '' + widget.Identifier + this.widgetItemCount,
        ////      IsDynamicWidget: false
        ////    })
        ////  }
        ////  else if (widget.WidgetName == "WealthHomeLoanBranchDetails") {
        ////    return this.widgetsGridsterItemArray.push({
        ////      cols: 6,
        ////      rows: 2,
        ////      y: 0,
        ////      x: 0,
        ////      component: WealthHomeLoanBankDetailsComponent,
        ////      value: widget.WidgetName,
        ////      WidgetId: widget.Identifier,
        ////      widgetItemCount: this.widgetItemCount,
        ////      WidgetSetting: '',
        ////      WidgetType: widget.WidgetType,
        ////      IsDynamicWidget: false
        ////    })
        ////  }
        ////  else if (widget.WidgetName == "MCAAccountSummary") {
        ////    return this.widgetsGridsterItemArray.push({
        ////      cols: 12,
        ////      rows: 3,
        ////      y: 0,
        ////      x: 0,
        ////      component: MCAAccountSummaryComponent,
        ////      value: widget.WidgetName,
        ////      WidgetId: widget.Identifier,
        ////      widgetItemCount: this.widgetItemCount,
        ////      WidgetSetting: '',
        ////      WidgetType: widget.WidgetType,
        ////      IsDynamicWidget: false
        ////    })
        ////  }
        ////  else if (widget.WidgetName == "MCATransaction") {
        ////    return this.widgetsGridsterItemArray.push({
        ////      cols: 12,
        ////      rows: 4,
        ////      y: 0,
        ////      x: 0,
        ////      component: MCATransactionComponent,
        ////      value: widget.WidgetName,
        ////      WidgetId: widget.Identifier,
        ////      widgetItemCount: this.widgetItemCount,
        ////      WidgetSetting: '',
        ////      WidgetType: widget.WidgetType,
        ////      IsDynamicWidget: false
        ////    })
        ////  }
        ////  else if (widget.WidgetName == "MCAVATAnalysis") {
        ////    return this.widgetsGridsterItemArray.push({
        ////      cols: 12,
        ////      rows: 2,
        ////      y: 0,
        ////      x: 0,
        ////      component: MCAVATAnalysisComponent,
        ////      value: widget.WidgetName,
        ////      WidgetId: widget.Identifier,
        ////      widgetItemCount: this.widgetItemCount,
        ////      WidgetSetting: '',
        ////      WidgetType: widget.WidgetType,
        ////      IsDynamicWidget: false
        ////    })
        ////  }
        ////  else if (widget.WidgetName == "WealthMCAAccountSummary") {
        ////    return this.widgetsGridsterItemArray.push({
        ////      cols: 12,
        ////      rows: 3,
        ////      y: 0,
        ////      x: 0,
        ////      component: WealthMCAAccountSummaryComponent,
        ////      value: widget.WidgetName,
        ////      WidgetId: widget.Identifier,
        ////      widgetItemCount: this.widgetItemCount,
        ////      WidgetSetting: '',
        ////      WidgetType: widget.WidgetType,
        ////      IsDynamicWidget: false
        ////    })
        ////  }
        ////  else if (widget.WidgetName == "WealthMCATransaction") {
        ////    return this.widgetsGridsterItemArray.push({
        ////      cols: 12,
        ////      rows: 4,
        ////      y: 0,
        ////      x: 0,
        ////      component: WealthMCATransactionComponent,
        ////      value: widget.WidgetName,
        ////      WidgetId: widget.Identifier,
        ////      widgetItemCount: this.widgetItemCount,
        ////      WidgetSetting: '',
        ////      WidgetType: widget.WidgetType,
        ////      IsDynamicWidget: false
        ////    })
        ////  }
        ////  else if (widget.WidgetName == "WealthMCAVATAnalysis") {
        ////    return this.widgetsGridsterItemArray.push({
        ////      cols: 12,
        ////      rows: 2,
        ////      y: 0,
        ////      x: 0,
        ////      component: WealthMCAVATAnalysisComponent,
        ////      value: widget.WidgetName,
        ////      WidgetId: widget.Identifier,
        ////      widgetItemCount: this.widgetItemCount,
        ////      WidgetSetting: '',
        ////      WidgetType: widget.WidgetType,
        ////      IsDynamicWidget: false
        ////    })
        ////  }
        ////  else if (widget.WidgetName == "WealthMCABranchDetails") {
        ////    return this.widgetsGridsterItemArray.push({
        ////      cols: 6,
        ////      rows: 2,
        ////      y: 0,
        ////      x: 0,
        ////      component: WealthMCABranchDetailsComponent,
        ////      value: widget.WidgetName,
        ////      WidgetId: widget.Identifier,
        ////      widgetItemCount: this.widgetItemCount,
        ////      WidgetSetting: '',
        ////      WidgetType: widget.WidgetType,
        ////      IsDynamicWidget: false
        ////    })
        ////  }
        }
        else {
          if (widget.WidgetType == 'Table') {
            return this.widgetsGridsterItemArray.push({
              cols: 6,
              rows: 3,
              y: 0,
              x: 0,
              component: SummaryAtGlanceComponent,
              value: widget.WidgetName,
              WidgetId: widget.Identifier,
              widgetItemCount: this.widgetItemCount,
              WidgetSetting: '',
              WidgetType: widget.WidgetType,
              IsDynamicWidget: true
            })
          }
          else if (widget.WidgetType == 'Form') {
            return this.widgetsGridsterItemArray.push({
              cols: 3,
              rows: 4,
              y: 0,
              x: 0,
              component: AccountInformationComponent,
              value: widget.WidgetName,
              WidgetId: widget.Identifier,
              widgetItemCount: this.widgetItemCount,
              WidgetSetting: '',
              WidgetType: widget.WidgetType,
              IsDynamicWidget: true
            })
          }
          else if (widget.WidgetType == 'BarGraph') {
            return this.widgetsGridsterItemArray.push({
              cols: 4,
              rows: 3,
              y: 0,
              x: 0,
              component: DynamicBarChartWidgetComponent,
              value: widget.WidgetName,
              WidgetId: widget.Identifier,
              widgetItemCount: this.widgetItemCount,
              WidgetSetting: '',
              WidgetType: widget.WidgetType,
              IsDynamicWidget: true
            })
          }
          else if (widget.WidgetType == 'LineGraph') {
            return this.widgetsGridsterItemArray.push({
              cols: 4,
              rows: 3,
              y: 0,
              x: 0,
              component: DynamicLineChartWidgetComponent,
              value: widget.WidgetName,
              WidgetId: widget.Identifier,
              widgetItemCount: this.widgetItemCount,
              WidgetSetting: '',
              WidgetType: widget.WidgetType,
              IsDynamicWidget: true
            })
          }
          else if (widget.WidgetType == 'PieChart') {
            return this.widgetsGridsterItemArray.push({
              cols: 4,
              rows: 3,
              y: 0,
              x: 0,
              component: DynamicPieChartWidgetComponent,
              value: widget.WidgetName,
              WidgetId: widget.Identifier,
              widgetItemCount: this.widgetItemCount,
              WidgetSetting: '',
              WidgetType: widget.WidgetType,
              IsDynamicWidget: true
            })
          }
          else if (widget.WidgetType == 'Html') {
            return this.widgetsGridsterItemArray.push({
              cols: 5,
              rows: 3,
              y: 0,
              x: 0,
              component: DynamicHhtmlComponent,
              value: widget.WidgetName,
              WidgetId: widget.Identifier,
              widgetItemCount: this.widgetItemCount,
              WidgetSetting: '',
              WidgetType: widget.WidgetType,
              IsDynamicWidget: true
            })
          }

        }
      }
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
    var response = await templateService.getTemplates(searchParameter);
    this.templateList = response.templateList;
    if (this.templateList.length == 0) {
      let message = "No record found";
      this._messageDialogService.openDialogBox('Error', message, Constants.msgBoxError).subscribe(data => {
        if (data == true) {
          this.getTemplate();
        }
      });
    } else {
      let template = this.templateList[0];
      this.PageName = this.PageName == '' ? template.DisplayName : this.PageName;
      this.PageTypeId = template.PageTypeId;
      this.PageIdentifier = template.Identifier;
      this.pageVersion = template.Version;
      //await this.getWidgetsByPageType();
      await this.getStaticAndDynamicWidgets();

      let pageWidgets: TemplateWidget[] = template.PageWidgets;
      if (pageWidgets.length != 0) {
        for (let i = 0; i < pageWidgets.length; i++) {
          this.widgetItemCount++;
          let gridsterItem: any = {};
          gridsterItem.x = pageWidgets[i].Xposition;
          gridsterItem.y = pageWidgets[i].Yposition;
          gridsterItem.cols = pageWidgets[i].Width;
          gridsterItem.rows = pageWidgets[i].Height;
          gridsterItem.WidgetId = pageWidgets[i].WidgetId;
          gridsterItem.WidgetName = pageWidgets[i].WidgetName;
          gridsterItem.WidgetSetting = pageWidgets[i].WidgetSetting;
          gridsterItem.widgetItemCount = this.widgetItemCount;
          gridsterItem.IsDynamicWidget = pageWidgets[i].IsDynamicWidget;
          gridsterItem.WidgetType = pageWidgets[i].IsDynamicWidget == false ? 'Static' : 'Dynamic';
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
    if (widgetType == 'Static') {
      if (widgetName == 'CustomerInformation') {
        gridObj.component = CustomerInformationComponent;
      }
      else if (widgetName == 'FSPDetails') {
        gridObj.component = FSPDetailsComponent;
      }
      else if (widgetName == 'AccountInformation') {
        gridObj.component = AccountInformationComponent;
      }
      else if (widgetName == 'Image') {
        gridObj.component = ImageComponent;
      }
      else if (widgetName == 'Video') {
        gridObj.component = VideoComponent;
      }
      else if (widgetName == 'Summary') {
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
      //else if (widgetName == 'CustomerDetails') {
      //  gridObj.component = CustomerDetailsComponent;
      //}
      //else if (widgetName == 'CorporateSaverAgentAddress') {
      //  gridObj.component = CorporateSaverAgentAddressComponent;
      //}
      //else if (widgetName == 'BranchDetails') {
      //  gridObj.component = BankDetailsComponent;
      //}
      //else if (widgetName == 'WealthBranchDetails') {
      //  gridObj.component = WealthBankDetailsComponent;
      //}
      //else if (widgetName == 'InvestmentPortfolioStatement') {
      //  gridObj.component = InvestmentPortfolioStatementComponent;
      //}
      //else if (widgetName == 'InvestmentWealthPortfolioStatement') {
      //  gridObj.component = InvestmentWealthPortfolioStatementComponent;
      //}
      //else if (widgetName == 'InvestorPerformance') {
      //  gridObj.component = InvestorPerformanceComponent;
      //}
      //else if (widgetName == 'WealthInvestorPerformance') {
      //  gridObj.component = WealthInvestorPerformanceComponent;
      //}
      //else if (widgetName == 'BreakdownOfInvestmentAccounts') {
      //  gridObj.component = BreakdownOfInvestmentAccountsComponent;
      //}
      //else if (widgetName == 'ExplanatoryNotes') {
      //  gridObj.component = ExplanatoryNotesComponent;
      //}
      //else if (widgetName == 'WealthExplanatoryNotes') {
      //  gridObj.component = WealthExplanatoryNotesComponent;
      //}
      //else if (widgetName == 'PersonalLoanDetail') {
      //  gridObj.component = PersonalLoanDetailComponent;
      //}
      //else if (widgetName == 'PersonalLoanTransaction') {
      //  gridObj.component = PersonalLoanTransactionComponent;
      //}
      //else if (widgetName == 'PersonalLoanPaymentDue') {
      //  gridObj.component = PersonalLoanPaymentDueComponent;
      //}
      else if (widgetName == 'SpecialMessage') {
        gridObj.component = SpecialMessageComponent;
      }
      //else if (widgetName == 'PL_InsuranceMessage') {
      //  gridObj.component = PersonalLoanInsuranceMessageComponent;
      //}
      //else if (widgetName == 'NedbankService') {
      //  gridObj.component = NedbankServiceComponent;
      //}
      //else if (widgetName == 'WealthNedbankService') {
      //  gridObj.component = WealthNedbankServiceComponent;
      //}
      //else if (widgetName == 'PersonalLoanTotalAmountDetail') {
      //  gridObj.component = PersonalLoanTotalAmountDetailComponent;
      //}
      //else if (widgetName == 'PersonalLoanAccountsBreakdown') {
      //  gridObj.component = PersonalLoanAccountsBreakdownComponent;
      //}
      //else if (widget.WidgetName == "HomeLoanTotalAmountDetail") {
      //  gridObj.component = HomeLoanTotalAmountDetailComponent;
      //}
      //else if (widget.WidgetName == "HomeLoanAccountsBreakdown") {
      //  gridObj.component = HomeLoanAccountsBreakdownComponent
      //}
      //else if (widget.WidgetName == "HomeLoanPaymentDueSpecialMsg") {
      //  gridObj.component = HomeLoanPaymentDueSpecialMsgComponent
      //}
      //else if (widget.WidgetName == "HomeLoanInstalmentDetail") {
      //  gridObj.component = HomeLoanInstalmentDetailComponent
      //}
      //else if (widget.WidgetName == "PortfolioCustomerDetails") {
      //  gridObj.component = PortfolioCustomerDetailsComponent
      //}
      //else if (widget.WidgetName == "CustomerAddressDetails") {
      //  gridObj.component = PortfolioCustomerAddressDetailsComponent
      //}
      //else if (widget.WidgetName == "ClientContactDetails") {
      //  gridObj.component = PortfolioClientContactDetailsComponent
      //}
      //else if (widget.WidgetName == "AccountSummary") {
      //  gridObj.component = PortfolioAccountSummaryDetailsComponent
      //}
      //else if (widget.WidgetName == "AccountAnalysis") {
      //  gridObj.component = PortfolioAccountAnalysisComponent
      //}
      //else if (widget.WidgetName == "PortfolioReminders") {
      //  gridObj.component = PortfolioRemindersComponent
      //}
      //else if (widget.WidgetName == "PortfolioNewsAlerts") {
      //  gridObj.component = PortfolioNewsAlertsComponent
      //}
      //else if (widget.WidgetName == "GreenbacksTotalRewardPoints") {
      //  gridObj.component = GreenbacksTotalRewardPointsComponent
      //}
      //else if (widget.WidgetName == "GreenbacksContactUs") {
      //  gridObj.component = GreenbacksContactUsComponent
      //}
      //else if (widget.WidgetName == "YTDRewardsPoints") {
      //  gridObj.component = YTDRewardPointsComponent
      //}
      //else if (widget.WidgetName == "PointsRedeemedYTD") {
      //  gridObj.component = PointsRedeemedYTDComponent
      //}
      //else if (widget.WidgetName == "ProductRelatedPointsEarned") {
      //  gridObj.component = ProductRelatedPointsEarnedComponent
      //}
      //else if (widget.WidgetName == "CategorySpendRewards") {
      //  gridObj.component = CategorySpendRewardsComponent
      //}
      else if (widget.WidgetName == "StaticHtml") {
        gridObj.component = StaticHtmlComponent
      }
      else if (widget.WidgetName == "CSAgentLogo") {
        gridObj.component = CSAgentLogoComponent
      }
      //else if (widget.WidgetName == "CorporateSaverAgentMessage") {
      //  gridObj.component = CorporateSaverAgentMessageComponent
      //}
      //else if (widget.WidgetName == "CorporateSaverTransaction") {
      //  gridObj.component = CorporateSaverTransactionComponent
      //}
      //else if (widget.WidgetName == "CorporateSaverClientandAgentDetails") {
      //  gridObj.component = CorporateSaverClientDetailsComponent
      //}
      //else if (widget.WidgetName == "CorporateSaverTableTotal") {
      //  gridObj.component = CorporateSaverTableTotalComponent
      //}
      //else if (widget.WidgetName == "CorporateAgentDetails") {
      //  gridObj.component = CorporateAgentDetailsComponent
      //}
      else if (widget.WidgetName == "SegmentBasedContent") {
        gridObj.component = SegmentBasedContentComponent
      }
      //else if (widget.WidgetName == "HomeLoanSummaryTaxPurpose") {
      //  gridObj.component = HomeLoanSummaryTaxPurposeComponent
      //}
      //else if (widget.WidgetName == "HomeLoanInstalment") {
      //  gridObj.component = HomeLoanInstalmentComponent
      //}
      //else if (widget.WidgetName == "WealthHomeLoanTotalAmountDetail") {
      //  gridObj.component = WealthHomeLoanTotalAmountDetailComponent
      //}
      //else if (widget.WidgetName == "WealthHomeLoanAccountsBreakdown") {
      //  gridObj.component = WealthHomeLoanAccountsBreakdownComponent
      //}
      //else if (widget.WidgetName == "WealthHomeLoanSummaryTaxPurpose") {
      //  gridObj.component = WealthHomeLoanSummaryTaxPurposeComponent
      //}
      //else if (widget.WidgetName == "WealthHomeLoanInstalment") {
      //  gridObj.component = WealthHomeLoanInstalmentComponent
      //}
      //else if (widget.WidgetName == "WealthHomeLoanBranchDetails") {
      //  gridObj.component = WealthHomeLoanBankDetailsComponent
      //}
      //else if (widget.WidgetName == "MCAAccountSummary") {
      //  gridObj.component = MCAAccountSummaryComponent
      //}
      //else if (widget.WidgetName == "MCATransaction") {
      //  gridObj.component = MCATransactionComponent
      //}
      //else if (widget.WidgetName == "MCAVATAnalysis") {
      //  gridObj.component = MCAVATAnalysisComponent
      //}
      //else if (widget.WidgetName == "WealthMCAAccountSummary") {
      //  gridObj.component = WealthMCAAccountSummaryComponent
      //}
      //else if (widget.WidgetName == "WealthMCATransaction") {
      //  gridObj.component = WealthMCATransactionComponent
      //}
      //else if (widget.WidgetName == "WealthMCAVATAnalysis") {
      //  gridObj.component = WealthMCAVATAnalysisComponent
      //}
      //else if (widget.WidgetName == "WealthMCABranchDetails") {
      //  gridObj.component = WealthMCABranchDetailsComponent
      //}
    }
    else {
      let dynaWidgets = this.widgetsArray.filter(item => item.Identifier == widget.WidgetId && item.WidgetName == widgetName && item.WidgetType != 'Static');
      widgetType = dynaWidgets[0].WidgetType;
      if (widgetType == 'Table') {
        gridObj.component = SummaryAtGlanceComponent;
      }
      else if (widgetType == 'Form') {
        gridObj.component = AccountInformationComponent;
      }
      else if (widgetType == 'LineGraph') {
        gridObj.component = DynamicLineChartWidgetComponent;
      }
      else if (widgetType == 'BarGraph') {
        gridObj.component = DynamicBarChartWidgetComponent;
      }
      else if (widgetType == 'PieChart') {
        gridObj.component = DynamicPieChartWidgetComponent;
      }
      else if (widgetType == 'Html') {
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

  getAssetLibraries() {
    let searchParameter: any = {};
    searchParameter.IsActive = true;
    searchParameter.PagingParameter = {};
    searchParameter.PagingParameter.PageIndex = Constants.DefaultPageIndex;
    searchParameter.PagingParameter.PageSize = Constants.DefaultPageSize;
    searchParameter.SortParameter = {};
    searchParameter.SortParameter.SortColumn = Constants.Name;
    searchParameter.SortParameter.SortOrder = Constants.Ascending;
    searchParameter.SearchMode = Constants.Contains;
    if (!this.pageEditModeOn) {
      this.uiLoader.start();
    }
    debugger;
    this._http.post(this.baseURL + URLConfiguration.assetLibraryGetUrl, searchParameter).subscribe(
      data => {
        if (!this.pageEditModeOn) {
          this.uiLoader.stop();
        }
        let records = <any[]>data;
        this.assetLibraryList.push(...records);
      },
      error => {
        if (!this.pageEditModeOn) {
          this.uiLoader.stop();
        }
        this._messageDialogService.openDialogBox('Error', error.error.Message, Constants.msgBoxError);
      }
    );
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
    } else {
      this.assetLibraryList.push(...records);
    }
  }

  onAssetLibrarySelected(event, type) {
    const value = event.target.value;
    this.imgAssetId = 0;
    this.vdoAssetId = 0;
    if (value == "0") {
      if (type == 'image') {
        this.imgAssetLibraryId = 0;
        this.imgAssetLibraryName = '';
        this.imageFormErrorObject.showAssetLibraryError = true;
      } else {
        this.vdoAssetLibraryId = 0;
        this.vdoAssetLibraryName = '';
        this.videoFormErrorObject.showAssetLibraryError = true;
      }
    }
    else {
      if (type == 'image') {
        this.imgAssetLibraryId = Number(value);
        this.imgAssetLibraryName = this.assetLibraryList.filter(x => x.Identifier == Number(event.target.value))[0].Name;
        this.imageFormErrorObject.showAssetLibraryError = false;
      } else {
        this.vdoAssetLibraryId = Number(value);
        this.vdoAssetLibraryName = this.assetLibraryList.filter(x => x.Identifier == Number(event.target.value))[0].Name;
        this.videoFormErrorObject.showAssetLibraryError = false;
      }
      this.LoadAsset(type, value);
    }
  }

  onAssetSelected(event, type) {
    const value = event.target.value;
    if (value == "0") {
      if (type == 'image') {
        this.imgAssetId = 0;
        this.imgAssetName = '';
        this.imageFormErrorObject.showAssetError = true;
      } else {
        this.vdoAssetId = 0;
        this.vdoAssetName = '';
        this.videoPreviewSrc = '';
        var videoDiv = document.getElementById('videoPreviewDiv');
        if (videoDiv.hasChildNodes()) {
          videoDiv.removeChild(document.getElementById('videoConfigPreviewSrc'));
        }
        this.videoFormErrorObject.showAssetError = true;
      }
    }
    else {
      var asset = this.assets.filter(x => x.Identifier == Number(event.target.value))[0];
      this.uiLoader.start();
      this._http.get(this.baseURL + 'assetlibrary/asset/download?assetIdentifier=' + asset.Identifier, { responseType: "arraybuffer", observe: 'response' }).pipe(map(response => response))
        .subscribe(
          data => {
            this.uiLoader.stop();
            let contentType = data.headers.get('Content-Type');
            let fileName = data.headers.get('x-filename');
            const blob = new Blob([data.body], { type: contentType });

            if (type == 'image') {
              let objectURL = URL.createObjectURL(blob);
              this.ImagePreviewSrc = this.sanitizer.bypassSecurityTrustResourceUrl(objectURL);
              this.imgAssetId = Number(value);
              this.imgAssetName = asset.Name;
              this.imageFormErrorObject.showAssetError = false;
            }
            else {
              let objectURL = URL.createObjectURL(blob);
              this.videoPreviewSrc = this.sanitizer.sanitize(SecurityContext.RESOURCE_URL, this.sanitizer.bypassSecurityTrustResourceUrl(objectURL));
              var videoDiv = document.getElementById('videoPreviewDiv');
              if (videoDiv != undefined && videoDiv != null) {
                if (videoDiv.hasChildNodes()) {
                  videoDiv.removeChild(document.getElementById('videoConfigPreviewSrc'));
                }

                var video = document.createElement('video');
                video.id = 'videoConfigPreviewSrc';
                video.style.height = "200px";
                video.style.width = "75%";

                var sourceTag = document.createElement('source');
                sourceTag.setAttribute('src', this.videoPreviewSrc);
                sourceTag.setAttribute('type', 'video/mp4');
                video.appendChild(sourceTag);
                videoDiv.appendChild(video);

                video.load();
                video.currentTime = 0;
                video.play();

              }
              this.vdoAssetId = Number(value);
              this.vdoAssetName = asset.Name;
              this.videoPreviewSrc = asset.FilePath;
              this.videoFormErrorObject.showAssetError = false;
            }
          },
          error => {
            this.uiLoader.stop();
            $('.overlay').show();
            this._messageDialogService.openDialogBox('Error', error.error.Message, Constants.msgBoxError);
          });
    }
  }

  LoadAsset(type, value): void {
    this.assets = [];
    this.assets.push({ 'Identifier': '0', 'Name': 'Select Asset' });

    let assetSearchParameter: AssetSearchParameter = new AssetSearchParameter();
    assetSearchParameter.AssetLibraryIdentifier = String(value);
    assetSearchParameter.IsDeleted = false;
    if (type == 'image') {
      assetSearchParameter.Extension = "jpg, png, jpeg";
      if (this.ImageConfigForm.controls.imgAsset == null || this.ImageConfigForm.controls.imgAsset.value == 0) {
        this.ImageConfigForm.patchValue({
          imgAsset: 0,
        });
      }
    } else {
      assetSearchParameter.Extension = "mp4";
      if (this.VideoConfigForm.controls.vdoAsset == null || this.VideoConfigForm.controls.vdoAsset.value == 0) {
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
    debugger;
    this._http.post(this.baseURL + URLConfiguration.assetGetUrl, assetSearchParameter).subscribe(
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
    if (actionFor == 'submit') {
      let imageConfig: any = {};
      imageConfig.AssetLibraryId = this.isPersonalizeImage == true ? 0 : this.imgAssetLibraryId;
      imageConfig.AssetLibrayName = this.isPersonalizeImage == true ? "" : this.imgAssetLibraryName;
      imageConfig.AssetId = this.isPersonalizeImage == true ? 0 : this.imgAssetId;
      imageConfig.AssetName = this.isPersonalizeImage == true ? "" : this.imgAssetName;
      imageConfig.SourceUrl = this.isPersonalizeImage == true ? "" : this.ImageConfigForm.value.imageUrl;
      imageConfig.Height = this.isPersonalizeImage == true ? "" : this.ImageConfigForm.value.imageHeight;
      imageConfig.Align = this.isPersonalizeImage == true ? "" : this.ImageConfigForm.value.imgAlign;
      imageConfig.isPersonalize = this.isPersonalizeImage;
      imageConfig.WidgetId = this.imageWidgetId;
      imageConfig.TempImageIdentifier = '' + this.imageWidgetId + this.selectedWidgetItemCount;
      //imageConfig.BlobObjectUrl = this.isPersonalizeImage == true ? "" : this.imageBlobObjectUrl;

      let oldItem = this.widgetsGridsterItemArray.filter(x => x.WidgetId == this.imageWidgetId && x.widgetItemCount == this.selectedWidgetItemCount)[0];
      let newItem = Object.assign({}, oldItem)
      newItem.WidgetSetting = JSON.stringify(imageConfig);
      const index: number = this.widgetsGridsterItemArray.indexOf(oldItem);
      this.widgetsGridsterItemArray.splice(index, 1);
      this.widgetsGridsterItemArray.push(newItem);
    }
    this.resetImageConfigForm();
    this.isImageConfig = !this.isImageConfig;
    this.imageWidgetId = 0;
    //this.imageBlobObjectUrl = '';
    this.selectedWidgetItemCount = 0;
    this.isMasterSaveBtnDisabled = false;
  }

  OnVideoConfigBtnClicked(actionFor) {
    if (actionFor == 'submit') {
      let videoConfig: any = {};
      videoConfig.AssetLibraryId = this.isPersonalize == true ? 0 : this.vdoAssetLibraryId;
      videoConfig.AssetLibrayName = this.isPersonalize == true ? "" : this.vdoAssetLibraryName;
      videoConfig.AssetId = this.isPersonalize == true ? 0 : this.vdoAssetId;
      videoConfig.AssetName = (this.isPersonalize == true || this.isEmbedded == true) ? "" : this.vdoAssetName;
      videoConfig.SourceUrl = (this.isPersonalize == false && this.isEmbedded == true) ? this.VideoConfigForm.value.vdoUrl : "";
      videoConfig.WidgetId = this.videoWidgetId;
      videoConfig.isPersonalize = this.isPersonalize;
      videoConfig.isEmbedded = this.isEmbedded;

      let oldItem = this.widgetsGridsterItemArray.filter(x => x.WidgetId == this.videoWidgetId && x.widgetItemCount == this.selectedWidgetItemCount)[0];
      let newItem = Object.assign({}, oldItem)
      newItem.WidgetSetting = JSON.stringify(videoConfig);
      const index: number = this.widgetsGridsterItemArray.indexOf(oldItem);
      this.widgetsGridsterItemArray.splice(index, 1);
      this.widgetsGridsterItemArray.push(newItem);
    }
    this.resetVideoConfigForm();
    this.isVideoConfig = !this.isVideoConfig;
    this.videoWidgetId = 0;
    this.videoBlobObjectUrl = '';
    this.selectedWidgetItemCount = 0;
    this.isMasterSaveBtnDisabled = false;
    let vid = <HTMLVideoElement>document.getElementById("videoConfigPreviewSrc");
    if (vid != undefined && vid != null) {
      vid.pause();
    }
  }

  OnStaticHtmlConfigBtnClicked(actionFor) {
    debugger;
    if (actionFor == 'submit') {
      let staticHtmlContent = this.StaticConfigForm.value['staticHtml'];
      let staticHtmlConfig: any = {};
      staticHtmlConfig.WidgetId = this.staticHtmlWidgetId;
      staticHtmlConfig.html = staticHtmlContent;
      let oldItem = this.widgetsGridsterItemArray.filter(x => x.WidgetId == this.staticHtmlWidgetId && x.widgetItemCount == this.selectedWidgetItemCount)[0];
      let newItem = Object.assign({}, oldItem)
      newItem.WidgetSetting = JSON.stringify(staticHtmlConfig);
      const index: number = this.widgetsGridsterItemArray.indexOf(oldItem);
      this.widgetsGridsterItemArray.splice(index, 1);
      this.widgetsGridsterItemArray.push(newItem);
    }
    this.isStaticHtmlConfig = !this.isStaticHtmlConfig;
    this.selectedWidgetItemCount = 0;
    this.isMasterSaveBtnDisabled = false;
  }

  OnSegmentBasedContentConfigBtnClicked(actionFor) {
    if (actionFor == 'submit') {
      let segmentId = this.SegmentBasedContentForm.value.SegmentId;
      let segmentBasedContent = this.SegmentBasedContentForm.value.SegmentBasedContent;
      let segmentBasedContentConfig: any = {};
      segmentBasedContentConfig.WidgetId = this.segmentBasedContentWidgetId;
      segmentBasedContentConfig.SegmentId = segmentId;
      segmentBasedContentConfig.Html = segmentBasedContent;


      let oldItem = this.widgetsGridsterItemArray.filter(x => x.WidgetId == this.segmentBasedContentWidgetId && x.widgetItemCount == this.selectedWidgetItemCount)[0];
      let newItem = Object.assign({}, oldItem)
      if (newItem.WidgetSetting === '')
        newItem.WidgetSetting = '[' + JSON.stringify(segmentBasedContentConfig) + ']';
      else {
        var setting = JSON.parse(newItem.WidgetSetting);
        var indexToRemove = 0;
        for (var i = 0; i < setting.length; i++) {
          if (segmentBasedContentConfig.SegmentId == setting[i].SegmentId) {
            indexToRemove = i;
            break;
          }
        }

        setting.splice(indexToRemove, 1);

        setting.push(segmentBasedContentConfig);
        newItem.WidgetSetting = JSON.stringify(setting);
      }
      const index: number = this.widgetsGridsterItemArray.indexOf(oldItem);
      this.widgetsGridsterItemArray.splice(index, 1);
      this.widgetsGridsterItemArray.push(newItem);
    }
    this.issegmentBasedContentConfig = !this.issegmentBasedContentConfig;
    this.selectedWidgetItemCount = 0;
    this.isMasterSaveBtnDisabled = false;
  }

  onSegmentSelected(event) {

    var selectedSegmentId = event.target.value;
    this.isMasterSaveBtnDisabled = true;
    this.issegmentBasedContentConfig = true;
    //this.segmentBasedContentWidgetId = widgetId;
    //this.selectedWidgetItemCount = widgetItemCount;

    var records = this.widgetsGridsterItemArray.filter(x => x.WidgetId == this.segmentBasedContentWidgetId && x.widgetItemCount == this.selectedWidgetItemCount);
    if (records.length != 0) {
      var widgetSetting = records[0].WidgetSetting;
      if (widgetSetting != null && widgetSetting != '' && this.testJSON(widgetSetting)) {
        var widgetConfigObj = JSON.parse(widgetSetting);
        var selectedItemVal = widgetConfigObj.filter(m => m.SegmentId == selectedSegmentId);
        if (selectedItemVal.length > 0) {
          this.SegmentBasedContentForm.patchValue({
            SegmentBasedContent: selectedItemVal[0].Html
          });
        }
        else {
          this.SegmentBasedContentForm.patchValue({
            SegmentBasedContent: ''
          });
        }
      } else {
        this.SegmentBasedContentForm.patchValue({
          SegmentBasedContent: ''
        });
        this.markFormGroupUnTouched(this.SegmentBasedContentForm);
      }
    }
  }

  getSegments() {
    if (!this.pageEditModeOn) {
      this.uiLoader.start();
    }

    this._http.get(this.baseURL + URLConfiguration.segmentGetUrl).subscribe(
      data => {
        if (!this.pageEditModeOn) {
          this.uiLoader.stop();
        }
        let records = <any[]>data;
        this.segmentList.push(...records);
      },
      error => {
        if (!this.pageEditModeOn) {
          this.uiLoader.stop();
        }
        this._messageDialogService.openDialogBox('Error', error.error.Message, Constants.msgBoxError);
      }
    );
  }

  resetImageConfigForm() {
    this.ImageConfigForm.patchValue({
      imgAssetLibrary: null,
      imgAsset: null,
      imageUrl: null,
      imageHeight: null,
      imgAlign: 0
    });
  }

  resetVideoConfigForm() {
    this.VideoConfigForm.patchValue({
      vdoAssetLibrary: null,
      vdoAsset: null,
      vdoUrl: null,
    });
  }

  pageDesignPreview() {
    let previewService = this.injector.get(PreviewDialogService);
    previewService.openPageDesignPreviewDialogBox(this.widgetsGridsterItemArray, this.BackgroundImageAssetId, this.BackgroundImageURL, this.PageTypeId);
  }
}
