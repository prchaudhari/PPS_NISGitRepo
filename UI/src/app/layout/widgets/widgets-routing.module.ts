import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ListComponent } from './list/list.component';
import { ViewComponent } from './view/view.component';
import { AddComponent } from './add/add.component';

const routes: Routes = [
    {
        path: '',
        children: [
            { path: '', component: AddComponent },
            { path: 'AvailableWidgets', component: AddComponent },
            { path: 'View', component: ViewComponent },
            { path: 'List', component: ListComponent }
        ]
    }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class WidgetsRoutingModule { }
