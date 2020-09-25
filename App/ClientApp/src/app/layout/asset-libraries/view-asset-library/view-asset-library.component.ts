import { Component, OnInit, Injector, ChangeDetectorRef, ViewChild, AfterViewInit, OnDestroy, ElementRef, SecurityContext } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import * as $ from 'jquery';
import { Router, ActivatedRoute, NavigationEnd } from '@angular/router';
import { WindowRef } from '../../../core/services/window-ref.service';
import { ConfigConstants } from '../../../shared/constants/configConstants';
import { AssetLibrary, Asset } from '../asset-library';
import { NgxUiLoaderService } from 'ngx-ui-loader';
import { Location } from '@angular/common';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { Constants } from 'src/app/shared/constants/constants';
import { MessageDialogService } from 'src/app/shared/services/mesage-dialog.service';
import { FormGroup, FormBuilder } from '@angular/forms';
import { map } from 'rxjs/operators';
import { AssetLibraryService } from '../asset-library.service';
import { DomSanitizer } from '@angular/platform-browser';

export interface ListElement {
  name: string;
  updatedby: string;
  date: string;
}

const List_Data: ListElement[] = [

];

@Component({
  selector: 'app-view-asset-library',
  templateUrl: './view-asset-library.component.html',
  styleUrls: ['./view-asset-library.component.scss']
})
export class ViewAssetLibraryComponent implements OnInit {

  public pageSize = 5;
  public currentPage = 0;
  public totalSize = 0;
  public assetLibrary: AssetLibrary;
  public asset: Asset;
  public assets: Asset[];
  public array: any;
  public sortedAssetLibraryList: Asset[] = [];
  public userClaimsRolePrivilegeOperations: any[] = [];
  public image;
  public isImage = true;
  public params;
  public baseURL = ConfigConstants.BaseURL;
  displayedColumns: string[] = ['name', 'updatedby', 'date', 'actions'];
  dataSource = new MatTableDataSource<any>();

  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;

  ngOnInit() {

    this.dataSource = new MatTableDataSource(List_Data);
    this.dataSource.sort = this.sort;

    //to hide tooltip
    const paginatorIntl = this.paginator._intl;
    paginatorIntl.nextPageLabel = '';
    paginatorIntl.previousPageLabel = '';
    paginatorIntl.firstPageLabel = '';
    paginatorIntl.lastPageLabel = '';
    var userClaimsDetail = JSON.parse(localStorage.getItem('userClaims'));
    if (userClaimsDetail) {
      this.userClaimsRolePrivilegeOperations = userClaimsDetail.Privileges;
    }
    else {
      this.userClaimsRolePrivilegeOperations = [];
    }
    this.DataFormat = localStorage.getItem('DateFormat');
    
    this.params = JSON.parse(localStorage.getItem('assetLibraryparams'));
    if (localStorage.getItem('assetLibraryparams')) {
      this.assetLibrary.Identifier = this.params.Routeparams.passingparams.AssetLibraryIdentifier;
      this.getAssetLibraryRecords();
    }
  }
  ngAfterViewInit() {
    this.dataSource.paginator = this.paginator;
  }

  public isCollapsedDetails: boolean = false;
  public isCollapsedAssets: boolean = true;
  public isNoRecord = false;

  navigateToListPage() {
    this._location.back();
  }
  public DataFormat;
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
    private router: Router,
    private sanitizer: DomSanitizer
  ) {
    this.assetLibrary = new AssetLibrary;
    this.asset = new Asset;
    let me = this;
    _router.events.subscribe(e => {
      if (e instanceof NavigationEnd) {
        if (e.url.includes('/assetlibrary/Add')) {
          localStorage.removeItem("assetLibraryparams");
        }
      }
    });
   
    _router.events.subscribe(e => {
      if (e instanceof NavigationEnd) {
        if (e.url.includes('/assetlibrary')) {
          //set passing parameters to localstorage.
          this.params = JSON.parse(localStorage.getItem('assetLibraryparams'));
          if (localStorage.getItem('assetLibraryparams')) {
            this.assetLibrary.Identifier = this.params.Routeparams.passingparams.AssetLibraryIdentifier;
            //this.getAssetLibraryRecords();
          }
        } else {
          localStorage.removeItem("assetLibraryparams");
        }
      }
    });
    this._window.nativeWindow.DownloadAsset = function (assetId: number): void {
      me.DownloadAsset(assetId);
    };
  }

  async getAssetLibraryRecords() {
    let assetLibraryService = this.injector.get(AssetLibraryService);
    let searchParameter: any = {};
    searchParameter.PagingParameter = {};
    searchParameter.PagingParameter.PageIndex = Constants.DefaultPageIndex;
    searchParameter.PagingParameter.PageSize = Constants.DefaultPageSize;
    searchParameter.SortParameter = {};
    searchParameter.SortParameter.SortColumn = Constants.Name;
    searchParameter.SortParameter.SortOrder = Constants.Ascending;
    searchParameter.SearchMode = Constants.Exact;
    searchParameter.Identifier = this.assetLibrary.Identifier;
    searchParameter.IsAssetDataRequired = true;
    let response = await assetLibraryService.getAssetLibrary(searchParameter);
    this.assetLibrary = response.assestLibraryList[0];
    if (this.assetLibrary.Assets == null || this.assetLibrary.Assets == undefined || this.assetLibrary.Assets.length == 0) {
      this.isNoRecord = true;
    }
    else {
      this.isNoRecord = false;
    }
    this.dataSource = new MatTableDataSource<Asset>(this.assetLibrary.Assets);
    //this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
    this.array = this.assetLibrary.Assets;
    this.totalSize = this.array.length;
    this.iterator();
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
    var fileType = asset.Name.split('.').pop();
    if (fileType == 'png' || fileType == 'jpeg' || fileType == 'jpg') {
      this.isImage = true;
    }
    else {
      this.isImage = false;
    }
    this._spinnerService.start();
    this._http.get(this.baseURL + 'assetlibrary/asset/download?assetIdentifier=' + asset.Identifier, { responseType: "arraybuffer", observe: 'response' }).pipe(map(response => response))
      .subscribe(
        data => {
          this._spinnerService.stop();
          let contentType = data.headers.get('Content-Type');
          let fileName = data.headers.get('x-filename');
          const blob = new Blob([data.body], { type: contentType });
          if (this.isImage) {
            let objectURL = URL.createObjectURL(blob);
            this.image = this.sanitizer.bypassSecurityTrustUrl(objectURL);
          }
          else {
            let objectURL = URL.createObjectURL(blob);
            this.image = this.sanitizer.sanitize(SecurityContext.RESOURCE_URL, this.sanitizer.bypassSecurityTrustResourceUrl(objectURL)); //this.sanitizer.bypassSecurityTrustUrl(objectURL);
            document.getElementById('videoPreview').removeChild(document.getElementById('videoPreview').childNodes[0])
            var sourceTag = document.createElement('source');
            sourceTag.setAttribute('src', this.image);
            sourceTag.setAttribute('type', 'video/mp4');
            document.getElementById('videoPreview').appendChild(sourceTag);
            let vid = <HTMLVideoElement>document.getElementById("videoPreview");
            vid.load();
            vid.currentTime = 0;
            vid.play();
          }
        },
        error => {
          $('.overlay').show();
          this._messageDialogService.openDialogBox('Error', error.error.Message, Constants.msgBoxError);
          this._spinnerService.stop();
        });
  }

  navigateToAssetLibraryEdit() {
    let queryParams = {
      Routeparams: {
        passingparams: {
          "AssetLibraryIdentifier": this.assetLibrary.Identifier,
        },
        filteredparams: {
          //passing data using json stringify.
          "AssetLibraryName": this.assetLibrary.Name != null ? this.assetLibrary.Name : ""
        }
      }
    }
    localStorage.setItem("assetLibraryparams", JSON.stringify(queryParams))
    const router = this.injector.get(Router);
    router.navigate(['assetlibrary', 'Edit']);
  }

  deleteAssetLibrary() {
    let message = 'Are you sure, you want to delete this record?';
    this._messageDialogService.openConfirmationDialogBox('Confirm', message, Constants.msgBoxWarning).subscribe(async (isConfirmed) => {
      if (isConfirmed) {
        let roleData = [{
          "Identifier": this.assetLibrary.Identifier,
        }];

        let isDeleted = await this.assetLibraryService.deleteAssetLibrary(roleData);
        if (isDeleted) {
          let messageString = Constants.recordDeletedMessage;
          this._messageDialogService.openDialogBox('Success', messageString, Constants.msgBoxSuccess);
          this._router.navigate(['assetlibrary']);
        }
      }
    });
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

  sortData(sort: MatSort) {
    const data = this.assetLibrary.Assets.slice();
    if (!sort.active || sort.direction === '') {
      this.sortedAssetLibraryList = data;
      return;
    }

    this.sortedAssetLibraryList = data.sort((a, b) => {
      const isAsc = sort.direction === 'asc';
      switch (sort.active) {
        case 'name': return compareStr(a.Name, b.Name, isAsc);
        case 'updatedby': return compareStr(a.LastUpdatedBy.FirstName == null ? '' : a.LastUpdatedBy.FirstName, b.LastUpdatedBy.FirstName == null ? '' : b.LastUpdatedBy.FirstName, isAsc);
        case 'date': return compareStr(a.LastUpdatedDate == null ? '' : a.LastUpdatedDate, b.LastUpdatedDate == null ? '' : b.LastUpdatedDate, isAsc);
        default: return 0;

      }
    });
    this.dataSource = new MatTableDataSource<Asset>(this.sortedAssetLibraryList);
    this.dataSource.sort = this.sort;
    this.array = this.sortedAssetLibraryList;
    this.totalSize = this.array.length;
    this.iterator();
  }

}

function compareStr(a: string, b: string, isAsc: boolean) {
  return (a.toLowerCase() < b.toLowerCase() ? -1 : 1) * (isAsc ? 1 : -1);
}

function compareNumber(a: number, b: number, isAsc: boolean) {
  return (a < b ? -1 : 1) * (isAsc ? 1 : -1);
}
