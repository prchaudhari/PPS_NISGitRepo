import { Component } from '@angular/core';
import { DialogComponent, DialogService } from '@tomblue/ng2-bootstrap-modal';
import * as $ from 'jquery';

@Component({
    selector: 'pagepreview',
    template: `<div class="modal-dialog modal-dialog-centered modal-xl" role="document">
                <div class="modal-content" style="overflow:auto;max-height:610px;">
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

    ngOnInit() {
      $(document).ready(function () {
        $('.nav-link').click(function (e) { 
          $('.tabDivClass').hide();
          $('.nav-link').removeClass('active');
          let newClasses = 'active ' + $(e.currentTarget).attr('class');
          $(e.currentTarget).attr('class', newClasses);
          let classlist = $(e.currentTarget).attr('class').split(' ');
          let className = classlist[classlist.length - 1];
          if($('.'+className).hasClass('d-none')) {
            $('.'+className).removeClass('d-none');
          }
          $('.'+className).show();
        });
      });
    }
}

export interface PagePreviewModel {
    message: string;
}

interface DialogOptions {
    backdropColor?: string;
}
