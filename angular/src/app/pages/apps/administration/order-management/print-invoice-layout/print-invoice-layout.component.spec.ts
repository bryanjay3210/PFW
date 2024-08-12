import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PrintInvoiceLayoutComponent } from './print-invoice-layout.component';

describe('PrintInvoiceLayoutComponent', () => {
  let component: PrintInvoiceLayoutComponent;
  let fixture: ComponentFixture<PrintInvoiceLayoutComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PrintInvoiceLayoutComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PrintInvoiceLayoutComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
