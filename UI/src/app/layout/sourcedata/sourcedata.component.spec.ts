import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SourcedataComponent } from './sourcedata.component';

describe('SourcedataComponent', () => {
  let component: SourcedataComponent;
  let fixture: ComponentFixture<SourcedataComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SourcedataComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SourcedataComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
