import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ReportCustomerListComponent } from './report-customer-list.component';

describe('CustomerListComponent', () => {
  let component: ReportCustomerListComponent;
  let fixture: ComponentFixture<ReportCustomerListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ReportCustomerListComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ReportCustomerListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
