import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ListEtlscheduleComponent } from './list-etlschedule.component';

describe('ListEtlscheduleComponent', () => {
  let component: ListEtlscheduleComponent;
  let fixture: ComponentFixture<ListEtlscheduleComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ListEtlscheduleComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ListEtlscheduleComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
