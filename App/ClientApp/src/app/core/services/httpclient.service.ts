import { AppSettings } from '../../appsettings';
import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpErrorResponse, HttpRequest } from '@angular/common/http';
import { map,catchError } from 'rxjs/operators';

import { Observable } from 'rxjs';
import { ConfigConstants } from 'src/app/shared/constants/configConstants';

@Injectable({
    providedIn: 'root'
  })
export class HttpClientService {
    private responseData: Observable<any>;
   
    constructor(private http: HttpClient) { 
        
    }

    /**Error Handling on request*/
    // private handleError(err: HttpErrorResponse) {
    //     return Observable.throw(err.message);
    // }
   
   
    public CallHttp(httpMethod: string, httpAction: string, requestData?: any, params?: any, header?:any) : Observable<any>{
        localStorage.setItem('LastRequestTime', (new Date()).toString());
      let url = AppSettings.baseURL + httpAction;
        httpMethod = httpMethod.toUpperCase();
        const req = new HttpRequest('POST', url, requestData, {
            reportProgress: true,
          });          
        this.responseData = this.http.request(req);
        return this.responseData;
    }

    public CallGetHttp(httpMethod: string, httpAction: string,  params?: any, header?:any) : Observable<any>{
      localStorage.setItem('LastRequestTime', (new Date()).toString());
      let url = AppSettings.baseURL + httpAction;
      httpMethod = httpMethod.toUpperCase();
      const req = new HttpRequest('GET', url);          
      this.responseData = this.http.request(req);
      return this.responseData;
  }
 
    

}
