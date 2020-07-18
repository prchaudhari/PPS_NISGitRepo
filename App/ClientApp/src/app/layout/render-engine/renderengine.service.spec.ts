import { TestBed } from '@angular/core/testing';

import { RenderengineService } from './renderengine.service';

describe('RenderengineService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: RenderengineService = TestBed.get(RenderengineService);
    expect(service).toBeTruthy();
  });
});
