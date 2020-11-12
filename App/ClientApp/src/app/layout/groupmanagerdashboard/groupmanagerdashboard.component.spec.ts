import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { GroupmanagerdashboardComponent } from './groupmanagerdashboard.component';

describe('GroupmanagerdashboardComponent', () => {
  let component: GroupmanagerdashboardComponent;
  let fixture: ComponentFixture<GroupmanagerdashboardComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ GroupmanagerdashboardComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(GroupmanagerdashboardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
