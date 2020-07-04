
import { Injectable, Injector } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpClient, HttpEvent, HttpEventType, HttpResponse } from '@angular/common/http';
import { NgxUiLoaderService } from 'ngx-ui-loader';
import { MessageDialogService } from 'src/app/shared/services/mesage-dialog.service';
import { URLConfiguration } from 'src/app/shared/urlConfiguration/urlconfiguration';
import { HttpClientService } from 'src/app/core/services/httpClient.service';
import { Constants } from 'src/app/shared/constants/constants';
import { Widget } from './widget';

@Injectable({
  providedIn: 'root'
})
export class WidgetService {

  public accessToken;
  public widgetList: Widget[] = [];
  public isRecordFound: boolean = false;
  public isRecordSaved: boolean = false;
  public isDependencyPresent: boolean = false;
  public isRecordDeleted = {};
  public resultFlag = {};

  constructor(private http: HttpClient,
    private injector: Injector,
    private uiLoader: NgxUiLoaderService,
    private _messageDialogService: MessageDialogService) { }

  //method to call api of get asset library.
  async getWidget(searchParameter): Promise<Widget[]> {
    let httpClientService = this.injector.get(HttpClientService);
    let requestUrl = URLConfiguration.widgetGetUrl;
    this.uiLoader.start();
    await httpClientService.CallHttp("POST", requestUrl, searchParameter).toPromise()
      .then((httpEvent: HttpEvent<any>) => {
        if (httpEvent.type == HttpEventType.Response) {
          if (httpEvent["status"] === 200) {
            this.widgetList = [];
            this.uiLoader.stop();
            httpEvent['body'].forEach(roleObject => {
              this.widgetList = [...this.widgetList, roleObject];
            });
          }
          else {
            this.widgetList = [];
            this.uiLoader.stop();
          }
        }
      }, (error: HttpResponse<any>) => {
        this.widgetList = [];
        this.uiLoader.stop();
        if (error["error"] != null) {
          let errorMessage = error["error"].Error["Message"];
          this._messageDialogService.openDialogBox('Error', errorMessage, Constants.msgBoxError);
        }
      });
    return <Widget[]>this.widgetList
  }



}
