import { ChangeDetectorRef, Component, Inject, OnInit } from '@angular/core';
import { UntypedFormBuilder, UntypedFormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import moment from 'moment';
import { AlertService } from 'src/services/alert.service';
import { LookupService } from 'src/services/lookup.service';
import { RoleService } from 'src/services/role.service';
import { CustomerService } from 'src/services/customer.service';
import { LocationService } from 'src/services/location.service';
import { ModuleCode, UserPermission } from 'src/static-data/enums/enums';
import { UserDTO, Location, Customer, Role, User } from 'src/services/interfaces/models';
import { Lookup } from 'src/services/interfaces/lookup.model';


@Component({
  selector: 'vex-user-create-update',
  templateUrl: './user-create-update.component.html',
  styleUrls: ['./user-create-update.component.scss']
})
export class UserCreateUpdateComponent implements OnInit {

  static id = 100;
  imageDefault = "assets/img/pfw_logo_sm.png";

  form: UntypedFormGroup;
  mode: 'create' | 'update' = 'create';
  inputType = 'password';

  roleList: Role[];
  customerList: Customer[];
  locationList: Location[]
  filteredLocationList: Location[]
  userTypeList: Lookup[];
  accessTypeList: Lookup[];

  isChangePasswordOnLogin: boolean = true;
  isActivated: boolean = false;
  visible = false;
  isCustomerUser: number = 0;

  currentUser = {} as User;
  userPermission = UserPermission;
  access = UserPermission.NoAccess;

  constructor(@Inject(MAT_DIALOG_DATA) public defaults: User,
    private dialogRef: MatDialogRef<UserCreateUpdateComponent>,
    private fb: UntypedFormBuilder,
    private cd: ChangeDetectorRef,
    private roleService: RoleService,
    private customerService: CustomerService,
    private locationService: LocationService,
    private lookupService: LookupService,
    private alertService: AlertService) {
    this.currentUser = JSON.parse(localStorage.getItem('CurrentUser'));
    let modulePermission = this.currentUser.role.rolePermissions.find(rp => rp.module.code == ModuleCode.UserManagement);
    this.access = modulePermission.accessTypeId;
  }

  ngOnInit() {
    this.getLookups();
    if (this.defaults) {
      this.mode = 'update';
      this.isCustomerUser = this.defaults.isCustomerUser ? 2 : 1;
      this.isActivated = this.defaults.isActivated;
      this.isChangePasswordOnLogin = this.defaults.isChangePasswordOnLogin;
    } else {
      this.defaults = {} as User;
    }

    this.form = this.fb.group({
      id: [UserCreateUpdateComponent.id++],
      userName: [this.defaults.userName || '', Validators.required],
      email: [this.defaults.email || '', Validators.required],
      roleId: [this.defaults.roleId || '', Validators.required],
      isCustomerUser: [this.isCustomerUser || 0, Validators.required],
      isActivated: [this.defaults.isActivated || false, Validators.required],
      isChangePasswordOnLogin: [this.defaults.isChangePasswordOnLogin || true, Validators.required],
      customerId: [this.defaults.customerId || ''],
      locationId: [this.defaults.locationId || ''],
      password: [this.defaults.passwordHash || '', Validators.required],
      passwordConfirm: [this.defaults.passwordHash || '', Validators.required],
    });
  }

  getLookups() {
    this.roleService.getRoles().subscribe((result: Role[]) => (this.roleList = result));
    this.customerService.getCustomers().subscribe((result: Customer[]) => (this.customerList = result));
    this.locationService.getLocations().subscribe((result: Location[]) => {
      (this.locationList = result)
      if (this.isUpdateMode()) {
        this.filteredLocationList = this.locationList.filter(e => e.customerId === this.defaults.customerId);
      }
    });
    this.lookupService.getAccessTypes().subscribe((result: Lookup[]) => (this.accessTypeList = result));
    this.lookupService.getUserTypes().subscribe((result: Lookup[]) => (this.userTypeList = result));
  }

  save() {
    if (this.isCreateMode()) {
      if (this.form.valid) {
        this.alertService.createNotification("User").then(answer => {
          if (!answer.isConfirmed) return;
          this.createUser();
        });
      }
      else this.alertService.validationNotification("User");
    }
    else if (this.isUpdateMode()) {
      if (this.form.valid) {
        this.alertService.updateNotification("User").then(answer => {
          if (!answer.isConfirmed) return;
          this.updateUser();
        });
      }
      else this.alertService.validationNotification("User");
    }
  }

  createUser() {
    const userdto = {} as UserDTO;
    this.mapFormValuesToUser(userdto);
    this.dialogRef.close(userdto);
  }

  updateUser() {
    const user = {} as User;
    this.mapFormValuesToUser(user);
    this.dialogRef.close(user);
  }

  mapFormValuesToUser(user: any) {
    user.userName = this.form.value.userName;
    user.email = this.form.value.email;
    user.roleId = this.form.value.roleId;
    
    if (this.isCreateMode()) {
      user.password = this.form.value.password;
    }
    else {
      user.id = this.defaults.id;
      user.passwordHash = this.defaults.passwordHash;
      user.passwordSalt = this.defaults.passwordSalt;
      user.isDeleted = this.defaults.isDeleted;
      user.modifiedBy = this.currentUser.userName;
      user.modifiedDate = moment(new Date());
    }

    user.isCustomerUser = this.form.value.isCustomerUser === 2;
    if (user.isCustomerUser) {
      user.customerId = this.form.value.customerId;
      user.locationId = this.form.value.locationId;
    }
    
    user.createdDate = this.isCreateMode ? moment(new Date()) : this.defaults.createdDate;
    user.createdBy = this.isCreateMode ? this.currentUser.userName : this.defaults.createdBy;
    user.isActive = this.isCreateMode ? true : this.defaults.isActive;
    user.isActivated = this.isActivated;
    user.isChangePasswordOnLogin = this.isChangePasswordOnLogin;
  }
  
  isCreateMode() {
    return this.mode === 'create';
  }

  isUpdateMode() {
    return this.mode === 'update';
  }

  onIsChangePasswordOnLoginClick(checked: boolean) {
    this.isChangePasswordOnLogin = checked;
  }

  onActivateClick(checked: boolean) {
    this.isActivated = checked;
  }

  toggleVisibility() {
    if (this.visible) {
      this.inputType = 'password';
      this.visible = false;
      this.cd.markForCheck();
    } else {
      this.inputType = 'text';
      this.visible = true;
      this.cd.markForCheck();
    }
  }

  onCustomerChange(event) {
    this.filteredLocationList = this.locationList.filter(e => e.customerId === event.value);
  }
}
