import { Component, OnInit, Injector, ChangeDetectorRef } from '@angular/core';
import { Location } from '@angular/common';
import { Constants } from 'src/app/shared/constants/constants';
import { FormGroup, FormBuilder, Validators, FormControl, SelectControlValueAccessor, FormArray, ValidatorFn } from '@angular/forms';
import { ActivatedRoute, Router, NavigationEnd } from '@angular/router';
import { NgxUiLoaderService } from 'ngx-ui-loader';
import { MessageDialogService } from 'src/app/shared/services/mesage-dialog.service';
import { DialogService } from '@tomblue/ng2-bootstrap-modal';
import { LocalStorageService } from 'src/app/shared/services/local-storage.service';

@Component({
  selector: 'app-add',
  templateUrl: './add.component.html',
  styleUrls: ['./add.component.scss']
})

export class AddComponent implements OnInit {
    
  public pageFormGroup: FormGroup;
  public pageTypelist: any[] = [];
  public onlyAlphabetsWithSpace = '[a-zA-Z ]*';
  public onlyAlphabetsWithSpaceQuoteHyphen = Constants.onlyAlphabetsWithSpaceQuoteHyphen;
  public pageFormErrorObject: any = {
    showPageTypeError: false,
    showPageNameError: false
  };
  public pageEditModeOn: boolean = false;
  public params: any = {};
  public PageIdentifier = 0;
  public PageName;
  public PageTypeId =0;
  public PageTypeName;

  //getters of usersForm group
  get pageName() {
    return this.pageFormGroup.get('pageName');
  }
  get pageType() {
    return this.pageFormGroup.get('pageType');
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
                  this.params = JSON.parse(localStorage.getItem('pageparams'));
                  if (localStorage.getItem('pageparams')) {
                      this.PageIdentifier = this.params.Routeparams.passingparams.PageIdentifier
                      this.PageName = this.params.Routeparams.filteredparams.PageName
                  }
              } else {
                  localStorage.removeItem("pageparams");
              }
          }
        });
      }

      //custom validation check
      pageFormValidaton(): boolean {
        this.pageFormErrorObject.showPageNameError = false;
        this.pageFormErrorObject.showPageTypeError = false;

        if (this.pageFormGroup.controls.pageName.invalid) {
            this.pageFormErrorObject.showPageNameError = true;
            return false;
        }
        if (this.PageTypeId == 0) {
          this.pageFormErrorObject.showPageTypeError = true;
          return false;
      }

      return true;
    }

    public onPageTypeSelected(event) {
      const value = event.target.value;
      if (value == "0") {
        this.pageFormErrorObject.showPageTypeError = true;
        this.PageTypeId = 0;
        this.PageTypeName = '';
      }
      else {
        this.pageFormErrorObject.showPageTypeError = false;
        this.PageTypeId = Number(value);
        let pageTypeObj = this.pageTypelist.find(s => s.Identifier == value);
        this.PageTypeName = pageTypeObj.Name;
      }
    }

    navigateToListPage() {
        this._location.back();
    }

    navigationTodashboardDesigner() {
      if(this.pageFormValidaton())
      {
        let queryParams = {
          Routeparams: {
            passingparams: {
              "PageName": this.pageFormGroup.value.pageName,
              "PageTypeId": this.PageTypeId,
              "PageTypeName": this.PageTypeName,
              "pageEditModeOn": this.pageEditModeOn
            }
          }
        }
        localStorage.setItem("pageDesignRouteparams", JSON.stringify(queryParams))
        this.router.navigate(['../dashboardDesigner']);
      }
    }

    getPageTypes(){
      this.pageTypelist = [ {"Identifier": 0, "Name": "Select"},{"Identifier": 1, "Name": "Home"}, 
      {"Identifier": 2, "Name": "Saving Account"}, {"Identifier": 3, "Name": "Current Account"} ];
    }

    //initialization call
    ngOnInit() {
      //Validations for Page Form.
      this.pageFormGroup = this.formbuilder.group({
          pageName: [null, Validators.compose([Validators.required, Validators.minLength(2),Validators.maxLength(50),
            Validators.pattern(this.onlyAlphabetsWithSpaceQuoteHyphen)])
          ],
          pageType: [0, Validators.compose([Validators.required])],
      });
      this.getPageTypes();
    }

}
