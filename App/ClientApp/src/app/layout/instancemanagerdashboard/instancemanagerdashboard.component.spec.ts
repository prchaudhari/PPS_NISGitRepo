import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { InstancemanagerdashboardComponent } from './instancemanagerdashboard.component';

describe('InstancemanagerdashboardComponent', () => {
  let component: InstancemanagerdashboardComponent;
  let fixture: ComponentFixture<InstancemanagerdashboardComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ InstancemanagerdashboardComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(InstancemanagerdashboardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
