import { Injectable, Injector } from '@angular/core';
import { HttpClientService } from 'src/app/core/services/httpClient.service';
import { URLConfiguration } from 'src/app/shared/urlConfiguration/urlconfiguration';
import { Observable } from 'rxjs';
import { HttpEvent, HttpEventType, HttpResponse, HttpClient, HttpHeaders } from '@angular/common/http';
import { Constants } from 'src/app/shared/constants/constants';
import { NgxUiLoaderService } from 'ngx-ui-loader';
import { MessageDialogService } from 'src/app/shared/services/mesage-dialog.service';
import { ConfigConstants } from 'src/app/shared/constants/configConstants';

@Injectable({
  providedIn: 'root'
})
export class DashboardReportService {
  //public variables
  public dashboardReport;
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

  public async getDashboardReport(searchParameter): Promise<any> {
    let httpClientService = this.injector.get(HttpClientService);
    let requestUrl = "AnalyticsData/GetInstanceManagerDashboard";
    if (!searchParameter.IsInstanceManager) {
      requestUrl = "AnalyticsData/GetGroupManagerDashboard";
    }
    this.uiLoader.start();
    var response : any = {};
    await httpClientService.CallHttp("POST", requestUrl, searchParameter).toPromise()
      .then((httpEvent: HttpEvent<any>) => {
        if (httpEvent.type == HttpEventType.Response) {
          if (httpEvent["status"] === 200) {
            this.dashboardReport = {};
            this.uiLoader.stop();
            if (httpEvent['body'] != null) {
            
              this.dashboardReport = httpEvent['body'];
              response.List = this.dashboardReport;
              //response.RecordCount = parseInt(httpEvent.headers.get('recordCount'));
            }
          }
          else {
            this.dashboardReport = [];
            response.List = this.dashboardReport;
            response.RecordCount = 0;
            this.isRecordFound = false;
            this.uiLoader.stop();
          }
        }
      }, (error: HttpResponse<any>) => {
        this.dashboardReport = [];
        response.List = this.dashboardReport;
        response.RecordCount = 0;
        this.isRecordFound = false;
        this.uiLoader.stop();
      });
    return response;
  }

 
}
