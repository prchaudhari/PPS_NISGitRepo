import { TestBed } from '@angular/core/testing';
import { ResourceService } from './resource.service';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { DialogService } from '@tomblue/ng2-bootstrap-modal';


describe('ResourceService', () => {
  beforeEach(() => TestBed.configureTestingModule({
    imports: [
        HttpClientTestingModule,
      ],
      providers: [
        DialogService,
      ]
  }));

  // it('should be created', () => {
  //   const service: ResourceService = TestBed.get(ResourceService);
  //   expect(service).toBeTruthy();
  // });
});
