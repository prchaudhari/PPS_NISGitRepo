import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { TemplateRoutingModule } from './template-routing.module';
import { ListComponent } from './list/list.component';
import { MatSortModule } from '@angular/material/sort';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule } from '@angular/material/paginator';
import { AddComponent } from './add/add.component';
import { ViewComponent } from './view/view.component';
import { OwlDateTimeModule, OwlNativeDateTimeModule } from 'ng-pick-datetime';
import { SharedModule } from '../../shared/modules/shared.module';
import { DisableControlDirective } from '../../shared/directive/disabled-control.directive';

@NgModule({
  declarations: [ListComponent, AddComponent, ViewComponent, DisableControlDirective],
  imports: [
    CommonModule,
    TemplateRoutingModule, MatSortModule, MatTableModule, MatPaginatorModule, OwlDateTimeModule,
    OwlNativeDateTimeModule, FormsModule, ReactiveFormsModule, NgbModule, SharedModule
  ],
})
export class TemplateModule { }
