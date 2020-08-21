import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HeaderComponent } from './header/header.component';
import { SidebarComponent } from './sidebar/sidebar.component';
import { RouterModule } from '@angular/router';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';
import { MatTableModule } from '@angular/material/table';
import { PaginatePipe } from './pagination/paginate.pipe';
import { SharedPipesModule } from '../pipes/shared-pipes.module';
import { PaginationModule } from './pagination/pagination.module';

@NgModule({
  declarations: [
    HeaderComponent,
    SidebarComponent,
  ],
  imports: [
    CommonModule,
    RouterModule,
    MatPaginatorModule,
    MatSortModule,
    MatTableModule,
    SharedPipesModule,
    PaginationModule

  ],
  exports: [
    HeaderComponent,
    SidebarComponent,
    RouterModule,
    MatPaginatorModule,
    MatSortModule,
    MatTableModule,
    SharedPipesModule,
    PaginationModule
  ]
})
export class SharedModule { }
