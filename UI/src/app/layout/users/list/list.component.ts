import { Component, OnInit, ViewChild } from '@angular/core';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';

export interface ListElement {
    name: string;
    email: string;
    mobileno: string;
    role: string;
}

const List_Data: ListElement[] = [
    { name: 'John Doe', email: 'john@qbcd.com', mobileno: '+91 8788999088', role: 'Admin'  },
    { name: 'Ravi Kumar', email: 'kumar@qbcd.com', mobileno: '+91 3465778789', role: 'Producer' },
    { name: 'Sunita J', email: 'sunitaj@qbcd.com', mobileno: '+91 7654567222', role: 'Admin' },
    { name: 'Sunita J', email: 'sunitaj@qbcd.com', mobileno: '+91 7654567222', role: 'Admin' },
    { name: 'Sunita J', email: 'sunitaj@qbcd.com', mobileno: '+91 7654567222', role: 'Admin' },
    { name: 'Sunita J', email: 'sunitaj@qbcd.com', mobileno: '+91 7654567222', role: 'Admin' },
    { name: 'Sunita J', email: 'sunitaj@qbcd.com', mobileno: '+91 7654567222', role: 'Admin' },
    { name: 'Sunita J', email: 'sunitaj@qbcd.com', mobileno: '+91 7654567222', role: 'Admin' },
    { name: 'Sunita J', email: 'sunitaj@qbcd.com', mobileno: '+91 7654567222', role: 'Admin' },
    { name: 'Sunita J', email: 'sunitaj@qbcd.com', mobileno: '+91 7654567222', role: 'Admin' },
    { name: 'Sunita J', email: 'sunitaj@qbcd.com', mobileno: '+91 7654567222', role: 'Admin' },
    { name: 'Sunita J', email: 'sunitaj@qbcd.com', mobileno: '+91 7654567222', role: 'Admin' },
    { name: 'Sunita J', email: 'sunitaj@qbcd.com', mobileno: '+91 7654567222', role: 'Admin' },
    { name: 'Sunita J', email: 'sunitaj@qbcd.com', mobileno: '+91 7654567222', role: 'Admin' },
    { name: 'Sunita J', email: 'sunitaj@qbcd.com', mobileno: '+91 7654567222', role: 'Admin' },
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
    displayedColumns: string[] = ['name', 'email', 'mobileno', 'role', 'active', 'lock', 'actions'];
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
