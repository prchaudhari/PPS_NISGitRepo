import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ListComponent } from './list/list.component';
import { ViewComponent } from './view/view.component';
import { AddEditComponent } from './add-edit/add-edit.component';
import { AuthGuard, UnAuthorisedUrlGuard } from 'src/app/core/guard';

const routes: Routes = [
  {
    path: '',
    children: [
      {
        path: '',
        component: ListComponent,
        canActivate: [AuthGuard]
      },
      {
        path: "list",
        component: ListComponent,
        canActivate: [UnAuthorisedUrlGuard, AuthGuard]
      },
      {
        path: 'add',
        data: { 'Operation': 'Create' },
        component: AddEditComponent,
        canActivate: [UnAuthorisedUrlGuard, AuthGuard]
      },
      {
        path: 'edit',
        data: { 'Operation': 'Edit' },
        component: AddEditComponent,
        canActivate: [UnAuthorisedUrlGuard, AuthGuard]
      },
      {
        path: 'view',
        data: { 'Operation': 'View' },
        component: ViewComponent,
        canActivate: [UnAuthorisedUrlGuard, AuthGuard]
      },
      //{
      //  path: 'profile', component: ProfileComponent,
      //  canActivate: [AuthGuard]
      //},
    ],
    data: { 'EntityName': 'User' }
  }
];


@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class TenantuserRoutingModule { }
