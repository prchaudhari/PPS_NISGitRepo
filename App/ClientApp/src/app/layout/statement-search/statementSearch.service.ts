import { Injectable, Injector } from '@angular/core';
import { HttpClientService } from 'src/app/core/services/httpClient.service';
import { URLConfiguration } from 'src/app/shared/urlConfiguration/urlconfiguration';
import { Observable } from 'rxjs';
import { HttpEvent, HttpEventType, HttpResponse, HttpClient, HttpHeaders } from '@angular/common/http';
import { Constants } from 'src/app/shared/constants/constants';
import { StatementSearch } from './statementsearch';
import { NgxUiLoaderService } from 'ngx-ui-loader';
import { MessageDialogService } from 'src/app/shared/services/mesage-dialog.service';
import { ConfigConstants } from 'src/app/shared/constants/configConstants';

@Injectable({
  providedIn: 'root'
})
export class StatementSearchService {
  //public variables
  public searchlist;
  public isRecordFound = false;

  constructor(private http: HttpClient,
    private injector: Injector,
    private uiLoader: NgxUiLoaderService,
    private _messageDialogService: MessageDialogService) { }

  public async getStatementSearch(searchParameter): Promise<any> {
    let httpClientService = this.injector.get(HttpClientService);
    let requestUrl = URLConfiguration.statementSearchGetUrl;
    this.uiLoader.start();
    var response : any = {};
    await httpClientService.CallHttp("POST", requestUrl, searchParameter).toPromise()
      .then((httpEvent: HttpEvent<any>) => {
        if (httpEvent.type == HttpEventType.Response) {
          if (httpEvent["status"] === 200) {
            this.searchlist = [];
            this.uiLoader.stop();
            if (httpEvent['body'] != null) {
              httpEvent['body'].forEach(scheduleObject => {
                this.searchlist = [...this.searchlist, scheduleObject];
              });
              response.List = this.searchlist;
              response.RecordCount = parseInt(httpEvent.headers.get('recordCount'));
            }else {
              response.List = this.searchlist;
              response.RecordCount = 0;
            }
          }
          else {
            this.searchlist = [];
            response.List = this.searchlist;
            response.RecordCount = 0;
            this.isRecordFound = false;
            this.uiLoader.stop();
          }
        }
      }, (error: HttpResponse<any>) => {
        this.searchlist = [];
        response.List = this.searchlist;
        response.RecordCount = 0;
        this.isRecordFound = false;
        this.uiLoader.stop();
      });
    return response;
  }

}
