import { Component, OnInit, Injector, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';
import { NgxUiLoaderService } from 'ngx-ui-loader';
import { Router, NavigationEnd } from '@angular/router';
import { Constants } from 'src/app/shared/constants/constants';
import { TenantConfigurationService } from '../tenant-configuration/tenantConfiguration.service';
import { HttpClient, HttpEvent, HttpEventType } from '@angular/common/http';
import { MessageDialogService } from 'src/app/shared/services/mesage-dialog.service';
import { ConfigConstants } from 'src/app/shared/constants/configConstants';
import { TenantConfiguration } from '../tenant-configuration/tenatconfiguration';
import { DialogService } from '@tomblue/ng2-bootstrap-modal';
import * as $ from 'jquery';
import { HttpClientService } from 'src/app/core/services/httpClient.service';

@Component({
  selector: 'app-theme-configuration',
  templateUrl: './theme-configuration.component.html',
  styleUrls: ['./theme-configuration.component.scss']
})
export class ThemeConfigurationComponent implements OnInit {

  public tenantThemeConfigurationForm: FormGroup;
  public isDefault: boolean = true;
  public isWidgetTheme: boolean = false;

  public isTheme1Active: boolean = false;
  public isTheme2Active: boolean = false;
  public isTheme3Active: boolean = false;
  public isTheme4Active: boolean = false;
  public isTheme5Active: boolean = false;
  public isTheme0Active: boolean = true;

  public ApplicationThemeName = '';
  public WidgetColorThemeName = '';
  public widgetThemeSetting: any = {};
  public weightTitleFont;
  public typeTitleFont;
  public weightHeaderLabelFont;
  public typeHeaderLabelFont;
  public weightDataLabelFont;
  public typeDataLabelFont;
  public baseURL: string = ConfigConstants.BaseURL;
  public TenantConfiguration:any = {};
  public fontWeightArr = [{Id: 0, Name: 'Select', Value: 'Select'}, {Id: 1, Name: 'Normal', Value: 'Normal'}, {Id: 2, Name: 'Italic', Value: 'Italic'}, {Id: 3, Name: 'Bold', Value: 'Bold'}];
  public fontTypeArr = [{Id: 0, Name: 'Select', Value: 'Select'}, {Id: 1, Name: 'Serif', Value: 'Serif'}, {Id: 2, Name: 'Tahoma', Value: 'Tahoma'}, {Id: 3, Name: 'Times Roman', Value: 'Times Roman'}];
  
  constructor(private formBuilder: FormBuilder,
    private spinner: NgxUiLoaderService,
    private router: Router,
    private _http: HttpClient,
    private tenantConfigurationService: TenantConfigurationService,
    private _messageDialogService: MessageDialogService,
    private injector: Injector) { }

  //Title
  get TitleFontColor() {
    return this.tenantThemeConfigurationForm.get('TitleFontColor');
  }

  get TitleFontSize() {
    return this.tenantThemeConfigurationForm.get('TitleFontSize');
  }

  get TitleFontWeight() {
    return this.tenantThemeConfigurationForm.get('TitleFontWeight');
  }

  get TitleFontType() {
    return this.tenantThemeConfigurationForm.get('TitleFontType');
  }

  //Header label
  get HeaderLabelFontColor() {
    return this.tenantThemeConfigurationForm.get('HeaderLabelFontColor');
  }

  get HeaderLabelFontSize() {
    return this.tenantThemeConfigurationForm.get('HeaderLabelFontSize');
  }

  get HeaderLabelFontWeight() {
    return this.tenantThemeConfigurationForm.get('HeaderLabelFontWeight');
  }

  get HeaderLabelFontType() {
    return this.tenantThemeConfigurationForm.get('HeaderLabelFontType');
  }

  //Data label
  get DataLabelFontColor() {
    return this.tenantThemeConfigurationForm.get('DataLabelFontColor');
  }

  get DataLabelFontSize() {
    return this.tenantThemeConfigurationForm.get('DataLabelFontSize');
  }

  get DataLabelFontWeight() {
    return this.tenantThemeConfigurationForm.get('DataLabelFontWeight');
  }

  get DataLabelFontType() {
    return this.tenantThemeConfigurationForm.get('DataLabelFontType');
  }

  ngOnInit() {

    this.tenantThemeConfigurationForm = this.formBuilder.group({
      TitleFontColor: ['#000000'],
      TitleFontSize: [16],
      TitleFontWeight: ['Bold'],
      TitleFontType: ['Select'],

      HeaderLabelFontColor: ['#000000'],
      HeaderLabelFontSize: [14],
      HeaderLabelFontWeight: ['Bold'],
      HeaderLabelFontType: ['Select'],

      DataLabelFontColor: ['#000000'],
      DataLabelFontSize: [12],
      DataLabelFontWeight: ['Normal'],
      DataLabelFontType: ['Select'],
    });

    this.getTenantConfigurationDetails();
  }

  async getTenantConfigurationDetails() {
    this.spinner.start();
    var searchParameter;
    this._http.post(this.baseURL + 'TenantConfiguration/list', searchParameter).subscribe(
      data => {
        let response = <TenantConfiguration>data[0];
        if(response != undefined) {
          this.TenantConfiguration = response;
          this.ApplicationThemeName = this.TenantConfiguration.ApplicationTheme != null ? this.TenantConfiguration.ApplicationTheme.toLocaleLowerCase() : 'theme0';
          switch (this.ApplicationThemeName) {
            case 'theme1':
              this.theme1();
              break;
            case 'theme2':
              this.theme2();
              break;
            case 'theme3':
              this.theme3();
              break;
            case 'theme4':
              this.theme4();
              break;
            case 'theme5':
              this.theme5();
              break;
            default:
              this.theme0();
              break;
          }

          if(this.TenantConfiguration.WidgetThemeSetting != null && this.TenantConfiguration.WidgetThemeSetting != '') {
            this.widgetThemeSetting = JSON.parse(this.TenantConfiguration.WidgetThemeSetting);
            
            this.WidgetColorThemeName = this.widgetThemeSetting.ColorTheme.toLocaleLowerCase();
            if(this.WidgetColorThemeName != '') {
              setTimeout(() => {
                $('#'+this.WidgetColorThemeName).prop('checked', true);
              }, 10);
            }

            this.tenantThemeConfigurationForm.patchValue({
              TitleFontColor : this.widgetThemeSetting.TitleColor != null ? this.widgetThemeSetting.TitleColor : "#000000",
              TitleFontSize : this.widgetThemeSetting.TitleSize != null ? this.widgetThemeSetting.TitleSize : 16,
              TitleFontWeight: this.widgetThemeSetting.TitleWeight != null ? this.widgetThemeSetting.TitleWeight : "Bold",
              TitleFontType: this.widgetThemeSetting.TitleType != null ? this.widgetThemeSetting.TitleType : 'Select',
              HeaderLabelFontColor: this.widgetThemeSetting.HeaderColor != null ? this.widgetThemeSetting.HeaderColor : "#000000",
              HeaderLabelFontSize: this.widgetThemeSetting.HeaderSize != null ? this.widgetThemeSetting.HeaderSize : 14,
              HeaderLabelFontWeight: this.widgetThemeSetting.HeaderWeight != null ? this.widgetThemeSetting.HeaderWeight : "Bold",
              HeaderLabelFontType: this.widgetThemeSetting.HeaderType != null ? this.widgetThemeSetting.HeaderType : 'Select',
              DataLabelFontColor: this.widgetThemeSetting.DataColor != null ? this.widgetThemeSetting.DataColor : "#000000",
              DataLabelFontSize: this.widgetThemeSetting.DataSize != null ? this.widgetThemeSetting.DataSize : 13,
              DataLabelFontWeight: this.widgetThemeSetting.DataWeight != null ? this.widgetThemeSetting.DataWeight : "Normal",
              DataLabelFontType: this.widgetThemeSetting.DataType != null ? this.widgetThemeSetting.DataType : 'Select'
            });

            this.weightTitleFont = this.widgetThemeSetting.TitleWeight != null ? this.widgetThemeSetting.TitleWeight : 16;
            this.typeTitleFont = this.widgetThemeSetting.TitleType != null ? this.widgetThemeSetting.TitleType : 'Select';
            this.weightHeaderLabelFont = this.widgetThemeSetting.HeaderWeight != null ? this.widgetThemeSetting.HeaderWeight : "Bold";
            this.typeHeaderLabelFont = this.widgetThemeSetting.HeaderType != null ? this.widgetThemeSetting.HeaderType : 'Select';
            this.weightDataLabelFont = this.widgetThemeSetting.DataWeight != null ? this.widgetThemeSetting.DataWeight : "Normal";
            this.typeDataLabelFont = this.widgetThemeSetting.DataType != null ? this.widgetThemeSetting.DataType : 'Select';
          }
        }
        
        this.spinner.stop();
      },
      error => {
        $('.overlay').show();
        this._messageDialogService.openDialogBox('Error', error.error.Message, Constants.msgBoxError);
        this.spinner.stop();
      });

  };

  isDefaultClicked() {
    this.isDefault = true;
    this.isWidgetTheme = false;
  }

  isCustomeClicked() {
    this.isDefault = false;
    this.isWidgetTheme = true;
    if(this.WidgetColorThemeName != '') {
      setTimeout(() => {
        $('#'+this.WidgetColorThemeName).prop('checked', true);
      }, 10);
    }
  }

  //Functions call to click the theme of the page--
  theme1() {
    const dom: any = document.querySelector('body');
    dom.classList.add('theme1');
    dom.classList.remove('theme2');
    dom.classList.remove('theme3');
    dom.classList.remove('theme4');
    dom.classList.remove('theme5');
    dom.classList.remove('theme0');
    this.isTheme1Active = true;
    this.isTheme2Active = false;
    this.isTheme3Active = false;
    this.isTheme4Active = false;
    this.isTheme5Active = false;
    this.isTheme0Active = false;
    this.ApplicationThemeName = "Theme1";
  }

  theme2() {
    const dom: any = document.querySelector('body');
    dom.classList.add('theme2');
    dom.classList.remove('theme1');
    dom.classList.remove('theme3');
    dom.classList.remove('theme4');
    dom.classList.remove('theme5');
    dom.classList.remove('theme0');
    this.isTheme1Active = false;
    this.isTheme3Active = false;
    this.isTheme2Active = true;
    this.isTheme4Active = false;
    this.isTheme5Active = false;
    this.isTheme0Active = false;
    this.ApplicationThemeName = "Theme2";
  }

  theme3() {
    const dom: any = document.querySelector('body');
    dom.classList.remove('theme2');
    dom.classList.remove('theme1');
    dom.classList.add('theme3');
    dom.classList.remove('theme4');
    dom.classList.remove('theme5');
    dom.classList.remove('theme0');
    this.isTheme1Active = false;
    this.isTheme3Active = true;
    this.isTheme2Active = false;
    this.isTheme4Active = false;
    this.isTheme5Active = false;
    this.isTheme0Active = false;
    this.ApplicationThemeName = "Theme3";
  }

  theme4() {
    const dom: any = document.querySelector('body');
    dom.classList.remove('theme2');
    dom.classList.remove('theme1');
    dom.classList.remove('theme3');
    dom.classList.add('theme4');
    dom.classList.remove('theme5');
    dom.classList.remove('theme0');
    this.isTheme1Active = false;
    this.isTheme3Active = false;
    this.isTheme2Active = false;
    this.isTheme4Active = true;
    this.isTheme5Active = false;
    this.isTheme0Active = false;
    this.ApplicationThemeName = "Theme4";
  }

  theme5() {
    const dom: any = document.querySelector('body');
    dom.classList.remove('theme2');
    dom.classList.remove('theme1');
    dom.classList.remove('theme3');
    dom.classList.remove('theme4');
    dom.classList.add('theme5');
    dom.classList.remove('theme0');
    this.isTheme1Active = false;
    this.isTheme3Active = false;
    this.isTheme2Active = false;
    this.isTheme4Active = false;
    this.isTheme5Active = true;
    this.isTheme0Active = false;
    this.ApplicationThemeName = "Theme5";
  }

  theme0() {
    const dom: any = document.querySelector('body');
    dom.classList.remove('theme2');
    dom.classList.remove('theme1');
    dom.classList.remove('theme3');
    dom.classList.remove('theme4');
    dom.classList.remove('theme5');
    dom.classList.add('theme0');
    this.isTheme1Active = false;
    this.isTheme3Active = false;
    this.isTheme2Active = false;
    this.isTheme4Active = false;
    this.isTheme5Active = false;
    this.isTheme0Active = true;
    this.ApplicationThemeName = "Theme0";
  }

  widgetColorThemeClicked(theme) {
    this.WidgetColorThemeName = theme;
  }

  titleFontWeightChange(event) {
    const value = event.target.value;
    if (value == "Select") {
      this.weightTitleFont = '';
    }else {
      this.weightTitleFont = value;
    }
  }

  titleFontTypeChange(event) {
    const value = event.target.value;
    if (value == "Select") {
      this.typeTitleFont = '';
    }else {
      this.typeTitleFont = value;
    }
  }

  headerLabelFontWeightChange(event) {
    const value = event.target.value;
    if (value == "Select") {
      this.weightHeaderLabelFont = '';
    }else {
      this.weightHeaderLabelFont = value;
    }
  }

  headerLabelFontTypeChange(event) {
    const value = event.target.value;
    if (value == "Select") {
      this.typeHeaderLabelFont = '';
    }else {
      this.typeHeaderLabelFont = value;
    }
  }

  dataLabelFontWeightChange(event) {
    const value = event.target.value;
    if (value == "Select") {
      this.weightDataLabelFont = '';
    }else {
      this.weightDataLabelFont = value;
    }
  }

  dataLabelFontTypeChange(event) {
    const value = event.target.value;
    if (value == "0") {
      this.typeDataLabelFont = '';
    }else {
      this.typeDataLabelFont = value;
    }
  }

  onSubmit() {
    this.TenantConfiguration.ApplicationTheme = this.ApplicationThemeName;
    this.widgetThemeSetting.ColorTheme = this.WidgetColorThemeName != '' ? this.WidgetColorThemeName : null;
    
    this.widgetThemeSetting.TitleColor = this.tenantThemeConfigurationForm.value.TitleFontColor != undefined ? this.tenantThemeConfigurationForm.value.TitleFontColor : null;
    this.widgetThemeSetting.TitleSize = this.tenantThemeConfigurationForm.value.TitleFontSize != undefined ? this.tenantThemeConfigurationForm.value.TitleFontSize : null;
    this.widgetThemeSetting.TitleWeight = this.weightTitleFont != undefined ? this.weightTitleFont : null;
    this.widgetThemeSetting.TitleType = this.typeTitleFont != undefined ? this.typeTitleFont : null;

    this.widgetThemeSetting.HeaderColor = this.tenantThemeConfigurationForm.value.HeaderLabelFontColor != undefined ? this.tenantThemeConfigurationForm.value.HeaderLabelFontColor : null;
    this.widgetThemeSetting.HeaderSize = this.tenantThemeConfigurationForm.value.HeaderLabelFontSize != undefined ? this.tenantThemeConfigurationForm.value.HeaderLabelFontSize : null;
    this.widgetThemeSetting.HeaderWeight = this.weightHeaderLabelFont != undefined ? this.weightHeaderLabelFont : null;
    this.widgetThemeSetting.HeaderType = this.typeHeaderLabelFont != undefined ? this.typeHeaderLabelFont : null;

    this.widgetThemeSetting.DataColor = this.tenantThemeConfigurationForm.value.DataLabelFontColor != undefined ? this.tenantThemeConfigurationForm.value.DataLabelFontColor : null;
    this.widgetThemeSetting.DataSize = this.tenantThemeConfigurationForm.value.DataLabelFontSize != undefined ? this.tenantThemeConfigurationForm.value.DataLabelFontSize : null;
    this.widgetThemeSetting.DataWeight = this.weightDataLabelFont != undefined ? this.weightDataLabelFont : null;
    this.widgetThemeSetting.DataType = this.typeDataLabelFont != undefined ? this.typeDataLabelFont : null;

    this.TenantConfiguration.WidgetThemeSetting = JSON.stringify(this.widgetThemeSetting);
    this.saveTenantThemeConfigurationRecord(this.TenantConfiguration);
  }

  //Api called here to save tenant application as well as widget theme configuration
  async saveTenantThemeConfigurationRecord(tenantConfiguration) {
    let httpClientService = this.injector.get(HttpClientService);
    let requestUrl = 'TenantConfiguration/Save';
    this.spinner.start();
    var data = [];
    data.push(tenantConfiguration);
    await httpClientService.CallHttp("POST", requestUrl, data).toPromise()
      .then((httpEvent: HttpEvent<any>) => {
        if (httpEvent.type == HttpEventType.Response) {
          this.spinner.stop();
          if (httpEvent["status"] === 200) {
            this._messageDialogService.openDialogBox('Message', "Tenant theme configuration saved successfully. You must need to logout and re-login to reflect new application theme.", Constants.msgBoxSuccess);
          }
        }
      }, (error) => {
        this._messageDialogService.openDialogBox('Error', error.error.Message, Constants.msgBoxError);
        this.spinner.stop();
      });
  }


}
