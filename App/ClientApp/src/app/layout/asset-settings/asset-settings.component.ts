import { FormBuilder, FormGroup } from '@angular/forms';
import { Component, OnInit } from '@angular/core';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { Location } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { IDropdownSettings } from 'ng-multiselect-dropdown';
@Component({
  selector: 'app-asset-settings',
  templateUrl: './asset-settings.component.html',
  styleUrls: ['./asset-settings.component.scss']
})
export class AssetSettingsComponent implements OnInit {
  public isUploadAssets: boolean = false;
  public isCollapsedImage: boolean = true;
  public isCollapsedVideo: boolean = true;

  imageForm: FormGroup;
  constructor(private _location: Location, private fb: FormBuilder) { }
  imagedropdownList = [];
  imageselectedItems = [];
  imagedropdownSettings: IDropdownSettings = {};

  videodropdownList = [];
  videoselectedItems = [];
  videodropdownSettings: IDropdownSettings = {};
  ngOnInit() {
    this.imagedropdownList = [
     
      { id: 1, item_text: 'png' },
      { id: 2, item_text: 'jpeg' }
     
    ];
  
    this.imagedropdownSettings   = {
      singleSelection: false,
      idField: 'id',
      textField: 'item_text',
      selectAllText: 'Select All',
      unSelectAllText: 'Un Select All',
      itemsShowLimit: 3,
      allowSearchFilter: false
    };
    this.videodropdownList = [
      { id: 1, item_text: 'mp4' },
    ];

    this.videodropdownSettings = {
      singleSelection: false,
      idField: 'id',
      textField: 'item_text',
      selectAllText: 'Select All',
      unSelectAllText: 'Un Select All',
      itemsShowLimit: 3,
      allowSearchFilter: false
    };

    this.imageForm = this.fb.group({
      
    })
  }
  onItemSelect(item: any) {
    console.log(item);
  }
  onSelectAll(items: any) {
    console.log(items);
  }

  navigateToListPage() {
    this._location.back();
  }


}
