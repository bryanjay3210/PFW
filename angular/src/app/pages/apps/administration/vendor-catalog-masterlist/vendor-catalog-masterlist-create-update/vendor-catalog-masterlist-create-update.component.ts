import { Component, Inject, OnInit } from '@angular/core';
import { UntypedFormBuilder, UntypedFormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import moment from 'moment';
import { AlertService } from 'src/services/alert.service';
import { ModuleCode, UserPermission } from 'src/static-data/enums/enums';
import { fadeInUp400ms } from 'src/@vex/animations/fade-in-up.animation';
import { stagger40ms } from 'src/@vex/animations/stagger.animation';
import { VendorCatalog, Role, User } from 'src/services/interfaces/models';
import { Lookup } from 'src/services/interfaces/lookup.model';


@Component({
  selector: 'vex-vendor-catalog-masterlist-create-update',
  templateUrl: './vendor-catalog-masterlist-create-update.component.html',
  styleUrls: ['./vendor-catalog-masterlist-create-update.component.scss'],
  animations: [
    fadeInUp400ms,
    stagger40ms
  ]
})
export class VendorCatalogMasterlistCreateUpdateComponent implements OnInit {

  static id = 100;
  imageDefault = "assets/img/pfw_logo_sm.png";

  form: UntypedFormGroup;
  mode: 'create' | 'update' = 'create';
  inputType = 'password';

  roleList: Role[];
  userTypeList: Lookup[];
  accessTypeList: Lookup[];
 
  visible = false;
 
  currentUser = {} as User;
  userPermission = UserPermission;
  access = UserPermission.NoAccess;

  constructor(@Inject(MAT_DIALOG_DATA) public defaults: VendorCatalog,
    private dialogRef: MatDialogRef<VendorCatalogMasterlistCreateUpdateComponent>,
    private fb: UntypedFormBuilder,
    private alertService: AlertService) {
    this.currentUser = JSON.parse(localStorage.getItem('CurrentUser'));
    let modulePermission = this.currentUser.role.rolePermissions.find(rp => rp.module.code == ModuleCode.VendorCatalogMasterlist);
    this.access = modulePermission ? modulePermission.accessTypeId : UserPermission.NoAccess;
  }

  ngOnInit() {
    if (this.defaults) {
      this.mode = 'update';
    } else {
      this.defaults = {} as VendorCatalog;
    }

    this.form = this.fb.group({
      id: [VendorCatalogMasterlistCreateUpdateComponent.id++],
      vendorCode: [this.defaults.vendorCode || '', Validators.required],
      vendorPartNumber: [this.defaults.vendorPartNumber || '', Validators.required],
      partsLinkNumber: [this.defaults.partsLinkNumber || '', Validators.required],
      price: [this.defaults.price || '', Validators.required],
      onHand: [this.defaults.onHand || '', Validators.required]
    });
  }

  save() {
    if (this.isCreateMode()) {
      if (this.form.valid) {
        this.alertService.createNotification("Vendor Catalog").then(answer => {
          if (!answer.isConfirmed) return;
          this.createVendorCatalog();
        });
      }
      else this.alertService.validationNotification("Vendor Catalog");
    }
    else if (this.isUpdateMode()) {
      if (this.form.valid) {
        this.alertService.updateNotification("Vendor Catalog").then(answer => {
          if (!answer.isConfirmed) return;
          this.updateVendorCatalog();
        });
      }
      else this.alertService.validationNotification("Vendor Catalog");
    }
  }

  createVendorCatalog() {
    const vendorCatalog = {} as VendorCatalog;
    this.mapFormValuesToVendorCatalog(vendorCatalog);
    this.dialogRef.close(vendorCatalog);
  }

  updateVendorCatalog() {
    const vendorCatalog = {} as VendorCatalog;
    this.mapFormValuesToVendorCatalog(vendorCatalog);
    this.dialogRef.close(vendorCatalog);
  }

  mapFormValuesToVendorCatalog(vendorCatalog: VendorCatalog) {
    vendorCatalog.vendorCode = this.form.value.vendorCode;
    vendorCatalog.vendorPartNumber = this.form.value.vendorPartNumber;
    vendorCatalog.partsLinkNumber = this.form.value.partsLinkNumber;
    vendorCatalog.price = this.form.value.price;
    vendorCatalog.onHand = this.form.value.onHand;

    vendorCatalog.isActive = this.isCreateMode() ? true : this.defaults.isActive;
    vendorCatalog.isDeleted = this.isCreateMode() ? false : this.defaults.isDeleted;
    vendorCatalog.createdBy = this.isCreateMode() ? this.currentUser.userName : this.defaults.createdBy;
    vendorCatalog.createdDate = this.isCreateMode() ? moment(new Date()) : this.defaults.createdDate;

    if (this.isUpdateMode()) {
      vendorCatalog.modifiedBy = this.currentUser.userName;
      vendorCatalog.modifiedDate = moment(new Date());
      vendorCatalog.id = this.defaults.id;
    }
  }

  isCreateMode() {
    return this.mode === 'create';
  }

  isUpdateMode() {
    return this.mode === 'update';
  }
}