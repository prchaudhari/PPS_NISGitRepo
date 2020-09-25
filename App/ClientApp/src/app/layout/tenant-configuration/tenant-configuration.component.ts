import { Component, OnInit, ViewChild, Injector } from '@angular/core';
import { Location } from '@angular/common';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';
import { Constants } from 'src/app/shared/constants/constants';
import { TenantConfigurationService } from './tenantConfiguration.service';
import { HttpClient, HttpResponse, HttpEvent, HttpEventType } from '@angular/common/http';
import { DialogService } from '@tomblue/ng2-bootstrap-modal';
import { MsgBoxComponent } from 'src/app/shared/modules/message/messagebox.component';
import { Router, NavigationEnd } from '@angular/router';
import { NgxUiLoaderService } from 'ngx-ui-loader';
import { MessageDialogService } from 'src/app/shared/services/mesage-dialog.service';
import { LocalStorageService } from 'src/app/shared/services/local-storage.service';
import { ConfigConstants } from 'src/app/shared/constants/configConstants';
import { TenantConfiguration } from './tenatconfiguration';
import { HttpClientService } from 'src/app/core/services/httpClient.service';

import * as $ from 'jquery';
@Component({
  selector: 'app-tenant-configuration',
  templateUrl: './tenant-configuration.component.html',
  styleUrls: ['./tenant-configuration.component.scss']
})
export class TenantConfigurationComponent implements OnInit {

  tenantConfigurationForm: FormGroup;
  public validUrlRegexPattern = '^(http(s?):\/\/)?([0-9]{2,3}).([0-9]{2,3}).([0-9]{2,3}).([0-9]{1,3})([:0-9]{3,6})?$';
  public onlyAlphabetswithInbetweenSpaceUpto50Characters = Constants.onlyAlphabetswithInbetweenSpaceUpto50Characters;
  public tenantConfigurationEditModeOn: boolean = false;
  public PriorityLevel: number = 0;
  public ConcurrencyCount: number = 0;
  public params;
  public TenantConfigurationIdentifier: number = 0;
  public baseURL: string = ConfigConstants.BaseURL;
  public AssetPathToolTip: string = '';
  public OutputPathToolTip: string = '';
  public isArchivalPathRequired = false;
  public archivalPathError = false;
  public isArchivalPeriodRequired = false;
  public archivalPeriodError = false;
  // Object created to initlialize the error boolean value.
  public tenantConfigurationFormErrorObject: any = {
    showPriorityLevelError: false,
    showConcurrencyCountError: false,
  };
  public setting: TenantConfiguration;
  //getters of render engine Form group
  get TenantConfigurationName() {
    return this.tenantConfigurationForm.get('TenantConfigurationName');
  }

  get TenantConfigurationAssetPath() {
    return this.tenantConfigurationForm.get('TenantConfigurationAssetPath');
  }
  get TenantConfigurationArchivalPath() {
    return this.tenantConfigurationForm.get('TenantConfigurationArchivalPath');
  }
  get TenantConfigurationDescription() {
    return this.tenantConfigurationForm.get('TenantConfigurationDescription');
  }
  get TenantConfigurationOutputPDFPath() {
    return this.tenantConfigurationForm.get('TenantConfigurationOutputPDFPath');
  }
  get TenantConfigurationOutputHTMLPath() {
    return this.tenantConfigurationForm.get('TenantConfigurationOutputHTMLPath');
  }
  get TenantConfigurationInputDataSourcePath() {
    return this.tenantConfigurationForm.get('TenantConfigurationInputDataSourcePath');
  }

  get TenantConfigurationDateFormat() {
    return this.tenantConfigurationForm.get('TenantConfigurationDateFormat');
  }
  get TenantConfigurationArchivalPeriod() {
    return this.tenantConfigurationForm.get('TenantConfigurationArchivalPeriod');
  }


  //function to validate all fields
  validateAllFormFields(formGroup: FormGroup) {
    Object.keys(formGroup.controls).forEach(field => {
      const control = formGroup.get(field);
      if (control instanceof FormControl) {
        control.markAsTouched({ onlySelf: true });
      } else if (control instanceof FormGroup) {
        this.validateAllFormFields(control);
      }
    });
  }

  constructor(private _location: Location,
    private formBuilder: FormBuilder,
    private http: HttpClient,
    private tenantConfigurationService: TenantConfigurationService,
    private _dialogService: DialogService,
    private spinner: NgxUiLoaderService,
    private router: Router,
    private _messageDialogService: MessageDialogService,
    private localstorageservice: LocalStorageService,
    private _http: HttpClient,
    private injector: Injector) {
    router.events.subscribe(e => {
      if (e instanceof NavigationEnd) {
        if (e.url.includes('/renderengines')) {
          //set passing parameters to localstorage.
          if (localStorage.getItem('tenantConfigurationEditRouteparams')) {
            this.params = JSON.parse(localStorage.getItem('tenantConfigurationEditRouteparams'));
            this.TenantConfigurationIdentifier = this.params.Routeparams.passingparams.TenantConfigurationIdentifier;
            this.tenantConfigurationEditModeOn = true;
          } else {
            this.tenantConfigurationEditModeOn = false;
          }
        } else {
          localStorage.removeItem("tenantConfigurationEditRouteparams");
        }
      }
    });
  }

  ngOnInit() {
    // Render engine form validations.
    this.tenantConfigurationForm = this.formBuilder.group({
      TenantConfigurationName: [null, Validators.compose([Validators.required, ,
      Validators.minLength(Constants.inputMinLenth), Validators.maxLength(Constants.inputMaxLenth),
      Validators.pattern(this.onlyAlphabetswithInbetweenSpaceUpto50Characters)])],
      TenantConfigurationDescription: [null, Validators.compose([])],
      TenantConfigurationOutputPDFPath: [null, Validators.compose([])],
      TenantConfigurationOutputHTMLPath: [null, Validators.compose([])],
      TenantConfigurationInputDataSourcePath: [null, Validators.compose([])],
      TenantConfigurationAssetPath: [null, Validators.compose([])],
      TenantConfigurationArchivalPath: [null, Validators.compose([])],
      TenantConfigurationDateFormat: [0, Validators.compose([])],
      TenantConfigurationArchivalPeriod: [null, Validators.compose([])],
    });
    this.getTenantConfigurationDetails();

  }

  async getTenantConfigurationDetails() {
    this.spinner.start();
    var AssetSearchParameter;
    this._http.post(this.baseURL + 'TenantConfiguration/list', AssetSearchParameter).subscribe(
      data => {
        this.setting = <TenantConfiguration>data[0];
        this.spinner.stop();
        
        this.tenantConfigurationForm.patchValue({
          TenantConfigurationName: this.setting.Name,
          TenantConfigurationDescription: this.setting.Description,
          TenantConfigurationOutputPDFPath: this.setting.OutputPDFPath,
          TenantConfigurationOutputHTMLPath: this.setting.OutputHTMLPath,
          TenantConfigurationInputDataSourcePath: this.setting.InputDataSourcePath,
          TenantConfigurationAssetPath: this.setting.AssetPath,
          TenantConfigurationArchivalPath: this.setting.ArchivalPath,
          TenantConfigurationArchivalPeriod: this.setting.ArchivalPeriod,
          TenantConfigurationDateFormat: this.setting.DateFormat
        });
        if (this.setting.IsAssetPathEditable) {
          this.AssetPathToolTip = "";
        }
        else {
          this.AssetPathToolTip = "If Assets are present in the system then you cannot change asset path";
        }
        if (this.setting.IsOutputHTMLPathEditable) {
          this.OutputPathToolTip = "";
        }
        else {
          this.OutputPathToolTip = "If schedule is executed then you cannot change output HTML or PDF path";
        }
      },
      error => {
        $('.overlay').show();
        this._messageDialogService.openDialogBox('Error', error.error.Message, Constants.msgBoxError);
        this.spinner.stop();
      });

  };

  saveButtonValidation(): boolean {
    if (this.tenantConfigurationForm.controls.TenantConfigurationName.invalid) {
      return true;
    }
    if (this.archivalPathError) {
      return true;
    }
    if (this.archivalPeriodError) {
      return true;
    }

    return false;
  }
  OnArchivalChange() {
    if (this.tenantConfigurationForm.value.TenantConfigurationArchivalPeriod > 0) {
      this.isArchivalPathRequired = true;
      if (this.tenantConfigurationForm.value.TenantConfigurationArchivalPath == null || this.tenantConfigurationForm.value.TenantConfigurationArchivalPath == '') {
        this.archivalPathError = true;
      }
      else {
        this.archivalPathError = false;
      }
    }
    if (this.tenantConfigurationForm.value.TenantConfigurationArchivalPath != null || this.tenantConfigurationForm.value.TenantConfigurationArchivalPath != '') {
      this.isArchivalPeriodRequired = true;
      if (this.tenantConfigurationForm.value.TenantConfigurationArchivalPeriod == 0 || this.tenantConfigurationForm.value.TenantConfigurationArchivalPeriod == null) {
        this.archivalPeriodError = true;
      }
      else {
        this.archivalPeriodError = false;
      }
    }
    if ((this.tenantConfigurationForm.value.TenantConfigurationArchivalPath == null || this.tenantConfigurationForm.value.TenantConfigurationArchivalPath == '')
      && (this.tenantConfigurationForm.value.TenantConfigurationArchivalPeriod == 0 || this.tenantConfigurationForm.value.TenantConfigurationArchivalPeriod == null)) {
      this.archivalPeriodError = false;
      this.archivalPathError = false;
      this.isArchivalPathRequired = false;
      this.isArchivalPeriodRequired = false;
    }
  }
  onConcurrencyCountSelected(event) {
    const value = event.target.value;
    if (value == "0") {
      this.tenantConfigurationFormErrorObject.showConcurrencyCountError = true;
      this.ConcurrencyCount = 0;
    }
    else {
      this.tenantConfigurationFormErrorObject.showConcurrencyCountError = false;
      this.ConcurrencyCount = Number(value);
    }
  }

  onPriorityLevelSelected(event) {
    const value = event.target.value;
    if (value == "0") {
      this.tenantConfigurationFormErrorObject.showPriorityLevelError = true;
      this.PriorityLevel = 0;
    }
    else {
      this.tenantConfigurationFormErrorObject.showPriorityLevelError = false;
      this.PriorityLevel = Number(value);
    }
  }

  navigateToTenantConfigurationList() {
    this.router.navigate(['renderengines']);
  }

  onSubmit() {

    this.setting.Name = this.tenantConfigurationForm.value.TenantConfigurationName;
    this.setting.OutputPDFPath = this.tenantConfigurationForm.value.TenantConfigurationDescription;
    this.setting.Description = this.tenantConfigurationForm.value.TenantConfigurationOutputPDFPath;
    this.setting.OutputHTMLPath = this.tenantConfigurationForm.value.TenantConfigurationOutputHTMLPath;
    this.setting.InputDataSourcePath = this.tenantConfigurationForm.value.TenantConfigurationInputDataSourcePath;
    this.setting.AssetPath = this.tenantConfigurationForm.value.TenantConfigurationAssetPath;
    this.setting.ArchivalPath = this.tenantConfigurationForm.value.TenantConfigurationArchivalPath;
    this.setting.ArchivalPeriod = this.tenantConfigurationForm.value.TenantConfigurationArchivalPeriod;
    this.setting.DateFormat = this.tenantConfigurationForm.value.TenantConfigurationDateFormat;
    this.saveTenantConfigurationRecord(this.setting);
  }

  //Api called here to save render engine record
  async saveTenantConfigurationRecord(tenantConfigurationObj) {
    let httpClientService = this.injector.get(HttpClientService);
    let requestUrl = 'TenantConfiguration/Save';
    this.spinner.start();
    var data = [];

    data.push(tenantConfigurationObj);
    await httpClientService.CallHttp("POST", requestUrl, data).toPromise()
      .then((httpEvent: HttpEvent<any>) => {
        if (httpEvent.type == HttpEventType.Response) {
          this.spinner.stop();
          if (httpEvent["status"] === 200) {
            this._messageDialogService.openDialogBox('Message', "Asset configuration saved successfully", Constants.msgBoxSuccess);
          }

        }
      }, (error) => {
        this._messageDialogService.openDialogBox('Error', error.error.Message, Constants.msgBoxError);

        this.spinner.stop();

      });

  }

}
