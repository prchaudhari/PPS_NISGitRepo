import { Component, OnInit } from '@angular/core';
import { Location } from '@angular/common';

@Component({
  selector: 'app-view',
  templateUrl: './view.component.html',
  styleUrls: ['./view.component.scss']
})
export class ViewComponent implements OnInit {

    public isCollapsedDetails: boolean = false;
    public isCollapsedPermissions: boolean = true;
    navigateToListPage() {
        this._location.back();
    }

    seq = [
        'Home V.2.0.0',
        'Saving Account V.2.0.0',
        'Current Account V.1.0.0',
    ];

    constructor(private _location: Location) { }

  ngOnInit() {
  }

}
