import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ListComponent } from './list/list.component';
import { ViewComponent } from './view/view.component';
import { AddComponent } from './add/add.component';
import { HistoryComponent } from './history/history.component';
import { AuthGuard, UnAuthorisedUrlGuard } from 'src/app/core/guard';

const routes: Routes = [
    {
        path: '',
        children: [
            { path: '', component: ListComponent,
            canActivate: [AuthGuard] },
            { path: 'Add', component: AddComponent,
            canActivate: [AuthGuard] },
            { path: 'View', component: ViewComponent,
            canActivate: [AuthGuard] },
            { path: 'List', component: ListComponent,
            canActivate: [AuthGuard] },
            { path: 'History', component: HistoryComponent,
            canActivate: [AuthGuard] },
        ]
    }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ScheduleManagementRoutingModule { }
