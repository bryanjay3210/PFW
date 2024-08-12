import { ChangeDetectorRef, Component, Inject, OnInit } from '@angular/core';
import { UntypedFormBuilder, UntypedFormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { Router } from '@angular/router';
import moment from 'moment';
import { AlertService } from 'src/services/alert.service';
import { User } from 'src/services/interfaces/models';
import { ModuleGroup } from 'src/services/interfaces/moduleGroup.model';
import { ModuleCode, UserPermission } from 'src/static-data/enums/enums';

@Component({
  selector: 'vex-module-group-create-update',
  templateUrl: './module-group-create-update.component.html',
  styleUrls: ['./module-group-create-update.component.scss']
})
export class ModuleGroupCreateUpdateComponent implements OnInit {

  static id = 100;
  imageDefault = "assets/img/pfw_logo_sm.png";

  form: UntypedFormGroup;
  mode: 'create' | 'update' = 'create';

  currentUser = {} as User;
  userPermission = UserPermission;
  access = UserPermission.NoAccess;

  constructor(@Inject(MAT_DIALOG_DATA) public defaults: ModuleGroup,
    private router: Router,
    private dialogRef: MatDialogRef<ModuleGroupCreateUpdateComponent>,
    private fb: UntypedFormBuilder,
    private cd: ChangeDetectorRef,
    private alertService: AlertService) {
    this.currentUser = JSON.parse(localStorage.getItem('CurrentUser'));
    let modulePermission = this.currentUser.role.rolePermissions.find(rp => rp.module.code == ModuleCode.ModuleManagement);
    this.access = modulePermission.accessTypeId;
  }

  ngOnInit() {
    if (this.currentUser === undefined || this.access === UserPermission.NoAccess) {
      this.alertService.unauthorizedNotification();
      this.router.navigate(['/login']);
      return;
    }

    if (this.defaults) {
      this.mode = 'update';
    } else {
      this.defaults = {} as ModuleGroup;
    }

    this.form = this.fb.group({
      id: [ModuleGroupCreateUpdateComponent.id++],
      name: [this.defaults.name || '', Validators.required],
      code: [this.defaults.code || '', Validators.required],
      description: [this.defaults.description || '', Validators.required],
    });
  }

  save() {
    if (this.isCreateMode()) {
      if (this.form.valid) {
        this.alertService.createNotification("ModuleGroup").then(answer => {
          if (!answer.isConfirmed) return;
          this.createModuleGroup();
        });
      }
      else this.alertService.validationNotification("ModuleGroup");
    }
    else if (this.isUpdateMode()) {
      if (this.form.valid) {
        this.alertService.updateNotification("ModuleGroup").then(answer => {
          if (!answer.isConfirmed) return;
          this.updateModuleGroup();
        });
      }
      else this.alertService.validationNotification("ModuleGroup");
    }
  }

  createModuleGroup() {
    let moduleGroup = new ModuleGroup();
    moduleGroup.name = this.form.value.name;
    moduleGroup.code = this.form.value.code;
    moduleGroup.description = this.form.value.description;
    moduleGroup.isActive = true;
    moduleGroup.isDeleted = false;
    moduleGroup.createdBy = "demo@user.com";
    moduleGroup.createdDate = moment(new Date());
    this.dialogRef.close(moduleGroup);
  }

  updateModuleGroup() {
    let moduleGroup = new ModuleGroup();
    moduleGroup.name = this.form.value.name;
    moduleGroup.code = this.form.value.code;
    moduleGroup.description = this.form.value.description;
    moduleGroup.isActive = this.defaults.isActive;
    moduleGroup.isDeleted = this.defaults.isDeleted;
    moduleGroup.createdBy = this.defaults.createdBy;
    moduleGroup.createdDate = this.defaults.createdDate;
    moduleGroup.modifiedBy = "modify@user.com";
    moduleGroup.modifiedDate = moment(new Date());
    moduleGroup.id = this.defaults.id;
    this.dialogRef.close(moduleGroup);
  }

  isCreateMode() {
    return this.mode === 'create';
  }

  isUpdateMode() {
    return this.mode === 'update';
  }
}
