import { Injectable } from '@angular/core';
import { HttpClient, HttpResponse } from '@angular/common/http';
// import { LoginService } from 'src/app/authentication/login/login.service';
@Injectable({
    providedIn: 'root'
})
export class LocalStorageService {
    CurrentUser = null;
    CurrentContext = null;
    currentConnectionStrings = null;
    clientCustomer = null;
    resourcesList: any = [];

    constructor(private http: HttpClient) { }

    //SetTexfabClients(texfabClients) {
    //    this.clientCustomer = JSON.stringify(texfabClients);
    //    localStorage.setItem('clientCustomer', JSON.stringify(this.clientCustomer))
    //}

    //GetTexfabClients() {
    //    return JSON.parse(localStorage.getItem('clientCustomer'));
    //}

    SetCurrentUser(user) {
        this.CurrentUser = JSON.stringify(user);
        localStorage.setItem('user', this.CurrentUser);
    }

    GetCurrentUser() {
        return JSON.parse(localStorage.getItem('user'));
    }

    //GetCurrentContext() {
    //    return JSON.parse(localStorage.getItem('currentContext'));
    //}

    //GetCurrentConnectionStrings() {
    //    return JSON.parse(localStorage.getItem('currentConnectionStrings'));
    //}

    //Function call to set the resource list from api--
    SetResource(resourcesList) {
        localStorage.setItem('AuthorisedResources', JSON.stringify(resourcesList));
    }

    //Function call to get the resource list--
    GetResource() {
        return JSON.parse(localStorage.getItem('AuthorisedResources'));
    }
}
