import { Component, OnInit, ViewChild, Injector } from '@angular/core';
import { Location } from '@angular/common';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';
import { __values } from 'tslib';
import { Constants } from 'src/app/shared/constants/constants';
import { TenantUserService } from '../../tenantuser/tenantuser.service';
import { Router, NavigationEnd } from '@angular/router';
import { NgxUiLoaderService } from 'ngx-ui-loader';
import { MessageDialogService } from 'src/app/shared/services/mesage-dialog.service';
import { CountryService } from '../../country/country.service';
import { RoleService } from '../../roles/role.service';
import { ConfigConstants } from 'src/app/shared/constants/configConstants';
import { LocalStorageService } from 'src/app/shared/services/local-storage.service';

@Component({
  selector: 'app-add-edit',
  templateUrl: './add-edit.component.html',
  styleUrls: ['./add-edit.component.scss']
})
export class AddEditComponent implements OnInit {

  tenantgroupuserFormGroup: FormGroup;
  public emailRegex = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
  public onlyAlphabetsWithSpaceQuoteHyphen = "[a-zA-Z]+[ ]{0,1}[a-zA-Z]*[ ]*$";
  public onlyCharacterswithInbetweenSpaceUpto50Characters = Constants.onlyCharacterswithInbetweenSpaceUpto50Characters;
  public onlyNumbers = '[0-9]*';
  public errorMsg: boolean;
  public roleList = [];
  public countrycodeLists = [{ "Identifier": 0, "Code": "Please Select", "DialingCode": "" }];
  image: string;
  FirstChar: string;
  SecondChar: string
  public selectedAll: any;
  public allRoles;
  public tenantuserId;
  public TenantGroupUser = { RoleIdentifier: 0, Image: '', CountryCode: 0 }
  public tenantgroupuserEditModeOn: boolean = false;
  public params;
  public UserIdentifier;
  public fileData: any = null;
  public previewUrl: any = [];
  public fileUploadProgress: string = null;
  public uploadedFilePath: string = null;
  public isImageLoded: boolean = false;
  public showDefaultLogo: boolean = true;
  public imageSize: number;
  public imageType = "";
  public imageName = "";
  public tenantgroupusersList = [];
  public profileImage;
  public tenantuserImage;
  public resourceId;
  public fileArray = [];
  public Theme = 6;
  public roleObject:any = {};
  public TenantCode = '';

  // Object created to initlialize the error boolean value.
  public tenantgroupuserFormErrorObject: any = {
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
    return this.tenantgroupuserFormGroup.get('firstName');
  }
  get lastName() {
    return this.tenantgroupuserFormGroup.get('lastName');
  }
  get code() {
    return this.tenantgroupuserFormGroup.get('code');
  }
  get email() {
    return this.tenantgroupuserFormGroup.get('email');
  }
  get countryCode() {
    return this.tenantgroupuserFormGroup.get('countryCode');
  }
  get mobileNumber() {
    return this.tenantgroupuserFormGroup.get('mobileNumber');
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
    private service: TenantUserService,
    private spinner: NgxUiLoaderService,
    private router: Router,
    private _messageDialogService: MessageDialogService,
    private injector: Injector,
    private localstorageservice: LocalStorageService,
  ) {
    router.events.subscribe(e => {
      if (e instanceof NavigationEnd) {
        if (e.url.includes('/tenantgroupusers/add')) {
          this.tenantgroupuserEditModeOn = false;
          localStorage.removeItem("tenantgroupuserRouteparams");
        }
      }
    });

    if (localStorage.getItem("tenantgroupuserRouteparams")) {
      this.tenantgroupuserEditModeOn = true;
    } else {
      this.tenantgroupuserEditModeOn = false;
    }

    router.events.subscribe(e => {
      if (e instanceof NavigationEnd) {
        if (e.url.includes('/tenantgroupusers')) {
          //set passing parameters to localstorage.
          this.params = JSON.parse(localStorage.getItem('tenantgroupuserRouteparams'));
          if (localStorage.getItem('tenantgroupuserRouteparams')) {
            this.UserIdentifier = this.params.Routeparams.passingparams.UserIdentifier;
          }
        } else {
          localStorage.removeItem("tenantgroupuserRouteparams");
        }
      }
    });
  }

  ngOnInit() {

    var userClaimsDetail = JSON.parse(localStorage.getItem('userClaims'));
    if (userClaimsDetail) {
      if(userClaimsDetail.IsTenantGroupManager == null || userClaimsDetail.IsTenantGroupManager.toLocaleLowerCase() != 'true') {
        this.localstorageservice.removeLocalStorageData();
        this.router.navigate(['login']);
      }
    }
    else {
      this.localstorageservice.removeLocalStorageData();
      this.router.navigate(['login']);
    }
    this.TenantCode = userClaimsDetail.TenantCode;

    // TenantGroupUser form validations.
    this.tenantgroupuserFormGroup = this.formBuilder.group({
      firstName: [null, Validators.compose([Validators.required, Validators.minLength(2), Validators.maxLength(50),
      Validators.pattern(this.onlyCharacterswithInbetweenSpaceUpto50Characters)])],
      lastName: [null, Validators.compose([Validators.required, Validators.minLength(2), Validators.maxLength(50),
      Validators.pattern(this.onlyCharacterswithInbetweenSpaceUpto50Characters)])],
      email: ['', Validators.compose([Validators.required, Validators.pattern(this.emailRegex)])],
      mobileNumber: ['', Validators.compose([Validators.maxLength(10), Validators.minLength(10), Validators.pattern(this.onlyNumbers)])],
      CountryCode: [0],
      Image: [null]
    })
    this.getCountries();
    this.getTenantGroupManagerRole();
  }

  //Function to get role--
  async getTenantGroupManagerRole() {
    let roleService = this.injector.get(RoleService);
    let searchParameter: any = {};
    searchParameter.PagingParameter = {};
    searchParameter.PagingParameter.PageIndex = Constants.DefaultPageIndex;
    searchParameter.PagingParameter.PageSize = Constants.DefaultPageSize;
    searchParameter.SortParameter = {};
    searchParameter.SortParameter.SortColumn = "Id";
    searchParameter.SortParameter.SortOrder = Constants.Ascending;
    searchParameter.SearchMode = Constants.Contains;
    searchParameter.Name = "Group Manager";
    searchParameter.TenantCode = ConfigConstants.TenantCode;
    var response = await roleService.getRoles(searchParameter);
    let _list = response.roleList;
    this.roleObject = _list[0];
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
   
    if (this.tenantgroupuserEditModeOn) {
      this.fillTenantGroupUserDetail();
    }
  }

  onFileChanged(event) {
    let file = event.target.files[0];
    if (file.size > 200000) {
      this.tenantgroupuserFormErrorObject.showProfilePictureSizeError = true;
      return false;
    }
    this.tenantgroupuserFormErrorObject.showProfilePictureSizeError = false;
    var pattern = /image-*/;
    if (!file.type.match(pattern)) {
      this.tenantgroupuserFormErrorObject.showProfilePictureTypeError = true;
      return false;
    }
    this.tenantgroupuserFormErrorObject.showProfilePictureTypeError = false;
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
  tenantgroupuserFormValidaton() {
    this.tenantgroupuserFormErrorObject.showProfilePictureSizeError = false;
    this.tenantgroupuserFormErrorObject.showTenantUserFirstNameError = false;
    this.tenantgroupuserFormErrorObject.showTenantUserLastNameError = false;
    this.tenantgroupuserFormErrorObject.showTenantUserEmailError = false;
    this.tenantgroupuserFormErrorObject.showCountryCodeError = false;
    this.tenantgroupuserFormErrorObject.showTenantUserMobileNumberError = false;
    this.tenantgroupuserFormErrorObject.roleShowError = false;
    if (this.imageSize > 200000) {
      this.tenantgroupuserFormErrorObject.showProfilePictureSizeError = true;
      return false;
    }
    if (this.tenantgroupuserFormGroup.controls.firstName.invalid) {
      this.tenantgroupuserFormErrorObject.showTenantUserFirstNameError = true;
      return false;
    }
    if (this.tenantgroupuserFormGroup.controls.lastName.invalid) {
      this.tenantgroupuserFormErrorObject.showTenantUserLastNameError = true;
      return false;
    }
    if (this.tenantgroupuserFormGroup.controls.email.invalid) {
      this.tenantgroupuserFormErrorObject.showTenantUserEmailError = true;
      return false;
    }

    if (this.tenantgroupuserFormGroup.controls.mobileNumber.invalid) {
      this.tenantgroupuserFormErrorObject.showTenantUserMobileNumberError = true;
      return false;
    }

    //if (this.TenantGroupUser.CountryCode == 0) {
    //  this.tenantgroupuserFormErrorObject.showCountryCodeError = true;
    //  return false;
    //}
    return true;
  }

  saveButtonValidation(): boolean {
    if (this.imageSize > 200000) {
      return true;
    }
    if (this.tenantgroupuserFormErrorObject.showProfilePictureSizeError) {
      return true;
    }
    if (this.tenantgroupuserFormErrorObject.showProfilePictureTypeError) {
      return true;
    }
    if (this.tenantgroupuserFormGroup.controls.firstName.invalid) {
      return true;
    }
    if (this.tenantgroupuserFormGroup.controls.lastName.invalid) {
      return true;
    }
    if (this.tenantgroupuserFormGroup.controls.email.invalid) {
      return true;
    }
    if (this.tenantgroupuserFormGroup.controls.mobileNumber.invalid) {
      return true;
    }
    //if (this.TenantGroupUser.CountryCode == 0) {
    //  return true;
    //}
    return false;
  }

  public onCountrySelected(event) {
    const value = event.target.value;
    if (value == "") {
      //this.tenantgroupuserFormErrorObject.showCountryCodeError = true;
      this.TenantGroupUser.CountryCode = 0
    }
    else {
      //this.tenantgroupuserFormErrorObject.showCountryCodeError = false;
      this.TenantGroupUser.CountryCode = value
    }
  }

  //Function to add TenantGroupUser--
  onSubmit() {
    if (this.tenantgroupuserFormValidaton()) {
      let selectedroleArray = [];
      selectedroleArray.push({
        "Identifier": this.roleObject.Identifier,
        "Name": this.roleObject.Name,
      });
      let tenantgroupuser: any = {
        "FirstName": this.tenantgroupuserFormGroup.value.firstName.trim(),
        "LastName": this.tenantgroupuserFormGroup.value.lastName.trim(),
        "EmailAddress": this.tenantgroupuserFormGroup.value.email,
        "ContactNumber": this.tenantgroupuserFormGroup.value.mobileNumber,
        "CountryId": this.tenantgroupuserFormGroup.value.CountryCode,
        "Roles": selectedroleArray,
        "Image": this.image,
      }
      if (this.tenantgroupuserEditModeOn) {
        tenantgroupuser.Identifier = this.params.Routeparams.passingparams.UserIdentifier;      
      }

      tenantgroupuser.IsActive = true;
      tenantgroupuser.IsLocked = false;
      this.saveRecord(tenantgroupuser);
    }
  }

  //Api called here to save record
  saveRecord(tenantgroupuser) {
    var formData = new FormData()
    var TenantGroupUserArr = [];
    tenantgroupuser.IsGroupManager = true;
    TenantGroupUserArr.push(tenantgroupuser);
    this.spinner.start();
    this.service.saveUser(TenantGroupUserArr, this.tenantgroupuserEditModeOn).subscribe(data => {
      this.spinner.stop();
      if (data == true) {
        let message = "User added successfully";
        if (this.tenantgroupuserEditModeOn) {
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
    this.router.navigate(['tenantgroupusers']);
  }

  //Function to patch/set TenantGroupUser details at edit mode--
  async fillTenantGroupUserDetail() {
    this.params = JSON.parse(localStorage.getItem('tenantgroupuserRouteparams'));
    this.UserIdentifier = this.params.Routeparams.passingparams.UserIdentifier;
    let tenantuserService = this.injector.get(TenantUserService);
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
    searchParameter.IsGroupManager = true;
    searchParameter.TenantCode = this.TenantCode;
    var response = await tenantuserService.getUser(searchParameter);
    this.tenantgroupusersList = response.usersList;
    this.tenantgroupusersList.forEach(tenantgroupuser => {
      this.resourceId = tenantgroupuser.ResourceIdentifier
      this.previewUrl = [];
      if (tenantgroupuser.ProfileImage) {
        if (tenantgroupuser.ProfileImage.URL) {
          this.profileImage = tenantgroupuser.ProfileImage.URL;
          this.previewUrl.push(this.profileImage);
          this.isImageLoded = true
          this.showDefaultLogo = false
        }
      }
      this.FirstChar = tenantgroupuser.FirstName.charAt(0);
      this.SecondChar = tenantgroupuser.LastName.charAt(0);

      let mobileNo, countryDiallingCode;
      let mobileNoArr = tenantgroupuser.ContactNumber.split('-');
      if (mobileNoArr.length > 1) {
        countryDiallingCode = mobileNoArr[0];
        mobileNo = mobileNoArr[1];
      }
      else {
        mobileNo = tenantgroupuser.MobileNumber;
      }
      let country = this.countrycodeLists.filter(i => { return i.Identifier == tenantgroupuser.CountryId });
      this.tenantgroupuserFormGroup.patchValue({
        firstName: tenantgroupuser.FirstName,
        lastName: tenantgroupuser.LastName,
        email: tenantgroupuser.EmailAddress,
        mobileNumber: mobileNo,
        CountryCode: country[0].Identifier
      })
      this.tenantgroupuserFormGroup.controls['CountryCode'].setValue(country[0].Identifier);
      this.TenantGroupUser.CountryCode = country[0].Identifier;
      this.image = tenantgroupuser.Image;
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
