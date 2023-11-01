import { Component, OnInit, Injector, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormControl, FormArray } from '@angular/forms';
import { NgxUiLoaderService } from 'ngx-ui-loader';
import { Router, NavigationEnd } from '@angular/router';
import { Constants } from '../../../shared/constants/constants';
import { ErrorMessageConstants } from '../../../shared/constants/constants';
import { MessageDialogService } from '../../../shared/services/mesage-dialog.service';
import { ScheduleService } from '../schedule.service';
import { Schedule } from '../schedule';
import { Statement } from '../../statement-defination/statement';
import { StatementService } from '../../statement-defination/statement.service';
import { ProductService } from '../../products/product.service';
import { Product } from '../../products/product';
import { User } from '../../users/user';
import * as $ from 'jquery';
import { stringify } from 'querystring';
import { promise } from 'protractor';
import { analyzeFile } from '@angular/compiler';

@Component({
  selector: 'app-add',
  templateUrl: './add.component.html',
  styleUrls: ['./add.component.scss']
})

export class AddComponent implements OnInit {
  // my change
  public selectedProduct: string;
  public statmentSelected = true;
  public i = 0;
  public pageTypeData: any;
  public pageTypeList: any;
  public checkBoxArray = [];
  // my changes
  public scheduleForm: FormGroup;
  public statementDefinitionList = [];
  public onlyAlphabetswithInbetweenSpaceUpto50Characters = Constants.onlyAlphabetswithInbetweenSpaceUpto50Characters;
  public schedule: Schedule;
  public scheduleDetails: Schedule;
  public scheduleRecords: any;
  public isShown: boolean = false;
  public isProductShown: boolean = true;
  public st: Statement;
  public pt: Product;
  public updateOperationMode: boolean;
  public params: any = [];
  public IsEndDateRequired = true;
  public IsExportToPDF = false;
  public filterFromDateError: boolean = false;
  public filterFromDateErrorMessage: string = "";
  public checkBoxError: boolean = false;
  public checkBoxErrorMessage: string = "";
  public filterToDateError: boolean = false;
  public filterToDateErrorMessage: string = "";
  public filterDateDifferenecError: boolean = false;
  public filterDateDiffErrorMessage: string = "";
  public selectedValueForProduct: string = "Please Select";
  public scheduleFormErrorObject: any = {
  };

  public DayOfMonthList: any = [];
  public TimeOfDayHoursList: any = [];
  public TimeOfDayMinutesList: any = [];
  public isFirstimeLoad = false;
  public IsStartDateDisable = false;
  public IsEndDateDisable = false;
  public isDaily: boolean = false;
  public isWeekly: boolean = false;
  public isMonthly: boolean = false;
  public isYearly: boolean = false;
  public isEndDate: boolean = true;
  public isEndAfter: boolean = false;
  public isNoEndDate: boolean = true;

  public ScheduleOccuranceMessage = '';
  public monthArray = ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December'];
  public dayArray = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'];
  public dayObjectArr = [{ Id: 1, 'Day': 'Monday' }, { Id: 2, 'Day': 'Tuesday' }, { Id: 3, 'Day': 'Wednesday' }, { Id: 4, 'Day': 'Thursday' }, { Id: 5, 'Day': 'Friday' }, { Id: 6, 'Day': 'Saturday' }, { Id: 7, 'Day': 'Sunday' }];
  public isCustom: boolean = false;
  public scheduleStartDate = new Date();
  public scheduleEndDate = new Date();
  public tommorrow = new Date();
  public scheduleOccuranceDay = 0;
  public scheduleOccuranceMonth = '';
  public selectedWeekdays = [];
  public DataFormat;
  public IsAnyBatchExecuted: boolean = false;
  public monthlyWarningMessage = '';
  public productList: any = [];
  get ScheduleName() {
    return this.scheduleForm.get('ScheduleName');
  }

  get StatementDefinition() {
    return this.scheduleForm.get('StatementDefinition');
  }

  get DayOfMonth() {
    return this.scheduleForm.get('DayOfMonth');
  }

  get TimeOfDayHours() {
    return this.scheduleForm.get('TimeOfDayHours');
  }

  get Products() {
    return this.scheduleForm.get('StatementDefinition');
  }

  get TimeOfDayMinutes() {
    return this.scheduleForm.get('TimeOfDayMinutes');
  }

  get filtershiftfromdate() {
    return this.scheduleForm.get('filtershiftfromdate');
  }

  get filtershiftenddate() {
    return this.scheduleForm.get('filtershiftenddate');
  }

  //get ExportToPDF() {
  //  return this.scheduleForm.get('ExportToPDF');
  //}

  //get NoEndDate() {
  //  return this.scheduleForm.get('NoEndDate');
  //}

  get RepeatEvery() {
    return this.scheduleForm.get('RepeatEvery');
  }

  get RecurrancePattern() {
    return this.scheduleForm.get('RecurrancePattern');
  }

  get RepeatEveryBy() {
    return this.scheduleForm.get('RepeatEveryBy');
  }

  get CustomYearMonth() {
    return this.scheduleForm.get('CustomYearMonth');
  }

  get CustomYearDay() {
    return this.scheduleForm.get('CustomYearDay');
  }

  get CustomMonthDay() {
    return this.scheduleForm.get('CustomMonthDay');
  }

  get scheduleEndAfterNoOccurences() {
    return this.scheduleForm.get('scheduleEndAfterNoOccurences');
  }

  getDaysInMonth(month, year) {
    return new Date(year, month, 0).getDate();
  }

  get StatementProductBatchName() {
    return this.scheduleForm.get('StatementProductBatchName');
  }

  OnRecurrancePatternChange() {
    let RecurrancePatternVal = this.scheduleForm.value.RecurrancePattern != null ? this.scheduleForm.value.RecurrancePattern : '';
    if (RecurrancePatternVal != '') {
      var d = this.scheduleStartDate;
      var today = d.toLocaleDateString();
      var dte = d.getDate();
      var day = d.getDay();
      var month = d.getMonth();
      this.isCustom = false;
      this.isNoEndDate = false;

      if (this.isEndDate == false && this.isEndAfter == false && RecurrancePatternVal != 'DoesNotRepeat') {
        this.isEndDate = true;
      }

      if (this.isEndDate == true) {
        setTimeout(() => {
          $('#enddate').prop('checked', true);
        }, 10);
      } else {
        setTimeout(() => {
          $('#enddate').prop('checked', false);
        }, 10);
      }

      if (this.isEndAfter == true) {
        setTimeout(() => {
          $('#endafter').prop('checked', true);
        }, 10);
      } else {
        setTimeout(() => {
          $('#endafter').prop('checked', false);
        }, 10);
      }
      if (RecurrancePatternVal == 'Weekday') {
        this.ScheduleOccuranceMessage = 'Occurs every Monday through Friday starting ' + today;
      } else if (RecurrancePatternVal == 'Daily') {
        this.ScheduleOccuranceMessage = 'Occurs every day starting ' + today;
      } else if (RecurrancePatternVal == 'Weekly') {
        this.ScheduleOccuranceMessage = 'Occurs every ' + this.dayArray[day] + ' starting ' + today;
      } else if (RecurrancePatternVal == 'Monthly') {
        this.ScheduleOccuranceMessage = 'Occurs every Month on day ' + dte + ' starting ' + today;
      } else if (RecurrancePatternVal == 'Yearly') {
        this.ScheduleOccuranceMessage = 'Occurs every Year on day ' + dte + ' of ' + this.monthArray[month] + ' starting ' + today;
      } else if (RecurrancePatternVal == 'Custom') {
        this.isCustom = true;
        this.setScheduleOccuranceMessage();
      } else {
        this.isNoEndDate = true;
        this.ScheduleOccuranceMessage = '';
        this.monthlyWarningMessage = '';
      }
    } else {
      this.ScheduleOccuranceMessage = '';
      this.monthlyWarningMessage = '';
    }
  }

  onRepeatEveryByValueChange() {
    this.monthlyWarningMessage = '';
    let repeatEveryByVal = this.scheduleForm.value.RepeatEveryBy != null ? this.scheduleForm.value.RepeatEveryBy : 'Day';
    if (repeatEveryByVal == 'Day') {
      this.isDaily = true;
      this.isWeekly = false;
      this.isMonthly = false;
      this.isYearly = false;
    } else if (repeatEveryByVal == 'Week') {
      this.isDaily = false;
      this.isWeekly = true;
      this.isMonthly = false;
      this.isYearly = false;
    } else if (repeatEveryByVal == 'Month') {
      this.isDaily = false;
      this.isWeekly = false;
      this.isMonthly = true;
      this.isYearly = false;
      if (this.scheduleForm.value.CustomMonthDay >= 29) {
        this.monthlyWarningMessage = 'Some of the months have fewer days so schedule will be executed on the last day of month.';
      }
    } else if (repeatEveryByVal == 'Year') {
      this.isDaily = false;
      this.isWeekly = false;
      this.isMonthly = false;
      this.isYearly = true;
      if (this.scheduleForm.value.CustomYearDay >= 29) {
        this.monthlyWarningMessage = 'Some of the months have fewer days so schedule will be executed on the last day of month.';
      }
    }
    this.setScheduleOccuranceMessage();
  }

  onRepeatEveryChange() {
    if (this.scheduleForm.value.RepeatEvery <= 0) {
      this.scheduleForm.controls['RepeatEvery'].setValue(1);
    }
    this.setScheduleOccuranceMessage();
  }

  onCustomMonthDayChange() {
    if (this.scheduleForm.value.CustomMonthDay <= 0) {
      this.scheduleForm.controls['CustomMonthDay'].setValue(1);
    } else if (this.scheduleForm.value.CustomMonthDay > 31) {
      this.scheduleForm.controls['CustomMonthDay'].setValue(31);
    }
    this.scheduleOccuranceDay = this.scheduleForm.value.CustomMonthDay;
    if (this.scheduleForm.value.CustomMonthDay >= 29) {
      this.monthlyWarningMessage = 'Some of the months have fewer days so schedule will be executed on the last day of month.';
    } else {
      this.monthlyWarningMessage = '';
    }
    this.setScheduleOccuranceMessage();
  }

  onCustomYearDayChange() {
    if (this.scheduleForm.value.CustomYearDay <= 0) {
      this.scheduleForm.controls['CustomYearDay'].setValue(1);
    } else if (this.scheduleForm.value.CustomYearDay > 31) {
      this.scheduleForm.controls['CustomYearDay'].setValue(31);
    } else if (this.scheduleForm.value.CustomYearDay > 29 && this.scheduleForm.value.CustomYearMonth == 'February') {
      this.scheduleForm.controls['CustomYearDay'].setValue(29);
    }
    this.scheduleOccuranceDay = this.scheduleForm.value.CustomYearDay;
    if (this.scheduleForm.value.CustomYearDay >= 29) {
      this.monthlyWarningMessage = 'Some of the months have fewer days so schedule will be executed on the last day of month.';
    } else {
      this.monthlyWarningMessage = '';
    }
    this.setScheduleOccuranceMessage();
  }

  onScheduleEndAfterNoOccurencesChange() {
    if (this.scheduleForm.value.scheduleEndAfterNoOccurences <= 0) {
      this.scheduleForm.controls['scheduleEndAfterNoOccurences'].setValue(1);
    }
    this.setScheduleOccuranceMessage();
  }

  onCustomYearMonthChange() {
    this.scheduleOccuranceMonth = this.scheduleForm.value.CustomYearMonth;
    this.setScheduleOccuranceMessage();
    if (this.scheduleForm.value.CustomYearDay > 29 && this.scheduleForm.value.CustomYearMonth == 'February') {
      this.scheduleForm.controls['CustomYearDay'].setValue(29);
    }
  }

  weekdaySelection(event, day, Id) {
    if (event.target.checked) {
      this.selectedWeekdays.push({ 'Id': Id, 'Day': day });
    } else {
      let index = this.selectedWeekdays.findIndex(x => x.Id == Id);
      this.selectedWeekdays.splice(index, 1);
    }
    this.setScheduleOccuranceMessage();
  }

  setScheduleOccuranceMessage() {
    this.monthlyWarningMessage = '';
    var dt = new Date(this.scheduleStartDate);
    var dayOfMonth = this.schedule.DayOfMonth == undefined || this.schedule.DayOfMonth == 0 ? dt.getDate() : this.schedule.DayOfMonth;
    var ssd = new Date(dt.getFullYear(), dt.getMonth(), dayOfMonth);

    var schedulestartdte = ssd.toLocaleDateString();
    var dte = this.scheduleOccuranceDay != 0 ? this.scheduleOccuranceDay : ssd.getDate();
    var month = this.scheduleOccuranceMonth != '' ? this.scheduleOccuranceMonth : this.monthArray[ssd.getMonth()];
    let RecurrancePatternVal = this.scheduleForm.value.RecurrancePattern != null ? this.scheduleForm.value.RecurrancePattern : '';

    if (RecurrancePatternVal == 'DoesNotRepeat') {
      this.ScheduleOccuranceMessage = 'Occurs once on ' + schedulestartdte;
    } else {
      let scheduleRunUtilMessage = '';
      if (this.isEndDate == true && this.scheduleForm.value.filtershiftenddate != null && this.scheduleForm.value.filtershiftenddate != '') {
        let sed = new Date(this.scheduleForm.value.filtershiftenddate);
        scheduleRunUtilMessage = ' until ' + sed.toLocaleDateString();
      } else if (this.isEndAfter == true && this.scheduleForm.value.scheduleEndAfterNoOccurences != null && this.scheduleForm.value.scheduleEndAfterNoOccurences != 0) {
        scheduleRunUtilMessage = ' upto ' + this.scheduleForm.value.scheduleEndAfterNoOccurences + " occurence.";
      }

      let repeatEvery = this.scheduleForm.value.RepeatEvery != null ? this.scheduleForm.value.RepeatEvery : 1;
      let repeatEveryByVal = this.scheduleForm.value.RepeatEveryBy != null ? this.scheduleForm.value.RepeatEveryBy : 'Day';
      let occurance = '';

      if (repeatEveryByVal == 'Day') {
        if (repeatEvery == 1) {
          occurance = 'day';
        } else {
          occurance = repeatEvery + ' days ';
        }
      }
      else if (repeatEveryByVal == 'Week') {
        var weekdaystr = '';
        if (this.selectedWeekdays.length > 0) {
          if (this.selectedWeekdays.length == 7 && repeatEvery == 1) {
            occurance = 'day';
            for (let i = 0; i < this.selectedWeekdays.length; i++) {
              let day = this.selectedWeekdays[i].Day;
              setTimeout(() => {
                $('#weekday-' + day.toLowerCase()).prop('checked', true);
              }, 10);
            }
          } else {
            this.selectedWeekdays.sort(function (a, b) {
              return a.Id - b.Id;
            });
            for (let i = 0; i < this.selectedWeekdays.length; i++) {
              let day = this.selectedWeekdays[i].Day;
              weekdaystr = weekdaystr + (weekdaystr != '' ? (i == (this.selectedWeekdays.length - 1) ? ' and ' : ', ') : '') + day;
              setTimeout(() => {
                $('#weekday-' + day.toLowerCase()).prop('checked', true);
              }, 10);
            }
            if (repeatEvery == 1) {
              occurance = '' + (weekdaystr != '' ? 'on ' + weekdaystr : ' week');
            } else {
              occurance = repeatEvery + ' weeks ' + (weekdaystr != '' ? 'on ' + weekdaystr : '');
            }
          }
        }
      }
      else if (repeatEveryByVal == 'Month') {
        if (repeatEvery == 1) {
          occurance = 'month on day ' + dte;
        } else {
          occurance = repeatEvery + ' months on day ' + dte;
        }
        this.scheduleForm.get('CustomMonthDay').setValue(dte);
        if (this.scheduleForm.value.CustomMonthDay >= 29) {
          this.monthlyWarningMessage = 'Some of the months have fewer days so schedule will be executed on the last day of month.';
        }
      }
      else if (repeatEveryByVal == 'Year') {
        if (repeatEvery == 1) {
          occurance = 'year on day ' + dte + ' of ' + month;
        } else {
          occurance = repeatEvery + ' years on day ' + dte + ' of ' + month;
        }
        this.scheduleForm.get('CustomYearDay').setValue(dte);
        this.scheduleForm.get('CustomYearMonth').setValue(month);
        if (this.scheduleForm.value.CustomYearDay >= 29) {
          this.monthlyWarningMessage = 'Some of the months have fewer days so schedule will be executed on the last day of month.';
        }
      }
      this.ScheduleOccuranceMessage = 'Occurs every ' + occurance + ' starting ' + schedulestartdte + scheduleRunUtilMessage;
    }
  }

  isEndDateClicked() {
    this.isEndDate = true;
    this.isEndAfter = false;
    this.isNoEndDate = false;
    this.setScheduleOccuranceMessage();
  }

  isEndAfterClicked() {
    this.isEndDate = false;
    this.isEndAfter = true;
    this.isNoEndDate = false;
    this.setScheduleOccuranceMessage();
  }

  constructor(
    private formBuilder: FormBuilder,
    private spinner: NgxUiLoaderService,
    private router: Router,
    private _messageDialogService: MessageDialogService,
    private injector: Injector,
    private productService: ProductService) {
    router.events.subscribe(e => {
      if (e instanceof NavigationEnd) {
        if (e.url.includes('/schedulemanagement/Add')) {
          this.updateOperationMode = false;
          localStorage.removeItem("scheduleparams");
        }
      }
    });

    if (localStorage.getItem("scheduleparams")) {
      this.updateOperationMode = true;
    } else {
      this.updateOperationMode = false;
    }
    this.schedule = new Schedule();
    this.schedule.Statement = new Statement();
    router.events.subscribe(e => {
      if (e instanceof NavigationEnd) {
        if (e.url.includes('/schedulemanagement')) {
          //set passing parameters to localstorage.
          this.params = JSON.parse(localStorage.getItem('scheduleparams'));
          if (localStorage.getItem('scheduleparams')) {
            this.schedule.ProductBatchName = this.params.Routeparams.passingparams.ScheduleProductBatchName;
          }
        } else {
          localStorage.removeItem("scheduleparams");
        }
      }
    });
  }

  ngOnInit() {

    this.tommorrow.setDate(this.tommorrow.getDate() + 1);
    this.tommorrow.setHours(0, 0, 0, 0);
    this.scheduleStartDate = this.tommorrow;

    this.DataFormat = localStorage.getItem('DateFormat');
    this.st = new Statement;
    this.st.Name = "Please Select";
    this.st.Identifier = 0;
    this.statementDefinitionList.push(this.st);
    this.getProducts();
    // this.pt = new Product;
    // this.pt.Name = "Please Select";
    // this.pt.Id = 0;
    // this.productList.push(this.pt);
    var obj = { Identifier: "Please Select", Name: "Please Select" };
    this.DayOfMonthList.push(obj);
    this.TimeOfDayHoursList.push(obj);
    this.TimeOfDayMinutesList.push(obj);

    for (var i = 1; i <= 29; i++) {
      var object = { Identifier: i.toString(), Name: "" };
      if (i < 10) {
        object.Name = "0" + i;
      }
      else {
        object.Name = i.toString();
      }
      this.DayOfMonthList.push(object);
    }

    for (var i = 0; i <= 23; i++) {
      var object = { Identifier: i.toString(), Name: "" };
      if (i < 10) {
        object.Name = "0" + i;
      }
      else {
        object.Name = i.toString();
      }
      this.TimeOfDayHoursList.push(object);
    }

    var x = 0;
    while (x <= 55) {
      var object = { Identifier: x.toString(), Name: "" };
      if (x < 10) {
        object.Name = "0" + x;
      }
      else {
        object.Name = x.toString();
      }
      this.TimeOfDayMinutesList.push(object);
      x = x + 5;
    }

    this.scheduleForm = this.formBuilder.group({
      ScheduleName: ['', Validators.compose([Validators.required, Validators.minLength(2), Validators.maxLength(50),
      Validators.pattern(this.onlyAlphabetswithInbetweenSpaceUpto50Characters)])],
      StatementDefinition: ["Please Select", Validators.compose([Validators.required])],
      //DayOfMonth: ["Please Select", Validators.compose([Validators.required])],
      TimeOfDayHours: ["Please Select", Validators.compose([Validators.required])],
      Products: ["Please Select", Validators.compose([Validators.required])],
      TimeOfDayMinutes: ["Please Select", Validators.compose([Validators.required])],
      filtershiftfromdate: [null, Validators.compose([Validators.required])],
      filtershiftenddate: [null],
      RepeatEvery: [null],
      RecurrancePattern: ["DoesNotRepeat", Validators.compose([Validators.required])],
      RepeatEveryBy: ["Day"],
      CustomYearMonth: [null],
      CustomYearDay: [null],
      CustomMonthDay: [null],
      scheduleEndAfterNoOccurences: [null],
      pagetype: this.formBuilder.array([]),
      StatementProductBatchName: ''
      //ExportToPDF: [false],
      //NoEndDate: [false],
    })

    if (this.updateOperationMode == false) {
      this.scheduleForm.controls['TimeOfDayHours'].setValue(obj.Identifier);
      this.scheduleForm.controls['TimeOfDayMinutes'].setValue(obj.Identifier);
      this.scheduleForm.controls['filtershiftfromdate'].setValue(this.tommorrow); //Default today is the schedule start date
      this.scheduleForm.controls['RepeatEvery'].setValue(1);
      this.scheduleForm.controls['scheduleEndAfterNoOccurences'].setValue(1);
    }

    this.getStatements();
  }

  GetSegmentsByProductFromDB() {
    this.checkBoxArray = [];
    this.pageTypeData = this.productService.getpageTypeByProductID(this.scheduleForm.get('StatementDefinition').value).subscribe(data => {
      this.pageTypeData = data;
      this.pageTypeData.forEach(item => {
        item.StatementViewModel.forEach(subitem => {
          if (this.scheduleRecords != null && this.scheduleRecords.ProductBatches != null && this.scheduleRecords.ProductBatches.length > 0) {
            if (this.scheduleRecords.ProductBatches.find(obj => obj.StatementId == subitem.Identifier)) {
              subitem.IsChecked = true;
              this.checkBoxArray.push(subitem);
            }
            else {
              subitem.IsChecked = false;
            }
          }
        });
      });
    });
  }

  public GetSegmentByProduct(event) {
    const value = event.target.value;
    if (value == "Please Select") {
      this.scheduleFormErrorObject.ProductShowError = true;
      this.scheduleForm.value.StatementDefinition = "Please Select";
      this.pageTypeData = null;
    }
    else {
      this.scheduleFormErrorObject.ProductShowError = false;
      this.scheduleForm.value.StatementDefinition = value;
    }
  }

  makeValidWhenStatementsAreBlack() {
    this.scheduleForm.get('pagetype')['controls'].forEach(page => {
      if (page['controls'].statements.value.length == 0) {
        page['controls'].radioBottonStatements.setErrors(null);
      }
    })
  }
  productTypeChangeToGetPageType(pageTypeData): FormGroup {
    return this.formBuilder.group({
      PageTypeName: [pageTypeData.PageTypeName],
      radioBottonStatements: ['', Validators.required],
      statements: [pageTypeData.StatementViewModel]
    });
  }
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

  async getStatements() {
    let statementService = this.injector.get(StatementService);
    var searchParameter: any = {}
    searchParameter.IsActive = true;
    searchParameter.PagingParameter = {};
    searchParameter.PagingParameter.PageIndex = Constants.DefaultPageIndex;
    searchParameter.PagingParameter.PageSize = Constants.DefaultPageSize;
    searchParameter.SortParameter = {};
    searchParameter.SortParameter.SortColumn = 'CreatedDate';
    searchParameter.SortParameter.SortOrder = Constants.Descending;
    searchParameter.SearchMode = Constants.Contains;
    searchParameter.Status = "Published";
    var response = await statementService.getStatements(searchParameter);
    var list = response.List;
    for (var i = 0; i < list.length; i++) {
      this.statementDefinitionList.push(list[i]);
    }
    if (localStorage.getItem('scheduleparams')) {
      this.getSchedule();
      this.getProducts();
    }
  }

  getProducts() {
    this.productService.getProducts().subscribe(data => {
      this.productList = data;
    });
  }

  async getSchedule() {
    this.isShown = !this.isShown;
    this.isProductShown = !this.isProductShown;
    let scheduleService = this.injector.get(ScheduleService);
    var searchParameter: any = {};
    searchParameter.IsActive = true;
    searchParameter.PagingParameter = {};
    searchParameter.PagingParameter.PageIndex = Constants.DefaultPageIndex;
    searchParameter.PagingParameter.PageSize = Constants.DefaultPageSize;
    searchParameter.SortParameter = {};
    searchParameter.SortParameter.SortColumn = 'Id';
    searchParameter.SortParameter.SortOrder = Constants.Descending;
    searchParameter.SearchMode = Constants.Exact;
    searchParameter.Identifier = this.schedule.Identifier;
    searchParameter.IsSchedulePagesRequired = true;
    searchParameter.ProductBatchName = this.schedule.ProductBatchName;
    var response = await scheduleService.getSchedule(searchParameter);
    if (response.List.length == 0) {
      this._messageDialogService.openDialogBox('Error', "Schedule Not Found", Constants.msgBoxError);
    }
    this.scheduleRecords = response.List[0];
    this.scheduleForm.controls['ScheduleName'].setValue(this.scheduleRecords != null && this.scheduleRecords.ProductBatches.length > 0 ? this.scheduleRecords.ProductBatches[0].ScheduleNameByUser : "");
    this.scheduleForm.get('StatementDefinition').setValue(this.scheduleRecords.ProductId);
    this.scheduleForm.get('StatementProductBatchName').setValue(this.scheduleRecords != null && this.scheduleRecords.ProductBatches.length > 0 ? this.scheduleRecords.ProductBatches[0].ProductBatchName : "");
    this.scheduleForm.controls['TimeOfDayHours'].setValue(this.scheduleRecords != null && this.scheduleRecords.ProductBatches.length > 0 ? this.scheduleRecords.ProductBatches[0].HourOfDay : 0);
    this.scheduleForm.controls['TimeOfDayMinutes'].setValue(this.scheduleRecords != null && this.scheduleRecords.ProductBatches.length > 0 ? this.scheduleRecords.ProductBatches[0].MinuteOfDay : 0);
    this.scheduleForm.controls['filtershiftfromdate'].setValue(this.scheduleRecords != null && this.scheduleRecords.ProductBatches.length > 0 ? new Date(this.scheduleRecords.ProductBatches[0].StartDate) : new Date());
    this.schedule.HourOfDay = this.scheduleRecords != null && this.scheduleRecords.ProductBatches.length > 0 ? this.scheduleRecords.ProductBatches[0].HourOfDay : 0;
    this.schedule.MinuteOfDay = this.scheduleRecords != null && this.scheduleRecords.ProductBatches.length > 0 ? this.scheduleRecords.ProductBatches[0].MinuteOfDay : 0;
    this.scheduleForm.value.RecurrancePattern = this.scheduleRecords != null && this.scheduleRecords.ProductBatches.length > 0 ? this.scheduleRecords.ProductBatches[0].RecurrancePattern : '';
    this.scheduleForm.value.RepeatEveryDayMonWeekYear = this.scheduleRecords != null && this.scheduleRecords.ProductBatches.length > 0 ? this.scheduleRecords.ProductBatches[0].RepeatEveryDayMonWeekYear : '';
    this.scheduleForm.value.WeekDays = this.scheduleRecords != null && this.scheduleRecords.ProductBatches.length > 0 ? this.scheduleRecords.ProductBatches[0].WeekDays : '';
    this.scheduleForm.value.IsEveryWeekDay = this.scheduleRecords != null && this.scheduleRecords.ProductBatches.length > 0 ? this.scheduleRecords.ProductBatches[0].IsEveryWeekDay : '';
    this.scheduleForm.value.MonthOfYear = this.scheduleRecords != null && this.scheduleRecords.ProductBatches.length > 0 ? this.scheduleRecords.ProductBatches[0].MonthOfYear : '';
    this.scheduleForm.value.IsEndsAfterNoOfOccurrences = this.scheduleRecords != null && this.scheduleRecords.ProductBatches.length > 0 ? this.scheduleRecords.ProductBatches[0].IsEndsAfterNoOfOccurrences : '';
    this.scheduleForm.value.NoOfOccurrence = this.scheduleRecords != null && this.scheduleRecords.ProductBatches.length > 0 ? this.scheduleRecords.ProductBatches[0].NoOfOccurrence : '';

    this.scheduleStartDate = new Date(this.scheduleRecords != null && this.scheduleRecords.ProductBatches.length > 0 ? new Date(this.scheduleRecords.ProductBatches[0].StartDate) : new Date());
    var startDate = new Date(this.scheduleRecords != null && this.scheduleRecords.ProductBatches.length > 0 ? new Date(this.scheduleRecords.ProductBatches[0].StartDate) : new Date());
    var endDate = new Date(this.scheduleRecords != null && this.scheduleRecords.ProductBatches.length > 0 ? new Date(this.scheduleRecords.ProductBatches[0].EndDate) : new Date());
    var currentDate = new Date();
    if (startDate.getTime() < currentDate.getTime()) {
      this.IsStartDateDisable = true;
    }
    if (endDate.getTime() < currentDate.getTime()) {
      this.IsEndDateDisable = true;
    }

    if (this.scheduleRecords.ProductBatches.length > 0) {
      if (this.scheduleRecords.ProductBatches[0].EndDate == null || this.scheduleRecords.ProductBatches[0].EndDate.toString() == "0001-01-01T00:00:00") {
        this.IsEndDateRequired = false;
        this.isEndDate = false;
      }
      else {
        this.scheduleForm.controls['filtershiftenddate'].setValue(new Date(this.scheduleRecords.ProductBatches[0].EndDate));
        this.isEndDate = true;
      }
    }

    if (this.scheduleRecords.ProductBatches.length > 0) {
      if (this.scheduleRecords.ProductBatches[0].NoOfOccurrences != null && this.scheduleRecords.ProductBatches[0].NoOfOccurrences != 0) {
        this.scheduleForm.controls['scheduleEndAfterNoOccurences'].setValue(this.scheduleRecords.ProductBatches[0].NoOfOccurrences);
        this.isEndAfter = true;
      }
    }

    if (this.scheduleRecords.ProductBatches.length > 0) {
      let pattern = '';
      let RepeatEveryBy = '';
      if (this.scheduleRecords.ProductBatches[0].RecurrancePattern == null || this.scheduleRecords.ProductBatches[0].RecurrancePattern == '') {
        this.scheduleForm.controls['RecurrancePattern'].setValue("Custom");
        this.scheduleForm.controls['RepeatEveryBy'].setValue("Month");
      } else {
        if (this.scheduleRecords.ProductBatches[0].RecurrancePattern.includes('Custom')) {
          let index = this.scheduleRecords.ProductBatches[0].RecurrancePattern.indexOf('-');
          RepeatEveryBy = this.scheduleRecords.ProductBatches[0].RecurrancePattern.substring(index + 1, this.scheduleRecords.ProductBatches[0].RecurrancePattern.length);
          this.scheduleForm.controls['RepeatEveryBy'].setValue(RepeatEveryBy);
          pattern = this.scheduleRecords.ProductBatches[0].RecurrancePattern.substring(0, index);
          this.scheduleForm.controls['RecurrancePattern'].setValue(pattern);
        } else {
          this.scheduleForm.controls['RecurrancePattern'].setValue(this.scheduleRecords.ProductBatches[0].RecurrancePattern);
        }
      }

      if (this.scheduleRecords.ProductBatches[0].DayOfMonth != null && this.scheduleRecords.ProductBatches[0].DayOfMonth != 0) {
        if (RepeatEveryBy == 'Month') {
          this.scheduleForm.controls['CustomMonthDay'].setValue(this.scheduleRecords.ProductBatches[0].DayOfMonth);
          this.scheduleOccuranceDay = this.scheduleRecords.ProductBatches[0].DayOfMonth;
        } else if (RepeatEveryBy == 'Year') {
          this.scheduleForm.controls['CustomYearDay'].setValue(this.scheduleRecords.ProductBatches[0].DayOfMonth);
          this.scheduleForm.controls['CustomYearMonth'].setValue(this.scheduleRecords.ProductBatches[0].MonthOfYear);
          this.scheduleOccuranceDay = this.scheduleRecords.ProductBatches[0].DayOfMonth;
          this.scheduleOccuranceMonth = this.scheduleRecords.ProductBatches[0].MonthOfYear;
        }
      } else {
        if (RepeatEveryBy == 'Month') {
          this.scheduleForm.controls['CustomMonthDay'].setValue(1);
        } else if (RepeatEveryBy == 'Year') {
          this.scheduleForm.controls['CustomYearDay'].setValue(1);
        }
      }

      if (this.scheduleRecords.ProductBatches[0].RepeatEveryDayMonWeekYear == null || this.scheduleRecords.ProductBatches[0].RepeatEveryDayMonWeekYear == 0) {
        this.scheduleForm.controls['RepeatEvery'].setValue(1);
      } else {
        this.scheduleForm.controls['RepeatEvery'].setValue(this.scheduleRecords.ProductBatches[0].RepeatEveryDayMonWeekYear);
      }

      if (this.scheduleRecords.ProductBatches[0].WeekDays != null && this.scheduleRecords.ProductBatches[0].WeekDays != '') {
        var scheduledays = this.scheduleRecords.ProductBatches[0].WeekDays.split(',');
        scheduledays.forEach(day => {
          var dayObj = this.dayObjectArr.filter(x => x.Day.toLocaleLowerCase() == day.toLocaleLowerCase())[0];
          this.selectedWeekdays.push({ 'Id': dayObj.Id, 'Day': dayObj.Day });
        });
      }

      if (this.scheduleRecords.ProductBatches[0].IsExportToPDF) {
        this.IsExportToPDF = true;
      }
      else {
        this.IsExportToPDF = false;
      }

      this.IsAnyBatchExecuted = this.scheduleRecords.ProductBatches[0].ExecutedBatchCount > 0 ? true : false;
    }
    this.OnRecurrancePatternChange();
    this.onRepeatEveryByValueChange();
    if (this.scheduleRecords != null && this.scheduleRecords.ProductBatches.length > 0) {
      this.GetSegmentsByProductFromDB();
    }
  }

  setStatementsRadioButton() {
    let pagetypeLength = this.scheduleForm.value.pagetype.length;
    for (let i = 0; i < pagetypeLength; i++) {
      if (this.scheduleForm.value.pagetype[i].radioBottonStatements == '') {
        for (let j = 0; j < this.scheduleForm.value.pagetype[i].statements.length; j++) {
          // this.scheduleForm.value.pagetype[i].get('radioBottonStatements').patchValue(90);
          console.log(this.scheduleForm['controls'].pagetype['controls'][j].get('radioBottonStatements').value);
          this.scheduleForm['controls'].pagetype['controls'][j].get('radioBottonStatements').value = 90;
          console.log(this.scheduleForm['controls'].pagetype['controls'][j].get('radioBottonStatements').value);
          // page
          break;
        }
        //isRecordSaved = await scheduleService.saveSchedule(pageArray, this.updateOperationMode);
      }
    }
  }
  public onStateDefinitionSelected(event) {
    const value = event.target.value;
    if (value == "Please select") {
      this.scheduleFormErrorObject.statementShowError = true;
      this.schedule.Statement.Identifier = 0;
      this.selectedProduct = value;
    }
    else {
      this.scheduleFormErrorObject.statementShowError = false;
      this.schedule.Statement.Identifier = Number(value);
      // my Changes
      this.selectedProduct = value;
    }



  }

  public IsExportToPDFClicked(event) {
    const value = event.checked;
    this.IsExportToPDF = value;
  }

  public NoEndDateClicked(event) {
    const value = event.checked;
    this.IsEndDateRequired = !value;
    if (!this.IsEndDateRequired) {
      this.filterToDateError = false;
      this.filterToDateErrorMessage = "";
      this.filterDateDifferenecError = false;
      this.filterDateDiffErrorMessage = "";;
      this.scheduleForm.controls['filtershiftenddate'].setValue("");
    }
  }

  public onDayOfMonthSelected(event) {
    const value = event.target.value;
    if (value == "Please Select") {
      this.scheduleFormErrorObject.dayOfmonthShowError = true;
      this.schedule.DayOfMonth = 0;
    }
    else {
      this.scheduleFormErrorObject.dayOfmonthShowError = false;
      this.schedule.DayOfMonth = Number(value);
    }
  }

  public onTimeOfDayHoursSelected(event) {
    const value = event.target.value;
    if (value == "Please Select") {
      this.scheduleFormErrorObject.hourOfDayShowError = true;
      this.schedule.HourOfDay = 0;
    }
    else {
      this.scheduleFormErrorObject.hourOfDayShowError = false;
      this.schedule.HourOfDay = Number(value);
    }
  }

  public onTimeOfDayMinutesSelected(event) {
    const value = event.target.value;
    if (value == "Please Select") {
      this.scheduleFormErrorObject.minuteOfHourShowError = true;
      this.schedule.MinuteOfDay = 0;
    }
    else {
      this.scheduleFormErrorObject.minuteOfHourShowError = false;
      this.schedule.MinuteOfDay = Number(value);
    }
  }

  onCheckboxError(event) {
    this.checkBoxError = false;
    this.checkBoxErrorMessage = "";
    if (this.checkBoxArray.length == 0) {
      this.checkBoxError = true;
      this.checkBoxErrorMessage = ErrorMessageConstants.getCheckboxErrorMessage;
    }
  }

  onCheckboxChange(statement, event) {
    if (event.target.checked) {
      this.checkBoxArray.push(statement);
    }
    else {
      this.checkBoxArray.forEach((element, index) => {
        if (element.Identifier == statement.Identifier) {
          this.checkBoxArray.splice(index, 1);
        }
      });
    }
  }

  onFilterDateChange(event) {
    this.filterFromDateError = false;
    this.filterToDateError = false;
    this.filterFromDateErrorMessage = "";
    this.filterToDateErrorMessage = "";
    this.filterDateDifferenecError = false;
    this.filterDateDiffErrorMessage = "";
    if (this.scheduleForm.value.filtershiftfromdate != null && this.scheduleForm.value.filtershiftfromdate != '') {
      if (this.IsStartDateDisable == false) {
        let startDate = this.scheduleForm.value.filtershiftfromdate;
        if (startDate.getTime() < this.tommorrow.getTime()) {
          this.filterFromDateError = true;
          this.filterFromDateErrorMessage = ErrorMessageConstants.getStartDateShouldBeDayAfterTodayOrGreaterThanItMessage;
        }
      }
    }
    if (this.scheduleForm.value.filtershiftenddate != null && this.scheduleForm.value.filtershiftenddate != '') {
      let toDate = this.scheduleForm.value.filtershiftenddate;
      if (this.updateOperationMode) {
        var date = new Date(this.schedule.EndDate);
        if (toDate.getTime() < date.getTime()) {
          this.filterToDateError = true;
          this.filterToDateErrorMessage = ErrorMessageConstants.getPreviousEndDateShouldNotBeLessThanEndDateMessage;
        }
      }
    }
    if (this.isEndDate == true) {
      let startDate = this.scheduleForm.value.filtershiftfromdate;
      let toDate = this.scheduleForm.value.filtershiftenddate;
      if (startDate != null && toDate != null) {
        if (this.monthDiff(startDate, toDate) < 30) {
          this.filterDateDifferenecError = true;
          this.filterDateDiffErrorMessage = ErrorMessageConstants.getStartDateAndEndDateShouldhaveMonthDifferenceMessage;
          this.filterToDateError = false;
          this.filterToDateErrorMessage = "";
        }
      }
    }

    this.scheduleStartDate = new Date(this.scheduleForm.value.filtershiftfromdate);
    this.scheduleEndDate = new Date(this.scheduleForm.value.filtershiftenddate);
    this.setScheduleOccuranceMessage();
  }

  monthDiff(d1, d2) {
    return Math.floor((Date.UTC(d2.getFullYear(), d2.getMonth(), d2.getDate())
      - Date.UTC(d1.getFullYear(), d1.getMonth(), d1.getDate())) / (1000 * 60 * 60 * 24));
  }

  vaildateForm() {
    if (this.scheduleForm.invalid)
      return true;
    if (this.scheduleForm.value.StatementDefinition == "Please Select") {
      return true;
    }
    if (this.scheduleForm.value.TimeOfDayHours == "Please Select") {
      return true;
    }
    if (this.scheduleForm.value.TimeOfDayMinutes == "Please Select") {
      return true;
    }
    if (this.scheduleForm.value.StatementDefinition == "0") {
      return true;
    }
    if (this.scheduleForm.value.RecurrancePattern == "Custom" && this.scheduleForm.value.RepeatEvery <= 0) {
      return true;
    }
    if (this.filterFromDateError || this.scheduleForm.value.filtershiftfromdate == "") {
      return true;
    }
    if (this.isEndDate == true && this.scheduleForm.value.RecurrancePattern != "DoesNotRepeat") {
      if (this.filterToDateError || this.scheduleForm.value.filtershiftenddate == "" || this.scheduleForm.value.filtershiftenddate == null) {
        return true;
      }
    }
    if (this.isEndDate == true && this.filterDateDifferenecError) {
      return true;
    }
    if (this.isEndAfter == true && this.scheduleForm.value.scheduleEndAfterNoOccurences <= 0) {
      return true;
    }
    if (this.checkBoxArray.length == 0) {
      return true;
    }

    return false;
  }

  async SaveSchedule() {
    let pageArray = [];
    this.checkBoxArray.forEach(item => {
      this.scheduleDetails = new Schedule();
      this.scheduleDetails.Statement = new Statement();
      this.scheduleDetails.Status = this.schedule.Status;
      this.scheduleDetails.HourOfDay = this.schedule.HourOfDay;
      this.scheduleDetails.MinuteOfDay = this.schedule.MinuteOfDay;
      this.scheduleDetails.ProductId = this.scheduleForm.value.StatementDefinition;
      this.scheduleDetails.ScheduleNameByUser = this.scheduleForm.value.ScheduleName;
      this.scheduleDetails.Name = '';
      this.scheduleDetails.Name = this.scheduleForm.value.ScheduleName + '_' + item.Name;
      this.scheduleDetails.Statement.Identifier = item.Identifier;
      //this.scheduleDetails.StartDate = new Date(this.scheduleForm.value.filtershiftfromdate.getFullYear(), this.scheduleForm.value.filtershiftfromdate.getMonth(), this.scheduleForm.value.filtershiftfromdate.getDate());
      this.scheduleDetails.StartDate = new Date(Date.UTC(this.scheduleForm.value.filtershiftfromdate.getFullYear(), this.scheduleForm.value.filtershiftfromdate.getMonth(), this.scheduleForm.value.filtershiftfromdate.getDate(), this.scheduleForm.value.filtershiftfromdate.getHours(), this.scheduleForm.value.filtershiftfromdate.getMinutes(), this.scheduleForm.value.filtershiftfromdate.getSeconds()));
      this.scheduleDetails.IsExportToPDF = this.IsExportToPDF;
      this.scheduleDetails.UpdateBy = new User();
      this.scheduleDetails.ProductBatchName = this.scheduleForm.value.StatementProductBatchName;
      this.scheduleDetails.RecurrancePattern = this.scheduleForm.value.RecurrancePattern;

      var userid = localStorage.getItem('UserId');
      this.scheduleDetails.UpdateBy.Identifier = Number(userid);
      this.scheduleDetails.RecurrancePattern = this.scheduleForm.value.RecurrancePattern;
      this.scheduleDetails.IsEveryWeekDay = this.scheduleForm.value.RecurrancePattern == "Weekday" ? true : false;

      var weekdaystr = '';
      var d = this.scheduleStartDate;
      var day = d.getDay();
      var month = d.getMonth();
      if (this.scheduleForm.value.RecurrancePattern == 'Weekly') {
        weekdaystr = weekdaystr + this.dayArray[day];
        this.scheduleDetails.WeekDays = weekdaystr;
      }
      else if (this.scheduleForm.value.RecurrancePattern == 'Monthly') {
        this.scheduleDetails.DayOfMonth = day;
      }
      else if (this.scheduleForm.value.RecurrancePattern == 'Yearly') {
        this.scheduleDetails.DayOfMonth = day;
        this.scheduleDetails.MonthOfYear = this.monthArray[month];
      }
      else if (this.scheduleForm.value.RecurrancePattern == "Custom") {
        this.scheduleDetails.RecurrancePattern = this.scheduleForm.value.RecurrancePattern + '-' + this.scheduleForm.value.RepeatEveryBy;
        this.scheduleDetails.RepeatEveryDayMonWeekYear = this.scheduleForm.value.RepeatEvery != null ? this.scheduleForm.value.RepeatEvery : 1;
        if (this.scheduleDetails.RecurrancePattern == 'Custom-Week' && this.selectedWeekdays.length > 0) {
          this.selectedWeekdays.sort(function (a, b) {
            return a.Id - b.Id;
          });
          for (let i = 0; i < this.selectedWeekdays.length; i++) {
            weekdaystr = weekdaystr + (weekdaystr != '' ? ',' : '') + this.selectedWeekdays[i].Day;
          }
          this.scheduleDetails.WeekDays = weekdaystr;
        } else {
          this.scheduleDetails.WeekDays = '';
        }

        if (this.scheduleDetails.RecurrancePattern == 'Custom-Month') {
          this.scheduleDetails.DayOfMonth = this.scheduleForm.value.CustomMonthDay;
        } else if (this.scheduleDetails.RecurrancePattern == 'Custom-Year') {
          this.scheduleDetails.DayOfMonth = this.scheduleForm.value.CustomYearDay;
        } else {
          this.scheduleDetails.DayOfMonth = 0;
        }
        this.scheduleDetails.MonthOfYear = this.scheduleDetails.RecurrancePattern == 'Custom-Year' ? this.scheduleForm.value.CustomYearMonth : null;
      }

      if (this.isEndAfter != null && this.isEndAfter == true && this.scheduleDetails.RecurrancePattern != 'DoesNotRepeat') {
        this.scheduleDetails.IsEndsAfterNoOfOccurrences = this.isEndAfter;
        this.scheduleDetails.NoOfOccurrences = this.scheduleForm.value.scheduleEndAfterNoOccurences;
      }

      this.scheduleDetails.EndDate = this.isEndDate && this.scheduleDetails.RecurrancePattern != 'DoesNotRepeat' ? new Date(Date.UTC(this.scheduleForm.value.filtershiftenddate.getFullYear(), this.scheduleForm.value.filtershiftenddate.getMonth(), this.scheduleForm.value.filtershiftenddate.getDate(), this.scheduleForm.value.filtershiftenddate.getHours(), this.scheduleForm.value.filtershiftenddate.getMinutes(), this.scheduleForm.value.filtershiftenddate.getSeconds())) : null;
      pageArray.push(this.scheduleDetails);
    });

    let scheduleService = this.injector.get(ScheduleService);
    let isRecordSaved = await scheduleService.saveSchedule(pageArray, this.updateOperationMode);
    if (isRecordSaved) {
      let message = Constants.recordAddedMessage;
      if (this.updateOperationMode) {
        message = Constants.recordUpdatedMessage;
      }
      this._messageDialogService.openDialogBox('Success', message, Constants.msgBoxSuccess);
      this.navigateToListPage()
    }
  }
  handleChange() {
    console.log(this.scheduleForm.value);
  }
  navigateToListPage() {
    this.router.navigate(['/schedulemanagement']);
  }

}
