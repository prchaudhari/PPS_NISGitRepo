import { Component, OnInit, Injector, ChangeDetectorRef, ViewChild, OnDestroy } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { FormGroup, FormBuilder } from '@angular/forms';
import { NgxUiLoaderService } from 'ngx-ui-loader';
import { Router, NavigationEnd } from '@angular/router';
import { Constants, ErrorMessageConstants } from 'src/app/shared/constants/constants';
import { MessageDialogService } from 'src/app/shared/services/mesage-dialog.service';
import { LocalStorageService } from 'src/app/shared/services/local-storage.service';
import { Widget } from '../widget';
import { WidgetService } from '../widget.service';
import { TemplateService } from '../../template/template.service';

@Component({
  selector: 'app-add',
  templateUrl: './add.component.html',
  styleUrls: ['./add.component.scss']
})
export class AddComponent implements OnInit {

  //select widget type radio
  public widgetList: Widget[] = [];
  public selectedLink: Widget;
  setWidgetType(e): void {
    this.selectedLink = e;
  }
  public pageTypeList = [];
  constructor(private injector: Injector,
    private fb: FormBuilder,
    private _messageDialogService: MessageDialogService,
    private router: Router,
    private localstorageservice: LocalStorageService,
    private spinner: NgxUiLoaderService,
    private widgetService: WidgetService) {

  }

  ngOnInit() {
    this.getPageTypes();
    this.getWidgetRecords(null);
  }
  async getPageTypes() {
    let templateService = this.injector.get(TemplateService);
    this.pageTypeList = [{ "Identifier": 0, "PageTypeName": "Select Page Type" }];
    let list = await templateService.getPageTypes();
    if (this.pageTypeList.length == 0) {
      let message = ErrorMessageConstants.getNoRecordFoundMessage;
      this._messageDialogService.openDialogBox('Error', message, Constants.msgBoxError).subscribe(data => {
        if (data == true) {
          this.getPageTypes();
        }
      });
    } else {

      this.pageTypeList = [...this.pageTypeList, ...list];
    }
  }
  async getWidgetRecords(searchParameter) {
    //this.spinner.start();
    let widgetService = this.injector.get(WidgetService);
    if (searchParameter == null) {
      searchParameter = {};
      searchParameter.PagingParameter = {};
      searchParameter.PagingParameter.PageIndex = Constants.DefaultPageIndex;
      searchParameter.PagingParameter.PageSize = Constants.DefaultPageSize;
      searchParameter.SortParameter = {};
      searchParameter.SortParameter.SortColumn = "WidgetName";
      searchParameter.SortParameter.SortOrder = Constants.Ascending;
      searchParameter.SearchMode = Constants.Contains;
      searchParameter.IsPageTypeDetailsRequired = true;
    }
    this.widgetList = await widgetService.getWidget(searchParameter);

    if (this.widgetList.length != 0) {
      this.widgetList[0].Checked = true;
      this.selectedLink = this.widgetList[0];

      for (var i = 0; i < this.widgetList.length; i++) {
        if (this.widgetList[i].WidgetName == "CustomerInformation") {
          this.widgetList[i].ImageSource = "assets/images/CustomerInfoWidget.PNG";
          this.widgetList[i].WidgetIcon = "fa fa-address-book-o";
        }
        else if (this.widgetList[i].WidgetName == "AccountInformation") {
          this.widgetList[i].ImageSource = "assets/images/AccountInfoWidget.PNG"
          this.widgetList[i].WidgetIcon = "fa fa-address-card-o";

        }
        else if (this.widgetList[i].WidgetName == "Summary") {
          this.widgetList[i].ImageSource = "assets/images/SummaryWidget.PNG"
          this.widgetList[i].WidgetIcon = "icon-widget-single-column2";

        }
        else if (this.widgetList[i].WidgetName == "Image") {
          this.widgetList[i].ImageSource = "assets/images/icon-image.png"
          this.widgetList[i].WidgetIcon = "fa fa-image";

        }
        else if (this.widgetList[i].WidgetName == "Video") {
          this.widgetList[i].ImageSource = "assets/images/VideoPlaceholder.jpg"
          this.widgetList[i].WidgetIcon = "icon-videoWidget";

        }
        else if (this.widgetList[i].WidgetName == "Analytics") {
          this.widgetList[i].ImageSource = "assets/images/Analytics.png"
          this.widgetList[i].WidgetIcon = "icon-AnalyticsWidget";

        }
        else if (this.widgetList[i].WidgetName == "SavingTransaction") {
          this.widgetList[i].ImageSource = "assets/images/TranscationListWidget.PNG"
          this.widgetList[i].WidgetIcon = "icon-widget-multi-column2";

        }
        else if (this.widgetList[i].WidgetName == "CurrentTransaction") {
          this.widgetList[i].ImageSource = "assets/images/TranscationListWidget.PNG"
          this.widgetList[i].WidgetIcon = "icon-widget-multi-column2";

        }
        else if (this.widgetList[i].WidgetName == "SavingTrend") {
          this.widgetList[i].ImageSource = "assets/images/SavingTrendWidget.PNG"
          this.widgetList[i].WidgetIcon = "icon-widget-line-chart2";

        }
        else if (this.widgetList[i].WidgetName == "Top4IncomeSources") {
          this.widgetList[i].ImageSource = "assets/images/Top4IncomeWidget.PNG"
          this.widgetList[i].WidgetIcon = "icon-rp-quality1";

        }
        else if (this.widgetList[i].WidgetName == "CurrentAvailableBalance") {
          this.widgetList[i].ImageSource = "assets/images/AvailableBalanceWidget.PNG"
          this.widgetList[i].WidgetIcon = "icon-AvailableBalanceWidget";

        }
        else if (this.widgetList[i].WidgetName == "SavingAvailableBalance") {
          this.widgetList[i].ImageSource = "assets/images/AvailableBalanceWidget.PNG"
          this.widgetList[i].WidgetIcon = "icon-AvailableBalanceWidget";

        }
        else if (this.widgetList[i].WidgetName == "SpendingTrend") {
          this.widgetList[i].ImageSource = "assets/images/SpendingTrendWidget.PNG"
          this.widgetList[i].WidgetIcon = "icon-rp-production1";

        }
        else if (this.widgetList[i].WidgetName == "ReminderaAndRecommendation") {
          this.widgetList[i].ImageSource = "assets/images/ReminderWidget.PNG"
          this.widgetList[i].WidgetIcon = "fa fa-bell-o";

        }
      }
    }



    // this.spinner.stop();
  }


  public onPageTypeSelected(event) {
    var searchParameter: any = {};
    searchParameter.PagingParameter = {};
    searchParameter.PagingParameter.PageIndex = Constants.DefaultPageIndex;
    searchParameter.PagingParameter.PageSize = Constants.DefaultPageSize;
    searchParameter.SortParameter = {};
    searchParameter.SortParameter.SortColumn = "WidgetName";
    searchParameter.SortParameter.SortOrder = Constants.Ascending;
    searchParameter.SearchMode = Constants.Contains;
    searchParameter.IsPageTypeDetailsRequired = true;

    const value = event.target.value;
    if (value == "0") {
      searchParameter.PageTypeId = "";
    }
    else {
      searchParameter.PageTypeId = value;

    }

    this.getWidgetRecords(searchParameter);
  }
}
