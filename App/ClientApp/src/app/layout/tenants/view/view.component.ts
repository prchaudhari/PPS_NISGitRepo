import { Component, OnInit, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { Router } from '@angular/router';
export interface ListElement {
  contactType: string;
  firstName: string;
  lastName: string;
  email: string;
  no: string;
}

const List_Data: ListElement[] = [
  { contactType: 'Type 01', firstName: 'John', lastName: 'Doe', email: 'john@xyz.com', no: '+91 6755344872' },
  { contactType: 'Type 01', firstName: 'John', lastName: 'Doe', email: 'john@xyz.com', no: '+91 6755344872' },
  { contactType: 'Type 01', firstName: 'John', lastName: 'Doe', email: 'john@xyz.com', no: '+91 6755344872' },
  { contactType: 'Type 01', firstName: 'John', lastName: 'Doe', email: 'john@xyz.com', no: '+91 6755344872' },
];

@Component({
  selector: 'app-view',
  templateUrl: './view.component.html',
  styleUrls: ['./view.component.scss']
})
export class ViewComponent implements OnInit {
  public isCollapsedDetails: boolean = false;
  public isCollapsedAssets: boolean = true;
  public isContactContainer: boolean = false;
  public pageSize = 5;
  public currentPage = 0;
  public totalSize = 0;

  displayedColumns: string[] = ['contactType', 'firstName', 'lastName', 'email', 'no'];
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
  ShowAddContactContainer(): void {
    this.isContactContainer = true;
  }
  CloseAddContactContainer(): void {
    this.isContactContainer = false;
  }
  navigateToListPage() {
    this.route.navigate(['tenants']);
  }
  constructor(private route: Router) { }

  ngOnInit() {
  }

}
