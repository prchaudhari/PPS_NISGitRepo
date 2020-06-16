import { Injectable, Injector } from '@angular/core';
import { NgxUiLoaderService } from 'ngx-ui-loader';
import { MessageDialogService } from '../shared/services/mesage-dialog.service';
import { HttpClientService } from '../core/services/httpClient.service';
import { URLConfiguration } from '../shared/urlConfiguration/urlconfiguration';
import { HttpEvent, HttpEventType, HttpResponse } from '@angular/common/http';
import { Constants } from '../shared/constants/constants';

@Injectable({
    providedIn: 'root'
})
export class AuthenticationService {
    public isRecordSaved: boolean = false;
    public isRecordFound: boolean = false;
    constructor(
        private injector: Injector,
        private uiLoader: NgxUiLoaderService,
        private _messageDialogService: MessageDialogService,
    ) { }

    //This api is called to save the changed password--
    public async savePassword(oldPassword, newPassword, currentUserEmailAddress): Promise<boolean> {
        let httpClientService = this.injector.get(HttpClientService);
        let requestUrl = URLConfiguration.changePasswordUrl + "?" + "userEmailAddress=" + currentUserEmailAddress + "&" + "oldPassword=" + oldPassword + "&" + "newPassword=" + newPassword;
        this.uiLoader.start();
        await httpClientService.CallHttp("POST", requestUrl).toPromise()
            .then((httpEvent: HttpEvent<any>) => {
                this.uiLoader.stop();
                if (httpEvent.type == HttpEventType.Response) {
                    if (httpEvent["status"] === 200) {
                        this.isRecordSaved = true;
                    }
                    else {
                        this.isRecordSaved = false;
                    }
                }
            }, (error: HttpResponse<any>) => {
                this.uiLoader.stop();
                let errorMessage = error["error"].Error["Message"];
                this._messageDialogService.openDialogBox('Error', errorMessage, Constants.msgBoxError);
            });
        return <boolean>this.isRecordSaved;
    }

    //This api is called to save the changed password--
    public async forgotPassword(resetPasswordValue): Promise<boolean> {
        let httpClientService = this.injector.get(HttpClientService);
        let requestUrl = URLConfiguration.forgotPasswordUrl + "?" + "userEmailAddress=" + resetPasswordValue;
        this.uiLoader.start();
        await httpClientService.CallHttp("POST", requestUrl, resetPasswordValue).toPromise()
            .then((httpEvent: HttpEvent<any>) => {
                this.uiLoader.stop();
                if (httpEvent.type == HttpEventType.Response) {
                    if (httpEvent["status"] === 200) {
                        this.isRecordSaved = true;
                    }
                    else {
                        this.isRecordSaved = false;
                    }
                }
            }, (error: HttpResponse<any>) => {
                this.uiLoader.stop();
                let errorMessage = error["error"].Error["Message"];
                this._messageDialogService.openDialogBox('Error', errorMessage, Constants.msgBoxError);
            });
        return <boolean>this.isRecordSaved;
    }


    //This api is called to save the changed password--
    public async setPassword(newPassword, token): Promise<boolean> {
        let httpClientService = this.injector.get(HttpClientService);
        let requestUrl = URLConfiguration.setPasswordUrl + "?" + "newPassword=" + newPassword + "&" + "encryptedText=" + token;
        this.uiLoader.start();
        await httpClientService.CallHttp("POST", requestUrl).toPromise()
            .then((httpEvent: HttpEvent<any>) => {
                this.uiLoader.stop();
                if (httpEvent.type == HttpEventType.Response) {
                    if (httpEvent["status"] === 200) {
                        this.isRecordSaved = true;
                    }
                    else {
                        this.isRecordSaved = false;
                    }
                }
            }, (error: HttpResponse<any>) => {
                this.uiLoader.stop();
                let errorMessage = error["error"].Error["Message"];
                this._messageDialogService.openDialogBox('Error', errorMessage, Constants.msgBoxError);
            });
        return <boolean>this.isRecordSaved;
    }
}
