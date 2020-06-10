import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ViewAssetLibraryComponent } from './view-asset-library.component';

describe('ViewAssetLibraryComponent', () => {
  let component: ViewAssetLibraryComponent;
  let fixture: ComponentFixture<ViewAssetLibraryComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ViewAssetLibraryComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ViewAssetLibraryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
