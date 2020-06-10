import { Component, OnInit, ViewChild } from '@angular/core';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { Location } from '@angular/common';
export interface ListElement {
    UserID: string;
    status: string;
    date: string;
}

const List_Data: ListElement[] = [
    { UserID: 'ID 01', status: 'Completed', date: '02/05/2020' },
    { UserID: 'ID 01', status: 'Failed', date: '03/05/2020' },
    { UserID: 'ID 01', status: 'In Progress', date: '04/05/2020' },
    { UserID: 'ID 01', status: 'Completed', date: '02/05/2020' },
    { UserID: 'ID 01', status: 'Completed', date: '05/05/2020' },
    { UserID: 'ID 01', status: 'Failed', date: '02/05/2020' },
];


@Component({
  selector: 'app-logs-details',
  templateUrl: './logs-details.component.html',
  styleUrls: ['./logs-details.component.scss']
})
export class LogsDetailsComponent implements OnInit {

    public isFilter: boolean = false;
    public pageSize = 5;
    public currentPage = 0;
    public totalSize = 0;
    closeFilter() {
        this.isFilter = !this.isFilter;
    }
    displayedColumns: string[] = ['UserID', 'status', 'date', 'actions'];
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
    navigateToListPage() {
        this._location.back();
    }
    constructor(private _location: Location) { }

}
