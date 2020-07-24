

import { Component, OnInit, Injector, ChangeDetectorRef, ViewChild, OnDestroy } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import * as $ from 'jquery';
import { Router, ActivatedRoute, NavigationEnd } from '@angular/router';
import { environment } from '../../../../environments/environment';
import { Schedule } from '../schedule';
import { SortParameter, SearchMode } from '../../../shared/models/commonmodel';
import { NgxUiLoaderService } from 'ngx-ui-loader';
import { Location } from '@angular/common';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { Constants } from 'src/app/shared/constants/constants';
import { MessageDialogService } from 'src/app/shared/services/mesage-dialog.service';
import { FormGroup, FormBuilder, Validators, FormControl, SelectControlValueAccessor, FormArray, ValidatorFn } from '@angular/forms';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { ScheduleService } from '../schedule.service';
@Component({
  selector: 'app-view',
  templateUrl: './view.component.html',
  styleUrls: ['./view.component.scss']
})
export class ViewComponent implements OnInit {
  public schedule: Schedule;
  public params;
  public baseURL: string = environment.baseURL;
  public userClaimsRolePrivilegeOperations: any[] = [];
  navigateToListPage() {
    this._location.back();
  }
  constructor(
    private _location: Location,
    private _router: Router,
    private _activatedRouter: ActivatedRoute,
    private _http: HttpClient,
    private _spinnerService: NgxUiLoaderService,
    private formbuilder: FormBuilder,
    private _messageDialogService: MessageDialogService,
    private scheduleService: ScheduleService,
    private injector: Injector,
    private router: Router,
  ) {
    this.schedule = new Schedule;

    let me = this;

    _router.events.subscribe(e => {
      if (e instanceof NavigationEnd) {
        if (e.url.includes('/schedule/Add')) {
          localStorage.removeItem("scheduleparams");
        }
      }

    });

    _router.events.subscribe(e => {
      if (e instanceof NavigationEnd) {
        if (e.url.includes('/schedule')) {
          //set passing parameters to localstorage.
          this.params = JSON.parse(localStorage.getItem('scheduleparams'));
          if (localStorage.getItem('scheduleparams')) {
            this.schedule.Identifier = this.params.Routeparams.passingparams.ScheduleIdentifier;
            this.getScheduleRecords();

          }
        } else {
          localStorage.removeItem("scheduleparams");
        }
      }

    });
  }

  ngOnInit() {
    var userClaimsDetail = JSON.parse(localStorage.getItem('userClaims'));
    if (userClaimsDetail) {
      this.userClaimsRolePrivilegeOperations = userClaimsDetail.Privileges;
    }
    else {
      this.userClaimsRolePrivilegeOperations = [];
    }
  }
  async getScheduleRecords() {
    let scheduleService = this.injector.get(ScheduleService);
    let searchParameter: any = {};
    searchParameter.PagingParameter = {};
    searchParameter.PagingParameter.PageIndex = Constants.DefaultPageIndex;
    searchParameter.PagingParameter.PageSize = Constants.DefaultPageSize;
    searchParameter.SortParameter = {};
    searchParameter.SortParameter.SortColumn = Constants.Name;
    searchParameter.SortParameter.SortOrder = Constants.Ascending;
    searchParameter.SearchMode = Constants.Exact;
    searchParameter.Identifier = this.schedule.Identifier;
    searchParameter.IsStatementDefinitionRequired = true;
    let schedule = await scheduleService.getSchedule(searchParameter);
    this.schedule = schedule[0];

  }

  navigateToScheduleEdit() {
    let queryParams = {
      Routeparams: {
        passingparams: {
          "ScheduleIdentifier": this.schedule.Identifier,
        },
        filteredparams: {
          //passing data using json stringify.
          "ScheduleName": this.schedule.Name != null ? this.schedule.Name : ""
        }
      }
    }
    localStorage.setItem("scheduleparams", JSON.stringify(queryParams))
    const router = this.injector.get(Router);
    router.navigate(['schedulemanagement', 'Edit']);
  }

  deleteSchedule() {
    let message = 'Are you sure, you want to delete this record?';
    this._messageDialogService.openConfirmationDialogBox('Confirm', message, Constants.msgBoxWarning).subscribe(async (isConfirmed) => {
      if (isConfirmed) {
        let roleData = [{
          "Identifier": this.schedule.Identifier,
        }];

        let isDeleted = await this.scheduleService.deleteSchedule(roleData);
        if (isDeleted) {
          let messageString = Constants.recordDeletedMessage;
          this._messageDialogService.openDialogBox('Success', messageString, Constants.msgBoxSuccess);
          this._router.navigate(['schedulemanagement']);
        }
      }
    });
  }
}
