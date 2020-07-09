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
  toggleNav() {
    if (this.sidebar_class == "hide-sidebar" || this.collapse_class == "collapse-container" || this.sidebar_footer == "hide-side-bar-footer", this.collapse_toogleClass == "fa-bars") {
      this.sidebar_class = "show-sidebar"
      this.collapse_class = "uncollapse-container"
      this.collapse_toogleClass = "fa-arrow-left"
      this.sidebar_footer = "show-side-bar-footer"
    } else {
      this.sidebar_class = "hide-sidebar"
      this.collapse_toogleClass = "fa-bars"
      this.collapse_class = "collapse-container"
      this.sidebar_footer = "hide-side-bar-footer"
    }
  }


  ChangeSideBar() {
    this.element = document.getElementById('HomeSettingIcon') as HTMLElement;
    console.log(this.element);
    this.IsMainMenu = !this.IsMainMenu
    if (this.IsMainMenu) {
      this.URL = '/dashboard';
      this.element.className = 'fa fa-cog';
      this.element.title = "Asset Configuration Settings";
      //$('.submenu-li').removeClass('active-li-submenu');
      this.route.navigate(['/dashboard']);
      this.iconTitle = "Asset Configuration Settings";
    }
    else {
      this.URL = '/settings';
      this.element.className = 'fa fa-home'
      this.element.title = "Home";
      this.iconTitle = "Home";
      //$('.submenu-li').removeClass('active-li-submenu');
      this.route.navigate(['/settings']);
    }
  }
  navigateToRoles() {
    this.URL = '/roles';
    this.route.navigate(['/roles']);
  }
  navigateToUser() {
    this.URL = '/user';
    this.route.navigate(['/user']);
  }
  navigateToWidgets() {
    this.URL = '/widgets'
    this.route.navigate(['/widgets']);
  }
  navigateToDashoard() {
    this.URL = '/dashboard';
    this.route.navigate(['/dashboard']);
  }
  navigateToTemplates() {
    this.URL = '/pages';
    this.route.navigate(['/pages']);
  }
  navigateToChangePassword() {
    this.URL = '/changepassword';
    this.route.navigate(['/changepassword']);
  }
  navigateToProfile() {
    this.URL = '/profile';

    this.route.navigate(['/profile']);
  }
  navigateToScheduleManagement() {
    this.URL = '/schedulemanagement';

    this.route.navigate(['/schedulemanagement']);
  }
  navigateToStatementDef() {
    this.URL = '/statementdefination';

    this.route.navigate(['/statementdefination']);
  }
  navigateToAssetLibraries() {
    this.URL = '/assetlibrary';

    this.route.navigate(['/assetlibrary']);
  }
  navigateToLogs() {
    this.URL = '/logs';

    this.route.navigate(['/logs']);
  }
  navigateToAnalytics() {
    this.URL = '/analytics';

    this.route.navigate(['/analytics']);
  }
  navigateToSettings() {

    this.route.navigate(['/settings']);
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
        })
      }

      $('.side-bar-li').click(function () {
        $('.side-bar-li').removeClass('active-li');
        $(this).addClass('active-li');
      })

      $('.submenu-li').click(function () {
        $('.submenu-li').removeClass('active-li-submenu');
        $(this).addClass('active-li-submenu');
      })


    })
    this.URL = this.route.url;
    if (this.URL == '/settings') {
      this.element = document.getElementById('HomeSettingIcon') as HTMLElement;
      this.element.className = 'fa fa-home'
      this.iconTitle = "Home";
      this.element.title = "Home"
      this.IsMainMenu = false;
    }
    else {
      this.element = document.getElementById('HomeSettingIcon') as HTMLElement;
      this.element.className = 'fa fa-cog'
      this.iconTitle = "Asset Configuration Settings"
      this.element.title = "Asset Configuration Settings";
    }
    if (this.URL.includes('/user')) {
      this.URL ='/user'
    }
    else if (this.URL.includes('/widgets')) {
      this.URL = '/widgets'
    }
    else if (this.URL.includes('/schedulemanagement')) {
      this.URL = '/schedulemanagement'
    }
    else if (this.URL.includes('/assetlibrary')) {
      this.URL = '/assetlibrary'
    }
    else if (this.URL.includes('/statementdefination')) {
      this.URL = '/statementdefination'
    }
    else if (this.URL.includes('/analytics')) {
      this.URL = '/analytics'
    }
    else if (this.URL.includes('/logs')) {
      this.URL = '/logs'
    }
  }

}
