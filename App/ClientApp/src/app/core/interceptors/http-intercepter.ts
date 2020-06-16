
import { Injectable, Injector } from '@angular/core';
import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest, HttpErrorResponse, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { Router } from '@angular/router';
import { CommonService } from '../services/common.service';
import { LocalStorageService } from 'src/app/shared/services/local-storage.service';
import { MessageDialogService } from 'src/app/shared/services/mesage-dialog.service';
import { Constants } from 'src/app/shared/constants/constants';
import { NgxUiLoaderService } from 'ngx-ui-loader';


@Injectable()
export class HttpIntercepter implements HttpInterceptor {
    constructor(
        private commonService: CommonService,
        private injector: Injector,
        private localstorageservice: LocalStorageService,
        private messageDialogService: MessageDialogService,
        private uiLoader: NgxUiLoaderService,
    ) {
    }
    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        const headers = {};
        const router = this.injector.get(Router);
        var currentUser = this.localstorageservice.GetCurrentUser();
        // Add Authorization Header If Already LoggedIn User
        if (currentUser) {
            headers['Authorization'] = currentUser.token_type + ' ' + currentUser.access_token;
            //headers['TenantCode'] = currentUser.TenantCode;
            if (headers['Content-Type'] != null) {
                if (request.url != 'https://localhost:44347/Login') {
                    headers['Content-Type'] = 'application/json';
                }
            }
            //headers['IPAddress'] = '185.93.3.123';
            //headers['userIdentifer'] = "1";
        }
        // Hold the request object
        const req = request.clone({
            setHeaders: headers
        });

        return next.handle(req)
            .pipe(
                tap(
                    (successResponse: HttpResponse<any>) => {
                        if (successResponse instanceof HttpResponse && successResponse['body'] && successResponse['body']['status'] === 400) {
                            if ('Unauthorized access.' == successResponse['body']['message']) {
                                //this.toastr.error(successResponse['body']['message']);
                                const router = this.injector.get(Router);
                                //router.navigate(['/login']);
                            }
                        }
                        //unautorised access
                        if (successResponse instanceof HttpResponse && successResponse['body'] && successResponse['body']['status'] === 401) {
                            // this.commonService.setEvent('isSessionExpired', true);                
                            //this.toastr.error(successResponse['body']['message']);
                            const router = this.injector.get(Router);
                            //router.navigate(['/login']);
                        }
                        if (successResponse instanceof HttpResponse && successResponse['body'] && successResponse['body']['status'] === 500) {
                            //this.toastr.error(successResponse['body']['message']);
                            const router = this.injector.get(Router);
                            //router.navigate(['/login']);
                        }
                    },
                    (errorResponse: HttpErrorResponse) => {
                        this.uiLoader.stop();
                        //if (navigator.onLine) {
                        if (errorResponse instanceof HttpErrorResponse && errorResponse.status === 400) {
                            //Bad Request
                            let urlArr = errorResponse.url.split('/');
                            if (urlArr[urlArr.length - 1] != "login") {
                                if (errorResponse.error != null) {
                                    if (errorResponse.error.Error) {
                                        let errorMessage = "";
                                        if (errorResponse.error.Error.ErrorData.length > 0) {
                                            errorResponse.error.Error.ErrorData.forEach(error => {
                                                error.Messages.forEach(message => {
                                                    errorMessage += "" + message + " ";
                                                })
                                            })
                                        }
                                        else {
                                            errorMessage = errorResponse.error.Error.Message;
                                        }
                                        this.messageDialogService.openDialogBox('Error', errorMessage, Constants.msgBoxError);
                                    }
                                }
                            }
                        }
                        if (errorResponse instanceof HttpErrorResponse && errorResponse.status === 401) {
                            //Unauthorized                                
                            let errorMessage = errorResponse.error;
                            this.messageDialogService.openDialogBox('Error', errorMessage, Constants.msgBoxError);
                            const router = this.injector.get(Router);
                            localStorage.removeItem('currentUserName');
                            localStorage.removeItem('user');
                            localStorage.removeItem('userClaims');
                            localStorage.removeItem('token');
                            localStorage.removeItem('currentUserTheme');
                            router.navigate(['/login']);
                        }
                        //    //if (errorResponse instanceof HttpErrorResponse && errorResponse.status === 403) {
                        //    //    //Forbidden
                        //    //    let errorMessage = errorResponse.error;
                        //    //    this.messageDialogService.openDialogBox('Error', errorMessage, Constants.msgBoxError);
                        //    //    const router = this.injector.get(Router);
                        //    //    router.navigate(['/login']);
                        //    //}
                        //    if (errorResponse instanceof HttpErrorResponse && errorResponse['body']['status'] === 404) {
                        //        //Not Found
                        //        let errorMessage = 'Server Not Found';
                        //        this.messageDialogService.openDialogBox('Error', errorMessage, Constants.msgBoxError);
                        //    }
                        //    //if (errorResponse instanceof HttpErrorResponse && errorResponse.status === 405) {
                        //    //    //Method Not Allowed
                        //    //    let errorMessage = errorResponse.error;
                        //    //    this.messageDialogService.openDialogBox('Error', errorMessage, Constants.msgBoxError);
                        //    //    const router = this.injector.get(Router);
                        //    //    router.navigate(['/login']);
                        //    //}
                        //    else {
                        //        let errorMessage = 'Something went wrong';
                        //        this.messageDialogService.openDialogBox('Error', errorMessage, Constants.msgBoxError);
                        //    }
                        //}
                        //else {
                        //    let errorMessage = 'Network Connection Lost';
                        //    this.messageDialogService.openDialogBox('Error', errorMessage, Constants.msgBoxError);
                        //}
                    }
                )
            );
    }
}
