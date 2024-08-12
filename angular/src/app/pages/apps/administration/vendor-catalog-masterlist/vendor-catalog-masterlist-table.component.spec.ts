import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';
import { VendorCatalogMasterlistTableComponent } from './vendor-catalog-masterlist-table.component';

describe('VendorCatalogMasterlistTableComponent', () => {
  let component: VendorCatalogMasterlistTableComponent;
  let fixture: ComponentFixture<VendorCatalogMasterlistTableComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [VendorCatalogMasterlistTableComponent]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(VendorCatalogMasterlistTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
