import { Component, OnInit, ViewChild, Injector } from '@angular/core';
import { Location } from '@angular/common';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';
import { __values } from 'tslib';
import { Constants } from 'src/app/shared/constants/constants';
import { UserService } from '../user.service';
import { HttpResponse, HttpEvent, HttpEventType } from '@angular/common/http';
import { HttpClient } from '@angular/common/http';
import { DialogService } from '@tomblue/ng2-bootstrap-modal';
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
import { Country } from '../../country/country';
import { CountryService } from '../../country/country.service';
@Component({
  selector: 'app-user-add-edit',
  templateUrl: './add-edit.component.html',
  styleUrls: ['./add-edit.component.scss']

})
export class UserAddEditComponent implements OnInit {
  userFormGroup: FormGroup;
  public emailRegex = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
  public onlyAlphabetsWithSpaceQuoteHyphen = "[a-zA-Z]+[ ]{0,1}[a-zA-Z]*[ ]*$";
  public onlyCharacterswithInbetweenSpaceUpto50Characters = Constants.onlyCharacterswithInbetweenSpaceUpto50Characters;
  public onlyNumbers = '[0-9]*';
  public errorMsg: boolean;
  public roleList = [{ "Name": "Select Role", "Identifier": 0 }];
  public roleLists: any = [];
  public userRoleLists = [];
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
  public userId;
  public User = { RoleIdentifier: 0, Image: '', CountryCode: 0 }
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
    showProfilePictureTypeError: false,
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


  //Initialization call--onlyNumbers
  ngOnInit() {
    // User form validations.
    this.userFormGroup = this.formBuilder.group({
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
      UserRole: [0, Validators.compose([Validators.required])],
      CountryCode: [0, Validators.compose([Validators.required])],
      Image: [null]
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
    //this.spinner.start();
    var copy = await this.loginService.getRoles(searchParameter);
    //this.spinner.stop();
    copy.forEach(role => {
      this.roleList.push(role);
    })
    this.getCountries();
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
    //this.countryList.forEach(c => {
    //  this.roleList.push({ "CountryNameCode": "Please select", "DialingCode": "" });
    //})
    //{ "CountryNameCode": "Please select", "DialingCode": "" ,"Identifier"},
    //{ "CountryNameCode": "IND +91", "DialingCode": "+91" },
    //{ "CountryNameCode": "USA +1", "DialingCode": "+1" },
    //{ "CountryNameCode": "UAE +971", "DialingCode": "+971" },
    //{ "CountryNameCode": "ZAF +27", "DialingCode": "+27" }
    if (this.userEditModeOn) {
      this.fillUserDetail();

    }
  }

  onFileChanged(event) {
    let file = event.target.files[0];
    if (file.size > 200000) {
      this.userFormErrorObject.showProfilePictureSizeError = true;
      return false;
    }

    this.userFormErrorObject.showProfilePictureSizeError = false;

    var pattern = /image-*/;

    if (!file.type.match(pattern)) {

      this.userFormErrorObject.showProfilePictureTypeError = true;

      return false;
    }
    this.userFormErrorObject.showProfilePictureTypeError = false;

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
  userFormValidaton() {
    this.userFormErrorObject.showProfilePictureSizeError = false;
    this.userFormErrorObject.showUserFirstNameError = false;
    this.userFormErrorObject.showUserLastNameError = false;
    this.userFormErrorObject.showUserEmailError = false;
    this.userFormErrorObject.showCountryCodeError = false;
    this.userFormErrorObject.showUserMobileNumberError = false;
    this.userFormErrorObject.roleShowError = false;
    if (this.imageSize > 200000) {
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
    if (this.User.CountryCode == 0) {
      this.userFormErrorObject.showCountryCodeError = true;
      return false;
    }
    return true;
  }

  saveButtonValidation(): boolean {

    if (this.imageSize > 200000) {
      return true;
    }
    if (this.userFormErrorObject.showProfilePictureSizeError) {
      return true;
    }
    if (this.userFormErrorObject.showProfilePictureTypeError) {
      return true;
    }

    if (this.userFormGroup.controls.firstName.invalid) {
      return true;
    }
    if (this.userFormGroup.controls.lastName.invalid) {
      return true;

    }
    if (this.userFormGroup.controls.email.invalid) {
      return true;
    }

    if (this.userFormGroup.controls.mobileNumber.invalid) {
      return true;
    }

    if (this.User.RoleIdentifier == 0) {
      return true;
    }
    if (this.User.CountryCode == 0) {
      return true;
    }
    return false;
  }

  public onRoleSelected(event) {

    const value = event.target.value;
    if (value == "0") {
      this.userFormErrorObject.roleShowError = true;
      this.User.RoleIdentifier = 0;

    }
    else {
      this.userFormErrorObject.roleShowError = false;

      this.User.RoleIdentifier = Number(value);

    }
  }
  public onCountrySelected(event) {

    const value = event.target.value;
    if (value == "") {
      this.userFormErrorObject.showCountryCodeError = true;
      this.User.CountryCode = 0
    }
    else {
      this.userFormErrorObject.showCountryCodeError = false;

      this.User.CountryCode = value

    }

    console.log(value);
  }

  //Function to add user--
  onSubmit() {
    if (this.userFormValidaton()) {

      let selectedroleArray = [];
      this.roleList.forEach(role => {
        if (role.Identifier == this.User.RoleIdentifier) {
          selectedroleArray.push({
            "Identifier": role.Identifier,
            "Name": role.Name,
          });
        }
      })
      let userObject: any = {
        "FirstName": this.userFormGroup.value.firstName.trim(),
        "LastName": this.userFormGroup.value.lastName.trim(),
        "EmailAddress": this.userFormGroup.value.email,
        //"ContactNumber": this.userFormGroup.value.CountryCode + "-" + this.userFormGroup.value.mobileNumber,
        "ContactNumber": this.userFormGroup.value.mobileNumber,
        "CountryId": this.userFormGroup.value.CountryCode,
        "Roles": selectedroleArray,
        "Image": this.image,
      }
      if (this.userEditModeOn) {
        userObject.Identifier = this.params.Routeparams.passingparams.UserIdentifier;
        userObject.IsActive = true;
        userObject.IsLocked = false;
      }
      else {
        userObject.IsActive = false;
        userObject.IsLocked = true;
      }
      //console.log(userObject)
      this.saveRecord(userObject);
    }
  }

  //Api called here to save record
  saveRecord(userObject) {
    var formData = new FormData()
    var UserArr = [];
    UserArr.push(userObject);

    this.spinner.start();
    this.service.saveUser(UserArr, this.userEditModeOn).subscribe(data => {
      this.spinner.stop();
      if (data == true) {
        let message = "User added successfully.Please check your email to activate user and set password";
        if (this.userEditModeOn) {
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
    this.router.navigate(['user']);
  }

  //Function to patch/set user details at edit mode--
  async fillUserDetail() {
    this.params = JSON.parse(localStorage.getItem('userRouteparams'));
    this.UserIdentifier = this.params.Routeparams.passingparams.UserIdentifier
    let userService = this.injector.get(UserService);
    let searchParameter: any = {};
    searchParameter.IsRolePrivilegesRequired = true;
    searchParameter.Identifier = this.UserIdentifier;
    searchParameter.PagingParameter = {};
    searchParameter.PagingParameter.PageIndex = Constants.DefaultPageIndex;
    searchParameter.PagingParameter.PageSize = Constants.DefaultPageSize;
    searchParameter.SortParameter = {};
    searchParameter.SortParameter.SortColumn = "Id";
    searchParameter.SortParameter.SortOrder = Constants.Ascending;
    searchParameter.SearchMode = Constants.Contains;
    //this.spinner.start();

    var response = await userService.getUser(searchParameter);
    this.usersList = response.usersList;

    //this.spinner.stop();
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
      this.FirstChar = userObject.FirstName.charAt(0);
      this.SecondChar = userObject.LastName.charAt(0);

      this.userRoleLists = userObject.Roles;
      let mobileNo, countryDiallingCode;
      let mobileNoArr = userObject.ContactNumber.split('-');
      if (mobileNoArr.length > 1) {
        countryDiallingCode = mobileNoArr[0];
        mobileNo = mobileNoArr[1];
      }
      else {
        mobileNo = userObject.MobileNumber;
      }
      let country = this.countrycodeLists.filter(i => { return i.Identifier == userObject.CountryId });
      this.userFormGroup.patchValue({
        firstName: userObject.FirstName,
        lastName: userObject.LastName,
        email: userObject.EmailAddress,
        mobileNumber: mobileNo,
        UserRole: this.userRoleLists[0].Identifier,
        CountryCode: country[0].Identifier
      })
      this.userFormGroup.controls['UserRole'].setValue(this.userRoleLists[0].Identifier);
      this.userFormGroup.controls['CountryCode'].setValue(country[0].Identifier);
      this.User.RoleIdentifier = this.userRoleLists[0].Identifier;
      this.User.CountryCode = country[0].Identifier;
      this.activeSlider = userObject.IsActive;
      this.lockSlider = userObject.IsLocked;
      this.image = userObject.Image;
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
