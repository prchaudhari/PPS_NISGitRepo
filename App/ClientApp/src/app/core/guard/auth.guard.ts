
import { Injectable, Injector } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, NavigationEnd } from '@angular/router';
import { Router } from '@angular/router';
import { FilterPipe } from '../../shared/pipes/filter.pipe';
import { LoginService } from '../../login/login.service';


@Injectable()
export class AuthGuard implements CanActivate {
    public userClaimsRolePrivilegeOperations;
    constructor(private router: Router,
        private authService: LoginService,
        private filter: FilterPipe,
        //private route: ActivatedRouteSnapshot,
        //private state: RouterStateSnapshot

    ) { }

    //CanAactivated added for gaurding normal routing
    canActivate(
        route: ActivatedRouteSnapshot,
        state: RouterStateSnapshot
    ): boolean {
        if (this.authService.loggedIn()) {
            return true
        }
        else {
            this.router.navigate(['login']);
            //return true;
        }
    }

    //CanLaod addd for gaurding lazy loaded routing--
    canLoad(
        route: ActivatedRouteSnapshot,
        state: RouterStateSnapshot
    ): boolean {
        if (this.authService.loggedIn()) {
            return true
        }
        else {
            //if (route.path != 'resetPassword') {
            this.router.navigate(['login']);
            //return true;
            //}
        }
    }


}

//CanActivate gaurd added to restrict the user to navigate to those pages who has not got access from rolepreviliges
//This gaurd is added to the respective routong file of each master
@Injectable()
export class UnAuthorisedUrlGuard implements CanActivate {
    public userClaimsRolePrivilegeOperations;

    constructor(private router: Router,
        private authService: LoginService,
        private filter: FilterPipe
    ) {
        var userClaimsDetail = JSON.parse(localStorage.getItem('userClaims'));
        this.userClaimsRolePrivilegeOperations = userClaimsDetail ? userClaimsDetail.Privileges : [];

    }

    canActivate(
        route: ActivatedRouteSnapshot,
        state: RouterStateSnapshot
    ): boolean {

        //EntityName and Operation is coming in the route param of canActivate from the routing page of each master--
        var searchText: { EntityName: string, Operation: string } = { EntityName: '', Operation: '' };
        searchText.EntityName = route.data.EntityName;
        searchText.Operation = route.data.Operation;
        if (this.filter.transform(this.userClaimsRolePrivilegeOperations, searchText)) {
            return true
        }
        else {
            this.router.navigate(['machinetiles']);
            //return true;
        }
    }

}



