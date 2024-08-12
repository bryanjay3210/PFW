import { Component, Inject, OnInit } from '@angular/core';
import { UntypedFormBuilder, UntypedFormGroup } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialog } from '@angular/material/dialog';
import moment from 'moment';
import { DemoDialogComponent } from 'src/app/pages/ui/components/components-overview/components/components-overview-dialogs/components-overview-dialogs.component';
import { AccountsReceivable } from 'src/services/interfaces/accountsreceivable.model';

@Component({
  selector: 'vex-accounts-receivable-create-update',
  templateUrl: './accounts-receivable-create-update.component.html',
  styleUrls: ['./accounts-receivable-create-update.component.scss']
})
export class AccountsReceivableCreateUpdateComponent implements OnInit {

  static id = 100;
  imageDefault = "assets/img/pfw_logo_sm.png";

  form: UntypedFormGroup;
  mode: 'create' | 'update' = 'create';

  constructor(@Inject(MAT_DIALOG_DATA) public defaults: any,
              private dialogRef: MatDialogRef<AccountsReceivableCreateUpdateComponent>,
              private fb: UntypedFormBuilder,
              private dialog: MatDialog) {
  }

  ngOnInit() {
    if (this.defaults) {
      this.mode = 'update';
    } else {
      this.defaults = {} as AccountsReceivable;
    }

    this.form = this.fb.group({
      id: [AccountsReceivableCreateUpdateComponent.id++],
      name: this.defaults.name,
      make: this.defaults.make,
      model: this.defaults.model,
      type: this.defaults.type,
      year: this.defaults.year,
      notes: this.defaults.notes
    });
  }

  save() {
    if (this.mode === 'create') {
      this.createAccountsReceivable();
    } else if (this.mode === 'update') {
      this.openDialog();
      // this.updateAccountsReceivable();
    }
  }

  openDialog() {
    this.dialog.open(DemoDialogComponent, {
      disableClose: false,
      width: '400px'
    }).afterClosed().subscribe(result => {
      if (result == "Yes")
      {
        this.updateAccountsReceivable();
      }
    });
  }

  createAccountsReceivable() {
    let accountsReceivable = new AccountsReceivable();  
    accountsReceivable.name = this.form.value.name;
    accountsReceivable.make = this.form.value.make;
    accountsReceivable.model = this.form.value.model;
    accountsReceivable.type = this.form.value.type;
    accountsReceivable.year = this.form.value.year;
    accountsReceivable.notes = this.form.value.notes;
    accountsReceivable.isActive = true;
    accountsReceivable.isDeleted = false;
    accountsReceivable.createdBy = "demo@user.com";
    accountsReceivable.createdDate = moment(new Date());
    this.dialogRef.close(accountsReceivable);
  }

  updateAccountsReceivable() {
    let accountsReceivable = new AccountsReceivable();  
    accountsReceivable.name = this.form.value.name;
    accountsReceivable.make = this.form.value.make;
    accountsReceivable.model = this.form.value.model;
    accountsReceivable.type = this.form.value.type;
    accountsReceivable.year = this.form.value.year;
    accountsReceivable.notes = this.form.value.notes;
    accountsReceivable.isActive = this.defaults.isActive;
    accountsReceivable.isDeleted = this.defaults.isDeleted;
    accountsReceivable.createdBy = this.defaults.createdBy;
    accountsReceivable.createdDate = this.defaults.createdDate;
    accountsReceivable.modifiedBy = "modify@user.com";
    accountsReceivable.modifiedDate = moment(new Date());
    accountsReceivable.id = this.defaults.id;

    this.dialogRef.close(accountsReceivable);
  }

  isCreateMode() {
    return this.mode === 'create';
  }

  isUpdateMode() {
    return this.mode === 'update';
  }
}
