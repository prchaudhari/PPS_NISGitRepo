import { Component, OnInit, Injector } from '@angular/core';
import { Location } from '@angular/common';
import { ActivatedRoute, Router, NavigationEnd } from '@angular/router';
import { TenantUserService } from '../tenantuser.service';
import { DialogService } from '@tomblue/ng2-bootstrap-modal';
import { Constants } from 'src/app/shared/constants/constants';
import { NgxUiLoaderService } from 'ngx-ui-loader';
import { LocalStorageService } from 'src/app/shared/services/local-storage.service';
import { TenantUser } from '../tenantuser';
import { MessageDialogService } from 'src/app/shared/services/mesage-dialog.service';

@Component({
  selector: 'app-view',
  templateUrl: './view.component.html',
  styleUrls: ['./view.component.scss']
})
export class ViewComponent implements OnInit {
  //Public variable delacred--
  public tenantuserId;
  public name;
  public code;
  public firstName;
  public mobileNumber;
  public email;
  public lastName;
  public active;
  public orgName;
  public message;
  public lockSlider: boolean;
  public activeSlider: boolean;
  public params;
  public TenantUserIdentifier;
  public TenantUserName;
  public countryDialingCode;
  public preferredLanguage;
  public tenantuserViewResources = {}
  public ResourceLoadingFailedMsg = "Resouce Loading Failed..";
  public Locale;
  public tenantuserViewArray: TenantUser[] = [];
  public tenantusersView
  public organizationUnitList = [];
  public ouName = "";
  public profileImageList: any = [];
  public tenantuserImage;
  public loggedInTenantUserIdentifier;
  public tenantuserRecord: any = {};;
  public TenantUserFilter: any = {
    FirstName: null,
    LastName: null,
    EmailAddress: null,
    MobileNumber: null,
    LockStatus: false,
    ActivationStatus: false,
  };

  constructor(private _location: Location,
    private route: ActivatedRoute,
    private service: TenantUserService,
    private _dialogService: DialogService,
    private spinner: NgxUiLoaderService,
    private router: Router,
    private localstorageservice: LocalStorageService,
    private injector: Injector,
    private _messageDialogService: MessageDialogService) {
    //getting localstorage item
    router.events.subscribe(e => {
      if (e instanceof NavigationEnd) {
        if (e.url.includes('/tenantuser')) {
          //set passing parameters to localstorage.
          this.params = JSON.parse(localStorage.getItem('tenantuserRouteparams'));
          if (this.params) {
            this.TenantUserIdentifier = this.params.Routeparams.passingparams.TenantUserIdentifier;
            this.TenantUserFilter.FirstName = this.params.Routeparams.filteredparams.FirstName;
            this.TenantUserFilter.LastName = this.params.Routeparams.filteredparams.LastName;
            this.TenantUserFilter.EmailAddress = this.params.Routeparams.filteredparams.EmailAddress;
            this.TenantUserFilter.MobileNumber = this.params.Routeparams.filteredparams.MobileNumber;
            this.TenantUserFilter.LockStatus = this.params.Routeparams.filteredparams.LockStatus;
            this.TenantUserFilter.ActivationStatus = this.params.Routeparams.filteredparams.ActivationStatus;
          }
        } else {
            localStorage.removeItem("tenantuserRouteparams");
        }
      }
    });
  }

  ngOnInit() {
    this.fillTenantUserDetail();
   
  }

  //Function call to view the perticular tenantuser detail by passing tenantuser identifier--
  async fillTenantUserDetail() {
    let tenantuserService = this.injector.get(TenantUserService);
    let searchParameter: any = {};
    searchParameter.Identifier = this.TenantUserIdentifier;
    searchParameter.PagingParameter = {};
    searchParameter.PagingParameter.PageIndex = Constants.DefaultPageIndex;
    searchParameter.PagingParameter.PageSize = Constants.DefaultPageSize;
    searchParameter.SortParameter = {};
    searchParameter.SortParameter.SortColumn = "Id";
    searchParameter.SortParameter.SortOrder = Constants.Ascending;
    searchParameter.SearchMode = Constants.Contains;
    searchParameter.IsInstanceManager = true;
    //this.spinner.start();
    var response = await tenantuserService.getUser(searchParameter);
    this.tenantuserViewArray = response.usersList;
    //this.spinner.stop();
    if (this.tenantuserViewArray.length > 0) {
      this.tenantuserRecord = this.tenantuserViewArray[0];
      this.profileImageList = this.tenantuserViewArray[0].ProfileImage;
      if (this.profileImageList.URL == "" || this.profileImageList.URL == undefined) {
        this.tenantuserImage = "assets/images/tenantuser.png"
      }
      else {
        this.tenantuserImage = this.profileImageList.URL;
      }
    }
  }

  //This function will take back to the tenantuser list page--
  backClicked() {
    // this._location.back();
    this.router.navigate(['tenantusers']);
  }

  editTenantUser() {
    let queryParams = {
      Routeparams: {
        passingparams: {
          "TenantUserIdentifier": this.TenantUserIdentifier,
        },
        filteredparams: {
          "FirstName": this.TenantUserFilter.FirstName != null ? this.TenantUserFilter.FirstName : null,
          "LastName": this.TenantUserFilter.LastName != null ? this.TenantUserFilter.LastName : null,
          "Code": this.TenantUserFilter.Code != null ? this.TenantUserFilter.Code : null,
          "EmailAddress": this.TenantUserFilter.EmailAddress != null ? this.TenantUserFilter.EmailAddress : null,
          "MobileNumber": this.TenantUserFilter.MobileNumber != null ? this.TenantUserFilter.MobileNumber : null,
          "OrganisationUnitIdentifier": this.TenantUserFilter.OrganisationUnitIdentifier != null ? this.TenantUserFilter.OrganisationUnitIdentifier : null,
          "DesignationIdentifier": this.TenantUserFilter.DesignationIdentifier != null ? this.TenantUserFilter.DesignationIdentifier : null,
          "PreferedLanguageIdentifier": this.TenantUserFilter.PreferedLanguageIdentifier != null ? this.TenantUserFilter.PreferedLanguageIdentifier : null,
          "LockStatus": this.TenantUserFilter.LockStatus,
          "ActivationStatus": this.TenantUserFilter.ActivationStatus,
        }
      }
    }
    localStorage.setItem("tenantuserRouteparams", JSON.stringify(queryParams))
    this.router.navigate(['tenantusers', 'edit']);
  }

  //function written to delete tenantuser--
  deleteTenantUser() {
    let message = "Are you sure you want to delete this tenantuser?"
    this._messageDialogService.openConfirmationDialogBox('Confirm', message, Constants.msgBoxWarning).subscribe(async (isConfirmed) => {
      if (isConfirmed) {
        let isDeleted = await this.service.deleteUser(this.TenantUserIdentifier);
        if (isDeleted) {
          let messageString = Constants.recordDeletedMessage;
          this._messageDialogService.openDialogBox('Success', messageString, Constants.msgBoxSuccess);
          this.router.navigate(['tenantusers']);
        }
        //}
      }
    });
  }
}
