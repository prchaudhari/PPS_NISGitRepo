import { Routes, RouterModule } from '@angular/router';
import { NgModule } from '@angular/core';
import { AuthGuard } from '../core/guard';
import { AuthenticationComponent } from './authentication.component';
import { ChangePasswordComponent } from '../layout/change-password/change-password.component';


 export const routes: Routes = [
    {
        path: '',
        component:AuthenticationComponent,
        children: [
            // { path: 'changePassword', component: ChangePasswordComponent, canActivate: [AuthGuard] },
            { path: 'changePassword', component: ChangePasswordComponent},
        ]

    }
]

@NgModule({
    exports: [RouterModule],
    imports: [RouterModule.forChild(routes)]
})

export class AuthenticationRoutingModule {

}
