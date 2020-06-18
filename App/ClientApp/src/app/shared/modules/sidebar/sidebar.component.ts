import { Component, OnInit, ViewChild, Output, HostListener, Injector } from '@angular/core';
import { Router } from '@angular/router';
import * as $ from 'jquery';
import { LoginService } from '../../../login/login.service';

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
        this.route.navigate(['/layout/roles']);
    }
    navigateToUser() {
        this.route.navigate(['/layout/user']);
    }
    navigateToWidgets() {
        this.route.navigate(['/layout/widgets']);
    }
    navigateToDashoard() {
        this.route.navigate(['/layout/dashboard']);
    }
    navigateToTemplates() {
        this.route.navigate(['/layout/pages']);
    }
    navigateToChangePassword() {
        this.route.navigate(['/layout/changepassword']);
    }
    navigateToProfile() {
        this.route.navigate(['/layout/profile']);
    }
    navigateToScheduleManagement() {
        this.route.navigate(['/layout/schedulemanagement']);
    }
    navigateToStatementDef() {
        this.route.navigate(['/layout/statementdefination']);
    }
    navigateToAssetLibraries() {
        this.route.navigate(['/layout/assetlibrary']);
    }
    navigateToLogs() {
        this.route.navigate(['/layout/logs']);
    }
    navigateToAnalytics() {
        this.route.navigate(['/layout/analytics']);
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

        localStorage.removeItem('currentUserName');
        localStorage.removeItem('currentUserTheme');
        //localStorage.removeItem('user');
        localStorage.removeItem('userClaims');
        localStorage.removeItem('token');
        this.route.navigate(['login']);
    }

    constructor(private route: Router, 
        private injector: Injector,
        private loginService: LoginService) {
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
  }

}
