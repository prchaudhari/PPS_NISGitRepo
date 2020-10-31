import { Injectable } from '@angular/core';
import { DialogService } from '@tomblue/ng2-bootstrap-modal';
import { MsgBoxComponent } from '../modules/message/messagebox.component';
import { PagePreviewComponent } from '../pagepreview/pagepreview.component'
import { WidgetPreviewComponent } from '../widgetpreview/widgetpreview.component'
import { Observable } from 'rxjs';
import { MultipleMessageboxComponent } from '../modules/multiple-messagebox/multiple-messagebox.component';
import { ErrorLogsViewComponent } from '../error-logs-view/error-logs-view.component'
import { PageDesignPreviewComponent } from '../../dashboard-designer/page-design-preview/page-design-preview.component';

@Injectable({
    providedIn: 'root'
})
export class MessageDialogService {

  constructor(private _dialogService: DialogService) { }

  openDialogBox(title, message, msgType): Observable<any> {
    return this._dialogService.addDialog(MsgBoxComponent, {
      title: title,
      message: message,
      msgType: msgType,
      actionButtonText: "Ok",
      showCancelButtun: false,
      cancelButtonText: "Cancel"
    });
  }
  
  openConfirmationDialogBox(title, message, msgType): Observable<any> {
    return this._dialogService.addDialog(MsgBoxComponent, {
      title: title,
      message: message,
      msgType: msgType,
      actionButtonText: "Ok",
      showCancelButtun: true,
      cancelButtonText: "Cancel"
    });
  }

}
