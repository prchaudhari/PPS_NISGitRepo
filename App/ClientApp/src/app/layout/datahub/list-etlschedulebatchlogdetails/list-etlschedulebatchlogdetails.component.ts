import { Component, OnInit, Injector, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';
import { Constants, ErrorMessageConstants } from 'src/app/shared/constants/constants';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { LocalStorageService } from 'src/app/shared/services/local-storage.service';
import { MessageDialogService } from 'src/app/shared/services/mesage-dialog.service';
import { Router, NavigationEnd } from '@angular/router';

import { DataHubService } from '../datahub.service';
import { ETLScheduleBatchLogDetail } from '../datahub';
import * as XLSX from 'xlsx';

@Component({
  selector: 'app-list-etlschedulebatchlogdetails',
  templateUrl: './list-etlschedulebatchlogdetails.component.html',
  styleUrls: ['./list-etlschedulebatchlogdetails.component.scss']
})
export class ListEtlschedulebatchlogdetailsComponent implements OnInit {
  public isFilter: boolean = false;
  public pageNo = 0;
  public pageSize = 5;
  public currentPage = 0;
  public totalSize = 0;
  public previousPageLabel: string;
  public isFilterDone = false;
  public totalRecordCount = 0;
  public sortOrder = Constants.Descending;
  public sortColumn = 'Id';

  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;

  public etlScheduleDetailForBatchLogDetail: any;
  public etlScheduleBatchLogDetailsList: ETLScheduleBatchLogDetail[] = [];
  public sortedEtlScheduleBatchLogDetailsList: ETLScheduleBatchLogDetail[] = [];
  public ETLScheduleBatchLogDetailsFilterForm: FormGroup;
  public dataSource: any;
  displayedColumns: string[] = ['recordNo', 'segment', 'language', 'referenceRecordId', 'status', 'actions']; // ['schedule', 'status', 'lastExecutionDate', 'actions'];
  public userClaimsRolePrivilegeOperations: any[] = [];
  public array: any;

  public filterSegmentValue = '';
  public filterLanguageValue = '';
  public filterStatusValue = '';
  public params: any = [];
  public scheduleName: string;

  public DateFormat;
  public ETLScheduleBatchLogIdentifier = 0;
  public ETLScheduleIdentifier = 0;
  public ETLScheduleBatchIdentifier = 0;
  public ETLScheduleBatchName = null;
  public ETLScheduleBatchExecutionDate = null;
  public ETLScheduleBatchLogMessage = null;
  public ETLScheduleName = null;

  closeFilter() {
    this.isFilter = !this.isFilter;
  }

  constructor(private injector: Injector, private _messageDialogService: MessageDialogService,
    private router: Router, private localstorageservice: LocalStorageService,
    private fb: FormBuilder, private dataHubService: DataHubService) {
    this.sortedEtlScheduleBatchLogDetailsList = this.etlScheduleBatchLogDetailsList.slice();

    router.events.subscribe(e => {
      if (e instanceof NavigationEnd) {
        if (e.url.includes('/list-etlschedulebatchlogdetails')) {
          //set passing parameters to localstorage.          
          if (localStorage.getItem('eTLScheduleBatchLogParams')) {
            this.params = JSON.parse(localStorage.getItem('eTLScheduleBatchLogParams'));
            this.ETLScheduleBatchLogIdentifier = this.params.Routeparams.passingparams.ETLScheduleBatchLogIdentifier;
            this.ETLScheduleName = this.params.Routeparams.passingparams.ETLScheduleName;
            this.ETLScheduleBatchExecutionDate = this.params.Routeparams.passingparams.ETLScheduleBatchExecutionDate;
          }
        }
      }
    });
  }

  //method called on initialization
  ngOnInit() {
    this.DateFormat = "dd/MM/yyyy h:mm a" //localStorage.getItem('DateFormat');

    var userClaimsDetail = JSON.parse(localStorage.getItem('userClaims'));
    if (userClaimsDetail) {
      this.userClaimsRolePrivilegeOperations = userClaimsDetail.Privileges;
    }
    else {
      this.localstorageservice.removeLocalStorageData();
      this.router.navigate(['login']);
    }

    this.getETLScheduleDetailForBatchLogDetails();
    this.getETLSchedulebatchLogDetails(null);
    this.ETLScheduleBatchLogDetailsFilterForm = this.fb.group({
      filterSegment: [null],
      filterLanguage: [null],
      filterStatus: [null]
    });
  }

  async getETLScheduleDetailForBatchLogDetails() {
    let searchParameter: any = {};
    let dataHubService = this.injector.get(DataHubService);

    if (this.ETLScheduleBatchLogIdentifier != null && this.ETLScheduleBatchLogIdentifier != 0) {
      searchParameter.EtlLogId = this.ETLScheduleBatchLogIdentifier;
    }

    var response = await dataHubService.getETLScheduleDetailForBatchLogDetails(searchParameter);
    this.etlScheduleDetailForBatchLogDetail = response;
  }

  public handlePage(e: any) {
    this.currentPage = e.pageIndex;
    this.pageSize = e.pageSize;
    //this.iterator();
    this.getETLSchedulebatchLogDetails(null);
  }

  //To sort table data using column
  sortData(sort: MatSort) {
    const data = this.etlScheduleBatchLogDetailsList.slice();
    if (!sort.active || sort.direction === '') {
      this.sortedEtlScheduleBatchLogDetailsList = data;
      return;
    }

    if (sort.direction == 'asc') {
      this.sortOrder = Constants.Ascending;
    } else {
      this.sortOrder = Constants.Descending;
    }

    switch (sort.active) {
      case 'recordNo': this.sortColumn = "Id"; break;
      case 'segment': this.sortColumn = "Segment"; break;
      case 'language': this.sortColumn = "Language"; break;
      case 'referenceRecordId': this.sortColumn = "ReferenceRecordId"; break;
      case 'status': this.sortColumn = "Status"; break;
      default: this.sortColumn = "Id"; break;
    }

    let searchParameter: any = {};
    searchParameter.IsActive = true;
    searchParameter.PagingParameter = {};
    searchParameter.PagingParameter.PageIndex = this.currentPage + 1;
    searchParameter.PagingParameter.PageSize = this.pageSize;
    searchParameter.SortParameter = {};
    searchParameter.SortParameter.SortColumn = this.sortColumn;
    searchParameter.SortParameter.SortOrder = this.sortOrder;
    searchParameter.SearchMode = Constants.Contains;

    this.filterSegmentValue = (this.ETLScheduleBatchLogDetailsFilterForm.value.filterSegment != null && this.ETLScheduleBatchLogDetailsFilterForm.value.filterSegment != '') ? this.ETLScheduleBatchLogDetailsFilterForm.value.filterSegment.trim() : "";
    this.filterLanguageValue = (this.ETLScheduleBatchLogDetailsFilterForm.value.filterLanguage != null && this.ETLScheduleBatchLogDetailsFilterForm.value.filterLanguage != '') ? this.ETLScheduleBatchLogDetailsFilterForm.value.filterLanguage.trim() : "";
    this.filterStatusValue = (this.ETLScheduleBatchLogDetailsFilterForm.value.filterStatus != null && this.ETLScheduleBatchLogDetailsFilterForm.value.filterStatus != '') ? this.ETLScheduleBatchLogDetailsFilterForm.value.filterStatus.trim() : "";

    searchParameter.Segment = this.filterSegmentValue;
    searchParameter.Language = this.filterLanguageValue;
    searchParameter.Status = this.filterStatusValue;

    this.getETLSchedulebatchLogDetails(searchParameter);
  }

  //This method has been used for fetching search records
  searchFilter(searchType) {
    this.isFilterDone = true;
    if (searchType == 'reset') {
      this.resetETLScheduleBatchLogDetailsFilterForm();
      this.getETLSchedulebatchLogDetails(null);
      this.isFilter = !this.isFilter;
    }
    else {
      let searchParameter: any = {};
      searchParameter.PagingParameter = {};
      searchParameter.PagingParameter.PageIndex = 1;
      searchParameter.PagingParameter.PageSize = this.pageSize;
      searchParameter.SortParameter = {};
      searchParameter.SortParameter.SortColumn = this.sortColumn;
      searchParameter.SortParameter.SortOrder = this.sortOrder;
      searchParameter.SearchMode = Constants.Contains;

      this.filterSegmentValue = (this.ETLScheduleBatchLogDetailsFilterForm.value.filterSegment != null && this.ETLScheduleBatchLogDetailsFilterForm.value.filterSegment != '') ? this.ETLScheduleBatchLogDetailsFilterForm.value.filterSegment.trim() : "";
      this.filterLanguageValue = (this.ETLScheduleBatchLogDetailsFilterForm.value.filterLanguage != null && this.ETLScheduleBatchLogDetailsFilterForm.value.filterLanguage != '') ? this.ETLScheduleBatchLogDetailsFilterForm.value.filterLanguage.trim() : "";
      this.filterStatusValue = (this.ETLScheduleBatchLogDetailsFilterForm.value.filterStatus != null && this.ETLScheduleBatchLogDetailsFilterForm.value.filterStatus != '') ? this.ETLScheduleBatchLogDetailsFilterForm.value.filterStatus.trim() : "";

      searchParameter.Segment = this.filterSegmentValue;
      searchParameter.Language = this.filterLanguageValue;
      searchParameter.Status = this.filterStatusValue;

      this.currentPage = 0;
      this.getETLSchedulebatchLogDetails(searchParameter);
      this.isFilter = !this.isFilter;
    }
  }

  //This method has been used for fetching Data Hub ETL-Schedules records
  async getETLSchedulebatchLogDetails(searchParameter) {
    let dataHubService = this.injector.get(DataHubService);
    if (searchParameter == null) {
      searchParameter = {};
      searchParameter.IsActive = true;
      searchParameter.PagingParameter = {};
      searchParameter.PagingParameter.PageIndex = this.currentPage + 1;
      searchParameter.PagingParameter.PageSize = this.pageSize;
      searchParameter.SortParameter = {};
      searchParameter.SortParameter.SortColumn = this.sortColumn;
      searchParameter.SortParameter.SortOrder = this.sortOrder;
      searchParameter.SearchMode = Constants.Contains;
    }

    if (this.filterSegmentValue != null && this.filterSegmentValue != '') {
      searchParameter.Segment = this.filterSegmentValue.trim();
    }
    if (this.filterLanguageValue != null && this.filterLanguageValue != '') {
      searchParameter.Language = this.filterLanguageValue.trim();
    }
    if (this.filterStatusValue != null && this.filterStatusValue != '') {
      searchParameter.Status = this.filterStatusValue.trim();
    }

    if (this.ETLScheduleBatchLogIdentifier != null && this.ETLScheduleBatchLogIdentifier != 0) {
      searchParameter.EtlLogId = this.ETLScheduleBatchLogIdentifier;
    }

    var response = await dataHubService.getETLScheduleBatchLogDetails(searchParameter);
    this.etlScheduleBatchLogDetailsList = response.List;
    this.totalRecordCount = response.RecordCount;
    if (this.etlScheduleBatchLogDetailsList.length == 0 && this.isFilterDone == true) {
      let message = ErrorMessageConstants.getNoRecordFoundMessage;
      this._messageDialogService.openDialogBox('Error', message, Constants.msgBoxError).subscribe(data => {
        if (data == true) {
          this.resetETLScheduleBatchLogDetailsFilterForm();
          this.getETLSchedulebatchLogDetails(null);
        }
      });
    }
    this.dataSource = new MatTableDataSource<ETLScheduleBatchLogDetail>(this.etlScheduleBatchLogDetailsList);
    this.dataSource.sort = this.sort;
    this.array = this.etlScheduleBatchLogDetailsList;
    this.totalSize = this.totalRecordCount;
    //this.iterator();
  }

  //To reset field of ETL Schedule filter form
  resetETLScheduleBatchLogDetailsFilterForm() {
    this.ETLScheduleBatchLogDetailsFilterForm.patchValue({
      filterSegment: null,
      filterLanguage: null,
      filterStatus: null
    });
    this.currentPage = 0;
    this.filterSegmentValue = '';
    this.filterLanguageValue = '';
    this.filterStatusValue = '';
  }

  //Getters for ETL Schedule Batch Log Detail Forms
  get filterSegment() {
    return this.ETLScheduleBatchLogDetailsFilterForm.get('filterSegment');
  }

  //Getters for ETL Schedule Batch Log Detail Forms
  get filterLanguage() {
    return this.ETLScheduleBatchLogDetailsFilterForm.get('filterLanguage');
  }

  //Getters for ETL Schedule Batch Log Detail Forms
  get filterStatus() {
    return this.ETLScheduleBatchLogDetailsFilterForm.get('filterStatus');
  }


  //this method helps to navigate to  DataHub View Page
  navigateToDataHubViewPage() {
    const router = this.injector.get(Router);
    router.navigate(['datahub', 'View']);
  }

  showLogMessage(logMessage: string) {
    if (logMessage.indexOf('@#@') != -1) {
      this.ETLScheduleBatchLogMessage = "<p>" + logMessage.slice(0, logMessage.indexOf('@#@')) + "<br/>" + logMessage.slice(logMessage.indexOf('@#@')) + "</p>";
    }
    else {
      this.ETLScheduleBatchLogMessage = "<p>" + logMessage + "</p>";
    }
  }

  closeLogMessage() {
    this.ETLScheduleBatchLogMessage = '';
  }

  async downloadLogItemData(searchParameter, eTLScheduleBatchLogIdentifier: Number) {
    let dataHubService = this.injector.get(DataHubService);
    if (searchParameter == null) {
      searchParameter = {};
      searchParameter.IsActive = true;
      searchParameter.PagingParameter = {};
      searchParameter.PagingParameter.PageIndex = 1;
      searchParameter.PagingParameter.PageSize = this.totalRecordCount;
      searchParameter.SortParameter = {};
      searchParameter.SortParameter.SortColumn = this.sortColumn;
      searchParameter.SortParameter.SortOrder = this.sortOrder;
      searchParameter.SearchMode = Constants.Contains;
    }

    searchParameter.EtlLogId = eTLScheduleBatchLogIdentifier;
    var response = await dataHubService.getETLScheduleBatchLogDetails(searchParameter);

    let dataOfETLScheduleBatchLogDetails: any = [];
    if(response.List.length > 0){
      for (let i = 0; i < response.List.length; i++) {
        if(response.List[i].Status == "Error" || response.List[i].Status == "Failure"){
          dataOfETLScheduleBatchLogDetails.push(
            {
              RecordNo: response.List[i].Identifier,
              Schedule : response.List[i].Schedule,
              Batch : response.List[i].BatchName,
              Segment: response.List[i].Segment,
              Language: response.List[i].Language,
              ReferenceRecordId: response.List[i].ReferenceRecordId,
              LogItemStatus: response.List[i].Status,
              LogMessage: response.List[i].LogMessage,
            }
          );
        }        
      }
  
      if(dataOfETLScheduleBatchLogDetails.length != 0){
        const ws: XLSX.WorkSheet = XLSX.utils.json_to_sheet(dataOfETLScheduleBatchLogDetails);
        const wb: XLSX.WorkBook = XLSX.utils.book_new();
        XLSX.utils.book_append_sheet(wb, ws, 'BatchLog');
    
        /* save to file */
        XLSX.writeFile(wb, 'ETLScheduleBatchLogDetails.xlsx');
      }
      else{
        this._messageDialogService.openDialogBox('Success', "There are no errors into ETL execution.", Constants.msgBoxSuccess);
      }
    }
    else{
      this._messageDialogService.openDialogBox('Success', "There are no errors into ETL execution.", Constants.msgBoxSuccess);
    }
  }

}
