import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { SharedModule } from '../shared/modules/shared.module';
import { DashboardDesignerRoutingModule } from './dashboard-designer-routing.module';
import { AddDashboardDesignerComponent } from './add-dashboard-designer/add-dashboard-designer.component';
import { ViewDashboardDesignerComponent } from './view-dashboard-designer/view-dashboard-designer.component';
import { GridsterModule } from 'angular-gridster2';
import { LayoutModule } from '../layout/layout.module';
import { MatSortModule } from '@angular/material/sort';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule } from '@angular/material/paginator';

@NgModule({
  declarations: [AddDashboardDesignerComponent, ViewDashboardDesignerComponent],
  imports: [
    CommonModule,
      DashboardDesignerRoutingModule,
      RouterModule,
      SharedModule, GridsterModule,
      LayoutModule,
      MatSortModule,
      MatTableModule,
      MatPaginatorModule
  ]
})
export class DashboardDesignerModule { }
