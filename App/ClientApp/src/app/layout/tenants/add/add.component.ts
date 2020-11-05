import { Component, OnInit, Injector, ChangeDetectorRef, ViewChild, ElementRef, OnDestroy } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import * as $ from 'jquery';
import { Router, ActivatedRoute, NavigationEnd } from '@angular/router';
import { Tenant, TenantContact } from '../tenant';
import { NgxUiLoaderService } from 'ngx-ui-loader';
import { Location } from '@angular/common';
import { Constants } from 'src/app/shared/constants/constants';
import { MessageDialogService } from 'src/app/shared/services/mesage-dialog.service';
import { FormGroup, FormBuilder, Validators, FormControl, SelectControlValueAccessor, FormArray, ValidatorFn } from '@angular/forms';
import { TenantService } from '../tenant.service';
import { CdkDragDrop, moveItemInArray, transferArrayItem } from '@angular/cdk/drag-drop';
import { ErrorMessageConstants } from 'src/app/shared/constants/constants';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { Country } from '../../country/country';
import { CountryService } from '../../country/country.service';
import { ContactTypeService } from '../../contacttype/contacttype.service';
import { LocalStorageService } from 'src/app/shared/services/local-storage.service';
@Component({
  selector: 'app-add',
  templateUrl: './add.component.html',
  styleUrls: ['./add.component.scss']
})
export class AddComponent implements OnInit {
  public isCollapsedDetails: boolean = false;
  public isCollapsedAssets: boolean = true;
  public isContactContainer: boolean = false;
  public isEditContactContainer: boolean = false;
  public pageSize = 5;
  public currentPage = 0;
  public totalSize = 0;
  public contactlist: TenantContact[] = [];
  public contactTypeList: any[] = [];
  public array: any;
  FirstChar: string;
  SecondChar: string
  public tenantFormGroup: FormGroup;
  public contactFormGroup: FormGroup;
  public contactEditFormGroup: FormGroup;

  public updateOperationMode: boolean;
  public params: any = [];
  public tenant: Tenant;
  public isTenantDetailsLoaded = false;
  public emailRegex = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
  public onlyAlphabetsWithSpaceQuoteHyphen = "[a-zA-Z]+[ ]{0,1}[a-zA-Z]*[ ]*$";
  public onlyCharacterswithInbetweenSpaceUpto50Characters = Constants.onlyCharacterswithInbetweenSpaceUpto50Characters;
  public onlyNumbers = '[0-9]*';
  public contactFormErrorObject: any = {
    showContactFirstNameError: false,
    showContactLastNameError: false,
    showContactCodeError: false,
    showContactEmailError: false,
    showCountryCodeError: false,
    showContactMobileNumberError: false,
    showContactTypeError: false,
  };
  public tenantFormErrorObject: any = {
    showCountryError: false,
    showProfilePictureSizeError: false,
    showProfilePictureTypeError: false,
    showCountryCodeError: false
  };
  public imageSize: number;
  public imageType = "";
  public imageName = "";
  image: string;
  displayedColumns: string[] = ['ContactType', 'FirstName', 'LastName', 'EmailAddress', 'ContactNumber', 'actions'];
  public tenantContactIdentifier = 0;
  public Contact = { RoleIdentifier: 0, Image: '', CountryCode: 0, ContactType: 0 }
  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
  public countrycodeList = [];
  public countrycodeLists = [{ "Identifier": 0, "CountryName": "Please Select", "Code": "Please Select", "DialingCode": "" }];
  dataSource = new MatTableDataSource<TenantContact>(this.contactlist);
  public tenantAddressFieldError: boolean = false;
  public tenantCountryCode = 0;

  get tenantName() {
    return this.tenantFormGroup.get('tenantName');
  }

  get tenantCountry() {
    return this.tenantFormGroup.get('tenantCountry');
  }

  // get tenantDomainName() {
  //   return this.tenantFormGroup.get('tenantDomainName');
  // }

  get tenantAddress() {
    return this.tenantFormGroup.get('tenantAddress');
  }

  get tenantCity() {
    return this.tenantFormGroup.get('tenantCity');
  }

  get tenantState() {
    return this.tenantFormGroup.get('tenantState');
  }

  get tenantPostalCode() {
    return this.tenantFormGroup.get('tenantPostalCode');
  }

  get tenantDescription() {
    return this.tenantFormGroup.get('tenantDescription');
  }

  get firstName() {
    return this.contactFormGroup.get('firstName');
  }

  get lastName() {
    return this.contactFormGroup.get('lastName');
  }

  get email() {
    return this.contactFormGroup.get('email');
  }

  get countryCode() {
    return this.contactFormGroup.get('countryCode');
  }

  get mobileNumber() {
    return this.contactFormGroup.get('mobileNumber');
  }

  get ContactType() {
    return this.contactFormGroup.get('ContactType');
  }

  get EditfirstName() {
    return this.contactEditFormGroup.get('EditfirstName');
  }

  get EditlastName() {
    return this.contactEditFormGroup.get('EditlastName');
  }

  get Editemail() {
    return this.contactEditFormGroup.get('Editemail');
  }

  get EditcountryCode() {
    return this.contactEditFormGroup.get('EditcountryCode');
  }

  get EditmobileNumber() {
    return this.contactEditFormGroup.get('EditmobileNumber');
  }

  get EditcontactType() {
    return this.contactEditFormGroup.get('EditcontactType');
  }

  ngAfterViewInit() {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
  }

  constructor(
    private _location: Location,
    private _router: Router,
    private _activatedRouter: ActivatedRoute,
    private _http: HttpClient,
    private _spinnerService: NgxUiLoaderService,
    private formbuilder: FormBuilder,
    private _messageDialogService: MessageDialogService,
    private tenantService: TenantService,
    private localstorageservice: LocalStorageService,
    private injector: Injector,
  ) {
    this.tenant = new Tenant;
    _router.events.subscribe(e => {
      if (e instanceof NavigationEnd) {
        if (e.url.includes('/tenants/Add')) {
          this.updateOperationMode = false;
          localStorage.removeItem("tenantparams");
        }
      }
    });

    if (localStorage.getItem("tenantparams")) {
      this.updateOperationMode = true;
    } else {
      this.updateOperationMode = false;
    }

    _router.events.subscribe(e => {
      if (e instanceof NavigationEnd) {
        if (e.url.includes('/tenants')) {
          //set passing parameters to localstorage.
          this.params = JSON.parse(localStorage.getItem('tenantparams'));
          if (localStorage.getItem('tenantparams')) {
            this.tenant.TenantCode = this.params.Routeparams.passingparams.TenantTenantCode;
            //this.getTenants();
          }
        } else {
          localStorage.removeItem("tenantparams");
        }
      }
    });
  }

  ngOnInit() {
    this.contactTypeList.push({ "Identifier": 0, "Name": "Select Contact Type" });

    this.tenantFormGroup = this.formbuilder.group({
      tenantName: ['', Validators.compose([Validators.required, Validators.minLength(2), Validators.maxLength(100)])],
      tenantDescription: ['', Validators.compose([Validators.maxLength(500)])],
      //tenantDomainName: ['', Validators.compose([Validators.required, Validators.minLength(2), Validators.maxLength(100)])],
      tenantAddress: ['', Validators.compose([Validators.maxLength(500)])],
      tenantCountry: [0, Validators.compose([Validators.required])],
      tenantState: ['', Validators.compose([Validators.required])],
      tenantCity: ['', Validators.compose([Validators.required])],
      tenantPostalCode: ['', Validators.compose([Validators.required])],
    });

    this.contactFormGroup = this.formbuilder.group({
      firstName: ['', Validators.compose([Validators.required, Validators.minLength(2), Validators.maxLength(50), Validators.pattern(this.onlyCharacterswithInbetweenSpaceUpto50Characters)])],
      lastName: ['', Validators.compose([Validators.required, Validators.minLength(2), Validators.maxLength(50), Validators.pattern(this.onlyCharacterswithInbetweenSpaceUpto50Characters)])],
      email: ['', Validators.compose([Validators.required, Validators.pattern(this.emailRegex)])],
      mobileNumber: ['', Validators.compose([Validators.required, Validators.maxLength(10), Validators.minLength(10), Validators.pattern(this.onlyNumbers)])],
      ContactType: [0, Validators.compose([Validators.required])],
      CountryCode: [0, Validators.compose([Validators.required])],
      Image: [null]
    });

    this.contactEditFormGroup = this.formbuilder.group({
      EditfirstName: ['', Validators.compose([Validators.required, Validators.minLength(2), Validators.maxLength(50), Validators.pattern(this.onlyCharacterswithInbetweenSpaceUpto50Characters)])],
      EditlastName: ['', Validators.compose([Validators.required, Validators.minLength(2), Validators.maxLength(50), Validators.pattern(this.onlyCharacterswithInbetweenSpaceUpto50Characters)])],
      Editemail: ['', Validators.compose([Validators.required, Validators.pattern(this.emailRegex)])],
      EditmobileNumber: ['', Validators.compose([Validators.required, Validators.maxLength(10), Validators.minLength(10), Validators.pattern(this.onlyNumbers)])],
      EditcontactType: [0, Validators.compose([Validators.required])],
      EditCountryCode: [0, Validators.compose([Validators.required])],
      EditImage: [null]
    });

    this.getCountries();
    this.getContactTypes();
  }

  async getCountries() {
    let searchParameter: any = {};
    searchParameter.PagingParameter = {};
    searchParameter.PagingParameter.PageIndex = Constants.DefaultPageIndex;
    searchParameter.PagingParameter.PageSize = Constants.DefaultPageSize;
    searchParameter.SortParameter = {};
    searchParameter.SortParameter.SortColumn = Constants.Name;
    searchParameter.SortParameter.SortOrder = Constants.Ascending;
    searchParameter.SearchMode = Constants.Contains;
    let countryService = this.injector.get(CountryService);
    var response = await countryService.getCountrys(searchParameter);
    var copy = response.List;
    copy.forEach(role => {
      this.countrycodeLists.push(role);
    })
    if (this.updateOperationMode) {
      this.getTenants();
    }
  }

  async getContactTypes() {
    let searchParameter: any = {};
    searchParameter.PagingParameter = {};
    searchParameter.PagingParameter.PageIndex = Constants.DefaultPageIndex;
    searchParameter.PagingParameter.PageSize = Constants.DefaultPageSize;
    searchParameter.SortParameter = {};
    searchParameter.SortParameter.SortColumn = Constants.Name;
    searchParameter.SortParameter.SortOrder = Constants.Ascending;
    searchParameter.SearchMode = Constants.Contains;
    let service = this.injector.get(ContactTypeService);
    var response = await service.getContactTypes(searchParameter);
    var copy = response.List;
    copy.forEach(role => {
      this.contactTypeList.push(role);
    })
  }

  async getTenants() {
    let tenantService = this.injector.get(TenantService);
    var searchParameter: any = {};
    searchParameter.IsActive = true;
    searchParameter.PagingParameter = {};
    searchParameter.PagingParameter.PageIndex = Constants.DefaultPageIndex;
    searchParameter.PagingParameter.PageSize = Constants.DefaultPageSize;
    searchParameter.SortParameter = {};
    searchParameter.SortParameter.SortColumn = 'Id';
    searchParameter.SortParameter.SortOrder = Constants.Descending;
    searchParameter.SearchMode = Constants.Exact;
    searchParameter.TenantCode = this.tenant.TenantCode;
    searchParameter.IsCountryRequired = true;
    var response = await tenantService.getTenant(searchParameter);
    var tenantList = response.List;
    this.isTenantDetailsLoaded = true;
    if (tenantList.length == 0) {
      this._messageDialogService.openDialogBox('Error', "Tenant Not Found", Constants.msgBoxError);
    }
    this.BindContacts();
    this.tenant = tenantList[0];
    this.tenantCountryCode = this.tenant.Country != null && this.tenant.Country.Identifier != null ? this.tenant.Country.Identifier : 0;
    this.tenantFormGroup.controls['tenantName'].setValue(this.tenant.TenantName);
    this.tenantFormGroup.controls['tenantDescription'].setValue(this.tenant.TenantDescription);
    //this.tenantFormGroup.controls['tenantDomainName'].setValue(this.tenant.TenantDomainName);
    this.tenantFormGroup.controls['tenantAddress'].setValue(this.tenant.PrimaryAddressLine1);
    this.tenantFormGroup.controls['tenantCountry'].setValue(this.tenantCountryCode);
    this.tenantFormGroup.controls['tenantState'].setValue(this.tenant.TenantState);
    this.tenantFormGroup.controls['tenantCity'].setValue(this.tenant.TenantState);
    this.tenantFormGroup.controls['tenantPostalCode'].setValue(this.tenant.PrimaryPinCode);
    this.FirstChar = this.tenant.TenantName.charAt(0);
    this.image = this.tenant.TenantLogo;
  }

  public handlePage(e: any) {
    this.currentPage = e.pageIndex;
    this.pageSize = e.pageSize;
  }

  ShowAddContactContainer(form, contact): void {
    if (form == "Add") {
      this.isContactContainer = true;
      this.contactFormGroup.patchValue({ firstName: '', lastName: '', email: '', mobileNumber: '', ContactType: 0, CountryCode: 0 });
      this.markFormGroupUnTouched(this.contactFormGroup);
    }
    else {
      if (this.updateOperationMode) {
        this.tenantContactIdentifier = contact.Identifier;
      }
      else {
        this.tenantContactIdentifier = 0;
      }
      this.isEditContactContainer = true;
      this.contactEditFormGroup.patchValue({
        EditfirstName: contact.FirstName, EditlastName: contact.LastName, Editemail: contact.EmailAddress,
        EditmobileNumber: contact.ContactNumber, EditCountryCode: contact.CountryId, EditcontactType: contact.ContactTypeId
      }
      );
    }
  }

  CloseAddContactContainer(form): void {
    if (form == "Add") {
      this.isContactContainer = false;
    }
    else {
      this.isEditContactContainer = false;
    }
    this.resetContactForm(form);
  }

  resetContactForm(action) {
    if (action == 'Add') {
      this.contactFormGroup.patchValue({ firstName: null, lastName: null, email: null, mobileNumber: null, ContactType: null, CountryCode: null });
    } else {
      this.contactEditFormGroup.patchValue({ EditfirstName: null, EditlastName: null, Editemail: null, EditmobileNumber: null, EditcontactType: null, EditCountryCode: null });
    }
    this.Contact.CountryCode = 0;
    this.Contact.ContactType = 0;
    this.contactFormErrorObject.showContactTypeError = false;
    this.contactFormErrorObject.showCountryCodeError = false;
  }

  navigateToListPage() {
    this._router.navigate(['/tenants']);
  }

  async BindContacts() {
    var searchParameter: any = {};
    searchParameter.IsActive = true;
    searchParameter.PagingParameter = {};
    searchParameter.PagingParameter.PageIndex = Constants.DefaultPageIndex;
    searchParameter.PagingParameter.PageSize = Constants.DefaultPageSize;
    searchParameter.SortParameter = {};
    searchParameter.SortParameter.SortColumn = 'Id';
    searchParameter.SortParameter.SortOrder = Constants.Descending;
    searchParameter.SearchMode = Constants.Exact;
    searchParameter.TenantCode = this.tenant.TenantCode;
    searchParameter.IsTenantPagesRequired = true;
    let tenantService = this.injector.get(TenantService);
    var response = await tenantService.getTenantContact(searchParameter);
    this.contactlist = response.List;
    this.dataSource = new MatTableDataSource<TenantContact>(this.contactlist);
    this.dataSource.sort = this.sort;
    this.array = this.contactlist;
    this.totalSize = this.array.length;
    this.iterator();
  }

  onFileChanged(event) {
    let file = event.target.files[0];
    if (file.size > 200000) {
      this.tenantFormErrorObject.showProfilePictureSizeError = true;
      return false;
    }

    this.tenantFormErrorObject.showProfilePictureSizeError = false;
    var pattern = /image-*/;
    if (!file.type.match(pattern)) {
      this.tenantFormErrorObject.showProfilePictureTypeError = true;
      return false;
    }
    this.tenantFormErrorObject.showProfilePictureTypeError = false;
    let reader = new FileReader();
    reader.readAsDataURL(file);

    reader.onload = (e) => {
      if (reader.DONE == 2) {
        this.image = reader.result.toString();
      }
    };
    reader.onerror = function (error) {
      //console.log('Error: ', error);
    };
  }

  contactFormValidation(form): boolean {
    if (form == 'Edit') {
      if (this.contactEditFormGroup.controls.EditcontactType.invalid) {
        return true;
      }
      if (this.contactEditFormGroup.controls.EditfirstName.invalid) {
        return true;
      }
      if (this.contactEditFormGroup.controls.EditlastName.invalid) {
        return true;
      }
      if (this.contactEditFormGroup.controls.Editemail.invalid) {
        return true;
      }
      if (this.contactEditFormGroup.controls.EditCountryCode.value == 0) {
        return true;
      }
      if (this.contactEditFormGroup.controls.EditmobileNumber.invalid) {
        return true;
      }
      return false;
    }
    else {
      if (this.contactFormGroup.controls.ContactType.invalid) {
        return true;
      }
      if (this.contactFormGroup.controls.firstName.invalid) {
        return true;
      }
      if (this.contactFormGroup.controls.lastName.invalid) {
        return true;
      }
      if (this.contactFormGroup.controls.email.invalid) {
        return true;
      }
      if (this.contactFormGroup.controls.CountryCode.value == 0) {
        return true;
      }
      if (this.contactFormGroup.controls.mobileNumber.invalid) {
        return true;
      }
      return false;
    }
  }

  public onContactTypeSelect(event) {
    const value = event.target.value;
    if (value == "0") {
      this.contactFormErrorObject.showContactTypeError = true;
      this.Contact.ContactType = 0;
    }
    else {
      this.contactFormErrorObject.showContactTypeError = false;
      this.Contact.ContactType = value;
    }
  }

  public onCountrySelected(event) {
    const value = event.target.value;
    if (value == "0") {
      this.contactFormErrorObject.showCountryCodeError = true;
      this.Contact.CountryCode = 0;
    }
    else {
      this.contactFormErrorObject.showCountryCodeError = false;
      this.Contact.CountryCode = value;
    }
  }

  public onTenantCountrySelected(event) {
    const value = event.target.value;
    if (value == "0") {
      this.tenantFormErrorObject.showCountryCodeError = true;
      this.tenantCountryCode = 0;
    }
    else {
      this.tenantFormErrorObject.showCountryCodeError = false;
      this.tenantCountryCode = Number(value);
    }
  }

  async UpdateContact() {
    var contact = this.contactTypeList.filter(s => this.contactEditFormGroup.value.EditcontactType == s.Identifier);
    var country = this.countrycodeLists.filter(s => this.contactEditFormGroup.value.EditCountryCode == s.Identifier);
    let contactObject: TenantContact = {
      "FirstName": this.contactEditFormGroup.value.EditfirstName.trim(),
      "LastName": this.contactEditFormGroup.value.EditlastName.trim(),
      "EmailAddress": this.contactEditFormGroup.value.Editemail,
      "ContactNumber": this.contactEditFormGroup.value.EditmobileNumber,
      "CountryId": this.contactEditFormGroup.value.EditCountryCode,
      "CountryCode": country[0].DialingCode,
      "ContactType": contact[0].Name,
      "ContactTypeId": this.contactEditFormGroup.value.EditcontactType,
      "ProfileImage": '',
      "Identifier": this.tenantContactIdentifier,
      "IsActive": true,
      "TenantCode": this.tenant.TenantCode,
      "IsActivationLinkSent": false
    }
    if (this.updateOperationMode) {
      var data = [];
      data.push(contactObject);
      let tenantService = this.injector.get(TenantService);
      let isRecordSaved = await tenantService.saveTenantContact(data, true);
      if (isRecordSaved) {
        let message = Constants.recordAddedMessage;
        if (this.updateOperationMode) {
          message = Constants.recordUpdatedMessage;
        }
        this._messageDialogService.openDialogBox('Success', message, Constants.msgBoxSuccess);
        this.BindContacts();
      }
    }
    else {
      var contacts = [];
      contacts = this.contactlist.filter(s => contactObject.EmailAddress == s.EmailAddress);
      if (contacts.length > 0) {
        this._messageDialogService.openDialogBox('Error', "Duplicate tenant contact found", Constants.msgBoxSuccess);
      }
      else {
        this.contactlist.forEach(item => {
          if (item.EmailAddress == contactObject.EmailAddress) {
            item = contactObject;
          }
        });
        this.dataSource = new MatTableDataSource<TenantContact>(this.contactlist);
        this.dataSource.paginator = this.paginator;
        this.dataSource.sort = this.sort;
      }
    }
    this.CloseAddContactContainer('Edit');
  }

  async onSubmitAdd() {
    var contact = this.contactTypeList.filter(s => this.contactFormGroup.value.ContactType == s.Identifier);
    var country = this.countrycodeLists.filter(s => this.contactFormGroup.value.CountryCode == s.Identifier);
    let contactObject: TenantContact = {
      "FirstName": this.contactFormGroup.value.firstName.trim(),
      "LastName": this.contactFormGroup.value.lastName.trim(),
      "EmailAddress": this.contactFormGroup.value.email,
      "ContactNumber": this.contactFormGroup.value.mobileNumber,
      "CountryId": this.contactFormGroup.value.CountryCode,
      "CountryCode": country[0].DialingCode,
      "ContactType": contact[0].Name,
      "ContactTypeId": this.contactFormGroup.value.ContactType,
      "ProfileImage": '',
      "Identifier": 0,
      "IsActive": true,
      "TenantCode": this.tenant.TenantCode,
      "IsActivationLinkSent": false
    }
    if (this.updateOperationMode) {
      var data = [];
      data.push(contactObject);
      let tenantService = this.injector.get(TenantService);
      let isRecordSaved = await tenantService.saveTenantContact(data, false);
      if (isRecordSaved) {
        let message = Constants.recordAddedMessage;
        if (this.updateOperationMode) {
          message = Constants.recordUpdatedMessage;
        }
        this._messageDialogService.openDialogBox('Success', message, Constants.msgBoxSuccess);
        this.BindContacts();
      }
    }
    else {
      var contacts = [];
      if (this.contactlist.length <= 0) {
        if (contactObject.ContactType == "Primary") {
          contactObject.IsActivationLinkSent = true;
        }
      }
      else {
        var primaryContact = this.contactlist.filter(s => s.ContactType == "Primary");
        if (primaryContact == null || primaryContact.length == 0) {
          if (contactObject.ContactType == "Primary") {
            contactObject.IsActivationLinkSent = true;
          }
        }
      }
      contacts = this.contactlist.filter(s => contactObject.EmailAddress == s.EmailAddress);
      if (contacts.length > 0) {
        this._messageDialogService.openDialogBox('Error', "Duplicate tenant contact found", Constants.msgBoxSuccess);
      }
      else {
        this.contactlist.push(contactObject);
        this.dataSource = new MatTableDataSource(this.contactlist);
        this.dataSource.sort = this.sort;
        this.array = this.contactlist;
        this.totalSize = this.array.length;
        this.iterator();
      }
    }
    this.CloseAddContactContainer('Add');

  }

  private iterator() {
    const end = (this.currentPage + 1) * this.pageSize;
    const start = this.currentPage * this.pageSize;
    const part = this.array.slice(start, end);
    this.dataSource = part;
    this.dataSource.sort = this.sort;
  }

  async DeleteContact(contact) {
    var index = this.contactlist.findIndex(s => contact.EmailAddress == s.EmailAddress);
    if (this.updateOperationMode) {
      let message = 'Are you sure, you want to delete this record?';
      this._messageDialogService.openConfirmationDialogBox('Confirm', message, Constants.msgBoxWarning).subscribe(async (isConfirmed) => {
        if (isConfirmed) {
          //let data = [{
          //  "Identifier": contact.Identifier,
          //  "TenantCode": this.tenant.TenantCode
          //}];
          let data = [];
          data.push(contact);

          let isDeleted = await this.tenantService.deleteTenantContact(data);
          if (isDeleted) {
            let messageString = Constants.recordDeletedMessage;
            this._messageDialogService.openDialogBox('Success', messageString, Constants.msgBoxSuccess);
            this.BindContacts();
          }
        }
      });
    }
    else {
      let message = 'Are you sure, you want to delete this record?';
      this._messageDialogService.openConfirmationDialogBox('Confirm', message, Constants.msgBoxWarning).subscribe(async (isConfirmed) => {
        if (isConfirmed) {
          this.contactlist.splice(index, 1);
          this.dataSource = new MatTableDataSource(this.contactlist);
          this.dataSource.sort = this.sort;
          this.array = this.contactlist;
          this.totalSize = this.array.length;
          this.iterator();
        }
      });
    }

  }

  onTenantAddressChange(value) {
    if (value == '') {
      this.tenantAddressFieldError = true;
    } else {
      this.tenantAddressFieldError = false;
    }
  };

  validateTenantForm() {
    if (this.tenantFormGroup.invalid) {
      return true;
    }
    if (this.tenantFormGroup.controls['tenantAddress'].value == '') {
      return true;
    }
    if(this.tenantCountryCode == 0) {
      return true;
    }
    if (this.imageSize > 200000) {
      return true;
    }
    if (this.tenantFormErrorObject.showProfilePictureSizeError) {
      return true;
    }
    if (this.tenantFormErrorObject.showProfilePictureTypeError) {
      return true;
    }
    return false;
  }

  async saveTenant() {
    if (this.contactlist.length > 0) {
      var primaryContact = this.contactlist.filter(s => s.ContactType == "Primary");
      if (primaryContact.length > 0) {

        this.tenant.TenantName = this.tenantFormGroup.value.tenantName;
        this.tenant.TenantDomainName = "domain.com";
        this.tenant.PrimaryPinCode = this.tenantFormGroup.value.tenantPostalCode;
        this.tenant.PrimaryAddressLine1 = this.tenantFormGroup.value.tenantAddress;
        this.tenant.TenantCity = this.tenantFormGroup.value.tenantCity
        this.tenant.TenantCountry = this.tenantFormGroup.value.tenantCountry;
        this.tenant.TenantState = this.tenantFormGroup.value.tenantState;
        this.tenant.TenantDescription = this.tenantFormGroup.value.tenantDescription;
        this.tenant.TenantLogo = this.image;
        this.tenant.TenantContacts = this.contactlist;
        this.tenant.TenantType = "Tenant";
        var userid = localStorage.getItem('UserId');
        this.tenant.User = {};
        this.tenant.User.Identifier = userid;
        var currentUser = this.localstorageservice.GetCurrentUser();
        this.tenant.ParentTenantCode = currentUser.TenantCode;

        let pageArray = [];
        pageArray.push(this.tenant);
        let tenantService = this.injector.get(TenantService);
        let isRecordSaved = await tenantService.saveTenant(pageArray, this.updateOperationMode);
        if (isRecordSaved) {
          let message = Constants.recordAddedMessage;
          if (this.updateOperationMode) {
            message = Constants.recordUpdatedMessage;
          }
          this._messageDialogService.openDialogBox('Success', message, Constants.msgBoxSuccess);
          this.navigateToListPage()
        }
      }
      else {
        this._messageDialogService.openDialogBox('Error', "Please add primary contact", Constants.msgBoxSuccess);

      }

    }
    else {
      this._messageDialogService.openDialogBox('Error', "Please add primary contact", Constants.msgBoxSuccess);
    }
  }

  async sentActivationLink(contact) {
    if (this.updateOperationMode) {
      let data = [];
      data.push(contact);

      let isDeleted = await this.tenantService.sendActivationLink(data);
      if (isDeleted) {
        let messageString = "Activation link sent successfully. Please check your email";
        this._messageDialogService.openDialogBox('Success', messageString, Constants.msgBoxSuccess);
        this.BindContacts();
      }
    }
    else {
      this.contactlist.forEach(item => {
        if (contact.EmailAddress == item.EmailAddress) {
          item.IsActivationLinkSent = true;
        }

      });
      this.dataSource = new MatTableDataSource(this.contactlist);
      this.dataSource.sort = this.sort;
      this.array = this.contactlist;
      this.totalSize = this.array.length;
      this.iterator();
    }

  }

  private markFormGroupUnTouched(formGroup: FormGroup) {
    (<any>Object).values(formGroup.controls).forEach(control => {
      control.markAsUntouched();
      if (control.controls) {
        this.markFormGroupUnTouched(control);
      }
    });
  }

}
