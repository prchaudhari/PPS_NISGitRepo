import { Component, OnInit, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { Router } from '@angular/router';

export interface ListElement {
  name: string;
  domainName: string;
  country: string;
}

const List_Data: ListElement[] = [
    { name: 'Name 01', domainName: 'Domain 01', country: 'India'},
    { name: 'Name 01', domainName: 'Domain 01', country: 'India'},
    { name: 'Name 01', domainName: 'Domain 01', country: 'India'},
    { name: 'Name 01', domainName: 'Domain 01', country: 'India'},
    { name: 'Name 01', domainName: 'Domain 01', country: 'India'},
    { name: 'Name 01', domainName: 'Domain 01', country: 'India'},
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

  displayedColumns: string[] = ['name', 'domainName', 'country', 'actions'];
  dataSource = new MatTableDataSource<ListElement>(List_Data);

  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;

  ngAfterViewInit() {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
  }
  public handlePage(e: any) {
    this.currentPage = e.pageIndex;
    this.pageSize = e.pageSize;
  }
  navigateToAdd() {
    this.route.navigate(['tenants', 'Add']);
  }
  navigateToView() {
    this.route.navigate(['tenants','View']);
  }
  constructor(private route: Router) { }

  ngOnInit() {
  }

}
