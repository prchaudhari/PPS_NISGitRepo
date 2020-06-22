
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


    // Add URLs to be excluded, especially file upload URLs
    const excludedURLs = [
      'API/login', '/asset/upload'
    ];
    
    var currentUser = this.localstorageservice.GetCurrentUser();
    // Add Authorization Header If Already LoggedIn User
    if (currentUser) {
      headers['Authorization'] = currentUser.token_type + ' ' + currentUser.access_token;
      headers['TenantCode'] = currentUser.TenantCode;
      if (headers['Content-Type'] != null) {
        if (request.url != 'https://localhost:API/Login') {
          headers['Content-Type'] = 'application/json';
        }
      }
    }
    // Hold the request object
    const req = request.clone({
      setHeaders: headers
    });

    //return next.handle(req);

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
                        if (errorResponse instanceof HttpErrorResponse && errorResponse.status === 401) {
                            //Unauthorized
                            let errorMessage = "Sorry, your session has expired. Please login again..!!";
                            this.messageDialogService.openDialogBox('Session Timeout', errorMessage, Constants.msgBoxError);
                            this.localstorageservice.removeLocalStorageData();
                            const router = this.injector.get(Router);
                            router.navigate(['/login']);
                        }
                    }
                )
            );
  }

  handleError(error: Error | HttpErrorResponse) {
    if (error instanceof HttpErrorResponse) {
      // Server or connection error happened
      if (!navigator.onLine) {
        // Handle offline error
        console.log('Network error');
      } else {
        // Handle Http Error (error.status === 403, 404...)
      }
    } else {
      // Handle Client Error (Angular Error, ReferenceError...)     
    }
    // Log the error anyway
    console.error('It happens: ', error);
  }
}
