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
import { DynamicModule } from 'ng-dynamic-component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { DisableControlDirective } from './../shared/directive/disabled-control.directive';
import { CustomerInformationComponent, AccountInformationComponent, ImageComponent, VideoComponent, SummaryAtGlanceComponent } from './widgetComponent/widgetComponent';

@NgModule({
  declarations: [AddDashboardDesignerComponent, ViewDashboardDesignerComponent, CustomerInformationComponent, 
    AccountInformationComponent, ImageComponent, VideoComponent, SummaryAtGlanceComponent, DisableControlDirective],
  imports: [
    CommonModule, DashboardDesignerRoutingModule, RouterModule, SharedModule, FormsModule, ReactiveFormsModule,
    GridsterModule, LayoutModule, MatSortModule, MatTableModule, MatPaginatorModule, DynamicModule
  ],
  entryComponents: [
    CustomerInformationComponent, 
    AccountInformationComponent,
    ImageComponent,
    VideoComponent,
    SummaryAtGlanceComponent],
})
export class DashboardDesignerModule { }
