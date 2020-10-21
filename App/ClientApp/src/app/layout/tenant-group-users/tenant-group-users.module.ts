import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { TenantGroupUsersRoutingModule } from './tenant-group-users-routing.module';
import { AddEditComponent } from './add-edit/add-edit.component';
import { ListComponent } from './list/list.component';
import { ViewComponent } from './view/view.component';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';
import { MatTableModule } from '@angular/material/table';
import { SharedModule } from '../../shared/modules/shared.module';

@NgModule({
  declarations: [AddEditComponent, ListComponent, ViewComponent],
  imports: [
    CommonModule,
    TenantGroupUsersRoutingModule, 
    MatPaginatorModule, MatSortModule, MatTableModule, FormsModule,
    ReactiveFormsModule, SharedModule
  ]
})
export class TenantGroupUsersModule { }
