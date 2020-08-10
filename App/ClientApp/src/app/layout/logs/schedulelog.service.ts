import { Injectable, Injector } from '@angular/core';
import { HttpClientService } from 'src/app/core/services/httpClient.service';
import { URLConfiguration } from 'src/app/shared/urlConfiguration/urlconfiguration';
import { Observable } from 'rxjs';
import { HttpEvent, HttpEventType, HttpResponse, HttpClient, HttpHeaders } from '@angular/common/http';
import { Constants } from 'src/app/shared/constants/constants';
import { ScheduleLog } from './schedulelog';
import { NgxUiLoaderService } from 'ngx-ui-loader';
import { MessageDialogService } from 'src/app/shared/services/mesage-dialog.service';
import { ConfigConstants } from 'src/app/shared/constants/configConstants';

@Injectable({
  providedIn: 'root'
})
export class ScheduleLogService {
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

  public async getScheduleLog(searchParameter): Promise<ScheduleLog[]> {
    let httpClientService = this.injector.get(HttpClientService);
    let requestUrl = URLConfiguration.scheduleLogGetUrl;
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
    return <ScheduleLog[]>this.schedulesList;
  }

  public async reRunSchdeulLog(scheduleLogIdentifier): Promise<boolean> {
    let httpClientService = this.injector.get(HttpClientService);
    let requestUrl = URLConfiguration.reRunScheduleLogGetUrl + "?" + "scheduleLogIdentifier=" + scheduleLogIdentifier;
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
}
