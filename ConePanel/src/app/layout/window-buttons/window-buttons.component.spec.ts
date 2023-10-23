import { ComponentFixture, TestBed } from '@angular/core/testing';

import { WindowButtonsComponent } from './window-buttons.component';

describe('WindowButtonsComponent', () => {
  let component: WindowButtonsComponent;
  let fixture: ComponentFixture<WindowButtonsComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [WindowButtonsComponent]
    });
    fixture = TestBed.createComponent(WindowButtonsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
