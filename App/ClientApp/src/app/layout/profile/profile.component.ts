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
    public onlyAlphabetsWithSpaceQuoteHyphen = "[a-zA-Z0-9-' ]*";
    public onlyNumbers = '[0-9]*';
    public resourceId;
    public profileImage;
    public fileArray = [];
    public languageLists = [];
    public userRecord: any = {};
  
    public countrycodeLists = [
        { "CountryNameCode": "Please select", "DialingCode": "" },
        { "CountryNameCode": "India +91", "DialingCode": "+91" },
        { "CountryNameCode": "USA +1", "DialingCode": "+1" },
        { "CountryNameCode": "UAE +971", "DialingCode": "+971" },
        { "CountryNameCode": "ZAF +27", "DialingCode": "+27" }
      ];

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
                Validators.minLength(2),Validators.maxLength(50),
                Validators.pattern(this.onlyAlphabetsWithSpaceQuoteHyphen)])],
            lastName: [null, Validators.compose([Validators.required,
                Validators.minLength(2),Validators.maxLength(50),
                Validators.pattern(this.onlyAlphabetsWithSpaceQuoteHyphen)])],
            code: [null, Validators.compose([Validators.required])],
            email: [null, Validators.compose([Validators.required, Validators.pattern(this.emailRegex)])],
            preferredLanguage: [''],
            Image: [''],
            CountryCode: [''],
            mobileNumber: [null, Validators.compose([Validators.required, Validators.maxLength(10),
                Validators.minLength(10), Validators.pattern(this.onlyNumbers)])]
        })
        this.getProfileRecord();
    }

    ngAfterViewInit(): void {
        //this.jqxcountryCodeDropDownList.createComponent(this.jqxCountryCodeDrownSettings);
        //this.jqxPreferredLanguageDropdownlist.createComponent(this.jqxPrefferedLanguageDrownSettings);
    }

    // Profile formgroup, validation access function.
    // get profileFormControls(){return this.profileFormGroup['controls']; }

    //Function to call preferred language from the localstorage--
    getResources() {
        var ResourcesArr = this.localstorageservice.GetResource();
        var userClaimsDetail = JSON.parse(localStorage.getItem('userClaims'));
        if (userClaimsDetail) {
            this.Locale = userClaimsDetail.PreferedLanguageCode;
            this.userClaimsRolePrivilegeOperations = userClaimsDetail.Privileges;
        }
        else {
            this.Locale = 'enUS';
            this.userClaimsRolePrivilegeOperations = [];
        }
        if (ResourcesArr != null) {
            if (ResourcesArr.length > 0) {
                // var profileSectionName = ResourcesArr[0].ResourceSections.filter(x => x.SectionName === ConfigConstants.ProfileSetUpUISection);
                // if (profileSectionName.length > 0) {
                //     profileSectionName.forEach(resourceSection => {
                //         let resourceItemArr = []
                //         resourceItemArr = resourceSection.ResourceItems;
                //         resourceItemArr.forEach(resource => {
                //             this.profileResources[resource.Key] = resource.Value;
                //         })
                //     })
                // }

                // else {
                //     //fallbackcall for resource api if resource fetching failed from localstorage--
                //     this.getResourcesIfLocalStorageFailed();
                // }
            }
        }
        else {
            //call for resource service from api--
            this.getResourcesIfLocalStorageFailed();
        }
    }

    async getResourcesIfLocalStorageFailed() {
        //var sectionStr = ConfigConstants.ProfileSetUpUISection;
        let resourceService = this.injector.get(ResourceService);
        //this.profileResources = await resourceService.getResources(sectionStr, this.Locale, false);
    }

    //Function call to get country code--
    // async getCountryDetails() {
    //     let service = this.injector.get(UserService);
    //     let searchParameter: any = {};
    //     searchParameter.PagingParameter = {};
    //     searchParameter.PagingParameter.PageIndex = Constants.DefaultPageIndex;
    //     searchParameter.PagingParameter.PageSize = Constants.DefaultPageSize;
    //     searchParameter.SortParameter = {};
    //     searchParameter.SortParameter.SortColumn = Constants.Name;
    //     searchParameter.SortParameter.SortOrder = Constants.Ascending;
    //     searchParameter.SearchMode = Constants.Contains;
    //     this.countrycodeList = await service.getCountryCode(searchParameter);
    //     this.isCountryDataRetrieved = true;
    //     this.countrycodeList.forEach(element => {
    //         let countryObject = {
    //             "Identifier": element.Identifier,
    //             "Name": element.Name,
    //             "Code": element.Code,
    //             "DialingCode": element.DialingCode,
    //             "DisplayName": element.DialingCode + '-' + element.Name,
    //         }
    //         this.countrycodeLists.push(countryObject);
    //     })
    //     this.jqxcountryCodeDropDownList.source(this.countrycodeLists);
    //     if (this.isLanguageDataRetrieved == true && this.isCountryDataRetrieved == true) {
    //         this.getProfileRecord();
    //     }
    // }
    
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
        this.uiLoader.start();
        this.usersList = await service.getUser(searchParameter);
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
                mobileNo = this.userRecord.ContactNumber;
            }
            this.profileFormGroup.patchValue({
                firstName: this.userRecord.FirstName,
                lastName: this.userRecord.LastName,
                email: this.userRecord.EmailAddress,
                mobileNumber: mobileNo,
                CountryCode: mobileNoArr[0],
            });
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

    onFileChanged(event) {
        let file = event.target.files[0];
        if (file.size > 4194304) {
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

        if (this.imageSize > 4194304) {
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
        // if (this.profileFormGroup.controls.email.invalid) {
        //     this.profileFormErrorObject.showUserEmailError = true;
        //     return false;
        // }
        if (this.profileFormGroup.controls.mobileNumber.invalid) {
            this.profileFormErrorObject.showUserMobileNumberError = true;
            return false;
        }
        return true;
    }

    //save functionality.
    onSubmit() {
        if (this.profileFormValidaton()) {
            this.userRecord.FirstName = this.profileFormGroup.value.firstName;
            this.userRecord.LastName = this.profileFormGroup.value.lastName;
            this.userRecord.EmailAddress = this.profileFormGroup.value.email;
            this.userRecord.ContactNumber = this.profileFormGroup.value.CountryCode + "-" + this.profileFormGroup.value.mobileNumber,
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
              window.location.reload();
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
        this.route.navigate(['dashboard']);
    }

}
