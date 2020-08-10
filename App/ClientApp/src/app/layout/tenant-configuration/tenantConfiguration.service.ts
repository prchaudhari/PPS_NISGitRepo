
import { Injectable, Injector } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpClient, HttpEvent, HttpEventType, HttpResponse } from '@angular/common/http';
import { NgxUiLoaderService } from 'ngx-ui-loader';
import { MessageDialogService } from 'src/app/shared/services/mesage-dialog.service';
import { URLConfiguration } from 'src/app/shared/urlConfiguration/urlconfiguration';
import { HttpClientService } from 'src/app/core/services/httpClient.service';
import { Constants } from 'src/app/shared/constants/constants';
import { TenantConfiguration } from './tenatconfiguration';

@Injectable({
  providedIn: 'root'
})
export class TenantConfigurationService {

  public accessToken;
  public renderEngines: TenantConfiguration[] = [];
  public isRecordFound: boolean = false;
  public isRecordSaved: boolean = false;
  public isDependencyPresent: boolean = false;
  public isRecordDeleted = {};
  public resultFlag = {};

  constructor(private http: HttpClient,
    private injector: Injector,
    private uiLoader: NgxUiLoaderService,
    private _messageDialogService: MessageDialogService) { }


  //method to call api of get render engine.
  async getTenantConfigurations(searchParameter): Promise<TenantConfiguration[]> {
    let httpClientService = this.injector.get(HttpClientService);
    let requestUrl = "TenantConfiguration/list";
    this.uiLoader.start();
    await httpClientService.CallHttp("POST", requestUrl, searchParameter).toPromise()
      .then((httpEvent: HttpEvent<any>) => {
        if (httpEvent.type == HttpEventType.Response) {
          if (httpEvent["status"] === 200) {
            this.renderEngines = [];
            this.uiLoader.stop();
            httpEvent['body'].forEach(roleObject => {
              this.renderEngines = [...this.renderEngines, roleObject];
            });
          }
          else {
            this.renderEngines = [];
            this.uiLoader.stop();
          }
        }
      }, (error: HttpResponse<any>) => {
        this.renderEngines = [];
        this.uiLoader.stop();
        if (error["error"] != null) {
          let errorMessage = error["error"].Error["Message"];
          this._messageDialogService.openDialogBox('Error', errorMessage, Constants.msgBoxError);
        }
      });
    return <TenantConfiguration[]>this.renderEngines
  }

  //service method to save or update render engine records
  public async saveTenantConfiguration(postData, renderEngineEditModeOn): Promise<boolean> {
    let httpClientService = this.injector.get(HttpClientService);
    let requestUrl = URLConfiguration.renderEngineAddUrl;
    if (renderEngineEditModeOn) {
      requestUrl = URLConfiguration.renderEngineUpdateUrl;
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

  //method to call api of delete render engine.
  public async deleteTenantConfiguration(postData): Promise<boolean> {
    let httpClientService = this.injector.get(HttpClientService);
    let requestUrl = URLConfiguration.renderEngineDeleteUrl;
    this.uiLoader.start();
    await httpClientService.CallHttp("POST", requestUrl, postData).toPromise()
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
      }, (error: HttpResponse<any>) => {
        this.uiLoader.stop();
        this.isRecordDeleted = false
      });
    return <boolean>this.isRecordDeleted;
  }

  //method to call api of deactivate render engine.
  public async deactivateTenantConfiguration(postData): Promise<boolean> {
    let httpClientService = this.injector.get(HttpClientService);
    let identifier = null;
    if (postData.length > 0) {
      identifier = postData[0].Identifier;
    }
    let requestUrl = URLConfiguration.renderEngineDeactivate + '?renderEngineIdentifier=' + identifier;
    this.uiLoader.start();
    await httpClientService.CallGetHttp("GET", requestUrl).toPromise()
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
      }, (error: HttpResponse<any>) => {
        this.uiLoader.stop();
        this.resultFlag = false
      });
    return <boolean>this.resultFlag;
  }

  //method to call api of activate render engine.
  public async activateTenantConfiguration(postData): Promise<boolean> {
    let httpClientService = this.injector.get(HttpClientService);
    let identifier = null;
    if (postData.length > 0) {
      identifier = postData[0].Identifier;
    }
    let requestUrl = URLConfiguration.renderEngineActivate + '?renderEngineIdentifier=' + identifier;
    this.uiLoader.start();
    await httpClientService.CallGetHttp("GET", requestUrl).toPromise()
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
      }, (error: HttpResponse<any>) => {
        this.uiLoader.stop();
        this.resultFlag = false
      });
    return <boolean>this.resultFlag;
  }

}
