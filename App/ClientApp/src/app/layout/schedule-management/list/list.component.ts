import { Component, OnInit, ViewChild } from '@angular/core';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';

export interface ListElement {
    name: string;
    template: string;
    startDate: string;
    endDate: string;
    DayOfMonth: string;
}

const List_Data: ListElement[] = [
    { name: 'SM 01', template: 'SD 01', startDate: '01/02/2020', endDate: '02/02/2020', DayOfMonth:'29' },
    { name: 'SM 02', template: 'SD 02', startDate: '04/03/2020', endDate: '-', DayOfMonth:'27' },
    { name: 'SM 03', template: 'Sd 03', startDate: '05/04/2020', endDate: '-', DayOfMonth:'22' },
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
    displayedColumns: string[] = ['name', 'template', 'startDate', 'endDate', 'DayOfMonth', 'actions'];
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
