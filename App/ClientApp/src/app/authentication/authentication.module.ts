import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { OwlDateTimeModule, OwlNativeDateTimeModule } from 'ng-pick-datetime';
import { GridsterModule } from 'angular-gridster2';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { OwlModule } from 'ngx-owl-carousel';
import { SharedModule } from '../shared/modules/shared.module';
import { AuthenticationComponent } from './authentication.component';
import { AuthenticationRoutingModule } from './authentication-routing.module';


@NgModule({
  declarations: [
   AuthenticationComponent,
  ],
  imports: [
    CommonModule,
    RouterModule,
    NgbModule,
    FormsModule,
    ReactiveFormsModule,
    OwlDateTimeModule,
    OwlNativeDateTimeModule,
    GridsterModule,
    SharedModule,
    HttpClientModule,
    GridsterModule,
    OwlModule,
    AuthenticationRoutingModule
  ],
  exports: [
  ],
})
export class AuthenticationModule { }

