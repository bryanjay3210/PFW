import { ComponentFixture, TestBed } from '@angular/core/testing';
import { PaymentCustomerListComponent } from './payment-customer-list.component';

describe('PaymentCustomerListComponent', () => {
  let component: PaymentCustomerListComponent;
  let fixture: ComponentFixture<PaymentCustomerListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PaymentCustomerListComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PaymentCustomerListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
