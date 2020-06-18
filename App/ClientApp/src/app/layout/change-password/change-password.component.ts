import { Component, OnInit } from '@angular/core';
import { Location } from '@angular/common';
import { HttpParams, HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { NgxUiLoaderService } from 'ngx-ui-loader';
import { Router, NavigationEnd } from '@angular/router';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';

@Component({
  selector: 'app-change-password',
  templateUrl: './change-password.component.html',
  styleUrls: ['./change-password.component.scss']
})
export class ChangePasswordComponent implements OnInit {
  statuscurrentPassword: string;
  statusnewPassword: string;
  statusconfirmPassword: string;
  statuscurrentPasswordLength: string;
  statusnewPasswordLength: string;
  statusconfirmPasswordLength: string;
  result: boolean = true;
  baseURL: string = environment.baseURL;
  currentPassword: string;
  newPassword: string;
  confirmPassword: string;
  userFormGroup: FormGroup;
  constructor(private _location: Location,
    private spinner: NgxUiLoaderService,
    private _http: HttpClient,
    private formBuilder: FormBuilder,
   private router: Router) { }
 

  backClicked() {
    this._location.back();
  }
  Submit(): boolean {
 //   userFormGroup.value.
    if (this.userFormGroup.value.currentPassword == undefined || this.userFormGroup.value.currentPassword == null) {
      this.statuscurrentPassword = "Please enter Current Password";
      return this.result = false;
    }
    if (this.userFormGroup.value.currentPassword != undefined && this.userFormGroup.value.currentPassword != null) {
      if (this.userFormGroup.value.currentPassword.length < 8) {
        this.statuscurrentPasswordLength = "Password length should not be less than 8 ";
        return this.result = false;
      }
    }
    this.statuscurrentPassword = '';
    if (this.userFormGroup.value.newPassword == undefined || this.userFormGroup.value.newPassword == null) {
      this.statusnewPassword = "Please enter New Password";
      return this.result = false;
    }
    if (this.userFormGroup.value.newPassword != undefined && this.userFormGroup.value.newPassword  != null) {
      if (this.userFormGroup.value.newPassword.length<8) {
        this.statusnewPasswordLength = "Password length should not be less than 8 ";
        return this.result = false;
      }
    }
    this.statusnewPassword = '';
    if (this.userFormGroup.value.newPassword1 == undefined || this.userFormGroup.value.newPassword1  == null) {
      this.statusconfirmPassword = "Please enter Confirm Password";
      return this.result = false;
    }
    if (this.userFormGroup.value.newPassword1  != undefined && this.userFormGroup.value.newPassword1  != null) {
      if (this.userFormGroup.value.newPassword1 < 8) {
        this.statusconfirmPasswordLength = "Password length should not be less than 8 ";
        return this.result = false;
      }
    }
    this.statusconfirmPassword = '';
    if (this.userFormGroup.value.newPassword != this.userFormGroup.value.newPassword1 ) {
      this.statusconfirmPassword = "Password not matched with new password";
      return this.result = false;
    }
    //console.log(this.newPassword);
    let params = new HttpParams();
    params = params.append('userEmail', localStorage.getItem('UserEmail'));
    params = params.append('oldPassword', this.userFormGroup.value.currentPassword);
    params = params.append('newPassword', this.userFormGroup.value.newPassword );

    let operationUrl = this.baseURL + 'User/ChangePassword';
    this.spinner.start();
    this._http.get(operationUrl, { params: params })
      .subscribe(data => {
        this.spinner.stop();

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

  ngOnInit(): void {
    this.userFormGroup = this.formBuilder.group({
      currentPassword: [null, Validators.compose([Validators.required, Validators.minLength(8)])],
      newPassword: [null, Validators.compose([Validators.required, Validators.minLength(8)])],
      newPassword1: [null, Validators.compose([Validators.required, Validators.minLength(8)])],

    })
  }

}
