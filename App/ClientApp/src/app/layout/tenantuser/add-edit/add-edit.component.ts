import { Component, OnInit, ViewChild, Injector } from '@angular/core';
import { Location } from '@angular/common';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';
import { __values } from 'tslib';
import { Constants } from 'src/app/shared/constants/constants';
import { TenantUserService } from '../tenantuser.service';
import { HttpResponse, HttpEvent, HttpEventType } from '@angular/common/http';
import { HttpClient } from '@angular/common/http';
import { DialogService } from '@tomblue/ng2-bootstrap-modal';
import { MsgBoxComponent } from 'src/app/shared/modules/message/messagebox.component';
import { Router, NavigationEnd } from '@angular/router';
import { jqxDropDownListComponent } from 'jqwidgets-ng/jqxdropdownlist';
import { NgxUiLoaderService } from 'ngx-ui-loader';
import { MessageDialogService } from 'src/app/shared/services/mesage-dialog.service';
import { LocalStorageService } from 'src/app/shared/services/local-storage.service';
import { LoginService } from '../../../login/login.service';
import { Country } from '../../country/country';
import { CountryService } from '../../country/country.service';
import { RoleService } from '../../roles/role.service';

@Component({
  selector: 'app-add-edit',
  templateUrl: './add-edit.component.html',
  styleUrls: ['./add-edit.component.scss']
})
export class AddEditComponent implements OnInit {

  tenantuserFormGroup: FormGroup;
  public emailRegex = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
  public onlyAlphabetsWithSpaceQuoteHyphen = "[a-zA-Z]+[ ]{0,1}[a-zA-Z]*[ ]*$";
  public onlyCharacterswithInbetweenSpaceUpto50Characters = Constants.onlyCharacterswithInbetweenSpaceUpto50Characters;
  public onlyNumbers = '[0-9]*';
  public errorMsg: boolean;
  public roleList = [{ "Name": "Select Role", "Identifier": 0 }];
  public roleLists: any = [];
  public source: string[] = [];
  public countrycodeList = [];
  public countrycodeLists = [
    { "Identifier": 0, "Code": "Please Select", "DialingCode": "" }
  ];
  image: string;
  FirstChar: string;
  SecondChar: string
  public countrycodesource = [];
  public selectedAll: any;
  public allRoles;
  public tenantuserId;
  public TenantUser = { RoleIdentifier: 0, Image: '', CountryCode: 0 }
  public addTenantUser: boolean = true;
  public editTenantUser: boolean;
  public roleName;
  public roleDescription;
  public slider: boolean = false;
  public activeSlider: boolean = true;
  public allowAccessSlider: boolean = false;
  public lockSlider: boolean = false;
  public notificationSlider: boolean = false;
  public Status;
  public OrganizationUnitName = [];
  public organizationUnitArr = [];
  public title;
  public tenantuserEditModeOn: boolean = false;
  public isCollapsedDetails: boolean = false;
  public isCollapsedRoles: boolean = true;
  public params;
  public TenantUserIdentifier;
  public TenantUserName;
  public tenantuserAddEditResources = {}
  public tenantuserRoleFilterForm: FormGroup;
  public isShowFilterRoles: boolean = false
  public Locale;
  fileData: any = null;
  // previewUrl: any = null;
  previewUrl: any = [];
  fileUploadProgress: string = null;
  uploadedFilePath: string = null;
  public isImageLoded: boolean = false;
  public showDefaultLogo: boolean = true;
  public imageSize: number;
  public imageType = "";
  public imageName = "";
  public parentOUName = "";
  public tenantusersList = [];
  public designationList = [];
  public designationLists: any = [];
  public designationListSource: any = [];
  public languageList = [];
  public languageLists: any = [];
  public languageSource: any[] = []
  public selectedImages = [];
  public treeData: any = [];
  public profileImage;
  public tenantuserImage;
  public resourceId;
  public tenantuserClaimsRolePrivilegeOperations = [];
  public isOrganizationDataRetrieved: boolean = false;
  public isRoleDataRetrieved: boolean = false;
  public isDestinationDataRetrieved: boolean = false;
  public isLanguageDataRetrieved: boolean = false;
  public isCountryDataRetrieved: boolean = false;
  public fileArray = [];
  public Theme = 6;
  public roleObject:any = {};

  // Object created to initlialize the error boolean value.
  public tenantuserFormErrorObject: any = {
    showProfilePictureSizeError: false,
    showProfilePictureTypeError: false,
    showTenantUserFirstNameError: false,
    showTenantUserLastNameError: false,
    showTenantUserCodeError: false,
    showTenantUserEmailError: false,
    showCountryCodeError: false,
    showTenantUserMobileNumberError: false,
  };

  //getters of tenantusersForm group
  get firstName() {
    return this.tenantuserFormGroup.get('firstName');
  }
  get lastName() {
    return this.tenantuserFormGroup.get('lastName');
  }
  get code() {
    return this.tenantuserFormGroup.get('code');
  }
  get email() {
    return this.tenantuserFormGroup.get('email');
  }
  get countryCode() {
    return this.tenantuserFormGroup.get('countryCode');
  }
  get mobileNumber() {
    return this.tenantuserFormGroup.get('mobileNumber');
  }

  //function to validate all fields
  validateAllFormFields(formGroup: FormGroup) {
    Object.keys(formGroup.controls).forEach(field => {
      const control = formGroup.get(field);
      if (control instanceof FormControl) {
        control.markAsTouched({ onlySelf: true });
      } else if (control instanceof FormGroup) {
        this.validateAllFormFields(control);
      }
    });
  }

  constructor(private _location: Location,
    private formBuilder: FormBuilder,
    private http: HttpClient,
    private service: TenantUserService,
    private _dialogService: DialogService,
    private spinner: NgxUiLoaderService,
    private router: Router,
    private _messageDialogService: MessageDialogService,
    private localstorageservice: LocalStorageService,
    private injector: Injector,
    private loginService: LoginService,
  ) {
    router.events.subscribe(e => {
      if (e instanceof NavigationEnd) {
        if (e.url.includes('/tenantusers/add')) {
          this.tenantuserEditModeOn = false;
          localStorage.removeItem("tenantuserRouteparams");
        }
      }
    });

    if (localStorage.getItem("tenantuserRouteparams")) {
      this.tenantuserEditModeOn = true;
    } else {
      this.tenantuserEditModeOn = false;
    }

    router.events.subscribe(e => {
      if (e instanceof NavigationEnd) {
        if (e.url.includes('/tenantusers')) {
          //set passing parameters to localstorage.
          this.params = JSON.parse(localStorage.getItem('tenantuserRouteparams'));
          if (localStorage.getItem('tenantuserRouteparams')) {
            this.TenantUserIdentifier = this.params.Routeparams.passingparams.TenantUserIdentifier;
            //this.TenantUserName = this.params.Routeparams.filteredparams.TenantUserName
          }
        } else {
          localStorage.removeItem("tenantuserRouteparams");
        }
      }
    });
  }


  //Initialization call--onlyNumbers
  ngOnInit() {
    // TenantUser form validations.
    this.tenantuserFormGroup = this.formBuilder.group({
      firstName: [null, Validators.compose([Validators.required, Validators.minLength(2),
      Validators.maxLength(50),
      Validators.pattern(this.onlyCharacterswithInbetweenSpaceUpto50Characters)])],
      lastName: [null, Validators.compose([Validators.required, Validators.minLength(2),
      Validators.maxLength(50),
      Validators.pattern(this.onlyCharacterswithInbetweenSpaceUpto50Characters)])],
      email: ['', Validators.compose([Validators.required,
      Validators.pattern(this.emailRegex)])],
      mobileNumber: ['', Validators.compose([Validators.required,
      Validators.maxLength(10),
      Validators.minLength(10),
      Validators.pattern(this.onlyNumbers)])],
      CountryCode: [0, Validators.compose([Validators.required])],
      Image: [null]
    })
    this.getCountries();
    this.getInstantManagerRole();
  }

  //Function to get role--
  async getInstantManagerRole() {
    let roleService = this.injector.get(RoleService);
    let searchParameter: any = {};
    searchParameter.PagingParameter = {};
    searchParameter.PagingParameter.PageIndex = Constants.DefaultPageIndex;
    searchParameter.PagingParameter.PageSize = Constants.DefaultPageSize;
    searchParameter.SortParameter = {};
    searchParameter.SortParameter.SortColumn = "Id";
    searchParameter.SortParameter.SortOrder = Constants.Ascending;
    searchParameter.SearchMode = Constants.Contains;
    searchParameter.Name = "Instance Manager";
    var response = await roleService.getRoles(searchParameter);
    let _list = response.roleList;
    this.roleObject = _list[0];
  }
 
  public countryList: Country;
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
   
    if (this.tenantuserEditModeOn) {
      this.fillTenantUserDetail();
    }
  }

  onFileChanged(event) {
    let file = event.target.files[0];
    if (file.size > 200000) {
      this.tenantuserFormErrorObject.showProfilePictureSizeError = true;
      return false;
    }

    this.tenantuserFormErrorObject.showProfilePictureSizeError = false;
    var pattern = /image-*/;
    if (!file.type.match(pattern)) {
      this.tenantuserFormErrorObject.showProfilePictureTypeError = true;
      return false;
    }
    this.tenantuserFormErrorObject.showProfilePictureTypeError = false;
    let reader = new FileReader();
    reader.readAsDataURL(file);
    reader.onload = (e) => {
      if (reader.DONE == 2) {
        this.image = reader.result.toString();
        //console.log('reader.result');
        //console.log(this.image);
      }
    };
    reader.onerror = function (error) {
      //console.log('Error: ', error);
    };
  }

  traverse(stationList) {
    var key;
    for (key in stationList) {
      stationList.expanded = true;
      if (key == "Name") {
        stationList.label = stationList.Name;
        delete stationList.Name;
      }
      if (key == "Identifier") {
        stationList.value = stationList.Identifier;
        // delete stationList.Identifier;
      }
      if (key == "Children") {
        stationList.items = stationList.Children;
        //delete stationList.Children;
      }
      if (stationList[key] !== null && typeof (stationList[key]) == "object") {
        //going one step down in the object tree!!
        this.traverse(stationList[key]);
      }
    };
  }

  //function call to search role from the rolelist--
  searchFilter(searchType) {
    //this.spinner.start();
    if (searchType == 'reset') {
   
    }
    else {
      let searchParameter: any = {};
      searchParameter.RoleName = this.tenantuserRoleFilterForm.value.filterRoleName != null ? this.tenantuserRoleFilterForm.value.filterRoleName : "";
      searchParameter.PagingParameter = {};
      searchParameter.PagingParameter.PageIndex = Constants.DefaultPageIndex;
      searchParameter.PagingParameter.PageSize = Constants.DefaultPageSize;
      searchParameter.SortParameter = {};
      searchParameter.SortParameter.SortColumn = Constants.UserName;
      searchParameter.SortParameter.SortOrder = Constants.Ascending;
      searchParameter.SearchMode = Constants.Contains;
    }
  }

  //Function to select all roles--
  selectAllRole(event) {
    this.allRoles = event.target.checked
    this.roleLists.forEach(allRole => {
      allRole.IsEnabled = event.target.checked;
    })
  }

  //custom validation check
  tenantuserFormValidaton() {
    this.tenantuserFormErrorObject.showProfilePictureSizeError = false;
    this.tenantuserFormErrorObject.showTenantUserFirstNameError = false;
    this.tenantuserFormErrorObject.showTenantUserLastNameError = false;
    this.tenantuserFormErrorObject.showTenantUserEmailError = false;
    this.tenantuserFormErrorObject.showCountryCodeError = false;
    this.tenantuserFormErrorObject.showTenantUserMobileNumberError = false;
    this.tenantuserFormErrorObject.roleShowError = false;
    if (this.imageSize > 200000) {
      this.tenantuserFormErrorObject.showProfilePictureSizeError = true;
      return false;
    }
    if (this.tenantuserFormGroup.controls.firstName.invalid) {
      this.tenantuserFormErrorObject.showTenantUserFirstNameError = true;
      return false;
    }
    if (this.tenantuserFormGroup.controls.lastName.invalid) {
      this.tenantuserFormErrorObject.showTenantUserLastNameError = true;
      return false;
    }
    if (this.tenantuserFormGroup.controls.email.invalid) {
      this.tenantuserFormErrorObject.showTenantUserEmailError = true;
      return false;
    }

    if (this.tenantuserFormGroup.controls.mobileNumber.invalid) {
      this.tenantuserFormErrorObject.showTenantUserMobileNumberError = true;
      return false;
    }

    if (this.TenantUser.CountryCode == 0) {
      this.tenantuserFormErrorObject.showCountryCodeError = true;
      return false;
    }
    return true;
  }

  saveButtonValidation(): boolean {
    if (this.imageSize > 200000) {
      return true;
    }
    if (this.tenantuserFormErrorObject.showProfilePictureSizeError) {
      return true;
    }
    if (this.tenantuserFormErrorObject.showProfilePictureTypeError) {
      return true;
    }
    if (this.tenantuserFormGroup.controls.firstName.invalid) {
      return true;
    }
    if (this.tenantuserFormGroup.controls.lastName.invalid) {
      return true;
    }
    if (this.tenantuserFormGroup.controls.email.invalid) {
      return true;
    }
    if (this.tenantuserFormGroup.controls.mobileNumber.invalid) {
      return true;
    }
    if (this.TenantUser.CountryCode == 0) {
      return true;
    }
    return false;
  }

  public onRoleSelected(event) {
    const value = event.target.value;
    if (value == "0") {
      this.tenantuserFormErrorObject.roleShowError = true;
      this.TenantUser.RoleIdentifier = 0;
    }
    else {
      this.tenantuserFormErrorObject.roleShowError = false;
      this.TenantUser.RoleIdentifier = Number(value);
    }
  }

  public onCountrySelected(event) {
    const value = event.target.value;
    if (value == "") {
      this.tenantuserFormErrorObject.showCountryCodeError = true;
      this.TenantUser.CountryCode = 0
    }
    else {
      this.tenantuserFormErrorObject.showCountryCodeError = false;
      this.TenantUser.CountryCode = value
    }
  }

  //Function to add tenantuser--
  onSubmit() {
    if (this.tenantuserFormValidaton()) {
      let selectedroleArray = [];
      this.roleList.forEach(role => {
        if (role.Identifier == this.TenantUser.RoleIdentifier) {
          selectedroleArray.push({
            "Identifier": this.roleObject.Identifier,
            "Name": this.roleObject.Name,
          });
        }
      })
      let tenantuserObject: any = {
        "FirstName": this.tenantuserFormGroup.value.firstName.trim(),
        "LastName": this.tenantuserFormGroup.value.lastName.trim(),
        "EmailAddress": this.tenantuserFormGroup.value.email,
        //"ContactNumber": this.tenantuserFormGroup.value.CountryCode + "-" + this.tenantuserFormGroup.value.mobileNumber,
        "ContactNumber": this.tenantuserFormGroup.value.mobileNumber,
        "CountryId": this.tenantuserFormGroup.value.CountryCode,
        "Roles": selectedroleArray,
        "Image": this.image,
      }
      if (this.tenantuserEditModeOn) {
        tenantuserObject.Identifier = this.params.Routeparams.passingparams.TenantUserIdentifier;
        tenantuserObject.IsActive = true;
        tenantuserObject.IsLocked = false;
      }
      else {
        tenantuserObject.IsActive = false;
        tenantuserObject.IsLocked = true;
      }
      //console.log(tenantuserObject)
      this.saveRecord(tenantuserObject);
    }
  }

  //Api called here to save record
  saveRecord(tenantuserObject) {
    var formData = new FormData()
    var TenantUserArr = [];
    tenantuserObject.IsInstanceManager = true;
    TenantUserArr.push(tenantuserObject);
    this.spinner.start();
    this.service.saveUser(TenantUserArr, this.tenantuserEditModeOn).subscribe(data => {
      this.spinner.stop();
      if (data == true) {
        let message = "User added successfully";
        if (this.tenantuserEditModeOn) {
          message = Constants.recordUpdatedMessage;
        }
        this._messageDialogService.openDialogBox('Success', message, Constants.msgBoxSuccess);
        this.navigateToListPage();
      }
    }, (error: any) => {
      this._messageDialogService.openDialogBox('Error', error.error.Message, Constants.msgBoxError);
      this.spinner.stop();
    });
  }

  //Back Functionality.
  navigateToListPage() {
    this.router.navigate(['tenantusers']);
  }

  //Function to patch/set tenantuser details at edit mode--
  async fillTenantUserDetail() {
    this.params = JSON.parse(localStorage.getItem('tenantuserRouteparams'));
    this.TenantUserIdentifier = this.params.Routeparams.passingparams.TenantUserIdentifier
    let tenantuserService = this.injector.get(TenantUserService);
    let searchParameter: any = {};
    searchParameter.IsRolePrivilegesRequired = true;
    searchParameter.Identifier = this.TenantUserIdentifier;
    searchParameter.PagingParameter = {};
    searchParameter.PagingParameter.PageIndex = Constants.DefaultPageIndex;
    searchParameter.PagingParameter.PageSize = Constants.DefaultPageSize;
    searchParameter.SortParameter = {};
    searchParameter.SortParameter.SortColumn = "Id";
    searchParameter.SortParameter.SortOrder = Constants.Ascending;
    searchParameter.SearchMode = Constants.Contains;
    searchParameter.IsInstanceManager = true;
    var response = await tenantuserService.getUser(searchParameter);
    this.tenantusersList = response.usersList;
    this.tenantusersList.forEach(tenantuserObject => {
      this.resourceId = tenantuserObject.ResourceIdentifier
      this.previewUrl = [];
      if (tenantuserObject.ProfileImage) {
        if (tenantuserObject.ProfileImage.URL) {
          this.profileImage = tenantuserObject.ProfileImage.URL;
          this.previewUrl.push(this.profileImage);
          this.isImageLoded = true
          this.showDefaultLogo = false
        }
      }
      this.FirstChar = tenantuserObject.FirstName.charAt(0);
      this.SecondChar = tenantuserObject.LastName.charAt(0);

      let mobileNo, countryDiallingCode;
      let mobileNoArr = tenantuserObject.ContactNumber.split('-');
      if (mobileNoArr.length > 1) {
        countryDiallingCode = mobileNoArr[0];
        mobileNo = mobileNoArr[1];
      }
      else {
        mobileNo = tenantuserObject.MobileNumber;
      }
      let country = this.countrycodeLists.filter(i => { return i.Identifier == tenantuserObject.CountryId });
      this.tenantuserFormGroup.patchValue({
        firstName: tenantuserObject.FirstName,
        lastName: tenantuserObject.LastName,
        email: tenantuserObject.EmailAddress,
        mobileNumber: mobileNo,
        CountryCode: country[0].Identifier
      })
      this.tenantuserFormGroup.controls['CountryCode'].setValue(country[0].Identifier);
      this.TenantUser.CountryCode = country[0].Identifier;
      this.activeSlider = tenantuserObject.IsActive;
      this.lockSlider = tenantuserObject.IsLocked;
      this.image = tenantuserObject.Image;
    });
  }

  //Function to upload image--
  fileUpload(fileInput: any) {
    this.previewUrl = [];
    this.fileArray = [];
    if (fileInput.target.files && fileInput.target.files[0]) {
      var filesAmount = fileInput.target.files.length;
      this.fileData = <File>fileInput.target.files[0];
      this.fileArray.push(this.fileData);
      //console.log(this.fileData);
      for (let i = 0; i < filesAmount; i++) {
        var reader = new FileReader();
        reader.onload = (event: any) => {
          this.previewUrl.push(event.target.result);
          this.isImageLoded = true
          this.showDefaultLogo = false
        }
        reader.readAsDataURL(fileInput.target.files[i]);
      }
    }

  }


}
