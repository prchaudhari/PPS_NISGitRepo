import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PagetypeComponent } from './pagetype.component';

describe('PagetypeComponent', () => {
  let component: PagetypeComponent;
  let fixture: ComponentFixture<PagetypeComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PagetypeComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PagetypeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
