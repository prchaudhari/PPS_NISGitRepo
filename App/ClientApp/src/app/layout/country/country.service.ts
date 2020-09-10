import { Injectable, Injector } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpClient, HttpEvent, HttpEventType, HttpResponse } from '@angular/common/http';
import { NgxUiLoaderService } from 'ngx-ui-loader';
import { MessageDialogService } from 'src/app/shared/services/mesage-dialog.service';
import { URLConfiguration } from 'src/app/shared/urlConfiguration/urlconfiguration';
import { HttpClientService } from 'src/app/core/services/httpClient.service';
import { Constants } from 'src/app/shared/constants/constants';
import { Country } from './country';

@Injectable({
  providedIn: 'root'
})
export class CountryService {

  public countryList: Country[] = [];
  public isRecordFound: boolean = false;
  public isRecordSaved: boolean = false;
  public isRecordDeleted = {};
  public resultFlag = {};

  constructor(private http: HttpClient,
    private injector: Injector,
    private uiLoader: NgxUiLoaderService,
    private _messageDialogService: MessageDialogService) { }

  //method to call api of get countrys.
  async getCountrys(searchParameter): Promise<any> {
    let httpClientService = this.injector.get(HttpClientService);
    let requestUrl = URLConfiguration.countryGetUrl;
    this.uiLoader.start();
    var response : any = {};
    await httpClientService.CallHttp("POST", requestUrl, searchParameter).toPromise()
      .then((httpEvent: HttpEvent<any>) => {
        if (httpEvent.type == HttpEventType.Response) {
          if (httpEvent["status"] === 200) {
            this.countryList = [];
            this.uiLoader.stop();
            httpEvent['body'].forEach(countryObject => {
              this.countryList = [...this.countryList, countryObject];
            });
            response.List = this.countryList;
            response.RecordCount = parseInt(httpEvent.headers.get('recordCount'));
          }
          else {
            this.countryList = [];
            response.List = this.countryList;
            response.RecordCount = 0;
            this.uiLoader.stop();
          }
        }
      }, (error: HttpResponse<any>) => {
        this.countryList = [];
        response.List = this.countryList;
        response.RecordCount = 0;
        this.uiLoader.stop();
        if (error["error"] != null) {
          let errorMessage = error["error"].Error["Message"];
          this._messageDialogService.openDialogBox('Error', errorMessage, Constants.msgBoxError);
        }
      });
    return response
  }

  //service method to save or update country records
  public async saveCountry(postData, countryEditModeOn): Promise<boolean> {
    let httpClientService = this.injector.get(HttpClientService);
    let requestUrl = URLConfiguration.countryAddUrl;
    if (countryEditModeOn) {
      requestUrl = URLConfiguration.countryUpdateUrl;
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

  //method to call api of delete country.
  public async deleteCountry(postData): Promise<boolean> {
    let httpClientService = this.injector.get(HttpClientService);
    let identifier = null;
    if (postData.length > 0) {
      identifier = postData[0].Identifier;
    }
    let requestUrl = URLConfiguration.countryDeleteUrl;
    this.uiLoader.start();
    await httpClientService.CallHttp("POST", requestUrl, postData).toPromise()
      .then((httpEvent: HttpEvent<any>) => {
        if (httpEvent.type == HttpEventType.Response) {
          this.uiLoader.stop();
          if (httpEvent["status"] === 200) {
            this.isRecordDeleted = true
          }
          else {
            this.isRecordDeleted = false
          }
        }
      }, (error) => {
        this._messageDialogService.openDialogBox('Error', error.error.Message, Constants.msgBoxError);
        this.uiLoader.stop();
        this.isRecordDeleted = false
      });
    return <boolean>this.isRecordDeleted;
  }
}
