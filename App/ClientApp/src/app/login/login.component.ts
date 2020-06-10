import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {
    public isForgotPassword: boolean = false;
    public isLogin: boolean = true;
    checkLogin() {
        this.route.navigate(['/layout/dashboard']);
        }
    isForgotPassswordForm() {
        this.isForgotPassword = true;
        this.isLogin = false;
    }
    isLoginForm() {
        this.isForgotPassword = false;
        this.isLogin = true;
    }
    constructor(private route: Router) { }

  ngOnInit() {
  }

}
