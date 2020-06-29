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

  navigateToRoles() {
    this.route.navigate(['/roles']);
  }
  navigateToUser() {
    this.route.navigate(['/user']);
  }
  navigateToWidgets() {
    this.route.navigate(['/widgets']);
  }
  navigateToDashoard() {
    this.route.navigate(['/dashboard']);
  }
  navigateToTemplates() {
    this.route.navigate(['/pages']);
  }
  navigateToChangePassword() {
    this.route.navigate(['/changepassword']);
  }
  navigateToProfile() {
    this.route.navigate(['/profile']);
  }
  navigateToScheduleManagement() {
    this.route.navigate(['/schedulemanagement']);
  }
  navigateToStatementDef() {
    this.route.navigate(['/statementdefination']);
  }
  navigateToAssetLibraries() {
    this.route.navigate(['/assetlibrary']);
  }
  navigateToLogs() {
    this.route.navigate(['/logs']);
  }
  navigateToAnalytics() {
    this.route.navigate(['/analytics']);
  }

  async logout() {
    // let loginService = this.injector.get(LoginService);
    // var userData = JSON.parse(localStorage.getItem('userClaims'));
    // let data = [{
    //     "UserIdentifier": userData.UserIdentifier,
    // }];
    // let isLoggedOut = await loginService.logoutUser(data);
    // if (isLoggedOut == true) {
    //     localStorage.removeItem('currentUserName');
    //     localStorage.removeItem('user');
    //     // localStorage.removeItem('AuthorisedResources');
    //     // localStorage.removeItem('selectedLangugage');
    //     // localStorage.removeItem('AuthorizedEnglishResources');
    //     // localStorage.removeItem('ApiResources');
    //     localStorage.removeItem('userClaims');
    //     localStorage.removeItem('token');
    //     //localStorage.removeItem('currentUserTheme');
    //     this.route.navigate(['login']);
    // }

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

  }

}
