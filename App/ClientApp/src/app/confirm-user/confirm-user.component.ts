import { Component, OnInit } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Router, ActivatedRoute } from '@angular/router';
import { HttpParams, HttpClient } from '@angular/common/http';
import { MessageDialogService } from 'src/app/shared/services/mesage-dialog.service';
import * as $ from 'jquery';
import { ConfigConstants } from 'src/app/shared/constants/configConstants'
import { Constants } from 'src/app/shared/constants/constants';
@Component({
  selector: 'app-confirm-user',
  templateUrl: './confirm-user.component.html',
})
export class ConfirmUserComponent implements OnInit {
  status: string;
  newPassword: string = '';
  confirmPassword: string = '';
  encryptedText: any;
  baseURL: string = environment.baseURL;
  constructor(private _http: HttpClient, private _router: Router, private _activatedRouter: ActivatedRoute,
    private _messageDialogService: MessageDialogService) { }

  ngOnInit() {
    //console.log(this._activatedRouter.snapshot.queryParams.token);
  };


  onSubmit(): boolean {
    if (this.newPassword != this.confirmPassword) {
      this.status = "Password does not match the confirm password";
      return false;
    } else {
      this.status = '';
    }
    this.encryptedText = this._activatedRouter.snapshot.queryParams.token;
    let params = new HttpParams();
    params = params.append('newPassword', this.newPassword);
    params = params.append('encryptedText', this.encryptedText);
    console.log(this.newPassword);
    console.log(this.encryptedText);
    let operationUrl = this.baseURL + 'User/Confirm';
    $('.overlay').show();
    this._http.get(operationUrl, { params: params })
      .subscribe(data => {
        this._messageDialogService.openDialogBox('Error', "Password generated successfully", Constants.msgBoxSuccess);

        this._router.navigate(['login']);
      },
        error => {
          this._messageDialogService.openDialogBox('Error', error.error.Message, Constants.msgBoxSuccess);
        },
        () => {
        }
      );

  }

  onCancel(): void {
    this._router.navigate(['login']);
  }
}

