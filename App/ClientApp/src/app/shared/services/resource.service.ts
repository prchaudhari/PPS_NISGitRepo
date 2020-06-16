import { Injectable, Injector } from '@angular/core';
import { HttpClientService } from 'src/app/core/services/httpClient.service';
import { URLConfiguration } from '../urlConfiguration/urlconfiguration';
import { HttpEvent, HttpEventType, HttpResponse, HttpClient, HttpHeaders } from '@angular/common/http';
import { NgxUiLoaderService } from 'ngx-ui-loader';
import { MessageDialogService } from './mesage-dialog.service';
import { Constants } from '../constants/constants';
import { SortOrder } from '../enums/sort-order.enum';
import { ConfigConstants } from '../constants/configConstants';

@Injectable({
    providedIn: 'root'
})
export class ResourceService {

    public resourcesList = [];
    public Resources = {};
    public ResourcesArr: any = [];

    constructor(private injector: Injector,
        private uiLoader: NgxUiLoaderService,
        private _messageDialogService: MessageDialogService,
        private http: HttpClient) { }


    //method to call api of get allowances.
    async getResources(resourceSectionName, locale, isLoginFlag) {
        var searchParameter = {
            "PagingParameter": {
                "PageIndex": 0,
                "PageSize": 0
            },
            "SortParameter": {
                "SortColumn": "SectionName",
                "SortOrder": SortOrder.Ascending
            },
            "SectionName": resourceSectionName,
            "Locale": locale,

        }
        let headers = new HttpHeaders({
            'Content-Type': 'application/json',
            'TenantCode': ConfigConstants.TenantCode
        })
        this.Resources = {}
        this.uiLoader.start();
        await this.http.post(ConfigConstants.ResourceUrl, searchParameter, { headers }).toPromise()
            .then((httpEvent: HttpEvent<any>) => {
                this.uiLoader.stop();
                if (httpEvent) {
                    this.ResourcesArr = httpEvent
                    if (this.ResourcesArr.length > 0) {
                        this.ResourcesArr[0].ResourceSections.forEach(resourceSection => {
                            resourceSection.ResourceItems.forEach(resource => {
                                this.Resources[resource.Key] = resource.Value;
                            })
                        })
                    }
                }
            }, (error: HttpResponse<any>) => {
                this.resourcesList = [];
                this.uiLoader.stop();
                if (error["error"] != null) {
                    let errorMessage = error["error"].Error["Message"];
                    this._messageDialogService.openDialogBox('Error', errorMessage, Constants.msgBoxError);
                }
            });
        if (isLoginFlag == true) {
            return this.ResourcesArr;
        }
        else {
            return this.Resources;
        }
    }
}
