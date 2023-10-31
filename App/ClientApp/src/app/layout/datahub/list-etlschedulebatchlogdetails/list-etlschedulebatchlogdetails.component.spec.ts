import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ListEtlschedulebatchlogdetailsComponent } from './list-etlschedulebatchlogdetails.component';

describe('ListEtlschedulebatchlogdetailsComponent', () => {
  let component: ListEtlschedulebatchlogdetailsComponent;
  let fixture: ComponentFixture<ListEtlschedulebatchlogdetailsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ListEtlschedulebatchlogdetailsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ListEtlschedulebatchlogdetailsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
