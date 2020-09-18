import { Component, Injector, OnInit, ViewChild } from '@angular/core';
import { DialogComponent, DialogService } from '@tomblue/ng2-bootstrap-modal';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { ScheduleLogServiceDetail } from '../../layout/logs-details/logs-details.service';
import { ScheduleLogDetail } from '../../layout/logs-details/log-details';
import { Constants } from 'src/app/shared/constants/constants';
import { ErrorMessageConstants } from 'src/app/shared/constants/constants';

@Component({
  selector: 'pagepreview',
  template: `<div class="modal-dialog modal-dialog-centered modal-xl" role="document">
                <div class="modal-content">
                  <div class="modal-body p-4">
                    <button type="button" class="close mt-n4 mr-n3" (click)="cancel()">
                      <span aria-hidden="true">&times;</span>
                    </button>
                    <div class="float-left">
                      <h6>Schedule Name : {{scheduleName}}</h6>
                    </div>
                    <div class="d-flex justify-content-center mb-1">
                        <div class="pagination-mat position-relative d-md-block d-none">
                          <mat-paginator #paginator [pageSize]="pageSize" [pageSizeOptions]="[5, 10, 20]"
                                  [showFirstLastButtons]="true" [length]="totalSize" [pageIndex]="currentPage" (page)="handlePage($event)">
                          </mat-paginator>
                        </div>                     
                    </div>
                    <div class="overflow-auto stylescrollbar" style="max-height:400px;">
                      <table mat-table [dataSource]="dataSource" matSort class="table table-responsive table-hover table-condensed table-cust">
                        <ng-container matColumnDef="scheduleLogId">
                          <th class="width15" mat-header-cell *matHeaderCellDef mat-sort-header> Schedule Log Id </th>
                          <td mat-cell *matCellDef="let element"> {{element.ScheduleLogId}} </td>
                        </ng-container>
                        <ng-container matColumnDef="scheduleLogDetailId">
                          <th class="width15" mat-header-cell *matHeaderCellDef mat-sort-header> Log Detail Id </th>
                          <td mat-cell *matCellDef="let element"> {{element.Identifier}} </td>
                        </ng-container>
                        <ng-container matColumnDef="customerId">
                          <th class="width10" mat-header-cell *matHeaderCellDef mat-sort-header> Customer Id </th>
                          <td mat-cell *matCellDef="let element"> {{element.CustomerId}} </td>
                        </ng-container>
                        <ng-container matColumnDef="customerName">
                          <th class="width15" mat-header-cell *matHeaderCellDef mat-sort-header> Customer Name </th>
                          <td mat-cell *matCellDef="let element"> {{element.CustomerName}} </td>
                        </ng-container>
                        <ng-container matColumnDef="errorLogMsg">
                          <th class="width45" mat-header-cell *matHeaderCellDef mat-sort-header> Error Log Message </th>
                          <td mat-cell *matCellDef="let element"> <div [innerHtml]="element.LogMessage"> </div> </td>
                        </ng-container>
                        <tr mat-header-row *matHeaderRowDef="displayedColumns"></tr>
                        <tr mat-row *matRowDef="let row; columns: displayedColumns;"></tr>
                      </table>
                    </div>
                  </div>
                </div>
              </div>`
})

export class ErrorLogsViewComponent extends DialogComponent<ErrorLogViewModel, boolean> implements OnInit, ErrorLogViewModel, DialogOptions {

  scheduleLogId: string;
  scheduleName: string;
  backdropColor: string = "red";
  public scheduleLogList: ScheduleLogDetail[] = [];
  public isLoaderActive: boolean = false;
  public isRecordFound: boolean = false;
  public pageNo = 0;
  public pageSize = 10;
  public currentPage = 0;
  public totalSize = 0;
  public array: any;
  public StatementName = '';

  displayedColumns: string[] = [ 'scheduleLogId', 'scheduleLogDetailId', 'customerId', 'customerName', 'errorLogMsg' ];
  dataSource = new MatTableDataSource<any>();

  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;

  constructor(dialogService: DialogService, 
    private injector: Injector,
    private scheduleLogService: ScheduleLogServiceDetail) {
      super(dialogService);
  }

  ngOnInit() {
    this.getScheduleLogs(null);
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

  async getScheduleLogs(searchParameter) {
    let scheduleLogService = this.injector.get(ScheduleLogServiceDetail);
    if (searchParameter == null) {
      searchParameter = {};
      searchParameter.IsActive = true;
      searchParameter.PagingParameter = {};
      searchParameter.PagingParameter.PageIndex = Constants.DefaultPageIndex;
      searchParameter.PagingParameter.PageSize = Constants.DefaultPageSize;
      searchParameter.SortParameter = {};
      searchParameter.SortParameter.SortColumn = 'Id';
      searchParameter.SortParameter.SortOrder = Constants.Descending;
      searchParameter.Status = "Failed";
    }
    searchParameter.ScheduleLogId = this.scheduleLogId;
    var response = await scheduleLogService.getScheduleLogDetail(searchParameter);
    this.scheduleLogList = response.List;
    if (this.scheduleLogList.length == 0) {
      let message = ErrorMessageConstants.getNoRecordFoundMessage;
    }
    this.dataSource = new MatTableDataSource<ScheduleLogDetail>(this.scheduleLogList);
    this.dataSource.sort = this.sort;
    this.array = this.scheduleLogList;
    this.totalSize = this.array.length;
    this.iterator();
  }

  ngAfterViewInit() {
    this.dataSource.paginator = this.paginator;
  }

  cancel() {
    this.close();
  }

}

export interface ErrorLogViewModel {
  scheduleLogId: string;
  scheduleName: string;
}

interface DialogOptions {
  backdropColor?: string;
}
