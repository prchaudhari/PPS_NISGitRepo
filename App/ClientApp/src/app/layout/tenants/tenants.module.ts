import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatSortModule } from '@angular/material/sort';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule } from '@angular/material/paginator';
import { TenantsRoutingModule } from './tenants-routing.module';
import { ListComponent } from './list/list.component';
import { ViewComponent } from './view/view.component';
import { AddComponent } from './add/add.component';
import { SharedModule } from '../../shared/modules/shared.module';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { OwlDateTimeModule, OwlNativeDateTimeModule } from 'ng-pick-datetime';
@NgModule({
  declarations: [ListComponent, ViewComponent, AddComponent],
  imports: [
    CommonModule,
    TenantsRoutingModule, OwlDateTimeModule, OwlNativeDateTimeModule, FormsModule, ReactiveFormsModule, MatSortModule, MatTableModule, MatPaginatorModule, SharedModule, NgbModule
  ]
})
export class TenantsModule { }
