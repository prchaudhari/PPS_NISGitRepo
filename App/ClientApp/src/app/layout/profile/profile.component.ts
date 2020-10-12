import { Component, OnInit, Injector, ViewChild } from '@angular/core';
import { Location } from '@angular/common';
import { Router } from '@angular/router';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';
import { LocalStorageService } from 'src/app/shared/services/local-storage.service';
import { ConfigConstants } from 'src/app/shared/constants/configConstants';
import { ResourceService } from 'src/app/shared/services/resource.service';
import { jqxDropDownListComponent } from 'jqwidgets-ng/jqxdropdownlist';
import { UserService } from '../../layout/users/user.service';
import { Constants } from 'src/app/shared/constants/constants';
import { MessageDialogService } from 'src/app/shared/services/mesage-dialog.service';
import { NgxUiLoaderService } from 'ngx-ui-loader';
import { HttpResponse, HttpEvent, HttpEventType } from '@angular/common/http';
import { Country } from '../country/country';
import { CountryService } from '../country/country.service';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html'
})
export class ProfileComponent implements OnInit {

  //profile form.
  public profileFormGroup: FormGroup;
  public profileResources = {}
  public ResourceLoadingFailedMsg = "Resouce Loading Failed..";
  public Locale;
  public isTheme1Active: boolean = false;
  public isTheme2Active: boolean = false;
  public isTheme3Active: boolean = false;
  public isTheme4Active: boolean = false;
  public isTheme5Active: boolean = false;
  public isTheme0Active: boolean = true;

  image: string;
  fileData: File = null;
  previewUrl: any = null;
  fileUploadProgress: string = null;
  uploadedFilePath: string = null;
  public isImageLoded: boolean = false;
  public showDefaultLogo: boolean = true;
  public imageSize: number;
  public imageType = "";
  public imageName = "";
  public countrycodesource = [];
  public emailRegex = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
  public countrycodeList;
  //public countrycodeLists = [];
  public receiveNotification: boolean = false;
  public usersList = [];
  public userIdentifier;
  public profileEditModeOn: boolean = false;
  public UserIdentifier;
  public userClaimsRolePrivilegeOperations = [];
  public languageList = [];
  public isLanguageDataRetrieved = false;
  public isCountryDataRetrieved = false;
  // source language
  public sourcelanguage = [];
  public onlyAlphabetsWithSpaceQuoteHyphen = "[a-zA-Z]+[ ]{0,1}[a-zA-Z]*[ ]*$";
  public onlyCharacterswithInbetweenSpaceUpto50Characters = Constants.onlyCharacterswithInbetweenSpaceUpto50Characters;
  public onlyNumbers = '[0-9]*';
  public resourceId;
  public profileImage;
  public fileArray = [];
  public languageLists = [];
  public userRecord: any = {};
  public User = { RoleIdentifier: 0, Image: '', CountryCode: '' }
  public countrycodeLists = [{ "Identifier": 0, "Code": "Please Select", "DialingCode": "" }];
  public isTenantAdminUser: boolean = false;
  public isInstantTenantManager: boolean = false;
  public isTenantGroupManager: boolean = false;
  public TenantCode = '';

  constructor(private _location: Location,
    private formbulder: FormBuilder,
    private route: Router,
    private injector: Injector,
    private localstorageservice: LocalStorageService,
    private _messageDialogService: MessageDialogService,
    private uiLoader: NgxUiLoaderService) {
    let userData = JSON.parse(localStorage.getItem('userClaims'));
    this.UserIdentifier = userData.UserIdentifier;
  }

  // Object created to initlialize the error boolean value.
  public profileFormErrorObject: any = {
    showProfilePictureSizeError: false,
    showUserFirstNameError: false,
    showUserLastNameError: false,
    showUserCodeError: false,
    showUserEmailError: false,
    showUserMobileNumberError: false,
    showCountryCodeError: false
  };

  //getters of usersForm group
  get firstName() {
    return this.profileFormGroup.get('firstName');
  }
  get lastName() {
    return this.profileFormGroup.get('lastName');
  }
  get code() {
    return this.profileFormGroup.get('code');
  }
  get email() {
    return this.profileFormGroup.get('email');
  }
  get countryCode() {
    return this.profileFormGroup.get('countryCode');
  }
  get mobileNumber() {
    return this.profileFormGroup.get('mobileNumber');
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

  ngOnInit() {
    // Profile form validations.
    this.profileFormGroup = this.formbulder.group({
      firstName: [null, Validators.compose([Validators.required,
      Validators.minLength(2), Validators.maxLength(50),
      Validators.pattern(this.onlyCharacterswithInbetweenSpaceUpto50Characters)])],
      lastName: [null, Validators.compose([Validators.required,
      Validators.minLength(2), Validators.maxLength(50),
      Validators.pattern(this.onlyCharacterswithInbetweenSpaceUpto50Characters)])],
      code: [null, Validators.compose([Validators.required])],
      email: [null, Validators.compose([Validators.required, Validators.pattern(this.emailRegex)])],
      preferredLanguage: [''],
      Image: [''],
      CountryCode: [''],
      mobileNumber: [null, Validators.compose([Validators.required, Validators.maxLength(10),
      Validators.minLength(10), Validators.pattern(this.onlyNumbers)])]
    })

    var userClaimsDetail = JSON.parse(localStorage.getItem('userClaims'));
    this.isInstantTenantManager = userClaimsDetail.IsInstanceTenantManager.toLocaleLowerCase() == 'true' ? true : false;
    this.isTenantGroupManager = userClaimsDetail.IsTenantGroupManager.toLocaleLowerCase() == 'true' ? true : false;
    this.TenantCode = userClaimsDetail.TenantCode != null ? userClaimsDetail.TenantCode : '';
    this.getCountries();
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
    copy.forEach(c => {
      this.countrycodeLists.push(c);
    })
    this.getProfileRecord();
  }

  async getProfileRecord() {
    let service = this.injector.get(UserService);
    let searchParameter: any = {};
    searchParameter.Identifier = this.UserIdentifier;
    searchParameter.PagingParameter = {};
    searchParameter.PagingParameter.PageIndex = Constants.DefaultPageIndex;
    searchParameter.PagingParameter.PageSize = Constants.DefaultPageSize;
    searchParameter.SortParameter = {};
    searchParameter.SortParameter.SortColumn = "Id";
    searchParameter.SortParameter.SortOrder = Constants.Ascending;
    searchParameter.SearchMode = Constants.Exact;
    searchParameter.IsSkipSuperAdmin = false;
    searchParameter.IsInstanceManager = this.isInstantTenantManager == true ? true : false;
    searchParameter.IsGroupManager = this.isTenantGroupManager == true ? true : false;
    if(this.isTenantGroupManager == true) {
      searchParameter.TenantCode = this.TenantCode;
    }
    this.uiLoader.start();
    var response = await service.getUser(searchParameter);
    this.usersList = response.usersList
    if (this.usersList.length > 0) {
      this.userRecord = this.usersList[0]
      this.userIdentifier = this.userRecord.Identifier;
      this.userRecord.roleName = this.usersList[0].Roles[0].Name;
      this.profileEditModeOn = true;
      let mobileNo, countryDiallingCode;
      let mobileNoArr = this.userRecord.ContactNumber.split('-');
      if (mobileNoArr.length > 1) {
        countryDiallingCode = mobileNoArr[0];
        mobileNo = mobileNoArr[1];
      }
      else {
        mobileNo = this.userRecord.MobileNumber;
      }
      let country = this.countrycodeLists.filter(i => { return i.Identifier == this.userRecord.CountryId });
      let countrycode = country != null ? country[0].Identifier : 0;
      this.profileFormGroup.patchValue({
        firstName: this.userRecord.FirstName,
        lastName: this.userRecord.LastName,
        email: this.userRecord.EmailAddress,
        mobileNumber: mobileNo,
        CountryCode: countrycode
      })
      this.profileFormGroup.controls['CountryCode'].setValue(countrycode);
      this.image = this.userRecord.Image;
      localStorage.setItem("currentUserTheme", this.userRecord.Theme);
      if (this.userRecord.Theme == 1) {
        this.theme1();
      }
      else if (this.userRecord.Theme == 2) {
        this.theme2();
      }
      else if (this.userRecord.Theme == 3) {
        this.theme3();
      }
      else if (this.userRecord.Theme == 4) {
        this.theme4();
      }
      else if (this.userRecord.Theme == 5) {
        this.theme5();
      }
      else if (this.userRecord.Theme == 0) {
        this.theme0();
      }
    }
  }
  
  public onCountrySelected(event) {
    const value = event.target.value;
    if (value == "") {
      this.profileFormErrorObject.showCountryCodeError = true;
      this.User.CountryCode = ''
    }
    else {
      this.profileFormErrorObject.showCountryCodeError = false;
      this.User.CountryCode = value
    }
  }

  onFileChanged(event) {
    let file = event.target.files[0];
    if (file.size > 200000) {
      this.profileFormErrorObject.showProfilePictureSizeError = true;
      return false;
    }
    this.profileFormErrorObject.showProfilePictureSizeError = false;
    var pattern = /image-*/;
    if (!file.type.match(pattern)) {
      this.profileFormErrorObject.showProfilePictureTypeError = true;
      return false;
    }
    this.profileFormErrorObject.showProfilePictureTypeError = false;
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

  //custom validation check
  profileFormValidaton(): boolean {
    this.profileFormErrorObject.showProfilePictureSizeError = false;
    this.profileFormErrorObject.showUserFirstNameError = false;
    this.profileFormErrorObject.showUserLastNameError = false;
    this.profileFormErrorObject.showUserCodeError = false;
    this.profileFormErrorObject.showUserEmailError = false;
    this.profileFormErrorObject.showCountryCodeError = false;
    this.profileFormErrorObject.showUserMobileNumberError = false;

    if (this.imageSize > 200000) {
      this.profileFormErrorObject.showProfilePictureSizeError = true;
      return false;
    }
    if (this.profileFormGroup.controls.firstName.invalid) {
      this.profileFormErrorObject.showUserFirstNameError = true;
      return false;
    }
    if (this.profileFormGroup.controls.lastName.invalid) {
      this.profileFormErrorObject.showUserLastNameError = true;
      return false;
    }
    if (this.profileFormGroup.controls.mobileNumber.invalid) {
      this.profileFormErrorObject.showUserMobileNumberError = true;
      return false;
    }
    return true;
  }

  saveButtonValidation(): boolean {
    if (this.imageSize > 200000) {
      return true;
    }
    if (this.profileFormErrorObject.showProfilePictureSizeError) {
      return true;
    }
    if (this.profileFormErrorObject.showProfilePictureTypeError) {
      return true;
    }

    if (this.profileFormGroup.controls.firstName.invalid) {
      return true;
    }
    if (this.profileFormGroup.controls.lastName.invalid) {
      return true;
    }
    if (this.profileFormGroup.controls.email.invalid) {
      return true;
    }
    if (this.profileFormGroup.controls.mobileNumber.invalid) {
      return true;
    }
    if (this.profileFormGroup.value.CountryCode == '') {
      return true;
    }
    return false;
  }

  //save functionality.
  onSubmit() {
    if (this.profileFormValidaton()) {
      this.userRecord.FirstName = this.profileFormGroup.value.firstName.trim();
      this.userRecord.LastName = this.profileFormGroup.value.lastName.trim();
      this.userRecord.EmailAddress = this.profileFormGroup.value.email;
      this.userRecord.ContactNumber = this.profileFormGroup.value.mobileNumber,
      this.userRecord.CountryId = this.profileFormGroup.value.CountryCode,
      this.userRecord.Image = this.image,
      this.saveRecord(this.userRecord);
    }
  }
 
  //Api called here to save record

  async saveRecord(profileObject) {
    var formData = new FormData()
    var UserArr: any = [];
    UserArr.push(profileObject);

    let userService = this.injector.get(UserService);
    this.uiLoader.start();
    userService.saveUser(UserArr, this.profileEditModeOn).subscribe(data => {
      this.uiLoader.stop();
      if (data == true) {
        let message = Constants.recordAddedMessage;
        if (this.profileEditModeOn) {
          message = Constants.recordUpdatedMessage;
        }
        this._messageDialogService.openDialogBox('Success', message, Constants.msgBoxSuccess);
        localStorage.removeItem('currentUserName');
        let newUserName = profileObject.FirstName + ' ' + profileObject.LastName;
        localStorage.setItem("currentUserName", newUserName);
        this.getProfileRecord();
        //const router = this.injector.get(Router);
        //router.navigate(['dashboard']);
      }
    }, (error: HttpResponse<any>) => {
      this.uiLoader.stop();
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

  //Functions call to click the theme of the page--
  theme1() {
    const dom: any = document.querySelector('body');
    dom.classList.add('theme1');
    dom.classList.remove('theme2');
    dom.classList.remove('theme3');
    dom.classList.remove('theme4');
    dom.classList.remove('theme5');
    dom.classList.remove('theme0');
    this.isTheme1Active = true;
    this.isTheme2Active = false;
    this.isTheme3Active = false;
    this.isTheme4Active = false;
    this.isTheme5Active = false;
    this.isTheme0Active = false;
  }

  theme2() {
    const dom: any = document.querySelector('body');
    dom.classList.add('theme2');
    dom.classList.remove('theme1');
    dom.classList.remove('theme3');
    dom.classList.remove('theme4');
    dom.classList.remove('theme5');
    dom.classList.remove('theme0');
    this.isTheme1Active = false;
    this.isTheme3Active = false;
    this.isTheme2Active = true;
    this.isTheme4Active = false;
    this.isTheme5Active = false;
    this.isTheme0Active = false;
  }

  theme3() {
    const dom: any = document.querySelector('body');
    dom.classList.remove('theme2');
    dom.classList.remove('theme1');
    dom.classList.add('theme3');
    dom.classList.remove('theme4');
    dom.classList.remove('theme5');
    dom.classList.remove('theme0');
    this.isTheme1Active = false;
    this.isTheme3Active = true;
    this.isTheme2Active = false;
    this.isTheme4Active = false;
    this.isTheme5Active = false;
    this.isTheme0Active = false;

  }

  theme4() {
    const dom: any = document.querySelector('body');
    dom.classList.remove('theme2');
    dom.classList.remove('theme1');
    dom.classList.remove('theme3');
    dom.classList.add('theme4');
    dom.classList.remove('theme5');
    dom.classList.remove('theme0');
    this.isTheme1Active = false;
    this.isTheme3Active = false;
    this.isTheme2Active = false;
    this.isTheme4Active = true;
    this.isTheme5Active = false;
    this.isTheme0Active = false;
  }

  theme5() {
    const dom: any = document.querySelector('body');
    dom.classList.remove('theme2');
    dom.classList.remove('theme1');
    dom.classList.remove('theme3');
    dom.classList.remove('theme4');
    dom.classList.add('theme5');
    dom.classList.remove('theme0');
    this.isTheme1Active = false;
    this.isTheme3Active = false;
    this.isTheme2Active = false;
    this.isTheme4Active = false;
    this.isTheme5Active = true;
    this.isTheme0Active = false;
  }

  theme0() {
    const dom: any = document.querySelector('body');
    dom.classList.remove('theme2');
    dom.classList.remove('theme1');
    dom.classList.remove('theme3');
    dom.classList.remove('theme4');
    dom.classList.remove('theme5');
    dom.classList.add('theme0');
    this.isTheme1Active = false;
    this.isTheme3Active = false;
    this.isTheme2Active = false;
    this.isTheme4Active = false;
    this.isTheme5Active = false;
    this.isTheme0Active = true;
  }

  backClicked() {
    if(this.isInstantTenantManager) {
      this.route.navigate(['tenantgroups']);
    }else if(this.isTenantGroupManager) {
      this.route.navigate(['tenants']);
    }else {
      this.route.navigate(['dashboard']);
    }
  }

}
