/// <reference path="../../appsettings.ts" />
import { Component, ViewChild, Output, Input, EventEmitter, SecurityContext } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { ConfigConstants } from '../../shared/constants/configConstants';
import * as Highcharts from 'highcharts';
import * as $ from 'jquery';
import { map } from 'rxjs/operators';
declare var require: any;
let Boost = require('highcharts/modules/boost');
let noData = require('highcharts/modules/no-data-to-display');
let More = require('highcharts/highcharts-more');
import { DomSanitizer, SafeResourceUrl, SafeUrl } from '@angular/platform-browser';
import { AppSettings } from '../../appsettings';
Boost(Highcharts);
noData(Highcharts);
More(Highcharts);
noData(Highcharts);

// Component Created for Customer Information Widget--
@Component({
  selector: 'customerInformation',
  template: `<div class="widget">
    <div class="widget-header">
      <span class="widget-header-title"> Customer Information </span>
    </div>
    <div class="widget-area">
      <div class="row">
        <div class="col-sm-4">
          <h4 class="mb-4">Laura J Donald</h4>
          <h6>4000 Executive Parkway, Saint Globin Rd #250,<br /> Canary Wharf, E94583</h6>
        </div>
        <div class="col-sm-8">
          <video class="doc-video" controls>
            <source src="assets/images/SampleVideo.mp4" type="video/mp4">
          </video>
        </div>
      </div>
    </div>
  </div>`
})
export class CustomerInformationComponent {
  @Input()
  widgetsGridsterItemArray: any[] = [];

}

// Component Created for Account Information Widget--
@Component({
  selector: 'accountInformation',
  template: `<div class="widget">
    <div class="widget-header">
      <span class="widget-header-title"> Account Information </span>
    </div>
    <div class="widget-area">
      <div class="list-row-small" *ngFor="let list of accountInfoLists">
        <div class="list-middle-row">
          <div class="list-text">
            {{list.title}}
          </div>
          <label class="list-value mb-0">
            {{list.value}}
          </label>
        </div>
      </div>
    </div>
  </div>`
})
export class AccountInformationComponent {
  @Input()
  widgetsGridsterItemArray: any[] = [];
  //account info
  accountInfoLists: any[] = [
    { title: 'Statement Date', value: '1-APR-2020' },
    { title: 'Statement Period', value: 'Annual Statement' },
    { title: 'Customer ID', value: 'ID2-8989-5656' },
    { title: 'RM Name', value: 'David Miller' },
    { title: 'RM Contact Number', value: '+4487867833' },
  ];

}

// Component Created for Image Widget--
@Component({
  selector: 'image-widget',
  template: `<div class="widget">
    <div class="widget-header">
      <span class="widget-header-title">Image </span>
    </div>
    <div class="widget-area position-relative width100">
      <div class="widget-indicator-inner text-center">
        <img [src]="ImageSrc" class="img-fluid" />
      </div>
    </div>
  </div>`
})
export class ImageComponent {

  @Input() imgItem: any;

  public ImageSrc: any;
  public baseURL = AppSettings.baseURL;

  constructor(private _http: HttpClient,
    private sanitizer: DomSanitizer) {}

  ngOnInit() {
    if (this.imgItem != null && this.imgItem.WidgetSetting != null && this.imgItem.WidgetSetting != '' && this.testJSON(this.imgItem.WidgetSetting)) {
      let widgetSetting = JSON.parse(this.imgItem.WidgetSetting);
      if (!widgetSetting.isPersonalize && widgetSetting.AssetLibraryId != 0 && widgetSetting.AssetId != 0) {
        this._http.get(this.baseURL + 'assetlibrary/asset/download?assetIdentifier=' + widgetSetting.AssetId, { responseType: "arraybuffer", observe: 'response' }).pipe(map(response => response))
        .subscribe(
          data => {
            let contentType = data.headers.get('Content-Type');
            let fileName = data.headers.get('x-filename');
            const blob = new Blob([data.body], { type: contentType });
            let objectURL = URL.createObjectURL(blob);
            this.ImageSrc = this.sanitizer.bypassSecurityTrustResourceUrl(objectURL); //this.sanitizer.sanitize(SecurityContext.RESOURCE_URL, this.sanitizer.bypassSecurityTrustResourceUrl(objectURL));
          },
          error => {
            //$('.overlay').show();
          });
      } else {
        this.ImageSrc = 'assets/images/icon-image.png';
      }
    } else {
      this.ImageSrc = 'assets/images/icon-image.png';
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
}

// Component Created for Video Widget--
@Component({
  selector: 'vidyo-widget',
  template: `<div class="widget">
    <div class="widget-header">
      <span class="widget-header-title">Video </span>
    </div>
    <div class="widget-area position-relative width100">
      <div class="widget-indicator-inner text-center" [id]="videoWidgetDivId">
        <video controls style="height: 200px;width: 75%;" [id]="videoControlTagId">
          <source src='{{videoSrc}}' type="video/mp4">
        </video>
      </div>
    </div>
  </div>`
})
export class VideoComponent {

  @Input() vdoItem: any;

  public videoSrc;
  public videoWidgetDivId;
  public videoControlTagId;
  public baseURL = AppSettings.baseURL;

  constructor(private _http: HttpClient,
    private sanitizer: DomSanitizer) {}

  ngOnInit() {
    if(this.vdoItem != null) {
      this.videoWidgetDivId = "VideoWidgetDiv"+this.vdoItem.x+this.vdoItem.y;
      this.videoControlTagId = 'videoConfigPreviewSrc'+this.vdoItem.x+this.vdoItem.y;
    }
    if (this.vdoItem != null && this.vdoItem.WidgetSetting != null && this.vdoItem.WidgetSetting != '' && this.testJSON(this.vdoItem.WidgetSetting)) {
      let widgetSetting = JSON.parse(this.vdoItem.WidgetSetting);
      if(widgetSetting.isEmbedded) {
        this.videoSrc = widgetSetting.SourceUrl;
      }
      else if (!widgetSetting.isPersonalize && widgetSetting.AssetLibraryId != 0 && widgetSetting.AssetId != 0) {
        this._http.get(this.baseURL + 'assetlibrary/asset/download?assetIdentifier=' + widgetSetting.AssetId, { responseType: "arraybuffer", observe: 'response' }).pipe(map(response => response))
        .subscribe(
          data => {
            let contentType = data.headers.get('Content-Type');
            const blob = new Blob([data.body], { type: contentType });
            let objectURL = URL.createObjectURL(blob);
            var url = this.sanitizer.sanitize(SecurityContext.RESOURCE_URL, this.sanitizer.bypassSecurityTrustResourceUrl(objectURL));
            var videoDiv = document.getElementById(this.videoWidgetDivId);
            if (videoDiv != undefined && videoDiv != null) {
              if (videoDiv.hasChildNodes()) {
                videoDiv.removeChild(document.getElementById(this.videoControlTagId));
              }
              
              var video = document.createElement('video');
              video.id = this.videoControlTagId;
              video.style.height = "200px";
              video.style.width = "75%";
              video.controls = true;

              var sourceTag = document.createElement('source');
              sourceTag.setAttribute('src', url);
              sourceTag.setAttribute('type', 'video/mp4');
              video.appendChild(sourceTag);
              videoDiv.appendChild(video);
            }
          },
          error => {
            //$('.overlay').show();
          });
      } 
      else {
        this.videoSrc = 'assets/images/SampleVideo.mp4';
      }
    } else {
      this.videoSrc = 'assets/images/SampleVideo.mp4';
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
}

// Component Created for Summary at glance Widget--
@Component({
  selector: 'summaryAtGlance',
  template: `<div class="widget">
    <div class="widget-header">
      <span class="widget-header-title"> Summary at Glance </span>
    </div>
    <div class="widget-area-grid padding-0">
      <div class="d-flex justify-content-center mb-4">
        <div class="pagination-mat position-relative d-md-block d-none">
          <mat-paginator #paginator [pageSize]="pageSize" [pageSizeOptions]="[5, 10, 20]"
                         [showFirstLastButtons]="true" [length]="totalSize" [pageIndex]="currentPage">
          </mat-paginator>
        </div>
      </div>
      <table mat-table [dataSource]="dataSourceSummary" matSort class="table-cust">
        <!-- Position Column -->
        <ng-container matColumnDef="account">
          <th class="width15" mat-header-cell *matHeaderCellDef mat-sort-header> Account </th>
          <td mat-cell *matCellDef="let element"> {{element.account}} </td>
        </ng-container>

        <!-- Name Column -->
        <ng-container matColumnDef="currency">
          <th class="width10" mat-header-cell *matHeaderCellDef mat-sort-header> Currency </th>
          <td mat-cell *matCellDef="let element"> {{element.currency}} </td>
        </ng-container>

        <ng-container matColumnDef="amount">
          <th class="width10" mat-header-cell *matHeaderCellDef mat-sort-header> Amount </th>
          <td mat-cell *matCellDef="let element"> {{element.amount}} </td>
        </ng-container>

        <tr mat-header-row *matHeaderRowDef="displayedColumnsSummary"></tr>
        <tr mat-row *matRowDef="let row; columns: displayedColumnsSummary;"></tr>
      </table>
    </div>
  </div>`
})
export class SummaryAtGlanceComponent {
  @Input()
  widgetsGridsterItemArray: any[] = [];
  displayedColumnsSummary: string[] = ['account', 'currency', 'amount'];
  dataSourceSummary = new MatTableDataSource<any>();
  public pageSize = 5;
  public currentPage = 0;
  public totalSize = 0;

  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;

  ngAfterViewInit() {
    this.dataSourceSummary.paginator = this.paginator;
  }

  ngOnInit() {
    this.dataSourceSummary = new MatTableDataSource(List_Data_Summary);
    this.dataSourceSummary.sort = this.sort;
  }
}

export interface ListSummary {
  account: string;
  currency: string;
  amount: string;
}

const List_Data_Summary: ListSummary[] = [
  { account: 'Saving Account', currency: 'GBP', amount: '873899' },
  { account: 'Current Account', currency: 'GBP', amount: '873899' },
  { account: 'Recurring Deposit', currency: 'GBP', amount: '873899' },
  { account: 'Wealth', currency: 'GBP', amount: '873899' },
];

// Component Created for saving Available Balance Widget--
@Component({
  selector: 'savingAvailableBalance',
  template: `<div class="widget">
    <div class="widget-header">
        <span class="widget-header-title"> Saving : Available Balance </span>
    </div>
    <div class="widget-area">
          <h4 style="text-align:right" class='mb-4'><i style="color: limegreen" class="fa fa-sort-asc" aria-hidden="true"></i>3245323</h4>
          <span style="float:left;">Total Deposits</span><span style="float:right;">16750,00</span><br />
          <span style="float:left;">Total Spend</span><span style="float:right;">16750,00</span><br />
          <span style="float:left;">Savings</span><span style="float:right;">3250.00</span><br />
    </div>
</div>`
})
export class SavingAvailableBalanceComponent {
  @Input()
  widgetsGridsterItemArray: any[] = [];
}

// Component Created for Current Available Balance Widget--
@Component({
  selector: 'currentAvailableBalance',
  template: `<div class="widget">
      <div class="widget-header">
          <span class="widget-header-title"> Current : Available Balance </span>
      </div>
      <div class="widget-area  position-relative width100">
        <h4 style="text-align:right" class='mb-4'><i style="color: limegreen" class="fa fa-sort-asc" aria-hidden="true"></i> 3245323</h4>
          <span style="float:left;">Total Deposits</span><span style="float:right;">1675000</span><br />
          <span style="float:left;">Total Spend</span><span style="float:right;">1675000</span><br />
          <span style="float:left;">Savings</span><span style="float:right;">3250.00</span><br />
    </div>
</div>`
})
export class CurrentAvailableBalanceComponent {
  @Input()
  widgetsGridsterItemArray: any[] = [];
}

// Component Created for Current Transaction details Widget--
@Component({
  selector: 'transactionDetails',
  template: `<div class="widget">
  <div class="widget-header">
    <span class="widget-header-title"> Transaction Details
      <span class="float-right mr-4"><i class="fa fa-caret-left mr-2" style="color:red;font-size: 20px;"></i>Chat Now</span>
    </span>
  </div>
  <div class="widget-area-grid padding-0">
      <div class='float-left'>
        <input type='radio' id='showAll' name='showAll' [checked]='isShowAll' (change)='ShowAll($event.target)'>&nbsp;<label for='showAll'>Show All</label>&nbsp;
        <input type='radio' id='grpDate' name='showAll' [checked]='isGroupByDate' (change)='GroupByDate($event.target)'>&nbsp;<label for='grpDate'>Group By Date</label>&nbsp;
      </div>
      
      <form [formGroup]="transactionForm">
      <div class='float-right'>
          <div class="float-left mr-2" *ngIf="isShowAll">
            <select class="form-control float-left" formControlName="filterStatus" id="filterStatus">
              <option value="0"> Search Item</option>
              <option value="Failed">Failed</option>
              <option value="Completed">Completed</option>
              <option value="In Progress">In Progress</option>
            </select>
          </div>
          <button href='javascript:void(0)' *ngIf="isShowAll" class='btn btn-light btn-sm'>Search</button>&nbsp;
          <button href='javascript:void(0)' class='btn btn-light btn-sm'>Reset</button>&nbsp;
          <button href='javascript:void(0)' class='btn btn-light btn-sm'>Print</button> 
      </div>
    </form>

    <div class="d-flex justify-content-center mb-4">
      <div class="pagination-mat position-relative d-md-block d-none">
        <mat-paginator #paginator [pageSize]="pageSize" [pageSizeOptions]="[5, 10, 20]"
                       [showFirstLastButtons]="true" [length]="totalSize" [pageIndex]="currentPage">
        </mat-paginator>
      </div>
    </div>
    <table mat-table [dataSource]="dataSource" matSort class="table-cust">
      <!-- Position Column -->
      <ng-container matColumnDef="date">
        <th class="width15" mat-header-cell *matHeaderCellDef mat-sort-header> Date </th>
        <td mat-cell *matCellDef="let element"> {{element.date}} </td>
      </ng-container>

      <!-- Name Column -->
      <ng-container matColumnDef="type">
        <th class="width10" mat-header-cell *matHeaderCellDef mat-sort-header> Type </th>
        <td mat-cell *matCellDef="let element"> {{element.type}} </td>
      </ng-container>

      <!-- Weight Column -->
      <ng-container matColumnDef="narration">
        <th class="width20" mat-header-cell *matHeaderCellDef mat-sort-header> Narration </th>
        <td mat-cell *matCellDef="let element"> {{element.narration}} </td>
      </ng-container>

      <!-- Symbol Column -->
      <ng-container matColumnDef="fcy">
        <th class="width15" mat-header-cell *matHeaderCellDef mat-sort-header> FCY(GBP) </th>
        <td mat-cell *matCellDef="let element"> {{element.fcy}} </td>
      </ng-container>

      <ng-container matColumnDef="currentRate">
        <th class="width15" mat-header-cell *matHeaderCellDef mat-sort-header> Current Rate </th>
        <td mat-cell *matCellDef="let element"> {{element.currentRate}} </td>
      </ng-container>

      <ng-container matColumnDef="lyc">
        <th class="width15" mat-header-cell *matHeaderCellDef mat-sort-header> LCY(GBP) </th>
        <td mat-cell *matCellDef="let element"> {{element.lyc}} </td>
      </ng-container>

      <ng-container matColumnDef="action">
        <th class="width10" mat-header-cell *matHeaderCellDef mat-sort-header> Action </th>
        <td mat-cell *matCellDef="let element">
          <div class="action-btns btn-tbl-action">
            <button type="button" title="View" id="btnView" routerLink="View"><span class="fa fa-paper-plane-o"></span></button>
          </div>
        </td>
      </ng-container>

      <tr mat-header-row *matHeaderRowDef="displayedColumns"></tr>
      <tr mat-row *matRowDef="let row; columns: displayedColumns;"></tr>
    </table>
    <!--mobile view start-->
    <div class="mobile-tile card-shadow d-md-none border-0" *ngFor="let list of TransactionDetailData" style="display:none;">
      <div class="row mb-1">
        <div class="col-5"> <label class="m-0"> Date </label> </div>
        <div class="col-7">  {{list.date}} </div>
      </div>
      <div class="row mb-1">
        <div class="col-5"> <label class="m-0"> Type </label> </div>
        <div class="col-7">  {{list.type}} </div>
      </div>
      <div class="row mb-1">
        <div class="col-5"> <label class="m-0">Narration </label> </div>
        <div class="col-7">  {{list.narration}} </div>
      </div>
      <div class="row mb-1">
        <div class="col-5"> <label class="m-0">FCY(GBP) </label> </div>
        <div class="col-7">  {{list.fcy}} </div>
      </div>
      <div class="row mb-1">
        <div class="col-5"> <label class="m-0"> Current Rate </label> </div>
        <div class="col-7">  {{list.currentRate}} </div>
      </div>
      <div class="row mb-1">
        <div class="col-5"> <label class="m-0">LCY(GBP) </label> </div>
        <div class="col-7">  {{list.lyc}} </div>
      </div>
      <div class="text-center">
        <div class="action-btns btn-tbl-action">
          <button type="button"><span class="fa fa-paper-plane-o"></span></button>
        </div>
      </div>
    </div>
    <!--mobile view end-->
  </div>`
})
export class TransactionDetailsComponent {
  @Input()
  widgetsGridsterItemArray: any[] = [];
  displayedColumns: string[] = ['date', 'type', 'narration', 'fcy', 'currentRate', 'lyc', 'action'];
  dataSource = new MatTableDataSource<any>();
  public pageSize = 5;
  public currentPage = 0;
  public totalSize = 0;
  public transactionForm: FormGroup;

  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;

  TransactionDetailData: TransactionDetail[] = [
    { date: '15/07/2020', type: 'CR', narration: 'NXT TXN: IIFL IIFL6574562', fcy: '1666.67', currentRate: '1.062', lyc: '1771.42' },
    { date: '15/07/2020', type: 'CR', narration: 'NXT TXN: IIFL IIFL6574562', fcy: '1666.67', currentRate: '1.062', lyc: '1771.42' },
    { date: '15/07/2020', type: 'DB', narration: 'NXT TXN: IIFL IIFL6574562', fcy: '1666.67', currentRate: '1.062', lyc: '1771.42' },

    { date: '19/07/2020', type: 'CR', narration: 'NXT TXN: IIFL IIFL3557346', fcy: '1254.71', currentRate: '1.123', lyc: '1876.00' },
    { date: '19/07/2020', type: 'CR', narration: 'NXT TXN: IIFL IIFL3557346', fcy: '1254.71', currentRate: '1.123', lyc: '1876.00' },

    { date: '25/07/2020', type: 'CR', narration: 'NXT TXN: IIFL IIFL8965435', fcy: '2345.12', currentRate: '1.461', lyc: '1453.21' },
    { date: '28/07/2020', type: 'CR', narration: 'NXT TXN: IIFL IIFL0034212', fcy: '1435.89', currentRate: '0.962', lyc: '1654.56' },
  ];
  TransactionDetailDataGroup: TransactionDetail[] = [
    { date: '15/07/2020', type: 'CR', narration: '-', fcy: '3333.34', currentRate: '2.124', lyc: '3542.84' },
    { date: '15/07/2020', type: 'DB', narration: '-', fcy: '1666.67', currentRate: '1.062', lyc: '1771.42' },

    { date: '19/07/2020', type: 'CR', narration: '-', fcy: '2491.42', currentRate: '2.246', lyc: '3752.00' },

    { date: '28/07/2020', type: 'CR', narration: '-', fcy: '1435.89', currentRate: '0.962', lyc: '1654.56' },
    { date: '25/07/2020', type: 'CR', narration: '-', fcy: '2345.12', currentRate: '1.461', lyc: '1453.21' },
  ];

  ngAfterViewInit() {
    this.dataSource.paginator = this.paginator;
  }
  public isShowAll = true;
  public isGroupByDate = false;
  ShowAll(event) {
    const value = event.checked;
    this.isShowAll = event.checked;
    if (value) {
      this.isGroupByDate = false;

      this.dataSource = new MatTableDataSource(this.TransactionDetailData);
      this.dataSource.sort = this.sort;
    }
    else {
      this.isGroupByDate = true;

      this.dataSource = new MatTableDataSource(this.TransactionDetailDataGroup);
      this.dataSource.sort = this.sort;
    }
  }
  GroupByDate(event) {
    const value = event.checked;
    this.isGroupByDate = event.checked;

    if (value) {
      this.isShowAll = false;
      this.dataSource = new MatTableDataSource(this.TransactionDetailDataGroup);
      this.dataSource.sort = this.sort;
    }
    else {
      this.isShowAll = true;

      this.dataSource = new MatTableDataSource(this.TransactionDetailData);
      this.dataSource.sort = this.sort;
    }
  }
  ngOnInit() {
    this.transactionForm = this.fb.group({
      filterStatus: [0],
    });
    this.dataSource = new MatTableDataSource(this.TransactionDetailData);
    this.dataSource.sort = this.sort;
  }
  constructor(
    private fb: FormBuilder,
   ) {
    
  }
}

// Component Created for Saving Transaction details Widget--
@Component({
  selector: 'savingtransactionDetails',
  template: `<div class="widget">
    <div class="widget-header">
      <span class="widget-header-title"> Transaction Details </span>

    </div>
    <div class="widget-area-grid padding-0">
    <div class='float-left'>
        <input type='radio' id='showAll' name='showAll' [checked]='isShowAll' (change)='ShowAll($event.target)'>&nbsp;<label for='showAll'>Show All</label>&nbsp;
        <input type='radio' id='grpDate' name='showAll' [checked]='isGroupByDate' (change)='GroupByDate($event.target)'>&nbsp;<label for='grpDate'>Group By Date</label>&nbsp;
      </div>
    
     <div class='float-right'>
          <div class="float-left mr-2">
         
            <select  class="form-control float-left"  id="filterStatus">
              <option value="0"> Search Item</option>
              <option value="Failed">Failed</option>
              <option value="Completed">Completed</option>
              <option value="In Progress">In Progress</option>
            </select>
          </div>
          <button href='javascript:void(0)'  class='btn btn-light btn-sm'>Search</button>&nbsp;
          <button href='javascript:void(0)' class='btn btn-light btn-sm'>Reset</button>&nbsp;
          <button href='javascript:void(0)' class='btn btn-light btn-sm'>Print</button> 
      </div>
      <div class="d-flex justify-content-center mb-4">
        <div class="pagination-mat position-relative d-md-block d-none">
          <mat-paginator #paginator [pageSize]="pageSize" [pageSizeOptions]="[5, 10, 20]"
                         [showFirstLastButtons]="true" [length]="totalSize" [pageIndex]="currentPage">
          </mat-paginator>
        </div>
      </div>
      <table mat-table [dataSource]="dataSource" matSort class="table-cust">
        <!-- Position Column -->
        <ng-container matColumnDef="date">
          <th class="width15" mat-header-cell *matHeaderCellDef mat-sort-header> Date </th>
          <td mat-cell *matCellDef="let element"> {{element.date}} </td>
        </ng-container>
  
        <!-- Name Column -->
        <ng-container matColumnDef="type">
          <th class="width10" mat-header-cell *matHeaderCellDef mat-sort-header> Type </th>
          <td mat-cell *matCellDef="let element"> {{element.type}} </td>
        </ng-container>
  
        <!-- Weight Column -->
        <ng-container matColumnDef="narration">
          <th class="width20" mat-header-cell *matHeaderCellDef mat-sort-header> Narration </th>
          <td mat-cell *matCellDef="let element"> {{element.narration}} </td>
        </ng-container>
  
        <!-- Symbol Column -->
        <ng-container matColumnDef="credit" >
          <th class="width15" mat-header-cell *matHeaderCellDef mat-sort-header> Credit </th>
          <td mat-cell *matCellDef="let element" style="color:limegreen"> {{element.credit}} </td>
        </ng-container>
  
        <ng-container matColumnDef="debit">
          <th class="width15" mat-header-cell *matHeaderCellDef mat-sort-header> Debit </th>
          <td mat-cell *matCellDef="let element" style="color:red"> {{element.debit}} </td>
        </ng-container>
  
        <ng-container matColumnDef="query">
          <th class="width15" mat-header-cell *matHeaderCellDef mat-sort-header> Query </th>
          <td mat-cell *matCellDef="let element"> <i class="fa fa-caret-left fa-3x" style="color:red" aria-hidden="true"></i></td>
        </ng-container>
        <ng-container matColumnDef="balance">
          <th class="width15" mat-header-cell *matHeaderCellDef mat-sort-header> Balance </th>
          <td mat-cell *matCellDef="let element"> {{element.balance}} </td>
        </ng-container>
  
  
        <tr mat-header-row *matHeaderRowDef="displayedColumns"></tr>
        <tr mat-row *matRowDef="let row; columns: displayedColumns;"></tr>
      </table>
      <!--mobile view start-->
      <div class="mobile-tile card-shadow d-md-none border-0" *ngFor="let list of TransactionDetailData" style="display:none;">
        <div class="row mb-1">
          <div class="col-5"> <label class="m-0"> Date </label> </div>
          <div class="col-7">  {{list.date}} </div>
        </div>
        <div class="row mb-1">
          <div class="col-5"> <label class="m-0"> Type </label> </div>
          <div class="col-7">  {{list.type}} </div>
        </div>
        <div class="row mb-1">
          <div class="col-5"> <label class="m-0">Narration </label> </div>
          <div class="col-7">  {{list.narration}} </div>
        </div>
        <div class="row mb-1">
          <div class="col-5"> <label class="m-0">Credit</label> </div>
          <div class="col-7">  {{list.credit}} </div>
        </div>
        <div class="row mb-1">
          <div class="col-5"> <label class="m-0"> Debit </label> </div>
          <div class="col-7">  {{list.debit}} </div>
        </div>
        <div class="row mb-1">
          <div class="col-5"> <label class="m-0">Query</label> </div>
          <div class="col-7">  {{list.query}} </div>
        </div>
        <div class="row mb-1">
          <div class="col-5"> <label class="m-0">Balance</label> </div>
          <div class="col-7">  {{list.balance}} </div>
        </div>
        
      </div>
      <!--mobile view end-->
    </div>`
})
export class SavingTransactionDetailsComponent {
  @Input()
  widgetsGridsterItemArray: any[] = [];
  displayedColumns: string[] = ['date', 'type', 'narration', 'credit', 'query', 'debit', 'balance',];
  dataSource = new MatTableDataSource<any>();
  public pageSize = 5;
  public currentPage = 0;
  public totalSize = 0;
  public transactionForm: FormGroup;
  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;

  TransactionDetailData: any[] = [
    { date: '15/07/2020', type: 'CR', query: '', narration: 'ACB Credit :Salary', credit: 'R\'1666.67', debit: '', balance: 'R\'1771.42' },
    { date: '15/07/2020', type: 'CR', query: '', narration: 'INTEREST', credit: 'R\'500.00', debit: '', balance: 'R\'1771.42' },
    { date: '18/07/2020', type: 'DB', query: '', narration: 'ACB Debit :Mobile', credit: '', debit: '-R\'100.123', balance: 'R\'1876.00' },
    { date: '18/07/2020', type: 'DB', query: '', narration: 'ACB Debit :DSTV', credit: '', debit: '-R\'500.00', balance: 'R\'1876.00' },
    { date: '22/07/2020', type: 'DB', query: '', narration: 'Bank Charges', credit: '', debit: '-R\'1.461', balance: 'R\'1453.21' },
    { date: '23/07/2020', type: 'DB', query: '', narration: 'INTERNET BANKING FEE', credit: '', debit: '-R\'0.962', balance: 'R\'1654.56' },
    { date: '25/07/2020', type: 'CR', query: '', narration: 'ACB Credit :Medical Aid', credit: 'R\'1666.67', debit: '-R\'1.062', balance: 'R\'1771.42' },
    { date: '25/07/2020', type: 'CR', query: '', narration: 'INTEREST', credit: 'R\'500.00', debit: '', balance: 'R\'1771.42' },
    { date: '26/07/2020', type: 'DB', query: '', narration: 'IBANK Payement:Electricity', credit: '', debit: '-R\'1.461', balance: 'R\'1453.21' },
    { date: '29/07/2020', type: 'DB', query: '', narration: 'IBANK Payement:Home Loan', credit: '', debit: '-R\'1.461', balance: 'R\'1453.21' },
  ];

  TransactionDetailDataGroup: any[] = [
    { date: '15/07/2020', type: 'CR', query: '', narration: '-', credit: 'R\'2166.67', debit: '', balance: 'R\'2271.42' },
    { date: '18/07/2020', type: 'DB', query: '', narration: '-', credit: '', debit: '-R\'600.123', balance: 'R\'1671.29' },
    { date: '22/07/2020', type: 'DB', query: '', narration: '-', credit: '', debit: '-R\'1.461', balance: 'R\'1669.83' },
    { date: '23/07/2020', type: 'DB', query: '', narration: '-', credit: '', debit: '-R\'0.962', balance: 'R\'1668.87' },
    { date: '25/07/2020', type: 'CR', query: '', narration: '-', credit: 'R\'2166.67', debit: '', balance: 'R\'3835.54' },
    { date: '26/07/2020', type: 'DB', query: '', narration: '-', credit: '', debit: '-R\'1.461', balance: 'R\'3834.07' },
    { date: '29/07/2020', type: 'DB', query: '', narration: '-', credit: '', debit: '-R\'1.461', balance: 'R\'3882.60' },
  ];

  ngAfterViewInit() {
    this.dataSource.paginator = this.paginator;
  }

  ngOnInit() {
    this.dataSource = new MatTableDataSource(this.TransactionDetailData);
    this.dataSource.sort = this.sort;
  }
  public isShowAll = true;
  public isGroupByDate = false;

  ShowAll(event) {
    const value = event.checked;
    this.isShowAll = event.checked;
    if (value) {
      this.isGroupByDate = false;

      this.dataSource = new MatTableDataSource(this.TransactionDetailData);
      this.dataSource.sort = this.sort;
    }
    else {
      this.isGroupByDate = true;

      this.dataSource = new MatTableDataSource(this.TransactionDetailDataGroup);
      this.dataSource.sort = this.sort;
    }
  }
  GroupByDate(event) {
    const value = event.checked;
    this.isGroupByDate = event.checked;

    if (value) {
      this.isShowAll = false;
      this.dataSource = new MatTableDataSource(this.TransactionDetailDataGroup);
      this.dataSource.sort = this.sort;
    }
    else {
      this.isShowAll = true;

      this.dataSource = new MatTableDataSource(this.TransactionDetailData);
      this.dataSource.sort = this.sort;
    }
  }
}

// Component Created for Reminder and Recommendation Widget--
@Component({
  selector: 'reminderAndRecommendation',
  template: `<div class="widget">
      <div class="widget-header">
         <span class="widget-header-title"> Reminder and Recommendations </span>
      </div>
      <div class="widget-area-grid" style="overflow-x:hidden;">
        <div class="row">
          <div class="col-lg-9">
          </div>
          <div class="col-lg-3">
            <i class="fa fa-caret-left fa-3x float-left text-danger" aria-hidden="true"></i>
            <span class="mt-2 d-inline-block ml-2">Click</span>
          </div>       
        </div>
        <div class="row" *ngFor="let list of actionList">
          <div class="col-lg-9 text-left">
            <p class="p-1" style="background-color: #dce3dc;">{{list.title}} </p>
          </div>
          <div class="col-lg-3">
            <a><i class="fa fa-caret-left fa-3x float-left text-danger"></i>
            <span class="mt-2 d-inline-block ml-2">{{list.action}}</span></a>
          </div>
        </div>
      </div>
</div>`
})
export class ReminderAndRecommComponent {
  @Input()
  widgetsGridsterItemArray: any[] = [];
  public actionList: any[] = [
    { title: "Update Missing Inofrmation", action: "Update" },
    { title: "Your Rewards Video ia available", action: "View" },
    { title: "Payment Due for Home Loan", action: "Pay" },
    { title: "Need financial planning for savings.", action: "Call Me" },
    { title: "Subscribe/Unsubscribe Alerts.", action: "Apply" },
    { title: "Your credit card payment is due now.", action: "Pay" }
  ];
}

// Component Created for Analytics Widget--
@Component({
  selector: 'analytics',
  template: `<div class="widget">
    <div class="widget">
    <div class="widget-header">
        <span class="widget-header-title"> Analytics </span>
    </div>
    <div class="widget-area position-relative width100">       
        <div id="chartWidgetPiecontainer" class="p-3"></div>
    </div>
</div>`
})
export class AnalyticsWidgetComponent {
  @Input()
  widgetsGridsterItemArray: any[] = [];

  public options4: any = {
    chart: {
      backgroundColor: 'rgba(0,0,0,0)',
      plotBorderWidth: null,
      plotShadow: false,
      type: 'pie',
      height: (9 / 16 * 100) + '%',
    },
    title: {
      text: ''
    },
    tooltip: {
      pointFormat: '{series.name}: <b>{point.percentage:.1f}%</b>'
    },
    accessibility: {
      point: {
        valueSuffix: '%'
      }
    },
    plotOptions: {
      pie: {
        allowPointSelect: true,
        cursor: 'pointer',
        dataLabels: {
          enabled: true,
          format: '<b>{point.name}</b>: {point.percentage:.1f} %'
        },
        showInLegend: false
      }
    },
    series: [{
      name: 'Percentage',
      colorByPoint: true,
      data: [
        {
          name: 'Cutomer Information',
          y: 11.84
        }, {
          name: 'Account Information',
          y: 10.85
        }, {
          name: 'Image',
          y: 4.67
        }, {
          name: 'Video',
          y: 4.18
        }, {
          name: 'News Alerts',
          y: 7.05
        }]
    }]
  }

  ngAfterViewInit() {
    setTimeout(() => {
      Highcharts.chart('chartWidgetPiecontainer', this.options4);      
    }, 100);
  }

  ngOnInit() {
    $(document).ready(function () {
      setTimeout(function () {
        window.dispatchEvent(new Event('resize'));
      }, 10);
    });
  }
}

// Component Created for Saving Trends Widget--
@Component({
  selector: 'savingTrends',
  template: `<div class="widget">
    <div class="widget">
    <div class="widget-header">
        <span class="widget-header-title"> Your Saving Trends </span>
    </div>
    <div class="widget-area position-relative width100">
      <div class="text-right" style="font-size:20px"><span>+5.6%</span><span class="pl-3">+3.5%</span></div>
      <div id="savingTrendscontainer" class="p-3"></div>        
    </div>
</div>`
})
export class SavingTrendsComponent {
  @Input()
  widgetsGridsterItemArray: any[] = [];
  public options4: any = {
    chart: {
      height: (9 / 16 * 100) + '%',
      backgroundColor: 'rgba(0,0,0,0)'
    },
    title: {
      text: ''
    },
    xAxis: {
      categories: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul']
    },
    labels: {
      items: [{
        //html: 'How you have been spending',
        style: {
          left: '50px',
          top: '18px',
          color: ( // theme
            Highcharts.defaultOptions.title.style &&
            Highcharts.defaultOptions.title.style.color
          ) || 'black'
        }
      }]
    },
    series: [{
      // type: 'spline',
      name: '',
      data: [1.5, 2.5, 3, 1.5, 3, 2, 4],
      marker: {
        lineWidth: 1,
        lineColor: Highcharts.getOptions().colors[3],
        fillColor: 'white'
      }
    }]
  }
  ngAfterViewInit() {
    setTimeout(() => {
      Highcharts.chart('savingTrendscontainer', this.options4);      
    }, 100);
  }

  ngOnInit() {
    $(document).ready(function () {
      setTimeout(function () {
        window.dispatchEvent(new Event('resize'));
      }, 10);
    });
  }
}

// Component Created for Top 4 Income sources Widget--
@Component({
  selector: 'topFourIncomdeSources',
  template: `<div class="widget">
   <div class="widget">
      <div class="widget-header">
         <span class="widget-header-title"> Top Four Income Sources </span>
      </div>
      <div class="widget-area">
         <table class="table-borderless width100">
            <thead>
               <tr>
                  <td style="width:50%"></td>
                  <td style="width:20%">This Month</td>
                  <td style="width:30%;">Usually you spend</td>
               </tr>
            </thead>
            <tbody>
               <tr *ngFor="let list of actionList">
                  <td style="width:50%">
                     <label>{{list.name}}</label>
                  </td>
                  <td style="width:20%">
                     {{list.thisMonth}}
                  </td>
                  <td style="width:30%;">
                     <span *ngIf="!list.isAscIcon" style="color: red" class="fa fa-sort-desc fa-2x" aria-hidden="true"></span>
                     <span *ngIf="!list.isAscIcon" class="ml-2">{{list.usuallySpend}}</span>
                      <span *ngIf="list.isAscIcon" class="fa fa-sort-asc fa-2x mt-1 float-left" aria-hidden="true" style="position:relative;top:6px;color:limegreen"></span>
                     <span *ngIf="list.isAscIcon" class="ml-2" style="position:relative;top:6px;">{{list.usuallySpend}}</span>
                  </td>
               </tr>
            </tbody>
         </table>
      </div>
   </div>
</div>`
})
export class TopIncomeSourcesComponent {
  @Input()
  widgetsGridsterItemArray: any[] = [];
  public actionList: any[] = [
    { name: "Salary Transfer", thisMonth: 3453, usuallySpend: 123, isAscIcon: true },
    { name: "Cash Deposit", thisMonth: 3453, usuallySpend: 6123, isAscIcon: false },
    { name: "Profit Earned", thisMonth: 3453, usuallySpend: 6123, isAscIcon: false },
    { name: "Rebete", thisMonth: 3453, usuallySpend: 123, isAscIcon: true }
  ]
}

// Component Created for Spending Trends Widget--
@Component({
  selector: 'spendingTrends',
  template: `<div class="widget">
    <div class="widget">
    <div class="widget-header">
        <span class="widget-header-title"> Spending Trends </span>
    </div>
    <div class="widget-area position-relative width100">
          <div id="spendingTrendscontainer" class="p-3"></div>       
    </div>
</div>`
})
export class SpendindTrendsComponent {
  @Input()
  widgetsGridsterItemArray: any[] = [];
  public options4: any = {
    chart: {
      height: (9 / 16 * 100) + '%',
      backgroundColor: 'rgba(0,0,0,0)'
    },
    title: {
      text: ''
    },
    xAxis: {
      categories: ['Jan', 'Feb', 'Mar', 'Apr', 'May']
    },
    labels: {
      items: [{
        //html: 'How you have been spending',
        style: {
          left: '50px',
          top: '18px',
          color: ( // theme
            Highcharts.defaultOptions.title.style &&
            Highcharts.defaultOptions.title.style.color
          ) || 'black'
        }
      }]
    },
    series: [{
      type: 'column',
      name: 'Your Income',
      data: [1, 3, 4, 2, 5]
    }, {
      type: 'column',
      name: 'Your Spending',
      data: [2, 2, 1, 4, 1]
    }, {
      type: 'spline',
      name: '',
      data: [1.5, 2.5, 3, 1.5, 3],
      marker: {
        lineWidth: 2,
        lineColor: Highcharts.getOptions().colors[3],
        fillColor: 'white'
      }
    }]
  }
  ngAfterViewInit() {
    setTimeout(() => {
      Highcharts.chart('spendingTrendscontainer', this.options4);  
    }, 10);
  }

  ngOnInit() {
    $(document).ready(function () {
      setTimeout(function () {
        window.dispatchEvent(new Event('resize'));
      }, 100);
    });
  }
}

export interface TransactionDetail {
  date: string;
  type: string;
  narration: string;
  fcy: string;
  currentRate: string;
  lyc: string;
}


// Component Created for Dynamic pier chart Widget--
@Component({
  selector: 'DynamicPieChartWidget',
  template: `<div class="widget">
    <div class="widget">
    <div class="widget-header">
        <span class="widget-header-title"> Analytics </span>
    </div>
    <div class="widget-area position-relative width100">       
      <div [attr.id]="barChartDivId" class="p-3"></div>      
    </div>
</div>`
})
export class DynamicPieChartWidgetComponent {
  @Input() piechartItem: any;
  widgetsGridsterItemArray: any[] = [];
  barChartDivId = '';

  public options4: any = {
    chart: {
      backgroundColor: 'rgba(0,0,0,0)',
      plotBorderWidth: null,
      plotShadow: false,
      type: 'pie',
      height: (9 / 16 * 100) + '%',
    },
    title: {
      text: ''
    },
    tooltip: {
      pointFormat: '{series.name}: <b>{point.percentage:.1f}%</b>'
    },
    accessibility: {
      point: {
        valueSuffix: '%'
      }
    },
    plotOptions: {
      pie: {
        allowPointSelect: true,
        cursor: 'pointer',
        dataLabels: {
          enabled: true,
          format: '<b>{point.name}</b>: {point.percentage:.1f} %'
        },
        showInLegend: false
      }
    },
    series: [{
      name: 'Percentage',
      colorByPoint: true,
      data: [
        {
          name: 'Cutomer Information',
          y: 11.84
        }, {
          name: 'Account Information',
          y: 10.85
        }, {
          name: 'Image',
          y: 4.67
        }, {
          name: 'Video',
          y: 4.18
        }, {
          name: 'News Alerts',
          y: 7.05
        }]
    }]
  }

  ngAfterViewInit() {
    if(this.piechartItem != undefined) {
      setTimeout(() => {
        Highcharts.chart(this.barChartDivId, this.options4);  
      }, 10);
    }
  }

  ngOnInit() {
    if(this.piechartItem != undefined) {
      this.barChartDivId = "pieChartcontainer"+this.piechartItem.WidgetId;
      $(document).ready(function () {
        setTimeout(function () {
          window.dispatchEvent(new Event('resize'));
        }, 100);
      });
    }
  }

}

// Component Created for Dynamic line chart Widget--
@Component({
  selector: 'DynamicLineChartWidget',
  template: `<div class="widget">
    <div class="widget">
    <div class="widget-header">
        <span class="widget-header-title"> Your Saving Trends </span>
    </div>
    <div class="widget-area position-relative width100">
      <div class="text-right" style="font-size:20px"><span>+5.6%</span><span class="pl-3">+3.5%</span></div>
      <div [attr.id]="barChartDivId" class="p-3"></div>      
    </div>
</div>`
})
export class DynamicLineChartWidgetComponent {
  @Input() linechartItem: any;
  widgetsGridsterItemArray: any[] = [];
  barChartDivId = '';
  public options4: any = {
    chart: {
      height: (9 / 16 * 100) + '%',
      backgroundColor: 'rgba(0,0,0,0)'
    },
    title: {
      text: ''
    },
    xAxis: {
      categories: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul']
    },
    labels: {
      items: [{
        //html: 'How you have been spending',
        style: {
          left: '50px',
          top: '18px',
          color: ( // theme
            Highcharts.defaultOptions.title.style &&
            Highcharts.defaultOptions.title.style.color
          ) || 'black'
        }
      }]
    },
    series: [{
      // type: 'spline',
      name: '',
      data: [1.5, 2.5, 3, 1.5, 3, 2, 4],
      marker: {
        lineWidth: 1,
        lineColor: Highcharts.getOptions().colors[3],
        fillColor: 'white'
      }
    }]
  }

  ngAfterViewInit() {
    if(this.linechartItem != undefined) {
      setTimeout(() => {
        Highcharts.chart(this.barChartDivId, this.options4);  
      }, 10);
    }
  }

  ngOnInit() {
    if(this.linechartItem != undefined) {
      this.barChartDivId = "lineGraphcontainer"+this.linechartItem.WidgetId;
      $(document).ready(function () {
        setTimeout(function () {
          window.dispatchEvent(new Event('resize'));
        }, 100);
      });
    }
  }

}

// Component Created for Dynamic Bar Chart Widget--
@Component({
  selector: 'DynamicBarChartWidget',
  template: `<div class="widget">
    <div class="widget">
    <div class="widget-header">
      <span class="widget-header-title"> Spending Trends </span>
    </div>
    <div class="widget-area position-relative width100">
        <div [attr.id]="barChartDivId" class="p-3"></div>       
    </div>
</div>`
})
export class DynamicBarChartWidgetComponent {
  @Input() barchartItem: any;

  barChartDivId = '';
  widgetsGridsterItemArray: any[] = [];
  public options4: any = {
    chart: {
      height: (9 / 16 * 100) + '%',
      backgroundColor: 'rgba(0,0,0,0)'
    },
    title: {
      text: ''
    },
    xAxis: {
      categories: ['Jan', 'Feb', 'Mar', 'Apr', 'May']
    },
    labels: {
      items: [{
        //html: 'How you have been spending',
        style: {
          left: '50px',
          top: '18px',
          color: ( // theme
            Highcharts.defaultOptions.title.style &&
            Highcharts.defaultOptions.title.style.color
          ) || 'black'
        }
      }]
    },
    series: [{
      type: 'column',
      name: 'Your Income',
      data: [1, 3, 4, 2, 5]
    }, {
      type: 'column',
      name: 'Your Spending',
      data: [2, 2, 1, 4, 1]
    }, {
      type: 'spline',
      name: '',
      data: [1.5, 2.5, 3, 1.5, 3],
      marker: {
        lineWidth: 2,
        lineColor: Highcharts.getOptions().colors[3],
        fillColor: 'white'
      }
    }]
  }
  ngAfterViewInit() {
    if(this.barchartItem != undefined) {
      setTimeout(() => {
        Highcharts.chart(this.barChartDivId, this.options4);  
      }, 10);
    }
  }

  ngOnInit() {
    if(this.barchartItem != undefined) {
      this.barChartDivId = "barGraphcontainer"+this.barchartItem.WidgetId;
      $(document).ready(function () {
        setTimeout(function () {
          window.dispatchEvent(new Event('resize'));
        }, 100);
      });
    }
  }
}

// Component Created for Image Widget--
@Component({
  selector: 'html-widget',
  template: `<div class="widget">
    <div class="widget-header">
      <span class="widget-header-title"> Dynamic Html </span>
    </div>
    <div class="widget-area position-relative width100">
      <div class="widget-indicator-inner text-center pt-2">
        <i class="fa fa-lg fa-code" aria-hidden="true" style='font-size:18em;'></i>
      </div>
    </div>
  </div>`
})
export class DynamicHhtmlComponent {

  constructor() {}

  ngOnInit() {
    
  }

}

/* ---  below highcharts components are created for page preview while designing..
    which are same as above highcharts widgets, but created due to facing issue in page preview in popup --- */

// Component Created for Saving Trends Widget for page preview while designing--
@Component({
  selector: 'savingTrendPreview',
  template: `<div class="widget">
    <div class="widget">
    <div class="widget-header">
        <span class="widget-header-title"> Your Saving Trends </span>
    </div>
    <div class="widget-area position-relative width100">
      <div class="text-right" style="font-size:20px"><span>+5.6%</span><span class="pl-3">+3.5%</span></div>
      <div id="savingTrendsPreviewContainer" class="p-3"></div>        
    </div>
</div>`
})
export class SavingTrendsPreviewComponent {
  @Input()
  widgetsGridsterItemArray: any[] = [];
  public options4: any = {
    chart: {
      height: (9 / 16 * 100) + '%',
      backgroundColor: 'rgba(0,0,0,0)'
    },
    title: {
      text: ''
    },
    xAxis: {
      categories: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul']
    },
    labels: {
      items: [{
        //html: 'How you have been spending',
        style: {
          left: '50px',
          top: '18px',
          color: ( // theme
            Highcharts.defaultOptions.title.style &&
            Highcharts.defaultOptions.title.style.color
          ) || 'black'
        }
      }]
    },
    series: [{
      // type: 'spline',
      name: '',
      data: [1.5, 2.5, 3, 1.5, 3, 2, 4],
      marker: {
        lineWidth: 1,
        lineColor: Highcharts.getOptions().colors[3],
        fillColor: 'white'
      }
    }]
  }
  ngAfterViewInit() {
    setTimeout(() => {
      Highcharts.chart('savingTrendsPreviewContainer', this.options4);      
    }, 100);
  }

  ngOnInit() {
    $(document).ready(function () {
      setTimeout(function () {
        window.dispatchEvent(new Event('resize'));
      }, 10);
    });
  }
}

// Component Created for Spending Trends Widget for page preview while designing--
@Component({
  selector: 'spendingTrendsPreivew',
  template: `<div class="widget">
    <div class="widget">
    <div class="widget-header">
        <span class="widget-header-title"> Spending Trends </span>
    </div>
    <div class="widget-area position-relative width100">
          <div id="spendingTrendsPreviewcontainer" class="p-3"></div>       
    </div>
</div>`
})
export class SpendindTrendsPreviewComponent {
  @Input()
  widgetsGridsterItemArray: any[] = [];
  public options4: any = {
    chart: {
      height: (9 / 16 * 100) + '%',
      backgroundColor: 'rgba(0,0,0,0)'
    },
    title: {
      text: ''
    },
    xAxis: {
      categories: ['Jan', 'Feb', 'Mar', 'Apr', 'May']
    },
    labels: {
      items: [{
        //html: 'How you have been spending',
        style: {
          left: '50px',
          top: '18px',
          color: ( // theme
            Highcharts.defaultOptions.title.style &&
            Highcharts.defaultOptions.title.style.color
          ) || 'black'
        }
      }]
    },
    series: [{
      type: 'column',
      name: 'Your Income',
      data: [1, 3, 4, 2, 5]
    }, {
      type: 'column',
      name: 'Your Spending',
      data: [2, 2, 1, 4, 1]
    }, {
      type: 'spline',
      name: '',
      data: [1.5, 2.5, 3, 1.5, 3],
      marker: {
        lineWidth: 2,
        lineColor: Highcharts.getOptions().colors[3],
        fillColor: 'white'
      }
    }]
  }
  ngAfterViewInit() {
    setTimeout(() => {
      Highcharts.chart('spendingTrendsPreviewcontainer', this.options4);  
    }, 10);
  }

  ngOnInit() {
    $(document).ready(function () {
      setTimeout(function () {
        window.dispatchEvent(new Event('resize'));
      }, 100);
    });
  }
}

// Component Created for Analytics Widget for page preview while designing--
@Component({
  selector: 'analyticsPreview',
  template: `<div class="widget">
    <div class="widget">
    <div class="widget-header">
        <span class="widget-header-title"> Analytics </span>
    </div>
    <div class="widget-area position-relative width100">       
        <div id="chartWidgetPiePreviewcontainer" class="p-3"></div>
    </div>
</div>`
})
export class AnalyticsWidgetPreviewComponent {
  @Input()
  widgetsGridsterItemArray: any[] = [];

  public options4: any = {
    chart: {
      backgroundColor: 'rgba(0,0,0,0)',
      plotBorderWidth: null,
      plotShadow: false,
      type: 'pie',
      height: (9 / 16 * 100) + '%',
    },
    title: {
      text: ''
    },
    tooltip: {
      pointFormat: '{series.name}: <b>{point.percentage:.1f}%</b>'
    },
    accessibility: {
      point: {
        valueSuffix: '%'
      }
    },
    plotOptions: {
      pie: {
        allowPointSelect: true,
        cursor: 'pointer',
        dataLabels: {
          enabled: true,
          format: '<b>{point.name}</b>: {point.percentage:.1f} %'
        },
        showInLegend: false
      }
    },
    series: [{
      name: 'Percentage',
      colorByPoint: true,
      data: [
        {
          name: 'Cutomer Information',
          y: 11.84
        }, {
          name: 'Account Information',
          y: 10.85
        }, {
          name: 'Image',
          y: 4.67
        }, {
          name: 'Video',
          y: 4.18
        }, {
          name: 'News Alerts',
          y: 7.05
        }]
    }]
  }

  ngAfterViewInit() {
    setTimeout(() => {
      Highcharts.chart('chartWidgetPiePreviewcontainer', this.options4);      
    }, 100);
  }

  ngOnInit() {
    debugger
    $(document).ready(function () {
      setTimeout(function () {
        window.dispatchEvent(new Event('resize'));
      }, 10);
    });
  }
}

// Component Created for Dynamic line chart Widget for page preview while designing ----
@Component({
  selector: 'DynamicLineChartWidgetPreview',
  template: `<div class="widget">
    <div class="widget">
    <div class="widget-header">
        <span class="widget-header-title"> Your Saving Trends </span>
    </div>
    <div class="widget-area position-relative width100">
      <div class="text-right" style="font-size:20px"><span>+5.6%</span><span class="pl-3">+3.5%</span></div>
      <div [attr.id]="barChartDivId" class="p-3"></div>          
    </div>
</div>`
})
export class DynamicLineChartWidgetPreviewComponent {
  @Input() linechartItem: any;
  widgetsGridsterItemArray: any[] = [];
  barChartDivId = '';
  public options4: any = {
    chart: {
      height: (9 / 16 * 100) + '%',
      backgroundColor: 'rgba(0,0,0,0)'
    },
    title: {
      text: ''
    },
    xAxis: {
      categories: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul']
    },
    labels: {
      items: [{
        //html: 'How you have been spending',
        style: {
          left: '50px',
          top: '18px',
          color: ( // theme
            Highcharts.defaultOptions.title.style &&
            Highcharts.defaultOptions.title.style.color
          ) || 'black'
        }
      }]
    },
    series: [{
      // type: 'spline',
      name: '',
      data: [1.5, 2.5, 3, 1.5, 3, 2, 4],
      marker: {
        lineWidth: 1,
        lineColor: Highcharts.getOptions().colors[3],
        fillColor: 'white'
      }
    }]
  }

  ngAfterViewInit() {
    if(this.linechartItem != undefined) {
      setTimeout(() => {
        Highcharts.chart(this.barChartDivId, this.options4);  
      }, 10);
    }
  }

  ngOnInit() {
    if(this.linechartItem != undefined) {
      this.barChartDivId = "dynamiclinechartpreviewcontainer"+this.linechartItem.WidgetId;
      $(document).ready(function () {
        setTimeout(function () {
          window.dispatchEvent(new Event('resize'));
        }, 100);
      });
    }
  }

}

// Component Created for Dynamic Bar Chart Widget for page preview while designing ----
@Component({
  selector: 'DynamicBarChartWidgetPreview',
  template: `<div class="widget">
    <div class="widget">
    <div class="widget-header">
      <span class="widget-header-title"> Spending Trends </span>
    </div>
    <div class="widget-area position-relative width100">
      <div [attr.id]="barChartDivId" class="p-3"></div>
    </div>
</div>`
})
export class DynamicBarChartWidgetPreviewComponent {
  @Input() dynamicBarchartItem: any;
  widgetsGridsterItemArray: any[] = [];
  barChartDivId = '';

  public options4: any = {
    chart: {
      height: (9 / 16 * 100) + '%',
      backgroundColor: 'rgba(0,0,0,0)'
    },
    title: {
      text: ''
    },
    xAxis: {
      categories: ['Jan', 'Feb', 'Mar', 'Apr', 'May']
    },
    labels: {
      items: [{
        //html: 'How you have been spending',
        style: {
          left: '50px',
          top: '18px',
          color: ( // theme
            Highcharts.defaultOptions.title.style &&
            Highcharts.defaultOptions.title.style.color
          ) || 'black'
        }
      }]
    },
    series: [{
      type: 'column',
      name: 'Your Income',
      data: [1, 3, 4, 2, 5]
    }, {
      type: 'column',
      name: 'Your Spending',
      data: [2, 2, 1, 4, 1]
    }, {
      type: 'spline',
      name: '',
      data: [1.5, 2.5, 3, 1.5, 3],
      marker: {
        lineWidth: 2,
        lineColor: Highcharts.getOptions().colors[3],
        fillColor: 'white'
      }
    }]
  }

  ngAfterViewInit() {
    if(this.dynamicBarchartItem != undefined) {
      setTimeout(() => {
        Highcharts.chart(this.barChartDivId, this.options4);  
      }, 10);
    }
  }

  ngOnInit() {
    if(this.dynamicBarchartItem != undefined) {
      this.barChartDivId = "dynamicbarchartpreviewcontainer"+this.dynamicBarchartItem.WidgetId;
      $(document).ready(function () {
        setTimeout(function () {
          window.dispatchEvent(new Event('resize'));
        }, 100);
      });
    }
  }

}

// Component Created for Dynamic pier chart Widget for page preview while designing --
@Component({
  selector: 'DynamicPieChartWidgetPreview',
  template: `<div class="widget">
    <div class="widget">
    <div class="widget-header">
        <span class="widget-header-title"> Analytics </span>
    </div>
    <div class="widget-area position-relative width100">       
      <div [attr.id]="barChartDivId" class="p-3"></div>  
    </div>
</div>`
})
export class DynamicPieChartWidgetPreviewComponent {
  @Input() piechartItem: any;
  widgetsGridsterItemArray: any[] = [];
  barChartDivId = '';

  public options4: any = {
    chart: {
      backgroundColor: 'rgba(0,0,0,0)',
      plotBorderWidth: null,
      plotShadow: false,
      type: 'pie',
      height: (9 / 16 * 100) + '%',
    },
    title: {
      text: ''
    },
    tooltip: {
      pointFormat: '{series.name}: <b>{point.percentage:.1f}%</b>'
    },
    accessibility: {
      point: {
        valueSuffix: '%'
      }
    },
    plotOptions: {
      pie: {
        allowPointSelect: true,
        cursor: 'pointer',
        dataLabels: {
          enabled: true,
          format: '<b>{point.name}</b>: {point.percentage:.1f} %'
        },
        showInLegend: false
      }
    },
    series: [{
      name: 'Percentage',
      colorByPoint: true,
      data: [
        {
          name: 'Cutomer Information',
          y: 11.84
        }, {
          name: 'Account Information',
          y: 10.85
        }, {
          name: 'Image',
          y: 4.67
        }, {
          name: 'Video',
          y: 4.18
        }, {
          name: 'News Alerts',
          y: 7.05
        }]
    }]
  }

  ngAfterViewInit() {
    if(this.piechartItem != undefined) {
      setTimeout(() => {
        Highcharts.chart(this.barChartDivId, this.options4);  
      }, 10);
    }
  }

  ngOnInit() {
    if(this.piechartItem != undefined) {
      this.barChartDivId = "dynamicpiechartpreviewcontainer"+this.piechartItem.WidgetId;
      $(document).ready(function () {
        setTimeout(function () {
          window.dispatchEvent(new Event('resize'));
        }, 100);
      });
    }
  }

}
