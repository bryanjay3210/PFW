import { Component, Inject, OnInit } from '@angular/core';
import { UntypedFormBuilder, UntypedFormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialog } from '@angular/material/dialog';
import moment from 'moment';
import { DemoDialogComponent } from 'src/app/pages/ui/components/components-overview/components/components-overview-dialogs/components-overview-dialogs.component';
import { AlertService } from 'src/services/alert.service';
import { Lookup } from 'src/services/interfaces/lookup.model';
import { LookupService } from 'src/services/lookup.service';
import { ModuleGroupService } from 'src/services/modulegroup.service';
import { ModuleCode, UserPermission } from 'src/static-data/enums/enums';
import { ModuleGroup, Role, RolePermission, User } from 'src/services/interfaces/models';

@Component({
  selector: 'vex-roles-permissions-create-update',
  templateUrl: './roles-permissions-create-update.component.html',
  styleUrls: ['./roles-permissions-create-update.component.scss']
})
export class RolesPermissionsCreateUpdateComponent implements OnInit {

  static id = 100;
  imageDefault = "assets/img/pfw_logo_sm.png";

  form: UntypedFormGroup;
  mode: 'create' | 'update' = 'create';

  moduleGroupList: ModuleGroup[];
  userTypeList: Lookup[];
  accessTypeList: Lookup[];

  currentUser = {} as User;
  userPermission = UserPermission;
  access = UserPermission.NoAccess;

  constructor(@Inject(MAT_DIALOG_DATA) public defaults: Role,
    private dialogRef: MatDialogRef<RolesPermissionsCreateUpdateComponent>,
    private fb: UntypedFormBuilder,
    private moduleGroupService: ModuleGroupService,
    private lookupService: LookupService,
    private alertService: AlertService) {
    this.currentUser = JSON.parse(localStorage.getItem('CurrentUser'));
    let modulePermission = this.currentUser.role.rolePermissions.find(rp => rp.module.code == ModuleCode.RolesAndPermissions);
    this.access = modulePermission.accessTypeId;
  }

  ngOnInit() {
    this.getModuleGroups();
    this.getLookups();
    if (this.defaults) {
      this.mode = 'update';
    } else {
      this.defaults = {} as Role;
    }

    this.form = this.fb.group({
      id: [RolesPermissionsCreateUpdateComponent.id++],
      name: [this.defaults.name || '', Validators.required],
      code: [this.defaults.code || '', Validators.required],
      // sortOrder: [this.defaults.sortOrder || 0, Validators.required],
      userTypeId: [this.defaults.userTypeId || '', Validators.required],
      description: [this.defaults.description || ''],
      notes: [this.defaults.notes || '']
    });
  }

  getModuleGroups() {
    this.moduleGroupService.getModuleGroups().subscribe((result: ModuleGroup[]) => {
      this.moduleGroupList = result;
      
        this.mapDefaultsPermissions();
      // }
    });
  }

  mapDefaultsPermissions() {
    if (this.moduleGroupList) {
      this.moduleGroupList.forEach(mg => {
        mg.modules.forEach(m => {
          let accessTypeId = 1;
          if (this.isUpdateMode()) {
            let modulePermission = this.defaults.rolePermissions.find(p => p.moduleGroupId === mg.id && p.moduleId === m.id);
            accessTypeId = modulePermission ? modulePermission.accessTypeId : 1;
          }
          m.accessTypeId = accessTypeId;
        })
      })
    }
  }

  getLookups() {
    this.lookupService.getAccessTypes().subscribe((result: Lookup[]) => (this.accessTypeList = result));
    this.lookupService.getUserTypes().subscribe((result: Lookup[]) => (this.userTypeList = result));
  }

  save() {
    if (this.isCreateMode()) {
      if (this.form.valid) {
        this.alertService.createNotification("Role").then(answer => {
          if (!answer.isConfirmed) return;
          this.createRole();
        });
      }
      else this.alertService.validationNotification("Role");
    }
    else if (this.isUpdateMode()) {
      if (this.form.valid) {
        this.alertService.updateNotification("Role").then(answer => {
          if (!answer.isConfirmed) return;
          this.updateRole();
        });
      }
      else this.alertService.validationNotification("Role");
    }
  }

  createRole() {
    const role = {} as Role;
    this.mapFormValuesToRole(role);
    this.dialogRef.close(role);
  }

  updateRole() {
    const role = {} as Role;
    this.mapFormValuesToRole(role);
    this.dialogRef.close(role);
  }

  mapFormValuesToRole(role: any) {
    role.name = this.form.value.name;
    role.code = this.form.value.code;
    role.userTypeId = this.form.value.userTypeId;
    role.description = this.form.value.description;
    role.notes = this.form.value.notes;

    role.sortOrder = this.isCreateMode() ? 0 : this.defaults.sortOrder;
    role.isActive = this.isCreateMode() ? true : this.defaults.isActive;
    role.isDeleted = this.isCreateMode() ? false : this.defaults.isDeleted;
    role.createdBy = this.isCreateMode() ? this.currentUser.userName : this.defaults.createdBy;
    role.createdDate = this.isCreateMode() ? moment(new Date()) : this.defaults.createdDate;

    if (this.isUpdateMode()) {
      role.modifiedBy = this.currentUser.userName;
      role.modifiedDate = moment(new Date());
      role.id = this.defaults.id;
    }

    role.rolePermissions = this.mapUIDataToModel(role);
  }
  
  mapUIDataToModel(role: Role): RolePermission[] {
    let result = new Array<RolePermission>;

    this.moduleGroupList.forEach(moduleGroup => {
      if (moduleGroup.modules.length > 0) {
        moduleGroup.modules.forEach(module => {
          const rolePermission = {} as RolePermission;
          
          if (this.isCreateMode()) {
            rolePermission.id = 0;
          }
          else {
            let rp = this.defaults.rolePermissions.find(rp => rp.roleId === this.defaults.id && rp.moduleGroupId === moduleGroup.id && rp.moduleId === module.id);
            rolePermission.id = rp && rp !== undefined ? rp.id : 0;
          }
          
          rolePermission.roleId = this.isCreateMode() ? 0 : role.id
          rolePermission.moduleGroupId = moduleGroup.id;
          rolePermission.moduleId = module.id;
          rolePermission.accessTypeId = module.accessTypeId;
          rolePermission.isActive = role.isActive;
          rolePermission.isDeleted = role.isDeleted;
          rolePermission.createdBy = role.createdBy;
          rolePermission.createdDate = role.createdDate;

          if (this.isUpdateMode()) {
            rolePermission.modifiedBy = role.modifiedBy;
            rolePermission.modifiedDate = role.modifiedDate;
          }

          result.push(rolePermission);
        });
      }
    });

    return result;
  }

  isCreateMode() {
    return this.mode === 'create';
  }

  isUpdateMode() {
    return this.mode === 'update';
  }


}
