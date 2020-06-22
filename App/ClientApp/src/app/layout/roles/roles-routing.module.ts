import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ListComponent } from './list/list.component';
import { ViewComponent } from './view/view.component';
import { AddComponent } from './add/add.component';
import { AuthGuard, UnAuthorisedUrlGuard } from 'src/app/core/guard';

const routes: Routes = [
    {
        path: '',
        children: [
            {
                path: '',
                data: { 'Operation': 'View' },
                component: ListComponent,
                canActivate: [AuthGuard]
            },
            {
                path: 'View',
                data: { 'Operation': 'View' },
                component: ViewComponent,
                canActivate: [UnAuthorisedUrlGuard, AuthGuard]
            },
            {
                path: 'Edit',
                data: { 'Operation': 'Edit' },
                component: AddComponent,
                canActivate: [UnAuthorisedUrlGuard, AuthGuard]
            },
            {
                path: 'Add',
                data: { 'Operation': 'Create' },
                component: AddComponent,
                canActivate: [UnAuthorisedUrlGuard, AuthGuard]
            }
        ],
        data: { 'EntityName': 'Role' }
    }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class RolesRoutingModule { }
