import { Component, OnInit } from '@angular/core';
import { Location } from '@angular/common';
import { Router } from '@angular/router';
@Component({
  selector: 'app-add',
  templateUrl: './add.component.html',
  styleUrls: ['./add.component.scss']
})
export class AddComponent implements OnInit {
    navigateToListPage() {
        this._location.back();
    }
    navigationTodashboardDesigner() {
        this.route.navigate(['../dashboardDesigner']);
    }
    constructor(private _location: Location, private route: Router) { }

  ngOnInit() {
  }

}
