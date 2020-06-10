import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ListAssetLibraryComponent } from './list-asset-library.component';

describe('ListAssetLibraryComponent', () => {
  let component: ListAssetLibraryComponent;
  let fixture: ComponentFixture<ListAssetLibraryComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ListAssetLibraryComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ListAssetLibraryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
