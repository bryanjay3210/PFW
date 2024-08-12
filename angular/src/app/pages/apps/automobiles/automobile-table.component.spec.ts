import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { AutomobileTableComponent } from './automobile-table.component';

describe('AutomobileTableComponent', () => {
  let component: AutomobileTableComponent;
  let fixture: ComponentFixture<AutomobileTableComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [AutomobileTableComponent]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AutomobileTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
