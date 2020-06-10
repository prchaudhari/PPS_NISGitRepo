import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import * as $ from 'jquery';
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
        this.route.navigate(['/layout/users']);
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
    constructor(private route: Router) {
    }

    ngOnInit() {
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
