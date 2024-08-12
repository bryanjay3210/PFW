import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';
import { PartsManifestTableComponent } from './parts-manifest-table.component';

describe('AutomobileTableComponent', () => {
  let component: PartsManifestTableComponent;
  let fixture: ComponentFixture<PartsManifestTableComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [PartsManifestTableComponent]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PartsManifestTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
