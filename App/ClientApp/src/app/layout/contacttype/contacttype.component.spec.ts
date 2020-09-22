import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ContacttypeComponent } from './contacttype.component';

describe('ContacttypeComponent', () => {
  let component: ContacttypeComponent;
  let fixture: ComponentFixture<ContacttypeComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ContacttypeComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ContacttypeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
