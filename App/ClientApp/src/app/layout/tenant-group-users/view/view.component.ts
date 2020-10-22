import { Component, OnInit, Injector } from '@angular/core';
import { Location } from '@angular/common';
import { ActivatedRoute, Router, NavigationEnd } from '@angular/router';
import { TenantUserService } from '../../tenantuser/tenantuser.service';
import { DialogService } from '@tomblue/ng2-bootstrap-modal';
import { Constants } from 'src/app/shared/constants/constants';
import { NgxUiLoaderService } from 'ngx-ui-loader';
import { LocalStorageService } from 'src/app/shared/services/local-storage.service';
import { TenantUser } from '../../tenantuser/tenantuser';
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
  public params;
  public UserIdentifier;
  public tenantuserViewArray: TenantUser[] = [];
  public tenantusersView
  public profileImageList: any = [];
  public tenantuserImage;
  public loggedInTenantUserIdentifier;
  public tenantgroupuserRecord: any = {};;
  public TenantUserFilter: any = {
    FirstName: null,
    LastName: null,
    EmailAddress: null,
    MobileNumber: null,
    LockStatus: false,
    ActivationStatus: false,
  };
  public TenantCode = '';

  constructor(
    private service: TenantUserService,
    private router: Router,
    private localstorageservice: LocalStorageService,
    private injector: Injector,
    private _messageDialogService: MessageDialogService) {
    //getting localstorage item
    router.events.subscribe(e => {
      if (e instanceof NavigationEnd) {
        if (e.url.includes('/tenantgroupusers')) {
          //set passing parameters to localstorage.
          this.params = JSON.parse(localStorage.getItem('tenantgroupuserRouteparams'));
          if (this.params) {
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
    this.fillTenantUserDetail();
  }

  //Function call to view the perticular tenantuser detail by passing tenantuser identifier--
  async fillTenantUserDetail() {
    let tenantuserService = this.injector.get(TenantUserService);
    let searchParameter: any = {};
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
    //this.spinner.start();
    var response = await tenantuserService.getUser(searchParameter);
    this.tenantuserViewArray = response.usersList;
    //this.spinner.stop();
    if (this.tenantuserViewArray.length > 0) {
      this.tenantgroupuserRecord = this.tenantuserViewArray[0];
      this.profileImageList = this.tenantuserViewArray[0].Image;
      if (this.profileImageList.URL == "" || this.profileImageList.URL == undefined) {
        this.tenantuserImage = "assets/images/tenantuser.png"
      }
      else {
        this.tenantuserImage = this.profileImageList.URL;
      }
    }
  }

  //This function will take back to the tenant group user list page--
  backClicked() {
    // this._location.back();
    this.router.navigate(['tenantgroupusers']);
  }

  editTenantGroupUser() {
    let queryParams = {
      Routeparams: {
        passingparams: {
          "UserIdentifier": this.UserIdentifier,
        }
      }
    }
    localStorage.setItem("tenantgroupuserRouteparams", JSON.stringify(queryParams))
    this.router.navigate(['tenantgroupusers', 'edit']);
  }

  //function written to delete tenant group user--
  deleteTenantGroupUser() {
    let message = "Are you sure you want to delete this tenant group user?"
    this._messageDialogService.openConfirmationDialogBox('Confirm', message, Constants.msgBoxWarning).subscribe(async (isConfirmed) => {
      if (isConfirmed) {
        let isDeleted = await this.service.deleteUser(this.UserIdentifier);
        if (isDeleted) {
          let messageString = Constants.recordDeletedMessage;
          this._messageDialogService.openDialogBox('Success', messageString, Constants.msgBoxSuccess);
          this.router.navigate(['tenantgroupusers']);
        }
        //}
      }
    });
  }
}
