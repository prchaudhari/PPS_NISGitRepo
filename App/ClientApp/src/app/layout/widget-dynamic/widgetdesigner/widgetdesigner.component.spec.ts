import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { WidgetdesignerComponent } from './widgetdesigner.component';

describe('WidgetdesignerComponent', () => {
  let component: WidgetdesignerComponent;
  let fixture: ComponentFixture<WidgetdesignerComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ WidgetdesignerComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(WidgetdesignerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
