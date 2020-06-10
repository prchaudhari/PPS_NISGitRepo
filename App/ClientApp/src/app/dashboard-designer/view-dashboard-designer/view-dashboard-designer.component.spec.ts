import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ViewDashboardDesignerComponent } from './view-dashboard-designer.component';

describe('ViewDashboardDesignerComponent', () => {
  let component: ViewDashboardDesignerComponent;
  let fixture: ComponentFixture<ViewDashboardDesignerComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ViewDashboardDesignerComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ViewDashboardDesignerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
