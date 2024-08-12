import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { AccountsReceivableTableComponent } from './accounts-receivable-table.component';

describe('AccountsReceivableTableComponent', () => {
  let component: AccountsReceivableTableComponent;
  let fixture: ComponentFixture<AccountsReceivableTableComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [AccountsReceivableTableComponent]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AccountsReceivableTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
