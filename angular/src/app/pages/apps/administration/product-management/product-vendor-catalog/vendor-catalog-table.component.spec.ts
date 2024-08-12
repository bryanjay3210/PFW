import { ComponentFixture, TestBed } from '@angular/core/testing';

import { VendorCatalogTableComponent } from './vendor-catalog-table.component';

describe('VendorCatalogTableComponent', () => {
  let component: VendorCatalogTableComponent;
  let fixture: ComponentFixture<VendorCatalogTableComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ VendorCatalogTableComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(VendorCatalogTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
