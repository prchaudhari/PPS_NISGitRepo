import { Component, OnInit, Injector } from '@angular/core';
import { BnNgIdleService } from 'bn-ng-idle';
import { Route, Router } from '@angular/router';
import { Location } from "@angular/common";
import { MessageDialogService } from '../shared/services/mesage-dialog.service';
import { Constants, DynamicGlobalVariable } from '../shared/constants/constants';
import { LocalStorageService } from 'src/app/shared/services/local-storage.service';

@Component({
  selector: 'app-layout',
  templateUrl: './layout.component.html',
  styleUrls: ['./layout.component.scss']
})
export class LayoutComponent implements OnInit {

  constructor(
    private bnIdle: BnNgIdleService,
    private location: Location,
    private injector: Injector,
    private messageDialogService: MessageDialogService,
    private router: Router,
    private localstorageservice: LocalStorageService,
    private dynamicGlobalVariable: DynamicGlobalVariable
  ) {
  }
  ngOnInit(): void {
    var userdeatils
    var userClaimsDetail = this.localstorageservice.GetCurrentUser();
    if (userClaimsDetail == null) {
      this.router.navigate(['/login']);
    }
    else {
      if (this.router.url == '/assetlibrary') {

      }
    }
  }

}
