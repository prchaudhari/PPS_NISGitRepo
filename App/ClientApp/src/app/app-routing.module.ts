import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { LoginComponent } from './login/login.component';
import { ConfirmUserComponent } from './confirm-user/confirm-user.component';
import { SelectTenantComponent } from './select-tenant/select-tenant.component';
import { EnvironmentSpecificResolver } from './core/services/env-specific/environment-specific-resolver.service';

const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full', resolve: { envSpecific: EnvironmentSpecificResolver } },
  { path: 'login', component: LoginComponent, resolve: { envSpecific: EnvironmentSpecificResolver }},
  { path: 'selectTenant', component: SelectTenantComponent, resolve: { envSpecific: EnvironmentSpecificResolver } },
  { path: 'layout', loadChildren: () => import('../app/layout/layout.module').then(m => m.LayoutModule) , resolve: { envSpecific: EnvironmentSpecificResolver }},
  { path: 'dashboardDesigner', loadChildren: () => import('../app/dashboard-designer/dashboard-designer.module').then(m => m.DashboardDesignerModule), resolve: { envSpecific: EnvironmentSpecificResolver } },
  { path: 'confirmuser', component: ConfirmUserComponent, resolve: { envSpecific: EnvironmentSpecificResolver } },
];


@NgModule({
  imports: [RouterModule.forRoot(routes)],
  providers: [
    EnvironmentSpecificResolver,
  ],
  exports: [RouterModule],
})
export class AppRoutingModule { }
