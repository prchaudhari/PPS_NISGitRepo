import { Injectable, Injector } from '@angular/core';
import { HttpClientService } from 'src/app/core/services/httpClient.service';
import { URLConfiguration } from 'src/app/shared/urlConfiguration/urlconfiguration';
import { Observable } from 'rxjs';
import { HttpEvent, HttpEventType, HttpResponse, HttpClient, HttpHeaders } from '@angular/common/http';
import { Constants } from 'src/app/shared/constants/constants';
import { User } from './user';
import { NgxUiLoaderService } from 'ngx-ui-loader';
import { MessageDialogService } from 'src/app/shared/services/mesage-dialog.service';
import { ConfigConstants } from 'src/app/shared/constants/configConstants';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  //public variables
  public usersList;
  public profileList;
  public designationList;
  public languageList;
  public isRecordFound;
  public isRecordSaved: boolean = false;
  public isRecordDeleted: boolean = false;
  public countrycodeList = [];
  public ouList = [];
  public isDependencyPresent: boolean = false;

  constructor(
    private injector: Injector,
    private uiLoader: NgxUiLoaderService,
    private http: HttpClient,
    private _messageDialogService: MessageDialogService) { }

  public async getUser(searchParameter): Promise<any> {
    let httpClientService = this.injector.get(HttpClientService);
    let requestUrl = URLConfiguration.userGetUrl;
    this.uiLoader.start();
    var response: any = {};
    await httpClientService.CallHttp("POST", requestUrl, searchParameter).toPromise()
      .then((httpEvent: HttpEvent<any>) => {
        if (httpEvent.type == HttpEventType.Response) {
          if (httpEvent["status"] === 200) {
            this.usersList = [];
            this.uiLoader.stop();
            httpEvent['body'].forEach(roleObject => {
              this.usersList = [...this.usersList, roleObject];
            });
            response.usersList = this.usersList;
            response.RecordCount = parseInt(httpEvent.headers.get('recordCount'));
          }
          else {
            this.usersList = [];
            response.usersList = this.usersList;
            response.RecordCount = 0;
            this.uiLoader.stop();
          }
        }
      }, (error: HttpResponse<any>) => {
          this.usersList = [];
        this.uiLoader.stop();
        if (error["error"] != null) {
          let errorMessage = error["error"].Error["Message"];
          this._messageDialogService.openDialogBox('Error', errorMessage, Constants.msgBoxError);
        }
      });
    return response
  }


  //method to call api of delete User.
  public async deleteUser(postData): Promise<boolean> {
    let httpClientService = this.injector.get(HttpClientService);
    let requestUrl = URLConfiguration.userDeleteUrl ;
    this.uiLoader.start();
    await httpClientService.CallHttp("POST", requestUrl, postData).toPromise()
      .then((httpEvent: HttpEvent<any>) => {
        if (httpEvent.type == HttpEventType.Response) {
          this.uiLoader.stop();
          if (httpEvent["status"] === 200) {
            this.isRecordDeleted = true;
          }
          else {
            this.isRecordDeleted = false;
          }
        }
      }, (error) => {
        this._messageDialogService.openDialogBox('Error', error.error.Message, Constants.msgBoxError);
        this.uiLoader.stop();
        this.isRecordDeleted = false;
      });
    return <boolean>this.isRecordDeleted;
  }

  //This api is save user data--
  public saveUser(postData, userEditModeOn): Observable<any> {
    let requestUrl = URLConfiguration.userAddUrl;
    if (userEditModeOn) {
      requestUrl = URLConfiguration.userUpdateUrl;
    }
    var baseUrl = ConfigConstants.BaseURL;
    var fullURL = baseUrl + requestUrl;
    return this.http.post(fullURL, postData);
  }

  //method to call api of unlock User.
  public async unlockUser(postData): Promise<boolean> {
    let httpClientService = this.injector.get(HttpClientService);
    let requestUrl = URLConfiguration.userUnlockUrl + "?" + "userIdentifier=" + postData;
    this.uiLoader.start();
    await httpClientService.CallGetHttp("GET", requestUrl).toPromise()
      .then((httpEvent: HttpEvent<any>) => {
        if (httpEvent.type == HttpEventType.Response) {
          this.uiLoader.stop();
          if (httpEvent["status"] === 200) {
            this.isRecordDeleted = true;
          }
          else {
            this.isRecordDeleted = false;
          }
        }
      }, (error: HttpResponse<any>) => {
        this.uiLoader.stop();
        this.isRecordDeleted = false;
      });
    return <boolean>this.isRecordDeleted;
  }
  public async userlockUrl(postData): Promise<boolean> {
    let httpClientService = this.injector.get(HttpClientService);
    let requestUrl = URLConfiguration.userlockUrl + "?" + "userIdentifier=" + postData;
    this.uiLoader.start();
    await httpClientService.CallGetHttp("GET", requestUrl).toPromise()
      .then((httpEvent: HttpEvent<any>) => {
        if (httpEvent.type == HttpEventType.Response) {
          this.uiLoader.stop();
          if (httpEvent["status"] === 200) {
            this.isRecordDeleted = true;
          }
          else {
            this.isRecordDeleted = false;
          }
        }
      }, (error: HttpResponse<any>) => {
        this.uiLoader.stop();
        this.isRecordDeleted = false;
      });
    return <boolean>this.isRecordDeleted;
  }

  public async activate(postData): Promise<boolean> {
    let httpClientService = this.injector.get(HttpClientService);
    let requestUrl = URLConfiguration.userActivate + "?" + "userIdentifier=" + postData;
    this.uiLoader.start();
    await httpClientService.CallGetHttp("GET", requestUrl).toPromise()
      .then((httpEvent: HttpEvent<any>) => {
        if (httpEvent.type == HttpEventType.Response) {
          this.uiLoader.stop();
          if (httpEvent["status"] === 200) {
            this.isRecordDeleted = true;
          }
          else {
            this.isRecordDeleted = false;
          }
        }
      }, (error: HttpResponse<any>) => {
        this.uiLoader.stop();
        this.isRecordDeleted = false;
      });
    return <boolean>this.isRecordDeleted;
  }

  public async deactivate(postData): Promise<boolean> {
    let httpClientService = this.injector.get(HttpClientService);
    let requestUrl = URLConfiguration.userDeactivate + "?" + "userIdentifier=" + postData;
    this.uiLoader.start();
    await httpClientService.CallGetHttp("GET", requestUrl).toPromise()
      .then((httpEvent: HttpEvent<any>) => {
        if (httpEvent.type == HttpEventType.Response) {
          this.uiLoader.stop();
          if (httpEvent["status"] === 200) {
            this.isRecordDeleted = true;
          }
          else {
            this.isRecordDeleted = false;
          }
        }
      }, (error: HttpResponse<any>) => {
        this.uiLoader.stop();
        this.isRecordDeleted = false;
      });
    return <boolean>this.isRecordDeleted;
  }

  //This api is called to save profile--
  public async saveProfile(postData): Promise<boolean> {
    let httpClientService = this.injector.get(HttpClientService);
    let requestUrl = URLConfiguration.profileUpdateUrl;
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
      }, (error: HttpResponse<any>) => {
        this.uiLoader.stop();
        this.isRecordSaved = false;
      });
    return <boolean>this.isRecordSaved;
  }

  //method to call api of get smtp.
  public async getProfile(): Promise<User[]> {
    let httpClientService = this.injector.get(HttpClientService);
    let requestUrl = URLConfiguration.profileGetUrl;
    this.uiLoader.start();
    await httpClientService.CallHttp("POST", requestUrl).toPromise()
      .then((httpEvent: HttpEvent<any>) => {
        if (httpEvent.type == HttpEventType.Response) {
          if (httpEvent["status"] === 200) {
            this.profileList = [];
            this.uiLoader.stop();
            if (this.profileList != null) {
              this.profileList = httpEvent['body']
            }
          }
          else {
            this.profileList = [];
            this.isRecordFound = false;
            this.uiLoader.stop();
          }
        }
      }, (error: HttpResponse<any>) => {
        this.profileList = [];
        this.isRecordFound = false;
        this.uiLoader.stop();
      });
    return <User[]>this.profileList;
  }

  //method to call api of dependency check downtime.
  public async checkDependency(identifier): Promise<boolean> {
    let httpClientService = this.injector.get(HttpClientService);
    let requestUrl = URLConfiguration.userCheckDeleteDependencyUrl + '?Identifier=' + identifier;
    this.uiLoader.start();
    await httpClientService.CallHttp("POST", requestUrl).toPromise()
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

  public async sendPassword(data): Promise<boolean> {
    let httpClientService = this.injector.get(HttpClientService);
    let requestUrl = URLConfiguration.userSendPassword;
    this.uiLoader.start();
    let result: boolean = false;
    await httpClientService.CallHttp("POST", requestUrl, data).toPromise()
      .then((httpEvent: HttpEvent<any>) => {
        if (httpEvent.type == HttpEventType.Response) {
          this.uiLoader.stop();
          if (httpEvent["status"] === 200) {
            result = true;
          }else {
            result = false;
          }
        }
      }, (error: HttpResponse<any>) => {
        this.uiLoader.stop();
        result = false;
      });
    return <boolean>result;
  }

}
