import { Component, OnInit, ViewChild } from '@angular/core';
import { CompactType, DisplayGrid, GridsterConfig, GridsterItem, GridType } from 'angular-gridster2';
import { Location } from '@angular/common';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import * as $ from 'jquery';

export interface ListElement {
    date: string;
    type: string;
    narration: string;
    fcy: string;
    currentRate: string;
    lyc: string;
}

const List_Data: ListElement[] = [
    { date: 'D3/D4/15', type: 'CR', narration: 'NXT TXN: IIFL IIFL3557346', fcy: '1666.67', currentRate: '1.062', lyc: '1771.42' },
    { date: 'D3/D4/15', type: 'CR', narration: 'NXT TXN: IIFL IIFL3557346', fcy: '1666.67', currentRate: '1.062', lyc: '1771.42' },
    { date: 'D3/D4/15', type: 'CR', narration: 'NXT TXN: IIFL IIFL3557346', fcy: '1666.67', currentRate: '1.062', lyc: '1771.42' },
    { date: 'D3/D4/15', type: 'CR', narration: 'NXT TXN: IIFL IIFL3557346', fcy: '1666.67', currentRate: '1.062', lyc: '1771.42' },
];
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
  selector: 'app-add-dashboard-designer',
  templateUrl: './add-dashboard-designer.component.html',
  styleUrls: ['./add-dashboard-designer.component.scss']
})
export class AddDashboardDesignerComponent implements OnInit {
    public isImageConfig: boolean = false;
    public isVideoConfig: boolean = false;
    public isWidgetSidebar: boolean = false;
    public isEmbedded: boolean = false;
    public isPersonalizeImage: boolean = false;
    public isPersonalize: boolean = false;

    isVideoConfigForm() {
        this.isVideoConfig = true;
    }
    isImageConfigForm() {
        this.isImageConfig = true;
    }
    options: GridsterConfig;
    dashboard: Array<GridsterItem>;
    item: any[];
    public pageSize = 5;
    public currentPage = 0;
    public totalSize = 0;
    displayedColumns: string[] = ['date', 'type', 'narration', 'fcy', 'currentRate', 'lyc', 'action',];
    displayedColumnsSummary: string[] = ['account', 'currency', 'amount'];
    dataSource = new MatTableDataSource<any>();
    dataSourceSummary = new MatTableDataSource<any>();

    @ViewChild(MatSort, { static: true }) sort: MatSort;
    @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;


    ngAfterViewInit() {
        this.dataSource.paginator = this.paginator;
        this.dataSourceSummary.paginator = this.paginator;

    }

    ListData: ListElement[] = [
        { date: 'D3/D4/15', type: 'CR', narration: 'NXT TXN: IIFL IIFL3557346', fcy: '1666.67', currentRate: '1.062', lyc: '1771.42' },
        { date: 'D3/D4/15', type: 'CR', narration: 'NXT TXN: IIFL IIFL3557346', fcy: '1666.67', currentRate: '1.062', lyc: '1771.42' },
        { date: 'D3/D4/15', type: 'CR', narration: 'NXT TXN: IIFL IIFL3557346', fcy: '1666.67', currentRate: '1.062', lyc: '1771.42' },
        { date: 'D3/D4/15', type: 'CR', narration: 'NXT TXN: IIFL IIFL3557346', fcy: '1666.67', currentRate: '1.062', lyc: '1771.42' },
    ];
    //account info
    accountInfoLists: any[] = [
        { title: 'Statement Date', value: '1-APR-20'},
        { title: 'Statement Period', value: 'Annual Statement'},
        { title: 'Customer ID', value: 'ID2-8989-5656'},
        { title: 'RM Name', value: 'David Miller'},
        { title: 'RM Contact Number', value: '+4487867833'},
    ]
    constructor(private _location: Location) { }

    ngOnInit() {

        $(document).ready(function () {
            $('.widget-delete-btn').click(function () {
                $(this).parent().parent().parent().parent().addClass('hide');
            });
        });

        this.dataSource = new MatTableDataSource(List_Data);
        this.dataSourceSummary = new MatTableDataSource(List_Data_Summary);
        this.dataSource.sort = this.sort;
        this.dataSourceSummary.sort = this.sort;

        //to hide tooltip
        //const paginatorIntl = this.paginator._intl;
        //paginatorIntl.nextPageLabel = '';
        //paginatorIntl.previousPageLabel = '';
        //paginatorIntl.firstPageLabel = '';
        //paginatorIntl.lastPageLabel = '';

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

    changedOptions() {
        if (this.options.api && this.options.api.optionsChanged) {
            this.options.api.optionsChanged();
        }
    }

    removeItem($event, item) {
        $event.preventDefault();
        $event.stopPropagation();
        this.dashboard.splice(this.dashboard.indexOf(item), 1);
    }

    addItem() {
        this.dashboard.push({ x: 0, y: 0, cols: 1, rows: 1 });
    }
    //Back Functionality.
    backClicked() {
        this._location.back();

    }

    isCustomerInformationWidgets: any[] = [];
    isAccountInformationWidgets: any[] = [];
    isSummaryatGlanceWidgets: any[] = [];
    isReminderWidgets: any[] = [];
    isInvestmentWidgets: any[] = [];
    isImageWidgets: any[] = [];
    isVideoWidgets: any[] = [];
    isAnalyticsWidgets: any[] = [];
    isAvailableBalanceWidgets: any[] = [];
    isTop4IncomeWidgets: any[] = [];
    isTransactionListWidgets: any[] = [];
    isSavingTrendWidgets: any[] = [];
    isTransactionMixWidgets: any[] = [];
    isSpendingTrendWidgets: any[] = [];
    isTransactionDetailsWidgets: any[] = [];
    isNewsAlertsWidgets: any[] = [];

    isCustomerInformation() {
        this.isCustomerInformationWidgets.push(this.isCustomerInformationWidgets)
    }
    isAccountInformation() {
        this.isAccountInformationWidgets.push(this.isAccountInformationWidgets)
    }
    isSummaryatGlance() {
        this.isSummaryatGlanceWidgets.push(this.isSummaryatGlanceWidgets)

    }
    isReminder() {
        this.isReminderWidgets.push(this.isReminderWidgets)
    }
    isInvestment() {
        this.isInvestmentWidgets.push(this.isInvestmentWidgets)
    }
    isImage() {
        this.isImageWidgets.push(this.isImageWidgets)
    }
    isVideo() {
        this.isVideoWidgets.push(this.isVideoWidgets)
    }
    isAnalytics() {
        this.isAnalyticsWidgets.push(this.isAnalyticsWidgets)
    }
    isAvailableBalance() {
        this.isAvailableBalanceWidgets.push(this.isAvailableBalanceWidgets)
    }
    isTransactionList() {
        this.isTransactionListWidgets.push(this.isTransactionListWidgets)
    }
    isSavingTrend() {
        this.isSavingTrendWidgets.push(this.isSavingTrendWidgets)
    }
    isTransactionMix() {
        this.isTransactionMixWidgets.push(this.isTransactionMixWidgets)
    }
    isSpendingTrend() {
        this.isSpendingTrendWidgets.push(this.isSpendingTrendWidgets)
    }
    isNewsAlerts() {
        this.isNewsAlertsWidgets.push(this.isNewsAlertsWidgets)
    }
    isTransactionDetails() {
        this.isTransactionDetailsWidgets.push(this.isTransactionDetailsWidgets)
    }
    isTop4Income() {
        this.isTop4IncomeWidgets.push(this.isTop4IncomeWidgets)
    }

}
