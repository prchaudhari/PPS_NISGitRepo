import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AddDashboardDesignerComponent } from './add-dashboard-designer/add-dashboard-designer.component';
import { ViewDashboardDesignerComponent } from './view-dashboard-designer/view-dashboard-designer.component';

const routes: Routes = [
    {
        path: '',
        children: [
            { path: '', component: AddDashboardDesignerComponent },
            { path: 'Add', component: AddDashboardDesignerComponent },
            { path: 'dashboardDesignerView', component: ViewDashboardDesignerComponent }
        ]
    }];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class DashboardDesignerRoutingModule { }
