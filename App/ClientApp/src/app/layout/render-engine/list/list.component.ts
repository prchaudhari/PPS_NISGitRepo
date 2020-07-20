import { Component, OnInit, Injector, ChangeDetectorRef, ViewChild, OnDestroy } from '@angular/core';
import { RenderEngine, RenderEngineSearchParameter } from '../renderengine.model';
import { RenderengineService } from '../renderengine.service';
import { FormGroup, FormBuilder } from '@angular/forms';
import { NgxUiLoaderService } from 'ngx-ui-loader';
import { Router, NavigationEnd } from '@angular/router';
import { Constants, ErrorMessageConstants } from 'src/app/shared/constants/constants';
import { MessageDialogService } from 'src/app/shared/services/mesage-dialog.service';
import { LocalStorageService } from 'src/app/shared/services/local-storage.service';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';

@Component({
  selector: 'app-list',
  templateUrl: './list.component.html',
  styles: []
})
export class ListComponent implements OnInit {

  //public variables
  public isFilter: boolean = false;
  public renderEngines: RenderEngine[] = [];
  public isLoaderActive: boolean = false;
  public isRecordFound: boolean = false;
  public pageNo = 0;
  public renderEngineFilterForm: FormGroup;
  public params: any = {};
  public userClaimsRolePrivilegeOperations: any[] = [];
  public array: any;
  public displayedColumns: string[] = ['name', 'url', 'prioritylevel', 'concurrencycount', 'active', 'actions'];
  public dataSource: any;

  public pageSize = 5;
  public currentPage = 0;
  public totalSize = 0;
  public previousPageLabel: string;
  public isFilterDone = false;
  public sortedRenderEngines: RenderEngine[] = [];

  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: true }) sort: MatSort;

  public handlePage(e: any) {
    this.currentPage = e.pageIndex;
    this.pageSize = e.pageSize;
    this.iterator();
  }

  private iterator() {
    const end = (this.currentPage + 1) * this.pageSize;
    const start = this.currentPage * this.pageSize;
    const part = this.array.slice(start, end);
    this.dataSource = part;
    this.dataSource.sort = this.sort;
  }

  //Getters for Render engine filter Forms
  get filterRenderEngineName() {
    return this.renderEngineFilterForm.get('filterRenderEngineName');
  }
  get filteDeactivateRenderEngine() {
    return this.renderEngineFilterForm.get('filteDeactivateRenderEngine');
  }

  constructor(private injector: Injector,
    private fb: FormBuilder,
    private uiLoader: NgxUiLoaderService,
    private _messageDialogService: MessageDialogService,
    private router: Router,
    private localstorageservice: LocalStorageService,
    private renderEngineService: RenderengineService) { 
      this.sortedRenderEngines = this.renderEngines.slice();
      if (localStorage.getItem('renderEngineEditRouteparams')) {
        localStorage.removeItem('renderEngineEditRouteparams');
    }
    if (localStorage.getItem('renderEngineViewRouteparams')) {
        localStorage.removeItem('renderEngineViewRouteparams');
    }
    }

  sortData(sort: MatSort) {
    const data = this.renderEngines.slice();
    if (!sort.active || sort.direction === '') {
      this.sortedRenderEngines = data;
      return;
    }

    this.sortedRenderEngines = data.sort((a, b) => {
      const isAsc = sort.direction === 'asc';
      switch (sort.active) {
        case 'name': return compareStr(a.RenderEngineName, b.RenderEngineName, isAsc);
        case 'url': return compareStr(a.URL == null ? '' : a.URL, b.URL == null ? '' : b.URL, isAsc);
        case 'prioritylevel': return compareNumber(a.PriorityLevel, b.PriorityLevel, isAsc);
        case 'concurrencycount': return compareNumber(a.NumberOfThread, b.NumberOfThread, isAsc);
        case 'active': return compareNumber(a.IsActive == true ? 1 : 0, b.IsActive == true ? 1 : 0, isAsc);
        default: return 0;
      }
    });
    this.dataSource = new MatTableDataSource<RenderEngine>(this.sortedRenderEngines);
    this.dataSource.sort = this.sort;
    this.array = this.sortedRenderEngines;
    this.totalSize = this.array.length;
    this.iterator();
  }

  ngOnInit() {

    this.getRenderEngines(null)
    this.renderEngineFilterForm = this.fb.group({
        filterRenderEngineName: [null],
    });
    var userClaimsDetail = JSON.parse(localStorage.getItem('userClaims'));
    if (userClaimsDetail) {
      this.userClaimsRolePrivilegeOperations = userClaimsDetail.Privileges;
    }
    else {
      this.userClaimsRolePrivilegeOperations = [];
    }
  }

  //This method has been used for fetching role records
  async getRenderEngines(searchParameter) {
    let renderengineService = this.injector.get(RenderengineService);
    if (searchParameter == null) {
      searchParameter = {};
      searchParameter.PagingParameter = {};
      searchParameter.PagingParameter.PageIndex = Constants.DefaultPageIndex;
      searchParameter.PagingParameter.PageSize = Constants.DefaultPageSize;
      searchParameter.SortParameter = {};
      searchParameter.SortParameter.SortColumn = Constants.Name;
      searchParameter.SortParameter.SortOrder = Constants.Ascending;
      searchParameter.SearchMode = Constants.Contains;
    }
    this.renderEngines = await renderengineService.getRenderEngines(searchParameter);
    if (this.renderEngines.length > 0) {
        this.isRecordFound = true;
    }
    if (this.renderEngines.length == 0 && this.isFilterDone == true) {
      let message = ErrorMessageConstants.getNoRecordFoundMessage;
      this._messageDialogService.openDialogBox('Error', message, Constants.msgBoxError).subscribe(data => {
        if (data == true) {
          this.resetRenderEngineFilterForm();
          this.getRenderEngines(null);
        }
      });
    }
    this.dataSource = new MatTableDataSource<RenderEngine>(this.renderEngines);
    this.dataSource.sort = this.sort;
    this.array = this.renderEngines;
    this.totalSize = this.array.length;
    this.iterator();
  }

  //This method has been used for fetching search records
  searchFilter(searchType) {
    this.isFilterDone = true;
    if (searchType == 'reset') {
      this.resetRenderEngineFilterForm();
      this.getRenderEngines(null);
      this.isFilter = !this.isFilter;
    }
    else {
      let searchParameter: any = {};
      searchParameter.PagingParameter = {};
      searchParameter.PagingParameter.PageIndex = Constants.DefaultPageIndex;
      searchParameter.PagingParameter.PageSize = Constants.DefaultPageSize;
      searchParameter.SortParameter = {};
      searchParameter.SortParameter.SortColumn = Constants.Name;
      searchParameter.SortParameter.SortOrder = Constants.Ascending;
      searchParameter.SearchMode = Constants.Contains;
      searchParameter.RenderEngineName = this.renderEngineFilterForm.value.filterRenderEngineName != null ? this.renderEngineFilterForm.value.filterRenderEngineName.trim() : "";
      this.currentPage = 0;
      this.getRenderEngines(searchParameter);
      this.isFilter = !this.isFilter;
    }
  }

  navigateToAddRenderEngine() {
    this.router.navigate(['renderengines', 'Add']);
  }

  navigateToViewRenderEngine(renderEngine: RenderEngine) {
    let queryParams = {
      Routeparams: {
        passingparams: {
          "RenderEngineIdentifier": renderEngine.Identifier,
        }
      }
    }
    localStorage.setItem("renderEngineViewRouteparams", JSON.stringify(queryParams))
    this.router.navigate(['renderengines', 'View']);
  }

  navigateToEditRederEngine(renderEngine: RenderEngine) {
    let queryParams = {
      Routeparams: {
        passingparams: {
          "RenderEngineIdentifier": renderEngine.Identifier,
        }
      }
    }
    localStorage.setItem("renderEngineEditRouteparams", JSON.stringify(queryParams))
    this.router.navigate(['renderengines', 'Edit']);
  }

  async deleteRenderEngine(renderEngine: RenderEngine) {
    let message = 'Are you sure, you want to delete this record?';
    this._messageDialogService.openConfirmationDialogBox('Confirm', message, Constants.msgBoxWarning).subscribe(async (isConfirmed) => {
      if (isConfirmed) {
        let renderEngineData = [{
          "Identifier": renderEngine.Identifier,
        }];
        let isDeleted = await this.renderEngineService.deleteRenderEngine(renderEngineData);
          if (isDeleted) {
            let messageString = Constants.recordDeletedMessage;
            this._messageDialogService.openDialogBox('Success', messageString, Constants.msgBoxSuccess);
            this.getRenderEngines(null);
          }
      }
    });
  }

  async DeactivateRenderEngine(renderEngine: RenderEngine) {
    let message = "Are you sure, you want to deactivate this record?";
    this._messageDialogService.openConfirmationDialogBox('Confirm', message, Constants.msgBoxWarning).subscribe(async (isConfirmed) => {
      if (isConfirmed) {
        let renderEngineData = [{
          "Identifier": renderEngine.Identifier,
        }];
        let resultFlag = await this.renderEngineService.deactivateRenderEngine(renderEngineData);
        if (resultFlag) {
          let messageString = Constants.recordDeactivatedMessage;
          this._messageDialogService.openDialogBox('Success', messageString, Constants.msgBoxSuccess);
          this.getRenderEngines(null);
        }
      }
    });
  }

  async ActivateRenderEngine(renderEngine: RenderEngine) {
    let renderEngineData = [{
      "Identifier": renderEngine.Identifier,
    }];
    let resultFlag = await this.renderEngineService.activateRenderEngine(renderEngineData);
    if (resultFlag) {
      let messageString = Constants.recordActivatedMessage;
      this._messageDialogService.openDialogBox('Success', messageString, Constants.msgBoxSuccess);
      this.getRenderEngines(null);
    }
  }

  resetRenderEngineFilterForm() {
    this.renderEngineFilterForm.patchValue({
      filterRenderEngineName: null,
    });
  }

}

function compareStr(a: string, b: string, isAsc: boolean) {
  return (a.toLowerCase() < b.toLowerCase() ? -1 : 1) * (isAsc ? 1 : -1);
}

function compareNumber(a: number, b: number, isAsc: boolean) {
  return (a < b ? -1 : 1) * (isAsc ? 1 : -1);
}
