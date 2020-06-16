import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { UsersRoutingModule } from './users-routing.module';
import { ListComponent } from './list/list.component';
import { ViewComponent } from './view/view.component';
import { UserAddEditComponent } from './add/add-edit.component';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';
import { MatTableModule } from '@angular/material/table';
import { SharedModule } from '../../shared/modules/shared.module';

@NgModule({
  declarations: [ListComponent, ViewComponent, UserAddEditComponent],
  imports: [
    CommonModule,
    UsersRoutingModule, MatPaginatorModule, MatSortModule, MatTableModule, FormsModule,
    ReactiveFormsModule, SharedModule
  ]
})
export class UsersModule { }
