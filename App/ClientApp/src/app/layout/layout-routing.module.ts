import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { DashboardComponent } from './dashboard/dashboard.component';
import { LayoutComponent } from './layout.component';
import { ChangePasswordComponent } from './change-password/change-password.component';
import { ProfileComponent } from './profile/profile.component';
import { LogsComponent } from './logs/logs.component';
import { AnalyticsComponent } from './analytics/analytics.component';
import { LogsDetailsComponent } from './logs-details/logs-details.component';
import { SourcedataComponent } from './sourcedata/sourcedata.component';
import { AssetSettingsComponent } from './asset-settings/asset-settings.component';
import { StatementSearchComponent } from './statement-search/statement-search.component';
import { TenantConfigurationComponent } from './tenant-configuration/tenant-configuration.component';
import { CountryComponent } from './country/country.component';
import { ThemeConfigurationComponent } from './theme-configuration/theme-configuration.component';
import { ContacttypeComponent} from './contacttype/contacttype.component'
import { MultiTenantUserAccessMapComponent } from './multi-tenant-user-access-map/multi-tenant-user-access-map.component';
import { InstancemanagerdashboardComponent } from './instancemanagerdashboard/instancemanagerdashboard.component';
import { GroupmanagerdashboardComponent } from './groupmanagerdashboard/groupmanagerdashboard.component';
import { EnvironmentSpecificResolver } from '../core/services/env-specific/environment-specific-resolver.service';
import { PagetypeComponent } from './pagetype/pagetype.component';

const routes: Routes = [
  {
    path: '',
    component: LayoutComponent,
    children: [
      { path: 'roles', loadChildren: () => import('./roles/roles.module').then(m => m.RolesModule) , resolve: { envSpecific: EnvironmentSpecificResolver }},
      { path: 'user', loadChildren: () => import('./users/users.module').then(m => m.UsersModule) , resolve: { envSpecific: EnvironmentSpecificResolver }},
      { path: 'widgets', loadChildren: () => import('./widgets/widgets.module').then(m => m.WidgetsModule) , resolve: { envSpecific: EnvironmentSpecificResolver }},
      { path: 'pages', loadChildren: () => import('./template/template.module').then(m => m.TemplateModule) , resolve: { envSpecific: EnvironmentSpecificResolver }},
      { path: 'tenants', loadChildren: () => import('./tenants/tenants.module').then(m => m.TenantsModule) , resolve: { envSpecific: EnvironmentSpecificResolver }},
      { path: 'tenantgroups', loadChildren: () => import('./tenant-groups/tenant-groups.module').then(m => m.TenantGroupsModule) , resolve: { envSpecific: EnvironmentSpecificResolver }},
      { path: 'dynamicwidget', loadChildren: () => import('./widget-dynamic/widget-dynamic.module').then(m => m.WidgetDynamicModule), resolve: { envSpecific: EnvironmentSpecificResolver } },
      { path: 'schedulemanagement', loadChildren: () => import('./schedule-management/schedule-management.module').then(m => m.ScheduleManagementModule) , resolve: { envSpecific: EnvironmentSpecificResolver }},
      { path: 'statementdefination', loadChildren: () => import('./statement-defination/statement-defination.module').then(m => m.StatementDefinationModule) , resolve: { envSpecific: EnvironmentSpecificResolver }},
      { path: 'assetlibrary', loadChildren: () => import('./asset-libraries/asset-libraries.module').then(m => m.AssetLibrariesModule) , resolve: { envSpecific: EnvironmentSpecificResolver }},
      { path: 'renderengines', loadChildren: () => import('./render-engine/render-engine.module').then(m => m.RenderEngineModule) , resolve: { envSpecific: EnvironmentSpecificResolver }},
      { path: 'tenantusers', loadChildren: () => import('./tenantuser/tenantuser.module').then(m => m.TenantuserModule), resolve: { envSpecific: EnvironmentSpecificResolver } },
      { path: 'tenantgroupusers', loadChildren: () => import('./tenant-group-users/tenant-group-users.module').then(m => m.TenantGroupUsersModule) , resolve: { envSpecific: EnvironmentSpecificResolver }},
      { path: 'dashboard', component: DashboardComponent , resolve: { envSpecific: EnvironmentSpecificResolver }},
      { path: 'changepassword', component: ChangePasswordComponent , resolve: { envSpecific: EnvironmentSpecificResolver }},
      { path: 'profile', component: ProfileComponent , resolve: { envSpecific: EnvironmentSpecificResolver }},
      { path: 'logs', component: LogsComponent, resolve: { envSpecific: EnvironmentSpecificResolver } },
      { path: 'logsDetails', component: LogsDetailsComponent , resolve: { envSpecific: EnvironmentSpecificResolver }},
      { path: 'sourceData', component: SourcedataComponent , resolve: { envSpecific: EnvironmentSpecificResolver }},
      { path: 'analytics', component: AnalyticsComponent , resolve: { envSpecific: EnvironmentSpecificResolver }},
      { path: 'settings', component: AssetSettingsComponent , resolve: { envSpecific: EnvironmentSpecificResolver }},
      { path: 'statemenetsearch', component: StatementSearchComponent , resolve: { envSpecific: EnvironmentSpecificResolver }},
      { path: 'tenantConfiguration', component: TenantConfigurationComponent , resolve: { envSpecific: EnvironmentSpecificResolver }},
      { path: 'country', component: CountryComponent , resolve: { envSpecific: EnvironmentSpecificResolver }},
      { path: 'themeConfiguration', component: ThemeConfigurationComponent , resolve: { envSpecific: EnvironmentSpecificResolver }},
      { path: 'contacttype', component: ContacttypeComponent , resolve: { envSpecific: EnvironmentSpecificResolver }},
      { path: 'multiTenantUserAccess', component: MultiTenantUserAccessMapComponent, resolve: { envSpecific: EnvironmentSpecificResolver } },
      { path: 'groupManagerDashboard', component: GroupmanagerdashboardComponent, resolve: { envSpecific: EnvironmentSpecificResolver } },
      { path: 'instanceManagerDashboard', component: InstancemanagerdashboardComponent, resolve: { envSpecific: EnvironmentSpecificResolver } },
      { path: 'pagetype', component: PagetypeComponent, resolve: { envSpecific: EnvironmentSpecificResolver }},
      { path: 'datahub', loadChildren: () => import('./datahub/datahub.module').then(m => m.DatahubModule), resolve: { envSpecific: EnvironmentSpecificResolver } },
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class LayoutRoutingModule { }
