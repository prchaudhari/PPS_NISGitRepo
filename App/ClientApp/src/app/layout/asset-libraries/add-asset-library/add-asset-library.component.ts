
import { Component, OnInit, Injector, ChangeDetectorRef, ViewChild, ElementRef, OnDestroy } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import * as $ from 'jquery';
import { Router, ActivatedRoute, NavigationEnd } from '@angular/router';
import { WindowRef } from '../../../core/services/window-ref.service';
import { ConfigConstants } from 'src/app/shared/constants/configConstants';
import { AssetLibrary, Asset, AssetLibrarySearchParameter, AssetSearchParameter } from '../asset-library';
import { SortParameter, SearchMode } from '../../../shared/models/commonmodel';
import { NgxUiLoaderService } from 'ngx-ui-loader';
import { Location } from '@angular/common';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { Constants } from 'src/app/shared/constants/constants';
import { MessageDialogService } from 'src/app/shared/services/mesage-dialog.service';
import { FormGroup, FormBuilder, Validators, FormControl, SelectControlValueAccessor, FormArray, ValidatorFn } from '@angular/forms';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { AssetLibraryService } from '../asset-library.service';
import { MatInput } from '@angular/material';
import { AssetSetting } from '../../asset-settings/asset-setting'
import { DomSanitizer, SafeResourceUrl, SafeUrl } from '@angular/platform-browser';
export interface ListElement {
  name: string;
  updatedby: string;
  date: string;
}

const List_Data: ListElement[] = [
  //{ name: 'Asset01.png', date: '01/02/2020', updatedby: 'User 01' },
  //{ name: 'Asset02.png', date: '01/02/2020', updatedby: 'User 02' },
  //{ name: 'Asset03.png', date: '01/02/2020', updatedby: 'User 03' },
  //{ name: 'Asset04.png', date: '01/02/2020', updatedby: 'User 04' },
  //{ name: 'Asset05.png', date: '01/02/2020', updatedby: 'User 05' },
  //{ name: 'Asset06.png', date: '01/02/2020', updatedby: 'User 06' },
  //{ name: 'Asset07.png', date: '01/02/2020', updatedby: 'User 07' }
];

@Component({
  selector: 'app-add-asset-library',
  templateUrl: './add-asset-library.component.html',
  styleUrls: ['./add-asset-library.component.scss'],

})
export class AddAssetLibraryComponent implements OnInit {

  public pageSize = 5;
  public currentPage = 0;
  public totalSize = 0;
  public updateOperationMode: boolean = false;
  public assetLibrary: AssetLibrary;
  public asset: Asset;
  public assets: Asset[];
  public FileExtension = [];
  public tab: number;
  public mode: string = "Add";
  public baseURL: string = ConfigConstants.BaseURL;
  public uploadAssetContainer: boolean;
  public isUploadAssets: boolean = false;
  public isCollapsedDetails: boolean = false;
  public isCollapsedAssets: boolean = true;
  public params;
  public isCheckAll: boolean = false
  public disableMultipleDelete = true;
  isAssetLibraryDetailSaveButtonDisable: boolean = false;
  isMultipleFileUploadEnable: boolean = true;
  fileToUpload: FileList = null;
  fileNameList: string[] = [];
  displayedColumns: string[] = ['name', 'updatedby', 'date', 'actions'];
  dataSource = new MatTableDataSource<any>();
  public image;
  public isImage = true;
  public assetLibraryFormErrorObject: any = {
    showAssetLibraryNameError: false
  };
  public assetFileTypeError = false;
  public assetFileSizeError = false;
  public assetFileWidthError = false;
  public assetFileHeightError = false;
  public assetFileWidthHeightError = false;
  public fileType = '';
  public assetLibraryFormGroup: FormGroup;
  fileSize;
  public isShowPreview = false;
  public array: any;
  public sortedAssetList: Asset[] = [];
  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
  @ViewChild(MatInput, { static: true }) multipleFileAssetUpload: ElementRef;
  @ViewChild(MatInput, { static: true }) video: ElementRef;

  public isNoRecord = true;
  public setting: AssetSetting = {
    Identifier: 0,
    ImageHeight: 0,
    ImageFileExtension: "",
    VideoFileExtension: "",
    ImageWidth: 0,
    ImageSize: 0,
    VideoSize: 0
  };
  public actualSetting: AssetSetting = {
    Identifier: 0,
    ImageHeight: 0,
    ImageFileExtension: "",
    VideoFileExtension: "",
    ImageWidth: 0,
    ImageSize: 0,
    VideoSize: 0
  };

  constructor(
    private _location: Location,
    private _window: WindowRef,
    private _router: Router,
    private _activatedRouter: ActivatedRoute,
    private _http: HttpClient,
    private _spinnerService: NgxUiLoaderService,
    private formbuilder: FormBuilder,
    private _messageDialogService: MessageDialogService,
    private assetLibraryService: AssetLibraryService,
    private injector: Injector,
    private sanitizer: DomSanitizer
  ) {
    this.assetLibrary = new AssetLibrary;
    this.asset = new Asset;
    this.tab = 1;
    this.uploadAssetContainer = false;
    let me = this;

    _router.events.subscribe(e => {
      if (e instanceof NavigationEnd) {
        if (e.url.includes('/assetlibrary/Add')) {
          this.updateOperationMode = false;
          localStorage.removeItem("assetLibraryparams");
        }
      }

    });

    if (localStorage.getItem("assetLibraryparams")) {
      this.updateOperationMode = true;
    } else {
      this.updateOperationMode = false;
    }

    _router.events.subscribe(e => {
      if (e instanceof NavigationEnd) {
        if (e.url.includes('/assetlibrary')) {
          //set passing parameters to localstorage.
          this.params = JSON.parse(localStorage.getItem('assetLibraryparams'));
          if (localStorage.getItem('assetLibraryparams')) {
            this.assetLibrary.Identifier = this.params.Routeparams.passingparams.AssetLibraryIdentifier;
            this.getAssetLibraryRecords('', false);

          }
        } else {
          localStorage.removeItem("assetLibraryparams");
        }
      }

    });

    this._window.nativeWindow.DownloadAsset = function (assetId: number): void {
      me.DownloadAsset(assetId);
    };
    this._window.nativeWindow.UpdateAsset = function (assetId: number): void {
      me.UpdateAsset(assetId);
    };
    this._window.nativeWindow.DeleteAsset = function (assetId: number): void {
      // $('.overlay').show();
      me.DeleteAsset(assetId);
    }
  }

  ngOnInit() {
    this.dataSource = new MatTableDataSource(List_Data);
    this.dataSource.sort = this.sort;
    this.assetLibraryFormGroup = this.formbuilder.group({
      assetLibraryName: [null, Validators.compose([Validators.required, Validators.minLength(2),
      Validators.maxLength(100)])],
      assetLibraryDescription: [null, Validators.compose([Validators.maxLength(500)])]

    });
    //to hide tooltip
    const paginatorIntl = this.paginator._intl;
    paginatorIntl.nextPageLabel = '';
    paginatorIntl.previousPageLabel = '';
    paginatorIntl.firstPageLabel = '';
    paginatorIntl.lastPageLabel = '';
    this.LoadAssetSettings();
  }

  ngAfterViewInit() {
    this.dataSource.paginator = this.paginator;
  }

  navigateToListPage() {
    this._router.navigate(['assetlibrary']);
  }

  private UpdateAsset(assetId: number): void {
    if (localStorage.getItem('RoleName') === 'Super Admin') {
      $('.overlay').show();
      //this._dialogService.addDialog(ConfirmComponent, {
      //  title: 'Alert',
      //  message: "Select Customer First",
      //  actionButtonText: "Close",
      //  showCancelButtun: false
      //}).subscribe(() => $('.overlay').hide());

      return;
    }

    this._router.navigate(['main/assetlibrary/updatessml', assetId]);
  }

  UploadAsset(): void {
    const requestFormData: FormData = new FormData();
    if (!this.assetLibrary.Identifier) {
      return;
    }

    if (this.fileToUpload && this.fileToUpload.length > 0) {
      this._spinnerService.start();
      var userid = localStorage.getItem('UserId')
      Array.prototype.forEach.call(this.fileToUpload, (file: File) => requestFormData.append('File', file));
      requestFormData.append('IsFolderUpload', this.isMultipleFileUploadEnable ? 'false' : 'true');
      requestFormData.append('AssetLibraryIdentifier', this.assetLibrary.Identifier ? this.assetLibrary.Identifier.toString() : null);
      requestFormData.append('LastUpdatedBy', userid);

      this._http.post(this.baseURL + 'assetlibrary/asset/upload', requestFormData).subscribe(
        data => {
          this._spinnerService.stop();
          let message: string = "Asset uploaded successfully.";
          let assetSearchParameter: AssetSearchParameter;
          assetSearchParameter = new AssetSearchParameter();
          assetSearchParameter.SortParameter.SortColumn = "Id";
          assetSearchParameter.AssetLibraryIdentifier = this.assetLibrary.Identifier ? this.assetLibrary.Identifier.toString() : null;
          this.LoadAsset(assetSearchParameter);
          $('.overlay').show();
          this.CloseUploadAssetContainer();
          this._messageDialogService.openDialogBox('Alert', message, Constants.msgBoxSuccess);
        },
        error => {
          this._spinnerService.stop();
          this._messageDialogService.openDialogBox('Error', error.error.Message, Constants.msgBoxError);

        });
    }
  }

  DownloadAsset(assetId: number): void {
    this._spinnerService.start();
    this._http.get(this.baseURL + 'assetlibrary/asset/download?assetIdentifier=' + assetId, { responseType: "arraybuffer", observe: 'response' }).pipe(map(response => response))
      .subscribe(
        data => {
          this._spinnerService.stop();
          let contentType = data.headers.get('Content-Type');
          let fileName = data.headers.get('x-filename');
          const blob = new Blob([data.body], { type: contentType });
          if (window.navigator.msSaveOrOpenBlob) {
            window.navigator.msSaveOrOpenBlob(blob, fileName);
          } else {
            var link = document.createElement('a');
            link.setAttribute("type", "hidden");
            link.download = fileName;
            link.href = window.URL.createObjectURL(blob);
            document.body.appendChild(link);
            link.click();
          }
        },
        error => {
          $('.overlay').show();
          this._messageDialogService.openDialogBox('Error', error.error.Message, Constants.msgBoxError);
          this._spinnerService.stop();
        });
  }
  closePreview() {
    let vid = <HTMLVideoElement>document.getElementById("videoPreview");

    vid.pause();
    this.image = '';
  }
  PreviewAsset(asset: Asset): void {
    var assetId = asset.Identifier;
    var fileType = asset.Name.split('.').pop();
    if (fileType == 'png' || fileType == 'jpeg' || fileType == 'jpg') {
      this.isImage = true;
    }
    else {
      this.isImage = false; ElementRef
    }
    var url = this.baseURL + "assets/" + this.assetLibrary.Identifier + "/" + asset.Name;
    this.image = url;
    if (!this.isImage) {
      document.getElementById('videoPreview').removeChild(document.getElementById('videoPreview').childNodes[0])
      var sourceTag = document.createElement('source');
      sourceTag.setAttribute('src', url);
      sourceTag.setAttribute('type', 'video/mp4');
      document.getElementById('videoPreview').appendChild(sourceTag);

      let vid = <HTMLVideoElement>document.getElementById("videoPreview");

      vid.load();
      vid.currentTime = 0;
      vid.play();
      //this.videoSource.nativeElement.setAttribute('src', url);
      //this.video.nativeElement.load();

    }
  }

  DeleteAsset(assetId: number): void {
    $('.overlay').show();
    let message = 'Are you sure, you want to delete this record?';
    this._messageDialogService.openConfirmationDialogBox('Confirm', message, Constants.msgBoxWarning).subscribe(async (isConfirmed) => {
      $('.overlay').hide();
      if (isConfirmed) {
        let assets: Asset[];
        assets = new Array<Asset>();
        let asset: Asset;
        asset = new Asset();
        asset.Identifier = assetId;
        assets.push(asset);

        this._http.post(this.baseURL + 'assetlibrary/asset/delete', assets).subscribe(
          data => {
            this._spinnerService.stop();
            let message: string;
            message = "Asset deleted successfully.";
            let assetSearchParameter: AssetSearchParameter;
            assetSearchParameter = new AssetSearchParameter();
            assetSearchParameter.SortParameter.SortColumn = "Id";
            assetSearchParameter.AssetLibraryIdentifier = this.assetLibrary.Identifier ? this.assetLibrary.Identifier.toString() : null;
            this.LoadAsset(assetSearchParameter);
            this._messageDialogService.openDialogBox('Alert', message, Constants.msgBoxSuccess);
            $('.overlay').show();

          },
          error => {
            $('.overlay').show();
            this._messageDialogService.openDialogBox('Error', error.error.Message, Constants.msgBoxError);
          });
      }
    });

    this._spinnerService.stop();
  }

  DeleteAll(): void {
    let assets: Asset[];
    assets = new Array<Asset>();
    let selectedIndexes = [];
    if (this.assetLibrary.Assets == null || this.assetLibrary.Assets == undefined || this.assetLibrary.Assets.length == 0) {
      this._messageDialogService.openDialogBox('Message', "Please select assets which you want to delete.", Constants.msgBoxError);

    }
    else {
      this.assetLibrary.Assets.forEach((item, index) => {
        if (item.IsChecked) {
          let asset: Asset;
          asset = new Asset();
          asset.Identifier = item.Identifier;
          assets.push(asset);
        }

      })


      $('.overlay').show();
      if (!assets.length) {
        this._messageDialogService.openDialogBox('Message', "Please select assets which you want to delete.", Constants.msgBoxError);
      } else {

        let message = 'Are you sure, you want to delete this record?';
        this._messageDialogService.openConfirmationDialogBox('Confirm', message, Constants.msgBoxWarning).subscribe(async (isConfirmed) => {
          $('.overlay').hide();
          if (isConfirmed) {
            this._spinnerService.start();
            this.isCheckAll = false;
            this._http.post(this.baseURL + 'assetlibrary/asset/delete', assets).subscribe(
              data => {
                this._spinnerService.stop();
                let message: string;
                message = "Asset deleted successfully.";
                let assetSearchParameter: AssetSearchParameter;
                assetSearchParameter = new AssetSearchParameter();
                assetSearchParameter.SortParameter.SortColumn = "Id";
                assetSearchParameter.AssetLibraryIdentifier = this.assetLibrary.Identifier ? this.assetLibrary.Identifier.toString() : null;
                this.LoadAsset(assetSearchParameter);
                this._messageDialogService.openDialogBox('Alert', message, Constants.msgBoxSuccess);
                $('.overlay').show();
              },
              error => {
                $('.overlay').show();
                this._messageDialogService.openDialogBox('Error', error.error.Message, Constants.msgBoxError);
              });
          }
        });

        this._spinnerService.stop();
      }
    }


  }
  getAssetDetails(): void {
    this.isCollapsedAssets = !this.isCollapsedAssets;
    if (!this.isCollapsedAssets) {
      if (this.assetLibrary.Identifier != null && this.assetLibrary.Identifier > 0) {
        if (this.assetLibrary.Assets == null) {
          let assetSearchParameter: AssetSearchParameter;
          assetSearchParameter = new AssetSearchParameter();
          assetSearchParameter.SortParameter.SortColumn = "Id";
          assetSearchParameter.AssetLibraryIdentifier = this.assetLibrary.Identifier ? this.assetLibrary.Identifier.toString() : null;
          this.LoadAsset(assetSearchParameter);
        }

      }

    }

  }

  LoadAsset(assetSearchParameter: AssetSearchParameter): void {
    let assets: Asset[];
    this._spinnerService.start();
    this._http.post(this.baseURL + 'assetlibrary/asset/list', assetSearchParameter).subscribe(
      data => {
        assets = <Asset[]>data;
        this.assets = assets;
        this.assetLibrary.Assets = this.assets;
        for (let i = 0; i < this.assetLibrary.Assets.length; i++) {

          this.assetLibrary.Assets[i].IsChecked = false;
        }
        if (this.assetLibrary.Assets == null || this.assetLibrary.Assets == undefined || this.assetLibrary.Assets.length == 0) {
          this.isNoRecord = true;
        }
        else {
          this.isNoRecord = false;

        }
        this.dataSource = new MatTableDataSource(this.assetLibrary.Assets);
        this.dataSource.sort = this.sort;
        this.dataSource.sort = this.sort;
        this.array = this.assetLibrary.Assets;
        this.totalSize = this.array.length;
        this.iterator();
        this._spinnerService.stop();
      },
      error => {
        $('.overlay').show();
        this._messageDialogService.openDialogBox('Error', error.error.Message, Constants.msgBoxError);
        this._spinnerService.stop();
      });
  }

  IsCheckAll(event): void {
    const value = event.checked;
    if (value) {
      this.isCheckAll = true;
      for (let i = 0; i < this.assetLibrary.Assets.length; i++) {

        this.assetLibrary.Assets[i].IsChecked = true;
      }
      this.dataSource = new MatTableDataSource(this.assetLibrary.Assets);
      this.disableMultipleDelete = false;

    }
    else {

      this.isCheckAll = false;
      for (let i = 0; i < this.assetLibrary.Assets.length; i++) {

        this.assetLibrary.Assets[i].IsChecked = false;
      }
      this.disableMultipleDelete = true;
      this.dataSource = new MatTableDataSource(this.assetLibrary.Assets);
    }
  }

  IsCheckItem(event, element): void {
    if (event.checked) {
      this.disableMultipleDelete = false;
      let itemIndex = 0;
      this.assetLibrary.Assets.forEach((item, index) => {
        if (item.Identifier == element.Identifier) {
          itemIndex = index;
        }
      })
      this.assetLibrary.Assets[itemIndex].IsChecked = true;

      let isdisable = false;
      this.assetLibrary.Assets.forEach((item, index) => {
        if (!item.IsChecked) {
          isdisable = true;
        }
      })
      if (!isdisable) {
        this.isCheckAll = true;
      }
    }
    else {
      this.isCheckAll = false;
      this.assetLibrary.Assets.forEach
      let itemIndex = 0;
      this.assetLibrary.Assets.forEach((item, index) => {
        if (item.Identifier == element.Identifier) {
          itemIndex = index;
        }
      });
      this.assetLibrary.Assets[itemIndex].IsChecked = false;

      let isdisable = true;
      this.assetLibrary.Assets.forEach((item, index) => {
        if (item.IsChecked) {
          isdisable = false;
        }
      })
      if (!isdisable) {
        this.disableMultipleDelete = false;
      }
      else {
        this.disableMultipleDelete = true;

      }
    }
  }

  CloseUploadAssetContainer(): void {
    this.uploadAssetContainer = false;
    this.assetFileTypeError = false;
    this.assetFileSizeError = false;
    this.assetFileWidthHeightError = false;
    this.assetFileWidthError = false;
    this.assetFileHeightError = false;
  }

  SetTab(tabNumber: number): void {
    this.tab = tabNumber;
  }

  ShowUploadAssetContainer(): void {
    this.multipleFileAssetUpload ? this.multipleFileAssetUpload.nativeElement.value = '' : null;
    if (this.updateOperationMode == false) {
      if (this.assetLibrary.Identifier <= 0 || this.assetLibrary.Identifier == null || this.assetLibrary.Identifier == undefined) {
        //  return true;
        this._messageDialogService.openDialogBox('Message', "Asset Library should be added first.", Constants.msgBoxSuccess);

      }
      else {
        this.fileToUpload = null;
        this.fileNameList.length = 0;
        this.uploadAssetContainer = true;
      }
    }
    else {
      this.fileToUpload = null;
      this.fileNameList.length = 0;
      this.uploadAssetContainer = true;
    }

  }

  disableUploadAsset(): boolean {
    if (this.updateOperationMode == false) {
      if (this.assetLibrary.Identifier <= 0 || this.assetLibrary.Identifier == null || this.assetLibrary.Identifier == undefined) {
        return true;
      }
      else {
        return false;
      }
    }
    else {
      return false;
    }
  }

  disableDeleteAsset(): boolean {
    if (this.disableMultipleDelete)
      return true;
    else
      return false;
  }

  setFileUploadMethod(value: string) {
    if (value === 'files') {
      // to clear file selections
      this.ShowUploadAssetContainer();
      this.isMultipleFileUploadEnable = true;
    } else {
      // to clear file selections
      this.ShowUploadAssetContainer();
      this.isMultipleFileUploadEnable = false;
    }
  }

  HandleFileUpload(e, multipleFileAssetUpload): void {

    var files = e.target.files;
    var file = files[0];

    var isValidFileType = false;
    for (var i = 0; i < this.FileExtension.length; i++) {
      if (this.FileExtension[i] == file.type) {
        isValidFileType = true;
      }
    }
    if (isValidFileType) {
      var imagePattern = /image-*/;
      var vedioPattern = /video-*/;
      if (file.type.match(imagePattern)) {
        if (file.size > this.actualSetting.ImageSize) {
          this.fileSize = this.setting.ImageSize
          this.assetFileSizeError = true;
          this.assetFileTypeError = false;
          this.fileNameList = [];
          multipleFileAssetUpload.value = "";
        }
        else {
          this.assetFileSizeError = false;

        }
        let img = new Image()
        img.src = window.URL.createObjectURL(e.target.files[0])
        img.onload = () => {
          console.log(img.width + "image " + img.height);
          console.log(this.actualSetting.ImageWidth + "image " + this.actualSetting.ImageHeight);
          var height = img.height;
          var width = img.width;

          if (height > this.actualSetting.ImageHeight && width > this.actualSetting.ImageWidth) {
            this.assetFileWidthHeightError = true;
            this.assetFileWidthError = false;
            this.assetFileHeightError = false;
            multipleFileAssetUpload.value = "";
          }
          else if (height > this.actualSetting.ImageHeight) {
            this.assetFileWidthHeightError = false;
            this.assetFileWidthError = false;
            this.assetFileHeightError = true;
            multipleFileAssetUpload.value = "";
          }
          else if (width > this.actualSetting.ImageWidth) {
            this.assetFileWidthHeightError = false;
            this.assetFileWidthError = true;
            this.assetFileHeightError = false;
            multipleFileAssetUpload.value = "";
          }
          else {
            this.assetFileWidthHeightError = false;
            this.assetFileWidthError = false;
            this.assetFileHeightError = false;
          }
          if (this.assetFileTypeError == false && this.assetFileSizeError == false &&
            this.assetFileWidthHeightError == false && this.assetFileHeightError == false && this.assetFileWidthError == false
          ) {
            this.assetFileTypeError = false;
            this.assetFileSizeError = false;
            this.assetFileWidthHeightError = false;
            this.assetFileWidthError = false;
            this.assetFileHeightError = false;
            this.fileNameList = Array.prototype.map.call(files, (file: File) => file.name);
            this.fileToUpload = files;
          }
          else {
            this.fileNameList = [];
          }
        }
        this.fileType = 'Image';

      }
      else if (file.type.match(vedioPattern)) {

        if (file.size > this.actualSetting.VideoSize) {
          this.fileSize = this.setting.VideoSize;
          this.fileType = 'Video';
          this.assetFileSizeError = true;
          this.assetFileTypeError = false;
          this.fileNameList = [];
          multipleFileAssetUpload.value = "";
        }
        else {
          this.assetFileSizeError = false;
        }
        if (this.assetFileTypeError == false && this.assetFileSizeError == false &&
          this.assetFileWidthHeightError == false && this.assetFileHeightError == false && this.assetFileWidthError == false
        ) {
          this.assetFileTypeError = false;
          this.assetFileSizeError = false;
          this.assetFileWidthHeightError = false;
          this.assetFileWidthError = false;
          this.assetFileHeightError = false;
          this.fileNameList = Array.prototype.map.call(files, (file: File) => file.name);
          this.fileToUpload = files;
        }
        else {
          this.fileNameList = [];
        }
      }
    }

    else {
      multipleFileAssetUpload.value = "";
      this.assetFileTypeError = true;
      this.assetFileSizeError = false;
      this.fileNameList = [];

    }
  }

  removeFileFromUpload(fileName: string): void {
    if (this.fileToUpload && this.fileToUpload.length > 0) {
      this.fileToUpload = Array.prototype.filter.call(this.fileToUpload, (file: File) => file.name != fileName);

      this.fileNameList = Array.prototype.map.call(this.fileToUpload, (file: File) => file.name);
    }
  }

  SaveAssetLibrary(): void {
    let action: string;
    let assetLibraries: AssetLibrary[];
    assetLibraries = new Array<AssetLibrary>();
    $('.overlay').show();
    let message: string;
    if (this.updateOperationMode == true || this.assetLibrary.Identifier > 0) {
      action = "assetlibrary/update";
      message = "Asset library updated successfully.";
    }
    else {
      action = "assetlibrary/add";
      message = "Asset library added successfully.";
    }
    this.assetLibrary.Name = this.assetLibraryFormGroup.value.assetLibraryName.trim();
    this.assetLibrary.Description = this.assetLibraryFormGroup.value.assetLibraryDescription;

    assetLibraries.push(this.assetLibrary);
    this.isAssetLibraryDetailSaveButtonDisable = true;
    this._spinnerService.start();
    this._http.post(this.baseURL + action, assetLibraries).subscribe(
      data => {
        this._spinnerService.stop();
        let result: boolean;
        result = <boolean>data;
        if (result) {
          this._messageDialogService.openDialogBox('Success', message, Constants.msgBoxSuccess);
          if (this.updateOperationMode == false) {
            this.getAssetLibraryRecords(this.assetLibrary.Name, true);
          }

        }
      },
      error => {
        this._messageDialogService.openDialogBox('Error', error.error.Message, Constants.msgBoxError);
        this._spinnerService.stop();
      });
  }

  async getAssetLibraryRecords(name, byName) {
    let assetLibraryService = this.injector.get(AssetLibraryService);
    let searchParameter: any = {};
    searchParameter.PagingParameter = {};
    searchParameter.PagingParameter.PageIndex = Constants.DefaultPageIndex;
    searchParameter.PagingParameter.PageSize = Constants.DefaultPageSize;
    searchParameter.SortParameter = {};
    searchParameter.SortParameter.SortColumn = Constants.Name;
    searchParameter.SortParameter.SortOrder = Constants.Ascending;
    searchParameter.SearchMode = Constants.Exact;

    if (byName) {
      searchParameter.Name = name;
      let assetLibrary = await assetLibraryService.getAssetLibrary(searchParameter);
      this.assetLibrary.Identifier = assetLibrary[0].Identifier;
    }
    else {
      searchParameter.Identifier = this.assetLibrary.Identifier;
      //searchParameter.IsAssetDataRequired = true;
      let assetLibrary = await assetLibraryService.getAssetLibrary(searchParameter);
      this.assetLibrary = assetLibrary[0];
      this.assetLibraryFormGroup.controls['assetLibraryName'].setValue(assetLibrary[0].Name);
      this.assetLibraryFormGroup.controls['assetLibraryDescription'].setValue(assetLibrary[0].Description);
      if (this.assetLibrary.Assets == null || this.assetLibrary.Assets == undefined || this.assetLibrary.Assets.length == 0) {
        this.isNoRecord = true;
      }
      else {
        this.isNoRecord = false;

      }
      if (this.assetLibrary.Assets != null) {
        for (let i = 0; i < this.assetLibrary.Assets.length; i++) {

          this.assetLibrary.Assets[i].IsChecked = false;
        }
        this.dataSource = new MatTableDataSource(this.assetLibrary.Assets);
        this.dataSource.sort = this.sort;
        this.dataSource.sort = this.sort;
        this.array = this.assetLibrary.Assets;
        this.totalSize = this.array.length;
        this.iterator();
      }

    }
  }

  Previous(): void {
    this.SetTab(1);
  }

  get assetLibraryName() {
    return this.assetLibraryFormGroup.get('assetLibraryName');
  }

  get assetLibraryDescription() {
    return this.assetLibraryFormGroup.get('assetLibraryDescription');
  }

  assetLibraryFormValidaton(): boolean {
    this.assetLibraryFormErrorObject.showAssetLibraryNameError = false;
    if (this.assetLibraryFormGroup.controls.assetLibraryName.invalid) {
      this.assetLibraryFormErrorObject.showAssetLibraryNameError = true;
      return false;
    }
    return true;
  }

  sortData(sort: MatSort) {
    const data = this.assetLibrary.Assets.slice();
    if (!sort.active || sort.direction === '') {
      this.sortedAssetList = data;
      return;
    }

    this.sortedAssetList = data.sort((a, b) => {
      const isAsc = sort.direction === 'asc';
      switch (sort.active) {
        case 'name': return compareStr(a.Name, b.Name, isAsc);
        case 'updatedby': return compareStr(a.LastUpdatedBy.FirstName == null ? '' : a.LastUpdatedBy.FirstName, b.LastUpdatedBy.FirstName == null ? '' : b.LastUpdatedBy.FirstName, isAsc);
        case 'date': return compareStr(a.LastUpdatedDate == null ? '' : a.LastUpdatedDate, b.LastUpdatedDate == null ? '' : b.LastUpdatedDate, isAsc);

        default: return 0;
      }
    });
    this.dataSource = new MatTableDataSource<Asset>(this.sortedAssetList);
    this.dataSource.sort = this.sort;
    this.array = this.sortedAssetList;
    this.totalSize = this.array.length;
    this.iterator();
  }
  public handlePage(e: any) {
    this.currentPage = e.pageIndex;
    this.pageSize = e.pageSize;
    this.iterator();
  }

  private iterator() {
    const end = (this.currentPage + 1) * this.pageSize;
    const start = this.currentPage * this.pageSize;
    const part = this.array.slice(start, end);
    this.dataSource = part;
    this.dataSource.sort = this.sort;
  }

  LoadAssetSettings(): void {

    var AssetSearchParameter;
    this._http.post(this.baseURL + 'AssetSetting/list', AssetSearchParameter).subscribe(
      data => {
        this.setting = data[0];

        //this.setting.ImageHeight = 0;
        //this.setting.ImageFileExtension = "";
        //this.setting.VideoFileExtension = "";
        //this.setting.ImageWidth = 0;
        this.actualSetting.ImageSize = this.setting.ImageSize * 1024 * 1024;
        this.actualSetting.VideoSize = this.setting.VideoSize * 1024 * 1024;
        this.actualSetting.ImageWidth = this.setting.ImageWidth * 37.7952755906;
        this.actualSetting.ImageHeight = this.setting.ImageHeight * 37.7952755906;
        this.FileExtension = this.setting.ImageFileExtension.split(",");
        var videoFileExtension = this.setting.VideoFileExtension.split(",");
        for (var i = 0; i < videoFileExtension.length; i++) {
          this.FileExtension.push(videoFileExtension[i]);
        }
        //image/jpeg,image/jpg,video/mp4
        for (var i = 0; i < this.FileExtension.length; i++) {
          if (this.FileExtension[i] == "png") {
            this.FileExtension[i] = "image/png";

          }
          else if (this.FileExtension[i] == "jpeg") {
            this.FileExtension[i] = "image/jpeg";

          }
          else if (this.FileExtension[i] == "mp4") {
            this.FileExtension[i] = "video/mp4";
          }
        }
        this.actualSetting.VideoFileExtension = this.FileExtension.join(",");

      },
      error => {
        $('.overlay').show();
        this._messageDialogService.openDialogBox('Error', error.error.Message, Constants.msgBoxError);
        this._spinnerService.stop();
      });
  }
}
function compareStr(a: string, b: string, isAsc: boolean) {
  return (a.toLowerCase() < b.toLowerCase() ? -1 : 1) * (isAsc ? 1 : -1);
}

function compareNumber(a: number, b: number, isAsc: boolean) {
  return (a < b ? -1 : 1) * (isAsc ? 1 : -1);
}
