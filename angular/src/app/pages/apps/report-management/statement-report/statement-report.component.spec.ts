import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';
import { StatementReportComponent } from './statement-report.component';

describe('StatementReportComponent', () => {
  let component: StatementReportComponent;
  let fixture: ComponentFixture<StatementReportComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [StatementReportComponent]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(StatementReportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
