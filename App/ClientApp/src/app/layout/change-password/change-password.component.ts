import { Component, OnInit } from '@angular/core';
import { Location } from '@angular/common';
import { HttpParams, HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { NgxUiLoaderService } from 'ngx-ui-loader';
import { Router, NavigationEnd } from '@angular/router';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';
import * as $ from 'jquery';

@Component({
  selector: 'app-change-password',
  templateUrl: './change-password.component.html',
  styleUrls: ['./change-password.component.scss']
})
export class ChangePasswordComponent implements OnInit {
  changePasswordForm: FormGroup;
  baseURL: string = environment.baseURL;
  public passwordRegex = "^(?=.*?[A-Z])(?=.*?[a-z0-9])(?=.*?[#?!@$%^&*-]).{8,}$";

  error_messages = {

    'oldPassword': [
      { type: 'required', message: 'old password is required.' },
      { type: 'minlength', message: 'Password should not be less than 8 characters.' },
      { type: 'maxlength', message: 'Password should not be grater than 30 characters.' }
    ],
    'newPassword': [
      { type: 'required', message: 'new password is required.' },
      { type: 'minlength', message: 'Password should not be less than 8 characters.' },
      { type: 'maxlength', message: 'Password should not be grater than 30 characters.' },
      {type: 'pattern', message : 'Password should be combination of at least one of Capital letters, a number and a special character.'}
    ],
    'confirmpassword': [
      { type: 'required', message: 'confirm password is required.' },
      { type: 'minlength', message: 'Password should not be less than 8 characters.' },
      { type: 'maxlength', message: 'Password should not be grater than 30 characters.' },
      {type: 'pattern', message : 'Password should be combination of at least one of Capital letters, a number and a special character.'}
    ],
  }

  constructor(private _location: Location,
    private spinner: NgxUiLoaderService,
    private _http: HttpClient,
    private formBuilder: FormBuilder,
    private router: Router) 
    {
    this.changePasswordForm = this.formBuilder.group({
      oldPassword: new FormControl('', Validators.compose([
        Validators.required,
        Validators.minLength(8),
        Validators.maxLength(30)
      ])),
      newPassword: new FormControl('', Validators.compose([
        Validators.required,
        Validators.minLength(8),
        Validators.maxLength(30),
        Validators.pattern(this.passwordRegex)
      ])),
      confirmpassword: new FormControl('', Validators.compose([
        Validators.required,
        Validators.minLength(8),
        Validators.maxLength(30),
        Validators.pattern(this.passwordRegex)
      ])),
    }, { 
      validators: [this.password.bind(this), this.oldPassword.bind(this) ]
    }
    );
  }

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
  }

  password(formGroup: FormGroup) {
    const { value: newPassword } = formGroup.get('newPassword');
    const { value: confirmPassword } = formGroup.get('confirmpassword');
    return newPassword === confirmPassword ? null :  { confirmPwdErr : "New Password and Confirm Password must be match."};
  }

  oldPassword(formGroup: FormGroup) {
    const { value: oldPassword } = formGroup.get('oldPassword');
    const { value: newPassword } = formGroup.get('newPassword');
    return oldPassword != '' && newPassword === oldPassword ?  
    { oldPwdErr : "Current Password and New Password must not be same."} : null;
  }

  Submit()  {

    let params = new HttpParams();
    params = params.append('userEmail', localStorage.getItem('UserEmail'));
    params = params.append('oldPassword', this.changePasswordForm.value.oldPassword);
    params = params.append('newPassword', this.changePasswordForm.value.newPassword );

    let operationUrl = this.baseURL + 'User/ChangePassword';
    this.spinner.start();
    this._http.get(operationUrl, { params: params })
      .subscribe(data => {
        this.spinner.stop();
        localStorage.removeItem('currentUserName');
        localStorage.removeItem('currentUserTheme');
        localStorage.removeItem('userClaims');
        localStorage.removeItem('token');
        this.router.navigate(['login']);
      },
        error => {
          this.spinner.stop();
        },
        () => {
          this.spinner.stop();
        }
      );
  }

  backClicked() {
    this._location.back();
  }

}
