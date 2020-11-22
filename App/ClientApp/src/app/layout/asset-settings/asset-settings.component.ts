import { AppSettings } from '../../appsettings';
import { Component, OnInit, Injector } from '@angular/core';
import { HttpClient, HttpEvent, HttpEventType } from '@angular/common/http';
import * as $ from 'jquery';
import { Router, ActivatedRoute } from '@angular/router';
import { ConfigConstants } from 'src/app/shared/constants/configConstants';
import { NgxUiLoaderService } from 'ngx-ui-loader';
import { Location } from '@angular/common';
import { LocalStorageService } from 'src/app/shared/services/local-storage.service';
import { Constants } from 'src/app/shared/constants/constants';
import { MessageDialogService } from 'src/app/shared/services/mesage-dialog.service';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { IDropdownSettings } from 'ng-multiselect-dropdown';
import { AssetSetting } from './asset-setting'
import { HttpClientService } from 'src/app/core/services/httpClient.service';

@Component({
  selector: 'app-asset-settings',
  templateUrl: './asset-settings.component.html',
  styleUrls: ['./asset-settings.component.scss']
})

export class AssetSettingsComponent implements OnInit {
  public isUploadAssets: boolean = false;
  public isCollapsedImage: boolean = true;
  public isCollapsedVideo: boolean = true;
  public baseURL: string = AppSettings.baseURL;
  //public onlyNumbers = '^[1-9]$';
  public onlyNumbers = "^(?=.*[1-9])[+]?([0-9]+(?:[\\.]\\d{1,2})?|\\.[0-9])$";
  imageForm: FormGroup;
  public isImageFileDropdownError = false;
  public isVideoFileDropdownError = false;

  constructor(private _location: Location,
    private _router: Router,
    private _activatedRouter: ActivatedRoute,
    private _http: HttpClient,
    private _spinnerService: NgxUiLoaderService,
    private formbuilder: FormBuilder,
    private _messageDialogService: MessageDialogService,
    private injector: Injector,
    private localstorageservice: LocalStorageService,
    private fb: FormBuilder) { }

  imagedropdownList = [];
  imageselectedItems = [];
  imagedropdownSettings: IDropdownSettings = {};

  videodropdownList = [];
  videoselectedItems = [];
  videodropdownSettings: IDropdownSettings = {};
  public setting: AssetSetting = {
    Identifier: 0,
    ImageHeight: 0,
    ImageFileExtension: "",
    VideoFileExtension: "",
    ImageWidth: 0,
    ImageSize: 0,
    VideoSize: 0
  };

  ngOnInit() {

    var userdata = this.localstorageservice.GetCurrentUser();
    if(userdata == null || userdata.RoleName != 'Tenant Admin') {
      this.localstorageservice.removeLocalStorageData();
      this._router.navigate(['login']);
    }

    this.imagedropdownList = [
      { id: 1, item_text: 'png' },
      { id: 2, item_text: 'jpeg' }
    ];

    this.imagedropdownSettings = {
      singleSelection: false,
      idField: 'id',
      textField: 'item_text',
      selectAllText: 'Select All',
      unSelectAllText: 'Un Select All',
      itemsShowLimit: 3,
      allowSearchFilter: false
    };

    this.videodropdownList = [
      { id: 1, item_text: 'mp4' },
    ];

    this.videodropdownSettings = {
      singleSelection: false,
      idField: 'id',
      textField: 'item_text',
      selectAllText: 'Select All',
      unSelectAllText: 'Un Select All',
      itemsShowLimit: 3,
      allowSearchFilter: false
    };

    this.imageForm = this.fb.group({
      assetImageHeight: [null, Validators.compose([Validators.required,
      Validators.pattern(this.onlyNumbers)
      ])],
      assetImageWidth: [null, Validators.compose([Validators.required,
      Validators.pattern(this.onlyNumbers)
      ])],
      assetImageSize: [null, Validators.compose([Validators.required,
      Validators.pattern(this.onlyNumbers)
      ])],
      assetVideoSize: [null, Validators.compose([Validators.required,
      Validators.pattern(this.onlyNumbers)
      ])],
      assetImageFile: [null],
      assetVideoFile: [null]
    })
    this.LoadAsset();
  }

  get assetImageHeight() {
    return this.imageForm.get('assetImageHeight');
  }

  get assetImageWidth() {
    return this.imageForm.get('assetImageWidth');
  }

  get assetImageSize() {
    return this.imageForm.get('assetImageSize');
  }

  get assetVideoSize() {
    return this.imageForm.get('assetVideoSize');
  }

  get assetVideoFile() {
    return this.imageForm.get('assetVideoFile');
  }

  get assetImageFile() {
    return this.imageForm.get('assetImageFile');
  }

  textBoxValueChanged(e): void {
    //console.log(e);
  }

  LoadAsset(): void {
    this._spinnerService.start();
    var AssetSearchParameter;
    this._http.post(this.baseURL + 'AssetSetting/list', AssetSearchParameter).subscribe(
      data => {
        this._spinnerService.stop();
        if(data != null && data[0] != undefined) {
          this.setting = <AssetSetting>data[0];
          this.imageForm.controls['assetVideoSize'].setValue(this.setting.VideoSize);
          this.imageForm.controls['assetImageSize'].setValue(this.setting.ImageSize);
          this.imageForm.controls['assetImageWidth'].setValue(this.setting.ImageWidth);
          this.imageForm.controls['assetImageHeight'].setValue(this.setting.ImageHeight);
          var imageFile = this.setting.ImageFileExtension.split(",");
          this.imageselectedItems = [];
          this.videoselectedItems = [];
          for (var i = 0; i < imageFile.length; i++) {
            var item = this.imagedropdownList.filter(x => x.item_text == imageFile[i]);
            if (item != null && item.length > 0) {
              this.imageselectedItems.push(item[0]);
            }
          }
          this.imageForm.controls['assetImageFile'].setValue(this.imageselectedItems);

          var videoFile = this.setting.VideoFileExtension.split(",");
          for (var i = 0; i < videoFile.length; i++) {
            var item = this.videodropdownList.filter(x => x.item_text == videoFile[i]);
            if (item != null && item.length > 0) {
              this.videoselectedItems.push(item[0]);
            }
          }
          this.imageForm.controls['assetVideoFile'].setValue(this.videoselectedItems);
        }
      },
      error => {
        $('.overlay').show();
        this._messageDialogService.openDialogBox('Error', error.error.Message, Constants.msgBoxError);
        this._spinnerService.stop();
      });
  }

  DisableSaveButton(): boolean {
    if (this.imageForm.invalid) {
      return true;
    }
    else if (this.videoselectedItems.length == 0) {
      return true;
    }
    else if (this.imageselectedItems.length == 0) {
      return true;
    }
    return false;
  }

  SaveAssetSettings(): void {
    this._spinnerService.start();
    this.setting.VideoSize = this.imageForm.value.assetVideoSize;
    this.setting.ImageSize = this.imageForm.value.assetImageSize;
    this.setting.ImageWidth = this.imageForm.value.assetImageWidth;
    this.setting.ImageHeight = this.imageForm.value.assetImageHeight;
    this.setting.ImageFileExtension = Array.prototype.map.call(this.imageselectedItems, (file: any) => file.item_text).join();
    this.setting.VideoFileExtension = Array.prototype.map.call(this.videoselectedItems, (file: any) => file.item_text).join();
    this.save(this.setting);
  }

  public async save(postData): Promise<void> {
    let httpClientService = this.injector.get(HttpClientService);
    let requestUrl = 'AssetSetting/Save';
    this._spinnerService.start();
    var data = [];
    data.push(postData);
    await httpClientService.CallHttp("POST", requestUrl, data).toPromise()
      .then((httpEvent: HttpEvent<any>) => {
        if (httpEvent.type == HttpEventType.Response) {
          this._spinnerService.stop();
          if (httpEvent["status"] === 200) {
            this._messageDialogService.openDialogBox('Message', "Asset configuration saved successfully", Constants.msgBoxSuccess);
          }
        }
      }, (error) => {
        this._messageDialogService.openDialogBox('Error', error.error.Message, Constants.msgBoxError);
        this._spinnerService.stop();
      });
  }

  onItemSelectImage(item: any) {
    var elements = this.imageselectedItems.filter(x => x.item_text == item.item_text);
    if (elements == null && elements.length == 0) {
      this.imageselectedItems.push(item);
      this.isImageFileDropdownError = false;
    }
  }

  onSelectAllImage(items: any) {
    this.imageselectedItems = [];
    for (var i = 0; i <= items.length; i++) {
      this.imageselectedItems.push(items[i]);
    }
    this.isImageFileDropdownError = false;
  }

  onItemDeSelectImage(item: any) {
    this.imageselectedItems.push(item);
    this.imageselectedItems = this.imageselectedItems.filter(x => x.item_text != item.item_text);
    if (this.imageselectedItems.length == 0) {
      this.isImageFileDropdownError = true;
    }
  }

  onItemDeSelectAllImage(item: any) {
    this.imageselectedItems = [];
    this.isImageFileDropdownError = true;
  }

  onItemSelectVideo(item: any) {
    this.videoselectedItems.push(item);
    this.isVideoFileDropdownError = false;
    var elements = this.videoselectedItems.filter(x => x.item_text == item.item_text);
    if (elements == null && elements.length == 0) {
      this.videoselectedItems.push(item);
      this.isVideoFileDropdownError = false;
    }
  }

  onSelectAllVideo(items: any) {
    this.videoselectedItems = [];
    for (var i = 0; i <= items.length; i++) {
      this.videoselectedItems.push(items[i]);
    }
    this.isVideoFileDropdownError = false;
  }

  onItemDeSelectVideo(item: any) {
    this.videoselectedItems = this.videoselectedItems.filter(x => x.item_text != item.item_text);
    if (this.videoselectedItems.length == 0) {
      this.isVideoFileDropdownError = true;
    }
  }

  onItemDeSelectAllVideo(item: any) {
    this.videoselectedItems = [];
    this.isVideoFileDropdownError = true;
  }

  navigateToListPage() {
    this._location.back();
  }

}
