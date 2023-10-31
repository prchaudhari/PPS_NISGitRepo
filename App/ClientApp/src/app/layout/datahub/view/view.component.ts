import { Component, OnInit, Injector, ChangeDetectorRef, ViewChild, OnDestroy } from '@angular/core';
import { Router, NavigationEnd } from '@angular/router';

import { DataHubService } from '../datahub.service';
import { MessageDialogService } from 'src/app/shared/services/mesage-dialog.service';
import { Constants, ErrorMessageConstants } from 'src/app/shared/constants/constants';
import { NgxUiLoaderService } from 'ngx-ui-loader';

@Component({
  selector: 'app-view',
  templateUrl: './view.component.html',
  styleUrls: ['./view.component.scss']
})
export class ViewComponent implements OnInit {
  public isFilter: boolean = false;
  closeFilter() {
    this.isFilter = !this.isFilter;
  }

  constructor(private injector: Injector, private router: Router,
     private dataHubService : DataHubService, private _messageDialogService: MessageDialogService,
     private uiLoader: NgxUiLoaderService) 
  { }  

  ngOnInit() {
  }

  //this method helps to navigate to ETL Schedule List Page
  navigateToListETLSchedule() {
    const router = this.injector.get(Router);
    router.navigate(['datahub', 'list-etlschedule']);
  }
}
