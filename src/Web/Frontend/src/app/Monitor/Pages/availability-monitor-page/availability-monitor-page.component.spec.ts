import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AvailabilityMonitorPageComponent } from './availability-monitor-page.component';

describe('AvailabilityMonitorPageComponent', () => {
  let component: AvailabilityMonitorPageComponent;
  let fixture: ComponentFixture<AvailabilityMonitorPageComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AvailabilityMonitorPageComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AvailabilityMonitorPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
