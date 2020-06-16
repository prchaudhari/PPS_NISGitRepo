
import { Injectable, Injector } from '@angular/core';
import { HttpClient, HttpEvent, HttpEventType, HttpResponse } from '@angular/common/http';
import { ConfigConstants } from 'src/app/shared/constants/configConstants';
import { LocalStorageService } from 'src/app/shared/services/local-storage.service';
import { NgxUiLoaderService } from 'ngx-ui-loader';
import { MessageDialogService } from 'src/app/shared/services/mesage-dialog.service';
import { URLConfiguration } from 'src/app/shared/urlConfiguration/urlconfiguration';
import { HttpClientService } from 'src/app/core/services/httpClient.service';
import { Constants } from 'src/app/shared/constants/constants';
import { Role } from 'src/app/layout/roles/role';


@Injectable({
    providedIn: 'root'
})
export class LoginService {

    public accessToken;
    public userClaimsRolePrivilegeOperations;
    public islogOut: boolean = false;
    public roleList: Role[] = [];

    constructor(private http: HttpClient,
        private localstorageservice: LocalStorageService,
        private injector: Injector,
        private uiLoader: NgxUiLoaderService,
        private _messageDialogService: MessageDialogService) { }

    getLoginDetails(loginObj) {
        function ObjectsToParams(loginObj) {
            var p = [];
            for (var key in loginObj) {
                p.push(key + '=' + encodeURIComponent(loginObj[key]));
            }
            return p.join('&');
        }
        return this.http.post(
            ConfigConstants.BaseURL + "login",
            ObjectsToParams(loginObj),
            {
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded',
                    "IPAddress": '185.93.3.123'
                },
                observe: 'response'
            }
        );
    }

    //Function for getting token after login--
    loggedIn() {
        return !!localStorage.getItem('token')
    }

    //Function to logout user--
    async logoutUser(data) {
        let httpClientService = this.injector.get(HttpClientService);
        let requestUrl = URLConfiguration.logoutUrl;
        this.uiLoader.start();
        await httpClientService.CallHttp("POST", requestUrl).toPromise()
            .then((httpEvent: HttpEvent<any>) => {
                if (httpEvent.type == HttpEventType.Response) {
                    this.uiLoader.stop();
                    if (httpEvent["status"] === 200) {
                        this.islogOut = true;
                    }
                    else {
                        this.islogOut = false;
                    }
                }
            }, (error: HttpResponse<any>) => {
                this.uiLoader.stop();
                if (error["error"] != null) {
                    let errorMessage = error["error"].Error["Message"];
                    this._messageDialogService.openDialogBox('Error', errorMessage, Constants.msgBoxError);
                }
            });
        return <boolean>this.islogOut;

    }

    //method to call api of get role.
    async getRoles(searchParameter): Promise<Role[]> {
        let httpClientService = this.injector.get(HttpClientService);
        let requestUrl = URLConfiguration.roleGetUrl;
        this.uiLoader.start();
        await httpClientService.CallHttp("POST", requestUrl, searchParameter).toPromise()
            .then((httpEvent: HttpEvent<any>) => {
                if (httpEvent.type == HttpEventType.Response) {
                    if (httpEvent["status"] === 200) {
                        this.roleList = [];
                        this.uiLoader.stop();
                        httpEvent['body'].forEach(roleObject => {
                            this.roleList = [...this.roleList, roleObject];
                        });
                    }
                    else {
                        this.roleList = [];
                        this.uiLoader.stop();
                    }
                }
            }, (error: HttpResponse<any>) => {
                this.roleList = [];
                this.uiLoader.stop();
                if (error["error"] != null) {
                    let errorMessage = error["error"].Error["Message"];
                    this._messageDialogService.openDialogBox('Error', errorMessage, Constants.msgBoxError);
                }
            });
        return <Role[]>this.roleList
    }
}

