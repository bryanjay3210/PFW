import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';
import { CustomerOrderTableComponent } from './customer-order-table.component';

describe('AutomobileTableComponent', () => {
  let component: CustomerOrderTableComponent;
  let fixture: ComponentFixture<CustomerOrderTableComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [CustomerOrderTableComponent]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CustomerOrderTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
