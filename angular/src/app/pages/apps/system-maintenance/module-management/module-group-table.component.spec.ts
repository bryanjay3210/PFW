import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';
import { ModuleGroupTableComponent } from './module-group-table.component';

describe('AutomobileTableComponent', () => {
  let component: ModuleGroupTableComponent;
  let fixture: ComponentFixture<ModuleGroupTableComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ModuleGroupTableComponent]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ModuleGroupTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
