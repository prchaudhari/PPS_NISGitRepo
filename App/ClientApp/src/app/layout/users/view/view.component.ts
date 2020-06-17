import { Component, OnInit, Injector } from '@angular/core';
import { Location } from '@angular/common';
import { ActivatedRoute, Router, NavigationEnd } from '@angular/router';
import { UserService } from '../user.service';
import { HttpResponse, HttpEvent, HttpEventType } from '@angular/common/http';
import { DialogService } from 'ng2-bootstrap-modal';
import { MsgBoxComponent } from 'src/app/shared/modules/message/messagebox.component';
import { Constants } from 'src/app/shared/constants/constants';
import { NgxUiLoaderService } from 'ngx-ui-loader';
import { LocalStorageService } from 'src/app/shared/services/local-storage.service';
import { ConfigConstants } from 'src/app/shared/constants/configConstants';
import { ResourceService } from 'src/app/shared/services/resource.service';
import { User } from '../user';
import { SortOrder } from 'src/app/shared/enums/sort-order.enum';
import { SearchMode } from 'src/app/shared/enums/search-mode.enum';
import { MessageDialogService } from 'src/app/shared/services/mesage-dialog.service';

@Component({
  selector: 'app-view',
  templateUrl: './view.component.html',
  styleUrls: ['./view.component.scss']
})
export class ViewComponent implements OnInit {
  //Public variable delacred--
  public isCollapsedDetails = false;
  public isCollapsedRoles = true;
  public userId;
  public name;
  public code;
  public firstName;
  public mobileNumber;
  public email;
  public lastName;
  public active;
  public orgName;
  public roleList = [];
  public roleName;
  public roleDescription;
  public message;
  public lockSlider: boolean;
  public activeSlider: boolean;
  public params;
  public UserIdentifier;
  public UserName;
  public countryDialingCode;
  public preferredLanguage;
  public userViewResources = {}
  public ResourceLoadingFailedMsg = "Resouce Loading Failed..";
  public Locale;
  public userClaimsRolePrivilegeOperations;
  public userViewArray: User[] = [];
  public usersView
  public organizationUnitList = [];
  public ouName = "";
  public profileImageList: any = [];
  public userImage;
  public loggedInUserIdentifier;
  public userRecord: any = {};;
  public UserFilter: any = {
    FirstName: null,
    LastName: null,
    EmailAddress: null,
    MobileNumber: null,
    RoleIdentifier: null,
    LockStatus: false,
    ActivationStatus: false,
  };

  constructor(private _location: Location,
    private route: ActivatedRoute,
    private service: UserService,
    private _dialogService: DialogService,
    private spinner: NgxUiLoaderService,
    private router: Router,
    private localstorageservice: LocalStorageService,
    private injector: Injector,
    private _messageDialogService: MessageDialogService) {
    //getting localstorage item
    router.events.subscribe(e => {
      if (e instanceof NavigationEnd) {
        if (e.url.includes('/user')) {
          //set passing parameters to localstorage.
          this.params = JSON.parse(localStorage.getItem('userRouteparams'));
          if (this.params) {
            this.UserIdentifier = this.params.Routeparams.passingparams.UserIdentifier;
            this.UserFilter.FirstName = this.params.Routeparams.filteredparams.FirstName;
            this.UserFilter.LastName = this.params.Routeparams.filteredparams.LastName;
            this.UserFilter.EmailAddress = this.params.Routeparams.filteredparams.EmailAddress;
            this.UserFilter.MobileNumber = this.params.Routeparams.filteredparams.MobileNumber;
            this.UserFilter.RoleIdentifier = this.params.Routeparams.filteredparams.RoleIdentifier;
            this.UserFilter.LockStatus = this.params.Routeparams.filteredparams.LockStatus;
            this.UserFilter.ActivationStatus = this.params.Routeparams.filteredparams.ActivationStatus;
          }
        } else {
          localStorage.removeItem("userRouteparams");

        }
      }
    });
  }

  ngOnInit() {
    this.fillUserDetail();
  }

  //Function call to view the perticular user detail by passing user identifier--
  async fillUserDetail() {
    let userService = this.injector.get(UserService);
    let searchParameter: any = {};
    searchParameter.Identifier = this.UserIdentifier;
    searchParameter.PagingParameter = {};
    searchParameter.PagingParameter.PageIndex = Constants.DefaultPageIndex;
    searchParameter.PagingParameter.PageSize = Constants.DefaultPageSize;
    searchParameter.SortParameter = {};
    searchParameter.SortParameter.SortColumn = "Id";
    searchParameter.SortParameter.SortOrder = Constants.Ascending;
    searchParameter.SearchMode = Constants.Contains;
    this.spinner.start();
    this.userViewArray = await userService.getUser(searchParameter);
    if (this.userViewArray.length > 0) {
      this.userRecord = this.userViewArray[0];
      this.profileImageList = this.userViewArray[0].ProfileImage;
      if (this.profileImageList.URL == "" || this.profileImageList.URL == undefined) {
        this.userImage = "assets/images/user.png"
      }
      else {
        this.userImage = this.profileImageList.URL;
      }
    }
  }

  //This function will take back to the user list page--
  backClicked() {
    // this._location.back();
    this.router.navigate(['user']);
  }
  editUser() {
    let queryParams = {
      Routeparams: {
        passingparams: {
          "UserIdentifier": this.UserIdentifier,
        },
        filteredparams: {
          "FirstName": this.UserFilter.FirstName != null ? this.UserFilter.FirstName : null,
          "LastName": this.UserFilter.LastName != null ? this.UserFilter.LastName : null,
          "Code": this.UserFilter.Code != null ? this.UserFilter.Code : null,
          "EmailAddress": this.UserFilter.EmailAddress != null ? this.UserFilter.EmailAddress : null,
          "MobileNumber": this.UserFilter.MobileNumber != null ? this.UserFilter.MobileNumber : null,
          "OrganisationUnitIdentifier": this.UserFilter.OrganisationUnitIdentifier != null ? this.UserFilter.OrganisationUnitIdentifier : null,
          "DesignationIdentifier": this.UserFilter.DesignationIdentifier != null ? this.UserFilter.DesignationIdentifier : null,
          "PreferedLanguageIdentifier": this.UserFilter.PreferedLanguageIdentifier != null ? this.UserFilter.PreferedLanguageIdentifier : null,
          "RoleIdentifier": this.UserFilter.RoleIdentifier != null ? this.UserFilter.RoleIdentifier : null,
          "LockStatus": this.UserFilter.LockStatus,
          "ActivationStatus": this.UserFilter.ActivationStatus,
        }
      }
    }
    localStorage.setItem("userRouteparams", JSON.stringify(queryParams))
    this.router.navigate(['user', 'userEdit']);
  }

  //function written to delete user--
  deleteUser() {
    let message = "Are you sure you want to delete this user?"
    this._messageDialogService.openConfirmationDialogBox('Confirm', message, Constants.msgBoxWarning).subscribe(async (isConfirmed) => {
      if (isConfirmed) {
        let isDeleted = await this.service.deleteUser(this.UserIdentifier);
        if (isDeleted) {
          let messageString = Constants.recordDeletedMessage;
          this._messageDialogService.openDialogBox('Success', messageString, Constants.msgBoxSuccess);
          this.router.navigate(['user']);
        }
        //}
      }
    });
  }
}
