import { Component, OnInit, Injector, ChangeDetectorRef, ViewChild, OnDestroy } from '@angular/core';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { FormGroup, FormBuilder } from '@angular/forms';
import { NgxUiLoaderService } from 'ngx-ui-loader';
import { Router, NavigationEnd } from '@angular/router';
import { Constants } from 'src/app/shared/constants/constants';
import { MessageDialogService } from 'src/app/shared/services/mesage-dialog.service';
import { ConfigConstants } from 'src/app/shared/constants/configConstants';
import { LocalStorageService } from 'src/app/shared/services/local-storage.service';
import { AssetLibrary } from '../asset-library';
import { AssetLibraryService } from '../asset-library.service';

export interface ListElement {
  name: string;
  description: string;
}

@Component({
  selector: 'app-list-asset-library',
  templateUrl: './list-asset-library.component.html',
  styleUrls: ['./list-asset-library.component.scss']
})

export class ListAssetLibraryComponent implements OnInit {

  public isFilter: boolean = false;
  public pageSize = 5;
  public currentPage = 0;
  public totalSize = 0;
  public assetLibraryFilterForm: FormGroup;
  public assetLibraryList: AssetLibrary[] = [];
  public isLoaderActive: boolean = false;
  public isRecordFound: boolean = false;
  public pageNo = 0;
  public params: any = {};
  public userClaimsRolePrivilegeOperations: any[] = [];
  public displayedColumns: string[] = ['name', 'description', 'actions'];
  public array: any;
  public isFilterDone = false;
  public totalRecordCount = 0;
  public filterAsstLibraryName = '';
  public sortColumn = 'Name';
  public sortOrder = Constants.Ascending;
  dataSource = new MatTableDataSource<any>();
  public sortedAssetLibraryList: AssetLibrary[] = [];
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: true }) sort: MatSort;

  constructor(private injector: Injector,
    private fb: FormBuilder,
    private uiLoader: NgxUiLoaderService,
    private _messageDialogService: MessageDialogService,
    private router: Router,
    private localstorageservice: LocalStorageService,
    private assetLibraryService: AssetLibraryService) {
  }

  resetAssetFilterForm() {
    this.assetLibraryFilterForm.controls['filterAssetLibraryName'].setValue('');
    this.filterAsstLibraryName = '';
    this.currentPage = 0;
  }

  searchFilter(searchType) {
    this.isFilterDone = true;
    if (searchType == 'reset') {
      this.resetAssetFilterForm();
      this.getAssetLibraryRecords(null);
      this.isFilter = !this.isFilter;
    }
    else {
      let searchParameter: any = {};
      searchParameter.PagingParameter = {};
      searchParameter.PagingParameter.PageIndex = 1;
      searchParameter.PagingParameter.PageSize = this.pageSize;
      searchParameter.SortParameter = {};
      searchParameter.SortParameter.SortColumn = Constants.Name;
      searchParameter.SortParameter.SortOrder = Constants.Ascending;
      searchParameter.SearchMode = Constants.Contains;
      this.filterAsstLibraryName = this.assetLibraryFilterForm.value.filterAssetLibraryName != null ? this.assetLibraryFilterForm.value.filterAssetLibraryName.trim() : "";
      searchParameter.Name = this.filterAsstLibraryName;
      this.currentPage = 0;
      this.getAssetLibraryRecords(searchParameter);
      this.isFilter = !this.isFilter;
    }
  }

  ngOnInit() {
    this.getAssetLibraryRecords(null)
    this.assetLibraryFilterForm = this.fb.group({
      filterAssetLibraryName: [null],
    });
    var userClaimsDetail = JSON.parse(localStorage.getItem('userClaims'));
    if (userClaimsDetail) {
      this.userClaimsRolePrivilegeOperations = userClaimsDetail.Privileges;
    }
    else {
      this.userClaimsRolePrivilegeOperations = [];
    }
  }

  async getAssetLibraryRecords(searchParameter) {
    let assetLibraryService = this.injector.get(AssetLibraryService);
    if (searchParameter == null) {
      searchParameter = {};
      searchParameter.PagingParameter = {};
      searchParameter.PagingParameter.PageIndex = this.currentPage + 1;
      searchParameter.PagingParameter.PageSize = this.pageSize;
      searchParameter.SortParameter = {};
      searchParameter.SortParameter.SortColumn = Constants.Name;
      searchParameter.SortParameter.SortOrder = Constants.Ascending;
      searchParameter.SearchMode = Constants.Contains;
      searchParameter.Name = this.filterAsstLibraryName;
    }
    var response = await assetLibraryService.getAssetLibrary(searchParameter);
    this.assetLibraryList = response.assestLibraryList;
    this.totalRecordCount = response.RecordCount;
    if (this.assetLibraryList.length == 0 && this.isFilterDone == true) {
      let message = "NO record found";//this.roleListResources['lblNoRecord'] == undefined ? this.ResourceLoadingFailedMsg : this.roleListResources['lblNoRecord']
      this._messageDialogService.openDialogBox('Error', message, Constants.msgBoxError).subscribe(data => {
        if (data == true) {
          this.resetAssetFilterForm();
          this.getAssetLibraryRecords(null);
        }
      });
    }
    this.dataSource = new MatTableDataSource<AssetLibrary>(this.assetLibraryList);
   
    this.dataSource.sort = this.sort;
    this.array = this.assetLibraryList;
    this.totalSize = this.totalRecordCount;
    //this.iterator();
  }

  public handlePage(e: any) {
    this.currentPage = e.pageIndex;
    this.pageSize = e.pageSize;
    //this.iterator();
    this.getAssetLibraryRecords(null);
  }

  private iterator() {
    const end = (this.currentPage + 1) * this.pageSize;
    const start = this.currentPage * this.pageSize;
    const part = this.array.slice(start, end);
    this.dataSource = part;
    this.dataSource.sort = this.sort;
  }

  //Getters for Role Forms
  get filterAssetLibraryName() {
    return this.assetLibraryFilterForm.get('filterAssetLibraryName');
  }

  //Getters for Role Forms
  get filterDeactivateRole() {
    return this.assetLibraryFilterForm.get('DeactivateRole');
  }

  sortData(sort: MatSort) {
    const data = this.assetLibraryList.slice();
    if (!sort.active || sort.direction === '') {
      this.sortedAssetLibraryList = data;
      return;
    }
    if (sort.direction == 'asc') {
      this.sortOrder = Constants.Ascending;
    }
    else {
      this.sortOrder = Constants.Descending;
    }
    if (sort.active == 'name') {
      this.sortColumn = 'Name';
    }
    else if (sort.active == 'description') {
      this.sortColumn = 'Description';
    }
    else if (sort.active == 'active') {
      this.sortColumn = 'IsActive';
    }
    let searchParameter: any = {};
    searchParameter.PagingParameter = {};
    searchParameter.PagingParameter.PageIndex = this.currentPage + 1;
    searchParameter.PagingParameter.PageSize = this.pageSize;
    searchParameter.SortParameter = {};
    searchParameter.SortParameter.SortColumn = this.sortColumn;
    searchParameter.SortParameter.SortOrder = this.sortOrder;
    searchParameter.SearchMode = Constants.Contains;
    this.filterAsstLibraryName = this.assetLibraryFilterForm.value.filterAssetLibraryName != null ? this.assetLibraryFilterForm.value.filterAssetLibraryName.trim() : "";
    searchParameter.Name = this.filterAsstLibraryName;
    this.getAssetLibraryRecords(searchParameter);

  }

  ngAfterViewInit() {
    this.dataSource.paginator = this.paginator;
  }

  closeFilter() {
    this.isFilter = !this.isFilter;
  }

  //this method helps to navigate to view
  navigateToAssetLibraryView(assetLibrary: AssetLibrary) {
    let queryParams = {
      Routeparams: {
        passingparams: {
          "AssetLibraryIdentifier": assetLibrary.Identifier,
        },
        filteredparams: {
          //passing data using json stringify.
          "AssetLibraryName": this.assetLibraryFilterForm.value.filterRoleName != null ? this.assetLibraryFilterForm.value.filterRoleName : ""
        }
      }
    }
    localStorage.setItem("assetLibraryparams", JSON.stringify(queryParams))
    const router = this.injector.get(Router);
    router.navigate(['assetlibrary', 'View']);
  }

  //this method helps to navigate to add
  navigateToAssetLibraryAdd() {
    const router = this.injector.get(Router);
    router.navigate(['assetlibrary', 'Add']);
  }

  //this method helps to navigate edit
  navigateToAssetLibraryEdit(assetLibrary) {
    let queryParams = {
      Routeparams: {
        passingparams: {
          "AssetLibraryIdentifier": assetLibrary.Identifier,
        },
        filteredparams: {
          //passing data using json stringify.
          "AssetLibraryName": this.assetLibraryFilterForm.value.filterRoleName != null ? this.assetLibraryFilterForm.value.filterRoleName : ""
        }
      }
    }
    localStorage.setItem("assetLibraryparams", JSON.stringify(queryParams))
    const router = this.injector.get(Router);
    router.navigate(['assetlibrary', 'Edit']);
  }

  //function written to delete role
  deleteAssetLibrary(role: AssetLibrary) {
    let message = 'Are you sure, you want to delete this record?';
    this._messageDialogService.openConfirmationDialogBox('Confirm', message, Constants.msgBoxWarning).subscribe(async (isConfirmed) => {
      if (isConfirmed) {
        let roleData = [{
          "Identifier": role.Identifier,
        }];

        let isDeleted = await this.assetLibraryService.deleteAssetLibrary(roleData);
        if (isDeleted) {
          let messageString = Constants.recordDeletedMessage;
          this._messageDialogService.openDialogBox('Success', messageString, Constants.msgBoxSuccess);
          this.getAssetLibraryRecords(null);
        }
      }
    });
  }
}

function compareStr(a: string, b: string, isAsc: boolean) {
  return (a.toLowerCase() < b.toLowerCase() ? -1 : 1) * (isAsc ? 1 : -1);
}

function compareNumber(a: number, b: number, isAsc: boolean) {
  return (a < b ? -1 : 1) * (isAsc ? 1 : -1);
}
