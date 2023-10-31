import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { EtlscheduledetailComponent } from './etlscheduledetail.component';

describe('EtlscheduledetailComponent', () => {
  let component: EtlscheduledetailComponent;
  let fixture: ComponentFixture<EtlscheduledetailComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ EtlscheduledetailComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(EtlscheduledetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
