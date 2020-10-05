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
import { BootstrapModalModule } from '@tomblue/ng2-bootstrap-modal';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { NgxUiLoaderModule } from 'ngx-ui-loader';
import { HttpIntercepter } from './core/interceptors/http-intercepter';
import { AuthGuard, UnAuthorisedUrlGuard } from './core/guard';
import { AuthenticationModule } from './authentication/authentication.module';
import { ConfirmUserComponent } from './confirm-user/confirm-user.component';
import { BnNgIdleService } from 'bn-ng-idle';
import { DynamicGlobalVariable } from './shared/constants/constants';
import { NgMultiSelectDropDownModule } from 'ng-multiselect-dropdown';
import { PagePreviewComponent } from './shared/pagepreview/pagepreview.component';
import { SafeHtmlPip } from './shared/pagepreview/pagepreview.component';
import { MultipleMessageboxComponent } from './shared/modules/multiple-messagebox/multiple-messagebox.component';
import { SafeHtmlPipee } from './shared/modules/multiple-messagebox/multiple-messagebox.component';
import { ErrorLogsViewComponent } from './shared/error-logs-view/error-logs-view.component';
import { SelectTenantComponent } from './select-tenant/select-tenant.component';

@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    MsgBoxComponent,
    ConfirmUserComponent,
    PagePreviewComponent,
    MultipleMessageboxComponent,
    SafeHtmlPip, 
    SafeHtmlPipee, ErrorLogsViewComponent, SelectTenantComponent
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
    AuthenticationModule,
    NgMultiSelectDropDownModule.forRoot()
  ],
  providers: [
    {
      provide: HTTP_INTERCEPTORS,
      useClass: HttpIntercepter,
      multi: true
    },
    AuthGuard, UnAuthorisedUrlGuard
    , BnNgIdleService, DynamicGlobalVariable
  ],
  bootstrap: [AppComponent],
  //Common component for alert message.
  entryComponents: [
    MsgBoxComponent,
    PagePreviewComponent,
    MultipleMessageboxComponent,
    ErrorLogsViewComponent
  ]
})
export class AppModule { }
