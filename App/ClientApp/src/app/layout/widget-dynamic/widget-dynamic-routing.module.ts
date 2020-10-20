import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ListComponent } from './list/list.component';
import { AddComponent } from './add/add.component';
import { ViewComponent } from './view/view.component';
import { WidgetdesignerComponent } from './widgetdesigner/widgetdesigner.component';

import { AuthGuard, UnAuthorisedUrlGuard } from 'src/app/core/guard';

const routes: Routes = [
  {
    path: '',
    children: [
      {
        path: '', component: ListComponent,
        canActivate: [AuthGuard]
      },
      {
        path: 'Add', component: AddComponent,
        canActivate: [AuthGuard]
      },
      {
        path: 'View', component: ViewComponent,
        canActivate: [AuthGuard]
      },
      {
        path: 'List', component: ListComponent,
        canActivate: [AuthGuard]
      },
      {
        path: 'Edit', component: AddComponent,
        canActivate: [AuthGuard]
      },
      {
        path: 'AddDesinger', component: WidgetdesignerComponent,
        canActivate: [AuthGuard]
      }
    ],
    data: { 'EntityName': 'Role' }
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class WidgetDynamicRoutingModule { }
