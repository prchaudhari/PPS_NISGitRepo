import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LayoutComponent } from './layout.component';
import { LayoutRoutingModule } from './layout-routing.module';
import { DashboardComponent } from './dashboard/dashboard.component';
import { RouterModule } from '@angular/router';
import { SharedModule } from '../shared/modules/shared.module';
import { ChangePasswordComponent } from './change-password/change-password.component';
import { ProfileComponent } from './profile/profile.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { OwlDateTimeModule, OwlNativeDateTimeModule } from 'ng-pick-datetime';
import { LogsComponent } from './logs/logs.component';
import { AnalyticsComponent } from './analytics/analytics.component';
import { LogsDetailsComponent } from './logs-details/logs-details.component';
import { SourcedataComponent } from './sourcedata/sourcedata.component';

@NgModule({
    declarations: [DashboardComponent, LayoutComponent, ChangePasswordComponent, ProfileComponent, LogsComponent, AnalyticsComponent, LogsDetailsComponent, SourcedataComponent],
  imports: [
    CommonModule,
    LayoutRoutingModule,
    SharedModule,
      RouterModule,
      NgbModule,
      OwlDateTimeModule,
      OwlNativeDateTimeModule

  ]
})
export class LayoutModule { }