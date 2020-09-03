import { Injectable, Injector } from '@angular/core';
import { HttpClientService } from 'src/app/core/services/httpClient.service';
import { URLConfiguration } from 'src/app/shared/urlConfiguration/urlconfiguration';
import { Observable } from 'rxjs';
import { HttpEvent, HttpEventType, HttpResponse, HttpClient, HttpHeaders } from '@angular/common/http';
import { Constants } from 'src/app/shared/constants/constants';
import { SourceData, WidgetVisitorPieChartData, PageWidgetVistorData, VisitorForDay} from './sourcedata';
import { NgxUiLoaderService } from 'ngx-ui-loader';
import { MessageDialogService } from 'src/app/shared/services/mesage-dialog.service';
import { ConfigConstants } from 'src/app/shared/constants/configConstants';

@Injectable({
  providedIn: 'root'
})
export class SourceDataService {
  //public variables
  public sourceData;
  public pieChartList;
  public profileList;
  public PageWidgetVistorData;
  public VisitorForDay;
  public designationList;
  public languageList;
  public isRecordFound;
  public isRecordSaved: boolean = false;
  public isRecordDeleted: boolean = false;
  public countrycodeList = [];
  public ouList = [];
  public DateWiseVisitorData;
  public isDependencyPresent: boolean = false;
  constructor(private http: HttpClient,
    private injector: Injector,
    private uiLoader: NgxUiLoaderService,
    private _messageDialogService: MessageDialogService) { }

  public async getSourceData(searchParameter): Promise<any> {
    let httpClientService = this.injector.get(HttpClientService);
    let requestUrl = "AnalyticsData/List";
    this.uiLoader.start();
    var response : any = {};
    await httpClientService.CallHttp("POST", requestUrl, searchParameter).toPromise()
      .then((httpEvent: HttpEvent<any>) => {
        if (httpEvent.type == HttpEventType.Response) {
          if (httpEvent["status"] === 200) {
            this.sourceData = [];
            this.uiLoader.stop();
            if (httpEvent['body'] != null) {
              httpEvent['body'].forEach(scheduleObject => {
                this.sourceData = [...this.sourceData, scheduleObject];
              });
              response.List = this.sourceData;
              response.RecordCount = parseInt(httpEvent.headers.get('recordCount'));
            }
          }
          else {
            this.sourceData = [];
            response.List = this.sourceData;
            response.RecordCount = 0;
            this.isRecordFound = false;
            this.uiLoader.stop();
          }
        }
      }, (error: HttpResponse<any>) => {
        this.sourceData = [];
        response.List = this.sourceData;
        response.RecordCount = 0;
        this.isRecordFound = false;
        this.uiLoader.stop();
      });
    return response;
  }

  public async getWidgetVisitorPieChartData(searchParameter): Promise<WidgetVisitorPieChartData[]> {
    let httpClientService = this.injector.get(HttpClientService);
    let requestUrl = "AnalyticsData/GetPieChartWidgeVisitor";
    this.uiLoader.start();
    await httpClientService.CallHttp("POST", requestUrl, searchParameter).toPromise()
      .then((httpEvent: HttpEvent<any>) => {
        if (httpEvent.type == HttpEventType.Response) {
          if (httpEvent["status"] === 200) {
            this.pieChartList = [];
            this.uiLoader.stop();
            if (httpEvent['body'] != null) {
              httpEvent['body'].forEach(scheduleObject => {
                this.pieChartList = [...this.pieChartList, scheduleObject];
              });
            }
          }
          else {
            this.pieChartList = [];
            this.isRecordFound = false;
            this.uiLoader.stop();
          }
        }
      }, (error: HttpResponse<any>) => {
          this.pieChartList = [];
        this.isRecordFound = false;
        this.uiLoader.stop();
      });
    return <WidgetVisitorPieChartData[]>this.pieChartList;
  }

  public async getPageWidgetVistorData(searchParameter): Promise<PageWidgetVistorData> {
    let httpClientService = this.injector.get(HttpClientService);
    let requestUrl = "AnalyticsData/GetPageWidgetVisitor";
    this.uiLoader.start();
    await httpClientService.CallHttp("POST", requestUrl, searchParameter).toPromise()
      .then((httpEvent: HttpEvent<any>) => {
        if (httpEvent.type == HttpEventType.Response) {
          if (httpEvent["status"] === 200) {
            this.pieChartList = [];
            this.uiLoader.stop();
            if (httpEvent['body'] != null) {
              //httpEvent['body'].forEach(scheduleObject => {
              //  this.pieChartList = [...this.pieChartList, scheduleObject];
              //});
              this.PageWidgetVistorData = httpEvent['body'];
            }
          }
          else {
            this.pieChartList = [];
            this.isRecordFound = false;
            this.uiLoader.stop();
          }
        }
      }, (error: HttpResponse<any>) => {
        this.pieChartList = [];
        this.isRecordFound = false;
        this.uiLoader.stop();
      });
    return <PageWidgetVistorData>this.PageWidgetVistorData;
  }

  public async getVisitorForDay(searchParameter): Promise<VisitorForDay> {
    let httpClientService = this.injector.get(HttpClientService);
    let requestUrl = "AnalyticsData/GeVisitorForDay";
    this.uiLoader.start();
    await httpClientService.CallHttp("POST", requestUrl, searchParameter).toPromise()
      .then((httpEvent: HttpEvent<any>) => {
        if (httpEvent.type == HttpEventType.Response) {
          if (httpEvent["status"] === 200) {
            this.VisitorForDay = [];
            this.uiLoader.stop();
            if (httpEvent['body'] != null) {
              //httpEvent['body'].forEach(scheduleObject => {
              //  this.pieChartList = [...this.pieChartList, scheduleObject];
              //});
              this.VisitorForDay = httpEvent['body'];
            }
          }
          else {
            this.VisitorForDay = [];
            this.isRecordFound = false;
            this.uiLoader.stop();
          }
        }
      }, (error: HttpResponse<any>) => {
          this.VisitorForDay = [];
        this.isRecordFound = false;
        this.uiLoader.stop();
      });
    return <VisitorForDay>this.VisitorForDay;
  }

  public async getVisitorForDate(searchParameter): Promise<any> {
    let httpClientService = this.injector.get(HttpClientService);
    let requestUrl = "AnalyticsData/GetDatewiseVisitor";
    this.uiLoader.start();
    await httpClientService.CallHttp("POST", requestUrl, searchParameter).toPromise()
      .then((httpEvent: HttpEvent<any>) => {
        if (httpEvent.type == HttpEventType.Response) {
          if (httpEvent["status"] === 200) {
            this.DateWiseVisitorData = [];
            this.uiLoader.stop();
            if (httpEvent['body'] != null) {
              //httpEvent['body'].forEach(scheduleObject => {
              //  this.pieChartList = [...this.pieChartList, scheduleObject];
              //});
              this.DateWiseVisitorData = httpEvent['body'];
            }
          }
          else {
            this.DateWiseVisitorData = [];
            this.isRecordFound = false;
            this.uiLoader.stop();
          }
        }
      }, (error: HttpResponse<any>) => {
          this.DateWiseVisitorData = [];
        this.isRecordFound = false;
        this.uiLoader.stop();
      });
    return <any>this.DateWiseVisitorData;
  }
}
