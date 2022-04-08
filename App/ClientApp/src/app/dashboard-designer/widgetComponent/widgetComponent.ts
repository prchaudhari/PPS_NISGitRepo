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
import { BrowserModule, DomSanitizer } from '@angular/platform-browser';
import { Pipe, PipeTransform } from '@angular/core';
import { AppSettings } from '../../appsettings';

declare var require: any;
let Boost = require('highcharts/modules/boost');
let noData = require('highcharts/modules/no-data-to-display');
let More = require('highcharts/highcharts-more');
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
      <div class="widget-indicator-inner">
        <img [src]="ImageSrc" [id]="ImageId"/>
      </div>
    </div>
  </div>`
})
export class ImageComponent {

  @Input() imgItem: any;

  public ImageSrc: any;
  public ImageId: any;
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
      this.ImageId = 'Image' + this.imgItem.TempImageIdentifier;
    }
    else {
      this.ImageSrc = 'assets/images/icon-image.png';
      this.ImageId = 'Image1';
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

  ngAfterViewInit() {
    setTimeout(() => {
      if (this.imgItem != null && this.imgItem.WidgetSetting != null && this.imgItem.WidgetSetting != '' && this.testJSON(this.imgItem.WidgetSetting)) {
        let widgetSetting = JSON.parse(this.imgItem.WidgetSetting);
        if (widgetSetting.TempImageIdentifier != null) {
          if (widgetSetting.Height != null && widgetSetting.Height != 0) {
            $('#Image' + widgetSetting.TempImageIdentifier).css('height', widgetSetting.Height + 'px');
          } else {
            $('#Image' + widgetSetting.TempImageIdentifier).css('height', 'auto');
          }

          if (widgetSetting.Align != null) {
            var className = widgetSetting.Align == 1 ? 'text-left' : widgetSetting.Align == 2 ? 'text-right' : 'text-center';
            $('#Image' + widgetSetting.TempImageIdentifier).parent().addClass(className);
          }
        } else {
          $('#Image1').css('height', 'auto');
        }
      } 
    }, 100);
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

// Component Created for StaticHtml Widget--
@Component({
  selector: 'static-html-widget',
  template: `<div class="widget">
    <div class="widget-header">
      <span class="widget-header-title"> Static Html </span>
    </div>
    <div class="widget-area position-relative width100">
      <div class="widget-indicator-inner text-center pt-2">
        <i class="fa fa-lg fa-code" aria-hidden="true" style='font-size:18em;'></i>
      </div>
    </div>
  </div>`
})
export class StaticHtmlComponent {

  constructor() {}

  ngOnInit() {
    
  }

}


// Component Created for SegmentBasedContent Widget--
@Component({
  selector: 'Segment-based-content-widget',
  template: `<div class="widget">
    <div class="widget-area position-relative width100">
    <div [innerHtml]="html">
                    </div>
      <!-- <div class="widget-indicator-inner text-center pt-2">
        <i class="fa fa-lg fa-id-card-o" aria-hidden="true" style='font-size:18em;'></i>
      </div> -->
    </div>
  </div>`
})
export class SegmentBasedContentComponent {

  @Input() SegmentBasedContentItem: any;

  public html: any;

  constructor(private _http: HttpClient,
    private sanitizer: DomSanitizer) {
    }

  ngOnInit() {
    if (this.SegmentBasedContentItem != null && this.SegmentBasedContentItem.WidgetSetting != null && this.SegmentBasedContentItem.WidgetSetting != '' && this.testJSON(this.SegmentBasedContentItem.WidgetSetting)) {
      let widgetSetting = JSON.parse(this.SegmentBasedContentItem.WidgetSetting);
      if(widgetSetting.length > 0)
      {
        this.html = this.sanitizer.bypassSecurityTrustHtml(widgetSetting[0].Html);
      }
      else
      {
        this.html = '';
      }
    }
    else {
      this.html = '';
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

  ngAfterViewInit() {
    setTimeout(() => {
      if (this.SegmentBasedContentItem != null && this.SegmentBasedContentItem.WidgetSetting != null && this.SegmentBasedContentItem.WidgetSetting != '' && this.testJSON(this.SegmentBasedContentItem.WidgetSetting)) {
        let widgetSetting = JSON.parse(this.SegmentBasedContentItem.WidgetSetting);
      } 
    }, 100);
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

// Component Created for Customer Details Widget -- Nedbank
@Component({
  selector: 'CustomerDetails',
  template: `<div class="widget">
    <div class="widget-area height100">
       <div class='card border-0'>
          <div class="card-body CustomerDetails">
          MR KOENA SOLOMON MOLOTO <br>1917 THAGE STREET <br>MAMELODI GARDENS <br>PRETORIA <br>0122<br>
          </div>
      </div>
    </div>
  </div>`
})
export class CustomerDetailsComponent {
  @Input()
  widgetsGridsterItemArray: any[] = [];
}

// Component Created for Bank Details Widget -- Nedbank
@Component({
  selector: 'BankDetails',
  template: `<div class="widget">
    <div class="widget-area height100">
      <div class='card border-0'>
          <div class="card-body BranchDetails">
              BankName<br>Address Line1, City, ZipCode<br>Address Line2, City, ZipCode<br>Country Name
              <br>Bank VAT Reg No XXXXXXXXXX
          </div>
          <div class="ConactCenterDiv text-success float-right pt-3">
              Nedbank Private Wealth Service Suite: XXXX XXX XXX
          </div>
      </div>
    </div>
  </div>`
})
export class BankDetailsComponent {
  @Input()
  widgetsGridsterItemArray: any[] = [];
}

// Component Created for Bank Details Widget -- Nedbank
@Component({
  selector: 'WealthBankDetails',
  template: `<div class="widget">
    <div class="widget-area height100">
      <div class='card border-0'>
          <div class="card-body BranchDetails">
              BankName<br>Address Line1, City, ZipCode<br>Address Line2, City, ZipCode<br>Country Name
              <br>Bank VAT Reg No XXXXXXXXXX
          </div>
          <div class="ConactCenterDiv text-success-w float-right pt-3">
              Nedbank Private Wealth Service Suite: XXXX XXX XXX
          </div>
      </div>
    </div>
  </div>`
})
export class WealthBankDetailsComponent {
  @Input()
  widgetsGridsterItemArray: any[] = [];
}


// Component Created for Investment Portfolio Statement Widget -- Nedbank
@Component({
  selector: 'InvestmentPortfolioStatement',
  template: `<div class="widget">
    <div class="widget-area height100">
      <div class="row">
          <div class='col-lg-12'>
              <div class='card border-0'>
                  <div class='card-body text-left'>
                      <div class="card-body-header-name pb-3">Dear Customer Name</div>
                      <div class='card-body-header pb-2'>Investment portfolio statement</div>
                      <div class='row pb-1'>
                          <div class='col-lg-4 pr-1'>
                              <div class='TotalAmountDetailsDiv'>
                                  <span class='fnt-14'>Current investor balance</span><br><span class="fnt-20">Total Closing Balance</span>&nbsp;<br>
                              </div>
                          </div>
                          <div class='col-lg-8 pl-0'>
                              <div class='TotalAmountDetailsDiv'></div>
                          </div>
                      </div>

                      <div class="pt-1 pb-2" style="background-color:#f3f3f3">
                          <table class="customTable mt-2" border="0" id="portfolio">
                              <tbody class="fnt-13">
                                  <tr>
                                      <td class="w-25">Account type:</td>
                                      <td class="w-25 text-right pr-4 text-success">Investment</td>
                                      <td class="w-25">Statement Day:</td>
                                      <td class="w-25 text-right text-success">Day of Statement</td>
                                  </tr>
                                  <tr>
                                      <td class="w-25">Investor no:</td>
                                      <td class="w-25 text-right pr-4 text-success">Investor ID</td>
                                      <td class="w-25">Statement Period:</td>
                                      <td class="w-25 text-right text-success">Min to Max Transaction Date</td>
                                  </tr>
                                  <tr>
                                      <td class="w-25">Statement date:</td>
                                      <td class="w-25 text-right pr-4 text-success">Statement Date</td>
                                  </tr>
                              </tbody>
                          </table>
                      </div>
                  </div>
              </div>
          </div>
      </div>
    </div>
  </div>`
})
export class InvestmentPortfolioStatementComponent {
  @Input()
  widgetsGridsterItemArray: any[] = [];
}

// Component Created for Investment Wealth Portfolio Statement Widget -- Nedbank
@Component({
  selector: 'InvestmentWealthPortfolioStatementComponent',
  template: `<div class="widget">
    <div class="widget-area height100">
      <div class="row">
          <div class='col-lg-12'>
              <div class='card border-0'>
                  <div class='card-body text-left'>
                      <div class="card-body-header-w pb-3">Dear Customer Name</div>
                      <div class='card-body-header-w pb-2'>Investment portfolio statement</div>
                      <div class='row pb-1'>
                          <div class='col-lg-4 pr-1'>
                              <div class='TotalAmountDetailsDivW'>
                                  <span class='fnt-14'>Current investor balance</span><br><span class="fnt-20">Total Closing Balance</span>&nbsp;<br>
                              </div>
                          </div>
                          <div class='col-lg-8 pl-0'>
                              <div class='TotalAmountDetailsDivW'></div>
                          </div>
                      </div>

                      <div class="pt-1 pb-2" style="background-color:#f3f3f3">
                          <table class="customTable mt-2" border="0" id="portfolio">
                              <tbody class="fnt-13">
                                  <tr>
                                      <td class="w-25">Account type:</td>
                                      <td class="w-25 text-right pr-4 text-success">Investment</td>
                                      <td class="w-25">Statement Day:</td>
                                      <td class="w-25 text-right text-success">Day of Statement</td>
                                  </tr>
                                  <tr>
                                      <td class="w-25">Investor no:</td>
                                      <td class="w-25 text-right pr-4 text-success">Investor ID</td>
                                      <td class="w-25">Statement Period:</td>
                                      <td class="w-25 text-right text-success">Min to Max Transaction Date</td>
                                  </tr>
                                  <tr>
                                      <td class="w-25">Statement date:</td>
                                      <td class="w-25 text-right pr-4 text-success">Statement Date</td>
                                  </tr>
                              </tbody>
                          </table>
                      </div>
                  </div>
              </div>
          </div>
      </div>
    </div>
  </div>`
})
export class InvestmentWealthPortfolioStatementComponent {
  @Input()
  widgetsGridsterItemArray: any[] = [];
}

// Component Created for Investor Performance Widget -- Nedbank
@Component({
  selector: 'InvestorPerformance',
  template: `<div class="widget">
    <div class="widget-area height100">
      <div class='card border-0'>
          <div class='card-body text-left'>
              <div class='card-body-header pb-2'>Investor performance</div>
              <div class='InvestmentPermanaceDiv'>
                  <table class='InvestorPermanaceTable' border='0' id='InvestorPerformance'>
                      <tbody>
                          <tr>
                              <td class='w-50' colspan='2'><span class='text-success fnt-18'>Notice deposits</span></td>
                          </tr>
                          <tr>
                              <td class='w-50 fnt-14'>Opening balance</td>
                              <td class='w-50 fnt-14'>Closing balance</td>
                          </tr>
                          <tr>
                              <td class='w-50 fnt-20'>xxx.xx</td>
                              <td class='w-50 fnt-20'>xxx.xx</td>
                          </tr>
                      </tbody>
                  </table>
              </div>
          </div>
      </div>
    </div>
  </div>`
})
export class InvestorPerformanceComponent {
  @Input()
  widgetsGridsterItemArray: any[] = [];
}

// Component Created for Investor Performance Widget -- Nedbank
@Component({
  selector: 'WealthInvestorPerformance',
  template: `<div class="widget">
    <div class="widget-area height100">
      <div class='card border-0'>
          <div class='card-body text-left'>
              <div class='card-body-header-w pb-2'>Investor performance</div>
              <div class='InvestmentPermanaceDiv'>
                  <table class='InvestorPermanaceTable' border='0' id='InvestorPerformance'>
                      <tbody>
                          <tr>
                              <td class='w-50' colspan='2'><span class='text-success-w fnt-18'>Notice deposits</span></td>
                          </tr>
                          <tr>
                              <td class='w-50 fnt-14'>Opening balance</td>
                              <td class='w-50 fnt-14'>Closing balance</td>
                          </tr>
                          <tr>
                              <td class='w-50 fnt-20'>xxx.xx</td>
                              <td class='w-50 fnt-20'>xxx.xx</td>
                          </tr>
                      </tbody>
                  </table>
              </div>
          </div>
      </div>
    </div>
  </div>`
})
export class WealthInvestorPerformanceComponent {
  @Input()
  widgetsGridsterItemArray: any[] = [];
}

// Component Created for Breakdown Of Investment Accounts Widget -- Nedbank
@Component({
  selector: 'BreakdownOfInvestmentAccounts',
  template: `<div class="widget">
    <div class="widget-area height100">
      <div class='card border-0'>
          <div class='card-body text-left'>
              <div class="card-body-header pb-2">Breakdown of your investment accounts</div>
              
                  <div id='JustInvest-9929' class='tab-pane fade in active show'>
                      <div style="background-color: #f3f3f3;padding:10px 0px">
                          <h4 class="pl-25px"><span class='InvestmentProdDesc'>Product Desc</span></h4>
                          <table border="0" class="InvestmentDetail customTable">
                              <tbody>
                                  <tr>
                                      <td class="w-25">Investment no:</td>
                                      <td class="text-right w-25"><span>Investor ID + Investoment ID</span></td>
                                      <td class='w-25'>Opening date:</td>
                                      <td class="text-right w-25"><span>Account Open Date</span></td>
                                  </tr>
                                  <tr>
                                      <td class='w-25'>Current interest rate:</td>
                                      <td class="text-right w-25"><span>Interest rate % pa</span></td>
                                      <td class='w-25'>Maturity date:</td>
                                      <td class="text-right w-25"><span>Maturity date</span></td>
                                  </tr>
                                  <tr>
                                      <td class='w-25'>Interest disposal:</td>
                                      <td class="text-right w-25"><span>Interest Disposal value</span></td>
                                      <td class='w-25'>Notice period:</td>
                                      <td class="text-right w-25" ><span>Notice Period value</span></td>
                                  </tr>
                                  <tr>
                                      <td class='w-25'>Interest due:</td>
                                      <td class="text-right w-25" ><span>Accured Interest</span></td>
                                      <td class='w-25'>&nbsp;</td>
                                      <td class="text-right w-25" >&nbsp;</td>
                                  </tr>
                              </tbody>
                          </table>
                          <div class="InvestmentClosingBalanceDiv">
                              <span class='fn-14'>Balance at&nbsp;</span> <span class="text-success">Max Transaction date</span><br>
                              <span class="text-success fnt-20">Total Closing Balance</span>
                          </div>
                      </div>

                      <div class="pt-1">
                          <table id="TableWidget" style="width:100%;" class="table-striped InvestmentBreakdown customTable">
                              <thead>
                                  <tr class='ht-30'>
                                      <th class="w-15">Date</th>
                                      <th class="w-40">Description</th>
                                      <th class="w-15 text-right">Debit</th>
                                      <th class="w-15 text-right">Credit</th>
                                      <th class="w-15 text-right">Balance</th>
                                  </tr>
                              </thead>
                              <tbody>
                                  <tr>
                                      <td class="w-15">DD/MM/YYYY</td>
                                      <td class="w-40">Transaction Description</td>
                                      <td class="w-15 text-right">-</td>
                                      <td class="w-15 text-right">-</td>
                                      <td class="w-15 text-right">xxx.xx</td>
                                  </tr>
                                  <tr>
                                      <td class="w-15">DD/MM/YYYY</td>
                                      <td class="w-40">Transaction Description</td>
                                      <td class="w-15 text-right">-</td>
                                      <td class="w-15 text-right">x.xx</td>
                                      <td class="w-15 text-right">-</td>
                                  </tr>
                                  <tr>
                                      <td class="w-15">DD/MM/YYYY</td>
                                      <td class="w-40">Transaction Description</td>
                                      <td class="w-15 text-right">-</td>
                                      <td class="w-15 text-right">-</td>
                                      <td class="w-15 text-right">xxx.x</td>
                                  </tr>
                                  <tr>
                                      <td class="w-15">DD/MM/YYYY</td>
                                      <td class="w-40">Transaction Description</td>
                                      <td class="w-15 text-right">-</td>
                                      <td class="w-15 text-right">x.xx</td>
                                      <td class="w-15 text-right">-</td>
                                  </tr>
                                  <tr>
                                      <td class="w-15">DD/MM/YYYY</td>
                                      <td class="w-40">Transaction Description</td>
                                      <td class="w-15 text-right">-</td>
                                      <td class="w-15 text-right">-</td>
                                      <td class="w-15 text-right">xxx.xx</td>
                                  </tr>
                                  <tr>
                                      <td class="w-15">DD/MM/YYYY</td>
                                      <td class="w-40">Transaction Description</td>
                                      <td class="w-15 text-right">-</td>
                                      <td class="w-15 text-right">x.xx</td>
                                      <td class="w-15 text-right">-</td>
                                  </tr>
                                  <tr>
                                      <td class="w-15">DD/MM/YYYY</td>
                                      <td class="w-40">Transaction Description</td>
                                      <td class="w-15 text-right">-</td>
                                      <td class="w-15 text-right">-</td>
                                      <td class="w-15 text-right">xxx.x</td>
                                  </tr>
                              </tbody>
                          </table>
                      </div>

                      <br/><br/>

                      <div style="background-color: #f3f3f3;padding:10px 0px">
                          <h4 class="pl-25px"><span class="InvestmentProdDesc">Product Desc</span></h4>
                          <table border="0" class="InvestmentDetail customTable">
                              <tbody>
                                  <tr>
                                      <td class="w-25">Investment no:</td>
                                      <td class="text-right w-25"><span>Investor ID with Investoment ID</span></td>
                                      <td class='w-25'>Opening date:</td>
                                      <td class="text-right w-25"><span>Acc Open Date</span></td>
                                  </tr>
                                  <tr>
                                      <td class='w-25'>Current interest rate:</td>
                                      <td class="text-right w-25"><span>Interest rate % pa</span></td>
                                      <td class='w-25'>Maturity date:</td>
                                      <td class="text-right w-25"><span>Maturity date</span></td>
                                  </tr>
                                  <tr>
                                      <td class='w-25'>Interest disposal:</td>
                                      <td class="text-right w-25"><span>Interest Disposal value</span></td>
                                      <td class='w-25'>Notice period:</td>
                                      <td class="text-right w-25"><span>Notice Period value</span></td>
                                  </tr>
                                  <tr>
                                      <td class='w-25'>Interest due:</td>
                                      <td class="text-right w-25"><span>Accured Interest</span></td>
                                      <td class='w-25'>&nbsp;</td>
                                      <td class="text-right w-25">&nbsp;</td>
                                  </tr>
                              </tbody>
                          </table>
                          <div class="InvestmentClosingBalanceDiv">
                              <span class='fn-14'>Balance at&nbsp;</span> <span class="text-success">Max Transaction date</span><br>
                              <span class="text-success fnt-20">Total Closing Balance</span>
                          </div>
                      </div>

                      <div class="pt-1">
                          <table id="TableWidget" style="width:100%;" class="table-striped InvestmentBreakdown customTable">
                              <thead>
                                  <tr class='ht-30'>
                                      <th class="w-15">Date</th>
                                      <th class="w-40">Description</th>
                                      <th class="w-15 text-right">Debit</th>
                                      <th class="w-15 text-right">Credit</th>
                                      <th class="w-15 text-right">Balance</th>
                                  </tr>
                              </thead>
                              <tbody>
                                  <tr>
                                      <td class="w-15">DD/MM/YYYY</td>
                                      <td class="w-40">Transaction Description</td>
                                      <td class="w-15 text-right">-</td>
                                      <td class="w-15 text-right">-</td>
                                      <td class="w-15 text-right">xxx.xx</td>
                                  </tr>
                                  <tr>
                                      <td class="w-15">DD/MM/YYYY</td>
                                      <td class="w-40">Transaction Description</td>
                                      <td class="w-15 text-right">-</td>
                                      <td class="w-15 text-right">x.xx</td>
                                      <td class="w-15 text-right">-</td>
                                  </tr>
                                  <tr>
                                      <td class="w-15">DD/MM/YYYY</td>
                                      <td class="w-40">Transaction Description</td>
                                      <td class="w-15 text-right">-</td>
                                      <td class="w-15 text-right">-</td>
                                      <td class="w-15 text-right">xxx.x</td>
                                  </tr>
                                  <tr>
                                      <td class="w-15">DD/MM/YYYY</td>
                                      <td class="w-40">Transaction Description</td>
                                      <td class="w-15 text-right">-</td>
                                      <td class="w-15 text-right">x.xx</td>
                                      <td class="w-15 text-right">-</td>
                                  </tr>
                                  <tr>
                                      <td class="w-15">DD/MM/YYYY</td>
                                      <td class="w-40">Transaction Description</td>
                                      <td class="w-15 text-right">-</td>
                                      <td class="w-15 text-right">-</td>
                                      <td class="w-15 text-right">xxx.xx</td>
                                  </tr>
                                  <tr>
                                      <td class="w-15">DD/MM/YYYY</td>
                                      <td class="w-40">Transaction Description</td>
                                      <td class="w-15 text-right">-</td>
                                      <td class="w-15 text-right">x.xx</td>
                                      <td class="w-15 text-right">-</td>
                                  </tr>
                                  <tr>
                                      <td class="w-15">DD/MM/YYYY</td>
                                      <td class="w-40">Transaction Description</td>
                                      <td class="w-15 text-right">-</td>
                                      <td class="w-15 text-right">-</td>
                                      <td class="w-15 text-right">xxx.x</td>
                                  </tr>
                              </tbody>
                          </table>
                      </div>
                  
              </div>

          </div>
      </div>
    </div>
  </div>`
})
export class BreakdownOfInvestmentAccountsComponent {
  @Input()
  widgetsGridsterItemArray: any[] = [];
}

// Component Created for Wealth Breakdown Of Investment Accounts Widget -- Nedbank
@Component({
  selector: 'WealthBreakdownOfInvestmentAccounts',
  template: `<div class="widget">
    <div class="widget-area height100">
      <div class='card border-0'>
          <div class='card-body text-left'>
              <div class="card-body-header-w pb-2">Breakdown of your investment accounts</div>
              
                  <div id='JustInvest-9929' class='tab-pane fade in active show'>
                      <div style="background-color: #f3f3f3;padding:10px 0px">
                          <h4 class="pl-25px"><span class='InvestmentProdDescW'>Product Desc</span></h4>
                          <table border="0" class="InvestmentDetailW customTable">
                              <tbody>
                                  <tr>
                                      <td class="w-25">Investment no:</td>
                                      <td class="text-right w-25"><span>Investor ID + Investoment ID</span></td>
                                      <td class='w-25'>Opening date:</td>
                                      <td class="text-right w-25"><span>Account Open Date</span></td>
                                  </tr>
                                  <tr>
                                      <td class='w-25'>Current interest rate:</td>
                                      <td class="text-right w-25"><span>Interest rate % pa</span></td>
                                      <td class='w-25'>Maturity date:</td>
                                      <td class="text-right w-25"><span>Maturity date</span></td>
                                  </tr>
                                  <tr>
                                      <td class='w-25'>Interest disposal:</td>
                                      <td class="text-right w-25"><span>Interest Disposal value</span></td>
                                      <td class='w-25'>Notice period:</td>
                                      <td class="text-right w-25" ><span>Notice Period value</span></td>
                                  </tr>
                                  <tr>
                                      <td class='w-25'>Interest due:</td>
                                      <td class="text-right w-25" ><span>Accured Interest</span></td>
                                      <td class='w-25'>&nbsp;</td>
                                      <td class="text-right w-25" >&nbsp;</td>
                                  </tr>
                              </tbody>
                          </table>
                          <div class="InvestmentClosingBalanceDiv">
                              <span class='fn-14'>Balance at&nbsp;</span> <span class="text-success-w">Max Transaction date</span><br>
                              <span class="text-success-w fnt-20">Total Closing Balance</span>
                          </div>
                      </div>

                      <div class="pt-1">
                          <table id="TableWidget" style="width:100%;" class="table-striped InvestmentBreakdownW customTable">
                              <thead>
                                  <tr class='ht-30'>
                                      <th class="w-15">Date</th>
                                      <th class="w-40">Description</th>
                                      <th class="w-15 text-right">Debit</th>
                                      <th class="w-15 text-right">Credit</th>
                                      <th class="w-15 text-right">Balance</th>
                                  </tr>
                              </thead>
                              <tbody>
                                  <tr>
                                      <td class="w-15">DD/MM/YYYY</td>
                                      <td class="w-40">Transaction Description</td>
                                      <td class="w-15 text-right">-</td>
                                      <td class="w-15 text-right">-</td>
                                      <td class="w-15 text-right">xxx.xx</td>
                                  </tr>
                                  <tr>
                                      <td class="w-15">DD/MM/YYYY</td>
                                      <td class="w-40">Transaction Description</td>
                                      <td class="w-15 text-right">-</td>
                                      <td class="w-15 text-right">x.xx</td>
                                      <td class="w-15 text-right">-</td>
                                  </tr>
                                  <tr>
                                      <td class="w-15">DD/MM/YYYY</td>
                                      <td class="w-40">Transaction Description</td>
                                      <td class="w-15 text-right">-</td>
                                      <td class="w-15 text-right">-</td>
                                      <td class="w-15 text-right">xxx.x</td>
                                  </tr>
                                  <tr>
                                      <td class="w-15">DD/MM/YYYY</td>
                                      <td class="w-40">Transaction Description</td>
                                      <td class="w-15 text-right">-</td>
                                      <td class="w-15 text-right">x.xx</td>
                                      <td class="w-15 text-right">-</td>
                                  </tr>
                                  <tr>
                                      <td class="w-15">DD/MM/YYYY</td>
                                      <td class="w-40">Transaction Description</td>
                                      <td class="w-15 text-right">-</td>
                                      <td class="w-15 text-right">-</td>
                                      <td class="w-15 text-right">xxx.xx</td>
                                  </tr>
                                  <tr>
                                      <td class="w-15">DD/MM/YYYY</td>
                                      <td class="w-40">Transaction Description</td>
                                      <td class="w-15 text-right">-</td>
                                      <td class="w-15 text-right">x.xx</td>
                                      <td class="w-15 text-right">-</td>
                                  </tr>
                                  <tr>
                                      <td class="w-15">DD/MM/YYYY</td>
                                      <td class="w-40">Transaction Description</td>
                                      <td class="w-15 text-right">-</td>
                                      <td class="w-15 text-right">-</td>
                                      <td class="w-15 text-right">xxx.x</td>
                                  </tr>
                              </tbody>
                          </table>
                      </div>

                      <br/><br/>

                      <div style="background-color: #f3f3f3;padding:10px 0px">
                          <h4 class="pl-25px"><span class="InvestmentProdDescW">Product Desc</span></h4>
                          <table border="0" class="InvestmentDetailW customTable">
                              <tbody>
                                  <tr>
                                      <td class="w-25">Investment no:</td>
                                      <td class="text-right w-25"><span>Investor ID with Investoment ID</span></td>
                                      <td class='w-25'>Opening date:</td>
                                      <td class="text-right w-25"><span>Acc Open Date</span></td>
                                  </tr>
                                  <tr>
                                      <td class='w-25'>Current interest rate:</td>
                                      <td class="text-right w-25"><span>Interest rate % pa</span></td>
                                      <td class='w-25'>Maturity date:</td>
                                      <td class="text-right w-25"><span>Maturity date</span></td>
                                  </tr>
                                  <tr>
                                      <td class='w-25'>Interest disposal:</td>
                                      <td class="text-right w-25"><span>Interest Disposal value</span></td>
                                      <td class='w-25'>Notice period:</td>
                                      <td class="text-right w-25"><span>Notice Period value</span></td>
                                  </tr>
                                  <tr>
                                      <td class='w-25'>Interest due:</td>
                                      <td class="text-right w-25"><span>Accured Interest</span></td>
                                      <td class='w-25'>&nbsp;</td>
                                      <td class="text-right w-25">&nbsp;</td>
                                  </tr>
                              </tbody>
                          </table>
                          <div class="InvestmentClosingBalanceDiv">
                              <span class='fn-14'>Balance at&nbsp;</span> <span class="text-success">Max Transaction date</span><br>
                              <span class="text-success fnt-20">Total Closing Balance</span>
                          </div>
                      </div>

                      <div class="pt-1">
                          <table id="TableWidget" style="width:100%;" class="table-striped InvestmentBreakdownW customTable">
                              <thead>
                                  <tr class='ht-30'>
                                      <th class="w-15">Date</th>
                                      <th class="w-40">Description</th>
                                      <th class="w-15 text-right">Debit</th>
                                      <th class="w-15 text-right">Credit</th>
                                      <th class="w-15 text-right">Balance</th>
                                  </tr>
                              </thead>
                              <tbody>
                                  <tr>
                                      <td class="w-15">DD/MM/YYYY</td>
                                      <td class="w-40">Transaction Description</td>
                                      <td class="w-15 text-right">-</td>
                                      <td class="w-15 text-right">-</td>
                                      <td class="w-15 text-right">xxx.xx</td>
                                  </tr>
                                  <tr>
                                      <td class="w-15">DD/MM/YYYY</td>
                                      <td class="w-40">Transaction Description</td>
                                      <td class="w-15 text-right">-</td>
                                      <td class="w-15 text-right">x.xx</td>
                                      <td class="w-15 text-right">-</td>
                                  </tr>
                                  <tr>
                                      <td class="w-15">DD/MM/YYYY</td>
                                      <td class="w-40">Transaction Description</td>
                                      <td class="w-15 text-right">-</td>
                                      <td class="w-15 text-right">-</td>
                                      <td class="w-15 text-right">xxx.x</td>
                                  </tr>
                                  <tr>
                                      <td class="w-15">DD/MM/YYYY</td>
                                      <td class="w-40">Transaction Description</td>
                                      <td class="w-15 text-right">-</td>
                                      <td class="w-15 text-right">x.xx</td>
                                      <td class="w-15 text-right">-</td>
                                  </tr>
                                  <tr>
                                      <td class="w-15">DD/MM/YYYY</td>
                                      <td class="w-40">Transaction Description</td>
                                      <td class="w-15 text-right">-</td>
                                      <td class="w-15 text-right">-</td>
                                      <td class="w-15 text-right">xxx.xx</td>
                                  </tr>
                                  <tr>
                                      <td class="w-15">DD/MM/YYYY</td>
                                      <td class="w-40">Transaction Description</td>
                                      <td class="w-15 text-right">-</td>
                                      <td class="w-15 text-right">x.xx</td>
                                      <td class="w-15 text-right">-</td>
                                  </tr>
                                  <tr>
                                      <td class="w-15">DD/MM/YYYY</td>
                                      <td class="w-40">Transaction Description</td>
                                      <td class="w-15 text-right">-</td>
                                      <td class="w-15 text-right">-</td>
                                      <td class="w-15 text-right">xxx.x</td>
                                  </tr>
                              </tbody>
                          </table>
                      </div>
                  
              </div>

          </div>
      </div>
    </div>
  </div>`
})
export class WealthBreakdownOfInvestmentAccountsComponent {
  @Input()
  widgetsGridsterItemArray: any[] = [];
}

// Component Created for Explanatory Notes Widget -- Nedbank
@Component({
  selector: 'ExplanatoryNotes',
  template: `<div class="widget">
    <div class="widget-area height100">
      <div class='card border-0'>
          <div class='card-body text-left'>
              <div class="card-body-header pb-2">Explanatory notes</div>
              <div class='ExplanatoryNotes'>
                  <span>Fixed deposits — Total balance of all your fixed-type accounts.</span><br />
                  <span>Notice deposits — Total balance of all your notice deposit accounts.</span><br />
                  <span>Linked deposits — Total balance of all your linked-type accounts.</span>
              </div>
          </div>
      </div>
    </div>
  </div>`
})
export class ExplanatoryNotesComponent {
  @Input()
  widgetsGridsterItemArray: any[] = [];
}

// Component Created for Explanatory Notes Widget -- Nedbank
@Component({
  selector: 'WealthExplanatoryNotes',
  template: `<div class="widget">
    <div class="widget-area height100">
      <div class='card border-0'>
          <div class='card-body text-left'>
              <div class="card-body-header-w pb-2">Explanatory notes</div>
              <div class='ExplanatoryNotes'>
                  <span>Fixed deposits — Total balance of all your fixed-type accounts.</span><br />
                  <span>Notice deposits — Total balance of all your notice deposit accounts.</span><br />
                  <span>Linked deposits — Total balance of all your linked-type accounts.</span>
              </div>
          </div>
      </div>
    </div>
  </div>`
})
export class WealthExplanatoryNotesComponent {
  @Input()
  widgetsGridsterItemArray: any[] = [];
}

// Component Created for Nedbank service Widget -- Nedbank
@Component({
  selector: 'NedbankService',
  template: `<div class="widget">
    <div class="widget-area height100">
      <div class="card border-0">
          <div class="card-body text-left">
              <div class="ServicesDiv">
                  <div class="serviceHeader pb-2">Nedbank service message header</div>
                  <span>Nedbank service message text</span><br>
                  <span>Nedbank service message text</span><br>
                  <span>Nedbank service message text</span>
              </div>
          </div>
      </div>
    </div>
  </div>`
})
export class NedbankServiceComponent {
  @Input()
  widgetsGridsterItemArray: any[] = [];
}

// Component Created for Nedbank service Widget -- Nedbank
@Component({
  selector: 'WealthNedbankService',
  template: `<div class="widget">
    <div class="widget-area height100">
      <div class="card border-0">
          <div class="card-body text-left">
          <div class='card-body-header-w pb-2'>Nedbank services</div>

          </div>
      </div>
    </div>
  </div>`
})

export class WealthNedbankServiceComponent {
  @Input()
  widgetsGridsterItemArray: any[] = [];
}

// Component Created for Personal loan detail Widget -- Nedbank
@Component({
  selector: 'PersonalLoanDetail',
  template: `<div class="widget">
    <div class="widget-area height100">
      <div class='card border-0'>
          <div class='card-body text-left pb-1 pt-1'>
              <div class='card-body-header pb-2'>Personal loan statement</div>
              <div class='row pb-1'>
                  <div class='col-lg-4 col-sm-4 pr-1'>
                      <div class='LoanAmountDetailsDiv'>
                          <span class="fnt-10pt">Loan Amount</span><br><span class="fnt-14pt">Loan Amount</span>&nbsp;<br>
                      </div>
                  </div>
                  <div class='col-lg-4 col-sm-4 pr-1 pl-0'>
                      <div class='LoanAmountDetailsDiv'>
                          <span class="fnt-10pt">Balance outstanding</span><br><span class="fnt-14pt">Outstanding balance</span>&nbsp;<br>
                      </div>
                  </div>
                  <div class='col-lg-4 col-sm-4 pl-0'>
                      <div class='LoanAmountDetailsDiv'>
                          <span class="fnt-10pt">Now due</span><br><span class="fnt-14pt">Now Due value</span>&nbsp;<br>
                      </div>
                  </div>
              </div>

              <div class="pt-2 pb-2" style="background-color:#f3f3f3">
                  <h4 class="pl-25px"><span class="NedbankPersonalLoanTxt">Nedbank personal loan</span></h4>
                  <table class="customTable mt-2" border="0">
                      <tbody>
                          <tr>
                              <td class="w-25">Account Number:</td>
                              <td class="w-25 text-right pr-4 text-success">Account Number</td>
                              <td class="w-25"></td>
                              <td class="w-25 text-right text-success"></td>
                          </tr>
                          <tr>
                              <td class="w-25">Statement date:</td>
                              <td class="w-25 text-right pr-4 text-success">Statement date</td>
                              <td class="w-25">Arrears:</td>
                              <td class="w-25 text-right text-success">Arrears amount</td>
                          </tr>
                          <tr>
                              <td class="w-25">Statement period:</td>
                              <td class="w-25 text-right pr-4 text-success">Min to Max Transaction Date</td>
                              <td class="w-25">Annual rate of interest:</td>
                              <td class="w-25 text-right pr-4 text-success">annual rate value</td>
                          </tr>
                          <tr>
                              <td class="w-25">Monthly instalment:</td>
                              <td class="w-25 text-right pr-4 text-success">Monthly instalment amount</td>
                              <td class="w-25">Original term (months):</td>
                              <td class="w-25 text-right pr-4 text-success">Month value</td>
                          </tr>
                          <tr>
                              <td class="w-25">Due by date:</td>
                              <td class="w-25 text-right pr-4 text-success">DD/MM/YYYY</td>
                              <td class="w-25"></td>
                              <td class="w-25 text-right pr-4 text-success"></td>
                          </tr>
                      </tbody>
                  </table>
              </div>
          </div>
      </div>
    </div>
  </div>`
})
export class PersonalLoanDetailComponent {
  @Input()
  widgetsGridsterItemArray: any[] = [];
}

// Component Created for Personal loan transaction Widget -- Nedbank
@Component({
  selector: 'PersonalLoanTransaction',
  template: `<div class="widget">
    <div class="widget-area height100">
      <div class='card border-0'>
          <div class='card-body text-left pb-1 pt-1'>
              <div class="pt-1">
                  <table id="TableWidget" class="LoanTransactionTable customTable">
                      <thead>
                          <tr class="ht-30">
                              <th class="w-12">Post date</th>
                              <th class="w-12">Effective date</th>
                              <th class="w-40">Transaction</th>
                              <th class="w-12 text-right">Debit</th>
                              <th class="w-12 text-right">Credit</th>
                              <th class="w-12 text-right">Balance outstanding</th>
                          </tr>
                      </thead>
                  </table>

                  <div class="pt-0 overflow-auto" style="max-height:125px;">
                      <table class="LoanTransactionTable customTable">
                          <tbody>
                              <tr>
                                  <td class="w-12">DD/MM/YYYY</td>
                                  <td class="w-12">DD/MM/YYYY</td>
                                  <td class="w-40">Transaction Description</td>
                                  <td class="w-12 text-right">xxx.x</td>
                                  <td class="w-12 text-right">-</td>
                                  <td class="w-12 text-right">xxx.xx</td>
                              </tr>
                              <tr>
                                  <td class="w-12">DD/MM/YYYY</td>
                                  <td class="w-12">DD/MM/YYYY</td>
                                  <td class="w-40">Transaction Description</td>
                                  <td class="w-12 text-right">-</td>
                                  <td class="w-12 text-right">x.xx</td>
                                  <td class="w-12 text-right">xxx.xx</td>
                              </tr>
                              <tr>
                                  <td class="w-12">DD/MM/YYYY</td>
                                  <td class="w-12">DD/MM/YYYY</td>
                                  <td class="w-40">Transaction Description</td>
                                  <td class="w-12 text-right">-</td>
                                  <td class="w-12 text-right">xxx.xx</td>
                                  <td class="w-12 text-right">xxx.xx</td>
                              </tr>
                              <tr>
                                  <td class="w-12">DD/MM/YYYY</td>
                                  <td class="w-12">DD/MM/YYYY</td>
                                  <td class="w-40">Transaction Description</td>
                                  <td class="w-12 text-right">-</td>
                                  <td class="w-12 text-right">x.xx</td>
                                  <td class="w-12 text-right">xxx.xx</td>
                              </tr>
                              <tr>
                                  <td class="w-12">DD/MM/YYYY</td>
                                  <td class="w-12">DD/MM/YYYY</td>
                                  <td class="w-40">Transaction Description</td>
                                  <td class="w-12 text-right">xxx.xx</td>
                                  <td class="w-12 text-right">-</td>
                                  <td class="w-12 text-right">xxx.xx</td>
                              </tr>
                              <tr>
                                  <td class="w-12">DD/MM/YYYY</td>
                                  <td class="w-12">DD/MM/YYYY</td>
                                  <td class="w-40">Transaction Description</td>
                                  <td class="w-12 text-right">-</td>
                                  <td class="w-12 text-right">xx.xx</td>
                                  <td class="w-12 text-right">xxx.xx</td>
                              </tr>
                              <tr>
                                  <td class="w-12">DD/MM/YYYY</td>
                                  <td class="w-12">DD/MM/YYYY</td>
                                  <td class="w-40">Transaction Description</td>
                                  <td class="w-12 text-right">xx.xx</td>
                                  <td class="w-12 text-right">-</td>
                                  <td class="w-12 text-right">xxx.xx</td>
                              </tr>
                          </tbody>
                      </table>
                  </div>
              </div>
          </div>
      </div>
    </div>
  </div>`
})
export class PersonalLoanTransactionComponent {
  @Input()
  widgetsGridsterItemArray: any[] = [];
}

// Component Created for Personal loan payment due details Widget -- Nedbank
@Component({
  selector: 'PersonalLoanPaymentDue',
  template: `<div class="widget">
    <div class="widget-area height100">
      <div class='card border-0'>
          <div class='card-body text-left pb-1 pt-1'>
              <div class='card-body-sub-header pb-2'>Payment Due</div>
              <div class="d-flex flex-row">
                  <div class="paymentDueHeaderBlock mr-1">After 120 + days</div>
                  <div class="paymentDueHeaderBlock mr-1">After 90 days</div>
                  <div class="paymentDueHeaderBlock mr-1">After 60 days</div>
                  <div class="paymentDueHeaderBlock mr-1">After 30 days</div>
                  <div class="paymentDueHeaderBlock">Current</div>
              </div>
              <div class="d-flex flex-row mt-1">
                  <div class="paymentDueFooterBlock mr-1">x.xx</div>
                  <div class="paymentDueFooterBlock mr-1">x.xx</div>
                  <div class="paymentDueFooterBlock mr-1">x.xx</div>
                  <div class="paymentDueFooterBlock mr-1">x.xx</div>
                  <div class="paymentDueFooterBlock">x.xx</div>
              </div>
          </div>
      </div>
    </div>
  </div>`
})
export class PersonalLoanPaymentDueComponent {
  @Input()
  widgetsGridsterItemArray: any[] = [];
}

// Component Created for special message Widget -- Nedbank
@Component({
  selector: 'SpecialMessage',
  template: `<div class="widget">
    <div class="widget-area height100">
      <div class="card border-0">
          <div class="card-body text-left pb-1 pt-1">
              <div class="SpecialMessageDiv">
                  <div class="SpecialMessageHeader">Special message header Text</div>
                  <p>Personalize special message text 1</p>
                  <p>Personalize special message text 2</p>
              </div>
          </div>
      </div>
    </div>
  </div>`
})
export class SpecialMessageComponent {
  @Input()
  widgetsGridsterItemArray: any[] = [];
}

// Component Created for Personal loan Insurance message Widget -- Nedbank
@Component({
  selector: 'PL_Insurance',
  template: `<div class="widget">
    <div class="widget-area height100">
      <div class="card border-0">
          <div class="card-body text-left py-4">
            <div class="InsuranceMessageDiv">
                <div class="card-body-header pb-2">Insurance</div>
                <p>Insurance related Personalize special message text 1</p>
                <p>Insurance related Personalize special message text 2</p>
                <p>Insurance related Personalize special message text 3</p>
            </div>
          </div>
      </div>
    </div>
  </div>`
})
export class PersonalLoanInsuranceMessageComponent {
  @Input()
  widgetsGridsterItemArray: any[] = [];
}

// Component Created for Personal loan total amount detail Widget -- Nedbank
@Component({
  selector: 'PersonalLoanTotalAmountDetail',
  template: `<div class="widget">
    <div class="widget-area height100">
      <div class='card border-0'>
          <div class='card-body text-left pb-1 pt-4'>
              <div class='card-body-header pb-2'>Personal loan statement</div>
              <div class='row pb-1'>
                  <div class='col-lg-4 col-sm-4 pr-1'>
                      <div class='LoanAmountDetailsDiv'>
                          <span class="fnt-10pt">Loan Amount</span><br><span class="fnt-14pt">R121 765.00</span><br>
                      </div>
                  </div>
                  <div class='col-lg-4 col-sm-4 pr-1 pl-0'>
                      <div class='LoanAmountDetailsDiv'>
                          <span class="fnt-10pt">Balance outstanding</span><br><span class="fnt-14pt">R114 190.00</span><br>
                      </div>
                  </div>
                  <div class='col-lg-4 col-sm-4 pl-0'>
                      <div class='LoanAmountDetailsDiv'>
                          <span class="fnt-10pt">Now due</span><br><span class="fnt-14pt">R4 742.00</span><br>
                      </div>
                  </div>
              </div>
          </div>
      </div>
    </div>
  </div>`
})
export class PersonalLoanTotalAmountDetailComponent {
  @Input()
  widgetsGridsterItemArray: any[] = [];
}

// Component Created for Personal loan total amount detail Widget -- Nedbank
@Component({
  selector: 'PersonalLoanAccountsBreakdown',
  template: `<div class="widget">
    <div class="widget-area height100">
      <div class='card border-0'>
        <div class='card-body text-left py-0'>
            <div class="tab-content">
                <div id="PersonalLoan-4001" class="tab-pane fade in active show">
                    <div class="PersonalLoanDetailDiv">
                        <h4 class="pl-25px"><span class="NedbankPersonalLoanTxt">Nedbank personal loan</span></h4>
                        <table class="customTable mt-2" border="0">
                            <tbody>
                                <tr>
                                    <td class="w-25">Account Number:</td>
                                    <td class="w-25 text-right pr-4 text-success">8004334234001</td>
                                    <td class="w-25"></td>
                                    <td class="w-25 text-right text-success"></td>
                                </tr>
                                <tr>
                                    <td class="w-25">Statement date:</td>
                                    <td class="w-25 text-right pr-4 text-success">30/01/2021</td>
                                    <td class="w-25">Arrears:</td>
                                    <td class="w-25 text-right text-success">R0.00</td>
                                </tr>
                                <tr>
                                    <td class="w-25">Statement period:</td>
                                    <td class="w-25 text-right pr-4 text-success">01/12/2020 - 30/01/2021</td>
                                    <td class="w-25">Annual rate of interest:</td>
                                    <td class="w-25 text-right pr-4 text-success">24% pa</td>
                                </tr>
                                <tr>
                                    <td class="w-25">Monthly instalment:</td>
                                    <td class="w-25 text-right pr-4 text-success">R3 297.00</td>
                                    <td class="w-25">Original term (months):</td>
                                    <td class="w-25 text-right pr-4 text-success">36</td>
                                </tr>
                                <tr>
                                    <td class="w-25">Due by date:</td>
                                    <td class="w-25 text-right pr-4 text-success">28/02/2021</td>
                                    <td class="w-25"></td>
                                    <td class="w-25 text-right pr-4 text-success"></td>
                                </tr>
                            </tbody>
                        </table>
                    </div>

                    <div class="pt-1">
                        <table id="TableWidget" class="LoanTransactionTable customTable">
                            <thead>
                                <tr class="ht-30">
                                    <th class="w-12 text-center">Post date</th>
                                    <th class="w-12 text-center">Effective date</th>
                                    <th class="w-40">Transaction</th>
                                    <th class="w-12 text-right">Debit</th>
                                    <th class="w-12 text-right">Credit</th>
                                    <th class="w-12 text-right">Balance outstanding</th>
                                </tr>
                            </thead>
                        </table>
                        <div class="pt-0 overflow-auto" style="max-height:200px;">
                            <table class="LoanTransactionTable customTable">
                                <tbody>
                                    <tr>
                                        <td class="w-12 text-center">01/12/2020</td>
                                        <td class="w-12 text-center">01/12/2020</td>
                                        <td class="w-40">Interest Debit</td>
                                        <td class="w-12 text-right">R1 490.00</td>
                                        <td class="w-12 text-right">-</td>
                                        <td class="w-12 text-right">R74 298.00</td>
                                    </tr>
                                    <tr>
                                        <td class="w-12 text-center">03/12/2020</td>
                                        <td class="w-12 text-center">03/12/2020</td>
                                        <td class="w-40">Insurance Premium</td>
                                        <td class="w-12 text-right">R188.00</td>
                                        <td class="w-12 text-right">-</td>
                                        <td class="w-12 text-right">R74 487.00</td>
                                    </tr>
                                    <tr>
                                        <td class="w-12 text-center">15/12/2020</td>
                                        <td class="w-12 text-center">15/12/2020</td>
                                        <td class="w-40">Payment - Thank you</td>
                                        <td class="w-12 text-right">-</td>
                                        <td class="w-12 text-right">R3 297.00</td>
                                        <td class="w-12 text-right">R71 189.00</td>
                                    </tr>
                                    <tr>
                                        <td class="w-12 text-center">31/12/2020</td>
                                        <td class="w-12 text-center">31/12/2020</td>
                                        <td class="w-40">Monthly Admin Fee</td>
                                        <td class="w-12 text-right">R69.00</td>
                                        <td class="w-12 text-right">-</td>
                                        <td class="w-12 text-right">R71 258.00</td>
                                    </tr>
                                    <tr>
                                        <td class="w-12 text-center">01/01/2021</td>
                                        <td class="w-12 text-center">01/01/2021</td>
                                        <td class="w-40">Interest Debit</td>
                                        <td class="w-12 text-right">R1 512.00</td>
                                        <td class="w-12 text-right">-</td>
                                        <td class="w-12 text-right">R72 770.00</td>
                                    </tr>
                                </tbody>
                            </table>
                        </div>
                    </div>

                    <div class="py-2">
                        <div class='payment-due-header'>Payment Due</div>
                        <div class="d-flex flex-row">
                            <div class="paymentDueHeaderBlock mr-1">After 120 + days</div>
                            <div class="paymentDueHeaderBlock mr-1">After 90 days</div>
                            <div class="paymentDueHeaderBlock mr-1">After 60 days</div>
                            <div class="paymentDueHeaderBlock mr-1">After 30 days</div>
                            <div class="paymentDueHeaderBlock">Current</div>
                        </div>
                        <div class="d-flex flex-row mt-1">
                            <div class="paymentDueFooterBlock mr-1">R0.00</div>
                            <div class="paymentDueFooterBlock mr-1">R0.00</div>
                            <div class="paymentDueFooterBlock mr-1">R0.00</div>
                            <div class="paymentDueFooterBlock mr-1">R0.00</div>
                            <div class="paymentDueFooterBlock">R3 297.00</div>
                        </div>
                    </div>

                </div>

                <div id="PersonalLoan-6001" class="tab-pane fade">
                    <div class="PersonalLoanDetailDiv">
                        <h4 class="pl-25px"><span class="NedbankPersonalLoanTxt">Nedbank personal loan</span></h4>
                        <table class="customTable mt-2" border="0">
                            <tbody>
                                <tr>
                                    <td class="w-25">Account Number:</td>
                                    <td class="w-25 text-right pr-4 text-success">8003922986001</td>
                                    <td class="w-25"></td>
                                    <td class="w-25 text-right text-success"></td>
                                </tr>
                                <tr>
                                    <td class="w-25">Statement date:</td>
                                    <td class="w-25 text-right pr-4 text-success">30/01/2021</td>
                                    <td class="w-25">Arrears:</td>
                                    <td class="w-25 text-right text-success">R774.00</td>
                                </tr>
                                <tr>
                                    <td class="w-25">Statement period:</td>
                                    <td class="w-25 text-right pr-4 text-success">01/12/2020 - 30/01/2021</td>
                                    <td class="w-25">Annual rate of interest:</td>
                                    <td class="w-25 text-right pr-4 text-success">0% pa</td>
                                </tr>
                                <tr>
                                    <td class="w-25">Monthly instalment:</td>
                                    <td class="w-25 text-right pr-4 text-success">R671.00</td>
                                    <td class="w-25">Original term (months):</td>
                                    <td class="w-25 text-right pr-4 text-success">60</td>
                                </tr>
                                <tr>
                                    <td class="w-25">Due by date:</td>
                                    <td class="w-25 text-right pr-4 text-success">28/02/2021</td>
                                    <td class="w-25"></td>
                                    <td class="w-25 text-right pr-4 text-success"></td>
                                </tr>
                            </tbody>
                        </table>
                    </div>

                    <div class="pt-1">
                        <table id="TableWidget" class="LoanTransactionTable customTable">
                            <thead>
                                <tr class="ht-30">
                                    <th class="w-12 text-center">Post date</th>
                                    <th class="w-12 text-center">Effective date</th>
                                    <th class="w-40">Transaction</th>
                                    <th class="w-12 text-right">Debit</th>
                                    <th class="w-12 text-right">Credit</th>
                                    <th class="w-12 text-right">Balance outstanding</th>
                                </tr>
                            </thead>
                        </table>
                        <div class="pt-0 overflow-auto" style="max-height:200px;">
                            <table class="LoanTransactionTable customTable">
                                <tbody>
                                    <tr>
                                        <td class="w-12 text-center">01/12/2020</td>
                                        <td class="w-12 text-center">01/12/2020</td>
                                        <td class="w-40">Payment - Thank you</td>
                                        <td class="w-12 text-right">-</td>
                                        <td class="w-12 text-right">R671.00</td>
                                        <td class="w-12 text-right">R47 363.00</td>
                                    </tr>
                                    <tr>
                                        <td class="w-12 text-center">24/12/2020</td>
                                        <td class="w-12 text-center">24/12/2020</td>
                                        <td class="w-40">Payment - Thank you</td>
                                        <td class="w-12 text-right">-</td>
                                        <td class="w-12 text-right">R1 789.00</td>
                                        <td class="w-12 text-right">R45 574.00</td>
                                    </tr>
                                    <tr>
                                        <td class="w-12 text-center">24/12/2020</td>
                                        <td class="w-12 text-center">24/12/2020</td>
                                        <td class="w-40">Payment Reversal</td>
                                        <td class="w-12 text-right">R1 789.00</td>
                                        <td class="w-12 text-right">-</td>
                                        <td class="w-12 text-right">R47 363.00</td>
                                    </tr>
                                    <tr>
                                        <td class="w-12 text-center">31/12/2020</td>
                                        <td class="w-12 text-center">31/12/2020</td>
                                        <td class="w-40">Payment - Thank you</td>
                                        <td class="w-12 text-right">-</td>
                                        <td class="w-12 text-right">R671.00</td>
                                        <td class="w-12 text-right">R46 692.00</td>
                                    </tr>
                                    <tr>
                                        <td class="w-12 text-center">29/01/2020</td>
                                        <td class="w-12 text-center">29/01/2020</td>
                                        <td class="w-40">Payment - Thank you</td>
                                        <td class="w-12 text-right">-</td>
                                        <td class="w-12 text-right">R671.00</td>
                                        <td class="w-12 text-right">R46 021.00</td>
                                    </tr>
                                </tbody>
                            </table>
                        </div>
                    </div>

                    <div class="py-2">
                        <div class='payment-due-header'>Payment Due</div>
                        <div class="d-flex flex-row">
                            <div class="paymentDueHeaderBlock mr-1">After 120 + days</div>
                            <div class="paymentDueHeaderBlock mr-1">After 90 days</div>
                            <div class="paymentDueHeaderBlock mr-1">After 60 days</div>
                            <div class="paymentDueHeaderBlock mr-1">After 30 days</div>
                            <div class="paymentDueHeaderBlock">Current</div>
                        </div>
                        <div class="d-flex flex-row mt-1">
                            <div class="paymentDueFooterBlock mr-1">R0.00</div>
                            <div class="paymentDueFooterBlock mr-1">R0.00</div>
                            <div class="paymentDueFooterBlock mr-1">R103.00</div>
                            <div class="paymentDueFooterBlock mr-1">R671.00</div>
                            <div class="paymentDueFooterBlock">R671.00</div>
                        </div>
                    </div>

                </div>
            </div>
        </div>
    </div>
    </div>
  </div>`
})
export class PersonalLoanAccountsBreakdownComponent {
  @Input()
  widgetsGridsterItemArray: any[] = [];
}

// Component Created for Home loan total amount detail Widget -- Nedbank
@Component({
  selector: 'HomeLoanTotalAmountDetail',
  template: `<div class="widget">
    <div class="widget-area height100">
     <div class='card border-0 pt-4'>
          <div class='card-body text-left pb-1'>
              <div class='card-body-header pb-2'>New instalment</div>
              <div class='row'>
                  <div class='col-lg-4 pr-1'>
                      <div class='TotalAmountDetailsDiv'>
                          <span class="fnt-10pt">Total Loan Amount</span><br><span class="fnt-14pt">R432 969.00</span><br>
                      </div>
                  </div>
                  <div class='col-lg-8 pl-0 text-right'>
                      <div class='TotalAmountDetailsDiv'>
                          <span class="fnt-10pt">Balance outstanding</span><br><span class="fnt-14pt">R136 320.00</span>&nbsp;<br>
                      </div>
                  </div>
              </div>
          </div>
      </div>
    </div>
  </div>`
})
export class HomeLoanTotalAmountDetailComponent {
  @Input()
  widgetsGridsterItemArray: any[] = [];
}

// Component Created for Home loan accounts breakdown Widget -- Nedbank
@Component({
  selector: 'HomeLoanAccountsBreakdown',
  template: `<div class="widget">
    <div class="widget-area height100">
      <div class='card border-0 pt-4'>
        <div class='card-body text-left py-1'>
            <div class="tab-content">
                <div id='HomeLoan-4001' class='tab-pane fade in active show'>
                    <div class="HomeLoanDetailDiv">
                        <h4 class="pl-25px"><span class="NedbankHomeLoanTxt">Nedbank home loan</span></h4>
                        <table class="customTable mt-2" border="0">
                            <tbody>
                                <tr>
                                    <td class="w-25">Bond no:</td>
                                    <td class="w-25 text-right pr-4 text-success">8003876814001</td>
                                    <td class="w-25">Address:</td>
                                    <td class="w-25 text-right text-success" rowspan="3">ERF 44 THE COVES H <br />ARTBEESPOORT <br /></td>
                                </tr>
                                <tr>
                                    <td class="w-25">Instalment:</td>
                                    <td class="w-25 text-right pr-4 text-success">R4 149.00</td>
                                </tr>
                                <tr>
                                    <td class="w-25">Arrears:</td>
                                    <td class="w-25 text-right pr-4 text-success">R0.00</td>
                                </tr>
                                <tr>
                                    <td class="w-25">Interest rate:</td>
                                    <td class="w-25 text-right pr-4 text-success">5.75% pa</td>
                                    <td class="w-25">Registration date:</td>
                                    <td class="w-25 text-right pr-4 text-success">12/12/2016</td>
                                </tr>
                                <tr>
                                    <td class="w-25">Loan term:</td>
                                    <td class="w-25 text-right pr-4 text-success">300 months</td>
                                    <td class="w-25">Registered amount</td>
                                    <td class="w-25 text-right pr-4 text-success">R516 037.00</td>
                                </tr>
                            </tbody>
                        </table>
                    </div>

                    <div class="pt-1">
                        <table id="TableWidget" class="LoanTransactionTable customTable">
                            <thead>
                                <tr class="ht-30">
                                    <th class="w-12 text-center">Post date</th>
                                    <th class="w-12 text-center">Effective date</th>
                                    <th class="w-40">Transaction</th>
                                    <th class="w-12 text-right">Debit (R)</th>
                                    <th class="w-12 text-right">Credit (R)</th>
                                    <th class="w-12 text-right">Balance (R)</th>
                                </tr>
                            </thead>
                        </table>
                        <div class="pt-0 overflow-auto" style="max-height:200px;">
                            <table class="LoanTransactionTable customTable">
                                <tbody>
                                    <tr>
                                        <td class="w-12 text-center">01/12/2020</td>
                                        <td class="w-12 text-center">01/12/2020</td>
                                        <td class="w-40">Interest Debit</td>
                                        <td class="w-12 text-right">R1 490.00</td>
                                        <td class="w-12 text-right">-</td>
                                        <td class="w-12 text-right">R74 298.00</td>
                                    </tr>
                                    <tr>
                                        <td class="w-12 text-center">03/12/2020</td>
                                        <td class="w-12 text-center">03/12/2020</td>
                                        <td class="w-40">Insurance Premium</td>
                                        <td class="w-12 text-right">R188.00</td>
                                        <td class="w-12 text-right">-</td>
                                        <td class="w-12 text-right">R74 487.00</td>
                                    </tr>
                                    <tr>
                                        <td class="w-12 text-center">15/12/2020</td>
                                        <td class="w-12 text-center">15/12/2020</td>
                                        <td class="w-40">Payment - Thank you</td>
                                        <td class="w-12 text-right">-</td>
                                        <td class="w-12 text-right">R3 297.00</td>
                                        <td class="w-12 text-right">R71 189.00</td>
                                    </tr>
                                    <tr>
                                        <td class="w-12 text-center">31/12/2020</td>
                                        <td class="w-12 text-center">31/12/2020</td>
                                        <td class="w-40">Monthly Admin Fee</td>
                                        <td class="w-12 text-right">R69.00</td>
                                        <td class="w-12 text-right">-</td>
                                        <td class="w-12 text-right">R71 258.00</td>
                                    </tr>
                                    <tr>
                                        <td class="w-12 text-center">01/01/2021</td>
                                        <td class="w-12 text-center">01/01/2021</td>
                                        <td class="w-40">Interest Debit</td>
                                        <td class="w-12 text-right">R1 512.00</td>
                                        <td class="w-12 text-right">-</td>
                                        <td class="w-12 text-right">R72 770.00</td>
                                    </tr>
                                    <tr>
                                        <td class="w-12 text-center">05/01/2021</td>
                                        <td class="w-12 text-center">05/01/2021</td>
                                        <td class="w-40">Insurance Premium</td>
                                        <td class="w-12 text-right">R188.00</td>
                                        <td class="w-12 text-right">-</td>
                                        <td class="w-12 text-right">R72 958.00</td>
                                    </tr>
                                    <tr>
                                        <td class="w-12 text-center">15/01/2021</td>
                                        <td class="w-12 text-center">15/01/2021</td>
                                        <td class="w-40">Payment - Thank you</td>
                                        <td class="w-12 text-right">-</td>
                                        <td class="w-12 text-right">R3 297.00</td>
                                        <td class="w-12 text-right">R69 660.00</td>
                                    </tr>
                                </tbody>
                            </table>
                        </div>
                    </div>

                    <div class="py-2">
                        <div class='card-body-header pb-2'>Home loan statement overview</div>
                        <div class='row pb-1'>
                            <div class='col-lg-4 pr-0'>
                                <div class='TotalAmountDetailsDiv'>
                                    <span class="fnt-14pt">Balance outstanding</span> <br> <span class="fnt-10pt">as at 2021-01-30</span>
                                </div>
                            </div>
                            <div class='col-lg-8 pl-0 text-right'>
                                <div class='TotalAmountDetailsDiv'>
                                    <span class="fnt-14pt">R69 660.00</span>&nbsp;<br>
                                </div>
                            </div>
                        </div>

                        <div class='card-body-sub-header pb-2 pt-1'>Payment Due</div>
                        <div class="d-flex flex-row">
                            <div class="paymentDueHeaderBlock mr-1">Current</div>
                            <div class="paymentDueHeaderBlock mr-1">After 30 days</div>
                            <div class="paymentDueHeaderBlock mr-1">After 60 days</div>
                            <div class="paymentDueHeaderBlock mr-1">After 90 days</div>
                            <div class="paymentDueHeaderBlock">After 120 + days</div>
                        </div>
                        <div class="d-flex flex-row mt-1">
                            <div class="paymentDueFooterBlock mr-1">R0.00</div>
                            <div class="paymentDueFooterBlock mr-1">R0.00</div>
                            <div class="paymentDueFooterBlock mr-1">R0.00</div>
                            <div class="paymentDueFooterBlock mr-1">R0.00</div>
                            <div class="paymentDueFooterBlock">R0.00</div>
                        </div>
                    </div>

                    <div class='PaymentDueSpecialMessageDiv'>
                        If you received payment relief because of Covid-19 lockdown, any arrear status that we show above will be resolved when we restructure your account after your three-month arrangement has ended. If you have not applied for relief and would like to, phone us on 0860 555 222 or speak to your relationship banker.
                    </div>

                    <div class='card-body-sub-header pt-2'>Summary for Tax Purposes</div>
                      <div class="pt-1">
                          <table class="LoanTransactionTable customTable">
                              <tbody>
                                  <tr class="ht-20">
                                      <td class="text-left">Interest </td>
                                      <td class="text-right">R0.00</td>
                                  </tr>
                                  <tr class="ht-20">
                                      <td class="text-left">Insurance/Assurnace </td>
                                      <td class="text-right">R0.00</td>
                                  </tr>
                                  <tr class="ht-20">
                                      <td class="text-left">Service fee </td>
                                      <td class="text-right">R0.00</td>
                                  </tr>
                                  <tr class="ht-20">
                                      <td class="text-left">Legal costs </td>
                                      <td class="text-right">R0.00</td>
                                  </tr>
                                  <tr class="ht-20">
                                      <td class="text-left">Total amount received </td>
                                      <td class="text-right">R0.00</td>
                                  </tr>
                              </tbody>
                          </table>
                      </div>

                      <div class='card-body-sub-header pt-2'>Instalment details</div>
                      <div class="card-body-sub-header2">Due to insurance changes, your new instalment details are as follows:</div>
                      <div class="pt-1">
                          <table class="LoanTransactionTable customTable">
                              <thead>
                                  <tr class="ht-30">
                                      <th class="w-50">Payments</th>
                                      <th class="w-50 text-right">Amount</th>
                                  </tr>
                              </thead>
                              <tbody>
                                  <tr class="ht-20">
                                      <td class="text-left">Basic instalment </td>
                                      <td class="text-right">R0.00</td>
                                  </tr>
                                  <tr class="ht-20">
                                      <td class="text-left">Homeowner's insurance </td>
                                      <td class="text-right">R0.00</td>
                                  </tr>
                                  <tr class="ht-20">
                                      <td class="text-left">Credit life insurance </td>
                                      <td class="text-right">R0.00</td>
                                  </tr>
                                  <tr class="ht-20">
                                      <td class="text-left">Transaction fees </td>
                                      <td class="text-right">R0.00</td>
                                  </tr>
                                  <tr class="ht-20">
                                      <td class="text-left">Subsidised account capital redemption </td>
                                      <td class="text-right">R0.00</td>
                                  </tr>
                                  <tr class="ht-20">
                                      <td class="text-left">Monthly service fee </td>
                                      <td class="text-right">R0.00</td>
                                  </tr>
                                  <tr class="ht-20">
                                      <td class="text-left font-weight-bold">Total instalment (effective from 01/04/2021) </td>
                                      <td class="text-right font-weight-bold">R0.00</td>
                                  </tr>
                              </tbody>
                          </table>
                      </div>

                </div>

                <div id='HomeLoan-6001' class='tab-pane fade'>
                    <div class="HomeLoanDetailDiv">
                        <h4 class="pl-25px"><span class="NedbankHomeLoanTxt">Nedbank home loan</span></h4>
                        <table class="customTable mt-2" border="0">
                            <tbody>
                                <tr>
                                    <td class="w-25">Bond no:</td>
                                    <td class="w-25 text-right pr-4 text-success">8003876816001</td>
                                    <td class="w-25">Address:</td>
                                    <td class="w-25 text-right text-success" rowspan="3">ERF 44 THE COVES H <br />ARTBEESPOORT <br /></td>
                                </tr>
                                <tr>
                                    <td class="w-25">Instalment:</td>
                                    <td class="w-25 text-right pr-4 text-success">R11 090.00</td>
                                </tr>
                                <tr>
                                    <td class="w-25">Arrears:</td>
                                    <td class="w-25 text-right pr-4 text-success">R0.00</td>
                                </tr>
                                <tr>
                                    <td class="w-25">Interest rate:</td>
                                    <td class="w-25 text-right pr-4 text-success">7.25% pa</td>
                                    <td class="w-25">Registration date:</td>
                                    <td class="w-25 text-right pr-4 text-success">21/01/2018</td>
                                </tr>
                                <tr>
                                    <td class="w-25">Loan term:</td>
                                    <td class="w-25 text-right pr-4 text-success">273 months</td>
                                    <td class="w-25">Registered amount</td>
                                    <td class="w-25 text-right pr-4 text-success">R416 879.00</td>
                                </tr>
                            </tbody>
                        </table>
                    </div>

                    <div class="pt-1">
                        <table id="TableWidget" class="LoanTransactionTable customTable">
                            <thead>
                                <tr class="ht-30">
                                    <th class="w-12 text-center">Post date</th>
                                    <th class="w-12 text-center">Effective date</th>
                                    <th class="w-40">Transaction</th>
                                    <th class="w-12 text-right">Debit (R)</th>
                                    <th class="w-12 text-right">Credit (R)</th>
                                    <th class="w-12 text-right">Balance (R)</th>
                                </tr>
                            </thead>
                        </table>
                        <div class="pt-0 overflow-auto" style="max-height:200px;">
                            <table class="LoanTransactionTable customTable">
                                <tbody>
                                    <tr>
                                        <td class="w-12 text-center">01/12/2020</td>
                                        <td class="w-12 text-center">01/12/2020</td>
                                        <td class="w-40">Interest Debit</td>
                                        <td class="w-12 text-right">R1 490.00</td>
                                        <td class="w-12 text-right">-</td>
                                        <td class="w-12 text-right">R71 298.00</td>
                                    </tr>
                                    <tr>
                                        <td class="w-12 text-center">03/12/2020</td>
                                        <td class="w-12 text-center">03/12/2020</td>
                                        <td class="w-40">Insurance Premium</td>
                                        <td class="w-12 text-right">R188.00</td>
                                        <td class="w-12 text-right">-</td>
                                        <td class="w-12 text-right">R71 487.00</td>
                                    </tr>
                                    <tr>
                                        <td class="w-12 text-center">15/12/2020</td>
                                        <td class="w-12 text-center">15/12/2020</td>
                                        <td class="w-40">Payment - Thank you</td>
                                        <td class="w-12 text-right">-</td>
                                        <td class="w-12 text-right">R3 297.00</td>
                                        <td class="w-12 text-right">R68 189.00</td>
                                    </tr>
                                    <tr>
                                        <td class="w-12 text-center">31/12/2020</td>
                                        <td class="w-12 text-center">31/12/2020</td>
                                        <td class="w-40">Monthly Admin Fee</td>
                                        <td class="w-12 text-right">R69.00</td>
                                        <td class="w-12 text-right">-</td>
                                        <td class="w-12 text-right">R68 258.00</td>
                                    </tr>
                                    <tr>
                                        <td class="w-12 text-center">01/01/2021</td>
                                        <td class="w-12 text-center">01/01/2021</td>
                                        <td class="w-40">Interest Debit</td>
                                        <td class="w-12 text-right">R1 512.00</td>
                                        <td class="w-12 text-right">-</td>
                                        <td class="w-12 text-right">R69 770.00</td>
                                    </tr>
                                    <tr>
                                        <td class="w-12 text-center">05/01/2021</td>
                                        <td class="w-12 text-center">05/01/2021</td>
                                        <td class="w-40">Insurance Premium</td>
                                        <td class="w-12 text-right">R188.00</td>
                                        <td class="w-12 text-right">-</td>
                                        <td class="w-12 text-right">R69 958.00</td>
                                    </tr>
                                    <tr>
                                        <td class="w-12 text-center">15/01/2021</td>
                                        <td class="w-12 text-center">15/01/2021</td>
                                        <td class="w-40">Payment - Thank you</td>
                                        <td class="w-12 text-right">-</td>
                                        <td class="w-12 text-right">R3 297.00</td>
                                        <td class="w-12 text-right">R66 660.00</td>
                                    </tr>
                                </tbody>
                            </table>
                        </div>
                    </div>

                    <div class="py-2">
                        <div class='card-body-header pb-2'>Home loan statement overview</div>
                        <div class='row pb-1'>
                            <div class='col-lg-4 pr-0'>
                                <div class='TotalAmountDetailsDiv'>
                                    <span class="fnt-14pt">Balance outstanding</span> <br> <span class="fnt-10pt">as at 2021-01-30</span>
                                </div>
                            </div>
                            <div class='col-lg-8 pl-0 text-right'>
                                <div class='TotalAmountDetailsDiv'>
                                    <span class="fnt-14pt">R69 660.00</span><br>
                                </div>
                            </div>
                        </div>

                        <div class='card-body-sub-header pb-2 pt-1'>Payment Due</div>
                        <div class="d-flex flex-row">
                            <div class="paymentDueHeaderBlock mr-1">Current</div>
                            <div class="paymentDueHeaderBlock mr-1">After 30 days</div>
                            <div class="paymentDueHeaderBlock mr-1">After 60 days</div>
                            <div class="paymentDueHeaderBlock mr-1">After 90 days</div>
                            <div class="paymentDueHeaderBlock">After 120 + days</div>
                        </div>
                        <div class="d-flex flex-row mt-1">
                            <div class="paymentDueFooterBlock mr-1">R0.00</div>
                            <div class="paymentDueFooterBlock mr-1">R0.00</div>
                            <div class="paymentDueFooterBlock mr-1">R0.00</div>
                            <div class="paymentDueFooterBlock mr-1">R0.00</div>
                            <div class="paymentDueFooterBlock">R0.00</div>
                        </div>
                    </div>

                    <div class='PaymentDueSpecialMessageDiv'>
                        If you received payment relief because of Covid-19 lockdown, any arrear status that we show above will be resolved when we restructure your account after your three-month arrangement has ended. If you have not applied for relief and would like to, phone us on 0860 555 222 or speak to your relationship banker.
                    </div>

                    <div class='card-body-sub-header pt-2'>Summary for Tax Purposes</div>
                      <div class="pt-1">
                          <table class="LoanTransactionTable customTable">
                              <tbody>
                                  <tr class="ht-20">
                                      <td class="text-left">Interest </td>
                                      <td class="text-right">R0.00</td>
                                  </tr>
                                  <tr class="ht-20">
                                      <td class="text-left">Insurance/Assurnace </td>
                                      <td class="text-right">R0.00</td>
                                  </tr>
                                  <tr class="ht-20">
                                      <td class="text-left">Service fee </td>
                                      <td class="text-right">R0.00</td>
                                  </tr>
                                  <tr class="ht-20">
                                      <td class="text-left">Legal costs </td>
                                      <td class="text-right">R0.00</td>
                                  </tr>
                                  <tr class="ht-20">
                                      <td class="text-left">Total amount received </td>
                                      <td class="text-right">R0.00</td>
                                  </tr>
                              </tbody>
                          </table>
                      </div>

                      <div class='card-body-sub-header pt-2'>Instalment details</div>
                      <div class="card-body-sub-header2">Due to insurance changes, your new instalment details are as follows:</div>
                      <div class="pt-1">
                          <table class="LoanTransactionTable customTable">
                              <thead>
                                  <tr class="ht-30">
                                      <th class="w-50">Payments</th>
                                      <th class="w-50 text-right">Amount</th>
                                  </tr>
                              </thead>
                              <tbody>
                                  <tr class="ht-20">
                                      <td class="text-left">Basic instalment </td>
                                      <td class="text-right">R0.00</td>
                                  </tr>
                                  <tr class="ht-20">
                                      <td class="text-left">Homeowner's insurance </td>
                                      <td class="text-right">R0.00</td>
                                  </tr>
                                  <tr class="ht-20">
                                      <td class="text-left">Credit life insurance </td>
                                      <td class="text-right">R0.00</td>
                                  </tr>
                                  <tr class="ht-20">
                                      <td class="text-left">Transaction fees </td>
                                      <td class="text-right">R0.00</td>
                                  </tr>
                                  <tr class="ht-20">
                                      <td class="text-left">Subsidised account capital redemption </td>
                                      <td class="text-right">R0.00</td>
                                  </tr>
                                  <tr class="ht-20">
                                      <td class="text-left">Monthly service fee </td>
                                      <td class="text-right">R0.00</td>
                                  </tr>
                                  <tr class="ht-20">
                                      <td class="text-left font-weight-bold">Total instalment (effective from 01/04/2021) </td>
                                      <td class="text-right font-weight-bold">R0.00</td>
                                  </tr>
                              </tbody>
                          </table>
                      </div>
                </div>
            </div>
        </div>
    </div>
    </div>
  </div>`
})
export class HomeLoanAccountsBreakdownComponent {
  @Input()
  widgetsGridsterItemArray: any[] = [];
}

// Component Created for Home loan payment due special msg Widget -- Nedbank
@Component({
  selector: 'HomeLoanPaymentDueSpecialMsg',
  template: `<div class="widget">
    <div class="widget-area height100">
     <div class="card border-0 pt-2">
          <div class="card-body text-left py-1">
              <div class="PaymentDueSpecialMessageDiv pt-2">
                  If you received payment relief because of Covid-19, any arrear status that we show above will be resolved when we restructure your account after your three-month arrangement has ended. If you have not applied for relief and would like to, phone us on 0860 555 222 or speak to your relationship banker.
              </div>
          </div>
      </div>
    </div>
  </div>`
})
export class HomeLoanPaymentDueSpecialMsgComponent {
  @Input()
  widgetsGridsterItemArray: any[] = [];
}

// Component Created for Home loan instalment details Widget -- Nedbank
@Component({
  selector: 'HomeLoanInstalmentDetail',
  template: `<div class="widget">
    <div class="widget-area height100">
      <div class='card border-0 pt-4'>
          <div class='card-body text-left py-1'>
              <div class='card-body-sub-header pt-1'>Instalment details</div>
              <div class="card-body-sub-header2">Due to insurance changes, your new instalment details are as follows:</div>
              <div class="pt-1">
                  <table class="LoanTransactionTable customTable">
                      <thead>
                          <tr class="ht-30">
                              <th class="w-50">Payments</th>
                              <th class="w-50 text-right">Amount</th>
                          </tr>
                      </thead>
                      <tbody>
                          <tr class="ht-20">
                              <td class="text-left">Basic instalment </td>
                              <td class="text-right">R0.00</td>
                          </tr>
                          <tr class="ht-20">
                              <td class="text-left">Homeowner's insurance </td>
                              <td class="text-right">R0.00</td>
                          </tr>
                          <tr class="ht-20">
                              <td class="text-left">Credit life insurance </td>
                              <td class="text-right">R0.00</td>
                          </tr>
                          <tr class="ht-20">
                              <td class="text-left">Transaction fees </td>
                              <td class="text-right">R0.00</td>
                          </tr>
                          <tr class="ht-20">
                              <td class="text-left">Subsidised account capital redemption </td>
                              <td class="text-right">R0.00</td>
                          </tr>
                          <tr class="ht-20">
                              <td class="text-left">Monthly service fee </td>
                              <td class="text-right">R0.00</td>
                          </tr>
                          <tr class="ht-20">
                              <td class="text-left font-weight-bold">Total instalment (effective from 01/02/2021) </td>
                              <td class="text-right font-weight-bold">R0.00</td>
                          </tr>
                      </tbody>
                  </table>
              </div>
          </div>
      </div>
    </div>
  </div>`
})
export class HomeLoanInstalmentDetailComponent {
  @Input()
  widgetsGridsterItemArray: any[] = [];
}

// Component Created for Portfolio product level Customer Details Widget -- Nedbank
@Component({
  selector: 'PortfolioCustomerDetails',
  template: `<div class="widget">
    <div class="widget-area height100">
       <div class='card border-0'>
          <div class="card-body CustomerDetails">
              <span class="fnt-14pt pt-1">MR. Firstname Lastname</span> <br />
              <p class="fnt-8pt pt-1">Customer Id: 171001255307</p>
              <p class="fnt-8pt">Mobile No: +2367 345 786</p>
              <p>Email: custemail@demo.com</p>
          </div>
      </div>
    </div>
  </div>`
})
export class PortfolioCustomerDetailsComponent {
  @Input()
  widgetsGridsterItemArray: any[] = [];
}

// Component Created for Portfolio product level Customer address details Widget -- Nedbank
@Component({
  selector: 'PortfolioCustomerAddressDetails',
  template: `<div class="widget">
    <div class="widget-area height100">
       <div class='card border-0'>
          <div class="card-body CustomerDetails">
              <span class="fnt-14pt">Address</span> <br />
              VERDEAU LIFESTYLE ESTATE <br />
              6 HERCULE CRESCENT DRIVE <br />
              WELLINGTON, 7655
          </div>
      </div>
    </div>
  </div>`
})
export class PortfolioCustomerAddressDetailsComponent {
  @Input()
  widgetsGridsterItemArray: any[] = [];
}

// Component Created for Portfolio product level client contact details Widget -- Nedbank
@Component({
  selector: 'PortfolioClientContactDetails',
  template: `<div class="widget">
    <div class="widget-area height100">
       <div class='card border-0'>
          <div class="card-body ClientDetails">
              <span class="fnt-14pt">Client contact details</span><br />
              <p>Mobile No: 0860 555 111</p>
              <p>Email: supportdesk@nedbank.com</p>
          </div>
      </div>
    </div>
  </div>`
})
export class PortfolioClientContactDetailsComponent {
  @Input()
  widgetsGridsterItemArray: any[] = [];
}

// Component Created for Portfolio product level account summary details Widget -- Nedbank
@Component({
  selector: 'PortfolioAccountSummaryDetails',
  template: `<div class="widget">
    <div class="widget-area height100">
       <div class='card border-0'>
          <div class="card-body">
              <div class="card-body-header pb-2">Account Summary</div>
              <div class="pt-1">
                  <table class="LoanTransactionTable customTable">
                      <thead>
                          <tr class="ht-30">
                              <th class="w-50">Account</th>
                              <th class="w-50 text-right">Total Amount</th>
                          </tr>
                      </thead>
                      <tbody>
                          <tr class="ht-30">
                              <td class="text-left">Investment </td>
                              <td class="text-right">R9 620.98</td>
                          </tr>
                          <tr class="ht-30">
                              <td class="text-left">Personal Loan </td>
                              <td class="text-right">R4 165.00</td>
                          </tr>
                          <tr class="ht-30">
                              <td class="text-left">Home Loan </td>
                              <td class="text-right">R7 969.00</td>
                          </tr>
                      </tbody>
                  </table>
              </div>
              <div class="pt-2">
                  <table class="LoanTransactionTable customTable">
                      <thead>
                          <tr class="ht-30">
                              <th class="text-left">Greenbacks rewards points </th>
                              <th class="text-right">234</th>
                          </tr>
                      </thead>
                  </table>
              </div>
          </div>
      </div>
    </div>
  </div>`
})
export class PortfolioAccountSummaryDetailsComponent {
  @Input()
  widgetsGridsterItemArray: any[] = [];
}

// Component Created for Portfolio product level account analysis Widget -- Nedbank
@Component({
  selector: 'PortfolioAccountAnalysis',
  template: `<div class="widget">
    <div class="widget-area width100">
      <div class="card border-0">
          <div class="card-body">
              <div class="card-body-header pb-2 text-center pb-1">Account analytics</div>
              <div id="AccountAnalysisBarGraphcontainer" style="height:200px;"></div>
          </div>
      </div>      
    </div>
</div>`
})
export class PortfolioAccountAnalysisComponent {
  @Input()
  widgetsGridsterItemArray: any[] = [];
  public options4: any = {
    chart: {
      backgroundColor: 'rgba(0,0,0,0)',
      type: 'column',
      style: {
        fontFamily: 'Mark Pro Regular',
        fontSize: '8pt'
      }
    },
    title: {
      text: ''
    },
    xAxis: {
      categories: ['Jan', 'Feb'],
      crosshair: true
    },
    yAxis: {
      title: {
        text: 'Amount (R)'
      }
    },
    credits: {
      enabled: false
    },
    tooltip: {
      headerFormat: '<span>{point.key}</span><table>',
      pointFormat: '<tr><td style="color:{series.color};padding:0">{series.name}: </td> <td style="padding:0"><b>{point.y:.2f}</b></td></tr>',
      footerFormat: '</table>',
      shared: true,
      useHTML: true
    },
    plotOptions: {
      column: {
        pointPadding: 0.2,
        borderWidth: 0
      }
    },
    series: [{
      name: 'Investment',
      data: [9456.12, 9620.98]
    }, {
      name: 'Personal loan',
      data: [-4465.00, -4165.00]
    }, {
      name: 'Home loan',
      data: [-8969.00, -7969.00]
    }],
    //colors: ['#005b00', '#434348', '#7cb5ec']
  }
  ngAfterViewInit() {
    setTimeout(() => {
      Highcharts.chart('AccountAnalysisBarGraphcontainer', this.options4);
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

// Component Created for Portfolio product level reminders and recommendation Widget -- Nedbank
@Component({
  selector: 'PortfolioReminders',
  template: `<div class="widget">
    <div class="widget-area height100">
       <div class='card border-0'>
          <div class='card-body ReminderDiv'>
              <div class="card-body-header pb-2 fnt-14pt">Reminder and Recommendation</div>
              <div class='row'>
                  <div class='col-lg-9 text-left'><p class='p-1' style='background-color: #dce3dc;'>Your Credit Card payment is due </p></div>
                  <div class='col-lg-3 text-left'>
                      <a href='javascript:void(0)' target='_blank'>
                          <i class='fa fa-caret-left fa-3x float-left text-success'></i><span class='mt-2 d-inline-block ml-2'>Pay Now</span>
                      </a>
                  </div>
              </div>
              <div class='row'>
                  <div class='col-lg-9 text-left'><p class='p-1' style='background-color: #dce3dc;'>Update missing information in your profile </p></div>
                  <div class='col-lg-3 text-left'>
                      <a href='javascript:void(0)' target='_blank'>
                          <i class='fa fa-caret-left fa-3x float-left text-success'></i><span class='mt-2 d-inline-block ml-2'>Update</span>
                      </a>
                  </div>
              </div>
              <div class='row'>
                  <div class='col-lg-9 text-left'>
                      <p class='p-1' style='background-color: #dce3dc;'>Based on your transaction trend add on BA Airmile card is offered free to you. </p>
                  </div>
                  <div class='col-lg-3 text-left'>
                      <a href='javascript:void(0)' target='_blank'>
                          <i class='fa fa-caret-left fa-3x float-left text-success'></i><span class='mt-2 d-inline-block ml-2'>Order</span>
                      </a>
                  </div>
              </div>
              <div class='row'>
                  <div class='col-lg-9 text-left'><p class='p-1' style='background-color: #dce3dc;'>Need saving deposit Boxes </p></div>
                  <div class='col-lg-3 text-left'>
                      <a href='javascript:void(0)' target='_blank'>
                          <i class='fa fa-caret-left fa-3x float-left text-success'></i><span class='mt-2 d-inline-block ml-2'>Apply</span>
                      </a>
                  </div>
              </div>
          </div>
      </div>
    </div>
  </div>`
})
export class PortfolioRemindersComponent {
  @Input()
  widgetsGridsterItemArray: any[] = [];
}

// Component Created for Portfolio product level news alerts Widget -- Nedbank
@Component({
  selector: 'PortfolioNewsAlerts',
  template: `<div class="widget">
    <div class="widget-area height100">
       <div class='card border-0'>
          <div class='card-body text-left py-1'>
            <div class='NewAlertDiv'>
                <div class='card-body-header pb-2'>News Alerts</div>
                <p>Covid 19 and the subsequent lockdown has affected all areas of our daily lives. The way we work, the way we bank and how we interact with each other.</p>
                <p>We want you to know we are in this together. That's why we are sharing advice, tips and news updates with you on ways to bank as well as ways to keep yorself and your loved ones safe.</p>
                <p>We would like to remind you of the credit life insurance benefits available to you through your Nedbank Insurance policy. When you pass away, Nedbank Insurance will cover your outstanding loan amount. If you are permanently employed, you will also enjoy cover for comprehensive disability and loss of income. The disability benefit will cover your monthly instalments if you cannot earn your usual income due to illness or bodily injury.</p>
                <div class='float-right'>
                    <a href='javascript:void(0)' target='_blank'>
                        <i class='fa fa-caret-left fa-3x float-left text-success'></i><span class='mt-2 d-inline-block ml-2'>More Info</span>
                    </a>
                </div>
            </div>
        </div>
      </div>
    </div>
  </div>`
})
export class PortfolioNewsAlertsComponent {
  @Input()
  widgetsGridsterItemArray: any[] = [];
}

// Component Created for Greenbacks product level total reward points Widget -- Nedbank
@Component({
  selector: 'GreenbacksTotalRewardPoints',
  template: `<div class="widget">
    <div class="widget-area height100">
       <div class='card border-0'>
          <div class="card-body text-center">
              <span class="TotalRewardPointsSpan" title='Total Rewards Points'>482</span>
          </div>
      </div>
    </div>
  </div>`
})
export class GreenbacksTotalRewardPointsComponent {
  @Input()
  widgetsGridsterItemArray: any[] = [];
}

// Component Created for Rewards product level greenbacks contact us Widget -- Nedbank
@Component({
  selector: 'GreenbacksContactUs',
  template: `<div class="widget">
    <div class="widget-area height100">
       <div class="card border-0">
          <div class="card-body GreenbacksContactUsDiv">
              <ul class="list-group float-right" style="width:250px;">
                  <li class="list-group-item d-flex justify-content-between align-items-center ht-30 JoinGreenbacks-li-bg border-white">
                      <a class="text-white" href="javascript:void(0)">
                          Join Greenbacks
                          <i class='fa fa-caret-right float-right text-white mt-1'></i>
                      </a>
                  </li>
                  <li class="list-group-item d-flex justify-content-between align-items-center ht-30 UseGreenbacks-li-bg border-white">
                      <a class="text-white" href="javascript:void(0)">
                          Use Greenbacks
                          <i class='fa fa-caret-right float-right text-white mt-1'></i>
                      </a>
                  </li>
                  <li class="list-group-item d-flex justify-content-between align-items-center ht-30 ContactUs-li-bg border-white">
                      Contact Us 0860 553 111
                  </li>
              </ul>
          </div>
      </div>
    </div>
  </div>`
})
export class GreenbacksContactUsComponent {
  @Input()
  widgetsGridsterItemArray: any[] = [];
}

// Component Created for Rewards product level YTD rewards points Widget -- Nedbank
@Component({
  selector: 'YTDRewardPoints',
  template: `<div class="widget">
    <div class="widget-area width100">
      <div class="card border-0">
          <div class="card-body">
              <div class="card-body-header pb-2 text-center pb-1">YTD Rewards Points</div>
              <div id="YTDRewardPointsBarGraphcontainer" style="height:200px;"></div>
          </div>
      </div>      
    </div>
</div>`
})
export class YTDRewardPointsComponent {
  @Input()
  widgetsGridsterItemArray: any[] = [];

  public options4: any = {
    chart: {
      backgroundColor: 'rgba(0,0,0,0)',
      type: 'column',
      style: {
        fontFamily: 'Mark Pro Regular',
        fontSize: '8pt'
      }
    },
    title: {
      text: ''
    },
    xAxis: {
      categories: ['Jan', 'Feb', 'Mar', 'Apr'],
      crosshair: true
    },
    yAxis: {
      title: {
        text: 'Points'
      }
    },
    credits: {
      enabled: false
    },
    tooltip: {
      headerFormat: '<span>{point.key}</span><table>',
      pointFormat: '<tr><td style="color:{series.color};padding:0">{series.name}: </td> <td style="padding:0"><b>{point.y:.2f}</b></td></tr>',
      footerFormat: '</table>',
      shared: true,
      useHTML: true
    },
    plotOptions: {
      column: {
        pointPadding: 0.2,
        borderWidth: 0
      },
      series: {
        color: '#005b00'
      }
    },
    series: [{
      name: 'Rewards Points',
      data: [98, 112, 128, 144]
    }]
  }

  ngAfterViewInit() {
    setTimeout(() => {
      Highcharts.chart('YTDRewardPointsBarGraphcontainer', this.options4);
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

// Component Created for Rewards product level Points redeemed YTD Widget -- Nedbank
@Component({
  selector: 'PointsRedeemedYTD',
  template: `<div class="widget">
    <div class="widget-area width100">
      <div class="card border-0">
          <div class="card-body">
              <div class="card-body-header pb-2 text-center pb-1">Points Redeemed YTD</div>
              <div id="PointsRedeemedYTDBarGraphcontainer" style="height:200px;"></div>
          </div>
      </div>      
    </div>
</div>`
})
export class PointsRedeemedYTDComponent {
  @Input()
  widgetsGridsterItemArray: any[] = [];

  public options4: any = {
    chart: {
      backgroundColor: 'rgba(0,0,0,0)',
      type: 'column',
      style: {
        fontFamily: 'Mark Pro Regular',
        fontSize: '8pt'
      }
    },
    title: {
      text: ''
    },
    xAxis: {
      categories: ['Jan', 'Feb', 'Mar', 'Apr'],
      crosshair: true
    },
    yAxis: {
      title: {
        text: 'Points'
      }
    },
    credits: {
      enabled: false
    },
    tooltip: {
      headerFormat: '<span>{point.key}</span><table>',
      pointFormat: '<tr><td style="color:{series.color};padding:0">{series.name}: </td> <td style="padding:0"><b>{point.y:.2f}</b></td></tr>',
      footerFormat: '</table>',
      shared: true,
      useHTML: true
    },
    plotOptions: {
      column: {
        pointPadding: 0.2,
        borderWidth: 0
      },
      series: {
        color: '#005b00'
      }
    },
    series: [{
      name: 'Redeemed Points',
      data: [96, 71, 87, 54]
    }]
  }

  ngAfterViewInit() {
    setTimeout(() => {
      Highcharts.chart('PointsRedeemedYTDBarGraphcontainer', this.options4);
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

// Component Created for Rewards product level Product Related Points Earned Widget -- Nedbank
@Component({
  selector: 'ProductRelatedPointsEarned',
  template: `<div class="widget">
    <div class="widget-area width100">
      <div class="card border-0">
          <div class="card-body">
              <div class="card-body-header pb-2 text-center pb-1">Product Related Points Earned</div>
              <div id="ProductRelatedPointsEarnedBarGraphcontainer" style="height:200px;"></div>
          </div>
      </div>      
    </div>
</div>`
})
export class ProductRelatedPointsEarnedComponent {
  @Input()
  widgetsGridsterItemArray: any[] = [];

  public options4: any = {
    chart: {
      backgroundColor: 'rgba(0,0,0,0)',
      type: 'column',
      style: {
        fontFamily: 'Mark Pro Regular',
        fontSize: '8pt'
      }
    },
    title: {
      text: ''
    },
    xAxis: {
      categories: ['Jan', 'Feb', 'Mar', 'Apr'],
      crosshair: true
    },
    yAxis: {
      title: {
        text: 'Points'
      }
    },
    credits: {
      enabled: false
    },
    tooltip: {
      headerFormat: '<span>{point.key}</span><table>',
      pointFormat: '<tr><td style="color:{series.color};padding:0">{series.name}: </td> <td style="padding:0"><b>{point.y:.2f}</b></td></tr>',
      footerFormat: '</table>',
      shared: true,
      useHTML: true
    },
    plotOptions: {
      column: {
        pointPadding: 0.2,
        borderWidth: 0
      }
    },
    series: [{
      name: 'Investment',
      data: [121, 145, 98, 134]
    }, {
      name: 'Personal loan',
      data: [232, 167, 125, 245]
    }, {
      name: 'Home loan',
      data: [104, 110, 211, 167]
    }]
  }

  ngAfterViewInit() {
    setTimeout(() => {
      Highcharts.chart('ProductRelatedPointsEarnedBarGraphcontainer', this.options4);
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

// Component Created for Rewards product level Category Spend Rewards Widget -- Nedbank
@Component({
  selector: 'CategorySpendRewards',
  template: `<div class="widget">
    <div class="widget-area width100">
      <div class="card border-0">
          <div class="card-body">
              <div class="card-body-header pb-2 text-center pb-1">Category Spend Rewards</div>
              <div id="CategorySpendRewardsPieChartcontainer" style="height:200px;"></div>
          </div>
      </div>      
    </div>
</div>`
})
export class CategorySpendRewardsComponent {
  @Input()
  widgetsGridsterItemArray: any[] = [];

  public options4: any = {
    chart: {
      plotBackgroundColor: null,
      plotBorderWidth: null,
      plotShadow: !1,
      type: 'pie',
      style: {
        fontFamily: 'Mark Pro Regular',
        fontSize: '8pt'
      }
    },
    title: { text: '' },
    credits: { enabled: false },
    tooltip: { pointFormat: '{series.name}: <b>{point.percentage:.1f}%</b>' },
    accessibility: { point: { valueSuffix: '%' } },
    plotOptions: {
      pie: {
        allowPointSelect: !0,
        cursor: 'pointer',
        dataLabels: {
          enabled: !0,
          format: '{point.percentage:.1f} %'
        },
        showInLegend: !0
      }
    },
    series: [{
      name: 'Percentage',
      colorByPoint: !0,
      data: [{ name: 'Fuel', y: 34 }, { name: 'Groceries', y: 15 }, { name: 'Travel', y: 21 }, { name: 'Movies', y: 19 }, { name: 'Shopping', y: 11 }]
    }]
  }

  ngAfterViewInit() {
    setTimeout(() => {
      Highcharts.chart('CategorySpendRewardsPieChartcontainer', this.options4);
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

/* ---  below highcharts components are created for page preview while designing..
    which are same as above highcharts widgets, but created due to facing issue in page preview in popup --- */

// Component Created for Portfolio product level account analysis Widget for page preview while designing -- Nedbank
@Component({
  selector: 'PortfolioAccountAnalysisPreview',
  template: `<div class="widget">
    <div class="widget-area width100">
      <div class="card border-0">
          <div class="card-body">
              <div class="card-body-header pb-2 text-center pb-1">Account analytics</div>
              <div id="AccountAnalysisPreviewBarGraphcontainer" style="height:200px;"></div>
          </div>
      </div>      
    </div>
</div>`
})
export class PortfolioAccountAnalysisPreviewComponent {
  @Input()
  widgetsGridsterItemArray: any[] = [];
  public options4: any = {
    chart: {
      height: (9 / 16 * 100) + '%',
      backgroundColor: 'rgba(0,0,0,0)',
      type: 'column',
      style: {
        fontFamily: 'Mark Pro Regular',
        fontSize: '8pt'
      }
    },
    title: {
      text: ''
    },
    xAxis: {
      categories: ['Jan', 'Feb'],
      crosshair: true
    },
    yAxis: {
      title: {
        text: 'Amount (R)'
      }
    },
    credits: {
      enabled: false
    },
    tooltip: {
      headerFormat: '<span>{point.key}</span><table>',
      pointFormat: '<tr><td style="color:{series.color};padding:0">{series.name}: </td> <td style="padding:0"><b>{point.y:.2f}</b></td></tr>',
      footerFormat: '</table>',
      shared: true,
      useHTML: true
    },
    plotOptions: {
      column: {
        pointPadding: 0.2,
        borderWidth: 0
      }
    },
    series: [{
      name: 'Investment',
      data: [9456.12, 9620.98]
    }, {
      name: 'Personal loan',
      data: [-4465.00, -4165.00]
    }, {
      name: 'Home loan',
      data: [-8969.00, -7969.00]
    }],
    //colors: ['#005b00', '#434348', '#7cb5ec']
  }
  ngAfterViewInit() {
    setTimeout(() => {
      Highcharts.chart('AccountAnalysisPreviewBarGraphcontainer', this.options4);
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

// Component Created for Rewards product level YTD rewards points Widget for page preview while designing -- Nedbank
@Component({
  selector: 'YTDRewardPointsPreview',
  template: `<div class="widget">
    <div class="widget-area width100">
      <div class="card border-0">
          <div class="card-body">
              <div class="card-body-header pb-2 text-center pb-1">YTD Rewards points</div>
              <div id="YTDRewardPointsBarGraphPreviewcontainer" style="height:200px;"></div>
          </div>
      </div>      
    </div>
</div>`
})
export class YTDRewardPointsPreviewComponent {
  @Input()
  widgetsGridsterItemArray: any[] = [];

  public options4: any = {
    chart: {
      backgroundColor: 'rgba(0,0,0,0)',
      type: 'column',
      style: {
        fontFamily: 'Mark Pro Regular',
        fontSize: '8pt'
      }
    },
    title: {
      text: ''
    },
    xAxis: {
      categories: ['Jan', 'Feb', 'Mar', 'Apr'],
      crosshair: true
    },
    yAxis: {
      title: {
        text: 'Points'
      }
    },
    credits: {
      enabled: false
    },
    tooltip: {
      headerFormat: '<span>{point.key}</span><table>',
      pointFormat: '<tr><td style="color:{series.color};padding:0">{series.name}: </td> <td style="padding:0"><b>{point.y:.2f}</b></td></tr>',
      footerFormat: '</table>',
      shared: true,
      useHTML: true
    },
    plotOptions: {
      column: {
        pointPadding: 0.2,
        borderWidth: 0
      },
      series: {
        color: '#005b00'
      }
    },
    series: [{
      name: 'Rewards Points',
      data: [98, 112, 128, 144]
    }]
  }

  ngAfterViewInit() {
    setTimeout(() => {
      Highcharts.chart('YTDRewardPointsBarGraphPreviewcontainer', this.options4);
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

// Component Created for Rewards product level Points redeemed YTD Widget for page preview while designing -- Nedbank
@Component({
  selector: 'PointsRedeemedYTDPreview',
  template: `<div class="widget">
    <div class="widget-area width100">
      <div class="card border-0">
          <div class="card-body">
              <div class="card-body-header pb-2 text-center pb-1">Points Redeemed YTD</div>
              <div id="PointsRedeemedYTDBarGraphPreviewcontainer" style="height:200px;"></div>
          </div>
      </div>      
    </div>
</div>`
})
export class PointsRedeemedYTDPreviewComponent {
  @Input()
  widgetsGridsterItemArray: any[] = [];

  public options4: any = {
    chart: {
      backgroundColor: 'rgba(0,0,0,0)',
      type: 'column',
      style: {
        fontFamily: 'Mark Pro Regular',
        fontSize: '8pt'
      }
    },
    title: {
      text: ''
    },
    xAxis: {
      categories: ['Jan', 'Feb', 'Mar', 'Apr'],
      crosshair: true
    },
    yAxis: {
      title: {
        text: 'Points'
      }
    },
    credits: {
      enabled: false
    },
    tooltip: {
      headerFormat: '<span>{point.key}</span><table>',
      pointFormat: '<tr><td style="color:{series.color};padding:0">{series.name}: </td> <td style="padding:0"><b>{point.y:.2f}</b></td></tr>',
      footerFormat: '</table>',
      shared: true,
      useHTML: true
    },
    plotOptions: {
      column: {
        pointPadding: 0.2,
        borderWidth: 0
      },
      series: {
        color: '#005b00'
      }
    },
    series: [{
      name: 'Redeemed Points',
      data: [96, 71, 87, 54]
    }]
  }

  ngAfterViewInit() {
    setTimeout(() => {
      Highcharts.chart('PointsRedeemedYTDBarGraphPreviewcontainer', this.options4);
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

// Component Created for Rewards product level Product Related Points Earned Widget for page preview while designing -- Nedbank
@Component({
  selector: 'ProductRelatedPointsEarnedPreview',
  template: `<div class="widget">
    <div class="widget-area width100">
      <div class="card border-0">
          <div class="card-body">
              <div class="card-body-header pb-2 text-center pb-1">Product Related Points Earned</div>
              <div id="ProductRelatedPointsEarnedBarGraphPreviewcontainer" style="height:200px;"></div>
          </div>
      </div>      
    </div>
</div>`
})
export class ProductRelatedPointsEarnedPreviewComponent {
  @Input()
  widgetsGridsterItemArray: any[] = [];

  public options4: any = {
    chart: {
      backgroundColor: 'rgba(0,0,0,0)',
      type: 'column',
      style: {
        fontFamily: 'Mark Pro Regular',
        fontSize: '8pt'
      }
    },
    title: {
      text: ''
    },
    xAxis: {
      categories: ['Jan', 'Feb', 'Mar', 'Apr'],
      crosshair: true
    },
    yAxis: {
      title: {
        text: 'Points'
      }
    },
    credits: {
      enabled: false
    },
    tooltip: {
      headerFormat: '<span>{point.key}</span><table>',
      pointFormat: '<tr><td style="color:{series.color};padding:0">{series.name}: </td> <td style="padding:0"><b>{point.y:.2f}</b></td></tr>',
      footerFormat: '</table>',
      shared: true,
      useHTML: true
    },
    plotOptions: {
      column: {
        pointPadding: 0.2,
        borderWidth: 0
      }
    },
    series: [{
      name: 'Investment',
      data: [121, 145, 98, 134]
    }, {
      name: 'Personal loan',
      data: [232, 167, 125, 245]
    }, {
      name: 'Home loan',
      data: [104, 110, 211, 167]
    }]
  }

  ngAfterViewInit() {
    setTimeout(() => {
      Highcharts.chart('ProductRelatedPointsEarnedBarGraphPreviewcontainer', this.options4);
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

// Component Created for Rewards product level Category Spend Rewards Widget for page preview while designing -- Nedbank
@Component({
  selector: 'CategorySpendRewardsPreview',
  template: `<div class="widget">
    <div class="widget-area width100">
      <div class="card border-0">
          <div class="card-body">
              <div class="card-body-header pb-2 text-center pb-1">Category Spend Rewards</div>
              <div id="CategorySpendRewardsPieChartPreviewcontainer" style="height:200px;"></div>
          </div>
      </div>      
    </div>
</div>`
})
export class CategorySpendRewardsPreviewComponent {
  @Input()
  widgetsGridsterItemArray: any[] = [];

  public options4: any = {
    chart: {
      plotBackgroundColor: null,
      plotBorderWidth: null,
      plotShadow: !1,
      type: 'pie',
      style: {
        fontFamily: 'Mark Pro Regular',
        fontSize: '8pt'
      }
    },
    title: { text: '' },
    credits: { enabled: false },
    tooltip: { pointFormat: '{series.name}: <b>{point.percentage:.1f}%</b>' },
    accessibility: { point: { valueSuffix: '%' } },
    plotOptions: {
      pie: {
        allowPointSelect: !0,
        cursor: 'pointer',
        dataLabels: {
          enabled: !0,
          format: '{point.percentage:.1f} %'
        },
        showInLegend: !0
      }
    },
    series: [{
      name: 'Percentage',
      colorByPoint: !0,
      data: [{ name: 'Fuel', y: 34 }, { name: 'Groceries', y: 15 }, { name: 'Travel', y: 21 }, { name: 'Movies', y: 19 }, { name: 'Shopping', y: 11 }]
    }]
  }

  ngAfterViewInit() {
    setTimeout(() => {
      Highcharts.chart('CategorySpendRewardsPieChartPreviewcontainer', this.options4);
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

// Component Created for special message Widget -- Nedbank
@Component({
  selector: 'HomeLoanSummaryTaxPurpose',
  template: `<div class="widget">
    <div class="widget-area height100">
      <div class="card border-0">
          <div class="card-body text-left pb-1 pt-1">
              <div class="SpecialMessageDiv">
                  <div class="SpecialMessageHeader">Special message header Text</div>
                  <p>Personalize special message text 1</p>
                  <p>Personalize special message text 2</p>
              </div>
          </div>
      </div>
    </div>
  </div>`
})
export class HomeLoanSummaryTaxPurposeComponent {
  @Input()
  widgetsGridsterItemArray: any[] = [];
}
