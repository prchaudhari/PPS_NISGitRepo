import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { LoginComponent } from './login/login.component';
import { ConfirmUserComponent } from './confirm-user/confirm-user.component';
import { SelectTenantComponent } from './select-tenant/select-tenant.component';

const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { path: 'selectTenant', component: SelectTenantComponent },
  { path: 'layout', loadChildren: () => import('../app/layout/layout.module').then(m => m.LayoutModule) },
  { path: 'dashboardDesigner', loadChildren: () => import('../app/dashboard-designer/dashboard-designer.module').then(m => m.DashboardDesignerModule) },
  { path: 'confirmuser', component: ConfirmUserComponent },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
