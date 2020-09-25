import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ListComponent } from './list/list.component';
import { AddComponent } from './add/add.component';
import { ViewComponent } from './view/view.component';

const routes: Routes = [
  {
    path: '',
    children: [ 
      { path: '',component: ListComponent },
      { path: 'Edit', component: AddComponent },
      { path: 'Add', component: AddComponent },
      { path: 'View', component: ViewComponent },
    ],
    data: { 'EntityName': 'TenantGroup' }
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class TenantGroupsRoutingModule { }
