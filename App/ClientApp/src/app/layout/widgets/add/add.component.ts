import { Component, OnInit, Injector, ChangeDetectorRef, ViewChild, OnDestroy } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { FormGroup, FormBuilder } from '@angular/forms';
import { NgxUiLoaderService } from 'ngx-ui-loader';
import { Router, NavigationEnd } from '@angular/router';
import { Constants } from 'src/app/shared/constants/constants';
import { MessageDialogService } from 'src/app/shared/services/mesage-dialog.service';
import { LocalStorageService } from 'src/app/shared/services/local-storage.service';
import { Widget } from '../widget';
import { WidgetService } from '../widget.service';
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
  public pageTypeList = [
    { "Name": "Select Page Type", "Identifier": "0" },
    { "Name": "Home", "Identifier": "1" },
    { "Name": "Saving Account", "Identifier": "2" },
    { "Name": "Current Account", "Identifier": "3" }

  ];
  constructor(private injector: Injector,
    private fb: FormBuilder,
    private _messageDialogService: MessageDialogService,
    private router: Router,
    private localstorageservice: LocalStorageService,
    private spinner: NgxUiLoaderService,
    private widgetService: WidgetService) {
    
  }

  ngOnInit() {
    this.getWidgetRecords(null);
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
        this.widgetList[i].ImageSource = "assets/images/ImageWidget.PNG"
        this.widgetList[i].WidgetIcon = "fa fa-image";

      }
      else if (this.widgetList[i].WidgetName == "Video") {
        this.widgetList[i].ImageSource = "assets/images/VideoWidget.PNG"
        this.widgetList[i].WidgetIcon = "icon-videoWidget";

      }
      this.widgetList[0].Checked = true;
      this.selectedLink = this.widgetList[0];

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
