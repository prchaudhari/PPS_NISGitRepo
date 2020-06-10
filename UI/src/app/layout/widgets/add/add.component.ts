import { Component, OnInit } from '@angular/core';
import { Location } from '@angular/common';
@Component({
  selector: 'app-add',
  templateUrl: './add.component.html',
  styleUrls: ['./add.component.scss']
})
export class AddComponent implements OnInit {

    //select widget type radio

    private selectedLink: string = "CustomerInformation";
    setWidgetType(e: string): void {
        this.selectedLink = e;
    }
    isSelected(name: string): boolean {
        if (!this.selectedLink) { 
            return false;
        }
        return (this.selectedLink === name); 
    }

    navigateToListPage() {
        this._location.back();
    }
    constructor(private _location: Location) { }

  ngOnInit() {
  }

}
