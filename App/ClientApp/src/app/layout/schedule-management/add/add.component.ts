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

@Component({
  selector: 'app-add',
  templateUrl: './add.component.html',
  styleUrls: ['./add.component.scss']
})


//export interface DropDonw {
//  Identifier: string,
//  Name:string
//}

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
  public scheduleFormErrorObject: any = {
  };
  //public DayOfMonthList: DropDonw[] = [];
  //public TimeOfDayHoursList: DropDonw[] = [];
  //public TimeOfDayMinutesList: DropDonw[] = [];
  public DayOfMonthList: any = [];
  public TimeOfDayHoursList: any = [];
  public TimeOfDayMinutesList: any = [];
  public isFirstimeLoad = false;
  public IsStartDateDisable = false;
  constructor(
    private formBuilder: FormBuilder,
    private spinner: NgxUiLoaderService,
    private router: Router,
    private _messageDialogService: MessageDialogService,
    private injector: Injector

  ) {

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

    for (var i = 0; i <= 59; i++) {
      var object = { Identifier: i.toString(), Name: "" };
      if (i < 10) {
        object.Name = "0" + i;
      }
      else {
        object.Name = i.toString();
      }
      this.TimeOfDayMinutesList.push(object);
    }

    this.scheduleForm = this.formBuilder.group({
      ScheduleName: [null, Validators.compose([Validators.required, Validators.minLength(2), Validators.maxLength(50),
      Validators.pattern(this.onlyAlphabetswithInbetweenSpaceUpto50Characters)])],
      StatementDefinition: [0, Validators.compose([Validators.required])],
      DayOfMonth: ["Please Select", Validators.compose([Validators.required])],
      TimeOfDayHours: ["Please Select", Validators.compose([Validators.required])],
      TimeOfDayMinutes: ["Please Select", Validators.compose([Validators.required])],
      filtershiftfromdate: [null, Validators.compose([Validators.required])],
      filtershiftenddate: [null,],
      ExportToPDF: [false],
      NoEndDate: [false],
    })

    this.scheduleForm.controls['StatementDefinition'].setValue(this.st.Identifier);
    this.scheduleForm.controls['DayOfMonth'].setValue(obj.Identifier);
    this.scheduleForm.controls['TimeOfDayHours'].setValue(obj.Identifier);
    this.scheduleForm.controls['TimeOfDayMinutes'].setValue(obj.Identifier);

    this.getStatements();
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

    var list = await statementService.getStatements(searchParameter);
    for (var i = 0; i < list.length; i++) {
      this.statementDefinitionList.push(list[i]);
    }
    if (localStorage.getItem('scheduleparams')) {

      this.getSchedule();
    }

  }

  async getSchedule() {
    let statementService = this.injector.get(ScheduleService);

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

    var statementList = await statementService.getSchedule(searchParameter);

    if (statementList.length == 0) {
      this._messageDialogService.openDialogBox('Error', "Schedule Not Found", Constants.msgBoxError);
    }
    this.schedule = statementList[0];
    this.scheduleForm.controls['ScheduleName'].setValue(this.schedule.Name);
    this.scheduleForm.controls['StatementDefinition'].setValue(this.schedule.Statement.Identifier);
    this.scheduleForm.controls['DayOfMonth'].setValue(this.schedule.DayOfMonth);
    this.scheduleForm.controls['TimeOfDayHours'].setValue(this.schedule.HourOfDay);
    this.scheduleForm.controls['TimeOfDayMinutes'].setValue(this.schedule.MinuteOfDay);
    this.scheduleForm.controls['filtershiftfromdate'].setValue(this.schedule.StartDate);
    var startDate = new Date(this.schedule.StartDate);
    var endDate = new Date(this.schedule.StartDate);
    var currentDate = new Date();
    if (startDate.getTime() < currentDate.getTime()) {
      this.IsStartDateDisable = true;
    }
    if (this.schedule.EndDate.toString() == "0001-01-01T00:00:00") {
      this.IsEndDateRequired = false;
    }
    else {
      this.scheduleForm.controls['filtershiftenddate'].setValue(this.schedule.EndDate);

    }
    if (this.schedule.IsExportToPDF) {
      this.IsExportToPDF = true;
    }
    else {
      this.IsExportToPDF = false;

    }

  }

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

  get ExportToPDF() {
    return this.scheduleForm.get('ExportToPDF');
  }

  get NoEndDate() {
    return this.scheduleForm.get('NoEndDate');
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
      this.filterFromDateError = false;
      this.filterFromDateErrorMessage = "";
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
    let currentDte = new Date();
    if (this.scheduleForm.value.filtershiftfromdate != null && this.scheduleForm.value.filtershiftfromdate != '') {
      let startDate = this.scheduleForm.value.filtershiftfromdate;
    
        if (startDate.getTime() < currentDte.getTime()) {
          this.filterFromDateError = true;
          this.filterFromDateErrorMessage = ErrorMessageConstants.getStartDateThanCurrentDateMessage;
        }
     
    }
    //if (this.scheduleForm.value.filtershiftenddate != null && this.scheduleForm.value.filtershiftenddate != '') {
    //  let toDate = this.scheduleForm.value.filtershiftenddate;
    //  if (toDate.getDate() < currentDte.getDate()) {
    //    this.filterToDateError = true;
    //    this.filterToDateErrorMessage = ErrorMessageConstants.getEndDateThanCurrentDateMessage;
    //  }
    //}
    if (this.scheduleForm.value.filtershiftfromdate != null && this.scheduleForm.value.filtershiftfromdate != '' &&
      this.scheduleForm.value.filtershiftenddate != null && this.scheduleForm.value.filtershiftenddate != '') {
      let startDate = this.scheduleForm.value.filtershiftfromdate;
      let toDate = this.scheduleForm.value.filtershiftenddate;
      if (this.IsEndDateRequired) {
        if (startDate.getTime() > toDate.getTime()) {
          this.filterToDateError = true;
          this.filterToDateErrorMessage = ErrorMessageConstants.getStartDateLessThanEndDateMessage;
        }
        else {
          if (this.monthDiff(startDate, toDate) < 30) {
            this.filterToDateError = true;
            this.filterToDateErrorMessage = "Start date and end date should have minimum one month diffrenece";
          }
        }
      }


    }
  }

  monthDiff(d1, d2) {

    return Math.floor((Date.UTC(d2.getFullYear(), d2.getMonth(), d2.getDate())
      - Date.UTC(d1.getFullYear(), d1.getMonth(), d1.getDate())) / (1000 * 60 * 60 * 24));

  }

  vaildateForm() {
    if (this.scheduleForm.invalid)
      return true;
    else if (this.scheduleForm.value.DayOfMonth == "Please Select") {

      return true;
    }
    else if (this.scheduleForm.value.HourOfDay == "Please Select") {

      return true;
    }
    else if (this.scheduleForm.value.MinuteOfDay == "Please Select") {

      return true;
    }
    else if (this.scheduleForm.value.StatementDefinition == "0") {
      return true;
    }
    else if (this.filterFromDateError || this.scheduleForm.value.filtershiftfromdate == "") {
      return true;
    }
    else if (this.IsEndDateRequired) {
      if (this.filterToDateError || this.scheduleForm.value.filtershiftenddate == "") {
        return true;
      }
    }
    return false;

  }

  async SaveSchedule() {

    this.schedule.Name = this.scheduleForm.value.ScheduleName;
    this.schedule.Statement.Identifier = this.scheduleForm.value.StatementDefinition;
    this.schedule.StartDate = this.scheduleForm.value.filtershiftfromdate;
    this.schedule.EndDate = this.IsEndDateRequired ? this.scheduleForm.value.filtershiftenddate : null;
    this.schedule.IsExportToPDF = this.IsExportToPDF;
    this.schedule.UpdateBy = new User();
    var userid = localStorage.getItem('UserId')
    this.schedule.UpdateBy.Identifier = Number(userid);
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
