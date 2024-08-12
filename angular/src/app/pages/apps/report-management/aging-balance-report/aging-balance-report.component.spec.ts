import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';
import { AgingBalanceReportComponent } from './aging-balance-report.component';

describe('AgingBalanceReportComponent', () => {
  let component: AgingBalanceReportComponent;
  let fixture: ComponentFixture<AgingBalanceReportComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [AgingBalanceReportComponent]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AgingBalanceReportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
