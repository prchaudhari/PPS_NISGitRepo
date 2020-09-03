
import { Injectable, Injector } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpClient, HttpEvent, HttpEventType, HttpResponse } from '@angular/common/http';
import { NgxUiLoaderService } from 'ngx-ui-loader';
import { MessageDialogService } from 'src/app/shared/services/mesage-dialog.service';
import { URLConfiguration } from 'src/app/shared/urlConfiguration/urlconfiguration';
import { HttpClientService } from 'src/app/core/services/httpClient.service';
import { Constants } from 'src/app/shared/constants/constants';
import { Statement } from './statement';

@Injectable({
  providedIn: 'root'
})
export class StatementService {

  public statementList: Statement[] = [];
  public isRecordFound: boolean = false;
  public isRecordSaved: boolean = false;
  public isRecordDeleted = {};
  public resultFlag = {};

  constructor(private http: HttpClient,
    private injector: Injector,
    private uiLoader: NgxUiLoaderService,
    private _messageDialogService: MessageDialogService) { }

  //method to call api of get statements.
  async getStatements(searchParameter): Promise<any> {
    let httpClientService = this.injector.get(HttpClientService);
    let requestUrl = URLConfiguration.statementGetUrl;
    this.uiLoader.start();
    var response : any = {};
    await httpClientService.CallHttp("POST", requestUrl, searchParameter).toPromise()
      .then((httpEvent: HttpEvent<any>) => {
        if (httpEvent.type == HttpEventType.Response) {
          if (httpEvent["status"] === 200) {
            this.statementList = [];
            this.uiLoader.stop();
            httpEvent['body'].forEach(statementObject => {
              this.statementList = [...this.statementList, statementObject];
            });
            response.List = this.statementList;
            response.RecordCount = parseInt(httpEvent.headers.get('recordCount'));
          }
          else {
            this.statementList = [];
            response.List = this.statementList;
            response.RecordCount = 0;
            this.uiLoader.stop();
          }
        }
      }, (error: HttpResponse<any>) => {
        this.statementList = [];
        response.List = this.statementList;
        response.RecordCount = 0;
        this.uiLoader.stop();
        if (error["error"] != null) {
          let errorMessage = error["error"].Error["Message"];
          this._messageDialogService.openDialogBox('Error', errorMessage, Constants.msgBoxError);
        }
      });
    return response
  }

  //service method to save or update statement records
  public async saveStatement(postData, statementEditModeOn): Promise<boolean> {
    let httpClientService = this.injector.get(HttpClientService);
    let requestUrl = URLConfiguration.statementAddUrl;
    if (statementEditModeOn) {
      requestUrl = URLConfiguration.statementUpdateUrl;
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

  //method to call api of delete statement.
  public async deleteStatement(postData): Promise<boolean> {
    let httpClientService = this.injector.get(HttpClientService);
    let identifier = null;
    if (postData.length > 0) {
      identifier = postData[0].Identifier;
    }
    let requestUrl = URLConfiguration.statementDeleteUrl + '?statementIdentifier=' + identifier;
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
      }, (error) => {
        this._messageDialogService.openDialogBox('Error', error.error.Message, Constants.msgBoxError);
        this.uiLoader.stop();
        this.isRecordDeleted = false
      });
    return <boolean>this.isRecordDeleted;
  }

  //method to call api of publish statement.
  public async publishStatement(postData): Promise<boolean> {
    let httpClientService = this.injector.get(HttpClientService);
    let identifier = null;
    if (postData.length > 0) {
      identifier = postData[0].Identifier;
    }
    let requestUrl = URLConfiguration.statementPublishUrl + '?statementIdentifier=' + identifier;
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
      }, (error) => {
        this._messageDialogService.openDialogBox('Error', error.error.Message, Constants.msgBoxError);
        this.uiLoader.stop();
        this.isRecordDeleted = false
      });
    return <boolean>this.isRecordDeleted;
  }

  //method to call api of clone statement.
  public async cloneStatement(postData): Promise<boolean> {
    let httpClientService = this.injector.get(HttpClientService);
    let identifier = null;
    if (postData.length > 0) {
      identifier = postData[0].Identifier;
    }
    let requestUrl = URLConfiguration.statementCloneUrl + '?statementIdentifier=' + identifier;
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
      }, (error) => {
        this._messageDialogService.openDialogBox('Error', error.error.Message, Constants.msgBoxError);
        this.uiLoader.stop();
        this.isRecordDeleted = false
      });
    return <boolean>this.isRecordDeleted;
  }

  //method to call api of preview page.
  public async previewStatement(postData): Promise<string> {
    let httpClientService = this.injector.get(HttpClientService);
    let identifier = null;
    if (postData.length > 0) {
        identifier = postData[0].Identifier;
    }
    
    let requestUrl = URLConfiguration.statementPreviewUrl + '?StatementIdentifier=' + identifier;
    this.uiLoader.start();
    let resultString:string="";

    await httpClientService.CallHttp("POST", requestUrl).toPromise()
        .then((httpEvent: HttpEvent<any>) => {
            if (httpEvent.type == HttpEventType.Response) {
                this.uiLoader.stop();
                if (httpEvent["status"] === 200) {
                    resultString = httpEvent['body'];
                }
                else {
                    resultString = '';
                }
            }
        }, (error) => {
            this._messageDialogService.openDialogBox('Error', error.error.Message, Constants.msgBoxError);
            this.uiLoader.stop();
            resultString='';
        });
    return <string>resultString;
}
}
