import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AssetSettingsComponent } from './asset-settings.component';

describe('AssetSettingsComponent', () => {
  let component: AssetSettingsComponent;
  let fixture: ComponentFixture<AssetSettingsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AssetSettingsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AssetSettingsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
