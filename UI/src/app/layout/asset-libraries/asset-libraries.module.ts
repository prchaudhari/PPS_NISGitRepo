import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { AssetLibrariesRoutingModule } from './asset-libraries-routing.module';
import { ListAssetLibraryComponent } from './list-asset-library/list-asset-library.component';
import { AddAssetLibraryComponent } from './add-asset-library/add-asset-library.component';
import { ViewAssetLibraryComponent } from './view-asset-library/view-asset-library.component';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';
import { MatTableModule } from '@angular/material/table';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';

@NgModule({
  declarations: [ListAssetLibraryComponent, AddAssetLibraryComponent, ViewAssetLibraryComponent],
  imports: [
    CommonModule,
      AssetLibrariesRoutingModule, MatPaginatorModule, MatSortModule, MatTableModule, NgbModule
  ]
})
export class AssetLibrariesModule { }
