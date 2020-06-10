import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AddDashboardDesignerComponent } from './add-dashboard-designer.component';

describe('AddDashboardDesignerComponent', () => {
  let component: AddDashboardDesignerComponent;
  let fixture: ComponentFixture<AddDashboardDesignerComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AddDashboardDesignerComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AddDashboardDesignerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
