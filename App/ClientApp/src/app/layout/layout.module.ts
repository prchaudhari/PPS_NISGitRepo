import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
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
import { BnNgIdleService } from 'bn-ng-idle';
import { DynamicGlobalVariable } from '../shared/constants/constants';
import { AssetSettingsComponent } from './asset-settings/asset-settings.component';
import { NgMultiSelectDropDownModule } from 'ng-multiselect-dropdown';
import { StatementSearchComponent } from './statement-search/statement-search.component';
import { TenantConfigurationComponent } from './tenant-configuration/tenant-configuration.component';
import { WindowRef } from '../core/services/window-ref.service';
import { DatePipe } from '@angular/common';
import { CountryComponent } from './country/country.component';
import { ThemeConfigurationComponent } from './theme-configuration/theme-configuration.component';
import { ContacttypeComponent } from './contacttype/contacttype.component';
import { MultiTenantUserAccessMapComponent } from './multi-tenant-user-access-map/multi-tenant-user-access-map.component';
import { InstancemanagerdashboardComponent } from './instancemanagerdashboard/instancemanagerdashboard.component';
import { GroupmanagerdashboardComponent } from './groupmanagerdashboard/groupmanagerdashboard.component';
@NgModule({
  declarations: [DashboardComponent, LayoutComponent, ChangePasswordComponent, ProfileComponent, LogsComponent, AnalyticsComponent, LogsDetailsComponent, SourcedataComponent, AssetSettingsComponent, StatementSearchComponent, TenantConfigurationComponent, CountryComponent, ThemeConfigurationComponent, ContacttypeComponent, MultiTenantUserAccessMapComponent, InstancemanagerdashboardComponent, GroupmanagerdashboardComponent],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    LayoutRoutingModule,
    SharedModule,
    RouterModule,
    NgbModule,
    OwlDateTimeModule,
    OwlNativeDateTimeModule,
    NgMultiSelectDropDownModule.forRoot()
  ],
  providers: [
    BnNgIdleService, DynamicGlobalVariable, WindowRef, DatePipe
  ],
 
})
export class LayoutModule { }
