import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';
import { DriverLogTableComponent } from './driver-log-table.component';

describe('AutomobileTableComponent', () => {
  let component: DriverLogTableComponent;
  let fixture: ComponentFixture<DriverLogTableComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [DriverLogTableComponent]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DriverLogTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
