import { Injectable, Injector } from '@angular/core';
import { HttpClientService } from 'src/app/core/services/httpClient.service';
import { URLConfiguration } from 'src/app/shared/urlConfiguration/urlconfiguration';
import { HttpEvent, HttpEventType, HttpResponse, HttpClient, HttpHeaders } from '@angular/common/http';
import { Constants } from 'src/app/shared/constants/constants';
import { NgxUiLoaderService } from 'ngx-ui-loader';
import { MessageDialogService } from 'src/app/shared/services/mesage-dialog.service';
import { Product } from './product';
import { AppSettings } from 'src/app/appsettings';


@Injectable({
  providedIn: 'root'
})
export class ProductService {
  //public variables
  public productList:any;
  constructor(private http: HttpClient,
    private injector: Injector,
    private uiLoader: NgxUiLoaderService,
    private _messageDialogService: MessageDialogService,
    private _http: HttpClient) { }

  // public async getProducts(): Promise<Product[]> {
  //   debugger;
  //   let httpClientService = this.injector.get(HttpClientService);
  //   let requestUrl = URLConfiguration.productGetUrl;
  //   this.uiLoader.start();
  //   var response: any = {};
  //   await httpClientService.CallHttp("GET", requestUrl).toPromise()
  //     .then((httpEvent: HttpEvent<any>) => {
  //       debugger;
  //       if (httpEvent.type == HttpEventType.Response) {
  //         if (httpEvent["status"] === 200) {
  //           this.productList = [];
  //           this.uiLoader.stop();
  //           if (httpEvent['body'] != null) {
  //             httpEvent['body'].forEach(scheduleObject => {
  //               this.productList = [...this.productList, scheduleObject];
  //             });
  //           } 
  //         }
  //         else {
  //           this.productList = [];
  //           this.uiLoader.stop();
  //         }
  //       }
  //     }, (error: HttpResponse<any>) => {
  //       debugger;
  //       this.productList = [];
  //       this.uiLoader.stop();
  //     });
  //   return <Product[]>this.productList;
  // }
  public baseURL: string = AppSettings.baseURL;
  // public async getProducts(): Promise<Product[]> {
  //   debugger;
  //   this._http.get(this.baseURL + URLConfiguration.productGetUrl).subscribe(
  //     data => {
  //       debugger;
  //       // let records = <any[]>data;
  //       this.productList=data;
        
  //     },
  //     error => {
        
  //       this._messageDialogService.openDialogBox('Error', error.error.Message, Constants.msgBoxError);
  //     } );
  //     console.log(this.productList);
  //     return this.productList;
  //   }


  // getProducts():any {
  //   debugger;
  //  return this._http.get(this.baseURL + URLConfiguration.productGetUrl);
  //   }
    getProducts():any {
      debugger;
      this._http.get(this.baseURL + URLConfiguration.productGetUrl).subscribe(data => {
      return data;
    }); 
    }
    getpageTypeByProductID(productId:string):any {
      debugger;
      this.uiLoader.start();
     return this._http.post(`${this.baseURL}Product/GetProductPageTypeMappingByProductId?productId=${productId}`,null);   
      }
}
