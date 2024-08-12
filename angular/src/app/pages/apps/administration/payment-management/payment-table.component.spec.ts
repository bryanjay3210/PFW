import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';
import { PaymentTableComponent } from './payment-table.component';

describe('AutomobileTableComponent', () => {
  let component: PaymentTableComponent;
  let fixture: ComponentFixture<PaymentTableComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [PaymentTableComponent]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PaymentTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
