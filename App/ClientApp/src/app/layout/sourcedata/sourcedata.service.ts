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
  public schedulesList;
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
  public isDependencyPresent: boolean = false;
  constructor(private http: HttpClient,
    private injector: Injector,
    private uiLoader: NgxUiLoaderService,
    private _messageDialogService: MessageDialogService) { }

  public async getSourceData(searchParameter): Promise<SourceData[]> {
    let httpClientService = this.injector.get(HttpClientService);
    let requestUrl = "AnalyticsData/List";
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
    return <SourceData[]>this.schedulesList;
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
              this.VisitorForDay = httpEvent['body'];
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
    return <VisitorForDay>this.VisitorForDay;
  }

}
