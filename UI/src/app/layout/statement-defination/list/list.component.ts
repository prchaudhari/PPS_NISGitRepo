import { Component, OnInit,ViewChild } from '@angular/core';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';

export interface ListElement {
    name: string;
    version: string;
    owner: string;
    date: string;
    status: string;
   
}

const List_Data: ListElement[] = [
    { name: 'SD 01', version: 'V.1.0.0', owner: 'John Doe', date: '01/02/2020', status: 'In Process'},
    { name: 'SD 02',  owner: 'Ravi', version: 'V.1.0.0', date: '01/02/2020', status: 'New' },
    { name: 'SD 03',  version: 'V.1.0.0', owner: 'John Doe', date: '01/02/2020', status: 'Published' },
    { name: 'SD 04',  owner: 'Naina', date: '01/02/2020', version: 'V.1.0.0', status: 'New' },
    { name: 'SD 05',  version: 'V.1.0.0', owner: 'John Doe', date: '01/02/2020', status: 'New'},
    { name: 'SD 06',  version: 'V.1.0.0', owner: 'David', date: '01/02/2020', status: 'New'},
    { name: 'SD 07', version: 'V.1.0.0', owner: 'John S.', date: '01/02/2020', status: 'New'}
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
    displayedColumns: string[] = ['name', 'version', 'owner', 'date', 'status', 'actions'];
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

