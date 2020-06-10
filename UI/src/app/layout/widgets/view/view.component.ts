import { Component, OnInit } from '@angular/core';
import { Location } from '@angular/common';
@Component({
  selector: 'app-view',
  templateUrl: './view.component.html',
  styleUrls: ['./view.component.scss']
})
export class ViewComponent implements OnInit {

    navigateToListPage() {
        this._location.back();
    }
    constructor(private _location: Location) { }

  ngOnInit() {
  }

}
