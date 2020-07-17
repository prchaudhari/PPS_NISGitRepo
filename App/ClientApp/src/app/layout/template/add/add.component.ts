import { Component, OnInit, Injector, ChangeDetectorRef } from '@angular/core';
import { Location } from '@angular/common';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';
import { Constants } from 'src/app/shared/constants/constants';
import { ActivatedRoute, Router, NavigationEnd } from '@angular/router';
import { NgxUiLoaderService } from 'ngx-ui-loader';
import { MessageDialogService } from 'src/app/shared/services/mesage-dialog.service';
import { DialogService } from '@tomblue/ng2-bootstrap-modal';
import { LocalStorageService } from 'src/app/shared/services/local-storage.service';
import { TemplateService } from '../template.service';
import { ErrorMessageConstants } from 'src/app/shared/constants/constants';

@Component({
  selector: 'app-add',
  templateUrl: './add.component.html',
  styleUrls: ['./add.component.scss']
})

export class AddComponent implements OnInit {
    
  public pageFormGroup: FormGroup;
  public pageTypelist: any[] = [];
  public onlyAlphabetsWithSpace = '[a-zA-Z ]*';
  public onlyAlphabetswithInbetweenSpaceUpto50Characters = Constants.onlyAlphabetswithInbetweenSpaceUpto50Characters;
  
  public pageEditModeOn: boolean = false;
  public params: any = {};
  public PageIdentifier = 0;
  public PageName;
  public PageTypeId =0;
  public PageTypeName;
  public PageWidgetArrayString: string = "";
  public PageTypeIdBeforeChange: number = 0;
  public PageWidgetArrayStringBeforePageTypeChange: string = "";
  public isPageTypeChangeActionByClickingPreviousButton: boolean = false;

  public pageFormErrorObject: any = {
    showPageNameError: false,
    showPageTypeError: false
  };

  //getters of page Form group
  get pageName() {
    return this.pageFormGroup.get('pageName');
  }
  get pageType() {
    return this.pageFormGroup.get('pageType');
  }

  get pf() {
    return this.pageFormGroup.controls;
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
      private formbuilder: FormBuilder,
      private injector: Injector,
      private _dialogService: DialogService,
      private uiLoader: NgxUiLoaderService,
      private _messageDialogService: MessageDialogService,
      private router: Router,
      private localstorageservice: LocalStorageService ) 
      {

        if (localStorage.getItem("pageparams")) {
          this.pageEditModeOn = true;
        } else {
          this.pageEditModeOn = false;
        }

        router.events.subscribe(e => {
          if (e instanceof NavigationEnd) {
              if (e.url.includes('/pages')) {
                  //set passing parameters to localstorage.
                  if (localStorage.getItem('pageparams')) {
                      this.params = JSON.parse(localStorage.getItem('pageparams'));
                      this.PageIdentifier = this.params.Routeparams.passingparams.PageIdentifier
                      this.PageName = this.params.Routeparams.filteredparams.PageName
                  }
                  if (localStorage.getItem('pageAddRouteparams')) {
                    this.params = JSON.parse(localStorage.getItem('pageAddRouteparams'));
                    this.PageName = this.params.Routeparams.passingparams.PageName
                    this.PageTypeId = this.params.Routeparams.passingparams.PageTypeId
                    this.PageTypeIdBeforeChange = this.params.Routeparams.passingparams.PageTypeId
                    this.PageWidgetArrayString = this.params.Routeparams.passingparams.PageWidgetArray
                    this.PageWidgetArrayStringBeforePageTypeChange = this.params.Routeparams.passingparams.PageWidgetArray
                    this.isPageTypeChangeActionByClickingPreviousButton = true;
                }
                if (localStorage.getItem('pageEditRouteparams')) {
                  this.params = JSON.parse(localStorage.getItem('pageEditRouteparams'));
                  this.PageName = this.params.Routeparams.passingparams.PageName
                  this.PageTypeId = this.params.Routeparams.passingparams.PageTypeId
                  this.PageIdentifier = this.params.Routeparams.passingparams.PageIdentifier
                  this.pageEditModeOn = this.params.Routeparams.passingparams.pageEditModeOn
              }
              }
              else {
                localStorage.removeItem("pageparams");
                localStorage.removeItem("pageAddRouteparams");
                localStorage.removeItem("pageEditRouteparams");
              }
          }
        });
      }

    //initialization call
    ngOnInit() {
      //Validations for Page Form.
      this.pageFormGroup = this.formbuilder.group({
          pageName: [null, Validators.compose([Validators.required, Validators.minLength(Constants.inputMinLenth),
            Validators.maxLength(Constants.inputMaxLenth), Validators.pattern(this.onlyAlphabetswithInbetweenSpaceUpto50Characters)])],
          pageType: [0, Validators.compose([Validators.required])],
      });

      if(this.PageName != null && this.PageTypeId != null) {
        this.pageFormGroup.patchValue({
          pageName: this.PageName,
          pageType: this.PageTypeId
        });
      }
      this.getPageTypes();
    }

    saveBtnValidation(): boolean {
      if (this.pageFormGroup.controls.pageName.invalid) {
          return true;
      }
      if (this.PageTypeId == 0) {
        return true;
      }
      return false; 
    }

    OnSaveBtnClicked() {
      let pageObject: any = {};
        pageObject.DisplayName = this.pageFormGroup.value.pageName;
        pageObject.PageTypeId = this.PageTypeId;
        if(this.pageEditModeOn) {
          pageObject.Identifier = this.PageIdentifier
        }
        pageObject.PageWidgets = [];
        this.saveTemplate(pageObject);
    }

  //method written to save role
  async saveTemplate(pageObject) {
      this.uiLoader.start();
      let pageArray = [];
      pageArray.push(pageObject);
      let templateService = this.injector.get(TemplateService);
      let isRecordSaved = await templateService.saveTemplate(pageArray, this.pageEditModeOn);
      this.uiLoader.stop();
      if (isRecordSaved) {
          let message = Constants.recordAddedMessage;
          if (this.pageEditModeOn) {
              message = Constants.recordUpdatedMessage;
          }
          this._messageDialogService.openDialogBox('Success', message, Constants.msgBoxSuccess);
          this.navigateToListPage()
      }
  }

    public onPageTypeSelected(event) {
      const value = event.target.value;
      if (value == "0") {
        this.pageFormErrorObject.showPageTypeError = true;
        this.PageTypeId = 0;
        this.PageTypeName = '';
      }
      else {
        if(this.isPageTypeChangeActionByClickingPreviousButton == true && Number(value) != this.PageTypeIdBeforeChange) {
          let message = "This page type change action will discard all page widget configuration. \nAre you sure, you want to change page type?";
          this._messageDialogService.openConfirmationDialogBox('Confirm', message, Constants.msgBoxWarning).subscribe(async (isConfirmed) => {
              if (isConfirmed) {
                this.pageFormErrorObject.showPageTypeError = false;
                this.PageTypeId = Number(value);
                let pageTypeObj = this.pageTypelist.find(s => s.Identifier == value);
                this.PageTypeName = pageTypeObj.Name;
                this.PageWidgetArrayString = "";
              }else {
                this.PageTypeId = this.PageTypeIdBeforeChange;
                this.PageWidgetArrayString = this.PageWidgetArrayStringBeforePageTypeChange;
                this.pageFormGroup.patchValue({
                  pageType: this.PageTypeId
                });
              }
          });
        }else {
          this.PageWidgetArrayString = this.PageWidgetArrayStringBeforePageTypeChange;
          this.pageFormErrorObject.showPageTypeError = false;
          this.PageTypeId = Number(value);
          let pageTypeObj = this.pageTypelist.find(s => s.Identifier == value);
          this.PageTypeName = pageTypeObj.Name;
        }
      }
    }

    navigateToListPage() {
      const router = this.injector.get(Router);
      router.navigate(['pages']);
    }

    navigationTodashboardDesigner() {
      let queryParams = {
        Routeparams: {
          passingparams: {
            "PageName": this.pageFormGroup.value.pageName,
            "PageTypeId": this.PageTypeId,
            "PageTypeName": this.PageTypeName,
            "pageEditModeOn": this.pageEditModeOn,
            "PageIdentifier": this.pageEditModeOn ? this.PageIdentifier : 0,
            "PageWidgetArrayString": this.PageWidgetArrayString
          }
        }
      }
      localStorage.setItem(this.pageEditModeOn ? "pageDesignEditRouteparams" : "pageDesignRouteparams", JSON.stringify(queryParams));
      this.router.navigate(['../dashboardDesigner']);
    }

    async getPageTypes() {
      let templateService = this.injector.get(TemplateService);
      this.pageTypelist = [{ "Identifier": 0, "PageTypeName": "Select Page Type"}];
      let list = await templateService.getPageTypes();
        if (this.pageTypelist.length == 0) {
            let message = ErrorMessageConstants.getNoRecordFoundMessage;
            this._messageDialogService.openDialogBox('Error', message, Constants.msgBoxError).subscribe(data => {
                if (data == true) {
                    this.getPageTypes();
                }
            });
        } else {
          this.pageTypelist = [...this.pageTypelist, ...list];
        }
    }

}
