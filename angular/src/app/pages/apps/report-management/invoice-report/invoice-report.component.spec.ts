import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';
import { InvoiceReportComponent } from './invoice-report.component';

describe('InvoiceReportComponent', () => {
  let component: InvoiceReportComponent;
  let fixture: ComponentFixture<InvoiceReportComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [InvoiceReportComponent]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(InvoiceReportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
