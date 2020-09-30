import { Injectable, Injector } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpClient, HttpEvent, HttpEventType, HttpResponse } from '@angular/common/http';
import { NgxUiLoaderService } from 'ngx-ui-loader';
import { MessageDialogService } from 'src/app/shared/services/mesage-dialog.service';
import { URLConfiguration } from 'src/app/shared/urlConfiguration/urlconfiguration';
import { HttpClientService } from 'src/app/core/services/httpClient.service';
import { Constants } from 'src/app/shared/constants/constants';

@Injectable({
  providedIn: 'root'
})
export class MultiTenantUserAccessMapService {

  public muliTenantUserRoleMappingList: any[] = [];
  public isRecordFound: boolean = false;
  public isRecordSaved: boolean = false;
  public isRecordDeleted = {};
  public resultFlag = {};

  constructor(private http: HttpClient,
    private injector: Injector,
    private uiLoader: NgxUiLoaderService,
    private _messageDialogService: MessageDialogService) { }

  //method to call api of get multi-tenant user role access list.
  async getMultiTenantUserRoleMappingList(searchParameter): Promise<any> {
    let httpClientService = this.injector.get(HttpClientService);
    let requestUrl = URLConfiguration.multiTenantUserRoleAccessGetUrl;
    this.uiLoader.start();
    var response : any = {};
    await httpClientService.CallHttp("POST", requestUrl, searchParameter).toPromise()
      .then((httpEvent: HttpEvent<any>) => {
        if (httpEvent.type == HttpEventType.Response) {
            if (httpEvent["status"] === 200) {
                this.muliTenantUserRoleMappingList = [];
                this.uiLoader.stop();
                httpEvent['body'].forEach(record => {
                    this.muliTenantUserRoleMappingList = [...this.muliTenantUserRoleMappingList, record];
                });
                response.List = this.muliTenantUserRoleMappingList;
                response.RecordCount = parseInt(httpEvent.headers.get('recordCount'));
            }
            else {
                this.muliTenantUserRoleMappingList = [];
                response.List = this.muliTenantUserRoleMappingList;
                response.RecordCount = 0;
                this.uiLoader.stop();
            }
        }
      }, (error: HttpResponse<any>) => {
        this.muliTenantUserRoleMappingList = [];
        this.uiLoader.stop();
        if (error["error"] != null) {
            let errorMessage = error["error"].Error["Message"];
            this._messageDialogService.openDialogBox('Error', errorMessage, Constants.msgBoxError);
        }
      });
    return response
  }

  //service method to save or update multi-tenant user role access mapping records
  public async saveMultiTenantUserRoleAccess(postData, editModeOn): Promise<boolean> {
    let httpClientService = this.injector.get(HttpClientService);
    let requestUrl = URLConfiguration.multiTenantUserRoleAccessAddUrl;
    if (editModeOn) {
        requestUrl = URLConfiguration.multiTenantUserRoleAccessUpdateUrl;
    }
    this.uiLoader.start();
    await httpClientService.CallHttp("POST", requestUrl, postData).toPromise()
        .then((httpEvent: HttpEvent<any>) => {
            if (httpEvent.type == HttpEventType.Response) {
                this.uiLoader.stop();
                if (httpEvent["status"] === 200) {
                    this.isRecordSaved = true;
                }
                else {
                    this.isRecordSaved = false;
                }
            }
        }, (error) => {
            this._messageDialogService.openDialogBox('Error', error.error.Message, Constants.msgBoxError);
            this.isRecordSaved = false;
            this.uiLoader.stop();
        });
    return <boolean>this.isRecordSaved;
  }

  //method to call api of delete multi-tenant user role access.
  public async deleteMultiTenantUserRoleAccess(postData): Promise<boolean> {
    let httpClientService = this.injector.get(HttpClientService);
    let identifier = null;
    if (postData.length > 0) {
      identifier = postData[0].Identifier;
    }
    let requestUrl = URLConfiguration.multiTenantUserRoleAccessDeleteUrl + '?identifier=' + identifier;
    this.uiLoader.start();
    await httpClientService.CallHttp("POST", requestUrl).toPromise()
      .then((httpEvent: HttpEvent<any>) => {
        if (httpEvent.type == HttpEventType.Response) {
          this.uiLoader.stop();
          if (httpEvent["status"] === 200) {
            this.isRecordDeleted = true
          }
          else {
            this.isRecordDeleted = false
          }
        }
      }, (error) => {
        this._messageDialogService.openDialogBox('Error', error.error.Message, Constants.msgBoxError);
        this.uiLoader.stop();
        this.isRecordDeleted = false
      });
    return <boolean>this.isRecordDeleted;
  }

  //method to call api of activate multi-tenant user role access.
  public async activateMultiTenantUserRoleAccess(postData): Promise<boolean> {
    let httpClientService = this.injector.get(HttpClientService);
    let identifier = null;
    if (postData.length > 0) {
      identifier = postData[0].Identifier;
    }
    let requestUrl = URLConfiguration.multiTenantUserRoleAccessActivate + '?identifier=' + identifier;
    this.uiLoader.start();
    await httpClientService.CallHttp("POST", requestUrl).toPromise()
      .then((httpEvent: HttpEvent<any>) => {
        if (httpEvent.type == HttpEventType.Response) {
          this.uiLoader.stop();
          if (httpEvent["status"] === 200) {
            this.resultFlag = true
          }
          else {
            this.resultFlag = false
          }
        }
      }, (error) => {
        this._messageDialogService.openDialogBox('Error', error.error.Message, Constants.msgBoxError);
        this.uiLoader.stop();
        this.resultFlag = false
      });
    return <boolean>this.resultFlag;
  }

  //method to call api of deactivate multi-tenant user role access.
  public async deactivateMultiTenantUserRoleAccess(postData): Promise<boolean> {
    let httpClientService = this.injector.get(HttpClientService);
    let identifier = null;
    if (postData.length > 0) {
      identifier = postData[0].Identifier;
    }
    let requestUrl = URLConfiguration.multiTenantUserRoleAccessDeactivate + '?identifier=' + identifier;
    this.uiLoader.start();
    await httpClientService.CallHttp("POST", requestUrl).toPromise()
      .then((httpEvent: HttpEvent<any>) => {
        if (httpEvent.type == HttpEventType.Response) {
          this.uiLoader.stop();
          if (httpEvent["status"] === 200) {
            this.resultFlag = true
          }
          else {
            this.resultFlag = false
          }
        }
      }, (error) => {
        this._messageDialogService.openDialogBox('Error', error.error.Message, Constants.msgBoxError);
        this.uiLoader.stop();
        this.resultFlag = false
      });
    return <boolean>this.resultFlag;
  }

}
