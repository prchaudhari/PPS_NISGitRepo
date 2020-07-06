import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ListComponent } from './list/list.component';
import { MatSortModule } from '@angular/material/sort';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule } from '@angular/material/paginator';
import { AddComponent } from './add/add.component';
import { ViewComponent } from './view/view.component';
import { OwlDateTimeModule, OwlNativeDateTimeModule } from 'ng-pick-datetime';
import { SharedModule } from '../../shared/modules/shared.module';
import { HistoryComponent } from './history/history.component';
import { ScheduleManagementRoutingModule } from './schedule-management-routing.module';

@NgModule({
  declarations: [ListComponent, AddComponent, ViewComponent, HistoryComponent],
  imports: [
    CommonModule,
      ScheduleManagementRoutingModule,
      MatTableModule,
      MatSortModule,
      NgbModule,
      MatPaginatorModule,
    OwlDateTimeModule, OwlNativeDateTimeModule, SharedModule, FormsModule,
    ReactiveFormsModule,
  ]
})
export class ScheduleManagementModule { }

