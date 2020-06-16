// import { TestBed, async, inject, ComponentFixture } from '@angular/core/testing';
// import { RouterTestingModule } from '@angular/router/testing';

// import { AuthGuard, UnAuthorisedUrlGuard } from './auth.guard';
// import { ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
// import { HttpClientModule } from '@angular/common/http';
// // import { LoginService } from 'src/app/login/login.service';
// import { FilterPipe } from 'src/app/shared/pipes/filter.pipe';
// import { LoginService } from 'src/app/authentication/login/login.service';

// describe('AuthGuard', () => {
//   let authGuard: AuthGuard;
//   let authService 
//   let route :ActivatedRouteSnapshot;
//   let state: RouterStateSnapshot
//   let router;
//   let filterPipe;
//   let fixture: ComponentFixture<UnAuthorisedUrlGuard>;

//   beforeEach(() => {
//     TestBed.configureTestingModule({
//       imports: [ RouterTestingModule,
//         HttpClientModule],
//       providers: [AuthGuard,UnAuthorisedUrlGuard,
//         LoginService,FilterPipe]
//     });
//   });

//   it('should ...', inject([AuthGuard], (guard: AuthGuard) => {
//     expect(guard).toBeTruthy();
//   }));

//   it('checks if a user is valid',
//     // inject your guard service AND Router
//     async(inject([AuthGuard, Router], (auth, router) => {
//       // add a spy
//       spyOn(router, 'navigate');
//       expect(auth.canActivate()).toBeFalsy();
//       expect(router.navigate).toHaveBeenCalled();
//     })
//   ));

//   it('checks if a user is valid for the child module routing',
//   // inject your guard service AND Router
//   async(inject([AuthGuard, Router], (auth, router) => {
//     // add a spy
//     spyOn(router, 'navigate');
//     expect(auth.canLoad()).toBeFalsy();
//     expect(router.navigate).toHaveBeenCalled();
//   })
// ));

// it('checks if a user is valid but not has the role previleges',
// // inject your guard service AND Router
// async(inject([UnAuthorisedUrlGuard, Router], (auth, router) => {
//   // add a spy
//   spyOn(router, 'navigate');
//   expect(auth.canActivate(new ActivatedRouteSnapshot)).toBeFalsy();
//   fixture.detectChanges()
//   expect(router.navigate).toHaveBeenCalled();
// })
// ));

// });
