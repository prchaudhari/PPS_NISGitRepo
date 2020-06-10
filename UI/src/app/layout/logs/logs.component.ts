import { Component, OnInit, ViewChild } from '@angular/core';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';

export interface ListElement {
    schedule: string;
    time: string;
    status: string;
    date: string;
    record: string;
}

const List_Data: ListElement[] = [
    { schedule: 'SM 01', time: '40 Minutes',record:'25/25', status: 'Completed', date:'02/05/2020' },
    { schedule: 'SM 01', time: '20 Minutes',record:'0/25', status: 'Failed', date:'03/05/2020' },
    { schedule: 'SM 01', time: '30 Minutes',record:'13/25', status: 'In Progress', date:'04/05/2020' },
    { schedule: 'SM 01', time: '50 Minutes',record:'30/30', status: 'Completed', date:'02/05/2020' },
    { schedule: 'SM 01', time: '60 Minutes',record:'125/125', status: 'Completed', date:'05/05/2020' },
    { schedule: 'SM 01', time: '20 Minutes',record:'0/25', status: 'Failed', date:'02/05/2020' },
];

@Component({
  selector: 'app-logs',
  templateUrl: './logs.component.html',
  styleUrls: ['./logs.component.scss']
})
export class LogsComponent implements OnInit {


    public isFilter: boolean = false;
    public pageSize = 5;
    public currentPage = 0;
    public totalSize = 0;
    closeFilter() {
        this.isFilter = !this.isFilter;
    }
    displayedColumns: string[] = ['schedule', 'time', 'record','status', 'date', 'actions'];
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
