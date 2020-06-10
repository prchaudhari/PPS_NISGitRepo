import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AddAssetLibraryComponent } from './add-asset-library.component';

describe('AddAssetLibraryComponent', () => {
  let component: AddAssetLibraryComponent;
  let fixture: ComponentFixture<AddAssetLibraryComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AddAssetLibraryComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AddAssetLibraryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
