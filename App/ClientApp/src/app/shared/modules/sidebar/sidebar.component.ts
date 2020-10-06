import { Component, OnInit, ViewChild, Output, HostListener, Injector } from '@angular/core';
import { Router } from '@angular/router';
import * as $ from 'jquery';
import { LoginService } from '../../../login/login.service';
import { LocalStorageService } from 'src/app/shared/services/local-storage.service';
//import { environment } from '../../../../environments/environment.prod'
import { environment } from '../../../../environments/environment'

@Component({
  selector: 'app-sidebar',
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.scss']
})
export class SidebarComponent implements OnInit {
  public collapseToogleClass: boolean = false;
  isTopbarShow: boolean = false

  public sidebar_class = 'hide-sidebar'
  public collapse_class = 'collapse-container'
  public sidebar_footer = 'hide-side-bar-footer'

  public collapse_toogleClass = 'fa-bars'
  public collapse_body_class = 'hide'

  public loggedInUserName = '';
  public AppVersion: string;
  public URL = '';
  public IsMainMenu = true;
  iconTitle = "Asset Configuration Settings";
  public element: HTMLElement;
  public userClaimsRolePrivilegeOperations;
  public isTenantAdminUser: boolean = false;
  public isInstantTenantManager: boolean = false;
  public isTenantGroupManager: boolean = false;
  public isByBtnClickEvent: boolean = false;

  toggleNav() {
    if (this.sidebar_class == "hide-sidebar" || this.collapse_class == "collapse-container" || this.sidebar_footer == "hide-side-bar-footer", this.collapse_toogleClass == "fa-bars") {
      this.showSidebar();
      this.isByBtnClickEvent = true;
    } else {
      this.isByBtnClickEvent = false;
      this.hideSidebar();
    }
  }

  hideSidebar(){
    if(!this.isByBtnClickEvent) {
      this.sidebar_class = "hide-sidebar";
      this.collapse_toogleClass = "fa-bars";
      this.collapse_class = "collapse-container";
      this.sidebar_footer = "hide-side-bar-footer";
    }
  }

  showSidebar(){
    if(!this.isByBtnClickEvent) {
      this.sidebar_class = "show-sidebar";
      this.collapse_class = "uncollapse-container";
      this.collapse_toogleClass = "fa-arrow-left";
      this.sidebar_footer = "show-side-bar-footer";
    }
  }

  ChangeSideBar() {
    this.IsMainMenu = !this.IsMainMenu;
    if (this.IsMainMenu) {
      this.URL = '/dashboard';
      this.route.navigate(['/dashboard']);
    }
    else {
      if(this.isInstantTenantManager == true) {
        this.URL = '/tenants';
        this.route.navigate(['/tenants']);
      }else if(this.isTenantGroupManager == true) {
        this.URL = '/tenantgroups';
        this.route.navigate(['/tenantgroups']);
      }else if(this.isTenantAdminUser == true) {
        this.URL = '/tenantConfiguration';
        this.route.navigate(['/tenantConfiguration']);
      }
      
    }
    this.hideSidebar();
  }

  navigateToRoles() {
    this.URL = '/roles';
    this.hideSidebar();
    this.route.navigate(['/roles']);
  }
  navigateToUser() {
    this.URL = '/user';
    this.hideSidebar();
    this.route.navigate(['/user']);
  }
  navigateToWidgets() {
    this.URL = '/widgets';
    this.hideSidebar();
    this.route.navigate(['/widgets']);
  }
  navigateToDashoard() {
    this.URL = '/dashboard';
    this.hideSidebar();
    this.route.navigate(['/dashboard']);
  }
  navigateToTemplates() {
    this.URL = '/pages';
    this.hideSidebar();
    this.route.navigate(['/pages']);
  }
  navigateToChangePassword() {
    this.URL = '/changepassword';
    this.hideSidebar();
    this.route.navigate(['/changepassword']);
  }
  navigateToProfile() {
    this.URL = '/profile';
    this.hideSidebar();
    this.route.navigate(['/profile']);
  }
  navigateToScheduleManagement() {
    this.URL = '/schedulemanagement';
    this.hideSidebar();
    this.route.navigate(['/schedulemanagement']);
  }
  navigateToTenants() {
    this.URL = '/tenants';
    this.hideSidebar();
    this.route.navigate(['/tenants']);
  }
  navigateToTenantUsers() {
    this.URL = '/tenantusers';
    this.hideSidebar();
    this.route.navigate(['/tenantusers']);
  }
  navigateToTenantGroups() {
    this.URL = '/tenantgroups';
    this.hideSidebar();
    this.route.navigate(['/tenantgroups']);
  }
  navigateToContactType() {
    this.URL = '/contacttype';
    this.hideSidebar();
    this.route.navigate(['/contacttype']);
  }
  navigateToDynamicWidgets() {
    this.URL = '/dynamicwidget';
    this.hideSidebar();
    this.route.navigate(['/dynamicwidget']);
  }
  navigateToStatementDef() {
    this.URL = '/statementdefination';
    this.hideSidebar();
    this.route.navigate(['/statementdefination']);
  }
  navigateToAssetLibraries() {
    this.URL = '/assetlibrary';
    this.hideSidebar();
    this.route.navigate(['/assetlibrary']);
  }
  navigateToLogs() {
    this.URL = '/logs';
    this.hideSidebar();
    this.route.navigate(['/logs']);
  }
  navigateToAnalytics() {
    this.URL = '/analytics';
    this.hideSidebar();
    this.route.navigate(['/analytics']);
  }
  navigateToSettings() {
    this.URL = '/settings';
    this.hideSidebar();
    this.route.navigate(['/settings']);
  }
  navigateToTenantConfig() {
    this.URL = '/tenantConfiguration';
    this.hideSidebar();
    this.route.navigate(['/tenantConfiguration']);
  }
  navigateToStatemenetSearch() {
    this.URL = '/statemenetsearch';
    this.hideSidebar();
    this.route.navigate(['/statemenetsearch']);
  }
  navigateToRenderEngine() {
    this.URL = '/renderengines';
    this.hideSidebar();
    this.route.navigate(['/renderengines']);
  }
  navigateToCountry() {
    this.URL = '/country';
    this.hideSidebar();
    this.route.navigate(['/country']);
  }
  navigateTothemeConfiguration() {
    this.URL = '/themeConfiguration';
    this.hideSidebar();
    this.route.navigate(['/themeConfiguration']);
  }
  navigateToMultiTenantUserAccess() {
    this.URL = '/multiTenantUserAccess';
    this.hideSidebar();
    this.route.navigate(['/multiTenantUserAccess']);
  }
  async logout() {
    this.localstorageservice.removeLocalStorageData();
    this.route.navigate(['login']);
  }

  constructor(private route: Router,
    private injector: Injector,
    private loginService: LoginService,
    private localstorageservice: LocalStorageService,) {
    this.AppVersion = environment.appVersion;
  }

  ngOnInit() {

    this.loggedInUserName = localStorage.getItem('currentUserName');
    var userClaimsDetail = JSON.parse(localStorage.getItem('userClaims'));
    this.userClaimsRolePrivilegeOperations = userClaimsDetail.Privileges;

    this.isInstantTenantManager = userClaimsDetail.IsInstanceTenantManager.toLocaleLowerCase() == 'true' ? true : false;
    this.isTenantGroupManager = userClaimsDetail.IsTenantGroupManager.toLocaleLowerCase() == 'true' ? true : false;

    var loggedInUserDetails = JSON.parse(localStorage.getItem('user'));
    this.isTenantAdminUser = loggedInUserDetails.RoleName == 'Super Admin' ? true : false;

    $(document).ready(function () {
      this.screenWidth = window.innerWidth;
      if (this.screenWidth <= 768) {
        $('.side-bar-li').click(function () {
          var mobileSidebar = true;
          if (mobileSidebar) {
            $('.side-bar-container').removeClass('show-sidebar');
            $('.side-bar-container').addClass('hide-sidebar');
            $('.overlay').addClass('hide-overlay');
            $('.overlay').removeClass('show-overlay');
            $('.xs-hamburger').addClass('fa-bars');
            $('.xs-hamburger').removeClass('fa-long-arrow-left');
            var mobileSidebar = false;
          } else {
            $('.side-bar-container').addClass('show-sidebar');
            $('.side-bar-container').removeClass('hide-sidebar');
            $('.overlay').removeClass('hide-overlay');
            $('.overlay').addClass('show-overlay');
            $('.xs-hamburger').removeClass('fa-bars');
            $('.xs-hamburger').addClass('fa-long-arrow-left');
            var mobileSidebar = true;
          }
        });
      }

      $('.side-bar-li').click(function () {
        $('.side-bar-li').removeClass('active-li');
        $(this).addClass('active-li');
      });

      $('.submenu-li').click(function () {
        $('.submenu-li').removeClass('active-li-submenu');
        $(this).addClass('active-li-submenu');
      });

    })
    
    this.URL = this.route.url;
    if(this.isInstantTenantManager == true || this.isTenantGroupManager == true || this.isTenantAdminUser) {
      if (this.URL.includes('/tenants') || this.URL.includes('/tenantConfiguration') ||this.URL.includes('/settings') || this.URL.includes('/country') || 
      this.URL.includes('/tenantgroups') || this.URL.includes('/themeConfiguration') || this.URL.includes('/contacttype') || this.URL.includes('/multiTenantUserAccess')
      || this.URL.includes('/tenantusers')) {
        this.IsMainMenu = false;
      }
      else {
        this.IsMainMenu = true;
      }
    }
    if (this.URL.includes('/tenants')) {
      this.URL = '/tenants';
    }
    else if (this.URL.includes('/country')) {
      this.URL = '/country';
    }
    else if (this.URL.includes('/contacttype')) {
      this.URL = '/contacttype';
    }
    else if (this.URL.includes('/tenantgroups')) {
      this.URL = '/tenantgroups';
    }
    else if (this.URL.includes('/themeConfiguration')) {
      this.URL = '/themeConfiguration';
    }
    else if (this.URL.includes('/user')) {
      this.URL ='/user';
    }
    else if (this.URL.includes('/widgets')) {
      this.URL = '/widgets';
    }
    else if (this.URL.includes('/schedulemanagement')) {
      this.URL = '/schedulemanagement';
    }
    else if (this.URL.includes('/assetlibrary')) {
      this.URL = '/assetlibrary';
    }
    else if (this.URL.includes('/statementdefination')) {
      this.URL = '/statementdefination';
    }
    else if (this.URL.includes('/pages')) {
      this.URL = '/pages';
    }
    else if (this.URL.includes('/analytics')) {
      this.URL = '/analytics';
    }
    else if (this.URL.includes('/logs')) {
      this.URL = '/logs';
    }
    else if (this.URL.includes('/settings')) {
      this.URL = '/settings';
    }
    else if(this.URL.includes('/renderengines')) {
      this.URL = '/renderengines';
    }
    else if (this.URL.includes('/tenantConfiguration')) {
      this.URL = '/tenantConfiguration';
    }
    else if (this.URL.includes('/multiTenantUserAccess')) {
      this.URL = '/multiTenantUserAccess';
    }
    else if(this.URL.includes('/tenantusers')) {
      this.URL = '/tenantusers';
    }
  }

}
