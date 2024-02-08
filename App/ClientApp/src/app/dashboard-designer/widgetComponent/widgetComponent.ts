import { Component, ViewChild, Output, Input, OnInit, EventEmitter, SecurityContext, Directive } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { ConfigConstants } from '../../shared/constants/configConstants';
import * as Highcharts from 'highcharts';
import * as $ from 'jquery';
import { map } from 'rxjs/operators';
import { BrowserModule, DomSanitizer } from '@angular/platform-browser';
import { Pipe, PipeTransform } from '@angular/core';
import { AppSettings } from '../../appsettings';

// import {AddDashboardDesignerComponent} from '../add-dashboard-designer/add-dashboard-designer.component';

declare var require: any;
let Boost = require('highcharts/modules/boost');
let noData = require('highcharts/modules/no-data-to-display');
let More = require('highcharts/highcharts-more');
Boost(Highcharts);
noData(Highcharts);
More(Highcharts);
noData(Highcharts);

// Component Created for Customer Information Widget--
@Component({
  selector: 'customerInformation',
  template: `<div class="widget">
    <div class="widget-header">
      <span class="widget-header-title"> Customer Information </span>
    </div>
    <div class="widget-area">
      <div class="row">
        <div class="col-sm-4">
          <h4 class="mb-4">Laura J Donald</h4>
          <h6>4000 Executive Parkway, Saint Globin Rd #250,<br/> Canary Wharf, E94583</h6>
        </div>
      
      </div>
    </div>
  </div>`
})
export class CustomerInformationComponent {
  @Input()
  widgetsGridsterItemArray: any[] = [];

}

// Component Created for Payment Summary--
@Component({
  selector: 'paymentSummary',
  template: `<div class="widget">
   
<div class="container_area">
            <div class="fsp-earnings-section">
                <div class="dark-blue-bg payment-sum text-white text-center">Payment summary</div>
                  <div class="fsp-payment-sum d-flex gap-1 w-100 py-4">
                        <div class="col-2"></div>
                        <div class="col-4">
                            <div class="font-weight-bold mb-1">Intermediary Total – March 2023</div>
                            <div class="font-weight-bold mb-1">Vat</div>
                            <div class="pt-1 mb-1"></div>
                            <div class="font-weight-bold">Total Due</div>
                        </div>
                        <div class="col-3">
                            <div class="font-weight-bold mb-1 text-right pe-2">R256 670.66</div>
                            <div class="font-weight-bold mb-1 text-right pe-2">R38 001.27</div>
                            <div class="pt-1 dark-blue-bg mb-1"></div>
                            <div class="font-weight-bold text-right pe-2">R291 671.93</div>
                            <div class="text-right pe-2 fst-italic">Posted – 3rd March</div>
                        </div>
                        <div class="col-3"></div>
                  </div>
            </div>
        </div>
  </div>`
})
export class PaymentSummaryComponent {
  @Input()
  widgetsGridsterItemArray: any[] = [];

}

// Component Created for FSP Heading--
@Component({
  selector: 'fspHeading',
  template: `<div class="widget">
    
        <div class="container_area" >
            <div class="header-section px-20">
                <div class="fsp-header-logo">
                    <div class="fsp-logo"><img src="assets/images/logo3.jpg" alt=""></div>
                    <div class="fsp-logo-text">
                        <h1 class="intermediary_title">Financial Service Provider (FSP) Statement</h1>
                        <h6 class="font-weight-bold fsp_inter_subheading">Miss Yvonne van Heerden T/A Yvonne Van Heerden Financial Planner CC</h6>
                    </div>
                </div>
            </div>
        </div>
  </div>`
})
export class FSPHeadingComponent {
  @Input()
  widgetsGridsterItemArray: any[] = [];

}

// Component Created for PPS Details--
@Component({
  selector: 'fspDetails',
  template: `<div class="widget">
   
        <div class="container_area">
            <div class="header-contact px-20">
                <div class="col-49">
                    <ul>
                        <li>
                            <dd>FSP:</dd>
                            <dt class="text-gray">124529534</dt>
                        </li>
                        <li>
                            <dd>FSP Agreement #:</dd>
                            <dt class="text-gray">2452953</dt>
                        </li>
                        <li>
                            <dd>Vat Reg #:</dd>
                            <dt class="text-gray">2452953</dt>
                        </li>
                        <li>
                            <dd>Month:</dd>
                            <dt class="text-gray">September 2023</dt>
                        </li>
                    </ul>
                </div>
                <div class="col-49">
                    <ul>
                        <li>
                            <dd>Contact details:</dd>
                            <dt></dt>
                        </li>
                        <li>
                            <dd>Mobile</dd>
                            <dt class="text-gray">hard coded</dt>
                        </li>
                        <li>
                            <dd>Email</dd>
                            <dt class="text-gray1">hard coded</dt>
                        </li>
                        <li>
                            <dd>Address</dd>
                            <dt class="text-gray">hard coded</dt>
                        </li>
                    </ul>
                </div>
            </div>
        </div>
  </div>`
})
export class FSPDetailsComponent {
  @Input()
  widgetsGridsterItemArray: any[] = [];

}

// Component Created for PPS Details--
@Component({
  selector: 'ppsFooter',
  template: `<div class="widget">
    
<div class="widget-area">
       <div class="container_area">
            <div class="fsp-footer-section dark-blue-bg py-1 px-2">
                    <ul class="fsp-social-icons">
                        <li><a href="#"><img src="assets/images/fb_foot.png"></a></li>
                        <li><a href="#"><img src="assets/images/insta_foot.png"></a></li>
                        <li><a href="#"><img src="assets/images/twitter_foot.png"></a></li>
                        <li><a href="#"><img src="assets/images/in_foot.png"></a></li>
                        <li><a href="#"><img src="assets/images/you_foot.png"></a></li>
                        <li><a href="#"><img src="assets/images/ticktok_foot.png"></a></li>
                    </ul>
                    <div class="fsp-copyright mb-0">hard coded</div>
                    <div></div>
                    <div></div>
                     <div></div>
                    <div class="fsp-page mb-0">Page 1/2</div>
                </div>
            </div>
        </div>
  </div>`
})
export class PPSFooter1Component {
  @Input()
  widgetsGridsterItemArray: any[] = [];

}

// Component Created for Summary at glance Widget--
@Component({
  selector: 'productSummary',
  template: `<div class="widget">
   
          <div class="container_area" >
            <div class="fsp-earnings-section" >
                <div class="dark-blue-bg payment-sum text-white text-center">Product summary</div>
                <div class="fsp-product-summery">
                    <div class="text-center py-3">
                        <a href="#"><img src="assets/images/IfQueryBtn.jpg"></a>
                    </div>
                    <div class="px-50">
                        <div class="prouct-table-block">
              <table width="100%" cellpadding="0" cellspacing="0">
                  <thead>
                                <tr>
                                    <th class="font-weight-bold text-white">No.</th>
                                    <th class="font-weight-bold text-white text-right pe-0 bdr-r-0">Product</th>
                                    <th class="font-weight-bold text-white text-left">Summary</th>
                                    <th class="font-weight-bold text-white text-center">Amount Payable</th>
                                   
                                </tr>
                       </thead>
                            <tbody>
                              <tr *ngFor="let item of actionPSList;let index=index">
                                    <td align="center" valign="center" class="px-1 py-1 fsp-bdr-right fsp-bdr-bottom"> {{index+1}}</td>
                                    <td class="fsp-bdr-right fsp-bdr-bottom px-1"> {{item.Commission_Type}}  </td>
                                    <td class="fsp-bdr-right fsp-bdr-bottom px-1">
                                {{ item.Prod_Group.trim() == 'Service Fee' ? 'Premium Under Advise Fee' : item.Prod_Group }}
                                    </td>
                                <td class="text-right fsp-bdr-right fsp-bdr-bottom px-1">{{formatCurrency(item.Display_Amount)}}
                                </td></tr>
                                <tr>
                                    <td align="center" valign="center" class="px-1 py-1 fsp-bdr-right fsp-bdr-bottom"></td>
                                    <td class="fsp-bdr-bottom ps-1 text-right">Total</td>
                                    <td class="fsp-bdr-right fsp-bdr-bottom ps-1 text-left">Due</td>
                                    <td class="text-right fsp-bdr-right fsp-bdr-bottom px-1">R256 670.66</td>
                                </tr>
                                <tr>
                                    <td align="center" valign="center" class="px-1 py-1 fsp-bdr-right fsp-bdr-bottom"></td>
                                    <td class="fsp-bdr-bottom ps-1 text-right">VAT</td>
                                    <td class="fsp-bdr-right fsp-bdr-bottom ps-1 text-left">Due</td>
                                    <td class="text-right fsp-bdr-right fsp-bdr-bottom px-1">R38 001.27</td>
                                </tr>
                                <tr>
                                    <td align="center" valign="center" class="px-1 py-1 fsp-bdr-right fsp-bdr-bottom"></td>
                                    <td class="fsp-bdr-bottom ps-1 text-right">Grand</td>
                                    <td class="fsp-bdr-right fsp-bdr-bottom ps-1 text-left">Total Due</td>
                                    <td class="text-right fsp-bdr-right fsp-bdr-bottom px-1">R294 671.93</td>
                                </tr>
                                <tr>
                                    <td align="center" valign="center" class="px-1 py-1 fsp-bdr-right fsp-bdr-bottom"></td>
                                    <td class="fsp-bdr-bottom ps-1 text-right">PPS</td>
                                    <td class="fsp-bdr-right fsp-bdr-bottom ps-1 text-left">Payment</td>
                                    <td class="text-right fsp-bdr-right fsp-bdr-bottom px-1">-R294 671.93</td>
                                </tr>
                                <tr>
                                    <td align="center" valign="center" class="px-1 py-1 fsp-bdr-right fsp-bdr-bottom"></td>
                                    <td class="fsp-bdr-bottom ps-1 text-right font-weight-bold">Balance</td>
                                    <td class="fsp-bdr-right fsp-bdr-bottom ps-1 text-left"></td>
                                    <td class="text-right fsp-bdr-right fsp-bdr-bottom px-1 font-weight-bold">R0.00</td>
                                </tr>
                      </tbody>
                           </table>
                          </div>
                    </div>
                </div>
            </div>
  </div>`
})

export class ProductSummaryComponent {
  @Input()
  widgetsGridsterItemArray: any[] = [];

  public actionPSList: any[] = [
    { Commission_Type: 'Safe Custody Fee', Prod_Group: ' Safe Custody Fee', Display_Amount: '52.65', Query_Link: "https://facebook.com" },
    { Commission_Type: 'Safe Custody Fee', Prod_Group: ' Service Fee', Display_Amount: '52.66', Query_Link: "https://facebook.com" },
    { Commission_Type: 'Safe Custody Fee', Prod_Group: ' Safe Custody Fee', Display_Amount: '52.67', Query_Link: "https://facebook.com" },
    { Commission_Type: 'Safe Custody Fee', Prod_Group: ' Service Fee', Display_Amount: '52.68', Query_Link: "https://facebook.com" }
  ]
  public formatCurrency(input: any): any {
    const amount: number = parseFloat(input); // Parse the string to a decimal
    const formattedAmount: string = amount.toLocaleString('en-ZA', {
      style: 'currency',
      currency: 'ZAR'
    });

    return ((amount < 0 ? '-' : '') + formattedAmount.replace(',', '.')).replace('--', '-');
  }

}

// Component Created for Summary at glance Widget--
@Component({
  selector: 'detailedTransactions',
  template: `<div class="widget">
  
<div class="container_area">
        <div class="fsp-earnings-section">
            <div class="dark-blue-bg payment-sum text-white text-center">Detailed transactions</div>
            <div class="fsp-product-summery">
                <div class="text-center py-3">
                    <a href="#"><img src="assets/images/IfQueryBtn.jpg"></a>
                </div>
                <div class="px-50">
                    <!--table 1 start here-->
                    <div class="prouct-table-block">
                        <div class="fsp-transaction-title font-weight-bold mb-3">Intermediary: 124411745 Kruger Van Heerden</div>
                        <table width="100%" cellpadding="0" cellspacing="0">
                            <thead>
                                <tr>
                                    <th class="font-weight-bold text-white text-left text-nowrap">Client name</th>
                                    <th class="font-weight-bold text-white pe-0 bdr-r-0 text-left text-nowrap">Member<br/>number</th>
                                    <th class="font-weight-bold text-white text-left text-nowrap">Policy number</th>
                                    <th class="font-weight-bold text-white text-left">Description</th>
                                    <th class="font-weight-bold text-white text-left">Commission<br/>type</th>
                                    <th class="font-weight-bold text-white text-left text-nowrap">Posted date</th>
                                    <th class="font-weight-bold text-white text-left text-nowrap">Posted amount</th>
                                    <th class="font-weight-bold text-white">Query</th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr *ngFor="let item of detailedTransactionList">
                                    <td class="text-left px-1 py-1 fsp-bdr-right fsp-bdr-bottom">{{item.Client_Name}}</td>
                                    <td class="text-right fsp-bdr-right fsp-bdr-bottom px-1">{{item.Member_Ref}}</td>
                                    <td class="text-right fsp-bdr-right fsp-bdr-bottom px-1">{{item.Policy_Ref}}</td>
                                    <td class="fsp-bdr-right fsp-bdr-bottom px-1">{{item.Description}}</td>
                                    <td class="text-left fsp-bdr-right fsp-bdr-bottom px-1">{{item.Commission_Type}}</td>
                                    <td class="text-left fsp-bdr-right fsp-bdr-bottom px-1">{{item.POSTED_DATE}}</td>
                                    <td class="text-right fsp-bdr-right fsp-bdr-bottom px-1">{{formatCurrency(item.Display_Amount)}}</td>
                                    <td class="text-left fsp-bdr-bottom px-1">
                                        <a href="{{item.Query_Link}}" target="_blank">
                                            <img class="leftarrowlogo" src="assets/images/leftarrowlogo.png" alt="Left Arrow">
                                        </a>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="center" valign="center" class="px-1 py-1 fsp-bdr-right fsp-bdr-bottom"></td>
                                    <td class="fsp-bdr-right fsp-bdr-bottom px-1 py-1"></td>
                                    <td class="fsp-bdr-right fsp-bdr-bottom px-1 py-1"></td>
                                    <td class="text-right fsp-bdr-right fsp-bdr-bottom px-1 py-1"></td>
                                    <td class="fsp-bdr-right fsp-bdr-bottom px-1 py-1"><br/></td>
                                    <td class="fsp-bdr-right fsp-bdr-bottom px-1 py-1"></td>
                                    <td class="text-right fsp-bdr-right fsp-bdr-bottom px-1 py-1">R20.18</td>
                                    <td class="text-left fsp-bdr-bottom px-1"><a href="#"><img src="assets/images/leftarrowlogo.png"></a></td>
                                </tr>
                            </tbody>
                        </table>
                    </div>

                    <!--table 2 start here-->

                    <div class="prouct-table-block">
                        <div class="fsp-transaction-title font-weight-bold mb-3">Intermediary: 2164250 Yvonne Van Heerden</div>
                        <table width="100%" cellpadding="0" cellspacing="0">
                            <thead>
                                <tr>
                                    <th class="font-weight-bold text-white text-left text-nowrap">Client name</th>
                                    <th class="font-weight-bold text-white pe-0 bdr-r-0 text-left text-nowrap">Member<br/>number</th>
                                    <th class="font-weight-bold text-white text-left text-nowrap">Policy number</th>
                                    <th class="font-weight-bold text-white text-left">Description</th>
                                    <th class="font-weight-bold text-white text-left">Commission<br/>type</th>
                                    <th class="font-weight-bold text-white text-left text-nowrap">Posted date</th>
                                    <th class="font-weight-bold text-white text-left text-nowrap">Posted amount</th>
                                    <th class="font-weight-bold text-white">Query</th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr *ngFor="let item of detailedTransactionIntList">
                                    <td class="text-left px-1 py-1 fsp-bdr-right fsp-bdr-bottom">{{item.Client_Name}}</td>
                                    <td class="text-right fsp-bdr-right fsp-bdr-bottom px-1">{{item.Member_Ref}}</td>
                                    <td class="text-right fsp-bdr-right fsp-bdr-bottom px-1">{{item.Policy_Ref}}</td>
                                    <td class="fsp-bdr-right fsp-bdr-bottom px-1">{{item.Description}}</td>
                                    <td class="text-left fsp-bdr-right fsp-bdr-bottom px-1">{{item.Commission_Type}}</td>
                                    <td class="text-left fsp-bdr-right fsp-bdr-bottom px-1">{{item.POSTED_DATE}}</td>
                                    <td class="text-right fsp-bdr-right fsp-bdr-bottom px-1">{{formatCurrency(item.Display_Amount)}}</td>
                                    <td class="text-left fsp-bdr-bottom px-1">
                                        <a href="{{item.Query_Link}}" target="_blank">
                                            <img class="leftarrowlogo" src="assets/images/leftarrowlogo.png" alt="Left Arrow">
                                        </a>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="center" valign="center" class="px-1 py-1 fsp-bdr-right fsp-bdr-bottom"></td>
                                    <td class="fsp-bdr-right fsp-bdr-bottom px-1 py-1"></td>
                                    <td class="fsp-bdr-right fsp-bdr-bottom px-1 py-1"></td>
                                    <td class="text-right fsp-bdr-right fsp-bdr-bottom px-1 py-1"></td>
                                    <td class="fsp-bdr-right fsp-bdr-bottom px-1 py-1"><br/></td>
                                    <td class="fsp-bdr-right fsp-bdr-bottom px-1 py-1"></td>
                                    <td class="text-right fsp-bdr-right fsp-bdr-bottom px-1 py-1">R20.18</td>
                                    <td class="text-left fsp-bdr-bottom px-1"><a href="#"><img src="assets/images/leftarrowlogo.png"></a></td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>
        </div>
        <div class="container_area">
            <div class="click-btn text-center w-100 pb-3" style="padding-top: 150px;">
                <a href="#"><img src="assets/images/click-more-information.png"></a>
            </div>
    </div>
    </div>`
})

export class DetailedTransactionsComponent {
  @Input()
  widgetsGridsterItemArray: any[] = [];

  public detailedTransactionList: any[] = [
    { INT_EXT_REF: '2164250', Int_Name: 'Mr SCHOELER', Client_Name: 'Mr SCHOELER', Member_Ref: '124556686', Policy_Ref: '5596100', Description: 'Safe Custody Service Fee', Commission_Type: 'Safe Custody Fee', POSTED_DATE: '20-Mar-23', Display_Amount: '17.55', Query_Link: "https://facebook.com", TYPE: 'Fiduciary_Data', Prod_Group: 'Safe Custody Fee' },
    { INT_EXT_REF: '2164250', Int_Name: 'Yvonne Van Heerden', Client_Name: 'Mr SCHOELER', Member_Ref: '124556686', Policy_Ref: '5596100', Description: 'Safe Custody Service Fee VAT', Commission_Type: 'Safe Custody Fee', POSTED_DATE: '20-Mar-23', Display_Amount: '2.63', Query_Link: "https://facebook.com", TYPE: 'Fiduciary_Data', Prod_Group: 'Safe Custody Fee' },
  ]
  public detailedTransactionIntList: any[] = [
    { INT_EXT_REF: '124411745', Int_Name: 'Kruger Van Heerden', Client_Name: 'DR N J Olivier', Member_Ref: '1217181', Policy_Ref: '5524069', Description: 'Safe Custody Service Fee', Commission_Type: 'Safe Custody Fee', POSTED_DATE: '20-Mar-23', Display_Amount: '17.55', Query_Link: "https://facebook.com", TYPE: 'Fiduciary_Data', Prod_Group: 'Safe Custody Fee' },
    { INT_EXT_REF: '124411745', Int_Name: 'Kruger Van Heerden', Client_Name: 'DR N J Olivier', Member_Ref: '124556686', Policy_Ref: '5596100', Description: 'Safe Custody Service Fee VAT	', Commission_Type: 'Safe Custody Fee', POSTED_DATE: '20-Mar-23', Display_Amount: '2.63', Query_Link: "https://facebook.com", TYPE: 'Fiduciary_Data', Prod_Group: 'VAT' },
  ]
  public formatCurrency(input: any): any {
    const amount: number = parseFloat(input); // Parse the string to a decimal
    const formattedAmount: string = amount.toLocaleString('en-ZA', {
      style: 'currency',
      currency: 'ZAR'
    });

    return ((amount < 0 ? '-' : '') + formattedAmount.replace(',', '.')).replace('--', '-');
  }

}

// Component Created for PPS Details--
@Component({
  selector: 'footerImage',
  template: `<div class="widget">
<div class="widget-area">
       <div class="container_area">
            <div class="fsp-bottom-image-car"></div>
            <div class="px-2 py-2">
                <div class="d-flex gap-1 image">
                    <div class="colom-4">
                        <img src="assets/images/img_1.png">
                    </div>
                    <div class="colom-4">
                        <img style="height:100%"  src="assets/images/img_2.png">
                    </div>
                    <div  class="colom-4">
                        <img style="height:100%"  src="assets/images/img_3.png">
                    </div>

                </div>
            </div>
        </div>
        </div>
  </div>`
})
export class FooterImageComponent {
  @Input()
  widgetsGridsterItemArray: any[] = [];

}

// Component Created for Account Information Widget--
@Component({
  selector: 'accountInformation',
  template: `<div class="widget">
    <div class="widget-header">
      <span class="widget-header-title"> Account Information </span>
    </div>
    <div class="widget-area">
      <div class="list-row-small" *ngFor="let list of accountInfoLists">
        <div class="list-middle-row">
          <div class="list-text">
            {{list.title}}
          </div>
          <label class="list-value mb-0">
            {{list.value}}
          </label>
        </div>
      </div>
    </div>
  </div>`
})
export class AccountInformationComponent {
  @Input()
  widgetsGridsterItemArray: any[] = [];
  //account info
  accountInfoLists: any[] = [
    { title: 'Statement Date', value: '1-APR-2020' },
    { title: 'Statement Period', value: 'Annual Statement' },
    { title: 'Customer ID', value: 'ID2-8989-5656' },
    { title: 'RM Name', value: 'David Miller' },
    { title: 'RM Contact Number', value: '+4487867833' },
  ];

}


// Component Created for PPSDetailS1 Firstpart.
@Component({
  selector: 'PPSDetails1',
  template: `<div class="widget">
   
        <div class="container_area">
            <div class="header-contact px-20">
                <div class="col-49">
                <ul>
                        <li>
                            <dd>Reference:</dd>
                            <dt class="text-gray">124529534</dt>
                        </li>
                        <li>
                            <dd>Measure type:</dd>
                            <dt class="text-gray">Commission</dt>
                        </li>
                        <li>
                            <dd>Month:</dd>
                            <dt class="text-gray">September 2023</dt>
                        </li>
                        <li>
                            <dd>Date:</dd>
                            <dt class="text-gray">2023-09-01 to 2023-01-01</dt>
                        </li>
                    </ul>
                </div>
                <div class="col-49">
                <ul>
                        <li>
                            <dd>Contact details:</dd>
                            <dt></dt>
                        </li>
                        <li>
                            <dd>M:</dd>
                            <dt class="text-gray">hard coded</dt>
                        </li>
                        <li>
                            <dd>E:</dd>
                            <dt class="text-gray1">hard coded</dt>
                        </li>
                        <li>
                            <dd>A:</dd>
                            <dt class="text-gray">hard coded</dt>
                        </li>
                    </ul>
                </div>
            </div>
        </div>
  </div>`
})
export class PPSDetails1Component {
  @Input()
  widgetsGridsterItemArray: any[] = [];

}
// Component Created for PPSDetailS2 secondpart
@Component({
  selector: 'PPSDetails2',
  template: `<div class="widget">

 <div class="container_area text-left">
            <div class="fsp-earnings-section pt-2">
                <div class="dark-blue-bg payment-sum text-white text-center">PPS production statement</div>
                <div class="px-20 pps-production-stat">
                    <div class="row py-2">
                        <div class="col-4 py-2 bdr-end-5">
                            <h4 class="font-weight-bold">Miss HW HLONGWANE</h4>
                            <div class="client-content">
                                <p class="font-weight-bold">Client contact details</p>
                                <p><span class="font-weight-bold">M:</span> hard coded</p>
                                <p><span class="font-weight-bold">E:</span> <a href="">hard coded</a></p>
                            </div>
                        </div>

                        <div class="col-3 py-2 bdr-end-5">
                            <h4 class="font-weight-bold">hard coded</h4>
                            // <div class="client-content">
                            //    <p>1 John Vorster Drive<br/>Randburg<br/>Gauteng</p>
                            //</div>
                        </div>

                        <div class="col-5 py-2">
                        <div class="client-content">
                 <table>
                    <tr>
                        <td class="font-weight-bold">Intermediary Reference:</td>
                        <td>124529534</td>
                    </tr>
                    <tr>
                        <td class="font-weight-bold">Measure Type:</td>
                        <td> Commission</td>
                    </tr>
                    <tr>
                        <td class="font-weight-bold">Month:</td>
			                  <td>September 2023</td>
                    </tr>
                    <tr>
                        <td colspan="2">
                       <b> Date From:</b>
                        2023-09-01 to 2023-01-01
                        </td>
                    </tr>
         
            </table>
            </div>
                        </div>
                    </div>
                </div>
                <div class="dark-blue-bg py-1"></div>
            </div>
        </div>
       
  </div>`
})
export class PPSDetails2Component {
  @Input()
  widgetsGridsterItemArray: any[] = [];

}

// Component Created for PPSDetailedTraction Widget--
@Component({
  selector: 'PPSDetailedTransactions',
  template: `<div class="widget">
   <div class="container_area">
            <div class="fsp-earnings-section">
                <div class="widget-indicator-inner  pt-2 dark-blue-bg payment-sum text-white text-center">Detailed transactions</div>
                
                <div class="pps-earning-period pps-details-table pt-1 pb-2 d-flex justify-content-between">                  
                    <div class="pps-monthly-table w-100">
                            <table cellpadding="0" cellspacing="0" width="100%">
                               <thead>
                                    <tr>
                                    <th class="bdr-right-white sky-blue-bg text-white font-weight-bold">Client<br/>name</th>
                                    <th class="bdr-right-white sky-blue-bg text-white font-weight-bold">Age</th>
                                    <th class="bdr-right-white sky-blue-bg text-white font-weight-bold">Member Number</th>
                                    <th class="bdr-right-white sky-blue-bg text-white font-weight-bold">Policy Number</th>
                                    <th class="bdr-right-white sky-blue-bg text-white font-weight-bold">Product</th>
                                    <th class="bdr-right-white sky-blue-bg text-white font-weight-bold">Date<br/>issued</th>
                                    <th class="bdr-right-white sky-blue-bg text-white font-weight-bold">Inception<br/>date</th>
                                    <th class="bdr-right-white sky-blue-bg text-white font-weight-bold">Com<br/>type</th>
                                    <th class="bdr-right-white sky-blue-bg text-white font-weight-bold">Quantity</th>
                                    <th class="bdr-right-white sky-blue-bg text-white font-weight-bold">Posted<br/>date</th>
                                    <th class="bdr-right-white sky-blue-bg text-white font-weight-bold">Earnings</th>
                                </tr>
                            </thead>
                                <tr>
                                  <td colspan = '11' class='text-left font-weight-bold'>PPS INSURANCE</td>
                                </tr>
                                <tr>
                                    <td class="bdr-right-white">Dr L Guvha</td>
                                    <td class="bdr-right-white">54</td>
                                    <td class="bdr-right-white">1012890</td>
                                    <td class="bdr-right-white">1845387</td>
                                    <td class="bdr-right-white">Professional Health<br/>Provider Whole Life<br/>Professional Health</td>
                                    <td class="bdr-right-white text-nowrap">22-Aug-2022</td>
                                    <td class="bdr-right-white text-nowrap">01-Oct-2022</td>
                                    <td class="bdr-right-white">2nd Year</td>
                                    <td class="bdr-right-white text-right">R3 964.19</td>
                                    <td class="bdr-right-white text-nowrap">06-Sept-2023</td>
                                    <td class="bdr-right-white text-right ewidth">R10 822.24</td>
                                </tr>

                                <tr>
                                    <td class="bdr-right-white" height="40"></td>
                                    <td class="bdr-right-white" height="40"></td>
                                    <td class="bdr-right-white" height="40"></td>
                                    <td class="bdr-right-white" height="40"></td>
                                    <td class="bdr-right-white" height="40"></td>
                                    <td class="bdr-right-white" height="40"></td>
                                    <td class="bdr-right-white" height="40"></td>
                                    <td class="bdr-right-white" height="40"></td>
                                    <td class="bdr-right-white" height="40"></td>
                                    <td class="bdr-right-white" height="40"></td>
                                    <td class="bdr-right-white" height="40"></td>
                                </tr>

                                <tr>
                                    <td class="bdr-right-white" height="40"></td>
                                    <td class="bdr-right-white" height="40"></td>
                                    <td class="bdr-right-white" height="40"></td>
                                    <td class="bdr-right-white" height="40"></td>
                                    <td class="bdr-right-white" height="40"></td>
                                    <td class="bdr-right-white" height="40"></td>
                                    <td class="bdr-right-white" height="40"></td>
                                    <td class="bdr-right-white" height="40"></td>
                                    <td class="bdr-right-white" height="40"></td>
                                    <td class="bdr-right-white" height="40"></td>
                                    <td class="bdr-right-white" height="40"></td>
                                </tr>

                                <tr>
                                    <td class="bdr-right-white" height="40"></td>
                                    <td class="bdr-right-white" height="40"></td>
                                    <td class="bdr-right-white" height="40"></td>
                                    <td class="bdr-right-white" height="40"></td>
                                    <td class="bdr-right-white" height="40"></td>
                                    <td class="bdr-right-white" height="40"></td>
                                    <td class="bdr-right-white" height="40"></td>
                                    <td class="bdr-right-white" height="40"></td>
                                    <td class="bdr-right-white" height="40"></td>
                                    <td class="bdr-right-white" height="40"></td>
                                    <td class="bdr-right-white" height="40"></td>
                                </tr>


                                <tr>
                                    <td class="bdr-right-white" height="40"></td>
                                    <td class="bdr-right-white" height="40"></td>
                                    <td class="bdr-right-white" height="40"></td>
                                    <td class="bdr-right-white" height="40"></td>
                                    <td class="bdr-right-white" height="40"></td>
                                    <td class="bdr-right-white" height="40"></td>
                                    <td class="bdr-right-white" height="40"></td>
                                    <td class="bdr-right-white" height="40"></td>
                                    <td class="bdr-right-white" height="40"></td>
                                    <td class="bdr-right-white" height="40"></td>
                                    <td class="bdr-right-white" height="40"></td>
                                </tr>

                                <tr>
                                    <td class="bdr-right-white" height="40"></td>
                                    <td class="bdr-right-white" height="40"></td>
                                    <td class="bdr-right-white" height="40"></td>
                                    <td class="bdr-right-white" height="40"></td>
                                    <td class="bdr-right-white" height="40"></td>
                                    <td class="bdr-right-white" height="40"></td>
                                    <td class="bdr-right-white" height="40"></td>
                                    <td class="bdr-right-white" height="40"></td>
                                    <td class="bdr-right-white" height="40"></td>
                                    <td class="bdr-right-white" height="40"></td>
                                    <td class="bdr-right-white" height="40"></td>
                                </tr>

                                <tr>
                                    <td class="bdr-right-white" height="40"></td>
                                    <td class="bdr-right-white" height="40"></td>
                                    <td class="bdr-right-white" height="40"></td>
                                    <td class="bdr-right-white" height="40"></td>
                                    <td class="bdr-right-white" height="40"></td>
                                    <td class="bdr-right-white" height="40"></td>
                                    <td class="bdr-right-white" height="40"></td>
                                    <td class="bdr-right-white" height="40"></td>
                                    <td class="bdr-right-white" height="40"></td>
                                    <td class="bdr-right-white" height="40"></td>
                                    <td class="bdr-right-white" height="40"></td>
                                </tr>

                                <tr>
                                    <td class="bdr-right-white" height="40"></td>
                                    <td class="bdr-right-white" height="40"></td>
                                    <td class="bdr-right-white" height="40"></td>
                                    <td class="bdr-right-white" height="40"></td>
                                    <td class="bdr-right-white" height="40"></td>
                                    <td class="bdr-right-white" height="40"></td>
                                    <td class="bdr-right-white" height="40"></td>
                                    <td class="bdr-right-white" height="40"></td>
                                    <td class="bdr-right-white" height="40"></td>
                                    <td class="bdr-right-white" height="40"></td>
                                    <td class="bdr-right-white" height="40"></td>
                                </tr>

                                <tr>
                                    <td class="bdr-right-white" height="40"></td>
                                    <td class="bdr-right-white" height="40"></td>
                                    <td class="bdr-right-white" height="40"></td>
                                    <td class="bdr-right-white" height="40"></td>
                                    <td class="bdr-right-white" height="40"></td>
                                    <td class="bdr-right-white" height="40"></td>
                                    <td class="bdr-right-white" height="40"></td>
                                    <td class="bdr-right-white" height="40"></td>
                                    <td class="bdr-right-white" height="40"></td>
                                    <td class="bdr-right-white" height="40"></td>
                                    <td class="bdr-right-white" height="40"></td>
                                </tr>

                                    <tr>
                                       <td class="dark-blue-bg text-white font-weight-bold "></td>
                                       <td class="dark-blue-bg text-white font-weight-bold "></td>
                                       <td class="dark-blue-bg text-white font-weight-bold "></td>
                                       <td class="dark-blue-bg text-white font-weight-bold "></td>
                                       <td class="dark-blue-bg text-white font-weight-bold "></td>
                                       <td class="dark-blue-bg text-white font-weight-bold "></td>
                                       <td class="dark-blue-bg text-white font-weight-bold "></td>
                                       <td class='dark-blue-bg text-white font-weight-bold text-right' colspan='2'>Sub Total</td>
                                       <td colspan="2" class="font-weight-bold text-right fs-16 pps-bg-gray" height="40">R29 250.62</td></tr>
                            </table>

                       <div class="text-center py-3"><a href="#"><img src="assets/images/IfQueryBtn.jpg"></a></div>
                    </div>
                </div>
              </div>
         </div>
  </div>`
})

export class PPSDetailedTransactionsComponent {
  @Input()
  widgetsGridsterItemArray: any[] = [];


}

/**
 * Component representing the Summary at Glance Widget for Earnings for Period.
 */
@Component({
  selector: 'PPSEarningForPeriod',
  template: `
    <!-- Widget Container -->
    <div class="widget">
      <!-- Middle Section -->
      <div class="middle-section">
        <div class="earnings-section">
          <!-- Earnings for Period Heading -->
          <h2 class="pps-earning-for-period-heading dark-blue-bg text-white">Earnings for Period</h2>
          <!-- FSP account postings summary Section -->
          <div class="earnings-section-monthly d-flex mb-2">
            <div class="d-flex gap-1 w-100">
              <!-- FSP account postings summary -->
              <div class="col-6">
                <!-- Heading for FSP account postings summary -->
                <h4  class="monthly-production-summary skyblue-bg-title text-white text-center">FSP account postings summary</h4>
                <div class="monthly-table">
                  <!-- Table for FSP account postings summary -->
                  <table width="100%" cellpadding="0" cellspacing="0">
                    <!-- Table Headers -->
                    <thead>
                      <tr>
                        <th class="text-white text-nowrap font-weight-bold">Posted Date</th>
                        <th class="text-left">Posted (1st Year)</th>
                        <th class="text-left">Posted (2nd Year)</th>
                        <th class="text-left">Premium under advice</th>
                      </tr>
                    </thead>
                    <!-- Table Body - Display FSP account postings summary -->
                    <tbody>
                      <tr *ngFor="let item of FSPAccountPostingsSummaryList; ">
                        <td class="text-nowrap">{{ item.Posted_Date }}</td>
                        <td class="text-right">{{formatCurrency(item.Posted_First_Year.toFixed(2))}}</td>
                        <td class="text-right">{{ formatCurrency(item.Posted_Second_Year)}}</td>
                        <td class="text-right">{{formatCurrency(item.Posted_First_Year+ item.Posted_Second_Year)}}</td>
                      </tr>
                      <!-- Total row for FSP account postings summary -->
                      <tr>
                        <td class="dark-blue-bg text-white font-weight-bold">Total</td>
                       <td class="text-right font-weight-bold">{{ formatCurrency(firstYearSum) }}</td>
                       <td class="text-right font-weight-bold">{{ formatCurrency(secondYearSum) }}</td>
                       <td class="text-right font-weight-bold">{{ formatCurrency(totalSum) }}</td>
                      </tr>
                    </tbody>
                  </table>
                </div>
              </div>

              <!-- Future-dated production Section -->
              <div class="col-6">
                <!-- Heading for Future-dated production -->
                <h4 class="monthly-production-summary skyblue-bg-title text-white text-center">Future-dated production</h4>
                <div class="monthly-table">
                  <!-- Table for Future-dated production -->
                  <table width="100%" cellpadding="0" cellspacing="0">
                    <!-- Table Headers -->
                    <thead>
                      <tr>
                        <th class="text-left text-nowrap text-white font-weight-bold">Due date</th>
                        <th style='height:50px;' class='text-left'>Fiduciary fees</th>
                        <th class="text-left">Allocated amount</th>
                      </tr>
                    </thead>
                    <!-- Table Body - Display Future-dated production -->
                    <tbody>
                      <tr *ngFor="let item of FutureDatedProductionList;">
                        <td class="text-nowrap">{{item.Due_Date }}</td>
                        <td> {{ item.Product_Description.trim() == 'Commission Service Fee' ? 'Premium Under Advise Fee' : item.Product_Description }} </td>
                        <td class="text-right">{{ formatCurrency(item.Allocated_Amount.toFixed(2)) }}</td>
                      </tr>
                      <!-- Total row for Future-dated production -->
                      <tr>
                        <td class="text-right font-weight-bold" colspan="2">SubTotal</td>
                        <td class="text-right font-weight-bold">{{ formatCurrency(allocatedAmountSum) }}</td>
                      </tr>
                    </tbody>
                  </table>
                </div>
              </div>
            </div>
          </div>
            <!-- Monthly production summary Section -->
          <div class="earnings-section-monthly d-flex">
            <!-- Two Columns Layout -->
            <div class="d-flex gap-1 w-100">
              <!-- Monthly production summary T1 -->
              <div class="col-6">
                <!-- Heading for Monthly production summary T1 -->
                <h4 class="monthly-production-summary skyblue-bg-title text-white text-center">Monthly production summary</h4>
                <div class="monthly-table">
                  <!-- Table for Monthly production summary T1 -->
                  <table width="100%" cellpadding="0" cellspacing="0">
                    <!-- Table Headers -->
                  <thead>
                    <tr>
                      <th class="text-nowrap text-white font-weight-bold text-left">Month</th>
                      <th class="text-left">Premium Under Advice(1st Year)</th>
                      <th class="text-left">Premium Under Advice(2nd Year)</th>
                    </tr>
                  </thead>
                    <!-- Table Body - Display Monthly production summary T1 -->
                    <tbody>
                      <tr *ngFor="let item of monthlyProductionSummaryT1List">
                        <td class="text-nowrap">{{item.Month }}</td>
                        <td class="text-right">{{ formatCurrency(item.Premium_Under_Advice_Td1.toFixed(2)) }}</td>
                        <td class="text-right">{{ formatCurrency(item.Premium_Under_Advice_Td2.toFixed(2)) }}</td>
                      </tr>
                      <!-- Total row for Monthly production summary T1 -->
                      <tr>
                        <td class="dark-blue-bg text-white font-weight-bold">Total</td>
                        <td class="text-right font-weight-bold">{{formatCurrency(premiumUnderAdviceTd1Sum)}}</td>
                        <td class="text-right font-weight-bold">{{formatCurrency(premiumUnderAdviceTd2Sum)}}</td>
                      </tr>
                    </tbody>
                  </table>
                </div>
              </div>

              <!-- Monthly production summary T2 -->
              <div class="col-6">
                <!-- Heading for Monthly production summary T2 -->
                <h4 class="monthly-production-summary skyblue-bg-title text-white text-center">Monthly production summary</h4>
                <div class="monthly-table">
                  <!-- Table for Monthly production summary T2 -->
                  <table width="100%" cellpadding="0" cellspacing="0">
                    <!-- Table Headers -->
                  <thead>
                    <tr>
                      <th class="text-nowrap text-white font-weight-bold text-left">Month</th>
                      <th class="text-left">Fiduciary Fees(1st Year)</th>
                      <th class="text-left">Fiduciary Fees(2nd Year)</th>
                    </tr>
                  </thead>
                    <!-- Table Body - Display Monthly production summary T2 -->
                    <tbody>
                      <tr *ngFor="let item of monthlyProductionSummaryT2List">
                        <td class="text-nowrap">{{ item.Month }}</td>
                        <td class="text-right">{{ formatCurrency(item.Fiduciary_Fees_Td1.toFixed(2)) }}</td>
                        <td class="text-right">{{ formatCurrency(item.Fiduciary_Fees_Td2.toFixed(2)) }}</td>
                      </tr>
                      <!-- Total row for Monthly production summary T2 -->
                      <tr>
                        <td class="dark-blue-bg text-white font-weight-bold">Total</td>
                        <td class="text-right font-weight-bold">{{formatCurrency(fiduciaryFeesTd1Sum)}}</td>
                        <td class="text-right font-weight-bold">{{formatCurrency(fiduciaryFeesTd2Sum)}}</td>
                      </tr>
                    </tbody>
                  </table>
                </div>
              </div>
            </div>
          </div>

          <!-- Total Earning Section -->
          <div class="total-earning mb-4">
            <div class="row">
              <div class="col-3 text-right"></div>
              <div class="col-3 text-right">
              <div style="padding-left: 12px;" class="dark-blue-bg text-white text-left font-weight-bold pe-3 py-1">Total earnings</div>
              </div>
              <div class="col-3 text-left">
                <div class="total-price-title py-1 font-weight-bold text-center">{{ formatCurrency(totalEarnings) }}</div>
              </div>
              <div class="col-3 text-right"></div>
            </div>
          </div>
        </div>
      </div>
    </div>`
})

export class PPSEarningForPeriodComponent {
  // Input property to receive data from parent component
  @Input() widgetsGridsterItemArray: any[] = [];

  // Monthly production summary T1 Data
  public monthlyProductionSummaryT1List: any[] = [
    { Month: "1 Sep-30 Sep", Premium_Under_Advice_Td1: 41940.06, Premium_Under_Advice_Td2: 13980.03 },
    { Month: "1 Nov-30 Nov", Premium_Under_Advice_Td1: 12258.45, Premium_Under_Advice_Td2: 4086.16 },
    { Month: "1 Aug-31 Aug", Premium_Under_Advice_Td1: 3802.87, Premium_Under_Advice_Td2: 1267.62 },
    { Month: "1 Jul-31 Jul", Premium_Under_Advice_Td1: 4525.03, Premium_Under_Advice_Td2: 1508.35 }
  ];

  // Monthly production summary T2 Data
  public monthlyProductionSummaryT2List: any[] = [
    { Month: "1 Sep-30 Sep", Fiduciary_Fees_Td1: 41940.06, Fiduciary_Fees_Td2: 13980.03 },
    { Month: "1 Nov-30 Nov", Fiduciary_Fees_Td1: 12258.45, Fiduciary_Fees_Td2: 4086.16 },
    { Month: "1 Aug-31 Aug", Fiduciary_Fees_Td1: 3802.87, Fiduciary_Fees_Td2: 1267.62 },
    { Month: "1 Jul-31 Jul", Fiduciary_Fees_Td1: 4525.03, Fiduciary_Fees_Td2: 1508.35 }
  ];

  // FSP account postings summary Data
  public FSPAccountPostingsSummaryList: any[] = [
    { Posted_Date: "10-Dec-2023", Posted_First_Year: -84343.70, Posted_Second_Year: -28114.52 },
    { Posted_Date: "07-Oct-2022", Posted_First_Year: 95952.63, Posted_Second_Year: 0.00 },
    { Posted_Date: "09-Oct-2023", Posted_First_Year: 0.00, Posted_Second_Year: 31984.20 },
    { Posted_Date: "12-Nov-2022", Posted_First_Year: 8865.64, Posted_Second_Year: 0.00 },
    { Posted_Date: "06-Nov-2023", Posted_First_Year: 0.00, Posted_Second_Year: 11266.57 },
    { Posted_Date: "15-Nov-2022", Posted_First_Year: 12884.95, Posted_Second_Year: 0.00 },
    { Posted_Date: "09-Nov-2022", Posted_First_Year: 12049.16, Posted_Second_Year: 0.00 },
    { Posted_Date: "17-Aug-2022", Posted_First_Year: 6905.91, Posted_Second_Year: 0.00 },
    { Posted_Date: "07-Aug-2023", Posted_First_Year: 0.00, Posted_Second_Year: 5705.91 },
    { Posted_Date: "08-Aug-2022", Posted_First_Year: 10211.82, Posted_Second_Year: 0.00 }
  ];

  // Future-dated production Data
  public FutureDatedProductionList: any[] = [
    { Due_Date: "1 Sep-30 Sep", Product_Description: "Commission Service Fee", Allocated_Amount: 13980.03 },
    { Due_Date: "1 Nov-30 Nov", Product_Description: "Safe Custody Service Fee VAT", Allocated_Amount: 4086.16 },
    { Due_Date: "1 Aug-31 Aug", Product_Description: "Commission Service Fee", Allocated_Amount: 1267.62 },
    { Due_Date: "1 Jul-31 Jul	", Product_Description: "Commission Service Fee", Allocated_Amount: 1508.35 }
  ];

  // Calculate sums for different properties
  // Calculate sum for 'Premium_Under_Advice_Td1' property in 'monthlyProductionSummaryT1List'
  premiumUnderAdviceTd1Sum: number = this.calculateSum('Premium_Under_Advice_Td1', 'monthlyProductionSummaryT1List');

  // Calculate sum for 'Premium_Under_Advice_Td2' property in 'monthlyProductionSummaryT1List'
  premiumUnderAdviceTd2Sum: number = this.calculateSum('Premium_Under_Advice_Td2', 'monthlyProductionSummaryT1List');

  // Calculate sum for 'Fiduciary_Fees_Td1' property in 'monthlyProductionSummaryT2List'
  fiduciaryFeesTd1Sum: number = this.calculateSum('Fiduciary_Fees_Td1', 'monthlyProductionSummaryT2List');

  // Calculate sum for 'Fiduciary_Fees_Td2' property in 'monthlyProductionSummaryT2List'
  fiduciaryFeesTd2Sum: number = this.calculateSum('Fiduciary_Fees_Td2', 'monthlyProductionSummaryT2List');

  // Calculate sum for 'Posted_First_Year' property in 'FSPAccountPostingsSummaryList'
  firstYearSum: number = this.calculateSum('Posted_First_Year', 'FSPAccountPostingsSummaryList');

  // Calculate sum for 'Posted_Second_Year' property in 'FSPAccountPostingsSummaryList'
  secondYearSum: number = this.calculateSum('Posted_Second_Year', 'FSPAccountPostingsSummaryList');

  // Calculate sum for 'Allocated_Amount' property in 'FutureDatedProductionList'
  allocatedAmountSum: number = this.calculateSum('Allocated_Amount', 'FutureDatedProductionList');

  /**
 * Total earnings calculation by summing up premiumUnderAdviceTd1Sum,
 * premiumUnderAdviceTd2Sum, fiduciaryFeesTd1Sum, and fiduciaryFeesTd2Sum.
 * This represents the combined earnings from different sources.
 */
  totalEarnings: number = parseFloat((this.premiumUnderAdviceTd1Sum + this.premiumUnderAdviceTd2Sum + this.fiduciaryFeesTd1Sum + this.fiduciaryFeesTd2Sum).toFixed(2));

  // Calculate total sum combining 'firstYearSum' and 'secondYearSum'
  totalSum: number = this.firstYearSum + this.secondYearSum;

  public formatCurrency(input: any): any {
    const amount: number = parseFloat(input); // Parse the string to a decimal
    const formattedAmount: string = amount.toLocaleString('en-ZA', {
      style: 'currency',
      currency: 'ZAR'
    });

    return ((amount < 0 ? '-' : '') + formattedAmount.replace(',', '.')).replace('--', '-');
  }
  /**
  * Private method to calculate the sum of a specified property in a given list.
  * @param property The property to calculate the sum for.
  * @param listName The name of the list containing items with the specified property.
  * @returns The sum of the specified property in the given list.
  */
  private calculateSum(property: string, listName: string): number {
    // Using the reduce function to iterate through the list and calculate the sum.
    return parseFloat(this[listName].reduce((sum, item) => sum + parseFloat(item[property]), 0).toFixed(2));
  }
}


// Component Created for Image Widget--
@Component({
  selector: 'image-widget',
  template: `<div class="widget">
    <div class="widget-header">
      <span class="widget-header-title">Image </span>
    </div>
    <section class="info-section">
            <div class="subsection">
               <b class="hed">Miss HW HLONGWANE</b> <br>
               <b>Client Contact Details</b><br>
               Mobile: 082-1234567<br>
              <span class="blue">Email: <u>y.vanheerden@google.com</u></span>
            </div>
                    <div class="vl"></div>
            <div class="subsection">
                <b>Address</b><br>
                1 John Vorster Drive<br>
                Randburg<br>
                Gauteng
            </div>
                <div class="vl"></div>
            <div class="subsection">
                <b>Intermediary Reference</b>&nbsp;&nbsp;: 27959<br>
                <b>Measure Type</b>&nbsp; &emsp;&emsp;&emsp;&emsp;: 124411742<br>
                <b>Month </b> &emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp; : September 2023<br>
                <b>Date From </b>&emsp;&emsp;&emsp;&emsp;&emsp;&nbsp;&nbsp;&nbsp;: 2023-9-10 to<br>
                   &emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&nbsp;&nbsp; 2023-01-01
            </div>
        </section>
        <hr class="horizontal-line">
  </div>`
})
export class ImageComponent {

  @Input() imgItem: any;

  public ImageSrc: any;
  public ImageId: any;
  public baseURL = AppSettings.baseURL;

  constructor(private _http: HttpClient,
    private sanitizer: DomSanitizer) { }

  ngOnInit() {
    if (this.imgItem != null && this.imgItem.WidgetSetting != null && this.imgItem.WidgetSetting != '' && this.testJSON(this.imgItem.WidgetSetting)) {
      let widgetSetting = JSON.parse(this.imgItem.WidgetSetting);
      if (!widgetSetting.isPersonalize && widgetSetting.AssetLibraryId != 0 && widgetSetting.AssetId != 0) {
        this._http.get(this.baseURL + 'assetlibrary/asset/download?assetIdentifier=' + widgetSetting.AssetId, { responseType: "arraybuffer", observe: 'response' }).pipe(map(response => response))
          .subscribe(
            data => {
              let contentType = data.headers.get('Content-Type');
              let fileName = data.headers.get('x-filename');
              const blob = new Blob([data.body], { type: contentType });
              let objectURL = URL.createObjectURL(blob);
              this.ImageSrc = this.sanitizer.bypassSecurityTrustResourceUrl(objectURL); //this.sanitizer.sanitize(SecurityContext.RESOURCE_URL, this.sanitizer.bypassSecurityTrustResourceUrl(objectURL));
            },
            error => {
              //$('.overlay').show();
            });
      } else {
        this.ImageSrc = 'assets/images/icon-image.png';
      }
      this.ImageId = 'Image' + this.imgItem.TempImageIdentifier;
    }
    else {
      this.ImageSrc = 'assets/images/icon-image.png';
      this.ImageId = 'Image1';
    }

  }

  testJSON(text) {
    if (typeof text !== "string") {
      return false;
    }
    try {
      JSON.parse(text);
      return true;
    }
    catch (error) {
      return false;
    }
  }

  ngAfterViewInit() {
    setTimeout(() => {
      if (this.imgItem != null && this.imgItem.WidgetSetting != null && this.imgItem.WidgetSetting != '' && this.testJSON(this.imgItem.WidgetSetting)) {
        let widgetSetting = JSON.parse(this.imgItem.WidgetSetting);
        if (widgetSetting.TempImageIdentifier != null) {
          if (widgetSetting.Height != null && widgetSetting.Height != 0) {
            $('#Image' + widgetSetting.TempImageIdentifier).css('height', widgetSetting.Height + 'px');
          } else {
            $('#Image' + widgetSetting.TempImageIdentifier).css('height', 'auto');
          }

          if (widgetSetting.Align != null) {
            var className = widgetSetting.Align == 1 ? 'text-left' : widgetSetting.Align == 2 ? 'text-right' : 'text-center';
            $('#Image' + widgetSetting.TempImageIdentifier).parent().addClass(className);
          }
        } else {
          $('#Image1').css('height', 'auto');
        }
      }
    }, 100);
  }
}

// Component Created for Video Widget--
@Component({
  selector: 'vidyo-widget',
  template: `<div class="widget">
    <div class="widget-header">
      <span class="widget-header-title">Video </span>
    </div>
    <div class="widget-area position-relative width100">
      <div class="widget-indicator-inner text-center" [id]="videoWidgetDivId">
        <video controls style="height: 200px;width: 75%;" [id]="videoControlTagId">
          <source src='{{videoSrc}}' type="video/mp4">
        </video>
      </div>
    </div>
  </div>`
})
export class VideoComponent {

  @Input() vdoItem: any;

  public videoSrc;
  public videoWidgetDivId;
  public videoControlTagId;
  public baseURL = AppSettings.baseURL;

  constructor(private _http: HttpClient,
    private sanitizer: DomSanitizer) { }

  ngOnInit() {
    if (this.vdoItem != null) {
      this.videoWidgetDivId = "VideoWidgetDiv" + this.vdoItem.x + this.vdoItem.y;
      this.videoControlTagId = 'videoConfigPreviewSrc' + this.vdoItem.x + this.vdoItem.y;
    }
    if (this.vdoItem != null && this.vdoItem.WidgetSetting != null && this.vdoItem.WidgetSetting != '' && this.testJSON(this.vdoItem.WidgetSetting)) {
      let widgetSetting = JSON.parse(this.vdoItem.WidgetSetting);
      if (widgetSetting.isEmbedded) {
        this.videoSrc = widgetSetting.SourceUrl;
      }
      else if (!widgetSetting.isPersonalize && widgetSetting.AssetLibraryId != 0 && widgetSetting.AssetId != 0) {
        this._http.get(this.baseURL + 'assetlibrary/asset/download?assetIdentifier=' + widgetSetting.AssetId, { responseType: "arraybuffer", observe: 'response' }).pipe(map(response => response))
          .subscribe(
            data => {
              let contentType = data.headers.get('Content-Type');
              const blob = new Blob([data.body], { type: contentType });
              let objectURL = URL.createObjectURL(blob);
              var url = this.sanitizer.sanitize(SecurityContext.RESOURCE_URL, this.sanitizer.bypassSecurityTrustResourceUrl(objectURL));
              var videoDiv = document.getElementById(this.videoWidgetDivId);
              if (videoDiv != undefined && videoDiv != null) {
                if (videoDiv.hasChildNodes()) {
                  videoDiv.removeChild(document.getElementById(this.videoControlTagId));
                }

                var video = document.createElement('video');
                video.id = this.videoControlTagId;
                video.style.height = "200px";
                video.style.width = "75%";
                video.controls = true;

                var sourceTag = document.createElement('source');
                sourceTag.setAttribute('src', url);
                sourceTag.setAttribute('type', 'video/mp4');
                video.appendChild(sourceTag);
                videoDiv.appendChild(video);
              }
            },
            error => {
              //$('.overlay').show();
            });
      }
      else {
        this.videoSrc = 'assets/images/SampleVideo.mp4';
      }
    } else {
      this.videoSrc = 'assets/images/SampleVideo.mp4';
    }
  }

  testJSON(text) {
    if (typeof text !== "string") {
      return false;
    }
    try {
      JSON.parse(text);
      return true;
    }
    catch (error) {
      return false;
    }
  }
}

// Component Created for Summary at glance Widget--
@Component({
  selector: 'summaryAtGlance',
  template: `<div class="widget">
    <div class="widget-header">
      <span class="widget-header-title"> Summary at Glance </span>
    </div>
    <div class="widget-area-grid padding-0">
      <div class="d-flex justify-content-center mb-4">
        <div class="pagination-mat position-relative d-md-block d-none">
          <mat-paginator #paginator [pageSize]="pageSize" [pageSizeOptions]="[5, 10, 20]"
                         [showFirstLastButtons]="true" [length]="totalSize" [pageIndex]="currentPage">
          </mat-paginator>
        </div>
      </div>
      <table mat-table [dataSource]="dataSourceSummary" matSort class="table-cust">
        <!-- Position Column -->
        <ng-container matColumnDef="account">
          <th class="width15" mat-header-cell *matHeaderCellDef mat-sort-header> Account </th>
          <td mat-cell *matCellDef="let element"> {{element.account}} </td>
        </ng-container>

        <!-- Name Column -->
        <ng-container matColumnDef="currency">
          <th class="width10" mat-header-cell *matHeaderCellDef mat-sort-header> Currency </th>
          <td mat-cell *matCellDef="let element"> {{element.currency}} </td>
        </ng-container>

        <ng-container matColumnDef="amount">
          <th class="width10" mat-header-cell *matHeaderCellDef mat-sort-header> Amount </th>
          <td mat-cell *matCellDef="let element"> {{element.amount}} </td>
        </ng-container>

        <tr mat-header-row *matHeaderRowDef="displayedColumnsSummary"></tr>
        <tr mat-row *matRowDef="let row; columns: displayedColumnsSummary;"></tr>
      </table>
    </div>
  </div>`
})
export class SummaryAtGlanceComponent {
  @Input()
  widgetsGridsterItemArray: any[] = [];
  displayedColumnsSummary: string[] = ['account', 'currency', 'amount'];
  dataSourceSummary = new MatTableDataSource<any>();
  public pageSize = 5;
  public currentPage = 0;
  public totalSize = 0;

  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;

  ngAfterViewInit() {
    this.dataSourceSummary.paginator = this.paginator;
  }

  ngOnInit() {
    this.dataSourceSummary = new MatTableDataSource(List_Data_Summary);
    this.dataSourceSummary.sort = this.sort;
  }
}

export interface ListSummary {
  account: string;
  currency: string;
  amount: string;
}

const List_Data_Summary: ListSummary[] = [
  { account: 'Saving Account', currency: 'GBP', amount: '873899' },
  { account: 'Current Account', currency: 'GBP', amount: '873899' },
  { account: 'Recurring Deposit', currency: 'GBP', amount: '873899' },
  { account: 'Wealth', currency: 'GBP', amount: '873899' },
];

// Component Created for saving Available Balance Widget--
@Component({
  selector: 'savingAvailableBalance',
  template: `<div class="widget">
    <div class="widget-header">
        <span class="widget-header-title"> Saving : Available Balance </span>
    </div>
    <div class="widget-area">
          <h4 style="text-align:right" class='mb-4'><i style="color: limegreen" class="fa fa-sort-asc" aria-hidden="true"></i>3245323</h4>
          <span style="float:left;">Total Deposits</span><span style="float:right;">16750,00</span><br/>
          <span style="float:left;">Total Spend</span><span style="float:right;">16750,00</span><br/>
          <span style="float:left;">Savings</span><span style="float:right;">3250.00</span><br/>
    </div>
</div>`
})
export class SavingAvailableBalanceComponent {
  @Input()
  widgetsGridsterItemArray: any[] = [];
}

// Component Created for Current Available Balance Widget--
@Component({
  selector: 'currentAvailableBalance',
  template: `<div class="widget">
      <div class="widget-header">
          <span class="widget-header-title"> Current : Available Balance </span>
      </div>
      <div class="widget-area  position-relative width100">
        <h4 style="text-align:right" class='mb-4'><i style="color: limegreen" class="fa fa-sort-asc" aria-hidden="true"></i> 3245323</h4>
          <span style="float:left;">Total Deposits</span><span style="float:right;">1675000</span><br/>
          <span style="float:left;">Total Spend</span><span style="float:right;">1675000</span><br/>
          <span style="float:left;">Savings</span><span style="float:right;">3250.00</span><br/>
    </div>
</div>`
})
export class CurrentAvailableBalanceComponent {
  @Input()
  widgetsGridsterItemArray: any[] = [];
}

// Component Created for Current Transaction details Widget--
@Component({
  selector: 'transactionDetails',
  template: `<div class="widget">
  <div class="widget-header">
    <span class="widget-header-title"> Transaction Details
      <span class="float-right mr-4"><i class="fa fa-caret-left mr-2" style="color:red;font-size: 20px;"></i>Chat Now</span>
    </span>
  </div>
  <div class="widget-area-grid padding-0">
      <div class='float-left'>
        <input type='radio' id='showAll' name='showAll' [checked]='isShowAll' (change)='ShowAll($event.target)'>&nbsp;<label for='showAll'>Show All</label>&nbsp;
        <input type='radio' id='grpDate' name='showAll' [checked]='isGroupByDate' (change)='GroupByDate($event.target)'>&nbsp;<label for='grpDate'>Group By Date</label>&nbsp;
      </div>
      
      <form [formGroup]="transactionForm">
      <div class='float-right'>
          <div class="float-left mr-2" *ngIf="isShowAll">
            <select class="form-control float-left" formControlName="filterStatus" id="filterStatus">
              <option value="0"> Search Item</option>
              <option value="Failed">Failed</option>
              <option value="Completed">Completed</option>
              <option value="In Progress">In Progress</option>
            </select>
          </div>
          <button href='javascript:void(0)' *ngIf="isShowAll" class='btn btn-light btn-sm'>Search</button>&nbsp;
          <button href='javascript:void(0)' class='btn btn-light btn-sm'>Reset</button>&nbsp;
          <button href='javascript:void(0)' class='btn btn-light btn-sm'>Print</button> 
      </div>
    </form>

    <div class="d-flex justify-content-center mb-4">
      <div class="pagination-mat position-relative d-md-block d-none">
        <mat-paginator #paginator [pageSize]="pageSize" [pageSizeOptions]="[5, 10, 20]"
                       [showFirstLastButtons]="true" [length]="totalSize" [pageIndex]="currentPage">
        </mat-paginator>
      </div>
    </div>
    <table mat-table [dataSource]="dataSource" matSort class="table-cust">
      <!-- Position Column -->
      <ng-container matColumnDef="date">
        <th class="width15" mat-header-cell *matHeaderCellDef mat-sort-header> Date </th>
        <td mat-cell *matCellDef="let element"> {{element.date}} </td>
      </ng-container>

      <!-- Name Column -->
      <ng-container matColumnDef="type">
        <th class="width10" mat-header-cell *matHeaderCellDef mat-sort-header> Type </th>
        <td mat-cell *matCellDef="let element"> {{element.type}} </td>
      </ng-container>

      <!-- Weight Column -->
      <ng-container matColumnDef="narration">
        <th class="width20" mat-header-cell *matHeaderCellDef mat-sort-header> Narration </th>
        <td mat-cell *matCellDef="let element"> {{element.narration}} </td>
      </ng-container>

      <!-- Symbol Column -->
      <ng-container matColumnDef="fcy">
        <th class="width15" mat-header-cell *matHeaderCellDef mat-sort-header> FCY(GBP) </th>
        <td mat-cell *matCellDef="let element"> {{element.fcy}} </td>
      </ng-container>

      <ng-container matColumnDef="currentRate">
        <th class="width15" mat-header-cell *matHeaderCellDef mat-sort-header> Current Rate </th>
        <td mat-cell *matCellDef="let element"> {{element.currentRate}} </td>
      </ng-container>

      <ng-container matColumnDef="lyc">
        <th class="width15" mat-header-cell *matHeaderCellDef mat-sort-header> LCY(GBP) </th>
        <td mat-cell *matCellDef="let element"> {{element.lyc}} </td>
      </ng-container>

      <ng-container matColumnDef="action">
        <th class="width10" mat-header-cell *matHeaderCellDef mat-sort-header> Action </th>
        <td mat-cell *matCellDef="let element">
          <div class="action-btns btn-tbl-action">
            <button type="button" title="View" id="btnView" routerLink="View"><span class="fa fa-paper-plane-o"></span></button>
          </div>
        </td>
      </ng-container>

      <tr mat-header-row *matHeaderRowDef="displayedColumns"></tr>
      <tr mat-row *matRowDef="let row; columns: displayedColumns;"></tr>
    </table>
    <!--mobile view start-->
    <div class="mobile-tile card-shadow d-md-none border-0" *ngFor="let list of TransactionDetailData" style="display:none;">
      <div class="row mb-1">
        <div class="col-5"> <label class="m-0"> Date </label> </div>
        <div class="col-7">  {{list.date}} </div>
      </div>
      <div class="row mb-1">
        <div class="col-5"> <label class="m-0"> Type </label> </div>
        <div class="col-7">  {{list.type}} </div>
      </div>
      <div class="row mb-1">
        <div class="col-5"> <label class="m-0">Narration </label> </div>
        <div class="col-7">  {{list.narration}} </div>
      </div>
      <div class="row mb-1">
        <div class="col-5"> <label class="m-0">FCY(GBP) </label> </div>
        <div class="col-7">  {{list.fcy}} </div>
      </div>
      <div class="row mb-1">
        <div class="col-5"> <label class="m-0"> Current Rate </label> </div>
        <div class="col-7">  {{list.currentRate}} </div>
      </div>
      <div class="row mb-1">
        <div class="col-5"> <label class="m-0">LCY(GBP) </label> </div>
        <div class="col-7">  {{list.lyc}} </div>
      </div>
      <div class="text-center">
        <div class="action-btns btn-tbl-action">
          <button type="button"><span class="fa fa-paper-plane-o"></span></button>
        </div>
      </div>
    </div>
    <!--mobile view end-->
  </div>`
})
export class TransactionDetailsComponent {
  @Input()
  widgetsGridsterItemArray: any[] = [];
  displayedColumns: string[] = ['date', 'type', 'narration', 'fcy', 'currentRate', 'lyc', 'action'];
  dataSource = new MatTableDataSource<any>();
  public pageSize = 5;
  public currentPage = 0;
  public totalSize = 0;
  public transactionForm: FormGroup;

  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;

  TransactionDetailData: TransactionDetail[] = [
    { date: '15/07/2020', type: 'CR', narration: 'NXT TXN: IIFL IIFL6574562', fcy: '1666.67', currentRate: '1.062', lyc: '1771.42' },
    { date: '15/07/2020', type: 'CR', narration: 'NXT TXN: IIFL IIFL6574562', fcy: '1666.67', currentRate: '1.062', lyc: '1771.42' },
    { date: '15/07/2020', type: 'DB', narration: 'NXT TXN: IIFL IIFL6574562', fcy: '1666.67', currentRate: '1.062', lyc: '1771.42' },

    { date: '19/07/2020', type: 'CR', narration: 'NXT TXN: IIFL IIFL3557346', fcy: '1254.71', currentRate: '1.123', lyc: '1876.00' },
    { date: '19/07/2020', type: 'CR', narration: 'NXT TXN: IIFL IIFL3557346', fcy: '1254.71', currentRate: '1.123', lyc: '1876.00' },

    { date: '25/07/2020', type: 'CR', narration: 'NXT TXN: IIFL IIFL8965435', fcy: '2345.12', currentRate: '1.461', lyc: '1453.21' },
    { date: '28/07/2020', type: 'CR', narration: 'NXT TXN: IIFL IIFL0034212', fcy: '1435.89', currentRate: '0.962', lyc: '1654.56' },
  ];
  TransactionDetailDataGroup: TransactionDetail[] = [
    { date: '15/07/2020', type: 'CR', narration: '-', fcy: '3333.34', currentRate: '2.124', lyc: '3542.84' },
    { date: '15/07/2020', type: 'DB', narration: '-', fcy: '1666.67', currentRate: '1.062', lyc: '1771.42' },

    { date: '19/07/2020', type: 'CR', narration: '-', fcy: '2491.42', currentRate: '2.246', lyc: '3752.00' },

    { date: '28/07/2020', type: 'CR', narration: '-', fcy: '1435.89', currentRate: '0.962', lyc: '1654.56' },
    { date: '25/07/2020', type: 'CR', narration: '-', fcy: '2345.12', currentRate: '1.461', lyc: '1453.21' },
  ];

  ngAfterViewInit() {
    this.dataSource.paginator = this.paginator;
  }
  public isShowAll = true;
  public isGroupByDate = false;
  ShowAll(event) {
    const value = event.checked;
    this.isShowAll = event.checked;
    if (value) {
      this.isGroupByDate = false;

      this.dataSource = new MatTableDataSource(this.TransactionDetailData);
      this.dataSource.sort = this.sort;
    }
    else {
      this.isGroupByDate = true;

      this.dataSource = new MatTableDataSource(this.TransactionDetailDataGroup);
      this.dataSource.sort = this.sort;
    }
  }
  GroupByDate(event) {
    const value = event.checked;
    this.isGroupByDate = event.checked;

    if (value) {
      this.isShowAll = false;
      this.dataSource = new MatTableDataSource(this.TransactionDetailDataGroup);
      this.dataSource.sort = this.sort;
    }
    else {
      this.isShowAll = true;

      this.dataSource = new MatTableDataSource(this.TransactionDetailData);
      this.dataSource.sort = this.sort;
    }
  }
  ngOnInit() {
    this.transactionForm = this.fb.group({
      filterStatus: [0],
    });
    this.dataSource = new MatTableDataSource(this.TransactionDetailData);
    this.dataSource.sort = this.sort;
  }
  constructor(
    private fb: FormBuilder,
  ) {

  }
}

// Component Created for Saving Transaction details Widget--
@Component({
  selector: 'savingtransactionDetails',
  template: `<div class="widget">
    <div class="widget-header">
      <span class="widget-header-title"> Transaction Details </span>

    </div>
    <div class="widget-area-grid padding-0">
    <div class='float-left'>
        <input type='radio' id='showAll' name='showAll' [checked]='isShowAll' (change)='ShowAll($event.target)'>&nbsp;<label for='showAll'>Show All</label>&nbsp;
        <input type='radio' id='grpDate' name='showAll' [checked]='isGroupByDate' (change)='GroupByDate($event.target)'>&nbsp;<label for='grpDate'>Group By Date</label>&nbsp;
      </div>
    
     <div class='float-right'>
          <div class="float-left mr-2">
         
            <select  class="form-control float-left"  id="filterStatus">
              <option value="0"> Search Item</option>
              <option value="Failed">Failed</option>
              <option value="Completed">Completed</option>
              <option value="In Progress">In Progress</option>
            </select>
          </div>
          <button href='javascript:void(0)'  class='btn btn-light btn-sm'>Search</button>&nbsp;
          <button href='javascript:void(0)' class='btn btn-light btn-sm'>Reset</button>&nbsp;
          <button href='javascript:void(0)' class='btn btn-light btn-sm'>Print</button> 
      </div>
      <div class="d-flex justify-content-center mb-4">
        <div class="pagination-mat position-relative d-md-block d-none">
          <mat-paginator #paginator [pageSize]="pageSize" [pageSizeOptions]="[5, 10, 20]"
                         [showFirstLastButtons]="true" [length]="totalSize" [pageIndex]="currentPage">
          </mat-paginator>
        </div>
      </div>
      <table mat-table [dataSource]="dataSource" matSort class="table-cust">
        <!-- Position Column -->
        <ng-container matColumnDef="date">
          <th class="width15" mat-header-cell *matHeaderCellDef mat-sort-header> Date </th>
          <td mat-cell *matCellDef="let element"> {{element.date}} </td>
        </ng-container>
  
        <!-- Name Column -->
        <ng-container matColumnDef="type">
          <th class="width10" mat-header-cell *matHeaderCellDef mat-sort-header> Type </th>
          <td mat-cell *matCellDef="let element"> {{element.type}} </td>
        </ng-container>
  
        <!-- Weight Column -->
        <ng-container matColumnDef="narration">
          <th class="width20" mat-header-cell *matHeaderCellDef mat-sort-header> Narration </th>
          <td mat-cell *matCellDef="let element"> {{element.narration}} </td>
        </ng-container>
  
        <!-- Symbol Column -->
        <ng-container matColumnDef="credit" >
          <th class="width15" mat-header-cell *matHeaderCellDef mat-sort-header> Credit </th>
          <td mat-cell *matCellDef="let element" style="color:limegreen"> {{element.credit}} </td>
        </ng-container>
  
        <ng-container matColumnDef="debit">
          <th class="width15" mat-header-cell *matHeaderCellDef mat-sort-header> Debit </th>
          <td mat-cell *matCellDef="let element" style="color:red"> {{element.debit}} </td>
        </ng-container>
  
        <ng-container matColumnDef="query">
          <th class="width15" mat-header-cell *matHeaderCellDef mat-sort-header> Query </th>
          <td mat-cell *matCellDef="let element"> <i class="fa fa-caret-left fa-3x" style="color:red" aria-hidden="true"></i></td>
        </ng-container>
        <ng-container matColumnDef="balance">
          <th class="width15" mat-header-cell *matHeaderCellDef mat-sort-header> Balance </th>
          <td mat-cell *matCellDef="let element"> {{element.balance}} </td>
        </ng-container>
  
  
        <tr mat-header-row *matHeaderRowDef="displayedColumns"></tr>
        <tr mat-row *matRowDef="let row; columns: displayedColumns;"></tr>
      </table>
      <!--mobile view start-->
      <div class="mobile-tile card-shadow d-md-none border-0" *ngFor="let list of TransactionDetailData" style="display:none;">
        <div class="row mb-1">
          <div class="col-5"> <label class="m-0"> Date </label> </div>
          <div class="col-7">  {{list.date}} </div>
        </div>
        <div class="row mb-1">
          <div class="col-5"> <label class="m-0"> Type </label> </div>
          <div class="col-7">  {{list.type}} </div>
        </div>
        <div class="row mb-1">
          <div class="col-5"> <label class="m-0">Narration </label> </div>
          <div class="col-7">  {{list.narration}} </div>
        </div>
        <div class="row mb-1">
          <div class="col-5"> <label class="m-0">Credit</label> </div>
          <div class="col-7">  {{list.credit}} </div>
        </div>
        <div class="row mb-1">
          <div class="col-5"> <label class="m-0"> Debit </label> </div>
          <div class="col-7">  {{list.debit}} </div>
        </div>
        <div class="row mb-1">
          <div class="col-5"> <label class="m-0">Query</label> </div>
          <div class="col-7">  {{list.query}} </div>
        </div>
        <div class="row mb-1">
          <div class="col-5"> <label class="m-0">Balance</label> </div>
          <div class="col-7">  {{list.balance}} </div>
        </div>
        
      </div>
      <!--mobile view end-->
    </div>`
})
export class SavingTransactionDetailsComponent {
  @Input()
  widgetsGridsterItemArray: any[] = [];
  displayedColumns: string[] = ['date', 'type', 'narration', 'credit', 'query', 'debit', 'balance',];
  dataSource = new MatTableDataSource<any>();
  public pageSize = 5;
  public currentPage = 0;
  public totalSize = 0;
  public transactionForm: FormGroup;
  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;

  TransactionDetailData: any[] = [
    { date: '15/07/2020', type: 'CR', query: '', narration: 'ACB Credit :Salary', credit: 'R\'1666.67', debit: '', balance: 'R\'1771.42' },
    { date: '15/07/2020', type: 'CR', query: '', narration: 'INTEREST', credit: 'R\'500.00', debit: '', balance: 'R\'1771.42' },
    { date: '18/07/2020', type: 'DB', query: '', narration: 'ACB Debit :Mobile', credit: '', debit: '-R\'100.123', balance: 'R\'1876.00' },
    { date: '18/07/2020', type: 'DB', query: '', narration: 'ACB Debit :DSTV', credit: '', debit: '-R\'500.00', balance: 'R\'1876.00' },
    { date: '22/07/2020', type: 'DB', query: '', narration: 'Bank Charges', credit: '', debit: '-R\'1.461', balance: 'R\'1453.21' },
    { date: '23/07/2020', type: 'DB', query: '', narration: 'INTERNET BANKING FEE', credit: '', debit: '-R\'0.962', balance: 'R\'1654.56' },
    { date: '25/07/2020', type: 'CR', query: '', narration: 'ACB Credit :Medical Aid', credit: 'R\'1666.67', debit: '-R\'1.062', balance: 'R\'1771.42' },
    { date: '25/07/2020', type: 'CR', query: '', narration: 'INTEREST', credit: 'R\'500.00', debit: '', balance: 'R\'1771.42' },
    { date: '26/07/2020', type: 'DB', query: '', narration: 'IBANK Payement:Electricity', credit: '', debit: '-R\'1.461', balance: 'R\'1453.21' },
    { date: '29/07/2020', type: 'DB', query: '', narration: 'IBANK Payement:Home Loan', credit: '', debit: '-R\'1.461', balance: 'R\'1453.21' },
  ];

  TransactionDetailDataGroup: any[] = [
    { date: '15/07/2020', type: 'CR', query: '', narration: '-', credit: 'R\'2166.67', debit: '', balance: 'R\'2271.42' },
    { date: '18/07/2020', type: 'DB', query: '', narration: '-', credit: '', debit: '-R\'600.123', balance: 'R\'1671.29' },
    { date: '22/07/2020', type: 'DB', query: '', narration: '-', credit: '', debit: '-R\'1.461', balance: 'R\'1669.83' },
    { date: '23/07/2020', type: 'DB', query: '', narration: '-', credit: '', debit: '-R\'0.962', balance: 'R\'1668.87' },
    { date: '25/07/2020', type: 'CR', query: '', narration: '-', credit: 'R\'2166.67', debit: '', balance: 'R\'3835.54' },
    { date: '26/07/2020', type: 'DB', query: '', narration: '-', credit: '', debit: '-R\'1.461', balance: 'R\'3834.07' },
    { date: '29/07/2020', type: 'DB', query: '', narration: '-', credit: '', debit: '-R\'1.461', balance: 'R\'3882.60' },
  ];

  ngAfterViewInit() {
    this.dataSource.paginator = this.paginator;
  }

  ngOnInit() {
    this.dataSource = new MatTableDataSource(this.TransactionDetailData);
    this.dataSource.sort = this.sort;
  }
  public isShowAll = true;
  public isGroupByDate = false;

  ShowAll(event) {
    const value = event.checked;
    this.isShowAll = event.checked;
    if (value) {
      this.isGroupByDate = false;

      this.dataSource = new MatTableDataSource(this.TransactionDetailData);
      this.dataSource.sort = this.sort;
    }
    else {
      this.isGroupByDate = true;

      this.dataSource = new MatTableDataSource(this.TransactionDetailDataGroup);
      this.dataSource.sort = this.sort;
    }
  }
  GroupByDate(event) {
    const value = event.checked;
    this.isGroupByDate = event.checked;

    if (value) {
      this.isShowAll = false;
      this.dataSource = new MatTableDataSource(this.TransactionDetailDataGroup);
      this.dataSource.sort = this.sort;
    }
    else {
      this.isShowAll = true;

      this.dataSource = new MatTableDataSource(this.TransactionDetailData);
      this.dataSource.sort = this.sort;
    }
  }
}

// Component Created for Reminder and Recommendation Widget--
@Component({
  selector: 'reminderAndRecommendation',
  template: `<div class="widget">
      <div class="widget-header">
         <span class="widget-header-title"> Reminder and Recommendations </span>
      </div>
      <div class="widget-area-grid" style="overflow-x:hidden;">
        <div class="row">
          <div class="col-lg-9">
          </div>
          <div class="col-lg-3">
            <i class="fa fa-caret-left fa-3x float-left text-danger" aria-hidden="true"></i>
            <span class="mt-2 d-inline-block ml-2">Click</span>
          </div>       
        </div>
        <div class="row" *ngFor="let list of actionList">
          <div class="col-lg-9 text-left">
            <p class="p-1" style="background-color: #dce3dc;">{{list.title}} </p>
          </div>
          <div class="col-lg-3">
            <a><i class="fa fa-caret-left fa-3x float-left text-danger"></i>
            <span class="mt-2 d-inline-block ml-2">{{list.action}}</span></a>
          </div>
        </div>
      </div>
</div>`
})
export class ReminderAndRecommComponent {
  @Input()
  widgetsGridsterItemArray: any[] = [];
  public actionList: any[] = [
    { title: "Update Missing Inofrmation", action: "Update" },
    { title: "Your Rewards Video ia available", action: "View" },
    { title: "Payment Due for Home Loan", action: "Pay" },
    { title: "Need financial planning for savings.", action: "Call Me" },
    { title: "Subscribe/Unsubscribe Alerts.", action: "Apply" },
    { title: "Your credit card payment is due now.", action: "Pay" }
  ];
}

// Component Created for Analytics Widget--
@Component({
  selector: 'analytics',
  template: `<div class="widget">
    <div class="widget">
    <div class="widget-header">
        <span class="widget-header-title"> Analytics </span>
    </div>
    <div class="widget-area position-relative width100">       
        <div id="chartWidgetPiecontainer" class="p-3"></div>
    </div>
</div>`
})
export class AnalyticsWidgetComponent {
  @Input()
  widgetsGridsterItemArray: any[] = [];

  public options4: any = {
    chart: {
      backgroundColor: 'rgba(0,0,0,0)',
      plotBorderWidth: null,
      plotShadow: false,
      type: 'pie',
      height: (9 / 16 * 100) + '%',
    },
    title: {
      text: ''
    },
    tooltip: {
      pointFormat: '{series.name}: <b>{point.percentage:.1f}%</b>'
    },
    accessibility: {
      point: {
        valueSuffix: '%'
      }
    },
    plotOptions: {
      pie: {
        allowPointSelect: true,
        cursor: 'pointer',
        dataLabels: {
          enabled: true,
          format: '<b>{point.name}</b>: {point.percentage:.1f} %'
        },
        showInLegend: false
      }
    },
    series: [{
      name: 'Percentage',
      colorByPoint: true,
      data: [
        {
          name: 'Cutomer Information',
          y: 11.84
        }, {
          name: 'Account Information',
          y: 10.85
        }, {
          name: 'Image',
          y: 4.67
        }, {
          name: 'Video',
          y: 4.18
        }, {
          name: 'News Alerts',
          y: 7.05
        }]
    }]
  }

  ngAfterViewInit() {
    setTimeout(() => {
      Highcharts.chart('chartWidgetPiecontainer', this.options4);
    }, 100);
  }

  ngOnInit() {
    $(document).ready(function () {
      setTimeout(function () {
        window.dispatchEvent(new Event('resize'));
      }, 10);
    });
  }
}

// Component Created for Saving Trends Widget--
@Component({
  selector: 'savingTrends',
  template: `<div class="widget">
    <div class="widget">
    <div class="widget-header">
        <span class="widget-header-title"> Your Saving Trends </span>
    </div>
    <div class="widget-area position-relative width100">
      <div class="text-right" style="font-size:20px"><span>+5.6%</span><span class="pl-3">+3.5%</span></div>
      <div id="savingTrendscontainer" class="p-3"></div>        
    </div>
</div>`
})
export class SavingTrendsComponent {
  @Input()
  widgetsGridsterItemArray: any[] = [];
  public options4: any = {
    chart: {
      height: (9 / 16 * 100) + '%',
      backgroundColor: 'rgba(0,0,0,0)'
    },
    title: {
      text: ''
    },
    xAxis: {
      categories: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul']
    },
    labels: {
      items: [{
        //html: 'How you have been spending',
        style: {
          left: '50px',
          top: '18px',
          color: ( // theme
            Highcharts.defaultOptions.title.style &&
            Highcharts.defaultOptions.title.style.color
          ) || 'black'
        }
      }]
    },
    series: [{
      // type: 'spline',
      name: '',
      data: [1.5, 2.5, 3, 1.5, 3, 2, 4],
      marker: {
        lineWidth: 1,
        lineColor: Highcharts.getOptions().colors[3],
        fillColor: 'white'
      }
    }]
  }
  ngAfterViewInit() {
    setTimeout(() => {
      Highcharts.chart('savingTrendscontainer', this.options4);
    }, 100);
  }

  ngOnInit() {
    $(document).ready(function () {
      setTimeout(function () {
        window.dispatchEvent(new Event('resize'));
      }, 10);
    });
  }
}

// Component Created for Top 4 Income sources Widget--
@Component({
  selector: 'topFourIncomdeSources',
  template: `<div class="widget">
   <div class="widget">
      <div class="widget-header">
         <span class="widget-header-title"> Top Four Income Sources </span>
      </div>
      <div class="widget-area">
         <table class="table-borderless width100">
            <thead>
               <tr>
                  <td style="width:50%"></td>
                  <td style="width:20%">This Month</td>
                  <td style="width:30%;">Usually you spend</td>
               </tr>
            </thead>
            <tbody>
               <tr *ngFor="let list of actionList">
                  <td style="width:50%">
                     <label>{{list.name}}</label>
                  </td>
                  <td style="width:20%">
                     {{list.thisMonth}}
                  </td>
                  <td style="width:30%;">
                     <span *ngIf="!list.isAscIcon" style="color: red" class="fa fa-sort-desc fa-2x" aria-hidden="true"></span>
                     <span *ngIf="!list.isAscIcon" class="ml-2">{{list.usuallySpend}}</span>
                      <span *ngIf="list.isAscIcon" class="fa fa-sort-asc fa-2x mt-1 float-left" aria-hidden="true" style="position:relative;top:6px;color:limegreen"></span>
                     <span *ngIf="list.isAscIcon" class="ml-2" style="position:relative;top:6px;">{{list.usuallySpend}}</span>
                  </td>
               </tr>
            </tbody>
         </table>
      </div>
   </div>
</div>`
})
export class TopIncomeSourcesComponent {
  @Input()
  widgetsGridsterItemArray: any[] = [];
  public actionList: any[] = [
    { name: "Salary Transfer", thisMonth: 3453, usuallySpend: 123, isAscIcon: true },
    { name: "Cash Deposit", thisMonth: 3453, usuallySpend: 6123, isAscIcon: false },
    { name: "Profit Earned", thisMonth: 3453, usuallySpend: 6123, isAscIcon: false },
    { name: "Rebete", thisMonth: 3453, usuallySpend: 123, isAscIcon: true }
  ]
}

// Component Created for Spending Trends Widget--
@Component({
  selector: 'spendingTrends',
  template: `<div class="widget">
    <div class="widget">
    <div class="widget-header">
        <span class="widget-header-title"> Spending Trends </span>
    </div>
    <div class="widget-area position-relative width100">
          <div id="spendingTrendscontainer" class="p-3"></div>       
    </div>
</div>`
})
export class SpendindTrendsComponent {
  @Input()
  widgetsGridsterItemArray: any[] = [];
  public options4: any = {
    chart: {
      height: (9 / 16 * 100) + '%',
      backgroundColor: 'rgba(0,0,0,0)'
    },
    title: {
      text: ''
    },
    xAxis: {
      categories: ['Jan', 'Feb', 'Mar', 'Apr', 'May']
    },
    labels: {
      items: [{
        //html: 'How you have been spending',
        style: {
          left: '50px',
          top: '18px',
          color: ( // theme
            Highcharts.defaultOptions.title.style &&
            Highcharts.defaultOptions.title.style.color
          ) || 'black'
        }
      }]
    },
    series: [{
      type: 'column',
      name: 'Your Income',
      data: [1, 3, 4, 2, 5]
    }, {
      type: 'column',
      name: 'Your Spending',
      data: [2, 2, 1, 4, 1]
    }, {
      type: 'spline',
      name: '',
      data: [1.5, 2.5, 3, 1.5, 3],
      marker: {
        lineWidth: 2,
        lineColor: Highcharts.getOptions().colors[3],
        fillColor: 'white'
      }
    }]
  }
  ngAfterViewInit() {
    setTimeout(() => {
      Highcharts.chart('spendingTrendscontainer', this.options4);
    }, 10);
  }

  ngOnInit() {
    $(document).ready(function () {
      setTimeout(function () {
        window.dispatchEvent(new Event('resize'));
      }, 100);
    });
  }
}

export interface TransactionDetail {
  date: string;
  type: string;
  narration: string;
  fcy: string;
  currentRate: string;
  lyc: string;
}

// Component Created for Dynamic pier chart Widget--
@Component({
  selector: 'DynamicPieChartWidget',
  template: `<div class="widget">
    <div class="widget">
    <div class="widget-header">
        <span class="widget-header-title"> Analytics </span>
    </div>
    <div class="widget-area position-relative width100">       
      <div [attr.id]="barChartDivId" class="p-3"></div>      
    </div>
</div>`
})
export class DynamicPieChartWidgetComponent {
  @Input() piechartItem: any;
  widgetsGridsterItemArray: any[] = [];
  barChartDivId = '';

  public options4: any = {
    chart: {
      backgroundColor: 'rgba(0,0,0,0)',
      plotBorderWidth: null,
      plotShadow: false,
      type: 'pie',
      height: (9 / 16 * 100) + '%',
    },
    title: {
      text: ''
    },
    tooltip: {
      pointFormat: '{series.name}: <b>{point.percentage:.1f}%</b>'
    },
    accessibility: {
      point: {
        valueSuffix: '%'
      }
    },
    plotOptions: {
      pie: {
        allowPointSelect: true,
        cursor: 'pointer',
        dataLabels: {
          enabled: true,
          format: '<b>{point.name}</b>: {point.percentage:.1f} %'
        },
        showInLegend: false
      }
    },
    series: [{
      name: 'Percentage',
      colorByPoint: true,
      data: [
        {
          name: 'Cutomer Information',
          y: 11.84
        }, {
          name: 'Account Information',
          y: 10.85
        }, {
          name: 'Image',
          y: 4.67
        }, {
          name: 'Video',
          y: 4.18
        }, {
          name: 'News Alerts',
          y: 7.05
        }]
    }]
  }

  ngAfterViewInit() {
    if (this.piechartItem != undefined) {
      setTimeout(() => {
        Highcharts.chart(this.barChartDivId, this.options4);
      }, 10);
    }
  }

  ngOnInit() {
    if (this.piechartItem != undefined) {
      this.barChartDivId = "pieChartcontainer" + this.piechartItem.WidgetId;
      $(document).ready(function () {
        setTimeout(function () {
          window.dispatchEvent(new Event('resize'));
        }, 100);
      });
    }
  }

}

// Component Created for Dynamic line chart Widget--
@Component({
  selector: 'DynamicLineChartWidget',
  template: `<div class="widget">
    <div class="widget">
    <div class="widget-header">
        <span class="widget-header-title"> Your Saving Trends </span>
    </div>
    <div class="widget-area position-relative width100">
      <div class="text-right" style="font-size:20px"><span>+5.6%</span><span class="pl-3">+3.5%</span></div>
      <div [attr.id]="barChartDivId" class="p-3"></div>      
    </div>
</div>`
})
export class DynamicLineChartWidgetComponent {
  @Input() linechartItem: any;
  widgetsGridsterItemArray: any[] = [];
  barChartDivId = '';
  public options4: any = {
    chart: {
      height: (9 / 16 * 100) + '%',
      backgroundColor: 'rgba(0,0,0,0)'
    },
    title: {
      text: ''
    },
    xAxis: {
      categories: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul']
    },
    labels: {
      items: [{
        //html: 'How you have been spending',
        style: {
          left: '50px',
          top: '18px',
          color: ( // theme
            Highcharts.defaultOptions.title.style &&
            Highcharts.defaultOptions.title.style.color
          ) || 'black'
        }
      }]
    },
    series: [{
      // type: 'spline',
      name: '',
      data: [1.5, 2.5, 3, 1.5, 3, 2, 4],
      marker: {
        lineWidth: 1,
        lineColor: Highcharts.getOptions().colors[3],
        fillColor: 'white'
      }
    }]
  }

  ngAfterViewInit() {
    if (this.linechartItem != undefined) {
      setTimeout(() => {
        Highcharts.chart(this.barChartDivId, this.options4);
      }, 10);
    }
  }

  ngOnInit() {
    if (this.linechartItem != undefined) {
      this.barChartDivId = "lineGraphcontainer" + this.linechartItem.WidgetId;
      $(document).ready(function () {
        setTimeout(function () {
          window.dispatchEvent(new Event('resize'));
        }, 100);
      });
    }
  }

}

// Component Created for Dynamic Bar Chart Widget--
@Component({
  selector: 'DynamicBarChartWidget',
  template: `<div class="widget">
    <div class="widget">
    <div class="widget-header">
      <span class="widget-header-title"> Spending Trends </span>
    </div>
    <div class="widget-area position-relative width100">
        <div [attr.id]="barChartDivId" class="p-3"></div>       
    </div>
</div>`
})
export class DynamicBarChartWidgetComponent {
  @Input() barchartItem: any;

  barChartDivId = '';
  widgetsGridsterItemArray: any[] = [];
  public options4: any = {
    chart: {
      height: (9 / 16 * 100) + '%',
      backgroundColor: 'rgba(0,0,0,0)'
    },
    title: {
      text: ''
    },
    xAxis: {
      categories: ['Jan', 'Feb', 'Mar', 'Apr', 'May']
    },
    labels: {
      items: [{
        //html: 'How you have been spending',
        style: {
          left: '50px',
          top: '18px',
          color: ( // theme
            Highcharts.defaultOptions.title.style &&
            Highcharts.defaultOptions.title.style.color
          ) || 'black'
        }
      }]
    },
    series: [{
      type: 'column',
      name: 'Your Income',
      data: [1, 3, 4, 2, 5]
    }, {
      type: 'column',
      name: 'Your Spending',
      data: [2, 2, 1, 4, 1]
    }, {
      type: 'spline',
      name: '',
      data: [1.5, 2.5, 3, 1.5, 3],
      marker: {
        lineWidth: 2,
        lineColor: Highcharts.getOptions().colors[3],
        fillColor: 'white'
      }
    }]
  }
  ngAfterViewInit() {
    if (this.barchartItem != undefined) {
      setTimeout(() => {
        Highcharts.chart(this.barChartDivId, this.options4);
      }, 10);
    }
  }

  ngOnInit() {
    if (this.barchartItem != undefined) {
      this.barChartDivId = "barGraphcontainer" + this.barchartItem.WidgetId;
      $(document).ready(function () {
        setTimeout(function () {
          window.dispatchEvent(new Event('resize'));
        }, 100);
      });
    }
  }
}

// Component Created for Image Widget--
@Component({
  selector: 'html-widget',
  template: `<div class="widget">
    <div class="widget-header">
      <span class="widget-header-title"> Dynamic Html </span>
    </div>
    <div class="widget-area position-relative width100">
      <div class="widget-indicator-inner text-center pt-2">
        <i class="fa fa-lg fa-code" aria-hidden="true" style='font-size:18em;'></i>
      </div>
    </div>
  </div>`
})
export class DynamicHhtmlComponent {

  constructor() { }

  ngOnInit() {
  }

}

// Component Created for StaticHtml Widget--
@Component({
  selector: 'static-html-widget',
  template: `<div class="widget">
    <div class="widget-header">
      <span class="widget-header-title"> Static Html </span>
    </div>
    <div class="widget-area position-relative width100">
      <div class="widget-indicator-inner text-center pt-2">
        <i class="fa fa-lg fa-code" aria-hidden="true" style='font-size:18em;'></i>
      </div>
      <h1>hello world static</h1>
      <div [innerHTML]="WidgetHTML"></div>
    </div>
  </div>`
})
export class StaticHtmlComponent {
  public staticHtmlContent: string = "";
  WidgetHTML: string;
  // constructor(private addDashboardDesignerComponent: AddDashboardDesignerComponent) {
  //  }
  ngOnInit() {
    // debugger;
    // console.log(this.addDashboardDesignerComponent.staticHtmlContent);
    // console.log( this.addDashboardDesignerComponent.StaticConfigForm.value['staticHtml']);
    // this.WidgetHTML= this.addDashboardDesignerComponent.staticHtmlContent;
  }

  ngONDestroy() {
    // debugger;
    // console.log(this.addDashboardDesignerComponent.staticHtmlContent);
    // console.log( this.addDashboardDesignerComponent.StaticConfigForm.value['staticHtml']);
    // this.WidgetHTML= this.addDashboardDesignerComponent.staticHtmlContent;
  }
}

// Component Created for StaticHtml Widget--
@Component({
  selector: 'cs-agent-logo-widget',
  template: `<div class="widget">
    <div class="widget-header">
      <span class="widget-header-title"></span>
    </div>
    <div class="widget-area position-relative width100">
      <div class="widget-indicator-inner pt-2">
        <i class="fa fa-lg fa-code" aria-hidden="true" style='font-size:2em;'><p>I am CS Agent</p></i>
      </div>
    </div>
  </div>`
})
export class CSAgentLogoComponent {
}


//Component Created for StaticHtml Widget--
//@Component({
//  selector: 'corporate-saver-agent-message',
//  template: `<div  class='card border-0 widget'><div class='text-left' style='margin:0 1.5%;'><div class='mt-3 HomeLoanDetailDiv' style='font-size: 8pt; color: #4d4e4d !important;padding:0;font-weight: 700; font-family: \"Arial\";'>BDO Wealth Advisers (Pty) Ltd</div><div class='HomeLoanDetailDiv'  style='font-size: 8pt;margin-top:2px;padding:2px; font-family: "Arial";'>As a valued client, you are enjoying the benefits of Bonus Call, Should your weighted average balance (WAB) for the following month drop below 85% of your opening balance, you will earn interest at 3,80% pa.If your WAB meets or exceeds the 85% threshold, you will earn an additional 25 basis points, ie 4,05% interest pa.</div></div></div>`
//})
//export class CorporateSaverAgentMessageComponent {
//}
//@Component({
//  selector: 'corporate-saver-transaction',
//  template: `<div id='WidgetId' class='card border-0'><div class='card-body text-left'><div class='CS-header'>Call transaction details for the period 01-02-2022 to 28-02-2022 </div><div id='HomeLoan-0101'><div class='' style='/* padding-right: 1.2%; */'><table class='LoanTransactionTable1 CScustomTable'><thead><tr class='ht-30'><th class='w-12 text-center' style='border-right: 2px solid white;'>Date</th><th class='text-center' style='width: 25%;border-right: 2px solid white;'>Payment details</th><th style='width: 25%;border-right: 2px solid white;' class='text-center'>Description</th><th class='w-15 text-center' style='border-right: 2px solid white;'>Amount</th><th class='w-8 text-center' style='border-right: 2px solid white;'>Rate</th><th class='w-15 text-center' style='border:none;'>Capital Balance</th></tr></thead></table><div class='pt-0 overflow-auto'><table id='HomeLoanTransactionTable' class='LoanTransactionTable CScustomTable'><tbody style='font-family: "arial";'><tr class='ht-20 CorporateSaverTable'><td class='w-12 text-center'>01/02/2022</td><td class='text-left'  style='width: 25%'>CASH-SOLD FRIDGE</td><td class='text-left'  style='width: 25%'>Balance brought forward</td><td class='w-15 text-right'></td><td class='w-8 text-center'>2.30%</td><td class='w-15 text-center'>R100 602.32</td></tr><tr class='ht-20 CorporateSaverTable'><td class='w-12 text-center'>12/02/2022</td><td class='text-left'  style='width: 25%'></td><td class='text-left'  style='width: 25%'>Agent fee paid</td><td class='w-15 text-right'>-R65.96</td><td class='w-8 text-center'></td><td class='w-15 text-center'>R100 738.56</td></tr><tr class='ht-20 CorporateSaverTable'><td class='w-12 text-center'>27/02/2022</td><td class='text-left'  style='width: 25%'></td><td class='text-left'  style='width: 25%'>Comprop - Lezmin</td><td class='w-15 text-right'>R8 998.26</td><td class='w-8 text-center'></td><td class='w-15 text-center'>R100 738.56</td></tr></tbody></table></div></div></div></div></div>`
//})
//export class CorporateSaverTransactionComponent {
//}
//@Component({
//  selector: 'corporate-agent-details',
//  template: `<div class='widget'>
//    <div class='CS-header col-lg-12 col-sm-12' style='padding: 0 1.5%;'>Tax totals</div>
//        <div class="col-lg-12 col-sm-12 row" style="padding: 0 1.6% 0 1%;margin-left: 0;">
//          <div class="" style="background-color: #f3f3f3;width: 100%;margin-left: 0.5%;">
//          <h4 class="pl-1" style="text-align:left;">
//          <span class="NedbankHomeLoanTxt1">Tax totals (year to date)</span></h4>
//          </div>
//          <div class="col-lg-6 col-sm-6" style="padding-right:0;padding-left: 0.5%;/* padding-top: 0.25%; */">
//            <div id="WidgetId" class="card border-0" style="padding-top: 0.5%;">
//              <div class="text-left py-0" style="padding-right: 0;/* padding-left: 1.5%; */">
//                <div class="HomeLoanDetailDiv" style="width: 100%;">
//                <table class="CScustomTable" border="0" style="height: auto;"><tbody>
//                  <tr><td class="w-25" style="padding-bottom: 8px !important;">Interest</td>
//                  <td class="w-25 text-right pr-1" style=" padding-bottom: 8px !important">R1&nbsp;018.29</td></tr>
//                  <tr><td class="w-25" style="padding-bottom: 8px !important">VAT on fee</td>
//                  <td class="w-25 text-right pr-1" style="padding-bottom: 8px !important">R0.00</td></tr>
//                </tbody></table>
//              </div>
//            </div>
//          </div>
//        </div>
//        <div class="col-lg-6 col-sm-6" style="padding-left: .25%;padding-top: 0.25%;padding-right: 0;">
//        <div id="WidgetId" class="card border-0">
//        <div class="card-body text-left py-0" style="padding-left: 0;padding-right: 0;">
//        <div class="card-body-header"></div><div class="HomeLoanDetailDiv" style="width: 100%;min-height: 74px;">
//        <table class="CScustomTable" border="0" style="height: auto;"><tbody>
//          <tr><td class="w-25" style="padding-bottom: 8px !important;">Agent fee deducted</td>
//          <td class="w-25 text-right pr-1" style=" padding-bottom: 8px !important">R551.53</td></tr>
//          <tr><td class="w-25" style="padding-bottom: 8px !important;"></td>
//          <td class="w-25 text-right pr-1" style=" padding-bottom: 8px !important"> </td></tr></tbody></table>
//                </div>
//              </div>
//            </div>
//          </div>
//        </div>`})
//export class CorporateAgentDetailsComponent {
//}
//@Component({
//  selector: 'corporate-saver-client-details',
//  // tslint:disable-next-line:max-line-length
//  template: `<div class='card-body text-left widget'><div class='CS-header'>Corporate Saver statement</div><div class='col-lg-12 col-sm-12 row' style='padding: initial;margin: 0px 0px 0px 0px;padding-right: 0px;'><div class='col-lg-6 col-sm-6' style='padding-right: 0.25%;padding: 0px 3px 0px 0px;'><div id='WidgetId' class='card border-0'><div class='card-body text-left py-0' style='padding-right: 0;padding-left: 0px;'><div class='' style='background-color: #f3f3f3;width: 100%;'><h4 class='pl-1' style='line-height: 1.8 !important;margin-bottom: 1%;'><span class='NedbankHomeLoanTxt1'>Client details</span></h4></div><div class='HomeLoanDetailDiv' style='width: 100%;'><table class='CScustomTable mt-2' border='0' style='height: auto;'><tbody><tr><td class='w-25' style='font-weight: bold;padding-bottom: 8px !important;'>Account no</td><td class='w-25 text-right pr-1' style='font-weight: bold; padding-bottom: 8px !important'>9000082385</td></tr><tr><td class='w-25' style='padding-bottom: 8px !important'>Branch code</td><td class='w-25 text-right pr-1' style='padding-bottom: 8px !important'>198765</td></tr><tr><td class='w-25' style='padding-bottom: 8px !important'>Agent's profile:</td><td class='w-25 text-right pr-1' style='padding-bottom: 8px !important'>PRO315</td></tr><tr><td class='w-25' style='padding-bottom: 8px !important'>CIF no</td><td class='w-25 text-right pr-1' style='padding-bottom: 8px !important'>5786407</td></tr><tr><td class='w-25' style='padding-bottom: 8px !important'>Client code</td><td class='w-25 text-right pr-1' style='padding-bottom: 8px !important'>292598</td></tr><tr><td class='w-25' style='padding-bottom: 8px !important'>Relationship manager</td><td class='w-25 text-right pr-1' style='padding-bottom: 8px !important'>Umhlali Agencies CC</td></tr><tr><td class='w-25' style='padding-bottom: 8px !important'>VAT Calculation</td><td class='w-25 text-right pr-1' style='padding-bottom: 8px !important'>VAT inclusive</td></tr><tr><td class='w-25' style='padding-bottom: 8px !important'>Client VAT no</td><td class='w-25 text-right pr-1' style='padding-bottom: 8px !important'>Not provided</td></tr></tbody></table></div></div></div></div><div class='col-lg-6 col-sm-6' style='padding-left: .25%;padding-right: inherit;'><div id='WidgetId' class='card border-0'><div class='text-left py-0' style='padding-left: 0;'><div class='card-body-header'></div><div class='' style='background-color: #f3f3f3;width: 100%;'><h4 class='pl-1' style='line-height: 1.8 !important;margin-bottom: 1%;'><span class='NedbankHomeLoanTxt1'>Agent details</span></h4></div><div class='HomeLoanDetailDiv' style='width: 100%;'><table class='CScustomTable mt-2' border='0' style='height: auto;'><tbody><tr><td class='w-25' style='font-weight: bold;padding-bottom: 8px !important;'>Tax invoice no</td><td class='w-25 text-right pr-1' style='font-weight: bold; padding-bottom: 8px !important'>3563136</td></tr><tr><td class='w-25' style='padding-bottom: 8px !important'>Contact person</td><td class='w-25 text-right pr-1' style='padding-bottom: 8px !important'>Louise Taylor</td></tr><tr><td class='w-25' style='padding-bottom: 8px !important'>Email address</td><td class='w-25 text-right pr-1' style='padding-bottom: 8px !important'>louise@robchap.co.za</td></tr><tr><td class='w-25' style='padding-bottom: 8px !important'>Registration no</td><td class='w-25 text-right pr-1' style='padding-bottom: 8px !important'>1986/012848/23</td></tr><tr><td class='w-25' style='padding-bottom: 8px !important'>VAT registration no</td><td class='w-25 text-right pr-1' style='padding-bottom: 8px !important'>4900153653</td></tr><tr><td class='w-25' style='padding-bottom: 8px !important'>FSP license no</td><td class='w-25 text-right pr-1' style='padding-bottom: 8px !important'>16616</td></tr><tr><td class='w-25' style='padding-bottom: 8px !important'>Agent reference</td><td class='w-25 text-right pr-1' style='padding-bottom: 8px !important'>WALLI</td></tr><tr><td class='w-25' style='padding-bottom: 8px !important;font-weight: 700;color:#006341 !important;'>Statement No</td><td class='w-25 text-right pr-1' style='padding-bottom: 8px !important;font-weight: 700;color:#006341 !important;'>184</td></tr></tbody></table></div></div></div></div></div></div>`
//})
//export class CorporateSaverClientDetailsComponent {
//}
//@Component({
//  selector: 'corporate-saver-table-total',
//  template: `<div class="widget"><div class="CS-header col-lg-12 col-sm-12 mt-3" style="margin-left: 0.5%;">Investment portfolio at 28-02-2022</div><div id="WidgetId" class="card border-0" style="margin: 0 1.5%;"><div class="CSTotalAmountDetailsDiv" style="height: 40px !important; text-align: center; padding: 6px !important;"><span class="fnt-14pt">Current investment details
//  </span></div><table class="CScustomTable HomeLoanDetailDiv" border="0" style="height: auto;margin-bottom:2%;"><tbody><tr><td colspan="2" class="w-25" style="font-weight: bold;padding-bottom: 8px !important;padding-top: 8px !important;">Call account @ 2,30% per annum</td></tr><tr><td class="w-25" style="padding-bottom: 8px !important">Interest instruction</td><td class="w-25 text-right" style="padding-bottom: 8px !important;padding-right: 15px;">Capitalised</td><td class="w-25" style="padding-bottom: 8px !important">Date Invested</td><td class="w-25 text-right pr-1" style="padding-bottom: 8px !important">08-10-2006</td></tr><tr><td class="w-25" style="padding-bottom: 8px !important">Capital</td><td class="w-25 text-right" style="padding-bottom: 8px !important;padding-right: 15px;">R89&nbsp;030.44</td><td class="w-25" style="padding-bottom: 8px !important">Agent fee deducted</td><td class="w-25 text-right pr-1" style="padding-bottom: 8px !important">R59.17</td></tr><tr><td class="w-25" style="padding-bottom: 8px !important">Interest</td><td class="w-25 text-right" style="padding-bottom: 8px !important;padding-right: 15px;">R122.63</td><td class="w-25" style="padding-bottom: 8px !important">VAT on fee</td><td class="w-25 text-right pr-1" style="padding-bottom: 8px !important">R8.88</td></tr><tr><td class="w-25" style="padding-bottom: 8px !important">Agent fee structure</td><td class="w-25 text-right" style="padding-bottom: 8px !important;padding-right: 15px;">1.18%  on capital</td><td class="w-25" style="padding-bottom: 8px !important">Interest (less agent fee and VAT)</td><td class="w-25 text-right pr-1" style="padding-bottom: 8px !important">R8.88</td></tr></tbody></table><div class="d-flex flex-row" style="margin-top: -1.5%;"><div class="paymentDueHeaderBlock1 " style="font-weight: bold;margin-right:3px; margin-bottom:1px; ">Total capital</div><div class="paymentDueHeaderBlock1 " style="font-weight: bold;margin-right:3px; margin-bottom:1px; ">Total interest</div><div class="paymentDueHeaderBlock1 " style="font-weight: bold;margin-right:3px; margin-bottom:1px; ">Total agent fee <br>(deducted)</div><div class="paymentDueHeaderBlock1 " style="font-weight: bold;margin-right:3px; margin-bottom:1px; ">VAT on fee</div><div class="paymentDueHeaderBlock1" style="font-weight: bold;margin-bottom:1px">Interest<br>(less agent fee &amp; VAT)</div></div><div class="d-flex flex-row" style="margin-top: 2px !important;margin-bottom: 1%;"><div class="paymentDueHeaderBlock1 " style="margin-right:3px; margin-bottom:1px; ">R89&nbsp;030.44</div><div class="paymentDueHeaderBlock1 " style="margin-right:3px; margin-bottom:1px; ">R190.68</div><div class="paymentDueHeaderBlock1" style="margin-right:3px; margin-bottom:1px; ">R59.17</div><div class="paymentDueHeaderBlock1 " style="margin-right:3px; margin-bottom:1px; ">R8.88</div><div class="paymentDueHeaderBlock1" style="margin-bottom:1px; ">R122.63</div></div><div class="card border-0"><div class="card-body text-left" style="padding: 0;"><div class="card-body-header mt-3-2" style="font-family: &quot;Arial&quot;;font-weight: 700;">Important information</div> <div class="" style="font-size: 9pt; font-family: &quot;Arial&quot;;"><p>Interest(less agent administration fee and VAT) is credited to your account in March.The agent administration fee and VAT are deducted in March and paid on your behalf to your agent, in accordance with the mandate held.</p></div></div></div></div></div>`})
//export class CorporateSaverTableTotalComponent {
//}

// Component Created for PageBreak Widget--
@Component({
  selector: 'page-break-widget',
  template: `<div class="widget">
    <div class="widget-header">
      <span class="widget-header-title"> Page Break </span>
    </div>
    <div class="widget-area position-relative width100">
      <div class="widget-indicator-inner text-center pt-2">
        <div style="page-break-after:always">&nbsp;</div>
      </div>
    </div>
  </div>`
})
export class PageBreakComponent {

  constructor() { }

  ngOnInit() {

  }

}


// Component Created for SegmentBasedContent Widget--
@Component({
  selector: 'Segment-based-content-widget',
  template: `<div class="widget">
    <div class="widget-area position-relative width100">
    <div [innerHtml]="html">
                    </div>
      <!-- <div class="widget-indicator-inner text-center pt-2">
        <i class="fa fa-lg fa-id-card-o" aria-hidden="true" style='font-size:18em;'></i>
      </div> -->
    </div>
  </div>`
})
export class SegmentBasedContentComponent {

  @Input() SegmentBasedContentItem: any;

  public html: any;

  constructor(private _http: HttpClient,
    private sanitizer: DomSanitizer) {
  }

  ngOnInit() {
    if (this.SegmentBasedContentItem != null && this.SegmentBasedContentItem.WidgetSetting != null && this.SegmentBasedContentItem.WidgetSetting != '' && this.testJSON(this.SegmentBasedContentItem.WidgetSetting)) {
      let widgetSetting = JSON.parse(this.SegmentBasedContentItem.WidgetSetting);
      if (widgetSetting.length > 0) {
        this.html = this.sanitizer.bypassSecurityTrustHtml(widgetSetting[0].Html);
      }
      else {
        this.html = '';
      }
    }
    else {
      this.html = '';
    }
  }

  testJSON(text) {
    if (typeof text !== "string") {
      return false;
    }
    try {
      JSON.parse(text);
      return true;
    }
    catch (error) {
      return false;
    }
  }

  ngAfterViewInit() {
    setTimeout(() => {
      if (this.SegmentBasedContentItem != null && this.SegmentBasedContentItem.WidgetSetting != null && this.SegmentBasedContentItem.WidgetSetting != '' && this.testJSON(this.SegmentBasedContentItem.WidgetSetting)) {
        let widgetSetting = JSON.parse(this.SegmentBasedContentItem.WidgetSetting);
      }
    }, 100);
  }

}


/* ---  below highcharts components are created for page preview while designing..
    which are same as above highcharts widgets, but created due to facing issue in page preview in popup --- */

// Component Created for Saving Trends Widget for page preview while designing--
@Component({
  selector: 'savingTrendPreview',
  template: `<div class="widget">
    <div class="widget">
    <div class="widget-header">
        <span class="widget-header-title"> Your Saving Trends </span>
    </div>
    <div class="widget-area position-relative width100">
      <div class="text-right" style="font-size:20px"><span>+5.6%</span><span class="pl-3">+3.5%</span></div>
      <div id="savingTrendsPreviewContainer" class="p-3"></div>        
    </div>
</div>`
})
export class SavingTrendsPreviewComponent {
  @Input()
  widgetsGridsterItemArray: any[] = [];
  public options4: any = {
    chart: {
      height: (9 / 16 * 100) + '%',
      backgroundColor: 'rgba(0,0,0,0)'
    },
    title: {
      text: ''
    },
    xAxis: {
      categories: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul']
    },
    labels: {
      items: [{
        //html: 'How you have been spending',
        style: {
          left: '50px',
          top: '18px',
          color: ( // theme
            Highcharts.defaultOptions.title.style &&
            Highcharts.defaultOptions.title.style.color
          ) || 'black'
        }
      }]
    },
    series: [{
      // type: 'spline',
      name: '',
      data: [1.5, 2.5, 3, 1.5, 3, 2, 4],
      marker: {
        lineWidth: 1,
        lineColor: Highcharts.getOptions().colors[3],
        fillColor: 'white'
      }
    }]
  }
  ngAfterViewInit() {
    setTimeout(() => {
      Highcharts.chart('savingTrendsPreviewContainer', this.options4);
    }, 100);
  }

  ngOnInit() {
    $(document).ready(function () {
      setTimeout(function () {
        window.dispatchEvent(new Event('resize'));
      }, 10);
    });
  }
}

// Component Created for Spending Trends Widget for page preview while designing--
@Component({
  selector: 'spendingTrendsPreivew',
  template: `<div class="widget">
    <div class="widget">
    <div class="widget-header">
        <span class="widget-header-title"> Spending Trends </span>
    </div>
    <div class="widget-area position-relative width100">
          <div id="spendingTrendsPreviewcontainer" class="p-3"></div>       
    </div>
</div>`
})
export class SpendindTrendsPreviewComponent {
  @Input()
  widgetsGridsterItemArray: any[] = [];
  public options4: any = {
    chart: {
      height: (9 / 16 * 100) + '%',
      backgroundColor: 'rgba(0,0,0,0)'
    },
    title: {
      text: ''
    },
    xAxis: {
      categories: ['Jan', 'Feb', 'Mar', 'Apr', 'May']
    },
    labels: {
      items: [{
        //html: 'How you have been spending',
        style: {
          left: '50px',
          top: '18px',
          color: ( // theme
            Highcharts.defaultOptions.title.style &&
            Highcharts.defaultOptions.title.style.color
          ) || 'black'
        }
      }]
    },
    series: [{
      type: 'column',
      name: 'Your Income',
      data: [1, 3, 4, 2, 5]
    }, {
      type: 'column',
      name: 'Your Spending',
      data: [2, 2, 1, 4, 1]
    }, {
      type: 'spline',
      name: '',
      data: [1.5, 2.5, 3, 1.5, 3],
      marker: {
        lineWidth: 2,
        lineColor: Highcharts.getOptions().colors[3],
        fillColor: 'white'
      }
    }]
  }
  ngAfterViewInit() {
    setTimeout(() => {
      Highcharts.chart('spendingTrendsPreviewcontainer', this.options4);
    }, 10);
  }

  ngOnInit() {
    $(document).ready(function () {
      setTimeout(function () {
        window.dispatchEvent(new Event('resize'));
      }, 100);
    });
  }
}

// Component Created for Analytics Widget for page preview while designing--
@Component({
  selector: 'analyticsPreview',
  template: `<div class="widget">
    <div class="widget">
    <div class="widget-header">
        <span class="widget-header-title"> Analytics </span>
    </div>
    <div class="widget-area position-relative width100">       
        <div id="chartWidgetPiePreviewcontainer" class="p-3"></div>
    </div>
</div>`
})
export class AnalyticsWidgetPreviewComponent {
  @Input()
  widgetsGridsterItemArray: any[] = [];

  public options4: any = {
    chart: {
      backgroundColor: 'rgba(0,0,0,0)',
      plotBorderWidth: null,
      plotShadow: false,
      type: 'pie',
      height: (9 / 16 * 100) + '%',
    },
    title: {
      text: ''
    },
    tooltip: {
      pointFormat: '{series.name}: <b>{point.percentage:.1f}%</b>'
    },
    accessibility: {
      point: {
        valueSuffix: '%'
      }
    },
    plotOptions: {
      pie: {
        allowPointSelect: true,
        cursor: 'pointer',
        dataLabels: {
          enabled: true,
          format: '<b>{point.name}</b>: {point.percentage:.1f} %'
        },
        showInLegend: false
      }
    },
    series: [{
      name: 'Percentage',
      colorByPoint: true,
      data: [
        {
          name: 'Cutomer Information',
          y: 11.84
        }, {
          name: 'Account Information',
          y: 10.85
        }, {
          name: 'Image',
          y: 4.67
        }, {
          name: 'Video',
          y: 4.18
        }, {
          name: 'News Alerts',
          y: 7.05
        }]
    }]
  }

  ngAfterViewInit() {
    setTimeout(() => {
      Highcharts.chart('chartWidgetPiePreviewcontainer', this.options4);
    }, 100);
  }

  ngOnInit() {
    $(document).ready(function () {
      setTimeout(function () {
        window.dispatchEvent(new Event('resize'));
      }, 10);
    });
  }
}

// Component Created for Dynamic line chart Widget for page preview while designing ----
@Component({
  selector: 'DynamicLineChartWidgetPreview',
  template: `<div class="widget">
    <div class="widget">
    <div class="widget-header">
        <span class="widget-header-title"> Your Saving Trends </span>
    </div>
    <div class="widget-area position-relative width100">
      <div class="text-right" style="font-size:20px"><span>+5.6%</span><span class="pl-3">+3.5%</span></div>
      <div [attr.id]="barChartDivId" class="p-3"></div>          
    </div>
</div>`
})
export class DynamicLineChartWidgetPreviewComponent {
  @Input() linechartItem: any;
  widgetsGridsterItemArray: any[] = [];
  barChartDivId = '';
  public options4: any = {
    chart: {
      height: (9 / 16 * 100) + '%',
      backgroundColor: 'rgba(0,0,0,0)'
    },
    title: {
      text: ''
    },
    xAxis: {
      categories: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul']
    },
    labels: {
      items: [{
        //html: 'How you have been spending',
        style: {
          left: '50px',
          top: '18px',
          color: ( // theme
            Highcharts.defaultOptions.title.style &&
            Highcharts.defaultOptions.title.style.color
          ) || 'black'
        }
      }]
    },
    series: [{
      // type: 'spline',
      name: '',
      data: [1.5, 2.5, 3, 1.5, 3, 2, 4],
      marker: {
        lineWidth: 1,
        lineColor: Highcharts.getOptions().colors[3],
        fillColor: 'white'
      }
    }]
  }

  ngAfterViewInit() {
    if (this.linechartItem != undefined) {
      setTimeout(() => {
        Highcharts.chart(this.barChartDivId, this.options4);
      }, 10);
    }
  }

  ngOnInit() {
    if (this.linechartItem != undefined) {
      this.barChartDivId = "dynamiclinechartpreviewcontainer" + this.linechartItem.WidgetId;
      $(document).ready(function () {
        setTimeout(function () {
          window.dispatchEvent(new Event('resize'));
        }, 100);
      });
    }
  }

}

// Component Created for Dynamic Bar Chart Widget for page preview while designing ----
@Component({
  selector: 'DynamicBarChartWidgetPreview',
  template: `<div class="widget">
    <div class="widget">
    <div class="widget-header">
      <span class="widget-header-title"> Spending Trends </span>
    </div>
    <div class="widget-area position-relative width100">
      <div [attr.id]="barChartDivId" class="p-3"></div>
    </div>
</div>`
})
export class DynamicBarChartWidgetPreviewComponent {
  @Input() dynamicBarchartItem: any;
  widgetsGridsterItemArray: any[] = [];
  barChartDivId = '';

  public options4: any = {
    chart: {
      height: (9 / 16 * 100) + '%',
      backgroundColor: 'rgba(0,0,0,0)'
    },
    title: {
      text: ''
    },
    xAxis: {
      categories: ['Jan', 'Feb', 'Mar', 'Apr', 'May']
    },
    labels: {
      items: [{
        //html: 'How you have been spending',
        style: {
          left: '50px',
          top: '18px',
          color: ( // theme
            Highcharts.defaultOptions.title.style &&
            Highcharts.defaultOptions.title.style.color
          ) || 'black'
        }
      }]
    },
    series: [{
      type: 'column',
      name: 'Your Income',
      data: [1, 3, 4, 2, 5]
    }, {
      type: 'column',
      name: 'Your Spending',
      data: [2, 2, 1, 4, 1]
    }, {
      type: 'spline',
      name: '',
      data: [1.5, 2.5, 3, 1.5, 3],
      marker: {
        lineWidth: 2,
        lineColor: Highcharts.getOptions().colors[3],
        fillColor: 'white'
      }
    }]
  }

  ngAfterViewInit() {
    if (this.dynamicBarchartItem != undefined) {
      setTimeout(() => {
        Highcharts.chart(this.barChartDivId, this.options4);
      }, 10);
    }
  }

  ngOnInit() {
    if (this.dynamicBarchartItem != undefined) {
      this.barChartDivId = "dynamicbarchartpreviewcontainer" + this.dynamicBarchartItem.WidgetId;
      $(document).ready(function () {
        setTimeout(function () {
          window.dispatchEvent(new Event('resize'));
        }, 100);
      });
    }
  }

}

// Component Created for Dynamic pier chart Widget for page preview while designing --
@Component({
  selector: 'DynamicPieChartWidgetPreview',
  template: `<div class="widget">
    <div class="widget">
    <div class="widget-header">
        <span class="widget-header-title"> Analytics </span>
    </div>
    <div class="widget-area position-relative width100">       
      <div [attr.id]="barChartDivId" class="p-3"></div>  
    </div>
</div>`
})
export class DynamicPieChartWidgetPreviewComponent {
  @Input() piechartItem: any;
  widgetsGridsterItemArray: any[] = [];
  barChartDivId = '';

  public options4: any = {
    chart: {
      backgroundColor: 'rgba(0,0,0,0)',
      plotBorderWidth: null,
      plotShadow: false,
      type: 'pie',
      height: (9 / 16 * 100) + '%',
    },
    title: {
      text: ''
    },
    tooltip: {
      pointFormat: '{series.name}: <b>{point.percentage:.1f}%</b>'
    },
    accessibility: {
      point: {
        valueSuffix: '%'
      }
    },
    plotOptions: {
      pie: {
        allowPointSelect: true,
        cursor: 'pointer',
        dataLabels: {
          enabled: true,
          format: '<b>{point.name}</b>: {point.percentage:.1f} %'
        },
        showInLegend: false
      }
    },
    series: [{
      name: 'Percentage',
      colorByPoint: true,
      data: [
        {
          name: 'Cutomer Information',
          y: 11.84
        }, {
          name: 'Account Information',
          y: 10.85
        }, {
          name: 'Image',
          y: 4.67
        }, {
          name: 'Video',
          y: 4.18
        }, {
          name: 'News Alerts',
          y: 7.05
        }]
    }]
  }

  ngAfterViewInit() {
    if (this.piechartItem != undefined) {
      setTimeout(() => {
        Highcharts.chart(this.barChartDivId, this.options4);
      }, 10);
    }
  }

  ngOnInit() {
    if (this.piechartItem != undefined) {
      this.barChartDivId = "dynamicpiechartpreviewcontainer" + this.piechartItem.WidgetId;
      $(document).ready(function () {
        setTimeout(function () {
          window.dispatchEvent(new Event('resize'));
        }, 100);
      });
    }
  }

}

//// Component Created for Customer Details Widget -- Nedbank
//@Component({
//  selector: 'CustomerDetails',
//  template: `<div class="widget">
//    <div class="widget-area height100">
//       <div class='card border-0'>
//          <div class="card-body CustomerDetails">
//          MR KOENA SOLOMON MOLOTO <br>1917 THAGE STREET <br>MAMELODI GARDENS <br>PRETORIA <br>0122<br>
//          </div>
//      </div>
//    </div>
//  </div>`
//})
//export class CustomerDetailsComponent {
//  @Input()
//  widgetsGridsterItemArray: any[] = [];
//}
//@Component({
//  selector: 'corporate-saver-agent-address',
//  template: `<div class='widget'><div class='card border-0' style='padding-right: 2%;'><div class='card-body py-1' style='font-size: 8pt;text-align: right; font-family: "arial";'><br><br>Wanderers Office park<br>52 Corlett Drive<br>Illovo<br>2196<br>Address Line1<br><br><p style='color:rgb(0, 91, 65) !important;'>Agent contact details | 0331243213</p></div></div></div>`
//})
//export class CorporateSaverAgentAddressComponent {
//}

//// Component Created for Bank Details Widget -- Nedbank
//@Component({
//  selector: 'BankDetails',
//  template: `<div class="widget">
//    <div class="widget-area height100">
//      <div class='card border-0'>
//          <div class="card-body BranchDetails">
//              BankName<br>Address Line1, City, ZipCode<br>Address Line2, City, ZipCode<br>Country Name
//              <br>Bank VAT Reg No XXXXXXXXXX
//          </div>
//          <div class="ConactCenterDiv text-success float-right pt-3">
//              Nedbank Private Wealth Service Suite: XXXX XXX XXX
//          </div>
//      </div>
//    </div>
//  </div>`
//})
//export class BankDetailsComponent {
//  @Input()
//  widgetsGridsterItemArray: any[] = [];
//}

//// Component Created for Bank Details Widget -- Nedbank
//@Component({
//  selector: 'WealthBankDetails',
//  template: `<div class="widget">
//    <div class="widget-area height100">
//      <div class='card border-0'>
//          <div class="card-body BranchDetails">
//              BankName<br>Address Line1, City, ZipCode<br>Address Line2, City, ZipCode<br>Country Name
//              <br>Bank VAT Reg No XXXXXXXXXX
//          </div>
//          <div class="ConactCenterDiv text-success-w float-right pt-3">
//              Nedbank Private Wealth Service Suite: XXXX XXX XXX
//          </div>
//      </div>
//    </div>
//  </div>`
//})
//export class WealthBankDetailsComponent {
//  @Input()
//  widgetsGridsterItemArray: any[] = [];
//}


//// Component Created for Investment Portfolio Statement Widget -- Nedbank
//@Component({
//  selector: 'InvestmentPortfolioStatement',
//  template: `<div class="widget">
//    <div class="widget-area height100">
//      <div class="row">
//          <div class='col-lg-12'>
//              <div class='card border-0'>
//                  <div class='card-body text-left'>
//                      <div class="card-body-header-name pb-3">Dear Customer Name</div>
//                      <div class='card-body-header pb-2'>Investment portfolio statement</div>
//                      <div class='row pb-1'>
//                          <div class='col-lg-4 pr-1'>
//                              <div class='TotalAmountDetailsDiv'>
//                                  <span class='fnt-14'>Current investor balance</span><br><span class="fnt-20">Total Closing Balance</span>&nbsp;<br>
//                              </div>
//                          </div>
//                          <div class='col-lg-8 pl-0'>
//                              <div class='TotalAmountDetailsDiv'></div>
//                          </div>
//                      </div>

//                      <div class="pt-1 pb-2" style="background-color:#F2F2F2">
//                          <table class="customTable mt-2" border="0" id="portfolio">
//                              <tbody class="fnt-13">
//                                  <tr>
//                                      <td class="w-25">Account type:</td>
//                                      <td class="w-25 text-right pr-4 text-success">Investment</td>
//                                      <td class="w-25">Statement Day:</td>
//                                      <td class="w-25 text-right text-success">Day of Statement</td>
//                                  </tr>
//                                  <tr>
//                                      <td class="w-25">Investor no:</td>
//                                      <td class="w-25 text-right pr-4 text-success">Investor ID</td>
//                                      <td class="w-25">Statement Period:</td>
//                                      <td class="w-25 text-right text-success">Min to Max Transaction Date</td>
//                                  </tr>
//                                  <tr>
//                                      <td class="w-25">Statement date:</td>
//                                      <td class="w-25 text-right pr-4 text-success">Statement Date</td>
//                                  </tr>
//                              </tbody>
//                          </table>
//                      </div>
//                  </div>
//              </div>
//          </div>
//      </div>
//    </div>
//  </div>`
//})
//export class InvestmentPortfolioStatementComponent {
//  @Input()
//  widgetsGridsterItemArray: any[] = [];
//}

//// Component Created for Investment Wealth Portfolio Statement Widget -- Nedbank
//@Component({
//  selector: 'InvestmentWealthPortfolioStatementComponent',
//  template: `<div class="widget">
//    <div class="widget-area height100">
//      <div class="row">
//          <div class='col-lg-12'>
//              <div class='card border-0'>
//                  <div class='card-body text-left'>
//                      <div class="card-body-header-w pb-3">Dear Customer Name</div>
//                      <div class='card-body-header-w pb-2'>Investment portfolio statement</div>
//                      <div class='row pb-1'>
//                          <div class='col-lg-4 pr-1'>
//                              <div class='TotalAmountDetailsDivW'>
//                                  <span class='fnt-14'>Current investor balance</span><br><span class="fnt-20">Total Closing Balance</span>&nbsp;<br>
//                              </div>
//                          </div>
//                          <div class='col-lg-8 pl-0'>
//                              <div class='TotalAmountDetailsDivW'></div>
//                          </div>
//                      </div>

//                      <div class="pt-1 pb-2" style="background-color:#F2F2F2">
//                          <table class="customTable mt-2" border="0" id="portfolio">
//                              <tbody class="fnt-13">
//                                  <tr>
//                                      <td class="w-25">Account type:</td>
//                                      <td class="w-25 text-right pr-4 text-success">Investment</td>
//                                      <td class="w-25">Statement Day:</td>
//                                      <td class="w-25 text-right text-success">Day of Statement</td>
//                                  </tr>
//                                  <tr>
//                                      <td class="w-25">Investor no:</td>
//                                      <td class="w-25 text-right pr-4 text-success">Investor ID</td>
//                                      <td class="w-25">Statement Period:</td>
//                                      <td class="w-25 text-right text-success">Min to Max Transaction Date</td>
//                                  </tr>
//                                  <tr>
//                                      <td class="w-25">Statement date:</td>
//                                      <td class="w-25 text-right pr-4 text-success">Statement Date</td>
//                                  </tr>
//                              </tbody>
//                          </table>
//                      </div>
//                  </div>
//              </div>
//          </div>
//      </div>
//    </div>
//  </div>`
//})
//export class InvestmentWealthPortfolioStatementComponent {
//  @Input()
//  widgetsGridsterItemArray: any[] = [];
//}

//// Component Created for Investor Performance Widget -- Nedbank
//@Component({
//  selector: 'InvestorPerformance',
//  template: `<div class="widget">
//    <div class="widget-area height100">
//      <div class='card border-0'>
//          <div class='card-body text-left'>
//              <div class='card-body-header pb-2'>Investor performance</div>
//              <div class='InvestmentPermanaceDiv'>
//                  <table class='InvestorPermanaceTable' border='0' id='InvestorPerformance'>
//                      <tbody>
//                          <tr>
//                              <td class='w-50' colspan='2'><span class='text-success fnt-18'>Notice deposits</span></td>
//                          </tr>
//                          <tr>
//                              <td class='w-50 fnt-14'>Opening balance</td>
//                              <td class='w-50 fnt-14'>Closing balance</td>
//                          </tr>
//                          <tr>
//                              <td class='w-50 fnt-20'>xxx.xx</td>
//                              <td class='w-50 fnt-20'>xxx.xx</td>
//                          </tr>
//                      </tbody>
//                  </table>
//              </div>
//          </div>
//      </div>
//    </div>
//  </div>`
//})
//export class InvestorPerformanceComponent {
//  @Input()
//  widgetsGridsterItemArray: any[] = [];
//}

//// Component Created for Investor Performance Widget -- Nedbank
//@Component({
//  selector: 'WealthInvestorPerformance',
//  template: `<div class="widget">
//    <div class="widget-area height100">
//      <div class='card border-0'>
//          <div class='card-body text-left'>
//              <div class='card-body-header-w pb-2'>Investor performance</div>
//              <div class='InvestmentPermanaceDiv'>
//                  <table class='InvestorPermanaceTable' border='0' id='InvestorPerformance'>
//                      <tbody>
//                          <tr>
//                              <td class='w-50' colspan='2'><span class='text-success-w fnt-18'>Notice deposits</span></td>
//                          </tr>
//                          <tr>
//                              <td class='w-50 fnt-14'>Opening balance</td>
//                              <td class='w-50 fnt-14'>Closing balance</td>
//                          </tr>
//                          <tr>
//                              <td class='w-50 fnt-20'>xxx.xx</td>
//                              <td class='w-50 fnt-20'>xxx.xx</td>
//                          </tr>
//                      </tbody>
//                  </table>
//              </div>
//          </div>
//      </div>
//    </div>
//  </div>`
//})
//export class WealthInvestorPerformanceComponent {
//  @Input()
//  widgetsGridsterItemArray: any[] = [];
//}

//// Component Created for Breakdown Of Investment Accounts Widget -- Nedbank
//@Component({
//  selector: 'BreakdownOfInvestmentAccounts',
//  template: `<div class="widget">
//    <div class="widget-area height100">
//      <div class='card border-0'>
//          <div class='card-body text-left'>
//              <div class="card-body-header pb-2">Breakdown of your investment accounts</div>

//                  <div id='JustInvest-9929' class='tab-pane fade in active show'>
//                      <div style="background-color: #F2F2F2;padding:10px 0px">
//                          <h4 class="pl-25px"><span class='InvestmentProdDesc'>Product Desc</span></h4>
//                          <table border="0" class="InvestmentDetail customTable">
//                              <tbody>
//                                  <tr>
//                                      <td class="w-25">Investment no:</td>
//                                      <td class="text-right w-25"><span>Investor ID + Investoment ID</span></td>
//                                      <td class='w-25'>Opening date:</td>
//                                      <td class="text-right w-25"><span>Account Open Date</span></td>
//                                  </tr>
//                                  <tr>
//                                      <td class='w-25'>Current interest rate:</td>
//                                      <td class="text-right w-25"><span>Interest rate % pa</span></td>
//                                      <td class='w-25'>Maturity date:</td>
//                                      <td class="text-right w-25"><span>Maturity date</span></td>
//                                  </tr>
//                                  <tr>
//                                      <td class='w-25'>Interest disposal:</td>
//                                      <td class="text-right w-25"><span>Interest Disposal value</span></td>
//                                      <td class='w-25'>Notice period:</td>
//                                      <td class="text-right w-25" ><span>Notice Period value</span></td>
//                                  </tr>
//                                  <tr>
//                                      <td class='w-25'>Interest due:</td>
//                                      <td class="text-right w-25" ><span>Accured Interest</span></td>
//                                      <td class='w-25'>&nbsp;</td>
//                                      <td class="text-right w-25" >&nbsp;</td>
//                                  </tr>
//                              </tbody>
//                          </table>
//                          <div class="InvestmentClosingBalanceDiv">
//                              <span class='fn-14'>Balance at&nbsp;</span> <span class="text-success">Max Transaction date</span><br>
//                              <span class="text-success fnt-20">Total Closing Balance</span>
//                          </div>
//                      </div>

//                      <div class="pt-1">
//                          <table id="TableWidget" style="width:100%;" class="table-striped InvestmentBreakdown customTable">
//                              <thead>
//                                  <tr class='ht-30'>
//                                      <th class="w-15">Date</th>
//                                      <th class="w-40">Description</th>
//                                      <th class="w-15 text-right">Debit</th>
//                                      <th class="w-15 text-right">Credit</th>
//                                      <th class="w-15 text-right">Balance</th>
//                                  </tr>
//                              </thead>
//                              <tbody>
//                                  <tr>
//                                      <td class="w-15">DD/MM/YYYY</td>
//                                      <td class="w-40">Transaction Description</td>
//                                      <td class="w-15 text-right">-</td>
//                                      <td class="w-15 text-right">-</td>
//                                      <td class="w-15 text-right">xxx.xx</td>
//                                  </tr>
//                                  <tr>
//                                      <td class="w-15">DD/MM/YYYY</td>
//                                      <td class="w-40">Transaction Description</td>
//                                      <td class="w-15 text-right">-</td>
//                                      <td class="w-15 text-right">x.xx</td>
//                                      <td class="w-15 text-right">-</td>
//                                  </tr>
//                                  <tr>
//                                      <td class="w-15">DD/MM/YYYY</td>
//                                      <td class="w-40">Transaction Description</td>
//                                      <td class="w-15 text-right">-</td>
//                                      <td class="w-15 text-right">-</td>
//                                      <td class="w-15 text-right">xxx.x</td>
//                                  </tr>
//                                  <tr>
//                                      <td class="w-15">DD/MM/YYYY</td>
//                                      <td class="w-40">Transaction Description</td>
//                                      <td class="w-15 text-right">-</td>
//                                      <td class="w-15 text-right">x.xx</td>
//                                      <td class="w-15 text-right">-</td>
//                                  </tr>
//                                  <tr>
//                                      <td class="w-15">DD/MM/YYYY</td>
//                                      <td class="w-40">Transaction Description</td>
//                                      <td class="w-15 text-right">-</td>
//                                      <td class="w-15 text-right">-</td>
//                                      <td class="w-15 text-right">xxx.xx</td>
//                                  </tr>
//                                  <tr>
//                                      <td class="w-15">DD/MM/YYYY</td>
//                                      <td class="w-40">Transaction Description</td>
//                                      <td class="w-15 text-right">-</td>
//                                      <td class="w-15 text-right">x.xx</td>
//                                      <td class="w-15 text-right">-</td>
//                                  </tr>
//                                  <tr>
//                                      <td class="w-15">DD/MM/YYYY</td>
//                                      <td class="w-40">Transaction Description</td>
//                                      <td class="w-15 text-right">-</td>
//                                      <td class="w-15 text-right">-</td>
//                                      <td class="w-15 text-right">xxx.x</td>
//                                  </tr>
//                              </tbody>
//                          </table>
//                      </div>

//                      <br/><br/>

//                      <div style="background-color: #F2F2F2;padding:10px 0px">
//                          <h4 class="pl-25px"><span class="InvestmentProdDesc">Product Desc</span></h4>
//                          <table border="0" class="InvestmentDetail customTable">
//                              <tbody>
//                                  <tr>
//                                      <td class="w-25">Investment no:</td>
//                                      <td class="text-right w-25"><span>Investor ID with Investoment ID</span></td>
//                                      <td class='w-25'>Opening date:</td>
//                                      <td class="text-right w-25"><span>Acc Open Date</span></td>
//                                  </tr>
//                                  <tr>
//                                      <td class='w-25'>Current interest rate:</td>
//                                      <td class="text-right w-25"><span>Interest rate % pa</span></td>
//                                      <td class='w-25'>Maturity date:</td>
//                                      <td class="text-right w-25"><span>Maturity date</span></td>
//                                  </tr>
//                                  <tr>
//                                      <td class='w-25'>Interest disposal:</td>
//                                      <td class="text-right w-25"><span>Interest Disposal value</span></td>
//                                      <td class='w-25'>Notice period:</td>
//                                      <td class="text-right w-25"><span>Notice Period value</span></td>
//                                  </tr>
//                                  <tr>
//                                      <td class='w-25'>Interest due:</td>
//                                      <td class="text-right w-25"><span>Accured Interest</span></td>
//                                      <td class='w-25'>&nbsp;</td>
//                                      <td class="text-right w-25">&nbsp;</td>
//                                  </tr>
//                              </tbody>
//                          </table>
//                          <div class="InvestmentClosingBalanceDiv">
//                              <span class='fn-14'>Balance at&nbsp;</span> <span class="text-success">Max Transaction date</span><br>
//                              <span class="text-success fnt-20">Total Closing Balance</span>
//                          </div>
//                      </div>

//                      <div class="pt-1">
//                          <table id="TableWidget" style="width:100%;" class="table-striped InvestmentBreakdown customTable">
//                              <thead>
//                                  <tr class='ht-30'>
//                                      <th class="w-15">Date</th>
//                                      <th class="w-40">Description</th>
//                                      <th class="w-15 text-right">Debit</th>
//                                      <th class="w-15 text-right">Credit</th>
//                                      <th class="w-15 text-right">Balance</th>
//                                  </tr>
//                              </thead>
//                              <tbody>
//                                  <tr>
//                                      <td class="w-15">DD/MM/YYYY</td>
//                                      <td class="w-40">Transaction Description</td>
//                                      <td class="w-15 text-right">-</td>
//                                      <td class="w-15 text-right">-</td>
//                                      <td class="w-15 text-right">xxx.xx</td>
//                                  </tr>
//                                  <tr>
//                                      <td class="w-15">DD/MM/YYYY</td>
//                                      <td class="w-40">Transaction Description</td>
//                                      <td class="w-15 text-right">-</td>
//                                      <td class="w-15 text-right">x.xx</td>
//                                      <td class="w-15 text-right">-</td>
//                                  </tr>
//                                  <tr>
//                                      <td class="w-15">DD/MM/YYYY</td>
//                                      <td class="w-40">Transaction Description</td>
//                                      <td class="w-15 text-right">-</td>
//                                      <td class="w-15 text-right">-</td>
//                                      <td class="w-15 text-right">xxx.x</td>
//                                  </tr>
//                                  <tr>
//                                      <td class="w-15">DD/MM/YYYY</td>
//                                      <td class="w-40">Transaction Description</td>
//                                      <td class="w-15 text-right">-</td>
//                                      <td class="w-15 text-right">x.xx</td>
//                                      <td class="w-15 text-right">-</td>
//                                  </tr>
//                                  <tr>
//                                      <td class="w-15">DD/MM/YYYY</td>
//                                      <td class="w-40">Transaction Description</td>
//                                      <td class="w-15 text-right">-</td>
//                                      <td class="w-15 text-right">-</td>
//                                      <td class="w-15 text-right">xxx.xx</td>
//                                  </tr>
//                                  <tr>
//                                      <td class="w-15">DD/MM/YYYY</td>
//                                      <td class="w-40">Transaction Description</td>
//                                      <td class="w-15 text-right">-</td>
//                                      <td class="w-15 text-right">x.xx</td>
//                                      <td class="w-15 text-right">-</td>
//                                  </tr>
//                                  <tr>
//                                      <td class="w-15">DD/MM/YYYY</td>
//                                      <td class="w-40">Transaction Description</td>
//                                      <td class="w-15 text-right">-</td>
//                                      <td class="w-15 text-right">-</td>
//                                      <td class="w-15 text-right">xxx.x</td>
//                                  </tr>
//                              </tbody>
//                          </table>
//                      </div>

//              </div>

//          </div>
//      </div>
//    </div>
//  </div>`
//})
//export class BreakdownOfInvestmentAccountsComponent {
//  @Input()
//  widgetsGridsterItemArray: any[] = [];
//}

//// Component Created for Wealth Breakdown Of Investment Accounts Widget -- Nedbank
//@Component({
//  selector: 'WealthBreakdownOfInvestmentAccounts',
//  template: `<div class="widget">
//    <div class="widget-area height100">
//      <div class='card border-0'>
//          <div class='card-body text-left'>
//              <div class="card-body-header-w pb-2">Breakdown of your investment accounts</div>

//                  <div id='JustInvest-9929' class='tab-pane fade in active show'>
//                      <div style="background-color: #F2F2F2;padding:10px 0px">
//                          <h4 class="pl-25px"><span class='InvestmentProdDescW'>Product Desc</span></h4>
//                          <table border="0" class="InvestmentDetailW customTable">
//                              <tbody>
//                                  <tr>
//                                      <td class="w-25">Investment no:</td>
//                                      <td class="text-right w-25"><span>Investor ID + Investoment ID</span></td>
//                                      <td class='w-25'>Opening date:</td>
//                                      <td class="text-right w-25"><span>Account Open Date</span></td>
//                                  </tr>
//                                  <tr>
//                                      <td class='w-25'>Current interest rate:</td>
//                                      <td class="text-right w-25"><span>Interest rate % pa</span></td>
//                                      <td class='w-25'>Maturity date:</td>
//                                      <td class="text-right w-25"><span>Maturity date</span></td>
//                                  </tr>
//                                  <tr>
//                                      <td class='w-25'>Interest disposal:</td>
//                                      <td class="text-right w-25"><span>Interest Disposal value</span></td>
//                                      <td class='w-25'>Notice period:</td>
//                                      <td class="text-right w-25" ><span>Notice Period value</span></td>
//                                  </tr>
//                                  <tr>
//                                      <td class='w-25'>Interest due:</td>
//                                      <td class="text-right w-25" ><span>Accured Interest</span></td>
//                                      <td class='w-25'>&nbsp;</td>
//                                      <td class="text-right w-25" >&nbsp;</td>
//                                  </tr>
//                              </tbody>
//                          </table>
//                          <div class="InvestmentClosingBalanceDiv">
//                              <span class='fn-14'>Balance at&nbsp;</span> <span class="text-success-w">Max Transaction date</span><br>
//                              <span class="text-success-w fnt-20">Total Closing Balance</span>
//                          </div>
//                      </div>

//                      <div class="pt-1">
//                          <table id="TableWidget" style="width:100%;" class="table-striped InvestmentBreakdownW customTable">
//                              <thead>
//                                  <tr class='ht-30'>
//                                      <th class="w-15">Date</th>
//                                      <th class="w-40">Description</th>
//                                      <th class="w-15 text-right">Debit</th>
//                                      <th class="w-15 text-right">Credit</th>
//                                      <th class="w-15 text-right">Balance</th>
//                                  </tr>
//                              </thead>
//                              <tbody>
//                                  <tr>
//                                      <td class="w-15">DD/MM/YYYY</td>
//                                      <td class="w-40">Transaction Description</td>
//                                      <td class="w-15 text-right">-</td>
//                                      <td class="w-15 text-right">-</td>
//                                      <td class="w-15 text-right">xxx.xx</td>
//                                  </tr>
//                                  <tr>
//                                      <td class="w-15">DD/MM/YYYY</td>
//                                      <td class="w-40">Transaction Description</td>
//                                      <td class="w-15 text-right">-</td>
//                                      <td class="w-15 text-right">x.xx</td>
//                                      <td class="w-15 text-right">-</td>
//                                  </tr>
//                                  <tr>
//                                      <td class="w-15">DD/MM/YYYY</td>
//                                      <td class="w-40">Transaction Description</td>
//                                      <td class="w-15 text-right">-</td>
//                                      <td class="w-15 text-right">-</td>
//                                      <td class="w-15 text-right">xxx.x</td>
//                                  </tr>
//                                  <tr>
//                                      <td class="w-15">DD/MM/YYYY</td>
//                                      <td class="w-40">Transaction Description</td>
//                                      <td class="w-15 text-right">-</td>
//                                      <td class="w-15 text-right">x.xx</td>
//                                      <td class="w-15 text-right">-</td>
//                                  </tr>
//                                  <tr>
//                                      <td class="w-15">DD/MM/YYYY</td>
//                                      <td class="w-40">Transaction Description</td>
//                                      <td class="w-15 text-right">-</td>
//                                      <td class="w-15 text-right">-</td>
//                                      <td class="w-15 text-right">xxx.xx</td>
//                                  </tr>
//                                  <tr>
//                                      <td class="w-15">DD/MM/YYYY</td>
//                                      <td class="w-40">Transaction Description</td>
//                                      <td class="w-15 text-right">-</td>
//                                      <td class="w-15 text-right">x.xx</td>
//                                      <td class="w-15 text-right">-</td>
//                                  </tr>
//                                  <tr>
//                                      <td class="w-15">DD/MM/YYYY</td>
//                                      <td class="w-40">Transaction Description</td>
//                                      <td class="w-15 text-right">-</td>
//                                      <td class="w-15 text-right">-</td>
//                                      <td class="w-15 text-right">xxx.x</td>
//                                  </tr>
//                              </tbody>
//                          </table>
//                      </div>

//                      <br/><br/>

//                      <div style="background-color: #F2F2F2;padding:10px 0px">
//                          <h4 class="pl-25px"><span class="InvestmentProdDescW">Product Desc</span></h4>
//                          <table border="0" class="InvestmentDetailW customTable">
//                              <tbody>
//                                  <tr>
//                                      <td class="w-25">Investment no:</td>
//                                      <td class="text-right w-25"><span>Investor ID with Investoment ID</span></td>
//                                      <td class='w-25'>Opening date:</td>
//                                      <td class="text-right w-25"><span>Acc Open Date</span></td>
//                                  </tr>
//                                  <tr>
//                                      <td class='w-25'>Current interest rate:</td>
//                                      <td class="text-right w-25"><span>Interest rate % pa</span></td>
//                                      <td class='w-25'>Maturity date:</td>
//                                      <td class="text-right w-25"><span>Maturity date</span></td>
//                                  </tr>
//                                  <tr>
//                                      <td class='w-25'>Interest disposal:</td>
//                                      <td class="text-right w-25"><span>Interest Disposal value</span></td>
//                                      <td class='w-25'>Notice period:</td>
//                                      <td class="text-right w-25"><span>Notice Period value</span></td>
//                                  </tr>
//                                  <tr>
//                                      <td class='w-25'>Interest due:</td>
//                                      <td class="text-right w-25"><span>Accured Interest</span></td>
//                                      <td class='w-25'>&nbsp;</td>
//                                      <td class="text-right w-25">&nbsp;</td>
//                                  </tr>
//                              </tbody>
//                          </table>
//                          <div class="InvestmentClosingBalanceDiv">
//                              <span class='fn-14'>Balance at&nbsp;</span> <span class="text-success">Max Transaction date</span><br>
//                              <span class="text-success fnt-20">Total Closing Balance</span>
//                          </div>
//                      </div>

//                      <div class="pt-1">
//                          <table id="TableWidget" style="width:100%;" class="table-striped InvestmentBreakdownW customTable">
//                              <thead>
//                                  <tr class='ht-30'>
//                                      <th class="w-15">Date</th>
//                                      <th class="w-40">Description</th>
//                                      <th class="w-15 text-right">Debit</th>
//                                      <th class="w-15 text-right">Credit</th>
//                                      <th class="w-15 text-right">Balance</th>
//                                  </tr>
//                              </thead>
//                              <tbody>
//                                  <tr>
//                                      <td class="w-15">DD/MM/YYYY</td>
//                                      <td class="w-40">Transaction Description</td>
//                                      <td class="w-15 text-right">-</td>
//                                      <td class="w-15 text-right">-</td>
//                                      <td class="w-15 text-right">xxx.xx</td>
//                                  </tr>
//                                  <tr>
//                                      <td class="w-15">DD/MM/YYYY</td>
//                                      <td class="w-40">Transaction Description</td>
//                                      <td class="w-15 text-right">-</td>
//                                      <td class="w-15 text-right">x.xx</td>
//                                      <td class="w-15 text-right">-</td>
//                                  </tr>
//                                  <tr>
//                                      <td class="w-15">DD/MM/YYYY</td>
//                                      <td class="w-40">Transaction Description</td>
//                                      <td class="w-15 text-right">-</td>
//                                      <td class="w-15 text-right">-</td>
//                                      <td class="w-15 text-right">xxx.x</td>
//                                  </tr>
//                                  <tr>
//                                      <td class="w-15">DD/MM/YYYY</td>
//                                      <td class="w-40">Transaction Description</td>
//                                      <td class="w-15 text-right">-</td>
//                                      <td class="w-15 text-right">x.xx</td>
//                                      <td class="w-15 text-right">-</td>
//                                  </tr>
//                                  <tr>
//                                      <td class="w-15">DD/MM/YYYY</td>
//                                      <td class="w-40">Transaction Description</td>
//                                      <td class="w-15 text-right">-</td>
//                                      <td class="w-15 text-right">-</td>
//                                      <td class="w-15 text-right">xxx.xx</td>
//                                  </tr>
//                                  <tr>
//                                      <td class="w-15">DD/MM/YYYY</td>
//                                      <td class="w-40">Transaction Description</td>
//                                      <td class="w-15 text-right">-</td>
//                                      <td class="w-15 text-right">x.xx</td>
//                                      <td class="w-15 text-right">-</td>
//                                  </tr>
//                                  <tr>
//                                      <td class="w-15">DD/MM/YYYY</td>
//                                      <td class="w-40">Transaction Description</td>
//                                      <td class="w-15 text-right">-</td>
//                                      <td class="w-15 text-right">-</td>
//                                      <td class="w-15 text-right">xxx.x</td>
//                                  </tr>
//                              </tbody>
//                          </table>
//                      </div>

//              </div>

//          </div>
//      </div>
//    </div>
//  </div>`
//})
//export class WealthBreakdownOfInvestmentAccountsComponent {
//  @Input()
//  widgetsGridsterItemArray: any[] = [];
//}

//// Component Created for Explanatory Notes Widget -- Nedbank
//@Component({
//  selector: 'ExplanatoryNotes',
//  template: `<div class="widget">
//    <div class="widget-area height100">
//      <div class='card border-0'>
//          <div class='card-body text-left'>
//              <div class="card-body-header pb-2">Explanatory notes</div>
//              <div class='ExplanatoryNotes'>
//                  <span>Fixed deposits — Total balance of all your fixed-type accounts.</span><br/>
//                  <span>Notice deposits — Total balance of all your notice deposit accounts.</span><br/>
//                  <span>Linked deposits — Total balance of all your linked-type accounts.</span>
//              </div>
//          </div>
//      </div>
//    </div>
//  </div>`
//})
//export class ExplanatoryNotesComponent {
//  @Input()
//  widgetsGridsterItemArray: any[] = [];
//}

//// Component Created for Explanatory Notes Widget -- Nedbank
//@Component({
//  selector: 'WealthExplanatoryNotes',
//  template: `<div class="widget">
//    <div class="widget-area height100">
//      <div class='card border-0'>
//          <div class='card-body text-left'>
//              <div class="card-body-header-w pb-2">Explanatory notes</div>
//              <div class='ExplanatoryNotes'>
//                  <span>Fixed deposits — Total balance of all your fixed-type accounts.</span><br/>
//                  <span>Notice deposits — Total balance of all your notice deposit accounts.</span><br/>
//                  <span>Linked deposits — Total balance of all your linked-type accounts.</span>
//              </div>
//          </div>
//      </div>
//    </div>
//  </div>`
//})
//export class WealthExplanatoryNotesComponent {
//  @Input()
//  widgetsGridsterItemArray: any[] = [];
//}

//// Component Created for Nedbank service Widget -- Nedbank
//@Component({
//  selector: 'NedbankService',
//  template: `<div class="widget">
//    <div class="widget-area height100">
//      <div class="card border-0">
//          <div class="card-body text-left">
//              <div class="ServicesDiv">
//                  <div class="serviceHeader pb-2">Nedbank service message header</div>
//                  <span>Nedbank service message text</span><br>
//                  <span>Nedbank service message text</span><br>
//                  <span>Nedbank service message text</span>
//              </div>
//          </div>
//      </div>
//    </div>
//  </div>`
//})
//export class NedbankServiceComponent {
//  @Input()
//  widgetsGridsterItemArray: any[] = [];
//}

// Component Created for Nedbank service Widget -- Nedbank
//@Component({
//  selector: 'WealthNedbankService',
//  template: `<div class="widget">
//    <div class="widget-area height100">
//      <div class="card border-0">
//          <div class="card-body text-left">
//          <div class='card-body-header-w pb-2'>Nedbank services</div>

//          </div>
//      </div>
//    </div>
//  </div>`
//})

//export class WealthNedbankServiceComponent {
//  @Input()
//  widgetsGridsterItemArray: any[] = [];
//}

//// Component Created for Personal loan detail Widget -- Nedbank
//@Component({
//  selector: 'PersonalLoanDetail',
//  template: `<div class="widget">
//    <div class="widget-area height100">
//      <div class='card border-0'>
//          <div class='card-body text-left pb-1 pt-1'>
//              <div class='card-body-header pb-2'>Personal loan statement</div>
//              <div class='row pb-1'>
//                  <div class='col-lg-4 col-sm-4 pr-1'>
//                      <div class='LoanAmountDetailsDiv'>
//                          <span class="fnt-10pt">Loan Amount</span><br><span class="fnt-14pt">Loan Amount</span>&nbsp;<br>
//                      </div>
//                  </div>
//                  <div class='col-lg-4 col-sm-4 pr-1 pl-0'>
//                      <div class='LoanAmountDetailsDiv'>
//                          <span class="fnt-10pt">Balance outstanding</span><br><span class="fnt-14pt">Outstanding balance</span>&nbsp;<br>
//                      </div>
//                  </div>
//                  <div class='col-lg-4 col-sm-4 pl-0'>
//                      <div class='LoanAmountDetailsDiv'>
//                          <span class="fnt-10pt">Now due</span><br><span class="fnt-14pt">Now Due value</span>&nbsp;<br>
//                      </div>
//                  </div>
//              </div>

//              <div class="pt-2 pb-2" style="background-color:#F2F2F2">
//                  <h4 class="pl-25px"><span class="NedbankPersonalLoanTxt">Nedbank personal loan</span></h4>
//                  <table class="customTable mt-2" border="0">
//                      <tbody>
//                          <tr>
//                              <td class="w-25">Account Number:</td>
//                              <td class="w-25 text-right pr-4 text-success">Account Number</td>
//                              <td class="w-25"></td>
//                              <td class="w-25 text-right text-success"></td>
//                          </tr>
//                          <tr>
//                              <td class="w-25">Statement date:</td>
//                              <td class="w-25 text-right pr-4 text-success">Statement date</td>
//                              <td class="w-25">Arrears:</td>
//                              <td class="w-25 text-right text-success">Arrears amount</td>
//                          </tr>
//                          <tr>
//                              <td class="w-25">Statement period:</td>
//                              <td class="w-25 text-right pr-4 text-success">Min to Max Transaction Date</td>
//                              <td class="w-25">Annual rate of interest:</td>
//                              <td class="w-25 text-right pr-4 text-success">annual rate value</td>
//                          </tr>
//                          <tr>
//                              <td class="w-25">Monthly instalment:</td>
//                              <td class="w-25 text-right pr-4 text-success">Monthly instalment amount</td>
//                              <td class="w-25">Original term (months):</td>
//                              <td class="w-25 text-right pr-4 text-success">Month value</td>
//                          </tr>
//                          <tr>
//                              <td class="w-25">Due by date:</td>
//                              <td class="w-25 text-right pr-4 text-success">DD/MM/YYYY</td>
//                              <td class="w-25"></td>
//                              <td class="w-25 text-right pr-4 text-success"></td>
//                          </tr>
//                      </tbody>
//                  </table>
//              </div>
//          </div>
//      </div>
//    </div>
//  </div>`
//})
//export class PersonalLoanDetailComponent {
//  @Input()
//  widgetsGridsterItemArray: any[] = [];
//}

//// Component Created for Personal loan transaction Widget -- Nedbank
//@Component({
//  selector: 'PersonalLoanTransaction',
//  template: `<div class="widget">
//    <div class="widget-area height100">
//      <div class='card border-0'>
//          <div class='card-body text-left pb-1 pt-1'>
//              <div class="pt-1">
//                  <table id="TableWidget" class="LoanTransactionTable customTable">
//                      <thead>
//                          <tr class="ht-30">
//                              <th class="w-12">Post date</th>
//                              <th class="w-12">Effective date</th>
//                              <th class="w-40">Transaction</th>
//                              <th class="w-12 text-right">Debit</th>
//                              <th class="w-12 text-right">Credit</th>
//                              <th class="w-12 text-right">Balance outstanding</th>
//                          </tr>
//                      </thead>
//                  </table>

//                  <div class="pt-0 overflow-auto" style="max-height:125px;">
//                      <table class="LoanTransactionTable customTable">
//                          <tbody>
//                              <tr>
//                                  <td class="w-12">DD/MM/YYYY</td>
//                                  <td class="w-12">DD/MM/YYYY</td>
//                                  <td class="w-40">Transaction Description</td>
//                                  <td class="w-12 text-right">xxx.x</td>
//                                  <td class="w-12 text-right">-</td>
//                                  <td class="w-12 text-right">xxx.xx</td>
//                              </tr>
//                              <tr>
//                                  <td class="w-12">DD/MM/YYYY</td>
//                                  <td class="w-12">DD/MM/YYYY</td>
//                                  <td class="w-40">Transaction Description</td>
//                                  <td class="w-12 text-right">-</td>
//                                  <td class="w-12 text-right">x.xx</td>
//                                  <td class="w-12 text-right">xxx.xx</td>
//                              </tr>
//                              <tr>
//                                  <td class="w-12">DD/MM/YYYY</td>
//                                  <td class="w-12">DD/MM/YYYY</td>
//                                  <td class="w-40">Transaction Description</td>
//                                  <td class="w-12 text-right">-</td>
//                                  <td class="w-12 text-right">xxx.xx</td>
//                                  <td class="w-12 text-right">xxx.xx</td>
//                              </tr>
//                              <tr>
//                                  <td class="w-12">DD/MM/YYYY</td>
//                                  <td class="w-12">DD/MM/YYYY</td>
//                                  <td class="w-40">Transaction Description</td>
//                                  <td class="w-12 text-right">-</td>
//                                  <td class="w-12 text-right">x.xx</td>
//                                  <td class="w-12 text-right">xxx.xx</td>
//                              </tr>
//                              <tr>
//                                  <td class="w-12">DD/MM/YYYY</td>
//                                  <td class="w-12">DD/MM/YYYY</td>
//                                  <td class="w-40">Transaction Description</td>
//                                  <td class="w-12 text-right">xxx.xx</td>
//                                  <td class="w-12 text-right">-</td>
//                                  <td class="w-12 text-right">xxx.xx</td>
//                              </tr>
//                              <tr>
//                                  <td class="w-12">DD/MM/YYYY</td>
//                                  <td class="w-12">DD/MM/YYYY</td>
//                                  <td class="w-40">Transaction Description</td>
//                                  <td class="w-12 text-right">-</td>
//                                  <td class="w-12 text-right">xx.xx</td>
//                                  <td class="w-12 text-right">xxx.xx</td>
//                              </tr>
//                              <tr>
//                                  <td class="w-12">DD/MM/YYYY</td>
//                                  <td class="w-12">DD/MM/YYYY</td>
//                                  <td class="w-40">Transaction Description</td>
//                                  <td class="w-12 text-right">xx.xx</td>
//                                  <td class="w-12 text-right">-</td>
//                                  <td class="w-12 text-right">xxx.xx</td>
//                              </tr>
//                          </tbody>
//                      </table>
//                  </div>
//              </div>
//          </div>
//      </div>
//    </div>
//  </div>`
//})
//export class PersonalLoanTransactionComponent {
//  @Input()
//  widgetsGridsterItemArray: any[] = [];
//}

//// Component Created for Personal loan payment due details Widget -- Nedbank
//@Component({
//  selector: 'PersonalLoanPaymentDue',
//  template: `<div class="widget">
//    <div class="widget-area height100">
//      <div class='card border-0'>
//          <div class='card-body text-left pb-1 pt-1'>
//              <div class='card-body-sub-header pb-2'>Payment Due</div>
//              <div class="d-flex flex-row">
//                  <div class="paymentDueHeaderBlock mr-1">After 120 + days</div>
//                  <div class="paymentDueHeaderBlock mr-1">After 90 days</div>
//                  <div class="paymentDueHeaderBlock mr-1">After 60 days</div>
//                  <div class="paymentDueHeaderBlock mr-1">After 30 days</div>
//                  <div class="paymentDueHeaderBlock">Current</div>
//              </div>
//              <div class="d-flex flex-row mt-1">
//                  <div class="paymentDueFooterBlock mr-1">x.xx</div>
//                  <div class="paymentDueFooterBlock mr-1">x.xx</div>
//                  <div class="paymentDueFooterBlock mr-1">x.xx</div>
//                  <div class="paymentDueFooterBlock mr-1">x.xx</div>
//                  <div class="paymentDueFooterBlock">x.xx</div>
//              </div>
//          </div>
//      </div>
//    </div>
//  </div>`
//})
//export class PersonalLoanPaymentDueComponent {
//  @Input()
//  widgetsGridsterItemArray: any[] = [];
//}

// Component Created for special message Widget -- Nedbank
@Component({
  selector: 'SpecialMessage',
  template: `<div class="widget">
    <div class="widget-area height100">
      <div class="card border-0">
          <div class="card-body text-left pb-1 pt-1">
              <div class="SpecialMessageDiv">
                  <div class="SpecialMessageHeader">Special message header Text</div>
                  <p>Personalize special message text 1</p>
                  <p>Personalize special message text 2</p>
              </div>
          </div>
      </div>
    </div>
  </div>`
})
export class SpecialMessageComponent {
  @Input()
  widgetsGridsterItemArray: any[] = [];
}

// Component Created for Personal loan Insurance message Widget -- Nedbank
@Component({
  selector: 'PL_Insurance',
  template: `<div class="widget">
    <div class="widget-area height100">
      <div class="card border-0">
          <div class="card-body text-left py-4">
            <div class="InsuranceMessageDiv">
                <div class="card-body-header pb-2">Insurance</div>
                <p>Insurance related Personalize special message text 1</p>
                <p>Insurance related Personalize special message text 2</p>
                <p>Insurance related Personalize special message text 3</p>
            </div>
          </div>
      </div>
    </div>
  </div>`
})
export class PersonalLoanInsuranceMessageComponent {
  @Input()
  widgetsGridsterItemArray: any[] = [];
}

//// Component Created for Personal loan total amount detail Widget -- Nedbank
//@Component({
//  selector: 'PersonalLoanTotalAmountDetail',
//  template: `<div class="widget">
//    <div class="widget-area height100">
//      <div class='card border-0'>
//          <div class='card-body text-left pb-1 pt-4'>
//              <div class='card-body-header pb-2'>Personal loan statement</div>
//              <div class='row pb-1'>
//                  <div class='col-lg-4 col-sm-4 pr-1'>
//                      <div class='LoanAmountDetailsDiv'>
//                          <span class="fnt-10pt">Loan Amount</span><br><span class="fnt-14pt">R121 765.00</span><br>
//                      </div>
//                  </div>
//                  <div class='col-lg-4 col-sm-4 pr-1 pl-0'>
//                      <div class='LoanAmountDetailsDiv'>
//                          <span class="fnt-10pt">Balance outstanding</span><br><span class="fnt-14pt">R114 190.00</span><br>
//                      </div>
//                  </div>
//                  <div class='col-lg-4 col-sm-4 pl-0'>
//                      <div class='LoanAmountDetailsDiv'>
//                          <span class="fnt-10pt">Now due</span><br><span class="fnt-14pt">R4 742.00</span><br>
//                      </div>
//                  </div>
//              </div>
//          </div>
//      </div>
//    </div>
//  </div>`
//})
//export class PersonalLoanTotalAmountDetailComponent {
//  @Input()
//  widgetsGridsterItemArray: any[] = [];
//}

// Component Created for Personal loan total amount detail Widget -- Nedbank
//@Component({
//  selector: 'PersonalLoanAccountsBreakdown',
//  template: `<div class="widget">
//    <div class="widget-area height100">
//      <div class='card border-0'>
//        <div class='card-body text-left py-0'>
//            <div class="tab-content">
//                <div id="PersonalLoan-4001" class="tab-pane fade in active show">
//                    <div class="PersonalLoanDetailDiv">
//                        <h4 class="pl-25px"><span class="NedbankPersonalLoanTxt">Nedbank personal loan</span></h4>
//                        <table class="customTable mt-2" border="0">
//                            <tbody>
//                                <tr>
//                                    <td class="w-25">Account Number:</td>
//                                    <td class="w-25 text-right pr-4 text-success">8004334234001</td>
//                                    <td class="w-25"></td>
//                                    <td class="w-25 text-right text-success"></td>
//                                </tr>
//                                <tr>
//                                    <td class="w-25">Statement date:</td>
//                                    <td class="w-25 text-right pr-4 text-success">30/01/2021</td>
//                                    <td class="w-25">Arrears:</td>
//                                    <td class="w-25 text-right text-success">R0.00</td>
//                                </tr>
//                                <tr>
//                                    <td class="w-25">Statement period:</td>
//                                    <td class="w-25 text-right pr-4 text-success">01/12/2020 - 30/01/2021</td>
//                                    <td class="w-25">Annual rate of interest:</td>
//                                    <td class="w-25 text-right pr-4 text-success">24% pa</td>
//                                </tr>
//                                <tr>
//                                    <td class="w-25">Monthly instalment:</td>
//                                    <td class="w-25 text-right pr-4 text-success">R3 297.00</td>
//                                    <td class="w-25">Original term (months):</td>
//                                    <td class="w-25 text-right pr-4 text-success">36</td>
//                                </tr>
//                                <tr>
//                                    <td class="w-25">Due by date:</td>
//                                    <td class="w-25 text-right pr-4 text-success">28/02/2021</td>
//                                    <td class="w-25"></td>
//                                    <td class="w-25 text-right pr-4 text-success"></td>
//                                </tr>
//                            </tbody>
//                        </table>
//                    </div>

//                    <div class="pt-1">
//                        <table id="TableWidget" class="LoanTransactionTable customTable">
//                            <thead>
//                                <tr class="ht-30">
//                                    <th class="w-12 text-center">Post date</th>
//                                    <th class="w-12 text-center">Effective date</th>
//                                    <th class="w-40">Transaction</th>
//                                    <th class="w-12 text-right">Debit</th>
//                                    <th class="w-12 text-right">Credit</th>
//                                    <th class="w-12 text-right">Balance outstanding</th>
//                                </tr>
//                            </thead>
//                        </table>
//                        <div class="pt-0 overflow-auto" style="max-height:200px;">
//                            <table class="LoanTransactionTable customTable">
//                                <tbody>
//                                    <tr>
//                                        <td class="w-12 text-center">01/12/2020</td>
//                                        <td class="w-12 text-center">01/12/2020</td>
//                                        <td class="w-40">Interest Debit</td>
//                                        <td class="w-12 text-right">R1 490.00</td>
//                                        <td class="w-12 text-right">-</td>
//                                        <td class="w-12 text-right">R74 298.00</td>
//                                    </tr>
//                                    <tr>
//                                        <td class="w-12 text-center">03/12/2020</td>
//                                        <td class="w-12 text-center">03/12/2020</td>
//                                        <td class="w-40">Insurance Premium</td>
//                                        <td class="w-12 text-right">R188.00</td>
//                                        <td class="w-12 text-right">-</td>
//                                        <td class="w-12 text-right">R74 487.00</td>
//                                    </tr>
//                                    <tr>
//                                        <td class="w-12 text-center">15/12/2020</td>
//                                        <td class="w-12 text-center">15/12/2020</td>
//                                        <td class="w-40">Payment - Thank you</td>
//                                        <td class="w-12 text-right">-</td>
//                                        <td class="w-12 text-right">R3 297.00</td>
//                                        <td class="w-12 text-right">R71 189.00</td>
//                                    </tr>
//                                    <tr>
//                                        <td class="w-12 text-center">31/12/2020</td>
//                                        <td class="w-12 text-center">31/12/2020</td>
//                                        <td class="w-40">Monthly Admin Fee</td>
//                                        <td class="w-12 text-right">R69.00</td>
//                                        <td class="w-12 text-right">-</td>
//                                        <td class="w-12 text-right">R71 258.00</td>
//                                    </tr>
//                                    <tr>
//                                        <td class="w-12 text-center">01/01/2021</td>
//                                        <td class="w-12 text-center">01/01/2021</td>
//                                        <td class="w-40">Interest Debit</td>
//                                        <td class="w-12 text-right">R1 512.00</td>
//                                        <td class="w-12 text-right">-</td>
//                                        <td class="w-12 text-right">R72 770.00</td>
//                                    </tr>
//                                </tbody>
//                            </table>
//                        </div>
//                    </div>

//                    <div class="py-2">
//                        <div class='payment-due-header'>Payment Due</div>
//                        <div class="d-flex flex-row">
//                            <div class="paymentDueHeaderBlock mr-1">After 120 + days</div>
//                            <div class="paymentDueHeaderBlock mr-1">After 90 days</div>
//                            <div class="paymentDueHeaderBlock mr-1">After 60 days</div>
//                            <div class="paymentDueHeaderBlock mr-1">After 30 days</div>
//                            <div class="paymentDueHeaderBlock">Current</div>
//                        </div>
//                        <div class="d-flex flex-row mt-1">
//                            <div class="paymentDueFooterBlock mr-1">R0.00</div>
//                            <div class="paymentDueFooterBlock mr-1">R0.00</div>
//                            <div class="paymentDueFooterBlock mr-1">R0.00</div>
//                            <div class="paymentDueFooterBlock mr-1">R0.00</div>
//                            <div class="paymentDueFooterBlock">R3 297.00</div>
//                        </div>
//                    </div>

//                </div>

//                <div id="PersonalLoan-6001" class="tab-pane fade">
//                    <div class="PersonalLoanDetailDiv">
//                        <h4 class="pl-25px"><span class="NedbankPersonalLoanTxt">Nedbank personal loan</span></h4>
//                        <table class="customTable mt-2" border="0">
//                            <tbody>
//                                <tr>
//                                    <td class="w-25">Account Number:</td>
//                                    <td class="w-25 text-right pr-4 text-success">8003922986001</td>
//                                    <td class="w-25"></td>
//                                    <td class="w-25 text-right text-success"></td>
//                                </tr>
//                                <tr>
//                                    <td class="w-25">Statement date:</td>
//                                    <td class="w-25 text-right pr-4 text-success">30/01/2021</td>
//                                    <td class="w-25">Arrears:</td>
//                                    <td class="w-25 text-right text-success">R774.00</td>
//                                </tr>
//                                <tr>
//                                    <td class="w-25">Statement period:</td>
//                                    <td class="w-25 text-right pr-4 text-success">01/12/2020 - 30/01/2021</td>
//                                    <td class="w-25">Annual rate of interest:</td>
//                                    <td class="w-25 text-right pr-4 text-success">0% pa</td>
//                                </tr>
//                                <tr>
//                                    <td class="w-25">Monthly instalment:</td>
//                                    <td class="w-25 text-right pr-4 text-success">R671.00</td>
//                                    <td class="w-25">Original term (months):</td>
//                                    <td class="w-25 text-right pr-4 text-success">60</td>
//                                </tr>
//                                <tr>
//                                    <td class="w-25">Due by date:</td>
//                                    <td class="w-25 text-right pr-4 text-success">28/02/2021</td>
//                                    <td class="w-25"></td>
//                                    <td class="w-25 text-right pr-4 text-success"></td>
//                                </tr>
//                            </tbody>
//                        </table>
//                    </div>

//                    <div class="pt-1">
//                        <table id="TableWidget" class="LoanTransactionTable customTable">
//                            <thead>
//                                <tr class="ht-30">
//                                    <th class="w-12 text-center">Post date</th>
//                                    <th class="w-12 text-center">Effective date</th>
//                                    <th class="w-40">Transaction</th>
//                                    <th class="w-12 text-right">Debit</th>
//                                    <th class="w-12 text-right">Credit</th>
//                                    <th class="w-12 text-right">Balance outstanding</th>
//                                </tr>
//                            </thead>
//                        </table>
//                        <div class="pt-0 overflow-auto" style="max-height:200px;">
//                            <table class="LoanTransactionTable customTable">
//                                <tbody>
//                                    <tr>
//                                        <td class="w-12 text-center">01/12/2020</td>
//                                        <td class="w-12 text-center">01/12/2020</td>
//                                        <td class="w-40">Payment - Thank you</td>
//                                        <td class="w-12 text-right">-</td>
//                                        <td class="w-12 text-right">R671.00</td>
//                                        <td class="w-12 text-right">R47 363.00</td>
//                                    </tr>
//                                    <tr>
//                                        <td class="w-12 text-center">24/12/2020</td>
//                                        <td class="w-12 text-center">24/12/2020</td>
//                                        <td class="w-40">Payment - Thank you</td>
//                                        <td class="w-12 text-right">-</td>
//                                        <td class="w-12 text-right">R1 789.00</td>
//                                        <td class="w-12 text-right">R45 574.00</td>
//                                    </tr>
//                                    <tr>
//                                        <td class="w-12 text-center">24/12/2020</td>
//                                        <td class="w-12 text-center">24/12/2020</td>
//                                        <td class="w-40">Payment Reversal</td>
//                                        <td class="w-12 text-right">R1 789.00</td>
//                                        <td class="w-12 text-right">-</td>
//                                        <td class="w-12 text-right">R47 363.00</td>
//                                    </tr>
//                                    <tr>
//                                        <td class="w-12 text-center">31/12/2020</td>
//                                        <td class="w-12 text-center">31/12/2020</td>
//                                        <td class="w-40">Payment - Thank you</td>
//                                        <td class="w-12 text-right">-</td>
//                                        <td class="w-12 text-right">R671.00</td>
//                                        <td class="w-12 text-right">R46 692.00</td>
//                                    </tr>
//                                    <tr>
//                                        <td class="w-12 text-center">29/01/2020</td>
//                                        <td class="w-12 text-center">29/01/2020</td>
//                                        <td class="w-40">Payment - Thank you</td>
//                                        <td class="w-12 text-right">-</td>
//                                        <td class="w-12 text-right">R671.00</td>
//                                        <td class="w-12 text-right">R46 021.00</td>
//                                    </tr>
//                                </tbody>
//                            </table>
//                        </div>
//                    </div>

//                    <div class="py-2">
//                        <div class='payment-due-header'>Payment Due</div>
//                        <div class="d-flex flex-row">
//                            <div class="paymentDueHeaderBlock mr-1">After 120 + days</div>
//                            <div class="paymentDueHeaderBlock mr-1">After 90 days</div>
//                            <div class="paymentDueHeaderBlock mr-1">After 60 days</div>
//                            <div class="paymentDueHeaderBlock mr-1">After 30 days</div>
//                            <div class="paymentDueHeaderBlock">Current</div>
//                        </div>
//                        <div class="d-flex flex-row mt-1">
//                            <div class="paymentDueFooterBlock mr-1">R0.00</div>
//                            <div class="paymentDueFooterBlock mr-1">R0.00</div>
//                            <div class="paymentDueFooterBlock mr-1">R103.00</div>
//                            <div class="paymentDueFooterBlock mr-1">R671.00</div>
//                            <div class="paymentDueFooterBlock">R671.00</div>
//                        </div>
//                    </div>

//                </div>
//            </div>
//        </div>
//    </div>
//    </div>
//  </div>`
//})
//export class PersonalLoanAccountsBreakdownComponent {
//  @Input()
//  widgetsGridsterItemArray: any[] = [];
//}

// Component Created for Home loan total amount detail Widget -- Nedbank
//@Component({
//  selector: 'HomeLoanTotalAmountDetail',
//  template: `<div class="widget">
//    <div class="widget-area height100">
//     <div class='card border-0 pt-4'>
//          <div class='card-body text-left pb-1'>
//              <div class='card-body-header pb-2'>New instalment</div>
//              <div class='row'>
//                  <div class='col-lg-4 pr-1'>
//                      <div class='TotalAmountDetailsDiv'>
//                          <span class="fnt-10pt">Total Loan Amount</span><br><span class="fnt-14pt">R432 969.00</span><br>
//                      </div>
//                  </div>
//                  <div class='col-lg-8 pl-0 text-right'>
//                      <div class='TotalAmountDetailsDiv'>
//                          <span class="fnt-10pt">Balance outstanding</span><br><span class="fnt-14pt">R136 320.00</span>&nbsp;<br>
//                      </div>
//                  </div>
//              </div>
//          </div>
//      </div>
//    </div>
//  </div>`
//})
//export class HomeLoanTotalAmountDetailComponent {
//  @Input()
//  widgetsGridsterItemArray: any[] = [];
//}

// Component Created for Home loan accounts breakdown Widget -- Nedbank
//@Component({
//  selector: 'HomeLoanAccountsBreakdown',
//  template: `<div class="widget">
//    <div class="widget-area height100">
//      <div class='card border-0 pt-4'>
//        <div class='card-body text-left py-1'>
//            <div class="tab-content">
//                <div id='HomeLoan-4001' class='tab-pane fade in active show'>
//                    <div class="HomeLoanDetailDiv">
//                        <h4 class="pl-25px"><span class="NedbankHomeLoanTxt">Nedbank home loan</span></h4>
//                        <table class="customTable mt-2" border="0">
//                            <tbody>
//                                <tr>
//                                    <td class="w-25">Bond no:</td>
//                                    <td class="w-25 text-right pr-4 text-success">8003876814001</td>
//                                    <td class="w-25">Address:</td>
//                                    <td class="w-25 text-right text-success" rowspan="3">ERF 44 THE COVES H <br/>ARTBEESPOORT <br/></td>
//                                </tr>
//                                <tr>
//                                    <td class="w-25">Instalment:</td>
//                                    <td class="w-25 text-right pr-4 text-success">R4 149.00</td>
//                                </tr>
//                                <tr>
//                                    <td class="w-25">Arrears:</td>
//                                    <td class="w-25 text-right pr-4 text-success">R0.00</td>
//                                </tr>
//                                <tr>
//                                    <td class="w-25">Interest rate:</td>
//                                    <td class="w-25 text-right pr-4 text-success">5.75% pa</td>
//                                    <td class="w-25">Registration date:</td>
//                                    <td class="w-25 text-right pr-4 text-success">12/12/2016</td>
//                                </tr>
//                                <tr>
//                                    <td class="w-25">Loan term:</td>
//                                    <td class="w-25 text-right pr-4 text-success">300 months</td>
//                                    <td class="w-25">Registered amount</td>
//                                    <td class="w-25 text-right pr-4 text-success">R516 037.00</td>
//                                </tr>
//                            </tbody>
//                        </table>
//                    </div>

//                    <div class="pt-1">
//                        <table id="TableWidget" class="LoanTransactionTable customTable">
//                            <thead>
//                                <tr class="ht-30">
//                                    <th class="w-12 text-center">Post date</th>
//                                    <th class="w-12 text-center">Effective date</th>
//                                    <th class="w-40">Transaction</th>
//                                    <th class="w-12 text-right">Debit (R)</th>
//                                    <th class="w-12 text-right">Credit (R)</th>
//                                    <th class="w-12 text-right">Balance (R)</th>
//                                </tr>
//                            </thead>
//                        </table>
//                        <div class="pt-0 overflow-auto" style="max-height:200px;">
//                            <table class="LoanTransactionTable customTable">
//                                <tbody>
//                                    <tr>
//                                        <td class="w-12 text-center">01/12/2020</td>
//                                        <td class="w-12 text-center">01/12/2020</td>
//                                        <td class="w-40">Interest Debit</td>
//                                        <td class="w-12 text-right">R1 490.00</td>
//                                        <td class="w-12 text-right">-</td>
//                                        <td class="w-12 text-right">R74 298.00</td>
//                                    </tr>
//                                    <tr>
//                                        <td class="w-12 text-center">03/12/2020</td>
//                                        <td class="w-12 text-center">03/12/2020</td>
//                                        <td class="w-40">Insurance Premium</td>
//                                        <td class="w-12 text-right">R188.00</td>
//                                        <td class="w-12 text-right">-</td>
//                                        <td class="w-12 text-right">R74 487.00</td>
//                                    </tr>
//                                    <tr>
//                                        <td class="w-12 text-center">15/12/2020</td>
//                                        <td class="w-12 text-center">15/12/2020</td>
//                                        <td class="w-40">Payment - Thank you</td>
//                                        <td class="w-12 text-right">-</td>
//                                        <td class="w-12 text-right">R3 297.00</td>
//                                        <td class="w-12 text-right">R71 189.00</td>
//                                    </tr>
//                                    <tr>
//                                        <td class="w-12 text-center">31/12/2020</td>
//                                        <td class="w-12 text-center">31/12/2020</td>
//                                        <td class="w-40">Monthly Admin Fee</td>
//                                        <td class="w-12 text-right">R69.00</td>
//                                        <td class="w-12 text-right">-</td>
//                                        <td class="w-12 text-right">R71 258.00</td>
//                                    </tr>
//                                    <tr>
//                                        <td class="w-12 text-center">01/01/2021</td>
//                                        <td class="w-12 text-center">01/01/2021</td>
//                                        <td class="w-40">Interest Debit</td>
//                                        <td class="w-12 text-right">R1 512.00</td>
//                                        <td class="w-12 text-right">-</td>
//                                        <td class="w-12 text-right">R72 770.00</td>
//                                    </tr>
//                                    <tr>
//                                        <td class="w-12 text-center">05/01/2021</td>
//                                        <td class="w-12 text-center">05/01/2021</td>
//                                        <td class="w-40">Insurance Premium</td>
//                                        <td class="w-12 text-right">R188.00</td>
//                                        <td class="w-12 text-right">-</td>
//                                        <td class="w-12 text-right">R72 958.00</td>
//                                    </tr>
//                                    <tr>
//                                        <td class="w-12 text-center">15/01/2021</td>
//                                        <td class="w-12 text-center">15/01/2021</td>
//                                        <td class="w-40">Payment - Thank you</td>
//                                        <td class="w-12 text-right">-</td>
//                                        <td class="w-12 text-right">R3 297.00</td>
//                                        <td class="w-12 text-right">R69 660.00</td>
//                                    </tr>
//                                </tbody>
//                            </table>
//                        </div>
//                    </div>

//                    <div class="py-2">
//                        <div class='card-body-header pb-2'>Home loan statement overview</div>
//                        <div class='row pb-1'>
//                            <div class='col-lg-4 pr-0'>
//                                <div class='TotalAmountDetailsDiv'>
//                                    <span class="fnt-14pt">Balance outstanding</span> <br> <span class="fnt-10pt">as at 2021-01-30</span>
//                                </div>
//                            </div>
//                            <div class='col-lg-8 pl-0 text-right'>
//                                <div class='TotalAmountDetailsDiv'>
//                                    <span class="fnt-14pt">R69 660.00</span>&nbsp;<br>
//                                </div>
//                            </div>
//                        </div>

//                        <div class='card-body-sub-header pb-2 pt-1'>Payment Due</div>
//                        <div class="d-flex flex-row">
//                            <div class="paymentDueHeaderBlock mr-1">Current</div>
//                            <div class="paymentDueHeaderBlock mr-1">After 30 days</div>
//                            <div class="paymentDueHeaderBlock mr-1">After 60 days</div>
//                            <div class="paymentDueHeaderBlock mr-1">After 90 days</div>
//                            <div class="paymentDueHeaderBlock">After 120 + days</div>
//                        </div>
//                        <div class="d-flex flex-row mt-1">
//                            <div class="paymentDueFooterBlock mr-1">R0.00</div>
//                            <div class="paymentDueFooterBlock mr-1">R0.00</div>
//                            <div class="paymentDueFooterBlock mr-1">R0.00</div>
//                            <div class="paymentDueFooterBlock mr-1">R0.00</div>
//                            <div class="paymentDueFooterBlock">R0.00</div>
//                        </div>
//                    </div>

//                    <div class='PaymentDueSpecialMessageDiv'>
//                        If you received payment relief because of Covid-19 lockdown, any arrear status that we show above will be resolved when we restructure your account after your three-month arrangement has ended. If you have not applied for relief and would like to, phone us on 0860 555 222 or speak to your relationship banker.
//                    </div>

//                    <div class='card-body-sub-header pt-2'>Summary for Tax Purposes</div>
//                      <div class="pt-1">
//                          <table class="LoanTransactionTable customTable">
//                              <tbody>
//                                  <tr class="ht-20">
//                                      <td class="text-left">Interest </td>
//                                      <td class="text-right">R0.00</td>
//                                  </tr>
//                                  <tr class="ht-20">
//                                      <td class="text-left">Insurance/Assurnace </td>
//                                      <td class="text-right">R0.00</td>
//                                  </tr>
//                                  <tr class="ht-20">
//                                      <td class="text-left">Service fee </td>
//                                      <td class="text-right">R0.00</td>
//                                  </tr>
//                                  <tr class="ht-20">
//                                      <td class="text-left">Legal costs </td>
//                                      <td class="text-right">R0.00</td>
//                                  </tr>
//                                  <tr class="ht-20">
//                                      <td class="text-left">Total amount received </td>
//                                      <td class="text-right">R0.00</td>
//                                  </tr>
//                              </tbody>
//                          </table>
//                      </div>

//                </div>

//                <div id='HomeLoan-6001' class='tab-pane fade'>
//                    <div class="HomeLoanDetailDiv">
//                        <h4 class="pl-25px"><span class="NedbankHomeLoanTxt">Nedbank home loan</span></h4>
//                        <table class="customTable mt-2" border="0">
//                            <tbody>
//                                <tr>
//                                    <td class="w-25">Bond no:</td>
//                                    <td class="w-25 text-right pr-4 text-success">8003876816001</td>
//                                    <td class="w-25">Address:</td>
//                                    <td class="w-25 text-right text-success" rowspan="3">ERF 44 THE COVES H <br/>ARTBEESPOORT <br/></td>
//                                </tr>
//                                <tr>
//                                    <td class="w-25">Instalment:</td>
//                                    <td class="w-25 text-right pr-4 text-success">R11 090.00</td>
//                                </tr>
//                                <tr>
//                                    <td class="w-25">Arrears:</td>
//                                    <td class="w-25 text-right pr-4 text-success">R0.00</td>
//                                </tr>
//                                <tr>
//                                    <td class="w-25">Interest rate:</td>
//                                    <td class="w-25 text-right pr-4 text-success">7.25% pa</td>
//                                    <td class="w-25">Registration date:</td>
//                                    <td class="w-25 text-right pr-4 text-success">21/01/2018</td>
//                                </tr>
//                                <tr>
//                                    <td class="w-25">Loan term:</td>
//                                    <td class="w-25 text-right pr-4 text-success">273 months</td>
//                                    <td class="w-25">Registered amount</td>
//                                    <td class="w-25 text-right pr-4 text-success">R416 879.00</td>
//                                </tr>
//                            </tbody>
//                        </table>
//                    </div>

//                    <div class="pt-1">
//                        <table id="TableWidget" class="LoanTransactionTable customTable">
//                            <thead>
//                                <tr class="ht-30">
//                                    <th class="w-12 text-center">Post date</th>
//                                    <th class="w-12 text-center">Effective date</th>
//                                    <th class="w-40">Transaction</th>
//                                    <th class="w-12 text-right">Debit (R)</th>
//                                    <th class="w-12 text-right">Credit (R)</th>
//                                    <th class="w-12 text-right">Balance (R)</th>
//                                </tr>
//                            </thead>
//                        </table>
//                        <div class="pt-0 overflow-auto" style="max-height:200px;">
//                            <table class="LoanTransactionTable customTable">
//                                <tbody>
//                                    <tr>
//                                        <td class="w-12 text-center">01/12/2020</td>
//                                        <td class="w-12 text-center">01/12/2020</td>
//                                        <td class="w-40">Interest Debit</td>
//                                        <td class="w-12 text-right">R1 490.00</td>
//                                        <td class="w-12 text-right">-</td>
//                                        <td class="w-12 text-right">R71 298.00</td>
//                                    </tr>
//                                    <tr>
//                                        <td class="w-12 text-center">03/12/2020</td>
//                                        <td class="w-12 text-center">03/12/2020</td>
//                                        <td class="w-40">Insurance Premium</td>
//                                        <td class="w-12 text-right">R188.00</td>
//                                        <td class="w-12 text-right">-</td>
//                                        <td class="w-12 text-right">R71 487.00</td>
//                                    </tr>
//                                    <tr>
//                                        <td class="w-12 text-center">15/12/2020</td>
//                                        <td class="w-12 text-center">15/12/2020</td>
//                                        <td class="w-40">Payment - Thank you</td>
//                                        <td class="w-12 text-right">-</td>
//                                        <td class="w-12 text-right">R3 297.00</td>
//                                        <td class="w-12 text-right">R68 189.00</td>
//                                    </tr>
//                                    <tr>
//                                        <td class="w-12 text-center">31/12/2020</td>
//                                        <td class="w-12 text-center">31/12/2020</td>
//                                        <td class="w-40">Monthly Admin Fee</td>
//                                        <td class="w-12 text-right">R69.00</td>
//                                        <td class="w-12 text-right">-</td>
//                                        <td class="w-12 text-right">R68 258.00</td>
//                                    </tr>
//                                    <tr>
//                                        <td class="w-12 text-center">01/01/2021</td>
//                                        <td class="w-12 text-center">01/01/2021</td>
//                                        <td class="w-40">Interest Debit</td>
//                                        <td class="w-12 text-right">R1 512.00</td>
//                                        <td class="w-12 text-right">-</td>
//                                        <td class="w-12 text-right">R69 770.00</td>
//                                    </tr>
//                                    <tr>
//                                        <td class="w-12 text-center">05/01/2021</td>
//                                        <td class="w-12 text-center">05/01/2021</td>
//                                        <td class="w-40">Insurance Premium</td>
//                                        <td class="w-12 text-right">R188.00</td>
//                                        <td class="w-12 text-right">-</td>
//                                        <td class="w-12 text-right">R69 958.00</td>
//                                    </tr>
//                                    <tr>
//                                        <td class="w-12 text-center">15/01/2021</td>
//                                        <td class="w-12 text-center">15/01/2021</td>
//                                        <td class="w-40">Payment - Thank you</td>
//                                        <td class="w-12 text-right">-</td>
//                                        <td class="w-12 text-right">R3 297.00</td>
//                                        <td class="w-12 text-right">R66 660.00</td>
//                                    </tr>
//                                </tbody>
//                            </table>
//                        </div>
//                    </div>

//                    <div class="py-2">
//                        <div class='card-body-header pb-2'>Home loan statement overview</div>
//                        <div class='row pb-1'>
//                            <div class='col-lg-4 pr-0'>
//                                <div class='TotalAmountDetailsDiv'>
//                                    <span class="fnt-14pt">Balance outstanding</span> <br> <span class="fnt-10pt">as at 2021-01-30</span>
//                                </div>
//                            </div>
//                            <div class='col-lg-8 pl-0 text-right'>
//                                <div class='TotalAmountDetailsDiv'>
//                                    <span class="fnt-14pt">R69 660.00</span><br>
//                                </div>
//                            </div>
//                        </div>

//                        <div class='card-body-sub-header pb-2 pt-1'>Payment Due</div>
//                        <div class="d-flex flex-row">
//                            <div class="paymentDueHeaderBlock mr-1">Current</div>
//                            <div class="paymentDueHeaderBlock mr-1">After 30 days</div>
//                            <div class="paymentDueHeaderBlock mr-1">After 60 days</div>
//                            <div class="paymentDueHeaderBlock mr-1">After 90 days</div>
//                            <div class="paymentDueHeaderBlock">After 120 + days</div>
//                        </div>
//                        <div class="d-flex flex-row mt-1">
//                            <div class="paymentDueFooterBlock mr-1">R0.00</div>
//                            <div class="paymentDueFooterBlock mr-1">R0.00</div>
//                            <div class="paymentDueFooterBlock mr-1">R0.00</div>
//                            <div class="paymentDueFooterBlock mr-1">R0.00</div>
//                            <div class="paymentDueFooterBlock">R0.00</div>
//                        </div>
//                    </div>

//                    <div class='PaymentDueSpecialMessageDiv'>
//                        If you received payment relief because of Covid-19 lockdown, any arrear status that we show above will be resolved when we restructure your account after your three-month arrangement has ended. If you have not applied for relief and would like to, phone us on 0860 555 222 or speak to your relationship banker.
//                    </div>

//                    <div class='card-body-sub-header pt-2'>Summary for Tax Purposes</div>
//                      <div class="pt-1">
//                          <table class="LoanTransactionTable customTable">
//                              <tbody>
//                                  <tr class="ht-20">
//                                      <td class="text-left">Interest </td>
//                                      <td class="text-right">R0.00</td>
//                                  </tr>
//                                  <tr class="ht-20">
//                                      <td class="text-left">Insurance/Assurnace </td>
//                                      <td class="text-right">R0.00</td>
//                                  </tr>
//                                  <tr class="ht-20">
//                                      <td class="text-left">Service fee </td>
//                                      <td class="text-right">R0.00</td>
//                                  </tr>
//                                  <tr class="ht-20">
//                                      <td class="text-left">Legal costs </td>
//                                      <td class="text-right">R0.00</td>
//                                  </tr>
//                                  <tr class="ht-20">
//                                      <td class="text-left">Total amount received </td>
//                                      <td class="text-right">R0.00</td>
//                                  </tr>
//                              </tbody>
//                          </table>
//                      </div>
//                </div>
//            </div>
//        </div>
//    </div>
//    </div>
//  </div>`
//})
//export class HomeLoanAccountsBreakdownComponent {
//  @Input()
//  widgetsGridsterItemArray: any[] = [];
//}

// Component Created for Home loan payment due special msg Widget -- Nedbank
//@Component({
//  selector: 'HomeLoanPaymentDueSpecialMsg',
//  template: `<div class="widget">
//    <div class="widget-area height100">
//     <div class="card border-0 pt-2">
//          <div class="card-body text-left py-1">
//              <div class="PaymentDueSpecialMessageDiv pt-2">
//                  If you received payment relief because of Covid-19, any arrear status that we show above will be resolved when we restructure your account after your three-month arrangement has ended. If you have not applied for relief and would like to, phone us on 0860 555 222 or speak to your relationship banker.
//              </div>
//          </div>
//      </div>
//    </div>
//  </div>`
//})
//export class HomeLoanPaymentDueSpecialMsgComponent {
//  @Input()
//  widgetsGridsterItemArray: any[] = [];
//}

// Component Created for Home loan instalment details Widget -- Nedbank
//@Component({
//  selector: 'HomeLoanInstalmentDetail',
//  template: `<div class="widget">
//    <div class="widget-area height100">
//      <div class='card border-0 pt-4'>
//          <div class='card-body text-left py-1'>
//              <div class='card-body-sub-header pt-1'>Instalment details</div>
//              <div class="card-body-sub-header2">Due to insurance changes, your new instalment details are as follows:</div>
//              <div class="pt-1">
//                  <table class="LoanTransactionTable customTable">
//                      <thead>
//                          <tr class="ht-30">
//                              <th class="w-50">Payments</th>
//                              <th class="w-50 text-right">Amount</th>
//                          </tr>
//                      </thead>
//                      <tbody>
//                          <tr class="ht-20">
//                              <td class="text-left">Basic instalment </td>
//                              <td class="text-right">R0.00</td>
//                          </tr>
//                          <tr class="ht-20">
//                              <td class="text-left">Homeowner's insurance </td>
//                              <td class="text-right">R0.00</td>
//                          </tr>
//                          <tr class="ht-20">
//                              <td class="text-left">Credit life insurance </td>
//                              <td class="text-right">R0.00</td>
//                          </tr>
//                          <tr class="ht-20">
//                              <td class="text-left">Transaction fees </td>
//                              <td class="text-right">R0.00</td>
//                          </tr>
//                          <tr class="ht-20">
//                              <td class="text-left">Subsidised account capital redemption </td>
//                              <td class="text-right">R0.00</td>
//                          </tr>
//                          <tr class="ht-20">
//                              <td class="text-left">Monthly service fee </td>
//                              <td class="text-right">R0.00</td>
//                          </tr>
//                          <tr class="ht-20">
//                              <td class="text-left font-weight-bold">Total instalment (effective from 01/02/2021) </td>
//                              <td class="text-right font-weight-bold">R0.00</td>
//                          </tr>
//                      </tbody>
//                  </table>
//              </div>
//          </div>
//      </div>
//    </div>
//  </div>`
//})
//export class HomeLoanInstalmentDetailComponent {
//  @Input()
//  widgetsGridsterItemArray: any[] = [];
//}

// Component Created for Portfolio product level Customer Details Widget -- Nedbank
//@Component({
//  selector: 'PortfolioCustomerDetails',
//  template: `<div class="widget">
//    <div class="widget-area height100">
//       <div class='card border-0'>
//          <div class="card-body CustomerDetails">
//              <span class="fnt-14pt pt-1">MR. Firstname Lastname</span> <br/>
//              <p class="fnt-8pt pt-1">Customer Id: 171001255307</p>
//              <p class="fnt-8pt">Mobile No: +2367 345 786</p>
//              <p>Email: custemail@demo.com</p>
//          </div>
//      </div>
//    </div>
//  </div>`
//})
//export class PortfolioCustomerDetailsComponent {
//  @Input()
//  widgetsGridsterItemArray: any[] = [];
//}

//// Component Created for Portfolio product level Customer address details Widget -- Nedbank
//@Component({
//  selector: 'PortfolioCustomerAddressDetails',
//  template: `<div class="widget">
//    <div class="widget-area height100">
//       <div class='card border-0'>
//          <div class="card-body CustomerDetails">
//              <span class="fnt-14pt">Address</span> <br/>
//              VERDEAU LIFESTYLE ESTATE <br/>
//              6 HERCULE CRESCENT DRIVE <br/>
//              WELLINGTON, 7655
//          </div>
//      </div>
//    </div>
//  </div>`
//})
//export class PortfolioCustomerAddressDetailsComponent {
//  @Input()
//  widgetsGridsterItemArray: any[] = [];
//}

//// Component Created for Portfolio product level client contact details Widget -- Nedbank
//@Component({
//  selector: 'PortfolioClientContactDetails',
//  template: `<div class="widget">
//    <div class="widget-area height100">
//       <div class='card border-0'>
//          <div class="card-body ClientDetails">
//              <span class="fnt-14pt">Client contact details</span><br/>
//              <p>Mobile No: 0860 555 111</p>
//              <p>Email: supportdesk@nedbank.com</p>
//          </div>
//      </div>
//    </div>
//  </div>`
//})
//export class PortfolioClientContactDetailsComponent {
//  @Input()
//  widgetsGridsterItemArray: any[] = [];
//}

//// Component Created for Portfolio product level account summary details Widget -- Nedbank
//@Component({
//  selector: 'PortfolioAccountSummaryDetails',
//  template: `<div class="widget">
//    <div class="widget-area height100">
//       <div class='card border-0'>
//          <div class="card-body">
//              <div class="card-body-header pb-2">Account Summary</div>
//              <div class="pt-1">
//                  <table class="LoanTransactionTable customTable">
//                      <thead>
//                          <tr class="ht-30">
//                              <th class="w-50">Account</th>
//                              <th class="w-50 text-right">Total Amount</th>
//                          </tr>
//                      </thead>
//                      <tbody>
//                          <tr class="ht-30">
//                              <td class="text-left">Investment </td>
//                              <td class="text-right">R9 620.98</td>
//                          </tr>
//                          <tr class="ht-30">
//                              <td class="text-left">Personal Loan </td>
//                              <td class="text-right">R4 165.00</td>
//                          </tr>
//                          <tr class="ht-30">
//                              <td class="text-left">Home Loan </td>
//                              <td class="text-right">R7 969.00</td>
//                          </tr>
//                      </tbody>
//                  </table>
//              </div>
//              <div class="pt-2">
//                  <table class="LoanTransactionTable customTable">
//                      <thead>
//                          <tr class="ht-30">
//                              <th class="text-left">Greenbacks rewards points </th>
//                              <th class="text-right">234</th>
//                          </tr>
//                      </thead>
//                  </table>
//              </div>
//          </div>
//      </div>
//    </div>
//  </div>`
//})
//export class PortfolioAccountSummaryDetailsComponent {
//  @Input()
//  widgetsGridsterItemArray: any[] = [];
//}

//// Component Created for Portfolio product level account analysis Widget -- Nedbank
//@Component({
//  selector: 'PortfolioAccountAnalysis',
//  template: `<div class="widget">
//    <div class="widget-area width100">
//      <div class="card border-0">
//          <div class="card-body">
//              <div class="card-body-header pb-2 text-center pb-1">Account analytics</div>
//              <div id="AccountAnalysisBarGraphcontainer" style="height:200px;"></div>
//          </div>
//      </div>      
//    </div>
//</div>`
//})
//export class PortfolioAccountAnalysisComponent {
//  @Input()
//  widgetsGridsterItemArray: any[] = [];
//  public options4: any = {
//    chart: {
//      backgroundColor: 'rgba(0,0,0,0)',
//      type: 'column',
//      style: {
//        fontFamily: 'Mark Pro Regular',
//        fontSize: '8pt'
//      }
//    },
//    title: {
//      text: ''
//    },
//    xAxis: {
//      categories: ['Jan', 'Feb'],
//      crosshair: true
//    },
//    yAxis: {
//      title: {
//        text: 'Amount (R)'
//      }
//    },
//    credits: {
//      enabled: false
//    },
//    tooltip: {
//      headerFormat: '<span>{point.key}</span><table>',
//      pointFormat: '<tr><td style="color:{series.color};padding:0">{series.name}: </td> <td style="padding:0"><b>{point.y:.2f}</b></td></tr>',
//      footerFormat: '</table>',
//      shared: true,
//      useHTML: true
//    },
//    plotOptions: {
//      column: {
//        pointPadding: 0.2,
//        borderWidth: 0
//      }
//    },
//    series: [{
//      name: 'Investment',
//      data: [9456.12, 9620.98]
//    }, {
//      name: 'Personal loan',
//      data: [-4465.00, -4165.00]
//    }, {
//      name: 'Home loan',
//      data: [-8969.00, -7969.00]
//    }],
//    //colors: ['#005b00', '#434348', '#7cb5ec']
//  }
//  ngAfterViewInit() {
//    setTimeout(() => {
//      Highcharts.chart('AccountAnalysisBarGraphcontainer', this.options4);
//    }, 100);
//  }

//  ngOnInit() {
//    $(document).ready(function () {
//      setTimeout(function () {
//        window.dispatchEvent(new Event('resize'));
//      }, 10);
//    });
//  }
//}

////// Component Created for Portfolio product level reminders and recommendation Widget -- Nedbank
////@Component({
////  selector: 'PortfolioReminders',
////  template: `<div class="widget">
////    <div class="widget-area height100">
////       <div class='card border-0'>
////          <div class='card-body ReminderDiv'>
////              <div class="card-body-header pb-2 fnt-14pt">Reminder and Recommendation</div>
////              <div class='row'>
////                  <div class='col-lg-9 text-left'><p class='p-1' style='background-color: #dce3dc;'>Your Credit Card payment is due </p></div>
////                  <div class='col-lg-3 text-left'>
////                      <a href='javascript:void(0)' target='_blank'>
////                          <i class='fa fa-caret-left fa-3x float-left text-success'></i><span class='mt-2 d-inline-block ml-2'>Pay Now</span>
////                      </a>
////                  </div>
////              </div>
////              <div class='row'>
////                  <div class='col-lg-9 text-left'><p class='p-1' style='background-color: #dce3dc;'>Update missing information in your profile </p></div>
////                  <div class='col-lg-3 text-left'>
////                      <a href='javascript:void(0)' target='_blank'>
////                          <i class='fa fa-caret-left fa-3x float-left text-success'></i><span class='mt-2 d-inline-block ml-2'>Update</span>
////                      </a>
////                  </div>
////              </div>
////              <div class='row'>
////                  <div class='col-lg-9 text-left'>
////                      <p class='p-1' style='background-color: #dce3dc;'>Based on your transaction trend add on BA Airmile card is offered free to you. </p>
////                  </div>
////                  <div class='col-lg-3 text-left'>
////                      <a href='javascript:void(0)' target='_blank'>
////                          <i class='fa fa-caret-left fa-3x float-left text-success'></i><span class='mt-2 d-inline-block ml-2'>Order</span>
////                      </a>
////                  </div>
////              </div>
////              <div class='row'>
////                  <div class='col-lg-9 text-left'><p class='p-1' style='background-color: #dce3dc;'>Need saving deposit Boxes </p></div>
////                  <div class='col-lg-3 text-left'>
////                      <a href='javascript:void(0)' target='_blank'>
////                          <i class='fa fa-caret-left fa-3x float-left text-success'></i><span class='mt-2 d-inline-block ml-2'>Apply</span>
////                      </a>
////                  </div>
////              </div>
////          </div>
////      </div>
////    </div>
////  </div>`
////})
////export class PortfolioRemindersComponent {
////  @Input()
////  widgetsGridsterItemArray: any[] = [];
////}

//// Component Created for Portfolio product level news alerts Widget -- Nedbank
//@Component({
//  selector: 'PortfolioNewsAlerts',
//  template: `<div class="widget">
//    <div class="widget-area height100">
//       <div class='card border-0'>
//          <div class='card-body text-left py-1'>
//            <div class='NewAlertDiv'>
//                <div class='card-body-header pb-2'>News Alerts</div>
//                <p>Covid 19 and the subsequent lockdown has affected all areas of our daily lives. The way we work, the way we bank and how we interact with each other.</p>
//                <p>We want you to know we are in this together. That's why we are sharing advice, tips and news updates with you on ways to bank as well as ways to keep yorself and your loved ones safe.</p>
//                <p>We would like to remind you of the credit life insurance benefits available to you through your Nedbank Insurance policy. When you pass away, Nedbank Insurance will cover your outstanding loan amount. If you are permanently employed, you will also enjoy cover for comprehensive disability and loss of income. The disability benefit will cover your monthly instalments if you cannot earn your usual income due to illness or bodily injury.</p>
//                <div class='float-right'>
//                    <a href='javascript:void(0)' target='_blank'>
//                        <i class='fa fa-caret-left fa-3x float-left text-success'></i><span class='mt-2 d-inline-block ml-2'>More Info</span>
//                    </a>
//                </div>
//            </div>
//        </div>
//      </div>
//    </div>
//  </div>`
//})
//export class PortfolioNewsAlertsComponent {
//  @Input()
//  widgetsGridsterItemArray: any[] = [];
//}

//// Component Created for Greenbacks product level total reward points Widget -- Nedbank
//@Component({
//  selector: 'GreenbacksTotalRewardPoints',
//  template: `<div class="widget">
//    <div class="widget-area height100">
//       <div class='card border-0'>
//          <div class="card-body text-center">
//              <span class="TotalRewardPointsSpan" title='Total Rewards Points'>482</span>
//          </div>
//      </div>
//    </div>
//  </div>`
//})
//export class GreenbacksTotalRewardPointsComponent {
//  @Input()
//  widgetsGridsterItemArray: any[] = [];
//}

//// Component Created for Rewards product level greenbacks contact us Widget -- Nedbank
//@Component({
//  selector: 'GreenbacksContactUs',
//  template: `<div class="widget">
//    <div class="widget-area height100">
//       <div class="card border-0">
//          <div class="card-body GreenbacksContactUsDiv">
//              <ul class="list-group float-right" style="width:250px;">
//                  <li class="list-group-item d-flex justify-content-between align-items-center ht-30 JoinGreenbacks-li-bg border-white">
//                      <a class="text-white" href="javascript:void(0)">
//                          Join Greenbacks
//                          <i class='fa fa-caret-right float-right text-white mt-1'></i>
//                      </a>
//                  </li>
//                  <li class="list-group-item d-flex justify-content-between align-items-center ht-30 UseGreenbacks-li-bg border-white">
//                      <a class="text-white" href="javascript:void(0)">
//                          Use Greenbacks
//                          <i class='fa fa-caret-right float-right text-white mt-1'></i>
//                      </a>
//                  </li>
//                  <li class="list-group-item d-flex justify-content-between align-items-center ht-30 ContactUs-li-bg border-white">
//                      Contact Us 0860 553 111
//                  </li>
//              </ul>
//          </div>
//      </div>
//    </div>
//  </div>`
//})
//export class GreenbacksContactUsComponent {
//  @Input()
//  widgetsGridsterItemArray: any[] = [];
//}

//// Component Created for Rewards product level YTD rewards points Widget -- Nedbank
//@Component({
//  selector: 'YTDRewardPoints',
//  template: `<div class="widget">
//    <div class="widget-area width100">
//      <div class="card border-0">
//          <div class="card-body">
//              <div class="card-body-header pb-2 text-center pb-1">YTD Rewards Points</div>
//              <div id="YTDRewardPointsBarGraphcontainer" style="height:200px;"></div>
//          </div>
//      </div>      
//    </div>
//</div>`
//})
//export class YTDRewardPointsComponent {
//  @Input()
//  widgetsGridsterItemArray: any[] = [];

//  public options4: any = {
//    chart: {
//      backgroundColor: 'rgba(0,0,0,0)',
//      type: 'column',
//      style: {
//        fontFamily: 'Mark Pro Regular',
//        fontSize: '8pt'
//      }
//    },
//    title: {
//      text: ''
//    },
//    xAxis: {
//      categories: ['Jan', 'Feb', 'Mar', 'Apr'],
//      crosshair: true
//    },
//    yAxis: {
//      title: {
//        text: 'Points'
//      }
//    },
//    credits: {
//      enabled: false
//    },
//    tooltip: {
//      headerFormat: '<span>{point.key}</span><table>',
//      pointFormat: '<tr><td style="color:{series.color};padding:0">{series.name}: </td> <td style="padding:0"><b>{point.y:.2f}</b></td></tr>',
//      footerFormat: '</table>',
//      shared: true,
//      useHTML: true
//    },
//    plotOptions: {
//      column: {
//        pointPadding: 0.2,
//        borderWidth: 0
//      },
//      series: {
//        color: '#005b00'
//      }
//    },
//    series: [{
//      name: 'Rewards Points',
//      data: [98, 112, 128, 144]
//    }]
//  }

//  ngAfterViewInit() {
//    setTimeout(() => {
//      Highcharts.chart('YTDRewardPointsBarGraphcontainer', this.options4);
//    }, 100);
//  }

//  ngOnInit() {
//    $(document).ready(function () {
//      setTimeout(function () {
//        window.dispatchEvent(new Event('resize'));
//      }, 10);
//    });
//  }
//}

//// Component Created for Rewards product level Points redeemed YTD Widget -- Nedbank
//@Component({
//  selector: 'PointsRedeemedYTD',
//  template: `<div class="widget">
//    <div class="widget-area width100">
//      <div class="card border-0">
//          <div class="card-body">
//              <div class="card-body-header pb-2 text-center pb-1">Points Redeemed YTD</div>
//              <div id="PointsRedeemedYTDBarGraphcontainer" style="height:200px;"></div>
//          </div>
//      </div>      
//    </div>
//</div>`
//})
//export class PointsRedeemedYTDComponent {
//  @Input()
//  widgetsGridsterItemArray: any[] = [];

//  public options4: any = {
//    chart: {
//      backgroundColor: 'rgba(0,0,0,0)',
//      type: 'column',
//      style: {
//        fontFamily: 'Mark Pro Regular',
//        fontSize: '8pt'
//      }
//    },
//    title: {
//      text: ''
//    },
//    xAxis: {
//      categories: ['Jan', 'Feb', 'Mar', 'Apr'],
//      crosshair: true
//    },
//    yAxis: {
//      title: {
//        text: 'Points'
//      }
//    },
//    credits: {
//      enabled: false
//    },
//    tooltip: {
//      headerFormat: '<span>{point.key}</span><table>',
//      pointFormat: '<tr><td style="color:{series.color};padding:0">{series.name}: </td> <td style="padding:0"><b>{point.y:.2f}</b></td></tr>',
//      footerFormat: '</table>',
//      shared: true,
//      useHTML: true
//    },
//    plotOptions: {
//      column: {
//        pointPadding: 0.2,
//        borderWidth: 0
//      },
//      series: {
//        color: '#005b00'
//      }
//    },
//    series: [{
//      name: 'Redeemed Points',
//      data: [96, 71, 87, 54]
//    }]
//  }

//  ngAfterViewInit() {
//    setTimeout(() => {
//      Highcharts.chart('PointsRedeemedYTDBarGraphcontainer', this.options4);
//    }, 100);
//  }

//  ngOnInit() {
//    $(document).ready(function () {
//      setTimeout(function () {
//        window.dispatchEvent(new Event('resize'));
//      }, 10);
//    });
//  }
//}

//// Component Created for Rewards product level Product Related Points Earned Widget -- Nedbank
//@Component({
//  selector: 'ProductRelatedPointsEarned',
//  template: `<div class="widget">
//    <div class="widget-area width100">
//      <div class="card border-0">
//          <div class="card-body">
//              <div class="card-body-header pb-2 text-center pb-1">Product Related Points Earned</div>
//              <div id="ProductRelatedPointsEarnedBarGraphcontainer" style="height:200px;"></div>
//          </div>
//      </div>      
//    </div>
//</div>`
//})
//export class ProductRelatedPointsEarnedComponent {
//  @Input()
//  widgetsGridsterItemArray: any[] = [];

//  public options4: any = {
//    chart: {
//      backgroundColor: 'rgba(0,0,0,0)',
//      type: 'column',
//      style: {
//        fontFamily: 'Mark Pro Regular',
//        fontSize: '8pt'
//      }
//    },
//    title: {
//      text: ''
//    },
//    xAxis: {
//      categories: ['Jan', 'Feb', 'Mar', 'Apr'],
//      crosshair: true
//    },
//    yAxis: {
//      title: {
//        text: 'Points'
//      }
//    },
//    credits: {
//      enabled: false
//    },
//    tooltip: {
//      headerFormat: '<span>{point.key}</span><table>',
//      pointFormat: '<tr><td style="color:{series.color};padding:0">{series.name}: </td> <td style="padding:0"><b>{point.y:.2f}</b></td></tr>',
//      footerFormat: '</table>',
//      shared: true,
//      useHTML: true
//    },
//    plotOptions: {
//      column: {
//        pointPadding: 0.2,
//        borderWidth: 0
//      }
//    },
//    series: [{
//      name: 'Investment',
//      data: [121, 145, 98, 134]
//    }, {
//      name: 'Personal loan',
//      data: [232, 167, 125, 245]
//    }, {
//      name: 'Home loan',
//      data: [104, 110, 211, 167]
//    }]
//  }

//  ngAfterViewInit() {
//    setTimeout(() => {
//      Highcharts.chart('ProductRelatedPointsEarnedBarGraphcontainer', this.options4);
//    }, 100);
//  }

//  ngOnInit() {
//    $(document).ready(function () {
//      setTimeout(function () {
//        window.dispatchEvent(new Event('resize'));
//      }, 10);
//    });
//  }
//}

//// Component Created for Rewards product level Category Spend Rewards Widget -- Nedbank
//@Component({
//  selector: 'CategorySpendRewards',
//  template: `<div class="widget">
//    <div class="widget-area width100">
//      <div class="card border-0">
//          <div class="card-body">
//              <div class="card-body-header pb-2 text-center pb-1">Category Spend Rewards</div>
//              <div id="CategorySpendRewardsPieChartcontainer" style="height:200px;"></div>
//          </div>
//      </div>      
//    </div>
//</div>`
//})
//export class CategorySpendRewardsComponent {
//  @Input()
//  widgetsGridsterItemArray: any[] = [];

//  public options4: any = {
//    chart: {
//      plotBackgroundColor: null,
//      plotBorderWidth: null,
//      plotShadow: !1,
//      type: 'pie',
//      style: {
//        fontFamily: 'Mark Pro Regular',
//        fontSize: '8pt'
//      }
//    },
//    title: { text: '' },
//    credits: { enabled: false },
//    tooltip: { pointFormat: '{series.name}: <b>{point.percentage:.1f}%</b>' },
//    accessibility: { point: { valueSuffix: '%' } },
//    plotOptions: {
//      pie: {
//        allowPointSelect: !0,
//        cursor: 'pointer',
//        dataLabels: {
//          enabled: !0,
//          format: '{point.percentage:.1f} %'
//        },
//        showInLegend: !0
//      }
//    },
//    series: [{
//      name: 'Percentage',
//      colorByPoint: !0,
//      data: [{ name: 'Fuel', y: 34 }, { name: 'Groceries', y: 15 }, { name: 'Travel', y: 21 }, { name: 'Movies', y: 19 }, { name: 'Shopping', y: 11 }]
//    }]
//  }

//  ngAfterViewInit() {
//    setTimeout(() => {
//      Highcharts.chart('CategorySpendRewardsPieChartcontainer', this.options4);
//    }, 100);
//  }

//  ngOnInit() {
//    $(document).ready(function () {
//      setTimeout(function () {
//        window.dispatchEvent(new Event('resize'));
//      }, 10);
//    });
//  }
//}

/* ---  below highcharts components are created for page preview while designing..
    which are same as above highcharts widgets, but created due to facing issue in page preview in popup --- */

//// Component Created for Portfolio product level account analysis Widget for page preview while designing -- Nedbank
//@Component({
//  selector: 'PortfolioAccountAnalysisPreview',
//  template: `<div class="widget">
//    <div class="widget-area width100">
//      <div class="card border-0">
//          <div class="card-body">
//              <div class="card-body-header pb-2 text-center pb-1">Account analytics</div>
//              <div id="AccountAnalysisPreviewBarGraphcontainer" style="height:200px;"></div>
//          </div>
//      </div>      
//    </div>
//</div>`
//})
//export class PortfolioAccountAnalysisPreviewComponent {
//  @Input()
//  widgetsGridsterItemArray: any[] = [];
//  public options4: any = {
//    chart: {
//      height: (9 / 16 * 100) + '%',
//      backgroundColor: 'rgba(0,0,0,0)',
//      type: 'column',
//      style: {
//        fontFamily: 'Mark Pro Regular',
//        fontSize: '8pt'
//      }
//    },
//    title: {
//      text: ''
//    },
//    xAxis: {
//      categories: ['Jan', 'Feb'],
//      crosshair: true
//    },
//    yAxis: {
//      title: {
//        text: 'Amount (R)'
//      }
//    },
//    credits: {
//      enabled: false
//    },
//    tooltip: {
//      headerFormat: '<span>{point.key}</span><table>',
//      pointFormat: '<tr><td style="color:{series.color};padding:0">{series.name}: </td> <td style="padding:0"><b>{point.y:.2f}</b></td></tr>',
//      footerFormat: '</table>',
//      shared: true,
//      useHTML: true
//    },
//    plotOptions: {
//      column: {
//        pointPadding: 0.2,
//        borderWidth: 0
//      }
//    },
//    series: [{
//      name: 'Investment',
//      data: [9456.12, 9620.98]
//    }, {
//      name: 'Personal loan',
//      data: [-4465.00, -4165.00]
//    }, {
//      name: 'Home loan',
//      data: [-8969.00, -7969.00]
//    }],
//    //colors: ['#005b00', '#434348', '#7cb5ec']
//  }
//  ngAfterViewInit() {
//    setTimeout(() => {
//      Highcharts.chart('AccountAnalysisPreviewBarGraphcontainer', this.options4);
//    }, 100);
//  }

//  ngOnInit() {
//    $(document).ready(function () {
//      setTimeout(function () {
//        window.dispatchEvent(new Event('resize'));
//      }, 10);
//    });
//  }
//}

//// Component Created for Rewards product level YTD rewards points Widget for page preview while designing -- Nedbank
//@Component({
//  selector: 'YTDRewardPointsPreview',
//  template: `<div class="widget">
//    <div class="widget-area width100">
//      <div class="card border-0">
//          <div class="card-body">
//              <div class="card-body-header pb-2 text-center pb-1">YTD Rewards points</div>
//              <div id="YTDRewardPointsBarGraphPreviewcontainer" style="height:200px;"></div>
//          </div>
//      </div>      
//    </div>
//</div>`
//})
//export class YTDRewardPointsPreviewComponent {
//  @Input()
//  widgetsGridsterItemArray: any[] = [];

//  public options4: any = {
//    chart: {
//      backgroundColor: 'rgba(0,0,0,0)',
//      type: 'column',
//      style: {
//        fontFamily: 'Mark Pro Regular',
//        fontSize: '8pt'
//      }
//    },
//    title: {
//      text: ''
//    },
//    xAxis: {
//      categories: ['Jan', 'Feb', 'Mar', 'Apr'],
//      crosshair: true
//    },
//    yAxis: {
//      title: {
//        text: 'Points'
//      }
//    },
//    credits: {
//      enabled: false
//    },
//    tooltip: {
//      headerFormat: '<span>{point.key}</span><table>',
//      pointFormat: '<tr><td style="color:{series.color};padding:0">{series.name}: </td> <td style="padding:0"><b>{point.y:.2f}</b></td></tr>',
//      footerFormat: '</table>',
//      shared: true,
//      useHTML: true
//    },
//    plotOptions: {
//      column: {
//        pointPadding: 0.2,
//        borderWidth: 0
//      },
//      series: {
//        color: '#005b00'
//      }
//    },
//    series: [{
//      name: 'Rewards Points',
//      data: [98, 112, 128, 144]
//    }]
//  }

//  ngAfterViewInit() {
//    setTimeout(() => {
//      Highcharts.chart('YTDRewardPointsBarGraphPreviewcontainer', this.options4);
//    }, 100);
//  }

//  ngOnInit() {
//    $(document).ready(function () {
//      setTimeout(function () {
//        window.dispatchEvent(new Event('resize'));
//      }, 10);
//    });
//  }
//}

//// Component Created for Rewards product level Points redeemed YTD Widget for page preview while designing -- Nedbank
//@Component({
//  selector: 'PointsRedeemedYTDPreview',
//  template: `<div class="widget">
//    <div class="widget-area width100">
//      <div class="card border-0">
//          <div class="card-body">
//              <div class="card-body-header pb-2 text-center pb-1">Points Redeemed YTD</div>
//              <div id="PointsRedeemedYTDBarGraphPreviewcontainer" style="height:200px;"></div>
//          </div>
//      </div>      
//    </div>
//</div>`
//})
//export class PointsRedeemedYTDPreviewComponent {
//  @Input()
//  widgetsGridsterItemArray: any[] = [];

//  public options4: any = {
//    chart: {
//      backgroundColor: 'rgba(0,0,0,0)',
//      type: 'column',
//      style: {
//        fontFamily: 'Mark Pro Regular',
//        fontSize: '8pt'
//      }
//    },
//    title: {
//      text: ''
//    },
//    xAxis: {
//      categories: ['Jan', 'Feb', 'Mar', 'Apr'],
//      crosshair: true
//    },
//    yAxis: {
//      title: {
//        text: 'Points'
//      }
//    },
//    credits: {
//      enabled: false
//    },
//    tooltip: {
//      headerFormat: '<span>{point.key}</span><table>',
//      pointFormat: '<tr><td style="color:{series.color};padding:0">{series.name}: </td> <td style="padding:0"><b>{point.y:.2f}</b></td></tr>',
//      footerFormat: '</table>',
//      shared: true,
//      useHTML: true
//    },
//    plotOptions: {
//      column: {
//        pointPadding: 0.2,
//        borderWidth: 0
//      },
//      series: {
//        color: '#005b00'
//      }
//    },
//    series: [{
//      name: 'Redeemed Points',
//      data: [96, 71, 87, 54]
//    }]
//  }

//  ngAfterViewInit() {
//    setTimeout(() => {
//      Highcharts.chart('PointsRedeemedYTDBarGraphPreviewcontainer', this.options4);
//    }, 100);
//  }

//  ngOnInit() {
//    $(document).ready(function () {
//      setTimeout(function () {
//        window.dispatchEvent(new Event('resize'));
//      }, 10);
//    });
//  }
//}

//// Component Created for Rewards product level Product Related Points Earned Widget for page preview while designing -- Nedbank
//@Component({
//  selector: 'ProductRelatedPointsEarnedPreview',
//  template: `<div class="widget">
//    <div class="widget-area width100">
//      <div class="card border-0">
//          <div class="card-body">
//              <div class="card-body-header pb-2 text-center pb-1">Product Related Points Earned</div>
//              <div id="ProductRelatedPointsEarnedBarGraphPreviewcontainer" style="height:200px;"></div>
//          </div>
//      </div>      
//    </div>
//</div>`
//})
//export class ProductRelatedPointsEarnedPreviewComponent {
//  @Input()
//  widgetsGridsterItemArray: any[] = [];

//  public options4: any = {
//    chart: {
//      backgroundColor: 'rgba(0,0,0,0)',
//      type: 'column',
//      style: {
//        fontFamily: 'Mark Pro Regular',
//        fontSize: '8pt'
//      }
//    },
//    title: {
//      text: ''
//    },
//    xAxis: {
//      categories: ['Jan', 'Feb', 'Mar', 'Apr'],
//      crosshair: true
//    },
//    yAxis: {
//      title: {
//        text: 'Points'
//      }
//    },
//    credits: {
//      enabled: false
//    },
//    tooltip: {
//      headerFormat: '<span>{point.key}</span><table>',
//      pointFormat: '<tr><td style="color:{series.color};padding:0">{series.name}: </td> <td style="padding:0"><b>{point.y:.2f}</b></td></tr>',
//      footerFormat: '</table>',
//      shared: true,
//      useHTML: true
//    },
//    plotOptions: {
//      column: {
//        pointPadding: 0.2,
//        borderWidth: 0
//      }
//    },
//    series: [{
//      name: 'Investment',
//      data: [121, 145, 98, 134]
//    }, {
//      name: 'Personal loan',
//      data: [232, 167, 125, 245]
//    }, {
//      name: 'Home loan',
//      data: [104, 110, 211, 167]
//    }]
//  }

//  ngAfterViewInit() {
//    setTimeout(() => {
//      Highcharts.chart('ProductRelatedPointsEarnedBarGraphPreviewcontainer', this.options4);
//    }, 100);
//  }

//  ngOnInit() {
//    $(document).ready(function () {
//      setTimeout(function () {
//        window.dispatchEvent(new Event('resize'));
//      }, 10);
//    });
//  }
//}

//// Component Created for Rewards product level Category Spend Rewards Widget for page preview while designing -- Nedbank
//@Component({
//  selector: 'CategorySpendRewardsPreview',
//  template: `<div class="widget">
//    <div class="widget-area width100">
//      <div class="card border-0">
//          <div class="card-body">
//              <div class="card-body-header pb-2 text-center pb-1">Category Spend Rewards</div>
//              <div id="CategorySpendRewardsPieChartPreviewcontainer" style="height:200px;"></div>
//          </div>
//      </div>      
//    </div>
//</div>`
//})
//export class CategorySpendRewardsPreviewComponent {
//  @Input()
//  widgetsGridsterItemArray: any[] = [];

//  public options4: any = {
//    chart: {
//      plotBackgroundColor: null,
//      plotBorderWidth: null,
//      plotShadow: !1,
//      type: 'pie',
//      style: {
//        fontFamily: 'Mark Pro Regular',
//        fontSize: '8pt'
//      }
//    },
//    title: { text: '' },
//    credits: { enabled: false },
//    tooltip: { pointFormat: '{series.name}: <b>{point.percentage:.1f}%</b>' },
//    accessibility: { point: { valueSuffix: '%' } },
//    plotOptions: {
//      pie: {
//        allowPointSelect: !0,
//        cursor: 'pointer',
//        dataLabels: {
//          enabled: !0,
//          format: '{point.percentage:.1f} %'
//        },
//        showInLegend: !0
//      }
//    },
//    series: [{
//      name: 'Percentage',
//      colorByPoint: !0,
//      data: [{ name: 'Fuel', y: 34 }, { name: 'Groceries', y: 15 }, { name: 'Travel', y: 21 }, { name: 'Movies', y: 19 }, { name: 'Shopping', y: 11 }]
//    }]
//  }

//  ngAfterViewInit() {
//    setTimeout(() => {
//      Highcharts.chart('CategorySpendRewardsPieChartPreviewcontainer', this.options4);
//    }, 100);
//  }

//  ngOnInit() {
//    $(document).ready(function () {
//      setTimeout(function () {
//        window.dispatchEvent(new Event('resize'));
//      }, 10);
//    });
//  }
//}

//// Component Created for special message Widget -- Nedbank
//@Component({
//  selector: 'HomeLoanSummaryTaxPurpose',
//  template: `<div class="widget">
//      <div class="widget-area width100">
//        <div class='card border-0'>
//          <div class='card-body text-left py-1'>
//            <div class='py-2'>
//              <div class='card-body-sub-header'> Summary for Tax Purposes</div>
//                <div class='d-flex flex-row mt-1 HomeLoanDetailDiv margin_right_15'>
//                  <table class='SummaryTable mt-2' border='0' style="width: 100%;margin-left: 10px !important;">
//                    <tbody>
//                      <tr><td>Interest</td><td class='text-right pr-4 text-success'>R0.00</td></tr>
//                      <tr><td>Insurance/Assurance</td><td class='text-right pr-4 text-success'>R0.00</td></tr>
//                      <tr><td>Service fee</td><td class='text-right pr-4 text-success'>R0.00</td></tr>
//                      <tr><td>Legal costs </td><td class='text-right pr-4 text-success'>R0.00</td></tr>
//                      <tr><td>Total amount received</td><td class='text-right pr-4 text-success'>R0.00</td></tr>
//                    </tbody>
//                  </table>
//                </div>
//              </div>
//            </div>
//           </div>
//          </div>
//        </div>`
//})
//export class HomeLoanSummaryTaxPurposeComponent {
//  @Input()
//  widgetsGridsterItemArray: any[] = [];
//}

//// Component Created for Home Loan Instalment Widget -- Nedbank
//@Component({
//  selector: 'HomeLoanInstalment',
//  template: `<div class="widget">
//                <div class="widget-area width100">
//                    <div class='card-body-sub-header pt-2'>Instalment details</div>
//                      <div class="card-body-sub-header2">Due to insurance changes, your new instalment details are as follows:</div>
//                      <div class="pt-1">
//                          <table class="LoanTransactionTable customTable">
//                              <thead>
//                                  <tr class="ht-30">
//                                      <th class="w-50">Payments</th>
//                                      <th class="w-50 text-right">Amount</th>
//                                  </tr>
//                              </thead>
//                              <tbody>
//                                  <tr class="ht-20">
//                                      <td class="text-left">Basic instalment </td>
//                                      <td class="text-right">R0.00</td>
//                                  </tr>
//                                  <tr class="ht-20">
//                                      <td class="text-left">Homeowner's insurance </td>
//                                      <td class="text-right">R0.00</td>
//                                  </tr>
//                                  <tr class="ht-20">
//                                      <td class="text-left">Credit life insurance </td>
//                                      <td class="text-right">R0.00</td>
//                                  </tr>
//                                  <tr class="ht-20">
//                                      <td class="text-left">Transaction fees </td>
//                                      <td class="text-right">R0.00</td>
//                                  </tr>
//                                  <tr class="ht-20">
//                                      <td class="text-left">Subsidised account capital redemption </td>
//                                      <td class="text-right">R0.00</td>
//                                  </tr>
//                                  <tr class="ht-20">
//                                      <td class="text-left">Monthly service fee </td>
//                                      <td class="text-right">R0.00</td>
//                                  </tr>
//                                  <tr class="ht-20">
//                                      <td class="text-left font-weight-bold">Total instalment (effective from 01/04/2021) </td>
//                                      <td class="text-right font-weight-bold">R0.00</td>
//                                  </tr>
//                              </tbody>
//                          </table>
//                          </div>
//                        </div>
//                      </div>`
//})
//export class HomeLoanInstalmentComponent {
//  @Input()
//  widgetsGridsterItemArray: any[] = [];
//}

//// Component Created for Home loan total amount for wealth detail Widget -- Nedbank
//@Component({
//  selector: 'WealthHomeLoanTotalAmountDetail',
//  template: `<div class="widget">
//    <div class="widget-area height100">
//     <div class='card border-0 pt-4'>
//          <div class='card-body text-left pb-1'>
//              <div class='card-body-header-w pb-2'>New instalment</div>
//              <div class='row'>
//                  <div class='col-lg-4 pr-1'>
//                      <div class='TotalAmountDetailsDivW'>
//                          <span class="fnt-10pt">Total Loan Amount</span><br><span class="fnt-14pt">R432 969.00</span><br>
//                      </div>
//                  </div>
//                  <div class='col-lg-8 pl-0 text-right'>
//                      <div class='TotalAmountDetailsDivW'>
//                          <span class="fnt-10pt">Balance outstanding</span><br><span class="fnt-14pt">R136 320.00</span>&nbsp;<br>
//                      </div>
//                  </div>
//              </div>
//          </div>
//      </div>
//    </div>
//  </div>`
//})
//export class WealthHomeLoanTotalAmountDetailComponent {
//  @Input()
//  widgetsGridsterItemArray: any[] = [];
//}

//// Component Created for Home loan accounts breakdown for wealth detail Widget -- Nedbank
//@Component({
//  selector: 'WealthHomeLoanAccountsBreakdown',
//  template: `<div class="widget">
//    <div class="widget-area height100">
//      <div class='card border-0 pt-4'>
//        <div class='card-body text-left py-1'>
//            <div class="tab-content">
//                <div id='HomeLoan-4001' class='tab-pane fade in active show'>
//                    <div class="HomeLoanDetailDiv">
//                        <h4 class="pl-25px"><span class="NedbankHomeLoanTxt">Nedbank home loan</span></h4>
//                        <table class="customTable mt-2" border="0">
//                            <tbody>
//                                <tr>
//                                    <td class="w-25">Bond no:</td>
//                                    <td class="w-25 text-right pr-4 text-success">8003876814001</td>
//                                    <td class="w-25">Address:</td>
//                                    <td class="w-25 text-right text-success" rowspan="3">ERF 44 THE COVES H <br/>ARTBEESPOORT <br/></td>
//                                </tr>
//                                <tr>
//                                    <td class="w-25">Instalment:</td>
//                                    <td class="w-25 text-right pr-4 text-success">R4 149.00</td>
//                                </tr>
//                                <tr>
//                                    <td class="w-25">Arrears:</td>
//                                    <td class="w-25 text-right pr-4 text-success">R0.00</td>
//                                </tr>
//                                <tr>
//                                    <td class="w-25">Interest rate:</td>
//                                    <td class="w-25 text-right pr-4 text-success">5.75% pa</td>
//                                    <td class="w-25">Registration date:</td>
//                                    <td class="w-25 text-right pr-4 text-success">12/12/2016</td>
//                                </tr>
//                                <tr>
//                                    <td class="w-25">Loan term:</td>
//                                    <td class="w-25 text-right pr-4 text-success">300 months</td>
//                                    <td class="w-25">Registered amount</td>
//                                    <td class="w-25 text-right pr-4 text-success">R516 037.00</td>
//                                </tr>
//                            </tbody>
//                        </table>
//                    </div>

//                    <div class="pt-1">
//                        <table id="TableWidget" class="LoanTransactionTable customTable">
//                            <thead>
//                                <tr class="ht-30">
//                                    <th class="w-12 text-center">Post date</th>
//                                    <th class="w-12 text-center">Effective date</th>
//                                    <th class="w-40">Transaction</th>
//                                    <th class="w-12 text-right">Debit (R)</th>
//                                    <th class="w-12 text-right">Credit (R)</th>
//                                    <th class="w-12 text-right">Balance (R)</th>
//                                </tr>
//                            </thead>
//                        </table>
//                        <div class="pt-0 overflow-auto" style="max-height:200px;">
//                            <table class="LoanTransactionTable customTable">
//                                <tbody>
//                                    <tr>
//                                        <td class="w-12 text-center">01/12/2020</td>
//                                        <td class="w-12 text-center">01/12/2020</td>
//                                        <td class="w-40">Interest Debit</td>
//                                        <td class="w-12 text-right">R1 490.00</td>
//                                        <td class="w-12 text-right">-</td>
//                                        <td class="w-12 text-right">R74 298.00</td>
//                                    </tr>
//                                    <tr>
//                                        <td class="w-12 text-center">03/12/2020</td>
//                                        <td class="w-12 text-center">03/12/2020</td>
//                                        <td class="w-40">Insurance Premium</td>
//                                        <td class="w-12 text-right">R188.00</td>
//                                        <td class="w-12 text-right">-</td>
//                                        <td class="w-12 text-right">R74 487.00</td>
//                                    </tr>
//                                    <tr>
//                                        <td class="w-12 text-center">15/12/2020</td>
//                                        <td class="w-12 text-center">15/12/2020</td>
//                                        <td class="w-40">Payment - Thank you</td>
//                                        <td class="w-12 text-right">-</td>
//                                        <td class="w-12 text-right">R3 297.00</td>
//                                        <td class="w-12 text-right">R71 189.00</td>
//                                    </tr>
//                                    <tr>
//                                        <td class="w-12 text-center">31/12/2020</td>
//                                        <td class="w-12 text-center">31/12/2020</td>
//                                        <td class="w-40">Monthly Admin Fee</td>
//                                        <td class="w-12 text-right">R69.00</td>
//                                        <td class="w-12 text-right">-</td>
//                                        <td class="w-12 text-right">R71 258.00</td>
//                                    </tr>
//                                    <tr>
//                                        <td class="w-12 text-center">01/01/2021</td>
//                                        <td class="w-12 text-center">01/01/2021</td>
//                                        <td class="w-40">Interest Debit</td>
//                                        <td class="w-12 text-right">R1 512.00</td>
//                                        <td class="w-12 text-right">-</td>
//                                        <td class="w-12 text-right">R72 770.00</td>
//                                    </tr>
//                                    <tr>
//                                        <td class="w-12 text-center">05/01/2021</td>
//                                        <td class="w-12 text-center">05/01/2021</td>
//                                        <td class="w-40">Insurance Premium</td>
//                                        <td class="w-12 text-right">R188.00</td>
//                                        <td class="w-12 text-right">-</td>
//                                        <td class="w-12 text-right">R72 958.00</td>
//                                    </tr>
//                                    <tr>
//                                        <td class="w-12 text-center">15/01/2021</td>
//                                        <td class="w-12 text-center">15/01/2021</td>
//                                        <td class="w-40">Payment - Thank you</td>
//                                        <td class="w-12 text-right">-</td>
//                                        <td class="w-12 text-right">R3 297.00</td>
//                                        <td class="w-12 text-right">R69 660.00</td>
//                                    </tr>
//                                </tbody>
//                            </table>
//                        </div>
//                    </div>

//                    <div class="py-2">
//                        <div class='card-body-header pb-2'>Home loan statement overview</div>
//                        <div class='row pb-1'>
//                            <div class='col-lg-4 pr-0'>
//                                <div class='TotalAmountDetailsDiv'>
//                                    <span class="fnt-14pt">Balance outstanding</span> <br> <span class="fnt-10pt">as at 2021-01-30</span>
//                                </div>
//                            </div>
//                            <div class='col-lg-8 pl-0 text-right'>
//                                <div class='TotalAmountDetailsDiv'>
//                                    <span class="fnt-14pt">R69 660.00</span>&nbsp;<br>
//                                </div>
//                            </div>
//                        </div>

//                        <div class='card-body-sub-header pb-2 pt-1'>Payment Due</div>
//                        <div class="d-flex flex-row">
//                            <div class="paymentDueHeaderBlock mr-1">Current</div>
//                            <div class="paymentDueHeaderBlock mr-1">After 30 days</div>
//                            <div class="paymentDueHeaderBlock mr-1">After 60 days</div>
//                            <div class="paymentDueHeaderBlock mr-1">After 90 days</div>
//                            <div class="paymentDueHeaderBlock">After 120 + days</div>
//                        </div>
//                        <div class="d-flex flex-row mt-1">
//                            <div class="paymentDueFooterBlock mr-1">R0.00</div>
//                            <div class="paymentDueFooterBlock mr-1">R0.00</div>
//                            <div class="paymentDueFooterBlock mr-1">R0.00</div>
//                            <div class="paymentDueFooterBlock mr-1">R0.00</div>
//                            <div class="paymentDueFooterBlock">R0.00</div>
//                        </div>
//                    </div>

//                    <div class='PaymentDueSpecialMessageDiv'>
//                        If you received payment relief because of Covid-19 lockdown, any arrear status that we show above will be resolved when we restructure your account after your three-month arrangement has ended. If you have not applied for relief and would like to, phone us on 0860 555 222 or speak to your relationship banker.
//                    </div>

//                    <div class='card-body-sub-header pt-2'>Summary for Tax Purposes</div>
//                      <div class="pt-1">
//                          <table class="LoanTransactionTable customTable">
//                              <tbody>
//                                  <tr class="ht-20">
//                                      <td class="text-left">Interest </td>
//                                      <td class="text-right">R0.00</td>
//                                  </tr>
//                                  <tr class="ht-20">
//                                      <td class="text-left">Insurance/Assurnace </td>
//                                      <td class="text-right">R0.00</td>
//                                  </tr>
//                                  <tr class="ht-20">
//                                      <td class="text-left">Service fee </td>
//                                      <td class="text-right">R0.00</td>
//                                  </tr>
//                                  <tr class="ht-20">
//                                      <td class="text-left">Legal costs </td>
//                                      <td class="text-right">R0.00</td>
//                                  </tr>
//                                  <tr class="ht-20">
//                                      <td class="text-left">Total amount received </td>
//                                      <td class="text-right">R0.00</td>
//                                  </tr>
//                              </tbody>
//                          </table>
//                      </div>

//                </div>

//                <div id='HomeLoan-6001' class='tab-pane fade'>
//                    <div class="HomeLoanDetailDiv">
//                        <h4 class="pl-25px"><span class="NedbankHomeLoanTxt-W">Nedbank home loan</span></h4>
//                        <table class="customTable mt-2" border="0">
//                            <tbody>
//                                <tr>
//                                    <td class="w-25">Bond no:</td>
//                                    <td class="w-25 text-right pr-4 text-success-w">8003876816001</td>
//                                    <td class="w-25">Address:</td>
//                                    <td class="w-25 text-right text-success" rowspan="3">ERF 44 THE COVES H <br/>ARTBEESPOORT <br/></td>
//                                </tr>
//                                <tr>
//                                    <td class="w-25">Instalment:</td>
//                                    <td class="w-25 text-right pr-4 text-success-w">R11 090.00</td>
//                                </tr>
//                                <tr>
//                                    <td class="w-25">Arrears:</td>
//                                    <td class="w-25 text-right pr-4 text-success-w">R0.00</td>
//                                </tr>
//                                <tr>
//                                    <td class="w-25">Interest rate:</td>
//                                    <td class="w-25 text-right pr-4 text-success-w">7.25% pa</td>
//                                    <td class="w-25">Registration date:</td>
//                                    <td class="w-25 text-right pr-4 text-success-w">21/01/2018</td>
//                                </tr>
//                                <tr>
//                                    <td class="w-25">Loan term:</td>
//                                    <td class="w-25 text-right pr-4 text-success-w">273 months</td>
//                                    <td class="w-25">Registered amount</td>
//                                    <td class="w-25 text-right pr-4 text-success-w">R416 879.00</td>
//                                </tr>
//                            </tbody>
//                        </table>
//                    </div>

//                    <div class="pt-1">
//                        <table id="TableWidget" class="LoanTransactionTable-W customTable">
//                            <thead>
//                                <tr class="ht-30">
//                                    <th class="w-12 text-center">Post date</th>
//                                    <th class="w-12 text-center">Effective date</th>
//                                    <th class="w-40">Transaction</th>
//                                    <th class="w-12 text-right">Debit (R)</th>
//                                    <th class="w-12 text-right">Credit (R)</th>
//                                    <th class="w-12 text-right">Balance (R)</th>
//                                </tr>
//                            </thead>
//                        </table>
//                        <div class="pt-0 overflow-auto" style="max-height:200px;">
//                            <table class="LoanTransactionTable customTable">
//                                <tbody>
//                                    <tr>
//                                        <td class="w-12 text-center">01/12/2020</td>
//                                        <td class="w-12 text-center">01/12/2020</td>
//                                        <td class="w-40">Interest Debit</td>
//                                        <td class="w-12 text-right">R1 490.00</td>
//                                        <td class="w-12 text-right">-</td>
//                                        <td class="w-12 text-right">R71 298.00</td>
//                                    </tr>
//                                    <tr>
//                                        <td class="w-12 text-center">03/12/2020</td>
//                                        <td class="w-12 text-center">03/12/2020</td>
//                                        <td class="w-40">Insurance Premium</td>
//                                        <td class="w-12 text-right">R188.00</td>
//                                        <td class="w-12 text-right">-</td>
//                                        <td class="w-12 text-right">R71 487.00</td>
//                                    </tr>
//                                    <tr>
//                                        <td class="w-12 text-center">15/12/2020</td>
//                                        <td class="w-12 text-center">15/12/2020</td>
//                                        <td class="w-40">Payment - Thank you</td>
//                                        <td class="w-12 text-right">-</td>
//                                        <td class="w-12 text-right">R3 297.00</td>
//                                        <td class="w-12 text-right">R68 189.00</td>
//                                    </tr>
//                                    <tr>
//                                        <td class="w-12 text-center">31/12/2020</td>
//                                        <td class="w-12 text-center">31/12/2020</td>
//                                        <td class="w-40">Monthly Admin Fee</td>
//                                        <td class="w-12 text-right">R69.00</td>
//                                        <td class="w-12 text-right">-</td>
//                                        <td class="w-12 text-right">R68 258.00</td>
//                                    </tr>
//                                    <tr>
//                                        <td class="w-12 text-center">01/01/2021</td>
//                                        <td class="w-12 text-center">01/01/2021</td>
//                                        <td class="w-40">Interest Debit</td>
//                                        <td class="w-12 text-right">R1 512.00</td>
//                                        <td class="w-12 text-right">-</td>
//                                        <td class="w-12 text-right">R69 770.00</td>
//                                    </tr>
//                                    <tr>
//                                        <td class="w-12 text-center">05/01/2021</td>
//                                        <td class="w-12 text-center">05/01/2021</td>
//                                        <td class="w-40">Insurance Premium</td>
//                                        <td class="w-12 text-right">R188.00</td>
//                                        <td class="w-12 text-right">-</td>
//                                        <td class="w-12 text-right">R69 958.00</td>
//                                    </tr>
//                                    <tr>
//                                        <td class="w-12 text-center">15/01/2021</td>
//                                        <td class="w-12 text-center">15/01/2021</td>
//                                        <td class="w-40">Payment - Thank you</td>
//                                        <td class="w-12 text-right">-</td>
//                                        <td class="w-12 text-right">R3 297.00</td>
//                                        <td class="w-12 text-right">R66 660.00</td>
//                                    </tr>
//                                </tbody>
//                            </table>
//                        </div>
//                    </div>

//                    <div class="py-2">
//                        <div class='card-body-header-w pb-2'>Home loan statement overview</div>
//                        <div class='row pb-1'>
//                            <div class='col-lg-4 pr-0'>
//                                <div class='TotalAmountDetailsDivW'>
//                                    <span class="fnt-14pt">Balance outstanding</span> <br> <span class="fnt-10pt">as at 28 February 2022</span>
//                                </div>
//                            </div>
//                            <div class='col-lg-8 pl-0 text-right'>
//                                <div class='TotalAmountDetailsDivW'>
//                                    <span class="fnt-14pt">R69 660.00</span><br>
//                                </div>
//                            </div>
//                        </div>

//                        <div class='card-body-sub-header-w pb-2 pt-1'>Payment Due</div>
//                        <div class="d-flex flex-row">
//                            <div class="paymentDueHeaderBlock-W mr-1">Current</div>
//                            <div class="paymentDueHeaderBlock-W mr-1">After 30 days</div>
//                            <div class="paymentDueHeaderBlock-W mr-1">After 60 days</div>
//                            <div class="paymentDueHeaderBlock-W mr-1">After 90 days</div>
//                            <div class="paymentDueHeaderBlock">After 120 + days</div>
//                        </div>
//                        <div class="d-flex flex-row mt-1">
//                            <div class="paymentDueFooterBlock mr-1">R0.00</div>
//                            <div class="paymentDueFooterBlock mr-1">R0.00</div>
//                            <div class="paymentDueFooterBlock mr-1">R0.00</div>
//                            <div class="paymentDueFooterBlock mr-1">R0.00</div>
//                            <div class="paymentDueFooterBlock">R0.00</div>
//                        </div>
//                    </div>
//            </div>
//        </div>
//    </div>
//    </div>
//  </div>`
//})
//export class WealthHomeLoanAccountsBreakdownComponent {
//  @Input()
//  widgetsGridsterItemArray: any[] = [];
//}

//// Component Created for special message Widget for Widget -- Nedbank
//@Component({
//  selector: 'WealthHomeLoanSummaryTaxPurpose',
//  template: `<div class="widget">
//      <div class="widget-area width100">
//        <div class='card border-0'>
//          <div class='card-body text-left py-1'>
//            <div class='py-2'>
//              <div class='card-body-sub-header-w'> Summary for Tax Purposes</div>
//                <div class='d-flex flex-row mt-1 HomeLoanDetailDiv margin_right_15'>
//                  <table class='SummaryTable mt-2' border='0' style="width: 100%;margin-left: 10px !important;">
//                    <tbody>
//                      <tr><td>Interest</td><td class='text-right pr-4 text-success-w'>R0.00</td></tr>
//                      <tr><td>Insurance/Assurance</td><td class='text-right pr-4 text-success-w'>R0.00</td></tr>
//                      <tr><td>Service fee</td><td class='text-right pr-4 text-success-w'>R0.00</td></tr>
//                      <tr><td>Legal costs </td><td class='text-right pr-4 text-success-w'>R0.00</td></tr>
//                      <tr><td>Total amount received</td><td class='text-right pr-4 text-success-w'>R0.00</td></tr>
//                    </tbody>
//                  </table>
//                </div>
//              </div>
//            </div>
//           </div>
//          </div>
//        </div>`
//})
//export class WealthHomeLoanSummaryTaxPurposeComponent {
//  @Input()
//  widgetsGridsterItemArray: any[] = [];
//}

//@Component({
//  selector: 'WealthHomeLoanInstalment',
//  template: `<div class="widget">
//                <div class="widget-area width100">
//                    <div class='card-body-sub-header-w pt-2'>Instalment details</div>
//                      <div class="card-body-sub-header2">Due to insurance changes, your new instalment details are as follows:</div>
//                      <div class="pt-1">
//                          <table class="LoanTransactionTable-W customTable">
//                              <thead>
//                                  <tr class="ht-30">
//                                      <th class="w-50">Payments</th>
//                                      <th class="w-50 text-right">Amount</th>
//                                  </tr>
//                              </thead>
//                              <tbody>
//                                  <tr class="ht-20">
//                                      <td class="text-left">Basic instalment </td>
//                                      <td class="text-right">R0.00</td>
//                                  </tr>
//                                  <tr class="ht-20">
//                                      <td class="text-left">Homeowner's insurance </td>
//                                      <td class="text-right">R0.00</td>
//                                  </tr>
//                                  <tr class="ht-20">
//                                      <td class="text-left">Credit life insurance </td>
//                                      <td class="text-right">R0.00</td>
//                                  </tr>
//                                  <tr class="ht-20">
//                                      <td class="text-left">Transaction fees </td>
//                                      <td class="text-right">R0.00</td>
//                                  </tr>
//                                  <tr class="ht-20">
//                                      <td class="text-left">Subsidised account capital redemption </td>
//                                      <td class="text-right">R0.00</td>
//                                  </tr>
//                                  <tr class="ht-20">
//                                      <td class="text-left">Monthly service fee </td>
//                                      <td class="text-right">R0.00</td>
//                                  </tr>
//                                  <tr class="ht-20">
//                                      <td class="text-left font-weight-bold">Total instalment (effective from 01/04/2021) </td>
//                                      <td class="text-right font-weight-bold">R0.00</td>
//                                  </tr>
//                              </tbody>
//                          </table>
//                          </div>
//                        </div>
//                      </div>`
//})
//export class WealthHomeLoanInstalmentComponent {
//  @Input()
//  widgetsGridsterItemArray: any[] = [];
//}

//// Component Created for Home Loan Bank Details Widget -- Nedbank
//@Component({
//  selector: 'WealthHomeLoanBranchDetails',
//  template: `<div class="widget">
//    <div class="widget-area height100">
//      <div class='card border-0'>
//          <div class="card-body BranchDetails">
//              BankName<br>Address Line1, City, ZipCode<br>Address Line2, City, ZipCode<br>Country Name
//              <br>Bank VAT Reg No XXXXXXXXXX
//              <br>22/02/2022
//          </div>
//          <div class="ConactCenterDiv text-success-w float-right pt-3">
//              Nedbank Private Wealth Service Suite: XXXX XXX XXX
//          </div>
//      </div>
//    </div>
//  </div>`
//})
//export class WealthHomeLoanBankDetailsComponent {
//  @Input()
//  widgetsGridsterItemArray: any[] = [];
//}

//// Component Created for MCA Account Summary for Widget -- Nedbank
//@Component({
//  selector: 'MCAAccountSummary',
//  template: `<div class="widget">
//    <div class="widget-area height100">
//     <div id class="card border-0">
//      <div class="card-body text-left py-0">
//        <div class="mca_header_label pb-2">Currency Account Statement</div>
//          <div class="mca_sub_header_label pb-2">Please examine this statement at once. If no errors are reported to us within 15 days, this statement will be considered correct.</div>
//        </div>
//       </div>
//       <div class="MCADetailDiv">
//        <h4 class="pl-25px">
//          <span class="mca_detail_statement_label">Account summary</span>
//        </h4>
//        <table class="mca_custom_table mt-2" border="0" style='width: 100%'><tbody>
//          <tr>
//            <td class="w-25">Account no:</td>
//            <td class="w-25 text-right pr-4 mca_text_custom_color">7503010231</td>
//            <td class="w-25">Statement no:</td><td class="w-25 text-right mca_text_custom_color">96</td>
//          </tr>
//          <tr>
//            <td class="w-25">Overdraft limit:</td><td class="w-25 text-right pr-4 mca_text_custom_color">0.00</td>
//            <td class="w-25">Statement date</td><td class="w-25 text-right mca_text_custom_color">09/05/2020</td>
//          </tr>
//          <tr>
//            <td class="w-25">Currency:</td><td class="w-25 text-right pr-4 mca_text_custom_color">USD</td>
//            <td class="w-25">Statement frequency</td><td class="w-25 text-right mca_text_custom_color">WEEKLY</td>
//          </tr>
//          <tr>
//            <td class="w-25">Free balance:</td><td class="w-25 text-right pr-4 mca_text_custom_color">0.00</td>
//            <td class="w-25"></td><td class="w-25 text-right pr-4 mca_text_custom_color"></td>
//          </tr>
//        </tbody></table>
//      </div>
//    </div>
//  </div>`
//})
//export class MCAAccountSummaryComponent {
//  @Input()
//  widgetsGridsterItemArray: any[] = [];
//}

//// Component Created for MCA Transaction for Widget -- Nedbank
//@Component({
//  selector: 'MCATransaction',
//  template: `<div class="widget">
//    <div class="widget-area height100">
//     <div class='pt-1'>
//      <table class='MCATransactionTable mca_custom_table' style='width: 100%'>
//        <thead>
//          <tr class='ht-30'>
//            <th class='w-15 text-center'>Transaction date</th>
//            <th class='w-35 text-left'>Description and additional information</th>
//            <th class='w-12 text-right'>Debit</th>
//            <th class='w-12 text-right'>Credit</th>
//            <th class='w-7 text-center'>Rate</th>
//            <th class='w-7 text-center'>Days</th>
//            <th class='w-12 text-right'>Accrued interest</th>
//          </tr>
//        </thead>
//      </table>
//     <div class='pt-0 overflow-auto'>
//        <table id='MCATransactionTable' class='MCATransactionTable mca_custom_table' style='width: 100%'>
//          <tbody>
//            <tr class='ht-20'>
//              <td class='w-15 text-center'>02/05/2020</td>
//              <td class='w-35 text-left'>BROUGHT FORWARD</td>
//              <td class='w-12 text-right'></td>
//              <td class='w-12 text-right'>50 606.44</td>
//              <td class='w-7 text-center'>0.03</td>
//              <td class='w-7 text-center'></td>
//              <td class='w-12 text-right'>0.04</td>
//            </tr>
//            <tr class='ht-20'>
//              <td class='w-15 text-center'>08/05/2020</td>
//              <td class='w-35 text-left'>INTEREST ACCRUED ON 50606.64 CR FROM 2020-05-02 TO 2020-05-08</td>
//              <td class='w-12 text-right'></td>
//              <td class='w-12 text-right'></td>
//              <td class='w-7 text-center'>0.03</td>
//              <td class='w-7 text-center'>7</td>
//              <td class='w-12 text-right'>0.28</td>
//            </tr>
//            <tr class='ht-20'>
//              <td class='w-15 text-center'>09/05/2020</td>
//              <td class='w-35 text-left'>CARRIED FORWARD</td>
//              <td class='w-12 text-right'></td>
//              <td class='w-12 text-right'>50 606.64</td>
//              <td class='w-7 text-center'>0.03</td>
//              <td class='w-7 text-center'></td>
//              <td class='w-12 text-right'>0.32</td>
//            </tr>
//            <tr class='ht-20'>
//              <td class='w-15 text-center'>10/05/2020</td>
//              <td class='w-35 text-left'>BROUGHT FORWARD</td>
//              <td class='w-12 text-right'></td>
//              <td class='w-12 text-right'>50 606.44</td>
//              <td class='w-7 text-center'>0.03</td>
//              <td class='w-7 text-center'></td>
//              <td class='w-12 text-right'>0.04</td>
//            </tr>
//          </tbody>
//         </table>
//        </div>
//      </div>
//    </div>
//  </div>`
//})
//export class MCATransactionComponent {
//  @Input()
//  widgetsGridsterItemArray: any[] = [];
//}

//// Component Created for MCA VAT Analysis for Widget -- Nedbank
//@Component({
//  selector: 'MCAVATAnalysis',
//  template: `<div class="widget">
//    <div class="widget-area height100">
//     <div class='pt-1'>
//      <table class='MCATransactionTable mca_small_table' style='width: 40%'>
//        <thead>
//          <tr class='ht-30'>
//            <th class='ip-w-25 text-left'>From</th>
//            <th class='ip-w-25 text-left'>To</th>
//            <th class='ip-w-25 text-right'>Rate</th>
//            <th class='ip-w-25 text-right'>Amount</th>
//          </tr>
//        </thead>
//      </table>
//     <div class='pt-0 overflow-auto'>
//        <table id='MCATransactionTable' class='MCATransactionTable mca_small_table' style='width: 40%'>
//          <tbody>
//            <tr class='ht-20'>
//              <td class='ip-w-25 text-left'>02/05/2020</td>
//              <td class='ip-w-25 text-left'>02/05/2020</td>
//              <td class='ip-w-25 text-right'>0.3</td>
//              <td class='ip-w-25 text-right'>50 606.44</td>
//            </tr>
//          </tbody>
//         </table>
//        </div>
//      </div>
//    </div>
//  </div>`
//})
//export class MCAVATAnalysisComponent {
//  @Input()
//  widgetsGridsterItemArray: any[] = [];
//}

//// Component Created for MCA Account Summary for Widget -- Nedbank
//@Component({
//  selector: 'WealthMCAAccountSummary',
//  template: `<div class="widget">
//    <div class="widget-area height100">
//     <div id class="card border-0">
//      <div class="card-body text-left py-0">
//        <div class="mca_header_label-w pb-2">Currency Account Statement</div>
//          <div class="mca_sub_header_label pb-2">Please examine this statement at once. If no errors are reported to us within 15 days, this statement will be considered correct.</div>
//        </div>
//       </div>
//       <div class="MCADetailDiv">
//        <h4 class="pl-25px">
//          <span class="mca_detail_statement_label-w">Account summary</span>
//        </h4>
//        <table class="mca_custom_table mt-2" border="0" style='width: 100%'><tbody>
//          <tr>
//            <td class="w-25">Account no:</td>
//            <td class="w-25 text-right pr-4 mca_text_custom_color-w">7503010231</td>
//            <td class="w-25">Statement no:</td><td class="w-25 text-right mca_text_custom_color-w">96</td>
//          </tr>
//          <tr>
//            <td class="w-25">Overdraft limit:</td><td class="w-25 text-right pr-4 mca_text_custom_color-w">0.00</td>
//            <td class="w-25">Statement date</td><td class="w-25 text-right mca_text_custom_color-w">09/05/2020</td>
//          </tr>
//          <tr>
//            <td class="w-25">Currency:</td><td class="w-25 text-right pr-4 mca_text_custom_color-w">USD</td>
//            <td class="w-25">Statement frequency</td><td class="w-25 text-right mca_text_custom_color-w">WEEKLY</td>
//          </tr>
//          <tr>
//            <td class="w-25">Free balance:</td><td class="w-25 text-right pr-4 mca_text_custom_color-w">0.00</td>
//            <td class="w-25"></td><td class="w-25 text-right pr-4 mca_text_custom_color-w"></td>
//          </tr>
//        </tbody></table>
//      </div>
//    </div>
//  </div>`
//})
//export class WealthMCAAccountSummaryComponent {
//  @Input()
//  widgetsGridsterItemArray: any[] = [];
//}

//// Component Created for MCA Transaction for Widget -- Nedbank
//@Component({
//  selector: 'WealthMCATransaction',
//  template: `<div class="widget">
//    <div class="widget-area height100">
//     <div class='pt-1'>
//      <table class='MCATransactionTable-W mca_custom_table' style='width: 100%'>
//        <thead>
//          <tr class='ht-30'>
//            <th class='w-15 text-center'>Transaction date</th>
//            <th class='w-35 text-left'>Description and additional information</th>
//            <th class='w-12 text-right'>Debit</th>
//            <th class='w-12 text-right'>Credit</th>
//            <th class='w-7 text-center'>Rate</th>
//            <th class='w-7 text-center'>Days</th>
//            <th class='w-12 text-right'>Accrued interest</th>
//          </tr>
//        </thead>
//      </table>
//     <div class='pt-0 overflow-auto'>
//        <table id='MCATransactionTable' class='MCATransactionTable-W mca_custom_table' style='width: 100%'>
//          <tbody>
//            <tr class='ht-20'>
//              <td class='w-15 text-center'>02/05/2020</td>
//              <td class='w-35 text-left'>BROUGHT FORWARD</td>
//              <td class='w-12 text-right'></td>
//              <td class='w-12 text-right'>50 606.44</td>
//              <td class='w-7 text-center'>0.03</td>
//              <td class='w-7 text-center'></td>
//              <td class='w-12 text-right'>0.04</td>
//            </tr>
//            <tr class='ht-20'>
//              <td class='w-15 text-center'>08/05/2020</td>
//              <td class='w-35 text-left'>INTEREST ACCRUED ON 50606.64 CR FROM 2020-05-02 TO 2020-05-08</td>
//              <td class='w-12 text-right'></td>
//              <td class='w-12 text-right'></td>
//              <td class='w-7 text-center'>0.03</td>
//              <td class='w-7 text-center'>7</td>
//              <td class='w-12 text-right'>0.28</td>
//            </tr>
//            <tr class='ht-20'>
//              <td class='w-15 text-center'>09/05/2020</td>
//              <td class='w-35 text-left'>CARRIED FORWARD</td>
//              <td class='w-12 text-right'></td>
//              <td class='w-12 text-right'>50 606.64</td>
//              <td class='w-7 text-center'>0.03</td>
//              <td class='w-7 text-center'></td>
//              <td class='w-12 text-right'>0.32</td>
//            </tr>
//            <tr class='ht-20'>
//              <td class='w-15 text-center'>10/05/2020</td>
//              <td class='w-35 text-left'>BROUGHT FORWARD</td>
//              <td class='w-12 text-right'></td>
//              <td class='w-12 text-right'>50 606.44</td>
//              <td class='w-7 text-center'>0.03</td>
//              <td class='w-7 text-center'></td>
//              <td class='w-12 text-right'>0.04</td>
//            </tr>
//          </tbody>
//         </table>
//        </div>
//      </div>
//    </div>
//  </div>`
//})
//export class WealthMCATransactionComponent {
//  @Input()
//  widgetsGridsterItemArray: any[] = [];
//}

//// Component Created for MCA VAT Analysis for Widget -- Nedbank
//@Component({
//  selector: 'WealthMCAVATAnalysis',
//  template: `<div class="widget">
//    <div class="widget-area height100">
//     <div class='pt-1'>
//      <table class='MCATransactionTable-W mca_small_table' style='width: 40%'>
//        <thead>
//          <tr class='ht-30'>
//            <th class='ip-w-25 text-left'>From</th>
//            <th class='ip-w-25 text-left'>To</th>
//            <th class='ip-w-25 text-right'>Rate</th>
//            <th class='ip-w-25 text-right'>Amount</th>
//          </tr>
//        </thead>
//      </table>
//     <div class='pt-0 overflow-auto'>
//        <table id='MCATransactionTable' class='MCATransactionTable-W mca_small_table' style='width: 40%'>
//          <tbody>
//            <tr class='ht-20'>
//              <td class='ip-w-25 text-left'>02/05/2020</td>
//              <td class='ip-w-25 text-left'>02/05/2020</td>
//              <td class='ip-w-25 text-right'>0.3</td>
//              <td class='ip-w-25 text-right'>50 606.44</td>
//            </tr>
//          </tbody>
//         </table>
//        </div>
//      </div>
//    </div>
//  </div>`
//})
//export class WealthMCAVATAnalysisComponent {
//  @Input()
//  widgetsGridsterItemArray: any[] = [];
//}

//// Component Created for Bank Details Widget -- Nedbank
//@Component({
//  selector: 'WealthMCABranchDetails',
//  template: `<div class="widget">
//    <div class="widget-area height100">
//      <div class='card border-0'>
//          <div class="card-body BranchDetails">
//              BankName<br>Address Line1, City, ZipCode<br>Address Line2, City, ZipCode<br>Country Name
//              <br>Bank VAT Reg No XXXXXXXXXX
//          </div>
//          <div class="ConactCenterDiv text-success-w float-right pt-3">
//              Nedbank Private Wealth Service Suite: XXXX XXX XXX
//          </div>
//      </div>
//    </div>
//  </div>`
//})
//export class WealthMCABranchDetailsComponent {
//  @Input()
//  widgetsGridsterItemArray: any[] = [];
//}
