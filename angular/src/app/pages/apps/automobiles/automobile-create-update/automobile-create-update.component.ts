import { Component, Inject, OnInit } from '@angular/core';
import { UntypedFormBuilder, UntypedFormGroup } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialog } from '@angular/material/dialog';
import moment from 'moment';
import { DemoDialogComponent } from 'src/app/pages/ui/components/components-overview/components/components-overview-dialogs/components-overview-dialogs.component';
import { Automobile } from 'src/services/interfaces/automobile.model';

@Component({
  selector: 'vex-automobile-create-update',
  templateUrl: './automobile-create-update.component.html',
  styleUrls: ['./automobile-create-update.component.scss']
})
export class AutomobileCreateUpdateComponent implements OnInit {

  static id = 100;
  imageDefault = "assets/img/pfw_logo_sm.png";

  form: UntypedFormGroup;
  mode: 'create' | 'update' = 'create';

  constructor(@Inject(MAT_DIALOG_DATA) public defaults: any,
              private dialogRef: MatDialogRef<AutomobileCreateUpdateComponent>,
              private fb: UntypedFormBuilder,
              private dialog: MatDialog) {
  }

  ngOnInit() {
    if (this.defaults) {
      this.mode = 'update';
    } else {
      this.defaults = {} as Automobile;
    }

    this.form = this.fb.group({
      id: [AutomobileCreateUpdateComponent.id++],
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
      this.createAutomobile();
    } else if (this.mode === 'update') {
      this.openDialog();
      // this.updateAutomobile();
    }
  }

  openDialog() {
    this.dialog.open(DemoDialogComponent, {
      disableClose: false,
      width: '400px'
    }).afterClosed().subscribe(result => {
      if (result == "Yes")
      {
        this.updateAutomobile();
      }
    });
  }

  createAutomobile() {
    let automobile = new Automobile();  
    automobile.name = this.form.value.name;
    automobile.make = this.form.value.make;
    automobile.model = this.form.value.model;
    automobile.type = this.form.value.type;
    automobile.year = this.form.value.year;
    automobile.notes = this.form.value.notes;
    automobile.isActive = true;
    automobile.isDeleted = false;
    automobile.createdBy = "demo@user.com";
    automobile.createdDate = moment(new Date());
    this.dialogRef.close(automobile);
  }

  updateAutomobile() {
    let automobile = new Automobile();  
    automobile.name = this.form.value.name;
    automobile.make = this.form.value.make;
    automobile.model = this.form.value.model;
    automobile.type = this.form.value.type;
    automobile.year = this.form.value.year;
    automobile.notes = this.form.value.notes;
    automobile.isActive = this.defaults.isActive;
    automobile.isDeleted = this.defaults.isDeleted;
    automobile.createdBy = this.defaults.createdBy;
    automobile.createdDate = this.defaults.createdDate;
    automobile.modifiedBy = "modify@user.com";
    automobile.modifiedDate = moment(new Date());
    automobile.id = this.defaults.id;

    this.dialogRef.close(automobile);
  }

  isCreateMode() {
    return this.mode === 'create';
  }

  isUpdateMode() {
    return this.mode === 'update';
  }
}
