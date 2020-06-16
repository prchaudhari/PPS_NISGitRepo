import { TestBed } from '@angular/core/testing';
import { HttpClientService } from './httpClient.service';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { ConfigConstants } from 'src/app/shared/constants/configConstants';

describe('HttpClientService', () => {
  //   let service: HttpClientService;
  //   let httpMock: HttpTestingController;
  // beforeEach(() => TestBed.configureTestingModule({
  //   imports: [HttpClientTestingModule], 
  //   providers: [HttpClientService,
  //       ],
      
  // }));

  // it('should be created', () => {
  //   const service: HttpClientService = TestBed.get(HttpClientService);
  //   expect(service).toBeTruthy();
  // });


  let httpMock: HttpTestingController;
  let service: HttpClientService;

  beforeEach(() => {

    TestBed.configureTestingModule({
        imports: [ HttpClientTestingModule ],
        providers: [ HttpClientService ]
    });

    service = TestBed.get(HttpClientService);
    httpMock = TestBed.get(HttpTestingController);

  });

   it('should be created', () => {
    const service: HttpClientService = TestBed.get(HttpClientService);
    expect(service).toBeTruthy();
  });
});
