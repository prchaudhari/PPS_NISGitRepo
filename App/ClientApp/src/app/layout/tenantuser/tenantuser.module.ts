import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { TenantuserRoutingModule } from './tenantuser-routing.module';
import { ListComponent } from './list/list.component';
import { ViewComponent } from './view/view.component';
import { AddEditComponent } from './add-edit/add-edit.component';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';
import { MatTableModule } from '@angular/material/table';
import { SharedModule } from '../../shared/modules/shared.module';

@NgModule({
  declarations: [ListComponent, ViewComponent, AddEditComponent],
  imports: [
    CommonModule,
    TenantuserRoutingModule, MatPaginatorModule, MatSortModule, MatTableModule, FormsModule,
    ReactiveFormsModule, SharedModule
  ]
})
export class TenantuserModule { }
