import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { UsersRoutingModule } from './users-routing.module';
import { ListComponent } from './list/list.component';
import { ViewComponent } from './view/view.component';
import { AddComponent } from './add/add.component';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';
import { MatTableModule } from '@angular/material/table';
import { SharedModule } from '../../shared/modules/shared.module';

@NgModule({
  declarations: [ListComponent, ViewComponent, AddComponent],
  imports: [
    CommonModule,
    SharedModule,
    UsersRoutingModule, MatPaginatorModule, MatSortModule, MatTableModule
  ]
})
export class UsersModule { }
