import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { DatahubRoutingModule } from './datahub-routing.module';
import { ViewComponent } from './view/view.component';

import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatSortModule } from '@angular/material/sort';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule } from '@angular/material/paginator';
import { OwlDateTimeModule, OwlNativeDateTimeModule } from 'ng-pick-datetime';
import { SharedModule } from '../../shared/modules/shared.module';
import { WindowRef } from '../../core/services/window-ref.service';
import { ListEtlscheduleComponent } from './list-etlschedule/list-etlschedule.component';
import { ListEtlschedulebatchlogComponent } from './list-etlschedulebatchlog/list-etlschedulebatchlog.component';
import { ListEtlschedulebatchlogdetailsComponent } from './list-etlschedulebatchlogdetails/list-etlschedulebatchlogdetails.component';
import { EtlscheduledetailComponent } from './etlscheduledetail/etlscheduledetail.component';

@NgModule({
  declarations: [ViewComponent, ListEtlscheduleComponent, ListEtlschedulebatchlogComponent, ListEtlschedulebatchlogdetailsComponent, EtlscheduledetailComponent],
  imports: [
    CommonModule,
    DatahubRoutingModule,
    MatTableModule,
    MatSortModule,
    NgbModule,
    MatPaginatorModule,
    OwlDateTimeModule, OwlNativeDateTimeModule, SharedModule, FormsModule,
    ReactiveFormsModule,
  ],
  providers: [
    WindowRef
  ]
})
export class DatahubModule { }
