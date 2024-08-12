import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';
import { ItemMasterlistReferenceTableComponent } from './item-masterlist-reference-table.component';

describe('AutomobileTableComponent', () => {
  let component: ItemMasterlistReferenceTableComponent;
  let fixture: ComponentFixture<ItemMasterlistReferenceTableComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ItemMasterlistReferenceTableComponent]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ItemMasterlistReferenceTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
