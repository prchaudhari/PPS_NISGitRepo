import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { StatementDefinationRoutingModule } from './statement-defination-routing.module';
import { ListComponent } from './list/list.component';
import { MatSortModule } from '@angular/material/sort';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule } from '@angular/material/paginator';
import { AddComponent } from './add/add.component';
import { ViewComponent } from './view/view.component';
import { OwlDateTimeModule, OwlNativeDateTimeModule } from 'ng-pick-datetime';
import { SharedModule } from '../../shared/modules/shared.module';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { DragDropModule } from '@angular/cdk/drag-drop';
@NgModule({
  declarations: [ListComponent, AddComponent, ViewComponent],
  imports: [
    CommonModule,
    StatementDefinationRoutingModule, MatSortModule, MatTableModule, MatPaginatorModule, OwlDateTimeModule,
    OwlNativeDateTimeModule, FormsModule, ReactiveFormsModule, NgbModule, SharedModule, DragDropModule
  ]
})
export class StatementDefinationModule { }
