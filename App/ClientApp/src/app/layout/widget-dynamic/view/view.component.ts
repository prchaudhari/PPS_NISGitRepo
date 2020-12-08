import { Component, OnInit, Injector, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';
import { NgxUiLoaderService } from 'ngx-ui-loader';
import { Router, ActivatedRoute, NavigationEnd } from '@angular/router';
import { Constants, ErrorMessageConstants } from '../../../shared/constants/constants';
import { MessageDialogService } from '../../../shared/services/mesage-dialog.service';
import { DynamicWidgetService } from '../dynamicwidget.service';
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
  public lineBarGraphList: any[] = [];
  public conditionList: any[] = [
    { "Name": "Select", "Identifier": "0" },
    { "Name": "Equals To", "Identifier": "EqualsTo" },
    { "Name": "Not Equals To", "Identifier": "NotEqualsTo" },
    { "Name": "Less Than", "Identifier": "LessThan" },
    { "Name": "Greater Than", "Identifier": "GreaterThan" },
    { "Name": "Contains", "Identifier": "Contains" },
    { "Name": "Not Contains", "Identifier": "NotContains" }

  ];
  public themeCSS: any = {};
  displayedColumns: string[] = ['series', 'displayName'];
  dataSource = new MatTableDataSource<ListElement>(List_Data);
  public entityFieldList: any[] = [{ "Name": "Select", "Identifier": 0 }];

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
  public isDefault: boolean = true;
  public isCustome: boolean = false;
  tableHeader = [];

  formList = [];

  drop(event: CdkDragDrop<string[]>) {
    moveItemInArray(this.tableHeader, event.previousIndex, event.currentIndex);
  }
  public widgetFilterlist: any[] = [];
  public displayWidgetFilterlist: any[] = [];

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
            this.dynamicWidgetDetails.Identifier = this.params.Routeparams.passingparams.DynamicWidgetIdentifier;
            this.getEntityField(this.params.Routeparams.passingparams.EntityId);
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
      this.dynamicWidgetDetails.Identifier = this.params.Routeparams.passingparams.DynamicWidgetIdentifier;
      this.getWidgetDetails();
    }
  }
  async getEntityField(value) {
    var data = await this.dynamicWidgetService.getEntityFields(value);
    this.entityFieldList = [{ "Name": "Select", "Identifier": 0 }];
    data.forEach(item => {
      this.entityFieldList.push(item);
    });

    if (this.entityFieldList.length == 0) {
      let message = ErrorMessageConstants.getNoRecordFoundMessage;
      this._messageDialogService.openDialogBox('Error', message, Constants.msgBoxError).subscribe(data => {
      });
    }
  }
  async getWidgetDetails() {
    let searchParameter: any = {};
    searchParameter.PagingParameter = {};
    searchParameter.PagingParameter.PageIndex = Constants.DefaultPageIndex;
    searchParameter.PagingParameter.PageSize = Constants.DefaultPageSize;
    searchParameter.SortParameter = {};
    searchParameter.SortParameter.SortColumn = "Id";
    searchParameter.SortParameter.SortOrder = Constants.Ascending;
    searchParameter.SearchMode = Constants.Exact;
    searchParameter.Identifier = this.dynamicWidgetDetails.Identifier;
    searchParameter.IsStatementPagesRequired = true;
    var response = await this.dynamicWidgetService.getDynamicWidgets(searchParameter);
    this.dynamicWidgetDetails = response.List[0];


    //this.setWidgetType(this.dynamicWidgetDetails.WidgetType);
    this.selectedLink = this.dynamicWidgetDetails.WidgetType;
    // this.getEntityField(this.dynamicWidgetDetails.EntityId);
    if (this.dynamicWidgetDetails.ThemeCSS != null && this.dynamicWidgetDetails.ThemeCSS != '') {
      this.themeCSS = JSON.parse(this.dynamicWidgetDetails.ThemeCSS);

    }
    this.isDefault = this.dynamicWidgetDetails.ThemeType == "Default" ? true : false;

    this.isCustome = this.dynamicWidgetDetails.ThemeType == "Default" ? false : true;
    if (this.dynamicWidgetDetails.WidgetFilterSettings != null && this.dynamicWidgetDetails.WidgetFilterSettings != '') {
      this.widgetFilterlist = JSON.parse(this.dynamicWidgetDetails.WidgetFilterSettings);
      this.widgetFilterlist.forEach(item => {
        var object = item;
        object.OperatorName = this.conditionList.filter(i => i.Identifier == item.Operator)[0].Name;
        object.FieldName = this.entityFieldList.filter(i => i.Identifier == item.FieldId)[0].Name;
        this.displayWidgetFilterlist.push(object);
      });
    }
    if (this.dynamicWidgetDetails.WidgetSettings != null && this.dynamicWidgetDetails.WidgetSettings != '') {

      var settings;
      if (this.dynamicWidgetDetails.WidgetType != 'Html') {
        settings = JSON.parse(this.dynamicWidgetDetails.WidgetSettings);
      }

      if (this.dynamicWidgetDetails.WidgetType == 'Form') {
        this.formList = settings;
      }
      else if (this.dynamicWidgetDetails.WidgetType == 'Table') {
        this.tableHeader = settings;
      }
      else if (this.dynamicWidgetDetails.WidgetType == 'Pie') {

      }
      else if (this.dynamicWidgetDetails.WidgetType == 'LineGraph' || this.dynamicWidgetDetails.WidgetType == 'BarGraph') {
        //this.selectedTheme = this.isCustome ? themeCSS.ChartColorTheme : this.selectedTheme;
        this.lineBarGraphList = settings.Details;
        this.dataSource = new MatTableDataSource<any>(this.lineBarGraphList);

      }
      else if (this.dynamicWidgetDetails.WidgetType == 'Html') {
        // this.rteObj.executeCommand('insertHTML', this.dynamicWidgetDetails.WidgetSettings);

      }

    }
  }

  navigationToEditDynamicWidget() {
    let queryParams = {
      Routeparams: {
        passingparams: {
          "DynamicWidgetName": this.dynamicWidgetDetails.WidgetName,
          "DynamicWidgetIdentifier": this.dynamicWidgetDetails.Identifier,
        }
      }
    }
    localStorage.setItem("dynamicWidgetEditRouteparams", JSON.stringify(queryParams))
    this._router.navigate(['dynamicwidget', 'Edit']);
  }

  async DeleteDynamicWidget() {
    let message = "Are you sure, you want to delete this record?";
    this._messageDialogService.openConfirmationDialogBox('Confirm', message, Constants.msgBoxWarning).subscribe(async (isConfirmed) => {
      if (isConfirmed) {
        let dynamicWidgetData = [{
          "Identifier": this.dynamicWidgetDetails.Identifier,
        }];

        let resultFlag = await this.dynamicWidgetService.deleteDynamicWidget(dynamicWidgetData);
        if (resultFlag) {
          let messageString = Constants.recordDeletedMessage;
          this._messageDialogService.openDialogBox('Success', messageString, Constants.msgBoxSuccess);
          this.navigateToListPage();
        }
      }
    });
  }
}

