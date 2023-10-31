import { Injectable, Injector } from '@angular/core';
import { HttpClientService } from 'src/app/core/services/httpClient.service';
import { URLConfiguration } from 'src/app/shared/urlConfiguration/urlconfiguration';
import { HttpEvent, HttpEventType, HttpResponse, HttpClient, HttpHeaders } from '@angular/common/http';
import { Constants } from 'src/app/shared/constants/constants';
import { NgxUiLoaderService } from 'ngx-ui-loader';
import { MessageDialogService } from 'src/app/shared/services/mesage-dialog.service';

@Injectable({
  providedIn: 'root'
})

export class DataHubService {
  public eTLSchedulesList;
  public isRecordFound;
  public schedulesList;
  public isRecordSaved: boolean = false;
  public isRecordApproved: boolean = false;
  public isRecordDeleted: boolean = false;
  public isRecordRetry: boolean = false;

  constructor(private http: HttpClient,
    private injector: Injector,
    private uiLoader: NgxUiLoaderService,
    private _messageDialogService: MessageDialogService) { }

  public async getETLSchedules(searchParameter): Promise<any> {
    let httpClientService = this.injector.get(HttpClientService);
    let requestUrl = URLConfiguration.eTLScheduleGetUrl;
    this.uiLoader.start();
    var response: any = {};
    await httpClientService.CallHttp("POST", requestUrl, searchParameter).toPromise()
      .then((httpEvent: HttpEvent<any>) => {
        if (httpEvent.type == HttpEventType.Response) {
          if (httpEvent["status"] === 200) {
            this.eTLSchedulesList = [];
            this.uiLoader.stop();
            if (httpEvent['body'] != null) {
              httpEvent['body'].forEach(scheduleObject => {
                this.eTLSchedulesList = [...this.eTLSchedulesList, scheduleObject];
              });
              response.List = this.eTLSchedulesList;
              response.RecordCount = parseInt(httpEvent.headers.get('recordCount'));
            } else {
              response.List = this.eTLSchedulesList;
              response.RecordCount = 0;
            }
          }
          else {
            this.eTLSchedulesList = [];
            response.List = this.eTLSchedulesList;
            response.RecordCount = 0;
            this.isRecordFound = false;
            this.uiLoader.stop();
          }
        }
      }, (error: HttpResponse<any>) => {
        this.eTLSchedulesList = [];
        response.List = this.eTLSchedulesList;
        response.RecordCount = 0;
        this.isRecordFound = false;
        this.uiLoader.stop();
      });
    return response;
  }

  public async getETLScheduleBatchLogs(searchParameter): Promise<any> {
    let httpClientService = this.injector.get(HttpClientService);
    let requestUrl = URLConfiguration.eTLScheduleBatchLogGetUrl;
    this.uiLoader.start();
    var response: any = {};
    await httpClientService.CallHttp("POST", requestUrl, searchParameter).toPromise()
      .then((httpEvent: HttpEvent<any>) => {
        if (httpEvent.type == HttpEventType.Response) {
          if (httpEvent["status"] === 200) {
            this.eTLSchedulesList = [];
            this.uiLoader.stop();
            if (httpEvent['body'] != null) {
              httpEvent['body'].forEach(scheduleObject => {
                this.eTLSchedulesList = [...this.eTLSchedulesList, scheduleObject];
              });
              response.List = this.eTLSchedulesList;
              response.RecordCount = parseInt(httpEvent.headers.get('recordCount'));
            } else {
              response.List = this.eTLSchedulesList;
              response.RecordCount = 0;
            }
          }
          else {
            this.eTLSchedulesList = [];
            response.List = this.eTLSchedulesList;
            response.RecordCount = 0;
            this.isRecordFound = false;
            this.uiLoader.stop();
          }
        }
      }, (error: HttpResponse<any>) => {
        this.eTLSchedulesList = [];
        response.List = this.eTLSchedulesList;
        response.RecordCount = 0;
        this.isRecordFound = false;
        this.uiLoader.stop();
      });
    return response;
  }

  public async getETLScheduleDetailForBatchLogDetails(searchParameter): Promise<any> {
    let httpClientService = this.injector.get(HttpClientService);
    let requestUrl = URLConfiguration.eTLScheduleDetailForBatchLogDetailsGetUrl;
    this.uiLoader.start();
    var response: any;
    await httpClientService.CallHttp("POST", requestUrl, searchParameter).toPromise()
      .then((httpEvent: HttpEvent<any>) => {
        if (httpEvent.type == HttpEventType.Response) {
          if (httpEvent["status"] === 200) {
            this.uiLoader.stop();
            if (httpEvent['body'] != null) {
              response = httpEvent['body'];
            } else {
              response = null;
            }
          }
          else {
            response = null;
            this.uiLoader.stop();
          }
        }
      }, (error: HttpResponse<any>) => {
        response = null;
        this.uiLoader.stop();
      });
    return response;
  }

  public async getETLScheduleBatchLogDetails(searchParameter): Promise<any> {
    let httpClientService = this.injector.get(HttpClientService);
    let requestUrl = URLConfiguration.eTLScheduleBatchLogDetailsGetUrl;
    this.uiLoader.start();
    var response: any = {};
    await httpClientService.CallHttp("POST", requestUrl, searchParameter).toPromise()
      .then((httpEvent: HttpEvent<any>) => {
        if (httpEvent.type == HttpEventType.Response) {
          if (httpEvent["status"] === 200) {
            this.eTLSchedulesList = [];
            this.uiLoader.stop();
            if (httpEvent['body'] != null) {
              httpEvent['body'].forEach(scheduleObject => {
                this.eTLSchedulesList = [...this.eTLSchedulesList, scheduleObject];
              });
              response.List = this.eTLSchedulesList;
              response.RecordCount = parseInt(httpEvent.headers.get('recordCount'));
            } else {
              response.List = this.eTLSchedulesList;
              response.RecordCount = 0;
            }
          }
          else {
            this.eTLSchedulesList = [];
            response.List = this.eTLSchedulesList;
            response.RecordCount = 0;
            this.isRecordFound = false;
            this.uiLoader.stop();
          }
        }
      }, (error: HttpResponse<any>) => {
        this.eTLSchedulesList = [];
        response.List = this.eTLSchedulesList;
        response.RecordCount = 0;
        this.isRecordFound = false;
        this.uiLoader.stop();
      });
    return response;
  }

  public async getScheduleForDataHub(searchParameter): Promise<any> {
    let httpClientService = this.injector.get(HttpClientService);
    let requestUrl = URLConfiguration.eTLScheduleDetailGetUrl;
    this.uiLoader.start();
    var response: any = {};
    await httpClientService.CallHttp("POST", requestUrl, searchParameter).toPromise()
      .then((httpEvent: HttpEvent<any>) => {
        if (httpEvent.type == HttpEventType.Response) {
          if (httpEvent["status"] === 200) {
            this.schedulesList = [];
            this.uiLoader.stop();
            if (httpEvent['body'] != null) {
              httpEvent['body'].forEach(scheduleObject => {
                this.schedulesList = [...this.schedulesList, scheduleObject];
              });
              response.List = this.schedulesList;
              response.RecordCount = parseInt(httpEvent.headers.get('recordCount'));
            } else {
              response.List = this.schedulesList;
              response.RecordCount = 0;
            }
          }
          else {
            this.schedulesList = [];
            response.List = this.schedulesList;
            response.RecordCount = 0;
            this.isRecordFound = false;
            this.uiLoader.stop();
          }
        }
      }, (error: HttpResponse<any>) => {
        this.schedulesList = [];
        response.List = this.schedulesList;
        response.RecordCount = 0;
        this.isRecordFound = false;
        this.uiLoader.stop();
      });
    return response;
  }

  public async runETL(identifier): Promise<boolean> {
    let httpClientService = this.injector.get(HttpClientService);
    let requestUrl = URLConfiguration.runETLUrl;
    this.uiLoader.start();
    var response: any = {};
    await httpClientService.CallHttp("POST", requestUrl + "?etlBatchId=" + identifier, null).toPromise()
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

  public async approvedETLScheduleBatch(etlBatchId): Promise<boolean> {
    let httpClientService = this.injector.get(HttpClientService);
    let requestUrl = URLConfiguration.approvedETLBatch + "?" + "etlBatchId=" + etlBatchId;
    this.uiLoader.start();
    await httpClientService.CallHttp("POST", requestUrl).toPromise()
      .then((httpEvent: HttpEvent<any>) => {
        if (httpEvent.type == HttpEventType.Response) {
          this.uiLoader.stop();
          if (httpEvent["status"] === 200) {
            this.isRecordApproved = true;
          }
          else {
            this.isRecordApproved = false;
          }
        }
      }, (error) => {
        this._messageDialogService.openDialogBox('Error', error.error.Message, Constants.msgBoxError);
        this.uiLoader.stop();
        this.isRecordApproved = false;
      });
    return <boolean>this.isRecordApproved;
  }

  public async deleteETLScheduleBatch(etlBatchId): Promise<boolean> {
    let httpClientService = this.injector.get(HttpClientService);
    let requestUrl = URLConfiguration.deleteETLBatch + "?" + "etlBatchId=" + etlBatchId;
    this.uiLoader.start();
    await httpClientService.CallHttp("POST", requestUrl).toPromise()
      .then((httpEvent: HttpEvent<any>) => {
        if (httpEvent.type == HttpEventType.Response) {
          this.uiLoader.stop();
          if (httpEvent["status"] === 200) {
            this.isRecordDeleted = true;
          }
          else {
            this.isRecordDeleted = false;
          }
        }
      }, (error) => {
        this._messageDialogService.openDialogBox('Error', error.error.Message, Constants.msgBoxError);
        this.uiLoader.stop();
        this.isRecordDeleted = false;
      });
    return <boolean>this.isRecordDeleted;
  }

  public async retryETLScheduleBatchExecution(etlBatchId): Promise<boolean> {
    let httpClientService = this.injector.get(HttpClientService);
    let requestUrl = URLConfiguration.retryETLBatchExecution + "?" + "etlBatchId=" + etlBatchId;
    this.uiLoader.start();
    await httpClientService.CallHttp("POST", requestUrl).toPromise()
      .then((httpEvent: HttpEvent<any>) => {
        if (httpEvent.type == HttpEventType.Response) {
          this.uiLoader.stop();
          if (httpEvent["status"] === 200) {
            this.isRecordRetry = true;
          }
          else {
            this.isRecordRetry = false;
          }
        }
      }, (error) => {
        this._messageDialogService.openDialogBox('Error', error.error.Message, Constants.msgBoxError);
        this.uiLoader.stop();
        this.isRecordRetry = false;
      });
    return <boolean>this.isRecordRetry;
  }
}