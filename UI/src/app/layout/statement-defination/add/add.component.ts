import { Component, OnInit } from '@angular/core';
import { Location } from '@angular/common';
import { CdkDragDrop, moveItemInArray, transferArrayItem } from '@angular/cdk/drag-drop';

@Component({
    selector: 'app-add',
    templateUrl: './add.component.html',
    styleUrls: ['./add.component.scss']
})
export class AddComponent implements OnInit {

    public isCollapsedDetails: boolean = false;
    public isCollapsedPermissions: boolean = true;
    navigateToListPage() {
        this._location.back();
    }

    seq = [
        'Home V.2.0.0',
        'Saving Account V.2.0.0',
        'Home V.1.0.0',
        'Current Account V.1.0.0',
        'Saving Account V.2.0.0',
    ];

    page = [
        'Home V.3.0.0',
        'Current Account V.2.0.0',
    ];

    drop(event: CdkDragDrop<string[]>) {
        if (event.previousContainer === event.container) {
            moveItemInArray(event.container.data, event.previousIndex, event.currentIndex);
        } else {
            transferArrayItem(event.previousContainer.data,
                event.container.data,
                event.previousIndex,
                event.currentIndex);
        }
    }

    constructor(private _location: Location) { }

    ngOnInit() {
    }

}
