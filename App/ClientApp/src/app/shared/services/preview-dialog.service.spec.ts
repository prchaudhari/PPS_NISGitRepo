import { TestBed } from '@angular/core/testing';

import { PreviewDialogService } from './preview-dialog.service';

describe('PreviewDialogService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: PreviewDialogService = TestBed.get(PreviewDialogService);
    expect(service).toBeTruthy();
  });
});
