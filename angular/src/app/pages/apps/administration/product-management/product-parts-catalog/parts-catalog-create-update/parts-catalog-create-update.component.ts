import { Component, Inject, OnInit } from '@angular/core';
import { UntypedFormBuilder, UntypedFormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import moment from 'moment';
import { AlertService } from 'src/services/alert.service';
import { ModuleCode, UserPermission } from 'src/static-data/enums/enums';
import { PartsCatalog, User } from 'src/services/interfaces/models';

@Component({
  selector: 'vex-parts-catalog-create-update',
  templateUrl: './parts-catalog-create-update.component.html',
  styleUrls: ['./parts-catalog-create-update.component.scss'],

})
export class PartsCatalogCreateUpdateComponent implements OnInit {

  static id = 100;
  imageDefault = "https://icons.iconarchive.com/icons/treetog/junior/256/contacts-icon.png";

  form: UntypedFormGroup;
  mode: 'create' | 'update' = 'create';

  productId: number;
  productPartNumber: string;

  currentUser = {} as User;
  userPermission = UserPermission;
  access = UserPermission.NoAccess;

  constructor(
    @Inject(MAT_DIALOG_DATA) public defaults: any,
    private dialogRef: MatDialogRef<PartsCatalogCreateUpdateComponent>,
    private fb: UntypedFormBuilder,
    private alertService: AlertService) {
    this.currentUser = JSON.parse(localStorage.getItem('CurrentUser'));
    let modulePermission = this.currentUser.role.rolePermissions.find(rp => rp.module.code == ModuleCode.ProductManagement);
    this.access = modulePermission.accessTypeId;
  }

  ngOnInit() {
    if (this.defaults.data) {
      this.mode = 'update';
      this.defaults = this.defaults.data;
      this.productId = this.defaults.productId;
      this.productPartNumber = this.defaults.partNumber;
    } else {
      this.productId = this.defaults.productId;
      this.productPartNumber = this.defaults.partNumber;
      this.defaults = {} as PartsCatalog;
    }

    

    this.form = this.fb.group({
      id: [PartsCatalogCreateUpdateComponent.id++],
      // productId: [this.defaults.productId || '', Validators.required],
      // partNumber: [this.defaults.partNumber || '', Validators.required],
      make: [this.defaults.make || '', Validators.required],
      model: [this.defaults.model || '', Validators.required],
      yearFrom: [this.defaults.yearFrom || 0, Validators.required],
      yearTo: [this.defaults.yearTo || 0, Validators.required],
      cylinder: [this.defaults.cylinder || 0, Validators.required],
      liter: [this.defaults.liter || 0, Validators.required],
      brand: [this.defaults.brand || ''],
      position: [this.defaults.position || ''],
      notes: [this.defaults.notes || ''],
      groupHead: [this.defaults.groupHead || ''],
      subGroup: [this.defaults.subGroup || ''],
      subModel: [this.defaults.subModel || ''],
    });
  }

  save() {
    if (this.mode === 'create') {
      if (this.form.valid) {
        this.alertService.createNotification("Parts Catalog").then(answer => {
          if (!answer.isConfirmed) return;
          this.createPartsCatalog();
        });
      }
      else this.alertService.validationNotification("Parts Catalog");
    }
    else if (this.mode === 'update') {
      if (this.form.valid) {
        this.alertService.updateNotification("Parts Catalog").then(answer => {
          if (!answer.isConfirmed) return;
          this.updatePartsCatalog();
        });
      }
      else this.alertService.validationNotification("Parts Catalog");
    }
  }

  createPartsCatalog() {
    const partsCatalog = {} as PartsCatalog;
    partsCatalog.make = this.form.value.make;
    partsCatalog.model = this.form.value.model;
    partsCatalog.yearFrom = this.form.value.yearFrom;
    partsCatalog.yearTo = this.form.value.yearTo;
    partsCatalog.cylinder = this.form.value.cylinder;
    partsCatalog.liter = this.form.value.liter;
    partsCatalog.brand = this.form.value.brand;
    partsCatalog.notes = this.form.value.notes;
    partsCatalog.position = this.form.value.position;
    partsCatalog.groupHead = this.form.value.groupHead;
    partsCatalog.subGroup = this.form.value.subGroup;
    partsCatalog.subModel = this.form.value.subModel;

    partsCatalog.productId = this.productId;
    partsCatalog.partNumber = this.productPartNumber;

    partsCatalog.isActive = true;
    partsCatalog.isDeleted = false;
    partsCatalog.createdBy = "demo@user.com";
    partsCatalog.createdDate = moment(new Date());



    this.dialogRef.close(partsCatalog);
  }

  updatePartsCatalog() {
    const partsCatalog = {} as PartsCatalog;
    partsCatalog.make = this.form.value.make;
    partsCatalog.model = this.form.value.model;
    partsCatalog.yearFrom = this.form.value.yearFrom;
    partsCatalog.yearTo = this.form.value.yearTo;
    partsCatalog.cylinder = this.form.value.cylinder;
    partsCatalog.liter = this.form.value.liter;
    partsCatalog.brand = this.form.value.brand;
    partsCatalog.notes = this.form.value.notes;
    partsCatalog.position = this.form.value.position;
    partsCatalog.groupHead = this.form.value.groupHead;
    partsCatalog.subGroup = this.form.value.subGroup;
    partsCatalog.subModel = this.form.value.subModel;

    partsCatalog.productId = this.defaults.productId;
    partsCatalog.partNumber = this.defaults.partNumber;

    partsCatalog.isActive = this.defaults.isActive;
    partsCatalog.isDeleted = this.defaults.isDeleted;
    partsCatalog.createdBy = this.defaults.createdBy;
    partsCatalog.createdDate = this.defaults.createdDate;
    partsCatalog.modifiedBy = "modify@user.com";
    partsCatalog.modifiedDate = moment(new Date());
    partsCatalog.id = this.defaults.id;

    this.dialogRef.close(partsCatalog);
  }

  isCreateMode() {
    return this.mode === 'create';
  }

  isUpdateMode() {
    return this.mode === 'update';
  }
}
