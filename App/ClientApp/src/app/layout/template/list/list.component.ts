import { Component, OnInit, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';

export interface ListElement {
    name: string;
    version: string;
    owner: string;
    date: string;
    status: string;
    pagetype: string;
}

const List_Data: ListElement[] = [
    { name: 'Display Name 01',pagetype:'Home', version: 'V.1.0.0', owner: 'John Doe', date: '01/02/2020', status: 'In Process' },
    { name: 'Display Name 02',pagetype:'Current Account', owner: 'Ravi', version: 'V.1.0.0', date: '01/02/2020', status: 'New' },
    { name: 'Display Name 03',pagetype:'Home', version: 'V.1.0.0', owner: 'John Doe', date: '01/02/2020', status: 'Published' },
    { name: 'Display Name 04',pagetype:'Saving Accounr', owner: 'Naina', date: '01/02/2020', version: 'V.1.0.0', status: 'New' },
    { name: 'Display Name 05',pagetype:'Home', version: 'V.1.0.0', owner: 'John Doe', date: '01/02/2020', status: 'New' },
    { name: 'Display Name 06',pagetype:'Home', version: 'V.1.0.0', owner: 'David', date: '01/02/2020', status: 'New' },
    { name: 'Display Name 07',pagetype:'Home', version: 'V.1.0.0', owner: 'John S.', date: '01/02/2020', status: 'New' }
];

@Component({
    selector: 'app-list',
    templateUrl: './list.component.html',
    styleUrls: ['./list.component.scss']
})
export class ListComponent implements OnInit {

    navigationTodashboardDesigner() {
        this.route.navigate(['../dashboardDesignerView']);
    }

    constructor(private route: Router) { }

    public isFilter: boolean = false;
    public pageSize = 5;
    public currentPage = 0;
    public totalSize = 0;
    closeFilter() {
        this.isFilter = !this.isFilter;
    }
    displayedColumns: string[] = ['name','pagetype', 'version', 'owner', 'date', 'status', 'actions'];
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

}
