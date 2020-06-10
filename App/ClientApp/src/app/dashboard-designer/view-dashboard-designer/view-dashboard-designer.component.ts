import { Component, OnInit, ViewChild } from '@angular/core';
import { CompactType, DisplayGrid, GridsterConfig, GridsterItem, GridType } from 'angular-gridster2';
import { Location } from '@angular/common';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
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

@Component({
  selector: 'app-view-dashboard-designer',
  templateUrl: './view-dashboard-designer.component.html',
  styleUrls: ['./view-dashboard-designer.component.scss']
})
export class ViewDashboardDesignerComponent implements OnInit {

    options: GridsterConfig;
    dashboard: Array<GridsterItem>;
    item: any[];
    public pageSize = 5;
    public currentPage = 0;
    public totalSize = 0;
    displayedColumnsSummary: string[] = ['account', 'currency', 'amount'];
    dataSourceSummary = new MatTableDataSource<any>();

    @ViewChild(MatSort, { static: true }) sort: MatSort;
    @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;


    ngAfterViewInit() {
        this.dataSourceSummary.paginator = this.paginator;

    }
    //Back Functionality.
    backClicked() {
        this._location.back();

    }
    //account info
    accountInfoLists: any[] = [
        { title: 'Statement Date', value: '1-APR-20' },
        { title: 'Statement Period', value: 'Annual Statement' },
        { title: 'Customer ID', value: 'ID2-8989-5656' },
        { title: 'RM Name', value: 'David Miller' },
        { title: 'RM Contact Number', value: '+4487867833' },
    ]
    constructor(private _location: Location) { }

    ngOnInit() {

        this.dataSourceSummary = new MatTableDataSource(List_Data_Summary);
        this.dataSourceSummary.sort = this.sort;

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
            mobileBreakpoint: 640,
            minCols: 20,
            maxCols: 20,
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
            fixedColWidth: 105,
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
                enabled: false,
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

        //for value
        this.dashboard = [
            { cols: 2, rows: 2, y: 0, x: 0 },
            { cols: 2, rows: 2, y: 0, x: 2, hasContent: true },
            { cols: 1, rows: 2, y: 0, x: 4 },
            { cols: 1, rows: 2, y: 2, x: 5 },
            { cols: 1, rows: 2, y: 1, x: 0 },
            { cols: 1, rows: 2, y: 1, x: 0 },
            { cols: 2, rows: 2, y: 3, x: 5, minItemRows: 2, minItemCols: 2, label: 'Min rows & cols = 2' },
            { cols: 2, rows: 2, y: 2, x: 0, maxItemRows: 2, maxItemCols: 2, label: 'Max rows & cols = 2' },
            { cols: 2, rows: 2, y: 2, x: 2, dragEnabled: true, resizeEnabled: true, label: 'Drag&Resize Enabled' },
            { cols: 1, rows: 2, y: 2, x: 4, dragEnabled: false, resizeEnabled: false, label: 'Drag&Resize Disabled' },
            { cols: 1, rows: 2, y: 2, x: 6 }
        ];
  }

}
