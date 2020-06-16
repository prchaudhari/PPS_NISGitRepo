import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { LoginComponent } from './login/login.component';
import { SharedModule } from './shared/modules/shared.module';
import { LayoutModule } from './layout/layout.module';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { DashboardDesignerModule } from './dashboard-designer/dashboard-designer.module';
import { MatSortModule } from '@angular/material/sort';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule } from '@angular/material/paginator';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ToastrModule } from 'ngx-toastr';
import { MsgBoxComponent } from './shared/modules/message/messagebox.component';
import { BootstrapModalModule } from 'ng2-bootstrap-modal';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { NgxUiLoaderModule } from 'ngx-ui-loader';
import { HttpIntercepter } from './core/interceptors/http-intercepter';
import { AuthGuard, UnAuthorisedUrlGuard } from './core/guard';
import { AuthenticationModule } from './authentication/authentication.module';

@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    MsgBoxComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    SharedModule,
    LayoutModule, BrowserAnimationsModule, DashboardDesignerModule, MatSortModule, MatTableModule, MatPaginatorModule,
    FormsModule,
    ReactiveFormsModule,
    BrowserAnimationsModule,
    BootstrapModalModule.forRoot({ container: document.body }),
    ToastrModule.forRoot(),
    NgxUiLoaderModule,
    AuthenticationModule
  ],
  providers: [
    {
      provide: HTTP_INTERCEPTORS,
      useClass: HttpIntercepter,
      multi: true
    },
    AuthGuard, UnAuthorisedUrlGuard
  ],
  bootstrap: [AppComponent],
  //Common component for alert message.
  entryComponents: [
    MsgBoxComponent
  ]
})
export class AppModule { }
