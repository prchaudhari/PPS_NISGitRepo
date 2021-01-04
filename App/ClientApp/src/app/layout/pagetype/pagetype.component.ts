import { Component, OnInit, Injector, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { NgxUiLoaderService } from 'ngx-ui-loader';
import { Router } from '@angular/router';
import { Constants } from '../../shared/constants/constants';
import { ErrorMessageConstants } from '../../shared/constants/constants';
import { MessageDialogService } from '../../shared/services/mesage-dialog.service';
import { LocalStorageService } from '../../shared/services/local-storage.service';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { PageType } from './pagetype';
import { PageTypeService } from './pagetype.service';

@Component({
  selector: 'app-pagetype',
  templateUrl: './pagetype.component.html',
  styleUrls: ['./pagetype.component.scss']
})
export class PagetypeComponent implements OnInit {
  public isFilter: boolean = false;
  public pagetypeList: PageType[] = [];
  public isLoaderActive: boolean = false;
  public isRecordFound: boolean = false;
  public pageNo = 0;
  public pageSize = 5;
  public currentPage = 0;
  public totalSize = 0;
  public array: any;
  public isFilterDone = false;
  public sortedPageTypeList: PageType[] = [];
  public pageTypeList: any[] = [];
  public PageTypeFilterForm: FormGroup;

  public userClaimsRolePrivilegeOperations: any[] = [];
  public onlyAlphabetsWithSpace = '[a-zA-Z ]*';
  public dialingCodeRegex = '^(\\+\\d{1,3})$';
  public onlyAlphabetswithInbetweenSpaceUpto50Characters = Constants.onlyAlphabetswithInbetweenSpaceUpto50Characters;
  public totalRecordCount = 0;
  public filterName1 = '';
  public sortOrder = Constants.Descending;
  public sortColumn = 'Name';
  public AddPageTypeFormGroup: FormGroup;
  public EditPageTypeFormGroup: FormGroup;
  public addPageTypeContainer: boolean;
  public isAddPageType: boolean = false;
  public editPageTypeContainer: boolean;
  public isEditPageType: boolean = false;

  public pagetypeId: number = 0;
  displayedColumns: string[] = ['name', 'code', 'actions'];
  dataSource = new MatTableDataSource<any>();

  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;

  closeFilter() {
    this.isFilter = !this.isFilter;
  }

  ngOnInit() {

    //var userClaimsDetail = JSON.parse(localStorage.getItem('userClaims'));
    //if (userClaimsDetail) {
    //  if (userClaimsDetail.IsTenantGroupManager == null || userClaimsDetail.IsTenantGroupManager.toLocaleLowerCase() != 'true') {
    //    this.localstorageservice.removeLocalStorageData();
    //    this.route.navigate(['login']);
    //  }
    //  this.userClaimsRolePrivilegeOperations = userClaimsDetail.Privileges;
    //}
    //else {
    //  this.localstorageservice.removeLocalStorageData();
    //  this.route.navigate(['login']);
    //}

    this.getPageTypes(null);
    this.PageTypeFilterForm = this.fb.group({
      filterName: [null]
    });

    this.AddPageTypeFormGroup = this.fb.group({
      AddName: [null, Validators.compose([Validators.required,
      Validators.minLength(Constants.inputMinLenth), Validators.maxLength(Constants.inputMaxLenth),
      Validators.pattern(this.onlyAlphabetsWithSpace)])
      ],
      AddDescription: ['', Validators.compose([Validators.maxLength(500)])],
    });

    this.EditPageTypeFormGroup = this.fb.group({
      EditName: [null, Validators.compose([Validators.required,
      Validators.minLength(Constants.inputMinLenth), Validators.maxLength(Constants.inputMaxLenth),
      Validators.pattern(this.onlyAlphabetsWithSpace)])
      ],
      EditDescription: ['', Validators.compose([Validators.maxLength(500)])],

    });

  }

  get AddName() {
    return this.AddPageTypeFormGroup.get('AddName');
  }

  get AddDescription() {
    return this.AddPageTypeFormGroup.get('AddDescription');
  }
  get EditName() {
    return this.EditPageTypeFormGroup.get('EditName');
  }

  get EditDescription() {
    return this.EditPageTypeFormGroup.get('EditDescription');
  }

  ngAfterViewInit() {
    this.dataSource.paginator = this.paginator;
  }

  constructor(private injector: Injector,
    private fb: FormBuilder,
    private uiLoader: NgxUiLoaderService,
    private _messageDialogService: MessageDialogService,
    private route: Router,
    private localstorageservice: LocalStorageService,
    private pagetypeService: PageTypeService) {
    this.addPageTypeContainer = false;
    this.sortedPageTypeList = this.pagetypeList.slice();
  }

  public handlePage(e: any) {
    this.currentPage = e.pageIndex;
    this.pageSize = e.pageSize;
    //this.iterator();
    this.getPageTypes(null);
  }

  //Getters for Page Forms
  get filterName() {
    return this.PageTypeFilterForm.get('filterName');
  }

  sortData(sort: MatSort) {
    const data = this.pagetypeList.slice();
    if (!sort.active || sort.direction === '') {
      this.sortedPageTypeList = data;
      return;
    }

    if (sort.direction == 'asc') {
      this.sortOrder = Constants.Ascending;
    } else {
      this.sortOrder = Constants.Descending;
    }
    //['name', 'code', 'dialingcode', 'actions'];
    switch (sort.active) {
      case 'name': this.sortColumn = "Name"; break;
      case 'code': this.sortColumn = "Description"; break;
      default: this.sortColumn = "Name"; break;
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
    this.getPageTypes(searchParameter);
  }

  async getPageTypes(searchParameter) {
    let pagetypeService = this.injector.get(PageTypeService);
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
    if (this.filterName1 != null && this.filterName1 != '') {
      searchParameter.PageTypeName = this.filterName1.trim();
    }

    var response = await pagetypeService.getPageTypes(searchParameter);
    this.pagetypeList = response.List;
    this.totalRecordCount = response.RecordCount;
    if (this.pagetypeList.length == 0 && this.isFilterDone == true) {
      let message = ErrorMessageConstants.getNoRecordFoundMessage;
      this._messageDialogService.openDialogBox('Error', message, Constants.msgBoxError).subscribe(data => {
        if (data == true) {
          this.resetPageTypeFilterForm();
          this.getPageTypes(null);
        }
      });
    }
    this.dataSource = new MatTableDataSource<PageType>(this.pagetypeList);
    this.dataSource.sort = this.sort;
    this.array = this.pagetypeList;
    this.totalSize = this.totalRecordCount;
    //this.iterator();
  }

  //This method has been used for fetching search records
  searchPageTypeFilter(searchType) {
    this.isFilterDone = true;
    if (searchType == 'reset') {
      this.resetPageTypeFilterForm();
      this.getPageTypes(null);
      this.isFilter = !this.isFilter;
    }
    else {
      let searchParameter: any = {};
      searchParameter.PagingParameter = {};
      searchParameter.PagingParameter.PageIndex = 1;
      searchParameter.PagingParameter.PageSize = this.pageSize;
      searchParameter.SortParameter = {};
      searchParameter.SortParameter.SortColumn = this.sortColumn;
      searchParameter.SortParameter.SortOrder = this.sortOrder;
      searchParameter.SearchMode = Constants.Contains;
      if (this.PageTypeFilterForm.value.filterName != null && this.PageTypeFilterForm.value.filterName != '') {
        this.filterName1 = this.PageTypeFilterForm.value.filterName.trim();
        searchParameter.Name = this.PageTypeFilterForm.value.filterName.trim();
      }

      this.currentPage = 0;
      this.getPageTypes(searchParameter);
      this.isFilter = !this.isFilter;
    }
  }

  resetPageTypeFilterForm() {
    this.PageTypeFilterForm.patchValue({
      filterName: null
    });

    this.currentPage = 0;
    this.filterName1 = '';
  }

  //this method helps to navigate to add
  navigateToPageTypeAdd() {
    this.pagetypeId = 0;
    this.addPageTypeContainer = true;
  }

  //this method helps to navigate edit
  navigateToPageTypeEdit(pagetype) {
    this.pagetypeId = pagetype.Identifier;
    this.editPageTypeContainer = true;
    this.EditPageTypeFormGroup.controls['EditName'].setValue(pagetype.PageTypeName);
    this.EditPageTypeFormGroup.controls['EditDescription'].setValue(pagetype.Description);
  }

  //function written to delete contact type
  deletePageType(pagetype: PageType) {
    let message = 'Are you sure, you want to delete this record?';
    this._messageDialogService.openConfirmationDialogBox('Confirm', message, Constants.msgBoxWarning).subscribe(async (isConfirmed) => {
      if (isConfirmed) {
        let data = [{
          "Identifier": pagetype.Identifier,
        }];
        let isDeleted = await this.pagetypeService.deletePageType(data);
        if (isDeleted) {
          let messageString = Constants.recordDeletedMessage;
          this._messageDialogService.openDialogBox('Success', messageString, Constants.msgBoxSuccess);
          this.getPageTypes(null);
        }
      }
    });
  }

  async AddUpdatePageType() {
    var pagetype: any = {};
    var isEditOperation = false;
    if (this.pagetypeId > 0) {
      isEditOperation = true;
      pagetype = {
        "PageTypeName": this.EditPageTypeFormGroup.value.EditName,
        "Description": this.EditPageTypeFormGroup.value.EditDescription,
        "Identifier": this.pagetypeId
      }
    }
    else {
      isEditOperation = false;
      pagetype = {
        "PageTypeName": this.AddPageTypeFormGroup.value.AddName,
        "Description": this.AddPageTypeFormGroup.value.AddDescription,
        "Identifier": 0
      }
    }
    var data = [];
    data.push(pagetype);
    let pagetypeService = this.injector.get(PageTypeService);
    let isRecordSaved = await pagetypeService.savePageType(data, isEditOperation);
    this.uiLoader.stop();
    if (isRecordSaved) {
      let message = Constants.recordAddedMessage;
      if (isEditOperation) {
        this.ClosePageTypeForm('Edit');
        message = Constants.recordUpdatedMessage;
      }
      else {
        this.ClosePageTypeForm('Add');
      }
      this._messageDialogService.openDialogBox('Success', message, Constants.msgBoxSuccess);
      this.getPageTypes(null);
    }
  }

  vaildateForm(form) {
    if (form == 'Add') {
      if (this.AddPageTypeFormGroup.invalid) {
        return true;
      }
      return false;
    }
    else if (form == 'Edit') {
      if (this.EditPageTypeFormGroup.invalid) {
        return true;
      }
      return false;
    }
  }

  ClosePageTypeForm(form) {
    if (form == 'Add') {
      this.addPageTypeContainer = false;
      this.AddPageTypeFormGroup.controls['AddName'].setValue('');
      this.AddPageTypeFormGroup.controls['AddDescription'].setValue('');
      this.AddPageTypeFormGroup.reset();
    }
    if (form == 'Edit') {
      this.editPageTypeContainer = false;
      this.EditPageTypeFormGroup.controls['EditName'].setValue('');
      this.EditPageTypeFormGroup.controls['EditDescription'].setValue('');
    }
  }
}
