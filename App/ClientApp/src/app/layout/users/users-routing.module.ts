import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ListComponent } from './list/list.component';
import { ViewComponent } from './view/view.component';
import { UserAddEditComponent } from './add/add-edit.component';
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
        path: "userlist",
        component: ListComponent,
        canActivate: [UnAuthorisedUrlGuard, AuthGuard]
      },
      {
        path: 'userAdd',
        data: { 'Operation': 'Create' },
        component: UserAddEditComponent,
        canActivate: [UnAuthorisedUrlGuard, AuthGuard]
      },
      {
        path: 'userEdit',
        data: { 'Operation': 'Edit' },
        component: UserAddEditComponent,
        canActivate: [UnAuthorisedUrlGuard, AuthGuard]
      },
      {
        path: 'userView',
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
export class UsersRoutingModule { }
