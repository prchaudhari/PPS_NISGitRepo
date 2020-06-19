
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

    return next.handle(req);
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
