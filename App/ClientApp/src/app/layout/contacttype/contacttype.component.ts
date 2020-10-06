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
import { ContactType } from './contacttype';
import { ContactTypeService } from './contacttype.service';

@Component({
  selector: 'app-contacttype',
  templateUrl: './contacttype.component.html',
  styleUrls: ['./contacttype.component.scss']
})
export class ContacttypeComponent implements OnInit {
  public isFilter: boolean = false;
  public contacttypeList: ContactType[] = [];
  public isLoaderActive: boolean = false;
  public isRecordFound: boolean = false;
  public pageNo = 0;
  public pageSize = 5;
  public currentPage = 0;
  public totalSize = 0;
  public array: any;
  public isFilterDone = false;
  public sortedContactTypeList: ContactType[] = [];
  public pageTypeList: any[] = [];
  public ContactTypeFilterForm: FormGroup;

  public userClaimsRolePrivilegeOperations: any[] = [];
  public onlyAlphabetsWithSpace = '[a-zA-Z ]*';
  public dialingCodeRegex = '^(\\+\\d{1,3})$';
  public onlyAlphabetswithInbetweenSpaceUpto50Characters = Constants.onlyAlphabetswithInbetweenSpaceUpto50Characters;
  public totalRecordCount = 0;
  public filterName1 = '';
  public sortOrder = Constants.Descending;
  public sortColumn = 'Name';
  public AddContactTypeFormGroup: FormGroup;
  public EditContactTypeFormGroup: FormGroup;
  public addContactTypeContainer: boolean;
  public isAddContactType: boolean = false;
  public editContactTypeContainer: boolean;
  public isEditContactType: boolean = false;
  closeFilter() {
    this.isFilter = !this.isFilter;
  }
  public contacttypeId: number = 0;
  displayedColumns: string[] = ['name', 'code', 'actions'];
  dataSource = new MatTableDataSource<any>();

  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;

  ngOnInit() {
    this.getContactTypes(null);
    this.ContactTypeFilterForm = this.fb.group({
      filterName: [null]
    });

    this.AddContactTypeFormGroup = this.fb.group({
      AddName: [null, Validators.compose([Validators.required,
      Validators.minLength(Constants.inputMinLenth), Validators.maxLength(Constants.inputMaxLenth),
      Validators.pattern(this.onlyAlphabetsWithSpace)])
      ],
      AddDescription: ['', Validators.compose([Validators.maxLength(500)])],
    });

    this.EditContactTypeFormGroup = this.fb.group({
      EditName: [null, Validators.compose([Validators.required,
      Validators.minLength(Constants.inputMinLenth), Validators.maxLength(Constants.inputMaxLenth),
      Validators.pattern(this.onlyAlphabetsWithSpace)])
      ],
      EditDescription: ['', Validators.compose([Validators.maxLength(500)])],

    });
    var userClaimsDetail = JSON.parse(localStorage.getItem('userClaims'));
    if (userClaimsDetail) {
      this.userClaimsRolePrivilegeOperations = userClaimsDetail.Privileges;
    }
    else {
      this.userClaimsRolePrivilegeOperations = [];
    }
  }
  get AddName() {
    return this.AddContactTypeFormGroup.get('AddName');
  }

  get AddDescription() {
    return this.AddContactTypeFormGroup.get('AddDescription');
  }
  get EditName() {
    return this.EditContactTypeFormGroup.get('EditName');
  }

  get EditDescription() {
    return this.EditContactTypeFormGroup.get('EditDescription');
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
    private contacttypeService: ContactTypeService) {
    this.addContactTypeContainer = false;
    this.sortedContactTypeList = this.contacttypeList.slice();
  }

  public handlePage(e: any) {
    this.currentPage = e.pageIndex;
    this.pageSize = e.pageSize;
    //this.iterator();
    this.getContactTypes(null);
  }

  //Getters for Page Forms
  get filterName() {
    return this.ContactTypeFilterForm.get('filterName');
  }

  sortData(sort: MatSort) {
    const data = this.contacttypeList.slice();
    if (!sort.active || sort.direction === '') {
      this.sortedContactTypeList = data;
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
    this.getContactTypes(searchParameter);
  }

  async getContactTypes(searchParameter) {
    let contacttypeService = this.injector.get(ContactTypeService);
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
      searchParameter.ContactTypeName = this.filterName1.trim();
    }

    var response = await contacttypeService.getContactTypes(searchParameter);
    this.contacttypeList = response.List;
    this.totalRecordCount = response.RecordCount;
    if (this.contacttypeList.length == 0 && this.isFilterDone == true) {
      let message = ErrorMessageConstants.getNoRecordFoundMessage;
      this._messageDialogService.openDialogBox('Error', message, Constants.msgBoxError).subscribe(data => {
        if (data == true) {
          this.resetContactTypeFilterForm();
          this.getContactTypes(null);
        }
      });
    }
    this.dataSource = new MatTableDataSource<ContactType>(this.contacttypeList);
    this.dataSource.sort = this.sort;
    this.array = this.contacttypeList;
    this.totalSize = this.totalRecordCount;
    //this.iterator();
  }

  //This method has been used for fetching search records
  searchContactTypeFilter(searchType) {
    this.isFilterDone = true;
    if (searchType == 'reset') {
      this.resetContactTypeFilterForm();
      this.getContactTypes(null);
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
      if (this.ContactTypeFilterForm.value.filterName != null && this.ContactTypeFilterForm.value.filterName != '') {
        this.filterName1 = this.ContactTypeFilterForm.value.filterName.trim();
        searchParameter.Name = this.ContactTypeFilterForm.value.filterName.trim();
      }

      this.currentPage = 0;
      this.getContactTypes(searchParameter);
      this.isFilter = !this.isFilter;
    }
  }

  resetContactTypeFilterForm() {
    this.ContactTypeFilterForm.patchValue({
      filterName: null
    });

    this.currentPage = 0;
    this.filterName1 = '';
  }

  //this method helps to navigate to add
  navigateToContactTypeAdd() {
    this.contacttypeId = 0;
    this.addContactTypeContainer = true;
  }

  //this method helps to navigate edit
  navigateToContactTypeEdit(contacttype) {
    this.contacttypeId = contacttype.Identifier;
    this.editContactTypeContainer = true;
    this.EditContactTypeFormGroup.controls['EditName'].setValue(contacttype.Name);
    this.EditContactTypeFormGroup.controls['EditDescription'].setValue(contacttype.Description);
  }

  //function written to delete role
  deleteContactType(role: ContactType) {
    let message = 'Are you sure, you want to delete this record?';
    this._messageDialogService.openConfirmationDialogBox('Confirm', message, Constants.msgBoxWarning).subscribe(async (isConfirmed) => {
      if (isConfirmed) {
        let data = [{
          "Identifier": role.Identifier,
        }];

        let isDeleted = await this.contacttypeService.deleteContactType(data);
        if (isDeleted) {
          let messageString = Constants.recordDeletedMessage;
          this._messageDialogService.openDialogBox('Success', messageString, Constants.msgBoxSuccess);
          this.getContactTypes(null);

        }
      }
    });
  }

  async AddUpdateContactType() {
    var contacttype: any = {};
    var isEditOperation = false;
    if (this.contacttypeId > 0) {
      isEditOperation = true;
      contacttype = {
        "Name": this.EditContactTypeFormGroup.value.EditName,
        "Description": this.EditContactTypeFormGroup.value.EditDescription,
        "Identifier": this.contacttypeId
      }
    }
    else {
      isEditOperation = false;
      contacttype = {
        "Name": this.AddContactTypeFormGroup.value.AddName,
        "Description": this.AddContactTypeFormGroup.value.AddDescription,
        "Identifier": 0
      }
    }
    var data = [];
    data.push(contacttype);
    let contacttypeService = this.injector.get(ContactTypeService);
    let isRecordSaved = await contacttypeService.saveContactType(data, isEditOperation);
    this.uiLoader.stop();
    if (isRecordSaved) {
      let message = Constants.recordAddedMessage;
      if (isEditOperation) {
        this.CloseContactTypeForm('Edit');
        message = Constants.recordUpdatedMessage;
      }
      else {
        this.CloseContactTypeForm('Add');
      }
      this._messageDialogService.openDialogBox('Success', message, Constants.msgBoxSuccess);
      this.getContactTypes(null);
    }
  }

  vaildateForm(form) {
    if (form == 'Add') {
      if (this.AddContactTypeFormGroup.invalid) {
        return true;
      }
      return false;
    }
    else if (form == 'Edit') {
      if (this.EditContactTypeFormGroup.invalid) {
        return true;
      }
      return false;
    }
  }

  CloseContactTypeForm(form) {
    if (form == 'Add') {
      this.addContactTypeContainer = false;
      this.AddContactTypeFormGroup.controls['AddName'].setValue('');
      this.AddContactTypeFormGroup.controls['AddDescription'].setValue('');
      this.AddContactTypeFormGroup.reset();
    }
    if (form == 'Edit') {
      this.editContactTypeContainer = false;
      this.EditContactTypeFormGroup.controls['EditName'].setValue('');
      this.EditContactTypeFormGroup.controls['EditDescription'].setValue('');
    }
  }
}
