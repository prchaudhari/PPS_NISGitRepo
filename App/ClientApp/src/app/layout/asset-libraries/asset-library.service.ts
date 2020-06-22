
import { Injectable, Injector } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpClient, HttpEvent, HttpEventType, HttpResponse } from '@angular/common/http';
import { NgxUiLoaderService } from 'ngx-ui-loader';
import { MessageDialogService } from 'src/app/shared/services/mesage-dialog.service';
import { URLConfiguration } from 'src/app/shared/urlConfiguration/urlconfiguration';
import { HttpClientService } from 'src/app/core/services/httpClient.service';
import { Constants } from 'src/app/shared/constants/constants';
import { AssetLibrary } from 'src/app/layout/asset-libraries/asset-library';

@Injectable({
  providedIn: 'root'
})
export class AssetLibraryService {

  public accessToken;
  public assestLibraryList: AssetLibrary[] = [];
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
  async getAssetLibrary(searchParameter): Promise<AssetLibrary[]> {
    let httpClientService = this.injector.get(HttpClientService);
    let requestUrl = URLConfiguration.assetLibraryGetUrl;
    this.uiLoader.start();
    await httpClientService.CallHttp("POST", requestUrl, searchParameter).toPromise()
      .then((httpEvent: HttpEvent<any>) => {
        if (httpEvent.type == HttpEventType.Response) {
          if (httpEvent["status"] === 200) {
            this.assestLibraryList = [];
            this.uiLoader.stop();
            httpEvent['body'].forEach(roleObject => {
              this.assestLibraryList = [...this.assestLibraryList, roleObject];
            });
          }
          else {
            this.assestLibraryList = [];
            this.uiLoader.stop();
          }
        }
      }, (error: HttpResponse<any>) => {
        this.assestLibraryList = [];
        this.uiLoader.stop();
        if (error["error"] != null) {
          let errorMessage = error["error"].Error["Message"];
          this._messageDialogService.openDialogBox('Error', errorMessage, Constants.msgBoxError);
        }
      });
    return <AssetLibrary[]>this.assestLibraryList
  }

  //service method to save or update asset library records
  public async saveAssetLibrary(postData, roleEditModeOn): Promise<boolean> {
    let httpClientService = this.injector.get(HttpClientService);
    let requestUrl = URLConfiguration.assetLibraryAddUrl;
    if (roleEditModeOn) {
      requestUrl = URLConfiguration.assetLibraryUpdateUrl;
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
 
  //method to call api of delete asset library.
  public async deleteAssetLibrary(postData): Promise<boolean> {
    let httpClientService = this.injector.get(HttpClientService);
    let identifier = null;
    if (postData.length > 0) {
      identifier = postData[0].Identifier;
    }
    let requestUrl = URLConfiguration.assetLibraryDeleteUrl + '?Identifier=' + identifier;
    this.uiLoader.start();
    await httpClientService.CallHttp("POST", requestUrl).toPromise()
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
      }, (error: HttpResponse<any>) => {
        this.uiLoader.stop();
        this.isRecordDeleted = false
      });
    return <boolean>this.isRecordDeleted;
  }

  public async checkDependency(postData): Promise<boolean> {
    let httpClientService = this.injector.get(HttpClientService);
    let identifier = null;
    if (postData.length > 0) {
      identifier = postData[0].Identifier;
    }
    let requestUrl = URLConfiguration.assetLibraryCheckIsDependencyUrl + '?assetLibraryIdentifier=' + identifier;
    this.uiLoader.start();
    await httpClientService.CallGetHttp("GET", requestUrl).toPromise()
      .then((httpEvent: HttpEvent<any>) => {
        if (httpEvent.type == HttpEventType.Response) {
          this.uiLoader.stop();
          if (httpEvent['body'] == true) {
            this.isDependencyPresent = true;
          }
          else {
            this.isDependencyPresent = false;
          }
        }
      }, (error: HttpResponse<any>) => {
        this.uiLoader.stop();
        this.isDependencyPresent = false;
      });
    return <boolean>this.isDependencyPresent;
  }
}
