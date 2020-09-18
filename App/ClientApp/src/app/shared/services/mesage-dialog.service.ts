import { Injectable } from '@angular/core';
import { DialogService } from '@tomblue/ng2-bootstrap-modal';
import { MsgBoxComponent } from 'src/app/shared/modules/message/messagebox.component';
import { PagePreviewComponent } from 'src/app/shared/pagepreview/pagepreview.component'
import { Observable } from 'rxjs';
import { MultipleMessageboxComponent } from '../modules/multiple-messagebox/multiple-messagebox.component';
import { ErrorLogsViewComponent } from '../error-logs-view/error-logs-view.component'

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

  openPreviewDialogBox(message): Observable<any> {
    return this._dialogService.addDialog(PagePreviewComponent, {
      htmlContent: message,
    });
  }

  openErrorLogDialogBox(scheduleLogId, scheduleName): Observable<any> {
    return this._dialogService.addDialog(ErrorLogsViewComponent, {
      scheduleLogId: scheduleLogId,
      scheduleName: scheduleName,
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

  openMulipleMessageDialogBox(title, message, msgType): Observable<any> {
    return this._dialogService.addDialog(MultipleMessageboxComponent, {
      title: title,
      message: message,
      msgType: msgType,
      actionButtonText: "Ok",
      showCancelButtun: false,
      cancelButtonText: "Cancel"
    });
  }

}
