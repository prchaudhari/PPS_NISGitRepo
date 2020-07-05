import { Component, OnInit, Injector, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';
import { NgxUiLoaderService } from 'ngx-ui-loader';
import { Router } from '@angular/router';
import { Constants } from 'src/app/shared/constants/constants';
import { ErrorMessageConstants } from 'src/app/shared/constants/constants';
import { MessageDialogService } from 'src/app/shared/services/mesage-dialog.service';
import { LocalStorageService } from 'src/app/shared/services/local-storage.service';
import { MatPaginator } from '@angular/material/paginator';
import {MatSort} from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { TemplateService } from '../template.service';
import { Template } from '../template';

@Component({
    selector: 'app-list',
    templateUrl: './list.component.html',
    styleUrls: ['./list.component.scss']
})
export class ListComponent implements OnInit {

    //public variables
    public isFilter: boolean = false;
    public templateList: Template[] = [];
    public isLoaderActive: boolean = false;
    public isRecordFound: boolean = false;
    public pageNo = 0;
    public pageSize = 5;
    public currentPage = 0;
    public totalSize = 0;
    public array: any;
    public isFilterDone = false;
    public sortedTemplateList : Template[] = [];
    public pageTypeList: any[] = [];
    public TemplateFilterForm: FormGroup;
    public filterPageTypeId: number = 0;
    public filterPageStatus: string = '';
    public filterFromDateError: boolean = false;
    public filterFromDateErrorMessage: string = "";
    public filterToDateError: boolean = false;
    public filterToDateErrorMessage: string = "";

    displayedColumns: string[] = ['name','pagetype', 'version', 'owner', 'publishedBy', 'date', 'status', 'actions'];
    dataSource = new MatTableDataSource<any>();

    @ViewChild(MatSort, { static: true }) sort: MatSort;
    @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;

    constructor(private injector: Injector,
        private fb: FormBuilder,
        private uiLoader: NgxUiLoaderService,
        private _messageDialogService: MessageDialogService,
        private route: Router,
        private localstorageservice: LocalStorageService,
        private templateService: TemplateService) {
            this.sortedTemplateList = this.templateList.slice();
        }

    public handlePage(e: any) {
        this.currentPage = e.pageIndex;
        this.pageSize = e.pageSize;
        this.iterator();
    }

    private iterator() {
        const end = (this.currentPage + 1) * this.pageSize;
        const start = this.currentPage * this.pageSize;
        const part = this.array.slice(start, end);
        this.dataSource = part;
        this.dataSource.sort = this.sort;
    }

    //Getters for Page Forms
    get filterDisplayName() {
        return this.TemplateFilterForm.get('filterDisplayName');
    }
    get filterOwner() {
        return this.TemplateFilterForm.get('filterOwner');
    }
    get filterPageType() {
        return this.TemplateFilterForm.get('filterPageType');
    }
    get filterStatus() {
        return this.TemplateFilterForm.get('filterStatus');
    }
    get filterPublishedOnFromDate() {
        return this.TemplateFilterForm.get('filterPublishedOnFromDate');
    }
    get filterPublishedOnToDate() {
        return this.TemplateFilterForm.get('filterPublishedOnToDate');
    }
    //End getter methods

    ngOnInit() {
        this.getTemplates(null);
        this.getPageTypes(null);
        this.TemplateFilterForm = this.fb.group({
            filterDisplayName: [null],
            filterOwner: [null],
            filterStatus: [null],
            filterPageType: [null],
            filterPublishedOnFromDate: [null],
            filterPublishedOnToDate: [null],
        });
    }

    sortData(sort: MatSort) {
        const data = this.templateList.slice();
        if (!sort.active || sort.direction === '') {
            this.sortedTemplateList = data;
            return;
        }

        this.sortedTemplateList = data.sort((a, b) => {
            const isAsc = sort.direction === 'asc';
            switch (sort.active) {
                case 'name': return compareStr(a.DisplayName, b.DisplayName, isAsc);
                case 'status': return compareStr(a.Status, b.Status, isAsc);
                case 'pagetype': return compareStr(a.PageTypeName, b.PageTypeName, isAsc);
                case 'owner': return compareStr(a.PageOwnerName, b.PageOwnerName, isAsc);
                case 'publishedBy': return compareStr(a.PagePublishedByUserName, b.PagePublishedByUserName, isAsc);
                case 'version' : return compare(Number(a.Version), Number(b.Version), isAsc); 
                //case 'date': return compareDate(a.PublishedOn, b.PublishedOn, isAsc);
                default: return 0;
            }
        });
        this.dataSource = new MatTableDataSource<Template>(this.sortedTemplateList);
        this.dataSource.sort = this.sort;
        this.array = this.sortedTemplateList;
        this.totalSize = this.array.length;
        this.iterator();
    }

    async getTemplates(searchParameter) {
        let templateService = this.injector.get(TemplateService);
        if (searchParameter == null) {
            searchParameter = {};
            searchParameter.IsActive = true;
            searchParameter.PagingParameter = {};
            searchParameter.PagingParameter.PageIndex = Constants.DefaultPageIndex;
            searchParameter.PagingParameter.PageSize = Constants.DefaultPageSize;
            searchParameter.SortParameter = {};
            searchParameter.SortParameter.SortColumn = 'CreatedDate';
            searchParameter.SortParameter.SortOrder = Constants.Descending;
            searchParameter.SearchMode = Constants.Contains;
        }
        this.templateList = await templateService.getTemplates(searchParameter);
        if (this.templateList.length == 0 && this.isFilterDone == true){
            let message = ErrorMessageConstants.getNoRecordFoundMessage;
            this._messageDialogService.openDialogBox('Error', message, Constants.msgBoxError).subscribe(data => {
                if (data == true) {
                    //this.resetRoleFilterForm();
                    this.getTemplates(null);
                }
            });
        }
        this.dataSource = new MatTableDataSource<Template>(this.templateList);
        this.dataSource.sort = this.sort;
        this.array = this.templateList;
        this.totalSize = this.array.length;
        this.iterator();
    }

    async getPageTypes(searchParameter) {
        this.pageTypeList = [ {"Id": 1, "Name": "Home"}, {"Id": 2, "Name": "Saving Account"}, {"Id": 3, "Name": "Current Account"} ];
    }

    validateFilterDate(): boolean {
        if(this.TemplateFilterForm.value.filterPublishedOnFromDate != null && this.TemplateFilterForm.value.filterPublishedOnFromDate != '' && 
            this.TemplateFilterForm.value.filterPublishedOnToDate != null && this.TemplateFilterForm.value.filterPublishedOnToDate != '') {
                let startDate = this.TemplateFilterForm.value.filterPublishedOnFromDate;
                let toDate = this.TemplateFilterForm.value.filterPublishedOnToDate;
                if(startDate.getTime() > toDate.getTime()) {
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
        if(this.TemplateFilterForm.value.filterPublishedOnFromDate != null && this.TemplateFilterForm.value.filterPublishedOnFromDate != '') {
            let startDate = this.TemplateFilterForm.value.filterPublishedOnFromDate;
            if(startDate.getTime() > currentDte.getTime()) {
                this.filterFromDateError = true;
                this.filterFromDateErrorMessage = ErrorMessageConstants.getStartDateLessThanCurrentDateMessage;
            }
        }
        if(this.TemplateFilterForm.value.filterPublishedOnToDate != null && this.TemplateFilterForm.value.filterPublishedOnToDate != '') {
            let toDate = this.TemplateFilterForm.value.filterPublishedOnToDate;
            if(toDate.getTime() > currentDte.getTime()) {
                this.filterToDateError = true;
                this.filterToDateErrorMessage = ErrorMessageConstants.getEndDateLessThanCurrentDateMessage;
            }
        }
        if(this.TemplateFilterForm.value.filterPublishedOnFromDate != null && this.TemplateFilterForm.value.filterPublishedOnFromDate != '' && 
            this.TemplateFilterForm.value.filterPublishedOnToDate != null && this.TemplateFilterForm.value.filterPublishedOnToDate != '') {
                let startDate = this.TemplateFilterForm.value.filterPublishedOnFromDate;
                let toDate = this.TemplateFilterForm.value.filterPublishedOnToDate;
                if(startDate.getTime() > toDate.getTime()) {
                    this.filterFromDateError = true;
                    this.filterFromDateErrorMessage = ErrorMessageConstants.getStartDateLessThanEndDateMessage;
                }
        }
    }

    //This method has been used for fetching search records
    searchTemplateRecordFilter(searchType) {
        this.filterFromDateError = false;
        this.isFilterDone = true;
        if (searchType == 'reset') {
            this.resetPageFilterForm();
            this.getTemplates(null);
            this.isFilter = !this.isFilter;
        }
        else {
            if(this.validateFilterDate()) {
                let searchParameter: any = {};
                searchParameter.PagingParameter = {};
                searchParameter.PagingParameter.PageIndex = Constants.DefaultPageIndex;
                searchParameter.PagingParameter.PageSize = Constants.DefaultPageSize;
                searchParameter.SortParameter = {};
                searchParameter.SortParameter.SortColumn = 'DisplayName';
                searchParameter.SortParameter.SortOrder = Constants.Ascending;
                searchParameter.SearchMode = Constants.Contains;
                if(this.TemplateFilterForm.value.filterDisplayName != null && this.TemplateFilterForm.value.filterDisplayName != '') {
                    searchParameter.DisplayName = this.TemplateFilterForm.value.filterDisplayName.trim();         
                } 
                if(this.TemplateFilterForm.value.filterOwner != null && this.TemplateFilterForm.value.filterOwner != '') {
                    searchParameter.PageOwner = this.TemplateFilterForm.value.filterOwner.trim();         
                }
                if(this.filterPageTypeId != 0) {
                    searchParameter.PageTypeId = this.filterPageTypeId;
                }
                if(this.TemplateFilterForm.value.filterStatus != null && this.TemplateFilterForm.value.filterStatus != 0) {
                    searchParameter.Status = this.TemplateFilterForm.value.filterStatus;
                }
                if(this.TemplateFilterForm.value.filterPublishedOnFromDate != null && this.TemplateFilterForm.value.filterPublishedOnFromDate != '') {
                    searchParameter.StartDate = this.TemplateFilterForm.value.filterPublishedOnFromDate;
                }
                if(this.TemplateFilterForm.value.filterPublishedOnToDate != null && this.TemplateFilterForm.value.filterPublishedOnToDate != '') {
                    searchParameter.EndDate = this.TemplateFilterForm.value.filterPublishedOnToDate;
                }
                this.currentPage = 0;
                this.getTemplates(searchParameter);
                this.isFilter = !this.isFilter;
            }
        }
    }

    resetPageFilterForm() {
        this.TemplateFilterForm.patchValue({
            filterDisplayName: null,
            filterOwner: null,
            filterPageType: 0,
            filterStatus: 0,
            filterPublishedOnFromDate: null,
            filterPublishedOnToDate: null
        });

        this.filterFromDateError = false;
        this.filterToDateError = false;
        this.filterFromDateErrorMessage = "";
        this.filterToDateErrorMessage = "";
    }

    closeFilter() {
        this.isFilter = !this.isFilter;
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

    navigationTodashboardDesigner(template: Template) {
        let queryParams = {
            Routeparams: {
                passingparams: {
                "PageName": template.DisplayName,
                "PageIdentifier": template.Identifier,
                }
            }
        }
        localStorage.setItem("pageDesignViewRouteparams", JSON.stringify(queryParams))
        this.route.navigate(['../dashboardDesignerView']);
    }

    async DeletePage(template: Template) {
        let message = "Are you sure, you want to delete this record?";
        this._messageDialogService.openConfirmationDialogBox('Confirm', message, Constants.msgBoxWarning).subscribe(async (isConfirmed) => {
            if (isConfirmed) {
                let pageData = [{
                    "Identifier": template.Identifier,
                }];
                
                let resultFlag = await this.templateService.deletePage(pageData);
                if (resultFlag) {
                    let messageString = Constants.recordDeletedMessage;
                    this._messageDialogService.openDialogBox('Success', messageString, Constants.msgBoxSuccess);
                    this.resetPageFilterForm();
                    this.getTemplates(null);
                }
            }
        });
    }

    async PublishPage(template: Template) {
        let message = "Are you sure, you want to publish this record?";
        this._messageDialogService.openConfirmationDialogBox('Confirm', message, Constants.msgBoxWarning).subscribe(async (isConfirmed) => {
            if (isConfirmed) {
                let pageData = [{
                    "Identifier": template.Identifier,
                }];
                let resultFlag = await this.templateService.publishPage(pageData);
                if (resultFlag) {
                    let messageString = Constants.PagePublishedSuccessfullyMessage;
                    this._messageDialogService.openDialogBox('Success', messageString, Constants.msgBoxSuccess);
                    this.resetPageFilterForm();
                    this.getTemplates(null);
                }
            }
        });
    }

    async ClonePage(template: Template) {
        let message = "Are you sure, you want to clone this record?";
        this._messageDialogService.openConfirmationDialogBox('Confirm', message, Constants.msgBoxWarning).subscribe(async (isConfirmed) => {
            if (isConfirmed) {
                let pageData = [{
                    "Identifier": template.Identifier,
                }];
                let resultFlag = await this.templateService.clonePage(pageData);
                if (resultFlag) {
                    let messageString = Constants.PageCloneSuccessfullyMessage;
                    this._messageDialogService.openDialogBox('Success', messageString, Constants.msgBoxSuccess);
                    this.resetPageFilterForm();
                    this.getTemplates(null);
                }
            }
        });
    }

    navigationToDashboardDesignerEdit(template: Template) {
        let queryParams = {
            Routeparams: {
                passingparams: {
                "PageName": template.DisplayName,
                "PageIdentifier": template.Identifier,
                "pageEditModeOn": true
                }
            }
        }
        localStorage.setItem("pageDesignEditRouteparams", JSON.stringify(queryParams))
        this.route.navigate(['../dashboardDesigner']);
    }
    
}

function compare(a: number, b: number, isAsc: boolean) {
    return (a < b ? -1 : 1) * (isAsc ? 1 : -1);
  }

  function compareStr(a: string, b: string, isAsc: boolean) {
    return (a.toLowerCase() < b.toLowerCase() ? -1 : 1) * (isAsc ? 1 : -1);
  }
  
  function compareDate(a: Date, b: Date, isAsc: boolean) {
    return (a.getTime() < b.getTime() ? -1 : 1) * (isAsc ? 1 : -1);
  }
