
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
        let tenantCode = currentUser.TenantCode;

        // Add URLs to be excluded, especially file upload URLs
        const excludedURLs = [
        '/asset/upload',
        ];

        if (excludedURLs.some(item => request.url.indexOf(item) == -1)) {
            request = request.clone({
              setHeaders: {
                'Authorization': 'Bearer ' + localStorage.getItem('token'),
                'Content-Type': 'application/json',
                'TenantCode': tenantCode
              }
            });
          }
          else {
            request = request.clone({
              setHeaders: {
                'Authorization': 'Bearer ' + localStorage.getItem('token'),
                'enctype': 'multipart/form-data',
                'TenantCode': tenantCode
              }
            });
          }

          return next.handle(request);            
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
