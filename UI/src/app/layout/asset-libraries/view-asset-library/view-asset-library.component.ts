import { Component, OnInit, ViewChild } from '@angular/core';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { Location } from '@angular/common';

export interface ListElement {
    name: string;
    updatedby: string;
    date: string;
}

const List_Data: ListElement[] = [
    { name: 'Asset01.png', date: '01/02/2020', updatedby: 'User 01' },
    { name: 'Asset02.png', date: '01/02/2020', updatedby: 'User 02' },
    { name: 'Asset03.png', date: '01/02/2020', updatedby: 'User 03' },
    { name: 'Asset04.png', date: '01/02/2020', updatedby: 'User 04' },
    { name: 'Asset05.png', date: '01/02/2020', updatedby: 'User 05' },
    { name: 'Asset06.png', date: '01/02/2020', updatedby: 'User 06' },
    { name: 'Asset07.png', date: '01/02/2020', updatedby: 'User 07' }
];

@Component({
  selector: 'app-view-asset-library',
  templateUrl: './view-asset-library.component.html',
  styleUrls: ['./view-asset-library.component.scss']
})
export class ViewAssetLibraryComponent implements OnInit {

    public pageSize = 5;
    public currentPage = 0;
    public totalSize = 0;

    displayedColumns: string[] = ['name', 'updatedby', 'date', 'actions'];
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

    public isCollapsedDetails: boolean = false;
    public isCollapsedAssets: boolean = true;
    navigateToListPage() {
        this._location.back();
    }
    constructor(private _location: Location) { }

}
