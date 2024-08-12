import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';
import { PutAwayComponent } from './put-away.component';

describe('AutomobileTableComponent', () => {
  let component: PutAwayComponent;
  let fixture: ComponentFixture<PutAwayComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [PutAwayComponent]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PutAwayComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
