import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ListAssetLibraryComponent } from './list-asset-library/list-asset-library.component';
import { AddAssetLibraryComponent } from './add-asset-library/add-asset-library.component';
import { ViewAssetLibraryComponent } from './view-asset-library/view-asset-library.component';

const routes: Routes = [
    {
        path: '',
        children: [
            { path: '', component: ListAssetLibraryComponent },
            { path: 'List', component: ListAssetLibraryComponent },
            { path: 'Add', component: AddAssetLibraryComponent },
            { path: 'View', component: ViewAssetLibraryComponent }
        ]
    }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AssetLibrariesRoutingModule { }
