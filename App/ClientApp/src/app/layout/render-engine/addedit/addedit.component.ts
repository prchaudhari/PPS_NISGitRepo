import { Component, OnInit, ViewChild, Injector } from '@angular/core';
import { Location } from '@angular/common';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';
import { Constants } from 'src/app/shared/constants/constants';
import { RenderengineService } from '../renderengine.service';
import { HttpResponse, HttpEvent, HttpEventType } from '@angular/common/http';
import { HttpClient } from '@angular/common/http';
import { DialogService } from '@tomblue/ng2-bootstrap-modal';
import { MsgBoxComponent } from 'src/app/shared/modules/message/messagebox.component';
import { Router, NavigationEnd } from '@angular/router';
import { NgxUiLoaderService } from 'ngx-ui-loader';
import { MessageDialogService } from 'src/app/shared/services/mesage-dialog.service';
import { LocalStorageService } from 'src/app/shared/services/local-storage.service';
import { ConfigConstants } from 'src/app/shared/constants/configConstants';

@Component({
  selector: 'app-addedit',
  templateUrl: './addedit.component.html',
  styles: []
})
export class AddeditComponent implements OnInit {

  renderEngineForm: FormGroup;
  public validUrlRegexPattern = '^(http(s?):\/\/)?([0-9]{2,3}).([0-9]{2,3}).([0-9]{2,3}).([0-9]{1,3})([:0-9]{3,6})?$';
  public onlyAlphabetswithInbetweenSpaceUpto50Characters = Constants.onlyAlphabetswithInbetweenSpaceUpto50Characters;
  public renderEngineEditModeOn: boolean = false;
  public PriorityLevel: number = 0;
  public ConcurrencyCount: number = 0;
  public params;
  public RenderEngineIdentifier: number = 0;

  // Object created to initlialize the error boolean value.
  public renderEngineFormErrorObject: any = {
    showPriorityLevelError: false,
    showConcurrencyCountError: false,
  };

  //getters of render engine Form group
  get RenderEngineName() {
    return this.renderEngineForm.get('RenderEngineName');
  }
  get RenderEngineURL() {
    return this.renderEngineForm.get('RenderEngineURL');
  }
  get RenderEnginePriorityLevel() {
    return this.renderEngineForm.get('RenderEnginePriorityLevel');
  }
  get RenderEngineConcurrencyCount() {
    return this.renderEngineForm.get('RenderEngineConcurrencyCount');
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
    private renderEngineService: RenderengineService,
    private _dialogService: DialogService,
    private spinner: NgxUiLoaderService,
    private router: Router,
    private _messageDialogService: MessageDialogService,
    private localstorageservice: LocalStorageService,
    private injector: Injector) { 
      router.events.subscribe(e => {
        if (e instanceof NavigationEnd) {
          if (e.url.includes('/renderengines')) {
            //set passing parameters to localstorage.
            if (localStorage.getItem('renderEngineEditRouteparams')) {
              this.params = JSON.parse(localStorage.getItem('renderEngineEditRouteparams'));
              this.RenderEngineIdentifier = this.params.Routeparams.passingparams.RenderEngineIdentifier;
              this.renderEngineEditModeOn = true;
            }else {
              this.renderEngineEditModeOn = false;
            }
          } else {
            localStorage.removeItem("renderEngineEditRouteparams");
          }
        }
      });
    }

  ngOnInit() {
    // Render engine form validations.
    this.renderEngineForm = this.formBuilder.group({
      RenderEngineName: [null, Validators.compose([Validators.required, Validators.minLength(2),
        Validators.maxLength(50), Validators.pattern(this.onlyAlphabetswithInbetweenSpaceUpto50Characters)])],
      RenderEngineURL: ['', Validators.compose([Validators.required, Validators.pattern(this.validUrlRegexPattern)])],
      RenderEnginePriorityLevel: [0, Validators.compose([Validators.required])],
      RenderEngineConcurrencyCount: [0, Validators.compose([Validators.required])]
    });

    if(this.renderEngineEditModeOn && this.RenderEngineIdentifier != 0){
      this.getRenderEngineDetails();
    }
  }

  async getRenderEngineDetails() {
    let searchParameter: any = {};
      searchParameter.PagingParameter = {};
      searchParameter.PagingParameter.PageIndex = Constants.DefaultPageIndex;
      searchParameter.PagingParameter.PageSize = Constants.DefaultPageSize;
      searchParameter.SortParameter = {};
      searchParameter.SortParameter.SortColumn = Constants.Name;
      searchParameter.SortParameter.SortOrder = Constants.Ascending;
      searchParameter.SearchMode = Constants.Exact;
      if (this.RenderEngineIdentifier != null){
        searchParameter.Identifier = this.RenderEngineIdentifier;
      }
      let renderEngineRecords = await this.renderEngineService.getRenderEngines(searchParameter);
      renderEngineRecords.forEach(re => {
        this.renderEngineForm.patchValue({
          RenderEngineName: re.RenderEngineName,
          RenderEngineURL: re.URL,
          RenderEnginePriorityLevel: re.PriorityLevel,
          RenderEngineConcurrencyCount: re.NumberOfThread
        });

        this.PriorityLevel = re.PriorityLevel;
        this.ConcurrencyCount = re.NumberOfThread;
      });
  };

  saveButtonValidation(): boolean {
    if (this.renderEngineForm.controls.RenderEngineName.invalid) {
      return true;
    }
    if (this.renderEngineForm.controls.RenderEngineURL.invalid) {
      return true;
    }
    if(this.ConcurrencyCount == 0) {
      return true;
    }
    if(this.PriorityLevel == 0){
      return true;
    }
    return false;
  }

  onConcurrencyCountSelected(event) {
    const value = event.target.value;
    if (value == "0") {
      this.renderEngineFormErrorObject.showConcurrencyCountError = true;
      this.ConcurrencyCount = 0;
    }
    else {
      this.renderEngineFormErrorObject.showConcurrencyCountError = false;
      this.ConcurrencyCount = Number(value);
    }
  }

  onPriorityLevelSelected(event) {
    const value = event.target.value;
    if (value == "0") {
      this.renderEngineFormErrorObject.showPriorityLevelError = true;
      this.PriorityLevel = 0;
    }
    else {
      this.renderEngineFormErrorObject.showPriorityLevelError = false;
      this.PriorityLevel = Number(value);
    }
  }

  navigateToRenderEngineList() {
    this.router.navigate(['renderengines']);
  }

  onSubmit() {
    let renderEngineObj: any = {
      "RenderEngineName" : this.renderEngineForm.value.RenderEngineName.trim(),
      "URL": this.renderEngineForm.value.RenderEngineURL.trim(),
      "PriorityLevel": this.PriorityLevel,
      "NumberOfThread": this.ConcurrencyCount
    };
    if(this.renderEngineEditModeOn) {
      renderEngineObj.Identifier = this.RenderEngineIdentifier;
    }
    this.saveRenderEngineRecord(renderEngineObj);
  }

  //Api called here to save render engine record
  async saveRenderEngineRecord(renderEngineObj) {
      var RenderEngineArr = [];
      RenderEngineArr.push(renderEngineObj);
      let isRecordSaved = await this.renderEngineService.saveRenderEngine(RenderEngineArr, this.renderEngineEditModeOn);
      if (isRecordSaved) {
          let message = Constants.recordAddedMessage;
          if (this.renderEngineEditModeOn) {
              message = Constants.recordUpdatedMessage;
          }
          this._messageDialogService.openDialogBox('Success', message, Constants.msgBoxSuccess);
          this.navigateToRenderEngineList();
          localStorage.removeItem("renderEngineEditRouteparams");
      }
  }

}
