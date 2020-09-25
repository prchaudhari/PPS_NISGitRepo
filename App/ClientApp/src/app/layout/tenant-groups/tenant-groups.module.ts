import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatSortModule } from '@angular/material/sort';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule } from '@angular/material/paginator';

import { TenantGroupsRoutingModule } from './tenant-groups-routing.module';
import { AddComponent } from './add/add.component';
import { ListComponent } from './list/list.component';

import { SharedModule } from '../../shared/modules/shared.module';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ViewComponent } from './view/view.component';

@NgModule({
  declarations: [AddComponent, ListComponent, ViewComponent],
  imports: [
    CommonModule,
    TenantGroupsRoutingModule, FormsModule, ReactiveFormsModule,MatSortModule, MatTableModule, 
    MatPaginatorModule, SharedModule, NgbModule
  ]
})
export class TenantGroupsModule { }
