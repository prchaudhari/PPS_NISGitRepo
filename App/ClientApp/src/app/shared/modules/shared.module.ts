import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HeaderComponent } from './header/header.component';
import { SidebarComponent } from './sidebar/sidebar.component';
import { RouterModule } from '@angular/router';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';
import { MatTableModule } from '@angular/material/table';

@NgModule({
    declarations: [
        HeaderComponent,
        SidebarComponent
    ],
  imports: [
      CommonModule,
      RouterModule,
      MatPaginatorModule,
      MatSortModule,
      MatTableModule
    ],
    exports: [
        HeaderComponent,
        SidebarComponent,
        RouterModule,
        MatPaginatorModule,
        MatSortModule,
        MatTableModule
    ]
})
export class SharedModule { }
