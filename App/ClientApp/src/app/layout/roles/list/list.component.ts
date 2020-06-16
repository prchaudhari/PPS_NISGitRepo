import { Component, OnInit, ViewChild, Injector } from '@angular/core';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { NgxUiLoaderService } from 'ngx-ui-loader';
import { HttpClient, HttpResponse, HttpHeaders, HttpEvent } from '@angular/common/http';
import { LoginService } from '../../../login/login.service';
import { MessageDialogService } from 'src/app/shared/services/mesage-dialog.service';
import { LocalStorageService } from 'src/app/shared/services/local-storage.service';
import { Constants } from 'src/app/shared/constants/constants';

export interface ListElement {
    name: string;
    description: string;
}

@Component({
  selector: 'app-list',
  templateUrl: './list.component.html',
  styleUrls: ['./list.component.scss']
})
export class ListComponent implements OnInit {
    
    public isFilter: boolean = false;
    public pageSize = 5;
    public currentPage = 0;
    public totalSize = 0;
    public roleList: any = [];

    closeFilter() {
        this.isFilter = !this.isFilter;
    }
    displayedColumns: string[] = ['name', 'description','actions'];
    dataSource = new MatTableDataSource<any>();

    @ViewChild(MatSort, { static: true }) sort: MatSort;
    @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;

    constructor(
        private loginService: LoginService,
        private http: HttpClient,
        private localstorageservice: LocalStorageService,
        private spinner: NgxUiLoaderService,
        private injector: Injector,
        private _messageDialogService: MessageDialogService,
      ) {
        this.getRoles();
      }

    ngOnInit() {

    }
    ngAfterViewInit() {
        this.dataSource.paginator = this.paginator;
    }

    async getRoles() {
        let searchParameter: any = {};
        searchParameter.PagingParameter = {};
        searchParameter.PagingParameter.PageIndex = Constants.DefaultPageIndex;
        searchParameter.PagingParameter.PageSize = Constants.DefaultPageSize;
        searchParameter.SortParameter = {};
        searchParameter.SortParameter.SortColumn = Constants.Name;
        searchParameter.SortParameter.SortOrder = Constants.Ascending;
        searchParameter.SearchMode = Constants.Contains;
        //searchParameter.GetPrivileges = true;
        this.roleList =await this.loginService.getRoles(searchParameter);
        this.dataSource = new MatTableDataSource(this.roleList);
        this.dataSource.sort = this.sort;

        //to hide tooltip
        const paginatorIntl = this.paginator._intl;
        paginatorIntl.nextPageLabel = '';
        paginatorIntl.previousPageLabel = '';
        paginatorIntl.firstPageLabel = '';
        paginatorIntl.lastPageLabel = '';
      }

 
}
