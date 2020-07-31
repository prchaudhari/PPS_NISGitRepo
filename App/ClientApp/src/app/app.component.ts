
import { Component, OnInit, Injector } from '@angular/core';
import { BnNgIdleService } from 'bn-ng-idle';
import { Route, Router } from '@angular/router';
import { Location } from "@angular/common";
import { MessageDialogService } from './shared/services/mesage-dialog.service';
import { Constants,DynamicGlobalVariable } from './shared/constants/constants';
import { LocalStorageService } from 'src/app/shared/services/local-storage.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})

export class AppComponent {
  title = 'UI';
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
  public isMessageBoxDisplyed = false;

  ngOnInit(): void {
    this.bnIdle.startWatching(1800).subscribe((isTimedOut: boolean) => {
      if (isTimedOut) {
        console.log('session expired');
        console.log(this.router.url);
        if (this.router.url != "/login") {
          if (this.dynamicGlobalVariable.IsSessionExpireMessageDisplyed == false) {
            this.dynamicGlobalVariable.IsSessionExpireMessageDisplyed = true;
            this.messageDialogService.openDialogBox('Error', "Session expired please login again", Constants.msgBoxError);
            this.localstorageservice.removeLocalStorageData();
            this.router.navigate(['login']);
          }
        }
      }
    });
  }
}
