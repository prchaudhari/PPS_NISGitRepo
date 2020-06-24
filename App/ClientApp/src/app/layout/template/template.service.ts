import { Injectable, Injector } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpClient, HttpEvent, HttpEventType, HttpResponse } from '@angular/common/http';
import { NgxUiLoaderService } from 'ngx-ui-loader';
import { MessageDialogService } from 'src/app/shared/services/mesage-dialog.service';
import { URLConfiguration } from 'src/app/shared/urlConfiguration/urlconfiguration';
import { HttpClientService } from 'src/app/core/services/httpClient.service';
import { Constants } from 'src/app/shared/constants/constants';
import { Template } from 'src/app/layout/template/template';

@Injectable({
  providedIn: 'root'
})
export class TemplateService {

  public templateList: Template[] = [];
  public isRecordFound: boolean = false;
  public isRecordSaved: boolean = false;
  public isRecordDeleted = {};
  public resultFlag = {};

  constructor(private http: HttpClient,
    private injector: Injector,
    private uiLoader: NgxUiLoaderService,
    private _messageDialogService: MessageDialogService) { }

    //method to call api of get pages.
  async getTemplates(searchParameter): Promise<Template[]> {
    let httpClientService = this.injector.get(HttpClientService);
    let requestUrl = URLConfiguration.pageGetUrl;
    this.uiLoader.start();
    await httpClientService.CallHttp("POST", requestUrl, searchParameter).toPromise()
        .then((httpEvent: HttpEvent<any>) => {
            if (httpEvent.type == HttpEventType.Response) {
                if (httpEvent["status"] === 200) {
                    this.templateList = [];
                    this.uiLoader.stop();
                    httpEvent['body'].forEach(pageObject => {
                        this.templateList = [...this.templateList, pageObject];
                    });
                }
                else {
                    this.templateList = [];
                    this.uiLoader.stop();
                }
            }
        }, (error: HttpResponse<any>) => {
            this.templateList = [];
            this.uiLoader.stop();
            if (error["error"] != null) {
                let errorMessage = error["error"].Error["Message"];
                this._messageDialogService.openDialogBox('Error', errorMessage, Constants.msgBoxError);
            }
        });
    return <Template[]>this.templateList
}
}
