import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PartsLinkTableComponent } from './parts-link-table.component';

describe('PartsLinkTableComponent', () => {
  let component: PartsLinkTableComponent;
  let fixture: ComponentFixture<PartsLinkTableComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PartsLinkTableComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PartsLinkTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
