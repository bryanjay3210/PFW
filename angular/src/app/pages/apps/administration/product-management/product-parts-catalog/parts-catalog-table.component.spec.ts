import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PartsCatalogTableComponent } from './parts-catalog-table.component';

describe('PartsCatalogTableComponent', () => {
  let component: PartsCatalogTableComponent;
  let fixture: ComponentFixture<PartsCatalogTableComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PartsCatalogTableComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PartsCatalogTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
