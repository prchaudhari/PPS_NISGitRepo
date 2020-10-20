import { Component, OnInit, Injector, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';
import { NgxUiLoaderService } from 'ngx-ui-loader';
import { Router } from '@angular/router';
import { Constants } from '../../../shared/constants/constants';
import { ErrorMessageConstants } from '../../../shared/constants/constants';
import { MessageDialogService } from '../../../shared/services/mesage-dialog.service';
import { LocalStorageService } from '../../../shared/services/local-storage.service';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { DynamicWidgetService } from '../dynamicWidget.service';
import { DynamicWidget } from '../dynamicwidget';
import { TemplateService } from '../../template/template.service';

export interface ListElement {
  Product: string;
  Entity: string;
  Widget: string;
  Owner: string;
  PublishedBy: string;
  PublishedDate: string;
  Status: string;
}
const List_Data: ListElement[] = [
  { Product: 'Common', Entity: 'Customer', Widget: 'Line Graph', Owner: 'NIS Super Admin', PublishedBy: 'NIS Super Admin', PublishedDate: '22/06/2020', Status: 'Published' },
  { Product: 'Saving', Entity: 'Account', Widget: 'Bar Graph', Owner: 'NIS Super Admin', PublishedBy: 'NIS Super Admin', PublishedDate: '22/06/2020', Status: 'New' },
  { Product: 'Current', Entity: 'Transaction', Widget: 'Pie Chart', Owner: 'NIS Super Admin', PublishedBy: 'NIS Super Admin', PublishedDate: '22/06/2020', Status: 'Published' },
  { Product: 'Current', Entity: 'Transaction', Widget: 'Table', Owner: 'NIS Super Admin', PublishedBy: 'NIS Super Admin', PublishedDate: '22/06/2020', Status: 'New' },
  { Product: 'Current', Entity: 'Transaction', Widget: 'Form', Owner: 'NIS Super Admin', PublishedBy: 'NIS Super Admin', PublishedDate: '22/06/2020', Status: 'Published' },
];

@Component({
  selector: 'app-list',
  templateUrl: './list.component.html',
  styleUrls: ['./list.component.scss']
})
export class ListComponent implements OnInit {

  //public variables
  public isFilter: boolean = false;
  public templateList: DynamicWidget[] = [];
  public isLoaderActive: boolean = false;
  public isRecordFound: boolean = false;
  public dynamicWidgetNo = 0;
  public pageSize = 5;
  public currentPage = 0;
  public totalSize = 0;
  public array: any;
  public isFilterDone = false;
  public sortedDynamicWidgetList: DynamicWidget[] = [];
  public dynamicWidgetTypeList: any[] = [];
  public DynamicWidgetFilterForm: FormGroup;
  public filterFromDateError: boolean = false;
  public filterFromDateErrorMessage: string = "";
  public filterToDateError: boolean = false;
  public filterToDateErrorMessage: string = "";

  displayedColumns: string[] = ['Name','Product', 'Entity', 'Widget', 'Owner', 'PublishedBy', 'PublishedDate', 'Status', 'actions'];
  dataSource = new MatTableDataSource<any>();
  public userClaimsRolePrivilegeOperations: any[] = [];

  public filterDsplyName = '';
  public filterPageTypeId: number = 0;
  public filterEntityId: number = 0;
  public filterDynamicWidgetStatus: string = '';
  public filterDynamicWidgetType: string = '';
  public filterDynamicWidgetOwner = '';
  public filterPublishStartDate = null;
  public filterPublishEndDate = null;
  public totalRecordCount = 0;
  public sortOrder = Constants.Descending;
  public sortColumn = 'PublishedDate';
  public pageTypeList: any[] = [{ "PageTypeName": "Select Page Type", "Identifier": 0 }];
  public entityList: any[] = [{ "Name": "Select Entity", "Identifier": 0 }];
  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;

  constructor(private injector: Injector,
    private fb: FormBuilder,
    private uiLoader: NgxUiLoaderService,
    private _messageDialogService: MessageDialogService,
    private route: Router,
    private localstorageservice: LocalStorageService,
    private dynamicWidgetService: DynamicWidgetService) {
    this.sortedDynamicWidgetList = this.templateList.slice();
    if (localStorage.getItem('dynamicWidgetAddRouteparams')) {
      localStorage.removeItem('dynamicWidgetAddRouteparams');
    }
    if (localStorage.getItem('dynamicWidgetEditRouteparams')) {
      localStorage.removeItem('dynamicWidgetEditRouteparams');
    }
  }

  public handlePage(e: any) {
    this.currentPage = e.pageIndex;
    this.pageSize = e.pageSize;
    //this.iterator();
    this.getDynamicWidgets(null);
  }

  get filterOwner() {
    return this.DynamicWidgetFilterForm.get('filterOwner');
  }
  get filterPageType() {
    return this.DynamicWidgetFilterForm.get('filterPageType');
  }
  get filterEntity() {
    return this.DynamicWidgetFilterForm.get('filterEntity');
  }
  get filterWidgetType() {
    return this.DynamicWidgetFilterForm.get('filterWidgetType');
  }
  get filterStatus() {
    return this.DynamicWidgetFilterForm.get('filterStatus');
  }
  get filterPublishedOnFromDate() {
    return this.DynamicWidgetFilterForm.get('filterPublishedOnFromDate');
  }
  get filterPublishedOnToDate() {
    return this.DynamicWidgetFilterForm.get('filterPublishedOnToDate');
  }

  //End getter methods
  public DataFormat;

  ngOnInit() {
    this.DataFormat = localStorage.getItem('DateFormat');
    this.getDynamicWidgets(null);
    this.getPageTypes();
    this.getEntities();
    //this.getEntityFields();
    this.DynamicWidgetFilterForm = this.fb.group({
      filterPageType: [0],
      filterEntity: [0],
      filterWidgetType: [0],
      filterOwner: [null],
      filterStatus: [0],
      filterPublishedOnFromDate: [null],
      filterPublishedOnToDate: [null]
    });

    var userClaimsDetail = JSON.parse(localStorage.getItem('userClaims'));
    if (userClaimsDetail) {
      this.userClaimsRolePrivilegeOperations = userClaimsDetail.Privileges;
    }
    else {
      this.userClaimsRolePrivilegeOperations = [];
    }
 
  }

  sortData(sort: MatSort) {
    const data = this.templateList.slice();
    if (!sort.active || sort.direction === '') {
      this.sortedDynamicWidgetList = data;
      return;
    }

    if (sort.direction == 'asc') {
      this.sortOrder = Constants.Ascending;
    } else {
      this.sortOrder = Constants.Descending;
    }
    //['Name', 'Product', 'Entity', 'Widget', 'Owner', 'PublishedBy', 'PublishedDate', 'Status', 'actions'];
    switch (sort.active) {
      case 'Name': this.sortColumn = "WidgetName"; break;
      case 'Product': this.sortColumn = "PageTypeName"; break;
      case 'Entity': this.sortColumn = "EntityName"; break;
      case 'Widget': this.sortColumn = "WidgetType"; break;
      case 'Owner': this.sortColumn = "CreatedByName"; break;
      case 'PublishedBy': this.sortColumn = "PublishedByName"; break;
      case 'PublishedDate': this.sortColumn = "PublishedDate"; break;
      case 'Status': this.sortColumn = "Status"; break;
      default: this.sortColumn = "PublishedDate"; break;
    }

    let searchParameter: any = {};
    searchParameter.IsActive = true;
    searchParameter.PagingParameter = {};
    searchParameter.PagingParameter.PageIndex = this.currentPage + 1;
    searchParameter.PagingParameter.PageSize = this.pageSize;
    searchParameter.SortParameter = {};
    searchParameter.SortParameter.SortColumn = this.sortColumn;
    searchParameter.SortParameter.SortOrder = this.sortOrder;
    searchParameter.SearchMode = Constants.Contains;
    this.getDynamicWidgets(searchParameter);
  }

  async getDynamicWidgets(searchParameter) {
    let dynamicWidgetService = this.injector.get(DynamicWidgetService);
    if (searchParameter == null) {
      searchParameter = {};
      searchParameter.IsActive = true;
      searchParameter.PagingParameter = {};
      searchParameter.PagingParameter.PageIndex = this.currentPage + 1;
      searchParameter.PagingParameter.PageSize = this.pageSize;
      searchParameter.SortParameter = {};
      searchParameter.SortParameter.SortColumn = this.sortColumn;
      searchParameter.SortParameter.SortOrder = this.sortOrder;
      searchParameter.SearchMode = Constants.Contains;
    }

    if (this.filterPageTypeId != 0) {
      searchParameter.PageTypeId = this.filterPageTypeId;
    }
    if (this.filterEntityId != 0) {
      searchParameter.EntityId = this.filterEntityId;
    }
    if (this.filterDynamicWidgetType != null && this.filterDynamicWidgetType != '') {
      searchParameter.WidgetType = this.filterDynamicWidgetType;
    }

    if (this.filterDynamicWidgetStatus != null && this.filterDynamicWidgetStatus != '') {
      searchParameter.Status = this.filterDynamicWidgetStatus;
    }
    if (this.filterPublishStartDate != null && this.filterPublishStartDate != '') {
      searchParameter.StartDate = new Date(this.filterPublishStartDate.setHours(0, 0, 0));
      //searchParameter.SortParameter.SortColumn = 'PublishedOn';
    }
    if (this.filterPublishEndDate != null && this.filterPublishEndDate != '') {
      searchParameter.EndDate = new Date(this.filterPublishEndDate.setHours(23, 59, 59));
      //searchParameter.SortParameter.SortColumn = 'PublishedOn';
    }
    var response = await dynamicWidgetService.getDynamicWidgets(searchParameter);
    this.templateList = response.List;
    this.totalRecordCount = response.RecordCount;
    if (this.templateList.length == 0 && this.isFilterDone == true) {
      let message = ErrorMessageConstants.getNoRecordFoundMessage;
      this._messageDialogService.openDialogBox('Error', message, Constants.msgBoxError).subscribe(data => {
        if (data == true) {
          this.resetDynamicWidgetFilterForm();
          this.getDynamicWidgets(null);
        }
      });
    }
    this.dataSource = new MatTableDataSource<DynamicWidget>(this.templateList);
    this.dataSource.sort = this.sort;
    this.array = this.templateList;
    this.totalSize = this.totalRecordCount;
    //this.iterator();
  }

  validateFilterDate(): boolean {
    if (this.DynamicWidgetFilterForm.value.filterPublishedOnFromDate != null && this.DynamicWidgetFilterForm.value.filterPublishedOnFromDate != '' &&
      this.DynamicWidgetFilterForm.value.filterPublishedOnToDate != null && this.DynamicWidgetFilterForm.value.filterPublishedOnToDate != '') {
      let startDate = this.DynamicWidgetFilterForm.value.filterPublishedOnFromDate;
      let toDate = this.DynamicWidgetFilterForm.value.filterPublishedOnToDate;
      if (startDate.getTime() > toDate.getTime()) {
        this.filterFromDateError = true;
        return false;
      }
    }
    return true;
  }

  onPublishedFilterDateChange(event) {
    this.filterFromDateError = false;
    this.filterToDateError = false;
    this.filterFromDateErrorMessage = "";
    this.filterToDateErrorMessage = "";
    let currentDte = new Date();
    if (this.DynamicWidgetFilterForm.value.filterPublishedOnFromDate != null && this.DynamicWidgetFilterForm.value.filterPublishedOnFromDate != '') {
      let startDate = this.DynamicWidgetFilterForm.value.filterPublishedOnFromDate;
      if (startDate.getTime() > currentDte.getTime()) {
        this.filterFromDateError = true;
        this.filterFromDateErrorMessage = ErrorMessageConstants.getStartDateLessThanCurrentDateMessage;
      }
    }
    if (this.DynamicWidgetFilterForm.value.filterPublishedOnToDate != null && this.DynamicWidgetFilterForm.value.filterPublishedOnToDate != '') {
      let toDate = this.DynamicWidgetFilterForm.value.filterPublishedOnToDate;
      if (toDate.getTime() > currentDte.getTime()) {
        this.filterToDateError = true;
        this.filterToDateErrorMessage = ErrorMessageConstants.getEndDateLessThanCurrentDateMessage;
      }
    }
    if (this.DynamicWidgetFilterForm.value.filterPublishedOnFromDate != null && this.DynamicWidgetFilterForm.value.filterPublishedOnFromDate != '' &&
      this.DynamicWidgetFilterForm.value.filterPublishedOnToDate != null && this.DynamicWidgetFilterForm.value.filterPublishedOnToDate != '') {
      let startDate = this.DynamicWidgetFilterForm.value.filterPublishedOnFromDate;
      let toDate = this.DynamicWidgetFilterForm.value.filterPublishedOnToDate;
      if (startDate.getTime() > toDate.getTime()) {
        this.filterFromDateError = true;
        this.filterFromDateErrorMessage = ErrorMessageConstants.getStartDateLessThanEndDateMessage;
      }
    }
  }

  //This method has been used for fetching search records
  searchDynamicWidgetRecordFilter(searchType) {
    this.filterFromDateError = false;
    this.isFilterDone = true;
    if (searchType == 'reset') {
      this.resetDynamicWidgetFilterForm();
      this.getDynamicWidgets(null);
      this.isFilter = !this.isFilter;
    }
    else {
      if (this.validateFilterDate()) {
        let searchParameter: any = {};
        searchParameter.PagingParameter = {};
        searchParameter.PagingParameter.PageIndex = 1;
        searchParameter.PagingParameter.PageSize = this.pageSize;
        searchParameter.SortParameter = {};
        searchParameter.SortParameter.SortColumn = this.sortColumn;
        searchParameter.SortParameter.SortOrder = this.sortOrder;
        searchParameter.SearchMode = Constants.Contains;

        if (this.DynamicWidgetFilterForm.value.filterOwner != null && this.DynamicWidgetFilterForm.value.filterOwner != '') {
          this.filterDynamicWidgetOwner = this.DynamicWidgetFilterForm.value.filterOwner.trim();
          searchParameter.DynamicWidgetOwner = this.DynamicWidgetFilterForm.value.filterOwner.trim();
        }
        if (this.DynamicWidgetFilterForm.value.filterPageType != 0) {
          this.filterPageTypeId = this.DynamicWidgetFilterForm.value.filterPageType;
          searchParameter.PageTypeId = this.DynamicWidgetFilterForm.value.filterPageType;
        }
        if (this.DynamicWidgetFilterForm.value.filterEntity != 0) {

          searchParameter.EntityId = this.DynamicWidgetFilterForm.value.filterEntity;
        }
        if (this.DynamicWidgetFilterForm.value.filterWidgetType != 0) {
          searchParameter.WidgetType = this.DynamicWidgetFilterForm.value.filterWidgetType ;
        }
        if (this.DynamicWidgetFilterForm.value.filterStatus != null && this.DynamicWidgetFilterForm.value.filterStatus != 0) {
          this.filterDynamicWidgetStatus = this.DynamicWidgetFilterForm.value.filterStatus;
          searchParameter.Status = this.DynamicWidgetFilterForm.value.filterStatus;
        }
        if (this.DynamicWidgetFilterForm.value.filterPublishedOnFromDate != null && this.DynamicWidgetFilterForm.value.filterPublishedOnFromDate != '') {
          this.filterPublishStartDate = this.DynamicWidgetFilterForm.value.filterPublishedOnFromDate;
          searchParameter.StartDate = new Date(this.DynamicWidgetFilterForm.value.filterPublishedOnFromDate.setHours(0, 0, 0));
        }
        if (this.DynamicWidgetFilterForm.value.filterPublishedOnToDate != null && this.DynamicWidgetFilterForm.value.filterPublishedOnToDate != '') {
          this.filterPublishEndDate = this.DynamicWidgetFilterForm.value.filterPublishedOnToDate;
          searchParameter.EndDate = new Date(this.DynamicWidgetFilterForm.value.filterPublishedOnToDate.setHours(23, 59, 59));
        }

        this.currentPage = 0;
        this.getDynamicWidgets(searchParameter);
        this.isFilter = !this.isFilter;
      }
    }
  }

  resetDynamicWidgetFilterForm() {
    this.DynamicWidgetFilterForm.patchValue({
      filterPageType: [0],
      filterEntity: [0],
      filterWidgetType: [0],
      filterOwner: [null],
      filterStatus: [0],
      filterPublishedOnFromDate: [null],
      filterPublishedOnToDate: [null]
    });

    this.currentPage = 0;
    this.filterDynamicWidgetOwner = '';
    this.filterDynamicWidgetStatus = '';
    this.filterPageTypeId = 0;
    this.filterPublishStartDate = null;
    this.filterPublishEndDate = null;
    this.filterFromDateError = false;
    this.filterToDateError = false;
    this.filterFromDateErrorMessage = "";
    this.filterToDateErrorMessage = "";
  }

  closeFilter() {
    this.isFilter = !this.isFilter;
  }

  async DeleteDynamicWidget(template: DynamicWidget) {
    let message = "Are you sure, you want to delete this record?";
    this._messageDialogService.openConfirmationDialogBox('Confirm', message, Constants.msgBoxWarning).subscribe(async (isConfirmed) => {
      if (isConfirmed) {
        let dynamicWidgetData = [{
          "Identifier": template.Identifier,
        }];

        let resultFlag = await this.dynamicWidgetService.deleteDynamicWidget(dynamicWidgetData);
        if (resultFlag) {
          let messageString = Constants.recordDeletedMessage;
          this._messageDialogService.openDialogBox('Success', messageString, Constants.msgBoxSuccess);
          this.resetDynamicWidgetFilterForm();
          this.getDynamicWidgets(null);
        }
      }
    });
  }

  async PublishDynamicWidget(template: DynamicWidget) {
    let message = "Are you sure, you want to publish this record?";
    this._messageDialogService.openConfirmationDialogBox('Confirm', message, Constants.msgBoxWarning).subscribe(async (isConfirmed) => {
      if (isConfirmed) {
        let dynamicWidgetData = [{
          "Identifier": template.Identifier,
        }];
        let resultFlag = await this.dynamicWidgetService.publishDynamicWidget(dynamicWidgetData);
        if (resultFlag) {
          let messageString = Constants.DynamicWidgetPublishedSuccessfullyMessage;
          this._messageDialogService.openDialogBox('Success', messageString, Constants.msgBoxSuccess);
          this.resetDynamicWidgetFilterForm();
          this.getDynamicWidgets(null);
        }
      }
    });
  }

  async CloneDynamicWidget(template: DynamicWidget) {
    let message = "Are you sure, you want to clone this record?";
    this._messageDialogService.openConfirmationDialogBox('Confirm', message, Constants.msgBoxWarning).subscribe(async (isConfirmed) => {
      if (isConfirmed) {
        let dynamicWidgetData = [{
          "Identifier": template.Identifier,
        }];
        let resultFlag = await this.dynamicWidgetService.cloneDynamicWidget(dynamicWidgetData);
        if (resultFlag) {
          let messageString = Constants.DynamicWidgetCloneSuccessfullyMessage;
          this._messageDialogService.openDialogBox('Success', messageString, Constants.msgBoxSuccess);
          this.resetDynamicWidgetFilterForm();
          this.getDynamicWidgets(null);
        }
      }
    });
  }

  async PreviewDynamicWidget(template: DynamicWidget) {
    let dynamicWidgetData = [{
      "Identifier": template.Identifier,
    }];
    let resultHtmlString = await this.dynamicWidgetService.previewDynamicWidget(dynamicWidgetData);
    if (resultHtmlString != '') {
      this._messageDialogService.openPreviewDialogBox(resultHtmlString);
    }
  }

  navigationToEditDynamicWidget(template: DynamicWidget) {
    let queryParams = {
      Routeparams: {
        passingparams: {
          "DynamicWidgetName": template.WidgetName,
          "DynamicWidgetIdentifier": template.Identifier,
        }
      }
    }
    localStorage.setItem("dynamicWidgetEditRouteparams", JSON.stringify(queryParams))
    this.route.navigate(['dynamicwidget', 'Edit']);
  }

  navigateToView() {
    this.route.navigate(['dynamicwidget', 'View']);
  }

  navigateToAdd() {
    this.route.navigate(['dynamicwidget', 'Add']);
  }

  async getPageTypes() {
    let dynamicWidgetService = this.injector.get(TemplateService);
    var data = await dynamicWidgetService.getPageTypes();
    data.forEach(item => {
      this.pageTypeList.push(item);
    })  
    if (this.pageTypeList.length == 0) {
      let message = ErrorMessageConstants.getNoRecordFoundMessage;
      this._messageDialogService.openDialogBox('Error', message, Constants.msgBoxError).subscribe(data => {
        if (data == true) {
          this.getPageTypes();
        }
      });
    }
  }

  async getEntities() {
    
    var data = await this.dynamicWidgetService.getEntities();
    data.forEach(item => {
      this.entityList.push(item);
    }) 
    if (this.entityList.length == 0) {
      let message = ErrorMessageConstants.getNoRecordFoundMessage;
      this._messageDialogService.openDialogBox('Error', message, Constants.msgBoxError).subscribe(data => {
      });
    }

  }
 
  public onPageTypeSelected(event) {
    const value = event.target.value;
    if (value == "0") {
      this.filterPageTypeId = 0;
    }
    else {
      this.filterPageTypeId = Number(value);
    }
  }

  public onEntitySelected(event) {
    const value = event.target.value;
    if (value == "0") {
      this.filterPageTypeId = 0;
    }
    else {
      this.filterPageTypeId = Number(value);
    }
  }
}

