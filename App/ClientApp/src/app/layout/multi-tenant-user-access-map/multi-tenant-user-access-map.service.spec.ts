import { TestBed } from '@angular/core/testing';

import { MultiTenantUserAccessMapService } from './multi-tenant-user-access-map.service';

describe('MultiTenantUserAccessMapService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: MultiTenantUserAccessMapService = TestBed.get(MultiTenantUserAccessMapService);
    expect(service).toBeTruthy();
  });
});
