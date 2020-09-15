import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ThemeConfigurationComponent } from './theme-configuration.component';

describe('ThemeConfigurationComponent', () => {
  let component: ThemeConfigurationComponent;
  let fixture: ComponentFixture<ThemeConfigurationComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ThemeConfigurationComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ThemeConfigurationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
