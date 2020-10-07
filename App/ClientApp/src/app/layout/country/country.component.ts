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
import { Country } from './country';
import { CountryService } from './country.service';

@Component({
  selector: 'app-country',
  templateUrl: './country.component.html',
  styleUrls: ['./country.component.scss']
})
export class CountryComponent implements OnInit {
  public isFilter: boolean = false;
  public countryList: Country[] = [];
  public isLoaderActive: boolean = false;
  public isRecordFound: boolean = false;
  public pageNo = 0;
  public pageSize = 5;
  public currentPage = 0;
  public totalSize = 0;
  public array: any;
  public isFilterDone = false;
  public sortedCountryList: Country[] = [];
  public pageTypeList: any[] = [];
  public CountryFilterForm: FormGroup;

  public userClaimsRolePrivilegeOperations: any[] = [];
  public onlyAlphabetsWithSpace = '[a-zA-Z ]*';
  public dialingCodeRegex = '^(\\+\\d{1,3})$';
  public onlyAlphabetswithInbetweenSpaceUpto50Characters = Constants.onlyAlphabetswithInbetweenSpaceUpto50Characters;
  public totalRecordCount = 0;
  public filterName1 = '';
  public filterCode1 = '';
  public filterDialingCode1 = '';
  public sortOrder = Constants.Descending;
  public sortColumn = 'Name';
  public AddCountryFormGroup: FormGroup;
  public EditCountryFormGroup: FormGroup;
  public addCountryContainer: boolean;
  public isAddCountry: boolean = false;
  public editCountryContainer: boolean;
  public isEditCountry: boolean = false;
  closeFilter() {
    this.isFilter = !this.isFilter;
  }
  public countryId: number = 0;
  displayedColumns: string[] = ['name', 'code', 'dialingcode', 'actions'];
  dataSource = new MatTableDataSource<any>();

  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;

  ngOnInit() {

    var userClaimsDetail = JSON.parse(localStorage.getItem('userClaims'));
    if (userClaimsDetail) {
      var userdata = this.localstorageservice.GetCurrentUser();
      if(userdata != null && userdata.RoleName == 'Tenant Admin') {
        this.userClaimsRolePrivilegeOperations = userClaimsDetail.Privileges;
      }
      else if(userClaimsDetail.IsTenantGroupManager != null && userClaimsDetail.IsTenantGroupManager.toLocaleLowerCase() == 'true') {
        this.userClaimsRolePrivilegeOperations = userClaimsDetail.Privileges;
      }
      else {
        this.localstorageservice.removeLocalStorageData();
        this.route.navigate(['login']);
      }
    }
    else {
      this.localstorageservice.removeLocalStorageData();
      this.route.navigate(['login']);
    }

    this.getCountrys(null);
    this.CountryFilterForm = this.fb.group({
      filterName: [null],
      filterCode: [null]
    });
    this.AddCountryFormGroup = this.fb.group({
      AddName: [null, Validators.compose([Validators.required,
      Validators.minLength(Constants.inputMinLenth), Validators.maxLength(Constants.inputMaxLenth),
        Validators.pattern(this.onlyAlphabetsWithSpace)])
      ],
      AddCode: [null, Validators.compose([Validators.required,
      Validators.minLength(Constants.inputMinLenth), Validators.maxLength(Constants.inputMaxLenth),
        Validators.pattern(this.onlyAlphabetsWithSpace)])
      ],
      AddDialingCode: [null, Validators.compose([Validators.required,
        Validators.pattern(this.dialingCodeRegex)
      ])]
    });

    this.EditCountryFormGroup = this.fb.group({
      EditName: [null, Validators.compose([Validators.required,
      Validators.minLength(Constants.inputMinLenth), Validators.maxLength(Constants.inputMaxLenth),
      Validators.pattern(this.onlyAlphabetsWithSpace)])
      ],
      EditCode: [null, Validators.compose([Validators.required,
      Validators.minLength(Constants.inputMinLenth), Validators.maxLength(Constants.inputMaxLenth),
      Validators.pattern(this.onlyAlphabetsWithSpace)])
      ],
      EditDialingCode: [null, Validators.compose([Validators.required,
      Validators.pattern(this.dialingCodeRegex)
      ])]
    });
    
  }
  get AddName() {
    return this.AddCountryFormGroup.get('AddName');
  }

  get AddCode() {
    return this.AddCountryFormGroup.get('AddCode');
  }
  get AddDialingCode() {
    return this.AddCountryFormGroup.get('AddDialingCode');
  }

  get EditName() {
    return this.EditCountryFormGroup.get('EditName');
  }

  get EditCode() {
    return this.EditCountryFormGroup.get('EditCode');
  }
  get EditDialingCode() {
    return this.EditCountryFormGroup.get('EditDialingCode');
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
    private countryService: CountryService) {
    this.addCountryContainer = false;
    this.sortedCountryList = this.countryList.slice();
  }


  public handlePage(e: any) {
    this.currentPage = e.pageIndex;
    this.pageSize = e.pageSize;
    //this.iterator();
    this.getCountrys(null);
  }

  //Getters for Page Forms
  get filterName() {
    return this.CountryFilterForm.get('filterName');
  }

  get filterCode() {
    return this.CountryFilterForm.get('filterCode');
  }

  get filterDialingCode() {
    return this.CountryFilterForm.get('filterDialingCode');
  }

  sortData(sort: MatSort) {
    const data = this.countryList.slice();
    if (!sort.active || sort.direction === '') {
      this.sortedCountryList = data;
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
      case 'code': this.sortColumn = "Code"; break;
      case 'dialingcode': this.sortColumn = "Dialingcode"; break;
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
    this.getCountrys(searchParameter);
  }

  async getCountrys(searchParameter) {
    let countryService = this.injector.get(CountryService);
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
      searchParameter.CountryName = this.filterName1.trim();
    }
    if (this.filterCode1 != null && this.filterCode1 != '') {
      searchParameter.CountryCode = this.filterCode1.trim();
    }

    var response = await countryService.getCountrys(searchParameter);
    this.countryList = response.List;
    this.totalRecordCount = response.RecordCount;
    if (this.countryList.length == 0 && this.isFilterDone == true) {
      let message = ErrorMessageConstants.getNoRecordFoundMessage;
      this._messageDialogService.openDialogBox('Error', message, Constants.msgBoxError).subscribe(data => {
        if (data == true) {
          this.resetCountryFilterForm();
          this.getCountrys(null);
        }
      });
    }
    this.dataSource = new MatTableDataSource<Country>(this.countryList);
    this.dataSource.sort = this.sort;
    this.array = this.countryList;
    this.totalSize = this.totalRecordCount;
    //this.iterator();
  }

  //This method has been used for fetching search records
  searchCountryFilter(searchType) {
    this.isFilterDone = true;
    if (searchType == 'reset') {
      this.resetCountryFilterForm();
      this.getCountrys(null);
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
      if (this.CountryFilterForm.value.filterName != null && this.CountryFilterForm.value.filterName != '') {
        this.filterName1 = this.CountryFilterForm.value.filterName.trim();
        searchParameter.Name = this.CountryFilterForm.value.filterName.trim();
      }
      if (this.CountryFilterForm.value.filterCode != null && this.CountryFilterForm.value.filterCode != '') {
        this.filterCode1 = this.CountryFilterForm.value.filterCode.trim();
        searchParameter.CountryOwner = this.CountryFilterForm.value.filterCode.trim();
      }
      if (this.CountryFilterForm.value.filterDialingCode != null && this.CountryFilterForm.value.filterDialingCode != 0) {
        this.filterDialingCode1 = this.CountryFilterForm.value.filterDialingCode;
        searchParameter.Status = this.CountryFilterForm.value.filterDialingCode;
      }

      this.currentPage = 0;
      this.getCountrys(searchParameter);
      this.isFilter = !this.isFilter;
    }
  }

  resetCountryFilterForm() {
    this.CountryFilterForm.patchValue({
      filterName: null,
      filterCode: null,
      filterDialingCode: null,
    });

    this.currentPage = 0;
    this.filterName1 = '';
    this.filterDialingCode1 = '';
    this.filterCode1 = '';
  }

  //this method helps to navigate to add
  navigateToCountryAdd() {
    this.countryId = 0;
    this.addCountryContainer = true;
  }

  //this method helps to navigate edit
  navigateToCountryEdit(country) {
    this.countryId = country.Identifier;
    this.editCountryContainer = true;
    this.EditCountryFormGroup.controls['EditName'].setValue(country.CountryName);
    this.EditCountryFormGroup.controls['EditCode'].setValue(country.Code);
    this.EditCountryFormGroup.controls['EditDialingCode'].setValue(country.DialingCode);
  }

  //function written to delete role
  deleteCountry(role: Country) {
    let message = 'Are you sure, you want to delete this record?';
    this._messageDialogService.openConfirmationDialogBox('Confirm', message, Constants.msgBoxWarning).subscribe(async (isConfirmed) => {
      if (isConfirmed) {
        let roleData = [{
          "Identifier": role.Identifier,
        }];

        let isDeleted = await this.countryService.deleteCountry(roleData);
        if (isDeleted) {
          let messageString = Constants.recordDeletedMessage;
          this._messageDialogService.openDialogBox('Success', messageString, Constants.msgBoxSuccess);
          this.getCountrys(null);

        }
      }
    });
  }

  async AddUpdateCountry() {
    var country: any = {};
    var isEditOperation = false;
    if (this.countryId > 0) {
      isEditOperation = true;
      country = {
        "CountryName": this.EditCountryFormGroup.value.EditName,
        "Code": this.EditCountryFormGroup.value.EditCode,
        "DialingCode": this.EditCountryFormGroup.value.EditDialingCode,
        "Identifier": this.countryId
      }
    }
    else {
      isEditOperation = false;
      country = {
        "CountryName": this.AddCountryFormGroup.value.AddName,
        "Code": this.AddCountryFormGroup.value.AddCode,
        "DialingCode": this.AddCountryFormGroup.value.AddDialingCode,
        "Identifier": 0
      }
    }
    var data = [];
    data.push(country);
    let countryService = this.injector.get(CountryService);
    let isRecordSaved = await countryService.saveCountry(data, isEditOperation);
    this.uiLoader.stop();
    if (isRecordSaved) {
      let message = Constants.recordAddedMessage;
      if (isEditOperation) {
        this.CloseCountryForm('Edit');
        message = Constants.recordUpdatedMessage;
      }
      else {
        this.CloseCountryForm('Add');
      }
      this._messageDialogService.openDialogBox('Success', message, Constants.msgBoxSuccess);
      this.getCountrys(null);
    }
  }

  vaildateForm(form) {
    if (form == 'Add') {
      if (this.AddCountryFormGroup.invalid) {
        return true;

      }
      return false;
    }
    else if (form == 'Edit') {
      if (this.EditCountryFormGroup.invalid) {
        return true;

      }
      return false;
    }
  }

  CloseCountryForm(form) {
    if (form == 'Add') {
      this.addCountryContainer = false;
      this.AddCountryFormGroup.controls['AddName'].setValue('');
      this.AddCountryFormGroup.controls['AddCode'].setValue('');
      this.AddCountryFormGroup.controls['AddDialingCode'].setValue('');
      this.AddCountryFormGroup.reset();
    }
    if (form == 'Edit') {
      this.editCountryContainer = false;
      this.EditCountryFormGroup.controls['EditName'].setValue('');
      this.EditCountryFormGroup.controls['EditCode'].setValue('');
      this.EditCountryFormGroup.controls['EditDialingCode'].setValue('');
    }
  }
}
