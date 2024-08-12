import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';
import { CustomerSalesReportComponent } from './customer-sales-report.component';

describe('CustomerSalesReportComponent', () => {
  let component: CustomerSalesReportComponent;
  let fixture: ComponentFixture<CustomerSalesReportComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [CustomerSalesReportComponent]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CustomerSalesReportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
