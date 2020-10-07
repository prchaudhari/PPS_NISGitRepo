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
      { path: '', component: ListComponent, canActivate: [AuthGuard] },
      { path: "list", component: ListComponent, canActivate: [AuthGuard] },
      { path: 'add', component: AddEditComponent, canActivate: [AuthGuard] },
      { path: 'edit', component: AddEditComponent, canActivate: [AuthGuard] },
      { path: 'view', component: ViewComponent, canActivate: [AuthGuard] },
    ],
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class TenantuserRoutingModule { }
