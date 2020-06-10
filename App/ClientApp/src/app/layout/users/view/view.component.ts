import { Component, OnInit } from '@angular/core';
import { Location } from '@angular/common';
@Component({
  selector: 'app-view',
  templateUrl: './view.component.html',
  styleUrls: ['./view.component.scss']
})
export class ViewComponent implements OnInit {

    constructor(private _location: Location) { }

    navigateToListPage() {
        this._location.back();
    }

  ngOnInit() {
  }

}
