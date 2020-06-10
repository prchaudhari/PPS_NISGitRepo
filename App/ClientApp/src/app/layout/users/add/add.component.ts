import { Component, OnInit } from '@angular/core';
import { Location } from '@angular/common';
@Component({
  selector: 'app-add',
  templateUrl: './add.component.html',
  styleUrls: ['./add.component.scss']
})
export class AddComponent implements OnInit {
  
    constructor(private _location: Location) { }

    navigateToListPage() {
        this._location.back();
    }

  ngOnInit() {
  }

}
