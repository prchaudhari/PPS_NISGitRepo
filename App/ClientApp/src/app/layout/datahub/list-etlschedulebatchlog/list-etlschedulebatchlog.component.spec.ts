import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ListEtlschedulebatchlogComponent } from './list-etlschedulebatchlog.component';

describe('ListEtlschedulebatchlogComponent', () => {
  let component: ListEtlschedulebatchlogComponent;
  let fixture: ComponentFixture<ListEtlschedulebatchlogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ListEtlschedulebatchlogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ListEtlschedulebatchlogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
