
import { Injectable, Injector } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpClient, HttpEvent, HttpEventType, HttpResponse } from '@angular/common/http';
import { NgxUiLoaderService } from 'ngx-ui-loader';
import { MessageDialogService } from 'src/app/shared/services/mesage-dialog.service';
import { URLConfiguration } from 'src/app/shared/urlConfiguration/urlconfiguration';
import { HttpClientService } from 'src/app/core/services/httpClient.service';
import { Constants } from 'src/app/shared/constants/constants';
import { DynamicWidget } from './dynamicWidget';

@Injectable({
  providedIn: 'root'
})
export class DynamicWidgetService {

  public dynamicWidgetList: DynamicWidget[] = [];
  public isRecordFound: boolean = false;
  public isRecordSaved: boolean = false;
  public isRecordDeleted = {};
  public resultFlag = {};

  constructor(private http: HttpClient,
    private injector: Injector,
    private uiLoader: NgxUiLoaderService,
    private _messageDialogService: MessageDialogService) { }

  //method to call api of get dynamicWidgets.
  async getDynamicWidgets(searchParameter): Promise<any> {
    let httpClientService = this.injector.get(HttpClientService);
    let requestUrl = URLConfiguration.dynamicWidgetGetUrl;
    this.uiLoader.start();
    var response: any = {};
    await httpClientService.CallHttp("POST", requestUrl, searchParameter).toPromise()
      .then((httpEvent: HttpEvent<any>) => {
        if (httpEvent.type == HttpEventType.Response) {
          if (httpEvent["status"] === 200) {
            this.dynamicWidgetList = [];
            this.uiLoader.stop();
            httpEvent['body'].forEach(dynamicWidgetObject => {
              this.dynamicWidgetList = [...this.dynamicWidgetList, dynamicWidgetObject];
            });
            response.List = this.dynamicWidgetList;
            response.RecordCount = parseInt(httpEvent.headers.get('recordCount'));
          }
          else {
            this.dynamicWidgetList = [];
            response.List = this.dynamicWidgetList;
            response.RecordCount = 0;
            this.uiLoader.stop();
          }
        }
      }, (error: HttpResponse<any>) => {
        this.dynamicWidgetList = [];
        response.List = this.dynamicWidgetList;
        response.RecordCount = 0;
        this.uiLoader.stop();
        if (error["error"] != null) {
          let errorMessage = error["error"].Error["Message"];
          this._messageDialogService.openDialogBox('Error', errorMessage, Constants.msgBoxError);
        }
      });
    return response
  }

  //service method to save or update dynamicWidget records
  public async saveDynamicWidget(postData, dynamicWidgetEditModeOn): Promise<boolean> {
    let httpClientService = this.injector.get(HttpClientService);
    let requestUrl = URLConfiguration.dynamicWidgetAddUrl;
    if (dynamicWidgetEditModeOn) {
      requestUrl = URLConfiguration.dynamicWidgetUpdateUrl;
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

  //method to call api of delete dynamicWidget.
  public async deleteDynamicWidget(postData): Promise<boolean> {
    let httpClientService = this.injector.get(HttpClientService);
    
    let requestUrl = URLConfiguration.dynamicWidgetDeleteUrl;
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
      }, (error) => {
        this._messageDialogService.openDialogBox('Error', error.error.Message, Constants.msgBoxError);
        this.uiLoader.stop();
        this.isRecordDeleted = false
      });
    return <boolean>this.isRecordDeleted;
  }

  //method to call api of publish dynamicWidget.
  public async publishDynamicWidget(postData): Promise<boolean> {
    let httpClientService = this.injector.get(HttpClientService);
    let identifier = null;
    if (postData.length > 0) {
      identifier = postData[0].Identifier;
    }
    let requestUrl = URLConfiguration.dynamicWidgetPublishUrl + '?widgetIdentifier=' + identifier;
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

  //method to call api of clone dynamicWidget.
  public async cloneDynamicWidget(postData): Promise<boolean> {
    let httpClientService = this.injector.get(HttpClientService);
    let identifier = null;
    if (postData.length > 0) {
      identifier = postData[0].Identifier;
    }
    let requestUrl = URLConfiguration.dynamicWidgetCloneUrl + '?dynamicWidgetIdentifier=' + identifier;
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

  //method to call api of preview page.
  public async previewDynamicWidget(postData): Promise<string> {
    let httpClientService = this.injector.get(HttpClientService);
    let identifier = null;
    if (postData.length > 0) {
      identifier = postData[0].Identifier;
    }

    let requestUrl = URLConfiguration.dynamicWidgetPreviewUrl + '?dynamicWidgetIdentifier=' + identifier;
    this.uiLoader.start();
    let resultString: string = "";

    await httpClientService.CallHttp("POST", requestUrl).toPromise()
      .then((httpEvent: HttpEvent<any>) => {
        if (httpEvent.type == HttpEventType.Response) {
          this.uiLoader.stop();
          if (httpEvent["status"] === 200) {
            resultString = httpEvent['body'];
          }
          else {
            resultString = '';
          }
        }
      }, (error) => {
        this._messageDialogService.openDialogBox('Error', error.error.Message, Constants.msgBoxError);
        this.uiLoader.stop();
        resultString = '';
      });
    return <string>resultString;
  }


  async getEntities(): Promise<any> {
    let httpClientService = this.injector.get(HttpClientService);
    let requestUrl = URLConfiguration.dynamicWidgetGetEntitiesUrl;
    this.uiLoader.start();
    let pageTypes: any[] = [];
    await httpClientService.CallHttp("POST", requestUrl).toPromise()
      .then((httpEvent: HttpEvent<any>) => {
        if (httpEvent.type == HttpEventType.Response) {
          if (httpEvent["status"] === 200) {
            pageTypes = [];
            this.uiLoader.stop();
            httpEvent['body'].forEach(pageType => {
              pageTypes = [...pageTypes, pageType];
            });
          }
          else {
            pageTypes = [];
            this.uiLoader.stop();
          }
        }
      }, (error: HttpResponse<any>) => {
        pageTypes = [];
        this.uiLoader.stop();
        if (error["error"] != null) {
          let errorMessage = error["error"].Error["Message"];
          this._messageDialogService.openDialogBox('Error', errorMessage, Constants.msgBoxError);
        }
      });
    return <any[]>pageTypes
  }

  async getEntityFields(id): Promise<any> {
    let httpClientService = this.injector.get(HttpClientService);
    let requestUrl = URLConfiguration.dynamicWidgetGetEntityFieldsUrl + '?entityIdentfier=' + id;
    this.uiLoader.start();
    let pageTypes: any[] = [];
    await httpClientService.CallHttp("POST", requestUrl).toPromise()
      .then((httpEvent: HttpEvent<any>) => {
        if (httpEvent.type == HttpEventType.Response) {
          if (httpEvent["status"] === 200) {
            pageTypes = [];
            this.uiLoader.stop();
            httpEvent['body'].forEach(pageType => {
              pageTypes = [...pageTypes, pageType];
            });
          }
          else {
            pageTypes = [];
            this.uiLoader.stop();
          }
        }
      }, (error: HttpResponse<any>) => {
        pageTypes = [];
        this.uiLoader.stop();
        if (error["error"] != null) {
          let errorMessage = error["error"].Error["Message"];
          this._messageDialogService.openDialogBox('Error', errorMessage, Constants.msgBoxError);
        }
      });
    return <any[]>pageTypes
  }
}
