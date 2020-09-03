import { Injectable, Injector } from '@angular/core';
import { HttpClientService } from 'src/app/core/services/httpClient.service';
import { URLConfiguration } from 'src/app/shared/urlConfiguration/urlconfiguration';
import { Observable } from 'rxjs';
import { HttpEvent, HttpEventType, HttpResponse, HttpClient, HttpHeaders } from '@angular/common/http';
import { Constants } from 'src/app/shared/constants/constants';
import { Schedule } from './schedule';
import { NgxUiLoaderService } from 'ngx-ui-loader';
import { MessageDialogService } from 'src/app/shared/services/mesage-dialog.service';
import { ConfigConstants } from 'src/app/shared/constants/configConstants';
import { ScheduleRunHistory } from './scheduleHitory';

@Injectable({
  providedIn: 'root'
})
export class ScheduleService {
  //public variables
  public schedulesList;
  public profileList;
  public designationList;
  public languageList;
  public isRecordFound;
  public isRecordSaved: boolean = false;
  public isRecordDeleted: boolean = false;
  public countrycodeList = [];
  public ouList = [];
  public isDependencyPresent: boolean = false;
  constructor(private http: HttpClient,
    private injector: Injector,
    private uiLoader: NgxUiLoaderService,
    private _messageDialogService: MessageDialogService) { }

  public async getSchedule(searchParameter): Promise<any> {
    let httpClientService = this.injector.get(HttpClientService);
    let requestUrl = URLConfiguration.scheduleGetUrl;
    this.uiLoader.start();
    var response : any = {};
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
            }else {
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

  public async getScheduleHistory(searchParameter): Promise<ScheduleRunHistory[]> {
    let httpClientService = this.injector.get(HttpClientService);
    let requestUrl = URLConfiguration.scheduleHistoryGetUrl;
    this.uiLoader.start();
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
            }
          }
          else {
            this.schedulesList = [];
            this.isRecordFound = false;
            this.uiLoader.stop();
          }
        }
      }, (error: HttpResponse<any>) => {
        this.schedulesList = [];
        this.isRecordFound = false;
        this.uiLoader.stop();
      });
    return <ScheduleRunHistory[]>this.schedulesList;
  }

  //method to call api of delete Schedule.
  public async deleteSchedule(postData): Promise<boolean> {
    let httpClientService = this.injector.get(HttpClientService);
    let requestUrl = URLConfiguration.scheduleDeleteUrl ;
    this.uiLoader.start();
    await httpClientService.CallHttp("POST", requestUrl, postData).toPromise()
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

  //This api is save schedule data--
  public async saveSchedule(postData, scheduleEditModeOn): Promise<boolean> {
    let httpClientService = this.injector.get(HttpClientService);
    let requestUrl = URLConfiguration.scheduleAddUrl;
    if (scheduleEditModeOn) {
      requestUrl = URLConfiguration.scheduleUpdateUrl;
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

  public async activate(postData): Promise<boolean> {
    let httpClientService = this.injector.get(HttpClientService);
    let requestUrl = URLConfiguration.scheduleActivate + "?" + "scheduleIdentifier=" + postData;
    this.uiLoader.start();
    await httpClientService.CallGetHttp("GET", requestUrl).toPromise()
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
      }, (error: HttpResponse<any>) => {
        this.uiLoader.stop();
        this.isRecordDeleted = false;
      });
    return <boolean>this.isRecordDeleted;
  }

  public async deactivate(postData): Promise<boolean> {
    let httpClientService = this.injector.get(HttpClientService);
    let requestUrl = URLConfiguration.scheduleDeactivate + "?" + "scheduleIdentifier=" + postData;
    this.uiLoader.start();
    await httpClientService.CallGetHttp("GET", requestUrl).toPromise()
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
      }, (error: HttpResponse<any>) => {
        this.uiLoader.stop();
        this.isRecordDeleted = false;
      });
    return <boolean>this.isRecordDeleted;
  }

  //method to call api of delete Schedule.
  public RunScheduleNow(postData) {
    let httpClientService = this.injector.get(HttpClientService);
    let requestUrl = URLConfiguration.RunScheduleNow ;
    httpClientService.CallHttp("POST", requestUrl, postData).toPromise()
    .then((httpEvent: HttpEvent<any>) => {

    }, (error: HttpResponse<any>) => {
        this.uiLoader.stop();
        this.isRecordDeleted = false;
      });     
  }

}
