import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { StatementDefinationRoutingModule } from './statement-defination-routing.module';
import { ListComponent } from './list/list.component';
import { AddComponent } from './add/add.component';
import { ViewComponent } from './view/view.component';
import { MatSortModule } from '@angular/material/sort';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule } from '@angular/material/paginator';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { DragDropModule } from '@angular/cdk/drag-drop';
import { OwlDateTimeModule, OwlNativeDateTimeModule } from 'ng-pick-datetime';
@NgModule({
  declarations: [ListComponent, AddComponent, ViewComponent],
  imports: [
    CommonModule,
      StatementDefinationRoutingModule,
      MatTableModule,
      MatSortModule,
      MatPaginatorModule,
      NgbModule, DragDropModule, OwlDateTimeModule, OwlNativeDateTimeModule
  ]
})
export class StatementDefinationModule { }
