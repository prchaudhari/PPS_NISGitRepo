import { Component, ViewChild, Output, Input, EventEmitter } from '@angular/core';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
// import { EventEmitter } from 'events';
import { ConfigConstants } from '../../shared/constants/configConstants';

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
export class CustomerInformationComponent  {
    @Input()
    widgetsGridsterItemArray:any[] = [];

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
export class AccountInformationComponent  {
    @Input()
    widgetsGridsterItemArray:any[] = [];
    //account info
    accountInfoLists: any[] = [
        { title: 'Statement Date', value: '1-APR-2020'},
        { title: 'Statement Period', value: 'Annual Statement'},
        { title: 'Customer ID', value: 'ID2-8989-5656'},
        { title: 'RM Name', value: 'David Miller'},
        { title: 'RM Contact Number', value: '+4487867833'},
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
        <img [src]="ImageSrc" class="img-fluid" style='height:220px;'/>
      </div>
    </div>
  </div>`
})
export class ImageComponent  {

    @Input() imgItem:any;

    public ImageSrc: any;
    public baseURL = ConfigConstants.BaseURL;
    
    ngOnInit() {
      if(this.imgItem != null && this.imgItem.WidgetSetting != null && this.imgItem.WidgetSetting != '' && this.testJSON(this.imgItem.WidgetSetting)) {
        let widgetSetting = JSON.parse(this.imgItem.WidgetSetting);
        if( !widgetSetting.isPersonalize && widgetSetting.AssetLibraryId != 0 && widgetSetting.AssetName != '') {
          this.ImageSrc = this.baseURL + "assets/" + widgetSetting.AssetLibraryId + "/" + widgetSetting.AssetName;
        }else {
          this.ImageSrc = 'assets/images/icon-image.png';
        }
      }else {
        this.ImageSrc = 'assets/images/icon-image.png';
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
}

// Component Created for Video Widget--
@Component({
    selector: 'vidyo-widget',
    template: `<div class="widget">
    <div class="widget-header">
      <span class="widget-header-title">Video </span>
    </div>
    <div class="widget-area position-relative width100">
      <div class="widget-indicator-inner text-center">
        <video class="doc-video" controls style='height:calc(100% - 40px);width:75%;margin-right:15%;'>
            <source [src]="videoSrc" type="video/mp4">
          </video>
      </div>
    </div>
  </div>`
})
export class VideoComponent  {

  @Input() vdoItem:any;

  public videoSrc: any;
  public baseURL = ConfigConstants.BaseURL;
    
  ngOnInit() {
      if(this.vdoItem != null && this.vdoItem.WidgetSetting != null && this.vdoItem.WidgetSetting != '' && this.testJSON(this.vdoItem.WidgetSetting)) {
        let widgetSetting = JSON.parse(this.vdoItem.WidgetSetting);
        if(!widgetSetting.isPersonalize && widgetSetting.AssetLibraryId != 0 && widgetSetting.AssetName != '') {
          this.videoSrc = this.baseURL + "assets/" + widgetSetting.AssetLibraryId + "/" + widgetSetting.AssetName;
        }else {
          this.videoSrc = 'assets/images/SampleVideo.mp4';
        }
      }else {
        this.videoSrc = 'assets/images/SampleVideo.mp4';
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
export class SummaryAtGlanceComponent  {
    @Input()
    widgetsGridsterItemArray:any[] = [];
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

// Component Created for Transaction details Widget--
@Component({
  selector: 'transactionDetails',
  template: `<div class="widget">
  <div class="widget-header">
    <span class="widget-header-title"> Transaction Details </span>
  </div>
  <div class="widget-area-grid padding-0">
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
export class TransactionDetailsComponent  {
  @Input()
  widgetsGridsterItemArray:any[] = [];
  displayedColumns: string[] = ['date', 'type', 'narration', 'fcy', 'currentRate', 'lyc', 'action'];
  dataSource = new MatTableDataSource<any>();
  public pageSize = 5;
  public currentPage = 0;
  public totalSize = 0;

  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;

  TransactionDetailData: TransactionDetail[] = [
    { date: 'D3/D4/15', type: 'CR', narration: 'NXT TXN: IIFL IIFL6574562', fcy: '1666.67', currentRate: '1.062', lyc: '1771.42' },
    { date: 'D3/D4/19', type: 'CR', narration: 'NXT TXN: IIFL IIFL3557346', fcy: '1254.71', currentRate: '1.123', lyc: '1876.00' },
    { date: 'D3/D4/25', type: 'CR', narration: 'NXT TXN: IIFL IIFL8965435', fcy: '2345.12', currentRate: '1.461', lyc: '1453.21' },
    { date: 'D3/D4/34', type: 'CR', narration: 'NXT TXN: IIFL IIFL0034212', fcy: '1435.89', currentRate: '0.962', lyc: '1654.56' },
  ];

  ngAfterViewInit() {
      this.dataSource.paginator = this.paginator;
  }

  ngOnInit() {
      this.dataSource = new MatTableDataSource(this.TransactionDetailData);
      this.dataSource.sort = this.sort;
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