import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';
import { StockSettingsComponent } from './stock-settings.component';

describe('AutomobileTableComponent', () => {
  let component: StockSettingsComponent;
  let fixture: ComponentFixture<StockSettingsComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [StockSettingsComponent]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(StockSettingsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
