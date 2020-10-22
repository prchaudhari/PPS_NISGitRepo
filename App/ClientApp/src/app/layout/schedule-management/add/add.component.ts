import { Component, OnInit, Injector, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';
import { NgxUiLoaderService } from 'ngx-ui-loader';
import { Router, NavigationEnd } from '@angular/router';
import { Constants } from '../../../shared/constants/constants';
import { ErrorMessageConstants } from '../../../shared/constants/constants';
import { MessageDialogService } from '../../../shared/services/mesage-dialog.service';
import { ScheduleService } from '../schedule.service';
import { Schedule } from '../schedule';
import { Statement } from '../../statement-defination/statement';
import { StatementService } from '../../statement-defination/statement.service';
import { User } from '../../users/user';
import * as $ from 'jquery';

@Component({
  selector: 'app-add',
  templateUrl: './add.component.html',
  styleUrls: ['./add.component.scss']
})

export class AddComponent implements OnInit {

  public scheduleForm: FormGroup;
  public statementDefinitionList = [];
  public onlyAlphabetswithInbetweenSpaceUpto50Characters = Constants.onlyAlphabetswithInbetweenSpaceUpto50Characters;
  public schedule: Schedule;
  public st: Statement;
  public updateOperationMode: boolean;
  public params: any = [];
  public IsEndDateRequired = true;
  public IsExportToPDF = false;
  public filterFromDateError: boolean = false;
  public filterFromDateErrorMessage: string = "";
  public filterToDateError: boolean = false;
  public filterToDateErrorMessage: string = "";
  public filterDateDifferenecError: boolean = false;
  public filterDateDiffErrorMessage: string = "";
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
  public monthArray = ['January','February','March','April','May','June','July','August','September','October','November','December'];
  public dayArray = ['Sunday','Monday','Tuesday','Wednesday','Thursday','Friday','Saturday'];
  public dayObjectArr = [{Id: 1, 'Day':'Monday'},{Id:2,'Day':'Tuesday'},{Id:3, 'Day':'Wednesday'},{Id:4,'Day':'Thursday'},{Id:5, 'Day':'Friday'},{Id:6,'Day':'Saturday'},{Id: 7, 'Day':'Sunday'}];
  public isCustom: boolean = false;
  public scheduleStartDate = new Date();
  public scheduleEndDate = new Date();
  public tommorrow = new Date();
  public scheduleOccuranceDay = 0;
  public scheduleOccuranceMonth = '';
  public selectedWeekdays = [];
  public DataFormat;
  public IsAnyBatchExecuted: boolean = false;

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

  OnRecurrancePatternChange() {
    let RecurrancePatternVal = this.scheduleForm.value.RecurrancePattern != null ? this.scheduleForm.value.RecurrancePattern : '';
    if(RecurrancePatternVal != '') {
      var d = this.scheduleStartDate;
      var today = d.toLocaleDateString();
      var dte = d.getDate();
      var day = d.getDay();
      var month = d.getMonth();
      this.isCustom = false;
      this.isNoEndDate = false;

      if(this.isEndDate == true) {
        setTimeout(() => {
          $('#enddate').prop('checked', true);
        }, 10);
      }else {
        setTimeout(() => {
          $('#enddate').prop('checked', false);
        }, 10);
      }

      if(this.isEndAfter == true) {
        setTimeout(() => {
          $('#endafter').prop('checked', true);
        }, 10);
      }else {
        setTimeout(() => {
          $('#endafter').prop('checked', false);
        }, 10);
      }
      if(RecurrancePatternVal == 'Weekday') {
        this.ScheduleOccuranceMessage = 'Occurs every Monday through Friday starting ' + today;
      }else if(RecurrancePatternVal == 'Daily') {
        this.ScheduleOccuranceMessage = 'Occurs every day starting ' + today;
      }else if(RecurrancePatternVal == 'Weekly') {
        this.ScheduleOccuranceMessage = 'Occurs every ' + this.dayArray[day] + ' starting ' + today;
      }else if(RecurrancePatternVal == 'Monthly') {
        this.ScheduleOccuranceMessage = 'Occurs every Month on day ' + dte + ' starting ' + today;
      }else if(RecurrancePatternVal == 'Yearly') {
        this.ScheduleOccuranceMessage = 'Occurs every Year on day '+ dte + ' of ' + this.monthArray[month] + ' starting ' + today;
      }else if(RecurrancePatternVal == 'Custom') {
        this.isCustom = true;
        this.setScheduleOccuranceMessage();
      }else {
        this.isNoEndDate = true;
        this.ScheduleOccuranceMessage = '';
      }
    }else {
      this.ScheduleOccuranceMessage = '';
    }
  }

  onRepeatEveryByValueChange() {
    let repeatEveryByVal = this.scheduleForm.value.RepeatEveryBy != null ? this.scheduleForm.value.RepeatEveryBy : 'Day';
    if(repeatEveryByVal == 'Day') {
      this.isDaily = true;
      this.isWeekly = false;
      this.isMonthly = false;
      this.isYearly = false;
    }else if(repeatEveryByVal == 'Week') {
      this.isDaily = false;
      this.isWeekly = true;
      this.isMonthly = false;
      this.isYearly = false;
    }else if(repeatEveryByVal == 'Month') {
      this.isDaily = false;
      this.isWeekly = false;
      this.isMonthly = true;
      this.isYearly = false;
    }else if(repeatEveryByVal == 'Year') {
      this.isDaily = false;
      this.isWeekly = false;
      this.isMonthly = false;
      this.isYearly = true;
    }
    this.setScheduleOccuranceMessage();
  }

  onRepeatEveryChange() {
    if(this.scheduleForm.value.RepeatEvery==0) {
      this.scheduleForm.controls['RepeatEvery'].setValue(1);
    }
    this.setScheduleOccuranceMessage();
  }

  onCustomMonthDayChange() {
    if(this.scheduleForm.value.CustomMonthDay==0) {
      this.scheduleForm.controls['CustomMonthDay'].setValue(1);
    }else if(this.scheduleForm.value.CustomMonthDay>31) {
      this.scheduleForm.controls['CustomMonthDay'].setValue(31);
    }
    this.scheduleOccuranceDay = this.scheduleForm.value.CustomMonthDay;
    this.setScheduleOccuranceMessage();
  }

  onCustomYearDayChange() {
    if(this.scheduleForm.value.CustomYearDay==0) {
      this.scheduleForm.controls['CustomYearDay'].setValue(1);
    }else if(this.scheduleForm.value.CustomYearDay>31) {
      this.scheduleForm.controls['CustomYearDay'].setValue(31);
    }
    this.scheduleOccuranceDay = this.scheduleForm.value.CustomYearDay;
    this.setScheduleOccuranceMessage();
  }

  onScheduleEndAfterNoOccurencesChange() {
    if(this.scheduleForm.value.scheduleEndAfterNoOccurences==0) {
      this.scheduleForm.controls['scheduleEndAfterNoOccurences'].setValue(1);
    }
    this.setScheduleOccuranceMessage();
  }

  onCustomYearMonthChange() {
    this.scheduleOccuranceMonth = this.scheduleForm.value.CustomYearMonth;
    this.setScheduleOccuranceMessage();
  }

  weekdaySelection(event, day, Id) {
    if (event.target.checked) {
      this.selectedWeekdays.push({'Id': Id, 'Day': day});
    }else {
      let index = this.selectedWeekdays.findIndex(x => x.Id == Id);
      this.selectedWeekdays.splice(index, 1);
    }
    this.setScheduleOccuranceMessage();
  }

  setScheduleOccuranceMessage() {

    var dt = new Date(this.schedule.StartDate);
    var dayOfMonth = this.schedule.DayOfMonth == 0 ? dt.getDate() : this.schedule.DayOfMonth;
    var ssd = new Date(dt.getFullYear(), dt.getMonth(), dayOfMonth);
    
    //var ssd = new Date(this.scheduleForm.value.filtershiftfromdate);
    var schedulestartdte = ssd.toLocaleDateString();
    var dte = this.scheduleOccuranceDay != 0 ? this.scheduleOccuranceDay : ssd.getDate();
    var month = this.scheduleOccuranceMonth != '' ? this.scheduleOccuranceMonth : this.monthArray[ssd.getMonth()];

    if(this.schedule.RecurrancePattern == 'DoesNotRepeat') {
      this.ScheduleOccuranceMessage = 'Occurs once on ' + schedulestartdte;
    }else {
      let scheduleRunUtilMessage = '';
      if(this.isEndDate == true && this.scheduleForm.value.filtershiftenddate != null && this.scheduleForm.value.filtershiftenddate != '') {
        let sed = new Date(this.scheduleForm.value.filtershiftenddate);
        scheduleRunUtilMessage = ' until '+sed.toLocaleDateString();
      }else if(this.isEndAfter == true && this.scheduleForm.value.scheduleEndAfterNoOccurences != null && this.scheduleForm.value.scheduleEndAfterNoOccurences != 0) {
        scheduleRunUtilMessage = ' upto '+this.scheduleForm.value.scheduleEndAfterNoOccurences + " occurence.";
      }
      
      let repeatEvery = this.scheduleForm.value.RepeatEvery != null ? this.scheduleForm.value.RepeatEvery : 1;
      let repeatEveryByVal = this.scheduleForm.value.RepeatEveryBy != null ? this.scheduleForm.value.RepeatEveryBy : 'Day';
      let occurance = '';

      if(repeatEveryByVal == 'Day') {
        if(repeatEvery == 1) {
          occurance = 'day';
        }else{
          occurance = repeatEvery+' days ';
        }
      }
      else if(repeatEveryByVal == 'Week') {
        var weekdaystr = '';
        if(this.selectedWeekdays.length > 0) {
          this.selectedWeekdays.sort(function(a, b){
            return a.Id - b.Id;
          });
          for(let i=0; i<this.selectedWeekdays.length; i++) {
            let day = this.selectedWeekdays[i].Day;
            weekdaystr = weekdaystr + (weekdaystr != '' ? (i == (this.selectedWeekdays.length - 1) ? ' and ' : ', ') : '') + day;                
            setTimeout(() => {
              $('#weekday-'+day.toLowerCase()).prop('checked', true);
            }, 10);
          }
        }
        if(repeatEvery == 1) {
          occurance = '' + (weekdaystr != '' ? 'on '+ weekdaystr : ' week');
        }else{
          occurance = repeatEvery+' weeks ' + (weekdaystr != '' ? 'on '+ weekdaystr : '');
        }
      }
      else if(repeatEveryByVal == 'Month') {
        if(repeatEvery == 1) {
          occurance = 'month on day '+dte;
        }else{
          occurance = repeatEvery+' months on day '+dte;
        }
        this.scheduleForm.get('CustomMonthDay').setValue(dte);
      }
      else if(repeatEveryByVal == 'Year') {
        if(repeatEvery == 1) {
          occurance = 'month on day '+dte+ ' of '+month;
        }else{
          occurance = repeatEvery+' months on day '+dte+ ' of '+month;
        }
        this.scheduleForm.get('CustomYearDay').setValue(dte);
        this.scheduleForm.get('CustomYearMonth').setValue(month);
      }
      this.ScheduleOccuranceMessage = 'Occurs every '+occurance+' starting ' + schedulestartdte + scheduleRunUtilMessage;
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
    private injector: Injector) {
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
            this.schedule.Identifier = this.params.Routeparams.passingparams.ScheduleIdentifier;
          }
        } else {
          localStorage.removeItem("scheduleparams");
        }
      }
    });
  }

  ngOnInit() {

    this.tommorrow.setDate(this.tommorrow.getDate() + 1);
    this.tommorrow.setHours(0,0,0,0);
    this.scheduleStartDate = this.tommorrow;

    this.DataFormat = localStorage.getItem('DateFormat');
    this.st = new Statement;
    this.st.Name = "Please Select";
    this.st.Identifier = 0;
    this.statementDefinitionList.push(this.st);
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

    var x=0;
    while (x<=55) {
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
      ScheduleName: [null, Validators.compose([Validators.required, Validators.minLength(2), Validators.maxLength(50),
      Validators.pattern(this.onlyAlphabetswithInbetweenSpaceUpto50Characters)])],
      StatementDefinition: [0, Validators.compose([Validators.required])],
      //DayOfMonth: ["Please Select", Validators.compose([Validators.required])],
      TimeOfDayHours: ["Please Select", Validators.compose([Validators.required])],
      TimeOfDayMinutes: ["Please Select", Validators.compose([Validators.required])],
      filtershiftfromdate: [null, Validators.compose([Validators.required])],
      filtershiftenddate: [null],
      RepeatEvery:[null],
      RecurrancePattern: ["DoesNotRepeat", Validators.compose([Validators.required])],
      RepeatEveryBy: ["Day"],
      CustomYearMonth:[null],
      CustomYearDay:[null],
      CustomMonthDay:[null],
      scheduleEndAfterNoOccurences: [null]
      //ExportToPDF: [false],
      //NoEndDate: [false],
    })

    if(this.updateOperationMode == false) {
      this.scheduleForm.controls['StatementDefinition'].setValue(this.st.Identifier);
      //this.scheduleForm.controls['DayOfMonth'].setValue(obj.Identifier);
      this.scheduleForm.controls['TimeOfDayHours'].setValue(obj.Identifier);
      this.scheduleForm.controls['TimeOfDayMinutes'].setValue(obj.Identifier);
      this.scheduleForm.controls['filtershiftfromdate'].setValue(this.tommorrow); //Default today is the schedule start date
      this.scheduleForm.controls['RepeatEvery'].setValue(1);
      this.scheduleForm.controls['scheduleEndAfterNoOccurences'].setValue(1);
    }
    
    this.getStatements();
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
    }
  }

  async getSchedule() {
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
    var response = await scheduleService.getSchedule(searchParameter);
    if (response.List.length == 0) {
      this._messageDialogService.openDialogBox('Error', "Schedule Not Found", Constants.msgBoxError);
    }
    this.schedule = response.List[0];
    this.scheduleForm.controls['ScheduleName'].setValue(this.schedule.Name);
    this.scheduleForm.controls['StatementDefinition'].setValue(this.schedule.Statement.Identifier);
    //this.scheduleForm.controls['DayOfMonth'].setValue(this.schedule.DayOfMonth);
    this.scheduleForm.controls['TimeOfDayHours'].setValue(this.schedule.HourOfDay);
    this.scheduleForm.controls['TimeOfDayMinutes'].setValue(this.schedule.MinuteOfDay);
    this.scheduleForm.controls['filtershiftfromdate'].setValue(new Date(this.schedule.StartDate));
    var startDate = new Date(this.schedule.StartDate);
    var endDate = new Date(this.schedule.EndDate);
    var currentDate = new Date();
    if (startDate.getTime() < currentDate.getTime()) {
      this.IsStartDateDisable = true;
    }
    if (endDate.getTime() < currentDate.getTime()) {
      this.IsEndDateDisable = true;
    }
    if (this.schedule.EndDate.toString() == "0001-01-01T00:00:00") {
      this.IsEndDateRequired = false;
      this.isEndDate = false;
    }
    else {
      this.scheduleForm.controls['filtershiftenddate'].setValue(new Date(this.schedule.EndDate));
      this.isEndDate = true;
    }
    
    if(this.schedule.NoOfOccurrences!=null && this.schedule.NoOfOccurrences!= 0){
      this.scheduleForm.controls['scheduleEndAfterNoOccurences'].setValue(this.schedule.NoOfOccurrences);
      this.isEndAfter = true;
    }

    let pattern = '';
    let RepeatEveryBy ='';
    if(this.schedule.RecurrancePattern == null || this.schedule.RecurrancePattern == '') {
      this.scheduleForm.controls['RecurrancePattern'].setValue("Custom");
      this.scheduleForm.controls['RepeatEveryBy'].setValue("Month");
    }else {
      if(this.schedule.RecurrancePattern.includes('Custom')) {
        let index = this.schedule.RecurrancePattern.indexOf('-');
        RepeatEveryBy = this.schedule.RecurrancePattern.substring(index+1, this.schedule.RecurrancePattern.length);
        this.scheduleForm.controls['RepeatEveryBy'].setValue(RepeatEveryBy);
        pattern = this.schedule.RecurrancePattern.substring(0, index);
        this.scheduleForm.controls['RecurrancePattern'].setValue(pattern);
      }else {
        this.scheduleForm.controls['RecurrancePattern'].setValue(this.schedule.RecurrancePattern);
      }
    }

    if(this.schedule.DayOfMonth != null && this.schedule.DayOfMonth != 0) {
      if(RepeatEveryBy=='Month') {
        this.scheduleForm.controls['CustomMonthDay'].setValue(this.schedule.DayOfMonth);
        this.scheduleOccuranceDay = this.schedule.DayOfMonth;
      }else if(RepeatEveryBy=='Year') {
        this.scheduleForm.controls['CustomYearDay'].setValue(this.schedule.DayOfMonth);
        this.scheduleForm.controls['CustomYearMonth'].setValue(this.schedule.MonthOfYear);
        this.scheduleOccuranceDay = this.schedule.DayOfMonth;
        this.scheduleOccuranceMonth = this.schedule.MonthOfYear;
      }
    }else {
      if(RepeatEveryBy=='Month') {
        this.scheduleForm.controls['CustomMonthDay'].setValue(1);
      }else if(RepeatEveryBy=='Year') {
        this.scheduleForm.controls['CustomYearDay'].setValue(1);
      }
    }

    if(this.schedule.RepeatEveryDayMonWeekYear == null || this.schedule.RepeatEveryDayMonWeekYear == 0) {
      this.scheduleForm.controls['RepeatEvery'].setValue(1);
    }else {
      this.scheduleForm.controls['RepeatEvery'].setValue(this.schedule.RepeatEveryDayMonWeekYear);
    }

    if(this.schedule.WeekDays != null && this.schedule.WeekDays!='') {
      var scheduledays = this.schedule.WeekDays.split(',');
      scheduledays.forEach(day => {
        var dayObj = this.dayObjectArr.filter(x => x.Day.toLocaleLowerCase() == day.toLocaleLowerCase())[0];
        this.selectedWeekdays.push({'Id': dayObj.Id, 'Day': dayObj.Day});
      }); 
    }
    
    if (this.schedule.IsExportToPDF) {
      this.IsExportToPDF = true;
    }
    else {
      this.IsExportToPDF = false;
    }

    this.IsAnyBatchExecuted = this.schedule.ExecutedBatchCount > 0 ? true : false;
    this.OnRecurrancePatternChange();
    this.onRepeatEveryByValueChange();
  }

  public onStateDefinitionSelected(event) {
    const value = event.target.value;
    if (value == "0") {
      this.scheduleFormErrorObject.statementShowError = true;
      this.schedule.Statement.Identifier = 0;
    }
    else {
      this.scheduleFormErrorObject.statementShowError = false;
      this.schedule.Statement.Identifier = Number(value);
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
    if (this.isEndDate==true) {
      let startDate = this.scheduleForm.value.filtershiftfromdate;
      let toDate = this.scheduleForm.value.filtershiftenddate;
      if(startDate!=null && toDate!=null) {
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
    // if (this.scheduleForm.value.DayOfMonth == "Please Select") {
    //   return true;
    // }
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
    if (this.isEndDate==true && this.scheduleForm.value.RecurrancePattern != "DoesNotRepeat") {
      if (this.filterToDateError || this.scheduleForm.value.filtershiftenddate == "" || this.scheduleForm.value.filtershiftenddate == null) {
        return true;
      }
    }
    if (this.filterDateDifferenecError) {
      return true;
    }
    if(this.isEndAfter==true && this.scheduleForm.value.scheduleEndAfterNoOccurences<=0) {
      return true;
    }
    return false;
  }

  async SaveSchedule() {
    this.schedule.Name = this.scheduleForm.value.ScheduleName;
    this.schedule.Statement.Identifier = this.scheduleForm.value.StatementDefinition;
    this.schedule.StartDate = this.scheduleForm.value.filtershiftfromdate;
    this.schedule.IsExportToPDF = this.IsExportToPDF;
    this.schedule.UpdateBy = new User();
    var userid = localStorage.getItem('UserId')
    this.schedule.UpdateBy.Identifier = Number(userid);

    if(this.updateOperationMode==false || this.schedule.ExecutedBatchCount == 0) {
      this.schedule.RecurrancePattern = this.scheduleForm.value.RecurrancePattern;
      this.schedule.IsEveryWeekDay = this.scheduleForm.value.RecurrancePattern == "Weekday" ? true : false;

      var weekdaystr = '';
      var d = this.scheduleStartDate;
      var day = d.getDay();
      var month = d.getMonth();
      if(this.scheduleForm.value.RecurrancePattern=='Weekly') {
        weekdaystr = weekdaystr + this.dayArray[day];
        this.schedule.WeekDays = weekdaystr;
      }
      else if(this.scheduleForm.value.RecurrancePattern=='Monthly') {
        this.schedule.DayOfMonth = day;
      }
      else if(this.scheduleForm.value.RecurrancePattern=='Yearly') {
        this.schedule.DayOfMonth = day;
        this.schedule.MonthOfYear = this.monthArray[month];
      }
      else if(this.scheduleForm.value.RecurrancePattern == "Custom") {
        this.schedule.RecurrancePattern = this.scheduleForm.value.RecurrancePattern + '-' +this.scheduleForm.value.RepeatEveryBy;
        this.schedule.RepeatEveryDayMonWeekYear = this.scheduleForm.value.RepeatEvery != null ? this.scheduleForm.value.RepeatEvery : 1;
        if(this.schedule.RecurrancePattern == 'Custom-Week' && this.selectedWeekdays.length > 0) {
          this.selectedWeekdays.sort(function(a, b) {
            return a.Id - b.Id;
          });
          for(let i=0; i<this.selectedWeekdays.length; i++) {
            weekdaystr = weekdaystr + (weekdaystr != '' ? ',' : '') + this.selectedWeekdays[i].Day;
          }
          this.schedule.WeekDays = weekdaystr;
        }else {
          this.schedule.WeekDays = '';
        }

        if(this.schedule.RecurrancePattern == 'Custom-Month') {
          this.schedule.DayOfMonth = this.scheduleForm.value.CustomMonthDay;
        }else if(this.schedule.RecurrancePattern == 'Custom-Year') {
          this.schedule.DayOfMonth = this.scheduleForm.value.CustomYearDay;
        }else {
          this.schedule.DayOfMonth = 0;
        }
        this.schedule.MonthOfYear = this.schedule.RecurrancePattern == 'Custom-Year' ? this.scheduleForm.value.CustomYearMonth : null;
      }
    }

    if(this.isEndAfter != null && this.isEndAfter == true) {
      this.schedule.IsEndsAfterNoOfOccurrences = this.isEndAfter;
      this.schedule.NoOfOccurrences = this.scheduleForm.value.scheduleEndAfterNoOccurences;
    }

    this.schedule.EndDate = this.isEndDate ? this.scheduleForm.value.filtershiftenddate : null;
    //console.log(this.schedule);

    let pageArray = [];
    pageArray.push(this.schedule);
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

  navigateToListPage() {
    this.router.navigate(['/schedulemanagement']);
  }
}
