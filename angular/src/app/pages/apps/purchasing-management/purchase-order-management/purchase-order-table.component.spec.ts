import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';
import { PurchaseOrderTableComponent } from './purchase-order-table.component';

describe('AutomobileTableComponent', () => {
  let component: PurchaseOrderTableComponent;
  let fixture: ComponentFixture<PurchaseOrderTableComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [PurchaseOrderTableComponent]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PurchaseOrderTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
