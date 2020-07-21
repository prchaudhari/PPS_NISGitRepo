import { Component, OnInit, ViewChild } from '@angular/core';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { Location } from '@angular/common';
export interface ListElement {
  id: string;
  date: string;
  period: string;
  customer: string;
  accountId: string;
  city: string;
}

const List_Data: ListElement[] = [
  { id: 'SD 01', date: '01/02/2020', period: '01/02/2020 - 02/02/2020', customer: 'customer A', accountId: '29', city: 'Pune' },
  { id: 'SD 01', date: '01/02/2020', period: '01/02/2020 - 02/02/2020', customer: 'customer A', accountId: '29', city: 'Pune' },
  { id: 'SD 01', date: '01/02/2020', period: '01/02/2020 - 02/02/2020', customer: 'customer A', accountId: '29', city: 'Mumbai' },
  { id: 'SD 01', date: '01/02/2020', period: '01/02/2020 - 02/02/2020', customer: 'customer A', accountId: '29', city: 'Pune' },
];


@Component({
  selector: 'app-statement-search',
  templateUrl: './statement-search.component.html',
  styleUrls: ['./statement-search.component.scss']
})
export class StatementSearchComponent implements OnInit {

  public isFilter: boolean = false;
  public pageSize = 5;
  public currentPage = 0;
  public totalSize = 0;
  closeFilter() {
    this.isFilter = !this.isFilter;
  }
  displayedColumns: string[] = ['id', 'date', 'period', 'customer', 'accountId','city', 'actions'];
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
  constructor(private _location: Location) { }

}

