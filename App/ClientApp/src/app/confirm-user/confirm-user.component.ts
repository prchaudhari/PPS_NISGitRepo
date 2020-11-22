import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { HttpParams, HttpClient } from '@angular/common/http';
import { MessageDialogService } from 'src/app/shared/services/mesage-dialog.service';
import { ConfigConstants } from 'src/app/shared/constants/configConstants'
import { Constants } from 'src/app/shared/constants/constants';
import * as $ from 'jquery';
import { NgxUiLoaderService } from 'ngx-ui-loader';
import { AppSettings } from '../appsettings';
@Component({
  selector: 'app-confirm-user',
  templateUrl: './confirm-user.component.html',
})

export class ConfirmUserComponent implements OnInit {
  status: string;
  newPassword: string = '';
  confirmPassword: string = '';
  encryptedText: any;
  baseURL: string = AppSettings.baseURL;
  public hideNewPwd = true;
  public hideConfirmPwd = true;
  public passwordRegex = new RegExp("^(?=.*?[A-Z])(?=.*?[a-z0-9])(?=.*?[#?!@$%^&*-]).{8,}$");

  constructor(private _http: HttpClient, 
    private _router: Router, 
    private _activatedRouter: ActivatedRoute,
    private spinner: NgxUiLoaderService,
    private _messageDialogService: MessageDialogService) 
    { }

  ngOnInit() {

    $(document).ready(function () {
      $(".fa-eye").mousedown(function () {
        $(this).parent().prev(".form-control").prop('type', 'text');
      }).mouseup(function () {
        $(this).parent().prev(".form-control").prop('type', 'password');
      }).mouseout(function () {
        $(this).parent().prev(".form-control").prop('type', 'password');
      });
    })

  };

  onSubmit(): boolean {
    if (this.newPassword == '' || this.confirmPassword == '') {
      this._messageDialogService.openDialogBox('Error', "Please enter Password.", Constants.msgBoxError);
      return false;
    }
    if (this.newPassword != '' && this.newPassword.length < 8) {
      this._messageDialogService.openDialogBox('Error', "New Password should not be less than 8 characters.", Constants.msgBoxError);
      return false;
    }
    if (this.confirmPassword != '' && this.confirmPassword.length < 8) {
      this._messageDialogService.openDialogBox('Error', "Confirm Password should not be less than 8 characters.", Constants.msgBoxError);
      return false;
    }
    if (this.newPassword != '' && !this.passwordRegex.test(this.newPassword)) {
      this._messageDialogService.openDialogBox('Error', "New Password should be combination of at least one of Capital letters, a number and a special character.", Constants.msgBoxError);
      return false;
    }
    if (this.confirmPassword != '' && !this.passwordRegex.test(this.confirmPassword)) {
      this._messageDialogService.openDialogBox('Error', "Confirm Password should be combination of at least one of Capital letters, a number and a special character.", Constants.msgBoxError);
      return false;
    }
    if (this.newPassword != this.confirmPassword) {
      this._messageDialogService.openDialogBox('Error', "Password does not match the confirm password.", Constants.msgBoxError);
      return false;
    } else {
      this.status = '';
    }

    this.encryptedText = this._activatedRouter.snapshot.queryParams.token;
    let params = new HttpParams();
    params = params.append('newPassword', this.newPassword);
    params = params.append('encryptedText', this.encryptedText);
    //console.log(this.newPassword);
    //console.log(this.encryptedText);
    let operationUrl = this.baseURL + 'User/Confirm';
    //$('.overlay').show();
    this.spinner.start();
    this._http.get(operationUrl, { params: params })
      .subscribe(data => {
        this.spinner.stop();
        this._messageDialogService.openDialogBox('Success', "Password generated successfully", Constants.msgBoxSuccess);
        localStorage.removeItem('currentUserName');
        localStorage.removeItem('currentUserTheme');
        localStorage.removeItem('userClaims');
        localStorage.removeItem('token');
        this._router.navigate(['login']);
      },
        error => {
          this._messageDialogService.openDialogBox('Error', error.error.Message, Constants.msgBoxError);
        },
        () => {
        }
      );
  }

  onCancel(): void {
    this._router.navigate(['login']);
  }
}
