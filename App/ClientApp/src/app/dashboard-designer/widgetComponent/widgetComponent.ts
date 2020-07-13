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

  removeItem(item) {
    this.widgetsGridsterItemArray.splice(this.widgetsGridsterItemArray.indexOf(item), 1);
  }

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

  removeItem(item) {
    this.widgetsGridsterItemArray.splice(this.widgetsGridsterItemArray.indexOf(item), 1);
  }

}

// Component Created for Image Widget--
@Component({
    selector: 'image',
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

    @Input() item:any;
    @Input('widgetsGridsterItemArray') itemArr:any;

    public ImageSrc: any;
    public baseURL = ConfigConstants.BaseURL;
    
    ngOnInit() {
      if(this.item != null && this.item.WidgetSetting != null && this.item.WidgetSetting != '' && this.testJSON(this.item.WidgetSetting)) {
        let widgetSetting = JSON.parse(this.item.WidgetSetting);
        if(widgetSetting.AssetLibraryId != 0 && widgetSetting.AssetName != '') {
          this.ImageSrc = this.baseURL + "assets/" + widgetSetting.AssetLibraryId + "/" + widgetSetting.AssetName;
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
    selector: 'vidyo',
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

  @Input() item:any;
  @Input('widgetsGridsterItemArray') itemArr:any;

  public videoSrc: any;
  public baseURL = ConfigConstants.BaseURL;
    
    ngOnInit() {
      if(this.item != null && this.item.WidgetSetting != null && this.item.WidgetSetting != '' && this.testJSON(this.item.WidgetSetting)) {
        let widgetSetting = JSON.parse(this.item.WidgetSetting);
        if(widgetSetting.AssetLibraryId != 0 && widgetSetting.AssetName != '') {
          this.videoSrc = this.baseURL + "assets/" + widgetSetting.AssetLibraryId + "/" + widgetSetting.AssetName;
        }
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

    removeItem(item) {
        this.widgetsGridsterItemArray.splice(this.widgetsGridsterItemArray.indexOf(item), 1);
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