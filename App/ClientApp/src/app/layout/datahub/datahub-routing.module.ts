import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AuthGuard, UnAuthorisedUrlGuard } from 'src/app/core/guard';

import { ViewComponent } from './view/view.component';
import { ListEtlscheduleComponent } from './list-etlschedule/list-etlschedule.component';
import { EtlscheduledetailComponent } from './etlscheduledetail/etlscheduledetail.component';
import { ListEtlschedulebatchlogComponent } from './list-etlschedulebatchlog/list-etlschedulebatchlog.component';
import { ListEtlschedulebatchlogdetailsComponent } from './list-etlschedulebatchlogdetails/list-etlschedulebatchlogdetails.component';


const routes: Routes = [
  {
    path: '',
    children: [
        { path: '', component: ViewComponent,
        canActivate: [AuthGuard] },
        { path: 'View', component: ViewComponent,
        canActivate: [AuthGuard] },
        { path: 'list-etlschedule', component: ListEtlscheduleComponent,
        canActivate: [AuthGuard] },
        { path: 'etlscheduledetail', component: EtlscheduledetailComponent,
        canActivate: [AuthGuard] },
        { path: 'list-etlschedulebatchlog', component: ListEtlschedulebatchlogComponent,
        canActivate: [AuthGuard] },
        { path: 'list-etlschedulebatchlogdetails', component: ListEtlschedulebatchlogdetailsComponent,
        canActivate: [AuthGuard] },
    ]
}
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class DatahubRoutingModule { }
