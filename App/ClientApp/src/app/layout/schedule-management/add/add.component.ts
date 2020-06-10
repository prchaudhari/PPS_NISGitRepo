import { Component, OnInit } from '@angular/core';
import { Location } from '@angular/common';
@Component({
  selector: 'app-add',
  templateUrl: './add.component.html',
  styleUrls: ['./add.component.scss']
})
export class AddComponent implements OnInit {

    navigateToListPage() {
        this._location.back();
    }
    constructor(private _location: Location) { }

  ngOnInit() {
  }

}
