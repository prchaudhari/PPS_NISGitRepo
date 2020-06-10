import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ScheduleManagementRoutingModule } from './schedule-management-routing.module';
import { ListComponent } from './list/list.component';
import { AddComponent } from './add/add.component';
import { ViewComponent } from './view/view.component';
import { HistoryComponent } from './history/history.component';
import { MatSortModule } from '@angular/material/sort';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule } from '@angular/material/paginator';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { OwlDateTimeModule, OwlNativeDateTimeModule } from 'ng-pick-datetime';

@NgModule({
  declarations: [ListComponent, AddComponent, ViewComponent, HistoryComponent],
  imports: [
    CommonModule,
      ScheduleManagementRoutingModule,
      MatTableModule,
      MatSortModule,
      NgbModule,
      MatPaginatorModule,
      OwlDateTimeModule, OwlNativeDateTimeModule
  ]
})
export class ScheduleManagementModule { }

