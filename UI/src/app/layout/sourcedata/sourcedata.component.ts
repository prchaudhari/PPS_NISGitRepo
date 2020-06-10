import { Component, OnInit, ViewChild } from '@angular/core';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { Location } from '@angular/common';

export interface ListElement {
    time: string;
    date: string;
    page: string;
    widget: string;
    userId: string;
}

const List_Data: ListElement[] = [
    { page: 'Home', time: '09:00', date: '02/05/2020', widget: 'Image', userId:'U001' },
    { page: 'Current Account', time: '10:00', date: '03/05/2020', widget: 'Video', userId: 'U002' },
    { page: 'Saving Account', time: '11:00', date: '04/05/2020', widget: 'Customer Information', userId: 'U003' },
    { page: 'Home', time: '11:00', date: '05/05/2020', widget: 'Customer Information', userId: 'U004' },
    { page: 'Saving Account', time: '11:00', date: '06/05/2020', widget: 'Available Balance', userId: 'U005' },
];

@Component({
  selector: 'app-sourcedata',
  templateUrl: './sourcedata.component.html',
  styleUrls: ['./sourcedata.component.scss']
})
export class SourcedataComponent implements OnInit {
    public isFilter: boolean = false;

    closeFilter() {
        this.isFilter = !this.isFilter;
    }
    public pageSize = 5;
    public currentPage = 0;
    public totalSize = 0;

    displayedColumns: string[] = ['date', 'time', 'page', 'widget','userId'];
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
