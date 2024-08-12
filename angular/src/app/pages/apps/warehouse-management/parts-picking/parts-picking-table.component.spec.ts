import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';
import { PartsPickingTableComponent } from './parts-picking-table.component';

describe('AutomobileTableComponent', () => {
  let component: PartsPickingTableComponent;
  let fixture: ComponentFixture<PartsPickingTableComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [PartsPickingTableComponent]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PartsPickingTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
