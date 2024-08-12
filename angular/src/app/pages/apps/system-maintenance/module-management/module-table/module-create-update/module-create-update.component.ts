import { Component, Inject, OnInit } from '@angular/core';
import { UntypedFormBuilder, UntypedFormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import moment from 'moment';
import { AlertService } from 'src/services/alert.service';
import { CustomerService } from 'src/services/customer.service';
import { LookupService } from 'src/services/lookup.service';
import { Module } from 'src/services/interfaces/module.model';
import { Location } from 'src/services/interfaces/models';
import { Lookup } from 'src/services/interfaces/lookup.model';
import { ModuleCode, UserPermission } from 'src/static-data/enums/enums';
import { ModuleService } from 'src/services/module.service';
import { User } from 'src/services/interfaces/models';
import { LocationService } from 'src/services/location.service';

@Component({
  selector: 'vex-module-create-update',
  templateUrl: './module-create-update.component.html',
  styleUrls: ['./module-create-update.component.scss'],

})
export class ModuleCreateUpdateComponent implements OnInit {

  static id = 100;
  imageDefault = "https://icons.iconarchive.com/icons/treetog/junior/256/contacts-icon.png";

  form: UntypedFormGroup;
  mode: 'create' | 'update' = 'create';

  moduleList: Module[];
  locationList = {} as Location[];
  positionTypeList: Lookup[];
  moduleGroupId: number;

  currentUser = {} as User;
  userPermission = UserPermission;
  access = UserPermission.NoAccess;

  constructor(@Inject(MAT_DIALOG_DATA) public defaults: any,
    private dialogRef: MatDialogRef<ModuleCreateUpdateComponent>,
    private fb: UntypedFormBuilder,
    private moduleservice: ModuleService,
    private locationService: LocationService,
    private lookupService: LookupService,
    private alertService: AlertService) {
    this.currentUser = JSON.parse(localStorage.getItem('CurrentUser'));
    let modulePermission = this.currentUser.role.rolePermissions.find(rp => rp.module.code == ModuleCode.ModuleManagement);
    this.access = modulePermission.accessTypeId;
  }

  ngOnInit() {
    this.moduleservice.getModules().subscribe((result: Module[]) => (this.moduleList = result));
    this.locationService.getLocationsByCustomerId((this.defaults && isNaN(this.defaults)) ? this.defaults.customerId : this.defaults).subscribe((result = {} as Location[]) => (this.locationList = result));
    this.lookupService.getPositionTypes().subscribe((result: Lookup[]) => (this.positionTypeList = result));

    if (this.defaults && isNaN(this.defaults)) {
      this.mode = 'update';
    } else {
      this.moduleGroupId = this.defaults;
      this.defaults = {} as Module;
    }

    this.form = this.fb.group({
      id: [ModuleCreateUpdateComponent.id++],
      name: [this.defaults.name || '', Validators.required],
      code: [this.defaults.code || '', Validators.required],
      description: [this.defaults.description || '', Validators.required]
    });
  }

  save() {
    if (this.mode === 'create') {
      if (this.form.valid) {
        if (this.duplicateValueDetected()) {
          this.alertService.duplicateNotification("Module");
          return;
        }
        this.alertService.createNotification("Module").then(answer => {
          if (!answer.isConfirmed) return;
          this.createModule();
        });
      }
      else this.alertService.validationNotification("Module");
    }
    else if (this.mode === 'update') {
      if (this.form.valid) {
        if (this.duplicateValueDetected()) {
          this.alertService.duplicateNotification("Module");
          return;
        }
        this.alertService.updateNotification("Module").then(answer => {
          if (!answer.isConfirmed) return;
          this.updateModule();
        });
      }
      else this.alertService.validationNotification("Module");
    }
  }

  duplicateValueDetected(): boolean {
    let result: boolean = true;
    result = this.isCreateMode() ? this.moduleList.findIndex(m => m.code === this.form.value.code) !== -1 : 
    this.moduleList.filter(m => m.id !== this.defaults.id).findIndex(m => m.code === this.form.value.code) !== -1;
    return result
  }

  createModule() {
    let module = new Module();
    module.moduleGroupId = this.moduleGroupId;
    module.name = this.form.value.name;
    module.code = this.form.value.code;
    module.description = this.form.value.description;
    module.isActive = true;
    module.isDeleted = false;
    module.createdBy = "demo@user.com";
    module.createdDate = moment(new Date());
    this.dialogRef.close(module);
  }

  updateModule() {
    let module = new Module();
    module.name = this.form.value.name;
    module.code = this.form.value.code;
    module.description = this.form.value.description;
    module.moduleGroupId = this.defaults.moduleGroupId;
    module.isActive = this.defaults.isActive;
    module.isDeleted = this.defaults.isDeleted;
    module.createdBy = this.defaults.createdBy;
    module.createdDate = this.defaults.createdDate;
    module.modifiedBy = "modify@user.com";
    module.modifiedDate = moment(new Date());
    module.id = this.defaults.id;

    this.dialogRef.close(module);
  }

  isCreateMode() {
    return this.mode === 'create';
  }

  isUpdateMode() {
    return this.mode === 'update';
  }
}
