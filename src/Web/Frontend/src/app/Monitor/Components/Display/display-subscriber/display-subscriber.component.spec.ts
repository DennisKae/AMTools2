import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DisplaySubscriberComponent } from './display-subscriber.component';

describe('DisplaySubscriberComponent', () => {
  let component: DisplaySubscriberComponent;
  let fixture: ComponentFixture<DisplaySubscriberComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DisplaySubscriberComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DisplaySubscriberComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
