import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ListComponent } from './list/list.component';
import { ViewComponent } from './view/view.component';
import { AddeditComponent } from './addedit/addedit.component';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';
import { MatTableModule } from '@angular/material/table';
import { SharedModule } from '../../shared/modules/shared.module';
import { RenderEngineRoutingModule } from './render-engine-routing-module';

@NgModule({
  declarations: [AddeditComponent, ViewComponent, ListComponent],
  imports: [
    CommonModule, RenderEngineRoutingModule, MatPaginatorModule, MatSortModule, MatTableModule, FormsModule,
      ReactiveFormsModule, NgbModule, SharedModule
  ]
})

export class RenderEngineModule { }
