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

const routes: Routes = [
  {
    path: '',
    component: LayoutComponent,
    children: [
      { path: 'roles', loadChildren: () => import('./roles/roles.module').then(m => m.RolesModule) },
      { path: 'user', loadChildren: () => import('./users/users.module').then(m => m.UsersModule) },
      { path: 'widgets', loadChildren: () => import('./widgets/widgets.module').then(m => m.WidgetsModule) },
      { path: 'pages', loadChildren: () => import('./template/template.module').then(m => m.TemplateModule) },
      { path: 'schedulemanagement', loadChildren: () => import('./schedule-management/schedule-management.module').then(m => m.ScheduleManagementModule) },
      { path: 'statementdefination', loadChildren: () => import('./statement-defination/statement-defination.module').then(m => m.StatementDefinationModule) },
      { path: 'assetlibrary', loadChildren: () => import('./asset-libraries/asset-libraries.module').then(m => m.AssetLibrariesModule) },
      { path: 'renderengines', loadChildren: () => import('./render-engine/render-engine.module').then(m => m.RenderEngineModule) },
      { path: 'dashboard', component: DashboardComponent },
      { path: 'changepassword', component: ChangePasswordComponent },
      { path: 'profile', component: ProfileComponent },
      { path: 'logs', component: LogsComponent },
      { path: 'logsDetails', component: LogsDetailsComponent },
      { path: 'sourceData', component: SourcedataComponent },
      { path: 'analytics', component: AnalyticsComponent },
      { path: 'settings', component: AssetSettingsComponent }
    ]
  }


];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class LayoutRoutingModule { }
