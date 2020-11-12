import { Component, OnInit, Injector } from '@angular/core';
import { Router } from '@angular/router';
import { LoginService } from './login.service';
import { DialogService } from '@tomblue/ng2-bootstrap-modal';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';
import { HttpClient, HttpResponse, HttpHeaders, HttpEvent, HttpParams } from '@angular/common/http';
import { LocalStorageService } from 'src/app/shared/services/local-storage.service';
import { NgxUiLoaderService } from 'ngx-ui-loader';
import { MessageDialogService } from 'src/app/shared/services/mesage-dialog.service';
import { ConfigConstants } from 'src/app/shared/constants/configConstants';
import { Constants, DynamicGlobalVariable } from 'src/app/shared/constants/constants';
import { ResourceService } from 'src/app/shared/services/resource.service';
import { AuthenticationService } from 'src/app/authentication/authentication.service';
import { TenantService } from '../layout/tenants/tenant.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html'

})

export class LoginComponent implements OnInit {
  [x: string]: any;

  public loginForm: FormGroup;
  public resetForm: FormGroup;
  public emailPattern = "^[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,4}$";
  public emailPatternRegex = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
  public UserIdentifier;
  public errorMsg: boolean;
  public Locale;
  public isLoginFlag: boolean = true;
  public isForgotPassword: boolean = false;
  public roleList: any = [];
  public loginResources = {};
  public ResourceLoadingFailedMsg = "Resouce Loading Failed..";
  public commonRolePrivileges = [];
  public loginErrorMsg: string = '';
  public status;
  public result;
  public roleDetail;

  public statePrivilegeMap: any = [{
    "State": "dashboard",
    "Entity": "Dashboard",
  },
  {
    "State": "user",
    "Entity": "User",
  },
  {
    "State": "dashboard",
    "Entity": "Dashboard",
  },
  {
    "State": "roles",
    "Entity": "Role",
  },
  {
    "State": "assetlibrary",
    "Entity": "Asset Library",
  },
  {
    "State": "widgets",
    "Entity": "Widget",
  },
  {
    "State": "pages",
    "Entity": "Page",
  },
  {
    "State": "statementdefination",
    "Entity": "Statement Definition",
  },
  {
    "State": "schedulemanagement",
    "Entity": "Schedule Management",
  },
  {
    "State": "logs",
    "Entity": "Log",
  },
  {
    "State": "analytics",
    "Entity": "Analytics",
  },
  {
    "State": "statemenetsearch",
    "Entity": "Statement Search",
  },
  ]
  // login form error Obj created.
  public loginFormErrorObject: any = {
    showUserNameError: false,
    showPasswordError: false,
  };

  public resetFormErrorObject: any = {
    showResetUserNameError: false,
  };

  //getters of loginForm group
  get userName() {
    return this.loginForm.get('userName');
  }

  get password() {
    return this.loginForm.get('password');
  }

  get resetUserName() {
    return this.resetForm.get('resetUserName');
  }

  constructor(private fb: FormBuilder,
    private route: Router,
    private loginService: LoginService,
    private _dialogService: DialogService,
    private http: HttpClient,
    private localstorageservice: LocalStorageService,
    private spinner: NgxUiLoaderService,
    private injector: Injector,
    private _messageDialogService: MessageDialogService,
    private dynamicGlobalVariable: DynamicGlobalVariable

  ) {
    //this.getResources();
  }
  isForgotPassswordForm() {
    this.isForgotPassword = true;
    this.isLoginFlag = false;
  }
  isLoginForm() {
    this.isForgotPassword = false;
    this.isLoginFlag = true;
  }
  ngOnInit() {
    this.loginForm = this.fb.group({
      userName: ['', [Validators.required, Validators.pattern(this.emailPatternRegex)]],
      password: ['', [Validators.required]]
    });

    this.resetForm = this.fb.group({
      resetUserName: ['', [Validators.required, Validators.pattern(this.emailPatternRegex)]],
    });
    this.dynamicGlobalVariable.IsSessionExpireMessageDisplyed = false;
    if (localStorage.getItem('LastRequestTime') != null && localStorage.getItem('LastRequestTime') != '') {
      let lastReqTime = new Date(localStorage.getItem('LastRequestTime'));
      let currentDate = new Date();
      let timeDiff = Math.floor((currentDate.getTime() - lastReqTime.getTime()) / (1000 * 60));
      if (timeDiff < 15) {
        var userClaimsDetail = JSON.parse(localStorage.getItem('userClaims'));
        if (userClaimsDetail) {
          var isFound = false;
          var state = 0;
          var userClaimsRolePrivilegeOperations = userClaimsDetail.Privileges;
          this.statePrivilegeMap.forEach(map => {
            var isPresent = userClaimsRolePrivilegeOperations.filter(p => p.EntityName == map.Entity);
            if (isPresent != undefined && isPresent.length > 0) {
              if (isFound == false) {
                isFound = true;
                state = map.State;
              }
            }
          });
          if (isFound) {
            this.route.navigate([state]);
          }
          // this.route.navigate(['dashboard']);
        } else {
          this.localstorageservice.removeLocalStorageData();
          this.route.navigate(['login']);
        }
      } else {
        this.localstorageservice.removeLocalStorageData();
        this.route.navigate(['login']);
      }
    } else {
      this.localstorageservice.removeLocalStorageData();
      this.route.navigate(['login']);
    }


  }

  //custom validation check
  loginFormValidaton(): boolean {
    this.loginFormErrorObject.showUserNameError = false;
    this.loginFormErrorObject.showPasswordError = false;

    if (this.loginForm.controls.userName.invalid) {
      this.loginFormErrorObject.showUserNameError = true;
      return false;
    }
    if (this.loginForm.controls.password.invalid) {
      this.loginFormErrorObject.showPasswordError = true;
      return false;
    }
    return true;
  }

  //Form validtion check on login click--
  OnSubmit() {
    this.loginErrorMsg = '';
    this.errorMsg = false;
    if (this.loginFormValidaton()) {
      let loginObj: any = {
        grant_type: 'password',
        client_id: '00000000-0000-0000-0000-000000000000',
        username: this.loginForm.value.userName,
        password: this.loginForm.value.password
      }
      this.checkLogin(loginObj);
    }
    else {
      //this.errorMsg = true;
    }
  };
  onForgotPasswordSubmit(): boolean {
    if (this.userEmail == undefined || this.userEmail == null) {
      this._messageDialogService.openDialogBox('Error', "Please enter Email ID.", Constants.msgBoxError);
      return this.result = false;
    }
    if (this.userEmail != undefined && this.userEmail != null && this.userEmail.length == 0) {
      this.status = "Please enter Email ID";
      this._messageDialogService.openDialogBox('Error', "Please enter Email ID.", Constants.msgBoxError);
      return this.result = false;
    }
    else if (!this.emailPatternRegex.test(this.userEmail)) {
      this._messageDialogService.openDialogBox('Error', "Invalid email address.", Constants.msgBoxError);
      return this.result = false;
    }

    let params = new HttpParams();
    params = params.append('userEmail', this.userEmail);

    let operationUrl = ConfigConstants.BaseURL + 'User/ResetPassword';
    this.spinner.start();
    this.http.get(operationUrl, { params })
      .subscribe(data => {
        this.spinner.stop();
        this._messageDialogService.openDialogBox('Success', "Reset password link sent successfully.Please check your email.", Constants.msgBoxSuccess);
        this.isLoginForm();
      },
        error => {
          this._messageDialogService.openDialogBox('Error', 'Email Id not registered with us..!!', Constants.msgBoxError);
          this.spinner.stop();
        },
        () => {
          this.spinner.stop();
        }
      );
  }
  //Login functinality--
  checkLogin(loginObj) {
    this.spinner.start();
    this.loginService.getLoginDetails(loginObj).subscribe(async (response: HttpResponse<any>) => {
      if (response.status == 200) {
        this.spinner.stop();
        this.isLoginFlag = true;
        let data = response.body;
        let access_token = data.access_token;
        let token_type = data.token_type;
        let userData: any = {};
        this.UserIdentifier = data.UserIdentifier;
        userData.UserIdentifier = data.UserIdentifier;
        userData.UserName = data.UserName;
        userData.UserPrimaryEmailAddress = data.UserPrimaryEmailAddress;
        userData.TenantCode = data.TenantCode;
        userData.IsInstanceTenantManager = data.IsInstanceTenantManager;
        userData.IsTenantGroupManager = data.IsTenantGroupManager;
        userData.IsUserHaveMultiTenantAccess = data.IsUserHaveMultiTenantAccess;
        userData.RoleName = data.RoleName;
        userData.RoleIdentifier = data.RoleIdentifier;
        userData.UserTheme = data.UserTheme;
        userData.Privileges = [];
        localStorage.setItem('token', access_token);
        this.localstorageservice.SetCurrentUser(data);
        let userName = userData.UserName;

        localStorage.setItem("UserId", userData.UserIdentifier);
        localStorage.setItem("UserEmail", userData.UserPrimaryEmailAddress);
        localStorage.setItem("currentUserName", userName);
        localStorage.setItem("currentUserTheme", userData.UserTheme);
        localStorage.setItem("currentUserTheme", userData.UserTheme);
        localStorage.setItem("DateFormat", data.DateFormat);
        localStorage.setItem("StatePrivilegeMap", JSON.stringify(this.statePrivilegeMap));

        if (data.IsPasswordResetByAdmin != null && data.IsPasswordResetByAdmin.toLocaleLowerCase() == 'true') {
          localStorage.setItem('userClaims', JSON.stringify(userData));
          this.route.navigate(['changepassword']);
        }
        else {

          //conditional code for theme
          this.handleTheme(userData.UserTheme);

          if (userData.IsInstanceTenantManager != null && userData.IsInstanceTenantManager.toLocaleLowerCase() == 'true') {
            localStorage.setItem('userClaims', JSON.stringify(userData));
            this.route.navigate(['instanceManagerDashboard']);
          }
          else if (userData.IsUserHaveMultiTenantAccess != null && userData.IsUserHaveMultiTenantAccess.toLocaleLowerCase() == 'true') {
            localStorage.setItem('userClaims', JSON.stringify(userData));
            this.route.navigate(['selectTenant']);
          }
          else if (userData.IsTenantGroupManager != null && userData.IsTenantGroupManager.toLocaleLowerCase() == 'true') {
            localStorage.setItem('userClaims', JSON.stringify(userData));
            this.route.navigate(['groupManagerDashboard']);
          }
          else {
            userData.Privileges = await this.getUserRoles(userData.RoleIdentifier);
            if (this.roleDetail.IsActive == false) {
              this._messageDialogService.openDialogBox('Error', "User role is deactivated.", Constants.msgBoxError);
              this.localstorageservice.removeLocalStorageData();
            }
            else {
              if (userData.Privileges.length == 0 || userData.Privileges == null) {
                this._messageDialogService.openDialogBox('Error', "User role has no permission assigned.", Constants.msgBoxError);
                this.localstorageservice.removeLocalStorageData();
              }
              else {
                localStorage.setItem('userClaims', JSON.stringify(userData));
                await this.getTenantRecords(userData.TenantCode);
                //this.navigateToLandingPage();
                this.loginErrorMsg = '';
                var userClaimsDetail = JSON.parse(localStorage.getItem('userClaims'));
                var userClaimsRolePrivilegeOperations = userClaimsDetail.Privileges;
                var isFound = false;
                var state = 0;
                this.statePrivilegeMap.forEach(map => {
                  var isPresent = userClaimsRolePrivilegeOperations.filter(p => p.EntityName == map.Entity);
                  if (isPresent != undefined && isPresent.length > 0) {
                    if (isFound == false) {
                      isFound = true;
                      state = map.State;
                    }
                  }
                });
                if (isFound) {
                  this.route.navigate([state]);
                }
              }
            }
          }
        }

      }
    }, (error: HttpResponse<any>) => {
      this.spinner.stop();
      if (error["error"]) {
        if (error["error"].error_description) {
          let errorMessage = error["error"].error_description;
          if (errorMessage == 'User not found') {
            errorMessage = 'Email Id not registered with us..!!';
          }
          this._messageDialogService.openDialogBox('Error', errorMessage, Constants.msgBoxError);
        }
      }
    });
  }

  async getUserRoles(roleIdentifier) {
    let searchParameter: any = {};
    searchParameter.PagingParameter = {};
    searchParameter.PagingParameter.PageIndex = Constants.DefaultPageIndex;
    searchParameter.PagingParameter.PageSize = Constants.DefaultPageSize;
    searchParameter.SortParameter = {};
    searchParameter.SortParameter.SortColumn = Constants.Name;
    searchParameter.SortParameter.SortOrder = Constants.Ascending;
    searchParameter.SearchMode = Constants.Exact;
    searchParameter.IsRequiredRolePrivileges = true;
    searchParameter.Identifier = roleIdentifier;
    this.roleList = await this.loginService.getRoles(searchParameter);
    this.roleDetail = this.roleList[0];
    if (this.roleList.length > 0) {
      this.roleList.forEach(role => {
        role.RolePrivileges.forEach(privilege => {
          privilege.RolePrivilegeOperations.forEach(operation => {
            if (!this.rolePrivilegeExists(privilege.EntityName, operation.Operation)) {
              var obj = { EntityName: privilege.EntityName, Operation: operation.Operation }
              this.commonRolePrivileges.push(obj);
            }
          });
        });
      });
    }
    return this.commonRolePrivileges;
  }

  async getTenantRecords(tenantcode) {
    let tenantService = this.injector.get(TenantService);
    let searchParameter: any = {};
    searchParameter.PagingParameter = {};
    searchParameter.PagingParameter.PageIndex = Constants.DefaultPageIndex;
    searchParameter.PagingParameter.PageSize = Constants.DefaultPageSize;
    searchParameter.SortParameter = {};
    searchParameter.SortParameter.SortColumn = 'Id';
    searchParameter.SortParameter.SortOrder = Constants.Descending;
    searchParameter.SearchMode = Constants.Exact;
    searchParameter.TenantCode = tenantcode;
    searchParameter.IsCountryRequired = false;
    searchParameter.IsContactRequired = false;
    var response = await tenantService.getTenant(searchParameter);
    let tenant = response.List[0];
    localStorage.setItem('tenantDetails', JSON.stringify(tenant));
  }

  rolePrivilegeExists(entityName, operationName) {
    return this.commonRolePrivileges.some(function (el) {
      return (el.EntityName === entityName && operationName === el.Operation);
    });
  }

  handleTheme(theme) {
    const dom: any = document.querySelector('body');
    if (theme.toLocaleLowerCase() == 'theme1') {
      dom.classList.add('theme1');
      dom.classList.remove('theme2');
      dom.classList.remove('theme3');
      dom.classList.remove('theme4');
      dom.classList.remove('theme5');
      dom.classList.remove('theme0');
    }
    else if (theme.toLocaleLowerCase() == 'theme2') {
      dom.classList.remove('theme1');
      dom.classList.add('theme2');
      dom.classList.remove('theme3');
      dom.classList.remove('theme4');
      dom.classList.remove('theme5');
      dom.classList.remove('theme0');
    }
    else if (theme.toLocaleLowerCase() == 'theme3') {
      dom.classList.remove('theme1');
      dom.classList.remove('theme2');
      dom.classList.add('theme3');
      dom.classList.remove('theme4');
      dom.classList.remove('theme5');
      dom.classList.remove('theme0');
    }
    else if (theme.toLocaleLowerCase() == 'theme4') {
      dom.classList.remove('theme1');
      dom.classList.remove('theme2');
      dom.classList.remove('theme3');
      dom.classList.add('theme4');
      dom.classList.remove('theme5');
      dom.classList.remove('theme0');
    }
    else if (theme.toLocaleLowerCase() == 'theme5') {
      dom.classList.remove('theme1');
      dom.classList.remove('theme2');
      dom.classList.remove('theme3');
      dom.classList.remove('theme4');
      dom.classList.add('theme5');
      dom.classList.remove('theme0');
    }
    else if (theme.toLocaleLowerCase() == 'Theme0') {
      dom.classList.remove('theme1');
      dom.classList.remove('theme2');
      dom.classList.remove('theme3');
      dom.classList.remove('theme4');
      dom.classList.remove('theme5');
      dom.classList.add('theme0');
    }
  }

  navigateToLandingPage() {
    var routeFound = false;
    var userClaimsDetail = JSON.parse(localStorage.getItem('userClaims'));
    if (userClaimsDetail.Privileges.length > 0) {
      for (var i = 0; i < userClaimsDetail.Privileges.length; i++) {
        if (userClaimsDetail.Privileges[i].EntityName == 'Role' && userClaimsDetail.Privileges[i].Operation == 'View') {
          routeFound = true;
          this.route.navigate(['roles']);
          break;
        }
        else if (userClaimsDetail.Privileges[i].EntityName == 'User' && userClaimsDetail.Privileges[i].Operation == 'View') {
          routeFound = true;
          this.route.navigate(['user']);
          break;
        }
        else if (userClaimsDetail.Privileges[i].EntityName == 'Organisation Unit' && userClaimsDetail.Privileges[i].Operation == 'View') {
          routeFound = true;
          this.route.navigate(['organisationUnit']);
          break;
        }
        else if (userClaimsDetail.Privileges[i].EntityName == 'Shift' && userClaimsDetail.Privileges[i].Operation == 'View') {
          routeFound = true;
          this.route.navigate(['shift']);
          break;
        }
        else if (userClaimsDetail.Privileges[i].EntityName == 'Role' && userClaimsDetail.Privileges[i].Operation == 'View') {
          routeFound = true;
          this.route.navigate(['roles']);
          break;
        }
        else if (userClaimsDetail.Privileges[i].EntityName == 'Allowance' && userClaimsDetail.Privileges[i].Operation == 'View') {
          routeFound = true;
          this.route.navigate(['allowances']);
          break;
        }
        else if (userClaimsDetail.Privileges[i].EntityName == 'Holiday' && userClaimsDetail.Privileges[i].Operation == 'View') {
          routeFound = true;
          this.route.navigate(['holidays']);
          break;
        }
        else if (userClaimsDetail.Privileges[i].EntityName == 'Operation' && userClaimsDetail.Privileges[i].Operation == 'View') {
          routeFound = true;
          this.route.navigate(['operation']);
          break;
        }
      }
      if (!routeFound)
        this.route.navigate(['machinetiles']);
    }
    else {
      this.route.navigate(['machinetiles']);
    }
  }

  resetPasswordValidation() {
    this.resetFormErrorObject.showResetUserNameError = false;
    if (this.resetForm.controls.resetUserName.invalid) {
      this.resetFormErrorObject.showResetUserNameError = true;
      return false;
    }
    return true;
  }

  async OnResetClick() {
    // this.uiLoader.start();
    if (this.resetPasswordValidation()) {
      let service = this.injector.get(AuthenticationService);
      let resetPasswordValue = this.resetForm.value.resetUserName;
      let resetValue = await service.forgotPassword(resetPasswordValue);
      if (resetValue) {
        let messageString = Constants.sentPasswordMessage;
        this._messageDialogService.openDialogBox('Success', messageString, Constants.msgBoxSuccess);
      }
    }
  }

  //Function to call preferred language from the localstorage--
  getResources() {
    var ResourcesArr = this.localstorageservice.GetResource();
    this.Locale = 'enUS'
    if (ResourcesArr != null) {
      if (ResourcesArr.length > 0) {
        var loginSectionName = ResourcesArr[0].ResourceSections.filter(x => x.SectionName === ConfigConstants.LoginUISection);
        if (loginSectionName.length > 0) {
          loginSectionName.forEach(resourceSection => {
            let resourceItemArr = []
            resourceItemArr = resourceSection.ResourceItems;
            resourceItemArr.forEach(resource => {
              this.loginResources[resource.Key] = resource.Value;
            })
          })
        }
        else {
          //fallbackcall for resource api if resource fetching failed from localstorage--
          this.getLoginResources();
        }
      }
    }
    else {
      //call for resource service from api--
      this.getLoginResources();
    }
  }

  //Function call to set resources in local storage--
  async getLoginResources() {
    let sectionStr = ConfigConstants.LoginUISection
    let resourceService = this.injector.get(ResourceService);
    this.loginResources = await resourceService.getResources(sectionStr, this.Locale, false);
  };

  goToForgotPassword() {
    this.route.navigate(['forgotPassword']);
  }
}
