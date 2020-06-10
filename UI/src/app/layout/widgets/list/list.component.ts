import { Component, OnInit, ViewChild } from '@angular/core';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';

export interface ListElement {
    name: string;
    page: string;
    widgettype: string;
}

const List_Data: ListElement[] = [
    { name: 'Widget 01', page: 'Home', widgettype:'Account Information' },
    { name: 'Widget 02', page: 'Home', widgettype:'Summary at Glance' },
    { name: 'Widget 03', page: 'Home', widgettype:'Reminder and Recommendation' },
    { name: 'Widget 04', page: 'Home', widgettype:'Image' },
    { name: 'Widget 05', page: 'Home', widgettype:'Video' },
    { name: 'Widget 06', page: 'Home', widgettype:'Analytics' },
    { name: 'Widget 07', page: 'Saving Account', widgettype:'Available balance' },
    { name: 'Widget 08', page: 'Saving Account', widgettype:'Top 4 income resources ' },
    { name: 'Widget 09', page: 'Saving Account', widgettype:'Transaction List ' },
    { name: 'Widget 10', page: 'Saving Account', widgettype:'Graph' },
    { name: 'Widget 10', page: 'Saving Account', widgettype: 'Video' },
    { name: 'Widget 10', page: 'Checking  Account', widgettype: 'Graph' },
    { name: 'Widget 11', page: 'Checking  Account', widgettype:'Transaction Mix ' },
];

@Component({
    selector: 'app-list',
    templateUrl: './list.component.html',
    styleUrls: ['./list.component.scss']
})
export class ListComponent implements OnInit {

    public isFilter: boolean = false;
    public pageSize = 5;
    public currentPage = 0;
    public totalSize = 0;
    closeFilter() {
        this.isFilter = !this.isFilter;
    }
    displayedColumns: string[] = ['name', 'page','widgettype', 'actions'];
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
    constructor() { }

}
