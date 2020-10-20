import { Component, OnInit, Injector, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';
import { NgxUiLoaderService } from 'ngx-ui-loader';
import { Router, ActivatedRoute, NavigationEnd } from '@angular/router';
import { Constants } from '../../../shared/constants/constants';
import { ErrorMessageConstants } from '../../../shared/constants/constants';
import { MessageDialogService } from '../../../shared/services/mesage-dialog.service';
import { DynamicWidgetService } from '../dynamicWidget.service';
import { DynamicWidget } from '../dynamicwidget';
import { CdkDragDrop, moveItemInArray } from '@angular/cdk/drag-drop';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';

export interface ListElement {
  series: string;
  displayName: string;
}

const List_Data: ListElement[] = [
  { series: 'Field Name 01', displayName: 'FN 01' },
  { series: 'Field Name 02', displayName: 'FN 02' },
  { series: 'Field Name 03', displayName: 'FN 03' },
  { series: 'Field Name 04', displayName: 'FN 04' },
  { series: 'Field Name 05', displayName: 'FN 05' },
];

@Component({
  selector: 'app-view',
  templateUrl: './view.component.html',
  styleUrls: ['./view.component.scss']
})
export class ViewComponent implements OnInit {

  public pageSize = 5;
  public currentPage = 0;
  public totalSize = 0;
  public dynamicWidgetDetails: DynamicWidget;
  public params;
  public userClaimsRolePrivilegeOperations: any[] = [];

  displayedColumns: string[] = ['series', 'displayName'];
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

  tableHeader = [
    { fn: 'Field Name 01', name: 'Date', sorting: 'Yes' },
    { fn: 'Field Name 02', name: 'Amout', sorting: 'No' },
    { fn: 'Field Name 03', name: 'Narration', sorting: 'Yes' },
    { fn: 'Field Name 04', name: 'Balance', sorting: 'No' },
    { fn: 'Field Name 05', name: 'Product', sorting: 'Yes' },
    { fn: 'Field Name 06', name: 'User', sorting: 'Yes' },
    { fn: 'Field Name 07', name: 'Role', sorting: 'Yes' },
  ];

  formList = [
    { fn: 'Field Name 01', dn: 'Customer Name' },
    { fn: 'Field Name 02', dn: 'Customer Id' },
    { fn: 'Field Name 03', dn: 'Balance' },
    { fn: 'Field Name 01', dn: 'Account Id' },
  ];

  drop(event: CdkDragDrop<string[]>) {
    moveItemInArray(this.tableHeader, event.previousIndex, event.currentIndex);
  }

  //select widget type radio

  selectedLink: string = "Form";
  setWidgetType(e: string): void {
    //  this.selectRowsOption = '';
    this.selectedLink = e;
  }
  isSelected(name: string): boolean {
    if (!this.selectedLink) { // if no radio button is selected, always return false so every nothing is shown  
      return false;
    }
    return (this.selectedLink === name); // if current radio button is selected, return true, else return false 
  }

  navigateToListPage() {
    this._router.navigate(['dynamicwidget']);
  }

  constructor(
    private _router: Router,
    private injector: Injector,
    private _messageDialogService: MessageDialogService,
    private dynamicWidgetService: DynamicWidgetService
  ) {
    this.dynamicWidgetDetails = new DynamicWidget;
    _router.events.subscribe(e => {
      if (e instanceof NavigationEnd) {
        if (e.url.includes('/dynamicwidget/Add')) {
          localStorage.removeItem("dynamicwidgetparams");
        }
      }
    });

    _router.events.subscribe(e => {
      if (e instanceof NavigationEnd) {
        if (e.url.includes('/dynamicwidget')) {
          //set passing parameters to localstorage.
          this.params = JSON.parse(localStorage.getItem('dynamicwidgetparams'));
          if (localStorage.getItem('dynamicwidgetparams')) {
            this.dynamicWidgetDetails.Identifier = this.params.Routeparams.passingparams.StatementIdentifier;
            //this.getWidgetDetails();
          }
        } else {
          localStorage.removeItem("dynamicwidgetparams");
        }
      }
    });

  }

  ngOnInit() {
    var userClaimsDetail = JSON.parse(localStorage.getItem('userClaims'));
    if (userClaimsDetail) {
      this.userClaimsRolePrivilegeOperations = userClaimsDetail.Privileges;
    }
    else {
      this.userClaimsRolePrivilegeOperations = [];
    }
    if (localStorage.getItem('dynamicwidgetparams')) {
      this.dynamicWidgetDetails.Identifier = this.params.Routeparams.passingparams.StatementIdentifier;
      this.getWidgetDetails();
    }
  }
  async getWidgetDetails() {
    let searchParameter: any = {};
    searchParameter.PagingParameter = {};
    searchParameter.PagingParameter.PageIndex = Constants.DefaultPageIndex;
    searchParameter.PagingParameter.PageSize = Constants.DefaultPageSize;
    searchParameter.SortParameter = {};
    searchParameter.SortParameter.SortColumn = Constants.Name;
    searchParameter.SortParameter.SortOrder = Constants.Ascending;
    searchParameter.SearchMode = Constants.Exact;
    searchParameter.Identifier = this.dynamicWidgetDetails.Identifier;
    searchParameter.IsStatementPagesRequired = true;
    var response = await this.dynamicWidgetService.getDynamicWidgets(searchParameter);
    this.dynamicWidgetDetails = response.List[0];
  }
}

