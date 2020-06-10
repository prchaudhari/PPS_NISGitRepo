import { Component, OnInit, ViewChild } from '@angular/core';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';

export interface ListElement {
    name: string;
    description: string;
}

const List_Data: ListElement[] = [
    { name: 'Role 01', description: 'Lorem Ipsum has been the industrys standard'},
    { name: 'Role 02', description: '-'},
    { name: 'Role 03', description: 'Lorem Ipsum has been the industry'},
    { name: 'Role 04', description: '-'},
    { name: 'Role 05', description: 'dummy text ever since the 1500s'},
    { name: 'Role 06', description: 'Lorem Ipsum has been the industrys standard dummy text ever since the 1500s'},
    { name: 'Role 07', description: 'dummy text ever since the 1500s'}
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
    displayedColumns: string[] = ['name', 'description','actions'];
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
