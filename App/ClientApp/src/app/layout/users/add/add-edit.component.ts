import { Component, OnInit, ViewChild, Injector } from '@angular/core';
import { Location } from '@angular/common';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';
import { __values } from 'tslib';
import { Constants } from 'src/app/shared/constants/constants';
import { UserService } from '../user.service';
import { HttpResponse, HttpEvent, HttpEventType } from '@angular/common/http';
import { HttpClient } from '@angular/common/http';
import { DialogService } from 'ng2-bootstrap-modal';
import { MsgBoxComponent } from 'src/app/shared/modules/message/messagebox.component';
import { Router, NavigationEnd } from '@angular/router';
import { jqxDropDownListComponent } from 'jqwidgets-ng/jqxdropdownlist';
import { NgxUiLoaderService } from 'ngx-ui-loader';
import { URLConfiguration } from 'src/app/shared/urlConfiguration/urlconfiguration';
import { MessageDialogService } from 'src/app/shared/services/mesage-dialog.service';
import { LocalStorageService } from 'src/app/shared/services/local-storage.service';
import { ConfigConstants } from 'src/app/shared/constants/configConstants';
import { SortOrder } from 'src/app/shared/enums/sort-order.enum';
import { SearchMode } from 'src/app/shared/enums/search-mode.enum';
import { LoginService } from '../../../login/login.service';
@Component({
  selector: 'app-user-add-edit',
  templateUrl: './add-edit.component.html',

})
export class UserAddEditComponent implements OnInit {
  userFormGroup: FormGroup;
  public emailRegex = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
  public onlyAlphabetsWithSpaceQuoteHyphen = "[a-zA-Z-0-9' ]*";
  public onlyNumbers = '[0-9]*';
  public errorMsg: boolean;
  public roleList;
  public roleLists: any = [];
  public userRoleLists = [];
  public source: string[] = [];
  public countrycodeList = [];
  public countrycodeLists = [];
  public countrycodesource = [];
  public selectedAll: any;
  public allRoles;
  public userId;
  public User = {RoleIdentifier:0}
  public addUser: boolean = true;
  public editUser: boolean;
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
  public userEditModeOn: boolean = false;
  public isCollapsedDetails: boolean = false;
  public isCollapsedRoles: boolean = true;
  public params;
  public UserIdentifier;
  public UserName;
  public userAddEditResources = {}
  public userRoleFilterForm: FormGroup;
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
  public usersList = [];
  public designationList = [];
  public designationLists: any = [];
  public designationListSource: any = [];
  public languageList = [];
  public languageLists: any = [];
  public languageSource: any[] = []
  public selectedImages = [];
  public treeData: any = [];
  public profileImage;
  public userImage;
  public resourceId;
  public userClaimsRolePrivilegeOperations = [];
  public isOrganizationDataRetrieved: boolean = false;
  public isRoleDataRetrieved: boolean = false;
  public isDestinationDataRetrieved: boolean = false;
  public isLanguageDataRetrieved: boolean = false;
  public isCountryDataRetrieved: boolean = false;
  public fileArray = [];
  public Theme = 6;

  // Object created to initlialize the error boolean value.
  public userFormErrorObject: any = {
    showProfilePictureSizeError: false,
    showUserFirstNameError: false,
    showUserLastNameError: false,
    showUserCodeError: false,
    showUserEmailError: false,
    showCountryCodeError: false,
    showUserMobileNumberError: false,
  };

  //getters of usersForm group
  get firstName() {
    return this.userFormGroup.get('firstName');
  }
  get lastName() {
    return this.userFormGroup.get('lastName');
  }
  get code() {
    return this.userFormGroup.get('code');
  }
  get email() {
    return this.userFormGroup.get('email');
  }
  get countryCode() {
    return this.userFormGroup.get('countryCode');
  }
  get mobileNumber() {
    return this.userFormGroup.get('mobileNumber');
  }

  get filterRoleName() {
    return this.userRoleFilterForm.get('filterRoleName');
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
    private service: UserService,
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
        if (e.url.includes('/user/userAdd')) {
          this.userEditModeOn = false;
          localStorage.removeItem("userRouteparams");
        }
      }

    });

    if (localStorage.getItem("userRouteparams")) {
      this.userEditModeOn = true;
    } else {
      this.userEditModeOn = false;
    }

    router.events.subscribe(e => {
      if (e instanceof NavigationEnd) {
        if (e.url.includes('/user')) {
          //set passing parameters to localstorage.
          this.params = JSON.parse(localStorage.getItem('userRouteparams'));
          if (localStorage.getItem('userRouteparams')) {
            this.UserIdentifier = this.params.Routeparams.passingparams.UserIdentifier;
            //this.UserName = this.params.Routeparams.filteredparams.UserName
          }
        } else {
          localStorage.removeItem("userRouteparams");
        }
      }

    });
  }


  //Initialization call--
  ngOnInit() {
    // User form validations.
    this.userFormGroup = this.formBuilder.group({
      firstName: [null, Validators.compose([Validators.required,
      Validators.pattern(this.onlyAlphabetsWithSpaceQuoteHyphen)])],
      lastName: [null, Validators.compose([Validators.required,
      Validators.pattern(this.onlyAlphabetsWithSpaceQuoteHyphen)])],
      code: [null, Validators.compose([Validators.required])],
      email: [null, Validators.compose([Validators.required,
      Validators.pattern(this.emailRegex)])],
      countryCode: [null, Validators.required],
      mobileNumber: [null, Validators.compose([Validators.required,
      Validators.maxLength(10),
      Validators.minLength(10),
      Validators.pattern(this.onlyNumbers)])],
      orgnisationUnit: [null],
      designation: [null],
      preferredLanguage: [null],
      profilePictire: [null]
    })
   
    this.getRole();
  }

  //Function to get role--
  async getRole() {
    let searchParameter: any = {};
    searchParameter.PagingParameter = {};
    searchParameter.PagingParameter.PageIndex = Constants.DefaultPageIndex;
    searchParameter.PagingParameter.PageSize = Constants.DefaultPageSize;
    searchParameter.SortParameter = {};
    searchParameter.SortParameter.SortColumn = Constants.Name;
    searchParameter.SortParameter.SortOrder = Constants.Ascending;
    searchParameter.SearchMode = Constants.Contains;
    //searchParameter.GetPrivileges = true;
    this.roleList = await this.loginService.getRoles(searchParameter);
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
    this.spinner.start();
    if (searchType == 'reset') {
      this.userRoleFilterForm.patchValue({
        filterRoleName: null
      })
      //this.getRole();
    }
    else {
      let searchParameter: any = {};
      searchParameter.RoleName = this.userRoleFilterForm.value.filterRoleName != null ? this.userRoleFilterForm.value.filterRoleName : "";
      searchParameter.PagingParameter = {};
      searchParameter.PagingParameter.PageIndex = Constants.DefaultPageIndex;
      searchParameter.PagingParameter.PageSize = Constants.DefaultPageSize;
      searchParameter.SortParameter = {};
      searchParameter.SortParameter.SortColumn = Constants.UserName;
      searchParameter.SortParameter.SortOrder = Constants.Ascending;
      searchParameter.SearchMode = Constants.Contains;

      //this.getRole();
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
  userFormValidaton(): boolean {
    this.userFormErrorObject.showProfilePictureSizeError = false;
    this.userFormErrorObject.showUserFirstNameError = false;
    this.userFormErrorObject.showUserLastNameError = false;
    this.userFormErrorObject.showUserEmailError = false;
    this.userFormErrorObject.showCountryCodeError = false;
    this.userFormErrorObject.showUserMobileNumberError = false;
    this.userFormErrorObject.roleShowError = false
    if (this.imageSize > 4194304) {
      this.userFormErrorObject.showProfilePictureSizeError = true;
      return false;
    }
    if (this.userFormGroup.controls.firstName.invalid) {
      this.userFormErrorObject.showUserFirstNameError = true;
      return false;
    }
    if (this.userFormGroup.controls.lastName.invalid) {
      this.userFormErrorObject.showUserLastNameError = true;
      return false;
    }
    if (this.userFormGroup.controls.code.invalid) {
      this.userFormErrorObject.showUserCodeError = true;
      return false;
    }
    if (this.userFormGroup.controls.email.invalid) {
      this.userFormErrorObject.showUserEmailError = true;
      return false;
    }
   
    if (this.userFormGroup.controls.mobileNumber.invalid) {
      this.userFormErrorObject.showUserMobileNumberError = true;
      return false;
    }
    
    if (this.User.RoleIdentifier == 0) {
      this.userFormErrorObject.roleShowError = true;
      return false;
    }
    return true;
  }

  //Function to add user--
  onSubmit() {
    if (this.userFormValidaton()) {
      var selectedCountryCode = "";
      

      let selectedroleArray = [];
      this.roleLists.forEach(role => {
        if (role.IsEnabled == true) {
          selectedroleArray.push({
            "Identifier": role.Identifier,
            "Name": role.Name,
            "Status": role.Status
          });
        }
      })
      let selectedLang: any = {}
      let userObject: any = {
        "FirstName": this.userFormGroup.value.firstName,
        "LastName": this.userFormGroup.value.lastName,
        "EmailAddress": this.userFormGroup.value.email,
        "MobileNumber": selectedCountryCode + '-' + this.userFormGroup.value.mobileNumber,
        "Roles": selectedroleArray,
        "PreferredLanguage": selectedLang,
      }
      if (this.userEditModeOn) {
        userObject.Identifier = this.params.Routeparams.passingparams.UserIdentifier;
        userObject.ResourceIdentifier = this.resourceId;
        userObject.IsActive = false;
        userObject.IsLocked = false;
      }
      else {
        userObject.IsActive = this.activeSlider;
        userObject.IsLocked = this.lockSlider;
      }
      //console.log(userObject)
      this.saveRecord(userObject);
    }
  }

  //Api called here to save record
  saveRecord(userObject) {
    var formData = new FormData()
    var UserArr: any = [];
    var userObj: any = {};
    userObj.UserData = userObject;
    userObj.Resources = [];
    UserArr.push(userObj);
    for (var j = 0; j < this.fileArray.length; j++) {
      var resourceObj: any = {};
      resourceObj.Identifier = j;
      resourceObj.File = this.fileArray[j];
      UserArr[0].Resources.push(resourceObj);
    }
    for (var i = 0; i < UserArr.length; i++) {
      formData.append('Users[' + i + '][UserData]', JSON.stringify(UserArr[i].UserData));
      for (var j = 0; j < UserArr[i].Resources.length; j++) {
        formData.append('Users[' + i + '][Resources][' + j + '][Identifier]', UserArr[i].Resources[j].Identifier);
        formData.append('Users[' + i + '][Resources][' + j + '][File]', UserArr[i].Resources[j].File);
      }
    }
    //debugger
    this.spinner.start();
    this.service.saveUser(formData, this.userEditModeOn).subscribe(data => {
      this.spinner.stop();
      if (data == true) {
        let message = Constants.recordAddedMessage;
        if (this.userEditModeOn && localStorage.getItem('userRouteparams')) {
          message = Constants.recordUpdatedMessage;
        }
        this._messageDialogService.openDialogBox('Success', message, Constants.msgBoxSuccess);
        this.navigateToListPage();
      }
    }, (error: HttpResponse<any>) => {
      this.spinner.stop();
    });
  }

  //Back Functionality.
  navigateToListPage() {
    this.router.navigate(['user']);
  }

  //Function to patch/set user details at edit mode--
  async fillUserDetail() {
    this.params = JSON.parse(localStorage.getItem('userRouteparams'));
    this.UserIdentifier = this.params.Routeparams.passingparams.UserIdentifier
    let userService = this.injector.get(UserService);
    let searchParameter: any = {};
    searchParameter.GetResources = true;
    searchParameter.GetRoles = true;
    searchParameter.GetOrganisationUnits = true;
    searchParameter.Identifiers = this.UserIdentifier;
    searchParameter.PagingParameter = {};
    searchParameter.PagingParameter.PageIndex = Constants.DefaultPageIndex;
    searchParameter.PagingParameter.PageSize = Constants.DefaultPageSize;
    searchParameter.SortParameter = {};
    searchParameter.SortParameter.SortColumn = Constants.UserCode;
    searchParameter.SortParameter.SortOrder = Constants.Ascending;
    searchParameter.SearchMode = Constants.Contains;
    this.spinner.stop();
    this.usersList = await userService.getUser(searchParameter);
    this.usersList.forEach(userObject => {
      this.resourceId = userObject.ResourceIdentifier
      this.previewUrl = [];
      if (userObject.ProfileImage) {
        if (userObject.ProfileImage.URL) {
          this.profileImage = userObject.ProfileImage.URL;
          this.previewUrl.push(this.profileImage);
          this.isImageLoded = true
          this.showDefaultLogo = false
        }
      }
      this.userRoleLists = userObject.Roles;
      let mobileNo, countryDiallingCode;
      let mobileNoArr = userObject.MobileNumber.split('-');
      if (mobileNoArr.length > 1) {
        countryDiallingCode = mobileNoArr[0];
        mobileNo = mobileNoArr[1];
      }
      else {
        mobileNo = userObject.MobileNumber;
      }
      this.userFormGroup.patchValue({
        firstName: userObject.FirstName,
        lastName: userObject.LastName,
        code: userObject.Code,
        email: userObject.EmailAddress,
        mobileNumber: mobileNo,
      })
      var selectedCountryIdentifier;
      this.countrycodeLists.forEach(country => {
        if (country.DialingCode == countryDiallingCode) {
          selectedCountryIdentifier = country.Identifier;
        }
      })
      if (selectedCountryIdentifier)
      this.activeSlider = userObject.IsActive;
      this.lockSlider = userObject.IsLocked;
      this.allowAccessSlider = userObject.AllowAccessOnOperatorConsole;
      this.notificationSlider = userObject.ReceiveAlertNotifications;
      this.Theme = userObject.Theme;
      //ou
     
      //role
      setTimeout(() => {
        this.userRoleLists.forEach(userRole => {
          this.roleLists.forEach(role => {
            if (userRole.Identifier == role.Identifier) {
              role.IsEnabled = true;
            }
          })
        })
      }, 2000);
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
