import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ListComponent } from './list/list.component';
import { ViewComponent } from './view/view.component';
import { AddComponent } from './add/add.component';


const routes: Routes = [
    {
        path: '',
        children: [
            // { path: '', component: ListComponent },
            // { path: 'Add', component: AddComponent },
            // { path: 'View', component: ViewComponent },
            // { path: 'List', component: ListComponent },
            {
                path: '',
                data: { 'Operation': 'View' },
                component: ListComponent,
            },
            {
                path: 'View',
                data: { 'Operation': 'View' },
                component: ViewComponent,
            },
            {
                path: 'Edit',
                data: { 'Operation': 'Edit' },
                component: AddComponent,
            },
            {
                path: 'Add',
                data: { 'Operation': 'Create' },
                component: AddComponent,
            }
        ]
    }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class RolesRoutingModule { }
