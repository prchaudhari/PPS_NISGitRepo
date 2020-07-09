import { Component } from '@angular/core';
import { DialogComponent, DialogService } from '@tomblue/ng2-bootstrap-modal';


@Component({
    selector: 'pagepreview',
    template: `<div class="modal-dialog modal-dialog-centered modal-xl" role="document">
                <div class="modal-content" style="overflow:auto;max-height:700px;">
                  <div class="modal-body p-1 text-center">
                  <button type="button" class="close p-1" (click)="cancel()">
                    <span aria-hidden="true">&times;</span>
                  </button>
                  <div [innerHtml]="message">
                    </div>
                  </div>
                </div>
              </div>`
})
export class PagePreviewComponent extends DialogComponent<PagePreviewModel, boolean> implements PagePreviewModel, DialogOptions {
    message: string;
    backdropColor: string = "red";
    constructor(dialogService: DialogService) {
        super(dialogService);
    }

    cancel() {
      this.close();
  }
}

export interface PagePreviewModel {
    message: string;
}

interface DialogOptions {
    backdropColor?: string;
}
