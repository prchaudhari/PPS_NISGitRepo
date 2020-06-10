import { Component, OnInit } from '@angular/core';
import { Location } from '@angular/common';
@Component({
  selector: 'app-change-password',
  templateUrl: './change-password.component.html',
  styleUrls: ['./change-password.component.scss']
})
export class ChangePasswordComponent implements OnInit {

    constructor(private _location: Location) { }

    backClicked() {
        this._location.back();
    }


  ngOnInit(): void {
  }

}
