import { ComponentFixture, TestBed } from '@angular/core/testing';

import { WindowButtonComponent } from './window-button.component';

describe('WindowButtonComponent', () => {
  let component: WindowButtonComponent;
  let fixture: ComponentFixture<WindowButtonComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [WindowButtonComponent]
    });
    fixture = TestBed.createComponent(WindowButtonComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
