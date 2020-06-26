import { Component, OnInit, Injector, ChangeDetectorRef, ViewChild, OnDestroy } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import * as $ from 'jquery';
import { Router, ActivatedRoute, NavigationEnd } from '@angular/router';
import { WindowRef } from '../../../core/services/window-ref.service';
import { environment } from '../../../../environments/environment';
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
  public params;
  public baseURL: string = environment.baseURL;
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

  }
  ngAfterViewInit() {
    this.dataSource.paginator = this.paginator;

  }

  public isCollapsedDetails: boolean = false;
  public isCollapsedAssets: boolean = true;
  navigateToListPage() {
    this._location.back();
  }
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
            this.getAssetLibraryRecords();

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
    let assetLibrary = await assetLibraryService.getAssetLibrary(searchParameter);
    this.assetLibrary = assetLibrary[0];
    this.dataSource = new MatTableDataSource(this.assetLibrary.Assets);
    this.dataSource.sort = this.sort;
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

}
