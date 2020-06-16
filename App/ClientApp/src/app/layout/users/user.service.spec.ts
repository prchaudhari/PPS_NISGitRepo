//import { TestBed, ComponentFixture, inject, async } from '@angular/core/testing';

//import { UserService } from './user.service';
//import { Injector } from '@angular/core';
//import { HttpTestingController, HttpClientTestingModule } from '@angular/common/http/testing';
//import { RouterTestingModule } from '@angular/router/testing';
//import { DialogService } from 'ng2-bootstrap-modal';
//import { HttpEvent, HttpEventType } from '@angular/common/http';
//import { URLConfiguration } from 'src/app/shared/urlConfiguration/urlconfiguration';

//describe('UserService', () => {
   
//    let service : UserService
//    let httpMock: HttpTestingController;
//    let injector: Injector;
//  beforeEach(() => {
//    injector = TestBed.configureTestingModule({
//        imports:
//            [
//            HttpClientTestingModule,
//            RouterTestingModule],
//        providers:
//            [UserService,
//             DialogService]
//    });

//  // inject the service
//  //HttpTestingController is used to mock the request--
//  service = TestBed.get(UserService);
//  httpMock = TestBed.get(HttpTestingController);
//});
//afterEach(() => {
//    httpMock.verify();
//  });

//  //Test case to ckeck for the craetion of the service--
//  it('should be created', () => {
//    const service: UserService = TestBed.get(UserService);
//    expect(service).toBeTruthy();
//  });

//  //Test case written to check the "GET Role" api--
//  it(
//    'should get users',
//    inject(
//      [HttpTestingController, UserService],
//      (httpMock: HttpTestingController, userService: UserService) => {
//        const mockUsers = [
//          { name: 'Bob', decription: 'www.yessss.com' },
//          { name: 'Juliette', decription: 'nope.com' }
//        ];

////getrole api is called and checked whether the response is equal to mock data--
//        userService.getUserById('').subscribe((event: HttpEvent<any>) => {
//          switch (event.type) {
//            case HttpEventType.Response:
//              expect(event.body).toEqual(mockUsers);
              
//          }
//        });

//        //exceptOne is call to Expect that a single request has been made which matches the given URL, and return its mock.
//        const mockReq = httpMock.expectOne("http://faktorywize-api-app.azurewebsites.net/" + "User/Get");

//        expect(mockReq.cancelled).toBeFalsy();
//        expect(mockReq.request.responseType).toEqual('json');
//        mockReq.flush(mockUsers);

//        httpMock.verify();
//      }
//    )
//  );

//  //Test case written to check the "GET Role" api--
//  it(
//    'should get roles',
//    inject(
//      [HttpTestingController, UserService],
//      (httpMock: HttpTestingController, dataService: UserService) => {
//        const mockUsers = [
//          { name: 'Bob', decription: 'www.yessss.com' },
//          { name: 'Juliette', decription: 'nope.com' }
//        ];

////getrole api is called and checked whether the response is equal to mock data--
//        dataService.getRole('').subscribe((event: HttpEvent<any>) => {
//          switch (event.type) {
//            case HttpEventType.Response:
//              expect(event.body).toEqual(mockUsers);
              
//          }
//        });

//        //exceptOne is call to Expect that a single request has been made which matches the given URL, and return its mock.
//        const mockReq = httpMock.expectOne("http://faktorywize-api-app.azurewebsites.net/" + "Role/Get");

//        expect(mockReq.cancelled).toBeFalsy();
//        expect(mockReq.request.responseType).toEqual('json');
//        mockReq.flush(mockUsers);

//        httpMock.verify();
//      }
//    )
//  );

//    //Test case written to check the "GET Role" api--
//    it(
//        'should get roles',
//        inject(
//          [HttpTestingController, UserService],
//          (httpMock: HttpTestingController, dataService: UserService) => {
//            const mockCountryDetails = [
//              { CountryIdentifier: '12344556', 
//                CountryDialingCode: '91',
//                CountryName:'India' ,
//                CountryCode:'+91'
//            },
//            ];
    
//    //getrole api is called and checked whether the response is equal to mock data--
//            dataService.getCountryCode('').subscribe((event: HttpEvent<any>) => {
//              switch (event.type) {
//                case HttpEventType.Response:
//                  expect(event.body).toEqual(mockCountryDetails);
                  
//              }
//            });
    
//            //exceptOne is call to Expect that a single request has been made which matches the given URL, and return its mock.
//            const mockReq = httpMock.expectOne("http://faktorywize-api-app.azurewebsites.net/" + "Country/Get");
    
//            expect(mockReq.cancelled).toBeFalsy();
//            expect(mockReq.request.responseType).toEqual('json');
//            mockReq.flush(mockCountryDetails);
    
//            httpMock.verify();
//          }
//        )
//      );


////Test cases written to test Delete API--
//it(
//    'should delete roles',
//    inject(
//      [HttpTestingController, UserService],
//      (httpMock: HttpTestingController, dataService: UserService) => {
//        const mockUser = [
//          { name: 'User'},
//        ];

//        dataService.deleteUser('').subscribe((event: HttpEvent<any>) => {
//          switch (event.type) {
//            case HttpEventType.Response:
//              expect(event.body).toEqual(mockUser);
//          }
//        });

// //exceptOne is call to Expect that a single request has been made which matches the given URL, and return its mock.
//        const mockReq = httpMock.expectOne("http://faktorywize-api-app.azurewebsites.net/" + "User/Delete");

//        expect(mockReq.cancelled).toBeFalsy();
//        expect(mockReq.request.responseType).toEqual('json');
//        mockReq.flush(mockUser);

//        httpMock.verify();
//      }
//    )
//  );

//  //Test cases written to test POST API--
//  it('should save user data', async( async() => {
//    const service: UserService = TestBed.get(UserService);
//    let requestUrl = URLConfiguration.userAddUrl;
//    // arrange
//    let mockUsers = [{
//        FirstName: "test",
//        LastName: "test",
//        EmailAddress:"vivek.singh@jyothy.com",
//        RolePrivileges: ""
//            }];

//    service.saveUser(mockUsers,requestUrl).subscribe(data=>{
//      var data = data
//       expect(data).toBeTruthy()
//    })
//     //exceptOne is call to Expect that a single request has been made which matches the given URL, and return its mock.
//    const mockReq = httpMock.expectOne("http://faktorywize-api-app.azurewebsites.net/" + "User/Add");
//    expect(mockReq.cancelled).toBeFalsy();
//    expect(mockReq.request.responseType).toEqual('json');
//    mockReq.flush(mockUsers);

//    httpMock.verify();
  
//  } ));


//});
