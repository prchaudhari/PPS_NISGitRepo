
import { Injectable, Injector } from '@angular/core';
import { HttpClientService } from 'src/app/core/services/httpClient.service';
import { URLConfiguration } from 'src/app/shared/urlConfiguration/urlconfiguration';
import { Observable } from 'rxjs';
import { HttpEvent, HttpEventType, HttpResponse, HttpClient, HttpHeaders } from '@angular/common/http';
import { Constants } from 'src/app/shared/constants/constants';
import { ScheduleLogDetail } from './log-details';
import { NgxUiLoaderService } from 'ngx-ui-loader';
import { MessageDialogService } from 'src/app/shared/services/mesage-dialog.service';
import { ConfigConstants } from 'src/app/shared/constants/configConstants';

@Injectable({
  providedIn: 'root'
})
export class ScheduleLogServiceDetail {
  //public variables
  public schedulesList;
  public profileList;
  public designationList;
  public languageList;
  public isRecordFound;
  public isRecordSaved: boolean = false;
  public isRecordDeleted: boolean = false;
  public resultflag: boolean = false;
  public countrycodeList = [];
  public ouList = [];
  public isDependencyPresent: boolean = false;
  constructor(private http: HttpClient,
    private injector: Injector,
    private uiLoader: NgxUiLoaderService,
    private _messageDialogService: MessageDialogService) { }

  public async getScheduleLogDetail(searchParameter): Promise<any> {
    let httpClientService = this.injector.get(HttpClientService);
    let requestUrl = URLConfiguration.scheduleLogGetDetailUrl;
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
            }
            else {
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
  
  public async reRunSchdeulLogDetail(postData): Promise<boolean> {
    let httpClientService = this.injector.get(HttpClientService);
    let requestUrl = URLConfiguration.reRunScheduleLogDetailGetUrl ;
    this.uiLoader.start();
    await httpClientService.CallHttp("POST", requestUrl, postData).toPromise()
      .then((httpEvent: HttpEvent<any>) => {
        if (httpEvent.type == HttpEventType.Response) {
          this.uiLoader.stop();
          if (httpEvent["status"] === 200) {
            this.resultflag = true;
          }
          else {
            this.resultflag = false;
          }
        }
      }, (error: HttpResponse<any>) => {
        this.uiLoader.stop();
        this.resultflag = false;
      });
    return <boolean>this.resultflag;
  }
}
